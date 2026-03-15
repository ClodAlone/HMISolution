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
using System.Windows.Input;
using System.Collections.Generic;
using Syncfusion.RDL.Data;
using Syncfusion.Reports.Server;

namespace Syncfusion.RDL.ServerProcessor
{
    #region Delegates

    internal delegate void GetReportEventHandler(object sender, GetReportEventArgs e);

    internal delegate void GetDataEventHandler(object sender, GetDataEventArgs e);

    internal delegate void GetSharedDataSourceEventHandler(object sender, GetSharedDataSourceEventArgs e);

    internal delegate void GetSharedDataSetEventHandler(object sender, GetSharedDataSetEventArgs e);

    internal delegate void IsValidConnectionEventHandler(object sender, IsValidConnectionEventArgs e);

    internal delegate void ExportedHandler(object sender, ExportedEventArgs e);

    #endregion

    internal class GetReportEventArgs : EventArgs
    {
        public byte[] Result { get; set; }
    }

    internal class ExportedEventArgs : EventArgs
    {
        public byte[] Result { get; set; }
    }

    internal class GetDataEventArgs : EventArgs
    {
        public RecordInfo Result { get; set; }
    }

    internal class GetSharedDataSourceEventArgs : EventArgs
    {
        public ServiceDataSourceDefinition Result { get; set; }
    }

    internal class GetSharedDataSetEventArgs : EventArgs
    {
        public SharedDatasetinfo Result { get; set; }
    }

    internal class IsValidConnectionEventArgs : EventArgs
    {
        public bool Result { get; set; }
    }
}
