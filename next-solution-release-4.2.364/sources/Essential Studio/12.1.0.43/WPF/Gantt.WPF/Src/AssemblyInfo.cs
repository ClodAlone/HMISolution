#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

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
[assembly: AssemblyTitle("Syncfusion.Gantt.WPF")]
[assembly: AssemblyCompany("Syncfusion, Inc.")]
[assembly: AssemblyProduct("Syncfusion Essential Studio")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
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


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision


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








[assembly: XmlnsPrefix("http://schemas.syncfusion.com/wpf", "syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Controls.Gantt")]
//#pragma warning disable 1699 // disable warning for "Use command line option '/keyfile' ...

//[assembly: AssemblyDelaySign(true)]
//[assembly: AssemblyKeyFile(@"..\..\..\..\..\Common\Keys\sf.publicsnk")]
//[assembly: AssemblyKeyName("")]

namespace Syncfusion
{
    /// <summary>
    /// This class holds the name of the Syncfusion.Windows.Controls.Gantt assembly and provides a helper 
    /// routine that helps with resolving types when loading a serialization stream and when 
    /// the framework probes for assemblies by reflection. 
    /// </summary>
    public class GanttWpfAssembly
    {
        /// <summary>
        /// The full name of this assembly without version information: "Syncfusion.Gantt.Wpf"
        /// </summary>
        public static readonly string Name;

        /// <summary>
        /// A reference to the <see cref="System.Reflection.Assembly"/> for the Gantt assembly.
        /// </summary>
        public static readonly Assembly Assembly;


        /// <summary>
        /// The root namespace of this assembly. Used internally for locating resources within the assembly.
        /// </summary>
        public static readonly string RootNamespace = "Syncfusion.Windows.Controls.Gantt";

        static GanttWpfAssembly()
        {
            Assembly = typeof(GanttWpfAssembly).Assembly;
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

namespace Syncfusion.Windows.Controls.Gantt
{
    internal class AssemblyInfo : Syncfusion.GanttWpfAssembly
    {
    }
}