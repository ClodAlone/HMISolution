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
using System.Security.Permissions;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Syncfusion.SfTreeNavigator.WPF")]
[assembly: AssemblyProduct("Syncfusion.SfTreeNavigator.WPF")]

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


[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Syncfusion Inc")]
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
////the line below to match the UICulture setting in the project file.

[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

#if SyncfusionFramework3_5
[assembly: System.Security.AllowPartiallyTrustedCallers]
#endif

[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Controls.Navigation")]
[assembly: XmlnsPrefix("http://schemas.syncfusion.com/wpf", "syncfusion")]

#if SyncfusionFramework3_5
[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "wpf")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Syncfusion.Windows")]
#endif
//// Version information for an assembly consists of the following four values:
////
////     Major Version
////      Minor Version 
////      Build Number
////      Revision
////
// You can specify all the values or you can default the Revision and Build Numbers 
//// by using the '*' as shown below:
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
//[assembly: AssemblyKeyFile(@"..\..\..\..\..\Common\Keys\sf.publicsnk")]

//[assembly: AssemblyKeyName("")]

namespace Syncfusion
{
    /// <summary>
    /// SharedBase assembly class.
    /// </summary>
    public class SfTreeNavigatorBaseAssembly
    {
        /// <summary>
        /// Name of the assembly.
        /// </summary>
        public static readonly string Name;

        /// <summary>
        /// Defines assembly object reference variable.
        /// </summary>
        public static readonly Assembly Assembly;

        /// <summary>
        /// Root namespace of the assembly.
        /// </summary>
        public static readonly string RootNamespace = "Syncfusion.Windows.Controls.Navigation";

        /// <summary>
        /// Initializes static members of the <see cref="SfTreeNavigatorBaseAssembly"/> class.
        /// </summary>
        static SfTreeNavigatorBaseAssembly()
        {
            Assembly = typeof(SfTreeNavigatorBaseAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

        /// <summary>
        /// Assemblies the resolver.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns>Assembly object.</returns>
        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            if (e.Name.StartsWith(SfTreeNavigatorBaseAssembly.Name))
            {
                return SfTreeNavigatorBaseAssembly.Assembly;
            }
            else if (e.Name.StartsWith(CoreAssembly.Name))
            {
                return CoreAssembly.Assembly;
            }
            else
            {
                string name = e.Name.ToLower();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int n = 0; n < assemblies.Length; n++)
                {
                    if (assemblies[n].GetName().Name.ToLower() == name)
                    {
                        return assemblies[n];
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Assembly info class
    /// </summary>
    internal class AssemblyInfo : SfTreeNavigatorBaseAssembly
    {
    }
}

namespace Syncfusion.Licensing
{
    /// <summary>
    /// Checking whether partial trust allowed or not.
    /// </summary>
    public class EnvironmentTestSfTreeNavigation
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

