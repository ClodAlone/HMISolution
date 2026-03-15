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
  /// Represents a line shape.
  /// </summary>
  /// <remarks>Supported by Essential PDF only.</remarks> 
  public class LineShape
    : Shape,
    IWidget
	{
    #region Class members
    /// <summary>
    /// Start point of the line.
    /// </summary>
    private PointF m_start;
    /// <summary>
    /// End point of the line.
    /// </summary>
    private PointF m_end;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets start location of the line.
    /// </summary>
    public PointF Start
    {
      get
      {
        return m_start;
      }
      set
      {
        if( m_start != value )
        {
          m_start = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets end location of the line.
    /// </summary>
    public PointF End
    {
      get
      {
        return m_end;
      }
      set
      {
        if( m_end != value )
        {
          m_end = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets start X coordinate of the line.
    /// </summary>
    public float X1
    {
      get
      {
        return m_start.X;
      }
      set
      {
        m_start.X = value;
      }
    }
    /// <summary>
    /// Gets / sets start Y coordinate of the line.
    /// </summary>
    public float Y1
    {
      get
      {
        return m_start.Y;
      }
      set
      {
        m_start.Y = value;
      }
    }
    /// <summary>
    /// Gets / sets end X coordinate of the line.
    /// </summary>
    public float X2
    {
      get
      {
        return m_end.X;
      }
      set
      {
        m_end.X = value;
      }
    }
    /// <summary>
    /// Gets / sets end Y coordinate of the line.
    /// </summary>
    public float Y2
    {
      get
      {
        return m_end.Y;
      }
      set
      {
        m_end.Y = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="canvas"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public LineShape( Canvas canvas )
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
      base.WriteXmlAttributes( writer );
      
      writer.WriteValue( PropertyNames.Type, ShapeType.Line );
      writer.WriteValue( PropertyNames.X1, this.X1 );
      writer.WriteValue( PropertyNames.Y1, this.Y1 );
      writer.WriteValue( PropertyNames.X2, this.X2 );
      writer.WriteValue( PropertyNames.Y2, this.Y2 );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
      
      this.X1 = reader.ReadFloat( PropertyNames.X1 );
      this.Y1 = reader.ReadFloat( PropertyNames.Y1 );
      this.X2 = reader.ReadFloat( PropertyNames.X2 );
      this.Y2 = reader.ReadFloat( PropertyNames.Y2 );
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
      ( cg as DLSGraphics ).DrawLineShape( this, ltWidget );
      ResetTransform( cg );
    }
    #endregion
	}
}
