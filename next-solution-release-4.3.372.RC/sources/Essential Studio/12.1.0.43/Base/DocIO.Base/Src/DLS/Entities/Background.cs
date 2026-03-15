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

#region File using directives
using System;
using System.IO;
using System.Text;
using System.Xml;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.DLS.Entities;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
#else
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
using System.Drawing.Imaging;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for BackgroundEffect.
    /// </summary>
    public class Background : XDLSSerializableBase
    {
        #region Fields
        /// <summary>
        /// Class fields.
        /// </summary>
        private BackgroundType m_effectType;
        private Color m_color = Color.White;
        private Color m_backColor = Color.White;
        private ImageRecord m_imageRecord;
        private BackgroundGradient m_gradient = new BackgroundGradient();

        private BackgroundFillType m_fillType;
        private EscherClass m_escher;        
        private Stream m_patternFill;
        private byte[] m_patternImage;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the type of background effect for document.
        /// </summary>
        public BackgroundType Type
        {
            get
            {
                return m_effectType;
            }
            set
            {
                m_effectType = value;
            }
        }
#if SILVERLIGHT || WP
        /// <summary>
        /// Get/set background picture bytes.
        /// </summary>
        public byte[] Picture
        {
            get
            {
                if (m_imageRecord == null)
                    return null;
                return m_imageRecord.ImageBytes;
            }
            set
            {
                if (m_imageRecord != null)
                    m_imageRecord.OccurenceCount--;
                m_fillType = BackgroundFillType.msofillPicture;
                LoadImage(value);
            }
        }        
#else
        /// <summary>
        /// Get/set background picture.
        /// </summary>
        public Image Picture
        {
            get
            {
                return GetImage();
            }
            set
            {
                if (m_imageRecord != null)
                    m_imageRecord.OccurenceCount--;
                m_fillType = BackgroundFillType.msofillPicture;
                LoadImage(value);
            }
        }
#endif
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>The image.</value>
        internal Image Image
        {
            get
            {
                return GetImage();
            }
        }
        /// <summary>
        /// Get/set background color.
        /// </summary>
        public Color Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }
        /// <summary>
        /// Get/set background gradient.
        /// </summary>
        public BackgroundGradient Gradient
        {
            get
            {
                return m_gradient;
            }
            set
            {
                m_gradient = value;
            }
        }
        /// <summary>
        /// Gets the image record.
        /// </summary>
        /// <value>The image record.</value>
        internal ImageRecord ImageRecord
        {
            get
            {
                return m_imageRecord;
            }
            set
            {
                if (m_imageRecord != null)
                    m_imageRecord.OccurenceCount--;
                m_imageRecord = value;
                m_imageRecord.OccurenceCount++;
            }
        }
        /// <summary>
        /// Get/set background image as byte array. 
        /// </summary>
        internal byte[] ImageBytes
        {
            get
            {
                if (m_imageRecord == null)
                    return null;
                return m_imageRecord.ImageBytes;
            }
            set
            {
                if (m_imageRecord != null)
                    m_imageRecord.OccurenceCount--;
#if SILVERLIGHT || WP
                LoadImage(value);
#else
                LoadImage(GetImage(value));
#endif
            }
        }
        /// <summary>
        /// Get/set background fill type.
        /// </summary>
        internal BackgroundFillType FillType
        {
            get
            {
                return m_fillType;
            }
            set
            {
                m_fillType = value;
            }
        }
        /// <summary>
        /// Get background color for picture background.
        /// </summary>
        internal Color PictureBackColor
        {
            get
            {
                return m_backColor;
            }
        }
        /// <summary>
        /// Gets or sets a pattern fill string.
        /// </summary>
        /// <value><c>true</c> if pattern fill string; otherwise, <c>false</c>.</value>
        internal Stream PatternFill
        {
            get
            {
                return m_patternFill;
            }
            set
            {
                m_patternFill = value;
            }
        }
        /// <summary>
        /// Gets or sets the pattern image.
        /// </summary>
        /// <value>The pattern image.</value>
        internal byte[] PatternImageBytes
        {
            get
            {
                return m_patternImage;
            }
            set
            {
                m_patternImage = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Background"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="type">The type.</param>
        internal Background(WordDocument doc, BackgroundType type)
            : base(doc, null)
        {
            m_effectType = type;
        }
        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="doc"></param>
        internal Background(WordDocument doc)
            : base(doc, null)
        {
            m_escher = doc.Escher;
            GetBackgroundData(m_escher.BackgroundContainer, true);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Background"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="container">The container.</param>
        internal Background(WordDocument doc, MsofbtSpContainer container)
            : base(doc, null)
        {
            m_escher = doc.Escher;
            GetBackgroundData(container, false);
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Clones background object.
        /// </summary>
        /// <returns>Exact copy of background object</returns>
        internal Background Clone()
        {
            Background background = new Background(Document, Type);
            background.m_imageRecord = m_imageRecord;
            background.Gradient = Gradient.Clone();
            background.Color = Color;

            return background;
        }
        /// <summary>
        /// Updates the image record.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal void UpdateImageRecord(WordDocument doc)
        {
            if (m_imageRecord != null)
            {
                ImageRecord image = m_imageRecord;
                if (image.IsMetafile)
                    m_imageRecord = doc.Images.LoadMetaFileImage(image.m_imageBytes, true);
                else
                    m_imageRecord = doc.Images.LoadImage(image.ImageBytes);
                m_imageRecord.Size = image.Size;
                m_imageRecord.ImageFormat = image.ImageFormat;
                m_imageRecord.Length = image.Length;
                image.Close();
                image = null;
            }
        }
        #endregion

        #region Class overrides
//#if !SILVERLIGHT
        /// <summary>
        /// Reads object data from xml attributes.
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);
            if (reader.HasAttribute(XDLSConstants.BackgroundTypeAttr))
            {
                m_effectType = (BackgroundType)reader.ReadEnum(XDLSConstants.BackgroundTypeAttr,
                  typeof(BackgroundType));
            }
            if (reader.HasAttribute(XDLSConstants.BackgroundColorAttr))
            {
                m_color = reader.ReadColor(XDLSConstants.BackgroundColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.BackgroundBackColorAttr))
            {
                m_backColor = reader.ReadColor(XDLSConstants.BackgroundBackColorAttr);
            }
        }
        /// <summary>
        /// Writes object data as xml attributes.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.BackgroundTypeAttr, m_effectType);
            if (m_imageRecord != null && m_imageRecord.IsMetafile)
            {
                writer.WriteValue(XDLSConstants.BackImageIsMetaAttr, m_imageRecord.IsMetafile);
            }
            if (m_color != Color.White)
            {
                writer.WriteValue(XDLSConstants.BackgroundColorAttr, m_color);
            }
            if (m_backColor != Color.White)
            {
                writer.WriteValue(XDLSConstants.BackgroundBackColorAttr, m_backColor);
            }
        }
        /// <summary>
        /// Writes object data as inside xml element.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(IXDLSContentWriter writer)
        {
            base.WriteXmlContent(writer);

            if (ImageBytes != null)
            {
                writer.WriteChildBinaryElement(XDLSConstants.ImageTag, ImageBytes);
            }
            else
            {
                ( writer as XDLSWriter ).WriteImage(Image);
            }
        }
        /// <summary>
        /// Reads object data from xml attributes.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlContent(IXDLSContentReader reader)
        {
            bool retVal = base.ReadXmlContent(reader);
            if (reader.TagName == XDLSConstants.ImageTag)
            {
#if SILVERLIGHT || WP
                LoadImage(reader.ReadChildBinaryElement());
#else
                Image image = GetImage(reader.ReadChildBinaryElement());
                LoadImage(image);
#endif
            }

            return retVal;
        }
        /// <summary>
        /// Registers child objects in XDSL holder.
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();
            XDLSHolder.AddElement(XDLSConstants.BackgroundGradientTag, m_gradient);
        }
//#endif
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the background data.
        /// </summary>
        private void GetBackgroundData(MsofbtSpContainer container, bool isDocBackground)
        {
            if (container == null || !container.HasFillEffect())
                return;

            m_fillType = container.GetBackgroundFillType();
            m_effectType = container.GetBackgroundType();
            if (m_effectType == BackgroundType.NoBackground && isDocBackground)
            {
                m_effectType = (m_doc.DOP.Dop2003.DispBkSpSaved ? BackgroundType.Color : BackgroundType.NoBackground);
            }

            if (m_effectType == BackgroundType.NoBackground)
                return;

            switch (m_effectType)
            {
                case BackgroundType.Color:
                    m_color = container.GetBackgroundColor(false);
                    if (m_effectType == BackgroundType.Color && m_color == Color.White)
                    {
                        m_effectType = BackgroundType.NoBackground;
                    }
                    break;
                case BackgroundType.Texture:
                case BackgroundType.Picture:
                    m_imageRecord = container.GetBackgroundImage(m_escher);
                    m_backColor = container.GetBackgroundColor(true);
                    break;
                case BackgroundType.Gradient:
                    m_gradient = new BackgroundGradient(Document, container);
                    break;
            }
        }
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <returns></returns>
        private Image GetImage()
        {
            Image image = null;
            if (ImageBytes != null)
            {
                try
                {
#if SILVERLIGHT || WP
                    image = Image.FromStream(new MemoryStream(ImageBytes));
#else
                    image = Image.FromStream(new MemoryStream(ImageBytes), true, false);
#endif
                }
                catch
                {
                    throw new ArgumentException("Argument is not image byte array");
                }
            }
            return image;
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
#if SILVERLIGHT || WP
        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        private void LoadImage(byte[] imageBytes)
        {
            if (imageBytes == null)
            {
                throw new ArgumentNullException("image");
            }
            Image image = GetImage(imageBytes);
            if (image.IsMetafile)
                m_imageRecord = Document.Images.LoadMetaFileImage(imageBytes, false);
            else
                m_imageRecord = Document.Images.LoadImage(imageBytes);
        }
#else
        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="image">The image.</param>
        private void LoadImage(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            
            if (image is Metafile)
                m_imageRecord = Document.Images.LoadMetaFileImage(WPicture.LoadMetafile(image as Metafile), false);
            else
                m_imageRecord = Document.Images.LoadImage(WPicture.LoadBitmap(image));
            m_imageRecord.UpdateImageSize(image);
        }
#endif
        #endregion
    }
}
