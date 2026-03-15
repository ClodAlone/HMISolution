#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

using Syncfusion.Pdf.Graphics;


namespace Syncfusion.Pdf.Grid
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PdfGridStyleBase : ICloneable
    {
        #region Fields
        private PdfBrush m_backgroundBrush;
        private PdfBrush m_textBrush;
        private PdfPen m_textPen;
        private PdfFont m_font;
        #endregion
        
        #region Properties
        /// <summary>
        /// Gets or sets the background brush.
        /// </summary>
        /// <value>The background brush.</value>
        public PdfBrush BackgroundBrush
        {
            get
            {
                return m_backgroundBrush;
            }
            set
            {
                m_backgroundBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the text brush.
        /// </summary>
        /// <value>The text brush.</value>
        public PdfBrush TextBrush
        {
            get
            {
                return m_textBrush;
            }
            set
            {
                m_textBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the text pen.
        /// </summary>
        /// <value>The text pen.</value>
        public PdfPen TextPen
        {
            get
            {
                return m_textPen;
            }
            set
            {
                m_textPen = value;
            }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public PdfFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class PdfGridStyle : PdfGridStyleBase
    {
        #region Fields
        private float m_cellSpacing;
        private PdfPaddings m_cellPadding;
        private PdfBorderOverlapStyle m_borderOverlapStyle;
        private bool m_bAllowHorizontalOverflow;
        private PdfHorizontalOverflowType m_HorizontalOverflowType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the cell spacing.
        /// </summary>
        /// <value>The cell spacing.</value>
        public float CellSpacing
        {
            get
            {
                return m_cellSpacing;
            }
            set
            {
                m_cellSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the cell padding.
        /// </summary>
        /// <value>The cell padding.</value>
        public PdfPaddings CellPadding
        {
            get
            {
                if (m_cellPadding == null)
                    m_cellPadding = new PdfPaddings();

                return m_cellPadding;
            }
            set
            {
                m_cellPadding = value;
            }
        }

        /// <summary>
        /// Gets or sets the border overlap style.
        /// </summary>
        /// <value>The border overlap style.</value>
        public PdfBorderOverlapStyle BorderOverlapStyle
        {
            get
            {
                return m_borderOverlapStyle;
            }
            set
            {
                m_borderOverlapStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow horizontal overflow.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow horizontal overflow]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowHorizontalOverflow
        {
            get
            {
                return m_bAllowHorizontalOverflow;
            }
            set
            {
                m_bAllowHorizontalOverflow = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the horizontal overflow.
        /// </summary>
        /// <value>The type of the horizontal overflow.</value>
        public PdfHorizontalOverflowType HorizontalOverflowType
        {
            get
            {
                return m_HorizontalOverflowType;
            }
            set
            {
                m_HorizontalOverflowType = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridStyle"/> class.
        /// </summary>
        public PdfGridStyle()
        {
            m_borderOverlapStyle = PdfBorderOverlapStyle.Overlap;
            m_bAllowHorizontalOverflow = false;
            m_HorizontalOverflowType = PdfHorizontalOverflowType.LastPage;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class PdfGridRowStyle : PdfGridStyleBase
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridRowStyle"/> class.
        /// </summary>
        public PdfGridRowStyle()
        {
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class PdfGridCellStyle : PdfGridRowStyle
    {
        #region Fields
        private PdfBorders m_borders = PdfBorders.Default;
        private PdfEdges m_edges;
        private PdfStringFormat m_format;
        private PdfImage m_backgroundImage;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the string format.
        /// </summary>
        /// <value>The string format.</value>
        public PdfStringFormat StringFormat
        {
            get
            {
                //if (m_format == null)
                //    m_format = GetDefaultFormat();

                return m_format;
            }
            set
            {
                m_format = value;
            }
        }

        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>The border.</value>
        public PdfBorders Borders
        {
            get
            {
                return m_borders;
            }
            set
            {
                m_borders = value;
            }
        }

        /// <summary>
        /// Gets or sets the background image.
        /// </summary>
        /// <value>The background image.</value>
        public PdfImage BackgroundImage
        {
            get
            {
                return m_backgroundImage;
            }
            set
            {
                m_backgroundImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the edges.
        /// </summary>
        /// <value>The edges.</value>
        internal PdfEdges Edges
        {
            get
            {
                if (m_edges == null)
                    m_edges = new PdfEdges();

                return m_edges;
            }
            set
            {
                m_edges = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridCellStyle"/> class.
        /// </summary>
        public PdfGridCellStyle()
        { }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the default format.
        /// </summary>
        /// <returns></returns>
        private PdfStringFormat GetDefaultFormat()
        {
            PdfStringFormat format = new PdfStringFormat();
            format.Alignment = PdfTextAlignment.Left;
            format.LineAlignment = PdfVerticalAlignment.Middle;
            return format;
        }
        #endregion
    }

    #region Enums
    /// <summary>
    /// 
    /// </summary>
    public enum PdfHorizontalOverflowType
    {
        /// <summary>
        /// 
        /// </summary>
        NextPage,
        /// <summary>
        /// 
        /// </summary>
        LastPage
    }
    #endregion
}
