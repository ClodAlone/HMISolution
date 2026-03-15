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
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    /// <summary>
    /// Type of the axis
    /// </summary>
    [DataContract]
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
        [EnumMember(Value = "Categorical")]
        Categorical,

        /// <summary>
        /// Series Axis
        /// </summary>
        [EnumMember(Value = "Series")]
        Series,

        /// <summary>
        /// Slicer Axis
        /// </summary>
        [EnumMember(Value = "Slicer")]
        Slicer
    }

    [DataContract]
    public enum FilterCase
    {
        /// <summary>
        /// FilterCase of Type Greater than
        /// </summary>
        [EnumMember(Value = "GreaterThan")]
        GreaterThan,

        /// <summary>
        /// FilterCase of Type Less than
        /// </summary>
        [EnumMember(Value = "LessThan")]
        LessThan,

        /// <summary>
        /// FilterCase of Type Equal to
        /// </summary>
        [EnumMember(Value = "EqualTo")]
        EqualTo,

        /// <summary>
        /// FilterCase of Type Greater than or equal to 
        /// </summary>
        [EnumMember(Value = "GreaterThanOrEqualto")]
        GreaterThanOrEqualto,

        /// <summary>
        /// FilterCase of Type Less that or Equal to
        /// </summary>
        [EnumMember(Value = "LessthanOrEqualto")]
        LessthanOrEqualto,

        /// <summary>
        /// FilterCase of Type Not Equals
        /// </summary>
        [EnumMember(Value = "NotEquals")]
        NotEquals
    }

    public enum SortOrder
    {
        /// <summary>
        /// Break Hierarchy with Descending (on Sorting)
        /// </summary>
        [EnumMember(Value = "BDESC")]
        BDESC,

        /// <summary>
        /// Break Hierarchy with Ascending (on Sorting)
        /// </summary>
        [EnumMember(Value = "BASC")]
        BASC,

        /// <summary>
        /// Descending (on Sorting)
        /// </summary>
        [EnumMember(Value = "DESC")]
        DESC,

        /// <summary>
        /// Ascending (on Sorting)
        /// </summary>
        [EnumMember(Value = "ASC")]
        ASC
    }

    public enum QueryBuilderEngineVersions
    {
        /// <summary>
        /// If no Version is supplied default version will be taken as None
        /// Will be redirected to Version1
        /// </summary>
        [EnumMember(Value = "None")]
        None,

        /// <summary>
        /// Represents the First Version of Query Builder Engine
        /// Contains VisualTotals Logic
        /// </summary>
        [EnumMember(Value = "Version1")]
        Version1,

        /// <summary>
        /// Represents the Current Version of Query Builder Engine
        /// Contains Hierarchize Logic to minimize the query
        /// </summary>
        [EnumMember(Value = "Version2")]
        Version2
    }
}
