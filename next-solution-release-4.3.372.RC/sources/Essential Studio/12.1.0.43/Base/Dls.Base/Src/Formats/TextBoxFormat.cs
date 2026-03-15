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
using System.Drawing;


using Syncfusion.DLS.XML;

#endregion

namespace Syncfusion.DLS
{
	/// <summary>
	/// Summary description for TextBoxFormat.
	/// </summary>
	public class TextBoxFormat : FormatBase
	{
    #region Class members
    /// <summary>
    /// TextBoxFormat class members
    /// </summary>
    private HorizontalOrigin m_horizRelation;
    private VerticalOrigin m_vertRelation;
    private float m_width;
    private float m_height;
    private Color m_fillColor;
    private Color m_lineColor;
    private TextBoxLineStyle m_lineStyle;
    private TextWrappingStyle m_wrapStyle;
    #endregion

    #region Class properties
    /// <summary>
    /// Get/set horizontal origin
    /// </summary>
    public HorizontalOrigin HorizontalOrigin
    {
      get
      {
        return m_horizRelation;
      }
      set
      {
        m_horizRelation  = value;
      }
    }

    /// <summary>
    /// Get/set vertical origin 
    /// </summary>
    public VerticalOrigin VerticalOrigin
    {
      get
      {
        return m_vertRelation;
      }
      set
      {
        m_vertRelation = value;
      }
    }
    /// <summary>
    /// Get/set text Wrapping style
    /// </summary>
    public TextWrappingStyle TextWrappingStyle
    {
      get
      {
        return m_wrapStyle;
      }
      set
      {
        m_wrapStyle = value;
      }
    }
    /// <summary>
    /// Get/set fill color for textbox
    /// </summary>
    public Color FillColor
    {
      get
      {
        return m_fillColor;
      }
      set
      {
        m_fillColor = value;
      }
    }
    /// <summary>
    /// Get/set tetxbox linestyle
    /// </summary>
    public TextBoxLineStyle LineStyle
    {
      get
      {
        return m_lineStyle;
      }
      set
      {
        m_lineStyle = value;
      }
    }
    /// <summary>
    /// Get/set textbox width
    /// </summary>
    public float Width
    {
      get
      {
        return m_width;
      }
      set
      {
        m_width = value;
      }
    }
    /// <summary>
    /// Get/set textbox height
    /// </summary>
    public float Height
    {
      get
      {
        return m_height;
      }
      set
      {
        m_height = value;
      }
    }
    /// <summary>
    /// Get/set line color.
    /// </summary>
    public Color LineColor
    {
      get
      {
        return m_lineColor;
      }
      set
      {
        m_lineColor = value;
      }
    }
 
    #endregion

    #region Class constructor
    /// <summary>
    /// 
    /// </summary>
    public TextBoxFormat()
		{
      m_wrapStyle = TextWrappingStyle.Square;
      m_fillColor = Color.White;
      m_lineColor = Color.Black;
      m_lineStyle = TextBoxLineStyle.Simple;
      m_horizRelation = HorizontalOrigin.Column;
      m_vertRelation = VerticalOrigin.Paragraph;
		}
    #endregion

    #region Class overrides
    /// <summary>
    /// Get degault tetxbox values
    /// </summary>
    /// <param name="key"></param>
    /// <returns>null ( don't use keys )</returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object GetDefValue( int key )
    {
      return null;
    }
    /// <summary>
    /// Write textbox's XML attributes
    /// </summary>
    /// <param name="writer"> XMLWriter</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      if( FillColor != Color.White )
        writer.WriteValue( XDLSConstants.ShapeFillColorAttr, FillColor );

      if( Height != 0 )
        writer.WriteValue( XDLSConstants.ShapeHeightAttr, Height );

      if( HorizontalOrigin != HorizontalOrigin.Column )
        writer.WriteValue( XDLSConstants.ShapeHorizOriginAttr, HorizontalOrigin );

      if( LineStyle != TextBoxLineStyle.Simple )
        writer.WriteValue( XDLSConstants.ShapeLineStyleAttr, LineStyle );

      if( TextWrappingStyle != TextWrappingStyle.Square )
        writer.WriteValue( XDLSConstants.ShapeTextWrappingStyleAttr, TextWrappingStyle );

      if( VerticalOrigin != VerticalOrigin.Paragraph )
        writer.WriteValue( XDLSConstants.ShapeVertOriginAttr, VerticalOrigin );

      if( Width != 0 )
        writer.WriteValue( XDLSConstants.ShapeWidthAttr, Width );

      if( LineColor != Color.Black )
        writer.WriteValue( XDLSConstants.ShapeLineColorAttr, LineColor );
    }
    /// <summary>
    /// Read textbox's XML attributes
    /// </summary>
    /// <param name="reader"> XMLReader</param>
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
      //TextBoxFillColor
      if( reader.HasAttribute( XDLSConstants.ShapeFillColorAttr ) )
      {
        FillColor = reader.ReadColor( XDLSConstants.ShapeFillColorAttr );
      }
      //TextBoxHeight
      if( reader.HasAttribute( XDLSConstants.ShapeHeightAttr ) )
      {
        Height = reader.ReadFloat( XDLSConstants.ShapeHeightAttr );
      }
      //TextBoxHorizOrigin
      if( reader.HasAttribute( XDLSConstants.ShapeHorizOriginAttr ) )
      {
        HorizontalOrigin = ( HorizontalOrigin )reader.ReadEnum( XDLSConstants.ShapeHorizOriginAttr, typeof( HorizontalOrigin ));// Syncfusion.DLS.HorizontalOrigin );
      }
      //TextBoxLineStyle
      if( reader.HasAttribute( XDLSConstants.ShapeLineStyleAttr) )
      {
        LineStyle = ( TextBoxLineStyle )reader.ReadEnum( XDLSConstants.ShapeLineStyleAttr, typeof( TextBoxLineStyle ));
      }
      //TextBoxTextWrappingStyle
      if( reader.HasAttribute( XDLSConstants.ShapeTextWrappingStyleAttr ) )
      {
        TextWrappingStyle = ( TextWrappingStyle )reader.ReadEnum( XDLSConstants.ShapeTextWrappingStyleAttr, typeof( TextWrappingStyle ));
      }
      //TextBoxTextBoxVertOrigin
      if( reader.HasAttribute( XDLSConstants.ShapeVertOriginAttr ) )
      {
        VerticalOrigin = ( VerticalOrigin )reader.ReadEnum( XDLSConstants.ShapeVertOriginAttr, typeof( VerticalOrigin ) );
      }
      //TextBoxWidth
      if( reader.HasAttribute( XDLSConstants.ShapeWidthAttr ) )
      {
        Width = reader.ReadFloat( XDLSConstants.ShapeWidthAttr );
      }
      //Shape line color
      if( reader.HasAttribute( XDLSConstants.ShapeLineColorAttr ))
      {
        LineColor = reader.ReadColor( XDLSConstants.ShapeLineColorAttr );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    virtual public TextBoxFormat Clone()
    {
      TextBoxFormat format = new TextBoxFormat();

      format.FillColor = FillColor;
      format.LineColor = LineColor;
      format.Height = Height;
      format.HorizontalOrigin = HorizontalOrigin;
      format.LineStyle = LineStyle;
      format.TextWrappingStyle = TextWrappingStyle;
      format.VerticalOrigin = VerticalOrigin;
      format.Width = Width;

      return format;
    }
   #endregion
	}
}
