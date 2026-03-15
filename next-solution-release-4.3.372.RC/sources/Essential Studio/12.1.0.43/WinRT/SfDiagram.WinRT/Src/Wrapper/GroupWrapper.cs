#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.UI.Xaml.Diagram.Utility;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media; 
#else
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    internal partial class GroupWrapper :
        NodeWrapper,
        IProtectedGroup
    {
        public GroupWrapper(SharedData shared)
            : base(shared)
        {

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

        protected override void SourceChanged()
        {
            base.SourceChanged();
            OnNodesChanged();
            OnConnectorsChanged();
            OnGroupsChanged();
        }

        public bool IsDescendentSelected { get; private set; }

        public bool IsParentSelected
        {
            get
            {
                return IsSelected || KnownParentGroup != null && KnownParentGroup.IsParentSelected;
            }
        }

        public void CheckDescendentSelected()
        {
            if (InternalNodes != null && InternalNodes.FirstOrDefault(e => e.IsSelected) != null)
            {
                DescendentSelected();
                return;
            }
            else if (InternalConnectors != null && InternalConnectors.FirstOrDefault(e => e.IsSelected) != null)
            {
                DescendentSelected();
                return;
            }
            else if (InternalGroups != null && InternalGroups.FirstOrDefault(e => e.IsSelected || e.IsDescendentSelected) != null)
            {
                DescendentSelected();
                return;
            }
            else
            {
                IsDescendentSelected = false;
            }
            if (KnownParentGroup != null)
            {
                KnownParentGroup.CheckDescendentSelected();
            }
        }

        public void DescendentSelected()
        {
            IsDescendentSelected = true;
            if (KnownParentGroup != null)
            {
                KnownParentGroup.DescendentSelected();
            }
        }

        protected override void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(name);
            switch (name)
            {
                case GroupConstants.Nodes:
                    OnNodesChanged();
                    break;
                case GroupConstants.Connectors:
                    OnConnectorsChanged();
                    break;
                case GroupConstants.Groups:
                    OnGroupsChanged();
                    break;
            }
        }

        private void OnNodesChanged()
        {
            CheckInternalCollection(ElementType.Node, Nodes, InternalNodes);
        }

        private void OnConnectorsChanged()
        {
            CheckInternalCollection(ElementType.Connector, Connectors, InternalConnectors);
        }

        private void OnGroupsChanged()
        {
            CheckInternalCollection(ElementType.Group, Groups, InternalGroups);
        }

        private void CheckInternalCollection<TSource, TWrap>
            (ElementType element, object source, ObservableElements<TSource, TWrap> observer)
            where TWrap : class, IWrapper
        {
            bool createNew = false;
            bool deleteOld = false;
            if (source != null)
            {
                if (observer == null)
                {
                    createNew = true;
                }
                else if (observer.Source == source)
                {
                    // No change
                }
                else
                {
                    createNew = true;
                    deleteOld = true;
                }
            }
            else
            {
                if (observer == null)
                {
                    // No change
                }
                else
                {
                    deleteOld = true;
                }
            }


            if (deleteOld)
            {
                if (!createNew)
                {
                    observer.Added -= InternalGroupable_Added;
                    observer.Deleted -= InternalGroupable_Deleted;
                    foreach (var item in observer)
                    {
                        (item as IInternalGroupable).BoundsChanged -= Item_BoundsChanged;
                    }
                    switch (element)
                    {
                        case ElementType.Node:
                            InternalNodes = null;
                            break;
                        case ElementType.Connector:
                            InternalConnectors = null;
                            break;
                        case ElementType.Group:
                            InternalGroups = null;
                            break;
                    }
                }
            }
            if (createNew)
            {
                Create(element);
                if (View != null)
                {
                    (View as UIElement).InvalidateMeasure();
                }
                //UpdateBounds();
            }
        }

        private void Create(ElementType element)
        {
            SourceType collType = SourceType.Group;
            if (this is ISelector)
            {
                collType = SourceType.SelectionList;
            }
            switch (element)
            {
                case ElementType.Node:
                    InternalNodes = new ObservableElements<object, IInternalNode>
                        (Nodes,
                         ElementType.Node,
                         collType,
                         SharedData.EventAggregator,
                         SharedData.Graph.GetNodeWrapper);
                    PrepareCollection(InternalNodes);
                    break;
                case ElementType.Connector:
                    InternalConnectors = new ObservableElements<object, IInternalConnector>
                        (Connectors,
                         ElementType.Connector,
                         collType,
                         SharedData.EventAggregator,
                         SharedData.Graph.GetConnectorWrapper);
                    PrepareCollection(InternalConnectors);
                    break;
                case ElementType.Group:
                    InternalGroups = new ObservableElements<object, IInternalGroup>
                        (Groups,
                         ElementType.Group,
                         collType,
                         SharedData.EventAggregator,
                         SharedData.Graph.GetGroupWrapper);
                    PrepareCollection(InternalGroups);
                    break;
            }
        }

        private void PrepareCollection<TSource, TWrap>
            (ObservableElements<TSource, TWrap> observer)
            where TWrap : class, IWrapper
        {
            foreach (var item in observer)
            {
                IInternalGroupable element =
                    item as IInternalGroupable;
                element.BoundsChanged += Item_BoundsChanged;
                if (!(this is ISelector))
                {
                    element.KnownParentGroup = this;
                }
            }
            observer.Added += InternalGroupable_Added;
            observer.Deleted += InternalGroupable_Deleted;
        }

        void InternalGroupable_Deleted<T>(CollectionArgs<T> obj)
        {
            IInternalGroupable element =
                obj.Element as IInternalGroupable;
            element.BoundsChanged -= Item_BoundsChanged;
            if (this is ISelector)
            {
                element.IsSelected = false;
            }
            if (!(this is ISelector))
            {
                element.KnownParentGroup = null;
            }
            //UpdateBounds();
            if (View != null)
            {
                (View as UIElement).InvalidateMeasure();
            }
        }

        private void Item_BoundsChanged(IInternalGroupable obj)
        {
            //UpdateBounds();
            if (View != null)
            {
                (View as UIElement).InvalidateMeasure();
            }
            if (obj.IsSelected)
            {
                (SharedData.Graph.InternalSelectedItems.View as UIElement).InvalidateMeasure();
            }
        }

        void InternalGroupable_Added<T>(CollectionArgs<T> obj)
        {
            IInternalGroupable element =
                obj.Element as IInternalGroupable;
            element.BoundsChanged += Item_BoundsChanged;
            if (this is ISelector && !element.IsSelected)
            {
                element.IsSelected = true;
            }
            if (!(this is ISelector))
            {
                element.KnownParentGroup = this;
                //UpdateBounds();
            }
            else if (GetSelectedItems().Count() == 1)
            {
                var item = GetSelectedItems().First() as IDiagramElement;
                if (item is INode)
                {
                    //RotateTo((item as INode).RotateAngle, null, false);
                    //UpdateBounds();
                    //RotateAngle = (item as INode).RotateAngle;
                }
                else if (item is IConnector)
                {
                    //UpdateBounds();
                }
            }
            else
            {
                //UpdateBounds();
            }
            if (View != null)
            {
                (View as UIElement).InvalidateMeasure();
            }
        }

        private double tempX;
        private double tempY;
        private double tempW;
        private double tempH;
        private Point tempPivot;
        private double tempA;


        public override void DragXTo(double newOffsetX)
        {
            double delta;
            GetDragXDelta(ref newOffsetX, out delta);

            if (InternalNodes != null)
            {
                foreach (var node in InternalNodes)
                {
                    if (node.CanDrag())
                    {
                        node.OffsetX += delta;
                    }
                }
            }
            if (InternalConnectors != null)
            {
                foreach (var conn in InternalConnectors)
                {
                    if (conn.InternalSegments != null)
                    {
                        (conn as ConnectorWrapper).DragToX(delta);
                    }
                    if (conn.CanDrag() || conn.CanSourceDrag())
                    {
                        conn.SourcePoint = new Point(conn.SourcePoint.X + delta, conn.SourcePoint.Y);
                        conn.TargetPoint = new Point(conn.TargetPoint.X + delta, conn.TargetPoint.Y);
                    }
                }
            }
            if (InternalGroups != null)
            {
                foreach (var node in InternalGroups)
                {
                    if (node.CanDrag())
                    {
                        node.OffsetX += delta;
                    }
                }
            }
            tempX = newOffsetX;
        }

        public override void DragYTo(double newOffsetY)
        {
            double delta;
            GetDragYDelta(ref newOffsetY, out delta);
            if (InternalNodes != null)
            {
                foreach (var node in InternalNodes)
                {
                    if (node.CanDrag())
                    {
                        node.OffsetY += delta;
                    }
                }
            }
            if (InternalConnectors != null)
            {
                foreach (var conn in InternalConnectors)
                {
                    if (conn.InternalSegments != null)
                    {
                        (conn as ConnectorWrapper).DragToY(delta);
                      
                    }
                    if (conn.CanDrag() || conn.CanTargetDrag())
                    {
                        conn.SourcePoint = new Point(conn.SourcePoint.X, conn.SourcePoint.Y + delta);
                        conn.TargetPoint = new Point(conn.TargetPoint.X, conn.TargetPoint.Y + delta);
                    }
                }
            }
            if (InternalGroups != null)
            {
                foreach (var node in InternalGroups)
                {
                    if (node.CanDrag())
                    {
                        node.OffsetY += delta;
                    }
                }
            }
            tempY = newOffsetY;
        }

        public override void RotateTo(double newAngle, Point? pivot)
        {
            double delta;
            GetRotateDelta(ref newAngle, out delta, ref pivot);
            MatrixExt newMatrix = MatrixExt.Identity;
            newMatrix.RotateAt(delta, OffsetX, OffsetY);
            //if (InternalNodes != null)
            {
                //foreach (var node in InternalNodes)
                foreach (IInternalGroupable item in GetSelectedItems())
                {
                    IInternalNode node = item as IInternalNode;
                    if (node != null && node.CanRotate())
                    {
                        Point trans = new Point(node.OffsetX, node.OffsetY);
                        trans = newMatrix.Transform(trans);
                        node.RotateAngle += delta;
                        node.OffsetX = trans.X;
                        node.OffsetY = trans.Y;
                        //node.RotateTo(node.RotateAngle + delta, null);
                    }
                }
            }
            tempA = newAngle;
        }

        public override void ScaleTo(double newWidth, double newHeight, Point? pivot, bool aspectRatio = false)
        {
            double deltaWidth, deltaHeight;
            GetScaleAsDelta(ref newWidth, ref newHeight, ref pivot, out deltaWidth, out deltaHeight, aspectRatio);
            if (deltaWidth != 1 && deltaWidth != 0 && deltaHeight != 1 && deltaHeight != 0)
            {
                ScaleDelta(deltaWidth, deltaHeight, pivot.Value, aspectRatio);
            }
            else if (deltaWidth != 1 && deltaWidth != 0)
            {
                ScaleDelta(deltaWidth, 1, pivot.Value, aspectRatio);
            }
            else if (deltaHeight != 1 && deltaHeight != 0)
            {
                ScaleDelta(1, deltaHeight, pivot.Value, aspectRatio);
            }
            if (View != null)
            {
                (View as UIElement).InvalidateMeasure();
            }

            MatrixExt scaledMatrix = MatrixExt.Identity;
            scaledMatrix.RotateAt(-RotateAngle, pivot.Value.X, pivot.Value.Y);
            scaledMatrix.ScaleAt(deltaWidth, deltaHeight, pivot.Value.X, pivot.Value.Y);
            scaledMatrix.RotateAt(RotateAngle, pivot.Value.X, pivot.Value.Y);
            Point newPosition = scaledMatrix.Transform(new Point(OffsetX, OffsetY));

            tempX = newPosition.X;
            tempY = newPosition.Y;
            tempW = newWidth;
            tempH = newHeight;
        }

        private void ScaleDelta(
            double deltaWidth,
            double deltaHeight,
            Point pivot,
            bool aspectRatio = false)
        {
            //if (InternalNodes != null)
            {
                //foreach (var node in InternalNodes)
                foreach (IInternalGroupable item in GetSelectedItems())
                {
                    if (item is IInternalNode)
                    {
                        IInternalNode node =
                            item as IInternalNode;

                        //MatrixExt newMatrix = MatrixExt.Identity;
                        ////newMatrix.Rotate(RotateAngle);
                        //Point pivotDelta = new Point(pivot.X - OffsetX, pivot.Y - OffsetY);
                        ////pivotDelta = newMatrix.Transform(pivotDelta);

                        //newMatrix = MatrixExt.Identity;
                        //newMatrix.RotateAt(-RotateAngle, pivot.X, pivot.Y);
                        //newMatrix.ScaleAt(deltaWidth, deltaHeight, pivot.X, pivot.Y);
                        //newMatrix.RotateAt(RotateAngle, pivot.X, pivot.Y);

                        //newMatrix.RotateAt(-RotateAngle, OffsetX - pivot.X, OffsetY - pivot.Y);
                        //newMatrix.ScaleAt(deltaWidth, deltaHeight, OffsetX - pivot.X, OffsetY - pivot.Y);
                        //newMatrix.RotateAt(RotateAngle, OffsetX - pivot.X, OffsetY - pivot.Y);

                        //newMatrix.Rotate(-RotateAngle);
                        //newMatrix.Scale(deltaWidth, deltaHeight);
                        //newMatrix.Rotate(RotateAngle);

                        //Point delta = new Point(node.OffsetX - OffsetX, node.OffsetY - OffsetY);
                        //Point transDelta = newMatrix.Transform(delta);
                        //node.OffsetX += (transDelta.X - delta.X);
                        //node.OffsetY += (transDelta.Y - delta.Y);

                        //Point newPosition = newMatrix.Transform(new Point(node.OffsetX, node.OffsetX));
                        //node.OffsetX = newPosition.X;
                        //node.OffsetY = newPosition.Y;
                        //node.Width *= deltaWidth;
                        //node.Height *= deltaHeight;

                        if (node.CanScale() &&
                            node.CanStartTransform(TransformState.ScaleX | TransformState.ScaleY))
                        {
                            node.ScaleTo(
                                Math.Max(MinWidth, node.ActualWidth) * deltaWidth,
                                Math.Max(MinHeight, node.ActualHeight) * deltaHeight,
                                pivot, aspectRatio);
                            node.EndTransform(TransformState.ScaleX | TransformState.ScaleY);
                        }
                    }
                    //node.ScaleTo(node.ActualWidth * deltaWidth,
                    //               node.ActualHeight * deltaHeight,
                    //               pivot, aspectRatio);
                }
            }
        }

        public override void MovePivotTo(Point newPivot)
        {
            Point orig = new Point(OffsetX, OffsetY);
            Point delta;
            GetPivotDelta(ref newPivot, out delta);

            ResetTo(orig.X + delta.X, orig.Y + delta.Y, RotateAngle, newPivot);
            tempPivot = newPivot;
        }

        public void AddRemoveItem(IInternalGroupable item, bool add)
        {
            if (item is IGroup)
            {
                IInternalGroup g =
                    item as IInternalGroup;
                if (add)
                {
                    //if (!InternalGroups.Contains(g))
                    {
                        //InternalGroups.Add(g);
                    }
                }
                else
                {
                    InternalGroups.Remove(g);
                }
            }
            else if (item is IConnector)
            {
                IInternalConnector e =
                    item as IInternalConnector;
                if (add)
                {
                    //if (!InternalConnectors.Contains(e))
                    {
                        //InternalConnectors.Add(e);
                    }
                }
                else
                {
                    InternalConnectors.Remove(e);
                }
            }
            else if (item is INode)
            {
                IInternalNode v =
                    item as IInternalNode;
                if (add)
                {
                    //if (!InternalNodes.Contains(v))
                    {
                        //InternalNodes.Add(v);
                    }
                }
                else
                {
                    InternalNodes.Remove(v);
                }
            }
        }

        Size IProtectedGroup.UpdateBounds()
        {
            return UpdateBounds();
        }

        protected virtual Size UpdateBounds()
        {
            if (this is ISelector)
            {
                Selector selector =
                    (this as IInternalSelector).View as
                    Selector;
                if (selector.nodeThumbDragging)
                {
                    ResetTo(tempX,
                            tempY,
                            tempA,
                            tempPivot);
                    return new Size(tempW, tempH);
                }
            }
            if ((TransformState & TransformState.Transforming) != TransformState.Transforming)
            {
                Rect newBounds = Rect.Empty;
                foreach (IInternalGroupable item in GetSelectedItems())
                {
                    MatrixExt groupMatrix = MatrixExt.Identity;
                    groupMatrix.Rotate(-RotateAngle);
                    Rect groupBounds;
                    Point[] corners = null;
                    if (item is INode && item.View !=null)
                    {
                        corners = new[]
                            {
                                item.Corners.Value.TopLeft,
                                item.Corners.Value.TopRight,
                                item.Corners.Value.BottomLeft,
                                item.Corners.Value.BottomRight
                            };
                    }
                    else if (item is IConnector && item.View != null)
                    {
                        corners = new[]
                        {
                            new Point(item.Bounds.Left, item.Bounds.Top), 
                            new Point(item.Bounds.Right, item.Bounds.Top), 
                            new Point(item.Bounds.Left, item.Bounds.Bottom), 
                            new Point(item.Bounds.Right, item.Bounds.Bottom)
                        };
                    }

                        double left = double.PositiveInfinity;
                        double top = double.PositiveInfinity;
                        double right = double.NegativeInfinity;
                        double bottom = double.NegativeInfinity;

                    if (corners != null)
                    {
                        for (int i = 0; i < corners.Length; i++)
                        {
                            var corner = corners[i];
                            Point trans = groupMatrix.Transform(corner);
                            corners[i] = trans;
                        }

                        foreach (Point corner in corners)
                        {
                            left = corner.X.Min(left, false);
                            top = corner.Y.Min(top, false);
                            right = corner.X.Max(right, false);
                            bottom = corner.Y.Max(bottom, false);
                        }
                    }
                    else
                    {
                        continue;
                    }
                    groupBounds = new Rect(new Point(left, top), new Point(right, bottom));

                    //groupBounds = GetTransformBounds(
                    //    new Point(0, 0),
                    //    ref dummyX, ref dummyY,
                    //    0, 0,
                    //    groupMatrix, ref corners);

                    //GetTransformBounds();
                    newBounds.Union(groupBounds);
                    //newBounds.Union(item.Corners.Value.OuterBounds);
                }
                if (!newBounds.IsEmpty)
                {
                    Point newPosition = new Point(newBounds.Left + newBounds.Width * Pivot.X,
                        newBounds.Top + newBounds.Height * Pivot.Y);
                    MatrixExt rotate = MatrixExt.Identity;
                    rotate.Rotate(RotateAngle);
                    newPosition = rotate.Transform(newPosition);

                    ResetTo(newPosition.X, newPosition.Y, RotateAngle, Pivot);
                    tempX = OffsetX;
                    tempY = OffsetY;
                    tempA = RotateAngle;
                    tempW = newBounds.Width;
                    tempH = newBounds.Height;
                    tempPivot = Pivot;
                }
                return new Size(newBounds.Width.Valid(), newBounds.Height.Valid());
            }
            return new Size(0, 0);
        }

        protected IEnumerable<IInternalGroupable> GetSelectedItems()
        {
            foreach (var item in GetSelectedNodeGroup())
            {
                yield return item;
            }
            if (InternalConnectors != null)
            {
                foreach (var connector in InternalConnectors)
                {
                    yield return connector;
                }
            }
        }

        protected IEnumerable<IInternalNode> GetSelectedNodeGroup()
        {
            if (InternalNodes != null)
            {
                foreach (var node in InternalNodes)
                {
                    yield return node;
                }
            }
            if (InternalGroups != null)
            {
                foreach (var group in InternalGroups)
                {
                    yield return group;
                }
            }
        }

        protected bool IsOneItemSelected(out IInternalGroupable firstItem)
        {
            firstItem = null;
            int cnt = 0;
            foreach (var item in GetSelectedItems())
            {
                firstItem = item;
                cnt++;
                if (cnt == 2)
                {
                    firstItem = null;
                    return false;
                }
            }
            if (firstItem == null)
            {
                return false;
            }
            return true;
        }

        public Point GetIntersection<T>(IInternalNode end)
        {
            throw new NotImplementedException();
        }

        protected override object RevertTo(object state)
        {
            if (state is Array)
            {
                var current = GetData();
                NodeChangedEventArgs toState = (NodeChangedEventArgs)(state as object[])[0];
                if (toState.RotateAngle != RotateAngle)
                {
                    RotateAngle = toState.RotateAngle;
                }
                if (toState.Pivot != Pivot)
                {
                    Pivot = toState.Pivot;
                }
                //if (toState.Width != Width)
                //{
                //    Width = toState.Width;
                //}
                //if (toState.Height != Height)
                //{
                //    Height = toState.Height;
                //}
                //if (toState.OffsetX != OffsetX)
                //{
                //    OffsetX = toState.OffsetX;
                //}
                //if (toState.OffsetY != OffsetY)
                //{
                //    OffsetY = toState.OffsetY;
                //}
                return current;
            }
            return state;
        }
    }
}
