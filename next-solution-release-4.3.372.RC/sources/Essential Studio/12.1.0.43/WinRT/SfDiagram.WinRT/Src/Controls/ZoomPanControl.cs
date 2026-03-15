#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
#if WINRT_USING
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;
using PointerRoutedEventArgs_Mouse = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using PointerRoutedEventArgs_Wheel = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using Windows.UI;
#else
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media; 
using System.Windows.Shapes;
using PointerRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using PointerRoutedEventArgs_Mouse = System.Windows.Input.MouseEventArgs;
using PointerRoutedEventArgs_Wheel = System.Windows.Input.MouseWheelEventArgs;
using ManipulationDeltaRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif
using Syncfusion.UI.Xaml.Diagram.Panels;
using Syncfusion.UI.Xaml.Diagram.Stencil;
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Controller;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Diagram.Controls
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public sealed partial class ScrollViewer : ContentControl, IInternalScrollInfo
    {
#if WPF
        static ScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollViewer), new FrameworkPropertyMetadata(typeof(ScrollViewer)));
        } 
#endif

        public CompositeTransform ZoomPanTransform
        {
            get { return (CompositeTransform)GetValue(ZoomPanTransformProperty); }
            set { SetValue(ZoomPanTransformProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomPanTransform.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomPanTransformProperty = DependencyProperty.Register("ZoomPanTransform", typeof(CompositeTransform), typeof(ScrollViewer), new PropertyMetadata(null));

        internal Rect PageBounds { get; set; }
        private PageBoundsChangedEvent _mPageBoundsChangedEvent;
        private ViewportChangedEvent _mViewportChangedEvent;

        internal SharedData _mSharedData;
        private DragProvider _mDragProvider;
        internal ScrollChanged _mCurrentState;
        public ScrollViewer()
        {
#if !WPF
            DefaultStyleKey = typeof(ScrollViewer);
#endif
            ZoomPanTransform = new CompositeTransform();

            //ZoomPanTransform.ScaleX = 0.5;
            //ZoomPanTransform.ScaleY = 0.5;
#if WINRT
            this.ManipulationMode =
                ManipulationModes.TranslateX |
                ManipulationModes.TranslateY |
                ManipulationModes.TranslateRailsX |
                ManipulationModes.TranslateRailsY |
                ManipulationModes.Scale;

            this.PointerPressed += ZoomPanControl_PointerPressed;
            this.PointerMoved += ZoomPanControl_PointerMoved;
            this.PointerReleased += ZoomPanControl_PointerReleased;
            this.PointerWheelChanged += ZoomPanControl_PointerWheel;
#else
            this.MouseLeftButtonDown += ZoomPanControl_PointerPressed;
            this.MouseMove += ZoomPanControl_PointerMoved;
            this.MouseLeftButtonUp += ZoomPanControl_PointerReleased;
            this.MouseWheel += ZoomPanControl_PointerWheel;
#endif
            _mDragProvider = new DragProvider(this);


            //PageWidth = 700;
            //PageHeight = 1200;
            //MultiplePage = true;
            //MinMargin = new Thickness(100);
            //MaxMargin = null;
            //PageBackground = new SolidColorBrush(Colors.White);
            //OffPageBackground = new SolidColorBrush(Colors.Gray);
            //PageOrientation = PageOrientation.Landscapse;
        }



        private void DoApplyTemplate()
        {
            //            ContentPresenter contentPresenter = GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
            //#if WPF
            //            contentPresenter.RenderTransform = ZoomPanTransform.Transform;
            //#else
            //            contentPresenter.RenderTransform = ZoomPanTransform;
            //#endif

            Canvas diagramCanvas = GetTemplateChild("PART_DiagramCanvas") as Canvas;
            DiagramPage page = _mSharedData.Page;
            Adorner adorner = _mSharedData.Adorner;
#if WPF
            _pageBackground.RenderTransform = ZoomPanTransform.Transform;
            page.RenderTransform = ZoomPanTransform.Transform;
            adorner.RenderTransform = ZoomPanTransform.Transform;
            //_mSharedData.ConnectorEditorPanel.RenderTransform = ZoomPanTransform.Transform;
#else
            //_pageBackground.RenderTransform = ZoomPanTransform;
            if (_mSharedData.Graph.PageSettings != null)
            {
                _pageBackground.Background = _mSharedData.Graph.PageSettings.PageBackground;
                _pageBackground.BorderBrush = _mSharedData.Graph.PageSettings.PageBorderBrush;
                if (_mSharedData.Graph.PageSettings.PageBorderThickness.HasValue)
                {
                    _pageBackground.BorderThickness = _mSharedData.Graph.PageSettings.PageBorderThickness.Value;
                }
                _mSharedData.SpatialSearch.UpdatePageBounds();
#if SyncfusionFramework4_5_1 && WINRT
                _mSharedData.PageSettingsController.UpdateExportSettings();
#endif
            }
            page.RenderTransform = ZoomPanTransform;
            //adorner.RenderTransform = ZoomPanTransform;
            //_mSharedData.ConnectorEditorPanel.RenderTransform = ZoomPanTransform;
            DiagramThumb.SegmentStyleDictionary = Tag as ResourceDictionary;
#endif
            _mSharedData._mPreview = new ContentPresenter()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,

            };
            diagramCanvas.Children.Add(_pageBackground);
            diagramCanvas.Children.Add(_mSharedData.GridLinePanel);
            diagramCanvas.Children.Add(page);
            diagramCanvas.Children.Add(adorner);
            //diagramCanvas.Children.Add(_mSharedData.ConnectorEditorPanel);
            diagramCanvas.Children.Add(_mSharedData._mPreview);
            VerticalScrollBar = GetTemplateChild("VerticalScrollBar") as ScrollBar;
            HorizontalScrollBar = GetTemplateChild("HorizontalScrollBar") as ScrollBar;
        }

        internal void SetSharedData(SharedData data)
        {
            _mSharedData = data;
            _mSharedData.SetScrollViewer(this);
            _mDragProvider.InitializeDragProvider(_mSharedData);

            DragStartingEvent dragStarting = _mSharedData.EventAggregator.GetEvent<DragStartingEvent>();
            DragEvent drag = _mSharedData.EventAggregator.GetEvent<DragEvent>();
            dragStarting.Subscribe(OnDragStarting);
            drag.Subscribe(OnDragDelta);

            _mPageBoundsChangedEvent = _mSharedData.EventAggregator.GetEvent<PageBoundsChangedEvent>();
            _mPageBoundsChangedEvent.Subscribe(OnBoundsChanged);

            _mViewportChangedEvent = _mSharedData.EventAggregator.GetEvent<ViewportChangedEvent>();
            _mViewportChangedEvent.Publish(new ChangeArgs<Rect>(null, Rect.Empty,
                                                                new Rect(HorizontalOffset, VerticalOffset,
                                                                         ViewportWidth, ViewportHeight)));
            _mCurrentState = new ScrollChanged(Viewport, new Rect(0, 0, 0, 0), new Rect(0, 0, 0, 0), CurrentZoom, MinZoom, MaxZoom, ZoomFactor, ScrollFactor);

        }

        internal void OnBoundsChanged(ChangeArgs<Rect> args)
        {
            PageBounds = args.NewValue;
            InvalidateArrange();
        }

        internal void OnDragStarting(ManipulationDragStartingArgs args)
        {
            if (args.Source != this)
                return;

#if WINRT
            if (!_mSharedData.Graph.Constraints.Contains(GraphConstraints.Pannable))
            {
                args.ManipulationArgs.Mode &= ~(ManipulationModes.TranslateX | ManipulationModes.TranslateY);
            }
            if (!_mSharedData.Graph.Constraints.Contains(GraphConstraints.PanRailsX))
            {
                args.ManipulationArgs.Mode &= ~(ManipulationModes.TranslateRailsX);
            }
            if (!_mSharedData.Graph.Constraints.Contains(GraphConstraints.PanRailsY))
            {
                args.ManipulationArgs.Mode &= ~(ManipulationModes.TranslateRailsY);
            }
            if (!_mSharedData.Graph.Constraints.Contains(GraphConstraints.Zoomable))
            {
                args.ManipulationArgs.Mode &= ~(ManipulationModes.Scale);
            }
#endif
        }

        internal void OnDragDelta(ManipulationDragEventArgs args)
        {
            if (args.Source != this)
                return;
            if (_mStartPoint == null || _mCaptureCount > 1)
            {
#if WINRT
                ZoomIn(new ZoomManipulationParamenter { ManipulationArgs = args.ManipulationArgs });
#else
            ZoomPanDelta(args.Delta.HorizontalChange,
                         args.Delta.VerticalChange,
                         args.Delta.ScaleChange);
#endif
            }
        }

        private void ZoomPanControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
#if WINRT
            if (_mSharedData.Graph.Tool.Contains(Tool.DrawOnce) || _mSharedData.Graph.Tool.Contains(Tool.ContinuesDraw))
            {
                if (_mSharedData.Graph.DefaultConnectorType == ConnectorType.PolyCubicBezier)
                {
                    if (_mCaptureCount <= 0)
                    {
                        Manual_PointerReleased(null, null, null, e);
                    }
                    else
                    {
                        object targetNode = null;
                        IPort targetPort = null;
                        object sourceCon = null;
                        DiagramThumb thumb = null;
                        FindElementsAtMousePosition(e.GetCurrentPoint(null).Position, ref targetNode, ref targetPort, ref thumb, ref sourceCon);
                        DiagramPage page = _mSharedData.Page;
                        Manual_PointerReleased(e.GetCurrentPoint(page).Position, targetNode, targetPort, e);
                    }

                }
                _mSharedData.Graph.Tool = _mSharedData.Graph.Tool & ~Tool.DrawOnce;
            }
            else
#endif
                if (_mCaptureCount == 1 && _mStartPoint.HasValue)
                {
                    //DragProvider.InteractiveObject = null;
                    DiagramPage page = _mSharedData.Page;
                    Point currentPosition = e.GetCurrentPoint(page).Position;
                    Rect rect = new Rect(_mStartPoint.Value, currentPosition);
                    this.ReleasePointerCapture(e);
                    _mSharedData.selectionRectangle.Visibility = Visibility.Collapsed;
                    //if (rect.Width > 0 || rect.Height > 0)
                    {
                        _mSharedData.EventAggregator.GetEvent<SelectionRectangleChangedEvent>().Publish(rect);
                    }
                }
            if (_mCaptureCount > 0)
            {
                _mCaptureCount--;
            }
            _mStartPoint = null;
        }

        private void ZoomPanControl_PointerMoved(object sender, PointerRoutedEventArgs_Mouse e)
        {
            if (_mCaptureCount == 1 && _mStartPoint.HasValue)
            {
#if WINRT
                if (_mSharedData.Graph.Tool.Contains(Tool.DrawOnce) ||
                    _mSharedData.Graph.Tool.Contains(Tool.ContinuesDraw))
                {
                    if (_mSharedData.Graph.DefaultConnectorType == ConnectorType.PolyCubicBezier)
                    {
                        (_mSharedData.Graph as SfDiagramWrapper).UpdateDrawingPath(e);
                    }
                }
                else if (_mSharedData.Graph.Tool.Contains(Tool.MultipleSelect) && _pointerId == e.Pointer.PointerId)
                {
                    _mSharedData.selectionRectangle.Visibility = Visibility.Visible;
                    DiagramPage page = _mSharedData.Page;
                    Point currentPosition = e.GetCurrentPoint(page).Position;
                    Rect rect = new Rect(_mStartPoint.Value, currentPosition);
                    _mSharedData.selectionRectangle.RenderTransform = new TranslateTransform()
                        {
                            X = rect.Left,
                            Y = rect.Y
                        };
                    _mSharedData.selectionRectangle.Width = rect.Width;
                    _mSharedData.selectionRectangle.Height = rect.Height;
                }
                else if (_pointerId != e.Pointer.PointerId)
                {
                    if (_mCaptureCount > 0)
                        _mCaptureCount--;
                }
#endif
            }
        }

        private Point? _mStartPoint = null;
        private int _mCaptureCount = 0;
        internal readonly Border _pageBackground = new Border();

        internal void Manual_PointerPressed(Point? startPoint,
                                            object node, object port, PointerRoutedEventArgs e)
        {
            _mSharedData.EventAggregator.GetEvent<DrawStartedEvent>().Publish(new DrawParameter(
                                                                                  _mSharedData.Graph.DrawingTool,
                                                                                  e, startPoint, node, port));
        }

        internal void Manual_PointerReleased(Point? endPoint,
                                            object node, object port, PointerRoutedEventArgs e)
        {
            _mSharedData.EventAggregator.GetEvent<DrawingCompletedEvent>().Publish(new DrawParameter(
                                                                                  _mSharedData.Graph.DrawingTool,
                                                                                  e, endPoint, node, port));
        }
        //private void ZoomPanControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        //{
        //    INode sourceNode = null;
        //    IPort sourcePort = null;
        //    bool abort = false;
        //    foreach (
        //        var element in VisualTreeHelper.FindElementsInHostCoordinates(e.GetCurrentPoint(null).Position, null))
        //    {
        //        if (element is INode || element is IConnector || element is IPort)
        //        {
        //            if (!(element is IConnector) &&
        //                _mSharedData.Graph.Tool.Contains(Tool.DrawOnce) ||
        //                _mSharedData.Graph.Tool.Contains(Tool.ContinuesDraw))
        //            {
        //                sourceNode = element as INode;
        //                sourcePort = element as IPort;
        //            }
        //            else
        //            {
        //                abort = true;
        //            }
        //            break;
        //        }
        //        else if (element == this)
        //        {
        //            break;
        //        }
        //    }
        //    if (!abort && _mStartPoint == null)
        //    {
        //        if (_mSharedData.Graph.Tool.Contains(Tool.DrawOnce) ||
        //            _mSharedData.Graph.Tool.Contains(Tool.ContinuesDraw))
        //        {
        //            DiagramPage page = _mSharedData.Page;
        //            Manual_PointerPressed(e.GetCurrentPoint(page).Position, sourceNode, sourcePort, e);

        //            //DragProvider.InteractiveObject = null;
        //        }
        //        else if (_mSharedData.Graph.Tool.Contains(Tool.ZoomPan) &&
        //                 (_mSharedData.Graph.Constraints.Contains(GraphConstraints.Pannable)) &&
        //                 (_mSharedData.Graph.Constraints.Contains(GraphConstraints.Zoomable)))
        //        {
        //            DragProvider.InteractiveObject = typeof (ZoomPanControl);
        //        }
        //        else if (_mSharedData.Graph.Tool.Contains(Tool.MultipleSelect))
        //        {
        //            DiagramPage page = _mSharedData.Page;
        //            _mStartPoint = e.GetCurrentPoint(page).Position;
        //            DragProvider.InteractiveObject = typeof (Rectangle);
        //            if (CapturePointer(e.Pointer))
        //            {
        //                _mCapturedPointer = e.Pointer;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (abort)
        //        {
        //            if (DragProvider.InteractiveObject == typeof (Rectangle) ||
        //                DragProvider.InteractiveObject == typeof (DrawingTool))
        //            {
        //                DragProvider.InteractiveObject = null;
        //            }
        //        }
        //        else
        //        {
        //            DragProvider.InteractiveObject = typeof (ZoomPanControl);
        //        }
        //        _mStartPoint = null;
        //        if (_mCapturedPointer != null)
        //        {
        //            ReleasePointerCapture(_mCapturedPointer);
        //            _mSharedData.selectionRectangle.Visibility = Visibility.Collapsed;
        //        }
        //        _mCapturedPointer = null;
        //    }
        //}
        uint _pointerId;
        private void ZoomPanControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            object sourceNode = null;
            IPort sourcePort = null;
            object sourceCon = null;
            DiagramThumb thumb = null;
            FindElementsAtMousePosition(e.GetCurrentPoint(null).Position, ref sourceNode, ref sourcePort, ref thumb, ref sourceCon);
            if (_mSharedData.Graph.Tool.Contains(Tool.DrawOnce) ||
                _mSharedData.Graph.Tool.Contains(Tool.ContinuesDraw))
            {

                DiagramPage page = _mSharedData.Page;
                Manual_PointerPressed(e.GetCurrentPoint(page).Position, sourceNode, sourcePort, e);
#if WINRT
                if (_mSharedData.Graph.DefaultConnectorType == ConnectorType.PolyCubicBezier)
                {
                    _mCaptureCount++;
                    _mStartPoint = e.GetCurrentPoint(_mSharedData.Page).Position;
                    this.CapturePointer(e);
                }
# endif
                return;
            }
            else if (_mSharedData.Graph.Tool.Contains(Tool.ZoomPan) &&
                     (_mSharedData.Graph.Constraints.Contains(GraphConstraints.Pannable)) &&
                     (_mSharedData.Graph.Constraints.Contains(GraphConstraints.Zoomable)))
            {
            }
            else if (sourceNode != null || sourcePort != null || thumb != null)
            {
                //_mCaptureCount++;
            }
            else if (_mSharedData.Graph.Tool.Contains(Tool.MultipleSelect))
            {
                if (_mStartPoint == null)
                {
                    if (sourceCon == null)
                    {
#if WINRT
                        _pointerId = e.Pointer.PointerId;
#endif
                        _mCaptureCount++;
                        DiagramPage page = _mSharedData.Page;
                        _mStartPoint = e.GetCurrentPoint(page).Position;
                        //DragProvider.InteractiveObject = typeof(Rectangle);
                        this.CapturePointer(e);
                    }
                }
                else
                {
                    this.CapturePointer(e);
                    _mCaptureCount++;
                    _mStartPoint = null;
                    _mSharedData.selectionRectangle.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void FindElementsAtMousePosition(Point e, ref object sourceNode, ref IPort sourcePort, ref DiagramThumb thumb, ref object sourceCon)
        {
            foreach (var element in this.FindElementsInHostCoordinates(e))
            {
                if (element is INode || element is IConnector || element is IPort || element is DiagramThumb)
                {
                    //if (!(element is IConnector) &&
                    //    _mSharedData.Graph.Tool.Contains(Tool.DrawOnce) ||
                    //    _mSharedData.Graph.Tool.Contains(Tool.ContinuesDraw))
                    //{
                    if (element is Node)
                    {
                        sourceNode = (element as Node).Wrapper.Source;
                    }
                    else if (element is Connector)
                    {
                        sourceCon = (element as Connector).Wrapper.Source;
                    }
                    else
                    {
                        sourceCon = element as IConnector;
                        sourceNode = element as INode;
                    }
                    thumb = element as DiagramThumb;
                    sourcePort = element as IPort;
                    //}
                    break;
                }
                else if (element == this)
                {
                    break;
                }
            }
            //throw new NotImplementedException();
        }


        private void ZoomPanControl_PointerWheel(object sender, PointerRoutedEventArgs_Wheel e)
        {
            if (_mSharedData.Graph.Constraints.Contains(GraphConstraints.Zoomable))
            {
                UpdateVisualState(e);
#if WINRT
                double delta = e.GetCurrentPoint(this).Properties.MouseWheelDelta;
#else
            double delta = e.Delta;
#endif
                if (CodeSharingUtilities.IsControlKeyPressed())
                {
                    ZoomPanDelta(0, 0, 1 + ZoomFactor * (delta > 0 ? 1 : -1), e.GetCurrentPoint(Page).Position);
                }
                else
                {
                    if (ScrollFactor.HasValue)
                    {
                        delta = (delta > 0 ? 1 : -1) * ScrollFactor.Value;
                    }
                    ZoomPanDelta(0, delta, 1);
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = base.MeasureOverride(availableSize);
            desiredSize = new Size(availableSize.Width.IsValid() ? availableSize.Width : desiredSize.Width,
                                   availableSize.Height.IsValid() ? availableSize.Height : desiredSize.Height);
            if (_mViewportChangedEvent != null) // && !availableSize.Width.IsValid() || !availableSize.Height.IsValid())
            {
                ViewportWidth = desiredSize.Width;
                ViewportHeight = desiredSize.Height;
            }
            Update();
            return desiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            ViewportWidth = finalSize.Width;
            ViewportHeight = finalSize.Height;
            Update();
            return new Size(finalSize.Width + 1, finalSize.Height + 1);
        }

        //public double CurrentZoom
        //{
        //    get { return ZoomPanTransform.ScaleX; }
        //    private set
        //    {
        //        double delta = value / ZoomPanTransform.ScaleX;
        //        //////ViewportWidth /= delta;
        //        //////ViewportHeight /= delta;
        //        //////HorizontalOffset /= delta;
        //        //////VerticalOffset /= delta;
        //        ////Left *= delta;
        //        ////Top *= delta;
        //        ////Right *= delta;
        //        ////Bottom *= delta;
        //        ZoomPanTransform.ScaleX = value;
        //        ZoomPanTransform.ScaleY = value;
        //    }
        //}

        public void BringIntoViewport(Rect bounds)
        {
            double x = HorizontalOffset, y = VerticalOffset;
            bounds = new Rect(new Point(bounds.X * CurrentZoom, bounds.Y * CurrentZoom), new Size(bounds.Width * CurrentZoom, bounds.Height * CurrentZoom));
            Rect viewport = _mSharedData.Graph.ScrollInfo.Viewport;
            //viewport.Intersect(bounds);
            //if (viewport != bounds)
            {
                //viewport = _mSharedData.Graph.Viewport;
                //if (bounds.Left < viewport.Left ||
                //    bounds.Right > viewport.Right)
                {
                    if (bounds.Right > viewport.Right)
                    {
                        x = bounds.Right - ViewportWidth;
                    }
                    if (bounds.Left < viewport.Left)
                    {
                        x = bounds.Left;
                    }
                }

                //if (bounds.Top < viewport.Top ||
                //    bounds.Bottom > viewport.Bottom)
                {
                    if (bounds.Bottom > viewport.Bottom)
                    {
                        y = bounds.Bottom - ViewportHeight;
                    }
                    if (bounds.Top < viewport.Top)
                    {
                        y = bounds.Top;
                    }
                }
            }
            ZoomPanTo(x, y, null, true);
        }

        public void BringIntoCenter(Rect bounds)
        {
            Point focusPoint = new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
            focusPoint = new Point(focusPoint.X * CurrentZoom, focusPoint.Y * CurrentZoom);
            double x = focusPoint.X - ViewportWidth / 2;
            double y = focusPoint.Y - ViewportHeight / 2;
            ZoomPanTo(x, y, null, true);
        }

        private void ZoomPanTo(double? x, double? y, double? scale, bool animate = false)
        {
            if (animate)
            {
                Storyboard storyboard = new Storyboard();
                DoubleAnimation xAnimation = new DoubleAnimation();
                xAnimation.From = HorizontalOffset;
                xAnimation.To = x ?? HorizontalOffset;
                xAnimation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 5 };
                xAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
                DoubleAnimation yAnimation = new DoubleAnimation();
                yAnimation.From = VerticalOffset;
                yAnimation.To = y ?? VerticalOffset;
                yAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
                yAnimation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 5 };
                Storyboard.SetTarget(xAnimation, this);
                Storyboard.SetTarget(yAnimation, this);
#if WINRT
                Storyboard.SetTargetProperty(xAnimation, "HorizontalOffset");
                Storyboard.SetTargetProperty(yAnimation, "VerticalOffset");
                xAnimation.EnableDependentAnimation = true;
                yAnimation.EnableDependentAnimation = true;
#else
                Storyboard.SetTargetProperty(xAnimation, new PropertyPath("HorizontalOffset"));
                Storyboard.SetTargetProperty(yAnimation, new PropertyPath("VerticalOffset")); 
#endif
                storyboard.Children.Add(xAnimation);
                storyboard.Children.Add(yAnimation);
                storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                storyboard.Begin();
            }
            else
            {
                HorizontalOffset = x ?? HorizontalOffset;
                VerticalOffset = y ?? VerticalOffset;
            }

            CurrentZoom = scale ?? CurrentZoom;

        }

        private void ZoomPanDelta(double deltax, double deltay, double deltaScale, Point? focusPoint = null)
        {
            MatrixExt _mSoureMatrix = MatrixExt.Identity;
            _mSoureMatrix.Scale(CurrentZoom, CurrentZoom);
            _mSoureMatrix.Translate(-HorizontalOffset, -VerticalOffset);
            double newSale = CurrentZoom * deltaScale;
            if (newSale > MaxZoom)
            {
                deltaScale = MaxZoom / CurrentZoom;
            }
            else if (newSale < MinZoom)
            {
                deltaScale = MinZoom / CurrentZoom;
            }
            MatrixExt delta = MatrixExt.Identity;
            Point pivot;
            if (focusPoint.HasValue)
            {
                pivot = _mSoureMatrix.Transform(focusPoint.Value);
            }
            else
            {
                pivot = new Point(ViewportWidth / 2, ViewportHeight / 2);
            }
            delta.ScaleAt(deltaScale, deltaScale, pivot.X, pivot.Y);
            newSale = CurrentZoom * deltaScale;
            delta.Translate(deltax, deltay);
            _mSoureMatrix = _mSoureMatrix * delta;
            Point topLeft = _mSoureMatrix.Transform(new Point(0, 0));
            ZoomPanTo(-topLeft.X, -topLeft.Y, newSale);
        }

        private void ZoomInOutParam(IZoomParameter param, bool isIn)
        {
            if (param is IZoomManipulationParameter)
            {
                var args = (param as IZoomManipulationParameter);
#if TOUCH
                if (args.ManipulationArgs != null)
                {
                    ZoomPanDelta(args.ManipulationArgs.Delta.Translation.X,
                                 args.ManipulationArgs.Delta.Translation.Y,
                                 args.ManipulationArgs.Delta.Scale,
                                 args.ManipulationArgs.Position);
                }
#endif
            }
            else if (param is IZoomPointerParameter)
            {
                var args = (param as IZoomPointerParameter);
                if (args.PointerArgs != null)
                {
                    ZoomPanDelta(0, 0,
                        isIn ? 1 + ZoomFactor : 1 - ZoomFactor,
                        args.PointerArgs.GetCurrentPoint(Page).Position);
                }
            }
            else if (param is IZoomPositionParameter)
            {
                var args = (param as IZoomPositionParameter);

                ZoomPanDelta(0, 0,
                             args.ZoomTo / CurrentZoom ??
                             (isIn ? 1 + (args.ZoomFactor ?? ZoomFactor)
                                   : 1 - (args.ZoomFactor ?? ZoomFactor)),
                             args.FocusPoint);

            }
        }

        public void ZoomIn(IZoomParameter param)
        {
            ZoomInOutParam(param, true);
        }

        public void ZoomOut(IZoomParameter param)
        {
            ZoomInOutParam(param, false);
        }

        public void Pan(Point delta)
        {
            ZoomPanTo(HorizontalOffset + delta.X, VerticalOffset + delta.Y, null);
        }

        public void PanTo(Point position)
        {
            ZoomPanTo(position.X, position.Y, null);
        }

        public void Reset()
        {
            ZoomPanTo(0, 0, 1);
        }

        public void ResetZoom()
        {
            ZoomPanTo(HorizontalOffset, VerticalOffset, 1);
        }

        public void ResetPan()
        {
            ZoomPanTo(0, 0, CurrentZoom);
        }


        public void SetPageBackground(object value, string property)
        {
            if (property.Equals("PageBackground"))
            {
                _pageBackground.Background = value as Brush;
            }
            else if (property.Equals("PageBorderThickness"))
            {
                if (value != null)
                {
                    _pageBackground.BorderThickness = (value as Thickness?).Value;
                }
            }
            else if (property.Equals("PageBorderBrush"))
            {
                _pageBackground.BorderBrush = value as Brush;
            }
        }

        //public void UpdateOffsets(double horizontalOffset, double verticalOffset)
        //{
        //    HorizontalOffset = horizontalOffset;
        //    VerticalOffset = verticalOffset;
        //}
    }

    public interface IScrollInfo
    {
        Rect Viewport { get; }

        double CurrentZoom { get; }
        double ZoomFactor { get; set; }
        double? ScrollFactor { get; set; }

        void BringIntoViewport(Rect bounds);
        void BringIntoCenter(Rect bounds);

        double MinZoom { get; set; }
        double MaxZoom { get; set; }

        double HorizontalOffset { get; }
        double VerticalOffset { get; }
        double ViewportWidth { get; }
        double ViewportHeight { get; }

        void ZoomIn(IZoomParameter param);
        void ZoomOut(IZoomParameter param);
        void Pan(Point delta);
        void PanTo(Point position);

        void Reset();
        void ResetZoom();
        void ResetPan();
    }

    internal interface IInternalScrollInfo : IScrollInfo
    {
        double Left { get; }
        double Top { get; }
        double Right { get; }
        double Bottom { get; }

        void SetPageBackground(object background, string propname);
        //void UpdateOffsets(double horizonatalOffset,double verticalOffset);
    }
}
