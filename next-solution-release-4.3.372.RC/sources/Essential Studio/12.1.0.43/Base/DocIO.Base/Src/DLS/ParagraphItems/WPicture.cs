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
using System.Collections;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.Layouting;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.ReaderWriter.Escher;
using System.Xml;
using System.Collections.Generic;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
using ImageFormat = Syncfusion.DocIO.DLS.Entities.ImageFormat;
#else
using Syncfusion.DocIO.Rendering;
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
using System.Drawing.Imaging;
#endif
#if WINRT
using Syncfusion.DocIO.Security.Cryptography;
#else
#if !WP
using System.Drawing;
#endif
using System.Security.Cryptography;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WPicture.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WPicture
      : ParagraphItem
#if !SILVERLIGHT && !WP
      , ILeafWidget
#endif
      , IWPicture
    {
        #region Class members
        /// <summary>
        /// Size of the picture.
        /// </summary>
        private SizeF m_size;
        /// <summary>
        ///  in percent
        /// </summary>
        private float m_widthScale = 100f;
        /// <summary>
        ///  in percent
        /// </summary>
        private float m_heightScale = 100f;
        /// <summary>
        /// 
        /// </summary>
        private HorizontalOrigin m_horizontalOrigin = HorizontalOrigin.Margin;
        /// <summary>
        /// 
        /// </summary>
        private ShapePosition m_shapePosition = ShapePosition.Static;
        /// <summary>
        /// 
        /// </summary>
        private VerticalOrigin m_verticalOrigin = VerticalOrigin.Margin;
        /// <summary>
        /// 
        /// </summary>
        private float m_horizPosition = 0;
        private TileRectangle m_fillRectable;
        /// <summary>
        /// 
        /// </summary>
        private float m_vertPosition = 0;
        /// <summary>
        /// DistanceBottom	Returns or sets the distance (in points) between the document text and the bottom edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        private float m_DistanceFromBottom=0.0f;
        /// <summary>
        /// DistanceLeft	Returns or sets the distance (in points) between the document text and the left edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        private float m_DistanceFromLeft=9.0f;
        /// <summary>
        /// DistanceRight	Returns or sets the distance (in points) between the document text and the right edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        private float m_DistanceFromRight=9.0f;
        /// <summary>
        /// DistanceTop	Returns or sets the distance (in points) between the document text and the top edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        private float m_DistanceFromTop=0.0f;
        /// <summary>
        /// 
        /// </summary>
        private TextWrappingStyle m_wrappingStyle = TextWrappingStyle.Inline;
        /// <summary>
        /// 
        /// </summary>
        private TextWrappingType m_wrappingType = TextWrappingType.Both;
        /// <summary>
        /// 
        /// </summary>
        private ShapeHorizontalAlignment m_horAlignment = ShapeHorizontalAlignment.None;
        /// <summary>
        /// 
        /// </summary>
        private ShapeVerticalAlignment m_vertAlignment = ShapeVerticalAlignment.None;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isBelowText = false;
        /// <summary>
        /// 
        /// </summary>
        private int m_spid = -1;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isHeader;
        /// <summary>
        /// 
        /// </summary>
        private InlineShapeObject m_inlinePictureShape;
        /// <summary>
        /// Holds additional docx picture props
        /// </summary>
        private List<Stream> m_docxProps;
        /// <summary>
        /// 
        /// </summary>
        private string m_altText;
        /// <summary>
        /// 
        /// </summary>
        private string m_title;
        /// <summary>
        /// 
        /// </summary>
        private WTextBody m_embedBody;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isShape;
        /// <summary>
        /// 
        /// </summary>
        private int m_orderIndex = int.MaxValue;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bLayoutInTableCell = true;
        private bool m_allowoverlap = true;
        private ImageRecord m_imageRecord;
        //indicate whether current wrapping bounds points added to the list or not. 
        internal bool IsWrappingBoundsAdded = false;
        //hold the wrapping bounds index.
        internal int WrapCollectionIndex = -1;
        private WrapPolygon m_wrapPolygon;
        #endregion

        #region Class properties
        /// <summary>
        /// it hold the picture rectangle value
        /// </summary>
        internal TileRectangle FillRectangle
        {
            get
            {
                if (m_fillRectable == null)
                    m_fillRectable = new TileRectangle();
                return m_fillRectable;
            }
            set { m_fillRectable = value; }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has borders.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has borders; otherwise, <c>false</c>.
        /// </value>
        internal bool HasBorder
        {
            get
            {
                if (TextWrappingStyle == TextWrappingStyle.Inline)
                {
                    bool isDefault = (PictureShape.PictureDescriptor.BorderBottom.IsDefault
                        && PictureShape.PictureDescriptor.BorderLeft.IsDefault
                        && PictureShape.PictureDescriptor.BorderRight.IsDefault
                        && PictureShape.PictureDescriptor.BorderTop.IsDefault);

                    if (IsShape)
                        isDefault |= (PictureShape.PictureDescriptor.BorderBottom.BorderType == (byte)BorderStyle.None
                            && PictureShape.PictureDescriptor.BorderLeft.BorderType == (byte)BorderStyle.None
                            && PictureShape.PictureDescriptor.BorderRight.BorderType == (byte)BorderStyle.None
                            && PictureShape.PictureDescriptor.BorderTop.BorderType == (byte)BorderStyle.None);
                    return !isDefault;
                }
                else
                    return (PictureShape.ShapeContainer != null
                        && PictureShape.ShapeContainer.ShapeOptions != null
                        && PictureShape.ShapeContainer.ShapeOptions.LineProperties.HasDefined
                        && PictureShape.ShapeContainer.ShapeOptions.LineProperties.UsefLine
                        && PictureShape.ShapeContainer.ShapeOptions.LineProperties.Line);
            }
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.Picture;
            }
        }
        /// <summary>
        /// Gets / sets picture height.
        /// </summary>
        public float Height
        {
            get
            {
                return Size.Height;
            }
            set
            {
                m_size.Height = value;
            }
        }
        /// <summary>
        /// Gets / sets picture width.
        /// </summary>
        public float Width
        {
            get
            {
                return Size.Width;
            }
            set
            {
                m_size.Width = value;
            }
        }
        /// <summary>
        /// Gets / sets picture height scale factor in percent.
        /// </summary>
        public float HeightScale
        {
            get
            {
                return m_heightScale;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Scale factor must be greater than 0");
                }
                m_heightScale = value;
            }
        }
        /// <summary>
        /// Gets / sets picture width scale factor in percent.
        /// </summary>
        public float WidthScale
        {
            get
            {
                return m_widthScale;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Scale factor must be greater than 0");
                }
                m_widthScale = value;
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets internal System.Drawing.Image object.
        /// </summary>
        public Image Image
        {
            get
            {
                return GetImage(ImageBytes);
            }
        }
#else
        /// <summary>
        /// Gets internal System.Drawing.Image object.
        /// </summary>
        internal Image Image
        {
          get
          {
              return GetImage(ImageBytes);
          }
        }

#endif
        /// <summary>
        /// Gets image byte array.
        /// </summary>
        public byte[] ImageBytes
        {
            get
            {
                if (m_imageRecord == null)
                    return null;
                return m_imageRecord.ImageBytes;
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
        }
        #region commented
        //    /// <summary>
        //    /// Gets/sets picture brightness.
        //    /// </summary>
        //    public float Brightness
        //    {
        //      get
        //      {
        //        return m_brightness;
        //      }
        //      set
        //      {
        //        if( value < 0 || value > 100 )
        //        {
        //          throw  new ArgumentOutOfRangeException( "Picture brighness must be greater than 0 and lower than 100" );
        //        }
        //        m_brightness = value;
        //      }
        //    }
        //    /// <summary>
        //    /// Get/set picture contrast.
        //    /// </summary>
        //    public float Contrast
        //    {
        //      get
        //      {
        //        return m_contrast;
        //      }
        //      set
        //      {
        //        if( value < 0 || value > 100 )
        //        {
        //          throw  new ArgumentOutOfRangeException( "Picture contrast must be greater than 0 and lower than 100" );
        //        }
        //        m_contrast = value;
        //      }
        //    }
        //    /// <summary>
        //    /// Getset picture color.
        //    /// </summary>
        //    public PictureColor Color
        //    {
        //      get
        //      {
        //        return m_color;
        //      }
        //      set
        //      {
        //        m_color = value;
        //        if( m_color == PictureColor.Washout )
        //        {
        //          m_contrast = 15;
        //          m_brightness = 85;
        //        }
        //      }
        //    }

        //    /// <summary>
        //    /// Get/set crop from left value.
        //    /// </summary>
        //    public float CropFromLeft
        //    {
        //      get
        //      {
        //        return m_cropLeft;
        //      }
        //      set
        //      {
        //        m_cropLeft = value;
        //      }
        //    }
        //    /// <summary>
        //    /// Get/set crop from right value.
        //    /// </summary>
        //    public float CropFromRight
        //    {
        //      get
        //      {
        //        return m_cropRight;
        //      }
        //      set
        //      {
        //        m_cropRight = value;
        //      }
        //    }
        //    /// <summary>
        //    /// Get/set crop from top value.
        //    /// </summary>
        //    public float CropFromTop
        //    {
        //      get
        //      {
        //        return m_cropTop;
        //      }
        //      set
        //      {
        //        m_cropTop = value;
        //      }
        //    }
        //    /// <summary>
        //    /// Get/set crop from bottom value.
        //    /// </summary>
        //    public float CropFromBottom
        //    {
        //      get
        //      {
        //        return m_cropBottom;
        //      }
        //      set
        //      {
        //        m_cropBottom = value;
        //      }
        //    } 
        #endregion
        /// <summary>
        /// Get/Set Shapeposition
        /// </summary>
        internal ShapePosition Position
        {
            get
            {
                return m_shapePosition;
            }
            set
            {
                m_shapePosition = value;
            }
        }
        /// <summary>
        /// Gets \ sets horizontal origin of the picture.
        /// </summary>
        public HorizontalOrigin HorizontalOrigin
        {
            get
            {
                return m_horizontalOrigin;
            }
            set
            {
                m_horizontalOrigin = value;
            }
        }
        /// <summary>
        /// Gets \ sets absolute horizontal position of the picture.
        /// </summary>
        public VerticalOrigin VerticalOrigin
        {
            get
            {
                return m_verticalOrigin;
            }
            set
            {
                m_verticalOrigin = value;
            }
        }
        /// <summary>
        /// Gets \ sets absolute horizontal position of the picture.
        /// </summary>
        /// <remarks>
        /// The value is measured in points and the position is relative to HorizontalOrigin.
        /// </remarks>
        public float HorizontalPosition
        {
            get
            {
                return m_horizPosition;
            }
            set
            {
                m_horizPosition = value;
            }
        }
        /// <summary>
        /// Gets \ sets absolute vertical position of the picture.
        /// </summary>
        /// <remarks>
        /// The value is measured in points and the position is relative to VerticalOrigin.
        /// </remarks>
        public float VerticalPosition
        {
            get
            {
                return m_vertPosition;
            }
            set
            {
                m_vertPosition = value;
            }
        }
        /// <summary>
        /// DistanceBottom	Returns or sets the distance (in points) between the document text and the bottom edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        internal float DistanceFromBottom
        {
            get { return m_DistanceFromBottom; }
            set { m_DistanceFromBottom = value; }
        }
        /// <summary>
        /// DistanceLeft	Returns or sets the distance (in points) between the document text and the left edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        internal float DistanceFromLeft
        {
            get { return m_DistanceFromLeft; }
            set { m_DistanceFromLeft = value; }
        }
        /// <summary>
        /// DistanceRight	Returns or sets the distance (in points) between the document text and the right edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        internal float DistanceFromRight
        {
            get { return m_DistanceFromRight; }
            set { m_DistanceFromRight = value; }
        }
        /// <summary>
        /// DistanceTop	Returns or sets the distance (in points) between the document text and the top edge of the text-free area surrounding the specified shape. Read/write Single.
        /// </summary>
        internal float DistanceFromTop
        {
            get { return m_DistanceFromTop; }
            set { m_DistanceFromTop = value; }
        }
        /// <summary>
        /// Gets \ sets text wrapping style of the picture.
        /// </summary>
        public TextWrappingStyle TextWrappingStyle
        {
            get
            {
                return m_wrappingStyle;
            }
            set
            {
                if (HasBorder)
                {
                    if (m_wrappingStyle == TextWrappingStyle.Inline
                        && value != TextWrappingStyle.Inline)
                        PictureShape.ConvertToShape();
                    else if (m_wrappingStyle != TextWrappingStyle.Inline
                        && value == TextWrappingStyle.Inline)
                        PictureShape.ConvertToInlineShape();
                }
                m_wrappingStyle = value;
            }
        }
        /// <summary>
        /// Gets \ sets text wrapping type of the picture.
        /// </summary>
        public TextWrappingType TextWrappingType
        {
            get
            {
                return m_wrappingType;
            }
            set
            {
                m_wrappingType = value;
            }
        }
        /// <summary>
        /// Gets / sets picture horizontal alignment.
        /// </summary>
        /// <remarks>
        /// If it is set as None, then the picture is explicitly positioned using position properties. Otherwise it is positioned according to the alignment specified. The position of the object is relative to HorizontalOrigin.
        /// </remarks>
        public ShapeHorizontalAlignment HorizontalAlignment
        {
            get
            {
                return m_horAlignment;
            }
            set
            {
                m_horAlignment = value;
            }
        }
        /// <summary>
        /// Gets / sets picture vertical alignment.
        /// </summary>
        /// <remarks>
        /// If it is set as None, then the picture is explicitly positioned using position properties. Otherwise it is positioned according to the alignment specified. The position of the object is relative to VerticalOrigin.
        /// </remarks>
        public ShapeVerticalAlignment VerticalAlignment
        {
            get
            {
                return m_vertAlignment;
            }
            set
            {
                m_vertAlignment = value;
            }
        }
        /// <summary>
        /// Gets \ sets whether picture is below image.
        /// </summary>
        public bool IsBelowText
        {
            get
            {
                return m_isBelowText;
            }
            set
            {
                m_isBelowText = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal WCharacterFormat PictureCharacterFormat
        {
            get
            {
                return m_charFormat;
            }
            set
            {
                m_charFormat = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int ShapeId
        {
            get
            {
                return m_spid;
            }
            set
            {
                m_spid = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsHeaderPicture
        {
            get
            {
                return m_isHeader;
            }
            set
            {
                m_isHeader = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal InlineShapeObject PictureShape
        {
            get
            {
                return m_inlinePictureShape;
            }
            set
            {
                m_inlinePictureShape = value;
            }
        }
        /// <summary>
        /// Gets/sets the size of picture object.
        /// </summary>
        internal SizeF Size
        {
            get
            {
                if (m_size.Width == float.MinValue
                    || m_size.Height == float.MinValue)
                    CheckPicSize(Image);
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is metafile.
        /// </summary>
        /// <value>
        /// 	if this instance is metafile, set to <c>true</c>.
        /// </value>
        internal bool IsMetaFile
        {
            get
            {
                if (m_imageRecord == null)
                    return false;
                return m_imageRecord.IsMetafile;
            }
        }
        /// <summary>
        /// Gets the additional docx properties.
        /// </summary>
        /// <value>The docx props.</value>
        internal List<Stream> DocxProps
        {
            get
            {
                if (m_docxProps == null)
                {
                    m_docxProps = new List<Stream>();
                }
                return m_docxProps;
            }
        }
        /// <summary>
        /// Gets the alternative text.
        /// </summary>
        /// <value>The alternative text.</value>
        public string AlternativeText
        {
            get
            {
                return m_altText;
            }
            set
            {
                m_altText = value;
            }
        }
        /// <summary>
        /// Gets the picture title.
        /// </summary>
        /// <value>The alternative text.</value>
        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
            }
        }
        /// <summary>
        /// Embedded text body
        /// </summary>
        internal WTextBody EmbedBody
        {
            get
            {
                return m_embedBody;
            }
            set
            {
                m_embedBody = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is shape.
        /// </summary>
        /// <value>
        /// 	if this instance is shape, set to <c>true</c>.
        /// </value>
        internal bool IsShape
        {
            get
            {
                return m_isShape;
            }
            set
            {
                m_isShape = value;
            }
        }
        /// <summary>
        /// Gets or sets the index of the order.
        /// </summary>
        /// <value>The index of the order.</value>
        internal int OrderIndex
        {
            get
            {
                if (m_orderIndex == int.MaxValue)
                {
                    //Update shape order index
                    if (Document != null && !Document.IsOpening && Document.Escher != null)
                    {
                        int index = Document.Escher.GetShapeOrderIndex(this.ShapeId);
                        if (index != -1)
                            m_orderIndex = index;
                    }
                }
                return m_orderIndex;
            }
            set
            {
                m_orderIndex = value;
            }
        }
        /// <summary>
        /// Returns the boolean value that represents whether a picture in a table is displayed inside or outside the table.
        /// </summary>
        internal bool LayoutInCell
        {
            get
            {
                return m_bLayoutInTableCell;
            }
            set
            {
                m_bLayoutInTableCell = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow overlap].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow overlap]; otherwise, <c>false</c>.
        /// </value>
        internal bool AllowOverlap
        {
            get
            {
                return m_allowoverlap;
            }
            set
            {
                m_allowoverlap = value;
            }
        }

        /// <summary>
        /// Gets or sets the wrap polygon.
        /// </summary>
        /// <value>
        /// The wrap polygon.
        /// </value>
        internal WrapPolygon WrapPolygon
        {
            get
            {
                if (m_wrapPolygon == null)
                {
                    m_wrapPolygon = new WrapPolygon();
                    m_wrapPolygon.Edited = false;
                    //Handled to add default wrap polygon veritces
                    m_wrapPolygon.Vertices.Add(new PointF(0, 0));
                    m_wrapPolygon.Vertices.Add(new PointF(0, 21600));
                    m_wrapPolygon.Vertices.Add(new PointF(21600, 21600));
                    m_wrapPolygon.Vertices.Add(new PointF(21600, 0));
                    m_wrapPolygon.Vertices.Add(new PointF(0, 0));
                }
                return m_wrapPolygon;
            }
            set { m_wrapPolygon = value; }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WPicture"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public WPicture(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_charFormat = new WCharacterFormat(doc);
            m_charFormat.SetOwner(this);
            m_inlinePictureShape = new InlineShapeObject(doc);
#if WINRT || WP
            m_size = new SizeF();
#endif
            m_size.Height = float.MinValue;
            m_size.Width = float.MinValue;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Loads image as bytes array.
        /// </summary>
        /// <param name="imageBytes"></param>
        public void LoadImage(byte[] imageBytes)
        {
            if (imageBytes == null)
                throw new ArgumentNullException("Image bytes cannot be null or empty");
            ResetImageData();
            Image image = GetImage(imageBytes);
#if SILVERLIGHT || WP || WINRT
            if (image.IsMetafile)
#else
            if (image is Metafile)
#endif
                LoadImage(imageBytes, true);
            else
            {
#if !SILVERLIGHT && !WP
                if (image != null && (image.RawFormat.Equals(ImageFormat.Tiff)
                  || image.RawFormat.Equals(ImageFormat.Bmp)))
                    ConvertBitmap(image);
                else
#endif
                    LoadImage(imageBytes, false);
            }
            imageBytes = null;
            CheckPicSize(image);
            //Update image record in Blip
            UpdateBlipImageRecord();
        }
#if SILVERLIGHT || WP
        /// <summary>
        /// Loads image.
        /// </summary>
        internal void LoadImage(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            ResetImageData();

            if (image.IsMetafile)
                m_imageRecord = Document.Images.LoadMetaFileImage(image.ImageData, false);
            else
                m_imageRecord = Document.Images.LoadImage(image.ImageData);
            CheckPicSize(image);
            //Update image record in Blip
            UpdateBlipImageRecord();
        }

        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        public void LoadImage(Stream imageStream)
        {
            if (imageStream == null)
            {
                throw new ArgumentNullException("imageStream");
            }

            Image image = new Image( imageStream );
            LoadImage(image);
        }
#else
        /// <summary>
        /// Loads image.
        /// </summary>
        /// <param name="image">The image.</param>
        public void LoadImage(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            ResetImageData();

            if (image is Metafile)
            {
                m_imageRecord = Document.Images.LoadMetaFileImage(LoadMetafile(image as Metafile), false);
            }
            else
            {
                if (image.RawFormat.Equals(ImageFormat.Tiff) || image.RawFormat.Equals(ImageFormat.Bmp))
                    ConvertBitmap(image);
                else
                    m_imageRecord = Document.Images.LoadImage(LoadBitmap(image));
            }
            CheckPicSize(image);
            //Update image record in Blip
            UpdateBlipImageRecord();
        }
#endif
        /// <summary>
        /// Update Blip ImageRecord
        /// </summary>
        private void UpdateBlipImageRecord()
        {
            if (Document != null && !Document.IsOpening && Document.Escher != null && Document.Escher.Containers != null
                && Document.Escher.Containers.ContainsKey(this.ShapeId))
            {
                MsofbtSpContainer container = Document.Escher.Containers[this.ShapeId] as MsofbtSpContainer;
                if (container != null && container.Bse != null && container.Bse.Blip != null)
                    container.Bse.Blip.ImageRecord = m_imageRecord;
            }
        }
        /// <summary>
        /// Add Caption for current Picture
        /// </summary>
        /// <param name="captionPosition"></param>
        /// <param name="name"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public IWParagraph AddCaption(string name, CaptionNumberingFormat format,
          CaptionPosition captionPosition)
        {
            WTextBody body = (OwnerParagraph.Owner as WTextBody);
            WParagraph paragraph = null;
            if (body != null)
            {
                int index = body.Paragraphs.IndexOf(OwnerParagraph);
                paragraph = new WParagraph(Document);
                paragraph.AppendText(name + " ");
                name = name.Replace(" ", "_");
                WSeqField field = (WSeqField)paragraph.AppendField("Figure " + name, FieldType.FieldSequence);
                field.CaptionName = "Figure " + name;
                field.NumberFormat = format;

                int pictureIndex = OwnerParagraph.Items.IndexOf(this);

                // Set needed formatting and paragraph location dependently on captionPosition value
                if (captionPosition == CaptionPosition.AboveImage)
                {
                    paragraph.ParagraphFormat.KeepFollow = true;
                    int captionIndex = (pictureIndex == 0) ? index : index + 1;

                    body.Paragraphs.Insert(captionIndex, paragraph);

                    if (pictureIndex > 0)
                    {
                        OwnerParagraph.Items.RemoveAt(pictureIndex);
                        WParagraph newParagraph = new WParagraph(Document);
                        newParagraph.Items.Insert(0, this);
                        body.Paragraphs.Insert(captionIndex + 1, newParagraph);
                    }
                }
                else
                {
                    OwnerParagraph.ParagraphFormat.KeepFollow = true;
                    body.Paragraphs.Insert(index + 1, paragraph);
                }
            }

            return paragraph;
        }
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            if (m_imageRecord != null)
            {
                Size size = m_imageRecord.Size;
                ImageFormat imageFormat = m_imageRecord.ImageFormat;
                int length = m_imageRecord.Length;
                if (m_imageRecord.IsMetafile)
                    m_imageRecord = m_doc.Images.LoadMetaFileImage(m_imageRecord.m_imageBytes, true);
                else
                    m_imageRecord = m_doc.Images.LoadImage(m_imageRecord.ImageBytes);
                
                m_imageRecord.Size = size;
                m_imageRecord.ImageFormat = imageFormat;
                m_imageRecord.Length = length;
            }
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WPicture pic = (WPicture)base.CloneImpl();
            if (pic.ImageRecord == null)
            {
                return null;
            }
            pic.m_charFormat = new WCharacterFormat(Document);
            pic.m_charFormat.ImportContainer(m_charFormat);

            pic.m_inlinePictureShape = (InlineShapeObject)PictureShape.Clone();

            //Maintain the local copy of the imagerecord while cloning
            ImageRecord curImageRecord = new ImageRecord(Document, m_imageRecord);
            pic.m_imageRecord = curImageRecord;

            if (m_size.Width != float.MinValue && m_size.Height != float.MinValue)
            {
                pic.Size = m_size;
            }

            if (this.EmbedBody != null)
            {
                pic.EmbedBody = (WTextBody)this.EmbedBody.Clone();
            }

            pic.Cloned = true;

            return pic;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            if ((nextOwner.OwnerBase != null && nextOwner.OwnerBase is HeaderFooter) ||
                nextOwner is HeaderFooter)
                this.IsHeaderPicture = true;
            Size size = m_imageRecord.Size;
            ImageFormat imageFormat = m_imageRecord.ImageFormat;
            int length = m_imageRecord.Length;
            if (m_imageRecord.IsMetafile)
                m_imageRecord = doc.Images.LoadMetaFileImage(m_imageRecord.m_imageBytes, true);
            else
                m_imageRecord = doc.Images.LoadImage(m_imageRecord.ImageBytes);
            m_imageRecord.Size = size;
            m_imageRecord.ImageFormat = imageFormat;
            m_imageRecord.Length = length;
            Document.CloneShapeEscher(doc, this);
            PictureShape.CloneRelationsTo(doc, nextOwner);
            this.Cloned = false;

            if (EmbedBody != null)
            {
                EmbedBody.CloneRelationsTo(doc, nextOwner);
            }
        }
        #endregion

        #region Class overrides
        //#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            writer.WriteValue(PropertyNames.Type, (Enum)ParagraphItemType.Picture);
            writer.WriteValue(PropertyNames.Width, Size.Width);
            writer.WriteValue(PropertyNames.Height, Size.Height);
            writer.WriteValue(XDLSConstants.WidthScale, m_widthScale);
            writer.WriteValue(XDLSConstants.HeightScale, m_heightScale);
            writer.WriteValue(XDLSConstants.ImageIsMetafileAttr, ImageRecord.IsMetafile);
            #region commented
            //      if( m_brightness != 50 )
            //      {
            //        writer.WriteValue( XDLSConstants.PictBrightnessAttr, m_brightness );
            //      }
            //      if( m_contrast != 50 )
            //      {
            //        writer.WriteValue( XDLSConstants.PictContrastAttr, m_contrast );
            //      }
            //      if( m_color != PictureColor.Automatic )
            //      {
            //        writer.WriteValue( XDLSConstants.PictColorAttr, m_color );
            //      }
            //      if( m_cropLeft != 0 )
            //      {
            //        writer.WriteValue( XDLSConstants.CropFromLeft, m_cropLeft );
            //      }
            //      if( m_cropRight != 0 )
            //      {
            //        writer.WriteValue( XDLSConstants.CropFromRight, m_cropRight );
            //      }
            //      if( m_cropTop != 0 )
            //      {
            //        writer.WriteValue( XDLSConstants.CropFromTop, m_cropTop );
            //      }
            //      if( m_cropBottom != 0 )
            //      {
            //        writer.WriteValue( XDLSConstants.CropFromBottom, m_cropBottom );
            //      } 
            #endregion

            if (m_wrappingStyle != TextWrappingStyle.Inline)
            {
                writer.WriteValue(XDLSConstants.ShapeHorizOriginAttr, m_horizontalOrigin);

                writer.WriteValue(XDLSConstants.ShapeVertOriginAttr, m_verticalOrigin);

                writer.WriteValue(XDLSConstants.ShapeVertPositionAttr, m_vertPosition);

                writer.WriteValue(XDLSConstants.ShapeHorizPositionAttr, m_horizPosition);

                writer.WriteValue(XDLSConstants.ShapeWrappingStyleAttr, m_wrappingStyle);

                writer.WriteValue(XDLSConstants.ShapeWrappingTypeAttr, m_wrappingType);

                writer.WriteValue(XDLSConstants.ShapeIsBelowTextAttr, m_isBelowText);

                writer.WriteValue(XDLSConstants.ShapeHorizAlignAttr, m_horAlignment);

                writer.WriteValue(XDLSConstants.ShapeVertAlignAttr, m_vertAlignment);

                writer.WriteValue(XDLSConstants.ShapeIdentAttr, m_spid);

                if (m_isHeader)
                    writer.WriteValue(XDLSConstants.ShapeIsHeaderAttr, m_isHeader);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            m_size.Width = reader.ReadFloat(PropertyNames.Width);
            m_size.Height = reader.ReadFloat(PropertyNames.Height);
            m_widthScale = reader.ReadFloat(XDLSConstants.WidthScale);
            m_heightScale = reader.ReadFloat(XDLSConstants.HeightScale);

            #region commented
            //      if( reader.HasAttribute( XDLSConstants.PictBrightnessAttr ))
            //      {
            //        m_brightness = reader.ReadFloat( XDLSConstants.PictBrightnessAttr ); 
            //      }
            //      if( reader.HasAttribute( XDLSConstants.PictContrastAttr ))
            //      {
            //        m_contrast = reader.ReadFloat( XDLSConstants.PictContrastAttr );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.PictColorAttr ))
            //      {
            //        m_color = ( PictureColor )reader.ReadEnum( XDLSConstants.PictColorAttr, typeof( PictureColor ));
            //      }
            //      if( reader.HasAttribute( XDLSConstants.CropFromLeft ))
            //      {
            //        m_cropLeft = reader.ReadFloat( XDLSConstants.CropFromLeft );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.CropFromRight ))
            //      {
            //        m_cropRight = reader.ReadFloat( XDLSConstants.CropFromRight );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.CropFromTop ))
            //      {
            //        m_cropTop = reader.ReadFloat( XDLSConstants.CropFromTop );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.CropFromBottom ))
            //      {
            //        m_cropBottom = reader.ReadFloat( XDLSConstants.CropFromBottom );
            //      } 
            #endregion

            if (reader.HasAttribute(XDLSConstants.ShapeHorizOriginAttr))
            {
                m_horizontalOrigin =
                  (HorizontalOrigin)
                  reader.ReadEnum(XDLSConstants.ShapeHorizOriginAttr, typeof(HorizontalOrigin));
            }

            if (reader.HasAttribute(XDLSConstants.ShapeVertOriginAttr))
            {
                m_verticalOrigin =
                  (VerticalOrigin)
                  reader.ReadEnum(XDLSConstants.ShapeVertOriginAttr, typeof(VerticalOrigin));
            }

            if (reader.HasAttribute(XDLSConstants.ShapeVertPositionAttr))
            {
                m_vertPosition = reader.ReadFloat(XDLSConstants.ShapeVertPositionAttr);
            }

            if (reader.HasAttribute(XDLSConstants.ShapeHorizPositionAttr))
            {
                m_horizPosition = reader.ReadFloat(XDLSConstants.ShapeHorizPositionAttr);
            }

            if (reader.HasAttribute(XDLSConstants.ShapeWrappingStyleAttr))
            {
                m_wrappingStyle =
                  (TextWrappingStyle)
                  reader.ReadEnum(XDLSConstants.ShapeWrappingStyleAttr, typeof(TextWrappingStyle));
            }

            if (reader.HasAttribute(XDLSConstants.ShapeWrappingTypeAttr))
            {
                m_wrappingType =
                  (TextWrappingType)
                  reader.ReadEnum(XDLSConstants.ShapeWrappingTypeAttr, typeof(TextWrappingType));
            }

            if (reader.HasAttribute(XDLSConstants.ShapeIsBelowTextAttr))
            {
                m_isBelowText = reader.ReadBoolean(XDLSConstants.ShapeIsBelowTextAttr);
            }

            if (reader.HasAttribute(XDLSConstants.ShapeHorizAlignAttr))
            {
                m_horAlignment =
                  (ShapeHorizontalAlignment)
                  reader.ReadEnum(XDLSConstants.ShapeHorizAlignAttr, typeof(ShapeHorizontalAlignment));
            }

            if (reader.HasAttribute(XDLSConstants.ShapeVertAlignAttr))
            {
                m_vertAlignment =
                  (ShapeVerticalAlignment)
                  reader.ReadEnum(XDLSConstants.ShapeVertAlignAttr, typeof(ShapeVerticalAlignment));
            }

            if (reader.HasAttribute(XDLSConstants.ShapeIdentAttr))
            {
                m_spid = reader.ReadInt(XDLSConstants.ShapeIdentAttr);
            }

            if (reader.HasAttribute(XDLSConstants.ShapeIsHeaderAttr))
            {
                m_isHeader = reader.ReadBoolean(XDLSConstants.ShapeIsHeaderAttr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlContent(IXDLSContentWriter writer)
        {
            base.WriteXmlContent(writer);
            if (m_imageRecord != null)
                writer.WriteChildBinaryElement(XDLSConstants.ImageTag, m_imageRecord.ImageBytes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override bool ReadXmlContent(IXDLSContentReader reader)
        {
            base.ReadXmlContent(reader);
#if !SILVERLIGHT && !WP
            if (reader.TagName == XDLSConstants.ImageTag)
            {
                Image image = GetImage(reader.ReadChildBinaryElement());
                LoadImage(image);
                return true;
            }
#endif

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.CharacterFormatTag, m_charFormat);
            XDLSHolder.AddElement(XDLSConstants.ShapeFormatTag, m_inlinePictureShape);
        }
        //#endif
        /// <summary>
        /// 
        /// </summary>
        internal override void Close()
        {
            base.Close();

            if (m_imageRecord != null && !this.DeepDetached)
                m_imageRecord.OccurenceCount--;

            if (m_embedBody != null)
            {
                m_embedBody.Close();
                m_embedBody = null;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Detaches from owner.
        /// </summary>
        internal override void Detach()
        {
            if (m_imageRecord != null)
            {
                m_imageRecord.Detach();
            }
            if (Document != null && Document.Escher != null && Document.Escher.Containers != null
                && Document.Escher.Containers.ContainsKey(this.ShapeId))
            {
                MsofbtSpContainer container = Document.Escher.Containers[this.ShapeId] as MsofbtSpContainer;
                if (container != null && container.Bse != null)
                {
                    container.Bse.Blip = null;
                    container.Bse = null;
                }
            }
        }
        internal override void Attach(WParagraph owner, int itemPos)
        {
            if (m_imageRecord != null)
            {
                m_imageRecord.Attach();
            }
            base.Attach(owner, itemPos);
        }
        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        /// <param name="isMetafile">if it specifies a metafile, set to <c>true</c>.</param>
        internal void LoadImage(byte[] imageBytes, bool isMetafile)
        {
            if (imageBytes == null)
            {
                throw new ArgumentNullException("image");
            }
            if (isMetafile)
                m_imageRecord = Document.Images.LoadMetaFileImage(imageBytes, false);
            else
                m_imageRecord = Document.Images.LoadImage(imageBytes);
            imageBytes = null;
            m_size = new SizeF(float.MinValue, float.MinValue);
        }
        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="imageRecord">The image record.</param>
        internal void LoadImage(ImageRecord imageRecord)
        {
            m_imageRecord = imageRecord;
            m_size = new SizeF(imageRecord.Size.Width, imageRecord.Size.Height);
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
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaFile"></param>
        internal static byte[] LoadMetafile(Metafile metaFile)
        {
            System.Drawing.Rectangle rect = metaFile.GetMetafileHeader().Bounds;
            Bitmap bitmap = null;
            try
            {
                bitmap = new Bitmap(rect.Width, rect.Height, metaFile.PixelFormat);
            }
            catch
            {
                throw new ArgumentException("Ivalid metafile format ");
            }
            Graphics graphics1 = Graphics.FromImage(bitmap);
            IntPtr ptr = graphics1.GetHdc();
            MemoryStream stream = new MemoryStream();

            Metafile metafile = new Metafile(stream, ptr, EmfType.EmfPlusOnly);
            graphics1.ReleaseHdc(ptr);
            Graphics graphics2 = Graphics.FromImage(metafile);
            graphics2.DrawImageUnscaled(metaFile, rect);
            graphics2.Dispose();
            metafile.Dispose();
            byte[] imageBytes = stream.ToArray();
            stream.Close();

            return imageBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        internal static byte[] LoadBitmap(Image image)
        {
            using (MemoryStream imageStream = new MemoryStream())
            {
                try
                {
                    if (image.RawFormat.Equals(ImageFormat.Tiff) || image.RawFormat.Equals(ImageFormat.Bmp))
                        image.Save(imageStream, ImageFormat.Png);
                    else
                        image.Save(imageStream, image.RawFormat);
                }
                catch
                {
                    image.Save(imageStream, ImageFormat.Png);
                }
                return imageStream.ToArray();
            }
        }

        /// <summary>
        /// Converts the .tiff format to .png. Tiff format is not supported by Word 2000. 
        /// </summary>
        /// <param name="image">The image.</param>
        private void ConvertBitmap(Image image)
        {
            if (m_imageRecord != null)
            {
                m_imageRecord.OccurenceCount--;
                m_imageRecord = null;
            }
            m_imageRecord = Document.Images.LoadImage(LoadBitmap(image));
        }
#endif
        /// <summary>
        /// Resets the data.
        /// </summary>
        private void ResetImageData()
        {
            //Handled to preserve borders defined.
            if (m_inlinePictureShape.ShapeContainer != null
                && m_inlinePictureShape.ShapeContainer.Shape != null)
                m_inlinePictureShape.ShapeContainer = null;
            if (m_imageRecord != null)
            {
                m_imageRecord.OccurenceCount--;
                m_imageRecord = null;
            }
            m_size = new SizeF(float.MinValue, float.MinValue);
        }
        #endregion
#if !SILVERLIGHT && !WP

        #region IWidget/ILeafWidget implement
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            dc.DrawPicture(this, ltWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        SizeF ILeafWidget.Measure(DrawingContext dc)
        {
            return dc.MeasureImage(this);
        }
        #endregion

        #region WidgetBase overrides
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Horizontal);

            if ((this.PreviousSibling is WFieldMark
                && (this.PreviousSibling as WFieldMark).Type == FieldMarkType.FieldSeparator
                && !(this.PreviousSibling.PreviousSibling != null
                && (this.PreviousSibling.PreviousSibling is WField)
                && ((this.PreviousSibling.PreviousSibling as WField).FieldType == FieldType.FieldIncludePicture
                || (this.PreviousSibling.PreviousSibling as WField).FieldType == FieldType.FieldHyperlink)))
                || this.PreviousSibling is WOleObject)
                m_layoutInfo.IsSkip = true;
            WParagraph ownerParagraph = this.OwnerParagraph;
            if (this.Owner is SDTInlineContent)
                ownerParagraph = GetOwnerParagraph();
            if ((ownerParagraph.IsInCell && ((ownerParagraph as IWidget).LayoutInfo.IsClipped)) || (ownerParagraph.OwnerTextBody.Owner is Shape))
                m_layoutInfo.IsClipped = true;
            m_layoutInfo.IsVerticalText = (ownerParagraph as IWidget).LayoutInfo.IsVerticalText;
            if (this.TextWrappingStyle != TextWrappingStyle.Inline)
                m_layoutInfo.IsSkipBottomAlign = true;
            if (this.ParaItemCharFormat.HasValue(WCharacterFormat.HiddenKey))
                m_layoutInfo.IsSkip = true;
        }
        #endregion
#endif

        #region Class utility methods
#if SILVERLIGHT || WP
        /// <summary>
        /// Converts size of the image to point units.
        /// </summary>
        /// <param name="image">Image object.</param>
        /// <returns>Size of the image int points.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal SizeF ConvertSize(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            m_imageRecord.Size = image.Size;
            m_imageRecord.ImageFormat = image.RawFormat;

            UnitsConvertor convertor = UnitsConvertor.Instance;
            SizeF result = convertor.ConvertFromPixels(image.Size, PrintUnits.Point);           

            return result;
        }
#else
        /// <summary>
        /// Converts size of the image to point units.
        /// </summary>
        /// <param name="image">Image object.</param>
        /// <returns>Size of the image int points.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal SizeF ConvertSize(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            SizeF result = image.Size;
            m_imageRecord.Size = image.Size;
            m_imageRecord.ImageFormat = image.RawFormat;
            // NOTE: For PixelFormat.Format8bppIndexed Graphics 
            // can't created from image object!
            if (image.PixelFormat == PixelFormat.Format8bppIndexed
              || image.PixelFormat == PixelFormat.Format4bppIndexed
              || image.PixelFormat == PixelFormat.Format1bppIndexed
              || (image is Metafile && image.PixelFormat == PixelFormat.Format32bppRgb)
              || !Enum.IsDefined(typeof(PixelFormat), image.PixelFormat)) //Checks for unsupported pixel formats
            {
                UnitsConvertor convertor = UnitsConvertor.Instance;
                result = convertor.ConvertFromPixels(image.Size, PrintUnits.Point, image.HorizontalResolution);
            }
            else
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    UnitsConvertor convertor = new UnitsConvertor(g);
                    result = convertor.ConvertFromPixels(image.Size, PrintUnits.Point);
                }
            }

            return result;
        }
#endif
        /// <summary>
        /// Checks the size of the picture.
        /// </summary>
        private void CheckPicSize(Image image)
        {
            if (image != null)
                m_size = ConvertSize(image);
        }
        #endregion
    }
}
