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

using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents Table Formatting.
  /// </summary>
  public class TableFormat 
    : CellFormat
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int CellSpacingKey = 52;
    /// <summary>
    /// 
    /// </summary>
    private const int LeftIndentKey = 53;
    /// <summary>
    /// 
    /// </summary>
    private const int VerticalBorderKey = 54;
    /// <summary>
    /// 
    /// </summary>
    private const int HorizontalBorderKey = 55;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets spacing between cells.
    /// </summary>
    public float CellSpacing
    {
      get
      {
        return ( float )this[ CellSpacingKey ];
      }
      set
      {
        this[ CellSpacingKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets table indent.
    /// </summary>
    public float LeftIndent
    {
      get
      {
        return ( float )this[ LeftIndentKey ];
      }
      set
      {
        this[ LeftIndentKey ] = value;
      }
    }
    /// <summary>
    /// Gets vertical border
    /// </summary>
    protected internal Border VerticalBorder
    {
      get
      {
        return ( Border )this[ VerticalBorderKey ];
      }
    }
    /// <summary>
    /// Gets horozontal border.
    /// </summary>
    protected internal Border HorizontalBorder
    {
      get
      {
        return ( Border )this[ HorizontalBorderKey ];
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    public TableFormat()
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override object GetDefValue( int key )
    {
      switch( key )
      {
        case CellSpacingKey:
          return ( float )-1;
        case LeftIndentKey:
          return ( float )0;
      }

      return base.GetDefValue( key );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected override FormatBase GetDefComposite( int key )
    {
      switch( key )
      {
        case VerticalBorderKey:
          return GetDefComposite( VerticalBorderKey, new Border( this, VerticalBorderKey ) );
        case HorizontalBorderKey:
          return GetDefComposite( HorizontalBorderKey, new Border( this, HorizontalBorderKey ) );
      }

      return base.GetDefComposite( key );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );

      if( HasKey( CellSpacingKey ) )
      {
        writer.WriteValue( XDLSConstants.CellSpacing, CellSpacing );
      }
      if( HasKey( LeftIndentKey ) )
      {
        writer.WriteValue( XDLSConstants.LeftOffset, LeftIndent );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );

      if( reader.HasAttribute( XDLSConstants.CellSpacing ) )
      {
        CellSpacing = reader.ReadFloat( XDLSConstants.CellSpacing );
      }
      if( reader.HasAttribute( XDLSConstants.LeftOffset ) )
      {
        LeftIndent = reader.ReadFloat( XDLSConstants.LeftOffset );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal override void EnsureComposites()
    {
      base.EnsureComposites();

      EnsureComposites( VerticalBorderKey, HorizontalBorderKey );
    }
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override void InitXDLSHolder()
    {
      base.InitXDLSHolder();
      XDLSHolder.AddElement( XDLSConstants.BorderVerticalTag, VerticalBorder );
      XDLSHolder.AddElement( XDLSConstants.BorderHorizontalTag, HorizontalBorder );
    }
    #endregion
  }
}
