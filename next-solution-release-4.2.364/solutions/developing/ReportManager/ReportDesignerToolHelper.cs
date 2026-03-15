using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using ReportManager.ComponentService;
using ReportManager.ReportService;
using ReportManager.UserDesigner;
using ReportSettings.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportManager
{
    internal class ReportDesignerToolHelper
    {
        #region Declarations
        readonly ReportServiceHelper helper;
        readonly ReportManagerComponent editorComponent;
        #endregion

        #region Constructors
        public ReportDesignerToolHelper(ReportDocument doc, ReportManagerComponent editorComponent)
        {
            this.editorComponent = editorComponent;
            helper = new ReportServiceHelper(doc, editorComponent.UFUAEditor);
        }
        #endregion

        #region Members
        public void OpenReportDesignerTool()
        {
            ReportDesignTool dt = null;
            try
            {
                editorComponent.Workspace.IsBusy = true;

                var report = helper.PrepareXtraReportDocument(onlyschema: true);
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

                    dt.DesignRibbonForm.DesignMdiController.AddCommandHandler(new SaveCommandHandler(panel, helper, editorComponent, subReportName));
                    dt.DesignRibbonForm.DesignMdiController.AddCommandHandler(new ShowTabCommandHandler(panel, helper, editorComponent));

                    //dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.AddNewDataSource, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.NewReport, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
                    //dt.DesignRibbonForm.DesignMdiController.SetCommandVisibility(ReportCommand.NewReportWizard, DevExpress.XtraReports.UserDesigner.CommandVisibility.None);
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
        #endregion
    }
}
