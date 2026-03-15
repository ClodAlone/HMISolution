// <copyright file="AssemblyInfo.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using Syncfusion.Documentation;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.Diagram.Wpf")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("syncfusion")]
[assembly: AssemblyProduct("Syncfusion.Diagram.Wpf")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

//// Setting ComVisible to false makes the types in this assembly not visible 
//// to COM components.  If you need to access a type in this assembly from 
//// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(true)]

////In order to begin building localizable applications, set 
////<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
////inside a <PropertyGroup>.  For example, if you are using US english
////in your source files, set the <UICulture> to en-US.  Then uncomment
////the NeutralResourceLanguage attribute below.  Update the "en-US" in
////the line below to match the UICulture setting in the project file.

////[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

#if SyncfusionFramework3_5
[assembly: System.Security.AllowPartiallyTrustedCallers]
#endif

[assembly: XmlnsPrefix("http://schemas.syncfusion.com/wpf", "syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Diagram")]
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ////where theme specific resource dictionaries are located
    ////(used if a resource is not found in the page, 
    //// or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly)]
    ////where the generic resource dictionary is located
    ////(used if a resource is not found in the page, 
    //// app, or any theme specific resource dictionaries)

//// Version information for an assembly consists of the following four values:
////
////      Major Version
////      Minor Version 
////      Build Number
////      Revision
////
//// You can specify all the values or you can default the Build and Revision Numbers 
//// by using the '*' as shown below:
//// [assembly: AssemblyVersion("1.0.*")]
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

[assembly: CLSCompliant(true)]

namespace Syncfusion
{
    /// <summary>
    /// Represents the DiagramWPF assembly
    /// </summary>
    public class DiagramWPFAssembly
    {
        /// <summary>
        /// Used to store the name.
        /// </summary>
        public static readonly string Name;

        /// <summary>
        /// Used to store the assembly.
        /// </summary>
        public static readonly Assembly Assembly;

        /// <summary>
        /// Used to store the root namespace.
        /// </summary>
        public static readonly string RootNamespace = "Syncfusion.Windows.Diagram";

        /// <summary>
        /// Initializes static members of the <see cref="DiagramWPFAssembly"/> class.
        /// </summary>
        static DiagramWPFAssembly()
        {
            Assembly = typeof(DiagramWPFAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

        /// <summary>
        /// Assemblies the resolver.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns>The assembly</returns>
        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            if (e.Name.StartsWith(DiagramWPFAssembly.Name))
            {
                return DiagramWPFAssembly.Assembly;
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
    /// Represents the Assembly info
    /// </summary>
    internal class AssemblyInfo : DiagramWPFAssembly
    {
    }
}

namespace Syncfusion.Licensing
{
    /// <summary>
    /// Checking whether partial trust allowed or not.
    /// </summary>
    public class EnvironmentTestDiagramWPF
    {
        #region PartialTrust Environment Testcode
        /// <summary>
        /// Gets a value indicating whether security permission can be granted. Read-only.
        /// </summary>
        public static bool IsSecurityGranted
        {
            get
            {
                System.Security.Permissions.SecurityPermission perm = new System.Security.Permissions.SecurityPermission(System.Security.Permissions.PermissionState.Unrestricted);
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
