//-------------------------------------------------------------------------------------------------
// <copyright file="ReportDesigner.xaml.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.Windows.Reports.Designer
{
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
    using Syncfusion.Windows.ReportDesigner.Resources;
    /// <summary>
    /// Partial Class for Report Designer
    /// </summary>

    public partial class ReportDesigner
        : UserControl
    {

        #region Variables

        DockState viewerReportDataDockState = DockState.Dock;
        DockState viewerPropertiesDockState = DockState.Dock;
        DockState viewerGroupingDockState = DockState.Dock;
        //DockState viewerToolboxDockState = DockState.Dock;
        DockState reportDataState = DockState.Dock;
        DockState propertyWindowState = DockState.Dock;
        DockState groupingWindowState = DockState.Dock;

        bool isReportRun = false;
        bool isButtonParameterChecked = false;
        bool internalDockStateChange = false;

        List<RibbonButton> recentFileItems = new List<RibbonButton>();

        public int flag = 0;

        #endregion

        #region Exposed Public Properties

        public event ReportChangedEventHandler ReportOpened;

        public event ReportChangedEventHandler NewReportOpened;

        internal DatabaseAccessTypes DatabaseAccess
        {
            get
            {
                return (DatabaseAccessTypes)GetValue(DatabaseAccessProperty);
            }
            set
            {
                SetValue(DatabaseAccessProperty, value);
            }
        }

        public bool ShowToolbox
        {
            get
            {
                return (bool)GetValue(ShowToolboxProperty);
            }
            set
            {
                SetValue(ShowToolboxProperty, value);
            }
        }

        public bool ShowRibbon
        {
            get
            {
                return (bool)GetValue(ShowRibbonProperty);
            }
            set
            {
                SetValue(ShowRibbonProperty, value);
            }
        }

        public bool ShowReportData
        {
            get
            {
                return (bool)GetValue(ShowReportDataProperty);
            }

            set
            {
                SetValue(ShowReportDataProperty, value);
            }
        }

        public bool ShowApplicationMenu
        {
            get
            {
                return (bool)GetValue(ShowApplicationMenuProperty);
            }

            set
            {
                SetValue(ShowApplicationMenuProperty, value);
            }
        }

        internal static readonly DependencyProperty DatabaseAccessProperty =
            DependencyProperty.Register("DatabaseAccess", typeof(DatabaseAccessTypes), typeof(ReportDesigner), new UIPropertyMetadata(DatabaseAccessTypes.None, null));

        public static readonly DependencyProperty ShowToolboxProperty =
            DependencyProperty.Register("ShowToolbox", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata(true));

        public bool ShowRuler
        {
            get { return (bool)GetValue(ShowRulerProperty); }
            set { SetValue(ShowRulerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRulerVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowRulerProperty =
            DependencyProperty.Register("ShowRuler", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata((bool)true, ShowRulerPropertyChanged));

        internal static void ShowRulerPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportdesigner = dependencyObject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue == false)
                {
                    reportdesigner.designerPanelControl.ShowRuler = false;
                }
                else
                {
                    reportdesigner.designerPanelControl.ShowRuler = true;
                }
            }
        }

        public static readonly DependencyProperty ShowRibbonProperty =
            DependencyProperty.Register("ShowRibbon", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata(true, OnShowRibbonPropertyPropertyChanged));

        public static void OnShowRibbonPropertyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;

            if (!DesignerProperties.GetIsInDesignMode(reportDesigner) && reportDesigner != null)
            {
                if ((bool)e.NewValue == false)
                {
                    reportDesigner.GridToolbox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    reportDesigner.GridToolbox.Visibility = Visibility.Visible;
                }
            }
        }

        public static readonly DependencyProperty ShowReportDataProperty =
           DependencyProperty.Register("ShowReportData", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata(true, OnShowReportDataPropertyChanged));

        public static void OnShowReportDataPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;

            if (!DesignerProperties.GetIsInDesignMode(reportDesigner) && reportDesigner != null)
            {
                if (e.NewValue != e.OldValue)
                {
                    if ((bool)e.NewValue == true)
                    {
                        reportDesigner.chk_ReportData.IsChecked = true;
                        DockingManager.SetState(reportDesigner.grd_ReportData, reportDesigner.reportDataState);
                    }
                    else
                    {
                        reportDesigner.reportDataState = DockingManager.GetState(reportDesigner.grd_ReportData);
                        reportDesigner.chk_ReportData.IsChecked = false;
                        reportDesigner.internalDockStateChange = true;
                        DockingManager.SetState(reportDesigner.grd_ReportData, DockState.Hidden);
                        reportDesigner.internalDockStateChange = false;
                    }
                }
            }
        }

        public static readonly DependencyProperty ShowApplicationMenuProperty =
            DependencyProperty.Register("ShowApplicationMenu", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata(true, OnShowApplicationMenuPropertyChanged));

        public static void OnShowApplicationMenuPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;

            if (!DesignerProperties.GetIsInDesignMode(reportDesigner) && reportDesigner != null)
            {
                if ((bool)e.NewValue == true)
                {
                    reportDesigner.MainMenu.Visibility = Visibility.Visible;
                    reportDesigner.MainMenu.Height = 38;
                }
                else
                {
                    reportDesigner.MainMenu.Visibility = Visibility.Collapsed;
                    reportDesigner.MainMenu.Height = 0;
                }
            }
        }

        public bool ShowProperties
        {
            get { return (bool)GetValue(ShowPropertiesProperty); }
            set { SetValue(ShowPropertiesProperty, value); }
        }

        public static readonly DependencyProperty ShowPropertiesProperty =
            DependencyProperty.Register("ShowProperties", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata(false, OnShowPropertiesPropertyChanged));

        public static void OnShowPropertiesPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;

            if (!DesignerProperties.GetIsInDesignMode(reportDesigner) && reportDesigner != null)
            {
                if (e.NewValue != e.OldValue)
                {
                    bool showPropertyWindow = (bool)e.NewValue;
                    reportDesigner.chk_Properties.IsChecked = showPropertyWindow;

                    if (showPropertyWindow)
                    {
                        reportDesigner.chk_Properties.IsChecked = true;
                        reportDesigner.designerPanelControl.ShowProperties = showPropertyWindow;
                        DockingManager.SetState(reportDesigner.grd_PropertyWindow, reportDesigner.propertyWindowState);
                    }
                    else
                    {
                        reportDesigner.designerPanelControl.ShowProperties = showPropertyWindow;
                        reportDesigner.propertyWindowState = DockingManager.GetState(reportDesigner.grd_PropertyWindow);
                        reportDesigner.chk_Properties.IsChecked = false;
                        reportDesigner.internalDockStateChange = true;
                        DockingManager.SetState(reportDesigner.grd_PropertyWindow, DockState.Hidden);
                        reportDesigner.internalDockStateChange = false;
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
            DependencyProperty.Register("ShowGrouping", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata(false, OnShowGroupingPropertyChanged));

        public static void OnShowGroupingPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;

            if (!DesignerProperties.GetIsInDesignMode(reportDesigner) && reportDesigner != null)
            {
                if (e.NewValue != e.OldValue)
                {
                    bool showGroupingWindow = (bool)e.NewValue;
                    reportDesigner.chk_Grouping.IsChecked = showGroupingWindow;

                    if (showGroupingWindow)
                    {
                        reportDesigner.chk_Grouping.IsChecked = true;
                        reportDesigner.designerPanelControl.ShowGrouping = showGroupingWindow;
                        DockingManager.SetState(reportDesigner.grd_GroupingWindow, reportDesigner.groupingWindowState);
                        DockingManager.SetNoHeader(reportDesigner.grd_GroupingWindow, true);
                    }
                    else
                    {
                        reportDesigner.designerPanelControl.ShowGrouping = showGroupingWindow;
                        reportDesigner.propertyWindowState = DockingManager.GetState(reportDesigner.grd_GroupingWindow);
                        reportDesigner.chk_Grouping.IsChecked = false;
                        reportDesigner.internalDockStateChange = true;
                        DockingManager.SetState(reportDesigner.grd_GroupingWindow, DockState.Hidden);
                        reportDesigner.internalDockStateChange = false;
                    }
                }
            }
        }

        public ImageSource ApplicationMenuButtonImage
        {
            get
            {
                return (ImageSource)GetValue(ApplicationMenuButtonImageProperty);
            }
            set
            {
                SetValue(ApplicationMenuButtonImageProperty, value);
            }
        }

        public static readonly DependencyProperty ApplicationMenuButtonImageProperty =
            DependencyProperty.Register("ApplicationButtonButtonImage", typeof(ImageSource), typeof(ReportDesigner), new UIPropertyMetadata(null, OnApplicationMenuButtonImagePropertyChanged));

        public static void OnApplicationMenuButtonImagePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;

            if (e.NewValue != null)
            {
                reportDesigner.RibbonToolBar.ApplicationMenu.ApplicationButtonImage = (BitmapImage)e.NewValue;
            }
        }

        internal string VisualStyle
        {
            get { return (string)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        internal static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(string), typeof(ReportDesigner), new UIPropertyMetadata("Office2007Blue", VisualStylePropertyChanged));

        internal static void VisualStylePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;

            if (!DesignerProperties.GetIsInDesignMode(reportDesigner) && reportDesigner != null)
            {
                reportDesigner.UpdateStyle();
            }
        }

        public DrawingReportItem DrawingReportItem
        {
            get { return (DrawingReportItem)GetValue(DrawingReportItemProperty); }
            set { SetValue(DrawingReportItemProperty, value); }
        }

        public static readonly DependencyProperty DrawingReportItemProperty =
            DependencyProperty.Register("DrawingReportItem", typeof(DrawingReportItem), typeof(ReportDesigner), new UIPropertyMetadata(DrawingReportItem.None, onDrawingReportItemPropertyChanged));

        public static void onDrawingReportItemPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;

            if (!DesignerProperties.GetIsInDesignMode(reportDesigner) && reportDesigner != null)
            {
                reportDesigner.designerPanelControl.DrawingReportItem=reportDesigner.DrawingReportItem;
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
        DependencyProperty.Register("Assemblies", typeof(List<Assembly>), typeof(ReportDesigner), new UIPropertyMetadata(new List<Assembly>(), onAssemblyPropertyChanged));

        public static void onAssemblyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;

            if (!DesignerProperties.GetIsInDesignMode(reportDesigner) && reportDesigner != null)
            {
                reportDesigner.designerPanelControl.Assemblies = reportDesigner.Assemblies;
            }
        }


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

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportServerUrl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerUrlProperty =
        DependencyProperty.Register("ReportServerUrl", typeof(string), typeof(ReportDesigner), new PropertyMetadata(string.Empty, onReportServerUrlPropertyChanged));

        public static void onReportServerUrlPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                reportDesigner.designerPanelControl.ReportServerUrl = reportDesigner.ReportServerUrl;
            }
        }

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
        DependencyProperty.Register("ReportServerCredential", typeof(ICredentials), typeof(ReportDesigner), new PropertyMetadata(null, onReportServerCredentialPropertyChanged));

        public static void onReportServerCredentialPropertyChanged(DependencyObject dependencyobject,DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyobject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                reportDesigner.designerPanelControl.ReportServerCredential = reportDesigner.ReportServerCredential;
            }
        }

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

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Windows.Reports.Viewer.ReportViewer.ReportServerFormsCredential"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReportServerFormsCredentialProperty =
        DependencyProperty.Register("ReportServerFormsCredential", typeof(ReportServerFormsCredential), typeof(ReportDesigner), new PropertyMetadata(null, onReportServerFormsCredentialPropertyChanged));

        public static void onReportServerFormsCredentialPropertyChanged(DependencyObject dependencyobject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyobject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                reportDesigner.designerPanelControl.ReportServerFormsCredential = reportDesigner.ReportServerFormsCredential;
            }
        }

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

        public static readonly DependencyProperty ReportFormatProperty =
            DependencyProperty.Register("ReportFormat", typeof(ReportFormat), typeof(ReportDesigner), new UIPropertyMetadata(ReportFormat.RDL2010, onReportFormatPropertyChanged));

        public static void onReportFormatPropertyChanged(DependencyObject dependencyobject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyobject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                reportDesigner.designerPanelControl.ReportFormat = reportDesigner.ReportFormat;
                if (reportDesigner.ReportFormat == ReportFormat.RDL2008)
                {
                    reportDesigner.btn_DataBar.Visibility = Visibility.Collapsed;
                    reportDesigner.btn_SparkLine.Visibility = Visibility.Collapsed;
                    reportDesigner.btn_Map.Visibility = Visibility.Collapsed;
                }
                else
                {
                    reportDesigner.btn_DataBar.Visibility = Visibility.Visible;
                    reportDesigner.btn_SparkLine.Visibility = Visibility.Visible;
                    reportDesigner.btn_Map.Visibility = Visibility.Visible;
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
    DependencyProperty.Register("DesignMode", typeof(DesignMode), typeof(ReportDesigner), new UIPropertyMetadata(DesignMode.RDL, OnDesignViewPropertyChanged));

        internal static void OnDesignViewPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportdesigner = dependencyObject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                reportdesigner.designerPanelControl.DesignMode = (DesignMode)e.NewValue;
            }
        }

        public bool EnableMDIDesigner
        {
            get { return (bool)GetValue(EnableMDIDesignerProperty); }
            set { SetValue(EnableMDIDesignerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMultiReportEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableMDIDesignerProperty =
            DependencyProperty.Register("EnableMDIDesigner", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata((bool)true, OnEnableMDIDesignerPropertyChanged));

        public static void OnEnableMDIDesignerPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportDesigner = dependencyObject as ReportDesigner;

            if (!DesignerProperties.GetIsInDesignMode(reportDesigner) && reportDesigner != null)
            {
                if ((bool)e.NewValue == false)
                {
                    reportDesigner.designerPanelControl.EnableMDIDesigner = (bool)reportDesigner.EnableMDIDesigner;
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
            DependencyProperty.Register("ShowHeader", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata((bool)false, OnShowHeaderPropertyChanged));

        internal static void OnShowHeaderPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportdesigner = dependencyObject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                reportdesigner.designerPanelControl.ShowHeader = (bool)e.NewValue;
            }
        }

        public bool ShowFooter
        {
            get { return (bool)GetValue(ShowFooterProperty); }
            set { SetValue(ShowFooterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFooterVisible.
        public static readonly DependencyProperty ShowFooterProperty =
            DependencyProperty.Register("ShowFooter", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata((bool)true, OnShowFooterPropertyChanged));

        internal static void OnShowFooterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportdesigner = dependencyObject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                reportdesigner.designerPanelControl.ShowFooter = (bool)e.NewValue;
            }
        }

        public bool ShowHelp
        {
            get { return (bool)GetValue(ShowHelpProperty); }
            set { SetValue(ShowHelpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowHelp.
        public static readonly DependencyProperty ShowHelpProperty =
            DependencyProperty.Register("ShowHelp", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata((bool)true, OnShowHelpPropertyChanged));

        internal static void OnShowHelpPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportdesigner = dependencyObject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                reportdesigner.designerPanelControl.ShowHelp = (bool)e.NewValue;
            }
        }

        public bool ShowZoom
        {
            get { return (bool)GetValue(ShowZoomProperty); }
            set { SetValue(ShowZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowZoom.
        public static readonly DependencyProperty ShowZoomProperty =
            DependencyProperty.Register("ShowZoom", typeof(bool), typeof(ReportDesigner), new UIPropertyMetadata((bool)true, OnShowZoomPropertyChanged));

        internal static void OnShowZoomPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportdesigner = dependencyObject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                {
                    reportdesigner.GridRoot.RowDefinitions[2].Height = new GridLength(25);
                    reportdesigner.grd_Zooming.Visibility = Visibility.Visible;
                }
                else
                {
                    reportdesigner.grd_Zooming.Visibility = Visibility.Collapsed;
                    reportdesigner.GridRoot.RowDefinitions[2].Height = new GridLength(0);
                }
            }
        }

        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ReportDesigner), new UIPropertyMetadata((double)100, OnZoomFactorPropertyChanged));

        internal static void OnZoomFactorPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDesigner reportdesigner = dependencyObject as ReportDesigner;
            if (e.NewValue != e.OldValue)
            {
                reportdesigner.designerPanelControl.ZoomFactor = (double)e.NewValue;
            }
        }

        #endregion

        #region Constructor

        public ReportDesigner()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                RESX.Culture = Thread.CurrentThread.CurrentCulture;
                InitializeComponent();
                this.Style = (System.Windows.Style)this.Resources["triggerStyle"];
                this.PreviewKeyUp += new KeyEventHandler(ReportDesigner_PreviewKeyUp);
                this.PreviewKeyDown += new KeyEventHandler(ReportDesigner_PreviewKeyDown);
                
                designerPanelControl.AddReportData(this.report_data);
                designerPanelControl.AddReportProperty(this.property_grid);
                designerPanelControl.AddGrouping(this.grouping_grid);

                designerPanelControl.ReportOpened += new ReportChangedEventHandler(designerPanelControl_OpenReportChanged);
                designerPanelControl.SerializeEvent += new SerializeEventhandler(designerPanelControl_SerializeEvent);
                designerPanelControl.ReportSaved += new ReportChangedEventHandler(designerPanelControl_ReportSave);
                designerPanelControl.UpdateHeaderFooterTabEvent += new UpdateHeaderFooterTabEventEventhandler(designerPanelControl_UpdateHeaderFooterTabEvent);
                designerPanelControl.NewReportOpened += new ReportChangedEventHandler(designerPanelControl_NewReportOpened);
                designerPanelControl.AllReportsClosed += new AllReportsClosedEventHandler(designerPanelControl_AllReportClosed);
                designerPanelControl.ReportItemDrawn += new ReportItemDrawnEventHanlder(designerPanelControl_ReportItemDrawn);
                this.GridRoot.DataContext = this;
                WireEvents();
                BindingProperties();
                this.Loaded += new RoutedEventHandler(ReportDesigner_Loaded);
            }
            else
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "<<Report Designer>>";
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
            try
            {
                UpdateCulture();
            }
            catch { }
        }

        void UpdateCulture()
        {
            this.btn_TextBox.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelTextBox");
            this.btn_TextBox.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipTextboxControl");
            this.btn_Image.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelImage");
            this.btn_Image.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipImageControl");
            this.btn_Line.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelLine");
            this.btn_Line.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipLineControl");
            this.btn_Rectangle.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelRectangle");
            this.btn_Rectangle.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipRectangleControl");

            this.btn_Gauge.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelGauge");
            this.btn_Gauge.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipGaugeControl");
            this.btn_Map.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelMap");
            this.btn_Map.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipMapControl");
            this.btn_DataBar.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelDataBar");
            this.btn_DataBar.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipDataBarControl");
            this.btn_SparkLine.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelSparkline");
            this.btn_SparkLine.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipSparklineControl");
            this.ribbonTabInsert.Caption = SR.GetString(CultureInfo.CurrentUICulture, "captionInsert");
            this.ribbonTabView.Caption = SR.GetString(CultureInfo.CurrentUICulture, "captionView");
            this.chk_ReportData.Content = SR.GetString(CultureInfo.CurrentUICulture, "chkBoxReportData");
            this.chk_Ruler.Content = SR.GetString(CultureInfo.CurrentUICulture, "chkBoxRuler");
            this.chk_Properties.Content = SR.GetString(CultureInfo.CurrentUICulture, "chkBoxPropertyWindow");
            this.chk_Grouping.Content = SR.GetString(CultureInfo.CurrentUICulture, "chkBoxGrouping");

            this.btn_Subreport.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelSubReport");
            this.btn_Subreport.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipSubreportControl");

            this.btn_Header.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelHeader");
            this.btn_AddHeader.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddHeader");

            this.btn_Footer.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelFooter");
            this.btn_AddFooter.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddFooter");
            this.btn_Footer.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipReportFooter");
            this.btn_Header.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipReportHeader");

            this.btn_chart.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelChart");
            this.btn_chart.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipChartControl");
            this.btn_ChartWizard.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerChartWizard");
            this.btn_InsertChart.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerInsertChart");

            this.btn_table.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelTable");
            this.btn_table.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipTableControl");
            this.btn_TableWizard.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerTableWizard");
            this.btn_InsertTable.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerInsertTable");
            this.btn_List.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelList");
            this.btn_List.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipListControl");

            this.btn_matrix.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelMatrix");
            this.btn_matrix.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipMatrixControl");
            this.btn_MatrixWizard.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerMatrixWizard");
            this.btn_InsertMatrix.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerInsertMatrix");

            this.bar_dataregion.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDataRegions");
            this.bar_datavisual.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDataVisualizations");
            this.bar_reportitems.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerReportItems");
            this.bar_subreport.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerSubReport");
            this.bar_headerandfooter.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerHeaderAndFooter");
            this.btn_application_menu.Description = SR.GetString(CultureInfo.CurrentUICulture, "txtOfficeButton");
            this.text_officebutton.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "toolTipOfficeButton");

            this.backStageNewButton.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelNew");
            this.backStageOpenButton.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelOpen");
            this.backStageSaveButton.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelSave");
            this.backStageSaveAsButton.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelSaveAs");
            this.backStageExitButton.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelExit");
            this.texBlock_recentreports.Text = SR.GetString(CultureInfo.CurrentUICulture, "textBlockRecentReports");
            this.SimpleMenuButtonFileNew.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelNew");
            this.SimpleMenuButtonFilOpen.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelOpen");
            this.simpleMenuSave.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelSave");
            this.simpleMenuSaveAs.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelSaveAs");
            this.simpleMenuExit.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelExit");
            this.RibbonToolBar.BackStageHeader = SR.GetString(CultureInfo.CurrentUICulture, "File");

            this.buttonFirst.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelFirst");
            this.buttonPrevious.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelPrevious");
            this.buttonNext.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelNext");
            this.buttonLast.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelLast");
            this.buttonRefresh.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelRefresh");
            this.buttonUp.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelUp");
            this.buttonStop.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelStop");
            this.buttonPrint.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelPrint");
            this.buttonPrintSetup.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelPageSetup");
            this.buttonPrintLayout.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelPrintLayout");

            this.reportBarExport.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerExport");
            this.reportBarPrint.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerPrint");
            this.reportBarNavigation.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerNavigation");

            this.buttonExport.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelExport");
            this.buttonPdfExport.Header = SR.GetString(CultureInfo.CurrentUICulture, "ribbonPdfExport");
            this.buttonExcelExport.Header = SR.GetString(CultureInfo.CurrentUICulture, "ribbonExcelExport");
            this.buttonWordExport.Header = SR.GetString(CultureInfo.CurrentUICulture, "ribbonWordExport");
            this.buttonHtmlExport.Header = SR.GetString(CultureInfo.CurrentUICulture, "ribbonHtmlExport");

            this.reportBarOptions.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerOptions");
            this.buttonDocumentMap.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelDocumentMap");
            this.buttonParameters.Label = SR.GetString(CultureInfo.CurrentUICulture, "labelParameters");
            this.labelOf.Content = SR.GetString(CultureInfo.CurrentUICulture, "of");
            this.format_barView.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerViews");
            this.reportBarDesignView.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerViews");
            this.RecentDocuments.Text = SR.GetString(CultureInfo.CurrentUICulture, "Recent Documents");
            this.texBlock_recentreports.Text = SR.GetString(CultureInfo.CurrentUICulture, "textBlockRecentReports");
            this.textBlockRecentReports.Header = SR.GetString(CultureInfo.CurrentUICulture, "textBlockRecentReports");

            DockingManager.SetHeader(grd_PropertyWindow, Syncfusion.Windows.ReportDesigner.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "chkBoxPropertyWindow"));
            DockingManager.SetHeader(grd_ReportData, Syncfusion.Windows.ReportDesigner.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "chkBoxReportData"));

        }

        void BindingProperties()
        {
            Binding serverurlbinding = new Binding();
            serverurlbinding.Source = this;
            serverurlbinding.Path = new PropertyPath("ReportServerUrl");
            serverurlbinding.Mode = BindingMode.TwoWay;
            designerPanelControl.SetBinding(ReportDesignView.ReportServerUrlProperty, serverurlbinding);

            Binding credentialbinding = new Binding();
            credentialbinding.Source = this;
            credentialbinding.Path = new PropertyPath("ReportServerCredential");
            credentialbinding.Mode = BindingMode.TwoWay;
            designerPanelControl.SetBinding(ReportDesignView.ReportServerCredentialProperty, credentialbinding);

            Binding serverformcredbinding = new Binding();
            serverformcredbinding.Source = this;
            serverformcredbinding.Path = new PropertyPath("ReportServerFormsCredential");
            serverformcredbinding.Mode = BindingMode.TwoWay;
            designerPanelControl.SetBinding(ReportDesignView.ReportServerFormsCredentialProperty, serverformcredbinding);

            Binding reportFormatbinding = new Binding();
            reportFormatbinding.Source = this;
            reportFormatbinding.Path = new PropertyPath("ReportFormat");
            reportFormatbinding.Mode = BindingMode.TwoWay;
            designerPanelControl.SetBinding(ReportDesignView.ReportFormatProperty, reportFormatbinding);

        }

        void ReportDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            designerPanelControl.DeserializeRecentFiles();
        }

        void designerPanelControl_ReportItemDrawn(object sender, ReportItemDrawnEventArgs e)
        {
            this.DrawingReportItem = DrawingReportItem.None;
            this.UpdateButtonStatus();
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
            }
            
        }

        void designerPanelControl_AllReportClosed(object sender, AllReportsClosedEventArgs e)
        {
            this.RaiseAllReportsClosed();
        }

        void designerPanelControl_OpenReportChanged(object sender, ReportChangedEventArgs e)
        {
            this.RaiseOpenReportChanged(e.ReportName);
        }

        void designerPanelControl_NewReportOpened(object sender, ReportChangedEventArgs e)
        {
            this.RaiseNewReportOpened(e.ReportName);
        }

        void designerPanelControl_UpdateHeaderFooterTabEvent(object sender, UpdateHeaderFooterTabEventArgs e)
        {
            this.UpdateHeaderFooterTab();
        }

        void designerPanelControl_ReportSave(object sender, ReportChangedEventArgs e)
        {
            this.RaiseReportSaved(e.ReportName);
        }

        #endregion

        void DesignerDockingManager_DockStateChanging(FrameworkElement sender, DockStateChangingEventArgs e)
        {
            if (e.TargetState == DockState.Hidden && !this.internalDockStateChange)
            {
                if (e.SourceElement == this.grd_PropertyWindow)
                {
                    this.ShowProperties = false;
                    this.propertyWindowState = e.PresentState;
                    e.Cancel = true;
                }
                else if (e.SourceElement == this.grd_ReportData)
                {
                    this.ShowReportData = false;
                    this.reportDataState = e.PresentState;
                    e.Cancel = true;
                }
                else if (e.SourceElement == this.grd_GroupingWindow)
                {
                    this.ShowGrouping = false;
                    this.groupingWindowState = e.PresentState;
                    e.Cancel = true;
                }
            }
        }

        void ReportDesigner_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.UpdateButtonStatus();
                var panel = designerPanelControl.GetDisplayTabDesignPanel();

                if (panel.SelectedReportItemType != DrawingReportItem.None)
                {
                    panel.SelectedReportItemType = DrawingReportItem.None;
                    panel.Cursor = Cursors.Arrow;
                }
                if (this.VisualStyle == "Metro")
                {
                    this.UpdateMetroImages();
                }
            }
        }

        void ReportDesigner_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.S)
                {
                    designerPanelControl.SaveDialog(SaveFileType.Save);
                }
                else if (e.Key == Key.N)
                {
                    this.NewReport();
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey.ToString().ToUpper() == "F")
            {
                RibbonToolBar.ShowBackStage();
            }
            else if (e.Key == Key.F5)
            {
                ViewReportViewer();
            }
            else if (e.Key == Key.F8)
            {
                ViewReportDesigner();
            }
        }


        #region Recent Files List

        private void AddRecentFilesToMenu(string fileName, string fileUrl, string reportServer)
        {
            RibbonButton button = new RibbonButton();
            button.Label = fileName;
            button.Content = reportServer;
            button.Click += new RoutedEventHandler(RecentFiles_Click);
            button.ToolTip = fileUrl;
            button.SmallIcon = null;
            this.recentFileItems.Add(button);
        }

        void designerPanelControl_SerializeEvent(object sender, SerializeEventArgs e)
        {
            try
            {
                backStageStackPanelRecentFiles.ItemsSource = null;
                stackPanelRecentFiles.ItemsSource = null;
                var recentFilesList = e.recentReports;
                this.recentFileItems.Clear();
                backStageStackPanelRecentFiles.SelectionMode = stackPanelRecentFiles.SelectionMode = SelectionMode.Single;

                foreach (RecentReport recentFile in recentFilesList)
                {
                    string fileext = System.IO.Path.GetExtension(recentFile.ReportPath).ToString().ToLower();
                    if (this.DesignMode == DesignMode.RDL)
                    {
                        if (fileext == ".rdl" || fileext == "")
                        {
                            this.AddRecentFilesToMenu(System.IO.Path.GetFileName(recentFile.ReportPath), recentFile.ReportPath, recentFile.ReportServer);
                        }
                    }
                    else
                    {
                        if (fileext == ".rdlc")
                        {
                            this.AddRecentFilesToMenu(System.IO.Path.GetFileName(recentFile.ReportPath), recentFile.ReportPath, recentFile.ReportServer);
                        }
                    }
                }

                backStageStackPanelRecentFiles.ItemsSource = this.recentFileItems;
                stackPanelRecentFiles.ItemsSource = this.recentFileItems;
            }
            catch
            {

            }
        }

        private void RecentFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RibbonButton SelectedItem = null;

                if (sender is RibbonButton)
                {
                    SelectedItem = (sender as RibbonButton);
                    stackPanelRecentFiles.SelectedItem = SelectedItem;
                    backStageStackPanelRecentFiles.SelectedItem = SelectedItem;
                }
                else
                {
                    SelectedItem = (sender as ListBox).SelectedItem as RibbonButton;
                }

                this.RibbonToolBar.HideBackStage();

                string fileName = SelectedItem.Label;
                string fileUrl = SelectedItem.ToolTip.ToString();
                if (SelectedItem.Content != null)
                {
                    ReportServerUrl = SelectedItem.Content.ToString();
                    designerPanelControl.OpenReport(fileUrl, ReportServerUrl);
                }
                else
                {
                    this.OpenReport(fileUrl);
                }

                this.ViewReportDesigner();
                this.RaiseOpenReportChanged(fileName);
            }
            catch
            {

            }
        }

        #endregion

        #region Common - Helper Methods

        private void WireEvents()
        {
 #if SyncfusionFramework3_5
            this.btn_Map.Visibility = System.Windows.Visibility.Collapsed;
#endif
            this.DesignerDockingManager.DockStateChanging += new DockStateChangingHandler(DesignerDockingManager_DockStateChanging);
            this.btn_InsertChart.Click += new RoutedEventHandler(Btn_InsertChart_Click);
            this.btn_DataBar.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(btn_DataBar_PreviewMouseLeftButtonDown);
            this.btn_DataBar.Click += new RoutedEventHandler(btn_DataBar_Click);
            this.btn_Map.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(btn_Map_PreviewMouseLeftButtonDown);
            this.btn_Map.Click += new RoutedEventHandler(btn_Map_Click);
            this.btn_SparkLine.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(btn_SparkLine_PreviewMouseLeftButtonDown);
            this.btn_SparkLine.Click += new RoutedEventHandler(btn_SparkLine_Click);
            this.btn_Gauge.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_Gauge_MouseLeftButtonDown);
            this.btn_Gauge.Click += new RoutedEventHandler(Btn_Gauge_Click);
            this.btn_Image.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_Image_PreviewMouseLeftButtonDown);
            this.btn_Image.Click += new RoutedEventHandler(Btn_Image_Click);
            this.btn_Line.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_Line_PreviewMouseLeftButtonDown);
            this.btn_Line.Click += new RoutedEventHandler(Btn_Line_Click);
            this.btn_List.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_List_PreviewMouseLeftButtonDown);
            this.btn_List.Click += new RoutedEventHandler(Btn_List_Click);
            this.btn_Rectangle.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_Rectangle_PreviewMouseLeftButtonDown);
            this.btn_Rectangle.Click += new RoutedEventHandler(Btn_Rectangle_Click);
            this.btn_Subreport.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_Subreport_PreviewMouseLeftButtonDown);
            this.btn_Subreport.Click += new RoutedEventHandler(Btn_Subreport_Click);
            this.btn_InsertTable.Click += new RoutedEventHandler(Btn_InsertTable_Click);
            this.btn_TextBox.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_TextBox_PreviewMouseLeftButtonDown);
            this.btn_TextBox.Click += new RoutedEventHandler(Btn_TextBox_Click);
            this.btn_chart.IsDropDownOpenChanged += new PropertyChangedCallback(btn_chart_IsDropDownOpenChanged);
            this.btn_table.IsDropDownOpenChanged += new PropertyChangedCallback(btn_table_IsDropDownOpenChanged);
            this.btn_matrix.IsDropDownOpenChanged += new PropertyChangedCallback(btn_matrix_IsDropDownOpenChanged);
            this.btn_Header.IsDropDownOpenChanged += new PropertyChangedCallback(btn_Header_IsDropDownOpenChanged);
            this.btn_Footer.IsDropDownOpenChanged += new PropertyChangedCallback(btn_Footer_IsDropDownOpenChanged);

            this.chk_Ruler.Checked += new RoutedEventHandler(chk_Ruler_Checked);
            this.chk_Ruler.Unchecked += new RoutedEventHandler(chk_Ruler_Unchecked);

            this.chk_Properties.Checked += new RoutedEventHandler(chk_Properties_Checked);
            this.chk_Properties.Unchecked += new RoutedEventHandler(chk_Properties_Unchecked);

            this.chk_ReportData.Checked += new RoutedEventHandler(Chk_ReportData_Checked);
            this.chk_ReportData.Unchecked += new RoutedEventHandler(Chk_ReportData_Unchecked);

            this.chk_Grouping.Checked += new RoutedEventHandler(chk_Grouping_Checked);
            this.chk_Grouping.Unchecked += new RoutedEventHandler(chk_Grouping_Unchecked);

            this.btn_ViewDesign.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_ViewDesign_PreviewMouseLeftButtonDown);
            this.btn_ViewReport.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_ViewReport_PreviewMouseLeftButtonDown);
            this.buttonFirst.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ButtonFirst_PreviewMouseLeftButtonDown);
            this.buttonPrevious.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ButtonPrevious_PreviewMouseLeftButtonDown);
            this.buttonNext.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ButtonNext_PreviewMouseLeftButtonDown);
            this.buttonLast.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ButtonLast_PreviewMouseLeftButtonDown);
            this.buttonPrint.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ButtonPrint_PreviewMouseLeftButtonDown);
            this.buttonRefresh.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ButtonRefresh_PreviewMouseLeftButtonDown);
            this.buttonParameters.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ButtonParameters_PreviewMouseLeftButtonDown);
            this.buttonPrintLayout.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ButtonPrintLayout_PreviewMouseLeftButtonDown);

            this.btn_ViewReport.Click += new RoutedEventHandler(Btn_ViewReport_Click);
            this.btn_ViewDesign.Click += new RoutedEventHandler(Btn_ViewDesign_Click);

            //// Event tagging for Buttons in the Viewer
            this.buttonFirst.Click += new RoutedEventHandler(ButtonFirst_Click);
            this.buttonPrevious.Click += new RoutedEventHandler(ButtonPrevious_Click);
            this.buttonNext.Click += new RoutedEventHandler(ButtonNext_Click);
            this.buttonLast.Click += new RoutedEventHandler(ButtonLast_Click);

            this.buttonExport.IsDropDownOpenChanged += buttonExport_IsDropDownOpenChanged;
            this.buttonPdfExport.Click += new RoutedEventHandler(buttonPdfExport_Click);
            this.buttonExcelExport.Click += new RoutedEventHandler(buttonExcelExport_Click);
            this.buttonWordExport.Click += new RoutedEventHandler(buttonWordExport_Click);
            this.buttonHtmlExport.Click += new RoutedEventHandler(buttonHtmlExport_Click);
            this.buttonPrint.Click += new RoutedEventHandler(ButtonPrint_Click);
            this.buttonRefresh.Click += new RoutedEventHandler(ButtonRefresh_Click);
            this.buttonParameters.Click += new RoutedEventHandler(buttonParameters_Click);

            this.buttonPrintSetup.Click += new RoutedEventHandler(buttonPrintSetup_Click);
            this.buttonPrintLayout.Click += new RoutedEventHandler(buttonPrintLayout_Click);

            designerPanelControl.reportViewerControl.NavigationButtonsStateChanged += new Syncfusion.Windows.Reports.Viewer.NavigationButtonVisibilityChangedEventHandler(reportViewerControl_NavigationButtonsStateChanged);

            designerPanelControl.TabControl.OnCloseAllTabs += new OnCloseTabsEventHandler(TabControl_OnCloseAllTabs);
            designerPanelControl.TabControl.OnCloseOtherTabs += new OnCloseTabsEventHandler(TabControl_OnCloseOtherTabs);
            designerPanelControl.MouseRightButtonDown += new MouseButtonEventHandler(designerPanelControl_MouseRightButtonDown);

            backStageStackPanelRecentFiles.SelectionChanged += new SelectionChangedEventHandler(RecentFiles_Click);
            stackPanelRecentFiles.SelectionChanged += new SelectionChangedEventHandler(RecentFiles_Click);
        }

        void buttonExport_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                if (this.buttonExport.IsDropDownOpen)
                {
                    UpdateMetroImages();
                    this.buttonExport.LargeIcon = (DrawingImage)this.Resources["MetroExportSelected"];
                }
                else
                {
                    this.buttonExport.LargeIcon = (DrawingImage)this.Resources["MetroExport"];
                }
            }
        }

        void buttonHtmlExport_Click(object sender, RoutedEventArgs e)
        {
            designerPanelControl.reportViewerControl.ExportingToHtml();
        }

        void buttonWordExport_Click(object sender, RoutedEventArgs e)
        {
            designerPanelControl.reportViewerControl.ExportingToWord();
        }

        void buttonExcelExport_Click(object sender, RoutedEventArgs e)
        {
            designerPanelControl.reportViewerControl.ExportingToExcel();
        }

        void buttonPdfExport_Click(object sender, RoutedEventArgs e)
        {
            designerPanelControl.reportViewerControl.ExportingToPdf();
        }

        void designerPanelControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.UpdateButtonStatus();
            var panel = designerPanelControl.GetDisplayTabDesignPanel();

            if (panel.SelectedReportItemType != DrawingReportItem.None)
            {
                panel.SelectedReportItemType = DrawingReportItem.None;

                if (panel.GetDrawingCanvas() is Canvas)
                {
                    (e.OriginalSource as Canvas).Cursor = Cursors.Arrow;
                }
                else
                {
                    panel.Cursor = Cursors.Arrow;
                }
            }
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
            }
        }

        void chk_RdlView_Unchecked(object sender, RoutedEventArgs e)
        {
            designerPanelControl.IsRdlVisible = false;
        }

        void chk_RdlView_Checked(object sender, RoutedEventArgs e)
        {
            designerPanelControl.IsRdlVisible = true;
        }

        void chk_Properties_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ShowProperties = false;
        }

        void chk_Properties_Checked(object sender, RoutedEventArgs e)
        {
            this.ShowProperties = true;
        }

        void reportViewerControl_NavigationButtonsStateChanged(object sender, Syncfusion.Windows.Reports.Viewer.NavigationButtonVisibilityChangedEventArgs e)
        {
            try
            {
                this.buttonFirst.IsEnabled = designerPanelControl.reportViewerControl.IsFirstVisible;
                this.buttonPrevious.IsEnabled = designerPanelControl.reportViewerControl.IsPreviousVisible;
                this.buttonNext.IsEnabled = designerPanelControl.reportViewerControl.IsNextVisible;
                this.buttonLast.IsEnabled = designerPanelControl.reportViewerControl.IsLastVisible;
                this.textBoxCurrentPage.Text = designerPanelControl.reportViewerControl.CurrentPage.ToString();
                this.textBoxTotalPages.Content = designerPanelControl.reportViewerControl.textBoxTotalPages.Text;
            }
            catch (Exception ex)
            {
                designerPanelControl.reportViewerControl.ShowException(ex);
            }
        }

        private void UpdateButtonStatus()
        {
            this.btn_TextBox.IsSelected = false;
            this.btn_Subreport.IsSelected = false;
            this.btn_Rectangle.IsSelected = false;
            this.btn_List.IsSelected = false;
            this.btn_Line.IsSelected = false;
            this.btn_Image.IsSelected = false;
            this.btn_Gauge.IsSelected = false;
            this.btn_DataBar.IsSelected = false;
            this.btn_Map.IsSelected = false;
            this.btn_SparkLine.IsSelected = false;
            this.buttonFirst.IsSelected = false;
            this.buttonPrevious.IsSelected = false;
            this.buttonNext.IsSelected = false;
            this.buttonLast.IsSelected = false;
            this.buttonRefresh.IsSelected = false;
            this.buttonStop.IsSelected = false;
            this.buttonUp.IsSelected = false;
            this.buttonPrint.IsSelected = false;
            this.buttonPrintLayout.IsSelected = false;
            this.buttonPrintSetup.IsSelected = false;
            this.buttonParameters.IsSelected = false;
        }

        #endregion

        #region Report Viewer related events

        void ButtonPrintLayout_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
                this.buttonPrintLayout.LargeIcon = (DrawingImage)this.Resources["MetroPrintLayoutSelected"];
            }
        }

        void ButtonParameters_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
                this.buttonParameters.SmallIcon = (DrawingImage)this.Resources["MetroParameterSelected"];
            }
        }

        void ButtonRefresh_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
                this.buttonRefresh.SmallIcon = (DrawingImage)this.Resources["MetroRefreshSelected"];
            }
        }

        void ButtonPrint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
                this.buttonPrint.LargeIcon = (DrawingImage)this.Resources["MetroPrintSelected"];
            }
        }

        void ButtonLast_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
                this.buttonLast.LargeIcon = (DrawingImage)this.Resources["MetroLast_NavDisabledSelected"];
            }
        }

        void ButtonNext_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
                this.buttonNext.LargeIcon = (DrawingImage)this.Resources["MetroNext_NavDisabledSelected"];
            }
        }

        void ButtonPrevious_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
                this.buttonPrevious.LargeIcon = (DrawingImage)this.Resources["MetroPrevious_NavDisabledSelected"];
            }
        }

        void ButtonFirst_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
                this.buttonFirst.LargeIcon = (DrawingImage)this.Resources["MetroFirst_NavDisabledSelected"];
            }
        }

        void Btn_ViewReport_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
                this.btn_ViewReport.LargeIcon = (DrawingImage)this.Resources["MetroRunSelected"];
            }
        }

        void Btn_ViewDesign_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
                this.btn_ViewDesign.LargeIcon = (DrawingImage)this.Resources["MetroDesignSelected"];
            }
        }

        void buttonPrintLayout_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            this.buttonPrintLayout.IsSelected = true;

            if (designerPanelControl.reportViewerControl.ViewMode == ViewMode.Normal)
            {
                designerPanelControl.reportViewerControl.ViewMode = ViewMode.Print;
            }
            else if (designerPanelControl.reportViewerControl.ViewMode == ViewMode.Print)
            {
                designerPanelControl.reportViewerControl.ViewMode = ViewMode.Normal;
                designerPanelControl.reportViewerControl.RefreshReportViewer();
                this.buttonPrintLayout.IsSelected = false;
                if (this.VisualStyle == "Metro")
                {
                    this.UpdateMetroImages();
                }
            }
        }

        private void BackStageButton_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                BackStageCommandButton button = sender as BackStageCommandButton;

                this.backStageNewButton.Icon = (DrawingImage)this.Resources["MetroNewItem"];
                this.backStageOpenButton.Icon = (DrawingImage)this.Resources["MetroFolderOpen"];
                this.backStageSaveButton.Icon = (DrawingImage)this.Resources["MetroSave"];
                this.backStageSaveAsButton.Icon = (DrawingImage)this.Resources["MetroSaveAs"];
                this.backStageExitButton.Icon = (DrawingImage)this.Resources["MetroExit"];

                if (button == backStageNewButton)
                {
                    this.backStageNewButton.Icon = (DrawingImage)this.Resources["MetroNewItemSelected"];
                }
                else if (button == backStageOpenButton)
                {
                    this.backStageOpenButton.Icon = (DrawingImage)this.Resources["MetroFolderOpenSelected"];
                }
                else if (button == backStageSaveButton)
                {
                    this.backStageSaveButton.Icon = (DrawingImage)this.Resources["MetroSaveSelected"];
                }
                else if (button == backStageSaveAsButton)
                {
                    this.backStageSaveAsButton.Icon = (DrawingImage)this.Resources["MetroSaveAsSelected"];
                }
                else if (button == backStageExitButton)
                {
                    this.backStageExitButton.Icon = (DrawingImage)this.Resources["MetroExitSelected"];
                }
            }
            catch
            {

            }
        }

        private void backStageButton_MouseLeave(object sender, MouseEventArgs e)
        {
            this.backStageNewButton.Icon = (DrawingImage)this.Resources["MetroNewItem"];
            this.backStageOpenButton.Icon = (DrawingImage)this.Resources["MetroFolderOpen"];
            this.backStageSaveButton.Icon = (DrawingImage)this.Resources["MetroSave"];
            this.backStageSaveAsButton.Icon = (DrawingImage)this.Resources["MetroSaveAs"];
            this.backStageExitButton.Icon = (DrawingImage)this.Resources["MetroExit"];
        }

        void buttonPrintSetup_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            this.buttonPrintSetup.IsSelected = true;
            designerPanelControl.reportViewerControl.buttonPageSetup_Click(sender, e);
            this.buttonPrintSetup.IsSelected = false;
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            this.buttonRefresh.IsSelected = true;

            try
            {

                designerPanelControl.reportViewerControl.ReportModel.ExceptionDetails.Clear();
                designerPanelControl.reportViewerControl.Refresh();
                this.buttonRefresh.IsSelected = false;

                if (this.VisualStyle == "Metro")
                {
                    this.UpdateMetroImages();
                }
            }
            catch (Exception ex)
            {
                designerPanelControl.reportViewerControl.ShowException(ex);
            }
        }

        private void ButtonFirst_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            this.buttonFirst.IsSelected = true;

            try
            {
                designerPanelControl.reportViewerControl.MoveFirst();
            }
            catch (Exception ex)
            {
                designerPanelControl.reportViewerControl.ShowException(ex);
            }
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            this.buttonPrevious.IsSelected = true;

            try
            {
                designerPanelControl.reportViewerControl.MovePrevious();
            }
            catch (Exception ex)
            {
                designerPanelControl.reportViewerControl.ShowException(ex);
            }
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            this.buttonNext.IsSelected = true;

            try
            {
                designerPanelControl.reportViewerControl.MoveNext();
            }
            catch (Exception ex)
            {
                designerPanelControl.reportViewerControl.ShowException(ex);
            }
        }

        private void ButtonLast_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            this.buttonLast.IsSelected = true;

            try
            {
                designerPanelControl.reportViewerControl.MoveLast();
            }
            catch (Exception ex)
            {
                designerPanelControl.reportViewerControl.ShowException(ex);
            }
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            this.buttonPrint.IsSelected = true;

            try
            {
                designerPanelControl.reportViewerControl.Print();
                this.buttonPrint.IsSelected = false;

                if (this.VisualStyle == "Metro")
                {
                    this.UpdateMetroImages();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
            }
        }

        private void Btn_ViewDesign_Click(object sender, RoutedEventArgs e)
        {
            this.ViewReportDesigner();
        }

        private void ViewReportDesigner()
        {
            if (isReportRun)
            {
                isReportRun = false;
                this.internalDockStateChange = true;
                DockingManager.SetState(this.grd_ReportData, this.viewerReportDataDockState);
                DockingManager.SetState(this.grd_PropertyWindow, this.viewerPropertiesDockState);
                DockingManager.SetState(this.grd_GroupingWindow, this.viewerGroupingDockState);
                this.internalDockStateChange = false;
            }

            designerPanelControl.OpenDesigner();

            this.ribbonTabInsert.Visibility = Visibility.Visible;
            this.ribbonTabView.Visibility = Visibility.Visible;
            this.ribbonTabRunReport.Visibility = Visibility.Collapsed;
            this.ribbonTabRunReport.IsChecked = false;
            this.ribbonTabInsert.IsChecked = true;

            if (this.ShowZoom)
            {
                this.GridRoot.RowDefinitions[2].Height = new GridLength(25);
                this.grd_Zooming.Visibility = Visibility.Visible;
            }
        }

        private void Btn_ViewReport_Click(object sender, RoutedEventArgs e)
        {
            this.ViewReportViewer();
        }

        private void ViewReportViewer()
        {
            isReportRun = true;

            if (this.ShowZoom)
            {
                this.grd_Zooming.Visibility = Visibility.Collapsed;
                this.GridRoot.RowDefinitions[2].Height = new GridLength(0);
            }

            this.viewerReportDataDockState = DockingManager.GetState(this.grd_ReportData);
            this.viewerPropertiesDockState = DockingManager.GetState(this.grd_PropertyWindow);
            this.viewerGroupingDockState = DockingManager.GetState(this.grd_GroupingWindow);

            this.internalDockStateChange = true;
            DockingManager.SetState(this.grd_ReportData, DockState.Hidden);
            DockingManager.SetState(this.grd_PropertyWindow, DockState.Hidden);
            DockingManager.SetState(this.grd_GroupingWindow, DockState.Hidden);
            this.internalDockStateChange = false;

            this.ribbonTabInsert.Visibility = Visibility.Collapsed;
            this.ribbonTabView.Visibility = Visibility.Collapsed;
            this.ribbonTabRunReport.Visibility = Visibility.Visible;
            this.ribbonTabRunReport.IsChecked = true;

            if (designerPanelControl.reportViewerControl.HasReportParameters)
            {
                this.buttonParameters.IsEnabled = true;
                this.isButtonParameterChecked = true;
            }
            else
            {
                this.buttonParameters.IsEnabled = false;
                this.isButtonParameterChecked = false;
            }

            designerPanelControl.OpenViewer();
        }

        public void buttonParameters_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.isButtonParameterChecked)
            {
                designerPanelControl.reportViewerControl.ParametersBlock(true);
                this.isButtonParameterChecked = false;
            }
            else
            {
                this.buttonParameters.IsSelected = true;
                this.isButtonParameterChecked = true;
                designerPanelControl.reportViewerControl.ParametersBlock(false);
            }
        }

        #endregion

        #region Tabbing

        void TabControl_OnCloseOtherTabs(object sender, CloseTabEventArgs e)
        {
            try
            {
                bool isClosed = this.PreCloseApplication(e.TargetTabItem);

                if (isClosed == false)
                {
                    e.Cancel = true;
                }
                else
                {
                    ReportInformation reportInformationTemp = designerPanelControl.GetReportInformationOnKey(Convert.ToDouble(e.TargetTabItem.Tag));
                    designerPanelControl.ReportsCollection.Clear();
                    designerPanelControl.ReportsCollection.Add(reportInformationTemp);
                }
            }
            catch 
            {
            }
        }

        void TabControl_OnCloseAllTabs(object sender, CloseTabEventArgs e)
        {
            try
            {
                bool isClosed = this.PreCloseApplication();

                if (isClosed == false)
                {
                    e.Cancel = true;
                }
                else
                {
                    designerPanelControl.ReportsCollection.Clear();
                    this.RaiseAllReportsClosed();
                }
            }
            catch
            {
            }
        }


        #endregion

        #region Ribbon Menus - Events

        #region Tab Insert - Events

        void btn_matrix_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                if (this.btn_matrix.IsDropDownOpen)
                {
                    UpdateMetroImages();
                    this.btn_matrix.LargeIcon = (DrawingImage)this.Resources["MetroMatrixSelected"];
                }
                else
                {
                    this.btn_matrix.LargeIcon = (DrawingImage)this.Resources["MetroMatrix"];
                }
            }
        }

        void btn_table_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                if (this.btn_table.IsDropDownOpen)
                {
                    UpdateMetroImages();
                    this.btn_table.LargeIcon = (DrawingImage)this.Resources["MetroTablesSelected"];
                }
                else
                {
                    this.btn_table.LargeIcon = (DrawingImage)this.Resources["MetroTables"];
                }
            }
        }

        void btn_chart_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                if (this.btn_chart.IsDropDownOpen)
                {
                    UpdateMetroImages();
                    this.btn_chart.LargeIcon = (DrawingImage)this.Resources["MetroChartSelected"];
                }
                else
                {
                    this.btn_chart.LargeIcon = (DrawingImage)this.Resources["MetroChart"];
                }
            }
        }

        void btn_Footer_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                if (this.btn_Footer.IsDropDownOpen)
                {
                    UpdateMetroImages();
                    this.btn_Footer.LargeIcon = (DrawingImage)this.Resources["MetroFooterSelected"];
                }
                else
                {
                    this.btn_Footer.LargeIcon = (DrawingImage)this.Resources["MetroFooter"];
                }
            }
        }

        void btn_Header_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                if (this.btn_Header.IsDropDownOpen)
                {
                    UpdateMetroImages();
                    this.btn_Header.LargeIcon = (DrawingImage)this.Resources["MetroHeaderSelected"];
                }
                else
                {
                    this.btn_Header.LargeIcon = (DrawingImage)this.Resources["MetroHeader"];
                }
            }
        }

        private void Btn_InsertTable_Click(object sender, RoutedEventArgs e)
        {
            UpdateButtonStatus();
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.Tablix);
        }

        private void Btn_TableWizard_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            designerPanelControl.ShowWizard(ReportItemWizard.Table);
        }

        private void Btn_MatrixWizard_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            designerPanelControl.ShowWizard(ReportItemWizard.Table);
        }

        private void Btn_InsertChart_Click(object sender, RoutedEventArgs e)
        {
            UpdateButtonStatus();
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.Chart);
        }

        private void Btn_ChartWizard_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonStatus();
            designerPanelControl.ShowWizard(ReportItemWizard.Chart);
        }

        private void btn_DataBar_Click(object sender, RoutedEventArgs e)
        {
            //UpdateButtonStatus();
            this.btn_DataBar.IsSelected = true;
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.DataBar);
        }

        private void btn_DataBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                UpdateMetroImages();
                this.btn_DataBar.LargeIcon = (DrawingImage)this.Resources["MetroDatabarSelected"];
            }
        }

        private void btn_Map_Click(object sender, RoutedEventArgs e)
        {
            //UpdateButtonStatus();
            this.btn_Map.IsSelected = true;
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.Map);
        }

        private void btn_Map_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                UpdateMetroImages();
                this.btn_Map.LargeIcon = (DrawingImage)this.Resources["MetroMapSelected"];
            }
        }
        private void btn_SparkLine_Click(object sender, RoutedEventArgs e)
        {
            //UpdateButtonStatus();
            this.btn_SparkLine.IsSelected = true;
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.Sparkline);
        }

        private void btn_SparkLine_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                UpdateMetroImages();
                this.btn_SparkLine.LargeIcon = (DrawingImage)this.Resources["MetroSparklineSelected"];
            }
        }

        void Btn_Gauge_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                UpdateMetroImages();
                this.btn_Gauge.LargeIcon = (DrawingImage)this.Resources["MetroGaugeSelected"];
            }
        }

        private void Btn_Gauge_Click(object sender, RoutedEventArgs e)
        {
            this.btn_Gauge.IsSelected = true;
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.Gauge);
        }

        void Btn_List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                UpdateMetroImages();
                this.btn_List.LargeIcon = (DrawingImage)this.Resources["MetroListSelected"];
            }
        }

        private void Btn_List_Click(object sender, RoutedEventArgs e)
        {
            this.btn_List.IsSelected = true;
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.List);

        }

        void Btn_TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                UpdateMetroImages();
                this.btn_TextBox.LargeIcon = (DrawingImage)this.Resources["MetroTextboxSelected"];
            }
        }

        private void Btn_TextBox_Click(object sender, RoutedEventArgs e)
        {
            this.btn_TextBox.IsSelected = true;
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.TextBox);
        }

        void Btn_Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                UpdateMetroImages();
                this.btn_Image.LargeIcon = (DrawingImage)this.Resources["MetroImageSelected"];
            }
        }

        private void Btn_Image_Click(object sender, RoutedEventArgs e)
        {
            this.btn_Image.IsSelected = true;
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.Image);
        }

        void Btn_Line_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                UpdateMetroImages();
                this.btn_Line.LargeIcon = (DrawingImage)this.Resources["MetroLineSelected"];
            }
        }

        private void Btn_Line_Click(object sender, RoutedEventArgs e)
        {
            this.btn_Line.IsSelected = true;
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.Line);
        }

        void Btn_Rectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                UpdateMetroImages();
                this.btn_Rectangle.LargeIcon = (DrawingImage)this.Resources["MetroRectangleSelected"];
            }
        }

        private void Btn_Rectangle_Click(object sender, RoutedEventArgs e)
        {
            this.btn_Rectangle.IsSelected = true;
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.Rectangle);
        }

        void Btn_Subreport_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateButtonStatus();

            if (this.VisualStyle == "Metro")
            {
                UpdateMetroImages();
                this.btn_Subreport.LargeIcon = (DrawingImage)this.Resources["MetroSubreportSelected"];
            }
        }

        private void Btn_Subreport_Click(object sender, RoutedEventArgs e)
        {
            this.btn_Subreport.IsSelected = true;
            designerPanelControl.UpdateDesignPanelReportItem(DrawingReportItem.SubReport);
        }

        private void Btn_AddHeader_Click(object sender, RoutedEventArgs e)
        {
            DesignPanel latestDesignPanel = designerPanelControl.GetDisplayTabDesignPanel();
            latestDesignPanel.SetHeaderVisibility();
            this.UpdateHeaderFooterTab();
        }

        internal void UpdateHeaderFooterTab()
        {
            DesignPanel latestDesignPanel = designerPanelControl.GetDisplayTabDesignPanel();

            if (latestDesignPanel.IsHeaderVisible)
            {
                btn_AddHeader.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerRemoveHeader");// "Remove Header";
            }
            else
            {
                btn_AddHeader.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddHeader");// "Add Header";
            }

            if (latestDesignPanel.IsFooterVisible)
            {
                btn_AddFooter.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerRemoveFooter");// "Remove Footer";                
            }
            else
            {
                btn_AddFooter.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddFooter"); // "Add Footer";
            }
        }

        private void Btn_AddFooter_Click(object sender, RoutedEventArgs e)
        {
            DesignPanel latestDesignPanel = designerPanelControl.GetDisplayTabDesignPanel();
            latestDesignPanel.SetFooterVisibility();
            this.UpdateHeaderFooterTab();
        }

        #endregion

        #region Tab View - Events

        private void Chk_ReportData_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ShowReportData = false;
        }

        private void Chk_ReportData_Checked(object sender, RoutedEventArgs e)
        {
            this.ShowReportData = true;
        }

        void chk_Ruler_Unchecked(object sender, RoutedEventArgs e)
        {
            designerPanelControl.ShowRuler = false;
        }

        void chk_Ruler_Checked(object sender, RoutedEventArgs e)
        {
            designerPanelControl.ShowRuler = true;
        }


        void chk_Grouping_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ShowGrouping = false;
        }

        void chk_Grouping_Checked(object sender, RoutedEventArgs e)
        {
            this.ShowGrouping = true;
        }

        #endregion

        #endregion

        #region File

        #region New / Close File

        public bool PreCloseApplication()
        {
            return PreCloseApplication(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>bool is cancled</returns>
        private bool PreCloseApplication(TabItemExt excludeTab)
        {
            return designerPanelControl.CloseApplication(excludeTab);
        }

        private void SimpleMenuNew_Click(object sender, RoutedEventArgs e)
        {
            this.RibbonToolBar.HideBackStage();
            this.backStageNewButton.Icon = (DrawingImage)this.Resources["MetroNewItem"];
            this.NewReport();
        }

        public void NewReport()
        {
            designerPanelControl.NewReport();
        }

        #endregion

        #region Save File

        private void SimpleMenuSave_Click(object sender, RoutedEventArgs e)
        {
            this.RibbonToolBar.IsHitTestVisible = false;
            this.RibbonToolBar.HideBackStage();
            this.backStageSaveButton.Icon = (DrawingImage)this.Resources["MetroSave"];
            designerPanelControl.SaveDialog(SaveFileType.Save);
            this.RibbonToolBar.IsHitTestVisible = true;
        }

        private void SimpleMenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            this.RibbonToolBar.IsHitTestVisible = false;
            this.RibbonToolBar.HideBackStage();
            this.backStageSaveAsButton.Icon = (DrawingImage)this.Resources["MetroSaveAs"];
            designerPanelControl.SaveDialog(SaveFileType.SaveAs);
            this.RibbonToolBar.IsHitTestVisible = true;
        }

        private void SimpleMenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.RibbonToolBar.HideBackStage();
            this.backStageExitButton.Icon = (DrawingImage)this.Resources["MetroExit"];

            if (this.PreCloseApplication())
            {
                Application.Current.Shutdown();
            }
        }

        public void SaveReportDialogue()
        {
            designerPanelControl.SaveDialog(SaveFileType.Save);
        }

        public void SaveAsReportDialogue()
        {
            designerPanelControl.SaveDialog(SaveFileType.SaveAs);
        }

        public void SaveReport(string reportPath)
        {
            designerPanelControl.SaveReport(reportPath);
        }

        public void SaveReport(Stream stream)
        {
            designerPanelControl.SaveReport(stream);
        }

        public Stream GetReportStream()
        {
            return designerPanelControl.GetReportStream();
        }
        #endregion

        #region Open File

        private void SimpleMenuOpen_Click(object sender, RoutedEventArgs e)
        {
            this.RibbonToolBar.IsHitTestVisible = false;
            this.RibbonToolBar.HideBackStage();
            this.backStageOpenButton.Icon = (DrawingImage)this.Resources["MetroFolderOpen"];
            this.OpenReportDialog();
            this.RibbonToolBar.IsHitTestVisible = true;
        }

        public void OpenReportDialog()
        {
            designerPanelControl.OpenReportDialog();
        }

        public void OpenReport(string reportPath)
        {
            designerPanelControl.OpenReport(reportPath);
        }

        internal void UpdateStyle()
        {
            if (this.VisualStyle == "Metro")
            {
                this.UpdateMetroImages();
            }
            else
            {
                this.btn_Footer.LargeIcon = (BitmapImage)this.Resources["Footer"];
                this.btn_Header.LargeIcon = (BitmapImage)this.Resources["Header"];
                this.btn_Gauge.LargeIcon = (BitmapImage)this.Resources["Gauge"];
                this.btn_DataBar.LargeIcon = (BitmapImage)this.Resources["Databar"];
                this.btn_Map.LargeIcon = (BitmapImage)this.Resources["Map"];
                this.btn_SparkLine.LargeIcon = (BitmapImage)this.Resources["Sparkline"];
                this.btn_chart.LargeIcon = (BitmapImage)this.Resources["Chart"];
                this.btn_TextBox.LargeIcon = (BitmapImage)this.Resources["TextBox"];
                this.btn_table.LargeIcon = (BitmapImage)this.Resources["Tables"];
                this.btn_Subreport.LargeIcon = (BitmapImage)this.Resources["Subreport"];
                this.btn_Rectangle.LargeIcon = (BitmapImage)this.Resources["Rectangle"];
                this.btn_matrix.LargeIcon = (BitmapImage)this.Resources["Matrix"];
                this.btn_List.LargeIcon = (BitmapImage)this.Resources["List"];
                this.btn_Line.LargeIcon = (BitmapImage)this.Resources["Line"];
                this.btn_Image.LargeIcon = (BitmapImage)this.Resources["Image"];
                this.buttonFirst.LargeIcon = (BitmapImage)this.Resources["First_NavDisabled"];
                this.buttonPrevious.LargeIcon = (BitmapImage)this.Resources["Previous_NavDisabled"];
                this.buttonNext.LargeIcon = (BitmapImage)this.Resources["Next_NavDisabled"];
                this.buttonLast.LargeIcon = (BitmapImage)this.Resources["Last_NavDisabled"];
                this.buttonPrint.LargeIcon = (BitmapImage)this.Resources["Print"];
                this.buttonPrintLayout.LargeIcon = (BitmapImage)this.Resources["PrintLayoutX"];
                this.buttonDocumentMap.SmallIcon = (BitmapImage)this.Resources["Documentmap"];
                this.buttonParameters.SmallIcon = (BitmapImage)this.Resources["Parameter"];
                this.buttonStop.SmallIcon = (BitmapImage)this.Resources["CloseReport"];
                this.buttonUp.SmallIcon = (BitmapImage)this.Resources["UpReport"];
                this.buttonRefresh.SmallIcon = (BitmapImage)this.Resources["RefreshReport"];
                this.btn_ViewDesign.LargeIcon = (BitmapImage)this.Resources["ReportDesign"];
                this.btn_ViewReport.LargeIcon = (BitmapImage)this.Resources["ReportRun"];
            }

            designerPanelControl.VisualStyle = this.VisualStyle;
        }

        internal void UpdateMetroImages()
        {
            this.btn_Footer.LargeIcon = (DrawingImage)this.Resources["MetroFooter"];
            this.btn_Header.LargeIcon = (DrawingImage)this.Resources["MetroHeader"];
            this.btn_Gauge.LargeIcon = (DrawingImage)this.Resources["MetroGauge"];
            this.btn_chart.LargeIcon = (DrawingImage)this.Resources["MetroChart"];
            this.btn_Map.LargeIcon = (DrawingImage)this.Resources["MetroMap"];
            this.btn_DataBar.LargeIcon = (DrawingImage)this.Resources["MetroDatabar"];
            this.btn_SparkLine.LargeIcon = (DrawingImage)this.Resources["MetroSparkline"];
            this.btn_TextBox.LargeIcon = (DrawingImage)this.Resources["MetroTextbox"];
            this.btn_table.LargeIcon = (DrawingImage)this.Resources["MetroTables"];
            this.btn_Subreport.LargeIcon = (DrawingImage)this.Resources["MetroSubreport"];
            this.btn_Rectangle.LargeIcon = (DrawingImage)this.Resources["MetroRectangle"];
            this.btn_matrix.LargeIcon = (DrawingImage)this.Resources["MetroMatrix"];
            this.btn_List.LargeIcon = (DrawingImage)this.Resources["MetroList"];
            this.btn_Line.LargeIcon = (DrawingImage)this.Resources["MetroLine"];
            this.btn_Image.LargeIcon = (DrawingImage)this.Resources["MetroImage"];
            this.buttonExport.LargeIcon = (DrawingImage)this.Resources["MetroExport"];
            this.buttonFirst.LargeIcon = (DrawingImage)this.Resources["MetroFirst_NavDisabled"];
            this.buttonPrevious.LargeIcon = (DrawingImage)this.Resources["MetroPrevious_NavDisabled"];
            this.buttonNext.LargeIcon = (DrawingImage)this.Resources["MetroNext_NavDisabled"];
            this.buttonLast.LargeIcon = (DrawingImage)this.Resources["MetroLast_NavDisabled"];
            this.buttonPrint.LargeIcon = (DrawingImage)this.Resources["MetroPrint"];
            this.buttonPrintLayout.LargeIcon = (DrawingImage)this.Resources["MetroPrintLayout"];
            this.buttonDocumentMap.SmallIcon = (BitmapImage)this.Resources["MetroDocumentMap"];
            this.buttonParameters.SmallIcon = (DrawingImage)this.Resources["MetroParameter"];
            this.buttonStop.SmallIcon = (DrawingImage)this.Resources["MetroStop"];
            this.buttonUp.SmallIcon = (DrawingImage)this.Resources["MetroUp"];
            this.buttonRefresh.SmallIcon = (DrawingImage)this.Resources["MetroRefresh"];
            this.btn_ViewDesign.LargeIcon = (DrawingImage)this.Resources["MetroDesign"];
            this.btn_ViewReport.LargeIcon = (DrawingImage)this.Resources["MetroRun"];
            this.backStageNewButton.Icon = (DrawingImage)this.Resources["MetroNewItem"];
            this.backStageOpenButton.Icon = (DrawingImage)this.Resources["MetroFolderOpen"];
            this.backStageSaveButton.Icon = (DrawingImage)this.Resources["MetroSave"];
            this.backStageSaveAsButton.Icon = (DrawingImage)this.Resources["MetroSaveAs"];
            this.backStageExitButton.Icon = (DrawingImage)this.Resources["MetroExit"];
        }
        #endregion

        #endregion

        #region Custom Event Methods

        public void ShowWizard(ReportItemWizard wizard)
        {
            this.UpdateButtonStatus();

            if (wizard == ReportItemWizard.Chart)
            {
                designerPanelControl.GetDisplayTabDesignPanel().AddChartThroughWizard(RESX.titleNewChart);
            }
            else if (wizard == ReportItemWizard.Table)
            {
                designerPanelControl.GetDisplayTabDesignPanel().AddTablixThroughWizard(RESX.titleNewTable);
            }
        }

        internal void RaiseOpenReportChanged(string reportName)
        {
            if (this.ReportOpened != null)
            {
                this.ReportOpened(this, new ReportChangedEventArgs { ReportName = reportName });
            }
        }

        public event ReportChangedEventHandler ReportSaved;

        internal void RaiseReportSaved(string reportName)
        {
            if (this.ReportSaved != null)
            {
                this.ReportSaved(this, new ReportChangedEventArgs { ReportName = reportName });
            }
        }

        internal void RaiseNewReportOpened(string reportName)
        {
            if (this.NewReportOpened != null)
            {
                this.NewReportOpened(this, new ReportChangedEventArgs { ReportName = reportName });
            }
        }

        public event AllReportsClosedEventHandler AllReportsClosed;

        internal void RaiseAllReportsClosed()
        {
            if (this.AllReportsClosed != null)
            {
                this.AllReportsClosed(this, new AllReportsClosedEventArgs { });
            }
        }

        #endregion

        private void RibbonToolBar_RibbonContextMenuOpening(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void RibbonToolBar_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void RibbonToolBar_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        #region DataSource/DataSet Method

        public string AddDataSet(string reportPath, string reportDataSetName)
        {
            return this.designerPanelControl.AddDataSet(reportPath, reportDataSetName);
        }

        public string AddDataSet(Stream reportStream, string reportDataSetName)
        {
            return this.designerPanelControl.AddDataSet(reportStream, reportDataSetName);
        }

        public string AddDataSource(string reportPath, string reportDataSourceName)
        {
            return this.designerPanelControl.AddDataSource(reportPath, reportDataSourceName);
        }

        public string AddDataSource(Stream reportStream, string reportDataSourceName)
        {
            return this.designerPanelControl.AddDataSource(reportStream, reportDataSourceName);
        }

        #endregion

        private void btn_zoomin_Click(object sender, RoutedEventArgs e)
        {
            double zoom = Math.Round(this.ZoomFactor + 25);
            if (zoom > 400)
            {
                this.ZoomFactor = 400;
            }
            else
            {
                this.ZoomFactor = zoom;
            }
        }

        private void btn_zoomout_Click(object sender, RoutedEventArgs e)
        {
            double zoom = Math.Round(this.ZoomFactor - 25);
            if (zoom < 25)
            {
                this.ZoomFactor = 25;
            }
            else
            {
                this.ZoomFactor = zoom;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue != 25 && e.OldValue != e.NewValue)
                this.ZoomFactor = Math.Round(e.NewValue);
        }
    }
}
