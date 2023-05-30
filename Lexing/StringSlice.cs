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

namespace CompactView.Lexing
{
    public readonly struct StringSlice
    {
        public readonly string Source;
        public readonly int Offset;
        public readonly int Length;

        public bool IsValid => Source != null;

        public StringSlice(string source, int offset, int length)
        {
            Source = source;
            Offset = offset;
            Length = length;
        }

        public char this[int index] => Source[Offset + index];

        public override string ToString() => Source.Substring(Offset, Length);
    }
}
