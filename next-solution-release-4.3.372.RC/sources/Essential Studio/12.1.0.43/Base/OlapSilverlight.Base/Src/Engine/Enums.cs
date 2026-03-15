#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Engine
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
    /// Holds the Enumeration of Expandable states.
    /// </summary>
    [DataContract]
    public enum ExpandableState
    {
        /// <summary>
        /// The cell can not be expanded or collapsed.
        /// </summary>
        [EnumMember(Value = "None")]
        None,

        /// <summary>
        /// The cell is in expanded state
        /// </summary>
        [EnumMember(Value = "Expanded")]
        Expanded,

        /// <summary>
        /// The cell is in collapsed state
        /// </summary>
        [EnumMember(Value = "Collapsed")]
        Collapsed
    }

    /// <summary>
    /// Holds the Enumeration of Grid Range Info Type.
    /// </summary>
    [DataContract]
    public enum GridRangeInfoType
    {
        /// <summary>
        /// Range is empty.
        /// </summary>
        Empty = 0x00,
        /// <summary>
        /// Range of cells.
        /// </summary>
        Cells = 0x01,
        /// <summary>
        /// Range with rows.
        /// </summary>
        Rows = 0x02,
        /// <summary>
        /// Range with columns.
        /// </summary>
        Cols = 0x04,
        /// <summary>
        /// Range is a whole table.
        /// </summary>
        //[Browsable(false)]
        Table = Rows | Cols
    }

    /// <summary>
    /// Holds the enumeration constants for PivotCellDescriptor Ex type.
    /// </summary>
    [DataContract]
    public enum PivotCellDescriptorExType
    {
        /// <summary>
        /// The cell descriptor is of any data type
        /// </summary>
        Any,

        /// <summary>
        /// The cell descriptor is of type Overall total
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
    /// Holds the enumeration constants for PivotCellDescriptor type.
    /// </summary>
    [DataContract]
    public enum PivotCellDescriptorType
    {
        /// <summary>
        /// The CellWrapper is of type any
        /// </summary>
        [EnumMember(Value = "Any")]
        Any,

        /// <summary>
        /// The CellWrapper is of type ColumnHeader
        /// </summary>
        [EnumMember(Value = "ColumnHeader")]
        ColumnHeader,

        /// <summary>
        /// The CellWrapper is of type RowHeader
        /// </summary>
        [EnumMember(Value = "RowHeader")]
        RowHeader,

        /// <summary>
        /// The CellWrapper is of type Value
        /// </summary>
        [EnumMember(Value = "Value")]
        Value,

        /// <summary>
        /// The CellWrapper is of type SummaryRow
        /// </summary>
        [EnumMember(Value = "SummaryRow")]
        SummaryRow,

        /// <summary>
        /// The CellWrapper is of type SummaryColumn
        /// </summary>
        [EnumMember(Value = "SummaryColumn")]
        SummaryColumn
    }

    /// <summary>
    /// Holds the enumeration constants for Summary Layout.
    /// </summary>
    [DataContract]
    public enum SummaryLayout
    {
        /// <summary>
        /// The relative summary position is None
        /// </summary>
        [EnumMember(Value = "None")]
        None,

        /// <summary>
        /// The relative summary position is Top
        /// </summary>
        [EnumMember(Value = "Top")]
        Top,

        /// <summary>
        /// The relative summary position is None
        /// </summary>
        [EnumMember(Value = "Bottom")]
        Bottom
    }

    /// <summary>
    /// Represents the Summary type.
    /// </summary>
    [DataContract]
    public enum SummaryType
    {
        /// <summary>
        /// Average
        /// </summary>
        Average,
        /// <summary>
        /// Count
        /// </summary>
        Count,
        /// <summary>
        /// Summation
        /// </summary>
        Sum,
        /// <summary>
        /// Expression
        /// </summary>
        Expression,
        /// <summary>
        /// String
        /// </summary>
        String,
        /// <summary>
        /// Maximum
        /// </summary>
        Max,
        /// <summary>
        /// Minimum
        /// </summary>
        Min,
        /// <summary>
        /// First
        /// </summary>
        First,
        /// <summary>
        /// Last
        /// </summary>
        Last
    }

    /// <summary>
    /// Represents the sorting types.
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// Ascending order.
        /// </summary>
        Ascending,
        /// <summary>
        /// Descending order.
        /// </summary>
        Descending,
        /// <summary>
        /// Un specified order.
        /// </summary>
        UnSpecified
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


}
