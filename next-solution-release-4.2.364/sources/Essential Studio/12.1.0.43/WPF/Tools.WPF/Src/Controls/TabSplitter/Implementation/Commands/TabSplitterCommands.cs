// <copyright file="TabSplitterCommands.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for TabSplitter Commands
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class TabSplitterCommands
    {
        /// <summary>
        /// Defines TabSplitterItem command.
        /// </summary>
        public static RoutedCommand CloseCurrentTabSplitterItem = new RoutedCommand();
        
        /// <summary>
        /// Defines OpenContextMenu command.
        /// </summary>
        public static RoutedCommand OpenContextMenu = new RoutedCommand();
        
        /// <summary>
        /// Defines CollapseBottomSelectedItem command.
        /// </summary>
        public static RoutedCommand CollapseBottomSelectedItem = new RoutedCommand();
    }
}
