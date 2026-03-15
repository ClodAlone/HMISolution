// ---------------------------------------------------------------------------
// Campari Software
//
// AssemblyInfo.cs
//
// Provides a cental location for Assembly attributes. Assembly attributes 
// are values that provide information about an assembly. The attributes are
// divided into the following sets of information: 
//
//    * Assembly identity attributes. 
//    * Informational attributes. 
//    * Assembly manifest attributes. 
//    * Strong name attributes. 
//
// ---------------------------------------------------------------------------
// Copyright (C) 2006 Campari Software
// All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
// ---------------------------------------------------------------------------
using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

#region Assembly identity attributes
[assembly: SatelliteContractVersionAttribute("1.0.0.0")]
#endregion

#region Informational attributes

// String value specifying the Win32 file version number. This normally defaults to the assembly version.
// [assembly: AssemblyFileVersion("")] 

// String value specifying version information that is not used by the runtime, such as a full product version number.
// [assembly: AssemblyInformationalVersionAttribute("")] 
// [assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = false)]
[assembly:CLSCompliant(true)]
[assembly:ComVisible(false)]

#endregion

#region Assembly manifest attributes

#if Debug
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// String value specifying a default alias to be used by referencing assemblies. This value provides
// a friendly name when the name of the assembly itself is not friendly (such as a GUID value). This
// value can also be used as a short form of the full assembly name.
// [assembly: AssemblyDefaultAliasAttribute("")]

[assembly: AssemblyDescription("Provides core functionality.")]
[assembly: AssemblyTitle("Campari Software Common Library for .NET 4.5.1 Core")]

#endregion

#region Strong name attributes

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]

#endregion
