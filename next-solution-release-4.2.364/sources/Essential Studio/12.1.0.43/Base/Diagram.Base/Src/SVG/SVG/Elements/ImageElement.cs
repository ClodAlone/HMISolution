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
using System.Drawing.Imaging;
using System.IO;
using System.Xml;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// ImageElement class.
    /// </summary>
    public class ImageElement : SuperElement
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
        /// Gets or sets the H ref.
        /// </summary>
        /// <value>The H ref.</value>
        public string HRef
        {
            get
            {
                return (string)GetAttribute(SVG.ATTR_HREF, string.Empty);
            }
            set
            {
                SetAttribute(SVG.ATTR_HREF, value, string.Empty);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageElement"/> class.
        /// </summary>
        public ImageElement()
        {
            m_name = SVG.NAME_IMAGE;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="node">The node.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute x = node.Attributes[SVG.ATTR_X];
            XmlAttribute y = node.Attributes[SVG.ATTR_Y];
            XmlAttribute width = node.Attributes[SVG.ATTR_WIDTH];
            XmlAttribute heigth = node.Attributes[SVG.ATTR_HEIGHT];
            XmlAttribute href = node.Attributes[SVG.ATTR_HREF];

            if (x != null)
            {
                X = new Length(x.Value);
            }

            if (y != null)
            {
                Y = new Length(y.Value);
            }

            if (width != null)
            {
                Width = new Length(width.Value);
            }

            if (heigth != null)
            {
                Height = new Length(heigth.Value);
            }

            if (href != null)
            {
                HRef = href.Value;
            }

            base.ParseXml(node);
        }

        /// <summary>
        /// Froms the image.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Image element</returns>
        public static ImageElement FromImage(Image img, float x, float y, float width, float height)
        {
            ImageElement res = new ImageElement();

            MemoryStream memstrImage = new MemoryStream();
            img.Save(memstrImage, ImageFormat.Png);

            res.HRef = SVG.VALUE_IMAGE_TYPE + System.Convert.ToBase64String(memstrImage.ToArray());
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
            Image img = null;
            float x = X.GetValue(g.ClipBounds.Width);
            float y = Y.GetValue(g.ClipBounds.Height);
            float w = Width.GetValue(g.ClipBounds.Width);
            float h = Height.GetValue(g.ClipBounds.Height);

            if (HRef.IndexOf(SVG.VALUE_IMAGE_TYPE) == 0)
            {
                string buf = HRef.Substring(SVG.VALUE_IMAGE_TYPE.Length);
                byte[] bbuf = Convert.FromBase64String(buf);
                MemoryStream memStream = new MemoryStream(bbuf);
                img = Image.FromStream(memStream);
            }
            else if (HRef != string.Empty)
            {
                img = Image.FromFile(HRef);
            }

            if (img != null)
            {
                g.DrawImage(img, new RectangleF(x, y, w, h));
            }

            base.DrawSelf(g);
        }
        #endregion
    }
}
