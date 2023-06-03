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
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Linq.Expressions;

namespace CompactView
{
    public static class PropertyConfigurationExtensions
    {
        public static PrimitivePropertyConfiguration HasInferedColumnName<TEntity, TExpression>(this EntityTypeConfiguration<TEntity> configuration, Expression<Func<TEntity, TExpression>> propertyExpression)
            where TEntity : class
            where TExpression : struct
        {
            string name = GetInferedColumnName(propertyExpression);
            return configuration.Property(propertyExpression).HasColumnName(name);
        }

        public static StringPropertyConfiguration HasInferedColumnName<TEntity>(this EntityTypeConfiguration<TEntity> configuration, Expression<Func<TEntity, string>> propertyExpression)
            where TEntity : class
        {
            string name = GetInferedColumnName(propertyExpression);
            return configuration.Property(propertyExpression).HasColumnName(name);
        }

        private static string GetInferedColumnName(LambdaExpression propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            var columnAttribute = memberExpression?.Member.GetCustomAttributes(typeof(ColumnAttribute), false)
                                    .FirstOrDefault() as ColumnAttribute;

            return columnAttribute?.Name;
        }
    }
}
