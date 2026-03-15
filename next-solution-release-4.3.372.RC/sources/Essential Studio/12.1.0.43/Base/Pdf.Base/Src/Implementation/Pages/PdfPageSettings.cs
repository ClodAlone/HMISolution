#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Text;
using Syncfusion.Pdf.Graphics;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represent class with setting of page.
    /// </summary>
    public class PdfPageSettings : ICloneable
    {
        #region Fields
        private PdfPageOrientation m_orientation = PdfPageOrientation.Portrait;
        private SizeF m_size = PdfPageSize.A4;
        private PdfMargins m_margins = new PdfMargins();
        private PdfPageRotateAngle m_rotateAngle = PdfPageRotateAngle.RotateAngle0;
        private PdfGraphicsUnit m_logicalUnit = PdfGraphicsUnit.Point;
        private PointF m_origin = PointF.Empty;
        /// <summary>
        /// Internal variable to store transition.
        /// </summary>
        private PdfPageTransition m_transition = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the page orientation.
        /// </summary>
        public PdfPageOrientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                if (m_orientation != value)
                {
                    m_orientation = value;
                    UpdateSize(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        public SizeF Size
        {
            get
            {
                return m_size;
            }
            set
            {
                SetSize(value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the page.
        /// </summary>
        public float Width
        {
            get
            {
                return m_size.Width;
            }
            set
            {
                m_size.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the page.
        /// </summary>
        public float Height
        {
            get
            {
                return m_size.Height;
            }
            set
            {
                m_size.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the margins of the page.
        /// </summary>
        public PdfMargins Margins
        {
            get
            {
                return m_margins;
            }
            set
            {
                m_margins = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of degrees by which the page should be rotated clockwise when displayed or printed.
        /// </summary>
        public PdfPageRotateAngle Rotate
        {
            get
            {
                return m_rotateAngle;
            }
            set
            {
                m_rotateAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the transition.
        /// </summary>
        /// <value>The transition.</value>
        public PdfPageTransition Transition
        {
            get
            {
                if (m_transition == null)
                {
                    m_transition = new PdfPageTransition();
                }

                return m_transition;
            }
            set
            {
                m_transition = value;
            }
        }
        /// <summary>
        /// Gets or sets the type of default user space units.
        /// </summary>
        /// <remarks>For PDF 1.6 and later versions.</remarks>
        internal PdfGraphicsUnit Unit
        {
            get
            {
                return m_logicalUnit;
            }
            set
            {
                m_logicalUnit = value;
            }
        }

        /// <summary>
        /// Gets or sets the origin of the page
        /// </summary>
        internal PointF Origin
        {
            get
            {
                return m_origin;
            }
            set
            {
                m_origin = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageSettings"/> class.
        /// </summary>
        public PdfPageSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageSettings"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public PdfPageSettings(SizeF size)
        {
            m_size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageSettings"/> class.
        /// </summary>
        /// <param name="pageOrientation">The page orientation.</param>
        public PdfPageSettings(PdfPageOrientation pageOrientation)
        {
            m_orientation = pageOrientation;
            UpdateSize(pageOrientation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageSettings"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="pageOrientation">The page orientation.</param>
        public PdfPageSettings(SizeF size, PdfPageOrientation pageOrientation)
        {
            m_size = size;
            m_orientation = pageOrientation;

            UpdateSize(pageOrientation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageSettings"/> class.
        /// </summary>
        /// <param name="margins">The margins.</param>
        public PdfPageSettings(float margins)
        {
            m_margins.SetMargins(margins);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageSettings"/> class.
        /// </summary>
        /// <param name="leftMargin">The left margin.</param>
        /// <param name="topMargin">The top margin.</param>
        /// <param name="rightMargin">The right margin.</param>
        /// <param name="bottomMargin">The bottom margin.</param>
        public PdfPageSettings(float leftMargin, float topMargin, float rightMargin,
            float bottomMargin)
        {
            m_margins.SetMargins(leftMargin, topMargin, rightMargin, bottomMargin);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageSettings"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="margins">The margins.</param>
        public PdfPageSettings(SizeF size, float margins)
        {
            m_size = size;
            m_margins.SetMargins(margins);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageSettings"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="leftMargin">The left margin.</param>
        /// <param name="topMargin">The top margin.</param>
        /// <param name="rightMargin">The right margin.</param>
        /// <param name="bottomMargin">The bottom margin.</param>
        public PdfPageSettings(SizeF size, float leftMargin, float topMargin,
            float rightMargin, float bottomMargin)
        {
            m_size = size;
            m_margins.SetMargins(leftMargin, topMargin, rightMargin, bottomMargin);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageSettings"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="pageOrientation">The page orientation.</param>
        /// <param name="margins">The margins.</param>
        public PdfPageSettings(SizeF size, PdfPageOrientation pageOrientation, float margins)
        {
            m_size = size;
            m_orientation = pageOrientation;

            m_margins.SetMargins(margins);
            UpdateSize(pageOrientation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageSettings"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="pageOrientation">The page orientation.</param>
        /// <param name="leftMargin">The left margin.</param>
        /// <param name="topMargin">The top margin.</param>
        /// <param name="rightMargin">The right margin.</param>
        /// <param name="bottomMargin">The bottom margin.</param>
        public PdfPageSettings(SizeF size, PdfPageOrientation pageOrientation,
            float leftMargin, float topMargin, float rightMargin, float bottomMargin)
        {
            m_size = size;
            m_orientation = pageOrientation;

            m_margins.SetMargins(leftMargin, topMargin, rightMargin, bottomMargin);
            UpdateSize(pageOrientation);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the margins.
        /// </summary>
        /// <param name="margins">The margins.</param>
        public void SetMargins(float margins)
        {
            m_margins.SetMargins(margins);
        }

        /// <summary>
        /// Sets the margins.
        /// </summary>
        /// <param name="leftRight">The left right.</param>
        /// <param name="topBottom">The top bottom.</param>
        public void SetMargins(float leftRight, float topBottom)
        {
            m_margins.SetMargins(leftRight, topBottom);
        }

        /// <summary>
        /// Sets the margins.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        public void SetMargins(float left, float top, float right, float bottom)
        {
            m_margins.SetMargins(left, top, right, bottom);
        }
        #endregion

        #region ICloneable implementation
        /// <summary>
        /// Creates a clone of the object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public object Clone()
        {
            PdfPageSettings settings = (PdfPageSettings)base.MemberwiseClone();
            settings.m_margins = (PdfMargins)Margins.Clone();

            if (GetTransition() != null)
            {
                settings.Transition = (PdfPageTransition)Transition.Clone();
            }

            return settings;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Returns size, shrinked by the margins.
        /// </summary>
        /// <returns>Returns size, shrinked by the margins.</returns>
        internal SizeF GetActualSize()
        {
            float width = Width - (Margins.Left + Margins.Right);
            float height = Height - (Margins.Top + Margins.Bottom);
            SizeF size = new SizeF(width, height);

            return size;
        }

        /// <summary>
        /// Gets the transition.
        /// </summary>
        /// <returns></returns>
        internal PdfPageTransition GetTransition()
        {
            return m_transition;
        }

        /// <summary>
        /// Update page size depending on orientation.
        /// </summary>
        /// <param name="orientation">Page orientation settings.</param>
        private void UpdateSize(PdfPageOrientation orientation)
        {
            float min = Math.Min(Width, Height);
            float max = Math.Max(Width, Height);

            switch (orientation)
            {
                case PdfPageOrientation.Portrait:
                    Size = new SizeF(min, max);
                    break;

                case PdfPageOrientation.Landscape:
                    Size = new SizeF(max, min);
                    break;
            }
        }

        /// <summary>
        /// Sets size to the page aaccording to the orientation.
        /// </summary>
        /// <param name="size">Size of the page.</param>
        private void SetSize(SizeF size)
        {
            float min = (float)Math.Min(size.Width, size.Height);
            float max = (float)Math.Max(size.Width, size.Height);

            if (Orientation == PdfPageOrientation.Portrait)
            {
                m_size = new SizeF(min, max);
            }
            else
            {
                m_size = new SizeF(max, min);
            }
        }
        #endregion
    }
}
