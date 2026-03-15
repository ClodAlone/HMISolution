//-------------------------------------------------------------------------------------------------
// <copyright file="OlapGauge.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.Silverlight.Olap.Gauge
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using Syncfusion.OlapSilverlight.Data;
    using Syncfusion.OlapSilverlight.Reports;
    using Syncfusion.OlapSilverlight.Engine;
    using Syncfusion.Windows.Gauge;
    using Syncfusion.OlapSilverlight.Manager;
    using Syncfusion.Windows.Controls.Theming;
    using System.Globalization;
    using System.Windows.Media.Animation;
    using System.ServiceModel.Channels;
    using System.ServiceModel;


    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
    Type = typeof(OlapGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/BlackTheme.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
    Type = typeof(OlapGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/BlendTheme.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
    Type = typeof(OlapGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/BlueTheme.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
    Type = typeof(OlapGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/SilverTheme.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
    Type = typeof(OlapGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/Generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
    Type = typeof(OlapGauge), XamlResource = "/Syncfusion.OlapGauge.Silverlight;component/Themes/Office2003Theme.xaml")]

    /// <summary>
    /// The OlapGauge method is called when the Gauge is needed for representing the data report values.
    /// This helps in highlighting the KPI information through the gauge visualization. Rendering Gauges is done through
    /// </summary>
    public class OlapGauge : ContentControl
    {
        #region Private Variables
        private KpiInfoCollection _kpiInfoCollection;
        #endregion

        #region Dependency Property Implementation
        /// <summary>
        /// ColumnsCount Dependency Property
        /// </summary>
        public static readonly DependencyProperty ColumnsCountProperty =
            DependencyProperty.Register("ColumnsCount", typeof(int), typeof(OlapGauge), new PropertyMetadata(0, OnColumnsCountChanged));



        /// <summary>
        /// FrameType Dependency Property
        /// </summary>
        public static readonly DependencyProperty FrameTypeProperty =
            DependencyProperty.Register("FrameType", typeof(GaugeFrameType), typeof(OlapGauge), new PropertyMetadata(GaugeFrameType.Circular,OnFrameTypeChanged));

        /// <summary>
        /// Radius Dependency Property
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(OlapGauge), new PropertyMetadata(100.00, OnRadiusChanged));

        /// <summary>
        /// RowsCount Dependency Property
        /// </summary>
        public static readonly DependencyProperty RowsCountProperty =
            DependencyProperty.Register("RowsCount", typeof(int), typeof(OlapGauge), new PropertyMetadata(0, OnRowsCountChanged));

        /// <summary>
        /// ShowMarkersTooltip Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowMarkersTooltipProperty =
            DependencyProperty.Register("ShowMarkersTooltip", typeof(bool), typeof(OlapGauge), new PropertyMetadata(true,OnShowMarkersTooltipChanged));

        /// <summary>
        /// MajorTickForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty MajorTickForegroundProperty =
            DependencyProperty.Register("MajorTickForeground", typeof(Brush), typeof(OlapGauge), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// PointerBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty PointerBackgroundProperty =
            DependencyProperty.Register("PointerBackground", typeof(Brush), typeof(OlapGauge), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// MarkerBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty MarkerBackgroundProperty =
            DependencyProperty.Register("MarkerBackground", typeof(Brush), typeof(OlapGauge), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// Background Dependency Property
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(OlapGauge), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// ScaleBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty ScaleBackgroundProperty =
            DependencyProperty.Register("ScaleBackground", typeof(Brush), typeof(OlapGauge), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// LabelTickForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty LabelTickForegroundProperty =
            DependencyProperty.Register("LabelTickForeground", typeof(Brush), typeof(OlapGauge), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// MinorTickForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinorTickForegroundProperty =
            DependencyProperty.Register("MinorTickForeground", typeof(Brush), typeof(OlapGauge), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// PointerCapBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty PointerCapBackgroundProperty =
            DependencyProperty.Register("PointerCapBackground", typeof(Brush), typeof(OlapGauge), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// ShowPointersTooltip Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowPointersTooltipProperty =
            DependencyProperty.Register("ShowPointersTooltip", typeof(bool), typeof(OlapGauge), new PropertyMetadata(true, OnShowPointersTooltipChanged));

        /// <summary>
        /// SizeToContainer Dependency Property
        /// </summary>
        public static readonly DependencyProperty SizeToContainerProperty =
            DependencyProperty.Register("SizeToContainer", typeof(bool), typeof(OlapGauge), new PropertyMetadata(false));

       /// <summary>
       /// OlapDataManager Dependenc Property
       /// </summary>
        public static readonly DependencyProperty OlapDataManagerProperty =
            DependencyProperty.Register("OlapDataManager", typeof(OlapDataManager), typeof(OlapGauge), new PropertyMetadata(null, OnOlapDataManagerChanged));

        /// <summary>
        ///VisualStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(VisualStyle), typeof(OlapGauge), new PropertyMetadata(VisualStyle.Default, OnVisualStylePropertyChanged));

        /// <summary>
        /// Service Uri Dependency Property
        /// </summary>
        public static readonly DependencyProperty ServiceUriProperty =
    DependencyProperty.Register("ServiceUri", typeof(Uri), typeof(OlapGauge), new PropertyMetadata(null,
        (dependencyObject, args) =>
        {
            OlapGauge olapGauge = dependencyObject as OlapGauge;
            if (olapGauge != null)
            {
                System.ServiceModel.Channels.Binding customBinding = new System.ServiceModel.Channels.CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement { MaxReceivedMessageSize = 2147483647 });
                EndpointAddress address = new EndpointAddress((Uri)args.NewValue);
                ChannelFactory<IOlapDataProvider> channel = new ChannelFactory<IOlapDataProvider>(customBinding, address);
                olapGauge.DataProvider = channel.CreateChannel();
                if (olapGauge.DefaultReport != null)
                {
                    OlapDataManager olapDataManager = new OlapDataManager();
                    olapDataManager.DataProvider = olapGauge.DataProvider;
                    olapDataManager.SetCurrentReport(olapGauge.DefaultReport);
                    olapGauge.OlapDataManager = olapDataManager;
                    olapGauge.DataBind();
                }
            }
        }));


        /// <summary>
        /// CurrentReport Dependency Property
        /// </summary>
        public static readonly DependencyProperty CurrentReportProperty =
            DependencyProperty.Register("CurrentReport", typeof(OlapReport), typeof(OlapGauge), new PropertyMetadata(null,
                (dependencyObject, args) =>
                {
                    OlapGauge olapGauge = dependencyObject as OlapGauge;
                    if (olapGauge != null)
                    {
                        OlapDataManager olapDataManager = new OlapDataManager();
                        olapDataManager.DataProvider = olapGauge.DataProvider;
                        olapDataManager.SetCurrentReport((OlapReport)args.NewValue);
                        olapGauge.OlapDataManager = olapDataManager;
                        olapGauge.DataBind();
                    }
                    else
                        olapGauge.DefaultReport = (OlapReport)args.NewValue;
                }));

        #endregion

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGauge"/> class.
        /// </summary>
        public OlapGauge()
        {
            DefaultStyleKey = typeof(OlapGauge);
           
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the Pointer Background Brush.
        /// </summary>
        /// <value>Pointer Background Brush</value>
        public Brush PointerBackground
        {
            get { return (Brush)this.GetValue(PointerBackgroundProperty); }
            set { this.SetValue(PointerBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Marker Background Brush.
        /// </summary>
        /// <value>Marker Background Brush</value>
        public Brush MarkerBackground
        {
            get { return (Brush)this.GetValue(MarkerBackgroundProperty); }
            set { this.SetValue(MarkerBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Background Brush.
        /// </summary>
        /// <value>Background Brush</value>
        public Brush Background
        {
            get { return (Brush)this.GetValue(BackgroundProperty); }
            set { this.SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the PointerCap Background Brush.
        /// </summary>
        /// <value>PointerCap Background Brush</value>
        public Brush PointerCapBackground
        {
            get { return (Brush)this.GetValue(PointerCapBackgroundProperty); }
            set { this.SetValue(PointerCapBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Scale Background Brush.
        /// </summary>
        /// <value>Scale Background Brush</value>
        public Brush ScaleBackground
        {
            get { return (Brush)this.GetValue(ScaleBackgroundProperty); }
            set { this.SetValue(ScaleBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the LabelTick Background Brush.
        /// </summary>
        /// <value>LabelTick Background Brush</value>
        public Brush LabelTickForeground
        {
            get { return (Brush)this.GetValue(LabelTickForegroundProperty); }
            set { this.SetValue(LabelTickForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the MinorTick Background Brush.
        /// </summary>
        /// <value>MinorTick Background Brush</value>
        public Brush MinorTickForeground
        {
            get { return (Brush)this.GetValue(MinorTickForegroundProperty); }
            set { this.SetValue(MinorTickForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the MajorTick Background Brush.
        /// </summary>
        /// <value>MajorTick Background Brush</value>
        public Brush MajorTickForeground
        {
            get { return (Brush)this.GetValue(MajorTickForegroundProperty); }
            set { this.SetValue(MajorTickForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the columns count.
        /// </summary>
        /// <value>The columns count.</value>
        public int ColumnsCount
        {
            get { return (int)this.GetValue(ColumnsCountProperty); }
            set { this.SetValue(ColumnsCountProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of the frame.
        /// </summary>
        /// <value>The type of the frame.</value>
        [DefaultValue(GaugeFrameType.Circular)]
        public GaugeFrameType FrameType
        {
            get { return (GaugeFrameType)GetValue(FrameTypeProperty); }
            set { SetValue(FrameTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataManager to associate to the control.
        /// </summary>
        /// <value>The Olap Data Manager.</value>
        //public OlapDataManager OlapDataManager { get; set; }

        [Browsable(false)]
        public OlapDataManager OlapDataManager
        {
            get { return (OlapDataManager)GetValue(OlapDataManagerProperty); }
            set { SetValue(OlapDataManagerProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Temporary Report for the OlapGrid
        /// </summary>
        internal OlapReport DefaultReport { get; set; }

        /// <summary>
        /// Gets or Sets the DataProvider
        /// </summary>
        internal IOlapDataProvider DataProvider { get; set; }

        ///<summary>
        ///Gets or Sets the Service Path for the OlapClient
        ///</summary>
        [Browsable(false)]
        public Uri ServiceUri
        {
            get { return (Uri)GetValue(ServiceUriProperty); }
            set { SetValue(ServiceUriProperty, value); }
        }


        /// <summary>
        /// Gets or Sets the VisualStyle of the OlapClient
        /// </summary>
        public VisualStyle VisualStyle
        {
            get { return (VisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }


        /// <summary>
        /// Gets or Sets the CurrentReport for the OlapDataManager
        /// </summary>
        [Browsable(false)]
        public OlapReport CurrentReport
        {
            get { return (OlapReport)GetValue(CurrentReportProperty); }
            set { SetValue(CurrentReportProperty, value); }
        }



        private static void OnOlapDataManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OlapGauge gauge = d as OlapGauge;
            if (gauge != null)
            {
                if (gauge.OlapDataManager != null && gauge.OlapDataManager.CurrentCellSet != null)
                {
                    gauge.OlapDataManager.ExecuteOlapTable(GridLayout.NoSummaries);
                    gauge.DataBind(gauge.OlapDataManager.PivotEngine);
                }
                else
                {
                    gauge.DataBind();
                }
            }
        }

        /// <summary>
        /// Called when [Visual Style changed]
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnVisualStylePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapGauge = dependencyObject as OlapGauge;
            if (olapGauge != null)
            {
                switch ((VisualStyle)e.NewValue)
                {
                    case VisualStyle.Blend:
                        SkinManager.SetVisualStyle(dependencyObject, VisualStyle.Blend);
                        break;
                    case VisualStyle.Default:
                        SkinManager.SetVisualStyle(dependencyObject, VisualStyle.Default);
                        break;
                    case VisualStyle.Metro:
                        SkinManager.SetVisualStyle(dependencyObject, VisualStyle.Metro);
                        break;
                    case VisualStyle.Office2003:
                        SkinManager.SetVisualStyle(dependencyObject, VisualStyle.Office2003);
                        break;
                    case VisualStyle.Office2007Black:
                        SkinManager.SetVisualStyle(dependencyObject, VisualStyle.Office2007Black);
                        break;
                    case VisualStyle.Office2007Blue:
                        SkinManager.SetVisualStyle(dependencyObject, VisualStyle.Office2007Blue);
                        break;
                    case VisualStyle.Office2007Silver:
                        SkinManager.SetVisualStyle(dependencyObject, VisualStyle.Office2007Silver);
                        break;
                    default:
                        SkinManager.SetVisualStyle(dependencyObject, VisualStyle.Default);
                        break;
                }
            }
        }

        private static void OnFrameTypeChanged(DependencyObject dependencyObject,
                                           DependencyPropertyChangedEventArgs e)
        {
            OlapGauge gauge = dependencyObject as OlapGauge;
            if (gauge != null)
            {
                gauge.DataBind();
            }
        }

        private static void OnShowMarkersTooltipChanged(DependencyObject dependencyObject,
                                                 DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnShowPointersTooltipChanged(DependencyObject dependencyObject,
                                                DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnShowGaugeHeadersChanged(DependencyObject dependencyObject,
                                               DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnShowGaugeLabelsChanged(DependencyObject dependencyObject,
                                               DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnShowGaugeFactorsChanged(DependencyObject dependencyObject,
                                               DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }
        private static void OnRowsCountChanged(DependencyObject dependencyObject,
                                              DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }
        private static void OnColumnsCountChanged(DependencyObject dependencyObject,
                                              DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.DataBind();
            }
        }

        private static void OnRadiusChanged(DependencyObject dependencyObject,
                                            DependencyPropertyChangedEventArgs e)
        {
            OlapGauge olapgauge = dependencyObject as OlapGauge;
            if (olapgauge != null)
            {
                olapgauge.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        [DefaultValue(100.0)]
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the rows count.
        /// </summary>
        /// <value>The rows count.</value>
        public int RowsCount
        {
            get { return (int)this.GetValue(RowsCountProperty); }
            set { this.SetValue(RowsCountProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show markers tooltip].
        /// </summary>
        /// <value><c>true</c> if [show markers tooltip]; otherwise, <c>false</c>.</value>
        public bool ShowMarkersTooltip
        {
            get { return (bool)this.GetValue(ShowMarkersTooltipProperty); }
            set { this.SetValue(ShowMarkersTooltipProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show pointers tooltip].
        /// </summary>
        /// <value><c>true</c> if [show pointers tooltip]; otherwise, <c>false</c>.</value>
        public bool ShowPointersTooltip
        {
            get { return (bool)this.GetValue(ShowPointersTooltipProperty); }
            set { this.SetValue(ShowPointersTooltipProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [size to container].
        /// </summary>
        /// <value><c>true</c> if [size to container]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool SizeToContainer
        {
            get { return (bool)GetValue(SizeToContainerProperty); }
            set { SetValue(SizeToContainerProperty, value); }
        }

        /// <summary>
        /// Gets the Kpi Info Collection which holds the Information to render the Gauge.
        /// </summary>
        /// <value>The Collection which holds the Information</value>
        public KpiInfoCollection KpiInfoCollection
        {
            get
            {
                return _kpiInfoCollection;
            }
            internal set
            {

                if (value != this._kpiInfoCollection)
                {
                    _kpiInfoCollection = value;
                    if (_kpiInfoCollection != null)
                    {
                        this.Refresh();
                    }
                    else
                    {
                        this.Content = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether gauge controls is processing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is processing; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessing
        {
            get
            {
                if (this.GaugeProgressBar != null)
                    return this.GaugeProgressBar.IsIndeterminate;
                return false;
            }
            internal set
            {
                if (GaugeProgressBarPopup != null)
                {
                    try
                    {
                        /// if its processing the displaying the processing dialog
                        GaugeProgressBarPopup.IsOpen = value;
                        if (value == true)
                        {
                            Storyboard animation1 = GaugeProgressBarPopup.Resources["Storyboard1"] as Storyboard;
                            animation1.Begin();
                            Storyboard animation2 = GaugeProgressBarPopup.Resources["Storyboard2"] as Storyboard;
                            animation2.Begin();
                        }
                        //if (this.GaugeProgressBar != null)
                        //    this.GaugeProgressBar.IsIndeterminate = value;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Renders the default Gauge when OlapDataManager is not associated
        /// </summary>
        /// <returns>A default OlapGauge Control will be returned</returns>
        private Grid DefaultGauge()
        {
            Grid defaultGrid = new Grid();
            defaultGrid.HorizontalAlignment = HorizontalAlignment.Center;
            defaultGrid.VerticalAlignment = VerticalAlignment.Top;
            for (int i = 0; i < 1; i++)
            {
                defaultGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Auto) });
                for (int j = 0; j < 1; j++)
                {
                    defaultGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Auto) });
                    OlapCircularGauge olapCircularGauge = new OlapCircularGauge();
                    olapCircularGauge.VisualStyle = (GaugeVisualStyle)Enum.Parse(typeof(GaugeVisualStyle), SkinManager.GetVisualStyle(this).ToString(), false);
                    Grid gridCell = new Grid();
                    gridCell.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
                    gridCell.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) });
                    gridCell.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Auto) });
                    olapCircularGauge.Radius = 150;
                    olapCircularGauge.FrameType = this.FrameType;
                    olapCircularGauge.ShowMarkersTooltip = this.ShowMarkersTooltip;
                    olapCircularGauge.ShowPointersTooltip = this.ShowPointersTooltip;
                    Grid.SetRow(olapCircularGauge, 1);
                    gridCell.Children.Add(olapCircularGauge);
                    Grid.SetColumn(gridCell, j);
                    Grid.SetRow(gridCell, i);
                    defaultGrid.Children.Add(gridCell);
                }
            }
            return defaultGrid;
        }

        /// <summary>
        /// Gets the default columns count if Columns Count is not specified.
        /// </summary>
        /// <returns>The Columns Count</returns>
        private int GetDefaultColumnsCount()
        {
            return Convert.ToInt32((Math.Sqrt(double.Parse(Convert.ToString(this.KpiInfoCollection.Count, CultureInfo.CurrentCulture), CultureInfo.CurrentCulture))));
        }



        /// <summary>
        /// Gets the default rows count when RowsCount is not specified.
        /// </summary>
        /// <returns>The Rows Count</returns>
        private int GetDefaultRowsCount()
        {
            return this.KpiInfoCollection.Count / ColumnsCount;
        }

        /// <summary>
        /// Shows the Information if Kpi elements are empty.
        /// </summary>
        /// <returns>Returns a grid with Alert Message</returns>
        private static Grid ShowEmptyKpi()
        {
            Grid defaultGrid = new Grid();
            defaultGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Auto) });
            TextBlock kpiText = new TextBlock();
            kpiText.Margin = new Thickness(5d);
            kpiText.HorizontalAlignment = HorizontalAlignment.Left;
            kpiText.VerticalAlignment = VerticalAlignment.Center;
            kpiText.Text = "KPI Data Not Available";
            kpiText.Foreground = new SolidColorBrush(Colors.Red);
            kpiText.FontSize = 15;
            Grid.SetRow(kpiText, 0);
            defaultGrid.Children.Add(kpiText);
            return defaultGrid;
        }

        /// <summary>
        /// Adds the control in a user controled Layout and places it as the content of ScrollViewer
        /// and then to the main content
        /// </summary>
        private void AddGridLayout()
        {
            //Grid control to render the Multiple Gauge in a proper layout structure
            Grid multipleGaugeGrid = new Grid();
            multipleGaugeGrid.HorizontalAlignment = HorizontalAlignment.Left;
            multipleGaugeGrid.VerticalAlignment = VerticalAlignment.Top;
            bool breakloop = false;
            int kpiCount = 0;
            if (RowsCount < 1 || ColumnsCount < 1)
            {
                ColumnsCount = GetDefaultColumnsCount();
                RowsCount = GetDefaultRowsCount();
                //RowsCount = Convert.ToInt32(500 / (this.Radius));
                //ColumnsCount = Convert.ToInt32(500 / (this.Radius));
            }
            for (int i = 0; i < RowsCount; i++)
            {
                multipleGaugeGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Auto) });
                for (int j = 0; j < this.KpiInfoCollection.Count || kpiCount < this.KpiInfoCollection.Count; j++)
                {
                    multipleGaugeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Auto) });
                    if (j > (ColumnsCount - 1) || kpiCount >= this.KpiInfoCollection.Count)
                    {
                        breakloop = true;
                        break;
                    }
                    else
                    {
                        Grid gridCell = AddOlapGaugeControltoGrid(ref kpiCount, i, j);
                        //Adding the Grid Cell as the children of Main Grid.
                        multipleGaugeGrid.Children.Add(gridCell);
                        breakloop = false;
                    }

                    if (breakloop)
                    {
                        break;
                    }
                }
            }
            //Appending the grid content to the scrollviewer to product automatic scrollbars
            InternalScrollViewer.Content = multipleGaugeGrid;
        }

        /// <summary>
        /// Populates the OlapCircularGauge and their Header Text into the Grid Cells
        /// </summary>
        /// <returns>Returns a grid with OlapGauge Controls</returns>
        private Grid AddOlapGaugeControltoGrid(ref int kpiCount, int i, int j)
        {
            //Each OlapCircularGauge will be placed in a Grid Cell along with the Header Text 
            Grid gridCell = AddGridCell();
            //This text block holds the Header Text information.
            TextBlock kpiText = new TextBlock();
            kpiText.HorizontalAlignment = HorizontalAlignment.Center;
            kpiText.Margin = new Thickness(5, 0, 5, 0);
            kpiText.Text = KpiInfoCollection[kpiCount].Kpi_Name + " For " + KpiInfoCollection[kpiCount].MemberName;
            Grid.SetRow(kpiText, 0);
            //Invoking a new instance of OlapCircularGauge.
            OlapCircularGauge olapCircularGauge = new OlapCircularGauge();
            olapCircularGauge.VisualStyle = (GaugeVisualStyle)Enum.Parse(typeof(GaugeVisualStyle), SkinManager.GetVisualStyle(this).ToString(), false);
            //Setting the KpiInfo property which the holds an information to render a single OlapGauge.
            olapCircularGauge.KpiInfo = KpiInfoCollection[kpiCount];
            olapCircularGauge.Radius = this.Radius;
            olapCircularGauge.FrameType = this.FrameType;
            olapCircularGauge.ShowMarkersTooltip = this.ShowMarkersTooltip;
            olapCircularGauge.ShowPointersTooltip = this.ShowPointersTooltip;
            olapCircularGauge.Margin = new Thickness(15);
            //Binds the OlapCircularGauge with KpiInfo property
            olapCircularGauge.DataBind();
            Grid.SetRow(olapCircularGauge, 1);
            gridCell.Children.Add(kpiText);
            gridCell.Children.Add(olapCircularGauge);
            kpiCount++;
            Grid.SetColumn(gridCell, j);
            Grid.SetRow(gridCell, i);
            VisualStyle vs = SkinManager.GetVisualStyle(this);
            if (vs != VisualStyle.Default)
            {
                SkinManager.SetVisualStyle(olapCircularGauge, vs);
            }
            return gridCell;
        }

        /// <summary>
        /// Creates a new grid cell with default formatting
        /// </summary>
        /// <returns>Returns a grid with default Formatting</returns>
        private static Grid AddGridCell()
        {
            Grid gridCell = new Grid();
            gridCell.HorizontalAlignment = HorizontalAlignment.Center;
            gridCell.VerticalAlignment = VerticalAlignment.Top;
            gridCell.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            gridCell.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) });
            gridCell.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Auto) });
            return gridCell;
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Binds the OlapGauge with the information available in KpiInfoCollection
        /// </summary>
        /// <returns>The OlapGauge's embedded with Information on Above</returns>
        public void Refresh()
        {
            //this.IsProcessing = true;
            //if (this.IsProcessing == true)
            {
                if (this.KpiInfoCollection == null)
                    return;
                if (this.KpiInfoCollection.Count > 0)
                {
                    AddGridLayout();
                }
                else
                {
                    //Displays an string message showing KPI Data not Available
                    this.Content = ShowEmptyKpi();
                }
              //  this.IsProcessing = false;

            }
            //this.IsProcessing = false;
        }
        #endregion

        #region Internal Properties
        /// <summary>
        /// Progress bar popup
        /// </summary>
        internal ProgressBar GaugeProgressBar { get; set; }

        /// <summary>
        /// Gauge processing progress bar
        /// </summary>
        internal Popup GaugeProgressBarPopup { get; set; }

        /// <summary>
        /// Scroll bar used to render the ScrollBars
        /// </summary>
        internal ScrollViewer InternalScrollViewer { get; set; }
        #endregion

        #region Public Methods

        public void DataBind()
        {
            if (this.OlapDataManager != null)
            {
                this.UpdateLayout();
                this.IsProcessing = true;

                this.OlapDataManager.CellSetChanged += OlapDataManager_CellSetChanged;
                this.OlapDataManager.ExecuteCellSet();
            }
        }

        void OlapDataManager_CellSetChanged(object sender, CellSetChangedEventArgs e)
        {
            OlapDataManager.CurrentCellSet = e.NewCellSet;

            this.Dispatcher.BeginInvoke(delegate
            {
                OlapDataManager.ExecuteOlapTable(GridLayout.NoSummaries);
                DataBind(this.OlapDataManager.PivotEngine);
            });
        }

        private void DataBind(PivotEngine engine)
        {
            if (engine != null)
            {
                this.KpiInfoCollection = engine.GetValidKpis();
            }

            this.IsProcessing = false;
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //this. = GetTemplateChild("PART_OlapGauge") as OlapGauge;
            //// Getting the OlapGaugeProgressbar
            this.GaugeProgressBar = GetTemplateChild("PART_OlapGaugeProgressbar") as ProgressBar;
            //// Getting the OlapGaugeProgressbarPopup
            this.GaugeProgressBarPopup = GetTemplateChild("PART_OlapGaugeProgressbarPopup") as Popup;
            //// Getting the ScrollViewer 
            this.InternalScrollViewer = GetTemplateChild("PART_InternalScrollBar") as ScrollViewer;
            if (DesignerProperties.GetIsInDesignMode(this))
                this.InternalScrollViewer.Content = DefaultGauge();
            if (this.InternalScrollViewer != null && this.KpiInfoCollection != null)
            {
                //this.DataBind();
               this.Refresh();
            }
        }
    }
}
