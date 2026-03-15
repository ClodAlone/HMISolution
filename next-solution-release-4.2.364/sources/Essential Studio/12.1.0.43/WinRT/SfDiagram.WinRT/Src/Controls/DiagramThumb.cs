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
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#else
using System.Windows.Controls;
using System.Windows.Input;
using PointerRoutedEventArgs = System.Windows.Input.MouseEventArgs;
using TappedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using ManipulationStartingRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using ManipulationStartedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using ManipulationDeltaRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using ManipulationCompletedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif
using Syncfusion.UI.Xaml.Diagram.Panels;
using Syncfusion.UI.Xaml.Diagram.Utility;
using System.Windows.Input;

namespace Syncfusion.UI.Xaml.Diagram.Controls
{

#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public sealed partial class DiagramThumb : Control
    {
        //internal readonly DragProvider _mDragProvider;
        private ScrollViewer _mScrollViewer;
        private DiagramThumbDragStartingEventArgs _mStartingArgs;

        internal static ResourceDictionary SegmentStyleDictionary = null;



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(DiagramThumb), new PropertyMetadata(null));



        public ICommand DragCommand
        {
            get { return (ICommand)GetValue(DragCommandProperty); }
            set { SetValue(DragCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragCommandProperty =
            DependencyProperty.Register("DragCommand", typeof(ICommand), typeof(DiagramThumb), new PropertyMetadata(null));



        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffsetX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(DiagramThumb), new PropertyMetadata(0d));



        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffsetY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(DiagramThumb), new PropertyMetadata(0d));

        

#if WPF
        static DiagramThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramThumb), new FrameworkPropertyMetadata(typeof(DiagramThumb)));
        } 
#endif

        public DiagramThumb()
        {
#if !WPF
            this.DefaultStyleKey = typeof(DiagramThumb);
#endif
            //_mDragProvider = new DragProvider();
            //_mDragProvider.InitializeDragProvider(this);


#if WINRT
            this.Tapped += DiagramThumb_Tapped;
            this.PointerPressed += DiagramThumb_PointerPressed;
            this.PointerReleased += DiagramThumb_PointerReleased;
#else
                this.MouseLeftButtonDown += DiagramThumb_PointerPressed;
                this.MouseLeftButtonUp += DiagramThumb_PointerReleased;
                this.MouseMove += DiagramThumb_PointerMoved; 
#endif

#if TOUCH
            this.ManipulationMode = ManipulationModes.None;
#endif
            this.Loaded += DiagramThumb_Loaded;
            Canvas.SetZIndex(this,1);
        }

        void DiagramThumb_Loaded(object sender, RoutedEventArgs e)
        {
            if (_mScrollViewer == null)
            {
                _mScrollViewer = this.FindVisualParent<ScrollViewer>() as ScrollViewer;
            }
        }

        void DiagramThumb_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!_mIsDragStarted)
                VisualStateManager.GoToState(this, "Normal", true);
        }

        void DiagramThumb_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!_mIsDragStarted)
                VisualStateManager.GoToState(this, "PointerOver", true);
        }

        void DiagramThumb_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Command != null)
            {
                Command.Execute(null);
                e.Handled = true;
                return;
            }
        }

        private void DoApplyTemplate()
        {
            if (_mScrollViewer == null)
            {
                _mScrollViewer = this.FindVisualParent<ScrollViewer>() as ScrollViewer;
            }
            if (Name.Equals("DuplicateCommandThumb") || Name.Equals("DrawCommandThumb") || Name.Equals("DeleteCommandThumb"))
            {
#if WINRT
                this.PointerEntered += DiagramThumb_PointerEntered;
                this.PointerExited += DiagramThumb_PointerExited;
# else 
                this.MouseEnter+=DiagramThumb_PointerEntered;
                this.MouseLeave+=DiagramThumb_PointerExited;
#endif
            }
        }

        internal Corner Corner { get; set; }
        Point _mInitialLocation = new Point(0, 0);
        Point _mCurrentLocation = new Point(0, 0);
        //internal Corner Corner { get; set; }
        private UIElement _mCurrentContainter = null;
        private bool _mIsCaptured = false;
        private bool _mIsDragStarted = false;

        private void DiagramThumb_PointerMoved(object sender, MouseEventArgs e)
        {
            if (_mIsCaptured)
            {
                if (DragCommand != null)
                {
                    object param = null;
                    if (this.Name.Equals("DrawCommandThumb"))
                    {
                        this.ReleasePointerCapture(e);
                        param = new DrawParameter(DrawingTool.Connector, e, null, null, null,
                           NullSourceTarget.SelectionAsSource | NullSourceTarget.CloneSourceAsTarget);
                        VisualStateManager.GoToState(this, "Normal", true);
                    }
                    if (this.Name.Equals("DuplicateCommandThumb"))
                    {
                        param = new DupilicateParameter() { DragClone = true, Thumb = this, PointerArgs = e };
                    }
                    _mIsCaptured = false;
                    _mIsDragStarted = true;
                    DragCommand.Execute(param);
                    return;
                }
                else
                {
                    Point newLocation = e.GetCurrentPoint(_mCurrentContainter).Position;
                    Point cumulativeChange = new Point(newLocation.X - _mInitialLocation.X,
                                                       newLocation.Y - _mInitialLocation.Y);
                    Point deltaChange = new Point(newLocation.X - _mCurrentLocation.X, newLocation.Y - _mCurrentLocation.Y);

                    if (deltaChange != new Point(0, 0))
                    {
                        if (!_mIsDragStarted)
                        {
                            _mIsDragStarted = true;
                            //DiagramThumbDragStartedEventArgs args = new DiagramThumbDragStartedEventArgs();
                            //OnDragStarted(args);

                            //if (_mDragStarted != null)
                            //{
                            //    _mDragStarted.Publish(args);
                            //}
                        }

                        DiagramThumbDragDelta cumulative = new DiagramThumbDragDelta(cumulativeChange);
                        DiagramThumbDragDelta delta = new DiagramThumbDragDelta(deltaChange);
                        _mCurrentLocation = newLocation;
                        DiagramThumbDragEventArgs eventArgs = new DiagramThumbDragEventArgs(this, cumulative, delta,
                                                                                            e,
                                                                                            _mCurrentContainter,
                                                                                            _mStartingArgs);
                        OnDragDelta(eventArgs);
                        //if (_mDragEvent != null)
                        //{
                        //    _mDragEvent.Publish(eventArgs);
                        //}
                    }
                }
            }
        }

        internal void Manual_PointerPressed(MouseEventArgs e)
        {
            DiagramThumb_PointerPressed(this, e);
        }

        void DiagramThumb_PointerPressed(object sender, MouseEventArgs e)
        {
            if (this.Visibility == Visibility.Visible && this.CapturePointer(e))
            {
#if WINRT
                this.PointerMoved += DiagramThumb_PointerMoved;
#else 
                this.MouseMove+=DiagramThumb_PointerMoved;
#endif
                _mIsCaptured = true;
                _mIsDragStarted = false;
                // ConnectorChangingEventArgs e = new ConnectorChangingEventArgs();

                DiagramThumbDragStartingEventArgs starting = new DiagramThumbDragStartingEventArgs();
                OnDragStarting(starting);
                VisualStateManager.GoToState(this, "Pressed", true);
                if (starting.Cancel)
                {
                    _mIsCaptured = false;
                    _mIsDragStarted = false;
                    this.ReleasePointerCapture(e);
                    return;
                }
                if (_mScrollViewer == null)
                {
                    _mScrollViewer = this.FindVisualParent<ScrollViewer>() as ScrollViewer;// ?? this.FindVisualParent<ConnectorEditorPanel>();
                }
                _mCurrentContainter = _mScrollViewer.Page;
                _mInitialLocation = e.GetCurrentPoint(_mCurrentContainter).Position;
                _mCurrentLocation = _mInitialLocation;

                //if (_mDragStarting != null)
                //{
                //    _mDragStarting.Publish(starting);
                //}
                _mStartingArgs = starting;
            }
            else
            {
                _mIsCaptured = false;
            }
        }

        private void DiagramThumb_PointerReleased(object sender, MouseButtonEventArgs e)
        {
            if (_mIsCaptured)
            {
                Point newLocation = e.GetCurrentPoint(_mCurrentContainter).Position;
                Point cumulativeChange = new Point(newLocation.X - _mInitialLocation.X,
                                                   newLocation.Y - _mInitialLocation.Y);
                Point deltaChange = new Point(newLocation.X - _mCurrentLocation.X, newLocation.Y - _mCurrentLocation.Y);
                DiagramThumbDragDelta cumulative = new DiagramThumbDragDelta(cumulativeChange);
                DiagramThumbDragDelta delta = new DiagramThumbDragDelta(deltaChange);
                _mCurrentLocation = newLocation;
                DiagramThumbDragEventArgs args = new DiagramThumbDragEventArgs(this, cumulative, delta, e,
                                                                               _mCurrentContainter, _mStartingArgs);
                OnDragComplete(args);
                //if (_mDragEvent != null)
                //{
                //    _mDragEvent.Publish(args);
                //}
            }

#if WINRT
                this.PointerMoved -= DiagramThumb_PointerMoved;
#else
            this.MouseMove -= DiagramThumb_PointerMoved;
#endif
            VisualStateManager.GoToState(this, "Normal", true);
            this.ReleasePointerCapture(e);
            _mIsCaptured = false;
            _mIsDragStarted = false;
        }

        public event DiagramThumbDragStartingEventHandler DragStarting;

        private void OnDragStarting(DiagramThumbDragStartingEventArgs args)
        {
            DiagramThumbDragStartingEventHandler handler = DragStarting;
            if (handler != null) handler(this, args);
        }

        public event DiagramThumbDragEventHandler DragDelta;

        private void OnDragDelta(DiagramThumbDragEventArgs args)
        {
            DiagramThumbDragEventHandler handler = DragDelta;
            if (handler != null) handler(this, args);
        }

        public event DiagramThumbDragEventHandler DragComplete;

        private void OnDragComplete(DiagramThumbDragEventArgs args)
        {
            DiagramThumbDragEventHandler handler = DragComplete;
            if (handler != null) handler(this, args);
        }

        internal void ReleaseThumbCapture()
        {
            _mIsCaptured = false;
        }
    }

    public class DiagramThumbDragEventArgs
    {
        public object Source { get; private set; }
        public DiagramThumbDragDelta Cumulative { get; private set; }
        public DiagramThumbDragDelta Delta { get; private set; }
        public PointerRoutedEventArgs PointerArgs { get; private set; }

        public UIElement Container { get; private set; }
        public DiagramThumbDragStartingEventArgs StartingArgs { get; private set; }

        public DiagramThumbDragEventArgs(
            object source,
            DiagramThumbDragDelta cumulative,
            DiagramThumbDragDelta delta,
            PointerRoutedEventArgs pointerArgs,
            UIElement container,
            DiagramThumbDragStartingEventArgs startingArgs)
        {
            Source = source;
            Cumulative = cumulative;
            Delta = delta;
            PointerArgs = pointerArgs;
            Container = container;
            StartingArgs = startingArgs;
        }
    }

    public class DiagramThumbDragDelta
    {
        public double HorizontalChange { get; private set; }
        public double VerticalChange { get; private set; }

        public DiagramThumbDragDelta(Point delta)
        {
            HorizontalChange = delta.X;
            VerticalChange = delta.Y;
        }
    }

    public class DiagramThumbDragStartingEventArgs
    {
        public UIElement Containter { get; set; }
        public Point? InitialValue { get; set; }
        public bool Cancel { get; set; }

        public DiagramThumbDragStartingEventArgs()
        {
            Cancel = false;
        }
    }

    public class DiagramThumbDragStartedEventArgs
    {
    }

    public delegate void DiagramThumbDragEventHandler(object sender, DiagramThumbDragEventArgs args);

    public delegate void DiagramThumbDragStartedEventHandler(object sender, DiagramThumbDragStartedEventArgs args);

    public delegate void DiagramThumbDragStartingEventHandler(object sender, DiagramThumbDragStartingEventArgs args);

    // ---

    internal class ManipulationDragDelta
    {
        public double HorizontalChange { get; private set; }
        public double VerticalChange { get; private set; }
        public double AngleChange { get; private set; }
        public double ScaleChange { get; private set; }

        public ManipulationDragDelta(double x, double y, double a, double s)
        {
            HorizontalChange = x;
            VerticalChange = y;
            AngleChange = a;
            ScaleChange = s;
        }
    }

    internal class ManipulationDragStartingArgs
    {
        public object Source { get; private set; }
        public bool Cancel { get; set; }
        public bool Bubble { get; set; }
        public ManipulationStartingRoutedEventArgs ManipulationArgs { get; private set; }
        //public UIElement Container { get; set; }
        //public double? MinHorizontalChange { get; set; }
        //public double? MinVerticalChange { get; set; }
        //public double? MinAngleChange { get; set; }
        //public double? MinScaleXChange { get; set; }
        //public double? MinScaleYChange { get; set; }
        public ManipulationDragStartingArgs(object source, ManipulationStartingRoutedEventArgs args)
        {
            Source = source;
            ManipulationArgs = args;
            Cancel = false;
            Bubble = false;
        }
    }

    internal class ManipulationDragEventArgs
    {
        public object Source { get; private set; }
        public ManipulationDragDelta Cumulative { get; private set; }
        public ManipulationDragDelta Delta { get; private set; }
        public ManipulationDeltaRoutedEventArgs ManipulationArgs { get; private set; }
        public UIElement Container { get; private set; }
        public double ParentScale { get; private set; }

        public ManipulationDragEventArgs(object source,
                                            ManipulationDragDelta cumulative,
                                            ManipulationDragDelta delta,
                                            ManipulationDeltaRoutedEventArgs manipulationArgs,
                                            UIElement container,
                                            double parentScale)
        {
            Source = source;
            Cumulative = cumulative;
            Delta = delta;
            ManipulationArgs = manipulationArgs;
            Container = container;
            ParentScale = parentScale;
        }

    }
    internal class NodeDragDeltaEventArgs
    {
        internal object Source { get; private set; }
        internal ManipulationDragDelta Cumulative { get; private set; }

        internal NodeDragDeltaEventArgs(object source,
                                            ManipulationDragDelta cumulative
                                           )
        {
            Source = source;
            Cumulative = cumulative;
        }

    }
    internal delegate void ManipulationStartingEventHandler(object sender, ManipulationDragStartingArgs args);
    internal delegate void ManipulationDragEventHandler(object sender, ManipulationDragEventArgs args);

    internal enum ThumbType
    {
        EndThumb,
        OrthoThumb,
        BezierThumb
    }

    //    public class DragCommandButton : Button
    //    {
    //        public DragCommandButton()
    //        {
    //#if WINRT
    //            this.AddHandler(PointerPressedEvent, new PointerEventHandler(SelectorItem_PointerPressed), true);
    //            this.AddHandler(PointerMovedEvent, new PointerEventHandler(SelectorItem_PointerMoved), true);
    //            this.AddHandler(PointerReleasedEvent, new PointerEventHandler(SelectorItem_PointerReleased), true);
    //#else
    //            this.MouseLeftButtonDown += SelectorItem_PointerPressed;
    //            this.MouseLeftButtonUp += SelectorItem_PointerReleased;
    //            this.MouseMove += SelectorItem_PointerMoved;
    //#endif
    //        }

    //        private bool isPressed = false;

    //        void SelectorItem_PointerReleased(object sender, MouseButtonEventArgs e)
    //        {
    //            this.ReleasePointerCapture(e);
    //            isPressed = false;
    //        }

    //        void SelectorItem_PointerMoved(object sender, MouseEventArgs e)
    //        {
    //            //ISelector selectorViewModel = this.DataContext as ISelector;
    //            if (isPressed)
    //            {
    //                this.ReleasePointerCapture(e);
    //                DrawParameter param = new DrawParameter(DrawingTool.Connector, e, null, null, null,
    //                    NullSourceTarget.SelectionAsSource | NullSourceTarget.CloneSourceAsTarget);
    //                if (DragCommand != null)
    //                {
    //                    DragCommand.Execute(param);
    //                }
    //                isPressed = false;
    //            }
    //        }

    //        void SelectorItem_PointerPressed(object sender, MouseButtonEventArgs e)
    //        {
    //            this.CapturePointer(e);
    //            e.Handled = true;
    //            isPressed = true;
    //        }

    //        public ICommand DragCommand
    //        {
    //            get { return (ICommand)GetValue(DragCommandProperty); }
    //            set { SetValue(DragCommandProperty, value); }
    //        }

    //        // Using a DependencyProperty as the backing store for DragCommand.  This enables animation, styling, binding, etc...
    //        public static readonly DependencyProperty DragCommandProperty =
    //            DependencyProperty.Register("DragCommand", typeof(ICommand), typeof(DragCommandButton), new PropertyMetadata(null));

    //    }
}
