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
using CompactView.Lexing;
using System;

namespace CompactView
{
    /// <summary>
    /// Represents a SQL string that also caches its tokenized form.
    /// The tokens are lazily computed on demand and cached for later use.
    /// </summary>
    public class SqlString
    {
        private readonly Lazy<TokenList> _lazyTokens;

        public string Sql { get; }

        public SqlLexer Lexer { get; set; } = SqlLexer.Shared;

        public SqlString(string sql)
        {
            Sql = sql;
            _lazyTokens = new Lazy<TokenList>(Tokenize);
        }

        public TokenList GetTokens()
        {
            return _lazyTokens.Value;
        }

        private TokenList Tokenize()
        {
            return Lexer.Tokenize(Sql);
        }
    }
}
