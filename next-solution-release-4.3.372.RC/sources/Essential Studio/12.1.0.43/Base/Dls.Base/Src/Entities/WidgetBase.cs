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

using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// The base implemenation of IWidget interface
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public abstract class WidgetBase
  : XDLSSerializableBase,
    IWidget
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    protected ILayoutInfo m_layoutInfo;
    #endregion
    
    #region Class properties
    /// <summary>
    /// Gets layout info.
    /// </summary>
    ILayoutInfo IWidget.LayoutInfo
    {
      get
      {
        if( m_layoutInfo == null )
        {
          CreateLayoutInfo();
        }
        return m_layoutInfo;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    protected WidgetBase()
    {}
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    /// <param name="doc"></param>
    public WidgetBase( IDocument doc )
      : base( doc )
    {}
    #endregion
    
    #region Class public methods
    /// <summary>
    /// Imlementation of Draw method of IWidget interface .
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    void IWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      DrawImpl( cg, ltWidget );
    }
    #endregion
    
    #region Class helper methods
    /// <summary>
    /// Draw widget to graphics.
    /// </summary>
    protected virtual void DrawImpl( CustomGraphics cg, LayoutedWidget ltWidget )
    {
#if DEBUG_RENDERING      
      Color wColor = Color.Green;
      
      if( this is Document )
      {
        wColor = Color.Green;
      }
      else if( this is Section )
      {
        wColor = Color.BlueViolet;
      }
      else if( this is Paragraph )
      {
        wColor = Color.Red;
      }
      else if( this is Picture )
      {
        wColor = Color.Violet;
      }
      else if( this is ParagraphItem )
      {
        wColor = Color.Gray;
      }

      (cg as DLSGraphics).DrawBounds( wColor, ltWidget.Bounds );
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    protected abstract void CreateLayoutInfo();
    #endregion
  }
	/// <summary>
	/// The base implemenation of IWidgetContainer interface
	/// </summary>
	[ Syncfusion.Documentation.DocumentationExclude() ]
	public abstract class WidgetContainer 
  : WidgetBase,
    IWidgetContainer
	{
    #region Class properties
	  /// <summary>
	  /// Gets count of child items.
	  /// </summary>
	  public int Count
	  {
	    get
	    {
	      return WidgetCollection.Count;
	    }
	  }
	  /// <summary>
	  /// Gets child item by index.
	  /// </summary>
	  public IWidget this[ int index ]
	  {
	    get
	    {
	      return WidgetCollection[ index ] as IWidget;
	    }
	  }
    /// <summary>
    /// 
    /// </summary>
    protected abstract ICollectionBase WidgetCollection{ get; }
    #endregion

    #region Class initialize/finalize methods
	  /// <summary>
	  /// 
	  /// </summary>
	  protected WidgetContainer()
	  {}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public WidgetContainer( IDocument doc ) : base( doc )
    {}
    #endregion
	}
}
