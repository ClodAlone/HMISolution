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
using System.Drawing;
using System;

using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents an ellipse shape.
  /// </summary>
  /// <remarks>Supported by Essential PDF only.</remarks> 
  public class EllipseShape
    : Shape,
    IWidget
	{
    #region Class members
    /// <summary>
    /// Defines the boundaries of the ellipse.
    /// </summary>
    private RectangleF m_bounds;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets the boundaries of the ellipse.
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
    /// Gets / sets location of the ellipse's boundaries.
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
    /// Gets / sets size of the ellipse's boundaries.
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
    /// Gets / sets width of the boundaries of the shape.
    /// </summary>
    public float Width
    {
      get
      {
        return m_bounds.Width;
      }
      set
      {
        if( m_bounds.Width != value )
        {
          m_bounds.Width = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets width of the boundaries of the shape.
    /// </summary>
    public float Height
    {
      get
      {
        return m_bounds.Height;
      }
      set
      {
        if( m_bounds.Height != value )
        {
          m_bounds.Height = value;
        }
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates a new ellipse shape.
    /// </summary>
    /// <param name="canvas"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public EllipseShape( Canvas canvas )
      : base( canvas )
    {
    }
    #endregion

    #region XML serialization overrides
    /// <summary>
    /// Serialize XML Attributes.
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      base.WriteXmlAttributes( writer );
      
      writer.WriteValue( PropertyNames.Type, ShapeType.Ellipse );
      writer.WriteValue( PropertyNames.X, this.Bounds.X );
      writer.WriteValue( PropertyNames.Y, this.Bounds.Y );
      writer.WriteValue( PropertyNames.Width, this.Bounds.Width );
      writer.WriteValue( PropertyNames.Height, this.Bounds.Height );
    }
    /// <summary>
    /// Read XML Attributes.
    /// </summary>
    /// <param name="reader"></param>
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      base.ReadXmlAttributes( reader );
      
      RectangleF bounds = RectangleF.Empty;
      bounds.X = reader.ReadFloat( PropertyNames.X );
      bounds.Y = reader.ReadFloat( PropertyNames.Y );
      bounds.Width  = reader.ReadFloat( PropertyNames.Width );
      bounds.Height = reader.ReadFloat( PropertyNames.Height );

      this.Bounds = bounds;
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
      ( cg as DLSGraphics ).DrawEllipseShape( this, ltWidget );
      ResetTransform( cg );
    }
    #endregion
	}
}
