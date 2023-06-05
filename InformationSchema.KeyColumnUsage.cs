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

namespace CompactView
{
    public partial class InformationSchema
    {
        [Table("KEY_COLUMN_USAGE", Schema = Schema)]
        public class KeyColumnUsage
        {
            [Key]
            [Column("CONSTRAINT_NAME")]
            public string ConstraintName { get; set; }

            [Column("TABLE_NAME")]
            public string TableName { get; set; }

            [Column("COLUMN_NAME")]
            public string ColumnName { get; set; }

            [Column("ORDINAL_POSITION")]
            public int OrdinalPosition { get; set; }

            #region Ignored
            [NotMapped]
            [Column("CONSTRAINT_CATALOG")]
            public string ConstraintCatalog { get; set; }

            [NotMapped]
            [Column("CONSTRAINT_SCHEMA")]
            public string ConstraintSchema { get; set; }

            [NotMapped]
            [Column("TABLE_CATALOG")]
            public string TableCatalog { get; set; }

            [NotMapped]
            [Column("TABLE_SCHEMA")]
            public string TableSchema { get; set; }

            [NotMapped]
            [Column("COLUMN_GUID")]
            public string ColumnGuid { get; set; }

            [NotMapped]
            [Column("COLUMN_PROPID")]
            public int? ColumnPropid { get; set; }
            #endregion

            public sealed class OrdinalComparer : IComparer<KeyColumnUsage>
            {
                public static OrdinalComparer Instance { get; } = new OrdinalComparer();

                public int Compare(KeyColumnUsage x, KeyColumnUsage y)
                {
                    return x.OrdinalPosition.CompareTo(y.OrdinalPosition);
                }
            }
        }
    }
}
