#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Win32;
using Syncfusion.Windows.Chart;
using Syncfusion.Windows.Gauge;
using Syncfusion.Windows.Reports.Common;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;
using Syncfusion.Windows.Edit;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using Syncfusion.Windows.Reports.Viewer;
using Syncfusion.Windows.PropertyGrid;
using Syncfusion.Windows.Reports.Designer.Editors;
using Syncfusion.Windows.Reports.Designer.Wizard;
using System.Net;
using Syncfusion.RDL.ServerProcessor;
using Syncfusion.RDL.Data;
using Syncfusion.Windows.ReportDesigner.Resources;


namespace Syncfusion.Windows.Reports.Designer
{
    /// <summary>
    /// Interaction logic for DesignerPanel.xaml
    /// </summary>
    public partial class ReportDesignView : UserControl
    {

        RDL.DOM.RDLType Type = RDL.DOM.RDLType.RDL2008;
        private int untitledCount = 1;
        private Object ownerWindow;
        private object selectedObject = null;
  
        internal string recentReportsUrl = Environment.CurrentDirectory + "\\RecentReports.xml";
        internal int recentReportsLimit = 15;
        internal static DesignPanel CurrentPanel;
        internal RecentReports recentReports = null;
        internal List<string> embeddedImages = new List<string>();
        internal List<string> toggleItems = new List<string>();
        internal ServerReportProcessor ReportServerProcessor;

        #region InternalProperties


        internal List<string> DataSetFields
        {
            get;
            set;
        }

        internal List<string> DataSetNames
        {
            get;
            set;
        }

        internal ReportsCollection ReportsCollection
        {
            get;
            set;
        }

        internal CachedAssemblyInfos AssemblyInfos
        {
            get;
            set;
        }

        internal CachedObjectInfos ObjectInfos
        {
            get;
            set;
        }

        internal List<ReportDataExplorer> ReportDatas
        {
            get;
            set;
        }

        internal List<ReportPropertyGrid> ReportPropertyGrids
        {
            get;
            set;
        }

        internal List<TablixGroupingPanel> TablixGroupingPanels
        {
            get;
            set;
        }

        internal List<ReportToolBox> ReportToolBoxs
        {
            get;
            set;
        }

        internal string ExcludeString
        {
            get
            {
                if (this.Type == Syncfusion.RDL.DOM.RDLType.RDL2008)
                {
                    return "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";
                }
                else if (this.Type == Syncfusion.RDL.DOM.RDLType.RDL2010)
                {
                    return "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";
                }

                return string.Empty;
            }
        }

        internal string VisualStyle
        {
            get
            {
                return (string)GetValue(VisualStyleProperty);
            }
            set
            {
                foreach (var reportData in this.ReportDatas)
                {
                    reportData.UpdateTreeStyle(this.VisualStyle);
                    //reportData.UpdateCulture();
                }

                if (value != this.VisualStyle)
                {
                    SetValue(VisualStyleProperty, value);
                    UpdateVisualStyle();
                }                
            }
        }

        internal static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(string), typeof(ReportDesignView), new UIPropertyMetadata("Office2007Blue"));

        internal bool IsRdlVisible
        {
            get { return (bool)GetValue(IsRdlVisibleProperty); }
            set { SetValue(IsRdlVisibleProperty, value); }
        }

        internal static readonly DependencyProperty IsRdlVisibleProperty =
            DependencyProperty.Register("IsRdlVisible", typeof(bool), typeof(ReportDesignView), new UIPropertyMetadata(false, null));

        public ReportFormat ReportFormat
        {
            get
            {
                return (ReportFormat)GetValue(ReportFormatProperty);
            }
            set
            {
                SetValue(ReportFormatProperty, value);
            }
        }

        #endregion

        // Using a DependencyProperty as the backing store for IsMultiReportEnabled.  This enables animation, styling, binding, etc...
        public bool EnableMDIDesigner
        {
            get
            {
                return (bool)GetValue(EnableMDIDesignerProperty);
            }
            set
            {
                SetValue(EnableMDIDesignerProperty, value);
            }
        }

        public static readonly DependencyProperty EnableMDIDesignerProperty =
            DependencyProperty.Register("EnableMDIDesigner", typeof(bool), typeof(ReportDesignView), new UIPropertyMetadata((bool)true, OnEnableMDIDesignerPropertyChanged));

        static void OnEnableMDIDesignerPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesignView designerpanel = dependencyObject as ReportDesignView;

            if (!DesignerProperties.GetIsInDesignMode(designerpanel) && designerpanel != null)
            {
                if ((bool)e.NewValue == false)
                {
                    designerpanel.UpdateDesignMode();
                    designerpanel.NewBlankReport();
                }
            }
        }

        public DesignMode DesignMode
        {
            get
            {
                return (DesignMode)GetValue(DesignModeProperty);
            }
            set
            {
                SetValue(DesignModeProperty, value);
            }
        }

        public static readonly DependencyProperty DesignModeProperty =
            DependencyProperty.Register("DesignMode", typeof(DesignMode), typeof(ReportDesignView), new UIPropertyMetadata(DesignMode.RDL, null));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportServerUrl"/> dependency property.
        /// </summary>
        [Browsable(false)]
        public string ReportServerUrl
        {
            get
            {
                return (string)this.GetValue(ReportServerUrlProperty);
            }

            set
            {
                this.SetValue(ReportServerUrlProperty, value);
            }
        }

        public static readonly DependencyProperty ReportServerUrlProperty =
        DependencyProperty.Register("ReportServerUrl", typeof(string), typeof(ReportDesignView), new PropertyMetadata(string.Empty, null));

        [Browsable(false)]
        public ICredentials ReportServerCredential
        {
            get
            {
                return (ICredentials)this.GetValue(ReportServerCredentialProperty);
            }

            set
            {
                this.SetValue(ReportServerCredentialProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportServerCredential"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerCredentialProperty =
        DependencyProperty.Register("ReportServerCredential", typeof(ICredentials), typeof(ReportDesignView), new PropertyMetadata(null, null));

        /// Get or Set the Report Server Forms Credential.
        /// </summary>
        /// <value>The Report server forms Credential.</value>
        [Browsable(false)]
        public ReportServerFormsCredential ReportServerFormsCredential
        {
            get
            {
                return (ReportServerFormsCredential)this.GetValue(ReportServerFormsCredentialProperty);
            }

            set
            {
                this.SetValue(ReportServerFormsCredentialProperty, value);
            }
        }

        public static readonly DependencyProperty ReportServerFormsCredentialProperty =
      DependencyProperty.Register("ReportServerFormsCredential", typeof(ReportServerFormsCredential), typeof(ReportDesignView), new PropertyMetadata(null, null));

        public DrawingReportItem DrawingReportItem
        {
            get { return (DrawingReportItem)GetValue(DrawingReportItemProperty); }
            set { SetValue(DrawingReportItemProperty, value); }
        }

        public static readonly DependencyProperty DrawingReportItemProperty =
            DependencyProperty.Register("DrawingReportItem", typeof(DrawingReportItem), typeof(ReportDesignView), new UIPropertyMetadata(DrawingReportItem.None, DrawingReportItemPropertyChanged));

        static void DrawingReportItemPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesignView designerpanel = dependencyObject as ReportDesignView;

            if (!DesignerProperties.GetIsInDesignMode(designerpanel) && designerpanel != null)
            {
                designerpanel.UpdateDesignPanelReportItem(designerpanel.DrawingReportItem);
            }
        }

        public List<Assembly> Assemblies
        {
            get
            {
                return (List<Assembly>)GetValue(AssembliesProperty);
            }

            set
            {
                SetValue(AssembliesProperty, value);
            }
        }

        public static readonly DependencyProperty AssembliesProperty =
        DependencyProperty.Register("Assemblies", typeof(List<Assembly>), typeof(ReportDesignView), new UIPropertyMetadata(new List<Assembly>(), null));


        public static readonly DependencyProperty ReportFormatProperty =
            DependencyProperty.Register("ReportFormat", typeof(ReportFormat), typeof(ReportDesignView), new UIPropertyMetadata(ReportFormat.RDL2010, null));

        public bool ShowRuler
        {
            get { return (bool)GetValue(ShowRulerProperty); }
            set { SetValue(ShowRulerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRulerVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowRulerProperty =
            DependencyProperty.Register("ShowRuler", typeof(bool), typeof(ReportDesignView), new UIPropertyMetadata((bool)true, ShowRulerPropertyChanged));

        internal static void ShowRulerPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesignView designerpanel = dependencyObject as ReportDesignView;
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue == false)
                {
                    designerpanel.ShowRuler = false;
                }
                else
                {
                    designerpanel.ShowRuler = true;
                }
            }
        }

        internal bool ShowProperties
        {
            get { return (bool)GetValue(ShowPropertiesProperty); }
            set { SetValue(ShowPropertiesProperty, value); }
        }

        internal static readonly DependencyProperty ShowPropertiesProperty = DependencyProperty.Register("ShowProperties", typeof(bool), typeof(ReportDesignView), new UIPropertyMetadata(true, OnShowPropertiesPropertyChanged));

        private static void OnShowPropertiesPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesignView designerpanel = dependencyObject as ReportDesignView;
            if (e.NewValue != e.OldValue)
            {
                bool showPropertyWindow = (bool)e.NewValue;
                if (showPropertyWindow)
                {
                    if (designerpanel.selectedObject != null )
                    {
                        designerpanel.UpdatePropertyObject(designerpanel.selectedObject);
                    }
                }
            }
        }

        public bool ShowGrouping
        {
            get { return (bool)GetValue(ShowGroupingProperty); }
            set { SetValue(ShowGroupingProperty, value); }
        }

        public static readonly DependencyProperty ShowGroupingProperty =
            DependencyProperty.Register("ShowGrouping", typeof(bool), typeof(ReportDesignView), new UIPropertyMetadata(false, OnShowGroupingPropertyChanged));

        public static void OnShowGroupingPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesignView designerpanel = dependencyObject as ReportDesignView;

            if (e.NewValue != e.OldValue)
            {
                bool showGroupingWindow = (bool)e.NewValue;
                if (showGroupingWindow)
                {
                    try
                    {
                        var selected = designerpanel.GetDisplayTabDesignPanel().SelectedReportItems.Where(reportItem => reportItem is TablixControl).FirstOrDefault();
                        if (selected != null)
                        {
                            designerpanel.UpdateGroupingObject(selected);
                        }
                    }
                    catch { }
                }

            }
        }

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHeaderVisible.
        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register("ShowHeader", typeof(bool), typeof(ReportDesignView), new UIPropertyMetadata((bool)false, OnShowHeaderPropertyChanged));

        internal static void OnShowHeaderPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesignView designview = dependencyObject as ReportDesignView;
            DesignPanel designPanel = designview.GetDisplayTabDesignPanel();

            if (e.OldValue != e.NewValue && designPanel != null)
            {
                designPanel.IsHeaderVisible = (bool)e.NewValue;
            }
        }

        public bool ShowFooter
        {
            get { return (bool)GetValue(ShowFooterProperty); }
            set { SetValue(ShowFooterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFooterVisible.
        public static readonly DependencyProperty ShowFooterProperty =
            DependencyProperty.Register("ShowFooter", typeof(bool), typeof(ReportDesignView), new UIPropertyMetadata((bool)true, OnShowFooterPropertyChanged));

        internal static void OnShowFooterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesignView designview = dependencyObject as ReportDesignView;
            DesignPanel designPanel = designview.GetDisplayTabDesignPanel();

            if (e.OldValue != e.NewValue && designPanel != null)
            {
                designPanel.IsFooterVisible = (bool)e.NewValue;
            }
        }

        public bool ShowHelp
        {
            get { return (bool)GetValue(ShowHelpProperty); }
            set { SetValue(ShowHelpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowHelp.
        public static readonly DependencyProperty ShowHelpProperty =
            DependencyProperty.Register("ShowHelp", typeof(bool), typeof(ReportDesignView), new UIPropertyMetadata((bool)true));

        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ReportDesignView), new UIPropertyMetadata((double)100));

        public ReportDesignView()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                this.ReportsCollection = new ReportsCollection();
                this.ReportDatas = new List<ReportDataExplorer>();
                this.ReportPropertyGrids = new List<ReportPropertyGrid>();
                this.ReportToolBoxs = new List<ReportToolBox>();
                this.TablixGroupingPanels = new List<TablixGroupingPanel>();
                this.DataSetNames = new List<string>();
                this.DataSetFields = new List<string>();
                this.AssemblyInfos = new CachedAssemblyInfos();
                this.ObjectInfos = new CachedObjectInfos();
                //this.ReportServerProcessor = new ServerReportProcessor();
                this.reportViewerControl.ShowToolBar = false;
                this.TabControl.SelectedItemChangedEvent += new SelectedItemChangedEventHandler(TabControl_SelectedItemChangedEvent);
                this.TabControl.OnCloseButtonClick += new OnCloseTabsEventHandler(TabControl_OnCloseButtonClick);
                this.TabControl.EnableLabelEdit = false;
                this.TabControl.AllowDragDrop = false;
            }
            else
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "<<Designer Panel>>";
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                Border border = new Border();
                border.Background = Brushes.LightGray;
                border.BorderThickness = new Thickness(1);
                border.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                border.Child = textBlock;

                Grid grid = new Grid();
                grid.Children.Add(border);

                this.MinHeight = 200;
                this.MinWidth = 300;
                this.AddChild(grid);
            }
        }

        private void DesingerPanel_load(object sender, RoutedEventArgs e)
        {
            this.VisualStyle = (SkinStorage.GetVisualStyle(this));

            if (this.DesignMode == DesignMode.RDLC)
            {
                this.UpdateDesignMode();
            }

            if ((this.TabControl.Items.Count == 0 && this.EnableMDIDesigner) || (this.SingleDocument.Content == null && !this.EnableMDIDesigner))
            {
                this.NewBlankReport();
            }
        }

        void UpdateDesignMode()
        {
            foreach (var reportData in this.ReportDatas)
            {
                reportData.DesignerMode = this.DesignMode;
            }
        }

        internal Syncfusion.Windows.Reports.Designer.Controls.DesignPanel GetDisplayTabDesignPanel()
        {
            Syncfusion.Windows.Reports.Designer.Controls.DesignPanel designPanelTemp = null;

            if (EnableMDIDesigner)
            {
                foreach (Syncfusion.Windows.Tools.Controls.TabItemExt tabItem in this.TabControl.Items)
                {
                    if (tabItem.IsSelected == true)
                    {
                        RDLDesignerControl rdlViewControl = tabItem.Content as RDLDesignerControl;
                        designPanelTemp = rdlViewControl.DesignControl;
                        CurrentPanel = designPanelTemp;
                    }
                }
            }
            else
            {
                designPanelTemp = ((RDLDesignerControl)this.SingleDocument.Content).DesignControl;
            }

            return designPanelTemp;
        }

        internal void UpdateDesignPanelReportItem(DrawingReportItem reportItemType)
        {
            DesignPanel designPanel = this.GetDisplayTabDesignPanel();
            designPanel.DrawReportItemControl(reportItemType);
        }

        #region tabbing

        private DesignPanel GetDesignPanel(double Key)
        {
            Syncfusion.RDL.DOM.ReportDefinition reportSettings = (from reportInformation in this.ReportsCollection
                                                                              where reportInformation.Key == Key
                                                                              select reportInformation.Report).FirstOrDefault();
            if (reportSettings != null)
            {
                DesignPanel DesignPanel = new DesignPanel(reportSettings, this);
                return DesignPanel;
            }

            return null;
        }

        internal Syncfusion.RDL.DOM.ReportDefinition GetDisplayReportSettings()
        {
            return this.GetDisplayTabDesignPanel().GetReportDefinition();
        }

        private Syncfusion.RDL.DOM.ReportDefinition GetReportSettingOnKey(double Key)
        {
            if (EnableMDIDesigner)
            {
                foreach (TabItemExt tabItem in this.TabControl.Items)
                {
                    if (Convert.ToDouble(tabItem.Tag) == Key)
                    {
                        return this.GetRenderedReportSettings(tabItem);
                    }
                }
            }
            else if (Convert.ToDouble(this.SingleDocument.Tag) == Key)
            {
                return this.GetRenderedReportSettings(this.SingleDocument);
            }

            return null;
        }

        public void NewReport()
        {
            this.NewBlankReport();
        }

        private void NewBlankReport()
        {
            try
            {
                string reportName = "Untitled" + untitledCount++.ToString() +"."+this.DesignMode.ToString().ToLower();
                Syncfusion.RDL.DOM.ReportDefinition newReport = this.GetNewReportSettings();
                double reportKey = this.PopulateReportsCollection(null, newReport);
                this.NewTabSplitterItem(reportName, reportKey);
                this.GetReportInformationOnKey(reportKey).DirtyReport = this.GetDirtyReport(newReport);
                this.RaiseNewReportOpen(reportName);
            }
            catch (Exception)
            {
            }
        }

        #region Reports Collection Related

        private Syncfusion.RDL.DOM.ReportDefinition GetNewReportSettings()
        {
            RDL.DOM.ReportDefinition report = new RDL.DOM.ReportDefinition();

            report.EmbeddedImages = new RDL.DOM.EmbeddedImages();
            report.DataSources = new RDL.DOM.DataSources();
            report.DataSets = new RDL.DOM.DataSets();

            RDL.DOM.Page page = new RDL.DOM.Page();
            page.PageFooter = new RDL.DOM.PageFooter();
            page.PageFooter.Height = new RDL.DOM.Size("0.5in");

            RDL.DOM.Body body = new RDL.DOM.Body();
            body.Height = new Syncfusion.RDL.DOM.Size("2.3in");

            if (this.ReportFormat == Designer.ReportFormat.RDL2010)
            {
                report.RDLType = RDL.DOM.RDLType.RDL2010;
                report.ReportSections = new RDL.DOM.ReportSections();
                RDL.DOM.ReportSection section = new RDL.DOM.ReportSection();
                section.Page = page;
                section.Width = new RDL.DOM.Size("6in");
                report.ReportSections.Add(section);
                section.Body = body;
            }
            else
            {
                report.RDLType = RDL.DOM.RDLType.RDL2008;
                report.Page = page;
                report.Width = new RDL.DOM.Size("6in");
                report.Body = body;
            }

            return report;
        }

        internal ReportInformation GetReportInformationOnKey(double ReportKey)
        {
            ReportInformation report = (from reportInformation in this.ReportsCollection
                                        where reportInformation.Key == ReportKey
                                        select reportInformation).SingleOrDefault();

            return report;
        }

        #endregion

        private void NewTabSplitterItem(string reportName, double reportKey)
        {
            try
            {
                #region InnerTab

                if (EnableMDIDesigner)
                {
                    this.SingleDocument.Visibility = System.Windows.Visibility.Collapsed;
                    this.MDIDocument.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.MDIDocument.Visibility = System.Windows.Visibility.Collapsed;
                    this.SingleDocument.Visibility = System.Windows.Visibility.Visible;
                }

                RDLDesignerControl rdlViewControl = new RDLDesignerControl();
                DesignPanel panel = this.GetDesignPanel(reportKey);
                CurrentPanel = panel;
                rdlViewControl.DesignControl = panel;

                Binding rdlbinding = new Binding();
                rdlbinding.Source = this;
                rdlbinding.Path = new PropertyPath("IsRdlVisible");
                rdlbinding.Mode = BindingMode.OneWay;
                rdlViewControl.SetBinding(RDLDesignerControl.IsRdlVisibleProperty, rdlbinding);

                Binding rulerbinding = new Binding();
                rulerbinding.Source = this;
                rulerbinding.Path = new PropertyPath("ShowRuler");
                rulerbinding.Mode = BindingMode.OneWay;
                panel.SetBinding(DesignPanel.IsRulerVisibleProperty, rulerbinding);

                Binding rulerStylebinding = new Binding();
                rulerStylebinding.Source = this;
                rulerStylebinding.Path = new PropertyPath("VisualStyle");
                rulerStylebinding.Mode = BindingMode.OneWay;
                panel.SetBinding(DesignPanel.VisualStyleProperty, rulerStylebinding);

                Binding helpbinding = new Binding();
                helpbinding.Source = this;
                helpbinding.Path = new PropertyPath("ShowHelp");
                helpbinding.Mode = BindingMode.OneWay;
                panel.SetBinding(DesignPanel.ShowHelpProperty, helpbinding);

                Binding zoombinding = new Binding();
                zoombinding.Source = this;
                zoombinding.Path = new PropertyPath("ZoomFactor");
                zoombinding.Mode = BindingMode.OneWay;
                panel.SetBinding(DesignPanel.ZoomFactorProperty, zoombinding);

                panel.ReportReportItemDrawn += (sen, arg) =>
                       {
                           this.DrawingReportItem = Designer.DrawingReportItem.None;
                           this.RaiseReportItemDrawnEvent(this, arg);
                           this.ToggleItemsUpdate(panel.reportItems);
                       };

                panel.ReportEmbeddedImageCollectionModified += (sen, arg) =>
                {
                    this.EmbeddedImagesUpdate(panel.EmbeddedImages);
                };

                panel.ReportDataSourceCollectionModified += (sen, arg) =>
                {
                    this.DataSourcesUpdate(panel.DataSources);
                };

                panel.ReportDataSetCollectionModified += (sen, arg) =>
                {
                    this.DataSetsUpdate(panel.DataSets);
                };

                panel.ReportDataSetFieldCollectionModified += (sen, arg) =>
                {
                    this.DataSetsUpdate(panel.DataSets);
                };

                panel.ReportParameterCollectionModified += (sen, arg) =>
                {
                    this.ParametersUpdate(panel.ReportParameters);
                };

                panel.ReportItemSelected += (sen, arg) =>
                {
                    if (this.ShowProperties)
                    {
                        this.UpdatePropertyObject(arg.SelectedItem);

                        if (arg.SelectedItem is IReportItemProperties)
                        {
                            this.ToggleItemsUpdate(panel.reportItems);
                        }
                    }
                    else
                    {
                        selectedObject = arg.SelectedItem;
                    }
                    if (this.ShowGrouping && sen is TablixControl)
                    {
                        this.UpdateGroupingObject(sen);
                    }

                };

                this.UpdateDesignPanel(panel);
                this.UpdateDesignerToolboxObj();

                if (!this.EnableMDIDesigner)
                {
                    this.DataSourcesUpdate(panel.DataSources);
                    this.EmbeddedImagesUpdate(panel.EmbeddedImages);
                    this.ParametersUpdate(panel.ReportParameters);
                    this.DataSetsUpdate(panel.DataSets);
                    this.ToggleItemsUpdate(panel.reportItems);
                }

                #endregion

                if (EnableMDIDesigner == false)
                {
                    this.TabControl.Items.Clear();
                }

                if (EnableMDIDesigner)
                {
                    TabItemExt tabItemMain = new TabItemExt();
                    tabItemMain.Header = reportName;
                    tabItemMain.Content = rdlViewControl;
                    tabItemMain.Tag = reportKey;
                    panel.Tag = reportKey;
                    this.TabControl.Items.Add(tabItemMain);
                    this.TabControl.SelectedItem = tabItemMain;

                    if (this.ReportsCollection.Count > 1)
                    {
                        this.TabControl.CloseButtonType = CloseButtonType.Common;
                        this.TabControl.DefaultContextMenuItemVisibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.TabControl.CloseButtonType = CloseButtonType.Hide;
                        this.TabControl.DefaultContextMenuItemVisibility= System.Windows.Visibility.Hidden;
                    }
                }
                else
                {
                    this.SingleDocument.Content = rdlViewControl;
                    this.SingleDocument.Tag = reportKey;
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Inner Tab Exception : " + ee.ToString());
            }
        }

        private double PopulateReportsCollection(ReportInfo reportInfo, Syncfusion.RDL.DOM.ReportDefinition report)
        {
            if (!EnableMDIDesigner)
            {
                this.ReportsCollection.Clear();
            }

            ReportInformation reportInformation = new ReportInformation();
            reportInformation.Report = report;

            if (reportInfo != null)
            {
                reportInformation.ReportName = System.IO.Path.GetFileName(reportInfo.ReportPath);
                reportInformation.ReportPath = reportInfo.ReportPath;
                reportInformation.ReportServer = reportInfo.ReportServer;
                reportInformation.ReportServerCredential = reportInfo.ReportServerCredential;
            }

            double topKey = (from rptInfo in this.ReportsCollection
                             orderby rptInfo.Key descending
                             select rptInfo.Key).FirstOrDefault();

            double reportKey = 1;

            if (topKey != 0)
            {
                reportKey = topKey + 1;
            }

            reportInformation.Key = reportKey;
            this.ReportsCollection.Add(reportInformation);
            return reportKey;
        }


        private void FocusTab(double ReportKey)
        {
            if (EnableMDIDesigner)
            {
                foreach (TabItemExt tabItem in this.TabControl.Items)
                {
                    if (Convert.ToDouble(tabItem.Tag) == ReportKey)
                    {
                        this.TabControl.SelectedItem = tabItem;
                    }
                }

            }
        }

        internal void ResetDirtyReport()
        {
            this.SetDirtyReport(GetDisplayTabKey());
        }

        private double GetDisplayTabKey()
        {
            double reportKey = 0;

            if (EnableMDIDesigner)
            {
                foreach (TabItemExt tabItem in this.TabControl.Items)
                {
                    if (tabItem.IsSelected == true)
                    {
                        reportKey = Convert.ToDouble(tabItem.Tag);
                    }
                }
            }
            else
            {
                reportKey = Convert.ToDouble(this.SingleDocument.Tag);
            }

            return reportKey;
        }

        internal string GetDirtyReport(RDL.DOM.ReportDefinition report)
        {
            return Syncfusion.Windows.Reports.Designer.Serializer.Common.SerializeObject<Syncfusion.RDL.DOM.ReportDefinition>(report);
        }

        private Syncfusion.RDL.DOM.ReportDefinition GetRenderedReportSettings(ContentControl contentControl)
        {
            RDLDesignerControl rdlViewControl = contentControl.Content as RDLDesignerControl;
            DesignPanel designPanel = rdlViewControl.DesignControl;
            return designPanel.GetReportDefinition();
        }

        internal Syncfusion.RDL.DOM.ReportDefinition GetRenderedReportSettings(TabItemExt tabItem)
        {
            RDLDesignerControl rdlViewControl = tabItem.Content as RDLDesignerControl;
            DesignPanel designPanel = rdlViewControl.DesignControl;
            return designPanel.GetReportDefinition();
        }

        private void SetTabInformation(double ReportKey, string HeaderName)
        {
            if (EnableMDIDesigner)
            {
                foreach (TabItemExt tabItem in this.TabControl.Items)
                {
                    if (Convert.ToDouble(tabItem.Tag) == ReportKey)
                    {
                        tabItem.Header = HeaderName;
                    }
                }
            }
        }

        #endregion

        #region Save File


        public void SaveReportDialogue()
        {
            this.SaveDialog(SaveFileType.Save);
        }

        public void SaveAsReportDialogue()
        {
            this.SaveDialog(SaveFileType.SaveAs);
        }

        internal void SaveDialog(SaveFileType saveType)
        {
            ReportInformation reportInfo = this.GetReportInformationOnKey(this.GetDisplayTabKey());
            this.SaveDialog(saveType, reportInfo);
        }

        internal void SaveDialog(SaveFileType saveType, ReportInformation reportInfo)
        {
            if (string.IsNullOrEmpty(reportInfo.ReportPath) || saveType == SaveFileType.SaveAs)
            {
                ReportSaveDialog saveReportDialog = new ReportSaveDialog();
                saveReportDialog.Owner = Window.GetWindow(this);
                SkinStorage.SetVisualStyle(saveReportDialog, SkinStorage.GetVisualStyle(saveReportDialog.Owner));

                if (this.DesignMode == Designer.DesignMode.RDL)
                {
                    saveReportDialog.DialogMode = DialogMode.Default;
                    saveReportDialog.FileType = DialogFileType.RDL;
                    saveReportDialog.Title = "Save an RDL File";
                }
                else
                {
                    saveReportDialog.FileType = DialogFileType.RDLC;
                    saveReportDialog.Title = "Save an RDLC File";
                }

                if (saveReportDialog.ShowDialog() == true)
                {
                    if (string.IsNullOrEmpty(saveReportDialog.ReportServerURL))
                    {
                        this.SaveReport(saveReportDialog.FilePath);
                    }
                    else
                    {
                        this.SaveReport(saveReportDialog.FilePath, saveReportDialog.ReportServerURL, saveReportDialog.ReportServerCredential);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(reportInfo.ReportServer))
            {
                this.SaveReport(reportInfo);
            }
            else if (!string.IsNullOrEmpty(reportInfo.ReportPath))
            {
                this.SaveReport(reportInfo.ReportPath);
            }
        }

        internal bool SaveDialog(RDL.DOM.ReportDefinition report, ReportInformation reportInfo)
        {
            if (string.IsNullOrEmpty(reportInfo.ReportPath))
            {
                ReportSaveDialog saveReportDialog = new ReportSaveDialog();
                saveReportDialog.Owner = Window.GetWindow(this);
                SkinStorage.SetVisualStyle(saveReportDialog, SkinStorage.GetVisualStyle(saveReportDialog.Owner));

                if (this.DesignMode == Designer.DesignMode.RDL)
                {
                    saveReportDialog.FileType = DialogFileType.RDL;
                    saveReportDialog.Title = "Save an RDL File";
                }
                else
                {
                    saveReportDialog.FileType = DialogFileType.RDLC;
                    saveReportDialog.Title = "Save an RDLC File";
                }

                if (saveReportDialog.ShowDialog() == true)
                {
                    if (string.IsNullOrEmpty(saveReportDialog.ReportServerURL))
                    {
                        this.SaveReport(saveReportDialog.FilePath);
                    }
                    else
                    {
                        this.SaveReport(saveReportDialog.FilePath, saveReportDialog.ReportServerURL, saveReportDialog.ReportServerCredential);
                    }
                }

            }
            else if (!string.IsNullOrEmpty(reportInfo.ReportServer))
            {
                return this.SaveReport(reportInfo);
            }
            else if (!string.IsNullOrEmpty(reportInfo.ReportPath))
            {
                this.SaveReport(report, reportInfo);
                return true;
            }

            return true;
        }

        private bool SaveReport(ReportInformation reportInformation)
        {
            RDL.DOM.ReportDefinition report = this.GetReportSettingOnKey(reportInformation.Key);

            try
            {
                ReportModel model = new ReportModel();
                model.ReportPath = reportInformation.ReportPath;
                model.ReportServerUrl = reportInformation.ReportServer;
                model.ReportServerCredential = this.ReportServerCredential;
                model.ReportServerFormsCredential = this.ReportServerFormsCredential;
                model.ReportingServer.SetReportDefinition(this.GetStreamArray(report));
                this.SetDirtyReport(reportInformation.Key);
                this.SerializeRecentFiles(reportInformation);
                this.SetTabInformation(reportInformation.Key, reportInformation.ReportName);
                return true;
            }
            catch { }

            return false;
        }

        private void SaveReport(RDL.DOM.ReportDefinition report, ReportInformation reportInformation)
        {
            reportInformation.ReportName = System.IO.Path.GetFileName(reportInformation.ReportPath);
            this.SaveSerialize(reportInformation.ReportPath, report);
            this.SetDirtyReport(reportInformation.Key);
            this.SerializeRecentFiles(reportInformation);
            this.SetTabInformation(reportInformation.Key, reportInformation.ReportName);
            this.RaiseReportSave(reportInformation.ReportName);
        }

        public void SaveReport(string reportPath)
        {
            Syncfusion.RDL.DOM.ReportDefinition report = this.GetDisplayReportSettings();

            ReportInformation reportInformation = (from reportValue in this.ReportsCollection
                                                   where reportValue.Key == this.GetDisplayTabKey()
                                                   select reportValue).SingleOrDefault();

            reportInformation.ReportPath = reportPath;
            this.SaveReport(report, reportInformation);
        }


        //Save Server Report
        private void SaveReport(string reportPath, string reportServerUrl, System.Net.ICredentials credential)
        {
            Syncfusion.RDL.DOM.ReportDefinition report = this.GetDisplayReportSettings();

            ReportInformation reportInformation = (from reportValue in this.ReportsCollection
                                                   where reportValue.Key == this.GetDisplayTabKey()
                                                   select reportValue).SingleOrDefault();

            reportInformation.ReportPath = reportPath;
            reportInformation.ReportName = System.IO.Path.GetFileName(reportPath);
            reportInformation.ReportServer = reportServerUrl;
            reportInformation.ReportServerCredential = credential;
            this.CreateReport(reportInformation);
        }

        private void CreateReport(ReportInformation reportInformation)
        {
            RDL.DOM.ReportDefinition report = this.GetReportSettingOnKey(reportInformation.Key);

            try
            {
                ServerReportProcessor model = new ServerReportProcessor();
                model.ReportPath = reportInformation.ReportPath;
                model.ReportServerUrl = reportInformation.ReportServer;
                model.ReportServerCredential = reportInformation.ReportServerCredential;
                model.ReportServerFormsCredential = this.ReportServerFormsCredential;
                model.CreateReport(reportInformation.ReportName, GetParentFolder(reportInformation.ReportPath), this.GetStreamArray(report));
                this.SetDirtyReport(reportInformation.Key);
                this.SerializeRecentFiles(reportInformation);
                this.SetTabInformation(reportInformation.Key, reportInformation.ReportName);
                this.ReportServerUrl = reportInformation.ReportServer;
                this.ReportServerCredential = reportInformation.ReportServerCredential;
            }
            catch { }
        }

        //private get Parent Folder name
        private string GetParentFolder(string reportpath)
        {
            string tempfile = System.IO.Path.GetFileName(reportpath);
            int folderlength = reportpath.Length - tempfile.Length;
            tempfile = reportpath.Remove(folderlength - 1, tempfile.Length + 1);
            if (string.IsNullOrEmpty(tempfile))
            {
                return "/";
            }
            return tempfile;
        }

        public void SaveReport(Stream stream)
        {
            Syncfusion.RDL.DOM.ReportDefinition report = this.GetDisplayReportSettings();
            byte[] reportByte = this.GetStreamArray(report);
            stream.Write(reportByte, 0, reportByte.Length);
        }

        public Stream GetReportStream()
        {
            Syncfusion.RDL.DOM.ReportDefinition report = this.GetDisplayReportSettings();
            string str = Encoding.ASCII.GetString(this.GetStreamArray(report));
            byte[] byteArray = Encoding.ASCII.GetBytes(str);
            MemoryStream stream = new MemoryStream(byteArray);
            return stream;
        }

        private void SaveSerialize(string reportPath, Syncfusion.RDL.DOM.ReportDefinition report)
        {
            string nameSpace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";

            if (report.RDLType == RDL.DOM.RDLType.RDL2010)
            {
                nameSpace = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";
            }

            XmlSerializer xs2 = new XmlSerializer(typeof(Syncfusion.RDL.DOM.ReportDefinition), nameSpace);
            System.Xml.Serialization.XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
            xs.Add("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
            TextWriter ws2 = new StreamWriter(reportPath);
            xs2.Serialize(ws2, report, xs);
            ws2.Close();
        }

        private MemoryStream GetStream(Syncfusion.RDL.DOM.ReportDefinition report)
        {
            string nameSpace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";

            if (report.RDLType == RDL.DOM.RDLType.RDL2010)
            {
                nameSpace = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";
            }

            System.Xml.Serialization.XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
            xs.Add("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
            XmlSerializer xs2 = new XmlSerializer(typeof(Syncfusion.RDL.DOM.ReportDefinition), nameSpace);
            MemoryStream memoryStream = new MemoryStream();
            TextWriter ws2 = new StreamWriter(memoryStream);
            xs2.Serialize(ws2, report, xs);
            return memoryStream;
        }

        private byte[] GetStreamArray(Syncfusion.RDL.DOM.ReportDefinition report)
        {
            MemoryStream stream = this.GetStream(report);
            return stream.ToArray();
        }

        #endregion

        #region Open File

        public void OpenReportDialog()
        {
            this.OpenDialogInner();
        }

        private void OpenDialogInner()
        {
            ReportOpenDialog opendialog = new ReportOpenDialog();
            opendialog.Owner = Window.GetWindow(this);
            SkinStorage.SetVisualStyle(opendialog, SkinStorage.GetVisualStyle(opendialog.Owner));
            if (this.DesignMode == Designer.DesignMode.RDL)
            {
                opendialog.DialogMode = DialogMode.Default;
                opendialog.FileType = DialogFileType.RDL;
                opendialog.Title = "Open an RDL File";
            }
            else
            {
                opendialog.DialogMode = DialogMode.Local;
                opendialog.FileType = DialogFileType.RDLC;
                opendialog.Title = "Open an RDLC File";
            }
            if (opendialog.ShowDialog() == true)
            {
                string fileUrl = opendialog.FilePath;
                if (File.Exists(fileUrl))
                {
                    OpenReport(fileUrl);
                }
                else
                {
                    this.ReportServerUrl = opendialog.ReportServerURL;
                    this.ReportServerCredential = opendialog.ReportServerCredential;
                    OpenReport(fileUrl);
                }
                #region Serialize Recent Files List
                this.SerializeRecentFiles(fileUrl);
                #endregion
            }
        }

        RDL.DOM.ReportDefinition LoadReport(ReportInfo reportInfo)
        {
            if (!string.IsNullOrEmpty(reportInfo.ReportPath) && File.Exists(reportInfo.ReportPath))
            {
                string path = System.IO.Path.GetFullPath(reportInfo.ReportPath);
                using (FileStream stream = new FileStream(reportInfo.ReportPath, FileMode.Open))
                {
                    return LoadReport(stream);
                }
            }
            else if (!string.IsNullOrEmpty(reportInfo.ReportPath) && (this.ReportServerCredential != null || this.ReportServerFormsCredential != null) && !string.IsNullOrEmpty(this.ReportServerUrl))
            {
                try
                {
                    if (this.ReportServerProcessor == null)
                    {
                        this.ReportServerProcessor = new ServerReportProcessor();
                    }
                    if (reportInfo.ReportServer == null || reportInfo.ReportServer.Equals(this.ReportServerUrl))
                    {
                        this.ReportServerProcessor.ReportServerCredential = this.ReportServerCredential;
                        this.ReportServerProcessor.ReportServerFormsCredential = this.ReportServerFormsCredential;
                        this.ReportServerProcessor.ReportServerUrl = this.ReportServerUrl;
                        reportInfo.ReportServer = this.ReportServerUrl;
                        reportInfo.ReportServerCredential = this.ReportServerCredential;
                    }
                    else
                    {
                        //this.ReportModel.ReportServerCredential = this.ReportServerCredential;
                        //this.ReportModel.ReportServerFormsCredential = this.ReportServerFormsCredential;
                        this.ReportServerProcessor.ReportServerUrl = reportInfo.ReportServer;
                    }

                    this.ReportServerProcessor.ReportPath = reportInfo.ReportPath;
                    string exception = null;

                    RDL.DOM.ReportDefinition report = LoadReport(this.ReportServerProcessor.GetReportDefinition(out exception));
                    if (!string.IsNullOrEmpty(exception))
                    {
                        throw new Exception(exception);
                    }
                    return report;
                }
                catch { }
            }

            return null;
        }

        Syncfusion.RDL.DOM.ReportDefinition LoadReport(Stream reportStream)
        {
            XElement rdl = XElement.Load(XmlReader.Create(reportStream));
            string Namespace = (from attribute in rdl.Attributes() where attribute.Name.LocalName == "xmlns" select attribute.Value).FirstOrDefault();
            string Version = (Regex.IsMatch(Namespace, @"\d{4}") ? Regex.Match(Namespace, @"\d{4}").Value : string.Empty);

            if (!string.IsNullOrEmpty(Version))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Syncfusion.RDL.DOM.ReportDefinition), Namespace);//"http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"                
                using (StringReader reader = new StringReader(rdl.ToString()))
                {
                    Syncfusion.RDL.DOM.ReportDefinition o = (Syncfusion.RDL.DOM.ReportDefinition)xs.Deserialize(reader);
                    if (o != null)
                    {
                        switch (Version)
                        {
                            case "2008":
                                o.RDLType = Syncfusion.RDL.DOM.RDLType.RDL2008;
                                this.ReportFormat = Designer.ReportFormat.RDL2008;
                                break;
                            case "2010":
                                o.RDLType = Syncfusion.RDL.DOM.RDLType.RDL2010;
                                this.ReportFormat = Designer.ReportFormat.RDL2010;
                                break;
                            default:
                                o.RDLType = Syncfusion.RDL.DOM.RDLType.None;
                                break;
                        }
                        return o;
                    }
                }
            }

            return null;
        }

        public void OpenReport(string reportPath)
        {
            ReportInfo reportInfo = new ReportInfo();
            reportInfo.ReportPath = reportPath;
            this.OpenReport(reportInfo);
            this.RaiseOpenReportChanged(System.IO.Path.GetFileName(reportPath));
        }

        internal void OpenReport(string reportPath, string reportServer)
        {
            this.ReportServerUrl = reportServer;
            if (this.ReportServerCredential == null)
            {
                LoginCredentials login = new LoginCredentials(reportServer);
                login.Owner = Window.GetWindow(this);
                SkinStorage.SetVisualStyle(login, SkinStorage.GetVisualStyle(login.Owner));
                if (login.ShowDialog() == true)
                {
                    this.ReportServerCredential = new NetworkCredential(login.Username, login.Password);
                }
            }
            OpenReport(reportPath);
        }

        private void OpenReport(ReportInfo reportInfo)
        {
            Syncfusion.RDL.DOM.ReportDefinition openReport = this.LoadReport(reportInfo);

            if (openReport != null)
            {
                string reportName = string.Empty;
                var existingKey = (from reportInformation in this.ReportsCollection
                                   where (reportInformation.ReportPath == reportInfo.ReportPath
                                          && ((reportInformation.ReportServer == null && reportInfo.ReportServer == null) || (reportInformation.ReportServer.Equals(reportInfo.ReportServer))))
                                   select reportInformation.Key).SingleOrDefault();

                if (existingKey == 0)
                {
                    try
                    {
                        double reportKey = this.PopulateReportsCollection(reportInfo, openReport);
                        ReportInformation information = this.GetReportInformationOnKey(reportKey);
                        this.NewTabSplitterItem(information.ReportName, reportKey);
                        information.DirtyReport = this.GetDirtyReport(openReport);
                        this.SerializeRecentFiles(information);
                        this.SetTabInformation(information.Key, information.ReportName);
                    }
                    catch (Exception excep)
                    {
                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxFile") + reportName + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxFailed") + "\n" + RESX.msgBoxDueTo + excep.Message.ToString(), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
                    }
                }
                else
                {
                    string reportContent = (from info in this.ReportsCollection
                                            where info.Key == existingKey
                                            select info.DirtyReport).First();
                    string newcontent = this.GetDirtyReport(openReport);

                    if (!reportContent.Equals(newcontent))
                    {
                        ReportInformation information = this.GetModifyReportsCollection(reportInfo, openReport, existingKey);
                        TabItemExt tabItem = this.GetDisplayTabItem(existingKey);
                        this.ModifyOpenReport(existingKey, tabItem);
                        this.SerializeRecentFiles(information);
                    }

                    this.FocusTab(existingKey);
                }
            }
            else
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotFindFile") + reportInfo.ReportPath + "'.", SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
            }
        }

        private ReportInformation GetModifyReportsCollection(ReportInfo reportInfo, RDL.DOM.ReportDefinition report, double reportKey)
        {
            try
            {
                ReportInformation reportInformation = new ReportInformation();
                reportInformation.Report = report;
                reportInformation.Key = reportKey;

                if (reportInfo != null)
                {
                    reportInformation.ReportName = System.IO.Path.GetFileName(reportInfo.ReportPath);
                    reportInformation.ReportPath = reportInfo.ReportPath;
                    reportInformation.ReportServer = reportInfo.ReportServer;
                    reportInformation.ReportServerCredential = reportInfo.ReportServerCredential;
                }
                for (int i = 0; i < this.ReportsCollection.Count(); i++)
                {
                    if (this.ReportsCollection[i].Key == reportKey)
                    {
                        this.ReportsCollection[i] = reportInformation;
                        return this.ReportsCollection[i];
                    }
                }
                if (!EnableMDIDesigner)
                {
                    this.ReportsCollection.Clear();
                    this.ReportsCollection.Add(reportInformation);
                }

                return reportInformation;
            }
            catch 
            {
                return null;
            }
        }

        private Syncfusion.Windows.Tools.Controls.TabItemExt GetDisplayTabItem(double reportKey)
        {
            if (EnableMDIDesigner)
            {
                foreach (Syncfusion.Windows.Tools.Controls.TabItemExt tabItem in this.TabControl.Items)
                {
                    if (tabItem.Tag.Equals(reportKey))
                    {
                        return tabItem;
                    }
                }
            }
            return null;
        }

        private void ModifyOpenReport(double reportKey, TabItemExt tabItem)
        {
            try
            {
                RDLDesignerControl rdlViewControl;
                if (EnableMDIDesigner)
                {
                    rdlViewControl = tabItem.Content as RDLDesignerControl;
                }
                else
                {
                    rdlViewControl = this.SingleDocument.Content as RDLDesignerControl;
                }

                DesignPanel panel = this.GetDesignPanel(reportKey);
                CurrentPanel = panel;
                rdlViewControl.DesignControl = panel;

                Binding rdlbinding = new Binding();
                rdlbinding.Source = this;
                rdlbinding.Path = new PropertyPath("IsRdlVisible");
                rdlbinding.Mode = BindingMode.OneWay;
                rdlViewControl.SetBinding(RDLDesignerControl.IsRdlVisibleProperty, rdlbinding);

                Binding rulerbinding = new Binding();
                rulerbinding.Source = this;
                rulerbinding.Path = new PropertyPath("ShowRuler");
                rulerbinding.Mode = BindingMode.OneWay;
                panel.SetBinding(DesignPanel.IsRulerVisibleProperty, rulerbinding);

                Binding rulerStylebinding = new Binding();
                rulerStylebinding.Source = this;
                rulerStylebinding.Path = new PropertyPath("VisualStyle");
                rulerStylebinding.Mode = BindingMode.OneWay;
                panel.SetBinding(DesignPanel.VisualStyleProperty, rulerStylebinding);

                Binding helpbinding = new Binding();
                helpbinding.Source = this;
                helpbinding.Path = new PropertyPath("ShowHelp");
                helpbinding.Mode = BindingMode.OneWay;
                panel.SetBinding(DesignPanel.ShowHelpProperty, helpbinding);

                Binding zoombinding = new Binding();
                zoombinding.Source = this;
                zoombinding.Path = new PropertyPath("ZoomFactor");
                zoombinding.Mode = BindingMode.OneWay;
                panel.SetBinding(DesignPanel.ZoomFactorProperty, zoombinding);

                panel.ReportReportItemDrawn += (sen, arg) =>
                       {
                           this.DrawingReportItem = Designer.DrawingReportItem.None;
                           this.RaiseReportItemDrawnEvent(this, arg);
                           this.ToggleItemsUpdate(panel.reportItems);
                       };

                panel.ReportEmbeddedImageCollectionModified += (sen, arg) =>
                {
                    this.EmbeddedImagesUpdate(panel.EmbeddedImages);
                };

                panel.ReportDataSourceCollectionModified += (sen, arg) =>
                {
                    this.DataSourcesUpdate(panel.DataSources);
                };

                panel.ReportDataSetCollectionModified += (sen, arg) =>
                {
                    this.DataSetsUpdate(panel.DataSets);
                };

                panel.ReportDataSetFieldCollectionModified += (sen, arg) =>
                {
                    this.DataSetsUpdate(panel.DataSets);
                };

                panel.ReportParameterCollectionModified += (sen, arg) =>
                {
                    this.ParametersUpdate(panel.ReportParameters);
                };

                panel.ReportItemSelected += (sen, arg) =>
                {
                    if (this.ShowProperties)
                    {
                        this.UpdatePropertyObject(arg.SelectedItem);

                        if (arg.SelectedItem is IReportItemProperties)
                        {
                            this.ToggleItemsUpdate(panel.reportItems);
                        }
                    }
                    else
                    {
                        selectedObject = arg.SelectedItem;
                    }
                    if (this.ShowGrouping && sen is TablixControl)
                    {
                        this.UpdateGroupingObject(sen);
                    }

                };

                this.UpdateDesignPanel(panel);
                this.UpdateDesignerToolboxObj();

                if (!this.EnableMDIDesigner)
                {
                    this.DataSourcesUpdate(panel.DataSources);
                    this.EmbeddedImagesUpdate(panel.EmbeddedImages);
                    this.ParametersUpdate(panel.ReportParameters);
                    this.DataSetsUpdate(panel.DataSets);
                    this.ToggleItemsUpdate(panel.reportItems);
                }

                if (EnableMDIDesigner == false)
                {
                    this.TabControl.Items.Clear();
                }

                if (EnableMDIDesigner)
                {
                    tabItem.Content = rdlViewControl;
                    panel.Tag = reportKey;
                    this.TabControl.SelectedItem = tabItem;
                }
                else
                {
                    this.SingleDocument.Content = rdlViewControl;
                    this.SingleDocument.Tag = reportKey;
                }
            }
            catch { }
        }

        private void SetDirtyReport(double Key)
        {
            ReportInformation reportInformation = this.GetReportInformationOnKey(Key);

            try
            {
                Syncfusion.RDL.DOM.ReportDefinition report = this.GetReportSettingOnKey(Key);
                reportInformation.DirtyReport = this.GetDirtyReport(report);
            }
            catch (Exception)
            {
                //MessageBox.Show("Error in maintaining Dirty State");
            }
        }

        internal bool CloseApplication(TabItemExt excludeTab)
        {
            try
            {
                Dictionary<ReportInformation, RDL.DOM.ReportDefinition> unSavedReports = new Dictionary<ReportInformation, RDL.DOM.ReportDefinition>();

                string alertString = string.Empty;

                if (EnableMDIDesigner)
                {
                    foreach (TabItemExt tabItem in this.TabControl.Items)
                    {
                        if (tabItem != excludeTab)
                        {
                            double reportKey = Convert.ToDouble(tabItem.Tag);

                            var reportInformation = (from reports in this.ReportsCollection
                                                     where reports.Key == reportKey
                                                     select reports).SingleOrDefault();

                            Syncfusion.RDL.DOM.ReportDefinition report = this.GetRenderedReportSettings(tabItem);
                            string reportSettingsString = this.GetDirtyReport(report);

                            if (!reportInformation.DirtyReport.Equals(reportSettingsString))
                            {
                                alertString += reportInformation.ReportName + Environment.NewLine;
                                unSavedReports.Add(reportInformation, report);
                            }
                        }
                    }
                }
                else
                {
                    double reportKey = Convert.ToDouble(this.SingleDocument.Tag);

                    var reportInformation = (from reports in this.ReportsCollection
                                             where reports.Key == reportKey
                                             select reports).SingleOrDefault();


                    Syncfusion.RDL.DOM.ReportDefinition report = this.GetRenderedReportSettings(this.SingleDocument);
                    string reportSettingsString = this.GetDirtyReport(report);

                    if (!reportInformation.DirtyReport.Equals(reportSettingsString))
                    {
                        alertString += reportInformation.ReportName + Environment.NewLine;
                        unSavedReports.Add(reportInformation, report);
                    }
                }

                if (unSavedReports.Count > 0)
                {
                    string alertInitialString = SR.GetString(CultureInfo.CurrentUICulture, "msgBoxalterString");
                    string alertMessage = alertInitialString + Environment.NewLine + Environment.NewLine + alertString;
                    MessageBoxResult result = MessageBox.Show(alertMessage, SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.YesNoCancel);

                    if (result == MessageBoxResult.Yes)
                    {
                        foreach (var reportInfo in unSavedReports.Keys)
                        {
                            RDL.DOM.ReportDefinition report = unSavedReports[reportInfo];
                            bool isSaved = this.SaveDialog(report, reportInfo);

                            if (isSaved == false)
                            {
                                return false;
                            }
                        }
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        return true;
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message.ToString());
            }

            return true;
        }

        internal void UpdatePropertyObject(object selectedObject)
        {
            foreach (var propertyGrid in this.ReportPropertyGrids)
            {
                if (propertyGrid.SelectedItem != selectedObject)
                {
                    propertyGrid.SelectedItem = selectedObject;
                }
            }
        }

        private void UpdateGroupingObject(object selectedObject)
        {
            if (this.TablixGroupingPanels != null)
            {
                foreach (var groupPanel in this.TablixGroupingPanels)
                {
                    if (groupPanel.SelectedReportItem != selectedObject)
                    {
                        groupPanel.SelectedReportItem = selectedObject;
                    }
                }
            }
        }

        internal void UpdateVisualStyle()
        {
            foreach (var reportData in this.ReportDatas)
            {
                reportData.UpdateTreeStyle(this.VisualStyle);
            }

            foreach (var reporttoolbox in this.ReportToolBoxs)
            {
                reporttoolbox.VisualStyle= this.VisualStyle;
            }

            foreach (var groupPanel in this.TablixGroupingPanels)
            {
                groupPanel.VisualStyle = this.VisualStyle;
            }
        }

        internal void UpdateDesignPanel(DesignPanel designPanel)
        {
            foreach (var reportData in this.ReportDatas)
            {
                reportData.DesignPanel = designPanel;
            }
        }

        internal void UpdateDesignerToolboxObj()
        {
            foreach (var reportToolbox in this.ReportToolBoxs)
            {
                reportToolbox.UpdateReportDesignerObject(this);
            }
        }

        internal void DataSetsUpdate(Syncfusion.RDL.DOM.DataSets reportDataSets)
        {
            this.DataSetNames.Clear();
            this.DataSetFields.Clear();
            foreach (Syncfusion.RDL.DOM.DataSet reportDataSet in reportDataSets)
            {
                this.DataSetNames.Add(reportDataSet.Name);

                foreach (var field in reportDataSet.Fields)
                {
                    this.DataSetFields.Add("=First(Fields!" + field.Name + ".Value,\"" + reportDataSet.Name + "\")");
                }
            }

            foreach (var reportData in this.ReportDatas)
            {
                reportData.DataSetsUpdate(reportDataSets);
            }
        }

        internal void DataSourcesUpdate(Syncfusion.RDL.DOM.DataSources reportDataSources)
        {
            foreach (var reportData in this.ReportDatas)
            {
                reportData.DataSourcesUpdate(reportDataSources);
            }
        }

        internal void EmbeddedImagesUpdate(Syncfusion.RDL.DOM.EmbeddedImages embeddedImages)
        {
            this.embeddedImages.Clear();
            foreach (Syncfusion.RDL.DOM.EmbeddedImage reportembeddedimage in embeddedImages)
            {
                this.embeddedImages.Add(reportembeddedimage.Name);
            }

            foreach (var reportData in this.ReportDatas)
            {
                reportData.EmbeddedImagesUpdate(embeddedImages);
            }
        }

        internal void ParametersUpdate(Syncfusion.RDL.DOM.ReportParameters reportParameters)
        {
            foreach (var reportData in this.ReportDatas)
            {
                reportData.ParametersUpdate(reportParameters);
            }
        }

        internal void ToggleItemsUpdate(List<IReportItemControl> reportItems)
        {
            this.toggleItems.Clear();

            foreach (var item in reportItems)
            {
                if (item is TextBoxControl && item.Parent.Name == "BodyCanvas")
                {
                    this.toggleItems.Add(item.ItemName);
                }
                else if (item is TablixControl)
                {
                    try
                    {
                        List<UIElement> list = (item as TablixControl).Children.OfType<UIElement>().ToList();                        
                        List <CellContentsControl> cells = list.OfType<CellContentsControl>().ToList();
                        foreach (var cell in cells)
                        {
                            if (cell.Content is TextBoxControl)
                            {
                                toggleItems.Add((cell.Content as TextBoxControl).Name);
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        internal void UpdateOwnerWindow(Window window)
        {
            if (ownerWindow == null)
            {
                ownerWindow = Window.GetWindow(this);
            }

            if (ownerWindow != null)
            {
                window.Owner = (Window)ownerWindow;
                SkinStorage.SetVisualStyle(window, SkinStorage.GetVisualStyle(this));
            }
        }

        #endregion

        #region Recent_Files_List_serialize_and_deserialize

        internal IOrderedEnumerable<RecentReport> RecentFilesSort()
        {
            ////Deleting Recent Files Link, whose index exceeds the list limit
            if (this.recentReports.Count > recentReportsLimit)
            {
                this.recentReports.RemoveRange(recentReportsLimit - 1, 1);
            }

            var recentFilesList = from file in this.recentReports
                                  orderby file.LastModified descending
                                  select file;

            return recentFilesList;
        }

        private void SerializeRecentFiles(string fileUrl)
        {
            if (this.recentReports != null)
            {
                var recentFileVar = (from file in this.recentReports
                                     where file.ReportPath == fileUrl
                                     select file).FirstOrDefault();

                if (recentFileVar == null)
                {
                    RecentReport recentFile = new RecentReport();
                    recentFile.ReportPath = fileUrl;
                    recentFile.LastModified = System.DateTime.Now;
                    string fileName = System.IO.Path.GetFileName(fileUrl);
                    this.recentReports.Add(recentFile);
                }
                else
                {
                    recentFileVar.LastModified = System.DateTime.Now;
                }

                this.RaiseSerializeEventhandler(this.RecentFilesSort());

                XmlSerializer recentXS = new XmlSerializer(typeof(RecentReports));
                TextWriter recentWS = new StreamWriter(this.recentReportsUrl);
                recentXS.Serialize(recentWS, this.recentReports);
                recentWS.Close();
            }
        }

        private void SerializeRecentFiles(ReportInformation reportInfo)
        {
            if (this.recentReports != null)
            {
                var recentFileVar = (from file in this.recentReports
                                     where file.ReportPath == reportInfo.ReportPath
                                     select file).FirstOrDefault();

                if (recentFileVar == null)
                {
                    RecentReport recentFile = new RecentReport();
                    recentFile.ReportPath = reportInfo.ReportPath;
                    recentFile.ReportServer = reportInfo.ReportServer;
                    recentFile.LastModified = System.DateTime.Now;
                    this.recentReports.Add(recentFile);
                }
                else
                {
                    recentFileVar.LastModified = System.DateTime.Now;
                }

                this.RaiseSerializeEventhandler(this.RecentFilesSort());
                XmlSerializer recentXS = new XmlSerializer(typeof(RecentReports));
                TextWriter recentWS = new StreamWriter(recentReportsUrl);
                recentXS.Serialize(recentWS, this.recentReports);
                recentWS.Close();
            }
        }

        internal void DeserializeRecentFiles()
        {
            #region DeSerialize Recent Files

            if (File.Exists(this.recentReportsUrl))
            {
                try
                {
                    XmlSerializer xs2 = new XmlSerializer(typeof(RecentReports));
                    TextReader r = new StreamReader(this.recentReportsUrl);
                    this.recentReports = (RecentReports)xs2.Deserialize(r);
                    r.Close();

                    if (this.recentReports.Count > 0)
                    {
                        this.RaiseSerializeEventhandler(this.RecentFilesSort());
                    }
                }
                catch
                {
                    this.recentReports = new RecentReports();
                }
            }
            else
            {
                this.recentReports = new RecentReports();
            }
            #endregion
        }
        #endregion

        #region reportviewer & designer

        internal void AddReportData(ReportDataExplorer reportData)
        {
            this.ReportDatas.Add(reportData);
            reportData.DesignerMode = this.DesignMode;
        }

        internal void RemoveReportData(ReportDataExplorer reportData)
        {
            this.ReportDatas.Remove(reportData);
        }

        internal void AddReportProperty(ReportPropertyGrid propertyGrid)
        {
            this.ReportPropertyGrids.Add(propertyGrid);
            propertyGrid.UpdateReportDesignerObject(this);
        }

        internal void RemoveReportProperty(ReportPropertyGrid propertyGrid)
        {
            this.ReportPropertyGrids.Remove(propertyGrid);
        }

        internal void AddToolBox(ReportToolBox reportToolbox)
        {
            this.ReportToolBoxs.Add(reportToolbox);
            reportToolbox.UpdateReportDesignerObject(this);
        }

        internal void RemoveToolbox(ReportToolBox reportToolbox)
        {
            this.ReportToolBoxs.Remove(reportToolbox);
        }

        internal void AddGrouping(TablixGroupingPanel tablixGrouping)
        {
            this.TablixGroupingPanels.Add(tablixGrouping);
            tablixGrouping.UpdateReportDesignerObject(this);
        }

        internal void RemoveGrouping(TablixGroupingPanel tablixGrouping)
        {
            this.TablixGroupingPanels.Remove(tablixGrouping);
        }

        public string AddDataSet(string reportPath, string reportDataSetName)
        {
            ReportInfo reportInfo = new ReportInfo();
            reportInfo.ReportPath = reportPath;
            Syncfusion.RDL.DOM.ReportDefinition reportDefinition = this.LoadReport(reportInfo);
            return this.AddDataSet(reportDefinition, reportDataSetName);
        }

        public string AddDataSet(Stream reportStream, string reportDataSetName)
        {
            Syncfusion.RDL.DOM.ReportDefinition reportDefinition = this.LoadReport(reportStream);
            return this.AddDataSet(reportDefinition, reportDataSetName);
        }

        private string AddDataSet(Syncfusion.RDL.DOM.ReportDefinition reportDefinition, string reportDataSetName)
        {
            if (reportDefinition != null)
            {
                var designPanel = this.GetDisplayTabDesignPanel();

                foreach (var dataSource in reportDefinition.DataSources)
                {
                    var dataSet = (from set in reportDefinition.DataSets
                                   where set.Name.Equals(reportDataSetName)
                                   select set).First();

                    if (dataSet != null)
                    {
                        int addCount = 0;
                        int availableCount = 0;
                        var tempName = dataSource.Name;
                        bool valid = false;

                        do
                        {
                            var source = (from sourceName in designPanel.DataSources
                                          where sourceName.Name.Equals(dataSource.Name)
                                          select sourceName).FirstOrDefault();
                            if (source != null)
                            {
                                if (!source.ConnectionProperties.ConnectString.Equals(dataSource.ConnectionProperties.ConnectString))
                                {
                                    dataSource.Name = tempName + (++addCount);
                                    availableCount = 1;
                                }
                            }
                            else
                            {
                                valid = true;
                            }
                        } while (availableCount > 0);

                        if (valid)
                        {
                            designPanel.DataSources.Add(dataSource);
                            designPanel.RaiseDataSourceCollectionModifiedEvent();
                        }

                        addCount = 0;
                        availableCount = 0;
                        tempName = dataSet.Name;

                        do
                        {
                            availableCount = (from setName in designPanel.DataSets
                                              where setName.Name.Equals(dataSet.Name)
                                              select setName).Count();

                            if (availableCount > 0)
                            {
                                dataSet.Name = tempName + (++addCount);
                            }
                        } while (availableCount > 0);

                        if (dataSet.Query.QueryParameters != null && dataSet.Query.QueryParameters.Count > 0)
                        {
                            foreach (var param in dataSet.Query.QueryParameters)
                            {
                                Syncfusion.RDL.DOM.ReportParameter reportParam = new Syncfusion.RDL.DOM.ReportParameter();
                                reportParam.Name = param.Name.Trim('@', ' ', ':');
                                reportParam.Prompt = param.Name.Trim('@', ' ', ':');
                                if (designPanel.ReportParameters != null)
                                {
                                    valid = (from parameter in designPanel.ReportParameters
                                             where parameter.Name.Equals(reportParam.Name) && parameter.Prompt.Equals(reportParam.Prompt)
                                             select parameter).Count() > 0 ? false : true;
                                    if (valid)
                                        designPanel.AddReportParameter(reportParam);
                                }
                                else
                                {
                                    designPanel.AddReportParameter(reportParam);
                                }
                            }
                        }

                        designPanel.DataSets.Add(dataSet);
                        designPanel.RaiseDataSetCollectionModifiedEvent();
                        designPanel.RaiseParameterCollectionModifiedEvent();

                        return dataSet.Name;
                    }
                }
            }

            return null;
        }

        public string AddDataSource(string reportPath, string reportDataSourceName)
        {
            ReportInfo reportInfo = new ReportInfo();
            reportInfo.ReportPath = reportPath;
            Syncfusion.RDL.DOM.ReportDefinition reportDefinition = this.LoadReport(reportInfo);
            return this.AddDataSource(reportDefinition, reportDataSourceName);
        }

        public string AddDataSource(Stream reportStream, string reportDataSourceName)
        {
            Syncfusion.RDL.DOM.ReportDefinition reportDefinition = this.LoadReport(reportStream);
            return this.AddDataSource(reportDefinition, reportDataSourceName);
        }

        private string AddDataSource(Syncfusion.RDL.DOM.ReportDefinition reportDefinition, string reportDataSourceName)
        {
            if (reportDefinition != null)
            {
                var dataSource = (from source in reportDefinition.DataSources
                                  where source.Name.Equals(reportDataSourceName)
                                  select source).First();
                var designPanel = this.GetDisplayTabDesignPanel();

                if ((dataSource != null))
                {
                    int addCount = 0;
                    int availableCount = 0;
                    var tempName = dataSource.Name;
                    bool valid = false;

                    do
                    {
                        var source = (from sourceName in designPanel.DataSources
                                      where sourceName.Name.Equals(dataSource.Name)
                                      select sourceName).FirstOrDefault();
                        if (source != null)
                        {
                            if (!source.ConnectionProperties.ConnectString.Equals(dataSource.ConnectionProperties.ConnectString))
                            {
                                dataSource.Name = tempName + (++addCount);
                                availableCount = 1;
                            }
                        }
                        else
                        {
                            valid = true;
                        }

                    } while (availableCount > 0);

                    if (valid)
                    {
                        designPanel.DataSources.Add(dataSource);
                        designPanel.RaiseDataSourceCollectionModifiedEvent();
                    }

                    foreach (var set in reportDefinition.DataSets)
                    {
                        addCount = 0;
                        availableCount = 0;
                        tempName = set.Name;

                        do
                        {
                            availableCount = (from setName in designPanel.DataSets
                                              where setName.Name.Equals(set.Name)
                                              select setName).Count();

                            if (availableCount > 0)
                            {
                                set.Name = tempName + (++addCount);
                            }
                        } while (availableCount > 0);

                        designPanel.DataSets.Add(set);

                        if (set.Query.QueryParameters != null && set.Query.QueryParameters.Count > 0)
                        {
                            foreach (var param in set.Query.QueryParameters)
                            {
                                Syncfusion.RDL.DOM.ReportParameter reportParam = new Syncfusion.RDL.DOM.ReportParameter();
                                reportParam.Name = param.Name.Trim('@', ' ', ':');
                                reportParam.Prompt = param.Name.Trim('@', ' ', ':');
                                if (designPanel.ReportParameters != null)
                                {
                                    valid = (from parameter in designPanel.ReportParameters
                                             where parameter.Name.Equals(reportParam.Name) && parameter.Prompt.Equals(reportParam.Prompt)
                                             select parameter).Count() > 0 ? false : true;
                                    if (valid)
                                        designPanel.AddReportParameter(reportParam);
                                }
                                else
                                {
                                    designPanel.AddReportParameter(reportParam);
                                }
                            }
                        }
                    }

                    designPanel.RaiseDataSetCollectionModifiedEvent();
                    designPanel.RaiseParameterCollectionModifiedEvent();

                    return dataSource.Name;
                }
            }

            return null;
        }

        internal void OpenViewer()
        {
            Syncfusion.RDL.DOM.ReportDefinition reportSettings = this.GetDisplayReportSettings(); // GetReportSettingsOnFocus()

            Syncfusion.RDL.DOM.DataSets tempDatasets = null;
            Syncfusion.RDL.DOM.DataSources tempDataSources = null;
            Syncfusion.RDL.DOM.ReportParameters tempParam = null;

            if (reportSettings.DataSets != null)
            {
                tempDatasets = new RDL.DOM.DataSets();

                foreach (var dataset in reportSettings.DataSets)
                {
                    tempDatasets.Add(dataset.Clone() as RDL.DOM.DataSet);
                }
            }
            if (reportSettings.DataSources != null)
            {
                tempDataSources = new RDL.DOM.DataSources();

                foreach (var datasource in reportSettings.DataSources)
                {
                    tempDataSources.Add(datasource.Clone() as RDL.DOM.DataSource);
                }
            }
            if (reportSettings.ReportParameters != null)
            {
                tempParam = new RDL.DOM.ReportParameters();
                foreach (var param in reportSettings.ReportParameters)
                {
                    tempParam.Add(param.Clone() as RDL.DOM.ReportParameter);
                }
            }

            this.GridViewerPanel.Visibility = Visibility.Visible;
            this.MDIDocument.Visibility = System.Windows.Visibility.Collapsed;
            this.SingleDocument.Visibility = System.Windows.Visibility.Collapsed;

            try
            {
                this.reportViewerControl.Reset();
                this.reportViewerControl.ViewMode = ViewMode.Normal;
                if (this.DesignMode == DesignMode.RDLC)
                {
                    this.reportViewerControl.ProcessingMode = Syncfusion.Windows.Reports.Viewer.ProcessingMode.Local;
                    this.reportViewerControl.LoadReport(reportSettings);
                    this.reportViewerControl.ShowLoadingIndicator();
                    IList<string> dataSets = this.reportViewerControl.GetDataSetNames();
                    this.reportViewerControl.DataSources.Clear();

                    if (tempDatasets != null && tempDatasets.Count > 0)
                    {
                        RdlcDataSourceUI rdlcDatasourceObject = new RdlcDataSourceUI(dataSets,tempDatasets, this.Assemblies);
                        this.UpdateOwnerWindow(rdlcDatasourceObject);

                        if (rdlcDatasourceObject.ShowDialog() == true)
                        {
                            for (int dataCount = 0; dataCount < tempDatasets.Count; dataCount++)
                            {
                                this.reportViewerControl.DataSources.Add(new Syncfusion.Windows.Reports.ReportDataSource
                                {
                                    Name = dataSets[dataCount],
                                    Value = rdlcDatasourceObject.SelectedObjects[dataCount]
                                });
                            }
                        }
                    }
                    this.reportViewerControl.RefreshReport();
                }
                else
                {
                    this.reportViewerControl.ProcessingMode = Syncfusion.Windows.Reports.Viewer.ProcessingMode.Remote;
                    this.reportViewerControl.LoadReport(reportSettings);
                    this.reportViewerControl.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                this.reportViewerControl.ShowException(ex);
            }

            var panel = this.GetDisplayTabDesignPanel();
            panel.DataSets = tempDatasets;
            panel.DataSources = tempDataSources;
            panel.ReportParameters = tempParam;
        }

        internal void OpenDesigner()
        {
            this.GridViewerPanel.Visibility = Visibility.Collapsed;

            if (this.EnableMDIDesigner)
            {
                this.MDIDocument.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.SingleDocument.Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion

        #region tab_events

        void TabControl_OnCloseButtonClick(object sender, CloseTabEventArgs e)
        {
            try
            {
                double ReportCount = (from reportInformation in this.ReportsCollection
                                      where reportInformation.Key == Convert.ToDouble(e.TargetTabItem.Tag)
                                      select reportInformation).Count();

                if (ReportCount > 0 && e.TargetTabItem.IsLoaded == true)
                {
                    ReportInformation reportInformation = this.GetReportInformationOnKey(Convert.ToDouble(e.TargetTabItem.Tag));

                    Syncfusion.RDL.DOM.ReportDefinition report = this.GetRenderedReportSettings(e.TargetTabItem);
                    string reportSettingsString = this.GetDirtyReport(report);

                    if (!reportInformation.DirtyReport.Equals(reportSettingsString))
                    {
                        MessageBoxResult result = MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSaveChanges") + (string.IsNullOrEmpty(reportInformation.ReportName) ? e.TargetTabItem.Header : reportInformation.ReportName) + "?", SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.YesNoCancel);

                        if (result == MessageBoxResult.Yes)
                        {
                            this.SaveDialog(SaveFileType.Save, reportInformation);
                            this.ReportsCollection.Remove(reportInformation);
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            this.ReportsCollection.Remove(reportInformation);
                        }
                        else if (result == MessageBoxResult.Cancel)
                        {
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        this.ReportsCollection.Remove(reportInformation);
                    }

                    this.CheckAllReportsClosed();
                    if (this.ReportsCollection.Count > 1)
                    {
                        this.TabControl.CloseButtonType = CloseButtonType.Common;
                        this.TabControl.DefaultContextMenuItemVisibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.TabControl.CloseButtonType = CloseButtonType.Hide;
                        this.TabControl.DefaultContextMenuItemVisibility = System.Windows.Visibility.Hidden;
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Inner Tab Exception : " + ee.ToString());
            }
        }

        private void CheckAllReportsClosed()
        {
            if (this.ReportsCollection.Count == 0)
            {
                this.RaiseAllReportClosed();
            }
        }

        void TabControl_SelectedItemChangedEvent(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.NewSelectedItem != null)
            {
                DesignPanel designPanel = ((e.NewSelectedItem.Content as RDLDesignerControl)).DesignControl;
                this.UpdateDesignPanel(designPanel);
                foreach (var propertygrid in ReportPropertyGrids)
                {
                    if (designPanel.SelectedReportItems.Count > 0)
                    {
                        propertygrid.SelectedItem = this.SetSelectedItem(designPanel.SelectedReportItems.First());
                    }
                    else
                    {
                        propertygrid.SelectedItem = designPanel.reportproperties;
                    }
                }
                foreach (var groupingpanel in this.TablixGroupingPanels)
                {
                    if (designPanel.SelectedReportItems != null)
                    {
                        groupingpanel.SelectedReportItem = designPanel.SelectedReportItems.Where(reportItem => reportItem is TablixControl).FirstOrDefault();
                    }
                    else
                    {
                        groupingpanel.SelectedReportItem = null;
                    }
                }
                this.UpdateDesignerToolboxObj();
                this.DataSourcesUpdate(designPanel.DataSources);
                this.EmbeddedImagesUpdate(designPanel.EmbeddedImages);
                this.ParametersUpdate(designPanel.ReportParameters);
                this.DataSetsUpdate(designPanel.DataSets);
                this.ToggleItemsUpdate(designPanel.reportItems);
                designPanel.Loaded += (s, eArgs) =>
                {
                    this.UpdateHeaderFooterTab();
                    try
                    {
                        if (CurrentPanel.RDLType == RDL.DOM.RDLType.RDL2010)
                        {
                            this.ReportFormat = Designer.ReportFormat.RDL2010;
                        }
                        else
                        {
                            this.ReportFormat = Designer.ReportFormat.RDL2008;
                        }
                    }
                    catch { }
                };
                this.RaiseOpenReportChanged(e.NewSelectedItem.Header.ToString());
            }
        }

        object SetSelectedItem(IReportItemControl selectedItem)
        {
            try
            {
                object selectedObject = null;
                switch (selectedItem.ItemType)
                {
                    case Designer.DrawingReportItem.Line:
                        selectedObject = (selectedItem as LineControl).Properties;
                        break;
                    case Designer.DrawingReportItem.Image:
                        selectedObject = (selectedItem as ImageControl).ImageProperties;
                        break;
                    case Designer.DrawingReportItem.TextBox:
                        selectedObject = (selectedItem as TextBoxControl).Properties;
                        break;
                    case Designer.DrawingReportItem.Tablix:
                        selectedObject = (selectedItem as TablixControl).tablixproperties;
                        break;
                    case Designer.DrawingReportItem.Gauge:
                        selectedObject = (selectedItem as GaugeControl).Properties;
                        break;
                    case Designer.DrawingReportItem.Rectangle:
                        selectedObject = (selectedItem as RectangleControl).Properties;
                        break;
                    case Designer.DrawingReportItem.Chart:
                        selectedObject = (selectedItem as ChartControl).chartProperties;
                        break;
                    case Designer.DrawingReportItem.SubReport:
                        selectedObject = (selectedItem as SubReportControl).Properties;
                        break;
                }
                return selectedObject;
            }
            catch { return null; }
        }

        #endregion

        #region custom_events

        internal void UpdateHeaderFooterTab()
        {
            this.RaiseUpdateHeaderFooterTab();
        }

        public void ShowWizard(ReportItemWizard wizard)
        {
            if (wizard == ReportItemWizard.Chart)
            {
                this.GetDisplayTabDesignPanel().AddChartThroughWizard(RESX.titleNewChart);
            }
            else if (wizard == ReportItemWizard.Table)
            {
                this.GetDisplayTabDesignPanel().AddTablixThroughWizard(RESX.titleNewTable);
            }
        }

        internal event SerializeEventhandler SerializeEvent;

        internal void RaiseSerializeEventhandler(IOrderedEnumerable<RecentReport> recentreports)
        {
            if (this.SerializeEvent != null)
            {
                this.SerializeEvent(this, new SerializeEventArgs { recentReports = recentreports });
            }
        }

        public event ReportChangedEventHandler ReportSaved;

        internal void RaiseReportSave(string reportName)
        {
            if (this.ReportSaved != null)
            {
                this.ReportSaved(this, new ReportChangedEventArgs { ReportName = reportName });
            }
        }

        public event ReportChangedEventHandler NewReportOpened;

        internal void RaiseNewReportOpen(string reportName)
        {
            if (this.NewReportOpened != null)
            {
                this.NewReportOpened(this, new ReportChangedEventArgs { ReportName = reportName });
            }
        }

        public event ReportChangedEventHandler ReportOpened;

        internal void RaiseOpenReportChanged(string reportname)
        {
            if (this.ReportOpened != null)
            {
                this.ReportOpened(this, new ReportChangedEventArgs { ReportName = reportname });
            }
        }

        public event AllReportsClosedEventHandler AllReportsClosed;

        internal void RaiseAllReportClosed()
        {
            if (this.AllReportsClosed != null)
            {
                this.AllReportsClosed(this, new AllReportsClosedEventArgs { });
            }
        }

        internal event UpdateHeaderFooterTabEventEventhandler UpdateHeaderFooterTabEvent;

        internal void RaiseUpdateHeaderFooterTab()
        {
            if (this.UpdateHeaderFooterTabEvent != null)
            {
                this.UpdateHeaderFooterTabEvent(this, new UpdateHeaderFooterTabEventArgs { });
            }
        }

        public event ReportItemDrawnEventHanlder ReportItemDrawn;

        internal void RaiseReportItemDrawnEvent(Object sender, ReportItemDrawnEventArgs arg)
        {
            if (this.ReportItemDrawn != null)
            {
                this.ReportItemDrawn(sender, arg);
            }
        }

        #endregion
    }
}