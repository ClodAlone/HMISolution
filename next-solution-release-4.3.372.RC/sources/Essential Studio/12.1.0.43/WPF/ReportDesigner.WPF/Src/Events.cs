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

namespace Syncfusion.Windows.Reports.Designer
{
    #region Custom Event Class

    public delegate void ReportItemDrawnEventHanlder(object sender, ReportItemDrawnEventArgs e);

    public class ReportItemDrawnEventArgs : System.EventArgs
    {
        public ReportItemDrawnEventArgs()
        {
        }
    }

    public delegate void AllReportsClosedEventHandler(object sender, AllReportsClosedEventArgs e);

    public class AllReportsClosedEventArgs : System.EventArgs
    {
        public AllReportsClosedEventArgs()
        {
        }
    }

    public delegate void ReportChangedEventHandler(object sender, ReportChangedEventArgs e);

    public class ReportChangedEventArgs : System.EventArgs
    {
        public ReportChangedEventArgs()
        {
        }

        public string ReportName { get; set; }
    }

    #endregion

    #region Internal events
    
    internal delegate void SerializeEventhandler(object sender, SerializeEventArgs e);

    internal class SerializeEventArgs : System.EventArgs
    {
        public SerializeEventArgs()
        {
        }

        public IOrderedEnumerable<RecentReport> recentReports { get; set; }
    }

    internal delegate void UpdateHeaderFooterTabEventEventhandler(object sender, UpdateHeaderFooterTabEventArgs e);

    internal class UpdateHeaderFooterTabEventArgs : System.EventArgs
    {
        public UpdateHeaderFooterTabEventArgs()
        {
        }
    }

    #endregion
}
