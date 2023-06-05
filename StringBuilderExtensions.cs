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

namespace CompactView
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendUpper(this StringBuilder stringBuilder, string value)
        {
            for (int i = 0; i < value.Length; i++)
                stringBuilder.Append(char.ToUpper(value[i]));

            return stringBuilder;
        }
        public static StringBuilder AppendLower(this StringBuilder stringBuilder, string value)
        {
            for (int i = 0; i < value.Length; i++)
                stringBuilder.Append(char.ToLower(value[i]));

            return stringBuilder;
        }

        public static StringBuilder AppendLine(this StringBuilder stringBuilder, char value)
        {
            return stringBuilder
                .Append(value)
                .AppendLine();
        }

        public static StringBuilder AppendLineCount(this StringBuilder stringBuilder, int count)
        {
            for (int i = 0; i < count; i++)
                stringBuilder.AppendLine();
            return stringBuilder;
        }
    }
}
