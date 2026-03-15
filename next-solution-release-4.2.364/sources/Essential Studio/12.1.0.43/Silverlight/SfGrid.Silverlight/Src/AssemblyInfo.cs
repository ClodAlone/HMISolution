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

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.SfGrid.Silverlight")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Syncfusion")]
[assembly: AssemblyProduct("Syncfusion.SfGrid.Silverlight")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
//[assembly: InternalsVisibleTo("System.Runtime.Serialization")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ac37bed8-f281-49bb-a2b7-4e103c59f777")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
#if (SyncfusionFramework4_0 && Silverlight5)
[assembly: AssemblyVersion("12.1500.0.43")]
#elif (SyncfusionFramework4_0 && Silverlight4)
[assembly: AssemblyVersion("12.1400.0.43")]
#else
[assembly: AssemblyVersion("12.1400.0.43")]
#endif




[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyKeyFile(@"..\..\..\..\..\Common\Keys\sf.publicsnk")]


namespace Syncfusion
{
    /// <summary>
    /// This class holds the name of the Syncfusion.Windows.Grid assembly and provides a helper 
    /// routine that helps with resolving types when loading a serialization stream and when 
    /// the framework probes for assemblies by reflection. 
    /// </summary>
    public class GridSilverlightAssembly
    {
        /// <summary>
        /// The full name of this assembly without version information: "Syncfusion.Grid.Silverlight"
        /// </summary>
        public static readonly string Name;

        /// <summary>
        /// A reference to the <see cref="System.Reflection.Assembly"/> for the Grid assembly.
        /// </summary>
        public static readonly Assembly Assembly;


        /// <summary>
        /// The root namespace of this assembly. Used internally for locating resources within the assembly.
        /// </summary>
        public static readonly string RootNamespace = "Syncfusion.Windows.Grid";

        static GridSilverlightAssembly()
        {
            Assembly = typeof(GridSilverlightAssembly).Assembly;
            string s = Assembly.FullName;
            int n = s.IndexOf(",");
            Name = s.Substring(0, n);
        }

    }
}

namespace Syncfusion.UI.Xaml.Grid
{
    public class ClassReferenceAttribute : Attribute
    {
        private bool isReviewed = false;

        public bool IsReviewed
        {
            get { return isReviewed; }
            set
            {
                if (isReviewed == value)
                    return;

                isReviewed = value;

            }
        }

        private bool shouldInclude = true;

        public bool ShouldInclude
        {
            get { return shouldInclude; }
            set
            {
                if (shouldInclude == value)
                    return;

                shouldInclude = value;

            }
        }
    }

    internal class AssemblyInfo : Syncfusion.GridSilverlightAssembly
    {
    }
}