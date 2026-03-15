//-------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//------------------------------------------------------------------------------------------------- 

using System.Xml.Serialization;

#if !SILVERLIGHT
namespace Syncfusion.Olap.Reports
#else
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Reports
#endif
{
    /// <summary>
    /// Type of the axis
    /// </summary>
#if SILVERLIGHT
    [DataContract]
#endif
    public enum AxisPosition
    {
        //// <summary>
        //// This will be used when elements are common for both 
        //// Categorical and Series elements say sorting elements.
        //// </summary>
        ////None,

        /// <summary>
        /// Categorical Axis
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "Categorical")]
#endif
        [XmlEnum("Categorical")]
        Categorical,

        /// <summary>
        /// Series Axis
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "Series")]
#endif
        [XmlEnum("Series")]
        Series,

        /// <summary>
        /// Slicer Axis
        /// </summary>
#if SILVERLIGHT        
        [EnumMember(Value = "Slicer")]
#endif
        Slicer
    }

#if SILVERLIGHT
    [DataContract]
#endif
    /// <summary>
    /// Represents the filter cases.
    /// </summary>
    public enum FilterCase
    {
        /// <summary>
        /// FilterCase of Type Greater than
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "GreaterThan")]
#endif        
        GreaterThan,

        /// <summary>
        /// FilterCase of Type Less than
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "LessThan")]
#endif           
        LessThan,

        /// <summary>
        /// FilterCase of Type Equal to
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "EqualTo")]
#endif           
        EqualTo,

        /// <summary>
        /// FilterCase of Type Greater than or equal to 
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "GreaterThanOrEqualTo")]
#endif           
        GreaterThanOrEqualTo,

        /// <summary>
        /// FilterCase of Type Less that or Equal to
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "LessThanOrEqualTo")]
#endif           
        LessThanOrEqualTo,

        /// <summary>
        /// FilterCase of Type Not Equals
        /// </summary>
        
#if SILVERLIGHT
        [EnumMember(Value = "NotEquals")]
#endif           
        NotEquals,

#if SILVERLIGHT
        [EnumMember(Value = "Between")]
#endif
        Between,

        /// <summary>
        /// FilterCase of Type Between
        /// </summary>

#if SILVERLIGHT
        [EnumMember(Value = "NotBetween")]
#endif
        NotBetween
    }

    /// <summary>
    /// Represents the sort orders.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Break Hierarchy with Descending (on Sorting)
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "BDESC")]
#endif          
        BDESC,

        /// <summary>
        /// Break Hierarchy with Ascending (on Sorting)
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "BASC")]
#endif          
        BASC,

        /// <summary>
        /// Descending (on Sorting)
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "DESC")]
#endif          
        DESC,

        /// <summary>
        /// Ascending (on Sorting)
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "ASC")]
#endif          
        ASC
    }
#if SILVERLIGHT
    //public enum MemberTypeEnum
    //{
    //    /// <summary>
    //    ///     The member type is unknown.
    //    /// </summary>
    //    Unknown = 0,

    //    /// <summary>
    //    ///     The member is a regular member.
    //    /// </summary>
    //    Regular = 1,

    //    /// <summary>
    //    ///     The member is an All member.
    //    /// </summary>
    //    All = 2,

    //    /// <summary>
    //    ///     The member is a measure.
    //    /// </summary>
    //    Measure = 3,

    //    /// <summary>
    //    ///     The member is a calculated member or calculated measure.
    //    /// </summary>
    //    Formula = 4,
    //}

    //[DataContract]
    //public enum KpiAxisType
    //{
    //    [EnumMember(Value = "Row")]
    //    Row,

    //    [EnumMember(Value = "Column")]
    //    Column,

    //    [EnumMember(Value = "None")]
    //    None
    //}

    //[DataContract]
    //public enum KpiTypeEnum
    //{
    //    /// <summary>
    //    /// Type of KPI is KPI_None
    //    /// </summary>
    //    [EnumMember(Value = "Kpi_None")]
    //    Kpi_None,

    //    /// <summary>
    //    /// Type of KPI is KPI_Value
    //    /// </summary>
    //    [EnumMember(Value = "Kpi_Value")]
    //    Kpi_Value,

    //    /// <summary>
    //    /// Type of KPI is KPI_Goal
    //    /// </summary>
    //    [EnumMember(Value = "Kpi_Goal")]
    //    Kpi_Goal,

    //    /// <summary>
    //    /// Type of KPI is summary
    //    /// </summary>
    //    [EnumMember(Value = "Kpi_Status")]
    //    Kpi_Status,

    //    /// <summary>
    //    /// Type of KPI is KPI_Trend
    //    /// </summary>
    //    [EnumMember(Value = "Kpi_Trend")]
    //    Kpi_Trend
    //}
#endif

#if !SILVERLIGHT
    /// <summary>
    /// Type of Summaries available
    /// </summary>
    public enum SummaryType
    {
        /// <summary>
        /// Summary Type of Average
        /// </summary>
        Average,

        /// <summary>
        /// Summary Type of Count
        /// </summary>
        Count,

        /// <summary>
        /// Summary Type of Sum
        /// </summary>
        Sum,

        /// <summary>
        /// Summary Type is of Expression
        /// </summary>
        Expression,

        /// <summary>
        /// Summary Type is of String
        /// </summary>
        String,

        /// <summary>
        /// Summary Type is of Max
        /// </summary>
        Max,

        /// <summary>
        /// Summary Type is of Min
        /// </summary>
        Min,

        /// <summary>
        /// Summary Type is of First
        /// </summary>
        First,

        /// <summary>
        /// Summary Type is of Last
        /// </summary>
        Last
    }
#endif

    /// <summary>
    /// Represents the query builder engine versions.
    /// </summary>
    public enum QueryBuilderEngineVersions
    {
        /// <summary>
        /// If no Version is supplied default version will be taken as None
        /// Will be redirected to Version1
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "None")]
#endif          
        None,

        /// <summary>
        /// Represents the First Version of Query Builder Engine
        /// Contains VisualTotals Logic
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "Version1")]
#endif          
        Version1,

            /// <summary>
        /// Represents the Current Version of Query Builder Engine
        /// Contains Hierarchize Logic to minimize the query
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "Version2")]
#endif          
        Version2,

        /// <summary>
        /// Represents the Current Version of Query Builder Engine
        /// Contains Exclude implementation and CubeSchema removal.
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "Version3")]
#endif
        Version3
    }

    /// <summary>
    /// Represents the drill types.
    /// </summary>
    public enum DrillType
    {
        /// <summary>
        /// This is default DrillType
        /// </summary>
        DrillMember,
        /// <summary>
        /// This will enable Drilling by member position
        /// </summary>
        DrillPosition,
        /// <summary>
        /// This will enable drilling by replacing the members
        /// </summary>
        DrillReplace
    }

    /// <summary>
    /// Represents the drill states.
    /// </summary>
    public enum DrillState
    {

        /// <summary>
        /// Default state.
        /// </summary>
        Default,

        /// <summary>
        /// To expand all levels.
        /// </summary>
        ExpandAll,

        /// <summary>
        /// To collapse all levels.
        /// </summary>
        CollapseAll,

        /// <summary>
        /// To expand until a specific level.
        /// </summary>
        ExpandToLevel,

        /// <summary>
        /// To collapse until a specific level.
        /// </summary>
        CollapseToLevel
    }
}
