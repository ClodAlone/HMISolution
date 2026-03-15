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
using Syncfusion.Windows.Data;
using System.Collections;

namespace Syncfusion.Windows.Controls.Schedule
{
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]


    internal class ScheduleAppoinmentsCollectionViewTableView : DataTableCollectionView
    {
        public ScheduleAppoinmentsCollectionViewTableView(IEnumerable source)
            : base(source)
        {
        }
    }
    #endif
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class ScheduleAppoinmentsCollectionView : QueryableCollectionView
    {
        public ScheduleAppoinmentsCollectionView(IEnumerable source)
            : base(source)
        {
        }
    }
}
