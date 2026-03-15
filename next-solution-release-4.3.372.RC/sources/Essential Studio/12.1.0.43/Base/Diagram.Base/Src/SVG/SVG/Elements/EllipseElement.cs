#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Xml;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// EllipseElement class.
    /// </summary>
    public class EllipseElement : SuperElement
    {
        #region Proprties
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length RX
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_RX, new Length(0));
            }
            set
            {
                m_attributes[SVG.ATTR_RX] = value;
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length RY
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_RY, new Length(0));
            }
            set
            {
                m_attributes[SVG.ATTR_RY] = value;
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length CX
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_CX, new Length(0));
            }
            set
            {
                m_attributes[SVG.ATTR_CX] = value;
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length CY
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_CY, new Length(0));
            }
            set
            {
                m_attributes[SVG.ATTR_CY] = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EllipseElement"/> class.
        /// </summary>
        public EllipseElement()
        {
            m_name = SVG.NAME_ELLIPSE;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Ellipse from the rectangle.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>Ellipse element</returns>
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
        /// Parses the XML.
        /// </summary>
        /// <param name="node">The node.</param>
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
        /// Froms the coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Ellipse element</returns>
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
        /// Draws self.
        /// </summary>
        /// <param name="g">The graphics.</param>
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
