// <copyright file="RibbonCommands.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class represents custom ribbon commands.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonCommands
    {
        #region Commands
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines QAT More Commands Ribbon command.
        /// </summary>
        public static RoutedCommand QATMoreCommands = new RoutedCommand();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines Place QAT below Ribbon command.
        /// </summary>
        public static RoutedCommand PlaceQATBelow = new RoutedCommand();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines Place QAT above Ribbon command.
        /// </summary>
        public static RoutedCommand PlaceQATAbove = new RoutedCommand();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines Minimize ribbon command.
        /// </summary>
        public static RoutedCommand MinimizeRibbon = new RoutedCommand();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines AddItem ribbon command.
        /// </summary>
        public static RoutedCommand AddItemToQAT = new RoutedCommand();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines RemoveItem ribbon command.
        /// </summary>
        public static RoutedCommand RemoveItemFromQAT = new RoutedCommand();
        #endregion
    }
}
