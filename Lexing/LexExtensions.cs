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

namespace CompactView.Lexing
{
    public static class LexExtensions
    {
        public static StringBuilder AppendSlice(this StringBuilder sb, StringSlice slice)
        {
            return sb.Append(slice.Source, slice.Offset, slice.Length);
        }
        public static StringBuilder AppendToken(this StringBuilder sb, Token token)
        {
            return AppendSlice(sb, token.Value);
        }
    }
}
