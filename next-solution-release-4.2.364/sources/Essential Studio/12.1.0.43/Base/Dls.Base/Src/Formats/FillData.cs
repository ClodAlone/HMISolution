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
	/// Represents filling properties.
	/// </summary>
	public class FillData : FormatBase
	{
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    protected const int NoFillKey = 1;
    protected const int ColorKey = 2;
    protected const int GradientFillKey = 3;
    protected const int GrColorStartKey = 4;
    protected const int GrColorEndKey = 5;
    protected const int GrModeKey = 6;
    #endregion
	  
    #region Class properties
	  /// <summary>
	  /// Gets / sets Enable filling setting.
	  /// </summary>
	  public bool NoFill
	  {
	    get
	    {
	      return ( bool )this[ NoFillKey ];
	    }
	    set
	    {
	      this[ NoFillKey ] = value;
	    }
	  }
    /// <summary>
    /// Gets / sets filling color.
    /// </summary>
    public Color Color
    {
      get
      {
        return ( Color )this[ ColorKey ];
      }
      set
      {
        if( value != Color.Empty )
          NoFill = false;
        
        this[ ColorKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets gradient fill setting.
    /// </summary>
    public bool GradientFill
    {
      get
      {
        return ( bool )this[ GradientFillKey ];
      }
      set
      {
        if( value )
          NoFill = false;
        
        this[ GradientFillKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets gradient start color .
    /// </summary>
    public Color GradientColorStart
    {
      get
      {
        return ( Color )this[ GrColorStartKey ];
      }
      set
      {
        this[ GrColorStartKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets gradient end color .
    /// </summary>
    public Color GradientColorEnd
    {
      get
      {
        return ( Color )this[ GrColorEndKey ];
      }
      set
      {
        this[ GrColorEndKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets gradient fill mode.
    /// </summary>
    public LinearGradientMode GradientMode
    {
      get
      {
        return ( LinearGradientMode )this[ GrModeKey ];
      }
      set
      {
        this[ GrModeKey ] = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="baseKey"></param>
    /// <param name="offset"></param>
    internal FillData( FormatBase parent, int baseKey, int offset )
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
        case NoFillKey:
          return true;
        case ColorKey:
          return Color.Empty;
        case GradientFillKey:
          return false;
        case GrColorStartKey:
          return Color.Empty;
        case GrColorEndKey:
          return Color.Empty;
        case GrModeKey:
          return LinearGradientMode.Horizontal;
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
      
      if( HasKey( NoFillKey ) )
      {
        writer.WriteValue( PropertyNames.NoFill, this.NoFill );
      }
      if( HasKey( ColorKey ) )
      {
        writer.WriteValue( PropertyNames.Color, this.Color );
      }
      if( HasKey( GradientFillKey ) )
      {
        writer.WriteValue( PropertyNames.GradientFill, this.GradientFill );
      }
      if( HasKey( GrColorStartKey ) )
      {
        writer.WriteValue( PropertyNames.GradientColorStart, this.GradientColorStart );
      }
      if( HasKey( GrColorEndKey ) )
      {
        writer.WriteValue( PropertyNames.GradientColorEnd, this.GradientColorEnd );
      }
      if( HasKey( GrModeKey ) )
      {
        writer.WriteValue( PropertyNames.GradientMode, this.GradientMode );
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

      if( reader.HasAttribute( PropertyNames.NoFill ) )
      {
        this.NoFill = reader.ReadBoolean( PropertyNames.NoFill );
      }
      if( reader.HasAttribute( PropertyNames.Color ) )
      {
        this.Color = reader.ReadColor( PropertyNames.Color );
      }
      if( reader.HasAttribute( PropertyNames.GradientFill ) )
      {
        this.GradientFill = reader.ReadBoolean( PropertyNames.GradientFill );
      }
      if( reader.HasAttribute( PropertyNames.GradientColorStart ) )
      {
        this.GradientColorStart = reader.ReadColor( PropertyNames.GradientColorStart );
      }
      if( reader.HasAttribute( PropertyNames.GradientColorEnd ) )
      {
        this.GradientColorEnd = reader.ReadColor( PropertyNames.GradientColorEnd );
      }
      if( reader.HasAttribute( PropertyNames.GradientMode ) )
      {
        this.GradientMode = ( LinearGradientMode )reader.ReadEnum( PropertyNames.GradientMode,
          typeof( LinearGradientMode ) );
      }
    }
    #endregion
  }
}
