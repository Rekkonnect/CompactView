/**************************************************************************
Copyright (C) 2011-2015 Iván Costales Suárez

This file is part of CompactView.

CompactView is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

CompactView is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with CompactView.  If not, see <http://www.gnu.org/licenses/>.

CompactView web site <http://sourceforge.net/p/compactview/>.
**************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CompactView
{
    public class SqlCeBase : IDisposable
    {

        /// <summary>
        /// Initialize a new instance of SqlCeDb
        /// </summary>
        public SqlCeBase()
        {
            LastError = string.Empty;
            regexSemicolon.Match(string.Empty);
        }

        public static Version[] AvailableVersions { get; } = {   // List of available versions starting with the oldest
            // new Version("2.0", "2.0.0.0", 0x73616261),
            new Version("3.1", "9.0.242.0", 0x002dd714),
            new Version("3.5", "3.5.0.0", 0x00357b9d),
            new Version("4.0", "4.0.0.0", 0x003d0900)
        };
        public SqlCeConnection Connection { get; private set; }
        public string FileName { get; private set; }
        public string Password { get; private set; }
        public string LastError { get; private set; }
        public Version Version { get; private set; }
        public bool BadPassword { get; private set; }
        public int QueryCount { get; private set; }

        // Regular expression to search texts finished with semicolons that is not between single quotes
        private Regex regexSemicolon = new Regex("(?:[^;']|'[^']*')+", RegexOptions.Compiled | RegexOptions.Multiline);

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Open the specified database file
        /// </summary>
        /// <param name="databaseFile">Database file name to open</param>
        public bool Open(string databaseFile)
        {
            return Open(databaseFile, null);
        }

        /// <summary>
        /// Open the specified database file
        /// </summary>
        /// <param name="databaseFile">Database file name to open</param>
        /// <param name="password">Password of database file</param>
        public bool Open(string databaseFile, string password)
        {
            Close();
            FileName = Path.GetFullPath(databaseFile);
            Password = password;

            bool ok = false;
            Version version = GetSdfVersion(FileName);
            if (version == null)
            {
                foreach (var ver in AvailableVersions.Reverse())
                {
                    ok = OpenConnection(ver, databaseFile, password);
                    if (ok || BadPassword)
                        break;
                }
            }
            else
            {
                ok = OpenConnection(version, databaseFile, password);
                if (!ok && (!BadPassword || password != null))
                    GlobalText.ShowError("UnableToOpen", LastError);
            }
            return ok;
        }

        public void Close()
        {
            if (Connection != null && Connection.State != ConnectionState.Closed)
                Connection.Close();
            Connection = null;
            Version = null;
            LastError = string.Empty;
            FileName = string.Empty;
            Password = string.Empty;
            BadPassword = false;
            _tableNames = null;
            _databaseInfo = null;
        }

        public bool IsOpen => Connection?.State == ConnectionState.Open;

        private DataTable _databaseInfo;

        public DataTable DatabaseInfo
        {
            get
            {
                if (_databaseInfo != null)
                    return _databaseInfo;
                if (Connection == null)
                    return null;
                _databaseInfo = new DataTable();
                _databaseInfo.Columns.Add("Property");
                _databaseInfo.Columns.Add("Value");
                _databaseInfo.Rows.Add("Version", Version);
                _databaseInfo.Rows.Add("File Path", FileName);
                _databaseInfo.Rows.Add("File Size", GetFileSize(new FileInfo(FileName).Length));

                try
                {
                    var dbInfo = Connection.GetDatabaseInfo();
                    foreach (var key in dbInfo)
                    {
                        var titleCase = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(key.Key);
                        _databaseInfo.Rows.Add(titleCase, key.Value);
                    }
                }
                catch
                {
                }

                try
                {
                    DbCommand cmd = Connection.CreateCommand();

                    AddDatabaseInfo(cmd, "Tables", "TABLES");
                    AddDatabaseInfo(cmd, "Indexes", "INDEXES");
                    AddDatabaseInfo(cmd, "Keys", "KEY_COLUMN_USAGE");
                    AddDatabaseInfo(cmd, "Table Constraints", "TABLE_CONSTRAINTS");
                    AddDatabaseInfo(cmd, "Foreign Constraints", "REFERENTIAL_CONSTRAINTS");

                    return _databaseInfo;
                }
                catch
                {
                    return null;
                }
            }
        }

        private void AddDatabaseInfo(DbCommand cmd, string propertyName, string tableName)
        {
            cmd.CommandText = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.{tableName}";
            _databaseInfo.Rows.Add(propertyName, cmd.ExecuteScalar().ToString());
        }

        private string GetFileSize(long bytes)
        {
            string[] size = { "Bytes", "KB", "MB", "GB", "TB" };
            int log = (int)Math.Log(0.1 + bytes, 1024);
            if (log > 4)
                log = 4;
            double n = bytes / Math.Pow(1024, log);
            return $"{n:N0} {size[log]}";
        }

        private List<string> _tableNames = null;

        public List<string> TableNames
        {
            get
            {
                if (_tableNames != null)
                    return _tableNames;
                _tableNames = new List<string>();
                using (DbDataReader dr = ExecuteReader("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = N'TABLE'"))
                {
                    while (dr.Read())
                        _tableNames.Add(dr.GetString(0));
                }
                return _tableNames;
            }
        }

        protected void ResetTableNames()
        {
            _tableNames?.Clear();
            _tableNames = null;
        }

        public IEnumerable<Match> GetSqlStatements(string sql)
        {
            foreach (Match match in regexSemicolon.Matches(sql))
            {
                if (!string.IsNullOrWhiteSpace(match.Value))
                {
                    yield return match;
                }
            }
        }

        public object ExecuteSql(string sql, bool updatable)
        {
            if (Connection == null)
                return null;

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            LastError = string.Empty;

            var command = new SqlCeCommand(null, Connection);

            ResultSetOptions options = updatable ? ResultSetOptions.Scrollable | ResultSetOptions.Updatable : ResultSetOptions.Scrollable;

            object result = null;
            QueryCount = 0;

            var matches = GetSqlStatements(sql);

            foreach (var m in matches)
            {
                QueryCount++;
                try
                {
                    command.CommandText = m.Value.Trim();
                    var resultset = command.ExecuteResultSet(options);
                    bool scrollable = resultset.Scrollable;
                    if (scrollable)
                        result = resultset;
                }
                catch (Exception e)
                {
                    LastError = $"{GlobalText.GetValue("Query")} {QueryCount}: {(e.InnerException == null ? e.Message : e.InnerException.Message)}";
                    return null;
                }
            }

            return result;
        }

        public bool Compact(string databaseFile)
        {
            return Compact(databaseFile, null, null);
        }

        public bool Compact(string databaseFile, string password, string newPassword)
        {
            string connectionStr = password == newPassword
                ? null
                : $"Data Source=; Password={(newPassword ?? string.Empty)}";

            return OperateEngine(
                databaseFile,
                password,
                engine => engine.Compact(connectionStr));
        }

        public bool Repair(string databaseFile)
        {
            return Repair(databaseFile, null, null);
        }

        public bool Repair(string databaseFile, string password, string newPassword)
        {
            string connectionStr = password == newPassword
                ? null
                : $"Data Source={databaseFile}; Password={(newPassword ?? string.Empty)}";

            bool ok = DoRepair(databaseFile, password, connectionStr, RepairOption.RecoverAllPossibleRows);

            if (ok)
            {
                ok = DoRepair(databaseFile, newPassword, connectionStr, RepairOption.DeleteCorruptedRows);
            }

            return ok;
        }

        private bool DoRepair(string databaseFile, string password, string repairConnectionString, RepairOption repairOptions)
        {
            Close();
            if (!Open(databaseFile, password))
                return false;
            Close();

            string connectionStr = GetConnectionString(databaseFile, password);

            try
            {
                var engine = new SqlCeEngine(connectionStr);
                engine.Repair(repairConnectionString, repairOptions);
                engine.Dispose();
                return true;
            }
            catch (Exception e)
            {
                LastError = (e.InnerException == null) ? e.Message : e.InnerException.Message;
                return false;
            }
        }

        public bool Shrink(string databaseFile)
        {
            return Shrink(databaseFile, null, null);
        }

        public bool Shrink(string databaseFile, string password, string newPassword)
        {
            string connectionStr = password == newPassword
                ? null
                : $"Data Source=; Password={(newPassword ?? string.Empty)}";

            return OperateEngine(
                databaseFile,
                password,
                engine => engine.Shrink());
        }

        public bool Upgrade(string databaseFile, Version toVersion)
        {
            return Upgrade(databaseFile, null, null, toVersion);
        }

        public bool Upgrade(string databaseFile, string password, string newPassword, Version toVersion)
        {
            Close();
            OpenConnection(toVersion, databaseFile, password);
            Close();

            string newConnectionStr = password == newPassword ? null : $"Data Source=; Password={(newPassword ?? string.Empty)}";
            string connectionStr = GetConnectionString(databaseFile, password);

            try
            {
                var engine = new SqlCeEngine(connectionStr);
                engine.Upgrade(newConnectionStr);
                return true;
            }
            catch (Exception e)
            {
                LastError = (e.InnerException == null) ? e.Message : e.InnerException.Message;
                return false;
            }
        }

        public bool Verify(string databaseFile)
        {
            return Verify(databaseFile, null);
        }

        public bool Verify(string databaseFile, string password)
        {
            Close();
            if (!Open(databaseFile, password))
                return false;
            Close();

            string connectionStr = GetConnectionString(databaseFile, password);

            try
            {
                LastError = string.Empty;
                var engine = new SqlCeEngine(connectionStr);
                return engine.Verify();
            }
            catch (Exception e)
            {
                LastError = (e.InnerException == null) ? e.Message : e.InnerException.Message;
                return false;
            }
        }

        public bool CreateDatabase(string databaseFile, Version version)
        {
            return CreateDatabase(databaseFile, null, int.MinValue, version);
        }

        public bool CreateDatabase(string databaseFile, string password, Version version)
        {
            return CreateDatabase(databaseFile, password, int.MinValue, version);
        }

        public bool CreateDatabase(string databaseFile, string password, int lcid, Version version)
        {
            try
            {
                OpenConnection(version, databaseFile, password);
            }
            catch
            {
            }
            Close();

            string connectionStr = $"Data Source={databaseFile}";
            if (!string.IsNullOrEmpty(password))
                connectionStr += $"; Password={password}; Encrypt=TRUE";
            if (lcid != int.MinValue)
                connectionStr += $"; LCID={lcid}";
            try
            {
                var engine = new SqlCeEngine(connectionStr);
                engine.CreateDatabase();
                return true;
            }
            catch (Exception e)
            {
                LastError = (e.InnerException == null) ? e.Message : e.InnerException.Message;
                return false;
            }
        }

        private DbDataReader ExecuteReader(string sql)
        {
            DbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            return cmd.ExecuteReader();
        }

        public string GetColumnDataType(string table, string column)
        {
            DbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = $"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{table}' AND COLUMN_NAME='{column}'";
            DbDataReader dr = cmd.ExecuteReader();
            string s = dr.Read() ? dr.GetString(0) : string.Empty;
            dr.Close();
            return s;
        }

        private bool OperateEngine(string databaseFile, string password, Action<SqlCeEngine> engineAction)
        {
            Close();
            if (!Open(databaseFile, password))
                return false;
            Close();

            string connectionStr = GetConnectionString(databaseFile, password);

            try
            {
                var engine = new SqlCeEngine(connectionStr);
                engineAction?.Invoke(engine);
                return true;
            }
            catch (Exception e)
            {
                LastError = (e.InnerException == null) ? e.Message : e.InnerException.Message;
                return false;
            }
        }


        private bool OpenConnection(Version version, string databaseFile, string password)
        {
            Connection = null;
            Version = null;

            string connectionStr = GetConnectionString(databaseFile, password);

            try
            {
                Connection = new SqlCeConnection(connectionStr);
                if (Connection == null)
                    return false;
                Connection.Open();
                Version = version;
                return true;
            }
            catch (Exception e)
            {
                int nativeError = int.MinValue;
                try
                {
                    var ex = e.GetBaseException() as SqlCeException;
                    nativeError = ex.NativeError;
                }
                catch
                {
                }
                switch (nativeError)
                {
                    case SqlCeNativeErrors.OldVersion:
                        break;
                    case SqlCeNativeErrors.BadPassword:
                        BadPassword = true;
                        break;
                    case SqlCeNativeErrors.IncorrectDatabaseVersion:
                        break;
                    default:
                        throw e;
                }
                LastError = e.Message;
                return false;
            }
        }

        private string GetConnectionString(string databaseFile, string password)
        {
            string connectionStr = $"Data Source=\"{databaseFile}\"; Max Database Size=4091";
            // Maximum 4 Gb (with value 4096 the program is not able to open databases above 2045 Mb)
            if (!string.IsNullOrEmpty(password))
                connectionStr += $"; Password=\"{password}\"";

            bool readOnly = false;
            try
            {
                var fileInfo = new FileInfo(databaseFile);
                readOnly = (fileInfo.Attributes & FileAttributes.ReadOnly) != 0;
            }
            catch
            {
            }
            if (readOnly)
                connectionStr += $"; Mode=Read Only; Temp Path=\"{Path.GetTempPath()}\"";

            return connectionStr;
        }

        private Version GetSdfVersion(string filePath)
        {
            UInt32 sdfCodeVersion;
            using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                reader.BaseStream.Seek(16, SeekOrigin.Begin);
                sdfCodeVersion = reader.ReadUInt32();
            }
            return AvailableVersions.FirstOrDefault(version => version.SdfCodeVersion == sdfCodeVersion);
        }
    }
}
