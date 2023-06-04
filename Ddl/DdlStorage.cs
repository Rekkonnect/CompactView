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

using System.Collections.Generic;
using System.Linq;

namespace CompactView.Ddl
{
    public class DdlStorage
    {
        private SchemaDdl _databaseDdl;
        private readonly Dictionary<string, SchemaDdl> _tableDdls = new Dictionary<string, SchemaDdl>();

        public SchemaDdl DatabaseDdl => _databaseDdl;

        public SchemaDdl TableDdl(string tableName)
        {
            return _tableDdls[tableName];
        }

        public void Clear()
        {
            _tableDdls.Clear();
            _databaseDdl = null;
        }

        public void Build(InformationSchemaCache informationSchemaCache)
        {
            // First build all the table DDLs for each individual table
            // Then continue by consecutively buildling off the entire schema DDL
            // By appending already calculated DDLs

            foreach (var tableName in informationSchemaCache.TableNames)
            {
                BuildTableDdl(informationSchemaCache, tableName);
            }

            ConstructDatabaseDdl();
        }

        private void BuildTableDdl(InformationSchemaCache cache, string tableName)
        {
            var schemaDdl = new SchemaDdl(SchemaObjectType.Table, tableName);
            _tableDdls[tableName] = schemaDdl;

            var tableColumns = cache.ColumnsByTable[tableName];
            schemaDdl.LoadColumns(tableName, tableColumns);

            var tableConstraintInfo = cache.TableConstraintInfoForTable(tableName);
            schemaDdl.LoadPrimaryKeys(tableConstraintInfo);

            var indexInfo = cache.IndexInfoForTable(tableName);
            schemaDdl.LoadIndexes(indexInfo);

            var referentialConstraintInfo = cache.ReferentialConstraintInfoForTable(tableName);
            schemaDdl.LoadForeignKeys(referentialConstraintInfo);

            schemaDdl.FinalizeBuilders();
        }

        private void ConstructDatabaseDdl()
        {
            var databaseDdlBuilder = new SchemaDdl.Builder(SchemaObjectType.Database);

            var allConstructedDdls = _tableDdls.Values.ToArray();

            foreach (var tableDdl in allConstructedDdls)
            {
                databaseDdlBuilder.ColumnsDdl.Builder.Append(tableDdl.ColumnsDdl.Sql);
                databaseDdlBuilder.PrimaryKeysDdl.Builder.Append(tableDdl.PrimaryKeysDdl.Sql);
                databaseDdlBuilder.IndexesDdl.Builder.Append(tableDdl.IndexesDdl.Sql);
                databaseDdlBuilder.ForeignKeysDdl.Builder.Append(tableDdl.ForeignKeysDdl.Sql);
            }

            _databaseDdl = databaseDdlBuilder.Build();
        }
    }
}
