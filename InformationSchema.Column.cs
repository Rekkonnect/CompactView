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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompactView
{
    public partial class InformationSchema
    {
        [Table("COLUMNS", Schema = Schema)]
        public class Column
        {
            [Key]
            [Column("TABLE_NAME", Order = 0)]
            public string TableName { get; set; }

            [Key]
            [Column("COLUMN_NAME", Order = 1)]
            public string ColumnName { get; set; }

            [Column("COLUMN_HASDEFAULT")]
            public bool HasDefaultValue { get; set; }

            [Column("COLUMN_DEFAULT")]
            public string DefaultValue { get; set; }

            [Column("ORDINAL_POSITION")]
            public int OrdinalPosition { get; set; }

            [Column("IS_NULLABLE")]
            public string IsNullableName { get; set; }

            [Column("DATA_TYPE")]
            public string DataType { get; set; }

            [Column("CHARACTER_MAXIMUM_LENGTH")]
            public int? CharacterMaximumLength { get; set; }

            [Column("NUMERIC_PRECISION")]
            public int? NumericPrecision { get; set; }

            [Column("NUMERIC_SCALE")]
            public int? NumericScale { get; set; }

            [Column("AUTOINC_SEED")]
            public long? AutoIncrementSeed { get; set; }

            [Column("AUTOINC_INCREMENT")]
            public long? AutoIncrementBy { get; set; }

            #region Ignored
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
            public string ColumnPropid { get; set; }

            [NotMapped]
            [Column("COLUMN_FLAGS")]
            public int ColumnFlags { get; set; }

            [NotMapped]
            [Column("TYPE_GUID")]
            public string TypeGuid { get; set; }

            [NotMapped]
            [Column("CHARACTER_OCTET_LENGTH")]
            public int CharacterOctetLength { get; set; }

            [NotMapped]
            [Column("DATETIME_PRECISION")]
            public string DatetimePrecision { get; set; }

            [NotMapped]
            [Column("CHARACTER_SET_CATALOG")]
            public string CharacterSetCatalog { get; set; }

            [NotMapped]
            [Column("CHARACTER_SET_SCHEMA")]
            public string CharacterSetSchema { get; set; }

            [NotMapped]
            [Column("CHARACTER_SET_NAME")]
            public string CharacterSetName { get; set; }

            [NotMapped]
            [Column("COLLATION_CATALOG")]
            public string CollationCatalog { get; set; }

            [NotMapped]
            [Column("COLLATION_SCHEMA")]
            public string CollationSchema { get; set; }

            [NotMapped]
            [Column("COLLATION_NAME")]
            public string CollationName { get; set; }

            [NotMapped]
            [Column("DOMAIN_CATALOG")]
            public string DomainCatalog { get; set; }

            [NotMapped]
            [Column("DOMAIN_SCHEMA")]
            public string DomainSchema { get; set; }

            [NotMapped]
            [Column("DOMAIN_NAME")]
            public string DomainName { get; set; }

            [NotMapped]
            [Column("DESCRIPTION")]
            public string Description { get; set; }

            [NotMapped]
            [Column("AUTOINC_MIN")]
            public int AutoIncrementMin { get; set; }

            [NotMapped]
            [Column("AUTOINC_MAX")]
            public int AutoIncrementMax { get; set; }

            [NotMapped]
            [Column("AUTOINC_NEXT")]
            public int AutoIncrementNext { get; set; }
            #endregion

            [NotMapped]
            public bool IsNullable => IsNullableName.Equals("YES",
                StringComparison.OrdinalIgnoreCase);

            [NotMapped]
            public bool IsRowGuidCol => KnownColumnFlags.IsRowGuidCol(ColumnFlags);

            public static class KnownColumnFlags
            {
                public const int RowGuidColA = 378;
                public const int RowGuidColB = 282;

                public static bool IsRowGuidCol(int flags)
                {
                    return flags is RowGuidColA
                        || flags is RowGuidColB;
                }
            }

            public sealed class OrdinalComparer : IComparer<Column>
            {
                public static OrdinalComparer Instance { get; } = new OrdinalComparer();

                public int Compare(Column x, Column y)
                {
                    return x.OrdinalPosition.CompareTo(y.OrdinalPosition);
                }
            }
        }
    }
}
