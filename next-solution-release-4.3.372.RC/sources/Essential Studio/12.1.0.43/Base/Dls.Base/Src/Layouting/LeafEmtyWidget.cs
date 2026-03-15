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
#endregion

namespace Syncfusion.Layouting
{
	/// <summary>
	/// Summary description for LeafEmtyWidget.
	/// </summary>
	public class LeafEmtyWidget : ILeafWidget
	{
    #region Class members
	  private SizeF m_size;
	  private LayoutInfo m_layoutInfo;
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public LeafEmtyWidget( SizeF size )
    {
      m_layoutInfo = new LayoutInfo( ChildrenLayoutDirection.Horizontal );
      m_size = size;
    }
    #endregion
	  
    #region ILeafWidget implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <returns></returns>
    public SizeF Measure( CustomGraphics graphics )
    {
      return m_size;
    }
    /// <summary>
    /// Gets layout info.
    /// </summary>
    public ILayoutInfo LayoutInfo
    {
      get
      {
        return m_layoutInfo;
      }
    }
    /// <summary>
    /// Draw range to graphics.
    /// </summary>
    public void Draw( CustomGraphics g, LayoutedWidget layoutedWidget )
    {
      //throw new NotImplementedException();
    }
    #endregion
  }
}
