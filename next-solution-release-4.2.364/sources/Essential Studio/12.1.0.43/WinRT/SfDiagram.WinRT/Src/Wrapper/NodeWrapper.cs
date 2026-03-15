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
using System.Reflection;
using System.Text;
using System.Windows;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes; 
#else
using System.Windows.Shapes;
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;

namespace Syncfusion.UI.Xaml.Diagram
{
    internal partial class NodeWrapper :
        GroupableWrapper,
        IProtectedNode
    {
        #region Fields

        private IView _mView;

        //protected NodeState _mCurrentState = new NodeState(0,0);
       
        //private Point _mCurrentOffset = new Point(0, 0);
        //private Point _mCurrentPivot = new Point(0.5, 0.5);
        //private double _mCurrentAngle = 0;
        //private Size _mCurrentSize = new Size(0, 0);

        private Size _mDesiredSize = new Size(0, 0);
        private double _mActualWidth = 0;
        private double _mActualHeight = 0;
        protected NodeChangedEventArgs _mCurrentState;

        #endregion
        
        public NodeWrapper(SharedData shared)
            : base(shared)
        {
        }

        protected override void SourceChanged()
        {
            base.SourceChanged();
            _mCurrentState = new NodeChangedEventArgs(CurrentChanged, OffsetX, OffsetY,
                                                    RotateAngle, UnitWidth, UnitHeight, Pivot);
            _mCurrentState.OffsetX = OffsetX;
            _mCurrentState.OffsetY = OffsetY;
        }

        private void CurrentChanged(ref NodeChangedEventArgs oldValue, ref NodeChangedEventArgs newValue)
        {
            if (SharedData != null && SharedData.NeededEvents.NeedNodeChangedEvent && !(this is ISelector))
            {
                SharedData.Graph.OnNodeChangedEvent(new ChangeEventArgs<object, NodeChangedEventArgs>(Source,
                                                                                                        ref oldValue,
                                                                                                        ref newValue));
            }
            if (SharedData.UndoRedoController != null && CanLogData())
            {
                base.LogData(new object[] {_mCurrentState, base.GetData() });
            }
        }

        public override object Undo(object data)
        {
            if (data is Array)
            {
                return RevertTo(data);
            }
            else
            {
                return base.Undo(data);
            }
        }

        public override object Redo(object data)
        {
            if (data is Array)
            {
                return RevertTo(data);
            }
            else
            {
                return base.Redo(data);
            }
        }

        protected virtual object RevertTo(object data)
        {
            if (data is Array)
            {
                var current = GetData();
                NodeChangedEventArgs toState = (NodeChangedEventArgs)(data as object[])[0];
                // TransformEventArgs tarState = (TransformEventArgs)(data as object[])[1];
                GroupableState gstate = (GroupableState)(data as object[])[1];
                if (toState.Pivot != Pivot)
                {
                    Pivot = toState.Pivot;
                }
                if (!IsSelected)
                {
                    if (toState.RotateAngle != RotateAngle)
                    {
                        RotateAngle = toState.RotateAngle;
                    }
                }
                if (toState.Width != UnitWidth)
                {
                    UnitWidth = toState.Width;
                }
                if (toState.Height != UnitHeight)
                {
                    UnitHeight = toState.Height;
                }
                if (toState.OffsetX != OffsetX)
                {
                    OffsetX = toState.OffsetX;
                }
                if (toState.OffsetY != OffsetY)
                {
                    OffsetY = toState.OffsetY;
                }
                if (gstate.Zindex != ZIndex)
                {
                    ZIndex = gstate.Zindex;
                }
                if (toState.ScaleX != _mCurrentState.ScaleX)
                {
                    (View as Node).ContentTransform.ScaleX = toState.ScaleX;
                    _mCurrentState.ScaleX = toState.ScaleX;

                }
                if (toState.ScaleY != _mCurrentState.ScaleY)
                {
                    (View as Node).ContentTransform.ScaleY = toState.ScaleY;
                    _mCurrentState.ScaleY = toState.ScaleY;
                }
                return current;
            }
            return data;
        }

        public override void Tap(bool fireEvent)
        {
            if (fireEvent)
            {
                SharedData.Graph.OnItemTappedEvent(new DiagramEventArgs(Source));
            }
            base.Tap(false);
        }

        public override void DoubleTap(bool fireEvent)
        {
            if (fireEvent)
            {
                SharedData.Graph.OnItemDoubleTappedEvent(new DiagramEventArgs(Source));
            }
            base.DoubleTap(false);
        }

        public void Invalidate(bool measure = true, bool arrange = true)
        {
            UIElement view = View as UIElement;
            if (view != null)
            {
                if(measure) view.InvalidateMeasure();
                if(arrange) view.InvalidateArrange();
            }
        }

        public override IView View
        {
            get { return _mView; }
            set
            {
                if (_mView != value)
                {
                    UnsetView();
                    _mView = value;
                    PrepareView();
                }
            }
        }

        private void UnsetView()
        {
            if (View != null)
            {
                if (View is Node)
                {
                    (View as Node).SetWrapper(null);
                }
                //if (InternalPorts != null)
                //{
                //    foreach (var internalPort in InternalPorts)
                //    {
                //        (View as Node).PortsHost.Children.Remove(internalPort.View);
                //    }
                //}
            }
            //if (!(this is IGroup))
            //{
            //    if (View != null)
            //    {
            //        (View as Control).SizeChanged -= view_SizeChanged;
            //    }
            //}
        }

        private void PrepareView()
        {
            if (View != null)
            {
                if (View is Node)
                {
                    (View as Node).SetWrapper(this);
                    (View as ISharedData).Init(SharedData);
                    //if (InternalPorts != null)
                    //{
                    //    foreach (var internalPort in InternalPorts)
                    //    {
                    //        (View as Node).PortsHost.Children.Add(internalPort.View);
                    //    }
                    //}
                }
                //if ((View as Control).ActualWidth != 0)
                //{
                //    this.ActualWidth = (View as Control).ActualWidth;
                //}
                //if ((View as Control).ActualHeight != 0)
                //{
                //    this.ActualHeight = (View as Control).ActualHeight;
                //}
                //UpdateCompositeTrans();
            }
            //if (!(this is IGroup))
            //{
            //    if (View != null)
            //    {
            //        (View as Control).SizeChanged += view_SizeChanged;
            //    }
            //}
        }

        //protected override void SourceChanged()
        //{
        //    base.SourceChanged();
        //    OnPivotChanged();
        //}

        public override object GetData()
        {
            return new [] {_mCurrentState, base.GetData()};
        }

        protected override void OnPropertyChanged(string name)
        {
            if (SharedData.UndoRedoController != null && CanLogData())
            {
                base.LogData(new object[] { _mCurrentState, base.GetData() });
            }
            base.OnPropertyChanged(name);
            if (!SharedData._unitchanging)
            {
                switch (name)
                {
                    case NodeConstants.OffsetX:
                        if (CanStartTransform(TransformState.TranslateX))
                        {
                            DragXTo(OffsetX);
                            UpdateBoundsCorners();
                            EndTransform(TransformState.TranslateX);
                        }
                        break;
                    case NodeConstants.OffsetY:
                        if (CanStartTransform(TransformState.TranslateY))
                        {
                            DragYTo(OffsetY);
                            UpdateBoundsCorners();
                            EndTransform(TransformState.TranslateY);
                        }
                        break;
                    case NodeConstants.RotateAngle:
                        if (CanStartTransform(TransformState.Rotate))
                        {
                            RotateTo(RotateAngle, null);
                            EndTransform(TransformState.Rotate);
                        }
                        break;
                    case "Pivot":
                        OnPivotChanged();
                        break;
                    case NodeConstants.Ports:
                        OnPortsChanged();
                        break;
                    case NodeConstants.Flip:
                        OnFlipChanged();
                        break;
                }
            }
        }

        private void OnFlipChanged()
        {
            //if (Flip != Diagram.Flip.None)
            {
                _mCurrentState.ScaleX *= -1;
                //(View as Node).ContentTransform.ScaleX = _mCurrentState.ScaleX;
            }
        } 
        protected override void OnGraphChanged()
        {
            base.OnGraphChanged();
            OnPortsChanged();
            if (Bounds == Rect.Empty && View == null)
            {
                double width = UnitWidth.IsValid() ? UnitWidth : DesiredSize.Width;
                double height = UnitHeight.IsValid() ? UnitHeight : DesiredSize.Height;
                Rect newBounds = new Rect(OffsetX, OffsetY, width, height);
                Center = new Point(OffsetX + width / 2, OffsetY + height / 2);
                this.SetBounds(newBounds);
            }
        }


        private void OnPivotChanged()
        {
            MovePivotTo(Pivot);
        }

        private void OnPortsChanged()
        {
            if (Ports != null)
            {
                DestructPorts();
                InternalPorts =
                    new ObservableElements
                        <INodePort, IInternalNodePort>
                        (Ports,
                         ElementType.Port,
                         SourceType.Node,
                         SharedData.EventAggregator,
                         SharedData.Graph
                                   .GetNodePortWrapper);
                ConstrctPorts();
            }
            else if (InternalPorts != null)
            {
                DestructPorts();
                InternalPorts = null;
            }
        }

        private void ConstrctPorts()
        {
            if (InternalPorts != null)
            {
                InternalPorts.Added += InternalPorts_Added;
                InternalPorts.Deleted += InternalPorts_Deleted;

                foreach (var internalPort in InternalPorts)
                {
                    PortAdded(internalPort);
                }
            }
        }

        private void DestructPorts()
        {
            if (InternalPorts != null)
            {
                foreach (var internalPort in InternalPorts)
                {
                    PortDeleted(internalPort);
                }
                InternalPorts.Added -= InternalPorts_Added;
                InternalPorts.Deleted -= InternalPorts_Deleted;
            }

        }

        void InternalPorts_Added(CollectionArgs<IInternalNodePort> obj)
        {
            PortAdded(obj.Element);
        }

        void InternalPorts_Deleted(CollectionArgs<IInternalNodePort> obj)
        {
            PortDeleted(obj.Element);
        }
        
        private void PortAdded(IInternalNodePort newPort)
        {
            newPort.KnownNode = this;
            if (View != null )
            {
                SharedData.Adorner.Children.Add(newPort.View);
                //(View as Node).InvalidateArrange();
            }
        }

        private void PortDeleted(IInternalNodePort oldPort)
        {
            oldPort.KnownNode = null;
            if (View != null )
            {
                SharedData.Adorner.Children.Remove(oldPort.View);
            }
        }

        public Size DesiredSize
        {
            get { return _mDesiredSize; }
            private set
            {
                if (_mDesiredSize != value)
                {
                    _mDesiredSize = value;
                    //OnPropertyChanged("DesiredSize");
                }
            }
        }

        public double ActualWidth
        {
            get { return _mActualWidth; }
            protected set
            {
                if (_mActualWidth != value)
                {
                    _mActualWidth = value;
                    //OnPropertyChanged("ActualWidth");
                }
            }
        }

        public double ActualHeight
        {
            get { return _mActualHeight; }
            protected set
            {
                if (_mActualHeight != value)
                {
                    _mActualHeight = value;
                    //OnPropertyChanged("ActualHeight");
                }
            }
        }

        public void SetDesiredSize(Size desiredSize)
        {
            DesiredSize = desiredSize;
        }

        public virtual void SetActualSize(Size actualSize)
        {
            ActualWidth = actualSize.Width;
            ActualHeight = actualSize.Height;
            _mCurrentState.Width = actualSize.Width;
            _mCurrentState.Height = actualSize.Height;
        }

        protected void GetScaleAsDelta(ref double newWidth, ref double newHeight,
                                       ref Point? pivot, out double deltaWidth, out double deltaHeight,
                                       bool aspectRatio = false)
        {
            if (pivot == null)
            {
                double x = OffsetX; // +Pivot.X * ActualWidth;
                double y = OffsetY; // +Pivot.Y * ActualHeight;
                pivot = new Point(x, y);
            }
            if (newWidth < MinWidth || newWidth > MaxWidth)
            {
                newWidth = _mCurrentState.Width;
            }
            if (newHeight < MinHeight || newHeight > MaxHeight)
            {
                newHeight = _mCurrentState.Height;
            }
            deltaWidth = newWidth / Math.Max(1, _mCurrentState.Width);
            deltaHeight = newHeight / Math.Max(1, _mCurrentState.Height);
            if (aspectRatio)
            {
                double large = Math.Max(Math.Abs(1 - deltaWidth), Math.Abs(1 - deltaHeight));
                if (Math.Abs(deltaHeight) == large)
                {
                    large = deltaHeight;
                }
                else
                {
                    large = deltaWidth;
                }
                deltaWidth = deltaHeight = large;
                newWidth = Math.Max(1, _mCurrentState.Width) * deltaWidth;
                newHeight = Math.Max(1, _mCurrentState.Height) * deltaHeight;
            }
            _mCurrentState.Width = newWidth;
            _mCurrentState.Height = newHeight;
        }

        public virtual void ScaleTo(double newWidth, double newHeight,
                            Point? pivot, bool aspectRatio = false)
        {
            double deltaWidth, deltaHeight;
            GetScaleAsDelta(ref newWidth, ref newHeight, ref pivot, out deltaWidth, out deltaHeight, aspectRatio);
            if (deltaWidth != 1 &&
                !(newWidth == ActualWidth || newWidth == UnitWidth || newWidth == DesiredSize.Width))
            {
                //_mCurrentSize.Width = newWidth;
                this.UnitWidth = newWidth;
            }
            if (deltaHeight != 1 &&
                !(newHeight == ActualHeight || newHeight == UnitHeight || newHeight == DesiredSize.Height))
            {
                //_mCurrentSize.Height = newHeight;
                this.UnitHeight = newHeight;
            }


            //if (deltaX > 1 || deltaY > 1)
            {
                MatrixExt scaledMatrix = MatrixExt.Identity;
                scaledMatrix.RotateAt(-RotateAngle, pivot.Value.X, pivot.Value.Y);
                scaledMatrix.ScaleAt(deltaWidth, deltaHeight, pivot.Value.X, pivot.Value.Y);
                scaledMatrix.RotateAt(RotateAngle, pivot.Value.X, pivot.Value.Y);
                //scaledMatrix = Matrix*scaledMatrix;

                Point newPosition = scaledMatrix.Transform(new Point(OffsetX, OffsetY));
                //DragXTo(newPosition.X, false);
                //DragYTo(newPosition.Y, false);
                OffsetX = newPosition.X;
                OffsetY = newPosition.Y;
                //UpdateCompositeTrans();
            }
        }

        protected void GetRotateDelta(ref double newAngle, out double deltaAngle, ref Point? pivot)
        {
            double x = OffsetX;
            double y = OffsetY;
            if (pivot == null)
            {
                pivot = new Point(x, y);
            }
            deltaAngle = newAngle - _mCurrentState.RotateAngle;
            _mCurrentState.RotateAngle = newAngle;
        }

        public virtual void RotateTo(double newAngle, Point? pivot)
        {
            double x = OffsetX;
            double y = OffsetY;

            if (_mCurrentState.RotateAngle != newAngle)
            {
                double delta;
                GetRotateDelta(ref newAngle, out delta, ref pivot);
                double angle = RotateAngle;
                if (pivot.Value != new Point(x, y))
                {
                    MatrixExt newMatrix = MatrixExt.Identity;
                    newMatrix.RotateAt(delta, pivot.Value.X, pivot.Value.Y);
                    Point trans = newMatrix.Transform(new Point(x, y));
                    DragXTo(trans.X);
                    DragYTo(trans.Y);
                }
                else
                {
                    if (angle != newAngle)
                    {
                        RotateAngle = newAngle;
                    }
                }
                //UpdateCompositeTrans();
                //if (affectsChild)
                //{
                //    RotateDelta(delta, pivot.Value);
                //}
            }
        }

        protected void GetDragXDelta(ref double newValue, out double deltaX)
        {
            deltaX = newValue - _mCurrentState.OffsetX;
            _mCurrentState.OffsetX = newValue;
        }

        public virtual void DragXTo(double newValue)
        {
            double x = OffsetX;
            if (newValue != _mCurrentState.OffsetX)
            {
                double delta;
                GetDragXDelta(ref newValue, out delta);
                if (x != newValue)
                {
                    OffsetX = newValue;
                }
                //UpdateCompositeTrans();
                //if (affectsChild)
                //{
                //    DragDeltaX(delta);
                //}
            }
        }

        protected void GetDragYDelta(ref double newValue, out double deltaY)
        {
            deltaY = newValue - _mCurrentState.OffsetY;
            _mCurrentState.OffsetY = newValue;
        }

        public virtual void DragYTo(double newValue)
        {
            double y = OffsetY;
            if (newValue != _mCurrentState.OffsetY)
            {
                double delta;
                GetDragYDelta(ref newValue, out delta);
                if (y != newValue)
                {
                    OffsetY = newValue;
                }
                //UpdateCompositeTrans();
                //if (affectsChild)
                //{
                //    DragDeltaY(delta);
                //}
            }
        }

        protected virtual void GetPivotDelta(ref Point newPivot,out Point delta)
        {
            delta = new Point((newPivot.X - _mCurrentState.Pivot.X) * ActualWidth,
                                    (newPivot.Y - _mCurrentState.Pivot.Y) * ActualHeight);
            _mCurrentState.Pivot = newPivot;
            MatrixExt newMatrix = MatrixExt.Identity;
            newMatrix.Rotate(RotateAngle);
                delta = newMatrix.Transform(delta);
        }

        public virtual void MovePivotTo(Point newPivot)
        {
            Point orig = new Point(OffsetX, OffsetY);
            if (_mCurrentState.Pivot.X != newPivot.X || _mCurrentState.Pivot.Y != newPivot.Y)
            {
                Point delta;
                GetPivotDelta(ref newPivot, out delta);
                if (Pivot != newPivot)
                {
                    Pivot = newPivot;
                }
                DragXTo(orig.X + delta.X);
                DragYTo(orig.Y + delta.Y);
            }
        }

        protected void ResetTo(double x, double y, double angle, Point pivot)
        {
            _mCurrentState.OffsetX = x;
            _mCurrentState.OffsetY = y;
            _mCurrentState.RotateAngle = angle;
            _mCurrentState.Pivot = pivot;

            Pivot = pivot;
            RotateAngle = angle;
            OffsetX = x;
            OffsetY = y;
        }

        public void UpdateBoundsCorners()
        {
            double x = OffsetX;
            double y = OffsetY;
            double width = UnitWidth.IsValid() ? UnitWidth : DesiredSize.Width;
            double height = UnitHeight.IsValid() ? UnitHeight : DesiredSize.Height;
            Point center;
            Corners = new RectCorners(new Rect(x, y, width, height), false, Pivot, out center, RotateAngle);
            Center = center;

            SetBounds(Corners.Value.OuterBounds);
        }

        public void UpdateCompositeTrans()
        {
            double x = OffsetX;
            double y = OffsetY;
            double width = UnitWidth.IsValid() ? UnitWidth : DesiredSize.Width;
            double height = UnitHeight.IsValid() ? UnitHeight : DesiredSize.Height;

            Point center;
            Corners = new RectCorners(new Rect(x, y, width, height), false, Pivot, out center, RotateAngle);
            Center = center;

            SetBounds(Corners.Value.OuterBounds);

            x = x - Pivot.X*width;
            y = y - Pivot.Y*height;
            if (View != null)
            {
                if (View is Selector)
                {
                }
                else
                    if (View is Node)
                    {
                        (View as Node).ArrangeTransform.TranslateX = x;
                        (View as Node).ArrangeTransform.TranslateY = y;
                        (View as Node).ArrangeTransform.Rotation = RotateAngle;
                        (View as Node).ContentTransform.ScaleX = _mCurrentState.ScaleX;
                    }
            }

            if (this.IsSelected)
            {
                SharedData.Adorner.UpdatePreview(this);
            }
        }

        public OrthogonalDirection GetDirection(Point port)
        {
            Point one, two, three, four, center;
            one = Corners.Value.TopLeft;
            two = Corners.Value.TopRight;
            three = Corners.Value.BottomRight;
            four = Corners.Value.BottomLeft;
            center = Center;

            double angle = port.FindAngle(center);
            //angle = angle + KnownNode.RotateAngle;
            //angle = angle%360;

            //Top
            if (angle > one.FindAngle(center) && angle < two.FindAngle(center))
            {
                return OrthogonalDirection.Top;
            }
            //Right
            else if (angle >= two.FindAngle(center) && angle < three.FindAngle(center))
            {
                return OrthogonalDirection.Right;
            }
            else if (angle >= three.FindAngle(center) && angle < four.FindAngle(center))
            {
                return OrthogonalDirection.Bottom;
            }
            else if (angle >= four.FindAngle(center))
            {
                return OrthogonalDirection.Left;
            }
            else if (angle < one.FindAngle(center))
            {
                return OrthogonalDirection.Left;
            }
            else
            {  //Somthing wrong.
                return OrthogonalDirection.Right;
            }
        }

        public TransformState TransformState { get; protected set; }

        public bool CanStartTransform(TransformState state)
        {
            if ((TransformState & state) != state)
            {
                TransformState |= state;
                TransformState |= TransformState.Transforming;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void EndTransform(TransformState state)
        {
            TransformState &= ~state;
            if (TransformState == TransformState.Transforming)
            {
                TransformState &= ~TransformState.Transforming;
            }
            else
            {
                TransformState = TransformState.Transforming;
            }
        }

        //public bool IsExpanded
        //{
        //    get { return true; }
        //}

        public Point GetIntersection(Point point)
        {
            if (Corners == null)
            {
                return Center;
            }
            LineUtil referece = new LineUtil() { X1 = point.X, Y1 = point.Y, X2 = Center.X, Y2 = Center.Y };
            Point[] corners = new Point[]
                {
                    Corners.Value.TopLeft,
                    Corners.Value.TopRight,
                    Corners.Value.BottomRight,
                    Corners.Value.BottomLeft
                };
            double shortest = double.MaxValue;
            Point? final = null;
            for (int i = 0; i <= 3; i++)
            {
                LineUtil trial = new LineUtil()
                    {
                        X1 = corners[i].X,
                        Y1 = corners[i].Y,
                        X2 = corners[(i + 1) % 4].X,
                        Y2 = corners[(i + 1) % 4].Y
                    };
                Point POI;
                if (referece.Intersect(trial, out POI))
                {
                    double susspect = POI.FindLength(point);
                    if (susspect < shortest)
                    {
                        shortest = susspect;
                        final = POI;
                    }
                }
            }
            if (final == null)
            {
                return Center;
            }
            return final.Value;
        }

        public Point GetIntersection(IInternalNode end)
        {
            return GetIntersection(end.Center);
        }

        protected override bool CanSelect()
        {
            if (base.CanSelect())
            {
                if (Constraints.Contains(NodeConstraints.Selectable))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanDrag()
        {
            if (!SharedData.Graph.InternalSelectedItems.UserInteracting ||
                (SharedData.Graph.InternalSelectedItems.UserInteracting &&
                 Constraints.Contains(NodeConstraints.Draggable)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanRotate()
        {
            if (!SharedData.Graph.InternalSelectedItems.UserInteracting ||
                (SharedData.Graph.InternalSelectedItems.UserInteracting &&
                 Constraints.Contains(NodeConstraints.Rotatable)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanScale()
        {
            if (!SharedData.Graph.InternalSelectedItems.UserInteracting ||
                (SharedData.Graph.InternalSelectedItems.UserInteracting &&
                 Constraints.Contains(NodeConstraints.Resizable)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<IConnector> InOutConnectors
        {
            get
            {
                if (InternalInOutConnectors != null)
                {
                    return InternalInOutConnectors.Select(e => e.Source as IConnector);
                }
                else
                {
                    return null;
                }
            }
        }

        public IEnumerable<IConnector> InConnectors
        {
            get
            {
                if (InternalInConnectors != null)
                {
                    return InternalInConnectors.Select(e => e.Source as IConnector);
                }
                else
                {
                    return null;
                }
            }
        }
        public IEnumerable<IConnector> OutConnectors
        {
            get
            {
                if (InternalOutConnectors != null)
                {
                    return InternalOutConnectors.Select(e => e.Source as IConnector);
                }
                else
                {
                    return null;
                }
            }
        }

        public IEnumerable<INode> Neighbors
        {
            get
            {
                if (InternalNeighbors != null)
                {
                    return InternalNeighbors.Select(e => e.Source as INode);
                }
                else
                {
                    return null;
                }
            }
        }
        public IEnumerable<INode> InNeighbors
        {
            get
            {
                if (InternalInNeighbors != null)
                {
                    return InternalInNeighbors.Select(e => e.Source as INode);
                }
                else
                {
                    return null;
                }
            }
        }
        public IEnumerable<INode> OutNeighbors
        {
            get
            {
                if (InternalOutNeighbors != null)
                {
                    return InternalOutNeighbors.Select(e => e.Source as INode);
                }
                else
                {
                    return null;
                }
            }
        }
        public IEnumerable<INode> Children
        {
            get
            {
                if (InternalChildren != null)
                {
                    return InternalChildren.Select(e => e.Source as INode);
                }
                else
                {
                    return null;
                }
            }
        }

        public void InializeRelationship()
        {
            _mInOutConnectors = new List<IInternalConnector>();
            _mInConnectors = new List<IInternalConnector>();
            _mOutConnectors = new List<IInternalConnector>();
            _mNodeNeighbors = new List<IInternalNode>();
            _mNodeInNeighbors = new List<IInternalNode>();
            _mNodeOutNeighbors = new List<IInternalNode>();
            _mNodeChildren = new List<IInternalNode>();
        }

        private List<IInternalConnector> _mInOutConnectors;

        private List<IInternalConnector> _mInConnectors;

        private List<IInternalConnector> _mOutConnectors;

        //public TSource PreviousSibling
        //{
        //    get
        //    {
        //        IInternalNode node =
        //            this as IInternalNode;
        //        IInternalGroup group =
        //            this as IInternalGroup;
        //        if (node != null)
        //        {
        //            return (TSource)(object)KnownParentGroup.InternalNodes.Previours(node).Source;
        //        }
        //        else
        //        {
        //            return (TSource)(object)KnownParentGroup.InternalGroups.Previours(group).Source;
        //        }
        //    }
        //}

        public IEnumerable<IInternalConnector> InternalInOutConnectors
        {
            get { return _mInOutConnectors; }
        }

        public IEnumerable<IInternalConnector> InternalInConnectors
        {
            get { return _mInConnectors; }
        }

        public IEnumerable<IInternalConnector> InternalOutConnectors
        {
            get { return _mOutConnectors; }
        }

        private List<IInternalNode> _mNodeNeighbors;

        private List<IInternalNode> _mNodeInNeighbors;

        private List<IInternalNode> _mNodeOutNeighbors;

        private List<IInternalNode> _mNodeChildren;

        public IEnumerable<IInternalNode> InternalNeighbors
        {
            get { return _mNodeNeighbors; }
        }

        public IEnumerable<IInternalNode> InternalInNeighbors
        {
            get { return _mNodeInNeighbors; }
        }

        public IEnumerable<IInternalNode> InternalOutNeighbors
        {
            get { return _mNodeOutNeighbors; }
        }

        public IEnumerable<IInternalNode> InternalChildren
        {
            get { return _mNodeChildren; }
        }

        public bool HasChild
        {
            get
            {
                if (InternalChildren != null)
                {
                    return
                        //OutNeighbors.Any();
                    InternalChildren.Any();
                }
                else return false;
            }
        }

        public Rect Rectangle
        {
            get { return new Rect(0, 0, UnitWidth.IsValid() ? UnitWidth : ActualWidth, UnitHeight.IsValid() ? UnitHeight : ActualHeight); }
        }
        public IInternalNode ParentNode
        {
            get
            {
                if(InternalInNeighbors!=null)
                return 
                    InternalInNeighbors.FirstOrDefault(
                        nei => nei.InternalChildren.Contains(this));
                //var k = InNeighbors.FirstOrDefault();
                return null;
            }
        }
        public IInternalNode FirstChild
        {
            get {
                if (InternalChildren != null) 
                    return InternalChildren.FirstOrDefault();
                return null;
            }
        }
        public IInternalNode LastChild
        {
            get
            {
                if (InternalChildren != null)
                return InternalChildren.LastOrDefault();
                return null;
            }
        }
        public IInternalNode PreviousSibling
        {
            get
            {
                if (ParentNode == null)
                {
                    return null;
                }
                else
                {
                    if (ParentNode.InternalChildren != null)
                    {
                        var ch = ParentNode.InternalChildren as List<IInternalNode>;
                        int chi = ch.IndexOf(this);
                        if (chi < 0 || chi > ch.Count - 1)
                        {
                            throw new IndexOutOfRangeException();
                        }

                        if (chi == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return ch[chi - 1];
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public IInternalNode NextSibling
        {
            get
            {
                if (ParentNode == null)
                {
                    return null;
                }
                else
                {
                    if (ParentNode.InternalChildren != null)
                    {
                        var ch = ParentNode.InternalChildren as List<IInternalNode>;
                        int chi = ch.IndexOf(this);
                        if (chi < 0 || chi > ch.Count - 1)
                        {
                            throw new IndexOutOfRangeException();
                        }

                        if (chi == ch.Count - 1)
                        {
                            return null;
                        }
                        else
                        {
                            return ch[chi + 1];
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /*
         * Properties ment for radial and table layout, these properties should will be removed in future.
         */
        public int Stage
        {
            get;
            set;

        }
        public double TempX
        {
            get;
            set;
        }
        public double TempY
        {
            get;
            set;
        }
        public bool Visited
        {
            get;
            set;
        }
        public bool SubTreeVal
        {
            get;
            set;
        }
        public double SegmentOffset
        {
            get;
            set;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (InternalInConnectors != null && InternalInConnectors.Any())
            {
                foreach (var con in InternalInConnectors.ToList())
                {
                    con.KnownTargetNode = null;
                    con.KnownTargetPort = null;
                }
            }
            if (InternalOutConnectors != null && InternalOutConnectors.Any())
            {
                foreach (var con in InternalOutConnectors.ToList())
                {
                    con.KnownSourceNode = null;
                    con.KnownSourcePort = null;
                }
            }
        }
    }

    internal enum Corner
    {
        None,
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    [Flags]
    internal enum TransformState
    {
        None = 0,
        TranslateX = 1,
        TranslateY = 2,
        ScaleX = 4,
        ScaleY = 8,
        Rotate = 16,
        Transforming = 32
    }

    internal struct NodeState
    {
        private double _offsetX;
        private double _offsetY;
        private double _height;
        private double _width;
        private double _rotateAngle;
        private Point _pivot;
        private int _scalX;
        private int _scaleY;

        public NodeState(double x, double y, double h, double w, double a, Point v,int scalx, int scaley)
        {
            _offsetX = x;
            _offsetY = y;
            _width = w;
            _height = h;
            _rotateAngle = a;
            _pivot = v;
            _scalX = scalx;
            _scaleY = scaley;
        }

        public NodeState(double x, double y)
            : this(x, y, 0, 0, 0, new Point(0.5, 0.5),1,1)
        {
        }
               
        public double OffsetX
        {
            get { return _offsetX; }
            set { _offsetX = value; }
        }

        public double OffsetY
        {
            get { return _offsetY; }
            set { _offsetY = value; }
        }

        public Point Pivot
        {
            get { return _pivot; }
            set { _pivot = value; }
        }

        public double RotateAngle
        {
            get { return _rotateAngle; }
            set { _rotateAngle = value; }
        }

        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int ScaleX
        {
            get { return _scalX; }
            set { _scalX = value; }
        }

        public int ScaleY
        {
            get { return _scaleY; }
            set { _scaleY = value; }
        }
        
    }

    internal struct GroupableState
    {
        private int _mzindex;
        public GroupableState(int z)
        {
            _mzindex = z;
        }

        public int Zindex
        {
            get
            {
                return _mzindex;
            }
            set
            {
                _mzindex = value;
            }
        }
    }
}
