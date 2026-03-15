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
using System.Xml;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Implements the "text" element of SVG DOM.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class TextElement : SuperElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the X coordinate of text.
        /// </summary>
        /// <value>The X coordinate .</value>
        public Length X
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_X, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_X, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of text.
        /// </summary>
        /// <value>The Y coordinate.</value>
        public Length Y
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_Y, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_Y, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the DX.
        /// </summary>
        /// <value>The DX.</value>
        public Length DX
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_DX, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_DX, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the DY.
        /// </summary>
        /// <value>The DY.</value>
        public Length DY
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_DY, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_DY, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the length of the text.
        /// </summary>
        /// <value>The length of the text.</value>
        public Length TextLength
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_TEXT_LENGTH, new Length(0));
            }

            set
            {
                SetAttribute(SVG.ATTR_TEXT_LENGTH, value, new Length(0));
            }
        }

        /// <summary>
        /// Gets or sets the angle of rotatation.
        /// </summary>
        /// <value>The angle.</value>
        public Number Rotate
        {
            get
            {
                return (Number)GetAttribute(SVG.ATTR_ROTATE, new Number(0));
            }

            set
            {
                SetAttribute(SVG.ATTR_ROTATE, value, new Number(0));
            }
        }

        /// <summary>
        /// Gets or sets the length adjust.
        /// </summary>
        /// <value>The length adjust.</value>
        public ELengthAdjust LengthAdjust
        {
            get
            {
                return (ELengthAdjust)GetAttribute(SVG.ATTR_LENGTH_ADJUST, ELengthAdjust.Spacing);
            }

            set
            {
                SetAttribute(SVG.ATTR_LENGTH_ADJUST, value, ELengthAdjust.Spacing);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TextElement"/> class.
        /// </summary>
        public TextElement()
            : base(SVG.NAME_TEXT)
        {
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Draws the element to specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        protected override void DrawSelf(Graphics g)
        {
            Font fnt = Utility.GetGDIFont(this as IFontAttributes);
            Brush br = Utility.GetGDIBrush(this);

            if (br != null && fnt != null)
            {
                g.DrawString(this.Text, fnt, br, X.Value, Y.Value - fnt.Height);
            }
        }

        /// <summary>
        /// Parses the XML document.
        /// </summary>
        /// <param name="node">The <see cref="XmlNode"/>.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute attrX = node.Attributes[SVG.ATTR_X];
            XmlAttribute attrY = node.Attributes[SVG.ATTR_Y];
            XmlAttribute attrDX = node.Attributes[SVG.ATTR_DX];
            XmlAttribute attrDY = node.Attributes[SVG.ATTR_DY];
            XmlAttribute attrTextLength = node.Attributes[SVG.ATTR_TEXT_LENGTH];
            XmlAttribute attrRotate = node.Attributes[SVG.ATTR_ROTATE];
            XmlAttribute attrLengthAdjust = node.Attributes[SVG.ATTR_LENGTH_ADJUST];

            if (attrX != null)
            {
                X = Length.Parse(attrX.Value);
            }

            if (attrY != null)
            {
                Y = Length.Parse(attrY.Value);
            }

            if (attrDX != null)
            {
                DX = Length.Parse(attrDX.Value);
            }

            if (attrDY != null)
            {
                DY = Length.Parse(attrDY.Value);
            }

            if (attrTextLength != null)
            {
                TextLength = Length.Parse(attrTextLength.Value);
            }

            if (attrRotate != null)
            {
                Rotate = Number.Parse(attrRotate.Value);
            }

            if (attrLengthAdjust != null)
            {
                LengthAdjust = ELengthAdjust.Parse(attrLengthAdjust.Value);
            }

            base.ParseXml(node);
        }
        #endregion
    }
}
