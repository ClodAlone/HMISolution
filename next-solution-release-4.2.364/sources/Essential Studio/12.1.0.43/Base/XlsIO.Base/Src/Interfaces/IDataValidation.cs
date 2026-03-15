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
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Contains a condition and the formatting attributes applied 
  /// to the cells, if the condition is met.
  /// </summary>
  public interface IDataValidation :
    IParentApplication,
    IOptimizedUpdate
  {
    /// <summary>
    /// Title of the prompt box.
    /// </summary>
    string PromptBoxTitle { get; set; }
    /// <summary>
    /// Text of the prompt box.
    /// </summary>
    string PromptBoxText { get; set; }
    /// <summary>
    /// Title of the error box.
    /// </summary>
    string ErrorBoxTitle { get; set; }
    /// <summary>
    /// Text of the error message.
    /// </summary>
    string ErrorBoxText { get; set; }
    /// <summary>
    /// Value of the first formula.
    /// </summary>
    string FirstFormula { get; set; }
    /// <summary>
    /// First formula's DateTime value.
    /// </summary>
    DateTime FirstDateTime { get; set; }
    /// <summary>
    /// Value of the second formula.
    /// </summary>
    string SecondFormula { get; set; }
    /// <summary>
    /// Second formula's DateTime value.
    /// </summary>
    DateTime SecondDateTime { get; set; }
    /// <summary>
    /// Type of the allowed data.
    /// </summary>
    ExcelDataType AllowType { get; set; }
    /// <summary>
    /// Compare operator used.
    /// </summary>
    ExcelDataValidationComparisonOperator CompareOperator { get; set; }
    /// <summary>
    /// Indicates whether formula contains list of values.
    /// </summary>
    bool IsListInFormula { get; set; }
    /// <summary>
    /// Indicates whether empty cell is allowed.
    /// </summary>
    bool IsEmptyCellAllowed { get; set; }
    /// <summary>
    /// Indicates whether to suppress drop-down arrow.
    /// </summary>
    bool IsSuppressDropDownArrow { get; set; }
    /// <summary>
    /// Indicates whether to show prompt box.
    /// </summary>
    bool ShowPromptBox { get; set; }
    /// <summary>
    /// Indicates whether to show error box.
    /// </summary>
    bool ShowErrorBox { get; set; }

    /// <summary>
    /// Horizontal position of the prompt box.
    /// </summary>
    int PromptBoxHPosition { get; set; }

    /// <summary>
    /// Vertical position of the prompt box.
    /// </summary>
    int PromptBoxVPosition { get; set; }
    /// <summary>
    /// Indicates whether prompt box is visible or not.
    /// </summary>
    bool IsPromptBoxVisible { get; set; }

    /// <summary>
    /// Indicates whether prompt box position is fixed.
    /// </summary>
    bool IsPromptBoxPositionFixed { get; set; }
    /// <summary>
    /// Type of the Error style
    /// </summary>
    ExcelErrorStyle ErrorStyle { get; set; }
    /// <summary>
    /// Array of possible values (when values in list are entered manually)
    /// </summary>
    string[] ListOfValues { get; set; }
    /// <summary>
    /// Range of possible values.
    /// </summary>
    IRange DataRange { get; set; }
  }
}
