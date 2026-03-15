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

using System.Diagnostics;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{

    /// <summary>
    /// This interface is implemented by classes that host series specific style information.
    /// </summary>
    public interface IChartSeriesStylesHost
    {
        /// <summary>
        ///	    A <see cref="ChartBaseStylesMap"/> acts as a repository that is used to hold information on registered base styles.
        ///	    This information forms the core that is needed to apply base style information to styles.
        /// </summary>
        ChartBaseStylesMap GetStylesMap();

        /// <summary>
        ///    Gets the back color hint from the host.
        /// </summary>
        Color BackColor { get; }
    }
}