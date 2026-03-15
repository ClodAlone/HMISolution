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
using System.Drawing;
using System.Drawing.Imaging;

using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
using System.IO;
#endregion  

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents an image.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class Picture
    : ParagraphItem,
      IPicture,
      ILeafWidget
  {
    #region Class members
    /// <summary>
    /// Size of the picture.
    /// </summary>
    private SizeF m_size;
    /// <summary>
    ///  in percent
    /// </summary>
    private float m_widthScale = 100;
    /// <summary>
    ///  in percent
    /// </summary>
    private float m_heightScale = 100;
    /// <summary>
    /// 
    /// </summary>
    private Image m_image = null;
    /// <summary>
    /// Image byte array.
    /// </summary>
    private byte[] m_imageBytes;
    /// <summary>
    /// 
    /// </summary>
    private bool m_isMetafile = false;
//    /// <summary>
//    /// 
//    /// </summary>
//    private float m_brightness = 50;
//    private float m_contrast = 50;
//    private PictureColor m_color;
//    private float m_cropLeft;
//    private float m_cropRight;
//    private float m_cropTop;
//    private float m_cropBottom;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets size of the picture object.
    /// </summary>
    public SizeF Size
    {
      get
      {
        return m_size;
      }
    }
    /// <summary>
    /// Gets / sets picture height.
    /// </summary>
    public float Height
    {
      get
      {
        return m_size.Height;
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
        return m_size.Width;
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
        if( value <= 0 )
        {
          throw  new ArgumentOutOfRangeException( "Scale factor must be greater than 0" );
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
        if( value <= 0 )
        {
          throw  new ArgumentOutOfRangeException( "Scale factor must be greater than 0" );
        }
        m_widthScale = value;
      }
    }
    /// <summary>
    /// Gets internal System.Drawing.Image object.
    /// </summary>
    public Image Image
    {
      get
      {
        return ( m_image == null ) ? GetImage() : m_image;
      }
    }
    /// <summary>
    /// Gets image byte array.
    /// </summary>
    public byte[] ImageBytes
    {
      get
      {
        return m_imageBytes;
      }
    }
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

    #region Class initialize/finalize methods
    /// <summary>
    /// Initialize constructor.
    /// </summary>
    /// <param name="doc"></param>
    public Picture( IDocument doc )
      : base( doc )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pic"></param>
    /// <param name="paragraph"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal Picture( Picture pic, IParagraph paragraph )
     : this( ( paragraph as Paragraph ).Document )
    {
      if( pic == null )
        throw new ArgumentNullException( "pic" );
      
      SetOwnerParagraph( paragraph as Paragraph, pic.StartIndex );
      byte[] imageBytes = new byte[ pic.ImageBytes.Length ];
      pic.ImageBytes.CopyTo( imageBytes, 0 );
      LoadImage( imageBytes );
      Height = pic.Height;
      Width = pic.Width;
      HeightScale = pic.HeightScale;
      WidthScale = pic.WidthScale;
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Loads image.
    /// </summary>
    public void LoadImage( Image image )
    {
      if( image == null )
      {
        throw new ArgumentNullException( "image" );
      }
      m_image = image;
      m_size = ImageShape.ConvertSize( m_image );

      if( image is Metafile )
      {
       LoadMetafile( image as Metafile );
      }
      else
      {
        LoadBitmap( image );
      }      
    }
    /// <summary>
    /// Loads image as bytes array.
    /// </summary>
    /// <param name="imageBytes"></param>
    public void LoadImage( byte[] imageBytes )
    {
      m_image = null;
      if( imageBytes == null )
      {
        throw new ArgumentNullException( "image" );
      }
      m_imageBytes = imageBytes;
      m_image = GetImage();    
    }
    /// <summary>
    /// Clones itself.
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    public override IParagraphItem Clone( IParagraph paragraph )
    {
      return new Picture( this, paragraph );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="metaFile"></param>
    private void LoadMetafile( Metafile metaFile )
    {
      Rectangle rect = metaFile.GetMetafileHeader().Bounds;
      Bitmap bitmap = null;
      try
      {
        bitmap = new Bitmap( rect.Width, rect.Height, metaFile.PixelFormat );
      }
      catch
      {
        throw new ArgumentException( "Ivalid metafile format ");
      }
      Graphics graphics1 = Graphics.FromImage( bitmap );
      IntPtr ptr = graphics1.GetHdc();
      MemoryStream stream = new MemoryStream();
      
      Metafile metafile = new Metafile( stream, ptr, EmfType.EmfOnly );
      graphics1.ReleaseHdc( ptr );
      Graphics graphics2 = Graphics.FromImage( metafile );
      graphics2.DrawImageUnscaled( metaFile, rect );
      graphics2.Dispose();
      metafile.Dispose();
      m_imageBytes = stream.ToArray();
      stream.Close(); 
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    private void LoadBitmap( Image image )
    {
      using( MemoryStream imageStream = new MemoryStream() )
      {
        try
        {
          image.Save( imageStream, image.RawFormat );
        }
        catch
        {
          image.Save( imageStream, ImageFormat.Png );
        }
        m_imageBytes = imageStream.ToArray();  
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Image GetImage()
    {
      Image tempImage = null;
      if( m_imageBytes != null )
      {
        MemoryStream imageStream = new MemoryStream( m_imageBytes );
        try
        {
          tempImage = Image.FromStream( imageStream );
        }
        catch
        {
          throw new ArgumentException( "Argument is not image byte array");
        }
//        finally
//        {
//         // imageStream.Close();
//        }
      }
      return tempImage;
    }
    #endregion

    #region IXDLSSerializable implement
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.SkipID = true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );

      writer.WriteValue( PropertyNames.Type, ( Enum )ParagraphItemType.Picture );
      writer.WriteValue( PropertyNames.Width, Width );
      writer.WriteValue( PropertyNames.Height, Height );
      writer.WriteValue( XDLSConstants.WidthScale, WidthScale );
      writer.WriteValue( XDLSConstants.HeightScale, HeightScale );
      writer.WriteValue( XDLSConstants.ImageIsMetafileAttr, Image is Metafile );
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
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlContent( IXDLSContentWriter writer )
    {
      base.WriteXmlContent( writer );
      if( m_imageBytes != null )
      {
        writer.WriteChildBinaryElement( XDLSConstants.ImageTag, m_imageBytes );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override bool ReadXmlContent( IXDLSContentReader reader )
    {
      base.ReadXmlContent( reader );

      if( reader.TagName == XDLSConstants.ImageTag )
      {
        m_imageBytes = reader.ReadChildBinaryElement();
        return true;
      }

      return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );

      Width = reader.ReadFloat( PropertyNames.Width );
      Height = reader.ReadFloat( PropertyNames.Height );
      WidthScale = reader.ReadFloat( XDLSConstants.WidthScale );
      HeightScale = reader.ReadFloat( XDLSConstants.HeightScale );
      if( reader.HasAttribute( XDLSConstants.ImageIsMetafileAttr ))
      {
        m_isMetafile = reader.ReadBoolean( XDLSConstants.ImageIsMetafileAttr );
      }
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

    }
    #endregion

    #region IWidget/ILeafWidget implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    void IWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      ( cg as DLSGraphics ).DrawPicture( this, ltWidget );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    SizeF ILeafWidget.Measure( CustomGraphics cg )
    {
      return ( cg as DLSGraphics ).MeasurePicture( this );
    }
    #endregion

    #region WidgetBase overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void CreateLayoutInfo()
    {
      bool isInSection = ( OwnerParagraph != null ) && ( OwnerParagraph.Owner is ISection );
      m_layoutInfo = new LayoutInfo( ChildrenLayoutDirection.Horizontal );
      m_layoutInfo.IsClippedVertical = !isInSection;
      m_layoutInfo.IsClippedHorizontal = true;
    }
    #endregion
  }
}
