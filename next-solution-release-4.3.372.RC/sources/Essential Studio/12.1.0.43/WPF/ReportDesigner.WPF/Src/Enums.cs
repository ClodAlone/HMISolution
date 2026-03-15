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
    #region Control Enums

    public enum DrawingReportItem
    {
        None,
        TextBox,
        Line,
        Rectangle,
        Image,
        Chart,
        DataBar,
        Sparkline,
        Gauge,
        Map,
        Tablix,
        SubReport,
        List
    }

    public enum ReportItemWizard
    {
        Chart,
        Table
    }

    public enum SaveFileType
    {
        Save,
        SaveAs
    }

    public enum ToolBarTypes
    {
        HandyToolBar,
        RibbonToolBar,
        None
    }

    internal enum DatabaseAccessTypes
    {
        None,
        Read,
        Full
    }

    public enum DesignMode
    {
        RDL,
        RDLC
    }

    public enum ReportFormat
    {
        RDL2008,
        RDL2010
    }

    #endregion
}
