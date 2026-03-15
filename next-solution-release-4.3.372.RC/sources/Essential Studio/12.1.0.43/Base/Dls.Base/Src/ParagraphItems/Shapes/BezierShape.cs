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
  /// Represents a bezier shape.
  /// </summary>
  /// <remarks>Supported by Essential PDF only.</remarks> 
  public class BezierShape
    : Shape
    , IWidget
	{
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private float m_x1;
    /// <summary>
    /// 
    /// </summary>
    private float m_y1;
    /// <summary>
    /// 
    /// </summary>
    private float m_x2;
    /// <summary>
    /// 
    /// </summary>
    private float m_y2;
    /// <summary>
    /// 
    /// </summary>
    private float m_x3;
    /// <summary>
    /// 
    /// </summary>
    private float m_y3;
    /// <summary>
    /// 
    /// </summary>
    private float m_x4;
    /// <summary>
    /// 
    /// </summary>
    private float m_y4;
    #endregion

    #region Class properties
    /// <summary>
    /// The x-coordinate of the starting point of the curve.
    /// </summary>
    public float X1
    {
      get
      {
        return m_x1;
      }
      set
      {
        m_x1 = value;
      }
    }
    /// <summary>
    /// The y-coordinate of the starting point of the curve. 
    /// </summary>
    public float Y1
    {
      get
      {
        return m_y1;
      }
      set
      {
        m_y1 = value;
      }
    }
    /// <summary>
    /// The x-coordinate of the first control point of the curve.
    /// </summary>
    public float X2
    {
      get
      {
        return m_x2;
      }
      set
      {
        m_x2 = value;
      }
    }
    /// <summary>
    /// The y-coordinate of the first control point of the curve.
    /// </summary>
    public float Y2
    {
      get
      {
        return m_y2;
      }
      set
      {
        m_y2 = value;
      }
    }
    /// <summary>
    /// The x-coordinate of the second control point of the curve.
    /// </summary>
    public float X3
    {
      get
      {
        return m_x3;
      }
      set
      {
        m_x3 = value;
      }
    }
    /// <summary>
    /// The y-coordinate of the second control point of the curve.
    /// </summary>
    public float Y3
    {
      get
      {
        return m_y3;
      }
      set
      {
        m_y3 = value;
      }
    }
    /// <summary>
    /// The x-coordinate of the ending point of the curve.
    /// </summary>
    public float X4
    {
      get
      {
        return m_x4;
      }
      set
      {
        m_x4 = value;
      }
    }
    /// <summary>
    /// The y-coordinate of the ending point of the curve.
    /// </summary>
    public float Y4
    {
      get
      {
        return m_y4;
      }
      set
      {
        m_y4 = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Create a new Bezier Shape.
    /// </summary>
    /// <param name="canvas"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public BezierShape( Canvas canvas )
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
      
      writer.WriteValue( PropertyNames.Type, ShapeType.Bezier );
      writer.WriteValue( PropertyNames.X1, this.X1 );
      writer.WriteValue( PropertyNames.Y1, this.Y1 );
      writer.WriteValue( PropertyNames.X2, this.X2 );
      writer.WriteValue( PropertyNames.Y2, this.Y2 );
      writer.WriteValue( PropertyNames.X3, this.X3 );
      writer.WriteValue( PropertyNames.Y3, this.Y3 );
      writer.WriteValue( PropertyNames.X4, this.X4 );
      writer.WriteValue( PropertyNames.Y4, this.Y4 );
    }
    /// <summary>
    /// Read XML Attributes.
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      base.ReadXmlAttributes( reader );
      
      this.X1 = reader.ReadFloat( PropertyNames.X1 );
      this.Y1 = reader.ReadFloat( PropertyNames.Y1 );
      this.X2 = reader.ReadFloat( PropertyNames.X2 );
      this.Y2 = reader.ReadFloat( PropertyNames.Y2 );
      this.X3 = reader.ReadFloat( PropertyNames.X3 );
      this.Y3 = reader.ReadFloat( PropertyNames.Y3 );
      this.X4 = reader.ReadFloat( PropertyNames.X4 );
      this.Y4 = reader.ReadFloat( PropertyNames.Y4 );
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
      ( cg as DLSGraphics ).DrawBezierShape( this, ltWidget );
      ResetTransform( cg );
    }
    #endregion
	}
}
