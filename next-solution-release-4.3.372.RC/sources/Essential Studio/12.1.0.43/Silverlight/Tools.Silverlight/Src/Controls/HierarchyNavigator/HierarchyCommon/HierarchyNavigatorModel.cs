#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Tools.Controls
{
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
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public interface IHierarchyNavigatorModelHost
    {
        /// <summary>
        /// 
        /// </summary>
        HierarchyNavigatorModel Model { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorModel : INotifyPropertyChanged, IDisposable
    {
        private bool showToolTip = false;

        /// <summary>
        /// 
        /// </summary>
        public bool ShowToolTip
        {
            get
            {
                return this.showToolTip;
            }

            internal set
            {
                if (this.showToolTip != value)
                {
                    this.showToolTip = value;
                    this.RaisePropertyChanged("ShowToolTip");
                }
            }
        }

        private bool isEnableHistory = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnableHistory
        {
            get
            {
                return this.isEnableHistory;
            }

            internal set
            {
                if (this.isEnableHistory != value)
                {
                    this.isEnableHistory = value;
                    this.RaisePropertyChanged("IsEnableHistory");
                }
            }
        }

        private bool isEnableEditMode = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnableEditMode
        {
            get
            {
                return this.isEnableEditMode;
            }

            internal set
            {
                if (this.isEnableEditMode != value)
                {
                    this.isEnableEditMode = value;
                    this.RaisePropertyChanged("IsEnableEditMode");
                }
            }
        }

        private Visibility showDropDownButton = Visibility.Visible;
        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowDropDownButton
        {
            get
            {
                return this.showDropDownButton;
            }

            internal set
            {
                if (this.showDropDownButton != value)
                {
                    this.showDropDownButton = value;
                    this.RaisePropertyChanged("ShowDropDownButton");
                }
            }
        }

        private Visibility showRefreshButton = Visibility.Visible;
        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowRefreshButton
        {
            get
            {
                return this.showRefreshButton;
            }

            internal set
            {
                if (this.showRefreshButton != value)
                {
                    this.showRefreshButton = value;
                    this.RaisePropertyChanged("ShowRefreshButton");
                }
            }
        }

        private int maxDrillDownLevel = -1;
        /// <summary>
        /// 
        /// </summary>
        public int MaxDrillDownLevel
        {
            get
            {
                return this.maxDrillDownLevel;
            }

            internal set
            {
                if (this.maxDrillDownLevel != value)
                {
                    this.maxDrillDownLevel = value;
                    this.RaisePropertyChanged("MaxDrillDownLevel");
                }
            }
        }

        private string displayMemberPath = "";
        /// <summary>
        /// 
        /// </summary>
        public string DisplayMemberPath
        {
            get { return displayMemberPath; }
            internal set
            {
                if (this.displayMemberPath != value)
                {
                    this.displayMemberPath = value;
                    this.RaisePropertyChanged("DisplayMemberPath");
                }
            }
        }

        private IEnumerable itemsSource;
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return this.itemsSource;
            }

            set
            {
                if (this.itemsSource != value)
                {
                    this.itemsSource = value;
                    this.RaisePropertyChanged("ItemsSource");
                }
            }
        }

        private ItemsPanelTemplate itemsPanel;
        /// <summary>
        /// 
        /// </summary>
        public ItemsPanelTemplate ItemsPanel
        {
            get
            {
                return this.itemsPanel;
            }

            internal set
            {
                if (this.itemsPanel != value)
                {
                    this.itemsPanel = value;
                    this.RaisePropertyChanged("ItemsPanel");
                }
            }
        }

        private DataTemplate itemTemplate;
        /// <summary>
        /// 
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return this.itemTemplate;
            }

            internal set
            {
                if (this.itemTemplate != value)
                {
                    this.itemTemplate = value;
                    this.RaisePropertyChanged("ItemTemplate");
                }
            }
        }

        private HierarchyNavigatorItemsCollection hierarchyNavigatorItems;
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorItemsCollection HierarchyNavigatorItems
        {
            get
            {
                return this.hierarchyNavigatorItems;
            }

            internal set
            {
                if (this.hierarchyNavigatorItems != value)
                {
                    this.hierarchyNavigatorItems = value;
                    this.RaisePropertyChanged("HierarchyNavigatorItems");
                }
            }
        }

        private HierarchyNavigatorItem selectedHierarchyNavigatorItem = null;
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorItem SelectedHierarchyNavigatorItem
        {
            get
            {
                return this.selectedHierarchyNavigatorItem;
            }

            internal set
            {
                if (this.selectedHierarchyNavigatorItem != value)
                {
                    this.selectedHierarchyNavigatorItem = value;
                    this.RaisePropertyChanged("SelectedHierarchyNavigatorItem");
                }
            }
        }

        private int selectedLevel = 0;
        /// <summary>
        /// 
        /// </summary>
        public int SelectedLevel
        {
            get
            {
                return this.selectedLevel;
            }

            internal set
            {
                if (this.selectedLevel != value)
                {
                    this.selectedLevel = value;
                    this.RaisePropertyChanged("SelectedLevel");
                }
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }

        #endregion

        private List<HierarchyNavigatorItemsCollection> historyNavigated = new List<HierarchyNavigatorItemsCollection>();
        /// <summary>
        /// 
        /// </summary>
        public List<HierarchyNavigatorItemsCollection> HistoryNavigated
        {
            get
            {
                return this.historyNavigated;
            }

            internal set
            {
                if (this.historyNavigated != value)
                {
                    this.historyNavigated = value;
                    this.RaisePropertyChanged("HistoryNavigated");
                }
            }
        }

        internal bool CheckForMaxDrillDownItem()
        {
            if (this.MaxDrillDownLevel < 0) return true;
            else if (this.MaxDrillDownLevel > this.SelectedLevel) return true;
            return false;
        }
    }
}
