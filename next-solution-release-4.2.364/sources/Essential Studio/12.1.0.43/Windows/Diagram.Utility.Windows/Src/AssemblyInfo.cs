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
using System.Security;
using System.Security.Permissions;
using System.Runtime.Serialization;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Syncfusion.Diagram.Utility.Windows")]
#if DEBUG
[assembly: AssemblyDescription("Debug")]
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyDescription("RELEASE")]
[assembly: AssemblyConfiguration("RELEASE")]
#endif
[assembly: AssemblyCompany("Syncfusion Inc.")]
[assembly: AssemblyProduct("Essential Diagram")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]

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


namespace Syncfusion
{
	/// <summary>
	/// This class holds the name of the Syncfusion.Diagram.Windows assembly and provides a helper 
	/// routine that helps with resolving types when loading a serialization stream and when 
	/// the framework probes for assemblies by reflection. 
	/// </summary>
	public class DiagramUtilityAssembly
	{
		/// <summary>
		/// The full name of this assembly without version information: "Syncfusion.Diagram.Windows"
		/// </summary>
		public static readonly string Name;

		/// <summary>
		/// A reference to the <see cref="System.Reflection.Assembly"/> for the grid assembly.
		/// </summary>
		public static readonly Assembly Assembly;


		/// <summary>
		/// The root namespace of this assembly. Used internally for locating resources within the assembly.
		/// </summary>
		public static readonly string RootNamespace = "Syncfusion.Windows.Forms.Diagram";

		static DiagramUtilityAssembly()
		{
			Assembly = typeof(DiagramUtilityAssembly).Assembly;
			string s = Assembly.FullName;
			int n = s.IndexOf(",");
			Name = s.Substring(0, n);
		}
        /// <summary>
        /// Assemblies the resolver.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
		public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
		{
			if (e.Name.StartsWith(DiagramBaseAssembly.Name))
				return DiagramBaseAssembly.Assembly;
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

		/// The Binder class helps with the resolution of Diagram types between different versions of 
		/// the Syncfusion assemblies.
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
			public class Binder : SerializationBinder
		{
            /// <summary>
            /// When overridden in a derived class, controls the binding of a serialized object to a type.
            /// </summary>
            /// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly"/> name of the serialized object.</param>
            /// <param name="typeName">Specifies the <see cref="T:System.Type"/> name of the serialized object.</param>
            /// <returns>
            /// The type of the object the formatter creates a new instance of.
            /// </returns>
			public override Type BindToType(
				string assemblyName, string typeName)
			{
				Type t = Type.GetType(typeName);
				if(t != null)
					return t;

				foreach (Assembly asm in new Assembly[] { 
															Syncfusion.DiagramBaseAssembly.Assembly
														})
				{
					t = asm.GetType(typeName);
					if(t != null)
						return t;
				}		

				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int n = 0; n < assemblies.Length; n++)
				{
					t = assemblies[n].GetType(typeName);
					if(t != null)
						return t;
				}

				return null;
			}
		}
	}

	internal class AssemblyInfo : DiagramUtilityAssembly
	{
	}
}
