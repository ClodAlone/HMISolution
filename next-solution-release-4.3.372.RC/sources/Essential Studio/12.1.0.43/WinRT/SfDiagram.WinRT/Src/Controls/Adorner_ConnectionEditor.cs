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
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Windows.Input;
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Controller;

namespace Syncfusion.UI.Xaml.Diagram.Controls
{
    internal sealed partial class Adorner : Panel, ISharedData
    {
        #region ConnectionEditor

        DiagramThumb _mSourceThumb;
        DiagramThumb _mTargetThumb;
        private Stack<DiagramThumb> _mConnectorThums;
        private Stack<Line> _mConnectorLines;
        private Stack<SegmentWrapper> _mTerminalSegments;
        private IInternalConnector _mSelectedConnector;
        private ContentPresenter _mConnectionIndicator = null;
        RunTimeConnectionIndicator _animatedConnectionIndicator = null;
        TranslateTransform _mConnectionIndicatorTransform;
        TranslateTransform _animatedIndicatorTransform;
        //IInternalSelector _mWrapper;

        public T GetEditor<T>(SegmentWrapper wrapper = null)
        {
            Type type = typeof(T);
            if (typeof(DiagramThumb) == type)
            {
                DiagramThumb thumb;
                if (_mConnectorThums.Any())
                {
                    thumb = _mConnectorThums.Pop();
                    thumb.Visibility = Visibility.Visible;
                }
                else
                {
                    thumb = new DiagramThumb();
                    thumb.RenderTransform = new TranslateTransform();
                }
                if (wrapper is OrthoWrapper)
                {
                    thumb.Style = DiagramThumb.SegmentStyleDictionary["OrthogonalThumb"] as Style;
                }
                else
                {
                    if ((wrapper is CBezierWrapper || wrapper is QBezierWrapper) &&
                        wrapper.ConnectorWrapper.SelectedSegment == null)
                    {
                        thumb.Style = DiagramThumb.SegmentStyleDictionary["EndThumb"] as Style;
                    }
                    else
                    {
                        thumb.Style = DiagramThumb.SegmentStyleDictionary["BezierThumb"] as Style;
                    }
                }
                Children.Add(thumb);
                return (T)(object)thumb;
            }
            else if (typeof(Line) == type)
            {
                Line line;
                if (_mConnectorLines.Any())
                {
                    line = _mConnectorLines.Pop();
                }
                else
                {
                    line = new Line();
#if WINRT
                    line.Stroke = new SolidColorBrush(new Color() { A = 255, B = 54, G = 109, R = 17 });
#else
                    line.Stroke = new SolidColorBrush(new Color() { A = 255, B = 54, G = 109, R = 17 }); 
#endif
                    line.StrokeDashArray = new DoubleCollection() { 3, 3 };
                    line.StrokeThickness = 1;
                    //line.RenderTransform = new TranslateTransform();
                }
                this.Children.Add(line);
                return (T)(object)line;
            }
            else if (typeof(OrthoWrapper) == type)
            {
                OrthoWrapper segment;
                if (_mTerminalSegments.Any())
                {
                    segment = _mTerminalSegments.Pop() as OrthoWrapper;
                }
                else
                {
                    segment = new OrthoWrapper(null, null, _mSharedData);
                    //segment.PrepareThumb();
                }
                return (T)(object)segment;
            }
            else if (typeof(LineWrapper) == type)
            {
                LineWrapper segment;
                if (_mTerminalSegments.Any())
                {
                    segment = _mTerminalSegments.Pop() as LineWrapper;
                }
                else
                {
                    segment = new LineWrapper(null, null, _mSharedData);
                }
                return (T)(object)segment;
            }
            else if (typeof(LineLengthWrapper) == type)
            {
                LineLengthWrapper segment;
                if (_mTerminalSegments.Any())
                {
                    segment = _mTerminalSegments.Pop() as LineLengthWrapper;
                }
                else
                {
                    segment = new LineLengthWrapper(null, null, _mSharedData);
                }
                return (T)(object)segment;
            }
            else if (typeof(QBezierWrapper) == type)
            {
                QBezierWrapper segment;
                if (_mTerminalSegments.Any())
                {
                    segment = _mTerminalSegments.Pop() as QBezierWrapper;
                }
                else
                {
                    segment = new QBezierWrapper(null, null, _mSharedData);
                }
                return (T)(object)segment;
            }
            else if (typeof(CBezierWrapper) == type)
            {
                CBezierWrapper segment;
                if (_mTerminalSegments.Any())
                {
                    segment = _mTerminalSegments.Pop() as CBezierWrapper;
                }
                else
                {
                    segment = new CBezierWrapper(null, null, _mSharedData);
                }
                return (T)(object)segment;
            }
            throw new InvalidOperationException("Only Thumb, Line, Wrapper is supported");
        }

        public void Recycle<T>(T element)
        {
            Type type = typeof(T);
            if (typeof(DiagramThumb) == type)
            {
                DiagramThumb thumb = element as DiagramThumb;
                thumb.Width = Double.NaN;
                thumb.Height = Double.NaN;
                Children.Remove(thumb);
                _mConnectorThums.Push(thumb);
                return;
            }
            else if (typeof(Line) == type)
            {
                Children.Remove(element as Line);
                _mConnectorLines.Push(element as Line);
                return;
            }
            else if (typeof(SegmentWrapper) == type)
            {
                _mTerminalSegments.Push(element as SegmentWrapper);
                return;
            }
            throw new InvalidOperationException("Only Thumb, Line is supported");
        }

        private void UpdateEndPointion()
        {
            _mSourceThumb.OffsetX = _mSelectedConnector.SourcePoint.X;
            _mSourceThumb.OffsetY = _mSelectedConnector.SourcePoint.Y;
            _mTargetThumb.OffsetX = _mSelectedConnector.TargetPoint.X;
            _mTargetThumb.OffsetY = _mSelectedConnector.TargetPoint.Y;
        }

        internal void SelectConnector(IInternalConnector connector)
        {
            if (_mSelectedConnector != connector)
            {
                _mTerminalSegments = new Stack<SegmentWrapper>();
                if (_mSelectedConnector != null)
                {
                    _mSelectedConnector.DisposeThums();
                    _mSelectedConnector = null;
                }
                if (connector == null)
                {
                    _mSourceThumb.Visibility = Visibility.Collapsed;
                    _mTargetThumb.Visibility = Visibility.Collapsed;
                }
                if (connector != null)
                {
                    _mSelectedConnector = connector;
                    _mSelectedConnector.PrepareThums();
                    if (_mSelectedConnector.Constraints.Contains(ConnectorConstraints.EndThumbs))
                    {
                        _mSourceThumb.Visibility = Visibility.Visible;
                        _mTargetThumb.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _mSourceThumb.Visibility = Visibility.Collapsed;
                        _mTargetThumb.Visibility = Visibility.Collapsed;
                    }
                    //InvalidateArrange();
                }
            }
        }

        internal void ManualPressTargetThumb(MouseEventArgs e)
        {
            _mTargetThumb.Manual_PointerPressed(e);
        }

        private void _mEndThumb_DragStarting(object sender, DiagramThumbDragStartingEventArgs args)
        {
            if (_mSharedData.UndoRedoController != null) _mSharedData.UndoRedoController.BeginComposite();
            ConnectorConstraints constraints = _mSelectedConnector.Constraints;

            if (sender == _mSourceThumb && constraints.Contains(ConnectorConstraints.SourceDraggable))
            {
                args.InitialValue = _mSelectedConnector.SourcePoint;
                _mSelectedConnector.SourceDragState = DragState.Starting;
            }
            else if (sender == _mTargetThumb && constraints.Contains(ConnectorConstraints.TargetDraggable))
            {
                args.InitialValue = _mSelectedConnector.TargetPoint;
                _mSelectedConnector.TargetDragState = DragState.Starting;
            }
            else
            {
                args.Cancel = true;
            }
            _animatedConnectionIndicator.DragOverPort = null;
            _animatedConnectionIndicator.DragOverNode = null;
        }

        private void _mEndThumb_DragDelta(object sender, DiagramThumbDragEventArgs args)
        {
            bool isSourceDragging = false;
            bool isTargetDragging = false;
            if (sender == _mSourceThumb)
            {
                isSourceDragging = true;
                if (_mSelectedConnector.SourceDragState == DragState.Starting)
                {
                    _mSelectedConnector.SourceDragState = DragState.Started;
                }
                else if (_mSelectedConnector.SourceDragState == DragState.Started)
                {
                    _mSelectedConnector.SourceDragState = DragState.Dragging;
                }
            }
            else if (sender == _mTargetThumb)
            {
                isTargetDragging = true;
                if (_mSelectedConnector.TargetDragState == DragState.Starting)
                {
                    _mSelectedConnector.TargetDragState = DragState.Started;
                }
                else if (_mSelectedConnector.TargetDragState == DragState.Started)
                {
                    _mSelectedConnector.TargetDragState = DragState.Dragging;
                }
            }

            ScrollViewer page = this.FindVisualParent<ScrollViewer>();
            Point pointFromPage = args.PointerArgs.GetCurrentPoint(null).Position;
            Point pointFromAdorner = args.PointerArgs.GetCurrentPoint(_mSharedData.Adorner).Position;
            IEnumerable<DependencyObject> intersects = page.FindElementsInHostCoordinates(pointFromPage);
            object targetObject = null;
            foreach (var element in intersects)
            {
                if (element is PortBase)
                {
                    if (_animatedConnectionIndicator.DragOverPort == null || element != _animatedConnectionIndicator.DragOverPort)
                    {
                        AddRunTimeConnectionPortAdorner((element as NodePort).Wrapper);
                    }
                    _animatedConnectionIndicator.DragOverPort = element as NodePort;
                    PortConstraints constraints = _animatedConnectionIndicator.DragOverPort.Wrapper.Constraints;
                    NodeConstraints nodeConstraints =
                       _animatedConnectionIndicator.DragOverPort.Wrapper.KnownNode.Constraints;
                    if (isSourceDragging && constraints.Contains(PortConstraints.OutConnect))
                    {
                    }
                    else if (isSourceDragging && constraints.Contains(PortConstraints.Inherit) &&
                             nodeConstraints.Contains(NodeConstraints.OutConnect))
                    {
                    }
                    else if (isTargetDragging && constraints.Contains(PortConstraints.InConnect))
                    {
                    }
                    else if (isTargetDragging && constraints.Contains(PortConstraints.Inherit) &&
                             nodeConstraints.Contains(NodeConstraints.InConnect))
                    {
                    }
                    else
                    {
                        continue;
                    }
                    targetObject = element;
                    break;
                }
                else if (element is Group)
                {
                }
                else if (element is Node)
                {

                    if (_animatedConnectionIndicator.DragOverNode == null || element != _animatedConnectionIndicator.DragOverNode)
                    {
                        AddRunTimeConnectionNodeAdorner((element as Node).Wrapper);
                    }

                    _animatedConnectionIndicator.DragOverNode = element as Node;
                    NodeConstraints nodeConstraints =
                       _animatedConnectionIndicator.DragOverNode.Constraints;
                    if (isSourceDragging && nodeConstraints.Contains(NodeConstraints.OutConnect))
                    {
                    }
                    else if (isTargetDragging && nodeConstraints.Contains(NodeConstraints.InConnect))
                    {
                    }
                    else
                    {
                        continue;
                    }
                    targetObject = element;
                    break;
                }
            }
            if (targetObject == null)
            {
                RemoveConnectionIndicator();
                _animatedConnectionIndicator.DragOverPort = null;
                _animatedConnectionIndicator.DragOverNode = null;
            }
            else if (targetObject is IPort)
            {
                _animatedConnectionIndicator.DragOverNode = null;
            }
            else if (targetObject is INode)
            {
                _animatedConnectionIndicator.DragOverPort = null;
            }
            if (isSourceDragging)
            {

                if (_animatedConnectionIndicator.DragOverPort != null)
                {

                    _mSelectedConnector.KnownSourcePort =
                       _animatedConnectionIndicator.DragOverPort.Wrapper;
                }
                else if (_animatedConnectionIndicator.DragOverNode != null)
                {
                    _mSelectedConnector.KnownSourcePort = null;
                    _mSelectedConnector.KnownSourceNode = _animatedConnectionIndicator.DragOverNode.Wrapper;
                }
                else
                {
                    _mSelectedConnector.KnownSourcePort = null;
                    _mSelectedConnector.KnownSourceNode = null;
                }
                Point pt = new Point(args.StartingArgs.InitialValue.Value.X + args.Cumulative.HorizontalChange,
                                                   args.StartingArgs.InitialValue.Value.Y + args.Cumulative.VerticalChange);

                SnapToGridlines(ref pt);
                _mSelectedConnector.SourcePoint = pt;
            }
            else if (isTargetDragging)
            {

                if (_animatedConnectionIndicator.DragOverPort != null)
                {
                    _mSelectedConnector.KnownTargetPort =
                       _animatedConnectionIndicator.DragOverPort.Wrapper;
                }
                else if (_animatedConnectionIndicator.DragOverNode != null)
                {
                    _mSelectedConnector.KnownTargetPort = null;
                    _mSelectedConnector.KnownTargetNode = _animatedConnectionIndicator.DragOverNode.Wrapper;
                }
                else
                {
                    _mSelectedConnector.KnownTargetPort = null;
                    _mSelectedConnector.KnownTargetNode = null;
                }

                Point pt = new Point(args.StartingArgs.InitialValue.Value.X + args.Cumulative.HorizontalChange,
                                                    args.StartingArgs.InitialValue.Value.Y + args.Cumulative.VerticalChange);
                SnapToGridlines(ref pt);
                _mSelectedConnector.TargetPoint = pt;

            }
            //InvalidateArrange();
        }

        private void SnapToGridlines(ref Point pt)
        {
            if (_mSharedData.SnapSettingsController.CanSnapToVerticalGridlines(_mSelectedConnector.Constraints))
            {
                double roundedX = pt.X.Round(_mSharedData.SnapSettingsController.GetVerticalSnapInterval());
                if (pt.X != roundedX)
                {
                    pt.X = roundedX;
                }
            }
            if (_mSharedData.SnapSettingsController.CanSnapToHorizontalGridlines(_mSelectedConnector.Constraints))
            {
                double roundedY = pt.Y.Round(_mSharedData.SnapSettingsController.GetHorizontalSnapInterval());
                if (pt.Y != roundedY)
                {
                    pt.Y = roundedY;
                }
            }

        }

        private void thumb_DragCompleted(object sender, DiagramThumbDragEventArgs args)
        {
            //_mWrapper.UserInteracting = true;
            RemoveConnectionIndicator();
            if (sender == _mSourceThumb)
            {
                if (_mSelectedConnector.SourceDragState != DragState.None)
                {
                    _mSelectedConnector.SourceDragState = DragState.Completed;
                    if (_mSelectedConnector != null)
                    {
                        _mSelectedConnector.SourceDragState = DragState.None;
                    }
                }
            }
            else if (sender == _mTargetThumb)
            {
                if (_mSelectedConnector.TargetDragState != DragState.None)
                {
                    _mSelectedConnector.TargetDragState = DragState.Completed;
                    if (_mSelectedConnector != null)
                    {
                        _mSelectedConnector.TargetDragState = DragState.None;
                    }
                }
            }
            if (_mSharedData.UndoRedoController != null) _mSharedData.UndoRedoController.EndComposite();
            //_mWrapper.UserInteracting = false;
            //InvalidateArrange();
        }

        private void InitializeConnectionEditor()
        {
            _mConnectorThums = new Stack<DiagramThumb>();
            _mConnectorLines = new Stack<Line>();
            _mTerminalSegments = new Stack<SegmentWrapper>();
            _mSourceThumb = new DiagramThumb() { RenderTransform = new TranslateTransform(), Visibility = Visibility.Collapsed };
            _mTargetThumb = new DiagramThumb() { RenderTransform = new TranslateTransform(), Visibility = Visibility.Collapsed };
            if (_mSourceThumb != null)
            {
                _mSourceThumb.DragStarting += _mEndThumb_DragStarting;
                _mSourceThumb.DragDelta += _mEndThumb_DragDelta;
                _mSourceThumb.DragComplete += thumb_DragCompleted;
            }
            if (_mTargetThumb != null)
            {
                _mTargetThumb.DragStarting += _mEndThumb_DragStarting;
                _mTargetThumb.DragDelta += _mEndThumb_DragDelta;
                _mTargetThumb.DragComplete += thumb_DragCompleted;
            }
            this.Children.Add(_mSourceThumb);
            this.Children.Add(_mTargetThumb);
        }

        #endregion

        # region Connection Indicator

        private void InitializeConnectionIndicator()
        {
            _mConnectionIndicator = new ContentPresenter() { Visibility = Visibility.Collapsed };
            _animatedConnectionIndicator = new RunTimeConnectionIndicator() { Visibility = Visibility.Collapsed };
            _animatedConnectionIndicator.RenderTransform = _animatedIndicatorTransform = new TranslateTransform();
            _mConnectionIndicator.RenderTransform = _mConnectionIndicatorTransform = new TranslateTransform();
            Children.Add(_animatedConnectionIndicator);
            Children.Add(_mConnectionIndicator);
        }

        private void AddRunTimeConnectionPortAdorner(IInternalNodePort internalNodePort)
        {
            if (internalNodePort.View != null)
            {
                AddConnectionIndicator(internalNodePort.OffsetX, internalNodePort.OffsetY,
                    internalNodePort.View.ActualWidth, internalNodePort.View.ActualHeight);
                _mConnectionIndicator.RenderTransformOrigin = (internalNodePort.View as UIElement).RenderTransformOrigin;
                _mConnectionIndicator.ContentTemplate = (_mSharedData.Graph as SfDiagramWrapper).
                    GetConnectionIndicator(internalNodePort.Source);
            }
        }

        private void AddRunTimeConnectionNodeAdorner(IInternalNode node)
        {
            if (node.View != null)
            {
                AddConnectionIndicator(node.OffsetX, node.OffsetY, node.ActualWidth * Scale, node.ActualHeight * Scale);
                _mConnectionIndicator.RenderTransformOrigin = (node.View as UIElement).RenderTransformOrigin;
                _mConnectionIndicator.ContentTemplate = (_mSharedData.Graph as SfDiagramWrapper).GetConnectionIndicator(node.Source);
            }
        }

        private void AddConnectionIndicator(double offsetx, double offsety, double width, double height)
        {
            _mConnectionIndicator.Visibility = Visibility.Visible;
            _animatedConnectionIndicator.Visibility = Visibility.Visible;
            _animatedIndicatorTransform.X = -50 + offsetx * Scale;
            _animatedIndicatorTransform.Y = -50 + offsety * Scale;
            _mConnectionIndicator.Width = width;
            _mConnectionIndicator.Height = height;
            _mConnectionIndicatorTransform.X = -_mConnectionIndicator.Width / 2 + offsetx * Scale;
            _mConnectionIndicatorTransform.Y = -_mConnectionIndicator.Height / 2 + offsety * Scale;

        }

        private void RemoveConnectionIndicator()
        {
            _animatedConnectionIndicator.Visibility = Visibility.Collapsed;
            _mConnectionIndicator.Visibility = Visibility.Collapsed;
        }

        #endregion
    }

}
