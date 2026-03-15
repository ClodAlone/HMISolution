#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region Using directives

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if SILVERLIGHT
[assembly: AssemblyTitle("Syncfusion.GridCommon.Silverlight")]
#else
[assembly: AssemblyTitle("Syncfusion.GridCommon.WPF")]
#endif
#if DEBUG
[assembly: AssemblyDescription("Debug")]
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyDescription("Prebuilt Release")]
[assembly: AssemblyConfiguration("Prebuilt Release")]
#endif
[assembly: AssemblyCompany("Syncfusion, Inc.")]
[assembly: AssemblyProduct("Syncfusion Essential Studio")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

#if !SILVERLIGHT
[assembly: XmlnsPrefix("http://schemas.syncfusion.com/wpf", "syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Collections")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.ComponentModel")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Controls.Cells")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Controls.Scroll")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Controls.VirtualTreeView")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Diagnostics")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.GridCommon")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Styles")]


// Specifies the location in which theme dictionaries are stored for types in an assembly.
[assembly: ThemeInfo(
    // Specifies the location of system theme-specific resource dictionaries for this project.
    // The default setting in this project is "None" since this default project does not
    // include these user-defined theme files:
    //     Themes\Aero.NormalColor.xaml
    //     Themes\Classic.xaml
    //     Themes\Luna.Homestead.xaml
    //     Themes\Luna.Metallic.xaml
    //     Themes\Luna.NormalColor.xaml
    //     Themes\Royale.NormalColor.xaml
    ResourceDictionaryLocation.None,

    // Specifies the location of the system non-theme specific resource dictionary:
    //     Themes\generic.xaml
    ResourceDictionaryLocation.SourceAssembly)]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//

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
//[assembly: AssemblyKeyName("")]

// Neutral resources language for assembly.
[assembly: NeutralResourcesLanguageAttribute("en-US")]

namespace Syncfusion
{
    /// <summary>
    /// This class holds the name of the Syncfusion.Windows.Grid assembly and provides a helper 
    /// routine that helps with resolving types when loading a serialization stream and when 
    /// the framework probes for assemblies by reflection. 
    /// </summary>
    public class GridCommonSilverlightAssembly
    {
        /// <summary>
        /// The full name of this assembly without version information: "Syncfusion.Grid.Silverlight"
        /// </summary>
        public static readonly string Name;

        /// <summary>
        /// A reference to the <see cref="System.Reflection.Assembly"/> for the Grid assembly.
        /// </summary>
        public static readonly Assembly Assembly;


        /// <summary>
        /// The root namespace of this assembly. Used internally for locating resources within the assembly.
        /// </summary>
        public static readonly string RootNamespace = "Syncfusion.Windows.GridCommon";

        static GridCommonSilverlightAssembly()
        {
            Assembly = typeof(GridCommonSilverlightAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

    }
}

namespace Syncfusion.Windows.GridCommon
{
    internal class AssemblyInfo : Syncfusion.GridCommonSilverlightAssembly
    {
    }
}
