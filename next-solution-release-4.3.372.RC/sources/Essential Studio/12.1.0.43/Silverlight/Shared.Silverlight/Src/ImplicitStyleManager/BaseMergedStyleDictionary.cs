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

namespace Syncfusion.Windows.Controls.Theming
{
    /// <summary>
    /// A class that defines the behavior of looking for an item recursively up 
    /// a chain of dictionaries.
    /// </summary>
    internal abstract class BaseMergedStyleDictionary
    {
        /// <summary>
        /// Gets or sets the parent of this merged dictionary.
        /// </summary>
        internal BaseMergedStyleDictionary Parent { get; set; }

        /// <summary>
        /// Retrieves an item using a key.  If the item is not found in the 
        /// local dictionary a lookup is attempted on the parent.
        /// </summary>
        /// <param name="key">The key to use to retrieve the item.</param>
        /// <returns>A style corresponding to the key.</returns>
        internal abstract Style this[string key] { get; }
    }
}