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
using System.Drawing.Imaging;

using Syncfusion;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;

using System.Drawing;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents an image shape.
  /// </summary>
  /// <remarks>Supported by Essential PDF only.</remarks> 
  public class ImageShape 
    : Shape,
      IWidget
  {
    #region Class members
    /// <summary>
    /// Boundaries of the image.
    /// </summary>
    private RectangleF m_bounds;
    /// <summary>
    /// 
    /// </summary>
    private Image m_image;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets boundaries of the image.
    /// </summary>
    public RectangleF Bounds
    {
      get
      {
        return m_bounds;
      }
      set
      {
        if( m_bounds != value )
        {
          m_bounds = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets location of the image.
    /// </summary>
    public PointF Location
    {
      get
      {
        return m_bounds.Location;
      }
      set
      {
        if( m_bounds.Location != value )
        {
          m_bounds.Location = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets size of the image.
    /// </summary>
    public SizeF Size
    {
      get
      {
        return m_bounds.Size;
      }
      set
      {
        if( m_bounds.Size != value )
        {
          m_bounds.Size = value;
        }
      }
    }
    /// <summary>
    /// Gets x coordinate of the shape.
    /// </summary>
    public float X
    {
      get
      {
        return m_bounds.X;
      }
      set
      {
        if( m_bounds.X != value )
        {
          m_bounds.X = value;
        }
      }
    }
    /// <summary>
    /// Gets y coordinate of the shape.
    /// </summary>
    public float Y
    {
      get
      {
        return m_bounds.Y;
      }
      set
      {
        if( m_bounds.Y != value )
        {
          m_bounds.Y = value;
        }
      }
    }
    /// <summary>
    /// Gets / Sets the width of the image.
    /// </summary>
    public float Width
    {
      get
      {
        return m_bounds.Width;
      }
      set
      {
        m_bounds.Width = value;
      }
    }
    /// <summary>
    /// Gets / Sets the height of the image.
    /// </summary>
    public float Height
    {
      get
      {
        return m_bounds.Height;
      }
      set
      {
        m_bounds.Height = value;
      }
    }
    /// <summary>
    /// Represents the Image.
    /// </summary>
    public Image Image
    {
      get
      {
        return m_image;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "Image" );

        if( m_image != value )
        {
          // NOTE: we have to clone image because user can dispose
          // image before saving of the document.
          m_image = ( value.Clone() as Image );

          this.Size = ConvertSize( m_image );
        }
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Create a new Image Shape.
    /// </summary>
    /// <param name="canvas"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public ImageShape( Canvas canvas )
      : base( canvas )
    {
    }
    #endregion

    #region XML serialization overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      base.WriteXmlAttributes( writer );
      
      writer.WriteValue( PropertyNames.Type, ShapeType.Image );
      writer.WriteValue( PropertyNames.X, this.Location.X );
      writer.WriteValue( PropertyNames.Y, this.Location.Y );
      writer.WriteValue( PropertyNames.Width, this.Width );
      writer.WriteValue( PropertyNames.Height, this.Height );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      base.ReadXmlAttributes( reader );
      
      m_bounds.X = reader.ReadFloat( PropertyNames.X );
      m_bounds.Y = reader.ReadFloat( PropertyNames.Y );
      
      this.Width  = reader.ReadFloat( PropertyNames.Width );
      this.Height = reader.ReadFloat( PropertyNames.Height );
    }
    /// <summary>
    /// Overloaded. Writes image to XML.
    /// </summary>
    /// <param name="writer">Writer object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlContent( IXDLSContentWriter writer )
    {
      base.WriteXmlContent( writer );

      writer.WriteImage( this.Image );
    }
    /// <summary>
    /// Overloaded. Reads image from XML.
    /// </summary>
    /// <param name="reader">Reader object.</param>
    /// <returns>True always.</returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override bool ReadXmlContent( IXDLSContentReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      base.ReadXmlContent( reader );

      m_image = reader.ReadImage( false );

      return true;
    }
    #endregion

    #region WidgetBase override
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    void IWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      ApplyTransform( cg );
      ( cg as DLSGraphics ).DrawImageShape( this, ltWidget );
      ResetTransform( cg );
    }
    #endregion

    #region Class utility methods
    /// <summary>
    /// Converts size of the image to point units.
    /// </summary>
    /// <param name="image">Image object.</param>
    /// <returns>Size of the image int points.</returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    internal static SizeF ConvertSize( Image image )
    {
      if( image == null )
        throw new ArgumentNullException( "image" );

      SizeF result = image.Size;

      // NOTE: For PixelFormat.Format8bppIndexed Graphics 
      // cann't created from image object!
      if( image.PixelFormat == PixelFormat.Format8bppIndexed 
          || image.PixelFormat == PixelFormat.Format4bppIndexed 
          || image.PixelFormat == PixelFormat.Format1bppIndexed )
      {
        UnitsConvertor convertor = new UnitsConvertor();
        result = convertor.ConvertFromPixels( image.Size, PrintUnits.Point );
      }
      else if( image is Metafile && image.PixelFormat == PixelFormat.Format32bppRgb )
      {
        UnitsConvertor convertor = new UnitsConvertor();
        result = convertor.ConvertFromPixels( image.Size, PrintUnits.Point );
      }
      else
      {
        using( Graphics g = Graphics.FromImage( image ) )
        {
          UnitsConvertor convertor = new UnitsConvertor( g );
          result = convertor.ConvertFromPixels( image.Size, PrintUnits.Point );
        }
      }

      return result;
    }
    #endregion
  }
}