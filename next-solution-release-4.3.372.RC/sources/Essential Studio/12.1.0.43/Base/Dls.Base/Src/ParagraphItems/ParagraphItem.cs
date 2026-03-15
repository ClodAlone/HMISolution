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
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a paragraph item.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public abstract class ParagraphItem
    : WidgetBase,
      IParagraphItem
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    protected int m_iStartIndex = 0;
    /// <summary>
    /// 
    /// </summary>
    private IParagraph m_ownerParagraph;
    #endregion
    
    #region Class properties
    /// <summary>
    /// Gets owner paragraph.
    /// </summary>
    public IParagraph OwnerParagraph
    {
      get
      {
        return m_ownerParagraph;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates a new Paragraph item.
    /// </summary>
    public ParagraphItem( IDocument doc )
      : base( doc )
    {
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Clones itself.
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    public abstract IParagraphItem Clone( IParagraph paragraph );
    #endregion

    #region IXDLSSerializable implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      //writer.WriteValue( "start", m_iStartIndex );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      //m_iStartIndex = reader.ReadInt( "start" );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paragraph"></param>
    /// <param name="startIndex"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal virtual void SetOwnerParagraph( IParagraph paragraph, int startIndex )
    {
      m_ownerParagraph = paragraph;
      m_iStartIndex = startIndex;
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal int StartIndex
    {
      get
      {
        return m_iStartIndex;
      }
      set
      {
        m_iStartIndex = value;
      }
    }
    #endregion
  }
}