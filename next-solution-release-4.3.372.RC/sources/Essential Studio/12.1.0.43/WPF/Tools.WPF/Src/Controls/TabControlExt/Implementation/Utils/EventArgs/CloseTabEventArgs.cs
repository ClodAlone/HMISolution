// <copyright file="CloseTabEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Shared;
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents event args with information about target tab item.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CloseTabEventArgs : EventArgs
    {
        #region Private Properties

        /// <summary>
        /// Local variable which represents cancel flag
        /// </summary>
        bool m_Cancel = false;

        #endregion
        #region Public properies
        /// <summary>
        /// Gets or sets the target tab item.
        /// </summary>
        /// <value>The target tab item.</value>
        public TabItemExt TargetTabItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the closing tab items.
        /// </summary>
        /// <value>The closing tab items.</value>
        public ObservableCollection<object> ClosingTabItems
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets Whether tab is Closable
        /// </summary>
        public bool Cancel
        {
            get
            {
                return m_Cancel;
            }
            set
            {
                m_Cancel = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseTabEventArgs"/> class.
        /// </summary>
        /// <param name="targetItem">The target item.</param>
        public CloseTabEventArgs(TabItemExt targetItem)
        {
            TargetTabItem = targetItem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseTabEventArgs"/> class.
        /// </summary>
        /// <param name="targetItem">The target item.</param>
        /// <param name="parameter">The parameter.</param>
        public CloseTabEventArgs(TabItemExt targetItem,String parameter)
        {
            TargetTabItem = targetItem;

            FrameworkElement parenttab = (targetItem.Parent != null) ? (targetItem.Parent as FrameworkElement) :
                VisualUtils.FindAncestor(targetItem as Visual, typeof(TabControlExt)) as TabControlExt;
            FrameworkElement parentdocument = (targetItem.Parent != null) ? (targetItem.Parent as FrameworkElement) :
                VisualUtils.FindAncestor(targetItem as Visual, typeof(DocumentTabControl)) as DocumentTabControl;

            if (parameter.Equals("CloseAllTabs"))
            {

                if ((parentdocument != null) && (parentdocument is DocumentTabControl))
                {
                    DocumentTabControl tabcontrol = parentdocument as DocumentTabControl;
                    ClosingTabItems = new ObservableCollection<object>();
                    for (int i = 0; i < tabcontrol.Items.Count; i++)
                    {
                        ClosingTabItems.Add(tabcontrol.Items[i]);
                    }
                }
                else if ((parenttab != null) && (parenttab is TabControlExt))
                {
                    TabControlExt tabcontrol = parenttab as TabControlExt;
                    ClosingTabItems = new ObservableCollection<object>();
                    for (int i = 0; i < tabcontrol.Items.Count; i++)
                    {
                        ClosingTabItems.Add(tabcontrol.Items[i]);
                    }
                }
            }
            else if (parameter.Equals("CloseOtherTabs"))
            {
                if ((parentdocument != null) && (parentdocument is DocumentTabControl))
                {
                    DocumentTabControl tabcontrol = parentdocument as DocumentTabControl;
                    ClosingTabItems = new ObservableCollection<object>();
                    for (int i = 0; i < tabcontrol.Items.Count; i++)
                    {
                        if (!targetItem.Equals(tabcontrol.Items[i]))
                        {
                            ClosingTabItems.Add(tabcontrol.Items[i]);
                        }
                    }
                }
                else if ((parenttab != null) && (parenttab is TabControlExt))
                {
                    TabControlExt tabcontrol = parenttab as TabControlExt;
                    ClosingTabItems = new ObservableCollection<object>();
                    for (int i = 0; i < tabcontrol.Items.Count; i++)
                    {
                        if (!targetItem.Equals(tabcontrol.Items[i]))
                        {
                            ClosingTabItems.Add(tabcontrol.Items[i]);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
