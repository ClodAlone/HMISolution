#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
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
    /// An interface IHTMLFormatsCollection.
    /// </summary>
    public interface IHTMLFormatsCollection
    : IList, IHTMLCollection
    {
        #region Interface Properties
        /// <summary>
        /// Gets or sets the format for the specified index. Type safe override.
        /// </summary>
        /// <param name="index">index value</param>
        new IHTMLFormat this[int index] 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the format for the specified element name. Name is case insensitive.
        /// </summary>
        /// <param name="name">string name value</param>
        IHTMLFormat this[string name] 
        { 
            get; 
            set;
        }
        #endregion

        #region Interface Methods
        /// <summary>
        /// Adds the format to the collection.
        /// </summary>
        /// <param name="format">Reference to the format.</param>
        /// <returns>Index of the format in the array.</returns>
        int Add(IHTMLFormat format);

        /// <summary>
        /// Adds the array of formats to the collection.
        /// </summary>
        /// <param name="formats">Array of formats.</param>
        void AddRange(IHTMLFormat[] formats);

        /// <summary>
        /// Overloaded. Indicates whether the collection contains the specified format.
        /// </summary>
        /// <param name="format">Format reference for check.</param>
        /// <returns>TRUE if collection contains the format; FALSE otherwise.</returns>
        bool Contains(IHTMLFormat format);

        /// <summary>
        /// Indicates whether the collection contains the format with the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the format.</param>
        /// <returns>TRUE if collection contains the format; FALSE otherwise.</returns>
        bool Contains(string name);

        /// <summary>
        /// Overloaded. Returns the index of the format. If the collection does not contain the format 
        /// -1 is returned.
        /// </summary>
        /// <param name="format">Format whose index is needed.</param>
        /// <returns>Zero based index of format; -1 otherwise.</returns>
        int IndexOf(IHTMLFormat format);

        /// <summary>
        /// Returns the index of the format with the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the format.</param>
        /// <returns>Zero based index of the format; -1 otherwise.</returns>
        int IndexOf(string name);

        /// <summary>
        /// Overloaded. Removes the format from the collection, if it belongs to it.
        /// </summary>
        /// <param name="format">Format whose reference must be removed from the collection.</param>
        void Remove(IHTMLFormat format);

        /// <summary>
        /// Removes the format with the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the format which must be removed.</param>
        void Remove(string name);
        #endregion
    }
}
