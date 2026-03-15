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
  /// This class contains constants used for autofilter parsing and serialization in Excel 2007 format.
  /// </summary>
  public sealed class AF
  {
    /// <summary>
    /// Name of the xml tag name that represents auto filter settings.
    /// </summary>
    public const string AutoFilterSettingsTagName = "autoFilter";
    /// <summary>
    /// Name of the xml attribute that represents reference to the cell range to which the AutoFilter is applied.
    /// </summary>
    public const string CellOrRangeReferenceAttributeName = "ref";
    /// <summary>
    /// Name of the xml tag that identifies a particular column in the AutoFilter range and specifies
    /// filter information that has been applied to this column
    /// </summary>
    public const string AutoFilterColumnTagName = "filterColumn";
    /// <summary>
    /// Name of the xml attribute that indicates the AutoFilter column to which this filter information applies.
    /// </summary>
    public const string FilterColumnDataAttributeName = "colId";
    /// <summary>
    /// Name of the xml tag that specifies the top N (percent or number of items) to filter by.
    /// </summary>
    public const string AutoFilterTopTenTagName = "top10";
    /// <summary>
    /// Name of the xml attribute that represents top or bottom value to use as the filter criteria.
    /// </summary>
    public const string TopOrBottomValueAttributeName = "val";
    /// <summary>
    /// Name of the xml attribute that represents flag indicating whether or not to filter by top order.
    /// </summary>
    public const string TopAttributeAttributeName = "top";
    /// <summary>
    /// Name of the xml attribute that represents the actual cell value in the range
    /// which is used to perform the comparison for this filter.
    /// </summary>
    public const string FilterValAttributeName = "filterVal";
    /// <summary>
    /// Name of the xml attribute that represents flag indicating whether or not to filter by percent value of the column.
    /// </summary>
    public const string FilterByPercentAttributeName = "percent";
    /// <summary>
    /// Name of the xml tag that groups filter criteria together.
    /// </summary>
    public const string FilterCriteriaTagName = "filters";
    /// <summary>
    /// Flag indicating whether to filter by blank.
    /// </summary>
    public const string FilterBlankAttributeName = "blank";
    /// <summary>
    /// Name of the xml tag that expresses a filter criteria value.
    /// </summary>
    public const string FilterTagName = "filter";
    /// <summary>
    /// Name of the xml attribute that represents filter value used in the criteria.
    /// </summary>
    public const string FilterValueAttributeName = "val";
    /// <summary>
    /// Name of the xml tag that represents custom filters criteria.
    /// </summary>
    public const string CustomFiltersCriteriaTagName = "customFilters";
    /// <summary>
    /// Name of the xml attribute that represents flag indicating whether the two criteria have an "and" relationship.
    /// '1' indicates "and", '0' indicates "or".
    /// </summary>
    public const string AndCriteriaAttributeName = "and";
    /// <summary>
    /// Name of the xml tag that represents custom filter criteria.
    /// </summary>
    public const string CustomFilterCriteriaTagName = "customFilter";
    /// <summary>
    /// Name of the xml attribute name that represents operator used by the filter comparison.
    /// </summary>
    public const string FilterComparisonOperatorAttributeName = "operator";

    #region Operator enumerations for filtering
    /// <summary>
    /// Show results which are equal to criteria.
    /// </summary>
    public const string OperatorEqual = "equal";
    /// <summary>
    /// Show results which are greater than criteria.
    /// </summary>
    public const string OperatorGreaterThan = "greaterThan";
    /// <summary>
    /// Show results which are greater than or equal to criteria.
    /// </summary>
    public const string OperatorGreaterThanOrEqual = "greaterThanOrEqual";
    /// <summary>
    /// Show results which are less than criteria.
    /// </summary>
    public const string OperatorLessThan = "lessThan";
    /// <summary>
    /// Show results which are less than or equal to criteria.
    /// </summary>
    public const string OperatorLessThanOrEqual = "lessThanOrEqual";
    /// <summary>
    /// Show results which are not equal to criteria.
    /// </summary>
    public const string OperatorNotEqual = "notEqual";
    #endregion
  }
}
