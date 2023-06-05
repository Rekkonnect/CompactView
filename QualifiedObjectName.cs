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

namespace CompactView
{
    public readonly struct QualifiedObjectName : IEquatable<QualifiedObjectName>
    {
        public string TableName { get; }
        public string ObjectName { get; }

        public QualifiedObjectName(string tableName, string objectName)
        {
            TableName = tableName;
            ObjectName = objectName;
        }

        public override string ToString()
        {
            return TableName + "." + ObjectName;
        }

        public bool Equals(QualifiedObjectName other)
        {
            return TableName == other.TableName
                && ObjectName == other.ObjectName;
        }

        public override bool Equals(object obj)
        {
            return obj is QualifiedObjectName
                && Equals((QualifiedObjectName)obj);
        }

        public override int GetHashCode()
        {
            return TableName.GetHashCode() ^ ObjectName.GetHashCode();
        }
    }
}
