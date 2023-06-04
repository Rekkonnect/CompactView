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

namespace CompactView
{
    public class ReferentialConstraintInfo
    {
        public InformationSchema.ReferentialConstraint Constraint { get; }
        public IReadOnlyList<InformationSchema.KeyColumnUsage> ColumnUsages { get; }
        public TableConstraintInfo UniqueTableConstraintInfo { get; }

        public IReadOnlyList<InformationSchema.KeyColumnUsage> TableConstraintColumnUsages
            => UniqueTableConstraintInfo.ColumnUsages;

        public string ConstraintName => Constraint.ConstraintName;

        public ReferentialConstraintInfo(
            InformationSchema.ReferentialConstraint referentialConstraint,
            IReadOnlyList<InformationSchema.KeyColumnUsage> columnUsage,
            TableConstraintInfo uniqueTableConstraintInfo)
        {
            Constraint = referentialConstraint;
            ColumnUsages = columnUsage;
            UniqueTableConstraintInfo = uniqueTableConstraintInfo;
        }
    }
}
