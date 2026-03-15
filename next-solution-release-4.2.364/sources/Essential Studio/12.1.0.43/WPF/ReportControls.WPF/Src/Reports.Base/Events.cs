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

namespace Syncfusion.RDL.Internal
{
    #region Delegates

    internal delegate void DataSourceUpdatedEventHandler(object sender, EventArgs e);

    internal delegate void CredentialCheckCompletedEventHandler(object sender, EventArgs e);

    internal delegate void ConnectionValidatedEventHandler(object sender, ReportingConnectionValidationEventArgs e);

    internal delegate void DataSourceCredentialsUpdatedEventHandler(object sender, EventArgs e);

    internal delegate void ReportItemLoadedHandler(object sender, ReportItemLoadedEventArgs e);

    internal delegate void ReportItemEvaluatedHanlder(object sender, ReportItemEvaluatedEventArgs e);

    internal delegate void ToggleChangedEventHandler(object sender, object e);

    internal delegate void MouseWheelScrollHandler(object sender, object e);

    #endregion

    class ReportItemEvaluatedEventArgs
    {
        public Exception Exception { get; set; }
    }

    class ReportItemLoadedEventArgs
    {
        public Exception Exception { get; set; }
    }

    class ReportingConnectionValidationEventArgs
    {
        public bool Success { get; set; }
        public string DataSourceName { get; set; }
    }

    class ReportingConnectionEventArgs
    {
        public string DataSourceName
        {
            get;
            set;
        }

        public string Promt
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string DataProvider
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }
    }

}
