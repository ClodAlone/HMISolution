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
using System.Diagnostics;
using System.Reflection;
using System.Security;
//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
#if !SyncfusionFramework4_0
[assembly: AllowPartiallyTrustedCallers()]
#endif
[assembly: AssemblyTitle( "Syncfusion.Scripting.Windows" ) ]
#if DEBUG
[assembly: AssemblyDescription( "Debug" ) ]
[assembly: AssemblyConfiguration( "DEBUG" ) ]
#else
[assembly: AssemblyDescription( "Prebuilt Release" ) ]
[assembly: AssemblyConfiguration( "Prebuilt Release" ) ]
#endif
[assembly: AssemblyCompany( "Syncfusion, Inc." ) ]
[assembly: AssemblyProduct( "Essential Scripting" ) ]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark( "" ) ]
[assembly: AssemblyCulture( "" ) ]
[assembly: CLSCompliant( true ) ]

//
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
































//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
[assembly: AssemblyDelaySign(true) ]
[assembly: AssemblyKeyFile("../../../../../Common/keys/sf.publicsnk")]
[assembly: AssemblyKeyName("") ]
