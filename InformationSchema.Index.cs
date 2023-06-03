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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;

namespace CompactView
{
    public partial class InformationSchema
    {
        [Table("INFORMATION_SCHEMA.INDEXES")]
        public class Index
        {
            [Key]
            [Column("INDEX_NAME")]
            public string IndexName { get; set; }

            [Column("TABLE_NAME")]
            public string TableName { get; set; }

            [Column("COLUMN_NAME")]
            public string ColumnName { get; set; }

            [Column("PRIMARY_KEY")]
            public bool PrimaryKey { get; set; }

            [Column("UNIQUE")]
            public bool Unique { get; set; }

            [Column("CLUSTERED")]
            public bool Clustered { get; set; }

            [Column("ORDINAL_POSITION")]
            public int OrdinalPosition { get; set; }

            [Column("COLLATION")]
            public int Collation { get; set; }
            
            #region Ignored
            [NotMapped]
            [Column("TABLE_CATALOG")]
            public string TableCatalog { get; set; }

            [NotMapped]
            [Column("TABLE_SCHEMA")]
            public string TableSchema { get; set; }

            [NotMapped]
            [Column("INDEX_CATALOG")]
            public string IndexCatalog { get; set; }

            [NotMapped]
            [Column("INDEX_SCHEMA")]
            public string IndexSchema { get; set; }

            [NotMapped]
            [Column("TYPE")]
            public string Type { get; set; }

            [NotMapped]
            [Column("FILL_FACTOR")]
            public int? FillFactor { get; set; }

            [NotMapped]
            [Column("INITIAL_SIZE")]
            public int? InitialSize { get; set; }

            [NotMapped]
            [Column("NULLS")]
            public string Nulls { get; set; }

            [NotMapped]
            [Column("SORT_BOOKMARKS")]
            public bool SortBookmarks { get; set; }

            [NotMapped]
            [Column("AUTO_UPDATE")]
            public bool AutoUpdate { get; set; }

            [NotMapped]
            [Column("NULL_COLLATION")]
            public string NullCollation { get; set; }

            [NotMapped]
            [Column("COLUMN_GUID")]
            public string ColumnGuid { get; set; }

            [NotMapped]
            [Column("COLUMN_PROPID")]
            public int? ColumnPropid { get; set; }

            [NotMapped]
            [Column("CARDINALITY")]
            public long? Cardinality { get; set; }

            [NotMapped]
            [Column("PAGES")]
            public int? Pages { get; set; }

            [NotMapped]
            [Column("FILTER_CONDITION")]
            public string FilterCondition { get; set; }

            [NotMapped]
            [Column("INTEGRATED")]
            public bool Integrated { get; set; }
            #endregion
            
            [NotMapped]
            public SortOrder SortOrder
            {
                get
                {
                    switch (Collation)
                    {
                        case 1:
                            return SortOrder.Ascending;
                        case 2:
                            return SortOrder.Descending;
                        default:
                            return SortOrder.Unspecified;
                    }
                }
            }


            public sealed class OrdinalComparer : IComparer<Index>
            {
                public static OrdinalComparer Instance { get; } = new OrdinalComparer();

                public int Compare(Index x, Index y)
                {
                    return x.OrdinalPosition.CompareTo(y.OrdinalPosition);
                }
            }
        }
    }
}
