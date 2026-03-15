#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region File using directives
using System;
using System.IO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    internal class TextBoxProps : BaseProps
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private float m_txbxLineWidth;
        private LineDashing m_lineDashing;
        private Color m_fillColor;
        private TextBoxLineStyle m_lineStyle;
        private WrapMode m_wrapMode;
        private float m_txID;
        private bool m_noLine;
        private Color m_lineColor;

        private uint m_leftMargin;
        private uint m_rightMargin;
        private uint m_topMargin;
        private uint m_bottomMargin;


        #endregion

        #region Class properties
        /// <summary>
        /// Get/set value which defines if
        /// there is a line around textbox shape
        /// </summary>
        internal bool NoLine
        {
            get
            {
                return m_noLine;
            }
            set
            {
                m_noLine = value;
            }
        }
        /// <summary>
        /// Get/set wrap text mode
        /// </summary>
        internal WrapMode WrapText
        {
            get
            {
                return m_wrapMode;
            }
            set
            {
                m_wrapMode = value;
            }
        }
        /// <summary>
        /// Get/set line style for textbox
        /// </summary>
        internal TextBoxLineStyle LineStyle
        {
            get
            {
                return m_lineStyle;
            }
            set
            {
                m_lineStyle = value;
            }
        }
        /// <summary>
        /// Get/set textbox fill color
        /// </summary>
        internal Color FillColor
        {
            get
            {
                return m_fillColor;
            }
            set
            {
                m_fillColor = value;
            }
        }
        /// <summary>
        /// Get/set line color.
        /// </summary>
        internal Color LineColor
        {
            get
            {
                return m_lineColor;
            }
            set
            {
                m_lineColor = value;
            }
        }

        /// <summary>
        /// Get/set textbox line width
        /// </summary>
        internal float TxbxLineWidth
        {
            get
            {
                return m_txbxLineWidth;
            }
            set
            {
                m_txbxLineWidth = value;
            }
        }
        /// <summary>
        /// Get/set line dashing for textbox
        /// </summary>
        internal LineDashing LineDashing
        {
            get
            {
                return m_lineDashing;
            }
            set
            {
                m_lineDashing = value;
            }
        }
        /// <summary>
        /// Get/set TXID value;
        /// </summary>
        internal float TXID
        {
            get
            {
                return m_txID;
            }
            set
            {
                m_txID = value;
            }
        }

        /// <summary>
        /// Gets or sets the left margin.
        /// </summary>
        /// <value>The left margin.</value>
        internal uint LeftMargin
        {
            get
            {
                return m_leftMargin;
            }
            set
            {
                m_leftMargin = value;
            }
        }
        /// <summary>
        /// Gets or sets the right margin.
        /// </summary>
        /// <value>The right margin.</value>
        internal uint RightMargin
        {
            get
            {
                return m_rightMargin;
            }
            set
            {
                m_rightMargin = value;
            }
        }
        /// <summary>
        /// Gets or sets the top margin.
        /// </summary>
        /// <value>The top margin.</value>
        internal uint TopMargin
        {
            get
            {
                return m_topMargin;
            }
            set
            {
                m_topMargin = value;
            }
        }
        /// <summary>
        /// Gets or sets the bottom margin.
        /// </summary>
        /// <value>The bottom margin.</value>
        internal uint BottomMargin
        {
            get
            {
                return m_bottomMargin;
            }
            set
            {
                m_bottomMargin = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor
        /// </summary>
        internal TextBoxProps()
        {
            m_fillColor = Color.White;
            m_lineColor = Color.Black;
            m_txbxLineWidth = 0.75f;
            base.RelHrzPos = HorizontalOrigin.Column;
            base.RelVrtPos = VerticalOrigin.Paragraph;
            m_lineStyle = TextBoxLineStyle.Simple;
            m_lineDashing = LineDashing.Solid;
            m_wrapMode = WrapMode.None;
            m_leftMargin = uint.MaxValue;
            m_rightMargin = uint.MaxValue;
            m_topMargin = uint.MaxValue;
            m_bottomMargin = uint.MaxValue;
        }
        #endregion
    }
}
