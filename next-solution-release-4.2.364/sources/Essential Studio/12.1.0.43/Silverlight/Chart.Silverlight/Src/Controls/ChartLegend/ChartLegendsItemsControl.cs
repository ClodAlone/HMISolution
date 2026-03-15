#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
//using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// EventArgument for the SeriesVisibility
    /// </summary>
    public class SeriesVisibilityEventArg : EventArgs
    {
        private Visibility olddata, newdata;

        /// <summary>
        /// Called when instance created for SeriesvisibilityEventArg
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public SeriesVisibilityEventArg(Visibility oldValue, Visibility newValue)
        {
            this.olddata = oldValue;
            this.newdata = newValue;
        }

        /// <summary>
        /// Gets the NewValue of SeriesVisibilityEventArg
        /// </summary>
        public Visibility NewValue
        {
            get
            {
                return newdata;
            }
        }

        /// <summary>
        /// Gets the OldValue of SeriesVisibilityEventArg
        /// </summary>
        public Visibility OldValue
        {
            get
            {
                return olddata;
            }
        }

       
    }

    /// <summary>
    /// The ChartLegend class represents an Legends in the <see
    /// cref="ChartArea">ChartArea</see>.
    /// </summary>
    /// <remarks>
    /// This class generates data automatically based on the chart series
    /// </remarks>
    /// <seealso cref="ChartSeries">ChartSeries class specification</seealso>
    public class ChartLegend : ItemsControl,IDisposable
    {
        #region revamp code
        StackPanel legendArrangeStackPanel = null;

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (legendArrangeStackPanel == null && this.Items != null && this.Items.Count > 0)
            {
                LegendItem item = this.Items[0] as LegendItem;
                legendArrangeStackPanel = VisualTreeHelper.GetParent(item) as StackPanel;
                legendArrangeStackPanel.Orientation = this.Orientation;
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Area != null && Area.Series != null && Area.Series.Count > 0)
            {
                if (Area.GetSeriesAxesType(Area.Series[0].Type) == ChartAxesType.None || this.Items.Count == 0)
                {
                    GenerateItems();
                }
                else
                {
                    GenerateItems();
                }
            }
            else if (Area.Series.Count == 0)
            {
                this.Items.Clear();
            }
            if (Area.IsSync)
            {
                GenerateItems();
            }
            return base.MeasureOverride(availableSize);
        }

        void UpdateItems()
        {

        }

        internal void GenerateItems()
        {
            if (Area != null && Area.Series!=null && Area.Series.Count>0)
            {
                this.Items.Clear();

                if (Area.GetSeriesAxesType(Area.Series[0].Type) == ChartAxesType.None && IsSegmentsLegend)
                {
                    int index = 0;
                    SegmentsCollection actualsegments = new SegmentsCollection();
                    foreach (ChartSeries ser in Area.Series)
                    {
                        foreach (Segment segment in ser.Segments)
                        {
                            if (segment is ChartPieSegment)
                            {
                                actualsegments.Add(segment);
                            }
                        }
                    }
                    foreach (Segment segment in Area.Series[0].Segments)
                    {
                        if (segment is ChartPyramidSegment || segment is ChartFunnelSegment)
                        {
                            actualsegments.Add(segment);

                        }
                    }
                    foreach (Segment segment in actualsegments)
                    {
                        LegendItem legend = new LegendItem();
                        legend.DataContext  = segment;

                        ChartLegendBinding(this, new PropertyPath("IconVisibility"), BindingMode.TwoWay, legend, LegendItem.IconVisibilityProperty);
                        ChartLegendBinding(this, new PropertyPath("CheckboxVisibility"), BindingMode.TwoWay, legend, LegendItem.CheckboxVisibilityProperty);
                        ChartLegendBinding(segment, new PropertyPath("Label"), BindingMode.TwoWay, legend, LegendItem.LabelProperty);

                        if (index >= segment.Series.Data.Count)
                        {
                            index = 0;
                        }
                        legend.IsChecked = segment.Series.Data[index].Visible;

                        if (segment.Interior == null)
                        {
                            Brush[] colors = segment.Series.Area.ColorModel.GetBrushes(segment.Series.Area.ColorModel.Palette);
                            segment.Interior = (colors == null || colors.Count() == 0) ? new SolidColorBrush(Colors.Transparent) : colors[actualsegments.IndexOf(segment) % colors.Count()];
                        }

                        if (segment.Series.DataSource != null)
                        {
                            string str = DataBinding.GetPropertyDataAsObject(segment.Series.DataSource, segment.Series.BindingPathX)[index + 1].ToString();
                            if (DataBinding.GetPropertyType(segment.Series.DataSource, segment.Series.BindingPathX) == typeof(DateTime))
                            {
                                double val = double.Parse(str);
                                legend.Label = DateTime.FromOADate(val).ToString(segment.Series.XAxis != null ? segment.Series.XAxis.LabelDateTimeFormat : "MM/dd/yyyy");
                            }
                            else
                            {
                                legend.Label = str;
                            }
                        }
                        else
                        {
                            legend.Label = segment.Series.Data[index].X.ToString();
                        }

                        Shape rect = null;
                        if (LegendIcon == ChartLegendIcon.SeriesType)
                        {
                            legend.IconTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartLegend), segment.Series.Type + legendTemplateSuffix) as DataTemplate;
                            if (legend.IconTemplate != null)
                            {
                                rect = legend.IconTemplate.LoadContent() as Shape;
                                rect.Margin = new Thickness(0, 0, 10, 0);
                            }
                        }
                        else
                        {
                            legend.IconTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartLegend), LegendIcon + legendTemplateSuffix) as DataTemplate;
                            if (legend.IconTemplate != null)
                            {
                                rect = legend.IconTemplate.LoadContent() as Shape;
                                rect.Margin = new Thickness(0, 0, 10, 0);
                            }
                        }

                        
                        legend.SeriesVisibilityChanged += new LegendItem.SeriesVisibilityEventHandler(legend_SeriesVisibilityChanged);
                        legend.IconTemplate = null;
                        legend.IconContent = rect;
                        legend.Margin = new Thickness(3, 2, 3, 2);
                        this.Items.Add(legend);
                        index++;
                    }
                }
                else
                {
                    foreach (ChartSeries series in Area.Series)
                    {
                        if (series.IsVisbileOnLegend)
                        {
                            if (series.isseriesvisible == false && series.Type != ChartTypes.Pie && series.Type != ChartTypes.Doughnut)
                            {
                                continue;
                            }

                            LegendItem legend = new LegendItem();
                            legend.DataContext = series;                     
                            ChartLegendBinding(this, new PropertyPath("IconVisibility"), BindingMode.TwoWay, legend, LegendItem.IconVisibilityProperty);
                            ChartLegendBinding(this, new PropertyPath("CheckboxVisibility"), BindingMode.TwoWay, legend, LegendItem.CheckboxVisibilityProperty);
                            ChartLegendBinding(series, new PropertyPath("Label"), BindingMode.TwoWay, legend, LegendItem.LabelProperty);
                            ChartLegendBinding(series, new PropertyPath("Visibility"), BindingMode.TwoWay, legend, LegendItem.IsCheckedProperty, new VisibilityToBoolConverter() );
                            Brush interior = null;
                            if (series.Interior == null)
                            {
                                Brush[] colors = series.Area.ColorModel.GetBrushes(series.Area.ColorModel.Palette);
                                interior = (colors == null || colors.Count() == 0) ? new SolidColorBrush(Colors.Transparent) : colors[series.Area.Series.IndexOf(series) % colors.Count()];
                            }
                              
                            Shape rect = null;
                            if (LegendIcon == ChartLegendIcon.SeriesType)
                            {
                                legend.IconTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartLegend), series.Type + legendTemplateSuffix) as DataTemplate;
                                if (legend.IconTemplate != null)
                                {
                                    rect = legend.IconTemplate.LoadContent() as Shape;
                                    rect.Margin = new Thickness(0, 0, 10, 0);
                                }
                            }
                            else
                            {
                                legend.IconTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartLegend), LegendIcon + legendTemplateSuffix) as DataTemplate;
                                if (legend.IconTemplate != null)
                                {
                                    rect = legend.IconTemplate.LoadContent() as Shape;
                                    rect.Margin = new Thickness(0, 0, 10, 0);
                                }
                            }

                            legend.IconTemplate = null;
                            legend.IconContent = rect;
                            legend.SeriesVisibilityChanged += new LegendItem.SeriesVisibilityEventHandler(legend_SeriesVisibilityChanged);
                            legend.Margin = new Thickness(3, 2, 3, 2);
                            this.Items.Add(legend);
                        }
                    }
                }
            }
        }

        void legend_SeriesVisibilityChanged(object sender, SeriesVisibilityEventArg e)
        {
            List<object> items = sender as List<object>;
            LegendItem item=items[0] as LegendItem;
            ChartSeries series = items[1] as ChartSeries;
            Segment segment = items[1] as Segment;
            
            if (series != null)
            {
                series.Visibility = e.NewValue;
                OnSeriesVisibilityChanged(series, new SeriesVisibilityEventArg(e.OldValue, e.NewValue));

                if (series.Area != null)
                {
                    series.Area.LoadArea();
                }
            }
            else if (segment != null)
            {
                int index = item != null ? this.Items.IndexOf(item) : 0;
                index = (index >= segment.Series.Data.Count ) ? index % segment.Series.Data.Count : index;

                segment.Series.Data[index].Visible = e.NewValue == Visibility.Visible ? true : false;
                segment.Series.Area.LoadArea();
                OnSeriesVisibilityChanged(series, new SeriesVisibilityEventArg(e.OldValue, e.NewValue));
            }
        }

        void ChartLegendBinding(object source, PropertyPath path, BindingMode mode, DependencyObject target, DependencyProperty targetProperty)
        {
            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = path;
            binding.Mode = mode;
            BindingOperations.SetBinding(target, targetProperty, binding);
        }
        void ChartLegendBinding(object source, PropertyPath path, BindingMode mode, DependencyObject target, DependencyProperty targetProperty, IValueConverter converter)
        {
            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = path;
            binding.Mode = mode;
            binding.Converter = converter;
            BindingOperations.SetBinding(target, targetProperty, binding);
        }

        #endregion

        List<CheckBox> checkboxcollection = new List<CheckBox>();
        private ChartArea m_area = null;
        internal ChildWindow LegendEditorChildWindow;
        internal Visibility iconVisibility;
        internal Visibility checkboxVisibility;
        private DispatcherTimer _timer = new DispatcherTimer();
        private string legendTemplateSuffix = "_Legend";
        
        /// <summary>
        /// Series Visibility Event Handler for Chart Legend.
        /// </summary>
        /// <param name="sender">Sender Object.  It may be either ChartSeries or Segment based on Chart Type</param>
        /// <param name="e">Series Visibility EventArgument</param>
        public delegate void SeriesVisibilityEventHandler(object sender, SeriesVisibilityEventArg e);
        
        /// <summary>
        /// Event Raise when Series Visibility changed by the chart legend
        /// </summary>
        public event SeriesVisibilityEventHandler SeriesVisibilityChanged;

        /// <summary>
        /// Calls when Series Visiblity Change by the Legend Checkbox
        /// </summary>
        /// <param name="obj">Sender Object.  It may be either ChartSeries or Segment</param>
        /// <param name="e">Event Argument</param>
        protected virtual void OnSeriesVisibilityChanged(object obj, SeriesVisibilityEventArg e)
        {
            if (this.SeriesVisibilityChanged != null)
            {
                SeriesVisibilityChanged(obj, e);
            }
        }

        /// <summary>
        /// Identifies Checkbox Dependency Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ChartLegend), new PropertyMetadata(new CornerRadius(10)));

        /// <summary>
        /// Identifies Checkbox Dependency Property
        /// </summary>
        public static readonly DependencyProperty CheckboxVisibilityProperty =
            DependencyProperty.Register("CheckboxVisibility", typeof(Visibility), typeof(ChartLegend), new PropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnLegendsDataChanged)));

        /// <summary>
        /// Identifies Icon Dependency Property
        /// </summary>
        public static readonly DependencyProperty IconVisibilityProperty =
            DependencyProperty.Register("IconVisibility", typeof(Visibility), typeof(ChartLegend), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnLegendsDataChanged)));
        StackPanel legenddata;

        /// <summary>
        /// Identifies Orientation Dependency Property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartLegend), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Identifies Legend Position Dependency Property
        /// </summary>
        public static readonly DependencyProperty DockPositionProperty =
            DependencyProperty.Register("DockPosition", typeof(ChartDock), typeof(ChartLegend), new PropertyMetadata(ChartDock.Top, new PropertyChangedCallback(OnLegendsDataChanged)));
        /// <summary>
        /// Identifies the LegendIcon dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendIconProperty =
            DependencyProperty.Register("LegendIcon", typeof(ChartLegendIcon), typeof(ChartLegend), new PropertyMetadata(ChartLegendIcon.Rectangle, new PropertyChangedCallback(OnLegendIconChanged)));

        /// <summary>
        ///  Identifies the IsSegementlegend dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSegmentsLegendProperty =
          DependencyProperty.Register("IsSegmentsLegend", typeof(bool), typeof(ChartLegend), new PropertyMetadata(true, new PropertyChangedCallback(OnbooleanvalueChanged)));

        /// <summary>
        /// Called when instance created for ChartLegend
        /// </summary>
        public ChartLegend()
        {
            DefaultStyleKey = typeof(ChartLegend);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Interval = new TimeSpan(0,0,1);
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
        }

        /// <summary>
        /// Get or Set Area Property
        /// </summary>
        public ChartArea Area
        {
            get
            {
                if (m_area == null)
                {
                    m_area = GetParentArea();
                }

                return m_area;
                ////return this.GetParentArea();
            }
        }

        /// <summary>
        /// Get or Set IsSegmentsLegendProperty
        /// </summary>
        public bool IsSegmentsLegend
        {
            get
            {
                return (bool)GetValue(IsSegmentsLegendProperty);
            }

            set
            {
                SetValue(IsSegmentsLegendProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DockPositionProperty. This is dependency property.
        /// </summary>
        /// <value>The Visibility.</value>
        public ChartDock DockPosition
        {
            get
            {
                return (ChartDock)GetValue(DockPositionProperty);
            }

            set
            {
                SetValue(DockPositionProperty, value);
            }
        }

        /// <summary>
        /// Get or Set LegendIcon
        /// </summary>
        public ChartLegendIcon LegendIcon
        {
            get
            {
                return (ChartLegendIcon)GetValue(LegendIconProperty);
            }
            set
            {
                SetValue(LegendIconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CornerRadiusProperty. This is dependency property.
        /// </summary>
        /// <value>The CornerRadius value.</value>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CheckboxProperty. This is dependency property.
        /// </summary>
        /// <value>The Visibility.</value>
        public Visibility CheckboxVisibility
        {
            get
            {
                return (Visibility)GetValue(CheckboxVisibilityProperty);
            }

            set
            {
                SetValue(CheckboxVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the IconProperty. This is dependency property.
        /// </summary>
        /// <value>The Visibility.</value>
        public Visibility IconVisibility
        {
            get
            {
                return (Visibility)GetValue(IconVisibilityProperty);
            }

            set
            {
                SetValue(IconVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets the Orientation. This is dependency property.
        /// </summary>
        /// <value>The Orientation.</value>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        #region OrientationSealedOnDock
        /// <summary>
        /// Identifies OrientationSealedOnDock Dependency Property
        /// </summary>
        public static readonly DependencyProperty OrientationSealedOnDockProperty =
            DependencyProperty.Register("OrientationSealedOnDock", typeof(bool), typeof(LegendItem), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the OrientationSealedOnDock value. This is a dependency property
        /// </summary>
        public bool OrientationSealedOnDock
        {
            get { return (bool)GetValue(OrientationSealedOnDockProperty); }
            set { SetValue(OrientationSealedOnDockProperty, value); }
        }
        #endregion

        internal ChartArea GetParentArea()
        {
            DependencyObject element = this;
            while (!(element is ChartArea) && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as ChartArea;
            }

            return null;
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (_timer.IsEnabled)
            {
                LegendEditor legendeditor = new LegendEditor(this);
                LegendEditorChildWindow = new ChildWindow();
                LegendEditorChildWindow.Content = legendeditor;
                //VisualStyle vs = SkinManager.GetVisualStyle(this);
                //SkinManager.SetVisualStyle(legendeditor, vs);
                //SkinManager.SetApplyStyleOnLoad(legendeditor, true);
                LegendEditorChildWindow.Title = "Chart Legend Properties";
                LegendEditorChildWindow.Closed += new EventHandler(LegendEditorChildWindow_Closed);
                LegendEditorChildWindow.Show();
            }
            else
            {
                _timer.Start();
            }
        }

        void LegendEditorChildWindow_Closed(object sender, EventArgs e)
        {
            this.IconVisibility = this.iconVisibility;
            this.CheckboxVisibility = this.checkboxVisibility;
        }

        void LegendCheckbox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckbox = sender as CheckBox;
            int position = checkboxcollection.IndexOf(currentCheckbox);
            ChartSeries series = Area.Series[position];
            Visibility olddata = series.Visibility;
            if ((bool)currentCheckbox.IsChecked == false)
            {                
                series.Visibility = Visibility.Collapsed;
                series.XAxis.VisibleRange = DoubleRange.Empty;
                series.YAxis.VisibleRange = DoubleRange.Empty;
                if (series.XAxis.IsAutoSetRange == true)
                {
                    series.XAxis.Range = new DoubleRange(0, 1);
                }

                if (series.YAxis.IsAutoSetRange == true)
                {
                    series.YAxis.Range = new DoubleRange(0, 1);
                }
            }
            else
            {
                series.Visibility = Visibility.Visible;
            }

            Area.LoadArea();
            Visibility newdata = series.Visibility;
            if (olddata != newdata)
            {
                OnSeriesVisibilityChanged(series, new SeriesVisibilityEventArg(olddata, newdata));
            }
        }

        void SegmentLegendCheckbox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckbox = sender as CheckBox;
            int position = checkboxcollection.IndexOf(currentCheckbox);
            Segment segment = Area.Series[0].Segments[position];
            bool olddata = segment.Series.Data[position].Visible;
            if ((bool)currentCheckbox.IsChecked == false)
            {
                segment.Series.Data[position].Visible = false;
            }
            else
            {
                segment.Series.Data[position].Visible = true;
            }

            Area.LoadArea();
            bool newdata = segment.Series.Data[position].Visible;
            if (olddata != newdata)
            {
                OnSeriesVisibilityChanged(segment, new SeriesVisibilityEventArg(olddata ? Visibility.Visible : Visibility.Collapsed, newdata ? Visibility.Visible : Visibility.Collapsed));
            }
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.ItemsControl"/> when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            this.UpdateLegends();
        }

        private static void OnLegendsDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartLegend legends = (ChartLegend)d;
            if (legends != null)
            {
                if (!legends.OrientationSealedOnDock)
                {
                    legends.Orientation = legends.DockPosition == ChartDock.Bottom || legends.DockPosition == ChartDock.Top ? Orientation.Horizontal : Orientation.Vertical;
                }
                if (legends.Area != null)
                {
                    //legends.Area.LoadArea();
                    foreach (ChartAxis axis in legends.Area.Axes)
                    {
                        axis.UpdateAxis();
                    }
                }
            }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartLegend legend = (ChartLegend)d;
            if (legend != null && legend.legendArrangeStackPanel!=null)
            {
                legend.legendArrangeStackPanel.Orientation = legend.Orientation;
            }
        }

        private static void OnbooleanvalueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartLegend legend = d as ChartLegend;
            legend.GenerateItems();
        }

        private static void OnLegendIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
           ChartLegend type = (ChartLegend)d;
           //type.UpdateLegends();
           if (type != null)
           {
               type.InvalidateMeasure();
           }
           //MessageBox.Show("Event");
            
        }

        /// <summary>
        /// Method implementation for Update legends when Property changes occurred
        /// </summary>
        public void UpdateLegends()
        {
            //legenddata = GetTemplateChild("LegendData") as StackPanel;
            //if (legenddata != null && Area != null)
            //{
            //     legenddata.Children.Clear();
            //    checkboxcollection.Clear();
            //    SeriesCollection areaSeries = Area.Series;
            //    if (Area.Series.Count == 0)
            //    {
            //        DrawSeriesLegends(new SeriesCollection());
            //    }
            //    else if (Area.firstseriestype == ChartAxesType.None)
            //    {
            //        DrawSegementsLegends(areaSeries[0].Segments);
            //    }
            //    else
            //    {
            //        DrawSeriesLegends(areaSeries);
            //    }
            //}
        }

        //private void DrawSeriesLegends(SeriesCollection areaSeries)
        //{
        //    foreach (ChartSeries series in areaSeries)
        //    {
        //        if (series.isseriesvisible == false)
        //        {
        //            continue;
        //        }

        //        StackPanel insidestackpanel = new StackPanel();
        //        insidestackpanel.Orientation = Orientation.Horizontal;
        //        insidestackpanel.Margin = new Thickness(5, 5, 5, 5);
        //        CheckBox legendCheckbox = new CheckBox();
        //        if (series.Visibility == Visibility.Visible)
        //        {
        //            legendCheckbox.IsChecked = true;
        //        }
        //        else
        //        {
        //            legendCheckbox.IsChecked = false;
        //        }

        //        legendCheckbox.Visibility = CheckboxVisibility;
        //        legendCheckbox.VerticalAlignment = VerticalAlignment.Center;
        //        checkboxcollection.Add(legendCheckbox);
        //        legendCheckbox.Click += new RoutedEventHandler(LegendCheckbox_Click);
        //        ContentPresenter content = new ContentPresenter();
        //        content.Width = 25;
        //        content.Height = 15;
        //        content.DataContext = series;

        //        Brush Interior = null;
        //        if (series.Interior == null)
        //        {
        //            Brush[] colors = series.Area.ColorModel.GetBrushes(series.Area.ColorModel.Palette);
        //            Interior = (colors == null || colors.Count() == 0) ? new SolidColorBrush(Colors.Transparent) : colors[series.Area.Series.IndexOf(series) % colors.Count()];
        //        }

        //        Shape rect = null;    
        //        if (LegendIcon == ChartLegendIcon.SeriesType)
        //        {
        //            content.ContentTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartLegend), series.Type + legendTemplateSuffix) as DataTemplate;
        //            if (content.ContentTemplate != null)
        //            {
        //                rect = content.ContentTemplate.LoadContent() as Shape;
        //                rect.Margin = new Thickness(0,0,10,0);
        //                rect.Fill = series.Interior;
        //            if (series.Type == ChartTypes.FastLine || series.Type == ChartTypes.HiLo || series.Type == ChartTypes.HiLoOpenClose || series.Type == ChartTypes.Line || series.Type == ChartTypes.StepLine || series.Type == ChartTypes.Kagi || series.Type == ChartTypes.Renko || series.Type == ChartTypes.Radar || series.Type == ChartTypes.Polar || series.Type == ChartTypes.Spline || series.Type == ChartTypes.RotatedSpline)
        //                rect.Stroke = series.Interior != null ? series.Interior : Interior;
        //            else
        //                rect.Stroke = series.Stroke;

        //            rect.StrokeThickness = series.StrokeThickness;
        //            rect.Visibility = IconVisibility;
        //            }
        //        }
        //        else
        //        {
        //            content.ContentTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartLegend), LegendIcon + legendTemplateSuffix) as DataTemplate;
        //            if (content.ContentTemplate != null)
        //            {
        //                rect = content.ContentTemplate.LoadContent() as Shape;
        //                rect.Margin = new Thickness(0, 0, 10, 0);
        //                rect.Fill = series.Interior;
        //            if (LegendIcon == ChartLegendIcon.StraightLine || LegendIcon == ChartLegendIcon.Cross)
        //                rect.Stroke = series.Interior;
        //            else
        //                rect.Stroke = series.Stroke;
        //            rect.StrokeThickness = series.StrokeThickness;
        //            rect.Visibility = IconVisibility;
        //            }
        //        }
        //          TextBlock legendText = new TextBlock();
        //          legendText.Text = series.Label;
        //          legendText.VerticalAlignment = VerticalAlignment.Center;
        //          insidestackpanel.Children.Add(legendCheckbox);
        //          if (rect != null)
        //          {
        //              insidestackpanel.Children.Add(rect);
        //          }

        //          insidestackpanel.Children.Add(legendText);
        //          legenddata.Children.Add(insidestackpanel);
        //         }
        //}
        //private void DrawSegementsLegends(SegmentsCollection segments)
        //{
        //    SegmentsCollection actualsegments = new SegmentsCollection();
        //    foreach (Segment segment in segments)
        //    {
        //        if (segment is ChartPieSegment || segment is ChartPyramidSegment || segment is ChartFunnelSegment)
        //        {
        //            actualsegments.Add(segment);
        //        }
        //    }

        //    int count = 0;
        //    foreach (Segment segment in actualsegments)
        //    {
        //        StackPanel insidestackpanel = new StackPanel();
        //        insidestackpanel.Orientation = Orientation.Horizontal;
        //        insidestackpanel.Margin = new Thickness(5, 5, 5, 5);
        //        CheckBox legendCheckbox = new CheckBox();
        //        if (segment.Series.Data[count].Visible == true && segment.Series.Visibility == Visibility.Visible)
        //        {
        //            legendCheckbox.IsChecked = true;
        //        }
        //        else
        //        {
        //            legendCheckbox.IsChecked = false;
        //        }
        //        legendCheckbox.Visibility = CheckboxVisibility;
        //        legendCheckbox.VerticalAlignment = VerticalAlignment.Center;
        //        checkboxcollection.Add(legendCheckbox);
        //        legendCheckbox.Click += new RoutedEventHandler(SegmentLegendCheckbox_Click);

        //        ContentPresenter content = new ContentPresenter();
        //        content.Width = 15;
        //        content.Height = 15;
        //        content.DataContext = segment;

        //        if (segment.Interior == null)
        //        {
        //            Brush[] colors = segment.Series.Area.ColorModel.GetBrushes(segment.Series.Area.ColorModel.Palette);
        //            segment.Interior = (colors == null || colors.Count() == 0) ? new SolidColorBrush(Colors.Transparent) : colors[actualsegments.IndexOf(segment) % colors.Count()];
        //        }
                
        //        TextBlock legendText = new TextBlock();
        //        if (segment.Series.DataSource != null)
        //        {
        //            string str = DataBinding.GetPropertyDataAsObject(segment.Series.DataSource, segment.Series.BindingPathX)[count + 1].ToString();
        //            if (DataBinding.GetPropertyType(segment.Series.DataSource, segment.Series.BindingPathX) == typeof(DateTime))
        //            {
        //                double val = double.Parse(str);
        //                legendText.Text = DateTime.FromOADate(val).ToString(segment.Series.XAxis != null ? segment.Series.XAxis.LabelDateTimeFormat : "MM/dd/yyyy");
        //            }
        //            else
        //            {
        //                legendText.Text = str;
        //            }
        //        }
        //        else
        //        {
        //            legendText.Text = segment.Series.Data[count].X.ToString();
        //        }

        //        Shape rect = null;
        //        if (LegendIcon == ChartLegendIcon.SeriesType)
        //        {
        //            content.ContentTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartLegend), segment.Series.Type + legendTemplateSuffix) as DataTemplate;
        //            if (content.ContentTemplate != null)
        //            {
        //                rect = content.ContentTemplate.LoadContent() as Shape;
        //                rect.Fill = segment.Interior;
        //                rect.Margin = new Thickness(0, 0, 10, 0);
        //                rect.Stroke = segment.Stroke;
        //                rect.StrokeThickness = segment.StrokeThickness;
        //                rect.Visibility = IconVisibility;
        //            }
        //        }
        //        else
        //        {
        //            content.ContentTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartLegend), LegendIcon + legendTemplateSuffix) as DataTemplate;
        //            if (content.ContentTemplate != null)
        //            {
        //                rect = content.ContentTemplate.LoadContent() as Shape;
        //                rect.Fill = segment.Interior;
        //                rect.Margin = new Thickness(0, 0, 10, 0);
        //                if (LegendIcon == ChartLegendIcon.StraightLine || LegendIcon == ChartLegendIcon.Cross)
        //                    rect.Stroke = segment.Interior;
        //                else
        //                    rect.Stroke = segment.Stroke;
        //                rect.StrokeThickness = segment.StrokeThickness;
        //                rect.Visibility = IconVisibility;
        //            }
        //        }
        //        legendText.VerticalAlignment = VerticalAlignment.Center;
        //        insidestackpanel.Children.Add(legendCheckbox);

        //        if (rect != null)
        //        {
        //            insidestackpanel.Children.Add(rect);
        //        }

        //        insidestackpanel.Children.Add(legendText);
        //        legenddata.Children.Add(insidestackpanel);
        //        count++;
        //    }
        //}

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
           

            if (this.checkboxcollection != null)
            {
                for (int temp = 0; temp < this.checkboxcollection.Count; temp++)
                {
                    CheckBox legendCheckbox = checkboxcollection[temp];
                    legendCheckbox.Click -= new RoutedEventHandler(SegmentLegendCheckbox_Click);
                    legendCheckbox.Click -= new RoutedEventHandler(LegendCheckbox_Click);
                }
                this.checkboxcollection.Clear();
                this.checkboxcollection = null;
            }
            this.m_area = null;
            if (this.legenddata != null)
            {
                this.legenddata.Children.Clear();
                this.legenddata = null;
            }
            if (_timer != null)
                _timer.Tick -= new EventHandler(_timer_Tick);
            if (this.LegendEditorChildWindow != null)
                LegendEditorChildWindow.Closed -= new EventHandler(LegendEditorChildWindow_Closed);
            this.ClearValue(LegendIconProperty);
            this.ClearValue(ChartLegend.DockPositionProperty);
            this.Resources.Clear();
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// The LegendVisibilityConverter is used to determine the Visibility of Legend in ChartArea.
    /// </summary>
    public class LegendVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }
    /// <summary>
    /// Return object from given object
    /// </summary>
    public class Converter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// The LegendPositionConverter is used to determine the Legend Position is ChartArea
    /// </summary>
    public class LegendPositionConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartDock data = (ChartDock)values;
            Dock result = Dock.Top;
            switch (data)
            {
                case ChartDock.Top:
                    result = Dock.Top;
                    break;
                case ChartDock.Bottom:
                    result = Dock.Bottom;
                    break;
                case ChartDock.Left:
                    result = Dock.Left;
                    break;
                case ChartDock.Right:
                    result = Dock.Right;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }
}
