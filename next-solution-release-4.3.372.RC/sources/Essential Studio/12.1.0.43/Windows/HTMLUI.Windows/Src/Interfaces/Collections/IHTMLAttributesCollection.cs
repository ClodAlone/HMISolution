#region Copyright Syncfusion Inc. 2001 - 2014

//// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Collection that stores and provides access to HTML element attributes.
    /// </summary>
    public interface IHTMLAttributesCollection
    : IList, IHTMLCollection
    {
        #region Interface Properties
        /// <summary>
        /// Gets or sets the attribute with the specified index. Type safe override.
        /// </summary>
        /// <param name="index">Index value</param>
        new IHTMLAttribute this[int index] 
        { 
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attribute with the specified name. Name is case insensitive.
        /// </summary>
        /// <param name="name">string value</param>
        IHTMLAttribute this[string name] 
        { 
            get;
            set;
        }
        #endregion

        #region Interface Methods
        /// <summary>
        /// Adds an attribute to the collection.
        /// </summary>
        /// <param name="attr">Reference to the attribute.</param>
        /// <returns>Index of the attribute in array.</returns>
        int Add(IHTMLAttribute attr);

        /// <summary>
        /// Creates the attribute, adds it to the collection and returns a reference on it.
        /// </summary>
        /// <param name="name">Name of the attribute. Case insensitive.</param>
        /// <param name="value">Value of the attribute.</param>
        /// <returns>Reference of the created attribute.</returns>
        IHTMLAttribute Add(string name, string value);

        /// <summary>
        /// Creates an attribute with the specified name and returns a reference on it.
        /// </summary>
        /// <param name="name">Name of the attribute. Case insensitive.</param>
        /// <returns>Reference of the created attribute.</returns>
        IHTMLAttribute Add(string name);

        /// <summary>
        /// Adds a range of attributes to the collection.
        /// </summary>
        /// <param name="attributes">Array of attributes.</param>
        void AddRange(IHTMLAttribute[] attributes);

        /// <summary>
        /// Overloaded. Indicates whether the collection contains the specified attribute. 
        /// </summary>
        /// <param name="attr">Attribute reference for check.</param>
        /// <returns>TRUE if the collection contains the attribute; FALSE otherwise.</returns>
        bool Contains(IHTMLAttribute attr);

        /// <summary>
        /// Indicates whether the collection contains the attribute with the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the attribute.</param>
        /// <returns>TRUE if the collection contains the attribute; FALSE otherwise.</returns>
        bool Contains(string name);

        /// <summary>
        /// Overloaded. Returns the index of the attribute. If the collection does not contain the attribute 
        /// -1 is returned.
        /// </summary>
        /// <param name="attr">Attribute whose index is needed.</param>
        /// <returns>Zero based index of the attribute; -1 otherwise.</returns>
        int IndexOf(IHTMLAttribute attr);

        /// <summary>
        /// Returns the index of the attribute with the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the attribute.</param>
        /// <returns>Zero based index of the attribute; -1 otherwise.</returns>
        int IndexOf(string name);

        /// <summary>
        /// Overloaded. Removes the attribute from collection if it belongs to it.
        /// </summary>
        /// <param name="attr">Attribute whose reference must be removed from the collection.</param>
        void Remove(IHTMLAttribute attr);

        /// <summary>
        /// Removes the attribute with the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the attribute which must be removed.</param>
        void Remove(string name);
        #endregion
    }
}