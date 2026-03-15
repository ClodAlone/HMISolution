//-------------------------------------------------------------------------------------------------
// <copyright file="SummaryInfo.cs" company="syncfusion">
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
    /// Represents the summary detail.
    /// </summary>
    [Serializable]
    public class SummaryInfo
#else
using Syncfusion.OlapSilverlight.Common;
using Syncfusion.OlapSilverlight.Engine;
namespace Syncfusion.OlapSilverlight.Reports
{ 
    /// <summary>
    /// Represents the summary detail.
    /// </summary>
    public class SummaryInfo
#endif

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryInfo"/> class.
        /// </summary>
        public SummaryInfo()
        {
            this.Key = string.Empty;
            this.Column = string.Empty;
        }

        /// <summary>
        /// After calculating the summary will be stored in this key name
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Column name of which summary need to be calculated
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Type of summary
        /// </summary>
        public SummaryType Type { get; set; }

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public string Expression { get; set; }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString { get; set; }
    }
}