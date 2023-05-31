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
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CompactView.Lexing
{
    public readonly struct StringSlice
    {
        public readonly string Source;
        public readonly int Offset;
        public readonly int Length;

        public int LastIndex => Offset + Length - 1;

        public bool IsValid => Source != null;

        public StringSlice(string source, int offset, int length)
        {
            Source = source;
            Offset = offset;
            Length = length;
        }

        // No validation for performance

        public int IndexOf(char c)
        {
            return NormalizeIndexOfResult(Source.IndexOf(c, Offset, Length));
        }

        public int IndexOf(char c, int start)
        {
            return NormalizeIndexOfResult(Source.IndexOf(c, start + Offset));
        }

        public int IndexOf(char c, int start, int length)
        {
            return NormalizeIndexOfResult(Source.IndexOf(c, start + Offset, length));
        }

        public int LastIndexOf(char c)
        {
            return NormalizeIndexOfResult(Source.LastIndexOf(c, LastIndex, Length));
        }

        private int NormalizeIndexOfResult(int result)
        {
            if (result < Offset)
                return result;

            return result - Offset;
        }

        public StringSlice Trim()
        {
            int start = 0;
            while (start < Length)
            {
                bool whitespace = char.IsWhiteSpace(this[start]);
                if (!whitespace)
                    break;
                start++;
            }

            int end = Length - 1;
            while (end >= start)
            {
                bool whitespace = char.IsWhiteSpace(this[end]);
                if (!whitespace)
                    break;
                end--;
            }

            int length = end - start + 1;

            return Slice(start, length);
        }

        public StringSlice Slice(int start, int length)
        {
            return new StringSlice(Source, start + Offset, length);
        }

        public LineEnumerator EnumerateLines()
        {
            return new LineEnumerator(this);
        }

        public char this[int index] => Source[Offset + index];

        public override string ToString() => Source.Substring(Offset, Length);

        public struct LineEnumerator : IEnumerator<StringSlice>, IEnumerable<StringSlice>
        {
            private readonly StringSlice _sourceSlice;
            private int _lineStartIndex;

            public StringSlice Current { get; private set; }
            object IEnumerator.Current => Current;

            public LineEnumerator(StringSlice source)
            {
                Current = default;
                _sourceSlice = source;
                _lineStartIndex = 0;
            }

            public bool MoveNext()
            {
                if (_lineStartIndex >= _sourceSlice.Length)
                {
                    return false;
                }

                int nextIndex = _sourceSlice.IndexOf('\n', _lineStartIndex);
                if (nextIndex < 0)
                {
                    nextIndex = _sourceSlice.Length;
                }

                int lineLength = nextIndex - _lineStartIndex;
                Current = _sourceSlice.Slice(_lineStartIndex, lineLength);
                _lineStartIndex = nextIndex + 1;
                return true;
            }

            public void Reset()
            {
                Current = default;
                _lineStartIndex = 0;
            }

            void IDisposable.Dispose() { }

            public IEnumerator<StringSlice> GetEnumerator()
            {
                return this;
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }
        }
    }
}
