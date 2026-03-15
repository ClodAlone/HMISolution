#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;

namespace Syncfusion.DocIO.DLS
{
  /// <summary>
  /// Summary description for WFieldEnd.
  /// </summary>
  public class WFieldEnd : ParagraphItem
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    protected string m_fieldName = "";
    #endregion

    #region Class properties
    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    /// <value>The type of the entity.</value>
    public override EntityType EntityType
    {
      get
      {
        return EntityType.FieldEnd;
      }
    }
    /// <summary>
    /// Gets / sets field name.
    /// </summary>
    public string FieldName
    {
      get
      {
        return m_fieldName;
      }
      set
      {
        m_fieldName = value;
      }
    }
    #endregion

    #region Class initialize / finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public WFieldEnd( IWordDocument doc ) : base( ( WordDocument )doc )
    {}
    /// <summary>
    /// Initializes a new instance of the <see cref="WFieldEnd"/> class.
    /// </summary>
    /// <param name="end">The end.</param>
    /// <param name="doc">The doc.</param>
    protected internal WFieldEnd( WFieldEnd end, IWordDocument doc )
      : this( doc)
    {
      FieldName = end.FieldName;
    }
    #endregion

    #region XDLSSerializable overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      writer.WriteValue( XDLSConstants.TypeTag, ParagraphItemType.FieldEnd );

      if( FieldName != string.Empty )
      {
        writer.WriteValue( XDLSConstants.FieldNameAttr, FieldName );
      }
    }
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );

      if( reader.HasAttribute( XDLSConstants.FieldNameAttr ) )
      {
        m_fieldName = reader.ReadString( XDLSConstants.FieldNameAttr );
      }
    }
    #endregion

    #region WidgetBase overrides
    /// <summary>
    /// 
    /// </summary>
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new Syncfusion.Layouting.LayoutInfo();
      //m_layoutInfo.IsSkip = true;
    }
    #endregion
  }
}

