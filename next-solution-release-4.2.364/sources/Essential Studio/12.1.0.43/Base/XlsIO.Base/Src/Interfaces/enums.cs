#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;

namespace Syncfusion.XlsIO
{
  /// <summary>
  ///Enumeration of the sheet types in Excel.
  /// </summary>
  public enum ExcelSheetType
  {
#if ( WINRT )
      /// <summary>
      ///Charts.
      /// </summary>
      Chart = 2,
      /// <summary>
      ///Dialogs.
      /// </summary>
      DialogSheet,
      /// <summary>
      ///Excel 4.0 International Macros.
      /// </summary>
      Excel4IntlMacroSheet,
      /// <summary>
      ///Excel 4.0 Macros.
      /// </summary>
      Excel4MacroSheet,
      /// <summary>
      ///Worksheets.
      /// </summary>
      Worksheet = 0,
#else

    /// <summary>
    ///Charts.
    /// </summary>
    [ Description( "Charts" ) ]
    Chart = 2,
    /// <summary>
    ///Dialogs.
    /// </summary>
    [ Description( "Dialogs" ) ]
    DialogSheet,
    /// <summary>
    ///Excel 4.0 International Macros.
    /// </summary>
    [ Description( "Excel 4.0 Intl Marcos" ) ]
    Excel4IntlMacroSheet,
    /// <summary>
    ///Excel 4.0 Macros.
    /// </summary>
    [ Description( "Excel 4.0 Macros" ) ]
    Excel4MacroSheet,
    /// <summary>
    ///Worksheets.
    /// </summary>
    [ Description( "Worksheets" ) ]
    Worksheet = 0,
#endif
  }

  /// <summary>
  ///Enumeration of the border line styles for cells in Excel.
  /// </summary>
  public enum ExcelLineStyle
  {
    /// <summary>
    ///Represents no border line style.
    /// </summary>
    None                = 0x0,
    /// <summary>
    ///Represents the thin border line style.
    /// </summary>
    Thin                = 0x1,
    /// <summary>
    ///Represents the medium border line style.
    /// </summary>
    Medium              = 0x2,
    /// <summary>
    ///Represents the dashed border line style.
    /// </summary>
    Dashed              = 0x3,
    /// <summary>
    ///Represents the dotted border line style.
    /// </summary>
    Dotted              = 0x4,
    /// <summary>
    ///Represents the thick border line style.
    /// </summary>
    Thick               = 0x5,
    /// <summary>
    ///Represents the double border line style.
    /// </summary>
    Double              = 0x6,
    /// <summary>
    ///Represents the hair border line style.
    /// </summary>
    Hair                = 0x7,
    /// <summary>
    ///Represents the medium_dashed medium_dashed line style.
    /// </summary>
    Medium_dashed       = 0x8,
    /// <summary>
    ///Represents the dash_dot border line style.
    /// </summary>
    Dash_dot            = 0x9,
    /// <summary>
    ///Represents the medium dash_dot border line style.
    /// </summary>
    Medium_dash_dot     = 0xA,
    /// <summary>
    ///Represents the dash_dot_dot border line style.
    /// </summary>
    Dash_dot_dot        = 0xB,
    /// <summary>
    ///Represents the medium dash_dot_dot border line style.
    /// </summary>
    Medium_dash_dot_dot = 0xC,
    /// <summary>
    ///Represents the medium slanted_dash_dot border line style.
    /// </summary>
    Slanted_dash_dot    = 0xD
  }

  /// <summary>
  ///Enumeration of the border and diagonal line types in Excel.
  /// </summary>
  public enum ExcelBordersIndex
  {
    /// <summary>
    ///Represents the diagonal line from top left to right bottom.
    /// </summary>
    DiagonalDown      = 0x00000005,
    /// <summary>
    /// Represents the diagonal line from bottom left to right top.
    /// </summary>
    DiagonalUp        = 0x00000006,
    /// <summary>
    ///Represents the border line on the bottom.
    /// </summary>
    EdgeBottom        = 0x00000009,
    /// <summary>
    ///Represents the border line on the left.
    /// </summary>
    EdgeLeft          = 0x00000007,
    /// <summary>
    ///Represents the border line on the right.
    /// </summary>
    EdgeRight         = 0x0000000A,
    /// <summary>
    ///Represents the border line on the top.
    /// </summary>
    EdgeTop           = 0x00000008,
  }

  /// <summary>
  ///Enumeration of the label types for formula in Excel.
  /// </summary>
  public enum ExcelFormulaLabel
  {
    /// <summary>
    ///Represents Column label for formula.
    /// </summary>
    ColumnLabels, // = 0x00000002,
    /// <summary>
    ///Represents Mixed label for formula.
    /// </summary>
    MixedLabels,  // = 0x00000003,
    /// <summary>
    ///Represents no label for formula.
    /// </summary>
    NoLabels,     // = 0xFFFFEFD2,
    /// <summary>
    ///Represents row label for formula.
    /// </summary>
    RowLabels,    // = 0x00000001,
  }

  /// <summary>
  ///Enumeration of the border weight types for lines/pictures in Excel.
  /// </summary>
  public enum ExcelBorderWeight //: uint
  {
    /// <summary>
    ///Represents weight of a Hairline border type.
    /// </summary>
    Hairline, // = 0x00000001,
    /// <summary>
    ///Represents weight of a Medium border type.
    /// </summary>
    Medium,   // = 0xFFFFEFD6,
    /// <summary>
    ///Represents weight of a Thick border type.
    /// </summary>
    Thick,    // = 0x00000004,
    /// <summary>
    ///Represents weight of a Thin border type.
    /// </summary>
    Thin,     // = 0x00000002,
    /// <summary>
    ///Represents weight of a None border type.
    /// </summary>
    None,     // = 0xFFFFFFFF
  }

  /// <summary>
  ///Enumeration of the Color Index in Excel.
  /// </summary>
  public enum ExcelColorIndex //: uint
  {
    /// <summary>
    ///Represents the automatic color index.
    /// </summary>
    ColorIndexAutomatic,   // = 0xFFFFEFF7,
    /// <summary>
    ///Represents no color index.
    /// </summary>
    ColorIndexNone,        // = 0xFFFFEFD2,
  }
  /// <summary>
  /// Enum that defines different types of the formula calculations.
  /// </summary>
  public enum ExcelCalculationMode : int
  {
    /// <summary>
    /// Represents the MANUAL calculation type.
    /// </summary>
    Manual = 0,
    /// <summary>
    /// Represents the AUTOMATIC calculation type.
    /// </summary>
    Automatic = 1,
    /// <summary>
    /// Represents the AUTOMATIC EXCEPT TABLES calculation type.
    /// </summary>
    AutomaticExceptTables,
  }
  /// <summary>
  ///Enumeration of the horizontal alignment options for cell formatting in Excel.
  /// </summary>
  public enum ExcelHAlign //: ushort//uint
  {
    /// <summary>
    ///Represents the general horizontal alignment setting.
    /// </summary>
    HAlignGeneral               = 0x0000,
    /// <summary>
    ///Represents left horizontal alignment setting.
    /// </summary>
    HAlignLeft                  = 0x0001,
    /// <summary>
    ///Represents center horizontal alignment setting.
    /// </summary>
    HAlignCenter                = 0x0002,
    /// <summary>
    ///Represents center horizontal alignment setting.
    /// </summary>
    HAlignRight                 = 0x0003,
    /// <summary>
    ///Represents fill horizontal alignment setting.
    /// </summary>
    HAlignFill                  = 0x0004,
    /// <summary>
    ///Represents justify  horizontal alignment setting.
    /// </summary>
    HAlignJustify               = 0x0005,
    /// <summary>
    ///Represents center across selection horizontal alignment setting.
    /// </summary>
    HAlignCenterAcrossSelection = 0x0006,
    /// <summary>
    ///Represents distributed horizontal alignment setting.
    /// </summary>
    HAlignDistributed           = 0x0007,
  }
  /// <summary>
  ///Enumeration of the vertical alignment options for cell formatting in Excel.
  /// </summary>
  public enum ExcelVAlign //: ushort
  {
    /// <summary>
    ///Represents top vertical alignment setting.
    /// </summary>
    VAlignTop         = 0x0000,
    /// <summary>
    ///Represents center vertical alignment setting.
    /// </summary>
    VAlignCenter      = 0x0001,
    /// <summary>
    ///Represents bottom vertical alignment setting.
    /// </summary>
    VAlignBottom      = 0x0002,
    /// <summary>
    ///Represents justify vertical alignment setting.
    /// </summary>
    VAlignJustify     = 0x0003,
    /// <summary>
    ///Represents distributed vertical alignment setting.
    /// </summary>
    VAlignDistributed = 0x0004,
  }

  /// <summary>
  ///Enumeration of page order for sheet in Excel.
  /// </summary>
  public enum ExcelOrder
  {
    /// <summary>
    ///Represents Down, then over setting.
    /// </summary>
    DownThenOver = 0x00000001,
    /// <summary>
    ///Represents Over, then down setting.
    /// </summary>
    OverThenDown = 0x00000002,
  }

  /// <summary>
  ///Enumeration of page orientation types in Excel.
  /// </summary>
  public enum ExcelPageOrientation
  {
    /// <summary>
    ///Represents landscape setting.
    /// </summary>
    Landscape = 0x00000002,
    /// <summary>
    ///Represents portrait setting.
    /// </summary>
    Portrait = 0x00000001,
  }

  /// <summary>
  ///Enumeration of paper size types in Excel.
  /// </summary>
  public enum ExcelPaperSize
  {
    /// <summary>
    /// Represents paper size of 10 inches X 14 inches 
    /// </summary>
    Paper10x14 = 0x00000010,
    /// <summary>
    ///Represents paper size of 11 inches X 17 inches 
    /// </summary>
    Paper11x17 = 0x00000011,
    /// <summary>
    ///Represents A3 (297 mm x  420 mm) paper size.
    /// </summary>
    PaperA3 = 0x00000008,
    /// <summary>
    ///Represents A4 (210 mm x  297 mm) paper size.
    /// </summary>
    PaperA4 = 0x00000009,
    /// <summary>
    ///Represents A4 Small (210 mm x  297 mm) paper size.
    /// </summary>
    PaperA4Small = 0x0000000A,
    /// <summary>
    ///Represents A5 (148 mm x  210 mm) paper size.
    /// </summary>
    PaperA5 = 0x0000000B,
    /// <summary>
    ///Represents B4 (250 mm x  353 mm) paper size.
    /// </summary>
    PaperB4 = 0x0000000C,
    /// <summary>
    ///Represents B5 (176 mm x  250 mm) paper size.
    /// </summary>
    PaperB5 = 0x0000000D,
    /// <summary>
    ///Represents C paper size.
    /// </summary>
    PaperCsheet = 0x00000018,
    /// <summary>
    ///Represents D paper size.
    /// </summary>
    PaperDsheet = 0x00000019,
    /// <summary>
    ///Represents Envelope# 10 paper size(4-1/8 X 9-1/2 inches). 
    /// </summary>
    PaperEnvelope10 = 0x00000014,
    /// <summary>
    ///Represents Envelope# 11 paper size( (4-1/2 X 10-3/8 inches). 
    /// </summary>
    PaperEnvelope11 = 0x00000015,
    /// <summary>
    ///Represents Envelope# 12 paper size(4-3/4 X 11 inches). 
    /// </summary>
    PaperEnvelope12 = 0x00000016,
    /// <summary>
    ///Represents Envelope# 14 paper size(5 X 11-1/2 inches). 
    /// </summary>
    PaperEnvelope14 = 0x00000017,
    /// <summary>
    ///Represents Envelope# 9 paper size(3-7/8  X 8-7/8 inches). 
    /// </summary>
    PaperEnvelope9 = 0x00000013,
    /// <summary>
    ///Represents B4 Envelope paper size (250 mm x 353 mm).
    /// </summary>
    PaperEnvelopeB4 = 0x00000021,
    /// <summary>
    ///Represents B5 Envelope paper size (176 mm x 250 mm). 
    /// </summary>
    PaperEnvelopeB5 = 0x00000022,
    /// <summary>
    ///Represents B6 Envelope paper size (176 mm x 125 mm). 
    /// </summary>
    PaperEnvelopeB6 = 0x00000023,
    /// <summary>
    ///Represents C3 Envelope paper size (324 mm x 458 mm). 
    /// </summary>
    PaperEnvelopeC3 = 0x0000001D,
    /// <summary>
    ///Represents C4 Envelope paper size (229 mm x 324 mm). 
    /// </summary>
    PaperEnvelopeC4 = 0x0000001E,
    /// <summary>
    ///Represents C5 Envelope paper size (162 mm x 229 mm).  
    /// </summary>
    PaperEnvelopeC5 = 0x0000001C,
    /// <summary>
    ///Represents C6 Envelope paper size (114 mm x 162 mm). 
    /// </summary>
    PaperEnvelopeC6 = 0x0000001F,
    /// <summary>
    ///Represents C65 Envelope paper size (114 mm x 229 mm). 
    /// </summary>
    PaperEnvelopeC65 = 0x00000020,
    /// <summary>
    ///Represents DL Envelope paper size (110 mm x 220 mm). 
    /// </summary>
    PaperEnvelopeDL = 0x0000001B,
    /// <summary>
    ///Represents Italy Envelope paper size (110 mm x 230 mm). 
    /// </summary>
    PaperEnvelopeItaly = 0x00000024,
    /// <summary>
    ///Represents Monarch Envelope paper size (3-7/8  X 7-1/2 inches). 
    /// </summary>
    PaperEnvelopeMonarch = 0x00000025,
    /// <summary>
    ///Represents Personal Envelope paper size (3-5/8  X 6-1/2 inches). 
    /// </summary>
    PaperEnvelopePersonal = 0x00000026,
    /// <summary>
    ///Represents E paper size.
    /// </summary>
    PaperEsheet = 0x0000001A,
    /// <summary>
    ///Represents Executive paper size (7-1/2  X 10-1/2 inches).
    /// </summary>
    PaperExecutive = 0x00000007,
    /// <summary>
    ///Represents German Fanfold paper size (8-1/2  X 13 inches).
    /// </summary>
    PaperFanfoldLegalGerman = 0x00000029,
    /// <summary>
    ///Represents German Standard Fanfold paper size (8-1/2  X 12 inches).
    /// </summary>
    PaperFanfoldStdGerman = 0x00000028,
    /// <summary>
    ///Represents U.S. Standard Fanfold  paper size (14-7/8  X 11 inches).
    /// </summary>
    PaperFanfoldUS = 0x00000027,
    /// <summary>
    ///Represents Folio paper size (8-1/2  X 13 inches).
    /// </summary>
    PaperFolio = 0x0000000E,
    /// <summary>
    ///Represents Ledger paper size (17  X 11 inches).
    /// </summary>
    PaperLedger = 0x00000004,
    /// <summary>
    ///Represents Legal paper size (8-1/2  X 14 inches).
    /// </summary>
    PaperLegal = 0x00000005,
    /// <summary>
    ///Represents Letter paper size (8-1/2  X 11 inches).
    /// </summary>
    PaperLetter = 0x00000001,
    /// <summary>
    ///Represents Letter Small paper size.
    /// </summary>
    PaperLetterSmall = 0x00000002,
    /// <summary>
    ///Represents Note paper size.
    /// </summary>
    PaperNote = 0x00000012,
    /// <summary>
    ///Represents Quarto paper size(215 mm x 275 mm).
    /// </summary>
    PaperQuarto = 0x0000000F,
    /// <summary>
    ///Represents Statement paper size(5-1/2  X 8-1/2 inches).
    /// </summary>
    PaperStatement = 0x00000006,
    /// <summary>
    ///Represents Tabloid paper size(11 X 17 inches).
    /// </summary>
    PaperTabloid = 0x00000003,
    /// <summary>
    ///Represents User paper size.
    /// </summary>
    PaperUser = 0x00000100,
    /// <summary>
    /// Represents ISO B4 paper size(250 mm by 353 mm).
    /// </summary>
    ISOB4 = 0x0000002A,
    /// <summary>
    /// Represents Japanese double postcard(200 mm by 148 mm).
    /// </summary>
    JapaneseDoublePostcard = 0x0000002B,
    /// <summary>
    /// Represents Standard paper(9 in. by 11 in.).
    /// </summary>
    StandardPaper9By11 = 0x0000002C,
    /// <summary>
    /// Represents Standard paper(10 in. by 11 in.).
    /// </summary>
    StandardPaper10By11 = 0x0000002D,
    /// <summary>
    /// Represents Standard paper(15 in. by 11 in.).
    /// </summary>
    StandardPaper15By11 = 0x0000002E,
    /// <summary>
    /// Represents Invite envelope (220 mm by 220 mm).
    /// </summary>
    InviteEnvelope = 0x0000002F,
    /// <summary>
    /// Represents Letter extra paper (9.275 in. by 12 in.).
    /// </summary>
    LetterExtraPaper9275By12 = 0x00000032,
    /// <summary>
    /// Represents Legal extra paper (9.275 in. by 15 in.).
    /// </summary>
    LegalExtraPaper9275By15 = 0x00000033,
    /// <summary>
    /// Represents Tabloid extra paper (11.69 in. by 18 in.).
    /// </summary>
    TabloidExtraPaper = 0x00000034,
    /// <summary>
    /// Represents A4 extra paper (236 mm by 322 mm).
    /// </summary>
    A4ExtraPaper = 0x00000035,
    /// <summary>
    /// Represents Letter transverse paper (8.275 in. by 11 in.).
    /// </summary>
    LetterTransversePaper = 0x00000036,
    /// <summary>
    /// Represents A4 transverse paper (210 mm by 297 mm).
    /// </summary>
    A4TransversePaper = 0x00000037,
    /// <summary>
    /// Represents Letter extra transverse paper (9.275 in. by 12 in.).
    /// </summary>
    LetterExtraTransversePaper = 0x00000038,
    /// <summary>
    /// Represents SuperA/SuperA/A4 paper (227 mm by 356 mm).
    /// </summary>
    SuperASuperAA4Paper = 0x00000039,
    /// <summary>
    /// Represents SuperB/SuperB/A3 paper (305 mm by 487 mm).
    /// </summary>
    SuperBSuperBA3Paper = 0x0000003A,
    /// <summary>
    /// Represents Letter plus paper (8.5 in. by 12.69 in.).
    /// </summary>
    LetterPlusPaper = 0x0000003B,
    /// <summary>
    /// Represents A4 plus paper (210 mm by 330 mm).
    /// </summary>
    A4PlusPaper = 0x0000003C,
    /// <summary>
    /// Represents A5 transverse paper (148 mm by 210 mm).
    /// </summary>
    A5TransversePaper = 0x0000003D,
    /// <summary>
    /// Represents JIS B5 transverse paper (182 mm by 257 mm).
    /// </summary>
    JISB5TransversePaper = 0x0000003E,
    /// <summary>
    /// Represents A3 extra paper (322 mm by 445 mm).
    /// </summary>
    A3ExtraPaper = 0x0000003F,
    /// <summary>
    /// Represents A5 extra paper (174 mm by 235 mm).
    /// </summary>
    A5ExtraPpaper = 0x00000040,
    /// <summary>
    /// Represents ISO B5 extra paper (201 mm by 276 mm).
    /// </summary>
    ISOB5ExtraPaper = 0x00000041,
    /// <summary>
    /// Represents A2 paper (420 mm by 594 mm).
    /// </summary>
    A2Paper = 0x00000042,
    /// <summary>
    /// Represents A3 transverse paper (297 mm by 420 mm).
    /// </summary>
    A3TransversePaper = 0x00000043,
    /// <summary>
    /// Represents A3 extra transverse paper (322 mm by 445 mm).
    /// </summary>
    A3ExtraTransversePaper = 0x00000044,
  }

  /// <summary>
  ///Enumeration representing print comments in Excel.
  /// </summary>
  public enum ExcelPrintLocation //: uint
  {
    /// <summary>
    ///Represents As displayed on sheet  setting.
    /// </summary>
    PrintInPlace,     // = 0x00000010,
    /// <summary>
    ///Represents (None) setting.
    /// </summary>
    PrintNoComments,  // = 0xFFFFEFD2,
    /// <summary>
    ///Represents at end of sheet setting.
    /// </summary>
    PrintSheetEnd,    // = 0x00000001,
  }

  /// <summary>
  ///Enumeration of Replace Error Values when printing in Excel.
  /// </summary>
  public enum ExcelPrintErrors
  {
    /// <summary>
    ///Represents the blank option.
    /// </summary>
    PrintErrorsBlank = 0x00000001,
    /// <summary>
    ///Represents the dash (--) option.
    /// </summary>
    PrintErrorsDash = 0x00000002,
    /// <summary>
    ///Represents the displayed option.
    /// </summary>
    PrintErrorsDisplayed = 0x00000000,
    /// <summary>
    ///Represents the #N/A option.
    /// </summary>
    PrintErrorsNA = 0x00000003,
  }

  /// <summary>
  ///Enumeration of page break extent types in Excel.
  /// </summary>
  public enum ExcelPageBreakExtent
  {
    /// <summary>
    ///Represents full page break option.
    /// </summary>
    PageBreakFull     = 1,
    /// <summary>
    ///Represents partial page break option.
    /// </summary>
    PageBreakPartial  = 2,
  }

  /// <summary>
  ///Enumeration of page break types in Excel.
  /// </summary>
  public enum ExcelPageBreak //: uint
  {
    /// <summary>
    ///Represents the Automatic option.
    /// </summary>
    PageBreakAutomatic, // = 0xFFFFEFF7,
    /// <summary>
    ///Represents the Manual option.
    /// </summary>
    PageBreakManual,    // = 0xFFFFEFD9,
    /// <summary>
    ///Represents the None option.
    /// </summary>
    PageBreakNone,      // = 0xFFFFEFD2,
  }

  /// <summary>
  ///Enumeration of Save as Access mode types in Excel.
  /// </summary>
  public enum ExcelSaveAsAccessMode
  {
    /// <summary>
    /// Exclusive mode.
    /// </summary>
    Exclusive = 0x00000003,
    /// <summary>
    /// No change mode.
    /// </summary>
    NoChange = 0x00000001,
    /// <summary>
    /// Shared mode.
    /// </summary>
    Shared = 0x00000002,
  }

  /// <summary>
  /// Enumeration of underline types for the fonts available in Excel.
  /// </summary>
  public enum ExcelUnderline
  {
    /// <summary>
    ///Represents no underline.
    /// </summary>
    None              = 0x00000000,
    /// <summary>
    ///Represents single underline.
    /// </summary>
    Single            = 0x00000001,
    /// <summary>
    ///Represents double underline.
    /// </summary>
    Double            = 0x00000002,
    /// <summary>
    ///Represents SingleAccounting underline.
    /// </summary>
    SingleAccounting  = 0x00000021,
    /// <summary>
    ///Represents DoubleAccounting underline.
    /// </summary>
    DoubleAccounting  = 0x00000022
  }
  /// <summary>
  ///Enumeration of merge operation types in Excel.
  /// </summary>
  public enum ExcelMergeOperation
  {
    /// <summary>
    ///Represents the Leave option.
    /// </summary>
    Leave,
    /// <summary>
    ///Represents the Delete option.
    /// </summary>
    Delete
  }
 
  /// <summary>
  /// Enumeration of Worksheet functions in Excel.
  /// </summary>
  [ CLSCompliant( false ) ]
  public enum ExcelFunction //: ushort
  {
    /// <summary>
    /// Represents the NONE function.
    /// </summary>
    NONE              = 0xFFFF,
    /// <summary>
    /// Represents the Custom function.
    /// </summary>
    CustomFunction    = 0xFF,
    /// <summary>
    /// Represents the ABS function.
    /// </summary>
    [ DefaultValue( 1 ),
    ReferenceIndex( 2 ) ]
    ABS               = 24,
    /// <summary>
    /// Represents the ACOS function.
    /// </summary>
    [ DefaultValue( 1 ),
    ReferenceIndex( 2 ) ]
    ACOS              = 99,
    /// <summary>
    /// Represents the ACOSH function.
    /// </summary>
    [ DefaultValue( 1 ),
    ReferenceIndex( 2 ) ]
    ACOSH             = 233,
    /// <summary>
    /// Represents the ADDRESS function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    ADDRESS           = 219,
    /// <summary>
    /// Represents the AND operation.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    AND               = 36,
    /// <summary>
    /// Represents the Areas function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 1 ) ]
    AREAS             = 75,
    /// <summary>
    /// Represents the ASIN function.
    /// </summary>
    [ DefaultValue( 1 ),
    ReferenceIndex( 2 ) ]
    ASIN              = 98,
    /// <summary>
    /// Represents the ASINH function.
    /// </summary>
    [ DefaultValue( 1 ),
    ReferenceIndex( 2 ) ]
    ASINH             = 232,
    /// <summary>
    /// Represents the ATAN function.
    /// </summary>
    [ DefaultValue( 1 ),
    ReferenceIndex( 2 ) ]
    ATAN              = 18,
    /// <summary>
    /// Represents the ATAN2 function.
    /// </summary>
    [ DefaultValue( 2 ),
    ReferenceIndex( 2 ) ]
    ATAN2             = 97,
    /// <summary>
    /// Represents the ATANH function.
    /// </summary>
    [ DefaultValue( 1 ),
    ReferenceIndex( 2 ) ]
    ATANH             = 234,
    /// <summary>
    /// Represents the AVEDEV function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    AVEDEV            = 269,
    /// <summary>
    /// Represents the AVERAGE function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    AVERAGE           = 5,
    /// <summary>
    /// Represents the AVERAGEA function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    AVERAGEA          = 361,
    /// <summary>
        /// Represents the HEX2BIN function.
        /// </summary>        
        HEX2BIN = 400,
        /// <summary>
        /// Represents the HEX2DEC function.
        /// </summary>
        HEX2DEC = 401,
        /// <summary>
        /// Represents the HEX2OCT function.
        /// </summary>
        HEX2OCT = 402,
        /// <summary>
        /// Represents the COUNTIF function.
        /// </summary>
        COUNTIFS = 403,
        /// <summary>
        /// Represents the BIN2DEC function.
        /// </summary>
        BIN2DEC = 404,
        /// <summary>
        /// Represents the BIN2HEX function.
        /// </summary>
        BIN2HEX = 405,
        /// <summary>
        /// Represents the BIN2OCT function.
        /// </summary>
        BIN2OCT = 406,
        /// <summary>
        /// Represents the DEC2BIN function.
        /// </summary>
        DEC2BIN = 407,
        /// <summary>
        /// Represents the DEC2HEX function.
        /// </summary>
        DEC2HEX = 408,
        /// <summary>
        /// Represents the DEC2OCT function.
        /// </summary>
        DEC2OCT = 409,
        /// <summary>
        /// Represents the OCT2BIN function.
        /// </summary>
        OCT2BIN = 410,
        /// <summary>
        /// Represents the OCT2DEC function.
        /// </summary>
        OCT2DEC = 411,
        /// <summary>
        /// Represents the OCT2HEX function.
        /// </summary>
        OCT2HEX = 412,
        /// <summary>
        /// Represents the ODDFPRICE function.
        /// </summary>
        ODDFPRICE = 413,
        /// <summary>
        /// Represents the ODDFYEILD function.
        /// </summary>
        ODDFYIELD = 414,
        /// <summary>
        /// Represents the ODDLPRICE function.
        /// </summary>
        ODDLPRICE = 415,
        /// <summary>
        /// Represents the ODDLYEILD function.
        /// </summary>
        ODDLYIELD = 416,
        /// <summary>
        /// Represents the ISODD function.
        /// </summary>
        ISODD = 417,
        /// <summary>
        /// Represents the ISEVEN function.
        /// </summary>
        ISEVEN = 418,
        /// <summary>
        /// Represents the LCM function.
        /// </summary>
        LCM = 419,
        /// <summary>
        /// Represents the GCD function.
        /// </summary>
        GCD = 420,
        /// <summary>
        /// Represents the SUMIFS function.
        /// </summary>
        SUMIFS = 421,
        /// <summary>
        /// Represents the AVERAGEIF function.
        /// </summary>
        AVERAGEIF = 422,
        /// <summary>
        /// Represents the AVERAGEIFS function.
        /// </summary>
        AVERAGEIFS = 423,
        /// <summary>
        /// Represents the CONVERT function.
        /// </summary>
        CONVERT = 424,
        /// <summary>
        /// Represents the COMPLEX function.
        /// </summary>
        COMPLEX = 425,
        /// <summary>
        /// Represents the COUPDAYBS function.
        /// </summary>
        COUPDAYBS = 426,
        /// <summary>
        /// Represents the COUPDAYS function.
        /// </summary>
        COUPDAYS = 427,
        /// <summary>
        /// Represents the COUPDAYSNC function.
        /// </summary>
        COUPDAYSNC = 428,
        /// <summary>
        /// Represents the COUPNCD function.
        /// </summary>
        COUPNCD = 429,
        /// <summary>
        /// Represents the COUPNUM function.
        /// </summary>
        COUPNUM = 430,
        /// <summary>
        /// Represents the COUPPCD function.
        /// </summary>
        COUPPCD = 431,
        /// <summary>
        /// Represents the DELTA function.
        /// </summary>
        DELTA = 432,
        /// <summary>
        /// Represents the DISC function.
        /// </summary>
        DISC = 433,
        /// <summary>
        /// Represents the DOLLARDE function.
        /// </summary>
        DOLLARDE = 434,
        /// <summary>
        /// Represents the DOLLARFR function.
        /// </summary>
        DOLLARFR = 435,
        /// <summary>
        /// Represents the DURATION function.
        /// </summary>
        DURATION = 436,
        /// <summary>
        /// Represents the EDATE function.
        /// </summary>
        EDATE = 437,
        /// <summary>
        /// Represents the EFFECT function.
        /// </summary>
        EFFECT = 438,
        /// <summary>
        /// Represents the EOMONTH function.
        /// </summary>
        EOMONTH = 439,
        /// <summary>
        /// Represents the ERF function.
        /// </summary>
        ERF = 440,
        /// <summary>
        /// Represents the ERFC function.
        /// </summary>
        ERFC = 441,
        /// <summary>
        /// Represents the FACTDOUBLE function.
        /// </summary>
        FACTDOUBLE = 442,
        /// <summary>
        /// Represents teh GESTEP function.
        /// </summary>
        GESTEP = 443,
        /// <summary>
        /// Represents the IFERROR function.
        /// </summary>
        IFERROR = 444,
        /// <summary>
        /// Represents the IMABS function
        /// </summary>
        IMABS = 445,
        /// <summary>
        /// Represents the IMAGINARY function.
        /// </summary>
        IMAGINARY = 446,
        /// <summary>
        /// Represents the IMARGUMENT function.
        /// </summary>
        IMARGUMENT = 447,
        /// <summary>
        /// Represents the IMCONJUGATE function.
        /// </summary>
        IMCONJUGATE = 448,
        /// <summary>
        /// Represents the IMCOS function.
        /// </summary>
        IMCOS = 449,
        /// <summary>
        /// Represents the IMEXP function.
        /// </summary>
        IMEXP = 450,
        /// <summary>
        /// Represents the IMLN function.
        /// </summary>
        IMLN = 451,
        /// <summary>
        /// Represents the IMLOG10 function.
        /// </summary>
        IMLOG10 = 452,
        /// <summary>
        /// Represents the IMLOG2 function.
        /// </summary>
        IMLOG2 = 453,
        /// <summary>
        /// Represents the IMREAL function.
        /// </summary>
        IMREAL = 454,
        /// <summary>
        /// Represents the IMSIN function.
        /// </summary>
        IMSIN = 455,
        /// <summary>
        /// Represents the IMSQRT function.
        /// </summary>
        IMSQRT = 456,
        /// <summary>
        ///Represents the IMSUB function. 
        /// </summary>
        IMSUB = 457,
        /// <summary>
        /// Represents the IMSUM function.
        /// </summary>        
        IMSUM = 458,
        /// <summary>
        /// Represents the IMDIV function.
        /// </summary>
        IMDIV = 459,
        /// <summary>
        /// Represents the IMPOWER function.
        /// </summary>
        IMPOWER = 460,
        /// <summary>
        /// Represents the IMPRODUCT function.
        /// </summary>
        IMPRODUCT = 461,
        /// <summary>
        /// Represents the ACCRINT function.
        /// </summary>
        ACCRINT = 462,
        /// <summary>
        /// Represents the ACCRINTM function.
        /// </summary>
        ACCRINTM = 463,
        /// <summary>
        /// Represents the AGGREGATE function.
        /// </summary>
        AGGREGATE = 464,
        /// <summary>
        /// Represents the AMORDEGRC function.
        /// </summary>
        AMORDEGRC = 465,
        /// <summary>
        /// Represents the AMORLINC function.
        /// </summary>
        AMORLINC = 466,
        /// <summary>
        /// Represents the BAHTTEXT function.
        /// </summary>
        BAHTTEXT = 467,
        /// <summary>
        /// Represents the BESSELI function.
        /// </summary>
        BESSELI = 468,
        /// <summary>
        /// Represents the BESSELJ function.
        /// </summary>
        BESSELJ = 469,
        /// <summary>
        /// Represents the BESSELK function.
        /// </summary>
        BESSELK = 470,
        /// <summary>
        /// Represents the BESSELY function.
        /// </summary>
        BESSELY = 471,
        /// <summary>
        /// Represents the CUBEKPIMEMBER function.
        /// </summary>
        CUBEKPIMEMBER=472,
        /// <summary>
        /// Represents the CUBEMEMBER function.
        /// </summary>
        CUBEMEMBER=473,
        /// <summary>
        /// Represents the CUBERANKEDMEMBER function.
        /// </summary>
        CUBERANKEDMEMBER=474,
        /// <summary>
        /// Represents the CUBESET function.
        /// </summary>
        CUBESET=475,
        /// <summary>
        /// Represents the CUBESETCOUNT function.
        /// </summary>
        CUBESETCOUNT=476,
        /// <summary>
        /// Represents the CUBEMEMBERPROPERTY function.
        /// </summary>
        CUBEMEMBERPROPERTY=477,
        /// <summary>
        /// Represents the CUMIPMT function.
        /// </summary>
        CUMIPMT=478,
        /// <summary>
        /// Represents the CUMPRINC function.
        /// </summary>
        CUMPRINC=479,
        /// <summary>
        /// Represents the FVSCHEDULE function.
        /// </summary>
        FVSCHEDULE=480,
        /// <summary>
        /// Represents the INTRATE function.
        /// </summary>
        INTRATE=481,
        /// <summary>
        /// Represents the LINTEST function.
        /// </summary>
        LINTEST=482,
        /// <summary>
        /// Represents the CUBEVALUE function.
        /// </summary>
        CUBEVALUE=483,
        /// <summary>
        /// Represents the MDURATION function.
        /// </summary>
        MDURATION=484,
        /// <summary>
        /// Represents the MROUND function.
        /// </summary>
        MROUND=485,
        /// <summary>
        /// Represents the MULTINOMIAL function.
        /// </summary>
        MULTINOMIAL=486,
        /// <summary>
        /// Represents the NETWORKDAYS function.
        /// </summary>
        NETWORKDAYS=487,
        /// <summary>
        /// Represents the NOMINAL function.
        /// </summary>
        NOMINAL=488,
        /// <summary>
        /// Represents the PRICE function.
        /// </summary>
        PRICE=489,
        /// <summary>
        /// Represents the PRICEDISC function.
        /// </summary>
        PRICEDISC=490,
        /// <summary>
        /// Represents the PRICEMAT function.
        /// </summary>
        PRICEMAT=491,
        /// <summary>
        /// Represents the QUOTIENT function.
        /// </summary>
        QUOTIENT=492,
        /// <summary>
        /// Represents the RANDBETWEEN function.
        /// </summary>
        RANDBETWEEN=493,
        /// <summary>
        /// Represents the RECEIVED function.
        /// </summary>
        RECEIVED=494,        
        /// <summary>
        /// Represents the SERIESSUM function.
        /// </summary>
        SERIESSUM=495,
        /// <summary>
        /// Represents the SQRTPI function.
        /// </summary>
        SQRTPI=496,
        /// <summary>
        /// Represents the TBILLEQ function.
        /// </summary>
        TBILLEQ=497,
        /// <summary>
        /// Represents the TBILLPRICE function.
        /// </summary>
        TBILLPRICE=498,
        /// <summary>
        /// Represents the TBILLYIELD function.
        /// </summary>
        TBILLYIELD=499,
        /// <summary>
        /// Represents the WEEKNUM function.
        /// </summary>
        WEEKNUM=500,
        /// <summary>
        /// Represents the WORKDAY function.
        /// </summary>
        WORKDAY=501,
        /// <summary>
        /// Represents the XIRR function.
        /// </summary>
        XIRR=502,
        /// <summary>
        /// Represents the XNPV function.
        /// </summary>
        XNPV=503,
        /// <summary>
        /// Represents the YEAR function.
        /// </summary>
        YEARFRAC = 504,
        /// <summary>
        /// Represents the YIELD function.
        /// </summary>
        YIELD=505,
        /// <summary>
        /// Represents the YIELDDISC function.
        /// </summary>
        YIELDDISC=506,
        /// <summary>
        /// Represents the YIELDMAT function.
        /// </summary>
        YIELDMAT=507,
        /// <summary>
        /// Represents the WORKDAY.INTL function.
        /// </summary>
        WORKDAYINTL=508,
        /// <summary>
        /// Represents the BETA.INV function.
        /// </summary>
        BETA_INV=509,
        /// <summary>
        /// Represents the BINOM.DIST function.
        /// </summary>
        BINOM_DIST=510,
        /// <summary>
        /// Represents the BINOM.INV function.
        /// </summary>
        BINOM_INV=511,
        /// <summary>
        /// Represents the CEILING.PRECISE function.
        /// </summary>
        CEILING_PRECISE=512,
        /// <summary>
        /// Represents the CHISQ.DIST function.
        /// </summary>
        CHISQ_DIST=513,
        /// <summary>
        /// Represents the CHISQ.DIST.RT function.
        /// </summary>
        CHISQ_DIST_RT=514,
        /// <summary>
        /// Represents the CHISQ.INV function.
        /// </summary>
        CHISQ_INV=515,
        /// <summary>
        /// Represents the CHISQ.INV.RT function.
        /// </summary>
        CHISQ_INV_RT=516,
        /// <summary>
        /// Represents the CHISQ.TEST function.
        /// </summary>
        CHISQ_TEST=517,
        /// <summary>
        /// Represents the CONFIDENCE.NORM function.
        /// </summary>
        CONFIDENCE_NORM=518,
        /// <summary>
        /// Represents the CONFIDENCE.T function.
        /// </summary>
        CONFIDENCE_T=519,
        /// <summary>
        /// Represents the COVARIANCE.P function.
        /// </summary>
        COVARIANCE_P=520,
        /// <summary>
        /// Represents the COVARIANCE.S function.
        /// </summary>
        COVARIANCE_S = 521,
        /// <summary>
        /// Represents the ERF.PRECISE function.
        /// </summary>
        ERF_PRECISE=522,
        /// <summary>
        /// Represents the ERFC.PRECISE function.
        /// </summary>      
        ERFC_PRECISE=523,
        /// <summary>
        /// Represents the F.DIST function.
        /// </summary>
        F_DIST=524,
        /// <summary>
        /// Represents the F.DIST.RT function.
        /// </summary>
        F_DIST_RT=525,
        /// <summary>
        /// Represents the F.INV function.
        /// </summary>
        F_INV=526,
        /// <summary>
        /// Represents the F.INV.RT function.
        /// </summary>
        F_INV_RT=527,
        /// <summary>
        /// Represents the F.TEST function.
        /// </summary>
        F_TEST=528,
        /// <summary>
        /// Represents the FLOOR.PRECISE function.
        /// </summary>
        FLOOR_PRECISE=529,
        /// <summary>
        /// Represents the GAMMA.DIST function.
        /// </summary>
        GAMMA_DIST=530,
        /// <summary>
        /// Represents the GAMMA.INV function.
        /// </summary>
        GAMMA_INV=531,
        /// <summary>
        /// Represents the GAMMALN.PRECISE function.
        /// </summary>
        GAMMALN_PRECISE=532,
        /// <summary>
        /// Represents the HYPGEOM.DIST function.
        /// </summary>
        HYPGEOM_DIST=533,
        /// <summary>
        /// Represents the LOGNORM.DIST function.
        /// </summary>
        LOGNORM_DIST=534,
        /// <summary>
        /// Represents the LOGNORM.INV function.
        /// </summary>
        LOGNORM_INV=535,
        /// <summary>
        /// Represents the MODE.MULT function.
        /// </summary>
        MODE_MULT=536,
        /// <summary>
        /// Represents the MODE.SNGL function.
        /// </summary>
        MODE_SNGL=537,
        /// <summary>
        /// Represents the NEGBINOM.DIST function.
        /// </summary>
        NEGBINOM_DIST=538,
        /// <summary>
        /// Represents the NETWORKDAYS.INTL function.
        /// </summary>
        NETWORKDAYS_INTL=539,
        /// <summary>
        /// Represents the NORM.DIST function.
        /// </summary>
        NORM_DIST=540,
        /// <summary>
        /// Represents the NORM.INV function.
        /// </summary>
        NORM_INV=541,
        /// <summary>
        /// Represents the NORM.S.DIST function.
        /// </summary>
        NORM_S_DIST=542,
        /// <summary>
        /// Represents the PERCENTILE.EXC function.
        /// </summary>
        PERCENTILE_EXC=543,
        /// <summary>
        /// Represents the PERCENTILE.INC function.
        /// </summary>
        PERCENTILE_INC=544,
        /// <summary>
        /// Represents the PERCENTRANK.EXC function.
        /// </summary>
        PERCENTRANK_EXC=545,
        /// <summary>
        /// Represents the PRECENTRANK.INC function.
        /// </summary>
        PERCENTRANK_INC=546,
        /// <summary>
        /// Represents the POISSON.DIST function.
        /// </summary>
        POISSON_DIST=547,
        /// <summary>
        /// Represents the QUARTILE.EXC function.
        /// </summary>
        QUARTILE_EXC=548,
        /// <summary>
        /// Represents the QUARTILE.INC function.
        /// </summary>
        QUARTILE_INC=549,
        /// <summary>
        /// Represents the RANK.AVG function.
        /// </summary>
        RANK_AVG=550,
        /// <summary>
        /// Represents the RANK.EQ function.
        /// </summary>
        RANK_EQ=551,
        /// <summary>
        /// Represents the STDEV.P function. 
        /// </summary>
        STDEV_P=552,
        /// <summary>
        /// Represents the STDEV.S function.
        /// </summary>
        STDEV_S=553,
        /// <summary>
        /// Represents the T.DIST function.
        /// </summary>
        T_DIST=554,
        /// <summary>
        /// Represents the T.DIST.2T function.
        /// </summary>
        T_DIST_2T=555,
        /// <summary>
        /// Represents the T.DIST.RT function.
        /// </summary>
        T_DIST_RT=556,
        /// <summary>
        /// Represents the T.INV function.
        /// </summary>
        T_INV=557,
        /// <summary>
        /// Represents the T.INV.2T function.
        /// </summary>
        T_INV_2T=558,
        /// <summary>
        /// Represents the T.TEST function.
        /// </summary>
        T_TEST=559,
        /// <summary>
        /// Represents the VAR.P function.
        /// </summary>
        VAR_P=560,
        /// <summary>
        /// Represents the VAR.S function.
        /// </summary>
        VAR_S=561,
        /// <summary>
        /// Represents the WEIBULL.DIST function.
        /// </summary>
        WEIBULL_DIST=562,
        /// <summary>
        /// Represents the WORKDAY.INTL function.
        /// </summary>
        WORKDAY_INTL=563,
        /// <summary>
        /// Represents the Z.TEST function.
        /// </summary>
        Z_TEST=564,
        /// <summary>
        /// Represents the BETA.DIST function.
        /// </summary>
        BETA_DIST=565,
        /// <summary>
        /// Represents the EUROCONVERT function.
        /// </summary>
        EUROCONVERT=566,
        /// <summary>
        /// Represents the PHONETIC function.
        /// </summary>
        PHONETIC=567,
        /// <summary>
        /// Represents the REGISTER.ID function.
        /// </summary>
        REGISTER_ID=568,
        /// <summary>
        /// Represents the SQL.REQUEST function.
        /// </summary>
        SQL_REQUEST=569,
        /// <summary>
        /// Represents the JIS function.
        /// </summary>
        JIS=570,
        /// <summary>
        /// Represents the EXPON.DIST function.
        /// </summary>
        EXPON_DIST=571,
        /// <summary>
        /// Represents the DAYS function.
        /// </summary>
        DAYS = 572,
        /// <summary>
        /// Represents the ISOWEEKNUM function.
        /// </summary>
        ISOWEEKNUM = 573,
        /// <summary>
        /// Represents the BITAND function.
        /// </summary>
        BITAND = 574,
        /// <summary>
        /// Represents the BITLSHIFT function.
        /// </summary>
        BITLSHIFT = 575,
        /// <summary>
        /// Represents the BITOR function.
        /// </summary>
        BITOR = 576,
        // <summary>
        /// Represents the BITXOR function.
        /// </summary>
        BITRSHIFT = 577,
        /// <summary>
        /// Represents the BITXOR function.
        /// </summary>
        BITXOR = 578,
        /// <summary>
        /// Represents the IMCOSH function.
        /// </summary>
        IMCOSH = 579,
        /// <summary>
        /// Represents the IMCOT function.
        /// </summary>
        IMCOT = 580,
        /// <summary>
        /// Represents the IMCSC function.
        /// </summary>
        IMCSC = 581,
        /// <summary>
        /// Represents the IMCSCH function.
        /// </summary>
        IMCSCH = 582,
        /// <summary>
        /// Represents the IMSEC function.
        /// </summary>
        IMSEC = 583,
        /// <summary>
        /// Represents the IMSECH function.
        /// </summary>
        IMSECH = 584,
        /// <summary>
        /// Represents the IMSINH function.
        /// </summary>
        IMSINH = 585,
        /// <summary>
        /// Represents the IMTAN function.
        /// </summary>
        IMTAN = 586,
        /// <summary>
        /// Represents the PDURATION function.
        /// </summary>
        PDURATION = 587,
        /// <summary>
        /// Represents the RRI function.
        /// </summary>
        RRI = 588,
        /// <summary>
        /// Represents the ISFORMULA function.
        /// </summary>
        ISFORMULA = 589,
        /// <summary>
        /// Represents the SHEET function.
        /// </summary>
        SHEET = 590,
        /// <summary>
        /// Represents the SHEETS function.
        /// </summary>
        SHEETS = 591,
        /// <summary>
        /// Represents the IFNA function.
        /// </summary>
        IFNA = 592,
        /// <summary>
        /// Represents the XOR function.
        /// </summary>
        XOR = 593,
        /// <summary>
        /// Represents the FORMULATEXT function.
        /// </summary>
        FORMULATEXT = 594,
        /// <summary>
        /// Represents the ACOT function.
        /// </summary>
        ACOT = 595,
        /// <summary>
        /// Represents the ACOTH function.
        /// </summary>
        ACOTH = 596,
        /// <summary>
        /// Represents the ARABIC function.
        /// </summary>
        ARABIC = 597,
        /// <summary>
        /// Represents the BASE function.
        /// </summary>
        BASE = 598,
        /// <summary>
        /// Represents the CEILING.MATH function.
        /// </summary>
        CEILING_MATH = 599,
        /// <summary>
        /// Represents the COMBINA function.
        /// </summary>
        COMBINA = 600,
        /// <summary>
        /// Represents the COT function.
        /// </summary>
        COT = 601,
        /// <summary>
        /// Represents the COTH function.
        /// </summary>
        COTH = 602,
        /// <summary>
        /// Represents the CSC function.
        /// </summary>
        CSC = 603,
        /// <summary>
        /// Represents the CSCH function.
        /// </summary>
        CSCH = 604,
        /// <summary>
        /// Represents the DECIMAL function.
        /// </summary>
        DECIMAL = 605,
        /// <summary>
        /// Represents the FLOOR.MATH function.
        /// </summary>
        FLOOR_MATH = 606,
        /// <summary>
        /// Represents the ISO.CEILING function.
        /// </summary>
        ISO_CEILING = 607,
        /// <summary>
        /// Represents the MUNIT function.
        /// </summary>
        MUNIT = 608,
        /// <summary>
        /// Represents the SEC function.
        /// </summary>
        SEC = 609,
        /// <summary>
        /// Represents the SECH function.
        /// </summary>
        SECH = 610,
        /// <summary>
        /// Represents the BINOM.DIST.RANGE function.
        /// </summary>
        BINOM_DIST_RANGE = 611,
        /// <summary>
        /// Represents the GAMMA function.
        /// </summary>
        GAMMA = 612,
        /// <summary>
        /// Represents the GAUSS function.
        /// </summary>
        GAUSS = 613,
        /// <summary>
        /// Represents the PERMUTATIONA function.
        /// </summary>
        PERMUTATIONA = 614,
        /// <summary>
        /// Represents the PHI function.
        /// </summary>
        PHI = 615,
        /// <summary>
        /// Represents the SKEW.P function.
        /// </summary>
        SKEW_P = 616,
        /// <summary>
        /// Represents the NUMBERVALUE function.
        /// </summary>
        NUMBERVALUE = 617,
        /// <summary>
        /// Represents the UNICHAR function.
        /// </summary>
        UNICHAR = 618,
        /// <summary>
        /// Represents the UNICODE function.
        /// </summary>
        UNICODE = 619,
        /// <summary>
        /// Represents the ENCODEURL function.
        /// </summary>
        ENCODEURL = 620,
        /// <summary>
        /// Represents the FILTERXML function.
        /// </summary>
        FILTERXML = 621,
        /// <summary>
        /// Represents the WEBSERVICE function.
        /// </summary>
        WEBSERVICE = 622,
        /// <summary>
    /// Represents the BETADIST function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    BETADIST          = 270,
    /// <summary>
    /// Represents the BETAINV function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    BETAINV           = 272,
    /// <summary>
    /// Represents the BINOMDIST function.
    /// </summary>
    [ DefaultValue( 4 ),
    ReferenceIndex( 2 ) ]
    BINOMDIST         = 273,
    /// <summary>
    /// Represents the CEILING function.
    /// </summary>
    [ DefaultValue( 2 ),
    ReferenceIndex( 2 ) ]
    CEILING           = 288,
    /// <summary>
    /// Represents the CELL function.
    /// </summary>
    [ ReferenceIndex( 2, 1 ) ]
    CELL              = 125,
    /// <summary>
    /// Represents the CHAR function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    CHAR              = 111,
    /// <summary>
    /// Represents the CHIDIST function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    CHIDIST           = 274,
    /// <summary>
    /// Represents the CHIINV function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    CHIINV            = 275,
    /// <summary>
    /// Represents the CHITEST function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    CHITEST           = 306,
    /// <summary>
    /// Represents the CHOOSE function.
    /// </summary>
    CHOOSE            = 100,
    /// <summary>
    /// Represents the CLEAN function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    CLEAN             = 162,
    /// <summary>
    /// Represents the CODE function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    CODE              = 121,
    /// <summary>
    /// Represents the COLUMN function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    COLUMN            = 9,
    /// <summary>
    /// Represents the COLUMNS function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 1 ) ]
    COLUMNS           = 77,
    /// <summary>
    /// Represents the COMBIN function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    COMBIN            = 276,
    /// <summary>
    /// Represents the CONCATENATE function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    CONCATENATE       = 336,
    /// <summary>
    /// Represents the CONFIDENCE function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    CONFIDENCE        = 277,
    /// <summary>
    /// Represents the CORREL function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    CORREL            = 307,
    /// <summary>
    /// Represents the COS function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    COS               = 16,
    /// <summary>
    /// Represents the COSH function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    COSH              = 230,
    /// <summary>
    /// Represents the COUNT function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    COUNT             = 0,
    /// <summary>
    /// Represents the COUNTA function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    COUNTA            = 169,
    /// <summary>
    /// Represents the COUNTBLANK function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 1 ) ]
    COUNTBLANK        = 347,
    /// <summary>
    /// Represents the COUNTIF function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 1, 2 ) ]
    COUNTIF           = 346,
    /// <summary>
    /// Represents the COVAR function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    COVAR             = 308,
    /// <summary>
    /// Represents the CRITBINOM function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    CRITBINOM         = 278,
    /// <summary>
    /// Represents the DATE function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    DATE              = 65,
    /// <summary>
    /// Represents the DATEVALUE function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    DATEVALUE         = 140,
    /// <summary>
    /// Represents the DAVERAGE function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DAVERAGE          = 42,
    /// <summary>
    /// Represents the DAY function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    DAY               = 67,
    /// <summary>
    /// Represents the DAYS360 function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    DAYS360           = 220,
    /// <summary>
    /// Represents the DB function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    DB                = 247,
    /// <summary>
    /// Represents the DCOUNT function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DCOUNT            = 40,
    /// <summary>
    /// Represents the DCOUNTA function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DCOUNTA           = 199,
    /// <summary>
    /// Represents the DDB function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    DDB               = 144,
    /// <summary>
    /// Represents the DEGREES function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    DEGREES           = 343,
    /// <summary>
    /// Represents the DEVSQ function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    DEVSQ             = 318,
    /// <summary>
    /// Represents the DMAX function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DMAX              = 44,
    /// <summary>
    /// Represents the DMIN function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ DefaultValue( 3 ) ]
    DMIN              = 43,
    /// <summary>
    /// Represents the DOLLAR function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    DOLLAR            = 13,
    /// <summary>
    /// Represents the DPRODUCT function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DPRODUCT          = 189,
    /// <summary>
    /// Represents the DSTDEV function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DSTDEV            = 45,
    /// <summary>
    /// Represents the DSTDEVP function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DSTDEVP           = 195,
    /// <summary>
    /// Represents the DSUM function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DSUM              = 41,
    /// <summary>
    /// Represents the DVAR function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DVAR              = 47,
    /// <summary>
    /// Represents the DVARP function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DVARP             = 196,
    /// <summary>
    /// Represents the ERROR function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    ERROR             = 84,
    /// <summary>
    /// Represents the ERRORTYPE function.
    /// </summary>
#if !(WINRT )
    [ Description( "ERROR.TYPE" ) ]
#endif
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    ERRORTYPE         = 261,
    /// <summary>
    /// Represents the EVEN function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    EVEN              = 279,
    /// <summary>
    /// Represents the EXACT function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    EXACT             = 117,
    /// <summary>
    /// Represents the EXP function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    EXP               = 21,
    /// <summary>
    /// Represents the EXPONDIST function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    EXPONDIST         = 280,
    /// <summary>
    /// Represents the FACT function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    FACT              = 184,
    /// <summary>
    /// Represents the FALSE function.
    /// </summary>
    [ DefaultValue( 0 ) ]
    [ ReferenceIndex( 1 ) ]
    FALSE             = 35,
    /// <summary>
    /// Represents the FDIST function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    FDIST             = 281,
    /// <summary>
    /// Represents the FIND function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    FIND              = 124,
    /// <summary>
    /// Represents the FINDB function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    FINDB             = 205,
    /// <summary>
    /// Represents the FINV function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    FINV              = 282,
    /// <summary>
    /// Represents the FISHER function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    FISHER            = 283,
    /// <summary>
    /// Represents the FISHERINV function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    FISHERINV         = 284,
    /// <summary>
    /// Represents the FIXED function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    FIXED             = 14,
    /// <summary>
    /// Represents the FLOOR function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    FLOOR             = 285,
    /// <summary>
    /// Represents the FORECAST function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2, 3, 3 ) ]
    FORECAST          = 309,
    /// <summary>
    /// Represents the FREQUENCY function.
    /// </summary>
    [ DefaultValue( 2 ) ]
      //    [ ReferenceIndex( typeof( ArrayPtg ), 3 ) ]
    [ ReferenceIndex( 1 ) ]
    FREQUENCY         = 252,
    /// <summary>
    /// Represents the FTEST function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
      //    [ ReferenceIndex( typeof( ArrayPtg ), 3 ) ]
    FTEST             = 310,
    /// <summary>
    /// Represents the FV function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    FV                = 57,
    /// <summary>
    /// Represents the GAMMADIST function.
    /// </summary>
    [ DefaultValue( 4 ) ]
    [ ReferenceIndex( 2 ) ]
    GAMMADIST         = 286,
    /// <summary>
    /// Represents the GAMMAINV function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    GAMMAINV          = 287,
    /// <summary>
    /// Represents the GAMMALN function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    GAMMALN           = 271,
    /// <summary>
    /// Represents the GEOMEAN function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    GEOMEAN           = 319,
    /// <summary>
    /// Represents the GETPIVOTDATA function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    GETPIVOTDATA      = 358,
    /// <summary>
    /// Represents the GROWTH function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    GROWTH            = 52,
    /// <summary>
    /// Represents the HARMEAN function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    HARMEAN           = 320,
    /// <summary>
    /// Represents the HLOOKUP function.
    /// </summary>
    [ ReferenceIndex( 2, 1 ) ]
    HLOOKUP           = 101,
    /// <summary>
    /// Represents the HOUR function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    HOUR              = 71,
    /// <summary>
    /// Represents the HYPERLINK function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    HYPERLINK         = 359,
    /// <summary>
    /// Represents the HYPGEOMDIST function.
    /// </summary>
    [ DefaultValue( 4 ) ]
    [ ReferenceIndex( 2 ) ]
    HYPGEOMDIST       = 289,
    /// <summary>
    /// Represents the IF function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    IF                = 1,
    /// <summary>
    /// Represents the INDEX function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ), 3 ) ]
    INDEX             = 29,
    /// <summary>
    /// Represents the INDIRECT function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    INDIRECT          = 148,
    /// <summary>
    /// Represents the INFO function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    INFO              = 244,
    /// <summary>
    /// Represents the INT function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    INT               = 25,
    /// <summary>
    /// Represents the INTERCEPT function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    INTERCEPT         = 311,
    /// <summary>
    /// Represents the IPMT function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    IPMT              = 167,
    /// <summary>
    /// Represents the IRR function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    IRR               = 62,
    /// <summary>
    /// Represents the ISBLANK function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    ISBLANK           = 129,
    /// <summary>
    /// Represents the ISERR function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    ISERR             = 126,
    /// <summary>
    /// Represents the ISERROR function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    ISERROR           = 3,
    /// <summary>
    /// Represents the ISLOGICAL function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    ISLOGICAL         = 198,
    /// <summary>
    /// Represents the ISNA function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    ISNA              = 2,
    /// <summary>
    /// Represents the ISNONTEXT function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    ISNONTEXT         = 190,
    /// <summary>
    /// Represents the ISNUMBER function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    ISNUMBER          = 128,
    /// <summary>
    /// Represents the ISPMT function.
    /// </summary>
    [ DefaultValue( 4 ) ]
    [ ReferenceIndex( 2 ) ]
    ISPMT             = 350,
    /// <summary>
    /// Represents the ISREF function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 1 ) ]
    ISREF             = 105,
    /// <summary>
    /// Represents the ISTEXT function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    ISTEXT            = 127,
    /// <summary>
    /// Represents the KURT function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    KURT              = 322,
    /// <summary>
    /// Represents the LARGE function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    LARGE             = 325,
    /// <summary>
    /// Represents the LEFT function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    LEFT              = 115,
    /// <summary>
    /// Represents the LEFTB function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    LEFTB             = 208,
    /// <summary>
    /// Represents the LEN function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    LEN               = 32,
    /// <summary>
    /// LENB function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    LENB              = 211,
    /// <summary>
    /// Represents the LINEST function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    LINEST            = 49,
    /// <summary>
    /// Represents the LN function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    LN                = 22,
    /// <summary>
    /// Represents the LOG function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    LOG               = 109,
    /// <summary>
    /// Represents the LOG10 function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    LOG10             = 23,
    /// <summary>
    /// Represents the LOGEST function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    LOGEST            = 51,
    /// <summary>
    /// Represents the LOGINV function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    LOGINV            = 291,
    /// <summary>
    /// Represents the LOGNORMDIST function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    LOGNORMDIST       = 290,
    /// <summary>
    /// Represents the LOOKUP function.
    /// </summary>
    [ ReferenceIndex( 2, 1, 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    LOOKUP            = 28,
    /// <summary>
    /// Represents the LOWER function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    LOWER             = 112,
    /// <summary>
    /// Represents the MATCH function.
    /// </summary>
    [ ReferenceIndex( 2, 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ), 3 ) ]
    MATCH             = 64,
    /// <summary>
    /// Represents the MAX function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ), 3 ) ]
    MAX               = 7,
    /// <summary>
    /// Represents the MAXA function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ), 3 ) ]
    MAXA              = 362,
    /// <summary>
    /// Represents the MDETERM function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 3 ) ]
    MDETERM           = 163,
    /// <summary>
    /// Represents the MEDIAN function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    MEDIAN            = 227,
    /// <summary>
    /// Represents the MID function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    MID               = 31,
    /// <summary>
    /// Represents the MIDB function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    MIDB              = 210,
    /// <summary>
    /// Represents the MIN function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ), 3 ) ]
    MIN               = 6,
    /// <summary>
    /// Represents the MINA function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ), 3 ) ]
    MINA              = 363,
    /// <summary>
    /// Represents the MINUTE function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    MINUTE            = 72,
    /// <summary>
    /// Represents the MINVERSE function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 3 ) ]
    MINVERSE          = 164,
    /// <summary>
    /// Represents the MIRR function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1, 2, 2 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    MIRR              = 61,
    /// <summary>
    /// Represents the MMULT function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    MMULT             = 165,
    /// <summary>
    /// Represents the MOD function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    MOD               = 39,
    /// <summary>
    /// Represents the MODE function.
    /// </summary>
    [ ReferenceIndex( 3 ) ]
    MODE              = 330,
    /// <summary>
    /// Represents the MONTH function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    MONTH             = 68,
    /// <summary>
    /// Represents the N function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 1 ) ]
    N                 = 131,
    /// <summary>
    /// Represents the NA function.
    /// </summary>
    [ DefaultValue( 0 ) ]
    [ ReferenceIndex( 1 ) ]
    NA                = 10,
    /// <summary>
    /// Represents the NEGBINOMDIST function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    NEGBINOMDIST      = 292,
    /// <summary>
    /// Represents the NORMDIST function.
    /// </summary>
    [ DefaultValue( 4 ) ]
    [ ReferenceIndex( 2 ) ]
    NORMDIST          = 293,
    /// <summary>
    /// Represents the NORMINV function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    NORMINV           = 295,
    /// <summary>
    /// Represents the NORMSDIST function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    NORMSDIST         = 294,
    /// <summary>
    /// Represents the NORMSINV function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    NORMSINV          = 296,
    /// <summary>
    /// Represents the NOT function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    NOT               = 38,
    /// <summary>
    /// Represents the NOW function.
    /// </summary>
    [ DefaultValue( 0 ) ]
    [ ReferenceIndex( 1 ) ]
    NOW               = 74,
    /// <summary>
    /// Represents the NPER function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    NPER              = 58,
    /// <summary>
    /// Represents the NPV function.
    /// </summary>
    [ ReferenceIndex( typeof( AreaPtg ), 2, 1 ) ]
    [ ReferenceIndex( 2 ) ]
    NPV               = 11,
    /// <summary>
    /// Represents the ODD function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    ODD               = 298,
    /// <summary>
    /// Represents the OFFSET function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    OFFSET            = 78,
    /// <summary>
    /// Represents the OR function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    OR                = 37,
    /// <summary>
    /// Represents the PEARSON function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    PEARSON           = 312,
    /// <summary>
    /// Represents the PERCENTILE function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 1, 2 ) ]
    PERCENTILE        = 328,
    /// <summary>
    /// Represents the PERCENTRANK function.
    /// </summary>
    [ ReferenceIndex( 1, 2 ) ]
    PERCENTRANK       = 329,
    /// <summary>
    /// Represents the PERMUT function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    PERMUT            = 299,
    /// <summary>
    /// Represents the PI function.
    /// </summary>
    [ DefaultValue( 0 ) ]
    [ ReferenceIndex( 1 ) ]
    PI                = 19,
    /// <summary>
    /// Represents the PMT function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    PMT               = 59,
    /// <summary>
    /// Represents the POISSON function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    POISSON           = 300,
    /// <summary>
    /// Represents the POWER function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    POWER             = 337,
    /// <summary>
    /// Represents the PPMT function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    PPMT              = 168,
    /// <summary>
    /// Represents the PROB function.
    /// </summary>
    [ ReferenceIndex( 3, 3, 2 ) ]
    PROB              = 317,
    /// <summary>
    /// Represents the PRODUCT function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    PRODUCT           = 183,
    /// <summary>
    /// Represents the PROPER function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    PROPER            = 114,
    /// <summary>
    /// Represents the PV function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    PV                = 56,
    /// <summary>
    /// Represents the QUARTILE function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 1, 2 ) ]
    QUARTILE          = 327,
    /// <summary>
    /// Represents the RADIANS function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    RADIANS           = 342,
    /// <summary>
    /// Represents the RAND function.
    /// </summary>
    [ DefaultValue( 0 ) ]
    [ ReferenceIndex( 1 ) ]
    RAND              = 63,
    /// <summary>
    /// Represents the RANK function.
    /// </summary>
    [ ReferenceIndex( 2, 1 ) ]
    RANK              = 216,
    /// <summary>
    /// Represents the RATE function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    RATE              = 60,
    /// <summary>
    /// Represents the REPLACE function.
    /// </summary>
    [ DefaultValue( 4 ) ]
    [ ReferenceIndex( 2 ) ]
    REPLACE           = 119,
    /// <summary>
    /// Represents the REPLACEB function.
    /// </summary>
    [ DefaultValue( 4 ) ]
    [ ReferenceIndex( 2 ) ]
    REPLACEB          = 207,
    /// <summary>
    /// Represents the RIGHT function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    RIGHT             = 116,
    /// <summary>
    /// Represents the RIGHTB function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    RIGHTB            = 209,
    /// <summary>
    /// Represents the ROMAN function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    ROMAN             = 354,
    /// <summary>
    /// Represents the ROUND function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    ROUND             = 27,
    /// <summary>
    /// Represents the ROUNDDOWN function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    ROUNDDOWN         = 213,
    /// <summary>
    /// Represents the ROUNDUP function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    ROUNDUP           = 212,
    /// <summary>
    /// Represents the ROW function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    ROW               = 8,
    /// <summary>
    /// Represents the ROWS function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 1 ) ]
    ROWS              = 76,
    /// <summary>
    /// Represents the RSQ function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    RSQ               = 313,
    /// <summary>
    /// Represents the SEARCH function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    SEARCH            = 82,
    /// <summary>
    /// Represents the SEARCHB function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    SEARCHB           = 206,
    /// <summary>
    /// Represents the SECOND function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    SECOND            = 73,
    /// <summary>
    /// Represents the SIGN function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    SIGN              = 26,
    /// <summary>
    /// Represents the SIN function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    SIN               = 15,
    /// <summary>
    /// Represents the SINH function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    SINH              = 229,
    /// <summary>
    /// Represents the SKEW function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    SKEW              = 323,
    /// <summary>
    /// Represents the SLN function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    SLN               = 142,
    /// <summary>
    /// Represents the SLOPE function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    SLOPE             = 315,
    /// <summary>
    /// Represents the SMALL function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 1 ) ]
    SMALL             = 326,
    /// <summary>
    /// Represents the SQRT function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    SQRT              = 20,
    /// <summary>
    /// Represents the STANDARDIZE function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    STANDARDIZE       = 297,
    /// <summary>
    /// Represents the STDEV function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    STDEV             = 12,
    /// <summary>
    /// Represents the STDEVA function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    STDEVA            = 366,
    /// <summary>
    /// Represents the STDEVP function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    STDEVP            = 193,
    /// <summary>
    /// Represents the STDEVPA function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    STDEVPA           = 364,
    /// <summary>
    /// Represents the STEYX function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    STEYX             = 314,
    /// <summary>
    /// Represents the SUBSTITUTE function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    SUBSTITUTE        = 120,
    /// <summary>
    /// Represents the SUBTOTAL function.
    /// </summary>
    [ ReferenceIndex( 2, 1 ) ]
    SUBTOTAL          = 344,
    /// <summary>
    /// Represents the SUM function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    SUM               = 4,
    /// <summary>
    /// Represents the SUMIF function.
    /// </summary>
    [ ReferenceIndex( 1, 2, 1 ) ]
    SUMIF             = 345,
    /// <summary>
    /// Represents the SUMPRODUCT function.
    /// </summary>
    [ ReferenceIndex( 3 ) ]
    SUMPRODUCT        = 228,
    /// <summary>
    /// Represents the SUMSQ function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    SUMSQ             = 321,
    /// <summary>
    /// Represents the SUMX2MY2 function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    SUMX2MY2          = 304,
    /// <summary>
    /// Represents the SUMX2PY2 function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    SUMX2PY2          = 305,
    /// <summary>
    /// Represents the SUMXMY2 function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    SUMXMY2           = 303,
    /// <summary>
    /// Represents the SYD function.
    /// </summary>
    [ DefaultValue( 4 ) ]
    [ ReferenceIndex( 2 ) ]
    SYD               = 143,
    /// <summary>
    /// Represents the T function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 1 ) ]
    T                 = 130,
    /// <summary>
    /// Represents the TAN function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    TAN               = 17,
    /// <summary>
    /// Represents the TANH function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    TANH              = 231,
    /// <summary>
    /// Represents the TDIST function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    TDIST             = 301,
    /// <summary>
    /// Represents the TEXT function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    TEXT              = 48,
    /// <summary>
    /// Represents the TIME function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 2 ) ]
    TIME              = 66,
    /// <summary>
    /// Represents the TIMEVALUE function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    TIMEVALUE         = 141,
    /// <summary>
    /// Represents the TINV function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 2 ) ]
    TINV              = 332,
    /// <summary>
    /// Represents the TODAY function.
    /// </summary>
    [ DefaultValue( 0 ) ]
    [ ReferenceIndex( 1 ) ]
    TODAY             = 221,
    /// <summary>
    /// Represents the TRANSPOSE function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ), 3 ) ]
    TRANSPOSE         = 83,
    /// <summary>
    /// Represents the TREND function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    TREND             = 50,
    /// <summary>
    /// Represents the TRIM function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    TRIM              = 118,
    /// <summary>
    /// Represents the TRIMMEAN function.
    /// </summary>
    [ DefaultValue( 2 ) ]
    [ ReferenceIndex( 3 ) ]
    TRIMMEAN          = 331,
    /// <summary>
    /// Represents the TRUE function.
    /// </summary>
    [ DefaultValue( 0 ) ]
    [ ReferenceIndex( 1 ) ]
    TRUE              = 34,
    /// <summary>
    /// Represents the TRUNC function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    TRUNC             = 197,
    /// <summary>
    /// Represents the TTEST function.
    /// </summary>
    [ DefaultValue( 4 ) ]
    [ ReferenceIndex( 3 ) ] //???
    TTEST             = 316,
    /// <summary>
    /// Represents the TYPE function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ), 3 ) ]
    TYPE              = 86,
    /// <summary>
    /// Represents the UPPER function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    UPPER             = 113,
    /// <summary>
    /// Represents the VALUE function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    VALUE             = 33,
    /// <summary>
    /// Represents the VAR function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    VAR               = 46,
    /// <summary>
    /// Represents the VARA function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    VARA              = 367,
    /// <summary>
    /// Represents the VARP function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    VARP              = 194,
    /// <summary>
    /// Represents the VARPA function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    VARPA             = 365,
    /// <summary>
    /// Represents the VDB function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    VDB               = 222,
    /// <summary>
    /// Represents the VLOOKUP function.
    /// </summary>
    [ ReferenceIndex( 2, 1 ) ]
    VLOOKUP           = 102,
    /// <summary>
    /// Represents the WEEKDAY function.
    /// </summary>
    [ ReferenceIndex( 2 ) ]
    WEEKDAY           = 70,
    /// <summary>
    /// Represents the WEIBULL function.
    /// </summary>
    [ DefaultValue( 4 ) ]
    [ ReferenceIndex( 2 ) ]
    WEIBULL           = 302,
    /// <summary>
    /// Represents the YEAR function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    [ ReferenceIndex( 2 ) ]
    YEAR              = 69,

    /// <summary>
    /// Represents the ZTEST function.
    /// </summary>
    [ ReferenceIndex( 1 ) ]
    [ ReferenceIndex( typeof( ArrayPtg ),  3 ) ]
    ZTEST             = 324,

    // Excel may have problems with following functions:
    
    /// <summary>
    /// Represents the ABSREF function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    ABSREF            = 79,
    /// <summary>
    /// Represents the ACTIVECELL function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    ACTIVECELL        = 94,
    /// <summary>
    /// Represents the ADDBAR function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    ADDBAR            = 151,
    /// <summary>
    /// Represents the ADDCOMMAND function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    ADDCOMMAND        = 153,
    /// <summary>
    /// Represents the ADDMENU function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    ADDMENU           = 152,
    /// <summary>
    /// Represents the ADDTOOLBAR function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    ADDTOOLBAR        = 253,
    /// <summary>
    /// Represents the APPTITLE function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    APPTITLE          = 262,
    /// <summary>
    /// Represents the ARGUMENT function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    ARGUMENT          = 81,
    /// <summary>
    /// Represents the ASC function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    ASC               = 214,
    /// <summary>
    /// Represents the CALL function.
    /// </summary>
    [ DefaultValue( 1 ) ]   // TODO: check param count
    CALL              = 150,
    /// <summary>
    /// Represents the CALLER function.
    /// </summary>
    [ DefaultValue( 0 ) ]   // TODO: check param count
    CALLER            = 89,
    /// <summary>
    /// Represents the CANCELKEY function.
    /// </summary>
    [ DefaultValue( 0 ) ]   // TODO: check param count
    CANCELKEY         = 170,
    /// <summary>
    /// Represents the CHECKCOMMAND function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    CHECKCOMMAND      = 155,
    /// <summary>
    /// Represents the CREATEOBJECT function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    CREATEOBJECT      = 236,
    /// <summary>
    /// Represents the CUSTOMREPEAT function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    CUSTOMREPEAT      = 240,
    /// <summary>
    /// Represents the CUSTOMUNDO function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    CUSTOMUNDO        = 239,
    /// <summary>
    /// Represents the DATEDIF function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    DATEDIF           = 351,
    /// <summary>
    /// Represents the DATESTRING function.
    /// </summary>
    [ DefaultValue( 1 ) ]
    DATESTRING        = 352,
    /// <summary>
    /// Represents the DBCS function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    DBCS              = 215,
    /// <summary>
    /// Represents the DELETEBAR function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    DELETEBAR         = 200,
    /// <summary>
    /// Represents the DELETECOMMAND function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    DELETECOMMAND     = 159,
    /// <summary>
    /// Represents the DELETEMENU function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    DELETEMENU        = 158,
    /// <summary>
    /// Represents the DELETETOOLBAR function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    DELETETOOLBAR     = 254,
    /// <summary>
    /// Represents the DEREF function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    DEREF             = 90,
    /// <summary>
    /// Represents the DGET function.
    /// </summary>
    [ DefaultValue( 3 ) ]
    [ ReferenceIndex( 1 ) ]
    DGET              = 235,
    /// <summary>
    /// Represents the DIALOGBOX function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    DIALOGBOX         = 161,
    /// <summary>
    /// Represents the DIRECTORY function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    DIRECTORY         = 123,
    /// <summary>
    /// Represents the DOCUMENTS function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    DOCUMENTS         = 93,
    /// <summary>
    /// Represents the ECHO function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    ECHO              = 87,
    /// <summary>
    /// Represents the ENABLECOMMAND function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    ENABLECOMMAND     = 154,
    /// <summary>
    /// Represents the ENABLETOOL function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    ENABLETOOL        = 265,
    /// <summary>
    /// Represents the EVALUATE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    EVALUATE          = 257,
    /// <summary>
    /// Represents the EXEC function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    EXEC              = 110,
    /// <summary>
    /// Represents the EXECUTE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    EXECUTE           = 178,
    /// <summary>
    /// Represents the FILES function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    FILES             = 166,
    /// <summary>
    /// Represents the FOPEN function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    FOPEN             = 132,
    /// <summary>
    /// Represents the FORMULACONVERT function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    FORMULACONVERT    = 241,
    /// <summary>
    /// Represents the FPOS function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    FPOS              = 139,
    /// <summary>
    /// Represents the FREAD function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    FREAD             = 136,
    /// <summary>
    /// Represents the FREADLN function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    FREADLN           = 135,
    /// <summary>
    /// Represents the FSIZE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    FSIZE             = 134,
    /// <summary>
    /// Represents the FWRITE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    FWRITE            = 138,
    /// <summary>
    /// Represents the FWRITELN function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    FWRITELN          = 137,
    /// <summary>
    /// Represents the FCLOSE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    FCLOSE            = 133,
    /// <summary>
    /// Represents the GETBAR function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETBAR            = 182,
    /// <summary>
    /// Represents the GETCELL function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETCELL           = 185,
    /// <summary>
    /// Represents the GETCHARTITEM function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETCHARTITEM      = 160,
    /// <summary>
    /// Represents the GETDEF function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETDEF            = 145,
    /// <summary>
    /// Represents the GETDOCUMENT function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    GETDOCUMENT       = 188,
    /// <summary>
    /// Represents the GETFORMULA function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETFORMULA        = 106,
    /// <summary>
    /// Represents the GETLINKINFO function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETLINKINFO       = 242,
    /// <summary>
    /// Represents the GETMOVIE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETMOVIE          = 335,
    /// <summary>
    /// Represents the GETNAME function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETNAME           = 107,
    /// <summary>
    /// Represents the GETNOTE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETNOTE           = 191,
    /// <summary>
    /// Represents the GETOBJECT function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETOBJECT         = 246,
    /// <summary>
    /// Represents the GETPIVOTFIELD function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETPIVOTFIELD     = 340,
    /// <summary>
    /// Represents the GETPIVOTITEM function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETPIVOTITEM      = 341,
    /// <summary>
    /// Represents the GETPIVOTTABLE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETPIVOTTABLE     = 339,
    /// <summary>
    /// Represents the GETTOOL function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETTOOL           = 259,
    /// <summary>
    /// Represents the GETTOOLBAR function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETTOOLBAR        = 258,
    /// <summary>
    /// Represents the GETWINDOW function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETWINDOW         = 187,
    /// <summary>
    /// Represents the GETWORKBOOK function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETWORKBOOK       = 268,
    /// <summary>
    /// Represents the GETWORKSPACE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GETWORKSPACE      = 186,
    /// <summary>
    /// Represents the GOTO function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GOTO              = 53,
    /// <summary>
    /// Represents the GROUP function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    GROUP             = 245,
    /// <summary>
    /// Represents the HALT function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    HALT              = 54,
    /// <summary>
    /// Represents the HELP function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    HELP              = 181,
    /// <summary>
    /// Represents the INITIATE function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    INITIATE          = 175,
    /// <summary>
    /// Represents the INPUT function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    INPUT             = 104,
    /// <summary>
    /// Represents the LASTERROR function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    LASTERROR         = 238,
    /// <summary>
    /// Represents the LINKS function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    LINKS             = 103,
    /// <summary>
    /// Represents the MOVIECOMMAND function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    MOVIECOMMAND      = 334,
    /// <summary>
    /// Represents the NAMES function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    NAMES             = 122,
    /// <summary>
    /// Represents the NOTE function.
    /// </summary>
    [ DefaultValue( 1 ) ] // TODO: check param count
    NOTE              = 192,
    /// <summary>
    /// Represents the NUMBERSTRING function.
    /// </summary>
    [ DefaultValue( 1 ) ] // TODO: check param count
    NUMBERSTRING      = 353,
    /// <summary>
    /// Represents the OPENDIALOG function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    OPENDIALOG        = 355,
    /// <summary>
    /// Represents the OPTIONSLISTSGET function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    OPTIONSLISTSGET   = 349,
    /// <summary>
    /// Represents the PAUSE function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    PAUSE             = 248,
    /// <summary>
    /// Represents the PIVOTADDDATA function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    PIVOTADDDATA      = 338,
    /// <summary>
    /// Represents the POKE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    POKE              = 177,
    /// <summary>
    /// Represents the PRESSTOOL function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    PRESSTOOL         = 266,
    /// <summary>
    /// Represents the REFTEXT function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    REFTEXT           = 146,
    /// <summary>
    /// Represents the REGISTER function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    REGISTER          = 149,
    /// <summary>
    /// Represents the REGISTERID function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    REGISTERID        = 267,
    /// <summary>
    /// Represents the RELREF function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    RELREF            = 80,
    /// <summary>
    /// Represents the RENAMECOMMAND function.
    /// </summary>
    [ DefaultValue( 2 ) ]  // TODO: check param count
    RENAMECOMMAND     = 156,
    /// <summary>
    /// Represents the REPT function.
    /// </summary>
    [ DefaultValue( 2 ) ] // TODO: check param count
    REPT              = 30,
    /// <summary>
    /// Represents the REQUEST function.
    /// </summary>
    [ DefaultValue( 1 ) ] // TODO: check param count
    REQUEST           = 176,
    /// <summary>
    /// Represents the RESETTOOLBAR function.
    /// </summary>
    [ DefaultValue( 1 ) ] // TODO: check param count
    RESETTOOLBAR      = 256,
    /// <summary>
    /// Represents the RESTART function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    RESTART           = 180,
    /// <summary>
    /// Represents the RESULT function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    RESULT            = 96,
    /// <summary>
    /// Represents the RESUME function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    RESUME            = 251,
    /// <summary>
    /// Represents the SAVEDIALOG function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    SAVEDIALOG        = 356,
    /// <summary>
    /// Represents the SAVETOOLBAR function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    SAVETOOLBAR       = 264,
    /// <summary>
    /// Represents the SCENARIOGET function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    SCENARIOGET       = 348,
    /// <summary>
    /// Represents the SELECTION function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    SELECTION         = 95,
    /// <summary>
    /// Represents the SERIES function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    SERIES            = 92,
    /// <summary>
    /// Represents the SETNAME function.
    /// </summary>
    [ DefaultValue( 1 ) ] // TODO: check param count
    SETNAME           = 88,
    /// <summary>
    /// Represents the SETVALUE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    SETVALUE          = 108,
    /// <summary>
    /// Represents the SHOWBAR function.
    /// </summary>
    [ DefaultValue( 0 ) ]  // TODO: check param count
    SHOWBAR           = 157,
    /// <summary>
    /// Represents the SPELLINGCHECK function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    SPELLINGCHECK     = 260,
    /// <summary>
    /// Represents the STEP function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    STEP              = 85,
    /// <summary>
    /// Represents the TERMINATE function.
    /// </summary>
    [ DefaultValue( 1 ) ] // TODO: check param count
    TERMINATE         = 179,
    /// <summary>
    /// Represents the TEXTBOX function.
    /// </summary>
    [ DefaultValue( 1 ) ] // TODO: check param count
    TEXTBOX           = 243,
    /// <summary>
    /// Represents the TEXTREF function.
    /// </summary>
    [ DefaultValue( 1 ) ] // TODO: check param count
    TEXTREF           = 147,
    /// <summary>
    /// Represents the UNREGISTER function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    UNREGISTER        = 201,
    /// <summary>
    /// Represents the USDOLLAR function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    USDOLLAR          = 204,
    /// <summary>
    /// Represents the VOLATIL function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    VOLATILE          = 237,
    /// <summary>
    /// Represents the WINDOWS function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    WINDOWS           = 91,
    /// <summary>
    /// Represents the WINDOWTITLE function.
    /// </summary>
    [ DefaultValue( 1 ) ]  // TODO: check param count
    WINDOWTITLE       = 263,    
  }
  /// <summary>
  /// Defines the view setting of the sheet.
  /// </summary>
  public enum SheetView
  {
      /// <summary>
      /// Normal view
      /// </summary>
      Normal,
      /// <summary>
      /// Page break preview
      /// </summary>
      PageBreakPreview,
      /// <summary>
      /// Page Layout View
      /// </summary>
      PageLayout,
  }
  /// <summary>
  /// Enumeration of Colors in Excel.
  /// </summary>
  public enum ExcelKnownColors
  {
    // Predefined colors.
    /// <summary>
    /// Represents the  color.
    /// </summary>
    Black           = 0x00,
    /// <summary>
    /// Represents the  color.
    /// </summary>
    White           = 0x01,
    /// <summary>
    /// Represents the  color.
    /// </summary>
    Red             = 0x02,
    /// <summary>
    /// Represents the  color.
    /// </summary>
    LightGreen      = 0x03,
    /// <summary>
    /// Represents the  color.
    /// </summary>
    Blue            = 0x04,
    /// <summary>
    /// Represents the  color.
    /// </summary>
    Yellow          = 0x05,
    /// <summary>
    /// Represents the  color.
    /// </summary>
    Magenta         = 0x06,
    /// <summary>
    /// Represents the  color.
    /// </summary>
    Cyan            = 0x07,
    
    // User can change this.
    
    /// <summary>
    /// No color.
    /// </summary>
    None            = 0x00,
    /// <summary>
    /// Represents the Aqua color.
    /// </summary>
    Aqua            = 0x31,
    /// <summary>
    /// Represents the BlackCustom color.
    /// </summary>
    BlackCustom     = 0x40,
    /// <summary>
    /// Represents the BlueCustom color.
    /// </summary>
    BlueCustom      = 0xC,
    /// <summary>
    /// Represents the Blue_grey color.
    /// </summary>
    Blue_grey       = 0x36,
    /// <summary>
    /// Represents the Bright_green color.
    /// </summary>
    Bright_green    = 0xB,
    /// <summary>
    /// Represents the Brown color.
    /// </summary>
    Brown           = 0x3C,
    /// <summary>
    /// Represents the Dark_blue color.
    /// </summary>
    Dark_blue       = 0x12,
    /// <summary>
    /// Represents the Dark_green color.
    /// </summary>
    Dark_green      = 0x3A,
    /// <summary>
    /// Represents the Dark_red color.
    /// </summary>
    Dark_red        = 0x10,
    /// <summary>
    /// Represents the Dark_teal color.
    /// </summary>
    Dark_teal       = 0x38,
    /// <summary>
    /// Represents the Dark_yellow color.
    /// </summary>
    Dark_yellow     = 0x13,
    /// <summary>
    /// Represents the Gold color.
    /// </summary>
    Gold            = 0x33,
    /// <summary>
    /// Represents the Green color.
    /// </summary>
    Green           = 0x11,
    /// <summary>
    /// Represents the Grey_25_percent color.
    /// </summary>
    Grey_25_percent = 0x16,
    /// <summary>
    /// Represents the Grey_40_percent color.
    /// </summary>
    Grey_40_percent = 0x37,
    /// <summary>
    /// Represents the Grey_50_percent color.
    /// </summary>
    Grey_50_percent = 0x17,
    /// <summary>
    /// Represents the Grey_80_percent color.
    /// </summary>
    Grey_80_percent = 0x3F,
    /// <summary>
    /// Represents the Indigo color.
    /// </summary>
    Indigo          = 0x3E,
    /// <summary>
    /// Represents the Lavender color.
    /// </summary>
    Lavender        = 0x2E,
    /// <summary>
    /// Represents the Light_blue color.
    /// </summary>
    Light_blue      = 0x30,
    /// <summary>
    /// Represents the Light_green color.
    /// </summary>
    Light_green     = 0x2A,
    /// <summary>
    /// Represents the Light_orange color.
    /// </summary>
    Light_orange    = 0x34,
    /// <summary>
    /// Represents the Light_turquoise color.
    /// </summary>
    Light_turquoise = 0x29,
    /// <summary>
    /// Represents the Light_yellow color.
    /// </summary>
    Light_yellow    = 0x2B,
    /// <summary>
    /// Represents the Lime color.
    /// </summary>
    Lime            = 0x32,
    /// <summary>
    /// Represents the Olive_green color.
    /// </summary>
    Olive_green     = 0x3B,
    /// <summary>
    /// Represents the Orange color.
    /// </summary>
    Orange          = 0x35,
    /// <summary>
    /// Represents the Pale_blue color.
    /// </summary>
    Pale_blue       = 0x2C,
    /// <summary>
    /// Represents the Pink color.
    /// </summary>
    Pink            = 0xE,
    /// <summary>
    /// Represents the Plum color.
    /// </summary>
    Plum            = 0x3D,
    /// <summary>
    /// Represents the Red2 color.
    /// </summary>
    Red2            = 0xA,
    /// <summary>
    /// Represents the Rose color.
    /// </summary>
    Rose            = 0x2D,
    /// <summary>
    /// Represents the Sea_green color.
    /// </summary>
    Sea_green       = 0x39,
    /// <summary>
    /// Represents the Sky_blue color.
    /// </summary>
    Sky_blue        = 0x28,
    /// <summary>
    /// Represents the Tan color.
    /// </summary>
    Tan             = 0x2F,
    /// <summary>
    /// Represents the Teal color.
    /// </summary>
    Teal            = 0x15,
    /// <summary>
    /// Represents the Turquoise color.
    /// </summary>
    Turquoise       = 0xF,
    /// <summary>
    /// Represents the Violet color.
    /// </summary>
    Violet          = 0x14,
    /// <summary>
    /// Represents the WhiteCustom color.
    /// </summary>
    WhiteCustom     = 0x9,
    /// <summary>
    /// Represents the YellowCustom color.
    /// </summary>
    YellowCustom    = 0xD,
    
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom0         = 8,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom1         = 9,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom2         = 10,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom3         = 11,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom4         = 12,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom5         = 13,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom6         = 14,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom7         = 15,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom8         = 16,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom9         = 17,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom10        = 18,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom11        = 19,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom12        = 20,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom13        = 21,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom14        = 22,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom15        = 23,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom16        = 24,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom17        = 25,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom18        = 26,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom19        = 27,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom20        = 28,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom21        = 29,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom22        = 30,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom23        = 31,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom24        = 32,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom25        = 33,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom26        = 34,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom27        = 35,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom28        = 36,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom29        = 37,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom30        = 38,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom31        = 39,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom32        = 40,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom33        = 41,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom34        = 42,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom35        = 43,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom36        = 44,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom37        = 45,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom38        = 46,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom39        = 47,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom40        = 48,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom41        = 49,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom42        = 50,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom43        = 51,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom44        = 52,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom45        = 53,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom46        = 54,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom47        = 55,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom48        = 56,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom49        = 57,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom50        = 58,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom51        = 59,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom52        = 60,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom53        = 61,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom54        = 62,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom55        = 63,
    /// <summary>
    /// Represents the Custom color.
    /// </summary>
    Custom56        = 64,
  }
  /// <summary>
  ///Enumeration of Group types in Excel.
  /// </summary>
  public enum ExcelGroupBy
  {
    /// <summary>
    ///Represents the grouping by rows option.
    /// </summary>
    ByRows,
    /// <summary>
    ///Represents the grouping by columns option.
    /// </summary>
    ByColumns
  }
  /// <summary>
  /// Enumeration of Patterns available in Excel.
  /// </summary>
  public enum ExcelPattern
  {
    /// <summary>
    /// No pattern.
    /// </summary>
    None                  = 0x0000,
    /// <summary>
    /// Represents the Solid pattern.
    /// </summary>
    Solid                 = 0x0001,
    /// <summary>
    /// Represents the Percent50 pattern.
    /// </summary>
    Percent50             = 0x0002,
    /// <summary>
    /// Represents the Percent70 pattern.
    /// </summary>
    Percent70             = 0x0003,
    /// <summary>
    /// Represents the Percent25 pattern.
    /// </summary>
    Percent25             = 0x0004,
    /// <summary>
    /// Represents the DarkHorizontal pattern.
    /// </summary>
    DarkHorizontal        = 0x0005,
    /// <summary>
    /// Represents the DarkVertical pattern.
    /// </summary>
    DarkVertical          = 0x0006,
    /// <summary>
    /// Represents the DarkDownwardDiagonal pattern.
    /// </summary>
    DarkDownwardDiagonal  = 0x0007,
    /// <summary>
    /// Represents the DarkUpwardDiagonal pattern.
    /// </summary>
    DarkUpwardDiagonal    = 0x0008,
    /// <summary>
    /// Represents the ForwardDiagonal pattern.
    /// </summary>
    ForwardDiagonal       = 0x0009,
    /// <summary>
    /// Represents the Percent75 pattern.
    /// </summary>
    Percent75             = 0x000A,
    /// <summary>
    /// Represents the Horizontal pattern.
    /// </summary>
    Horizontal            = 0x000B,
    /// <summary>
    /// Represents the Vertical pattern.
    /// </summary>
    Vertical              = 0x000C,
    /// <summary>
    /// Represents the LightDownwardDiagonal pattern.
    /// </summary>
    LightDownwardDiagonal = 0x000D,
    /// <summary>
    /// Represents the LightUpwardDiagonal pattern.
    /// </summary>
    LightUpwardDiagonal   = 0x000E,
    /// <summary>
    /// Represents the Angle pattern.
    /// </summary>
    Angle                 = 0x000F,
    /// <summary>
    /// Represents the Percent60 pattern.
    /// </summary>
    Percent60             = 0x0010,
    /// <summary>
    /// Represents the Percent10 pattern.
    /// </summary>
    Percent10             = 0x0011,
    /// <summary>
    /// Represents the Percent05 pattern.
    /// </summary>
    Percent05             = 0x0012,
    /// <summary>
    /// Represents the Percent50Gray pattern.
    /// </summary>
    Percent50Gray             = 2,
    /// <summary>
    /// Represents the Percent75Gray pattern.
    /// </summary>
    Percent75Gray             = 3,
    /// <summary>
    /// Represents the Percent25Gray pattern.
    /// </summary>
    Percent25Gray             = 4,
    /// <summary>
    /// Represents the HorizontalStripe pattern.
    /// </summary>
    HorizontalStripe          = 5,
    /// <summary>
    /// Represents the VerticalStripe pattern.
    /// </summary>
    VerticalStripe            = 6,
    /// <summary>
    /// Represents the ReverseDiagonalStripe pattern.
    /// </summary>
    ReverseDiagonalStripe     = 7,
    /// <summary>
    /// Represents the DiagonalStripe pattern.
    /// </summary>
    DiagonalStripe            = 8,
    /// <summary>
    /// Represents the DiagonalCrosshatch pattern.
    /// </summary>
    DiagonalCrosshatch        = 9,
    /// <summary>
    /// Represents the ThickDiagonalCrosshatch pattern.
    /// </summary>
    ThickDiagonalCrosshatch   = 10,
    /// <summary>
    /// Represents the ThinHorizontalStripe pattern.
    /// </summary>
    ThinHorizontalStripe      = 11,
    /// <summary>
    /// Represents the ThinVerticalStripe pattern.
    /// </summary>
    ThinVerticalStripe        = 12,
    /// <summary>
    /// Represents the ThinReverseDeagonalStripe pattern.
    /// </summary>
    ThinReverseDeagonalStripe = 13,
    /// <summary>
    /// Represents the ThinDiagonalStripe pattern.
    /// </summary>
    ThinDiagonalStripe        = 14,
    /// <summary>
    /// Represents the ThinHorizontalCrosshatch pattern.
    /// </summary>
    ThinHorizontalCrosshatch  = 15,
    /// <summary>
    /// Represents the ThinDiagonalCrosshatch pattern.
    /// </summary>
    ThinDiagonalCrosshatch    = 16,
    /// <summary>
    /// Represents the Percent125Gray pattern.
    /// </summary>
    Percent125Gray            = 17,
    /// <summary>
    /// Represents the Percent625Gray pattern.
    /// </summary>
    Percent625Gray            = 18,
    
    /// <summary>
    /// Represent gradient pattern.
    /// </summary>
    Gradient                  = 4000
  }

  /// <summary>
  /// This enumeration used for controlling output stream infill on 
  /// save operation provided by Workbook.
  /// </summary>
  [Flags]
  public enum SkipExtRecords
  {
    /// <summary>
    /// Do not skip any information from source file.
    /// </summary>
    None            = 0x00,
    /// <summary>
    /// Skip macros extension records in output stream.
    /// </summary>
    Macros          = 0x01,
    /// <summary>
    /// Skip drawings extension records in output stream. 
    /// </summary>
    Drawings        = 0x02,
    /// <summary>
    /// Skip summary information substreams creation.
    /// </summary>
    SummaryInfo     = 0x04,
    /// <summary>
    /// Skip coping of substreams from source to destination files.
    /// </summary>
    CopySubstreams  = 0x10,
    /// <summary>
    /// Skip all extended records. 
    /// </summary>
    All = Macros | Drawings | SummaryInfo | CopySubstreams,
  }

  /// <summary>
  /// Enumeration which controls visibility of worksheet in Excel.
  /// </summary>
  public enum WorksheetVisibility
  {
    /// <summary>
    /// Worksheet is visible to the user.
    /// </summary>
    Visible = 0,
    /// <summary>
    /// Worksheet is hidden for the user.
    /// </summary>
    Hidden = 1,
    /// <summary>
    /// The strong hidden flag can only be set and cleared with a Visual Basic 
    /// macro. It is not possible to make such a sheet visible via the user interface.
    /// </summary>
    StrongHidden = 2
  }

  /// <summary>
  /// Enumeration of the Save types in Excel.
  /// </summary>
  public enum ExcelSaveType
  {
    /// <summary>
    ///Represents the save as xls option.
    /// </summary>
    SaveAsXLS,
    /// <summary>
    ///Represents the save as template option.
    /// </summary>
    SaveAsTemplate
  }
  /// <summary>
  /// Enumeration of the Chart types in Excel.
  /// </summary>
  public enum ExcelChartType
  {
    /// <summary>
    /// Represents the Column_Clustered chart type.
    /// </summary>
    Column_Clustered,         // +
    /// <summary>
    /// Represents the Column_Stacked chart type.
    /// </summary>
    Column_Stacked,           // +
    /// <summary>
    /// Represents the Column_Stacked_100 chart type.
    /// </summary>
    Column_Stacked_100,       // +
    /// <summary>
    /// Represents the Column_Clustered_3D chart type.
    /// </summary>
    Column_Clustered_3D,      // +
    /// <summary>
    /// Represents the Column_Stacked_3D chart type.
    /// </summary>
    Column_Stacked_3D,        // +
    /// <summary>
    /// Represents the Column_Stacked_100_3D chart type.
    /// </summary>
    Column_Stacked_100_3D,    // +
    /// <summary>
    /// Represents the Column_3D chart type.
    /// </summary>
    Column_3D,                // +
    /// <summary>
    /// Represents the Bar_Clustered chart type.
    /// </summary>
    Bar_Clustered,            // +
    /// <summary>
    /// Represents the Bar_Stacked chart type.
    /// </summary>
    Bar_Stacked,              // +
    /// <summary>
    /// Represents the Bar_Stacked_100 chart type.
    /// </summary>
    Bar_Stacked_100,          // +
    /// <summary>
    /// Represents the Bar_Clustered_3D chart type.
    /// </summary>
    Bar_Clustered_3D,         // +
    /// <summary>
    /// Represents the Bar_Stacked_3D chart type.
    /// </summary>
    Bar_Stacked_3D,           // +
    /// <summary>
    /// Represents the Bar_Stacked_100_3D chart type.
    /// </summary>
    Bar_Stacked_100_3D,       // +
    /// <summary>
    /// Represents the Line chart type.
    /// </summary>
    Line,                     // +
    /// <summary>
    /// Represents the Line_Stacked chart type.
    /// </summary>
    Line_Stacked,             // +
    /// <summary>
    /// Represents the Line_Stacked_100 chart type.
    /// </summary>
    Line_Stacked_100,         // +
    /// <summary>
    /// Represents the Line_Markers chart type.
    /// </summary>
    Line_Markers,             // +
    /// <summary>
    /// Represents the Line_Markers_Stacked chart type.
    /// </summary>
    Line_Markers_Stacked,     // +
    /// <summary>
    /// Represents the Line_Markers_Stacked_100 chart type.
    /// </summary>
    Line_Markers_Stacked_100, // +
    /// <summary>
    /// Represents the Line_3D chart type.
    /// </summary>
    Line_3D,
    /// <summary>
    /// Represents the Pie chart type.
    /// </summary>
    Pie,
    /// <summary>
    /// Represents the Pie_3D chart type.
    /// </summary>
    Pie_3D,
    /// <summary>
    /// Represents the PieOfPie chart type.
    /// </summary>
    PieOfPie,
    /// <summary>
    /// Represents the Pie_Exploded chart type.
    /// </summary>
    Pie_Exploded,
    /// <summary>
    /// Represents the Pie_Exploded_3D chart type.
    /// </summary>
    Pie_Exploded_3D,
    /// <summary>
    /// Represents the Pie_Bar chart type.
    /// </summary>
    Pie_Bar,
    /// <summary>
    /// Represents the Scatter_Markers chart type.
    /// </summary>
    Scatter_Markers,
    /// <summary>
    /// Represents the Scatter_SmoothedLine_Markers chart type.
    /// </summary>
    Scatter_SmoothedLine_Markers,
    /// <summary>
    /// Represents the Scatter_SmoothedLine chart type.
    /// </summary>
    Scatter_SmoothedLine,
    /// <summary>
    /// Represents the Scatter_Line_Markers chart type.
    /// </summary>
    Scatter_Line_Markers,
    /// <summary>
    /// Represents the Scatter_Line chart type.
    /// </summary>
    Scatter_Line,
    /// <summary>
    /// Represents the Area chart type.
    /// </summary>
    Area,                 // +
    /// <summary>
    /// Represents the Area_Stacked chart type.
    /// </summary>
    Area_Stacked,         // +
    /// <summary>
    /// Represents the Area_Stacked_100 chart type.
    /// </summary>
    Area_Stacked_100,     // +
    /// <summary>
    /// Represents the Area_3D chart type.
    /// </summary>
    Area_3D,              // +
    /// <summary>
    /// Represents the Area_Stacked_3D chart type.
    /// </summary>
    Area_Stacked_3D,      // +
    /// <summary>
    /// Represents the Area_Stacked_100_3D chart type.
    /// </summary>
    Area_Stacked_100_3D,  // +
    /// <summary>
    /// Represents the Doughnut chart type.
    /// </summary>
    Doughnut,
    /// <summary>
    /// Represents the Doughnut_Exploded chart type.
    /// </summary>
    Doughnut_Exploded,
    /// <summary>
    /// Represents the Radar chart type.
    /// </summary>
    Radar,
    /// <summary>
    /// Represents the Radar_Markers chart type.
    /// </summary>
    Radar_Markers,
    /// <summary>
    /// Represents the Radar_Filled chart type.
    /// </summary>
    Radar_Filled,
    /// <summary>
    /// Represents the Surface_3D chart type.
    /// </summary>
    Surface_3D,
    /// <summary>
    /// Represents the Surface_NoColor_3D chart type.
    /// </summary>
    Surface_NoColor_3D,
    /// <summary>
    /// Represents the Surface_Contour chart type.
    /// </summary>
    Surface_Contour,
    /// <summary>
    /// Represents the Surface_NoColor_Contour chart type.
    /// </summary>
    Surface_NoColor_Contour,
    /// <summary>
    /// Represents the Bubble chart type.
    /// </summary>
    Bubble,
    /// <summary>
    /// Represents the Bubble_3D chart type.
    /// </summary>
    Bubble_3D,
    /// <summary>
    /// Represents the Stock_HighLowClose chart type.
    /// </summary>
    Stock_HighLowClose,
    /// <summary>
    /// Represents the Stock_OpenHighLowClose chart type.
    /// </summary>
    Stock_OpenHighLowClose,
    /// <summary>
    /// Represents the Stock_VolumeHighLowClose chart type.
    /// </summary>
    Stock_VolumeHighLowClose,
    /// <summary>
    /// Represents the Stock_VolumeOpenHighLowClose chart type.
    /// </summary>
    Stock_VolumeOpenHighLowClose,
    /// <summary>
    /// Represents the Cylinder_Clustered chart type.
    /// </summary>
    Cylinder_Clustered,
    /// <summary>
    /// Represents the Cylinder_Stacked chart type.
    /// </summary>
    Cylinder_Stacked,
    /// <summary>
    /// Represents the Cylinder_Stacked_100 chart type.
    /// </summary>
    Cylinder_Stacked_100,
    /// <summary>
    /// Represents the Cylinder_Bar_Clustered chart type.
    /// </summary>
    Cylinder_Bar_Clustered,
    /// <summary>
    /// Represents the Cylinder_Bar_Stacked chart type.
    /// </summary>
    Cylinder_Bar_Stacked,
    /// <summary>
    /// Represents the Cylinder_Bar_Stacked_100 chart type.
    /// </summary>
    Cylinder_Bar_Stacked_100,
    /// <summary>
    /// Represents the Cylinder_Clustered_3D chart type.
    /// </summary>
    Cylinder_Clustered_3D,
    /// <summary>
    /// Represents the Cone_Clustered chart type.
    /// </summary>
    Cone_Clustered,
    /// <summary>
    /// Represents the Cone_Stacked chart type.
    /// </summary>
    Cone_Stacked,
    /// <summary>
    /// Represents the Cone_Stacked_100 chart type.
    /// </summary>
    Cone_Stacked_100,
    /// <summary>
    /// Represents the Cone_Bar_Clustered chart type.
    /// </summary>
    Cone_Bar_Clustered,
    /// <summary>
    /// Represents the Cone_Bar_Stacked chart type.
    /// </summary>
    Cone_Bar_Stacked,
    /// <summary>
    /// Represents the Cone_Bar_Stacked_100 chart type.
    /// </summary>
    Cone_Bar_Stacked_100,
    /// <summary>
    /// Represents the Cone_Clustered_3D chart type.
    /// </summary>
    Cone_Clustered_3D,
    /// <summary>
    /// Represents the Pyramid_Clustered chart type.
    /// </summary>
    Pyramid_Clustered,
    /// <summary>
    /// Represents the Pyramid_Stacked chart type.
    /// </summary>
    Pyramid_Stacked,
    /// <summary>
    /// Represents the Pyramid_Stacked_100 chart type.
    /// </summary>
    Pyramid_Stacked_100,
    /// <summary>
    /// Represents the Pyramid_Bar_Clustered chart type.
    /// </summary>
    Pyramid_Bar_Clustered,
    /// <summary>
    /// Represents the Pyramid_Bar_Stacked chart type.
    /// </summary>
    Pyramid_Bar_Stacked,
    /// <summary>
    /// Represents the Pyramid_Bar_Stacked_100 chart type.
    /// </summary>
    Pyramid_Bar_Stacked_100,
    /// <summary>
    /// Represents the Pyramid_Clustered_3D chart type.
    /// </summary>
    Pyramid_Clustered_3D,
    /// <summary>
    /// Represents the chart that contain different Series types.
    /// </summary>
    Combination_Chart
  }
  /// <summary>
  /// Enumeration of the legend placement for Charts in Excel.
  /// </summary>
  public enum ExcelLegendPosition
  {
    /// <summary>
    ///Represents the bottom option.
    /// </summary>
    Bottom    = 0,
    /// <summary>
    ///Represents the Corner option.
    /// </summary>
    Corner    = 1,
    /// <summary>
    ///Represents the Top option.
    /// </summary>
    Top       = 2,
    /// <summary>
    ///Represents the Right option.
    /// </summary>
    Right     = 3,
    /// <summary>
    ///Represents the Left option.
    /// </summary>
    Left      = 4,
    /// <summary>
    ///Represents the Not Docked option.
    /// </summary>
    NotDocked = 7
  }
  /// <summary>
  /// Enumeration of the print size of charts in Excel.
  /// </summary>
  public enum ExcelPrintedChartSize
  {
    /// <summary>
    ///Represents the Custom option.
    /// </summary>
    Custom          = 1,
    /// <summary>
    ///Represents the ScaleToFit page option.
    /// </summary>
    ScaleToFitPage  = 2,
    /// <summary>
    ///Represents the Use Full Page option.
    /// </summary>
    UseFullPage     = 3
  }
  /// <summary>
  /// Enumeration of the Empty Plot area.
  /// </summary>
  public enum ExcelChartPlotEmpty
  {
    /// <summary>
    /// No plot.
    /// </summary>
    NotPlotted    = 0,
    /// <summary>
    /// Represents the Zero empty plot.
    /// </summary>
    Zero          = 1,
    /// <summary>
    /// Represents the Interpolated empty plot.
    /// </summary>
    Interpolated  = 2,
  }
  /// <summary>
  /// Enumeration of the axes used for Charts in Excel.
  /// </summary>
  public enum ExcelAxisUsed
  {
    /// <summary>
    ///Represents the Primary axis option.
    /// </summary>
    Primary             = 1,
    /// <summary>
    ///Represents the Primary and Secondary axis option.
    /// </summary>
    PrimaryAndSecondary = 2
  }
  /// <summary>
  /// Enumeration of the background mode for Charts in Excel.
  /// </summary>
  public enum ExcelChartBackgroundMode
  {
    /// <summary>
    ///Represents the Transparent option.
    /// </summary>
    Transparent = 1,
    /// <summary>
    ///Represents the Opaque option.
    /// </summary>
    Opaque = 2
  }
  /// <summary>
  /// Enumeration of the  horizontal alignment options for Charts in Excel.
  /// </summary>
  public enum ExcelChartHorzAlignment : int
  {
    /// <summary>
    ///Represents the Left alignment option for the horizontal alignment setting for Chart.
    /// </summary>
    Left    = 1,
    /// <summary>
    ///Represents the Center alignment option for the horizontal alignment setting for Chart.
    /// </summary>
    Center  = 2,
    /// <summary>
    ///Represents the Right alignment option for the horizontal alignment setting for Chart.
    /// </summary>
    Right   = 3,
    /// <summary>
    ///Represents the Justify alignment option for the horizontal alignment setting for Chart.
    /// </summary>
    Justify = 4
  }
  /// <summary>
  /// Enumeration of the vertical alignment options for Charts in Excel.
  /// </summary>
  public enum ExcelChartVertAlignment : int
  {
    /// <summary>
    ///Represents the Top alignment option for the Vertical alignment setting for Chart.
    /// </summary>
    Top     = 1,
    /// <summary>
    ///Represents the Center alignment option for the Vertical alignment setting for Chart.
    /// </summary>
    Center  = 2,
    /// <summary>
    ///Represents the Bottom alignment option for the Vertical alignment setting for Chart.
    /// </summary>
    Bottom  = 3,
    /// <summary>
    ///Represents the Justify alignment option for the Vertical alignment setting for Chart.
    /// </summary>
    Justify = 4
  }
  /// <summary>
  /// Enumeration of the Border Pattern setting for Chart formatting in Excel.
  /// </summary>
  public enum ExcelAutoType
  {
    /// <summary>
    ///Represents the Automatic option for the Border Pattern setting.
    /// </summary>
    Auto,
    /// <summary>
    ///Represents the None option for the Border Pattern setting.
    /// </summary>
    None,
    /// <summary>
    ///Represents the Custom option for the Border Pattern setting.
    /// </summary>
    Custom,
  }
  /// <summary>
  /// Enumeration of the allowed line patterns for Charts in Excel.
  /// </summary>
  public enum ExcelChartLinePattern
  {
    /// <summary>
    ///Represents the Solid line pattern setting for Chart.
    /// </summary>
    Solid = 0,
    /// <summary>
    ///Represents the Dash line pattern setting for Chart.
    /// </summary>
    Dash = 1,
    /// <summary>
    ///Represents the Dot line pattern setting for Chart.
    /// </summary>
    Dot = 2,
    /// <summary>
    ///Represents the Dash-dot line pattern setting for Chart.
    /// </summary>
    DashDot = 3,
    /// <summary>
    ///Represents the Dash-dot-dot line pattern setting for Chart.
    /// </summary>
    DashDotDot = 4,
    /// <summary>
    ///Represents the no line pattern setting for Chart.
    /// </summary>
    None = 5,
    /// <summary>
    ///Represents the Dark Gray line pattern setting for Chart.
    /// </summary>
    DarkGray = 6,
    /// <summary>
    ///Represents the Medium Gray line pattern setting for Chart.
    /// </summary>
    MediumGray = 7,
    /// <summary>
    ///Represents the Light Gray line pattern setting for Chart.
    /// </summary>
    LightGray = 8,
    /// <summary>
    /// Represents the rounded dot line pattern setting for chart.
    /// </summary>
    CircleDot = 9,
  };
  /// <summary>
  /// Enumeration of the allowed LineWeight values for Charts in Excel. 
  /// </summary>
  public enum ExcelChartLineWeight
  {
    /// <summary>
    ///Represents the Hairline weight for Chart line.
    /// </summary>
    Hairline = 0xffff, // For short must be equal to -1.
    /// <summary>
    ///Represents the Narrow weight for Chart line.
    /// </summary>
    Narrow = 0, // (single)
    /// <summary>
    ///Represents the Medium weight for Chart line.
    /// </summary>
    Medium = 1, // (double)
    /// <summary>
    ///Represents the Wide weight for Chart line.
    /// </summary>
    Wide   = 2, // (triple)
  };
  /// <summary>
  /// Enumeration of the marker types for Chart lines in Excel.
  /// </summary>
  public enum ExcelChartMarkerType
  {
    /// <summary>
    ///Represents the None option for the marker type.
    /// </summary>
    None              =  0,
    /// <summary>
    ///Represents the square style in the custom marker option for Chart lines.
    /// </summary>
    Square            =  1,
    /// <summary>
    ///Represents the diamond style in the custom marker option for Chart lines.
    /// </summary>
    Diamond           =  2,
    /// <summary>
    ///Represents the Triangle style in the custom marker option for Chart lines.
    /// </summary>
    Triangle          =  3,
    /// <summary>
    ///Represents the X style in the custom marker option for Chart lines.
    /// </summary>
    X                 =  4,
    /// <summary>
    ///Represents the Star style in the custom marker option for Chart lines.
    /// </summary>
    Star              =  5,
    /// <summary>
    ///Represents the Dow Jones style in the custom marker option for Chart lines.
    /// </summary>
    DowJones          =  6,
    /// <summary>
    ///Represents the Standard Deviation style in the custom marker option for Chart lines.
    /// </summary>
    StandardDeviation =  7,
    /// <summary>
    ///Represents the Circle style in the custom marker option for Chart lines.
    /// </summary>
    Circle            =  8,
    /// <summary>
    ///Represents the Plus style in the custom marker option for Chart lines.
    /// </summary>
    PlusSign          =  9,
  }
  /// <summary>
  /// Enumeration of the primary axis types for Charts in Excel.
  /// </summary>
  public enum ExcelAxisType
  {
    /// <summary>
    ///Represents the Category (X) Axis.
    /// </summary>
    Category,
    /// <summary>
    ///Represents the Value (Y) Axis.
    /// </summary>
    Value,
    /// <summary>
    /// Represents the Series Axis.
    /// </summary>
    Serie,
  };
  /// <summary>
  /// Enumeration of the insert options in Excel.
  /// </summary>
  public enum ExcelInsertOptions
  {
    /// <summary>
    /// Indicates that after insert operation inserted rows/columns
    /// must be formatted as row above or column left.
    /// </summary>
    FormatAsBefore,
    /// <summary>
    /// Indicates that after insert operation inserted rows/columns
    /// must be formatted as row below or column right.
    /// </summary>
    FormatAsAfter,
    /// <summary>
    /// Indicates that after insert operation inserted rows/columns
    /// must have default format.
    /// </summary>
    FormatDefault,
  };
  /// <summary>
  /// Enumeration of the type of conditional formatting in Excel.
  /// </summary>
  public enum ExcelCFType
  {
    /// <summary>
    ///Represents the Cell Value Is option for conditional formatting.
    /// </summary>
    CellValue = 1,
    /// <summary>
    ///Represents the Formula Is option for conditional formatting.
    /// </summary>
    Formula   = 2,
    /// <summary>
    ///Represents the ColorScale option for conditional formatting.
    /// </summary>
    ColorScale=3,
    /// <summary>
    ///Represents the DataBar option for conditional formatting.
    /// </summary>
    DataBar=4,
    /// <summary>
    ///Represents the IconSet option for conditional formatting.
    /// </summary>
    IconSet=6,    
    ///<summary>
    ///Represents conditional formatting rule highlights cells that are completely blank
    ///</summary>
    Blank,
    ///<summary>
    ///Represents conditional formatting rule highlights cells that are not blank
    ///</summary>
    NoBlank,
    /// <summary>
    /// Represents the Specific Text conditional formatting rule based on the text
    /// </summary>
    SpecificText,
    /// <summary>
    /// Represents conditional formatting rule highlights cells that conatins errors
    /// </summary>
    ContainsErrors,
    /// <summary>
    /// Represents conditional formatting rule highlights cells that does not conatins errors
    /// </summary>
    NotContainsErrors,
    /// <summary>
    /// Represents Time Perdiod conditional formatting rule highlights cells that has date time
    /// </summary>
    TimePeriod,
  }
  /// <summary>
  /// Enumeration of the Comparison operator for conditional formatting in Excel.
  /// </summary>
  public enum ExcelComparisonOperator
  {
    /// <summary>
    ///Represents no option for comparison in conditional formatting.
    /// </summary>
    None,    
    /// <summary>
    ///Represents between option for comparison in conditional formatting.
    /// </summary>
    Between,   
    /// <summary>
    ///Represents not between option for comparison in conditional formatting.
    /// </summary>
    NotBetween,
    /// <summary>
    ///Represents equal to option for comparison in conditional formatting.
    /// </summary>
    Equal,
    /// <summary>
    ///Represents not equal to option for comparison in conditional formatting.
    /// </summary>
    NotEqual,    
    /// <summary>
    ///Represents greater than option for comparison in conditional formatting.
    /// </summary>
    Greater,
    /// <summary>
    ///Represents less than option for comparison in conditional formatting.
    /// </summary>
    Less,    
    /// <summary>
    ///Represents greater than or equal to option for comparison in conditional formatting.
    /// </summary>
    GreaterOrEqual,
    /// <summary>
    ///Represents less than or equal to option for comparison in conditional formatting.
    /// </summary>
    LessOrEqual,
    /// <summary>
    /// Represents the begins with operation for Specific Text conditional formatting.
    /// </summary>
    BeginsWith,
    /// <summary>
    ///  Represents the contains text operation for Specific Text conditional formatting.
    /// </summary>
    ContainsText,
    /// <summary>
    ///  Represents the ends with operation for Specific Text conditional formatting.
    /// </summary>
    EndsWith,
    /// <summary>
    /// Represents the not contains text operation for Specific Text conditional formatting.
    /// </summary>
    NotContainsText,
    
  }
  /// <summary>
  /// Enumeration of the time periods for date time conditional formatting in Excel.
  /// </summary>
  public enum CFTimePeriods
  {
      /// <summary>
      /// Represents today's time period type
      /// </summary>
      Today,
      /// <summary>
      /// Represents yesterday's time period type
      /// </summary>
      Yesterday,
      /// <summary>
      /// Represents tomorrow's time period type
      /// </summary>
      Tomorrow,
      /// <summary>
      /// Represents last seven days time period type
      /// </summary>
      Last7Days,
      /// <summary>
      /// Represents this month time period type
      /// </summary>
      ThisMonth,
      /// <summary>
      /// Represents last month time period type
      /// </summary>
      LastMonth,
      /// <summary>
      /// Represents next month period type
      /// </summary>
      NextMonth,
      /// <summary>
      /// Represents this week period type
      /// </summary>
      ThisWeek,
      /// <summary>
      /// Represents last week time period type
      /// </summary>
      LastWeek,
      /// <summary>
      /// Represents next week time period type
      /// </summary>
      NextWeek,
  }
  /// <summary>
  /// Enumeration of the Comparison operator for conditional formatting in Excel.
  /// </summary>
  public enum ExcelDataValidationComparisonOperator
  {
    /// <summary>
    ///Represents between option for comparison in conditional formatting.
    /// </summary>
    Between,
    /// <summary>
    ///Represents not between option for comparison in conditional formatting.
    /// </summary>
    NotBetween,
    /// <summary>
    ///Represents equal to option for comparison in conditional formatting.
    /// </summary>
    Equal,
    /// <summary>
    ///Represents not equal to option for comparison in conditional formatting.
    /// </summary>
    NotEqual,
    /// <summary>
    ///Represents greater than option for comparison in conditional formatting.
    /// </summary>
    Greater,
    /// <summary>
    ///Represents less than option for comparison in conditional formatting.
    /// </summary>
    Less,
    /// <summary>
    ///Represents greater than or equal to option for comparison in conditional formatting.
    /// </summary>
    GreaterOrEqual,
    /// <summary>
    ///Represents less than or equal to option for comparison in conditional formatting.
    /// </summary>
    LessOrEqual,
  }
  /// <summary>
  /// Possible data types:
  /// </summary>
  public enum ExcelDataType : int
  {
    /// <summary>
    /// Represents the Any data type.
    /// </summary>
    Any        = 0,
    /// <summary>
    /// Represents the Integer data type.
    /// </summary>
    Integer    = 1,
    /// <summary>
    /// Represents the Decimal data type.
    /// </summary>
    Decimal    = 2,
    /// <summary>
    /// Represents the User data type.
    /// </summary>
    User       = 3,
    /// <summary>
    /// Represents the Date data type.
    /// </summary>
    Date       = 4,
    /// <summary>
    /// Represents the Time data type.
    /// </summary>
    Time       = 5,
    /// <summary>
    /// Represents the TextLength data type.
    /// </summary>
    TextLength = 6,
    /// <summary>
    /// Represents the Formula data type.
    /// </summary>
    Formula    = 7
  }
  /// <summary>
  /// Possible error style values:
  /// </summary>
  public enum ExcelErrorStyle : int
  {
    /// <summary>
    /// Represents the Stop error style.
    /// </summary>
    Stop     = 0,
    /// <summary>
    /// Represents the Warning error style.
    /// </summary>
    Warning  = 1,
    /// <summary>
    /// Represents the Info error style.
    /// </summary>
    Info     = 2
  }

  /// <summary>
  /// Enumeration of possible directions to shift cells after clearing a range.
  /// </summary>
  public enum ExcelMoveDirection
  {
    /// <summary>
    /// Represents the MoveLeft move direction.
    /// </summary>
    MoveLeft,
    /// <summary>
    /// Represents the MoveUp move direction.
    /// </summary>
    MoveUp,
    /// <summary>
    /// Represents the None move direction.
    /// </summary>
    None,
  }
  /// <summary>
  /// Enumeration of shapes available in Excel.
  /// </summary>
  public enum ExcelShapeType
  {
    /// <summary>
    /// Represents the AutoShape shape type.
    /// </summary>
    AutoShape         = 1,
    /// <summary>
    /// Represents the Callout shape type.
    /// </summary>
    Callout           = 2,
    /// <summary>
    /// Represents the Canvas shape type.
    /// </summary>
    Canvas            = 20,
    /// <summary>
    /// Represents the Chart shape type.
    /// </summary>
    Chart             = 3,
    /// <summary>
    /// Represents the Comment shape type.
    /// </summary>
    Comment           = 4,
    /// <summary>
    /// Represents the Diagram shape type.
    /// </summary>
    Diagram           = 21,
    /// <summary>
    /// Represents the EmbeddedOLEObject shape type.
    /// </summary>
    EmbeddedOLEObject = 7,
    /// <summary>
    /// Represents the FormControl shape type.
    /// </summary>
    FormControl       = 8,
    /// <summary>
    /// Represents the Freeform shape type.
    /// </summary>
    Freeform          = 5,
    /// <summary>
    /// Represents the Group shape type.
    /// </summary>
    Group             = 6,
    /// <summary>
    /// Represents the Line shape type.
    /// </summary>
    Line              = 9,
    /// <summary>
    /// Represents the LinkedOLEObject shape type.
    /// </summary>
    LinkedOLEObject   = 10,
    /// <summary>
    /// Represents the LinkedPicture shape type.
    /// </summary>
    LinkedPicture     = 11,
    /// <summary>
    /// Cannot be used with this property. This constant is used with shapes
    /// in other Microsoft Office applications. 
    /// </summary>
    Media             = 16,
    /// <summary>
    /// Represents the OLEControlObject shape type.
    /// </summary>
    OLEControlObject  = 12,
    /// <summary>
    /// Represents the Picture shape type.
    /// </summary>
    Picture           = 13,
    /// <summary>
    /// Cannot be used with this property. This constant is used with shapes
    /// in other Microsoft Office applications. 
    /// </summary>
    Placeholder       = 14,
    /// <summary>
    /// Represents the ScriptAnchor shape type.
    /// </summary>
    ScriptAnchor      = 18,
    /// <summary>
    /// Represents the ShapeTypeMixed shape type.
    /// </summary>
    ShapeTypeMixed    = -2,
    /// <summary>
    /// Represents the Table shape type.
    /// </summary>
    Table             = 19,
    /// <summary>
    /// Represents the TextBox shape type.
    /// </summary>
    TextBox           = 17,
    /// <summary>
    /// Represents the TextEffect shape type.
    /// </summary>
    TextEffect        = 15,
    /// <summary>
    /// Represents the Unknown shape type.
    /// </summary>
    Unknown           = 0,
  }
  /// <summary>
  /// Enumeration to specify the possible Text Rotation options.
  /// </summary>
  public enum ExcelTextRotation
  {
    /// <summary>
    /// Represents the LeftToRight text rotation.
    /// </summary>
    LeftToRight         = 0,
    /// <summary>
    /// Represents the TopToBottom text rotation.
    /// </summary>
    TopToBottom         = 1,
    /// <summary>
    /// Represents the CounterClockwise text rotation.
    /// </summary>
    CounterClockwise    = 2,
    /// <summary>
    /// Represents the Clockwise text rotation.
    /// </summary>
    Clockwise           = 3
  }
  /// <summary>
  /// Enumeration to align the excel comment Horizontally.
  /// </summary>
  public enum ExcelCommentHAlign
  {
    /// <summary>
    /// Represents the Left comment align.
    /// </summary>
    Left      = 1,
    /// <summary>
    /// Represents the Center comment align.
    /// </summary>
    Center    = 2,
    /// <summary>
    /// Represents the Right comment align.
    /// </summary>
    Right     = 3,
    /// <summary>
    /// Represents the Justified comment align.
    /// </summary>
    Justified = 4,
    /// <summary>
    /// Represents the Justified comment align.
    /// </summary>
    Justify = 4,
    /// <summary>
    /// Represents the Distributed comment align.
    /// </summary>
    Distributed = 7,
  }

  /// <summary>
  /// Enumeration to align the excel comment vertically
  /// </summary>
  public enum ExcelCommentVAlign
  {
    /// <summary>
    /// Represents the Top comment align.
    /// </summary>
    Top     = 1,
    /// <summary>
    /// Represents the Center comment align.
    /// </summary>
    Center  = 2,
    /// <summary>
    /// Represents the Bottom comment align.
    /// </summary>
    Bottom  = 3,
    /// <summary>
    /// Represents the Justified comment align.
    /// </summary>
    Justify = 4,
    /// <summary>
    /// Represents the Distributed comment align.
    /// </summary>
    Distributed = 7,
  }
  /// <summary>
  /// Enumeration to specify if the spreadsheet should be opened inside browser or saved 
  /// as an attachment to disk.
  /// </summary>
  public enum ExcelDownloadType
  {
    /// <summary>
    ///  File should be opened in browser.
    /// </summary>
    Open,
    /// <summary>
    /// Prompt dialog should be displayed.
    /// </summary>
    PromptDialog,
  }
  /// <summary>
  /// HttpContent type.
  /// </summary>
  public enum ExcelHttpContentType
  {
    /// <summary>
    /// Represents the Excel97 HttpContent type.
    /// </summary>
    Excel97,
    /// <summary>
    /// Represents the Excel2000 HttpContent type.
    /// </summary>
    Excel2000,
    /// <summary>
    /// Represents the Excel2007 HttpContent type.
    /// </summary>
    Excel2007,
    /// <summary>
    /// Represents the Excel2010 HttpContent type.
    /// </summary>
    Excel2010,
    /// <summary>
    /// Represents the Excel2013 HttpContent type.
    /// </summary>
    Excel2013,
    /// <summary>
    /// Represents the CSV HttpContent type.
    /// </summary>
    CSV,
  }
  /// <summary>
  /// Enumeration to specify the style merge options.
  /// </summary>
  public enum ExcelStyleMergeOptions
  {
    /// <summary>
    /// Represents the Leave style merge option.
    /// </summary>
    Leave,
    /// <summary>
    /// Represents the Replace style merge option.
    /// </summary>
    Replace,
    /// <summary>
    /// Represents the CreateDiffName style merge option.
    /// </summary>
    CreateDiffName,
  }
  /// <summary>
  /// Enumeration to specify Names Merge options.
  /// </summary>
  public enum ExcelNamesMergeOptions
  {
    /// <summary>
    /// Represents the Leave names merge option.
    /// </summary>
    Leave,
    /// <summary>
    /// Represents the Replace names merge option.
    /// </summary>
    Replace,
    /// <summary>
    /// Represents the Rename names merge option.
    /// </summary>
    Rename,
    /// <summary>
    /// Represents the MakeLocal names merge option.
    /// </summary>
    MakeLocal,
  }
  /// <summary>
  /// Enumeration to specify options when copying worksheets.
  /// </summary>
  [ Flags ]
  public enum ExcelWorksheetCopyFlags
  {
    /// <summary>
    /// No flags.
    /// </summary>
    None = 0,
    /// <summary>
    /// Represents the ClearBefore copy flags.
    /// </summary>
    ClearBefore       = 1,
    /// <summary>
    /// Represents the CopyNames copy flags.
    /// </summary>
    CopyNames         = 2,
    /// <summary>
    /// Represents the CopyCells copy flags.
    /// </summary>
    CopyCells         = 4,
    /// <summary>
    /// Represents the CopyRowHeight copy flags.
    /// </summary>
    CopyRowHeight     = 8,
    /// <summary>
    /// Represents the CopyColumnHeight copy flags.
    /// </summary>
    CopyColumnHeight  = 0x10,
    /// <summary>
    /// Represents the CopyOptions copy flags.
    /// </summary>
    CopyOptions       = 0x20,
    /// <summary>
    /// Represents the CopyMerges copy flags.
    /// </summary>
    CopyMerges        = 0x40,
    /// <summary>
    /// Represents the CopyShapes copy flags.
    /// </summary>
    CopyShapes        = 0x80,
    /// <summary>
    /// Represents the CopyConditionlFormats copy flags.
    /// </summary>
    CopyConditionlFormats        = 0x100,
    /// <summary>
    /// Represents the CopyAutoFilters copy flags.
    /// </summary>
    CopyAutoFilters              = 0x200,
    /// <summary>
    /// Represents the CopyDataValidations copy flags.
    /// </summary>
    CopyDataValidations          = 0x400,
    /// <summary>
    /// Copy page setup (page breaks, paper orientation, header, footer and other properties).
    /// </summary>
    CopyPageSetup                = 0x800,
    /// <summary>
    /// Copy table objects.
    /// </summary>
    CopyTables                   = 0x0A00,
    /// <summary>
    /// Copy pivot table objects.
    /// </summary>
    CopyPivotTables              = 0x1000,
    /// <summary>
    /// Copies palette.
    /// </summary>
    CopyPalette                  = 0x2000,
    /// <summary>
    /// Represents the CopyAll copy flags, except palette.
    /// </summary>
    CopyAll = ClearBefore | CopyNames | CopyCells | CopyRowHeight
      | CopyColumnHeight | CopyOptions | CopyMerges | CopyShapes
      | CopyConditionlFormats | CopyAutoFilters | CopyDataValidations
      | CopyPageSetup | CopyTables | CopyPivotTables| CopyPalette ,
    /// <summary>
    /// Represents the CopyWithoutNames copy flags.
    /// </summary>
    CopyWithoutNames = ClearBefore | CopyCells | CopyRowHeight
      | CopyColumnHeight | CopyOptions | CopyMerges | CopyShapes
      | CopyConditionlFormats | CopyAutoFilters | CopyDataValidations
      | CopyPageSetup | CopyTables | CopyPivotTables,
  }

  /// <summary>
  /// Enumeration to specify the options to update formulas and merged ranges during copy range
  /// operation.
  /// </summary>
  [ Flags ]
  public enum ExcelCopyRangeOptions
  {
    /// <summary>
    /// No flags.
    /// </summary>
    None            = 0,
    /// <summary>
    /// Indicates whether update formula during copy. WARNING: you should always
    /// specify this flag if your operations could change position of Array formula.
    /// </summary>
    UpdateFormulas  = 1,
    /// <summary>
    /// Indicates whether update merges during copy.
    /// </summary>
    UpdateMerges    = 2,
    /// <summary>
    /// Indicates that we have to copy styles during range copy.
    /// </summary>
    CopyStyles      = 4,
    /// <summary>
    /// Indicates that we have to copy shapes during range copy.
    /// </summary>
    CopyShapes      = 8,
    /// <summary>
    /// Indicates that we have to copy error indicators during range copy.
    /// </summary>
    CopyErrorIndicators = 16,
    /// <summary>
    /// Indicates that we have to copy conditional formats during range copy.
    /// </summary>
    CopyConditionalFormats = 32,
    /// <summary>
    /// Indicates that we have to copy data validations during range copy.
    /// </summary>
    CopyDataValidations = 64,
    /// <summary>
    /// All flags.
    /// </summary>
    All = UpdateFormulas |
      UpdateMerges |
      CopyStyles |
      CopyShapes |
      CopyErrorIndicators |
      CopyConditionalFormats |
      CopyDataValidations,
  }
  /// <summary>
  /// Enumeration to specify the options of excel formula
  /// </summary>
  [ Flags ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum ExcelParseFormulaOptions
  {
    /// <summary>
    /// No flags.
    /// </summary>
    None      = 0,
    /// <summary>
    /// Represents the RootLevel formula parse option.
    /// </summary>
    RootLevel = 1,
    /// <summary>
    /// Represents the InArray formula parse option.
    /// </summary>
    InArray   = 2,
    /// <summary>
    /// Represents the InName formula parse option.
    /// </summary>
    InName    = 4,
    /// <summary>
    /// Operand in function.
    /// </summary>
    ParseOperand = 8,
    /// <summary>
    /// Operand is complex.
    /// </summary>
    ParseComplexOperand = 16,
    /// <summary>
    /// Indicates that R1C1 notation must be used.
    /// </summary>
    UseR1C1 = 32,
  }

  /// <summary>
  /// Enumeration to specify an excel cell type.
  /// </summary>
  public enum ExcelCellType
  {
    /// <summary>
    /// Cells of any format.
    /// </summary>
    AllFormatConditions,   
    /// <summary>
    /// Cells having validation criteria.
    /// </summary>
    AllValidation,
    /// <summary>
    /// Empty cells.
    /// </summary>
    Blanks,
    /// <summary>
    /// Cells containing notes.
    /// </summary>
    Comments,
    /// <summary>
    /// Cells containing constants.
    /// </summary>
    Constants,
    /// <summary>
    /// Cells containing formulas.
    /// </summary>
    Formulas,
    /// <summary>
    /// The last cell in the used range.
    /// </summary>
    LastCell,
    /// <summary>
    /// Cells having the same format.
    /// </summary>
    SameFormatConditions,
    /// <summary>
    /// Cells having the same validation criteria.
    /// </summary>
    SameValidation,
    /// <summary>
    /// All visible cells.
    /// </summary>
    Visible,
  }
  /// <summary>
  /// Data type for autofilters.
  /// </summary>
  public enum ExcelFilterDataType
  {
    /// <summary>
    /// Represents the filter data type.
    /// </summary>
    NotUsed = 0,
    /// <summary>
    /// Represents the FloatingPoint filter data type.
    /// </summary>
    FloatingPoint,
    /// <summary>
    /// Represents the String filter data type.
    /// </summary>
    String,
    /// <summary>
    /// Represents the Boolean filter data type.
    /// </summary>
    Boolean,
    /// <summary>
    /// Represents the ErrorCode filter data type.
    /// </summary>
    ErrorCode,
    /// <summary>
    /// Represents the MatchAllBlanks filter data type.
    /// </summary>
    MatchAllBlanks,
    /// <summary>
    /// Represents the MatchAllNonBlanks filter data type.
    /// </summary>
    MatchAllNonBlanks,
  }
  /// <summary>
  /// Possible conditions in autofilter.
  /// </summary>
  public enum ExcelFilterCondition
  {
    /// <summary>
    /// Represents the Less filter condition type.
    /// </summary>
    Less            = 1,
    /// <summary>
    /// Represents the Equal filter condition type.
    /// </summary>
    Equal           = 2,
    /// <summary>
    /// Represents the LessOrEqual filter condition type.
    /// </summary>
    LessOrEqual     = 3,
    /// <summary>
    /// Represents the Greater filter condition type.
    /// </summary>
    Greater         = 4,
    /// <summary>
    /// Represents the NotEqual filter condition type.
    /// </summary>
    NotEqual        = 5,
    /// <summary>
    /// Represents the GreaterOrEqual filter condition type.
    /// </summary>
    GreaterOrEqual  = 6,
  }
  /// <summary>
  /// Parsing options.
  /// </summary>
  [ Flags ]
  public enum ExcelParseOptions
  {
    /// <summary>
    /// Represents the Default parse option.
    /// </summary>
    Default = 0,
    /// <summary>
    /// Represents the SkipStyles parse option
    /// </summary>
    [ Obsolete( "This value is obsolete and won't affect on the XlsIO. It will be removed in next release. Sorry for inconvenience." ) ]
    SkipStyles = 1,
    /// <summary>
    /// Represents the DoNotParseCharts parse option
    /// </summary>
    DoNotParseCharts = 2,
    /// <summary>
    /// This is special mode. In this mode user can't modify strings or add new strings
    /// (numbers and other types are ok), but it gives more speed and less memory usage.
    /// </summary>
    [ Obsolete( "This value is obsolete and won't affect on the XlsIO performance. It will be removed in next release. Sorry for inconvenience." ) ]
    StringsReadOnly = 4,
    /// <summary>
    /// Preserves the Pivot table.
    /// </summary>
    DoNotParsePivotTable=8,
    /// <summary>
    /// Parses the sheet when accessed.
    /// </summary>
    ParseWorksheetsOnDemand = 16,
  }
  /// <summary>
  /// ExportDataTable options.
  /// </summary>
  [ Flags ]
  public enum ExcelExportDataTableOptions
  {
    /// <summary>
    /// No datatable exports flags.
    /// </summary>
    None          = 0,
    /// <summary>
    /// Represents the ColumnNames datatable export flag.
    /// </summary>
    ColumnNames   = 1,
    /// <summary>
    /// Represents the ComputedFormulaValues datatable export flag.
    /// </summary>
    ComputedFormulaValues = 2,
    /// <summary>
    /// Indicates that XlsIO should try to detect column types.
    /// </summary>
    DetectColumnTypes = 4,
    /// <summary>
    /// When DetectColumnTypes is set and this flag is set too, it means that
    /// default column style must be used to detect style, if this flag is not set,
    /// but DetectColumnTypes is set, then first cell in the column will be used
    /// to detect column type.
    /// </summary>
    DefaultStyleColumnTypes = 8,
    /// <summary>
    /// Indicates whether to preserve Ole date (double numbers) instead of date-time values.
    /// </summary>
    PreserveOleDate = 16,
  }
  /// <summary>
  /// Excel rectangle style. Used in chart frames.
  /// </summary>
  public enum ExcelRectangleStyle
  {
    /// <summary>
    /// Represents the Regular rectangle style.
    /// </summary>
    Regular = 0,
    /// <summary>
    /// Represents the Shadowed rectangle style.
    /// </summary>
    Shadowed = 4,
  }
  /// <summary>
  /// The order in which page fields are added to the PivotTable report�s layout.
  /// </summary>
  public enum ExcelPagesOrder
  {
    /// <summary>
    /// Represents the DownThenOver pages order.
    /// </summary>
    DownThenOver,
    /// <summary>
    /// Represents the OverThenDown pages order.
    /// </summary>
    OverThenDown,
  }
  /// <summary>
  /// Possible types of hyperlinks.
  /// </summary>
  public enum ExcelHyperLinkType
  {
    /// <summary>
    /// No hyperlink.
    /// </summary>
    None,
    /// <summary>
    /// Represents the Url hyperlink type.
    /// </summary>
    Url,
    /// <summary>
    /// Represents the File hyperlink type.
    /// </summary>
    File,
    /// <summary>
    /// Represents the Unc hyperlink type.
    /// </summary>
    Unc,
    /// <summary>
    /// Represents the Workbook hyperlink type.
    /// </summary>
    Workbook
  }
  /// <summary>
  /// Data source type.
  /// </summary>
  public enum ExcelDataSourceType
  {
    /// <summary>
    /// Represents the Worksheet data source type.
    /// </summary>
    Worksheet = 1,
    /// <summary>
    /// Represents the ExternalData data source type.
    /// </summary>
    ExternalData = 2,
    /// <summary>
    /// Represents the Consolidation data source type.
    /// </summary>
    Consolidation = 4,
    /// <summary>
    /// Represents the ScenarioPivotTable data source type.
    /// </summary>
    ScenarioPivotTable = 8,
  }
  /// <summary>
  /// Possible types of param, specified as string, number in FindFirst, FindAll methods. 
  /// </summary>
  [ Flags ]
  public enum ExcelFindType
  {
    /// <summary>
    /// Represents the Text Finding type.
    /// </summary>
    Text = 1,
    /// <summary>
    /// Represents the Formula Finding type.
    /// </summary>
    Formula = 2,
    /// <summary>
    /// Represents the FormulaStringValue Finding type.
    /// </summary>
    FormulaStringValue = 4,
    /// <summary>
    /// Represents the Error Finding type.
    /// </summary>
    Error = 8,
    /// <summary>
    /// Represents the Number Finding type.
    /// </summary>
    Number = 16,
    /// <summary>
    /// Represents the FormulaValue Finding type.
    /// </summary>
    FormulaValue = 32,
  }
  /// <summary>
  /// Possible type of finding options
  /// </summary>
  public enum ExcelFindOptions
  {
      /// <summary>
      /// Represents none of the option is selected.
      /// </summary>
      None,
      /// <summary>
      /// Represents to match the case while finding the value.
      /// </summary>
      MatchCase,
      /// <summary>
      /// Represents to match the whole search word while finding the value.
      /// </summary>
      MatchEntireCellContent,
  }
  /// <summary>
  /// Possible types of direction order.
  /// </summary>
  [ Flags ]
  public enum ExcelReadingOrderType
  {
    /// <summary>
    /// Represents the Context reading order type.
    /// </summary>
    Context = 0,
    /// <summary>
    /// Represents the LeftToRight reading order type.
    /// </summary>
    LeftToRight = 1,
    /// <summary>
    /// Represents the RightToLeft reading order type.
    /// </summary>
    RightToLeft = 2
  }
  /// <summary>
  /// Possible image formats.
  /// </summary>
  public enum ExcelImageFormat
  {
    /// <summary>
    /// Try to keep original picture format.
    /// </summary>
    Original,
    //    /// <summary>
    //    /// DIN picture format.
    //    /// </summary>
    //    Dib = ( int )MsoBlipType.msoblipDIB,
    /// <summary>
    /// Use PNG picture format.
    /// </summary>
    Png = ( int )MsoBlipType.msoblipPNG,
    /// <summary>
    /// Use JPG picture format.
    /// </summary>
    Jpeg = ( int )MsoBlipType.msoblipJPEG,
  }
  /// <summary>
  /// Represents the MeasureUnits types.
  /// </summary>
  public enum MeasureUnits
  {
    /// <summary>
    /// Specifies 1/75 inch as the unit of measure.
    /// </summary>
    Display,
    /// <summary>
    /// Specifies the document unit (1/300 inch) as the unit of measure.
    /// </summary>
    Document,
    /// <summary>
    /// Specifies the inch as the unit of measure.
    /// </summary>
    Inch,
    /// <summary>
    /// Specifies the millimeter as the unit of measure.
    /// </summary>
    Millimeter,
    /// <summary>
    /// Specifies the centimeter as the unit of measure.
    /// </summary>
    Centimeter,
    /// <summary>
    /// Specifies a device pixel as the unit of measure.
    /// </summary>
    Pixel,
    /// <summary>
    /// Specifies a printer's point (1/72 inch) as the unit of measure.
    /// </summary>
    Point,
    /// <summary>
    /// 12700 emu's = 1 point.
    /// </summary>
    EMU,
  }
  /// <summary>
  /// Bubble size.
  /// </summary>
  public enum ExcelBubbleSize : int
  {
    /// <summary>
    /// Represents the Area bubble size option.
    /// </summary>
    Area  = 1,
    /// <summary>
    /// Represents the Width bubble size option.
    /// </summary>
    Width = 2
  }
  /// <summary>
  /// Enumeration limits values which can be set by user.
  /// </summary>
  public enum ExcelPieType
  {
    /// <summary>
    /// Represents the Normal pie type.
    /// </summary>
    Normal = 0,
    /// <summary>
    /// Represents the Pie pie type.
    /// </summary>
    Pie = 1,
    /// <summary>
    /// Represents the Bar pie type.
    /// </summary>
    Bar = 2
  }
  /// <summary>
  /// Enumeration limits values which can be set by user.
  /// </summary>
  public enum ExcelSplitType
  {
    /// <summary>
    /// Represents the Position split type.
    /// </summary>
    Position = 0,
    /// <summary>
    /// Represents the Value split type.
    /// </summary>
    Value = 1,
    /// <summary>
    /// Represents the Percent split type.
    /// </summary>
    Percent = 2,
    /// <summary>
    /// Represents the Custom split type.
    /// </summary>
    Custom = 3
  }
  /// <summary>
  /// Represents the drop line style type.
  /// </summary>
  public enum ExcelDropLineStyle
  {
    /// <summary>
    /// Represents the Drop line style.
    /// </summary>
    Drop  = 0,
    /// <summary>
    /// Represents the HiLow drop line style.
    /// </summary>
    HiLow = 1,
    /// <summary>
    /// Represents the Series drop line style.
    /// </summary>
    Series = 2
  }
  /// <summary>
  /// Represents the ExcelLegendSpacing options.
  /// </summary>
  public enum ExcelLegendSpacing
  {
    /// <summary>
    /// Represents the Close ExcelLegendSpacing option.
    /// </summary>
    Close = 0,
    /// <summary>
    /// Represents the Medium ExcelLegendSpacing option.
    /// </summary>
    Medium = 1,
    /// <summary>
    /// Represents the Open ExcelLegendSpacing option.
    /// </summary>
    Open = 2,
  }
  /// <summary>
  /// Base format options.
  /// </summary>
  public enum ExcelBaseFormat
  {
    /// <summary>
    /// Represents Rectangle base format.
    /// </summary>
    Rectangle = 0,
    /// <summary>
    /// Represents Circle base format.
    /// </summary>
    Circle   = 1,
  }
  /// <summary>
  /// Top format options.
  /// </summary>
  public enum ExcelTopFormat
  {
    /// <summary>
    /// Represents Straight top format.
    /// </summary>
    Straight = 0,
    /// <summary>
    /// Represents Sharp top format.
    /// </summary>
    Sharp    = 1,
    /// <summary>
    /// Represents Trunc top format.
    /// </summary>
    Trunc    = 2,
  }
  /// <summary>
  /// Object text is linked to.
  /// </summary>
  public enum ExcelObjectTextLink : int
  {
    /// <summary>
    /// Represents the Chart object text type.
    /// </summary>
    Chart     = 1,
    /// <summary>
    /// Represents the YAxis object text type.
    /// </summary>
    YAxis     = 2,
    /// <summary>
    /// Represents the XAxis object text type.
    /// </summary>
    XAxis     = 3,
    /// <summary>
    /// Represents the DataLabel object text type.
    /// </summary>
    DataLabel = 4,
    /// <summary>
    /// Represents the ZAxis object text type.
    /// </summary>
    ZAxis     = 7,
    /// <summary>
    /// Represents the DisplayUnit object text type.
    /// </summary>
    DisplayUnit = 12
  }
  /// <summary>
  /// Enumeration which represents axis line identifier.
  /// </summary>
  public enum ExcelAxisLineIdentifier
  {
    /// <summary>
    /// The axis line itself.
    /// </summary>
    AxisLineItself = 0,
    /// <summary>
    /// Major grid line along the axis.
    /// </summary>
    MajorGridLine = 1,
    /// <summary>
    /// Minor grid line along the axis.
    /// </summary>
    MinorGridLine = 2,
    /// <summary>
    /// Walls or floor -- walls if parent axis is type 0 or 2;
    /// floor if parent axis is type 1.
    /// </summary>
    WallsOrFloor = 3,
  };
  /// <summary>
  /// Possible format types.
  /// </summary>
  public enum ExcelFormatType
  {
    /// <summary>
    /// Represents unknown format type.
    /// </summary>
    Unknown,
    /// <summary>
    /// Represents general number format.
    /// </summary>
    General,
    /// <summary>
    /// Represents text number format.
    /// </summary>
    Text,
    /// <summary>
    /// Represents number number format.
    /// </summary>
    Number,
    /// <summary>
    /// Represents datetime number format.
    /// </summary>
    DateTime,
  }
  /// <summary>
  /// Indicates what property will be used for export.
  /// </summary>
  public enum ExcelExportType
  {
    /// <summary>
    /// Represents Boolean property.
    /// </summary>
    Bool,
    /// <summary>
    /// Represents Number property.
    /// </summary>
    Number,
    /// <summary>
    /// Represents Text property.
    /// </summary>
    Text,
    /// <summary>
    /// Represents DateTime property.
    /// </summary>
    DateTime,
    /// <summary>
    /// Represents TimeSpan property.
    /// </summary>
    TimeSpan,
    /// <summary>
    /// Represents Error property.
    /// </summary>
    Error,
    /// <summary>
    /// Represents Formula.
    /// </summary>
    Formula,
  }
  /// <summary>
  /// Supported Xml save types.
  /// </summary>
  public enum ExcelXmlSaveType
  {
    /// <summary>
    /// Xml format used by MS Excel.
    /// </summary>
    MSExcel,
    /// <summary>
    /// Xml format used by Syncfusion.DLS. This format is used to export
    /// into Syncfusion.Pdf and Syncfusion.DocIO.
    /// </summary>
    DLS,
//    /// <summary>
//    /// OpenOffice XML file format. Not supported yet.
//    /// </summary>
//    OpenOffice,
//    /// <summary>
//    /// Office 12 XML file format. Not supported yet.
//    /// </summary>
//    Office12
  }
  /// <summary>
  /// Property IDs for the SummaryInformation Property Set.
  /// </summary>
  public enum ExcelBuiltInProperty
  {
    /// <summary>
    /// Title document property Id.
    /// </summary>
    Title               = 0x00000002,
    /// <summary>
    /// Subject document property Id.
    /// </summary>
    Subject,
    /// <summary>
    /// Author document property Id.
    /// </summary>
    Author,
    /// <summary>
    /// Keywords document property Id.
    /// </summary>
    Keywords,
    /// <summary>
    /// Comments document property Id.
    /// </summary>
    Comments,
    /// <summary>
    /// Template document property Id.
    /// </summary>
    Template,
    /// <summary>
    /// LastAuthor document property Id.
    /// </summary>
    LastAuthor,
    /// <summary>
    /// Revnumber document property Id.
    /// </summary>
    RevisionNumber,
    /// <summary>
    /// EditTime document property Id.
    /// </summary>
    EditTime,
    /// <summary>
    /// LastPrinted document property Id.
    /// </summary>
    LastPrinted,
    /// <summary>
    /// CreationDate document property Id.
    /// </summary>
    CreationDate,
    /// <summary>
    /// LastSaveDate document property Id.
    /// </summary>
    LastSaveDate,
    /// <summary>
    /// PageCount document property Id.
    /// </summary>
    PageCount,
    /// <summary>
    /// WordCount document property Id.
    /// </summary>
    WordCount,
    /// <summary>
    /// CharCount document property Id.
    /// </summary>
    CharCount,
    /// <summary>
    /// Thumbnail document property Id.
    /// </summary>
    Thumbnail,
    /// <summary>
    /// ApplicationName document property Id.
    /// </summary>
    ApplicationName,
    /// <summary>
    /// Security document property Id.
    /// </summary>
    Security,

    /// <summary>
    /// Category Id.
    /// </summary>
    Category = 1000,
    /// <summary>
    /// Target format for presentation (35mm, printer, video, and so on) id.
    /// </summary>
    PresentationTarget,
    /// <summary>
    /// ByteCount Id.
    /// </summary>
    ByteCount,
    /// <summary>
    /// LineCount Id.
    /// </summary>
    LineCount,
    /// <summary>
    /// ParCount Id.
    /// </summary>
    ParagraphCount,
    /// <summary>
    /// SlideCount Id.
    /// </summary>
    SlideCount,
    /// <summary>
    /// NoteCount Id.
    /// </summary>
    NoteCount,
    /// <summary>
    /// HiddenCount Id.
    /// </summary>
    HiddenCount,
    /// <summary>
    /// MmclipCount Id.
    /// </summary>
    MultimediaClipCount,
    /// <summary>
    /// ScaleCrop property Id.
    /// </summary>
    ScaleCrop,
    /// <summary>
    /// HeadingPair Id.
    /// </summary>
    HeadingPair,
    /// <summary>
    /// DocParts Id.
    /// </summary>
    DocParts,
    /// <summary>
    /// Manager Id.
    /// </summary>
    Manager,
    /// <summary>
    /// Company Id.
    /// </summary>
    Company,
    /// <summary>
    /// LinksDirty Id.
    /// </summary>
    LinksDirty,
  }
  /// <summary>
  /// Enumeration of possible directions to clear the Cell formats, content, comments or clear all of them.
  /// </summary>
  public enum ExcelClearOptions
  {
      /// <summary>
      /// Clears the formats of the cell.
      /// </summary>
      ClearFormat,
      /// <summary>
      /// Clears the contents of the cell.
      /// </summary>
      ClearContent,
      /// <summary>
      /// Clears the comments of the cell.
      /// </summary>
      ClearComment,
      /// <summary>
      /// Clears the comments, content and formats of the cell.
      /// </summary>
      ClearAll,
      /// <summary>
      /// Clears all the Conditional Format
      /// </summary>
      ClearConditionalFormats,
      /// <summary>
      /// Clears all data validations
      /// </summary>
      ClearDataValidations,
  }
   

  /// <summary>
  /// Supported Xml open types.
  /// </summary>
  public enum ExcelXmlOpenType
  {
    /// <summary>
    /// Xml format used by MS Excel.
    /// </summary>
    MSExcel,
    //    /// <summary>
    //    /// OpenOffice XML file format. Not supported yet.
    //    /// </summary>
    //    OpenOffice,
    //    /// <summary>
    //    /// Office 12 XML file format. Not supported yet.
    //    /// </summary>
    //    Office12
  }
  /// <summary>
  /// Represents line style.
  /// </summary>
  public enum ExcelShapeLineStyle
  {
    /// <summary>
    /// Represents single line style.
    /// </summary>
    Line_Single = 1,
    /// <summary>
    /// Represents thin thin line style.
    /// </summary>
    Line_Thin_Thin = 2,
    /// <summary>
    /// Represents thin thick line style.
    /// </summary>
    Line_Thin_Thick = 3,
    /// <summary>
    /// Represents thick thin line style.
    /// </summary>
    Line_Thick_Thin = 4,
    /// <summary>
    /// Represents thick between thin line style.
    /// </summary>
    Line_Thick_Between_Thin = 5
  }
  /// <summary>
  /// Represents shape dash line style.
  /// </summary>
  public enum ExcelShapeDashLineStyle
  {
    /// <summary>
    /// Represents solid style.
    /// </summary>
    Solid = 0,
    /// <summary>
    /// Represents Dotted style.
    /// </summary>
    Dotted = 1,
    /// <summary>
    /// Represents Dotted_Strange style.
    /// </summary>
    Dotted_Round = 2,
    /// <summary>
    /// Represents Dashed style.
    /// </summary>
    Dashed = 6,
    /// <summary>
    /// Represents Medium_dashed style.
    /// </summary>
    Medium_Dashed = 7,
    /// <summary>
    /// Represents Dash_dot style.
    /// </summary>
    Dash_Dot = 8,
    /// <summary>
    /// Represents Medium_dash_dot style.
    /// </summary>
    Medium_Dash_Dot = 9,
    /// <summary>
    /// Represents Dash_dot_dot style.
    /// </summary>
    Dash_Dot_Dot = 10
  }
  /// <summary>
  /// Represents shape arrow style type.
  /// </summary>
  public enum ExcelShapeArrowStyle
  {
    /// <summary>
    /// Represents no arrow.
    /// </summary>
    LineNoArrow = 0,
    /// <summary>
    /// Represents standard arrow.
    /// </summary>
    LineArrow = 1,
    /// <summary>
    /// Represents Stealth arrow.
    /// </summary>
    LineArrowStealth = 2,
    /// <summary>
    /// Represents Diamond arrow.
    /// </summary>
    LineArrowDiamond = 3,
    /// <summary>
    /// Represents Oval arrow.
    /// </summary>
    LineArrowOval = 4,
    /// <summary>
    /// Represents Open arrow.
    /// </summary>
    LineArrowOpen = 5,
  }
  /// <summary>
  /// Represents arrow head length.
  /// </summary>
  public enum ExcelShapeArrowLength
  {
    /// <summary>
    /// Represents short arrow head length.
    /// </summary>
    ArrowHeadShort = 0,
    /// <summary>
    /// Represents short arrow head length.
    /// </summary>
    ArrowHeadMedium = 1,
    /// <summary>
    /// Represents short arrow head length.
    /// </summary>
    ArrowHeadLong = 2
  }
  /// <summary>
  /// Represents arrow head width.
  /// </summary>
  public enum ExcelShapeArrowWidth
  {
    /// <summary>
    /// Represents short arrow head width.
    /// </summary>
    ArrowHeadNarrow = 0,
    /// <summary>
    /// Represents short arrow head width.
    /// </summary>
    ArrowHeadMedium = 1,
    /// <summary>
    /// Represents short arrow head width.
    /// </summary>
    ArrowHeadWide = 2
  }
  /// <summary>
  /// Represents shape arrow width length.
  /// </summary>
  public enum ExcelShapeArrowWidthLength
  {
    /// <summary>
    /// Represents ArrowNarrowShort type.
    /// </summary>
    ArrowNarrowShort = 1,
    /// <summary>
    /// Represents ArrowNarrowMedium type.
    /// </summary>
    ArrowNarrowMedium = 2,
    /// <summary>
    /// Represents ArrowNarrowLong type.
    /// </summary>
    ArrowNarrowLong = 3,
    /// <summary>
    /// Represents ArrowMediumShort type.
    /// </summary>
    ArrowMediumShort = 4,
    /// <summary>
    /// Represents ArrowMediumMedium type.
    /// </summary>
    ArrowMediumMedium = 5,
    /// <summary>
    /// Represents ArrowMediumLong type.
    /// </summary>
    ArrowMediumLong = 6,
    /// <summary>
    /// Represents ArrowWideShort type.
    /// </summary>
    ArrowWideShort = 7,
    /// <summary>
    /// Represents ArrowWideMedium type.
    /// </summary>
    ArrowWideMedium = 8,
    /// <summary>
    /// Represents ArrowWideLong type.
    /// </summary>
    ArrowWideLong = 9,
  }
  /// <summary>
  /// Represents shape fill type.
  /// </summary>
  public enum ExcelFillType
  {
    /// <summary>
    /// Solid color.
    /// </summary>
    SolidColor = 0,
    /// <summary>
    /// Represents pattern type.
    /// </summary>
    Pattern = 1,
    /// <summary>
    /// Represents texture type.
    /// </summary>
    Texture = 2,
    /// <summary>
    /// Represents picture type.
    /// </summary>
    Picture = 3,
    /// <summary>
    /// Represents unsupport gradient that can be created using Excel 2007,
    /// but not supported correctly by Excel 2003 structures.
    /// </summary>
    UnknownGradient = 4,
    /// <summary>
    /// Represents gradient type.
    /// </summary>
    Gradient = 7,
  }
  /// <summary>
  /// Represents excel shape shading style.
  /// </summary>
  public enum ExcelGradientStyle
  {
    /// <summary>
    /// Represents horizontal style.
    /// </summary>
    Horizontal = 0,
    /// <summary>
    /// Represents vertical style.
    /// </summary>
    Vertical = 1,
    /// <summary>
    /// Represents diagonal up style.
    /// </summary>
    Diagonl_Up = 2,
    /// <summary>
    /// Represents diagonal down style.
    /// </summary>
    Diagonl_Down = 3,
    /// <summary>
    /// Represents from corner style.
    /// </summary>
    From_Corner = 4,
    /// <summary>
    /// Represents from center style.
    /// </summary>
    From_Center = 5,
  }
  /// <summary>
  /// Represents shape shading variants.
  /// </summary>
  public enum ExcelGradientVariants
  {
    /// <summary>
    /// Represents first shading variants.
    /// </summary>
    ShadingVariants_1 = 1,
    /// <summary>
    /// Represents second shading variants.
    /// </summary>
    ShadingVariants_2 = 2,
    /// <summary>
    /// Represents third shading variants.
    /// </summary>
    ShadingVariants_3 = 3,
    /// <summary>
    /// Represents fourth shading variants.
    /// </summary>
    ShadingVariants_4 = 4
  }
  /// <summary>
  /// Represents gradient color type.
  /// </summary>
  public enum ExcelGradientColor
  {
    /// <summary>
    /// Represents one color gradient style.
    /// </summary>
    OneColor,
    /// <summary>
    /// Represents two color gradient style.
    /// </summary>
    TwoColor,
    /// <summary>
    /// Represents preset gradient style.
    /// </summary>
    Preset
  }
  /// <summary>
  /// Represents gradient texture.
  /// </summary>
  public enum ExcelTexture
  {
    /// <summary>
    /// Represents Newsprint texture type.
    /// </summary>
    Newsprint = 13,
    /// <summary>
    /// Represents Recycled Paper texture type.
    /// </summary>
    Recycled_Paper = 14,
    /// <summary>
    /// Represents Parchment texture type.
    /// </summary>
    Parchment = 15,
    /// <summary>
    /// Represents Stationery texture type.
    /// </summary>
    Stationery = 16,
    /// <summary>
    /// Represents Green Marble texture type.
    /// </summary>
    Green_Marble = 9,
    /// <summary>
    /// Represents White Marble texture type.
    /// </summary>
    White_Marble = 10,
    /// <summary>
    /// Represents Brown Marble texture type.
    /// </summary>
    Brown_Marble = 11,
    /// <summary>
    /// Represents Granite texture type.
    /// </summary>
    Granite = 12,
    /// <summary>
    /// Represents Blue Tissue Paper texture type.
    /// </summary>
    Blue_Tissue_Paper = 17,
    /// <summary>
    /// Represents Pink Tissue Paper texture type.
    /// </summary>
    Pink_Tissue_Paper = 18,
    /// <summary>
    /// Represents Purple Mesh texture type.
    /// </summary>
    Purple_Mesh = 19,
    /// <summary>
    /// Represents Bouquet texture type.
    /// </summary>
    Bouquet = 20,
    /// <summary>
    /// Represents Papyrus texture type.
    /// </summary>
    Papyrus = 1,
    /// <summary>
    /// Represents Canvas texture type.
    /// </summary>
    Canvas = 2,
    /// <summary>
    /// Represents Denim texture type.
    /// </summary>
    Denim = 3,
    /// <summary>
    /// Represents Woven Mat texture type.
    /// </summary>
    Woven_Mat = 4,
    /// <summary>
    /// Represents Water Droplets texture type.
    /// </summary>
    Water_Droplets = 5,
    /// <summary>
    /// Represents Paper Bag texture type.
    /// </summary>
    Paper_Bag = 6,
    /// <summary>
    /// Represents Fish Fossil texture type.
    /// </summary>
    Fish_Fossil = 7,
    /// <summary>
    /// Represents Sand texture type.
    /// </summary>
    Sand = 8,
    /// <summary>
    /// Represents Cork texture type.
    /// </summary>
    Cork = 21,
    /// <summary>
    /// Represents Walnut texture type.
    /// </summary>
    Walnut = 22,
    /// <summary>
    /// Represents Oak texture type.
    /// </summary>
    Oak = 23,
    /// <summary>
    /// Represents Medium Wood texture type.
    /// </summary>
    Medium_Wood = 24,
    /// <summary>
    /// Represents user defined texture type.
    /// </summary>
    User_Defined = -1,
  }
  /// <summary>
  /// Represents excel gradient pattern.
  /// </summary>
  public enum ExcelGradientPattern
  {
    /// <summary>
    /// Represents 5% gradient pattern
    /// </summary>
    Pat_5_Percent = 1,
    /// <summary>
    /// Represents 10% gradient pattern
    /// </summary>
    Pat_10_Percent = 2,
    /// <summary>
    /// Represents 20% gradient pattern
    /// </summary>
    Pat_20_Percent = 3,
    /// <summary>
    /// Represents 25% gradient pattern
    /// </summary>
    Pat_25_Percent = 4,
    /// <summary>
    /// Represents 30% gradient pattern
    /// </summary>
    Pat_30_Percent = 5,
    /// <summary>
    /// Represents 40% gradient pattern
    /// </summary>
    Pat_40_Percent = 6,
    /// <summary>
    /// Represents 50% gradient pattern
    /// </summary>
    Pat_50_Percent = 7,
    /// <summary>
    /// Represents 60% gradient pattern
    /// </summary>
    Pat_60_Percent = 8,
    /// <summary>
    /// Represents 70% gradient pattern
    /// </summary>
    Pat_70_Percent = 9,
    /// <summary>
    /// Represents 75% gradient pattern
    /// </summary>
    Pat_75_Percent = 10,
    /// <summary>
    /// Represents 80% gradient pattern
    /// </summary>
    Pat_80_Percent = 11,
    /// <summary>
    /// Represents 90% gradient pattern
    /// </summary>
    Pat_90_Percent = 12,
    /// <summary>
    /// Represents Dark Downward Diagonal gradient pattern
    /// </summary>
    Pat_Dark_Downward_Diagonal = 15,
    /// <summary>
    /// Represents Dark Horizontal gradient pattern
    /// </summary>
    Pat_Dark_Horizontal = 13,
    /// <summary>
    /// Represents Dark Upward Diagonal gradient pattern
    /// </summary>
    Pat_Dark_Upward_Diagonal = 16,
    /// <summary>
    /// Represents Dark Vertical gradient pattern
    /// </summary>
    Pat_Dark_Vertical = 14,
    /// <summary>
    /// Represents Dashed Downward Diagonal gradient pattern
    /// </summary>
    Pat_Dashed_Downward_Diagonal = 28,
    /// <summary>
    /// Represents Dashed Horizontal gradient pattern
    /// </summary>
    Pat_Dashed_Horizontal = 32,
    /// <summary>
    /// Represents Dashed Upward Diagonal gradient pattern
    /// </summary>
    Pat_Dashed_Upward_Diagonal = 27,
    /// <summary>
    /// Represents Dashed Vertical gradient pattern
    /// </summary>
    Pat_Dashed_Vertical = 31,
    /// <summary>
    /// Represents Diagonal Brick gradient pattern
    /// </summary>
    Pat_Diagonal_Brick = 40,
    /// <summary>
    /// Represents Divot gradient pattern
    /// </summary>
    Pat_Divot = 46,
    /// <summary>
    /// Represents Dotted Diamond gradient pattern
    /// </summary>
    Pat_Dotted_Diamond = 24,
    /// <summary>
    /// Represents Dotted Grid gradient pattern
    /// </summary>
    Pat_Dotted_Grid = 45,
    /// <summary>
    /// Represents Horizontal Brick gradient pattern
    /// </summary>
    Pat_Horizontal_Brick = 35,
    /// <summary>
    /// Represents Large Checker Board gradient pattern
    /// </summary>
    Pat_Large_Checker_Board = 36,
    /// <summary>
    /// Represents Large Confetti gradient pattern
    /// </summary>
    Pat_Large_Confetti = 33,
    /// <summary>
    /// Represents Large Grid gradient pattern
    /// </summary>
    Pat_Large_Grid = 34,
    /// <summary>
    /// Represents Light Downward Diagonal gradient pattern
    /// </summary>
    Pat_Light_Downward_Diagonal = 21,
    /// <summary>
    /// Represents Light Horizontal gradient pattern
    /// </summary>
    Pat_Light_Horizontal = 19,
    /// <summary>
    /// Represents Light Upward Diagonal gradient pattern
    /// </summary>
    Pat_Light_Upward_Diagonal = 22,
    /// <summary>
    /// Represents Light Vertical gradient pattern
    /// </summary>
    Pat_Light_Vertical = 20,
    /// <summary>
    /// Represents Mixed gradient pattern
    /// </summary>
    Pat_Mixed = -2,
    /// <summary>
    /// Represents Narrow Horizontal gradient pattern
    /// </summary>
    Pat_Narrow_Horizontal = 30,
    /// <summary>
    /// Represents Narrow Vertical gradient pattern
    /// </summary>
    Pat_Narrow_Vertical = 29,
    /// <summary>
    /// Represents Outlined Diamond gradient pattern
    /// </summary>
    Pat_Outlined_Diamond = 41,
    /// <summary>
    /// Represents Plaid gradient pattern
    /// </summary>
    Pat_Plaid = 42,
    /// <summary>
    /// Represents Shingle gradient pattern
    /// </summary>
    Pat_Shingle = 47,
    /// <summary>
    /// Represents Small Checker Board gradient pattern
    /// </summary>
    Pat_Small_Checker_Board = 17,
    /// <summary>
    /// Represents Small Confetti gradient pattern
    /// </summary>
    Pat_Small_Confetti = 37,
    /// <summary>
    /// Represents Small Grid gradient pattern
    /// </summary>
    Pat_Small_Grid = 23,
    /// <summary>
    /// Represents Solid Diamond gradient pattern
    /// </summary>
    Pat_Solid_Diamond = 39,
    /// <summary>
    /// Represents Sphere gradient pattern
    /// </summary>
    Pat_Sphere = 43,
    /// <summary>
    /// Represents Trellis gradient pattern
    /// </summary>
    Pat_Trellis = 18,
    /// <summary>
    /// Represents Wave gradient pattern
    /// </summary>
    Pat_Wave = 48,
    /// <summary>
    /// Represents Weave gradient pattern
    /// </summary>
    Pat_Weave = 44,
    /// <summary>
    /// Represents Wide Downward Diagonal gradient pattern
    /// </summary>
    Pat_Wide_Downward_Diagonal = 25,
    /// <summary>
    /// Represents Wide Upward Diagonal gradient pattern
    /// </summary>
    Pat_Wide_Upward_Diagonal = 26,
    /// <summary>
    /// Represents Zig Zag gradient pattern
    /// </summary>
    Pat_Zig_Zag = 38
  }
  /// <summary>
  /// Represents preset gradient type.
  /// </summary>
  public enum ExcelGradientPreset
  {
    /// <summary>
    /// Represents early sunset preset gradient type.
    /// </summary>
    Grad_Early_Sunset = 1,
    /// <summary>
    /// Represents late sunset preset gradient type.
    /// </summary>
    Grad_Late_Sunset = 2,
    /// <summary>
    /// Represents nightfall preset gradient type.
    /// </summary>
    Grad_Nightfall = 3,
    /// <summary>
    /// Represents daybreak preset gradient type.
    /// </summary>
    Grad_Daybreak = 4,
    /// <summary>
    /// Represents horizon preset gradient type.
    /// </summary>
    Grad_Horizon = 5,
    /// <summary>
    /// Represents desert preset gradient type.
    /// </summary>
    Grad_Desert = 6,
    /// <summary>
    /// Represents ocean preset gradient type.
    /// </summary>
    Grad_Ocean = 7,
    /// <summary>
    /// Represents calm water preset gradient type.
    /// </summary>
    Grad_Calm_Water = 8,
    /// <summary>
    /// Represents fire preset gradient type.
    /// </summary>
    Grad_Fire = 9,
    /// <summary>
    /// Represents fog preset gradient type.
    /// </summary>
    Grad_Fog = 10,
    /// <summary>
    /// Represents moss preset gradient type.
    /// </summary>
    Grad_Moss = 11,
    /// <summary>
    /// Represents peacock preset gradient type.
    /// </summary>
    Grad_Peacock = 12,
    /// <summary>
    /// Represents wheat preset gradient type.
    /// </summary>
    Grad_Wheat = 13,
    /// <summary>
    /// Represents parchment preset gradient type.
    /// </summary>
    Grad_Parchment = 14,
    /// <summary>
    /// Represents mahogany preset gradient type.
    /// </summary>
    Grad_Mahogany = 15,
    /// <summary>
    /// Represents rainbow preset gradient type.
    /// </summary>
    Grad_Rainbow = 16,
    /// <summary>
    /// Represents rainbowII preset gradient type.
    /// </summary>
    Grad_RainbowII = 17,
    /// <summary>
    /// Represents gold preset gradient type.
    /// </summary>
    Grad_Gold = 18,
    /// <summary>
    /// Represents goldII preset gradient type.
    /// </summary>
    Grad_GoldII = 19,
    /// <summary>
    /// Represents brass preset gradient type.
    /// </summary>
    Grad_Brass = 20,
    /// <summary>
    /// Represents chrome preset gradient type.
    /// </summary>
    Grad_Chrome = 21,
    /// <summary>
    /// Represents chromeII preset gradient type.
    /// </summary>
    Grad_ChromeII = 22,
    /// <summary>
    /// Represents silver preset gradient type.
    /// </summary>
    Grad_Silver = 23,
    /// <summary>
    /// Represents sapphire preset gradient type.
    /// </summary>
    Grad_Sapphire = 24,
  }
  /// <summary>
  /// Represents enum of chart tick mark values.
  /// </summary>
  public enum ExcelTickMark
  {
    /// <summary>
    /// Represents tick mark none.
    /// </summary>
    TickMark_None = 0,
    /// <summary>
    /// Represents tick mark inside.
    /// </summary>
    TickMark_Inside = 1,
    /// <summary>
    /// Represents tick mark outside.
    /// </summary>
    TickMark_Outside = 2,
    /// <summary>
    /// Represents tick mark cross.
    /// </summary>
    TickMark_Cross = 3,
  }
  /// <summary>
  /// Represents enum of chart tick label position values.
  /// </summary>
  public enum ExcelTickLabelPosition
  {
    /// <summary>
    /// Represents none label position.
    /// </summary>
    TickLabelPosition_None = 0,
    /// <summary>
    /// Represents low label position.
    /// </summary>
    TickLabelPosition_Low = 1,
    /// <summary>
    /// Represents high label position.
    /// </summary>
    TickLabelPosition_High = 2,
    /// <summary>
    /// Represents next to axis label position.
    /// </summary>
    TickLabelPosition_NextToAxis = 3,
  }
  /// <summary>
  /// Represents auto format values.
  /// </summary>
  public enum ExcelAutoFormat
  {
    /// <summary>
    /// Represents Simple auto format.
    /// </summary>
    Simple,
    /// <summary>
    /// Represents Classic_1 auto format.
    /// </summary>
    Classic_1,
    /// <summary>
    /// Represents Classic_2 auto format.
    /// </summary>
    Classic_2,
    /// <summary>
    /// Represents Classic_3 auto format.
    /// </summary>
    Classic_3,
    /// <summary>
    /// Represents Accounting_1 auto format.
    /// </summary>
    Accounting_1,
    /// <summary>
    /// Represents Accounting_2 auto format.
    /// </summary>
    Accounting_2,
    /// <summary>
    /// Represents Accounting_3 auto format.
    /// </summary>
    Accounting_3,
    /// <summary>
    /// Represents Accounting_4 auto format.
    /// </summary>
    Accounting_4,
    /// <summary>
    /// Represents Colorful_1 auto format.
    /// </summary>
    Colorful_1,
    /// <summary>
    /// Represents Colorful_2 auto format.
    /// </summary>
    Colorful_2,
    /// <summary>
    /// Represents Colorful_3 auto format.
    /// </summary>
    Colorful_3,
    /// <summary>
    /// Represents List_1 auto format.
    /// </summary>
    List_1,
    /// <summary>
    /// Represents List_2 auto format.
    /// </summary>
    List_2,
    /// <summary>
    /// Represents List_3 auto format.
    /// </summary>
    List_3,
    /// <summary>
    /// Represents Effect3D_1 auto format.
    /// </summary>
    Effect3D_1,
    /// <summary>
    /// Represents Effect3D_2 auto format.
    /// </summary>
    Effect3D_2,
    /// <summary>
    /// Represents None auto format.
    /// </summary>
    None,
  }
  /// <summary>
  /// Represents auto format options.
  /// </summary>
  [ Flags ]
  public enum ExcelAutoFormatOptions
  {
    /// <summary>
    /// Represents number auto format option.
    /// </summary>
    Number = 1,
    /// <summary>
    /// Represents border auto format option.
    /// </summary>
    Border = 2,
    /// <summary>
    /// Represents font auto format option.
    /// </summary>
    Font = 4,
    /// <summary>
    /// Represents patterns auto format option.
    /// </summary>
    Patterns = 8,
    /// <summary>
    /// Represents alignment auto format option.
    /// </summary>
    Alignment = 16,
    /// <summary>
    /// Represents width\height auto format option.
    /// </summary>
    Width_Height = 32,
    /// <summary>
    /// Represents none auto format option.
    /// </summary>
    None = 0,
    /// <summary>
    /// Represents all auto format option.
    /// </summary>
    All = 63
  }
  /// <summary>
  /// Error-bar type.
  /// </summary>
  public enum ExcelErrorBarType
  {
    /// <summary>
    /// Represents the Percentage error-bar source type.
    /// </summary>
    Percentage        = 1,
    /// <summary>
    /// Represents the FixedValue error-bar source type.
    /// </summary>
    Fixed              = 2,
    /// <summary>
    /// Represents the StandardDeviation error-bar source type.
    /// </summary>
    StandardDeviation = 3,
    /// <summary>
    /// Represents the Custom error-bar source type.
    /// </summary>
    Custom            = 4,
    /// <summary>
    /// Represents the StandardError error-bar source type.
    /// </summary>
    StandardError     = 5,
  }
  /// <summary>
  /// Represents error bar include values.
  /// </summary>
  public enum ExcelErrorBarInclude
  {
    /// <summary>
    /// Represents both error bar include.
    /// </summary>
    Both,
    /// <summary>
    /// Represents plus error bar include.
    /// </summary>
    Plus,
    /// <summary>
    /// Represents minus error bar include.
    /// </summary>
    Minus
  }
  /// <summary>
  /// Represents trend line values.
  /// </summary>
  public enum ExcelTrendLineType
  {
    /// <summary>
    /// Represents Exponential trend line type.
    /// </summary>
    Exponential = 1,
    /// <summary>
    /// Represents Linear trend line type.
    /// </summary>
    Linear = 5,
    /// <summary>
    /// Represents Logarithmic trend line type.
    /// </summary>
    Logarithmic = 2,
    /// <summary>
    /// Represents Moving average trend line type.
    /// </summary>
    Moving_Average = 4,
    /// <summary>
    /// Represents Polynomial trend line type.
    /// </summary>
    Polynomial = 0,
    /// <summary>
    /// Represents Power trend line type.
    /// </summary>
    Power = 3
  }
  /// <summary>
  /// Represents category type.
  /// </summary>
  public enum ExcelCategoryType
  {
    /// <summary>
    /// Represents Category category type.
    /// </summary>
    Category,
    /// <summary>
    /// Represents time category type.
    /// </summary>
    Time,
    /// <summary>
    /// Represents automatic category type.
    /// </summary>
    Automatic,
  }
  /// <summary>
  /// Represents axis text direction.
  /// </summary>
  public enum ExcelAxisTextDirection
  {
    /// <summary>
    /// Represents context text direction.
    /// </summary>
    Context,
    /// <summary>
    /// Represents Left-To-Right text direction.
    /// </summary>
    LeftToRight,
    /// <summary>
    /// Represents Right-To-Left text direction.
    /// </summary>
    RightToLeft
  }
  /// <summary>
  /// Represents sheet protection flags enums.
  /// </summary>
  [ Flags() ]
  public enum ExcelSheetProtection
  {
    /// <summary>
    /// Represents none flags.
    /// </summary>
    None                  = 0x0,
    /// <summary>
    /// True to protect shapes.
    /// </summary>
    Objects               = 0x1,
    /// <summary>
    /// True to protect scenarios.
    /// </summary>
    Scenarios              = 0x2,
    /// <summary>
    /// True allows the user to format any cell on a protected worksheet.
    /// </summary>
    FormattingCells        = 0x4,
    /// <summary>
    /// True allows the user to format any column on a protected worksheet.
    /// </summary>
    FormattingColumns      = 0x8,
    /// <summary>
    /// True allows the user to format any row on a protected.
    /// </summary>
    FormattingRows         = 0x10,
    /// <summary>
    /// True allows the user to insert columns on the protected worksheet.
    /// </summary>
    InsertingColumns       = 0x20,
    /// <summary>
    /// True allows the user to insert rows on the protected worksheet.
    /// </summary>
    InsertingRows          = 0x40,
    /// <summary>
    /// True allows the user to insert hyperlinks on the worksheet.
    /// </summary>
    InsertingHyperlinks    = 0x80,
    /// <summary>
    /// True allows the user to delete columns on the protected worksheet,
    /// where every cell in the column to be deleted is unlocked.
    /// </summary>
    DeletingColumns        = 0x100,
    /// <summary>
    /// True allows the user to delete rows on the protected worksheet,
    /// where every cell in the row to be deleted is unlocked.
    /// </summary>
    DeletingRows           = 0x200,
    /// <summary>
    /// True to protect locked cells.
    /// </summary>
    LockedCells            = 0x400,
    /// <summary>
    /// True allows the user to sort on the protected worksheet.
    /// </summary>
    Sorting                = 0x800,
    /// <summary>
    /// True allows the user to set filters on the protected worksheet.
    /// Users can change filter criteria but can not enable or disable an auto filter.
    /// </summary>
    Filtering             = 0x1000,
    /// <summary>
    /// True allows the user to use pivot table reports on the protected worksheet.
    /// </summary>
    UsingPivotTables      = 0x2000,
    /// <summary>
    /// True to protect the user interface, but not macros.
    /// </summary>
    UnLockedCells          = 0x4000,
    /// <summary>
    /// True to protect content.
    /// </summary>
    Content = 0x8000,
    /// <summary>
    /// Represents all flags
    /// </summary>
    All                    = 0xffff,
  }
  /// <summary>
  /// Represents excel chart uint to display.
  /// </summary>
  public enum ExcelChartDisplayUnit
  {
    /// <summary>
    /// Represents None display Unit
    /// </summary>
    None = 0,
    /// <summary>
    /// Represents Hundreds display Unit
    /// </summary>
    Hundreds = 1,
    /// <summary>
    /// Represents Thousands display Unit
    /// </summary>
    Thousands = 2,
    /// <summary>
    /// Represents TenThousands display Unit
    /// </summary>
    TenThousands = 3,
    /// <summary>
    /// Represents HundredThousands display Unit
    /// </summary>
    HundredThousands = 4,
    /// <summary>
    /// Represents Millions display Unit
    /// </summary>
    Millions = 5,
    /// <summary>
    /// Represents TenMillions display Unit
    /// </summary>
    TenMillions = 6,
    /// <summary>
    /// Represents HundredMillions display Unit
    /// </summary>
    HundredMillions = 7,
    /// <summary>
    /// Represents ThousandMillions display Unit
    /// </summary>
    ThousandMillions = 8,
    /// <summary>
    /// Represents MillionMillions display Unit
    /// </summary>
    MillionMillions = 9,
    /// <summary>
    /// Represents Custom display Unit
    /// </summary>
    Custom  = 0xffff
  }
  /// <summary>
  /// Represents chart base unit.
  /// </summary>
  public enum ExcelChartBaseUnit
  {
    /// <summary>
    /// Represents Day base unit.
    /// </summary>
    Day = 0,
    /// <summary>
    /// Represents Month base unit.
    /// </summary>
    Month = 1,
    /// <summary>
    /// Represents Year base unit.
    /// </summary>
    Year = 2,
  }
  /// <summary>
  /// Represents excel open type.
  /// </summary>
  public enum ExcelOpenType
  {
    /// <summary>
    /// Represents CSV open type. If data in the file exceeds worksheet limits, excepion will be thrown.
    /// </summary>
    CSV,
    /// <summary>
    /// Represents SpreadsheetML open type.
    /// </summary>
    SpreadsheetML,
    /// <summary>
    /// Represents BIFF open type.
    /// </summary>
    BIFF,
    /// <summary>
    /// Represents SpreadsheetML that is used in Excel 2007 (Office Open XML format).
    /// </summary>
    SpreadsheetML2007,
    /// <summary>
    /// Represents SpreadsheetML that is used in Excel 2010 (Office Open XML format).
    /// </summary>
    SpreadsheetML2010,
    /// <summary>
    /// Automatically indicates open type.
    /// </summary>
    Automatic
  }
  /// <summary>
  /// Represents Data label placement.
  /// </summary>
  public enum ExcelDataLabelPosition
  {
    /// <summary>
    /// Represents default position.
    /// </summary>
    Automatic = 0,
    /// <summary>
    /// Represents the Outside data label placement option.
    /// </summary>
    Outside = 1,
    /// <summary>
    /// Represents the Inside data label placement option.
    /// </summary>
    Inside  = 2,
    /// <summary>
    /// Represents the Center data label placement option.
    /// </summary>
    Center  = 3,
    /// <summary>
    /// Represents the OutsideBase data label placement option.
    /// </summary>
    OutsideBase    = 4,
    /// <summary>
    /// Represents the Above data label placement option.
    /// </summary>
    Above   = 5,
    /// <summary>
    /// Represents the Below data label placement option.
    /// </summary>
    Below   = 6,
    /// <summary>
    /// Represents the Left data label placement option.
    /// </summary>
    Left    = 7,
    /// <summary>
    /// Represents the Right data label placement option.
    /// </summary>
    Right   = 8,
    /// <summary>
    /// Represents the BestFit data label placement option.
    /// </summary>
    BestFit    = 9,
    /// <summary>
    /// Represents the Moved data label placement option.
    /// </summary>
    Moved   = 10,
  }
  /// <summary>
  /// Represents flags of excel ignore error indicator.
  /// </summary>
  [ Flags ]
  public enum ExcelIgnoreError
  {
    /// <summary>
    /// Represents None flag of excel ignore error indicator.
    /// </summary>
    None = 0,
    /// <summary>
    /// Represents EvaluateToError flag of excel ignore error indicator.
    /// </summary>
    EvaluateToError = 1,
    /// <summary>
    /// Represents EmptyCellReferences flag of excel ignore error indicator.
    /// </summary>
    EmptyCellReferences = 2,
    /// <summary>
    /// Represents NumberAsText flag of excel ignore error indicator.
    /// </summary>
    NumberAsText = 4,
    /// <summary>
    /// Represents OmittedCells flag of excel ignore error indicator.
    /// </summary>
    OmittedCells = 8,
    /// <summary>
    /// Represents InconsistentFormula flag of excel ignore error indicator.
    /// </summary>
    InconsistentFormula = 16,
    /// <summary>
    /// Represents TextDate flag of excel ignore error indicator.
    /// </summary>
    TextDate = 32,
    /// <summary>
    /// Represents UnlockedFormulaCells flag of excel ignore error indicator.
    /// </summary>
    UnlockedFormulaCells = 64,
    /// <summary>
    /// Represents All flag of excel ignore error indicator.
    /// </summary>
    All = 127
  }
  /// <summary>
  /// Represents encryption algorithm that will be used for encryption.
  /// </summary>
  public enum ExcelEncryptionType
  {
    /// <summary>
    /// No encryption.
    /// </summary>
    None,
    /// <summary>
    /// Standard encryption.
    /// </summary>
    Standard,
  }
  /// <summary>
  /// Represents possible excel versions.
  /// </summary>
  public enum ExcelVersion
  {
    /// <summary>
    /// Represents excel version 97-2003.
    /// </summary>
    Excel97to2003,
    /// <summary>
    /// Represents excel version 2007
    /// </summary>
    Excel2007,
    /// <summary>
    /// Represents excel version 2010
    /// </summary>
    Excel2010, 
    /// <summary>
    /// Represents excel version 2013
    /// </summary>
    Excel2013,

  }
  /// <summary>
  /// Defines the possible settings for vertical alignment of a run of text.
  /// This is used to get superscript or subscript text without altering the
  /// font size properties of the rest of the text run.
  /// </summary>
  public enum ExcelFontVertialAlignment
  {
    /// <summary>
    /// Returns the text in this run to the baseline, default,
    /// alignment, and returns it to the original font size.
    /// </summary>
    Baseline = 0,
    /// <summary>
    /// Specifies that this text should be superscript. Raises the text in this
    /// run above the baseline and changes it to a smaller size, if a smaller
    /// size is available.
    /// </summary>
    Superscript = 1,
    /// <summary>
    /// Specifies that this text should be subscript. Lowers the text in this
    /// run below the baseline and changes it to a smaller size, if a smaller
    /// size is available.
    /// </summary>
    Subscript = 2,
  }
  /// <summary>
  /// Specifies check state of the check box.
  /// </summary>
  public enum ExcelCheckState
  {
    /// <summary>
    /// Indicates that checkbox is unchecked.
    /// </summary>
    Unchecked,
    /// <summary>
    /// Indicates that checkbox is checked.
    /// </summary>
    Checked,
    /// <summary>
    /// Mixed state.
    /// </summary>
    Mixed,
  }
  /// <summary>
  /// Possible image types for image conversion.
  /// </summary>
  public enum ImageType
  {
    /// <summary>
    /// Bitmap image.
    /// </summary>
    Bitmap,
    /// <summary>
    /// Metafile image.
    /// </summary>
    Metafile,
  }
  /// <summary>
  /// Flags for expand/collapse settings.
  /// </summary>
  [ Flags ]
  public enum ExpandCollapseFlags
  {
    /// <summary>
    /// Default options.
    /// </summary>
    Default,
    /// <summary>
    /// Indicates whether subgroups must be included into operation.
    /// </summary>
    IncludeSubgroups,
    /// <summary>
    /// Indicates whether we have to expand parent group when expanding child (to make it visible).
    /// </summary>
    ExpandParent,
  }
  /// <summary>
  /// Specifies existing built-in styles for Excel 2007.
  /// </summary>
  public enum BuiltInStyles
  {
    /// <summary>
    /// Indicates Normal style.
    /// </summary>
    Normal = 0,
    //RowLevel_,
    //ColLevel_,
    /// <summary>
    /// Indicates Comma style.
    /// </summary>
    Comma = Normal + 3,
    /// <summary>
    /// Indicates Currency style.
    /// </summary>
    Currency,
    /// <summary>
    /// Indicates Percent style.
    /// </summary>
    Percent,
    /// <summary>
    /// Indicates Comma[0] style.
    /// </summary>
    Comma0,
    /// <summary>
    /// Indicates Currency[0] style.
    /// </summary>
    Currency0,
    ///// <summary>
    ///// Indicates Currency style.
    ///// </summary>
    //Hyperlink,
    ///// <summary>
    ///// Indicates Followed Hyperlink style.
    ///// </summary>
    //FollowedHyperlink,
    /// <summary>
    /// Indicates Note style.
    /// </summary>
    Note = Currency0 + 3,
    /// <summary>
    /// Indicates Warning Text style.
    /// </summary>
    WarningText,
    //Emphasis 1,
    //Emphasis 2,
    //,
    /// <summary>
    /// Indicates Title style.
    /// </summary>
    Title = WarningText + 4,
    /// <summary>
    /// Indicates Heading 1 style.
    /// </summary>
    Heading1,
    /// <summary>
    /// Indicates Heading 2 style.
    /// </summary>
    Heading2,
    /// <summary>
    /// Indicates Heading 3 style.
    /// </summary>
    Heading3,
    /// <summary>
    /// Indicates Heading 4 style.
    /// </summary>
    Heading4,
    /// <summary>
    /// Indicates Input style.
    /// </summary>
    Input,
    /// <summary>
    /// Indicates Output style.
    /// </summary>
    Output,
    /// <summary>
    /// Indicates Calculation style.
    /// </summary>
    Calculation,
    /// <summary>
    /// Indicates Check Cell style.
    /// </summary>
    CheckCell,
    /// <summary>
    /// Indicates Linked Cell style.
    /// </summary>
    LinkedCell,
    /// <summary>
    /// Indicates Total style.
    /// </summary>
    Total,
    /// <summary>
    /// Indicates Good style.
    /// </summary>
    Good,
    /// <summary>
    /// Indicates Bad style.
    /// </summary>
    Bad,
    /// <summary>
    /// Indicates Neutral style.
    /// </summary>
    Neutral,
    /// <summary>
    /// Indicates Accent1 style.
    /// </summary>
    Accent1,
    /// <summary>
    /// Indicates 20% - Accent1 style.
    /// </summary>
    Accent1_20,
    /// <summary>
    /// Indicates 40% - Accent1 style.
    /// </summary>
    Accent1_40,
    /// <summary>
    /// Indicates 60% - Accent1 style.
    /// </summary>
    Accent1_60,
    /// <summary>
    /// Indicates Accent2 style.
    /// </summary>
    Accent2,
    /// <summary>
    /// Indicates 20% - Accent2 style.
    /// </summary>
    Accent2_20,
    /// <summary>
    /// Indicates 40% - Accent2 style.
    /// </summary>
    Accent2_40,
    /// <summary>
    /// Indicates 60% - Accent2 style.
    /// </summary>
    Accent2_60,
    /// <summary>
    /// Indicates Accent3 style.
    /// </summary>
    Accent3,
    /// <summary>
    /// Indicates 20% - Accent3 style.
    /// </summary>
    Accent3_20,
    /// <summary>
    /// Indicates 40% - Accent3 style.
    /// </summary>
    Accent3_40,
    /// <summary>
    /// Indicates 60% - Accent3 style.
    /// </summary>
    Accent3_60,
    /// <summary>
    /// Indicates Accent4 style.
    /// </summary>
    Accent4,
    /// <summary>
    /// Indicates 20% - Accent4 style.
    /// </summary>
    Accent4_20,
    /// <summary>
    /// Indicates 40% - Accent4 style.
    /// </summary>
    Accent4_40,
    /// <summary>
    /// Indicates 60% - Accent4 style.
    /// </summary>
    Accent4_60,
    /// <summary>
    /// Indicates Accent5 style.
    /// </summary>
    Accent5,
    /// <summary>
    /// Indicates 20% - Accent5 style.
    /// </summary>
    Accent5_20,
    /// <summary>
    /// Indicates 40% - Accent5 style.
    /// </summary>
    Accent5_40,
    /// <summary>
    /// Indicates 60% - Accent5 style.
    /// </summary>
    Accent5_60,
    /// <summary>
    /// Indicates Accent6 style.
    /// </summary>
    Accent6,
    /// <summary>
    /// Indicates 20% - Accent6 style.
    /// </summary>
    Accent6_20,
    /// <summary>
    /// Indicates 40% - Accent6 style.
    /// </summary>
    Accent6_40,
    /// <summary>
    /// Indicates 60% - Accent6 style.
    /// </summary>
    Accent6_60,
    /// <summary>
    /// Indicates Explanatory Text style.
    /// </summary>
    ExplanatoryText,
  }
  /// <summary>
  /// Represents possible combo box type values..
  /// </summary>
  public enum ExcelComboType
  {
    /// <summary>
    /// Regular sheet dropdown control.
    /// </summary>
    Regular = 0,
    /// <summary>
    /// PivotTable page field dropdown.
    /// </summary>
    PivotTablePageField = 1,
    /// <summary>
    /// AutoFilter dropdown.
    /// </summary>
    AutoFilter = 3,
    /// <summary>
    /// AutoComplete dropdown.
    /// </summary>
    AutoComplete = 5,
    /// <summary>
    /// Data validation list dropdown.
    /// </summary>
    DataValidation = 6,
    /// <summary>
    /// PivotTable row or column field dropdown.
    /// </summary>
    PivotTableRowOrColumn = 7,
    /// <summary>
    /// Dropdown for the Total Row of a table.
    /// </summary>
    TableTotalRow = 9
  }
  /// <summary>
  /// Defines action that must be taken when meeting unknown variable during template markers processing.
  /// </summary>
  public enum UnknownVariableAction
  {
    /// <summary>
    /// Throws exception if no variable is defined.
    /// </summary>
    Exception,
    /// <summary>
    /// Skips processing variable and leaves it in the document.
    /// </summary>
    Skip,
    /// <summary>
    /// Replaces variable with empty string.
    /// </summary>
    ReplaceBlank,
  }
    /// <summary>
    /// shape border join type 
    /// (Supported in Excel 2007 and higher)
    /// </summary>
  public enum Excel2007BorderJoinType
  {
      /// <summary>
      /// Rounded edge
      /// </summary>
      Round,
      /// <summary>
      /// Beveled edge
      /// </summary>
      Bevel,
      /// <summary>
      /// Metter join 
      /// </summary>
      Mitter
  }
    /// <summary>
    /// MS Chart Font preservation Type
    /// (Internal use)
    /// </summary>
  public enum ChartParagraphType
  {
      /// <summary>
      /// Without the Font Tag
      /// </summary>
      Default,
      /// <summary>
      /// font with the default Property tag
      /// </summary>
      CustomDefault,
      /// <summary>
      /// font in ritch text tag
      /// </summary>
      Custom
  }
    /// <summary>
    /// Defines action for the unknown value type and numberformat in the 
    /// template marker variable.
    /// </summary>
  public enum VariableTypeAction
  {
      /// <summary>
      /// Detects the DataType of the marker variable.
      /// </summary>
      DetectDataType,
      /// <summary>
      /// Detects both the NumberFormat and DataType of the marker variable.
      /// </summary>
      DetectNumberFormat,
      /// <summary>
      /// Represents the None Action.
      /// </summary>
      None
  }
  /// <summary>
  /// Represents the sort orientation.
  /// </summary>
  public enum SortOrientation
  {
      /// <summary>
      /// Sorts the range from top to Bottom.
      /// </summary>
      TopToBottom,
      /// <summary>
      /// Sorts the range from Left to Right.
      /// </summary>
      LeftToRight
  }
  /// <summary>
  /// Represents the sort by in the range.
  /// </summary>
  public enum SortOn
  {
      /// <summary>
      /// Sort based on values in the cell.
      /// </summary>
      Values,
      /// <summary>
      /// Sort based on the cell back color.
      /// </summary>
      CellColor,
      /// <summary>
      /// Sort based on the font color.
      /// </summary>
      FontColor
  }
  /// <summary>
  /// Represents the algorithm to sort.
  /// </summary>
  public enum SortingAlgorithms
  {
      /// <summary>
      /// Represents the QuickSort Algorithm.
      /// </summary>
      QuickSort,
      /// <summary>
      /// Represents the HeapSort Algorithm.
      /// </summary>
      HeapSort,
      /// <summary>
      /// Represents the Merge Algorithm.
      /// </summary>
      MergeSort,
      /// <summary>
      /// Represents the InsertionSort Algorithm.
      /// </summary>
      InsertionSort,
  }
  /// <summary>
  /// Represents the sort order.
  /// </summary>
  public enum OrderBy
  {
      /// <summary>
      /// Represent the ascending sort.
      /// </summary>
      Ascending,
      /// <summary>
      /// Represent the descending sort.
      /// </summary>
      Descending,
      /// <summary>
      /// Represents the position value in the sort list
      /// Note:Applicable only to CellColor and FontColor
      /// </summary>
      OnTop,
      /// <summary>
      /// Represents the position of value in the sort list.
      /// Note: Applicable only to CellColor and FontColor type.
      /// </summary>
      OnBottom
  }
  /// <summary>
  /// Gets or Sets the way picture are displayed on the walls and Faces of 3D-chart.
  /// </summary>
  public enum ExcelChartPictureType
  {
      /// <summary>
      /// Represent the picture format is Stack.
      /// </summary>
      stack,
      /// <summary>
      /// Represent the picture format is stackScale.
      /// </summary>
      stackScale,
      /// <summary>
      /// Represent the picture format is stretch.
      /// </summary>
      stretch
  }
  /// <summary>
  /// Represents the extension property data.
  /// </summary>
  public enum CellPropertyExtensionType
  {
      /// <summary>
      /// Cell interior foreground color.
      /// </summary>
      ForeColor = 0x4,
      /// <summary>
      /// Cell interior background color.
      /// </summary>
      BackColor = 0x5,
      /// <summary>
      /// Cell interior gradient fill.
      /// </summary>
      GradientFill = 0x6,
      /// <summary>
      /// Top cell border color.
      /// </summary>
      TopBorderColor = 0x7,
      /// <summary>
      /// Bottom cell border color.
      /// </summary>
      BottomBorderColor = 0x8,
      /// <summary>
      /// Left cell border color.
      /// </summary>
      LeftBorderColor = 0x9,
      /// <summary>
      /// Right cell border color.
      /// </summary>
      RightBorderColor = 0xA,
      /// <summary>
      /// Diagonal cell border color.
      /// </summary>
      DiagonalCellBorder = 0xB,
      /// <summary>
      /// Cell text color.
      /// </summary>
      TextColor = 0xD,
      /// <summary>
      /// Font Scheme
      /// </summary>
      FontScheme = 0xE,
      /// <summary>
      /// The text indentation level.
      /// </summary>
      TextIndentationLevel = 0xF,
  }
  /// <summary>
  /// Font Scheme.
  /// </summary>
  public enum FontScheme
  {
      /// <summary>
      /// No font scheme.
      /// </summary>
      None=0x0,
      /// <summary>
      /// Major scheme.
      /// </summary>
      MajorScheme=0x1,
      /// <summary>
      /// Minor scheme.
      /// </summary>
      MinorScheme=0x2,
      /// <summary>
      /// Ninched state.
      /// </summary>
      Niched=0xFF,
  }
  /// <summary>
  /// Conditional format template.
  /// </summary>
  public enum ConditionalFormatTemplate
  {
      /// <summary>
      /// Cell value.
      /// </summary>
      CellValue = 0x0000,
      /// <summary>
      /// Formula.
      /// </summary>
      Formula = 0x0001,
      /// <summary>
      /// Color scale formatting.
      /// </summary>
      ColorScale = 0x0002,
      /// <summary>
      /// Data bar formatting.
      /// </summary>
      DataBar = 0x0003,
      /// <summary>
      /// Icon set formatting.
      /// </summary>
      IconSet = 0x0004,
      /// <summary>
      /// Filter.
      /// </summary>
      Filter = 0x0005,
      /// <summary>
      /// Unique values.
      /// </summary>
      UniqueValues = 0x0007,
      /// <summary>
      /// Contains text.
      /// </summary>
      ContainsText = 0x0008,
      /// <summary>
      /// Contains blanks.
      /// </summary>
      ContainsBlanks = 0x0009,
      /// <summary>
      /// Contains no blanks.
      /// </summary>
      ContainsNoBlanks = 0x000A,
      /// <summary>
      /// Contains errors.
      /// </summary>
      ContainsErrors = 0x000B,
      /// <summary>
      /// Contains no errors.
      /// </summary>
      ContainsNoErrors = 0x000C,
      /// <summary>
      /// Today.
      /// </summary>
      Today = 0x000F,
      /// <summary>
      /// Tomorrow.
      /// </summary>
      Tomorrow = 0x0010,
      /// <summary>
      /// Yesterday.
      /// </summary>
      Yesterday = 0x0011,
      /// <summary>
      /// Last 7 days.
      /// </summary>
      Last7Days = 0x0012,
      /// <summary>
      /// Last month.
      /// </summary>
      LastMonth = 0x0013,
      /// <summary>
      /// Next month.
      /// </summary>
      NextMonth = 0x0014,
      /// <summary>
      /// This week.
      /// </summary>
      ThisWeek = 0x0015,
      /// <summary>
      /// Next week.
      /// </summary>
      NextWeek = 0x0016,
      /// <summary>
      /// Last week.
      /// </summary>
      LastWeek = 0x0017,
      /// <summary>
      /// This month.
      /// </summary>
      ThisMonth = 0x0018,
      /// <summary>
      /// Above average.
      /// </summary>
      AboveAverage = 0x0019,
      /// <summary>
      /// Below Average.
      /// </summary>
      BelowAverage = 0x001A,
      /// <summary>
      /// Duplicate values.
      /// </summary>
      DuplicateValues = 0x001B,
      /// <summary>
      /// Above or equal to average.
      /// </summary>
      AboveOrEqualToAverage = 0x001D,
      /// <summary>
      /// Below or equal to average.
      /// </summary>
      BelowOrEqualToAverage = 0x001E,
  }
  /// <summary>
  /// Conditional format type of Text rule.
  /// </summary>
  public enum CFTextRuleType
  {
      /// <summary>
      /// Text contains.
      /// </summary>
      TextContains = 0x0000,
      /// <summary>
      /// Text does not contain.
      /// </summary>
      TextNotContains = 0x0001,
      /// <summary>
      /// Text begins with.
      /// </summary>
      TextBeginsWith = 0x0002,
      /// <summary>
      /// Text ends with.
      /// </summary>
      TextEndsWith = 0x0003,
  }
  /// <summary>
  /// Represents the excel series name level.
  /// </summary>
  public enum ExcelSeriesNameLevel
  {
      /// <summary>
      /// Series name from data range
      /// </summary>
      SeriesNameLevelAll,
      /// <summary>
      /// Series name is filteres
      /// </summary>
      SeriesNameLevelNone,
  }
  /// <summary>
  /// Represents the excel categoris name level.
  /// </summary>
  public enum ExcelCategoriesLabelLevel 
  {
      /// <summary>
      /// Categories name from data range
      /// </summary>
      CategoriesLabelLevelAll,
      /// <summary>
      /// Categories name is filtered
      /// </summary>
      CategoriesLabelLevelNone,
  }
  /// <summary>
  /// ConsolidationFunctions 
  /// </summary>
  public enum ConsolidationFunction
  {
      /// <summary>
      ///  Sum
      /// </summary>
      Sum = 9,
      /// <summary>
      /// Count
      /// </summary>
      Count = 3,
      /// <summary>
      /// Average
      /// </summary>
      Average = 1,
      /// <summary>
      /// Maximum
      /// </summary>
      Max = 4,
      /// <summary>
      /// Minimum
      /// </summary>
      Min = 5,
      /// <summary>
      /// Multiply
      /// </summary>
      Product = 6,
      /// <summary>
      /// Count numerical values only
      /// </summary>
      CountNums = 2,
      /// <summary>
      /// Standard deviation, based on a sample
      /// </summary>
      StdDev = 7,
      /// <summary>
      /// Standard deviation, based on the whole population
      /// </summary>
      StdDevp = 8,
      /// <summary>
      /// Variation, based on a sample
      /// </summary>
      Var = 10,
      /// <summary>
      /// Variation, based on the whole population.
      /// </summary>
      Varp = 11
  }
  /// <summary>
  /// It's Define the connection type
  /// </summary>
  public enum ExcelConnectionsType
  {
      /// <summary>
      /// connection type is ODBC
      /// </summary>
      ConnectionTypeODBC=1,
      /// <summary>
      /// connection type is OLEDB
      /// </summary>
      ConnectionTypeOLEDB=5,
      /// <summary>
      /// connection type is Text file
      /// </summary>
      ConnectionTypeTEXT=6,
      /// <summary>
      /// connection type web file
      /// </summary>
      ConnectionTypeWEB=4
  }
    public enum ExcelCommandType
    {
        Default,
        Sql=2,
        Table=3,
        List=5
    }
    public enum ExcelListObjectSourceType
    {
        SrcQuery = 2
    }
    /// <summary>
    /// It's represent the table type
    /// </summary>
    public enum ExcelTableType
    {
        queryTable,
        worksheet,
        xml,
    }
    /// <summary>
    /// It's represent the cretential for connect the server
    /// </summary>
    public enum ExcelCredentialsMethod
    {
        integrated,
        none,
        stored
    }
    public enum AutoShapeType
    {
        Unknown = -1,

        //Connectors
        Line = 224,
        ElbowConnector = 227,
        CurvedConnector = 228,
        BentConnector2 = 229,
        StraightConnector = 225,
        BentConnector4 = 230,
        BentConnector5 = 231,
        CurvedConnector2 = 232,
        CurvedConnector4 = 233,
        CurvedConnector5 = 234,

        //Rectangles
        Rectangle = 1,
        RoundedRectangle = 5,
        SnipSingleCornerRectangle = 155,
        SnipSameSideCornerRectangle = 156,
        SnipDiagonalCornerRectangle = 157,
        SnipAndRoundSingleCornerRectangle = 154,
        RoundSingleCornerRectangle = 151,
        RoundSameSideCornerRectangle = 152,
        RoundDiagonalCornerRectangle = 153,

        //Basic Shapes
        Oval = 9,
        IsoscelesTriangle = 7,
        RightTriangle = 8,
        Parallelogram = 2,
        Trapezoid = 3,
        Diamond = 4,
        RegularPentagon = 12,
        Hexagon = 10,
        Heptagon = 145,
        Octagon = 6,
        Decagon = 144,
        Dodecagon = 146,
        Pie = 142,
        Chord = 161,
        Teardrop = 160,
        Frame = 158,
        HalfFrame = 159,
        L_Shape = 162,
        DiagonalStripe = 141,
        Cross = 11,
        Plaque = 28,
        Can = 13,
        Cube = 14,
        Bevel = 15,
        Donut = 18,
        NoSymbol = 19,
        BlockArc = 20,
        FoldedCorner = 16,
        SmileyFace = 17,
        Heart = 21,
        LightningBolt = 22,
        Sun = 23,
        Moon = 24,
        Cloud = 179,
        Arc = 25,
        DoubleBracket = 26,
        DoubleBrace = 27,
        LeftBracket = 29,
        RightBracket = 30,
        LeftBrace = 31,
        RightBrace = 32,


        //BlockArrows
        RightArrow = 33,
        LeftArrow = 34,
        UpArrow = 35,
        DownArrow = 36,
        LeftRightArrow = 37,
        UpDownArrow = 38,
        QuadArrow = 39,
        LeftRightUpArrow = 40,
        BentArrow = 41,
        UTurnArrow = 42,
        LeftUpArrow = 43,
        BentUpArrow = 44,
        CurvedRightArrow = 45,
        CurvedLeftArrow = 46,
        CurvedUpArrow = 47,
        CurvedDownArrow = 48,
        StripedRightArrow = 49,
        NotchedRightArrow = 50,
        Pentagon = 51,
        Chevron = 52,
        RightArrowCallout = 53,
        DownArrowCallout = 56,
        LeftArrowCallout = 54,
        UpArrowCallout = 55,
        LeftRightArrowCallout = 57,
        UpDownArrowCallout = 58,
        QuadArrowCallout = 59,
        CircularArrow = 60,

        //Equations
        MathPlus = 163,
        MathMinus = 164,
        MathMultiply = 165,
        MathDivision = 166,
        MathEqual = 167,
        MathNotEqual = 168,

        //FlowCharts
        FlowChartProcess = 61,
        FlowChartAlternateProcess = 62,
        FlowChartDecision = 63,
        FlowChartData = 64,
        FlowChartPredefinedProcess = 65,
        FlowChartInternalStorage = 66,
        FlowChartDocument = 67,
        FlowChartMultiDocument = 68,
        FlowChartTerminator = 69,
        FlowChartPreparation = 70,
        FlowChartManualInput = 71,
        FlowChartManualOperation = 72,
        FlowChartConnector = 73,
        FlowChartOffPageConnector = 74,
        FlowChartCard = 75,
        FlowChartPunchedTape = 76,
        FlowChartSummingJunction = 77,
        FlowChartOr = 78,
        FlowChartCollate = 79,
        FlowChartSort = 80,
        FlowChartExtract = 81,
        FlowChartMerge = 82,
        FlowChartStoredData = 83,
        FlowChartDelay = 84,
        FlowChartSequentialAccessStorage = 85,
        FlowChartMagneticDisk = 86,
        FlowChartDirectAccessStorage = 87,
        FlowChartDisplay = 88,

        //StarsAndBanner
        Explosion1 = 89,
        Explosion2 = 90,
        Star4Point = 91,
        Star5Point = 92,
        Star6Point = 147,
        Star7Point = 148,
        Star8Point = 93,
        Star10Point = 149,
        Star12Point = 150,
        Star16Point = 94,
        Star24Point = 95,
        Star32Point = 96,
        UpRibbon = 97,
        DownRibbon = 98,
        CurvedUpRibbon = 99,
        CurvedDownRibbon = 100,
        VerticalScroll = 101,
        HorizontalScroll = 102,
        Wave = 103,
        DoubleWave = 104,

        //CallOuts
        RectangularCallout = 105,
        RoundedRectangularCallout = 106,
        OvalCallout = 107,
        CloudCallout = 108,
        LineCallout1 = 109,
        LineCallout2 = 111,
        LineCallout3 = 112,
        LineCallout1AccentBar = 114,
        LineCallout2AccentBar = 115,
        LineCallout3AccentBar = 116,
        LineCallout1NoBorder = 113,
        LineCallout2NoBorder = 119,
        LineCallout3NoBorder = 120,
        LineCallout1BorderAndAccentBar = 122,
        LineCallout2BorderAndAccentBar = 123,
        LineCallout3BorderAndAccentBar = 124,

    }
    #region ImageQuality
    /// <summary>
    /// It represents the chart image Scaling.
    /// </summary>
    public enum ScalingMode
    {
        /// <summary>
        /// Save the image with normal image quality.
        /// </summary>
        Normal = 96,
        /// <summary>
        /// Save the image with best image quality which results large image size.
        /// </summary>
        Best = 128
    }
    #endregion
  }

