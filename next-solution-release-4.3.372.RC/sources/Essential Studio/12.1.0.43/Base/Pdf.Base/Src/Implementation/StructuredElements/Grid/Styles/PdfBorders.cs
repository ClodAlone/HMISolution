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
using Syncfusion.Pdf.Interactive;
using System.Drawing;

namespace Syncfusion.Pdf
{
    public class PdfBorders
    {
        #region Fields
        private PdfPen m_left;
        private PdfPen m_right;
        private PdfPen m_top;
        private PdfPen m_bottom;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public PdfPen Left
        {
            get
            {
                return m_left;
            }
            set
            {
                m_left = value;
            }
        }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        public PdfPen Right
        {
            get
            {
                return m_right;
            }
            set
            {
                m_right = value;
            }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public PdfPen Top
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = value;
            }
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public PdfPen Bottom
        {
            get
            {
                return m_bottom;
            }
            set
            {
                m_bottom = value;
            }
        }

        /// <summary>
        /// Sets all.
        /// </summary>
        /// <value>All.</value>
        public PdfPen All
        {
            set
            {
                m_left = m_right = m_top = m_bottom = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is all.
        /// </summary>
        /// <value><c>true</c> if this instance is all; otherwise, <c>false</c>.</value>
        internal bool IsAll
        {
            get
            {
                return ((m_left == m_right) && (m_left == m_top) && (m_left == m_bottom));
            }
        }

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>The default.</value>
        public static PdfBorders Default
        {
            get
            {
                return new PdfBorders();
            }
        }
        #endregion

        #region Constructor
        public PdfBorders()
        {
            PdfPen defaultBorderPenLeft = new PdfPen(new PdfColor(0, 0, 0));
            defaultBorderPenLeft.DashStyle = PdfDashStyle.Solid;
            PdfPen defaultBorderPenRight = new PdfPen(new PdfColor(0, 0, 0));
            defaultBorderPenRight.DashStyle = PdfDashStyle.Solid;
            PdfPen defaultBorderPenTop = new PdfPen(new PdfColor(0, 0, 0));
            defaultBorderPenTop.DashStyle = PdfDashStyle.Solid;
            PdfPen defaultBorderPenBottom = new PdfPen(new PdfColor(0, 0, 0));
            defaultBorderPenBottom.DashStyle = PdfDashStyle.Solid;

            m_left = defaultBorderPenLeft;
            m_right = defaultBorderPenRight;
            m_top = defaultBorderPenTop;
            m_bottom = defaultBorderPenBottom;
        }
        #endregion
    }

    public class PdfEdges
    {
        #region Fields
        private int m_left;
        private int m_right;
        private int m_top;
        private int m_bottom;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public int Left
        {
            get
            {
                return m_left;
            }
            set
            {
                m_left = value;
            }
        }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        public int Right
        {
            get
            {
                return m_right;
            }
            set
            {
                m_right = value;
            }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public int Top
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = value;
            }
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public int Bottom
        {
            get
            {
                return m_bottom;
            }
            set
            {
                m_bottom = value;
            }
        }

        /// <summary>
        /// Sets all.
        /// </summary>
        /// <value>All.</value>
        public int All
        {
            set
            {
                m_left = m_right = m_top = m_bottom = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is all.
        /// </summary>
        /// <value><c>true</c> if this instance is all; otherwise, <c>false</c>.</value>
        internal bool IsAll
        {
            get
            {
                return ((m_left == m_right) && (m_left == m_top) && (m_left == m_bottom));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEdges"/> class.
        /// </summary>
        public PdfEdges()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEdges"/> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="top">The top.</param>
        /// <param name="bottom">The bottom.</param>
        public PdfEdges(int left, int right, int top, int bottom)
        {
            m_left = left;
            m_right = right;
            m_top = top;
            m_bottom = bottom;
        }
        #endregion
    }

    public class PdfPaddings
    {
        #region Fields
        private float m_left;
        private float m_right;
        private float m_top;
        private float m_bottom;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public float Left
        {
            get
            {
                return m_left;
            }
            set
            {
                m_left = value;
            }
        }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        public float Right
        {
            get
            {
                return m_right;
            }
            set
            {
                m_right = value;
            }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public float Top
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = value;
            }
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public float Bottom
        {
            get
            {
                return m_bottom;
            }
            set
            {
                m_bottom = value;
            }
        }

        /// <summary>
        /// Sets all.
        /// </summary>
        /// <value>All.</value>
        public float All
        {
            set
            {
                m_left = m_right = m_top = m_bottom = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPaddings"/> class.
        /// </summary>
        public PdfPaddings()
        {
            m_left = m_right = m_top = m_bottom = .5f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPaddings"/> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="top">The top.</param>
        /// <param name="bottom">The bottom.</param>
        public PdfPaddings(float left, float right, float top, float bottom)
        {
            m_left = left;
            m_right = right;
            m_top = top;
            m_bottom = bottom;
        }
        #endregion
    }
}
