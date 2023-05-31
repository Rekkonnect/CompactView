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
using System.Collections.Specialized;

namespace CompactView.Lexing
{
    public partial class SqlLexer
    {
        public static class KnownWords
        {
            private static readonly string[] keywordsArray = new[]
            {
                "ACTION", "ADD", "ALL", "ALTER", "AND", "ANY", "AS", "ASC", "AUTHORIZATION", "AVG", "BACKUP", "BEGIN", "BETWEEN", "BREAK", "BROWSE", "BULK", "BY", "CASCADE",
                "CASE", "CHECK", "CHECKPOINT", "CLOSE", "CLUSTERED", "COALESCE", "COLLATE", "COLUMN", "COMMIT", "COMPUTE", "CONSTRAINT", "CONTAINS", "CONTAINSTABLE",
                "CONTINUE", "CONVERT", "COUNT", "CREATE", "CROSS", "CURRENT", "CURRENT_DATE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER", "CURSOR",
                "DATABASE", "DATABASEPASSWORD", "DATEADD", "DATEDIFF", "DATENAME", "DATEPART", "DBCC", "DEALLOCATE", "DECLARE", "DEFAULT", "DELETE", "DENY", "DESC",
                "DISK", "DISTINCT", "DISTRIBUTED", "DOUBLE", "DROP", "DUMP", "ELSE", "ENCRYPTION", "END", "ERRLVL", "ESCAPE", "EXCEPT", "EXEC", "EXECUTE", "EXISTS",
                "EXIT", "EXPRESSION", "FETCH", "FILE", "FILLFACTOR", "FOR", "FOREIGN", "FREETEXT", "FREETEXTTABLE", "FROM", "FULL", "FUNCTION", "GOTO", "GRANT",
                "GROUP", "HAVING", "HOLDLOCK", "IDENTITY", "IDENTITY_INSERT", "IDENTITYCOL", "IF", "IN", "INDEX", "INNER", "INSERT", "INTERSECT", "INTO", "IS",
                "JOIN", "KEY", "KILL", "LEFT", "LIKE", "LINENO", "LOAD", "MAX", "MIN", "NATIONAL", "NO", "NOCHECK", "NONCLUSTERED", "NOT", "NULL", "NULLIF", "OF", "OFF",
                "OFFSETS", "ON", "OPEN", "OPENDATASOURCE", "OPENQUERY", "OPENROWSET", "OPENXML", "OPTION", "OR", "ORDER", "OUTER", "OVER", "PERCENT", "PLAN",
                "PRECISION", "PRIMARY", "PRINT", "PROC", "PROCEDURE", "PUBLIC", "RAISERROR", "READ", "READTEXT", "RECONFIGURE", "REFERENCES", "REPLICATION",
                "RESTORE", "RESTRICT", "RETURN", "REVOKE", "RIGHT", "ROLLBACK", "ROWCOUNT", "ROWGUIDCOL", "RULE", "SAVE", "SCHEMA", "SELECT", "SESSION_USER", "SET",
                "SETUSER", "SHUTDOWN", "SOME", "STATISTICS", "SUM", "SYSTEM_USER", "TABLE", "TEXTSIZE", "THEN", "TO", "TOP", "TRAN", "TRANSACTION", "TRIGGER",
                "TRUNCATE", "TSEQUAL", "UNION", "UNIQUE", "UPDATE", "UPDATETEXT", "USE", "USER", "VALUES", "VARYING", "VIEW", "WAITFOR", "WHEN", "WHERE", "WHILE",
                "WITH", "WRITETEXT",
            };
            private static readonly string[] typesArray = new[]
            {
                "BIGINT", "INT", "INTEGER", "SMALLINT", "TINYINT", "BIT", "NUMERIC", "DECIMAL", "DEC", "MONEY", "FLOAT", "REAL", "DATETIME", "NATIONAL CHARACTER",
                "NCHAR", "NATIONAL CHARACTER VARYING", "NVARCHAR", "NTEXT", "BINARY", "VARBINARY", "IMAGE", "UNIQUEIDENTIFIER", "TIMESTAMP", "ROWVERSION"
            };
            private static readonly string[] functionsArray = new[]
            {
                // String Functions
                "NCHAR",
                "CHARINDEX",
                "LEN",
                "LOWER",
                "LTRIM",
                "PATINDEX",
                "REPLACE",
                "REPLICATE",
                "RTRIM",
                "SPACE",
                "STR",
                "STUFF",
                "SUBSTRING",
                "UNICODE",
                "UPPER",

                // Math Functions
                "ABS",
                "ACOS",
                "ASIN",
                "ATAN",
                "ATN2",
                "CEILING",
                "COS",
                "COT",
                "DEGREES",
                "EXP",
                "FLOOR",
                "LOG",
                "LOG10",
                "PI",
                "POWER",
                "RADIANS",
                "RAND",
                "ROUND",
                "SIGN",
                "SIN",
                "SQRT",
                "TAN",

                // Date Functions
                "DATEDIFF",
                "DATEPART",
                "GETDATE",

                // Advanced Functions
                "COALESCE",
                "DATALENGTH",
                "@@IDENTITY",
            };

            public static readonly StringCollection Keywords;
            public static readonly StringCollection Types;
            public static readonly StringCollection Functions;

            public static readonly StringCollection All;

            static KnownWords()
            {
                Keywords = StringCollectionFromRange(keywordsArray);
                Types = StringCollectionFromRange(typesArray);
                Functions = StringCollectionFromRange(functionsArray);

                foreach (var keyword in keywordsArray)
                {
                    Functions.Remove(keyword);
                }

                All = new StringCollection();
                All.AddRange(keywordsArray);
                All.AddRange(typesArray);
                All.AddRange(functionsArray);
            }

            private static StringCollection StringCollectionFromRange(string[] range)
            {
                var result = new StringCollection();
                result.AddRange(range);
                return result;
            }
        }
    }
}
