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

using System.Data.Entity;
using System.Data.SqlServerCe;

namespace CompactView
{
    //[DbConfigurationType(typeof(SqlCeDbConfiguration))]
    public class SqlCeContext : DbContext
    {
        public DbSet<InformationSchema.Table> InformationSchema_Tables { get; set; }
        public DbSet<InformationSchema.Index> InformationSchema_Indexes { get; set; }
        public DbSet<InformationSchema.Column> InformationSchema_Columns { get; set; }
        public DbSet<InformationSchema.KeyColumnUsage> InformationSchema_KeyColumnUsages { get; set; }
        public DbSet<InformationSchema.ReferentialConstraint> InformationSchema_ReferentialConstraints { get; set; }

        static SqlCeContext()
        {
            //Database.SetInitializer<SqlCeContext>(null);
        }

        public SqlCeContext(string connectionString)
            : base(connectionString) { }
        public SqlCeContext(SqlCeConnection connection)
            : base(connection, false) { }

        private static SqlCeConnection CreateConnection(string connectionString)
        {
            return new SqlCeConnection(connectionString);
        }
    }
}
