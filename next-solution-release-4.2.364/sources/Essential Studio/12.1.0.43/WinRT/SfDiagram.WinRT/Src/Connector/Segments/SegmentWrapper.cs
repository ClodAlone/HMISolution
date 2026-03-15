#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    internal abstract class SegmentWrapper : WrapperBase, IInternalSegment, IEnumerable<IInternalSegment>
    {
        protected SegmentWrapper(IConnectorSegment source, ConnectorWrapper connector, SharedData shared)
            : base(shared)
        {
            Source = source;
            ConnectorWrapper = connector;
        }

        protected override void SharedDataInitialized()
        {
        }

        protected override void SourceChanged()
        {
        }

        protected override void OnPropertyChanged(string propertyName)
        {
        }

        public ConnectorWrapper ConnectorWrapper { get; set; }

        public IInternalSegment Prev { get; set; }
        public IInternalSegment Next { get; set; }
        public BezierSmoothness BezierSmoothness { get; set; }
        public SegmentConstraints Constraints { get; set; }
        public bool IsTerminal { get; set; }
        private Point? _endPoint;

        public Point? EndPoint
        {
            get { return _endPoint; }
            set
            {
                if (value != _endPoint && _endPoint != null)
                {
                    Point? old = _endPoint;
                    _endPoint = value;
                    if (SegmentChangedCallback != null)
                    {
                        if (ConnectorWrapper.IsSelected || ConnectorWrapper.TargetNode == null)
                        {
                            SegmentChangedCallback(new PropChangedEventArgs<IInternalSegment>(this, old, _endPoint, "Point3"));
                        }
                    }
                    else if (Next != null && Next.SegmentChangedCallback != null)
                    {
                        Next.SegmentChangedCallback(new PropChangedEventArgs<IInternalSegment>(this, old, _endPoint, "Source"));
                    }
                }
                else
                    _endPoint = value;
            }
        }

        public IEnumerator<IInternalSegment> GetEnumerator()
        {
            IInternalSegment current = this;
            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        protected Point GetSourcePoint(Point? adjPoint, out double angle, SegmentWrapper first = null)
        {
            angle = 0;
            if (Source is IQuadraticCurveSegment || Source is ICubicCurveSegment || Source is IOrthogonalSegment)
            {
                Point end;
                OrthogonalDirection dir = GetBezierDirection(ConnectorWrapper.KnownSourcePort, ConnectorWrapper.KnownSourceNode, ConnectorWrapper.SourcePoint,
                               ConnectorWrapper.KnownTargetPort, ConnectorWrapper.KnownTargetNode, ConnectorWrapper.TargetPoint, out end, first);
                angle = dir.ToAngle();
                return end;
            }
            else if (ConnectorWrapper.KnownSourcePort != null)
            {
                return new Point(ConnectorWrapper.KnownSourcePort.OffsetX, ConnectorWrapper.KnownSourcePort.OffsetY);
            }
            else if (ConnectorWrapper.KnownSourceNode != null)
            {
                if (adjPoint != null)
                {
                    return ConnectorWrapper.KnownSourceNode.GetIntersection(adjPoint.Value);
                }
                else if (ConnectorWrapper.KnownTargetPort != null)
                {
                    return ConnectorWrapper.KnownSourceNode.GetIntersection(
                        new Point(ConnectorWrapper.KnownTargetPort.OffsetX, ConnectorWrapper.KnownTargetPort.OffsetY));
                }
                else if (ConnectorWrapper.KnownTargetNode != null)
                {
                    return ConnectorWrapper.KnownSourceNode.GetIntersection(ConnectorWrapper.KnownTargetNode);
                }
                else
                {
                    return ConnectorWrapper.KnownSourceNode.GetIntersection(ConnectorWrapper.TargetPoint);
                }
            }
            else
            {
                return ConnectorWrapper.SourcePoint;
            }
        }

        public abstract Point GetSourcePoint(out double angle);
        public abstract bool AddView();
        public abstract void UpdateAdjacentSegments(PropChangedEventArgs<IInternalSegment> propChangedEventArgs);
        public abstract void PrepareThumb(bool isLast = false);
        public abstract void UpdateThumb();
        public abstract void DisposeThumb();

        protected Point GetBezierAdjPoint(
            double angle,
            Point srcEnd,
            Point tarEnd)
        {
            double distance = 60;
            Point endAdj = new Point(0, 0);
            OrthogonalDirection dir;
            if (angle > 45 && angle < 135)
            {
                dir = OrthogonalDirection.Bottom;
            }
            else if (angle > 135 && angle < 225)
            {
                dir = OrthogonalDirection.Left;
            }
            else if (angle > 225 && angle < 315)
            {
                dir = OrthogonalDirection.Top;
            }
            else
            {
                dir = OrthogonalDirection.Right;
            }

            //distance = Math.Min(srcEnd.FindLength(tarEnd) * 0.3, distance);
            //endAdj = srcEnd.Transform(distance, dir);
            switch (dir)
            {
                case OrthogonalDirection.Right:
                    distance = Math.Min(Math.Abs(srcEnd.X - tarEnd.X) * 0.45, distance);
                    endAdj = new Point(srcEnd.X + distance, srcEnd.Y);
                    break;
                case OrthogonalDirection.Bottom:
                    distance = Math.Min(Math.Abs(srcEnd.Y - tarEnd.Y) * 0.45, distance);
                    endAdj = new Point(srcEnd.X, srcEnd.Y + distance);
                    break;
                case OrthogonalDirection.Left:
                    distance = Math.Min(Math.Abs(srcEnd.X - tarEnd.X) * 0.45, distance);
                    endAdj = new Point(srcEnd.X - distance, srcEnd.Y);
                    break;
                case OrthogonalDirection.Top:
                    distance = Math.Min(Math.Abs(srcEnd.Y - tarEnd.Y) * 0.45, distance);
                    endAdj = new Point(srcEnd.X, srcEnd.Y - distance);
                    break;
            }
            return endAdj;
        }

        protected OrthogonalDirection GetBezierDirection(
            IInternalNodePort sourcePort,
            IInternalNode sourceNode,
            Point sourcePoint,
            IInternalNodePort targetPort,
            IInternalNode targetNode,
            Point targetPoint,
            out Point end,
            SegmentWrapper first = null)
        {
            OrthogonalDirection? dir;
            if (ConnectorWrapper.RunPoint != null)
            {
            }
            if (sourcePort != null)
            {
                dir = sourcePort.GetDirection();
            }
            else if (sourceNode != null)
            {
                if ((first != null) && (first as OrthoWrapper).Length.IsValid())
                {
                    dir = (first as OrthoWrapper).Direction;
                }
                else
                {
                    if (targetNode != null)
                    {
                        Point inter = sourceNode.GetIntersection(targetNode);
                        dir = sourceNode.GetDirection(inter);
                    }
                    else
                    {
                        dir = sourceNode.GetDirection(targetPoint);
                    }
                }
            }
            else
            {
                Point src = sourcePoint, tar = targetPoint;
                if (targetPort != null)
                {
                    tar = new Point(targetPort.OffsetX, targetPort.OffsetY);
                }
                else if (targetNode != null)
                {
                    tar = targetNode.GetIntersection(sourcePoint);
                }

                if (Math.Abs(tar.X - src.X) > Math.Abs(tar.Y - src.Y))
                {
                    dir = src.X < tar.X ? OrthogonalDirection.Right : OrthogonalDirection.Left;
                }
                else
                {
                    dir = src.Y < tar.Y ? OrthogonalDirection.Bottom : OrthogonalDirection.Top;
                }
            }

            if (sourcePort != null)
            {
                end = new Point(sourcePort.OffsetX, sourcePort.OffsetY);
            }
            else if (sourceNode != null)
            {
                switch (dir)
                {
                    case OrthogonalDirection.Right:
                        end = sourceNode.Corners.Value.Right;
                        break;
                    case OrthogonalDirection.Bottom:
                        end = sourceNode.Corners.Value.Bottom;
                        break;
                    case OrthogonalDirection.Left:
                        end = sourceNode.Corners.Value.Left;
                        break;
                    case OrthogonalDirection.Top:
                        end = sourceNode.Corners.Value.Top;
                        break;
                    default:
                        end = new Point(0, 0);
                        break;
                }
            }
            else
            {
                end = sourcePoint;
            }
            return dir.Value;
        }

        protected void SetStartPoint(Point start)
        {
            ConnectorWrapper.PathFigure.StartPoint = start;
        }

        protected Point TerminateLine()
        {
            Point tail = new Point(0, 0);
            if (ConnectorWrapper.KnownTargetPort != null)
            {
                tail = new Point(ConnectorWrapper.KnownTargetPort.OffsetX, ConnectorWrapper.KnownTargetPort.OffsetY);
            }
            else if (ConnectorWrapper.KnownTargetNode != null)
            {
                tail = ConnectorWrapper.KnownTargetNode.GetIntersection(ConnectorWrapper.RunPoint.Value);
            }
            else
            {
                tail = ConnectorWrapper.TargetPoint;
            }
            return tail;
        }
        protected Point TerminateLine(double radius, double angle)
        {
            Point segmentEnd = ConnectorWrapper.RunPoint.Value.Transform(radius, angle);
            ConnectorWrapper.RunAngle = angle;// runPoint.Value.FindAngle(segmentEnd);
            ConnectorWrapper.RunPoint = segmentEnd;
            //ConnectorWrapper.InternalSegments.Add(new LineSegment() { Point = segmentEnd });
            return segmentEnd;
        }

        protected void AddLineSegment(Point point)
        {
            ConnectorWrapper.RunAngle = ConnectorWrapper.RunPoint.Value.FindAngle(point);
            if (IsTerminal)
            {
                double length = ConnectorWrapper.RunPoint.Value.FindLength(point);
                OrthoWrapper _wrapper = GetThumb<OrthoWrapper>();
                (_wrapper as SegmentWrapper).ConnectorWrapper = ConnectorWrapper;
                _wrapper.Length = length;
                _wrapper.Direction = ConnectorWrapper.RunPoint.Value.ToDirection(point);
                _wrapper.EndPoint = point;
                _wrapper.SetSource(null);
                _wrapper._mPrev = ConnectorWrapper.TerminalSegment.LastOrDefault();
                if (ConnectorWrapper.TerminalSegment.Count() < 1)
                {
                    _wrapper._mPrev = (this as OrthoWrapper)._mPrev;
                }
                if (_wrapper._mPrev != null)
                {
                    (_wrapper._mPrev as OrthoWrapper)._mNext = _wrapper;
                }
                _wrapper._mNext = null;
                (ConnectorWrapper.TerminalSegment as IList<SegmentWrapper>).Add(_wrapper);
                _wrapper.PrepareThumb();
            }
            ConnectorWrapper.RunPoint = point;
            ConnectorWrapper.InternalSegments.Add(new LineSegment() { Point = point });
        }

        protected void AddLineSegment(double radius, double angle)
        {
            Point segmentEnd = ConnectorWrapper.RunPoint.Value.Transform(radius, angle);
            if (IsTerminal)
            {
                double length = ConnectorWrapper.RunPoint.Value.FindLength(segmentEnd);
                OrthoWrapper _wrapper = GetThumb<OrthoWrapper>();
                (_wrapper as SegmentWrapper).ConnectorWrapper = ConnectorWrapper;
                _wrapper.Length = length;
                _wrapper.Direction = ConnectorWrapper.RunPoint.Value.ToDirection(segmentEnd);
                _wrapper.EndPoint = segmentEnd;
                _wrapper.SetSource(null);
                _wrapper._mPrev = ConnectorWrapper.TerminalSegment.LastOrDefault();
                if (ConnectorWrapper.TerminalSegment.Count() < 1)
                {
                    _wrapper._mPrev = (this as OrthoWrapper)._mPrev;
                }
                if (_wrapper._mPrev != null)
                {
                    (_wrapper._mPrev as OrthoWrapper)._mNext = _wrapper;
                }
                _wrapper._mNext = null;
                (ConnectorWrapper.TerminalSegment as IList<SegmentWrapper>).Add(_wrapper);
                _wrapper.PrepareThumb();
            }
            ConnectorWrapper.RunAngle = angle; // runPoint.Value.FindAngle(segmentEnd);
            ConnectorWrapper.RunPoint = segmentEnd;
            ConnectorWrapper.InternalSegments.Add(new LineSegment() { Point = segmentEnd });
        }

        protected T GetThumb<T>(SegmentWrapper wrapper = null)
        {
            return SharedData.Adorner.GetEditor<T>(wrapper);
        }
        internal void Recycle<T>(T element)
        {
            SharedData.Adorner.Recycle(element);
        }
        protected void Transform(DiagramThumb thumb, double x, double y)
        {
            thumb.OffsetX = x;
            thumb.OffsetY = y;
        }
        protected void Invalidate()
        {
            UIElement element = ConnectorWrapper.View as UIElement;
            if (element != null)
            {
                element.InvalidateArrange();
            }
            SharedData.Adorner.InvalidateArrange();
        }

        public Action<PropChangedEventArgs<IInternalSegment>> SegmentChangedCallback
        {
            get;
            set;
        }

    }

    internal class LineWrapper : SegmentWrapper, ILineInternal
    {
        private Point? _point;
        private readonly ILineSegment _source;
        private DiagramThumb _thumb;

        public LineWrapper(ILineSegment source, ConnectorWrapper connector, SharedData shared) :
            base(source, connector, shared)
        {
            _source = source;
        }

        public Point? Point
        {
            get { return _source.Point ?? _point; }
            set
            {
                if (_source.Point != null)
                {
                    _source.Point = value;
                }
                else if (value != null)
                {
                    _point = value.Value;
                }
            }
        }

        public override Point GetSourcePoint(out double angle)
        {
            Point? pt = (Source as ILineSegment).Point;
            return GetSourcePoint(pt, out angle);
        }

        private void Update()
        {
            Point = TerminateLine();
        }

        public override bool AddView()
        {
            Point? pt = (Source as ILineSegment).Point;
            if (pt != null)
            {
                AddLineSegment(Point.Value);
                EndPoint = Point;
                return true;
            }
            else
            {
                Update();
                AddLineSegment(Point.Value);
                EndPoint = Point;
                return false;
            }
        }

        public override void PrepareThumb(bool isLast = false)
        {
            if (_thumb == null && this.Next != null)
            {
                _thumb = GetThumb<DiagramThumb>();
                _thumb.DragStarting += _thumb_DragStarting;
                _thumb.DragDelta += _thumb_DragDelta;
                _thumb.DragComplete += _thumb_DragDelta;
            }
        }

        void _thumb_DragStarting(object sender, DiagramThumbDragStartingEventArgs args)
        {
            if (ConnectorWrapper.SelectedSegment != null && ConnectorWrapper.SelectedSegment != this)
            {
                (ConnectorWrapper.SelectedSegment as SegmentWrapper).DisposeThumb();
                if (ConnectorWrapper.SelectedSegment.Next != null)
                {
                    (ConnectorWrapper.SelectedSegment.Next as SegmentWrapper).DisposeThumb();
                }
            }
            ConnectorWrapper.SelectedSegment = this;
            this.PrepareThumb();
            if (Next != null)
            {
                (Next as SegmentWrapper).PrepareThumb();
            }
        }

        public override void UpdateThumb()
        {
            if (_thumb != null)
            {
                Transform(_thumb, Point.Value.X, Point.Value.Y);
            }
        }
        public override void DisposeThumb()
        {
            if (_thumb != null && ConnectorWrapper.SelectedSegment == null)
            {
                _thumb.DragDelta -= _thumb_DragDelta;
                _thumb.DragComplete -= _thumb_DragDelta;
                _thumb.DragStarting -= _thumb_DragStarting;
                Recycle(_thumb);
                _thumb = null;
            }
        }

        void _thumb_DragDelta(object sender, DiagramThumbDragEventArgs args)
        {
            Point = new Point(Point.Value.X + args.Delta.HorizontalChange, Point.Value.Y + args.Delta.VerticalChange);
            Invalidate();
        }

        public override void UpdateAdjacentSegments(PropChangedEventArgs<IInternalSegment> propChangedEventArgs)
        {
            //throw new NotImplementedException();
        }
    }

    internal class LineLengthWrapper : SegmentWrapper, ILineLengthInternal
    {
        private readonly ILineSegmentLength _source;
        private DoubleExt _length;
        private DoubleExt _angle;
        private DiagramThumb _thumb;

        public LineLengthWrapper(ILineSegmentLength source, ConnectorWrapper connector, SharedData shared) :
            base(source, connector, shared)
        {
            _source = source;
        }

        public DoubleExt Length
        {
            get { return _source.Length.IsValid() ? _source.Length : _length; }
            set
            {
                if (_source.Length.IsValid())
                {
                    _source.Length = value;
                }
                else if (value.IsValid())
                {
                    _length = value;
                }
            }
        }

        public DoubleExt Angle
        {
            get { return _source.Angle.IsValid() ? _source.Angle : _angle; }
            set
            {
                if (_source.Angle.IsValid())
                {
                    _source.Angle = value;
                }
                else if (value.IsValid())
                {
                    _angle = value;
                }
            }
        }

        public RelativeMode AngleMode
        {
            get { return _source.AngleMode; }
            set { _source.AngleMode = value; }
        }

        public override Point GetSourcePoint(out double angle)
        {
            Point? pt = null;
            if (ConnectorWrapper.KnownSourceNode != null)
            {
                if ((Source as ILineSegmentLength).Length.IsValid() && (Source as ILineSegmentLength).Angle.IsValid())
                {
                    Point start = new Point(ConnectorWrapper.KnownSourceNode.OffsetX,
                                            ConnectorWrapper.KnownSourceNode.OffsetY);
                    pt = start.Transform(
                        ConnectorWrapper.KnownSourceNode.ActualWidth + ConnectorWrapper.KnownSourceNode.ActualHeight,
                        (Source as ILineSegmentLength).Angle.ActualValue);
                }
            }
            return GetSourcePoint(pt, out angle);
        }

        private void Update()
        {
            Point pt = TerminateLine();
            Length = ConnectorWrapper.RunPoint.Value.FindLength(pt);
            Angle = ConnectorWrapper.RunPoint.Value.FindAngle(pt);
        }

        public override bool AddView()
        {
            if (_source.Angle.Value.IsValid() && _source.Length.Value.IsValid())
            {
                double angle = _source.Angle.ActualValue;
                if (_source.AngleMode == RelativeMode.Relative)
                {
                    angle += ConnectorWrapper.RunAngle;
                }
                double radius = _source.Length.ActualValue;
                AddLineSegment(radius, angle);
                EndPoint = ConnectorWrapper.RunPoint;
                return true;
            }
            else
            {
                Update();
                double angle = Angle.ActualValue;
                if (AngleMode == RelativeMode.Relative)
                {
                    angle += ConnectorWrapper.RunAngle;
                }
                double radius = Length.ActualValue;
                AddLineSegment(radius, angle);
                EndPoint = ConnectorWrapper.RunPoint;
                return false;
            }
        }

        public override void PrepareThumb(bool isLast = false)
        {
            if (_thumb == null && Next != null)
            {
                _thumb = GetThumb<DiagramThumb>();
                _thumb.DragDelta += _thumb_DragDelta;
                _thumb.DragComplete += _thumb_DragDelta;
                _thumb.DragStarting += _thumb_DragStarting;
            }
        }

        void _thumb_DragStarting(object sender, DiagramThumbDragStartingEventArgs args)
        {
            (ConnectorWrapper.SelectedSegment as SegmentWrapper).DisposeThumb();
            if (ConnectorWrapper.SelectedSegment.Next != null)
            {
                (ConnectorWrapper.SelectedSegment.Next as SegmentWrapper).DisposeThumb();
            }
            ConnectorWrapper.SelectedSegment = this;
            this.PrepareThumb();
            if (Next != null)
            {
                (Next as SegmentWrapper).PrepareThumb();
            }
        }

        public override void UpdateThumb()
        {
            Point prev;
            if (Prev != null)
            {
                prev = Prev.EndPoint.Value;
            }
            else
            {
                prev = ConnectorWrapper.PathFigure.StartPoint;
            }
            Point pt = prev.Transform(Length.Value, Angle.Value);
            if (_thumb != null)
            {
                Transform(_thumb, pt.X, pt.Y);
            }
        }

        public override void DisposeThumb()
        {
            if (_thumb != null && ConnectorWrapper.SelectedSegment == null)
            {
                _thumb.DragStarting -= _thumb_DragStarting;
                _thumb.DragDelta -= _thumb_DragDelta;
                _thumb.DragComplete -= _thumb_DragDelta;
                Recycle(_thumb);
                _thumb = null;
            }
        }


        void _thumb_DragDelta(object sender, DiagramThumbDragEventArgs args)
        {
            Point prev;
            if (Prev != null)
            {
                prev = Prev.EndPoint.Value;
            }
            else
            {
                prev = ConnectorWrapper.PathFigure.StartPoint;
            }
            Point pt = prev.Transform(Length.Value, Angle.Value);
            pt = new Point(pt.X + args.Delta.HorizontalChange, pt.Y + args.Delta.VerticalChange);
            Length = prev.FindLength(pt);
            Angle = prev.FindAngle(pt);
            Invalidate();
        }

        public override void UpdateAdjacentSegments(PropChangedEventArgs<IInternalSegment> propChangedEventArgs)
        {
            //throw new NotImplementedException();
        }
    }

    internal class OrthoWrapper : SegmentWrapper, IOrthoInternal
    {
        public OrthoWrapper(IOrthogonalSegment source, ConnectorWrapper connector, SharedData shared) :
            base(source, connector, shared)
        {
            _source = source;
        }

        private IOrthogonalSegment _source;
        private DiagramThumb _thumb, _thumb1 = null;
        private DoubleExt _angle;
        private DoubleExt _length;

        public Point? StartPoint
        {
            get
            {
                return _mPrev != null && _mPrev.EndPoint != null ? _mPrev.EndPoint : ConnectorWrapper.PathFigure.StartPoint;
            }
        }

        private IInternalSegment _next;

        public IInternalSegment _mNext
        {
            get { return _source == null ? _next : Next; }
            set
            {
                if (_source == null)
                {
                    _next = value;
                }
            }
        }

        private IInternalSegment _prev;

        public IInternalSegment _mPrev
        {
            get { return _source == null ? _prev : Prev; }
            set
            {
                if (_source == null)
                {
                    _prev = value;
                }
            }
        }


        public DoubleExt Length
        {
            get
            {
                return _source != null ? _source.Length : _length;
            }
            set
            {
                _length = value;
                if (_source != null && _source.Length.IsValid())
                {
                    _source.Length = value;
                }
            }
        }

        private OrthogonalDirection _direction;

        public OrthogonalDirection Direction
        {
            get
            {
                return _source != null ? _source.Direction : _direction;
            }
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                }
                if (_source != null)
                {
                    if (_source.Direction != value)
                    {
                        _source.Direction = value;
                    }
                }
            }
        }

        public void SetSource(IOrthogonalSegment source)
        {
            Source = source;
            _source = source;
        }

        public override Point GetSourcePoint(out double angle)
        {
            Point? pt = null;
            if (ConnectorWrapper.KnownSourceNode != null)
            {
                Point start = new Point(ConnectorWrapper.KnownSourceNode.OffsetX,
                                        ConnectorWrapper.KnownSourceNode.OffsetY);
                double a = (Source as IOrthogonalSegment).Direction.ToAngle();
                if (a.IsValid())
                {
                    pt = start.Transform(
                            ConnectorWrapper.KnownSourceNode.ActualWidth + ConnectorWrapper.KnownSourceNode.ActualHeight, a);
                }
            }
            return GetSourcePoint(null, out angle, this as SegmentWrapper);
        }

        public override bool AddView()
        {
            IOrthogonalSegment orthoSegment = Source as IOrthogonalSegment;
            if (orthoSegment.Length.IsValid())
            {
                IsTerminal = false;
                double radius = orthoSegment.Length.ActualValue;
                _angle = orthoSegment.Direction.ToAngle(ConnectorWrapper.RunAngle);
                if (!_angle.IsValid())
                {
                    //throw new NotImplementedException();
                }
                AddLineSegment(radius, _angle.ActualValue);
                EndPoint = ConnectorWrapper.RunPoint;
                return true;
            }
            else
            {
                IsTerminal = true;
                if (this.Prev == null)
                {
                    ConnectorWrapper.RunPoint = null;
                }
                Terminate();
                EndPoint = ConnectorWrapper.RunPoint;
                return false;
            }
        }

        private void Terminate()
        {
            if (Prev == null)
            {
                TerminateOrthoConnection(ConnectorWrapper.SourcePoint, ConnectorWrapper.KnownSourceNode,
                                     ConnectorWrapper.KnownSourcePort,
                                     ConnectorWrapper.TargetPoint, ConnectorWrapper.KnownTargetNode,
                                     ConnectorWrapper.KnownTargetPort);
            }
            else
            {
                TerminateOrthoConnection(ConnectorWrapper.RunPoint.Value, null,
                                     null,
                                     ConnectorWrapper.TargetPoint, ConnectorWrapper.KnownTargetNode,
                                     ConnectorWrapper.KnownTargetPort);
            }
        }

        private void TerminateOrthoConnection(
            Point sourcePoint,
            IInternalNode knownSourceNode,
            IInternalNodePort sourcePort,
            Point targetPoint,
            IInternalNode knownTargetNode,
            IInternalNodePort targetPort)
        {
            OrthogonalDirection? srcDirection = null;
            OrthogonalDirection? tarDirection = null;

            if (sourcePort != null)
            {
                srcDirection = sourcePort.GetDirection();
            }

            if (targetPort != null)
            {
                tarDirection = targetPort.GetDirection();
            }

            if (knownSourceNode != null && knownSourceNode.Corners == null)
            {
                knownSourceNode.UpdateBoundsCorners();
            }
            if (knownTargetNode != null && knownTargetNode.Corners == null)
            {
                knownTargetNode.UpdateBoundsCorners();
            }

            Thickness sourceMargin = new Thickness(10, 10, 10, 10);
            Thickness targetMargin = new Thickness(10, 10, 10, 10);

            #region Both ends connected
            if (knownSourceNode != null && knownTargetNode != null)
            {
                if (srcDirection == null || tarDirection == null)
                {
                    RectCorners sourceCorner = knownSourceNode.Corners.Value;
                    RectCorners targetCorner = knownTargetNode.Corners.Value;
                    // Top -> Bottom
                    if (sourceCorner.Top.Y > targetCorner.Bottom.Y &&
                             Math.Abs(sourceCorner.Top.Y - targetCorner.Bottom.Y) >
                             (sourceMargin.Top + targetMargin.Bottom))
                    {
                        srcDirection = srcDirection ?? OrthogonalDirection.Top;
                        tarDirection = tarDirection ?? OrthogonalDirection.Bottom;
                    }
                    // Bottom -> Top
                    else if (sourceCorner.Bottom.Y < targetCorner.Top.Y &&
                        Math.Abs(sourceCorner.Bottom.Y - targetCorner.Top.Y) >
                        (sourceMargin.Bottom + targetMargin.Top))
                    {
                        srcDirection = srcDirection ?? OrthogonalDirection.Bottom;
                        tarDirection = tarDirection ?? OrthogonalDirection.Top;
                    }
                    //Right -> Left
                    else if (sourceCorner.Right.X < targetCorner.Left.X &&
                             Math.Abs(sourceCorner.Right.X - targetCorner.Left.X) >
                             (sourceMargin.Right + targetMargin.Left))
                    {
                        srcDirection = srcDirection ?? OrthogonalDirection.Right;
                        tarDirection = tarDirection ?? OrthogonalDirection.Left;
                    }
                    // Left -> Right
                    else if (sourceCorner.Left.X > targetCorner.Right.X &&
                             Math.Abs(sourceCorner.Left.X - targetCorner.Right.X) >
                             (sourceMargin.Left + targetMargin.Right))
                    {
                        srcDirection = srcDirection ?? OrthogonalDirection.Left;
                        tarDirection = tarDirection ?? OrthogonalDirection.Right;
                    }
                    else
                    {
                        srcDirection = srcDirection ?? OrthogonalDirection.Top;
                        tarDirection = tarDirection ?? OrthogonalDirection.Bottom;
                    }
                }
                DefaultOrthoConnection(knownSourceNode, sourcePort, knownTargetNode, targetPort, srcDirection, tarDirection);
                return;
            }
            #endregion

            #region Atleast one end connected

            if (knownSourceNode == null ^ knownTargetNode == null)
            {
                IInternalNode node = knownSourceNode;
                Point? fixedPoint = sourcePoint;
                Thickness nodeMargin = new Thickness(0, 0, 0, 0);
                if (node == null)
                {
                    node = knownTargetNode;
                    nodeMargin = sourceMargin;
                }
                else
                {
                    fixedPoint = targetPoint;
                }

                //if (fixedPoint == null)
                //{
                //    throw new InvalidOperationException();
                //}

                OrthogonalDirection nodeDirection = OrthogonalDirection.Top;
                Point nodeConnectingPoint = new Point(0, 0);

                RectCorners nodeCorners = node.Corners.Value;
                if (nodeCorners.Bottom.Y + nodeMargin.Bottom < fixedPoint.Value.Y)
                {
                    nodeDirection = OrthogonalDirection.Bottom;
                    nodeConnectingPoint = nodeCorners.Bottom;
                }
                else if (nodeCorners.Top.Y - nodeMargin.Top > fixedPoint.Value.Y)
                {
                    nodeDirection = OrthogonalDirection.Top;
                    nodeConnectingPoint = nodeCorners.Top;
                }
                else if (nodeCorners.Left.X - nodeMargin.Left > fixedPoint.Value.X)
                {
                    nodeDirection = OrthogonalDirection.Left;
                    nodeConnectingPoint = nodeCorners.Left;
                }
                else if (nodeCorners.Right.X + nodeMargin.Right < fixedPoint.Value.X)
                {
                    nodeDirection = OrthogonalDirection.Right;
                    nodeConnectingPoint = nodeCorners.Right;
                }
                else
                {
                    double top = Math.Abs(fixedPoint.Value.Y - nodeCorners.Top.Y);
                    double right = Math.Abs(fixedPoint.Value.X - nodeCorners.Right.X);
                    double bottom = Math.Abs(fixedPoint.Value.Y - nodeCorners.Bottom.Y);
                    double left = Math.Abs(fixedPoint.Value.X - nodeCorners.Left.X);
                    double shortes = double.MaxValue;
                    if (shortes > top)
                    {
                        shortes = top;
                        nodeDirection = OrthogonalDirection.Top;
                        nodeConnectingPoint = nodeCorners.Top;
                    }
                    if (shortes > right)
                    {
                        shortes = right;
                        nodeDirection = OrthogonalDirection.Right;
                        nodeConnectingPoint = nodeCorners.Right;
                    }
                    if (shortes > bottom)
                    {
                        shortes = bottom;
                        nodeDirection = OrthogonalDirection.Bottom;
                        nodeConnectingPoint = nodeCorners.Bottom;
                    }
                    if (shortes > left)
                    {
                        //shortes = left;
                        nodeDirection = OrthogonalDirection.Left;
                        nodeConnectingPoint = nodeCorners.Left;
                    }
                }
                if (node == knownSourceNode)
                {
                    srcDirection = srcDirection ?? nodeDirection;
                    sourcePoint = nodeConnectingPoint;
                    if (sourcePort != null)
                    {
                        sourcePoint = new Point(sourcePort.OffsetX, sourcePort.OffsetY);
                    }
                }
                else
                {
                    tarDirection = tarDirection ?? nodeDirection;
                    targetPoint = nodeConnectingPoint;
                    if (targetPort != null)
                    {
                        targetPoint = new Point(targetPort.OffsetX, targetPort.OffsetY);
                    }
                }
            }
            #endregion
            //else if (sourcePoint != null && targetPoint != null)
            //{

            //}
            //else
            //{
            //    throw new InvalidOperationException();
            //}

            if (ConnectorWrapper.RunPoint == null)
            {
                ConnectorWrapper.RunPoint = sourcePoint;
                SetStartPoint(ConnectorWrapper.RunPoint.Value);
            }

            OrthoConnection3Segment(
                sourcePoint,
                targetPoint,
                sourceMargin,
                targetMargin,
                srcDirection ?? tarDirection ?? OrthogonalDirection.Bottom, tarDirection);
        }

        private void Swap<T>(ref T src, ref T tar)
        {
            T temp = src;
            src = tar;
            tar = temp;
        }

        private void OrthoConnection3Segment(
            Point srcPoint,
            Point tarPoint,
            ref RectCorners srcCorners,
            ref RectCorners tarCorners,
            Thickness srcMargin,
            Thickness tarMargin,
            Orientation srcOrientation,
            OrthogonalDirection srcDirection,
            OrthogonalDirection tarDirection,
            double extra = 20)
        {
            if (srcOrientation == Orientation.Horizontal)
            {
                switch (tarDirection)
                {
                    case OrthogonalDirection.Left:
                        if (srcCorners.Right.X < tarCorners.Left.X)
                        {
                            extra = srcCorners.Right.X - srcPoint.X + 20;
                        }
                        else
                        {
                            if ((srcDirection == OrthogonalDirection.Top && srcPoint.Y > tarPoint.Y) ||
                                (srcDirection == OrthogonalDirection.Bottom && srcPoint.Y < tarPoint.Y))
                            {
                                extra = Math.Min(tarCorners.Left.X, srcPoint.X) - srcPoint.X - 20;
                            }
                            else
                            {
                                extra = Math.Min(tarCorners.Left.X, srcCorners.Left.X) - srcPoint.X - 20;
                            }
                        }
                        break;
                    case OrthogonalDirection.Right:
                        if (srcCorners.Left.X > tarCorners.Right.X)
                        {
                            extra = srcCorners.Left.X - srcPoint.X - 20;
                        }
                        else
                        {
                            if ((srcDirection == OrthogonalDirection.Top && srcPoint.Y > tarPoint.Y) ||
                                (srcDirection == OrthogonalDirection.Bottom && srcPoint.Y < tarPoint.Y))
                            {
                                extra = Math.Max(tarCorners.Right.X, srcPoint.X) - srcPoint.X + 20;
                            }
                            else
                            {
                                extra = Math.Max(tarCorners.Right.X, srcCorners.Right.X) - srcPoint.X + 20;
                            }
                        }
                        break;
                    case OrthogonalDirection.Top:
                    case OrthogonalDirection.Bottom:
                        throw new InvalidOperationException("3 segments cannot be drawn");
                }

                AddLineSegment(extra, 0);
                AddLineSegment(tarPoint.Y - srcPoint.Y, 90);
                AddLineSegment(tarPoint);
            }
            else if (srcOrientation == Orientation.Vertical)
            {
                switch (tarDirection)
                {
                    case OrthogonalDirection.Top:
                        if (srcCorners.Bottom.Y < tarCorners.Top.Y)
                        {
                            extra = srcCorners.Bottom.Y - srcPoint.Y + 20;
                        }
                        else
                        {
                            if ((srcDirection == OrthogonalDirection.Left && srcPoint.X > tarPoint.X) ||
                                (srcDirection == OrthogonalDirection.Right && srcPoint.X < tarPoint.X))
                            {
                                extra = Math.Min(tarCorners.Top.Y, srcPoint.Y) - srcPoint.Y - 20;
                            }
                            else
                            {
                                extra = Math.Min(tarCorners.Top.Y, srcCorners.Top.Y) - srcPoint.Y - 20;
                            }

                        }
                        break;
                    case OrthogonalDirection.Bottom:
                        if (srcCorners.Top.Y > tarCorners.Bottom.Y)
                        {
                            extra = srcCorners.Top.Y - srcPoint.Y - 20;
                        }
                        else
                        {
                            if ((srcDirection == OrthogonalDirection.Left && srcPoint.X > tarPoint.X) ||
                                (srcDirection == OrthogonalDirection.Right && srcPoint.X < tarPoint.X))
                            {
                                extra = Math.Max(tarCorners.Bottom.Y, srcPoint.Y) - srcPoint.Y + 20;
                            }
                            else
                            {
                                extra = Math.Max(tarCorners.Bottom.Y, srcCorners.Bottom.Y) - srcPoint.Y + 20;
                            }
                        }
                        break;
                    case OrthogonalDirection.Left:
                    case OrthogonalDirection.Right:
                        throw new InvalidOperationException("3 segments cannot be drawn");
                }

                AddLineSegment(extra, 90);
                AddLineSegment(tarPoint.X - srcPoint.X, 0);
                AddLineSegment(tarPoint);
            }
        }

        private void OrthoConnection3Segment(
            Point srcPoint,
            Point tarPoint,
            Thickness srcMargin,
            Thickness tarMargin,
            OrthogonalDirection srcDirection,
            OrthogonalDirection? tarDirection,
            double extra = 20)
        {
            if (srcDirection == OrthogonalDirection.Left || srcDirection == OrthogonalDirection.Right)
            {
                double diff = tarPoint.X - srcPoint.X;
                if (diff == 0)
                {
                    ConnectorWrapper.RunPoint = srcPoint;
                    AddLineSegment(tarPoint);
                    return;
                }
                if (srcDirection == OrthogonalDirection.Right)
                {
                    if (tarDirection != null && tarDirection == OrthogonalDirection.Right)
                    {
                        extra = Math.Max(srcPoint.X, tarPoint.X) - srcPoint.X + extra;
                    }
                    if (srcPoint.X > tarPoint.X)
                    {
                        extra = -extra;
                    }
                }
                else if (srcDirection == OrthogonalDirection.Left)
                {
                    if (tarDirection != null && tarDirection == OrthogonalDirection.Left)
                    {
                        extra = srcPoint.X - Math.Min(srcPoint.X, tarPoint.X) + extra;
                    }
                    if (srcPoint.X > tarPoint.X)
                    {
                        extra = -extra;
                    }
                    //if (diff < 20)
                    //{
                    //    firstSegmentLength = -diff/2;
                    //}
                }
                if (Prev == null)
                {
                    AddLineSegment(extra, 0);
                }
                double temp = tarPoint.Y - ConnectorWrapper.RunPoint.Value.Y;
                temp = temp > 0 ? temp : -temp;
                if (temp > 0)
                {
                    AddLineSegment(tarPoint.Y - ConnectorWrapper.RunPoint.Value.Y, 90);
                }
                AddLineSegment(tarPoint);
            }
            else if (srcDirection == OrthogonalDirection.Top || srcDirection == OrthogonalDirection.Bottom)
            {
                if (srcDirection == OrthogonalDirection.Bottom)
                {
                    if (tarDirection != null && tarDirection == OrthogonalDirection.Bottom)
                    {
                        extra = Math.Max(srcPoint.Y, tarPoint.Y) - srcPoint.Y + extra;
                    }
                }
                else if (srcDirection == OrthogonalDirection.Top)
                {
                    if (tarDirection != null && tarDirection == OrthogonalDirection.Top)
                    {
                        extra = srcPoint.Y - Math.Min(srcPoint.Y, tarPoint.Y) + extra;
                    }
                    if (srcPoint.Y > tarPoint.Y)
                    {
                        extra = -extra;
                    }
                }
                if (Prev == null)
                {
                    AddLineSegment(extra, 90);
                }
                double temp = tarPoint.X - ConnectorWrapper.RunPoint.Value.X;
                temp = temp > 0 ? temp : -temp;
                if (temp > 0)
                {
                    AddLineSegment(tarPoint.X - ConnectorWrapper.RunPoint.Value.X, 0);
                }
                AddLineSegment(tarPoint);
            }
        }

        private void OrthoConnection4Segment(
            Point srcPoint,
            Point tarPoint,
            ref RectCorners sourceCorners,
            ref RectCorners targetCorners,
            Thickness srcMargin,
            Thickness tarMargin,
            OrthogonalDirection srcDirection,
            OrthogonalDirection tarDirection)
        {
            double firstSegmentLength = 20;
            switch (srcDirection)
            {
                case OrthogonalDirection.Left:
                    if (sourceCorners.Left.X > targetCorners.Left.X &&
                        sourceCorners.Left.X < targetCorners.Right.X)
                    {
                        if (tarDirection == OrthogonalDirection.Bottom && srcPoint.Y < tarPoint.Y)
                        {
                            firstSegmentLength += sourceCorners.Left.X - targetCorners.Left.X;
                        }
                        else if (tarDirection == OrthogonalDirection.Top && srcPoint.Y > tarPoint.Y)
                        {
                            firstSegmentLength += sourceCorners.Left.X - targetCorners.Left.X;
                        }
                    }
                    firstSegmentLength += srcPoint.X - sourceCorners.Left.X;
                    AddLineSegment(firstSegmentLength, 180);
                    break;
                case OrthogonalDirection.Right:
                    if (sourceCorners.Right.X < targetCorners.Right.X &&
                        sourceCorners.Right.X > targetCorners.Left.X)
                    {
                        if (tarDirection == OrthogonalDirection.Bottom && srcPoint.Y < tarPoint.Y)
                        {
                            firstSegmentLength += targetCorners.Right.X - sourceCorners.Right.X;
                        }
                        else if (tarDirection == OrthogonalDirection.Top && srcPoint.Y > tarPoint.Y)
                        {
                            firstSegmentLength += targetCorners.Right.X - sourceCorners.Right.X;
                        }
                    }
                    firstSegmentLength += sourceCorners.Right.X - srcPoint.X;
                    AddLineSegment(firstSegmentLength, 0);
                    break;
                case OrthogonalDirection.Top:
                    if (sourceCorners.Top.Y > targetCorners.Top.Y &&
                        sourceCorners.Top.Y < targetCorners.Bottom.Y)
                    {
                        if (tarDirection == OrthogonalDirection.Right && srcPoint.X < tarPoint.X)
                        {
                            firstSegmentLength += sourceCorners.Top.Y - targetCorners.Top.Y;
                        }
                        else if (tarDirection == OrthogonalDirection.Left && srcPoint.X > tarPoint.X)
                        {
                            firstSegmentLength += sourceCorners.Top.Y - targetCorners.Top.Y;
                        }
                    }
                    firstSegmentLength += srcPoint.Y - sourceCorners.Top.Y;
                    AddLineSegment(firstSegmentLength, 270);
                    break;
                case OrthogonalDirection.Bottom:
                    if (sourceCorners.Bottom.Y < targetCorners.Bottom.Y &&
                        sourceCorners.Bottom.Y > targetCorners.Top.Y)
                    {
                        if (tarDirection == OrthogonalDirection.Right && srcPoint.X < tarPoint.X)
                        {
                            firstSegmentLength += targetCorners.Bottom.Y - sourceCorners.Bottom.Y;
                        }
                        else if (tarDirection == OrthogonalDirection.Left && srcPoint.X > tarPoint.X)
                        {
                            firstSegmentLength += targetCorners.Bottom.Y - sourceCorners.Bottom.Y;
                        }
                    }
                    firstSegmentLength += sourceCorners.Bottom.Y - srcPoint.Y;
                    AddLineSegment(firstSegmentLength, 90);
                    break;
            }

            if (srcDirection == OrthogonalDirection.Top || srcDirection == OrthogonalDirection.Bottom)
            {
                OrthoConnection3Segment(ConnectorWrapper.RunPoint.Value, tarPoint,
                                        ref sourceCorners, ref targetCorners,
                                        new Thickness(10, 10, 10, 10), new Thickness(10, 10, 10, 10),
                                        Orientation.Horizontal, srcDirection, tarDirection);
            }
            else if (srcDirection == OrthogonalDirection.Right || srcDirection == OrthogonalDirection.Left)
            {
                OrthoConnection3Segment(ConnectorWrapper.RunPoint.Value, tarPoint,
                                        ref sourceCorners, ref targetCorners,
                                        new Thickness(10, 10, 10, 10), new Thickness(10, 10, 10, 10),
                                        Orientation.Vertical, srcDirection, tarDirection);
            }
        }

        private void OrthoConnection5Segment(
            Point srcPoint,
            Point tarPoint,
            ref RectCorners sourceCorners,
            ref RectCorners targetCorners,
            Thickness srcMargin,
            Thickness tarMargin,
            OrthogonalDirection srcDirection,
            OrthogonalDirection tarDirection)
        {
            double firstSegmentLength = 20;
            switch (srcDirection)
            {
                case OrthogonalDirection.Left:
                    AddLineSegment(firstSegmentLength, 180);
                    break;
                case OrthogonalDirection.Top:
                    AddLineSegment(firstSegmentLength, 270);
                    break;
                case OrthogonalDirection.Right:
                    AddLineSegment(firstSegmentLength, 0);
                    break;
                case OrthogonalDirection.Bottom:
                    AddLineSegment(firstSegmentLength, 90);
                    break;
            }


            if (srcDirection == OrthogonalDirection.Top || srcDirection == OrthogonalDirection.Bottom)
            {
                OrthoConnection4Segment(
                    ConnectorWrapper.RunPoint.Value,
                    tarPoint,
                    ref sourceCorners,
                    ref targetCorners,
                    srcMargin,
                    tarMargin,
                    srcPoint.X > tarPoint.X ? OrthogonalDirection.Left : OrthogonalDirection.Right,
                    tarDirection);
            }
            else
            {
                OrthoConnection4Segment(
                    ConnectorWrapper.RunPoint.Value,
                    tarPoint,
                    ref sourceCorners,
                    ref targetCorners,
                    srcMargin,
                    tarMargin,
                    srcPoint.Y > tarPoint.Y ? OrthogonalDirection.Top : OrthogonalDirection.Bottom,
                    tarDirection);
            }
        }

        private void DefaultOrthoConnection(
            IInternalNode knownSourceNode,
            IInternalNodePort sourcePort,
            IInternalNode knownTargetNode,
            IInternalNodePort targetPort,
            OrthogonalDirection? srcDirection = null,
            OrthogonalDirection? tarDirection = null)
        {
            NoOfSeg pts = NoOfSeg.Zero;

            #region Src, Tar Point
            Point? srcPoint = null;
            Point? tarPoint = null;
            RectCorners sourceCorners = knownSourceNode.Corners.Value;
            RectCorners targetCorners = knownTargetNode.Corners.Value;
            if (sourcePort != null)
            {
                srcPoint = new Point(sourcePort.OffsetX, sourcePort.OffsetY);
            }
            else
            {
                switch (srcDirection)
                {
                    case OrthogonalDirection.Left:
                        srcPoint = sourceCorners.Left;
                        break;
                    case OrthogonalDirection.Top:
                        srcPoint = sourceCorners.Top;
                        break;
                    case OrthogonalDirection.Right:
                        srcPoint = sourceCorners.Right;
                        break;
                    case OrthogonalDirection.Bottom:
                        srcPoint = sourceCorners.Bottom;
                        break;
                }
            }

            if (targetPort != null)
            {
                tarPoint = new Point(targetPort.OffsetX, targetPort.OffsetY);
            }
            else
            {
                switch (tarDirection)
                {
                    case OrthogonalDirection.Left:
                        tarPoint = targetCorners.Left;
                        break;
                    case OrthogonalDirection.Top:
                        tarPoint = targetCorners.Top;
                        break;
                    case OrthogonalDirection.Right:
                        tarPoint = targetCorners.Right;
                        break;
                    case OrthogonalDirection.Bottom:
                        tarPoint = targetCorners.Bottom;
                        break;
                }
            }
            #endregion

            #region Swap
            bool swap = false;
            switch (srcDirection)
            {
                case OrthogonalDirection.Left:
                    switch (tarDirection)
                    {
                        case OrthogonalDirection.Right:
                        case OrthogonalDirection.Bottom:
                            swap = true;
                            break;
                    }
                    break;
                case OrthogonalDirection.Top:
                    switch (tarDirection)
                    {
                        case OrthogonalDirection.Left:
                        case OrthogonalDirection.Right:
                        case OrthogonalDirection.Bottom:
                            swap = true;
                            break;
                    }
                    break;
                case OrthogonalDirection.Bottom:
                    switch (tarDirection)
                    {
                        case OrthogonalDirection.Right:
                            swap = true;
                            break;
                    }
                    break;
            }

            if (swap)
            {
                Swap(ref srcPoint, ref tarPoint);
                Swap(ref srcDirection, ref tarDirection);
                Swap(ref sourcePort, ref targetPort);
                Swap(ref knownSourceNode, ref knownTargetNode);
                Swap(ref sourceCorners, ref targetCorners);
            }
            #endregion

            Thickness sourceMargin = new Thickness(10, 10, 10, 10);
            Thickness targetMargin = new Thickness(10, 10, 10, 10);

            // Right -> Left
            #region Right -> Left
            if (srcDirection == OrthogonalDirection.Right && tarDirection == OrthogonalDirection.Left)
            {
                if (sourceCorners.Right.X + sourceMargin.Right <= targetCorners.Left.X + targetMargin.Left)
                {
                    pts = NoOfSeg.Three;
                }
                // S Bend
                else if (sourceCorners.Bottom.Y <= targetCorners.Top.Y)
                {
                    pts = NoOfSeg.Five;
                }
                else if (sourceCorners.Top.Y >= targetCorners.Top.Y)
                {
                    pts = NoOfSeg.Five;
                }
                // @ Bend
                else if ((sourcePort != null && sourcePort.OffsetY <= targetCorners.Top.Y) ||
                            (sourcePort == null && sourceCorners.Right.Y <= targetCorners.Top.Y))
                {
                    pts = NoOfSeg.Five;
                }
                else if ((sourcePort != null && sourcePort.OffsetY >= targetCorners.Bottom.Y) ||
                            (sourcePort == null && sourceCorners.Right.Y >= targetCorners.Bottom.Y))
                {
                    pts = NoOfSeg.Five;
                }
                // Complete overlap
                else
                {
                    pts = NoOfSeg.Five;
                }
            }
            #endregion
            // Right -> Right
            #region Right -> Right

            else if (srcDirection == OrthogonalDirection.Right && tarDirection == OrthogonalDirection.Right)
            {
                if (sourceCorners.Right.X >= targetCorners.Right.X)
                {
                    if ((sourcePort != null && sourcePort.OffsetY < targetCorners.Top.Y) ||
                        (sourcePort == null && sourceCorners.Right.Y < targetCorners.Top.Y))
                    {
                        pts = NoOfSeg.Three;
                    }
                    else if ((sourcePort != null && sourcePort.OffsetY > targetCorners.Bottom.Y) ||
                                (sourcePort == null && sourceCorners.Right.Y > targetCorners.Bottom.Y))
                    {
                        pts = NoOfSeg.Three;
                    }
                    else if (sourceCorners.Right.X >= targetCorners.Left.X)
                    {
                        pts = NoOfSeg.Five;
                    }
                    // Complete overlap
                    else
                    {
                        pts = NoOfSeg.Three;
                    }
                }
                else if ((targetPort != null && sourceCorners.Bottom.Y < targetPort.OffsetY) ||
                            (targetPort == null && sourceCorners.Bottom.Y < targetCorners.Right.Y))
                {
                    pts = NoOfSeg.Three;
                }
                else if ((targetPort != null && sourceCorners.Top.Y > targetPort.OffsetY) ||
                            (targetPort == null && sourceCorners.Top.Y > targetCorners.Right.Y))
                {
                    pts = NoOfSeg.Three;
                }
                else if (sourceCorners.Right.X < targetCorners.Left.X)
                {
                    pts = NoOfSeg.Five;
                }
                // Complete overlap
                else
                {
                    pts = NoOfSeg.Three;
                }
            }
            #endregion
            // Right -> Top
            #region Right -> Top

            else if (srcDirection == OrthogonalDirection.Right && tarDirection == OrthogonalDirection.Top)
            {
                if ((sourcePort != null && sourcePort.OffsetY < targetCorners.Top.Y) ||
                    (sourcePort == null && sourceCorners.Bottom.Y < targetCorners.Top.Y))
                {
                    if (sourceCorners.Bottom.Y < targetCorners.Top.Y)
                    {
                        if ((targetPort != null && sourceCorners.Right.X < targetPort.OffsetX) ||
                            (targetPort == null && sourceCorners.Right.X < targetCorners.Top.X))
                        {
                            pts = NoOfSeg.Two;
                        }
                        else
                        {
                            pts = NoOfSeg.Four;
                        }
                    }
                    else if ((targetPort != null && sourceCorners.Left.X > targetPort.OffsetX) ||
                                (targetPort == null && sourceCorners.Left.X > targetCorners.Top.X))
                    {
                        pts = NoOfSeg.Four;
                    }
                    // Complete overlap
                    else
                    {
                        pts = NoOfSeg.Two;
                    }
                }
                else if (sourceCorners.Right.X < targetCorners.Left.X)
                {
                    pts = NoOfSeg.Four;
                }
                else
                {
                    pts = NoOfSeg.Four;
                }
            }
            #endregion
            // Right -> Bottom
            #region Right -> Bottom

            else if (srcDirection == OrthogonalDirection.Right && tarDirection == OrthogonalDirection.Bottom)
            {
                if ((sourcePort != null && sourcePort.OffsetY > targetCorners.Bottom.Y) ||
                    (sourcePort == null && sourceCorners.Bottom.Y > targetCorners.Bottom.Y))
                {
                    if (sourceCorners.Top.Y > targetCorners.Bottom.Y)
                    {
                        if ((targetPort != null && sourceCorners.Right.X < targetPort.OffsetX) ||
                            (targetPort == null && sourceCorners.Right.X < targetCorners.Bottom.X))
                        {
                            pts = NoOfSeg.Two;
                        }
                        else
                        {
                            pts = NoOfSeg.Four;
                        }
                    }
                    else if ((targetPort != null && sourceCorners.Left.X > targetPort.OffsetX) ||
                                (targetPort == null && sourceCorners.Left.X > targetCorners.Bottom.X))
                    {
                        pts = NoOfSeg.Four;
                    }
                    // Complete overlap
                    else
                    {
                        pts = NoOfSeg.Two;
                    }
                }
                else if (sourceCorners.Right.X < targetCorners.Left.X)
                {
                    pts = NoOfSeg.Four;
                }
                else
                {
                    pts = NoOfSeg.Four;
                }
            }
            #endregion

            // Bottom -> Top
            #region Bottom -> Top

            else if (srcDirection == OrthogonalDirection.Bottom && tarDirection == OrthogonalDirection.Top)
            {
                if (sourceCorners.Bottom.Y < targetCorners.Top.Y)
                {
                    pts = NoOfSeg.Three;
                }
                // S bend
                else if (sourceCorners.Right.X < targetCorners.Left.X)
                {
                    pts = NoOfSeg.Five;
                }
                else if (sourceCorners.Left.X > targetCorners.Right.X)
                {
                    pts = NoOfSeg.Five;
                }
                // @ bend
                else
                {
                    pts = NoOfSeg.Five;
                }
            }
            #endregion
            // Bottom -> Bottom
            #region Bottom -> Bottom

            else if (srcDirection == OrthogonalDirection.Bottom && tarDirection == OrthogonalDirection.Bottom)
            {
                if (sourceCorners.Bottom.Y < targetCorners.Bottom.Y)
                {
                    if ((sourcePort != null && sourcePort.OffsetX < targetCorners.Left.X) ||
                        (sourcePort == null && sourceCorners.Bottom.X < targetCorners.Left.X))
                    {
                        pts = NoOfSeg.Three;
                    }
                    else if ((sourcePort != null && sourcePort.OffsetX > targetCorners.Right.X) ||
                                (sourcePort == null && sourceCorners.Bottom.X > targetCorners.Right.X))
                    {
                        pts = NoOfSeg.Three;
                    }
                    else if (sourceCorners.Bottom.Y < targetCorners.Top.Y)
                    {
                        pts = NoOfSeg.Five;
                    }
                    // Complete overlap
                    else
                    {
                        pts = NoOfSeg.Three;
                    }
                }
                else if ((targetPort != null && sourceCorners.Left.X > targetPort.OffsetX) ||
                            (targetPort == null && sourceCorners.Left.X > targetCorners.Left.X))
                {
                    pts = NoOfSeg.Three;
                }
                else if ((targetPort != null && sourceCorners.Right.X < targetPort.OffsetX) ||
                            (targetPort == null &&
                            sourceCorners.Right.X < targetCorners.Right.X))
                {
                    pts = NoOfSeg.Three;
                }
                else if (sourceCorners.Top.Y > targetCorners.Bottom.Y)
                {
                    pts = NoOfSeg.Five;
                }
                else
                {
                    pts = NoOfSeg.Three;
                }
            }
            #endregion
            // Bottom -> Left
            #region Bottom -> Left

            else if (srcDirection == OrthogonalDirection.Bottom && tarDirection == OrthogonalDirection.Left)
            {
                if ((sourcePort != null && sourcePort.OffsetX < targetCorners.Left.X) ||
                    (sourcePort == null && sourceCorners.Bottom.X < targetCorners.BottomLeft.X))
                {
                    if (sourceCorners.Right.X < targetCorners.Left.X)
                    {
                        if ((targetPort != null && sourceCorners.Bottom.Y < targetPort.OffsetY) ||
                            (targetPort == null && sourceCorners.Bottom.Y < targetCorners.Left.Y))
                        {
                            pts = NoOfSeg.Two;
                        }
                        else
                        {
                            pts = NoOfSeg.Four;
                        }
                    }
                    else if ((targetPort != null && sourceCorners.Top.Y > targetPort.OffsetY) ||
                                (targetPort == null && sourceCorners.Top.Y > targetCorners.Left.Y))
                    {
                        pts = NoOfSeg.Four;
                    }
                    // Overlap
                    else
                    {
                        pts = NoOfSeg.Two;
                    }
                }
                else
                {
                    pts = NoOfSeg.Four;
                }
            }
            #endregion

            // Left -> Left
            #region Left -> Left

            else if (srcDirection == OrthogonalDirection.Left && tarDirection == OrthogonalDirection.Left)
            {
                if (sourceCorners.Left.X < targetCorners.Left.X)
                {
                    if ((targetPort != null && sourceCorners.Bottom.Y < targetPort.OffsetY) ||
                        (targetPort == null && sourceCorners.Bottom.Y < targetCorners.Left.Y))
                    {
                        pts = NoOfSeg.Three;
                    }
                    else if ((targetPort != null && sourceCorners.Top.Y > targetPort.OffsetY) ||
                                (targetPort == null && sourceCorners.Top.Y > targetCorners.Left.Y))
                    {
                        pts = NoOfSeg.Three;
                    }
                    else
                    {
                        pts = NoOfSeg.Five;
                    }
                }
                else if ((sourcePort != null && sourcePort.OffsetY < targetCorners.Top.Y) ||
                            (sourcePort == null && sourceCorners.Left.Y < targetCorners.Top.Y))
                {
                    pts = NoOfSeg.Three;
                }
                else if ((sourcePort != null && sourcePort.OffsetY > targetCorners.Bottom.Y) ||
                            (sourcePort == null && sourceCorners.Left.Y > targetCorners.Bottom.Y))
                {
                    pts = NoOfSeg.Three;
                }
                else if (sourceCorners.Left.X > targetCorners.Right.Y)
                {
                    pts = NoOfSeg.Five;
                }
                // Overlapped
                else
                {
                    pts = NoOfSeg.Three;
                }
            }
            #endregion
            // Left -> Top
            #region Left -> Top

            else if (srcDirection == OrthogonalDirection.Left && tarDirection == OrthogonalDirection.Top)
            {
                if ((sourcePort != null && sourcePort.OffsetY < targetCorners.Top.Y) ||
                    (sourcePort == null && sourceCorners.Bottom.Y < targetCorners.Top.Y))
                {
                    if (sourceCorners.Bottom.Y < targetCorners.Top.Y)
                    {
                        if ((targetPort != null && sourceCorners.Left.X > targetPort.OffsetX) ||
                            (targetPort == null && sourceCorners.Left.X > targetCorners.Top.X))
                        {
                            pts = NoOfSeg.Two;
                        }
                        else
                        {
                            pts = NoOfSeg.Four;
                        }
                    }
                    else if ((targetPort != null && sourceCorners.Right.X < targetPort.OffsetX) ||
                                (targetPort == null && sourceCorners.Right.X < targetCorners.Top.X))
                    {
                        pts = NoOfSeg.Four;
                    }
                    // Complete overlap
                    else
                    {
                        pts = NoOfSeg.Two;
                    }
                }
                else if (sourceCorners.Left.X > targetCorners.Right.X)
                {
                    pts = NoOfSeg.Four;
                }
                else
                {
                    pts = NoOfSeg.Four;
                }
            }
            #endregion

            // Top -> Top
            #region Top -> Top

            else if (srcDirection == OrthogonalDirection.Top && tarDirection == OrthogonalDirection.Top)
            {
                if (sourceCorners.Top.Y < targetCorners.Top.Y)
                {
                    if ((targetPort != null && sourceCorners.Left.X > targetPort.OffsetX) ||
                        (targetPort == null && sourceCorners.Left.X > targetCorners.Left.X))
                    {
                        pts = NoOfSeg.Three;
                    }
                    else if ((targetPort != null && sourceCorners.Right.X < targetPort.OffsetX) ||
                             (targetPort == null && sourceCorners.Right.X < targetCorners.Right.X))
                    {
                        pts = NoOfSeg.Three;
                    }
                    else if (sourceCorners.Bottom.Y < targetCorners.Top.Y)
                    {
                        pts = NoOfSeg.Five;
                    }
                }
                else if ((sourcePort != null && sourcePort.OffsetX > targetCorners.Right.X) ||
                         (sourcePort == null && sourceCorners.Left.X > targetCorners.Right.X))
                {
                    pts = NoOfSeg.Three;
                }
                else if ((sourcePort != null && sourcePort.OffsetX < targetCorners.Left.X) ||
                         (sourcePort == null && sourceCorners.BottomRight.X < targetCorners.Left.X))
                {
                    pts = NoOfSeg.Three;
                }
                else if (sourceCorners.Top.Y > targetCorners.Bottom.Y)
                {
                    pts = NoOfSeg.Five;
                }
                else
                {
                    pts = NoOfSeg.Three;
                }
            }

            #endregion

            if (swap)
            {
                Swap(ref srcPoint, ref tarPoint);
                Swap(ref srcDirection, ref tarDirection);
                Swap(ref sourcePort, ref targetPort);
                Swap(ref knownSourceNode, ref knownTargetNode);
                Swap(ref sourceCorners, ref targetCorners);
            }

            if (ConnectorWrapper.RunPoint == null)
            {
                ConnectorWrapper.RunPoint = srcPoint;
                SetStartPoint(ConnectorWrapper.RunPoint.Value);
            }

            AddOrthoSegments(pts, srcPoint, tarPoint,
                             ref sourceCorners, ref targetCorners, srcDirection, tarDirection);
        }

        enum NoOfSeg
        {
            Zero,
            One,
            Two,
            Three,
            Four,
            Five
        }

        private void AddOrthoSegments(
            NoOfSeg noOfSeg,
            Point? srcPoint,
            Point? tarPoint,
            ref RectCorners sourceCorners,
            ref RectCorners targetCorners,
            OrthogonalDirection? srcDirection,
            OrthogonalDirection? tarDirection)
        {
            if (noOfSeg == NoOfSeg.One)
            {
                AddLineSegment(tarPoint.Value);
            }
            else if (noOfSeg == NoOfSeg.Two)
            {
                switch (srcDirection)
                {
                    case OrthogonalDirection.Left:
                    case OrthogonalDirection.Right:
                        ConnectorWrapper.RunPoint = srcPoint;
                        AddLineSegment(new Point(tarPoint.Value.X, srcPoint.Value.Y));
                        AddLineSegment(tarPoint.Value);
                        break;
                    case OrthogonalDirection.Top:
                    case OrthogonalDirection.Bottom:
                        ConnectorWrapper.RunPoint = srcPoint;
                        AddLineSegment(new Point(srcPoint.Value.X, tarPoint.Value.Y));
                        AddLineSegment(tarPoint.Value);
                        break;
                }
            }
            else if (noOfSeg == NoOfSeg.Three)
            {
                OrthoConnection3Segment(
                    srcPoint.Value,
                    tarPoint.Value,
                    new Thickness(10, 10, 10, 10),
                    new Thickness(10, 10, 10, 10),
                    srcDirection.Value, tarDirection);
            }
            else if (noOfSeg == NoOfSeg.Four)
            {
                OrthoConnection4Segment(srcPoint.Value, tarPoint.Value,
                                        ref sourceCorners, ref targetCorners,
                                        new Thickness(10, 10, 10, 10), new Thickness(10, 10, 10, 10), srcDirection.Value,
                                        tarDirection.Value);
            }
            else if (noOfSeg == NoOfSeg.Five)
            {
                OrthoConnection5Segment(srcPoint.Value, tarPoint.Value,
                                        ref sourceCorners, ref targetCorners,
                                        new Thickness(10, 10, 10, 10), new Thickness(10, 10, 10, 10), srcDirection.Value,
                                        tarDirection.Value);
            }
            else
            {
            }
        }

        public override void PrepareThumb(bool isLast = false)
        {
            if (!IsTerminal && ConnectorWrapper.Constraints.Contains(ConnectorConstraints.SegmentThumbs))
            {
                if (_thumb == null)
                {
                    _thumb = GetThumb<DiagramThumb>(this);
                    _thumb.DragDelta += thumb_DragDelta;
                    _thumb.DragComplete += thumb_DragComplete;
                    _thumb.DragStarting += thumb_DragStarting;
                    _thumb.Visibility = Visibility.Collapsed;
                }
                if (_thumb1 == null)
                {
                    //_thumb1 = GetThumb<DiagramThumb>();
                    //_thumb1.DragDelta += thumb_DragDelta;
                    //_thumb1.DragComplete += thumb_DragDelta;
                    //_thumb1.DragStarting += thumb_DragStarting;
                }
            }
            //throw new NotImplementedException();
        }

        internal void RefereshThumb()
        {
            if (_thumb != null)
            {
                _thumb.DragDelta -= thumb_DragDelta;
                _thumb.DragComplete -= thumb_DragComplete;
                _thumb.DragStarting -= thumb_DragStarting;
                _thumb.ReleaseThumbCapture();
                _thumb.Visibility = Visibility.Collapsed;
                _thumb.DragDelta += thumb_DragDelta;
                _thumb.DragComplete += thumb_DragComplete;
                _thumb.DragStarting += thumb_DragStarting;
            }
            if (_thumb1 != null)
            {
                _thumb1.DragDelta -= thumb_DragDelta;
                _thumb1.DragComplete -= thumb_DragComplete;
                _thumb1.DragStarting -= thumb_DragStarting;
                _thumb1.Visibility = Visibility.Collapsed;
                _thumb1.ReleaseThumbCapture();
                _thumb1.DragDelta += thumb_DragDelta;
                _thumb1.DragComplete += thumb_DragComplete;
                _thumb1.DragStarting += thumb_DragStarting;
            }
        }

        private void thumb_DragComplete(object sender, DiagramThumbDragEventArgs args)
        {
            ConnectorWrapper.RemoveInValidSegments();
        }

        private void thumb_DragStarting(object sender, DiagramThumbDragStartingEventArgs args)
        {
            ConnectorWrapper.SelectedSegment = this;
            _dragStarting = true;
        }

        private bool _dragStarting = false;
        private void thumb_DragDelta(object sender, DiagramThumbDragEventArgs args)
        {
            if (_mNext == null && _dragStarting)
            {
                AddEndSegments(args);
            }
            else if (_mPrev == null && _dragStarting)
            {
                AddStartSegments(args);
            }
            else
            {
                if (_source != null)
                {
                    UpdateSegments(args);
                }
                else
                {
                    if (ConnectorWrapper.KnownSourceNode == ConnectorWrapper.KnownTargetNode
                        && ConnectorWrapper.KnownSourceNode != null &&
                        ConnectorWrapper.KnownTargetNode != null)
                    {
                        ConnectorWrapper.AddEndSegments(ConnectorWrapper.TerminalSegment.Last());
                    }
                    else
                    {
                        ConnectorWrapper.AddEndSegments(this);
                    }
                    UpdateSegments(args);
                }

            }
            _dragStarting = false;
            ConnectorWrapper.CheckSegments();
            ConnectorWrapper.UpdateGeometry();
        }

        private void AddEndSegments(DiagramThumbDragEventArgs args)
        {
            if (_source == null)
            {
                (_mPrev as OrthoWrapper).Length += 5;
                if ((_mPrev as OrthoWrapper)._direction == _direction)
                {
                    UpdatePreviousSegmnet(_mPrev as OrthoWrapper);
                }
                Length = 3 * Length.Value / 4;
                ConnectorWrapper.AddEndSegments(this);
            }
            else if (_mPrev == null)
            {
                AddStartSegments(args);
            }
        }

        private void AddStartSegments(DiagramThumbDragEventArgs args)
        {
            if (_source == null)
            {
                if (_mPrev == null)
                {
                    ConnectorWrapper.AddStartSegments(this);
                    ConnectorWrapper.AddStartSegments(this);
                    Length = 3 * Length.Value / 4;
                }
            }
            else
            {
                if (_mPrev == null)
                {
                    ConnectorWrapper.AddStartSegments(this);
                    Length = 3 * Length.Value / 4;
                }
            }
        }

        private void UpdateSegments(DiagramThumbDragEventArgs args)
        {
            if (_mPrev != null)
            {
                if (_mPrev is OrthoWrapper)
                {
                    OrthoWrapper _previous = _mPrev as OrthoWrapper;
                    if (_previous.Direction == OrthogonalDirection.Right ||
                        _previous.Direction == OrthogonalDirection.Bottom)
                    {
                        if (_previous.Direction == OrthogonalDirection.Right)
                        {
                            _previous.Length += args.Delta.HorizontalChange;
                            if (_mNext != null && _source != null)
                            {
                                UpdateSegment1(args);
                            }
                        }
                        else
                        {
                            _previous.Length += args.Delta.VerticalChange;
                            if (_mNext != null && _source != null)
                            {
                                UpdateSegment1(args);
                            }
                        }
                    }
                    else
                    {
                        if (_previous.Direction == OrthogonalDirection.Left)
                        {
                            _previous.Length -= args.Delta.HorizontalChange;
                            if (_mNext != null && _source != null)
                            {
                                UpdateSegment1(args);
                            }
                        }
                        else
                        {
                            _previous.Length -= args.Delta.VerticalChange;
                            if (_mNext != null && _source != null)
                            {
                                UpdateSegment1(args);
                            }
                        }
                    }
                }
            }
        }

        void UpdateSegment1(DiagramThumbDragEventArgs args)
        {
            if (_mNext is OrthoWrapper)
            {
                OrthoWrapper nextseg = _mNext as OrthoWrapper;
                if (nextseg.Direction == OrthogonalDirection.Top || nextseg.Direction == OrthogonalDirection.Bottom)
                {
                    if (nextseg.Direction == OrthogonalDirection.Top)
                    {
                        (_mNext as OrthoWrapper).Length += args.Delta.VerticalChange;
                    }
                    else
                    {
                        (_mNext as OrthoWrapper).Length -= args.Delta.VerticalChange;
                    }
                }
                else
                {
                    if (nextseg.Direction == OrthogonalDirection.Right)
                    {
                        (_mNext as OrthoWrapper).Length -= args.Delta.HorizontalChange;
                    }
                    else
                    {
                        (_mNext as OrthoWrapper).Length += args.Delta.HorizontalChange;
                    }
                }
            }
        }

        private void UpdatePreviousSegmnet(OrthoWrapper prev)
        {
            if (prev.Direction == OrthogonalDirection.Left || prev.Direction == OrthogonalDirection.Right)
            {
                prev.Direction = OrthogonalDirection.Bottom;
            }
            else
            {
                prev.Direction = OrthogonalDirection.Right;
            }
        }

        public override void UpdateThumb()
        {
            if (!IsTerminal)
            {
                PrepareThumb();
                Point start, end, mid;
                if (_thumb == null && _thumb1 == null)
                {
                    return;
                }
                if (_thumb != null)
                {
                    double l = Length.Value > 0 ? Length.Value : -Length.Value;
                    if (l * SharedData.ScrollViewer.CurrentZoom >= 50)
                    {
                        _thumb.Visibility = Visibility.Visible;
                        ChangeThumbOrientation(_thumb);
                        if (_mPrev != null)
                        {
                            start = _mPrev.EndPoint.Value;
                        }
                        else
                        {
                            start = ConnectorWrapper.PathFigure.StartPoint;
                        }
                        end = this.EndPoint.Value;
                        mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
                        Transform(_thumb, mid.X, mid.Y);
                    }
                    else
                    {
                        _thumb.Visibility = Visibility.Collapsed;
                        _thumb.ReleaseThumbCapture();
                    }
                }
                if (_thumb1 != null)
                {
                    if ((_mNext != null || _source != null))
                    {
                        _thumb1.Visibility = Visibility.Visible;
                        Transform(_thumb1, this.EndPoint.Value.X, this.EndPoint.Value.Y);
                    }
                    else
                    {
                        _thumb1.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void ChangeThumbOrientation(DiagramThumb thumb)
        {
            if (Direction == OrthogonalDirection.Left || Direction == OrthogonalDirection.Right)
            {
                thumb.Width = 40;
                thumb.Height = 15;
            }
            else
            {
                thumb.Width = 15;
                thumb.Height = 40;
            }
        }

        public override void DisposeThumb()
        {
            if (_thumb != null)
            {
                _thumb.DragDelta -= thumb_DragDelta;
                _thumb.DragComplete -= thumb_DragComplete;
                _thumb.DragStarting -= thumb_DragStarting;
                _thumb.ReleaseThumbCapture();
                Recycle(_thumb);
                _thumb = null;
            }
            //if (_thumb1 != null)
            //{
            //    _thumb1.DragDelta -= thumb1_DragDelta;
            //    _thumb1.DragComplete -= thumb1_DragDelta;
            //    _thumb1.DragStarting -= thumb1_DragStarting;
            //    Recycle(_thumb1);
            //    _thumb1 = null;
            //}
        }

        public override void UpdateAdjacentSegments(PropChangedEventArgs<IInternalSegment> propChangedEventArgs)
        {
            //throw new NotImplementedException();
        }

    }

    internal class QBezierWrapper : SegmentWrapper, IQuadraticCurveInternal
    {
        public QBezierWrapper(IQuadraticCurveSegment source, ConnectorWrapper connector, SharedData shared) :
            base(source, connector, shared)
        {
            _source = source;
        }

        private readonly IQuadraticCurveSegment _source;
        private DiagramThumb _thumb1, _thumb2;

        private Point _point1;
        private Point _point2;

        public Point? Point1
        {
            get { return _source.Point1 ?? _point1; }
            set
            {
                if (_source.Point1 != null)
                {
                    _source.Point1 = value;
                }
                else if (value != null)
                {
                    _point1 = value.Value;
                }
            }
        }
        public Point? Point2
        {
            get { return _source.Point2 ?? _point2; }
            set
            {
                if (_source.Point2 != null)
                {
                    _source.Point2 = value;
                }
                else if (value != null)
                {
                    _point2 = value.Value;
                }
            }
        }

        public override Point GetSourcePoint(out double angle)
        {
            Point? adj = (Source as IQuadraticCurveSegment).Point1;
            return GetSourcePoint(adj, out angle);
        }

        private void Update()
        {
            Point end;
            if (_source.Point1 == null)
            {
                double distance = 60;
                Point1 = ConnectorWrapper.RunPoint.Value.Transform(distance, ConnectorWrapper.RunAngle);
                //GetBezierDirection(ConnectorWrapper.KnownSourcePort, ConnectorWrapper.KnownSourceNode, ConnectorWrapper.SourcePoint,
                //                   ConnectorWrapper.KnownTargetPort, ConnectorWrapper.KnownTargetNode, ConnectorWrapper.TargetPoint,
                //                   out end, out endAdj);
                //Point1 = endAdj;
            }
            if (_source.Point2 == null)
            {
                OrthogonalDirection dir = GetBezierDirection(ConnectorWrapper.KnownTargetPort, ConnectorWrapper.KnownTargetNode, ConnectorWrapper.TargetPoint,
                                   ConnectorWrapper.KnownSourcePort, ConnectorWrapper.KnownSourceNode, ConnectorWrapper.SourcePoint,
                                   out end);
                Point2 = end;
            }
        }

        private void AddQBezierSegment()
        {
            ConnectorWrapper.InternalSegments.Add(new QuadraticBezierSegment() { Point1 = Point1.Value, Point2 = Point2.Value });
            ConnectorWrapper.RunPoint = Point2;
            ConnectorWrapper.RunAngle = Point1.Value.FindAngle(Point2.Value);
        }

        public override bool AddView()
        {
            if (_source.Point1 == null || _source.Point2 == null)
            {
                IsTerminal = true;
                Update();
                AddQBezierSegment();
                EndPoint = Point2;
                return false;
            }
            IsTerminal = false;
            AddQBezierSegment();
            EndPoint = Point2;
            return true;
        }

        public override void PrepareThumb(bool isLast = false)
        {
            if (_thumb1 == null && ConnectorWrapper.SelectedSegment != null &&
                (this == ConnectorWrapper.SelectedSegment || this.Prev == ConnectorWrapper.SelectedSegment || _source == null))
            {
                _thumb1 = GetThumb<DiagramThumb>();
                _thumb1.DragDelta += _thumb1_DragDelta;
                _thumb1.DragComplete += _thumb1_DragDelta;
            }
            if (_thumb2 == null && this.Next != null)
            {
                _thumb2 = GetThumb<DiagramThumb>(this);
                _thumb2.DragDelta += _thumb1_DragDelta;
                _thumb2.DragComplete += _thumb1_DragDelta;
                _thumb2.DragStarting += _thumb_DragStarting;
            }
        }

        void _thumb_DragStarting(object sender, DiagramThumbDragStartingEventArgs args)
        {
            if (ConnectorWrapper.SelectedSegment != null)
            {
                (ConnectorWrapper.SelectedSegment as SegmentWrapper).DisposeThumb();
                if (ConnectorWrapper.SelectedSegment.Next != null)
                {
                    (ConnectorWrapper.SelectedSegment.Next as SegmentWrapper).DisposeThumb();
                }
            }
            ConnectorWrapper.SelectedSegment = this;
            this.PrepareThumb();
            if (Next != null)
            {
                (Next as SegmentWrapper).PrepareThumb();
            }
        }

        private void _thumb1_DragDelta(object sender, DiagramThumbDragEventArgs args)
        {
            if (_source != null)
            {
                if (sender == _thumb1)
                {
                    _source.Point1 = new Point(Point1.Value.X + args.Delta.HorizontalChange,
                        Point1.Value.Y + args.Delta.VerticalChange);
                }
                else if (sender == _thumb2)
                {
                    _source.Point2 = new Point(Point2.Value.X + args.Delta.HorizontalChange,
                        Point2.Value.Y + args.Delta.VerticalChange);
                }
            }
            Invalidate();
        }

        public override void UpdateThumb()
        {
            if (_thumb1 != null)
            {
                Transform(_thumb1, Point1.Value.X, Point1.Value.Y);
            }

            if (_thumb2 != null)
            {
                Transform(_thumb2, Point2.Value.X, Point2.Value.Y);
            }
        }

        public override void DisposeThumb()
        {
            if (ConnectorWrapper.SelectedSegment == null ||
                (this == ConnectorWrapper.SelectedSegment || this.Prev == ConnectorWrapper.SelectedSegment || _source != null))
            {
                if (_thumb1 != null)
                {
                    _thumb1.DragDelta -= _thumb1_DragDelta;
                    _thumb1.DragComplete -= _thumb1_DragDelta;
                    Recycle(_thumb1);
                    _thumb1 = null;
                }
            }
            if (_thumb2 != null && ConnectorWrapper.SelectedSegment == null)
            {
                _thumb2.DragDelta -= _thumb1_DragDelta;
                _thumb2.DragComplete -= _thumb1_DragDelta;
                _thumb2.DragStarting -= _thumb_DragStarting;
                Recycle(_thumb2);
                _thumb2 = null;
            }
        }

        public override void UpdateAdjacentSegments(PropChangedEventArgs<IInternalSegment> propChangedEventArgs)
        {
            //throw new NotImplementedException();
        }
    }

    internal class CBezierWrapper : SegmentWrapper, ICubicCurveInternal
    {
        bool _isPointUpdated = false;
        internal CBezierWrapper(ICubicCurveSegment source, ConnectorWrapper connector, SharedData shared) :
            base(source, connector, shared)
        {
            _source = source;
            _source.Constraints = SegmentConstraints.Inherit;
            SegmentChangedCallback = UpdateAdjacentSegments;
            source.PropertyChanged += source_PropertyChanged;
        }

        void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Point1":
                    if (Prev != null && Prev is CBezierWrapper && !(Prev as CBezierWrapper)._isPointUpdated)
                    {
                        _isPointUpdated = true;
                        Point? old = _point1 ?? Point1;
                        SegmentChangedCallback(new PropChangedEventArgs<IInternalSegment>(this, old, Point1, "Point1"));
                        _point1 = (sender as CubicCurveSegment).Point1.Value;
                        if (_source.Vector1 != null)
                        {
                            UpdateVector1();
                        }
                        _isPointUpdated = false;
                    }
                    break;
                case "Point2":
                    if (Next != null && Next is CBezierWrapper && !(Next as CBezierWrapper)._isPointUpdated)
                    {
                        _isPointUpdated = true;
                        Point? old = _point2 ?? Point2;
                        SegmentChangedCallback(new PropChangedEventArgs<IInternalSegment>(this, old, Point2, "Point2"));
                        _point2 = (sender as CubicCurveSegment).Point2.Value;
                        if (_source.Vector2 != null)
                        {
                            UpdateVector2();
                        }
                        _isPointUpdated = false;
                    }
                    break;
                case "Point3":
                    break;
                case "Vector1":
                    Vector1ToPoint1();
                    break;
                case "Vector2":
                    Vector2ToPoint2();
                    break;

            }
        }

        private void UpdateVector1()
        {
            if (Prev == null)
            {
                if (_source.Vector1.Value.X != Point1.Value.X - ConnectorWrapper.SourcePoint.X || _source.Vector1.Value.Y != Point2.Value.Y - ConnectorWrapper.SourcePoint.Y)
                {
                    _source.Vector1 = new Vector(Point1.Value.X - ConnectorWrapper.SourcePoint.X,
                        Point2.Value.Y - ConnectorWrapper.SourcePoint.Y);
                }
            }
            else
            {
                if (_source.Vector1.Value.X != Point1.Value.X - Prev.EndPoint.Value.X || _source.Vector1.Value.Y != Point1.Value.Y - Prev.EndPoint.Value.Y)
                    _source.Vector1 = new Vector(Point1.Value.X - Prev.EndPoint.Value.X, Point1.Value.Y - Prev.EndPoint.Value.Y);
            }
        }

        private void UpdateVector2()
        {
            Point? end = EndPoint ?? Point3;
            if (end != null)
                if (_source.Vector2.Value.X != Point2.Value.X - end.Value.X || _source.Vector2.Value.Y != Point2.Value.Y - end.Value.Y)
                    _source.Vector2 = new Vector(Point2.Value.X - end.Value.X, Point2.Value.Y - end.Value.Y);
        }
        private readonly ICubicCurveSegment _source;
        private DiagramThumb _thumb1, _thumb2, _thumb3;
        private Line _line1, _line2;

        private Point? _point1;
        private Point? _point2;
        private Point? _point3;
        public Point? Point1
        {
            get { return _source.Point1 ?? _point1; }
            set
            {
                if (_source.Point1 != null)
                {
                    _source.Point1 = value;
                }
                else if (value != null)
                {
                    _point1 = value.Value;
                }
            }
        }

        public Point? Point2
        {
            get { return _source.Point2 ?? _point2; }
            set
            {
                if (_source.Point2 != null)
                {
                    _source.Point2 = value;
                }
                else if (value != null)
                {
                    _point2 = value.Value;
                }
            }
        }

        public Point? Point3
        {
            get { return _source.Point3 ?? _point3; }
            set
            {
                if (_source.Point3 != null)
                {
                    _source.Point3 = value;
                }
                else if (value != null)
                {
                    _point3 = value.Value;
                }
            }
        }

        public override Point GetSourcePoint(out double angle)
        {
            Point? adj = _source.Point1;
            return GetSourcePoint(adj, out angle);
        }

        private void Update()
        {
            Point end;

            if (_source.Point1 == null)
            {
                if (_source.Point3 != null)
                {
                    Point1 = GetBezierAdjPoint(ConnectorWrapper.RunAngle, ConnectorWrapper.RunPoint.Value, Point3.Value);
                }
                else
                {
                    OrthogonalDirection dir = GetBezierDirection(ConnectorWrapper.KnownTargetPort, ConnectorWrapper.KnownTargetNode, ConnectorWrapper.TargetPoint,
                                       null, null, ConnectorWrapper.RunPoint.Value, out end);
                    Point1 = GetBezierAdjPoint(ConnectorWrapper.RunAngle, ConnectorWrapper.RunPoint.Value, end);
                    Point2 = _source.Point2 ?? GetBezierAdjPoint(dir.ToAngle(), end, ConnectorWrapper.RunPoint.Value);
                    Point3 = _source.Point3 ?? end;

                }

                //double distance = 10;
                //Point1 = ConnectorWrapper.RunPoint.Value.Transform(distance, ConnectorWrapper.RunAngle);
                //GetBezierDirection(ConnectorWrapper.KnownSourcePort, ConnectorWrapper.KnownSourceNode, ConnectorWrapper.SourcePoint,
                //                   ConnectorWrapper.KnownTargetPort, ConnectorWrapper.KnownTargetNode, ConnectorWrapper.TargetPoint,
                //                   out end, out endAdj);
            }
            //else
            //{
            //    Point1 = Point1.Value;
            //}
            if (_source.Point2 == null || _source.Point3 == null)
            {
                OrthogonalDirection dir = GetBezierDirection(ConnectorWrapper.KnownTargetPort, ConnectorWrapper.KnownTargetNode, ConnectorWrapper.TargetPoint,
                                   null, null, ConnectorWrapper.RunPoint.Value, out end);
                Point2 = _source.Point2 ?? GetBezierAdjPoint(dir.ToAngle(), end, ConnectorWrapper.RunPoint.Value);
                Point3 = _source.Point3 ?? end;
                //OrthogonalDirection dir = GetBezierDirection(ConnectorWrapper.KnownTargetPort, ConnectorWrapper.KnownTargetNode, ConnectorWrapper.TargetPoint,
                //                   null, null, ConnectorWrapper.RunPoint.Value,
                //                   out end);

                //Point2 = _source.Point2 ?? endAdj;
                //Point3 = _source.Point3 ?? end;
            }
            //else
            //{
            //    Point2 = Point2.Value;
            //    Point3 = Point3.Value;
            //}
            //AddCBezierSegment(p1.Value, p2.Value, p3.Value);
        }

        private void Vector2ToPoint2()
        {
            if (Next == null)
            {
                if (_source.Point2 == null || (_source.Point2.Value.X != ConnectorWrapper.TargetPoint.X + _source.Vector2.Value.X ||
                    _source.Point2.Value.Y != ConnectorWrapper.TargetPoint.Y + _source.Vector2.Value.Y))
                    _source.Point2 = new Point(ConnectorWrapper.TargetPoint.X + _source.Vector2.Value.X,
                        ConnectorWrapper.TargetPoint.Y + _source.Vector2.Value.Y);
            }
            else
            {
                if (_source.Point2 == null || (_source.Point2.Value.X == Point3.Value.X + _source.Vector2.Value.X ||
                    _source.Point2.Value.Y == Point3.Value.Y + _source.Vector2.Value.Y))
                    _source.Point2 = new Point(Point3.Value.X + _source.Vector2.Value.X,
                       Point3.Value.Y + _source.Vector2.Value.Y);
            }
        }


        private void Vector1ToPoint1()
        {
            if (Prev == null)
            {
                if (_source.Point1 == null || (_source.Point1.Value.X != ConnectorWrapper.SourcePoint.X + _source.Vector1.Value.X || _source.Point2.Value.Y != ConnectorWrapper.SourcePoint.Y + _source.Vector1.Value.Y))
                    _source.Point1 = new Point(ConnectorWrapper.SourcePoint.X + _source.Vector1.Value.X,
                        ConnectorWrapper.SourcePoint.Y + _source.Vector1.Value.Y);
            }
            else
            {
                if (_source.Point1 == null || (_source.Point1.Value.X != Prev.EndPoint.Value.X + _source.Vector1.Value.X || _source.Point2.Value.Y != Prev.EndPoint.Value.Y + _source.Vector1.Value.Y))
                    _source.Point1 = new Point(Prev.EndPoint.Value.X + _source.Vector1.Value.X,
                       Prev.EndPoint.Value.Y + _source.Vector1.Value.Y);
            }
        }


        private void AddCBezierSegment()
        {
            ConnectorWrapper.InternalSegments.Add(new BezierSegment()
                {
                    Point1 = Point1.Value,
                    Point2 = Point2.Value,
                    Point3 = Point3.Value
                });

            ConnectorWrapper.RunPoint = Point3.Value;
            ConnectorWrapper.RunAngle = Point2.Value.FindAngle(Point3.Value);
        }

        public override bool AddView()
        {
            if (_source.Point1 == null && _source.Vector1 != null)
            {
                Vector1ToPoint1();

            }
            if (_source.Point2 == null && _source.Vector2 != null)
            {
                Vector2ToPoint2();
            }
            if (_source.Point1 == null || _source.Point2 == null || _source.Point3 == null)
            {
                Update();
                AddCBezierSegment();
                EndPoint = Point3;
                return false;
            }
            AddCBezierSegment();
            EndPoint = Point3;
            return true;
        }

        public override void PrepareThumb(bool isLast = false)
        {
            if (ConnectorWrapper.SelectedSegment != null && (this == ConnectorWrapper.SelectedSegment || this.Prev == ConnectorWrapper.SelectedSegment))
            {
                if (_thumb1 != null)
                {
                    return;
                }
                _thumb1 = GetThumb<DiagramThumb>();
                _thumb1.DragDelta += _thumb1_DragDelta;
                _thumb1.DragComplete += _thumb1_DragDelta;
                _thumb2 = GetThumb<DiagramThumb>();
                _thumb2.DragDelta += _thumb1_DragDelta;
                _thumb2.DragComplete += _thumb1_DragDelta;

                _line1 = GetThumb<Line>();
                _line2 = GetThumb<Line>();
            }
            if (_thumb3 == null && this.Next != null)
            {
                _thumb3 = GetThumb<DiagramThumb>(this);
                _thumb3.DragStarting += _thumb3_DragStarting;
                _thumb3.DragDelta += _thumb1_DragDelta;
                _thumb3.DragComplete += _thumb1_DragDelta;
            }

        }

        void _thumb3_DragStarting(object sender, DiagramThumbDragStartingEventArgs args)
        {
            
            if (this != ConnectorWrapper.SelectedSegment)
            {
                if (ConnectorWrapper.SelectedSegment != null)
                {
                    (ConnectorWrapper.SelectedSegment as SegmentWrapper).DisposeThumb();
                    if (ConnectorWrapper.SelectedSegment.Next != null)
                        (ConnectorWrapper.SelectedSegment.Next as SegmentWrapper).DisposeThumb();
                }
                ConnectorWrapper.SelectedSegment = this;
                PrepareThumb();
                if (Next != null)
                {
                    (Next as SegmentWrapper).PrepareThumb();
                }
            }
            else if (Prev == null)
            {
                if (Next != null)
                {
                    (Next as SegmentWrapper).PrepareThumb();
                }
            }

        }

        private void _thumb1_DragDelta(object sender, DiagramThumbDragEventArgs args)
        {
            if (sender == _thumb1)
            {
                _source.Point1 = new Point(Point1.Value.X + args.Delta.HorizontalChange,
                                   Point1.Value.Y + args.Delta.VerticalChange);
            }

            else if (sender == _thumb2)
            {
                _source.Point2 = new Point(Point2.Value.X + args.Delta.HorizontalChange,
                                   Point2.Value.Y + args.Delta.VerticalChange);
            }
            else if (sender == _thumb3)
            {
                Point? pt = new Point(Point3.Value.X + args.Delta.HorizontalChange,
                                   Point3.Value.Y + args.Delta.VerticalChange);
                _source.Point3 = pt;
            }
            Invalidate();
        }



        public override void UpdateAdjacentSegments(PropChangedEventArgs<IInternalSegment> propChangedEventArgs)
        {
            if (propChangedEventArgs.PropertyName.Equals("Point3"))
            {
                double diffx = (propChangedEventArgs.NewValue as Point?).Value.X - (propChangedEventArgs.OldValue as Point?).Value.X;
                double dify = (propChangedEventArgs.NewValue as Point?).Value.Y - (propChangedEventArgs.OldValue as Point?).Value.Y;
                this._isPointUpdated = true;
                CBezierWrapper next = null;
                if (Next != null && Next is CBezierWrapper)
                {
                    next = Next as CBezierWrapper;
                    next.Point1 = new Point(next.Point1.Value.X + diffx, next.Point1.Value.Y + dify);
                    next._isPointUpdated = true;
                }
                _source.Point2 = new Point(Point2.Value.X + diffx,
                        Point2.Value.Y + dify);
                this._isPointUpdated = false;
                if (next != null)
                {
                    next._isPointUpdated = false;
                }
            }
            else if (propChangedEventArgs.PropertyName.Equals("Source"))
            {
                this._isPointUpdated = true;
                double diffx = (propChangedEventArgs.NewValue as Point?).Value.X - (propChangedEventArgs.OldValue as Point?).Value.X;
                double dify = (propChangedEventArgs.NewValue as Point?).Value.Y - (propChangedEventArgs.OldValue as Point?).Value.Y;
                _source.Point1 = new Point(Point1.Value.X + diffx,
                        Point1.Value.Y + dify);
                this._isPointUpdated = false;
            }
            else if (CanSmooth(BezierSmoothness.SymmetricAngle))
            {
                if (propChangedEventArgs.PropertyName.Equals("Point1"))
                {
                    if (Prev != null && Prev is CBezierWrapper)
                    {
                        double angle = Prev.EndPoint.Value.FindAngle(Point1.Value);
                        double length = 0;
                        if (CanSmooth(BezierSmoothness.SymmetricDistance))
                        {
                            length = Prev.EndPoint.Value.FindLength(Point1.Value);
                        }
                        else
                        {
                            length = Prev.EndPoint.Value.FindLength((Prev as CBezierWrapper).Point2.Value);
                        }
                        (Prev as CBezierWrapper)._source.Point2 = Prev.EndPoint.Value.Transform(length, angle - 180);
                    }
                }
                else if (propChangedEventArgs.PropertyName.Equals("Point2"))
                {
                    if (Next != null && Next is CBezierWrapper)
                    {
                        double angle = Point3.Value.FindAngle(Point2.Value);
                        double length = 0;
                        if (ConnectorWrapper.BezierSmoothness.Contains(BezierSmoothness.SymmetricDistance))
                        {
                            length = Point2.Value.FindLength(Point3.Value);
                        }
                        else
                        {
                            length = Point3.Value.FindLength((Next as CBezierWrapper).Point1.Value);
                        }
                        (Next as CBezierWrapper)._source.Point1 = Point3.Value.Transform(length, 180 + angle);
                    }
                }

            }
            else if (CanSmooth(BezierSmoothness.SymmetricDistance))
            {
                if (propChangedEventArgs.PropertyName.Equals("Point2"))
                {
                    if (Prev != null && Prev is CBezierWrapper)
                    {
                        double angle = Prev.EndPoint.Value.FindAngle(Point1.Value);
                        (Prev as CBezierWrapper)._source.Point2 = Prev.EndPoint.Value.Transform(Prev.EndPoint.Value.FindLength(Point1.Value), angle - 180);
                    }
                }
                else
                {
                    if (Next != null && Next is CBezierWrapper)
                    {
                        double angle = Point3.Value.FindAngle(Point2.Value);
                        (Next as CBezierWrapper)._source.Point1 = Point3.Value.Transform(Point2.Value.FindLength(Point3.Value), 180 + angle);
                    }
                }
            }
        }

        private bool CanSmooth(BezierSmoothness smooth)
        {
            if (ConnectorWrapper.Constraints.Contains(ConnectorConstraints.InheritSmoothness))
            {
                return SharedData.Graph.BezierSmoothness.Contains(smooth);
            }
            else if (this._source.Constraints.Contains(SegmentConstraints.Inherit))
            {
                return ConnectorWrapper.BezierSmoothness.Contains(smooth);
            }
            else
            {
                return this._source.BezierSmoothness.Contains(smooth);
            }
        }

        public override void UpdateThumb()
        {
            if (_thumb1 == null && _thumb2 == null && _thumb3 == null)
            {
                return;
            }

            if (_thumb1 != null && _thumb2 != null)
            {
                Point start = new Point(0, 0);
                if (Prev == null)
                {
                    start = ConnectorWrapper.SourcePoint;
                }
                else
                {
                    start = Prev.EndPoint.Value;
                }
                Transform(_thumb1, Point1.Value.X, Point1.Value.Y);
                Transform(_thumb2, Point2.Value.X, Point2.Value.Y);

                double scale=1;
                if (SharedData.ScrollViewer != null)
                    scale = SharedData.ScrollViewer.CurrentZoom;

                _line1.X1 = start.X*scale;
                _line1.Y1 = start.Y*scale;
                _line1.X2 = Point1.Value.X*scale;
                _line1.Y2 = Point1.Value.Y*scale;

                _line2.X1 = Point2.Value.X*scale;
                _line2.Y1 = Point2.Value.Y*scale;
                _line2.X2 = Point3.Value.X*scale;
                _line2.Y2 = Point3.Value.Y*scale;

            }
            if (_thumb3 != null)
            {
                Transform(_thumb3, Point3.Value.X, Point3.Value.Y);
            }
        }

        public override void DisposeThumb()
        {
            if (ConnectorWrapper.SelectedSegment == null ||
                (this == ConnectorWrapper.SelectedSegment || this.Prev == ConnectorWrapper.SelectedSegment))
            {
                if (_thumb1 != null)
                {
                    _thumb1.DragDelta -= _thumb1_DragDelta;
                    _thumb1.DragComplete -= _thumb1_DragDelta;
                    _thumb2.DragDelta -= _thumb1_DragDelta;
                    _thumb2.DragComplete -= _thumb1_DragDelta;
                    Recycle(_thumb1);
                    Recycle(_thumb2);
                    _thumb1 = null;
                    _thumb2 = null;
                    Recycle(_line1);
                    Recycle(_line2);
                }
            }

            if (_thumb3 != null && ConnectorWrapper.SelectedSegment == null)
            {
                _thumb3.DragStarting -= _thumb3_DragStarting;
                _thumb3.DragDelta -= _thumb1_DragDelta;
                _thumb3.DragComplete -= _thumb1_DragDelta;
                Recycle(_thumb3);
                _thumb3 = null;
            }
        }

        Vector? ICubicCurveSegment.Vector1
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        Vector? ICubicCurveSegment.Vector2
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
