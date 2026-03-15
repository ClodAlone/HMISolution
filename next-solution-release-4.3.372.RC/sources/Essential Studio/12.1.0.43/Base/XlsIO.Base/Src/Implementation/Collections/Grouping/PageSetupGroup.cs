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

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Drawing;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections.Grouping
{
  /// <summary>
  /// Summary description for PageSetupGroup.
  /// </summary>
  public class PageSetupGroup
    : CommonObject
    , IPageSetup
  {
    #region Class members
    /// <summary>
    /// Parent group of worksheets.
    /// </summary>
    private WorksheetGroup m_sheetGroup;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance and sets its application and parent properties.
    /// </summary>
    /// <param name="application">Application object for the new instance.</param>
    /// <param name="parent">Parent object for the new instance.</param>
    public PageSetupGroup( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
    }
    /// <summary>
    /// Looks for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_sheetGroup = FindParent( typeof( WorksheetGroup ) ) as WorksheetGroup;

      if( m_sheetGroup == null )
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent group." );
    }
    #endregion

    #region IPageSetup Members
    /// <summary>
    /// Indicates whether summary rows will appear below detail in outlines.
    /// </summary>
    public bool AutoFirstPageNumber
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.AutoFirstPageNumber;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.AutoFirstPageNumber;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.AutoFirstPageNumber = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the number of pages tall the worksheet will be scaled
    /// to when it is printed. Applies only to worksheets. Read / write int.
    /// </summary>
    public int FitToPagesTall
    {
      get
      {
        int result = m_sheetGroup[ 0 ].PageSetup.FitToPagesTall;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          int curValue = m_sheetGroup[ i ].PageSetup.FitToPagesTall;

          if( curValue != result )
          {
            return int.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.FitToPagesTall = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the number of pages wide the worksheet will be scaled
    /// to when it is printed. Applies only to worksheets. Read / write int.
    /// </summary>
    public int FitToPagesWide
    {
      get
      {
        int result = m_sheetGroup[ 0 ].PageSetup.FitToPagesWide;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          int curValue = m_sheetGroup[ i ].PageSetup.FitToPagesWide;

          if( curValue != result )
          {
            return int.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.FitToPagesWide = value;
        }
      }
    }

    /// <summary>
    /// True if cell gridlines are printed on the page. Applies only to
    /// worksheets. Read / write Boolean.
    /// </summary>
    public bool PrintGridlines
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.PrintGridlines;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.PrintGridlines;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.PrintGridlines = value;
        }
      }
    }

    /// <summary>
    /// True if row and column headings are printed with this page. Applies
    /// only to worksheets. Read / write Boolean.
    /// </summary>
    public bool PrintHeadings
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.PrintHeadings;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.PrintHeadings;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.PrintHeadings = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the range to be printed, as a string using A1-style
    /// references in the language of the macro. Read / write String.
    /// </summary>
    public string PrintArea
    {
      get
      {
        string result = m_sheetGroup[ 0 ].PageSetup.PrintArea;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = m_sheetGroup[ i ].PageSetup.PrintArea;

          if( curValue != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.PrintArea = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the columns that contain the cells to be repeated
    /// on the left side of each page, as a string in A1-style notation
    /// in the language of the macro. Read / write String.
    /// </summary>
    public string PrintTitleColumns
    {
      get
      {
        string result = m_sheetGroup[ 0 ].PageSetup.PrintTitleColumns;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = m_sheetGroup[ i ].PageSetup.PrintTitleColumns;

          if( curValue != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.PrintTitleColumns = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the rows that contain the cells to be repeated at
    /// the top of each page, as a string in A1-style notation in the
    /// language of the macro. Read / write String.
    /// </summary>
    public string PrintTitleRows
    {
      get
      {
        string result = m_sheetGroup[ 0 ].PageSetup.PrintTitleRows;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = m_sheetGroup[ i ].PageSetup.PrintTitleRows;

          if( curValue != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.PrintTitleRows = value;
        }
      }
    }

    /// <summary>
    /// Indicates whether summary rows will appear below detail in outlines.
    /// </summary>
    public bool IsSummaryRowBelow
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.IsSummaryRowBelow;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.IsSummaryRowBelow;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.IsSummaryRowBelow = value;
        }
      }
    }

    /// <summary>
    /// Indicates whether summary columns will appear right of the detail in outlines.
    /// </summary>
    public bool IsSummaryColumnRight
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.IsSummaryColumnRight;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.IsSummaryColumnRight;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.IsSummaryColumnRight = value;
        }
      }
    }

    /// <summary>
    /// Indicates whether fit to page mode is selected.
    /// </summary>
    public bool IsFitToPage
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.IsFitToPage;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.IsFitToPage;

          if( curValue != result || !curValue )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.IsFitToPage = value;
        }
      }
    }
    #endregion

    #region IPageSetupBase Members
    /// <summary>
    /// True if elements of the document will be printed in black and white.
    /// Read / write Boolean.
    /// </summary>
    public bool BlackAndWhite
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.BlackAndWhite;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.BlackAndWhite;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.BlackAndWhite = value;
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
        double result = m_sheetGroup[ 0 ].PageSetup.BottomMargin;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          double curValue = m_sheetGroup[ i ].PageSetup.BottomMargin;

          if( curValue != result )
          {
            return double.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.BottomMargin = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the center part of the footer. Read / write String.
    /// </summary>
    public string CenterFooter
    {
      get
      {
        string result = m_sheetGroup[ 0 ].PageSetup.CenterFooter;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = m_sheetGroup[ i ].PageSetup.CenterFooter;

          if( curValue != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.CenterFooter = value;
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
        return null;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.CenterFooterImage = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the center part of the header. Read / write String.
    /// </summary>
    public string CenterHeader
    {
      get
      {
        string result = m_sheetGroup[ 0 ].PageSetup.CenterHeader;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = m_sheetGroup[ i ].PageSetup.CenterHeader;

          if( curValue != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.CenterHeader = value;
        }
      }
    }
    /// <summary>
    /// Gets / set image for center part of the header.
    /// </summary>
    public Image CenterHeaderImage
    {
      get
      {
        return null;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.CenterHeaderImage = value;
        }
      }
    }
    /// <summary>
    /// True if the sheet is centered horizontally on the page when it is
    /// printed. Read / write Boolean.
    /// </summary>
    public bool CenterHorizontally
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.CenterHorizontally;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.CenterHorizontally;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.CenterHorizontally = value;
        }
      }
    }

    /// <summary>
    /// True if the sheet is centered vertically on the page when it is
    /// printed. Read / write Boolean.
    /// </summary>
    public bool CenterVertically
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.CenterVertically;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.CenterVertically;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.CenterVertically = value;
        }
      }
    }

    /// <summary>
    /// Number of copies to print.
    /// </summary>
    public int Copies
    {
      get
      {
        int result = m_sheetGroup[ 0 ].PageSetup.Copies;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          int curValue = m_sheetGroup[ i ].PageSetup.Copies;

          if( curValue != result )
          {
            return int.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.Copies = value;
        }
      }
    }

    /// <summary>
    /// True if the sheet will be printed without graphics.
    /// Read / write Boolean.
    /// </summary>
    public bool Draft
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.Draft;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.Draft;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.Draft = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the first page number that will be used when
    /// this sheet is printed. If xlAutomatic, Microsoft Excel chooses the
    /// first page number. The default is xlAutomatic. Read / write Long.
    /// </summary>
    public short FirstPageNumber
    {
      get
      {
        short result = m_sheetGroup[ 0 ].PageSetup.FirstPageNumber;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          short curValue = m_sheetGroup[ i ].PageSetup.FirstPageNumber;

          if( curValue != result )
          {
            return short.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.FirstPageNumber = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the distance from the bottom of the page to the footer,
    /// in inches. Read / write Double.
    /// </summary>
    public double FooterMargin
    {
      get
      {
        double result = m_sheetGroup[ 0 ].PageSetup.FooterMargin;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          double curValue = m_sheetGroup[ i ].PageSetup.FooterMargin;

          if( curValue != result )
          {
            return double.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.FooterMargin = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the distance from the top of the page to the header,
    /// in inches. Read / write Double.
    /// </summary>
    public double HeaderMargin
    {
      get
      {
        double result = m_sheetGroup[ 0 ].PageSetup.HeaderMargin;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          double curValue = m_sheetGroup[ i ].PageSetup.HeaderMargin;

          if( curValue != result )
          {
            return double.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.HeaderMargin = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the left part of the footer. Read / write String.
    /// &amp;L Left aligns the characters that follow.
    /// &amp;C Centers the characters that follow.
    /// &amp;R Right aligns the characters that follow.
    /// &amp;E Turns double-underline printing on or off.
    /// &amp;X Turns superscript printing on or off.
    /// &amp;Y Turns subscript printing on or off.
    /// &amp;B Turns bold printing on or off.
    /// &amp;I Turns italic printing on or off.
    /// &amp;U Turns underline printing on or off.
    /// &amp;S Turns strikethrough printing on or off.
    /// &amp;D Prints the current date.
    /// &amp;T Prints the current time.
    /// &amp;F Prints the name of the document.
    /// &amp;A Prints the name of the workbook tab.
    /// &amp;P Prints the page number.
    /// &amp;P+number Prints the page number plus the specified number.
    /// &amp;P-number Prints the page number minus the specified number.
    /// &amp;&amp; Prints a single ampersand.
    /// &amp; "fontname" Prints the characters that follow in the specified font. Be sure to include the double quotation marks.
    /// &amp;nn Prints the characters that follow in the specified font size. Use a two-digit number to specify a size in points.
    /// &amp;N Prints the total number of pages in the document.
    /// </summary>
    public string LeftFooter
    {
      get
      {
        string result = m_sheetGroup[ 0 ].PageSetup.LeftFooter;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = m_sheetGroup[ i ].PageSetup.LeftFooter;

          if( curValue != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.LeftFooter = value;
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
        return null;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.LeftFooterImage = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the left part of the header. Read / write String.
    /// </summary>
    public string LeftHeader
    {
      get
      {
        string result = m_sheetGroup[ 0 ].PageSetup.LeftHeader;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = m_sheetGroup[ i ].PageSetup.LeftHeader;

          if( curValue != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.LeftHeader = value;
        }
      }
    }
    /// <summary>
    /// Gets / set image for left part of the header.
    /// </summary>
    public Image LeftHeaderImage
    {
      get
      {
        return null;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.LeftHeaderImage = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the size of the left margin, in inches.
    /// Read / write Double.
    /// </summary>
    public double LeftMargin
    {
      get
      {
        double result = m_sheetGroup[ 0 ].PageSetup.LeftMargin;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          double curValue = m_sheetGroup[ i ].PageSetup.LeftMargin;

          if( curValue != result )
          {
            return double.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.LeftMargin = value;
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
        ExcelOrder result = m_sheetGroup[ 0 ].PageSetup.Order;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          ExcelOrder curValue = m_sheetGroup[ i ].PageSetup.Order;

          if( curValue != result )
          {
            return ExcelOrder.DownThenOver;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.Order = value;
        }
      }
    }

    /// <summary>
    /// Portrait or landscape printing mode. Read / write ExcelPageOrientation.
    /// </summary>
    public ExcelPageOrientation Orientation
    {
      get
      {
        ExcelPageOrientation result = m_sheetGroup[ 0 ].PageSetup.Orientation;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          ExcelPageOrientation curValue = m_sheetGroup[ i ].PageSetup.Orientation;

          if( curValue != result )
          {
            return ExcelPageOrientation.Portrait;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.Orientation = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the size of the paper. Read / write ExcelPaperSize.
    /// </summary>
    public ExcelPaperSize PaperSize
    {
      get
      {
        ExcelPaperSize result = m_sheetGroup[ 0 ].PageSetup.PaperSize;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          ExcelPaperSize curValue = m_sheetGroup[ i ].PageSetup.PaperSize;

          if( curValue != result )
          {
            return ExcelPaperSize.PaperA4;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.PaperSize = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the way comments are printed with the sheet.
    /// Read / write ExcelPrintLocation.
    /// </summary>
    public ExcelPrintLocation PrintComments
    {
      get
      {
        ExcelPrintLocation result = m_sheetGroup[ 0 ].PageSetup.PrintComments;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          ExcelPrintLocation curValue = m_sheetGroup[ i ].PageSetup.PrintComments;

          if( curValue != result )
          {
            return ExcelPrintLocation.PrintInPlace;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.PrintComments = value;
        }
      }
    }

    /// <summary>
    /// Sets or returns an ExcelPrintErrors constant specifying the type of
    /// print error displayed. This feature allows users to suppress the
    /// display of error values when printing a worksheet. Read / write.
    /// </summary>
    public ExcelPrintErrors PrintErrors
    {
      get
      {
        ExcelPrintErrors result = m_sheetGroup[ 0 ].PageSetup.PrintErrors;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          ExcelPrintErrors curValue = m_sheetGroup[ i ].PageSetup.PrintErrors;

          if( curValue != result )
          {
            return ExcelPrintErrors.PrintErrorsDisplayed;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.PrintErrors = value;
        }
      }
    }

    /// <summary>
    /// True if cell notes are printed as end notes with the sheet. Applies
    /// only to worksheets. Read / write Boolean.
    /// </summary>
    public bool PrintNotes
    {
      get
      {
        bool result = m_sheetGroup[ 0 ].PageSetup.PrintNotes;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = m_sheetGroup[ i ].PageSetup.PrintNotes;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.PrintNotes = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the print quality. Read / write ushort.
    /// </summary>
    public int PrintQuality
    {
      get
      {
        int result = m_sheetGroup[ 0 ].PageSetup.PrintQuality;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          int curValue = m_sheetGroup[ i ].PageSetup.PrintQuality;

          if( curValue != result )
          {
            return int.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.PrintQuality = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the right part of the footer. Read / write String.
    /// </summary>
    public string RightFooter
    {
      get
      {
        string result = m_sheetGroup[ 0 ].PageSetup.RightFooter;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = m_sheetGroup[ i ].PageSetup.RightFooter;

          if( curValue != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.RightFooter = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the right part of the header. Read / write String.
    /// </summary>
    public string RightHeader
    {
      get
      {
        string result = m_sheetGroup[ 0 ].PageSetup.RightHeader;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = m_sheetGroup[ i ].PageSetup.RightHeader;

          if( curValue != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.RightHeader = value;
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
        return null;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.RightFooterImage = value;
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
        return null;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.RightHeaderImage = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the size of the right margin, in inches.
    /// Read / write Double.
    /// </summary>
    public double RightMargin
    {
      get
      {
        double result = m_sheetGroup[ 0 ].PageSetup.RightMargin;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          double curValue = m_sheetGroup[ i ].PageSetup.RightMargin;

          if( curValue != result )
          {
            return double.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.RightMargin = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the size of the top margin, in inches.
    /// Read / write Double.
    /// </summary>
    public double TopMargin
    {
      get
      {
        double result = m_sheetGroup[ 0 ].PageSetup.TopMargin;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          double curValue = m_sheetGroup[ i ].PageSetup.TopMargin;

          if( curValue != result )
          {
            return double.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.TopMargin = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets a percentage (between 10 and 400 percent) by which
    /// Microsoft Excel will scale the worksheet for printing. Applies only
    /// to worksheets. Read / write ushort.
    /// </summary>
    public int Zoom
    {
      get
      {
        int result = m_sheetGroup[ 0 ].PageSetup.Zoom;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          int curValue = m_sheetGroup[ i ].PageSetup.Zoom;

          if( curValue != result )
          {
            return int.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          m_sheetGroup[ i ].PageSetup.Zoom = value;
        }
      }
    }

    /// <summary>
    /// Indicates whether the header and footer margins are aligned with page margins. Read/Write Boolean.
    /// </summary>
    public bool AlignHFWithPageMargins
    {
        get
        {
            bool result = m_sheetGroup[0].PageSetup.AlignHFWithPageMargins;

            for (int i = 1, len = m_sheetGroup.Count; i < len; i++)
            {
                bool curValue = m_sheetGroup[i].PageSetup.AlignHFWithPageMargins;

                if (curValue != result)
                {
                    return false;
                }
            }

            return result;
        }
        set
        {
            for (int i = 0, len = m_sheetGroup.Count; i < len; i++)
            {
                m_sheetGroup[i].PageSetup.AlignHFWithPageMargins = value;
            }
        }
    }
    /// <summary>
    /// True - The header / footer of the first page is different with other pages.Otherwise False.
    /// </summary>
    public bool DifferentFirstPageHF
    {
        get
        {
            bool result = m_sheetGroup[0].PageSetup.DifferentFirstPageHF;

            for (int i = 1, len = m_sheetGroup.Count; i < len; i++)
            {
                bool curValue = m_sheetGroup[i].PageSetup.DifferentFirstPageHF;

                if (curValue != result)
                {
                    return false;
                }
            }

            return result;
        }
        set
        {
            for (int i = 0, len = m_sheetGroup.Count; i < len; i++)
            {
                m_sheetGroup[i].PageSetup.DifferentFirstPageHF = value;
            }
        }
    }
    /// <summary>
    /// True - The header/footer odd pages are differed with even page. Otherwise False.
    /// </summary>
    public bool DifferentOddAndEvenPagesHF
    {
        get
        {
            bool result = m_sheetGroup[0].PageSetup.DifferentOddAndEvenPagesHF;

            for (int i = 1, len = m_sheetGroup.Count; i < len; i++)
            {
                bool curValue = m_sheetGroup[i].PageSetup.DifferentOddAndEvenPagesHF;

                if (curValue != result)
                {
                    return false;
                }
            }

            return result;
        }
        set
        {
            for (int i = 0, len = m_sheetGroup.Count; i < len; i++)
            {
                m_sheetGroup[i].PageSetup.DifferentOddAndEvenPagesHF = value;
            }
        }
    }
    /// <summary>
    /// Indicates whether the header and footer are scaled with document scaling.Read/Write Boolean.
    /// </summary>
    public bool HFScaleWithDoc
    {
        get
        {
            bool result = m_sheetGroup[0].PageSetup.HFScaleWithDoc;

            for (int i = 1, len = m_sheetGroup.Count; i < len; i++)
            {
                bool curValue = m_sheetGroup[i].PageSetup.HFScaleWithDoc;

                if (curValue != result)
                {
                    return false;
                }
            }

            return result;
        }
        set
        {
            for (int i = 0, len = m_sheetGroup.Count; i < len; i++)
            {
                m_sheetGroup[i].PageSetup.HFScaleWithDoc = value;
            }
        }
    }
    /// <summary>
    /// Gets / sets background image.
    /// </summary>
    public
#if !SILVERLIGHT && !WINRT && !WP
      Bitmap
#else
      Image
#endif
      BackgoundImage
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }
    #endregion
  }
}
