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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Reports.Designer
{
    /// <summary>
    /// Interaction logic for ReportToolBox.xaml
    /// </summary>
    public partial class ReportToolBox : UserControl
    {
        internal string VisualStyle
        {
            get
            {
                return (string)GetValue(VisualStyleProperty);
            }
            set
            {
                 SetValue(VisualStyleProperty, value);
                 SkinStorage.SetVisualStyle(this, value);
            }
        }

        internal static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(string), typeof(ReportToolBox), new UIPropertyMetadata("Office2007Blue"));

        public ReportDesignView ReportDesignView
        {
            get { return (ReportDesignView)GetValue(ReportDesignViewProperty); }
            set { SetValue(ReportDesignViewProperty, value); }
        }

        public static readonly DependencyProperty ReportDesignViewProperty =
            DependencyProperty.Register("ReportDesignView", typeof(ReportDesignView), typeof(ReportToolBox), new UIPropertyMetadata(null, OnReportDesignViewPropertyChanged));

        internal static void OnReportDesignViewPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportToolBox reportToolbox = dependencyObject as ReportToolBox;

            if (reportToolbox != null)
            {
                if (e.NewValue != e.OldValue)
                {
                    if (e.OldValue != null)
                    {
                        ReportDesignView view = e.OldValue as ReportDesignView;
                        view.RemoveToolbox(reportToolbox);
                    }

                    if (e.NewValue != null)
                    {
                        ReportDesignView view = e.NewValue as ReportDesignView;
                        view.AddToolBox(reportToolbox);
                    }
                }
            }
        }

        internal ReportDesignView DesignView
        {
            get;
            set;
        }

        GroupViewItem selectedGroupViewItem;

        public ReportToolBox()
        {
            InitializeComponent();
            DesignView = new ReportDesignView();  
        }

        void DesignPanel_ReportItemDrawn(object sender, ReportItemDrawnEventArgs e)
        {
            if (selectedGroupViewItem != null)
            {
                selectedGroupViewItem.IsSelected = false;
            }
        }

        internal void UpdateReportDesignerObject(ReportDesignView designview)
        {
            if (designview != null)
            {
                DesignView = designview;
                this.DesignView.ReportItemDrawn += new ReportItemDrawnEventHanlder(DesignPanel_ReportItemDrawn);
            }
        }

        private void ClickViewItem(object sender, RoutedEventArgs e)
        {
            string selectedItem = ((sender as GroupViewItem).Text);
            selectedGroupViewItem = (sender as GroupViewItem);
            switch (selectedItem.Trim())
            {
                case "Line":
                    this.DesignView.DrawingReportItem = Syncfusion.Windows.Reports.Designer.DrawingReportItem.Line;
                    break;
                case "Chart":
                    this.DesignView.DrawingReportItem = Syncfusion.Windows.Reports.Designer.DrawingReportItem.Chart;
                    break;
                case "Gauge":
                    this.DesignView.DrawingReportItem = Syncfusion.Windows.Reports.Designer.DrawingReportItem.Gauge;
                    break;
                case "Image":
                    this.DesignView.DrawingReportItem = Syncfusion.Windows.Reports.Designer.DrawingReportItem.Image;
                    break;
                case "List":
                    this.DesignView.DrawingReportItem = Syncfusion.Windows.Reports.Designer.DrawingReportItem.List;
                    break;
                case "Rectangle":
                    this.DesignView.DrawingReportItem = Syncfusion.Windows.Reports.Designer.DrawingReportItem.Rectangle;
                    break;
                case "Subreport":
                    this.DesignView.DrawingReportItem = Syncfusion.Windows.Reports.Designer.DrawingReportItem.SubReport;
                    break;
                case "Table":
                    this.DesignView.DrawingReportItem = Syncfusion.Windows.Reports.Designer.DrawingReportItem.Tablix;
                    break;
                case "TextBox":
                    this.DesignView.DrawingReportItem = Syncfusion.Windows.Reports.Designer.DrawingReportItem.TextBox;
                    break;
            }
        }   

    }
}
