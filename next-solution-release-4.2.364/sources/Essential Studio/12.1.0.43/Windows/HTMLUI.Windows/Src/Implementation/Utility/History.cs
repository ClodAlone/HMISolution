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
using System.ComponentModel;

using Syncfusion.Windows.Forms.HTMLUI;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    #region enum
    /// <summary>
    /// Type of path in history.
    /// </summary>
    public enum PathType
    {
        /// <summary>
        /// Type is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Document is local file.
        /// </summary>
        File,

        /// <summary>
        /// Document is Uri resource.
        /// </summary>
        Uri
    }
    #endregion

    /// <summary>
    /// Class that represents the history of loaded HTML documents for the control.
    /// </summary>
    public class History : IDisposable
    {
        #region Class members
        /// <summary>
        /// Index of current position in the history stack.
        /// </summary>
        private int m_historyIndex;

        /// <summary>
        /// Holds path of the document as key and the history index of this path as value.
        /// </summary>
        private Hashtable m_PathIndex;

        /// <summary>
        /// Holds the path of document as value and the history index of this path as key.
        /// </summary>
        private Hashtable m_IndexPath;

        /// <summary>
        /// Holds the type of path. FileName or Uri.
        /// </summary>
        private Hashtable m_indexType;

        /// <summary>
        /// Array holding all visited links of the control.
        /// </summary>
        private ArrayList m_visitedLinks;

        /// <summary>
        /// Indicates whether the object has been disposed.
        /// </summary>
        private bool m_bDisposed;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets the number of items in the history.
        /// </summary>
        public int Count
        {
            get
            {
                return m_IndexPath.Count;
            }
        }

        /// <summary>
        /// Gets the current index in the history.
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return m_historyIndex;
            }
        }

        /// <summary>
        /// Gets the type of path for the document with the specified index.
        /// </summary>
        /// <param name="index">Integer index value</param>
        protected PathType this[int index]
        {
            get
            {
                return (PathType)m_indexType[index];
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the History class
        /// </summary>
        public History()
        {
            m_IndexPath = new Hashtable();
            m_PathIndex = new Hashtable();
            m_indexType = new Hashtable();
            m_visitedLinks = new ArrayList();

            m_historyIndex = -1;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Pushes  the path to the document in the history.
        /// </summary>
        /// <param name="path">String path for history inserting.</param>
        /// <param name="type">Type of path.</param>
        /// <returns>Integer value</returns>
        public int Push(string path, PathType type)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException("path - string cannot be empty");

            // Path already exists in history, return index of this path.
            if (m_PathIndex.ContainsKey(path))
            {
                m_historyIndex = (int)m_PathIndex[path];
            }
            else 
            {
                // path is new.
                // We are inside history, remove all history elements above current.
                if (m_historyIndex < (m_PathIndex.Count - 1))
                {
                    int bottomBorder = m_historyIndex + 1;
                    int topBorder = m_PathIndex.Count;
                    string delPath = string.Empty;

                    for (int i = bottomBorder; i < topBorder; i++)
                    {
                        delPath = (string)m_IndexPath[i];
                        m_PathIndex.Remove(delPath);
                        m_IndexPath.Remove(i);
                        m_indexType.Remove(i);
                    }
                }

                // Add path to history.
                m_historyIndex++;
                m_PathIndex.Add(path, m_historyIndex);
                m_IndexPath.Add(m_historyIndex, path);
                m_indexType.Add(m_historyIndex, type);
                m_visitedLinks.Add(path);
            }

            return m_historyIndex;
        }

        /// <summary>
        /// Returns the path from history stack container according to the defined deep value.
        /// If step is greater than stack capacity, it returns an empty string.
        /// </summary>
        /// <param name="deep">Index in history for getting the path.</param>
        /// <returns>History item.</returns>
        /// <remarks>Deep value must be less than or equal to zero.</remarks>
        public HistoryPair Pop(int deep)
        {
            int newIndex = m_historyIndex + deep;
            int bottomBorder = 0;
            int topBorder = m_PathIndex.Count - 1;

            // Check bounds of the new index.
            newIndex = (newIndex < bottomBorder) ? bottomBorder : newIndex;
            newIndex = (newIndex > topBorder) ? topBorder : newIndex;
            m_historyIndex = newIndex;

            if (m_IndexPath.ContainsKey(newIndex))
            {
                return new HistoryPair((string)m_IndexPath[newIndex], this[newIndex]);
            }

            return new HistoryPair(string.Empty, PathType.Unknown);
        }

        /// <summary>
        /// Returns the path from history stack container according to the defined deep value.
        /// If step is greater than stack capacity, it returns an empty string.
        /// </summary>
        /// <param name="deep">Index in history for getting path.</param>
        /// <returns>History item.</returns>
        /// <remarks>Deep value must be less than or equal to zero.
        /// This method does not remove any elements from the history stack.</remarks>
        public HistoryPair GetAt(int deep)
        {
            int oldIndex = m_historyIndex;
            HistoryPair result = Pop(deep);
            m_historyIndex = oldIndex;

            return result;
        }

        /// <summary>
        /// Indicates whether the specified path already exists in the history.
        /// </summary>
        /// <param name="path">Path for searching.</param>
        /// <returns>True if history already contains such path; false otherwise.</returns>
        protected internal bool Contains(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            return m_visitedLinks.Contains(path);
        }

        /// <summary>
        /// Resets the list of visited links.
        /// </summary>
        public void ResetVisited()
        {
            m_visitedLinks.Clear();
        }

        /// <summary>
        /// Excludes the link element from the list of visited links.
        /// </summary>
        /// <param name="linkElement">Element for excluding from visited list.</param>
        public void ExcludeFromVisited(AElementImpl linkElement)
        {
            if (linkElement == null)
                throw new ArgumentNullException("linkElement");

            string path = linkElement.GetPath();

            if (path != null)
            {
                m_visitedLinks.Remove(path);
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Disposes object.
        /// </summary>
        public void Dispose()
        {
            if (!m_bDisposed)
            {
                if (m_PathIndex != null)
                {
                    m_PathIndex.Clear();
                    m_PathIndex = null;
                }

                if (m_IndexPath != null)
                {
                    m_IndexPath.Clear();
                    m_IndexPath = null;
                }

                if (m_indexType != null)
                {
                    m_indexType.Clear();
                    m_indexType = null;
                }

                if (m_visitedLinks != null)
                {
                    m_visitedLinks.Clear();
                    m_visitedLinks = null;
                }

                m_bDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }

    /// <summary>
    /// Class that represents pairs of history data.
    /// </summary>
    public class HistoryPair
    {
        #region HistoryPair class members
        /// <summary>
        /// Key of the history.
        /// </summary>
        private string m_key;

        /// <summary>
        /// Type of the item.
        /// </summary>
        private PathType m_tag;
        #endregion

        #region HistoryPair class properties
        /// <summary>
        /// Gets or sets the key in the history.
        /// </summary>
        public string Key
        {
            get
            {
                return m_key;
            }
            set
            {
                if (value != m_key)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_key, value);
                    m_key = value;
                    OnKeyChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the item in the history.
        /// </summary>
        public PathType Tag
        {
            get
            {
                return m_tag;
            }
            set
            {
                if (value != m_tag)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_tag, value);
                    m_tag = value;
                    OnTagChanged(args);
                }
            }
        }
        #endregion

        #region HistoryPair Class events
        /// <summary>
        /// Event. Raised when key has been changed.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler KeyChanged;

        /// <summary>
        /// Event. Raised when type of item has been changed.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler TagChanged;
        #endregion

        #region HistoryPair Class event Raisers
        /// <summary>
        /// Raises KeyChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseKeyChanged(ValueChangedEventArgs args)
        {
            if (KeyChanged != null)
            {
                KeyChanged(this, args);
            }
        }

        /// <summary>
        /// Raises TagChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseTagChanged(ValueChangedEventArgs args)
        {
            if (TagChanged != null)
            {
                TagChanged(this, args);
            }
        }
        #endregion

        #region HistoryPair initialize methods
        /// <summary>
        /// Initializes a new instance of the HistoryPair class
        /// </summary>
        /// <param name="key">String key value</param>
        /// <param name="tag">PathType instance</param>
        public HistoryPair(string key, PathType tag)
        {
            m_key = key;
            m_tag = tag;
        }
        #endregion

        #region HistoryPair class overrides
        /// <summary>
        /// Raises KeyChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnKeyChanged(ValueChangedEventArgs args)
        {
            RaiseKeyChanged(args);
        }

        /// <summary>
        /// Raises TagChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnTagChanged(ValueChangedEventArgs args)
        {
            RaiseTagChanged(args);
        }
        #endregion
    }
}
