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
  /// Represents a rectangle shape.
  /// </summary>
  /// <remarks>Supported by Essential PDF only.</remarks> 
  public class RectangleShape 
    : Shape,
      IWidget
  {
    #region Class members
    /// <summary>
    /// Boundaries for the shape.
    /// </summary>
    private RectangleF m_bounds;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets boundaries of the rectangle.
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
    /// Gets / sets location of the rectangle.
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
    /// Gets / sets size of the rectangle.
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
    /// Gets / sets width of the rectangle.
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
    /// Gets / sets height of the rectangle.
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
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="canvas"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public RectangleShape( Canvas canvas )
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
      
      writer.WriteValue( PropertyNames.Type, ShapeType.Rectangle );
      
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
      ( cg as DLSGraphics ).DrawRectangleShape( this, ltWidget );
      ResetTransform( cg );
    }
    #endregion
  }
}