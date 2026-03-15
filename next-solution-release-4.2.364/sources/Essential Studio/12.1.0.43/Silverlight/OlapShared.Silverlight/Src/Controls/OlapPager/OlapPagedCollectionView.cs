#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Collections;

namespace Syncfusion.Silverlight.Shared.Olap
{
    /// <summary>
    /// OlapPagedCollectionView represents the Page view for OLAP Pager
    /// </summary>
    public class OlapPagedCollectionView : IPagedCollectionView, IEnumerable
    {
        #region [ Private Variables ]

        int totalItemCount = -1;
        int pageIndex = 0;
        int pageSize = 20;

        #endregion

        #region [ Constructor ]
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapPagedCollectionView"/> class.
        /// </summary>
        public OlapPagedCollectionView() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapPagedCollectionView"/> class.
        /// </summary>
        /// <param name="totalItemCount">The total item count.</param>
        /// <param name="pageIndex">Index of the page.</param>
        public OlapPagedCollectionView(int totalItemCount, int pageIndex)
        {
            SetTotalItemCount(totalItemCount);
            SetPageIndex(pageIndex);
        } 

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the number of pages we currently have
        /// </summary>
        public int PageCount
        {
            get { return (this.pageSize > 0) ? Math.Max(1, (int)Math.Ceiling((double)this.ItemCount / this.pageSize)) : 0; }
        }

        #endregion

        #region IPagedCollectionView
        
        public bool CanChangePage
        {
            get { return true; }
        }

        public bool IsPageChanging
        {
            get { return false; }
        }

        public int ItemCount
        {
            get { return this.TotalItemCount; }
        }

        public bool MoveToFirstPage()
        {
            return this.MoveToPage(0);
        }

        public bool MoveToLastPage()
        {
            return this.MoveToPage(this.PageCount - 1);
        }

        public bool MoveToNextPage()
        {
            return this.MoveToPage(this.pageIndex + 1);
        }

        public bool MoveToPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= this.PageCount) return false;

            if (!RaisePageChangingEvent(pageIndex))
            {
                this.pageIndex = pageIndex;
                RaisePageChangedEvent();
            }

            return true;
        }

        public bool MoveToPreviousPage()
        {
            return this.MoveToPage(this.pageIndex - 1);
        }

        public event EventHandler<EventArgs> PageChanged;

        public event EventHandler<PageChangingEventArgs> PageChanging;

        public int PageIndex
        {
            get { return this.pageIndex; }
        }

        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                this.pageSize = value;
            }
        }

        public int TotalItemCount
        {
            get { return totalItemCount; }
        }
        #endregion

        #region [ Public Helper Methods ]

        /// <summary>
        /// Sets the total item count.
        /// </summary>
        /// <param name="totalItemCount">The total item count.</param>
        public void SetTotalItemCount(int totalItemCount)
        {
            this.totalItemCount = totalItemCount;
        }

        /// <summary>
        /// Sets the index of the page.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        public void SetPageIndex(int pageIndex)
        {
            this.pageIndex = pageIndex;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return null;
        }
        #endregion

        #region [ Protected Methods ]

        /// <summary>
        /// Raises the page changing event.
        /// </summary>
        /// <param name="newPageIndex">New index of the page.</param>
        /// <returns></returns>
        protected bool RaisePageChangingEvent(int newPageIndex)
        {
            var args = new PageChangingEventArgs(newPageIndex);

            if (this.PageChanging != null)
                this.PageChanging(this, args);

            return args.Cancel;
        }

        /// <summary>
        /// Raises the page changed event.
        /// </summary>
        protected void RaisePageChangedEvent()
        {
            if (this.PageChanged != null)
                this.PageChanged(this, EventArgs.Empty);
        } 

        #endregion
    }
}
