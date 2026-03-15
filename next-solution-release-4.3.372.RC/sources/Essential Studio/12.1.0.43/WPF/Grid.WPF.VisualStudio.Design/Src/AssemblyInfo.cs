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

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.Grid.WPF.VisualStudio.Design")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Syncfusion Inc")]
[assembly: AssemblyProduct("Syncfusion.Grid.WPF")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]


[assembly: XmlnsPrefix("http://schemas.syncfusion.com/wpf", "syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion")]
[assembly: XmlnsDefinition("http://schemas.syncfusion.com/wpf", "Syncfusion.Grid.WPF.VisualStudio.Design")]


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







[assembly: AssemblyDelaySign(true)]

[assembly: AssemblyKeyFile("../../../../../Common/keys/sf.publicsnk")]

[assembly: AssemblyKeyName("")]
namespace Syncfusion
{

    public class SharedBaseAssembly
    {
        public static readonly string Name;

        public static readonly Assembly Assembly;
        public static readonly string RootNamespace = "Syncfusion.Grid.WPF.VisualStudio.Design";

        static SharedBaseAssembly()
        {
            Assembly = typeof(SharedBaseAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

        public static Assembly AssemblyResolver(object sender, System.ResolveEventArgs e)
        {
            if (e.Name.StartsWith(SharedBaseAssembly.Name))
                return SharedBaseAssembly.Assembly;
            else if (e.Name.StartsWith(CoreAssembly.Name))
                return CoreAssembly.Assembly;
            else
            {
                string name = e.Name.ToLower();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int n = 0; n < assemblies.Length; n++)
                {
                    if (assemblies[n].GetName().Name.ToLower() == name)
                        return assemblies[n];
                }
            }

            return null;
        }
    }

    internal class AssemblyInfo : SharedBaseAssembly
    {
    }
}