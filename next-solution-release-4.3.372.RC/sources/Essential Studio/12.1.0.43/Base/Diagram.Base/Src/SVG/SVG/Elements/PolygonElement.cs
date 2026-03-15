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
    /// Class for Polygon.
    /// </summary>
    public class PolygonElement : SuperElement
    {
        #region Proprties
        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>The points.</value>
        public PointsArray Points
        {
            get
            {
                return (PointsArray)GetAttribute(SVG.ATTR_POINTS, null);
            }
            set
            {
                SetAttribute(SVG.ATTR_POINTS, value, null);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonElement"/> class.
        /// </summary>
        public PolygonElement()
        {
            m_name = SVG.NAME_POLYGON;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="node">The node.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute attrPoints = node.Attributes[SVG.ATTR_POINTS];

            if (attrPoints != null)
            {
                Points = new PointsArray(attrPoints.Value);
            }

            base.ParseXml(node);
        }

        /// <summary>
        /// Froms the points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>Polygon element</returns>
        public static PolygonElement FromPoints(PointF[] points)
        {
            PolygonElement res = new PolygonElement();

            res.Points = new PointsArray(points);

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
            Pen pn = Utility.GetGDIPen(this as IStrokeAttributes);
            Brush br = Utility.GetGDIBrush(this);

            if (br != null)
            {
                g.FillPolygon(br, Points.Points);
            }

            if (pn != null)
            {
                g.DrawPolygon(pn, Points.Points);
            }

            base.DrawSelf(g);
        }
        #endregion
    }
}
