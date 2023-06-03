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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
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

        public InformationSchemaSqlCeContext Context { get; }

        public InformationSchemaCache(InformationSchemaSqlCeContext context)
        {
            Context = context;
        }

        public void Clear()
        {
            tableNames = null;
            indexesByTable = null;
            columnsByTable = null;
            keyColumnUsagesByTable = null;
            tableConstraintsByTable = null;
            referentialConstraintsByTable = null;
        }

        public void Load()
        {
            // Get table names
            tableNames = new StringCollection();
            var tableNameList = Context.Tables.Select(d => d.TableName).ToArray();
            tableNames.AddRange(tableNameList);

            indexesByTable = ToListValueStringDictionary(Context.Indexes, d => d.TableName);
            columnsByTable = ToListValueStringDictionary(Context.Columns, d => d.TableName);
            keyColumnUsagesByTable = ToListValueStringDictionary(Context.KeyColumnUsages, d => d.TableName);
            tableConstraintsByTable = ToListValueStringDictionary(Context.TableConstraints, d => d.TableName);
            referentialConstraintsByTable = ToListValueStringDictionary(Context.ReferentialConstraints, d => d.ConstraintTableName);

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

        private static ListValueStringDictionary<T> ToListValueStringDictionary<T>(DbSet<T> dbSet, Func<T, string> tableNameGetter)
            where T : class
        {
            var dictionary = new ListValueStringDictionary<T>();
            var items = dbSet.ToList();
            foreach (var item in items)
                dictionary.Add(tableNameGetter(item), item);
            return dictionary;
        }
    }
}
