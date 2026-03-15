using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using DevExpress.XtraReports.UserDesigner;
using ReportManager.ReportService;
using ReportSettings.Documents;
using DevExpress.Xpf.Reports.UserDesigner;
using ReportManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using System.ComponentModel;

namespace ReportManager.UserDesigner
{
    internal class SaveCommandHandler : DevExpress.XtraReports.UserDesigner.ICommandHandler
    {
        #region Declarations
        readonly XRDesignPanel panel;
        readonly ReportServiceHelper helper;
        readonly Dispatcher dispatcher;
        readonly ReportDesigner reportDesigner;
        readonly ReportManagerComponent editorComponent;
        readonly String subReportName;
        #endregion

        #region Constructors
        public SaveCommandHandler(XRDesignPanel panel, ReportServiceHelper helper) :
            this(panel, helper, null, null, null, null)
        { }

        public SaveCommandHandler(XRDesignPanel panel, ReportServiceHelper helper, ReportManagerComponent editorComponent, String subReportName) :
            this(panel, helper, null, editorComponent, null, subReportName)
        { }

        public SaveCommandHandler(XRDesignPanel panel, ReportServiceHelper helper, Dispatcher dispatcher) : 
            this(panel, helper, dispatcher, null, null, null)
        { }

        public SaveCommandHandler(XRDesignPanel panel, ReportServiceHelper helper, Dispatcher dispatcher, ReportManagerComponent editorComponent)
            : this (panel, helper, dispatcher, editorComponent, null, null)
        { }

        public SaveCommandHandler(XRDesignPanel panel, ReportServiceHelper helper, Dispatcher dispatcher, ReportManagerComponent editorComponent, ReportDesigner reportDesigner, String subReportName)
        {
            this.panel = panel;
            this.helper = helper;
            this.dispatcher = dispatcher;
            this.editorComponent = editorComponent;
            this.reportDesigner = reportDesigner;
            this.subReportName = subReportName;
        }
        #endregion

        #region Implementation
        public void HandleCommand(DevExpress.XtraReports.UserDesigner.ReportCommand command,
            object[] args)
        {
            // Save the report.
            Save(command);

            if (args != null && panel.ReportState == ReportState.Changed)
            {
                for (int ii = 0; ii < args.Length; ii++)
                {
                    if (args[ii] is CancelEventArgs)
                        (args[ii] as CancelEventArgs).Cancel = true;
                }
            }
        }

        public bool CanHandleCommand(DevExpress.XtraReports.UserDesigner.ReportCommand command, ref bool useNextHandler)
        {
            useNextHandler = !(command == ReportCommand.SaveAll || 
                command == ReportCommand.SaveFile ||
                command == ReportCommand.SaveFileAs ||
                command == ReportCommand.Closing);
            return !useNextHandler;
        }

        void Save(DevExpress.XtraReports.UserDesigner.ReportCommand command)
        {
            bool changed = panel.ReportState == ReportState.Changed;
            var action = (Action)(() =>
            {
                if (changed && panel.Report != null)
                {
                    if (reportDesigner == null && command == ReportCommand.Closing)
                    {
                        if (editorComponent != null && editorComponent.UIInterface != null)
                        {
                            var res = editorComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                                helper.ReportDoc.Title), CustomDialogIcons.Question);
                            if (res == CustomDialogResults.No)
                                panel.ReportState = ReportState.None;

                            if (res != CustomDialogResults.Yes)
                                return;
                        }
                    }

                    helper.AddPlaceholderToConnectionString(panel.Report);

                    using (var stream = new MemoryStream())
                    {
                        bool error = false;
                        try
                        {
                            panel.Report.SaveLayout(stream);
                        }
                        catch (Exception ex)
                        {
                            error = true;
                            if (reportDesigner == null && editorComponent != null && editorComponent.UIInterface != null)
                                editorComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorSavingReport, helper.ReportDoc.Title, ex.Message));
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            try
                            {
                                panel.Report.SaveLayoutToXml(memoryStream);
                            }
                            catch (Exception ex)
                            {
                                error = true;
                                Debug.Fail("Invalid report layout XML ('{0}')", ex.Message);
                            }

                            if (!error)
                            {
                                if (subReportName != null)
                                {
                                    helper.CleanSubReportsMap(panel.Report, true);
                                    helper.SaveSubReportDocument(memoryStream.ToArray(), subReportName);
                                }
                                else
                                {
                                    helper.CleanSubReportsMap(panel.Report, false);
                                    helper.SaveReportDocument(stream.ToArray(), memoryStream.ToArray(), reportDesigner == null || command == ReportCommand.SaveAll);
                                }

                            }
                        }
                    }

                    helper.NormalizeConnectionString(panel.Report);
                }

                if (reportDesigner != null)
                {
                    if (command == ReportCommand.Closing)
                    {
                        try
                        {
                            if (editorComponent != null)
                                editorComponent.Workspace.IsBusy = true;
                                reportDesigner.DocumentSource = helper.LoadReportDocument(reportDesigner.ReportSerializer);
                        }
                        finally
                        {
                            if (editorComponent != null)
                                editorComponent.Workspace.IsBusy = false;
                        }
                    }
                }

                // Prevent the "Report has been changed" dialog from being shown.
                panel.ReportState = ReportState.Saved;
            });

            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(DispatcherPriority.Background, action);
                // Prevent the "Report has been changed" dialog from being shown.
                panel.ReportState = ReportState.Saved;
            }
            else
                action.Invoke();
        }
        #endregion
    }
}
