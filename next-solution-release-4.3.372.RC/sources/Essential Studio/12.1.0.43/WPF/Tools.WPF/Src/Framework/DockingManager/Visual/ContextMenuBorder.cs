// <copyright file="ContextMenuBorder.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class allows to process events of its context menu.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ContextMenuBorder : Border
    {
        #region Constants
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant contains the name of one of the items.
        /// </summary>
        private const string DockableMenuItemName = "PART_DockableMenuItem";
        
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant contains the name of one of the items.
        /// </summary>
        private const string HideMenuItemName = "PART_HideMenuItem";
        
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant contains the name of one of the items.
        /// </summary>
        private const string AutoHideMenuItemName = "PART_AutoHideMenuItem";
        
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant contains the name of one of the items.
        /// </summary>
        private const string FloatingMenuItemName = "PART_FloatingMenuItem";
        
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant contains the name of one of the items.
        /// </summary>
        private const string TabbedMenuItemName = "PART_TabbedMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string MaximizeMenuItemName = "PART_MaximizeMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string MinimizeMenuItemName = "PART_MinimizeMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string RestoreMenuItemName = "PART_RetoreMenuItem";

        #endregion

        #region Implementation
      
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Raises the Initialized event. This method is invoked
        /// whenever IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event
        /// data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (null != ContextMenu)
            {
                foreach (MenuItem item in ContextMenu.Items)
                {
                    if (DockableMenuItemName == item.Name)
                    {
                        item.IsChecked = true;
                    }

                    item.Click -= new RoutedEventHandler(OnMenuItemClick);
                    item.Click += new RoutedEventHandler(OnMenuItemClick);
                }
            }
            else
            {
                Debug.Print("It is incorrect use control.");
            }
        }
        
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when the user clicks on Context menu
        /// item.
        /// </summary>
        /// <param name="sender">Item element which was clicked.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            FrameworkElement element = (FrameworkElement)DataContext;
            DockingManager owner = DockingManager.ResolveManager(element);

            owner.LockLayoutUpdate = true;
            owner.LockPropertyChangedAction = true;

            switch (item.Name)
            {
                case HideMenuItemName:
                    owner.ExecuteClose(element);
                    break;

                case TabbedMenuItemName:
                    owner.ExecuteDocument(element);
                    break;

                case AutoHideMenuItemName:
                    owner.ExecuteAutoHide(element);
                    break;

                case FloatingMenuItemName:

                    owner.ExtractElementToWindow(element, ActionMode.Active, false);
                    DockingManager.SetNoDock(element, true);
                    DockingManager.SetNewFocusedElement(element);

                    break;
            }

            owner.LockPropertyChangedAction = false;
        }
        #endregion
    }
}
