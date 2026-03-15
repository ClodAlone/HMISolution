#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Text;
    using System.Xml;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
#if WINDOWS_PHONE
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Data;
#else
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.Foundation;
#endif

    /// <summary>
    /// Represents legend for a <see cref="SfChart"/>.
    /// </summary>
    /// <remarks>
    /// Chart legend will be added as chart's child. Each item in legend contain key information about the <see cref="ChartSeriesBase"/>. Legend has all abilities such as docking, enabling or
    /// disabling desired series in a <see cref="SfChart"/>.
    ///</remarks>
    [ClassReference(IsReviewed = false)]
    public class ChartLegend : ItemsControl,ICloneable
    {
        #region DependencyProperties
        
        /// <summary>
        /// Gets or Sets the orientation of chart legend.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartOrientation Orientation
        {
            get { return (ChartOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(ChartOrientation), typeof(ChartLegend), new PropertyMetadata(ChartOrientation.Default, new PropertyChangedCallback(OnOrientationChanged)));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartLegend sender = d as ChartLegend;
            sender.ChangeOrientation();
        }

        /// <summary>
        /// Identifies the CornerRadius dependency property.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public static DependencyProperty CornerRadiusProperty =
             DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ChartLegend), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// Identifies the CheckBoxVisibility dependency property.
        /// </summary>
        public static DependencyProperty CheckBoxVisibilityProperty =
          DependencyProperty.Register("CheckBoxVisibility", typeof(Visibility), typeof(ChartLegend), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the IconVisibility dependency property.
        /// </summary>
        public static DependencyProperty IconVisibilityProperty =
          DependencyProperty.Register("IconVisibility", typeof(Visibility), typeof(ChartLegend), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the IconWidth dependency property.
        /// </summary>
        public static DependencyProperty IconWidthProperty =
          DependencyProperty.Register("IconWidth", typeof(double), typeof(ChartLegend), new PropertyMetadata(15d));

        /// <summary>
        /// Identifies the IconHeight dependency property.
        /// </summary>
        public static DependencyProperty IconHeightProperty =
          DependencyProperty.Register("IconHeight", typeof(double), typeof(ChartLegend), new PropertyMetadata(15d));

		/// <summary>
        /// Identifies the ItemMargin dependency property.
        /// </summary>
        public static DependencyProperty ItemMarginProperty =
          DependencyProperty.RegisterAttached("ItemMargin", typeof(Thickness), typeof(ChartLegend), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Gets or Sets the position of the ChartLegend.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartDock DockPosition
        {
            get { return (ChartDock)GetValue(DockPositionProperty); }
            set { SetValue(DockPositionProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DockPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DockPositionProperty =
            DependencyProperty.Register("DockPosition", typeof(ChartDock), typeof(ChartLegend), new PropertyMetadata(ChartDock.Top,new PropertyChangedCallback(OnDockPositionChanged)));
            

        /// <summary>
        /// Identifies the OffsetX dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
         DependencyProperty.Register("OffsetX", typeof(double), typeof(ChartLegend), new PropertyMetadata(0d,new PropertyChangedCallback(OnOffsetValueChanged)));

       
        /// <summary>
        /// Identifies the OffsetY dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
        DependencyProperty.Register("OffsetY", typeof(double), typeof(ChartLegend), new PropertyMetadata(0d, new PropertyChangedCallback(OnOffsetValueChanged)));

        #endregion

        #region InternalProperties

        internal ChartAxis XAxis
        {
            get;
            set;
        }

        internal ChartAxis YAxis
        {
            get;
            set;
        }

        internal ChartBase ChartArea
        {
            get;
            set;
        }

        internal int RowColumnIndex { get; set; }

        internal Rect ArrangeRect { get; set; }

        private ChartDock internalDockPosition = ChartDock.Top;

        internal ChartDock InternalDockPosition
        {
            get { return internalDockPosition; }
            set { internalDockPosition = value; }
        }

        #endregion

        #region methods


        public DependencyObject Clone()
        {
            ChartLegend legend = new ChartLegend()
            {
                CheckBoxVisibility = this.CheckBoxVisibility,
                CornerRadius = this.CornerRadius,
                DockPosition = this.DockPosition,
                Header = this.Header,
                HeaderTemplate = this.HeaderTemplate,
                IconHeight = this.IconHeight,
                IconVisibility = this.IconVisibility,
                IconWidth = this.IconWidth,
                //ItemContainerStyle = this.ItemContainerStyle,
                //ItemContainerStyleSelector = this.ItemContainerStyleSelector,
                //ItemContainerTransitions = this.ItemContainerTransitions,
                ItemMargin = this.ItemMargin,
                OffsetX = this.OffsetX,
                OffsetY = this.OffsetY,
                Orientation = this.Orientation
            };
            ChartCloning.CloneControl(this, legend);
            return legend;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the legend position, this is works for 2D charts alone.
        /// </summary>
        /// <value>
        /// The legend position.
        /// </value>
        public LegendPosition LegendPosition
        {
            get { return (LegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendPositionProperty =
            DependencyProperty.Register("LegendPosition", typeof(LegendPosition), typeof(ChartLegend), new PropertyMetadata(LegendPosition.Outside, OnLegendPositionChanged));

        private static void OnLegendPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as ChartLegend).OnLegendPositionChanged();
        }

        private void OnLegendPositionChanged()
        {
            if (LegendPosition == Charts.LegendPosition.Inside)
                InternalDockPosition = ChartDock.Floating;
            else
                InternalDockPosition = DockPosition;

            ChartDockPanel.SetDock(this, InternalDockPosition);

            if (Parent != null)
            {
                ChartArea.UpdateLegendArrangeRect();
                (Parent as ChartDockPanel).InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or Sets the margin for legend item.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Thickness ItemMargin
        {
            get
            {
                return (Thickness)this.GetValue(ChartLegend.ItemMarginProperty);
            }

            set
            {
                this.SetValue(ChartLegend.ItemMarginProperty, value);
            }
        }     

        /// <summary>
        /// Gets or sets the header for the legend.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ChartLegend), null);

        /// <summary>
        /// Gets or Sets the legend header template.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

      
        /// <summary>
        ///  Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ChartLegend), null);

        /// <summary>
        /// Gets or sets the CornerRadius of legend's border.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(ChartLegend.CornerRadiusProperty);
            }
            set
            {
                SetValue(ChartLegend.CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets a value that determines whether to show/hide CheckBox in legend item.
        ///</summary>
        [ClassReference(IsReviewed = false)]
        public Visibility CheckBoxVisibility
        {
            get
            {
                return (Visibility)this.GetValue(CheckBoxVisibilityProperty);
            }

            set
            {
                this.SetValue(CheckBoxVisibilityProperty, value);
            }
        }

        /// <summary>
        ///Gets or Sets the visibility of the legend icon.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Visibility IconVisibility
        {
            get
            {
                return (Visibility)this.GetValue(IconVisibilityProperty);
            }

            set
            {
                this.SetValue(IconVisibilityProperty, value);
            }
        }

        /// <summary>
        ///Gets or Sets width of the legend icon..
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double IconWidth
        {
            get
            {
                return (double)this.GetValue(ChartLegend.IconWidthProperty);
            }

            set
            {
                this.SetValue(IconWidthProperty, value);
            }
        }

        /// <summary>
        ///Gets or Sets height of the legend 
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double IconHeight
        {
            get
            {
                return (double)this.GetValue(ChartLegend.IconHeightProperty);
            }

            set
            {
                this.SetValue(ChartLegend.IconHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the x-axis value of the left edge of ChartLegend relative to Chart.
        /// </summary>
        /// <value>The OffsetX.</value>
        [ClassReference(IsReviewed = false)]
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the y-axis value of the top edge of ChartLegend relative to Chart.
        /// </summary>
        /// <value>The OffsetY.</value>
        [ClassReference(IsReviewed = false)]
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }


        private static void OnDockPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartLegend).OnDockPositionChanged((ChartDock)e.OldValue , (ChartDock)e.NewValue);
        }

        private void OnDockPositionChanged(ChartDock oldValue, ChartDock newValue)
        {
            if (LegendPosition == LegendPosition.Outside)
            {
                InternalDockPosition = DockPosition;
                if (ChartArea != null)
                {
                    ChartArea.LayoutLegends();
                    ChartArea.UpdateLegendArrangeRect();
                }
            }

            ChangeOrientation();

            if (LegendPosition == LegendPosition.Inside)
            {
                var dockPanel = Parent as ChartDockPanel;
                if (dockPanel == null) return;
                Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                ChartArea.UpdateLegendArrangeRect();
                dockPanel.InvalidateMeasure();
            }
            else if (ChartDockPanel.GetDock(this) != InternalDockPosition)
                ChartDockPanel.SetDock(this, InternalDockPosition);
        }

        private static void OnOffsetValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartLegend legend = d as ChartLegend;
            legend.OnOffsetValueChanged(e);
        }
        private void OnOffsetValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Parent is ChartDockPanel)
            {
                if (DockPosition == ChartDock.Floating)
                {
                    (Parent as ChartDockPanel).InvalidateArrange();
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegend"/> class.
        /// </summary>
        public ChartLegend()
        {
            this.DefaultStyleKey = typeof(ChartLegend);
            this.Loaded += ChartLegend_Loaded;
        }

        void ChartLegend_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeOrientation();
        }

        private void ChangeOrientation()
        {
            ItemsPresenter itemsPresenter = GetVisualChild<ItemsPresenter>(this);
            if (itemsPresenter != null)
            {
                if (VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
                {
#if WINDOWS_PHONE
                    StackPanel itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0) as StackPanel;
#else
                    StackPanel itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 1) as StackPanel;
#endif
                    if (itemsPanel != null)
                    {
                        itemsPanel.Orientation = (Orientation)Enum.Parse(typeof(Orientation)
                            , (this.Orientation == ChartOrientation.Default
                            ? ((this.DockPosition != ChartDock.Left && this.DockPosition != ChartDock.Right)
                            ? ChartOrientation.Horizontal : ChartOrientation.Vertical)
                            : this.Orientation).ToString(), false);
                    }
                }
            }
        }

        private static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject v = (DependencyObject)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        #endregion
    }

    /// <summary>
    /// class implementation for LegendItem
    /// </summary>
    public class LegendItem : DependencyObject, INotifyPropertyChanged
    {
        #region properties

        /// <summary>
        /// Get or Set Label property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(LegendItem), new PropertyMetadata(string.Empty, OnLabelChanged));

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LegendItem).OnPropertyChanged("Label");
        }

        /// <summary>
        /// Get or Set LegendIconTemplate property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate LegendIconTemplate
        {
            get { return (DataTemplate)GetValue(LegendIconTemplateProperty); }
            set { SetValue(LegendIconTemplateProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for LegendIconTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LegendIconTemplateProperty =
            DependencyProperty.Register("LegendIconTemplate", typeof(DataTemplate), typeof(LegendItem), new PropertyMetadata(null, OnLegendIconTemplateChanged));

        private static void OnLegendIconTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LegendItem).OnPropertyChanged("LegendIconTemplate");
        }

        /// <summary>
        /// Get or Set Interior property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
            DependencyProperty.Register("Interior", typeof(Brush), typeof(LegendItem), new PropertyMetadata(null, OnInteriorChanged));

        private static void OnInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LegendItem).OnPropertyChanged("Interior");
        }

        /// <summary>
        /// Get or Set IconVisibilityProperty
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Visibility IconVisibility
        {
            get { return (Visibility)GetValue(IconVisibilityProperty); }
            set { SetValue(IconVisibilityProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IconVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconVisibilityProperty =
            DependencyProperty.Register("IconVisibility", typeof(Visibility), typeof(LegendItem), new PropertyMetadata(Visibility.Collapsed, OnIconVisibilityChanged));

        private static void OnIconVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LegendItem).OnPropertyChanged("IconVisibility");
        }

        /// <summary>
        /// Get or Set CheckBoxVisibility
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Visibility CheckBoxVisibility
        {
            get { return (Visibility)GetValue(CheckBoxVisibilityProperty); }
            set { SetValue(CheckBoxVisibilityProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CheckBoxVisibilityProperty =
            DependencyProperty.Register("CheckBoxVisibility", typeof(Visibility), typeof(LegendItem), new PropertyMetadata(Visibility.Collapsed, OnCheckBoxVisibilityChanged));

        private static void OnCheckBoxVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LegendItem).OnPropertyChanged("CheckBoxVisibility");
        }

        /// <summary>
        /// Get or Set Iconwidth property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(LegendItem), new PropertyMetadata(double.NaN, OnIconWidthPropertyChanged));

        private static void OnIconWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LegendItem).OnPropertyChanged("IconWidth");
        }

        /// <summary>
        /// Get or Set IconHeightProperty
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for IconHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(LegendItem), new PropertyMetadata(double.NaN, OnIconHeightPropertyChanged));

        private static void OnIconHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LegendItem).OnPropertyChanged("IconHeight");
        }

        /// <summary>
        /// Get or Set ItemMarginProperty
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Thickness ItemMargin
        {
            get { return (Thickness)GetValue(ItemMarginProperty); }
            set { SetValue(ItemMarginProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemMargin.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register("ItemMargin", typeof(Thickness), typeof(LegendItem), new PropertyMetadata(new Thickness(0), OnItemMarginPropertyChanged));

        private static void OnItemMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LegendItem).OnPropertyChanged("ItemMargin");
        }

        /// <summary>
        /// Get or Set IsSeriesVisible property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool IsSeriesVisible
        {
            get { return (bool)GetValue(IsSeriesVisibleProperty); }
            set { SetValue(IsSeriesVisibleProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsSeriesVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSeriesVisibleProperty =
            DependencyProperty.Register("IsSeriesVisible", typeof(bool), typeof(LegendItem), new PropertyMetadata(true, OnSeriesVisible));

        private static void OnSeriesVisible(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LegendItem).OnPropertyChanged("IsSeriesVisible");
        }

        /// <summary>
        /// Get or Set VisibilityOnlegendProperty
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Visibility VisibilityOnLegend
        {
            get { return (Visibility)GetValue(VisibilityOnLegendProperty); }
            set { SetValue(VisibilityOnLegendProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibilityOnLegend.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibilityOnLegendProperty =
            DependencyProperty.Register("VisibilityOnLegend", typeof(Visibility), typeof(LegendItem), new PropertyMetadata(Visibility.Visible,OnVisibilityOnLegend));

        private static void OnVisibilityOnLegend(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LegendItem).OnPropertyChanged("VisibilityOnLegend");
        }
        private ChartSegment segment;
        /// <summary>
        /// Get or Set Segment property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartSegment Segment
        {
            get
            {
                return segment;
            }
            set
            {
                segment = value;
                if (segment != null)
                {
                    Binding binding = new Binding();
                    binding.Source = segment;
                    binding.Path = new PropertyPath("Interior");                    
                    binding.Converter = new InteriorConverter(segment.Series);
                    binding.ConverterParameter = segment.Series.Segments.IndexOf(segment);
                    BindingOperations.SetBinding(this, LegendItem.InteriorProperty, binding);
                }
            }
        }

        private object item;

        /// <summary>
        /// Get or Set Item property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }

        private ChartSeriesBase series;

        private TrendlineBase trendline;

        internal int Index { get; set; }

        /// <summary>
        /// Get or Set Trendline Property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public TrendlineBase Trendline
        {
            get
            {
                return trendline;
            }
            set
            {
                trendline = value;

                if (trendline != null)
                {
                    Binding binding = new Binding();
                    binding.Source = trendline;
                    binding.Path = new PropertyPath("Stroke");
                    BindingOperations.SetBinding(this, LegendItem.InteriorProperty, binding);
                    
                    binding = new Binding();
                    binding.Source = trendline;
                    binding.Path = new PropertyPath("Label");
                    BindingOperations.SetBinding(this, LegendItem.LabelProperty, binding);

                    binding = new Binding();
                    binding.Source = trendline;
                    binding.Path = new PropertyPath("LegendIconTemplate");
                    BindingOperations.SetBinding(this, LegendItem.LegendIconTemplateProperty, binding);

                    binding = new Binding();
                    binding.Source = trendline;
                    binding.Path = new PropertyPath("VisibilityOnLegend");
                    BindingOperations.SetBinding(this, LegendItem.VisibilityOnLegendProperty, binding);

                    binding = new Binding();
                    binding.Source = trendline;
                    binding.Mode = BindingMode.TwoWay;
                    binding.Path = new PropertyPath("IsTrendlineVisible");
                    BindingOperations.SetBinding(this, LegendItem.IsSeriesVisibleProperty, binding);
                }
            }
        }

        /// <summary>
        /// Get or Set Series Property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartSeriesBase Series
        {
            get
            {
                return series;
            }
            set
            {
                series = value;

                if (series != null)
                {
                    Binding binding = new Binding();
                    binding.Source = series;
                    binding.Path = new PropertyPath("Interior");
                    binding.Converter = new InteriorConverter(series);
                    binding.ConverterParameter = Index;
                    BindingOperations.SetBinding(this, LegendItem.InteriorProperty, binding);

                    binding = new Binding();
                    binding.Source = series;
                    binding.Path = new PropertyPath("Label");
                    BindingOperations.SetBinding(this, LegendItem.LabelProperty, binding);

                    binding = new Binding();
                    binding.Source = series;
                    binding.Path = new PropertyPath("LegendIconTemplate");
                    BindingOperations.SetBinding(this, LegendItem.LegendIconTemplateProperty, binding);

                    binding = new Binding();
                    binding.Source = series;
                    binding.Path = new PropertyPath("VisibilityOnLegend");
                    BindingOperations.SetBinding(this, LegendItem.VisibilityOnLegendProperty, binding);

                    binding = new Binding();
                    binding.Source = series;
                    binding.Mode = BindingMode.TwoWay;
                    binding.Path = new PropertyPath("IsSeriesVisible");
                    BindingOperations.SetBinding(this, LegendItem.IsSeriesVisibleProperty, binding);
                }
            }
        }

        private ChartLegend legend;

        internal ChartLegend Legend
        {
            get
            {
                return legend;
            }
            set
            {
                legend = value;

                if (legend != null)
                {
                    if (this.Segment != null && this.Segment.Series is AccumulationSeriesBase)
                    {
                        Binding binding = new Binding();
                        binding.Source = legend;
                        binding.Path = new PropertyPath("IconVisibility");
                        BindingOperations.SetBinding(this, LegendItem.IconVisibilityProperty, binding);

                        binding = new Binding();
                        binding.Source = legend;
                        binding.Path = new PropertyPath("CheckBoxVisibility");
                        BindingOperations.SetBinding(this, LegendItem.CheckBoxVisibilityProperty, binding);
                        this.CheckBoxVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Binding binding = new Binding();
                        binding.Source = legend;
                        binding.Path = new PropertyPath("IconVisibility");
                        BindingOperations.SetBinding(this, LegendItem.IconVisibilityProperty, binding);

                        binding = new Binding();
                        binding.Source = legend;
                        binding.Path = new PropertyPath("CheckBoxVisibility");
                        BindingOperations.SetBinding(this, LegendItem.CheckBoxVisibilityProperty, binding);

                        binding = new Binding();
                        binding.Source = legend;
                        binding.Path = new PropertyPath("IconWidth");
                        BindingOperations.SetBinding(this, LegendItem.IconWidthProperty, binding);

                        binding = new Binding();
                        binding.Source = legend;
                        binding.Path = new PropertyPath("IconHeight");
                        BindingOperations.SetBinding(this, LegendItem.IconHeightProperty, binding);

                        binding = new Binding();
                        binding.Source = legend;
                        binding.Path = new PropertyPath("ItemMargin");
                        BindingOperations.SetBinding(this, LegendItem.ItemMarginProperty, binding);
                    }
                }
            }
        }

        #endregion

        #region ctor

       

        #endregion

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

     
