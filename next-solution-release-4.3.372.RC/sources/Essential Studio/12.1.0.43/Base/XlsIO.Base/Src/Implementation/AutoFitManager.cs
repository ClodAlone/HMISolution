#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if !SILVERLIGHT && !WINRT && !WP
using System.Drawing;
#endif
using System.Text;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections;

#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Metro.Implementation.Drawing;
using Windows.UI.Xaml.Media;
#elif (SILVERLIGHT || WP)
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
#endif

#if SILVERLIGHT
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Silverlight.Implementation.Drawing;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.WP.Implementation.Drawing;
#endif
namespace Syncfusion.XlsIO.Implementation
{
    internal class AutoFitManager : IDisposable
    {
        /// <summary>
        /// Default size of autofilter arrow width.
        /// </summary>
        private const double DEF_AUTO_FILTER_WIDTH = 1.363;
        /// <summary>
        /// This text should be added to value of the header of the table to equal the width of the dropdown symbol.
        /// </summary>
        private const string DROPDOWNSYMBOL = "AA";

#if !WINRT && !SILVERLIGHT && !WP
        private Graphics m_graphics;
#endif
        RangeImpl m_rangeImpl;
        WorksheetImpl m_worksheet;
        WorkbookImpl m_book;
        int m_row;
        int m_column;
        int m_lastRow;
        int m_lastColumn;
        /// <summary>
        /// Intializes the AutoFit Manager
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="lastRow"></param>
        /// <param name="lastColumn"></param>
        /// <param name="rangeImpl"></param>
        internal AutoFitManager(int row, int column, int lastRow, int lastColumn,
            RangeImpl rangeImpl)
        {
            this.m_row = row;
            this.m_column = column;
            this.m_lastRow = lastRow;
            this.m_lastColumn = lastColumn;
            m_rangeImpl = rangeImpl;
            m_worksheet = rangeImpl.Worksheet as WorksheetImpl;
            m_book = rangeImpl.Workbook;
#if !SILVERLIGHT && !WINRT && !WP
            using (Bitmap bitmap = new Bitmap(100, 0x7d0))
            {
                m_graphics = Graphics.FromImage(bitmap);
            }
#endif
        }
        internal AutoFitManager()
        {
#if !SILVERLIGHT && !WINRT && !WP
            using (Bitmap bitmap = new Bitmap(100, 0x7d0))
            {
                m_graphics = Graphics.FromImage(bitmap);
            }
#endif
        }
        internal IWorksheet Worksheet
        {
            get
            {
                return m_worksheet;
            }
        }
        /// <summary>
        /// Measures to fit column.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="firstRow">The first row.</param>
        /// <param name="lastRow">The last row.</param>
        /// <param name="firstColumn">The first column.</param>
        /// <param name="lastColumn">The last column.</param>
        internal void MeasureToFitColumn()
        {
            int num = 14;
            int firstRow = m_row;
            int lastRow = m_lastRow;
            int firstColumn = m_column;
            int lastColumn = m_lastColumn;
            bool containsTable = false;
            Dictionary<int, IList<object>> measurable = new Dictionary<int, IList<object>>();
            Dictionary<int, int> columnsWidth = new Dictionary<int, int>();
            RectangleF ef = new RectangleF((float)0f, (float)0f, (float)1800f, (float)100f);
            IRange filterRange = m_worksheet.AutoFilters.FilterRange;
            WorksheetImpl sheetImpl = m_worksheet as WorksheetImpl;
            IMigrantRange migrantRange = m_worksheet.MigrantRange;
            if (sheetImpl.ListObjects.Count > 0)
            {
                IListObject listObject = Worksheet.ListObjects[0];
                IRange location = listObject.Location;
                if (location.Row >= firstRow && location.LastRow <= lastRow && location.Column <= firstColumn &&
                    location.LastColumn >= lastColumn)
                    if (listObject.ShowTotals)
                        num = num + 6;
            }

            if (sheetImpl.ListObjects.Count > 0)
            {
                foreach (IListObject listObject in sheetImpl.ListObjects)
                {
                    IRange location = listObject.Location;
                    if (location.Row >= firstRow && location.LastRow <= lastRow && location.Column <= firstColumn &&
                        location.LastColumn >= lastColumn)
                    {
                        containsTable = true;
                    }
                }
            }
            for (int row = firstRow; row <= lastRow; row++)
            {
                for (int column = firstColumn; column <= lastColumn; column++)
                {
                    ushort xfIndex = (ushort)m_worksheet.GetXFIndex(row, column);
                    int iFormatIndex = m_book.GetExtFormat(xfIndex).NumberFormatIndex;
                    ExtendedFormatImpl xFormat = m_book.InnerExtFormats[xfIndex] as ExtendedFormatImpl;
                    int iFontIndex = xFormat.FontIndex;

                    // Use 'as' to increase performance.
                    FontImpl fontImpl = m_book.InnerFonts[iFontIndex] as FontImpl;

                    FormatImpl formatImpl = m_book.InnerFormats[iFormatIndex];
                    IStyle style = m_worksheet.InnerGetCellStyle(column, row, xfIndex, m_rangeImpl[row, column] as RangeImpl);
                    int num4 = 0;
                    if (!RangeImpl.IsMergedCell(sheetImpl.MergeCells, row, column, false, ref num4) || num4 != 0)
                    {
                        if (!columnsWidth.ContainsKey(column))
                        {
                            columnsWidth.Add(column, 0);
                        }
                        string text = m_rangeImpl.GetDisplayText(row, column, formatImpl);
                        switch (text)
                        {
                            case null:
                            case "":
                                continue;
                        }

                        if (containsTable && sheetImpl.ListObjects.Count > 0)
                         {
                             foreach (IListObject listObject in sheetImpl.ListObjects)
                             {
                                 IRange location = listObject.Location;
                                 if (location.Row ==row  && location.LastRow <= lastRow && location.Column <= column &&
                                     location.LastColumn >= column)
                                 {
                                     text = string.Format(@"{0}{1}", text, DROPDOWNSYMBOL);
                                 }
                             }
                         }
                       migrantRange.ResetRowColumn(row, column);
                        bool hasWrapText=(migrantRange.CellStyle.WrapText || migrantRange.WrapText);
                       if ((style.Rotation == 0 || style.Rotation == 0xff) && !hasWrapText)
                        {
                            IList<object> arrList = (measurable.ContainsKey(column)) ? measurable[column] : null;
                            if (arrList == null)
                            {
                                arrList = new List<object>();
                                measurable.Add(column, arrList);
                            }
                            if (style.HorizontalAlignment == ExcelHAlign.HAlignCenterAcrossSelection)
                            {
                                IRange cellRange = m_rangeImpl[row, column++];
                                if (column != cellRange.Column + 1)
                                    continue;
                            }
                            if (filterRange != null && filterRange.Row == row && column >= filterRange.Column
                                && column <= filterRange.LastColumn)
                            {
                                SortTextToFit(arrList, fontImpl, text, true);
                            }
                            else
                            {
                                SortTextToFit(arrList, fontImpl, text, false);
                            }
                        }
                        else if (hasWrapText)
                        {
                            int columnWidth = m_worksheet.GetColumnWidthInPixels(column);
                            string[] words = text.Split(' ','\n');
                            string autoFitText; 
                            int biggestLength = 0;
                            for (int index = 0; index < words.Length; index++)
                            {
                                autoFitText = words[index].ToString();
                                if (autoFitText.Length >0)
                                {
                                    int length = MeasureCharacterRanges(style, autoFitText, num, ef);
                                    if (length < columnWidth)
                                    {
                                        for (int temp = index + 1; temp < words.Length; temp++)
                                        {
                                            index = temp;
                                            autoFitText = string.Format(@"{0}{1}{2}", autoFitText, " ", words[temp]);
                                            int currentLength = MeasureCharacterRanges(style, autoFitText, num, ef);
                                            if (currentLength < columnWidth)
                                            {
                                                if (currentLength > biggestLength)
                                                {
                                                    biggestLength = currentLength;
                                                    temp = words.Length;
                                                }
                                            }
                                            else
                                            {
                                                index = temp - 1;
                                                temp = words.Length;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        biggestLength = columnWidth;
                                        index = words.Length;
                                    }
                                }
                                columnsWidth[column] = biggestLength;
                            }
                        }
                        else
                        {
                            int num5 = MeasureCharacterRanges(style, text, num, ef);
                            int num6 = (columnsWidth.ContainsKey(column)) ? columnsWidth[column] : 0;
                            if (num6 < num5)
                            {
                                columnsWidth[column] = num5;
                            }
                        }
                    }
                }
            }
            IDictionaryEnumerator enumerator = measurable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IList<object> list3 = (IList<object>)enumerator.Value;
                int key = (int)enumerator.Key;
                int num8 = 0;
                for (int k = 0; k < list3.Count; k++)
                {
                    StyleWithText styleWithText = (StyleWithText)list3[k];
                    int num10 = MeasureCharacterRanges(styleWithText, num, ef);
                    if (num8 < num10)
                        num8 = num10;
                }
                int num11 = (columnsWidth.ContainsKey(key)) ? columnsWidth[key] : 0;
                if (num8 > num11)
                    columnsWidth[key] = num8;
            }
            IDictionaryEnumerator enumerator2 = columnsWidth.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                int num12 = (int)enumerator2.Value;
                if (num12 != 0)
                {
                    sheetImpl.SetColumnWidthInPixels((int)enumerator2.Key, num12);
                }
            }
        }
        private Font CreateFont(string fontName, float size, FontStyle fontStyle)
        {
#if ( WINRT )
            Font font = new Font();
            font.Name = fontName;
            font.Size = size;
            font.Bold = FontStyle.Bold == FontStyle.Bold;
            font.Italic = FontStyle.Italic == FontStyle.Italic;
            return font;
#elif (SILVERLIGHT || WP)
            Font font = new Font();
            font.Name = fontName;
            font.Size = size;
            font.Bold = FontStyle.Bold == FontStyle.Bold;
            font.Italic = FontStyle.Italic == FontStyle.Italic;
            return font;
#else
              try
            {
                return new Font(fontName, (float)size, fontStyle);
            }
            catch
            {
                return new Font(fontName, (float)size);
            }
            
#endif
        }
        /// <summary>
        /// Measures the character ranges.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="styleWithText">The style with text.</param>
        /// <param name="paramNum">The param num.</param>
        /// <param name="rectF">The rect F.</param>
        /// <returns></returns>
        private int MeasureCharacterRanges(StyleWithText styleWithText,
            int paramNum, RectangleF rectF)
        {
            int num = 0;
            using (Font font = CreateFont(styleWithText.fontName, (float)styleWithText.size, styleWithText.style))
            {
               
                for (int i = 0; i < styleWithText.strValues.Count; i++)
                {
                    string text = (string)styleWithText.strValues[i];
#if ( WINRT )
                int num3 = ((int)(MeasureString(text, font, rectF, false).Width + 0.05));
#else
                    int num3 = ((int)(MeasureString(text, font, rectF, false).Width + 0.05)) + paramNum;
#endif
                    if (num3 > 100)
                        num3++;
                    if (num < num3)
                        num = num3;
                }
            }
            return num;
        }
#if !WINRT && !SILVERLIGHT && !WP
        private StringFormat CreateStringFormat(string text, bool isAutoFitRow)
        {
            StringFormat stringformat = null;
            if (isAutoFitRow)
            {
                CharacterRange[] rangeArray = new CharacterRange[] { new CharacterRange(0, text.Length) };
                stringformat = new StringFormat();
                stringformat.FormatFlags = stringformat.FormatFlags | StringFormatFlags.MeasureTrailingSpaces;
                stringformat.Trimming = StringTrimming.None;
                stringformat.SetMeasurableCharacterRanges(rangeArray);
                Region[] regionArray = new Region[1];
            }
            else
            {
                CharacterRange[] ranges = new CharacterRange[] { new CharacterRange(0, text.Length) };
                stringformat = new StringFormat();
                stringformat.SetMeasurableCharacterRanges(ranges);
            }
            return stringformat;
        }
#endif
        private RectangleF MeasureString(string text, Font font, RectangleF rectF, bool isAutoFitRow)
        {
#if ( WINRT )
        Windows.Foundation.Size size = new Windows.Foundation.Size(double.MaxValue,
            double.MaxValue);
        UIDispatcher.Initialize();
        Action action=new Action(
            delegate
            {
        
                Windows.UI.Xaml.Controls.TextBlock textBlock = new Windows.UI.Xaml.Controls.TextBlock();

                 textBlock.Text = text;
                 textBlock.FontFamily = new FontFamily(font.Name);
                 textBlock.FontSize = font.Size;
                 textBlock.FontStyle = (font.Italic) ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
                 textBlock.FontWeight = (font.Bold) ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal;
                 textBlock.Measure(size);
                 size = textBlock.DesiredSize;
            });
        UIDispatcher.Execute(action);
       RectangleF newRectF=new RectangleF();
            newRectF.Height=size.Height;
            newRectF.Width=size.Width;
            return newRectF;

        
#elif SILVERLIGHT || WP
#if SILVERLIGHT
            SizeF result = SizeF.Empty;
#elif WP
            SizeF result = SizeF.Empty;
#endif

            if (ColorExtension.IsBackgroundThread)
            {
                ManualResetEvent threadCompleteEvent = new ManualResetEvent(false);
                DispatcherOperation operation = System.Windows.Deployment.Current.Dispatcher.BeginInvoke(
                  delegate
                  {
                      result = Measure(text, font.Name, font.Bold, font.Italic, font.Size);
                      threadCompleteEvent.Set();
                  });

                threadCompleteEvent.WaitOne();
                threadCompleteEvent.Close();
            }
            else
            {
                result = Measure(text,font.Name,font.Bold,font.Italic,font.Size);
            }
            RectangleF rectangle = new RectangleF();
            rectangle.Height = result.Height;
            rectangle.Width = result.Width;
            return rectangle;

#else
            StringFormat strFormat = CreateStringFormat(text, isAutoFitRow);
            return m_graphics.MeasureCharacterRanges(text, font, rectF, strFormat)[0].GetBounds(m_graphics);
#endif
        }
#if WP
        internal SizeF CheckThreadMeasureText(string strValue, string fontName, bool isBold, bool isItalic, float fontSize)
        {
            
            SizeF sizeF = new SizeF();
            if (ColorExtension.IsBackgroundThread)
            {
                ManualResetEvent threadCompleteEvent = new ManualResetEvent(false);
                DispatcherOperation operation = System.Windows.Deployment.Current.Dispatcher.BeginInvoke(
                  delegate
                  {
                      sizeF = MeasureText(strValue, fontName, isBold, isItalic, fontSize);
                      threadCompleteEvent.Set();
                  });

                threadCompleteEvent.WaitOne();
                threadCompleteEvent.Close();
            }
            else
            {
                sizeF = MeasureText(strValue, fontName, isBold, isItalic, fontSize);
            }
            return sizeF;
        }
        internal SizeF MeasureText(string strValue, string fontName, bool isBold, bool isItalic, float fontSize)
        {
            Windows.Foundation.Size size = new Windows.Foundation.Size(double.MaxValue, double.MaxValue);
               TextBlock textBlock = new TextBlock()
               {
                   Text = strValue,
                   FontFamily = new FontFamily(fontName),
                   FontSize = ApplicationImpl.ConvertUnitsStatic(fontSize * 96, MeasureUnits.Point, MeasureUnits.Inch),
                   FontWeight = isBold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
                   FontStyle = isItalic ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal,
                   TextWrapping = System.Windows.TextWrapping.NoWrap,
                   FontStretch = System.Windows.FontStretches.Expanded,
               };
               textBlock.Measure(new System.Windows.Size(1800, 1800));
               size.Width = textBlock.ActualWidth;
               size.Height = textBlock.ActualHeight;
            return new SizeF((float)size.Width, (float)size.Height);
        }
#endif
#if  (SILVERLIGHT) || ( WINRT ) || (WP)
    internal SizeF Measure( string strValue,string fontName,bool bold,bool italic,float fontSize )
    {
#if (SILVERLIGHT)
      TextBlock textBlock = new TextBlock()
      {
        Text = strValue,
        FontFamily = new FontFamily( fontName ),
        FontSize = ApplicationImpl.ConvertUnitsStatic(fontSize * 96, MeasureUnits.Point, MeasureUnits.Inch),
        FontWeight = bold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
        FontStyle = italic ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal,
        TextWrapping = System.Windows.TextWrapping.NoWrap,
        FontStretch = System.Windows.FontStretches.Expanded,
      };
#if WPF_PARTIAL
        textBlock.Measure(new System.Windows.Size(1800,1800));
      return new SizeF( ( float )textBlock.DesiredSize.Width, ( float )textBlock.DesiredSize.Height);

#endif
      textBlock.Measure(new System.Windows.Size(1800,1800));

      return new SizeF( ( float )textBlock.ActualWidth, ( float )textBlock.ActualHeight);
#elif WP
        Windows.Foundation.Size size = new Windows.Foundation.Size(double.MaxValue, double.MaxValue);
        Action action = new Action(
       delegate
       {
           TextBlock textBlock = new TextBlock()
           {
               Text = strValue,
               FontFamily = new FontFamily(fontName),
               FontSize = ApplicationImpl.ConvertUnitsStatic(fontSize * 96, MeasureUnits.Point, MeasureUnits.Inch),
               FontWeight = bold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
               FontStyle = italic ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal,
               TextWrapping = System.Windows.TextWrapping.NoWrap,
               FontStretch = System.Windows.FontStretches.Expanded,
           };
           textBlock.Measure(new System.Windows.Size(1800, 1800));
           size.Width = textBlock.ActualWidth;
           size.Height = textBlock.ActualHeight;
       });
        System.Windows.Deployment.Current.Dispatcher.BeginInvoke(action);
        return new SizeF((float)size.Width, (float)size.Height);

#elif ( WINRT )
        Windows.Foundation.Size size = new Windows.Foundation.Size(double.MaxValue,
            double.MaxValue);
        UIDispatcher.Initialize();
        Action action=new Action(
            delegate
            {
        
                Windows.UI.Xaml.Controls.TextBlock textBlock = new Windows.UI.Xaml.Controls.TextBlock();

                 textBlock.Text = strValue;
                 textBlock.FontFamily = new FontFamily(fontName);
                 textBlock.FontSize =(double) fontSize;
                 textBlock.FontStyle = (italic) ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
                 textBlock.FontWeight = (bold) ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal;
                 textBlock.Measure(size);
                 size = textBlock.DesiredSize;
                 textBlock.Measure(size);
            });
        UIDispatcher.Execute(action);
        
        return new SizeF((float)size.Width,(float)size.Height);
#endif
    }

#endif

        /// <summary>
        /// Measures the character ranges.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="style">The style.</param>
        /// <param name="strText">The STR text.</param>
        /// <param name="num">The num.</param>
        /// <param name="rectF">The rect F.</param>
        /// <returns></returns>
        private int MeasureCharacterRanges(IStyle style, string strText,
            int num, RectangleF rectF)
        {
            Font font2;
            FontStyle regular = FontStyle.Regular;
            double size = 10;
            string familyName = "Arial";
            IFont font = style.Font;
            if (font != null)
            {
                if (font.Bold)
                    regular |= FontStyle.Bold;
                if (font.Italic)
                    regular |= FontStyle.Italic;
                if (font.Strikethrough)
                    regular |= FontStyle.Strikeout;
                if (font.Underline != ExcelUnderline.None)
                    regular |= FontStyle.Underline;
                familyName = font.FontName;
                size = font.Size;
            }
            try
            {
                font2 = CreateFont(familyName, (float)size, regular);
            }
            catch
            {
                font2 = CreateFont(familyName, (float)size,regular);
            }
            if (style.Rotation == 90)
            {
                return (((int)((GetFontHeight(font2) * 1.1) + 0.5)) + 6);
            }
           
            RectangleF bounds = MeasureString(strText, font2, rectF, false);
            if (style.Rotation == 0 || style.Rotation == 0xff)
            {
                int num2 = ((int)(bounds.Width + 0.5)) + num;
                if (num2 > 100)
                {
                    num2++;
                }
                return num2;
            }
            int num3 = ((int)(bounds.Width + 0.5)) + num;
            int num4 = (int)((GetFontHeight(font2) * 1.1) + 0.5);
            double d = (3.1415926535897931 * Math.Abs(style.Rotation)) / 180.0;
            font2.Dispose();
            font2 = null;
            return (int)(((num3 * Math.Cos(d)) + (num4 * Math.Sin(d))) + 6.5);
        }
        /// <summary>
        /// Sorts the text to fit.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="fontImpl">The font impl.</param>
        /// <param name="strText">The STR text.</param>
        private static void SortTextToFit(IList<object> list, FontImpl fontImpl, string strText, bool AutoFilter)
        {
            FontStyle regular = FontStyle.Regular;
            double size = 10;
            string name = "Arial";
            IFont font = fontImpl;
            if (font != null)
            {
                if (font.Bold)
                    regular |= FontStyle.Bold;
                if (font.Italic)
                    regular |= FontStyle.Italic;
                if (font.Strikethrough)
                    regular |= FontStyle.Strikeout;
                if (font.Underline != ExcelUnderline.None)
                    regular |= FontStyle.Underline;
                name = font.FontName;
                size = font.Size;
            }
            for (int i = 0; i < list.Count; i++)
            {
                StyleWithText styleWithText = (StyleWithText)list[i];
                if (((styleWithText.fontName == name) && styleWithText.size == size) &&
                    (styleWithText.style == regular))
                {
                    for (int j = 0; j < styleWithText.strValues.Count; j++)
                    {
                        string str = styleWithText.strValues[j];
                        if (str.Length < strText.Length)
                        {
                            styleWithText.strValues.Insert(j, strText);
                            if (styleWithText.strValues.Count > 5)
                            {
                                styleWithText.strValues.RemoveAt(5);
                            }
                            return;
                        }
                    }
                    if (styleWithText.strValues.Count < 5)
                    {
                        styleWithText.strValues.Add(strText);
                    }
                    return;
                }
            }
            StyleWithText swText = new StyleWithText();
            swText.fontName = name;
            if (AutoFilter)
                swText.size = size + DEF_AUTO_FILTER_WIDTH;
            else
                swText.size = size;
            swText.style = regular;
            swText.strValues.Add(strText);
            list.Add(swText);
        }
        /// <summary>
        /// Collection of DisplayText with matching fonts.
        /// This class used to improve the AutoFitToColumn method 
        /// Performance.
        /// </summary>
        class StyleWithText
        {
            internal string fontName;
            internal double size;
            internal FontStyle style;
            internal List<string> strValues;
            internal StyleWithText()
            {
                strValues = new List<string>();
            }
        }
        internal int CalculateWrappedCell(ExtendedFormatImpl format, string stringValue, int columnWidth,ApplicationImpl applicationImpl)
        {
            int num9 = 0;
            int num6 = 0;
            int num = 19;

            switch (stringValue)
            {
                case null:
                case "":
                    {
                        return 0;
                    }
                default:
                    {
                        IFont font = format.Font;
                        int calculatedValue = (int)((stringValue.Length / 406) * (font.Size) + (2 * Convert.ToInt32((font.Bold || font.Italic))));   // cell accepts 406 chars if column width is 1 pixel
                        num9 = (calculatedValue < columnWidth) ? columnWidth : calculatedValue; 
                        num6 = MeasureCell(format, stringValue, (float)num9, num, true);
                        return num6;
                    }

            }
        }

        private int MeasureCell(ExtendedFormatImpl format, string stringValue, float columnWidth, int num, bool isString)
        {
            IFont font = format.Font;
            double size = font.Size;
            FontStyle regular = FontStyle.Regular;

            if (stringValue[stringValue.Length - 1] == '\n')
            {
                stringValue = stringValue + "a";
            }

            if (font.Bold)
            {
                regular |= FontStyle.Bold;
            }
            if (font.Italic)
            {
                regular |= FontStyle.Italic;
            }
            if (font.Strikethrough)
            {
                regular |= FontStyle.Strikeout;
            }
            if (font.Underline != ExcelUnderline.None)
            {
                regular |= FontStyle.Underline;
            }

            if (isString && (font.FontName == "Times New Roman"))
            {
                stringValue = ModifySepicalChar(stringValue);
            }

            using (Font font2 =CreateFont(font.FontName,(float)size,regular))
            {



                float num2 = 0f;
                float num3 = 0f;
                float num4 = 600f;
                if (!format.WrapText)
                {
                    columnWidth = 600f;
                }
                else if (columnWidth < 100f)
                {
                    switch (format.HorizontalAlignment)
                    {
                        case ExcelHAlign.HAlignLeft:
                        case ExcelHAlign.HAlignRight:
                            columnWidth = (float)(columnWidth - 1f);
                            break;
                    }
                }
                else
                {
                    columnWidth = (float)(columnWidth - 2f);
                }

                RectangleF ef = new RectangleF(num2, num3, columnWidth, num4);

                RectangleF bounds = MeasureString(stringValue, font2, ef, true);

                int num5 = (int)((int)((bounds.Height * 1.1) + 0.5));
                if ((font.Size >= 20) || (num5 > 100))
                {
                    num5 = num5 + 1;
                }

                if (format.WrapText)
                {
                    if (size >= 10)
                    {
                        return num5;
                    }
                    int num6 = CalculateFontHeight(font);
                    float num7 =(float) bounds.Height;
                    if (num7 > 100f)
                    {
                        num7 = (float)(num7 + 1f);
                    }
                    int num8 = (int)((int)System.Math.Ceiling((double)((num7 * 1.0) / ((double)num6))));
                    if (num7 > 100f)
                    {
                        num8 = (int)(((int)((num7 * 1.0) / ((double)num6))) + 1);
                    }
                    if (num8 == 1)
                    {
                        return CalculateFontHeightFromGraphics(font);
                    }
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();
                    for (int i = 0; i < num8; i = (int)(i + 1))
                    {
                        builder.Append("\n0");
                    }
                    return MeasureFontSize(format, builder.ToString(), columnWidth);
                }
                int num10 = System.Math.Abs(format.Rotation);
                if (num10 == 90)
                {
                    return (int)(((int)(bounds.Width + 0.5)) + num);
                }
                int num11 = (int)(((int)(bounds.Width + 0.5)) + num);
                int num12 = (int)((int)((GetFontHeight(font2) * 1.1) + 0.5));
                return (int)((int)(((num11 * System.Math.Sin((double)((3.1415926535897931 * num10) / 180.0))) + (num12 * System.Math.Cos((double)((3.1415926535897931 * num10) / 180.0)))) + 6.5));
            }
        }

        private int CalculateFontHeightFromGraphics(IFont font)
        {
            double size = font.Size;
            FontStyle regular = FontStyle.Regular;
            if (font.Bold)
            {
                regular |= FontStyle.Bold;
            }
            if (font.Italic)
            {
                regular |= FontStyle.Italic;
            }
            if (font.Strikethrough)
            {
                regular |= FontStyle.Strikeout;
            }
            if (font.Underline != ExcelUnderline.None)
            {
                regular |= FontStyle.Underline;
            }
            using (Font font2 = CreateFont(font.FontName, (float)size, regular))
            {
                int num3 = (int)((int)((GetFontHeight(font2) * 1.1) + 0.5));
                if (((font.Size >= 20) || (num3 > 100)) || ((font.Size == 12) && font.Bold))
                {
                    num3 = (int)(num3 + 1);
                }
                if (font.Size == 8)
                {
                    num3 = (int)(num3 + 2);
                }
                else if (font.Size < 10)
                {
                    num3 = (int)(num3 + 1);
                }
                return num3;
            }
        }
        private int CalculateFontHeight(IFont font)
        {
            double size = font.Size;
            FontStyle regular = FontStyle.Regular;
            if (font.Bold)
            {
                regular |= FontStyle.Bold;
            }
            if (font.Italic)
            {
                regular |= FontStyle.Italic;
            }
            if (font.Strikethrough)
            {
                regular |= FontStyle.Strikeout;
            }
            if (font.Underline != ExcelUnderline.None)
            {
                regular |= FontStyle.Underline;
            }
            using (Font font2 = CreateFont(font.FontName, (float)size, regular))
            {
                return (int)((int)System.Math.Ceiling((double)GetFontHeight(font2)));
            }
        }
        private float GetFontHeight(Font font)
        {
#if ( WINRT ) || (SILVERLIGHT) || (WP)
            return font.GetHeight();
#else
            return font.GetHeight(m_graphics);
#endif
        }
        private int MeasureFontSize(ExtendedFormatImpl extendedFromat, string stringValue, float columnWidth)
        {
            string str;
            if (stringValue == "")
            {
                return 0;
            }
            double size = extendedFromat.Font.Size;
            if ((((str = extendedFromat.Font.FontName) == null) || (str != "Calibri")) && (size < 10))
            {
                size = (int)((int)((size * 1.1) + 0.5));
                if (size > 10)
                {
                    size = 10;
                }
            }
            using (Font font = CreateFont(extendedFromat.Font.FontName, (float)size,FontStyle.Regular))
            {
                float num2 = 0f;
                float num3 = 0f;
                float num4 = 600f;
                if (!extendedFromat.WrapText)
                {
                    columnWidth = 600f;
                }
                RectangleF ef = new RectangleF(num2, num3, columnWidth, num4);
                return (int)((int)(MeasureString(stringValue, font, ef, true).Height * 1.1 + 0.5));
            }
        }

        private string ModifySepicalChar(string stringValue)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            char[] chArray = stringValue.ToCharArray();

            for (int i = 0; i < stringValue.Length; i++)
            {
                switch (chArray[i])
                {
                    case ' ':
                        if (i != 0)
                        {
                            switch (chArray[i - 1])
                            {
                                case '%':
                                case '&':
                                    builder.Append(chArray[i]);
                                    break;
                            }
                        }
                        builder.Append(chArray[i]);
                        continue;

                    case '/':
                        {
                            builder.Append('W');
                            continue;
                        }
                    default:
                        {
                            builder.Append(chArray[i]);
                            continue;
                        }
                }
                builder.Append(chArray[i - 1]);
                builder.Append(chArray[i]);
            }
            return builder.ToString();
        }
        public void Dispose()
        {
#if !SILVERLIGHT && !WINRT && !WP
            m_graphics.Dispose();
#endif
            m_worksheet = null;
            m_rangeImpl = null;
            m_book = null;
        }
    }
}
