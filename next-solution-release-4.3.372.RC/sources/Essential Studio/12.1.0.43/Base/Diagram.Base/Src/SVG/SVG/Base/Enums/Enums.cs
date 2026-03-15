#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// SpreadMethod enumeration.
    /// </summary>
    public enum SpreadMethod
    {
        /// <summary>
        /// Pad Spread method
        /// </summary>
        pad,

        /// <summary>
        /// Reflect Spread method
        /// </summary>
        reflect,

        /// <summary>
        /// Repeat Spread method
        /// </summary>
        repeat
    }

    /// <summary>
    /// Length type enumeration.
    /// </summary>
    public enum LengthType
    {
        /// <summary>
        /// Unknown length method
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Length in number
        /// </summary>
        Number = 1,

        /// <summary>
        /// Length in percentage
        /// </summary>
        Percentage = 2,

        /// <summary>
        /// Length in EMS
        /// </summary>
        EMS = 3,

        /// <summary>
        /// Length in EXS
        /// </summary>
        EXS = 4,

        /// <summary>
        /// Length in PX
        /// </summary>
        PX = 5,

        /// <summary>
        /// Length in CM
        /// </summary>
        CM = 6,

        /// <summary>
        /// Length in MM
        /// </summary>
        MM = 7,

        /// <summary>
        /// Length in IN
        /// </summary>
        IN = 8,

        /// <summary>
        /// Length in PT
        /// </summary>
        PT = 9,

        /// <summary>
        /// Length in PC
        /// </summary>
        PC = 10,
    }

    /// <summary>
    /// Angle type enumeration.
    /// </summary>
    public enum AngleType
    {
        /// <summary>
        /// Unknown angle.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// Unspecified angle.
        /// </summary>
        UNSPECIFIED = 1,

        /// <summary>
        /// Angle in Degree
        /// </summary>
        DEG = 2,

        /// <summary>
        /// Angle in radians.
        /// </summary>
        RAD = 3,

        /// <summary>
        /// Angle in grad.
        /// </summary>
        GRAD = 4,
    }
}
