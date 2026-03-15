#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

#if !SILVERLIGHT
using Syncfusion.Olap.Manager;
namespace Syncfusion.Olap
{
#else
using Syncfusion.OlapSilverlight.Manager;
namespace Syncfusion.OlapSilverlight
{
#endif

    /// <summary>
    /// Represents the IPageable control.
    /// </summary>
    public interface IPageableControl
    {
        /// <summary>
        /// Gets or sets the data manager.
        /// </summary>
        /// <value>The data manager.</value>
        OlapDataManager OlapDataManager { get; set; }
        /// <summary>
        /// Binds the data.
        /// </summary>
        void DataBind();
        /// <summary>
        /// Occurs when [refresh pager].
        /// </summary>
        event EventHandler RefreshPager;
    }
}
