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
    /// Implemenets the "stop" element of SVG DOM.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class StopElement : SuperElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public Length Offset
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_OFFSET, new Length(0));
            }

            set
            {
                SetAttribute(SVG.ATTR_OFFSET, value, new Length(0));
            }
        }

        /// <summary>
        /// Gets or sets the color of the stop.
        /// </summary>
        /// <value>The color of the stop.</value>
        public NoneColor StopColor
        {
            get
            {
                return (NoneColor)GetAttribute(SVG.ATTR_STOP_COLOR, new NoneColor(Color.Black));
            }

            set
            {
                SetAttribute(SVG.ATTR_STOP_COLOR, value, new NoneColor(Color.Black));
            }
        }

        /// <summary>
        /// Gets or sets the stop opacity.
        /// </summary>
        /// <value>The stop opacity.</value>
        public Opacity StopOpacity
        {
            get
            {
                return (Opacity)GetAttribute(SVG.ATTR_STOP_OPACITY, new Opacity(1f));
            }

            set
            {
                SetAttribute(SVG.ATTR_STOP_OPACITY, value, new Opacity(1f));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="StopElement"/> class.
        /// </summary>
        public StopElement()
            : base(SVG.NAME_STOP)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates the stop element by the specified color and offset.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns StopElement.</returns>
        public static StopElement FromColor(Color color, Length offset)
        {
            StopElement res = new StopElement();

            res.StopColor = new NoneColor(color);
            res.StopOpacity = color.A;
            res.Offset = offset;

            return res;
        }

        /// <summary>
        /// Parses the XML document.
        /// </summary>
        /// <param name="node">The <see cref="XmlNode"/>.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute attrOffset = node.Attributes[SVG.ATTR_OFFSET];
            XmlAttribute attrStopColor = node.Attributes[SVG.ATTR_STOP_COLOR];
            XmlAttribute attrStopOpacity = node.Attributes[SVG.ATTR_STOP_OPACITY];

            if (attrOffset != null)
            {
                Offset = new Length(attrOffset.Value);
            }

            if (attrStopColor != null)
            {
                StopColor = new NoneColor(attrStopColor.Value);
            }

            if (attrStopOpacity != null)
            {
                StopOpacity = new Opacity(attrStopOpacity.Value);
            }

            base.ParseXml(node);
        }
        #endregion
    }
}
