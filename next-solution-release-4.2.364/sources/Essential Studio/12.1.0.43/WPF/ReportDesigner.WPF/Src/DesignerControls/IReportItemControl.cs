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
using Syncfusion.RDL.DOM;
using System.Windows.Media;

namespace Syncfusion.Windows.Reports.Designer.Controls 
{
    internal interface IReportItemControl
    {
        Syncfusion.RDL.DOM.ReportItem ReportItem { get; set; }

        string ItemName { get; set; }

        double ItemTop { get; set; }

        double ItemLeft { get; set; }

        double ItemHeight { get; set; }

        double ItemWidth { get; set; }

        bool IsItemSelected { get; set; }

        bool IsFocusedItem { get; set; }

        bool IsTablixItem { get; set; }

        System.Windows.Controls.Canvas Parent { get; set; }

        DesignPanel Panel { get; set; }

        DrawingReportItem ItemType { get; }

        event ReportItemControlSizeHandler ReportItemSizeChanged;

        event ReportItemSelectedEvent ReportItemSelected;

        void RaiseReportItemSizeChangedEvent();

        void RaiseReportItemSelectedEvent(SelectedItemEventArgs selectedArg);

        ReportItem GetReportItem();

        ImageSource GetImageSource();

        void RestoreReportItem(ReportItem reportItem);

        void UpdateItemSizeProperties();
    }

    internal delegate void ReportItemControlSizeHandler(object sender, EventArgs e);

    internal delegate void ReportItemSelectedEvent(object sender, SelectedItemEventArgs e);

    internal class SelectedItemEventArgs : EventArgs
    {
        public object SelectedItem
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get;
            set;
        }
    }  
}
