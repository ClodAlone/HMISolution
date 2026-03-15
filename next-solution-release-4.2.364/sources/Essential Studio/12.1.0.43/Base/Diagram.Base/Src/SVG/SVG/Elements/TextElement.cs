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
using System.Drawing.Drawing2D;
using System.Xml;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// TextElement class.
    /// </summary>
    public class TextElement : SuperElement
    {
        #region Properties
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
        /// Gets or sets the DX length.
        /// </summary>
        /// <value>The length.</value>
        public Length DX
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_DX, new Length(0));
            }
            set
            {
                m_attributes[SVG.ATTR_DX] = value;
            }
        }

        /// <summary>
        /// Gets or sets the DY length.
        /// </summary>
        /// <value>The length.</value>
        public Length DY
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_DY, new Length(0));
            }
            set
            {
                SetAttribute(SVG.ATTR_DY, value, new Length(0));
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
        /// Gets or sets the rotate.
        /// </summary>
        /// <value>The rotate.</value>
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
        {
            m_name = SVG.NAME_TEXT;
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Draws self.
        /// </summary>
        /// <param name="g">The graphics.</param>
        protected override void DrawSelf(Graphics g)
        {
            Font fnt = Utility.GetGDIFont(this as IFontAttributes);
            Brush br = Utility.GetGDIBrush(this);

            if (br != null && fnt != null)
            {
                g.DrawString(m_text, fnt, br, X.Value, Y.Value - fnt.Height);
            }
        }

        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="node">The node.</param>
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
