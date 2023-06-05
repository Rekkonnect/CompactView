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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CompactView
{
    public class TokenList : List<Token>
    {
        public Token AtPosition(int position)
        {
            int min = 0;
            int max = Count - 1;
            while (min <= max)
            {
                int mid = (min + max) / 2;
                Token token = this[mid];
                if (position < token.Value.Offset)
                {
                    max = mid - 1;
                }
                else if (position > token.Value.LastIndex)
                {
                    min = mid + 1;
                }
                else
                {
                    return token;
                }
            }

            return Token.Invalid;
        }

        public string WithoutComments()
        {
            var result = new StringBuilder();

            foreach (var token in new NoCommentEnumerator(this))
            {
                result.AppendToken(token);
            }

            return result.ToString();
        }

        public class NoCommentEnumerator : IEnumerator<Token>, IEnumerable<Token>
        {
            private readonly List<Token> _tokenList;
            private int _currentIndex = -1;

            public Token Current => _tokenList[_currentIndex];

            object IEnumerator.Current => Current;

            public NoCommentEnumerator(List<Token> tokenList)
            {
                _tokenList = tokenList;
            }

            void IDisposable.Dispose() { }

            public bool MoveNext()
            {
                while (true)
                {
                    _currentIndex++;

                    if (_currentIndex >= _tokenList.Count)
                    {
                        return false;
                    }

                    Token token = _tokenList[_currentIndex];
                    switch (token.Kind)
                    {
                        case TokenKind.SingleLineComment:
                        case TokenKind.MultiLineComment:
                            break;

                        default:
                            return true;
                    }
                }
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            public IEnumerator<Token> GetEnumerator() => this;
            IEnumerator IEnumerable.GetEnumerator() => this;
        }
    }
}
