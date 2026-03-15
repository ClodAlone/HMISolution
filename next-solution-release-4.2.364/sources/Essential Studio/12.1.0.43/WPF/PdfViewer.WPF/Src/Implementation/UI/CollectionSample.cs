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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Threading;

namespace Syncfusion.Windows.PdfViewer
{
    class AsyncVList<T> : VirtualizationList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        # region Fields
        public ScrollViewer m_virtualScroll;
        private readonly SynchronizationContext m_synchronizationContext;
        private bool m_isLoading;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncVList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        public AsyncVList(ItemsProvider<T> itemsProvider, ScrollViewer viewer, CustomVPanel virtualPanel)
            : base(itemsProvider, viewer, virtualPanel)
        {
            m_synchronizationContext = SynchronizationContext.Current;
            m_virtualScroll = viewer;
        }
        #endregion

        #region INotifyCollectionChanged

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler h = CollectionChanged;
            if (h != null)
                h(this, e);
        }

        /// <summary>
        /// Fires the collection reset event.
        /// </summary>
        private void FireCollectionReset(object args)
        {
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(e);
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler h = PropertyChanged;
            if (h != null)
                h(this, e);
        }

        /// <summary>
        /// Fires the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void FirePropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
            OnPropertyChanged(e);
        }

        #endregion
        
        /// <summary>
        /// Gets the synchronization context used for UI-related operations.
        /// </summary>
        /// <value>The synchronization context.</value>
        protected SynchronizationContext SynchronizationContext
        {
            get { return m_synchronizationContext; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the collection is loading.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this collection is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get
            {
                return m_isLoading;
            }
            set
            {
                if (value != m_isLoading)
                {
                    m_isLoading = value;
                    FirePropertyChanged("IsLoading");
                }
            }
        }

        /// <summary>
        /// Asynchronously loads the count of items.
        /// </summary>
        protected override void LoadItemCount()
        {
            Count = 0;
            IsLoading = true;

            ThreadPool.QueueUserWorkItem(LoadCountWork);
        }

        /// <summary>
        /// Performed on background thread.
        /// </summary>
        private void LoadCountWork(object args)
        {
            int count = FetchItemCount();
            SynchronizationContext.Send(LoadCountCompleted, count);
        }

        /// <summary>
        /// Performed on UI-thread after LoadCountWork.
        /// </summary>
        private void LoadCountCompleted(object args)
        {
            Count = (int)args;
            IsLoading = false;
            FireCollectionReset(args);
        }

        /// <summary>
        /// Populates the page within the dictionary.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="page">The page.</param>
        internal override void RePopulatePage(int pageIndex, IList<T> page)
        {
            ThreadPool.QueueUserWorkItem(LoadCompleted, new object[] { pageIndex, page });
            base.RePopulatePage(pageIndex, page);
        }

        /// <summary>
        /// Asynchronously loads the page.
        /// </summary>
        protected override void Load(int index)
        {
            IsLoading = true;
            IList<T> page = null;
            LoadWork(new object[] { index, page });
        }
        protected override void LoadBlankPage(int index)
        {
            IsLoading = true;
            IList<T> page = FetchBlankPage(index);
            ThreadPool.QueueUserWorkItem(LoadBlankCompleted, new object[] { index, page });
        }
        private void LoadBlankCompleted(object args)
        {
            int pageIndex = (int)((object[])args)[0];
            IList<T> page = (IList<T>)((object[])args)[1];

            Populate(pageIndex, page);
            IsLoading = false;
            SynchronizationContext.Send(FireCollectionReset, new object[] { pageIndex, page });
        }
        /// <summary>
        /// Performed on background thread.
        /// </summary>
        /// <param name="args">Index of the page to load.</param>
        private void LoadWork(object args)
        {
            int pageIndex = (int)((object[])args)[0];
            IList<T> page = Fetch(pageIndex);

            ThreadPool.QueueUserWorkItem(LoadCompleted, new object[] { pageIndex, page });
        }

        /// <summary>
        /// Performed on UI-thread after LoadPageWork.
        /// </summary>
        /// <param name="args">object[] { int pageIndex, IList(T) page }</param>
        private void LoadCompleted(object args)
        {
            int pageIndex = (int)((object[])args)[0];
            IList<T> page = (IList<T>)((object[])args)[1];

            Populate(pageIndex, page);
            IsLoading = false;
            SynchronizationContext.Send(FireCollectionReset, new object[] { pageIndex, page });
        }
    }
}