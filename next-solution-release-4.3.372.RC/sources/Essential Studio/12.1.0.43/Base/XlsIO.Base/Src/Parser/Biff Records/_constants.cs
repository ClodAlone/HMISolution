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

using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Enum that defines constants for all known Biff records.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum TBIFFRecord
  {
    /// <summary>
    /// Represents the Array Biff record.
    /// </summary>
    Array                       = 0x221,
    /// <summary>
    /// Represents the AutoFilter Biff record.
    /// </summary>
    AutoFilter                  = 0x9E,
    /// <summary>
    /// Represents the AutoFilterInfo Biff record.
    /// </summary>
    AutoFilterInfo              = 0x9D,
    /// <summary>
    /// Represents the BOF Biff record.
    /// </summary>
    BOF                         = 0x809,
    /// <summary>
    /// Represents the BOF2 Biff record.
    /// </summary>
    BOF2                        = 0x409,
    /// <summary>
    /// Represents the Backup Biff record.
    /// </summary>
    Backup                      = 0x40,
    /// <summary>
    /// Represents the Begin Biff record.
    /// </summary>
    Begin                       = 0x1033,
    /// <summary>
    /// It stores the background bitmap of a worksheet.
    /// </summary>
    Bitmap                      = 0x00E9,
    /// <summary>
    /// Represents the Blank Biff record.
    /// </summary>
    Blank                       = 0x201,
    /// <summary>
    /// Represents the BookBool Biff record.
    /// </summary>
    BookBool                    = 0xDA,
    /// <summary>
    /// Represents the BoolErr Biff record.
    /// </summary>
    BoolErr                     = 0x205,
    /// <summary>
    /// Represents the BottomMargin Biff record.
    /// </summary>
    BottomMargin                = 0x29,
    /// <summary>
    /// Represents the BoundSheet Biff record.
    /// </summary>
    BoundSheet                  = 0x85,
    /// <summary>
    /// Represents the CF Biff record.
    /// </summary>
    CF                          = 0x1B1,
    /// <summary>
    /// Represents the CF12 Biff record.
    /// </summary>
    CF12 = 0x87A,
    /// <summary>
    /// Represents the CFEx Biff record.
    /// </summary>
    CFEx = 0x87B,
    /// <summary>
    /// Represents the CRN Biff record.
    /// </summary>
    CRN                         = 0x5A,
    /// <summary>
    /// Represents the CalCount Biff record.
    /// </summary>
    CalCount                    = 0xC,
    /// <summary>
    /// Represents the CalcMode Biff record.
    /// </summary>
    CalcMode                    = 0xD,
    /// <summary>
    /// Represents the CodeName Biff record.
    /// </summary>
    CodeName                    = 0x01BA,
    /// <summary>
    /// Represents the Codepage Biff record.
    /// </summary>
    Codepage                    = 0x42,
    /// <summary>
    /// Represents the ColumnInfo Biff record.
    /// </summary>
    ColumnInfo                  = 0x7D,
    /// <summary>
    /// Represents the CondFMT Biff record.
    /// </summary>
    CondFMT                     = 0x1B0,
    /// <summary>
    /// Represents the CondFMT12 Biff record.
    /// </summary>
    CondFMT12 = 0x879,
    /// <summary>
    /// Represents the Continue Biff record.
    /// </summary>
    Continue                    = 0x003C,
    /// <summary>
    /// Represents the Continue Frt record.
    /// </summary>
    ContinueFrt                 = 0x812,
    /// <summary>
    /// Represents the Country Biff record.
    /// </summary>
    Country                     = 0x8C,
    /// <summary>
    /// Represents custom property record.
    /// </summary>
    CustomProperty              = 0x418,
    /// <summary>
    /// Represents the DBCell Biff record.
    /// </summary>
    DBCell                      = 0xD7,
    /// <summary>
    /// Represents the DCON Biff record.
    /// </summary>
    DCON                        = 0x50,
    /// <summary>
    /// Represents the DCONBIN Biff record.
    /// </summary>
    DCONBIN                     = 0x1B5,
    /// <summary>
    /// Represents the DCONNAME Biff record.
    /// </summary>
    DCONNAME                    = 0x52,
    /// <summary>
    /// Represents the DCONRef Biff record.
    /// </summary>
    DCONRef                     = 0x51,
    /// <summary>
    /// Represents the DSF Biff record.
    /// </summary>
    DSF                         = 0x161,
    /// <summary>
    /// Represents the DV Biff record.
    /// </summary>
    DV                          = 0x1BE,
    /// <summary>
    /// Represents the DVal Biff record.
    /// </summary>
    DVal                        = 0x1B2,
    /// <summary>
    /// Represents the DateWindow1904 Biff record.
    /// </summary>
    DateWindow1904              = 0x22,
    /// <summary>
    /// Represents the DefaultColWidth Biff record.
    /// </summary>
    DefaultColWidth             = 0x55,
    /// <summary>
    /// Represents the DefaultRowHeight Biff record.
    /// </summary>
    DefaultRowHeight            = 0x225,
    /// <summary>
    /// Represents the Delta Biff record.
    /// </summary>
    Delta                       = 0x10,
    /// <summary>
    /// Represents the Dimensions Biff record.
    /// </summary>
    Dimensions                  = 0x200,
    /// <summary>
    /// Represents the EOF Biff record.
    /// </summary>
    EOF                         = 0x0A,
    /// <summary>
    /// Represents the End Biff record.
    /// </summary>
    End                         = 0x1034,
    /// <summary>
    /// Represents the ExtSST Biff record.
    /// </summary>
    ExtSST                      = 0xFF,
    /// <summary>
    /// Represents the ExtSSTInfoSub Biff record.
    /// </summary>
    ExtSSTInfoSub               = 0xFFF,
    /// <summary>
    /// Represents the ExtendedFormat Biff record.
    /// </summary>
    ExtendedFormat              = 0xE0,
    /// <summary>
    /// Represents the ExtendedFormatCRC Biff record.
    /// </summary>
    ExtendedFormatCRC = 0x87C,
    /// <summary>
    /// Represents the Extension of ExtendedFormat Biff record.
    /// </summary>
    ExtendedXFRecord = 0x87D,
    /// <summary>
    /// Represents the ExternCount Biff record.
    /// </summary>
    ExternCount                 = 0x16,
    /// <summary>
    /// Represents the ExternName Biff record.
    /// </summary>
    ExternName                  = 0x23,
    /// <summary>
    /// Represents the ExternSheet Biff record.
    /// </summary>
    ExternSheet                 = 0x17,
    /// <summary>
    /// Represents the FilePass Biff record.
    /// </summary>
    FilePass                    = 0x2F,
    /// <summary>
    /// Represents the FileSharing Biff record.
    /// </summary>
    FileSharing                 = 0x5B,
    /// <summary>
    /// Represents the FilterMode Biff record.
    /// </summary>
    FilterMode                  = 0x9B,
    /// <summary>
    /// Represents the FnGroupCount Biff record.
    /// </summary>
    FnGroupCount                = 0x9C,
    /// <summary>
    /// Represents the Font Biff record.
    /// </summary>
    Font                        = 0x31,
    /// <summary>
    /// Represents the Footer Biff record.
    /// </summary>
    Footer                      = 0x15,
    /// <summary>
    /// Represents the Format Biff record.
    /// </summary>
    Format                      = 0x41E,
    /// <summary>
    /// Represents the Formula Biff record.
    /// </summary>
    Formula                     = 0x06,
    /// <summary>
    /// Represents the Gridset Biff record.
    /// </summary>
    Gridset                     = 0x82,
    /// <summary>
    /// Represents the Guts Biff record.
    /// </summary>
    Guts                        = 0x80,
    /// <summary>
    /// Represents the HasBasic Biff record.
    /// </summary>
    HasBasic                    = 0xD3,
    /// <summary>
    /// Represents the HCenter Biff record.
    /// </summary>
    HCenter                     = 0x83,
    /// <summary>
    /// Represents the HLink Biff record.
    /// </summary>
    HLink                       = 0x1B8,
    /// <summary>
    /// Represents the Header Biff record.
    /// </summary>
    Header                      = 0x14,
    /// <summary>
    /// Represents image in header or footer.
    /// </summary>
    HeaderFooterImage           = 0x866, // 2150
    /// <summary>
    /// This record specifies the even page header and footer text, and the first page
    /// header and footer text of the current sheet.
    /// </summary>
    HeaderFooter = 0x89C, // 2204
    /// <summary>
    /// Represents the HideObj Biff record.
    /// </summary>
    HideObj                     = 0x8D,
    /// <summary>
    /// Represents the HorizontalPageBreaks Biff record.
    /// </summary>
    HorizontalPageBreaks        = 0x1B,
    /// <summary>
    /// Represents ImageData biff record.
    /// </summary>
    ImageData                   = 0X7F,
    /// <summary>
    /// Represents the Index Biff record.
    /// </summary>
    Index                       = 0x20B,
    /// <summary>
    /// Represents the InterfaceEnd Biff record.
    /// </summary>
    InterfaceEnd                = 0xE2,
    /// <summary>
    /// Represents the InterfaceHdr Biff record.
    /// </summary>
    InterfaceHdr                = 0xE1,
    /// <summary>
    /// Represents the Iteration Biff record.
    /// </summary>
    Iteration                   = 0x11,
    /// <summary>
    /// Represents the Label Biff record.
    /// </summary>
    Label                       = 0x204,
    /// <summary>
    /// Represents the LabelRanges Biff record.
    /// </summary>
    LabelRanges                 = 0x15F,
    /// <summary>
    /// Represents the LabelSST Biff record.
    /// </summary>
    LabelSST                    = 0xFD,
    /// <summary>
    /// Represents the LeftMargin Biff record.
    /// </summary>
    LeftMargin                  = 0x26,
    /// <summary>
    /// Represents the MMS Biff record.
    /// </summary>
    MMS                         = 0xC1,
    /// <summary>
    /// Represents the MergeCells Biff record.
    /// </summary>
    MergeCells                  = 0xE5,
    /// <summary>
    /// Represents the MSODrawing Biff record.
    /// </summary>
    MSODrawing                  = 0xEC,
    /// <summary>
    /// Represents the MSODrawingGroup Biff record.
    /// </summary>
    MSODrawingGroup             = 0xEB,
    /// <summary>
    /// Represents the MulBlank Biff record.
    /// </summary>
    MulBlank                    = 0xBE,
    /// <summary>
    /// Represents the MulRK Biff record.
    /// </summary>
    MulRK                       = 0xBD,
    /// <summary>
    /// Represents the Name Biff record.
    /// </summary>
    Name                        = 0x18,
    /// <summary>
    /// Represents the Note Biff record.
    /// </summary>
    Note                        = 0x1C,
    /// <summary>
    /// Represents the Number Biff record.
    /// </summary>
    Number                      = 0x203,
    /// <summary>
    /// Represents the OBJ Biff record.
    /// </summary>
    OBJ                         = 0x5D,
    /// <summary>
    /// Represents the ObjectProtect Biff record.
    /// </summary>
    ObjectProtect               = 0x63,
    /// <summary>
    /// Represents the OleSize Biff record.
    /// </summary>
    OleSize                     = 0xDE,
    /// <summary>
    /// Represents the Palette Biff record.
    /// </summary>
    Palette                     = 0x92,
    /// <summary>
    /// Represents the Pane Biff record.
    /// </summary>
    Pane                        = 0x41,
    /// <summary>
    /// Represents the Password Biff record.
    /// </summary>
    Password                    = 0x13,
    /// <summary>
    /// Represents the PasswordRev4 Biff record.
    /// </summary>
    PasswordRev4                = 0x1BC,
    /// <summary>
    /// Represents the Precision Biff record.
    /// </summary>
    Precision                   = 0xE,
    /// <summary>
    /// Represents the PrintedChartSize Biff record.
    /// </summary>
    PrintedChartSize            = 0x33,
    /// <summary>
    /// THis records saves settings and printer driver information.
    /// </summary>
    PrinterSettings             = 0x4D,
    /// <summary>
    /// Represents the PrintGridlines Biff record.
    /// </summary>
    PrintGridlines              = 0x2B,
    /// <summary>
    /// Represents the PrintHeaders Biff record.
    /// </summary>
    PrintHeaders                = 0x2A,
    /// <summary>
    /// Represents the PrintSetup Biff record.
    /// </summary>
    PrintSetup                  = 0xA1,
    /// <summary>
    /// Represents the Protect Biff record.
    /// </summary>
    Protect                     = 0x12,
    /// <summary>
    /// Represents the ProtectionRev4 Biff record.
    /// </summary>
    ProtectionRev4              = 0x1AF,
    /// <summary>
    /// Represents the QuickTip Biff record.
    /// </summary>
    QuickTip                    = 0x800,
    /// <summary>
    /// Represents the RefMode Biff record.
    /// </summary>
    RefMode                     = 0xF,
    /// <summary>
    /// Represents the RefreshAll Biff record.
    /// </summary>
    RefreshAll                  = 0x1B7,
    /// <summary>
    /// Represents the RightMargin Biff record.
    /// </summary>
    RightMargin                 = 0x27,
    /// <summary>
    /// Represents the RK Biff record.
    /// </summary>
    RK                          = 0x27E,
    /// <summary>
    /// Represents the Row Biff record.
    /// </summary>
    Row                         = 0x208,
    /// <summary>
    /// Represents the RString Biff record.
    /// </summary>
    RString                     = 0xD6,
    /// <summary>
    /// Represents the SaveRecalc Biff record.
    /// </summary>
    SaveRecalc                  = 0x5F,
    /// <summary>
    /// Represents the ScenProtect Biff record.
    /// </summary>
    ScenProtect                 = 0xDD,
    /// <summary>
    /// Represents the Selection Biff record.
    /// </summary>
    Selection                   = 0x1D,
    /// <summary>
    /// Represents the default column width for all sheet columns.
    /// </summary>
    DxGCol                      = 0x99,
    /// <summary>
    /// Represents the Setup Biff record.
    /// </summary>
    Setup                       = 0xA1,
    /// <summary>
    /// Represents the beginning of a collection of records
    /// </summary>
    StartBlock                  = 0x852,
    /// <summary>
    /// Represents the shape formatting properties for chart elements
    /// </summary>
    ShapePropsStream            = 0x8A4,
    /// <summary>
    /// Represents the end of a collection of records
    /// </summary>
    EndBlock                    = 0x853,
    /// <summary>
    /// Represents the SharedFormula Biff record.
    /// </summary>
    SharedFormula               = 0xBC,
    /// <summary>
    /// Represents the SharedFormula2 Biff record.
    /// </summary>
    SharedFormula2              = 0x04BC,
    /// <summary>
    /// This record stores the colour of the tab below the sheet containing the sheet name.
    /// </summary>
    SheetLayout                 = 0x0862,
    /// <summary>
    /// Represents the Sort Biff record.
    /// </summary>
    Sort                        = 0x90,
    /// <summary>
    /// Represents the SST Biff record.
    /// </summary>
    SST                         = 0xFC,
    /// <summary>
    /// Represents the String Biff record.
    /// </summary>
    String                      = 0x207,
    /// <summary>
    /// Represents the Style Biff record.
    /// </summary>
    Style                       = 0x293,
    /// <summary>
    /// Represents the SupBook Biff record.
    /// </summary>
    SupBook                     = 0x1AE,
    /// <summary>
    /// Represents the TabId Biff record.
    /// </summary>
    TabId                       = 0x13D,
    /// <summary>
    /// Represents the Table Biff record.
    /// </summary>
    Table                       = 0x236,
    /// <summary>
    /// Represents the Template Biff record.
    /// </summary>
    Template                    = 0x60,
    /// <summary>
    /// Represents the TextObject Biff record.
    /// </summary>
    TextObject                  = 0x1B6,
    /// <summary>
    /// Represents the TopMargin Biff record.
    /// </summary>
    TopMargin                   = 0x28,
    /// <summary>
    /// Represents the UseSelFS Biff record.
    /// </summary>
    UseSelFS                    = 0x160,
    /// <summary>
    /// Represents the VCenter Biff record.
    /// </summary>
    VCenter                     = 0x84,
    /// <summary>
    /// Represents the VerticalPageBreaks Biff record.
    /// </summary>
    VerticalPageBreaks          = 0x1A,
    /// <summary>
    /// Represents the WSBool Biff record.
    /// </summary>
    WSBool                      = 0x81,
    /// <summary>
    /// Represents the WindowOne Biff record.
    /// </summary>
    WindowOne                   = 0x3D,
    /// <summary>
    /// Represents the WindowProtect Biff record.
    /// </summary>
    WindowProtect               = 0x19,
    /// <summary>
    /// Represents the WindowTwo Biff record.
    /// </summary>
    WindowTwo                   = 0x23E,
    /// <summary>
    /// Represents the WindowZoom Biff record.
    /// </summary>
    WindowZoom                  = 0xA0,
    /// <summary>
    /// Represents the WriteAccess Biff record.
    /// </summary>
    WriteAccess                 = 0x5C,
    /// <summary>
    /// Represents the WriteProtection record.
    /// </summary>
    WriteProtection             = 0x86,
    /// <summary>
    /// Represents the XCT Biff record.
    /// </summary>
    XCT                         = 0x59,
    /// <summary>
    /// Represents the Unknown Biff record.
    /// </summary>
    Unknown                     = 0,
    /// <summary>
    /// Represents the UnkBegin Biff record.
    /// </summary>
    UnkBegin                    = 0x1c0,
    /// <summary>
    /// Represents the UnkEnd Biff record.
    /// </summary>
    RecalcId                    = 0x1c1,
    /// <summary>
    /// Represents the UnkMarker Biff record.
    /// </summary>
    UnkMarker                   = 0xEF,
    /// <summary>
    /// Represents the UnkMacrosDisable Biff record.
    /// </summary>
    UnkMacrosDisable            = 0x1bd,
    /// <summary>
    /// This record contains workbook-specific information.
    /// </summary>
    BookExt                     = 0x863,

    /// <summary>
    /// Represents the ChartDataLabels Biff record.
    /// </summary>
    ChartDataLabels             = 0x86B,
    /// <summary>
    /// Represents the ChartChart Biff record.
    /// </summary>
    ChartChart                  = 0x1002,
    /// <summary>
    /// Represents the ChartSeries Biff record.
    /// </summary>
    ChartSeries                 = 0x1003,
    /// <summary>
    /// Represents the ChartDataFormat Biff record.
    /// </summary>
    ChartDataFormat             = 0x1006,
    /// <summary>
    /// Represents the ChartLineFormat Biff record.
    /// </summary>
    ChartLineFormat             = 0x1007,
    /// <summary>
    /// Represents the ChartMarkerFormat Biff record.
    /// </summary>
    ChartMarkerFormat           = 0x1009,
    /// <summary>
    /// Represents the ChartAreaFormat Biff record.
    /// </summary>
    ChartAreaFormat             = 0x100A,
    /// <summary>
    /// Represents the ChartPieFormat Biff record.
    /// </summary>
    ChartPieFormat              = 0x100B,
    /// <summary>
    /// Represents the ChartAttachedLabel Biff record.
    /// </summary>
    ChartAttachedLabel          = 0x100C,
    /// <summary>
    /// Represents the ChartAttachedLabelLayout Biff record.
    /// </summary>
    ChartAttachedLabelLayout    = 0x089D,
    /// <summary>
    /// Represents the ChartAttachedLabelPlotArea Biff record 
    /// </summary>
    PlotAreaLayout  = 0x08A7,
    /// <summary>
    /// Represents the ChartSeriesText Biff record.
    /// </summary>
    ChartSeriesText             = 0x100D,
    /// <summary>
    /// Represents the ChartChartFormat Biff record.
    /// </summary>
    ChartChartFormat            = 0x1014,
    /// <summary>
    /// Represents the ChartLegend Biff record.
    /// </summary>
    ChartLegend                 = 0x1015,
    /// <summary>
    /// Represents the ChartSeriesList Biff record.
    /// </summary>
    ChartSeriesList             = 0x1016,
    /// <summary>
    /// Represents the ChartBar Biff record.
    /// </summary>
    ChartBar                    = 0x1017,
    /// <summary>
    /// Represents the ChartLine Biff record.
    /// </summary>
    ChartLine                   = 0x1018,
    /// <summary>
    /// Represents the ChartPie Biff record.
    /// </summary>
    ChartPie                    = 0x1019,
    /// <summary>
    /// Represents the ChartArea Biff record.
    /// </summary>
    ChartArea                   = 0x101A,
    /// <summary>
    /// Represents the ChartScatter Biff record.
    /// </summary>
    ChartScatter                = 0x101B,
    /// <summary>
    /// Represents the ChartChartLine Biff record.
    /// </summary>
    ChartChartLine              = 0x101C,
    /// <summary>
    /// Represents the ChartAxis Biff record.
    /// </summary>
    ChartAxis                   = 0x101D,
    /// <summary>
    /// Represents the ChartTick Biff record.
    /// </summary>
    ChartTick                   = 0x101E,
    /// <summary>
    /// Represents the ChartValueRange Biff record.
    /// </summary>
    ChartValueRange             = 0x101F,
    /// <summary>
    /// Represents the ChartCatserRange Biff record.
    /// </summary>
    ChartCatserRange            = 0x1020,
    /// <summary>
    /// Represents the ChartAxisLineFormat Biff record.
    /// </summary>
    ChartAxisLineFormat         = 0x1021,
    /// <summary>
    /// Represents the ChartFormatLink Biff record.
    /// </summary>
    ChartFormatLink             = 0x1022,
    /// <summary>
    /// Represents the ChartDefaultText Biff record.
    /// </summary>
    ChartDefaultText            = 0x1024,
    /// <summary>
    /// Represents the ChartText Biff record.
    /// </summary>
    ChartText                   = 0x1025,
    /// <summary>
    /// Represents the ChartFontx Biff record.
    /// </summary>
    ChartFontx                  = 0x1026,
    /// <summary>
    /// Represents the ChartObjectLink Biff record.
    /// </summary>
    ChartObjectLink             = 0x1027,
    /// <summary>
    /// Represents the ChartFrame Biff record.
    /// </summary>
    ChartFrame                  = 0x1032,
    /// <summary>
    /// Represents the ChartPlotArea Biff record.
    /// </summary>
    ChartPlotArea               = 0x1035,
    /// <summary>
    /// Represents the Chart3D Biff record.
    /// </summary>
    Chart3D                     = 0x103A,
    /// <summary>
    /// Represents the ChartPicf Biff record.
    /// </summary>
    ChartPicf                   = 0x103C,
    /// <summary>
    /// Represents the ChartDropBar Biff record.
    /// </summary>
    ChartDropBar                = 0x103D,
    /// <summary>
    /// Represents the ChartRadar Biff record.
    /// </summary>
    ChartRadar                  = 0x103E,
    /// <summary>
    /// Represents the ChartSurface Biff record.
    /// </summary>
    ChartSurface                = 0x103F,
    /// <summary>
    /// Represents the ChartRadarArea Biff record.
    /// </summary>
    ChartRadarArea              = 0x1040,
    /// <summary>
    /// Represents the ChartAxisParent Biff record.
    /// </summary>
    ChartAxisParent             = 0x1041,
    /// <summary>
    /// Represents the ChartLegendxn Biff record.
    /// </summary>
    ChartLegendxn               = 0x1043,
    /// <summary>
    /// Represents the ChartShtprops Biff record.
    /// </summary>
    ChartShtprops               = 0x1044,
    /// <summary>
    /// Represents the ChartSertocrt Biff record.
    /// </summary>
    ChartSertocrt               = 0x1045,
    /// <summary>
    /// Represents the ChartAxesUsed Biff record.
    /// </summary>
    ChartAxesUsed               = 0x1046,
    /// <summary>
    /// Represents the ChartSbaseref Biff record.
    /// </summary>
    ChartSbaseref               = 0x1048,
    /// <summary>
    /// Represents the ChartSerParent Biff record.
    /// </summary>
    ChartSerParent              = 0x104A,
    /// <summary>
    /// Represents the ChartSerAuxTrend Biff record.
    /// </summary>
    ChartSerAuxTrend            = 0x104B,
    /// <summary>
    /// Represents the ChartIfmt Biff record.
    /// </summary>
    ChartIfmt                   = 0x104E,
    /// <summary>
    /// Represents the ChartPos Biff record.
    /// </summary>
    ChartPos                    = 0x104F,
    /// <summary>
    /// Represents the ChartAlruns Biff record.
    /// </summary>
    ChartAlruns                 = 0x1050,
    /// <summary>
    /// Represents the ChartAI Biff record.
    /// </summary>
    ChartAI                     = 0x1051,
    /// <summary>
    /// Represents the chart text properties stream
    /// </summary>
    ChartTextPropsStream        = 0x8A5,
    /// <summary>
    /// Represents the ChartSerAuxErrBar Biff record.
    /// </summary>
    ChartSerAuxErrBar           = 0x105B,
    /// <summary>
    /// Represents the ChartSerFmt Biff record.
    /// </summary>
    ChartSerFmt                 = 0x105D,
    /// <summary>
    /// Represents the Chart3DDataFormat Biff record.
    /// </summary>
    Chart3DDataFormat           = 0x105F,
    /// <summary>
    /// Represents the ChartFbi Biff record.
    /// </summary>
    ChartFbi                    = 0x1060,
    /// <summary>
    /// Represents the ChartBoppop Biff record.
    /// </summary>
    ChartBoppop                 = 0x1061,
    /// <summary>
    /// Represents the ChartAxcext Biff record.
    /// </summary>
    ChartAxcext                 = 0x1062,
    /// <summary>
    /// Represents the ChartDat Biff record.
    /// </summary>
    ChartDat                    = 0x1063,
    /// <summary>
    /// Represents the ChartPlotGrowth Biff record.
    /// </summary>
    ChartPlotGrowth             = 0x1064,
    /// <summary>
    /// Represents the ChartSiIndex Biff record.
    /// </summary>
    ChartSiIndex                = 0x1065,
    /// <summary>
    /// Represents the ChartGelFrame Biff record.
    /// </summary>
    ChartGelFrame               = 0x1066,
    /// <summary>
    /// Represents the ChartBoppCustom Biff record.
    /// </summary>
    ChartBoppCustom             = 0x1067,
    /// <summary>
    /// Represents the ChartShadow Biff record.
    /// </summary>
    ChartShadow                 = 0x1068,
    /// <summary>
    /// Represents the ChartUnits Biff record.
    /// </summary>
    ChartUnits                  = 0x1001,
    /// <summary>
    /// Represents the ChartWrapper Biff record.
    /// </summary>
    ChartWrapper                = 0x851,
    /// <summary>
    /// Represents the ChartAxisDisplayUnits biff record.
    /// </summary>
    ChartAxisDisplayUnits       = 0x857,
    /// <summary>
    /// Represents the ChartBegDispUnitRecord biff record.
    /// </summary>
    ChartBegDispUnit            = 0x854,
    /// <summary>
    /// Represents the ChartEndDispUnitRecord biff record.
    /// </summary>
    ChartEndDispUnit            = 0x855,
    /// <summary>
    /// Represents the ChartAxisOffsetRecord biff record.
    /// </summary>
    ChartAxisOffset             = 0x856,

    /// <summary>
    /// Represents the CacheData Biff record.
    /// </summary>
    CacheData   = 0xC6,
    /// <summary>
    /// Represents the CacheDataEx Biff record.
    /// </summary>
    CacheDataEx = 0x122,
    /// <summary>
    /// Represents the DataItem Biff record.
    /// </summary>
    DataItem    = 0xC5,
    /// <summary>
    /// Represents the ViewExtendedInfo Biff record.
    /// </summary>
    ViewExtendedInfo = 0xF1,
    /// <summary>
    /// Represents the ExternalSourceInfo Biff record.
    /// </summary>
    ExternalSourceInfo  = 0xDC,
    /// <summary>
    /// Represents the SQLDataTypeId Biff record.
    /// </summary>
    SQLDataTypeId       = 0x1BB,
    /// <summary>
    /// Represents the RuleFilter Biff record.
    /// </summary>
    RuleFilter          = 0xF2,
    /// <summary>
    /// Represents the ParsedExpression Biff record.
    /// </summary>
    ParsedExpression    = 0xF9,
    /// <summary>
    /// Represents the PivotFormat Biff record.
    /// </summary>
    PivotFormat         = 0xFB,
    /// <summary>
    /// Represents the PivotFormula Biff record.
    /// </summary>
    PivotFormula        = 0x103,
    /// <summary>
    /// Represents the StreamId Biff record.
    /// </summary>
    StreamId            = 0xD5,
    /// <summary>
    /// Represents the RowColumnFieldId Biff record.
    /// </summary>
    RowColumnFieldId    = 0xB4,
    /// <summary>
    /// Represents the LineItemArray Biff record.
    /// </summary>
    LineItemArray       = 0xB5,
    /// <summary>
    /// Represents the PivotName Biff record.
    /// </summary>
    PivotName           = 0xF6,
    /// <summary>
    /// Represents the PivotNamePair Biff record.
    /// </summary>
    PivotNamePair       = 0xF8,
    /// <summary>
    /// Represents the PageItem Biff record.
    /// </summary>
    PageItem            = 0xB6,
    /// <summary>
    /// Represents the RuleData Biff record.
    /// </summary>
    RuleData            = 0xF0,
    /// <summary>
    /// Represents the SelectionInfo Biff record.
    /// </summary>
    SelectionInfo       = 0xF7,
    /// <summary>
    /// Represents the sheet protection biff record.
    /// </summary>
    SheetProtection     = 0x867,
    /// <summary>
    /// Represents range protection and error indicators.
    /// </summary>
    RangeProtection     = 0x868,
    /// <summary>
    /// Represents the PivotString Biff record.
    /// </summary>
    PivotString         = 0xCD,
    /// <summary>
    /// Represents the PivotSourceInfo Biff record.
    /// </summary>
    PivotSourceInfo     = 0xD0,
    /// <summary>
    /// Represents the PageItemIndexes Biff record.
    /// </summary>
    PageItemIndexes     = 0xD2,
    /// <summary>
    /// Represents the PageItemNameCount Biff record.
    /// </summary>
    PageItemNameCount   = 0xD1,
    /// <summary>
    /// Represents the PivotViewFields Biff record.
    /// </summary>
    PivotViewFields     = 0xB1,
    /// <summary>
    /// Represents the PivotViewFieldsEx Biff record.
    /// </summary>
    PivotViewFieldsEx   = 0x100,
    /// <summary>
    /// Represents the PivotViewItem Biff record.
    /// </summary>
    PivotViewItem       = 0xB2,
    /// <summary>
    /// Represents the PivotViewDefinition Biff record.
    /// </summary>
    PivotViewDefinition = 0xB0,
    /// <summary>
    /// Represents the PivotViewSource Biff record.
    /// </summary>
    PivotViewSource     = 0xE3,
    /// <summary>
    /// Represents the PivotDateTime Biff record.
    /// </summary>
    PivotDateTime       = 0xCE,
    /// <summary>
    /// Represents the PivotDouble Biff record.
    /// </summary>
    PivotDouble         = 0xC9,
    /// <summary>
    /// Represents the PivotEmpty Biff record.
    /// </summary>
    PivotEmpty          = 0xCF,
    /// <summary>
    /// Represents the PivotBoolean Biff record.
    /// </summary>
    PivotBoolean        = 0xCA,
    /// <summary>
    /// Represents the PivotError Biff record.
    /// </summary>
    PivotError          = 0xCB,
    /// <summary>
    /// Represents the PivotField Biff record.
    /// </summary>
    PivotField          = 0xC7,
    /// <summary>
    /// Represents the PivotIndexList Biff record.
    /// </summary>
    PivotIndexList      = 0xC8,
    /// <summary>
    /// Represents pivot view additional info record.
    /// </summary>
    PivotViewAdditionalInfo = 0x864,
    /// <summary>
    /// This record specifies whether to check for compatibility with earlier application versions when saving the workbook from a version of the application<67> to the binary formats of other versions of the application<68>.
    /// </summary>
    Compatibility = 2188,
    /// <summary>
    /// Rerpresents the External connection record. 
    /// </summary>
    DBQueryExt = 0x803,
    /// <summary>
    /// Represents properties for a query table.
    /// </summary>
    Qsi=0x01AD,
    /// <summary>
    /// Represents the properties for a query table field.
    /// </summary>
    Qsif = 0x807,
    /// <summary>
    /// Represents a DbQuery or ParamQry record depending on the record.
    /// </summary>
    DbOrParamQry = 0xDC,
    /// <summary>
    /// Represents the name and refresh information for a query table or a PivotTable view.
    /// </summary>
    QsiSXTag = 0x802,
    /// <summary>
    /// Represents shared feature data that is used to describe a table in a worksheet
    /// </summary>
    Feature12 = 0x878,
    /// <summary>
    /// Represents the properties related to the formatting of a query table.
    /// </summary>
    Qsir = 0x806,
    /// <summary>
    /// Represents the additional properties 
    /// </summary>
    ChartMlFrt = 0x089E,
    /// <summary>
    /// Represents the DataBaseConnection 
    /// </summary>
    DConn = 0x0876,
    /// <summary>
    /// Represents the oledbconnection
    /// </summary>
    OleDbConn = 0x080A,
    /// <summary>
    /// Represents the Externalconnectioncollection
    /// </summary>
    ExtString = 0x0804,
    /// <summary>
    /// Represents the Feature record for query table
    /// </summary>
    TextQuery = 0x0805,
    /// <summary>
    /// Represents the pagelayout view record
    /// </summary>
    PageLayoutView = 0x88B
  }
}