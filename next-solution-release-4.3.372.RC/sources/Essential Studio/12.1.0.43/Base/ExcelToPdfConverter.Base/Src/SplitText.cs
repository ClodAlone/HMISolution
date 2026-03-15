#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Drawing;
using Syncfusion.XlsIO;
using Syncfusion.Pdf.Graphics;

namespace Syncfusion.ExcelToPdfConverter
{
    public class SplitText
    {
        private RectangleF m_originRect;
        private IWorksheet m_sheet;
        private string m_text;
        private PdfFont m_pdfFont;
        private PdfBrush m_brush;
        private PdfStringFormat m_format;
        private int m_row;
        private int m_adjacentColumn;

        internal RectangleF OriginRect
        {
            get
            {
                return m_originRect;
            }
            set
            {
                m_originRect = value;
            }
        }
        internal IWorksheet Sheet
        {
            get
            {
                return m_sheet;
            }
            set
            {
                m_sheet = value;
            }
        }
        internal string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }
        internal PdfFont TextFont
        {
            get
            {
                return m_pdfFont;
            }
            set
            {
                m_pdfFont = value;
            }
        }
        internal PdfBrush Brush
        {
            get
            {
                return m_brush;
            }
            set
            {
                m_brush = value;
            }
        }
        internal PdfStringFormat Format
        {
            get
            {
                return m_format;
            }
            set
            {
                m_format = value;
            }
        }
        internal int Row
        {
            get
            {
                return m_row;
            }
            set
            {
                m_row = value;
            }
        }
        internal int AdjacentColumn
        {
            get
            {
                return m_adjacentColumn;
            }
            set
            {
                m_adjacentColumn = value;
            }
        }

    }
}
