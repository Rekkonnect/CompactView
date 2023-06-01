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
        [Table("INFORMATION_SCHEMA.COLUMNS")]
        public class Column
        {
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
            public string ColumnPropid { get; set; }

            [Column("ORDINAL_POSITION")]
            public int OrdinalPosition { get; set; }

            [Column("COLUMN_HASDEFAULT")]
            public bool ColumnHasdefault { get; set; }

            [Column("COLUMN_DEFAULT")]
            public string ColumnDefault { get; set; }

            [Column("COLUMN_FLAGS")]
            public int ColumnFlags { get; set; }

            [Column("IS_NULLABLE")]
            public string IsNullableString { get; set; }

            [Column("DATA_TYPE")]
            public string DataType { get; set; }

            [Column("TYPE_GUID")]
            public string TypeGuid { get; set; }

            [Column("CHARACTER_MAXIMUM_LENGTH")]
            public int CharacterMaximumLength { get; set; }

            [Column("CHARACTER_OCTET_LENGTH")]
            public int CharacterOctetLength { get; set; }

            [Column("NUMERIC_PRECISION")]
            public int? NumericPrecision { get; set; }

            [Column("NUMERIC_SCALE")]
            public string NumericScale { get; set; }

            [Column("DATETIME_PRECISION")]
            public string DatetimePrecision { get; set; }

            [Column("CHARACTER_SET_CATALOG")]
            public string CharacterSetCatalog { get; set; }

            [Column("CHARACTER_SET_SCHEMA")]
            public string CharacterSetSchema { get; set; }

            [Column("CHARACTER_SET_NAME")]
            public string CharacterSetName { get; set; }

            [Column("COLLATION_CATALOG")]
            public string CollationCatalog { get; set; }

            [Column("COLLATION_SCHEMA")]
            public string CollationSchema { get; set; }

            [Column("COLLATION_NAME")]
            public string CollationName { get; set; }

            [Column("DOMAIN_CATALOG")]
            public string DomainCatalog { get; set; }

            [Column("DOMAIN_SCHEMA")]
            public string DomainSchema { get; set; }

            [Column("DOMAIN_NAME")]
            public string DomainName { get; set; }

            [Column("DESCRIPTION")]
            public string Description { get; set; }

            [Column("AUTOINC_MIN")]
            public int AutoincMin { get; set; }

            [Column("AUTOINC_MAX")]
            public int AutoincMax { get; set; }

            [Column("AUTOINC_NEXT")]
            public int AutoincNext { get; set; }

            [Column("AUTOINC_SEED")]
            public int AutoincSeed { get; set; }

            [Column("AUTOINC_INCREMENT")]
            public int AutoincIncrement { get; set; }
        }
    }
}
