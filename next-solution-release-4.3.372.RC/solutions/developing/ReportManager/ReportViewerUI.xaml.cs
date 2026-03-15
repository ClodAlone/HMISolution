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
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Printing;
using ReportSettings.Documents;
using Utilities;
using Utilities.WPF;
using System.Threading.Tasks;
using DevExpress.XtraReports;
using DocumentManager.ComponentService;
using ReportManager.ComponentService;
using ReportManager.ReportService;
using System.Diagnostics;

namespace ReportManager
{
    /// <summary>
    /// Interaction logic for ReportViewerUI.xaml
    /// </summary>
    public partial class ReportViewerUI : UserControl, IDisposable
    {
        #region Declarations

        readonly ReportManagerComponent Manager;
        readonly ReportDocument Document;
        readonly IDocument DocumentParent;
        readonly UserControl ActiveView;
        readonly IList<ReportParameters.Parameter> Parameters;
        readonly ReportServiceHelper ServiceHelper;
        readonly string Title;

        #endregion

        #region Constructors

        public ReportViewerUI(ReportManagerComponent manager, ReportDocument doc, IDocument parent, UserControl activeView, IList<ReportParameters.Parameter> parameters = null, string title = null, ReportServiceHelper serviceHelper = null)
        {
            InitializeComponent();

            Manager = manager;
            Document = doc;
            DocumentParent = parent;
            ActiveView = activeView;
            Parameters = parameters;
            Title = title;
            ServiceHelper = serviceHelper;

            SetBusy(true);

            this.Loaded += ReportViewer_Loaded;
        }

        #endregion

        private bool bLoaded;
        private void ReportViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (bLoaded)
                return;

            bLoaded = true;

            Document.ActiveView = ActiveView ?? this;
            Document.Parent = DocumentParent;

            var wnd = this.FindParent<Window>();
            wnd.WindowStyle = Document.WindowStyle;
            wnd.ShowActivated = true;
            wnd.WindowState = Document.WindowState;
            /*
            if (wnd.WindowState != System.Windows.WindowState.Maximized)
                wnd.SizeToContent = SizeToContent.WidthAndHeight;
            */
            if (!Double.IsNaN(Document.Width) && Document.Width > 0)
                wnd.Width = Document.Width;
            if (!Double.IsNaN(Document.Height) && Document.Height > 0)
                wnd.Height = Document.Height;
            wnd.ResizeMode = Document.ResizeMode;
            wnd.ShowInTaskbar = Document.ShowInTaskbar;
            wnd.WindowStartupLocation = Document.WindowStartupLocation;
            if (wnd.WindowStartupLocation == WindowStartupLocation.Manual)
            {
                wnd.Top = Document.Top;
                wnd.Left = Document.Left;
            }
            else if (wnd.WindowStartupLocation == WindowStartupLocation.CenterScreen)
            {
                wnd.WindowStartupLocation = WindowStartupLocation.Manual;
                var workingArea = System.Windows.SystemParameters.WorkArea;
                wnd.Top = (workingArea.Height - wnd.ActualHeight) / 2;
                wnd.Left = (workingArea.Width - wnd.ActualWidth) / 2;
            }
            else if (wnd.WindowStartupLocation == WindowStartupLocation.CenterOwner)
            {
                if (wnd.Owner != null)
                {
                    wnd.WindowStartupLocation = WindowStartupLocation.Manual;
                    wnd.Top = ((wnd.Owner.ActualHeight - wnd.ActualHeight) / 2) + wnd.Owner.Top;
                    wnd.Left = ((wnd.Owner.ActualWidth - wnd.ActualWidth) / 2) + wnd.Owner.Left;
                }
                else
                {
                    wnd.WindowStartupLocation = WindowStartupLocation.Manual;
                    var workingArea = System.Windows.SystemParameters.WorkArea;
                    wnd.Top = (workingArea.Height - wnd.ActualHeight) / 2;
                    wnd.Left = (workingArea.Width - wnd.ActualWidth) / 2;
                }
            }
            wnd.Title = !string.IsNullOrEmpty(Title) ? Title : Document.Title;

            WPFUtilities.ThemeHelper.SetTheme(wnd);
            var task = Task.Factory.StartNew(() =>
            {
                var helper = ServiceHelper;
                if (helper == null)
                    helper = new ReportServiceHelper(Document, Manager.UFUAEditor, throwOnException: true);
                var report = helper.PrepareXtraReportDocument(Parameters);
                report.CreateDocument();
                return report;
            });

            task.ContinueWith((ret) =>
            {
                if (ret.Exception == null)
                {
                    try
                    {
                        preview.DocumentSource = ret.Result;
                   }
                    catch (Exception ex)
                    {
                        txtUriError.Text = String.Format(Properties.Resources.ErrorLoadingReport,
                                        Document.Title, ex.Message);
                        SetError(true);
                    }
                    finally
                    {
                        SetBusy(false);
                    }
                }
                else
                {
                    txtUriError.Text = String.Format(Properties.Resources.ErrorLoadingReport,
                                        Document.Title, ret.Exception.InnerException.Message);
                    SetError(true);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void SetBusy(bool bSet)
        {
            if (bSet)
                uriError.Visibility = Visibility.Collapsed;

            preview.Visibility = bSet ? Visibility.Collapsed : Visibility.Visible;
            uriLoading.Visibility = bSet ? Visibility.Visible : Visibility.Collapsed;
            uriLoading.Refresh();
        }

        void SetError(bool bSet)
        {
            if (bSet)
                uriLoading.Visibility = Visibility.Collapsed;

            preview.Visibility = bSet ? Visibility.Collapsed : Visibility.Visible;
            uriError.Visibility = bSet ? Visibility.Visible : Visibility.Collapsed;
            uriError.Refresh();
        }

        #region IDisposable Members

        bool bDisposed = false;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
        }

        #endregion
       
    }
}
