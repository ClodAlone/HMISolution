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

using Syncfusion.XlsIO.Interfaces;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Contains a condition and the formatting attributes applied 
  /// to the cells, if the condition is met.
  /// </summary>
  public interface IConditionalFormat
    : IParentApplication
    , IOptimizedUpdate
  {
    /// <summary>
    /// Type of the conditional format.
    /// </summary>
    ExcelCFType             FormatType { get; set; }
    /// <summary>
    ///  Type of time period.
    /// </summary>
    CFTimePeriods TimePeriodType { get; set; }
    /// <summary>
    /// Type of the comparison operator.
    /// </summary>
    ExcelComparisonOperator Operator { get; set; }
    /// <summary>
    /// Indicates whether the font is bold.
    /// </summary>
    bool                    IsBold { get; set; }
    /// <summary>
    /// Indicates whether font is italic.
    /// </summary>
    bool                    IsItalic { get; set; }
    /// <summary>
    /// Font color.
    /// </summary>
    ExcelKnownColors        FontColor { get; set; }
    /// <summary>
    /// Font color.
    /// </summary>
    Color                   FontColorRGB { get; set; }
    /// <summary>
    /// Underline type.
    /// </summary>
    ExcelUnderline          Underline { get; set; }
    /// <summary>
    /// Indicates whether font is struck through.
    /// </summary>
    bool                    IsStrikeThrough { get; set; }
    /// <summary>
    /// Color of the left line.
    /// </summary>
    ExcelKnownColors        LeftBorderColor { get; set; }
    /// <summary>
    /// Color of the left line.
    /// </summary>
    Color                   LeftBorderColorRGB { get; set; }
    /// <summary>
    /// Left border line style.
    /// </summary>
    ExcelLineStyle          LeftBorderStyle { get; set; }
    /// <summary>
    /// Color of the right line.
    /// </summary>
    ExcelKnownColors        RightBorderColor { get; set; }
    /// <summary>
    /// Color of the right line.
    /// </summary>
    Color                   RightBorderColorRGB { get; set; }
    /// <summary>
    /// Right border line style.
    /// </summary>
    ExcelLineStyle          RightBorderStyle { get; set; }
    /// <summary>
    /// Color of the top line.
    /// </summary>
    ExcelKnownColors        TopBorderColor { get; set; }
    /// <summary>
    /// Color of the top line.
    /// </summary>
    Color                   TopBorderColorRGB { get; set; }
    /// <summary>
    /// Top border line style.
    /// </summary>
    ExcelLineStyle          TopBorderStyle { get; set; }
    /// <summary>
    /// Color of the bottom line.
    /// </summary>
    ExcelKnownColors        BottomBorderColor { get; set; }
    /// <summary>
    /// Color of the bottom line
    /// </summary>
    Color                   BottomBorderColorRGB { get; set; }
    /// <summary>
    /// Bottom border line style.
    /// </summary>
    ExcelLineStyle          BottomBorderStyle { get; set; }
    /// <summary>
    /// First formula.
    /// </summary>
    string                  FirstFormula { get; set; }
    /// <summary>
    /// First formula in R1C1 notation. Read-only.
    /// </summary>
    string                  FirstFormulaR1C1 { get;set; }
    /// <summary>
    /// Second formula.
    /// </summary>
    string                  SecondFormula { get; set; }
    /// <summary>
    /// Second formula in R1C1 notation. Read-only.
    /// </summary>
    string                  SecondFormulaR1C1 { get;set; }
    /// <summary>
    /// Pattern foreground color.
    /// </summary>
    ExcelKnownColors        Color { get; set; }
    /// <summary>
    /// Pattern foreground color.
    /// </summary>
    Color                   ColorRGB { get; set; }
    /// <summary>
    /// Pattern background color.
    /// </summary>
    ExcelKnownColors        BackColor { get; set; }
    /// <summary>
    /// Pattern background color.
    /// </summary>
    Color                   BackColorRGB { get; set; }
    /// <summary>
    /// Fill pattern style.
    /// </summary>
    ExcelPattern            FillPattern { get; set; }
    /// <summary>
    /// Indicates whether font is superscript.
    /// </summary>
    bool                    IsSuperScript { get; set; }
    /// <summary>
    /// Indicates whether font is subscript.
    /// </summary>
    bool                    IsSubScript { get; set; }
    /// <summary>
    /// True if contains font formatting.
    /// </summary>
    bool                    IsFontFormatPresent { get; set; }
    /// <summary>
    /// True if contains border formatting.
    /// </summary>
    bool                    IsBorderFormatPresent { get; set; }
    /// <summary>
    /// True if contains pattern formatting.
    /// </summary>
    bool                    IsPatternFormatPresent { get; set; }
    /// <summary>
    /// If true - format color present. otherwise - false.
    /// </summary>
    bool                    IsFontColorPresent { get; set; }
    /// <summary>
    /// If true - pattern color present, otherwise - false.
    /// </summary>
    bool                    IsPatternColorPresent { get; set; }
    /// <summary>
    /// If true - background color present. otherwise - false.
    /// </summary>
    bool                    IsBackgroundColorPresent { get; set; }
    /// <summary>
    /// True if left border style and color are modified.
    /// </summary>
    bool                    IsLeftBorderModified { get; set; }
    /// <summary>
    /// True if right border style and color modified.
    /// </summary>
    bool                    IsRightBorderModified { get; set; }
    /// <summary>
    /// True if top border style and color are modified.
    /// </summary>
    bool                    IsTopBorderModified { get; set; }
    /// <summary>
    /// True if bottom border style and color are modified.
    /// </summary>
    bool                    IsBottomBorderModified { get; set; }
    /// <summary>
    /// Returns data bar settings. Valid only if FormatType is set to DataBar. Read-only.
    /// </summary>
    IDataBar DataBar { get; }
    /// <summary>
    /// Returns iconset settings. Valid only if FormatType is set to IconSet. Read-only.
    /// </summary>
    IIconSet IconSet { get; }
    /// <summary>
    /// Returns color scale settings. Valid only if FormatType is set to ColorScale. Read-only.
    /// </summary>
    IColorScale ColorScale { get; }
    /// <summary>
    /// Number format of the conditional format rule.
    /// </summary>
    string NumberFormat { get; set; }
    ///<summary>
    /// Lower priority conditional formatting rules are evaluated.
    ///</summary>
    bool StopIfTrue { get; set; }
    /// <summary>
    /// The text value in a Specific Text conditional formatting rule. 
    /// Valid only for type =contains Text, notContainsText,beginsWith,endsWith. The default value is null.
    /// </summary>
    string Text { get; set; }
  }
}
