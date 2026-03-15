// <copyright file="NetResource.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// The NETRESOURCE structure contains information about a
    /// network resource. The structure is returned during
    /// enumeration of network resources. NETRESOURCE is also
    /// specified when making or querying a network connection with
    /// calls to various Windows Networking functions.
    /// </summary>

    [StructLayout(LayoutKind.Sequential)]
    internal struct NetResource
    {
        /// <summary>
        /// Represents the scope
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public ResourceScope Scope;

        /// <summary>
        /// Represents the Type
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public ResourceType Type;

        /// <summary>
        /// Represents the Display Type
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public ResourceDisplayType DisplayType;

        /// <summary>
        /// Represents the Usage
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public ResourceUsage Usage;

        /// <summary>
        /// Represents the local name
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string LocalName;

        /// <summary>
        /// Represents the Remote Name
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string RemoteName;

        /// <summary>
        /// Represents the comment
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Comment;

        /// <summary>
        /// Represents the provider
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Provider;
    }
}