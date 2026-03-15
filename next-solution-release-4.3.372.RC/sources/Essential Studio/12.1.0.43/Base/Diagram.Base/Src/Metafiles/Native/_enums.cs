#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Specifies the type of character information the user wants to retrieve.
    /// </summary>
    internal enum StringInfoType : uint
    {
        /// <summary>
        /// Retrieves character type info
        /// </summary>
        CT_TYPE1 = 1,

        /// <summary>
        /// Retrieves bi-directional layout info
        /// </summary>
        CT_TYPE2 = 2,

        /// <summary>
        /// Retrieves text processing info
        /// </summary>
        CT_TYPE3 = 4
    }

    /// <summary>
    /// Native enum.
    /// </summary>
    [Flags]
    internal enum FormatMessageFlags
    {
        /// <summary>
        /// Allocate buffer.
        /// </summary>
        AllocateBuffer = 0x00000100,

        /// <summary>
        /// Ignore inserts.
        /// </summary>
        IgnoreInserts = 0x00000200,

        /// <summary>
        /// From string.
        /// </summary>
        FromString = 0x00000400,

        /// <summary>
        /// From Hmodule.
        /// </summary>
        FromHmodule = 0x00000800,

        /// <summary>
        /// From system.
        /// </summary>
        FromSystem = 0x00001000,

        /// <summary>
        /// Arugement array.
        /// </summary>
        ArgumentArray = 0x00002000
    }
}
