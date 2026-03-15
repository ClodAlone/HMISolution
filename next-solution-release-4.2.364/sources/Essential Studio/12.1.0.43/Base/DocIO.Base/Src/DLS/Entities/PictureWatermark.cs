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

#region file using directives
using System;
using System.IO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;

#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
#else
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
#endif

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for Watermark.
    /// </summary>
    public class PictureWatermark : Watermark
    {
        #region Fields
        /// <summary>
        /// Picture watermark members.
        /// </summary>
        private WPicture m_picture;
        private ImageRecord m_imageRecord;
        private bool m_washout = true;
        private int m_originalPib = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Get/set picture scaling in percents.
        /// </summary>
        public float Scaling
        {
            get
            {
                return m_picture.HeightScale;
            }
            set
            {
                m_picture.HeightScale = m_picture.WidthScale = value;
            }
        }
        /// <summary>
        /// Get/set washout property for Picture watermark.
        /// </summary>
        public bool Washout
        {
            get
            {
                return m_washout;
            }
            set
            {
                m_washout = value;
            }
        }
#if SILVERLIGHT || WP
        /// <summary>
        /// Get/set picture for Picture watermark.
        /// </summary>
        internal Image Picture
#else
        public Image Picture
#endif
        {
            get
            {
                if (m_picture.Document == null && m_imageRecord != null)
                    return GetImage(m_imageRecord.ImageBytes);
                return m_picture.Image;
            }
            set
            {
                m_originalPib = -1;
                if (m_picture.Document != null)
                    m_picture.LoadImage(value);
                else
                {
                    byte[] imageBytes;
#if SILVERLIGHT || WP
                    imageBytes = value.ImageData;
#else
                    if (value is Metafile)
                        imageBytes = WPicture.LoadMetafile(value as Metafile);
                    else
                        imageBytes = WPicture.LoadBitmap(value);
#endif
                    m_imageRecord = new ImageRecord(null, imageBytes);
#if SILVERLIGHT || WP
                    if (value.IsMetafile)
#else
                    if (value is Metafile)
#endif
                        m_imageRecord.IsMetafile = true;
                }
            }
        }
        /// <summary>
        /// Gets/sets the WPicture.
        /// </summary>
        internal WPicture WordPicture
        {
            get
            {
                return m_picture;
            }
            set
            {
                m_picture = value;
            }
        }
        /// <summary>
        /// Gets/sets index in BStoreContainer which contains
        /// picture watermark data (image). 
        /// </summary>
        internal int OriginalPib
        {
            get
            {
                return m_originalPib;
            }
            set
            {
                m_originalPib = value;
            }
        }
        #endregion

#if SILVERLIGHT || WP
        /// <summary>
        /// Load watermark image
        /// </summary>
        /// <param name="bytes"></param>
        public void LoadPicture(byte[] bytes) 
        {
            if (bytes == null)
                throw new ArgumentNullException("image bytes is empty");

            MemoryStream stream = new MemoryStream(bytes);
            Picture = new Image(stream);
        }
#endif

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PictureWatermark"/> class.
        /// </summary>
        public PictureWatermark()
            : base(WatermarkType.PictureWatermark)
        {
            m_picture = new WPicture(null);
            //Apply Default WaterMark properties as per MSWord behavior
            m_picture.HorizontalAlignment = ShapeHorizontalAlignment.Center;
            m_picture.VerticalAlignment = ShapeVerticalAlignment.Center;
            m_picture.TextWrappingStyle = TextWrappingStyle.Behind;
            m_picture.IsBelowText = true;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PictureWatermark"/> class.
        /// </summary>
        /// <param name="image">Image for picture watermark</param>
        /// <param name="washout">Washout property</param>
#if SILVERLIGHT || WP
        internal PictureWatermark(Image image, bool washout) :
#else
		public PictureWatermark(Image image, bool washout) :
#endif
            this()
        {
            Picture = image;
            m_washout = washout;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PictureWatermark"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal PictureWatermark(WordDocument doc)
            : base(doc, WatermarkType.PictureWatermark)
        {
            m_picture = new WPicture(doc);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Updates the image.
        /// </summary>
        internal void UpdateImage()
        {
            if (m_imageRecord != null)
            {
                m_picture.LoadImage(m_imageRecord.ImageBytes, m_imageRecord.IsMetafile);
                m_imageRecord.Close();
                m_imageRecord = null;
            }
        }
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        /// <returns></returns>
        private Image GetImage(byte[] imageBytes)
        {
            Image image = null;
            if (imageBytes != null)
            {
                try
                {
#if SILVERLIGHT || WP
                    image = Image.FromStream(new MemoryStream(imageBytes));
#else
                    image = Image.FromStream(new MemoryStream(imageBytes), true, false);
#endif
                    imageBytes = null;
                }
                catch
                {
                    throw new ArgumentException("Argument is not image byte array");
                }
            }
            return image;
        }
//#if !SILVERLIGHT
        /// <summary>
        /// Registers child objects in XDSL holder.
        /// </summary>
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.ImageTag, m_picture);
        }
        /// <summary>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            if (!m_washout)
            {
                writer.WriteValue(XDLSConstants.WatermarkPictureWashoutAttr, m_washout);
            }
            if (m_originalPib != -1)
            {
                writer.WriteValue(XDLSConstants.WatermarkPicturePibAttr, m_originalPib);
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.WatermarkPictureWashoutAttr))
            {
                m_washout = reader.ReadBoolean(XDLSConstants.WatermarkPictureWashoutAttr);
            }
            if (reader.HasAttribute(XDLSConstants.WatermarkPicturePibAttr))
            {
                m_originalPib = reader.ReadInt(XDLSConstants.WatermarkPicturePibAttr);
            }
        }
//#endif
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            PictureWatermark pw = (PictureWatermark)base.CloneImpl();
            pw.WordPicture = (WPicture)WordPicture.Clone();
            return pw;
        }
        #endregion
    }
}
