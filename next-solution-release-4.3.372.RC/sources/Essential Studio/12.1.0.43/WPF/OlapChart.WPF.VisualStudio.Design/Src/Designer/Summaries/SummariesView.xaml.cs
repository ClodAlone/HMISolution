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
using Syncfusion.Windows.Shared.Olap;
using Syncfusion.Olap.Reports;

namespace Syncfusion.OlapChart.WPF.VisualStudio.Design
{
    /// <summary>
    /// Creates/Displays the mdx query.
    /// </summary>
    public partial class SummariesView
        : UserControl
    {
        #region Members

        private string connectionString;
        private ModelItem selectedControl;
        private WizardWindow wizardWindowInstance;
        private OlapReport olapReport;
        private Report report;
        private CubeConfiguration cubeConfiguration;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SummariesView"/> class.
        /// </summary>
        /// <param name="selectedControl">The selected control.</param>
        /// <param name="wizardWindow">The wizard window.</param>
        public SummariesView(ModelItem selectedControl, WizardWindow wizardWindow)
        {
            InitializeComponent();

            //// Selected grid's instance and wizard instance.
            this.selectedControl = selectedControl;
            this.wizardWindowInstance = wizardWindow;

            //// Initializing the cube configuration window and report.
            this.cubeConfiguration = new CubeConfiguration();
            this.report = new Report();
        } 

        #endregion

        #region Initilizer

        /// <summary>
        /// Initializes the controls.
        /// </summary>
        /// <param name="conn">The conn.</param>
        public void InitializeControls(string conn)
        {
            this.connectionString = conn;
            var designerSettings = selectedControl.Properties["DesignerSettings"].ComputedValue;

            if (designerSettings != null)
            {
                //// Connection string to check for currnet and designer settings connection string equality.
                string conStr = selectedControl.Properties["DesignerSettings"].Value.Properties["ConnectionString"].ComputedValue.ToString();

                if (!string.IsNullOrEmpty(conStr))
                {
                    if (conStr == this.connectionString)
                    {
                        // this.isExisting = true;
                        this.ExistingConnection(designerSettings, conStr);
                    }
                    else
                    {
                        //// If connection strings modified. But, user wants the same cube structure which already was created.
                        // this.isExisting = false;
                        // this.ExistingConnection(designerSettings, this.connectionString);                        
                        this.NewConnection();
                    }

                    this.ShowMdxQuery();
                }
                else
                {
                    throw new ArgumentException("Connection String is empty in designer settings.");
                }
            }
            else
            {
                //// This is being created newly. Get the connection string information passed from the connection properties view.
                this.NewConnection();
                this.ShowMdxQuery();
            }
        }

        /// <summary>
        /// News the connection.
        /// </summary>
        private void NewConnection()
        {
            if (!string.IsNullOrEmpty(this.connectionString))
            {
                // this.isExisting = false;
                this.MetaTreeOlapDataManager = new OlapDataManager(this.connectionString);                
                this.NewDataSource();
            }
            else
            {
                MessageBox.Show("Connection string is empty");
            }
        }

        /// <summary>
        /// Existings the connection.
        /// </summary>
        /// <param name="designerSettings">The designer settings.</param>
        /// <param name="conStr">The connection string.</param>
        private void ExistingConnection(object designerSettings, string conStr)
        {
            DesignerSettings desSetting = designerSettings as DesignerSettings;

            if (desSetting.IsValidConnectionString)
            {
                this.MetaTreeOlapDataManager = new OlapDataManager(conStr);
                //// ToDO: try { if connection string alone is changed, report can be rendered. 
                this.olapReport = desSetting.GetOlapReport();
                //// }
                //// catch {if exception based on cube structure mismatch [WARN User about this, load it as new cube]}
                this.UpdateDataSource();
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// New data source.
        /// </summary>
        private void NewDataSource()
        {
            //// OlapClient configuration window's olap data manage bound here.
            this.cubeConfiguration.OlapDataManager = this.MetaTreeOlapDataManager;
        }

        /// <summary>
        /// Updates the existing data source.
        /// </summary>
        private void UpdateDataSource()
        {
            ////Initilize new manager to all controls
            this.NewDataSource();
            this.MetaTreeOlapDataManager.SetCurrentReport(this.olapReport);
        }

        #endregion

        #region Commit Method

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public void CommitChanges()
        {
            if (this.MetaTreeOlapDataManager != null)
            {
                if (this.selectedControl.Properties["DesignerSettings"].ComputedValue != null)
                {                    
                    this.selectedControl.Properties["DesignerSettings"].Value.Properties["CurrentCubeName"].SetValue(this.MetaTreeOlapDataManager.CurrentCubeName);
#if SyncfusionFramework4_0 || SyncfusionFramework4_5

                    this.GenerateDirectXamlReport(this.MetaTreeOlapDataManager.CurrentReport);

#endif
#if SyncfusionFramework3_5

                    Report xamlReport = this.GenerateReport(this.MetaTreeOlapDataManager.CurrentReport);
                    this.selectedControl.Properties["DesignerSettings"].Value.Properties["Report"].SetValue(report);                    
                    //// Slice and Dice model station commit completed.

#endif
                    this.MetaTreeOlapDataManager.DataProvider.CloseConnection();
                }
                else
                {
                    throw new ArgumentNullException("Commit unsuccessful. Designer settings were not properly saved after the cube configuration process.");
                }
            }
            else
            {
                // throw new ArgumentNullException("Could not commit the changes. MetaTree is null while trying to commit the changes after the cube configuration process.");
            }
        }

        private void ShowMdxQuery()
        {
            try
            {
                if (this.MetaTreeOlapDataManager != null)
                {
                    string str = this.MetaTreeOlapDataManager.GetMDXQuery();

                    if (!string.IsNullOrEmpty(str))
                    {
                        this.txtQuery.Text = this.MetaTreeOlapDataManager.GetMDXQuery();
                    }
                    else
                    {
                        this.txtQuery.Text = string.Empty;
                    }
                }
            }
            catch
            {
                //// Current cube is null.
                this.txtQuery.Text = string.Empty;
            }
        }

        #endregion

        #region Report Generation

        #region Helper Method

        /// <summary>
        /// Generates the report.
        /// </summary>
        /// <param name="currentReport">The current report.</param>
        /// <returns>Returns XAML display report.</returns>
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
            }

            return this.report;
        }

        #endregion

        #region Axis Processing

        /// <summary>
        /// Creates the categorical report elements.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>Returns the categorical axis elements.</returns>
        private CategoricalAxis CreateCategoricalReportElements(Syncfusion.Olap.Reports.Items items)
        {
            return this.ProcessAxis(items, new CategoricalAxis()) as CategoricalAxis;
        }

        /// <summary>
        /// Creates the series report elements.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>Returns the series axis elements.</returns>
        private SeriesAxis CreateSeriesReportElements(Syncfusion.Olap.Reports.Items items)
        {
            return this.ProcessAxis(items, new SeriesAxis()) as SeriesAxis;
        }

        /// <summary>
        /// Creates the slicer report elements.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>Returns the slicer axis elements.</returns>
        private SlicerAxis CreateSlicerReportElements(Syncfusion.Olap.Reports.Items items)
        {
            return this.ProcessAxis(items, new SlicerAxis()) as SlicerAxis;
        }

        /// <summary>
        /// Processes the axis.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="tAxis">The T axis.</param>
        /// <returns>Returns the IAxisElements</returns>
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
                        //// When first time, the dimension elements will be null. Processed more then once iff, more then one dimension in the same axis. [TestCase]
                        tAxis.ReportDimensionElements = new ReportDimensionElements();
                    }

                    tAxis.ReportDimensionElements.Add(this.GetReportDimensionElement(item));
                }
            }

            return tAxis;
        }

        #endregion

        #region Element Processing

        /// <summary>
        /// Gets the report measure elements.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns XAML report measure elements.</returns>
        private ReportMeasureElements GetReportMeasureElements(Syncfusion.Olap.Reports.Item item)
        {
            ReportMeasureElements reportMeasureElements = new ReportMeasureElements();

            foreach (Syncfusion.Olap.Reports.MeasureElement me in (item.ElementValue as Syncfusion.Olap.Reports.MeasureElements).Elements)
            {
                reportMeasureElements.Add(new ReportMeasureElement() { Name = me.Name });
            }

            return reportMeasureElements;
        }

        /// <summary>
        /// Gets the report kpi elements.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns XAML report kpi elements</returns>
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

        /// <summary>
        /// Gets the report dimension element.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns the XAML report dimension element.</returns>
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
            //// Excluded dimension element calculation. [For, Excluded members]
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

        #region Report Generation 4.0

        /// <summary>
        /// Generates the direct xaml report which is compatible for VS 2010.
        /// </summary>
        /// <param name="olapReport">The olap report.</param>
        private void GenerateDirectXamlReport(OlapReport currentOlapReport)
        {
            if (currentOlapReport != null)
            {
                //// Initializing a report.
                this.selectedControl.Properties["DesignerSettings"].Value.Properties["Report"].SetValue(new Report());
                var curReport = this.selectedControl.Properties["DesignerSettings"].Value.Properties["Report"].Value;

                //// Process Categorical Axis
                if (currentOlapReport.CategoricalElements != null)
                {
                    if (currentOlapReport.CategoricalElements.Count > 0)
                    {
                        curReport.Properties["CategoricalAxis"].SetValue(new CategoricalAxis());
                        var curReportCategoricalElements = curReport.Properties["CategoricalAxis"].Value;

                        //// Process categorical axis.
                        this.ProcessCurrentAxis(currentOlapReport.CategoricalElements, curReportCategoricalElements);
                    }
                }

                //// Process Series Axis
                if (currentOlapReport.SeriesElements != null)
                {
                    if (currentOlapReport.SeriesElements.Count > 0)
                    {
                        curReport.Properties["SeriesAxis"].SetValue(new SeriesAxis());
                        var curReportSeriesElements = curReport.Properties["SeriesAxis"].Value;

                        //// Process Series axis.
                        this.ProcessCurrentAxis(currentOlapReport.SeriesElements, curReportSeriesElements);
                    }
                }

                //// Process Slicer Axis
                if (currentOlapReport.SlicerElements != null)
                {
                    if (currentOlapReport.SlicerElements.Count > 0)
                    {
                        curReport.Properties["SlicerAxis"].SetValue(new SlicerAxis());
                        var curReportSlicerElements = curReport.Properties["SlicerAxis"].Value;

                        //// Process categorical axis.
                        this.ProcessCurrentAxis(currentOlapReport.SlicerElements, curReportSlicerElements);
                    }
                }
            }
        }

        private void ProcessCurrentAxis(Items items, ModelItem curReportAxis)
        {
            foreach (Item item in items)
            {
                var type = item.ElementValue;

                //// Process Measures
                if (type is MeasureElements)
                {
                    // this.ProcessDimension(item, curReportAxis);
                    curReportAxis.Properties["ReportMeasureElements"].SetValue(new ReportMeasureElements());
                    var curReportMeasureElements = curReportAxis.Properties["ReportMeasureElements"];

                    foreach (Syncfusion.Olap.Reports.MeasureElement me in (item.ElementValue as Syncfusion.Olap.Reports.MeasureElements).Elements)
                    {
                        curReportMeasureElements.Collection.Add(new ReportMeasureElement() { Name = me.Name });
                    }
                }
                else if (type is KpiElements)
                {
                    // this.ProcessDimension(item, curReportAxis);
                    curReportAxis.Properties["ReportKpiElements"].SetValue(new ReportKpiElements());
                    var curReportKpiElements = curReportAxis.Properties["ReportKpiElements"];

                    foreach (Syncfusion.Olap.Reports.KpiElement kpi in (item.ElementValue as Syncfusion.Olap.Reports.KpiElements).Elements)
                    {
                        //// ToDo: Create a property configuration window to alter the properties of Kpi.
                        curReportKpiElements.Collection.Add(new ReportKpiElement() { Name = kpi.Name, ShowKPIGoal = kpi.ShowKPIGoal, ShowKPIStatus = kpi.ShowKPIStatus, ShowKPITrend = kpi.ShowKPITrend, ShowKPIValue = kpi.ShowKPIValue });
                    }
                }
                else
                {
                    if (curReportAxis.Properties["ReportDimensionElements"].ComputedValue == null)
                    {
                        curReportAxis.Properties["ReportDimensionElements"].SetValue(new ReportDimensionElements());
                    }

                    var curReportDimensionElements = curReportAxis.Properties["ReportDimensionElements"];
                    this.ProcessDimension(item, curReportDimensionElements);
                }
            }
        }

        private void ProcessDimension(Item item, ModelProperty curReportDimensionElements)
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
                            curReportDimensionElements.Collection.Add(reportDimensionElement);

                            ////
                            //// Excluded dimension element calculation. [For, Excluded members]
                            ////

                            if (item.ExcludedElementValue != null)
                            {
                                // reportDimensionElement.ExcludedMembers = new ExcludedMembers();
                                curReportDimensionElements.Collection.Last().Properties["ExcludedMembers"].SetValue(new ExcludedMembers());
                                var curDimensionExcludedMemebers = curReportDimensionElements.Collection.Last().Properties["ExcludedMembers"];

                                var excludedDimensionElement = item.ExcludedElementValue as Syncfusion.Olap.Reports.DimensionElement;

                                if (excludedDimensionElement.Hierarchy != null)
                                {
                                    if (excludedDimensionElement.Hierarchy.LevelElements != null)
                                    {
                                        if (excludedDimensionElement.Hierarchy.LevelElements.Count > 0)
                                        {
                                            foreach (Syncfusion.Olap.Reports.LevelElement le in excludedDimensionElement.Hierarchy.LevelElements)
                                            {
                                                var members = le.MemberElements;

                                                foreach (Syncfusion.Olap.Reports.MemberElement m in members)
                                                {
                                                    curDimensionExcludedMemebers.Collection.Add(new ExcludedMember() { MemberName = m.Name, DimensionName = excludedDimensionElement.Name, HierarchyName = le.ParentHierarchy.Name, LevelName = le.Name });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

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

        #region Events

        private void hyperlinkCreateOrEdit_Click(object sender, RoutedEventArgs e)
        {
            CubeConfiguration cubeConfig = new CubeConfiguration();
            cubeConfig.OlapDataManager = this.cubeConfiguration.OlapDataManager;
            cubeConfig.Owner = this.wizardWindowInstance;

            //// If Ok is clicked, true; else, false will be returned.
            bool? result = cubeConfig.ShowDialog();

            if (result == true)
            {
                this.ShowMdxQuery();
            }
        }

        #endregion
    }
}
