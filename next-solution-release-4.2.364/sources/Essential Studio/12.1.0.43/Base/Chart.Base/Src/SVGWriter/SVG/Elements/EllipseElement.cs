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

using System;
using System.Drawing;
using System.Xml;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Implements the "ellipse" element of SVG DOM.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class EllipseElement : SuperElement
    {
        #region Proprties
        /// <summary>
        /// Gets or sets the horizontal radius.
        /// </summary>
        /// <value>The horizontal radius.</value>
        public Length RX
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_RX, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_RX, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the vertical radius.
        /// </summary>
        /// <value>The vertical radius.</value>
        public Length RY
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_RY, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_RY, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the element center.
        /// </summary>
        /// <value>The X coordinate of the element center.</value>
        public Length CX
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_CX, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_CX, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the element center.
        /// </summary>
        /// <value>The Y coordinate of the element center.</value>
        public Length CY
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_CY, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_CY, value, Length.Empty);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EllipseElement"/> class.
        /// </summary>
        public EllipseElement()
            : base(SVG.NAME_ELLIPSE)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates the new <see cref="EllipseElement"/> instance by the specified <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>Returns EllipseElement.</returns>
        public static EllipseElement FromRectangleF(RectangleF rect)
        {
            EllipseElement res = new EllipseElement();

            res.CX = new Length(rect.X + rect.Width / 2);
            res.CY = new Length(rect.Y + rect.Height / 2);
            res.RX = new Length(rect.Width / 2);
            res.RY = new Length(rect.Height / 2);

            return res;
        }

        /// <summary>
        /// Parses the XML document.
        /// </summary>
        /// <param name="node">The <see cref="XmlNode"/>.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute cx = node.Attributes[SVG.ATTR_CX];
            XmlAttribute cy = node.Attributes[SVG.ATTR_CY];
            XmlAttribute rx = node.Attributes[SVG.ATTR_RX];
            XmlAttribute ry = node.Attributes[SVG.ATTR_RY];

            if (cx != null)
            {
                CX = new Length(cx.Value);
            }

            if (cy != null)
            {
                CY = new Length(cy.Value);
            }

            if (rx != null)
            {
                RX = new Length(rx.Value);
            }

            if (ry != null)
            {
                RY = new Length(ry.Value);
            }

            base.ParseXml(node);
        }

        /// <summary>
        /// Creates instance of the <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.EllipseElement"/> by its location and size.
        /// </summary>
        /// <param name="x">The x-location of the element.</param>
        /// <param name="y">The y-location of the element.</param>
        /// <param name="width">The width of the element.</param>
        /// <param name="height">The height of the element.</param>
        /// <returns>Calculated <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.EllipseElement"/>.</returns>
        public static EllipseElement FromCoordinates(float x, float y, float width, float height)
        {
            EllipseElement res = new EllipseElement();

            res.CX = new Length(x + width / 2);
            res.CY = new Length(y + height / 2);
            res.RX = new Length(width / 2);
            res.RY = new Length(height / 2);

            return res;
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Draws the element to specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        protected override void DrawSelf(Graphics g)
        {
            RectangleF rc = new RectangleF(
                CX.Value - RX.Value, CY.Value - RY.Value, 2 * RX.Value, 2 * RY.Value);
            Pen pn = Utility.GetGDIPen(this);
            Brush br = Utility.GetGDIBrush(this);

            if (br != null)
            {
                g.FillEllipse(br, rc);
            }

            if (pn != null)
            {
                g.DrawEllipse(pn, rc);
            }
        }
        #endregion
    }
}
