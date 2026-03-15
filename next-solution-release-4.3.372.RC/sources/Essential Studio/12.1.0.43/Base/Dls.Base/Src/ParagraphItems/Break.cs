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
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
	/// <summary>
	/// Summary description for Break.
	/// </summary>
	public class Break : ParagraphItem,
    ILeafWidget
	{
    #region Fields
    /// <summary>
    /// 
    /// </summary>
    private BreakType m_breakType;
    #endregion

    #region Properties
    /// <summary>
    /// Gets break type.
    /// </summary>
    public BreakType BreakType
    {
      get
      {
        return m_breakType;
      }     
    }
    #endregion

    #region Constructor
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public Break( IDocument doc ) : base( doc ) 
    {}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc">Document</param>
    /// <param name="breakType">Break type</param>
		public Break( IDocument doc, BreakType breakType) : base( doc ) 
		{
      m_breakType = breakType;
		}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="docBreak"></param>
    /// <param name="paragraph"></param>
    protected internal Break( Break docBreak, IParagraph paragraph )
      : this( paragraph.Document, docBreak.BreakType )
    {
    }
    #endregion

    #region ILeafWidget Members
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <returns></returns>
    public System.Drawing.SizeF Measure( CustomGraphics graphics )
    {
      return new System.Drawing.SizeF();
    }

    #endregion

    #region XDLSSerializable overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlAttributes(Syncfusion.DLS.XML.IXDLSAttributeWriter writer)
    {
      base.WriteXmlAttributes (writer);
      writer.WriteValue( XDLSConstants.TypeTag, ParagraphItemType.Break );
      writer.WriteValue( XDLSConstants.BreakTypeAttr, BreakType );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
    {
      base.ReadXmlAttributes (reader);
      m_breakType = ( BreakType )reader.ReadEnum( XDLSConstants.BreakTypeAttr, typeof( BreakType ));
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    public override IParagraphItem Clone( IParagraph paragraph )
    {
      return new Break( this, paragraph );
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void CreateLayoutInfo()
    {
      if( m_breakType == BreakType.PageBreak )
      {
        m_layoutInfo = new LayoutParagraphInfo( ChildrenLayoutDirection.Vertical, false );
        m_layoutInfo.IsPageBreakItem = true;
      }
    } 
    #endregion
	}
}
