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
using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion


namespace Syncfusion.DLS
{
	/// <summary>
	/// Summary description for TextBox.
	/// </summary>
	public class TextBox 
    : ParagraphItem, ITextBox
	{
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    protected TextBody m_textBody;
    protected TextBoxFormat m_txbxFormat;
    #endregion

    #region Class properties
    /// <summary>
    /// Get/set TextBoxFormat value
    /// </summary>
    public TextBoxFormat TextBoxFormat
    {
      get
      {
        return m_txbxFormat;
      }
      set
      {
        m_txbxFormat = value;
      }
    }
    /// <summary>
    /// Get/set TextBody value
    /// </summary>
    public ITextBody TextBoxBody 
    { 
      get
      { 
        return m_textBody; 
      }
    }
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public TextBox( IDocument doc )
      : base( doc )
		{
      m_txbxFormat = DocumentEx.CreateTextboxFormatImpl();
      m_textBody = DocumentEx.CreateTextBodyImpl();
		}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="txbxItem"></param>
    /// <param name="paragraph"></param>
    protected internal TextBox( TextBox txbxItem, IParagraph paragraph )
      : this( paragraph.Document )
    {
      foreach( Paragraph par in txbxItem.TextBoxBody.Paragraphs )
      {
        this.TextBoxBody.Paragraphs.Add( par.Clone( paragraph.Document ) );
      }
      m_txbxFormat = txbxItem.TextBoxFormat.Clone();
    }
    #endregion 

    #region Class overrides
    /// <summary>
    /// Clones itself.
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    public override IParagraphItem Clone( IParagraph paragraph )
    {
      return CloneImpl( paragraph );
    }

    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void CreateLayoutInfo()
    {
      //throw new NotImplementedException();
      m_layoutInfo = new LayoutInfo();
    }

    /// <summary>
    /// Clone method implementation.
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    protected virtual TextBox CloneImpl( IParagraph paragraph )
    {
      return new TextBox( this, paragraph );
    }
    #endregion

    #region IXDLSSerializable implement
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddElement( XDLSConstants.TextBodyTag, TextBoxBody );
      XDLSHolder.AddElement( XDLSConstants.TextBoxFormatTag, TextBoxFormat );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      writer.WriteValue( XDLSConstants.TypeTag, ParagraphItemType.TextBox );
    }
    #endregion
	}
}
