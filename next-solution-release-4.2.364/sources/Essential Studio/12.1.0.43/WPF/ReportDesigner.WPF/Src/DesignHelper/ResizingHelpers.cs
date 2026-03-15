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
using System.Windows.Controls;

namespace Syncfusion.Windows.Reports.Designer.Controls
{
    class ReportItemImageControl : System.Windows.Controls.Image
    {
        public IReportItemControl ReportItem { get; set; }

        public string ReportItemName { get; set; }

        public double ItemTop { get; set; }

        public double ItemLeft { get; set; }

        public double ItemWidth { get; set; }

        public double ItemHeight { get; set; }
    }

    class ReportItemLocationInfo
    {
        public IReportItemControl ReportItem { get; set; }

        public Canvas ReportItemArea { get; set; }

        public double ItemLeft { get; set; }

        public double ItemRight { get; set; }

        public double ItemTop { get; set; }
        
        public double ItemBottom { get; set; }

        public bool IsSelected { get; set; }

        public Rect Bounds { get; set; }
    }
}
