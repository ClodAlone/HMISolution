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
    /// RectElement class.
    /// </summary>
    public class RectElement : SuperElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public Length Width
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_WIDTH, new Length(0));
            }
            set
            {
                m_attributes[SVG.ATTR_WIDTH] = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public Length Height
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_HEIGHT, new Length(0));
            }
            set
            {
                m_attributes[SVG.ATTR_HEIGHT] = value;
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length X
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_X, new Length(0));
            }
            set
            {
                m_attributes[SVG.ATTR_X] = value;
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length Y
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_Y, new Length(0));
            }
            set
            {
                m_attributes[SVG.ATTR_Y] = value;
            }
        }

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
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RectElement"/> class.
        /// </summary>
        public RectElement()
        {
            m_name = SVG.NAME_RECT;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Rect from the rectangle.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>Rect element</returns>
        public static RectElement FromRectangleF(RectangleF rect)
        {
            RectElement res = new RectElement();

            res.X = new Length(rect.X);
            res.Y = new Length(rect.Y);
            res.Width = new Length(rect.Width);
            res.Height = new Length(rect.Height);

            return res;
        }

        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="node">The node.</param>
        internal override void ParseXml(System.Xml.XmlNode node)
        {
            XmlAttribute x = node.Attributes[SVG.ATTR_X];
            XmlAttribute y = node.Attributes[SVG.ATTR_Y];
            XmlAttribute rx = node.Attributes[SVG.ATTR_RX];
            XmlAttribute ry = node.Attributes[SVG.ATTR_RY];
            XmlAttribute width = node.Attributes[SVG.ATTR_WIDTH];
            XmlAttribute heigth = node.Attributes[SVG.ATTR_HEIGHT];

            if (x != null)
            {
                X = new Length(x.Value);
            }

            if (y != null)
            {
                Y = new Length(y.Value);
            }

            if (rx != null)
            {
                RX = new Length(rx.Value);
            }

            if (ry != null)
            {
                RY = new Length(ry.Value);
            }

            if (width != null)
            {
                Width = new Length(width.Value);
            }

            if (heigth != null)
            {
                Height = new Length(heigth.Value);
            }

            base.ParseXml(node);
        }

        /// <summary>
        /// Creates the <see cref="Syncfusion.SVG.IO.RectElement"/> from given coords.
        /// </summary>
        /// <param name="x">The x-location of the element.</param>
        /// <param name="y">The y-location of the element.</param>
        /// <param name="width">The width of the element.</param>
        /// <param name="height">The height of the element.</param>
        /// <returns>Rect element</returns>
        public static RectElement FromCoordinates(float x, float y, float width, float height)
        {
            RectElement res = new RectElement();

            res.X = new Length(x);
            res.Y = new Length(y);
            res.Width = new Length(width);
            res.Height = new Length(height);

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
            Pen pen = Utility.GetGDIPen(this);
            Brush br = Utility.GetGDIBrush(this);

            float x = X.GetValue(g.ClipBounds.Width);
            float y = Y.GetValue(g.ClipBounds.Height);
            float w = Width.GetValue(g.ClipBounds.Width);
            float h = Height.GetValue(g.ClipBounds.Height);

            if (br != null)
            {
                g.FillRectangle(br, x, y, w, h);
            }

            if (pen != null)
            {
                g.DrawRectangle(pen, x, y, w, h);
            }
        }
        #endregion
    }
}
