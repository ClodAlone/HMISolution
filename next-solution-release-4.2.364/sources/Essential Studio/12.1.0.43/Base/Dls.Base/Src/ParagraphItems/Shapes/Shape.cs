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
using System.Drawing.Drawing2D;
using System.Xml;

using Syncfusion.Layouting;
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents base shape.
  /// </summary>
  /// <remarks>Supported by Essential PDF only.</remarks> 
  public class Shape
    : WidgetBase,
      IShape,
      ILeafWidget
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private IStyle m_style;
    /// <summary>
    /// 
    /// </summary>
    private ShapeFormat m_shapeProps = null;
    /// <summary>
    /// 
    /// </summary>
    protected Canvas m_canvas;
    /// <summary>
    /// 
    /// </summary>
    private Matrix m_oldMatrix;
    /// <summary>
    /// 
    /// </summary>
    private Matrix m_matrix;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets name of style attached to the range.
    /// </summary>
    public string StyleName
    {
      get
      {
        return m_style.Name;
      }
    }
    /// <summary>
    /// Gets / sets format of the shape.
    /// </summary>
    public ShapeFormat ShapeFormat
    {
      get
      {
        return m_shapeProps;
      }
    }
    /// <summary>
    /// Gets / sets the world transformation for this shape object.
    /// </summary>
    public Matrix Transform
    {
      get
      {
        return m_matrix;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public Shape( Canvas canvas )
      : base( canvas.Document )
    {
      Document docEx = this.Document as Document;

      if( docEx != null )
      {
        m_shapeProps = docEx.CreateShapeFormatImpl();
      }
      
      m_canvas = canvas;
      m_matrix = new Matrix();
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Attaches style to the range.
    /// </summary>
    /// <param name="styleName">Style to be attached to the range.</param>
    public void ApplyStyle( string styleName )
    {
      m_style = Document.Styles.FindByName( styleName );
      m_shapeProps.ApplyBase( ( m_style as ShapeStyle ).ShapeFormat );
    }
    #endregion

    #region XML serialization overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddRefElement( PropertyNames.Style, m_style );
      XDLSHolder.AddElement( PropertyNames.ShapeFormat, m_shapeProps );
      
      XDLSHolder.SkipID = true;
    }
    /// <summary>
    /// Overloaded. Writes data to XML.
    /// </summary>
    /// <param name="writer">Writer object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlContent( IXDLSContentWriter writer )
    {
      base.WriteXmlContent( writer );

      if( !this.Transform.IsIdentity )
      {
        writer.WriteChildElement( PropertyNames.Transform, this.Transform );
      }
    }
    /// <summary>
    /// Overloaded. Reads inner content.
    /// </summary>
    /// <param name="reader">Reader object.</param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override bool ReadXmlContent( IXDLSContentReader reader )
    {
       bool result = base.ReadXmlContent( reader );
     
      if( reader.TagName == PropertyNames.Transform )
      {
        m_matrix = ( Matrix )reader.ReadChildElement( typeof( Matrix ) );
        result = true;
      }

      return result;
    }
    #endregion

    #region ILeafWidget implement
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new LayoutInfo( ChildrenLayoutDirection.Horizontal );
    }
    /// <summary>
    /// Gets the size of the Custom Graphics
    /// </summary>
    /// <param name="graphics"></param>
    /// <returns></returns>
    public SizeF Measure( CustomGraphics graphics )
    {
      return SizeF.Empty;
    }
    #endregion
    
    #region Class helper methods
    /// <summary>
    /// Helper Method to Apply Transform on the custom graphics
    /// </summary>
    protected void ApplyTransform( CustomGraphics cg )
    {
      m_oldMatrix = cg.ApplyTransform( Transform );
    }
    /// <summary>
    /// Helper Method to reset Transform on the custom graphics
    /// </summary>
    protected void ResetTransform( CustomGraphics cg )
    {
      cg.ResetTransform( m_oldMatrix );
    }
    #endregion
  }
}