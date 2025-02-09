﻿/**************************************************************************
Copyright (C) 2011-2014 Iván Costales Suárez

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
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace CompactView
{
    public class Settings
    {
        // Window
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public bool Maximized;

        // Appearance
        public int TextColor1;
        public int TextColor2;
        public int BackColor1;
        public int BackColor2;
        public int ColorSet;

        // Behavior
        public bool OmitSelectedTextExecutionPopup;

        // Recent Files
        public StringCollection RecentFiles;
        public int MaxRecentFiles;

        public Settings()
        {
            MaxRecentFiles = 10;
            RecentFiles = new StringCollection();
        }

        public bool AddToRecentFiles(string fileName)
        {
            int i = RecentFiles.IndexOf(fileName);
            if (i >= 0)
                RecentFiles.RemoveAt(i);

            while (RecentFiles.Count >= MaxRecentFiles)
                RecentFiles.RemoveAt(MaxRecentFiles - 1);
            RecentFiles.Insert(0, fileName);
            return true;
        }

        private string FileName
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName + @"\Settings.xml"); }
        }

        public void Load()
        {
            var table = new DataTable("Settings");

            try
            {
                if (!Directory.Exists(FileName))
                    Directory.CreateDirectory(Path.GetDirectoryName(FileName));
                table.ReadXml(FileName);
            }
            catch
            {
            }

            if (table.Rows.Count <= 0)
                return;
            DataRow row = table.Rows[0];

            table.ReadField(row, "X", ref X);
            table.ReadField(row, "Y", ref Y);
            table.ReadField(row, "Width", ref Width);
            table.ReadField(row, "Height", ref Height);
            table.ReadField(row, "Maximized", ref Maximized);

            table.ReadField(row, "TextColor1", ref TextColor1);
            table.ReadField(row, "TextColor2", ref TextColor2);
            table.ReadField(row, "BackColor1", ref BackColor1);
            table.ReadField(row, "BackColor2", ref BackColor2);
            table.ReadField(row, "ColorSet", ref ColorSet);

            table.ReadField(row, nameof(OmitSelectedTextExecutionPopup), ref OmitSelectedTextExecutionPopup);

            RecentFiles.Clear();
            for (int i = 1; i <= MaxRecentFiles; i++)
            {
                if (table.Columns.Contains($"RecentFiles{i}"))
                    RecentFiles.Add(row.Field<string>($"RecentFiles{i}"));
            }
        }

        public void Save()
        {
            var table = new DataTable("Settings");

            table.Columns.Add("X", typeof(int));
            table.Columns.Add("Y", typeof(int));
            table.Columns.Add("Width", typeof(int));
            table.Columns.Add("Height", typeof(int));
            table.Columns.Add("Maximized", typeof(bool));

            table.Columns.Add("TextColor1", typeof(int));
            table.Columns.Add("TextColor2", typeof(int));
            table.Columns.Add("BackColor1", typeof(int));
            table.Columns.Add("BackColor2", typeof(int));
            table.Columns.Add("ColorSet", typeof(int));

            table.Columns.Add(nameof(OmitSelectedTextExecutionPopup), typeof(bool));

            for (int i = 1; i <= RecentFiles.Count; i++)
                table.Columns.Add($"RecentFiles{i}", typeof(string));

            DataRow row = table.NewRow();
            row[0] = X;
            row[1] = Y;
            row[2] = Width;
            row[3] = Height;
            row[4] = Maximized;

            row[5] = TextColor1;
            row[6] = TextColor2;
            row[7] = BackColor1;
            row[8] = BackColor2;
            row[9] = ColorSet;

            row[10] = OmitSelectedTextExecutionPopup;

            for (int i = 0; i < RecentFiles.Count; i++)
                row[i + 11] = RecentFiles[i];

            table.Rows.Add(row);

            try
            {
                using (var xw = new XmlTextWriter(FileName, Encoding.UTF8))
                {
                    xw.Formatting = Formatting.Indented;
                    table.WriteXml(xw, XmlWriteMode.WriteSchema);
                }
            }
            catch
            {
            }
        }
    }
}
