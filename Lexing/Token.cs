﻿/**************************************************************************
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
    public readonly struct Token
    {
        public static Token Invalid { get; } = new Token(TokenKind.Undetermined, default);

        public TokenKind Kind { get; }
        public StringSlice Value { get; }

        public bool IsValid => Value.Source != null;

        public Token(TokenKind kind, StringSlice value)
        {
            Kind = kind;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Kind}: '{Value}'";
        }
    }
}
