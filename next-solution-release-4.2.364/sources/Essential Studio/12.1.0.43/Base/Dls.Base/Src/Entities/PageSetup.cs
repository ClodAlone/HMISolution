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
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents page Setup settings of a page in a document.
  /// </summary>
  public class PageSetup : XDLSSerializableBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_PAGE_WIDTH = 595;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_PAGE_HEIGHT = 842;
    /// <summary>
    /// 
    /// </summary>
    private const float DEF_PAGE_MARGINS = 20f;
    /// <summary>
    /// 
    /// </summary>
    private const float DEF_PAGE_MARGIN_LEFT = 50f;
    /// <summary>
    /// 
    /// </summary>
    private const float DEF_AUTO_TAB_LENGHT = 36f;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private SizeF m_pageSize;
    private PageOrientation m_orientation;
    private MarginsF m_margins;
    protected float m_fHeaderDistance;
    protected float m_fFooterDistance;
    private PageAlignment m_vertAlignment;
    /// <summary>
    /// The title page mark
    /// </summary>
    private bool m_titlePage = false;
    /// <summary>
    /// True if the document has different headers and footers 
    /// for odd-numbered and even-numbered pages. 
    /// </summary>
    private bool m_oddEvenHeader = false;
    /// <summary>
    /// 
    /// </summary>
    private float m_fDefaultTabWidth = DEF_AUTO_TAB_LENGHT;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets the length of the auto tab.
    /// </summary>
    /// <value>The length of the auto tab.</value>
    public float DefaultTabWidth
    {
      get
      {
        return m_fDefaultTabWidth;
      }
      set
      {
        if( value != m_fDefaultTabWidth )
        {
          m_fDefaultTabWidth = value;
        }
      }
    }
    /// <summary>
    /// Gets /sets page size in points.
    /// </summary>
    public SizeF PageSize
    {
      get
      {
        return m_pageSize;
      }
      set
      {
        m_pageSize = value;
      }
    }
    /// <summary>
    /// Gets / sets orientation of a page.
    /// </summary>
    public PageOrientation Orientation
    {
      get
      {
        return m_orientation;
      }
      set
      {
        if( m_orientation != value)
        {
          m_orientation = value;
          switch ( value )
          {
            case PageOrientation.Portrait:
              if( PageSize.Width > PageSize.Height )
              {
                PageSize = new SizeF( PageSize.Height, PageSize.Width );
              }
              break;
            case PageOrientation.Landscape:
              if ( PageSize.Height > PageSize.Width )
              {
                PageSize = new SizeF( PageSize.Height, PageSize.Width );
              }
              break;
          }
          //PageSize = new SizeF( PageSize.Height, PageSize.Width );
        }
      }
    }
    /// <summary>
    /// Gets / sets vertical alignment.
    /// </summary>
    /// <remarks>Not supported by Essential DPF</remarks>
    public PageAlignment VerticalAlignment
    {
      get
      {
        return m_vertAlignment;
      }
      set
      {
        m_vertAlignment = value;
      }
    }
    /// <summary>
    /// Gets / sets page margins in points.
    /// </summary>
    public MarginsF Margins
    {
      get
      {
        if( m_margins == null )
        {
          m_margins = new MarginsF();
        }
        return m_margins;
      }
      set
      {
        m_margins = value;
      }
    }
    /// <summary>
    /// Gets / sets height of header in points.
    /// </summary>
    /// <remarks>Not supported by Essential DPF</remarks>
    public float HeaderDistance
    {
      get
      {
        return m_fHeaderDistance;
      }
      set
      {
        m_fHeaderDistance = value;
      }
    }
    /// <summary>
    /// Gets / sets footer height in points.
    /// </summary>
    /// <remarks>Not supported by Essential DPF</remarks>
    public float FooterDistance
    {
      get
      {
        return m_fFooterDistance;
      }
      set
      {
        m_fFooterDistance = value;
      }
    }
    /// <summary>
    /// Gets width of client area.
    /// </summary>
    public float ClientWidth
    {
      get
      {
        return PageSize.Width - Margins.Left - Margins.Right;
      }
    }
    /// <summary>
    /// Setting to specify that the current section has a different header/footer for first page.
    /// </summary>
    public bool DifferentFirstPage
    {
      get
      {
        return m_titlePage;
      }
      set
      {
        m_titlePage = value;
      }
    }
    /// <summary>
    /// True if the document has different headers and footers 
    /// for odd-numbered and even-numbered pages. 
    /// </summary>
    public bool DifferentOddAndEvenPages
    {
      get
      {
        return m_oddEvenHeader;
      }
      set
      {
        m_oddEvenHeader = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates PageSetup object for specified document.
    /// </summary>
    /// <param name="doc"></param>
    public PageSetup( IDocument doc )
      : base( doc )
    {
      m_pageSize = new SizeF( DEF_PAGE_WIDTH, DEF_PAGE_HEIGHT );
      m_margins = new MarginsF();
      m_margins.All = DEF_PAGE_MARGINS;
      m_margins.Left = DEF_PAGE_MARGIN_LEFT;
    }
    #endregion

    #region Xml serialization overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      
      if( DefaultTabWidth != DEF_AUTO_TAB_LENGHT )
      {
        writer.WriteValue( XDLSConstants.AutoTabWidthAttr, DefaultTabWidth );
      }
      if( PageSize.Height != 0 )
      {
        writer.WriteValue( XDLSConstants.PSPageHeightAttr, PageSize.Height );
      }
      if( PageSize.Width != 0 )
      {
        writer.WriteValue( XDLSConstants.PSPageWidthAttr, PageSize.Width );
      }
      if( VerticalAlignment != 0 )
      {
        writer.WriteValue( XDLSConstants.PSAlignmentAttr, VerticalAlignment );
      }
      if( FooterDistance >= 0 )
      {
        writer.WriteValue( XDLSConstants.PSFooterDistanceAttr, FooterDistance );
      }
      if( HeaderDistance >= 0 )
      {
        writer.WriteValue( XDLSConstants.PSHeaderDistanceAttr, HeaderDistance );
      }
      if( Orientation != 0 )
      {
        writer.WriteValue( XDLSConstants.PSOrientationAttr, Orientation );
      }
      if( Margins.Bottom >= 0 )
      {
        writer.WriteValue( XDLSConstants.PSBottomMarginAttr, Margins.Bottom );
      }
      if( Margins.Top >= 0 )
      {
        writer.WriteValue( XDLSConstants.PSTopMarginAttr, Margins.Top );
      }
      if( Margins.Left >= 0 )
      {
        writer.WriteValue( XDLSConstants.PSLeftMarginAttr, Margins.Left );
      }
      if( Margins.Right >= 0 )
      {
        writer.WriteValue( XDLSConstants.PSRightMarginAttr, Margins.Right );
      }
      
      if( DifferentFirstPage )
      {
        writer.WriteValue( XDLSConstants.PageSetupFirstPageAttr, DifferentFirstPage );
      }
      
      if( DifferentOddAndEvenPages )
      {
        writer.WriteValue( XDLSConstants.PageSetupDiffOddEvenPagesAttr, DifferentOddAndEvenPages );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
      
      if( reader.HasAttribute( XDLSConstants.AutoTabWidthAttr ) )
      {
        DefaultTabWidth = reader.ReadFloat( XDLSConstants.AutoTabWidthAttr );
      }
      if( reader.HasAttribute( XDLSConstants.PSPageHeightAttr ) )
      {
        PageSize = new SizeF( PageSize.Width, reader.ReadFloat( XDLSConstants.PSPageHeightAttr ) );
      }
      if( reader.HasAttribute( XDLSConstants.PSPageWidthAttr ) )
      {
        PageSize = new SizeF( reader.ReadFloat( XDLSConstants.PSPageWidthAttr ), PageSize.Height );
      }
      if( reader.HasAttribute( XDLSConstants.PSAlignmentAttr ) )
      {
        VerticalAlignment = ( PageAlignment )reader.ReadEnum( XDLSConstants.PSAlignmentAttr, typeof( PageAlignment ) );
      }
      if( reader.HasAttribute( XDLSConstants.PSFooterDistanceAttr ))
      {
        FooterDistance = reader.ReadFloat( XDLSConstants.PSFooterDistanceAttr );
      }
      if( reader.HasAttribute( XDLSConstants.PSHeaderDistanceAttr ))
      {
        HeaderDistance = reader.ReadFloat( XDLSConstants.PSHeaderDistanceAttr );
      }
      if( reader.HasAttribute( XDLSConstants.PSOrientationAttr ))
      {
        Orientation = ( PageOrientation )reader.ReadEnum( XDLSConstants.PSOrientationAttr, typeof( PageOrientation ) );
      }
      if( reader.HasAttribute( XDLSConstants.PSBottomMarginAttr ))
      {
        Margins.Bottom = reader.ReadFloat( XDLSConstants.PSBottomMarginAttr );
      }
      if( reader.HasAttribute( XDLSConstants.PSTopMarginAttr ))
      {
        Margins.Top = reader.ReadFloat( XDLSConstants.PSTopMarginAttr );
      }
      if( reader.HasAttribute( XDLSConstants.PSLeftMarginAttr ))
      {
        Margins.Left = reader.ReadFloat( XDLSConstants.PSLeftMarginAttr );
      }
      if( reader.HasAttribute( XDLSConstants.PSRightMarginAttr ))
      {
        Margins.Right = reader.ReadFloat( XDLSConstants.PSRightMarginAttr );
      }
      if( reader.HasAttribute( XDLSConstants.PageSetupFirstPageAttr ) )
      {
        DifferentFirstPage = reader.ReadBoolean( XDLSConstants.PageSetupFirstPageAttr );
      }
      
      if( reader.HasAttribute( XDLSConstants.PageSetupDiffOddEvenPagesAttr ) )
      {
        DifferentOddAndEvenPages = reader.ReadBoolean( XDLSConstants.PageSetupDiffOddEvenPagesAttr );
      }
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Clones itself.
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public virtual PageSetup Clone( IDocument doc )
    {
      PageSetup pSett = DocumentEx.CreatePageSetupImpl();
      pSett.VerticalAlignment = VerticalAlignment;
      pSett.FooterDistance = FooterDistance;
      pSett.HeaderDistance = HeaderDistance;
      pSett.Margins = Margins.Clone();
      pSett.Orientation = Orientation;
      pSett.PageSize = PageSize;
      return pSett;
    }
    #endregion
  }
}