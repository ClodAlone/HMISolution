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

using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Collection for formats.
    /// </summary>
    public class HTMLFormatsCollection : EventBaseCollection, IHTMLFormatsCollection, IDisposable
    {
        #region Class members

        /// <summary>
        /// Storage of parent property.
        /// </summary>
        private IHTMLElement m_parent;

        /// <summary>
        /// Storage of name of format - to - format.
        /// </summary>
        private IDictionary m_dict = CollectionsUtil.CreateCaseInsensitiveHashtable();

        /// <summary>
        /// Indicates whether we were disposed once.
        /// </summary>
        private bool m_bDisposed;
        #endregion

        #region Class Properties

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
        /// Gets or sets the attribute with the specified index.
        /// </summary>
        /// <param name="index">An integer index value</param>
        public IHTMLFormat this[int index]
        {
            get
            {
                return (IHTMLFormat)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Gets or sets the format with the specified name. Name is case insensitive.
        /// </summary>
        /// <param name="name">A string value </param>
        public IHTMLFormat this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                if (name.Length == 0)
                    throw new ArgumentException("name can not be empty");

                if (!m_dict.Contains(name)) return null;

                return (IHTMLFormat)m_dict[name];
            }
            set
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                if (name.Length == 0)
                    throw new ArgumentException("name can not be empty");

                if (!m_dict.Contains(name))
                {
                    //// add new
                    this.Add(value);
                }
                else
                {
                    //// replace
                    List[InnerList.IndexOf(m_dict[name])] = value;
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the HTMLFormatsCollection class
        /// </summary>
        public HTMLFormatsCollection()
        {
            this.Inserted += new CollectionEventHandler(HTMLFormatsCollection_Inserted);
            this.Removed += new CollectionEventHandler(HTMLFormatsCollection_Removed);
            this.Set += new CollectionEventHandler(HTMLFormatsCollection_Set);
            this.Cleared += new CollectionEventHandler(HTMLFormatsCollection_Cleared);
        }

        /// <summary>
        /// Initializes a new instance of the HTMLFormatsCollection class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public HTMLFormatsCollection(IHTMLElement parent)
        {
            m_parent = parent;
        }

        /// <summary>
        /// Finalizes an instance of the HTMLFormatsCollection class
        /// </summary>
        ~HTMLFormatsCollection()
        {
            this.Inserted -= new CollectionEventHandler(HTMLFormatsCollection_Inserted);
            this.Removed -= new CollectionEventHandler(HTMLFormatsCollection_Removed);
            this.Set -= new CollectionEventHandler(HTMLFormatsCollection_Set);
            this.Cleared -= new CollectionEventHandler(HTMLFormatsCollection_Cleared);
        }
        #endregion

        #region Class Helper Methods

        /// <summary>
        /// Adds the specified format into the collection.
        /// </summary>
        /// <param name="format">Format for adding into collection.</param>
        /// <returns>Index in the collection.</returns>
        public int Add(IHTMLFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            if (m_dict.Contains(format.Name))
                throw new ArgumentException("Two formats with the same name can not be added into collection.");

            return List.Add(format);
        }

        /// <summary>
        /// Adds the range of formats into the collection.
        /// </summary>
        /// <param name="formats">Array of formats.</param>
        public void AddRange(IHTMLFormat[] formats)
        {
            if (formats == null)
                throw new ArgumentNullException("formats");

            for (int i = 0; i < formats.Length; i++)
            {
                List.Add(formats[i]);
            }
        }

        /// <summary>
        /// Indicates whether the collection contains the specified format.
        /// </summary>
        /// <param name="format">Format reference for check.</param>
        /// <returns>True if the collection contains such a format; false otherwise.</returns>
        public bool Contains(IHTMLFormat format)
        {
            if (format == null) return false;

            return InnerList.Contains(format);
        }

        /// <summary>
        /// Indicates whether the collection contains the format with specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the format.</param>
        /// <returns>True if the collection contains such a format; false otherwise.</returns>
        public bool Contains(string name)
        {
            if (name == null || name.Length == 0) return false;

            return m_dict.Contains(name);
        }

        /// <summary>
        /// Returns the index of the specified format; if collection does not contain such format, it will
        /// return -1.
        /// </summary>
        /// <param name="format">Format whose index is needed.</param>
        /// <returns>Zero-based index of format; -1 otherwise.</returns>
        public int IndexOf(IHTMLFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return InnerList.IndexOf(format);
        }

        /// <summary>
        /// Returns the index of format of the format which contains the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the format.</param>
        /// <returns>Zero-based index of format; -1 otherwise.</returns>
        public int IndexOf(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name can not be empty");

            if (!m_dict.Contains(name)) return -1;

            return this.IndexOf(this[name]);
        }

        /// <summary>
        /// Removes the specified format from the collection, if it belongs to it.
        /// </summary>
        /// <param name="format">Reference of the format which must be removed from the collection.</param>
        public void Remove(IHTMLFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            List.Remove(format);
        }

        /// <summary>
        /// Removes the format with the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of format which must be removed.</param>
        public void Remove(string name)
        {
            this.Remove(this[name]);
        }
        #endregion

        #region Class utility methods

        /// <summary>
        /// Catch inserted items.
        /// </summary>
        /// <param name="sender">Source that triggers the event</param>
        /// <param name="e">A CollectionEventArgs object</param>
        private void HTMLFormatsCollection_Inserted(object sender, CollectionEventArgs e)
        {
            IHTMLFormat format = (IHTMLFormat)e.Value;
            m_dict[format.Name] = format;
        }

        /// <summary>
        /// Catch removed items.
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">CollectionEventArgs instance</param>
        private void HTMLFormatsCollection_Removed(object sender, CollectionEventArgs e)
        {
            IHTMLFormat format = (IHTMLFormat)e.Value;
            m_dict.Remove(format.Name);
        }

        /// <summary>
        /// Catch replacement of items.
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">CollectionEventArgs instance</param>
        private void HTMLFormatsCollection_Set(object sender, CollectionEventArgs e)
        {
            IHTMLFormat format = (IHTMLFormat)e.Value;
            IHTMLFormat format2 = (IHTMLFormat)e.OldValue;

            m_dict.Remove(format2.Name);
            m_dict[format.Name] = format;
        }

        /// <summary>
        /// Catch collection cleaning.
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">CollectionEventArgs instance</param>
        private void HTMLFormatsCollection_Cleared(object sender, CollectionEventArgs e)
        {
            m_dict.Clear();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Clears all resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_bDisposed)
            {
                if (m_dict != null)
                {
                    m_dict.Clear();
                    m_dict = null;
                }

                // Set flag that we were disposed.
                m_bDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}