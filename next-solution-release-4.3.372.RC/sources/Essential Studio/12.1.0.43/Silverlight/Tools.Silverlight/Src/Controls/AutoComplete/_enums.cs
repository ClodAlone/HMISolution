#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Scope of the enumeration
    /// </summary>
    internal enum ResourceScope : uint
    {
        /// <summary>
        /// Indicates the connected ResourceScope
        /// </summary>
        CONNECTED = 0x00000001,

        /// <summary>
        /// Indicates the Gloabalnet ResourceScope
        /// </summary>
        GLOBALNET = 0x00000002,

        /// <summary>
        /// Indicates the Remembered ResourceScope
        /// </summary>
        REMEMBERED = 0x00000003,

        /// <summary>
        /// Indicates the Recent ResourceScope
        /// </summary>
        RECENT = 0x00000004,

        /// <summary>
        /// Indicates the Context enumeration
        /// </summary>
        CONTEXT = 0x00000005,
    }

    /// <summary>
    /// Set of bit flags identifying the type of resource
    /// </summary>
    [Flags]
    internal enum ResourceType : uint
    {
        /// <summary>
        /// Indicates Any ResourceType
        /// </summary>
        ANY = 0x00000000,

        /// <summary>
        /// Indicates Disk ResourceType
        /// </summary>
        DISK = 0x00000001,

        /// <summary>
        /// Indicates Print ResourceType
        /// </summary>
        PRINT = 0x00000002,

        /// <summary>
        /// Indicates Reserved ResourceType
        /// </summary>
        RESERVED = 0x00000008,

        /// <summary>
        /// Indicates Unknown ResourceType
        /// </summary>
        UNKNOWN = 0xFFFFFFFF,
    }

    /// <summary>
    /// Set of bit flags describing how the resource can be used
    /// </summary>
    [Flags]
    internal enum ResourceUsage : uint
    {
        /// <summary>
        /// Indicates Connectable ResourceUsage
        /// </summary>
        CONNECTABLE = 0x00000001,

        /// <summary>
        /// Indicates CONTAINER ResourceUsage
        /// </summary>
        CONTAINER = 0x00000002,

        /// <summary>
        /// Indicates NOLOCALDEVICE ResourceUsage
        /// </summary>
        NOLOCALDEVICE = 0x00000004,

        /// <summary>
        /// Indicates SIBLING ResourceUsage
        /// </summary>
        SIBLING = 0x00000008,

        /// <summary>
        /// Indicates ATTACHED ResourceUsage
        /// </summary>
        ATTACHED = 0x00000010,

        /// <summary>
        /// Indicates RESERVED ResourceUsage
        /// </summary>
        RESERVED = 0x80000000,

        /// <summary>
        /// Indicates ALL ResourceUsage
        /// </summary>
        ALL = (CONNECTABLE | CONTAINER | ATTACHED),
    }

    /// <summary>
    /// Display options for the network object in a network browsing user interface
    /// </summary>
    internal enum ResourceDisplayType : uint
    {
        /// <summary>
        /// Indicates GENERIC ResourceDisplayType
        /// </summary>
        GENERIC = 0x00000000,

        /// <summary>
        /// Indicates DOMAIN ResourceDisplayType
        /// </summary>
        DOMAIN = 0x00000001,

        /// <summary>
        /// Indicates SERVER ResourceDisplayType
        /// </summary>
        SERVER = 0x00000002,

        /// <summary>
        /// Indicates SHARE ResourceDisplayType
        /// </summary>
        SHARE = 0x00000003,

        /// <summary>
        /// Indicates FILE ResourceDisplayType
        /// </summary>
        FILE = 0x00000004,

        /// <summary>
        /// Indicates GROUP ResourceDisplayType
        /// </summary>
        GROUP = 0x00000005,

        /// <summary>
        /// Indicates NETWORK ResourceDisplayType
        /// </summary>
        NETWORK = 0x00000006,

        /// <summary>
        /// Indicates ROOT ResourceDisplayType
        /// </summary>
        ROOT = 0x00000007,

        /// <summary>
        /// Indicates SHAREADMIN ResourceDisplayType
        /// </summary>
        SHAREADMIN = 0x00000008,

        /// <summary>
        /// Indicates DIRECTORY ResourceDisplayType
        /// </summary>
        DIRECTORY = 0x00000009,

        /// <summary>
        /// Indicates GENETREERIC ResourceDisplayType
        /// </summary>
        TREE = 0x0000000A,

        /// <summary>
        /// Indicates NDSCONTAINER ResourceDisplayType
        /// </summary>
        NDSCONTAINER = 0x0000000B,
    }
}