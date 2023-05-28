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
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public delegate void BeforePagePrintDelegate(int posIniChar, int posEndChar, PrintPageEventArgs e);

public static class RichTextBoxExtensions
{
    /// <summary>
    /// Print the content of the RichTextBox
    /// </summary>
    /// <param name="printType">Printing type to define how to perform the printing process</param>
    public static void Print(this RichTextBox richTextBox, PrintType printType)
    {
        new RichTextBoxHelper(richTextBox).Print(printType, null, null);
    }

    /// <summary>
    /// Print the content of the RichTextBox
    /// </summary>
    /// <param name="printType">Printing type to define how to perform the printing process</param>
    /// <param name="margins">Page margins or null value for use default margins</param>
    public static void Print(this RichTextBox richTextBox, PrintType printType, Margins margins)
    {
        new RichTextBoxHelper(richTextBox).Print(printType, margins, null);
    }

    /// <summary>
    /// Print the content of the RichTextBox
    /// </summary>
    /// <param name="printType">Printing type to define how to perform the printing process</param>
    /// <param name="printType">Delegate invoked before print each page</param>
    public static void Print(this RichTextBox richTextBox, PrintType printType, BeforePagePrintDelegate beforePagePrintDelegate)
    {
        new RichTextBoxHelper(richTextBox).Print(printType, null, beforePagePrintDelegate);
    }

    /// <summary>
    /// Print the content of the RichTextBox
    /// </summary>
    /// <param name="printType">Printing type to define how to perform the printing process</param>
    /// <param name="margins">Page margins or null value for use default margins</param>
    /// <param name="printType">Delegate invoked before print each page</param>
    public static void Print(this RichTextBox richTextBox, PrintType printType, Margins margins, BeforePagePrintDelegate beforePagePrintDelegate)
    {
        new RichTextBoxHelper(richTextBox).Print(printType, margins, beforePagePrintDelegate);
    }

    /// <summary>
    /// Sets the font name for the selected text of the RichTextBox
    /// </summary>
    /// <param name="fontName">Name of the font to use</param>
    /// <returns>Returns true on success, false on failure</returns>
    public static bool SelectionFontName(this RichTextBox richTextBox, string fontName)
    {
        return RichTextBoxHelper.SelectionFontName(richTextBox, fontName);
    }

    /// <summary>
    /// Sets the font size for the selected text of the RichTextBox
    /// </summary>
    /// <param name="fontSize">Font size to use</param>
    /// <returns>Returns true on success, false on failure</returns>
    public static bool SelectionFontSize(this RichTextBox richTextBox, int fontSize)
    {
        return RichTextBoxHelper.SelectionFontSize(richTextBox, fontSize);
    }

    /// <summary>
    /// Sets the font style for the selected text of the RichTextBox
    /// </summary>
    /// <param name="fontStyle">Font style to apply to selected text</param>
    /// <returns>Returns true on success, false on failure</returns>
    public static bool SelectionFontStyle(this RichTextBox richTextBox, FontStyle fontStyle)
    {
        return RichTextBoxHelper.SelectionFontStyle(richTextBox, fontStyle);
    }

    /// <summary>
    /// Sets the font color for the selected text of the RichTextBox
    /// </summary>
    /// <param name="color">Color to apply</param>
    /// <returns>Returns true on success, false on failure</returns>
    public static bool SelectionFontColor(this RichTextBox richTextBox, Color color)
    {
        return RichTextBoxHelper.SelectionFontColor(richTextBox, color);
    }

    /// <summary>
    /// Sets the font color for the word in the selected point
    /// </summary>
    /// <param name="color">Color to apply</param>
    /// <returns>Returns true on success, false on failure</returns>
    public static bool WordFontColor(this RichTextBox richTextBox, Color color)
    {
        return RichTextBoxHelper.WordFontColor(richTextBox, color);
    }

    /// <summary>
    /// Sets the background color for the selected text of the RichTextBox
    /// </summary>
    /// <param name="color">Color to apply</param>
    /// <returns>Returns true on success, false on failure</returns>
    public static bool SelectionBackColor(this RichTextBox richTextBox, Color color)
    {
        return RichTextBoxHelper.SelectionBackColor(richTextBox, color);
    }

    public static void SetRedraw(this RichTextBox richTextBox, bool enableRedraw)
    {
        RichTextBoxHelper.SetRedraw(richTextBox, enableRedraw);
    }

    public static int GetFirstVisibleCharIndex(this RichTextBox richTextBox)
    {
        return RichTextBoxHelper.GetFirstVisibleCharIndex(richTextBox);
    }

    public static int GetLastVisibleCharIndex(this RichTextBox richTextBox)
    {
        return RichTextBoxHelper.GetLastVisibleCharIndex(richTextBox);
    }

    public static int GetFirstVisibleLine(this RichTextBox richTextBox)
    {
        return RichTextBoxHelper.GetFirstVisibleLine(richTextBox);
    }

    public static int GetVisibleLines(this RichTextBox richTextBox)
    {
        return RichTextBoxHelper.GetVisibleLines(richTextBox);
    }

    public static void HideSelection(this RichTextBox richTextBox, bool hide)
    {
        RichTextBoxHelper.HideSelection(richTextBox, hide);
    }

    public static void ScrollLines(this RichTextBox richTextBox, int linesToScroll)
    {
        RichTextBoxHelper.ScrollLines(richTextBox, linesToScroll);
    }

    public static int GetHScroll(this RichTextBox richTextBox)
    {
        return RichTextBoxHelper.GetHScroll(richTextBox);
    }

    public static void SetHScroll(this RichTextBox richTextBox, int position)
    {
        RichTextBoxHelper.SetHScroll(richTextBox, position);
    }

    public static int GetEventMask(this RichTextBox richTextBox)
    {
        return RichTextBoxHelper.GetEventMask(richTextBox);
    }

    public static void SetEventMask(this RichTextBox richTextBox, int mask)
    {
        RichTextBoxHelper.SetEventMask(richTextBox, mask);
    }
}
