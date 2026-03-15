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

using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents Cell Formatting.
  /// </summary>
  public class CellFormat : FormatBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int BordersKey = 1;
    /// <summary>
    /// 
    /// </summary>
    private const int VrAlignmentKey = 2;
    /// <summary>
    /// 
    /// </summary>
    private const int PaddingsKey = 3;
    /// <summary>
    /// 
    /// </summary>
    private const int ShadingColorKey = 4;
    /// <summary>
    /// 
    /// </summary>
    private const int VerticalMergeKey = 6;
    /// <summary>
    /// 
    /// </summary>
    private const int HorizontalMergeKey = 8;
    /// <summary>
    /// 
    /// </summary>
    private const int TextWrapKey = 9;
    #endregion
    
    #region Class properties
    /// <summary>
    /// Gets or sets a value indicating whether [text wrap].
    /// </summary>
    /// <value><c>true</c> if [text wrap]; otherwise, <c>false</c>.</value>
    public bool TextWrap
    {
      get
      {
        return (bool)this[ TextWrapKey ];
      }
      set
      {
        this[ TextWrapKey ] = value;
      }
    }
    /// <summary>
    /// Gets borders.
    /// </summary>
    public Borders Borders
    {
      get
      {
        return this[ BordersKey ] as Borders;
      }
    }
    /// <summary>
    /// Gets paddings.
    /// </summary>
    public Paddings Paddings
    {
      get
      {
        return this[ PaddingsKey ] as Paddings;
      }
    }
    /// <summary>
    /// Gets/sets vertical alignment.
    /// </summary>
    public VerticalAlignment VerticalAlignment
    {
      get
      {
        return (VerticalAlignment)this[ VrAlignmentKey ];
      }
      set
      {
        this[ VrAlignmentKey ] = value;
      }
    }
    /// <summary>
    /// Gets/sets background color.
    /// </summary>
    public Color BackColor
    {
      get
      {
        return (Color)this[ ShadingColorKey ];
      }
      set
      {
        this[ ShadingColorKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets the way of vertical merging of the cell.
    /// </summary>
    public CellMerge VerticalMerge
    {
      get
      {
        return (CellMerge)this[ VerticalMergeKey ];
      }
      set
      {
        this[ VerticalMergeKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets the way of horizontal merging of the cell.
    /// </summary>
    public CellMerge HorizontalMerge
    {
      get
      {
        return (CellMerge)this[ HorizontalMergeKey ];
      }
      set
      {
        this[ HorizontalMergeKey ] = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CellFormat()
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal override void EnsureComposites()
    {
      EnsureComposites( BordersKey, PaddingsKey );
    }
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
        case VrAlignmentKey:
          return VerticalAlignment.Top;
        case VerticalMergeKey:
          return CellMerge.None;
        case HorizontalMergeKey:
          return CellMerge.None;
        case ShadingColorKey:
          return Color.Empty;
        case TextWrapKey:
          return true;
      }
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override FormatBase GetDefComposite( int key )
    {
      switch( key )
      {
        case BordersKey:
          return GetDefComposite( BordersKey, new Borders( this, BordersKey ) );
        case PaddingsKey:
          return GetDefComposite( PaddingsKey, new Paddings( this, PaddingsKey ) );
      }
      return null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      
      if( HasKey( VrAlignmentKey ) )
      {
        writer.WriteValue( XDLSConstants.TableVrAlignmentAttr, VerticalAlignment );
      }
      if( HasKey( VerticalMergeKey ) )
      {
        writer.WriteValue( XDLSConstants.TableVrMergeAttr, VerticalMerge );
      }
      if( HasKey( HorizontalMergeKey ) )
      {
        writer.WriteValue( XDLSConstants.TableHorizMergeAttr, HorizontalMerge );
      }
      if( HasKey( ShadingColorKey ) )
      {
        writer.WriteValue( XDLSConstants.TableCellShadingColorAttr, BackColor );
      }
      if( HasKey( TextWrapKey ) )
      {
        writer.WriteValue( XDLSConstants.CellTextWrapAttr, TextWrap );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
    {
      base.ReadXmlAttributes (reader);

      if( reader.HasAttribute( XDLSConstants.TableVrAlignmentAttr ) )
      {
        VerticalAlignment = 
          ( VerticalAlignment )reader.ReadEnum( XDLSConstants.TableVrAlignmentAttr, typeof( VerticalAlignment ) );
      }
      if( reader.HasAttribute( XDLSConstants.TableVrMergeAttr ) )
      {
        VerticalMerge = ( CellMerge )reader.ReadEnum( XDLSConstants.TableVrMergeAttr, typeof( CellMerge ) );
      }
      if( reader.HasAttribute( XDLSConstants.TableHorizMergeAttr ) )
      {
        HorizontalMerge = ( CellMerge )reader.ReadEnum( XDLSConstants.TableHorizMergeAttr, typeof( CellMerge ) );
      }
      if( reader.HasAttribute( XDLSConstants.TableCellShadingColorAttr ) )
      {
        BackColor = reader.ReadColor( XDLSConstants.TableCellShadingColorAttr );
      }
      if( reader.HasAttribute( XDLSConstants.CellTextWrapAttr ) )
      {
        TextWrap = reader.ReadBoolean( XDLSConstants.CellTextWrapAttr );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddElement( XDLSConstants.BordersItemTag, Borders );
      XDLSHolder.AddElement( XDLSConstants.TableCellPaddingsAttr, Paddings );
    }
    #endregion
  }
}
