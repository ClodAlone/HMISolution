#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else

#endif

namespace Syncfusion.UI.Xaml.Controls.DataPager
{
    internal class PageNavigationController
    {
        #region Private Members

        private SfDataPager dataPager;

        #endregion

        #region Ctor

        public PageNavigationController(SfDataPager pager)
        {
            dataPager = pager;
        }

        #endregion

        #region Internal Methods

        internal void MoveToPage(int pageIndex, bool isElipsisClicked)
        {
            if (CheckNewPageIsInView(pageIndex))
            {
                if (!isElipsisClicked)
                    ScrollToCurrentPage(pageIndex);
                else
                {
                    ScrollThePager(pageIndex);
                }
            }
            this.HideCurrentPage(this.dataPager.PageIndex);
            this.ShowCurrentPage(pageIndex);
        }

        internal void HideCurrentPage(int pageIndex)
        {
            var scrollableElement =
               this.dataPager.ItemGenerator.Items.FirstOrDefault(item => item.Index == pageIndex);
            if (scrollableElement != null)
            {
                (scrollableElement.Element as NumericButton).IsCurrentPage = false;
            }
        }

        internal void ShowCurrentPage(int pageIndex)
        {
            var scrollableElement =
               this.dataPager.ItemGenerator.Items.FirstOrDefault(item => item.Index == pageIndex);
            if (scrollableElement != null && !scrollableElement.IsElipsisElement)
            {
                (scrollableElement.Element as NumericButton).IsCurrentPage = true;
            }
        }

        #endregion

        #region Helper Methods

        private bool CheckNewPageIsInView(int pageIndex)
        {
            if (pageIndex < this.dataPager.ItemGenerator.ActualStartPageIndex ||
                pageIndex > this.dataPager.ItemGenerator.ActualEndPageIndex ||
                (pageIndex == 0 && this.dataPager.ItemsPanel!=null && this.dataPager.ItemsPanel.HorizontalOffset > 0))
                return true;

            return false;
        }

        private void ScrollToCurrentPage(int pageIndex)
        {
            var offSet = 0d;
            if (pageIndex < this.dataPager.ItemGenerator.ActualStartPageIndex)
            {
                if ((this.dataPager.AutoEllipsisMode == AutoEllipsisMode.Before ||
                     this.dataPager.AutoEllipsisMode == AutoEllipsisMode.Both) && pageIndex > 0)
                    pageIndex = pageIndex - 1;
                this.dataPager.ItemGenerator.VirtualStartIndex = pageIndex;
                this.dataPager.ItemGenerator.VirtualEndIndex = pageIndex +
                                                               (Math.Min(this.dataPager.NumericButtonCount,
                                                                         this.dataPager.PageCount) - 1);
            }
            else
            {
                if ((this.dataPager.AutoEllipsisMode == AutoEllipsisMode.After ||
                     this.dataPager.AutoEllipsisMode == AutoEllipsisMode.Both) && pageIndex < this.dataPager.PageCount - 1)
                {
                    pageIndex += 1;
                }
                                
                this.dataPager.ItemGenerator.VirtualEndIndex = pageIndex;
                this.dataPager.ItemGenerator.VirtualStartIndex = pageIndex - (Math.Min(this.dataPager.NumericButtonCount,this.dataPager.PageCount) - 1);

                if (this.dataPager.ItemGenerator.VirtualStartIndex < 0)
                {
                    this.dataPager.ItemGenerator.VirtualStartIndex = pageIndex-1;
                    this.dataPager.ItemGenerator.VirtualEndIndex = this.dataPager.NumericButtonCount - 1;
                }
            }
            offSet = this.dataPager.ItemGenerator.VirtualStartIndex*this.dataPager.ItemsPanel.DefaultItemSize.Width;

            this.dataPager.ItemsPanel.SetHorizontalOffset(offSet,true);
        }

        private void ScrollThePager(int pageIndex)
        {
            var offSet = 0d;
            if ((this.dataPager.AutoEllipsisMode == AutoEllipsisMode.Before ||
                    this.dataPager.AutoEllipsisMode == AutoEllipsisMode.Both) && pageIndex > 0)
                pageIndex = pageIndex - 1;
            this.dataPager.ItemGenerator.VirtualStartIndex = pageIndex;
            this.dataPager.ItemGenerator.VirtualEndIndex = pageIndex +
                                                           (Math.Min(this.dataPager.NumericButtonCount,
                                                                     this.dataPager.PageCount) - 1);
            offSet = this.dataPager.ItemGenerator.VirtualStartIndex * this.dataPager.ItemsPanel.DefaultItemSize.Width;

            this.dataPager.ItemsPanel.SetHorizontalOffset(offSet, true);
        }

        #endregion
    }
}