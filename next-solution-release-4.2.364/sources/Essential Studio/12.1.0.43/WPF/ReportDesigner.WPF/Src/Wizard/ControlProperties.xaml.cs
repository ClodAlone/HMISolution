//-------------------------------------------------------------------------------------------------
// <copyright file="ControlProperties.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

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
using System.ComponentModel;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using Syncfusion.RDL.DOM;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using Syncfusion.Windows.Reports.Sql;
using System.IO;
using RESX= Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Syncfusion.RDL.Internal;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

#if !SyncfusionFramework3_5
using Syncfusion.UI.Xaml.Maps;
#endif

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for Essential WPF RibbonWindow1.xaml
    /// </summary>
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    internal partial class ControlProperties : ChromelessWindow
    {
        #region Members

        private bool reportParameterIsCorrect = true;

        private bool isReportParameter = false;

        private bool isReport = false;

        private bool isValidValue = false;
        string error_title = "";

        string Error_title
        {
            get
            {
                if (string.IsNullOrEmpty(error_title))
                {
                    error_title = SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner");
                }
                return error_title;
            }
        }

        #endregion

        #region Private properties

        internal static RDL.DOM.ReportUnitType UnitType { get; set; }

        internal EmbeddedImages CachedEmbeddedImages { get; set; }

        internal Parameters Parameters { get; set; }

        internal RDL.DOM.SortExpressions SortExpressions { get; set; }

        internal RDL.DOM.Filters Filters { get; set; }

        private ReportParameters ReportParameters { get; set; }

        private DataSets DataSets { get; set; }

        private EmbeddedImages EmbeddedImages { get; set; }

        private ReportItem ReportItem { get; set; }

        private UIElement CurrentUIElement { get; set; }

        private string CurrentElement = string.Empty;

        internal ReportDefinition Report;

        private EmbeddedImage CurrentEmbeddedImage { get; set; }

        private Body ReportBody { get; set; }

        private string[] ReportParameterNames { get; set; }

        private ImageSourceType ImageSourceType { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlProperties"/> class.
        /// </summary>
        public ControlProperties()
        {
            try
            {
                InitializeComponent();
                CurrentUIElement = new System.Windows.Controls.TextBox();
                lbox_NodeType.SelectedItem = lbox_NodeType.Items[0];
                // lbox_NodeType.SelectionChanged += new SelectionChangedEventHandler(lbox_NodeType_SelectionChanged);
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        #region Sub Report

        public ControlProperties(SubReportControl subReportControl)
        {
            try
            {
                InitializeComponent();
                this.Title =  SR.GetString(CultureInfo.CurrentUICulture,"titleSubReportProperties");
                this.CurrentUIElement = subReportControl;
                this.SubReportGeneral = new SubReportGeneral();
                this.SubReportVisibility = new SubReportVisibility();
                this.SubReportVisibility.Visibility = System.Windows.Visibility.Hidden;
                this.SubReportBorder = new SubReportBorder();
                this.SubReportBorder.Visibility = System.Windows.Visibility.Hidden;
                AddTab(ControlPropertyType.General, SubReportGeneral);
                AddTab(ControlPropertyType.Visibility, SubReportVisibility);
                AddTab(ControlPropertyType.Border, SubReportBorder);
               // PopulateSubReportFromReport(subReportControl);
                EventInitialize();
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        private void UpdateHelpButton()
        {
            if (ReportDesignView.CurrentPanel != null)
            {
                if (!ReportDesignView.CurrentPanel.ShowHelp)
                {
                    this.btn_HelponControlProperties.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Chart

        /// <param name="child">The child.</param>
        public ControlProperties(ChartControl chartControl, ReportDefinition reportSettings, ChartChild child)
        {
            //try
            //{
                if (child == ChartChild.Chart)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"headerChartProperties");
                    this.CurrentUIElement = chartControl;
                    this.ChartGeneral = new ChartGeneral();
                    if (chartControl.ChartControlType == ChartControlType.DataBar)
                    {
                        ChartGeneral.ComboChartTypes.ComboType = Editors.ValueType.DataBarType;
                    }
                    else if (chartControl.ChartControlType == ChartControlType.Sparkline)
                    {
                        ChartGeneral.ComboChartTypes.ComboType = Editors.ValueType.SparklineType;
                    }
                    else
                    {
                        ChartGeneral.ComboChartTypes.ComboType = Editors.ValueType.ChartType;
                    }
                    this.ChartBorder = new ChartBorder();
                    this.ChartBorder.Visibility = System.Windows.Visibility.Hidden;
                    this.ChartBackground = new ChartBackground();
                    this.ChartBackground.Visibility = System.Windows.Visibility.Hidden;
                    this.ChartAreaBackground = new ChartAreaBackground();
                    this.ChartAreaBackground.Visibility = System.Windows.Visibility.Hidden;
                    AddTab(ControlPropertyType.General, ChartGeneral);
                    AddTab(ControlPropertyType.Border, ChartBorder);
                    AddTab(ControlPropertyType.Fill, ChartBackground);
                    AddTab(ControlPropertyType.AreaColor, ChartAreaBackground);
                }

               else if (child == ChartChild.Series)
                {
                    InitializeComponent();
                    this.SeriesGeneral = new SeriesGeneral1();
                    this.SeriesAxes=new SeriesAxes();
                    this.SeriesAxes.Visibility = System.Windows.Visibility.Hidden;
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleSeriesProperties");
                    this.CurrentUIElement = chartControl;
                    AddTab(ControlPropertyType.General, SeriesGeneral);
                    AddTab(ControlPropertyType.Axes, SeriesAxes);
                }

               else if (child == ChartChild.Legend)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleLegendProperties");
                    this.CurrentUIElement = chartControl;
                    this.LegendGeneral = new LegendGeneral();
                    this.LegendFont = new LegendFont();
                    this.LegendFont.Visibility = System.Windows.Visibility.Hidden;
                    this.LegendBorder = new LegendBorder();
                    this.LegendBorder.Visibility = System.Windows.Visibility.Hidden;
                    AddTab(ControlPropertyType.General, LegendGeneral);
                    AddTab(ControlPropertyType.Font, LegendFont);
                    AddTab(ControlPropertyType.Border, LegendBorder);
                }

               else if (child == ChartChild.ValueAxis)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleValueAxisProperties");
                    this.CurrentUIElement = chartControl;
                    this.AxisValueGeneral = new AxisValueGeneral();
                    this.AxisValueLabel = new AxisValueLabel();
                    this.AxisValueLabel.Visibility = System.Windows.Visibility.Hidden;
                    this.AxisValueTickMarks = new AxisValueTickMarks();
                    this.AxisValueTickMarks.Visibility = System.Windows.Visibility.Hidden;
                    AddTab(ControlPropertyType.General, AxisValueGeneral);
                    AddTab(ControlPropertyType.Label, AxisValueLabel);
                    AddTab(ControlPropertyType.Tick, AxisValueTickMarks);
                }

               else if (child == ChartChild.CategoryAxis)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleCategoryAxisProperties");
                    this.CurrentUIElement = chartControl;
                    this.AxisCategoryGeneral = new AxisCategoryGeneral();
                    this.AxisCategoryLabel = new AxisCategoryLabel();
                    this.AxisCategoryLabel.Visibility = System.Windows.Visibility.Hidden;
                    this.AxisCategoryTickMarks = new AxisCategoryTickMarks();
                    this.AxisCategoryTickMarks.Visibility = System.Windows.Visibility.Hidden;
                    AddTab(ControlPropertyType.General, AxisCategoryGeneral);
                    AddTab(ControlPropertyType.Label, AxisCategoryLabel);
                    AddTab(ControlPropertyType.Tick, AxisCategoryTickMarks);
                }

               else if (child == ChartChild.ChartTitle)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleChartTitleProperties");
                    this.CurrentUIElement = chartControl;
                    this.ChartTitle = new Syncfusion.Windows.Reports.Designer.Dialogs.ChartTitle();
                    AddTab(ControlPropertyType.General, ChartTitle);
                }

               else if (child == ChartChild.CategoryTitle)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleCategoryAxisTitleProperties");
                    this.CurrentUIElement = chartControl;
                    this.CategoryAxisTitle = new CategoryAxisTitle();
                    AddTab(ControlPropertyType.General, CategoryAxisTitle);
                }

               else if (child == ChartChild.ValueTitle)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleValueAxisTitleProperties");
                    this.CurrentUIElement = chartControl;
                    this.ValueAxisTitle = new ValueAxisTitle();
                    AddTab(ControlPropertyType.General, ValueAxisTitle);
                }

                else if (child == ChartChild.SecondaryCategoryTitle)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleSecondaryCategoryAxisTitleProperties");
                    this.CurrentUIElement = chartControl;
                    this.SecondaryCategoryAxisTitle = new SecondaryCategoryAxisTitle();
                    AddTab(ControlPropertyType.General, SecondaryCategoryAxisTitle);
                }

                else if (child == ChartChild.SecondaryValueTitle)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleSecondaryValueAxisTitleProperties");
                    this.CurrentUIElement = chartControl;
                    this.SecondaryValueAxisTitle = new SecondaryValueAxisTitle();
                    AddTab(ControlPropertyType.General, SecondaryValueAxisTitle);
                }

                EventInitialize();
            //}
            //catch(Exception e)
            //{
            //    MessageBox.Show("Unexpected error occurs");
            //}
        }

        #endregion

        #region Gauge

        public ControlProperties(GaugeControl gaugeControl, ReportDefinition reportSettings, GaugeChild child)
        {
            try
            {
                if (child == GaugeChild.GaugeScale)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleScaleProperties");
                    this.CurrentUIElement = gaugeControl;
                    this.ScaleGeneral = new ScaleGeneral();
                    this.ScaleLabel = new ScaleLabel();
                    this.ScaleLabel.Visibility = System.Windows.Visibility.Hidden;
                    this.ScaleMajorTick = new ScaleMajorTick();
                    this.ScaleMajorTick.Visibility = System.Windows.Visibility.Hidden;
                    this.ScaleMinorTick = new ScaleMinorTick();
                    this.ScaleMinorTick.Visibility = System.Windows.Visibility.Hidden;
                    AddTab(ControlPropertyType.General, ScaleGeneral);
                    AddTab(ControlPropertyType.Label, ScaleLabel);
                    AddTab(ControlPropertyType.MajorTick, ScaleMajorTick);
                    AddTab(ControlPropertyType.MinorTick, ScaleMinorTick);
                }

                else if (child == GaugeChild.GaugePointer)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titlePointerProperties");
                    this.CurrentUIElement = gaugeControl;
                    this.PointerGeneral = new PointerGeneral();
                    this.PointerColorFill = new PointerColorFill();
                    this.PointerColorFill.Visibility = System.Windows.Visibility.Hidden;
                    AddTab(ControlPropertyType.General, PointerGeneral);
                    AddTab(ControlPropertyType.Fill, PointerColorFill);
                }

                else if (child == GaugeChild.GaugeRange)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleRangeProperties");
                    this.CurrentUIElement = gaugeControl;
                    this.RangeGeneral = new RangeGeneral();
                    this.RangeBorder = new RangeBorder();
                    this.RangeBorder.Visibility = System.Windows.Visibility.Hidden;
                    AddTab(ControlPropertyType.General, RangeGeneral);
                    AddTab(ControlPropertyType.Border, RangeBorder);
                }

                else if (child == GaugeChild.GaugeProperties)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleGaugeProperties");
                    this.CurrentUIElement = gaugeControl;
                    this.GaugeGeneral = new GaugeGeneral();
                    AddTab(ControlPropertyType.General, GaugeGeneral);
                }

                else if (child == GaugeChild.GaugePanel)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleGaugePanelProperties");
                    this.CurrentUIElement = gaugeControl;
                    this.GaugePanelGeneral = new PanelGeneral();
                    this.GaugePanelVisibility = new PanelVisibility();
                    this.GaugePanelVisibility.Visibility = System.Windows.Visibility.Hidden;
                    this.GaugePanelFill = new PanelFill();
                    this.GaugePanelFill.Visibility = System.Windows.Visibility.Hidden;
                    this.GaugePanelBorder = new PanelBorder();
                    this.GaugePanelBorder.Visibility = System.Windows.Visibility.Hidden;
                    AddTab(ControlPropertyType.General, GaugePanelGeneral);
                    AddTab(ControlPropertyType.Visibility, GaugePanelVisibility);
                    AddTab(ControlPropertyType.Fill, GaugePanelFill);
                    AddTab(ControlPropertyType.Border, GaugePanelBorder);
                }

                //populateGaugeFromReport(gaugeControl, reportSettings);
                EventInitialize();
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

       #endregion

        #region ReportParameter

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlProperties"/> class.
        /// </summary>
        /// <param name="report">The report.</param>
        public ControlProperties(ReportParameters parameters, DataSets datasets)
        {
            try
            {
                InitializeComponent();

                this.isReportParameter = true;
                this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleReportParameterProperties");
                
                this.ReportParameters = parameters;
                this.CurrentElement = "Parameter";
                this.DataSets = datasets;

                if (this.ReportParameters != null && this.ReportParameters.Count > 0)
                {
                    this.ReportParameterNames = (from reportParam in this.ReportParameters
                                                 select reportParam.Name).ToArray<string>();
                }
                this.ReportParameterNew = new Syncfusion.RDL.DOM.ReportParameter();
                this.ReportParamGeneral = new ReportParameterGeneral(this.DataSets, this.ReportParameterNames);
                this.ReportParamDefaultValue = new ReportParameterDefaultValues(this.DataSets);
                this.ReportParamAvailValue = new ReportParameterAvailableValues(this.DataSets);
                
                //InitializeReportSettings();
                this.ReportParamDefaultValue.Visibility = System.Windows.Visibility.Hidden;
                this.ReportParamAvailValue.Visibility = System.Windows.Visibility.Hidden;
                this.AddTab(ControlPropertyType.General, this.ReportParamGeneral);
                this.AddTab(ControlPropertyType.AvailableValues, this.ReportParamAvailValue);
                this.AddTab(ControlPropertyType.DefaultValues, this.ReportParamDefaultValue);
                EventInitialize();
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ControlProperties"/> class.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="header">The header.</param>
        public ControlProperties(ReportParameters parameters,DataSets datasets, string name)
        {
            try
            {
                InitializeComponent();
                this.isReportParameter = true;
                this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleReportParameterProperties");
                this.ReportParameters = parameters;
                this.CurrentElement = "Parameter";
                this.DataSets = datasets;

                this.ReportParameterNew = (from parameterObject in this.ReportParameters
                                           where parameterObject.Name == name
                                           select parameterObject).SingleOrDefault();

                this.ReportParameterNames = (from reportParam in this.ReportParameters
                                             where reportParam.Name != name
                                             select reportParam.Name).ToArray<string>();

                this.ReportParamGeneral = new ReportParameterGeneral(this.DataSets, this.ReportParameterNew);
                this.ReportParamDefaultValue = new ReportParameterDefaultValues(this.DataSets, this.ReportParameterNew);
                this.ReportParamAvailValue = new ReportParameterAvailableValues(this.DataSets, this.ReportParameterNew);

                //InitializeReportSettings();
                this.ReportParamDefaultValue.Visibility = System.Windows.Visibility.Hidden;
                this.ReportParamAvailValue.Visibility = System.Windows.Visibility.Hidden;
                this.AddTab(ControlPropertyType.General, this.ReportParamGeneral);
                this.AddTab(ControlPropertyType.AvailableValues, this.ReportParamAvailValue);
                this.AddTab(ControlPropertyType.DefaultValues, this.ReportParamDefaultValue);
                EventInitialize();
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        #endregion

        #region image
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlProperties"/> class.
        /// </summary>
        /// <param name="imageControl">The image control.</param>
        /// <param name="EmbeddedImages">The embedded images.</param>
        public ControlProperties(ImageControl imageControl, EmbeddedImages embeddedImages)
        {
            try
            {
                InitializeComponent();
                this.Title = SR.GetString(CultureInfo.CurrentUICulture,"headerImageProperties");
                this.CurrentUIElement = imageControl;
                this.EmbeddedImages = embeddedImages;
                this.CachedEmbeddedImages = new EmbeddedImages();

                this.ImageGeneral = new ImageGeneral();

                List<string> Choices = new List<string>();
                
                if (imageControl.ImageProperties.Source == RDL.DOM.Source.Database)
                {
                    this.ImageSourceType = ImageSourceType.Database;
                    foreach (var dataset in imageControl.Panel.DataSets)
                    {
                        foreach (var field in dataset.Fields)
                        {
                            Choices.Add("=First(Fields!" + field.Name + ".Value,\"" + dataset.Name + "\")");
                        }
                    }
                }
                else
                {
                    this.ImageSourceType = ImageSourceType.Embedded;
                    foreach (var embddedImage in this.EmbeddedImages)
                    {
                        Choices.Add(embddedImage.Name);
                    }
                }
                this.ImageGeneral = new ImageGeneral(ImageSourceType);
                this.ImageGeneral.ImageName.ChoiceItems = Choices;
                this.ImageGeneral.btn_Import.Click += new RoutedEventHandler(btn_Import_Click);
                this.ImageGeneral.ImageName.SelectionChanged += new SelectionChangedEventHandler(ImageName_SelectionChanged);
                this.ImageSize = new ImageSize();
                ImageSize.Visibility = System.Windows.Visibility.Hidden;
                this.ImageVisibility = new TextBoxVisibility();
                ImageVisibility.Visibility = System.Windows.Visibility.Hidden;
                this.ImageBorder = new ImageBorder();
                ImageBorder.Visibility = System.Windows.Visibility.Hidden;
                this.ControlAction = new ControlAction(imageControl.Action,imageControl.Panel.DataSets);
                ControlAction.Visibility = System.Windows.Visibility.Hidden;
                AddTab(ControlPropertyType.General, ImageGeneral);
                AddTab(ControlPropertyType.Size, ImageSize);
                AddTab(ControlPropertyType.Border, ImageBorder);
                AddTab(ControlPropertyType.Action, ControlAction);
                EventInitialize();
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }
        #endregion

        #region Line
        public ControlProperties(LineControl line)
        {
            try
            {
                InitializeComponent();
                this.Title = "Line Properties";
                this.CurrentUIElement = line;
                this.LineGeneral = new LineGeneral();
                this.LineStyle = new LineStyle();
                // this.LineGeneral.Visibility = System.Windows.Visibility.Hidden;
                this.LineStyle.Visibility = System.Windows.Visibility.Hidden;
                AddTab(ControlPropertyType.General, LineGeneral);
                AddTab(ControlPropertyType.Style, LineStyle);
                EventInitialize();
            }
            catch
            {
            }
        }
        #endregion

        #region Rectangle

        public ControlProperties(RectangleControl rect)
        {
            try
            {
                InitializeComponent();
                this.Title = SR.GetString(CultureInfo.CurrentUICulture,"headerRectangleProperties");
                this.CurrentUIElement = rect;
                this.RectangleGeneral = new RectangleGeneral();
                this.RectangleStyle = new RectangleStyle();
                this.RectangleFill = new RectangleFill();
                // this.LineGeneral.Visibility = System.Windows.Visibility.Hidden;
                this.RectangleStyle.Visibility = System.Windows.Visibility.Hidden;
                this.RectangleFill.Visibility = System.Windows.Visibility.Hidden;
                AddTab(ControlPropertyType.General, RectangleGeneral);
                AddTab(ControlPropertyType.Style, RectangleStyle);
                AddTab(ControlPropertyType.Fill, RectangleFill);
                EventInitialize();
            }
            catch
            {
            }
        }

        #endregion

        #region Map

        public ControlProperties(MapControl map,MapChild child)
        {
#if !SyncfusionFramework3_5
            try
            {
                if (child == MapChild.Map)
                {
                    InitializeComponent();
                    this.Title = "Map Properties";
                    this.CurrentUIElement = map;
                    this.MapGeneral = new MapGeneral();
                    this.MapFill = new MapFill();
                    this.MapFill.Visibility = System.Windows.Visibility.Hidden;
                    AddTab(ControlPropertyType.General, MapGeneral);
                    AddTab(ControlPropertyType.Fill, MapFill);
                    EventInitialize();
                }
                else if (child == MapChild.Layer)
                {
                    InitializeComponent();
                    this.Title = "Layer Properties";
                    this.CurrentUIElement = map;
                    this.LayerGeneral = new Dialogs.LayerGeneral();
                    this.LayerGeneral.btn_browse.Click += new RoutedEventHandler(btn_browse_Click);
                    AddTab(ControlPropertyType.General, LayerGeneral);
                    EventInitialize();
                }
            }
            catch
            {
            }
#endif
        }

        #endregion

        #region TextBox
        public ControlProperties(TextBoxControl textBox, DataSets dataSets, TextBoxPropertyType textBoxPropertyType)
        {
            try
            {
                InitializeComponent();
                //InitializeReportSettings();
                //InitializeReportItems("TextBox");
                this.CurrentUIElement = textBox;

                //this.TextBoxText_General = new TextBoxText_General();
                this.TextBoxGeneral = new TextBoxGeneral();
                this.TextBoxAlignment = new TextBoxAlignment();
                this.TextBoxAlignment.Visibility = System.Windows.Visibility.Hidden;
                this.TextBoxFont = new TextBoxFont();
                this.TextBoxFont.Visibility = System.Windows.Visibility.Hidden;
                this.TextBoxFill = new TextBoxFill();
                this.TextBoxFill.Visibility = System.Windows.Visibility.Hidden;
                this.TextBoxVisibility = new TextBoxVisibility();
                this.TextBoxVisibility.Visibility = System.Windows.Visibility.Hidden;
                this.TextBoxBorder = new TextBoxBorder();
                this.TextBoxBorder.Visibility = System.Windows.Visibility.Hidden;
                this.ControlAction = new ControlAction(textBox.Action,dataSets);
                this.ControlAction.Visibility = System.Windows.Visibility.Hidden;

                if (textBoxPropertyType.Equals(TextBoxPropertyType.TextBoxProperty))
                {
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleTextBoxProperties");
                    TextBoxGeneral.ValueRowDef.Height = new GridLength(0);
                    TextBoxGeneral.stpnl_Value.Visibility = System.Windows.Visibility.Hidden;
                    AddTab(ControlPropertyType.General, TextBoxGeneral);
                    //AddTab(ControlPropertyType.General, TextBoxText_General);                
                }
                else if (textBoxPropertyType.Equals(TextBoxPropertyType.CreatePlaceHolder))
                {
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleCreatePlaceHolder");
                    TextBoxGeneral.ToolTipRowDef.Height = new GridLength(0);
                    TextBoxGeneral.stpnl_ToolTip.Visibility = System.Windows.Visibility.Hidden;
                    TextBoxGeneral.Value.SelectionChanged += new SelectionChangedEventHandler(Value_SelectionChanged);
                    AddTab(ControlPropertyType.General, TextBoxGeneral);
                    HiddenTextBoxWizardAlignmentProperties(TextBoxAlignment);
                }
                else if (textBoxPropertyType.Equals(TextBoxPropertyType.PlaceHolderProperty))
                {
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titlePlaceHolderProperties");
                    TextBoxGeneral.ToolTipRowDef.Height = new GridLength(0);
                    TextBoxGeneral.stpnl_ToolTip.Visibility = System.Windows.Visibility.Hidden;
                    TextBoxGeneral.Value.SelectionChanged += new SelectionChangedEventHandler(Value_SelectionChanged);
                    AddTab(ControlPropertyType.General, TextBoxGeneral);
                    HiddenTextBoxWizardAlignmentProperties(TextBoxAlignment);
                }
                else if (textBoxPropertyType.Equals(TextBoxPropertyType.TextProperty))
                {
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleTextProperties");
                    TextBoxGeneral.stpnl_Value.Visibility = System.Windows.Visibility.Hidden;
                    this.TextBoxAlignment.Visibility = System.Windows.Visibility.Visible;
                    HiddenTextBoxWizardAlignmentProperties(TextBoxAlignment);
                }
                AddTab(ControlPropertyType.Alignment, TextBoxAlignment);
                AddTab(ControlPropertyType.Font, TextBoxFont);
                AddTab(ControlPropertyType.Action, ControlAction);

                if (textBoxPropertyType.Equals(TextBoxPropertyType.TextBoxProperty))
                {
                    AddTab(ControlPropertyType.Fill, TextBoxFill);
                    AddTab(ControlPropertyType.Border, TextBoxBorder);
                    AddTab(ControlPropertyType.Visibility, TextBoxVisibility);
                }

                //textBox.TextBox = TextBoxControl.PopulateTextboxReportItem(textBox, "");
                //PopulateTextBoxFromReport(textBox, dataSets, textBoxPropertyType);
                EventInitialize();
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }


        private void HiddenTextBoxWizardAlignmentProperties(TextBoxAlignment textBoxAlignment)
        {
            if (textBoxAlignment != null)
            {
                // for title
                textBoxAlignment.txt_HeaderText.Text = Syncfusion.Windows.Reports.Designer.Properties.Resources.textBlockTextAlignInfo2;

                // for vertical alignment
                textBoxAlignment.stpnl_Vertical.Visibility = System.Windows.Visibility.Hidden;

                // for padding 
                textBoxAlignment.PaddingGrid.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        #endregion

        #region Tablix
        public ControlProperties(TablixControl tablix,TablixPropertyType type)
        {
            try
            {
                RDL.DOM.DataSet dataSet = null;

                if (ReportDesignView.CurrentPanel.DataSets != null)
                {
                    foreach (RDL.DOM.DataSet set in ReportDesignView.CurrentPanel.DataSets)
                    {
                        if (set.Name.Equals(tablix.ReportDataSetName))
                        {
                            dataSet = set;
                        }
                    }
                }

                if (type == TablixPropertyType.Tablix)
                {
                    InitializeComponent();
                    this.Title = SR.GetString(CultureInfo.CurrentUICulture,"titleTablixProperties");
                    this.CurrentUIElement = tablix;

                    this.TablixGeneral = new TablixGeneral(tablix);
                    this.TablixVisibility = new TablixVisibility();
                    this.TablixSorting = new TablixSorting(dataSet, tablix.SortExpressions);
                    this.TablixFilters = new TablixFilters(dataSet, tablix.Filters);

                    this.TablixVisibility.Visibility = System.Windows.Visibility.Hidden;
                    this.TablixSorting.Visibility = System.Windows.Visibility.Hidden;
                    this.TablixFilters.Visibility = System.Windows.Visibility.Hidden;

                    AddTab(ControlPropertyType.General, this.TablixGeneral);
                    AddTab(ControlPropertyType.Visibility, this.TablixVisibility);
                    AddTab(ControlPropertyType.Filter, this.TablixFilters);
                    AddTab(ControlPropertyType.Sorting, this.TablixSorting);
                    EventInitialize();
                }

                else
                {
                    InitializeComponent();
                    this.Title = "Column Visibility";
                    this.CurrentUIElement = tablix;

                    this.TablixVisibility = new TablixVisibility();

                    AddTab(ControlPropertyType.Visibility, this.TablixVisibility);
                    EventInitialize();
                }
            }
            catch { }
        }
        #endregion

        #region Report

        public ControlProperties(ReportDefinition reportDefinition)
        {
            try
            {
                InitializeComponent();
                this.isReport = true;
                this.Title = "Report Properties";
                this.Report = reportDefinition;
                this.CurrentElement = "Report";

                if (this.Report.RDLType == RDLType.RDL2010)
                {
                    this.ReportPageSetup = new ReportPageSetup(this.Report.ReportSections.First().Page,UnitType);
                }
                else
                {
                    this.ReportPageSetup = new ReportPageSetup(this.Report.Page, UnitType);
                }

                this.ReportCode = new ReportCode(this.Report.Code);
                this.ReportReferences = new ReportReferences(this.Report);
                this.ReportVariables = new ReportVariables(this.Report.Variables);
                this.ReportCode.Visibility = System.Windows.Visibility.Hidden;
                this.ReportReferences.Visibility = System.Windows.Visibility.Hidden;
                this.ReportVariables.Visibility = System.Windows.Visibility.Hidden;

                AddTab(ControlPropertyType.PageSetup, this.ReportPageSetup);
                AddTab(ControlPropertyType.Code, this.ReportCode);
                AddTab(ControlPropertyType.Reference, this.ReportReferences);
                AddTab(ControlPropertyType.Variable, this.ReportVariables);
                EventInitialize();
            }
            catch { }
        }

        #endregion

        #endregion

        #region Public Properties

        #region Chart
        /// <summary>
        /// Gets or sets the chart general.
        /// </summary>
        /// <value>The chart general.</value>
        public ChartGeneral ChartGeneral { get; set; }

        /// <summary>
        /// Gets or sets the chart border.
        /// </summary>
        /// <value>The chart border.</value>
        public ChartBorder ChartBorder { get; set; }

        /// <summary>
        /// Gets or sets the chart background.
        /// </summary>
        /// <value>The chart background.</value>
        public ChartBackground ChartBackground { get; set; }

        /// <summary>
        /// Gets or sets the chart area background.
        /// </summary>
        /// <value>The chart area background.</value>
        public ChartAreaBackground ChartAreaBackground { get; set; }

        /// <summary>
        /// Gets or sets the legand general.
        /// </summary>
        /// <value>The legand general.</value>
        public LegendGeneral LegendGeneral { get; set; }

        /// <summary>
        /// Gets or sets the legand font.
        /// </summary>
        /// <value>The legand font.</value>
        public LegendFont LegendFont { get; set; }

        /// <summary>
        /// Gets or sets the legand border.
        /// </summary>
        /// <value>The legand border.</value>
        public LegendBorder LegendBorder { get; set; }

        /// <summary>
        /// Gets or sets the axis category general.
        /// </summary>
        /// <value>The axis category general.</value>
        public AxisCategoryGeneral AxisCategoryGeneral { get; set; }

        /// <summary>
        /// Gets or sets the axis value general.
        /// </summary>
        /// <value>The axis value general.</value>
        public AxisValueGeneral AxisValueGeneral { get; set; }

        /// <summary>
        /// Gets or sets the axis category label.
        /// </summary>
        /// <value>The axis category label.</value>
        public AxisCategoryLabel AxisCategoryLabel { get; set; }

        /// <summary>
        /// Gets or sets the axis value label.
        /// </summary>
        /// <value>The axis value label.</value>
        public AxisValueLabel AxisValueLabel { get; set; }

        /// <summary>
        /// Gets or sets the axis category tick marks.
        /// </summary>
        /// <value>The axis category tick marks.</value>
        public AxisCategoryTickMarks AxisCategoryTickMarks { get; set; }

        /// <summary>
        /// Gets or sets the axis value tick marks.
        /// </summary>
        /// <value>The axis value tick marks.</value>
        public AxisValueTickMarks AxisValueTickMarks { get; set; }

        /// <summary>
        /// Gets or sets the chart title.
        /// </summary>
        /// <value>The chart title.</value>
        public Syncfusion.Windows.Reports.Designer.Dialogs.ChartTitle ChartTitle { get; set; }

        /// <summary>
        /// Gets or sets the category title.
        /// </summary>
        /// <value>The category title.</value>
        public CategoryAxisTitle CategoryAxisTitle { get; set; }

        /// <summary>
        /// Gets or sets the value title.
        /// </summary>
        /// <value>The value title.</value>
        public ValueAxisTitle ValueAxisTitle { get; set; }

        /// <summary>
        /// Gets or sets the secondary category title.
        /// </summary>
        /// <value>The secondary category title.</value>
        public SecondaryCategoryAxisTitle SecondaryCategoryAxisTitle { get; set; }

        /// <summary>
        /// Gets or sets the secondary value title.
        /// </summary>
        /// <value>The secondary value title.</value>
        public SecondaryValueAxisTitle SecondaryValueAxisTitle { get; set; }

        /// <summary>
        /// Gets or sets the series general.
        /// </summary>
        /// <value>The series general.</value>
        public SeriesGeneral1 SeriesGeneral { get; set; }

        /// <summary>
        /// Gets or sets the series axes.
        /// </summary>
        /// <value>The series axes.</value>
        public SeriesAxes SeriesAxes { get; set; }

        /// <summary>
        /// Gets or sets the image general.
        /// </summary>
        /// <value>The image general.</value>
        public ImageGeneral ImageGeneral { get; set; }

        /// <summary>
        /// Gets or sets the size of the image.
        /// </summary>
        /// <value>The size of the image.</value>
        public ImageSize ImageSize { get; set; }

        /// <summary>
        /// Gets or sets the image Border.
        /// </summary>
        /// <value>The image Border.</value>
        public ImageBorder ImageBorder { get; set; }

        /// <summary>
        /// Gets or sets the scale general.
        /// </summary>
        /// <value>The scale general.</value>
        public ScaleGeneral ScaleGeneral { get; set; }

        /// <summary>
        /// Gets or sets the scale label.
        /// </summary>
        /// <value>The scale label.</value>
        public ScaleLabel ScaleLabel { get; set; }

        /// <summary>
        /// Gets or sets the scale major tick.
        /// </summary>
        /// <value>The scale major tick.</value>
        public ScaleMajorTick ScaleMajorTick { get; set; }

        /// <summary>
        /// Gets or sets the scale minor tick.
        /// </summary>
        /// <value>The scale minor tick.</value>
        public ScaleMinorTick ScaleMinorTick { get; set; }

        /// <summary>
        /// Gets or sets the pointer general.
        /// </summary>
        /// <value>The pointer general.</value>
        public PointerGeneral PointerGeneral { get; set; }

        /// <summary>
        /// Gets or sets the pointer color fill.
        /// </summary>
        /// <value>The pointer color fill.</value>
        public PointerColorFill PointerColorFill { get; set; }

        /// <summary>
        /// Gets or sets the range general.
        /// </summary>
        /// <value>The range general.</value>
        public RangeGeneral RangeGeneral { get; set; }

        /// <summary>
        /// Gets or sets the range border.
        /// </summary>
        /// <value>The range border.</value>
        public RangeBorder RangeBorder { get; set; }

        /// <summary>
        /// Gets or sets the gauge general.
        /// </summary>
        /// <value>The gauge general.</value>
        public GaugeGeneral GaugeGeneral { get; set; }

        /// <summary>
        /// Gets or sets the gauge panel general.
        /// </summary>
        /// <value>The gauge panel general.</value>
        public PanelGeneral GaugePanelGeneral { get; set; }

        /// <summary>
        /// Gets or sets the gauge panel visibility.
        /// </summary>
        /// <value>The gauge panel visibility.</value>
        public PanelVisibility GaugePanelVisibility { get; set; }

        /// <summary>
        /// Gets or sets the gauge panel border.
        /// </summary>
        /// <value>The gauge panel border.</value>
        public PanelBorder GaugePanelBorder { get; set; }

        /// <summary>
        /// Gets or sets the gauge panel fill.
        /// </summary>
        /// <value>The gauge panel fill.</value>
        public PanelFill GaugePanelFill { get; set; }


        #endregion

        #region Report

        /// <summary>
        /// Gets or sets the ReportCode.
        /// </summary>
        /// <value>The Report Code.</value>
        public ReportCode ReportCode { get; set; }

        /// <summary>
        /// Gets or sets the ReportPage setup.
        /// </summary>
        /// <value>The Report Page Setup.</value>
        public ReportPageSetup ReportPageSetup { get; set; }

        /// <summary>
        /// Gets or sets the ReportReferences.
        /// </summary>
        /// <value>The ReportReferences.</value>
        public ReportReferences ReportReferences { get; set; }

        /// <summary>
        /// Gets or sets the ReportVariables.
        /// </summary>
        /// <value>The ReportVariables.</value>
        public ReportVariables ReportVariables { get; set; }

        #endregion

        #region Tablix

        /// <summary>
        /// Gets or sets the TablixGeneral setup.
        /// </summary>
        /// <value>The TablixGeneral.</value>
        public TablixGeneral TablixGeneral { get; set; }

        /// <summary>
        /// Gets or sets the TablixVisibility setup.
        /// </summary>
        /// <value>The TablixVisibility.</value>
        public TablixVisibility TablixVisibility { get; set; }

        /// <summary>
        /// Gets or sets the TablixFilters.
        /// </summary>
        /// <value>The TablixFilters.</value>
        public TablixFilters TablixFilters { get; set; }

        /// <summary>
        /// Gets or sets the TablixSorting.
        /// </summary>
        /// <value>The TablixSorting.</value>
        public TablixSorting TablixSorting { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the image visibility.
        /// </summary>
        /// <value>The image visibility.</value>
        public TextBoxVisibility ImageVisibility { get; set; }

        /// <summary>
        /// Gets or sets the text box general.
        /// </summary>
        /// <value>The text box general.</value>
        public TextBoxGeneral TextBoxGeneral { get; set; }

        /// <summary>
        /// Gets or sets the sub report general.
        /// </summary>
        /// <value>The sub report general.</value>
        public SubReportGeneral SubReportGeneral { get; set; }

        /// <summary>
        /// Gets or sets the sub report visibility.
        /// </summary>
        /// <value>The sub report visibility.</value>
        public SubReportVisibility SubReportVisibility { get; set; }

        /// <summary>
        /// Gets or sets the sub report border.
        /// </summary>
        /// <value>The sub report border.</value>
        public SubReportBorder SubReportBorder { get; set; }


        /// <summary>
        /// Gets or sets the Line general.
        /// </summary>
        /// <value>The Line general.</value>
        public LineGeneral LineGeneral { get; set; }

        /// <summary>
        /// Gets or sets the Line Style.
        /// </summary>
        /// <value>The Line Style.</value>
        public LineStyle LineStyle { get; set; }

        /// <summary>
        /// Gets or sets the text box text_ general.
        /// </summary>
        /// <value>The text box text_ general.</value>
        public TextBoxText_General TextBoxText_General { get; set; }

        /// <summary>
        /// Gets or sets the text box alignment.
        /// </summary>
        /// <value>The text box alignment.</value>
        public TextBoxAlignment TextBoxAlignment { get; set; }

        /// <summary>
        /// Gets or sets the text box font.
        /// </summary>
        /// <value>The text box font.</value>
        public TextBoxFont TextBoxFont { get; set; }

        /// <summary>
        /// Gets or sets the text box fill.
        /// </summary>
        /// <value>The text box fill.</value>
        public TextBoxFill TextBoxFill { get; set; }

        /// <summary>
        /// Gets or sets the text box visibility.
        /// </summary>
        /// <value>The text box visibility.</value>
        public TextBoxVisibility TextBoxVisibility { get; set; }

        public TextBoxBorder TextBoxBorder { get; set; }

        /// <summary>
        /// Gets or sets the Control action.
        /// </summary>
        /// <value>The Control action.</value>
        public ControlAction ControlAction { get; set; }

        /// <summary>
        /// Gets or sets the tablix general.
        /// </summary>
        /// <value>The tablix general.</value>

        public FooterGeneral FooterGeneral { get; set; }

        /// <summary>
        /// Gets or sets the header general.
        /// </summary>
        /// <value>The header general.</value>
        public HeaderGeneral HeaderGeneral { get; set; }

        /// <summary>
        /// Gets or sets the rectangle general.
        /// </summary>
        /// <value>The rectangle general.</value>
        public RectangleGeneral RectangleGeneral { get; set; }

        /// <summary>
        /// Gets or sets the rectangle style.
        /// </summary>
        /// <value>The rectangle fill.</value>
        public RectangleStyle RectangleStyle { get; set; }

        /// <summary>
        /// Gets or sets the rectangle fill.
        /// </summary>
        /// <value>The rectangle fill.</value>
        public RectangleFill RectangleFill { get; set; }

   
        /// <summary>
        /// Gets or sets the rectangle visibility.
        /// </summary>
        /// <value>The rectangle visibility.</value>x
        public TextBoxVisibility RectangleVisibility { get; set; }

        public MapGeneral MapGeneral { get; set; }

        public LayerGeneral LayerGeneral { get; set; }

        public MapFill MapFill { get; set; }


        /// <summary>
        /// Gets or sets the report body fill.
        /// </summary>
        /// <value>The report body fill.</value>
        public TextBoxFill ReportBodyFill { get; set; }

        /// <summary>
        /// Gets or sets the report param default value.
        /// </summary>
        /// <value>The report param default value.</value>
        public ReportParameterDefaultValues ReportParamDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the report param general.
        /// </summary>
        /// <value>The report param general.</value>
        public ReportParameterGeneral ReportParamGeneral { get; set; }

        /// <summary>
        /// Gets or sets the report param avail value.
        /// </summary>
        /// <value>The report param avail value.</value>
        public ReportParameterAvailableValues ReportParamAvailValue { get; set; }

        /// <summary>
        /// Gets or sets the report parameter new.
        /// </summary>
        /// <value>The report parameter new.</value>
        public Syncfusion.RDL.DOM.ReportParameter ReportParameterNew { get; set; }

        /// <summary>
        /// Gets or sets the sub report general.
        /// </summary>
        /// <value>The sub report general.</value>


        #endregion

        #region SelectDeselect ControlProperty Listbox Item

        private void SelectDeselectReportParameterTab()
        {
            if (this.lbox_NodeType.SelectedItem.ToString().Equals(ConvertEnum(ControlPropertyType.General)) && this.ReportParamGeneral.Visibility == System.Windows.Visibility.Hidden)
            {
                this.ReportParamGeneral.Visibility = System.Windows.Visibility.Visible;
                this.ReportParamDefaultValue.Visibility = System.Windows.Visibility.Hidden;
                this.ReportParamAvailValue.Visibility = System.Windows.Visibility.Hidden;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString().Equals(ConvertEnum(ControlPropertyType.DefaultValues)) && this.ReportParamDefaultValue.Visibility == System.Windows.Visibility.Hidden)
            {
                this.ReportParamDefaultValue.Visibility = System.Windows.Visibility.Visible;
                this.ReportParamGeneral.Visibility = System.Windows.Visibility.Hidden;
                this.ReportParamAvailValue.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString().Equals(ConvertEnum(ControlPropertyType.AvailableValues)) && this.ReportParamAvailValue.Visibility == System.Windows.Visibility.Hidden)
            {
                this.ReportParamAvailValue.Visibility = System.Windows.Visibility.Visible;
                this.ReportParamGeneral.Visibility = System.Windows.Visibility.Hidden;
                this.ReportParamDefaultValue.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        //private void SelectDeselectRectangleTab(UIElement uiElement)
        //{
        //    if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
        //        && (uiElement is RectangleGeneral))
        //    {
        //        uiElement.Visibility = System.Windows.Visibility.Visible;
        //    }
        //    else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Visibility)
        //        && (uiElement is TextBoxVisibility))
        //    {
        //        uiElement.Visibility = System.Windows.Visibility.Visible;
        //    }
        //    else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Fill)
        //        && (uiElement is TextBoxFill))
        //    {
        //        uiElement.Visibility = System.Windows.Visibility.Visible;
        //    }
        //    else if ((uiElement is RectangleGeneral)
        //            || (uiElement is TextBoxVisibility)
        //            || (uiElement is TextBoxFill))
        //    {
        //        uiElement.Visibility = System.Windows.Visibility.Hidden;
        //    }
        //}

        private void SelectDeselectLineTab(UIElement uiElement)
        {
            if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
                LineStyle.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
                LineGeneral.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SelectDeselectRectangleTab(UIElement uiElement)
        {
            if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General) && uiElement is RectangleGeneral)
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
               RectangleStyle.Visibility = System.Windows.Visibility.Hidden;
                RectangleFill.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Style) && uiElement is RectangleStyle)
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
                RectangleGeneral.Visibility = System.Windows.Visibility.Hidden;
                RectangleFill.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Fill) && uiElement is RectangleFill)
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
                RectangleGeneral.Visibility = System.Windows.Visibility.Hidden;
                RectangleStyle.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SelectDeselectMapTab(UIElement uiElement)
        {
            if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General) && uiElement is MapGeneral)
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
                //RectangleStyle.Visibility = System.Windows.Visibility.Hidden;
                MapFill.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Fill) && uiElement is MapFill)
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
                MapGeneral.Visibility = System.Windows.Visibility.Hidden;
                //RectangleStyle.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General) && uiElement is LayerGeneral)
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
                //RectangleStyle.Visibility = System.Windows.Visibility.Hidden;
                //MapFill.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SelectDeselectTextBoxTab(UIElement uiElement)
        {
            if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is TextBoxGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is TextBoxText_General))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Alignment)
                    && (uiElement is TextBoxAlignment))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Font)
                    && (uiElement is TextBoxFont))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Fill)
                    && (uiElement is TextBoxFill))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Visibility)
                    && (uiElement is TextBoxVisibility))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString()== ConvertEnum(ControlPropertyType.Border)
                &&(uiElement is TextBoxBorder))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if ((this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Action)
                && (uiElement is ControlAction)))
            {
                this.ControlAction.Visibility = System.Windows.Visibility.Visible;
            }
            else if ((uiElement is TextBoxVisibility)
                    || (uiElement is TextBoxGeneral)
                    || (uiElement is TextBoxFont)
                    || (uiElement is TextBoxFill)
                    || (uiElement is TextBoxAlignment)
                    || (uiElement is TextBoxText_General)
                    || (uiElement is TextBoxBorder)
                    || (uiElement is ControlAction))
            {
                uiElement.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SelectDeselectChartTab(UIElement uiElement)
        {
            if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is ChartGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Border)
                && (uiElement is ChartBorder))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Fill)
                && (uiElement is ChartBackground))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.AreaColor)
                && (uiElement is ChartAreaBackground))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is LegendGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Font)
                && (uiElement is LegendFont))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Border)
                && (uiElement is LegendBorder))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is SeriesGeneral1))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is AxisValueGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Label)
                && (uiElement is AxisValueLabel))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Tick)
                && (uiElement is AxisValueTickMarks))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is AxisCategoryGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Label)
                && (uiElement is AxisCategoryLabel))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Tick)
                && (uiElement is AxisCategoryTickMarks))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is Syncfusion.Windows.Reports.Designer.Dialogs.ChartTitle))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
               && (uiElement is CategoryAxisTitle))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is ValueAxisTitle))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Axes)
            && (uiElement is SeriesAxes))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if ((uiElement is ChartGeneral)
                    || (uiElement is ChartBorder)
                    || (uiElement is ChartBackground)
                    || (uiElement is ChartAreaBackground)
                    || (uiElement is LegendGeneral)
                    || (uiElement is LegendFont)
                    || (uiElement is LegendBorder)
                    || (uiElement is SeriesGeneral1)
                    || (uiElement is AxisValueGeneral)
                    || (uiElement is AxisValueLabel)
                    || (uiElement is AxisValueTickMarks)
                    || (uiElement is AxisCategoryGeneral)
                    || (uiElement is AxisCategoryLabel)
                    || (uiElement is AxisCategoryTickMarks)
                    || (uiElement is Syncfusion.Windows.Reports.Designer.Dialogs.ChartTitle)
                    || (uiElement is CategoryAxisTitle)
                    || (uiElement is ValueAxisTitle)
                    || (uiElement is SeriesAxes))
            {
                uiElement.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SelectDeselectImageTab(UIElement uiElement)
        {
            if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is ImageGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Size)
                    && (uiElement is ImageSize))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Visibility)
                    && (uiElement is TextBoxVisibility))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Border)
                    && (uiElement is ImageBorder))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Action)
                    && (uiElement is ControlAction))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if ((uiElement is TextBoxVisibility)
                    || (uiElement is ImageSize)
                    || (uiElement is ImageGeneral)
                    || (uiElement is ImageBorder)
                    || (uiElement is ControlAction))
            {
                uiElement.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SelectDeselectGaugeTab(UIElement uiElement)
        {
            if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is ScaleGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Label)
                && (uiElement is ScaleLabel))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.MajorTick)
                && (uiElement is ScaleMajorTick))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.MinorTick)
                && (uiElement is ScaleMinorTick))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is PointerGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Fill)
                && (uiElement is Syncfusion.Windows.Reports.Designer.Dialogs.PointerColorFill))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is RangeGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Border)
                && (uiElement is RangeBorder))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is GaugeGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is PanelGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Fill)
                && (uiElement is PanelFill))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Visibility)
                && (uiElement is PanelVisibility))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Border)
                && (uiElement is PanelBorder))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if ((uiElement is ScaleGeneral)
                    || (uiElement is ScaleLabel)
                    || (uiElement is ScaleMajorTick)
                    || (uiElement is ScaleMinorTick)
                    || (uiElement is PointerGeneral)
                    || (uiElement is PointerColorFill)
                    || (uiElement is RangeGeneral)
                    || (uiElement is RangeBorder)
                    || (uiElement is GaugeGeneral)
                    || (uiElement is PanelGeneral)
                    || (uiElement is PanelVisibility)
                    || (uiElement is PanelFill)
                    || (uiElement is PanelBorder))
            {
                uiElement.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SelectDeselectSubReportTab(UIElement uiElement)
        {
            if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is SubReportGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Visibility)
                && (uiElement is SubReportVisibility))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Border)
                && (uiElement is SubReportBorder))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if ((uiElement is SubReportGeneral)
                    || (uiElement is SubReportVisibility)
                    || (uiElement is SubReportBorder))
            {
                uiElement.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SelectDeselectReportTab(UIElement uiElement)
        {
            if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.PageSetup)
                && (uiElement is ReportPageSetup))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Code)
                && (uiElement is ReportCode))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Variable)
                && (uiElement is ReportVariables))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Reference)
                && (uiElement is ReportReferences))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if ((uiElement is ReportPageSetup)
                    || (uiElement is ReportCode)
                    || (uiElement is ReportVariables)
                    || (uiElement is ReportReferences))
            {
                uiElement.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SelectDeselectTablixTab(UIElement uiElement)
        {
            if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.General)
                && (uiElement is TablixGeneral))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Visibility)
                && (uiElement is TablixVisibility))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Sorting)
                && (uiElement is TablixSorting))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.lbox_NodeType.SelectedItem.ToString() == ConvertEnum(ControlPropertyType.Filter)
                && (uiElement is TablixFilters))
            {
                uiElement.Visibility = System.Windows.Visibility.Visible;
            }
            else if ((uiElement is TablixGeneral)
                    || (uiElement is TablixVisibility)
                    || (uiElement is TablixSorting)
                    || (uiElement is TablixFilters))
            {
                uiElement.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        #endregion


        void btn_browse_Click(object sender, RoutedEventArgs e)
        {
#if !SyncfusionFramework3_5
            MapControl mapControl = this.CurrentUIElement as MapControl;
            string FileName=mapControl.Panel.GetShapeFile();
            this.LayerGeneral.txt_ShapeFile.Text = FileName;
#endif
        }

        /// <summary>
        /// Handles the Click event of the btn_Import control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// 

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            ImageControl imageControl = this.CurrentUIElement as ImageControl;
            EmbeddedImage embeddedImage = imageControl.Panel.GetEmbeddedImage();

            if (embeddedImage != null)
            {
                bool check;
                int imageCount = 0;
                var imageName = embeddedImage.Name;

                do
                {
                    check = (from embedImage in this.CachedEmbeddedImages
                             where embedImage.Name.Equals(embeddedImage.Name)
                             select embedImage).Count() > 0 ? true : false;

                    embeddedImage.Name = (check == true) ? imageName + (++imageCount) : embeddedImage.Name;

                } while (check);

                this.CachedEmbeddedImages.Add(embeddedImage);
                this.ImageGeneral.ImageName.ChoiceItems.Add(embeddedImage.Name);
                this.ImageGeneral.ImageName.Text = embeddedImage.Name;
            }
        }

        private void ImageName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string stemp = e.AddedItems[0].ToString();
                this.ImageGeneral.ImageName.Text = stemp;
            }
        }

        void Value_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string value = this.TextBoxGeneral.Value.SelectedItem.ToString();
            string Name = this.TextBoxGeneral.Name.Text;
            if (value.Contains("\""))
            {
                if (Name == null || (Name != null && Name.Trim() == string.Empty))
                {
                    this.TextBoxGeneral.Name.Text = "<<Expr>>";
                }
            }
            else
            {
                string exp = this.RemoveExpressionIdentifier(value);

                if (exp != string.Empty)
                {
                    this.TextBoxGeneral.Name.Text = exp;
                }
            }
        }

        private string RemoveExpressionIdentifier(string p)
        {
            if (!(p.StartsWith("=Globals!", true, null) || p.StartsWith("=User!", true, null)))
            {
                p = p.Replace("!", string.Empty);
                p = p.Replace("=", string.Empty);
                p = p.Replace(".Value", string.Empty);
                p = p.Replace("Fields", string.Empty);
                return p;
            }
            else
            {
                return string.Empty;
            }
        }

        private void PopulateReportBodyFromReport(Body reportBody)
        {
            this.ReportBodyFill.FillColor.Text = reportBody.Style.BackgroundColor;
        }

        private void AddTab(ControlPropertyType controlPropertyType, UserControl controlName)
        {
            this.lbox_NodeType.Items.Add(ConvertEnum(controlPropertyType));
            Grid.SetColumn(controlName, 1);
            controlName.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            controlName.HorizontalAlignment = HorizontalAlignment.Left;
            this.grd_PlaceHolder.Children.Add(controlName);
        }

        private string ConvertEnum(ControlPropertyType controlPropertyType)
        {
            string tempString;
            switch (controlPropertyType)
            {
                case ControlPropertyType.DefaultValues:
                    tempString = "Default Values";
                    break;
                case ControlPropertyType.PageSetup:
                    tempString = "Page Setup";
                    break;
                default:
                    tempString = controlPropertyType.ToString();
                    break;
            }
            return SR.GetString(CultureInfo.CurrentUICulture, tempString);
        }

        private void EventInitialize()
        {
            lbox_NodeType.SelectedItem = lbox_NodeType.Items[0];
            lbox_NodeType.SelectionChanged += new SelectionChangedEventHandler(lbox_NodeType_SelectionChanged);
            btn_ControlPropertiesOk.Click += new RoutedEventHandler(btn_ControlPropertiesOk_Click);
            btn_ControlPropertiesCancel.Click += new RoutedEventHandler(btn_ControlPropertiesCancel_Click);
            this.UpdateHelpButton();
        }

        private void btn_ControlPropertiesCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btn_ControlPropertiesOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.isReportParameter)
            {
                this.PopulateControlInformation();
                if (reportParameterIsCorrect)
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else if (this.isReport)
            {
                this.PopulateReportInformation();
                if (this.isValidValue)
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else
            {
                bool check = false;
                string tempName = null;

                if (this.CurrentUIElement is TextBoxControl)
                {
                    if ((this.CurrentUIElement as TextBoxControl).TextBoxProperties.IsEnabled)
                    {
                        tempName = this.TextBoxGeneral.Name.Text;
                        check = (this.CurrentUIElement as TextBoxControl).ItemName != tempName ? true : false;
                    }
                }
                else if (this.CurrentUIElement is LineControl)
                {
                    tempName = this.LineGeneral.txt_GeneralName.Text;
                    check = (this.CurrentUIElement as LineControl).ItemName != tempName ? true : false;
                }
                else if (this.CurrentUIElement is RectangleControl)
                {
                    tempName = this.RectangleGeneral.txt_GeneralName.Text;
                    check = (this.CurrentUIElement as RectangleControl).ItemName != tempName ? true : false;
                }
                else if (this.CurrentUIElement is ChartControl)
                {
                    try
                    {
                        tempName = this.ChartGeneral.txt_GeneralName.Text;
                        check = (this.CurrentUIElement as ChartControl).ItemName != tempName ? true : false;
                    }
                    catch(Exception){}
                }
#if !SyncfusionFramework3_5
                else if (this.CurrentUIElement is MapControl)
                {
                    try
                    {
                        tempName = this.MapGeneral.txt_GeneralName.Text;
                        check = (this.CurrentUIElement as MapControl).ItemName != tempName ? true : false;
                    }
                    catch (Exception) { }
                }
#endif
                else if ((this.CurrentUIElement is ImageControl))
                {
                    tempName = this.ImageGeneral.txt_GeneralName.Text;
                    check = (this.CurrentUIElement as ImageControl).ItemName != tempName ? true : false;
                }
                else if ((this.CurrentUIElement is GaugeControl))
                {
                    try
                    {
                        tempName = this.GaugeGeneral.txt_GeneralName.Text;
                        check = (this.CurrentUIElement as GaugeControl).ItemName != tempName ? true : false;
                    }
                    catch(Exception){}
                }
                else if (this.CurrentUIElement is SubReportControl)
                {
                    tempName = this.SubReportGeneral.txt_GeneralName.Text;
                    check = (this.CurrentUIElement as SubReportControl).ItemName != tempName ? true : false;
                }

                if (check)
                {
                    check = ((from name in ReportDesignView.CurrentPanel.reportItems
                              where name.ItemName.Equals(tempName)
                              select name).Count()) > 0 ? true : false;
                    if (check)
                    {
                        MessageBox.Show("<Name:>: " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCtrName") + tempName + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxExist"), this.Error_title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (string.IsNullOrEmpty(tempName))
                    {
                        MessageBox.Show("<Name:>: " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.Error_title, MessageBoxButton.OK, MessageBoxImage.Error);
                        check = true;
                    }
                }

                if (!check)
                {
                    if (this.CurrentUIElement is TextBoxControl || this.CurrentUIElement is ImageControl)
                    {
                        this.PopulateActionParameters();
                        if (this.isValidValue)
                        {
                            this.DialogResult = true;
                            this.Close();
                        }
                    }
                    else if(this.CurrentUIElement is TablixControl)
                    {
                        this.PopulateTablixInformation();
                        if (this.isValidValue)
                        {
                            this.DialogResult = true;
                            this.Close();
                        }
                    }
                    else
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
        }

        private void lbox_NodeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CurrentElement.ToLower().Equals("parameter"))
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectReportParameterTab();
                }
            }

            if (this.CurrentUIElement is TextBoxControl)
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectTextBoxTab(uiElement);
                }
            }
            else if (this.CurrentUIElement is LineControl)
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectLineTab(uiElement);
                }
            }

            else if (this.CurrentUIElement is RectangleControl)
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectRectangleTab(uiElement);
                }
            }

            else if (this.CurrentUIElement is ChartControl)
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectChartTab(uiElement);
                }
            }

            else if ((this.CurrentUIElement is ImageControl))
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectImageTab(uiElement);
                }
            }
            else if ((this.CurrentUIElement is GaugeControl))
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectGaugeTab(uiElement);
                }
            }
#if !SyncfusionFramework3_5
            else if ((this.CurrentUIElement is MapControl))
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectMapTab(uiElement);
                }
            }
#endif
            else if (this.CurrentUIElement is SubReportControl)
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectSubReportTab(uiElement);
                }
            }
            else if (this.CurrentElement.ToLower().Equals("report"))
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectReportTab(uiElement);
                }
            }
            else if (this.CurrentUIElement is TablixControl)
            {
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    SelectDeselectTablixTab(uiElement);
                }
            }
        }

        private string ImageToBase64(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                if(format!=null)
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }


        /// <summary>
        /// Creates the new instance of the <see cref="BitmapEncoder"/> class by extension of file.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>The BitmapEncoder</returns>       


        private void btn_HelponControlProperties_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentElement.ToLower().Equals("parameter"))
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/ReportParameter");
            }

            if (this.CurrentUIElement is TextBoxControl)
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/TextBoxProperties");
            }
            else if (this.CurrentUIElement is LineControl)
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/LineProperties");   
            }

            else if (this.CurrentUIElement is RectangleControl)
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/RectangleProperties");
            }
#if !SyncfusionFramework3_5
            else if (this.CurrentUIElement is MapControl)
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/RectangleProperties");
            }
#endif
            else if (this.CurrentUIElement is ChartControl)
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF");
            }

            else if ((this.CurrentUIElement is ImageControl))
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/ImageProperties");
            }
            else if ((this.CurrentUIElement is GaugeControl))
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/GaugeProperties");
            }
            else if (this.CurrentUIElement is SubReportControl)
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/ChartProperties");
            }
        }

        private void ChromelessWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.lbox_NodeType.Focus();
        }

        private void PopulateControlInformation()
        {
            try
            {
                this.reportParameterIsCorrect = true;
                if (this.ReportParameters == null)
                {
                    this.ReportParameters = new ReportParameters();
                }
                                               
                foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
                {
                    #region General
                   
                    if (uiElement is ReportParameterGeneral)
                    {
                        ReportParameterGeneral reportParamGeneral = (ReportParameterGeneral)uiElement;

                        #region Parameter Name

                        if (!Common.Util.CheckNameWithPreviousCollection(reportParamGeneral.text_ParamName.Text.ToString(), this.ReportParameterNames))
                        {
                            MessageBox.Show("<General Properties>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxParameterDeclared"), this.Error_title, MessageBoxButton.OK);
                            this.reportParameterIsCorrect = false;
                        }

                        else if (reportParamGeneral.chkBox_AllowNull.IsChecked == true && reportParamGeneral.chkBox_AllowMultiple.IsChecked == true)
                        {
                            MessageBox.Show("<General Properties>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCannotIncludeNullValues"), this.Error_title, MessageBoxButton.OK);
                            this.reportParameterIsCorrect = false;                                                                           
                        }

                        else if (!Common.Util.CheckNameWithRE(reportParamGeneral.text_ParamName.Text))
                        {
                            MessageBox.Show("<Name:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSpecifyValidName"), this.Error_title, MessageBoxButton.OK);
                            this.reportParameterIsCorrect = false;
                        }
                       
                        if (this.reportParameterIsCorrect)
                        {
                            if (reportParamGeneral.text_ParamName.Text.ToString() == string.Empty)
                            {
                                this.ReportParameterNew.Name = null;
                            }
                            else
                            {
                                this.ReportParameterNew.Name = reportParamGeneral.text_ParamName.Text.ToString();
                            }

                        #endregion

                            #region Parameter Prompt Name

                            if (reportParamGeneral.text_ParamPromptName.Text.ToString().Equals(string.Empty) || reportParamGeneral.rBtn_Internal.IsChecked == true)
                            {
                                this.ReportParameterNew.Prompt = null;
                            }
                            else
                            {
                                this.ReportParameterNew.Prompt = reportParamGeneral.text_ParamPromptName.Text.ToString();
                            }

                            #endregion

                            #region Parameter DataType

                            string strAsDataType = string.Empty;
                            switch (reportParamGeneral.cmbBox_ParamType.SelectedIndex)
                            {
                                case 0:
                                    strAsDataType = "String";
                                    break;
                                case 1:
                                    strAsDataType = "Boolean";
                                    break;
                                case 2:
                                    strAsDataType = "DateTime";
                                    break;
                                case 3:
                                    strAsDataType = "Integer";
                                    break;
                                case 4:
                                    strAsDataType = "Float";
                                    break;
                            }

                            this.ReportParameterNew.DataType = (RDL.DOM.DataTypes)Enum.Parse(typeof(RDL.DOM.DataTypes), strAsDataType);

                            #endregion

                            #region Allow Blank

                            if (reportParamGeneral.chkBox_AllowBlank.IsChecked == true)
                            {
                                this.ReportParameterNew.AllowBlank = true;
                            }
                            if (!(reportParamGeneral.chkBox_AllowBlank.IsChecked == true))
                            {
                                this.ReportParameterNew.AllowBlank =false;
                            }

                            #endregion

                            #region Allow Multiple Value

                            if (reportParamGeneral.chkBox_AllowMultiple.IsChecked == true)
                            {
                                this.ReportParameterNew.MultiValue = true;
                            }
                            if (!(reportParamGeneral.chkBox_AllowMultiple.IsChecked == true))
                            {
                                this.ReportParameterNew.MultiValue =false;
                            }
                            #endregion

                            #region Allow Nullable

                            if (reportParamGeneral.chkBox_AllowNull.IsChecked == true)
                            {
                                this.ReportParameterNew.Nullable = true;
                            }
                            if (!(reportParamGeneral.chkBox_AllowNull.IsChecked == true))
                            {
                                this.ReportParameterNew.Nullable =false;
                            }
                            #endregion

                            #region Parameter Visibility

                            if (reportParamGeneral.rBtn_Hidden.IsChecked == true)
                            {
                                this.ReportParameterNew.Hidden = true;
                            }
                            else
                            {
                                this.ReportParameterNew.Hidden = false;
                            }
                        }
                            #endregion
                       
                    }
                   
                    #endregion

                    #region Valid Values

                    else if (uiElement is ReportParameterAvailableValues)
                    {
                        ReportParameterAvailableValues reportParamAvailable = (ReportParameterAvailableValues)uiElement;
                        int availValuesCount = 0;
                        if (this.reportParameterIsCorrect)
                        {
                            #region DataSet Reference

                            if (reportParamAvailable.rbtn_GetValueFromQuery.IsChecked == true)
                            {
                                if (this.ReportParameterNew.ValidValues == null)
                                {
                                    this.ReportParameterNew.ValidValues = new ValidValues();
                                }

                                this.ReportParameterNew.ValidValues.ParameterValues = null;
                                if (reportParamAvailable.cmb_QueryDataSet.Text.Equals(string.Empty))
                                {
                                    MessageBox.Show("<DataSet:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectDataSetFromList"), this.Error_title, MessageBoxButton.OK);
                                    this.reportParameterIsCorrect = false;
                                    break;
                                }
                                else if (reportParamAvailable.cmb_QueryValueField.Text.Equals(string.Empty))
                                {
                                    MessageBox.Show("<Value:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxValueFromList"), this.Error_title, MessageBoxButton.OK);
                                    this.reportParameterIsCorrect = false;
                                    break;
                                }
                                else
                                {
                                    this.ReportParameterNew.ValidValues.DataSetReference = new DataSetReference();
                                    this.ReportParameterNew.ValidValues.DataSetReference.DataSetName = reportParamAvailable.cmb_QueryDataSet.Text.ToString();
                                    this.ReportParameterNew.ValidValues.DataSetReference.ValueField = reportParamAvailable.cmb_QueryValueField.Text.ToString();
                                    this.ReportParameterNew.ValidValues.DataSetReference.LabelField = reportParamAvailable.cmb_QueryLabelField.Text.ToString();
                                }

                            }

                            #endregion

                            #region Available Parameter Values

                            else if (reportParamAvailable.rbtn_SpecifyValue.IsChecked == true)
                            {
                                List<StackPanel> availablePanels = reportParamAvailable.listBox_Values.Items.OfType<StackPanel>().ToList();
                                if (availablePanels.Count > 0)
                                {
                                    if (this.ReportParameterNew.ValidValues == null)
                                    {
                                        this.ReportParameterNew.ValidValues = new ValidValues();
                                    }

                                    this.ReportParameterNew.ValidValues.DataSetReference = null;
                                    this.ReportParameterNew.ValidValues.ParameterValues = new ParameterValues();
                                    foreach (StackPanel panel in availablePanels)
                                    {
                                        if (this.reportParameterIsCorrect)
                                        {
                                            List<System.Windows.Controls.TextBox> availableParameterLabels = panel.Children.OfType<System.Windows.Controls.TextBox>().ToList();
                                            List<ComboBox> availableParameterValues = panel.Children.OfType<ComboBox>().ToList();
                                            for (int i = 0; i < availableParameterValues.Count; i++)
                                            {
                                                availValuesCount++;
                                                RDL.DOM.ParameterValue paramValue = new RDL.DOM.ParameterValue();
                                                if (availableParameterValues[i].Text.ToString().Equals("(Null)"))
                                                {
                                                    if (availValuesCount > 1 && this.ReportParameterNew.MultiValue == true)
                                                    {
                                                        MessageBox.Show("<Allow null value>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxParameterVannotInclude"), this.Error_title, MessageBoxButton.OK);
                                                        this.reportParameterIsCorrect = false;
                                                        break;
                                                    }
                                                    else if (this.ReportParameterNew.Nullable == true)
                                                    {
                                                        paramValue.Label = availableParameterLabels[i].Text.ToString();
                                                        paramValue.Value = null;
                                                        this.ReportParameterNew.ValidValues.ParameterValues.Add(paramValue);
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("<Available Value" + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNullValueSpecified"), this.Error_title, MessageBoxButton.OK);
                                                        this.reportParameterIsCorrect = false;
                                                        break;
                                                    }
                                                }
                                                else if (availableParameterValues[i].Text.ToString().Equals(string.Empty))
                                                {
                                                    MessageBox.Show("<Available Value" + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxBlankSpecified"), this.Error_title, MessageBoxButton.OK);
                                                    this.reportParameterIsCorrect = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    
                                                    paramValue.Label = availableParameterLabels[i].Text.ToString();
                                                    paramValue.Value = availableParameterValues[i].Text.ToString();
                                                    this.ReportParameterNew.ValidValues.ParameterValues.Add(paramValue);

                                                    int parsedInt;
                                                    if (this.ReportParameterNew.DataType == DataTypes.Integer)                                                    
                                                    {
                                                        if (!int.TryParse(paramValue.Value, out parsedInt) && !paramValue.Value.StartsWith("="))
                                                        {
                                                            MessageBox.Show("<Available value " + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidValue"), this.Error_title, MessageBoxButton.OK);
                                                            this.reportParameterIsCorrect = false;
                                                        }
                                                        else
                                                        {
                                                            this.reportParameterIsCorrect = true;
                                                        }
                                                    }

                                                    float parsedfloat;
                                                    if (this.ReportParameterNew.DataType == DataTypes.Float)                                                    
                                                    {
                                                        if (!float.TryParse(paramValue.Value, out parsedfloat) && !paramValue.Value.StartsWith("="))
                                                        {
                                                            MessageBox.Show("<Available value " + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidValue"), this.Error_title, MessageBoxButton.OK);
                                                            this.reportParameterIsCorrect = false;
                                                        }
                                                        else
                                                        {
                                                            this.reportParameterIsCorrect = true;
                                                        }
                                                        
                                                    }

                                                    DateTime parseddatetime;
                                                    if (this.ReportParameterNew.DataType == DataTypes.DateTime)                                                   
                                                    {
                                                        if (!DateTime.TryParse(paramValue.Value, out parseddatetime) && !paramValue.Value.StartsWith("="))
                                                        {
                                                            MessageBox.Show("<Available value " + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidValue"), this.Error_title, MessageBoxButton.OK);
                                                            this.reportParameterIsCorrect = false;
                                                        }
                                                        else
                                                        {
                                                            this.reportParameterIsCorrect = true;
                                                        }                                                        
                                                    }                                                   
                                                    Boolean parsedboolean;
                                                    if (this.ReportParameterNew.DataType == DataTypes.Boolean)
                                                    {
                                                        if (!Boolean.TryParse(paramValue.Value, out parsedboolean) && !paramValue.Value.StartsWith("="))
                                                        {
                                                            MessageBox.Show("<Available value " + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidValue"), this.Error_title, MessageBoxButton.OK);
                                                            this.reportParameterIsCorrect = false;
                                                        }
                                                        else
                                                        {
                                                            this.reportParameterIsCorrect = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region No Available Values

                            else
                            {
                                this.ReportParameterNew.ValidValues = null;
                            }
                            #endregion
                        }

                    }
                    #endregion

                    #region Default Values

                    else if (uiElement is ReportParameterDefaultValues)
                    {
                        ReportParameterDefaultValues reportParamDefault = (ReportParameterDefaultValues)uiElement;
                        int availValuesCount = 0;
                        if (this.reportParameterIsCorrect)
                        {
                            if (reportParamDefault.rbtn_GetValueFromQuery.IsChecked == true)
                            {
                                if (this.ReportParameterNew.DefaultValue == null)
                                {
                                    this.ReportParameterNew.DefaultValue = new DefaultValue();
                                }
                                this.ReportParameterNew.DefaultValue.Values = null;
                                if (reportParamDefault.cmb_QueryDataSet.Text.Equals(string.Empty))
                                {
                                    MessageBox.Show("<DataSet:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectDataSetFromList"), this.Error_title, MessageBoxButton.OK);
                                    this.reportParameterIsCorrect = false;
                                    break;
                                }
                                else if (reportParamDefault.cmb_QueryValueField.Text.Equals(string.Empty))
                                {
                                    MessageBox.Show("<Value:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxValueFromList"), this.Error_title, MessageBoxButton.OK);
                                    this.reportParameterIsCorrect = false;
                                    break;
                                }
                                else
                                {
                                    this.ReportParameterNew.DefaultValue.DataSetReference = new DataSetReference();
                                    this.ReportParameterNew.DefaultValue.DataSetReference.DataSetName = reportParamDefault.cmb_QueryDataSet.Text.ToString();
                                    this.ReportParameterNew.DefaultValue.DataSetReference.ValueField = reportParamDefault.cmb_QueryValueField.Text.ToString();
                                }
                            }
                            else if (reportParamDefault.rbtn_SpecifyValue.IsChecked == true)
                            {
                                List<StackPanel> avaliablePanels = reportParamDefault.listBox_Values.Items.OfType<StackPanel>().ToList();
                                if (avaliablePanels.Count > 0)
                                {
                                    if (this.ReportParameterNew.DefaultValue == null)
                                    {
                                        this.ReportParameterNew.DefaultValue = new DefaultValue();
                                    }

                                    this.ReportParameterNew.DefaultValue.DataSetReference = null;
                                    this.ReportParameterNew.DefaultValue.Values = new Values();
                                    this.ReportParameterNew.DefaultValue.Values.Clear();

                                    foreach (StackPanel spanel in avaliablePanels)
                                    {
                                        if (this.reportParameterIsCorrect)
                                        {
                                            List<ComboBox> availableParameterValues = spanel.Children.OfType<ComboBox>().ToList();

                                            for (int i = 0; i < availableParameterValues.Count; i++)
                                            {
                                                availValuesCount++;
                                                if (availableParameterValues[i].Text.ToString().Equals("(Null)"))
                                                {
                                                    if (availValuesCount > 1 && this.ReportParameterNew.Nullable == true)
                                                    {
                                                        MessageBox.Show("<Allow null value>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxParameterVannotInclude"), this.Error_title, MessageBoxButton.OK);
                                                        this.reportParameterIsCorrect = false;
                                                        break;
                                                    }
                                                    else if (this.ReportParameterNew.Nullable == true)
                                                    {
                                                        string paramValue = string.Empty; //Syncfusion.RDL.DOM.ValueType? paramValue = new Syncfusion.RDL.DOM.ValueType(string.Empty);
                                                        this.ReportParameterNew.DefaultValue.Values.Add(paramValue);
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("<Default Value" + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNullValueSpecified"), this.Error_title, MessageBoxButton.OK);
                                                        this.reportParameterIsCorrect = false;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (availValuesCount > 1 && this.ReportParameterNew.MultiValue == false)
                                                    {
                                                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxMultipleValueSpecified"), this.Error_title, MessageBoxButton.OK);
                                                        this.reportParameterIsCorrect = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        string paramValue = availableParameterValues[i].Text; //Syncfusion.RDL.DOM.ValueType? paramValue = new Syncfusion.RDL.DOM.ValueType(availableParameterValues[i].Text.ToString());
                                                        this.ReportParameterNew.DefaultValue.Values.Add(paramValue);

                                                        int parsedInt;
                                                        if (this.ReportParameterNew.DataType == DataTypes.Integer)
                                                        {
                                                            if (!int.TryParse(paramValue, out parsedInt) && !paramValue.StartsWith("="))
                                                            {
                                                                MessageBox.Show("<Default value " + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidValue"), this.Error_title, MessageBoxButton.OK);
                                                                this.reportParameterIsCorrect = false;
                                                            }

                                                        }

                                                        float parsedfloat;
                                                        if (this.ReportParameterNew.DataType == DataTypes.Float)
                                                        {
                                                            if (!float.TryParse(paramValue, out parsedfloat) && !paramValue.StartsWith("="))
                                                            {
                                                                MessageBox.Show("<Default value " + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidValue"), this.Error_title, MessageBoxButton.OK);
                                                                this.reportParameterIsCorrect = false;
                                                            }
                                                            else
                                                            {
                                                                this.reportParameterIsCorrect = true;
                                                            }
                                                        }

                                                        DateTime parseddatetime;
                                                        if (this.ReportParameterNew.DataType == DataTypes.DateTime)
                                                        {
                                                            if (!DateTime.TryParse(paramValue, out parseddatetime) && !paramValue.StartsWith("="))
                                                            {
                                                                MessageBox.Show("<Default value " + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidValue"), this.Error_title, MessageBoxButton.OK);
                                                                this.reportParameterIsCorrect = false;
                                                            }
                                                            else
                                                            {
                                                                this.reportParameterIsCorrect = true;
                                                            }
                                                        } 
                                                        Boolean parsedboolean;
                                                        if (this.ReportParameterNew.DataType == DataTypes.Boolean)
                                                        {
                                                            if (!Boolean.TryParse(paramValue, out parsedboolean) && !paramValue.StartsWith("="))
                                                            {
                                                                MessageBox.Show("<Default value " + (i + 1) + ">\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidValue"), this.Error_title, MessageBoxButton.OK);
                                                                this.reportParameterIsCorrect = false;
                                                            }
                                                            else
                                                            {
                                                                this.reportParameterIsCorrect = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                this.ReportParameterNew.DefaultValue = null;
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception)
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }       

        private bool PopulateReportInformation()
        {
            foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
            {
                if (uiElement is ReportCode)
                {
                    ReportCode code = (ReportCode)uiElement;
                    TextRange textRange = new TextRange(code.Richtxt_CustomCode.Document.ContentStart, code.Richtxt_CustomCode.Document.ContentEnd);
                    this.Report.Code = textRange.Text;
                    this.isValidValue = true;
                }
                else if (uiElement is ReportPageSetup)
                {
                    ReportPageSetup setup = (ReportPageSetup)uiElement;

                    RDL.DOM.Page  page= null;
                    if (this.Report.RDLType == RDLType.RDL2010)
                    {
                        page = this.Report.ReportSections.First().Page;
                    }
                    else
                    {
                        page = this.Report.Page;
                    }

                    page.PageHeight = new RDL.DOM.Size(setup.updwn_Height.Value + setup.txt_HeightUnit.Text);
                    page.PageWidth = new RDL.DOM.Size(setup.updwn_Width.Value + setup.txt_WidthUnit.Text);
                    page.TopMargin = new RDL.DOM.Size(setup.updwn_Top.Value + setup.txt_TopUnit.Text);
                    page.LeftMargin = new RDL.DOM.Size(setup.updwn_Left.Value + setup.txt_LeftUnit.Text);
                    page.BottomMargin = new RDL.DOM.Size(setup.updwn_Bottom.Value + setup.txt_BottomUnit.Text);
                    page.RightMargin = new RDL.DOM.Size(setup.updwn_Right.Value + setup.txt_RightUnit.Text);

                    {
                        UnitType = setup.UnitType;
                    }
                    this.isValidValue = true;
                }
                else if (uiElement is ReportVariables)
                {
                    if (this.Report.Variables == null)
                    {
                        this.Report.Variables = new RDL.DOM.Variables();
                    }

                    this.Report.Variables.Clear();
                    List<StackPanel> Panels = (uiElement as ReportVariables).lbx_Variables.Items.OfType<StackPanel>().ToList();

                    foreach(var panel in Panels)
                    {
                        List<System.Windows.Controls.TextBox> textbox = panel.Children.OfType<System.Windows.Controls.TextBox>().ToList();
                        this.isValidValue = (from txt in textbox
                                             where string.IsNullOrEmpty(txt.Text)
                                             select txt).Count() > 0 ? false : true;

                        if (this.isValidValue)
                        {
                            this.Report.Variables.Add(new RDL.DOM.Variable() { Name = textbox.First().Text, Value = textbox.Last().Text });
                        }
                        else
                        {
                            MessageBox.Show("<Specify Name or Value> \n " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.Error_title, MessageBoxButton.OK);
                            this.isValidValue = false;
                            return this.isValidValue;
                        }
                    }
                }
                else if (uiElement is ReportReferences)
                {
                    if (this.Report.CodeModules == null)
                    {
                        this.Report.CodeModules = new RDL.DOM.CodeModules();
                    }
                    if (this.Report.Classes == null)
                    {
                        this.Report.Classes = new RDL.DOM.Classes();
                    }

                    this.Report.Classes.Clear();
                    this.Report.CodeModules.Clear();
                    ReportReferences references = (ReportReferences)uiElement;
                    List<StackPanel> ClassPanels = references.lbx_Classes.Items.OfType<StackPanel>().ToList();
                    List<StackPanel> AssemblyPanels = references.lbx_Assemblies.Items.OfType<StackPanel>().ToList();

                    if (ClassPanels.Count > 0 )
                    {
                        foreach (var panel in ClassPanels)
                        {
                            List<System.Windows.Controls.TextBox> textbox = panel.Children.OfType<System.Windows.Controls.TextBox>().ToList();
                            this.isValidValue = (from txt in textbox
                                                 where string.IsNullOrEmpty(txt.Text)
                                                 select txt).Count() > 0 ? false : true;

                            if (this.isValidValue)
                            {
                                this.Report.Classes.Add(new RDL.DOM.Class() { ClassName = textbox.First().Text, InstanceName = textbox.Last().Text });
                                this.isValidValue = true;
                            }
                            else
                            {
                                MessageBox.Show("<Instance Name or Class Name>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.Error_title, MessageBoxButton.OK);
                                this.isValidValue = false;
                                return this.isValidValue;
                            }
                        }
                    }
                    if (AssemblyPanels.Count > 0)
                    {
                        foreach (StackPanel panel in AssemblyPanels)
                        {
                            System.Windows.Controls.TextBox textbox = panel.Children.OfType<System.Windows.Controls.TextBox>().FirstOrDefault();

                            if (!string.IsNullOrEmpty(textbox.Text))
                            {
                                this.Report.CodeModules.Add(new RDL.DOM.CodeModule() { Value = textbox.Text });
                                this.isValidValue = true;
                            }
                            else
                            {
                                MessageBox.Show("<Specify Assembly>\n " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.Error_title, MessageBoxButton.OK);
                                this.isValidValue = false;
                                return this.isValidValue;
                            }
                        }
                    }
                }
            }
            return this.isValidValue;
        }

        private void PopulateActionParameters()
        {
            if (ControlAction.rbtn_goToReport.IsChecked == true)
            {
                if (!string.IsNullOrEmpty(ControlAction.txt_reportName.Text))
                {
                    List<StackPanel> Panels = ControlAction.lbx_Parameters.Items.OfType<StackPanel>().ToList();
                    Parameters validParameters = new Parameters();

                    foreach (var panel in Panels)
                    {
                        System.Windows.Controls.TextBox label = panel.Children.OfType<System.Windows.Controls.TextBox>().FirstOrDefault();
                        ComboBox value = panel.Children.OfType<ComboBox>().ToList().FirstOrDefault();
                        this.isValidValue = string.IsNullOrEmpty(label.Text) == false ? true : false;

                        if (this.isValidValue)
                        {
                            validParameters.Add(new Parameter() { Name = label.Text, Value = value.Text });
                        }
                        else
                        {
                            MessageBox.Show("<Name> \n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.Error_title, MessageBoxButton.OK);
                            break;
                        }
                    }
                    if (this.isValidValue)
                    {
                        this.Parameters = validParameters;
                    }
                }
                else
                {
                    MessageBox.Show("<Specify a report>." + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.Error_title, MessageBoxButton.OK);
                    this.isValidValue = false;
                }
            }
            else if (ControlAction.rbtn_goToUrl.IsChecked == true)
            {
                if (string.IsNullOrEmpty(ControlAction.cmbx_UrlPath.Text))
                {
                    MessageBox.Show("<Select URL>\n " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.Error_title, MessageBoxButton.OK);
                    this.isValidValue = false;
                }
                else
                {
                    this.isValidValue = true;
                }
            }
            else
            {
                this.isValidValue = true;
            }
        }

        private bool PopulateTablixInformation()
        {
            foreach (UIElement uiElement in this.grd_PlaceHolder.Children)
            {
                if (uiElement is TablixFilters)
                {
                    TablixFilters filterUI = (TablixFilters)uiElement;
                    List<Grid> Panels = filterUI.lbx_Filters.Items.OfType<Grid>().ToList();
                    RDL.DOM.Filters Filters = new RDL.DOM.Filters();
                    this.isValidValue = true;

                    foreach (var panel in Panels)
                    {
                        List<ComboBox> expression = panel.Children.OfType<ComboBox>().ToList();
                        System.Windows.Controls.TextBox txt_value = panel.Children.OfType<System.Windows.Controls.TextBox>().ToList().FirstOrDefault();

                        if (!string.IsNullOrEmpty(expression.First().Text))
                        {
                            if (!string.IsNullOrEmpty(txt_value.Text))
                            {
                                string temp = txt_value.Text;

                                switch (expression.ElementAt(1).Text)
                                {
                                    case "Integer":
                                        this.isValidValue = (System.Text.RegularExpressions.Regex.IsMatch(temp, "^[-+]?[0-9]+$") == true) ? true : false;
                                        temp = "Value is not an Integer.";
                                        break;

                                    case "Float":
                                        this.isValidValue = (System.Text.RegularExpressions.Regex.IsMatch(temp, @"^[-+]?[0-9]*\.?[0-9]+$") == true) ? true : false;
                                        temp = "Value is not Float.";
                                        break;

                                    case "DateTime":
                                        this.isValidValue = (System.Text.RegularExpressions.Regex.IsMatch(temp,
                                                             @"^((0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|2\d)\d\d)\s*((0?[0-9]|1[0-9]|2[0-3])[:|.][0-5]?[\d])?\s*(([Aa]|[Pp])[Mm])?\s*$") == true) ? true : false;
                                        temp = "Value is not Data Time.";
                                        break;

                                    case "Boolean":
                                        if (temp.ToLower().Equals("true") || temp.ToLower().Equals("false"))
                                        {
                                            this.isValidValue = true;
                                        }
                                        else
                                        {
                                            this.isValidValue = false;
                                            temp = "Value is not Boolean.";
                                        }
                                        break;

                                    case "String":
                                        this.isValidValue = true;
                                        break;
                                }
                                if (this.isValidValue)
                                {
                                    RDL.DOM.FilterOperators opr = new RDL.DOM.FilterOperators();

                                    switch (expression.Last().Text)
                                    {
                                        case ">":
                                            opr = RDL.DOM.FilterOperators.GreaterThan;
                                            break;
                                        case "<":
                                            opr = RDL.DOM.FilterOperators.LessThan;
                                            break;
                                        case "<=":
                                            opr = RDL.DOM.FilterOperators.LessThanOrEqual;
                                            break;
                                        case ">=":
                                            opr = RDL.DOM.FilterOperators.GreaterThanOrEqual;
                                            break;
                                        case "In":
                                            opr = RDL.DOM.FilterOperators.In;
                                            break;
                                        case "Like":
                                            opr = RDL.DOM.FilterOperators.Like;
                                            break;
                                        case "<>":
                                            opr = RDL.DOM.FilterOperators.NotEqual;
                                            break;
                                        case "Top N":
                                            opr = RDL.DOM.FilterOperators.TopN;
                                            break;
                                        case "Bottom N":
                                            opr = RDL.DOM.FilterOperators.BottomN;
                                            break;
                                        case "Between":
                                            opr = RDL.DOM.FilterOperators.Between;
                                            break;
                                        case "Bottom %":
                                            opr = RDL.DOM.FilterOperators.BottomPercent;
                                            break;
                                        case "Top %":
                                            opr = RDL.DOM.FilterOperators.TopPercent;
                                            break;
                                    }

                                    FilterValues values = new FilterValues();
                                    values.Add(new FilterValue() { DataType =(RDL.DOM.DataTypes)expression.ElementAt(1).SelectedItem, Value = txt_value.Text });
                                    Filters.Add(new Filter() { FilterExpression = ConvertFieldToExpression(expression.First().Text), FilterValues = values, Operator = opr });
                                }
                                else
                                {
                                    MessageBox.Show("<Value> \n" + temp, this.Error_title, MessageBoxButton.OK);
                                    return this.isValidValue = false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("<Value> \n " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.Error_title, MessageBoxButton.OK);
                                return this.isValidValue = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("<Expression> \n " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.Error_title, MessageBoxButton.OK);
                            return this.isValidValue = false;
                        }
                    }
                    if (this.isValidValue && Filters.Count > 0)
                    {
                        this.Filters = Filters;
                    }
                }
                else if (uiElement is TablixSorting)
                {
                    TablixSorting sortingUI = (TablixSorting)uiElement;
                    List<StackPanel> Panels = sortingUI.lbx_SortingOptions.Items.OfType<StackPanel>().ToList();
                    RDL.DOM.SortExpressions expressions = new RDL.DOM.SortExpressions();
                    this.isValidValue = Panels.Count > 0 ? this.isValidValue : true;

                    foreach (var panel in Panels)
                    {
                        List<ComboBox> valueDirection = panel.Children.OfType<ComboBox>().ToList();
                        
                        if (!string.IsNullOrEmpty(valueDirection.First().Text))
                        {
                            var direction = valueDirection.Last().Text.Equals("A to Z") ? RDL.DOM.SortDirection.Ascending : RDL.DOM.SortDirection.Descending;
                            expressions.Add(new RDL.DOM.SortExpression() { Value = valueDirection.First().Text, Direction = direction });
                            this.isValidValue = true;
                        }
                        else
                        {
                            MessageBox.Show("<Sort by> \n " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.Error_title, MessageBoxButton.OK);
                            return this.isValidValue = false;
                        }
                    }
                    if (this.isValidValue && expressions.Count > 0)
                    {
                        this.SortExpressions = expressions;
                    }
                }
                else if (uiElement is TablixGeneral &&
                    (uiElement as TablixGeneral).grd_GroupGeneral.Visibility == System.Windows.Visibility.Visible)
                {
                    TablixGeneral general = (TablixGeneral)uiElement;
                    List<StackPanel> Panles = general.lbx_GroupExpression.Items.OfType<StackPanel>().ToList();

                    foreach (var panel in Panles)
                    {
                        ComboBox cmb_GroupExp = panel.Children.OfType<ComboBox>().ToList().First();

                        if (!string.IsNullOrEmpty(cmb_GroupExp.Text))
                        {
                            this.isValidValue = true;
                        }
                        else
                        {
                            MessageBox.Show("<Group By>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxGroupNotValid"),
                                            this.Error_title, MessageBoxButton.OK);
                            return this.isValidValue = false;
                        }
                    }
                }
                else if (uiElement is TablixVisibility)
                {
                    this.isValidValue = true;
                }
            }

            return this.isValidValue;
        }

        internal static string ConvertFieldToExpression(string expression)
        {
            if (expression.StartsWith("[") && expression.EndsWith("]"))
            {
                expression = expression.Replace("[", "=Fields!");
                expression = expression.Replace("]", ".Value");
            }

            return expression;
        }
    }
}
