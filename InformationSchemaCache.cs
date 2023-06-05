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

using Garyon.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;

namespace CompactView
{
    public class InformationSchemaCache
    {
        private StringCollection tableNames;
        private ListValueStringDictionary<InformationSchema.Index> indexesByTable;
        private ListValueStringDictionary<InformationSchema.Column> columnsByTable;
        private ListValueStringDictionary<InformationSchema.KeyColumnUsage> keyColumnUsagesByTable;
        private ListValueStringDictionary<InformationSchema.TableConstraint> tableConstraintsByTable;
        private ListValueStringDictionary<InformationSchema.ReferentialConstraint> referentialConstraintsByTable;

        private Dictionary<string, TableConstraintInfo> tableConstraintInfoByName;
        private Dictionary<string, ReferentialConstraintInfo> referentialConstraintInfoByName;
        private Dictionary<QualifiedObjectName, IndexInfo> indexInfoByName;

        public IEnumerable<string> TableNames
            => tableNames.Cast<string>();

        public IReadOnlyDictionary<string, IReadOnlyList<InformationSchema.Index>> IndexesByTable
            => indexesByTable;

        public IReadOnlyDictionary<string, IReadOnlyList<InformationSchema.Column>> ColumnsByTable
            => columnsByTable;

        public IReadOnlyDictionary<string, IReadOnlyList<InformationSchema.KeyColumnUsage>> KeyColumnUsagesByTable
            => keyColumnUsagesByTable;

        public IReadOnlyDictionary<string, IReadOnlyList<InformationSchema.TableConstraint>> TableConstraintsByTable
            => tableConstraintsByTable;

        public IReadOnlyDictionary<string, IReadOnlyList<InformationSchema.ReferentialConstraint>> ReferentialConstraintsByTable
            => referentialConstraintsByTable;

        public IReadOnlyDictionary<string, TableConstraintInfo> TableConstraintInfoByName
            => tableConstraintInfoByName;

        public IReadOnlyDictionary<string, ReferentialConstraintInfo> ReferentialConstraintInfoByName
            => referentialConstraintInfoByName;

        public SqlCeInformationSchemaData SchemaData { get; }

        public InformationSchemaCache(SqlCeInformationSchemaData schemaData)
        {
            SchemaData = schemaData;
        }

        public IReadOnlyList<TableConstraintInfo> TableConstraintInfoForTable(string tableName)
        {
            return InfoForTable(
                tableName,
                tableConstraintsByTable,
                tableConstraintInfoByName,
                c => c.ConstraintName);
        }

        public IReadOnlyList<IndexInfo> IndexInfoForTable(string tableName)
        {
            var constraints = indexesByTable[tableName];

            var result = new HashSet<IndexInfo>();
            foreach (var info in constraints)
            {
                var indexInfo = indexInfoByName[info.QualifiedName];
                result.Add(indexInfo);
            }

            return result.ToArray();
        }

        public IReadOnlyList<ReferentialConstraintInfo> ReferentialConstraintInfoForTable(string tableName)
        {
            return InfoForTable(
                tableName,
                referentialConstraintsByTable,
                referentialConstraintInfoByName,
                c => c.ConstraintName);
        }

        private static IReadOnlyList<TInfo> InfoForTable<TDbObject, TInfo>(
            string tableName,
            ListValueStringDictionary<TDbObject> dictionaryByTable,
            Dictionary<string, TInfo> dictionaryByInfoName,
            Func<TDbObject, string> nameGetter)

            where TDbObject : class
            where TInfo : class
        {
            var constraints = dictionaryByTable[tableName];

            var result = new HashSet<TInfo>();
            foreach (var info in constraints)
            {
                var indexInfo = dictionaryByInfoName[nameGetter(info)];
                result.Add(indexInfo);
            }

            return result.ToArray();
        }

        public void Clear()
        {
            tableNames = null;
            indexesByTable = null;
            columnsByTable = null;
            keyColumnUsagesByTable = null;
            tableConstraintsByTable = null;
            referentialConstraintsByTable = null;
            tableConstraintInfoByName = null;
            referentialConstraintInfoByName = null;
        }

        public void Load()
        {
            // Get table names
            tableNames = new StringCollection();
            var tableNameList = SchemaData.Tables.Select(d => d.TableName).ToArray();
            tableNames.AddRange(tableNameList);

            indexesByTable = ToListValueStringDictionary(SchemaData.Indexes, d => d.TableName);
            columnsByTable = ToListValueStringDictionary(SchemaData.Columns, d => d.TableName);
            keyColumnUsagesByTable = ToListValueStringDictionary(SchemaData.KeyColumnUsages, d => d.TableName);
            tableConstraintsByTable = ToListValueStringDictionary(SchemaData.TableConstraints, d => d.TableName);
            referentialConstraintsByTable = ToListValueStringDictionary(SchemaData.ReferentialConstraints, d => d.ConstraintTableName);

            // Sort the loaded information by their ordinal positions
            // to preserve DDL order

            SortByOrdinalComparer(
                columnsByTable,
                InformationSchema.Column.OrdinalComparer.Instance);

            SortByOrdinalComparer(
                indexesByTable,
                InformationSchema.Index.OrdinalComparer.Instance);

            SortByOrdinalComparer(
                keyColumnUsagesByTable,
                InformationSchema.KeyColumnUsage.OrdinalComparer.Instance);

            LoadTableConstraintInfo();
            LoadReferentialConstraintInfo();
            LoadIndexInfo();
        }

        private void LoadTableConstraintInfo()
        {
            tableConstraintInfoByName = new Dictionary<string, TableConstraintInfo>();
            var keyColumnUsagesByConstraint = new ListValueStringDictionary<InformationSchema.KeyColumnUsage>();

            // Flattening and serially iterating is a great option
            // without bumping the structural complexity
            // while not sacrificing performance

            foreach (var constraint in tableConstraintsByTable.Values.Flatten())
            {
                var constraintInfo = new TableConstraintInfo(
                    constraint,
                    keyColumnUsagesByConstraint[constraint.ConstraintName]);

                tableConstraintInfoByName.Add(constraint.ConstraintName, constraintInfo);
            }

            foreach (var keyColumnUsage in keyColumnUsagesByTable.Values.Flatten())
            {
                string constraintName = keyColumnUsage.ConstraintName;
                keyColumnUsagesByConstraint[constraintName].Add(keyColumnUsage);
            }
        }

        private void LoadReferentialConstraintInfo()
        {
            referentialConstraintInfoByName = new Dictionary<string, ReferentialConstraintInfo>();
            var keyColumnUsagesByConstraint = new ListValueStringDictionary<InformationSchema.KeyColumnUsage>();

            foreach (var constraint in referentialConstraintsByTable.Values.Flatten())
            {
                var tableConstraint = tableConstraintInfoByName[constraint.UniqueConstraintName];

                var constraintInfo = new ReferentialConstraintInfo(
                    constraint,
                    keyColumnUsagesByConstraint[constraint.ConstraintName],
                    tableConstraint);

                referentialConstraintInfoByName.Add(constraint.ConstraintName, constraintInfo);
            }

            foreach (var keyColumnUsage in keyColumnUsagesByTable.Values.Flatten())
            {
                string constraintName = keyColumnUsage.ConstraintName;
                keyColumnUsagesByConstraint[constraintName].Add(keyColumnUsage);
            }
        }

        private void LoadIndexInfo()
        {
            // Due to the way the data is stored, a LINQ
            // query is very appealing

            var indexInfo =
                from index in indexesByTable.Values.Flatten()
                group index
                    by new QualifiedObjectName(index.TableName, index.IndexName)
                    into indexGroup
                select new IndexInfo(indexGroup.Key, indexGroup.ToList());

            indexInfoByName = indexInfo.ToDictionary(x => x.QualifiedName);
        }

        private static void SortByOrdinalComparer<TEntity, TComparer>(
            ListValueStringDictionary<TEntity> dictionary,
            TComparer comparer)
            where TComparer : IComparer<TEntity>
        {
            foreach (var tableColumnList in dictionary.Values)
            {
                tableColumnList.Sort(comparer);
            }
        }

        private static ListValueStringDictionary<T> ToListValueStringDictionary<T>(
            IReadOnlyCollection<T> collection,
            Func<T, string> tableNameGetter)

            where T : class
        {
            var dictionary = new ListValueStringDictionary<T>();
            foreach (var item in collection)
                dictionary.Add(tableNameGetter(item), item);
            return dictionary;
        }
    }
}
