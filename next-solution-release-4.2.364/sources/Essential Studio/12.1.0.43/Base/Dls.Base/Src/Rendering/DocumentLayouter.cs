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
using System.Collections;
using System.Diagnostics;
using System.Drawing;

using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS.Rendering
{
  /// <summary>
  /// 
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class DocumentLayouter : ILayoutProcessHandler
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private PageCollection m_pages = new PageCollection();
    /// <summary>
    /// 
    /// </summary>
    private Page m_currPage = null;
    /// <summary>
    /// 
    /// </summary>
    private IWidgetContainer m_docWidget = null;
    /// <summary>
    /// 
    /// </summary>
    private ISection m_currSection = null;
    /// <summary>
    /// 
    /// </summary>
    private CustomGraphics m_cg;
    private HeaderFooterLPHandler m_headerLPHandler;
    private HeaderFooterLPHandler m_footerLPHandler;
    private int m_columnIndex = 0;
    private float m_columnsWidth = 0f;
    private int m_nextPageIndex = 0;
    private bool m_bFirstPageForSection = true;
    private bool m_bDirty = false;
    #endregion
    
    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public PageCollection Pages
    {
      get
      {
        return m_pages;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected Page CurrentPage
    {
      get
      {
        return m_currPage;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected ISection CurrentSection
    {
      get
      {
        return m_currSection;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected Column CurrentColumn
    {
      get
      {
        return ( CurrentSection.Columns.Count == 0 )
          ? null : CurrentSection.Columns[ m_columnIndex ];
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected bool IsEvenPage
    {
      get
      {
        // m_nextPageIndex means index of NEXT page ( zero-based ), so
        // (m_nextPageIndex - 1) means index of CURRENT page ( zero-based ), but
        // (m_nextPageIndex - 1 + 1) means ORDINAL number, so
        // next expression means even page number for current page:
        return ( m_nextPageIndex % 2 ) == 0; 
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public DocumentLayouter()
    {
      m_headerLPHandler = new HeaderFooterLPHandler( this, false /*as header*/ );
      m_footerLPHandler = new HeaderFooterLPHandler( this, true /*as footer*/ );
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="cg"></param>
    public PageCollection Layout( IDocument doc, CustomGraphics cg )
    {
      if( doc.Sections.Count < 1 )
        return null;
      
      m_docWidget = doc as IWidgetContainer;
      
      if( m_docWidget == null )
        throw new DLSException( "Document can't support IWidgetContainer interface" );
      
      m_currSection = doc.Sections[ 0 ];
      m_cg = cg;

      if( !LayoutPages() )
      {
        for( int i = 0, len = m_pages.Count; i < len; i++ )
        {
          Page page = m_pages[ i ];
          page.UpdateFieldsNumPages( len );
        }
        
        // Second pass
        LayoutPages();
      }
      
      return m_pages;
    }
    #endregion
    
    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool LayoutPages()
    {
      // Prepare layouting process
      m_bDirty = false;
      m_pages.Clear();
      m_nextPageIndex = 0;
      m_bFirstPageForSection = true;
      Layouter.ResetCounters();

      // Start layouting process
      CreateNewPage();
      LayoutHeaderFooter();
      m_bFirstPageForSection = false;
      
      // Layout page columns
      Layouter clsLayouter = new Layouter();
      clsLayouter.LeafLayoutAfter += new Layouter.LeafLayoutEventHandler( Layouter_LeafLayoutAfter );
      clsLayouter.Layout( m_docWidget, this, m_cg  );
      clsLayouter.LeafLayoutAfter -= new Layouter.LeafLayoutEventHandler( Layouter_LeafLayoutAfter );
      
      return !m_bDirty;
    }
    /// <summary>
    /// 
    /// </summary>
    private void CreateNewPage()
    {
      m_currPage = new Page( CurrentSection, m_nextPageIndex );
      m_pages.Add( m_currPage );
      m_nextPageIndex++;
      
      m_columnsWidth = 0f;
      m_columnIndex = 0;
    }
    /// <summary>
    /// 
    /// </summary>
    private void LayoutHeaderFooter()
    {
      IWidgetContainer hWidget = GetCurrentHeader();
      IWidgetContainer fWidget = GetCurrentFooter();
      
      Layouter headerLayouter = new Layouter();
      headerLayouter.LeafLayoutAfter += new Layouter.LeafLayoutEventHandler( Layouter_LeafLayoutAfter );
      headerLayouter.Layout( hWidget, m_headerLPHandler, m_cg );
      headerLayouter.LeafLayoutAfter -= new Layouter.LeafLayoutEventHandler( Layouter_LeafLayoutAfter );
      
      Layouter footerLayouter = new Layouter();
      footerLayouter.LeafLayoutAfter += new Layouter.LeafLayoutEventHandler( Layouter_LeafLayoutAfter );
      footerLayouter.Layout( fWidget, m_footerLPHandler, m_cg );
      footerLayouter.LeafLayoutAfter -= new Layouter.LeafLayoutEventHandler( Layouter_LeafLayoutAfter );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private TextBody GetCurrentHeader()
    {
      HeadersFooters hfs = CurrentSection.HeadersFooters;
      TextBody header = hfs.OddHeader;
      
      if( CurrentSection.PageSetup.DifferentOddAndEvenPages && IsEvenPage )
      {
        header = hfs.EvenHeader;
      }
      
      if( CurrentSection.PageSetup.DifferentFirstPage && m_bFirstPageForSection )
      {
        header = hfs.FirstPageHeader;
      }
      
      return header;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private TextBody GetCurrentFooter()
    {
      HeadersFooters hfs = CurrentSection.HeadersFooters;
      TextBody footer = hfs.OddFooter;
      
      if( CurrentSection.PageSetup.DifferentOddAndEvenPages && IsEvenPage )
      {
        footer = hfs.EvenFooter;
      }
      
      if( CurrentSection.PageSetup.DifferentFirstPage && m_bFirstPageForSection )
      {
        footer = hfs.FirstPageFooter;
      }
      
      return footer;
    }
    /// <summary>
    /// 
    /// </summary>
    private void OnNextSection()
    {
      m_bFirstPageForSection = true;
    }
    #endregion
    
    #region Class event handlers
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ltWidget"></param>
    private void Layouter_LeafLayoutAfter( object sender, LayoutedWidget ltWidget )
    {
      IField field = ltWidget.Widget as IField;
      
      if( field != null )
      {
        if( field.FieldType == DLSFieldType.FieldPage )
        {
          //field.Text = string.Format( field.FieldPattern, CurrentPage.Number + 1 );
          ltWidget.TextTag = ( CurrentPage.Number + 1 ).ToString();
          
          TextRange tr = field as TextRange;
          if( tr != null )
          {
            tr.Text = ltWidget.TextTag;
          }
        }
        else if( field.FieldType == DLSFieldType.FieldNumPages )
        {
          CurrentPage.AddCachedFields( field );
        }
        
        m_bDirty = true;
      }
    }
    #endregion
    
    #region ILayoutProcessHandler members
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    bool ILayoutProcessHandler.GetNextArea( out RectangleF area )
    {
      int colCount = CurrentSection.Columns.Count;
      
      // Is second pass and no columns 
      bool bPageEnd = ( colCount == 0 && m_columnIndex > 0 ); 
      // Or column index greate that max column index
      bPageEnd = bPageEnd || ( colCount > 0 && m_columnIndex > colCount - 1 ); 

      if( bPageEnd )
      {
        CreateNewPage();
        LayoutHeaderFooter();
        m_bFirstPageForSection = false;
      }

      area = CurrentPage.GetColumnArea( m_columnIndex, ref m_columnsWidth );
      m_columnIndex++;

      return !area.IsEmpty;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ltWidget"></param>
    /// <returns></returns>
    void ILayoutProcessHandler.PushLayoutedWidget( LayoutedWidget ltWidget )
    {
      CurrentPage.PageWidgets.Add( ltWidget );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stWidgetContainer"></param>
    /// <param name="state"></param>
    bool ILayoutProcessHandler.HandleSplittedWidget( SplitWidgetContainer stWidgetContainer, LayoutState state )
    {
      if( stWidgetContainer == null )
        throw new ArgumentNullException( "stWidgetContainer" );
      
      if( stWidgetContainer.Count < 1 )
        throw new DLSException( "Split widget container (document) must contains at last one child element!" );

      IWidget widget = stWidgetContainer[ 0 ];
      ISection nextSection = widget as ISection;
      
      if( nextSection == null )
      {
        SplitWidgetContainer swContainer = widget as SplitWidgetContainer;
        if( swContainer != null )
        {
          nextSection = swContainer.RealWidgetContainer as ISection;          
        }
      }
      
      if( nextSection == null )
        throw new DLSException( "Child of SplitWidgetContainer object can't support ISecton interface!" );

      if( m_currSection != nextSection )
      {
        m_currSection = nextSection;
        OnNextSection();
      }
      return true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal bool HeaderGetNextArea( out RectangleF area )
    {
      // LW as page header didn't add
      area = ( CurrentPage.PageWidgets.Count != 0 ) ?
        RectangleF.Empty:
        CurrentPage.GetHeaderArea();

      return !area.IsEmpty;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ltWidget"></param>
    /// <returns></returns>
    internal void HeaderPushLayoutedWidget( LayoutedWidget ltWidget )
    {
      CurrentPage.PageWidgets.Add( ltWidget );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal bool FooterGetNextArea( out RectangleF area )
    {
      // LW as page footer didn't add
      area = ( CurrentPage.PageWidgets.Count != 1 ) ?
        RectangleF.Empty :
        CurrentPage.GetFooterArea();

      return !area.IsEmpty;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ltWidget"></param>
    /// <returns></returns>
    internal void FooterPushLayoutedWidget( LayoutedWidget ltWidget )
    {
      ltWidget.ShiftLocation( 0, -ltWidget.Bounds.Height );
      CurrentPage.PageWidgets.Add( ltWidget );
    }
    #endregion
    
    #region Class internal declarations
    /// <summary>
    /// 
    /// </summary>
    public class HeaderFooterLPHandler : ILayoutProcessHandler
    {
      #region Class members
      private DocumentLayouter m_dl;
      private bool m_bFooter = false;
      #endregion

      #region Class initialize/finalize methods
      /// <summary>
      /// 
      /// </summary>
      /// <param name="dl"></param>
      /// <param name="bFooter"></param>
      public HeaderFooterLPHandler( DocumentLayouter dl, bool bFooter )
      {
        m_dl = dl;
        m_bFooter = bFooter;
      }
      #endregion
      
      #region ILayoutProcessHandler members
      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public bool GetNextArea( out RectangleF area )
      {
        if( !m_bFooter )
          return m_dl.HeaderGetNextArea( out area );
        else
          return m_dl.FooterGetNextArea( out area );
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="ltWidget"></param>
      /// <returns></returns>
      public void PushLayoutedWidget( LayoutedWidget ltWidget )
      {
        if( !m_bFooter )
          m_dl.HeaderPushLayoutedWidget( ltWidget );
        else
          m_dl.FooterPushLayoutedWidget( ltWidget );
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="stWidgetContainer"></param>
      /// <param name="state"></param>
      public bool HandleSplittedWidget( SplitWidgetContainer stWidgetContainer, LayoutState state )
      {
        return false;
      }
      #endregion
    }
    #endregion
  }
}