#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// A class representing page margins.
    /// </summary>
    public class PdfMargins : ICloneable
    {
        #region Constants
        /// <summary>
        /// Represents the Default Page Margin value.
        /// </summary>
        private const float PageMargin = 0f;
        #endregion

        #region Fields
        private float m_left;
        private float m_top;
        private float m_right;
        private float m_bottom;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the left margin size.
        /// </summary>
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
        /// Gets or sets the top margin size.
        /// </summary>
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
        /// Gets or sets the right margin size.
        /// </summary>
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
        /// Gets or sets the bottom margin size.
        /// </summary>
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
        /// Sets margin of each side.
        /// </summary>
        /// <value>Margin of each side.</value>
        public float All
        {
            set
            {
                SetMargins(value);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfMargins"/> class.
        /// </summary>
        public PdfMargins()
        {
            SetMargins(PageMargin);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the margins.
        /// </summary>
        /// <param name="margin">The margin size.</param>
        internal void SetMargins(float margin)
        {
            m_left = m_top = m_right = m_bottom = margin;
        }

        /// <summary>
        /// Sets the margins.
        /// </summary>
        /// <param name="leftRight">The left right.</param>
        /// <param name="topBottom">The top bottom.</param>
        internal void SetMargins(float leftRight, float topBottom)
        {
            m_left = m_right = leftRight;
            m_top = m_bottom = topBottom;
        }

        /// <summary>
        /// Sets the margins.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        internal void SetMargins(float left, float top, float right, float bottom)
        {
            m_left = left;
            m_top = top;
            m_right = right;
            m_bottom = bottom;
        }
        #endregion

        #region ICloneable implementation
        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            PdfMargins margins = (PdfMargins)base.MemberwiseClone();
            return margins;
        }
        #endregion
    }
}
