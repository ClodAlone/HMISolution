#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO
{
    public interface IPivotCalculatedFields
    {
        /// <summary>
        /// Returns the number of items in the collection
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets single entry from the collection.
        /// </summary>
        /// <param name="index">Zero-based index of the item to get.</param>
        /// <returns>Single entry from the collection.</returns>
        IPivotField this[int index] { get; }
        /// <summary>
        /// Gets single entry from the collection.
        /// </summary>
        /// <param name="name">Calculated Field Name</param>
        /// <returns>Single entry from the collection.</returns>
        IPivotField this[string name] { get; }
        /// <summary>
        /// Adds the calculated field to the specified pivot table.
        /// </summary>
        /// <param name="name">name of the calculated pivot field.</param>
        /// <param name="formula">formula of the calculated pivot field.</param>
        /// <returns>Calculated pivot field</returns>
        IPivotField Add(string name, string formula);
    }
}
