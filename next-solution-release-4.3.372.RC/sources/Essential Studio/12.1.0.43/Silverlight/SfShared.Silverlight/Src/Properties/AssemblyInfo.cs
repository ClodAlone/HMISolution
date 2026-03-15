#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Syncfusion.SfShared.Silverlight")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Syncfusion.SfShared.Silverlight")]
[assembly: AssemblyCopyright("Copyright (c) 2001-2014 Syncfusion. Inc,")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("26dfe892-41a0-4897-be9d-6ae4ebe3b780")]

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

/// <summary>
/// Represents a class for the reference attributes
/// </summary>
public class ClassReferenceAttribute : Attribute
{
    private bool isReviewed = false;

    /// <summary>
    /// Returns a value when set
    /// </summary>
    /// <value>
    /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
    /// </value>
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

    /// <summary>
    /// Returns a value when set
    /// </summary>
    /// <value>
    /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
    /// </value>
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