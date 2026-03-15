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
    /// LineElement class.
    /// </summary>
    public class LineElement : SuperElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length X1
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_X1, new Length("0%"));
            }
            set
            {
                SetAttribute(SVG.ATTR_X1, value, new Length("0%"));
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length Y1
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_Y1, new Length("0%"));
            }
            set
            {
                SetAttribute(SVG.ATTR_Y1, value, new Length("0%"));
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length X2
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_X2, new Length("100%"));
            }
            set
            {
                SetAttribute(SVG.ATTR_X2, value, new Length("100%"));
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length Y2
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_Y2, new Length("0%"));
            }
            set
            {
                SetAttribute(SVG.ATTR_Y2, value, new Length("0%"));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LineElement"/> class.
        /// </summary>
        public LineElement()
        {
            m_name = SVG.NAME_LINE;
        }
        #endregion

        #region Public methdos
        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="node">The node.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute attrX1 = node.Attributes[SVG.ATTR_X1];
            XmlAttribute attrY1 = node.Attributes[SVG.ATTR_Y1];
            XmlAttribute attrX2 = node.Attributes[SVG.ATTR_X2];
            XmlAttribute attrY2 = node.Attributes[SVG.ATTR_Y2];

            if (attrX1 != null)
            {
                X1 = new Length(attrX1.Value);
            }

            if (attrY1 != null)
            {
                Y1 = new Length(attrY1.Value);
            }

            if (attrX2 != null)
            {
                X2 = new Length(attrX2.Value);
            }

            if (attrY2 != null)
            {
                Y2 = new Length(attrY2.Value);
            }

            base.ParseXml(node);
        }

        /// <summary>
        /// Creates the <see cref="Syncfusion.SVG.IO.LineElement"/> from its coords.
        /// </summary>
        /// <param name="pt1">The first point.</param>
        /// <param name="pt2">The second point.</param>
        /// <returns>
        /// Created <see cref="Syncfusion.SVG.IO.LineElement"/>.
        /// </returns>
        public static LineElement FromPoints(PointF pt1, PointF pt2)
        {
            LineElement res = new LineElement();

            res.X1 = new Length(pt1.X);
            res.Y1 = new Length(pt1.Y);
            res.X2 = new Length(pt2.X);
            res.Y2 = new Length(pt2.Y);

            return res;
        }

        /// <summary>
        /// Creates the <see cref="Syncfusion.SVG.IO.LineElement"/> from its coords.
        /// </summary>
        /// <param name="x1">The x-location of the first point.</param>
        /// <param name="y1">The y-location of the first point.</param>
        /// <param name="x2">The x-location of the second point.</param>
        /// <param name="y2">The y-location of the second point.</param>
        /// <returns>Created <see cref="Syncfusion.SVG.IO.LineElement"/>.</returns>
        public static LineElement FromCoordinates(float x1, float y1, float x2, float y2)
        {
            LineElement res = new LineElement();

            res.X1 = new Length(x1);
            res.Y1 = new Length(y1);
            res.X2 = new Length(x2);
            res.Y2 = new Length(y2);

            return res;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Draws self.
        /// </summary>
        /// <param name="g">The graphics.</param>
        protected override void DrawSelf(Graphics g)
        {
            Pen pn = Utility.GetGDIPen((this as IStrokeAttributes));
            PointF pt1 = new PointF(X1.Value, Y1.Value);
            PointF pt2 = new PointF(X2.Value, Y2.Value);

            if (pn != null)
            {
                g.DrawLine(pn, pt1, pt2);
            }
        }
        #endregion
    }
}
