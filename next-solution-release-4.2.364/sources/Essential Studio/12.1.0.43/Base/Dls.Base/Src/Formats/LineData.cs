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

using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
	/// <summary>
	/// Represents line properties
	/// </summary>
	public class LineData : FormatBase
	{
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    public const int NoLineKey = 1;
    /// <summary>
    /// 
    /// </summary>
    public const int LineColorKey = 2;
    /// <summary>
    /// 
    /// </summary>
    public const int LineWidthKey = 3;
    /// <summary>
    /// 
    /// </summary>
    public const int DashStyleKey = 4;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets whether line is enabled.
    /// </summary>
    public bool NoLine
    {
      get
      {
        return ( bool )this[ NoLineKey ];
      }
      set
      {
        this[ NoLineKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets line color.
    /// </summary>
    public Color LineColor
    {
      get
      {
        return ( Color )this[ LineColorKey ];
      }
      set
      {
        NoLine = ( value == Color.Empty );
        
        this[ LineColorKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets line width.
    /// </summary>
    public float LineWidth
    {
      get
      {
        return ( float )this[ LineWidthKey ];
      }
      set
      {
        if( value != 0 )
          NoLine = false;
        else
          NoLine = true;
        
        this[ LineWidthKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets line style.
    /// </summary>
    public DashStyle DashStyle
    {
      get
      {
        return ( DashStyle )this[ DashStyleKey ];
      }
      set
      {
        this[ DashStyleKey ] = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="baseKey"></param>
    /// <param name="offset"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    internal LineData( FormatBase parent, int baseKey, int offset )
      : base( parent, baseKey, offset )
    {}
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object GetDefValue( int key )
    {
      switch( key )
      {
        case NoLineKey:
          return false;
        case LineColorKey:
          return Color.Black;
        case LineWidthKey:
          return 1f;
        case DashStyleKey:
          return DashStyle.Solid;
      }
      
      throw new ArgumentException( "key has invalid value" );
    }
    /// <summary>
    /// Overloaded. Writes data to XML.
    /// </summary>
    /// <param name="writer">Writer object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      
      if( HasKey( NoLineKey ) )
      {
        writer.WriteValue( PropertyNames.NoLine, this.NoLine );
      }
      if( HasKey( LineColorKey ) )
      {
        writer.WriteValue( PropertyNames.Color, this.LineColor );
      }
      if( HasKey( LineWidthKey ) )
      {
        writer.WriteValue( PropertyNames.Width, this.LineWidth );
      }
      if( HasKey( DashStyleKey ) )
      {
        writer.WriteValue( PropertyNames.DashStyle, this.DashStyle );
      }
    }
    /// <summary>
    /// Overloaded. Reads attributes.
    /// </summary>
    /// <param name="reader">Reader object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );

      if( reader.HasAttribute( PropertyNames.NoLine ) )
      {
        this.NoLine = reader.ReadBoolean( PropertyNames.NoLine );
      }
      if( reader.HasAttribute( PropertyNames.Color ) )
      {
        this.LineColor = reader.ReadColor( PropertyNames.Color );
      }
      if( reader.HasAttribute( PropertyNames.Width ) )
      {
        this.LineWidth = reader.ReadFloat( PropertyNames.Width );
      }
      if( reader.HasAttribute( PropertyNames.DashStyle ) )
      {
        this.DashStyle = ( DashStyle )reader.ReadEnum( PropertyNames.DashStyle,
          typeof( DashStyle ) );
      }
    }
    #endregion
  }
}