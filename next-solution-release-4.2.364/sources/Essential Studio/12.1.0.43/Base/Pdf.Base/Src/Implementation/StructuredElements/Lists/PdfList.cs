#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.Lists namespace contains classes for creating structure elements in PDF document.
/// </summary>
namespace Syncfusion.Pdf.Lists
{
    /// <summary>
    /// Represents base class for lists.
    /// </summary>
    /// <seealso cref="PdfLayoutElement"/> Class    
    public abstract class PdfList : PdfLayoutElement
    {
        #region Constants
        /// <summary>
        /// The characters for splitting.
        /// </summary>
        protected static readonly char[] c_splitChars = new char[] { '\n' };
        #endregion

        #region Static Methods
        /// <summary>
        /// Creates an item collection.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The item collection initialized properly.</returns>
        /// <exclude/>
        protected static PdfListItemCollection CreateItems(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            PdfListItemCollection lic = new PdfListItemCollection(text.Split(c_splitChars));

            return lic;
        }
        #endregion

        #region	Fields
        /// <summary>
        /// Holds collection of items.
        /// </summary>
        private PdfListItemCollection m_items;

        /// <summary>
        /// Tabulation for items.
        /// </summary>
        private float m_indent = 10;

        /// <summary>
        /// Indent between marker and text.
        /// </summary>
        private float m_textIndent = 5;

        /// <summary>
        /// List's font.
        /// </summary>
        private PdfFont m_font;

        /// <summary>
        /// The pen for the list.
        /// </summary>
        private PdfPen m_pen;

        /// <summary>
        /// The brush for the list.
        /// </summary>
        private PdfBrush m_brush;

        /// <summary>
        /// The string format for the list.
        /// </summary>
        private PdfStringFormat m_format;
        #endregion

        #region Properties
        /// <summary>
        /// Gets items of the list.
        /// </summary>
        public PdfListItemCollection Items
        {
            get
            {
                if (m_items == null)
                {
                    m_items = new PdfListItemCollection();
                }

                return m_items;
            }
        }

        /// <summary>
        /// Gets or sets tabulation for the list.
        /// </summary>
        public float Indent
        {
            get
            {
                return m_indent;
            }
            set
            {
                m_indent = value;
            }
        }

        /// <summary>
        /// Gets or sets the indent from the marker to the list item text.
        /// </summary>
        public float TextIndent
        {
            get
            {
                return m_textIndent;
            }
            set
            {
                m_textIndent = value;
            }
        }

        /// <summary>
        /// Gets or sets the list font.
        /// </summary>		
        public PdfFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("font");

                m_font = value;
            }
        }

        /// <summary>
        /// Gets or sets list brush.
        /// </summary>
        public PdfBrush Brush
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

        /// <summary>
        /// Gets or sets list pen.
        /// </summary>
        public PdfPen Pen
        {
            get
            {
                return m_pen;
            }
            set
            {
                m_pen = value;
            }
        }

        /// <summary>
        /// Gets or sets the format of the list.
        /// </summary>
        /// <value>The format.</value>
        public PdfStringFormat StringFormat
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

        /// <summary>
        /// Gets a value indicating whether to raise begin item layout event.
        /// </summary>
        internal bool RiseBeginItemLayout
        {
            get
            {
                return (BeginItemLayout != null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether to raise end item layout event.
        /// </summary>
        internal bool RiseEndItemLayout
        {
            get
            {
                return (EndItemLayout != null);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that rises when item begin layout.
        /// </summary>
        public event BeginItemLayoutEventHandler BeginItemLayout;

        /// <summary>
        /// Event that rises when item end layout.
        /// </summary>
        public event EndItemLayoutEventHandler EndItemLayout;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new list.
        /// </summary>
        internal PdfList()
        {
        }

        /// <summary>
        /// Creates new list with items.
        /// </summary>
        /// <param name="items">Collection of list items.</param>
        internal PdfList(PdfListItemCollection items)
        {
            if (items == null)
                throw new ArgumentException("Items collection can't be null", "items");

            m_items = items;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfList"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        internal PdfList(PdfFont font)
        {
            Font = font;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws an list on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the list should be printed.</param>
        /// <param name="x">X co-ordinate of the list.</param>
        /// <param name="y">Y co-ordinate of the list.</param>
        public override void Draw(PdfGraphics graphics, float x, float y)
        {
            PdfListLayouter layoutrer = new PdfListLayouter(this);

            layoutrer.Layout(graphics, x, y);
        }

        /// <summary>
        /// Draws list on the Graphics.
        /// </summary>
        /// <param name="graphics">Pdf graphics.</param>
        protected override void DrawInternal(PdfGraphics graphics)
        {
            PdfListLayouter layoutrer = new PdfListLayouter(this);

            layoutrer.Layout(graphics, PointF.Empty);
        }

        /// <summary>
        /// Layouts list at page.
        /// </summary>
        /// <param name="param">Pdf layout parameters.</param>
        /// <returns>Returns layout results.</returns>
        protected override PdfLayoutResult Layout(PdfLayoutParams param)
        {
            PdfListLayouter layoutrer = new PdfListLayouter(this);

            return layoutrer.Layout(param);
        }

        /// <summary>
        /// Rise the BeginItemLayout event.
        /// </summary>
        /// <param name="args">The instance containing the event data.</param>
        internal void OnBeginItemLayout(BeginItemLayoutEventArgs args)
        {
            if (RiseBeginItemLayout)
            {
                BeginItemLayout(this, args);
            }

        }

        /// <summary>
        /// Rise the EndItemLayout event.
        /// </summary>
        /// <param name="args">The instance containing the event data.</param>
        internal void OnEndItemLayout(EndItemLayoutEventArgs args)
        {
            if (RiseEndItemLayout)
            {
                EndItemLayout(this, args);
            }

        }
        #endregion
    }
}
