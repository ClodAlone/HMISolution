using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.XtraReports.UserDesigner;
using ReportManager.ReportService;
using ReportManager.ComponentService;

namespace ReportManager.UserDesigner
{
    internal class ShowTabCommandHandler : DevExpress.XtraReports.UserDesigner.ICommandHandler
    {
        #region Declarations
        readonly XRDesignPanel panel;
        readonly ReportServiceHelper helper;
        readonly ReportManagerComponent editorComponent;
        #endregion

        #region Constructors
        public ShowTabCommandHandler(XRDesignPanel panel, ReportServiceHelper helper) : 
            this(panel, helper, null)
        { }

        public ShowTabCommandHandler(XRDesignPanel panel, ReportServiceHelper helper, ReportManagerComponent editorComponent)
        {
            this.panel = panel;
            this.helper = helper;
            this.editorComponent = editorComponent;
        }
        #endregion

        #region Implementation
        public void HandleCommand(DevExpress.XtraReports.UserDesigner.ReportCommand command,
            object[] args)
        {
            int tabindex = 0;
            bool onlyschema = false;
            if (command == ReportCommand.ShowDesignerTab)
                onlyschema = true;
            else if (command == ReportCommand.ShowPreviewTab)
                tabindex = 1;
            else if (command == ReportCommand.ShowHTMLViewTab)
                tabindex = 2;

            if (panel.Report != null)
            {
                try
                {
                    if (editorComponent != null)
                        editorComponent.Workspace.IsBusy = true;
                    if (panel.Report.DataSource is DataSet)
                    {
                        var dataset = panel.Report.DataSource as DataSet;
                        dataset.Clear();
                        helper.FillDataSource(ref dataset, onlyschema);
                    }
                }
                catch (Exception ex) 
                { }
                finally
                {
                    if (editorComponent != null)
                        editorComponent.Workspace.IsBusy = false;
                }
            }

            panel.SelectedTabIndex = tabindex;
        }

        public bool CanHandleCommand(DevExpress.XtraReports.UserDesigner.ReportCommand command, ref bool useNextHandler)
        {
            useNextHandler = !(command == ReportCommand.ShowDesignerTab ||
                command == ReportCommand.ShowPreviewTab || 
                command == ReportCommand.ShowHTMLViewTab);
            return !useNextHandler;
        }

        #endregion
    }
}
