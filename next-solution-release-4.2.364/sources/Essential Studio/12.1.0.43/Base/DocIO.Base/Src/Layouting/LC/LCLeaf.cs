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
using System.Drawing;

#if DEBUG_LAYOUTING
using System.Diagnostics;
#endif
#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// Summary description for LCText.
  /// </summary>
  public class LCLeaf : LayoutContext
  {
    #region Properties
    /// <summary>
    /// 
    /// </summary>
    protected ILeafWidget LeafWidget
    {
      get
      {
        return m_widget as ILeafWidget;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="LCLeaf"/> class.
    /// </summary>
    /// <param name="strWidget">The STR widget.</param>
    /// <param name="lcOperator">The lc operator.</param>
    public LCLeaf( ILeafWidget strWidget, ILCOperator lcOperator )
      : base( strWidget, lcOperator )
    {
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Layouts the specified rect.
    /// </summary>
    /// <param name="rect">The rect.</param>
    /// <returns></returns>
    public override LayoutedWidget Layout( RectangleF rect )
    {
      CreateLayoutArea( rect );
      ILeafWidget leafWidget = LeafWidget;
      SizeF size = leafWidget.Measure( CustomGraphics );
      
      LayoutTabsInfo tabsInfo = leafWidget.LayoutInfo as LayoutTabsInfo;
      
      if( tabsInfo != null )
      {
        float tabWidth = ( float )tabsInfo.GetNextTabPosition( rect.X );
        
        
        if( tabsInfo.CurrTabJustification == TabJustification.Left )
        {
          size.Width = tabWidth;
        }
      }
      
      bool clipped = ( LayoutInfo.IsClipped && m_layoutArea.Height != 0 && m_layoutArea.Width != 0 );
      
      if( m_layoutArea.TryFit( size ) || clipped )
      {
        FitWidget( size, leafWidget );
        
        LayoutParagraphInfo paragraphInfo = LayoutInfo as LayoutParagraphInfo;
        bool isPageBreak = ( paragraphInfo != null ) ? paragraphInfo.IsPageBreak : false;
        
        if( !isPageBreak )
        {
          m_ltState = LayoutState.Fitted;
        }
        else
        {
          m_ltState = LayoutState.Breaked;
        }

        if( LayoutInfo.IsPageBreakItem )
        {
          m_ltState = LayoutState.Fitted;          
        }
      }
      else
      {
        ISplitLeafWidget splitLeafWidget = LeafWidget as ISplitLeafWidget;

        if( splitLeafWidget != null && size.Height <= m_layoutArea.ClientArea.Height )
        {
          SplitUpWidget( splitLeafWidget );
        }
        else
        {
          m_ltState = LayoutState.NotFitted;
          m_bIsVerticalNotFitted = ( size.Height > m_layoutArea.ClientArea.Height );
        }
      }
      
#if DEBUG_LAYOUTING
      /* Debug code */ DBG_CommitChildContext( this );
#endif
      DoLayoutAfter();
      return m_ltWidget;
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void DoLayoutAfter()
    {
      LayoutFieldInfo fieldInfo = LayoutInfo as LayoutFieldInfo;
      bool cont = ( fieldInfo != null ) ? ( fieldInfo.FieldType > -1 ) : false;
      
      if( cont && m_ltWidget != null )
      {
        m_lcOperator.SendLeafLayoutAfter( m_ltWidget );
      }
    }
    #endregion
    
    #region Implementation
    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    /// <param name="widget"></param>
    private void FitWidget( SizeF size, IWidget widget )
    {
      double width = size.Width + LayoutInfo.Paddings.Left + LayoutInfo.Paddings.Right;
      double height = size.Height + LayoutInfo.Paddings.Top + LayoutInfo.Paddings.Bottom;

      if( width > m_layoutArea.ClientArea.Width )
        width = m_layoutArea.ClientArea.Width;
      if( height > m_layoutArea.ClientArea.Height )
        height = m_layoutArea.ClientArea.Height;
      
      
      m_ltWidget = new LayoutedWidget( widget );
      m_ltWidget.Bounds = new RectangleF(
        ( float )(m_layoutArea.ClientArea.X - LayoutInfo.Paddings.Left),
        ( float )(m_layoutArea.ClientArea.Y - LayoutInfo.Paddings.Top),
        ( float )width, ( float )height );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="splitLeafWidget"></param>
    private void SplitUpWidget( ISplitLeafWidget splitLeafWidget )
    {
      SizeF size;
      ISplitLeafWidget[] splitedLeafWidgets;
      
      if( ( LayoutInfo as LayoutTabsInfo ) != null )
      {
        splitedLeafWidgets = new ISplitLeafWidget[]{ splitLeafWidget, splitLeafWidget  };
      }
      else
      {
        splitedLeafWidgets = splitLeafWidget.SplitByOffset(
          CustomGraphics, m_layoutArea.ClientArea.Size );
      }
      
      m_ltState = LayoutState.NotFitted;

      if( splitedLeafWidgets != null )
      {
        size = splitedLeafWidgets[ 0 ].Measure( CustomGraphics );
        
        if( !m_layoutArea.TryFit( size ) )
        {
#if DEBUG_LAYOUTING
          Trace.WriteLine( "Split string not fitted to line" );
#endif
          size.Width = m_layoutArea.ClientArea.Width;
        }
        FitWidget( size, splitedLeafWidgets[ 0 ] );
        
        if( !LayoutInfo.TextWrap )
        {
          m_ltState = LayoutState.Fitted;
        }
        else
        {
          m_sptWidget = splitedLeafWidgets[ 1 ];
          m_ltState = LayoutState.Splitted;
        }
      }
    }
    #endregion
  }
}