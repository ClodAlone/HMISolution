// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// This indicates complete mode.
    /// </summary>

    public enum SourceMode
    {
        /// <summary>
        /// This set the File Path as the Source for the Source Mode
        /// </summary>
        FilePath = 0,

        /// <summary>
        /// This set the Registry as the Source for the Source Mode
        /// </summary>
        Registry,

        /// <summary>
        /// This set the Custom as the Source for the Source Mode
        /// </summary>
        Custom
    }

    /// <summary>
    /// Specifies different types of comparision mode to display the drop-down hint
    /// </summary>

    public enum StringMode
    {
        /// <summary>
        /// Comarison Starts From starting Index
        /// </summary>
        StartChar = 0,

        /// <summary>
        /// Comparison Starting From Specific Index
        /// </summary>
        IndexBased,

        /// <summary>
        /// Compares Substring
        /// </summary>
        AnyChar
    }

    /// <summary>
    /// Scope of the enumeration
    /// </summary>
    internal enum ResourceScope : uint
    {
        /// <summary>
        /// Represents the connected
        /// </summary>
        CONNECTED = 0x00000001,

        /// <summary>
        /// Represents the GLOBALNET
        /// </summary>
        GLOBALNET = 0x00000002,

        /// <summary>
        /// Represents the REMEMBERED
        /// </summary>
        REMEMBERED = 0x00000003,

        /// <summary>
        /// Represents the RECENT
        /// </summary>
        RECENT = 0x00000004,

        /// <summary>
        /// Represents the CONTEXT
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
        /// Represents the ANY
        /// </summary>
        ANY = 0x00000000,

        /// <summary>
        /// Represents the DISK
        /// </summary>
        DISK = 0x00000001,

        /// <summary>
        /// Represents the PRINT
        /// </summary>
        PRINT = 0x00000002,

        /// <summary>
        /// Represents the RESERVED
        /// </summary>
        RESERVED = 0x00000008,

        /// <summary>
        /// Represents the UNKNOWN
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
        /// Represents the CONNECTABLE
        /// </summary>
        CONNECTABLE = 0x00000001,

        /// <summary>
        /// Represents the CONTAINER
        /// </summary>
        CONTAINER = 0x00000002,

        /// <summary>
        /// Represents the NOLOCALDEVICE
        /// </summary>
        NOLOCALDEVICE = 0x00000004,

        /// <summary>
        /// Represents the SIBLING
        /// </summary>
        SIBLING = 0x00000008,

        /// <summary>
        /// Represents the ATTACHED
        /// </summary>
        ATTACHED = 0x00000010,

        /// <summary>
        /// Represents the RESERVED
        /// </summary>
        RESERVED = 0x80000000,

        /// <summary>
        /// Represents the ALL
        /// </summary>
        ALL = (CONNECTABLE | CONTAINER | ATTACHED),
    }

    /// <summary>
    /// Display options for the network object in a network browsing user interface
    /// </summary>
    internal enum ResourceDisplayType : uint
    {
        /// <summary>
        /// Represents the GENERIC
        /// </summary>
        GENERIC = 0x00000000,

        /// <summary>
        /// Represents the DOMAIN
        /// </summary>
        DOMAIN = 0x00000001,

        /// <summary>
        /// Represents the SERVER
        /// </summary>
        SERVER = 0x00000002,

        /// <summary>
        /// Represents the SHARE
        /// </summary>
        SHARE = 0x00000003,

        /// <summary>
        /// Represents the FILE
        /// </summary>
        FILE = 0x00000004,

        /// <summary>
        /// Represents the GROUP
        /// </summary>
        GROUP = 0x00000005,

        /// <summary>
        /// Represents the NETWORK
        /// </summary>
        NETWORK = 0x00000006,

        /// <summary>
        /// Represents the ROOT
        /// </summary>
        ROOT = 0x00000007,

        /// <summary>
        /// Represents the SHAREADMIN
        /// </summary>
        SHAREADMIN = 0x00000008,

        /// <summary>
        /// Represents the DIRECTORY
        /// </summary>
        DIRECTORY = 0x00000009,

        /// <summary>
        /// Represents the TREE
        /// </summary>
        TREE = 0x0000000A,

        /// <summary>
        /// Represents the NDSCONTAINER
        /// </summary>
        NDSCONTAINER = 0x0000000B,
    }

    /// <summary>
    /// Positions for the Popup.
    /// </summary>
    public enum PopupPlacement
    {
        /// <summary>
        /// Represents Top PopupPlacement
        /// </summary>
        Top,

        /// <summary>
        /// Represents Bottom PopupPlacement
        /// </summary>
        Bottom,

        /// <summary>
        /// Represents Right PopupPlacement
        /// </summary>
        Right,

        /// <summary>
        /// Represents Left PopupPlacement
        /// </summary>
        Left
    }
}