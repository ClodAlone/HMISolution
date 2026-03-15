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
using Syncfusion.RDL.Data;

#if WINRT
namespace Syncfusion.UI.Xaml.Reports
#elif MVC
namespace Syncfusion.Reports.Mvc
#else
namespace Syncfusion.Windows.Reports
#endif
{
    #region Delegates

    public delegate void ReportLoadedEventHandler(object sender, EventArgs e);

    public delegate void SubreportProcessingEventHandler(Object sender, SubreportProcessingEventArgs e);

    internal delegate void ReportExceptionHandler(object sender, ReportExceptionEventArgs e);

    internal delegate void DrillThroughEventHandler(object sender,DrillThroughEventArgs e);

#if WINRT
    public delegate void ViewButtonClickHandler(object sender, ViewButtonClickEventArgs args);

    public delegate void RenderingBeginEventHandler(object sender, EventArgs e);

    public delegate void RefreshEventHandler(object sender, EventArgs e);

    public delegate void ReportErrorEventHandler(object sender, ReportErrorEventArgs e);

    public delegate void RenderingCompletedEventHandler(object sender, EventArgs e);
#endif

    #endregion

#if WINRT
    public class ViewButtonClickEventArgs
        : System.EventArgs
    {
        public bool Cancel
        {
            get;
            set;
        }
    }

    public class ReportErrorEventArgs
    : System.EventArgs
    {
        public string Message
        {
            get;
            set;
        }
    }

#endif

    public class ReportExceptionEventArgs
        : System.EventArgs
    {
        public Exception Exception
        {
            get;
            set;
        }
    }

    public class SubreportProcessingEventArgs : EventArgs
    {
        public IList<string> DataSourceName
        {
            get;
            set;
        }

        public ReportDataSourceCollection DataSources
        {
            get;
            set;
        }

        public ReportParameterInfoCollection Parameters
        {
            get;
            set;
        }

        
        //Get or Set Name of the subreport 
        public string ReportPath
        {
            get;
            set;
        }
    }

    internal class DrillThroughEventArgs
        : System.EventArgs
    {
        internal DrillThroughModel Model 
        {
            get; 
            set;
        }
    }
}
