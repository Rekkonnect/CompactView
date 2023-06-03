/**************************************************************************
Copyright (C) 2023 Rekkonnect

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
using System.Data.Entity;
using System.Data.SqlServerCe;

namespace CompactView
{
    public readonly struct TableColumnName : IEquatable<TableColumnName>
    {
        public string TableName { get; }
        public string ColumnName { get; }

        public TableColumnName(string tableName, string columnName)
        {
            TableName = tableName;
            ColumnName = columnName;
        }

        public override string ToString()
        {
            return TableName + "." + ColumnName;
        }

        public bool Equals(TableColumnName other)
        {
            return TableName == other.TableName
                && ColumnName == other.ColumnName;
        }

        public override bool Equals(object obj)
        {
            return obj is TableColumnName
                && Equals((TableColumnName)obj);
        }

        public override int GetHashCode()
        {
            return TableName.GetHashCode() ^ ColumnName.GetHashCode();
        }
    }

    public class InformationSchemaSqlCeContext : DbContext
    {
        public DbSet<InformationSchema.Table> Tables { get; set; }
        public DbSet<InformationSchema.Index> Indexes { get; set; }
        public DbSet<InformationSchema.Column> Columns { get; set; }
        public DbSet<InformationSchema.KeyColumnUsage> KeyColumnUsages { get; set; }
        public DbSet<InformationSchema.TableConstraint> TableConstraints { get; set; }
        public DbSet<InformationSchema.ReferentialConstraint> ReferentialConstraints { get; set; }

        public InformationSchemaSqlCeContext(string connectionString)
            : base(connectionString) { }
        public InformationSchemaSqlCeContext(SqlCeConnection connection)
            : base(connection, false) { }
    }
}
