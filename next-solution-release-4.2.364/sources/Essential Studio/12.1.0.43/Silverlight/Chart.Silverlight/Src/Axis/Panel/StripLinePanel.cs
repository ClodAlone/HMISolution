#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Data;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for StripLinePanel
    /// </summary>
    public class StripLinePanel : Panel
    {
        internal bool IsRefreshStriplines = false;
        ChartAxisHeaderPanel panel;
        ContentPresenter presenter;
        /// <summary>
        /// Get or Set Area property
        /// </summary>
        public ChartArea Area
        {
            get
            {
                return (ChartArea)GetValue(AreaProperty);
            }

            set
            {
                SetValue(AreaProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the Area dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaProperty =
DependencyProperty.Register("Area", typeof(ChartArea), typeof(StripLinePanel), new PropertyMetadata(null));
        /// <summary>
        /// Get or Set Axes property
        /// </summary>
        public AxesCollection Axes
        {
            get
            {
                return (AxesCollection)GetValue(AxesProperty);
            }

            set
            {
                SetValue(AxesProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the Axes dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesProperty =
DependencyProperty.Register("Axes", typeof(AxesCollection), typeof(StripLinePanel), new PropertyMetadata(null));

        Grid stripLineGrid = null;

        /// <summary>
        /// Called when instance created for StripLinePanel
        /// </summary>
        public StripLinePanel()
        {
            stripLineGrid = new Grid();
            panel = new ChartAxisHeaderPanel();
            presenter = new ContentPresenter();           
            this.Axes = new AxesCollection();
            this.Axes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Axes_CollectionChanged);
        }

        void Axes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (ChartAxis axis in e.NewItems)
            {
                axis.StripLines.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(StripLines_CollectionChanged);
            }
        }

        void StripLines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateMeasure();
        }

        void StripLineBinding(object source, PropertyPath path, BindingMode mode, DependencyObject target, DependencyProperty targetProperty)
        {
            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = path;
            binding.Mode = mode;
            BindingOperations.SetBinding(target, targetProperty, binding);
        }

        void GenerateStripLines(Size avilableSize)
        {
            this.IsRefreshStriplines = true;
            this.Children.Clear();
            //Grid grid = XamlReader.Load(@"<Grid xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Background='{Binding Interior, Mode=OneWay}' Width='{Binding Width}' Height='{Binding Height}'><ContentPresenter Content='{Binding StriplineContent}' HorizontalAlignment='{Binding HorizontalAlignment}' VerticalAlignment='{Binding VerticalAlignment}' Height='{Binding ContentHeight}' Width='{Binding ContentWidth}' Visibility='{Binding ContentVisibility}' /></Grid>") as Grid;

            foreach (ChartAxis cda in this.Axes)
            {
                if (!(cda.ChartAxesProvider is IChartCartesianAxes))
                {
                    this.Children.Clear();
                    return;
                }

                double SegmentMinValue = cda.Range.Start + (cda.Range.Start * (-1));
                double SegmentMaxValue = cda.Range.End - cda.Range.Start;
                SegmentMaxValue = (SegmentMaxValue == 0) ? 1 : SegmentMaxValue;

                #region StripLines
                if (cda.StripLines != null && cda.StripLines.Count != 0)
                {
                    #region Secondary Axis
                    if (cda.Orientation == Orientation.Vertical)
                    {
                        foreach (ChartStripLine stripLine in cda.StripLines)
                        {
                            if (stripLine.IsSegmented == true)
                            {
                                #region Segmented
                                double startStrip = stripLine.StartFromAxis ? cda.Range.Start + stripLine.Offset : stripLine.Start;
                                double endStrip = stripLine.StartFromAxis ? (stripLine.RepeatUntil == 0 ? cda.Range.End : stripLine.RepeatUntil) : stripLine.RepeatUntil;
                                double periodStrip = stripLine.RepeatEvery;

                                double xaxisSegmentMinValue = this.Area == null ? 0 : this.Area.PrimaryAxis.Range.Start + (this.Area.PrimaryAxis.Range.Start * (-1));
                                double xaxisSegmentMaxValue = this.Area == null ? 1 : this.Area.PrimaryAxis.Range.End - this.Area.PrimaryAxis.Range.Start;
                                xaxisSegmentMaxValue = (xaxisSegmentMaxValue == 0) ? 1 : xaxisSegmentMaxValue;
                                do
                                {
                                    if ((cda.Range.Inside(startStrip) || cda.Range.Inside(endStrip)) &&
                                        (this.Area.PrimaryAxis.Range.Inside(stripLine.SegmentStartValue) && this.Area.PrimaryAxis.Range.Inside(stripLine.SegmentEndValue)))
                                    {

                                        //Grid grid = XamlReader.Load(@"<Grid xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' VerticalAlignment='Top' HorizontalAlignment='Left' Background='{Binding Interior, Mode=OneWay}' Width='{Binding Width}' Height='{Binding Height}'><ContentPresenter Content='{Binding StriplineContent}' HorizontalAlignment='{Binding HorizontalAlignment}' VerticalAlignment='{Binding VerticalAlignment}' Height='{Binding ContentHeight}' Width='{Binding ContentWidth}' Visibility='{Binding ContentVisibility}' /></Grid>") as Grid;

                                        Grid grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
                                        grid.DataContext = stripLine;

                                        //grid.DataContext = stripLine;
                                        //stripLineGrid.Children.Add(grid);

                                        double segmentvalue = startStrip + (cda.Range.Start * (-1));
                                        Line ln = new Line();
                                        double segmentstartvalue = stripLine.SegmentStartValue + (this.Area.PrimaryAxis.Range.Start * (-1));
                                        ln.X1 = (avilableSize.Width / (xaxisSegmentMaxValue - xaxisSegmentMinValue) * segmentstartvalue);// +Axesthickness.Left;
                                        ln.Y1 = (avilableSize.Height * (1 - ((1 / (SegmentMaxValue - SegmentMinValue)) * segmentvalue)))
                                            - (!double.IsNaN(stripLine.Height) ? stripLine.Height / 2 : 0);

                                        double segmentendvalue = stripLine.SegmentEndValue + (this.Area.PrimaryAxis.Range.Start * (-1));
                                        ln.X2 = (avilableSize.Width / (xaxisSegmentMaxValue - xaxisSegmentMinValue) * segmentendvalue);// +Axesthickness.Left;
                                        ln.Y2 = (avilableSize.Height * (1 - ((1 / (SegmentMaxValue - SegmentMinValue)) * segmentvalue)))
                                            - (!double.IsNaN(stripLine.Height) ? stripLine.Height / 2 : 0);


                                        grid.Width = Math.Abs(ln.X2 - ln.X1);
                                        grid.Height = stripLine.Height;
                                        panel = new ChartAxisHeaderPanel();
                                        presenter = new ContentPresenter();

                                        grid.Margin = new Thickness(ln.X1, ln.Y1, 0, 0);
                                        panel.Orientation = stripLine.ContentOrientation;
                                        panel.HorizontalAlignment = HorizontalAlignment.Left;
                                        panel.VerticalAlignment = VerticalAlignment.Top;

                                        panel.DataContext = stripLine;
                                        presenter.HorizontalAlignment = HorizontalAlignment.Left;
                                        presenter.VerticalAlignment = VerticalAlignment.Top;
                                        presenter.Content = stripLine.StriplineContent;
                                        presenter.Width = stripLine.ContentWidth;
                                        presenter.Height = stripLine.ContentHeight;
                                        if (stripLine.ContentOrientation == Orientation.Vertical)
                                            presenter.RenderTransform = new RotateTransform() { Angle = -90 };
                                        if (!panel.Children.Contains(presenter))
                                        panel.Children.Add(presenter);
                                        this.Children.Add(grid);
                                        if (!this.Children.Contains(panel))
                                        this.Children.Add(panel);

                                    }
                                    startStrip = startStrip + periodStrip;
                                } while ((periodStrip != 0) && (startStrip < endStrip));
                                #endregion
                            }
                            else
                            {
                                #region DefaultStripline

                                double startStrip = stripLine.StartFromAxis ? cda.Range.Start + stripLine.Offset : stripLine.Start;
                                double endStrip = stripLine.StartFromAxis ? (stripLine.RepeatUntil == 0 ? cda.Range.End : stripLine.RepeatUntil) : stripLine.RepeatUntil;
                                double periodStrip = stripLine.RepeatEvery;
                                do
                                {
                                    if (cda.Range.Inside(startStrip) || cda.Range.Inside(endStrip))
                                    {
                                        //Grid grid = XamlReader.Load(@"<Grid xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' VerticalAlignment='Top' Background='{Binding Interior, Mode=OneWay}' Width='{Binding Width}' Height='{Binding Height}'><ContentPresenter Content='{Binding StriplineContent}' HorizontalAlignment='{Binding HorizontalAlignment}' VerticalAlignment='{Binding VerticalAlignment}' Height='{Binding ContentHeight}' Width='{Binding ContentWidth}' Visibility='{Binding ContentVisibility}' /></Grid>") as Grid;
                                        //grid.DataContext = stripLine;
                                        //stripLineGrid.Children.Add(grid);
                                        Grid grid = new Grid() { VerticalAlignment = VerticalAlignment.Top };
                                        grid.DataContext = stripLine;
                                        double segmentvalue = startStrip + (cda.Range.Start * (-1));
                                        Point ln = new Point();
                                        ln.X = 0;
                                        ln.Y = (avilableSize.Height * (1 - ((1 / (SegmentMaxValue - SegmentMinValue)) * segmentvalue)));
                                        //- (!double.IsNaN(stripLine.Height) ? stripLine.Height/2 : 0);-- Fix SD9573

                                        grid.Width = double.NaN;
                                        if (stripLine.isPixelWidth == false && cda.Range.Inside((stripLine.StartFromAxis ? cda.Range.Start + stripLine.Offset : stripLine.Start) + stripLine.Height))
                                        {
                                            Rect clienrect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
                                            double height = avilableSize.Height * (((cda.Range.Start  + stripLine.Height) - cda.Range.Start) / cda.Range.Delta);
                                            grid.Height = height;
                                        }
                                        else
                                        {
                                            grid.Height = stripLine.Height;
                                        }                                      
                                        grid.Margin = new Thickness(0, ln.Y - grid.Height, 0, 0);
                                        //ContentPresenter presenter = grid.Children[0] as ContentPresenter;
                                        //if (stripLine.ContentOrientation == Orientation.Vertical)
                                        //    presenter.RenderTransform = new RotateTransform() { Angle = 90 };
                                        //presenter.Margin = new Thickness(stripLine.ContentOffsetX, stripLine.ContentOffsetY, 0, 0);
                                        //presenter.HorizontalAlignment = stripLine.HorizontalAlignment;
                                        //presenter.VerticalAlignment = stripLine.VerticalAlignment;
										panel = new ChartAxisHeaderPanel();
                                        presenter = new ContentPresenter();
                                        
                                        panel.DataContext = stripLine;
                                        panel.Orientation = stripLine.ContentOrientation;
                                        panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                        panel.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                                        
                                        presenter.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                        presenter.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                        presenter.Content = stripLine.StriplineContent;
                                        presenter.Width = stripLine.ContentWidth;
                                        presenter.Height = stripLine.ContentHeight;
                                        if (stripLine.ContentOrientation == Orientation.Vertical)
                                            presenter.RenderTransform = new RotateTransform() { Angle = -90 };
                                        if (!panel.Children.Contains(presenter))
                                        panel.Children.Add(presenter);
                                        this.Children.Add(grid);
                                        if(!this.Children.Contains(panel))
                                            this.Children.Add(panel);

                                    }
                                    startStrip = startStrip + periodStrip;
                                } while ((periodStrip != 0) && (startStrip < endStrip));
                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region PrimaryAxis
                    if (cda.Orientation == Orientation.Horizontal)
                    {
                        foreach (ChartStripLine stripLine in cda.StripLines)
                        {
                            if (stripLine.IsSegmented == true)
                            {
                                #region Segmented
                                double startStrip = stripLine.StartFromAxis ? cda.Range.Start + stripLine.Offset : stripLine.Start;
                                double endStrip = stripLine.StartFromAxis ? (stripLine.RepeatUntil == 0 ? cda.Range.End : stripLine.RepeatUntil) : stripLine.RepeatUntil;
                                double periodStrip = stripLine.RepeatEvery;

                                double yaxisSegmentMinValue = this.Area == null ? 0 : this.Area.SecondaryAxis.Range.Start + (this.Area.SecondaryAxis.Range.Start * (-1));
                                double yaxisSegmentMaxValue = this.Area == null ? 1 : this.Area.SecondaryAxis.Range.End - this.Area.SecondaryAxis.Range.Start;
                                yaxisSegmentMaxValue = (yaxisSegmentMaxValue == 0) ? 1 : yaxisSegmentMaxValue;

                                do
                                {

                                    if ((cda.Range.Inside(startStrip) || cda.Range.Inside(endStrip)))
                                    {
                                        //Grid grid = XamlReader.Load(@"<Grid xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' HorizontalAlignment='Left' VerticalAlignment='Top' Background='{Binding Interior, Mode=OneWay}' Width='{Binding Width}' Height='{Binding Height}'><ContentPresenter Content='{Binding StriplineContent}' HorizontalAlignment='{Binding HorizontalAlignment}' VerticalAlignment='{Binding VerticalAlignment}' Height='{Binding ContentHeight}' Width='{Binding ContentWidth}' Visibility='{Binding ContentVisibility}' /></Grid>") as Grid;
                                        //grid.DataContext = stripLine;
                                        //stripLineGrid.Children.Add(grid);

                                        Grid grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
                                        grid.DataContext = stripLine;
                                        double segmentvalue = startStrip + (cda.Range.Start * (-1));
                                        double segmentstartvalue = stripLine.SegmentStartValue + (cda.Range.Start * (-1));
                                        double segmentendvalue = stripLine.SegmentEndValue + (cda.Range.Start * (-1));

                                        Line ln = new Line();
                                        ln.X1 = (avilableSize.Width / (SegmentMaxValue - SegmentMinValue) * segmentvalue)
                                             - (!double.IsNaN(stripLine.Width) ? stripLine.Width / 2 : 0);

                                        ln.Y1 = (avilableSize.Height * (1 - ((1 / (yaxisSegmentMaxValue - yaxisSegmentMinValue)) * stripLine.SegmentEndValue)));// -Axesthickness.Top;

                                        ln.X2 = (avilableSize.Width / (SegmentMaxValue - SegmentMinValue) * segmentvalue)
                                             - (!double.IsNaN(stripLine.Width) ? stripLine.Width / 2 : 0);

                                        ln.Y2 = (avilableSize.Height * (1 - ((1 / (yaxisSegmentMaxValue - yaxisSegmentMinValue)) * stripLine.SegmentStartValue)));// -Axesthickness.Top;

                                        grid.Height = Math.Abs(ln.Y2 - ln.Y1);
                                        grid.Width = stripLine.Width;

                                        grid.Margin = new Thickness(ln.X1, ln.Y1, 0, 0);
                                        //ContentPresenter presenter = grid.Children[0] as ContentPresenter;
                                        //if (stripLine.ContentOrientation == Orientation.Vertical)
                                        //    presenter.RenderTransform = new RotateTransform() { Angle = 90 };
                                        //presenter.Margin = new Thickness(stripLine.ContentOffsetX, stripLine.ContentOffsetY, 0, 0);
                                        //presenter.HorizontalAlignment = stripLine.HorizontalAlignment;
                                        //presenter.VerticalAlignment = stripLine.VerticalAlignment;
                                       panel = new ChartAxisHeaderPanel();
                                       presenter = new ContentPresenter();

                                       panel.Orientation = stripLine.ContentOrientation;
                                       panel.HorizontalAlignment = HorizontalAlignment.Left;
                                       panel.VerticalAlignment = VerticalAlignment.Top;
                                        panel.DataContext = stripLine;
                                        presenter.HorizontalAlignment = HorizontalAlignment.Left;
                                        presenter.VerticalAlignment = VerticalAlignment.Top;
                                        presenter.Content = stripLine.StriplineContent;
                                        presenter.Width = stripLine.ContentWidth;
                                        presenter.Height = stripLine.ContentHeight;
                                        if (stripLine.ContentOrientation == Orientation.Vertical)
                                            presenter.RenderTransform = new RotateTransform() { Angle = -90 };
                                        if (!panel.Children.Contains(presenter))
                                            panel.Children.Add(presenter);
                                        this.Children.Add(grid);
                                        if (!this.Children.Contains(panel))
                                            this.Children.Add(panel);



                                    }
                                    startStrip = startStrip + periodStrip;
                                } while ((periodStrip != 0) && (startStrip < endStrip));
                                #endregion
                            }
                            else
                            {
                                #region Default


                                double startStrip = stripLine.StartFromAxis ? cda.Range.Start + stripLine.Offset : stripLine.Start;
                                double endStrip = stripLine.StartFromAxis ? (stripLine.RepeatUntil == 0 ? cda.Range.End : stripLine.RepeatUntil) : stripLine.RepeatUntil;
                                double periodStrip = stripLine.RepeatEvery;
                                do
                                {
                                    if (cda.Range.Inside(startStrip) || cda.Range.Inside(endStrip))
                                    {
                                        //Grid grid = XamlReader.Load(@"<Grid xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' HorizontalAlignment='Left' Background='{Binding Interior, Mode=OneWay}' Width='{Binding Width}' Height='{Binding Height}'><ContentPresenter Content='{Binding StriplineContent}' HorizontalAlignment='{Binding HorizontalAlignment}' VerticalAlignment='{Binding VerticalAlignment}' Height='{Binding ContentHeight}' Width='{Binding ContentWidth}' Visibility='{Binding ContentVisibility}' /></Grid>") as Grid;
                                        //grid.DataContext = stripLine;
                                        //stripLineGrid.Children.Add(grid);

                                        Grid grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Left };
                                        grid.DataContext = stripLine;

                                        double segmentvalue = startStrip + (cda.Range.Start * (-1));
                                        Point ln = new Point();
                                        ln.X = (avilableSize.Width / (SegmentMaxValue - SegmentMinValue) * segmentvalue);
                                        // - (!double.IsNaN(stripLine.Width) ? stripLine.Width/2 : 0); -- Fix SD9573
                                        ln.Y = 0;

                                        grid.Height = double.NaN;
                                        if (stripLine.isPixelWidth == false && cda.Range.Inside((stripLine.StartFromAxis ? cda.Range.Start + stripLine.Offset : stripLine.Start) + stripLine.Width))
                                        {
                                             Rect clienrect = new Rect(0, 0, avilableSize.Width, avilableSize.Height);
                                            //Rect clienrect = new Rect(0, 0, this.ActualWidth, this.ActualHeight); - Previous Code -- Fix SD9573
                                            double width = clienrect.Width * ((( cda.Range.Start + stripLine.Width) - cda.Range.Start) / cda.Range.Delta);
                                            grid.Width = width;
                                        }
                                        else
                                        {
                                            grid.Width = stripLine.Width;
                                        }

                                        grid.Margin = new Thickness(ln.X, 0, 0, 0);
                                        //ContentPresenter presenter = grid.Children[0] as ContentPresenter;
                                        //if(stripLine.ContentOrientation==Orientation.Vertical)
                                        //presenter.RenderTransform = new RotateTransform() { Angle = 90 };
                                        //presenter.Margin = new Thickness(stripLine.ContentOffsetX, stripLine.ContentOffsetY, 0, 0);
                                        //presenter.HorizontalAlignment = stripLine.HorizontalAlignment;
                                        //presenter.VerticalAlignment = stripLine.VerticalAlignment;
                                        panel = new ChartAxisHeaderPanel();
                                        presenter = new ContentPresenter();
                                        panel.Orientation = stripLine.ContentOrientation;
                                        panel.HorizontalAlignment = HorizontalAlignment.Left;
                                        panel.VerticalAlignment = VerticalAlignment.Top;
                                        panel.DataContext = stripLine;
                                        presenter.HorizontalAlignment = HorizontalAlignment.Left;
                                        presenter.VerticalAlignment = VerticalAlignment.Top;
                                        presenter.Content = stripLine.StriplineContent;
                                        presenter.Width = stripLine.ContentWidth;
                                        presenter.Height = stripLine.ContentHeight;
                                        if (stripLine.ContentOrientation == Orientation.Vertical)
                                            presenter.RenderTransform = new RotateTransform() { Angle = -90 };
                                        if (!panel.Children.Contains(presenter))
                                            panel.Children.Add(presenter);
                                        this.Children.Add(grid);
                                        if (!this.Children.Contains(panel))
                                            this.Children.Add(panel);
                                    }
                                    startStrip = startStrip + periodStrip;
                                } while ((periodStrip != 0) && (startStrip < endStrip));
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }
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
            base.MeasureOverride(availableSize);

            if ((from chartaxis in this.Axes select chartaxis.StripLines.Count > 0).Contains<bool>(true))
            {
                //if(!IsRefreshStriplines)
                GenerateStripLines(availableSize);
                //GenerateItems(availableSize);

                //stripLineGrid.Measure(availableSize);

                foreach (UIElement element in this.Children)
                {
                    element.Measure(availableSize);
                }
            }

            RectangleGeometry geo = new RectangleGeometry();
            geo.Rect = new Rect(0, 0, availableSize.Width, availableSize.Height);
            this.Clip = geo;
            return availableSize;
        }

        //void GenerateItems(Size availableSize)
        //{
        //    stripLineGrid.Children.Clear();
        //    foreach (ChartAxis cda in this.Axes)
        //    {
        //        foreach (ChartStripLine stripLine in cda.StripLines)
        //        {
        //            stripLine.Axis = cda;
        //            stripLine.Area = this.Area;
        //            //stripLine.Width = double.NaN;
        //            //stripLine.Height = double.NaN;
        //            RectangleGeometry geo = new RectangleGeometry();
        //            geo.Rect = new Rect(0, 0, availableSize.Width, availableSize.Height);
        //            stripLine.Clip = geo;
        //            stripLine.HorizontalAlignment = cda.Orientation == Orientation.Horizontal ? HorizontalAlignment.Left : HorizontalAlignment.Stretch;
        //            stripLine.VerticalAlignment = cda.Orientation == Orientation.Horizontal ? VerticalAlignment.Stretch : VerticalAlignment.Top;
        //            stripLineGrid.Children.Add(stripLine);
        //        }
        //    }

        //}

        static Grid previousGrid = null;

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            previousGrid = null;

            foreach (UIElement element in this.Children)
            {
                Grid grid = element as Grid;
                ChartAxisHeaderPanel panel = element as ChartAxisHeaderPanel;
                if (grid != null)
                {
                    ChartStripLine stripLine = grid.DataContext as ChartStripLine;
                    grid.Background = stripLine.Interior;
                    previousGrid = grid;
                    grid.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }
                else if (panel != null)
                {
                    ChartStripLine stripLine = panel.DataContext as ChartStripLine;
                    panel.Visibility = stripLine.ContentVisibility;
                    double left = 0, top = 0;
                    switch (stripLine.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Center:
                            left = previousGrid.Margin.Left + (previousGrid.ActualWidth / 2) - (panel.DesiredSize.Width / 2);
                            break;
                        case HorizontalAlignment.Left:
                            left = previousGrid.Margin.Left;
                            break;
                        case HorizontalAlignment.Right:
                            left = previousGrid.Margin.Left + previousGrid.ActualWidth - panel.DesiredSize.Width;
                            break;
                        case HorizontalAlignment.Stretch:
                            left = previousGrid.Margin.Left + (previousGrid.ActualWidth / 2) - (panel.DesiredSize.Width / 2);
                            break;
                    }

                    switch (stripLine.VerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            top = previousGrid.Margin.Top + (previousGrid.ActualHeight / 2) - (panel.DesiredSize.Height / 2);
                            break;
                        case VerticalAlignment.Bottom:
                            top = previousGrid.Margin.Top + previousGrid.ActualHeight - previousGrid.DesiredSize.Height;
                            break;
                        case VerticalAlignment.Top:
                            top=previousGrid.Margin.Top;
                            break;
                        case VerticalAlignment.Stretch:
                            top = previousGrid.Margin.Top + (previousGrid.ActualHeight / 2) - (panel.DesiredSize.Height / 2);
                            break;
                    }

                    left += stripLine.ContentOffsetX;
                    top += stripLine.ContentOffsetY;

                    left = left < 0 ? 0 : left;
                    left = left + panel.DesiredSize.Width > finalSize.Width ? left - ((left + panel.DesiredSize.Width) - finalSize.Width) : left;

                    top = top < 0 ? 0 : top;
                    top = top + panel.DesiredSize.Height > finalSize.Height ? top - ((top + panel.DesiredSize.Height) - finalSize.Height) : top;

                    panel.Arrange(new Rect(left, top, panel.DesiredSize.Width, panel.DesiredSize.Height));
                }

            }

            //stripLineGrid.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }
    }
}
