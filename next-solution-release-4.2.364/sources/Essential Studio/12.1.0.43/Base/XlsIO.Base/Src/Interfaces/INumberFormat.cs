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

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Represents settings of number format.
	/// </summary>
	public interface INumberFormat : IParentApplication
	{
    /// <summary>
    /// Returns format index. Read-only.
    /// </summary>
    int Index { get; }
    /// <summary>
    /// Returns format string. Read-only.
    /// </summary>
    string FormatString { get; }
    /// <summary>
    /// Returns format type of the first section of this number format. Read-only.
    /// </summary>
    ExcelFormatType FormatType { get; }
    /// <summary>
    /// Indicates whether the first section of this number format contains fraction sign. Read-only.
    /// </summary>
    bool IsFraction { get; }
    /// <summary>
    /// Indicates whether first section of this number format contains E/E+
    /// or E- signs in format string. Read-only.
    /// </summary>
    bool IsScientific { get; }
    /// <summary>
    /// Indicates whether thousand separator is present in the first section
    /// of this number format. Read-only.
    /// </summary>
    bool IsThousandSeparator { get; }
    /// <summary>
    /// Number of digits after "." sign in the first section of this number format. Read-only.
    /// </summary>
    int DecimalPlaces { get; }
  }
}
