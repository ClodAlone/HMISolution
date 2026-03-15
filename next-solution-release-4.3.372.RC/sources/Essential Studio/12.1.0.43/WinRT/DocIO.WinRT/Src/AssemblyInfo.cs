#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
//
#endregion

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
#if WINRT
using System.Windows;
using System.Security;
#else
using System.Windows.Media;
using System.Security;
using System.Security.Permissions;
#endif
using System.ComponentModel;
//
// General information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle( "Syncfusion.DocIO.WinRT" )]
#if DEBUG
[assembly: AssemblyDescription( "Debug" )]
[assembly: AssemblyConfiguration( "DEBUG" )]
#else
[assembly: AssemblyDescription("Prebuilt Release")]
[assembly: AssemblyConfiguration("Prebuilt Release")]
#endif
[assembly: AssemblyCompany( "Syncfusion Inc." )]
[assembly: AssemblyProduct( "Syncfusion Essential Suite" )]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]
[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Syncfusion.SfRichTextBoxAdv.WinRT, PublicKey=00240000048000009400000006020000002400005253413100040000010001002382fcb1069523ce72d849497a557a445c151eaf4007aa79adef551a8204ca7f728e5378607d85695b16f129ec35bf4af15dcf6d3581deb8bb0debb239c33e7f1271a37c7f60f1044ae417730f5082abee5f9ec568a8a4cef04074394755706376e982dc6f9d15430faaad385ae8f00a77ef1c97517f1a1517004ce78028b9ce")]
#if !WINRT
[assembly: SecurityPermission( SecurityAction.RequestMinimum )]
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






//
// In order to sign your assembly, you must specify a key to use. Refer to the
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below to control which key is used for signing.
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
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//


[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyKeyFile("../../../../../Common/keys/sf.publicsnk")]
[assembly: AssemblyKeyName("")]

// Neutral resources language for assembly.
[assembly: NeutralResourcesLanguageAttribute( "en-US" )]

namespace Syncfusion.Documentation
{
    [DocumentationExclude]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = true)]
    public class DocumentationExcludeAttribute : Attribute
    {
        public DocumentationExcludeAttribute()
        { }
    }
}