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

using System.ComponentModel.DataAnnotations.Schema;

namespace CompactView
{
    public partial class InformationSchema
    {
        [Table("INFORMATION_SCHEMA.INDEXES")]
        public class Index
        {
            [Column("TABLE_CATALOG")]
            public string TableCatalog { get; set; }

            [Column("TABLE_SCHEMA")]
            public string TableSchema { get; set; }

            [Column("TABLE_NAME")]
            public string TableName { get; set; }

            [Column("INDEX_CATALOG")]
            public string IndexCatalog { get; set; }

            [Column("INDEX_SCHEMA")]
            public string IndexSchema { get; set; }

            [Column("INDEX_NAME")]
            public string IndexName { get; set; }

            [Column("PRIMARY_KEY")]
            public bool PrimaryKey { get; set; }

            [Column("UNIQUE")]
            public bool Unique { get; set; }

            [Column("CLUSTERED")]
            public bool Clustered { get; set; }

            [Column("TYPE")]
            public string Type { get; set; }

            [Column("FILL_FACTOR")]
            public int? FillFactor { get; set; }

            [Column("INITIAL_SIZE")]
            public int? InitialSize { get; set; }

            [Column("NULLS")]
            public string Nulls { get; set; }

            [Column("SORT_BOOKMARKS")]
            public bool SortBookmarks { get; set; }

            [Column("AUTO_UPDATE")]
            public bool AutoUpdate { get; set; }

            [Column("NULL_COLLATION")]
            public string NullCollation { get; set; }

            [Column("ORDINAL_POSITION")]
            public int? OrdinalPosition { get; set; }

            [Column("COLUMN_NAME")]
            public string ColumnName { get; set; }

            [Column("COLUMN_GUID")]
            public string ColumnGuid { get; set; }

            [Column("COLUMN_PROPID")]
            public int? ColumnPropid { get; set; }

            [Column("COLLATION")]
            public string Collation { get; set; }

            [Column("CARDINALITY")]
            public long? Cardinality { get; set; }

            [Column("PAGES")]
            public int? Pages { get; set; }

            [Column("FILTER_CONDITION")]
            public string FilterCondition { get; set; }

            [Column("INTEGRATED")]
            public bool Integrated { get; set; }
        }
    }
}
