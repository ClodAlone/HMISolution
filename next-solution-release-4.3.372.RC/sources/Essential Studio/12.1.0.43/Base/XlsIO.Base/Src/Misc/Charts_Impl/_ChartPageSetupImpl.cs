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
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Interfaces.Charts;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Class allows user to configure Print setting of chart.
  /// </summary>
  public class ChartPageSetupImpl
    : CommonObject
    , IChartPageSetup
  {
    #region Class constants
    /// <summary>
    /// Value of the top margin by default
    /// </summary>
    public const double DEFAULT_TOPMARGIN = 1;
    /// <summary>
    /// Value of the bottom margin by default
    /// </summary>
    public const double DEFAULT_BOTTOMMARGIN = 1;
    /// <summary>
    /// Value of the left margin by default
    /// </summary>
    public const double DEFAULT_LEFTMARGIN = 0.75;
    /// <summary>
    /// Value of the right margin by default
    /// </summary>
    public const double DEFAULT_RIGHTMARGIN = 0.75;
    /// <summary>
    /// Set indexes as constants
    /// </summary>
    private enum THeaderSide : int
    {
      /// <summary>
      /// Left part of header or footer formatting
      /// </summary>
      Left      = 0,
      /// <summary>
      /// Center part of header or footer formatting
      /// </summary>
      Center    = 1,
      /// <summary>
      /// Right part of header or footer formatting
      /// </summary>
      Right     = 2,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Specifies a header for a sheet
    /// </summary>
    private HeaderRecord            m_Header;
    /// <summary>
    /// Specifies a footer for a sheet
    /// </summary>
    private FooterRecord            m_Footer;
    /// <summary>
    /// Whether to center between horizontal margins
    /// </summary>
    private HCenterRecord           m_HCenter;
    /// <summary>
    /// Whether to center between vertical margins
    /// </summary>
    private VCenterRecord           m_VCenter;
    /// <summary>
    /// Unknown record. This record by it hex dump seems contains 
    /// additional information about system printer. If we find such record then
    /// store it otherwise we skip it.
    /// </summary>
    private UnknownRecord           m_Unknown;
    /// <summary>
    /// Stores print setup options
    /// </summary>
    private PrintSetupRecord        m_Setup;
    /// <summary>
    /// This record contains information about worksheet bottom margin
    /// </summary>
    private BottomMarginRecord      m_BottomMargin;
    /// <summary>
    /// This record contains information about worksheet left margin
    /// </summary>
    private LeftMarginRecord        m_LeftMargin;
    /// <summary>
    /// This record contains information about worksheet right margin
    /// </summary>
    private RightMarginRecord       m_RightMargin;
    /// <summary>
    /// This record contains information about worksheet top margin
    /// </summary>
    private TopMarginRecord         m_TopMargin;
    /// <summary>
    /// Array of headers: 0 - left header, 1 - center header, 2 - right header
    /// </summary>
    private string[] m_arrHeaders = new string[] { string.Empty, string.Empty, string.Empty };
    /// <summary>
    /// Array of footers: 0 - left footer, 1 - center footer, 2 - right footer
    /// </summary>
    private string[] m_arrFooters = new string[] { string.Empty, string.Empty, string.Empty };
    /// <summary>
    /// 
    /// </summary>
    private PrintedChartSizeRecord m_chartSize;
    #endregion

    #region IPageSetup Members
    /// <summary>
    /// True if elements of the document will be printed in black and white.
    /// Read/write Boolean.
    /// </summary>
    public bool   BlackAndWhite
    {
      get
      {
        return m_Setup.IsNoColor;
      }
      set
      {
        if( m_Setup.IsNoColor != value )
        {
          m_Setup.IsNoColor = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the size of the bottom margin, in points.
    /// Read/write Double
    /// </summary>
    public double BottomMargin
    {
      get
      {
        return ( m_BottomMargin != null ) ? m_BottomMargin.BottomMargin :
          DEFAULT_BOTTOMMARGIN;
      }
      set
      {
        if( m_BottomMargin == null && value != DEFAULT_BOTTOMMARGIN )
        {
          m_BottomMargin = BiffRecordFactory.GetRecord( TBIFFRecord.BottomMargin ) as
            BottomMarginRecord;
          SetChanged();
        }

        if( m_BottomMargin != null && m_BottomMargin.BottomMargin != value )
        {
          m_BottomMargin.BottomMargin = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the center part of the footer. Read/write String
    /// Special Formatting symbols:
    /// &P Current page number
    /// &N Page count
    /// &D Current date
    /// &T Current time
    /// &A Sheet name (BIFF5-BIFF8)
    /// &F File name without path
    /// &Z File path without file name (BIFF8X)
    /// &G Picture (BIFF8X)
    /// &B Bold on/off (BIFF2-BIFF4)
    /// &I Italic on/off (BIFF2-BIFF4)
    /// &U Underlining on/off
    /// &E Double underlining on/off (BIFF5-BIFF8)
    /// &S Strikeout on/off
    /// &X Superscript on/off (BIFF5-BIFF8)
    /// &Y Subscript on/off (BIFF5-BIFF8)
    /// &"[FONTNAME]" Set new font [FONTNAME]
    /// &"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE]. 
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic, 
    /// or Bold Italic. But this setting is dependent on the used font, it may 
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value). 
    /// If this command is followed by a plain number to be printed in the header, 
    /// it will be separated from the font height with a space character.
    /// </summary>
    public string CenterFooter
    {
      get
      {
        return m_arrFooters[ (int)THeaderSide.Center ];
      }
      set
      {
        if( m_arrFooters[ (int)THeaderSide.Center ] != value )
        {
          m_arrFooters[ (int)THeaderSide.Center ] = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the center part of the header. Read/write String.
    /// Special Formatting symbols:
    /// &P Current page number
    /// &N Page count
    /// &D Current date
    /// &T Current time
    /// &A Sheet name (BIFF5-BIFF8)
    /// &F File name without path
    /// &Z File path without file name (BIFF8X)
    /// &G Picture (BIFF8X)
    /// &B Bold on/off (BIFF2-BIFF4)
    /// &I Italic on/off (BIFF2-BIFF4)
    /// &U Underlining on/off
    /// &E Double underlining on/off (BIFF5-BIFF8)
    /// &S Strikeout on/off
    /// &X Superscript on/off (BIFF5-BIFF8)
    /// &Y Subscript on/off (BIFF5-BIFF8)
    /// &"[FONTNAME]" Set new font [FONTNAME]
    /// &"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE]. 
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic, 
    /// or Bold Italic. But this setting is dependent on the used font, it may 
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value). 
    /// If this command is followed by a plain number to be printed in the header, 
    /// it will be separated from the font height with a space character.
    /// </summary>
    public string CenterHeader
    {
      get
      {
        return m_arrHeaders[ (int)THeaderSide.Center ];
      }
      set
      {
        if( m_arrHeaders[ (int)THeaderSide.Center ] != value )
        {
          m_arrHeaders[ (int)THeaderSide.Center ] = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// True if the sheet is centered horizontally on the page when it's
    /// printed. Read/write Boolean.
    /// </summary>
    public bool   CenterHorizontally
    {
      get
      {
        return ( m_HCenter.IsHCenter != 0 );
      }
      set
      {
        ushort newValue = ( value ) ? ( ushort ) 1 : ( ushort ) 0;
        if( m_HCenter.IsHCenter != newValue )
        {
          m_HCenter.IsHCenter = newValue;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// True if the sheet is centered vertically on the page when it's
    /// printed. Read/write Boolean.
    /// </summary>
    public bool   CenterVertically
    {
      get
      {
        return ( m_VCenter.IsVCenter != 0 );
      }
      set
      {
        ushort newValue = value ? ( ushort ) 1 : ( ushort ) 0;
        if( m_VCenter.IsVCenter != newValue )
        {
          m_VCenter.IsVCenter = newValue;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Number of copies to print
    /// </summary>
    public int    Copies
    {
      get
      {
        return m_Setup.Copies;
      }
      set
      {
        if( m_Setup.Copies != (ushort) value )
        {
          m_Setup.Copies = (ushort) value;
          m_Setup.IsNotValidSettings = false;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// True if the sheet will be printed without graphics.
    /// Read/write Boolean.
    /// </summary>
    public bool   Draft
    {
      get
      {
        return m_Setup.IsDraft;
      }
      set
      {
        if( m_Setup.IsDraft != value )
        {
          m_Setup.IsDraft = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the first page number that will be used when
    /// this sheet is printed. If xlAutomatic, Excel chooses the
    /// first page number. The default is xlAutomatic. Read/write Long.
    /// </summary>
    public int    FirstPageNumber
    {
      get
      {
        return m_Setup.PageStart;
      }
      set
      {
        if( m_Setup.PageStart != ( ushort ) value )
        {
          m_Setup.PageStart = ( ushort ) value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the number of pages tall the worksheet will be scaled
    /// to when it's printed. Applies only to worksheets. Read/write Boolean.
    /// </summary>
    public bool   FitToPagesTall
    {
      get
      {
        return ( m_Setup.FitHeight != 0 );
      }
      set
      {
        ushort newValue = value ? (ushort) 1 : (ushort) 0;
        if( m_Setup.FitHeight != newValue )
        {
          m_Setup.FitHeight = newValue;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the number of pages wide the worksheet will be scaled
    /// to when it's printed. Applies only to worksheets. Read/write Boolean.
    /// </summary>
    public bool   FitToPagesWide
    {
      get
      {
        return ( m_Setup.FitWidth != 0 );
      }
      set
      {
        ushort newValue = value ? (ushort) 1 : (ushort) 0;
        if( m_Setup.FitWidth != newValue )
        {
          m_Setup.FitWidth = newValue;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the distance from the bottom of the page to the footer,
    /// in points. Read/write Double.
    /// </summary>
    public double FooterMargin
    {
      get
      {
        return m_Setup.FooterMargin;
      }
      set
      {
        if( m_Setup.FooterMargin != value )
        {
          m_Setup.FooterMargin = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the distance from the top of the page to the header,
    /// in points. Read/write Double.
    /// </summary>
    public double HeaderMargin
    {
      get
      {
        return m_Setup.HeaderMargin;
      }
      set
      {
        if( m_Setup.HeaderMargin != value )
        {
          m_Setup.HeaderMargin = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the left part of the footer. Read/write String.
    /// Special Formatting symbols:
    /// &P Current page number
    /// &N Page count
    /// &D Current date
    /// &T Current time
    /// &A Sheet name (BIFF5-BIFF8)
    /// &F File name without path
    /// &Z File path without file name (BIFF8X)
    /// &G Picture (BIFF8X)
    /// &B Bold on/off (BIFF2-BIFF4)
    /// &I Italic on/off (BIFF2-BIFF4)
    /// &U Underlining on/off
    /// &E Double underlining on/off (BIFF5-BIFF8)
    /// &S Strikeout on/off
    /// &X Superscript on/off (BIFF5-BIFF8)
    /// &Y Subscript on/off (BIFF5-BIFF8)
    /// &"[FONTNAME]" Set new font [FONTNAME]
    /// &"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE]. 
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic, 
    /// or Bold Italic. But this setting is dependent on the used font, it may 
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value). 
    /// If this command is followed by a plain number to be printed in the header, 
    /// it will be separated from the font height with a space character.
    /// </summary>
    public string LeftFooter
    {
      get
      {
        return m_arrFooters[ (int)THeaderSide.Left ];
      }
      set
      {
        if( m_arrFooters[ (int)THeaderSide.Left ] != value )
        {
          m_arrFooters[ (int)THeaderSide.Left ] = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the left part of the header. Read/write String.
    /// Special Formatting symbols:
    /// &P Current page number
    /// &N Page count
    /// &D Current date
    /// &T Current time
    /// &A Sheet name (BIFF5-BIFF8)
    /// &F File name without path
    /// &Z File path without file name (BIFF8X)
    /// &G Picture (BIFF8X)
    /// &B Bold on/off (BIFF2-BIFF4)
    /// &I Italic on/off (BIFF2-BIFF4)
    /// &U Underlining on/off
    /// &E Double underlining on/off (BIFF5-BIFF8)
    /// &S Strikeout on/off
    /// &X Superscript on/off (BIFF5-BIFF8)
    /// &Y Subscript on/off (BIFF5-BIFF8)
    /// &"[FONTNAME]" Set new font [FONTNAME]
    /// &"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE]. 
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic, 
    /// or Bold Italic. But this setting is dependent on the used font, it may 
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value). 
    /// If this command is followed by a plain number to be printed in the header, 
    /// it will be separated from the font height with a space character.
    /// </summary>
    public string LeftHeader
    {
      get
      {
        return m_arrHeaders[ (int)THeaderSide.Left ];
      }
      set
      {
        if( m_arrHeaders[ (int)THeaderSide.Left ] != value )
        {
          m_arrHeaders[ (int)THeaderSide.Left ] = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the size of the left margin, in points.
    /// Read/write Double.
    /// </summary>
    public double LeftMargin
    {
      get
      {
        return ( m_LeftMargin != null ) ? m_LeftMargin.LeftMargin : DEFAULT_LEFTMARGIN;
      }
      set
      {
        if( m_LeftMargin == null && value != DEFAULT_LEFTMARGIN )
        {
          m_LeftMargin = BiffRecordFactory.GetRecord( TBIFFRecord.LeftMargin ) as
            LeftMarginRecord;
          SetChanged();
        }

        if( m_LeftMargin != null && m_LeftMargin.LeftMargin != value )
        {
          m_LeftMargin.LeftMargin = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the order that Excel uses to number
    /// pages when printing a large worksheet. Read/write ExcelOrder
    /// </summary>
    public ExcelOrder Order
    {
      get
      {
        return m_Setup.IsLeftToRight ? ExcelOrder.OverThenDown : ExcelOrder.DownThenOver;
      }
      set
      {
        bool newValue = value == ( ExcelOrder.OverThenDown );
        if( m_Setup.IsLeftToRight != newValue )
        {
          m_Setup.IsLeftToRight = newValue;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Portrait or landscape printing mode. Read/write ExcelPageOrientation
    /// </summary>
    public ExcelPageOrientation Orientation
    {
      get
      {
        return m_Setup.IsNotLandscape ? ExcelPageOrientation.Portrait : ExcelPageOrientation.Landscape;
      }
      set
      {
        m_Setup.IsNotLandscape = ( value == ExcelPageOrientation.Portrait );
        m_Setup.IsNotValidSettings = false;
        m_Setup.IsNoOrientation = false;
        SetChanged();
      }
    }

    /// <summary>
    /// Returns or sets the size of the paper. Read/write ExcelPaperSize
    /// </summary>
    public ExcelPaperSize PaperSize
    {
      get
      {
        return ( ExcelPaperSize ) m_Setup.PaperSize;
      }
      set
      {
        m_Setup.PaperSize = (ushort) value;
        m_Setup.IsNotValidSettings = false;
        SetChanged();
      }
    }

    /// <summary>
    /// Returns or sets the way comments are printed with the sheet.
    /// Read/write ExcelPrintLocation.
    /// </summary>
    public ExcelPrintLocation PrintComments
    {
      get
      {
        if( m_Setup.IsNotes == false )
        {
          return ExcelPrintLocation.PrintNoComments;
        }
        else if( m_Setup.IsPrintNotesAsDisplayed )
        {
          return ExcelPrintLocation.PrintInPlace;
        }
        else return ExcelPrintLocation.PrintSheetEnd;
      }
      set
      {
        switch( value )
        {
          case ExcelPrintLocation.PrintNoComments:
            m_Setup.IsNotes = false;
            break;
          
          case ExcelPrintLocation.PrintInPlace:
            m_Setup.IsNotes = true;
            m_Setup.IsPrintNotesAsDisplayed = true;
            break;
          
          case ExcelPrintLocation.PrintSheetEnd:
            m_Setup.IsNotes = true;
            m_Setup.IsPrintNotesAsDisplayed = false;
            break;
        }

        SetChanged();
      }
    }

    /// <summary>
    /// Sets or returns an ExcelPrintErrors contstant specifying the type of
    /// print error displayed. This feature allows users to suppress the
    /// display of error values when printing a worksheet. Read/write.
    /// </summary>
    public ExcelPrintErrors PrintErrors
    {
      get
      {
        return m_Setup.PrintErrors;
      }
      set
      {
        if( m_Setup.PrintErrors != value )
        {
          m_Setup.PrintErrors = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// True if cell notes are printed as end notes with the sheet. Applies
    /// only to worksheets. Read/write Boolean.
    /// </summary>
    public bool   PrintNotes
    {
      get
      {
        return m_Setup.IsNotes;
      }
      set
      {
        if( m_Setup.IsNotes != value )
        {
          m_Setup.IsNotes = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the print quality. Read/write ushort
    /// </summary>
    public int    PrintQuality
    {
      get
      {
        return m_Setup.HResolution;
      }
      set
      {
        m_Setup.HResolution = (ushort) value;
        m_Setup.VResolution = (ushort) value;
        m_Setup.IsNotValidSettings = false;
        
        SetChanged();
      }
    }

    /// <summary>
    /// Returns or sets the right part of the footer. Read/write String
    /// Special Formatting symbols:
    /// &P Current page number
    /// &N Page count
    /// &D Current date
    /// &T Current time
    /// &A Sheet name (BIFF5-BIFF8)
    /// &F File name without path
    /// &Z File path without file name (BIFF8X)
    /// &G Picture (BIFF8X)
    /// &B Bold on/off (BIFF2-BIFF4)
    /// &I Italic on/off (BIFF2-BIFF4)
    /// &U Underlining on/off
    /// &E Double underlining on/off (BIFF5-BIFF8)
    /// &S Strikeout on/off
    /// &X Superscript on/off (BIFF5-BIFF8)
    /// &Y Subscript on/off (BIFF5-BIFF8)
    /// &"[FONTNAME]" Set new font [FONTNAME]
    /// &"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE]. 
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic, 
    /// or Bold Italic. But this setting is dependent on the used font, it may 
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value). 
    /// If this command is followed by a plain number to be printed in the header, 
    /// it will be separated from the font height with a space character.
    /// </summary>
    public string RightFooter
    {
      get
      {
        return m_arrFooters[ (int)THeaderSide.Right ];
      }
      set
      {
        if( m_arrFooters[ (int)THeaderSide.Right ] != value )
        {
          m_arrFooters[ (int)THeaderSide.Right ] = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the right part of the header. Read/write String
    /// Special Formatting symbols:
    /// &P Current page number
    /// &N Page count
    /// &D Current date
    /// &T Current time
    /// &A Sheet name (BIFF5-BIFF8)
    /// &F File name without path
    /// &Z File path without file name (BIFF8X)
    /// &G Picture (BIFF8X)
    /// &B Bold on/off (BIFF2-BIFF4)
    /// &I Italic on/off (BIFF2-BIFF4)
    /// &U Underlining on/off
    /// &E Double underlining on/off (BIFF5-BIFF8)
    /// &S Strikeout on/off
    /// &X Superscript on/off (BIFF5-BIFF8)
    /// &Y Subscript on/off (BIFF5-BIFF8)
    /// &"[FONTNAME]" Set new font [FONTNAME]
    /// &"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE]. 
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic, 
    /// or Bold Italic. But this setting is dependent on the used font, it may 
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value). 
    /// If this command is followed by a plain number to be printed in the header, 
    /// it will be separated from the font height with a space character.
    /// </summary>
    public string RightHeader
    {
      get
      {
        return m_arrHeaders[ (int)THeaderSide.Right ];
      }
      set
      {
        if( m_arrHeaders[ (int)THeaderSide.Right ] != value )
        {
          m_arrHeaders[ (int)THeaderSide.Right ] = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the size of the right margin, in points.
    /// Read/write Double.
    /// </summary>
    public double RightMargin
    {
      get
      {
        return ( m_RightMargin != null ) ? m_RightMargin.RightMargin : DEFAULT_RIGHTMARGIN;
      }
      set
      {
        if( m_RightMargin == null && value != DEFAULT_RIGHTMARGIN )
        {
          m_RightMargin = BiffRecordFactory.GetRecord( TBIFFRecord.RightMargin ) as
            RightMarginRecord;
          SetChanged();
        }

        if( m_RightMargin != null && m_RightMargin.RightMargin != value )
        {
          m_RightMargin.RightMargin = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets the size of the top margin, in points.
    /// Read/write Double.
    /// </summary>
    public double TopMargin
    {
      get
      {
        return ( m_TopMargin != null ) ? m_TopMargin.TopMargin : DEFAULT_TOPMARGIN;
      }
      set
      {
        if( m_TopMargin == null && value != DEFAULT_TOPMARGIN )
        {
          m_TopMargin = BiffRecordFactory.GetRecord( TBIFFRecord.TopMargin ) as
            TopMarginRecord;
          SetChanged();
        }

        if( m_TopMargin != null && m_TopMargin.TopMargin != value )
        {
          m_TopMargin.TopMargin = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Returns or sets a percentage (between 10 and 400 percent) by which
    /// Excel will scale the worksheet for printing. Applies only
    /// to worksheets. Read/write ushort.
    /// </summary>
    public int   Zoom
    {
      get
      {
        return m_Setup.Scale;
      }
      set
      {
        m_Setup.Scale = (ushort) value;
        m_Setup.IsNotValidSettings = false;
        SetChanged();
      }
    }

    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Sets application and parent fields
    /// </summary>
    /// <param name="application">Application object for the page setup</param>
    /// <param name="parent">Parent object for the page setup</param>
    public ChartPageSetupImpl( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();

      m_Header  = (HeaderRecord) BiffRecordFactory.GetRecord( TBIFFRecord.Header );
      m_Footer  = (FooterRecord) BiffRecordFactory.GetRecord( TBIFFRecord.Footer );
      m_HCenter = (HCenterRecord) BiffRecordFactory.GetRecord( TBIFFRecord.HCenter );
      m_VCenter = (VCenterRecord) BiffRecordFactory.GetRecord( TBIFFRecord.VCenter );
      m_Setup   = (PrintSetupRecord) BiffRecordFactory.GetRecord( TBIFFRecord.PrintSetup );
      m_chartSize = (PrintedChartSizeRecord)
        BiffRecordFactory.GetRecord( TBIFFRecord.PrintedChartSize );
    }
    /// <summary>
    /// Recovers page setup from the stream and sets its application and parent fields
    /// current record in the stream must be PrintHeadersRecord
    /// </summary>
    /// <param name="application">Application object for the page setup</param>
    /// <param name="parent">Parent object for the page setup</param>
    /// <param name="reader">BiffReader that contains page setup records</param>
    public ChartPageSetupImpl( IApplication application, object parent
      , BiffReader reader )
      : base( application, parent )
    {
      FindParents();
      Parse( reader );
    }
    /// <summary>
    /// Recovers Page setup from the Biff Records array starting from position
    /// </summary>
    /// <param name="application">Application object for the page setup</param>
    /// <param name="parent">Parent object for the page setup</param>
    /// <param name="data">array of Biff Records that contains all needed records</param>
    /// <param name="position">position of PrintHeadersRecord in the array</param>
    public ChartPageSetupImpl( IApplication application, object parent
      , BiffRecordRaw[] data, ref int position )
      : base( application, parent )
    {
      FindParents();
      Parse( data, ref position );
    }
    /// <summary>
    /// Recovers Page setup from the Biff Records ArrayList starting from position
    /// </summary>
    /// <param name="application">Application object for the page setup</param>
    /// <param name="parent">Parent object for the page setup</param>
    /// <param name="data">ArrayList which contains Biff Records</param>
    /// <param name="position">position of PrintHeadersRecord in the array</param>
    public ChartPageSetupImpl( IApplication application, object parent
      , ArrayList data, ref int position )
      : base( application, parent )
    {
      FindParents();
      BiffRecordRaw[] dt =( BiffRecordRaw[] )data.ToArray( typeof( BiffRecordRaw ) );
      Parse( dt, ref position );
    }
    /// <summary>
    /// Find parent Worksheet.
    /// </summary>
    /// <exception cref="System.ArgumentException">
    /// When the parent worksheet cannot be found.
    /// </exception>
    private void FindParents()
    {
//      object result = FindParent( typeof( WorksheetImpl ) );
//      if( result == null )
//        throw new ArgumentException( "PageSetup class must be a leaf of Worksheet object tree" );
//      m_worksheet = ( WorksheetImpl )result;
    }
    #endregion

    #region Class reader methods
    /// <summary>
    /// Recovers Page setup from the Biff Records array starting from position
    /// </summary>
    /// <param name="data">Biff Records data</param>
    /// <param name="position">position of first PageSetup record - PrintHeadersRecord</param>
    public void Parse( BiffRecordRaw[] data, ref int position )
    {
      m_Header          = ( HeaderRecord )GetRecordUpdatePos( data, ref position, TBIFFRecord.Header );
      m_Footer          = ( FooterRecord )GetRecordUpdatePos( data, ref position, TBIFFRecord.Footer );
      m_HCenter         = ( HCenterRecord )GetRecordUpdatePos( data, ref position, TBIFFRecord.HCenter );
      m_VCenter         = ( VCenterRecord )GetRecordUpdatePos( data, ref position, TBIFFRecord.VCenter );

      while( GetNextMargin( data, ref position ) ){}

      BiffRecordRaw raw = GetRecordUpdatePos( data, ref position );

      if( raw is UnknownRecord )
      {
        m_Unknown = ( UnknownRecord ) raw;
        m_Setup = ( PrintSetupRecord ) GetRecordUpdatePos( data, ref position );
      }
      else
      {
        m_Setup = ( PrintSetupRecord ) raw;
      }

      m_chartSize = (PrintedChartSizeRecord) GetRecordUpdatePos( data, ref position );

      // Should split header and footer onto three sections left, center, and right
      m_arrHeaders = ParseHeaderFooterString( m_Header.Header );
      m_arrFooters = ParseHeaderFooterString( m_Footer.Footer );
    }
    /// <summary>
    /// Recovers page setup from the stream, first record must be PrintHeadersRecord
    /// </summary>
    /// <param name="reader">Stream that contains all needed records</param>
    public void Parse( BiffReader reader )
    {
      m_Header = ( HeaderRecord ) reader.GetRecord();
      m_Footer = ( FooterRecord ) reader.GetRecord();
      m_HCenter = ( HCenterRecord ) reader.GetRecord();
      m_VCenter = ( VCenterRecord ) reader.GetRecord();

      BiffRecordRaw record = reader.GetRecord();

      // Get margins from the stream
      // there can be any of them or none
      while( GetNextMargin( reader, ref record ) );

      if( record is UnknownRecord ) m_Unknown = ( UnknownRecord )record;
      m_Setup = ( PrintSetupRecord )reader.GetRecord();
      m_chartSize = (PrintedChartSizeRecord) reader.GetRecord();

      // Split header and footer onto three sections left, center, and right
      m_arrHeaders = ParseHeaderFooterString( m_Header.Header );
      m_arrFooters = ParseHeaderFooterString( m_Footer.Footer );

    }
    /// <summary>
    /// This function splits header or footer onto three parts left, center and right
    /// </summary>
    /// <param name="strToSplit">
    /// Header (footer) string that will be splitted into
    /// parts: left, center, right
    /// </param>
    /// <returns>Array of splitted strings</returns>
    public string[] ParseHeaderFooterString( string strToSplit )
    {
      string[] arrOutput = new string[] { string.Empty, string.Empty, string.Empty };

      int iLStart = strToSplit.IndexOf( "&L" );
      int iCStart = strToSplit.IndexOf( "&C" );
      int iRStart = strToSplit.IndexOf( "&R" );

      // if nothing found then return empty array
      if( iLStart == iCStart && 
          iCStart == iRStart && 
          iRStart == -1 )
        return arrOutput;

      // left part of the header
      if( iLStart >= 0 )
      {
        int end = strToSplit.Length;

        if( iCStart > iLStart )
          end = iCStart;
        else if( iRStart > iLStart )
          end = iRStart;

        arrOutput[ (int)THeaderSide.Left ] = 
          strToSplit.Substring( iLStart + 2, end - iLStart - 2 );
      }

      // center part of header 
      if( iCStart >= 0 )
      {
        int end = strToSplit.Length;

        if( iRStart > iCStart ) end = iRStart;
        
        arrOutput[ (int)THeaderSide.Center ] = 
          strToSplit.Substring( iCStart + 2, end - iCStart - 2 );

        // can be skipped declaration of left part
        if( iCStart > 0 && iLStart < 0 )
        {
          arrOutput[ (int)THeaderSide.Left ] = strToSplit.Substring( 0, iCStart );
        }
      }

      // right part of header
      if( iRStart >= 0 )
      {
        int end = strToSplit.Length;
        
        arrOutput[ (int)THeaderSide.Right ] = 
          strToSplit.Substring( iRStart + 2, end - iRStart - 2 );

        // can be skipped declaration of center part
        if( iRStart > 0 && iCStart < 0 )
        {
          arrOutput[ (int)THeaderSide.Center ] = strToSplit.Substring( 0, iRStart );
        }
      }

      return arrOutput;
    }

    /// <summary>
    /// Function combines header or footer strings array to one format string
    /// </summary>
    /// <param name="parts">Array which must contains only 3 elements</param>
    /// <returns>Combined format string</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When parameter parts is null
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When number of strings in parts is not 3
    /// </exception>
    public string CreateHeaderFooterString( string[] parts )
    {
      if( parts == null )
        throw new ArgumentNullException( "parts" );

      if( parts.Length < 3 || parts.Length > 3 )
        throw new ArgumentException( "Parts array must have only three elements", "parts" );

      string output = string.Empty;

      if( parts[ 0 ] != null && parts[ 0 ].Length > 0 )
      {
        output += "&L" + parts[0];
      }

      if( parts[ 1 ] != null && parts[ 1 ].Length > 0 )
      {
        output += "&C" + parts[1];
      }
      
      if( parts[ 2 ] != null && parts[ 2 ].Length > 0 )
      {
        output += "&R" + parts[ 2 ];
      }

      return output;
    }
    /// <summary>
    /// Recovers next Record from the array and if it is record that describes some Margin
    /// ( left, right, top or bottom ) then it increases offset and return true,
    /// leaves offset and returns false otherwise
    /// </summary>
    /// <param name="data">Array of Biff Records</param>
    /// <param name="offset">Position of current record in the array</param>
    /// <returns>True if margin was recovered from the stream, False otherwise</returns>
    private bool GetNextMargin( BiffRecordRaw[] data, ref int offset )
    {
      BiffRecordRaw record = data[ offset ];

      switch( record.TypeCode )
      {
        case TBIFFRecord.LeftMargin:
          m_LeftMargin = ( LeftMarginRecord )record;
          offset++;
          return true;

        case TBIFFRecord.RightMargin:
          m_RightMargin = ( RightMarginRecord )record;
          offset++;
          return true;

        case TBIFFRecord.TopMargin:
          m_TopMargin = ( TopMarginRecord )record;
          offset++;
          return true;

        case TBIFFRecord.BottomMargin:
          m_BottomMargin = ( BottomMarginRecord )record;
          offset++;
          return true;

        default:
          return false;
      }
    }

    /// <summary>
    /// Recieves Biff Record and if it is record that describes some Margin
    /// ( left, right, top or bottom ) then it recovers next record from the stream a
    /// nd returns true, returns false otherwise
    /// </summary>
    /// <param name="data">Array of Biff Records</param>
    /// <param name="offset">Position of current record in the array</param>
    /// <returns>True if margin was recovered from the stream, False otherwise</returns>
    private bool GetNextMargin( BiffReader reader, ref BiffRecordRaw record )
    {
      switch( record.TypeCode )
      {
        case TBIFFRecord.LeftMargin:
          m_LeftMargin = ( LeftMarginRecord ) record;
          record = reader.GetRecord();
          return true;

        case TBIFFRecord.RightMargin:
          m_RightMargin = ( RightMarginRecord ) record;
          record = reader.GetRecord();
          return true;

        case TBIFFRecord.TopMargin:
          m_TopMargin = ( TopMarginRecord ) record;
          record = reader.GetRecord();
          return true;

        case TBIFFRecord.BottomMargin:
          m_BottomMargin = ( BottomMarginRecord ) record;
          record = reader.GetRecord();
          return true;

        default:
          return false;
      }
    }

    /// <summary>
    /// Returns current record from the Biff Records array and updates position by 1
    /// </summary>
    /// <param name="data">Array of Biff records</param>
    /// <param name="pos">
    /// Position of the record in the array that will be returned
    /// </param>
    /// <returns>Current record from array</returns>
    private BiffRecordRaw GetRecordUpdatePos( BiffRecordRaw[] data, ref int pos )
    {
      return data[ pos++ ];
    }
    /// <summary>
    /// Returns record of the specified type from the array of Biff
    /// records and sets position after returned record.
    /// </summary>
    /// <param name="data">Array of Biff records</param>
    /// <param name="pos">Starting from this position record must be searched</param>
    /// <param name="type">Type of the needed record</param>
    /// <returns>Biff record if it was found, null otherwise</returns>
    private BiffRecordRaw GetRecordUpdatePos( BiffRecordRaw[] data, ref int pos, TBIFFRecord type )
    {
      BiffRecordRaw rec = null;

      do
      {
        rec = data[ pos ];

        pos++;
        if( pos >= data.Length )
        {
          rec = null;
          break;
        }
      }
      while( rec.TypeCode != type );

      return rec;
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Adds all records to OffsetArrayList
    /// </summary>
    /// <param name="records">OffsetArrayList which will get all records</param>
    /// <exception cref="System.ArgumentNullException">
    /// When at least one of the internal records is null
    /// </exception>
    public void Serialize( OffsetArrayList records )
    {
      if( m_Header == null )
        throw new ArgumentNullException( "m_Header" );
      if( m_Footer == null )
        throw new ArgumentNullException( "m_Footer" );
      if( m_HCenter == null )
        throw new ArgumentNullException( "m_HCenter" );
      if( m_VCenter == null )
        throw new ArgumentNullException( "m_VCenter" );
      if( m_Setup == null )
        throw new ArgumentNullException( "m_Setup" );
      if( m_chartSize == null )
        throw new ArgumentNullException( "m_chartSize" );

      m_Header.Header = CreateHeaderFooterString( m_arrHeaders );
      records.Add( m_Header );
      
      m_Footer.Footer = CreateHeaderFooterString( m_arrFooters );
      records.Add( m_Footer );
      
      records.Add( m_HCenter );
      records.Add( m_VCenter );

      if( m_LeftMargin != null )    records.Add( m_LeftMargin );
      if( m_RightMargin != null )   records.Add( m_RightMargin );
      if( m_TopMargin != null )     records.Add( m_TopMargin );
      if( m_BottomMargin != null )  records.Add( m_BottomMargin );
      if( m_Unknown != null )       records.Add( m_Unknown );

      records.Add( m_Setup );
      records.Add( m_chartSize );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// This method is called after any changes in page setup.
    /// Sets Saved property of the parent workbook to the False state
    /// </summary>
    protected void SetChanged()
    {
      // TODO: implement
      //m_worksheet.SetChanged();
    }
    #endregion
  }
}
