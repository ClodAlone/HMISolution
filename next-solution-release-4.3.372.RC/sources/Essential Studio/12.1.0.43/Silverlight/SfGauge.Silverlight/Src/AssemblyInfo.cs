#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.SfGauge.Silverlight")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Syncfusion.SfGauge.Silverlight")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("adcb74de-7e2e-4cb8-a1f2-0cb6265a3b91")]

[assembly: XmlnsPrefix("clr-namespace:Syncfusion.UI.Xaml.Gauges;assembly=Syncfusion.SfGauge.Silverlight", "syncfusion")]
[assembly: XmlnsDefinition("clr-namespace:Syncfusion.UI.Xaml.Gauges;assembly=Syncfusion.SfGauge.Silverlight", "Syncfusion.UI.Xaml.Gauges")]
#if (SyncfusionFramework4_0 && Silverlight5)
[assembly: AssemblyVersion("12.1500.0.43")]
#elif (SyncfusionFramework4_0 && Silverlight4)
[assembly: AssemblyVersion("12.1400.0.43")]
#else
[assembly: AssemblyVersion("12.1400.0.43")]
#endif




#pragma warning disable 1699 // disable warning for "Use command line option '/keyfile' ...

[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyKeyFile(@"..\..\..\..\..\Common\Keys\sf.publicsnk")]
[assembly: AssemblyKeyName("")]

namespace Syncfusion.UI.Xaml.Gauges
{
    internal class ClassReferenceAttribute : Attribute
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
