#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// This class contains constants used for data validation parsing / serialization
  /// in Excel 2007 SpreadsheetML format.
  /// </summary>
  public sealed class DV
  {
    #region Constants
    /// <summary>
    /// Name of xml tag that represents data validations.
    /// </summary>
    public const string DataValidationsTagName = "dataValidations";
    /// <summary>
    /// Name of xml attribute name that represents data validation item count
    /// </summary>
    public const string ItemCountAttributeName = "count";
    /// <summary>
    /// Name of xml attribute name that indicates whether all input prompts for the worksheet are disabled.
    /// </summary>
    public const string DisablePromptsAttributeName = "disablePrompts";
    /// <summary>
    /// Name of xml attribute name that represents the x-coordinate (relative to window) of top-left
    /// corner of the data validation input prompt (textbox).
    /// </summary>
    public const string XCoodrinateAttributeName = "xWindow";
    /// <summary>
    /// Name of xml attribute name that represents the y-coordinate (relative to window) of top-left
    /// corner of the data validation input prompt (textbox).
    /// </summary>
    public const string YCoodrinateAttributeName = "yWindow";
    /// <summary>
    /// Name of xml tag that represents data validation.
    /// </summary>
    public const string DataValidationTagName = "dataValidation";
    /// <summary>
    /// Name of xml attribute name that represents data validation error message.
    /// </summary>
    public const string ErrorAttributeName = "error";
    /// <summary>
    /// Name of xml attribute name that represents data validation sequence of references.
    /// </summary>
    public const string CFSequenceOfReferencesAttributeName = "sqref";
    /// <summary>
    /// Name of xml attribute name that represents data validation type.
    /// </summary>
    public const string TypeAttributeName = "type";

    #region Data validation types
    /// <summary>
    /// Represents  which uses a custom formula to check the cell value.
    /// </summary>
    public const string TypeCustom = "custom";
    /// <summary>
    /// Represents  which checks for date values satisfying the given condition.
    /// </summary>
    public const string TypeDate = "date";
    /// <summary>
    /// Represents  which checks for decimal values satisfying the given condition.
    /// </summary>
    public const string TypeDecimal = "decimal";
    /// <summary>
    /// Represents  which checks for a value matching one of list of values.
    /// </summary>
    public const string TypeList = "list";
    /// <summary>
    /// Represents no data validation.
    /// </summary>
    public const string TypeNone = "none";
    /// <summary>
    /// Represents  which checks for text values, whose length satisfies the given condition.
    /// </summary>
    public const string TypeTextLength = "textLength";
    /// <summary>
    /// Represents  which checks for time values satisfying the given condition.
    /// </summary>
    public const string TypeTime = "time";
    /// <summary>
    /// Represrnts  which checks for whole number values satisfying the given condition.
    /// </summary>
    public const string TypeWhole = "whole";
    #endregion

    /// <summary>
    /// Name of xml attribute name that represents data validation allow blank status.
    /// </summary>
    public const string AllowBlankAttributeName = "allowBlank";
    /// <summary>
    /// Name of xml attribute name that represents data validation message text.
    /// </summary>
    public const string ErrorMessageAttributeName = "error";
    /// <summary>
    /// Name of xml attribute name that represents data validation error style.
    /// </summary>
    public const string ErrorStyleAttributeName = "errorStyle";
    #region Data validation error styles
    /// <summary>
    /// This data validation error style uses an information icon in the error alert.
    /// </summary>
    public const string ErrorStyleInformationIcon = "information";
    /// <summary>
    /// This data validation error style uses a stop icon in the error alert.
    /// </summary>
    public const string ErrorStyleStopIcon = "stop";
    /// <summary>
    /// This data validation error style uses a warning icon in the error alert.
    /// </summary>
    public const string ErrorStyleWarningIcon = "warning";
    #endregion
    /// <summary>
    /// Name of xml attribute name that represents data validation title bar text of error alert.
    /// </summary>
    public const string ErrorAlertTextAttributeName = "errorTitle";
    /// <summary>
    /// Name of xml attribute name that represents data validation relational operator used with this data validation.
    /// </summary>
    public const string OperatorAttributeName = "operator";

    #region Data validation relational operators
    /// <summary>
    /// Represents value  which checks if a value is between two other values.
    /// </summary>
    public const string OperatorBetween = "between";
    /// <summary>
    /// Represents value  which checks if a value is equal to a specified value.
    /// </summary>
    public const string OperatorEqual = "equal";
    /// <summary>
    /// Represents value  which checks if a value is greater than a specified value.
    /// </summary>
    public const string OperatorGreaterThan = "greaterThan";
    /// <summary>
    /// Represents value  which checks if a value is greater than or equal to a specified value.
    /// </summary>
    public const string OperatorGreaterThanOrEqual = "greaterThanOrEqual";
    /// <summary>
    /// Represents value  which checks if a value is less than a specified value.
    /// </summary>
    public const string OperatorLessThan = "lessThan";
    /// <summary>
    /// Represents value  which checks if a value is less than or equal to a specified value.
    /// </summary>
    public const string OperatorLessThanOrEqual = "lessThanOrEqual";
    /// <summary>
    /// Represents value  which checks if a value is not between two other values.
    /// </summary>
    public const string OperatorNotBetween = "notBetween";
    /// <summary>
    /// Represents value  which checks if a value is not equal to a specified value.
    /// </summary>
    public const string OperatorNotEqual = "notEqual";
    #endregion

    /// <summary>
    /// Name of xml attribute name that represents data validation message text of input prompt.
    /// </summary>
    public const string InputPromptAttributeName = "prompt";
    /// <summary>
    /// Name of xml attribute name that represents data validation title bar text of input prompt.
    /// </summary>
    public const string PromptTitleAttributeName = "promptTitle";
    /// <summary>
    /// Name of xml attribute name that indicates whether to display the dropdown combo box for a list type data validation.
    /// </summary>
    public const string ShowDropDownAttributeName = "showDropDown";
    /// <summary>
    /// Name of xml attribute name that indicates whether to display the error alert message.
    /// </summary>
    public const string ShowErrorMessageAttributeName = "showErrorMessage";
    /// <summary>
    /// Name of xml attribute name that indicates whether to display the input prompt message.
    /// </summary>
    public const string ShowInputMessageAttributeName = "showInputMessage";
    /// <summary>
    /// Name of xml tag name that represents the first formula in the data validation dropdown.
    /// </summary>
    public const string FormulaOneTagName = "formula1";
    /// <summary>
    /// Name of xml tag name that represents the second formula in the data validation dropdown.
    /// </summary>
    public const string FormulaTwoTagName = "formula2";
    #endregion

    #region Constructor
    /// <summary>
    /// Prevents a default instance of the DV class from being created.
    /// </summary>
    private DV()
    {
    }
    #endregion

    #region DataValidation XML SpreadSheet Constants
    /// <summary>
    /// Represents Range pref.
    /// </summary>
    public const string DEF_RANGE_PREF = "Range";
    /// <summary>
    /// Represents Type pref.
    /// </summary>
    public const string DEF_TYPE_PREF = "Type";
    /// <summary>
    /// Represents Selected pref.
    /// </summary>
    public const string DEF_MIN_PREF = "Min";
    /// <summary>
    /// Represents Selected pref.
    /// </summary>
    public const string DEF_MAX_PREF = "Max";
    /// <summary>
    /// Represents Selected pref.
    /// </summary>
    public const string DEF_INPUTTITLE_PREF = "InputTitle";
    /// <summary>
    /// Represents Selected pref.
    /// </summary>
    public const string DEF_INPUTMESSAGE_PREF = "InputMessage";
    /// <summary>
    /// Represents Selected pref.
    /// </summary>
    public const string DEF_ERRORSTYLE_PREF = "ErrorStyle";
    /// <summary>
    /// Represents Selected pref.
    /// </summary>
    public const string DEF_ERRORMESSAGE_PREF = "ErrorMessage";
    /// <summary>
    /// Represents Selected pref.
    /// </summary>
    public const string DEF_ERRORTITLE_PREF = "ErrorTitle";
    /// <summary>
    /// Represents Selected pref.
    /// </summary>
    public const string DEF_CELLRANGELIST_PREF = "CellRangeList";
    /// <summary>
    /// Represents Value pref.
    /// </summary>
    public const string DEF_VALUE_PREF = "Value";
    /// <summary>
    /// Represents Qualifier pref.
    /// </summary>
    public const string DEF_QUALIFIER_PREF = "Qualifier";
    #endregion
  }
}
