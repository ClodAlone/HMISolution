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
using System.Drawing.Drawing2D;
using Syncfusion.Documentation;
using System.Xml;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Impements the "path" element of SVG DOM.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class PathElement : SuperElement
    {
        #region Proprties
        /// <summary>
        /// Gets or sets the path data.
        /// </summary>
        /// <value>The path data.</value>
        public Data D
        {
            get
            {
                return (Data)GetAttribute(SVG.ATTR_DATA, null);
            }

            set
            {
                SetAttribute(SVG.ATTR_DATA, value, null);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PathElement"/> class.
        /// </summary>
        public PathElement()
            : base(SVG.NAME_PATH)
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
            XmlAttribute d = node.Attributes[SVG.ATTR_DATA];

            if (d != null)
            {
                D = new Data(d.Value);
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
            if (D != null)
            {
                Pen pen = Utility.GetGDIPen(this);
                Brush br = Utility.GetGDIBrush(this);
                PointF[] pts = D.PathData.Points;
                byte[] tps = D.PathData.Types;
                GraphicsPath gp = new GraphicsPath(pts, tps);

                if (br != null)
                {
                    g.FillPath(br, gp);
                }

                if (pen != null)
                {
                    g.DrawPath(pen, gp);
                }
            }
        }
        #endregion
    }
}
