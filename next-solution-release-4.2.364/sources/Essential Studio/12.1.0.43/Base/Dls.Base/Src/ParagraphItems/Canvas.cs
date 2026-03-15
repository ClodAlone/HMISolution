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

using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents the canvas that contains shapes.
  /// </summary>
  /// <remarks>Supported by Essential PDF only.</remarks> 
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class Canvas
    : ParagraphItem,
      ICanvas,
      ISplitLeafWidget
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private ShapeCollection m_shapes = null;
    /// <summary>
    /// 
    /// </summary>
    private Color m_borderColor = Color.Empty;
    /// <summary>
    /// Size of the canvas.
    /// </summary>
    private SizeF m_size;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets border color
    /// </summary>
    public Color BorderColor
    {
      get
      {
        return m_borderColor;
      }
      set
      {
        m_borderColor = value;        
      }
    }
    /// <summary>
    /// Gets / sets canvas width
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
    /// Gets / sets canvas height
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
    /// Gets / sets size of the canvas.
    /// </summary>
    public SizeF Size
    {
      get
      {
        return m_size;
      }
      set
      {
        if( m_size != value )
        {
          m_size = value;
        }
      }
    }
    /// <summary>
    /// Gets canvas shapes collection.
    /// </summary>
    public ShapeCollection Shapes
    {
      get
      {
        return m_shapes;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="doc"></param>
    public Canvas( Document doc )
      : base( doc )
    {
      m_shapes = new ShapeCollection( this );
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds a new shape to the canvas.
    /// </summary>
    /// <param name="shapeType"></param>
    /// <returns></returns>
    public Shape AddShape( ShapeType shapeType )
    {
      Shape shape = Document.CreateShape( shapeType, this );

      if( shape != null )
      {
        m_shapes.Add( shape );
      }

      return shape;
    }
    /// <summary>
    /// Creates Arc shape and adds it to collection.
    /// </summary>
    /// <returns> Created Arc shape.</returns>
    /// <remarks>Coordinates of this shape needs to be set to correctly
    /// display this shape on the canvas.</remarks>
    public ArcShape AddArc()
    {
      return ( ArcShape )AddShape( Syncfusion.DLS.ShapeType.Arc );
    }
    /// <summary>
    /// Creates Bezier curve shape and adds it to collection.
    /// </summary>
    /// <returns> Created Bezier curve shape.</returns>
		/// <remarks>Coordinates of this shape needs to be set to correctly
		/// display this shape on the canvas.</remarks>
    public BezierShape AddBezier()
    {
      return ( BezierShape )AddShape( Syncfusion.DLS.ShapeType.Bezier );
    }
    /// <summary>
    /// Creates Ellipse shape and adds it to collection.
    /// </summary>
    /// <returns> Created Ellipse shape.</returns>
		/// <remarks>Coordinates of this shape needs to be set to correctly
		/// display this shape on the canvas.</remarks>
    public EllipseShape AddEllipse()
    {
      return ( EllipseShape )AddShape( Syncfusion.DLS.ShapeType.Ellipse );
    }
    /// <summary>
    /// Creates Image shape and adds it to collection.
    /// </summary>
    /// <param name="image">Image object.</param>
    /// <returns> Created Image shape.</returns>
		/// <remarks>Coordinates of this shape needs to be set to correctly
		/// display this shape on the canvas.</remarks>
    public ImageShape AddImage( Image image )
    {
      if( image == null )
        throw new ArgumentNullException( "image" );

      ImageShape imgShape = AddShape( Syncfusion.DLS.ShapeType.Image ) as ImageShape;
      imgShape.Image = image;

      return imgShape;
    }
    /// <summary>
    /// Creates Line shape and adds it to collection.
    /// </summary>
    /// <returns> Created Line shape.</returns>
		/// <remarks>Coordinates of this shape needs to be set to correctly
		/// display this shape on the canvas.</remarks>
    public LineShape AddLine()
    {
      return ( LineShape )AddShape( Syncfusion.DLS.ShapeType.Line );
    }
    /// <summary>
    /// Creates Path shape and adds it to collection.
    /// </summary>
    /// <returns> Created Path shape.</returns>
		/// <remarks>Coordinates of this shape needs to be set to correctly
		/// display this shape on the canvas.</remarks>
    public PathShape AddPath()
    {
      return ( PathShape )AddShape( Syncfusion.DLS.ShapeType.Path );
    }
    /// <summary>
    /// Creates Pie shape and adds it to collection.
    /// </summary>
    /// <returns> Created Pie shape.</returns>
		/// <remarks>Coordinates of this shape needs to be set to correctly
		/// display this shape on the canvas.</remarks>
    public PieShape AddPie()
    {
      return ( PieShape )AddShape( Syncfusion.DLS.ShapeType.Pie );
    }
    /// <summary>
    /// Creates Polygon shape and adds it to collection.
    /// </summary>
    /// <returns> Created Polygon shape.</returns>
		/// <remarks>Coordinates of this shape needs to be set to correctly
		/// display this shape on the canvas.</remarks>
    public PolygonShape AddPolygon()
    {
      return ( PolygonShape )AddShape( Syncfusion.DLS.ShapeType.Polygon );
    }
    /// <summary>
    /// Creates Rectangle shape and adds it to collection.
    /// </summary>
    /// <returns> Created Rectangle shape.</returns>
		/// <remarks>Coordinates of this shape needs to be set to correctly
		/// display this shape on the canvas.</remarks>
    public RectangleShape AddRectangle()
    {
      return ( RectangleShape )AddShape( Syncfusion.DLS.ShapeType.Rectangle );
    }
    /// <summary>
    /// Creates Text shape and adds it to collection.
    /// </summary>
    /// <param name="text">Text data of the shape.</param>
    /// <returns> Created Text shape.</returns>
		/// <remarks>Coordinates of this shape needs to be set to correctly
		/// display this shape on the canvas.</remarks>
    public TextShape AddText( string text )
    {
      if( text == null )
        throw new ArgumentNullException( "text" );

      TextShape txtShape = AddShape( Syncfusion.DLS.ShapeType.Text ) as TextShape;
      txtShape.Text = text;

      return txtShape;
    }
    /// <summary>
    /// Clones itself.
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    public override IParagraphItem Clone( IParagraph paragraph )
    {
      throw new NotImplementedException();
    }
    #endregion

    #region IXDLSSerializable implement
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddElement( PropertyNames.Shapes, m_shapes );
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
      
      writer.WriteValue( PropertyNames.Type, ParagraphItemType.Canvas );
      writer.WriteValue( PropertyNames.BorderColor, this.BorderColor );
      writer.WriteValue( PropertyNames.Width, this.Width );
      writer.WriteValue( PropertyNames.Height, this.Height );
    }
    /// <summary>
    /// Overloaded. Reads XML attributes.
    /// </summary>
    /// <param name="reader">Reader object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );

      this.BorderColor = reader.ReadColor( PropertyNames.BorderColor );
      this.Width  = reader.ReadFloat( PropertyNames.Width );
      this.Height = reader.ReadFloat( PropertyNames.Height );
    }
    #endregion

    #region WidgetBase overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new LayoutInfo( ChildrenLayoutDirection.Horizontal );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public virtual void Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      DLSGraphics dg = cg as DLSGraphics;
      dg.DrawCanvas( this, ltWidget );

      // Draw child shapes
      for( int i = 0; i < m_shapes.Count; i++ )
      {
        ( m_shapes[ i ] as IWidget ).Draw( cg, ltWidget );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public SizeF Measure( CustomGraphics graphics )
    {
      return this.Size;
    } 
   
    #endregion

    #region ISplitLeafWidget Members
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public ISplitLeafWidget[] SplitByOffset( CustomGraphics graphics, SizeF offset )
    {      
      return new ISplitLeafWidget[] { this, new Canvas( DocumentEx ) };
    }

    #endregion
  }
}