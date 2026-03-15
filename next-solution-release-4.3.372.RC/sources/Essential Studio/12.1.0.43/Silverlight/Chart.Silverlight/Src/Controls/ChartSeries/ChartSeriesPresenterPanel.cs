#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Linq;
    using System.Collections.Specialized;
    using System.Net;
    using System.Windows;
    using System.Windows.Data;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Collections.ObjectModel;
    /// <summary>
    /// Class implementation for seriesPresenter 
    /// </summary>
    public class SeriesPresenter : Panel, IDisposable
    {
        /// <summary>
        /// Identifies SegmentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentsProperty =
            DependencyProperty.Register("Segments", typeof(SegmentsCollection), typeof(SeriesPresenter), new PropertyMetadata(null, OnSegmentsPropertyChanged));

        /// <summary>
        /// Identifies SegmentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentTemplateProperty =
            DependencyProperty.Register("SegmentTemplate", typeof(DataTemplate), typeof(SeriesPresenter), new PropertyMetadata(null, OnSegmentTemplateChanged));

        internal Size TotalSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the segments collection. This is a dependency property.
        /// </summary>
        /// <value>The segments.</value>
        public SegmentsCollection Segments
        {
            get
            {
                return (SegmentsCollection)GetValue(SegmentsProperty);
            }

            set
            {
                SetValue(SegmentsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segment template. This is a dependency property.
        /// </summary>
        /// <value>The segment template.</value>
        public DataTemplate SegmentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(SegmentTemplateProperty);
            }

            set
            {
                SetValue(SegmentTemplateProperty, value);
            }
        }

        private Canvas canvas;
        /// <summary>
        /// Provides the behavior for the "Arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement ui in Children)
            {
                ui.Arrange(new Rect(0, 0, ui.DesiredSize.Width, ui.DesiredSize.Height));
                if (VisualTreeHelper.GetChildrenCount(ui) > 0)
                {
                    canvas = VisualTreeHelper.GetChild(ui, 0) as Canvas;
                    if (canvas != null)
                    {
                        canvas.MouseEnter -= new MouseEventHandler(canvas_MouseEnter);
                        canvas.MouseLeave -= new MouseEventHandler(canvas_MouseLeave);

                        canvas.MouseEnter += new MouseEventHandler(canvas_MouseEnter);
                        canvas.MouseLeave += new MouseEventHandler(canvas_MouseLeave);
                    }
                }
            }

            return base.ArrangeOverride(finalSize);
        }

        void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (((Canvas)sender).DataContext is Segment)
            {
                Segment segment = ((Segment)((Canvas)sender).DataContext);
                if (segment != null && segment.Series != null)
                {
                    ChartSeries series = segment.Series;
                    series.OnMouseLeave(series, new ChartMouseEventArgs(e, ((Segment)((Canvas)sender).DataContext)));
                }
            }
        }

        void canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            if (((Canvas)sender).DataContext is Segment)
            {
                Segment segment = ((Segment)((Canvas)sender).DataContext);
                if (segment != null && segment.Series != null)
                {
                    ChartSeries series = segment.Series;
                    series.OnMouseEnter(series, new ChartMouseEventArgs(e, ((Segment)((Canvas)sender).DataContext)));
                }
            }
        }

        internal Canvas GetParentCanvas()
        {
            DependencyObject element = this;
            while (!(element is Canvas))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element is Canvas)
            {
                return element as Canvas;
            }
            else
            {
                return null;
            }
        }

        internal Grid GetParentGrid()
        {
            DependencyObject element = this;
            while (!(element is Grid))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element is Grid)
            {
                return element as Grid;
            }
            else
            {
                return null;
            }
        }

        internal ChartArea GetParentArea()
        {
            DependencyObject element = this;
            while (!(element is ChartArea))
            {
                element = VisualTreeHelper.GetParent(element);
                if (element == null)
                {
                    return null;
                }
            }

            if (element is ChartArea)
            {
                return element as ChartArea;
            }
            else
            {
                return null;
            }
        }
        internal void LoadIndicators(ChartSeries series)
        {
            if (series != null && series.Indicators != null)
            {

                foreach (ChartTechnicalIndicator indicator in series.Indicators.Items)
                {
                    ObservableCollection<ContentPresenter> coll = new ObservableCollection<ContentPresenter>();
                    if (indicator != null)
                    {
                        indicator.Series = series;
                        indicator.VisiblePoints = series.Data;

                        switch (indicator.IndicatorType)
                        {
                            case IndicatorTypes.BollingerBands:
                                {
                                    foreach (var item in this.Children)
                                    {
                                        if (((item as ContentPresenter).Content is ChartBollingerBand) || ((item as ContentPresenter).Content is SimpleAverage) || ((item as ContentPresenter).Content is ChartTriangularAverage))
                                        {
                                            coll.Add(item as ContentPresenter);
                                        }
                                    }
                                    foreach (var item in coll)
                                    {
                                        this.Children.Remove(item);
                                    }
                                    ContentPresenter presenter = new ContentPresenter();

                                    if (indicator.BollingerIndicator != null && indicator.BollingerIndicator.UpperbandPresenter != null)
                                    {
                                        presenter = indicator.BollingerIndicator.UpperbandPresenter;
                                    }
                                    if (Children.Contains(presenter))
                                    {
                                        Children.Remove(presenter);
                                    }
                                    Children.Add(presenter);

                                    presenter = new ContentPresenter();
                                    if (indicator.BollingerIndicator != null && indicator.BollingerIndicator.Lowerbandpresenter != null)
                                    {
                                        presenter = indicator.BollingerIndicator.Lowerbandpresenter;
                                    }
                                    if (Children.Contains(presenter))
                                    {
                                        Children.Remove(presenter);
                                    }
                                    Children.Add(presenter);

                                    presenter = new ContentPresenter();
                                    if (indicator.BollingerIndicator != null && indicator.BollingerIndicator.SignalPresenter != null)
                                    {
                                        presenter = indicator.BollingerIndicator.SignalPresenter;
                                    }
                                    if (Children.Contains(presenter))
                                    {
                                        Children.Remove(presenter);
                                    }
                                    Children.Add(presenter);
                                }
                                break;
                            case IndicatorTypes.SimpleAverage:
                                foreach (var item in this.Children)
                                {
                                    if (((item as ContentPresenter).Content is SimpleAverage) || ((item as ContentPresenter).Content is ChartBollingerBand) || ((item as ContentPresenter).Content is ChartTriangularAverage))
                                    {
                                        coll.Add(item as ContentPresenter);
                                    }
                                }
                                foreach (var item in coll)
                                {
                                    this.Children.Remove(item);
                                }
                                ContentPresenter simplepresenter = new ContentPresenter();

                                if (indicator.simpleaverage != null && indicator.simpleaverage.SimpleAveragePresenter != null)
                                {
                                    simplepresenter = indicator.simpleaverage.SimpleAveragePresenter;
                                }
                                if (Children.Contains(simplepresenter))
                                {
                                    Children.Remove(simplepresenter);
                                }
                                Children.Add(simplepresenter);
                                break;
                            case IndicatorTypes.TriangularAverage:
                                foreach (var item in this.Children)
                                {
                                    if (((item as ContentPresenter).Content is SimpleAverage) || ((item as ContentPresenter).Content is ChartBollingerBand) || ((item as ContentPresenter).Content is ChartTriangularAverage))
                                    {
                                        coll.Add(item as ContentPresenter);
                                    }
                                }
                                foreach (var item in coll)
                                {
                                    this.Children.Remove(item);
                                }
                                ContentPresenter triangularpresenter = new ContentPresenter();

                                if (indicator.trinagularIndicator != null && indicator.trinagularIndicator.SimpleAveragePresenter != null)
                                {
                                    triangularpresenter = indicator.trinagularIndicator.SimpleAveragePresenter;
                                }
                                if (Children.Contains(triangularpresenter))
                                {
                                    Children.Remove(triangularpresenter);
                                }
                                Children.Add(triangularpresenter);
                                break;
                        }
                        indicator.VisiblePoints = null;
                    }

                }

            }
        }
        private Chart GetChartParent(ChartArea area)
        {
            UIElement obj = area;
            while (typeof(Chart) != obj.GetType())
            {
                obj = VisualTreeHelper.GetParent(obj) as UIElement;
            }
            return obj as Chart;
        }

        internal void LoadSegments()
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                (this.Children[i] as ContentPresenter).Content = null;
                (this.Children[i] as ContentPresenter).ContentTemplate = null;
            }
            this.Children.Clear();

            SegmentsCollection segments = Segments as SegmentsCollection;
            try
            {
                if (segments != null)
                {
                    foreach (Segment segment in segments)
                    {
                        ////If First Series Axes Type is None.  then no other series would display
                        if (segments[0].Series.isseriesvisible || segment.Series.Type == ChartTypes.Pie || segment.Series.Type == ChartTypes.Doughnut)
                        {
                            ContentPresenter presenter = new ContentPresenter();
                            presenter.Content = segment;
                            if (segment is ChartAdornment)
                            {
                                presenter.ContentTemplate = ((ChartAdornment)segment).Template;
                            }
                            else
                            {
                                if (segment.Series.Type == ChartTypes.Histogram && segment is FastLineSegment)
                                {
                                    presenter.ContentTemplate = ((FastLineSegment)segment).Template;
                                    ((FastLineSegment)segment).Interior = new SolidColorBrush(Colors.Black);
                                }
                                else
                                {
                                    presenter.ContentTemplate = segment.SegmentTemplate;//SegmentTemplate;
                                }
                            }

                            Children.Add(presenter);
                        }
                    }
                }
            }
            catch (Exception)
            {
                this.Segments.Clear();
            }
        }

        internal void LoadAdornments(SeriesCollection data)
        {
            List<UIElement> ele = new List<UIElement>();
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (((ContentPresenter)this.Children[i]).Content is ChartAdornment)
                {
                    ele.Add(this.Children[i]);
                }
            }

            for (int i = 0; i < ele.Count; i++)
            {
                this.Children.Remove(ele[i]);
                ele[i] = null;
            }

            foreach (ChartSeries ser in data)
            {
                if (ser.Visibility == Visibility.Visible)
                {
                    foreach (Segment seg in ser.Adornments)
                    {
                        if (seg.Series.isseriesvisible || seg.Series.Type == ChartTypes.Pie || seg.Series.Type == ChartTypes.Doughnut)
                        {
                            ContentPresenter presenter = new ContentPresenter();
                            presenter.Content = seg;
                            if (seg is ChartAdornment)
                            {
                                presenter.ContentTemplate = ((ChartAdornment)seg).Template;
                            }
                            else
                            {
                                presenter.ContentTemplate = seg.SegmentTemplate;// SegmentTemplate;
                            }

                            this.Children.Add(presenter);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Grid canvas = VisualTreeHelper.GetParent(this) as Grid;
            ChartSeries series = VisualTreeHelper.GetParent(canvas) as ChartSeries;

            ////Canvas canvas = GetParentCanvas();
            ////Grid grid = GetParentGrid();
            ChartArea area = GetParentArea();
            if (area != null)
            {
                if (double.IsPositiveInfinity(availableSize.Width))
                {
                    availableSize.Width = series.Area.seriesItemControl.ActualWidth;
                }

                if (double.IsPositiveInfinity(availableSize.Height))
                {
                    availableSize.Height = series.Area.seriesItemControl.ActualHeight;
                }

                if (area.AreaType == ChartAxesType.None || area.AreaType == ChartAxesType.RadarAxes || area.AreaType == ChartAxesType.PolarAxes)
                {
                    ////availableSize = area.axissize;
                    //availableSize = new Size(area.grid.ActualWidth, area.grid.ActualHeight);
                    //RectangleGeometry rectclip = new RectangleGeometry();
                    //if (area.seriesGrid != null)
                    //{
                    //    rectclip.Rect = new Rect(0, 0, availableSize.Width, availableSize.Height);
                    //    //rectclip.Transform = new TranslateTransform() { X = 0, Y = series.StrokeThickness * -1 };
                    //    area.seriesGrid.Clip = rectclip;
                    //}

                    RectangleGeometry rectgeo = new RectangleGeometry();
                    rectgeo.Rect = new Rect(0, 0, availableSize.Width, availableSize.Height);
                    area.seriesItemControl.Clip = rectgeo;
                }
                else if (area != null && area.axissize.Height != 0 && area.axissize.Width != 0 && area.isZoomactivated && area.IsZoomAllAxes)
                {
                    double scrollheight = 0;
                    double scrollwidth = 0;
                    if (area.HorizontalBar != null && area.VerticalBar != null)
                    {
                        if (area.HorizontalScrollBarVisibility == Visibility.Collapsed)
                        {
                            scrollheight = area.HorizontalBar.ActualHeight;
                        }

                        if (area.VerticalScrollBarVisibility == Visibility.Collapsed)
                        {
                            scrollwidth = area.VerticalBar.ActualWidth;
                        }
                    }

                    double height = area.axissize.Height - (area.AxesThickness.Top + area.AxesThickness.Bottom);
                    double width = area.axissize.Width - (area.AxesThickness.Right + area.AxesThickness.Left);

                    if (height > 0 && width > 0)
                    {
                        availableSize.Height = height;
                        availableSize.Width = width;
                    }
                    else
                    {
                        availableSize.Height = availableSize.Width = 0;
                    }
                }
                else
                {
                    RectangleGeometry rectgeo = new RectangleGeometry();
                    rectgeo.Rect = new Rect(0, 0, availableSize.Width, availableSize.Height);
                    area.seriesItemControl.Clip = rectgeo;
                }
            }

            this.TotalSize = availableSize;
            IChartTransformer transformer = ChartTransform.CreateCartesian(new Rect(new Point(0, 0), availableSize), series);
            List<ContentPresenter> val = (from data in this.Children.OfType<ContentPresenter>() where data.Content is Segment select data).Cast<ContentPresenter>().ToList();
            foreach (ContentPresenter ui in val)
            {
                ((Segment)ui.Content).Update(transformer);
                ui.Measure(availableSize);
                ChartAdornment adorn = ui.Content as ChartAdornment;
                if (adorn != null && adorn.Series.AdornmentsInfo != null)
                {
                    InitializeAdornments(ui, adorn.Series, availableSize);
                }
            }

            if (this.Children.Count != 0 && series.EnableAnimation == true && series.Animation != null && series.Area != null)
            {
                if (series.Area.isFirstTimeAnimate || series.IsRefreshAnimation)
                    series.Animation.AnimateSeries(true);
                series.IsRefreshAnimation = false;
                if (series.Area.isFirstTimeAnimate)
                {
                    //series.Area.isFirstTimeAnimate = false;
                }
            }

            return base.MeasureOverride(availableSize);
        }

        internal void update()
        {
            Line l = new Line();
            l.Visibility = Visibility.Collapsed;
            this.Children.Add(l);
            this.Children.Remove(l);
        }
        internal static T FindParent<T>(UIElement control) where T : UIElement
        {
            if (control != null)
            {
                UIElement p = VisualTreeHelper.GetParent(control) as UIElement;
                if (p != null)
                {
                    if (p is T)
                        return p as T;
                    else
                        return SeriesPresenter.FindParent<T>(p);
                }
            }
            return null;
        }

        private void InitializeAdornments(UIElement ui, ChartSeries series, Size availableSize)
        {
            Canvas canvas = VisualTreeHelper.GetChild(ui as ContentPresenter, 0) as Canvas;
            ////Adornment Label
            ContentControl ele = VisualTreeHelper.GetChild(canvas, 0) as ContentControl;

            ////Adornment Symbol
            ContentControl cntrl = VisualTreeHelper.GetChild(canvas, 1) as ContentControl;

            ////Segement Connector
            ContentControl connector = VisualTreeHelper.GetChild(canvas, 2) as ContentControl;

            Binding heightBinding = new Binding();
            heightBinding.Source = series.AdornmentsInfo;
            heightBinding.Path = new PropertyPath("SymbolHeight");
            heightBinding.Mode = BindingMode.OneWay;
            heightBinding.Converter = new SymbolHeightWidthConvertor();
            heightBinding.ConverterParameter = series;
            BindingOperations.SetBinding(cntrl, ContentControl.HeightProperty, heightBinding);


            Binding widthBinding = new Binding();
            widthBinding.Source = series.AdornmentsInfo;
            widthBinding.Path = new PropertyPath("SymbolWidth");
            widthBinding.Mode = BindingMode.OneWay;
            widthBinding.Converter = new SymbolHeightWidthConvertor();
            widthBinding.ConverterParameter = series;
            BindingOperations.SetBinding(cntrl, ContentControl.WidthProperty, widthBinding);

            ////Binding Segment Label
            Binding labelSegment = new Binding();
            labelSegment.Source = series.AdornmentsInfo;
            labelSegment.Path = new PropertyPath("SegmentLabelContent");
            labelSegment.Mode = BindingMode.OneWay;
            labelSegment.Converter = new SegmentLabelConverter();
            labelSegment.ConverterParameter = (ui as ContentPresenter).Content as ChartAdornment;
            BindingOperations.SetBinding(ele, ContentControl.ContentProperty, labelSegment);

            ////Binding Adornments Position
            ele.Measure(availableSize);
            List<object> data = new List<object>();
            ////if (series.AdornmentsInfo.SegmentLabelRotation != 0)
            ////{
            ////    data.Add(ele.DesiredSize.Width);
            ////}
            ////else
            ////{
            data.Add(ele.DesiredSize.Height);
            ////}

            data.Add(availableSize.Height);
            //data.Add(Canvas.GetTop(ele));
            data.Add((ele != null && ele.DataContext is ChartAdornment) ? (ele.DataContext as ChartAdornment).Y : Canvas.GetTop(ele));
            data.Add(series);
            data.Add(connector);
            if ((ui as ContentPresenter).Content is ChartPieAdornment)
            {
                data.Add(((ui as ContentPresenter).Content as ChartPieAdornment).m_angle);
            }
            else
            {
                data.Add(0d);
            }

            Binding positionLabel = new Binding();
            positionLabel.Source = series.AdornmentsInfo;
            positionLabel.Path = new PropertyPath("AdornmentsPosition");
            positionLabel.Mode = BindingMode.OneWay;
            positionLabel.Converter = new PositionAdornmentLabel();
            positionLabel.ConverterParameter = data;
            BindingOperations.SetBinding(ele, Canvas.TopProperty, positionLabel);
            BindingOperations.SetBinding(cntrl, Canvas.TopProperty, positionLabel);

            List<object> datax = new List<object>();
            //datax.Add(Canvas.GetLeft(ele));
            datax.Add((ele != null && ele.DataContext is ChartAdornment) ? (ele.DataContext as ChartAdornment).X : Canvas.GetLeft(ele));
            datax.Add(series);
            datax.Add(connector);
            if ((ui as ContentPresenter).Content is ChartPieAdornment)
            {
                datax.Add(((ui as ContentPresenter).Content as ChartPieAdornment).m_angle);
            }
            else
            {
                datax.Add(0d);
            }

            Binding xPosition = new Binding();
            xPosition.Source = series.AdornmentsInfo;
            xPosition.Path = new PropertyPath("AdornmentsPosition");
            xPosition.Mode = BindingMode.OneWay;
            xPosition.Converter = new XPositionAdornmentLabel();
            xPosition.ConverterParameter = datax;
            BindingOperations.SetBinding(ele, Canvas.LeftProperty, xPosition);
            BindingOperations.SetBinding(cntrl, Canvas.LeftProperty, xPosition);

            ////Binding Adornments Horizontal Alignment
            List<object> horizontaldata = new List<object>();
            horizontaldata.Add(ele.DesiredSize.Width);
            horizontaldata.Add(Canvas.GetLeft(ele));
            horizontaldata.Add(series.AdornmentsInfo.SymbolWidth);
            horizontaldata.Add(availableSize);
            if (ui is ContentPresenter)
            {
                horizontaldata.Add((ui as ContentPresenter).Content as ChartAdornment);
            }
            horizontaldata.Add(series.Type);
            Binding hAlign = new Binding();
            hAlign.Source = series.AdornmentsInfo;
            hAlign.Path = new PropertyPath("HorizontalAlignment");
            hAlign.Mode = BindingMode.OneWay;
            hAlign.Converter = new AdornmentHorizontalConverter();
            hAlign.ConverterParameter = horizontaldata;
            BindingOperations.SetBinding(ele, Canvas.LeftProperty, hAlign);

            ////Binding Adornments Vertical Alignment
            List<object> verticaldata = new List<object>();
            verticaldata.Add(ele.DesiredSize.Height);
            verticaldata.Add(Canvas.GetTop(ele));
            verticaldata.Add(series.AdornmentsInfo.SymbolHeight);
            verticaldata.Add(availableSize);
            if (ui is ContentPresenter)
            {
                verticaldata.Add((ui as ContentPresenter).Content as ChartAdornment);
            }
            verticaldata.Add(series.Type);
            Binding vAlign = new Binding();
            vAlign.Source = series.AdornmentsInfo;
            vAlign.Path = new PropertyPath("VerticalAlignment");
            vAlign.Mode = BindingMode.OneWay;
            vAlign.Converter = new AdornmentVerticalConverter();
            vAlign.ConverterParameter = verticaldata;
            BindingOperations.SetBinding(ele, Canvas.TopProperty, vAlign);

            ////Binding Adornment Label Rotation
            Binding rotate = new Binding();
            List<object> rotatedata = new List<object>();
            rotatedata.Add(ele);
            rotatedata.Add(series);
            rotate.Source = series.AdornmentsInfo;
            rotate.Path = new PropertyPath("SegmentLabelRotation");
            rotate.Mode = BindingMode.OneWay;
            rotate.Converter = new RotateAdornmentLabel();
            rotate.ConverterParameter = rotatedata;
            BindingOperations.SetBinding(ele, ContentControl.RenderTransformProperty, rotate);

            ////Binding Adornment Symbol Height
            Binding symHeight = new Binding();
            symHeight.Source = series.AdornmentsInfo;
            symHeight.Path = new PropertyPath("SymbolHeight");
            symHeight.Mode = BindingMode.OneWay;
            symHeight.Converter = new SymbolHeightConverter();
            symHeight.ConverterParameter = (cntrl != null && cntrl.DataContext is ChartAdornment) ? (cntrl.DataContext as ChartAdornment).Y : Canvas.GetTop(cntrl); //Canvas.GetTop(cntrl);
            BindingOperations.SetBinding(cntrl, Canvas.TopProperty, symHeight);

            ////Binding Adornment Symbol Width
            Binding symWidth = new Binding();
            symWidth.Source = series.AdornmentsInfo;
            symWidth.Path = new PropertyPath("SymbolWidth");
            symWidth.Mode = BindingMode.OneWay;
            symWidth.Converter = new SymbolWidthConverter();
            symWidth.ConverterParameter = (cntrl != null && cntrl.DataContext is ChartAdornment) ? (cntrl.DataContext as ChartAdornment).X : Canvas.GetLeft(cntrl); //Canvas.GetLeft(cntrl);
            BindingOperations.SetBinding(cntrl, Canvas.LeftProperty, symWidth);

            ////Binding Segment Show Line
            Binding connectVisible = new Binding();
            connectVisible.Source = series.AdornmentsInfo;
            connectVisible.Path = new PropertyPath("SegmentShowLine");
            connectVisible.Mode = BindingMode.OneWay;
            connectVisible.Converter = new ConnectorVisibilityConverter();
            connectVisible.ConverterParameter = series;
            BindingOperations.SetBinding(connector, ContentControl.VisibilityProperty, connectVisible);

            ////Bind the Connector Template
            if (series.AdornmentsInfo.ConnectorTemplate != null)
            {
                List<object> obj = new List<object>();
                obj.Add(connector);
                if (series.AdornmentsInfo.SegmentIsOut == true && series.AdornmentsInfo.SegmentShowLine == true)
                {
                    if ((ui as ContentPresenter).Content is ChartPieAdornment)
                    {
                        obj.Add(((ui as ContentPresenter).Content as ChartPieAdornment).m_angle);
                    }
                    else if ((ui as ContentPresenter).Content is ChartAccumulationAdornment && series.AdornmentsInfo.SegmentHorizontalAlignment == HorizontalAlignment.Left)
                    {
                        obj.Add(180d);
                    }
                    else
                    {
                        obj.Add(0d);
                    }
                }
                else
                {
                    obj.Add(0d);
                }

                Binding connectorRotate = new Binding();
                connectorRotate.Source = series.AdornmentsInfo;
                connectorRotate.Path = new PropertyPath("ConnectorTemplate");
                connectorRotate.Mode = BindingMode.OneWay;
                connectorRotate.Converter = new ConnectorRotateConverter();
                connectorRotate.ConverterParameter = obj;
                BindingOperations.SetBinding(connector, ContentControl.RenderTransformProperty, connectorRotate);
            }
            if ((ui as ContentPresenter).Content is ChartPieAdornment)
            {

                var pieadornmentmode = ChartPieAdornment.GetAdornmentMode(series.AdornmentsInfo);
                if (pieadornmentmode == AdornmentMode.Radial)
                {
                    var stangle = ((ui as ContentPresenter).Content as ChartPieAdornment).m_startangle;
                    var endangle = ((ui as ContentPresenter).Content as ChartPieAdornment).m_endangle;
                    var angle = endangle - ((endangle - stangle) / 2);
                    CompositeTransform transform =(CompositeTransform)ele.RenderTransform;
                    if (transform == null)
                    {
                        //transform = new CompositeTransform(){ CenterX =ele.DesiredSize.Width /2 , CenterY= ele.DesiredSize.Height /2};
                        ele.RenderTransform = transform;
                    }
                    transform.CenterX = 0;
                    transform.CenterY = 0;
                    if (angle <= 90)
                    {
                        transform.Rotation=((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle;
                        //ele.RenderTransform = new CompositeTransform { Rotation = ((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle };
                        cntrl.RenderTransform = new CompositeTransform { Rotation = ((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle };
                    }
                    else if (angle > 90 && angle <= 180)
                    {
                         transform.Rotation=((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle - 180;
                       // ele.RenderTransform = new CompositeTransform { Rotation = ((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle - 180 };
                        cntrl.RenderTransform = new CompositeTransform { Rotation = ((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle - 180 };
                    }
                    else if (angle > 180 && angle <= 270)
                    {
                         transform.Rotation=((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle - 180;
                        //ele.RenderTransform = new CompositeTransform { Rotation = ((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle - 180 };
                        cntrl.RenderTransform = new CompositeTransform { Rotation = ((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle - 180 };
                    }
                    else
                    {
                         transform.Rotation=((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle;
                       // ele.RenderTransform = new CompositeTransform { Rotation = ((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle };
                        cntrl.RenderTransform = new CompositeTransform { Rotation = ((ui as ContentPresenter).Content as ChartPieAdornment).LabelAngle };
                    }
                }
            }
        }

        private void OnSegmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        private static void OnSegmentsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SeriesPresenter host = d as SeriesPresenter;
            SegmentsCollection oldSegments = e.OldValue as SegmentsCollection;
            SegmentsCollection newSegments = e.NewValue as SegmentsCollection;
            if (newSegments != null)
            {
                newSegments.CollectionChanged += new NotifyCollectionChangedEventHandler(host.OnSegmentsCollectionChanged);
            }

            if (oldSegments != null)
            {
                oldSegments.CollectionChanged -= new NotifyCollectionChangedEventHandler(host.OnSegmentsCollectionChanged);
            }

            if (host != null)
            {
                host.Children.Clear();
                host.LoadSegments();
            }
        }

        private static void OnSegmentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SeriesPresenter host = d as SeriesPresenter;
            if (host != null)
            {
                host.Children.Clear();
                if (host.SegmentTemplate != null)
                {
                    ////host.LoadSegments();
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Segments != null)
            {
                this.Segments.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnSegmentsCollectionChanged);
                for (int temp = 0; temp < this.Segments.Count; temp++)
                    this.Segments[temp].Dispose();
                this.Segments.Clear();
                this.Segments = null;
            }
            this.ClearValue(SeriesPresenter.SegmentsProperty);
            if (canvas != null)
            {
                canvas.MouseEnter -= new MouseEventHandler(canvas_MouseEnter);
                canvas.MouseLeave -= new MouseEventHandler(canvas_MouseLeave);
                canvas.Children.Clear();
                canvas = null;
            }
            this.ClearValue(SeriesPresenter.SegmentTemplateProperty);
            this.Children.Clear();
        }

        #endregion
    }
}
