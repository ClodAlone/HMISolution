#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Linq.Expressions;
using System.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Controls;
using System.Windows.Media; 
using System.Windows.Shapes;
#endif
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram
{
    internal sealed partial class ConnectorWrapper :
        GroupableWrapper,
        IInternalConnector
    {
        private ObservableElements<IConnectorSegment, SegmentWrapper> ConnectorSegments;
        private SegmentWrapper FirstSegment;
        private SegmentWrapper LastSegment;
        internal List<SegmentWrapper> TerminalSegment = new List<SegmentWrapper>();
        public Point? RunPoint { get; set; }
        public double RunAngle { get; set; }
        private bool _editingThums = false;
        internal bool IsEndPointChanged = false;
        private bool IsBoundsChanged = false;
        private bool IsSegmentAdded = false;
        private bool _orthoEditingFinished = true;
        internal bool isRoutted = false;

        private ConnectorChangedEventArgs _mCurrentSource;
        private ConnectorChangedEventArgs _mCurrentTarget;


        public override object GetData()
        {
            return new[] { _mCurrentSource, _mCurrentTarget, base.GetData() };
        }

        private void SourceChangedNotification(
            ref ConnectorChangedEventArgs oldValue,
            ref ConnectorChangedEventArgs newValue)
        {
            if (SharedData != null && SharedData.NeededEvents.NeedSourceChangedEvent)
            {
                SharedData.Graph.OnConnectorSourceChangedEvent(
                    new ChangeEventArgs<object, ConnectorChangedEventArgs>(this.Source, ref oldValue, ref newValue));
            }
            if (SharedData.UndoRedoController != null && CanLogData())
            {
                LogData(new[] { oldValue, _mCurrentTarget, base.GetData() });
            }

            if (oldValue.InternalNode != newValue.InternalNode)
            {
                if (oldValue.InternalNode != null)
                {
                    oldValue.InternalNode.BoundsChanged -= newV_BoundsChanged;
                }
                if (newValue.InternalNode != null)
                {
                    newValue.InternalNode.BoundsChanged += newV_BoundsChanged;
                }
                if (SharedData.Graph.CanRelate)
                {
                    SharedData.RelationshipController.SourceChanged(this, oldValue.InternalNode, newValue.InternalNode);
                }
            }
            if (oldValue.InternalPort != newValue.InternalPort)
            {
                if (oldValue.InternalPort != null)
                {
                    oldValue.InternalPort.PositionChanged -= InternalPort_PositionChanged;
                }
                if (newValue.InternalPort != null)
                {
                    newValue.InternalPort.PositionChanged += InternalPort_PositionChanged;
                }
            }
            if (FirstSegment != null && FirstSegment.Count() > 0 && (this.IsSelected || SourceNode == null))
            {
                IInternalSegment firstsegment = FirstSegment.First();
                if (firstsegment.SegmentChangedCallback != null && (this.SourceDragState == DragState.Dragging ||
                (firstsegment.Source as CubicCurveSegment).Point1 != null) && oldValue.Point != new Point(0, 0) && oldValue.Point != newValue.Point)
                {
                    firstsegment.SegmentChangedCallback(new PropChangedEventArgs<IInternalSegment>(firstsegment, oldValue.Point, newValue.Point, "Source"));
                }
            }
        }

        void InternalPort_PositionChanged()
        {
            if (View != null)
            {
                (View as UIElement).InvalidateArrange();
            }
        }

        private void TargetChangedNotification(
            ref ConnectorChangedEventArgs oldValue,
            ref ConnectorChangedEventArgs newValue)
        {
            if (SharedData != null && SharedData.NeededEvents.NeedTargetChangedEvent)
            {
                SharedData.Graph.OnConnectorTargetChangedEvent(
                    new ChangeEventArgs<object, ConnectorChangedEventArgs>(this.Source, ref oldValue, ref newValue));
            }
            if (SharedData.UndoRedoController != null && CanLogData())
            {
                LogData(new[] { _mCurrentSource, oldValue, base.GetData() });
            }
            if (oldValue.InternalNode != newValue.InternalNode)
            {
                if (oldValue.InternalNode != null)
                {
                    oldValue.InternalNode.BoundsChanged -= newV_BoundsChanged;
                }
                if (newValue.InternalNode != null)
                {
                    newValue.InternalNode.BoundsChanged += newV_BoundsChanged;
                }
                if (SharedData.Graph.CanRelate)
                {
                    SharedData.RelationshipController.TargetChanged(this, oldValue.InternalNode, newValue.InternalNode);
                }
            }
            if (oldValue.InternalPort != newValue.InternalPort)
            {
                if (oldValue.InternalPort != null)
                {
                    oldValue.InternalPort.PositionChanged -= InternalPort_PositionChanged;
                }
                if (newValue.InternalPort != null)
                {
                    newValue.InternalPort.PositionChanged += InternalPort_PositionChanged;
                }
            }
        }

        public override object Undo(object data)
        {
            return RevertTo(data);
        }

        public override object Redo(object data)
        {
            return RevertTo(data);
        }

        private object RevertTo(object data)
        {
            if (data is Array)
            {
                ConnectorChangedEventArgs srcState = (ConnectorChangedEventArgs)(data as object[])[0];
                ConnectorChangedEventArgs tarState = (ConnectorChangedEventArgs)(data as object[])[1];
                GroupableState gstate = (GroupableState)(data as object[])[2];
                var current = GetData();
                //if (connectorChanged.IsSource)
                {
                    //ConnectorChangedArgs current = _mCurrentSource;
                    SourcePoint = srcState.Point;
                    SourceNode = srcState.Node;
                    SourcePort = srcState.Port;
                }
                //else
                {
                    //ConnectorChangedArgs current = _mCurrentTarget;
                    TargetPoint = tarState.Point;
                    TargetNode = tarState.Node;
                    TargetPort = tarState.Port;
                }
                if (gstate.Zindex != ZIndex)
                {
                    ZIndex = gstate.Zindex;
                }
                return current;
            }
            return data;
        }

        public override void Tap(bool fireEvent)
        {
            if (fireEvent)
            {
                SharedData.Graph.OnItemTappedEvent(new DiagramEventArgs(this.Source));
            }
            base.Tap(false);
        }

        public override void DoubleTap(bool fireEvent)
        {
            if (fireEvent)
            {
                SharedData.Graph.OnItemDoubleTappedEvent(new DiagramEventArgs(this.Source));
            }
            base.DoubleTap(false);
        }

        public Connector Connector;

        public override IView View
        {
            get { return Connector; }
            set
            {
                UnsetView();
                Connector = value as Connector;
                PrepareView();
            }
        }

        protected override bool CanSelect()
        {
            if (base.CanSelect())
            {
                if (Constraints.Contains(ConnectorConstraints.Selectable))
                {
                    return true;
                }
            }
            return false;
        }

        private void UnsetView()
        {
            if (View != null)
            {
                if (View is Connector)
                {
                    (View as Connector).Wrapper = null;
                }
            }
        }

        private void PrepareView()
        {
            if (View != null)
            {
                if (View is Connector)
                {
                    (View as Connector).Wrapper = this;
                }
            }
        }

        public ConnectorWrapper(SharedData shared)
            : base(shared)
        {
            _mCurrentSource = new ConnectorChangedEventArgs(SourceChangedNotification, true);
            _mCurrentTarget = new ConnectorChangedEventArgs(TargetChangedNotification, false);
        }

        private void SourceNodeChanged()
        {
            if (SourceNode != null)
            {
                if (KnownSourceNode == null ||
                    KnownSourceNode.Source != SourceNode)
                {
                    KnownSourceNode = SharedData.Graph.GetNodeWrapper(SourceNode, true);
                }
            }
            else
            {
                KnownSourceNode = null;
            }
        }

        private void TargetNodeChanged()
        {
            if (TargetNode != null)
            {
                if (KnownTargetNode == null ||
                    KnownTargetNode.Source != TargetNode)
                {
                    KnownTargetNode = SharedData.Graph.GetNodeWrapper(TargetNode, true);
                }
            }
            else
            {
                KnownTargetNode = null;
            }
        }

        private void SourcePortChanged()
        {
            if (SourcePort != null)
            {
                if (KnownSourcePort == null ||
                    KnownSourcePort.Source != SourcePort)
                {
                    KnownSourcePort =
                        SharedData.Graph
                                  .GetNodePortWrapper(SourcePort as INodePort,
                                                      false);
                }
            }
            else
            {
                KnownSourcePort = null;
            }
            _mCurrentSource.InternalPort = KnownSourcePort;
        }

        private void TargetPortChanged()
        {
            if (TargetPort != null)
            {
                if (KnownTargetPort == null ||
                    KnownTargetPort.Source != TargetPort)
                {
                    KnownTargetPort =
                        SharedData.Graph
                                  .GetNodePortWrapper(TargetPort as INodePort,
                                                      false);
                }
            }
            else
            {
                KnownTargetPort = null;
            }
            _mCurrentTarget.InternalPort = KnownTargetPort;
        }

        protected override void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(name);
            if (!SharedData._unitchanging)
            {
                switch (name)
                {
                    case ConnectorConstants.SourceNode:
                        SourceNodeChanged();
                        break;
                    case ConnectorConstants.TargetNode:
                        TargetNodeChanged();
                        break;
                    case ConnectorConstants.SourcePort:
                        SourcePortChanged();
                        break;
                    case ConnectorConstants.TargetPort:
                        TargetPortChanged();
                        break;
                    case ConnectorConstants.SourcePoint:
                        IsEndPointChanged = true;
                        _mCurrentSource.Point = SourcePoint;
                        UpdateBounds();
                        UpdateFirstSegment();
                        CheckSegments();
                        break;
                    case ConnectorConstants.TargetPoint:
                        _mCurrentTarget.Point = TargetPoint;
                        UpdateBounds();
                        CheckSegments();
                        break;
                    case GroupableConstants.Bounds:
                        if (View != null)
                        {
                            (View as Control).InvalidateArrange();
                        }
                        break;
                    case ConnectorConstants.Segments:
                        OnSegmentsChanged();
                        break;
                }
            }
        }

        private void OnSegmentsChanged()
        {
            if (ConnectorSegments != null)
            {
                ConnectorSegments.Clear();
            }
            if (Segments != null)
            {
                if (ConnectorSegments != null && ConnectorSegments.Source == Segments)
                {
                    return;
                }
                ConnectorSegments = new ObservableElements<IConnectorSegment, SegmentWrapper>
                        (Segments, ElementType.Segment, SourceType.Segments, SharedData.EventAggregator, NewSeg);
                ConnectorSegments.Added += ConnectorSegments_Added;
                ConnectorSegments.Deleted += ConnectorSegments_Deleted;
                foreach (var segment in ConnectorSegments)
                {
                    ConnectorSegments_Added(new CollectionArgs<SegmentWrapper>(null, segment, ElementType.Segment,
                                                                               SourceType.Segments,
                                                                               ItemSource.UnKnown));
                }
            }
            if (View != null)
            {
                (View as UIElement).InvalidateArrange();
            }
        }

        void ConnectorSegments_Deleted(CollectionArgs<SegmentWrapper> obj)
        {
            RefreshConnectorSegment();
            //SegmentWrapper del = obj.Element;
            //if (del == FirstSegment)
            //{
            //    if (FirstSegment.Next != null)
            //    {
            //        FirstSegment.Next.Prev = null;
            //        FirstSegment = FirstSegment.Next as SegmentWrapper;
            //    }
            //    else
            //    {
            //        FirstSegment = null;
            //        LastSegment = null;
            //    }
            //}
            //else if(del == LastSegment)
            //{
            //    if (LastSegment.Prev != null)
            //    {
            //        LastSegment.Prev.Next = null;
            //        LastSegment = LastSegment.Prev as SegmentWrapper;
            //    }
            //    else
            //    {
            //        FirstSegment = null;
            //        LastSegment = null;
            //    }
            //}
            //else
            //{
            //    del.Prev.Next = del.Next;
            //    del.Next.Prev = del.Prev;
            //}
        }

        void ConnectorSegments_Added(CollectionArgs<SegmentWrapper> obj)
        {
            RefreshConnectorSegment();
            //return;
            //if (false && obj.Index.HasValue && Segments.Count > obj.Index)
            //{
            //    IConnectorSegment segment = Segments[obj.Index.Value];
            //    IInternalSegment next = ConnectorSegments.FirstOrDefault(item => item.Source == segment);
            //    if (next != null)
            //    {
            //        IInternalSegment prev = next.Prev;
            //        IInternalSegment newSegment = obj.Element;
            //        if (prev == null)
            //        {
            //            FirstSegment = newSegment as SegmentWrapper;
            //        }
            //        else
            //        {
            //            prev.Next = newSegment;
            //            newSegment.Prev = prev;
            //        }
            //        newSegment.Next = next;
            //        next.Prev = newSegment;
            //    }
            //}
            //else
            //{
            //    SegmentWrapper newElement = obj.Element;
            //    if (FirstSegment == null)
            //    {
            //        FirstSegment = newElement;
            //        LastSegment = newElement;
            //    }
            //    else
            //    {
            //        LastSegment.Next = newElement;
            //        newElement.Prev = LastSegment;
            //        LastSegment = newElement;
            //    }
            //}
        }

        private void RefreshConnectorSegment()
        {
            if (_editingThums && View != null && _orthoEditingFinished)
            {
                DisposeThums();
                _editingThums = true;
            }
            FirstSegment = null;
            LastSegment = null;
            foreach (IConnectorSegment segment in Segments)
            {
                if (FirstSegment == null)
                {
                    FirstSegment = ConnectorSegments.FirstOrDefault(item => item.Source == segment);
                    LastSegment = FirstSegment;
                }
                else
                {
                    LastSegment.Next = ConnectorSegments.FirstOrDefault(item => item.Source == segment);
                    LastSegment.Next.Prev = LastSegment;
                    LastSegment = LastSegment.Next as SegmentWrapper;
                }
            }
            if (_editingThums && View != null && _orthoEditingFinished)
            {
                PrepareThums();
                (View as UIElement).InvalidateArrange();
            }
        }

        private SegmentWrapper NewSeg(IConnectorSegment segment, bool canNew)
        {
            if (segment is ILineSegment)
            {
                return new LineWrapper(segment as ILineSegment, this, SharedData);
            }
            else if (segment is ILineSegmentLength)
            {
                return new LineLengthWrapper(segment as ILineSegmentLength, this, SharedData);
            }
            else if (segment is IOrthogonalSegment)
            {
                return new OrthoWrapper(segment as IOrthogonalSegment, this, SharedData);
            }
            else if (segment is IQuadraticCurveSegment)
            {
                return new QBezierWrapper(segment as IQuadraticCurveSegment, this, SharedData);
            }
            else if (segment is ICubicCurveSegment)
            {
                return new CBezierWrapper(segment as ICubicCurveSegment, this, SharedData);
            }
            throw new InvalidOperationException("Invalid segment");
        }

        protected override void OnGraphChanged()
        {
            base.OnGraphChanged();
            SourceNodeChanged();
            TargetNodeChanged();
            SourcePortChanged();
            TargetPortChanged();
            _mCurrentSource.Point = SourcePoint;
            _mCurrentTarget.Point = TargetPoint;
            OnSegmentsChanged();
            Point? start = null;
            Point? end = null;
            if (KnownSourceNode != null)
            {
                start = KnownSourceNode.Center;
            }
            if (KnownTargetNode != null)
            {
                end = KnownTargetNode.Center;
            }
            UpdateBounds(start, end);
        }

        private void UpdateBounds(Point? start = null, Point? end = null)
        {
            if (View != null)
            {
                (View as Control).InvalidateArrange();
            }
            else
            {
                SetBounds(new Rect(start ?? SourcePoint, end ?? TargetPoint));
            }
        }

        private IInternalNode _mKnownSourceNode;

        public IInternalNode KnownSourceNode
        {
            get { return _mKnownSourceNode; }
            set
            {
                if (_mKnownSourceNode != value)
                {
                    if (_mKnownSourceNode != null)
                    {
                        _mKnownSourceNode.BoundsChanged -= _mKnownSourceNode_BoundsChanged;
                    }
                    _mKnownSourceNode = value;
                    if (_mKnownSourceNode != null)
                    {
                        _mKnownSourceNode.BoundsChanged += _mKnownSourceNode_BoundsChanged;
                    }
                    if (value != null)
                    {
                        if (value.Source != SourceNode)
                        {
                            SourceNode = value.Source;
                        }
                    }
                    else
                    {
                        SourceNode = null;
                    }
                    _mCurrentSource.InternalNode = KnownSourceNode;
                }
            }
        }

        void _mKnownSourceNode_BoundsChanged(IInternalGroupable sender)
        {
            if ((sender as IInternalNode) == KnownSourceNode)
            {
                IsBoundsChanged = true;
                UpdateFirstSegment();
            }
            else
            {
            }
            CheckSegments();
        }

        void newV_BoundsChanged(IInternalGroupable obj)
        {

            if (obj == KnownSourceNode)
            {
                Point newPos = new Point(KnownSourceNode.OffsetX, KnownSourceNode.OffsetY);
                if (KnownTargetNode != null)
                {
                    UpdateBounds(newPos, new Point(KnownTargetNode.OffsetX, KnownTargetNode.OffsetY));
                }
                else
                {
                    UpdateBounds(newPos);
                }
            }
            else
            {
                Point newPos = new Point(KnownTargetNode.OffsetX, KnownTargetNode.OffsetY);
                if (KnownSourceNode != null)
                {
                    UpdateBounds(new Point(KnownSourceNode.OffsetX, KnownSourceNode.OffsetY), newPos);
                }
                else
                {
                    UpdateBounds(null, newPos);
                }
            }
        }

        private IInternalNode _mKnownTargetNode;

        public IInternalNode KnownTargetNode
        {
            get { return _mKnownTargetNode; }
            set
            {
                if (_mKnownTargetNode != value)
                {
                    if (_mKnownTargetNode != null)
                    {
                        _mKnownTargetNode.BoundsChanged -= _mKnownSourceNode_BoundsChanged;
                    }
                    _mKnownTargetNode = value;
                    if (_mKnownTargetNode != null)
                    {
                        _mKnownTargetNode.BoundsChanged += _mKnownSourceNode_BoundsChanged;
                    }
                    if (value != null)
                    {
                        if (value.Source != TargetNode)
                        {
                            TargetNode = value.Source;
                        }
                    }
                    else
                    {
                        TargetNode = null;
                    }
                    _mCurrentTarget.InternalNode = value;
                }
            }
        }

        private IInternalSegment selectedSegment;

        public IInternalSegment SelectedSegment
        {
            get { return selectedSegment; }
            set { selectedSegment = value; }
        }

        //private Point GetSourcePoint(IConnectorSegment segment)
        //{
        //    Point? segPoint = null;
        //    if (segment is ILineSegment)
        //    {
        //        segPoint = (segment as ILineSegment).Point;
        //    }
        //    else if (segment is ILineSegmentLength)
        //    {
        //        if (KnownSourcePort != null)
        //        {
        //        }
        //        if(KnownSourceNode != null)
        //        {
        //            if ((segment as ILineSegmentLength).Length.IsValid() && (segment as ILineSegmentLength).Angle.IsValid())
        //            {
        //                Point start = new Point(KnownSourceNode.OffsetX, KnownSourceNode.OffsetY);
        //                segPoint = start.Transform(
        //                    KnownSourceNode.ActualWidth + KnownSourceNode.ActualHeight,
        //                    (segment as ILineSegmentLength).Angle.ActualValue);
        //            }
        //        }
        //    }
        //    else if (segment is IOrthogonalSegment)
        //    {
        //        if (KnownSourcePort != null)
        //        {
        //        }
        //        if (KnownSourceNode != null)
        //        {
        //            Point start = new Point(KnownSourceNode.OffsetX, KnownSourceNode.OffsetY);
        //            segPoint = start.Transform(
        //                KnownSourceNode.ActualWidth + KnownSourceNode.ActualHeight,
        //                (segment as IOrthogonalSegment).Direction.ToAngle());
        //        }
        //    }
        //    else if (segment is IQuadraticCurveSegment || segment is ICubicCurveSegment)
        //    {
        //        Point end, endAdj;
        //        Point? adj;
        //        if (segment is IQuadraticCurveSegment)
        //        {
        //            adj = (segment as IQuadraticCurveSegment).Point1;
        //        }
        //        else
        //        {
        //            adj = (segment as ICubicCurveSegment).Point1;
        //        }

        //        if (adj != null)
        //        {
        //            if (KnownSourcePort != null)
        //            {
        //                end = new Point(KnownSourcePort.OffsetX, KnownTargetPort.OffsetY);
        //            }
        //            else if(KnownSourceNode != null)
        //            {
        //                end = KnownSourceNode.GetIntersection(adj.Value);
        //            }
        //            else
        //            {
        //                end = SourcePoint;
        //            }
        //        }
        //        else
        //        {
        //            GetBezierDirection(KnownSourcePort, KnownSourceNode, SourcePoint,
        //                           KnownTargetPort, KnownTargetNode, TargetPoint,
        //                           out end, out endAdj);
        //        }
        //        segPoint = end;
        //    }

        //    Point runPoint;
        //    if (KnownSourcePort != null)
        //    {
        //        runPoint = new Point(KnownSourcePort.OffsetX, KnownSourcePort.OffsetY);
        //    }
        //    else if (KnownSourceNode != null)
        //    {
        //        if (segPoint != null)
        //        {
        //            runPoint = KnownSourceNode.GetIntersection(segPoint.Value);
        //        }
        //        else if (KnownTargetPort != null)
        //        {
        //            runPoint = KnownSourceNode.GetIntersection(new Point(KnownTargetPort.OffsetX, KnownTargetNode.OffsetX));
        //        }
        //        else if (KnownTargetNode != null)
        //        {
        //            runPoint = KnownSourceNode.GetIntersection(KnownTargetNode);
        //        }
        //        else
        //        {
        //            runPoint = KnownSourceNode.GetIntersection(TargetPoint);
        //        }
        //    }
        //    else
        //    {
        //        runPoint = SourcePoint;
        //    }
        //    return runPoint;
        //}

        public void UpdateGeometry()
        {
            if (!isRoutted)
            {
                //CheckSegments();
                if (TerminalSegment != null)
                {
                    foreach (var internalSegment in TerminalSegment.ToList())
                    {
                        (internalSegment as SegmentWrapper).Recycle(internalSegment);
                        if (internalSegment is OrthoWrapper)
                        {
                            (internalSegment as OrthoWrapper).RefereshThumb();
                        }
                        TerminalSegment.Remove(internalSegment);
                    }
                }
                InternalSegments.Clear();
                if (Segments == null || Segments.Count < 1)
                {
                    Segments = InializeSegments();
                    if (_editingThums)
                    {
                        PrepareThums();
                    }
                }
                double angle;
                RunPoint = null;
                RunPoint = FirstSegment.GetSourcePoint(out angle); //GetSourcePoint(FirstSegment.Source as IConnectorSegment);
                RunAngle = angle;
                PathFigure.StartPoint = RunPoint.Value;
                foreach (SegmentWrapper seg in FirstSegment)
                {
                    if (!seg.AddView())
                    {
                        break;
                    }
                    if (seg.Next == null)
                    {
                        if (seg is IOrthogonalSegment)
                        {
                            IConnectorSegment term1 = GetNewSegment(seg);
                            SegmentWrapper wrapper1 = NewSeg(term1, true);
                            wrapper1.Prev = seg;
                            wrapper1.AddView();
                            break;
                        }
                        IConnectorSegment term = GetNewSegment(seg);
                        SegmentWrapper wrapper = NewSeg(term, true);
                        ConnectorSegments.Add(wrapper, ItemSource.UnKnown);
                        wrapper.Prev = seg;
                        wrapper.AddView();
                        break;
                        //seg.Terminate();
                    }
                    #region OLD

                    /*var connectorSegment = seg.Source as IConnectorSegment;
                    lastSegment = connectorSegment;
                    if (connectorSegment is ILineSegment)
                    {
                        ILineSegment lineSegment = connectorSegment as ILineSegment;
                        #region Null Run
                        if (runPoint == null)
                        {
                            if (lineSegment.Point != null)
                            {
                                runPoint = KnownSourcePort != null
                                               ? new Point(KnownSourcePort.OffsetX, KnownSourcePort.OffsetY)
                                               : KnownSourceNode != null
                                                     ? KnownSourceNode.GetIntersection(lineSegment.Point.Value)
                                                 : SourcePoint;
            }
                        else
                            {
                                runPoint =
                                    KnownSourcePort != null
                                        ? new Point(KnownSourcePort.OffsetX, KnownSourcePort.OffsetY)
                                        : KnownSourceNode != null
                                              ? KnownTargetPort != null
                                                    ? KnownSourceNode.GetIntersection(new Point(KnownTargetPort.OffsetX,
                                                                                                  KnownTargetPort.OffsetY))
                                                    : KnownTargetNode != null
                                                          ? KnownSourceNode.GetIntersection(KnownTargetNode)
                                                          : KnownSourceNode.GetIntersection(TargetPoint)
                                              : SourcePoint;
                            }
                            PathFigure.StartPoint = runPoint.Value;
                        } 
                        #endregion
                        if (lineSegment.Point != null)
                        {
                            AddLineSegment(ref runPoint, ref runAngle, lineSegment.Point.Value);
                        }
                        else
                        {
                            TerminateStraightConnection(ref runPoint, ref runAngle);
                            isTerminated = true;
                            break;
                        }
                    }
                    else if (connectorSegment is ILineSegmentLength)
                    {
                        ILineSegmentLength segmentLength = connectorSegment as ILineSegmentLength;
                        #region Null Run
                        if (runPoint == null)
                        {
                            if (segmentLength.Length.IsValid() && segmentLength.Angle.IsValid())
                            {

                                Point start = new Point(0, 0);
                                if (KnownSourcePort != null)
                                {
                                    start = new Point(KnownSourcePort.OffsetX, KnownSourcePort.OffsetY);
                                }
                                else
                                {
                                    start = new Point(KnownSourceNode.OffsetX, KnownSourceNode.OffsetY);
                                }
                                start = start.Transform(
                                    KnownSourceNode.ActualWidth + KnownSourceNode.ActualHeight,
                                    segmentLength.Angle.ActualValue);
                                runPoint = KnownSourceNode.GetIntersection(start);
                            }
                            else
                            {
                                runPoint = KnownSourcePort != null
                                        ? new Point(KnownSourcePort.OffsetX, KnownSourcePort.OffsetY)
                                        : KnownSourceNode != null
                                              ? KnownTargetPort != null
                                                    ? KnownSourceNode.GetIntersection(new Point(KnownTargetPort.OffsetX,
                                                                                                  KnownTargetPort.OffsetY))
                                                    : KnownTargetNode != null
                                                          ? KnownSourceNode.GetIntersection(KnownTargetNode)
                                                          : KnownSourceNode.GetIntersection(TargetPoint)
                                              : SourcePoint;
                            }
                            PathFigure.StartPoint = runPoint.Value;
                        } 
                        #endregion
                        if (segmentLength.Angle.Value.IsValid() && segmentLength.Length.Value.IsValid())
                        {
                            double angle = segmentLength.Angle.ActualValue;
                            if (segmentLength.AngleMode == RelativeMode.Relative)
                            {
                                angle += runAngle;
                            }
                            double radius = segmentLength.Length.ActualValue;
                            AddLineSegment(ref runPoint, ref runAngle, radius, angle);
                        }
                        else
                        {
                            TerminateStraightConnection(ref runPoint, ref runAngle);
                            isTerminated = true;
                            break;
                        }
                    }
                    else if (connectorSegment is IOrthogonalSegment)
                    {
                        IOrthogonalSegment orthoSegment = connectorSegment as IOrthogonalSegment;
                        double segAngle = orthoSegment.Direction.ToAngle(runAngle);
                        #region Null Run
                        if (runPoint == null)
                        {
                            if (orthoSegment.Length.IsValid())
                            {
                                if (!segAngle.IsValid())
                                {
                                    throw new NotImplementedException();
                                }
                                Point start = new Point(0, 0);
                                if (KnownSourcePort != null)
                                {
                                    start = new Point(KnownSourcePort.OffsetX, KnownSourcePort.OffsetY);
                                }
                                else
                                {
                                    start = new Point(KnownSourceNode.OffsetX, KnownSourceNode.OffsetY);
                                }
                                start = start.Transform(
                                    KnownSourceNode.Width + KnownSourceNode.Height,
                                    segAngle);
                                runPoint = KnownSourceNode.GetIntersection(start);
                                PathFigure.StartPoint = runPoint.Value;
                            }
                            else
                            {
                                TerminateOrthoConnection(ref runPoint, ref runAngle,
                                                         SourcePoint, KnownSourceNode,
                                                         KnownSourcePort,
                                                         TargetPoint, KnownTargetNode,
                                                         KnownTargetPort);
                                isTerminated = true;
                                break;
                            }
                        } 
                        #endregion
                        if (orthoSegment.Length.IsValid())
                        {
                            double radius = orthoSegment.Length.ActualValue;
                            double ang = orthoSegment.Direction.ToAngle(runAngle);
                            if (!ang.IsValid())
                            {
                                throw new NotImplementedException();
                            }
                            AddLineSegment(ref runPoint, ref runAngle, radius, ang);
                        }
                        else
                        {
                            TerminateOrthoConnection(ref runPoint, ref runAngle,
                                                     SourcePoint, KnownSourceNode,
                                                     KnownSourcePort,
                                                     TargetPoint, KnownTargetNode,
                                                     KnownTargetPort);
                            isTerminated = true;
                            break;
                        }
                    }
                    else if (connectorSegment is ICubicCurveSegment)
                    {
                        ICubicCurveSegment quadB = connectorSegment as ICubicCurveSegment;
                        Point p1, p2, p3;
                        if (quadB.Point1 == null || quadB.Point2 == null || quadB.Point3 == null)
                        {
                            TerminateCBezierConnection(ref runPoint, quadB.Point1, quadB.Point2, quadB.Point3);
                            isTerminated = true;
                            break;
                        }
                        else
                        {
                            p1 = quadB.Point1.Value;
                            p2 = quadB.Point2.Value;
                            p3 = quadB.Point3.Value;
                        }
                        AddCBezierSegment(p1, p2, p3);
                    }
                    else if (connectorSegment is IQuadraticCurveSegment)
                    {
                        IQuadraticCurveSegment quadB = connectorSegment as IQuadraticCurveSegment;
                        Point p1, p2;
                        if (quadB.Point1 == null || quadB.Point2 == null)
                        {
                            TerminateQBezierConnection(ref runPoint, quadB.Point1, quadB.Point2);
                            isTerminated = true;
                            break;
                        }
                        else
                        {
                            p1 = quadB.Point1.Value;
                            p2 = quadB.Point2.Value;
                        }
                        AddQBezierSegment(p1, p2);
                    }*/
                    #endregion
                }
                /*if (!isTerminated)
                {
                    if (lastSegment is IOrthogonalSegment)
                    {
                        TerminateOrthoConnection(ref runPoint, ref runAngle,
                                                 SourcePoint, KnownSourceNode,
                                                 KnownSourcePort,
                                                 TargetPoint, KnownTargetNode,
                                                 KnownTargetPort);
                    }
                    else if (lastSegment is IQuadraticCurveSegment)
                    {
                        TerminateQBezierConnection(ref runPoint, null, null);
                    }
                    else if (lastSegment is ICubicCurveSegment)
                    {
                        TerminateCBezierConnection(ref runPoint, null, null, null);
                    }
                    else
                    {
                        TerminateStraightConnection(ref runPoint, ref runAngle);
                    }
                }*/
                if (SourcePoint != PathFigure.StartPoint)
                {
                    SourcePoint = PathFigure.StartPoint;
                }
                if (RunPoint != null && TargetPoint != RunPoint)
                {
                    TargetPoint = RunPoint.Value;
                }
            }
            else
            {
                PathFigure.StartPoint = SourcePoint;
            }
            SetBounds(Connector.Geometry.Bounds);
            if (this.IsSelected)
            {
                SharedData.Adorner.UpdatePreview(this);
            }
            RunPoint = null;
            if (_editingThums)
            {
                if (SharedData.Adorner != null)
                {
                    SharedData.Adorner.InvalidateArrange();
                }
            }
        }

        private void UpdateFirstSegment()
        {
            OrthoWrapper first, second;
            Point? startpoint;
            if (FirstSegment != null && (IsEndPointChanged || IsBoundsChanged))
            {
                if (FirstSegment is OrthoWrapper)
                {
                    first = FirstSegment as OrthoWrapper;
                    if (first.Next != null)
                    {
                        if (first.Next is OrthoWrapper)
                        {
                            second = first.Next as OrthoWrapper;
                            double ang;
                            startpoint = FirstSegment.GetSourcePoint(out ang);
                            first.Length = GetLength(first, startpoint);
                            second.Length = GetLength(second, startpoint);
                            if (first.EndPoint != null && SourceNode != null && KnownSourceNode.Bounds != null &&
                                KnownSourcePort == null && second != null)
                            {
                                double difflenght = 0, difflength1 = 0;
                                if (KnownSourceNode.Bounds.Contains(first.EndPoint.Value))
                                {
                                    if (IsSegmentAdded)
                                    {
                                        RemoveSegments(FirstSegment.First() as SegmentWrapper);
                                        if (second.Direction == OrthogonalDirection.Left ||
                                            second.Direction == OrthogonalDirection.Right)
                                        {
                                            difflenght = KnownSourceNode.Bounds.Height / 2;
                                            difflength1 = KnownSourceNode.Bounds.Width / 2;
                                        }
                                        else
                                        {
                                            difflenght = KnownSourceNode.Bounds.Width / 2;
                                            difflength1 = KnownSourceNode.Bounds.Height / 2;
                                        }
                                        if (second.Next != null)
                                        {
                                            OrthoWrapper third = second.Next as OrthoWrapper;
                                            if (third.Length.Value >= 0)
                                            {
                                                third.Length = third.Length + difflength1;
                                            }
                                            else
                                            {
                                                third.Length = third.Length - difflength1;
                                            }
                                        }
                                        IsSegmentAdded = false;
                                    }
                                    if (second.Length.Value >= 0)
                                    {
                                        second.Length = second.Length.Value - difflenght;
                                    }
                                    else
                                    {
                                        second.Length = second.Length.Value + difflenght;
                                    }
                                }
                            }
                        }
                    }
                }
                IsBoundsChanged = false;
                IsEndPointChanged = false;
            }
        }

        private double GetLength(OrthoWrapper segment, Point? start)
        {
            double length1 = 0;
            if ((segment.Direction == OrthogonalDirection.Right || segment.Direction == OrthogonalDirection.Left) && segment.EndPoint != null)
            {
                length1 = segment.EndPoint.Value.X - start.Value.X;
                if (length1 >= 0)
                {
                    segment.Direction = OrthogonalDirection.Right;
                }
                else
                {
                    segment.Direction = OrthogonalDirection.Left;
                }
            }
            else if (segment.EndPoint != null)
            {
                length1 = segment.EndPoint.Value.Y - start.Value.Y;
                if (length1 >= 0)
                {
                    segment.Direction = OrthogonalDirection.Bottom;
                }
                else
                {
                    segment.Direction = OrthogonalDirection.Top;
                }
            }
            length1 = length1 >= 0 ? length1 : -length1;
            return length1;
        }

        private void UpdateLastSegment()
        {
            double length = 0;
            OrthoWrapper last;
            if (FirstSegment != null && KnownTargetNode != null &&
                TerminalSegment.Count > 0 && FirstSegment.Count() > 0)
            {
                if (FirstSegment.Last() is OrthoWrapper && TerminalSegment.First() is OrthoWrapper
                    && (IsEndPointChanged || IsBoundsChanged))
                {
                    OrthoWrapper last1 = FirstSegment.Last() as OrthoWrapper;
                    OrthoWrapper last2 = TerminalSegment.First() as OrthoWrapper;
                    if (last1.EndPoint != null && (last1.Prev == null || last1.Prev is OrthoWrapper))
                    {
                        if (last1.EndPoint.Value.X == last2.EndPoint.Value.X)
                        {
                            if (last1.Direction == OrthogonalDirection.Top ||
                                last1.Direction == OrthogonalDirection.Bottom)
                            {
                                RemoveSegments(last1);
                            }
                        }
                        else if (last1.EndPoint.Value.Y == last2.EndPoint.Value.Y)
                        {
                            if (last1.Direction == OrthogonalDirection.Right ||
                                last1.Direction == OrthogonalDirection.Left)
                            {
                                RemoveSegments(last1);
                            }
                        }
                        else if (TerminalSegment.Count == 1)
                        {
                            RemoveSegments(last1);
                        }
                    }
                }

                //if (FirstSegment.Last() is OrthoWrapper && TerminalSegment.First() is OrthoWrapper
                //    && TerminalSegment.Last() is OrthoWrapper)
                //{
                //    //last = FirstSegment.Last() as OrthoWrapper;
                //TerminalLast = TerminalSegment.Last() as OrthoWrapper;
                //TerminalFirst = TerminalSegment.First() as OrthoWrapper;
                //if (last.Direction == OrthogonalDirection.Right || last.Direction == OrthogonalDirection.Left)
                //{
                //    if (last.EndPoint.Value.Y == TerminalFirst.EndPoint.Value.Y)
                //    {
                //        length = last.StartPoint.Value.X - TerminalLast.EndPoint.Value.X;
                //        if (length >= 0)
                //        {
                //            last.Direction = OrthogonalDirection.Left;
                //        }
                //        else
                //        {
                //            last.Direction = OrthogonalDirection.Right;
                //        }
                //        last.Length = length;
                //    }
                //}
                //else
                //{
                //    if (last.EndPoint.Value.X == TerminalFirst.EndPoint.Value.X)
                //    {
                //        length = last.StartPoint.Value.Y - TerminalLast.EndPoint.Value.Y;
                //        if (length >= 0)
                //        {
                //            last.Direction = OrthogonalDirection.Top;
                //        }
                //        else
                //        {
                //            last.Direction = OrthogonalDirection.Bottom;
                //        }
                //        last.Length = length;
                //    }
                //}

                //}
                if (FirstSegment != null)
                {
                    if (FirstSegment.Last() is OrthoWrapper && KnownTargetNode.Bounds != null && KnownTargetPort == null)
                    {
                        last = FirstSegment.Last() as OrthoWrapper;
                        if (KnownTargetNode.Bounds.Contains(last.EndPoint.Value))
                        {
                            if (last.Direction == OrthogonalDirection.Right ||
                                last.Direction == OrthogonalDirection.Left)
                            {
                                if (last.StartPoint.Value.X >= KnownTargetNode.Bounds.Right)
                                {
                                    length = KnownTargetNode.Bounds.Right - last.EndPoint.Value.X + 25;
                                    last.Length = GetLength1(last.Length.Value, length);
                                }
                                else
                                {
                                    length = last.EndPoint.Value.X - KnownTargetNode.Bounds.Left - 25;
                                    last.Length = GetLength1(last.Length.Value, length);
                                }
                            }
                            else
                            {
                                if (last.StartPoint.Value.Y >= KnownTargetNode.Bounds.Bottom)
                                {
                                    length = KnownTargetNode.Bounds.Bottom - last.EndPoint.Value.Y + 25;
                                    last.Length = GetLength1(last.Length.Value, length);
                                }
                                else
                                {
                                    length = last.EndPoint.Value.Y - KnownTargetNode.Bounds.Top - 25;
                                    last.Length = GetLength1(last.Length.Value, length);
                                }
                            }
                        }
                    }
                }
            }
            IsEndPointChanged = false;
            IsBoundsChanged = false;
        }

        private double GetLength1(double length, double diff)
        {
            double l = 0;
            if (length >= 0)
            {
                l = length - diff;
            }
            else
            {
                l = length + diff;
            }
            return l;
        }


        internal void CheckSegments()
        {
            if (FirstSegment != null)
            {
                if (FirstSegment.Count() > 0)
                {
                    if (TerminalSegment.Count() > 0)
                    {
                        if (FirstSegment.Last() is OrthoWrapper && TerminalSegment.First() is OrthoWrapper)
                        {
                            OrthoWrapper last1 = FirstSegment.Last() as OrthoWrapper;
                            OrthoWrapper last2 = TerminalSegment.First() as OrthoWrapper;
                            if (last1.EndPoint != null && (last1.Prev == null || last1.Prev is OrthoWrapper)
                               && (KnownSourceNode != KnownTargetNode ||
                              (KnownSourceNode == null && KnownTargetNode == null)))
                            {
                                if ((last1.EndPoint.Value.X == last2.EndPoint.Value.X) &&
                                    (last1.StartPoint.Value.X == last2.StartPoint.Value.X))
                                {
                                    RemoveSegments(last1);
                                }
                                else if ((last1.EndPoint.Value.Y == last2.EndPoint.Value.Y) &&
                                    (last1.StartPoint.Value.Y == last2.StartPoint.Value.Y))
                                {
                                    RemoveSegments(last1);
                                }
                                else if (TerminalSegment.Count == 1)
                                {
                                    //RemoveSegments(last1);
                                }
                            }
                        }
                    }

                    if (FirstSegment is OrthoWrapper)
                    {
                        OrthoWrapper first = FirstSegment as OrthoWrapper;

                        if (first.EndPoint != null && SourceNode != null && KnownSourceNode.Bounds != null && KnownSourcePort == null)
                        {
                            UpdateAdditionalSegments(first);
                        }
                    }
                }
            }
        }

        private void UpdateAdditionalSegments(OrthoWrapper first)
        {
            IInternalSegment next2 = null;
            next2 = first.Next != null ? first.Next : null;
            double length = 0, difflenght = 0;
            if (next2 != null)
            {
                if (next2 is OrthoWrapper)
                {
                    OrthoWrapper second = next2 as OrthoWrapper;
                    if (KnownSourceNode.Bounds.Contains(first.EndPoint.Value))
                    {
                        if (second.Direction == OrthogonalDirection.Left ||
                            second.Direction == OrthogonalDirection.Right)
                        {
                            IConnectorSegment term1 = GetNewSegment(first);
                            SegmentWrapper wrapper1 = NewSeg(term1, true);
                            length = second.EndPoint.Value.Y -
                                     (KnownSourceNode.Bounds.Bottom - KnownSourceNode.Bounds.Height / 2);
                            first.Length = length >= 0 ? length : -length;
                            if (length >= 0)
                            {
                                first.Direction = OrthogonalDirection.Bottom;
                            }
                            else
                            {
                                first.Direction = OrthogonalDirection.Top;
                            }
                            (term1 as IOrthogonalSegment).Length = 25;
                            if (second.EndPoint.Value.X > KnownSourceNode.Bounds.Left &&
                                second.EndPoint.Value.X > KnownSourceNode.Bounds.Right)
                            {
                                (term1 as IOrthogonalSegment).Direction = OrthogonalDirection.Right;
                            }
                            else
                            {
                                (term1 as IOrthogonalSegment).Direction = OrthogonalDirection.Left;
                            }
                            wrapper1.Next = first;
                            first.Prev = wrapper1;
                            difflenght = KnownSourceNode.Bounds.Width / 2 + 25;
                            _orthoEditingFinished = false;
                            ConnectorSegments.Insert(wrapper1, ItemSource.UnKnown, 0);
                            wrapper1.PrepareThumb();
                            _orthoEditingFinished = true;
                        }
                        else
                        {
                            IConnectorSegment term1 = GetNewSegment(first);
                            SegmentWrapper wrapper1 = NewSeg(term1, true);
                            length = second.EndPoint.Value.X -
                                     (KnownSourceNode.Bounds.Right - KnownSourceNode.Bounds.Width / 2);
                            first.Length = length >= 0 ? length : -length;
                            if (length >= 0)
                            {
                                first.Direction = OrthogonalDirection.Right;
                            }
                            else
                            {
                                first.Direction = OrthogonalDirection.Left;
                            }
                            (term1 as IOrthogonalSegment).Length = 25;
                            if (second.EndPoint.Value.Y > KnownSourceNode.Bounds.Top &&
                                second.EndPoint.Value.Y > KnownSourceNode.Bounds.Bottom)
                            {
                                (term1 as IOrthogonalSegment).Direction = OrthogonalDirection.Bottom;
                            }
                            else
                            {
                                (term1 as IOrthogonalSegment).Direction = OrthogonalDirection.Top;
                            }
                            wrapper1.Next = first;
                            first.Prev = wrapper1;
                            difflenght = KnownSourceNode.Bounds.Height / 2 + 25;
                            _orthoEditingFinished = false;
                            ConnectorSegments.Insert(wrapper1, ItemSource.UnKnown, 0);
                            wrapper1.PrepareThumb();
                            _orthoEditingFinished = true;
                        }
                        IsSegmentAdded = true;
                    }
                    if (second.Length.Value >= 0)
                    {
                        second.Length = second.Length.Value - difflenght;
                    }
                    else
                    {
                        second.Length = second.Length.Value + difflenght;
                    }
                }
            }
        }

        public void PrepareThums()
        {
            _editingThums = true;
            if (!Constraints.Contains(ConnectorConstraints.SegmentThumbs))
            {
                return;
            }
            if (FirstSegment != null)
            {
                foreach (SegmentWrapper segment in FirstSegment)
                {
                    segment.PrepareThumb(segment == LastSegment);
                }
            }
            if (TerminalSegment != null)
            {
                foreach (SegmentWrapper internalSegment in TerminalSegment)
                {
                    internalSegment.PrepareThumb();
                }
            }
        }

        public void UpdateThums()
        {
            if (!Constraints.Contains(ConnectorConstraints.SegmentThumbs))
            {
                return;
            }
            if (FirstSegment != null)
            {
                foreach (SegmentWrapper segment in FirstSegment)
                {
                    segment.UpdateThumb();
                }
            }
            if (TerminalSegment != null)
            {
                foreach (SegmentWrapper internalSegment in TerminalSegment)
                {
                    internalSegment.UpdateThumb();
                }
            }
        }

        public void DisposeThums()
        {
            _editingThums = false;
            if (!Constraints.Contains(ConnectorConstraints.SegmentThumbs))
            {
                return;
            }
            SelectedSegment = null;
            if (FirstSegment != null)
            {
                foreach (SegmentWrapper segment in FirstSegment)
                {
                    segment.DisposeThumb();
                }
            }
            if (TerminalSegment != null)
            {
                foreach (SegmentWrapper internalSegment in TerminalSegment)
                {
                    internalSegment.DisposeThumb();
                }
            }
        }

        internal void AddEndSegments(IInternalSegment segment)
        {
            _orthoEditingFinished = false;
            SegmentWrapper _wrapper = null;
            if (ConnectorSegments.Count > 0)
            {
                _wrapper = ConnectorSegments.Last();
                if (!(_wrapper as OrthoWrapper).Length.IsValid())
                {
                    ConnectorSegments.Remove(_wrapper);
                    _wrapper.DisposeThumb();
                }
            }
            int index = TerminalSegment.IndexOf(segment as SegmentWrapper);
            for (int i = 0; i <= index; i++)
            {
                OrthoWrapper _item = TerminalSegment.ElementAt(i) as OrthoWrapper;
                double lenght = _item.Length.Value;
                OrthogonalSegment _segment = new OrthogonalSegment()
                     {
                         Length = new DoubleExt(Double.NaN),
                         Direction = _item.Direction
                     };
                _segment.Length = _item.Length;
                _segment.Direction = _item.Direction;
                _item.SetSource(_segment);
                ConnectorSegments.Add(_item, ItemSource.UnKnown);
            }
            for (int i = index; i >= 0; i--)
            {
                TerminalSegment.RemoveAt(i);
            }
            Connector.InvalidateArrange();
            _orthoEditingFinished = true;
        }

        internal void AddStartSegments(IInternalSegment segment)
        {
            OrthoWrapper ortho = segment as OrthoWrapper;
            _orthoEditingFinished = false;
            IConnectorSegment term1 = GetNewSegment(segment);
            SegmentWrapper wrapper1 = NewSeg(term1, true);

            IConnectorSegment term2 = GetNewSegment(segment);
            SegmentWrapper wrapper2 = NewSeg(term2, true);
            (term1 as IOrthogonalSegment).Length = ortho.Length / 4;
            (term2 as IOrthogonalSegment).Length = 5;
            if (ortho.Direction == OrthogonalDirection.Top || ortho.Direction == OrthogonalDirection.Bottom)
            {
                (term1 as IOrthogonalSegment).Direction = ortho.Direction;
                (term2 as IOrthogonalSegment).Direction = OrthogonalDirection.Right;
            }
            else
            {
                (term1 as IOrthogonalSegment).Direction = ortho.Direction;
                (term2 as IOrthogonalSegment).Direction = OrthogonalDirection.Bottom;
            }
            wrapper2.PrepareThumb();
            wrapper1.PrepareThumb();
            if (ortho._mNext != null)
            {
                if (ortho._mNext is OrthoWrapper)
                {
                    OrthoWrapper next = ortho._mNext as OrthoWrapper;
                    next.Length = GetLength1(next.Length.Value, 5);
                }
            }
            ConnectorSegments.Insert(wrapper2, ItemSource.UnKnown, 0);
            ConnectorSegments.Insert(wrapper1, ItemSource.UnKnown, 0);
            Connector.InvalidateArrange();
            _orthoEditingFinished = true;
        }

        private void RemoveSegments(SegmentWrapper last1)
        {
            _orthoEditingFinished = false;
            ConnectorSegments.Remove(last1);
            last1.DisposeThumb();
            if (last1.Prev != null)
            {
                last1.Prev.Next = null;
            }
            if (last1.Next != null)
            {
                last1.Next.Prev = null;
            }
            last1.Prev = null;
            last1.Next = null;
            _orthoEditingFinished = true;
        }

        internal void RemoveInValidSegments()
        {
            OrthoWrapper previous = null, previous1 = null, next = null, last = null, current1 = null, current = null;
            double length = 0;
            Point? startpoint;
            _orthoEditingFinished = false;
            if (FirstSegment != null)
            {
                foreach (SegmentWrapper seg in FirstSegment)
                {
                    if (seg is OrthoWrapper)
                    {
                        length = (seg as OrthoWrapper).Length.Value;
                        length = length >= 0 ? length : -length;
                        if (length < 5)
                        {
                            current = seg as OrthoWrapper;
                            previous = current.Prev as OrthoWrapper;
                            next = current.Next as OrthoWrapper;
                            if (previous != null)
                            {
                                previous1 = previous.Prev as OrthoWrapper;
                            }
                            if (next != null)
                            {
                                last = next.Next as OrthoWrapper;
                                RemoveSegments(next);
                            }
                            RemoveSegments(current);
                        }
                    }
                }
            }
            if (previous1 != null)
            {
                startpoint = previous1.EndPoint;
            }
            else
            {
                startpoint = PathFigure.StartPoint;
            }
            if (TerminalSegment.Count() > 0 && FirstSegment != null)
            {
                current1 = TerminalSegment.First() as OrthoWrapper;
                length = current1.Length.Value;
                length = length >= 0 ? length : -length;
                if (next == null)
                {
                    current1.DisposeThumb();
                }
                if (length < 5)
                {
                    RemoveSegments(FirstSegment.Last() as SegmentWrapper);
                }
            }
            if (previous != null)
            {
                if (previous.Direction == OrthogonalDirection.Right ||
                    previous.Direction == OrthogonalDirection.Left)
                {
                    double diff;
                    double diff1;
                    if (last != null)
                    {
                        diff = last.EndPoint.Value.X - startpoint.Value.X;
                        diff1 = last.EndPoint.Value.Y - startpoint.Value.Y;
                    }
                    else
                    {
                        diff = TargetPoint.X - startpoint.Value.X;
                        diff1 = TargetPoint.Y - startpoint.Value.Y;
                    }
                    if (diff >= 0)
                    {
                        previous.Direction = OrthogonalDirection.Right;
                    }
                    else
                    {
                        previous.Direction = OrthogonalDirection.Left;
                    }
                    if (last != null)
                    {
                        if (diff1 >= 0)
                        {
                            last.Direction = OrthogonalDirection.Bottom;
                        }
                        else
                        {
                            last.Direction = OrthogonalDirection.Top;
                        }
                        last.Length = diff1 >= 0 ? diff1 : -diff1;
                    }
                    previous.Length = diff >= 0 ? diff : -diff;
                }
                else
                {
                    double diff;
                    double diff1;
                    if (last != null)
                    {
                        diff = last.EndPoint.Value.Y - startpoint.Value.Y;
                        diff1 = last.EndPoint.Value.X - startpoint.Value.X;
                    }
                    else
                    {
                        diff = TargetPoint.Y - startpoint.Value.Y;
                        diff1 = TargetPoint.X - startpoint.Value.X;
                    }
                    if (diff >= 0)
                    {
                        previous.Direction = OrthogonalDirection.Bottom;
                    }
                    else
                    {
                        previous.Direction = OrthogonalDirection.Top;
                    }
                    if (last != null)
                    {
                        if (diff1 >= 0)
                        {
                            last.Direction = OrthogonalDirection.Right;
                        }
                        else
                        {
                            last.Direction = OrthogonalDirection.Left;
                        }
                        last.Length = diff1 >= 0 ? diff1 : -diff1;
                    }
                    previous.Length = diff >= 0 ? diff : -diff;
                }
            }
            RefreshConnectorSegment();
            _orthoEditingFinished = true;
            Connector.InvalidateArrange();
        }

        public IConnectorSegments InializeSegments()
        {
            ConnectorSegments segments = new ConnectorSegments();
            if (SharedData != null && SharedData.Graph != null)
            {
                switch (SharedData.Graph.DefaultConnectorType)
                {
                    case ConnectorType.Line:
                        segments.Add(NewConnectorSegment<ILineSegmentLength>());
                        break;
                    case ConnectorType.Orthogonal:
                        segments.Add(NewConnectorSegment<IOrthogonalSegment>());
                        break;
                    case ConnectorType.QuadraticBezier:
                        segments.Add(NewConnectorSegment<IQuadraticCurveSegment>());
                        break;
#if WINRT
                    case ConnectorType.PolyCubicBezier:
#endif
                    case ConnectorType.CubicBezier:
                        segments.Add(NewConnectorSegment<ICubicCurveSegment>());
                        break;
                }
            }
            return segments;
        }

        private IConnectorSegment GetNewSegment(IConnectorSegment seg)
        {
            if (seg is ILineSegment)
            {
                return new StraightSegment();
            }
            else if (seg is ILineSegmentLength)
            {
                return new LineSegmentLength();
            }
            else if (seg is IOrthogonalSegment)
            {
                return new OrthogonalSegment();
            }
            else if (seg is IQuadraticCurveSegment)
            {
                return new QuadraticCurveSegment();
            }
            else if (seg is ICubicCurveSegment)
            {
                return new CubicCurveSegment();
            }
            throw new InvalidOperationException("Invalid Segment");
        }

        public TSegment NewConnectorSegment<TSegment>() where TSegment : IConnectorSegment
        {
            object temp = null;
            Type target = typeof(TSegment);
            if (target == typeof(IOrthogonalSegment))
            {
                temp = new OrthogonalSegment();
            }
            else if (target == typeof(ILineSegment))
            {
                temp = new StraightSegment();
            }
            else if (target == typeof(ILineSegmentLength))
            {
                temp = new LineSegmentLength();
            }
            else if (target == typeof(IQuadraticCurveSegment))
            {
                temp = new QuadraticCurveSegment();
            }
            else if (target == typeof(ICubicCurveSegment))
            {
                temp = new CubicCurveSegment();
            }
            TSegment newSegment = (TSegment)temp;
            return newSegment;
        }

        internal void AddLineSegment(ref Point? runPoint, ref double runAngle, Point point)
        {
            runAngle = runPoint.Value.FindAngle(point);
            runPoint = point;
            InternalSegments.Add(new LineSegment() { Point = point });
        }

        internal void InsertLineSegment(ref Point? runPoint, ref double runAngle, Point point, int index)
        {
            runAngle = runPoint.Value.FindAngle(point);
            runPoint = point;
            InternalSegments.Insert(index, new LineSegment() { Point = point });
        }

        public PathSegmentCollection InternalSegments
        {
            get
            {
                if (Connector != null)
                {
                    return Connector._mInternalSegments;
                }
                return null;
            }
        }

        private IInternalNodePort _knownSourcePort;
        private IInternalNodePort _knownTargetPort;

        public PathFigure PathFigure
        {
            get { return Connector._mPathFigure; }
        }

        public void UpdateDecorator()
        {
            throw new NotImplementedException();
        }

        public IInternalNodePort KnownSourcePort
        {
            get { return _knownSourcePort; }
            set
            {
                if (_knownSourcePort != value)
                {
                    _knownSourcePort = value;
                    if (_knownSourcePort != null)
                    {
                        if (_knownSourcePort.Source != SourcePort)
                        {
                            SourcePort = (IPort)_knownSourcePort.Source;
                            KnownSourceNode = _knownSourcePort.KnownNode;
                        }
                    }
                    else
                    {
                        SourcePort = null;
                    }
                }
            }
        }

        public DragState SourceDragState
        {
            get { return _mCurrentSource.DragState; }
            set
            {
                _mCurrentSource.DragState = value;
                OnDragStateChanged("SourceDragState");
            }
        }

        private void OnDragStateChanged(string p)
        {
            switch (p)
            {
                case "SourceDragState":
                    if (SourceDragState == DragState.Started)
                    {
                        if (FirstSegment != null)
                        {
                            if (SelectedSegment != FirstSegment.First())
                            {
                                if (SelectedSegment != null)
                                {
                                    (SelectedSegment as SegmentWrapper).DisposeThumb();
                                    if (SelectedSegment.Next != null)
                                    {
                                        (SelectedSegment.Next as SegmentWrapper).DisposeThumb();
                                    }
                                }
                                SelectedSegment = FirstSegment.First();
                                (SelectedSegment as SegmentWrapper).PrepareThumb();

                            }
                        }
                        else
                        {
                            if (SelectedSegment != null && SelectedSegment.Next != null)
                            {
                                (SelectedSegment.Next as SegmentWrapper).DisposeThumb();
                            }
                        }
                    }
                    break;
                case "TargetDragState":
                    if (TargetDragState == DragState.Started)
                    {
                        if (FirstSegment != null)
                        {
                            if (FirstSegment.Count() > 0)
                            {
                                if (SelectedSegment != FirstSegment.Last())
                                {
                                    if (SelectedSegment != null)
                                    {
                                        (SelectedSegment as SegmentWrapper).DisposeThumb();
                                        if (SelectedSegment.Next != null)
                                        {
                                            (SelectedSegment.Next as SegmentWrapper).DisposeThumb();
                                        }
                                    }
                                    SelectedSegment = FirstSegment.Last();
                                    (SelectedSegment as SegmentWrapper).PrepareThumb(SelectedSegment == LastSegment);
                                }
                            }
                        }
                    }
                    break;
            }

            //throw new NotImplementedException();
        }

        public DragState TargetDragState
        {
            get { return _mCurrentTarget.DragState; }
            set
            {
                _mCurrentTarget.DragState = value;
                OnDragStateChanged("TargetDragState");
            }
        }

        public IInternalNodePort KnownTargetPort
        {
            get { return _knownTargetPort; }
            set
            {
                if (_knownTargetPort != value)
                {
                    _knownTargetPort = value;
                    if (_knownTargetPort != null)
                    {
                        if (_knownTargetPort.Source != TargetPort)
                        {
                            TargetPort = (IPort)_knownTargetPort.Source;
                            KnownTargetNode = _knownTargetPort.KnownNode;
                        }
                    }
                    else
                    {
                        TargetPort = null;
                    }
                }
            }
        }

        public bool IsIntersect(Rect rect)
        {
            if (InternalSegments != null)
            {
                if (rect.Contains(PathFigure.StartPoint))
                {
                    return true;
                }
                foreach (var internalSegment in InternalSegments)
                {
                    if ((internalSegment is LineSegment) && rect.Contains((internalSegment as LineSegment).Point))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpdateParentBridging(Rect rect)
        {
            List<Quad> quads = SharedData.SpatialSearch.FindQuads(rect);
            foreach (Quad quad in quads)
            {
                foreach (IInternalConnector con in quad.objects.OfType<IInternalConnector>())
                {
                    if (con.View != null && con.Bounds.IsIntersect(rect))
                    {
                        (con.View as Control).InvalidateArrange();
                    }
                }
            }
        }

        internal class BridgeSegments
        {
            public List<IEnumerable<PathSegment>> Bridges { get; set; }
            public List<Point> BridgeStartPoint { get; set; }
        }

        public bool CanBridge()
        {
            if (Constraints.Contains(ConnectorConstraints.InheritBridging))
            {
                return SharedData.Graph.CanBridge;
            }
            else
            {
                return Constraints.Contains(ConnectorConstraints.Bridging);
            }
        }

        public void UpdateRouting()
        {
            if (CanRoute())
            {
                if (!isRoutted)
                {
                    SharedData.Routing.UpdateRouting(this);
                }
                else
                {
                    isRoutted = false;
                }
            }
        }

        public bool CanRoute()
        {
            if (Constraints.Contains(ConnectorConstraints.InheritRouting))
            {
                return SharedData.Graph.Constraints.Contains(GraphConstraints.Routing);
            }
            else
                return Constraints.Contains(ConnectorConstraints.Routing);
        }

        public bool CanSourceDrag()
        {
            if (!SharedData.Graph.InternalSelectedItems.UserInteracting ||
                            (SharedData.Graph.InternalSelectedItems.UserInteracting &&
                             Constraints.Contains(ConnectorConstraints.SourceDraggable)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanDrag()
        {
            if (!SharedData.Graph.InternalSelectedItems.UserInteracting ||
                (SharedData.Graph.InternalSelectedItems.UserInteracting &&
                 Constraints.Contains(ConnectorConstraints.EndDraggable)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanTargetDrag()
        {
            if (!SharedData.Graph.InternalSelectedItems.UserInteracting ||
                (SharedData.Graph.InternalSelectedItems.UserInteracting &&
                 Constraints.Contains(ConnectorConstraints.TargetDraggable)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateBridging()
        {
            double bridgeSpacing = 15d;

            int count = -1;
            if (CanBridge() && !CanRoute())
            {
                BridgeSegments[] bridges = new BridgeSegments[0];
                bridgeSpacing = Connector.BridgeSpace;
                List<Point> line1 = this.GetPoints();
                foreach (ConnectorWrapper c in SharedData.Graph.InternalConnectors)
                {
                    if (Connector.Geometry != null)
                    {
                        if (Connector.ZIndex > (c.View as Connector).ZIndex)
                        {
                            if (Connector.Geometry.Bounds.IsIntersect((c.View as Connector).Geometry.Bounds))
                            {
                                List<Point> line2 = c.GetPoints();
                                List<Point> intersectPts = line1.Intersect(line2, false);
                                foreach (Point p in intersectPts)
                                {
                                    double fullLength;
                                    int segmentIndex;
                                    double length = this.GetLengthAtFractionPoint(
                                        Connector.Geometry.Figures[0], p, out fullLength, out segmentIndex);
                                    if (segmentIndex < 0)
                                    {
                                        continue;
                                    }
                                    if (Connector.Geometry.Figures[0].Segments[segmentIndex] is LineSegment)
                                    {
                                        Point startBridge, endBridge;
                                        double fractLength = (length - (bridgeSpacing / 2)) / fullLength;
                                        //this.VirtualConnectorPathGeometry.GetPointAtFractionLength(fractLength, out startBridge, out dummy);
                                        startBridge = this.GetPointAtLength((length - (bridgeSpacing / 2)), line1);
                                        fractLength = (length + (bridgeSpacing / 2)) / fullLength;
                                        //this.VirtualConnectorPathGeometry.GetPointAtFractionLength(fractLength, out endBridge, out dummy);
                                        endBridge = this.GetPointAtLength((length + (bridgeSpacing / 2)), line1);
                                        if (endBridge.Equals(new Point(0, 0)))
                                            endBridge = startBridge;
                                        Point start, end;
                                        if (segmentIndex == 0)
                                        {
                                            start = (Connector.Geometry.Figures[0].StartPoint);
                                        }
                                        else
                                        {
                                            start = Connector.Geometry.Figures[0].Segments[segmentIndex - 1].GetEndPoint();
                                        }
                                        end = (Connector.Geometry.Figures[0].Segments[segmentIndex] as LineSegment).Point;
                                        double angle = start.FindAngle(end);
                                        //if (arcD.ContainsKey(segmentIndex))
                                        if (bridges.Length > segmentIndex && bridges[segmentIndex] != null)
                                        {
                                            Point fixedpoint;
                                            if (segmentIndex == 0)
                                            {
                                                fixedpoint = (Connector.Geometry.Figures[0].StartPoint);
                                            }
                                            else
                                            {
                                                fixedpoint = Connector.Geometry.Figures[0].Segments[segmentIndex - 1].GetEndPoint();
                                            }

                                            double fix = Math.Abs(fixedpoint.FindLength(endBridge));
                                            double var;

                                            int insertAt = -1;
                                            count = -1;
                                            foreach (List<PathSegment> arc in bridges[segmentIndex].Bridges)
                                            {
                                                count++;
                                                var = Math.Abs(fixedpoint.FindLength(arc.Last().GetEndPoint()));
                                                if (fix < var)
                                                {
                                                    insertAt = count;
                                                    break;
                                                }
                                            }
                                            if (insertAt >= 0)
                                            {
                                                List<PathSegment> paths = new List<PathSegment>();
                                                foreach (
                                                    PathSegment _segment in
                                                        Connector.Internal_CreateSegments(startBridge, endBridge,
                                                                                          angle))
                                                    paths.Add(_segment);
                                                bridges[segmentIndex].Bridges.Insert(insertAt, paths);
                                                bridges[segmentIndex].BridgeStartPoint.Insert(insertAt, startBridge);
                                            }
                                            else
                                            {
                                                List<PathSegment> paths = new List<PathSegment>();
                                                foreach (
                                                    PathSegment _segment in
                                                        Connector.Internal_CreateSegments(startBridge, endBridge,
                                                                                          angle))
                                                    paths.Add(_segment);
                                                bridges[segmentIndex].Bridges.Add(paths);
                                                bridges[segmentIndex].BridgeStartPoint.Add(startBridge);
                                            }
                                        }
                                        else
                                        {
                                            if (!double.IsNaN(startBridge.X) && !double.IsNaN(startBridge.Y) &&
                                                !endBridge.Equals(new Point(0, 0)))
                                            {
                                                List<ArcSegment> arcs = new List<ArcSegment>();
                                                List<PathSegment> paths = new List<PathSegment>();
                                                foreach (
                                                    PathSegment _segment in
                                                        Connector.Internal_CreateSegments(startBridge, endBridge,
                                                                                          angle))
                                                    paths.Add(_segment);
                                                List<Point> stpoints = new List<Point>();
                                                List<Point> edpoints = new List<Point>();
                                                stpoints.Add(startBridge);
                                                edpoints.Add(endBridge);
                                                if (bridges.Length < segmentIndex + 1)
                                                {
                                                    Array.Resize(ref bridges, segmentIndex + 1);
                                                }
                                                bridges[segmentIndex] = new BridgeSegments();
                                                bridges[segmentIndex].Bridges = new List<IEnumerable<PathSegment>>() { paths };
                                                bridges[segmentIndex].BridgeStartPoint = stpoints;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //if (arcD.Count != 0)
                if (bridges.Count() != 0)
                {
                    //var v = (from key in arcD.Keys orderby key ascending select key);
                    count = -1;
                    //foreach (int Index in v)
                    int Index = -1;
                    foreach (var bridge in bridges)
                    {
                        Index++;
                        if (bridge == null)
                        {
                            continue;
                        }
                        for (int i = 1; i < bridge.Bridges.Count; i++)
                        {
                            if (bridge.Bridges[i].Last().GetEndPoint().FindLength(
                                bridge.Bridges[i - 1].Last().GetEndPoint()) < bridgeSpacing)
                            {
                                bridge.Bridges[i - 1].Last().SetEndPoint(bridge.Bridges[i].Last().GetEndPoint());
                                bridge.Bridges.RemoveAt(i);
                                bridge.BridgeStartPoint.RemoveAt(i);
                                i--;
                            }
                        }
                        var item = bridge.Bridges;
                        int SegmentIndex;
                        Point pre = this.SourcePoint;
                        foreach (var arc in bridge.Bridges)
                        {
                            count++;
                            SegmentIndex = Index + count;
                            Point end = Connector.Geometry.Figures[0].Segments[SegmentIndex].GetEndPoint();
                            if (SegmentIndex != 0)
                                pre = Connector.Geometry.Figures[0].Segments[SegmentIndex - 1].GetEndPoint();
                            int i = 1;
                            Rect r = new Rect(new Point(end.X - bridgeSpacing, end.Y - bridgeSpacing),
                                              new Point(end.X + bridgeSpacing, end.Y + bridgeSpacing));
                            Rect r1 = new Rect(new Point(pre.X - bridgeSpacing, pre.Y - bridgeSpacing),
                                               new Point(pre.X + bridgeSpacing, pre.Y + bridgeSpacing));
                            if (!r.Contains(bridge.BridgeStartPoint[bridge.Bridges.IndexOf(arc)]) &&
                                !r1.Contains(arc.Last().GetEndPoint()))
                            {
                                //count++;
                                foreach (PathSegment _segments in arc)
                                {
                                    Connector.Geometry.Figures[0].Segments.Insert(SegmentIndex + i,
                                                                                  _segments);
                                    i++;
                                    count++;
                                }
                                Connector.Geometry.Figures[0].Segments.Insert(SegmentIndex + i,
                                                                              new LineSegment()
                                                                                  {
                                                                                      Point = end
                                                                                  });
                                (Connector.Geometry.Figures[0].Segments[SegmentIndex] as LineSegment)
                                    .Point = bridge.BridgeStartPoint[bridge.Bridges.IndexOf(arc)];
                            }
                            else
                            {
                                count--;
                            }
                        }
                    }
                }
            }

        }

        private List<Point> GetPoints()
        {
            PathGeometry geom = Connector.Geometry;
            List<Point> pts = new List<Point>();
            PathFigure fig = geom.Figures[0];
            pts.Add(fig.StartPoint);
            foreach (PathSegment seg in fig.Segments)
            {
                if (seg is LineSegment)
                {
                    pts.Add((seg as LineSegment).Point);
                }
                else if (seg is PolyLineSegment)
                {
                    pts.AddRange((seg as PolyLineSegment).Points);
                }
                else if (seg is BezierSegment)
                {
                    pts.AddRange(BezireToPoly(pts[pts.Count - 1], seg as PathSegment));
                }
                else if (seg is QuadraticBezierSegment)
                {
                    pts.AddRange(BezireToPoly(pts[pts.Count() - 1], seg as PathSegment));
                }
                //else if (seg is ArcSegment)
                //{
                //    pts.AddRange(ArcToPoly(pts[pts.Count - 1], seg as ArcSegment));
                //}
            }
            return pts;
        }

        private double GetLengthAtFractionPoint(PathFigure pathFigure, Point at, out double fullLength, out int segmentIndex)
        {
            double confirm = 100d;
            fullLength = 0;
            segmentIndex = -1;
            int count = 0;
            double LengthAtFractionPoint = 0;
            if (pathFigure == null)
            {
                return 0;
            }
            //bool isAlreadyFlattened = true;

            //foreach (PathSegment pathSegment in pathFigure.Segments)
            //{
            //    if (!(pathSegment is PolyLineSegment) && !(pathSegment is LineSegment))
            //    {
            //        isAlreadyFlattened = false;
            //        break;
            //    }
            //}

            //PathFigure pathFigureFlattened = isAlreadyFlattened ? pathFigure : pathFigure.GetFlattenedPathFigure();
            PathFigure pathFigureFlattened = pathFigure;
            Point pt1 = pathFigureFlattened.StartPoint;
            Point previouspt2 = pt1;

            foreach (PathSegment pathSegment in pathFigureFlattened.Segments)
            {
                IEnumerable<Point> pointCollection = new List<Point>();
                if (pathSegment is LineSegment)
                {
                    Point pt2 = (pathSegment as LineSegment).Point;
                    pointCollection = new List<Point>() { pt2 };
                }
                else if (pathSegment is PolyLineSegment)
                {
                    pointCollection = (pathSegment as PolyLineSegment).Points;
                }
                else if (pathSegment is BezierSegment)
                {
                    pointCollection = BezireToPoly(previouspt2, pathSegment as PathSegment);
                }
                else if (pathSegment is QuadraticBezierSegment)
                {
                    pointCollection = BezireToPoly(previouspt2, pathSegment as PathSegment);
                }
                //Arc segment not supported.
                //else if (pathSegment is ArcSegment)
                //{
                //    pointCollection = ArcToPoly(previouspt2, pathSegment as ArcSegment);
                //}
                else
                {
                    pointCollection = new List<Point>();
                }

                foreach (Point pt2 in pointCollection)
                {
                    double suspect = getSlope(pt2, pt1, at, (this as ConnectorWrapper));
                    if (suspect < confirm)
                    {
                        confirm = suspect;
                        LengthAtFractionPoint = fullLength + at.FindLength(previouspt2);
                        segmentIndex = count;
                    }
                    fullLength += pt2.FindLength(pt1);
                    pt1 = pt2;
                    previouspt2 = pt2;
                }
                count++;
            }
            return LengthAtFractionPoint;
        }

        private double getSlope(Point st, Point en, Point point, ConnectorWrapper c)
        {
            double three = 3.0;// MeasureUnitsConverter.FromPixels(3.0, this.MeasurementUnit);
            double delx = Math.Abs(st.X - en.X);
            double dely = Math.Abs(st.Y - en.Y);
            double lhs = ((point.Y - st.Y) / (en.Y - st.Y));
            double rhs = ((point.X - st.X) / (en.X - st.X));
            if (double.IsInfinity(lhs) || double.IsInfinity(rhs) || double.IsNaN(lhs) || double.IsNaN(rhs))
            {
                if (st.X == en.X)
                {
                    if (st.Y == en.Y)
                    {
                        return 10000d;
                    }
                    else if (((st.Y > point.Y) && (point.Y > en.Y)) || ((st.Y < point.Y) && (point.Y < en.Y)))
                    {
                        return Math.Abs(st.X - point.X);
                    }
                    else
                    {
                        return 10000d;
                    }
                }
                else if (st.Y == en.Y)
                {
                    if (((st.X > point.X) && (point.X > en.X)) || ((st.X < point.X) && (point.X < en.X)))
                    {
                        return Math.Abs(st.Y - point.Y);
                    }
                    else
                    {
                        return 10000d;
                    }
                }
                else
                {
                    return 10000d;
                }
            }
            else if ((c.View as Connector).Geometry.Figures[0].Segments.Count() < 2)
            {
                if ((st.X >= point.X && point.X >= en.X) || (st.X <= point.X && point.X <= en.X) || delx < three)
                {
                    if ((st.Y >= point.Y && point.Y >= en.Y) || (st.Y <= point.Y && point.Y <= en.Y) || dely < three)
                    {
                        return Math.Abs(lhs - rhs);
                    }
                    else
                    {
                        return 10000d;
                    }
                }
                else
                {
                    return 10000d;
                }
            }
            else
            {
                return 10000d;
            }
        }

        private Point GetPointAtLength(double length, List<Point> pts)
        {
            double run = 0;
            Point? pre = null;
            Point found = new Point(0, 0);
            foreach (Point pt in pts)
            {
                if (!pre.HasValue)
                {
                    pre = pt;
                    continue;
                }
                else
                {
                    double l = pre.Value.FindLength(pt);
                    if (run + l > length)
                    {
                        double r = length - run;
                        double deg = pre.Value.FindAngle(pt);
                        double x = r * Math.Cos(deg * Math.PI / 180);
                        double y = r * Math.Sin(deg * Math.PI / 180);
                        found = new Point(pre.Value.X + x, pre.Value.Y + y);
                        break;
                    }
                    else
                    {
                        run += l;
                    }
                }
                pre = pt;
            }
            return found;
        }

        public override void Dispose()
        {
            base.Dispose();
            KnownSourceNode = null;
            KnownTargetNode = null;
            KnownSourcePort = null;
            KnownTargetPort = null;
            //SharedData.RelationshipController.SourceChanged(this, KnownSourceNode, null);
            //SharedData.RelationshipController.TargetChanged(this, KnownTargetNode, null);
            //_mKnownSourceNode = null;
            //_mKnownTargetNode = null;
        }

        List<Point> BezireToPoly(Point start, PathSegment segment)
        {
            List<Point> points = new List<Point>();
            if (segment is BezierSegment)
            {
                BezierSegment bezSeg = segment as BezierSegment;
                Point pt0 = start;
                Point pt1 = bezSeg.Point1;
                Point pt2 = bezSeg.Point2;
                Point pt3 = bezSeg.Point3;
                FlattenCubicBezier(points, pt0, pt1, pt2, pt3, 10);
            }
            else
            {
                QuadraticBezierSegment qudSeg = segment as QuadraticBezierSegment;
                Point pt0 = start;
                Point pt1 = qudSeg.Point1;
                Point pt2 = qudSeg.Point2;
                FlattenQuadraticBezier(points, pt0, pt1, pt2, 10);
            }
            return points;
        }

        void FlattenCubicBezier(List<Point> points, Point ptStart, Point ptCtrl1, Point ptCtrl2, Point ptEnd, double tolerance)
        {

            int max = (int)((ptStart.FindLength(ptCtrl1) +
                              ptCtrl1.FindLength(ptCtrl2) +
                              ptCtrl2.FindLength(ptEnd)) / tolerance);

            for (int i = 0; i <= max; i++)
            {
                double t = (double)i / max;

                double x = (1 - t) * (1 - t) * (1 - t) * ptStart.X +
                           3 * t * (1 - t) * (1 - t) * ptCtrl1.X +
                           3 * t * t * (1 - t) * ptCtrl2.X +
                           t * t * t * ptEnd.X;

                double y = (1 - t) * (1 - t) * (1 - t) * ptStart.Y +
                           3 * t * (1 - t) * (1 - t) * ptCtrl1.Y +
                           3 * t * t * (1 - t) * ptCtrl2.Y +
                           t * t * t * ptEnd.Y;

                points.Add(new Point(x, y));
            }
        }

        void FlattenQuadraticBezier(List<Point> points, Point ptStart, Point ptCtrl, Point ptEnd, double tolerance)
        {

            int max = (int)((ptCtrl.FindLength(ptStart) +
                                ptEnd.FindLength(ptCtrl)) / tolerance);

            for (int i = 0; i <= max; i++)
            {
                double t = (double)i / max;

                double x = (1 - t) * (1 - t) * ptStart.X +
                           2 * t * (1 - t) * ptCtrl.X +
                           t * t * ptEnd.X;

                double y = (1 - t) * (1 - t) * ptStart.Y +
                           2 * t * (1 - t) * ptCtrl.Y +
                           t * t * ptEnd.Y;

                points.Add(new Point(x, y));
            }
        }

        internal bool IsIntersectsWith(Rect selectionRect)
        {
            if (selectionRect.Contains(SourcePoint) || selectionRect.Contains(TargetPoint))
            {
                return true;
            }
            List<Point> _rect = new List<Point>(){
                new Point(selectionRect.Left,selectionRect.Top),
                new Point(selectionRect.Right,selectionRect.Top),
                new Point(selectionRect.Right,selectionRect.Bottom),
                new Point(selectionRect.Left,selectionRect.Bottom),
                new Point(selectionRect.Left,selectionRect.Top)
                };
            List<Point> _line = this.GetPoints();
            if (_line.Intersect(_rect, false).Count > 0)
            {
                return true;
            }
            return false;
        }

        internal void DragToX(double delta)
        {
            if (FirstSegment != null)
            {
                foreach (var seg in FirstSegment)
                {
                    if (seg.Next != null)
                    {
                        if (seg is ILineInternal)
                        {
                            ILineInternal segment = seg as ILineInternal;
                            if (segment.Point != null)
                                segment.Point =
                                    new Point(segment.Point.Value.X + delta,
                                        segment.Point.Value.Y);
                        }
                        else if (seg is IQuadraticCurveInternal)
                        {
                            IQuadraticCurveInternal segment = seg as IQuadraticCurveInternal;
                            segment.Point1 =
                                new Point(segment.Point1.Value.X + delta,
                                    segment.Point1.Value.Y);
                            segment.Point2 =
                                new Point(segment.Point2.Value.X + delta,
                                    segment.Point2.Value.Y);
                        }
                        else if (seg is ICubicCurveInternal)
                        {
                            ICubicCurveInternal segment = seg as ICubicCurveInternal;
                            segment.Point3 =
                                new Point(segment.Point3.Value.X + delta,
                                    segment.Point3.Value.Y);
                        }
                        else if (seg is ILineLengthInternal)
                        {

                        }
                    }
                    else if (seg is IQuadraticCurveInternal)
                    {
                        IQuadraticCurveInternal segment = seg as IQuadraticCurveInternal;
                        segment.Point1 =
                                new Point(segment.Point1.Value.X + delta,
                                    segment.Point1.Value.Y);
                    }
                }
            }
        }


        internal void DragToY(double delta)
        {
            if (FirstSegment != null)
            {
                foreach (var seg in FirstSegment)
                {
                    if (seg.Next != null)
                    {
                        if (seg is ILineInternal)
                        {
                            ILineInternal segment = seg as ILineInternal;
                            if (segment.Point != null)
                                segment.Point = new Point(segment.Point.Value.X, segment.Point.Value.Y + delta);
                        }
                        else if (seg is IQuadraticCurveInternal)
                        {
                            IQuadraticCurveInternal segment = seg as IQuadraticCurveInternal;
                            segment.Point1 = new Point(segment.Point1.Value.X, segment.Point1.Value.Y + delta);
                            segment.Point2 = new Point(segment.Point2.Value.X, segment.Point2.Value.Y + delta);
                        }
                        else if (seg is ICubicCurveInternal)
                        {
                            ICubicCurveInternal segment = seg as ICubicCurveInternal;
                            segment.Point3 = new Point(segment.Point3.Value.X, segment.Point3.Value.Y + delta);
                        }
                        else if (seg is ILineLengthInternal)
                        {
                            //(seg as LineSegment).Point = new Point((seg as LineSegment).Point.X + delta,
                            //    (seg as LineSegment).Point.Y);
                        }
                    }
                    else
                    {
                        if (seg is IQuadraticCurveInternal)
                        {
                            IQuadraticCurveInternal segment = seg as IQuadraticCurveInternal;
                            segment.Point1 = new Point(segment.Point1.Value.X, segment.Point1.Value.Y + delta);
                        }
                    }
                }
            }
        }
    }

    //internal struct ConnectorState
    //{
    //    private object _targetPort;
    //    private object _targetNode;
    //    private object _sourceNode;
    //    private Point _targetPoint;
    //    private Point _sourcePoint;
    //    private object _sourcePort;

    //    public Point SourcePoint
    //    {
    //        get { return _sourcePoint; }
    //        set { _sourcePoint = value; }
    //    }

    //    public Point TargetPoint
    //    {
    //        get { return _targetPoint; }
    //        set { _targetPoint = value; }
    //    }

    //    public object SourceNode
    //    {
    //        get { return _sourceNode; }
    //        set { _sourceNode = value; }
    //    }

    //    public object TargetNode
    //    {
    //        get { return _targetNode; }
    //        set { _targetNode = value; }
    //    }

    //    public object SourcePort
    //    {
    //        get { return _sourcePort; }
    //        set { _sourcePort = value; }
    //    }

    //    public object TargetPort
    //    {
    //        get { return _targetPort; }
    //        set { _targetPort = value; }
    //    }

    //    public ConnectorState(Point sPoint, Point tPoint, object sNode, object tNode, object sPort, object tPort)
    //    {
    //        _sourcePoint = sPoint;
    //        _targetPoint = tPoint;
    //        _sourceNode = sNode;
    //        _targetNode = tNode;
    //        _sourcePort = sPort;
    //        _targetPort = tPort;
    //    }
    //}
}
