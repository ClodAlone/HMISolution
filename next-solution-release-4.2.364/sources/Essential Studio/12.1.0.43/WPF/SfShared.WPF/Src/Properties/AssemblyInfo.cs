#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Reflection;
using System.Resources;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Security.Permissions;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.SfShared.Wpf")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Syncfusion.SfShared.Wpf")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

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









//[assembly: AssemblyDelaySign(true)]
//[assembly: AssemblyKeyFile(@"..\..\..\..\..\Common\Keys\sf.publicsnk")]

//[assembly: AssemblyKeyName("")]

/// <summary>
/// Represents a class for the Reference attributes
/// </summary>
public class ClassReferenceAttribute : Attribute
{
    private bool isReviewed = false;

    /// <summary>
    /// Returns a value when set
    /// </summary>
    /// <value>
    /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
    /// </value>
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

    /// <summary>
    /// Returns a value when set
    /// </summary>
    /// <value>
    /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
    /// </value>
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

namespace Syncfusion
{
    /// <summary>
    /// SharedBase assembly class.
    /// </summary>
    public class SharedBaseAssembly
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
        public static readonly string RootNamespace = "Syncfusion.Windows.Shared";

        /// <summary>
        /// Initializes static members of the <see cref="SharedBaseAssembly"/> class.
        /// </summary>
        static SharedBaseAssembly()
        {
            Assembly = typeof(SharedBaseAssembly).Assembly;
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
            if (e.Name.StartsWith(SharedBaseAssembly.Name))
            {
                return SharedBaseAssembly.Assembly;
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
    internal class AssemblyInfo : SharedBaseAssembly
    {
    }
}

namespace Syncfusion.Licensing
{
    /// <summary>
    /// Checking whether partial trust allowed or not.
    /// </summary>
    public class EnvironmentTest
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