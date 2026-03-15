#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
#if WinRT
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public class QueryColumnDraggingEventArgs : GridCancelEventArgs
    {
        public QueryColumnDraggingEventArgs(SfDataGrid dataGrid) : base(dataGrid)
        {
            
        }

        public int From { get; internal set; }
        public int To { get; internal set; }
        public Point PopupPosition { get; internal set; }
        public QueryColumnDraggingReason Reason { get; internal set; }
    }

    public enum QueryColumnDraggingReason
    {
        DragStarting,
        DragStarted,
        Dragging,
        Dropping,
        Dropped
    }


    public delegate void QueryColumnDraggingEventHandler(object sender, QueryColumnDraggingEventArgs e);

}
