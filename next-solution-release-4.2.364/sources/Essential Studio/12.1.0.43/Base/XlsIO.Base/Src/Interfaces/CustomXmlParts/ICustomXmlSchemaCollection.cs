#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;

namespace Syncfusion.XlsIO
{

    /// <summary>
    /// A collection of all the Name objects in the application or
    /// workbook. Each Name object represents a defined name for a
    /// range of cells.
    /// </summary>
    public interface ICustomXmlSchemaCollection : IEnumerable
    {
        #region Interface properties
        /// <summary>
        /// Returns the number of objects in the collection. Read-only Long.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Returns a single Name object from a Names collection.
        /// </summary>
        string this[int index] { get; set; }
        #endregion

        #region Interface methods
        /// <summary>
        /// Defines a new name. 
        /// </summary>
        /// <param name="name">Name for the new Name object.</param>
        /// <returns>Returns a Name object.</returns>
        void Add(string name);
        /// <summary>
        /// Defines a new name. 
        /// </summary>
        /// <param name="name">Name for the new Name object.</param>
        /// <param name="namedObject">Range that will be associated with the name.</param>
        void Clear();
        /// <summary>
        /// Defines a new name.
        /// </summary>
        /// <param name="name">Name object to add.</param>
        ICustomXmlSchemaCollection Clone();
        /// <summary>
        /// Defines a new name.
        /// </summary>
        /// <param name="name">Name object to add.</param>
        int IndexOf(string value);
        /// <summary>
        /// Removes Name object from the collection.
        /// </summary>
        /// <param name="name">Name of the object to remove from the collection.</param>
        void Remove(string name);
        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        void RemoveAt(int index);
        
        #endregion
    }
}
