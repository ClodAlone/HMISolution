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

using System.Collections;

namespace Syncfusion.XlsIO
{
    public interface ICustomXmlPartCollection : IEnumerable
    {

        #region Interface properties
        /// <summary>
        /// Returns the number of objects in the collection. Read-only Long.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Returns the parent object for the specified object.
        /// </summary>
        ICustomXmlPart this[int index] { get;}
        #endregion

        #region Interface methods
        /// <summary>
        /// Defines a new name. 
        /// </summary>
        /// <param name="name">Name for the new Name object.</param>
        /// <returns>Returns a Name object.</returns>
        ICustomXmlPart Add(ICustomXmlPart customXmlPart);
        /// <summary>
        /// Defines a new name. 
        /// </summary>
        /// <param name="name">Name for the new Name object.</param>
        /// <returns>Returns a Name object.</returns>
        ICustomXmlPart Add(string ID);
        // <summary>
        /// Defines a new name. 
        /// </summary>
        /// <param name="name">Name for the new Name object.</param>
        /// <returns>Returns a Name object.</returns>
        ICustomXmlPart Add(string ID, byte[] XmlData);
        /// <summary>
        /// Defines a new name.
        /// </summary>
        /// <param name="name">Name object to add.</param>
        void Clear();
        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        void RemoveAt(int index);
        /// <summary>
        /// Checks whether the Name object is present in the collection or not
        /// </summary>
        /// <param name="name">Name object to check whether it is present or not.</param>
        /// <returns></returns>
        ICustomXmlPartCollection Clone();
        /// <summary>
        /// Checks whether the Name object is present in the collection or not
        /// </summary>
        /// <param name="name">Name object to check whether it is present or not.</param>
        /// <returns></returns>
        ICustomXmlPart GetById(string id);
        #endregion

    }
}
