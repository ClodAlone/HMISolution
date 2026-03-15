//-------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.Olap.Engine
{

    /// <summary>
    /// Holds the Enumeration of AxisType
    /// </summary>
    public enum KpiAxisType
    {
        /// <summary>
        /// Row Axis type for KPI Elements
        /// </summary>
        Row,

        /// <summary>
        /// Column Axis type for KPI Elements
        /// </summary>
        Column,

        /// <summary>
        /// KPI Item not present
        /// </summary>
        None
    }

    /// <summary>
    /// Specifies the Type of Sorting
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// Sorting type is Ascending
        /// </summary>
        Ascending,

        /// <summary>
        /// Sorting type is Descending
        /// </summary>
        Descending,

        /// <summary>
        /// Sorting type is UnSpecified
        /// </summary>
        UnSpecified
    }

    /// <summary>
    /// Represents cell current expandable cell.
    /// </summary>
    public enum ExpandableState
    {
        /// <summary>
        /// The cell can not be expanded or collapsed.
        /// </summary>
        None,

        /// <summary>
        /// The cell is in expanded state
        /// </summary>
        Expanded,

        /// <summary>
        /// The cell is in collapsed state
        /// </summary>
        Collapsed
    }

    /// <summary>
    /// Represents Grid Layout
    /// </summary>
    public enum GridLayout
    {
        /// <summary>
        /// Grid Layout will be Normal
        /// </summary>
        Normal,

        /// <summary>
        /// Grid Layout will be like Excel
        /// </summary>
        ExcelLikeLayout,

        /// <summary>
        /// Grid without Summaries
        /// </summary>
        NoSummaries,

        /// <summary>
        /// Grid Layout will be Normal with Top positioned Summary
        /// </summary>
        NormalTopSummary,

        /// <summary>
        /// Grid Layout will be like Excel along with Member Properties
        /// </summary>
        ExcelLikeLayoutWithMemberProperties
    }

    /// <summary>
    /// Extended types for cell descriptor.
    /// </summary>
    public enum PivotCellDescriptorExType
    {
        /// <summary>
        /// The cell descriptor is of any datatype
        /// </summary>
        Any,

        /// <summary>
        /// The cell descriptor is of type Overalltotal
        /// </summary>
        OverallTotal,

        /// <summary>
        /// The cell descriptor is of type DrilledDown
        /// </summary>
        DrilledDown,

        /// <summary>
        /// The cell descriptor is of type SummaryRow
        /// </summary>
        SummaryRow,

        /// <summary>
        /// The cell descriptor is of type SummaryColumn
        /// </summary>
        SummaryColumn,
    }

    /// <summary>
    /// Represents cellwrapper type.
    /// </summary>
    public enum PivotCellDescriptorType
    {
        /// <summary>
        /// The cellwrapper is of type any
        /// </summary>
        Any,

        /// <summary>
        /// The cellwrapper is of type ColumnHeader
        /// </summary>
        ColumnHeader,

        /// <summary>
        /// The cellwrapper is of type RowHeader
        /// </summary>
        RowHeader,

        /// <summary>
        /// The cellwrapper is of type Value
        /// </summary>
        Value,

        /// <summary>
        /// The cellwrapper is of type SummaryRow
        /// </summary>
        SummaryRow,

        /// <summary>
        /// The cellwrapper is of type SummaryColumn
        /// </summary>
        SummaryColumn
    }

    /// <summary>
    /// Relative summary position.
    /// </summary>
    public enum SummaryLayout
    {
        /// <summary>
        /// The relative summary position is None
        /// </summary>
        None,

        /// <summary>
        /// The relative summary position is Top
        /// </summary>
        Top,

        /// <summary>
        /// The relative summary position is None
        /// </summary>
        Bottom
    }
}
