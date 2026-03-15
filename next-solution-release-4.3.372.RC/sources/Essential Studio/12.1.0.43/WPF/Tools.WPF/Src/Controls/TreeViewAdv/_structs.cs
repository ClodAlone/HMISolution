// <copyright file="_structs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Represents struct NanUnion
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct NanUnion
    {
        /// <summary>
        /// Presents double value
        /// </summary>
        [FieldOffset(0)]
        internal double DoubleValue;

        /// <summary>
        /// Presents unit value
        /// </summary>
        [FieldOffset(0)]
        internal ulong UintValue;
    }
}