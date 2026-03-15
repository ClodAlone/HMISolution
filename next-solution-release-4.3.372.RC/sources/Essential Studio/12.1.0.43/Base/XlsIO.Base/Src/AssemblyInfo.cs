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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Syncfusion.Documentation;
using System.Security;
using System.Security.Permissions;

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
#if !ClientProfile
[assembly: AssemblyTitle("Syncfusion.XlsIO.Base")]
#else
[assembly: AssemblyTitle("Syncfusion.XlsIO.Base")]
#endif
#if !SyncfusionFramework4_0
[assembly: AllowPartiallyTrustedCallers()]
#endif
#if DEBUG
[assembly: AssemblyDescription("Debug")]
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyDescription("Prebuilt Release")]
[assembly: AssemblyConfiguration("Prebuilt Release")]
#endif
[assembly: AssemblyCompany("Syncfusion, Inc.")]
[assembly: AssemblyProduct("Essential XlsIO-Syncfusion.XlsIO.ClientProfile-Syncfusion.XlsIO.Base")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: DocumentationNamespaceExclude("Syncfusion,Syncfusion.XlsIO.Collections,Syncfusion.XlsIO.Implementation,Syncfusion.XlsIO.Implementation.Charts,Syncfusion.XlsIO.Implementation.Clipboard,Syncfusion.XlsIO.Implementation.Collections,Syncfusion.XlsIO.Implementation.Exceptions,Syncfusion.XlsIO.Interfaces,Syncfusion.XlsIO.Interfaces.Collections,Syncfusion.XlsIO.IO,Syncfusion.XlsIO.IO.Stream,Syncfusion.XlsIO.IO.Stream.Win32,Syncfusion.XlsIO.Parser,Syncfusion.XlsIO.Parser.Biff_Records,Syncfusion.XlsIO.Parser.Biff_Records.Charts,Syncfusion.XlsIO.Parser.Biff_Records.Formula,Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing,Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords,Syncfusion.XlsIO.Implementation.Shapes,Syncfusion.XlsIO.Implementation.ZLib,Syncfusion.XlsIO.Implementation.ZLib.Exceptions,Syncfusion.XlsIO.Implementation.ZLib.Interfaces,Syncfusion.XlsIO.Parser.Biff_Records.PivotTable,Syncfusion.XlsIO.Implementation.Collections.Grouping,Syncfusion.XlsIO.Implementation.PivotTables,Syncfusion.XlsIO.FormatParser.FormatTokens,Syncfusion.XlsIO.FormatParser,Syncfusion.XlsIO.Interfaces.XmlSerialization,Syncfusion.XlsIO.Implementation.XmlSerialization,Syncfusion.XlsIO.Implementation.XmlReaders,Syncfusion.XlsIO.Implementation.TemplateMarkers,Syncfusion.XmlSerializersCreator")]
//[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Syncfusion.ExcelToPdfConverter.Base, PublicKey=00240000048000009400000006020000002400005253413100040000010001002382fcb1069523ce72d849497a557a445c151eaf4007aa79adef551a8204ca7f728e5378607d85695b16f129ec35bf4af15dcf6d3581deb8bb0debb239c33e7f1271a37c7f60f1044ae417730f5082abee5f9ec568a8a4cef04074394755706376e982dc6f9d15430faaad385ae8f00a77ef1c97517f1a1517004ce78028b9ce")]
//[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Syncfusion.ExcelToPdfConverter.ClientProfile, PublicKey=00240000048000009400000006020000002400005253413100040000010001002382fcb1069523ce72d849497a557a445c151eaf4007aa79adef551a8204ca7f728e5378607d85695b16f129ec35bf4af15dcf6d3581deb8bb0debb239c33e7f1271a37c7f60f1044ae417730f5082abee5f9ec568a8a4cef04074394755706376e982dc6f9d15430faaad385ae8f00a77ef1c97517f1a1517004ce78028b9ce")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Syncfusion.ExcelToPdfConverter.Base")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Syncfusion.ExcelToPdfConverter.ClientProfile")]

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
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
//[assembly: AssemblyDelaySign(true)]
//[assembly: AssemblyKeyFile("../../../../../Common/keys/sf.publicsnk")]
//[assembly: AssemblyKeyName("")]

[assembly: CLSCompliant(true)]
//[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true)]
//[assembly: PermissionSet( SecurityAction.RequestMinimum )]
//[assembly: ComVisible(true)]

namespace Syncfusion
{
  ///<exclude/>
  /// <summary>
  /// This class holds the name of the Syncfusion.XlsIO.Base assembly and provides a helper 
  /// routine that helps with resolving types when loading a serialization stream and when 
  /// the framework probes for assemblies by reflection. 
  /// </summary>
  public class XlsIOBaseAssembly
  {
    /// <summary>
    /// The full name of this assembly without version information: "Syncfusion.XlsIO"
    /// </summary>
    public static readonly string Name;

    /// <summary>
    /// A reference to the <see cref="System.Reflection.Assembly"/> for the XlsIO assembly.
    /// </summary>
    public static readonly Assembly Assembly;

    static XlsIOBaseAssembly()
    {
      Assembly = typeof( XlsIOBaseAssembly ).Assembly;
      string s = Assembly.FullName;
      int n = s.IndexOf( "," );
      Name = s.Substring( 0, n );
    }

    /// <summary>
    /// The root namespace of this assembly. Used internally for locating resources within the assembly.
    /// </summary>
    public static readonly string RootNamespace = "Syncfusion.XlsIO";

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
    ///     public static GridModel LoadSoap(Stream s)
    ///     {
    ///       try
    ///       {
    ///         AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(GridAssembly.AssemblyResolver);
    ///         SoapFormatter b = new SoapFormatter();
    ///         b.AssemblyFormat = FormatterAssemblyStyle.Simple;
    ///         GridModel t = b.Deserialize(s) as GridModel;
    ///         t.Modified = false;
    ///         return t;
    ///       }
    ///       finally
    ///       {
    ///         AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(GridAssembly.AssemblyResolver);
    ///       }
    ///     }
    /// </code>
    /// </example>
 
    public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
    {
      if (e.Name.StartsWith(CoreAssembly.Name))
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

  internal class AssemblyInfo : XlsIOBaseAssembly
  {
  }
}
