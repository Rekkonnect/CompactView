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
    internal class RtfSqlString
    {
        private readonly Lazy<string> _lazyRtf;

        public SqlString SqlString { get; }

        public RtfSqlString(SqlString sqlString)
        {
            SqlString = sqlString;
            _lazyRtf = new Lazy<string>(BuildRtf);
        }

        public string GetRtf()
        {
            return _lazyRtf.Value;
        }

        private string BuildRtf()
        {
            var tokens = SqlString.GetTokens();
            var builder = new RtfStringBuilder();

            foreach (var token in tokens)
            {
                builder.AppendUnprocessed(@"\cf");
                builder.Append((int)token.Kind + 1);
                builder.Append(' ');
                builder.AppendToken(token);
            }

            return builder.ToString();
        }
    }
}
