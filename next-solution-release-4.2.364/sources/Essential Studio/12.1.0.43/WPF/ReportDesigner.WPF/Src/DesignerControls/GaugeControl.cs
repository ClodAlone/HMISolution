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
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;


namespace Syncfusion.Windows.Reports.Designer.Controls
{
    internal enum GaugeChild
    {
        GaugeScale,
        GaugePointer,
        GaugeRange,
        GaugeProperties,
        GaugePanel
    }

#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Interaction logic for Gauge Control
    /// </summary>
    [TemplatePart(Name = "PART_InternalGauge", Type = typeof(CircularGauge))]
    internal class GaugeControl : Control, IReportItemControl, INotifyPropertyChanged
    {
        # region Variables

        //private string TagName = "Gauge";
        private string Str = string.Empty;
        private string pointerName = string.Empty;

        private double _radius = 0.74;
        private double pointervalue = 0;
        private double _Radius;
        //private double GaugeNewRadius;
        //private double GaugeOldRadius;

        private int scalecount = 0;
        private int radialgaugecount = 0;
        public int radialpointercount = 0;
        private int scalerangecount = 0;
        //private int radialscalecount = 0;
        //private int addpointercount = 0;
        //private int pointerindex = 0;
        private int rangeStartValue = 0;

        private bool isFocusedItem;
        private bool isItemSelected;

        private object propertyOldValue = null;

        private System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();        
        internal Editors.GaugeProperties Properties;
        internal ReportingConvertorUtil propertyValueConvertor;
        private string error_title;

        # endregion

        # region Properties Exposed

        internal DesignerDashStyleBorder GaugePanel { get; set; }

        internal CircularGauge InternalGauge { get; set; }

        internal CircularScale SeriesScale { get; set; }

        internal CircularLabelTick MajorLabelTick { get; set; }

        internal CircularMarkTick MajorTick { get; set; }

        internal CircularMarkTick MinorTick { get; set; }

        internal CircularPointer MemberValuePointer { get; set; }

        internal CircularRange MemberRange { get; set; }

        internal MenuItem AddPointer { get; set; }

        internal MenuItem AddScale { get; set; }

        internal MenuItem AddRange { get; set; }

        internal System.Windows.Controls.Separator seperator1 { get; set; }

        internal System.Windows.Controls.Separator seperator2 { get; set; }

        internal MenuItem DeleteGauge { get; set; }

        internal MenuItem DeleteScale { get; set; }

        internal MenuItem DeletePointer { get; set; }

        internal MenuItem DeleteRange { get; set; }

        internal MenuItem ScaleProperties { get; set; }

        internal MenuItem PointerProperties { get; set; }

        internal MenuItem RangeProperties { get; set; }

        internal MenuItem GaugeProperties { get; set; }

        internal MenuItem GaugePanelProperties { get; set; }

        public StackPanel ValuePanel { get; set; }

        private Border ValueInnerBorder { get; set; }

        public Label ValuePanelDataField { get; set; }

        private Grid DataField { get; set; }

        private string GaugeInputValue { get; set; }

        private string DataSetName { get; set; }
        
        internal Syncfusion.RDL.DOM.ReportDefinition Report { get; set; }

        internal Syncfusion.RDL.DOM.DataSets DataSets { get; set; }

        internal Syncfusion.RDL.DOM.DataSources DataSources { get; set; }

        private List<string> DataBindingList { get; set; }

        public double Gaugeradius
        {
            get
            {
                return ActualHeight / 2;
            }
        }

        internal double Radius
        {
            get
            {
                if (this.InternalGauge != null)
                {
                    return this.InternalGauge.Radius;
                }
                else
                {
                    return _Radius;
                }
            }
            set
            {
                if (this.InternalGauge != null)
                {
                    this.InternalGauge.Radius = value;
                }
                else
                {
                    this._Radius = value;
                }
            }
        }

        private List<string> ValueItems
        {
            get
            {
                return this.GetPanelChildrens(this.ValuePanel);
            }
        }

        # endregion

        # region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeControl"/> class.
        /// </summary>
        internal GaugeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GaugeControl), new FrameworkPropertyMetadata(typeof(GaugeControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeControl"/> class.
        /// </summary>
        /// <param name="DataSets">The data sets.</param>
        /// <param name="DataSources">The data sources.</param>
        internal GaugeControl(Syncfusion.RDL.DOM.DataSets DataSets, Syncfusion.RDL.DOM.DataSources DataSources)
        {
            this.Properties = new Editors.GaugeProperties();
            this.propertyValueConvertor = new ReportingConvertorUtil();
            this.Properties.PropertyChanged += new PropertyChangedEventHandler(Properties_PropertyChanged);
            this.Properties.PropertyChanging += new PropertyChangingEventHandler(Properties_PropertyChanging);
            this.PropertyChanged += new PropertyChangedEventHandler(GaugeControl_PropertyChanged);
            this.DataSets = DataSets;
            this.DataSources = DataSources;
            this.error_title = SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner");
        }
        
        # endregion

        private void SetGaugeRadius(double width, double height)
        {
            if (this.InternalGauge != null)
            {
                double circleRadius = width / 2;
                double additionalSpace = height - width;

                double leftSpace = 0;
                double rightSpace = 0;
                double topSpace = additionalSpace / 2;
                double bottomSpace = additionalSpace / 2;

                if (height < width)
                {
                    circleRadius = height / 2;
                    additionalSpace = width - height;

                    leftSpace = additionalSpace / 2;
                    rightSpace = additionalSpace / 2;
                    topSpace = 0;
                    bottomSpace = 0;
                }

                this.InternalGauge.Margin = new Thickness(leftSpace, topSpace, rightSpace, bottomSpace);
                this.InternalGauge.Radius = circleRadius;
            }
        }

        #region ReportItemControl Interface

        public Syncfusion.RDL.DOM.ReportItem ReportItem
        {
            get;
            set;
        }

        public DesignPanel Panel
        {
            get;
            set;
        }

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
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Height = propertyValueConvertor.GetSizeValue(value, this.Properties.Height);
                    this.Properties.IsInternalPropertyChange = false;
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
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Width = propertyValueConvertor.GetSizeValue(value, this.Properties.Width);
                    this.Properties.IsInternalPropertyChange = false;
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
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Top = propertyValueConvertor.GetSizeValue(value, this.Properties.Top);
                    this.Properties.IsInternalPropertyChange = false;
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
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Left = propertyValueConvertor.GetSizeValue(value, this.Properties.Left);
                    this.Properties.IsInternalPropertyChange = false;
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
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.Properties, IsSelected = value });
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


        public new Canvas Parent { get; set; }

        public DrawingReportItem ItemType
        {
            get
            {
                return DrawingReportItem.Gauge;
            }
        }

        public event ReportItemControlSizeHandler ReportItemSizeChanged;

        public void RaiseReportItemSizeChangedEvent()
        {
            if (this.ReportItemSizeChanged != null)
            {
                this.ReportItemSizeChanged(this, new EventArgs());
                SetGaugeRadius(ActualWidth, ActualHeight);
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
            Syncfusion.RDL.DOM.GaugePanel gauge = new Syncfusion.RDL.DOM.GaugePanel();
            gauge.Name = this.Properties.Name;
            gauge.Visibility = new RDL.DOM.Visibility();
            if (this.Properties.ToggleItem != null)
            {
                gauge.Visibility.ToggleItem = this.Properties.ToggleItem;
            }
            if (this.Properties.Hidden!=null && (this.Properties.Hidden == "True" || this.Properties.Hidden.StartsWith("=")))
            {
                gauge.Visibility.Hidden = this.Properties.Hidden;
            }
            if (!string.IsNullOrEmpty(this.Properties.DocumentMapLabel))
            {
                gauge.DocumentMapLabel = this.Properties.DocumentMapLabel;
            }
            if (this.Properties.PageBreak != RDL.DOM.BreakLocation.None)
            {
                gauge.PageBreak = new RDL.DOM.PageBreak();
                gauge.PageBreak.BreakLocation = this.Properties.PageBreak;
            }
            if (!string.IsNullOrEmpty(this.Properties.DataElementName))
            {
                gauge.DataElementName = this.Properties.DataElementName;
            }
            if (this.Properties.DataElementOutput!=null && this.Properties.DataElementOutput != "Auto")
            {
                gauge.DataElementOutput = (RDL.DOM.DataElementOutputs)Enum.Parse(typeof(RDL.DOM.DataElementOutputs), this.Properties.DataElementOutput);
            }

            gauge.Height = new RDL.DOM.Size(this.ItemHeight / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Height));
            gauge.Width = new RDL.DOM.Size(this.ItemWidth / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Width));

            gauge.Left = new RDL.DOM.Size(this.ItemLeft / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Left));
            gauge.Top = new RDL.DOM.Size(this.ItemTop / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Top));

            this.UpdateGaugeObj(gauge);
            return gauge;
        }

        public void RestoreReportItem(RDL.DOM.ReportItem reportItem)
        {
            this.Properties.IsInternalPropertyChange = true;
            this.ReportItem = reportItem;

            if (this.ReportItem != null)
            {
                this.PopulateReportItem();
                this.ReportItem = null;
            }

            this.Properties.IsInternalPropertyChange = false;
        }

        public void UpdateItemSizeProperties()
        {
            this.Properties.IsInternalPropertyChange = true;
            this.Properties.Width = propertyValueConvertor.GetSizeValue(this.ItemWidth, this.Properties.Width);
            this.Properties.Top = propertyValueConvertor.GetSizeValue(this.ItemTop, this.Properties.Top);
            this.Properties.Left = propertyValueConvertor.GetSizeValue(this.ItemLeft, this.Properties.Left);
            this.Properties.Height = propertyValueConvertor.GetSizeValue(this.ItemHeight, this.Properties.Height);
            this.Properties.IsInternalPropertyChange = false;
        }

        #endregion

        #region Deserialization

        public void PopulateReportItem()
        {
            Syncfusion.RDL.DOM.GaugePanel gaugeBase = this.ReportItem as RDL.DOM.GaugePanel;

            this.ValuePanel.Children.Clear();

            this.PointerProperties.Items.Clear();
            this.RangeProperties.Items.Clear();
            this.ScaleProperties.Items.Clear();
            this.AddRange.Items.Clear();
            this.AddPointer.Items.Clear();
            foreach (RDL.DOM.GaugeScale scale in gaugeBase.RadialGauges[0].GaugeScales)
            {
                for (int i = 0; i < scale.GaugePointers.Count; i++)
                {
                    this.PointerProperties.Items.Add(scale.GaugePointers[i].Name);
                }
                for (int i = 0; i < scale.ScaleRanges.Count; i++)
                {
                    this.RangeProperties.Items.Add(scale.GaugePointers[i].Name);
                }
                this.ScaleProperties.Items.Add(scale.Name);
                this.AddRange.Items.Add(scale.Name);
                this.AddPointer.Items.Add(scale.Name);
            }

            this.Properties.Name = gaugeBase.Name;
            if (gaugeBase.Height != null)
            {
                this.Properties.Height = gaugeBase.Height.size;
            }
            if (gaugeBase.Width != null)
            {
                this.Properties.Width = gaugeBase.Width.size;
            }
            if (gaugeBase.Top != null)
            {
                this.Properties.Top = gaugeBase.Top.size;
            }
            if (gaugeBase.Left != null)
            {
                this.Properties.Left = gaugeBase.Left.size;
            }
            this.Properties.DocumentMapLabel = gaugeBase.DocumentMapLabel;
            this.Properties.DataElementName = gaugeBase.DataElementName;

            if (gaugeBase.Visibility != null)
            {
                if (gaugeBase.Visibility.ToggleItem != null)
                    this.Properties.ToggleItem = gaugeBase.Visibility.ToggleItem;
                if (gaugeBase.Visibility.Hidden != null)
                    this.Properties.Hidden = gaugeBase.Visibility.Hidden;
            }
            if (gaugeBase.PageBreak != null)
            {
                this.Properties.PageBreak = gaugeBase.PageBreak.BreakLocation;
            }
            if (gaugeBase.DataElementOutput != RDL.DOM.DataElementOutputs.Auto)
            {
                this.Properties.DataElementOutput = gaugeBase.DataElementOutput.ToString();
            }

            if (gaugeBase.Style != null)
            {
                if (gaugeBase.Style.Border != null)
                {
                    if (gaugeBase.Style.Border.Style != null)
                    {
                        this.Properties.BorderStyle = gaugeBase.Style.Border.Style;
                    }
                    if (gaugeBase.Style.Border.Color != null)
                    {
                        this.Properties.BorderColor = gaugeBase.Style.Border.Color;
                    }
                    if (gaugeBase.Style.Border.Width != null)
                    {
                        this.Properties.BorderWidth = gaugeBase.Style.Border.Width.PixelValue + "pt";
                    }
                }

                this.Properties.BackFill = gaugeBase.Style.BackgroundColor;
            }

            if (gaugeBase != null)
            {
                if (gaugeBase.RadialGauges != null && gaugeBase.RadialGauges.Count != 0)
                {
                    if (gaugeBase.RadialGauges[0].GaugeScales != null)
                    {
                        int pointerCount = 0;
                        int panelCount = 0;
                        int rangeCount = 0;
                        
                        if (this.InternalGauge.Scales.Count != 0)
                        {
                            this.InternalGauge.Scales.RemoveAt(0);
                        }

                        this.UpdateGaugeProperties();
                        this.UpdatePointerValues();

                        for (int scale = 0; scale < gaugeBase.RadialGauges[0].GaugeScales.Count; scale++)
                        {
                            ScalePropertiesChanged(scale);
                            if (gaugeBase.RadialGauges[0].GaugeScales.Count != 0)
                            {
                                if (this.SeriesScale.Pointers.Count != 0 && pointerCount == 0)
                                {
                                    this.SeriesScale.Pointers.RemoveAt(0);
                                    pointerCount++;
                                }
                                if (ValuePanel.Children.Count != 0 && panelCount == 0)
                                {
                                    this.ValuePanel.Children.RemoveAt(0);
                                    panelCount++;
                                }
                                for (int pointer = 0; pointer < gaugeBase.RadialGauges[0].GaugeScales[scale].GaugePointers.Count; pointer++)
                                {
                                    if (gaugeBase.RadialGauges[0].GaugeScales[scale].GaugePointers.Count != 0)
                                    {
                                        this.MemberValuePointer = new CircularPointer();
                                        this.UpdatePointerProperties(scale, pointer);
                                        this.SeriesScale.Pointers.Add(MemberValuePointer);
                                    }
                                }
                                if (this.SeriesScale.Ranges.Count != 0 && rangeCount == 0)
                                {
                                    this.SeriesScale.Ranges.RemoveAt(0);
                                    rangeCount++;
                                }
                                for (int range = 0; range < gaugeBase.RadialGauges[0].GaugeScales[scale].ScaleRanges.Count; range++)
                                {
                                    if (gaugeBase.RadialGauges[0].GaugeScales[scale].ScaleRanges.Count != 0)
                                    {
                                        this.MemberRange = new CircularRange();
                                        this.UpdateRangeProperties(scale, range);
                                        this.SeriesScale.Ranges.Add(MemberRange);
                                    }
                                }

                                this.InternalGauge.Scales.Add(SeriesScale);
                                this.AddScaleToGauge();
                            }
                        }
                    }


                    this.InternalGauge.SecondFrameThickness = new Thickness(0);
                    this.InternalGauge.EnableEffects = false;
                }

                switch (gaugeBase.RadialGauges[0].BackFrame.FrameShape)
                {
                    case Syncfusion.RDL.DOM.FrameShape.CustomCircular1:
                        {
                            this.Properties.Type = "Circular1";
                            break;
                        }
                    case Syncfusion.RDL.DOM.FrameShape.CustomCircular2:
                        {
                            this.Properties.Type = "Circular2";
                            break;
                        }
                    case Syncfusion.RDL.DOM.FrameShape.CustomCircular3:
                        {
                            this.Properties.Type = "Circular3";
                            break;
                        }
                    case Syncfusion.RDL.DOM.FrameShape.CustomCircular7:
                        {
                            this.Properties.Type = "Circular4";
                            break;
                        }
                }                

                if (gaugeBase.DataSetName != null)
                {
                    this.DataSetName = gaugeBase.DataSetName;
                }

                this.Properties.Dataset = this.DataSetName;
            }
        }

        private void ScalePropertiesChanged(int scale)
        {
            Syncfusion.RDL.DOM.GaugePanel gaugeBase = this.ReportItem as RDL.DOM.GaugePanel;

            if (gaugeBase.RadialGauges[0].GaugeScales.Count != 0)
            {
                Syncfusion.RDL.DOM.RadialScale radialScale = gaugeBase.RadialGauges[0].GaugeScales[scale] as Syncfusion.RDL.DOM.RadialScale;

                if (radialScale.MinimumValue != null)
                {
                    if (radialScale.MinimumValue.Value == string.Empty)
                    {
                        radialScale.MinimumValue.Value = "0";
                    }
                }
                else
                {
                    radialScale.MinimumValue = new Syncfusion.RDL.DOM.MinimumValue();
                    radialScale.MinimumValue.Value = "0";
                }

                if (radialScale.MaximumValue != null)
                {
                    if (radialScale.MaximumValue.Value == string.Empty)
                    {
                        radialScale.MaximumValue.Value = "100";
                    }

                }

                if (radialScale.Multiplier != 0)
                {
                    this.SeriesScale.Minimum = Convert.ToDouble(radialScale.MinimumValue.Value) * (radialScale.Multiplier);
                    this.SeriesScale.Maximum = Convert.ToDouble(radialScale.MaximumValue.Value) * (radialScale.Multiplier);
                }
                else
                {
                    this.SeriesScale.Minimum = Convert.ToDouble(radialScale.MinimumValue.Value);
                    this.SeriesScale.Maximum = Convert.ToDouble(radialScale.MaximumValue.Value);
                }

                this.SeriesScale.MajorIntervalValue = (this.SeriesScale.Maximum - this.SeriesScale.Minimum) / 10;
                this.SeriesScale.MinorIntervalValue = this.SeriesScale.MajorIntervalValue / 4;
                this.SeriesScale.Radius = this.InternalGauge.Radius * ((radialScale.Radius * 2) / 100);

                if (radialScale.ScaleLabels != null)
                {
                    switch (radialScale.ScaleLabels.Placement)
                    {
                        case RDL.DOM.Placement.Inside:
                            {
                                this.MajorLabelTick.TickPlacement = ScalePlacement.Inside;
                                break;
                            }

                        case RDL.DOM.Placement.Cross:
                            {
                                this.MajorLabelTick.TickPlacement = ScalePlacement.Cross;
                                break;
                            }

                        case RDL.DOM.Placement.Outside:
                            {
                                this.MajorLabelTick.TickPlacement = ScalePlacement.Outside;
                                break;
                            }
                    }

                    if (radialScale.ScaleLabels.Style == null)
                    {
                        radialScale.ScaleLabels.Style = new Syncfusion.RDL.DOM.Style();
                    }
                    this.MajorLabelTick.FontSize = Convert.ToDouble(radialScale.ScaleLabels.Style.FontSize.FloatValue);
                    this.MajorLabelTick.Angle = radialScale.ScaleLabels.FontAngle;
                    this.MajorLabelTick.DistanceFromScale = radialScale.ScaleLabels.DistanceFromScale;

                    if (radialScale.ScaleLabels.Style.Color == "LightGray")
                    {
                        radialScale.ScaleLabels.Style.Color = Brushes.LightGray.ToString();
                    }
                    this.MajorLabelTick.BackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(radialScale.ScaleLabels.Style.Color));
                }
                else
                {
                    radialScale.ScaleLabels = new Syncfusion.RDL.DOM.ScaleLabels();
                    radialScale.ScaleLabels.Style = new Syncfusion.RDL.DOM.Style();
                }

                if (radialScale.GaugeMajorTickMarks != null)
                {
                    this.MajorTick.TickHeight = radialScale.GaugeMajorTickMarks.Length;
                    this.MajorTick.TickWidth = radialScale.GaugeMajorTickMarks.Width;

                    if (radialScale.GaugeMajorTickMarks.Style == null)
                    {
                        radialScale.GaugeMajorTickMarks.Style = new Syncfusion.RDL.DOM.Style();
                    }

                    if (radialScale.GaugeMajorTickMarks.Style.BackgroundColor == "LightGray")
                    {
                        radialScale.GaugeMajorTickMarks.Style.BackgroundColor = Brushes.LightGray.ToString();
                    }
                    this.MajorTick.BackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(radialScale.GaugeMajorTickMarks.Style.BackgroundColor));

                    switch (radialScale.GaugeMajorTickMarks.Shape)
                    {
                        case RDL.DOM.Shape.Rectangle:
                            {
                                this.MajorTick.TickShape = TickShape.Rectangle;
                                break;
                            }

                        case RDL.DOM.Shape.Wedge:
                            {
                                this.MajorTick.TickShape = TickShape.RoundedRectangle;
                                break;
                            }

                        case RDL.DOM.Shape.Circle:
                            {
                                this.MajorTick.TickShape = TickShape.Ellipse;
                                break;
                            }

                        case RDL.DOM.Shape.Triangle:
                            {
                                this.MajorTick.TickShape = TickShape.Triangle;
                                break;
                            }
                        default:
                            {
                                radialScale.GaugeMajorTickMarks.Shape = RDL.DOM.Shape.Rectangle;
                                this.MajorTick.TickShape = TickShape.Rectangle;
                                break;
                            }
                    }

                    switch (radialScale.GaugeMajorTickMarks.Placement)
                    {
                        case RDL.DOM.Placement.Cross:
                            {
                                this.MajorTick.TickPlacement = ScalePlacement.Cross;
                                break;
                            }

                        case RDL.DOM.Placement.Inside:
                            {
                                this.MajorTick.TickPlacement = ScalePlacement.Inside;
                                break;
                            }

                        case RDL.DOM.Placement.Outside:
                            {
                                this.MajorTick.TickPlacement = ScalePlacement.Outside;
                                break;
                            }
                    }
                }
                else
                {
                    radialScale.GaugeMajorTickMarks = new Syncfusion.RDL.DOM.GaugeMajorTickMarks();
                    radialScale.GaugeMajorTickMarks.Style = new Syncfusion.RDL.DOM.Style();
                }

                if (radialScale.GaugeMinorTickMarks != null)
                {
                    this.MinorTick.TickHeight = radialScale.GaugeMinorTickMarks.Length;
                    this.MinorTick.TickWidth = radialScale.GaugeMinorTickMarks.Width;

                    if (radialScale.GaugeMinorTickMarks.Style == null)
                    {
                        radialScale.GaugeMinorTickMarks.Style = new Syncfusion.RDL.DOM.Style();
                    }

                    if (radialScale.GaugeMinorTickMarks.Style.BackgroundColor == "LightGray")
                    {
                        radialScale.GaugeMinorTickMarks.Style.BackgroundColor = Brushes.LightGray.ToString();
                    }
                    this.MinorTick.BackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(radialScale.GaugeMinorTickMarks.Style.BackgroundColor));

                    switch (radialScale.GaugeMajorTickMarks.Shape)
                    {
                        case RDL.DOM.Shape.Rectangle:
                            {
                                this.MinorTick.TickShape = TickShape.Rectangle;
                                break;
                            }

                        case RDL.DOM.Shape.Wedge:
                            {
                                this.MinorTick.TickShape = TickShape.RoundedRectangle;
                                break;
                            }

                        case RDL.DOM.Shape.Circle:
                            {
                                this.MinorTick.TickShape = TickShape.Ellipse;
                                break;
                            }

                        case RDL.DOM.Shape.Triangle:
                            {
                                this.MinorTick.TickShape = TickShape.Triangle;
                                break;
                            }
                        default:
                            {
                                radialScale.GaugeMajorTickMarks.Shape = RDL.DOM.Shape.Rectangle;
                                this.MinorTick.TickShape = TickShape.Rectangle;
                                break;
                            }
                    }

                    switch (radialScale.GaugeMinorTickMarks.Placement)
                    {
                        case RDL.DOM.Placement.Cross:
                            {
                                this.MinorTick.TickPlacement = ScalePlacement.Cross;
                                break;
                            }

                        case RDL.DOM.Placement.Inside:
                            {
                                this.MinorTick.TickPlacement = ScalePlacement.Inside;
                                break;
                            }

                        case RDL.DOM.Placement.Outside:
                            {
                                this.MinorTick.TickPlacement = ScalePlacement.Outside;
                                break;
                            }
                    }
                }
                else
                {
                    radialScale.GaugeMinorTickMarks = new Syncfusion.RDL.DOM.GaugeMinorTickMarks();
                    radialScale.GaugeMinorTickMarks.Style = new Syncfusion.RDL.DOM.Style();
                }

                if (this.MemberValuePointer != null)
                {
                    if (radialScale.GaugePointers.Count != 0)
                    {
                        if ((gaugeBase.RadialGauges[0].GaugeScales[0].GaugePointers[0] as Syncfusion.RDL.DOM.RadialPointer).Type == RDL.DOM.RadialPointerType.Marker)
                        {
                            this.MemberValuePointer.PointerLength = radialScale.GaugePointers[0].MarkerLength;
                        }
                        else
                        {
                            this.MemberValuePointer.PointerLength = this.InternalGauge.Radius * ((radialScale.Radius * 2) / 100);
                        }
                    }
                }

                gaugeBase.RadialGauges[0].GaugeScales[0] = radialScale;
            }
        }


        private void UpdateGaugeProperties()
        {
            Syncfusion.RDL.DOM.GaugePanel gaugeBase = this.ReportItem as RDL.DOM.GaugePanel;
            this.Properties.Name = gaugeBase.Name;
            Syncfusion.RDL.DOM.RadialGauge radialGauge = gaugeBase.RadialGauges[0];

            if (radialGauge.BackFrame.FrameBackground.Style == null)
            {
                radialGauge.BackFrame.FrameBackground.Style = new Syncfusion.RDL.DOM.Style();
            }

            if (radialGauge.BackFrame.FrameBackground.Style.BackgroundColor != "LightGray")
            {
                this.Properties.BackFill = radialGauge.BackFrame.FrameBackground.Style.BackgroundColor;
            }

            this.Properties.Thickness = Convert.ToString(radialGauge.BackFrame.FrameWidth) + "pt";
            this.InternalGauge.FirstFrameThickness = new Thickness(radialGauge.BackFrame.FrameWidth);
            if (radialGauge.BackFrame.Style == null)
            {
                radialGauge.BackFrame.Style = new Syncfusion.RDL.DOM.Style();
            }

            if (radialGauge.BackFrame.Style.BackgroundColor != "LightGray")
            {
                this.Properties.FrameFill = radialGauge.BackFrame.Style.BackgroundColor;
            }

            switch (radialGauge.BackFrame.FrameShape)
            {
                case RDL.DOM.FrameShape.CustomCircular7:
                    {
                        this.InternalGauge.FrameType = GaugeFrameType.CircularWithDarkOuterFrames;
                        break;
                    }

                case RDL.DOM.FrameShape.CustomCircular1:
                    {
                        this.InternalGauge.FrameType = GaugeFrameType.FullCircle;
                        break;
                    }

                case RDL.DOM.FrameShape.CustomCircular2:
                    {
                        this.InternalGauge.FrameType = GaugeFrameType.CircularWithInnerLeftGradient;
                        break;
                    }

                case RDL.DOM.FrameShape.CustomCircular3:
                    {
                        this.InternalGauge.FrameType = GaugeFrameType.CircularWithInnerTopGradient;
                        break;
                    }
                default:
                    {
                        radialGauge.BackFrame.FrameShape = RDL.DOM.FrameShape.CustomCircular1;
                        this.InternalGauge.FrameType = GaugeFrameType.FullCircle;
                        break;
                    }
            }

            gaugeBase.RadialGauges[0] = radialGauge;
        }

        private void UpdateRangeProperties(int scale, int range)
        {
            Syncfusion.RDL.DOM.GaugePanel gaugeBase = this.ReportItem as RDL.DOM.GaugePanel;

            if (gaugeBase.RadialGauges[0].GaugeScales.Count != 0 && gaugeBase.RadialGauges[0].GaugeScales[scale].ScaleRanges.Count != 0)
            {
                Syncfusion.RDL.DOM.ScaleRange scaleRange = gaugeBase.RadialGauges[0].GaugeScales[scale].ScaleRanges[range] as Syncfusion.RDL.DOM.ScaleRange;

                if (scaleRange.StartValue != null)
                {
                    if ((scaleRange.StartValue.Value == string.Empty))
                    {
                        scaleRange.StartValue.Value = "70";
                    }
                }
                else
                {
                    scaleRange.StartValue = new Syncfusion.RDL.DOM.StartValue();
                    scaleRange.StartValue.Value = "70";
                }

                if (scaleRange.EndValue != null)
                {
                    if (scaleRange.EndValue.Value == string.Empty)
                    {
                        scaleRange.EndValue.Value = "100";
                    }
                }
                else
                {
                    scaleRange.EndValue = new Syncfusion.RDL.DOM.EndValue();
                    scaleRange.EndValue.Value = "100";
                }

                this.MemberRange.StartValue = Convert.ToDouble(scaleRange.StartValue.Value);
                this.MemberRange.EndValue = Convert.ToDouble(scaleRange.EndValue.Value);
                this.MemberRange.StartWidth = scaleRange.StartWidth;
                this.MemberRange.EndWidth = scaleRange.EndWidth;

                if (scaleRange.Style == null)
                {
                    scaleRange.Style = new Syncfusion.RDL.DOM.Style();
                }

                if ((scaleRange.Style.BackgroundGradientType == RDL.DOM.BackgroundGradientTypes.None) || (scaleRange.Style.BackgroundGradientType == RDL.DOM.BackgroundGradientTypes.Default))
                {
                    if (scaleRange.Style.BackgroundColor == "LightGray")
                    {
                        scaleRange.Style.BackgroundColor = Brushes.LightGray.ToString();
                    }
                    this.MemberRange.BackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(scaleRange.Style.BackgroundColor));
                }
                else
                {
                    if (scaleRange.Style.BackgroundGradientEndColor == "LightGray")
                    {
                        scaleRange.Style.BackgroundGradientEndColor = Brushes.LightGray.ToString();
                    }
                    this.MemberRange.BackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(scaleRange.Style.BackgroundGradientEndColor));
                }

                this.MemberRange.DistanceFromScale = scaleRange.DistanceFromScale;

                switch (scaleRange.Placement)
                {
                    case RDL.DOM.Placement.Cross:
                        {
                            this.MemberRange.RangePosition = ScalePlacement.Cross;
                            break;
                        }

                    case RDL.DOM.Placement.Inside:
                        {
                            this.MemberRange.RangePosition = ScalePlacement.Inside;
                            break;
                        }

                    case RDL.DOM.Placement.Outside:
                        {
                            this.MemberRange.RangePosition = ScalePlacement.Outside;
                            break;
                        }
                }
                if (scaleRange.Style.Border.Width != null)
                    this.MemberRange.BorderWidth = Convert.ToDouble(scaleRange.Style.Border.Width.PixelValue);

                if (scaleRange.Style.Border.Color == "LightGray")
                {
                    scaleRange.Style.Border.Color = Brushes.LightGray.ToString();
                }
                this.MemberRange.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(scaleRange.Style.Border.Color));

                gaugeBase.RadialGauges[0].GaugeScales[scale].ScaleRanges[range] = scaleRange;
            }
        }

        private void UpdatePointerProperties(int scale, int point)
        {
            Syncfusion.RDL.DOM.GaugePanel gaugeBase = this.ReportItem as RDL.DOM.GaugePanel;

            if (gaugeBase.RadialGauges[0].GaugeScales.Count != 0 && gaugeBase.RadialGauges[0].GaugeScales[scale].GaugePointers.Count != 0)
            {
                Syncfusion.RDL.DOM.RadialPointer radialPointer = gaugeBase.RadialGauges[0].GaugeScales[scale].GaugePointers[point] as Syncfusion.RDL.DOM.RadialPointer;
                this.MemberValuePointer.Name = radialPointer.Name;
                this.MemberValuePointer.PointerWidth = radialPointer.Width;
                this.MemberValuePointer.PointerNeedleType = (PointerNeedleType)radialPointer.Type;
                double Number;
                bool isNumber = double.TryParse(radialPointer.GaugeInputValue.Value.Trim(), out Number);
                if (isNumber)
                {
                    this.MemberValuePointer.Value = Convert.ToDouble(radialPointer.GaugeInputValue.Value);
                }
                else
                {
                    this.MemberValuePointer.Value = pointervalue;
                }

                if (radialPointer.Type == RDL.DOM.RadialPointerType.Marker)
                {
                    this.MemberValuePointer.PointerLength = radialPointer.MarkerLength;
                }
                else
                {
                    this.MemberValuePointer.PointerLength = this.InternalGauge.Radius * (((gaugeBase.RadialGauges[0].GaugeScales[scale] as Syncfusion.RDL.DOM.RadialScale).Radius * 2) / 100);
                }

                this.MemberValuePointer.PointerPlacement = (ScalePlacement)radialPointer.Placement;
                this.SeriesScale.PointerCap.PointerCapRadius = (radialPointer.PointerCap.Width / 6);

                if (radialPointer.PointerCap.Style == null)
                {
                    radialPointer.PointerCap.Style = new Syncfusion.RDL.DOM.Style();
                }

                if (radialPointer.PointerCap.Style.BackgroundColor == "LightGray")
                {
                    radialPointer.PointerCap.Style.BackgroundColor = Brushes.LightGray.ToString();
                }
                this.SeriesScale.PointerCap.BackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(radialPointer.PointerCap.Style.BackgroundColor));

                if (radialPointer.Style == null)
                {
                    radialPointer.Style = new Syncfusion.RDL.DOM.Style();
                }
                //if (radialPointer.Style.Border.Width != null)
                //    this.MemberValuePointer.BorderWidth = Convert.ToDouble(radialPointer.Style.Border.Width.PixelValue);

                if (radialPointer.Style.Border.Color == "LightGray")
                {
                    radialPointer.Style.Border.Color = Brushes.LightGray.ToString();
                }
                this.MemberValuePointer.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(radialPointer.Style.Border.Color));

                switch (radialPointer.NeedleStyle)
                {
                    case RDL.DOM.NeedleStyleGauge.Triangular:
                        {
                            this.MemberValuePointer.NeedleStyle = NeedleStyle.Triangle;
                            break;
                        }

                    case RDL.DOM.NeedleStyleGauge.Rectangular:
                        {
                            this.MemberValuePointer.NeedleStyle = NeedleStyle.Rectangle;
                            break;
                        }

                    case RDL.DOM.NeedleStyleGauge.Arrow:
                        {
                            this.MemberValuePointer.NeedleStyle = NeedleStyle.Arrow;
                            break;
                        }
                    default:
                        {
                            radialPointer.NeedleStyle = RDL.DOM.NeedleStyleGauge.Triangular;
                            this.MemberValuePointer.NeedleStyle = NeedleStyle.Triangle;
                            break;
                        }
                }

                radialPointer.PointerCap.OnTop = true;

                switch (radialPointer.MarkerStyle)
                {
                    case RDL.DOM.MarkerStyle.Triangle:
                        {
                            this.MemberValuePointer.MarkerStyle = Syncfusion.Windows.Gauge.MarkerStyle.Triangle;
                            break;
                        }

                    case RDL.DOM.MarkerStyle.Rectangle:
                        {
                            this.MemberValuePointer.MarkerStyle = Syncfusion.Windows.Gauge.MarkerStyle.Rectangle;
                            break;
                        }

                    case RDL.DOM.MarkerStyle.Diamond:
                        {
                            this.MemberValuePointer.MarkerStyle = Syncfusion.Windows.Gauge.MarkerStyle.Diamond;
                            break;
                        }

                    case RDL.DOM.MarkerStyle.Trapezoid:
                        {
                            this.MemberValuePointer.MarkerStyle = Syncfusion.Windows.Gauge.MarkerStyle.Trapezoid;
                            break;
                        }

                    case RDL.DOM.MarkerStyle.Pentagon:
                        {
                            this.MemberValuePointer.MarkerStyle = Syncfusion.Windows.Gauge.MarkerStyle.Pentagon;
                            break;
                        }
                    default:
                        {
                            radialPointer.MarkerStyle = RDL.DOM.MarkerStyle.Triangle;
                            this.MemberValuePointer.MarkerStyle = Syncfusion.Windows.Gauge.MarkerStyle.Triangle;
                            break;
                        }
                }

                if ((radialPointer.Style.BackgroundGradientType == RDL.DOM.BackgroundGradientTypes.None) || (radialPointer.Style.BackgroundGradientType == RDL.DOM.BackgroundGradientTypes.Default))
                {
                    if (radialPointer.Style.BackgroundColor == "LightGray")
                    {
                        radialPointer.Style.BackgroundColor = Brushes.LightGray.ToString();
                    }
                    this.MemberValuePointer.BackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(radialPointer.Style.BackgroundColor));
                }
                else
                {
                    if (radialPointer.Style.BackgroundGradientEndColor == "LightGray")
                    {
                        radialPointer.Style.BackgroundGradientEndColor = Brushes.LightGray.ToString();
                    }
                    this.MemberValuePointer.BackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(radialPointer.Style.BackgroundGradientEndColor));
                }

                gaugeBase.RadialGauges[0].GaugeScales[scale].GaugePointers[point] = radialPointer;

                if (radialPointer.GaugeInputValue.Value != null)
                {
                    Button buttonObj = new Button();
                    buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                    buttonObj.Margin = new Thickness(2);
                    string Str = radialPointer.GaugeInputValue.Value.Trim();
                    double Num;
                    bool isNum = double.TryParse(Str, out Num);
                    if (radialPointer.GaugeInputValue.Value != "" && !isNum)
                        buttonObj.Content = "[" + radialPointer.GaugeInputValue.Value.ToString().Replace("_", "__").ToString().Replace("Fields!", "").Replace(".Value", "").Replace("=", "") + "]";
                    else
                        buttonObj.Content = radialPointer.Name;
                    ValuePanel.Children.Add(buttonObj);
                    //this.MemberValuePointer.Name = buttonObj.Content.ToString();
                }
                pointervalue = pointervalue + 10;
            }
        }

        void UpdatePointerValues()
        {
            if (GaugeInputValue != null)
            {
                Button buttonObj = new Button();
                buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                buttonObj.Margin = new Thickness(2);
                buttonObj.Content = "[" + GaugeInputValue.ToString().Replace("_", "__").ToString().Replace("Fields!", "").Replace(".Value", "").Replace("=", "") + "]";
                ValuePanel.Children.Add(buttonObj);
            }
        }

        #endregion

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            try
            {
                this.GaugePanel = GetTemplateChild("PART_GaugePanel") as DesignerDashStyleBorder;
                this.InternalGauge = GetTemplateChild("PART_InternalGauge") as CircularGauge;
                this.ValuePanel = GetTemplateChild("PART_ValuePanel") as StackPanel;
                this.ValuePanelDataField = GetTemplateChild("PART_ValuePanelField") as Label;
                this.ValuePanelDataField.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(valuePanelDataField_MouseLeftButtonUp);
                this.ValuePanelDataField.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(valuePanelDataField_MouseRightButtonDown);
                this.ValueInnerBorder = GetTemplateChild("PART_ValueInnerBorder") as Border;
                this.AddScale = GetTemplateChild("PART_DataMenuAdd1") as MenuItem;
                this.AddPointer = GetTemplateChild("PART_DataMenuAdd2") as MenuItem;
                this.AddRange = GetTemplateChild("PART_DataMenuAdd3") as MenuItem;
                this.seperator1 = GetTemplateChild("PART_DataMenuSep1") as System.Windows.Controls.Separator;
                this.seperator2 = GetTemplateChild("PART_DataMenuSep2") as System.Windows.Controls.Separator;
                this.DeletePointer = GetTemplateChild("PART_DataMenuDelete2") as MenuItem;
                this.DeleteGauge = GetTemplateChild("PART_DataMenuDelete4") as MenuItem;
                this.DeleteRange = GetTemplateChild("PART_DataMenuDelete1") as MenuItem;
                this.DeleteScale = GetTemplateChild("PART_DataMenuDelete3") as MenuItem;
                this.ScaleProperties = GetTemplateChild("PART_DataMenuProperty1") as MenuItem;
                this.PointerProperties = GetTemplateChild("PART_DataMenuProperty2") as MenuItem;
                this.RangeProperties = GetTemplateChild("PART_DataMenuProperty3") as MenuItem;
                this.GaugeProperties = GetTemplateChild("PART_DataMenuProperty4") as MenuItem;
                this.GaugePanelProperties = GetTemplateChild("PART_DataMenuProperty5") as MenuItem;

                //// To create value panel Button Obj During DeSerialization


                if (this.InternalGauge != null)
                {
                    //  this.InternalGauge.Radius = this._Radius;
                    this.AddScale.Click += new RoutedEventHandler(AddScale_Click);
                    this.AddPointer.Click += new RoutedEventHandler(AddPointer_Click);
                    this.AddRange.Click += new RoutedEventHandler(AddRange_Click);
                    this.ScaleProperties.Click += new RoutedEventHandler(ScaleProperties_Click);
                    this.RangeProperties.Click += new RoutedEventHandler(RangeProperties_Click);
                    this.PointerProperties.Click += new RoutedEventHandler(PointerProperties_Click);
                    this.GaugeProperties.Click += new RoutedEventHandler(GaugeProperties_Click);
                    this.DeleteRange.Click += new RoutedEventHandler(DeleteRange_Click);
                    this.DeletePointer.Click += new RoutedEventHandler(DeletePointerMenu_Click);
                    this.DeleteScale.Click += new RoutedEventHandler(DeleteScale_Click);
                    this.DeleteGauge.Click += new RoutedEventHandler(DeleteGauge_Click);
                    this.GaugePanelProperties.Click += new RoutedEventHandler(GaugePanelProperties_Click);
                    this.ValuePanel.PreviewDragOver += new System.Windows.DragEventHandler(Panel_PreviewDragOver);
                    this.ValuePanel.PreviewDrop += new System.Windows.DragEventHandler(ValuePanel_PreviewDrop);

                    //// If Deserialization means to update Old Gauge else to create new Gauge

                    InitializeGauge();
                }

                this.Properties.IsInternalPropertyChange = true;
                this.Properties.Name = this.ItemName;
                this.Properties.UpdatePropertyValue();

                if (this.ReportItem != null)
                {
                    this.PopulateReportItem();
                }

                this.Properties.IsInternalPropertyChange = false;

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
        }

        void GaugeControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;

            if (propertyName.Equals("IsFocusedItem"))
            {
                if (this.isFocusedItem)
                {
                    this.ValueInnerBorder.Margin = new Thickness(0, -30, 0, 0);
                    this.ValueInnerBorder.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.ValueInnerBorder.Margin = new Thickness(0);
                    this.UpdateLayout();
                    this.ValueInnerBorder.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        void Properties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "BACKFILL":
                    {
                        propertyValue = this.Properties.BackFill;
                        break;
                    }
                case "FRAMEFILL":
                    {
                        propertyValue = this.Properties.FrameFill;
                        break;
                    }
                case "THICKNESS":
                    {
                        propertyValue = this.Properties.Thickness;
                        break;
                    }
                case "TYPE":
                    {
                        propertyValue = this.Properties.Type;
                        break;
                    }
                case "BORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColor;
                        break;
                    }
                case "BORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyle;
                        break;
                    }
                case "BORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidth;
                        break;
                    }
                case "NAME":
                    {
                        propertyValue = this.Properties.Name;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.Properties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.Properties.ToggleItem;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.Properties.DocumentMapLabel;
                        break;
                    }
                case "PAGEBREAK":
                    {
                        propertyValue = this.Properties.PageBreak;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.Properties.Left;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.Properties.Top;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.Properties.Height;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.Properties.Width;
                        break;
                    }
                case "DATAELEMENTNAME":
                    {
                        propertyValue = this.Properties.DataElementName;
                        break;
                    }
                case "DATAELEMENTOUTPUT":
                    {
                        propertyValue = this.Properties.DataElementOutput;
                        break;
                    }
            }
            this.propertyOldValue = propertyValue;
        }

        void Properties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "BACKFILL":
                    {
                        propertyValue = this.Properties.BackFill;
                        this.InternalGauge.Background = this.propertyValueConvertor.GetBackGroundColor(this.Properties.BackFill);
                        break;
                    }
                case "FRAMEFILL":
                    {
                        propertyValue = this.Properties.FrameFill;
                        this.InternalGauge.FirstFrameFillColor = this.propertyValueConvertor.GetBackGroundColor(this.Properties.FrameFill);
                        break;
                    }
                case "THICKNESS":
                    {
                        propertyValue = this.Properties.Thickness;
                        this.InternalGauge.FirstFrameThickness = this.propertyValueConvertor.GetBorderThickness(this.Properties.Thickness);
                        break;
                    }
                case "TYPE":
                    {
                        propertyValue = this.Properties.Type;
                        this.InternalGauge.FrameType = this.propertyValueConvertor.GetGaugeType(this.Properties.Type);
                        break;
                    }
                case "BORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColor;
                        this.GaugePanel.BorderBrush = this.propertyValueConvertor.GetColor(this.Properties.BorderColor);
                        break;
                    }
                case "BORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyle;
                        this.GaugePanel.DashStyle = this.propertyValueConvertor.GetBorderStyle(this.Properties.BorderStyle);
                        break;
                    }
                case "BORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidth;
                        this.GaugePanel.BorderThickness = this.propertyValueConvertor.GetBorderThickness(this.Properties.BorderWidth);
                        break;
                    }
                case "NAME":
                    {
                        propertyValue = this.Properties.Name;
                        this.ItemName = this.Properties.Name;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.Properties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.Properties.ToggleItem;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.Properties.DocumentMapLabel;
                        break;
                    }
                case "PAGEBREAK":
                    {
                        propertyValue = this.Properties.PageBreak;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.Properties.Left;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Left))
                            this.ItemLeft = new RDL.DOM.Size(this.Properties.Left).PixelValue;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.Properties.Top;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Top))
                            this.ItemTop = new RDL.DOM.Size(this.Properties.Top).PixelValue;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.Properties.Height;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Height))
                            this.ItemHeight = new RDL.DOM.Size(this.Properties.Height).PixelValue;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.Properties.Width;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Width))
                            this.ItemWidth = new RDL.DOM.Size(this.Properties.Width).PixelValue;
                        break;
                    }
                case "DATAELEMENTNAME":
                    {
                        propertyValue = this.Properties.DataElementName;
                        break;
                    }
                case "DATAELEMENTOUTPUT":
                    {
                        propertyValue = this.Properties.DataElementOutput;
                        break;
                    }
            }

            if (!this.Properties.IsInternalPropertyChange)
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

        # region Value Panel Related Events

        /// <summary>
        /// While doing click in Smart tag present in the Value panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void valuePanelDataField_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.ValuePanelDataField.Visibility = Visibility.Visible;

            //// If Value panel contains no fields means smart tag will show all the data sources, data sets and data fields present. 
            if (this.ValuePanel.Children.Count == 0)
            {
                this.NoValuePanelChildrens();
            }
            //// present else it will show the data fields of the data set to which the field already added belongs.
            else
            {
                if (this.DataSetName != null && this.DataSetName != string.Empty)
                {
                    if ((this.DataSources != null) && (this.DataSets != null))
                    {
                        if (this.DataSets.Count != 0)
                        {
                            this.ValuePanelDataField.ContextMenu = null;
                            ContextMenu contextMenu = new ContextMenu();
                            MenuItem menuItem = new MenuItem();
                            string str = this.DataSetName;
                            var fields = (from dataSetThis in this.DataSets
                                          where dataSetThis.Name == str
                                          select dataSetThis.Fields).SingleOrDefault();

                            if (fields != null)
                            {
                                foreach (Syncfusion.RDL.DOM.Field field in fields)
                                {
                                    MenuItem menuItem1 = new MenuItem();
                                    menuItem1.Header = field.Name;
                                    menuItem1.Click += new RoutedEventHandler(menuItem1_Click);
                                    contextMenu.Items.Add(menuItem1);
                                }
                            }
                            else
                            {
                                MenuItem menuItem1 = new MenuItem();
                                menuItem1.Header = SR.GetString(CultureInfo.CurrentUICulture, "msgBoxDataSet") + " '" + this.DataSetName.ToString() + "' " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxDataSetNotExistInReport");
                                contextMenu.Items.Add(menuItem1);
                            }

                            this.ValuePanelDataField.ContextMenu = contextMenu;
                            this.ValuePanelDataField.ContextMenu.IsOpen = true;
                        }
                    }
                }
                else
                {
                    this.NoValuePanelChildrens();
                }
            }
        }

        /// <summary>
        /// To disable context menu support for the smart tag present in value panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void valuePanelDataField_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.ValuePanelDataField.ContextMenu != null)
            {
                this.ValuePanelDataField.ContextMenu = null;
            }
        }

        /// <summary>
        /// To trigger add data set event while selecting the option from smart tag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddDataSet_Click(object sender, RoutedEventArgs e)
        {
            this.Panel.AddDataSet();
        }

        /// <summary>
        /// To trigger add data source event while selecting the option from smart tag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddDataSource_Click(object sender, RoutedEventArgs e)
        {
            this.Panel.AddDataSource();
        }

        /// <summary>
        /// To add data field to the value panel while selecting the field from smart tag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItem1_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = this.DataSetName;
            this.CreateValueButtonObj(header, parent, null);
        }

        /// <summary>
        /// To add data field to the value panel while selecting the field from smart tag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItem4_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = this.DataSets[0].Name;
            this.CreateValueButtonObj(header, parent, null);
        }

        /// <summary>
        /// To add data field to the value panel while selecting the field from smart tag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItem3_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            string parent = ((sender as MenuItem).Parent as MenuItem).Header.ToString();
            this.CreateValueButtonObj(header, parent, null);
        }

        /// <summary>
        /// To add data field to the value panel while dropping the field in value panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Panel_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;

            if (itemCollection != null && itemCollection.Count > 0)
            {
                TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;

                if (treeViewItem.Tag != null && treeViewItem.Tag.GetType().Name == "String"&&treeViewItem.Tag.ToString()!="Parameters"&&!treeViewItem.Tag.ToString().StartsWith("#BuiltIn#") && treeViewItem.Tag.ToString() != string.Empty)
                {
                    e.Effects = DragDropEffects.All;
                    e.Handled = true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// To add data field to the value panel while dropping the field in value panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ValuePanel_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            this.Properties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange = change;
            this.Panel.EditingManager.AddAction(action);

            TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
            if (itemCollection.Count > 0)
            {
                TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;

                if (e.Source.GetType().Name == "Button")
                {
                    string pointerName = ((Button)e.Source).Content.ToString();
                    CreateValueButtonObj(treeViewItem.Header.ToString(), treeViewItem.Tag.ToString(), pointerName);
                }
                else
                {
                    //// To find the aggregate function for the particular data field which will be used while creating the button.
                    CreateValueButtonObj(treeViewItem.Header.ToString(), treeViewItem.Tag.ToString(), null);
                }
            }

            change.NewValue = this.GetReportItem();
            this.Properties.IsInternalPropertyChange = false;
        }

        /// <summary>
        /// To remove the button from the value panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void DeletePointerMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Properties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange = change;
            this.Panel.EditingManager.AddAction(action);

            Button buttonObj = ((MenuItem)sender).Tag as Button;
            if (buttonObj != null)
            {
                StackPanel stackPanel = Util.GetParentItem<StackPanel>(buttonObj as DependencyObject) as StackPanel;
                if (stackPanel != null)
                {
                    if (this.ValuePanel.Children.Count > 0)
                    {
                        string orgString = buttonObj.Name.ToString();

                        foreach (CircularScale scale in this.InternalGauge.Scales)
                        {
                            if (scale.Pointers.Count > 0)
                            {
                                for (int i = 0; i < scale.Pointers.Count; i++)
                                {
                                    if (orgString == scale.Pointers[i].Name)
                                    {
                                        this.InternalGauge.Scales.Remove(scale);
                                        scale.Pointers.Remove(scale.Pointers[i]);
                                        this.PointerProperties.Items.RemoveAt(i);
                                        radialpointercount--;
                                        pointervalue -= 10;
                                        stackPanel.Children.Remove((UIElement)buttonObj);
                                        if (scale.Pointers.Count == 0)
                                        {
                                            scale.PointerCap.PointerCapRadius = 0;
                                        }
                                        pointerName = orgString;
                                        if (i < scale.Pointers.Count)
                                        {
                                            for (int j = i; j < scale.Pointers.Count; j++)
                                            {
                                                scale.Pointers[j].Value = scale.Pointers[j].Value - 10;
                                            }
                                        }
                                        this.InternalGauge.Scales.Add(scale);
                                        break;
                                    }
                                }
                                break;
                            }
                        }

                    }
                }
            }

            else
            {
                foreach (CircularScale scale in this.InternalGauge.Scales)
                {
                    if (scale.Pointers.Count > 0)
                    {
                        this.InternalGauge.Scales.Remove(scale);
                        scale.Pointers.RemoveAt(scale.Pointers.Count - 1);
                        this.PointerProperties.Items.RemoveAt(scale.Pointers.Count);
                        radialpointercount--;
                        pointervalue -= 10;
                        this.ValuePanel.Children.RemoveAt(scale.Pointers.Count);
                        if (scale.Pointers.Count == 0)
                        {
                            scale.PointerCap.PointerCapRadius = 0;
                        }
                        this.InternalGauge.Scales.Add(scale);
                        break;
                    }
                }
            }
            this.ChangeContextMenuVisibility();
            change.NewValue = this.GetReportItem();
            this.Properties.IsInternalPropertyChange = false;
        }

        internal void DeletePointer_Click()
        {
            foreach (CircularScale scale in this.InternalGauge.Scales)
            {
                if (scale.Pointers.Count > 0)
                {
                    this.InternalGauge.Scales.Remove(scale);
                    scale.Pointers.RemoveAt(scale.Pointers.Count - 1);
                    radialpointercount--;
                    pointervalue -= 10;
                    this.ValuePanel.Children.RemoveAt(scale.Pointers.Count);
                    if (scale.Pointers.Count == 0)
                    {
                        scale.PointerCap.PointerCapRadius = 0;
                    }
                    this.InternalGauge.Scales.Add(scale);
                    break;
                }
            }
        }

        # endregion

        # region Common Events

        /// <summary>
        /// To remove the range from gauge.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DeleteRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);
                this.DeleteGaugeRange();
                this.ChangeContextMenuVisibility();
                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        internal void DeleteGaugeRange()
        {
            foreach (CircularScale scale in this.InternalGauge.Scales)
            {
                if (scale.Ranges.Count > 0)
                {
                    this.InternalGauge.Scales.Remove(scale);
                    scale.Ranges.RemoveAt(scale.Ranges.Count - 1);
                    this.RangeProperties.Items.RemoveAt(scale.Ranges.Count);
                    scalerangecount--;
                    this.MemberRange = null;
                    this.InternalGauge.Scales.Add(scale);
                    break;
                }
            }
        }

        /// <summary>
        /// To remove the pointer from gauge.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DeletePointer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);
                this.SeriesScale.Pointers.Clear();
                this.MemberValuePointer = null;
                this.SeriesScale.PointerCap.PointerCapRadius = 0;
                this.InternalGauge.Scales.Add(SeriesScale);
                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;

            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        /// <summary>
        /// To remove the scale from gauge.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DeleteScale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);
                this.DeleteGaugeScale();
                this.ChangeContextMenuVisibility();
                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        internal void DeleteGaugeScale()
        {
            foreach (var pointer in this.InternalGauge.Scales.Last().Pointers)
            {
                foreach (var item in this.PointerProperties.Items)
                {
                    if ((item as MenuItem).Header.ToString() == pointer.Name)
                    {
                        this.ValuePanel.Children.RemoveAt(this.PointerProperties.Items.IndexOf(item));
                        this.PointerProperties.Items.Remove(item);
                        radialpointercount--;
                        break;
                    }
                }
            }
            foreach (var range in this.InternalGauge.Scales.Last().Ranges)
            {
                foreach (var item in this.RangeProperties.Items)
                {
                    if (range.Name.ToString() == ((item as MenuItem).Header).ToString())
                    {
                        this.RangeProperties.Items.Remove(item);
                        break;
                    }
                }
            }

            this.InternalGauge.Scales.RemoveAt(this.InternalGauge.Scales.Count - 1);
            this.AddRange.Items.RemoveAt(this.AddPointer.Items.Count - 1);
            this.ScaleProperties.Items.RemoveAt(this.AddPointer.Items.Count - 1);
            this.AddPointer.Items.RemoveAt(this.AddPointer.Items.Count - 1);
            this.scalecount--;
            _radius = _radius + .20;
        }

        /// <summary>
        /// To add the range to gauge.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddRange_Click(object sender, RoutedEventArgs e)
        {
            this.Properties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange = change;
            this.Panel.EditingManager.AddAction(action);
            AddRangeToGauge();
            this.ChangeContextMenuVisibility();
            change.NewValue = this.GetReportItem();
            this.Properties.IsInternalPropertyChange = false;
        }

        /// <summary>
        /// Handles the Click event of the AddPointer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void AddPointer_Click(object sender, RoutedEventArgs e)
        {
            this.Properties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange = change;
            this.Panel.EditingManager.AddAction(action);
            this.AddPointerToGauge();
            this.ChangeContextMenuVisibility();
            this.SeriesScale.PointerCap.PointerCapRadius = this.InternalGauge.Radius / 20;
            change.NewValue = this.GetReportItem();
            this.Properties.IsInternalPropertyChange = false;
        }

        /// <summary>
        /// Handles the Click event of the AddScale control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void AddScale_Click(object sender, RoutedEventArgs e)
        {
            this.Properties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange = change;
            this.Panel.EditingManager.AddAction(action);
            this.AddScaleToGauge();
            this.AddGaugeScale();
            this.ChangeContextMenuVisibility();
            change.NewValue = this.GetReportItem();
            this.Properties.IsInternalPropertyChange = false;
        }

        internal void AddGaugeScale()
        {
            this.InternalGauge.Scales.Add(SeriesScale);
            MenuItem m1 = new MenuItem();
            this.AddPointer.Items.Add(m1);
            MenuItem m2 = new MenuItem();
            MenuItem m3 = new MenuItem();
            m3.Header = m2.Header = m1.Header = "RadialScale" + ++scalecount;
            this.SeriesScale.Name = m3.Header.ToString();
            this.AddRange.Items.Add(m2);
            this.ScaleProperties.Items.Add(m3);
            m1.Click += new RoutedEventHandler(m1_Click);
            m3.Click += new RoutedEventHandler(m1_Click);
            m2.Click += new RoutedEventHandler(m1_Click);
        }

        void m1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //CircularScale scale = sender as CircularScale;
                MenuItem temp = sender as MenuItem;

                string s = temp.Header.ToString();
                string menuname = s.Remove(s.Length - 1, 1);

                if (menuname.ToLower() == "radialscale")
                {
                    foreach (CircularScale scale in this.InternalGauge.Scales)
                    {
                        if (s == scale.Name)
                        {
                            this.SeriesScale = scale;
                            break;

                        }
                    }
                }
                else if (menuname.ToLower() == "radialpointer")
                {
                    foreach (CircularScale scale in this.InternalGauge.Scales)
                    {
                        foreach (CircularPointer pointer in scale.Pointers)
                        {
                            if (s == pointer.Name)
                            {
                                this.MemberValuePointer = pointer;
                                break;
                            }
                        }
                    }
                }
                else if (menuname.ToLower() == "scalerange")
                {
                    foreach (CircularScale scale in this.InternalGauge.Scales)
                    {
                        foreach (CircularRange range in scale.Ranges)
                        {
                            if (s == range.Name)
                            {
                                this.MemberRange = range;
                                break;
                            }
                        }
                    }
                }

            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        /// <summary>
        /// Handles the Click event of the DeleteGauge control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void DeleteGauge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteGaugeRaised();
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }

        }

        /// <summary>
        /// Handles the Click event of the ScaleProperties control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void ScaleProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateScaleProperties();
            if (controlProperties.ShowDialog() == true)
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);

                foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is ScaleGeneral)
                    {
                        this.SeriesScale.Maximum = controlProperties.ScaleGeneral.ScaleMaximumValue.Value.Value;
                        this.SeriesScale.Minimum = controlProperties.ScaleGeneral.ScaleMinimumValue.Value.Value;
                        if (controlProperties.ScaleGeneral.ScaleLabelsMultiple.Value.Value > 1)
                        {
                            this.SeriesScale.Maximum = controlProperties.ScaleGeneral.ScaleLabelsMultiple.Value.Value * controlProperties.ScaleGeneral.ScaleMaximumValue.Value.Value;
                            this.SeriesScale.Minimum = controlProperties.ScaleGeneral.ScaleLabelsMultiple.Value.Value * controlProperties.ScaleGeneral.ScaleMinimumValue.Value.Value;
                        }
                        this.SeriesScale.MajorIntervalValue = (this.SeriesScale.Maximum - this.SeriesScale.Minimum) / 10;
                        this.SeriesScale.MinorIntervalValue = this.SeriesScale.MajorIntervalValue / 4;
                        // this.SeriesScale.Radius = controlProperties.ScaleGeneral.ScaleRadius.Value.Value;
                        this.SeriesScale.StartAngle = (float)controlProperties.ScaleGeneral.ScaleStartAngle.Value;
                        this.SeriesScale.GapSweepAngle = (float)controlProperties.ScaleGeneral.ScaleSweepAngle.Value;
                        //  controlProperties.GaugeGeneral.FrameFill.Color = new SolidColorBrush(this.InternalGauge.Background).Color;

                    }
                    else if (uiElement is ScaleLabel)
                    {
                        switch (controlProperties.ScaleLabel.LabelPlacement.Text.ToLower())
                        {
                            case "inside":
                                {
                                    this.MajorLabelTick.TickPlacement = ScalePlacement.Inside;
                                    break;
                                }

                            case "cross":
                                {
                                    this.MajorLabelTick.TickPlacement = ScalePlacement.Cross;
                                    break;
                                }

                            case "outside":
                                {
                                    this.MajorLabelTick.TickPlacement = ScalePlacement.Outside;
                                    break;
                                }
                        }

                        this.MajorLabelTick.FontSize = controlProperties.ScaleLabel.LabelFont.Value.Value;
                        this.MajorLabelTick.Angle = controlProperties.ScaleLabel.LabelAngle.Value;
                        this.MajorLabelTick.BackgroundBrush = new SolidColorBrush(controlProperties.ScaleLabel.LabelColor.Color);
                    }
                    else if (uiElement is ScaleMajorTick)
                    {
                        this.MajorTick.TickHeight = controlProperties.ScaleMajorTick.MajorTickLength.Value.Value;
                        this.MajorTick.TickWidth = controlProperties.ScaleMajorTick.MajorTickWidth.Value.Value;
                        this.MajorTick.BackgroundBrush = new SolidColorBrush(controlProperties.ScaleMajorTick.MajorTickColor.Color);
                        switch (controlProperties.ScaleMajorTick.MajorTickShape.Text.ToLower())
                        {
                            case "rectangle":
                                {
                                    this.MajorTick.TickShape = TickShape.Rectangle;
                                    break;
                                }

                            case "wede":
                                {
                                    this.MajorTick.TickShape = TickShape.RoundedRectangle;
                                    break;
                                }

                            case "circle":
                                {
                                    this.MajorTick.TickShape = TickShape.Ellipse;
                                    break;
                                }

                            case "triangle":
                                {
                                    this.MajorTick.TickShape = TickShape.Triangle;
                                    break;
                                }
                            default:
                                {

                                    this.MajorTick.TickShape = TickShape.Rectangle;
                                    break;
                                }
                        }

                        switch (controlProperties.ScaleMajorTick.MajorTickPlacement.Text.ToLower())
                        {
                            case "cross":
                                {
                                    this.MajorTick.TickPlacement = ScalePlacement.Cross;
                                    break;
                                }

                            case "inside":
                                {
                                    this.MajorTick.TickPlacement = ScalePlacement.Inside;
                                    break;
                                }

                            case "outside":
                                {
                                    this.MajorTick.TickPlacement = ScalePlacement.Outside;
                                    break;
                                }
                        }
                    }
                    else if (uiElement is ScaleMinorTick)
                    {
                        this.MinorTick.TickHeight = controlProperties.ScaleMinorTick.MinorTickLength.Value.Value;
                        this.MinorTick.TickWidth = controlProperties.ScaleMinorTick.MinorTickWidth.Value.Value;
                        this.MinorTick.BackgroundBrush = new SolidColorBrush(controlProperties.ScaleMinorTick.clrpkr_MinorTickColor.Color);
                        switch (controlProperties.ScaleMinorTick.MinorTickShape.Text)
                        {
                            case "rectangle":
                                {
                                    this.MinorTick.TickShape = TickShape.Rectangle;
                                    break;
                                }

                            case "wede":
                                {
                                    this.MinorTick.TickShape = TickShape.RoundedRectangle;
                                    break;
                                }

                            case "circle":
                                {
                                    this.MinorTick.TickShape = TickShape.Ellipse;
                                    break;
                                }

                            case "triangle":
                                {
                                    this.MinorTick.TickShape = TickShape.Triangle;
                                    break;
                                }
                            default:
                                {
                                    this.MinorTick.TickShape = TickShape.Rectangle;
                                    break;
                                }
                        }

                        switch (controlProperties.ScaleMinorTick.MinorTickPlacement.Text.ToLower())
                        {
                            case "cross":
                                {
                                    this.MinorTick.TickPlacement = ScalePlacement.Cross;
                                    break;
                                }

                            case "inside":
                                {
                                    this.MinorTick.TickPlacement = ScalePlacement.Inside;
                                    break;
                                }

                            case "outside":
                                {
                                    this.MinorTick.TickPlacement = ScalePlacement.Outside;
                                    break;
                                }
                        }

                    }
                }

                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }
        }

        private ControlProperties UpdateScaleProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, GaugeChild.GaugeScale);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.ScaleGeneral.ScaleMaximumValue.Value = this.SeriesScale.Maximum;
            controlProperties.ScaleGeneral.ScaleMinimumValue.Value = this.SeriesScale.Minimum;
            controlProperties.ScaleGeneral.ScaleStartAngle.Value = this.SeriesScale.StartAngle;
            controlProperties.ScaleGeneral.ScaleSweepAngle.Value = this.SeriesScale.GapSweepAngle;
            controlProperties.ScaleLabel.LabelPlacement.Text = this.MajorLabelTick.TickPlacement.ToString();
            controlProperties.ScaleLabel.LabelFont.Value = this.MajorLabelTick.FontSize;
            controlProperties.ScaleLabel.LabelColor.Color = ((SolidColorBrush)this.MajorLabelTick.BackgroundBrush).Color;
            controlProperties.ScaleMajorTick.MajorTickLength.Value = this.MajorTick.TickHeight;
            controlProperties.ScaleMajorTick.MajorTickWidth.Value = this.MajorTick.TickWidth;
            controlProperties.ScaleMajorTick.MajorTickColor.Color = ((SolidColorBrush)this.MajorTick.BackgroundBrush).Color;
            controlProperties.ScaleMajorTick.MajorTickPlacement.Text = this.MajorTick.TickPlacement.ToString();
            controlProperties.ScaleMinorTick.MinorTickLength.Value = this.MinorTick.TickHeight;
            controlProperties.ScaleMinorTick.MinorTickWidth.Value = this.MinorTick.TickWidth;
            controlProperties.ScaleMinorTick.clrpkr_MinorTickColor.Color = ((SolidColorBrush)this.MinorTick.BorderBrush).Color;
            controlProperties.ScaleMinorTick.MinorTickPlacement.Text = this.MinorTick.TickPlacement.ToString();
            return controlProperties;
        }

        /// <summary>
        /// Handles the Click event of the PointerProperties control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void PointerProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdatePointerProperties();

            if (controlProperties.ShowDialog() == true)
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);

                foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is PointerGeneral)
                    {
                        this.MemberValuePointer.PointerWidth = controlProperties.PointerGeneral.PointerWidth.Value.Value;
                        this.MemberValuePointer.Value = Convert.ToDouble(controlProperties.PointerGeneral.PointerValue.Text);

                        switch (controlProperties.PointerGeneral.PointerType.Text.ToLower())
                        {
                            case "bar":
                                {
                                    this.MemberValuePointer.PointerNeedleType = PointerNeedleType.Bar;
                                    break;
                                }
                            case "needle":
                                {
                                    this.MemberValuePointer.PointerNeedleType = PointerNeedleType.Needle;
                                    break;
                                }
                            case "marker":
                                {
                                    this.MemberValuePointer.PointerNeedleType = PointerNeedleType.Marker;
                                    break;
                                }
                        }

                        switch (controlProperties.PointerGeneral.NeedleType.Text.ToLower())
                        {
                            case "triangle":
                                {
                                    this.MemberValuePointer.NeedleStyle = NeedleStyle.Triangle;
                                    break;
                                }

                            case "rectangle":
                                {
                                    this.MemberValuePointer.NeedleStyle = NeedleStyle.Rectangle;
                                    break;
                                }

                            case "arrow":
                                {
                                    this.MemberValuePointer.NeedleStyle = NeedleStyle.Arrow;
                                    break;
                                }
                            default:
                                {
                                    this.MemberValuePointer.NeedleStyle = NeedleStyle.Triangle;
                                    break;
                                }
                        }

                        if (this.MemberValuePointer.PointerNeedleType == PointerNeedleType.Marker)
                        {
                            this.MemberValuePointer.PointerLength = (this.SeriesScale.Radius * 0.74) / 2;
                        }
                        else if (this.MemberValuePointer.PointerLength < this.SeriesScale.Radius)
                        {
                            this.MemberValuePointer.PointerLength = this.SeriesScale.Radius;
                        }

                        this.SeriesScale.PointerCap.BackgroundBrush = new SolidColorBrush(controlProperties.PointerGeneral.clrpkr_CapColor.Color);
                    }
                    else if (uiElement is PointerColorFill)
                    {
                        this.MemberValuePointer.BackgroundBrush = new SolidColorBrush(controlProperties.PointerColorFill.clrpkr_PointerColor.Color);
                        this.MemberValuePointer.BorderBrush = new SolidColorBrush(controlProperties.PointerColorFill.clrpkr_PointerBorderColor.Color);
                        this.MemberValuePointer.BorderThickness = new Thickness(controlProperties.PointerColorFill.PointerBorderWidth.Value.Value);
                    }
                }

                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }
        }

        private ControlProperties UpdatePointerProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, GaugeChild.GaugePointer);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.PointerGeneral.PointerWidth.Value = this.MemberValuePointer.PointerWidth;
            controlProperties.PointerGeneral.PointerType.Text = this.MemberValuePointer.PointerNeedleType.ToString();
            controlProperties.PointerGeneral.NeedleType.Text = this.MemberValuePointer.NeedleStyle.ToString();
            controlProperties.PointerGeneral.CapRadius.Value = this.SeriesScale.PointerCap.PointerCapRadius / 6;
            controlProperties.PointerGeneral.PointerValue.Text = this.MemberValuePointer.Value.ToString();
            controlProperties.PointerColorFill.clrpkr_PointerColor.Color = ((SolidColorBrush)this.MemberValuePointer.BackgroundBrush).Color;
            controlProperties.PointerColorFill.clrpkr_PointerBorderColor.Color = ((SolidColorBrush)this.MemberValuePointer.BorderBrush).Color;
            controlProperties.PointerColorFill.PointerBorderWidth.Value = this.MemberValuePointer.BorderThickness.Top;
            // controlProperties.PointerGeneral.CapRadius.Value = this.SeriesScale.PointerCap.PointerCapRadius / 6;
            return controlProperties;
        }


        /// <summary>
        /// Handles the Click event of the RangeProperties control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void RangeProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateRangeProperties();

            if (controlProperties.ShowDialog() == true)
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);

                foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is RangeGeneral)
                    {
                        this.MemberRange.StartValue = controlProperties.RangeGeneral.RangeStartValue.Value.Value;
                        this.MemberRange.EndValue = controlProperties.RangeGeneral.RangeEndValue.Value.Value;
                        this.MemberRange.StartWidth = controlProperties.RangeGeneral.RangeStartWidth.Value.Value;
                        this.MemberRange.EndWidth = controlProperties.RangeGeneral.RangeEndWidth.Value.Value;
                        this.MemberRange.BackgroundBrush = new SolidColorBrush(controlProperties.RangeGeneral.RangeColor.Color);
                        switch (controlProperties.RangeGeneral.RangePlacement.Text.ToLower())
                        {
                            case "cross":
                                {
                                    this.MemberRange.RangePosition = ScalePlacement.Cross;
                                    break;
                                }

                            case "inside":
                                {
                                    this.MemberRange.RangePosition = ScalePlacement.Inside;
                                    break;
                                }

                            case "outside":
                                {
                                    this.MemberRange.RangePosition = ScalePlacement.Outside;
                                    break;
                                }
                        }
                        this.MemberRange.DistanceFromScale = controlProperties.RangeGeneral.RangeDistanceFromScale.Value.Value;
                    }
                    if (uiElement is RangeBorder)
                    {
                        this.MemberRange.BorderWidth = controlProperties.RangeBorder.RangeBorderWidth.Value.Value;
                        this.MemberRange.BorderBrush = new SolidColorBrush(controlProperties.RangeBorder.RangeBorderColor.Color);
                    }
                }

                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }
        }

        private ControlProperties UpdateRangeProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, GaugeChild.GaugeRange);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.RangeGeneral.RangeColor.Color = ((SolidColorBrush)this.MemberRange.BackgroundBrush).Color;
            controlProperties.RangeGeneral.RangeDistanceFromScale.Value = this.MemberRange.DistanceFromScale;
            controlProperties.RangeGeneral.RangeStartValue.Value = this.MemberRange.StartValue;
            controlProperties.RangeGeneral.RangeEndValue.Value = this.MemberRange.EndValue;
            controlProperties.RangeGeneral.RangeStartWidth.Value = this.MemberRange.StartWidth;
            controlProperties.RangeGeneral.RangeEndWidth.Value = this.MemberRange.EndWidth;
            controlProperties.RangeGeneral.RangePlacement.Text = this.MemberRange.RangePosition.ToString();
            controlProperties.RangeBorder.RangeBorderWidth.Value = this.MemberRange.BorderWidth;
            controlProperties.RangeBorder.RangeBorderColor.Color = ((SolidColorBrush)this.MemberRange.BorderBrush).Color;

            return controlProperties;
        }

        /// <summary>
        /// Handles the Click event of the GaugeProperties control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void GaugeProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateGaugePropertiesDialog();
            if (controlProperties.ShowDialog() == true)
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);

                foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is GaugeGeneral)
                    {
                        this.Properties.Name = controlProperties.GaugeGeneral.GeneralName.Text;
                        this.Properties.BackFill = controlProperties.GaugeGeneral.GaugeBackFill.Text;
                        this.Properties.Thickness = controlProperties.GaugeGeneral.FrameThickness.Text;
                        this.Properties.FrameFill = controlProperties.GaugeGeneral.clrpkr_FrameColor.Text;

                        switch (controlProperties.GaugeGeneral.FrameType.TextValue)
                        {
                            case "Circular4":
                                {
                                    this.Properties.Type = "Circular4";
                                    break;
                                }

                            case "Circular1":
                                {
                                    this.Properties.Type = "Circular1";
                                    break;
                                }

                            case "Circular2":
                                {
                                    this.Properties.Type = "Circular2";
                                    break;
                                }

                            case "Circular3":
                                {
                                    this.Properties.Type = "Circular3";
                                    break;
                                }
                        }

                    }
                }

                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }
        }

        private ControlProperties UpdateGaugePropertiesDialog()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, GaugeChild.GaugeProperties);
            this.Panel.UpdateOwnerWindow(controlProperties);

            controlProperties.GaugeGeneral.GeneralName.Text = this.Properties.Name;
            controlProperties.GaugeGeneral.updwn_FrameThickness.Text = this.Properties.Thickness;
            controlProperties.GaugeGeneral.clrpkr_FrameColor.Text = this.Properties.FrameFill;
            controlProperties.GaugeGeneral.FrameType.TextValue = this.Properties.Type;
            controlProperties.GaugeGeneral.clrpkr_GaugeBackFill.Text = this.Properties.BackFill;


            return controlProperties;
        }

        /// <summary>
        /// Handles the Click event of the GaugePanelProperties control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void GaugePanelProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlproperties = UpdateGaugepanelProperties();

            if (controlproperties.ShowDialog() == true)
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);

                foreach (UIElement uiElement in controlproperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is PanelGeneral)
                    {
                        this.ItemName = controlproperties.GaugePanelGeneral.Name.Text;
                        this.DataSetName = controlproperties.GaugePanelGeneral.cmb_datasetName.Text;
                    }
                    else if (uiElement is PanelFill)
                    {
                        this.GaugePanel.Background = new SolidColorBrush(controlproperties.GaugePanelFill.PanelBackground.Color);
                    }
                    else if (uiElement is PanelBorder)
                    {
                        this.GaugePanel.BorderThickness = new Thickness(controlproperties.GaugePanelBorder.BorderWidth.Value.Value);
                        this.GaugePanel.BorderBrush = new SolidColorBrush(controlproperties.GaugePanelBorder.BorderColor.Color);
                    }
                }

                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }
        }

        private ControlProperties UpdateGaugepanelProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this, this.Report, GaugeChild.GaugePanel);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.GaugePanelGeneral.Name.Text = this.ItemName;
            controlProperties.GaugePanelGeneral.DatasetName.Items.Clear();

            foreach (var set in this.DataSets)
            {
                controlProperties.GaugePanelGeneral.DatasetName.Items.Add(set.Name);
            }

            controlProperties.GaugePanelGeneral.cmb_datasetName.Text = this.DataSetName;
            controlProperties.GaugePanelFill.PanelBackground.Color = ((SolidColorBrush)this.GaugePanel.Background).Color;
            controlProperties.GaugePanelBorder.BorderColor.Color = ((SolidColorBrush)this.GaugePanel.BorderBrush).Color;
            controlProperties.GaugePanelBorder.BorderWidth.Value = this.GaugePanel.BorderThickness.Top;
            return controlProperties;
        }

        # endregion

        # region Common Helper Methods

        protected override Size MeasureOverride(Size constraint)
        {
            double radius = 0.74;
            try
            {
                if (this.InternalGauge != null && this.InternalGauge.Radius > 0
                    && constraint.Height > 0 && constraint.Width > 0 && this.InternalGauge != null)
                {
                    //// While Designing new Gauge Override values based on Radius else based on base values.
                  foreach(CircularScale scale in InternalGauge.Scales)
                  {
                      if (ActualHeight != 0 && ActualWidth != 0)
                      {
                          SetGaugeRadius(ActualWidth, ActualHeight);
                          scale.Radius = this.InternalGauge.Radius * radius;
                          radius = radius - .20;
                      }
                      foreach(CircularPointer pointer in scale.Pointers)
                      {
                        if (pointer != null)
                        {
                            //scale.PointerCap.PointerCapRadius = this.InternalGauge.Radius / 20;
                            //pointer.PointerWidth = scale.PointerCap.PointerCapRadius * 3;
                            if (pointer.PointerNeedleType == PointerNeedleType.Marker)
                            {
                                pointer.PointerLength = (scale.Radius * _radius) / 2;
                            }
                            else
                            {
                                pointer.PointerLength = scale.Radius;
                            }
                        }
                      }
                      foreach(CircularRange range in scale.Ranges)
                      {
                          if (range != null)
                          {
                            range.StartWidth = this.InternalGauge.Radius / 25;
                            range.EndWidth = this.InternalGauge.Radius / 8;
                            range.BorderWidth = this.InternalGauge.Radius / 50;
                            range.DistanceFromScale = this.InternalGauge.Radius / 10;
                          }
                      }
                  }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
            return base.MeasureOverride(constraint);
        }

        internal List<string> GetPanelChildrens(StackPanel stackPanel)
        {
            List<string> listOfChildrens = new List<string>();

            if (stackPanel != null)
            {
                foreach (var item in stackPanel.Children)
                {
                    if (item is Button)
                    {
                        listOfChildrens.Add((item as Button).Content.ToString());
                    }
                }
            }

            return listOfChildrens;
        }

        /// <summary>
        /// To draw the default gauge.
        /// </summary>
        private void InitializeGauge()
        {
            if (this.InternalGauge != null)
            {
                this.AddScaleToGauge();
                MenuItem scalemenu = new MenuItem();
                MenuItem pointermenu = new MenuItem();
                MenuItem rangemenu = new MenuItem();
                scalemenu.Header = pointermenu.Header = rangemenu.Header = "RadialScale" + ++scalecount;
                this.SeriesScale.Name = scalemenu.Header.ToString();
                this.ScaleProperties.Items.Add(scalemenu);
                this.AddPointer.Items.Add(pointermenu);
                this.AddRange.Items.Add(rangemenu);
                pointermenu.Click += new RoutedEventHandler(m1_Click);
                scalemenu.Click += new RoutedEventHandler(m1_Click);
                rangemenu.Click += new RoutedEventHandler(m1_Click);
                this.AddPointerToGauge();
                this.AddRangeToGauge();
                this.InternalGauge.Scales.Add(SeriesScale);
                this.InternalGauge.FrameType = GaugeFrameType.FullCircle;
                this.InternalGauge.FirstFrameFillColor = Brushes.LightGray;
                this.InternalGauge.FirstFrameThickness = new Thickness(this.InternalGauge.Radius / 7);
                this.InternalGauge.SecondFrameThickness = new Thickness(0);
                this.InternalGauge.Background = Brushes.Lavender;
                this.InternalGauge.EnableEffects = false;
                this.InternalGauge.IsColorMergeWithBase = false;
                this.GaugePanel.Background = Brushes.White;
                this.GaugePanel.BorderThickness = new Thickness(1);
                this.GaugePanel.BorderBrush = Brushes.Silver;
            }
        }

        /// <summary>
        /// TO add the range to gauge.
        /// </summary>
        internal void AddRangeToGauge()
        {
            //// Defining member range
            if (this.MemberRange == null)
            {
                this.MemberRange = new CircularRange();
                this.MemberRange.StartValue = 70;
                this.MemberRange.EndValue = 100;
            }
            else
            {
                foreach (var gaugeScale in this.InternalGauge.Scales)
                {
                    if (gaugeScale == this.SeriesScale && gaugeScale.Ranges.Count != 0)
                    {
                        rangeStartValue = (int)gaugeScale.Ranges[gaugeScale.Ranges.Count - 1].EndValue;
                    }
                }                

                if (rangeStartValue == 100 || this.SeriesScale.Ranges.Count == 0)
                {
                    rangeStartValue = 0;
                }

                this.MemberRange = new CircularRange();
                this.MemberRange.StartValue = rangeStartValue;
                this.MemberRange.EndValue = rangeStartValue + 20;
            }

            this.MemberRange.StartWidth = 15;
            this.MemberRange.EndWidth = 25;
            this.MemberRange.RangePosition = ScalePlacement.Inside;
            this.MemberRange.DistanceFromScale = 30;
            this.MemberRange.BackgroundBrush = Brushes.Salmon;
            this.MemberRange.BorderWidth = this.InternalGauge.Radius / 50;
            this.MemberRange.BorderBrush = Brushes.Silver;
            this.MemberRange.Name = "ScaleRange" + ++scalerangecount;
            MenuItem rangemenu1 = new MenuItem();
            rangemenu1.Header = this.MemberRange.Name;
            rangemenu1.Click += new RoutedEventHandler(m1_Click);
            RangeProperties.Items.Add(rangemenu1);
            this.SeriesScale.Ranges.Add(MemberRange);
            rangeStartValue = (int)this.MemberRange.EndValue;
        }

        /// <summary>
        /// To add the pointer to gauge.
        /// </summary>
        internal void AddPointerToGauge()
        {
            int pointerValue = 0;
            int assignPointValue = 0;
            //// Defining member value pointer
            this.MemberValuePointer = new CircularPointer();
            this.MemberValuePointer.PointerLength = this.SeriesScale.Radius;
            if (this.SeriesScale.Pointers.Count > 0)
            {
                this.MemberValuePointer.PointerWidth = this.SeriesScale.Pointers[this.SeriesScale.Pointers.Count - 1].PointerWidth;
            }
            else
            {
                this.MemberValuePointer.PointerWidth = 20;
            }
            if (this.SeriesScale.Pointers.Count > 0)
            {
                this.MemberValuePointer.PointerNeedleType = this.SeriesScale.Pointers[this.SeriesScale.Pointers.Count - 1].PointerNeedleType;
            }
            else
            {
                this.MemberValuePointer.PointerNeedleType = PointerNeedleType.Needle;
            }
            if (this.SeriesScale.Pointers.Count > 0)
            {
                this.MemberValuePointer.MarkerStyle = this.SeriesScale.Pointers[this.SeriesScale.Pointers.Count - 1].MarkerStyle;
            }
            else
            {
                this.MemberValuePointer.MarkerStyle = Syncfusion.Windows.Gauge.MarkerStyle.Rectangle;
            }
            if (this.SeriesScale.Pointers.Count > 0)
            {
                this.MemberValuePointer.NeedleStyle = this.SeriesScale.Pointers[this.SeriesScale.Pointers.Count - 1].NeedleStyle;
            }
            else
            {
                this.MemberValuePointer.NeedleStyle = NeedleStyle.Triangle;
            }
            if (this.SeriesScale.Pointers.Count > 0)
            {
                this.MemberValuePointer.PointerPlacement = this.SeriesScale.Pointers[this.SeriesScale.Pointers.Count - 1].PointerPlacement;
            }
            else
            {
                this.MemberValuePointer.PointerPlacement = ScalePlacement.Cross;
            }

           this.MemberValuePointer.BorderWidth = 1;
           if (pointervalue > 100 && this.SeriesScale.Pointers.Count != 0)
            {
                pointerValue = 100 / this.SeriesScale.Pointers.Count;
                assignPointValue = pointerValue;
                foreach (var pointer in this.SeriesScale.Pointers)
                {
                    pointer.Value = assignPointValue;
                    assignPointValue = (int)pointer.Value + pointerValue;
                }
            }
            else
            {
                this.MemberValuePointer.Value = pointervalue;
            }
            pointervalue += 10;
            this.MemberValuePointer.BorderBrush = Brushes.LavenderBlush;
            this.MemberValuePointer.EnablePointerInteraction = false;
            pointerName = "RadialPointer" + ++radialpointercount;
            bool check;

            do
            {
                check = (from name in this.ValueItems
                         where name == pointerName
                         select name).Count() > 0 ? true : false;

                pointerName = (check == true) ? "RadialPointer" + ++radialpointercount : pointerName;
            } while (check);

            this.MemberValuePointer.Name = pointerName;
            MenuItem pointermenu1 = new MenuItem();
            pointermenu1.Header = this.MemberValuePointer.Name;
            this.PointerProperties.Items.Add(pointermenu1);
            pointermenu1.Click += new RoutedEventHandler(m1_Click);
            this.SeriesScale.Pointers.Add(MemberValuePointer);
            this.SeriesScale.PointerCap.PointerCapRadius = this.InternalGauge.Radius / 20;

            this.MemberValuePointer.BackgroundBrush = Brushes.Gold;
            if (this.SeriesScale.Pointers.Count == 1)
            {
                this.SeriesScale.PointerCap.BackgroundBrush = Brushes.Gray;
            }      

            Button buttonObj = new Button();
            buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
            buttonObj.Margin = new Thickness(2);
            buttonObj.Content = this.MemberValuePointer.Name;
            buttonObj.Name = this.MemberValuePointer.Name;
            ValuePanel.Children.Add(buttonObj);

        }

        /// <summary>
        /// To add the scale to gauge.
        /// </summary>
        private void AddScaleToGauge()
        {
            //// Defining series scale
            // double radius = 0.74;
            this.SeriesScale = new CircularScale();
            this.SeriesScale.ShadowOffset = 2;
            this.SeriesScale.Minimum = 0;
            this.SeriesScale.Maximum = 100;
            this.SeriesScale.MajorIntervalValue = 10;
            this.SeriesScale.MinorIntervalValue = this.SeriesScale.MajorIntervalValue / 4;
            this.SeriesScale.StartAngle = 120;
            this.SeriesScale.GapSweepAngle = 300;
            this.SeriesScale.ScaleBarSize = 2;
            if (this.InternalGauge.Scales.Count != 0)
            {
                this.SeriesScale.Radius = this.InternalGauge.Scales[this.InternalGauge.Scales.Count - 1].Radius * 0.74;
            }
            else
            {
                this.SeriesScale.Radius = this.InternalGauge.Radius * _radius;
                _radius = _radius - .20;
            }
            
            this.SeriesScale.BorderWidth = 1.0;
            this.SeriesScale.BackgroundBrush = Brushes.Transparent;
            this.SeriesScale.BorderBrush = Brushes.Transparent;

            //// Defining Major tick
            this.MajorTick = new CircularMarkTick();
            this.MajorTick.TickWidth = 2;
            this.MajorTick.TickHeight = this.InternalGauge.Radius / 8;
            this.MajorTick.TickStyle = TickStyle.MajorTick;
            this.MajorTick.TickPlacement = ScalePlacement.Cross;
            this.MajorTick.TickShape = TickShape.Rectangle;
            this.MajorTick.BackgroundBrush = Brushes.Black;

            //// Defining minor tick
            this.MinorTick = new CircularMarkTick();
            this.MinorTick.TickWidth = 1;
            this.MinorTick.TickHeight = this.InternalGauge.Radius / 15;
            this.MinorTick.TickStyle = TickStyle.MinorTick;
            this.MinorTick.TickPlacement = ScalePlacement.Cross;
            this.MinorTick.TickShape = TickShape.Rectangle;
            this.MinorTick.BackgroundBrush = Brushes.Black;

            //// Defining Major label tick
            this.MajorLabelTick = new CircularLabelTick();
            if (this.InternalGauge.Radius > 0)
                this.MajorLabelTick.FontSize = this.InternalGauge.Radius / 10;
            this.MajorLabelTick.TickStyle = TickStyle.MajorTick;
            this.MajorLabelTick.TickPlacement = ScalePlacement.Inside;
            this.MajorLabelTick.DistanceFromScale = this.InternalGauge.Radius / 20;
            this.MajorLabelTick.Angle = 0;
            this.MajorLabelTick.BackgroundBrush = Brushes.Black;
            number.NumberDecimalDigits = 0;
            this.MajorLabelTick.NumberFormatInfo = number;

            this.SeriesScale.Ticks.Add(MinorTick);
            this.SeriesScale.Ticks.Add(MajorTick);
            this.SeriesScale.Ticks.Add(MajorLabelTick);
        }

        /// <summary>
        /// To change the context menu depends on the presence of pointer, scale and range.
        /// </summary>
        private void ChangeContextMenuVisibility()
        {
            if (this.InternalGauge.Scales.Count == 0)
            {
                this.AddScale.Visibility = Visibility.Visible;
                this.AddPointer.Visibility = Visibility.Collapsed;
                this.AddRange.Visibility = Visibility.Collapsed;
                this.ScaleProperties.Visibility = Visibility.Collapsed;
                this.PointerProperties.Visibility = Visibility.Collapsed;
                this.RangeProperties.Visibility = Visibility.Collapsed;
                this.DeleteScale.Visibility = Visibility.Collapsed;
                this.DeleteRange.Visibility = Visibility.Collapsed;
                this.DeletePointer.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.AddScale.Visibility = Visibility.Visible;
                this.ScaleProperties.Visibility = Visibility.Visible;
                this.DeleteScale.Visibility = Visibility.Visible;

                if (this.PointerProperties.Items.Count == 0)
                {
                    this.AddPointer.Visibility = Visibility.Visible;
                    this.DeletePointer.Visibility = Visibility.Collapsed;
                    this.PointerProperties.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.DeletePointer.Visibility = Visibility.Visible;
                    this.PointerProperties.Visibility = Visibility.Visible;
                }

                if (this.RangeProperties.Items.Count == 0)
                {
                    this.AddRange.Visibility = Visibility.Visible;
                    this.DeleteRange.Visibility = Visibility.Collapsed;
                    this.RangeProperties.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.DeleteRange.Visibility = Visibility.Visible;
                    this.RangeProperties.Visibility = Visibility.Visible;
                }
            }
        }


        /// <summary>
        /// To add the Context menu for the button added in value panel.
        /// </summary>
        /// <param name="parentButton"></param>
        /// <returns></returns>
        ContextMenu GetButtonContextMenu(Button parentButton)
        {
            ContextMenu contentMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Tag = parentButton;
            menuItem.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDelete");
            menuItem.Click += new RoutedEventHandler(DeletePointerMenu_Click);
            MenuItem properties = new MenuItem { Header = "Pointer Properties" };
            properties.Tag = parentButton;
            properties.Click += new RoutedEventHandler(properties_Click);
            contentMenu.Items.Add(menuItem);
            contentMenu.Items.Add(properties);

            return contentMenu;
        }

        void properties_Click(object sender, RoutedEventArgs e)
        {
            Button buttonObj = ((MenuItem)sender).Tag as Button;
            if (buttonObj != null)
            {
                StackPanel stackPanel = Util.GetParentItem<StackPanel>(buttonObj as DependencyObject) as StackPanel;
                if (stackPanel != null)
                {
                    if (this.ValuePanel.Children.Count > 0)
                    {
                        string orgString = buttonObj.Content.ToString();
                        for (int i = 0; i < this.ValuePanel.Children.Count; i++)
                        {
                            try
                            {
                                foreach (CircularScale sacle in this.InternalGauge.Scales)
                                {
                                    foreach (CircularPointer pointer in sacle.Pointers)
                                    {
                                        if (orgString == pointer.Name)
                                        {
                                            this.MemberValuePointer = pointer;
                                            // stackPanel.Children.Remove((UIElement)buttonObj);
                                            break;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
                            }
                        }
                    }
                }
            }
            this.PointerProperties_Click(this.MemberValuePointer, e);

        }



        /// <summary>
        /// To hide the value panel.
        /// </summary>
        internal void HidePanels()
        {
            if (this.ValuePanel != null)
            {
                this.ValueInnerBorder.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        # region Value Panel Helper Methods

        /// <summary>
        /// To add the data sources, data sets and data fields in the smart tag.
        /// </summary>
        private void NoValuePanelChildrens()
        {
            if ((this.DataSources != null) && (this.DataSets != null))
            {
                if (this.DataSources.Count != 0)
                {
                    if ((this.DataSources.Count >= 1) && (this.DataSets.Count > 1))
                    {
                        this.ValuePanelDataField.ContextMenu = null;
                        ContextMenu contextMenu = new ContextMenu();

                        foreach (Syncfusion.RDL.DOM.DataSource dataSource in this.DataSources)
                        {
                            var dataSets = from dataSet in this.DataSets
                                           where dataSet.Query.DataSourceName == dataSource.Name
                                           select dataSet;


                            var dataSets1 = dataSets.ToList();
                            MenuItem menuItem = new MenuItem();
                            menuItem.Header = dataSource.Name;


                            foreach (Syncfusion.RDL.DOM.DataSet dataSet in dataSets1)
                            {
                                MenuItem menuItem2 = new MenuItem();
                                menuItem2.Header = dataSet.Name;

                                var fields = from field in dataSet.Fields
                                             select field;
                                var field1 = fields.ToList();

                                foreach (Syncfusion.RDL.DOM.Field field in field1)
                                {
                                    MenuItem menuItem3 = new MenuItem();
                                    menuItem3.Header = field.Name;
                                    menuItem2.Items.Add(menuItem3);
                                    menuItem3.Click += new RoutedEventHandler(menuItem3_Click);
                                }
                                menuItem.Items.Add(menuItem2);
                            }
                            contextMenu.Items.Add(menuItem);
                        }
                        this.ValuePanelDataField.ContextMenu = contextMenu;
                        this.ValuePanelDataField.ContextMenu.IsOpen = true;
                    }
                    else
                    {
                        this.ValuePanelDataField.ContextMenu = null;
                        ContextMenu contextMenu3 = new ContextMenu();
                        if (this.DataSets.Count == 1)
                        {
                            var fields = from field in DataSets[0].Fields
                                         select field;
                            var field1 = fields.ToList();

                            foreach (Syncfusion.RDL.DOM.Field field in field1)
                            {
                                MenuItem menuItem4 = new MenuItem();
                                menuItem4.Header = field.Name;
                                menuItem4.Click += new RoutedEventHandler(menuItem4_Click);
                                contextMenu3.Items.Add(menuItem4);
                            }
                            this.ValuePanelDataField.ContextMenu = contextMenu3;
                            this.ValuePanelDataField.ContextMenu.IsOpen = true;
                        }
                    }
                    if ((this.DataSources.Count >= 1) && (this.DataSets.Count == 0))
                    {
                        this.ValuePanelDataField.ContextMenu = null;
                        ContextMenu contextMenu = new ContextMenu();
                        MenuItem menuItem6 = new MenuItem();
                        menuItem6.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddDataSet");
                        menuItem6.Click += new RoutedEventHandler(AddDataSet_Click);
                        contextMenu.Items.Add(menuItem6);
                        this.ValuePanelDataField.ContextMenu = contextMenu;
                        this.ValuePanelDataField.ContextMenu.IsOpen = true;
                    }
                }
                else
                {
                    this.ValuePanelDataField.ContextMenu = null;
                    ContextMenu contextMenu = new ContextMenu();
                    MenuItem menuItem5 = new MenuItem();
                    menuItem5.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerAddDataSource");
                    menuItem5.Click += new RoutedEventHandler(AddDataSource_Click);
                    contextMenu.Items.Add(menuItem5);
                    this.ValuePanelDataField.ContextMenu = contextMenu;
                    this.ValuePanelDataField.ContextMenu.IsOpen = true;
                }
            }
        }

        /// <summary>
        /// To create the button based on the aggregate function of the data field added.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="parent"></param>
        private void CreateValueButtonObj(string header, string parent, string pointerName)
        {
            this.Properties.IsInternalPropertyChange = true;
            EditAction action = new EditAction();
            action.EditingType = EditActionType.ItemChanged;
            ItemChange change = new ItemChange();
            change.ReportItem = this;
            change.OldValue = this.GetReportItem();
            action.ItemChange = change;
            this.Panel.EditingManager.AddAction(action);
            string DataType = string.Empty;
            string prefixString = string.Empty;

            if (parent != null && this.DataSets != null)
            {
                string dataSetName = parent;

                if (dataSetName != null)
                {
                    DataType = (from dataSetThis in this.DataSets
                                from DataSetfield in dataSetThis.Fields
                                where DataSetfield.Name == header
                                where dataSetThis.Name == dataSetName
                                select DataSetfield.TypeName).FirstOrDefault();
                }

                if (DataType!=null&&DataType != string.Empty && DataType.StartsWith("System."))
                {
                    if (DataType.StartsWith("System.Int")
                        || DataType.StartsWith("System.Byte")
                        || DataType.StartsWith("System.Boolean")
                        || DataType.StartsWith("System.Decimal")
                        || DataType.StartsWith("System.Double")
                        || DataType.StartsWith("System.Single")
                        )
                    {
                        prefixString = "Sum";
                    }
                    else
                    {
                        prefixString = "Count";
                    }
                }
                else if (DataType != string.Empty)
                {
                    if (
                            DataType == "bigint" ||
                            DataType == "int" ||
                            DataType == "smallint" ||
                            DataType == "tinyint" ||
                            DataType == "bit" ||
                            DataType == "decimal" ||
                            DataType == "numeric" ||
                            DataType == "money" ||
                            DataType == "smallmoney" ||
                            DataType == "float" ||
                            DataType == "real"
                        )
                    {
                        prefixString = "Sum";
                    }
                    else if (
                                DataType == "datetime" ||
                                DataType == "smalldatetime" ||
                                DataType == "char" ||
                                DataType == "varchar" ||
                                DataType == "varchar(max)" ||
                                DataType == "text" ||
                                DataType == "nchar" ||
                                DataType == "nvarchar" ||
                                DataType == "nvarchar(max)" ||
                                DataType == "ntext" ||
                                DataType == "binary" ||
                                DataType == "varbinary" ||
                                DataType == "varbinary(max)" ||
                                DataType == "image"
                            )
                    {
                        prefixString = "Count";
                    }
                    else
                    {
                        prefixString = "Count";
                    }
                }
            }

            if (prefixString != string.Empty)
            {
                if (this.DataSetName == null || this.DataSetName == string.Empty)
                {
                    this.DataSetName = parent;
                }

                if (parent == this.DataSetName)
                {
                    // ValuePanel.Children.Clear();

                    Button buttonObj = new Button();
                    buttonObj.ContextMenu = GetButtonContextMenu(buttonObj);
                    buttonObj.Margin = new Thickness(2);
                    buttonObj.Content = "[" + prefixString + "(" + header.Replace("_", "__") + ")" + "]";
                    if (ValuePanel.Children.Count > 0)
                    {
                        if (pointerName != null)
                        {
                            int i = 0;
                            foreach (Button button in ValuePanel.Children)
                            {
                                if (button.Content.ToString() == pointerName)
                                {
                                    ValuePanel.Children.Remove(button);
                                    ValuePanel.Children.Insert(i, buttonObj);
                                    buttonObj.Name = button.Name;
                                    break;
                                }
                                i++;
                            }
                        }
                        else
                        {
                            buttonObj.Name = ((Button)ValuePanel.Children[0]).Name;
                            ValuePanel.Children.RemoveAt(0);
                            ValuePanel.Children.Insert(0, buttonObj);
                        }
                    }
                    else
                    {
                        this.AddPointerToGauge();
                        ValuePanel.Children.RemoveAt(0);
                        ValuePanel.Children.Insert(0, buttonObj);
                    }
                    // this.GaugeBase = UpdateGaugeObj();
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxFieldFromCurrentDataSet") + this.DataSetName.ToString() + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCanBeAdded"), this.error_title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            change.NewValue = this.GetReportItem();
            this.Properties.IsInternalPropertyChange = false;
        }

        # endregion

        # region Serialization Helper methods

        /// <summary>
        /// To instantiate default base objects and to update the properties.
        /// </summary>
        /// <returns></returns>
        private Syncfusion.RDL.DOM.GaugePanel UpdateGaugeObj(Syncfusion.RDL.DOM.GaugePanel gaugeObj)
        {
            try
            {
                gaugeObj.Style = new Syncfusion.RDL.DOM.Style();
                gaugeObj.Style.Border = new Syncfusion.RDL.DOM.Border();
                gaugeObj.RadialGauges = new Syncfusion.RDL.DOM.RadialGauges();
                Syncfusion.RDL.DOM.RadialGauge radialGauge = new Syncfusion.RDL.DOM.RadialGauge();

                //// To update base objects depends on the presence of gauge.
                if (this.InternalGauge != null)
                {
                    gaugeObj.AutoLayout = true;
                    gaugeObj.Style.BackgroundColor = this.GaugePanel.Background.ToString();
                    gaugeObj.Style.Border.Color = this.Properties.BorderColor;
                    gaugeObj.Style.Border.Width = this.Properties.BorderWidth;
                    gaugeObj.Style.Border.Style = this.Properties.BorderStyle;


                    radialGauge.Name = "RadialGauge" + ++radialgaugecount;
                    radialGauge.BackFrame = new Syncfusion.RDL.DOM.BackFrame();
                    radialGauge.BackFrame.Style = new Syncfusion.RDL.DOM.Style();
                    radialGauge.BackFrame.FrameBackground = new Syncfusion.RDL.DOM.FrameBackground();
                    radialGauge.BackFrame.FrameBackground.Style = new Syncfusion.RDL.DOM.Style();
                    if (this.Properties.Thickness.Contains("pt"))
                    {
                        radialGauge.BackFrame.FrameWidth = Convert.ToSingle((this.Properties.Thickness.Remove(this.Properties.Thickness.Length - 2, 2)));
                    }
                    else
                    {
                        radialGauge.BackFrame.FrameWidth = Convert.ToSingle((this.Properties.Thickness));
                    }
                    radialGauge.BackFrame.Style.BackgroundColor = this.Properties.FrameFill;
                    radialGauge.BackFrame.FrameBackground.Style.BackgroundColor = this.Properties.BackFill;
                    radialGauge.BackFrame.FrameStyle = RDL.DOM.FrameStyle.Simple;

                    switch (this.Properties.Type)
                    {
                        case "Circular1":
                            {
                                radialGauge.BackFrame.FrameShape = RDL.DOM.FrameShape.CustomCircular1;
                                break;
                            }
                        case "Circular2":
                            {
                                radialGauge.BackFrame.FrameShape = RDL.DOM.FrameShape.CustomCircular2;
                                break;
                            }
                        case "Circular3":
                            {
                                radialGauge.BackFrame.FrameShape = RDL.DOM.FrameShape.CustomCircular3;
                                break;
                            }
                        case "Circular4":
                            {
                                radialGauge.BackFrame.FrameShape = RDL.DOM.FrameShape.CustomCircular7;
                                break;
                            }
                    }

                    //// To update base objects depends on the presence of Gauge scale.
                    radialGauge.GaugeScales = new Syncfusion.RDL.DOM.GaugeScales();
                    Syncfusion.RDL.DOM.RadialScale radialScale = null;
                    if (this.SeriesScale != null && this.MajorLabelTick != null && this.MajorTick != null && this.MinorTick != null)
                    {
                        foreach (CircularScale scale in this.InternalGauge.Scales)
                        {
                            radialScale = new Syncfusion.RDL.DOM.RadialScale();
                            radialScale.Name = scale.Name;
                            radialScale.MinimumValue = new Syncfusion.RDL.DOM.MinimumValue();
                            radialScale.MaximumValue = new Syncfusion.RDL.DOM.MaximumValue();
                            radialScale.GaugeMajorTickMarks = new Syncfusion.RDL.DOM.GaugeMajorTickMarks();
                            radialScale.GaugeMinorTickMarks = new Syncfusion.RDL.DOM.GaugeMinorTickMarks();
                            radialScale.GaugeMajorTickMarks.Style = new Syncfusion.RDL.DOM.Style();
                            radialScale.GaugeMinorTickMarks.Style = new Syncfusion.RDL.DOM.Style();
                            radialScale.ScaleLabels = new Syncfusion.RDL.DOM.ScaleLabels();
                            radialScale.ScaleLabels.Style = new Syncfusion.RDL.DOM.Style();
                            if (this.InternalGauge.Radius != 0)
                            {
                                radialScale.Radius = (float)(((scale.Radius * 100) / (this.InternalGauge.Radius)) / 2); //(float)37; // (float)((this.InternalGauge.Radius * 0.74) / 2);
                            }
                            if (radialScale.Multiplier == 0)
                            {
                                radialScale.MinimumValue.Value = scale.Minimum.ToString();
                                radialScale.MaximumValue.Value = scale.Maximum.ToString();
                            }
                            else
                            {
                                radialScale.MinimumValue.Value = (scale.Minimum / radialScale.Multiplier).ToString();
                                radialScale.MaximumValue.Value = (scale.Maximum / radialScale.Multiplier).ToString();
                            }

                            if (radialScale.Multiplier == 0 && scale.Maximum.ToString() != "0")
                            {
                                radialScale.Multiplier = 1;
                            }

                            radialScale.MaximumValue.Multiplier = 1;
                            radialScale.MinimumValue.Multiplier = 1;
                            radialScale.StartAngle = (float)scale.StartAngle - 90;
                            radialScale.SweepAngle = (float)scale.GapSweepAngle;
                            radialScale.ScaleLabels.Placement = (RDL.DOM.Placement)this.MajorLabelTick.TickPlacement;
                            radialScale.ScaleLabels.Style.FontSize = ((this.MajorLabelTick.FontSize).ToString() + "pt");
                            radialScale.ScaleLabels.FontAngle = (float)this.MajorLabelTick.Angle;
                            radialScale.ScaleLabels.DistanceFromScale = (float)this.MajorLabelTick.DistanceFromScale;
                            radialScale.ScaleLabels.Style.Color = this.MajorLabelTick.BackgroundBrush.ToString();
                            radialScale.GaugeMajorTickMarks.Placement = (RDL.DOM.Placement)this.MajorTick.TickPlacement;
                            radialScale.GaugeMajorTickMarks.Shape = (Syncfusion.RDL.DOM.Shape)this.MajorTick.TickShape;
                            radialScale.GaugeMajorTickMarks.Length = (float)this.MajorTick.TickHeight;
                            radialScale.GaugeMajorTickMarks.Width = (float)this.MajorTick.TickWidth;
                            radialScale.GaugeMajorTickMarks.Style.BackgroundColor = this.MajorTick.BackgroundBrush.ToString();
                            radialScale.GaugeMajorTickMarks.Interval = (float) scale.MajorIntervalValue;
                            radialScale.GaugeMinorTickMarks.Placement = (RDL.DOM.Placement)this.MinorTick.TickPlacement;
                            radialScale.GaugeMinorTickMarks.Shape = (Syncfusion.RDL.DOM.Shape)this.MinorTick.TickShape;
                            radialScale.GaugeMinorTickMarks.Length = (float)this.MinorTick.TickHeight;
                            radialScale.GaugeMinorTickMarks.Width = (float)this.MinorTick.TickWidth;
                            radialScale.GaugeMinorTickMarks.Style.BackgroundColor = this.MinorTick.BackgroundBrush.ToString();
                            radialScale.GaugeMinorTickMarks.Interval = (float)scale.MinorIntervalValue;
                           
                            radialGauge.GaugeScales.Add(radialScale);

                            //// To update base objects depends on the presence of pointer.
                            radialScale.GaugePointers = new Syncfusion.RDL.DOM.GaugePointers();
                            if (this.MemberValuePointer != null)
                            {
                                int count = 0;
                                foreach (CircularPointer pointer in scale.Pointers)
                                {
                                    Syncfusion.RDL.DOM.RadialPointer radialPointer = new Syncfusion.RDL.DOM.RadialPointer();
                                    radialPointer.Name = pointer.Name;
                                    radialPointer.PointerCap = new Syncfusion.RDL.DOM.PointerCap();
                                    radialPointer.GaugeInputValue = new Syncfusion.RDL.DOM.GaugeInputValue();
                                    radialPointer.PointerCap.Style = new Syncfusion.RDL.DOM.Style();
                                    radialPointer.Style = new Syncfusion.RDL.DOM.Style();
                                    radialPointer.Style.Border = new Syncfusion.RDL.DOM.Border();

                                    radialPointer.PointerCap.Width = (float)((scale.PointerCap.PointerCapRadius) * 6);
                                    radialPointer.PointerCap.Style.BackgroundColor = scale.PointerCap.BackgroundBrush.ToString();

                                    if (pointer.PointerNeedleType == PointerNeedleType.Needle)
                                    {
                                        radialPointer.MarkerLength = (float)(scale.Radius);
                                    }
                                    else
                                    {
                                        radialPointer.MarkerLength = (float)pointer.PointerLength;
                                    }

                                    radialPointer.Width = (float)pointer.PointerWidth;
                                    radialPointer.Type = (Syncfusion.RDL.DOM.RadialPointerType)pointer.PointerNeedleType;

                                    switch (pointer.NeedleStyle)
                                    {
                                        case NeedleStyle.Arrow:
                                            {
                                                radialPointer.NeedleStyle = RDL.DOM.NeedleStyleGauge.Arrow;
                                                break;
                                            }
                                        case NeedleStyle.Triangle:
                                            {
                                                radialPointer.NeedleStyle = RDL.DOM.NeedleStyleGauge.Triangular;
                                                break;
                                            }
                                        case NeedleStyle.Rectangle:
                                            {
                                                radialPointer.NeedleStyle = RDL.DOM.NeedleStyleGauge.Rectangular;
                                                break;
                                            }
                                    }

                                    radialPointer.PointerCap.OnTop = true;
                                    radialPointer.MarkerStyle = (RDL.DOM.MarkerStyle)pointer.MarkerStyle;
                                    radialPointer.Placement = (RDL.DOM.Placement)pointer.PointerPlacement;
                                    radialPointer.Style.Border.Width = pointer.BorderWidth.ToString() + "pt";
                                    if (pointer.BorderWidth == 0)
                                    {
                                        radialPointer.Style.Border.Width ="1pt";
                                    }
                                    radialPointer.Style.BackgroundColor = pointer.BackgroundBrush.ToString();
                                    radialPointer.Style.BackgroundGradientEndColor = pointer.BackgroundBrush.ToString();
                                    radialPointer.Style.Border.Color = pointer.BorderBrush.ToString();
                                    string str1 = string.Empty;
                                    if ((this.ValuePanel != null) && (this.ValuePanel.Children.Count != 0))
                                    {
                                        if (this.ValuePanel.Children[count] != null)
                                        {
                                            str1 = (this.ValuePanel.Children[count] as Button).Content.ToString();
                                            str1 = str1.Replace("__", "_");
                                        }
                                    }

                                    if ((this.ValuePanel != null) && (this.ValuePanel.Children.Count != 0) && str1.Contains("Count") || str1.Contains("Sum"))
                                    {
                                        string gaugeInputValue = string.Empty;
                                        string subStrDataType = string.Empty;
                                        string subFieldName = string.Empty;
                                        if (this.ValuePanel.Children[count] != null)
                                        {
                                            string str = (this.ValuePanel.Children[count] as Button).Content.ToString();
                                            str = str.Replace("__", "_");


                                            try
                                            {
                                                if ((str.IndexOf("(") > 0) && (str.IndexOf(")") > 0))
                                                {
                                                    int l = str.IndexOf("(") - str.IndexOf("[");
                                                    int l1 = str.IndexOf(")") - str.IndexOf("(");
                                                    subStrDataType = str.Substring(str.IndexOf("[") + 1, (str.IndexOf("(") - str.IndexOf("[") - 1));
                                                    subFieldName = str.Substring((str.IndexOf("(") + 1), (str.IndexOf(")") - str.IndexOf("(") - 1));
                                                    gaugeInputValue = "=" + subStrDataType + "(" + "Fields!" + subFieldName + ".Value" + ")";
                                                }
                                                else
                                                {
                                                    subFieldName = str.Substring((str.IndexOf("[") + 1), (str.IndexOf("]") - str.IndexOf("[") - 1));
                                                    gaugeInputValue = subFieldName;
                                                }
                                            }

                                            catch
                                            {
                                                gaugeInputValue = str;
                                            }

                                            radialPointer.GaugeInputValue.Value = gaugeInputValue;
                                        }
                                    }
                                    else
                                    {
                                        radialPointer.GaugeInputValue.Value = pointer.Value.ToString();
                                    }

                                    radialScale.GaugePointers.Add(radialPointer);
                                    count++;
                                }
                            }

                            radialScale.ScaleRanges = new Syncfusion.RDL.DOM.ScaleRanges();
                            //// To update base objects depends of the presence of range.
                            if (this.MemberRange != null)
                            {
                                foreach (CircularRange range in scale.Ranges)
                                {
                                    Syncfusion.RDL.DOM.ScaleRange scaleRange = new Syncfusion.RDL.DOM.ScaleRange();
                                    scaleRange.Name = range.Name;
                                    scaleRange.StartValue = new Syncfusion.RDL.DOM.StartValue();
                                    scaleRange.EndValue = new Syncfusion.RDL.DOM.EndValue();
                                    scaleRange.Style = new Syncfusion.RDL.DOM.Style();
                                    scaleRange.Style.Border = new Syncfusion.RDL.DOM.Border();

                                    scaleRange.Placement = (RDL.DOM.Placement)range.RangePosition;
                                    scaleRange.StartValue.Value = range.StartValue.ToString();
                                    scaleRange.EndValue.Value = range.EndValue.ToString();
                                    scaleRange.StartWidth = (float)(range.StartWidth);
                                    scaleRange.EndWidth = (float)(range.EndWidth);
                                    scaleRange.Style.BackgroundColor = range.BackgroundBrush.ToString();
                                    scaleRange.Style.Border.Color = range.BorderBrush.ToString();
                                    scaleRange.DistanceFromScale = (float)range.DistanceFromScale;
                                    scaleRange.Style.Border.Width = ((range.BorderWidth).ToString() + "pt");
                                    if (range.BorderWidth > 20)
                                        scaleRange.Style.Border.Width = "20pt";
                                    radialScale.ScaleRanges.Add(scaleRange);
                                }
                            }

                        }
                    }

                    gaugeObj.RadialGauges.Add(radialGauge);
                    gaugeObj.DataSetName = this.DataSetName;
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message.ToString());
            }

            return gaugeObj;
        }
        # endregion

        #region Custom Event Methods

        internal void DeleteGaugeRaised()
        {
            if (this.DeleteGauge != null)
            {
                this.Panel.DeleteSelectedReportItems();
            }
        }

        #endregion

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
    }  
}
