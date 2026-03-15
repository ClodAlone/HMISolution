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

using System.Diagnostics.CodeAnalysis;
using System;

namespace Syncfusion.Windows.Controls
{
    /// <summary>
    /// Interface describing the common value, maximum and minimum properties
    /// that a TimeInput control is expected to have.
    /// </summary>
    /// <remarks>Used for internal coercion of these properties.</remarks>
    /// <QualityBand>Preview</QualityBand>
    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "Not an exception class.")]
    internal interface ITimeInput
    {
        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        /// <value>The current time.</value>
        DateTime? Value { get; set; }

        /// <summary>
        /// Gets or sets the minimum time.
        /// </summary>
        /// <value>The minimum time.</value>
        DateTime? Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum time.
        /// </summary>
        /// <value>The maximum time.</value>
        DateTime? Maximum { get; set; }
    }
}
