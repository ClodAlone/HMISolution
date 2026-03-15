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
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.ReportWriter.Base")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Syncfusion, Inc.")]
[assembly: AssemblyProduct("Syncfusion Essential Studio")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: InternalsVisibleTo("Syncfusion.ReportViewer.Wpf")]
[assembly: InternalsVisibleTo("Syncfusion.ReportViewer.Mvc")]
//[assembly: InternalsVisibleTo("Syncfusion.ReportViewer.Wpf, PublicKey=00240000048000009400000006020000002400005253413100040000010001002382FCB1069523CE72D849497A557A445C151EAF4007AA79ADEF551A8204CA7F728E5378607D85695B16F129EC35BF4AF15DCF6D3581DEB8BB0DEBB239C33E7F1271A37C7F60F1044AE417730F5082ABEE5F9EC568A8A4CEF04074394755706376E982DC6F9D15430FAAAD385AE8F00A77EF1C97517F1A1517004CE78028B9CE")]
//[assembly: InternalsVisibleTo("Syncfusion.ReportViewer.Mvc, PublicKey=00240000048000009400000006020000002400005253413100040000010001002382FCB1069523CE72D849497A557A445C151EAF4007AA79ADEF551A8204CA7F728E5378607D85695B16F129EC35BF4AF15DCF6D3581DEB8BB0DEBB239C33E7F1271A37C7F60F1044AE417730F5082ABEE5F9EC568A8A4CEF04074394755706376E982DC6F9D15430FAAAD385AE8F00A77EF1C97517F1A1517004CE78028B9CE")]

//#pragma warning disable 1699 // disable warning for "Use command line option '/keyfile' ...

//// In order to begin building localizable applications, set 
//// <UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//// inside a <PropertyGroup>.  For example, if you are using US english
//// in your source files, set the <UICulture> to en-US.  Then uncomment
//// the NeutralResourceLanguage attribute below.  Update the "en-US" in
//// the line below to match the UICulture setting in the project file.

//// [assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //// (used if a resource is not found in the page, 
    //// or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //// (used if a resource is not found in the page, 
    //// app, or any theme specific resource dictionaries)
)]

#if SyncfusionFramework4_0
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]
#endif

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
//[assembly: AssemblyKeyFile(@"../../../../../Common/keys/sf.publicsnk")]
