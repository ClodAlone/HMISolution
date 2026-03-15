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
using System.Resources;
using System.Globalization;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.OlapGrid.WPF")]
#if DEBUG
[assembly: AssemblyDescription("Debug")]
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyDescription("Prebuilt Release")]
[assembly: AssemblyConfiguration("Prebuilt Release")]
#endif
[assembly: AssemblyCompany("Syncfusion, Inc.")]
[assembly: AssemblyProduct("Syncfusion Essential BI")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]
[assembly: System.Security.AllowPartiallyTrustedCallers]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
#if SyncfusionFramework4_0
[assembly: SecurityRules(System.Security.SecurityRuleSet.Level1)]
#endif
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










[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyKeyFile(@"../../../../../Common/keys/sf.publicsnk")]
[assembly: AssemblyKeyName("")]

[assembly: XmlnsPrefix("http://schemas.syncfusion.com/wpf", "syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Grid.Olap")]


namespace Syncfusion
{
    public class OlapGridWPFAssembly
    {
        public static readonly string Name;

        public static readonly Assembly Assembly;

        public static readonly string RootNamespace = "Syncfusion.Windows.Grid.Olap";

        static OlapGridWPFAssembly()
        {
            Assembly = typeof(OlapGridWPFAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            if (e.Name.StartsWith(SharedBaseAssembly.Name))
                return SharedBaseAssembly.Assembly;
            else if (e.Name.StartsWith(OlapGridWPFAssembly.Name))
                return OlapGridWPFAssembly.Assembly;
            else if (e.Name.StartsWith(CoreAssembly.Name))
                return CoreAssembly.Assembly;
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

    internal class AssemblyInfo : OlapGridWPFAssembly
    {
    }
}