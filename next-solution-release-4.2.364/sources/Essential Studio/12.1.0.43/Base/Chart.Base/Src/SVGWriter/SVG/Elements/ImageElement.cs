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
using System.Drawing.Imaging;
using System.IO;
using System.Xml;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class ImageElement : SuperElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the element width.
        /// </summary>
        /// <value>The width.</value>
        public Length Width
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_WIDTH, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_WIDTH, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the element height.
        /// </summary>
        /// <value>The height.</value>
        public Length Height
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_HEIGHT, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_HEIGHT, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        /// <value>The X coordinate.</value>
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
        /// Gets or sets the Y coordinate.
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
        /// Gets or sets the image referance.
        /// </summary>
        /// <value>The referance.</value>
        public string HRef
        {
            get
            {
                return (string)GetAttribute(SVG.ATTR_HREF, "");
            }

            set
            {
                this.SetAttribute(SVG.ATTR_HREF, value, "");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageElement"/> class.
        /// </summary>
        public ImageElement()
            : base(SVG.NAME_IMAGE)
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
        /// Creates the <see cref="ImageElement"/> instance by the specified <see cref="Image"/>.
        /// </summary>
        /// <param name="img">The <see cref="Image"/>.</param>
        /// <param name="x">The x-location of the element.</param>
        /// <param name="y">The y-location of the element.</param>
        /// <param name="width">The width of the element.</param>
        /// <param name="height">The height of the element.</param>
        /// <returns>The <see cref="ImageElement"/> instance.</returns>
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
        /// Draws the element to specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
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
            else if (HRef != "")
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
