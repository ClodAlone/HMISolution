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
using System.Windows.Markup;
using System.Windows;
using System;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "Syncfusion.Gauge.Wpf" )]
[assembly: AssemblyDescription( "" )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "Syncfusion Inc" )]
[assembly: AssemblyProduct( "Syncfusion.Gauge.Wpf" )]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]
[assembly: ComVisible( false )]

#if SyncfusionFramework3_5
[assembly: System.Security.AllowPartiallyTrustedCallers]
#endif

// Specifies the location in which theme dictionaries are stored for types in an assembly.
[assembly: ThemeInfo(
    // Specifies the location of system theme-specific resource dictionaries for this project.
    // The default setting in this project is "None" since this default project does not
    // include these user-defined theme files:
    //     Themes\Aero.NormalColor.xaml
    //     Themes\Generic.xaml
    //     Themes\Luna.Homestead.xaml
    //     Themes\Luna.Metallic.xaml
    //     Themes\Luna.NormalColor.xaml
    //     Themes\Royale.NormalColor.xaml
    ResourceDictionaryLocation.SourceAssembly,

    // Specifies the location of the system non-theme specific resource dictionary:
    //     Themes\generic.xaml
   ResourceDictionaryLocation.SourceAssembly )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:

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






//[assembly: AssemblyDelaySign(true)]

//[assembly: AssemblyKeyFile(@"..\..\..\..\..\common\keys\sf.publicsnk")]

//[assembly: AssemblyKeyName("")]

[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Gauge")]

namespace Syncfusion
{
    /// <summary>
    /// Resolves the Gauge Assembly.
    /// </summary>
    public class GaugeAssembly
    {

        /// <summary>
        /// Stores the name of the Assembly
        /// </summary>
        public static readonly string Name;

        /// <summary>
        /// Variable of Assembly type.
        /// </summary>
        public static readonly Assembly Assembly;

        /// <summary>
        /// Stores the root namespace <see cref="Syncfusion.Windows.Gauge"/>
        /// </summary>
        public static readonly string RootNamespace = "Syncfusion.Windows.Gauge";

        static GaugeAssembly()
        {
            Assembly = typeof(GaugeAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

        /// <summary>
        /// Resolves the Assembly.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            if (e.Name.StartsWith(GaugeAssembly.Name))
                return GaugeAssembly.Assembly;
            else if (e.Name.StartsWith(CoreAssembly.Name))
                return CoreAssembly.Assembly;
#if SILVERLIGHT
#else
            else
            {
                string name = e.Name.ToLower();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int n = 0; n < assemblies.Length; n++)
                {
                    if (assemblies[n].GetName().Name.ToLower() == name)
                        return assemblies[n];
                }
            }
#endif
            return null;
        }
    }

    internal class AssemblyInfo : GaugeAssembly
    {
    }
}