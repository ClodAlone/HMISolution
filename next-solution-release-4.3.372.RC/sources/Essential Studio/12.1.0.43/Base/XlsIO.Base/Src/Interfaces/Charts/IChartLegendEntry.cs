#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
#endregion

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Summary description for IChartLegendEntry.
	/// </summary>
	public interface IChartLegendEntry
	{
    #region Class properties
    /// <summary>
    /// If true then this entry deleted. otherwise false.
    /// </summary>
    bool IsDeleted { get; set; }
    /// <summary>
    /// True if the legend entry has been formatted.
    /// </summary>
    bool IsFormatted { get; set; }
    /// <summary>
    /// Represents text area.
    /// </summary>
    IChartTextArea TextArea { get; }
    #endregion

    #region Class methods
    /// <summary>
    /// Clears current data point
    /// </summary>
    void Clear();
    /// <summary>
    /// Deletes current legend entry.
    /// </summary>
    void Delete();
    #endregion
	}
}
