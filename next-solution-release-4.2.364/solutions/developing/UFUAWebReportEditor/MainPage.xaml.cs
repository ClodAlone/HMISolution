using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.Xpf.Printing;

namespace UFUAWebReportEditor
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void dXTabControl1_SelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            if (e.NewSelectedItem == previewTab)
            {
                var reportPreviewModel = (ReportPreviewModel)documentPreview.Model;
                reportPreviewModel.ReportName = reportDesigner.SessionId;
                reportPreviewModel.CreateDocument();
            }
        }
    }
}
