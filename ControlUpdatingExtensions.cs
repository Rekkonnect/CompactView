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
using System.Windows.Forms;

namespace CompactView
{
    public static class ControlUpdatingExtensions
    {
        public static void BeginUpdate(this Control control)
        {
            switch (control)
            {
                case ListView listView:
                    listView.BeginUpdate();
                    break;

                case ComboBox comboBox:
                    comboBox.BeginUpdate();
                    break;

                case ListBox listBox:
                    listBox.BeginUpdate();
                    break;
            }
        }
        public static void EndUpdate(this Control control)
        {
            switch (control)
            {
                case ListView listView:
                    listView.EndUpdate();
                    break;

                case ComboBox comboBox:
                    comboBox.EndUpdate();
                    break;

                case ListBox listBox:
                    listBox.EndUpdate();
                    break;
            }
        }
    }
}
