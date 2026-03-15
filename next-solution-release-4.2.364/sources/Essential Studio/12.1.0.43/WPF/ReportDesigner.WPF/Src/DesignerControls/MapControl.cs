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
using Syncfusion.Windows.Gauge;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Reports.Common;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;
#if !SyncfusionFramework3_5
using Syncfusion.UI.Xaml.Maps;
#endif

namespace Syncfusion.Windows.Reports.Designer.Controls
{
    internal enum MapChild
    {
        Map,
        Layer
    }

#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
#if !SyncfusionFramework3_5
    [TemplatePart(Name = "PART_InternalMap", Type = typeof(SfMap))]
#endif
    internal class MapControl : Control, IReportItemControl, INotifyPropertyChanged
    {

        internal Syncfusion.Windows.Reports.Designer.Editors.MapProperties MapProperties;
        //private bool isInternalResize = false;
        private bool isFocusedItem;
        private bool isItemSelected;
        private object propertyOldValue = null;
        internal ReportingConvertorUtil propertyValueConvertor;

        #region Public Properties

        public RDL.DOM.MapSpatialDataRegion MapDataRegion { get; set; }

        public RDL.DOM.MapSpatialDataSet MapDataSet { get; set; }

        public RDL.DOM.MapColorRule ColorRule { get; set; }

        public RDL.DOM.MapMarkerRule MapMarkerRule { get; set; }

        public RDL.DOM.MapPolygonLayer MapPolygonLayer { get; set; }

        public RDL.DOM.MapShapefile MapShapefile { get; set; }

        public MenuItem AddLayer { get; set; }
        
#if !SyncfusionFramework3_5
        internal SfMap InternalMap { get; set; }
#endif
        internal RDL.DOM.Action Action { get; set; }

        internal Syncfusion.RDL.DOM.ReportDefinition Report { get; set; }

        internal MenuItem Properties { get; set; }

        internal MenuItem Delete { get; set; }

        internal RDL.DOM.MapViewport ViewPort { get; set; }

        internal RDL.DOM.MapDataRegions DataRegions { get; set; }

        public RDL.DOM.MapLayers MapLayers { get; set; }

        public RDL.DOM.MapLegends MapLegends { get; set; }

        public RDL.DOM.MapTitles MapTitles { get; set; }

        public RDL.DOM.MapDistanceScale DistanceScale { get; set; }

        public RDL.DOM.MapColorScale ColorScale { get; set; }

        public RDL.DOM.MapBorderSkin BorderSkin { get; set; }

        public DesignerDashStyleBorder MapBorder { get; set; }

        internal Syncfusion.RDL.DOM.DataSets DataSets { get; set; }

        internal Syncfusion.RDL.DOM.DataSources DataSources { get; set; }

        #endregion

        #region constructor
        static MapControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MapControl), new FrameworkPropertyMetadata(typeof(MapControl)));
        }

        internal MapControl(Syncfusion.RDL.DOM.DataSets DataSets, Syncfusion.RDL.DOM.DataSources DataSources)
        {
            this.MapProperties = new Editors.MapProperties();
            this.propertyValueConvertor = new ReportingConvertorUtil();
            this.MapProperties.PropertyChanged += new PropertyChangedEventHandler(Properties_PropertyChanged);
            this.MapProperties.PropertyChanging += new PropertyChangingEventHandler(Properties_PropertyChanging);
            this.PropertyChanged += new PropertyChangedEventHandler(MapControl_PropertyChanged);
            this.DataSets = DataSets;
            this.DataSources = DataSources;
        }


        #endregion

        void MapControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
        }
        void Properties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            #if !SyncfusionFramework3_5

            switch (propertyName)
            {
                case "BACKGROUNDCOLOR":
                    {
                        propertyValue = this.MapProperties.BackgroundColor;
                        this.InternalMap.Background = this.propertyValueConvertor.GetBackGroundColor(this.MapProperties.BackgroundColor);
                        foreach (var layer in this.InternalMap.Layers)
                        {
                            layer.Background = this.propertyValueConvertor.GetBackGroundColor(this.MapProperties.BackgroundColor);
                        }
                        break;
                    }
                case "BORDERCOLOR":
                    {
                        propertyValue = this.MapProperties.BorderColor;
                        this.InternalMap.BorderBrush = this.propertyValueConvertor.GetColor(this.MapProperties.BorderColor);

                        break;
                    }
                //case "BORDERSTYLE":
                //    {
                //        propertyValue = this.MapProperties.BorderStyle;
                //        this.InternalMap.Style. = this.propertyValueConvertor.GetBorderStyle(this.Properties.BorderStyle);
                //        break;
                //    }
                case "BORDERWIDTH":
                    {
                        propertyValue = this.MapProperties.BorderWidth;
                        this.InternalMap.BorderThickness = this.propertyValueConvertor.GetBorderThickness(this.MapProperties.BorderWidth);

                        break;
                    }
                case "NAME":
                    {
                        propertyValue = this.MapProperties.Name;
                        this.ItemName = this.MapProperties.Name;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.MapProperties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.MapProperties.ToggleItem;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.MapProperties.DocumentMapLabel;
                        break;
                    }
                case "PAGEBREAK":
                    {
                        propertyValue = this.MapProperties.BreakLocation;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.MapProperties.Left;
                        if (!this.MapProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.MapProperties.Left))
                            this.ItemLeft = new RDL.DOM.Size(this.MapProperties.Left).PixelValue;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.MapProperties.Top;
                        if (!this.MapProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.MapProperties.Top))
                            this.ItemTop = new RDL.DOM.Size(this.MapProperties.Top).PixelValue;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.MapProperties.Height;
                        if (!this.MapProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.MapProperties.Height))
                            this.ItemHeight = new RDL.DOM.Size(this.MapProperties.Height).PixelValue;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.MapProperties.Width;
                        if (!this.MapProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.MapProperties.Width))
                            this.ItemWidth = new RDL.DOM.Size(this.MapProperties.Width).PixelValue;
                        break;
                    }
                case "DATAELEMENTNAME":
                    {
                        propertyValue = this.MapProperties.DataElementName;
                        break;
                    }
                case "DATAELEMENTOUTPUT":
                    {
                        propertyValue = this.MapProperties.DataElementOutput;
                        break;
                    }
            }
#endif

            if (!this.MapProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.Properties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
                if (propertyName == "TOP" || propertyName == "LEFT" || propertyName == "HEIGHT" || propertyName == "WIDTH")
                {
                    this.Panel.EditingManager.IsMergeAction = true;
                    this.RaiseReportItemSizeChangedEvent();
                    this.Panel.EditingManager.IsMergeAction = false;
                }
            }

        }
        void Properties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();
            switch (propertyName)
            {
                case "BACKGROUNDCOLOR":
                    {
                        propertyValue = this.MapProperties.BackgroundColor;
                        break;
                    }
                case "BORDERCOLOR":
                    {
                        propertyValue = this.MapProperties.BorderColor;
                        break;
                    }
                case "BORDERSTYLE":
                    {
                        propertyValue = this.MapProperties.BorderStyle;
                        break;
                    }
                case "BORDERWIDTH":
                    {
                        propertyValue = this.MapProperties.BorderWidth;
                        break;
                    }
                case "NAME":
                    {
                        propertyValue = this.MapProperties.Name;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.MapProperties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.MapProperties.ToggleItem;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.MapProperties.DocumentMapLabel;
                        break;
                    }
                case "PAGEBREAK":
                    {
                        propertyValue = this.MapProperties.BreakLocation;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.MapProperties.Left;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.MapProperties.Top;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.MapProperties.Height;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.MapProperties.Width;
                        break;
                    }
                case "DATAELEMENTNAME":
                    {
                        propertyValue = this.MapProperties.DataElementName;
                        break;
                    }
                case "DATAELEMENTOUTPUT":
                    {
                        propertyValue = this.MapProperties.DataElementOutput;
                        break;
                    }
            }
            this.propertyOldValue = propertyValue;
        }

        #region ReportItemControl Interface

        public DesignPanel Panel
        {
            get;
            set;
        }

        public Syncfusion.RDL.DOM.ReportItem ReportItem
        {
            get;
            set;
        }

        private string DataSetName { get; set; }

        public string ItemName
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        }

        public double ItemHeight
        {
            get
            {
                return this.ActualHeight;
            }
            set
            {
                if (this.ItemHeight != value)
                {
                    this.MapProperties.IsInternalPropertyChange = true;
                    this.MapProperties.Height = propertyValueConvertor.GetSizeValue(value, this.MapProperties.Height);
                    this.MapProperties.IsInternalPropertyChange = false;
                }
                this.Height = value;
            }
        }

        public double ItemWidth
        {
            get
            {
                return this.ActualWidth;
            }
            set
            {
                if (this.ItemWidth != value)
                {
                    this.MapProperties.IsInternalPropertyChange = true;
                    this.MapProperties.Width = propertyValueConvertor.GetSizeValue(value, this.MapProperties.Width);
                    this.MapProperties.IsInternalPropertyChange = false;
                }
                this.Width = value;
            }
        }

        public double ItemTop
        {
            get
            {
                return Canvas.GetTop(this);
            }
            set
            {
                if (this.ItemTop != value)
                {
                    this.MapProperties.IsInternalPropertyChange = true;
                    this.MapProperties.Top = propertyValueConvertor.GetSizeValue(value, this.MapProperties.Top);
                    this.MapProperties.IsInternalPropertyChange = false;
                }
                Canvas.SetTop(this, value);
            }
        }

        public double ItemLeft
        {
            get
            {
                return Canvas.GetLeft(this);
            }
            set
            {
                if (this.ItemLeft != value)
                {
                    this.MapProperties.IsInternalPropertyChange = true;
                    this.MapProperties.Left = propertyValueConvertor.GetSizeValue(value, this.MapProperties.Left);
                    this.MapProperties.IsInternalPropertyChange = false;
                }
                Canvas.SetLeft(this, value);
            }
        }

        public bool IsTablixItem
        {
            get;
            set;
        }

        public bool IsItemSelected
        {
            get
            {
                return this.isItemSelected;
            }
            set
            {
                if (this.isItemSelected != value)
                {
                    isItemSelected = value;
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.MapProperties, IsSelected = value });
                    OnPropertyChanged("IsItemSelected");
                }
            }
        }

        public bool IsFocusedItem
        {
            get
            {
                return isFocusedItem;
            }
            set
            {
                if (isFocusedItem != value)
                {
                    this.isFocusedItem = value;
                    OnPropertyChanged("IsFocusedItem");
                }
            }
        }

        public new Canvas Parent
        {
            get;
            set;
        }

        public DrawingReportItem ItemType
        {
            get
            {
                return DrawingReportItem.Map;
            }
        }

        public event ReportItemControlSizeHandler ReportItemSizeChanged;


        public void RaiseReportItemSizeChangedEvent()
        {
            if (this.ReportItemSizeChanged != null)
            {
                this.ReportItemSizeChanged(this, new EventArgs());
#if !SyncfusionFramework3_5
                this.InternalMap.Height = ActualHeight;
                this.InternalMap.Width = ActualWidth;
                foreach (var layer in this.InternalMap.Layers)
                {
                    if (ActualHeight > 10)
                        layer.Height = ActualHeight - 10;
                    if (ActualWidth > 20)
                        layer.Width = ActualWidth - 20;
                }
#endif
            }
        }

        public event ReportItemSelectedEvent ReportItemSelected;

        public void RaiseReportItemSelectedEvent(SelectedItemEventArgs selectedObjectArg)
        {
            if (this.ReportItemSelected != null)
            {
                this.ReportItemSelected(this, selectedObjectArg);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public ImageSource GetImageSource()
        {
            System.Windows.Media.Imaging.RenderTargetBitmap rtb = new System.Windows.Media.Imaging.RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush();
                vb.AutoLayoutContent = true;
                vb.Visual = this;
                ctx.DrawRectangle(vb, null, new Rect(new Point(0, 0), new System.Windows.Size(this.ActualWidth, this.ActualHeight)));
            }

            rtb.Render(dv);
            return rtb;
        }

        public Syncfusion.RDL.DOM.ReportItem GetReportItem()
        {
            Syncfusion.RDL.DOM.Map Map = new Syncfusion.RDL.DOM.Map();


            if (this.ToolTip != null)
            {
                Map.ToolTip = this.MapProperties.ToolTip;
            }
            if (!string.IsNullOrEmpty(this.MapProperties.DocumentMapLabel))
            {
                Map.DocumentMapLabel = this.MapProperties.DocumentMapLabel;
            }

            Map.Name = this.MapProperties.Name;
            Map.Visibility = new RDL.DOM.Visibility();

            if (this.MapProperties.ToggleItem != null)
            {
                Map.Visibility.ToggleItem = this.MapProperties.ToggleItem;
            }
            if (this.MapProperties.Hidden != null && (this.MapProperties.Hidden == "True" || this.MapProperties.Hidden.StartsWith("=")))
            {
                Map.Visibility.Hidden = this.MapProperties.Hidden;
            }
            if (!string.IsNullOrEmpty(this.MapProperties.DocumentMapLabel))
            {
                Map.DocumentMapLabel = this.MapProperties.DocumentMapLabel;
            }

            if (!string.IsNullOrEmpty(this.MapProperties.BreakLocation))
            {
                Map.PageBreak = new RDL.DOM.PageBreak();
                Map.PageBreak.BreakLocation = (RDL.DOM.BreakLocation)Enum.Parse(typeof(RDL.DOM.BreakLocation), this.MapProperties.BreakLocation);
            }
            if (!string.IsNullOrEmpty(this.MapProperties.DataElementName))
            {
                Map.DataElementName = this.MapProperties.DataElementName;
            }
            if (this.MapProperties.DataElementOutput != null && this.MapProperties.DataElementOutput != "Auto")
            {
                Map.DataElementOutput = (RDL.DOM.DataElementOutputs)Enum.Parse(typeof(RDL.DOM.DataElementOutputs), this.MapProperties.DataElementOutput);
            }
            Map.Style = new RDL.DOM.Style();
            Map.Style.BackgroundColor = this.MapProperties.BackgroundColor;
            Map.Height = new RDL.DOM.Size(Convert.ToDouble(this.ItemHeight) / 96 + DesignPanel.GetMeasuredUnit(this.MapProperties.Height));
            Map.Width = new RDL.DOM.Size(Convert.ToDouble(this.ItemWidth) / 96 + DesignPanel.GetMeasuredUnit(this.MapProperties.Width));

            Map.Left = new RDL.DOM.Size(this.ItemLeft / 96 + DesignPanel.GetMeasuredUnit(this.MapProperties.Left));
            Map.Top = new RDL.DOM.Size(this.ItemTop / 96 + DesignPanel.GetMeasuredUnit(this.MapProperties.Top));
            this.UpdateMapObj(Map);
            return Map;
        }

        public void RestoreReportItem(RDL.DOM.ReportItem reportItem)
        {
            this.MapProperties.IsInternalPropertyChange = true;
            this.ReportItem = reportItem;

            if (this.ReportItem != null)
            {
                this.PopulateReportItem();
                this.ReportItem = null;
            }

            this.MapProperties.IsInternalPropertyChange = false;
        }

        public void UpdateItemSizeProperties()
        {
            this.MapProperties.IsInternalPropertyChange = true;
            this.MapProperties.Width = propertyValueConvertor.GetSizeValue(this.ItemWidth, this.MapProperties.Width);
            this.MapProperties.Top = propertyValueConvertor.GetSizeValue(this.ItemTop, this.MapProperties.Top);
            this.MapProperties.Left = propertyValueConvertor.GetSizeValue(this.ItemLeft, this.MapProperties.Left);
            this.MapProperties.Height = propertyValueConvertor.GetSizeValue(this.ItemHeight, this.MapProperties.Height);
            this.MapProperties.IsInternalPropertyChange = false;
        }

        #endregion
        public TextBlock MapTitle { get; set; }

        public override void OnApplyTemplate()
        {
            try
            {
                this.MapBorder = GetTemplateChild("PART_MapBorder") as DesignerDashStyleBorder;

                Grid internalMapGrid = GetTemplateChild("PART_InternalMapGrid") as Grid;

#if !SyncfusionFramework3_5
                this.InternalMap = new SfMap();
                internalMapGrid.Children.Add(this.InternalMap);
#endif
                this.Delete = GetTemplateChild("PART_DataMenuDelete") as MenuItem;
                this.Delete.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDelete");
                this.Properties = GetTemplateChild("PART_DataMenuProperty") as MenuItem;
                this.Properties.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerMapProperties");
                this.MapTitle = GetTemplateChild("PART_DataMapTitle") as TextBlock;
                this.AddLayer = GetTemplateChild("PART_DataAddLayer") as MenuItem;
                this.AddLayer.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddLayer");
                this.AddLayer.Click += new RoutedEventHandler(AddLayer_Click);
                this.Delete.Click += new RoutedEventHandler(DeleteMap_Click);
                this.Properties.Click += new RoutedEventHandler(Properties_Click);

                this.InitializeMap();

                base.OnApplyTemplate();

                this.MapProperties.IsInternalPropertyChange = true;
                this.MapProperties.Name = this.ItemName;
                this.MapProperties.UpdatePropertyValue();

                if (this.ReportItem != null)
                {
                    this.PopulateReportItem();
                }

                this.MapProperties.IsInternalPropertyChange = false;
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message.ToString());
            }

        }


        void DeleteMap_Click(object sender, RoutedEventArgs e)
        {
            if (this.Delete != null)
            {
                this.Panel.DeleteSelectedReportItems();
            }
        }

        internal ControlProperties UpdatePropertyDialog()
        {
            ControlProperties controlProperties = new ControlProperties(this,MapChild.Map);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.MapGeneral.txt_GeneralName.Text = this.MapProperties.Name;
            if (this.MapProperties.BackgroundColor != null)
                controlProperties.MapFill.clrpkr_Fill.Text = this.MapProperties.BackgroundColor;

            return controlProperties;
        }

        void Properties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdatePropertyDialog();

            if (controlProperties.ShowDialog() == true)
            {
                this.MapProperties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);
                change.NewValue = this.GetReportItem();
                this.MapProperties.IsInternalPropertyChange = false;

                foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is MapGeneral)
                    {
                        this.MapProperties.Name = controlProperties.MapGeneral.txt_GeneralName.Text;
                    }
                    else if (uiElement is MapFill)
                    {
                        this.MapProperties.BackgroundColor = controlProperties.MapFill.clrpkr_Fill.Text;
                    }
                }
            }

        }


        void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateLayerProperties();
            if (controlProperties.ShowDialog() == true)
            {
                this.MapProperties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);
                foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is LayerGeneral)
                    {
                        this.AddLayerValue(controlProperties.LayerGeneral.txt_ShapeFile.Text);
                    }
                }

                change.NewValue = this.GetReportItem();
                this.MapProperties.IsInternalPropertyChange = false;
            }
        }

        private void AddLayerValue(string LayerUri)
        {
            RDL.DOM.MapPolygonLayer polygonLayer = new RDL.DOM.MapPolygonLayer();
            RDL.DOM.MapShapefile shapefile = new RDL.DOM.MapShapefile();
            shapefile.Source = LayerUri;
            polygonLayer.MapSpatialData = shapefile as RDL.DOM.MapShapefile;
            this.MapLayers = new RDL.DOM.MapLayers();
            this.MapLayers.Add(polygonLayer);

#if !SyncfusionFramework3_5
            ShapeFileLayer filelayer = new ShapeFileLayer();
            filelayer.Uri = new Uri(LayerUri, UriKind.RelativeOrAbsolute).AbsolutePath;
            filelayer.Background = this.InternalMap.Background;
            filelayer.ShapeSettings.ShapeFill = Brushes.White;
            //ShapeSetting shapeSetting = new ShapeSetting();
            //shapeSetting.FillSetting.AutoFillColors = true;
            //filelayer.ShapeSettings = shapeSetting;
            this.InternalMap.Layers.Add(filelayer);
#endif
        }

        private ControlProperties UpdateLayerProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this, MapChild.Layer);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.LayerGeneral.txt_ShapeFile.Text = string.Empty;
            return controlProperties;
        }
        private void InitializeMap()
        {
#if !SyncfusionFramework3_5
            InternalMap.Background =SetBackground("Silver","White");
            InternalMap.Width = this.Width;
            InternalMap.Height = this.Height;
            InternalMap.BorderThickness = new Thickness(1);
            InternalMap.BorderBrush = Brushes.Black;
            ShapeFileLayer filelayer = new ShapeFileLayer();
            filelayer.Background = SetBackground("Silver", "White");
            filelayer.ShapeSettings = new ShapeSetting();
            filelayer.ShapeSettings.ShapeFill = Brushes.White;
            filelayer.ShapeSettings.FillSetting.AutoFillColors = true;
            InternalMap.Layers.Add(filelayer);
            InternalMap.EnablePan = false;
            InternalMap.EnableZoom = false;
#endif

            this.MapTitle.Height = 20;
            this.MapTitle.Width = this.Width;
        }

        private LinearGradientBrush SetBackground(string color1, string color2)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(0, 1);
            GradientStop gra1 = new GradientStop();
            gra1.Color = (Color)ColorConverter.ConvertFromString(color1);
            gra1.Offset = 1;
            GradientStop gra2 = new GradientStop();
            gra2.Color = (Color)ColorConverter.ConvertFromString(color2);
            gra2.Offset = 0;
            brush.GradientStops.Add(gra1);
            brush.GradientStops.Add(gra2);
            return brush;
        }

        #region Deserialization

        public void PopulateReportItem()
        {
            try
            {
                Syncfusion.RDL.DOM.Map MapBase = this.ReportItem as RDL.DOM.Map;

                this.MapProperties.Name = MapBase.Name;
                if (MapBase.Height != null)
                {
                    this.MapProperties.Height = MapBase.Height.size;
                }
                if (MapBase.Width != null)
                {
                    this.MapProperties.Width = MapBase.Width.size;
                }
                if (MapBase.Top != null)
                {
                    this.MapProperties.Top = MapBase.Top.size;
                }
                if (MapBase.Left != null)
                {
                    this.MapProperties.Left = MapBase.Left.size;
                }

                this.MapProperties.DocumentMapLabel = MapBase.DocumentMapLabel;
                this.MapProperties.DataElementName = MapBase.DataElementName;

                if (MapBase.Visibility != null)
                {
                    if (MapBase.Visibility.ToggleItem != null)
                        this.MapProperties.ToggleItem = MapBase.Visibility.ToggleItem;
                    if (MapBase.Visibility.Hidden != null)
                        this.MapProperties.Hidden = MapBase.Visibility.Hidden;
                }
                if (MapBase.PageBreak != null)
                {
                    this.MapProperties.BreakLocation = MapBase.PageBreak.BreakLocation.ToString();
                }
                if (MapBase.DataElementOutput != RDL.DOM.DataElementOutputs.Auto)
                {
                    this.MapProperties.DataElementOutput = MapBase.DataElementOutput.ToString();
                }
                if (MapBase.Style != null)
                {
                    if (MapBase.Style.Border != null)
                    {
                        if (MapBase.Style.Border.Style != null)
                        {
                            this.MapProperties.BorderStyle = MapBase.Style.Border.Style;
                        }
                        if (MapBase.Style.Border.Color != null)
                        {
                            this.MapProperties.BorderColor = MapBase.Style.Border.Color;
                        }
                        if (MapBase.Style.Border.Width != null)
                        {
                            this.MapProperties.BorderWidth = MapBase.Style.Border.Width.PixelValue + "pt";
                        }
                    }
                    this.MapProperties.BackgroundColor = MapBase.Style.BackgroundColor;
                    this.MapProperties.MaximumSpatialElementCount = MapBase.MaximumSpatialElementCount.ToString();
                    this.MapProperties.MaximumTotalPointCount = MapBase.MaximumTotalPointCount.ToString();
                    this.MapProperties.TileLanguage = MapBase.TileLanguage;
                    this.MapProperties.AntiAliasing = MapBase.AntiAliasing.ToString();
                    this.MapProperties.TextAntiAliasingQuality = MapBase.TextAntiAliasingQuality.ToString();

                    if (MapBase.DataSetName != null)
                    {
                        this.DataSetName = MapBase.DataSetName;
                    }

                }

                #region viewport
                if (MapBase.MapViewport != null)
                {
                    this.ViewPort.MapCoordinateSystem = MapBase.MapViewport.MapCoordinateSystem;
                    this.ViewPort.MapProjection = MapBase.MapViewport.MapProjection;
                    this.ViewPort.ProjectionCenterX = MapBase.MapViewport.ProjectionCenterX;
                    this.ViewPort.ProjectionCenterY = MapBase.MapViewport.ProjectionCenterY;

                    this.ViewPort.MapLimits = new RDL.DOM.MapLimits();
                    this.ViewPort.MapLimits.MaximumX = MapBase.MapViewport.MapLimits.MaximumX;
                    this.ViewPort.MapLimits.MaximumY = MapBase.MapViewport.MapLimits.MaximumY;
                    this.ViewPort.MapLimits.MinimumX = MapBase.MapViewport.MapLimits.MinimumX;
                    this.ViewPort.MapLimits.MinimumY = MapBase.MapViewport.MapLimits.MinimumY;
                    this.ViewPort.MaximumZoom = MapBase.MapViewport.MaximumZoom;

                    //this.ViewPort.MapView = new RDL.DOM.MapView();
                    this.ViewPort.MapView.Zoom = MapBase.MapViewport.MapView.Zoom;

                    this.ViewPort.MapMeridians = new RDL.DOM.MapMeridians();
                    this.ViewPort.MapMeridians.Style = new RDL.DOM.Style();
                    this.ViewPort.MapMeridians.Style.Border = new RDL.DOM.Border();
                    this.ViewPort.MapMeridians.Style.Border.Color = MapBase.MapViewport.MapMeridians.Style.Border.Color;
                    this.ViewPort.MapMeridians.Style.Border.Style = MapBase.MapViewport.MapMeridians.Style.Border.Style;
                    this.ViewPort.MapMeridians.Style.Border.Width = MapBase.MapViewport.MapMeridians.Style.Border.Width;
                    this.ViewPort.MapMeridians.Style.FontSize = MapBase.MapViewport.MapMeridians.Style.FontSize;
                    this.ViewPort.MapMeridians.Style.Format = MapBase.MapViewport.MapMeridians.Style.Format;
                    this.ViewPort.MapMeridians.Style.Color = MapBase.MapViewport.MapMeridians.Style.Color;
                    this.ViewPort.MapMeridians.Hidden = MapBase.MapViewport.MapMeridians.Hidden;
                    this.ViewPort.MapMeridians.Interval = MapBase.MapViewport.MapMeridians.Interval;
                    this.ViewPort.MapMeridians.LabelPosition = MapBase.MapViewport.MapMeridians.LabelPosition;
                    this.ViewPort.MapMeridians.ShowLabels = MapBase.MapViewport.MapMeridians.ShowLabels;

                    this.ViewPort.MapParallels = new RDL.DOM.MapParallels();
                    this.ViewPort.MapParallels.Style = new RDL.DOM.Style();
                    this.ViewPort.MapParallels.Style.Border = new RDL.DOM.Border();
                    this.ViewPort.MapParallels.Style.Border.Color = MapBase.MapViewport.MapParallels.Style.Border.Color;
                    this.ViewPort.MapParallels.Style.Border.Style = MapBase.MapViewport.MapParallels.Style.Border.Style;
                    this.ViewPort.MapParallels.Style.Border.Width = MapBase.MapViewport.MapParallels.Style.Border.Width;
                    this.ViewPort.MapParallels.Style.FontSize = MapBase.MapViewport.MapParallels.Style.FontSize;
                    this.ViewPort.MapParallels.Style.Format = MapBase.MapViewport.MapParallels.Style.Format;
                    this.ViewPort.MapParallels.Style.Color = MapBase.MapViewport.MapParallels.Style.Color;
                    this.ViewPort.MapParallels.Hidden = MapBase.MapViewport.MapParallels.Hidden;
                    this.ViewPort.MapParallels.Interval = MapBase.MapViewport.MapParallels.Interval;
                    this.ViewPort.MapParallels.LabelPosition = MapBase.MapViewport.MapParallels.LabelPosition;
                    this.ViewPort.MapParallels.ShowLabels = MapBase.MapViewport.MapParallels.ShowLabels;

                    this.ViewPort.GridUnderContent = MapBase.MapViewport.GridUnderContent;
                    this.ViewPort.Style = new RDL.DOM.Style();
                    this.ViewPort.Style.Border = new RDL.DOM.Border();
                    this.ViewPort.Style.Border.Color = MapBase.MapViewport.Style.Border.Color;
                    this.ViewPort.Style.Border.Style = MapBase.MapViewport.Style.Border.Style;
                    this.ViewPort.Style.Border.Width = MapBase.MapViewport.Style.Border.Width;

                    this.ViewPort.LeftMargin = MapBase.MapViewport.LeftMargin;
                    this.ViewPort.RightMargin = MapBase.MapViewport.RightMargin;
                    this.ViewPort.TopMargin = MapBase.MapViewport.TopMargin;
                    this.ViewPort.BottomMargin = MapBase.MapViewport.BottomMargin;
                }

                #endregion

                #region Dataregion
                if (MapBase.MapDataRegions != null)
                {
                    this.DataRegions = new RDL.DOM.MapDataRegions();
                    foreach (RDL.DOM.MapDataRegion MapDataRegion in MapBase.MapDataRegions)
                    {
                        RDL.DOM.MapDataRegion DataRegion = new RDL.DOM.MapDataRegion();
                        DataRegion.Name = MapDataRegion.Name;
                        DataRegion.MapMember = new RDL.DOM.MapMember();
                        DataRegion.MapMember.Group = new RDL.DOM.Group();
                        DataRegion.MapMember.Group.Name = MapDataRegion.MapMember.Group.Name;
                        foreach (RDL.DOM.GroupExpression GroupExp in DataRegion.MapMember.Group.GroupExpressions)
                        {
                            DataRegion.MapMember.Group.GroupExpressions.Add(GroupExp);
                        }
                        this.DataRegions.Add(DataRegion);
                    }
                }

                #endregion

                if (MapBase.MapLayers != null)
                {
                    foreach (var Maplayer in MapBase.MapLayers)
                    {
                        #region polygonlayer
                        if (Maplayer is Syncfusion.RDL.DOM.MapPolygonLayer)
                        {
                            var Polylayer = Maplayer as RDL.DOM.MapPolygonLayer;
                            this.MapPolygonLayer = new RDL.DOM.MapPolygonLayer();
                            try
                            {
                                if (Polylayer.MapSpatialData != null)
                                {
                                    if ((Polylayer.MapSpatialData as RDL.DOM.MapShapefile) != null)
                                    {
                                        var shapefile = (Polylayer.MapSpatialData as RDL.DOM.MapShapefile);

                                        if (shapefile.MapFieldNames != null && shapefile.MapFieldNames.MapFieldName != null && shapefile.MapFieldNames.MapFieldName.Count > 0)
                                        {
                                            this.MapShapefile.MapFieldNames = new RDL.DOM.MapFieldNames();

                                            foreach (var field in shapefile.MapFieldNames.MapFieldName)
                                            {
                                                this.MapShapefile.MapFieldNames.MapFieldName.Add(field);
                                            }
                                        }

                                        this.MapShapefile.Source = shapefile.Source;
                                    }
                                    if ((Polylayer.MapSpatialData as Syncfusion.RDL.DOM.MapSpatialDataRegion) != null)
                                    {
                                        this.MapDataRegion.VectorData = (Polylayer.MapSpatialData as Syncfusion.RDL.DOM.MapSpatialDataRegion).VectorData;
                                    }
                                    if ((Polylayer.MapSpatialData as Syncfusion.RDL.DOM.MapSpatialDataSet) != null)
                                    {
                                        var spatialDataSet = (Polylayer.MapSpatialData as Syncfusion.RDL.DOM.MapSpatialDataSet);


                                        if (spatialDataSet.MapFieldNames != null && spatialDataSet.MapFieldNames.MapFieldName != null && spatialDataSet.MapFieldNames.MapFieldName.Count > 0)
                                        {
                                            this.MapDataSet.MapFieldNames.MapFieldName = new List<string>();
                                            foreach (var field in spatialDataSet.MapFieldNames.MapFieldName)
                                            {
                                                this.MapDataSet.MapFieldNames.MapFieldName.Add(field);
                                            }
                                        }
                                        this.MapDataSet.SpatialField = spatialDataSet.SpatialField;
                                        this.MapDataSet.DataSetName = spatialDataSet.DataSetName;
                                    }
                                }
                                if (Polylayer.MapBindingFieldPairs != null && Polylayer.MapBindingFieldPairs.Count > 0)
                                {
                                    foreach (var field in Polylayer.MapBindingFieldPairs)
                                    {
                                        RDL.DOM.MapBindingFieldPair pair = new RDL.DOM.MapBindingFieldPair();
                                        //pair.BindingExpression = this.Model.ExpressionEngine.GetEvalExpressionString(field.BindingExpression);
                                        //pair.FieldName = this.Model.ExpressionEngine.GetEvalExpressionString(field.FieldName);
                                        pair.BindingExpression = field.BindingExpression;
                                        pair.FieldName = field.FieldName;
                                        this.MapPolygonLayer.MapBindingFieldPairs.Add(pair);
                                    }
                                }
                                if (Polylayer.MapCenterPointRules != null)
                                {
                                    this.MapPolygonLayer.MapCenterPointRules = new RDL.DOM.MapCenterPointRules();
                                    if (Polylayer.MapCenterPointRules.MapColorRule != null)
                                    {


                                        //if (Polylayer.MapCenterPointRules.MapColorRule is Syncfusion.RDL.DOM.MapColorRangeRule)
                                        //{
                                        //    var rangeRule = Polylayer.MapCenterPointRules.MapColorRule  as Syncfusion.RDL.DOM.MapColorRangeRule;
                                        //    if (rangeRule != null)
                                        //    {
                                        //        this.ColorRule.MapColorRangeRule = new MapColorRangeRuleExp();
                                        //        colorRule.MapColorRangeRule.StartColor = this.GetExpressionKey(rangeRule.StartColor);
                                        //        colorRule.MapColorRangeRule.EndColor = this.GetExpressionKey(rangeRule.EndColor);
                                        //        colorRule.MapColorRangeRule.MiddleColor = this.GetExpressionKey(rangeRule.MiddleColor);
                                        //    }
                                        //}
                                        //else if (Polylayer.MapCenterPointRules.MapColorRule is Syncfusion.RDL.DOM.MapColorPaletteRule)
                                        //{
                                        //    var paletteRule = Polylayer.MapCenterPointRules.MapColorRule as Syncfusion.RDL.DOM.MapColorPaletteRule;
                                        //    if (paletteRule != null)
                                        //    {
                                        //        colorRule.ColorPalette = new MapColorPaletteRuleExp();
                                        //        colorRule.ColorPalette.Palette = this.GetExpressionKey(paletteRule.Palette.ToString());
                                        //    }
                                        //}
                                        //else if (Polylayer.MapCenterPointRules.MapColorRule is Syncfusion.RDL.DOM.MapCustomColorRule)
                                        //{
                                        //    var customColor = Polylayer.MapCenterPointRules.MapColorRule as RDL.DOM.MapCustomColorRule;
                                        //    if (customColor != null && customColor.MapCustomColors != null && customColor.MapCustomColors.MapCustomColor != null)
                                        //    {
                                        //        //this.ColorRule.MapCustomColors = new List<string>();
                                        //        foreach (var color in customColor.MapCustomColors.MapCustomColor)
                                        //        {
                                        //            this.ColorRule.MapCustomColors.Add(color);
                                        //        }
                                        //    }
                                        //}

                                        this.ColorRule.BucketCount = Polylayer.MapCenterPointRules.MapColorRule.BucketCount;
                                        this.ColorRule.DataElementName = Polylayer.MapCenterPointRules.MapColorRule.DataElementName;
                                        this.ColorRule.DataElementOutput = Polylayer.MapCenterPointRules.MapColorRule.DataElementOutput;
                                        //colorRule.DataValue = this.GetExpressionKey(mapColorRule.DataValue);
                                        this.ColorRule.DataValue = Polylayer.MapCenterPointRules.MapColorRule.DataValue;
                                        this.ColorRule.DistributionType = Polylayer.MapCenterPointRules.MapColorRule.DistributionType;
                                        this.ColorRule.EndValue = Polylayer.MapCenterPointRules.MapColorRule.EndValue;

                                        this.MapPolygonLayer.MapCenterPointRules.MapColorRule = this.ColorRule;
                                    }
                                    if (Polylayer.MapCenterPointRules.MapMarkerRule != null)
                                    {
                                        this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.BucketCount = Polylayer.MapCenterPointRules.MapMarkerRule.BucketCount;
                                        this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.DataElementName = Polylayer.MapCenterPointRules.MapMarkerRule.DataElementName;
                                        this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.DataElementOutput = Polylayer.MapCenterPointRules.MapMarkerRule.DataElementOutput;
                                        //marker.DataValue = this.GetExpressionKey(mapMarkerRule.DataValue);
                                        this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.DataValue = Polylayer.MapCenterPointRules.MapMarkerRule.DataValue;
                                        this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.DistributionType = Polylayer.MapCenterPointRules.MapMarkerRule.DistributionType;
                                        this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.EndValue = Polylayer.MapCenterPointRules.MapMarkerRule.EndValue;
                                    }
                                    if (Polylayer.MapCenterPointRules.MapSizeRule != null)
                                    {
                                        this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.BucketCount = Polylayer.MapCenterPointRules.MapSizeRule.BucketCount;
                                        this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.DataElementName = Polylayer.MapCenterPointRules.MapSizeRule.DataElementName;
                                        this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.DataElementOutput = Polylayer.MapCenterPointRules.MapSizeRule.DataElementOutput;
                                        //sizeRule.DataValue = this.GetExpressionKey(mapSizeRule.DataValue);
                                        this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.DataValue = Polylayer.MapCenterPointRules.MapSizeRule.DataValue;
                                        this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.DistributionType = Polylayer.MapCenterPointRules.MapSizeRule.DistributionType;
                                        this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.StartSize = Polylayer.MapCenterPointRules.MapSizeRule.StartSize;
                                        this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.EndSize = Polylayer.MapCenterPointRules.MapSizeRule.EndSize;
                                        this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.StartValue = Polylayer.MapCenterPointRules.MapSizeRule.StartValue;
                                        this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.EndValue = Polylayer.MapCenterPointRules.MapSizeRule.EndValue;

                                    }
                                }
                                if (Polylayer.MapCenterPointTemplate != null)
                                {

                                    if (Polylayer.MapCenterPointTemplate.Style != null)
                                    {
                                        //if (Polylayer.MapCenterPointTemplate.Style.Border != null)
                                        //    mapPolygonLayer.MapCenterPointTemplate.Border = GetBorderExp(Polylayer.MapCenterPointTemplate.Style.Border);
                                        //mapPolygonLayer.MapCenterPointTemplate.StyleExp = GetFontStyleExp(Polylayer.MapCenterPointTemplate.Style);
                                    }
                                    if (Polylayer.MapCenterPointTemplate.ActionInfo != null)
                                    {
                                        //mapPolygonLayer.MapCenterPointTemplate.ActionInfo = GetActionInfoExp(Polylayer.MapCenterPointTemplate.ActionInfo);
                                    }
                                    if (Polylayer.MapCenterPointTemplate.Size != null)
                                    {
                                        this.MapPolygonLayer.MapCenterPointTemplate.Size = Polylayer.MapCenterPointTemplate.Size.size;
                                    }

                                    this.MapPolygonLayer.MapCenterPointTemplate.DataElementLabel = Polylayer.MapCenterPointTemplate.DataElementLabel;
                                    this.MapPolygonLayer.MapCenterPointTemplate.DataElementName = Polylayer.MapCenterPointTemplate.DataElementName;
                                    this.MapPolygonLayer.MapCenterPointTemplate.DataElementOutput = Polylayer.MapCenterPointTemplate.DataElementOutput;
                                    this.MapPolygonLayer.MapCenterPointTemplate.Hidden = Polylayer.MapCenterPointTemplate.Hidden;
                                    this.MapPolygonLayer.MapCenterPointTemplate.Label = Polylayer.MapCenterPointTemplate.Label;
                                    this.MapPolygonLayer.MapCenterPointTemplate.LabelPlacement = Polylayer.MapCenterPointTemplate.LabelPlacement;
                                    this.MapPolygonLayer.MapCenterPointTemplate.OffsetX = Polylayer.MapCenterPointTemplate.OffsetX;
                                    this.MapPolygonLayer.MapCenterPointTemplate.OffsetY = Polylayer.MapCenterPointTemplate.OffsetY;
                                    this.MapPolygonLayer.MapCenterPointTemplate.ToolTip = Polylayer.MapCenterPointTemplate.ToolTip;
                                }
                                if (Polylayer.MapPolygonTemplate != null)
                                {

                                    if (Polylayer.MapPolygonTemplate.Style != null)
                                    {
                                        if (Polylayer.MapPolygonTemplate.Style.Border != null)
                                        {
                                            this.MapPolygonLayer.MapPolygonTemplate.Style.Border.Color = Polylayer.MapPolygonTemplate.Style.Border.Color;
                                            this.MapPolygonLayer.MapPolygonTemplate.Style.Border.Style = Polylayer.MapPolygonTemplate.Style.Border.Style;
                                            this.MapPolygonLayer.MapPolygonTemplate.Style.Border.Width = Polylayer.MapPolygonTemplate.Style.Border.Width;
                                        }
                                        this.MapPolygonLayer.MapPolygonTemplate.Style.BackgroundColor = Polylayer.MapPolygonTemplate.Style.BackgroundColor;
                                        this.MapPolygonLayer.MapPolygonTemplate.Style.BackgroundGradientEndColor = Polylayer.MapPolygonTemplate.Style.BackgroundGradientEndColor;
                                        this.MapPolygonLayer.MapPolygonTemplate.Style.BackgroundGradientType = Polylayer.MapPolygonTemplate.Style.BackgroundGradientType;
                                        this.MapPolygonLayer.MapPolygonTemplate.Style.BackgroundHatchType = Polylayer.MapPolygonTemplate.Style.BackgroundHatchType;
                                    }
                                    if (Polylayer.MapPolygonTemplate.ActionInfo != null)
                                    {
                                        //mapPolygonLayer.MapPolygonTemplate.ActionInfo = GetActionInfoExp(Polylayer.MapPolygonTemplate.ActionInfo);
                                    }

                                    this.MapPolygonLayer.MapPolygonTemplate.Label = Polylayer.MapPolygonTemplate.Label;
                                    this.MapPolygonLayer.MapPolygonTemplate.ShowLabel = Polylayer.MapPolygonTemplate.ShowLabel;
                                    this.MapPolygonLayer.MapPolygonTemplate.CenterPointOffsetX = Polylayer.MapPolygonTemplate.CenterPointOffsetX;
                                    this.MapPolygonLayer.MapPolygonTemplate.CenterPointOffsetY = Polylayer.MapPolygonTemplate.CenterPointOffsetY;
                                    this.MapPolygonLayer.MapPolygonTemplate.DataElementLabel = Polylayer.MapPolygonTemplate.DataElementLabel;
                                    this.MapPolygonLayer.MapPolygonTemplate.DataElementName = Polylayer.MapPolygonTemplate.DataElementName;
                                    this.MapPolygonLayer.MapPolygonTemplate.DataElementOutput = Polylayer.MapPolygonTemplate.DataElementOutput;
                                    this.MapPolygonLayer.MapPolygonTemplate.Hidden = Polylayer.MapPolygonTemplate.Hidden;
                                    this.MapPolygonLayer.MapPolygonTemplate.Label = Polylayer.MapPolygonTemplate.Label;
                                    //this.lableFieldName.Add(GetFieldName(mapPolygonTemplate.Label));
                                    this.MapPolygonLayer.MapPolygonTemplate.LabelPlacement = Polylayer.MapPolygonTemplate.LabelPlacement;
                                    this.MapPolygonLayer.MapPolygonTemplate.OffsetX = Polylayer.MapPolygonTemplate.OffsetX;
                                    this.MapPolygonLayer.MapPolygonTemplate.OffsetY = Polylayer.MapPolygonTemplate.OffsetY;
                                    this.MapPolygonLayer.MapPolygonTemplate.ScaleFactor = Polylayer.MapPolygonTemplate.ScaleFactor;
                                    this.MapPolygonLayer.MapPolygonTemplate.ShowLabel = Polylayer.MapPolygonTemplate.ShowLabel;
                                    this.MapPolygonLayer.MapPolygonTemplate.ToolTip = Polylayer.MapPolygonTemplate.ToolTip;
                                }
                                if (Polylayer.MapFieldDefinitions != null)
                                {
                                    //mapPolygonLayer.MapFieldDefinitions = GetMapFieldDefExp(Polylayer.MapFieldDefinitions);
                                }
                                if (Polylayer.MapPolygonRules != null)
                                {
                                    this.MapPolygonLayer.MapPolygonRules = new RDL.DOM.MapPolygonRules();
                                    if (Polylayer.MapPolygonRules.MapColorRule != null)
                                    {

                                        if (Polylayer.MapPolygonRules.MapColorRule is Syncfusion.RDL.DOM.MapColorRangeRule)
                                        {
                                            var rangeRule = Polylayer.MapPolygonRules.MapColorRule as Syncfusion.RDL.DOM.MapColorRangeRule;
                                            if (rangeRule != null)
                                            {
                                                //colorRule.MapColorRangeRule = new MapColorRangeRuleExp();
                                                //colorRule.MapColorRangeRule.StartColor = this.GetExpressionKey(rangeRule.StartColor);
                                                //colorRule.MapColorRangeRule.EndColor = this.GetExpressionKey(rangeRule.EndColor);
                                                //colorRule.MapColorRangeRule.MiddleColor = this.GetExpressionKey(rangeRule.MiddleColor);
                                            }
                                        }
                                        else if (Polylayer.MapPolygonRules.MapColorRule is Syncfusion.RDL.DOM.MapColorPaletteRule)
                                        {
                                            var paletteRule = Polylayer.MapPolygonRules.MapColorRule as Syncfusion.RDL.DOM.MapColorPaletteRule;
                                            if (paletteRule != null)
                                            {
                                                //colorRule.ColorPalette = new MapColorPaletteRuleExp();
                                                //colorRule.ColorPalette.Palette = this.GetExpressionKey(paletteRule.Palette.ToString());
                                            }
                                        }
                                        else if (Polylayer.MapPolygonRules.MapColorRule is Syncfusion.RDL.DOM.MapCustomColorRule)
                                        {
                                            var customColor = Polylayer.MapPolygonRules.MapColorRule as Syncfusion.RDL.DOM.MapCustomColorRule;
                                            if (customColor != null && customColor.MapCustomColors != null && customColor.MapCustomColors.MapCustomColor != null)
                                            {
                                                //colorRule.MapCustomColors = new List<string>();
                                                //foreach (var color in customColor.MapCustomColors.MapCustomColor)
                                                //{
                                                //    colorRule.MapCustomColors.Add(this.GetExpressionKey(color));
                                                //}
                                            }
                                        }

                                        this.MapPolygonLayer.MapPolygonRules.MapColorRule.BucketCount = Polylayer.MapPolygonRules.MapColorRule.BucketCount;
                                        this.MapPolygonLayer.MapPolygonRules.MapColorRule.DataElementName = Polylayer.MapPolygonRules.MapColorRule.DataElementName;
                                        this.MapPolygonLayer.MapPolygonRules.MapColorRule.DataElementOutput = Polylayer.MapPolygonRules.MapColorRule.DataElementOutput;
                                        //colorRule.DataValue = this.GetExpressionKey(mapColorRule.DataValue);
                                        this.MapPolygonLayer.MapPolygonRules.MapColorRule.DataValue = Polylayer.MapPolygonRules.MapColorRule.DataValue;
                                        this.MapPolygonLayer.MapPolygonRules.MapColorRule.DistributionType = Polylayer.MapPolygonRules.MapColorRule.DistributionType;
                                        this.MapPolygonLayer.MapPolygonRules.MapColorRule.EndValue = Polylayer.MapPolygonRules.MapColorRule.EndValue;
                                    }
                                }

                                if (Polylayer.MapPolygons != null)
                                {
                                    this.MapPolygonLayer.MapPolygons = new RDL.DOM.MapPolygons();
                                    foreach (var polygon in Polylayer.MapPolygons)
                                    {
                                        RDL.DOM.MapPolygon polygn = new RDL.DOM.MapPolygon();
                                        if (polygon.MapCenterPointTemplate != null)
                                        {
                                            //polygn.MapCenterPointTemplate = GetMapPointTemplateExp(polygon.MapCenterPointTemplate);
                                        }
                                        if (polygon.MapFields != null)
                                        {
                                            //polygn.MapFields = GetMapFieldsExp(polygon.MapFields);
                                        }
                                        if (polygon.MapPolygonTemplate != null)
                                        {
                                            //polygn.MapPolygonTemplate = GetMapPolygonTemplateExp(polygon.MapPolygonTemplate);
                                        }
                                        polygn.UseCustomCenterPointTemplate = polygon.UseCustomCenterPointTemplate;
                                        polygn.UseCustomPolygonTemplate = polygon.UseCustomPolygonTemplate;
                                        polygn.VectorData = polygon.VectorData;
                                        this.MapPolygonLayer.MapPolygons.Add(polygn);
                                    }
                                }
                                this.MapPolygonLayer.DataElementName = Polylayer.DataElementName;
                                this.MapPolygonLayer.MapDataRegionName = Polylayer.MapDataRegionName;
                                this.MapPolygonLayer.DataElementOutput = Polylayer.DataElementOutput;

                            }
                            catch { }
                        }
                        #endregion
                        this.MapLayers.Add(this.MapPolygonLayer);
                    }
                }

                #region maplegends

                if (MapBase.MapLegends != null)
                {
                    this.MapLegends = new RDL.DOM.MapLegends();
                    foreach (RDL.DOM.MapLegend Maplegend in MapBase.MapLegends)
                    {
                        RDL.DOM.MapLegend Legend = new RDL.DOM.MapLegend();
                        Legend.MapLegendTitle = new RDL.DOM.MapLegendTitle();
                        Legend.MapLegendTitle.Style = new RDL.DOM.Style();
                        Legend.Name = Maplegend.Name;
                        Legend.MapLegendTitle.Caption = Maplegend.MapLegendTitle.Caption;

                        Legend.MapLegendTitle.Style.FontSize = Maplegend.MapLegendTitle.Style.FontSize;
                        Legend.MapLegendTitle.Style.Format = Maplegend.MapLegendTitle.Style.Format;
                        Legend.MapLegendTitle.Style.Color = Maplegend.MapLegendTitle.Style.Color;
                        Legend.MapLegendTitle.Style.FontWeight = Maplegend.MapLegendTitle.Style.FontWeight;
                        Legend.MapLegendTitle.Style.FontStyle = Maplegend.MapLegendTitle.Style.FontStyle;
                        Legend.MapLegendTitle.Style.BackgroundColor = Maplegend.MapLegendTitle.Style.BackgroundColor;
                        Legend.AutoFitTextDisabled = Maplegend.AutoFitTextDisabled;
                        Legend.DockOutsideViewport = Maplegend.DockOutsideViewport;
                        Legend.EquallySpacedItems = Maplegend.EquallySpacedItems;
                        Legend.Hidden = Maplegend.Hidden;
                        Legend.InterlacedRows = Maplegend.InterlacedRows;
                        Legend.InterlacedRowsColor = Maplegend.InterlacedRowsColor;
                        Legend.Position = Maplegend.Position;
                        Legend.Layout = Maplegend.Layout;

                        Legend.LeftMargin = Maplegend.LeftMargin;
                        Legend.RightMargin = Maplegend.RightMargin;
                        Legend.TopMargin = Maplegend.TopMargin;
                        Legend.BottomMargin = Maplegend.BottomMargin;

                        this.MapLegends.Add(Legend);
                    }
                }

                #endregion

                #region Maptitles
                if (MapBase.MapTitles != null)
                {
                    this.MapTitles = new RDL.DOM.MapTitles();
                    foreach (RDL.DOM.MapTitle Maptitle in MapBase.MapTitles)
                    {
                        RDL.DOM.MapTitle Title = new RDL.DOM.MapTitle();
                        Title.Style = new RDL.DOM.Style();
                        Title.Style.Border = new RDL.DOM.Border();
                        Title.Name = Maptitle.Name;
                        Title.Text = Maptitle.Text;
                        Title.Angle = Maptitle.Angle;
                        Title.TextShadowOffset = Maptitle.TextShadowOffset;
                        Title.DockOutsideViewport = Maptitle.DockOutsideViewport;
                        Title.Position = Maptitle.Position;
                        Title.LeftMargin = Maptitle.LeftMargin;
                        Title.RightMargin = Maptitle.RightMargin;
                        Title.TopMargin = Maptitle.TopMargin;
                        Title.BottomMargin = Maptitle.BottomMargin;
                        Title.DockOutsideViewport = Maptitle.DockOutsideViewport;
                        Title.Style.Border.Color = Maptitle.Style.Border.Color;
                        Title.Style.Border.Style = Maptitle.Style.Border.Style;
                        Title.Style.Border.Width = Maptitle.Style.Border.Width;
                        Title.Style.FontSize = Maptitle.Style.FontSize;
                        Title.Style.Format = Maptitle.Style.Format;
                        Title.Style.Color = Maptitle.Style.Color;
                        Title.Style.BackgroundGradientType = Maptitle.Style.BackgroundGradientType;
                        Title.Style.BackgroundGradientEndColor = Maptitle.Style.BackgroundGradientEndColor;
                        Title.Style.FontWeight = Maptitle.Style.FontWeight;
                        Title.Style.TextAlign = Maptitle.Style.TextAlign;
                        Title.Style.ShadowOffset = Maptitle.Style.ShadowOffset;

                        this.MapTitles.Add(Title);
                    }
                }

                #endregion

                #region Distance scale
                if (MapBase.MapDistanceScale != null)
                {
                    this.DistanceScale = new RDL.DOM.MapDistanceScale();
                    this.DistanceScale.DockOutsideViewport = MapBase.MapDistanceScale.DockOutsideViewport;
                    this.DistanceScale.Hidden = MapBase.MapDistanceScale.Hidden;
                    this.DistanceScale.MapLocation = MapBase.MapDistanceScale.MapLocation;
                    this.DistanceScale.ScaleBorderColor = MapBase.MapDistanceScale.ScaleBorderColor;
                    this.DistanceScale.ScaleColor = MapBase.MapDistanceScale.ScaleColor;
                    this.DistanceScale.LeftMargin = MapBase.MapDistanceScale.LeftMargin;
                    this.DistanceScale.RightMargin = MapBase.MapDistanceScale.RightMargin;
                    this.DistanceScale.TopMargin = MapBase.MapDistanceScale.TopMargin;
                    this.DistanceScale.BottomMargin = MapBase.MapDistanceScale.BottomMargin;
                    this.DistanceScale.Style = new RDL.DOM.Style();
                    this.DistanceScale.Style.Border = new RDL.DOM.Border();

                    this.DistanceScale.Style.Border.Color = MapBase.MapDistanceScale.Style.Border.Color;
                    this.DistanceScale.Style.Border.Style = MapBase.MapDistanceScale.Style.Border.Style;
                    this.DistanceScale.Style.Border.Width = MapBase.MapDistanceScale.Style.Border.Width;
                    this.DistanceScale.Style.FontSize = MapBase.MapDistanceScale.Style.FontSize;
                    this.DistanceScale.Style.Format = MapBase.MapDistanceScale.Style.Format;
                    this.DistanceScale.Style.Color = MapBase.MapDistanceScale.Style.Color;
                    this.DistanceScale.Style.BackgroundGradientType = MapBase.MapDistanceScale.Style.BackgroundGradientType;
                    this.DistanceScale.Style.BackgroundGradientEndColor = MapBase.MapDistanceScale.Style.BackgroundGradientEndColor;
                    this.DistanceScale.Style.FontWeight = MapBase.MapDistanceScale.Style.FontWeight;
                    this.DistanceScale.Style.TextAlign = MapBase.MapDistanceScale.Style.TextAlign;
                    this.DistanceScale.Style.ShadowOffset = MapBase.MapDistanceScale.Style.ShadowOffset;
                    this.DistanceScale.MapSize = new RDL.DOM.MapSize();
                    this.DistanceScale.MapSize.Height = MapBase.MapDistanceScale.MapSize.Height;
                    this.DistanceScale.MapSize.Width = MapBase.MapDistanceScale.MapSize.Width;
                    this.DistanceScale.MapSize.Unit = MapBase.MapDistanceScale.MapSize.Unit;
                }

                #endregion

                #region color scale
                if (MapBase.MapColorScale != null)
                {
                    this.ColorScale = new RDL.DOM.MapColorScale();
                    this.ColorScale.ColorBarBorderColor = MapBase.MapColorScale.ColorBarBorderColor;
                    this.ColorScale.DockOutsideViewport = MapBase.MapColorScale.DockOutsideViewport;
                    this.ColorScale.Hidden = MapBase.MapColorScale.Hidden;
                    this.ColorScale.HideEndLabels = MapBase.MapColorScale.HideEndLabels;
                    this.ColorScale.LabelFormat = MapBase.MapColorScale.LabelFormat;
                    this.ColorScale.LabelInterval = MapBase.MapColorScale.LabelInterval;

                    this.ColorScale.LabelPlacement = MapBase.MapColorScale.LabelPlacement;
                    this.ColorScale.MapColorScaleTitle = new RDL.DOM.MapColorScaleTitle();
                    this.ColorScale.MapColorScaleTitle.Caption = MapBase.MapColorScale.MapColorScaleTitle.Caption;
                    this.ColorScale.MapColorScaleTitle.Style = new RDL.DOM.Style();
                    this.ColorScale.MapColorScaleTitle.Style.FontSize = MapBase.MapColorScale.MapColorScaleTitle.Style.FontSize;
                    this.ColorScale.MapColorScaleTitle.Style.FontWeight = MapBase.MapColorScale.MapColorScaleTitle.Style.FontWeight;
                    this.ColorScale.NoDataText = MapBase.MapColorScale.NoDataText;
                    this.ColorScale.RangeGapColor = MapBase.MapColorScale.RangeGapColor;
                    this.ColorScale.Style = new RDL.DOM.Style();
                    this.ColorScale.Style.Border = new RDL.DOM.Border();
                    this.ColorScale.Style.Border.Width = MapBase.MapColorScale.Style.Border.Width;
                    this.ColorScale.Style.Border.Style = MapBase.MapColorScale.Style.Border.Style;
                    this.ColorScale.Style.Border.Color = MapBase.MapColorScale.Style.Border.Color;

                    this.ColorScale.Style.BackgroundColor = MapBase.MapColorScale.Style.BackgroundColor;
                    this.ColorScale.Style.BackgroundGradientEndColor = MapBase.MapColorScale.Style.BackgroundGradientEndColor;
                    if (MapBase.MapColorScale.Style.BackgroundGradientType !=  RDL.DOM.BackgroundGradientTypes.Default)
                    {
                        this.ColorScale.Style.BackgroundGradientType = MapBase.MapColorScale.Style.BackgroundGradientType;
                    }
                    this.ColorScale.Style.FontSize = MapBase.MapColorScale.Style.FontSize;
                    this.ColorScale.Style.FontWeight = MapBase.MapColorScale.Style.FontWeight;
                    this.ColorScale.Style.ShadowOffset = MapBase.MapColorScale.Style.ShadowOffset;
                    this.ColorScale.LeftMargin = MapBase.MapColorScale.LeftMargin;
                    this.ColorScale.RightMargin = MapBase.MapColorScale.RightMargin;
                    this.ColorScale.TopMargin = MapBase.MapColorScale.TopMargin;
                    this.ColorScale.BottomMargin = MapBase.MapColorScale.BottomMargin;
                }
                if (MapBase.MapBorderSkin != null)
                {
                    this.BorderSkin = new RDL.DOM.MapBorderSkin();
                    if (MapBase.MapBorderSkin.MapBorderSkinType !=  RDL.DOM.BorderSkinType.None)
                    {
                        this.BorderSkin.MapBorderSkinType = MapBase.MapBorderSkin.MapBorderSkinType;
                    }
                    this.BorderSkin.Style = new RDL.DOM.Style();
                    this.BorderSkin.Style.Border = new RDL.DOM.Border();
                    this.BorderSkin.Style.Border.Color = MapBase.MapBorderSkin.Style.Border.Color;
                    this.BorderSkin.Style.Border.Style = MapBase.MapBorderSkin.Style.Border.Style;
                    this.BorderSkin.Style.Border.Width = MapBase.MapBorderSkin.Style.Border.Width;
                    this.BorderSkin.Style.BackgroundColor = MapBase.MapBorderSkin.Style.BackgroundColor;
                    this.BorderSkin.Style.BackgroundGradientEndColor = MapBase.MapBorderSkin.Style.BackgroundGradientEndColor;
                    this.BorderSkin.Style.BackgroundGradientType = MapBase.MapBorderSkin.Style.BackgroundGradientType;
                    this.BorderSkin.Style.Color = MapBase.MapBorderSkin.Style.Color;

                }

                #endregion

            }
            catch (Exception)
            {

            }
        }


        #endregion

        #region Serialisation

        private Syncfusion.RDL.DOM.Map UpdateMapObj(Syncfusion.RDL.DOM.Map Map)
        {
            //Syncfusion.RDL.DOM.Map Map = new RDL.DOM.Map();

            RDL.DOM.MapTitles MapTitles = new RDL.DOM.MapTitles();
            RDL.DOM.MapLegends MapLegends = new RDL.DOM.MapLegends();

            Map.Name = this.MapProperties.Name;
            if (this.MapProperties.Size != null)
            {
                Map.Height = this.MapProperties.Size.Height;

                Map.Width = this.MapProperties.Size.Width;
            }
            if (this.MapProperties.Location != null)
            {
                Map.Left = this.MapProperties.Location.Left;
                Map.Top = this.MapProperties.Location.Top;
            }

            if (this.MapProperties.DocumentMapLabel != null)
                Map.DocumentMapLabel = this.MapProperties.DocumentMapLabel;
            if (this.MapProperties.DataElementName != null)
                Map.DataElementName = this.MapProperties.DataElementName;

            Map.Visibility = new RDL.DOM.Visibility();
            if (this.MapProperties.ToggleItem != null)
                Map.Visibility.ToggleItem = this.MapProperties.ToggleItem;
            if (this.MapProperties.Hidden != null)
                Map.Visibility.Hidden = this.MapProperties.Hidden;

            if (this.MapProperties.BreakLocation != null)
            {
                Map.PageBreak = new RDL.DOM.PageBreak();
                Map.PageBreak.BreakLocation = (RDL.DOM.BreakLocation)Enum.Parse(typeof(RDL.DOM.BreakLocation), this.MapProperties.BreakLocation);
            }
            if (this.MapProperties.DataElementOutput != null)
            {
                Map.DataElementOutput = (RDL.DOM.DataElementOutputs)Enum.Parse(typeof(RDL.DOM.DataElementOutputs), this.MapProperties.DataElementOutput);
            }

            Map.Style = new RDL.DOM.Style();
            Map.Style.Border = new RDL.DOM.Border();

            if (this.MapProperties.BorderStyle != null)
            {
                Map.Style.Border.Style = this.MapProperties.BorderStyle;
            }
            if (this.MapProperties.BorderColor != null)
            {
                Map.Style.Border.Color = this.MapProperties.BorderColor;
            }
            if (this.MapProperties.BorderWidth != null)
            {
                Map.Style.Border.Width = this.MapProperties.BorderWidth;
            }

            Map.Style.BackgroundColor = this.MapProperties.BackgroundColor;
            if (this.MapProperties.MaximumSpatialElementCount != null)
                Map.MaximumSpatialElementCount = int.Parse(this.MapProperties.MaximumSpatialElementCount);
            if (this.MapProperties.MaximumTotalPointCount != null)
                Map.MaximumTotalPointCount = int.Parse(this.MapProperties.MaximumTotalPointCount);
            Map.TileLanguage = this.MapProperties.TileLanguage;
            if (this.MapProperties.AntiAliasing != null)
                Map.AntiAliasing = (RDL.DOM.AntiAliasing)Enum.Parse(typeof(RDL.DOM.AntiAliasing), this.MapProperties.AntiAliasing);
            Map.TextAntiAliasingQuality = (RDL.DOM.TextAntiAliasingQuality)Enum.Parse(typeof(RDL.DOM.TextAntiAliasingQuality), this.MapProperties.TextAntiAliasingQuality);
            Map.DataSetName = this.DataSetName;

            Map.MapViewport = new RDL.DOM.MapViewport();
            Map.MapViewport.Style = new RDL.DOM.Style();
            Map.MapViewport.Style.Border = new RDL.DOM.Border();
#if !SyncfusionFramework3_5
            Map.MapViewport.Style.Border.Color = InternalMap.BorderBrush.ToString();
            Map.MapViewport.Style.Border.Width = InternalMap.BorderThickness.ToString();
#endif

            #region viewport
            if (this.ViewPort != null)
            {
                Map.MapViewport = new RDL.DOM.MapViewport();
                Map.MapViewport.MapCoordinateSystem = this.ViewPort.MapCoordinateSystem;
                Map.MapViewport.MapProjection = this.ViewPort.MapProjection;
                Map.MapViewport.ProjectionCenterX = this.ViewPort.ProjectionCenterX;
                Map.MapViewport.ProjectionCenterY = this.ViewPort.ProjectionCenterY;

                if (this.ViewPort.MapLimits != null)
                {
                    Map.MapViewport.MapLimits = new RDL.DOM.MapLimits();
                    Map.MapViewport.MapLimits.MaximumX = this.ViewPort.MapLimits.MaximumX;
                    Map.MapViewport.MapLimits.MaximumY = this.ViewPort.MapLimits.MaximumY;
                    Map.MapViewport.MapLimits.MinimumX = this.ViewPort.MapLimits.MinimumX;
                    Map.MapViewport.MapLimits.MinimumY = this.ViewPort.MapLimits.MinimumY;
                    Map.MapViewport.MaximumZoom = this.ViewPort.MaximumZoom;
                }

                if (Map.MapViewport.MapView != null)
                {
                    //this.ViewPort.MapView = new RDL.DOM.MapView();
                    this.ViewPort.MapView.Zoom = Map.MapViewport.MapView.Zoom;
                }

                if (this.ViewPort.MapMeridians != null)
                {
                    Map.MapViewport.MapMeridians = new RDL.DOM.MapMeridians();
                    Map.MapViewport.MapMeridians.Style = new RDL.DOM.Style();
                    Map.MapViewport.MapMeridians.Style.Border = new RDL.DOM.Border();
                    Map.MapViewport.MapMeridians.Style.Border.Color = this.ViewPort.MapMeridians.Style.Border.Color;
                    Map.MapViewport.MapMeridians.Style.Border.Style = this.ViewPort.MapMeridians.Style.Border.Style;
                    Map.MapViewport.MapMeridians.Style.Border.Width = this.ViewPort.MapMeridians.Style.Border.Width;
                    Map.MapViewport.MapMeridians.Style.FontSize = this.ViewPort.MapMeridians.Style.FontSize;
                    Map.MapViewport.MapMeridians.Style.Format = this.ViewPort.MapMeridians.Style.Format;
                    Map.MapViewport.MapMeridians.Style.Color = this.ViewPort.MapMeridians.Style.Color;
                    Map.MapViewport.MapMeridians.Hidden = this.ViewPort.MapMeridians.Hidden;
                    Map.MapViewport.MapMeridians.Interval = this.ViewPort.MapMeridians.Interval;
                    Map.MapViewport.MapMeridians.LabelPosition = this.ViewPort.MapMeridians.LabelPosition;
                    Map.MapViewport.MapMeridians.ShowLabels = this.ViewPort.MapMeridians.ShowLabels;
                }

                if (this.ViewPort.MapParallels!=null)
                {
                    Map.MapViewport.MapParallels = new RDL.DOM.MapParallels();
                    Map.MapViewport.MapParallels.Style = new RDL.DOM.Style();
                    Map.MapViewport.MapParallels.Style.Border = new RDL.DOM.Border();
                    Map.MapViewport.MapParallels.Style.Border.Color = this.ViewPort.MapParallels.Style.Border.Color;
                    Map.MapViewport.MapParallels.Style.Border.Style = this.ViewPort.MapParallels.Style.Border.Style;
                    Map.MapViewport.MapParallels.Style.Border.Width = this.ViewPort.MapParallels.Style.Border.Width;
                    Map.MapViewport.MapParallels.Style.FontSize = this.ViewPort.MapParallels.Style.FontSize;
                    Map.MapViewport.MapParallels.Style.Format = this.ViewPort.MapParallels.Style.Format;
                    Map.MapViewport.MapParallels.Style.Color = this.ViewPort.MapParallels.Style.Color;
                    Map.MapViewport.MapParallels.Hidden = this.ViewPort.MapParallels.Hidden;
                    Map.MapViewport.MapParallels.Interval = this.ViewPort.MapParallels.Interval;
                    Map.MapViewport.MapParallels.LabelPosition = this.ViewPort.MapParallels.LabelPosition;
                    Map.MapViewport.MapParallels.ShowLabels = this.ViewPort.MapParallels.ShowLabels;
                }
                
                Map.MapViewport.GridUnderContent = this.ViewPort.GridUnderContent;
                Map.MapViewport.Style = new RDL.DOM.Style();
                Map.MapViewport.Style.Border = new RDL.DOM.Border();
                Map.MapViewport.Style.Border.Color =this.ViewPort.Style.Border.Color;
                Map.MapViewport.Style.Border.Style = this.ViewPort.Style.Border.Style;
                Map.MapViewport.Style.Border.Width =this.ViewPort.Style.Border.Width;

                Map.MapViewport.LeftMargin = this.ViewPort.LeftMargin;
                Map.MapViewport.RightMargin = this.ViewPort.RightMargin;
                Map.MapViewport.TopMargin = this.ViewPort.TopMargin;
                Map.MapViewport.BottomMargin = this.ViewPort.BottomMargin;
            }

            #endregion

            #region maplayers
            Map.MapLayers = new RDL.DOM.MapLayers();
            if (this.MapLayers != null)
            {
                foreach (var Maplayer in this.MapLayers)
                {
                    #region polygonlayer
                    if (Maplayer is Syncfusion.RDL.DOM.MapPolygonLayer)
                    {
                        var Polylayer = Maplayer as RDL.DOM.MapPolygonLayer;
                        this.MapPolygonLayer = new RDL.DOM.MapPolygonLayer();

                        try
                        {
                            if (Polylayer.MapSpatialData != null)
                            {

                                if ((Polylayer.MapSpatialData as RDL.DOM.MapShapefile) != null)
                                {
                                    var shapefile = (Polylayer.MapSpatialData as RDL.DOM.MapShapefile);
                                    RDL.DOM.MapShapefile MapShapefile = new RDL.DOM.MapShapefile();

                                    if (shapefile.MapFieldNames != null && shapefile.MapFieldNames.MapFieldName != null && shapefile.MapFieldNames.MapFieldName.Count > 0)
                                    {
                                        MapShapefile.MapFieldNames = new RDL.DOM.MapFieldNames();

                                        foreach (var field in shapefile.MapFieldNames.MapFieldName)
                                        {
                                            MapShapefile.MapFieldNames.MapFieldName.Add(field);
                                        }
                                    }
                                    MapShapefile.Source = shapefile.Source;


                                    this.MapPolygonLayer.MapSpatialData = Polylayer.MapSpatialData as RDL.DOM.MapShapefile;
                                }
                                if ((Polylayer.MapSpatialData as Syncfusion.RDL.DOM.MapSpatialDataRegion) != null)
                                {
                                    this.MapDataRegion=new RDL.DOM.MapSpatialDataRegion();
                                    this.MapDataRegion.VectorData = (Polylayer.MapSpatialData as Syncfusion.RDL.DOM.MapSpatialDataRegion).VectorData;

                                    this.MapPolygonLayer.MapSpatialData = Polylayer.MapSpatialData as RDL.DOM.MapSpatialDataRegion;
                                }
                                if ((Polylayer.MapSpatialData as Syncfusion.RDL.DOM.MapSpatialDataSet) != null)
                                {
                                    var spatialDataSet = (Polylayer.MapSpatialData as Syncfusion.RDL.DOM.MapSpatialDataSet);
                                    this.MapDataSet = new RDL.DOM.MapSpatialDataSet();

                                    if (spatialDataSet.MapFieldNames != null && spatialDataSet.MapFieldNames.MapFieldName != null && spatialDataSet.MapFieldNames.MapFieldName.Count > 0)
                                    {
                                        this.MapDataSet.MapFieldNames.MapFieldName = new List<string>();
                                        foreach (var field in spatialDataSet.MapFieldNames.MapFieldName)
                                        {
                                            this.MapDataSet.MapFieldNames.MapFieldName.Add(field);
                                        }
                                    }
                                    this.MapDataSet.SpatialField = spatialDataSet.SpatialField;
                                    this.MapDataSet.DataSetName = spatialDataSet.DataSetName;

                                    this.MapPolygonLayer.MapSpatialData = this.MapDataSet as RDL.DOM.MapSpatialDataSet;
                                }
                            }
                            if (Polylayer.MapBindingFieldPairs != null && Polylayer.MapBindingFieldPairs.Count > 0)
                            {
                                foreach (var field in Polylayer.MapBindingFieldPairs)
                                {
                                    RDL.DOM.MapBindingFieldPair pair = new RDL.DOM.MapBindingFieldPair();
                                    //pair.BindingExpression = this.Model.ExpressionEngine.GetEvalExpressionString(field.BindingExpression);
                                    //pair.FieldName = this.Model.ExpressionEngine.GetEvalExpressionString(field.FieldName);
                                    pair.BindingExpression = field.BindingExpression;
                                    pair.FieldName = field.FieldName;
                                    this.MapPolygonLayer.MapBindingFieldPairs.Add(pair);
                                }
                            }
                            if (Polylayer.MapCenterPointRules != null)
                            {
                                this.MapPolygonLayer.MapCenterPointRules = new RDL.DOM.MapCenterPointRules();
                                if (Polylayer.MapCenterPointRules.MapColorRule != null)
                                {


                                    //if (Polylayer.MapCenterPointRules.MapColorRule is Syncfusion.RDL.DOM.MapColorRangeRule)
                                    //{
                                    //    var rangeRule = Polylayer.MapCenterPointRules.MapColorRule  as Syncfusion.RDL.DOM.MapColorRangeRule;
                                    //    if (rangeRule != null)
                                    //    {
                                    //        this.ColorRule.MapColorRangeRule = new MapColorRangeRuleExp();
                                    //        colorRule.MapColorRangeRule.StartColor = this.GetExpressionKey(rangeRule.StartColor);
                                    //        colorRule.MapColorRangeRule.EndColor = this.GetExpressionKey(rangeRule.EndColor);
                                    //        colorRule.MapColorRangeRule.MiddleColor = this.GetExpressionKey(rangeRule.MiddleColor);
                                    //    }
                                    //}
                                    //else if (Polylayer.MapCenterPointRules.MapColorRule is Syncfusion.RDL.DOM.MapColorPaletteRule)
                                    //{
                                    //    var paletteRule = Polylayer.MapCenterPointRules.MapColorRule as Syncfusion.RDL.DOM.MapColorPaletteRule;
                                    //    if (paletteRule != null)
                                    //    {
                                    //        colorRule.ColorPalette = new MapColorPaletteRuleExp();
                                    //        colorRule.ColorPalette.Palette = this.GetExpressionKey(paletteRule.Palette.ToString());
                                    //    }
                                    //}
                                    //else if (Polylayer.MapCenterPointRules.MapColorRule is Syncfusion.RDL.DOM.MapCustomColorRule)
                                    //{
                                    //    var customColor = Polylayer.MapCenterPointRules.MapColorRule as RDL.DOM.MapCustomColorRule;
                                    //    if (customColor != null && customColor.MapCustomColors != null && customColor.MapCustomColors.MapCustomColor != null)
                                    //    {
                                    //        //this.ColorRule.MapCustomColors = new List<string>();
                                    //        foreach (var color in customColor.MapCustomColors.MapCustomColor)
                                    //        {
                                    //            this.ColorRule.MapCustomColors.Add(color);
                                    //        }
                                    //    }
                                    //}

                                    this.ColorRule.BucketCount = Polylayer.MapCenterPointRules.MapColorRule.BucketCount;
                                    this.ColorRule.DataElementName = Polylayer.MapCenterPointRules.MapColorRule.DataElementName;
                                    this.ColorRule.DataElementOutput = Polylayer.MapCenterPointRules.MapColorRule.DataElementOutput;
                                    //colorRule.DataValue = this.GetExpressionKey(mapColorRule.DataValue);
                                    this.ColorRule.DataValue = Polylayer.MapCenterPointRules.MapColorRule.DataValue;
                                    this.ColorRule.DistributionType = Polylayer.MapCenterPointRules.MapColorRule.DistributionType;
                                    this.ColorRule.EndValue = Polylayer.MapCenterPointRules.MapColorRule.EndValue;

                                    this.MapPolygonLayer.MapCenterPointRules.MapColorRule = this.ColorRule;
                                }
                                if (Polylayer.MapCenterPointRules.MapMarkerRule != null)
                                {
                                    this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.BucketCount = Polylayer.MapCenterPointRules.MapMarkerRule.BucketCount;
                                    this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.DataElementName = Polylayer.MapCenterPointRules.MapMarkerRule.DataElementName;
                                    this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.DataElementOutput = Polylayer.MapCenterPointRules.MapMarkerRule.DataElementOutput;
                                    //marker.DataValue = this.GetExpressionKey(mapMarkerRule.DataValue);
                                    this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.DataValue = Polylayer.MapCenterPointRules.MapMarkerRule.DataValue;
                                    this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.DistributionType = Polylayer.MapCenterPointRules.MapMarkerRule.DistributionType;
                                    this.MapPolygonLayer.MapCenterPointRules.MapMarkerRule.EndValue = Polylayer.MapCenterPointRules.MapMarkerRule.EndValue;
                                }
                                if (Polylayer.MapCenterPointRules.MapSizeRule != null)
                                {
                                    this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.BucketCount = Polylayer.MapCenterPointRules.MapSizeRule.BucketCount;
                                    this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.DataElementName = Polylayer.MapCenterPointRules.MapSizeRule.DataElementName;
                                    this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.DataElementOutput = Polylayer.MapCenterPointRules.MapSizeRule.DataElementOutput;
                                    //sizeRule.DataValue = this.GetExpressionKey(mapSizeRule.DataValue);
                                    this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.DataValue = Polylayer.MapCenterPointRules.MapSizeRule.DataValue;
                                    this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.DistributionType = Polylayer.MapCenterPointRules.MapSizeRule.DistributionType;
                                    this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.StartSize = Polylayer.MapCenterPointRules.MapSizeRule.StartSize;
                                    this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.EndSize = Polylayer.MapCenterPointRules.MapSizeRule.EndSize;
                                    this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.StartValue = Polylayer.MapCenterPointRules.MapSizeRule.StartValue;
                                    this.MapPolygonLayer.MapCenterPointRules.MapSizeRule.EndValue = Polylayer.MapCenterPointRules.MapSizeRule.EndValue;

                                }
                            }
                            if (Polylayer.MapCenterPointTemplate != null)
                            {

                                if (Polylayer.MapCenterPointTemplate.Style != null)
                                {
                                    //if (Polylayer.MapCenterPointTemplate.Style.Border != null)
                                    //    mapPolygonLayer.MapCenterPointTemplate.Border = GetBorderExp(Polylayer.MapCenterPointTemplate.Style.Border);
                                    //mapPolygonLayer.MapCenterPointTemplate.StyleExp = GetFontStyleExp(Polylayer.MapCenterPointTemplate.Style);
                                }
                                if (Polylayer.MapCenterPointTemplate.ActionInfo != null)
                                {
                                    //mapPolygonLayer.MapCenterPointTemplate.ActionInfo = GetActionInfoExp(Polylayer.MapCenterPointTemplate.ActionInfo);
                                }
                                if (Polylayer.MapCenterPointTemplate.Size != null)
                                {
                                    this.MapPolygonLayer.MapCenterPointTemplate.Size = Polylayer.MapCenterPointTemplate.Size.size;
                                }

                                this.MapPolygonLayer.MapCenterPointTemplate.DataElementLabel = Polylayer.MapCenterPointTemplate.DataElementLabel;
                                this.MapPolygonLayer.MapCenterPointTemplate.DataElementName = Polylayer.MapCenterPointTemplate.DataElementName;
                                this.MapPolygonLayer.MapCenterPointTemplate.DataElementOutput = Polylayer.MapCenterPointTemplate.DataElementOutput;
                                this.MapPolygonLayer.MapCenterPointTemplate.Hidden = Polylayer.MapCenterPointTemplate.Hidden;
                                this.MapPolygonLayer.MapCenterPointTemplate.Label = Polylayer.MapCenterPointTemplate.Label;
                                this.MapPolygonLayer.MapCenterPointTemplate.LabelPlacement = Polylayer.MapCenterPointTemplate.LabelPlacement;
                                this.MapPolygonLayer.MapCenterPointTemplate.OffsetX = Polylayer.MapCenterPointTemplate.OffsetX;
                                this.MapPolygonLayer.MapCenterPointTemplate.OffsetY = Polylayer.MapCenterPointTemplate.OffsetY;
                                this.MapPolygonLayer.MapCenterPointTemplate.ToolTip = Polylayer.MapCenterPointTemplate.ToolTip;
                            }
                            if (Polylayer.MapPolygonTemplate != null)
                            {

                                if (Polylayer.MapPolygonTemplate.Style != null)
                                {
                                    if (Polylayer.MapPolygonTemplate.Style.Border != null)
                                    {
                                        this.MapPolygonLayer.MapPolygonTemplate.Style.Border.Color = Polylayer.MapPolygonTemplate.Style.Border.Color;
                                        this.MapPolygonLayer.MapPolygonTemplate.Style.Border.Style = Polylayer.MapPolygonTemplate.Style.Border.Style;
                                        this.MapPolygonLayer.MapPolygonTemplate.Style.Border.Width = Polylayer.MapPolygonTemplate.Style.Border.Width;
                                    }
                                    this.MapPolygonLayer.MapPolygonTemplate.Style.BackgroundColor = Polylayer.MapPolygonTemplate.Style.BackgroundColor;
                                    this.MapPolygonLayer.MapPolygonTemplate.Style.BackgroundGradientEndColor = Polylayer.MapPolygonTemplate.Style.BackgroundGradientEndColor;
                                    this.MapPolygonLayer.MapPolygonTemplate.Style.BackgroundGradientType = Polylayer.MapPolygonTemplate.Style.BackgroundGradientType;
                                    this.MapPolygonLayer.MapPolygonTemplate.Style.BackgroundHatchType = Polylayer.MapPolygonTemplate.Style.BackgroundHatchType;
                                }
                                if (Polylayer.MapPolygonTemplate.ActionInfo != null)
                                {
                                    //mapPolygonLayer.MapPolygonTemplate.ActionInfo = GetActionInfoExp(Polylayer.MapPolygonTemplate.ActionInfo);
                                }

                                this.MapPolygonLayer.MapPolygonTemplate.Label = Polylayer.MapPolygonTemplate.Label;
                                this.MapPolygonLayer.MapPolygonTemplate.ShowLabel = Polylayer.MapPolygonTemplate.ShowLabel;
                                this.MapPolygonLayer.MapPolygonTemplate.CenterPointOffsetX = Polylayer.MapPolygonTemplate.CenterPointOffsetX;
                                this.MapPolygonLayer.MapPolygonTemplate.CenterPointOffsetY = Polylayer.MapPolygonTemplate.CenterPointOffsetY;
                                this.MapPolygonLayer.MapPolygonTemplate.DataElementLabel = Polylayer.MapPolygonTemplate.DataElementLabel;
                                this.MapPolygonLayer.MapPolygonTemplate.DataElementName = Polylayer.MapPolygonTemplate.DataElementName;
                                this.MapPolygonLayer.MapPolygonTemplate.DataElementOutput = Polylayer.MapPolygonTemplate.DataElementOutput;
                                this.MapPolygonLayer.MapPolygonTemplate.Hidden = Polylayer.MapPolygonTemplate.Hidden;
                                this.MapPolygonLayer.MapPolygonTemplate.Label = Polylayer.MapPolygonTemplate.Label;
                                //this.lableFieldName.Add(GetFieldName(mapPolygonTemplate.Label));
                                this.MapPolygonLayer.MapPolygonTemplate.LabelPlacement = Polylayer.MapPolygonTemplate.LabelPlacement;
                                this.MapPolygonLayer.MapPolygonTemplate.OffsetX = Polylayer.MapPolygonTemplate.OffsetX;
                                this.MapPolygonLayer.MapPolygonTemplate.OffsetY = Polylayer.MapPolygonTemplate.OffsetY;
                                this.MapPolygonLayer.MapPolygonTemplate.ScaleFactor = Polylayer.MapPolygonTemplate.ScaleFactor;
                                this.MapPolygonLayer.MapPolygonTemplate.ShowLabel = Polylayer.MapPolygonTemplate.ShowLabel;
                                this.MapPolygonLayer.MapPolygonTemplate.ToolTip = Polylayer.MapPolygonTemplate.ToolTip;
                            }
                            if (Polylayer.MapFieldDefinitions != null)
                            {
                                //mapPolygonLayer.MapFieldDefinitions = GetMapFieldDefExp(Polylayer.MapFieldDefinitions);
                            }
                            if (Polylayer.MapPolygonRules != null)
                            {
                                this.MapPolygonLayer.MapPolygonRules = new RDL.DOM.MapPolygonRules();
                                if (Polylayer.MapPolygonRules.MapColorRule != null)
                                {

                                    if (Polylayer.MapPolygonRules.MapColorRule is Syncfusion.RDL.DOM.MapColorRangeRule)
                                    {
                                        var rangeRule = Polylayer.MapPolygonRules.MapColorRule as Syncfusion.RDL.DOM.MapColorRangeRule;
                                        if (rangeRule != null)
                                        {                                            
                                            //colorRule.MapColorRangeRule = new MapColorRangeRuleExp();
                                            //colorRule.MapColorRangeRule.StartColor = this.GetExpressionKey(rangeRule.StartColor);
                                            //colorRule.MapColorRangeRule.EndColor = this.GetExpressionKey(rangeRule.EndColor);
                                            //colorRule.MapColorRangeRule.MiddleColor = this.GetExpressionKey(rangeRule.MiddleColor);
                                        }
                                    }
                                    else if (Polylayer.MapPolygonRules.MapColorRule is Syncfusion.RDL.DOM.MapColorPaletteRule)
                                    {
                                        var paletteRule = Polylayer.MapPolygonRules.MapColorRule as Syncfusion.RDL.DOM.MapColorPaletteRule;
                                        if (paletteRule != null)
                                        {
                                            //colorRule.ColorPalette = new MapColorPaletteRuleExp();
                                            //colorRule.ColorPalette.Palette = this.GetExpressionKey(paletteRule.Palette.ToString());
                                        }
                                    }
                                    else if (Polylayer.MapPolygonRules.MapColorRule is Syncfusion.RDL.DOM.MapCustomColorRule)
                                    {
                                        var customColor = Polylayer.MapPolygonRules.MapColorRule as Syncfusion.RDL.DOM.MapCustomColorRule;
                                        if (customColor != null && customColor.MapCustomColors != null && customColor.MapCustomColors.MapCustomColor != null)
                                        {
                                            //colorRule.MapCustomColors = new List<string>();
                                            //foreach (var color in customColor.MapCustomColors.MapCustomColor)
                                            //{
                                            //    colorRule.MapCustomColors.Add(this.GetExpressionKey(color));
                                            //}
                                        }
                                    }

                                    this.MapPolygonLayer.MapPolygonRules.MapColorRule.BucketCount = Polylayer.MapPolygonRules.MapColorRule.BucketCount;
                                    this.MapPolygonLayer.MapPolygonRules.MapColorRule.DataElementName = Polylayer.MapPolygonRules.MapColorRule.DataElementName;
                                    this.MapPolygonLayer.MapPolygonRules.MapColorRule.DataElementOutput = Polylayer.MapPolygonRules.MapColorRule.DataElementOutput;
                                    //colorRule.DataValue = this.GetExpressionKey(mapColorRule.DataValue);
                                    this.MapPolygonLayer.MapPolygonRules.MapColorRule.DataValue = Polylayer.MapPolygonRules.MapColorRule.DataValue;
                                    this.MapPolygonLayer.MapPolygonRules.MapColorRule.DistributionType = Polylayer.MapPolygonRules.MapColorRule.DistributionType;
                                    this.MapPolygonLayer.MapPolygonRules.MapColorRule.EndValue = Polylayer.MapPolygonRules.MapColorRule.EndValue;
                                }
                            }

                            if (Polylayer.MapPolygons != null)
                            {
                                this.MapPolygonLayer.MapPolygons = new RDL.DOM.MapPolygons();
                                foreach (var polygon in Polylayer.MapPolygons)
                                {
                                    RDL.DOM.MapPolygon polygn = new RDL.DOM.MapPolygon();
                                    if (polygon.MapCenterPointTemplate != null)
                                    {
                                        //polygn.MapCenterPointTemplate = GetMapPointTemplateExp(polygon.MapCenterPointTemplate);
                                    }
                                    if (polygon.MapFields != null)
                                    {
                                        //polygn.MapFields = GetMapFieldsExp(polygon.MapFields);
                                    }
                                    if (polygon.MapPolygonTemplate != null)
                                    {
                                        //polygn.MapPolygonTemplate = GetMapPolygonTemplateExp(polygon.MapPolygonTemplate);
                                    }
                                    polygn.UseCustomCenterPointTemplate = polygon.UseCustomCenterPointTemplate;
                                    polygn.UseCustomPolygonTemplate = polygon.UseCustomPolygonTemplate;
                                    polygn.VectorData = polygon.VectorData;
                                    this.MapPolygonLayer.MapPolygons.Add(polygn);
                                }
                            }
                            this.MapPolygonLayer.DataElementName = Polylayer.DataElementName;
                            this.MapPolygonLayer.MapDataRegionName = Polylayer.MapDataRegionName;
                            this.MapPolygonLayer.DataElementOutput = Polylayer.DataElementOutput;

                        }
                        catch { }
                    }

                    #endregion

                    Map.MapLayers.Add(this.MapPolygonLayer);
                }
            }

            #endregion

            #region dataregions
            if (this.DataRegions != null)
            {
                Map.MapDataRegions = new RDL.DOM.MapDataRegions();
                foreach (RDL.DOM.MapDataRegion MapDataRegion in this.DataRegions)
                {
                    RDL.DOM.MapDataRegion DataRegion = new RDL.DOM.MapDataRegion();
                    DataRegion.Name = MapDataRegion.Name;
                    DataRegion.MapMember = new RDL.DOM.MapMember();
                    DataRegion.MapMember.Group = new RDL.DOM.Group();
                    DataRegion.MapMember.Group.Name = MapDataRegion.MapMember.Group.Name;
                    foreach (RDL.DOM.GroupExpression GroupExp in DataRegion.MapMember.Group.GroupExpressions)
                    {
                        DataRegion.MapMember.Group.GroupExpressions.Add(GroupExp);
                    }
                    Map.MapDataRegions.Add(DataRegion);
                }
            }

            #endregion

            #region maplegends
            if (this.MapLegends != null)
            {
                Map.MapLegends = new RDL.DOM.MapLegends();
                foreach (RDL.DOM.MapLegend Maplegend in this.MapLegends)
                {
                    RDL.DOM.MapLegend Legend = new RDL.DOM.MapLegend();
                    Legend.MapLegendTitle = new RDL.DOM.MapLegendTitle();
                    Legend.MapLegendTitle.Style = new RDL.DOM.Style();
                    Legend.Name = Maplegend.Name;
                    Legend.MapLegendTitle.Caption = Maplegend.MapLegendTitle.Caption;

                    Legend.MapLegendTitle.Style.FontSize = Maplegend.MapLegendTitle.Style.FontSize;
                    Legend.MapLegendTitle.Style.Format = Maplegend.MapLegendTitle.Style.Format;
                    Legend.MapLegendTitle.Style.Color = Maplegend.MapLegendTitle.Style.Color;
                    Legend.MapLegendTitle.Style.FontWeight = Maplegend.MapLegendTitle.Style.FontWeight;
                    Legend.MapLegendTitle.Style.FontStyle = Maplegend.MapLegendTitle.Style.FontStyle;
                    Legend.MapLegendTitle.Style.BackgroundColor = Maplegend.MapLegendTitle.Style.BackgroundColor;
                    Legend.AutoFitTextDisabled = Maplegend.AutoFitTextDisabled;
                    Legend.DockOutsideViewport = Maplegend.DockOutsideViewport;
                    Legend.EquallySpacedItems = Maplegend.EquallySpacedItems;
                    Legend.Hidden = Maplegend.Hidden;
                    Legend.InterlacedRows = Maplegend.InterlacedRows;
                    Legend.InterlacedRowsColor = Maplegend.InterlacedRowsColor;
                    Legend.Position = Maplegend.Position;
                    Legend.Layout = Maplegend.Layout;

                    Legend.LeftMargin = Maplegend.LeftMargin;
                    Legend.RightMargin = Maplegend.RightMargin;
                    Legend.TopMargin = Maplegend.TopMargin;
                    Legend.BottomMargin = Maplegend.BottomMargin;

                    Map.MapLegends.Add(Legend);
                }
            }

            #endregion

            #region maptitles
            if (this.MapTitles != null)
            {
                Map.MapTitles = new RDL.DOM.MapTitles();
                foreach (RDL.DOM.MapTitle Maptitle in this.MapTitles)
                {
                    RDL.DOM.MapTitle Title = new RDL.DOM.MapTitle();
                    Title.Style = new RDL.DOM.Style();
                    Title.Style.Border = new RDL.DOM.Border();
                    Title.Name = Maptitle.Name;
                    Title.Text = Maptitle.Text;
                    Title.Angle = Maptitle.Angle;
                    Title.TextShadowOffset = Maptitle.TextShadowOffset;
                    Title.DockOutsideViewport = Maptitle.DockOutsideViewport;
                    Title.Position = Maptitle.Position;
                    Title.LeftMargin = Maptitle.LeftMargin;
                    Title.RightMargin = Maptitle.RightMargin;
                    Title.TopMargin = Maptitle.TopMargin;
                    Title.BottomMargin = Maptitle.BottomMargin;
                    Title.DockOutsideViewport = Maptitle.DockOutsideViewport;
                    Title.Style.Border.Color = Maptitle.Style.Border.Color;
                    Title.Style.Border.Style = Maptitle.Style.Border.Style;
                    Title.Style.Border.Width = Maptitle.Style.Border.Width;
                    Title.Style.FontSize = Maptitle.Style.FontSize;
                    Title.Style.Format = Maptitle.Style.Format;
                    Title.Style.Color = Maptitle.Style.Color;
                    Title.Style.BackgroundGradientType = Maptitle.Style.BackgroundGradientType;
                    Title.Style.BackgroundGradientEndColor = Maptitle.Style.BackgroundGradientEndColor;
                    Title.Style.FontWeight = Maptitle.Style.FontWeight;
                    Title.Style.TextAlign = Maptitle.Style.TextAlign;
                    Title.Style.ShadowOffset = Maptitle.Style.ShadowOffset;

                    Map.MapTitles.Add(Title);
                }
            }

            #endregion

            #region distancescale

            if (this.DistanceScale != null)
            {
                Map.MapDistanceScale = new RDL.DOM.MapDistanceScale();
                Map.MapDistanceScale.DockOutsideViewport = this.DistanceScale.DockOutsideViewport;
                Map.MapDistanceScale.Hidden = this.DistanceScale.Hidden;
                Map.MapDistanceScale.MapLocation = this.DistanceScale.MapLocation;
                Map.MapDistanceScale.ScaleBorderColor = this.DistanceScale.ScaleBorderColor;
                Map.MapDistanceScale.ScaleColor = this.DistanceScale.ScaleColor;
                Map.MapDistanceScale.LeftMargin = this.DistanceScale.LeftMargin;
                Map.MapDistanceScale.RightMargin = this.DistanceScale.RightMargin;
                Map.MapDistanceScale.TopMargin = this.DistanceScale.TopMargin;
                Map.MapDistanceScale.BottomMargin = this.DistanceScale.BottomMargin;
                Map.MapDistanceScale.Style = new RDL.DOM.Style();
                Map.MapDistanceScale.Style.Border = new RDL.DOM.Border();

                Map.MapDistanceScale.Style.Border.Color = this.DistanceScale.Style.Border.Color;
                Map.MapDistanceScale.Style.Border.Style = this.DistanceScale.Style.Border.Style;
                Map.MapDistanceScale.Style.Border.Width = this.DistanceScale.Style.Border.Width;
                Map.MapDistanceScale.Style.FontSize = this.DistanceScale.Style.FontSize;
                Map.MapDistanceScale.Style.Format = this.DistanceScale.Style.Format;
                Map.MapDistanceScale.Style.Color = this.DistanceScale.Style.Color;
                Map.MapDistanceScale.Style.BackgroundGradientType = this.DistanceScale.Style.BackgroundGradientType;
                Map.MapDistanceScale.Style.BackgroundGradientEndColor = this.DistanceScale.Style.BackgroundGradientEndColor;
                Map.MapDistanceScale.Style.FontWeight = this.DistanceScale.Style.FontWeight;
                Map.MapDistanceScale.Style.TextAlign = this.DistanceScale.Style.TextAlign;
                Map.MapDistanceScale.Style.ShadowOffset = this.DistanceScale.Style.ShadowOffset;
                Map.MapDistanceScale.MapSize = new RDL.DOM.MapSize();
                Map.MapDistanceScale.MapSize.Height = this.DistanceScale.MapSize.Height;
                Map.MapDistanceScale.MapSize.Width = this.DistanceScale.MapSize.Width;
                Map.MapDistanceScale.MapSize.Unit = this.DistanceScale.MapSize.Unit;
            }
            #endregion

            #region colorscale
            if (this.ColorScale != null)
            {
                Map.MapColorScale = new RDL.DOM.MapColorScale();
                Map.MapColorScale.ColorBarBorderColor = this.ColorScale.ColorBarBorderColor;
                Map.MapColorScale.DockOutsideViewport = this.ColorScale.DockOutsideViewport;
                Map.MapColorScale.Hidden = this.ColorScale.Hidden;
                Map.MapColorScale.HideEndLabels = this.ColorScale.HideEndLabels;
                Map.MapColorScale.LabelFormat = this.ColorScale.LabelFormat;
                Map.MapColorScale.LabelInterval = this.ColorScale.LabelInterval;

                Map.MapColorScale.LabelPlacement = this.ColorScale.LabelPlacement;
                Map.MapColorScale.MapColorScaleTitle = new RDL.DOM.MapColorScaleTitle();
                Map.MapColorScale.MapColorScaleTitle.Caption = this.ColorScale.MapColorScaleTitle.Caption;
                Map.MapColorScale.MapColorScaleTitle.Style = new RDL.DOM.Style();
                Map.MapColorScale.MapColorScaleTitle.Style.FontSize = this.ColorScale.MapColorScaleTitle.Style.FontSize;
                Map.MapColorScale.MapColorScaleTitle.Style.FontWeight = this.ColorScale.MapColorScaleTitle.Style.FontWeight;
                Map.MapColorScale.NoDataText = this.ColorScale.NoDataText;
                Map.MapColorScale.RangeGapColor = this.ColorScale.RangeGapColor;
                Map.MapColorScale.Style = new RDL.DOM.Style();
                Map.MapColorScale.Style.Border = new RDL.DOM.Border();
                Map.MapColorScale.Style.Border.Width = this.ColorScale.Style.Border.Width;
                Map.MapColorScale.Style.Border.Style = this.ColorScale.Style.Border.Style;
                Map.MapColorScale.Style.Border.Color = this.ColorScale.Style.Border.Color;

                Map.MapColorScale.Style.BackgroundColor = this.ColorScale.Style.BackgroundColor;
                Map.MapColorScale.Style.BackgroundGradientEndColor = this.ColorScale.Style.BackgroundGradientEndColor;
                if (this.ColorScale.Style.BackgroundGradientType !=  RDL.DOM.BackgroundGradientTypes.Default)
                {
                    Map.MapColorScale.Style.BackgroundGradientType = this.ColorScale.Style.BackgroundGradientType;
                }
                Map.MapColorScale.Style.FontSize = this.ColorScale.Style.FontSize;
                Map.MapColorScale.Style.FontWeight = this.ColorScale.Style.FontWeight;
                Map.MapColorScale.Style.ShadowOffset = this.ColorScale.Style.ShadowOffset;
                Map.MapColorScale.LeftMargin = this.ColorScale.LeftMargin;
                Map.MapColorScale.RightMargin = this.ColorScale.RightMargin;
                Map.MapColorScale.TopMargin = this.ColorScale.TopMargin;
                Map.MapColorScale.BottomMargin = this.ColorScale.BottomMargin;
            }

            #endregion

            #region borderskin
            if (this.BorderSkin != null)
            {
                Map.MapBorderSkin = new RDL.DOM.MapBorderSkin();
                if (Map.MapBorderSkin.MapBorderSkinType !=  RDL.DOM.BorderSkinType.None)
                {
                    Map.MapBorderSkin.MapBorderSkinType = this.BorderSkin.MapBorderSkinType;
                }
                this.BorderSkin.Style = new RDL.DOM.Style();
                this.BorderSkin.Style.Border = new RDL.DOM.Border();
                Map.MapBorderSkin.Style.Border.Color = this.BorderSkin.Style.Border.Color;
                Map.MapBorderSkin.Style.Border.Style = this.BorderSkin.Style.Border.Style;
                Map.MapBorderSkin.Style.Border.Width = this.BorderSkin.Style.Border.Width;
                Map.MapBorderSkin.Style.BackgroundColor = this.BorderSkin.Style.BackgroundColor;
                Map.MapBorderSkin.Style.BackgroundGradientEndColor = this.BorderSkin.Style.BackgroundGradientEndColor;
                Map.MapBorderSkin.Style.BackgroundGradientType = this.BorderSkin.Style.BackgroundGradientType;
                Map.MapBorderSkin.Style.Color = this.BorderSkin.Style.Color;

            }

            #endregion

            return Map;
        }
        #endregion
    }
}
