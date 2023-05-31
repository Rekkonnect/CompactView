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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CompactView
{
    public class SqlRichTextBox : RichTextBox
    {
        private readonly AuxiliarySqlRtb _auxiliaryRtb = new AuxiliarySqlRtb();

        private bool _parsing = false;

        public SqlRichTextBox()
        {
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (_parsing)
                return;

            base.OnTextChanged(e);
            ProcessViaAuxiliary();
        }

        private void ProcessViaAuxiliary()
        {
            _parsing = true;

            _auxiliaryRtb.SetText(Text);

            int firstVisibleLineBefore = this.GetFirstVisibleLine();
            int hScroll = this.GetHScroll();
            int selectionLength = SelectionLength;
            int selectionStart = SelectionStart;

            this.SetRedraw(false);
            //Select(0, TextLength);

            // The big parsing moment
            Rtf = _auxiliaryRtb.Rtf;

            Select(selectionStart, selectionLength);
            this.SetHScroll(hScroll);
            int firstVisibleLineAfter = this.GetFirstVisibleLine();
            this.ScrollLines(firstVisibleLineBefore - firstVisibleLineAfter);

            this.SetRedraw(true);
            Refresh();

            _parsing = false;
        }

        private sealed class AuxiliarySqlRtb : RichTextBox
        {
            private static readonly Color[] _colorMappings =
                new Color[(int)TokenKind.MultiLineComment + 1];

            private static readonly string _rtfHeader;

            static AuxiliarySqlRtb()
            {
                _colorMappings[(int)TokenKind.Basic] = Color.Black;
                _colorMappings[(int)TokenKind.Identifier] = Color.FromArgb(192, 0, 128);
                _colorMappings[(int)TokenKind.BracketedIdentifier] = Color.FromArgb(64, 0, 128);
                _colorMappings[(int)TokenKind.Keyword] = Color.FromArgb(0, 0, 255);
                _colorMappings[(int)TokenKind.Type] = Color.FromArgb(0, 128, 128);
                _colorMappings[(int)TokenKind.Number] = Color.FromArgb(255, 0, 0);
                _colorMappings[(int)TokenKind.String] = Color.FromArgb(0, 128, 0);
                _colorMappings[(int)TokenKind.SingleLineComment] = Color.FromArgb(0, 168, 32);
                _colorMappings[(int)TokenKind.MultiLineComment] = Color.FromArgb(0, 168, 32);

                _rtfHeader = GenerateRtfHeader();
            }

            public void SetText(string text)
            {
                Text = string.Empty;

                var tokenized = SqlLexer.Shared.Tokenize(text);

                var builder = new RtfStringBuilder();
                builder.AppendUnprocessed(_rtfHeader);

                var metaHeaders = GetMetaHeaders();
                builder.AppendUnprocessed(metaHeaders);

                // This header specifies the Latin text
                builder.AppendUnprocessed(@"\lang1033 ");

                foreach (var token in tokenized)
                {
                    builder.AppendUnprocessed(@"\cf");
                    builder.Append((int)token.Kind);
                    builder.Append(' ');
                    builder.AppendToken(token);
                }

                Rtf = builder.ToString();
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

        private sealed class RtfStringBuilder
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
                    _stringBuilder.AppendSlice(line);
                    _stringBuilder.Append(@"\par");
                    _stringBuilder.Append('\n');
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
}
