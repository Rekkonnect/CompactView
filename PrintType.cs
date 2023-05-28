/**************************************************************************
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
/// <summary>
/// Printing type to define how to perform the printing process
/// </summary>
public enum PrintType
{
    /// <summary>Print the content directly without showing any dialog</summary>
    DirectPrint,

    /// <summary>Show print dialog before to do printing</summary>
    ShowPrintDialog,

    /// <summary>Show print dialog before to do printing and calculate total number of pages before show dialog</summary>
    ShowPrintDialogWithTotalPages,

    /// <summary>Show a print preview</summary>
    PrintPreview
};
