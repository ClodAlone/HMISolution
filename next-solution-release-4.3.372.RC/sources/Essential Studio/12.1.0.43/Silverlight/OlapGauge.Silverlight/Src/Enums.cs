#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base
{

    [DataContract]
    public enum AxisPosition
    {
        [EnumMember(Value = "Categorical")]
        Categorical,
        [EnumMember(Value = "Series")]
        Series,
        [EnumMember(Value = "Slicer")]
        Slicer
    }

    [DataContract]
    public enum KpiAxisType
    {
        [EnumMember(Value = "Row")]
        Row,

        [EnumMember(Value = "Column")]
        Column,

        [EnumMember(Value = "None")]
        None
    }

    [DataContract]
    public enum KpiTypeEnum
    {
        /// <summary>
        /// Type of KPI is KPI_None
        /// </summary>
        [EnumMember(Value = "Kpi_None")]
        Kpi_None,

        /// <summary>
        /// Type of KPI is KPI_Value
        /// </summary>
        [EnumMember(Value = "Kpi_Value")]
        Kpi_Value,

        /// <summary>
        /// Type of KPI is KPI_Goal
        /// </summary>
        [EnumMember(Value = "Kpi_Goal")]
        Kpi_Goal,

        /// <summary>
        /// Type of KPI is summary
        /// </summary>
        [EnumMember(Value = "Kpi_Status")]
        Kpi_Status,

        /// <summary>
        /// Type of KPI is KPI_Trend
        /// </summary>
        [EnumMember(Value = "Kpi_Trend")]
        Kpi_Trend
    }

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

    [DataContract]
    public enum PivotCellDescriptorType
    {
        /// <summary>
        /// The cellwrapper is of type any
        /// </summary>
        [EnumMember(Value = "Any")]
        Any,

        /// <summary>
        /// The cellwrapper is of type ColumnHeader
        /// </summary>
        [EnumMember(Value = "ColumnHeader")]
        ColumnHeader,

        /// <summary>
        /// The cellwrapper is of type RowHeader
        /// </summary>
        [EnumMember(Value = "RowHeader")]
        RowHeader,

        /// <summary>
        /// The cellwrapper is of type Value
        /// </summary>
        [EnumMember(Value = "Value")]
        Value,

        /// <summary>
        /// The cellwrapper is of type SummaryRow
        /// </summary>
        [EnumMember(Value = "SummaryRow")]
        SummaryRow,

        /// <summary>
        /// The cellwrapper is of type SummaryColumn
        /// </summary>
        [EnumMember(Value = "SummaryColumn")]
        SummaryColumn
    }

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


}
