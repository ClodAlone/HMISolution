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
    /// Implements the "circle" element of SVG DOM.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class CircleElement : SuperElement
    {
        #region Proprties
        /// <summary>
        /// Gets or sets the radius of circle.
        /// </summary>
        /// <value>The radius of circle.</value>
        public Length R
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_R, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_R, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the X coorditane of element center.
        /// </summary>
        /// <value>The X coorditane of element center.</value>
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
        /// Gets or sets the Y coorditane of element center.
        /// </summary>
        /// <value>The Y coorditane of element center.</value>
        public Length CY
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_CY, new Length(0));
            }

            set
            {
                this.SetAttribute(SVG.ATTR_CY, value, Length.Empty);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CircleElement"/> class.
        /// </summary>
        public CircleElement()
            : base(SVG.NAME_CIRCLE)
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
        /// Draws the element to specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
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
