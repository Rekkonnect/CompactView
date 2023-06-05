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

using CompactView.Lexing;
using Garyon.Extensions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace CompactView.Ddl
{
    public class SchemaDdl
    {
        private readonly FinalizableSqlStringBuilder _entireDdl = new FinalizableSqlStringBuilder();

        private readonly FinalizableSqlStringBuilder _columns = new FinalizableSqlStringBuilder();
        private readonly FinalizableSqlStringBuilder _primaryKeys = new FinalizableSqlStringBuilder();
        private readonly FinalizableSqlStringBuilder _indexes = new FinalizableSqlStringBuilder();
        private readonly FinalizableSqlStringBuilder _foreignKeys = new FinalizableSqlStringBuilder();

        public SchemaObjectType ObjectType { get; set; }

        public string Name { get; set; }

        public SqlString EntireDdl => _entireDdl.SqlString;
        public SqlString ColumnsDdl => _columns.SqlString;
        public SqlString PrimaryKeysDdl => _primaryKeys.SqlString;
        public SqlString IndexesDdl => _indexes.SqlString;
        public SqlString ForeignKeysDdl => _foreignKeys.SqlString;

        public SchemaDdl(SchemaObjectType objectType, string name = null)
        {
            ObjectType = objectType;
            Name = name;
        }

        public void LoadColumns(string tableName, IReadOnlyList<InformationSchema.Column> columns)
        {
            DdlFactory.Shared.AppendTable(_columns.Builder, tableName, columns);
            _columns.Builder.AppendLineCount(2);
        }
        public void LoadIndexes(IReadOnlyList<IndexInfo> indexes)
        {
            foreach (var index in indexes)
            {
                if (index.FirstColumn.PrimaryKey)
                    continue;

                DdlFactory.Shared.AppendIndexInfo(_indexes.Builder, index);
                _indexes.Builder.AppendLineCount(2);
            }
        }
        public void LoadPrimaryKeys(IReadOnlyList<TableConstraintInfo> constraints)
        {
            foreach (var constraint in constraints)
            {
                if (constraint.TableConstraint.ConstraintTypeValue != InformationSchema.ConstraintType.PrimaryKey)
                    continue;

                DdlFactory.Shared.AppendPrimaryKeyInfo(_primaryKeys.Builder, constraint);
                _primaryKeys.Builder.AppendLineCount(2);
            }
        }
        public void LoadForeignKeys(IReadOnlyList<ReferentialConstraintInfo> constraints)
        {
            foreach (var constraint in constraints)
            {
                DdlFactory.Shared.AppendForeignKeyInfo(_foreignKeys.Builder, constraint);
                _foreignKeys.Builder.AppendLineCount(2);
            }
        }

        public void FinalizeBuilders()
        {
            _columns.FinalizeBuilder();
            _primaryKeys.FinalizeBuilder();
            _indexes.FinalizeBuilder();
            _foreignKeys.FinalizeBuilder();

            _entireDdl.Clear();
            _entireDdl.Builder
                .Append(_columns.FinalizedString)
                .Append(_primaryKeys.FinalizedString)
                .Append(_indexes.FinalizedString)
                .Append(_foreignKeys.FinalizedString);

            _entireDdl.FinalizeBuilder();
        }

        public class Builder
        {
            private readonly SchemaDdl _schemaDdl;

            internal FinalizableSqlStringBuilder EntireDdl => _schemaDdl._entireDdl;
            internal FinalizableSqlStringBuilder ColumnsDdl => _schemaDdl._columns;
            internal FinalizableSqlStringBuilder PrimaryKeysDdl => _schemaDdl._primaryKeys;
            internal FinalizableSqlStringBuilder IndexesDdl => _schemaDdl._indexes;
            internal FinalizableSqlStringBuilder ForeignKeysDdl => _schemaDdl._foreignKeys;

            public Builder(SchemaObjectType objectType, string name = null)
            {
                _schemaDdl = new SchemaDdl(objectType, name);
            }

            public SchemaDdl Build()
            {
                _schemaDdl.FinalizeBuilders();
                return _schemaDdl;
            }
        }
    }

    public class DdlFactory
    {
        public static DdlFactory Shared { get; } = new DdlFactory();

        public bool AvoidIdentifierBrackets { get; set; }

        public void AppendIdentifier(StringBuilder builder, string identifier)
        {
            bool useBrackets = true;

            if (AvoidIdentifierBrackets)
            {
                useBrackets = SqlLexer.KnownWords.All.Contains(identifier);
            }

            if (useBrackets)
            {
                AppendBracketedIdentifier(builder, identifier);
            }
            else
            {
                builder.Append(identifier);
            }
        }
        public void AppendBracketedIdentifier(StringBuilder builder, string identifier)
        {
            builder
                .Append('[')
                .Append(identifier)
                .Append(']');
        }

        public void AppendTable(
            StringBuilder builder,
            string tableName,
            IReadOnlyList<InformationSchema.Column> column)
        {
            builder.Append("CREATE TABLE ");
            AppendIdentifier(builder, tableName);
            builder
                .AppendLine()
                .AppendLine('(');

            for (int i = 0; i < column.Count; i++)
            {
                AppendColumnInfo(builder, column[i]);
                if (i < column.Count - 1)
                {
                    builder.AppendLine(',');
                }
            }

            builder
                .AppendLine()
                .Append(");");
        }
        public void AppendColumnInfo(StringBuilder builder, InformationSchema.Column column)
        {
            builder.Append(' ', 4);

            AppendIdentifier(builder, column.ColumnName);

            builder.Append(' ');
            builder.AppendUpper(column.DataType);

            AppendSpecificTypeProperties(builder, column);

            if (!column.IsNullable)
            {
                builder.Append(" NOT NULL");
            }
            if (column.HasDefaultValue)
            {
                builder.Append(" DEFAULT ");
                builder.Append(column.DefaultValue);
            }
            if (column.IsRowGuidCol)
            {
                builder.Append(" ROWGUIDCOL");
            }
            if (column.AutoIncrementBy > 0)
            {
                builder
                    .Append(" IDENTITY (")
                    .Append(column.AutoIncrementSeed)
                    .Append(',')
                    .Append(column.AutoIncrementBy)
                    .Append(')');
            }
        }
        private void AppendSpecificTypeProperties(
            StringBuilder builder,
            InformationSchema.Column column)
        {
            switch (column.DataType)
            {
                case "nvarchar":
                case "nchar":
                case "binary":
                case "varbinary":
                    builder
                        .Append('(')
                        .Append(column.CharacterMaximumLength)
                        .Append(')');
                    break;

                case "decimal":
                case "numeric":
                    builder
                        .Append('(')
                        .Append(column.NumericPrecision)
                        .Append(',')
                        .Append(column.NumericScale)
                        .Append(')');
                    break;
            }
        }

        public void AppendPrimaryKeyInfo(StringBuilder builder, TableConstraintInfo tableConstraintInfo)
        {
            AssertPrimaryKeyConstraint(tableConstraintInfo.TableConstraint);

            string tableName = tableConstraintInfo.TableName;
            string constraintName = tableConstraintInfo.ConstraintName;

            builder.Append("ALTER TABLE ");
            AppendIdentifier(builder, tableName);
            builder.Append(" ADD CONSTRAINT ");
            AppendIdentifier(builder, constraintName);
            builder.Append(" PRIMARY KEY (");

            for (int i = 0; i < tableConstraintInfo.ColumnUsages.Count; i++)
            {
                var primaryKey = tableConstraintInfo.ColumnUsages[i];
                AppendIdentifier(builder, primaryKey.ColumnName);

                if (i < tableConstraintInfo.ColumnUsages.Count - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(");");
        }

        public void AppendForeignKeyInfo(StringBuilder builder, ReferentialConstraintInfo referentialConstraint)
        {
            AssertPrimaryKeyConstraint(referentialConstraint.UniqueTableConstraintInfo.TableConstraint);

            builder.Append("ALTER TABLE ");
            AppendIdentifier(builder, referentialConstraint.Constraint.ConstraintTableName);
            builder.AppendLine();
            builder.Append("ADD CONSTRAINT ");
            AppendIdentifier(builder, referentialConstraint.ConstraintName);
            builder.AppendLine();
            builder.Append("FOREIGN KEY (");

            for (int i = 0; i < referentialConstraint.ColumnUsages.Count; i++)
            {
                var referentialConstraintColumn = referentialConstraint.ColumnUsages[i];
                AppendIdentifier(builder, referentialConstraintColumn.ColumnName);

                if (i < referentialConstraint.ColumnUsages.Count - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.AppendLine(')');
            builder.Append("REFERENCES ");
            AppendIdentifier(builder, referentialConstraint.Constraint.UniqueConstraintTableName);

            builder.Append(" (");

            for (int i = 0; i < referentialConstraint.TableConstraintColumnUsages.Count; i++)
            {
                var uniqueConstraintColumn = referentialConstraint.TableConstraintColumnUsages[i];
                AppendIdentifier(builder, uniqueConstraintColumn.ColumnName);

                if (i < referentialConstraint.TableConstraintColumnUsages.Count - 1)
                {
                    builder.Append(", ");
                }
            }

            builder
                .AppendLine(')')
                .Append("ON DELETE ")
                .Append(referentialConstraint.Constraint.DeleteRuleName)
                .AppendLine()
                .Append("ON UPDATE ")
                .Append(referentialConstraint.Constraint.UpdateRuleName)
                .Append(';');
        }

        public void AppendIndexInfo(StringBuilder builder, IndexInfo indexInfo)
        {
            string tableName = indexInfo.TableName;
            string indexName = indexInfo.IndexName;

            builder.Append("CREATE ");
            if (indexInfo.FirstColumn.Unique)
            {
                builder.Append("UNIQUE ");
            }
            builder.Append("INDEX ");
            AppendIdentifier(builder, indexName);
            builder.Append(" ON ");
            AppendIdentifier(builder, tableName);
            builder.Append(" (");

            for (int i = 0; i < indexInfo.ColumnIndexes.Count; i++)
            {
                var index = indexInfo.ColumnIndexes[i];
                AppendIdentifier(builder, index.ColumnName);

                // Always assume a present sort order
                // if there is none, we won't mind a stray space
                builder.Append(' ');
                AppendSortOrder(builder, index.SortOrder);

                if (i < indexInfo.ColumnIndexes.Count - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(");");
        }

        private void AppendSortOrder(StringBuilder builder, SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    builder.Append("ASC");
                    break;
                case SortOrder.Descending:
                    builder.Append("DESC");
                    break;
            }
        }

        private static void AssertPrimaryKeyConstraint(InformationSchema.TableConstraint tableConstraint)
        {
            Debug.Assert(tableConstraint.ConstraintTypeValue
                is InformationSchema.ConstraintType.PrimaryKey);
        }
    }
}
