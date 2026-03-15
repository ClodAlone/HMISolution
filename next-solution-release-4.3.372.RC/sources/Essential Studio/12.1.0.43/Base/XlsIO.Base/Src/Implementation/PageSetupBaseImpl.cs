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
using System.IO;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.Security;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using System.Drawing;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Summary description for PageSetupBaseImpl.
  /// </summary>
  public class PageSetupBaseImpl
    : CommonObject
    , IPageSetupBase
    , IBiffStorage
  {
    #region Class constants
    /// <summary>
    /// Value of the top margin by default.
    /// </summary>
    public const double DEFAULT_TOPMARGIN = 1;
    /// <summary>
    /// Value of the bottom margin by default.
    /// </summary>
    public const double DEFAULT_BOTTOMMARGIN = 1;
    /// <summary>
    /// Value of the left margin by default.
    /// </summary>
    public const double DEFAULT_LEFTMARGIN = 0.75;
    /// <summary>
    /// Value of the right margin by default.
    /// </summary>
    public const double DEFAULT_RIGHTMARGIN = 0.75;
    /// <summary>
    /// Set indexes as constants.
    /// </summary>
    protected enum THeaderSide : int
    {
      /// <summary>
      /// Left part of header or footer formatting.
      /// </summary>
      Left      = 0,
      /// <summary>
      /// Center part of header or footer formatting.
      /// </summary>
      Center    = 1,
      /// <summary>
      /// Right part of header or footer formatting.
      /// </summary>
      Right     = 2,
    }
    /// <summary>
    /// Name of the shape with header image.
    /// </summary>
    private static readonly string[] DEF_HEADER_NAMES = new string[]
    { 
      "LH", "CH", "RH",
    };
    /// <summary>
    /// Name of the shape with header image.
    /// </summary>
    private static readonly string[] DEF_FOOTER_NAMES = new string[]
    { 
      "LF", "CF", "RF",
    };
    /// <summary>
    /// Value of the header footer string limit
    /// </summary>
    private const int HeaderFooterStringLimit = 253;
    #endregion

    #region Class members
    /// <summary>
    /// Whether to center between horizontal margins.
    /// </summary>
    protected bool    m_bHCenter;
    /// <summary>
    /// Whether to center between vertical margins.
    /// </summary>
    protected bool    m_bVCenter;
    /// <summary>
    /// Unknown record. This record  contains additional information about
    /// system printer. If such record is found, it is stored, otherwise we is skipped.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected /*UnknownRecord*/PrinterSettingsRecord    m_unknown;
    /// <summary>
    /// Stores print setup options.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected PrintSetupRecord m_setup;
    /// <summary>
    /// This record contains information about worksheet bottom margin.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected double     m_dBottomMargin = DEFAULT_BOTTOMMARGIN;
    /// <summary>
    /// This record contains information about worksheet left margin.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected double     m_dLeftMargin = DEFAULT_LEFTMARGIN;
    /// <summary>
    /// This record contains information about worksheet right margin.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected double     m_dRightMargin = DEFAULT_RIGHTMARGIN;
    /// <summary>
    /// This record contains information about worksheet top margin.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected double     m_dTopMargin = DEFAULT_TOPMARGIN;
    /// <summary>
    /// Array of headers: 0 - left header, 1 - center header, 2 - right header.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected string[] m_arrHeaders = new string[] { string.Empty, string.Empty, string.Empty };
    /// <summary>
    /// Array of footers: 0 - left footer, 1 - center footer, 2 - right footer.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected string[] m_arrFooters = new string[] { string.Empty, string.Empty, string.Empty };
    /// <summary>
    /// Parent sheet.
    /// </summary>
    private WorksheetBaseImpl m_sheet;
    /// <summary>
    /// Contains background image.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected BitmapRecord m_backgroundImage;
    /// <summary>
    /// Indicates whether page setup is in FitTo printing mode.
    /// </summary>
    private bool m_bFitToPage;
      /// <summary>
      /// Dictionary which stores Max paper width
      /// </summary>
    internal Dictionary<ExcelPaperSize, double> dictPaperWidth = new Dictionary<ExcelPaperSize, double>();
      /// <summary>
      /// Dictionary which stores Max paper height
      /// </summary>
    internal Dictionary<ExcelPaperSize, double> dictPaperHeight = new Dictionary<ExcelPaperSize, double>();
    /// <summary>
    /// Stores print setup options.
    /// </summary>
    [CLSCompliant(false)]
    protected HeaderAndFooterRecord m_headerFooter;
    #endregion

    #region Internal Classes
    /// <summary>
    /// This class contains size of the paper.
    /// </summary>
    public sealed class PaperSizeEntry
    {
      #region Class members
      /// <summary>
      /// Paper width in points.
      /// </summary>
      public double Width;
      /// <summary>
      /// Paper height in points.
      /// </summary>
      public double Height;
      #endregion

      /// <summary>
      /// Default constructor. To prevent creation without arguments.
      /// </summary>
      private PaperSizeEntry()
      {
      }
      /// <summary>
      /// Initializes new instance of paper size entry.
      /// </summary>
      /// <param name="width">Paper width.</param>
      /// <param name="height">Paper height.</param>
      /// <param name="units">Units in which width and height are set.</param>
      public PaperSizeEntry( double width, double height, MeasureUnits units )
      {
        Width = ApplicationImpl.ConvertUnitsStatic( width, units, MeasureUnits.Point );
        Height = ApplicationImpl.ConvertUnitsStatic( height, units, MeasureUnits.Point );
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether fit to page mode is selected.
    /// </summary>
    public virtual bool IsFitToPage
    {
      get
      {
        return m_bFitToPage;
      }
      set
      {
        m_bFitToPage = value;
      }
    }
    /// <summary>
    /// Returns or sets the height of the pages that the worksheet will be scaled
    /// to when it is printed. Applies only to worksheets. Read/write Boolean.
    /// </summary>
    public int FitToPagesTall
    {
      get
      {
        return ( int )m_setup.FitHeight;
      }
      set
      {
        ushort newValue = ( ushort )value;

        if( m_setup.FitHeight != newValue )
        {
          m_setup.FitHeight = newValue;
          SetChanged();
        }

        if( !m_sheet.ParentWorkbook.Loading )
            IsFitToPage = value > 0 ? true : (FitToPagesWide > 0) ? true : false;
      }
    }
    /// <summary>
    /// Returns or sets the width of the pages the worksheet will be scaled
    /// to when it is printed. Applies only to worksheets. Read/write Boolean.
    /// </summary>
    public int FitToPagesWide
    {
      get
      {
        return ( int )m_setup.FitWidth;
      }
      set
      {
        ushort newValue = ( ushort )value;

        if( m_setup.FitWidth != newValue )
        {
          m_setup.FitWidth = newValue;
          SetChanged();
        }

        if( !m_sheet.ParentWorkbook.Loading )
            IsFitToPage = value > 0 ? true : (FitToPagesTall > 0) ? true : false;
      }
    }
    /// <summary>
    /// True if paper size, scaling factor, paper orientation (portrait / landscape),
    /// print resolution, and number of copies are not initialized.
    /// </summary>
    public bool IsNotValidSettings
    {
      get
      {
        return m_setup.IsNotValidSettings;
      }
      internal set
      {
        m_setup.IsNotValidSettings = value;
      }
    }
    /// <summary>
    /// Indicates whether FirstPageNumber is set to Auto or not.
    /// </summary>
    public bool AutoFirstPageNumber
    {
      get
      {
        return !m_setup.IsUsePage;
      }
      set
      {
        m_setup.IsUsePage = !value;
      }
    }
    /// <summary>
    /// True if elements of the document will be printed in black and white.
    /// Read/write Boolean.
    /// </summary>
    public bool   BlackAndWhite
    {
      get
      {
        return m_setup.IsNoColor;
      }
      set
      {
        if( m_setup.IsNoColor != value )
        {
          m_setup.IsNoColor = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the size of the bottom margin, in inches.
    /// Read / write Double.
    /// </summary>
    public double BottomMargin
    {
      get
      {
        return m_dBottomMargin;
      }
      set
      {
        if( m_dBottomMargin != value )
        {
          m_dBottomMargin = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the center part of the footer. Read / write String.
    /// Special Formatting symbols:
    /// &amp;P Current page number
    /// &amp;N Page count
    /// &amp;D Current date
    /// &amp;T Current time
    /// &amp;A Sheet name (BIFF5-BIFF8)
    /// &amp;F File name without path
    /// &amp;Z File path without file name (BIFF8X)
    /// &amp;G Picture (BIFF8X)
    /// &amp;B Bold on/off (BIFF2-BIFF4)
    /// &amp;I Italic on/off (BIFF2-BIFF4)
    /// &amp;U Underlining on/off
    /// &amp;E Double underlining on/off (BIFF5-BIFF8)
    /// &amp;S Strikeout on/off
    /// &amp;X Superscript on/off (BIFF5-BIFF8)
    /// &amp;Y Subscript on/off (BIFF5-BIFF8)
    /// &amp;"[FONTNAME]" Set new font [FONTNAME]
    /// &amp;"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE].
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic,
    /// or Bold Italic. But this setting is dependent on the used font, it may
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &amp;[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value).
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
            if (CheckHeaderFooterString(value))
            {
                m_arrFooters[(int)THeaderSide.Center] = value;
                SetChanged();
            }
        }
      }
    }
    /// <summary>
    /// Gets / set image for center part of the footer.
    /// </summary>
    public Image CenterFooterImage
    {
      get
      {
        BitmapShapeImpl shape = ( BitmapShapeImpl )m_sheet.HeaderFooterShapes[
          DEF_FOOTER_NAMES[ ( int )THeaderSide.Center ] ];

        return ( shape != null ) ? shape.Picture : null;
      }
      set
      {
        m_sheet.HeaderFooterShapes.SetPicture( DEF_FOOTER_NAMES[ ( int )THeaderSide.Center ], value );
      }
    }
    /// <summary>
    /// Gets / set image for center part of the header.
    /// </summary>
    public Image CenterHeaderImage
    {
      get
      {
        BitmapShapeImpl shape = ( BitmapShapeImpl )m_sheet.HeaderFooterShapes[
          DEF_HEADER_NAMES[ ( int )THeaderSide.Center ] ];

        return ( shape != null ) ? shape.Picture : null;
      }
      set
      {
        m_sheet.HeaderFooterShapes.SetPicture( DEF_HEADER_NAMES[ ( int )THeaderSide.Center ], value );
      }
    }
    /// <summary>
    /// Returns or sets the center part of the header. Read/write String.
    /// Special Formatting symbols:
    /// &amp;P Current page number
    /// &amp;N Page count
    /// &amp;D Current date
    /// &amp;T Current time
    /// &amp;A Sheet name (BIFF5-BIFF8)
    /// &amp;F File name without path
    /// &amp;Z File path without file name (BIFF8X)
    /// &amp;G Picture (BIFF8X)
    /// &amp;B Bold on/off (BIFF2-BIFF4)
    /// &amp;I Italic on/off (BIFF2-BIFF4)
    /// &amp;U Underlining on/off
    /// &amp;E Double underlining on/off (BIFF5-BIFF8)
    /// &amp;S Strikeout on/off
    /// &amp;X Superscript on/off (BIFF5-BIFF8)
    /// &amp;Y Subscript on/off (BIFF5-BIFF8)
    /// &amp;"[FONTNAME]" Set new font [FONTNAME]
    /// &amp;"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE].
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic,
    /// or Bold Italic. But this setting is dependent on the used font, it may
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &amp;[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value).
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
            if (CheckHeaderFooterString(value))
            {
                m_arrHeaders[(int)THeaderSide.Center] = value;
                SetChanged();
            }
        }
      }
    }
    /// <summary>
    /// True if the sheet is centered horizontally on the page when it is
    /// printed. Read/write Boolean.
    /// </summary>
    public bool   CenterHorizontally
    {
      get
      {
        return m_bHCenter;
      }
      set
      {
        if( m_bHCenter != value )
        {
          m_bHCenter = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// True if the sheet is centered vertically on the page when it is
    /// printed. Read/write Boolean.
    /// </summary>
    public bool   CenterVertically
    {
      get
      {
        return m_bVCenter;
      }
      set
      {
        if( m_bVCenter != value )
        {
          m_bVCenter = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Number of copies to print.
    /// </summary>
    public int    Copies
    {
      get
      {
        return m_setup.Copies;
      }
      set
      {
        if( value < 1 )
        {
          if( ( ( WorkbookImpl )m_sheet.Workbook ).Loading )
          {
            value = 1;
          }
          else
          {
            throw new ArgumentOutOfRangeException( "Number of copies can not be less then 1" );
          }
        }

        if( m_setup.Copies != (ushort) value )
        {
          m_setup.Copies = (ushort) value;
          m_setup.IsNotValidSettings = false;
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
        return m_setup.IsDraft;
      }
      set
      {
        if( m_setup.IsDraft != value )
        {
          m_setup.IsDraft = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the first page number that will be used when
    /// this sheet is printed.
    /// </summary>
    public short    FirstPageNumber
    {
      get
      {
        return m_setup.PageStart;
      }
      set
      {
        if( m_setup.PageStart != ( short ) value )
        {
          m_setup.PageStart = ( short ) value;
          AutoFirstPageNumber = false;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the distance from the bottom of the page to the footer,
    /// in inches. Read/write Double.
    /// </summary>
    public double FooterMargin
    {
      get
      {
        return m_setup.FooterMargin;
      }
      set
      {
        if( m_setup.FooterMargin != value )
        {
          m_setup.FooterMargin = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the distance from the top of the page to the header,
    /// in inches. Read/write Double.
    /// </summary>
    public double HeaderMargin
    {
      get
      {
        return m_setup.HeaderMargin;
      }
      set
      {
        if( m_setup.HeaderMargin != value )
        {
          m_setup.HeaderMargin = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the left part of the footer. Read/write String.
    /// Special Formatting symbols:
    /// &amp;P Current page number
    /// &amp;N Page count
    /// &amp;D Current date
    /// &amp;T Current time
    /// &amp;A Sheet name (BIFF5-BIFF8)
    /// &amp;F File name without path
    /// &amp;Z File path without file name (BIFF8X)
    /// &amp;G Picture (BIFF8X)
    /// &amp;B Bold on/off (BIFF2-BIFF4)
    /// &amp;I Italic on/off (BIFF2-BIFF4)
    /// &amp;U Underlining on/off
    /// &amp;E Double underlining on/off (BIFF5-BIFF8)
    /// &amp;S Strikeout on/off
    /// &amp;X Superscript on/off (BIFF5-BIFF8)
    /// &amp;Y Subscript on/off (BIFF5-BIFF8)
    /// &amp;"[FONTNAME]" Set new font [FONTNAME]
    /// &amp;"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE].
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic,
    /// or Bold Italic. But this setting is dependent on the used font, it may
    /// differ (localized style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &amp;[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value).
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
            if (CheckHeaderFooterString(value))
            {
                m_arrFooters[(int)THeaderSide.Left] = value;
                SetChanged();
            }
        }
      }
    }
    /// <summary>
    /// Returns or sets the left part of the header. Read/write String.
    /// Special Formatting symbols:
    /// &amp;P Current page number
    /// &amp;N Page count
    /// &amp;D Current date
    /// &amp;T Current time
    /// &amp;A Sheet name (BIFF5-BIFF8)
    /// &amp;F File name without path
    /// &amp;Z File path without file name (BIFF8X)
    /// &amp;G Picture (BIFF8X)
    /// &amp;B Bold on/off (BIFF2-BIFF4)
    /// &amp;I Italic on/off (BIFF2-BIFF4)
    /// &amp;U Underlining on/off
    /// &amp;E Double underlining on/off (BIFF5-BIFF8)
    /// &amp;S Strikeout on/off
    /// &amp;X Superscript on/off (BIFF5-BIFF8)
    /// &amp;Y Subscript on/off (BIFF5-BIFF8)
    /// &amp;"[FONTNAME]" Set new font [FONTNAME]
    /// &amp;"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE].
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic,
    /// or Bold Italic. But this setting is dependent on the used font, it may
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &amp;[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value).
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
            if (CheckHeaderFooterString(value))
            {
                m_arrHeaders[(int)THeaderSide.Left] = value;
                SetChanged();
            }
        }
      }
    }
    /// <summary>
    /// Gets / set image for left part of the footer.
    /// </summary>
    public Image LeftFooterImage
    {
      get
      {
        BitmapShapeImpl shape = ( BitmapShapeImpl )m_sheet.HeaderFooterShapes[
          DEF_FOOTER_NAMES[ ( int )THeaderSide.Left ] ];

        return ( shape != null ) ? shape.Picture : null;
      }
      set
      {
        m_sheet.HeaderFooterShapes.SetPicture( DEF_FOOTER_NAMES[ ( int )THeaderSide.Left ], value );
      }
    }
    /// <summary>
    /// Gets / set image for left part of the header.
    /// </summary>
    public Image LeftHeaderImage
    {
      get
      {
        BitmapShapeImpl shape = ( BitmapShapeImpl )m_sheet.HeaderFooterShapes[
          DEF_HEADER_NAMES[ ( int )THeaderSide.Left ] ];

        return ( shape != null ) ? shape.Picture : null;
      }
      set
      {
        m_sheet.HeaderFooterShapes.SetPicture( DEF_HEADER_NAMES[ ( int )THeaderSide.Left ], value );
      }
    }
    /// <summary>
    /// Returns or sets the size of the left margin, in inches.
    /// Read/write Double.
    /// </summary>
    public double LeftMargin
    {
      get
      {
        return m_dLeftMargin;
      }
      set
      {
        if( m_dLeftMargin != value )
        {
          m_dLeftMargin = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the order that Microsoft Excel uses to number
    /// pages when printing a large worksheet. Read / write ExcelOrder.
    /// </summary>
    public ExcelOrder Order
    {
      get
      {
        return m_setup.IsLeftToRight ? ExcelOrder.OverThenDown : ExcelOrder.DownThenOver;
      }
      set
      {
        bool newValue = value == ( ExcelOrder.OverThenDown );
        if( m_setup.IsLeftToRight != newValue )
        {
          m_setup.IsLeftToRight = newValue;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Portrait or landscape printing mode. Read/write ExcelPageOrientation.
    /// </summary>
    public ExcelPageOrientation Orientation
    {
      get
      {
        return m_setup.IsNotLandscape ? ExcelPageOrientation.Portrait : ExcelPageOrientation.Landscape;
      }
      set
      {
        m_setup.IsNotLandscape = ( value == ExcelPageOrientation.Portrait );
        m_setup.IsNotValidSettings = false;
        m_setup.IsNoOrientation = false;
        SetChanged();
      }
    }
    /// <summary>
    /// Returns or sets the size of the paper. Read / write ExcelPaperSize.
    /// </summary>
    public ExcelPaperSize PaperSize
    {
      get
      {
        return ( ExcelPaperSize )m_setup.PaperSize;
      }
      set
      {
        m_setup.PaperSize = ( ushort )value;
        m_setup.IsNotValidSettings = false;
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
        if( m_setup.IsNotes == false )
        {
          return ExcelPrintLocation.PrintNoComments;
        }
        else if( m_setup.IsPrintNotesAsDisplayed )
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
            m_setup.IsNotes = false;
            break;

          case ExcelPrintLocation.PrintInPlace:
            m_setup.IsNotes = true;
            m_setup.IsPrintNotesAsDisplayed = true;
            break;

          case ExcelPrintLocation.PrintSheetEnd:
            m_setup.IsNotes = true;
            m_setup.IsPrintNotesAsDisplayed = false;
            break;
        }

        SetChanged();
      }
    }
    /// <summary>
    /// Sets or returns an ExcelPrintErrors constant specifying the type of
    /// print error displayed. This feature allows users to suppress the
    /// display of error values when printing a worksheet. Read/write.
    /// </summary>
    public ExcelPrintErrors PrintErrors
    {
      get
      {
        return m_setup.PrintErrors;
      }
      set
      {
        if( m_setup.PrintErrors != value )
        {
          m_setup.PrintErrors = value;
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
        return m_setup.IsNotes;
      }
      set
      {
        if( m_setup.IsNotes != value )
        {
          m_setup.IsNotes = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the print quality. Read / write ushort.
    /// </summary>
    public int    PrintQuality
    {
      get
      {
        return m_setup.HResolution;
      }
      set
      {
        m_setup.HResolution = (ushort) value;
        m_setup.VResolution = (ushort) value;
        m_setup.IsNotValidSettings = false;

        SetChanged();
      }
    }
    /// <summary>
    /// Returns or sets the right part of the footer. Read / write String.
    /// Special Formatting symbols:
    /// &amp;P Current page number
    /// &amp;N Page count
    /// &amp;D Current date
    /// &amp;T Current time
    /// &amp;A Sheet name (BIFF5-BIFF8)
    /// &amp;F File name without path
    /// &amp;Z File path without file name (BIFF8X)
    /// &amp;G Picture (BIFF8X)
    /// &amp;B Bold on/off (BIFF2-BIFF4)
    /// &amp;I Italic on/off (BIFF2-BIFF4)
    /// &amp;U Underlining on/off
    /// &amp;E Double underlining on/off (BIFF5-BIFF8)
    /// &amp;S Strikeout on/off
    /// &amp;X Superscript on/off (BIFF5-BIFF8)
    /// &amp;Y Subscript on/off (BIFF5-BIFF8)
    /// &amp;"[FONTNAME]" Set new font [FONTNAME]
    /// &amp;"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE].
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic,
    /// or Bold Italic. But this setting is dependent on the used font, it may
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &amp;[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value).
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
            if (CheckHeaderFooterString(value))
            {
                m_arrFooters[(int)THeaderSide.Right] = value;
                SetChanged();
            }
        }
      }
    }
    /// <summary>
    /// Gets / set image for right part of the footer.
    /// </summary>
    public Image RightFooterImage
    {
      get
      {
        BitmapShapeImpl shape = ( BitmapShapeImpl )m_sheet.HeaderFooterShapes[
          DEF_FOOTER_NAMES[ ( int )THeaderSide.Right ] ];

        return ( shape != null ) ? shape.Picture : null;
      }
      set
      {
        m_sheet.HeaderFooterShapes.SetPicture( DEF_FOOTER_NAMES[ ( int )THeaderSide.Right ], value );
      }
    }
    /// <summary>
    /// Returns or sets the right part of the header. Read / write String.
    /// Special Formatting symbols:
    /// &amp;P Current page number
    /// &amp;N Page count
    /// &amp;D Current date
    /// &amp;T Current time
    /// &amp;A Sheet name (BIFF5-BIFF8)
    /// &amp;F File name without path
    /// &amp;Z File path without file name (BIFF8X)
    /// &amp;G Picture (BIFF8X)
    /// &amp;B Bold on/off (BIFF2-BIFF4)
    /// &amp;I Italic on/off (BIFF2-BIFF4)
    /// &amp;U Underlining on/off
    /// &amp;E Double underlining on/off (BIFF5-BIFF8)
    /// &amp;S Strikeout on/off
    /// &amp;X Superscript on/off (BIFF5-BIFF8)
    /// &amp;Y Subscript on/off (BIFF5-BIFF8)
    /// &amp;"[FONTNAME]" Set new font [FONTNAME]
    /// &amp;"[FONTNAME],[FONTSTYLE]" Set new font with specified style [FONTSTYLE].
    /// The style [fontstyle] is in most cases one of Regular, Bold, Italic,
    /// or Bold Italic. But this setting is dependent on the used font, it may
    /// differ (localised style names, or Standard, Oblique, ...). (BIFF5-BIFF8)
    /// &amp;[FONTHEIGHT] Set font height in points ([FONTHEIGHT] is a decimal value).
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
            if (CheckHeaderFooterString(value))
            {
                m_arrHeaders[(int)THeaderSide.Right] = value;
                SetChanged();
            }
        }
      }
    }
    /// <summary>
    /// Gets / set image for right part of the header.
    /// </summary>
    public Image RightHeaderImage
    {
      get
      {
        BitmapShapeImpl shape = ( BitmapShapeImpl )m_sheet.HeaderFooterShapes[
          DEF_HEADER_NAMES[ ( int )THeaderSide.Right ] ];

        return ( shape != null ) ? shape.Picture : null;
      }
      set
      {
        m_sheet.HeaderFooterShapes.SetPicture( DEF_HEADER_NAMES[ ( int )THeaderSide.Right ], value );
      }
    }
    /// <summary>
    /// Returns or sets the size of the right margin, in inches.
    /// Read/write Double.
    /// </summary>
    public double RightMargin
    {
      get
      {
        return m_dRightMargin;
      }
      set
      {
        if( m_dRightMargin != value )
        {
          m_dRightMargin = value;
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
        return m_dTopMargin;
      }
      set
      {
        if( m_dTopMargin != value )
        {
          m_dTopMargin = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets a percentage (between 10 and 400 percent) by which
    /// Microsoft Excel will scale the worksheet for printing. Applies only
    /// to worksheets. Read/write ushort.
    /// </summary>
    public int   Zoom
    {
      get
      {
        return m_setup.Scale;
      }
      set
      {
        if( value < 10 || value > 400 )
          throw new ArgumentOutOfRangeException( "Zoom value must be beetween 10 and 400 percent." );

        m_setup.Scale = (ushort) value;
        m_setup.IsNotValidSettings = false;
        SetChanged();
      }
    }
    /// <summary>
    /// Gets / sets background image.
    /// </summary>
    public
#if  (SILVERLIGHT) || ( WINRT ) || WP
      Image
#else
      Bitmap
#endif
      BackgoundImage
    {
      get
      {
        if( m_backgroundImage == null )
        {
          return null;
        }
        else
        {
          return m_backgroundImage.Picture;
        }
      }
      set
      {
        if( value == null )
        {
          m_backgroundImage = null;
          return;
        }

        if( m_backgroundImage == null )
        {
          m_backgroundImage = ( BitmapRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Bitmap );
        }

        m_backgroundImage.Picture = value;
      }
    }
    /// <summary>
    /// Returns page width in points. Read-only.
    /// </summary>
    public double PageWidth
    {
      get
      {
        PaperSizeEntry paper = ( AppImplementation.DicPaperSizeTable.ContainsKey( ( int )PaperSize ) )
          ? AppImplementation.DicPaperSizeTable[(int)PaperSize]
          : AppImplementation.DicPaperSizeTable[(int)ExcelPaperSize.PaperA4];

        return ( Orientation == ExcelPageOrientation.Portrait )
          ? paper.Width
          : paper.Height;
      }
    }
    /// <summary>
    /// Returns page width in points. Read-only.
    /// </summary>
    public double PageHeight
    {
      get
      {
          PaperSizeEntry paper = (AppImplementation.DicPaperSizeTable.ContainsKey((int)PaperSize))
          ? AppImplementation.DicPaperSizeTable[(int)PaperSize]
          : AppImplementation.DicPaperSizeTable[(int)ExcelPaperSize.PaperA4];

        return ( Orientation == ExcelPageOrientation.Portrait )
          ? paper.Height
          : paper.Width;
      }
    }
    /// <summary>
    /// Gets / sets horizontal resolution in dpi.
    /// </summary>
    public int HResolution
    {
      get
      {
        return m_setup.HResolution;
      }
      set
      {
        m_setup.HResolution = ( ushort )value;
      }
    }
    /// <summary>
    /// Gets / sets vertical resolution in dpi.
    /// </summary>
    public int VResolution
    {
      get
      {
        return m_setup.VResolution;
      }
      set
      {
        m_setup.VResolution = ( ushort )value;
      }
    }
    
    #endregion

    #region Class helper properties
    /// <summary>
    /// Represents full header string. Read/write.
    /// </summary>
    public string FullHeaderString
    {
      get
      {
        return CreateHeaderFooterString( m_arrHeaders );
      }
      set
      {
        m_arrHeaders = ParseHeaderFooterString( value );
      }
    }
    /// <summary>
    /// Gets footer full string. Read/write.
    /// </summary>
    public string FullFooterString
    {
      get
      {
        return CreateHeaderFooterString( m_arrFooters );
      }
      set
      {
        m_arrFooters = ParseHeaderFooterString( value );
      }
    }
    /// <summary>
    /// Indicates whether the header and footer margins are aligned with page margins. Read/Write Boolean.
    /// </summary>
    public bool AlignHFWithPageMargins
    {
        get
        {
            return m_headerFooter.AlignHFWithPageMargins;
        }
        set 
        {
            m_headerFooter.AlignHFWithPageMargins = value;
        }
    }
    /// <summary>
    /// True - The header / footer of the first page is different with other pages.Otherwise False.
    /// </summary>
    public bool DifferentFirstPageHF
    {
        get
        {
            return m_headerFooter.DifferentFirstPageHF;
        }
        set
        {
            m_headerFooter.DifferentFirstPageHF = value;
        }
    }
    /// <summary>
    /// True - The header/footer odd pages are differed with even page. Otherwise False.
    /// </summary>
    public bool DifferentOddAndEvenPagesHF
    {
        get
        {
            return m_headerFooter.DifferentOddAndEvenPagesHF;
        }
        set 
        {
            m_headerFooter.DifferentOddAndEvenPagesHF = value;
        }
    }
    /// <summary>
    /// Indicates whether the header and footer are scaled with document scaling.Read/Write Boolean.
    /// </summary>
    public bool HFScaleWithDoc
    {
        get 
        {
            return m_headerFooter.HFScaleWithDoc;
        }
        set
        {
            m_headerFooter.HFScaleWithDoc = value;
        }
    }
    #endregion

    #region Class Initialize/Finalize methods

      /// <summary>
      /// Fills the dictionaries with paper width size and height size
      /// </summary>
      /// <param name="pageSetup"></param>
    private void FillMaxPaperSize(ApplicationImpl application)
    {
        
        dictPaperWidth.Add(ExcelPaperSize.A2Paper,application .ConvertUnits (420,MeasureUnits .Millimeter,MeasureUnits .Inch ));
        dictPaperHeight.Add(ExcelPaperSize.A2Paper, application.ConvertUnits(594, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.A3ExtraPaper, application.ConvertUnits(322, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.A3ExtraPaper, application.ConvertUnits(445, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.A3ExtraTransversePaper, application.ConvertUnits(332, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.A3ExtraTransversePaper, application.ConvertUnits(445, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.A3TransversePaper, application.ConvertUnits(297, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.A3TransversePaper, application.ConvertUnits(420, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.A4ExtraPaper, application.ConvertUnits(236, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.A4ExtraPaper, application.ConvertUnits(332, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.A4PlusPaper, application.ConvertUnits(210, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.A4PlusPaper, application.ConvertUnits(330, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.A4TransversePaper, application.ConvertUnits(210, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.A4TransversePaper, application.ConvertUnits(297, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.A5ExtraPpaper, application.ConvertUnits(174, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.A5ExtraPpaper, application.ConvertUnits(235, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.A5TransversePaper, application.ConvertUnits(148, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.A5TransversePaper, application.ConvertUnits(210, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.InviteEnvelope, application.ConvertUnits(220, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.InviteEnvelope, application.ConvertUnits(220, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.ISOB4, application.ConvertUnits(250, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.ISOB4, application.ConvertUnits(353, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.ISOB5ExtraPaper, application.ConvertUnits(210, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.ISOB5ExtraPaper, application.ConvertUnits(276, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.JapaneseDoublePostcard, application.ConvertUnits(200, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.JapaneseDoublePostcard, application.ConvertUnits(148, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.JISB5TransversePaper, application.ConvertUnits(182, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.JISB5TransversePaper, application.ConvertUnits(257, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.LegalExtraPaper9275By15, 9.275);
        dictPaperHeight.Add(ExcelPaperSize.LegalExtraPaper9275By15, 15);
        dictPaperWidth.Add(ExcelPaperSize.LetterExtraPaper9275By12, 9.275);
        dictPaperHeight.Add(ExcelPaperSize.LetterExtraPaper9275By12, 12);
        dictPaperWidth.Add(ExcelPaperSize.LetterExtraTransversePaper, 9.275);
        dictPaperHeight.Add(ExcelPaperSize.LetterExtraTransversePaper, 12);
        dictPaperWidth.Add(ExcelPaperSize.LetterPlusPaper, 8.5);
        dictPaperHeight.Add(ExcelPaperSize.LetterPlusPaper, 12.69);
        dictPaperWidth.Add(ExcelPaperSize.LetterTransversePaper, 8.275);
        dictPaperHeight.Add(ExcelPaperSize.LetterTransversePaper, 11);
        dictPaperWidth.Add(ExcelPaperSize.Paper10x14, 10);
        dictPaperHeight.Add(ExcelPaperSize.Paper10x14, 14);
        dictPaperWidth.Add(ExcelPaperSize.Paper11x17, 11);
        dictPaperHeight.Add(ExcelPaperSize.Paper11x17, 17);
        dictPaperWidth.Add(ExcelPaperSize.PaperA3, application.ConvertUnits(297, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperA3, application.ConvertUnits(420, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperA4, application.ConvertUnits(210, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperA4, application.ConvertUnits(297, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperA4Small, application.ConvertUnits(210, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperA4Small, application.ConvertUnits(297, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperA5, application.ConvertUnits(148, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperA5, application.ConvertUnits(210, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperB4, application.ConvertUnits(250, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperB4, application.ConvertUnits(353, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperB5, application.ConvertUnits(176, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperB5, application.ConvertUnits(250, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperCsheet, 17);
        dictPaperHeight.Add(ExcelPaperSize.PaperCsheet, 22);
        dictPaperWidth.Add(ExcelPaperSize.PaperDsheet, 22);
        dictPaperHeight.Add(ExcelPaperSize.PaperDsheet, 34);
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelope10, 4.125);
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelope10, 9.5);
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelope11, 4.5);
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelope11, 10.375);
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelope12, 4.75);
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelope12, 11);
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelope14, 5);
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelope14, 11.5);
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelope9, 3.875);
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelope9, 8.875);
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeB4, application.ConvertUnits(250, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeB4, application.ConvertUnits(353, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeB5, application.ConvertUnits(176, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeB5, application.ConvertUnits(250, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeB6, application.ConvertUnits(176, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeB6, application.ConvertUnits(125, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeC3, application.ConvertUnits(324, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeC3, application.ConvertUnits(458, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeC4, application.ConvertUnits(229, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeC4, application.ConvertUnits(324, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeC5, application.ConvertUnits(162, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeC5, application.ConvertUnits(229, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeC6, application.ConvertUnits(114, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeC6, application.ConvertUnits(162, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeC65, application.ConvertUnits(114, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeC65, application.ConvertUnits(229, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeDL, application.ConvertUnits(110, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeDL, application.ConvertUnits(220, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeItaly, application.ConvertUnits(110, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeItaly, application.ConvertUnits(230, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopeMonarch, 3.875);
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopeMonarch, 7.5);
        dictPaperWidth.Add(ExcelPaperSize.PaperEnvelopePersonal, 3.625);
        dictPaperHeight.Add(ExcelPaperSize.PaperEnvelopePersonal, 6.5);
        dictPaperWidth.Add(ExcelPaperSize.PaperEsheet, 34);
        dictPaperHeight.Add(ExcelPaperSize.PaperEsheet, 34);
        dictPaperWidth.Add(ExcelPaperSize.PaperExecutive, 7.5);
        dictPaperHeight.Add(ExcelPaperSize.PaperExecutive, 7.5);
        dictPaperWidth.Add(ExcelPaperSize.PaperFanfoldLegalGerman, 8.5);
        dictPaperHeight.Add(ExcelPaperSize.PaperFanfoldLegalGerman, 13);
        dictPaperWidth.Add(ExcelPaperSize.PaperFanfoldStdGerman, 8.5);
        dictPaperHeight.Add(ExcelPaperSize.PaperFanfoldStdGerman, 12);
        dictPaperWidth.Add(ExcelPaperSize.PaperFanfoldUS, 14.875);
        dictPaperHeight.Add(ExcelPaperSize.PaperFanfoldUS, 11);
        dictPaperWidth.Add(ExcelPaperSize.PaperFolio, 8.5);
        dictPaperHeight.Add(ExcelPaperSize.PaperFolio, 13);
        dictPaperWidth.Add(ExcelPaperSize.PaperLedger, 17);
        dictPaperHeight.Add(ExcelPaperSize.PaperLedger, 11);
        dictPaperWidth.Add(ExcelPaperSize.PaperLegal, 8.5);
        dictPaperHeight.Add(ExcelPaperSize.PaperLegal, 14);
        dictPaperWidth.Add(ExcelPaperSize.PaperLetter, 8.5);
        dictPaperHeight.Add(ExcelPaperSize.PaperLetter, 11);
        dictPaperWidth.Add(ExcelPaperSize.PaperLetterSmall, 8.5);
        dictPaperHeight.Add(ExcelPaperSize.PaperLetterSmall, 11);
        dictPaperWidth.Add(ExcelPaperSize.PaperNote, 8.5);
        dictPaperHeight.Add(ExcelPaperSize.PaperNote, 11);
        dictPaperWidth.Add(ExcelPaperSize.PaperQuarto, application.ConvertUnits(215, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.PaperQuarto, application.ConvertUnits(275, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.PaperStatement, 5.5);
        dictPaperHeight.Add(ExcelPaperSize.PaperStatement, 8.5);
        dictPaperWidth.Add(ExcelPaperSize.PaperTabloid, 11);
        dictPaperHeight.Add(ExcelPaperSize.PaperTabloid, 17);
        dictPaperWidth.Add(ExcelPaperSize.StandardPaper10By11, 10);
        dictPaperHeight.Add(ExcelPaperSize.StandardPaper10By11, 11);
        dictPaperWidth.Add(ExcelPaperSize.StandardPaper15By11, 15);
        dictPaperHeight.Add(ExcelPaperSize.StandardPaper15By11, 11);
        dictPaperWidth.Add(ExcelPaperSize.StandardPaper9By11, 9);
        dictPaperHeight.Add(ExcelPaperSize.StandardPaper9By11, 11);
        dictPaperWidth.Add(ExcelPaperSize.SuperASuperAA4Paper, application.ConvertUnits(227, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.SuperASuperAA4Paper, application.ConvertUnits(356, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.SuperBSuperBA3Paper, application.ConvertUnits(305, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperHeight.Add(ExcelPaperSize.SuperBSuperBA3Paper, application.ConvertUnits(487, MeasureUnits.Millimeter, MeasureUnits.Inch));
        dictPaperWidth.Add(ExcelPaperSize.TabloidExtraPaper, 11.69);
        dictPaperHeight.Add(ExcelPaperSize.TabloidExtraPaper, 18);
    }
    /// <summary>
    /// Sets application and parent fields.
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    public PageSetupBaseImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_setup   = ( PrintSetupRecord )BiffRecordFactory.GetRecord( TBIFFRecord.PrintSetup );
      m_headerFooter = new HeaderAndFooterRecord();
      FillMaxPaperSize(application as ApplicationImpl);
      FindParents();
    }
    /// <summary>
    /// Find parent Worksheet.
    /// </summary>
    /// <exception cref="System.ArgumentException">
    /// Can't find parent worksheet.
    /// </exception>
    protected virtual void FindParents()
    {
      m_sheet = FindParent( Parent, typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl;

      if( m_sheet == null )
        throw new ArgumentNullException( "Parent worksheet." );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// This function splits header or footer into three parts left, center, and right.
    /// </summary>
    /// <param name="strToSplit">
    /// Header (footer) string that will be split into
    /// parts: left, center, right.
    /// </param>
    /// <returns>Array of split strings.</returns>
    protected string[] ParseHeaderFooterString( string strToSplit )
    {
      if( strToSplit == null )
        throw new ArgumentNullException( "strToSplit" );

      string[] arrOutput = new string[] { string.Empty, string.Empty, string.Empty };

      int iLength = strToSplit.Length;

      if( iLength == 0 )
        return arrOutput;

      int iLStart = strToSplit.IndexOf( "&L" );
      int iCStart = strToSplit.IndexOf( "&C" );
      int iRStart = strToSplit.IndexOf( "&R" );

      // If nothing is found, return empty array.
      if( iLStart == iCStart &&
        iCStart == iRStart &&
        iRStart == -1 )
      {
        // NOTE: this is some kind of workaround. There could be some files that have no section specified.
        // In this case MS Excel treats header/footer as center part.
        arrOutput[ ( int )THeaderSide.Center ] = strToSplit;
        return arrOutput;
      }

      // Left part of header.
      if( iLStart >= 0 )
      {
        int end = iLength;

        if( iCStart > iLStart )
        {
          end = iCStart;
        }
        else if( iRStart > iLStart )
        {
          end = iRStart;
        }

        arrOutput[ ( int )THeaderSide.Left ] =
          strToSplit.Substring( iLStart + 2, end - iLStart - 2 );
      }

      // Center part of header.
      if( iCStart >= 0 )
      {
        int end = iLength;

        if( iRStart > iCStart ) end = iRStart;

        arrOutput[ ( int )THeaderSide.Center ] =
          strToSplit.Substring( iCStart + 2, end - iCStart - 2 );

        // Can be skipped declaration of left part.
        if( iCStart > 0 && iLStart < 0 )
        {
          arrOutput[ ( int )THeaderSide.Left ] = strToSplit.Substring( 0, iCStart );
        }
      }

      // Right part of header.
      if( iRStart >= 0 )
      {
        int end = iLength;

        arrOutput[ ( int )THeaderSide.Right ] =
          strToSplit.Substring( iRStart + 2, end - iRStart - 2 );

        // Can be skipped declaration of center part.
        if( iRStart > 0 && iCStart < 0 && iLStart < 0 )
        {
          arrOutput[ ( int )THeaderSide.Center ] = strToSplit.Substring( 0, iRStart );
        }
      }

      return arrOutput;
    }

    /// <summary>
    /// Function combines header or footer strings array to one format string.
    /// </summary>
    /// <param name="parts">Array which must contain only 3 elements.</param>
    /// <returns>Combined format string.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When parameter is null.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When number of strings in parts is not 3.
    /// </exception>
    protected string CreateHeaderFooterString( string[] parts )
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
    /// Checks the header and footer string limit [MS Excel allows to enter only 253 characters]
    /// </summary>
    /// <param name="value">header or footer string value to set</param>
    /// <returns>boolean value</returns>
    private bool CheckHeaderFooterString(string value)
    {
        if (value.Length > HeaderFooterStringLimit)
        {
            throw new ArgumentOutOfRangeException("value", "The string is too long.Reduce the number of characters used.");
        }
        return true;
    }
    /// <summary>
    /// Adds all records to OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList which will get all records.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When at least one of the internal records is null.
    /// </exception>
    [ CLSCompliant( false ) ]
    public virtual void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_setup == null )
        throw new ArgumentNullException( "m_Setup" );

      SerializeStartRecords( records );
      
      HeaderFooterRecord header = ( HeaderFooterRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Header );
      header.Value = CreateHeaderFooterString( m_arrHeaders );
      records.Add( header );
     
      HeaderFooterRecord footer = ( HeaderFooterRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Footer );
      footer.Value = CreateHeaderFooterString( m_arrFooters );
      records.Add( footer );

      SheetCenterRecord hCenter = ( SheetCenterRecord )BiffRecordFactory.GetRecord( TBIFFRecord.HCenter );
      hCenter.IsCenter = ( ushort )( m_bHCenter ? 1 : 0 );
      records.Add( hCenter );

      SheetCenterRecord vCenter = ( SheetCenterRecord )BiffRecordFactory.GetRecord( TBIFFRecord.VCenter );
      vCenter.IsCenter = ( ushort )( m_bVCenter ? 1 : 0 );
      records.Add( vCenter );

      SerializeMargin( records, TBIFFRecord.LeftMargin, m_dLeftMargin, DEFAULT_LEFTMARGIN );
      SerializeMargin( records, TBIFFRecord.RightMargin, m_dRightMargin, DEFAULT_RIGHTMARGIN );
      SerializeMargin( records, TBIFFRecord.TopMargin, m_dTopMargin, DEFAULT_TOPMARGIN );
      SerializeMargin( records, TBIFFRecord.BottomMargin, m_dBottomMargin, DEFAULT_BOTTOMMARGIN );
      if( m_unknown != null )       records.Add( m_unknown );

      records.Add( m_setup );
      if (m_headerFooter != null && m_headerFooter.Length != -1)
      {
          records.Add(m_headerFooter);
      }
      SerializeEndRecords( records );
    }
    /// <summary>
    /// Serializes some records before main page setup block.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeStartRecords( OffsetArrayList records )
    {
    }
    /// <summary>
    /// Serializes some records after main page setup block.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeEndRecords( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_backgroundImage != null
#if !SILVERLIGHT && !WINRT && !WP
        && m_backgroundImage.Picture != null
#endif
)
      {
        records.Add(m_backgroundImage);
      }
    }
    /// <summary>
    /// Parses page setup.
    /// </summary>
    /// <param name="data">Array with biff records.</param>
    /// <param name="position">Starting position.</param>
    /// <returns>Position after parsing.</returns>
    public virtual int Parse( IList<BiffRecordRaw> data, int position )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( position < 0 || position > data.Count - 1 )
        throw new ArgumentOutOfRangeException( "position", "Value cannot be less than 0 and greater than data.Count - 1" );

      for( int len = data.Count; position < len; position++ )
      {
        BiffRecordRaw record = data[ position ];

        if( !ParseRecord( record ) )
        {
          position--;
          break;
        }
      }

      //m_arrHeaders = ParseHeaderFooterString( m_header.Value );
      //m_arrFooters = ParseHeaderFooterString( m_footer.Value );

      return position;
    }
    /// <summary>
    /// Parses record.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    /// <returns>True if record was successfully parsed, false otherwise.</returns>
    [ CLSCompliant( false ) ]
    protected virtual bool ParseRecord( BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      MarginRecord margin = null;

      switch( record.TypeCode )
      {
        case TBIFFRecord.Header:
          HeaderFooterRecord header  = ( HeaderFooterRecord )record;
          m_arrHeaders = ParseHeaderFooterString( header.Value );
          break;

        case TBIFFRecord.Footer:
          HeaderFooterRecord footer  = ( HeaderFooterRecord )record;
          m_arrFooters = ParseHeaderFooterString( footer.Value );
          break;

        case TBIFFRecord.HCenter:
          SheetCenterRecord hCenter = ( SheetCenterRecord )record;
          m_bHCenter = hCenter.IsCenter != 0;
          break;

        case TBIFFRecord.VCenter:
          SheetCenterRecord vCenter = ( SheetCenterRecord )record;
          m_bVCenter = vCenter.IsCenter != 0;
          break;

        case TBIFFRecord.PrintSetup:
          m_setup = ( PrintSetupRecord )record;
          break;

        case TBIFFRecord.Bitmap:
          m_backgroundImage = ( BitmapRecord )record;
          break;

        case TBIFFRecord.LeftMargin:
          margin = ( MarginRecord )record;
          m_dLeftMargin = margin.Margin;
          break;

        case TBIFFRecord.RightMargin:
          margin = ( MarginRecord )record;
          m_dRightMargin = margin.Margin;
          break;

        case TBIFFRecord.TopMargin:
          margin = ( MarginRecord )record;
          m_dTopMargin = margin.Margin;
          break;

        case TBIFFRecord.BottomMargin:
          margin = ( MarginRecord )record;
          m_dBottomMargin = margin.Margin;
          break;

        case TBIFFRecord.PrinterSettings:
          m_unknown = ( PrinterSettingsRecord )record;
          break;

        case TBIFFRecord.HeaderFooter:
          m_headerFooter = (HeaderAndFooterRecord)record;
          break;

        default:
          return false;
      }

      return true;
    }
    /// <summary>
    /// Returns record of the specified type from the array of Biff
    /// records and sets its position after the returned record.
    /// </summary>
    /// <param name="data">Array of Biff records.</param>
    /// <param name="pos">Starting from this position, record must be searched.</param>
    /// <param name="type">Type of the needed record.</param>
    /// <returns>Biff record if it was found, null otherwise.</returns>
    [ CLSCompliant( false ) ]
    protected BiffRecordRaw GetOrCreateRecord( IList data, ref int pos, TBIFFRecord type )
    {
      BiffRecordRaw rec = ( BiffRecordRaw )data[ pos ];

      if( rec.TypeCode != type )
      {
        rec = BiffRecordFactory.GetRecord( type );
      }
      else
      {
        pos++;
      }

      return rec;
    }
    /// <summary>
    /// Returns current record from the Biff records array and updates its position by 1.
    /// </summary>
    /// <param name="data">Array of Biff records.</param>
    /// <param name="pos">
    /// Position of the record in the array that will be returned.
    /// </param>
    /// <returns>Current record from array.</returns>
    [ CLSCompliant( false ) ]
    protected BiffRecordRaw GetRecordUpdatePos( IList data, ref int pos )
    {
      return ( BiffRecordRaw )data[ pos++ ];
    }
    /// <summary>
    /// Returns record of the specified type from the array of Biff
    /// records and sets its position after the returned record.
    /// </summary>
    /// <param name="data">Array of Biff records.</param>
    /// <param name="pos">Starting from this position, record must be searched.</param>
    /// <param name="type">Type of the needed record.</param>
    /// <returns>Biff record if it was found, null otherwise.</returns>
    [ CLSCompliant( false ) ]
    protected BiffRecordRaw GetRecordUpdatePos( IList data, ref int pos, TBIFFRecord type )
    {
      BiffRecordRaw rec = null;

      do
      {
        rec = ( BiffRecordRaw )data[ pos ];

        pos++;
        if( pos >= data.Count )
        {
          rec = null;
          break;
        }
      }
      while( rec.TypeCode != type );

      return rec;
    }
    /// <summary>
    /// Serializes margin.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    /// <param name="code">Margin's code.</param>
    /// <param name="marginValue">Margin value.</param>
    /// <param name="defaultValue">Default margin value.</param>
    private void SerializeMargin( OffsetArrayList records, TBIFFRecord code, double marginValue,
      double defaultValue )
    {
      if( marginValue != defaultValue )
      {
        MarginRecord margin = ( MarginRecord )BiffRecordFactory.GetRecord( code );
        margin.Margin = marginValue;
        records.Add( margin );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// This method is called after changes to the page setup.
    /// Sets Saved property of the parent workbook to the False state.
    /// </summary>
    protected void SetChanged()
    {
      m_sheet.SetChanged();
    }
    #endregion

    #region IBiffStorage Members

    /// <summary>
    /// Returns type code of the biff storage. Read-only.
    /// </summary>
    public Syncfusion.XlsIO.Parser.Biff_Records.TBIFFRecord TypeCode
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// Returns code of the biff storage. Read-only.
    /// </summary>
    public int RecordCode
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// Indicates whether data array is required by this record.
    /// </summary>
    public bool NeedDataArray
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Indicates record position in stream. This is a utility member of class and
    /// is used only in the serialization process. Does not influence the data.
    /// </summary>
    public long StreamPos
    {
      get
      {
        return -1;
      }
      set
      {
      }
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public virtual int GetStoreSize( ExcelVersion version )
    {
      // TODO:  Add PageSetupBaseImpl.GetStoreSize( version ) getter implementation
      int iResult = /*m_HCenter.GetStoreSize( version )*/2 + BiffRecordRaw.DEF_HEADER_SIZE
        + /*m_VCenter.GetStoreSize( version )*/2 + BiffRecordRaw.DEF_HEADER_SIZE
        + m_setup.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE
        + ( ( m_unknown != null ) ? m_unknown.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE : 0 )
        + ( ( m_dBottomMargin != DEFAULT_BOTTOMMARGIN ) ? 8 + BiffRecordRaw.DEF_HEADER_SIZE : 0 )
        + ( ( m_dTopMargin != DEFAULT_TOPMARGIN ) ? 8 + BiffRecordRaw.DEF_HEADER_SIZE : 0 )
        + ( ( m_dRightMargin != DEFAULT_RIGHTMARGIN ) ? 8 + BiffRecordRaw.DEF_HEADER_SIZE : 0 )
        + ( ( m_dLeftMargin != DEFAULT_LEFTMARGIN ) ? 8 + BiffRecordRaw.DEF_HEADER_SIZE : 0 );

      int iHeaderLen = FullHeaderString.Length;
      int iFooterLen = FullFooterString.Length;

      if( iHeaderLen > 0 )
        iResult += iHeaderLen * 2 + 3;

      iResult += BiffRecordRaw.DEF_HEADER_SIZE;

      // We don't add DEF_HEADER_SIZE since we need to return whole size without one header.
      if( iFooterLen > 0 )
        iResult += iFooterLen * 2 + 3;// + BiffRecordRaw.DEF_HEADER_SIZE;

      if( m_backgroundImage != null )
      {
        iResult += m_backgroundImage.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE;
      }
      if (m_headerFooter != null && m_headerFooter.Length > 0)
      {
          iResult += m_headerFooter.GetStoreSize(version) + BiffRecordRaw.DEF_HEADER_SIZE;
      }
      return iResult;
    }

    /// <summary>
    /// Save record data to stream.
    /// </summary>
    /// <param name="writer">Writer that will receive record data.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns>Size of the record.</returns>
    /// <exception cref="System.ArgumentNullException">If writer is NULL.</exception>
    /// <exception cref="System.ApplicationException">
    ///   If m_iLength of internal record data array is less than zero.
    /// </exception>
    public int FillStream( System.IO.BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, int streamPosition )
    {
      int iResult = FillStreamStart( writer, provider, encryptor, streamPosition );

      iResult += SerializeHeaderFooterString( writer, provider, encryptor,
        TBIFFRecord.Header, FullHeaderString, streamPosition + iResult );

      iResult += SerializeHeaderFooterString( writer, provider, encryptor,
        TBIFFRecord.Footer, FullFooterString, streamPosition + iResult );

      iResult += WriteUShortRecord( writer, provider, encryptor, TBIFFRecord.HCenter,
        ( ushort )( m_bHCenter ? 1 : 0 ), streamPosition + iResult );

      iResult += WriteUShortRecord( writer, provider, encryptor, TBIFFRecord.VCenter,
        ( ushort )( m_bVCenter ? 1 : 0 ), streamPosition + iResult );

      iResult += FillStreamWithMargin( writer, provider, encryptor, TBIFFRecord.LeftMargin,
        m_dLeftMargin, DEFAULT_LEFTMARGIN, streamPosition + iResult );

      iResult += FillStreamWithMargin( writer, provider, encryptor, TBIFFRecord.RightMargin,
        m_dRightMargin, DEFAULT_RIGHTMARGIN, streamPosition + iResult );

      iResult += FillStreamWithMargin( writer, provider, encryptor, TBIFFRecord.TopMargin,
        m_dTopMargin, DEFAULT_TOPMARGIN, streamPosition + iResult );

      iResult += FillStreamWithMargin( writer, provider, encryptor, TBIFFRecord.BottomMargin,
        m_dBottomMargin, DEFAULT_BOTTOMMARGIN, streamPosition + iResult );

      if( m_unknown != null )
        iResult += m_unknown.FillStream( writer, provider, encryptor, streamPosition + iResult );

      iResult += m_setup.FillStream( writer, provider, encryptor, streamPosition + iResult );

      if (m_headerFooter != null && m_headerFooter.Length > 0)
          iResult += m_headerFooter.FillStream(writer, provider, encryptor, streamPosition + iResult);
      
      iResult += FillStreamEnd( writer, provider, encryptor, streamPosition + iResult );
      return iResult;
    }

    /// <summary>
    /// Serializes header / footer string.
    /// </summary>
    /// <param name="writer">Writer to write data into.</param>
    /// <param name="provider">Object that gives access to the temporary buffer.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="code">Record code to serialize (header or footer).</param>
    /// <param name="value">String value to serialize.</param>
    /// <param name="streamPosition">Position in the output stream, used to reduce Flush
    /// calls of the writer.BaseStream.</param>
    private int SerializeHeaderFooterString( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, TBIFFRecord code, string value, int streamPosition )
    {
      int iOffset = 0;
      provider.WriteUInt16( iOffset, ( ushort )code );
      iOffset += 2;

      int iLength = ( value != null ) ? value.Length * 2 : 0;

      if( iLength > 0 ) iLength += 3; // we have to add size for string length and unicode identifier.

      provider.WriteInt16( iOffset, ( short )iLength );
      iOffset += 2;

      provider.WriteString16BitUpdateOffset( ref iOffset, value );
      iLength += BiffRecordRaw.DEF_HEADER_SIZE;
      // TODO: this must be changed.
      ByteArrayDataProvider byteArrayProvider = ( ByteArrayDataProvider )provider;

      if( encryptor != null )
      {
        encryptor.Encrypt( provider, BiffRecordRaw.DEF_HEADER_SIZE, iLength,
          streamPosition + BiffRecordRaw.DEF_HEADER_SIZE );
      }

      provider.WriteInto( writer, 0, iLength, byteArrayProvider.InternalBuffer );
      return iLength;
    }
    /// <summary>
    /// Writes record that contains single UInt16 value into the writer.
    /// </summary>
    /// <param name="writer">Writer to write value into.</param>
    /// <param name="provider">Object that gives access to the temporary buffer.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="code">Record code to write.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="streamPosition">Position in the output stream, used to reduce Flush
    /// calls of the writer.BaseStream.</param>
    /// <returns>Size of the written data.</returns>
    [ CLSCompliant( false ) ]
    protected int WriteUShortRecord( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, TBIFFRecord code, ushort value, int streamPosition )
    {
      provider.WriteUInt16( 0, ( ushort )code );
      provider.WriteUInt16( 2, 2 );
      provider.WriteUInt16( 4, value );

      if( encryptor != null )
      {
        encryptor.Encrypt( provider, 4, 2, streamPosition + BiffRecordRaw.DEF_HEADER_SIZE );
      }

      provider.WriteInto( writer, 0, 6, null );

      return 6;
    }
    /// <summary>
    /// Writes margin record into writer.
    /// </summary>
    /// <param name="writer">Writer to write margin into.</param>
    /// <param name="provider">Object that gives access to the temporary buffer.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="code">Record code to write.</param>
    /// <param name="value">Margin value.</param>
    /// <param name="defaultValue">Default margin value. If value equals to defaultValue,
    /// then record is not written into writer.</param>
    /// <param name="streamPosition">Position in the output stream, used to reduce Flush
    /// calls of the writer.BaseStream.</param>
    /// <returns>Size of the written data.</returns>
    private int FillStreamWithMargin( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, TBIFFRecord code, double value, double defaultValue, int streamPosition )
    {
      int iResult = 0;

      if( value != defaultValue )
      {
        provider.WriteUInt16( 0, ( ushort )code );
        provider.WriteUInt16( 2, 8 );
        provider.WriteDouble( 4, value );
        
        if( encryptor != null )
        {
          encryptor.Encrypt( provider, 4, 8, streamPosition + BiffRecordRaw.DEF_HEADER_SIZE );
        }

        provider.WriteInto( writer, 0, 12, null );
        iResult = 12;
      }

      return iResult;
    }
    /// <summary>
    /// Fills stream with some records before main page setup records.
    /// </summary>
    /// <param name="writer">Writer to write records into.</param>
    /// <param name="provider">Object that gives access to the temporary buffer.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns>Size of the serialized data.</returns>
    protected virtual int FillStreamStart( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, int streamPosition )
    {
      return 0;
    }
    /// <summary>
    /// Fills stream with some records after main page setup records.
    /// </summary>
    /// <param name="writer">Writer to write records into.</param>
    /// <param name="provider">Object that gives access to the temporary buffer.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns>Size of the serialized data.</returns>
    protected virtual int FillStreamEnd( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, int streamPosition )
    {
      int iResult = 0;

      if( m_backgroundImage != null
#if !SILVERLIGHT && !WINRT && !WP
        && m_backgroundImage.Picture != null
#endif
        )
      {
        iResult = m_backgroundImage.FillStream( writer, provider, encryptor, streamPosition );
      }

      return iResult;
    }
    #endregion

      #region Dispose
    public override void Dispose()
    {
        base.Dispose();
        if (m_setup != null)
        {
            m_setup = null;
  }
        if (m_unknown != null && m_sheet == null)
        {
            m_unknown.Dispose();
        }
        if (dictPaperHeight != null)
        {
            dictPaperHeight.Clear();
            dictPaperHeight = null;
        }
        if (dictPaperWidth != null)
        {
            dictPaperWidth.Clear();
            dictPaperWidth = null;
        }
        //m_sheet = null;

        GC.SuppressFinalize(this);
    }
      #endregion
  }
}
