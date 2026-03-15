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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
#endregion

namespace Syncfusion.DocIO.DLS
{
	/// <summary>
	/// Summary description for WFieldSeparator.
	/// </summary>
	public class WFieldSeparator : ParagraphItem
	{
    #region Class properties
    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    /// <value>The type of the entity.</value>
    public override EntityType EntityType
    {
      get
      {
        return EntityType.FieldSeparator;
      }
    }   
    #endregion

    #region Class initilaize / finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public WFieldSeparator( WordDocument doc )
      : base( doc )
    {}
    /// <summary>
    /// Initializes a new instance of the <see cref="WFieldSeparator"/> class.
    /// </summary>
    /// <param name="separator">The separator.</param>
    /// <param name="doc">The doc.</param>
    protected internal WFieldSeparator( WFieldSeparator separator,  WordDocument doc )
      : this( doc )
    {}
    #endregion

    #region XDLSSerializable overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      writer.WriteValue( XDLSConstants.TypeTag, ParagraphItemType.FieldSeparator );     
    }
    #endregion  

    #region WidgetBase overrides
    /// <summary>
    /// 
    /// </summary>
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new Syncfusion.Layouting.LayoutInfo();
    }
    #endregion
  }
}
