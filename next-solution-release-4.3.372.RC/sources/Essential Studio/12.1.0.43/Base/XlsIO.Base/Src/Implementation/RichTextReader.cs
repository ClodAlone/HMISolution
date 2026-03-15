#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;

#if WINRT
using Windows.UI;
#else
using System.Drawing;
#endif
#if SILVERLIGHT || WP
using System.Windows.Media;
#endif
/// <summary>
/// Class wich reads Rich Text.
/// </summary>
public class RichTextReader
{
    #region Constants
    /// <summary>
    /// Represents the string seperator.
    /// </summary>
    internal const string Seperator = ";";
    /// <summary>
    /// Represents the control word start.
    /// </summary>
    internal const string ControlStart = @"\";
    /// <summary>
    /// Represents the para end without the properties.
    /// </summary>
    internal const string ParaEnd = "pard";
    /// <summary>
    /// Represents the font index.
    /// </summary>
    internal const string FontIndex = "f";
    /// <summary>
    /// Font size in half-points (the default is 24).
    /// </summary>
    internal const string FontSize = "fs";
    /// <summary>
    /// Represents font bold attribute
    /// </summary>
    internal const string Bold = "b";
    /// <summary>
    /// Represents font italic attribute
    /// </summary>
    internal const string Italic = "i";
    /// <summary>
    /// Represents the tab character.
    /// </summary>
    internal const string Tab = "tab";
    /// <summary>
    /// Represents the Foreground color (default is 0).
    /// </summary>
    internal const string ForegroundColor = "cf";
    /// <summary>
    /// Represents the para end with properties.
    /// </summary>
    internal const string Para = "par\r\n";
    /// <summary>
    /// Represents the para end with properties.
    /// </summary>
    internal const string Para2 = "par\n";
    /// <summary>
    /// Represents the para end with properties.
    /// </summary>
    internal const string Para3 = "par";
    /// <summary>
    /// Continuous underline. \ul0 turns off all underlining.
    /// </summary>
    internal const string UnderLine = "ul";
    /// <summary>
    /// Continuous underline with italics font attribute
    /// </summary>
    internal const string ItalicsUnderline = "iul";
    /// <summary>
    /// Stops underlining.
    /// </summary>
    internal const string StopUnderLine = "ulnone";
    /// <summary>
    /// Represents the text strike through.
    /// </summary>
    internal const string Strike = "strike";
    /// <summary>
    /// Subscripts text and shrinks point size according to font information.
    /// </summary>
    internal const string Subscript = "sub";
    /// <summary>
    /// Turns off superscripting or subscripting.
    /// </summary>
    internal const string NoSuperSub = "nosupersub";
    /// <summary>
    /// Superscripts text and shrinks point size according to font information.
    /// </summary>
    internal const string Superscript = "super";
    /// <summary>
    /// Marks a destination whose text should be ignored.
    /// </summary>
    internal const string DestinationMark = "*";
    /// <summary>
    /// Represents the Red color.
    /// </summary>
    internal const string Red = "red";
    /// <summary>
    /// Represents the Green color.
    /// </summary>
    internal const string Green = "green";
    /// <summary>
    /// Represents the blue color.
    /// </summary>
    internal const string Blue = "blue";
    #endregion

    #region Members
    /// <summary>
    /// Represents the Application.
    /// </summary>
    private IApplication m_application;
    /// <summary>
    /// Dictionary of Colors table.
    /// </summary>
    private Dictionary<int, Color> m_colorDict = new Dictionary<int, Color>();
    /// <summary>
    /// Entire Rtf Text.
    /// </summary>
    private string[] m_complete;
    /// <summary>
    /// Represents the font Dictionary.
    /// </summary>
    private Dictionary<int, IFont> m_fontDict = new Dictionary<int, IFont>();
    /// <summary>
    /// Index of the RTF Text.
    /// </summary>
    private int m_index;
    /// <summary>
    /// Represents the workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Represents the current Font Index.
    /// </summary>
    private int m_iFontIndex;
    /// <summary>
    /// Represents the Rtf Text.
    /// </summary>
    private string m_rtfText;
    /// <summary>
    /// Represents the WorksheetImpl.
    /// </summary>
    private WorksheetImpl m_sheet;
    /// <summary>
    /// Represents the Range to store the RTF Text.
    /// </summary>
    private IRange m_range;
    /// <summary>
    /// Represents the RichTextString class Object.
    /// </summary>
    private RichTextString m_rtf;
    #endregion

    #region Intialization
    /// <summary>
    /// Intializes the RichTextReader Members.
    /// </summary>
    /// <param name="parentSheet">sheet.</param>
    public RichTextReader(IWorksheet parentSheet)
    {
        this.m_sheet = parentSheet as WorksheetImpl;
        this.m_book = parentSheet.Workbook as WorkbookImpl;
        this.m_application = parentSheet.Application;
    }
    #endregion
    #region Methods.
    /// <summary>
    /// Finds the color.
    /// </summary>
    /// <param name="findColor">Color of the find.</param>
    /// <returns></returns>
    private Color FindColor(string findColor)
    {
        string[] strArray = findColor.Split(ControlStart.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        byte red = 0;
        byte green = 0;
        byte blue = 0;
        if (strArray.Length == 3)
        {
            red = byte.Parse(strArray[0].TrimStart(Red.ToCharArray()));
            green = byte.Parse(strArray[1].TrimStart(Green.ToCharArray()));
            blue = byte.Parse(strArray[2].TrimStart(Blue.ToCharArray()));
            return Color.FromArgb((byte)255,red, green, blue);
        }
#if SILVERLIGHT || WINRT || WP
        return new Color();
#else
        return Color.Empty;
#endif
    }
    /// <summary>
    /// Parses this instance.
    /// </summary>
    private void Parse()
    {
        this.m_complete = this.m_rtfText.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        this.m_index = 0;
        while (this.m_index < this.m_complete.Length)
        {
#if SILVERLIGHT
            this.m_complete[this.m_index].Split(new char[] { ' ' },StringSplitOptions.None);
#else
            this.m_complete[this.m_index].Split(new char[] { ' ' }, 2);
#endif
            this.m_index++;
            this.ParseFontTable();
            this.ParseColorTable();
            this.ParseContent();
        }
        if (this.m_range != null && this.m_range.WrapText)
        {
            this.m_range.AutofitColumns();
        }
        this.m_rtf.TextObject.RtfText = this.m_rtfText;
    }

    /// <summary>
    /// Parses the color table.
    /// </summary>
    private void ParseColorTable()
    {
        while (!this.m_complete[this.m_index].Contains(@"\par") &&
               !("{" + this.m_complete[this.m_index]).StartsWith(RtfTextWriter.DEF_TAGS[2]))
        {
            this.m_index++;
        }
#if SILVERLIGHT || WINRT || WP
        this.m_colorDict.Add(0, Color.FromArgb(0,0,0,0));
#else
        this.m_colorDict.Add(0, Color.Black);
#endif
        int count = this.m_colorDict.Count;
        string str = "{" + this.m_complete[this.m_index];
        if (str.StartsWith(RtfTextWriter.DEF_TAGS[2]))
        {
            string[] strArray = str.Split(Seperator.ToCharArray());
            for (int i = 1; i < strArray.Length; i++)
            {
                Color color = this.FindColor(strArray[i]);
#if SILVERLIGHT || WINRT || WP
                if (color.A==0 && color.R==0 && color.G==0 && color.B==0)
#else
                if (color != Color.Empty)
#endif
                {
                    this.m_colorDict.Add(count, color);
                    count++;
                }
            }
            this.m_index++;
        }
    }

    /// <summary>
    /// Parses the content.
    /// </summary>
    private void ParseContent()
    {
        if (this.m_complete[this.m_index].Contains(ControlStart + DestinationMark))
        {
            this.m_index++;
        }
        this.ParseControlWord();
    }

    /// <summary>
    /// Parses the control word.
    /// </summary>
    private void ParseControlWord()
    {
        int index = 0;
#if SILVERLIGHT || WINRT || WP
        Color empty = new Color();
#else
         Color empty = Color.Empty;
#endif
        IFont baseFont = this.m_fontDict[0];
        IFont font2 = baseFont;
        while (this.m_index < this.m_complete.Length)
        {
            bool isTextAppended = false;
            string[] strArray = this.m_complete[this.m_index].Split(ControlStart.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (index < strArray.Length)
            {
#if SILVERLIGHT
                string[] strArray2 = strArray[index].Split(new char[] { ' ' },StringSplitOptions.None);
#else
                string[] strArray2 = strArray[index].Split(new char[] { ' ' }, 2);
#endif
                int num2 = this.ParseNumber(strArray2[0]);
#if SILVERLIGHT
                string str = (num2 != -2147483648) ? strArray2[0].Split(num2.ToString().ToCharArray(), StringSplitOptions.None)[0] : strArray2[0];
#else
                string str = (num2 != -2147483648) ? strArray2[0].Split(num2.ToString().ToCharArray(), 2)[0] : strArray2[0];
#endif

                int iStartPos = (this.m_rtf.Text != null) ? this.m_rtf.Text.Length : 0;

                if ((strArray2[0].Contains(Environment.NewLine) || strArray2[0].Contains("\n")) &&
                    (!strArray2[0].EndsWith(Environment.NewLine) || !strArray2[0].EndsWith("\n")))
                {
#if !SILVERLIGHT
                    string[] strArray3 = strArray[index].Split(new string[] { Environment.NewLine, "\n" }, 2, System.StringSplitOptions.RemoveEmptyEntries);
#else
                    string[] strArray3 = strArray[index].Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
#endif
                    if (strArray3.Length > 1)
                    {
                        if (font2 != baseFont)
                            baseFont = this.m_book.CreateFont(font2);
                        this.m_rtf.SetText(this.m_rtf.Text + '\n' + strArray3[1]);
                        isTextAppended = true;
                    }
                    else if (strArray3.Length > 0 && !strArray3[0].Contains(Para3))
                        this.m_rtf.SetText(this.m_rtf.Text + strArray3[0]);
                }
                else if ((strArray2.Length > 1) &&
                         !strArray2[0].StartsWith(Tab) && !strArray2[1].StartsWith(" ") &&
                         (!strArray2[0].StartsWith(Para) && !strArray2[0].StartsWith(Para2) && !strArray2[0].StartsWith(Para3)))
                    this.m_rtf.SetText(this.m_rtf.Text + strArray2[1]);

                int iEndPos = (this.m_rtf.Text != null) ? this.m_rtf.Text.Length - 1 : 0;
                switch (str)
                {
                    case ParaEnd:
                        baseFont = this.m_fontDict[0];
                        break;

                    case FontIndex:
                        baseFont = this.m_book.CreateFont(this.m_fontDict[0]);
                        baseFont.FontName = this.m_fontDict[num2].FontName;
                        baseFont.RGBColor = empty;
                        font2 = baseFont;
                        break;

                    case FontSize:
                        baseFont = this.m_book.CreateFont(font2);
                        baseFont.Size = (double)num2 / 2;
                        font2 = baseFont;
                        break;

                    case Tab:
                        this.m_rtf.SetText(this.m_rtf.Text + "\t");
                        if (strArray2.Length > 1)
                            this.m_rtf.SetText(this.m_rtf.Text + strArray2[1].Replace(Environment.NewLine, ""));
                        break;

                    case ForegroundColor:
                        baseFont = this.m_book.CreateFont(font2);
                        baseFont.RGBColor = this.m_colorDict[num2];
                        empty = baseFont.RGBColor;
                        font2 = baseFont;
                        break;

                    case Para:
                    case Para2:
                        if (!isTextAppended)
                        {
                            baseFont = this.m_book.CreateFont(font2);
                            string[] splitStrings = strArray2[0].Split(str.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            string text = string.Empty;
                            if (index != strArray.Length - 1)
                                text = this.m_rtf.Text + '\n';
                            else
                                text = this.m_rtf.Text;
                            this.m_rtf.SetText(text + (splitStrings.Length > 0 ? splitStrings[0] : ""));
                            if (strArray2.Length > 1)
                                this.m_rtf.SetText(this.m_rtf.Text + " " + strArray2[1]);
                            if (this.m_range != null && !this.m_range.WrapText)
                            {
                                this.m_range.WrapText = true;
                            }
                        }
                        break;

                    case UnderLine:
                        baseFont = this.m_book.CreateFont(baseFont);

                        if (strArray2.Length > 1 && strArray2[1].StartsWith(" "))
                        {
                            this.m_rtf.SetText(this.m_rtf.Text + strArray2[1]);
                            iEndPos = (this.m_rtf.Text != null) ? this.m_rtf.Text.Length - 1 : 0;
                        }

                        baseFont.Underline = ExcelUnderline.Single;
                        font2 = baseFont;
                        break;

                    case StopUnderLine:
                        baseFont = this.m_book.CreateFont(baseFont);
                        baseFont.Underline = ExcelUnderline.None;
                        font2 = baseFont;
                        break;

                    case Strike:
                        baseFont = this.m_book.CreateFont(baseFont);
                        baseFont.Strikethrough = !baseFont.Strikethrough;
                        font2 = baseFont;
                        break;

                    case Subscript:
                        baseFont = this.m_book.CreateFont(baseFont);
                        baseFont.Subscript = !baseFont.Subscript;
                        font2 = baseFont;
                        break;

                    case NoSuperSub:
                        baseFont = this.m_book.CreateFont(baseFont);
                        baseFont.Subscript = !baseFont.Subscript && baseFont.Subscript;
                        baseFont.Superscript = !baseFont.Superscript && baseFont.Superscript;
                        font2 = baseFont;
                        break;

                    case Superscript:
                        baseFont = this.m_book.CreateFont(baseFont);
                        baseFont.Superscript = !baseFont.Superscript;
                        font2 = baseFont;
                        break;

                    case DestinationMark:
                        index++;
                        break;

                    case Bold:
                        baseFont = this.m_book.CreateFont(font2);
                        if (num2 != 0)
                            baseFont.Bold = true;
                        else
                            baseFont.Bold = false;
                        font2 = baseFont;
                        break;

                    case Italic:
                    case ItalicsUnderline:
                        baseFont = this.m_book.CreateFont(font2);

                        if (strArray2.Length > 1 && strArray2[1].StartsWith(" "))
                        {
                            this.m_rtf.SetText(this.m_rtf.Text + strArray2[1]);
                            iEndPos = (this.m_rtf.Text != null) ? this.m_rtf.Text.Length - 1 : 0;
                        }

                        if (num2 != 0)
                            baseFont.Italic = true;
                        else
                            baseFont.Italic = false;
                        if (str == ItalicsUnderline)
                            baseFont.Underline = ExcelUnderline.Single;
                        font2 = baseFont;
                        break;

                    default:
                        if (font2 != baseFont)
                            baseFont = this.m_book.CreateFont(font2);
                        break;
                }
                if (((strArray2.Length > 1)) && (iStartPos <= iEndPos))
                {
                    this.m_rtf.SetFont(iStartPos, iEndPos, baseFont);
                }
                else if (((strArray2.Length == 1)) && (iStartPos > iEndPos) && iEndPos >= 0)
                {
                    this.m_rtf.SetFont(iStartPos, iStartPos, baseFont);
                }
                index++;
                continue;
            }
            this.m_index++;
            index = 0;
        }
    }

    /// <summary>
    /// Parses the font table.
    /// </summary>
    private void ParseFontTable()
    {
        this.m_index++;
        int key = (this.m_fontDict.Count > 0) ? (this.m_fontDict.Count + 1) : 0;
        while (this.m_complete[this.m_index].StartsWith(ControlStart + FontIndex) || 
               this.m_complete[this.m_index].StartsWith(ControlStart + "*"))
        {
            string str = this.m_complete[this.m_index];
            int startIndex = str.IndexOf(' ') + 1;
            int length = str.LastIndexOf(Seperator[0]) - startIndex;
            IFont font = this.m_book.CreateFont();
            if (length != -1)
                font.FontName = str.Substring(startIndex, length);
            this.m_fontDict.Add(key, font);
            key++;
            this.m_index++;
        }
        if (key == 0)
        {
            this.m_fontDict.Add(this.m_iFontIndex, this.DefaultFont);
        }
    }

    /// <summary>
    /// Parses the number.
    /// </summary>
    /// <param name="numText">The num text.</param>
    /// <returns></returns>
    private int ParseNumber(string numText)
    {
        string[] strArray = numText.Split(ControlStart.ToCharArray());
        string s = "";
        if (strArray.Length > 0)
        {
            foreach (char ch in strArray[0].ToCharArray())
            {
                int result = 0;
                if (int.TryParse(ch.ToString(), out result))
                {
                    s = s + result.ToString();
                }
            }
        }
        if ((s != null) && (s != string.Empty))
        {
            return int.Parse(s);
        }
        return -2147483648;
    }

    /// <summary>
    /// Sets the RTF.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <param name="text">The text.</param>
    public void SetRTF(int row, int column, string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException("RTF Text");
        }
        this.m_rtfText = text;
        this.m_range = this.m_sheet[row, column];
        this.m_rtf = this.m_range.RichText as RichTextString;
        this.Parse();
    }
    /// <summary>
    /// Sets the RTF.
    /// </summary>
    /// <param name="text">The text.</param>
    public void SetRTF(object shape, string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException("RTF Text");
        }
        this.m_rtfText = text;
        this.m_rtf = (CreateRichTextString() as RichTextString);
        this.Parse();
        
        switch (shape.GetType().Name)
        {
            case "TextBoxShapeImpl":
                (shape as ITextBoxShape).RichText = m_rtf;
                break;

            default:
                break;
        }
    }
    /// <summary>
    /// Creates rich text string.
    /// </summary>
    protected IRichTextString CreateRichTextString()
    {
        IRTFWrapper rtfString = new RangeRichTextString(this.m_sheet.Application, m_sheet, -1, -1);
        return rtfString;
    }
    #endregion
    #region Properties
    protected virtual FontImpl DefaultFont
    {
        get
        {
            return (FontImpl)this.m_book.InnerFonts[this.m_iFontIndex];
        }
    }
    #endregion
}


