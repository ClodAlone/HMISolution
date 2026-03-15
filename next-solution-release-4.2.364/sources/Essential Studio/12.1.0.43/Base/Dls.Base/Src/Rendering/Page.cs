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
using System.Collections;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS.Rendering
{
  /// <summary>
  /// Summary description for Page.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class Page
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private LayoutedWidgetList m_pageWidgets = new LayoutedWidgetList();
    private PageSetup m_pageSetup;
    private HeadersFooters m_headersFooters;
    private ISection m_docSection;
    private int m_iNumber;
    private Image m_backgroundImage = null;
    private ArrayList m_cachedFields = new ArrayList();
    #endregion

    #region Class properties
    /// <summary>
    /// Collection of layouted widget.
    /// </summary>
    public LayoutedWidgetList PageWidgets
    {
      get
      {
        return m_pageWidgets;
      }
    }
    /// <summary>
    /// Gets page Setup info.
    /// </summary>
    public PageSetup Setup
    {
      get
      {
        return m_pageSetup;
      }
    }
    /// <summary>
    /// Gets page number.
    /// </summary>
    public int Number
    {
      get
      {
        return m_iNumber;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public Page( ISection section, int iNumber )
    {
      m_docSection = section;
      m_pageSetup = section.PageSetup;
      m_headersFooters = section.HeadersFooters;
      m_iNumber = iNumber;
      

      IDocument doc = section.Document;
      m_backgroundImage = doc.BackgroundImage;
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    public void Draw( CustomGraphics g )
    {
      if( m_backgroundImage != null )
      {
        (g as DLSGraphics ).DrawBackgroundImage( Setup, m_backgroundImage );
      }
      
#if DEBUG_PAGEDRAWING
      RectangleF rect = new RectangleF( 
        Setup.Margins.Left, Setup.Margins.Top, 
        Setup.PageSize.Width - ( Setup.Margins.Left + Setup.Margins.Right ), 
        Setup.PageSize.Height - ( Setup.Margins.Top + Setup.Margins.Bottom )  
      );
      ( g as DLSGraphics ).DrawBounds( Color.Magenta, rect );
#endif
      
      for( int i = 0; i < m_pageWidgets.Count; i++ )
      {
        LayoutedWidget widget = m_pageWidgets[ i ];
        
        widget.Draw( g );
#if DEBUG_PAGEDRAWING
        (g as DLSGraphics ).DrawBounds( Color.Yellow, widget.Bounds );
#endif
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="numPages"></param>
    public void UpdateFieldsNumPages( int numPages )
    {
      for( int i = 0; i < m_cachedFields.Count; i++ )
      {
        IField field = m_cachedFields[ i ] as IField;
        
        if( field != null && field.FieldType == DLSFieldType.FieldNumPages )
        {
          //field.Text = string.Format( field.FieldPattern, numPages );
          (field as TextRange).Text = numPages.ToString();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="field"></param>
    public void AddCachedFields( IField field )
    {
      m_cachedFields.Add( field );
    }
    #endregion

    #region Class helper methods
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="layouter"></param>
//    /// <param name="column"></param>
//    /// <param name="prevWidth"></param>
//    /// <param name="cg"></param>
//    /// <returns></returns>
    /*private LayoutedWidget LayoutPageColumn( Layouter layouter, Column column, float prevWidth, CustomGraphics cg )
    {
      RectangleF rect = GetColumnArea( column, prevWidth );
      return layouter.DoLayout( cg, rect );
    }
    */
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="section"></param>
    /*private LayoutedWidget LayoutPageHeader( ISection section, CustomGraphics cg )
    {
      TextBody header = section.HeadersFooters.OddHeader;
      
      if( section.DifferentOddAndEvenPages && ( m_iNumber % 2 ) > 0 )
      {
        header = section.HeadersFooters.EvenHeader;
      }
      
      if( section.DifferentFirstPage && m_bFirstPageForSection )
      {
        header = section.HeadersFooters.FirstPageHeader;
      }
      
      RectangleF rect = GetHeaderArea(); 
      
      Layouter headerLayouter = new Layouter( header );
      return headerLayouter.DoLayout( cg, rect );
    }
    */
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="section"></param>
    /*private LayoutedWidget LayoutPageFooter( ISection section, CustomGraphics cg )
    {
      TextBody footer = section.HeadersFooters.OddFooter;
      
      if( section.DifferentOddAndEvenPages && ( m_iNumber % 2 ) > 0 )
      {
        footer = section.HeadersFooters.EvenFooter;
      }
      
      if( section.DifferentFirstPage && m_bFirstPageForSection )
      {
        footer = section.HeadersFooters.FirstPageFooter;
      }

      RectangleF rect = GetFooterArea();
      
      Layouter footerLayouter = new Layouter( footer );
      LayoutedWidget ltWidget = footerLayouter.DoLayout( cg, rect );
      ltWidget.ShiftLocation( 0, -ltWidget.Bounds.Height );
      return ltWidget;
    }
    */
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal protected RectangleF GetHeaderArea()
    {
      float left = ( m_pageSetup.Margins.Left != -0.05f ) ? m_pageSetup.Margins.Left : 0;
      float right = ( m_pageSetup.Margins.Right != -0.05f ) ? m_pageSetup.Margins.Right : 0;
      float top = ( m_pageSetup.Margins.Top != -0.05f ) ? m_pageSetup.Margins.Top : 0;
      float bottom = ( m_pageSetup.Margins.Bottom != -0.05f ) ? m_pageSetup.Margins.Bottom : 0;
      float hdrDistance = ( m_pageSetup.HeaderDistance != -0.05f ) ? m_pageSetup.HeaderDistance : 0;
      float width = m_pageSetup.PageSize.Width;
      float height = m_pageSetup.PageSize.Height;

      return new RectangleF
        ( left, 
        hdrDistance,
        width - ( left + right ),
        height - ( top + bottom )
        );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal protected RectangleF GetFooterArea()
    {
      float left = ( m_pageSetup.Margins.Left != -0.05f ) ? m_pageSetup.Margins.Left : 0;
      float right = ( m_pageSetup.Margins.Right != -0.05f ) ? m_pageSetup.Margins.Right : 0;
      float top = ( m_pageSetup.Margins.Top != -0.05f ) ? m_pageSetup.Margins.Top : 0;
      float bottom = ( m_pageSetup.Margins.Bottom != -0.05f ) ? m_pageSetup.Margins.Bottom : 0;
      float ftrDistance = ( m_pageSetup.FooterDistance != -0.05f ) ? m_pageSetup.FooterDistance : 0;
      float width = m_pageSetup.PageSize.Width;
      float height = m_pageSetup.PageSize.Height;

      return new RectangleF(
        left,
        height -ftrDistance,
        width - ( left + right ),
        height - ( top + bottom )
        );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="column"></param>
    /// <param name="prevWidth"></param>
    /// <returns></returns>
    internal protected RectangleF GetColumnArea( Column column, float prevWidth )
    {
      MarginsF pageMargins = m_pageSetup.Margins;
      float pageWidth = m_pageSetup.PageSize.Width;
      float pageHeight = m_pageSetup.PageSize.Height;

      float top = ( pageMargins.Top != -0.05f ) ? pageMargins.Top : 0f;
      float left = ( pageMargins.Left != -0.05f ) ? pageMargins.Left : 0f;
      float right = ( pageMargins.Right != -0.05f ) ? pageMargins.Right : 0f;
      float bottom = ( pageMargins.Bottom != -0.05f ) ? pageMargins.Bottom : 0f;

      float headerHeight = m_headersFooters.IsEmpty
        ? 0
        : m_pageWidgets[ 0 ].Bounds.Height
          + ( ( m_pageSetup.HeaderDistance != -0.05f ) ? m_pageSetup.HeaderDistance : 0f );
      float footerHeight = m_headersFooters.IsEmpty
        ? 0
        : m_pageWidgets[ 1 ].Bounds.Height
          + ( ( m_pageSetup.FooterDistance != -0.05f ) ? m_pageSetup.FooterDistance : 0f );
      float columnWidth = ( column == null )
        ? pageWidth - ( left + right )
        : column.Width;

      return new RectangleF
        (
          left + prevWidth,
          Math.Max( top, headerHeight ),
          columnWidth,
          pageHeight -
          (
            Math.Max( top, headerHeight ) +
            Math.Max( bottom, footerHeight )
          )
        );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnIndex"></param>
    /// <param name="prevColumnsWidth"></param>
    /// <returns></returns>
    internal protected RectangleF GetColumnArea( int columnIndex, ref float prevColumnsWidth )
    {
      Column col = ( m_docSection.Columns.Count > columnIndex ) 
                     ? m_docSection.Columns[ columnIndex ]
                     : null;
      
      RectangleF resRect = GetColumnArea( col, prevColumnsWidth );
      
      if( col != null ) prevColumnsWidth += col.Width + col.Space;

      return resRect;
    }
    #endregion
  }
}