#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Syncfusion.UI.Xaml.Controls.DataPager
{
    public class PageIndexChangingEventArgs : CancelEventArgs
    {
        public int OldPageIndex { get; internal set; }

        public int NewPageIndex { get; set; }
    }

    public class PageIndexChangedEventArgs : EventArgs
    {
        public int OldPageIndex { get; internal set; }

        public int NewPageIndex { get; internal set; }
    }

    public class OnDemandLoadingEventArgs : EventArgs
    {
        public int StartIndex { get; internal set; }
        public int PageSize { get; internal set; }
    }

    public delegate void PageIndexChangedEventhandler(object sender, PageIndexChangedEventArgs args);

    public delegate void PageIndexChangingEventhandler(object sender, PageIndexChangingEventArgs args);

    public delegate void OnDemandLoadingEventHandler(object sender, OnDemandLoadingEventArgs args);
}