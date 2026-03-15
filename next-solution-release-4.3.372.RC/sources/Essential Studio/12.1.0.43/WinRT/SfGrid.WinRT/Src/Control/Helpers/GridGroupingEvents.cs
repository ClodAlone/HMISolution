#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using Syncfusion.Data;

namespace Syncfusion.UI.Xaml.Grid
{
    #region Event Args & Handlers

    public delegate void GroupChangingEventHandler(object sender, GroupChangingEventArgs e);

    public class GroupChangingEventArgs: GridCancelEventArgs
    {
        public GroupChangingEventArgs(object originalSource)
            : base(originalSource)
        {
            
        }

        public Group Group
        {
            get;
            internal set;
        }
    }

    public delegate void GroupChangedEventHandler(object sender, GroupChangedEventArgs e);

    public class GroupChangedEventArgs : GridEventArgs
    {
        public GroupChangedEventArgs(object originalSource)
            : base(originalSource)
        {
            
        }

        public Group Group
        {
            get;
            internal set;
        }
    }

    #endregion
}
