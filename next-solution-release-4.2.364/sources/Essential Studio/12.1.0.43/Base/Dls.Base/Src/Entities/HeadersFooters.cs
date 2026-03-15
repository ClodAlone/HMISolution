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

using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents page headers and footers
  /// </summary>
  public class HeadersFooters : XDLSSerializableBase
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private TextBody m_evenHeader = null;
    private TextBody m_oddFooter = null;
    private TextBody m_oddHeader = null;
    private TextBody m_evenFooter = null;
    private TextBody m_firstPageHeader = null;
    private TextBody m_firstPageFooter = null;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets default header.
    /// </summary>
    public TextBody Header
    {
      get
      {
        return OddHeader;
      }
    }
    /// <summary>
    /// Gets default footer.
    /// </summary>
    public TextBody Footer
    {
      get
      {
        return OddFooter;
      }
    }
    /// <summary>
    /// Gets even header.
    /// </summary>
    public TextBody EvenHeader
    {
      get
      {
        return m_evenHeader;
      }
    }
    /// <summary>
    /// Gets odd header ( This is also the default header ).
    /// </summary>
    public TextBody OddHeader
    {
      get
      {
        return m_oddHeader;
      }
    }
    /// <summary>
    /// Gets even footer
    /// </summary>
    public TextBody EvenFooter
    {
      get
      {
        return m_evenFooter;
      }
    }
    /// <summary>
    /// Gets odd footer ( This is also the default footer ).
    /// </summary>
    public TextBody OddFooter
    {
      get
      {
        return m_oddFooter;
      }
    }
    /// <summary>
    /// Gets first page header.
    /// </summary>
    public TextBody FirstPageHeader
    {
      get
      {
        return m_firstPageHeader;
      }
    }
    /// <summary>
    /// Gets first page footer.
    /// </summary>
    public TextBody FirstPageFooter
    {
      get
      {
        return m_firstPageFooter;
      }
    }
    /// <summary>
    /// Detects whether all headers/footers are empty.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return ( m_evenHeader.Paragraphs.Count == 0 && m_evenFooter.Paragraphs.Count == 0 &&
                 m_oddFooter.Paragraphs.Count == 0 && m_oddHeader.Paragraphs.Count == 0 && 
                 m_firstPageFooter.Paragraphs.Count == 0 && m_firstPageHeader.Paragraphs.Count == 0 );
      }
    }
    /// <summary>
    /// Gets TextBody at specified index.
    /// </summary>
    public TextBody this[ int index ]
    {
      get
      {
        if( index < 0 || index > 5 )
          throw new ArgumentOutOfRangeException( "index", "index cann't be less 0 or greater 5" );
        
        return this[ (HeaderFooterType)index ];
      }
    }
    /// <summary>
    /// Gets TextBody by specified HeaderFooter type.
    /// </summary>
    public TextBody this[ HeaderFooterType hfType ]
    {
      get
      {
        switch( hfType )
        {
          case HeaderFooterType.EvenHeader:
            return EvenHeader;
          case HeaderFooterType.OddHeader:
            return OddHeader;
          case HeaderFooterType.EvenFooter:
            return EvenFooter;
          case HeaderFooterType.OddFooter:
            return OddFooter;
          case HeaderFooterType.FirstPageHeader:
            return FirstPageHeader;
          case HeaderFooterType.FirstPageFooter:
            return FirstPageFooter;
        }
        
        throw new ArgumentException( "Invalid header/footer type", "hfType" );
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates HeadersFooters object for specified document.
    /// </summary>
    public HeadersFooters( IDocument doc )
      : base( doc )
    {
      m_evenHeader = DocumentEx.CreateTextBodyImpl();
      m_oddHeader = DocumentEx.CreateTextBodyImpl();
      m_evenFooter = DocumentEx.CreateTextBodyImpl();
      m_oddFooter = DocumentEx.CreateTextBodyImpl();
      m_firstPageFooter = DocumentEx.CreateTextBodyImpl();
      m_firstPageHeader = DocumentEx.CreateTextBodyImpl();
    }
    #endregion

    #region XML serialization overrides
    /// <summary>
    /// 
    /// </summary>
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddElement( XDLSConstants.EvenHeaderTag, EvenHeader );
      XDLSHolder.AddElement( XDLSConstants.OddHeaderTag, OddHeader );
      XDLSHolder.AddElement( XDLSConstants.EvenFooterTag, EvenFooter );
      XDLSHolder.AddElement( XDLSConstants.OddFooterTag, OddFooter );
      XDLSHolder.AddElement( XDLSConstants.FirstPageHeaderTag, FirstPageHeader );
      XDLSHolder.AddElement( XDLSConstants.FirstPageFooterTag, FirstPageFooter );
    }
    #endregion
  }
}