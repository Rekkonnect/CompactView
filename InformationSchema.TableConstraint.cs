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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompactView
{
    public partial class InformationSchema
    {
        [Table("INFORMATION_SCHEMA.TABLE_CONSTRAINTS")]
        public class TableConstraint
        {
            [Key]
            [Column("CONSTRAINT_NAME")]
            public string ConstraintName { get; set; }

            [Column("CONSTRAINT_CATALOG")]
            public string ConstraintCatalog { get; set; }

            [Column("CONSTRAINT_SCHEMA")]
            public string ConstraintSchema { get; set; }

            [Column("TABLE_CATALOG")]
            public string TableCatalog { get; set; }

            [Column("TABLE_SCHEMA")]
            public string TableSchema { get; set; }

            [Column("TABLE_NAME")]
            public string TableName { get; set; }

            [Column("CONSTRAINT_TYPE")]
            public string ConstraintTypeName { get; set; }

            [Column("IS_DEFERRABLE")]
            public bool IsDeferrable { get; set; }

            [Column("INITIALLY_DEFERRED")]
            public bool InitiallyDeferred { get; set; }

            [Column("DESCRIPTION")]
            public string Description { get; set; }

            [NotMapped]
            public ConstraintType ConstraintTypeValue
                => MapConstraintRuleName(ConstraintTypeName);

            private static ConstraintType MapConstraintRuleName(string name)
            {
                switch (name)
                {
                    case ConstraintTypeNames.PrimaryKey:
                        return ConstraintType.PrimaryKey;
                    case ConstraintTypeNames.ForeignKey:
                        return ConstraintType.ForeignKey;
                    default:
                        return ConstraintType.Unknown;
                }
            }

            public static class ConstraintTypeNames
            {
                public const string PrimaryKey = "PRIMARY KEY";
                public const string ForeignKey = "FOREIGN KEY";
            }
        }

        public enum ConstraintType
        {
            Unknown,
            PrimaryKey,
            ForeignKey,
        }
    }
}
