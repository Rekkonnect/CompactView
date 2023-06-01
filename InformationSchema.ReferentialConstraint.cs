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
        [Table("INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS")]
        public class ReferentialConstraint
        {
            [Column("CONSTRAINT_CATALOG")]
            public string ConstraintCatalog { get; set; }

            [Column("CONSTRAINT_SCHEMA")]
            public string ConstraintSchema { get; set; }

            [Column("CONSTRAINT_TABLE_NAME")]
            public string ConstraintTableName { get; set; }

            [Column("CONSTRAINT_NAME")]
            public string ConstraintName { get; set; }

            [Column("UNIQUE_CONSTRAINT_CATALOG")]
            public string UniqueConstraintCatalog { get; set; }

            [Column("UNIQUE_CONSTRAINT_SCHEMA")]
            public string UniqueConstraintSchema { get; set; }

            [Column("UNIQUE_CONSTRAINT_TABLE_NAME")]
            public string UniqueConstraintTableName { get; set; }

            [Column("UNIQUE_CONSTRAINT_NAME")]
            public string UniqueConstraintName { get; set; }

            [Column("MATCH_OPTION")]
            public string MatchOption { get; set; }

            [Column("UPDATE_RULE")]
            public string UpdateRule { get; set; }

            [Column("DELETE_RULE")]
            public string DeleteRule { get; set; }

            [Column("DESCRIPTION")]
            public string Description { get; set; }
        }
    }
}
