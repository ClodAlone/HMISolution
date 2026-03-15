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
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if DEBUG
[assembly: AssemblyDescription("Debug")]
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyDescription("Prebuilt Release")]
[assembly: AssemblyConfiguration("Prebuilt Release")]
#endif

[assembly: AssemblyTitle("Syncfusion.ChartConverter.Wpf")]
[assembly: AssemblyCompany("Syncfusion, Inc.")]
[assembly: AssemblyProduct("Syncfusion Essential Studio-Syncfusion.ChartConverter.Wpf.resources")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

#if !SyncfusionFramework4_0
[assembly: AllowPartiallyTrustedCallers]
#endif


// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("354f9d73-72c8-4643-bb17-84edb307e754")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
#if SyncfusionFramework4_5_1
[assembly: AssemblyVersion("12.1451.0.43")]
#elif SyncfusionFramework4_5
[assembly: AssemblyVersion("12.1450.0.43")]
#elif SyncfusionFramework4_0
[assembly: AssemblyVersion("12.1400.0.43")]
#elif SyncfusionFramework3_5
[assembly: AssemblyVersion("12.1350.0.43")]
#elif SyncfusionFramework2_0
[assembly: AssemblyVersion("12.1200.0.43")]
#else
[assembly: AssemblyVersion("12.1350.0.43")]
#endif




//#pragma warning disable 1699 // disable warning for "Use command line option '/keyfile' ...

//[assembly: AssemblyDelaySign(true)]
//[assembly: AssemblyKeyFile("../../../../../Common/keys/sf.publicsnk")]

namespace Syncfusion
{
    /// <summary>
    /// This class holds the name of the Syncfusion.Windows.Chart assembly and provides a helper 
    /// routine that helps with resolving types when loading a serialization stream and when 
    /// the framework probes for assemblies by reflection. 
    /// </summary>
    public class ChartConverterWpfAssembly
    {
        /// <summary>
        /// The full name of this assembly without version information: "Syncfusion.Chart.Wpf"
        /// </summary>
        public static readonly string Name;

        /// <summary>
        /// A reference to the <see cref="System.Reflection.Assembly"/> for the Chart assembly.
        /// </summary>
        public static readonly Assembly Assembly;


        /// <summary>
        /// The root namespace of this assembly. Used internally for locating resources within the assembly.
        /// </summary>
        public static readonly string RootNamespace = "Syncfusion.Windows.Chart";

        static ChartConverterWpfAssembly()
        {
            Assembly = typeof(ChartConverterWpfAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

        /// <summary>
        /// This delegate helps with resolving types and can be used as a eventhandler
        /// for a <see cref="System.AppDomain.AssemblyResolve"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data with information about the type.</param>
        /// <returns>A reference to the assembly where the type is located.</returns>
        /// <remarks>
        /// Use this handler when reading back types from a serialization stread
        /// saved with an earlier version of this assembly.
        /// </remarks>
        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            if (e.Name.StartsWith(Name))
                return Assembly;
            else if (e.Name.StartsWith(ChartWPFAssembly.Name))
                return ChartWPFAssembly.Assembly;
            else
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int n = 0; n < assemblies.Length; n++)
                {
                    if (assemblies[n].GetName().Name == e.Name)
                        return assemblies[n];
                }
            }

            return null;
        }

    }
}

namespace Syncfusion.Windows.Chart.Converter
{
    internal class AssemblyInfo : Syncfusion.ChartConverterWpfAssembly
    {
    }
}