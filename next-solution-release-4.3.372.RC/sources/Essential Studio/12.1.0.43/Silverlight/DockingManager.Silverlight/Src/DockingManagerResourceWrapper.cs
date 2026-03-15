#region Copyright Syncfusion Inc. 2001 - 2014
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
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerResourceWrapper
    {
        const string CloseValue = "CloseToolTip";
        const string MenuValue = "MenuToolTip";
        const string HideValue = "HideContextMenu";
        const string FloatingValue = "FloatingContextMenu";
        const string DockableValue = "DockableContextMenu";
        const string AutoHideToolTipValue = "AutoHideToolTip";
        const string MaximizeValue = "MaximizeToolTip";
        const string RestoreValue = "RestoreToolTip";
        const string AutoHideContextMenuValue = "AutoHideContextMenu";

        /// <summary>
        /// 
        /// </summary>
        ~DockingManagerResourceWrapper()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public DockingManagerResourceWrapper()
        {
            CultureInfo cultureInfo = CultureInfo.CurrentUICulture;

            bool flag = false;
            if (PreviousCulture != cultureInfo)
                flag = true;

            closeToolTip = SR.GetString(cultureInfo, CloseValue, flag);
            menuToolTip = SR.GetString(cultureInfo, MenuValue, false);
            hideContextMenu = SR.GetString(cultureInfo, HideValue, false);
            floatingContextMenu = SR.GetString(cultureInfo, FloatingValue, false);
            dockableContextMenu = SR.GetString(cultureInfo, DockableValue, false);
            autohideToolTip = SR.GetString(cultureInfo, AutoHideToolTipValue, false);
            maximizeToolTip = SR.GetString(cultureInfo, MaximizeValue, false);
            restoreToolTip = SR.GetString(cultureInfo, RestoreValue, false);
            autohideContextMenu = SR.GetString(cultureInfo, AutoHideContextMenuValue, false);

            if (flag)
                ChangeStrings();
        }

        DockingManager docking = null;

        internal DockingManager DockingManager
        {
            get
            {
                return docking;
            }
            set
            {
                docking = value;
                if (docking != null && PreviousCulture != CultureInfo.CurrentUICulture)
                {
                    ChangeStrings();
                }
            }
        }

        private void ChangeStrings()
        {
            if (DockingManager != null)
            {
                foreach (Window w in DockingManager.WindowCollection.Values)
                {
                    if (w.closeButton != null)
                        ToolTipService.SetToolTip(w.closeButton, CloseToolTip);
                    if (w.dockToggle != null)
                        ToolTipService.SetToolTip(w.dockToggle, AutoHideToolTip);
                    if (w.optionsButton != null)
                        ToolTipService.SetToolTip(w.optionsButton, MenuToolTip);
                    if (w.maximizeButton != null)
                    {
                        if (w.MaximizedState == MaximizedState.Maximized)
                            ToolTipService.SetToolTip(w.maximizeButton, RestoreToolTip);
                        else
                            ToolTipService.SetToolTip(w.maximizeButton, MaximizeToolTip);
                    }
                    if (w.floatContextMenuItemAdv != null)
                        w.floatContextMenuItemAdv.Header = FloatingContextMenu;
                    if (w.hideContextMenuItemAdv != null)
                        w.hideContextMenuItemAdv.Header = HideContextMenu;
                    if (w.autoHideContextMenuItemAdv != null)
                        w.autoHideContextMenuItemAdv.Header = AutoHideContextMenu;
                    if (w.dockContextMenuItemAdv != null)
                        w.dockContextMenuItemAdv.Header = DockableContextMenu;
                }
            }
        }

        private CultureInfo PreviousCulture
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CloseToolTip
        {
            get { return closeToolTip; }
            set { closeToolTip = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MenuToolTip
        {
            get { return menuToolTip; }
            set { menuToolTip = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string HideContextMenu
        {
            get { return hideContextMenu; }
            set { hideContextMenu = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string FloatingContextMenu
        {
            get { return floatingContextMenu; }
            set { floatingContextMenu = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DockableContextMenu
        {
            get { return dockableContextMenu; }
            set { dockableContextMenu = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AutoHideToolTip
        {
            get { return autohideToolTip; }
            set { autohideToolTip = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MaximizeToolTip
        {
            get { return maximizeToolTip; }
            set { maximizeToolTip = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RestoreToolTip
        {
            get { return restoreToolTip; }
            set { restoreToolTip = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AutoHideContextMenu
        {
            get { return autohideContextMenu; }
            set { autohideContextMenu = value; }
        }

        string closeToolTip;
        string menuToolTip;
        string floatingContextMenu;
        string dockableContextMenu;
        string autohideToolTip;
        string restoreToolTip;
        string maximizeToolTip;
        string hideContextMenu;
        string autohideContextMenu;
    }
}
