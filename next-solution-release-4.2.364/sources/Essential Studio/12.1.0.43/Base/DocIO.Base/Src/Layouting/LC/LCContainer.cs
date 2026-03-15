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
  /// LCContainer class.
  /// </summary>
  public class LCContainer : LayoutContext
  {
    #region Members
    /// <summary>
    /// 
    /// </summary>
    protected int m_curWidgetIndex = 0; 
    /// <summary>
    /// 
    /// </summary>
    protected LayoutedWidget m_currChildLW;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_bAtLastOneChildFitted = false;
    #endregion

    #region Properties
    /// <summary>
    /// 
    /// </summary>
    protected IWidgetContainer WidgetContainer
    {
      get
      {
        return m_widget as IWidgetContainer;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected IWidget CurrentChildWidget
    {
      get
      {
        bool isExist = ( m_curWidgetIndex > -1 && m_curWidgetIndex < WidgetContainer.Count );
        return isExist ? WidgetContainer[ m_curWidgetIndex ] : null;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="LCContainer"/> class.
    /// </summary>
    /// <param name="widget">The widget.</param>
    /// <param name="lcOperator">The lc operator.</param>
    public LCContainer( IWidgetContainer widget, ILCOperator lcOperator )
      : base( widget, lcOperator )
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
      CreateLayoutedWidget( rect.Location );
      
      do
      {
        // Creates next child context
        LayoutContext childContext = CreateNextChildContext();

        // If child context NULL - break cycle
        if( childContext == null )
        {
          if( m_bAtLastOneChildFitted )
          {
            m_ltState = LayoutState.Fitted;
          }
          break;
        }

        childContext.LayoutInfo.TextWrap = LayoutInfo.TextWrap;
        DoLayoutChild( childContext );
        // "Commit process" for child context in current context
        CommitChildContext( childContext );

      }
      while( State == LayoutState.Unknown );
      
      // Executes after layout actions
      DoLayoutAfter();

#if DEBUG_LAYOUTING
      /* Debug code */ DBG_CommitChildContext( this );
#endif
      return m_ltWidget;
    }
    #endregion

    #region Overrides
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual LayoutContext CreateNextChildContext()
    {
      RepeatNextWidget: 
      IWidget childWidget = CurrentChildWidget;

      if( childWidget != null )
      {
        if( childWidget.LayoutInfo != null && childWidget.LayoutInfo.IsSkip )
        {
          if( NextChildWidget() )
          {
            goto RepeatNextWidget; // Try repeat get child widget
          }
          else
            return null;
        }

        return LayoutContext.Create( childWidget, m_lcOperator );
      }

      return null;
    }
    /// <summary>
    /// 
    /// </summary>
    protected virtual void CommitForNotFitted( LayoutContext childContext )
    {
      m_bIsVerticalNotFitted = childContext.IsVerticalNotFitted;
      
      if( CommitKeepWithNext() )
      {
        return;
      }
      
      if( m_bAtLastOneChildFitted )
      {
        SplitedUpWidget( CurrentChildWidget );
        m_ltState = LayoutState.Splitted;
      }
      else
      {
        m_ltState = LayoutState.NotFitted;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected virtual void CommitForFitted( LayoutContext childContext )
    {
      AddChildLW( childContext );
      NextChildWidget();
      
      if( childContext.LayoutInfo.IsLineBreak && CurrentChildWidget != null )
      {
        SplitedUpWidget( CurrentChildWidget );
        m_ltState = LayoutState.Splitted;
      }
      else if( childContext.LayoutInfo.IsPageBreakItem  )
      {
        if( CurrentChildWidget != null )
        {
          SplitedUpWidget( CurrentChildWidget );
        }
        else
        {
          m_sptWidget = new SplitWidgetContainer( WidgetContainer, childContext.Widget, WidgetContainer.Count -1 );
        }
        m_ltState = LayoutState.Breaked;
      }
      else if( !m_bAtLastOneChildFitted )
      {
        m_bAtLastOneChildFitted = true;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected virtual void CommitForSplitted( LayoutContext childContext )
    {
      m_bIsVerticalNotFitted = childContext.IsVerticalNotFitted;
      LayoutParagraphInfo paragraphInfo = LayoutInfo as LayoutParagraphInfo;
      
      if( paragraphInfo != null )
      {
        if( paragraphInfo.FirstLineIndent > 0 )
        {
          paragraphInfo.SignFirstLineIndent();
        }
      }
      
      AddChildLW( childContext );
      SplitedUpWidget( childContext.SplittedWidget );
      m_ltState = LayoutState.Splitted;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="childContext"></param>
    protected virtual void CommitForBreaked( LayoutContext childContext )
    {

      AddChildLW( childContext );
      // update current widget index      
      LayoutedWidget child = m_ltWidget.ChildWidgets[ m_ltWidget.ChildWidgets.Count - 1 ];
      
      if( child != null )
      {
        LayoutedWidget chld = child.ChildWidgets[ child.ChildWidgets.Count - 1 ];

        if( chld != null )
        {
          m_curWidgetIndex = ( chld.Widget.LayoutInfo.IsPageBreakItem ) ? child.ChildWidgets.Count - 1 : m_curWidgetIndex;
        }        
      }      
      
      NextChildWidget();
      
      if( CurrentChildWidget != null )
      {
        SplitedUpWidget( CurrentChildWidget );
        m_ltState = LayoutState.Splitted;
      }
      else
      {
        m_ltState = LayoutState.Breaked;
      }
      
    }
    /// <summary>
    /// 
    /// </summary>
    protected virtual void UpdateClientArea()
    {
      Spacings margins = m_currChildLW.Widget.LayoutInfo.Margins;
      RectangleF bounds = m_currChildLW.Bounds;
      bounds.X -= ( float )margins.Left;
      bounds.Y -= ( float )margins.Top;
      bounds.Width += ( float )( margins.Left + margins.Right );
      bounds.Height += ( float )( margins.Top + margins.Bottom );
      
      switch( LayoutInfo.ChildrenLayoutDirection )
      {
        case ChildrenLayoutDirection.Horizontal:
          m_layoutArea.CutFromLeft( bounds.Right );
          break;
        case ChildrenLayoutDirection.Vertical:
          m_layoutArea.CutFromTop( bounds.Bottom );
          break;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected virtual void ChangeChildsAlignment()
    {}
    #endregion
    
    #region Implementation
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected bool NextChildWidget()
    {
      if( m_curWidgetIndex > -1 && 
        m_curWidgetIndex < WidgetContainer.Count - 1 )
      {
        m_curWidgetIndex++;
        return true;
      }
      m_curWidgetIndex = -1;
      return false;
    }
    /// <summary>
    /// 
    /// </summary>
    protected void SplitedUpWidget( IWidget splitWidget )
    {
      // Creates "splitted widget container"
      m_sptWidget = new SplitWidgetContainer
        (
        WidgetContainer,
        splitWidget,
        m_curWidgetIndex 
        );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="childContext"></param>
    protected void CommitChildContext( LayoutContext childContext )
    {
      PreviousTabCorrection();
      
      switch( childContext.State )
      {
        case LayoutState.Unknown:
          //throw new InvalidLayoutStateException();
          //CommitForNotFitted( childContext );
          m_ltState = LayoutState.Unknown;
          break;
          //case LayoutState.Fitting:
        case LayoutState.Fitted:
          CommitForFitted( childContext );
          break;
        case LayoutState.NotFitted:
          CommitForNotFitted( childContext );
          break;
        case LayoutState.Splitted:
          CommitForSplitted( childContext );
          break;
        case LayoutState.Breaked:
          CommitForBreaked( childContext );
          break;
      }
    }
    /// <summary>
    /// Layouted current child 
    /// </summary>
    /// <param name="childContext"></param>
    protected virtual void DoLayoutChild( LayoutContext childContext )
    {
      m_currChildLW = childContext.Layout( m_layoutArea.ClientActiveArea );
    }
    /// <summary>
    /// 
    /// </summary>
    protected void AddChildLW( LayoutContext childContext )
    {
      m_ltWidget.ChildWidgets.Add( m_currChildLW );
      UpdateClientArea();
      UpdateLWBounds( childContext );
    }
    /// <summary>
    /// 
    /// </summary>
    private void UpdateLWBounds( LayoutContext childContext )
    {
      RectangleF bounds = m_ltWidget.Bounds;
      RectangleF childBounds = m_currChildLW.Bounds;

      double rightPad = ( m_bSkipAreaSpacing ) ? 
        0f : childContext.BoundsPaddingRight;
      double bottomPad = ( m_bSkipAreaSpacing ) ? 
        0f : childContext.BoundsPaddingBottom;
      
      ChangeChildsAlignment();
      
      double right = Math.Max( childBounds.Right + rightPad, bounds.Right );
      double bottom = Math.Max( childBounds.Bottom + bottomPad, bounds.Bottom );
      SizeF size = new SizeF( ( float )( right - bounds.Left ), ( float )( bottom - bounds.Top ) );
      m_ltWidget.Bounds = new RectangleF( bounds.Location, size );
    }
    /// <summary>
    /// Previouses the tab correction.
    /// </summary>
    private void PreviousTabCorrection()
    {
      if( m_ltWidget.ChildWidgets.Count > 0 )
      {
        LayoutTabsInfo tabsInfo =
          m_ltWidget.ChildWidgets[ m_ltWidget.ChildWidgets.Count - 1 ].Widget.LayoutInfo
          as LayoutTabsInfo;
        
        if( tabsInfo != null )
        {
          if( tabsInfo.CurrTabJustification != TabJustification.Left )
          {
            RectangleF bounds = m_currChildLW.Bounds;
            float tabPos = ( float )tabsInfo.GetNextTabPosition( bounds.X );
            
            switch( tabsInfo.CurrTabJustification )
            {
              case TabJustification.Centered:
                tabPos -= ( bounds.Width / 2 );
                break;
              case TabJustification.Decimal:
                // tabPos = desimal position
              case TabJustification.Right:
                tabPos -= bounds.Width;
                break;
            }
          
            if( tabPos > 0 )
            {
              bounds.X += tabPos;
            }
            
            m_currChildLW.Bounds = bounds;
          }
        }
      }
    }
    /// <summary>
    /// Commits the keep with next.
    /// </summary>
    private bool CommitKeepWithNext()
    {
      if( m_ltWidget.ChildWidgets.Count > 0 )
      {
        IWidget widget = m_ltWidget.ChildWidgets[ m_ltWidget.ChildWidgets.Count - 1 ].Widget;
        LayoutParagraphInfo paragraphInfo = widget.LayoutInfo as LayoutParagraphInfo;
        
        if( paragraphInfo != null )
        {
          if( paragraphInfo.IsKeepWithNext && m_bAtLastOneChildFitted )
          {
            m_ltWidget.ChildWidgets.RemoveAt( m_ltWidget.ChildWidgets.Count - 1 );
            m_curWidgetIndex -= 1;
      
            // restore property first line indent
            if( paragraphInfo.FirstLineIndent < 0 )
            {
              paragraphInfo.SignFirstLineIndent();
            }
      
            SplitedUpWidget( widget );
            m_ltState = LayoutState.Splitted;
            return true;
          }
        }
      }
      
      return false;
    }
    #endregion
  }
  /// <summary>
  /// LCLineContainer class.
  /// </summary>
  public class LCLineContainer : LCContainer
  {
    #region Constructors
      /// <summary>
      /// Initializes a new instance of the <see cref="LCLineContainer"/> class.
      /// </summary>
      /// <param name="container">The container.</param>
      /// <param name="lcOperator">The lc operator.</param>
    public LCLineContainer( IWidgetContainer container, ILCOperator lcOperator )
      : base( container, lcOperator )
    {
      m_bSkipAreaSpacing = true;
    }
    #endregion
    
    #region Overrides
    /// <summary>
    /// Layouted current child
    /// </summary>
    /// <param name="childContext"></param>
    protected override void DoLayoutChild( LayoutContext childContext )
    {
      RectangleF clArea = m_layoutArea.ClientActiveArea;
      
      LayoutParagraphInfo paragraphInfo = LayoutInfo as LayoutParagraphInfo;
      
      bool isFirst = false; 
      
      if( paragraphInfo != null )
      {
        isFirst = paragraphInfo.FirstLineIndent != 0;        
      }
      
      
      if( m_ltWidget.ChildWidgets.Count == 0 )
      {
        if( isFirst && ( childContext.Widget as SplitWidgetContainer ) == null )
        {
          clArea.X += paragraphInfo.FirstLineIndent;
          clArea.Width -= paragraphInfo.FirstLineIndent;
        }

        if( paragraphInfo.LevelNumber != -1 )
        {
          clArea.X += paragraphInfo.ListTab;
          clArea.Width -= paragraphInfo.ListTab;          
        }               
      }
      
      m_currChildLW = childContext.Layout( clArea );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override LayoutContext CreateNextChildContext()
    {
      return ( WidgetContainer != null ) 
        ? new LCContainer( WidgetContainer, m_lcOperator ) : null;
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void CommitForNotFitted( LayoutContext childContext )
    {
      m_bIsVerticalNotFitted = childContext.IsVerticalNotFitted;
            
      if( /*m_ltState == LayoutState.Fitting AtLastOneChildFitted*/ m_bAtLastOneChildFitted )
      {
        LayoutParagraphInfo paragraphInfo = LayoutInfo as LayoutParagraphInfo;
        bool isKeepTogether = ( paragraphInfo != null ) ? paragraphInfo.IsKeepTogether : false;
        
        if( !isKeepTogether )
        {
          m_sptWidget = childContext.SplittedWidget;
          m_ltState = LayoutState.Splitted;
        }
        else
        {
          m_ltState = LayoutState.NotFitted;
        }
      }
      else
      {
        m_ltState = LayoutState.NotFitted;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void CommitForFitted( LayoutContext childContext )
    {
      AddChildLW( childContext );
      m_ltState = LayoutState.Fitted;
      
      LayoutParagraphInfo paragraphInfo = childContext.LayoutInfo as LayoutParagraphInfo;
      bool isPageBreak = ( paragraphInfo != null ) ? paragraphInfo.IsPageBreak : false;
      
      if( isPageBreak )
      {
        m_layoutArea.CutFromTop();
        m_ltState = LayoutState.Breaked;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void CommitForSplitted( LayoutContext childContext )
    {
      AddChildLW( childContext );
      m_widget = childContext.SplittedWidget;
      
      //m_ltState = LayoutState.Fitting;
      m_bAtLastOneChildFitted = true;
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void UpdateClientArea()
    {
      RectangleF bounds = m_currChildLW.Bounds;
      bounds.Height -= ( float )m_currChildLW.Widget.LayoutInfo.Margins.Top;
      m_layoutArea.CutFromTop( bounds.Bottom );
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void ChangeChildsAlignment()
    {
      m_currChildLW.AlignBottom( CustomGraphics );
      
      LayoutParagraphInfo paragraphInfo = m_currChildLW.Widget.LayoutInfo as LayoutParagraphInfo;
      HorizontalAlignment alignment = ( paragraphInfo != null ) ? paragraphInfo.Justification : HorizontalAlignment.Left;
      double subWidth = m_layoutArea.ClientActiveArea.Right - m_currChildLW.Bounds.Right - paragraphInfo.Margins.Right;

      switch( alignment )
      {
        case HorizontalAlignment.Center:
          subWidth = subWidth / 2;
          m_currChildLW.ShiftLocation( subWidth, 0 );
          break;
        case HorizontalAlignment.Right:
          m_currChildLW.ShiftLocation( subWidth, 0 );
          break;
        case HorizontalAlignment.Justify:
          m_currChildLW.AlignJustify( CustomGraphics, subWidth );
          break;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void DoLayoutAfter()
    {
      LayoutParagraphInfo paragraphInfo = LayoutInfo as LayoutParagraphInfo;
      bool cont = ( paragraphInfo != null ) ? ( paragraphInfo.LevelNumber > -1 ) : false;
      cont &= ( State != LayoutState.NotFitted );

      if( cont )
      {
        if( paragraphInfo.ListRestart )
        {
          NumberedListCounter.Instance.ResetLevel( paragraphInfo.LevelNumber );
        }
        
        paragraphInfo.ListItemIndex = NumberedListCounter.Instance.NextLevelNumber( paragraphInfo.LevelNumber, paragraphInfo.ListStyleName );
      }
    }
    #endregion
  }
}