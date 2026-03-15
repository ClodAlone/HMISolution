#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;

namespace Syncfusion.Windows.Controls
{
    /// <summary>
    /// Defines time formats used for formatting and parsing DateTime values.
    /// </summary>
    public interface ITimeFormat
    {
        /// <summary>
        /// Gets the format to use to display a DateTime as a time value.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>A format to use during display of a DateTime.</returns>
        string GetTimeDisplayFormat(CultureInfo culture);

        /// <summary>
        /// Gets the formats to use to parse a string to a DateTime.
        /// </summary>
        /// <param name="culture">Culture used to determine formats.</param>
        /// <returns>An array of formats to be used during parsing.</returns>
        string[] GetTimeParseFormats(CultureInfo culture);
    }
}
