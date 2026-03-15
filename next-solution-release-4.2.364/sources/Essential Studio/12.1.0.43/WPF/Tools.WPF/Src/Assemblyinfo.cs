// <copyright file="AssemblyInfo.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region Using directives
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using System.Security.Permissions;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.Tools.Wpf")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Syncfusion Inc")]
[assembly: AssemblyProduct("Syncfusion.Tools.Wpf-Syncfusion.Tools.Wpf.resources")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

////In order to begin building localizable applications, set 
////<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
////inside a <PropertyGroup>.  For example, if you are using US english
////in your source files, set the <UICulture> to en-US.  Then uncomment
////the NeutralResourceLanguage attribute below.  Update the "en-US" in
////the line below to match the UICulture setting in the project file..

[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

#if SyncfusionFramework3_5
[assembly: System.Security.AllowPartiallyTrustedCallers]
#endif

[assembly: XmlnsPrefix("http://schemas.syncfusion.com/wpf", "syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Tools.Controls")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Tools")]


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
    ResourceDictionaryLocation.SourceAssembly,

    // Specifies the location of the system non-theme specific resource dictionary:
    //     Themes\generic.xaml
    ResourceDictionaryLocation.SourceAssembly)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
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















// In order to sign your assembly, you must specify a key to use. Refer to the 
// Microsoft .NET Framework documentation for more information on assembly signing.
// Use the following below to control which key is used for signing:
// Notes: 
// (*) If no key is specified, the assembly is not signed.
// (*) KeyName refers to a key that has been installed in the Crypto Service
// Provider (CSP) on your machine. KeyFile refers to a file which contains
// a key.
// (*) If the KeyFile and the KeyName values are both specified, the 
// following processing occurs:
// (1) If the KeyName can be found in the CSP, that key is used.
// (2) If the KeyName does not exist and the KeyFile does exist, the key 
// in the KeyFile is installed into the CSP and used.
// (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
// When specifying the KeyFile, the location of the KeyFile should be
// relative to the project output directory which is
// %Project Directory%\obj\<configuration>. For example, if your KeyFile is
// located in the project directory, you would specify the AssemblyKeyFile 
// attribute as [assembly: AssemblyKeyFile(@"c:\quick\Common\Keys\sf.publicsnk")]
// (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
// documentation for more information on this.
//[assembly: AssemblyDelaySign(true)]

//[assembly: AssemblyKeyFile(@"..\..\..\..\..\Common\Keys\sf.publicsnk")]

//[assembly: AssemblyKeyName("")]

namespace Syncfusion
{
    /// <summary>
    /// Represents the Tools WPF Assembly
    /// </summary>
    public class ToolsWPFAssembly
    {
        /// <summary>
        /// Represents the Assembly Name
        /// </summary>
        public static readonly string Name;

        /// <summary>
        /// Represents the Assembly
        /// </summary>
        public static readonly Assembly Assembly;

        /// <summary>
        /// Represents the Root Name Space
        /// </summary>
        public static readonly string RootNamespace = "Syncfusion.Tools.WPF";

        /// <summary>
        /// Initializes static members of the <see cref="ToolsWPFAssembly"/> class.
        /// </summary>
        static ToolsWPFAssembly()
        {
            Assembly = typeof(ToolsWPFAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

        /// <summary>
        /// Assemblies the resolver.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns>returns the assembly</returns>
        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            if (e.Name.StartsWith(SharedBaseAssembly.Name))
            {
                return SharedBaseAssembly.Assembly;
            }
            else if (e.Name.StartsWith(ToolsWPFAssembly.Name))
            {
                return ToolsWPFAssembly.Assembly;
            }
            else if (e.Name.StartsWith(CoreAssembly.Name))
            {
                return CoreAssembly.Assembly;
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
    /// Represents the Assembly Info class
    /// </summary>
    internal class AssemblyInfo : ToolsWPFAssembly
    {
    }
}
namespace Syncfusion.Licensing
{
    /// <summary>
    /// Checking whether partial trust allowed or not.
    /// </summary>
    public class EnvironmentTestTools
    {
        #region PartialTrust Environment Testcode
        /// <summary>
        /// Gets a value indicating whether security permission can be granted. Read-only.
        /// </summary>
        public static bool IsSecurityGranted
        {
            get
            {
                SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
                bool bResult = false;
                try
                {
                    perm.Demand();
                    bResult = true;
                }
                catch (Exception)
                {
                }

                return bResult;
            }
        }

        /// <summary>
        /// Validates the license.
        /// </summary>
        /// <param name="controltype">The control type.</param>
        public static void ValidateLicense(Type controltype)
        {
            if (IsSecurityGranted)
            {
                StartValidateLicense(controltype);
            }
        }

        /// <summary>
        /// Starts the validate license.
        /// </summary>
        /// <param name="controltype">The controltype.</param>
        public static void StartValidateLicense(Type controltype)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);

                new Syncfusion.Core.Licensing.LicensedComponent(controltype);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }
        #endregion
    }
}
