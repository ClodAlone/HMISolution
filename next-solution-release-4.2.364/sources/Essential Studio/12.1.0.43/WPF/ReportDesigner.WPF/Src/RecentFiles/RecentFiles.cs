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
using System.Xml.Serialization;
using System.Net;

namespace Syncfusion.Windows.Reports
{
    [XmlRoot("RecentReports")]
    public class RecentReports : List<RecentReport>
    {
    }

    public class RecentReport
    {
        public string ReportPath { get; set; }

        public DateTime LastModified { get; set; }

        public string ReportServer { get; set; }
    }

    internal class ReportInfo
    {
        public string ReportPath { get; set; }

        public string ReportServer { get; set; }

        [XmlIgnore]
        public ICredentials ReportServerCredential { get; set; }
    }
}
