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
using Garyon.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CompactView
{
    public class SqlRichTextBox : RichTextBox
    {
        private readonly AuxiliarySqlRtb _auxiliaryRtb = new AuxiliarySqlRtb();

        private bool _parsing = false;

        internal RtfSqlStringDictionary RtfSqlCache => _auxiliaryRtb.RtfSqlCache;

        public SqlString SqlString
        {
            set
            {
                ProcessViaAuxiliary(value);
            }
        }

        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                HandleTextChanged();
            }
        }

        public SqlRichTextBox()
        {
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            HandleTextChanged();
        }

        private void HandleTextChanged()
        {
            if (_parsing)
                return;

            var sqlString = new SqlString(Text);
            ProcessViaAuxiliary(sqlString);
        }

        private void ProcessViaAuxiliary(SqlString sql)
        {
            _parsing = true;

            // Preserve the font
            float previousZoom = ZoomFactor;
            var previousFont = Font;
            _auxiliaryRtb.ZoomFactor = ZoomFactor;
            _auxiliaryRtb.Font = Font;

            _auxiliaryRtb.SetSqlString(sql);

            int firstVisibleLineBefore = this.GetFirstVisibleLine();
            int hScroll = this.GetHScroll();
            int selectionLength = SelectionLength;
            int selectionStart = SelectionStart;

            this.SetRedraw(false);

            Rtf = _auxiliaryRtb.Rtf;
            ZoomFactor = previousZoom;
            Font = previousFont;

            Select(selectionStart, selectionLength);
            this.SetHScroll(hScroll);
            int firstVisibleLineAfter = this.GetFirstVisibleLine();
            this.ScrollLines(firstVisibleLineBefore - firstVisibleLineAfter);

            this.SetRedraw(true);
            Refresh();

            _parsing = false;
        }

        internal sealed class RtfSqlStringDictionary
        {
            private readonly Dictionary<SqlString, RtfSqlString> _dictionary =
                new Dictionary<SqlString, RtfSqlString>();

            public void Clear()
            {
                _dictionary.Clear();
            }

            public void Add(SqlString sqlString)
            {
                _dictionary.Add(sqlString, new RtfSqlString(sqlString));
            }

            public void AddRange(IEnumerable<SqlString> sqlStrings)
            {
                foreach (var sqlString in sqlStrings)
                {
                    Add(sqlString);
                }
            }

            public void SetRange(IEnumerable<SqlString> sqlStrings)
            {
                Clear();
                AddRange(sqlStrings);
            }

            public bool TryGetValue(SqlString sqlString, out RtfSqlString rtfSqlString)
            {
                return _dictionary.TryGetValue(sqlString, out rtfSqlString);
            }
            public RtfSqlString ValueOrNull(SqlString sqlString)
            {
                return _dictionary.ValueOrDefault(sqlString);
            }

            public void Remove(SqlString sqlString)
            {
                _dictionary.Remove(sqlString);
            }
        }

        private sealed class AuxiliarySqlRtb : RichTextBox
        {
            private static readonly Color[] _colorMappings =
                new Color[(int)TokenKind.KnownFunction + 1];

            private static readonly string _rtfHeader;

            public readonly RtfSqlStringDictionary RtfSqlCache =
                new RtfSqlStringDictionary();

            static AuxiliarySqlRtb()
            {
                SetColorMapping(TokenKind.Basic, Color.Black);
                SetColorMapping(TokenKind.Identifier, 96, 0, 128);
                SetColorMapping(TokenKind.BracketedIdentifier, 64, 0, 128);
                SetColorMapping(TokenKind.Keyword, 0, 0, 255);
                SetColorMapping(TokenKind.Type, 0, 128, 128);
                SetColorMapping(TokenKind.Number, 255, 0, 0);
                SetColorMapping(TokenKind.String, 0, 128, 0);
                SetColorMapping(TokenKind.SingleLineComment, 0, 168, 32);
                SetColorMapping(TokenKind.MultiLineComment, 0, 168, 32);
                SetColorMapping(TokenKind.KnownFunction, 128, 96, 255);

                _rtfHeader = GenerateRtfHeader();
            }

            private static void SetColorMapping(TokenKind kind, Color color)
            {
                _colorMappings[(int)kind] = color;
            }
            private static void SetColorMapping(TokenKind kind, int r, int g, int b)
            {
                var color = Color.FromArgb(r, g, b);
                SetColorMapping(kind, color);
            }

            private void SetRtfSql(RtfSqlString rtf)
            {
                Text = string.Empty;
                Rtf = GenerateRtf(rtf);
            }

            private string GenerateRtf(RtfSqlString rtfString)
            {
                var builder = new RtfStringBuilder();
                builder.AppendUnprocessed(_rtfHeader);

                var metaHeaders = GetMetaHeaders();
                builder.AppendUnprocessed(metaHeaders);

                // This header specifies the Latin text
                builder.AppendUnprocessed(@"\lang1033 ");

                var rtf = rtfString.GetRtf();
                builder.AppendUnprocessed(rtf);

                builder.AppendUnprocessed("\\par\n");
                builder.Append('}');
                return builder.ToString();
            }

            public void SetSqlString(SqlString sql)
            {
                var rtfString = GetRtfString(sql);
                SetRtfSql(rtfString);
            }

            private RtfSqlString GetRtfString(SqlString sql)
            {
                return RtfSqlCache.ValueOrNull(sql)
                    ?? new RtfSqlString(sql);
            }

            private StringSlice GetMetaHeaders()
            {
                var rtf = Rtf;
                const string delimitingStart = @"\rtf1";
                const string delimitingEnd = @"\par";
                int metaStart = rtf.IndexOf(delimitingStart) + delimitingStart.Length;
                int metaEnd = rtf.LastIndexOf(delimitingEnd);
                int metaLength = metaEnd - metaStart;

                return new StringSlice(rtf, metaStart, metaLength);
            }

            private static string GenerateRtfHeader()
            {
                var builder = new StringBuilder();
                builder.Append(@"{\rtf1{");
                RtbHelpers.ColorTableString(_colorMappings, builder);
                builder.Append('}');

                return builder.ToString();
            }
        }

        private static class RtbHelpers
        {
            public static void ColorTableString(Color[] colors, StringBuilder builder)
            {
                builder.Append(@"\colortbl ;");

                for (int i = 0; i < colors.Length; i++)
                {
                    var color = colors[i];
                    builder.Append(@"\red");
                    builder.Append(color.R);
                    builder.Append(@"\green");
                    builder.Append(color.G);
                    builder.Append(@"\blue");
                    builder.Append(color.B);
                    builder.Append(';');
                }
            }
        }
    }

    internal sealed class RtfStringBuilder
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public RtfStringBuilder AppendUnprocessed(string text)
        {
            _stringBuilder.Append(text);
            return this;
        }

        public RtfStringBuilder Append(byte value)
        {
            _stringBuilder.Append(value);
            return this;
        }

        public RtfStringBuilder Append(int value)
        {
            _stringBuilder.Append(value);
            return this;
        }

        public RtfStringBuilder Append(char value)
        {
            _stringBuilder.Append(value);
            return this;
        }

        public RtfStringBuilder AppendUnprocessed(StringSlice stringSlice)
        {
            _stringBuilder.AppendSlice(stringSlice);
            return this;
        }

        public RtfStringBuilder Append(StringSlice stringSlice)
        {
            foreach (var line in stringSlice.EnumerateLines())
            {
                EscapeAppend(line);

                bool isTrailingText = line.MatchesEnd(stringSlice);
                if (!isTrailingText)
                {
                    _stringBuilder.Append(@"\par");
                    _stringBuilder.Append('\n');
                }
            }

            return this;
        }

        public RtfStringBuilder EscapeAppend(StringSlice slice)
        {
            for (int i = 0; i < slice.Length; i++)
            {
                char c = slice[i];
                switch (c)
                {
                    case '\\':
                    case '{':
                    case '}':
                        _stringBuilder.Append('\\');
                        _stringBuilder.Append(c);
                        break;

                    default:
                        _stringBuilder.Append(c);
                        break;
                }
            }

            return this;
        }

        public RtfStringBuilder AppendToken(Token token)
        {
            var stringSlice = token.Value;
            return Append(stringSlice);
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
