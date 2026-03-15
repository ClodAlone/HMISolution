//-------------------------------------------------------------------------------------------------
// <copyright file="FilterValue.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;


#if !SILVERLIGHT
namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the filter value item.
    /// </summary>
    [Serializable]
    public class FilterValue : Element
#else

using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class FilterValue : Element
#endif
    {
        #region Public Variable
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the filter value.
        /// </summary>
        /// <value>The filter value.</value>
        public double Filter_Value { get; set; }
        #endregion
    }
}
