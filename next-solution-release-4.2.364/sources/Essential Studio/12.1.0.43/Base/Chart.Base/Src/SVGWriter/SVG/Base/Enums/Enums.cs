#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Specifies the spread methods.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum SpreadMethod
    {
        /// <summary>
        /// The pad value
        /// </summary>
        pad,

        /// <summary>
        /// The reflect value
        /// </summary>
        reflect,

        /// <summary>
        /// The repeat value
        /// </summary>
        repeat
    }

    /// <summary>
    /// Specifies the types of length object.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum LengthType
    {
        /// <summary>
        /// Unknown length type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Number type of length.
        /// </summary>
        Number = 1,

        /// <summary>
        /// Percentage type of length.
        /// </summary>
        Percentage = 2,

        /// <summary>
        /// "EMS" type of length.
        /// </summary>
        EMS = 3,

        /// <summary>
        /// "EXS" type of length.
        /// </summary>
        EXS = 4,

        /// <summary>
        /// "PX" type of length.
        /// </summary>
        PX = 5,

        /// <summary>
        /// "CM" type of length.
        /// </summary>
        CM = 6,

        /// <summary>
        /// "MM" type of length.
        /// </summary>
        MM = 7,

        /// <summary>
        /// "IN" type of length.
        /// </summary>
        IN = 8,

        /// <summary>
        /// "PT" type of length.
        /// </summary>
        PT = 9,

        /// <summary>
        /// "PC" type of length.
        /// </summary>
        PC = 10,
    }

    /// <summary>
    /// Specifies the types of angle object.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum AngleType
    {
        /// <summary>
        /// "UNKNOWN" type of angle.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// "UNSPECIFIED" type of angle.
        /// </summary>
        UNSPECIFIED = 1,

        /// <summary>
        /// "DEG" type of angle.
        /// </summary>
        DEG = 2,

        /// <summary>
        /// "RAD" type of angle.
        /// </summary>
        RAD = 3,

        /// <summary>
        /// "GRAD" type of angle.
        /// </summary>
        GRAD = 4,
    }
}
