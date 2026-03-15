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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorHistoryListBox : ItemsControl
    {
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorHistoryListBox()
        {
            this.DefaultStyleKey = typeof(HierarchyNavigatorHistoryListBox);
        }

        internal ScrollViewer PART_HistoryScrollViewer;

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_HistoryScrollViewer = this.GetTemplateChild("PART_HistoryScrollViewer") as ScrollViewer;
        }
    }
}
