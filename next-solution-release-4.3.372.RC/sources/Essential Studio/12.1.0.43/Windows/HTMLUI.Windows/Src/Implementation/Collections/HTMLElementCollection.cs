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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Scripting;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Implementation of IHTMLElementCollection interface.
    /// </summary>
    /// Allow script engine to see this class for tree generation.
    //// [ ScriptBrowsable( PropertyType.Collection, typeof( IHTMLElement ) ) ]
    public class HTMLElementsCollection
    : EventBaseCollection, IHTMLElementsCollection
    {
        #region Class constants

        /// <summary>
        /// Default case insensitive comparer for internal use.
        /// </summary>
        private static readonly IComparer DEF_COMPARER = new CaseInsensitiveComparer();
        #endregion

        #region Class members

        /// <summary>
        /// Storage of parent property.
        /// </summary>
        private IHTMLElement m_parent;
        #endregion

        #region Class Properties

        /// <summary>
        /// Gets or sets the HTML element specified by its collection index.
        /// </summary>
        /// <param name="index">an int value</param>
        public IHTMLElement this[int index]
        {
            get
            {
                return (IHTMLElement)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Returns the child element by its unique ID if such element exists; NULL otherwise.
        /// </summary>
        /// <param name="uniqueID">a string id</param>
        public IHTMLElement this[string uniqueID]
        {
            get
            {
                return GetElementByUniqueID(uniqueID);
            }
        }

        /// <summary>
        /// Gets the parent element of the current collection.
        /// </summary>
        public IHTMLElement Parent
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>
        /// Gets the parent element of the BaseElement type. This property is for internal usage only.
        /// </summary>
        private BaseElement ParentEx
        {
            get
            {
                return this.Parent as BaseElement;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Prevents a default instance of the HTMLElementsCollection class from being created
        /// </summary>
        private HTMLElementsCollection()
        {
            this.m_parent = null;
        }

        /// <summary>
        /// Initializes a new instance of the HTMLElementsCollection class
        /// </summary>
        /// <param name="parent">Collection parent.</param>
        public HTMLElementsCollection(IHTMLElement parent)
        {
            m_parent = parent;
        }
        #endregion

        #region Class public methods

        /// <summary>
        /// Adds the specified element into the collection.
        /// </summary>
        /// <param name="element">Element to add into the collection.</param>
        /// <returns>Index of the element added to the collection.</returns>
        public int Add(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return InnerList.Add(element);
        }

        /// <summary>
        /// Creates an element from it's string representation and adds it to the collection.
        /// </summary>
        /// <param name="outerHtml">String representation of the element.</param>
        /// <returns>Index of the element in the collection if created; -1 otherwise.</returns>
        public int Add(string outerHtml)
        {
            if (outerHtml == null)
                throw new ArgumentNullException("outerHtml");
            if (outerHtml.Length == 0)
                throw new ArgumentException("outerHtml - string can not be empty");

            int index = -1;
            BaseElement result = CreateElement(outerHtml);

            if (result != null && ParentEx != null)
            {
                // Add XML data to the parent XML data.
                this.ParentEx.Storage.AppendChild(result.Storage);

                // Add to collection.
                index = IndexOf(result);

                if (index == -1)
                {
                    index = List.Add(result);
                }

                InputHTML parentDoc = result.Document;

                // Change document's appearance.
                if (parentDoc != null && !parentDoc.IsDisposed)
                {
                    parentDoc.Reaction |= ReactLevel.ReCalculateDoc;
                    parentDoc.Reaction |= ReactLevel.RePaintdoc;
                    parentDoc.PerformChanges();
                }
            }

            return index;
        }

        /// <summary>
        /// Adds an array of elements into the collections.
        /// </summary>
        /// <param name="values">Array of elements.</param>
        public void AddRange(IHTMLElement[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            for (int i = 0; i < values.Length; i++)
            {
                InnerList.Add(values[i]);
            }
        }

        /// <summary>
        /// Indicates whether collection contains the specified element.
        /// </summary>
        /// <param name="element">Element to check.</param>
        /// <returns>True if item exists in collection; false otherwise.</returns>
        public bool Contains(IHTMLElement element)
        {
            if (element == null) return false;

            return InnerList.Contains(element);
        }

        /// <summary>
        /// Returns the index of the element from the collection.
        /// </summary>
        /// <param name="element">Element whose index is needed.</param>
        /// <returns>Zero-based index of the item in the collection.</returns>
        public int IndexOf(IHTMLElement element)
        {
            return InnerList.IndexOf(element);
        }

        /// <summary>
        /// Inserts the element into the specified position.
        /// </summary>
        /// <param name="index">Place where element must be placed.</param>
        /// <param name="element">Element to place.</param>
        public void Insert(int index, IHTMLElement element)
        {
            if (index < 0 || index > List.Count)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0 and greater List.Count");

            InnerList.Insert(index, element);
        }

        /// <summary>
        /// Creates an element from it's string representation and inserts it
        /// into the specified position.
        /// </summary>
        /// <param name="index">Index where element must be placed.</param>
        /// <param name="outerHtml">String representation of the element.</param>
        public void Insert(int index, string outerHtml)
        {
            if (index < 0 || index > List.Count)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0 and greater List.Count");

            BaseElement result = CreateElement(outerHtml);

            if (result != null && this.ParentEx != null)
            {
                XmlElement refChild = null;

                if (index > 0)
                {
                    BaseElement prevElement = this[index - 1] as BaseElement;
                    refChild = prevElement.Storage;
                }

                // Insert XML data into the parent XML data.
                this.ParentEx.Storage.InsertAfter(result.Storage, refChild);

                // Insert element into collection.
                int curIndex = IndexOf(result);

                if (curIndex != -1)
                {
                    InnerList.Remove(result);
                }

                List.Insert(index, result);

                InputHTML parentDoc = result.Document;

                // Change document's appearance.
                if (parentDoc != null && !parentDoc.IsDisposed)
                {
                    parentDoc.Reaction |= ReactLevel.ReCalculateDoc;
                    parentDoc.Reaction |= ReactLevel.RePaintdoc;
                    parentDoc.PerformChanges();
                }
            }
        }

        /// <summary>
        /// Overloaded. Removes the specified element from the collection and disposes it.
        /// </summary>
        /// <param name="element">Element to remove.</param>
        public void Remove(IHTMLElement element)
        {
            if (element == null) return;

            Remove(element, true);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="element">Element to be removed from the collection.</param>
        /// <param name="bDispose">Indicates whether to dispose the element.</param>
        public void Remove(IHTMLElement element, bool bDispose)
        {
            if (element == null) return;

            BaseElement elm = element as BaseElement;

            // Remove element from XML tree.
            if (m_parent != null && elm != null && !elm.IsDisposed &&
              ParentEx.Storage != null && elm.Storage.ParentNode == ParentEx.Storage)
            {
                this.ParentEx.Storage.RemoveChild(elm.Storage);
            }

            // Unregister element from the document.
            if (this.ParentEx != null && this.ParentEx.Control != null &&
              !this.ParentEx.IsDisposed && !elm.IsDisposed)
            {
                this.ParentEx.Document.RemoveElement(elm);
            }

            // Remove from the list.
            if (List.Contains(element))
            {
                List.Remove(element);
            }

            InputHTML parentDoc = elm.Document;

            if (bDispose)
            {
                if (elm != null && !elm.IsDisposed)
                {
                    elm.Dispose();
                }
            }

            // Change document's appearance.
            if (parentDoc != null && !parentDoc.IsDisposed)
            {
                parentDoc.Reaction |= ReactLevel.ReCalculateDoc;
                parentDoc.Reaction |= ReactLevel.RePaintdoc;
                parentDoc.PerformChanges();
            }
        }

        /// <summary>
        /// Overloaded. Clears and disposes the collection of child elements.
        /// </summary>
        public new void Clear()
        {
            Clear(true);
        }

        /// <summary>
        /// Clears the collection of child elements.
        /// </summary>
        /// <param name="bDispose">Indicates whether to dispose all children.</param>
        public void Clear(bool bDispose)
        {
            BaseElement elm = null;

            for (int i = 0, len = List.Count; i < len; i++)
            {
                elm = List[i] as BaseElement;

                if (elm != null)
                {
                    if (m_parent != null && elm != null && !elm.IsDisposed &&
                      ParentEx.Storage != null && elm.Storage.ParentNode == ParentEx.Storage)
                    {
                        BaseElement parent = m_parent as BaseElement;
                        parent.Storage.RemoveChild(elm.Storage);
                    }

                    // Unregister element from the document.
                    if (this.ParentEx != null && this.ParentEx.Control != null &&
                      !this.ParentEx.IsDisposed && !elm.IsDisposed)
                    {
                        this.ParentEx.Document.RemoveElement(elm);
                    }

                    if (bDispose && !elm.IsDisposed)
                    {
                        elm.Dispose();
                    }
                }
            }

            base.Clear();
        }

        /// <summary>
        /// Search method. Returns a reference on the element with the specified ID.
        /// </summary>
        /// <param name="id">Unique id of the element.</param>
        /// <returns>NULL if nothing is found; element reference otherwise.</returns>
        public IHTMLElement GetElementByID(string id)
        {
            if (id == null)
                throw new ArgumentNullException("name");

            if (id.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            object el = null;

            for (int i = 0, len = InnerList.Count; i < len; i++)
            {
                el = InnerList[i];
                if (el is IHTMLElement)
                {
                    IHTMLElement elm = el as IHTMLElement;

                    if (elm.ID == id) return elm;
                }
            }

            return null;
        }

        /// <summary>
        /// Search method. Returns a reference of the element with the specified unique ID.
        /// </summary>
        /// <param name="id">Unique id of the element.</param>
        /// <returns>NULL if nothing is found; element reference otherwise.</returns>
        public IHTMLElement GetElementByUniqueID(string id)
        {
            if (id == null)
                throw new ArgumentNullException("name");

            if (id.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            object el = null;

            for (int i = 0, len = InnerList.Count; i < len; i++)
            {
                el = InnerList[i];
                if (el is IHTMLElement)
                {
                    IHTMLElement elm = el as IHTMLElement;

                    if (elm.UniqueID == id) return elm;
                }
            }

            return null;
        }

        /// <summary>
        /// Overloaded. Returns an array of elements with the specified name.
        /// </summary>
        /// <param name="name">Name of the element for returning.</param>
        /// <returns>Array of elements with such a name.</returns>
        public IHTMLElement[] GetElementsByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            ArrayList list = new ArrayList();

            object el = null;
            for (int i = 0, len = InnerList.Count; i < len; i++)
            {
                el = InnerList[i];
                if (el is IHTMLElement)
                {
                    IHTMLElement elm = el as IHTMLElement;
                    if (Utilities.StrEquals(elm.Name, name))
                    {
                        list.Add(elm);
                    }
                }
            }

            return (IHTMLElement[])list.ToArray(typeof(IHTMLElement));
        }

        /// <summary>
        /// Returns an array of elements with name from the names array.
        /// </summary>
        /// <param name="names">Array of names of the element for returning.</param>
        /// <returns>Elements with such names.</returns>
        public IHTMLElement[] GetElementsByName(string[] names)
        {
            if (names == null)
                throw new ArgumentNullException("name");

            if (names.Length == 0)
                throw new ArgumentException("name - string array can not be empty");

            ArrayList list = new ArrayList();
            Hashtable childNames = InfillHashByChildNames(names);

            object el = null;
            for (int i = 0, len = InnerList.Count; i < len; i++)
            {
                el = InnerList[i];
                if (el is IHTMLElement)
                {
                    IHTMLElement elm = el as IHTMLElement;
                    if (childNames.ContainsKey(elm.Name))
                    {
                        list.Add(elm);
                    }
                }
            }

            return (IHTMLElement[])list.ToArray(typeof(IHTMLElement));
        }

        /// <summary>
        /// Returns an array of elements from the collection.
        /// </summary>
        /// <returns>Array from the collection.</returns>
        internal ArrayList GetArray()
        {
            return InnerList;
        }
        #endregion
        #region Class utility methods

        /// <summary>
        /// Infills hashtable by names of child elements.
        /// </summary>
        /// <param name="names">Array of names.</param>
        /// <returns>Hashtable of child names.</returns>
        private Hashtable InfillHashByChildNames(string[] names)
        {
            if (names == null)
                throw new ArgumentNullException("name");

            if (names.Length == 0)
                throw new ArgumentException("name - string array can not be empty");

            Hashtable result = CollectionsUtil.CreateCaseInsensitiveHashtable(names.Length);

            for (int i = 0; i < names.Length; i++)
            {
                result[names[i]] = 1;
            }

            return result;
        }

        /// <summary>
        /// Creates an element from it's string representation.
        /// </summary>
        /// <param name="outerHtml">String representation of the element.</param>
        /// <returns>Element if created; Null otherwise.</returns>
        private BaseElement CreateElement(string outerHtml)
        {
            if (outerHtml == null)
                throw new ArgumentNullException("outerHtml");
            if (outerHtml.Length == 0)
                throw new ArgumentException("outerHtml - string can not be empty");

            BaseElement elm = null;

            if (this.ParentEx != null)
            {
                // Correct element's data.
                XmlElement xmlElm = HTMLUIControl.HTMLParser.ParseString(outerHtml);

                // Get XmlElement from the XML data.
                XmlElement element = GetrFirstElement(xmlElm);

                if (element != null)
                {
                    // Create element.
                    elm = this.ParentEx.Control.ConvertDocument(element, this.ParentEx, this.ParentEx.Document);

                    // Create format for this element.
                    bool quiteMode = elm.Document.QuietMode;
                    elm.Document.QuietMode = true;

                    elm.Document.RecreateFormatElement(elm);

                    elm.Document.Reaction = ReactLevel.None;
                    elm.Document.QuietMode = quiteMode;
                }
            }

            return elm;
        }

        /// <summary>
        /// Searches for the first child of XmlElement type.
        /// </summary>
        /// <param name="parent">Parent XmlElement.</param>
        /// <returns>XmlElement object if found; Null otherwise.</returns>
        private XmlElement GetrFirstElement(XmlElement parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            XmlElement result = null;
            XmlNode node = null;

            if (parent.HasChildNodes)
            {
                node = parent.FirstChild;

                while (node != null)
                {
                    result = node as XmlElement;

                    if (result != null)
                    {
                        break;
                    }

                    node = parent.NextSibling;
                }
            }

            // Change parent document for XML data.
            if (this.ParentEx != null && result != null)
            {
                XmlDocument parentDocument = this.ParentEx.Document.Document;
                result = parentDocument.ImportNode(result, true) as XmlElement;
            }
            else
            {
                result = null;
            }

            return result;
        }
        #endregion
    }
}