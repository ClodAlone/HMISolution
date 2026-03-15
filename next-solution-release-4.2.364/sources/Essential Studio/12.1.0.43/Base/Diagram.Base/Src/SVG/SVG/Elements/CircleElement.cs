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
    /// CircleElement class.
    /// </summary>
    public class CircleElement : SuperElement
    {
        #region Proprties
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length R
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_R, new Length(0));
            }
            set
            {
                SetAttribute(SVG.ATTR_R, value, new Length(0));
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
        /// Initializes a new instance of the <see cref="CircleElement"/> class.
        /// </summary>
        public CircleElement()
        {
            m_name = SVG.NAME_CIRCLE;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="node">The node.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute cx = node.Attributes[SVG.ATTR_CX];
            XmlAttribute cy = node.Attributes[SVG.ATTR_CY];
            XmlAttribute r = node.Attributes[SVG.ATTR_R];

            if (cx != null)
            {
                CX = new Length(cx.Value);
            }

            if (cy != null)
            {
                CY = new Length(cy.Value);
            }

            if (r != null)
            {
                R = new Length(r.Value);
            }

            base.ParseXml(node);
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Draws self.
        /// </summary>
        /// <param name="g">The graphics.</param>
        protected override void DrawSelf(Graphics g)
        {
            RectangleF rc = new RectangleF(CX.Value - R.Value, CY.Value - R.Value, 2 * R.Value, 2 * R.Value);
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
