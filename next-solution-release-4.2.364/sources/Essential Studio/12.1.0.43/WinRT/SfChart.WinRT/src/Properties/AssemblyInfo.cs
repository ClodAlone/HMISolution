#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.SfChart.WinRT")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Syncfusion.SfChart.WinRT")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyKeyFile(@"..\..\..\Common\Keys\sf.publicsnk")]
#if SyncfusionFramework4_5_1
[assembly: AssemblyVersion("12.1810.0.43")]
#elif SyncfusionFramework4_5
[assembly: AssemblyVersion("12.1800.0.43")]
#else
[assembly: AssemblyVersion("12.1800.0.43")]
#endif




namespace Syncfusion.UI.Xaml.Charts
{
    public class ClassReferenceAttribute : Attribute
    {
        private bool isReviewed = false;

        public bool IsReviewed
        {
            get { return isReviewed; }
            set
            {
                if (isReviewed == value)
                    return;

                isReviewed = value;

            }
        }

        private bool shouldInclude = true;

        public bool ShouldInclude
        {
            get { return shouldInclude; }
            set
            {
                if (shouldInclude == value)
                    return;

                shouldInclude = value;

            }
        }
    }

}