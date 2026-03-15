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
    /// Implemenets the "polyline" element of SVG DOM.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class PolylineElement : SuperElement
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
        /// Initializes a new instance of the <see cref="PolylineElement"/> class.
        /// </summary>
        public PolylineElement()
            : base(SVG.NAME_POLYLINE)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Parses the XML document.
        /// </summary>
        /// <param name="node">The <see cref="XmlNode"/>.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute attrPoints = node.Attributes[SVG.ATTR_POINTS];

            if (attrPoints != null)
            {
                Points = new PointsArray(attrPoints.Value);
            }

            base.ParseXml(node);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Draws the element to specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        protected override void DrawSelf(Graphics g)
        {
            Pen pn = Utility.GetGDIPen(this as IStrokeAttributes);

            if (pn != null)
            {
                g.DrawLines(pn, Points.Points);
            }

            base.DrawSelf(g);
        }
        #endregion
    }
}
