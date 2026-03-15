// <copyright file="SplitterPageCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents splitter page collection
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SplitterPagesCollection : ObservableCollection<SplitterPage>
    {
        #region Private members
        /// <summary>
        /// Presents the Selected Item
        /// </summary>
        private SplitterPage m_selectedItem = null;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when [selected item changed].
        /// </summary>
        public event SelectedPageChangedHandler SelectedItemChanged;
        #endregion

        #region Initializer
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitterPagesCollection"/> class.
        /// </summary>
        /// <param name="ownerCollection">The owner collection.</param>
        public SplitterPagesCollection(TabSplitterItem ownerCollection)
        {
            CollectionOwner = ownerCollection;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public SplitterPage SelectedItem
        {
            get
            {
                return m_selectedItem;
            }

            set
            {
                SetSelectedItem(value);
            }
        }
        
        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get
            {
                if (m_selectedItem != null)
                {
                    return IndexOf(m_selectedItem);
                }
                else
                {
                    return -1;
                }
            }

            set
            {
                if (value != SelectedIndex)
                {
                    SetSelectedItem(this[value]);
                }
            }
        }
        
        /// <summary>
        /// Gets the owner collection.
        /// </summary>
        /// <value>The owner collection.</value>
        public TabSplitterItem CollectionOwner
        {
            get;
            private set;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Verifies the index of the Z.
        /// </summary>
        protected internal void VerifyZIndex()
        {
            int count = Count - 1;
            int firstTabIndex;

            for (firstTabIndex = 0; firstTabIndex <= count; firstTabIndex++)
            {
                SplitterPage element = Items[firstTabIndex];

                if (element.Visibility == Visibility.Visible)
                {
                    break;
                }
            }

            Dock tabStripPlacement = SplitterPage.GetTabStripPlacement(Items[0]);

            if (tabStripPlacement == Dock.Top)
            {
                for (int i = 0; i <= count; i++)
                {
                    SplitterPage element = Items[i];

                    if (element != null)
                    {
                        if (i == SelectedIndex)
                        {
                            Panel.SetZIndex(element, 10000);
                        }
                        else
                        {
                            Panel.SetZIndex(element, (count - i));
                        }

                        if (i == firstTabIndex)
                        {
                            Panel.SetZIndex(element, 9999);
                        }
                    }
                }
            }
            else
            {
                for (int i = count; i >= 0; i--)
                {
                    SplitterPage element = Items[i];

                    if (element != null)
                    {
                        if (i == SelectedIndex)
                        {
                            Panel.SetZIndex(element, 10000);
                        }
                        else
                        {
                            Panel.SetZIndex(element, i);
                        }

                        if (i == count && (element.Visibility == Visibility.Visible))
                        {
                            Panel.SetZIndex(element, 9999);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [selected item changed].
        /// </summary>
        /// <param name="newPage">The new page.</param>
        /// <param name="oldPage">The old page.</param>
        protected virtual void OnSelectedItemChanged(SplitterPage newPage, SplitterPage oldPage)
        {
            if (null != SelectedItemChanged)
            {
                SplitterPagesSelectionChangedEventArgs args = new SplitterPagesSelectionChangedEventArgs(oldPage, newPage);
                SelectedItemChanged(args);
            }
        }

        /// <summary>
        /// Sets the selected item.
        /// </summary>
        /// <param name="page">The page of SelectedItem.</param>
        private void SetSelectedItem(SplitterPage page)
        {
            if (Contains(page) || null == page)
            {
                if (m_selectedItem != page)
                {
                    SplitterPage oldPage = m_selectedItem;

                    SetSelected(oldPage, false);
                    SetSelected(page, true);

                    m_selectedItem = page;
                    OnSelectedItemChanged(page, oldPage);
                    VerifyZIndex();
                }
            }
            else
            {
                throw new ArgumentException("There is no such page in collection");
            }
        }

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="page">The page of SetSelected.</param>
        /// <param name="isSelected">if set to true [is selected].</param>
        private static void SetSelected(SplitterPage page, bool isSelected)
        {
            if (null != page)
            {
                page.IsSelected = isSelected;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, SplitterPage item)
        {
            base.InsertItem(index, item);
            item.PageStorage = this;
            SelectedIndex = index;
        }
        
        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            if (index == SelectedIndex)
            {
                if (1 == Count)
                {
                    SelectedItem = null;
                }
                else if (Count - 1 == index)
                {
                    SelectedItem = this[index - 1];
                }
                else
                {
                    SelectedItem = this[index + 1];
                }
            }

            this[index].PageStorage = null;
            base.RemoveItem(index);
        }
        #endregion
    }
}