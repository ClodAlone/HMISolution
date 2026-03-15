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

using System.Windows;
namespace Syncfusion.Windows.Controls
{
    /// <summary>
    /// Provides data for the UpDownBase.Parsing event.
    /// </summary>
    /// <typeparam name="T">Type of Value property.</typeparam>
    /// <QualityBand>Stable</QualityBand>
    public class UpDownParsingEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Gets the original string value that will be parsed.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets the value to be used.
        /// </summary>
        /// <value>The parsed value.</value>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// this <see cref="UpDownParsingEventArgs&lt;T&gt;"/> is handled.
        /// </summary>
        /// <value><c>True</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="UpDownParsingEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="text">The text that will be parsed.</param>
        public UpDownParsingEventArgs(string text)
        {
            Text = text;
        }
    }
}
