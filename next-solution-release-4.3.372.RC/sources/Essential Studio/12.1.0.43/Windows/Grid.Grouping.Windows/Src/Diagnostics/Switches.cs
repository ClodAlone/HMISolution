//-------------------------------------------------------------------------------------------------
// <copyright file="Switches.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    ///    Provides predefined switches for enabling/disabling trace
    ///    output or code instrumentation in the Syncfusion shared library.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    sealed class Switches
    {
        static Switches()
        {
////            AutoPopulate.Level = TraceLevel.Verbose;
        }

        public static TraceSwitch GroupingGrid = new TraceSwitch("Grid.Grouping", "Grid Grouping");
        public static TraceSwitch GroupingEngine = new TraceSwitch("Grid.Grouping.Engine", "Grid Grouping");
        public static TraceSwitch CurCellNestedGrid = new TraceSwitch("Grid.Grouping.CurCellNestedGrid", "Grid Grouping CurCellNestedGrid");
        public static TraceSwitch AutoPopulate = new TraceSwitch("Grouping.AutoPopulate", "AutoPopulate");
        public static TraceSwitch SynchronizeGridWithEngine = new TraceSwitch("Grid.Grouping.SynchronizeGridWithEngine", "SynchronizeGridWithEngine");    
    }
}
