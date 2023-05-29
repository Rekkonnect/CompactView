/**************************************************************************
Copyright (C) 2011-2014 Iván Costales Suárez
          (C) 2023 Rekkonnect

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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CompactView
{
    public class SqlParser
    {
        private static readonly string[] keywords =
        {
            "action", "add", "all", "alter", "and", "any", "as", "asc", "authorization", "avg", "backup", "begin", "between", "break", "browse", "bulk", "by", "cascade",
            "case", "check", "checkpoint", "close", "clustered", "coalesce", "collate", "column", "commit", "compute", "constraint", "contains", "containstable",
            "continue", "convert", "count", "create", "cross", "current", "current_date", "current_time", "current_timestamp", "current_user", "cursor",
            "database", "databasepassword", "dateadd", "datediff", "datename", "datepart", "dbcc", "deallocate", "declare", "default", "delete", "deny", "desc",
            "disk", "distinct", "distributed", "double", "drop", "dump", "else", "encryption", "end", "errlvl", "escape", "except", "exec", "execute", "exists",
            "exit", "expression", "fetch", "file", "fillfactor", "for", "foreign", "freetext", "freetexttable", "from", "full", "function", "goto", "grant",
            "group", "having", "holdlock", "identity", "identity_insert", "identitycol", "if", "in", "index", "inner", "insert", "intersect", "into", "is",
            "join", "key", "kill", "left", "like", "lineno", "load", "max", "min", "national", "no", "nocheck", "nonclustered", "not", "null", "nullif", "of", "off",
            "offsets", "on", "open", "opendatasource", "openquery", "openrowset", "openxml", "option", "or", "order", "outer", "over", "percent", "plan",
            "precision", "primary", "print", "proc", "procedure", "public", "raiserror", "read", "readtext", "reconfigure", "references", "replication",
            "restore", "restrict", "return", "revoke", "right", "rollback", "rowcount", "rowguidcol", "rule", "save", "schema", "select", "session_user", "set",
            "setuser", "shutdown", "some", "statistics", "sum", "system_user", "table", "textsize", "then", "to", "top", "tran", "transaction", "trigger",
            "truncate", "tsequal", "union", "unique", "update", "updatetext", "use", "user", "values", "varying", "view", "waitfor", "when", "where", "while",
            "with", "writetext",
        };
        private static readonly string[] types =
        {
            "bigint", "int", "integer", "smallint", "tinyint", "bit", "numeric", "decimal", "dec", "money", "float", "real", "datetime", "national character",
            "nchar", "national character varying", "nvarchar", "ntext", "binary", "varbinary", "image", "uniqueidentifier", "timestamp", "rowversion"
        };

        private static readonly Regex regexNumbers;
        private static readonly Regex regexKeywords;
        private static readonly Regex regexStrings;
        private static readonly Regex regexKeysQuoted;
        private static readonly Regex regexKeysBrackets;
        private static readonly Regex regexPar;
        private static readonly Regex regexInsertColortbl;
        private static readonly Regex regexSingleLineComments;
        private static readonly Regex regexMultiLineComments;
        private static readonly Regex regexTypes;

        private readonly RichTextBox richTextBoxAux = new RichTextBox();
        private RichTextBox richTextBox;
        private int selectionStart;
        private int selectionLength;

        static SqlParser()
        {
            var pattern = new StringBuilder(keywords.Length * 8);

            regexKeywords = BuildKeywordPattern(pattern, keywords);
            regexTypes = BuildKeywordPattern(pattern, types);

            regexInsertColortbl = new Regex(@";}}");
            regexKeysQuoted = new Regex(@"(?<A>(?<!\\)')(?<B>(\\'|[^'\\])*)(?<C>\\cf[134] )(?<D>[^'\\]*)(?<E>\\cf0 )(?<F>(\\'|[^'])*')", RegexOptions.Compiled | RegexOptions.Multiline);
            regexKeysBrackets = new Regex(@"(?<A>\[)(?<B>(\\'|[^\\])*)(?<C>\\cf[134] )(?<D>[^\\]*)(?<E>\\cf0 )(?<F>[^\[]*\])", RegexOptions.Compiled | RegexOptions.Multiline);
            regexNumbers = new Regex(@"(?<A>\b\d+\b)", RegexOptions.Compiled | RegexOptions.Multiline);
            regexStrings = new Regex(@"(?<A>(?<!\\)'(\\'|[^'])*(?<!\\)')", RegexOptions.Compiled | RegexOptions.Multiline);
            regexPar = new Regex(@"\\par\b", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.RightToLeft);
            regexSingleLineComments = new Regex(@"(?<A>--.*)\n?", RegexOptions.Compiled | RegexOptions.Multiline);
            regexMultiLineComments = new Regex(@"(?<A>\/\*.*\*\/)", RegexOptions.Compiled | RegexOptions.Multiline);
        }

        private static Regex BuildKeywordPattern(StringBuilder pattern, string[] keywords)
        {
            pattern.Clear();
            pattern.Append(@"(?<A>\b(");
            foreach (string keyword in keywords)
                pattern.Append(keyword).Append('|');
            pattern.Length--;
            pattern.Append(@")\b)");
            return new Regex(pattern.ToString(), RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }

        public string Parse(string text)
        {
            if (richTextBox != null && richTextBox.Font != richTextBoxAux.Font)
                richTextBoxAux.Font = richTextBox.Font;
            richTextBoxAux.Clear();
            richTextBoxAux.Text = text;
            richTextBoxAux.SelectAll();
            string rtf = richTextBoxAux.SelectedRtf.Replace(@"\rtf1", @"\rtf1{\colortbl ;\red255\green0\blue0;\red0\green128\blue0;\red0\green0\blue255;\red0\green128\blue128;\red0\green168\blue32;}");

            Parse(regexNumbers, ref rtf, 1);
            Parse(regexKeywords, ref rtf, 3);
            Parse(regexTypes, ref rtf, 4);
            Parse(regexStrings, ref rtf, 2);
            while (regexKeysQuoted.IsMatch(rtf))
            {
                rtf = regexKeysQuoted.Replace(rtf, "${A}${B}${D}${F}");
            }
            while (regexKeysBrackets.IsMatch(rtf))
            {
                rtf = regexKeysBrackets.Replace(rtf, "${A}${B}${D}${F}");
            }
            Parse(regexSingleLineComments, ref rtf, 5);
            Parse(regexMultiLineComments, ref rtf, 5);

            return rtf;
        }

        private static void Parse(Regex regex, ref string result, int enclosingColorIndex)
        {
            result = regex.Replace(result, $@"\cf{enclosingColorIndex} ${{A}}\cf0 ");
        }

        public RichTextBox RichTextBox
        {
            get
            {
                return richTextBox;
            }
            set
            {
                if (value == richTextBox)
                    return;

                if (richTextBox != null)
                {
                    richTextBox.VScroll -= RichTextBox_Event;
                    richTextBox.TextChanged -= RichTextBox_Event;
                    richTextBox.SizeChanged -= RichTextBox_Event;
                }
                richTextBox = value;
                richTextBox.VScroll += RichTextBox_Event;
                richTextBox.TextChanged += RichTextBox_Event;
                richTextBox.SizeChanged += RichTextBox_Event;
            }
        }

        public void Update()
        {
            int posIni = richTextBox.GetFirstVisibleCharIndex();
            int posEnd = richTextBox.GetLastVisibleCharIndex();
            ParseRichTextBox(posIni, posEnd);
        }

        private bool parsing = false;

        public void ParseRichTextBox(int posIniChar, int posEndChar)
        {
            if (parsing)
                return;
            parsing = true;

            int firstVisibleLineBefore = richTextBox.GetFirstVisibleLine();
            int hScroll = richTextBox.GetHScroll();
            selectionLength = richTextBox.SelectionLength;
            selectionStart = richTextBox.SelectionStart;

            richTextBox.SetRedraw(false);
            richTextBox.Select(posIniChar, posEndChar - posIniChar + 1);
            richTextBox.SelectedRtf = Parse(richTextBox.SelectedText);

            richTextBox.Select(selectionStart, selectionLength);
            richTextBox.SetHScroll(hScroll);
            int firstVisibleLineAfter = richTextBox.GetFirstVisibleLine();
            richTextBox.ScrollLines(firstVisibleLineBefore - firstVisibleLineAfter);

            richTextBox.SetRedraw(true);
            richTextBox.Refresh();

            parsing = false;
        }

        private void RichTextBox_Event(object sender, EventArgs e)
        {
            Update();
        }
    }
}
