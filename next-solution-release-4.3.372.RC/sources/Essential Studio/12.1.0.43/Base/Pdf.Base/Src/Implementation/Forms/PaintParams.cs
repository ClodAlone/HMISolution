#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents class with field's paint parameters.
    /// </summary>
    internal class PaintParams
    {
        #region Fields
        /// <summary>
        /// Internal variable to store back brush.
        /// </summary>
        private PdfBrush m_backBrush = null;

        /// <summary>
        /// Internal variable to store fore brush.
        /// </summary>
        private PdfBrush m_foreBrush = null;

        /// <summary>
        /// Internal variable to store border width.
        /// </summary>
        private int m_borderWidth = 1;

        /// <summary>
        /// Internal variable to store border pen.
        /// </summary>
        private PdfPen m_borderPen = null;

        /// <summary>
        /// Internal variable to store border style.
        /// </summary>
        private PdfBorderStyle m_borderStyle = PdfBorderStyle.Solid;

        /// <summary>
        /// Internal variable to store bounds.
        /// </summary>
        private RectangleF m_bounds = RectangleF.Empty;

        /// <summary>
        /// Internal variable to store shadow brush.
        /// </summary>
        private PdfBrush m_shadowBrush = null;
        
        /// <summary>
        /// Rotation angle of the form fields.
        /// </summary>
        private int m_rotationAngle;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PaintParams"/> class.
        /// </summary>
        public PaintParams()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaintParams"/> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="backBrush">The back brush.</param>
        /// <param name="foreBrush">The fore brush.</param>
        /// <param name="borderPen">The border pen.</param>
        /// <param name="style">The style.</param>
        /// <param name="borderWidth">Width of the border.</param>
        /// <param name="shadowBrush">The shadow brush.</param>
        public PaintParams(RectangleF bounds, PdfBrush backBrush, PdfBrush foreBrush,
            PdfPen borderPen, PdfBorderStyle style, int borderWidth,
            PdfBrush shadowBrush, int rotationAngle)
        {
            m_bounds = bounds;
            m_backBrush = backBrush;
            m_foreBrush = foreBrush;
            m_borderPen = borderPen;
            m_borderStyle = style;
            m_borderWidth = borderWidth;
            m_shadowBrush = shadowBrush;
            m_rotationAngle = rotationAngle;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the back brush.
        /// </summary>
        /// <value>The back brush.</value>
        public PdfBrush BackBrush
        {
            get
            {
                return m_backBrush;
            }

            set
            {
                m_backBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the fore brush.
        /// </summary>
        /// <value>The fore brush.</value>
        public PdfBrush ForeBrush
        {
            get
            {
                return m_foreBrush;
            }

            set
            {
                m_foreBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the border pen.
        /// </summary>
        /// <value>The border pen.</value>
        public PdfPen BorderPen
        {
            get
            {
                return m_borderPen;
            }

            set
            {
                m_borderPen = value;
            }
        }

        /// <summary>
        /// Gets or sets the border style.
        /// </summary>
        /// <value>The border style.</value>
        public PdfBorderStyle BorderStyle
        {
            get
            {
                return m_borderStyle;
            }

            set
            {
                m_borderStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>The width of the border.</value>
        public int BorderWidth
        {
            get
            {
                return m_borderWidth;
            }

            set
            {
                m_borderWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }

            set
            {
                m_bounds = value;
            }
        }

        /// <summary>
        /// Gets or sets the shadow brush.
        /// </summary>
        /// <value>The shadow brush.</value>
        public PdfBrush ShadowBrush
        {
            get
            {
                return m_shadowBrush;
            }

            set
            {
                m_shadowBrush = value;
            }
        }

        /// <summary>
        /// Gets or Set the rotation angle.
        /// </summary>
        public int RotationAngle
        {
            get
            {
                return m_rotationAngle;
            }
            set
            {
                m_rotationAngle = value;
            }
        }
        #endregion
    }
}
