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
using System;
using System.Resources;
using System.Windows;
using Syncfusion;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.Olap.MVVM.WPF")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Syncfusion Software Inc")]
[assembly: AssemblyProduct("Syncfusion.Olap.MVVM.WPF")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("2abbcd2f-f8db-4dea-8ade-0af3f5687bf9")]
//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]
//[assembly: System.Security.AllowPartiallyTrustedCallers]

[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

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
























//
// In order to sign your assembly, you must specify a key to use. Refer to the 
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the following below to control which key is used for signing:
//
// Notes: 
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers to a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers to a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the 
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key 
//           in the KeyFile is installed into the CSP and used.
//   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative to the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile 
//       attribute as //[assembly: AssemblyKeyFile(@"c:\quick\Common\Keys\sf.publicsnk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//

[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyKeyFile(@"../../../../../Common/keys/sf.publicsnk")]

[assembly: AssemblyKeyName("")]
[assembly: XmlnsPrefix("http://schemas.syncfusion.com/wpf", "syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Olap.MVVM")]

namespace Syncfusion
{
    public class OlapMVVMWPFAssembly
    {

        public static readonly Assembly Assembly;
        public static readonly string Name;

        public static readonly string RootNamespace = "Syncfusion.Windows.Olap.MVVM";

        static OlapMVVMWPFAssembly()
        {
            Assembly = typeof(OlapMVVMWPFAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            if (e.Name.StartsWith(SharedBaseAssembly.Name))
                return SharedBaseAssembly.Assembly;
            else if (e.Name.StartsWith(OlapMVVMWPFAssembly.Name))
                return OlapMVVMWPFAssembly.Assembly;
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

    internal class AssemblyInfo : OlapMVVMWPFAssembly
    {
    }
}
