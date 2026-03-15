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
  /// This class contains constants used for conditional formatting parsing/serialization
  /// in Excel 2007 SpreadsheetML format.
  /// </summary>
  public sealed class CF
  {
    #region Constants
    /// <summary>
    /// Name of the xml tag that defines conditional formattings
    /// </summary>
    public const string ConditionalFormattingsTagName = "conditionalFormattings";
    /// <summary>
    /// Name of the xml tag that represents conditional formatting.
    /// </summary>
    public const string ConditionalFormattingTagName = "conditionalFormatting";
    /// <summary>
    /// Name of the xml tag that represents conditional formatting rule.
    /// </summary>
    public const string RuleTagName = "cfRule";
    /// <summary>
    /// Name of the xml tag that represent Ends With Spefific Text conitional formatting.
    /// </summary>
    public const string EndsWith = "endsWith";
    /// <summary>
    /// Name of the xml tag that represent Begins With Spefific Text conitional formatting.
    /// </summary>
    public const string BeginsWith = "beginsWith";
    /// <summary>
    /// Name of the xml tag that represent Contains Text Spefific Text conitional formatting.
    /// </summary>
    public const string ContainsText = "containsText";
    /// <summary>
    /// Name of the xml tag that represent NotContains Text Spefific Text conitional formatting.
    /// </summary>
    public const string NotContainsText = "notContainsText";
    /// <summary>
    /// Name of the xml tag that represent contains error conitional formatting.
    /// </summary>
    public const string TypeContainsError = "containsErrors";
    /// <summary>
    /// Name of the xml tag that represent not Contains Error conitional formatting.
    /// </summary>
    public const string TypeNotContainsError = "notContainsErrors";
    /// <summary>
    ///Name of the xml tag that represent text attribute of Spefific Text conitional formatting. 
    /// </summary>
    public const string TextAttributeName = "text";
    /// <summary>
    /// Name of the xml attribute that represents type of conditional formatting rule.
    /// </summary>
    public const string TypeAttributeName = "type";
    /// <summary>
    /// Name of the xml attribute that represents time period type conditional formatting rule.
    /// </summary>
    public const string TimePeriodTypeName = "timePeriod";
    /// <summary>
    /// Name of the xml attribute that represents time period type attribute name.
    /// </summary>
    public const string TimePeriodAttributeName = "timePeriod";
    /// <summary>
    /// Name of the xml attribute that represents an index to a dxf element in the Styles Part indicating which cell formatting to
    /// apply when the conditional formatting rule criteria is met.
    /// </summary>
    public const string DifferentialFormattingIdAttributeName = "dxfId";
    /// <summary>
    /// Name of the xml attribute that represents the operator in a "cell value is" conditional formatting rule.
    /// </summary>
    public const string OperatorAttributeName = "operator";
    /// <summary>
    /// Name of the xml tag that defines the border color.
    /// </summary>
    public const string BorderColorTagName = "borderColor";
    /// <summary>
    /// Name of the xml tag that defines the negative fill color.
    /// </summary>
    public const string NegativeFillColorTagName = "negativeFillColor";
    /// <summary>
    /// Name of the xml tag that defines the negative border color.
    /// </summary>
    public const string NegativeBorderColorTagName = "negativeBorderColor";
    /// <summary>
    /// Name of the xml tag that defines the axis color.
    /// </summary>
    public const string AxisColorTagName = "axisColor";
    /// <summary>
    /// Name of the xml attribute that represents the border of data bar
    /// </summary>
    public const string BorderAttributeName = "border";
    /// <summary>
    /// Name of the xml attribute that represents the gradient fill of data bar
    /// </summary>
    public const string GradientAttributeName = "gradient";
    /// <summary>
    /// Name of the xml attribute that represents the direction of data bar
    /// </summary>
    public const string DirectionAttributeName = "direction";
    /// <summary>
    /// Name of the xml attribute that represents the negative bar color of data bar
    /// </summary>
    public const string NegativeBarColorSameAsPositiveAttributeName = "negativeBarColorSameAsPositive";
    /// <summary>
    /// Name of the xml attribute that represents the negative bar border color of data bar
    /// </summary>
    public const string NegativeBarBorderColorSameAsPositiveAttributeName = "negativeBarBorderColorSameAsPositive";
    /// <summary>
    /// Name of the xml attribute that represents the axis position of data bar
    /// </summary>
    public const string AxisPositionAttributeName = "axisPosition";

    #region CF Time Period Types
    /// <summary>
    /// Today's time period type
    /// </summary>
    public const string TimePeriodToday = "today";
    /// <summary>
    /// Yesterday's time period type
    /// </summary>
    public const string TimePeriodYesterday = "yesterday";
    /// <summary>
    /// Tomorrow's time period type
    /// </summary>
    public const string TimePeriodTomorrow = "tomorrow";
    /// <summary>
    /// Last seven days time period type
    /// </summary>
    public const string TimePeriodLastsevenDays = "last7Days";
    /// <summary>
    /// Last week time period type
    /// </summary>
    public const string TimePeriodLastWeek= "lastWeek";
    /// <summary>
    /// This week time period type
    /// </summary>    
    public const string TimePeriodThisWeek = "thisWeek";
    /// <summary>
    /// Next week time period type
    /// </summary>
    public const string TimePeriodNextWeek = "nextWeek";
    /// <summary>
    /// Last month period type
    /// </summary>
    public const string TimePeriodLastMonth = "lastMonth";
    /// <summary>
    /// This month period type
    /// </summary>
    public const string TimePeriodThisMonth = "thisMonth";
    /// <summary>
    /// Next month time period type
    /// </summary>
    public const string TimePeriodNextMonth = "nextMonth";

    #endregion

    #region CF operators
    /// <summary>
    /// Name of the xml tag that represents begin with condition.
    /// </summary>
    public const string OperatorBeginsWith = "beginsWith";
    /// <summary>
    /// Name of the xml tag that represents between condition.
    /// </summary>
    public const string OperatorBetween = "between";
    /// <summary>
    /// Name of the xml tag that represents contains text condition.
    /// </summary>
    public const string OperatorContains = "containsText";
    /// <summary>
    /// Name of the xml tag that represents ends with condition.
    /// </summary>
    public const string OperatorEndsWith = "endsWith";
    /// <summary>
    /// Name of the xml tag that represents equal condition.
    /// </summary>
    public const string OperatorEqual = "equal";
    /// <summary>
    /// Name of the xml tag that represents greater than condition.
    /// </summary>
    public const string OperatorGreaterThan = "greaterThan";
    /// <summary>
    /// Name of the xml tag that represents greater than or equal to condition.
    /// </summary>
    public const string OperatorGreaterThanOrEqual = "greaterThanOrEqual";
    /// <summary>
    /// Name of the xml tag that represents less than condition.
    /// </summary>
    public const string OperatorLessThan = "lessThan";
    /// <summary>
    /// Name of the xml tag that represents less than or equal to condition.
    /// </summary>
    public const string OperatorLessThanOrEqual = "lessThanOrEqual";
    /// <summary>
    /// Name of the xml tag that represents not between condition.
    /// </summary>
    public const string OperatorNotBetween = "notBetween";
    /// <summary>
    /// Name of the xml tag that represents not contains condition.
    /// </summary>
    public const string OperatorDoesNotContain = "notContains";
    /// <summary>
    /// Name of the xml tag that represents not equal condition.
    /// </summary>
    public const string OperatorNotEqual = "notEqual";
    #endregion

    /// <summary>
    /// Name of the xml tag To stop evaluating conditional formatting rules for a cell.
    /// </summary>
    public const string StopIfTrueAttributeName = "stopIfTrue";
    /// <summary>
    /// Name of the xml attribute that represents the priority of this conditional formatting rule.
    /// </summary>
    public const string PriorityAttributeName = "priority";
    /// <summary>
    /// Name of the xml tag name that represents a formula whose calculated value specifies the criteria for the conditional
    /// formatting rule.
    /// </summary>
    public const string FormulaTagName = "formula";
    /// <summary>
    /// Represents conditional formatting rule that compares a cell value to a formula calculated result, using an operator.
    /// </summary>
    public const string TypeCellIs = "cellIs";
    /// <summary>
    /// Represents conditional formatting rule that contains a formula to evaluate.
    /// </summary>
    public const string TypeExpression = "expression";
    /// <summary>
    /// Represents conditional formatting rule that contains databar.
    /// </summary>
    public const string TypeDataBar = "dataBar";
    /// <summary>
    /// Represents conditional formatting rule that contains iconset.
    /// </summary>
    public const string TypeIconSet = "iconSet";
    /// <summary>
    /// Represents conditional formatting rule that contains color scale.
    /// </summary>
    public const string TypeColorScale = "colorScale";
    /// <summary>
    /// Represents conditional formatting rule highlights cells that are completely blank
    /// </summary>
    public const string TypeContainsBlank = "containsBlanks";    
    /// <summary>
    /// Represents conditional formatting rule highlights cells that are not blank
    /// </summary>
    public const string TypeNotContainsBlank = "notContainsBlanks";
    /// <summary>
    /// Describes a data bar conditional formatting rule.
    /// </summary>
    public const string DataBarTag = "dataBar";
    /// <summary>
    /// Describes the values of the interpolation points in a gradient scale.
    /// </summary>
    public const string ValueObjectTag = "cfvo";
    /// <summary>
    /// Default value for minimum length of the data bar.
    /// </summary>
    public const int DefaultDataBarMinLength = 10;
    /// <summary>
    /// Default value for maximum length of the data bar.
    /// </summary>
    public const int DefaultDataBarMaxLength = 90;
    /// <summary>
    /// The maximum length of the data bar, as a percentage of the cell width.
    /// </summary>
    public const string MaxLengthTag = "maxLength";
    /// <summary>
    /// The minimum length of the data bar, as a percentage of the cell width.
    /// </summary>
    public const string MinLengthTag = "minLength";
    /// <summary>
    /// Indicates whether to show the values of the cells on which this data bar is applied.
    /// </summary>
    public const string ShowValueAttribute = "showValue";
    /// <summary>
    /// Describes an icon set conditional formatting rule.
    /// </summary>
    public const string IconSetTag = "iconSet";
    /// <summary>
    /// The icon set to display.
    /// </summary>
    public const string IconSetAttribute = "iconSet";
    /// <summary>
    /// Describes a color scale conditional formatting rule.
    /// </summary>
    public const string ColorScaleTag = "colorScale";
    /// <summary>
    /// Indicates whether the thresholds indicate percentile values, instead of number values.
    /// </summary>
    public const string PercentAttribute = "percent";
    /// <summary>
    /// If '1', reverses the default order of the icons in this icon set.
    /// </summary>
    public const string ReverseAttribute = "reverse";
    public const string GreaterAttribute = "gte";
    /// <summary>
    /// Possible value types for ConditionValue.
    /// </summary>
    public static readonly string[] ValueTypes = new string[]
    {
      "none",
      "num",
      "min",
      "max",
      "percent",      
      "percentile",
      "formula",
    };
    /// <summary>
    /// Names used in xml as IconSet type (order is important).
    /// </summary>
    public static string[] IconSetTypeNames = new string[]
    {
      "3Arrows",
      "3ArrowsGray",
      "3Flags",
      "3TrafficLights1",
      "3TrafficLights2",
      "3Signs",
      "3Symbols",
      "3Symbols2",
      "4Arrows",
      "4ArrowsGray",
      "4RedToBlack",
      "4Rating",
      "4TrafficLights",
      "5Arrows",
      "5ArrowsGray",
      "5Rating",
      "5Quarters",      
    };
    #endregion

    #region Constructor
    /// <summary>
    ///Prevents a default instance of the CF class from being created.
    /// </summary>
    private CF()
    {
    }
    #endregion
  }
}
