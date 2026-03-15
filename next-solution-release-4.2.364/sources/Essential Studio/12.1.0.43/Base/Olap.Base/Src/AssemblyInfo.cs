#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if SyncfusionFramework4_0
[assembly: SecurityRules(SecurityRuleSet.Level1)]
#endif

[assembly: AssemblyTitle("Syncfusion.Olap.Base")]
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
[assembly: ComVisible(false)]
[assembly: AllowPartiallyTrustedCallers()]

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

















#pragma warning disable 1699 // disable warning for "Use command line option '/keyfile' ...

[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyKeyFile(@"../../../../../Common/keys/sf.publicsnk")]
[assembly: AssemblyKeyName("")]

[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Olap.Manager")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Olap.Reports")]

// Neutral resources language for assembly.
[assembly: NeutralResourcesLanguageAttribute("en-US")]

namespace Syncfusion
{
    /// <summary>
    /// Model Assembly.
    /// </summary>
    public class ModelAssembly
    {

        /// <summary>
        /// Assembly member of type <see cref="Assembly"/>.
        /// </summary>
        public static readonly Assembly Assembly;
        /// <summary>
        /// Name of the <see cref="ModelAssembly"/>.
        /// </summary>
        public static readonly string Name;

        /// <summary>
        /// Namespace of the <see cref="ModelAssembly"/>
        /// </summary>
        public static readonly string RootNamespace = "Syncfusion.Olap";

        /// <summary>
        /// Initializes the <see cref="ModelAssembly"/> class.
        /// </summary>
        static ModelAssembly()
        {
            Assembly = typeof(ModelAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

        /// <summary>
        /// Assemblies the resolver.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns>An object of type <see cref="Assembly"/>.</returns>
        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            if (e.Name.StartsWith(Syncfusion.CoreAssembly.Name))
                return Syncfusion.CoreAssembly.Assembly;
            else if (e.Name.StartsWith(ModelAssembly.Name))
                return ModelAssembly.Assembly;
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

    /// <summary>
    /// Represents the Assembly information.
    /// </summary>
    internal class AssemblyInfo : ModelAssembly
    {
    }
}
