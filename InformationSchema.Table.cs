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
using System;
using System.ComponentModel.DataAnnotations;

namespace CompactView
{
    public partial class InformationSchema
    {
        [Table("INFORMATION_SCHEMA.TABLES")]
        public class Table
        {
            [Key]
            [Column("TABLE_NAME")]
            public string TableName { get; set; }

            #region Ignored
            [NotMapped]
            [Column("TABLE_CATALOG")]
            public string TableCatalog { get; set; }

            [NotMapped]
            [Column("TABLE_SCHEMA")]
            public string TableSchema { get; set; }

            [NotMapped]
            [Column("TABLE_TYPE")]
            public string TableType { get; set; }

            [NotMapped]
            [Column("TABLE_GUID")]
            public Guid TableGuid { get; set; }

            [NotMapped]
            [Column("DESCRIPTION")]
            public string Description { get; set; }

            [NotMapped]
            [Column("TABLE_PROPID")]
            public int? TablePropid { get; set; }

            [NotMapped]
            [Column("DATE_CREATED")]
            public DateTime? DateCreated { get; set; }

            [NotMapped]
            [Column("DATE_MODIFIED")]
            public DateTime? DateModified { get; set; }
            #endregion
        }
    }
}
