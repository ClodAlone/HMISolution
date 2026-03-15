#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Observable collection of tab items.
    /// </summary>
    internal class TabHeaderCollection : ObservableCollection<TabItemAdv>
    {
        #region Private members
        /// <summary>
        /// Parent of visual element.
        /// </summary>
        private TabLayoutPanel mParent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the parent of visual element.
        /// </summary>
        public TabLayoutPanel Parent
        {
            get
            {
                return mParent;
            }

            set
            {
                mParent = value;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Occurs when collection is changed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (mParent != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    int startIndex = e.NewStartingIndex;
                    foreach (TabItemAdv elem in e.NewItems)
                    {
                        if (!mParent.Children.Contains(elem))
                        {
                            mParent.Children.Insert(startIndex, elem);
                            startIndex++;
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (TabItemAdv elem in e.OldItems)
                    {
                        mParent.Children.Remove(elem);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    mParent.Children.Clear();
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates panel's children collection.
        /// </summary>
        internal void UpdatePanelChildren()
        {
            foreach (TabItemAdv item in this.Items)
            {
                if (mParent != null && !mParent.Children.Contains(item))
                {
                    mParent.Children.Add(item);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Observable collection of popup menu items.
    /// </summary>
    public class TabPopupMenuItemCollection : ObservableCollection<TabPopupMenuItem>
    {
        #region Private members
        /// <summary>
        /// Parent of visual element.
        /// </summary>
        private StackPanel m_parent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the parent of visual element.
        /// </summary>
        public StackPanel Parent
        {
            get
            {
                return m_parent;
            }

            set
            {
                m_parent = value;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Occurs when collection is changed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (m_parent != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (TabPopupMenuItem elem in e.NewItems)
                    {
                        if (!m_parent.Children.Contains(elem))
                        {
                            m_parent.Children.Add(elem);
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (TabPopupMenuItem elem in e.OldItems)
                    {
                        m_parent.Children.Remove(elem);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    m_parent.Children.Clear();
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the panel's children collection.
        /// </summary>
        internal void UpdatePanelChildren()
        {
            foreach (TabPopupMenuItem item in this.Items)
            {
                if (!m_parent.Children.Contains(item))
                {
                    m_parent.Children.Add(item);
                }
            }
        }
        #endregion
    }
}
