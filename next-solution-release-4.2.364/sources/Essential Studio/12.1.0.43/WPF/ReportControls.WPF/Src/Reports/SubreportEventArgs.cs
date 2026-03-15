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
#if MVC
namespace Syncfusion.Reports.Mvc
#else
namespace Syncfusion.Windows.Reports
#endif
{
    public delegate void SubreportProcessingEventHandler(Object sender, SubreportProcessingEventArgs e);

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

        public string ReportPath
        {
            get;
            set;
        }

    }
}
