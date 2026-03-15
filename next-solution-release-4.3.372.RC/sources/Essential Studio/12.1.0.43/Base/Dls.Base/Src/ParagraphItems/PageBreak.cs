#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
//using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Syncfusion.DLS.XML;

using Syncfusion.Layouting;

#endregion

namespace Syncfusion.DLS
{
  public class PageBreak
    : ParagraphItem,
    ILeafWidget
  {
    #region Class initialize / finalize methods
	  /// <summary>
	  /// 
	  /// </summary>
	  /// <param name="doc"></param>
		public PageBreak( IDocument doc )
	  : base( doc )
		{}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paragraph"></param>
    protected internal PageBreak( IParagraph paragraph )
      : this( paragraph.Document )
    {}
    #endregion

    #region ILeafWidget Members
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <returns></returns>
    public System.Drawing.SizeF Measure( CustomGraphics graphics )
    {
      return new SizeF();
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
      writer.WriteValue( XDLSConstants.TypeTag, ParagraphItemType.PageBreak );
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
      return new PageBreak( paragraph );
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new LayoutParagraphInfo( ChildrenLayoutDirection.Vertical, false );
      m_layoutInfo.IsPageBreakItem = true;
    } 
    #endregion
  }
}
