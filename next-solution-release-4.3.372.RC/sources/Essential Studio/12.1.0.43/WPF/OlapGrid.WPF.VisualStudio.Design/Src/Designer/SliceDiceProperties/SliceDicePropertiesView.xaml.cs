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
using Microsoft.Windows.Design.Model;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Reports;
using Syncfusion.Windows.Shared.Olap;

namespace Syncfusion.OlapGrid.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class SliceDicePropertiesView 
        : UserControl,
        IWizardNavigationButtonsStatus
    {
        #region Members

        private string connectionString;
        private string currentCubeName;
        private ModelItem selectedControl;
        private OlapReport olapReport;
        private Report report;
        private WizardWindow wizardWindowInstance;
        private bool isExisting = false;

        #endregion

        #region Constructor and initilizer

        public SliceDicePropertiesView(ModelItem selectedControl, WizardWindow winzardWindow)
        {
            InitializeComponent();

            this.selectedControl = selectedControl;
            this.wizardWindowInstance = winzardWindow;

            this._CanBackButtonEnabled = false;
            this._CanNextButtonEnabled = false;
            this._CanFinishButtonEnabled = false;            

            this.report = new Report();

            this.axisBuilderColumn.Drop += new DragEventHandler(axisBuilderColumn_Drop);
            this.axisBuilderRow.Drop += new DragEventHandler(axisBuilderRow_Drop);
            this.axisBuilderSlicer.Drop += new DragEventHandler(axisBuilderSlicer_Drop);
        }

        public void InitializeControls(string conn)
        {
            this.connectionString = conn;

            var designerSettings = selectedControl.Properties["DesignerSettings"].ComputedValue;

            if (designerSettings != null)
            {
                string conStr = selectedControl.Properties["DesignerSettings"].Value.Properties["ConnectionString"].ComputedValue.ToString();

                if (!string.IsNullOrEmpty(conStr))
                {
                    this.isExisting = true;
                    DesignerSettings desSetting = designerSettings as DesignerSettings;
                    if (desSetting.IsValidConnectionString)
                    {
                        this.MetaTreeOlapDataManager = new OlapDataManager(desSetting.ConnectionString);
                        this.olapReport = desSetting.GetOlapReport();
                        this.UpdateDataSource();
                    }
                    //this.MetaTreeOlapDataManager = new OlapDataManager(desSetting.ConnectionString);
                    //this.olapReport = desSetting.GetOlapReport();
                    //this.currentCubeName = desSetting.CurrentCubeName;
                    //this.UpdateDataSource();
                }
                else
                {
                    throw new ArgumentException("Connection String is empty in designer settings.");
                }
            }
            else
            {
                //// This is being created newly. So, get the connection string information passed from the connection properties view.
                if (!string.IsNullOrEmpty(this.connectionString))
                {
                    this.isExisting = false;
                    this.UpdateButtonStatus(true, false, false);
                    this.MetaTreeOlapDataManager = new OlapDataManager(this.connectionString);
                    this.NewDataSource();
                }
                else
                {
                    MessageBox.Show("Connection string is empty");
                    //throw new ArgumentException("Connection string is empty.");
                }
            }
        }

        #endregion

        #region Helper Methods

        private void NewDataSource()
        {
            this.cubeDimensionBrowser.OlapDataManager = this.MetaTreeOlapDataManager;
            this.cubeSelector.OlapDataManager = this.MetaTreeOlapDataManager;
            this.axisBuilderColumn.OlapDataManager = this.MetaTreeOlapDataManager;
            this.axisBuilderRow.OlapDataManager = this.MetaTreeOlapDataManager;
            this.axisBuilderSlicer.OlapDataManager = this.MetaTreeOlapDataManager;
        }

        private void UpdateDataSource()
        {
            ////Initilize new manager to all controls
            this.NewDataSource();
            this.MetaTreeOlapDataManager.SetCurrentReport(this.olapReport);
        }

        private void CheckAndUpdateStatus()
        {
            if (this.isExisting)
            {
                this.StatusOfExisting();
            }
            else
            {
                this.StatusOfNew();
            }
        }

        private void StatusOfNew()
        {
            if (this.axisBuilderColumn.MetaTreeNodes.Count > 0 ||
                this.axisBuilderRow.MetaTreeNodes.Count > 0 ||
                this.axisBuilderSlicer.MetaTreeNodes.Count > 0)
            {
                this.UpdateButtonStatus(true, true, false);
            }
            else
            {
                this.UpdateButtonStatus(true, false, false);
            }
        }

        private void StatusOfExisting()
        {
            if (this.axisBuilderColumn.MetaTreeNodes.Count > 0 ||
                this.axisBuilderRow.MetaTreeNodes.Count > 0 ||
                this.axisBuilderSlicer.MetaTreeNodes.Count > 0)
            {
                this.UpdateButtonStatus(true, true, true);
            }
            else
            {
                this.UpdateButtonStatus(true, false, false);
            }
        }

        #endregion

        #region Events
        
        private void axisBuilderColumn_Drop(object sender, DragEventArgs e)
        {
            this.CheckAndUpdateStatus();
        }

        private void axisBuilderRow_Drop(object sender, DragEventArgs e)
        {
            this.CheckAndUpdateStatus();
        }

        private void axisBuilderSlicer_Drop(object sender, DragEventArgs e)
        {
            this.CheckAndUpdateStatus();
        }

        #endregion

        #region Commit Method

        public void CommitChanges()
        {
            if (this.MetaTreeOlapDataManager != null)
            {
                if (this.selectedControl.Properties["DesignerSettings"].ComputedValue != null)
                {
                    this.selectedControl.Properties["DesignerSettings"].Value.Properties["CurrentCubeName"].SetValue(this.MetaTreeOlapDataManager.CurrentCubeName);
                    // this.selectedControl.Properties["DesignerSettings"].Value.Properties["CategoricalElements"].SetValue(this.MetaTreeOlapDataManager.CurrentReport.CategoricalElements);
                    // this.selectedControl.Properties["DesignerSettings"].Value.Properties["SeriesElements"].SetValue(this.MetaTreeOlapDataManager.CurrentReport.SeriesElements);
                    // this.selectedControl.Properties["DesignerSettings"].Value.Properties["SlicerElements"].SetValue(this.MetaTreeOlapDataManager.CurrentReport.SlicerElements);
                    this.selectedControl.Properties["DesignerSettings"].Value.Properties["Report"].SetValue(this.GenerateReport(this.MetaTreeOlapDataManager.CurrentReport));
                    // MessageBox.Show("Slice and Dice model station commit completed.");
                }
                else
                {
                    // MessageBox.Show("Commit unsuccessful. Designer settings was not properly saved.");
                }
            }
            else
            {
                // MessageBox.Show("Could not commit the changes. MetaTree is null or badly formatted.");
            }
        }

        #endregion

        #region Report Generation

        #region Helper Method

        private Report GenerateReport(Syncfusion.Olap.Reports.OlapReport currentReport)
        {
            var v = currentReport;

            ////
            //// For categorical axis wrapper processing.
            ////

            if (v.CategoricalElements != null)
            {
                if (v.CategoricalElements.Count > 0)
                {
                    //// Gets categorical report elements.
                    this.report.CategoricalAxis = this.CreateCategoricalReportElements(v.CategoricalElements);
                }
                else
                {
                    // Unset the value.
                }
            }

            ////
            //// For series axis wrapper processing.
            ////

            if (v.SeriesElements != null)
            {
                if (v.SeriesElements.Count > 0)
                {
                    //// Gets series report elements.
                    this.report.SeriesAxis = this.CreateSeriesReportElements(v.SeriesElements);
                }
                else
                {
                    // Unset the value.
                }
            }

            ////
            //// For slicer axis wrapper processing.
            ////

            if (v.SlicerElements != null)
            {
                if (v.SlicerElements.Count > 0)
                {
                    //// Gets slicer report elements.
                    this.report.SlicerAxis = this.CreateSlicerReportElements(v.SlicerElements);
                }
                else
                {
                    // Unset the value.
                }
            }

            // Syncfusion.Olap.Common.Common.ToXml(this.report, @"c:\OlapReport.xml", false);
            return this.report;
        }

        #endregion

        #region Axis Processing

        private CategoricalAxis CreateCategoricalReportElements(Syncfusion.Olap.Reports.Items items)
        {
            return this.ProcessAxis(items, new CategoricalAxis()) as CategoricalAxis;
        }

        private SeriesAxis CreateSeriesReportElements(Syncfusion.Olap.Reports.Items items)
        {
            return this.ProcessAxis(items, new SeriesAxis()) as SeriesAxis;
        }

        private SlicerAxis CreateSlicerReportElements(Syncfusion.Olap.Reports.Items items)
        {
            return this.ProcessAxis(items, new SlicerAxis()) as SlicerAxis;
        }

        private IAxisElements ProcessAxis(Syncfusion.Olap.Reports.Items items, IAxisElements tAxis)
        {
            //// Processing the T axis.
            foreach (Syncfusion.Olap.Reports.Item item in items)
            {
                var type = item.ElementValue;

                if (type is Syncfusion.Olap.Reports.MeasureElements)
                {
                    //// Measure will be considered as a dimension. But, collection of measures. Processed once [TestCase] as a dimension.
                    if (tAxis.ReportMeasureElements == null)
                    {
                        //// When first time, the measure elements will be null.
                        tAxis.ReportMeasureElements = new ReportMeasureElements();
                    }

                    tAxis.ReportMeasureElements.AddRange(this.GetReportMeasureElements(item));
                }
                else if (type is Syncfusion.Olap.Reports.KpiElements)
                {
                    //// Kpi will be considered as a dimension. But, collection of kpi. Processed once [TestCase] as a dimension.
                    if (tAxis.ReportKpiElements == null)
                    {
                        //// When first time, the kpi elements will be null.
                        tAxis.ReportKpiElements = new ReportKpiElements();
                    }

                    tAxis.ReportKpiElements.AddRange(this.GetReportKpiElements(item));
                }
                else
                {
                    if (tAxis.ReportDimensionElements == null)
                    {
                        //// When first time, the dimension elements will be null. Processed more then once iff, more then one dimension in the axis. [TestCase]
                        tAxis.ReportDimensionElements = new ReportDimensionElements();
                    }

                    tAxis.ReportDimensionElements.Add(this.GetReportDimensionElement(item));
                }
            }

            return tAxis;
        }

        #endregion

        #region Element Processing

        private ReportMeasureElements GetReportMeasureElements(Syncfusion.Olap.Reports.Item item)
        {
            ReportMeasureElements reportMeasureElements = new ReportMeasureElements();

            foreach (Syncfusion.Olap.Reports.MeasureElement me in (item.ElementValue as Syncfusion.Olap.Reports.MeasureElements).Elements)
            {
                reportMeasureElements.Add(new ReportMeasureElement() { Name = me.Name });
            }

            return reportMeasureElements;
        }

        private ReportKpiElements GetReportKpiElements(Syncfusion.Olap.Reports.Item item)
        {
            ReportKpiElements reportKpiElements = new ReportKpiElements();

            foreach (Syncfusion.Olap.Reports.KpiElement kpi in (item.ElementValue as Syncfusion.Olap.Reports.KpiElements).Elements)
            {
                //// ToDo: Create a property configuration window to alter the properties of Kpi.
                reportKpiElements.Add(new ReportKpiElement() { Name = kpi.Name, ShowKPIGoal = kpi.ShowKPIGoal, ShowKPIStatus = kpi.ShowKPIStatus, ShowKPITrend = kpi.ShowKPITrend, ShowKPIValue = kpi.ShowKPIValue });
            }

            return reportKpiElements;
        }

        private ReportDimensionElement GetReportDimensionElement(Syncfusion.Olap.Reports.Item item)
        {
            ReportDimensionElement reportDimensionElement = new ReportDimensionElement();
            var dimensionElement = item.ElementValue as Syncfusion.Olap.Reports.DimensionElement;

            ////
            //// Dimension element calculation.
            ////

            if (dimensionElement != null)
            {
                reportDimensionElement.DimensionName = dimensionElement.Name;
                reportDimensionElement.HierarchyName = dimensionElement.HierarchyName;

                if (dimensionElement.Hierarchy != null)
                {
                    if (dimensionElement.Hierarchy.LevelElements != null)
                    {
                        if (dimensionElement.Hierarchy.LevelElements.Count > 0)
                        {
                            reportDimensionElement.LevelName = dimensionElement.Hierarchy.LevelElements[0].Name;
                        }
                    }
                }
            }

            ////
            //// Excluded dimension element calculation.
            ////

            if (item.ExcludedElementValue != null)
            {
                reportDimensionElement.ExcludedMembers = new ExcludedMembers();
                var excludedDimensionElement = item.ExcludedElementValue as Syncfusion.Olap.Reports.DimensionElement;

                if (excludedDimensionElement.Hierarchy != null)
                {
                    if (excludedDimensionElement.Hierarchy.LevelElements != null)
                    {
                        if (excludedDimensionElement.Hierarchy.LevelElements.Count > 0)
                        {
                            //var members = excludedDimensionElement.Hierarchy.LevelElements[0].MemberElements;

                            //foreach (Syncfusion.Olap.Reports.MemberElement m in members)
                            //{
                            //    reportDimensionElement.ExcludedMembers.Add(new ExcludedMember() { MemberName = m.Name, DimensionName = excludedDimensionElement.Name, HierarchyName = excludedDimensionElement.HierarchyName, LevelName = excludedDimensionElement.Hierarchy.LevelElements[0].Name });
                            //}

                            foreach (Syncfusion.Olap.Reports.LevelElement le in excludedDimensionElement.Hierarchy.LevelElements)
                            {
                                var members = le.MemberElements;

                                foreach (Syncfusion.Olap.Reports.MemberElement m in members)
                                {
                                    reportDimensionElement.ExcludedMembers.Add(new ExcludedMember() { MemberName = m.Name, DimensionName = excludedDimensionElement.Name, HierarchyName = le.ParentHierarchy.Name, LevelName = le.Name });
                                }
                            }
                        }
                    }
                }
            }

            return reportDimensionElement;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the meta tree olap data manager.
        /// </summary>
        /// <value>The meta tree olap data manager.</value>
        public OlapDataManager MetaTreeOlapDataManager
        {
            get;
            set;
        }

        #endregion

        #region IWizardNavigationButtonsStatus Members

        private bool _CanBackButtonEnabled;
        public bool CanBackButtonEnabled
        {
            get
            {
                return _CanBackButtonEnabled;
            }
            set
            {
                _CanBackButtonEnabled = value;
            }
        }

        private bool _CanFinishButtonEnabled;
        public bool CanFinishButtonEnabled
        {
            get
            {
                return _CanFinishButtonEnabled;
            }
            set
            {
                _CanFinishButtonEnabled = value;
            }
        }

        private bool _CanNextButtonEnabled;
        public bool CanNextButtonEnabled
        {
            get
            {
                return _CanNextButtonEnabled;
            }
            set
            {
                _CanNextButtonEnabled = value;
            }
        }

        public void UpdateButtonStatus(bool back, bool next, bool finish)
        {
            this.CanBackButtonEnabled = back;
            this.CanNextButtonEnabled = next;
            this.CanFinishButtonEnabled = finish;
        }

        #endregion
    }
}
