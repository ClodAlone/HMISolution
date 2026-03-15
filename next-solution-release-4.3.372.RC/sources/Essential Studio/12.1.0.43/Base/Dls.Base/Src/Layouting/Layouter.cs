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
  /// <summary>
  /// 
  /// </summary>
  public class Layouter
    : ILCOperator
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private CustomGraphics m_cg;
    #endregion
    
    #region Class events
    /// <summary>
    /// 
    /// </summary>
    public delegate void LeafLayoutEventHandler( object sender, LayoutedWidget ltWidget );
    /// <summary>
    /// 
    /// </summary>
    public event LeafLayoutEventHandler LeafLayoutAfter;
    #endregion
    
    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public CustomGraphics CustomGraphics
    {
      get
      {
        return m_cg;
      }
    }
    #endregion
    
    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    public static void ResetCounters()
    {
      NumberedListCounter.Instance.ResetCounter();
    }
    /// <summary>
    /// Layout specified widget container.
    /// <remarks>Method use ILayoutProcessHandler for control layouting process</remarks>
    /// </summary>
    /// <param name="widget">The widget.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="cg">The custom graphics.</param>
    public void Layout( IWidgetContainer widget, ILayoutProcessHandler handler, CustomGraphics cg )
    {
      RectangleF area;
      IWidgetContainer currWidget = widget;
      m_cg = cg;
      #region #dbg
#if DEBUG_LAYOUTING 
      /* DEBUG code */ Trace.WriteLine( "<Layouting-Start>");
#endif
      #endregion

      // Main layouting cycle
      while( handler.GetNextArea( out area ) )
      {
        if( area.IsEmpty )
          break;

        #region #dbg
#if DEBUG_LAYOUTING
        /* DEBUG code */ DBG_NextLayoutingArea( area );
#endif
        #endregion
        LayoutContext lc = LayoutContext.Create( currWidget, this );
        LayoutedWidget ltWidget = lc.Layout( area );
        handler.PushLayoutedWidget( ltWidget );
        #region #dbg
#if DEBUG_LAYOUTING
        /* DEBUG code */ Trace.WriteLine( "<Push-LayoutedWidget> " + ltWidget.Bounds.ToString() );
#endif
        #endregion

        if( lc.IsEnsureSplitted() )
        {
          SplitWidgetContainer splittedWC = lc.SplittedWidget as SplitWidgetContainer;
          bool bContinue = handler.HandleSplittedWidget( splittedWC, lc.State );
          
          if( bContinue )
          {
            #region #dbg
#if DEBUG_LAYOUTING
        /* DEBUG code */ Trace.WriteLine( "<Handle-Splitted> " + lc.State.ToString() );
#endif
            #endregion
            currWidget = splittedWC;
            continue;
          }
        }

        // If current widget not splitted - break main cycle
        break;
      } // End of main layouting cycle
    }
    #endregion
    
    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lfWidget"></param>
    void ILCOperator.SendLeafLayoutAfter( LayoutedWidget ltWidget )
    {
      if( LeafLayoutAfter != null )
      {
        LeafLayoutAfter( this, ltWidget );
      }
    }
    #endregion
    
    #region DEBUG
#if DEBUG_LAYOUTING
    /// <summary>
    /// 
    /// </summary>
    /// <param name="area"></param>
    private void DBG_NextLayoutingArea( RectangleF area )
    {
      Trace.WriteLine( "<Next-Area>" + area.ToString() );
    }
#endif
    #endregion
  }
  /// <summary>
  /// 
  /// </summary>
  public interface ILayoutProcessHandler
  {
    /// <summary>
    /// Gets the next free area.
    /// </summary>
    /// <param name="rect">The rectangle of allowed area.</param>
    /// <returns>True if area allowed, else False</returns>
    bool GetNextArea( out RectangleF rect );
    /// <summary>
    /// Pushes the layouted widget to external holder.
    /// </summary>
    /// <param name="ltWidget">The layouted widget.</param>
    void PushLayoutedWidget( LayoutedWidget ltWidget );
    /// <summary>
    /// Handles the splitted widget.
    /// </summary>
    /// <param name="stWidgetContainer">The splitted widget container.</param>
    /// <param name="state">The current state of layout context.</param>
    /// <returns>True for continue layouting process, False - for stopping</returns>
    bool HandleSplittedWidget( SplitWidgetContainer stWidgetContainer, LayoutState state );
  }
}