#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.PdfViewer
{
    class VirtualizationList<T> : IList<T>, IList
    {
        # region Fields
        const int m_gapBetweenPages = 4;
        private ScrollViewer m_virtualScroll;
        private CustomVPanel m_virtualPanel;
        private DispatcherTimer m_dispatcherTimer = new DispatcherTimer();
        private readonly Dictionary<int, IList<T>> m_pages = new Dictionary<int, IList<T>>();
        private readonly Dictionary<int, DateTime> m_pageTouchTimes = new Dictionary<int, DateTime>();
        private double m_previousPosition;
        internal List<int> m_rend = new List<int>();
        private readonly int m_Size = 1;
        private readonly ItemsProvider<T> m_itemsProvider;
        private int m_count = -1;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizationList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        public VirtualizationList(ItemsProvider<T> itemsProvider, ScrollViewer viewer, CustomVPanel customPanel)
        {
            m_virtualScroll = viewer;
            m_itemsProvider = itemsProvider;
            m_virtualPanel = customPanel;
            m_dispatcherTimer.Tick += new EventHandler(m_dispatcherTimer_Tick);
            m_dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            m_dispatcherTimer.Start();
        }
        #endregion

        #region Methods
        public void Unload()
        {
            m_virtualPanel.Pages = null;
            m_itemsProvider.CurrentPage = 0;
            m_pages.Clear();
        }

        void ComputePageBounds()
        {
            //Zoom factor will affect the page size.
            double top = m_gapBetweenPages;
            for (int i = 0; i < m_virtualPanel.Pages.Length; i++)
            {
                Rect pageBounds = new Rect(0, top, m_virtualPanel.Pages[i].Width, m_virtualPanel.Pages[i].Height);
                m_virtualPanel.Pages[i].Bounds = pageBounds;
                top += (m_virtualPanel.Pages[i].Bounds.Height + m_gapBetweenPages);
            }
        }

        void m_dispatcherTimer_Tick(object sender, EventArgs e)
        {
            int firstIndexItem, lastIndexItem;
            m_virtualPanel.VisibleRange(out firstIndexItem, out lastIndexItem);
            int i ;
            m_itemsProvider.CurrentPage = firstIndexItem;
            
                if (this.m_virtualScroll.VerticalOffset == m_previousPosition)
                {
                    for (i = firstIndexItem; i <= lastIndexItem; i++)
                    {
                        if (!m_rend.Contains(i))
                        {
                            m_rend.Add(i);
                            Request(i);
                            ComputePageBounds();
                        }
                    }
                }
                m_previousPosition = m_virtualScroll.VerticalOffset;
            
            List<int> temprenderPage = new List<int>(m_rend);
            foreach (int pageRender in temprenderPage)
            {
                if (pageRender < firstIndexItem - 1 || pageRender > lastIndexItem + 1)
                {
                    CleanPages(pageRender);
                    m_rend.Remove(pageRender);
                }
            }
            m_previousPosition = m_virtualScroll.VerticalOffset;
        }

        /// <summary>
        /// Cleans up any stale pages that have not been accessed in the period dictated by PageTimeout.
        /// </summary>
        public void CleanPages(int key)
        {
            var timeOut = TimeSpan.FromMilliseconds(50);
            List<int> keys = new List<int>(m_pageTouchTimes.Keys);
            m_pages.Remove(key);
            m_pageTouchTimes.Remove(key);
        }

        public void CleanAllPages()
        {
            m_pages.Clear();
            m_rend.Clear();
            m_pageTouchTimes.Clear();
        }

        internal virtual void RePopulatePage(int pageIndex, IList<T> page)
        {
            if (m_pages.ContainsKey(pageIndex))
                m_pages[pageIndex] = page;
        }
        /// <summary>
        /// Populates the page within the dictionary.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="page">The page.</param>
        protected virtual void Populate(int pageIndex, IList<T> page)
        {
            if (m_pages.ContainsKey(pageIndex))
                m_pages[pageIndex] = page;
        }

        /// <summary>
        /// Makes a request for the specified page, creating the necessary slots in the dictionary,
        /// and updating the page touch time.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void Request(int pageIndex)
        {

            m_pages.Remove(pageIndex);
            m_pages.Add(pageIndex, null);
            Load(pageIndex);
        }

        /// <summary>
        /// Loads the count of items.
        /// </summary>
        protected virtual void LoadItemCount()
        {
            Count = FetchItemCount();
        }

        /// <summary>
        /// Loads the page of items.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void Load(int pageIndex)
        {
            Thread STAThread = new Thread(() =>
                    {
                        Populate(pageIndex, Fetch(pageIndex));
                    });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.IsBackground = true;
            STAThread.Start();
        }
        protected virtual void LoadBlankPage(int pageIndex)
        {

            Populate(pageIndex, FetchBlankPage(pageIndex));

        }
        /// <summary>
        /// Fetches the requested page from the ItemsProvider.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns></returns>
        protected IList<T> Fetch(int pageIndex)
        {
            return ItemsProvider.GetRange(pageIndex, Size);
        }
        protected IList<T> FetchBlankPage(int pageIndex)
        {
            return ItemsProvider.GetBlankPage(pageIndex);
        }
        /// <summary>
        /// Fetches the count of itmes from the ItemsProvider.
        /// </summary>
        /// <returns></returns>
        protected int FetchItemCount()
        {
            return ItemsProvider.Count();
        }

        #endregion

        #region ItemsProvider

        /// <summary>
        /// Gets the items provider.
        /// </summary>
        /// <value>The items provider.</value>
        public ItemsProvider<T> ItemsProvider
        {
            get { return m_itemsProvider; }
        }

        #endregion

        #region PageSize

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int Size
        {
            get { return m_Size; }
        }

        #endregion

        #region IList<T>, IList
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public virtual int Count
        {
            get
            {
                if (m_count == -1)
                {
                    LoadItemCount();
                }
                return m_count;
            }
            protected set
            {
                m_count = value;
            }
        }

        /// <summary>
        /// Gets the item at the specified index. 
        /// </summary>
        public T this[int index]
        {
            get
            {
                int pageIndex = index / Size;
                int pageOffset = index % Size;
                RequestBlankPage(index);
                if (!m_pages.ContainsKey(pageIndex) || m_pages[pageIndex] == null)
                    return default(T);

                return m_pages[pageIndex][pageOffset];
            }
            set { throw new NotSupportedException(); }
        }
        /// <summary>
        /// Makes a request for the specified page, creating the necessary slots in the dictionary,
        /// and updating the page touch time.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void RequestBlankPage(int pageIndex)
        {
            if (!m_pages.ContainsKey(pageIndex))
            {
                m_pages.Add(pageIndex, null);
                LoadBlankPage(pageIndex);
            }
        }
        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Contains(T item)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        /// <summary>
        /// 
        /// </summary>
        public int IndexOf(T item)
        {
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        #endregion
    }
}

