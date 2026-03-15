// <copyright file="TabControlCommands.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent the TabControl Commands class
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class TabControlCommands
    {
        #region Commands
        /// <summary>
        /// Defines Close TabItemExt command.
        /// </summary>
        public static RoutedCommand CloseTabItem = new RoutedCommand();

        /// <summary>
        /// Defines CloseCurrentTabItem command.
        /// </summary>
        public static RoutedCommand CloseCurrentTabItem = new RoutedCommand();

        /// <summary>
        /// Defines CloseOtherTabs command.
        /// </summary>
        public static RoutedCommand CloseTabs = new RoutedCommand();

        /// <summary>
        /// Defines OpenContextMenu command.
        /// </summary>
        public static RoutedCommand OpenContextMenu = new RoutedCommand();
        #endregion
    }
}
