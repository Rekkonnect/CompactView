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
        [Table("INFORMATION_SCHEMA.KEY_COLUMN_USAGE")]
        public class KeyColumnUsage
        {
            [Column("CONSTRAINT_CATALOG")]
            public string ConstraintCatalog { get; set; }

            [Column("CONSTRAINT_SCHEMA")]
            public string ConstraintSchema { get; set; }

            [Column("CONSTRAINT_NAME")]
            public string ConstraintName { get; set; }

            [Column("TABLE_CATALOG")]
            public string TableCatalog { get; set; }

            [Column("TABLE_SCHEMA")]
            public string TableSchema { get; set; }

            [Column("TABLE_NAME")]
            public string TableName { get; set; }

            [Column("COLUMN_NAME")]
            public string ColumnName { get; set; }

            [Column("COLUMN_GUID")]
            public string ColumnGuid { get; set; }

            [Column("COLUMN_PROPID")]
            public int? ColumnPropid { get; set; }

            [Column("ORDINAL_POSITION")]
            public int? OrdinalPosition { get; set; }
        }
    }
}
