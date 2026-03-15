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
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents page column.
  /// </summary>
  public class Column : XDLSSerializableBase
  {
    #region Class members
    /// <summary>
    /// Column width.
    /// </summary>
    private float m_fWidth = 0;
    /// <summary>
    /// Spacing between column.
    /// </summary>
    private float m_fSpacing = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets column width.
    /// </summary>
    public float Width
    {
      get
      {
        return m_fWidth;
      }
      set
      {
        m_fWidth = value;
      }
    }
    /// <summary>
    /// Gets / sets spacing between current and next column.
    /// </summary>
    public float Space
    {
      get
      {
        return m_fSpacing;
      }
      set
      {
        m_fSpacing = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initialize constructor.
    /// </summary>
    /// <param name="doc">The doc.</param>
    public Column( IDocument doc )
      : base( doc )
    {
    }
    #endregion

    #region XDLSSerializableBase implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      writer.WriteValue( XDLSConstants.ColumnWidthAttr, Width );
      writer.WriteValue( XDLSConstants.ColumnSpacingAttr, Space );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
      Width = reader.ReadFloat( XDLSConstants.ColumnWidthAttr );
      Space = reader.ReadFloat( XDLSConstants.ColumnSpacingAttr );
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Clones itself.
    /// </summary>
    /// <returns></returns>
    public Column Clone()
    {
      Column column = DocumentEx.CreateColumnImpl();
      column.Width = Width;
      column.Space = Space;
      return column;
    }
    #endregion
  }
}