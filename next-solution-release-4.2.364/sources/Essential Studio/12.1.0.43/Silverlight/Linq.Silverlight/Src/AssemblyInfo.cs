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
using System;
using System.Windows.Markup;


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.Linq.Silverlight")]
#if DEBUG
[assembly: AssemblyDescription("Debug")]
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyDescription("Prebuilt Release")]
[assembly: AssemblyConfiguration("Prebuilt Release")]
#endif
[assembly: AssemblyCompany("Syncfusion, Inc.")]
[assembly: AssemblyProduct("Syncfusion Essential Studio")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: InternalsVisibleTo("Syncfusion.Grid.Silverlight, PublicKey=00240000048000009400000006020000002400005253413100040000010001002382FCB1069523CE72D849497A557A445C151EAF4007AA79ADEF551A8204CA7F728E5378607D85695B16F129EC35BF4AF15DCF6D3581DEB8BB0DEBB239C33E7F1271A37C7F60F1044AE417730F5082ABEE5F9EC568A8A4CEF04074394755706376E982DC6F9D15430FAAAD385AE8F00A77EF1C97517F1A1517004CE78028B9CE")]
// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("edd53f03-08be-4da3-86e1-734573103c93")]

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

#if (SyncfusionFramework4_0 && Silverlight5)
[assembly: AssemblyVersion("12.1500.0.43")]
#elif (SyncfusionFramework4_0 && Silverlight4)
[assembly: AssemblyVersion("12.1400.0.43")]
#else
[assembly: AssemblyVersion("12.1400.0.43")]
#endif




#if !SILVERLIGHT
[assembly: XmlnsPrefix("http://schemas.syncfusion.com/wpf", "syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Windows.Data")]
#endif

[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyKeyFile(@"..\..\..\..\..\Common\Keys\sf.publicsnk")]

[assembly: XmlnsPrefix("clr-namespace:Syncfusion.Windows.Data;assembly=Syncfusion.Linq.Silverlight", "syncfusion")]
[assembly: XmlnsDefinition("clr-namespace:Syncfusion.Windows.Data;assembly=Syncfusion.Linq.Silverlight", "Syncfusion.Windows.Data")]