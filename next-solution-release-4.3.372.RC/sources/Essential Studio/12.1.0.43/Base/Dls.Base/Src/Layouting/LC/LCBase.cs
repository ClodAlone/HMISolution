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
using System.Diagnostics;
using System.Drawing;
#endregion

namespace Syncfusion.Layouting
{
  public interface ILCOperator
  {
    /// <summary>
    /// Gets or sets the custom graphics.
    /// </summary>
    /// <value>The custom graphics.</value>
    CustomGraphics CustomGraphics
    {
      get;
    }
    /// <summary>
    /// Sends the event that current leaf widget layouted complete.
    /// </summary>
    /// <param name="ltWidget">The widget.</param>
    void SendLeafLayoutAfter( LayoutedWidget ltWidget );
  }
  /// <summary>
  /// Summary description for LayoutContext.
  /// </summary>
  public abstract class LayoutContext
  {
    #region Members
#if DEBUG_LAYOUTING    
    /// <summary>
    /// 
    /// </summary>
    private static string m_DBGIdentSpace = "";
#endif
    /// <summary>
    /// 
    /// </summary>
    protected LayoutState m_ltState = LayoutState.Unknown;
    /// <summary>
    /// 
    /// </summary>
    protected IWidget m_sptWidget = null;
    /// <summary>
    /// 
    /// </summary>
    protected IWidget m_widget;
    /// <summary>
    /// 
    /// </summary>
    protected LayoutedWidget m_ltWidget = null;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_bSkipAreaSpacing = false;
    /// <summary>
    /// 
    /// </summary>
    //protected LayoutArea m_clientArea;
    /// <summary>
    /// 
    /// </summary>
    protected LayoutArea m_layoutArea;
    /// <summary>
    /// 
    /// </summary>
    protected ILCOperator m_lcOperator;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_bIsVerticalNotFitted;
    #endregion

    #region Properties
    /// <summary>
    /// 
    /// </summary>
    public IWidget SplittedWidget
    {
      get
      {
        return m_sptWidget;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public LayoutState State
    {
      get
      {
        return m_ltState;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ILayoutInfo LayoutInfo
    {
      get
      {
        return m_widget.LayoutInfo;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public LayoutArea LayoutArea
    {
      get
      {
        return m_layoutArea;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public CustomGraphics CustomGraphics
    {
      get
      {
        return m_lcOperator.CustomGraphics;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected internal double BoundsPaddingRight
    {
      get
      {
        return m_widget.LayoutInfo.Paddings.Right + 
          m_widget.LayoutInfo.Margins.Right;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected internal double BoundsPaddingBottom
    {
      get
      {
        return m_widget.LayoutInfo.Paddings.Bottom + 
          m_widget.LayoutInfo.Margins.Bottom;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected internal IWidget Widget
    {
      get
      {
        return m_widget;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected internal bool IsVerticalNotFitted
    {
      get
      {
        return m_bIsVerticalNotFitted;
      }
    }
    #endregion
    
    #region Constructors
    /// <summary>
    /// 
    /// </summary>
    public LayoutContext( IWidget widget, ILCOperator lcOperator )
    {
      m_widget = widget;
      m_sptWidget = widget;
      m_lcOperator = lcOperator;
#if DEBUG_LAYOUTING      
      DBG_CreateNextChildContext( this );
#endif
    }
    #endregion

    #region Public methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract LayoutedWidget Layout( RectangleF rect );
    /// <summary>
    /// Determines whether ensure splitted.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if ensure splitted; otherwise, <c>false</c>.
    /// </returns>
    public bool IsEnsureSplitted()
    {
      return State == LayoutState.Splitted && SplittedWidget != null;
    }
    /// <summary>
    /// 
    /// </summary>
    protected virtual void DoLayoutAfter()
    {}
    #endregion
    
    #region Implementation
    /// <summary>
    /// Creates the layout area.
    /// </summary>
    /// <param name="rect">The rect.</param>
    protected void CreateLayoutArea( RectangleF rect )
    {
      if( m_bSkipAreaSpacing )
        m_layoutArea = new LayoutArea( rect );
      else
        m_layoutArea = new LayoutArea( rect, LayoutInfo );
    }
    /// <summary>
    /// 
    /// </summary>
    protected void CreateLayoutedWidget( PointF location )
    {
      m_ltWidget = new LayoutedWidget( m_widget );
      RectangleF bounds = m_ltWidget.Bounds;
      
      location.X += ( float )LayoutInfo.Margins.Left;
      location.Y += ( float )LayoutInfo.Margins.Top;
      bounds.Location = location;
      
      m_ltWidget.Bounds = bounds;
    }
    #endregion

    #region Utility methods
    /// <summary>
    /// Creates the specified widget.
    /// </summary>
    /// <param name="widget">The widget.</param>
    /// <param name="lcOperator">The lc operator.</param>
    /// <returns></returns>
    public static LayoutContext Create( IWidget widget, ILCOperator lcOperator )
    {
      // Test widget as IWidgetContainer
      IWidgetContainer wtContainer = widget as IWidgetContainer;
      if( wtContainer != null )
      {
        if( wtContainer.LayoutInfo.IsLineContainer )
        {
          return new LCLineContainer( wtContainer, lcOperator );
        }

        return new LCContainer( wtContainer, lcOperator );
      }

      // Test widget as ILeafWidget
      ILeafWidget leafWidget = widget as ILeafWidget;
      if( leafWidget != null )
      {
        return new LCLeaf( leafWidget, lcOperator );
      }

      // Test widget as ITableWidget
      ITableWidget table = widget as ITableWidget;
      if( table != null )
      {
        return new LCTable( table, lcOperator );
      }

      // Test widget as SplitTableWidget
      SplitTableWidget spltTable = widget as SplitTableWidget;
      if( spltTable != null )
      {
        return new LCTable( spltTable, lcOperator );
      }

      throw new ArgumentException( "Invalid widget type: " + widget.GetType() );
    }
    #endregion

    #region DEBUG
#if DEBUG_LAYOUTING    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="childContext"></param>
    protected internal void DBG_CreateNextChildContext( LayoutContext childContext )
    {
#if DEBUG_LAYOUTING
      if( childContext != null )
      {
        DBG_WriteIn( DBG_GetContextInfo( childContext ), "<Create>"  );
      }
      DBG_RightIndent();
#endif
    }
    protected internal void DBG_RightIndent()
    {
#if DEBUG_LAYOUTING
       m_DBGIdentSpace += "  ";
#endif
    }
    protected internal void DBG_LeftIndent()
    {
#if DEBUG_LAYOUTING
      m_DBGIdentSpace = m_DBGIdentSpace.Substring( 0, m_DBGIdentSpace.Length - 2 );
#endif
    }
    protected internal void DBG_WriteSpec( string text, string category )
    {
#if DEBUG_LAYOUTING
      Trace.WriteLine( text + " ---------------------------------------", category );
#endif
    }
    protected internal void DBG_WriteIn( string text, string category )
    {
#if DEBUG_LAYOUTING
      Trace.WriteLine( text,
        m_DBGIdentSpace +
        ( m_DBGIdentSpace.Length / 2 ).ToString( "##00" ) + category
      );
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="childContext"></param>
    protected internal void DBG_CommitChildContext( LayoutContext childContext )
    {
      DBG_UpdateLA( childContext );

#if DEBUG_LAYOUTING
      DBG_LeftIndent();

      if( childContext != null )
      {
        DBG_WriteIn( DBG_GetContextInfo( childContext ), "<Commit>" );
      }
#endif
    }
    /// <summary>
    /// DBs the g_ update LA.
    /// </summary>
    /// <param name="context">The context.</param>
    protected internal void DBG_UpdateLA( LayoutContext context )
    {
#if DEBUG_LAYOUTING
      if( context != null )
      {
        RectangleF ca = context.LayoutArea.ClientActiveArea;
        DBG_WriteIn(
          string.Format( "{{Width: {0}, Height: {1}}}", ca.Width, ca.Height ), 
          "<ClientLA>" );
      }
#endif
    }
    /// <summary>
    /// /
    /// </summary>
    /// <param name="childContext"></param>
    /// <returns></returns>
    private string DBG_GetContextInfo( LayoutContext childContext )
    {
#if DEBUG_LAYOUTING
      string strState = ( childContext.State == LayoutState.Unknown )
        ? ""
        : "[" + childContext.State.ToString() + "]";
      string strType = "";

      if( childContext.GetType() == typeof( LCLeaf ) )
      {
        strType = "Leaf";
      }
      else if( childContext.GetType() == typeof( LCLineContainer ) )
      {
        strType = "LineContainer";
      }
      else if( childContext.GetType() == typeof( LCContainer ) )
      {
        strType = "Container";
      }
      else if( childContext.GetType() == typeof( LCTable ) )
      {
        strType = "Table";
      }
      else
      {
        strType = "UnknownContext";
      }

      Type type = childContext.Widget.GetType();
      string typeName = type.Name;
      if(typeName == "SplitWidgetContainer")
      {
        typeName = "SWC-" + (childContext.Widget as SplitWidgetContainer).RealWidgetContainer.GetType().Name;
      }
      return strState + strType + "(" + typeName + ")";
        //childContext.DBG_ChildIndex.ToString() + " / " +
        //childContext.DBG_ChildMax.ToString();

#else
      return string.Empty;
#endif
    }
#endif
    #endregion

  }
}