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
using System.Windows;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input; 
using Windows.UI.Xaml.Controls.Primitives;
#else
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Controls; 
using PressedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using PointerRoutedEventArgs = System.Windows.Input.MouseEventArgs;

using TappedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using ManipulationStartingRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using ManipulationStartedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using ManipulationDeltaRoutedEventArgs = System.Windows.Input.MouseEventArgs;
using ManipulationCompletedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using ManipulationInertiaStartingRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Utility;
using System.Diagnostics;
using Syncfusion.UI.Xaml.Diagram.Panels;
using ScrollViewer = Syncfusion.UI.Xaml.Diagram.Controls.ScrollViewer;

namespace Syncfusion.UI.Xaml.Diagram.Controls
{
    internal enum InteractiveObject
    {
        None,
        Node,
        Zoom
    }

    internal class DragProvider
    {
        private readonly Control _mSource;
        private EventAggregartor _mEventAggregartor;

        private UIElement _mCurrentContainter = null;

        private DragStartingEvent _mDragStarting;
        private DragEvent _mDragEvent;
        private SharedData _mSharedData;
        private UIElement _mOriginalSource;

        public DragProvider(Control source)
        {
            _mSource = source;
#if WINRT
            source.AddHandler(ScrollViewer.PointerPressedEvent, new PointerEventHandler(source_PointerPressed), true);
            source.ManipulationStarting += OnManipulationStarting;
            source.ManipulationStarted += OnManipulationStarted;
            source.ManipulationDelta += OnManipulationDelta;
            source.ManipulationInertiaStarting += OnManipulationInertiaStarting;
            source.ManipulationCompleted += OnManipulationCompleted;
#else
            source.AddHandler(ScrollViewer.MouseLeftButtonDownEvent, new MouseEventHandler(source_PointerPressed), true);
            source.MouseLeftButtonDown += source_MouseLeftButtonDown;
            source.MouseMove += source_MouseMove;
            source.MouseLeftButtonUp += source_MouseLeftButtonUp;
#endif
        }

        void source_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _mOriginalSource = e.OriginalSource as UIElement;
        }

        public void InitializeDragProvider(SharedData sharedData)
        {
            _mSharedData = sharedData;

            _mEventAggregartor = sharedData.EventAggregator;
            _mDragStarting = _mEventAggregartor.GetEvent<DragStartingEvent>();
            _mDragEvent = _mEventAggregartor.GetEvent<DragEvent>();
        }


        private bool UpdateHandle(RoutedEventArgs e)
        {
            bool handle = true;
            if (_mSource is ScrollViewer &&
                _mOriginalSource != null &&
                _mOriginalSource.FindVisualParent<ScrollBar>() != null)
            {
                handle = false;
            }
            if (_mSource is Node)
            {
                if (_mSharedData.Graph.Tool.Contains(Tool.ContinuesDraw) ||
                    _mSharedData.Graph.Tool.Contains(Tool.DrawOnce))
                {
                    handle = false;
                }
                NodeConstraints constraints = (_mSource as Node).Wrapper.Constraints;
                if (_mSharedData.Graph.Constraints.Contains(GraphConstraints.Pannable) ||
                    _mSharedData.Graph.Constraints.Contains(GraphConstraints.Zoomable))
                {
                    if (constraints.Contains(NodeConstraints.AllowPan))
                    {
                        handle = false;
                    }
                }
            }
            else if(_mSource is ScrollViewer)
            {
                if (_mSharedData.Graph.Tool.Contains(Tool.ContinuesDraw) ||
                    _mSharedData.Graph.Tool.Contains(Tool.DrawOnce))
                {
                    handle = false;
                }
                else if (!_mSharedData.Graph.Constraints.Contains(GraphConstraints.Pannable) &&
                    !_mSharedData.Graph.Constraints.Contains(GraphConstraints.Zoomable))
                {
                    handle = false;
                }
            }
            if (handle)
            {
                if (e is ManipulationCompletedRoutedEventArgs)
                {
                    (e as ManipulationCompletedRoutedEventArgs).Handled = true;
                }
                else if (e is ManipulationDeltaRoutedEventArgs)
                {
#if !SILVERLIGHT
                    (e as ManipulationDeltaRoutedEventArgs).Handled = true; 
#endif
                }
                else if (e is ManipulationStartedRoutedEventArgs)
                {
                    (e as ManipulationStartedRoutedEventArgs).Handled = true;
                }
                else if (e is ManipulationStartingRoutedEventArgs)
                {
                    (e as ManipulationStartingRoutedEventArgs).Handled = true;
                }
                else if (e is ManipulationInertiaStartingRoutedEventArgs)
                {
                    (e as ManipulationInertiaStartingRoutedEventArgs).Handled = true;
                }
            }
            return handle;
        }
#if WINRT
        private void OnManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            _mCurrentContainter = _mSharedData.Page;
            e.Container = _mCurrentContainter;
            UpdateHandle(e);
            if (e.Handled)
            {
                if (_mSource is ScrollViewer)
                {
                    ManipulationModes removeModes = ManipulationModes.None;
                    if (!_mSharedData.Graph.Constraints.Contains(GraphConstraints.PannableX))
                    {
                        removeModes |= ManipulationModes.TranslateX;
                    }
                    if (!_mSharedData.Graph.Constraints.Contains(GraphConstraints.PannableY))
                    {
                        removeModes |= ManipulationModes.TranslateY;
                    }
                    if (!_mSharedData.Graph.Constraints.Contains(GraphConstraints.Zoomable))
                    {
                        removeModes |= ManipulationModes.Scale;
                    }
                    if (!_mSharedData.Graph.Constraints.Contains(GraphConstraints.PanRailsX))
                    {
                        removeModes |= ManipulationModes.TranslateRailsX;
                    }
                    if (!_mSharedData.Graph.Constraints.Contains(GraphConstraints.PanRailsY))
                    {
                        removeModes |= ManipulationModes.TranslateRailsY;
                    }
                    e.Mode &= ~removeModes;
                }
                ManipulationDragStartingArgs starting = new ManipulationDragStartingArgs(_mSource, e);
                _mDragStarting.Publish(starting);
            }
        }

        private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            UpdateHandle(e);
            if (e.Handled)
            {
                if (_mSharedData.UndoRedoController != null)
                {
                    _mSharedData.UndoRedoController.BeginComposite(null);
                }
                DiagramThumbDragStartedEventArgs thumbStarted = new DiagramThumbDragStartedEventArgs();
                OnDragStarted(thumbStarted);
            }
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            UpdateHandle(e);
            if (e.Handled)
            {
                double parentScale = 1;
                if (_mSource is INode)
                {
                    parentScale = _mSharedData.ScrollViewer.ZoomPanTransform.ScaleX;
                }

                ManipulationDragDelta delta = new ManipulationDragDelta(
                    e.Delta.Translation.X,
                    e.Delta.Translation.Y,
                    e.Delta.Rotation,
                    e.Delta.Scale);
                ManipulationDragDelta cum = new ManipulationDragDelta(
                    e.Cumulative.Translation.X,
                    e.Cumulative.Translation.Y,
                    e.Cumulative.Rotation,
                    e.Cumulative.Scale);
                ManipulationDragEventArgs drag = new ManipulationDragEventArgs(
                    _mSource, cum, delta, e, _mCurrentContainter, parentScale);
                _mDragEvent.Publish(drag);
            }
        }

        private void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            UpdateHandle(e);
            if (e.Handled)
            {
                double parentScale = 1;
                if (_mSource is INode)
                {
                    parentScale = _mSharedData.ScrollViewer.ZoomPanTransform.ScaleX;
                }
                ManipulationDragDelta delta = new ManipulationDragDelta(0, 0, 0, 1);
                ManipulationDragDelta cum = new ManipulationDragDelta(
                    e.Cumulative.Translation.X,
                    e.Cumulative.Translation.Y,
                    e.Cumulative.Rotation,
                    e.Cumulative.Scale);
                ManipulationDragEventArgs drag = new ManipulationDragEventArgs(
                    _mSource, cum, delta, null, _mCurrentContainter, parentScale);
                _mDragEvent.Publish(drag);
                if (_mSharedData.UndoRedoController != null) 
                    _mSharedData.UndoRedoController.EndComposite();
                if (_mSource is INode)
                {
                    if (_mSharedData.Graph.InternalConnectors != null)
                    {
                        foreach (IInternalConnector con in _mSharedData.Graph.InternalConnectors)
                        {
                            if (con.CanRoute())
                            {
                                con.UpdateRouting();
                                (con as ConnectorWrapper).isRoutted = true;
                                (con.View as Connector).InvalidateArrange();
                            }
                        }
                    }
                }
                _mSharedData.Adorner.ClearGuidelines();
            }
        }
#else
        private Point? _mStartPoint = null;

        void source_MouseLeftButtonDown(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            _mCurrentContainter = _mSharedData.Page;
            _mStartPoint = e.GetCurrentPoint(null).Position;
            UpdateHandle(e);
            if (e.Handled)
            {
                _mSource.CaptureMouse();
                ManipulationDragStartingArgs starting = new ManipulationDragStartingArgs(_mSource, e);
                _mDragStarting.Publish(starting);
                if (_mSharedData.UndoRedoController != null)
                {
                    _mSharedData.UndoRedoController.BeginComposite(null);
                }
                DiagramThumbDragStartedEventArgs thumbStarted = new DiagramThumbDragStartedEventArgs();
                OnDragStarted(thumbStarted);
            }
        }

        void source_MouseMove(object sender, PointerRoutedEventArgs e)
        {
            if (_mStartPoint != null)
            {
                Point current = e.GetCurrentPoint(null).Position;
                UpdateHandle(e);
#if !SILVERLIGHT
                if (e.Handled) 
#else
                if(UpdateHandle(e))
#endif
                {
                    double parentScale = 1;
                    if (_mSource is INode)
                    {
                        parentScale = (_mSource).FindVisualParent<ScrollViewer>().ZoomPanTransform.ScaleX;
                    }

                    ManipulationDragDelta delta = new ManipulationDragDelta(
                         current.X - _mStartPoint.Value.X,
                         current.Y - _mStartPoint.Value.Y,
                        0, 1);
                    _mStartPoint = current;
                    ManipulationDragEventArgs drag = new ManipulationDragEventArgs(
                        _mSource, null, delta, null, _mCurrentContainter, parentScale);
                    _mDragEvent.Publish(drag);
                }
            }
        }

        void source_MouseLeftButtonUp(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            if (_mStartPoint != null)
            {
                UpdateHandle(e);
                Point current = e.GetCurrentPoint(null).Position;
                if (e.Handled)
                {
                    _mSource.ReleaseMouseCapture();
                    double parentScale = 1;
                    if (_mSource is INode)
                    {
                        parentScale = (_mSource).FindVisualParent<ScrollViewer>().ZoomPanTransform.ScaleX;
                    }
                    ManipulationDragDelta delta = new ManipulationDragDelta(
                        _mStartPoint.Value.X - current.X,
                        _mStartPoint.Value.Y - current.Y,
                        0, 1);
                    ManipulationDragEventArgs drag = new ManipulationDragEventArgs(
                        _mSource, null, delta, null, _mCurrentContainter, parentScale);
                    _mDragEvent.Publish(drag);
                    if (_mSharedData.UndoRedoController != null)
                        _mSharedData.UndoRedoController.EndComposite();
                    _mStartPoint = null;
                }
            }
        }
#endif
        private void OnDragStarted(DiagramThumbDragStartedEventArgs args)
        {
            DiagramThumbDragStartedEventHandler handler = DragStarted;
            if (handler != null) handler(_mSource, args);
            
        }

        public event DiagramThumbDragStartedEventHandler DragStarted;
    }
}
