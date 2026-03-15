// <copyright file="AssemblyInfo.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region Using directives

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Resources;
using System.Globalization;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.Chart.MVVM.WPF")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Syncfusion.Chart.MVVM.WPF")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

////In order to begin building localizable applications, set 
////<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
////inside a <PropertyGroup>.  For example, if you are using US english
////in your source files, set the <UICulture> to en-US.  Then uncomment
////the NeutralResourceLanguage attribute below.  Update the "en-US" in
////the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
#if SyncfusionFramework3_5
[assembly: System.Security.AllowPartiallyTrustedCallers]
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








////
//// In order to sign your assembly, you must specify a key to use. Refer to the 
//// Microsoft .NET Framework documentation for more information on assembly signing.
////
//// Use the following below to control which key is used for signing:
////
//// Notes: 
////   (*) If no key is specified, the assembly is not signed.
////   (*) KeyName refers to a key that has been installed in the Crypto Service
////       Provider (CSP) on your machine. KeyFile refers to a file which contains
////       a key.
////   (*) If the KeyFile and the KeyName values are both specified, the 
////       following processing occurs:
////       (1) If the KeyName can be found in the CSP, that key is used.
////       (2) If the KeyName does not exist and the KeyFile does exist, the key 
////           in the KeyFile is installed into the CSP and used.
////   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
////       When specifying the KeyFile, the location of the KeyFile should be
////       relative to the project output directory which is
////       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
////       located in the project directory, you would specify the AssemblyKeyFile 
////       attribute as [assembly: AssemblyKeyFile(@"c:\quick\Common\Keys\sf.publicsnk")]
////   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
////       documentation for more information on this.
////
//[assembly: AssemblyDelaySign(true)]
//[assembly: AssemblyKeyFile("../../../../../Common/keys/sf.publicsnk")]


//[assembly: AssemblyKeyName("")]
[assembly: XmlnsPrefix("http://schemas.syncfusion.com/wpf", "syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Chart")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Chart.MVVM")]

namespace Syncfusion
{
    /// <summary>
    /// Represents ChartWPFAssembly
    /// </summary>
    public class ChartMVVMWPFAssembly
    {
        /// <summary>
        /// Initializes Name
        /// </summary>
        public static readonly string Name;

        /// <summary>
        /// Initializes Assembly
        /// </summary>
        public static readonly Assembly Assembly;

        /// <summary>
        /// Initializes RootNamespace
        /// </summary>
        public static readonly string RootNamespace = "Syncfusion.Windows.Chart.MVVM";

        /// <summary>
        /// Initializes static members of the <see cref="ChartWPFAssembly"/> class.
        /// </summary>
        static ChartMVVMWPFAssembly()
        {
            Assembly = typeof(ChartMVVMWPFAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

        /// <summary>
        /// Assemblies the resolver.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns>The Assembly</returns>
        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            if (e.Name.StartsWith(SharedBaseAssembly.Name))
            {
                return SharedBaseAssembly.Assembly;
            }
            else if (e.Name.StartsWith(ChartMVVMWPFAssembly.Name))
            {
                return ChartMVVMWPFAssembly.Assembly;
            }
            else if (e.Name.StartsWith(ChartWPFAssembly.Name))
            {
                return ChartWPFAssembly.Assembly;
            }
            else
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int n = 0; n < assemblies.Length; n++)
                {
                    if (assemblies[n].GetName().Name == e.Name)
                    {
                        return assemblies[n];
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Represents AssemblyInfo
    /// </summary>
    internal class AssemblyInfo : ChartMVVMWPFAssembly
    {
    }
}
#endregion