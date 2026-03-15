//-------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if SILVERLIGHT
using System.Runtime.Serialization;
namespace Syncfusion.OlapSilverlight.Data
#else
namespace Syncfusion.Olap.Data
#endif
{
    /// <summary>
    /// Specifies the type of the meta tree node
    /// </summary>
    public enum MetaTreeNodeType
    {
        /// <summary>
        /// Nothing is specified to the node
        /// </summary>
        None,

        /// <summary>
        /// Node type is Cube
        /// </summary>
        Cube,

        /// <summary>
        /// Measure group node
        /// </summary>
        MeasureGroup,

        /// <summary>
        /// Measure node
        /// </summary>
        Measure,

        /// <summary>
        /// Dimension node
        /// </summary>
        Dimension,

        /// <summary>
        /// Hierarchy Node
        /// </summary>
        Hierarchy,

        /// <summary>
        /// Display Folder
        /// </summary>
        DisplayFolder,

        /// <summary>
        /// Level Node
        /// </summary>
        Level,

        /// <summary>
        /// Node type is Member
        /// </summary>
        Member,

        /// <summary>
        /// node type is KPI root Element
        /// </summary>
        KPI_ROOT,

        /// <summary>
        /// Node type is KPI
        /// </summary>
        KPI,

        /// <summary>
        /// Node type is KPI_Value
        /// </summary>
        KPI_Value,

        /// <summary>
        /// Node type is KPI_Goal
        /// </summary>
        KPI_Goal,

        /// <summary>
        /// Node type is KPI_Status
        /// </summary>
        KPI_Status,

        /// <summary>
        /// Node type is KPI_Trend
        /// </summary>
        KPI_Trend,

        /// <summary>
        /// Node type is NamedSet
        /// </summary>
        NamedSet,

        /// <summary>
        /// Node type is Calculated Member Group
        /// </summary>
        CalculatedMemberGroup,

        /// <summary>
        /// Node type is Calculated Member
        /// </summary>
        CalculatedMember,
        /// <summary>
        /// Node type is VirtualKpi Group
        /// </summary>
        VirtualKPIGroup,
        /// <summary>
        /// Node type is VirtualKpi Member
        /// </summary>
        VirtualKPIMember,
         /// <summary>
        /// Node type is Virtual Kpi Value
        /// </summary>
        VirtualKPI_Value,
         /// <summary>
        /// Node type is Virtual Kpi Goal
        /// </summary>
        VirtualKPI_Goal,
         /// <summary>
        /// Node type is Virtual Kpi Status
        /// </summary>
        VirtualKPI_Status,
         /// <summary>
        /// Node type is Virtual Kpi Trend
        /// </summary>
        VirtualKPI_Trend
    }

    /// <summary>
    /// Meta tree node check status
    /// </summary>
    public enum MetaTreeNodeCheckedType
    {
        /// <summary>
        /// Current node and the child node are not selected
        /// </summary>
        NoneSelected,

        /// <summary>
        /// Only Current node selected, none of the child nodes are selected
        /// </summary>
        CurrentChecked,

        /// <summary>
        /// Some child node are selected, for this MetaTreeNode IsSelected property 
        /// will be set to null
        /// </summary>
        SomeChildChecked
    }

    /// <summary>
    /// Type of the axis
    /// </summary>
    public enum AxisType
    {
        /// <summary>
        /// Categorical Axis
        /// </summary>
        Categorical,

        /// <summary>
        /// Series Axis
        /// </summary>
        Series,

        /// <summary>
        /// Filter condition
        /// </summary>
        Slicer
    }

    /// <summary>
    /// Axis Number in AdomdCellset.
    /// </summary>
    public enum AxisNum
    {
        /// <summary>
        /// Categorical Axis Number
        /// </summary>
        Axis0,

        /// <summary>
        /// Series Axis Number
        /// </summary>
        Axis1
    }

    /// <summary>
    /// This Enum details can be found in the following link location:
    /// http://msdn.microsoft.com/en-us/library/microsoft.analysisservices.adomdclient.leveltypeenum(SQL.90).aspx
    /// </summary>
    public enum LevelTypeEnum
    {
        /// <summary>
        /// Type of Level is All
        /// </summary>
        All,

        /// <summary>
        /// Type of Level is Account
        /// </summary>
        Account,

        /// <summary>
        /// Type of Level is BomResource
        /// </summary>
        BomResource,

        /// <summary>
        /// Type of Level is Calculated
        /// </summary>
        Calculated,

        /// <summary>
        /// Type of Level is Channel
        /// </summary>
        Channel,

        /// <summary>
        /// Type of Level is Company
        /// </summary>
        Company,

        /// <summary>
        /// Type of Level is CurrencyDestination
        /// </summary>
        CurrencyDestination,

        /// <summary>
        /// Type of Level is CurrencySource
        /// </summary>
        CurrencySource,

        /// <summary>
        /// Type of Level is Customer
        /// </summary>
        Customer,

        /// <summary>
        /// Type of Level is CustomerGroup
        /// </summary>
        CustomerGroup,

        /// <summary>
        /// Type of Level is CustomerHousehold
        /// </summary>
        CustomerHousehold,

        /// <summary>
        /// Type of Level is GeoCity
        /// </summary>
        GeoCity,

        /// <summary>
        /// Type of Level is GeoContinent
        /// </summary>
        GeoContinent,

        /// <summary>
        /// Type of Level is GeoCountry
        /// </summary>
        GeoCountry,

        /// <summary>
        /// Type of Level is County
        /// </summary>
        GeoCounty,

        /// <summary>
        /// Type of Level is GeoPoint
        /// </summary>
        GeoPoint,

        /// <summary>
        /// Type of Level is GeoPostalCode
        /// </summary>
        GeoPostalCode,

        /// <summary>
        /// Type of Level is GeoRegion
        /// </summary>
        GeoRegion,

        /// <summary>
        /// Type of Level is GeoStateOrProvince
        /// </summary>
        GeoStateOrProvince,

        /// <summary>
        /// Type of Level is OrgUnit
        /// </summary>
        OrgUnit,

        /// <summary>
        /// Type of Level is Person
        /// </summary>
        Person,

        /// <summary>
        /// Type of Level is Product
        /// </summary>
        Product,

        /// <summary>
        /// Type of Level is ProductGroup
        /// </summary>
        ProductGroup,

        /// <summary>
        /// Type of Level is Promotion
        /// </summary>
        Promotion,

        /// <summary>
        /// Type of Level is Quantitative
        /// </summary>
        Quantitative,

        /// <summary>
        /// Type of Level is Regular
        /// </summary>
        Regular,

        /// <summary>
        /// Type of Level is Representative
        /// </summary>
        Representative,

        /// <summary>
        /// Type of Level is Reserved1
        /// </summary>
        Reserved1,

        /// <summary>
        /// Type of Level is Scenario
        /// </summary>
        Scenario,

        /// <summary>
        /// Type of Level is Time
        /// </summary>
        Time,

        /// <summary>
        /// Type of Level is TimeDays
        /// </summary>
        TimeDays,

        /// <summary>
        /// Type of Level is TimeHalfYears
        /// </summary>
        TimeHalfYears,

        /// <summary>
        /// Type of Level is TimeHours
        /// </summary>
        TimeHours,

        /// <summary>
        /// Type of Level is TimeMinutes
        /// </summary>
        TimeMinutes,

        /// <summary>
        /// Type of Level is TimeMonths
        /// </summary>
        TimeMonths,

        /// <summary>
        /// Type of Level is TimeQuarters
        /// </summary>
        TimeQuarters,

        /// <summary>
        /// Type of Level is TimeSeconds
        /// </summary>
        TimeSeconds,

        /// <summary>
        /// Type of Level is TimeUndefined
        /// </summary>
        TimeUndefined,

        /// <summary>
        /// Type of Level is TimeWeeks
        /// </summary>
        TimeWeeks,

        /// <summary>
        /// Type of Level is TimeYears
        /// </summary>
        TimeYears,

        /// <summary>
        /// Type of Level is Utility
        /// </summary>
        Utility
    }

    /// <summary>
    /// Represents the dimension types.
    /// </summary>
    public enum DimensionTypeEnum
    {
        /// <summary>
        /// Type of Dimension is Accounts
        /// </summary>
        Accounts,

        /// <summary>
        /// Type of Dimension is BillOfMaterials
        /// </summary>
        BillOfMaterials,

        /// <summary>
        /// Type of Dimension is Channel
        /// </summary>
        Channel,

        /// <summary>
        /// Type of Dimension is Currency
        /// </summary>
        Currency,

        /// <summary>
        /// Type of Dimension is Customers
        /// </summary>
        Customers,

        /// <summary>
        /// Type of Dimension is Geography
        /// </summary>
        Geography,

        /// <summary>
        /// Type of Dimension is Measure
        /// </summary>
        Measure,

        /// <summary>
        /// Type of Dimension is Organization
        /// </summary>
        Organization,

        /// <summary>
        /// Type of Dimension is Other
        /// </summary>
        Other,

        /// <summary>
        /// Type of Dimension is Products
        /// </summary>
        Products,

        /// <summary>
        /// Type of Dimension is Promotion
        /// </summary>
        Promotion,

        /// <summary>
        /// Type of Dimension is Quantitative
        /// </summary>
        Quantitative,

        /// <summary>
        /// Type of Dimension is Rates
        /// </summary>
        Rates,

        /// <summary>
        /// Type of Dimension is Scenario
        /// </summary>
        Scenario,

        /// <summary>
        /// Type of Dimension is Time
        /// </summary>
        Time,

        /// <summary>
        /// Type of Dimension is Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Type of Dimension is Utility
        /// </summary>
        Utility
    }

    /// <summary>
    /// Represents the enumeration constants for Cell Types.
    /// </summary>
    public enum CellType
    {
        /// <summary>
        /// Type of Cell is ColumnHeader
        /// </summary>
        ColumnHeader,

        /// <summary>
        /// Type of Cell is ColumnTable
        /// </summary>
        ColumnTable,

        /// <summary>
        /// Type of Cell is RowHeader
        /// </summary>
        RowHeader,

        /// <summary>
        /// Type of Cell is RowTable
        /// </summary>
        RowTable,

        /// <summary>
        /// Type of Cell is DataCell
        /// </summary>
        DataCell
    }

    /// <summary>
    /// Represents the Member types.
    /// </summary>
    public enum MemberTypeEnum
    {
        /// <summary>
        ///     The member type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     The member is a regular member.
        /// </summary>
        Regular = 1,

        /// <summary>
        ///     The member is an All member.
        /// </summary>
        All = 2,

        /// <summary>
        ///     The member is a measure.
        /// </summary>
        Measure = 3,

        /// <summary>
        /// The member is a calculated member or calculated measure.
        /// </summary>
        Formula = 4,
    }

    ////Added for identifying specific KPI type in Member Object
    /// <summary>
    /// Represents the specific KPI type in Member Object.
    /// </summary>
    public enum KpiTypeEnum
    {
        /// <summary>
        /// Type of KPI is KPI_None
        /// </summary>
        Kpi_None,

        /// <summary>
        /// Type of KPI is KPI_Value
        /// </summary>
        Kpi_Value,

        /// <summary>
        /// Type of KPI is KPI_Goal
        /// </summary>
        Kpi_Goal,

        /// <summary>
        /// Type of KPI is summary
        /// </summary>
        Kpi_Status,

        /// <summary>
        /// Type of KPI is KPI_Trend
        /// </summary>
        Kpi_Trend
    }

    /// <summary>
    /// Enumeration code for chaging order of Measures in CubeDimensionBrowser
    /// </summary>
    public enum SortCubeMeasureOrder
    {
        /// <summary>
        ///  Measures will be displaued in Ascending Order
        /// </summary>
        ASC,

        /// <summary>
        ///  Measures will be displayed in Descending Order
        /// </summary>
        DSC,

        /// <summary>
        /// Measures will be displayed as it is
        /// </summary>
        Default
    }

}
