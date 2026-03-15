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
    /// Interface which is responsible for the collection of HTML elements.
    /// </summary>
    public interface IHTMLElementsCollection
    : IList, IHTMLCollection
    {
        /// <summary>
        /// Gets or sets the element with the specified index.
        /// </summary>
        /// <param name="index">index value</param>
        new IHTMLElement this[int index] 
        { 
            get; 
            set;
        }

        /// <summary>
        /// Returns the child element with the specified unique ID if such element exists; NULL otherwise.
        /// </summary>
        /// <param name="uniqueID">An unique string value</param>
        IHTMLElement this[string uniqueID] 
        { 
            get;
        }

        /// <summary>
        /// Adds an element to the collection.
        /// </summary>
        /// <param name="element">Element for adding to the collection.</param>
        /// <returns>Index in the collection.</returns>
        int Add(IHTMLElement element);

        /// <summary>
        /// Creates an element from it's string representation and adds it to the collection.
        /// </summary>
        /// <param name="outerHtml">String representation of the element.</param>
        /// <returns>Index of the element in the collection if created; -1 otherwise.</returns>
        int Add(string outerHtml);

        /// <summary>
        /// Adds a range of elements to the collection.
        /// </summary>
        /// <param name="values">Array of elements for adding to the collection.</param>
        void AddRange(IHTMLElement[] values);

        /// <summary>
        /// Indicates whether such element already exists in the collection.
        /// </summary>
        /// <param name="element">Element object.</param>
        /// <returns>TRUE if element exists in the collection.</returns>
        bool Contains(IHTMLElement element);

        /// <summary>
        /// Returns the index of the specified element in the collection.
        /// </summary>
        /// <param name="element">Element object.</param>
        /// <returns>Index of the element in the collection.</returns>
        int IndexOf(IHTMLElement element);

        /// <summary>
        /// Overloaded. Inserts the specified element into the collection in the specified index.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        /// <param name="element">Element for adding to the collection.</param>
        void Insert(int index, IHTMLElement element);

        /// <summary>
        /// Creates the element from it's string representation and inserts it
        /// into the specified index.
        /// </summary>
        /// <param name="index">Index where the element must be placed.</param>
        /// <param name="outerHtml">String representation of the element.</param>
        void Insert(int index, string outerHtml);

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="element">Element for removing from the collection.</param>
        void Remove(IHTMLElement element);

        /// <summary>
        /// Overloaded. Returns an array of elementswith the specified name.
        /// </summary>
        /// <param name="name">Name of the element.</param>
        /// <returns>Array of elements.</returns>
        IHTMLElement[] GetElementsByName(string name);

        /// <summary>
        /// Returns an array of elements with the specified names.
        /// </summary>
        /// <param name="names">Array of names.</param>
        /// <returns>Array of elements by names.</returns>
        IHTMLElement[] GetElementsByName(string[] names);

        /// <summary>
        /// Returns an array of elements with the specified user ID.
        /// </summary>
        /// <param name="id">ID of the element defined in the HTML document.</param>
        /// <returns>Element object by ID.</returns>
        IHTMLElement GetElementByID(string id);

        /// <summary>
        /// Returns an array of elements with the specified unique ID.
        /// </summary>
        /// <param name="id">Unique ID of the element.</param>
        /// <returns>Element by its unique ID.</returns>
        IHTMLElement GetElementByUniqueID(string id);
    }
}