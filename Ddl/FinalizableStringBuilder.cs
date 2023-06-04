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

using System.Text;

namespace CompactView.Ddl
{
    internal class FinalizableStringBuilder
    {
        public StringBuilder Builder { get; }
        public string FinalizedString { get; private set; }

        public FinalizableStringBuilder()
            : this(new StringBuilder())
        {
        }

        public FinalizableStringBuilder(StringBuilder builder)
        {
            Builder = builder;
        }

        public string ForceGetFinalizedString()
        {
            if (FinalizedString is null)
            {
                FinalizeBuilder();
            }

            return FinalizedString;
        }

        public virtual void FinalizeBuilder()
        {
            FinalizedString = Builder.ToString();
        }

        public void ClearFinalizedString()
        {
            FinalizedString = null;
        }

        public virtual void Clear()
        {
            ClearFinalizedString();
            Builder.Clear();
        }
    }
}
