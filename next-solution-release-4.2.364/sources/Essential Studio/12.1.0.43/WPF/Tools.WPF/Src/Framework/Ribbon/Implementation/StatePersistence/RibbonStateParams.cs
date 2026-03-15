#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Represent the state of Whole Ribbon Control
    /// </summary>
    [Serializable]
    public class RibbonStateParams 
    {
        /// <summary>
        /// Represent the stored items in QAT Menu Items.
        /// </summary>
        private ArrayList qatMenuItems = new ArrayList();

        /// <summary>
        /// Represents the stored index in QAT Items.
        /// </summary>
        private ArrayList qatItemsIndex = new ArrayList();

        /// <summary>
        /// Represent the Coordinates of the Window.
        /// </summary>
        private ArrayList windowStates = new ArrayList();


        private ArrayList tabOrderList = new ArrayList();

        private ArrayList ribbonCustomTabCollection = new ArrayList();

        private ArrayList collapsedTabList = new ArrayList(); 

        private ArrayList barOrderList = new ArrayList();
        
        /// <summary>
        /// Gets or sets the QAT menu items.
        /// </summary>
        /// <value>The QAT menu items.</value>
        public ArrayList QATMenuItems
        {
            get 
            {
                return this.qatMenuItems;
            }
            set 
            {
                this.qatMenuItems = value;
            }
        }

        /// <summary>
        /// Gets or Sets the Index of Items in QAT menu.
        /// </summary>
        public ArrayList QATItemsIndex
        {
            get { return qatItemsIndex; }
            set { qatItemsIndex = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is QAT below.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is QAT below; otherwise, <c>false</c>.
        /// </value>
        public bool IsQATBelow { get; set; }


        /// <summary>
        /// Gets or sets the state of the ribbon.
        /// </summary>
        /// <value>The state of the ribbon.</value>
        public RibbonState RibbonState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the window states.
        /// </summary>
        /// <value>The window states.</value>
        public ArrayList WindowStates
        {
            get { return windowStates; }
            set { this.windowStates = value; }
        }

        /// <summary>
        /// Gets or sets the QAT items string.
        /// </summary>
        /// <value>The QAT items string.</value>
        public string QATItemsString
        { 
            get;
            set; 
        }

        public ArrayList QatApplicationItemIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the QAT items initial string.
        /// </summary>
        /// <value>The QAT items initial string.</value>
        public string  QATItemsInitialString
        {
            get;
            set;
        }

        /// <summary>
        /// Get or sets the Tab Order
        /// </summary>
        public ArrayList TabOrderList
        {
            get
            {
                return this.tabOrderList;
            }
            set
            {
                this.tabOrderList = value;
            }
        }
        public ArrayList BarOrderList
        {
            get
            {
                return this.barOrderList;
            }
            set
            {
                this.barOrderList = value;
            }
        }

        public ArrayList RibbonCustomTabCollection
        {
            get
            {
                return this.ribbonCustomTabCollection;
            }
            set
            {
                this.ribbonCustomTabCollection = value;
            }
        }

        public ArrayList CollapsedTabList
        {
            get
            {
                return this.collapsedTabList;
            }
            set
            {
                this.collapsedTabList = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonStateParams"/> class.
        /// </summary>
        public RibbonStateParams()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonStateParams"/> class.
        /// </summary>
        /// <param name="bar">The bar.</param>
        /// <param name="QATBelow">if set to <c>true</c> [QAT below].</param>
        /// <param name="ribbonState">State of the ribbon.</param>
        /// <param name="windowState">State of the window.</param>
        public RibbonStateParams(QuickAccessToolBar bar,ArrayList qatApplicationitemindex,string qatString,string qatItemsInitialString,bool isQATBelow,RibbonState ribbonState,ArrayList windowCoordinates)
        {
            this.QATItemsInitialString = qatItemsInitialString;
            this.QATItemsString = qatString;
            this.QatApplicationItemIndex = qatApplicationitemindex;
            StoreQATMenuItems(bar);
            StoreQATItemIndex(bar);

            //For Ribbon Customization
            if (bar != null && bar.Ribbon != null && bar.Ribbon.ShowCustomizeRibbon)
            {
                StoreRibbonTabOrder(bar);
                StoreRibbonBarOrder(bar);
                CreateCustomTab(bar);
                FindCollapsedTabs(bar);
            }

            this.IsQATBelow = isQATBelow;
            this.RibbonState = ribbonState;
            foreach (var item in windowCoordinates)
                this.WindowStates.Add(item);
        }

        /// <summary>
        /// Stores the QAT menu items.
        /// </summary>
        /// <param name="bar">The bar.</param>
        private void StoreQATMenuItems(QuickAccessToolBar bar)
        {
            if (bar != null && bar.QATMenuItems != null)
                foreach (RibbonButton item in bar.QATMenuItems)
                {
                    MenuItemState ItemState = new MenuItemState();
                    ItemState.SynchronizedItem = RibbonCommandManager.GetSynchronizedItem(item);
                    ItemState.Label = item.Label;
                    ItemState.IsSelected = item.IsSelected;
                    this.QATMenuItems.Add(ItemState);
                }
        }

        /// <summary>
        /// Stores the QAT menu items.
        /// </summary>
        /// <param name="bar">The bar.</param>
        private void StoreQATItemIndex(QuickAccessToolBar bar)
        {
            if (bar == null || bar.Items == null) return;

            foreach (Object o in bar.InternalCommandManager.Items)
            {
                if (!(o is QuickAccessToolBarItem)) continue;

                QuickAccessToolBarItem item = o as QuickAccessToolBarItem;

                QATItemState state = new QATItemState(
                    item.Label,
                    ((System.Collections.Generic.IList<QuickAccessToolBarItem>)bar.InternalCommandManager.Items).IndexOf(item));

                this.QATItemsIndex.Add(state);
            }
        }

        private void StoreRibbonTabOrder(QuickAccessToolBar bar)
        {
            if (bar == null || bar.Ribbon == null)
                return;

            var ribbon = bar.Ribbon;

            foreach (var tab in ribbon.Items)
            {
                if (tab is RibbonTab)
                {
                    string originalHeader = QATCustomizationDialog.GetTabOriginalHeader(tab as RibbonTab);
                    string caption=(tab as RibbonTab).Caption;
                    int index = ribbon.Items.IndexOf(tab);
                    TabItemState tabState = new TabItemState(caption, originalHeader,index);

                    this.TabOrderList.Add(tabState);
                }
            }
        }

        private void StoreRibbonBarOrder(QuickAccessToolBar bar)
        {
            if (bar == null || bar.Ribbon == null)
                return;

            var ribbon = bar.Ribbon;
            int tabIndex = 0;

            foreach (var item in ribbon.Items)
            {
                if (item is RibbonTab)
                {
                    foreach (var ribbonBar in (item as RibbonTab).Items)
                    {
                        if (ribbonBar is RibbonBar)
                        {
                            string originalHeader = QATCustomizationDialog.GetBarOriginalHeader(ribbonBar as RibbonBar);

                            string header = (ribbonBar as RibbonBar).Header;
                            int index = (item as RibbonTab).Items.IndexOf(ribbonBar);
                            BarItemState barState = new BarItemState(header, tabIndex, originalHeader,index);

                            this.BarOrderList.Add(barState);
                        }
                    }
                }
                tabIndex++;
            }
        }

        private void CreateCustomTab(QuickAccessToolBar bar)
        {
            if (bar == null || bar.Ribbon == null)
                return;

            var ribbon = bar.Ribbon;

            foreach (var tab in ribbon.Items)
            {
                if (tab is RibbonTab)
                {
                    RibbonTab ribbnTab = tab as RibbonTab;

                    if (ribbnTab.Tag != null)
                    {
                        bool isCustomTab = ribbnTab.Tag.ToString() == "True" ? true : false;
                        if (isCustomTab)
                        {
                            RibbonCustomTabCollection.Add(BuildCollection(ribbnTab, bar));
                        }
                    }
                }
            }
        }

        private RibbonCustomTab BuildCollection(RibbonTab tab,QuickAccessToolBar bar)
        {
            RibbonCustomTab customTab = new RibbonCustomTab() { Caption = tab.Caption };

            foreach (var ribbonBar in tab.Items)
            {
                RibbonBar customBar = ribbonBar as RibbonBar;
                if (customBar != null)
                {
                    string header = string.Empty;
                    string index = string.Empty;

                    header = (customBar as RibbonBar).Header;

                    RibbonCustomBar newBar = new RibbonCustomBar();
                    newBar.Header = header;

                    foreach (var item in customBar.Items)
                    {
                        string commandLabel = string.Empty;
                        if (item is IRibbonControl)
                            commandLabel = (item as IRibbonControl).Label;

                         index = FindItemIndex(bar, item as IRibbonControl);
                         newBar.ItemIndex.Add(index);
                         newBar.CustomCommandList.Add(commandLabel);
                    }

                    customTab.RibbonBarCollection.Add(newBar);
                }
            }

            return customTab;
        }

        private string FindItemIndex(QuickAccessToolBar bar,IRibbonControl control)
        {
            string itemIndex = string.Empty;

            if (bar == null || bar.Ribbon == null)
                return string.Empty;

            foreach (var item in bar.Ribbon.Items)
            {
                RibbonTab tab = item as RibbonTab;

                if (tab != null)
                {
                    foreach (var subItem in tab.Items)
                    {
                        RibbonBar ribbonBar = subItem as RibbonBar;

                        foreach (var innerItem in ribbonBar.Items)
                        {
                            if (innerItem is ItemsControl)
                            {
                                foreach (var child in (innerItem as ItemsControl).Items)
                                {
                                    IRibbonControl ribbonControl = child as IRibbonControl;
                                    if (ribbonControl != null && ribbonControl.Label == control.Label)
                                    {
                                        int tabIndex = bar.Ribbon.Items.IndexOf(item);
                                        int barIndex = tab.Items.IndexOf(ribbonBar);
                                        int ctrlIndex = (innerItem as ItemsControl).Items.IndexOf(child);
                                        int panelIndex = ribbonBar.Items.IndexOf(innerItem);

                                        itemIndex = tabIndex + "," + barIndex + "," + ctrlIndex + "," + panelIndex;

                                        return itemIndex;
                                    }
                                }
                            }
                            else
                            {
                                IRibbonControl ribbonControl = innerItem as IRibbonControl;
                                if (ribbonControl != null && ribbonControl.Label == control.Label)
                                {
                                    int tabIndex = bar.Ribbon.Items.IndexOf(item);
                                    int barIndex = tab.Items.IndexOf(ribbonBar);
                                    int ctrlIndex = ribbonBar.Items.IndexOf(innerItem);

                                    itemIndex = tabIndex + "," + barIndex + "," + ctrlIndex;
                                    foreach(object element in bar.Ribbon.BackStage.Items)
                                    {
                                        if (element is BackStageCommandButton && (element as BackStageCommandButton).Header !=null && (element as BackStageCommandButton).Header.ToString() == control.Label)
                                        { 
                                            itemIndex = bar.Ribbon.BackStage.Items.IndexOf(element).ToString();
                                            break;
                                        }
                                    }
                                    foreach (object qatitem in bar.Ribbon.QuickAccessToolBar.QATMenuItems)
                                    {
                                        if (qatitem is IRibbonControl && (qatitem as IRibbonControl).Label == control.Label)
                                        {
                                            itemIndex = bar.Ribbon.QuickAccessToolBar.QATMenuItems.IndexOf(qatitem as RibbonButton).ToString()+"q";
                                            break;
                                        }
                                    }
                                    return itemIndex;
                                }
                            }
                        }
                    }
                }
            }
            return itemIndex;
            
        }

        private void FindCollapsedTabs(QuickAccessToolBar bar)
        {
            if (bar == null)
                return;
            if (bar.Ribbon != null)
            {
                 var ribbon = bar.Ribbon;

                 foreach (var tab in ribbon.Items)
                 {
                     if (tab is RibbonTab)
                     {
                         if ((tab as RibbonTab).Visibility == Visibility.Collapsed)
                         {
                             int index = bar.Ribbon.Items.IndexOf(tab);
                             CollapsedTabList.Add(index);
                         }
                     }
                 }
            }
        }
    }

    [Serializable]
    public class RibbonCustomTab
    {
        public RibbonCustomTab()
        {
            RibbonBarCollection = new ArrayList();
        }
        public string Caption { get; set; }
        public ArrayList RibbonBarCollection { get; set; }
    }

    [Serializable]
    public class RibbonCustomBar
    {
        public RibbonCustomBar()
        {
            ItemIndex = new ArrayList();
            CustomCommandList = new ArrayList();
        }
        public string Header { get; set; }
        public ArrayList ItemIndex { get; set; }
        public ArrayList CustomCommandList { get; set; }
    }

    internal class RibbonBarOrder
    {
        internal int TabIndex { get; set; }
        internal RibbonBar RibbonBarItem { get; set; }
    }
}
