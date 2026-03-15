#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// This class plays two roles: it's a base class for all bookmarks
    /// and it's a root of a bookmarks tree.
    /// </summary>
    public class PdfBookmarkBase :
        IPdfWrapper,
        IEnumerable
    {
        #region Fields
        /// <summary>
        /// Collection of the descend outlines.
        /// </summary>
        private List<PdfBookmarkBase> m_list = new List<PdfBookmarkBase>();

        /// <summary>
        /// Internal variable to store dictinary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        /// <summary>
        /// Cross table of the document;
        /// </summary>
        private PdfCrossTable m_crossTable = new PdfCrossTable();

        /// <summary>
        /// Internal variable to store loaded bookmark.
        /// </summary>
        private List<PdfBookmark> bookmark = null;

        /// <summary>
        /// Temp variable to store loaded bookmark.
        /// </summary>
        private List<PdfBookmarkBase> m_booklist = null;

        /// <summary>
        /// Gets or sets the whether the bookmark tree is expanded or not
        /// </summary>
        private bool m_isExpanded = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfOutlineBase"/> class.
        /// </summary>
        /// <remarks>Note that the Type field shouldn't be generated.</remarks>
        internal PdfBookmarkBase()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBookmarkBase"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfBookmarkBase(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            m_dictionary = dictionary;

            if (crossTable != null)
            {
                m_crossTable = crossTable;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets number of the elements in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                PdfLoadedDocument m_loadedDocument = m_crossTable.Document as PdfLoadedDocument;
                if (m_loadedDocument != null)
                {
                    if (m_booklist == null)
                    {
                        m_booklist = new List<PdfBookmarkBase>();
                        for (int n = 0; n < List.Count; n++)
                        {
                            m_booklist.Add(List[n]);
                        }
                    }

                    return List.Count;

                    //if (List.Count == 0)
                    //{
                    //    return 0;
                    //}
                    //else
                    //{
                    //    return m_booklist.Count;
                    //}
                }
                else
                {
                    return List.Count;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Interactive.PdfBookmark"/> at the specified index.
        /// </summary>
        /// <value>index</value>
        public PdfBookmark this[int index]
        {
            get
            {
                return List[index] as PdfBookmark;
            }
        }

        /// <summary>
        /// Gets the sub items.
        /// </summary>
        internal virtual List<PdfBookmarkBase> List
        {
            get
            {
                return m_list;
            }
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        internal PdfDictionary Dictionary
        {
            get
            {
                return m_dictionary;
            }
        }

        /// <summary>
        /// Gets the cross table.
        /// </summary>
        internal PdfCrossTable CrossTable
        {
            get
            {
                return m_crossTable;
            }
        }
        /// <summary>
        /// Gets or sets the whether to expand the node or not
        /// </summary>
        internal bool IsExpanded
        {
            get
            {
                if (Dictionary.ContainsKey("Count"))
                {
                    if ((Dictionary[DictionaryProperties.Count] as PdfNumber).IntValue < 0)
                    {
                        return false;
                    }
                    else
                        return true;
                }
                else
                {
                    return m_isExpanded;
                }
            }
            set
            {
                m_isExpanded = value;
                if (Count > 0)
                {
                    int count;
                    if (m_isExpanded)
                    {
                        count = List.Count;
                    }
                    else
                    {
                        count = -List.Count;
                    }
                    m_dictionary.SetNumber(DictionaryProperties.Count, count);
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates and adds an outline.
        /// </summary>
        /// <param name="title">The title of the new outline.</param>
        /// <returns>The outline created.</returns>
        public PdfBookmark Add(string title)
        {
            if (title == null)
            {
                throw new ArgumentNullException("title");
            }

            PdfBookmark previous = (Count < 1) ? null : this[Count - 1];
            PdfBookmark outline = new PdfBookmark(title, this, previous, null);

            if (previous != null)
            {
                previous.Next = outline;
            }

            List.Add(outline);
            UpdateFields();

            return outline;
        }

        /// <summary>
        /// Determines whether the specified outline is a direct descendant of the outline base.
        /// </summary>
        /// <param name="outline">The outline.</param>
        /// <returns>
        /// <c>true</c> if the specified outline is a direct descendant of the outline base;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(PdfBookmark outline)
        {
            return List.Contains(outline);
        }

        /// <summary>
        /// Removes the specified bookmark from the document.
        /// </summary>
        /// <param name="title">The title of the outline.</param>
        public void Remove(string title)
        {
            if (title == null)
            {
                throw new ArgumentNullException("title");
            }

            int index = -1;

            if (bookmark == null)
            {
                bookmark = new List<PdfBookmark>();

                if (m_crossTable.Document is PdfLoadedDocument)
                {
                    PdfLoadedDocument m_loadedDocument = m_crossTable.Document as PdfLoadedDocument;

                    Dictionary<PdfPageBase, object> pageToBookmarkDic = m_loadedDocument.CreateBookmarkDestinationDictionary();
                }

                for (int n = 0; n < List.Count; n++)
                {
                    if ((List[n] as PdfBookmark) == null)
                        throw new Exception("bookmark");

                    bookmark.Add(List[n] as PdfBookmark);

                    if (List[n].List.Count != 0)
                    {
                        for (int i = 0; i < List[n].List.Count; i++)
                        {
                            bookmark.Add(List[n].List[i] as PdfBookmark);
                        }
                    }

                }

                if (m_booklist == null)
                {
                    m_booklist = new List<PdfBookmarkBase>();
                    for (int j = 0; j < bookmark.Count; j++)
                    {
                        m_booklist.Add(bookmark[j]);
                    }
                }
            }

            for (int c = 0; c < bookmark.Count; c++)
            {
                if (bookmark[c] is PdfLoadedBookmark)
                {
                    PdfLoadedBookmark pdfbookmark = bookmark[c] as PdfLoadedBookmark;
                    if (pdfbookmark.Title.Equals(title))
                    {
                        index = c;
                        break;
                    }
                }
                else if (bookmark[c] is PdfBookmark)
                {
                    PdfBookmark pdfbookmark = bookmark[c] as PdfBookmark;
                    if (pdfbookmark.Title.Equals(title))
                    {
                        index = c;
                        break;
                    }
                }
            }

            RemoveAt(index);
        }

        /// <summary>
        /// Removes the specified bookmark from the document at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            if (bookmark == null)
            {
                bookmark = new List<PdfBookmark>();
                PdfLoadedDocument m_loadedDocument = m_crossTable.Document as PdfLoadedDocument;

                Dictionary<PdfPageBase, object> pageToBookmarkDic = m_loadedDocument.CreateBookmarkDestinationDictionary();

                for (int n = 0; n < List.Count; n++)
                {
                    if ((List[n] as PdfBookmark) == null)
                        throw new Exception("bookmark");

                    bookmark.Add(List[n] as PdfBookmark);
                }

                if (m_booklist == null)
                {
                    m_booklist = new List<PdfBookmarkBase>();
                    for (int j = 0; j < bookmark.Count; j++)
                    {
                        m_booklist.Add(bookmark[j]);
                    }
                }
            }

            if (index < 0 || index >= bookmark.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (index >= List.Count)
            {
                if (index >= bookmark.Count)
                    throw new ArgumentOutOfRangeException();
            }

            if (bookmark[index] is PdfBookmark)
            {
                PdfBookmark current = bookmark[index] as PdfBookmark;
                if (index == 0)
                {
                    if (current.Dictionary.ContainsKey(DictionaryProperties.Next) == true)
                    {
                        m_dictionary.SetProperty(DictionaryProperties.First, current.Dictionary[DictionaryProperties.Next]);
                    }
                    else if (current.Dictionary.ContainsKey(DictionaryProperties.Prev) == false)
                    {
                        if (List.Count > 1)
                        {
                            m_dictionary.SetProperty(DictionaryProperties.First, current.Dictionary[DictionaryProperties.First]);
                        }
                        else
                        {
                            m_dictionary.Remove(DictionaryProperties.First);
                            m_dictionary.Remove(DictionaryProperties.Last);
                        }
                    }
                    else
                    {
                        m_dictionary.SetProperty(DictionaryProperties.First, current.Dictionary[DictionaryProperties.Next]);
                    }
                }
                else if ((current.Parent != null) && (current.Previous == null) && (current.Next != null))
                {
                    current.Parent.Dictionary.SetProperty(DictionaryProperties.First, current.Dictionary[DictionaryProperties.Next]);
                    current.Next.Dictionary.Remove(DictionaryProperties.Prev);
                }
                else if ((current.Parent != null) && (current.Previous != null) && (current.Next != null))
                {
                    current.Previous.Dictionary.SetProperty(DictionaryProperties.Next, current.Dictionary[DictionaryProperties.Next]);
                    PdfReferenceHolder refh = current.Dictionary[DictionaryProperties.Next] as PdfReferenceHolder;
                    if (refh != null)
                    {
                        PdfDictionary dic = m_crossTable.GetObject(refh) as PdfDictionary;
                        dic.SetProperty(DictionaryProperties.Prev, current.Dictionary[DictionaryProperties.Prev]);
                    }
                }
                else if ((current.Parent != null) && (current.Previous != null) && (current.Next == null))
                {
                    current.Previous.Dictionary.Remove(DictionaryProperties.Next);
                    current.Parent.Dictionary.SetProperty(DictionaryProperties.Last, current.Dictionary[DictionaryProperties.Prev]);
                }
                else
                {
                    current.Parent.Dictionary.Remove(DictionaryProperties.First);
                }
            }
            else if (bookmark[index] is PdfLoadedBookmark)
            {
                PdfLoadedBookmark current = bookmark[index] as PdfLoadedBookmark;
                if (index == 0)
                {
                    if (current.Dictionary.ContainsKey(DictionaryProperties.Next) == true)
                    {
                        m_dictionary.SetProperty(DictionaryProperties.First, current.Dictionary[DictionaryProperties.Next]);
                    }
                    else if (current.Dictionary.ContainsKey(DictionaryProperties.Prev) == false)
                    {
                        if (List.Count > 1)
                        {
                            m_dictionary.SetProperty(DictionaryProperties.First, current.Dictionary[DictionaryProperties.First]);
                        }
                        else
                        {
                            m_dictionary.Remove(DictionaryProperties.First);
                            m_dictionary.Remove(DictionaryProperties.Last);
                        }
                    }
                    else
                    {
                        m_dictionary.SetProperty(DictionaryProperties.First, current.Dictionary[DictionaryProperties.Next]);
                    }
                }
                else if ((current.Parent != null) && (current.Previous == null) && (current.Next != null))
                {
                    current.Parent.Dictionary.SetProperty(DictionaryProperties.First, current.Dictionary[DictionaryProperties.Next]);
                    current.Next.Dictionary.Remove(DictionaryProperties.Prev);
                }
                else if ((current.Parent != null) && (current.Previous != null) && (current.Next != null))
                {
                    current.Previous.Dictionary.SetProperty(DictionaryProperties.Next, current.Dictionary[DictionaryProperties.Next]);
                    PdfReferenceHolder refh = current.Dictionary[DictionaryProperties.Next] as PdfReferenceHolder;
                    if (refh != null)
                    {
                        PdfDictionary dic = m_crossTable.GetObject(refh) as PdfDictionary;
                        dic.SetProperty(DictionaryProperties.Prev, current.Dictionary[DictionaryProperties.Prev]);
                    }
                }
                else if ((current.Parent != null) && (current.Previous != null) && (current.Next == null))
                {
                    current.Previous.Dictionary.Remove(DictionaryProperties.Next);
                    current.Parent.Dictionary.SetProperty(DictionaryProperties.Last, current.Dictionary[DictionaryProperties.Prev]);
                }
                else
                {
                    current.Parent.Dictionary.Remove(DictionaryProperties.First);
                }
            }


            m_list.RemoveAt(index);
            bookmark.RemoveAt(index);
            m_booklist.RemoveAt(index);
        }

        /// <summary>
        /// Removes all the bookmark from the document.
        /// </summary>
        public void Clear()
        {
            List.Clear();
            if (m_booklist != null)
            {
                m_booklist.Clear();
            }

            //m_dictionary.Remove(DictionaryProperties.First);
            //m_dictionary.Remove(DictionaryProperties.Last);
        }

        /// <summary>
        /// Inserts a new outline at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="title">The title of the new outline.</param>
        /// <returns>The new outline.</returns>
        public PdfBookmark Insert(int index, string title)
        {
            if (title == null)
            {
                throw new ArgumentNullException("title");
            }

            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (title == null)
            {
                throw new ArgumentNullException("title");
            }

            PdfBookmark outline;

            if (index == Count)
            {
                outline = Add(title);
            }
            else
            {
                PdfBookmark next = (PdfBookmark)this[index];
                PdfBookmark prevoius = (index == 0) ? null : (PdfBookmark)this[index - 1];

                outline = new PdfBookmark(title, this, prevoius, next);
                //List.Add(outline);
                List.Insert(index, outline);
                if (prevoius != null)
                {
                    prevoius.Next = outline;
                }

                next.Previous = outline;

                UpdateFields();
            }

            return outline;
        }

        /// <summary>
        /// To get the BookMark Collection
        /// </summary>
        /// <param name="pageBookmarks"></param>
        /// <param name="bookmarks"></param>
        private void GetBookmarkCollection(List<PdfBookmark> pageBookmarks, List<PdfBookmark> bookmarks)
        {
            if (pageBookmarks != null)
            {
                foreach (object var in pageBookmarks)
                {
                    bookmarks.Add(var as PdfBookmark);
                }
            }
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see>
        /// object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Reproduces the tree.
        /// </summary>
        /// <returns>The list of bookmark kids.</returns>
        internal void ReproduceTree()
        {
            PdfLoadedBookmark currentBookmark = GetFirstBookMark(this);

            bool isBookmark = currentBookmark != null;

            while (isBookmark && currentBookmark.m_dictionary != null)
            {
                currentBookmark.SetParent(this);
                m_list.Add(currentBookmark);
                currentBookmark = currentBookmark.Next as PdfLoadedBookmark;
                isBookmark = currentBookmark != null;
            }
        }

        /// <summary>
        /// Updates all outline dictionary fields.
        /// </summary>
        private void UpdateFields()
        {
            if (Count > 0)
            {
                int count;
                if (IsExpanded)
                {
                    count = List.Count;
                }
                else
                {
                    count = -List.Count;
                }
                m_dictionary.SetNumber(DictionaryProperties.Count,count);
                m_dictionary.SetProperty(DictionaryProperties.First, new PdfReferenceHolder(this[0]));
                m_dictionary.SetProperty(DictionaryProperties.Last, new PdfReferenceHolder(this[Count - 1]));
            }
            else
            {
                m_dictionary.Clear();
            }

            m_dictionary.Modify();
        }

        /// <summary>
        /// Gets the first book mark.
        /// </summary>
        /// <param name="bookmark">The bookmark.</param>
        /// <returns>First Bookmark</returns>
        private PdfLoadedBookmark GetFirstBookMark(PdfBookmarkBase bookmark)
        {
            PdfLoadedBookmark firstBookmark = null;
            PdfDictionary dictionary = bookmark.Dictionary;

            if (dictionary.ContainsKey(DictionaryProperties.First))
            {
                PdfDictionary first = CrossTable.GetObject(dictionary[DictionaryProperties.First]) as PdfDictionary;
                firstBookmark = new PdfLoadedBookmark(first, CrossTable);
            }

            return firstBookmark;
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion
    }
}
