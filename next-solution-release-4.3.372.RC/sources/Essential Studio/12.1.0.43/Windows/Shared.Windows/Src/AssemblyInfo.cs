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
using System.Security;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

using Syncfusion.Documentation;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
#if !SyncfusionFramework4_0
[assembly: AllowPartiallyTrustedCallers]
#endif
[assembly: AssemblyTitle("Syncfusion.Shared.Windows")]
#if DEBUG
[assembly: AssemblyDescription("Debug")]
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyDescription("Prebuilt Release")]
[assembly: AssemblyConfiguration("Prebuilt Release")]
#endif
[assembly: AssemblyCompany("Syncfusion, Inc.")]
[assembly: AssemblyProduct("Syncfusion Essential Suite")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]	
[assembly: DocumentationNamespaceExclude("Syncfusion.ComponentModel.Design.Serialization,Syncfusion.Documentation,Syncfusion.Reflection,Syncfusion.Runtime.InteropServices,Syncfusion.Win32,Syncfusion.Windows.Forms.Localization")]	

[assembly: CLSCompliant(true)]
#pragma warning disable
[assembly:SecurityPermissionAttribute(SecurityAction.RequestMinimum, UnmanagedCode=true)]
[assembly:PermissionSetAttribute(SecurityAction.RequestMinimum, Name = "FullTrust")]
#pragma warning enable
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


































[assembly: SatelliteContractVersion("1.1.0.0")]

//
// In order to sign your assembly you must specify a key to use. Refer to the 
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
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.publicsnk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//

[assembly: AssemblyDelaySign(true)]

[assembly: AssemblyKeyFile("../../../../../Common/keys/sf.publicsnk")]

[assembly: AssemblyKeyName("")]


// Neutral resources language for assembly.
[assembly: NeutralResourcesLanguageAttribute("en-US")]


namespace Syncfusion
{
    /// <exclude/>
	/// <summary>
	/// This class holds the name of the Syncfusion.Shared.Windows assembly and provides a helper 
	/// routine that helps with resolving types when loading a serialization stream and when 
	/// the framework probes for assemblies by reflection. 
	/// </summary>
	public class SharedWindowsAssembly
	{
		/// <summary>
		/// The full name of this assembly without version information: "Syncfusion.Shared.Windows"
		/// </summary>
		public static readonly string Name;

		/// <summary>
		/// A reference to the <see cref="System.Reflection.Assembly"/> for the grid assembly.
		/// </summary>
		public static readonly Assembly Assembly;


		/// <summary>
		/// The root namespace of this assembly. Used internally for locating resources within the assembly.
		/// </summary>
		public static readonly string RootNamespace = "Syncfusion.Windows.Forms";

		static SharedWindowsAssembly()
		{
			Assembly = typeof(SharedWindowsAssembly).Assembly;
			string s = Assembly.FullName;
			int n = s.IndexOf(",");
			Name = s.Substring(0, n);
		}
		
		/// <summary>
		/// This delegate helps with resolving types and can be used as a eventhandler
		/// for a <see cref="System.AppDomain.AssemblyResolve"/> event.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event data with information about the type.</param>
		/// <returns>A reference to the assembly where the type is located.</returns>
		/// <remarks>
		/// Use this handler when reading back types from a serialization stread
		/// saved with an earlier version of this assembly.
		/// </remarks>
		/// <example>
		/// <code lang="C#">
		/// 		public static GridModel LoadSoap(Stream s)
		/// 		{
		/// 			try
		/// 			{
		/// 				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(SharedWindowsAssembly.AssemblyResolver);
		/// 				SoapFormatter b = new SoapFormatter();
		/// 				b.AssemblyFormat = FormatterAssemblyStyle.Simple;
		/// 				GridModel t = b.Deserialize(s) as GridModel;
		/// 				t.Modified = false;
		/// 				return t;
		/// 			}
		/// 			finally
		/// 			{
		/// 				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(SharedWindowsAssembly.AssemblyResolver);
		/// 			}
		/// 		}
		/// 
		/// </code>
		/// </example>
		public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
		{
			if(e.Name.StartsWith(Syncfusion.CoreAssembly.Name))
				return Syncfusion.CoreAssembly.Assembly;
			else if (e.Name.StartsWith(SharedBaseAssembly.Name))
				return SharedBaseAssembly.Assembly;
			else if (e.Name.StartsWith(SharedWindowsAssembly.Name))
				return SharedWindowsAssembly.Assembly;
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
}

namespace Syncfusion.Windows.Forms
{
	internal class AssemblyInfo : SharedWindowsAssembly
	{
	}
}
