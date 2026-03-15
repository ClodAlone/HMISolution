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

namespace Syncfusion.Grouping
{
    /// <summary>
    ///    Provides predefined switches for enabling / disabling trace
    ///    output or code instrumentation in the Syncfusion shared library.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    sealed class Switches
    {
        static Switches()
        {
            ////            CurrentRecord.Level = TraceLevel.Verbose;
            ////            AutoPopulate.Level = TraceLevel.Verbose;
            ////            DescriptorPropertyChange.Level = TraceLevel.Verbose;
        }

        public static TraceSwitch GroupingEngine = new TraceSwitch("Grouping.Engine", "Grouing Engine");
        public static TraceSwitch CurrentRecord = new TraceSwitch("Grouping.CurrentRecord", "CurrentRecord");
        public static TraceSwitch AutoPopulate = new TraceSwitch("Grouping.AutoPopulate", "AutoPopulate");
        public static TraceSwitch DescriptorPropertyChange = new TraceSwitch("Grouping.DescriptorPropertyChange", "DescriptorPropertyChange");
        public static TraceSwitch RuntimeElementsInTableCollection = new TraceSwitch("Grid.Grouping.RuntimeElementsInTableCollection", "RuntimeElementsInTableCollection");
        public static TraceSwitch InvalidateCounterBottomUp = new TraceSwitch("Grid.Grouping.InvalidateCounterBottomUp", "InvalidateCounterBottomUp");
    }
}
