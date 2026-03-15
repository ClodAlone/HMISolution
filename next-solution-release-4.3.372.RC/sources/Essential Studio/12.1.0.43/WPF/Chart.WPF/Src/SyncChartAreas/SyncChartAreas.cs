// <copyright file="ChartArea.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Effects;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Syncfusion.Licensing;
    using System.Linq;
    using Syncfusion.Windows.Shared;
    using System.Xml.Serialization;
    using System.Text;
    using System.Xml;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the Synchronization of more then one ChartArea
    /// </summary>
    /// <seealso cref="ChartArea"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SyncChartAreas : ChartArea, IChartSerializer
    {
        internal bool SyncInteractiveCursorMove = false;
        internal bool Minimum = false;
        //internal InteractiveCursorCollection interactiveCursors = new InteractiveCursorCollection();

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse was left and the handled state.</param>
        /// 
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.SyncInteractiveCursorMove = false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event. 
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            ChartCartesianAxisPanel v = Chart.FindAnchestor<ChartCartesianAxisPanel>((DependencyObject)e.OriginalSource);
            if (v != null && (e.Source is SyncChartAreas))
                e.Handled = true;
        }
        #region Members

        /// <summary>
        /// Intializes the splitter
        /// </summary>
        internal Thumb splitter;
        #endregion

        internal static readonly DependencyProperty CursorSeriesProperty = DependencyProperty.Register("CursorSeries", typeof(ChartSeries), typeof(SyncChartAreas), new PropertyMetadata(null));
        /// <summary>
        /// Gets or Sets value fro the Cursor Visibility
        /// </summary>
        internal ChartSeries CursorSeries
        {
            get { return (ChartSeries)GetValue(CursorSeriesProperty); }
            set { SetValue(CursorSeriesProperty, value); }
        }

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="SyncChartAreas"/> class.
        /// </summary>
        /// <remarks>
        /// Primary and secondary axes are being created automatically.
        /// </remarks>
        static SyncChartAreas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SyncChartAreas), new FrameworkPropertyMetadata(typeof(SyncChartAreas)));

            ItemsControl.ItemsSourceProperty.OverrideMetadata(typeof(SyncChartAreas), new FrameworkPropertyMetadata(null));
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SyncChartAreas"/> class.
        /// </summary>
        /// <remarks>
        /// Primary and secondary axes are being created automatically.
        /// </remarks>
        public SyncChartAreas()
            : base()
        {
            //    this.DefaultStyleKey = typeof(SyncChartAreas);
            Areas = new ChartAreasCollection();
            Areas.CollectionChanged += new NotifyCollectionChangedEventHandler(Areas_CollectionChanged);
            this.IsSyncChartArea = true;
            //ResourceDictionary rd = new ResourceDictionary()
            //{
            //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            //};

            //if (rd != null)
            //{
            //    this.Style = rd["syncChartArea"] as Style;
            //}

            SetValue(SyncAreasPanel.OrientationProperty, Orientation.Vertical);
            this.Loaded += new RoutedEventHandler(SyncChartAreas_Loaded);
            InteractiveCursors.CollectionChanged += new NotifyCollectionChangedEventHandler(SyncInteractiveCursorCollection_CollectionChanged);
            this.SizeChanged += new SizeChangedEventHandler(SyncChartAreas_SizeChanged);
        }

        void SyncChartAreas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SyncChartAreas sarea = sender as SyncChartAreas;
            if (sarea.Areas != null)
            {
                foreach (ChartArea area in sarea.Areas)
                {
                    foreach (ChartSeries ser in area.Series)
                    {
                        if (ser.Indicators != null && ser.Presenter != null && ser.Presenter.m_IndicatorPresenter != null)
                        {
                            ser.Presenter.m_IndicatorPresenter.InvalidateVisual();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (this.Areas != null)
            {
                foreach (ChartArea item in this.Areas)
                {
                    item.Dispose();
                }

                this.Areas.Clear();
                this.Areas = null;
            }
            base.Dispose();
        }

        private void SyncInteractiveCursor_Changed()
        {
            if (this.Areas == null)
                return;

            foreach (ChartArea ca in this.Areas)
            {
                ca.InteractiveCursors.Clear();
                ca.SyncChartArea = this;
            }

            int index = 0;
            foreach (InteractiveCursor syncinteractivecursor in this.InteractiveCursors)
            {
                foreach (ChartArea ca in this.Areas)
                {
                    InteractiveCursor ic = new InteractiveCursor();

                    Binding MoveonmousemoveBinding = new Binding();
                    MoveonmousemoveBinding.Source = syncinteractivecursor;
                    MoveonmousemoveBinding.Path = new PropertyPath("BindWithMouseMoveOnSegment");
                    MoveonmousemoveBinding.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ic, InteractiveCursor.BindWithMouseMoveOnSegmentProperty, MoveonmousemoveBinding);

                    Binding XBinding = new Binding();
                    XBinding.Source = syncinteractivecursor;
                    XBinding.Path = new PropertyPath("OffsetX");
                    XBinding.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ic, InteractiveCursor.OffsetXProperty, XBinding);

                    Binding YBinding = new Binding();
                    YBinding.Source = syncinteractivecursor;
                    YBinding.Path = new PropertyPath("OffsetY");
                    YBinding.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ic, InteractiveCursor.OffsetYProperty, YBinding);

                    Binding XValBinding = new Binding();
                    XValBinding.Source = syncinteractivecursor;
                    XValBinding.Path = new PropertyPath("XValue");
                    XValBinding.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ic, InteractiveCursor.XValueProperty, XValBinding);

                    Binding YValBinding = new Binding();
                    YValBinding.Source = syncinteractivecursor;
                    YValBinding.Path = new PropertyPath("YValue");
                    YValBinding.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ic, InteractiveCursor.YValueProperty, YValBinding);


                    Binding CursorVisibility = new Binding();
                    CursorVisibility.Source = syncinteractivecursor;
                    CursorVisibility.Path = new PropertyPath("CursorVisibility");
                    BindingOperations.SetBinding(ic, InteractiveCursor.CursorVisibilityProperty, CursorVisibility);

                    Binding CursorStrokeThickness = new Binding();
                    CursorStrokeThickness.Source = syncinteractivecursor;
                    CursorStrokeThickness.Path = new PropertyPath("CursorStrokeThickness");
                    BindingOperations.SetBinding(ic, InteractiveCursor.CursorStrokeThicknessProperty, CursorStrokeThickness);

                    Binding VerticalCursorStroke = new Binding();
                    VerticalCursorStroke.Source = syncinteractivecursor;
                    VerticalCursorStroke.Path = new PropertyPath("VerticalCursorStroke");
                    BindingOperations.SetBinding(ic, InteractiveCursor.VerticalCursorStrokeProperty, VerticalCursorStroke);

                    Binding HorizontalCursorStroke = new Binding();
                    HorizontalCursorStroke.Source = syncinteractivecursor;
                    HorizontalCursorStroke.Path = new PropertyPath("HorizontalCursorStroke");
                    BindingOperations.SetBinding(ic, InteractiveCursor.HorizontalCursorStrokeProperty, HorizontalCursorStroke);

                    Binding LabelVisibility = new Binding();
                    LabelVisibility.Source = syncinteractivecursor;
                    LabelVisibility.Path = new PropertyPath("LabelVisibility");
                    BindingOperations.SetBinding(ic, InteractiveCursor.HorizontalLabelVisibilityProperty, LabelVisibility);
                    BindingOperations.SetBinding(ic, InteractiveCursor.VerticalLabelVisibilityProperty, LabelVisibility);

                    Binding HorizontalLabelVisibility = new Binding();
                    HorizontalLabelVisibility.Source = syncinteractivecursor;
                    HorizontalLabelVisibility.Path = new PropertyPath("HorizontalLabelVisibility");
                    BindingOperations.SetBinding(ic, InteractiveCursor.HorizontalLabelVisibilityProperty, HorizontalLabelVisibility);

                    Binding VerticalLabelVisibility = new Binding();
                    VerticalLabelVisibility.Source = syncinteractivecursor;
                    VerticalLabelVisibility.Path = new PropertyPath("VerticalLabelVisibility");
                    BindingOperations.SetBinding(ic, InteractiveCursor.VerticalLabelVisibilityProperty, VerticalLabelVisibility);

                    Binding LabelBackground = new Binding();
                    LabelBackground.Source = syncinteractivecursor;
                    LabelBackground.Path = new PropertyPath("LabelBackground");
                    BindingOperations.SetBinding(ic, InteractiveCursor.LabelBackgroundProperty, LabelBackground);

                    Binding LabelForeground = new Binding();
                    LabelForeground.Source = syncinteractivecursor;
                    LabelForeground.Path = new PropertyPath("LabelForeground");
                    BindingOperations.SetBinding(ic, InteractiveCursor.LabelForegroundProperty, LabelForeground);

                    Binding LabelContent = new Binding();
                    LabelContent.Source = syncinteractivecursor;
                    LabelContent.Path = new PropertyPath("VerticalCursorLabelContent");
                    LabelContent.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ic, InteractiveCursor.VerticalCursorLabelContentProperty, LabelContent);

                    Binding cursor = new Binding();
                    cursor.Source = syncinteractivecursor;
                    cursor.Path = new PropertyPath("Cursor");
                    cursor.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ic, InteractiveCursor.CursorProperty, cursor);

                    Binding BindWithSegment = new Binding();
                    BindWithSegment.Source = syncinteractivecursor;
                    BindWithSegment.Path = new PropertyPath("IsBindWithSegment");
                    BindingOperations.SetBinding(ic, InteractiveCursor.IsBindWithSegmentProperty, BindWithSegment);

                    Binding labelPosition = new Binding();
                    labelPosition.Source = syncinteractivecursor;
                    labelPosition.Path = new PropertyPath("LabelPosition");
                    labelPosition.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ic, InteractiveCursor.LabelPositionProperty, labelPosition);

                    Binding isInversedLabel = new Binding();
                    isInversedLabel.Source = syncinteractivecursor;
                    isInversedLabel.Path = new PropertyPath("IsInversedLabel");
                    BindingOperations.SetBinding(ic, InteractiveCursor.IsInversedLabelProperty, isInversedLabel);


                    Binding enableHorizontalMove = new Binding();
                    enableHorizontalMove.Source = syncinteractivecursor;
                    enableHorizontalMove.Path = new PropertyPath("EnableHorizontalMove");
                    BindingOperations.SetBinding(ic, InteractiveCursor.EnableHorizontalMoveProperty, enableHorizontalMove);

                    Binding enableVerticalMove = new Binding();
                    enableVerticalMove.Source = syncinteractivecursor;
                    enableVerticalMove.Path = new PropertyPath("EnableVerticalMove");
                    BindingOperations.SetBinding(ic, InteractiveCursor.EnableVerticalMoveProperty, enableVerticalMove);

                    Binding Tooltip = new Binding();
                    Tooltip.Source = syncinteractivecursor;
                    Tooltip.Path = new PropertyPath("ToolTip");
                    BindingOperations.SetBinding(ic, InteractiveCursor.ToolTipProperty, Tooltip);

                    ic.CollectionIndex = index;
                    if (this.Areas.IndexOf(ca) != 0)
                    {
                        ic.Y1 = 17;
                        ic.HorizontalCursorVisibility = Visibility.Collapsed;
                    }
                    //ic.VerticalLabelVisibility = Visibility.Collapsed;
                    //if (this.Areas.IndexOf(ca) == this.Areas.Count - 1)
                    //    ic.VerticalLabelVisibility = Visibility.Visible;
                    //  SetInteractiveCursorProperties(syncinteractivecursor, ic);
                    FieldInfo finfo = syncinteractivecursor.GetType().GetField("LocationChanged", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
                    Delegate eventDelegate = finfo != null ? (Delegate)finfo.GetValue(syncinteractivecursor) : null;
                    if (eventDelegate != null)
                    {
                        var eventHandler = (QtpInteracitveCursorLocationChangedEventHandler)eventDelegate.GetInvocationList().First();
                        ic.AddlocationHandler(eventHandler);
                    }
                    syncinteractivecursor.interactivecursor.Add(ic);
                    ca.InteractiveCursors.Add(ic);
                }
                index++;
            }
        }

        /// <summary>
        /// Invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        /// <seealso cref="SyncChartAreas"/>
        public override void OnApplyTemplate()
        {
            //ItemsControl sa = GetTemplateChild("Part_Presenter") as ItemsControl;

            base.OnApplyTemplate();


        }

        private InteractiveCursor SetInteractiveCursorProperties(InteractiveCursor parent, InteractiveCursor child)
        {
            if (parent != null && child != null)
            {
                child.HorizontalLabelVisibility = parent.HorizontalLabelVisibility;
                child.VerticalLabelVisibility = parent.VerticalLabelVisibility;
                child.HorizontalCursorVisibility = parent.HorizontalCursorVisibility;
                child.VerticalCursorVisibility = parent.VerticalCursorVisibility;
            }

            return child;
        }


        internal bool isCustomPanel = false;
        private static void OnAreasPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            SyncChartAreas area = d as SyncChartAreas;
            if (area != null)
            {
                area.isCustomPanel = true;
            }
        }

        void SyncInteractiveCursorCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SyncInteractiveCursor_Changed();
        }

        void SyncChartAreas_Loaded(object sender, RoutedEventArgs e)
        {
            if (tempalteRD == null)
            {
                tempalteRD = ChartDictionaries.GenericDictionary;
                //tempalteRD = new SharedResourceDictionary()
                //{
                //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                //};
            }

            if (tempalteRD != null)
            {
                this.Style = tempalteRD["syncChartArea"] as Style;
            }

            SyncInteractiveCursor_Changed();

            //Binding Properties for Splitter
        }


        internal void SetBindings(ChartArea chartArea)
        {
            //foreach (ChartArea chartArea in syncChartArea.Areas)
            //{
            Canvas canvas = GetSplitterCanvas(chartArea);
            if (chartArea.m_areaPresenter != null)
            {
               chartArea.m_areaPresenter.m_splitter = canvas;
                if (chartArea.m_areaPresenter.m_splitter != null)
                {
                   if ((chartArea.m_areaPresenter.m_splitter.Children[0] as Thumb).Margin != new Thickness(0))
                    {
                      (VisualTreeHelper.GetParent(chartArea.m_areaPresenter) as ChartDockPanel).Children.Add(chartArea.m_areaPresenter.m_splitter);
                    }
                    Chart.SetDock(chartArea.m_areaPresenter.m_splitter, ChartDock.Bottom);
                    splitter = chartArea.m_areaPresenter.m_splitter.Children[0] as Thumb;

                    Binding SplitterLength = new Binding();
                    SplitterLength.Source = chartArea.m_areaPresenter.m_seriesContainer;
                    SplitterLength.Path = new PropertyPath("ActualWidth");
                    BindingOperations.SetBinding(splitter, Thumb.WidthProperty, SplitterLength);

                    Binding SplitterColor = new Binding();
                    SplitterColor.Source = this;
                    SplitterColor.Path = new PropertyPath("SplitterColor");
                    BindingOperations.SetBinding(splitter, Thumb.BackgroundProperty, SplitterColor);


                    SplitterColor = new Binding();
                    SplitterColor.Source = this;
                    SplitterColor.Path = new PropertyPath("SplitterStroke");
                    BindingOperations.SetBinding(splitter, Thumb.BorderBrushProperty, SplitterColor);

                    SplitterColor = new Binding();
                    SplitterColor.Source = this;
                    SplitterColor.Path = new PropertyPath("SplitterWidth");
                    BindingOperations.SetBinding(splitter, Thumb.HeightProperty, SplitterColor);
                    SyncChartAreas sArea = chartArea.ChartAreaParent as SyncChartAreas;
                    if (sArea == null)
                        return;
                    if (chartArea.index == sArea.Areas.Count - 1)
                    {
                        splitter.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        switch (this.SplitterVisiblity)
                        {
                            case SpliterVisibility.Hide:
                                splitter.Visibility = Visibility.Collapsed;
                                break;
                            case SpliterVisibility.ShowAlways:
                                splitter.Visibility = Visibility.Visible;
                                splitter.Opacity = 1;
                                break;
                            case SpliterVisibility.ShowOnMouseHover:
                                splitter.Visibility = Visibility.Visible;
                                splitter.Opacity = 0;
                                break;
                            default:
                                break;
                        }
                    }
                    splitter.DragDelta += new DragDeltaEventHandler(splitter_DragDelta);
                    splitter.MouseEnter += new MouseEventHandler(splitter_MouseEnter);
                    splitter.MouseLeave += new MouseEventHandler(splitter_MouseLeave);

                }
            }
            //}
        }

        private static Canvas GetSplitterCanvas(ChartArea chartArea)
        {
            Canvas canvas = new Canvas();    
            canvas.VerticalAlignment = VerticalAlignment.Stretch;
            canvas.Margin = new Thickness(0, 0, 0, 2);
            Thumb thumb = new Thumb();
            thumb.Height = 2.0;
            thumb.BorderThickness = new Thickness(1);
            thumb.Margin = chartArea.AxesThickness;
            thumb.DataContext = chartArea;
            canvas.Children.Add(thumb);
            return canvas;
        }
        internal bool splitterFlag = false;
        void splitter_MouseLeave(object sender, MouseEventArgs e)
        {
            splitterFlag = false;
            if (SplitterVisiblity == SpliterVisibility.Hide)
                return;
            FrameworkElement Element = sender as FrameworkElement;
            if (Element != null)
            {
                ChartArea chartArea = Element.DataContext as ChartArea;
                if (chartArea != null && chartArea.m_areaPresenter != null && chartArea.m_areaPresenter.m_splitter != null)
                {
                    splitter = chartArea.m_areaPresenter.m_splitter.Children[0] as Thumb;
                    switch (SplitterVisiblity)
                    {
                        case SpliterVisibility.Hide:
                            splitter.Visibility = Visibility.Collapsed;
                            break;
                        case SpliterVisibility.ShowAlways:
                            splitter.Visibility = Visibility.Visible;
                            break;
                        case SpliterVisibility.ShowOnMouseHover:
                            splitter.Visibility = Visibility.Visible;
                            splitter.Opacity = 0;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        void splitter_MouseEnter(object sender, MouseEventArgs e)
        {
            splitterFlag = true;
            if (SplitterVisiblity == SpliterVisibility.Hide)
                return;
            FrameworkElement Element = sender as FrameworkElement;
            if (Element != null)
            {
                ChartArea chartArea = Element.DataContext as ChartArea;
                if (chartArea != null && chartArea.m_areaPresenter != null && chartArea.m_areaPresenter.m_splitter != null)
                {
                    splitter = chartArea.m_areaPresenter.m_splitter.Children[0] as Thumb;
                    splitter.Cursor = Cursors.SizeNS;
                    switch (SplitterVisiblity)
                    {
                        case SpliterVisibility.Hide:
                            splitter.Visibility = Visibility.Collapsed;
                            break;
                        case SpliterVisibility.ShowAlways:
                            splitter.Visibility = Visibility.Visible;
                            break;
                        case SpliterVisibility.ShowOnMouseHover:
                            splitter.Visibility = Visibility.Visible;
                            splitter.Opacity = 1;
                            break;
                        default:
                            break;
                    }
                }
                e.Handled = true;
            }
        }

        void splitter_DragDelta(object sender, DragDeltaEventArgs e)
        {

            FrameworkElement Element = sender as FrameworkElement;
            ChartArea chartArea = Element.DataContext as ChartArea;

            #region new splitter
            if (e.VerticalChange != 0.0)
            {
                isSpliterDragged = true;
                SyncChartAreas SyncArea = chartArea.ChartAreaParent;
                DependencyObject obj = VisualTreeHelper.GetParent(chartArea);
                if (obj is SyncAreasPanel)
                {
                    (obj as SyncAreasPanel)._splitterStartFlag = false;
                }
                int index = (from a in SyncArea.Areas where (a.m_areaPresenter.m_splitter.Children[0] as Thumb).Equals(sender as Thumb) select SyncArea.Areas.IndexOf(a)).First<int>();
                if (index >= 0 && index != SyncArea.Areas.Count - 1)
                {
                    double totalHeight = SyncArea.Areas[index].Height + SyncArea.Areas[index + 1].Height;
                    if (Math.Round(totalHeight, 0) > Math.Round(SyncArea.Areas[0].TotalSize.Height, 0))
                    {
                        totalHeight = SyncArea.Areas[0].TotalSize.Height;
                    }

                    if (SyncArea.Areas[index].Height < 0 || SyncArea.Areas[index + 1].Height < 0)
                    {
                        return;
                    }
                    SyncArea.Areas[index + 1].BottomAreaSplitterFlag = false;
                    SyncArea.Areas[index + 1].TopAreaSplitterFlag = false;

                    double newheight = SyncArea.Areas[index + 1].Height - Math.Floor(e.VerticalChange);
                    double newheightPrevious = SyncArea.Areas[index].Height + Math.Floor(e.VerticalChange);
                    newheight = newheight <= 0 ? 0 : newheight;
                    newheightPrevious = newheightPrevious <= 0 ? 0 : newheightPrevious;

                    if (index == 0)
                    {
                        if (SyncArea.Areas[index].Legend != null)
                        {
                            SyncArea.Areas[index].ActualMinHeight = SyncArea.Areas[index].MinHeight + 11 + SyncArea.Areas[index].Legend.ActualHeight + SyncArea.Areas[index].Legend.Margin.Top + SyncArea.Areas[index].Legend.Margin.Bottom + 0.5 + SyncArea.Areas[index].Padding.Bottom;
                        }
                        else
                        {
                            if ((SyncArea.Parent as Chart).Legends.Count > 0)
                            {
                                SyncArea.Areas[index].ActualMinHeight = SyncArea.Areas[index].MinHeight + 11 + SyncArea.Areas[index].Padding.Bottom + (SyncArea.Parent as Chart).Legends[(SyncArea.Parent as Chart).Legends.Count - 1].ActualHeight + 0.5;
                            }
                            else
                            {
                                SyncArea.Areas[index].ActualMinHeight = SyncArea.Areas[index].MinHeight + 11 + SyncArea.Areas[index].Padding.Bottom + 0.5;
                            }
                        }
                    }

                    if (index + 1 == SyncArea.Areas.Count - 1)
                    {
                        if (SyncArea.Areas[index + 1].Legend != null)
                        {
                            SyncArea.Areas[index + 1].ActualMinHeight = SyncArea.Areas[index + 1].BottomSpace + SyncArea.Areas[index + 1].MinHeight + 11 + SyncArea.Areas[index + 1].Legend.ActualHeight + SyncArea.Areas[index + 1].Legend.Margin.Top + SyncArea.Areas[index + 1].Legend.Margin.Bottom + SyncArea.Areas[index + 1].Padding.Top;
                        }
                        else
                        {
                            SyncArea.Areas[index + 1].ActualMinHeight = SyncArea.Areas[index + 1].BottomSpace + SyncArea.Areas[index + 1].MinHeight + 11 + chartArea.AreaPaddingBottom;
                        }
                    }
                    else
                    {
                        SyncArea.Areas[index + 1].ActualMinHeight = SyncArea.Areas[index + 1].MinHeight;
                    }

                    if (newheight < (SyncArea.Areas[index + 1].ActualMinHeight + 10))
                    {
                        newheight = SyncArea.Areas[index + 1].ActualMinHeight;
                        newheightPrevious = totalHeight - newheight;
                        if (index + 1 == 1)
                        {
                            SyncArea.Areas[index + 1].BottomAreaSplitterFlag = true;
                            SyncArea.Areas[index + 1].TopAreaSplitterFlag = true;
                        }
                    }

                    if (newheightPrevious < SyncArea.Areas[index].ActualMinHeight)
                    {
                        SyncArea.Areas[index + 1].TopAreaSplitterFlag = true;
                        newheightPrevious = SyncArea.Areas[index].ActualMinHeight;
                        newheight = totalHeight - newheightPrevious;
                    }

                    ////if (Math.Round(totalHeight, 0) > Math.Round(SyncArea.Areas[0].TotalSize.Height, 0))
                    ////{
                    ////    return;
                    ////}

                    ////if (totalHeight == (newheight + newheightPrevious)   && Math.Round(totalHeight, 0) <= Math.Round(SyncArea.Areas[0].TotalSize.Height, 0))
                    ////{
                    SyncArea.Areas[index + 1].SplitRatio = Math.Round(newheight, 0) / SyncArea.Areas[index + 1].TotalSize.Height;
                    SyncArea.Areas[index].SplitRatio = Math.Round(newheightPrevious, 0) / SyncArea.Areas[index].TotalSize.Height;

                    SyncArea.Areas[index + 1].Height = newheight;// Math.Round(newheight, 0);
                    SyncArea.Areas[index].Height = newheightPrevious;// Math.Round(newheightPrevious, 0);
                    for (int i = 0; i < SyncArea.Areas.Count;i++ )
                    {
                        SyncArea.Areas[i].IsSplitterDrag = true;
                    }
                    UpdateSpliterFlag = false;
                    ////}

                }
            }
            e.Handled = true;

            #endregion
        }

        internal bool UpdateSpliterFlag = false;

        void m_areas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetAreaProperties();
        }

        private void Series_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ChartSeries series in e.NewItems)
                {
                    series.IsIndexed = false;
                    Syncfusion.Windows.Chart.Chart parent = base.Parent as Syncfusion.Windows.Chart.Chart;

                    if (series.YAxis != null)
                    {
                        if (series.YAxis.Area != null)
                        {
                            if (series.YAxis.Area.index != 0)
                            {
                                // series.YAxis.EdgeLabelsDrawingMode = EdgeLabelsDrawingMode.Shift;
                            }
                        }
                    }
                    if (parent != null)
                    {
                        if (parent.m_internalSeriesList != null && series.DataSource != null)
                        {
                            Binding binding = new Binding();
                            binding.Path = new PropertyPath("DataContext");
                            binding.Source = series.Area;
                            BindingOperations.SetBinding(series, ChartArea.DataContextProperty, binding);
                            //parent.m_internalSeriesList.Clear();
                            parent.m_internalSeriesList.Add(series);
                            if (parent.Legends.Count > 0)
                                parent.Legends[0].ItemsSource = parent.m_internalSeriesList;
                        }
                    }
                }
            }
            if (e.OldItems != null)
            {
                Syncfusion.Windows.Chart.Chart parent = base.Parent as Syncfusion.Windows.Chart.Chart;
                parent.m_internalSeriesList.Clear();
            }

        }

        #endregion

        #region Members


        internal ChartSeries interactiveSeries = null;

        internal ChartArea interactiveArea = null;

        private ChartAreasCollection m_areas = new ChartAreasCollection();
        private ChartArea area;
        private ChartAxis Axis;
        #endregion

        #region Dependancy Properties

        /// <summary>
        /// Identifies the Panning range, It is a dependencyProperty
        /// </summary>
        public static readonly DependencyProperty PanningRange_SyncProperty =
         DependencyProperty.RegisterAttached("PanningRange_Sync", typeof(DoubleRange), typeof(SyncChartAreas), new UIPropertyMetadata(DoubleRange.Empty));

        /// <summary>
        /// Identifies the Panning is set or not, It is a dependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsPanning_SyncProperty =
          DependencyProperty.RegisterAttached("IsPanning_Sync", typeof(bool), typeof(SyncChartAreas), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the Margin, It is a dependencyProperty
        /// </summary>
        public new static readonly DependencyProperty MarginProperty =
 DependencyProperty.Register("Margin", typeof(Thickness), typeof(SyncChartAreas), new UIPropertyMetadata(new Thickness()));

        /// <summary>
        /// Identifies the Area, It is a dependencyProperty
        /// </summary>
        public static readonly DependencyProperty AreasProperty =
   DependencyProperty.Register("Areas", typeof(ChartAreasCollection), typeof(SyncChartAreas), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the AreasPanel, It is a dependencyProperty
        /// </summary>
        public static readonly DependencyProperty AreasPanelProperty =
          DependencyProperty.Register("AreasPanel", typeof(ItemsPanelTemplate), typeof(SyncChartAreas), new UIPropertyMetadata(new ItemsPanelTemplate(new FrameworkElementFactory(typeof(SyncAreasPanel))), OnAreasPanelChanged));

        /// <summary>
        /// Identifies the SyncChartArea, It is a dependencyProperty
        /// </summary>
        internal static readonly DependencyProperty IsSyncChartAreaProperty =
            DependencyProperty.Register("IsSyncChartArea", typeof(bool), typeof(SyncChartAreas), new PropertyMetadata(false));

        //public static readonly DependencyProperty SplitterColorProperty =
        //    DependencyProperty.Register("SplitterColor", typeof(Brush), typeof(SyncChartAreas), new PropertyMetadata(Brushes.Gray));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the panning range_ sync.
        /// </summary>
        /// <value>The panning range_ sync.</value>
        public DoubleRange PanningRange_Sync
        {
            get { return (DoubleRange)GetValue(PanningRange_SyncProperty); }
            set { SetValue(PanningRange_SyncProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is panning_ sync.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is panning_ sync; otherwise, <c>false</c>.
        /// </value>
        public bool IsPanning_Sync
        {
            get { return (bool)GetValue(IsPanning_SyncProperty); }
            set { SetValue(IsPanning_SyncProperty, value); }
        }
        /// <summary>
        /// Gets or sets the areas.
        /// </summary>
        /// <value>The areas.</value>
        public ChartAreasCollection Areas
        {
            get { return (ChartAreasCollection)GetValue(AreasProperty); }
            set { SetValue(AreasProperty, value); }
        }


        /// <summary>
        /// Gets or sets the outer margin of an element.  This is a dependency property.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// Provides margin values for the element. The default value is a <see cref="T:System.Windows.Thickness"/> with all properties equal to 0 (zero).
        /// </returns>
        public new Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }


        /// <summary>
        /// Gets or sets the areas panel.
        /// </summary>
        /// <value>The areas panel.</value>
        public ItemsPanelTemplate AreasPanel
        {
            get
            {
                return (ItemsPanelTemplate)GetValue(AreasPanelProperty);
            }

            set
            {
                SetValue(AreasPanelProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is sync chart area.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is sync chart area; otherwise, <c>false</c>.
        /// </value>
        internal bool IsSyncChartArea
        {
            get
            {
                return (bool)GetValue(IsSyncChartAreaProperty);
            }
            set
            {
                SetValue(IsSyncChartAreaProperty, value);
            }
        }
        #endregion

        #region Events


        internal void SetEnableZoomingForYAxis(ChartArea area)
        {
            if (area != null)
            {
                var axes = (from axis in area.Axes
                            where axis.Orientation == Orientation.Vertical
                            select axis).ToList<ChartAxis>();

                foreach (var item in axes)
                {
                    item.EnableZooming = false;
                }
            }
        }

        void Areas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChartAreasCollection areas = sender as ChartAreasCollection;
            if (areas != null && e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<ChartArea>())
                {
                    item.hasMinWidth = false;
                    item.hasMinWidthInLeft = false;
                    item.hasMinWidthInRight = false;
                    if (item.Axes.Count == 2)
                    {
                        this.SetAreaProperties();
                    }
                    item.AreaPresenterEvent += new AreaPresenterEventHandler(item_AreaPresenterEvent);
                    item.Series.CollectionChanged += new NotifyCollectionChangedEventHandler(Series_CollectionChanged);
                }
            }
            else if (areas != null)
            {
                this.SetAreaProperties();
            }

        }

        void item_AreaPresenterEvent(object sender)
        {
            ChartArea area = sender as ChartArea;
            if (area != null)
            {
                SetBindings(area);
            }
        }

        #endregion

        #region Methods


        /// <summary>
        /// Sets the area properties.
        /// </summary>
        internal void SetAreaProperties()
        {

            if (this.Areas == null)
                return;

            if (this.Areas != null)
            {
                //this.interactiveCursors.Clear();

                foreach (ChartArea area in this.Areas)
                {
                    area.BorderBrush = Brushes.Transparent;
                    area.Padding = new Thickness(0, 0, 0, 0);
                    area.IsSync = true;
                    area.index = this.Areas.IndexOf(area);
                    area.PrimaryChartArea = Areas[0];


                    Clone(area.PrimaryAxis, this.PrimaryAxis);
                    area.updated = true;
                    //This condition is to show the primary axis when only one area is added to syncChart.
                    if (this.Areas.Count == 1)
                    {
                        area.PrimaryAxis.AxisVisibility = Visibility.Visible;
                    }
                    else if (this.Areas[this.Areas.Count - 1] != area)
                    {
                        area.PrimaryAxis.AxisVisibility = Visibility.Collapsed;
                        if (area.m_areaPresenter != null && VisualTreeHelper.GetChildrenCount(area.m_areaPresenter) > 3)
                        {
                            DependencyObject obj = VisualTreeHelper.GetChild(area.m_areaPresenter, 0);
                            obj = VisualTreeHelper.GetChild(obj, 1);
                            obj = VisualTreeHelper.GetChild(obj, 3);
                            if (obj is ChartZoomingScrollBar)
                            {
                                (obj as ChartZoomingScrollBar).Visibility = System.Windows.Visibility.Collapsed;
                                (obj as ChartZoomingScrollBar).Height = 0d;
                            }
                        }
                    }
                    area.IsContextMenuEnabled = false;
                    area.ChartAreaParent = this;

                    area.IsContextMenuEnabled = this.IsContextMenuEnabled;
                    area.updateArea = this.updateArea;
                    area.EnableLazyLoading = this.EnableLazyLoading;
                    area.IsRetainAxisPosition = this.IsRetainAxisPosition;

                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("DataContext");
                    binding.Source = this;
                    BindingOperations.SetBinding(area, ChartArea.DataContextProperty, binding);

                }
            }

        }

        internal void SetPrimaryAxisRange()
        {

            var startRange = (from item in this.Areas
                              where item.PrimaryAxis != null && item != null
                              orderby item.PrimaryAxis.m_visibleRange.Start ascending
                              select item.PrimaryAxis.m_visibleRange.Start).ToList<double>().Min();


            var endRange = (from item in this.Areas
                            where item.PrimaryAxis != null && item != null && item.PrimaryAxis.m_visibleRange.End != 1.0
                            orderby item.PrimaryAxis.m_visibleRange.End descending
                            select item.PrimaryAxis.m_visibleRange.End).ToList<double>();


            if (endRange.Count > 0)
            {
                var range = new DoubleRange(startRange, endRange[0]);
                this.PrimaryAxis.m_visibleRange = range;
            }

        }


        /// <summary>
        /// Clones the specified axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="parentAxis">The parent axis.</param>
        /// <returns></returns>
        public new object Clone(ChartAxis axis, ChartAxis parentAxis)
        {
            if (area != null)
            {
                area.Dispose();
            }
            area = new ChartArea();
            area.Clone(axis, parentAxis);
            if (Axis != null)
            {
                Axis.Dispose();
            }
            Axis = new ChartAxis();
            Axis.Clone(axis, parentAxis);
            axis.AllowDrop = parentAxis.AllowDrop;
            axis.AutoScrollingDelta = parentAxis.AutoScrollingDelta;
            axis.AxisVisibility = parentAxis.AxisVisibility;
            axis.ContentPath = parentAxis.ContentPath;
            axis.DataContext = parentAxis.DataContext;
            axis.DateTimeInterval = parentAxis.DateTimeInterval;
            axis.DateTimeRange = parentAxis.DateTimeRange;
            axis.DesiredIntervalsCount = parentAxis.DesiredIntervalsCount;
            axis.EdgeLabelsDrawingMode = parentAxis.EdgeLabelsDrawingMode;
            axis.EnableZooming = parentAxis.EnableZooming;
            axis.EnableAutoScrolling = parentAxis.EnableAutoScrolling;
            Binding EnableAutoIntervalOnZoomingBinding = new Binding("EnableAutoIntervalOnZooming") { Source = parentAxis };
            BindingOperations.SetBinding(axis, ChartAxis.EnableAutoIntervalOnZoomingProperty, EnableAutoIntervalOnZoomingBinding);           
            axis.Focusable = parentAxis.Focusable;
            axis.Header = parentAxis.Header;
            axis.HeaderPosition = parentAxis.HeaderPosition;
            axis.HeaderAlignment = parentAxis.HeaderAlignment;
            axis.HidePartialLabel = parentAxis.HidePartialLabel;
            axis.IgnoreRangePaddingsOnZoom = parentAxis.IgnoreRangePaddingsOnZoom;
            axis.IntersectAction = parentAxis.IntersectAction;
            axis.Interval = parentAxis.Interval;
            axis.BaseInterval = parentAxis.BaseInterval;
            axis.IntervalOffset = parentAxis.IntervalOffset;
            axis.IsAutoSetRange = parentAxis.IsAutoSetRange;
            axis.IsInversed = parentAxis.IsInversed;
           
            //axis.IsLogarithmic = parentAxis.IsLogarithmic;
            axis.IsLogarithmicLabels = parentAxis.IsLogarithmicLabels;
            axis.AxisLabelsPosition = parentAxis.AxisLabelsPosition;
            axis.TickLinesPosition = parentAxis.TickLinesPosition;
            axis.LabelPosition = parentAxis.LabelPosition;
            axis.LabelHorizontalAlignment = parentAxis.LabelHorizontalAlignment;
            axis.LabelVerticalAlignment = parentAxis.LabelVerticalAlignment;
            axis.LabelBackground = parentAxis.LabelBackground;
            axis.LabelBorderBrush = parentAxis.LabelBorderBrush;
            axis.LabelBorderThickness = parentAxis.LabelBorderThickness;
            axis.LabelCornerRadius = parentAxis.LabelCornerRadius;
            axis.LabelDateTimeFormat = parentAxis.LabelDateTimeFormat;
            axis.LabelFontFamily = parentAxis.LabelFontFamily;
            axis.LabelFontSize = parentAxis.LabelFontSize;
            axis.LabelFontWeight = parentAxis.LabelFontWeight;
            axis.LabelForeground = parentAxis.LabelForeground;
            axis.LabelFormat = parentAxis.LabelFormat;
            axis.LabelRotateAngle = parentAxis.LabelRotateAngle;
            if(parentAxis.LabelsPrefix!=null) axis.LabelsPrefix = parentAxis.LabelsPrefix;
            if (parentAxis.LabelsPostfix != null) axis.LabelsPostfix = parentAxis.LabelsPostfix;
            axis.LabelsMode = parentAxis.LabelsMode;
            axis.LabelsSource = parentAxis.LabelsSource;
            if (parentAxis.LabelTemplate != null) axis.LabelTemplate = parentAxis.LabelTemplate;
            axis.Language = parentAxis.Language;
            axis.LineStroke = parentAxis.LineStroke;
            axis.LogarithmicBase = parentAxis.LogarithmicBase;
            //axis.m_visibleInterval = parentAxis.m_visibleInterval;
            //axis.m_visibleLables = parentAxis.m_visibleLables;
            // axis.m_visibleRange = parentAxis.m_visibleRange;
            axis.ValueType = parentAxis.ValueType;
            axis.Range = parentAxis.Range;
            axis.InternalRange = parentAxis.InternalRange;
            axis.ZoomFactor = parentAxis.ZoomFactor;//--Fix SD9765
            axis.RangeCalculationMode = parentAxis.RangeCalculationMode;
            axis.RangePadding = parentAxis.RangePadding;
            axis.SmallTickSize = parentAxis.SmallTickSize;
            axis.SmallTicksPerInterval = parentAxis.SmallTicksPerInterval;
            axis.TickLineStroke = parentAxis.TickLineStroke;
            axis.SmallTickLineStroke = parentAxis.SmallTickLineStroke;
            axis.TickSize = parentAxis.TickSize;
            axis.SetValue(ChartArea.ShowGridLinesProperty, parentAxis.GetValue(ChartArea.ShowGridLinesProperty));
            axis.SetValue(ChartArea.GridLineStrokeProperty, parentAxis.GetValue(ChartArea.GridLineStrokeProperty));
            axis.isNeedUpdate = parentAxis.isNeedUpdate;
            axis.InteractiveCursorTemplate = parentAxis.InteractiveCursorTemplate;
            axis.InteractiveCursorContentVisibility = parentAxis.InteractiveCursorContentVisibility;
            axis.LabelTimeSpanFormat = parentAxis.LabelTimeSpanFormat;
            Binding timespanbinding = new Binding();
            timespanbinding.Source = parentAxis;
            timespanbinding.Path = new PropertyPath("LabelTimeSpanFormat");
            timespanbinding.Mode = BindingMode.OneWay;
            axis.SetBinding(ChartAxis.LabelTimeSpanFormatProperty, timespanbinding);
            return axis;

        }

        #endregion



        #region IChartSerializer Members

        /// <summary>
        /// Initialize the Serialize string
        /// </summary>
        /// <returns></returns>
        public new string Serialize()
        {
            //string _xamlString;
            EditorHelper.Register<BindingExpression, BindingConvertor>();
            PrimaryAxis = (ChartAxis)GetValue(PrimaryAxisProperty);
            StringBuilder outstr = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(outstr, settings));
            //this string need for turning on expression saving mode 
            dsm.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(this, dsm);
            //_xamlString = outstr.ToString();
            if (this.Areas != null)
            {
                StringBuilder _AreasString = new StringBuilder("<" + this.GetType().Name + ".Areas>");
                foreach (ChartArea _area in this.Areas)
                {
                    _AreasString.Append(_area.Serialize());
                }
                _AreasString.Append("</" + this.GetType().Name + ".Areas>");
                outstr = outstr.Replace(Chart.SubString(outstr.ToString(), "<" + this.GetType().Name + ".Areas>", "</" + this.GetType().Name + ".Areas>"), _AreasString.ToString());
           
                  }
            outstr = outstr.Replace(Chart.SubString(outstr.ToString(), "<" + this.GetType().Name + ".Style>", "</" + this.GetType().Name + ".Style>"), String.Empty);

            return outstr.ToString();
        }

        /// <summary>
        /// Initialize Deserialize new object
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public new object Deserialize(string xamlString)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    /// <summary>
    /// Class represents Panel for SyncChartAreas
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SyncAreasPanel : StackPanel
    {
        internal double OldLegendValue = 0d;
        internal bool SplitterPositionFlag = false;

        /// <summary>
        /// Arranges the content of a <see cref="T:System.Windows.Controls.StackPanel"/> element.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the arranged size of this <see cref="T:System.Windows.Controls.StackPanel"/> element and its child elements.
        /// </returns>
        /// <param name="arrangeSize">The <see cref="T:System.Windows.Size"/> that this element should use to arrange its child elements.</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                return base.ArrangeOverride(arrangeSize);
            }

            double startY = 0d;
            double actualheight = 0d;
            foreach (Control control in this.Children)
            {
                ChartArea area = control as ChartArea;
                if (area != null)
                {
                    actualheight = area.Height > 0 && !double.IsNaN(area.Height) && !double.IsPositiveInfinity(area.Height) ? area.Height : 0;
                    control.Arrange(new Rect(0, startY, arrangeSize.Width, actualheight));
                    startY += actualheight;
                }
            }

            return arrangeSize;
        }

        private int turn = 0;

        /// <summary>
        /// Measures the child elements of a <see cref="T:System.Windows.Controls.StackPanel"/> in anticipation of arranging them during the <see cref="M:System.Windows.Controls.StackPanel.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="constraint1">An upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.</param>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the desired size of the element.
        /// </returns>
        protected override Size MeasureOverride(Size constraint1)
        {
            Size constraint = new Size();
            double height = 1;
            double width = 1;
            if (Children.Count > 0 && (double.IsPositiveInfinity(constraint1.Width)|| double.IsPositiveInfinity(constraint1.Height)))
            {

                height = (((this.Children[0] as ChartArea).Parent as Chart).ActualHeight / ((this.Children[0] as ChartArea).Parent as Chart).Areas.Count) -
                            ((this.Children[0] as ChartArea).ElementMargin.Bottom * 2);
                width = ((this.Children[0] as ChartArea).Parent as Chart).ActualWidth - (this.Children[0] as ChartArea).ElementMargin.Left * 2;
            }
           constraint.Width = double.IsNaN(constraint1.Width) ? 1: double.IsPositiveInfinity(constraint1.Width) ? width : constraint1.Width;
           constraint.Height = double.IsNaN(constraint1.Height) ?1: double.IsPositiveInfinity(constraint1.Height) ? height : constraint1.Height;
            constraint.Width = constraint.Width <= 0 ? 1 : constraint.Width;
            constraint.Height = constraint.Height <= 0 ? 1 : constraint.Height;
            double ReaminingSplitRatio = 0;
            if (this.Orientation == Orientation.Horizontal)
            {
                return base.MeasureOverride(constraint);
            }
            bool indicatorenabled = false;
            foreach (Control control in this.Children)
            {
                ChartArea area = control as ChartArea;
                if (area.isIndicatorEnabled)
                {
                    indicatorenabled = true;
                }
            }
            var data = (from area in this.Children.OfType<ChartArea>() where area.isLoadedFirst && !double.IsNaN(area.Height) select area.Height);
            if (!indicatorenabled)
            {
                UpdateSplitRatio(constraint, data.Sum(), data.Count<double>());

            }

            double totalratio = 1d;
            double areaRatio = 0d;
            int areacount = this.Children.Count;
            foreach (Control control in this.Children)
            {
                ChartArea area = control as ChartArea;
                if (area != null)
                {
                    if (!area.IsSplitterDrag)
                    {
                        
                        //if (area.index == 0)
                        {
                            totalratio = (double.IsNaN(area.SplitRatio) || double.IsPositiveInfinity(totalratio)) ? 1d / areacount : area.SplitRatio;
                            areacount--;
                            areaRatio = totalratio;
                        }
                       //else
                       // {
                       //     totalratio = (1 - areaRatio) / (this.Children.Count - 1);
                       // }
                        double marginHeight = area.ElementMargin.Bottom + area.ElementMargin.Top + area.Margin.Bottom + area.Margin.Top;//area.BottomSpace  removed for the SD9897 - Unstable size of  Sync Chart Area
                        marginHeight = this.Children.Count <= 1 ? marginHeight + 60 : marginHeight;
                        if (area.index != this.Children.Count - 1)
                        {
                            area.Height = (constraint.Height - marginHeight) * totalratio;
                            constraint.Height = constraint.Height - area.Height;
                        }
                        else
                        {
                            area.Height = (constraint.Height - marginHeight);
                        }
                        area.Measure(constraint);
                    }
                    else
                    {
                        area.isAreaHeightSet = area.isLoadedFirst ? !double.IsNaN(area.Height) : area.isAreaHeightSet;
                        area.isLoadedFirst = false;

                        area.TotalSize = constraint;
                        if (_flag == true)
                        {
                            if (area.SyncChartArea != null && area.SyncChartArea.Areas.Count > 1 && this.Children.Count < 3)
                            {
                                if (area.index == 0)
                                {
                                    area.SplitRatio = 1 - (area.SyncChartArea.Areas[area.index + 1].Height / constraint.Height);
                                    _flag = false;
                                }
                            }
                        }

                        if (this.Children.Count > 2)
                        {
                            if (area.index == this.Children.Count - 1)
                            {
                                area.SplitRatio = 1 - ReaminingSplitRatio;
                            }
                            ReaminingSplitRatio = ReaminingSplitRatio + area.SplitRatio;
                        }
                        double ratio = double.IsNaN(area.SplitRatio) ? area.SplitRatio = 1d / this.Children.Count : area.SplitRatio;


                        double newheight = (constraint.Height * ratio > 0) ? constraint.Height * ratio : 0d;

                        newheight = area.isAreaHeightSet && area.isLoadedFirst ? area.Height : newheight;

                        if (area.ActualMinHeight >= newheight)
                        {
                            double tempRatio = (area.ActualMinHeight - newheight) / constraint.Height;

                            newheight = area.ActualMinHeight;

                            area.flag = true;
                            if (area.SyncChartArea != null)
                            {
                                if (area.SyncChartArea.Areas.Count > area.index + 1 && this.Children.Count < 3)
                                {
                                    area.SyncChartArea.Areas[area.index + 1].SplitRatio += tempRatio;
                                    if (area.SyncChartArea.Areas[area.index + 1].SplitRatio + area.SplitRatio != 1)
                                    {
                                        area.SyncChartArea.Areas[area.index + 1].SplitRatio = 1 - area.SplitRatio;
                                    }

                                }
                            }

                        }
                        else
                        {
                            area.flag = false;

                        }
                        if (area.SyncChartArea != null)
                        {
                            if (this.Children.Count < 3)
                            {
                                if (area.SyncChartArea.Areas.Count > area.index + 1)
                                {
                                    if (area.SyncChartArea.Areas[area.index + 1].SplitRatio + area.SplitRatio != 1)
                                    {
                                        area.SyncChartArea.Areas[area.index + 1].SplitRatio = 1 - area.SplitRatio;
                                    }
                                }
                                else
                                {
                                    if (area.index > 0 && area.SyncChartArea.Areas[area.index - 1].SplitRatio + area.SplitRatio != 1)
                                    {
                                        area.SyncChartArea.Areas[area.index - 1].SplitRatio = 1 - area.SplitRatio;
                                    }
                                }
                            }
                        }
                        if (area.BottomAreaSplitterFlag == true && area.SyncChartArea.Areas[area.index - 1].TopAreaSplitterFlag == false)
                        {
                            area.flag = true;
                        }
                        else if (area.TopAreaSplitterFlag == true)
                        {
                            area.flag = false;
                        }

                        double lastHeight = newheight >= area.ActualMinHeight ? newheight : area.ActualMinHeight;

                        area.Height = double.IsNaN(lastHeight) || double.IsPositiveInfinity(lastHeight) ? 0 : lastHeight;
                        turn = 1;
                        if (turn == 0)
                            area.IsSplitterDrag = false;
                        area.Measure(constraint);
                    }

                }
            }

            return constraint;
        }
        private void UpdateSplitRatio(ChartArea area, Size TotalSize, double areaHeight)
        {
            if (!(double.IsNaN(areaHeight)))
            {
                area.SplitRatio = Math.Round(areaHeight, 0) / TotalSize.Height;
                area.SyncChartArea.splitterFlag = true;
            }
        }
        internal bool _flag = false;
        internal bool _splitterStartFlag = true;
        internal double splitterMinValue =0;
        internal double splitterMaxValue = 1;

        /// <summary>
        /// Update the split ratio value At the time of initialize alone.
        /// </summary>
        /// <param name="totalSize"></param>
        /// <param name="totalheight"></param>
        /// <param name="totalcount"></param>

        private void UpdateSplitRatio(Size totalSize, double totalheight, double totalcount)
        {
            double totoalheight = totalheight;
            double remainingheight = totalSize.Height - totoalheight;

            foreach (Control control in this.Children)
            {
                ChartArea area = control as ChartArea;
                if (this.Children.Count == 1)
                {
                    area.SplitRatio = 1;
                    break;
                }
                if (area != null && !double.IsNaN(area.Height) && area.flag == true)
                {
                    if (area.index == 0)
                    {
                        area.SplitRatio = area.Height / totalSize.Height;
                        area.flag = false;
                    }
                    else
                    {
                        _flag = true;
                    }
                    area.flag = false;
                }
                else if (area != null && area.isLoadedFirst)
                {
                    if (area.index == 0)
                    {
                        SyncChartAreas sArea = area.SyncChartArea as SyncChartAreas;
                        double value = 0d;
                        if (sArea != null)
                        {
                            //for (int i = 1; i < sArea.Areas.Count; i++)
                            //{
                            //    if (sArea.Areas[i].SplitpositionFlag == true)
                            //        value = sArea.Areas[i].SplitterPosition + value;
                            //}
                            //if (value != 0d)
                            //{
                            //    value = (1 - value) / (sArea.Areas.Count - 1);
                            //}
                            //else
                            {
                                value = area.SplitterPosition;
                            }
                            if (value < splitterMinValue)
                                value = splitterMinValue;
                            else if (value > splitterMaxValue)
                                value = splitterMaxValue;
                            area.SplitterPosition = value;

                        }
                        if (this.Children.Count != totalcount)
                            area.SplitRatio = area.SplitterPosition == 0d ? (remainingheight / totalSize.Height) / (this.Children.Count - totalcount) : area.SplitterPosition;
                    }
                }
                else
                {

                    if (area.index == 0)
                    {
                        if (!_splitterStartFlag)
                        {
                            SyncChartAreas sArea = area.SyncChartArea as SyncChartAreas;
                            double value = 0d;
                            for (int i = 1; i < sArea.Areas.Count; i++)
                            {
                                if (SplitterPositionFlag == true)
                                    value = sArea.Areas[i].SplitterPosition + value;
                            }
                            if (value != 0d)
                            {
                                value = (1 - value) / (sArea.Areas.Count - 1);
                                if (value < splitterMinValue)
                                    value = splitterMinValue;
                                else if (value > splitterMaxValue)
                                    value = splitterMaxValue;
                                area.SplitterPosition = value;
                            }
                            double minvalue = 0d;
                            double _value = 0d;

                            SyncChartAreas sArea1 = area.SyncChartArea as SyncChartAreas;
                            for (int i = 1; i < sArea1.Areas.Count; i++)
                            {
                                minvalue = sArea1.Areas[i].SplitterBottomSpace + sArea1.Areas[i].MinHeight + 10;

                                double newheight = sArea1.Areas[i].SplitterPosition * sArea1.Areas[i].TotalSize.Height;

                                if (newheight < minvalue)
                                {
                                    _value = sArea1.Areas[i].SplitterPosition = minvalue / sArea1.Areas[i].TotalSize.Height;
                                    splitterMinValue = sArea1.Areas[i].SplitterPosition;
                                }
                            }
                            if (_value != 0d)
                            {
                                splitterMaxValue = area.SplitterPosition = _value = (1 - _value) / (sArea.Areas.Count - 1);
                            }

                        }

                    }
                }

            }

        }

    }
}