#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Represents collection of all pivot tables inside worksheet.
    /// </summary>
    public interface IPivotTables
    {
        /// <summary>
        /// Returns number of items in the collection.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets single entry from the collection.
        /// </summary>
        /// <param name="index">Zero-based index of the item to get.</param>
        /// <returns>Single entry from the collection.</returns>
        IPivotTable this[int index] { get; }
        /// <summary>
        /// Gets single entry from the collection.
        /// </summary>
        /// <param name="name">Pivot table name.</param>
        /// <returns>Single entry from the collection.</returns>
        IPivotTable this[string name] { get; }
        /// <summary>
        /// Adds new pivot table to the collection.
        /// </summary>
        /// <param name="name">Name of the new pivot table.</param>
        /// <param name="location">Pivot table location.</param>
        /// <param name="cache"></param>
        /// <returns></returns>
        IPivotTable Add(string name, IRange location, IPivotCache cache);
        /// <summary>
        /// Removes pivot table from the collection.
        /// </summary>
        /// <param name="name">name of the pivot table to remove.</param>
        void Remove(string name);
        /// <summary>
        /// Removes piovt table from the collection base on the index.
        /// </summary>
        /// <param name="index"></param>
        void RemoveAt(int index);
    }
}
