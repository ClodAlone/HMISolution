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
using System.Text.RegularExpressions;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a Field.
  /// </summary>
  public class Field : 
    TextRange,
    IField
  {
    #region Members
    /// <summary>
    /// 
    /// </summary>
    private DLSFieldType m_fieldType = DLSFieldType.FieldPage;
    /// <summary>
    /// 
    /// </summary>
    private string m_fieldPattern = "{0}";
    #endregion

    #region Properties
    /// <summary>
    /// Gets / sets field pattern.
    /// </summary>
    public string FieldPattern
    {
      get
      {
        return m_fieldPattern;
      }
      set
      {
        m_fieldPattern = value;
      }
    }
    /// <summary>
    /// Gets / sets field type.
    /// </summary>
    public DLSFieldType FieldType
    {
      get
      {
        return m_fieldType;
      }
      set
      {
        m_fieldType = value;
      }
    }
    #endregion

    #region Constructors

    /// <summary>
    /// Initializing constructor.
    /// </summary>
    /// <param name="doc"></param>
    public Field( IDocument doc )
      : base( doc )
    {}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="field"></param>
    /// <param name="paragraph"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal Field( Field field, IParagraph paragraph )
      : base( field, paragraph )
    {}
    #endregion
    
    #region WidgetBase overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new LayoutFieldInfo( ChildrenLayoutDirection.Horizontal );
      SetLayoutFieldInfo();
    }
    /// <summary>
    /// Sets the layout field info.
    /// </summary>
    private void SetLayoutFieldInfo()
    {
      m_layoutInfo.IsLineBreak = CharacterFormat.LineBreak;
      
      if( CharacterFormat.Position > 0 )
      {
        m_layoutInfo.Margins.Bottom = CharacterFormat.Position;
      }

      if( CharacterFormat.Position < 0 )
      {
        m_layoutInfo.Margins.Top = -CharacterFormat.Position;
      }
      
      ( m_layoutInfo as LayoutFieldInfo ).FieldType = ( int )FieldType;
    }
    #endregion
    
    #region XDLSSerializationBase overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
      m_fieldType = ( DLSFieldType )reader.ReadEnum( XDLSConstants.FieldTypeAttr, typeof( DLSFieldType ) );
    }  
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      writer.WriteValue( XDLSConstants.FieldTypeAttr, FieldType );
    }
    #endregion
  }
}