using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using ReportSettings.Documents;
using ReportManager.ComponentService;
using ReportManager.ReportService;
using DataReader.Extensions;
using System.Data;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UI;
using ReportManager.UserDesigner;
using System.Windows.Media;
using Utilities.WPF;
using DevExpress.Xpf.Docking;
using System.IO;
using System.Text;
using System.ComponentModel.Design;

namespace ReportManager
{
    /// <summary>
    /// Interaction logic for DocumentEditorControl.xaml
    /// </summary>
    public partial class ReportEditorUI : UserControl, IDisposable
    {
        #region Declarations
        internal readonly ReportManagerComponent editorComponent;
        internal readonly ReportServiceHelper helper;
        readonly UIReportEditorCommands reportEditorCommands;
        #endregion

        bool bLoaded;
        public bool HasBeenLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        public ReportEditorUI(ReportManagerComponent editorComponent, ReportDocument doc)
        {
            this.editorComponent = editorComponent;
            Document = doc;

            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                var control = (from c in reportDesigner.GetVisualChildrenOfType<Grid>()
                               where c.Name == "PART_HeaderContainer"
                               select c).FirstOrDefault();
                if (control != null)
                    control.Visibility = Visibility.Collapsed;

                DockLayoutManager manager = DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByType(reportDesigner, typeof(DockLayoutManager)) as DockLayoutManager;
                if (manager != null)
                {
                    using (var reader = new MemoryStream(Encoding.Default.GetBytes(Properties.Resources.ReportDesignerLayout)))
                    {
                        try
                        {
                            manager.RestoreLayoutFromStream(reader);
                        }
                        catch { }
                    }
                }

                ChangeDataSourceCommit();
            };

            helper = new ReportServiceHelper(doc, this.editorComponent.UFUAEditor);

            reportDesigner.DocumentOpenFailed += ReportDesigner_DocumentLoadFailed;
            reportDesigner.DocumentClosing += ReportDesigner_DocumentClosing;
            reportDesigner.DocumentSource = helper.LoadReportDocument(reportDesigner.ReportSerializer);

            reportEditorCommands = new UIReportEditorCommands(this);

            Document.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "ReaderItemSources")
                {
                    var model = new DataReader.DataReaderModel(Document.ReaderItemSources);
                    model.NormalizeConnectionString(doc?.rootBase);
                    helper.ReportDocModel = model;
                    ChangeDataSourceCommit();
                }
            };
        }

        private void ReportDesigner_DocumentClosing(object sender, DevExpress.Xpf.Reports.UserDesigner.ReportDesignerDocumentClosingEventArgs e)
        {
            if (reportDesigner.ActiveDocument != null)
                reportDesigner.ActiveDocument.Save();
        }

        void OpenReportDesignerTool()
        {
            ReportDesignTool dt = null;
            try
            {
                editorComponent.Workspace.IsBusy = true;

                reportDesigner.ActiveDocument.Save();
                helper.SaveReportDocument(reportDesigner.ReportSerializer, reportDesigner.ActiveDocument.Report);
                var report = helper.PrepareXtraReportDocument(reportDesigner.ReportSerializer, reportDesigner.ActiveDocument.Report, true);
                dt = new ReportDesignTool(report);
                dt.DesignRibbonForm.DesignMdiController.DesignPanelLoaded += (s, ev) =>
                {
                    XRDesignPanel panel = s as XRDesignPanel;
                    helper.LoadSubReports(panel.Report, onlyschema: true);

                    String subReportName = null;
                    if (panel.Report != report)
                    {
                        var selectionService = dt.DesignRibbonForm.DesignMdiController.ActiveDesignPanel.GetService<ISelectionService>();
                        if (selectionService == null || !(selectionService.PrimarySelection is XRSubreport))
                            return;
                        var subReport = selectionService.PrimarySelection as XRSubreport;
                        subReport.ReportSource = panel.Report;
                        subReportName = helper.GetUniqueControlKey(subReport, subReport.Name);
                    }
                    else
                    {
                        var form = panel.FindForm();
                        if (form != null)
                        {
                            form.FormClosing += (s2, ev2) => { ev2.Cancel = true; };
                        }
                    }

                    dt.DesignRibbonForm.DesignMdiController.AddCommandHandler(new SaveCommandHandler(panel, helper, Dispatcher, editorComponent, reportDesigner, subReportName));
                    dt.DesignRibbonForm.DesignMdiController.AddCommandHandler(new ShowTabCommandHandler(panel, helper, editorComponent));

                    //dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.AddNewDataSource, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.NewReport, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.NewReportWizard, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.OpenFile, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    //dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.SaveAll, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.SaveFile, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.SaveFileAs, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    /*
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.ShowviewDesignerInfo, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.ShowHTMLViewTab, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.ShowPreviewTab, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.ShowTabbedInterface, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.ShowWindowInterface, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    */
                    dt.DesignRibbonForm.DesignMdiController.SqlWizardSettings.EnableCustomSql = true;
                    dt.DesignRibbonForm.DesignMdiController.SqlWizardSettings.DatabaseCredentialsSavingBehavior = DevExpress.DataAccess.Wizard.SensitiveInfoSavingBehavior.Prompt;
                    editorComponent.Workspace.IsBusy = false;

                    if (!String.IsNullOrEmpty(helper.LastErrorInfo) && editorComponent.UIInterface != null)
                        editorComponent.UIInterface.ShowError(helper.LastErrorInfo);
                };

                //dt.DesignRibbonForm.DesignMdiController.AnyDocumentActivated += (s, ev) =>
                //{
                //    var form = dt.DesignRibbonForm.DesignMdiController.ActiveDesignPanel.FindForm();
                //    if (form != null)
                //    {
                //        form.FormClosing += (s2, ev2) => { ev2.Cancel = true; };
                //    }
                //};

                dt.ShowRibbonDesignerDialog();
            }
            finally
            {
                editorComponent.Workspace.IsBusy = false;
            }
        }

        void ReportDesigner_DocumentLoadFailed(object sender, DevExpress.Xpf.Reports.UserDesigner.ReportDesignerDocumentLoadFailedEventArgs e)
        {
            reportDesigner.Visibility = System.Windows.Visibility.Hidden;
            viewDesignerInfo.Visibility = System.Windows.Visibility.Visible;
            txtDesignerInfo.Text = String.Format(Properties.Resources.ReportWpfStartDesignError, e.ErrorMessage).Replace("'newline'", Environment.NewLine);
        }

        void ChangeDataSourceCommit()
        {
            if (bDisposed || 
                reportDesigner.ActiveDocument == null || 
                reportDesigner.ActiveDocument.Report == null)
                return;

            if (helper.ReportDocModel != null && helper.ReportDocModel.IsValid())
            {
                try
                {
                    var dataSet = new DataSet(helper.ReportDocModel.DataSourceName);
                    helper.FillDataSource(ref dataSet, true);
                    if (!helper.InvalidDataSource)
                    {
                        reportDesigner.ActiveDocument.Report.DataSource = dataSet;
                        reportDesigner.ActiveDocument.Load(reportDesigner.ActiveDocument.Report);
                    }
                    else
                    {
                        editorComponent.UIInterface.ShowError(String.Format(Properties.Resources.ReportDataSetInitError, helper.LastErrorInfo));
                    }
                }
                catch (Exception ex)
                {
                    editorComponent.UIInterface.ShowError(String.Format(Properties.Resources.ReportDataSetInitError, ex.Message));
                }
            }
        }

        private void dXTabControl1_SelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            if (bDisposed || reportDesigner.ActiveDocument == null)
                return;

            if (e.NewSelectedItem == previewTab)
            {
                editorComponent.Workspace.IsBusy = true;
                try
                {
                    reportDesigner.ActiveDocument.Save();
                    var report = helper.PrepareXtraReportDocument(reportDesigner.ReportSerializer, reportDesigner.ActiveDocument.Report);
                    reportPreview.Model = new XtraReportPreviewModel(report);

                    txtErrorInfo.Text = helper.LastErrorInfo;
                    if (!String.IsNullOrEmpty(txtErrorInfo.Text))
                    {
                        viewErrorInfo.Visibility = System.Windows.Visibility.Visible;
                        reportPreview.Opacity = 0.1;
                    }
                    else
                    {
                        viewErrorInfo.Visibility = System.Windows.Visibility.Collapsed;
                        reportPreview.Opacity = 1.0;
                        try
                        {
                            report.CreateDocument();
                        }
                        catch (Exception ex)
                        {
                            txtErrorInfo.Text = ex.Message;
                            viewErrorInfo.Visibility = System.Windows.Visibility.Visible;
                            reportPreview.Opacity = 0.1;
                        }
                    }
                }
                finally
                {
                    editorComponent.Workspace.IsBusy = false;
                }
            }
        }

        #region Methods

        internal void SaveReportLayout()
        {
            if (bDisposed || reportDesigner.ActiveDocument == null)
                return;

            reportDesigner.ActiveDocument.Save();
            helper.SaveReportDocument(reportDesigner.ReportSerializer, reportDesigner.ActiveDocument.Report);
        }

        #endregion

        #region Properties

        ReportDocument document;
        [Browsable(false)]
        public ReportDocument Document
        {
            get
            {
                return document;
            }
            private set
            {
                document = value;
            }
        }
        #endregion

        #region Commands

        public UIReportEditorCommands ReportEditorCommands
        {
            get { return reportEditorCommands; }
        }

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            SaveReportLayout();
            Document.SaveToFile();
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            bool hasChanges = reportDesigner.ActiveDocument != null && reportDesigner.ActiveDocument.HasChanges;
            e.CanExecute = Document != null && (Document.NeedsSave || hasChanges);
        }

        private void OnCommandDelete(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (bDisposed || ReportEditorCommands.DiagramCommands == null)
                return;

            ReportEditorCommands.DiagramCommands.Delete.Execute(null);
        }

        private void CanCommandDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ReportEditorCommands.DiagramCommands != null && ReportEditorCommands.DiagramCommands.Delete.CanExecute(null);
        }

        internal void OnFontNameSelectionChanged(FontFamily fontFamily)
        {
            if (bDisposed || fontFamily == null || ReportEditorCommands.SelectedItems == null)
                return;

            for (int ii = 0; ii < ReportEditorCommands.SelectedItems.Count; ii++)
                ReportEditorCommands.SelectedItems[ii].FontFamily = fontFamily;
            ReportEditorCommands.ForceUpdateFontProperties();
        }

        internal void OnFontSizeSelectionChanged(int fontSize)
        {
            if (bDisposed || ReportEditorCommands.SelectedItems == null)
                return;

            for (int ii = 0; ii < ReportEditorCommands.SelectedItems.Count; ii++)
                ReportEditorCommands.SelectedItems[ii].FontSize = fontSize;
            ReportEditorCommands.ForceUpdateFontProperties();
        }

        internal void OnBorderWidthSelectionChanged(int width)
        {
            if (bDisposed || ReportEditorCommands.SelectedItems == null)
                return;

            for (int ii = 0; ii < ReportEditorCommands.SelectedItems.Count; ii++)
                ReportEditorCommands.SelectedItems[ii].BorderThickness = new Thickness(width);
            ReportEditorCommands.ForceUpdateBorderProperties();
        }

    private void ReportDisignerTool_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            OpenReportDesignerTool();
        }

        private void OnOpenDesignForm(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (bDisposed)
                return;

            OpenReportDesignerTool();
        }

        private void CanOpenDesignForm(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region IDisposable Members

        bool bDisposed = false;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            reportDesigner.DocumentOpenFailed -= ReportDesigner_DocumentLoadFailed;
            reportDesigner.DocumentClosing -= ReportDesigner_DocumentClosing;

            if (reportDesigner.ActiveDocument != null)
                reportDesigner.ActiveDocument.Save();
        }

        #endregion

    }
}
