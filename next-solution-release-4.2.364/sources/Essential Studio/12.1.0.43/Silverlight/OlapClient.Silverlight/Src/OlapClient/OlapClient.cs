#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.OlapSilverlight.Manager;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.Silverlight.Grid.Olap;
using Syncfusion.Silverlight.Tools.Olap;
using Syncfusion.OlapSilverlight.Common;
using Syncfusion.Silverlight.Chart.Olap;
using Syncfusion.OlapSilverlight.Reports;
using System.IO;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Chart;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using Syncfusion.Silverlight.Grid.Olap.Converter;
using Syncfusion.Windows.Controls.Theming;
using Syncfusion.Silverlight.Shared.Olap;
using Syncfusion.Silverlight.Grid.Olap.Common;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Globalization;
using Syncfusion.Silverlight.Client.Olap.Resources;
using Syncfusion.OlapSilverlight.Engine;

namespace Syncfusion.Silverlight.Client.Olap
{
    [StyleTypedProperty(Property = "AxisElementBuilderStyle", StyleTargetType = typeof(AxisElementBuilder))]
    [StyleTypedProperty(Property = "TabControlAdvStyle", StyleTargetType = typeof(TabControlAdv))]
    [StyleTypedProperty(Property = "TreeViewStyle", StyleTargetType = typeof(CubeDimensionBrowser))]
    [TemplatePart(Name = "PART_OlapClient", Type = typeof(OlapClient))]
    [DesignTimeVisible(true)]
    public class OlapClient : Control
    {
        #region Private members

        #region Client members
        private ReportNameGetter _reportNameGetter;
        private Image _fullScreen, _normal;

        #endregion

        #region Client toolbar members

        private OlapToolBarButton btnFullScreen, btnNewReport, btnAddReport, btnRemoveReport, btnReName, btnSave, btnLoad, btnShowExpander, btnTogglePivot, btnAutoExecute, btnEnablePaging, btnNewServer, btnShowMdx, btnCreateCalcMeasure, btnVirtualKpiElement;
        private GridLength _cubeAreaLength, _axisAreaLength;

        #endregion

        #region Chart toolbar members

        OlapToolBarButton btnShowChartToolTip, btnShowLegend;
        ComboBox cmbxChartTypes, cmbxChartPalette;

        #endregion

        #region Grid toolbar members

        OlapToolBarButton btnGridStyle, btnGridWrd, btnGridExl, btnGridPdf, btnValueTooltip, btnFrzHeader;
        ComboBox cmbxGridLayout;

        #endregion

        #region Drag drop manager

        private DragDropManager DragDropManager
        {
            get
            {
                if (this.OlapDataManager != null)
                    return this.OlapDataManager.DragDropManager;
                else
                    return null;
            }
            set
            {
                if (this.OlapDataManager != null)
                {
                    this.OlapDataManager.DragDropManager = value;
                }
            }
        }

        #endregion

        #region Progressbar members

        private Canvas _progressLayer;
        private ProgressBar _olapClientProgressBar;
        private Popup _olapClientProgressPopup;
        private Border _categoricalBorder;
        private Border _seriesBorder;
        private Border _slicerBorder;
        private Border _toolbarborder;
        private Border _tabborder;
        private Border _cubeborder;
        private Border _buttonborder;
        Storyboard animation1, animation2;
        int flag = 0;

        private bool Isprocessing
        {
            get
            {
                if (this._olapClientProgressPopup != null)
                {
                    return this._olapClientProgressPopup.IsOpen;
                }
                return false;
            }
            set
            {
                if (this._olapClientProgressPopup != null)
                {
                    this._olapClientProgressPopup.IsOpen = value;
                    if (value == true)
                    {
                        if(animation1 == null) animation1 = _olapClientProgressPopup.Resources["Storyboard1"] as Storyboard;
                        if (animation2 == null) animation2 = _olapClientProgressPopup.Resources["Storyboard2"] as Storyboard;

                        animation1.Begin();
                        animation2.Begin();
                        this._progressLayer.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this._progressLayer.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    //if (this._olapClientProgressBar != null)
                    //{
                    //    this._olapClientProgressBar.IsIndeterminate = value;
                    //    if (value)
                    //    {
                    //        this._progressLayer.Visibility = System.Windows.Visibility.Visible;
                    //    }
                    //    else
                    //    {
                    //        this._progressLayer.Visibility = System.Windows.Visibility.Collapsed;
                    //    }
                    //}
                }
            }
        }
        #endregion

        #region Exporting Members
        private ExportingGridStyleInfo _gridStyleInfo;
        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapClient"/> class.
        /// </summary>
        public OlapClient()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.DefaultStyleKey = typeof(OlapClient);
                this.BorderBrush = (Brush)new SolidColorBrush(Colors.Black);
                this.BorderThickness = new Thickness(0.5);
                this.CornerRadius = new CornerRadius(7);
                this.Background = (Brush)new SolidColorBrush(Colors.White);
                this.Loaded += new RoutedEventHandler(OlapClient_Loaded);
            }
            else
            {
                this.MinWidth = 120d;
                this.MinHeight = 24d;
            }
        }

        #endregion

        #region Dependency properties

        #region ShowToolBarsInOlapClient
        /// <summary>
        /// Gets or sets a value indicating whether to Show All ToolBars in OlapClient or not
        /// </summary>
        /// <value><c>true</c>[Show All ToolBars in OlapClient]; otherwise, <c>false</c>Hide All ToolBars in OlapClient</value>
        

        public bool ShowToolBarsInOlapClient
        {
            get { return (bool)GetValue(ShowToolBarsProperty); }
            set { SetValue(ShowToolBarsProperty, value); }
        }
        /// <summary>
        /// using ShowToolBarsInOlapClient Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowToolBarsProperty = DependencyProperty.Register("ShowToolBarsInOlapClient", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, (dependencyObject, args) =>
        {
            OlapClient olapclient = dependencyObject as OlapClient;
            if (olapclient != null)
            {
                if ((bool)args.NewValue)
                {
                    if (olapclient.OlapClientToolBar != null)
                        olapclient.OlapClientToolBar.Visibility = System.Windows.Visibility.Visible;
                    if (olapclient.OlapChartToolBar != null)
                        olapclient.OlapChartToolBar.Visibility = System.Windows.Visibility.Visible;
                    if (olapclient.OlapGridToolBar != null)
                        olapclient.OlapGridToolBar.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    if (olapclient.OlapClientToolBar != null)
                        olapclient.OlapClientToolBar.Visibility = System.Windows.Visibility.Collapsed;
                    if (olapclient.OlapChartToolBar != null)
                        olapclient.OlapChartToolBar.Visibility = System.Windows.Visibility.Collapsed;
                    if (olapclient.OlapGridToolBar != null)
                        olapclient.OlapGridToolBar.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
            ));
        #endregion

        #region AllowMultiMemberSelection
        /// <summary>
        /// Gets or sets a value indicating whether [show checkbox in MemberEditor TreeView].
        /// </summary>
        /// <value><c>true</c> if [show checkbox in MemberEditor TreeView]; otherwise, <c>false</c>.</value>
        public bool AllowMultiMemberSelection
        {
            get { return (bool)GetValue(MultiMemberSelectionProperty); }
            set { SetValue(MultiMemberSelectionProperty, value); }

        }
        /// <summary>
        /// using AllowMultiMemberSelection Dependency Property
        /// </summary>
        internal static readonly DependencyProperty MultiMemberSelectionProperty = DependencyProperty.Register("AllowMultiMemberSelection", typeof(bool), typeof(OlapClient), new PropertyMetadata(true));

        #endregion

        #region SplitButtonDisplayMode
        /// <summary>
        /// Gets or sets the SplitButton display mode.
        /// </summary>
        /// <value>SplitButton display mode</value>
        public SplitButtonDisplayMode AxisItemDisplayMode
	    {
            get { return (SplitButtonDisplayMode)GetValue(ShowAttributeNameProperty); }
            

            set { SetValue(ShowAttributeNameProperty,value); }
        }
        /// <summary>
        /// SplitButton display mode dependency property
        /// </summary>
        public static readonly DependencyProperty ShowAttributeNameProperty = DependencyProperty.Register("SplitButtonDisplayMode", typeof(SplitButtonDisplayMode), typeof(OlapClient), new PropertyMetadata(SplitButtonDisplayMode.WithoutHierarchyCaption));

        #endregion

        #region Axes Element Builder Ordering
        /// <summary>
        /// Gets or sets the order for the axes (Three type of Axis Element Builder) like Categorical(Or Column-C), Series(Or Row-R) and Slicer(Or Filter-F).
        /// Default order is CRF (Column, Row and Filter).
        /// </summary>
        public AxesOrder AxesOrder
        {
            get { return (AxesOrder)GetValue(AxesOrderProperty); }
            set { SetValue(AxesOrderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxesOrder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxesOrderProperty =
            DependencyProperty.Register("AxesOrder", typeof(AxesOrder), typeof(OlapClient), new PropertyMetadata(AxesOrder.CRF, OnAxesOrderChanged));

        private static void OnAxesOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null)
            {
                client.UpdateAxesOrder(client);
            }
        }
        #endregion

        #region Corner radius

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(OlapClient), new PropertyMetadata(null));

        #endregion

        #region Display Mode
        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        /// <value>The display mode.</value>
        public DisplayModes DisplayMode
        {
            get { return (DisplayModes)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        /// <summary>
        /// DisplayMode dependency Property
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(DisplayModes), typeof(OlapClient), new PropertyMetadata(DisplayModes.Both,
            (sender, e) => { OlapClient client = sender as OlapClient; if (client.OlapDataManager != null) { client.SetDisplayMode(client); } }));

        #endregion

        

        #region Can show cube selector
        /// <summary>
        /// Gets or sets whether cube selector part can be displayed in <see cref="OlapClient"/> control.
        /// </summary>
        public bool ShowCubeSelector
        {
            get { return (bool)GetValue(ShowCubeSelectorProperty); }
            set { SetValue(ShowCubeSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowCubeSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCubeSelectorProperty =
            DependencyProperty.Register("ShowCubeSelector", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, OnShowCubeSelectorChanged));

        private static void OnShowCubeSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null && client.CubeSelectorHolder != null)
            {
                client.CubeSelectorHolder.Visibility = (bool)args.NewValue ? Visibility.Visible : Visibility.Collapsed;
                client.CubeSelectorHeight.Height = (bool)args.NewValue ? new GridLength(50) : new GridLength(0);
            }
        }

        #endregion

        #region Can show cube dimension browser
        /// <summary>
        /// Gets or sets whether cube dimension browser part can be displayed in <see cref="OlapClient"/> control.
        /// </summary>
        public bool ShowCubeBrowser
        {
            get { return (bool)GetValue(ShowCubeBrowserProperty); }
            set { SetValue(ShowCubeBrowserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowCubeBrowser.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCubeBrowserProperty =
            DependencyProperty.Register("ShowCubeBrowser", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, OnShowCubeBrowserChanged));

        private static void OnShowCubeBrowserChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null && client.CubeBrowserHolder != null)
            {
                client.CubeBrowserHolder.Visibility = (bool)args.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #region Can show full screen button
        /// <summary>
        /// Gets or sets whether full screen button can be displayed in <see cref="OlapClient"/> control.
        /// </summary>
        public bool ShowFullScreenButton
        {
            get { return (bool)GetValue(ShowFullScreenButtonProperty); }
            set { SetValue(ShowFullScreenButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFullScreenButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowFullScreenButtonProperty =
            DependencyProperty.Register("ShowFullScreenButton", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, OnShowFullScreenButtonChanged));

        private static void OnShowFullScreenButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null && client.btnFullScreen != null)
            {
                client.btnFullScreen.Visibility = (bool)args.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region ShowReportButtons

        /// <summary>
        /// Gets or sets a value indicating whether [show report buttons].
        /// </summary>
        /// <value><c>true</c> if [show report buttons]; otherwise, <c>false</c>.</value>
        public bool ShowReportButtons
        {
            get { return (bool)GetValue(ShowReportButtonsProperty); }
            set { SetValue(ShowReportButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowReportButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowReportButtonsProperty =
            DependencyProperty.Register("ShowReportButtons", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, OnShowReportButtonsPropertyChanged));

        /// <summary>
        /// Called when [show report buttons property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowReportButtonsPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OlapClient client = sender as OlapClient;
            if (client.btnAddReport != null && client.btnRemoveReport != null && client.btnReName != null && client.btnNewReport != null)
            {
                if ((bool)e.NewValue)
                {
                    client.btnNewReport.Visibility = Visibility.Visible;
                    client.btnAddReport.Visibility = Visibility.Visible;
                    client.btnReName.Visibility = Visibility.Visible;
                    client.btnRemoveReport.Visibility = Visibility.Visible;
                    client.btnLoad.Visibility = Visibility.Visible;
                    client.btnSave.Visibility = Visibility.Visible;
                    client.ReportList.Visibility = Visibility.Visible;
                    client.btnShowMdx.Visibility = Visibility.Visible;
                }
                else
                {
                    client.btnNewReport.Visibility = Visibility.Collapsed;
                    client.btnAddReport.Visibility = Visibility.Collapsed;
                    client.btnReName.Visibility = Visibility.Collapsed;
                    client.btnRemoveReport.Visibility = Visibility.Collapsed;
                    client.btnLoad.Visibility = Visibility.Collapsed;
                    client.btnSave.Visibility = Visibility.Collapsed;
                    client.ReportList.Visibility = Visibility.Collapsed;
                    client.btnShowMdx.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region AutoExecute

        /// <summary>
        /// Gets or sets whether the elements will automatically execute the query.
        /// </summary>
        /// <value><c>true</c> if [auto execute]; otherwise, <c>false</c>.</value>
        public bool AutoExecute
        {
            get
            {
                return (bool)GetValue(AutoExecuteProperty);
            }

            set
            {
                SetValue(AutoExecuteProperty, value);
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoExecute
        /// </summary>
        public static readonly DependencyProperty AutoExecuteProperty = DependencyProperty.Register("AutoExecute", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, OnAutoExecuteChanged));

        /// <summary>
        /// Called when [execute button visibility changed].
        /// </summary>
        /// <param name="dependencyobj">The dependencyobj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAutoExecuteChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OlapClient olapClient = sender as OlapClient;
            if (olapClient.btnAutoExecute != null)
            {
                if (olapClient.AutoExecute)
                    olapClient.btnAutoExecute.Visibility = Visibility.Collapsed;
                else
                    olapClient.btnAutoExecute.Visibility = Visibility.Visible;
                olapClient.AxisElementBuilderColumn.AutoExecute = olapClient.AxisElementBuilderRow.AutoExecute = olapClient.AxisElementBuilderSlicer.AutoExecute = olapClient.AutoExecute;
            }
        }

        #endregion

        #region NonSSASData

        /// <summary>
        /// Gets or sets a value indicating whether this instance is non SSAS data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is non SSAS data; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("This property is no longer exist.")]
        public bool IsNonSSASData
        {
            get { return (bool)GetValue(IsNonSSASDataProperty); }
            set { SetValue(IsNonSSASDataProperty, value); }
        }
        [Obsolete("This property is no longer exist.")]
        public static readonly DependencyProperty IsNonSSASDataProperty =
            DependencyProperty.Register("IsNonSSASData", typeof(bool), typeof(OlapClient), new PropertyMetadata(false, OnIsNonSSASDataPropertyChanged));

        /// <summary>
        /// Called when [Is non SSAS data property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        [Obsolete("This method is no longer exist.")]
        private static void OnIsNonSSASDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client.OlapDataManager != null)
            {
                if ((bool)args.NewValue)
                    client.OlapDataManager.ProviderName = (client.OlapDataManager.ProviderName != Providers.ActivePivot) ? Providers.Mondrian : Providers.ActivePivot;
                else
                    client.OlapDataManager.ProviderName = Providers.SSAS;
            }
        }

        #endregion

        #region HeaderBackground

        /// <summary>
        /// Gets or sets the HeaderBackground.
        /// </summary>
        /// <value>The corner radius.</value>
        public Brush HeaderHoverBackground
        {
            get { return (Brush)GetValue(HeaderHoverBackgroundProperty); }
            set { SetValue(HeaderHoverBackgroundProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderHoverBackgroundProperty =
            DependencyProperty.Register("HeaderHoverBackground", typeof(Brush), typeof(OlapClient), new PropertyMetadata(null));
        #endregion

        #region HeaderBorder

        /// <summary>
        /// Gets or sets the VisualStyle.
        /// </summary>
        /// <value>The corner radius.</value>
        public Brush HeaderHoverBorderBrush
        {
            get { return (Brush)GetValue(HeaderHoverBorderBrushProperty); }
            set { SetValue(HeaderHoverBorderBrushProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderHoverBorderBrushProperty =
            DependencyProperty.Register("HeaderHoverBorderBrush", typeof(Brush), typeof(OlapClient), new PropertyMetadata(null));
        #endregion

        #region ButtonStyle

        /// <summary>
        /// Gets or sets the VisualStyle.
        /// </summary>
        /// <value>The corner radius.</value>
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(OlapClient), new PropertyMetadata(null));

        #endregion

        #region TreeViewItemStyle

        /// <summary>
        /// Gets or sets the VisualStyle.
        /// </summary>
        /// <value>The corner radius.</value>
        public Style TreeViewItemStyle1
        {
            get { return (Style)GetValue(TreeViewItemStyleProperty); }
            set { SetValue(TreeViewItemStyleProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TreeViewItemStyleProperty =
            DependencyProperty.Register("TreeViewItemStyle1", typeof(Style), typeof(OlapClient), new PropertyMetadata(null));

        #endregion

        #region VisualStyle

        /// <summary>
        /// Gets or sets the VisualStyle.
        /// </summary>
        /// <value>The corner radius.</value>
        public OlapClientVisualStyle VisualStyle
        {
            get { return (OlapClientVisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(OlapClientVisualStyle), typeof(OlapClient), new PropertyMetadata(OlapClientVisualStyle.Default, OnVisualStylePropertyChanged));



        private static void OnVisualStylePropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OlapClient client = sender as OlapClient;
            client.ApplyBrushes(client.VisualStyle.ToString(), true);
            if (client.OlapPager != null)
            {
                client.OlapPager.VisualStyle = (Syncfusion.Windows.Shared.VisualStyle)Enum.Parse(typeof(Syncfusion.Windows.Shared.VisualStyle), client.VisualStyle.ToString(), true);
            }
        }

        public Color GetColorFromHexaDecimal(string hexaColor)
        {
            return
                Color.FromArgb(
                    Convert.ToByte("ff", 16),
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16));
        }

        #endregion

        #region Enable/Disable Paging Support
        /// <summary>
        /// Gets or sets the olap pager.
        /// </summary>
        /// <value>The olap pager.</value>
        public OlapPager OlapPager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable paging].
        /// </summary>
        /// <value><c>true</c> if [enable paging]; otherwise, <c>false</c>.</value>
        public bool EnablePaging
        {
            get { return (bool)GetValue(EnablePagingProperty); }
            set { SetValue(EnablePagingProperty, value); }
        }

        //Using a DependencyProperty as the backing store for EnablePaging.
        public static readonly DependencyProperty EnablePagingProperty =
            DependencyProperty.Register("EnablePaging", typeof(bool), typeof(OlapClient), new PropertyMetadata(false, OnEnablePagingChanged));


        /// <summary>
        /// Called when [enable paging changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEnablePagingChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null)
            {
                if (client.OlapDataManager != null && client.OlapDataManager.CurrentReport != null)
                {
                    client.OlapDataManager.CurrentReport.EnablePaging = client.EnablePaging;
                    client.OlapDataManager.ExecuteCellSet();
                    client._tabborder.Margin = client.OlapDataManager.CurrentReport.EnablePaging ? new Thickness(0, 0, -10, -40) : new Thickness(0, 0, -10, -10);
                }
                
               if (client.OlapPager != null)
                {
                    client.OlapPager.Visibility = client.EnablePaging ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                   
                }
                   
            }
        }

        /// <summary>
        /// Gets or sets the boolean value for whether default member of a hierarchy used to avoid the results with its All Member.
        /// </summary>
        public bool UseDefaultMember
        {
            get { return (bool)GetValue(UseDefaultMemberProperty); }
            set { SetValue(UseDefaultMemberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseDefaultMember.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseDefaultMemberProperty =
            DependencyProperty.Register("UseDefaultMember", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, OnUseDefaultMemberChanged));


        /// <summary>
        /// Called when UseDefaultMemberChanged
        /// </summary>
        /// <param name="d">instance contains the target object</param>
        /// <param name="args">instance contain the event data</param>
        private static void OnUseDefaultMemberChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null)
            {
                if (client.OlapDataManager != null && client.OlapDataManager.CurrentReport != null)
                {
                    client.OlapDataManager.CurrentReport.UseDefaultMember = client.UseDefaultMember;
                    client.OlapDataManager.NotifyReportChanged();
                }
            }
        }


        /// <summary>
        /// Gets or set the boolean value for VisualTotalVisibility
        /// </summary>
        public bool VisualTotalVisibility
        {
            get { return (bool)GetValue(VisualTotalVisibilityProperty); }
            set { SetValue(VisualTotalVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisualTotalVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualTotalVisibilityProperty =
            DependencyProperty.Register("VisualTotalVisibility", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, OnVisualTotalVisibilityChanged));

        /// <summary>
        /// Called when VisualTotalVisibility Changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void OnVisualTotalVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null)
            {
                if (client.OlapDataManager != null && client.OlapDataManager.CurrentReport != null)
                {
                    client.OlapDataManager.CurrentReport.VisualTotalVisibility = client.VisualTotalVisibility;
                    client.OlapDataManager.NotifyReportChanged();
                }
            }
        }
        #endregion

        #region Connection string

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return (string)GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConnectionString.
        public static readonly DependencyProperty ConnectionStringProperty =
            DependencyProperty.Register("ConnectionString", typeof(string), typeof(OlapClient), new PropertyMetadata(null, OnConnectionStringChanged));

        private static void OnConnectionStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null && client.OlapDataManager != null)
            {
                client.OlapDataManager.ConnectionString = args.NewValue.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the name of the service provider.
        /// </summary>
        public Providers ProviderName
        {
            get { return (Providers)GetValue(ProviderNameProperty); }
            set { SetValue(ProviderNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProviderName.
        public static readonly DependencyProperty ProviderNameProperty =
            DependencyProperty.Register("ProviderName", typeof(Providers), typeof(OlapClient), new PropertyMetadata(Providers.SSAS, OnProviderNameChanged));

        private static void OnProviderNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null && client.OlapDataManager != null)
            {
                client.OlapDataManager.ProviderName = (Providers)args.NewValue;
            }
        }
        /// <summary>
        ///  Gets or sets a value indicating whether[IsMeasureEditorOpen]
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [IsMeasureEditorOpen]; otherwise, <c>false</c>.
        /// </value>
        public bool IsMeasureEditorOpen
        {
            get { return (bool)GetValue(IsMeasureEditorOpenProperty); }
            set { SetValue(IsMeasureEditorOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMeasureEditorOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMeasureEditorOpenProperty =
            DependencyProperty.Register("IsMeasureEditorOpen", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, IsMeasureEditorOpenChanged));

        private static void IsMeasureEditorOpenChanged(DependencyObject depObject, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = depObject as OlapClient;
            if (client != null)
            {
                if (client.AxisElementBuilderColumn != null)
                    client.AxisElementBuilderColumn.IsMeasureEditorOpen = (bool)args.NewValue;
                if (client.AxisElementBuilderRow != null)
                    client.AxisElementBuilderRow.IsMeasureEditorOpen = (bool)args.NewValue;
                if (client.AxisElementBuilderSlicer != null)
                    client.AxisElementBuilderSlicer.IsMeasureEditorOpen = (bool)args.NewValue;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether[IsMemberEditorOpen]
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [IsMemberEditorOpen]; otherwise, <c>false</c>.
        /// </value>
        public bool IsMemberEditorOpen
        {
            get { return (bool)GetValue(IsMemberEditorOpenProperty); }
            set { SetValue(IsMemberEditorOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMemberEditorOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMemberEditorOpenProperty =
            DependencyProperty.Register("IsMemberEditorOpen", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, IsMemberEditorOpenChanged));

        private static void IsMemberEditorOpenChanged(DependencyObject depObject, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = depObject as OlapClient;
            if (client != null)
            {
                if (client.AxisElementBuilderColumn != null)
                    client.AxisElementBuilderColumn.IsMemberEditorOpen = (bool)args.NewValue;
                if (client.AxisElementBuilderRow != null)
                    client.AxisElementBuilderRow.IsMemberEditorOpen = (bool)args.NewValue;
                if (client.AxisElementBuilderSlicer != null)
                    client.AxisElementBuilderSlicer.IsMemberEditorOpen = (bool)args.NewValue;
            }
        }
        #endregion

        #region Enable/Disable New Connection Support

        /// <summary>
        /// Gets or sets a value indicating whether [enabled connection option].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enabled connection option]; otherwise, <c>false</c>.
        /// </value>
        public bool EnabledConnectionOption
        {
            get { return (bool)GetValue(EnabledConnectionOptionProperty); }
            set { SetValue(EnabledConnectionOptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnabledConnectionOption.
        public static readonly DependencyProperty EnabledConnectionOptionProperty =
            DependencyProperty.Register("EnabledConnectionOption", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, OnNewConnectionOptionChanged));

        /// <summary>
        /// Called when [new connection option changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNewConnectionOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
           
               if (client != null && client.btnNewServer != null)
                {
                   if (!(bool)args.NewValue)
                   {
                       client.btnNewServer.Visibility = Visibility.Collapsed;
                   }
                   else
                   {
                       client.btnNewServer.Visibility = Visibility.Visible;
                   }
                }
          
        }

        #endregion

        #region Enable/Disble the Virtualkpi element

        /// <summary>
        /// Gets or sets a value indicating whether the Virtual Kpi support are to be enabled.
        /// </summary>
        /// <value>
        /// <c>true</c>if virtual kpi support are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsVirtualKpiEnabled
        {
            get { return (bool)GetValue(IsVirtualKpiEnabledProperty); }
            set { SetValue(IsVirtualKpiEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsVirtualKpiEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVirtualKpiEnabledProperty =
            DependencyProperty.Register("IsVirtualKpiEnabled", typeof(bool), typeof(OlapClient), new PropertyMetadata(false, new PropertyChangedCallback(OnVirtualKpiEnabled)));

        private static void OnVirtualKpiEnabled(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null)
            {
                if (client.btnVirtualKpiElement != null)
                {
                    if ((bool)args.NewValue)
                    {
                        client.btnVirtualKpiElement.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        client.btnVirtualKpiElement.Visibility = Visibility.Collapsed;
                        if (client.OlapDataManager != null && client.OlapDataManager.CurrentReport != null)
                        {
                            client.OlapDataManager.CurrentReport.VirtualKpiElements.Clear();
                            client.CubeDimensionBrowser.Refresh();

                            //// Refresh the Axis Element build items if any.
                            var virtualKpis = client.AxisElementBuilderColumn.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.VirtualKPIMember).ToList();
                            if (virtualKpis.Count > 0)
                            {
                                foreach (var item in virtualKpis)
                                {
                                    client.AxisElementBuilderColumn.MetaTreeNodes.Remove(item);
                                    client.AxisElementBuilderColumn.Synchronize(client.AxisElementBuilderColumn.ReportItems, item);
                                }
                            }

                            virtualKpis = client.AxisElementBuilderRow.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.VirtualKPIMember).ToList();
                            if (virtualKpis.Count > 0)
                            {
                                foreach (var item in virtualKpis)
                                {
                                    client.AxisElementBuilderRow.MetaTreeNodes.Remove(item);
                                    client.AxisElementBuilderRow.Synchronize(client.AxisElementBuilderRow.ReportItems, item);
                                }
                            }

                            virtualKpis = client.AxisElementBuilderSlicer.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.VirtualKPIMember).ToList();
                            if (virtualKpis.Count > 0)
                            {
                                foreach (var item in virtualKpis)
                                {
                                    client.AxisElementBuilderSlicer.MetaTreeNodes.Remove(item);
                                    client.AxisElementBuilderSlicer.Synchronize(client.AxisElementBuilderSlicer.ReportItems, item);
                                }
                            }

                            client.OlapDataManager.NotifyElementChanged();
                        }
                    }
                }
            }
        }
#endregion

        #region Enable/Disable the Calculated Members

        /// <summary>
        /// Gets or sets a value indicating whether the calculated members are to be enabled.
        /// </summary>
        /// <value>
        /// <c>true</c>if calculated members are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCalculatedMembersEnabled
        {
            get { return (bool)GetValue(IsCalculatedMembersEnabledProperty); }
            set { SetValue(IsCalculatedMembersEnabledProperty, value); }
        }

        private static readonly DependencyProperty IsCalculatedMembersEnabledProperty =
            DependencyProperty.Register("IsCalculatedMembersEnabled", typeof(bool), typeof(OlapClient), new PropertyMetadata(false, new PropertyChangedCallback(OnCalculatedMembersEnabled)));

        private static void OnCalculatedMembersEnabled(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null)
            {
                if ((bool)args.NewValue)
                {
                    if(client.btnCreateCalcMeasure!=null)

                        client.btnCreateCalcMeasure.Visibility = Visibility.Visible;
                }
                else
                {
                    client.btnCreateCalcMeasure.Visibility = Visibility.Collapsed;

                    if (client.OlapDataManager != null && client.OlapDataManager.CurrentReport != null)
                    {
                        client.OlapDataManager.CurrentReport.CalculatedMembers.Clear();
                        client.CubeDimensionBrowser.Refresh();

                        //// Refresh the Axis Element build items if any.
                        var calcMembers = client.AxisElementBuilderColumn.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.CalculatedMember).ToList();
                        if (calcMembers.Count > 0)
                        {
                            foreach (var item in calcMembers)
                            {
                                client.AxisElementBuilderColumn.MetaTreeNodes.Remove(item);
                                client.AxisElementBuilderColumn.Synchronize(client.AxisElementBuilderColumn.ReportItems, item);
                            }                            
                        }

                        calcMembers = client.AxisElementBuilderRow.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.CalculatedMember).ToList();
                        if (calcMembers.Count > 0)
                        {
                            foreach (var item in calcMembers)
                            {
                                client.AxisElementBuilderRow.MetaTreeNodes.Remove(item);
                                client.AxisElementBuilderRow.Synchronize(client.AxisElementBuilderRow.ReportItems, item);
                            }
                        }

                        calcMembers = client.AxisElementBuilderSlicer.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.CalculatedMember).ToList();
                        if (calcMembers.Count > 0)
                        {
                            foreach (var item in calcMembers)
                            {
                                client.AxisElementBuilderSlicer.MetaTreeNodes.Remove(item);
                                client.AxisElementBuilderSlicer.Synchronize(client.AxisElementBuilderSlicer.ReportItems, item);
                            }                            
                        }

                        client.OlapDataManager.NotifyElementChanged();
                    }
                }
            }
        }

        #endregion

        #region Grid Layout settings

        /// <summary>
        /// Gets or sets value for GridLayout 
        /// </summary>
        public GridLayout GridLayout
        {
            get { return (GridLayout)GetValue(GridLayoutProperty); }
            set { SetValue(GridLayoutProperty, value); }
        }

        
        // Using a DependencyProperty as the backing store for GridLayout.  This enables animation, styling, binding, etc...
        /// <summary>
        /// GridLayout dependency property
        /// </summary>
        public static readonly DependencyProperty GridLayoutProperty =
            DependencyProperty.Register("GridLayout", typeof(GridLayout), typeof(OlapClient), new PropertyMetadata(GridLayout.Normal, OnGridLayoutChanged));

        private static void OnGridLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapClient client = d as OlapClient;
            if (client != null && client.OlapGrid != null)
            {
                client.OlapGrid.Layout = (GridLayout)args.NewValue;
                if (client.cmbxGridLayout != null)
                {
                    if ((GridLayout)args.NewValue == GridLayout.ExcelLikeLayout)
                        client.cmbxGridLayout.SelectedIndex = 1;
                    else if ((GridLayout)args.NewValue == GridLayout.NoSummaries)
                        client.cmbxGridLayout.SelectedIndex = 2;
                    else if ((GridLayout)args.NewValue == GridLayout.NormalTopSummary)
                        client.cmbxGridLayout.SelectedIndex = 3;
                    else if ((GridLayout)args.NewValue == GridLayout.Normal)
                        client.cmbxGridLayout.SelectedIndex = 0;
                }
            }
        
        }
        #endregion

        /// <summary>
        /// Gets or set a value indication whether use where clause or sub select clause for slicing
        /// </summary>
        public bool UseWhereClauseForSlicing
        {
            get { return (bool)GetValue(UseWhereClauseForSlicingProperty); }
            set { SetValue(UseWhereClauseForSlicingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseWhereClauseForSlicing. ...
        public static readonly DependencyProperty UseWhereClauseForSlicingProperty =
            DependencyProperty.Register("UseWhereClauseForSlicing", typeof(bool), typeof(OlapClient), new PropertyMetadata(true, new PropertyChangedCallback(
                (obj, args) =>
                {
                    OlapClient olapClient = obj as OlapClient;
                    if (olapClient != null)
                    {
                        if (olapClient.OlapDataManager != null && olapClient.OlapDataManager.CurrentReport != null)
                        {
                            olapClient.OlapDataManager.CurrentReport.UseWhereClauseForSlicing = olapClient.UseWhereClauseForSlicing;
                        }
                    }
                })));

        #region OlapDataManager
        /// <summary>
        /// Gets or sets the olap data manager.
        /// </summary>
        /// <value>The olap data manager.</value>
        [Browsable(false)]
        public OlapDataManager OlapDataManager
        {
            get { return (OlapDataManager)GetValue(OlapDataManagerExProperty); }
            set { SetValue(OlapDataManagerExProperty, value); }
        }

        /// <summary>
        /// OlapDataManager Dependency Property
        /// </summary>
        public static readonly DependencyProperty OlapDataManagerExProperty =
            DependencyProperty.Register("OlapDataManager", typeof(OlapDataManager), typeof(OlapClient), new PropertyMetadata(null, (dependencyObject, args) =>
            {
                OlapClient olapClient = dependencyObject as OlapClient;
                if (olapClient != null && olapClient.OlapDataManager != null)
                {
                    if (olapClient.IsNonSSASData)
                        olapClient.OlapDataManager.ProviderName = olapClient.OlapDataManager.ProviderName != Providers.ActivePivot ? Providers.Mondrian : Providers.ActivePivot;
                    olapClient.OlapDataManager.ConnectionString = olapClient.ConnectionString;

                    //// Tagging manager events 
                    olapClient.TagManagerEvents();
                    //// Binding the OlapDataManager to internal components
                    olapClient.BindManagerToControls();
                    ///Enables the paging in CurrentReport with respect to OlapClient
                    if (olapClient.OlapDataManager.CurrentReport != null)
                    {
                        if (!olapClient.OlapDataManager.CurrentReport.EnablePaging)
                            olapClient.OlapDataManager.CurrentReport.EnablePaging = olapClient.EnablePaging;

                        if (olapClient.OlapDataManager.CurrentReport.UseDefaultMember)
                            olapClient.OlapDataManager.CurrentReport.UseDefaultMember = olapClient.UseDefaultMember;

                        if (olapClient.OlapDataManager.CurrentReport.VisualTotalVisibility)
                            olapClient.OlapDataManager.CurrentReport.VisualTotalVisibility = olapClient.VisualTotalVisibility;
                   
                        if (!olapClient.UseWhereClauseForSlicing)
                            olapClient.OlapDataManager.CurrentReport.UseWhereClauseForSlicing = olapClient.UseWhereClauseForSlicing;
                    }
                }
            }));

        #endregion

        #region ServiceUri
        /// <summary>
        /// Gets or Sets the Service Path for the OlapClient
        /// </summary>
        [Browsable(false)]
        public Uri ServiceUri
        {
            get { return (Uri)GetValue(ServiceUriProperty); }
            set { SetValue(ServiceUriProperty, value); }
        }

        /// <summary>
        /// ServiceUri dependency property
        /// </summary>
        public static readonly DependencyProperty ServiceUriProperty =
            DependencyProperty.Register("ServiceUri", typeof(Uri), typeof(OlapClient), new PropertyMetadata(null,
                (dependencyObject, args) =>
                {
                    OlapClient olapClient = dependencyObject as OlapClient;
                    if (olapClient != null)
                    {
                        System.ServiceModel.Channels.Binding customBinding = new System.ServiceModel.Channels.CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement { MaxReceivedMessageSize = 2147483647 });
                        EndpointAddress address = new EndpointAddress((Uri)args.NewValue);
                        ChannelFactory<IOlapDataProvider> channel = new ChannelFactory<IOlapDataProvider>(customBinding, address);
                        olapClient.DataProvider = channel.CreateChannel();
                        if (olapClient.TempReport != null)
                        {
                            OlapDataManager olapDataManager = new OlapDataManager();
                            olapDataManager.DataProvider = olapClient.DataProvider;
                            olapDataManager.SetCurrentReport(olapClient.TempReport);
                            olapClient.OlapDataManager = olapDataManager;
                            olapClient.DataBind();
                        }
                    }
                }));

        #endregion

        #region CurrentReport
        /// <summary>
        /// Gets or Sets the CurrentReport for OlapDataManager of OlapClient
        /// </summary>
        [Browsable(false)]
        public OlapReport CurrentReport
        {
            get { return (OlapReport)GetValue(CurrentReportProperty); }
            set { SetValue(CurrentReportProperty, value); }
        }

        /// <summary>
        /// CurrentReport dependency property
        /// </summary>
        public static readonly DependencyProperty CurrentReportProperty =
            DependencyProperty.Register("CurrentReport", typeof(OlapReport), typeof(OlapClient), new PropertyMetadata(null,
                (dependencyObject, args) =>
                {
                    OlapClient olapClient = dependencyObject as OlapClient;
                    if (olapClient != null)
                    {
                        OlapDataManager olapDataManager = new OlapDataManager();
                        olapDataManager.DataProvider = olapClient.DataProvider;
                        olapDataManager.SetCurrentReport((OlapReport)args.NewValue);
                        olapClient.OlapDataManager = olapDataManager;
                        olapClient.DataBind();
                    }
                    else
                        olapClient.TempReport = (OlapReport)args.NewValue;
                }));

        #endregion

        #endregion

        #region Internal Properties
        /// <summary>
        /// Gets or set the holder for cube selector space. This will be used to change its visibility.
        /// </summary>
        internal System.Windows.Controls.Grid CubeSelectorHolder { get; set; }

        /// <summary>
        /// Gets or set the Rowheight for the CubeSelector Space
        /// </summary>
        internal System.Windows.Controls.RowDefinition CubeSelectorHeight { get; set; }

        /// <summary>
        /// Gets or set the holder for cube dimension browser space. This will be used to change its visibility.
        /// </summary>
        internal System.Windows.Controls.Grid CubeBrowserHolder { get; set; }

        internal Border MainBorder { get; set; }

        /// <summary>
        /// Gets or Sets the Temporary Report
        /// </summary>
        internal OlapReport TempReport { get; set; }

        /// <summary>
        /// Gets or Sets the DataProvider
        /// </summary>
        internal IOlapDataProvider DataProvider { get; set; }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the axis element builder column.
        /// </summary>
        /// <value>The axis element builder column.</value>
        public AxisElementBuilder AxisElementBuilderColumn { get; set; }

        /// <summary>
        /// Gets or sets the axis element builder row.
        /// </summary>
        /// <value>The axis element builder row.</value>
        public AxisElementBuilder AxisElementBuilderRow { get; set; }

        /// <summary>
        /// Gets or sets the axis element builder slicer.
        /// </summary>
        /// <value>The axis element builder slicer.</value>
        public AxisElementBuilder AxisElementBuilderSlicer { get; set; }

        /// <summary>
        /// Gets or sets the report list.
        /// </summary>
        /// <value>The report list.</value>
        public ComboBox ReportList { get; set; }

        /// <summary>
        /// Gets or sets the olap tab control.
        /// </summary>
        /// <value>The olap tab control.</value>
        public TabControlAdv OlapTabControl { get; set; }

        /// <summary>
        /// Gets or sets the olap chart tab.
        /// </summary>
        /// <value>The olap chart tab.</value>
        public TabItemAdv OlapChartTab { get; set; }

        /// <summary>
        /// Gets or sets the olap grid tab.
        /// </summary>
        /// <value>The olap grid tab.</value>
        public TabItemAdv OlapGridTab { get; set; }

        /// <summary>
        /// Gets or sets the olap client tool bar.
        /// </summary>
        /// <value>The olap client tool bar.</value>
        public OlapToolBar OlapClientToolBar { get; set; }

        /// <summary>
        /// Gets or sets the olap grid tool bar.
        /// </summary>
        /// <value>The olap grid tool bar.</value>
        public OlapToolBar OlapGridToolBar { get; set; }

        /// <summary>
        /// Gets or sets the olap chart tool bar.
        /// </summary>
        /// <value>The olap chart tool bar.</value>
        public OlapToolBar OlapChartToolBar { get; set; }

        /// <summary>
        /// Gets or sets the cube selector.
        /// </summary>
        /// <value>The cube selector.</value>
        public ComboBox CubeSelector { get; set; }

        /// <summary>
        /// Gets or sets the olap grid.
        /// </summary>
        /// <value>The olap grid.</value>
        public Syncfusion.Silverlight.Grid.Olap.OlapGrid OlapGrid { get; set; }

        /// <summary>
        /// Gets or sets the olap chart.
        /// </summary>
        /// <value>The olap chart.</value>
        public OlapChart OlapChart { get; set; }

        /// <summary>
        /// Gets or sets the cube dimension browser.
        /// </summary>
        /// <value>The cube dimension browser.</value>
        public CubeDimensionBrowser CubeDimensionBrowser { get; set; }

        //Added for Blendability support
        Style builderstyle;
        Style tabstyle;
        Style treeviewstyle;
        public Style AxisElementBuilderStyle
        {
            get
            {
                return builderstyle;
            }
            set
            {
                builderstyle = value;
            }
        }
        public Style TabControlAdvStyle
        {
            get
            {
                return tabstyle;
            }
            set
            {
                tabstyle = value;
            }
        }
        public Style TreeViewStyle
        {
            get
            {
                return treeviewstyle;
            }
            set
            {
                treeviewstyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the value for Sort order of Measures in CubeDimensionBrowser
        /// </summary>
        public SortCubeMeasureOrder MeasureSortOrderInCubeBrowser { get; set; }
        
        #endregion

        #region Data binding

        /// <summary>
        /// Binds the OlapData with OlapClient and display the report.
        /// </summary>
        public void DataBind()
        {
            try
            {
                this.OlapDataManager.ReportList.Clear();
                this.OlapDataManager.GetCubes();
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_BindDataManager"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        #endregion

        #region Binding manager to controls

        /// <summary>
        /// Tags events related to OlapDataManager 
        /// </summary>
        private void TagManagerEvents()
        {
            this.OlapDataManager.CubeInfoCollectionChanged -= CubeInfoCollectionChanged;
            this.OlapDataManager.CubeInfoCollectionChanged += CubeInfoCollectionChanged;
            this.OlapDataManager.CubeChanged -= CubeChanged;
            this.OlapDataManager.CubeChanged += CubeChanged;
            this.OlapDataManager.CellSetChanging -= CellSetChanging;
            this.OlapDataManager.CellSetChanging += CellSetChanging;
            this.OlapDataManager.CellSetChanged += CellSetChanged;
            this.OlapGrid.AfterRefresh += new OlapGridDrillDownEventHander(OlapGrid_AfterRefresh);
            this.OlapChart.OnDataRefreshCompleted += new Chart.Olap.OlapChart.DataRefreshCompleted(OlapChart_OnDataRefreshCompleted);
            this.OlapDataManager.OnError += new OlapSilverlight.Manager.OlapDataManager.EventHandler(OlapDataManager_OnError);
            this.OlapDataManager.CubeSchemaChanged -= OlapDataManager_CubeSchemaChanged;
            this.OlapDataManager.CubeSchemaChanged += OlapDataManager_CubeSchemaChanged;
            this.OlapDataManager.MdxQueryObtained -= new MdxObtainedEventHandler(OlapDataManager_MdxQueryObtained);
            this.OlapDataManager.MdxQueryObtained += new MdxObtainedEventHandler(OlapDataManager_MdxQueryObtained);
        }

        void OlapDataManager_MdxQueryObtained()
        {
            this.Isprocessing = false;
            MdxDialog mdxDlg = new MdxDialog(this.OlapDataManager.CurrentReport.CurrentMdxQuery);
            mdxDlg.FlowDirection = this.FlowDirection;
            mdxDlg.Title = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_MDXDialog_Title");
            GradientStopCollection gradiantCollection = new GradientStopCollection();
            mdxDlg.VisualStyle = (Windows.Shared.VisualStyle)Enum.Parse(typeof(Windows.Shared.VisualStyle),this.VisualStyle.ToString(),true);
            mdxDlg.Background = this.Background;
            SkinManager.SetVisualStyle(mdxDlg,(Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Windows.Controls.Theming.VisualStyle),this.VisualStyle.ToString(),true));
            mdxDlg.ShowDialog();
        }

        // These temp. variables for checking whether Grid/Chart loaded before CubeInfoCollection ( Cube dimension browser) 
        bool cubedimbrowserUpdated;
        bool chartOrGridUpdated;
        void OlapDataManager_CubeSchemaChanged(object sender, CubeSchemaChangedEventArgs e)
        {
            if (this.CubeDimensionBrowser != null)
            {                
                this.CubeDimensionBrowser.Refresh();
                this.cubedimbrowserUpdated = true;
            }

            if (this.chartOrGridUpdated)
            {
                this.Isprocessing = false;
            }
        }

        void OlapChart_OnDataRefreshCompleted(object sender, DataRefreshCompletedEventArgs e)
        {
            if (this.cubedimbrowserUpdated)
            {
                if (this.DisplayMode != DisplayModes.GridOnly)
                    this.Isprocessing = false;
                if (this.OlapDataManager != null && this.OlapPager != null)
                {
                    this.btnEnablePaging.IsChecked = this.OlapDataManager.CurrentReport.EnablePaging;
                    this.OlapPager.Visibility = this.OlapDataManager.CurrentReport.EnablePaging ? Visibility.Visible : Visibility.Collapsed;
                }
                if (this.OlapDataManager != null && !this.UseWhereClauseForSlicing)
                    this.OlapDataManager.CurrentReport.UseWhereClauseForSlicing = this.UseWhereClauseForSlicing;
            }
            else
            {
                this.chartOrGridUpdated = true;
            }
        }

        void OlapGrid_AfterRefresh(object sender, OlapGridDrillDownEventArgs e)
        {
            if (this.cubedimbrowserUpdated)
            {
                if (this.DisplayMode == DisplayModes.GridOnly)
                    this.Isprocessing = false;
                if (this.OlapDataManager != null && this.OlapPager != null)
                {
                    this.btnEnablePaging.IsChecked = this.OlapDataManager.CurrentReport.EnablePaging;
                    this.OlapPager.Visibility = this.OlapDataManager.CurrentReport.EnablePaging ? Visibility.Visible : Visibility.Collapsed;
                }
                if (this.OlapDataManager != null && !this.UseWhereClauseForSlicing)
                    this.OlapDataManager.CurrentReport.UseWhereClauseForSlicing = this.UseWhereClauseForSlicing;
            }
            else
            {
                this.chartOrGridUpdated = true;
            }
        }

        void OlapDataManager_OnError(object sender, OlapDataManager.ErrorEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (this.Isprocessing)
                {
                    this.Isprocessing = false;
                }
                WindowControl.ShowAlert(e.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_ServiceCall"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            });
        }

        /// <summary>
        /// Binds the OlapDataManager to internal controls.
        /// </summary>
        private void BindManagerToControls()
        {
            try
            {
                this.CubeDimensionBrowser.OlapDataManager = this.OlapDataManager;
                this.AxisElementBuilderColumn.OlapDataManager = this.OlapDataManager;
                this.AxisElementBuilderRow.OlapDataManager = this.OlapDataManager;
                this.AxisElementBuilderSlicer.OlapDataManager = this.OlapDataManager;
                this.OlapPager.OlapDataManager = this.OlapDataManager;
                this.ReportList.ItemsSource = this.OlapDataManager.ReportList;
                this.SetDisplayMode(this);
                this.UpdateVirtualKpiElement();
                this.UpdateCalculatedMembers();
                this.OlapDataManager.GetCubes();
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_BindDataManager"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Updates the visibility of OLAP Pager control.
        /// </summary>
        private void UpdatePagerVisibility()
        {
            if (this.OlapDataManager.CurrentReport != null && !string.IsNullOrEmpty(this.OlapDataManager.CurrentReport.CurrentCubeName) && !string.IsNullOrEmpty(this.OlapDataManager.CurrentReport.Name))
            {
                if (this.btnEnablePaging != null)
                {
                    this.btnEnablePaging.IsChecked = this.OlapDataManager.CurrentReport.EnablePaging;
                }
                if (this.OlapPager != null)
                {
                    this.OlapPager.Visibility = this.OlapDataManager.CurrentReport.EnablePaging ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }
            }
            else if (this.OlapPager != null)
            {
                if (this.OlapPager != null)
                {
                    this.OlapPager.Visibility = this.EnablePaging ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }
            }
        }
        private void UpdateVirtualKpiElement()
        {
            if (this.OlapDataManager.CurrentReport != null && this.OlapDataManager.CurrentReport.VirtualKpiElements.Count > 0)
            {
                this.IsVirtualKpiEnabled = true;
            }
            else if(this.OlapDataManager.CurrentReport!=null && this.OlapDataManager.CurrentReport.VirtualKpiElements.Count==0 && !this.IsVirtualKpiEnabled)
            {
                this.IsVirtualKpiEnabled = false;
            }
        }

        private void UpdateCalculatedMembers()
        {
            if (this.OlapDataManager.CurrentReport != null && this.OlapDataManager.CurrentReport.CalculatedMembers.Count > 0)
            {
                this.IsCalculatedMembersEnabled = true;
            }
            else if (this.OlapDataManager.CurrentReport != null && this.OlapDataManager.CurrentReport.CalculatedMembers.Count == 0 && !this.IsCalculatedMembersEnabled)
            {
                this.IsCalculatedMembersEnabled = false;
            }
        }

        /// <summary>
        /// Sets the display mode.
        /// </summary>
        /// <param name="client">The client.</param>
        private void SetDisplayMode(OlapClient client)
        {
            if (client.DisplayMode == DisplayModes.Both)
            {
                client.OlapDataManager.IsProcessing = true;
                client.OlapGrid.OlapDataManager = client.OlapDataManager;
                client.OlapChart.OlapDataManager = client.OlapDataManager;
                //client.OlapTabControl
                (client.OlapTabControl.Items[0] as TabItemAdv).Visibility = System.Windows.Visibility.Visible;
                (client.OlapTabControl.Items[1] as TabItemAdv).Visibility = System.Windows.Visibility.Visible;
                (client.OlapTabControl.Items[0] as TabItemAdv).IsSelected = true;

                //client.OlapDataManager.NotifyCellSetChanged();
                if (client.OlapGrid.InternalGrid == null)
                {
                    client.OlapGrid.ApplyTemplate();
                }
                //client.OlapGrid.Refresh();
            }
            else if (client.DisplayMode == DisplayModes.ChartOnly)
            {
                client.OlapDataManager.IsProcessing = true;
                client.OlapGrid.OlapDataManager = null;
                client.OlapChart.OlapDataManager = client.OlapDataManager;
                (client.OlapTabControl.Items[1] as TabItemAdv).Visibility = System.Windows.Visibility.Collapsed;
                (client.OlapTabControl.Items[0] as TabItemAdv).Visibility = System.Windows.Visibility.Visible;
                (client.OlapTabControl.Items[0] as TabItemAdv).IsSelected = true;
                //client.OlapDataManager.NotifyCellSetChanged();
            }
            else if (client.DisplayMode == DisplayModes.GridOnly)
            {
                client.OlapDataManager.IsProcessing = true;
                client.OlapGrid.OlapDataManager = client.OlapDataManager;
                client.OlapChart.OlapDataManager = null;
                (client.OlapTabControl.Items[0] as TabItemAdv).Visibility = System.Windows.Visibility.Collapsed;
                (client.OlapTabControl.Items[1] as TabItemAdv).Visibility = System.Windows.Visibility.Visible;
                (client.OlapTabControl.Items[1] as TabItemAdv).IsSelected = true;
                //client.OlapDataManager.NotifyCellSetChanged();
            }

            UpdateCurrentTab();
        }

        #endregion

        #region Events

        #region Layout event handlers
        /// <summary>
        /// Handles the Loaded event of the OlapClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OlapClient_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                this.UpdateLayout();
                this.UpdateReportButtons();
                //this.Isprocessing = true;
                if (ShowToolBarsInOlapClient)
                {
                    OlapClientToolBar.Visibility = System.Windows.Visibility.Visible;
                    OlapChartToolBar.Visibility = System.Windows.Visibility.Visible;
                    OlapGridToolBar.Visibility = System.Windows.Visibility.Visible;
                    
                }
                else
                {
                    OlapClientToolBar.Visibility = System.Windows.Visibility.Collapsed;
                    OlapChartToolBar.Visibility = System.Windows.Visibility.Collapsed;
                    OlapGridToolBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                
                if (this.OlapTabControl != null)
                {
                    var tabChild = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.OlapTabControl, 0), 0), 0);
                    if (tabChild != null)
                    {
                        (tabChild as System.Windows.Controls.Grid).Background = (Brush)new SolidColorBrush(Colors.Transparent);
                    }
                }
                if (OlapChartTab != null && this.DisplayMode != DisplayModes.GridOnly)
                {
                    this.OlapChartTab.CaptureMouse();
                    this.OlapChartTab.MouseLeftButtonDown += TabItem_MouseLeftButtonDown;
                    this.OlapChartTab.GotFocus += OlapTab_GotFocus;
                }
                if (OlapGridTab != null && this.DisplayMode != DisplayModes.ChartOnly)
                {
                    this.OlapGridTab.CaptureMouse();
                    this.OlapGridTab.MouseLeftButtonDown += TabItem_MouseLeftButtonDown;
                    this.OlapGridTab.GotFocus += OlapTab_GotFocus;
                }
            }
        }

        private void UpdateCurrentTab()
        {
            if (this.DisplayMode == DisplayModes.GridOnly)
            {
                _currentTab = this.OlapGridTab != null ? this.OlapGridTab.Header.ToString() : "OlapGrid"; //TODO: For localization consideration, Change this hard code "OlapGrid" when change the Grid Tab item Header
            }
            else
            {
                _currentTab = this.OlapChartTab != null ? this.OlapChartTab.Header.ToString() : "OlapChart"; //TODO: For localization consideration Change this hard code "OlapChart" when change the Chart Tab item Header
            }
        }

        private string _currentTab;
        void TabItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabItemAdv tabItem = sender as TabItemAdv;
            if (_currentTab != null && !(_currentTab.Equals(tabItem.Header.ToString())))
            {
                this.Isprocessing = true;
                _currentTab = tabItem.Header.ToString();
            }            
        }

        void OlapTab_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.Isprocessing)
            {
                this.Isprocessing = false;
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes 
        /// (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //if (!DesignerProperties.GetIsInDesignMode(this))
            //{
            //// Layout updation
            this.MainBorder = this.GetTemplateChild("PART_Border") as Border;
            this.OlapGrid = this.GetTemplateChild("PART_OlapGrid") as Syncfusion.Silverlight.Grid.Olap.OlapGrid;
            this.OlapChart = this.GetTemplateChild("PART_OlapChart") as OlapChart;
            this.CubeSelectorHolder = this.GetTemplateChild("PART_CubeSelectorPlaceHolder") as System.Windows.Controls.Grid;
            this.CubeBrowserHolder = this.GetTemplateChild("PART_CubeBrowserPlaceHolder") as System.Windows.Controls.Grid;
            this.CubeSelectorHeight = this.GetTemplateChild("PART_CubeSelectorHeight") as System.Windows.Controls.RowDefinition;
            this.CubeSelector = this.GetTemplateChild("PART_CubeSelector") as ComboBox;
            this.CubeDimensionBrowser = this.GetTemplateChild("PART_CubeDimensionBrowser") as CubeDimensionBrowser;
            this.CubeSelector.SelectionChanged += CubeSelector_SelectionChanged;
            this.AxisElementBuilderColumn = this.GetTemplateChild("PART_ColumnAxis") as AxisElementBuilder;
            this.AxisElementBuilderRow = this.GetTemplateChild("PART_RowAxis") as AxisElementBuilder;
            this.AxisElementBuilderSlicer = this.GetTemplateChild("PART_SlicerAxis") as AxisElementBuilder;
            this.OlapTabControl = this.GetTemplateChild("PART_OlapTabControl") as TabControlAdv;
            this.OlapChartTab = this.GetTemplateChild("PART_OlapChartTab") as TabItemAdv;
            this.OlapGridTab = this.GetTemplateChild("PART_OlapGridTab") as TabItemAdv;
            this._progressLayer = this.GetTemplateChild("PART_ProgressLayer") as Canvas;
            this._olapClientProgressBar = this.GetTemplateChild("PART_ProgressBar") as ProgressBar;
            this._olapClientProgressPopup = this.GetTemplateChild("PART_ProgressPopup") as Popup;
            this._categoricalBorder = this.GetTemplateChild("CategoricalHeader") as Border;
            this._seriesBorder = this.GetTemplateChild("SeriesHeader") as Border;
            this._slicerBorder = this.GetTemplateChild("SlicerHeader") as Border;
            this._tabborder = this.GetTemplateChild("TabBorder") as Border;
            this._toolbarborder = this.GetTemplateChild("ToolBarBorder") as Border;
            this._cubeborder = this.GetTemplateChild("CubeBorder") as Border;
            this._buttonborder = this.GetTemplateChild("ButtonBorder") as Border;
            this.OlapPager = this.GetTemplateChild("PART_OlapPagerControl") as OlapPager;

            this.UpdateAxesOrder(this);
            if (this.CubeSelectorHeight != null)
                this.CubeSelectorHeight.Height = this.ShowCubeSelector ? new GridLength(50) : new GridLength(0);
            if (this.CubeSelectorHolder != null)
                this.CubeSelectorHolder.Visibility = this.ShowCubeSelector ? Visibility.Visible : Visibility.Collapsed;
            if (this.CubeBrowserHolder != null)
                this.CubeBrowserHolder.Visibility = this.ShowCubeBrowser ? Visibility.Visible : Visibility.Collapsed;
            if (this.OlapTabControl != null)
                this.OlapTabControl.CloseButtonType = CloseButtonType.Hide;
            if (this.AxisElementBuilderColumn != null)
            {
                this.AxisElementBuilderColumn.SplitButtonDisplayMode = this.AxisItemDisplayMode;
                this.AxisElementBuilderColumn.AllowMultiMemberSelection = this.AllowMultiMemberSelection;
                this.AxisElementBuilderColumn.SelectionChanged += new SelectionChangedEventHandler(AxisElementBuilder_SelectionChanged);
                this.AxisElementBuilderColumn.MouseMove += new MouseEventHandler(AxisElementBuilder_MouseMove);
                this.AxisElementBuilderColumn.MouseLeave += new MouseEventHandler(AxisElementBuilder_MouseLeave);
                this.AxisElementBuilderColumn.MouseLeftButtonUp += new MouseButtonEventHandler(AxisElementBuilder_MouseLeftButtonUp);
                this.AxisElementBuilderColumn.IsMeasureEditorOpen = this.IsMeasureEditorOpen;
                this.AxisElementBuilderColumn.IsMemberEditorOpen = this.IsMemberEditorOpen;
            }
            if (this.AxisElementBuilderRow != null)
            {
                this.AxisElementBuilderRow.SplitButtonDisplayMode = this.AxisItemDisplayMode;
                this.AxisElementBuilderRow.AllowMultiMemberSelection = this.AllowMultiMemberSelection;
                this.AxisElementBuilderRow.SelectionChanged += new SelectionChangedEventHandler(AxisElementBuilder_SelectionChanged);
                this.AxisElementBuilderRow.MouseMove += new MouseEventHandler(AxisElementBuilder_MouseMove);
                this.AxisElementBuilderRow.MouseLeave += new MouseEventHandler(AxisElementBuilder_MouseLeave);
                this.AxisElementBuilderRow.IsMeasureEditorOpen = this.IsMeasureEditorOpen;
                this.AxisElementBuilderRow.IsMemberEditorOpen = this.IsMemberEditorOpen;
            }
            if (this.AxisElementBuilderSlicer != null)
            {
                this.AxisElementBuilderSlicer.SplitButtonDisplayMode = this.AxisItemDisplayMode;
                this.AxisElementBuilderSlicer.AllowMultiMemberSelection = this.AllowMultiMemberSelection;
                this.AxisElementBuilderSlicer.SelectionChanged += new SelectionChangedEventHandler(AxisElementBuilder_SelectionChanged);
                this.AxisElementBuilderSlicer.MouseMove += new MouseEventHandler(AxisElementBuilder_MouseMove);
                this.AxisElementBuilderSlicer.MouseLeave += new MouseEventHandler(AxisElementBuilder_MouseLeave);
                this.AxisElementBuilderSlicer.IsMeasureEditorOpen = this.IsMeasureEditorOpen;
                this.AxisElementBuilderSlicer.IsMemberEditorOpen = this.IsMemberEditorOpen;
            }
            this.CubeDimensionBrowser.MeasureSortOrderInCubeBrowser = this.MeasureSortOrderInCubeBrowser;
            this.CubeDimensionBrowser.MouseMove += new MouseEventHandler(CubeDimensionBrowser_MouseMove);
            this.CubeDimensionBrowser.NodeClicked += new NodeClickedHandler(CubeDimensionBrowser_NodeClicked);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(OlapClient_MouseLeftButtonUp);
            if (this.OlapGrid != null)
                this.OlapGrid.Layout = this.GridLayout;
            UpdateClientToolBarLayout();
            UpdateGridToolBarLayout();
            UpdateChartToolBarLayout();
            ApplyBrushes(this.VisualStyle.ToString(), false);

            if (this.OlapPager != null)
            {
                this.OlapPager.VisualStyle = (Syncfusion.Windows.Shared.VisualStyle)this.VisualStyle;
                this._tabborder.Margin = new Thickness(0, 0, -10, -40);
            }
            else
                this._tabborder.Margin = new Thickness(0, 0, -10, -10);
            if(btnNewServer!=null)
                btnNewServer.Visibility = (this.EnabledConnectionOption) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void UpdateAxesOrder(OlapClient client)
        {
            if (client._categoricalBorder != null && client._seriesBorder != null && client._slicerBorder != null)
            {
                switch (client.AxesOrder)
                {                       
                    case AxesOrder.CFR:
                        client._categoricalBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 0);
                        client._slicerBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 1);
                        client._seriesBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 2);
                        break;
                    case AxesOrder.FRC:
                        client._slicerBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 0);
                        client._seriesBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 1);
                        client._categoricalBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 2);
                        break;
                    case AxesOrder.FCR:
                        client._slicerBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 0);
                        client._categoricalBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 1);
                        client._seriesBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 2);
                        break;
                    case AxesOrder.RCF:
                        client._seriesBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 0);
                        client._categoricalBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 1);
                        client._slicerBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 2);
                        break;
                    case AxesOrder.RFC:
                        client._seriesBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 0);
                        client._slicerBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 1);
                        client._categoricalBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 2);
                        break;
                    default:
                        client._categoricalBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 0);
                        client._seriesBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 1);
                        client._slicerBorder.SetValue(System.Windows.Controls.Grid.ColumnProperty, 2);
                        break;
                }
            }
        }

        internal void ApplyVisualStyleforOlapPager(string VisualStyle)
        {
            this.OlapPager.VisualStyle = (Syncfusion.Windows.Shared.VisualStyle)Enum.Parse(typeof(Syncfusion.Windows.Shared.VisualStyle), VisualStyle, true);
            switch (this.OlapPager.VisualStyle)
            {
                case Syncfusion.Windows.Shared.VisualStyle.Default:
                    this.OlapPager.CategoricalPager.Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245));
                    this.OlapPager.SeriesPager.Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245));
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Blend:
                    this.OlapPager.CategoricalPager.Background = new SolidColorBrush(Color.FromArgb(255, 59, 59, 59));
                    this.OlapPager.SeriesPager.Background = new SolidColorBrush(Color.FromArgb(255, 59, 59, 59));
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2007Blue:
                    this.OlapPager.CategoricalPager.Background = new SolidColorBrush(Color.FromArgb(255, 227, 239, 255));
                    this.OlapPager.SeriesPager.Background = new SolidColorBrush(Color.FromArgb(255, 227, 239, 255));
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2007Black:
                    this.OlapPager.CategoricalPager.Background = new SolidColorBrush(Color.FromArgb(255, 164, 171, 180));
                    this.OlapPager.SeriesPager.Background = new SolidColorBrush(Color.FromArgb(255, 164, 171, 180));
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2007Silver:
                    this.OlapPager.CategoricalPager.Background = new SolidColorBrush(Color.FromArgb(255, 238, 239, 243));
                    this.OlapPager.SeriesPager.Background = new SolidColorBrush(Color.FromArgb(255, 238, 239, 243));
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2010Blue:
                    this.OlapPager.CategoricalPager.Background = new SolidColorBrush(Color.FromArgb(255, 207, 221, 238));
                    this.OlapPager.SeriesPager.Background = new SolidColorBrush(Color.FromArgb(255, 207, 221, 238));
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2010Black:
                    this.OlapPager.CategoricalPager.Background = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
                    this.OlapPager.SeriesPager.Background = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2010Silver:
                    this.OlapPager.CategoricalPager.Background = new SolidColorBrush(Color.FromArgb(255, 228, 232, 237));
                    this.OlapPager.SeriesPager.Background = new SolidColorBrush(Color.FromArgb(255, 228, 232, 237));
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Metro:
                    this.OlapPager.CategoricalPager.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    this.OlapPager.SeriesPager.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Transparent:
                    this.OlapPager.CategoricalPager.Background = new SolidColorBrush(Colors.Transparent);
                    this.OlapPager.SeriesPager.Background = new SolidColorBrush(Colors.Transparent);
                    break;
                default:
                    break;
            }

        }

        internal void ApplyBrushes(string Visualstyle, bool isChangedEvent)
        {
            OlapClient client = this;
            System.Windows.ResourceDictionary r = new System.Windows.ResourceDictionary();
            r.Source = new Uri("/Syncfusion.OlapClient.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            System.Windows.ResourceDictionary source = new System.Windows.ResourceDictionary();
            source.Source = new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/ToolBar/OlapToolBar.xaml", UriKind.RelativeOrAbsolute);
            System.Windows.ResourceDictionary dictionary = new System.Windows.ResourceDictionary();
            dictionary.Source = new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/CubeDimensionBrowser/CubeDimensionBrowser.xaml", UriKind.RelativeOrAbsolute);
            System.Windows.ResourceDictionary r1 = new System.Windows.ResourceDictionary();
            r1.Source = new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/SplitButton/SplitButton.xaml", UriKind.RelativeOrAbsolute);
            Syncfusion.Windows.Controls.Theming.VisualStyle visualStyle = (Syncfusion.Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Syncfusion.Windows.Controls.Theming.VisualStyle), Visualstyle, true);
            System.Windows.ResourceDictionary r2 = null;
            try
            {
                r2 = new System.Windows.ResourceDictionary() { Source = new Uri(string.Format("/Syncfusion.Theming.{0};component/TabControlAdv.xaml", Visualstyle), UriKind.RelativeOrAbsolute) };
            }
            catch { }
            string clientBackgroundBrush, borderBrush, headerHoverBackground, headerHoverBorderBrush, headerBackgroundBrush,
                tabControlAdvStyle, cubeDimensionalBrowserStyle, toolBarBackgroundBrush, axisElementBuilderBackgroundBrush, buttonStyle;
            switch (Visualstyle)
            {
                case "Blend":
                    clientBackgroundBrush = string.Format("{0}ClientBackgroundBrush", Visualstyle);
                    borderBrush = string.Format("{0}BorderBrush", Visualstyle);
                    headerHoverBackground = string.Format("{0}HeaderHoverBackground", Visualstyle);
                    headerHoverBorderBrush = string.Format("{0}HeaderHoverBorderBrush", Visualstyle);
                    headerBackgroundBrush = string.Format("{0}HeaderBackgroundBrush", Visualstyle);
                    tabControlAdvStyle = string.Format("{0}TabControlAdvStyle", Visualstyle);
                    cubeDimensionalBrowserStyle = string.Format("{0}CubeDimensionalBrowserStyle", Visualstyle);
                    toolBarBackgroundBrush = string.Format("{0}ToolBarBackgroundBrush", Visualstyle);
                    axisElementBuilderBackgroundBrush = string.Format("{0}AxisElementBuilderBackgroundBrush", Visualstyle);
                    buttonStyle = string.Format("{0}ButtonStyle", Visualstyle);
                    
                    client.Foreground = new SolidColorBrush(Colors.White);
                    client.BorderThickness = new Thickness(0.5);
                    if (client.OlapChart != null) client.OlapChart.ChartVisualStyle = OlapChartVisualStyle.Blend;
                    if (client.OlapClientToolBar != null) client.OlapClientToolBar.BorderThickness = new Thickness(0.5);
                    if (client.OlapChartToolBar != null) client.OlapChartToolBar.BorderThickness = new Thickness(0.5);
                    if (client.OlapGridToolBar != null) client.OlapGridToolBar.BorderThickness = new Thickness(0.5);
                    break;
                case "Metro":
                    clientBackgroundBrush = string.Format("{0}ClientBackgroundBrush", Visualstyle);
                    borderBrush = string.Format("{0}BorderBrush", Visualstyle);
                    headerHoverBackground = string.Format("{0}HeaderHoverBackground", Visualstyle);
                    headerHoverBorderBrush = string.Format("{0}HeaderHoverBorderBrush", Visualstyle);
                    headerBackgroundBrush = string.Format("{0}HeaderBackgroundBrush", Visualstyle);
                    tabControlAdvStyle = string.Format("{0}TabControlAdvStyle", Visualstyle);                   
                    cubeDimensionalBrowserStyle = string.Format("{0}CubeDimensionalBrowserStyle", Visualstyle);
                    toolBarBackgroundBrush = string.Format("{0}ToolBarBackgroundBrush", Visualstyle);
                    axisElementBuilderBackgroundBrush = string.Format("{0}AxisElementBuilderBackgroundBrush", Visualstyle);
                    buttonStyle = string.Format("{0}ButtonStyle", Visualstyle);
                    //client.Foreground = new SolidColorBrush(Colors.White);
                    client.BorderThickness = new Thickness(1);
                    if (client.OlapChart != null) client.OlapChart.ChartVisualStyle = OlapChartVisualStyle.Metro;
                    if (client.OlapClientToolBar != null) client.OlapClientToolBar.BorderThickness = new Thickness(1);
                    if (client.OlapChartToolBar != null) client.OlapChartToolBar.BorderThickness = new Thickness(0.5);
                    if (client.OlapGridToolBar != null) client.OlapGridToolBar.BorderThickness = new Thickness(0.5);
                    break;
                case "Office2007Blue":
                case "Office2007Black":
                case "Office2007Silver":
                    clientBackgroundBrush = string.Format("{0}ClientBackgroundBrush", Visualstyle);
                    borderBrush = string.Format("{0}BorderBrush", Visualstyle);
                    buttonStyle = string.Format("{0}ButtonStyle", "Office2007");
                    headerHoverBackground = string.Format("{0}HeaderHoverBackground", "Office2007");
                    headerHoverBorderBrush = string.Format("{0}HeaderHoverBorderBrush", "Office2007");
                    headerBackgroundBrush = string.Format("{0}HeaderBackgroundBrush", Visualstyle);
                    tabControlAdvStyle = string.Format("{0}TabControlAdvStyle", Visualstyle);
                    cubeDimensionalBrowserStyle = string.Format("{0}CubeDimensionalBrowserStyle", Visualstyle);
                    toolBarBackgroundBrush = string.Format("{0}ToolBarBackgroundBrush", Visualstyle);
                    axisElementBuilderBackgroundBrush = string.Format("{0}AxisElementBuilderBackgroundBrush", Visualstyle);
                    client.Foreground = new SolidColorBrush(Colors.Black);
                    client.BorderThickness = new Thickness(1);
                    if (client.OlapChart != null)
                        client.OlapChart.ChartVisualStyle = visualStyle == Syncfusion.Windows.Controls.Theming.VisualStyle.Office2007Black ? OlapChartVisualStyle.Office2007Black : visualStyle == Syncfusion.Windows.Controls.Theming.VisualStyle.Office2007Blue ? OlapChartVisualStyle.Office2007Blue : OlapChartVisualStyle.Office2007Silver;
                    if (client.OlapClientToolBar != null) client.OlapClientToolBar.BorderThickness = new Thickness(1);
                    if (client.OlapGridToolBar != null) client.OlapGridToolBar.BorderThickness = new Thickness(1);
                    if (client.OlapChartToolBar != null) client.OlapChartToolBar.BorderThickness = new Thickness(1);
                    break;
                case "Office2010Blue":
                case "Office2010Black":
                case "Office2010Silver":
                     clientBackgroundBrush = string.Format("{0}ClientBackgroundBrush", Visualstyle);
                    borderBrush = string.Format("{0}BorderBrush", Visualstyle);
                    buttonStyle = string.Format("{0}ButtonStyle", "Office2010");
                    headerHoverBackground = string.Format("{0}HeaderHoverBackground", "Office2010");
                    headerHoverBorderBrush = string.Format("{0}HeaderHoverBorderBrush", "Office2010");
                    headerBackgroundBrush = string.Format("{0}HeaderBackgroundBrush", Visualstyle);
                    tabControlAdvStyle = string.Format("{0}TabControlAdvStyle", Visualstyle);
                    cubeDimensionalBrowserStyle = string.Format("{0}CubeDimensionalBrowserStyle", Visualstyle);
                    toolBarBackgroundBrush = string.Format("{0}ToolBarBackgroundBrush", Visualstyle);
                    axisElementBuilderBackgroundBrush = string.Format("{0}AxisElementBuilderBackgroundBrush", Visualstyle);
                    client.Foreground = new SolidColorBrush(Colors.Black);
                    client.BorderThickness = new Thickness(1);
                    if (client.OlapChart != null)
                        client.OlapChart.ChartVisualStyle = visualStyle == Syncfusion.Windows.Controls.Theming.VisualStyle.Office2010Black ? OlapChartVisualStyle.Office2010Black : visualStyle == Syncfusion.Windows.Controls.Theming.VisualStyle.Office2010Blue ? OlapChartVisualStyle.Office2010Blue : OlapChartVisualStyle.Office2010Silver;
                    if (client.OlapClientToolBar != null) client.OlapClientToolBar.BorderThickness = new Thickness(1);
                    if (client.OlapGridToolBar != null) client.OlapGridToolBar.BorderThickness = new Thickness(1);
                    if (client.OlapChartToolBar != null) client.OlapChartToolBar.BorderThickness = new Thickness(1);
                    break;
                case "Transparent":
                    clientBackgroundBrush = string.Format("{0}ClientBackgroundBrush", Visualstyle);
                    borderBrush = string.Format("{0}BorderBrush", Visualstyle);
                    buttonStyle = string.Format("{0}ButtonStyle", "Transparent");
                    headerHoverBackground = string.Format("{0}HeaderHoverBackground", "Transparent");
                    headerHoverBorderBrush = string.Format("{0}HeaderHoverBorderBrush", "Transparent");
                    headerBackgroundBrush = string.Format("{0}HeaderBackgroundBrush", Visualstyle);
                    tabControlAdvStyle = string.Format("{0}TabControlAdvStyle", Visualstyle);
                    cubeDimensionalBrowserStyle = string.Format("{0}CubeDimensionalBrowserStyle", Visualstyle);
                    toolBarBackgroundBrush = string.Format("{0}ToolBarBackgroundBrush", Visualstyle);
                    axisElementBuilderBackgroundBrush = string.Format("{0}AxisElementBuilderBackgroundBrush", Visualstyle);
                    client.Foreground = new SolidColorBrush(Colors.Black);
                    client.BorderThickness = new Thickness(1);
                    if (client.OlapChart != null)
                        client.OlapChart.ChartVisualStyle = OlapChartVisualStyle.Transparent;
                    if (client.OlapClientToolBar != null) client.OlapClientToolBar.BorderThickness = new Thickness(1);
                    if (client.OlapGridToolBar != null) client.OlapGridToolBar.BorderThickness = new Thickness(1);
                    if (client.OlapChartToolBar != null) client.OlapChartToolBar.BorderThickness = new Thickness(1);
                    break;
                default:
                    clientBackgroundBrush = "DefaultClientBackgroundBrush";
                    borderBrush = "DefaultBorderBrush";
                    buttonStyle = "DefaultButtonStyle";
                    headerHoverBackground = "DefaultHeaderHoverBackground";
                    headerHoverBorderBrush = "DefaultHeaderHoverBorderBrush";
                    headerBackgroundBrush = "DefaultHeaderBackgroundBrush";
                    tabControlAdvStyle = "DefaultTabControlAdvStyle";
                    cubeDimensionalBrowserStyle = "DefaultCubeDimensionalBrowserStyle";
                    toolBarBackgroundBrush = "DefaultToolBarBackgroundBrush";
                    axisElementBuilderBackgroundBrush = "DefaultAxisElementBuilderBackgroundBrush";
                    client.BorderThickness = new Thickness(0.5);
                    client.Foreground = new SolidColorBrush(Colors.Black);
                    if (client.OlapChart != null) client.OlapChart.ChartVisualStyle = OlapChartVisualStyle.Default;
                    if (client.OlapClientToolBar != null) client.OlapClientToolBar.BorderThickness = new Thickness(0.5);
                    if (client.OlapChartToolBar != null) client.OlapChartToolBar.BorderThickness = new Thickness(0.5);
                    if (client.OlapGridToolBar != null) client.OlapGridToolBar.BorderThickness = new Thickness(0.5);
                    break;
            }
            client.Background = r[clientBackgroundBrush] as Brush;
            client.BorderBrush = r[borderBrush] as Brush;
            client.HeaderHoverBackground = r[headerHoverBackground] as Brush;
            client.HeaderHoverBorderBrush = r[headerHoverBorderBrush] as Brush;
            //client.ButtonStyle = source["BlendButtonStyle"] as Style;
            client.ButtonStyle = source[buttonStyle] as Style;
            if (client._categoricalBorder != null)
            {
                client._categoricalBorder.Background = r[headerBackgroundBrush] as Brush;
                client._categoricalBorder.BorderBrush = r[headerBackgroundBrush] as Brush;
            }
            if (client._seriesBorder != null)
            {
                client._seriesBorder.Background = r[headerBackgroundBrush] as Brush;
                client._seriesBorder.BorderBrush = r[headerBackgroundBrush] as Brush;
            }
            if (client._slicerBorder != null)
            {
                client._slicerBorder.Background = r[headerBackgroundBrush] as Brush;
                client._slicerBorder.BorderBrush = r[headerBackgroundBrush] as Brush;
            }
            try
            {
                switch (client.VisualStyle)
                {
                    case OlapClientVisualStyle.Default:
                        client.OlapGrid.VisualStyle = OlapGridVisualStyle.Default;
                        break;
                    case OlapClientVisualStyle.Blend:
                        client.OlapGrid.VisualStyle = OlapGridVisualStyle.Blend;
                        break;
                    case OlapClientVisualStyle.Office2007Blue:
                        client.OlapGrid.VisualStyle = OlapGridVisualStyle.Office2007Blue;
                        break;
                    case OlapClientVisualStyle.Office2007Black:
                        client.OlapGrid.VisualStyle = OlapGridVisualStyle.Office2007Black;
                        break;
                    case OlapClientVisualStyle.Office2007Silver:
                        client.OlapGrid.VisualStyle = OlapGridVisualStyle.Office2007Silver;
                        break;
                    case OlapClientVisualStyle.Office2010Blue:
                        client.OlapGrid.VisualStyle = OlapGridVisualStyle.Office2010Blue;
                        break;
                    case OlapClientVisualStyle.Office2010Black:
                        client.OlapGrid.VisualStyle = OlapGridVisualStyle.Office2010Black;
                        break;
                    case OlapClientVisualStyle.Office2010Silver:
                        client.OlapGrid.VisualStyle = OlapGridVisualStyle.Office2010Silver;
                        break;
                    case OlapClientVisualStyle.Metro:
                        client.OlapGrid.VisualStyle = OlapGridVisualStyle.Metro;
                        break;
                    case OlapClientVisualStyle.Transparent:
                        client.OlapGrid.VisualStyle = OlapGridVisualStyle.Transparent;
                        break;
                    default:
                        break;
                }
                SkinManager.SetVisualStyle(client, visualStyle);
                ApplyVisualStyleforOlapPager(this.VisualStyle.ToString());
            }
            catch { }
            if (client.OlapTabControl != null)
            {
                if (client.TabControlAdvStyle != null && !isChangedEvent)
                    client.OlapTabControl.Style = client.TabControlAdvStyle;
                else if (r2 != null)
                    client.OlapTabControl.Style = r2[tabControlAdvStyle] as Style;
            }
            if (client.CubeDimensionBrowser != null)
            {
                if (client.TreeViewStyle != null && !isChangedEvent)
                {
                    client.CubeDimensionBrowser.Style = client.TreeViewStyle;
                }
                else
                {
                    client.CubeDimensionBrowser.Style = dictionary[cubeDimensionalBrowserStyle] as Style;
                }
            }

            if (client.OlapClientToolBar != null)
            {
                client.OlapClientToolBar.BorderBrush = r[borderBrush] as Brush;
                client.OlapClientToolBar.Background = r[toolBarBackgroundBrush] as Brush;
            }
            if (client.OlapChartToolBar != null)
            {
                client.OlapChartToolBar.BorderBrush = r[borderBrush] as Brush;
                client.OlapChartToolBar.Background = r[toolBarBackgroundBrush] as Brush;
            }
            if (client.OlapGridToolBar != null)
            {
                client.OlapGridToolBar.Background = r[toolBarBackgroundBrush] as Brush;
                client.OlapGridToolBar.BorderBrush = r[borderBrush] as Brush;
            }

            if (client.AxisElementBuilderColumn != null)
            {
                if (client.AxisElementBuilderStyle != null && !isChangedEvent)
                    client.AxisElementBuilderColumn.Style = client.AxisElementBuilderStyle;
                else
                {
                    client.AxisElementBuilderColumn.Background = r[axisElementBuilderBackgroundBrush] as Brush;
                    client.AxisElementBuilderColumn.BorderBrush = r[borderBrush] as Brush;
                }
                client.AxisElementBuilderColumn.SplitButtonVisualStyle = visualStyle;

            }
            if (client.AxisElementBuilderRow != null)
            {
                if (client.AxisElementBuilderStyle != null && !isChangedEvent)
                    client.AxisElementBuilderRow.Style = client.AxisElementBuilderStyle;
                else
                {
                    client.AxisElementBuilderRow.Background = r[axisElementBuilderBackgroundBrush] as Brush;
                    client.AxisElementBuilderRow.BorderBrush = r[borderBrush] as Brush;
                }
                client.AxisElementBuilderRow.SplitButtonVisualStyle = visualStyle;
            }
            if (client.AxisElementBuilderSlicer != null)
            {
                if (client.AxisElementBuilderStyle != null && !isChangedEvent)
                    client.AxisElementBuilderSlicer.Style = client.AxisElementBuilderStyle;
                else
                {
                    client.AxisElementBuilderSlicer.Background = r[axisElementBuilderBackgroundBrush] as Brush;
                    client.AxisElementBuilderSlicer.BorderBrush = r[borderBrush] as Brush;
                }
                client.AxisElementBuilderSlicer.SplitButtonVisualStyle = visualStyle;
            }
        }

        void OlapClient_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            flag = 0;
            SetVisibility(Visibility.Collapsed);
        }

        void AxisElementBuilder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (flag == 2)
            {
                flag = 0;
                this._categoricalBorder.Background = this.Background;
                this._categoricalBorder.BorderBrush = this.BorderBrush;
                this._seriesBorder.Background = this.Background;
                this._seriesBorder.BorderBrush = this.BorderBrush;
                this._slicerBorder.Background = this.Background;
                this._slicerBorder.BorderBrush = this.BorderBrush;
                SetVisibility(Visibility.Collapsed);
            }
        }

        void AxisElementBuilder_MouseLeave(object sender, MouseEventArgs e)
        {
            this._categoricalBorder.Background = this.Background;
            this._categoricalBorder.BorderBrush = this.BorderBrush;
            this._seriesBorder.Background = this.Background;
            this._seriesBorder.BorderBrush = this.BorderBrush;
            this._slicerBorder.Background = this.Background;
            this._slicerBorder.BorderBrush = this.BorderBrush;
        }

        void AxisElementBuilder_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag == -2)
            {
                flag = 2;
                SetVisibility(Visibility.Visible);
            }
            if (flag == 2)
            {
                AxisPosition axis = (sender as AxisElementBuilder).Axis;
                switch (axis)
                {
                    case AxisPosition.Categorical:
                        this._categoricalBorder.Background = this.HeaderHoverBackground;
                        this._categoricalBorder.BorderBrush = this.HeaderHoverBorderBrush;
                        break; ;
                    case AxisPosition.Series:
                        this._seriesBorder.Background = this.HeaderHoverBackground;
                        this._seriesBorder.BorderBrush = this.HeaderHoverBorderBrush;
                        break;
                    case AxisPosition.Slicer:
                        this._slicerBorder.Background = this.HeaderHoverBackground;
                        this._slicerBorder.BorderBrush = this.HeaderHoverBorderBrush;
                        break;
                }
            }
        }

        void AxisElementBuilder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var axisBuilder = sender as AxisElementBuilder;
            if (axisBuilder.SelectedItem != null)
                flag = -2;
            axisBuilder.SelectedIndex = -1;
        }

        void CubeDimensionBrowser_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag == 1)
            {
                flag = 2;
                SetVisibility(Visibility.Visible);
            }
        }

        void SetVisibility(Visibility value)
        {
            _toolbarborder.Visibility = value;
            _tabborder.Visibility = value;
            _cubeborder.Visibility = value;
            _buttonborder.Visibility = value;
        }

        void CubeDimensionBrowser_NodeClicked(object sender, NodeClickedEventArgs e)
        {
            if ((sender as CDTreeViewItem) != null)
            {
                flag = 1;
            }
        }

        /// <summary>
        /// Update the OlapClient tool bar layout.
        /// </summary>
        /// 
        private void UpdateClientToolBarLayout()
        {
            //// Layout updation
            this.OlapClientToolBar = this.GetTemplateChild("PART_clientToolBar") as OlapToolBar;
            this.btnTogglePivot = this.GetTemplateChild("PART_TogglePivot") as OlapToolBarButton;
            this.btnShowExpander = this.GetTemplateChild("PART_ShowExpander") as OlapToolBarButton;
            this.btnAutoExecute = this.GetTemplateChild("PART_AutoExecute") as OlapToolBarButton;
            if (this.btnAutoExecute != null)
            {
                if (this.AutoExecute)
                    this.btnAutoExecute.Visibility = Visibility.Collapsed;
                else
                    this.btnAutoExecute.Visibility = Visibility.Visible;
                this.AxisElementBuilderColumn.AutoExecute = this.AxisElementBuilderRow.AutoExecute = this.AxisElementBuilderSlicer.AutoExecute = this.AutoExecute;
            }
            this.btnFullScreen = this.GetTemplateChild("PART_FullScreenButton") as OlapToolBarButton;
            if (this.btnFullScreen != null)
            {
                this.btnFullScreen.Visibility = this.ShowFullScreenButton ? Visibility.Visible : Visibility.Collapsed;
            }

            this.btnAddReport = this.GetTemplateChild("PART_Add") as OlapToolBarButton;
            this.btnRemoveReport = this.GetTemplateChild("PART_Rmv") as OlapToolBarButton;
            this.btnNewReport = this.GetTemplateChild("PART_New") as OlapToolBarButton;
            this.btnReName = this.GetTemplateChild("PART_Rnm") as OlapToolBarButton;
            this.ReportList = this.GetTemplateChild("PART_ReportList") as ComboBox;
            this.btnSave = this.GetTemplateChild("PART_Sav") as OlapToolBarButton;
            this.btnLoad = this.GetTemplateChild("PART_Lod") as OlapToolBarButton;
            this.btnEnablePaging = this.GetTemplateChild("PART_EnablePaging") as OlapToolBarButton;
            this.btnNewServer = this.GetTemplateChild("PART_Server") as OlapToolBarButton;
            this.btnShowMdx = this.GetTemplateChild("PART_ShowMdx") as OlapToolBarButton;
            this.btnCreateCalcMeasure = this.GetTemplateChild("PART_CreateCalcMeasure") as OlapToolBarButton;
            this.btnVirtualKpiElement = this.GetTemplateChild("PART_showVirtualKpiEditor") as OlapToolBarButton;
            if (this.btnVirtualKpiElement != null)
            {
                if (this.IsVirtualKpiEnabled)
                    this.btnVirtualKpiElement.Visibility = Visibility.Visible;
                else
                    this.btnVirtualKpiElement.Visibility = Visibility.Collapsed;
            }
            if (this.btnCreateCalcMeasure != null)
            {
                if (this.IsCalculatedMembersEnabled)
                    this.btnCreateCalcMeasure.Visibility = Visibility.Visible;
                else
                    this.btnCreateCalcMeasure.Visibility = Visibility.Collapsed;
            }

            //// Initializing the source for full screen button
            _fullScreen = new Image();
            _fullScreen.Source = new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ToolBarImages/FullScreen.png", UriKind.RelativeOrAbsolute));
            _normal = new Image();
            _normal.Source = new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ToolBarImages/NormalScreen.png", UriKind.RelativeOrAbsolute));
            this.btnFullScreen.Content = _fullScreen;
            ResourceWrapper resWrapper = null;
            if (this.MainBorder != null)
            {
                resWrapper = this.MainBorder.Resources["resourceWrapper"] as ResourceWrapper;
            }
            if (resWrapper == null)
                resWrapper = new ResourceWrapper();
            ToolTipService.SetToolTip(_fullScreen, resWrapper.ViewFullScreenToolTip);
            ToolTipService.SetToolTip(_normal, resWrapper.ExitFullScreenToolTip);

            //// Event tagging
            this.btnAddReport.Click += AddReport_Click;
            this.btnNewReport.Click += NewReport_Click;
            this.btnRemoveReport.Click += RemoveReport_Click;
            this.btnReName.Click += ReName_Click;
            this.btnSave.Click += Save_Click;
            this.btnLoad.Click += Load_Click;
            this.ReportList.SelectionChanged += ReportList_SelectionChanged;
            this.btnFullScreen.MouseEnter += FullScreenButton_MouseEvent;
            this.btnFullScreen.MouseLeave += FullScreenButton_MouseEvent;
            this.btnFullScreen.Click += FullScreenButton_Click;
            this.btnTogglePivot.Click += btnTogglePivot_Click;
            this.btnShowExpander.Click += btnShowExpander_Click;
            this.btnAutoExecute.Click += btnAutoExecute_Click;
            this.btnEnablePaging.Click += new RoutedEventHandler(btnEnablePaging_Click);
            this.btnNewServer.Click += new RoutedEventHandler(btnNewServer_Click);
            this.btnShowMdx.Click += new RoutedEventHandler(btnShowMdx_Click);
            this.btnCreateCalcMeasure.Click += new RoutedEventHandler(btnCreateCalcMeasure_Click);
            this.btnVirtualKpiElement.Click += new RoutedEventHandler(btnVirtualKpiElement_Click);
        }

        void btnVirtualKpiElement_Click(object sender, RoutedEventArgs e)
        {
            LoadVirtualKpiEditor();
        }

        private void LoadVirtualKpiEditor()
        {
            this.Isprocessing = true;
            if (this.OlapDataManager != null && !String.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName))
            {
                VirtualKpiEditor virtualKpiEditor = new VirtualKpiEditor(this.OlapDataManager);
                virtualKpiEditor.Closed += new ClosedEventHandler(virtualKpiEditor_Closed);
                virtualKpiEditor.txtGoalExpression.Text = virtualKpiEditor.txtKpiName.Text = virtualKpiEditor.txtStatusExpression.Text = string.Empty;
                virtualKpiEditor.txtTrendExpression.Text = virtualKpiEditor.txtValueExpression.Text = string.Empty;
                virtualKpiEditor._value.IsSelected = true;
                virtualKpiEditor.cubeDimensionBrowser.OlapDataManager = this.OlapDataManager;
                if (this.CubeDimensionBrowser != null && this.CubeDimensionBrowser.Items.Count > 0)
                {
                    virtualKpiEditor.cubeDimensionBrowser.ItemsSource = this.CubeDimensionBrowser.ItemsSource;
                    virtualKpiEditor.cubeDimensionBrowser.Style = this.CubeDimensionBrowser.Style;
                    virtualKpiEditor.VisualStyle = (Windows.Shared.VisualStyle)Enum.Parse(typeof(Windows.Shared.VisualStyle), this.VisualStyle.ToString(), true);
                    SkinManager.SetVisualStyle(virtualKpiEditor, (Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Windows.Controls.Theming.VisualStyle), virtualKpiEditor.VisualStyle.ToString(), true));
                    virtualKpiEditor.ShowDialog();
                }
            }
            this.Isprocessing = false;
        }

        void virtualKpiEditor_Closed(object sender, ClosedEventArgs e)
        {
            if (sender is VirtualKpiEditor)
            {
                (sender as VirtualKpiEditor).Dispose();
            }
        }

        void btnCreateCalcMeasure_Click(object sender, RoutedEventArgs e)
        {
            ShowCalculatedMemberEditor();
        }

        void btnShowMdx_Click(object sender, RoutedEventArgs e)
        {
            ShowMdx();
        }

        /// <summary>
        /// Opens the calculated member editor window
        /// </summary>
        
        public void ShowCalculatedMemberEditor()
        {
            this.Isprocessing = true;
            if (this.OlapDataManager != null && !String.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName))
            {
                CalcMemberEditor calcMemberEditor = new CalcMemberEditor();
                calcMemberEditor.FlowDirection = this.FlowDirection;
                calcMemberEditor.CalcMeasureTreeView.Style = this.TreeViewStyle;
                calcMemberEditor.AutoExecute = this.AutoExecute;
                calcMemberEditor.CaptionText.Text = calcMemberEditor.ExpressionText.Text = calcMemberEditor.FormatText.Text = string.Empty;
                calcMemberEditor.CaptionText.Focus();
                if (calcMemberEditor.CalcMeasureTreeView.OlapDataManager == null || this.OlapDataManager.CurrentCubeName != calcMemberEditor.CurrentCubeName)
                {                    
                    calcMemberEditor.CurrentCubeName = this.OlapDataManager.CurrentCubeName;
                    calcMemberEditor.CalcMeasureTreeView.OlapDataManager = this.OlapDataManager;
                    if (this.CubeDimensionBrowser != null && this.CubeDimensionBrowser.Items.Count > 0)
                    {
                        calcMemberEditor.CalcMeasureTreeView.ItemsSource = this.CubeDimensionBrowser.ItemsSource;
                        calcMemberEditor.MemberTypeText.ItemsSource = (this.CubeDimensionBrowser.Items[0] as MetaTreeNode).ChildNodes.Where(i => i.NodeType == MetaTreeNodeType.Dimension);
                        calcMemberEditor.MemberTypeText.SelectedIndex = 0;
                    }
                }
                calcMemberEditor.VisualStyle = (Windows.Shared.VisualStyle)Enum.Parse(typeof(Windows.Shared.VisualStyle), this.VisualStyle.ToString(), true);
                calcMemberEditor.Background = this.Background;
                SkinManager.SetVisualStyle(calcMemberEditor, (Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Windows.Controls.Theming.VisualStyle), calcMemberEditor.VisualStyle.ToString(), true));
                calcMemberEditor.ShowDialog();
                calcMemberEditor.Closed += new ClosedEventHandler(calcMemberEditor_Closed);

            }
           
            this.Isprocessing = false;
        }
       
        void calcMemberEditor_Closed(object sender, ClosedEventArgs e)
        {
            if (sender is CalcMemberEditor)
            {
                (sender as CalcMemberEditor).Dispose();
            }
        }

        /// <summary>
        /// Shows MDX query for current Report
        /// </summary>
        public void ShowMdx()
        {
            this.Isprocessing = true;
            if (this.OlapDataManager != null && this.OlapDataManager.CurrentReport != null)
            {
                this.OlapDataManager.GetMdxQuery(this.OlapDataManager.CurrentReport);
            }
        }

        /// <summary>
        /// Update the OlapGrid tool bar layout.
        /// </summary>
        private void UpdateGridToolBarLayout()
        {
            //// Layout updating
            this.OlapGridToolBar = this.GetTemplateChild("PART_GridToolBar") as OlapToolBar;
            this.btnGridStyle = this.GetTemplateChild("PART_GStyle") as OlapToolBarButton;
            this.btnValueTooltip = this.GetTemplateChild("PART_VToolTip") as OlapToolBarButton;
            this.btnFrzHeader = this.GetTemplateChild("PART_Freeze") as OlapToolBarButton;
            this.btnGridWrd = this.GetTemplateChild("PART_GWrd") as OlapToolBarButton;
            this.btnGridExl = this.GetTemplateChild("PART_GExl") as OlapToolBarButton;
            this.btnGridPdf = this.GetTemplateChild("PART_GPdf") as OlapToolBarButton;

            this.cmbxGridLayout = this.GetTemplateChild("PART_GLayout") as ComboBox;

            ////Event tagging
            if (this.cmbxGridLayout != null)
                this.cmbxGridLayout.SelectionChanged += gridLayout_SelectionChanged;
            if (this.btnGridStyle != null)
                this.btnGridStyle.Click += btnGridStyle_Click;
            if (this.btnValueTooltip != null)
                this.btnValueTooltip.Click += btnValueTooltip_Click;
            if (this.btnFrzHeader != null)
                this.btnFrzHeader.Click += btnFrzHeader_Click;
            if (this.btnGridWrd != null)
                this.btnGridWrd.Click += btnGridWrd_Click;
            if (this.btnGridExl != null)
                this.btnGridExl.Click += btnGridExl_Click;
            if (this.btnGridPdf != null)
                this.btnGridPdf.Click += btnGridPdf_Click;

            //// Assigning items source
            if (this.cmbxGridLayout != null)
            {
                ResourceWrapper resWrapper = null;
                if (this.MainBorder != null)
                {
                    resWrapper = this.MainBorder.Resources["resourceWrapper"] as ResourceWrapper;
                }
                if (resWrapper == null)
                    resWrapper = new ResourceWrapper();
                System.Collections.Generic.List<ImageData> comboItems = new System.Collections.Generic.List<ImageData>();
                comboItems.Add(new ImageData(resWrapper.OlapGridLayout_Normal, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ToolBarImages/Normal.png", UriKind.RelativeOrAbsolute))));
                comboItems.Add(new ImageData(resWrapper.OlapGridLayout_ExcelLike, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ToolBarImages/Normal.png", UriKind.RelativeOrAbsolute))));
                comboItems.Add(new ImageData(resWrapper.OlapGridLayout_NoSummaries, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ToolBarImages/NoSummaries.png", UriKind.RelativeOrAbsolute))));
                comboItems.Add(new ImageData(resWrapper.OlapGridLayout_NormalTopSummary, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ToolBarImages/NormalTopSummary.png", UriKind.RelativeOrAbsolute))));
                this.cmbxGridLayout.ItemsSource = comboItems;
                //
                if (this.GridLayout == OlapSilverlight.Engine.GridLayout.ExcelLikeLayout)
                    this.cmbxGridLayout.SelectedIndex = 1;
                else if (this.GridLayout == OlapSilverlight.Engine.GridLayout.NoSummaries)
                    this.cmbxGridLayout.SelectedIndex = 2;
                else if (this.GridLayout == OlapSilverlight.Engine.GridLayout.NormalTopSummary)
                    this.cmbxGridLayout.SelectedIndex = 3;
                else
                    this.cmbxGridLayout.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Update OlapChart tool bar layout.
        /// </summary>
        private void UpdateChartToolBarLayout()
        {
            ////Layout updation
            this.OlapChartToolBar = this.GetTemplateChild("PART_ChartToolBar") as OlapToolBar;
            this.cmbxChartTypes = this.GetTemplateChild("Part_ChartTypes") as ComboBox;
            this.cmbxChartPalette = this.GetTemplateChild("Part_ChartPalette") as ComboBox;
            this.btnShowChartToolTip = this.GetTemplateChild("PART_ChartToolTip") as OlapToolBarButton;
            this.btnShowLegend = this.GetTemplateChild("PART_ShowLegend") as OlapToolBarButton;

            //// Event tagging
            if (this.cmbxChartTypes != null)
                this.cmbxChartTypes.SelectionChanged += cmbxChartTypes_SelectionChanged;
            if (this.cmbxChartPalette != null)
                this.cmbxChartPalette.SelectionChanged += cmbxChartPalette_SelectionChanged;
            if (this.btnShowChartToolTip != null)
                this.btnShowChartToolTip.Click += btnShowChartToolTip_Click;
            if (this.btnShowLegend != null)
                this.btnShowLegend.Click += btnShowLegend_Click;

            ResourceWrapper resourceWrapper = null;
            if (this.MainBorder != null)
            {
                resourceWrapper = this.MainBorder.Resources["resourceWrapper"] as ResourceWrapper;
            }
            if (resourceWrapper == null)
                resourceWrapper = new ResourceWrapper();

            //// Assigning items source
            if (this.cmbxChartTypes != null)
            {                
                System.Collections.Generic.List < ImageData > comoBoxItems= new System.Collections.Generic.List<ImageData>();
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Area, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Area.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Bar, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Bar.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Column, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Column.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Funnel, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Funnel.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Line, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Line.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Pie, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Pie.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Polar, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Polar.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Pyramid, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Pyramid.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Radar, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Radar.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_RotatedSpline, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/RotatedSpline.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Scatter, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Scatter.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_Spline, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/Spline.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_SplineArea, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/SplineArea.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_StackingArea, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/StackingArea.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_StackingBar, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/StackingBar.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_StackingBar100, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/StackingBar100.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_StackingColumn, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/StackingColumn.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_StackingColumn100, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/StackingColumn100.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_StepArea, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/StepArea.png", UriKind.RelativeOrAbsolute))));
                comoBoxItems.Add(new ImageData(resourceWrapper.OlapChartTypes_StepLine, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartTypes/StepLine.png", UriKind.RelativeOrAbsolute))));
                this.cmbxChartTypes.ItemsSource = comoBoxItems;
                this.cmbxChartTypes.SelectedIndex = 2;
            }

            //// Assigning items source
            if (this.cmbxChartPalette != null)
            {                
                System.Collections.Generic.List<ImageData> comoBoItem = new System.Collections.Generic.List<ImageData>();
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Analog, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Analog.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Colorful, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Colorful.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Custom, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Custom.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Default, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Default.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_DefaultAlpha, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/DefaultAlpha.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_DefaultDark, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Defaultdark.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_EarthTone, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/EarthTone.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_GrayScale, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Grayscale.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Metro, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Metro.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Nature, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Nature.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Palette1, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Palette1.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Palette2, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Palette2.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Palette3, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Palette3.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Palette4, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Palette4.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Palette5, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Palette5.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Palette6, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Palette6.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Palette7, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Palette7.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Palette8, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Palette8.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Pastel, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Pastel.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_Triad, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/Triad.png", UriKind.RelativeOrAbsolute))));
                comoBoItem.Add(new ImageData(resourceWrapper.OlapChart_ColorPalette_WarmCold, new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Images/ChartPalettes/WarmCold.png", UriKind.RelativeOrAbsolute))));
                this.cmbxChartPalette.ItemsSource = comoBoItem;
                this.cmbxChartPalette.SelectedIndex = 3;
            }
        }

        #endregion

        #region Grid ToolBar items event handler

        /// <summary>
        /// Handles the Click event of the btnGridStyle button to show the Style dialog of OlapGrid
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnGridStyle_Click(object sender, RoutedEventArgs e)
        {
            this.ShowGridStyleDialog();
        }

        /// <summary>
        /// Handles the Click event of the btnFrzHeader button to Freeze the Headers of OlapGrid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnFrzHeader_Click(object sender, RoutedEventArgs e)
        {
            this.OlapGrid.FreezeHeaders = !this.OlapGrid.FreezeHeaders;
        }

        /// <summary>
        /// Handles the Click event of the btnValueTooltip button to enable or disable the value cell tool tip.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnValueTooltip_Click(object sender, RoutedEventArgs e)
        {
            this.OlapGrid.ShowValueCellToolTip = !this.OlapGrid.ShowValueCellToolTip;
        }

        /// <summary>
        /// Handles the Click event of the btnGridWrd button to Export the OlapGrid to Word.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnGridWrd_Click(object sender, RoutedEventArgs e)
        {
            this.ExportGridToWord();
        }

        /// <summary>
        /// Handles the Click event of the btnGridExl button to Export the OlapGrid to Excel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnGridExl_Click(object sender, RoutedEventArgs e)
        {
            this.ExportGridToExcel();
        }

        /// <summary>
        /// Handles the Click event of the btnGridPdf to Export the OlapGrid to Excel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnGridPdf_Click(object sender, RoutedEventArgs e)
        {
            this.ExportGridToPdf();
        }

        /// <summary>
        /// Shows the grid style dialog.
        /// </summary>
        private void ShowGridStyleDialog()
        {
            try
            {
                this.OlapGrid.ShowStyleDialog();
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture,"OlapClient_Errors_LoadingStyleDialog"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Exports the grid to word.
        /// </summary>
        public void ExportGridToWord()
        {
            try
            {
                SaveFileDialog sfv = new SaveFileDialog();
                sfv.DefaultExt = ".Doc";
                sfv.Filter = "(*.Doc)|*.Doc";
                if (sfv.ShowDialog() == true)
                {
                    Stream stream = sfv.OpenFile();
                    if (this.OlapGrid.GridStyleInfo != null)
                        _gridStyleInfo = this.OlapGrid.GridStyleInfo;
                    _gridStyleInfo = _gridStyleInfo ?? new ExportingGridStyleInfo();
                    GridWordExport wordExport = new GridWordExport(this.OlapGrid.OlapDataManager.PivotEngine, this.OlapGrid.Layout);
                    wordExport.Export(stream, _gridStyleInfo);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_FailedToExport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Exports the grid to excel.
        /// </summary>
        public void ExportGridToExcel()
        {
            try
            {
                SaveFileDialog sfv = new SaveFileDialog();

                sfv.DefaultExt = ".xls";
                sfv.Filter = "(*.xls)|*.xls";

                if (sfv.ShowDialog() == true)
                {
                    Stream stream = sfv.OpenFile();
                    if (this.OlapGrid.GridStyleInfo != null)
                        _gridStyleInfo = this.OlapGrid.GridStyleInfo;
                    _gridStyleInfo = _gridStyleInfo ?? new ExportingGridStyleInfo();
                    GridExcelExport exportExcel = new GridExcelExport(this.OlapGrid.OlapDataManager.PivotEngine, _gridStyleInfo, this.OlapGrid.Layout, this.OlapGrid.OlapDataManager.ItemSource == null ? false : true);
                    exportExcel.Export(stream);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_FailedToExport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Exports the grid to PDF.
        /// </summary>
        public void ExportGridToPdf()
        {
            try
            {
                SaveFileDialog sfv = new SaveFileDialog();
                sfv.DefaultExt = ".pdf";
                sfv.Filter = "(*.pdf)|*.pdf";
                if (sfv.ShowDialog() == true)
                {
                    Stream stream = sfv.OpenFile();
                    if (this.OlapGrid.GridStyleInfo != null)
                        _gridStyleInfo = this.OlapGrid.GridStyleInfo;
                    _gridStyleInfo = _gridStyleInfo ?? new ExportingGridStyleInfo();
                    GridPdfExport exporttopdf = new GridPdfExport(this.OlapGrid.OlapDataManager.PivotEngine, _gridStyleInfo);
                    exporttopdf.Export(stream);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_FailedToExport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the gridLayout combo box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void gridLayout_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.cmbxGridLayout.IsDropDownOpen = false;
            if (this.OlapDataManager != null)
            {
                if (this.cmbxGridLayout.SelectedIndex == 0)
                {
                    this.OlapGrid.Layout = OlapSilverlight.Engine.GridLayout.Normal;
                    this.OlapGrid.Refresh();
                }
                else if (this.cmbxGridLayout.SelectedIndex == 1)
                {
                    this.OlapGrid.Layout = OlapSilverlight.Engine.GridLayout.ExcelLikeLayout;
                    this.OlapDataManager.NotifyCellSetChanged();
                    this.OlapGrid.Refresh();
                }
                else if (this.cmbxGridLayout.SelectedIndex == 2)
                {
                    this.OlapGrid.Layout = OlapSilverlight.Engine.GridLayout.NoSummaries;
                    this.OlapDataManager.NotifyCellSetChanged();
                    this.OlapGrid.Refresh();
                }
                else if (this.cmbxGridLayout.SelectedIndex == 3)
                {
                    this.OlapGrid.Layout = OlapSilverlight.Engine.GridLayout.NormalTopSummary;
                    this.OlapDataManager.NotifyCellSetChanged();
                    this.OlapGrid.Refresh();
                }
            }
        }

        #endregion

        #region Chart ToolBar items event handler

        /// <summary>
        /// Handles the Click event of the btnShowLegend button to show/hide the chart legends.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnShowLegend_Click(object sender, RoutedEventArgs e)
        {
            if (this.OlapChart.OlapArea != null)
            {
                if (this.OlapChart.OlapArea.Legends.Visibility == System.Windows.Visibility.Collapsed)
                {
                    this.OlapChart.OlapArea.Legends.Visibility = System.Windows.Visibility.Visible;
                }
                else if (this.OlapChart.OlapArea.Legends.Visibility == System.Windows.Visibility.Visible)
                {
                    this.OlapChart.OlapArea.Legends.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbxChartPalette combo box to change the chart color palette.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void cmbxChartPalette_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.cmbxChartPalette.SelectedIndex == 0)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Analog;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 1)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Colorful;
            }
            else if(this.cmbxChartPalette.SelectedIndex==2)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Custom;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 3)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Default;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 4)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.DefaultAlpha;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 5)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.DefaultDark;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 6)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.EarthTone;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 7)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Grayscale;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 8)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Metro;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 9)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Nature;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 10)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Palette1;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 11)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Palette2;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 12)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Palette3;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 13)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Palette4;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 14)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Palette5;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 15)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Palette6;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 16)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Palette7;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 17)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Palette8;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 18)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Pastel;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 19)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.Triad;
            }
            else if (this.cmbxChartPalette.SelectedIndex == 20)
            {
                this.OlapChart.OlapChartColorPalette = ChartColorPalette.WarmCold;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnShowChartToolTip button to enable or disable the series tool tip.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnShowChartToolTip_Click(object sender, RoutedEventArgs e)
        {
            this.OlapChart.ShowSeriesToolTip = !this.OlapChart.ShowSeriesToolTip;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbxChartTypes combo box to change the chart type.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void cmbxChartTypes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                this.Isprocessing = true;
                cmbxChartTypes.IsDropDownOpen = false;
                this.Dispatcher.BeginInvoke(() =>
                {
                    if (this.cmbxChartTypes.SelectedIndex == 0)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Area;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 1)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Bar;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 2)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Column;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 3)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Funnel;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 4)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Line;
                    }
                    else if(this.cmbxChartTypes.SelectedIndex==5)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Pie;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 6)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Polar;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 7)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Pyramid;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 8)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Radar;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 9)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.RotatedSpline;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 10)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Scatter;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 11)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.Spline;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 12)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.SplineArea;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 13)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.StackingArea;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 14)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.StackingBar;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 15)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.StackingBar100;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 16)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.StackingColumn;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 17)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.StackingColumn100;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 18)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.StepArea;
                    }
                    else if (this.cmbxChartTypes.SelectedIndex == 19)
                    {
                        this.OlapChart.OlapChartType = OlapChartTypes.StepLine;
                    }
                });
            }
        }

        void btnNewServer_Click(object sender, RoutedEventArgs e)
        {
            ShowConnectionDialog();
        }

        /// <summary>
        /// Opens the new connection dialog.
        /// </summary>
        public void ShowConnectionDialog()
        {
            ConnectionDialog connectionDlg = new ConnectionDialog(this.ConnectionString, this.ProviderName);
            connectionDlg.FlowDirection = this.FlowDirection;
            connectionDlg.Title = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_ConnectionDialog_Title");
            connectionDlg.Closed -= new ClosedEventHandler(ConnectionDlg_Closed);
            connectionDlg.Closed += new ClosedEventHandler(ConnectionDlg_Closed);
            connectionDlg.VisualStyle = (Windows.Shared.VisualStyle)Enum.Parse(typeof(Windows.Shared.VisualStyle), this.VisualStyle.ToString(), true);
            connectionDlg.Background = this.Background;
            SkinManager.SetVisualStyle(connectionDlg, (Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Windows.Controls.Theming.VisualStyle), connectionDlg.VisualStyle.ToString(), true));
            connectionDlg.ShowDialog();
        }

        void ConnectionDlg_Closed(object sender, ClosedEventArgs e)
        {
            ConnectionDialog dialog = sender as ConnectionDialog;
            if (dialog != null)
            {
                if (dialog.DialogResult && !string.IsNullOrEmpty(dialog.ConnectionString))
                {
                    this.Isprocessing = true;
                    UpdateConnection(dialog.ConnectionString, dialog.ProviderName);
                }
            }
        }
        #endregion

        #region Progress bar event handlers

        /// <summary>
        /// Handles the CellsSet changed event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.OlapSilverlight.Manager.CellSetChangedEventArgs"/> instance containing the event data.</param>
        void CellSetChanged(object sender, CellSetChangedEventArgs e)
        {
            //this.Isprocessing = false;
        }

        /// <summary>
        ///Handles the CellsSet changing event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.OlapSilverlight.Manager.CellSetChangingEventArgs"/> instance containing the event data.</param>
        void CellSetChanging(object sender, CellSetChangingEventArgs e)
        {
            this.Isprocessing = true;

            //// Hiding the processing bar of OlapChart and OlapGrid
            this.OlapChart.ShowProcessingBar = false;
            this.OlapGrid.ShowProcessingBar = false;
        }

        #endregion

        #region Reports event handlers

        /// <summary>
        /// Handles the Click event of the btnEnablePaging control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnEnablePaging_Click(object sender, RoutedEventArgs e)
        {
            if (this.OlapDataManager != null && this.OlapDataManager.CurrentReport != null)
            {
                this.btnEnablePaging.IsChecked = this.OlapDataManager.CurrentReport.EnablePaging = !this.OlapDataManager.CurrentReport.EnablePaging;
                this.OlapDataManager.ExecuteCellSet();
                if (this.OlapPager != null)
                {
                    this.OlapPager.Visibility = this.OlapDataManager.CurrentReport.EnablePaging ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    this.OlapPager.Dispatcher.BeginInvoke(() =>
                        {
                            ApplyVisualStyleforOlapPager(this.VisualStyle.ToString());
                        });
                    
                }
                this._tabborder.Margin = this.OlapDataManager.CurrentReport.EnablePaging ? new Thickness(0, 0, -10, -40) : new Thickness(0, 0, -10, -10);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnShowExpander button to show/hide the expanders.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnShowExpander_Click(object sender, RoutedEventArgs e)
        {
            ShowOrHideExpander();
        }

        /// <summary>
        /// Handles the Click event of the btnAutoExecute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnAutoExecute_Click(object sender, RoutedEventArgs e)
        {
            this.OlapDataManager.NotifyReportChanged();
        }

        /// <summary>
        /// Handles the Click event of the btnTogglePivot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void btnTogglePivot_Click(object sender, RoutedEventArgs e)
        {
            TogglePivot();
        }

        /// <summary>
        /// Handles the Click event of the Load report button to load a report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadReportDefinition();
        }

        /// <summary>
        /// Handles the Click event of the Save button to save the report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveReport();
        }

        /// <summary>
        /// Handles the Click event of the NewReport button to create a new session.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void NewReport_Click(object sender, RoutedEventArgs e)
        {
            this.CreateNewSession();
        }

        /// <summary>
        /// Handles the Click event of the AddReport button to add a new report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void AddReport_Click(object sender, RoutedEventArgs e)
        {
            AddReport();
        }

        /// <summary>
        /// Handles the Click event of the ReName button to rename the current report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void ReName_Click(object sender, RoutedEventArgs e)
        {
            RenameReport();
        }

        /// <summary>
        /// Handles the Click event of the RemoveReport button to remove the current report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void RemoveReport_Click(object sender, RoutedEventArgs e)
        {
            RemoveReport();
        }

        /// <summary>
        /// Shows/Hide expander buttons in Chart and Grid
        /// </summary>
        public void ShowOrHideExpander()
        {
            try
            {
                if (this.OlapDataManager != null && !string.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName) && !string.IsNullOrEmpty(this.OlapDataManager.CurrentReport.Name))
                {
                    this.btnShowExpander.IsChecked = this.OlapDataManager.CurrentReport.ShowExpanders = !this.OlapDataManager.CurrentReport.ShowExpanders;
                    this.OlapDataManager.ExecuteCellSet();
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_ToggleExpanderVisiblity"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Swap the categorical and series element in the result set.
        /// </summary>
        public void TogglePivot()
        {
            try
            {
                if (this.OlapDataManager != null && !string.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName) && !string.IsNullOrEmpty(this.OlapDataManager.CurrentReport.Name))
                {
                    this.btnTogglePivot.IsChecked = this.OlapDataManager.CurrentReport.TogglePivot = !this.OlapDataManager.CurrentReport.TogglePivot;
                    //this.OlapDataManager.ExecuteCellSet();
                    this.OlapDataManager.NotifyElementChanged();
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture,"OlapClient_Errors_ToggleExpanderVisiblity"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ReportList combo box to change the current report of OlapDataManager.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void ReportList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                ReportList.IsDropDownOpen = false;
                if (this.ReportList.SelectedItem != null)
                {
                    if (this.OlapDataManager.Cubes != null && this.OlapDataManager.Cubes.Count > 0)
                    {
                        this.OlapDataManager.SetCurrentReport(this.ReportList.SelectedItem as OlapReport);
                        this.UpdatePagerVisibility();
                        this.OlapDataManager.NotifyReportChanged();
                        var r = (from cube in this.OlapDataManager.Cubes where cube.Name.ToUpper().Equals(this.OlapDataManager.CurrentCubeName.ToUpper()) select cube).SingleOrDefault();
                        if (r != null)
                            this.CubeSelector.SelectedItem = r as CubeInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture,"OlapClient_Errors_ChangingtheReport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Loads the report definition.
        /// </summary>
        public void LoadReportDefinition()
        {
            if (!this.OlapDataManager.IsCurrentReportModified)
            {
                LoadReports();
            }
            else
            {
                string message = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_Savechanges");
                string title = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_Save");
                WindowControl.ShowAlert(message,title, DialogIcon.Exclamation, DialogButton.YesNoCancel, LoadAlertClosed, AnimationType.HorizontalSwivel);
            }
        }

        /// <summary>
        /// Handles the Alert window closed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.ClosedEventArgs"/> instance containing the event data.</param>
        void LoadAlertClosed(object sender, ClosedEventArgs e)
        {
            e.Cancel = false;
            if (e.DialogResult == DialogResult.Yes)
            {
                this.SaveReport();
                this.LoadReports();
            }
            else if (e.DialogResult == DialogResult.No)
            {
                this.LoadReports();
            }
        }

        /// <summary>
        /// Loads the reports set from xml file.
        /// </summary>
        private void LoadReports()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "(.xml)|*.xml";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == true)
                {
                    Stream filePath = openFileDialog.File.OpenRead();
                    this.OlapDataManager.LoadReportFromStream(filePath);
                    if (this.OlapDataManager.ReportList.Count > 0)
                    {
                        this.ReportList.ItemsSource = this.OlapDataManager.ReportList;
                        if (this.OlapDataManager.CurrentReport != null)
                        {
                            int currentReportIndex = this.OlapDataManager.ReportList.IndexOf(this.OlapDataManager.CurrentReport);
                            this.AxisElementBuilderColumn.IsSavedReport = this.AxisElementBuilderRow.IsSavedReport = this.AxisElementBuilderSlicer.IsSavedReport = true;
                            this.ReportList.SelectedIndex = currentReportIndex > -1 ? currentReportIndex : 0;
                        }
                        else
                        {
                            this.ReportList.SelectedIndex = 0;
                        }
                    }
                    this.cmbxChartPalette.SelectedItem = this.OlapChart.OlapChartColorPalette.ToString();
                    this.cmbxChartTypes.SelectedItem = this.OlapChart.OlapChartType.ToString();
                    this.cmbxGridLayout.SelectedItem = this.OlapGrid.Layout.ToString();
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_LoadingReport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Saves the report.
        /// </summary>
        public bool SaveReport()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "xml";
                saveFileDialog.Filter = "(.xml)|*.xml";

                if (saveFileDialog.ShowDialog() == true)
                {
                    Stream fileName = saveFileDialog.OpenFile();
                    string s = saveFileDialog.SafeFileName;
                    this.OlapDataManager.CurrentReport.ChartSettings = this.OlapChart.ChartSettings;
                    this.OlapDataManager.CurrentReport.GridSettings = this.OlapGrid.GridSettings;
                    this.OlapDataManager.SaveReport(fileName);

                    return true;
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_SavingReport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
            return false;
        }

        /// <summary>
        /// Saves the report
        /// </summary>
        public void SaveReports()
        {
            this.SaveReport();
        }

        /// <summary>
        /// Creates the new session.
        /// </summary>
        public void CreateNewSession()
        {
            if (!this.OlapDataManager.IsCurrentReportModified)
            {
                CreateNewReportSet();
            }
            else
            {
                string message = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_Savechanges");
                string title = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_Save");
                WindowControl.ShowAlert(message, title, DialogIcon.Exclamation, DialogButton.YesNoCancel, NewSessionAlertClosed, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Handles the Alert window closed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.ClosedEventArgs"/> instance containing the event data.</param>
        void NewSessionAlertClosed(object sender, ClosedEventArgs e)
        {
            if (e.DialogResult == DialogResult.Yes)
            {
                this.Save();
                (sender as WindowControl).Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (e.DialogResult == DialogResult.No)
            {
                CreateNewReportSet();
            }
        }

        /// <summary>
        /// Invokes save report method
        /// </summary>
        private void Save()
        {
            if (this.SaveReport())
            {
                this.AddReport();
            }
            else
            {
                string message = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_Savechanges");
                string title = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_Save");
                WindowControl.ShowAlert(message, title, DialogIcon.Exclamation, DialogButton.YesNoCancel, NewSessionAlertClosed, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Creates the new report set.
        /// </summary>
        private void CreateNewReportSet()
        {
            _reportNameGetter = new ReportNameGetter();
            _reportNameGetter.FlowDirection = this.FlowDirection;
            _reportNameGetter.Title = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_ReportNameGetter_NewReport");
            _reportNameGetter.DialogType = ReportDialogType.NewReport;
            _reportNameGetter.VisualStyle = (Windows.Shared.VisualStyle)Enum.Parse(typeof(Windows.Shared.VisualStyle), this.VisualStyle.ToString(), true);
            _reportNameGetter.Background = this.Background;
            SkinManager.SetVisualStyle(_reportNameGetter, (Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Windows.Controls.Theming.VisualStyle), this.VisualStyle.ToString(), true));
            _reportNameGetter.ShowDialog();

            //// Event tagging
            this._reportNameGetter.Closed += _reportNameGetter_Closed;
        }

        /// <summary>
        /// Add a report to the report set.
        /// </summary>
        public void AddReport()
        {
            try
            {
                if (this.OlapDataManager != null && this.OlapDataManager.ReportList.Count > 0)
                {
                    _reportNameGetter = new ReportNameGetter();
                    _reportNameGetter.FlowDirection = this.FlowDirection;
                    _reportNameGetter.ReportList = this.OlapDataManager.ReportList;
                    _reportNameGetter.Title = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_ReportNameGetter_AddReport");
                    _reportNameGetter.DialogType = ReportDialogType.AddReport;
                    _reportNameGetter.VisualStyle = (Windows.Shared.VisualStyle)Enum.Parse(typeof(Windows.Shared.VisualStyle), this.VisualStyle.ToString(), true);
                    _reportNameGetter.Background = this.Background;
                    SkinManager.SetVisualStyle(_reportNameGetter, (Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Windows.Controls.Theming.VisualStyle), this.VisualStyle.ToString(), true));
                    _reportNameGetter.ShowDialog();

                    //// Event tagging
                    this._reportNameGetter.Closed += _reportNameGetter_Closed;
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_AddingReport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Updates the connection.
        /// </summary>
        /// <param name="newConnectionString">The new connection string.</param>
        public void UpdateConnection(string newConnectionString)
        {
            ResetClient(true);
            if (!this.Isprocessing)
            {
                this.Isprocessing = true;
            }

            this.OlapDataManager.ConnectionString = newConnectionString;
            if (this.OlapDataManager != null)
            {
                this.OlapDataManager.GetCubes();
            }
        }

        /// <summary>
        /// Update the connection based on connection string and provider details.
        /// </summary>
        /// <param name="newConnectionString">The new connection string.</param>
        /// <param name="providerName">The provider name.</param>
        public void UpdateConnection(string newConnectionString, Providers providerName)
        {
            ResetClient(true);
            if (!this.Isprocessing)
            {
                this.Isprocessing = true;
            }

            this.ConnectionString = newConnectionString;
            this.ProviderName = providerName;
            if (this.OlapDataManager != null)
            {
                this.OlapDataManager.GetCubes();
            }
        }


        /// <summary>
        /// Resets the client.
        /// </summary>
        public void ResetClient()
        {
            ResetClient(false);
        }

        /// <summary>
        /// Resets the client with or without connection string
        /// </summary>
        internal void ResetClient(bool newConnection)
        {
            if (!this.Isprocessing)
            {
                this.Isprocessing = true;
            }

            this.cubedimbrowserUpdated = this.chartOrGridUpdated = false;
            this.CubeDimensionBrowser.ItemsSource = null;
            this.CubeSelector.ItemsSource = null;
            this.AxisElementBuilderRow.MetaTreeNodes.Clear();
            this.AxisElementBuilderColumn.MetaTreeNodes.Clear();
            this.AxisElementBuilderSlicer.MetaTreeNodes.Clear();
            this.OlapDataManager.ReportList.Clear();            
            this.OlapDataManager.CurrentReport = null;
            if (newConnection)
                this.OlapDataManager.CurrentCubeName = null;
            this.OlapChart.DataBind();
            this.OlapGrid.DataBind();
            this.Isprocessing = false;
        }

        /// <summary>
        /// Rename the current report.
        /// </summary>
        public void RenameReport()
        {
            try
            {
                if (this.OlapDataManager != null && !string.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName) && !string.IsNullOrEmpty(this.OlapDataManager.CurrentReport.Name))
                {
                    _reportNameGetter = new ReportNameGetter();
                    _reportNameGetter.FlowDirection = this.FlowDirection;
                    _reportNameGetter.ReportList = this.OlapDataManager.ReportList;
                    _reportNameGetter.Title = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_ReportNameGetter_RenameReport");
                    _reportNameGetter.DialogType = ReportDialogType.RenameReport;
                    _reportNameGetter.VisualStyle = (Windows.Shared.VisualStyle)Enum.Parse(typeof(Windows.Shared.VisualStyle), this.VisualStyle.ToString(), true);
                    _reportNameGetter.Background = this.Background;
                    SkinManager.SetVisualStyle(_reportNameGetter, (Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Windows.Controls.Theming.VisualStyle), this.VisualStyle.ToString(), true));
                    _reportNameGetter.ShowDialog();

                    //// Event tagging
                    this._reportNameGetter.Closed += _reportNameGetter_Closed;
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_RenamingReport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Removes the current report from report set.
        /// </summary>
        public void RemoveReport()
        {
            try
            {
                if (this.OlapDataManager != null && this.OlapDataManager.ReportList.Count > 1 && this.ReportList.SelectedItem != null)
                {
                    this.OlapDataManager.ReportList.Remove(this.OlapDataManager.CurrentReport);
                    this.ReportList.SelectedIndex = this.ReportList.Items.Count - 1;
                    this.OlapDataManager.IsCurrentReportModified = true;
                    this.UpdateReportButtons();
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_RemovingReport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Handles the closed event of add/rename report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void _reportNameGetter_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                e.Cancel = true;
                if ((sender as ReportNameGetter).DialogResult == true)
                {
                    this.OlapDataManager.MdxQuery = null;
                    switch (this._reportNameGetter.DialogType)
                    {
                        case ReportDialogType.NewReport:
                            this.OlapDataManager.ReportList.Clear();
                            AddReport(_reportNameGetter.NewReportName);
                            this.AxisElementBuilderColumn.IsSavedReport = this.AxisElementBuilderRow.IsSavedReport = this.AxisElementBuilderSlicer.IsSavedReport = false;
                            this.OlapDataManager.IsCurrentReportModified = true;
                            break;
                        case ReportDialogType.AddReport:
                            AddReport(_reportNameGetter.NewReportName);
                            this.AxisElementBuilderColumn.IsSavedReport = this.AxisElementBuilderRow.IsSavedReport = this.AxisElementBuilderSlicer.IsSavedReport = false;
                            this.OlapDataManager.IsCurrentReportModified = true;
                            this.UpdateReportButtons();
                            break;
                        case ReportDialogType.RenameReport:
                            ReNameReport(this.OlapDataManager.CurrentReport.Name, _reportNameGetter.NewReportName);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_ProcessReport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Adds the report.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        private void AddReport(string reportName)
        {
            OlapReport report = new OlapReport(reportName);
            //report.CurrentCubeName = this.OlapDataManager.Cubes[0].Name;
            report.CurrentCubeName = this.CubeSelector.Items.Count > 0 ? (this.CubeSelector.Items[this.CubeSelector.SelectedIndex] as CubeInfo).Name.ToString() : this.OlapDataManager.Cubes[0].Name; //this.CubeSelector.Items[this.CubeSelector.SelectedIndex].ToString(); //this.CubeSelector.Items.Count > 0 ? (this.CubeSelector.Items[this.CubeSelector.SelectedIndex] as CubeInfo).Name.ToString() : this.OlapDataManager.Cubes[0].Name;
            report.EnablePaging = this.EnablePaging;
            report.UseDefaultMember = this.UseDefaultMember;
            report.VisualTotalVisibility = this.VisualTotalVisibility;
            report.DrillType = this.OlapDataManager.CurrentReport.DrillType;
            this.OlapDataManager.ReportList.Add(report);
            this.ReportList.SelectedIndex = this.ReportList.Items.Count - 1;
        }

        /// <summary>
        /// Adds the new report.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        /// <param name="newReport">The new report.</param>
        public void AddCustomReport(string reportName, OlapReport newReport)
        {
            try
            {
                if (this.OlapDataManager != null && newReport != null && !string.IsNullOrEmpty(reportName) && (!string.IsNullOrEmpty(newReport.CurrentCubeName) || !string.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName)))
                {
                    newReport.Name = reportName;
                    newReport.CurrentCubeName = string.IsNullOrEmpty(newReport.CurrentCubeName) ? this.OlapDataManager.CurrentCubeName : newReport.CurrentCubeName;
                    this.OlapDataManager.ReportList.Add(newReport);

                    if (this.OlapDataManager.Cubes != null)
                    {
                        this.ReportList.SelectedIndex = this.ReportList.Items.Count - 1;
                    }
                    this.UpdateReportButtons();
                }
                else if (string.IsNullOrEmpty(reportName))
                {
                    throw new Exception(SR.GetString(CultureInfo.CurrentUICulture, "Exception_PleaseSpecifyTheReportNameInTheReport"));
                }
                else if (string.IsNullOrEmpty(newReport.CurrentCubeName) || string.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName))
                {
                    throw new Exception(SR.GetString(CultureInfo.CurrentUICulture, "Exception_PleaseSpecifyTheCubeNameInTheReport"));
                }
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_AddingReport"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Res the name report.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        private void ReNameReport(string oldName, string newName)
        {
            this.OlapDataManager.ReportList[oldName].Name = newName;
            this.ReportList.ItemsSource = new OlapReportCollection();
            this.ReportList.ItemsSource = this.OlapDataManager.ReportList;
            this.ReportList.SelectedItem = this.OlapDataManager.ReportList[newName];
        }

        /// <summary>
        /// Updates the report buttons.
        /// </summary>
        private void UpdateReportButtons()
        {
            if (this.ReportList != null && this.btnRemoveReport != null)
            {
                if (this.ReportList.Items.Count > 1)
                {
                    this.btnRemoveReport.IsEnabled = true;
                }
                else if (this.ReportList.Items.Count < 2)
                {
                    this.btnRemoveReport.IsEnabled = false;
                }
            }
        }

        #endregion

        #region Full screen event handlers

        /// <summary>
        /// Handles the Click event of the FullScreen button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            RowDefinition row;
            if (this.btnFullScreen.IsChecked)
            {
                if (this.OlapClientToolBar.Visibility != System.Windows.Visibility.Collapsed)
                {
                    this.OlapClientToolBar.Visibility = System.Windows.Visibility.Collapsed;
                }
                this.btnFullScreen.Content = _normal;
            }
            else
            {
                if (this.ShowToolBarsInOlapClient)
                {
                    if (this.OlapClientToolBar.Visibility != System.Windows.Visibility.Visible)
                    {
                        this.OlapClientToolBar.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                    this.btnFullScreen.Content = _fullScreen;
            }

            ColumnDefinition column;
            column = this.GetTemplateChild("PART_CubeSelectorSpace") as ColumnDefinition;
            column.Width = column.Width.Equals(_cubeAreaLength) ? new GridLength(0, _cubeAreaLength.GridUnitType) : _cubeAreaLength;

            row = this.GetTemplateChild("PART_AxisElementsSpace") as RowDefinition;
            row.Height = row.Height.Equals(_axisAreaLength) ? new GridLength(0, _axisAreaLength.GridUnitType) : _axisAreaLength;
        }

        /// <summary>
        /// Handles the MouseEvent event of the FullScreen button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void FullScreenButton_MouseEvent(object sender, MouseEventArgs e)
        {
            GridLength temp1 = (this.GetTemplateChild("PART_CubeSelectorSpace") as ColumnDefinition).Width;
            GridLength temp2 = (this.GetTemplateChild("PART_AxisElementsSpace") as RowDefinition).Height;

            if (!temp1.Equals(new GridLength(0, temp1.GridUnitType)))
            {
                this._cubeAreaLength = temp1;
            }
            if (!temp2.Equals(new GridLength(0, temp2.GridUnitType)))
            {
                this._axisAreaLength = temp2;
            }

            //OlapToolBarButton button = (sender as OlapToolBarButton);
            //button.Opacity = button.Opacity > 0 && !button.IsChecked ? 0 : 1;
        }

        #endregion

        #region Cubes change event handlers

        /// <summary>
        /// Handles the CubeInfo collection changed event.
        /// </summary>
        void CubeInfoCollectionChanged(object sender, CubeInfoCollectionChangedEventArgs e)
        {
            //foreach (CubeInfo cube in e.NewCubes)
            //{
            //    this.CubeSelector.Items.Add(cube);
            //}
            this.CubeSelector.ItemsSource = e.NewCubes;
            if (this.OlapDataManager.CurrentReport != null && !string.IsNullOrEmpty(this.OlapDataManager.CurrentReport.Name) && !string.IsNullOrEmpty(this.OlapDataManager.CurrentReport.CurrentCubeName))
            {
                if (!this.OlapDataManager.ReportList.Contains(this.OlapDataManager.CurrentReport))
                    this.OlapDataManager.ReportList.Add(this.OlapDataManager.CurrentReport);
                int currentReportIndex = this.OlapDataManager.ReportList.IndexOf(this.OlapDataManager.CurrentReport);
                this.ReportList.SelectedIndex = currentReportIndex > -1 ? currentReportIndex : 0;
            }
            else if (this.OlapDataManager.ReportList != null && this.OlapDataManager.ReportList.Count > 0)
            {
                this.ReportList.SelectedIndex = this.ReportList.Items.Count - 1;
            }
            else
            {
                OlapReport report = new OlapReport("Default Report");
                report.CurrentCubeName = this.OlapDataManager.Cubes[0].Name;
                report.DrillType = this.OlapDataManager.CurrentReport.DrillType;
                report.EnablePaging = this.EnablePaging;
                report.UseDefaultMember = this.UseDefaultMember;
                report.VisualTotalVisibility = this.VisualTotalVisibility;
                this.OlapDataManager.ReportList.Add(report);
                this.ReportList.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the CubeSelector control.
        /// </summary>
        /// <param name="sender">The source of the event (CubeSelector).</param>
        void CubeSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox cubeSelector = (sender as ComboBox);
            if (cubeSelector != null && cubeSelector.SelectedItem != null)
            {
                string selectCubeName = (cubeSelector.SelectedItem as CubeInfo).Name;
                if (string.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName) || this.OlapDataManager.CurrentReport != null && !this.OlapDataManager.CurrentCubeName.ToUpper().Equals(selectCubeName.ToUpper()))
                {
                    this.OlapDataManager.CurrentCubeName = selectCubeName;
                }
            }
            else if(cubeSelector != null && !string.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName) && cubeSelector.SelectedItem == null)
            {

                var r = (from cube in this.OlapDataManager.Cubes where cube.Name.ToUpper().Equals(this.OlapDataManager.CurrentCubeName.ToUpper()) select cube).SingleOrDefault();
                this.CubeSelector.SelectedItem = r as CubeInfo;
            }
        }

        /// <summary>
        /// Handles the CubeChanged event of the OlapDataManager control.
        /// </summary>
        /// <param name="sender">The source of the event(OlapDataManager).</param>
        void CubeChanged(object sender, CubeChangedEventArgs e)
        {
            try
            {
                this.chartOrGridUpdated = this.cubedimbrowserUpdated = false;
                this.OlapDataManager.GetCubeSchema(e.NewCubeName);
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_Errors_GettingCubes"), DialogIcon.Error, DialogButton.OK, null, AnimationType.Zoom);
            }
        }

        #endregion

        #region Unreachable code

        void UnreachableCode()
        {
            Syncfusion.Windows.Controls.Cells.CoveredCellInfo Cvinfo = new Windows.Controls.Cells.CoveredCellInfo();
        }

        #endregion

        #endregion

        #region DragDrop Handler

        /// <summary>
        /// Mouse move event handler for drag and drop support
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.DragDropManager != null)
            {
                Point point = e.GetPosition(null);
                this.DragDropManager.DragDropPopup.Child.Visibility = System.Windows.Visibility.Visible;
                this.DragDropManager.DragDropPopup.HorizontalOffset = point.X - (this.DragDropManager.DragDropPopup.ActualWidth + 5);
                this.DragDropManager.DragDropPopup.VerticalOffset = point.Y + 15;
            }
        }

        /// <summary>
        /// Mouse left button up event handler for drag and drop support
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (this.DragDropManager != null)
            {
                if (DragDropManager.Source is AxisElementBuilder)
                {
                    (DragDropManager.Source as AxisElementBuilder).ReAarrangeElemets(DragDropManager.SelectedNode);
                }
                this.DragDropManager.DragDropPopup.IsOpen = false;
                this.DragDropManager = null;
            }
        }

        #endregion

       
    }

    #region Display Mode
    /// <summary>
    /// DisplayModes of OlapClient
    /// </summary>
    public enum DisplayModes
    {
        /// <summary>
        /// Diplay client, only with OlapChart control
        /// </summary>
        ChartOnly,

        /// <summary>
        /// Display client, only with OlapGrid control
        /// </summary>
        GridOnly,

        /// <summary>
        /// Display client with both OlapChart and OlapGrid
        /// </summary>
        Both
    }

    #endregion

    #region AxesOrder
    /// <summary>
    /// The axes order for <see cref="OlapClient"/> control.
    /// </summary>
    public enum AxesOrder
    {
        /// <summary>
        /// The axes can be displayed in the order of Column, Row and Filter. Default order.
        /// </summary>
        CRF,
        /// <summary>
        /// The axes can be displayed in the order of Column, Filter and Row.
        /// </summary>
        CFR,
        /// <summary>
        /// The axes can be displayed in the order of Filter, Row and Column.
        /// </summary>
        FRC,
        /// <summary>
        /// The axes can be displayed in the order of Filter, Column and Row.
        /// </summary>
        FCR,
        /// <summary>
        /// The axes can be displayed in the order of Row, Column and Filter.
        /// </summary>
        RCF,
        /// <summary>
        /// The axes can be displayed in the order of Row, Filter and Column.
        /// </summary>
        RFC
    }
    #endregion

    #region OlapClientVisualStyle Enumeration
    /// <summary>
    /// Specifies the VisualStyle for OlapClient
    /// </summary>
    public enum OlapClientVisualStyle
    {
        /// <summary>
        /// Provides Default Style for OlapClient
        /// </summary>
        Default,
        /// <summary>
        /// Provides Blend Style for OlapClient
        /// </summary>
        Blend,
        /// <summary>
        /// Provides Office2007Blue Style for OlapClient
        /// </summary>
        Office2007Blue,
        /// <summary>
        /// Provides Office2007Black Style for OlapClient
        /// </summary>
        Office2007Black,
        /// <summary>
        /// Provides Office2007Silver Style for OlapClient
        /// </summary>
        Office2007Silver,
        /// <summary>
        /// Provides Office2010Blue Style for OlapClient
        /// </summary>
        Office2010Blue,
        /// <summary>
        /// Provides Office2010Black Style for OlapClient
        /// </summary>
        Office2010Black,
        /// <summary>
        /// Provides Office2010Silver Style for OlapClient
        /// </summary>
        Office2010Silver,
        /// <summary>
        /// Provides Metro Style for OlapClient
        /// </summary>
        Metro,
        /// <summary>
        /// Provides Transparent style for OlapClient
        /// </summary>
        Transparent,
    }
    #endregion
}