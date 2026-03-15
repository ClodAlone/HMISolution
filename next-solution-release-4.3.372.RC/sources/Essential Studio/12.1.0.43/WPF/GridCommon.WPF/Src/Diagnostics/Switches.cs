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
using System.Windows.Interop;

namespace Syncfusion.Windows.Diagnostics
{
	/// <summary>
	///    Provides predefined switches for enabling / disabling trace
	///    output or code instrumentation in the Syncfusion shared library.
	/// </summary>
	public sealed class Switches
	{
        static Switches()
        {
            //MouseController.Level = TraceLevel.Verbose;
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                CurrentCell.Level = TraceLevel.Verbose;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static TraceSwitch MouseController = new TraceSwitch("Grid.MouseController", "Enable Tracing for MouseController");
        /// <summary>
        /// 
        /// </summary>
        public static TraceSwitch Styles = new TraceSwitch("Grid.Styles", "Enable Tracing for Styles");
        /// <summary>
        /// 
        /// </summary>
        public static TraceSwitch Serialization = new TraceSwitch("Grid.Serialization", "Enable Tracing for Serialization");
        /// <summary>
        /// 
        /// </summary>
        public static TraceSwitch VolatileData = new TraceSwitch("Grid.VolatileData", "Enable Tracing for VolatileData");
        /// <summary>
        /// 
        /// </summary>
        public static TraceSwitch CurrentCell = new TraceSwitch("Grid.CurrentCell", "Enable Tracing for CurrentCell");
        /// <summary>
        /// 
        /// </summary>
        public static TraceSwitch SelectRange = new TraceSwitch("Grid.SelectRange", "Enable Tracing for SelectRange");
    }
}
