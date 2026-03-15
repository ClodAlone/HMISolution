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
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#else
using System.Windows.Input;
using PointerRoutedEventArgs = System.Windows.Input.MouseEventArgs;
using TappedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Panels;

namespace Syncfusion.UI.Xaml.Diagram
{
    internal sealed partial class SelectorWrapper :
        GroupWrapper,
        IInternalSelector
    {
        SelectedEvent<IInternalGroupable> _mSelectedEvent;
        UnSelectedEvent<IInternalGroupable> _mUnSelectedEvent;

        private NodeDragStartingEvent _mDragStarting;
        private NodeDragEvent _mDragEvent;
        private bool _hasItems;

        public SelectorWrapper(SharedData shared)
            : base(shared)
        {
            Subsribe();
        }

        public override void Dispose()
        {
            base.Dispose();
            UnSubsribe();
        }

        public override void Tap(bool fireEvent)
        {
            base.Tap(false);
        }

        public override void DoubleTap(bool fireEvent)
        {
            base.DoubleTap(false);
        }

        private void Subsribe()
        {
            _mSelectedEvent = SharedData.EventAggregator.GetEvent<SelectedEvent<IInternalGroupable>>();
            _mUnSelectedEvent = SharedData.EventAggregator.GetEvent<UnSelectedEvent<IInternalGroupable>>();
            _mSelectedEvent.Subscribe(Selected, true);
            _mUnSelectedEvent.Subscribe(UnSelected, true);

            _mDragStarting = SharedData.EventAggregator.GetEvent<NodeDragStartingEvent>();
            _mDragEvent = SharedData.EventAggregator.GetEvent<NodeDragEvent>();

            _mDragStarting.Subscribe(DragStarting);
            _mDragEvent.Subscribe(Dragging);
        }

        private void UnSubsribe()
        {
            if (_mSelectedEvent != null) _mSelectedEvent.UnSubscribe(Selected);
            if (_mUnSelectedEvent != null) _mUnSelectedEvent.UnSubscribe(UnSelected);

            if (_mDragStarting != null) _mDragStarting.UnSubscribe(DragStarting);
            if (_mDragEvent != null) _mDragEvent.UnSubscribe(Dragging);
        }

        internal void DragStarting(PointerRoutedEventArgs args)
        {
            this.UpdateBounds();
            _InitialLocation = new Point(OffsetX, OffsetY);
            (View as Selector).HidePivotThumb();
        }

        Point _InitialLocation;
        internal void Dragging(NodeDragDeltaEventArgs args)
        {
            //if (args.Source is Node)
            {
                //Point? pivot = args.ManipulationArgs != null ? args.ManipulationArgs.Position : (Point?)null;
                //if (pivot != null)
                //{
                //    UIElement source = args.Source as UIElement;
                //    //pivot = new Point(OffsetX, OffsetY);
                //    //pivot = new Point(pivot.Value.X / args.ParentScale, pivot.Value.Y / args.ParentScale);
                //    //pivot = source.TransformToVisual(source.FindVisualParent<ZoomPanControl>()).TransformPoint(pivot.Value);
                //    MatrixExt sourceMatrix = MatrixExt.Identity;
                //    sourceMatrix.Rotate(RotateAngle);
                //    sourceMatrix.Translate(OffsetX, OffsetY);

                //    MatrixExt matrix = MatrixExt.Identity;
                //    Point center = sourceMatrix.Transform(pivot.Value);//new Point(pivot.Value.X / args.ParentScale, pivot.Value.Y / args.ParentScale); //sourceMatrix.Transform(pivot.Value);

                //    matrix.ScaleAt(args.ParentScale, args.ParentScale, center.X, center.Y);
                //    matrix.ScaleAt(args.Delta.ScaleChange, args.Delta.ScaleChange, center.X, center.Y);
                //    matrix.RotateAt(args.Delta.AngleChange, center.X, center.Y);
                //    matrix.Translate(args.Delta.HorizontalChange, args.Delta.VerticalChange);
                //    matrix.ScaleAt(1 / args.ParentScale, 1 / args.ParentScale, center.X, center.Y);

                //    //center = new Point(pivot.Value.X / args.ParentScale, pivot.Value.Y / args.ParentScale);

                //    sourceMatrix *= matrix;
                //    Point topleft = sourceMatrix.Transform(new Point(0, 0));
                //    Point topright = sourceMatrix.Transform(new Point(ActualWidth, 0));
                //    Point bottomLeft = sourceMatrix.Transform(new Point(0, ActualHeight));
                //    Point bottomRight = sourceMatrix.Transform(new Point(ActualWidth, ActualHeight));
                //    if (CanStartTransform(TransformState.Rotate))
                //    {
                //        RotateTo(topleft.FindAngle(topright), center);
                //        EndTransform(TransformState.Rotate);
                //    }
                //    OffsetX = topleft.X;
                //    OffsetY = topleft.Y;
                //    if (CanStartTransform(ScaleXY))
                //    {
                //        ScaleTo(topleft.FindLength(topright), topleft.FindLength(bottomLeft), center);
                //        EndTransform(ScaleXY);
                //    }
                //}

                //Point? pivot = args.ManipulationArgs != null ? args.ManipulationArgs.Position : (Point?) null;
                //if (CanStartTransform(TransformState.Rotate))
                //{
                //    RotateTo(RotateAngle + args.Delta.AngleChange, pivot);
                //    EndTransform(TransformState.Rotate);
                //}
                //OffsetX += args.Delta.HorizontalChange;
                //OffsetY += args.Delta.VerticalChange;
                //if (CanStartTransform(ScaleXY))
                //{
                //    ScaleTo(ActualWidth*args.Delta.ScaleChange, ActualHeight*args.Delta.ScaleChange, pivot);
                //    EndTransform(ScaleXY);
                //}
                UserInteracting = true;
                if (this.Constraints.Contains(NodeConstraints.Draggable))
                {


                    SharedData.Adorner.ClearGuidelines();

                    SnapParameter _horizontalSnap = null;

                    SnapParameter _verticalSnap = null;

                    Point originalPosition = new Point(_InitialLocation.X + args.Cumulative.HorizontalChange,
                        _InitialLocation.Y + args.Cumulative.VerticalChange);

                    //Point originalPosition = 
                    List<SnapParameter> Snaps = new List<SnapParameter>();

                    if (this.InternalNodes.Count() == 1)
                    {
                        //SharedData.Guidelines.SnapToInOutNeighbours(InternalNodes.ElementAt(0), originalPosition, ref _horizontalSnap, ref _verticalSnap);

                        SharedData.SnapSettingsController.FindTargetConnectors(InternalNodes.ElementAt(0),
                            originalPosition.X - OffsetX, originalPosition.Y - OffsetY, ref Snaps);

                        if (CanCreateGuidelines() && (_horizontalSnap == null || _verticalSnap == null))
                        {
                            SharedData.SnapSettingsController.FindPossipleSnaps(this.InternalNodes.ElementAt(0), originalPosition,
                                ref _horizontalSnap, ref _verticalSnap);
                        }
                    }


                    if (SharedData.SnapSettingsController.CanSnapToVerticalGridlines(Constraints) && _horizontalSnap == null)
                    {
                        if (originalPosition.X < OffsetX)
                        {
                            SnapToVerticalGuidelines("left", ref _horizontalSnap, originalPosition.X);
                        }
                        else if (originalPosition.X > OffsetX)
                        {
                            SnapToVerticalGuidelines("right", ref _horizontalSnap, originalPosition.X);
                        }
                    }
                    else if (_horizontalSnap == null)
                    {
                        OffsetX = originalPosition.X;
                    }
                    if (SharedData.SnapSettingsController.CanSnapToHorizontalGridlines(Constraints) && _verticalSnap == null)
                    {
                        if (originalPosition.Y < OffsetY)
                        {
                            SnapToHorizontalGridlines("top", ref _verticalSnap, originalPosition.Y);

                        }
                        else if (originalPosition.Y > OffsetY)
                        {
                            SnapToHorizontalGridlines("bottom", ref _verticalSnap, originalPosition.Y);
                        }
                    }
                    else if (_verticalSnap == null)
                    {
                        OffsetY = originalPosition.Y;
                    }
                    if (_horizontalSnap != null && _horizontalSnap.SnapInfo != null)
                    {
                        Snaps.Add(_horizontalSnap);
                    }
                    if (_verticalSnap != null && _verticalSnap.SnapInfo != null)
                    {
                        Snaps.Add(_verticalSnap);
                    }
                    if (Snaps.Count > 0)
                    {
                        if (this.InternalNodes.Count() == 1 && InternalConnectors.Count == 0 && InternalGroups.Count == 0)
                            SharedData.SnapSettingsController.SnapPosition(Snaps, InternalNodes.ElementAt(0));
                        else
                            SharedData.SnapSettingsController.SnapPosition(Snaps, this);
                    }
                }
                UserInteracting = false;
            }
        }

        private void SnapToHorizontalGridlines(string direction, ref SnapParameter verticalSnap, double originalposition)
        {
            double delta = originalposition - OffsetY;
            double bottom = this.Bounds.Bottom + delta;
            double roundedBottom = bottom.Round(SharedData.SnapSettingsController.GetHorizontalSnapInterval());
            double top = this.Bounds.Top + delta;
            double roundedTop = top.Round(SharedData.SnapSettingsController.GetHorizontalSnapInterval());
            double? _newValue;
            double? _currentValue;
            if (direction.Equals("top"))
            {
                if (Math.Abs(roundedTop - top) <= Math.Abs(roundedBottom - bottom))
                {
                    _newValue = OffsetY + roundedTop - this.Bounds.Top;
                    _currentValue = OffsetY;
                    verticalSnap = new SnapParameter(
                        SnapReason.GridLine,
                        SnapChanges.Y,
                        new SnapState(null, _currentValue, null, null, null),
                        new SnapState(null, _newValue, null, null, null),
                        new GridlineSnapInfo(roundedTop, Side.Top));
                }
                else
                {
                    _newValue = OffsetY + roundedBottom - this.Bounds.Bottom;
                    _currentValue = OffsetY;
                    verticalSnap = new SnapParameter(
                        SnapReason.GridLine,
                        SnapChanges.Y,
                        new SnapState(null, _currentValue, null, null, null),
                        new SnapState(null, _newValue, null, null, null),
                        new GridlineSnapInfo(roundedBottom, Side.Bottom));
                }
            }
            else if (direction.Equals("bottom"))
            {
                if (Math.Abs(roundedBottom - bottom) <= Math.Abs(roundedTop - top))
                {
                    _newValue = OffsetY + roundedBottom - this.Bounds.Bottom;
                    _currentValue = OffsetY;
                    verticalSnap = new SnapParameter(
                        SnapReason.GridLine,
                        SnapChanges.Y,
                        new SnapState(null, _currentValue, null, null, null),
                        new SnapState(null, _newValue, null, null, null),
                        new GridlineSnapInfo(roundedBottom, Side.Bottom));
                }
                else
                {
                    _newValue = OffsetY + roundedTop - this.Bounds.Top;
                    _currentValue = OffsetY;
                    verticalSnap = new SnapParameter(
                        SnapReason.GridLine,
                        SnapChanges.Y,
                        new SnapState(null, _currentValue, null, null, null),
                        new SnapState(null, _newValue, null, null, null),
                        new GridlineSnapInfo(roundedTop, Side.Top));
                }
            }
        }

        private void SnapToVerticalGuidelines(string direction, ref SnapParameter horizontalSnap, double originalposition)
        {
            double delta = originalposition - OffsetX;
            double left = this.Bounds.Left + delta;
            double roundedLeft = left.Round(SharedData.SnapSettingsController.GetVerticalSnapInterval());

            double right = this.Bounds.Right + delta;
            double roundedRight = right.Round(SharedData.SnapSettingsController.GetVerticalSnapInterval());

            double? _current;
            double? _new;
            if (direction.Equals("left"))
            {
                if (Math.Abs(roundedLeft - left) <= Math.Abs(roundedRight - right))
                {
                    _current = OffsetX;
                    _new = OffsetX + (roundedLeft - this.Bounds.Left);
                    horizontalSnap = new SnapParameter(
                        SnapReason.GridLine,
                        SnapChanges.X,
                        new SnapState(_current, null, null, null, null),
                        new SnapState(_new, null, null, null, null),
                        new GridlineSnapInfo(roundedLeft, Side.Left));
                }
                else
                {
                    _current = OffsetX;
                    _new = OffsetX + (roundedRight - this.Bounds.Right);
                    horizontalSnap = new SnapParameter(
                        SnapReason.GridLine,
                        SnapChanges.X,
                        new SnapState(_current, null, null, null, null),
                        new SnapState(_new, null, null, null, null),
                        new GridlineSnapInfo(roundedRight, Side.Right));
                }
            }
            else if (direction.Equals("right"))
            {
                if (Math.Abs(roundedRight - right) <= Math.Abs(roundedLeft - left))
                {
                    _current = OffsetX;
                    _new = OffsetX + (roundedRight - this.Bounds.Right);
                    horizontalSnap = new SnapParameter(
                        SnapReason.GridLine,
                        SnapChanges.X,
                        new SnapState(_current, null, null, null, null),
                        new SnapState(_new, null, null, null, null),
                        new GridlineSnapInfo(roundedRight, Side.Right));
                }
                else
                {
                    _current = OffsetX;
                    _new = OffsetX + (roundedLeft - this.Bounds.Left);
                    horizontalSnap = new SnapParameter(
                        SnapReason.GridLine,
                        SnapChanges.X,
                        new SnapState(_current, null, null, null, null),
                        new SnapState(_new, null, null, null, null),
                        new GridlineSnapInfo(roundedLeft, Side.Left));
                }
            }
        }

        private bool CanCreateGuidelines()
        {
            if (this.Constraints.Contains(NodeConstraints.InheritSnapToObject))
                return !(SharedData.Graph.SnapSettings.SnapToObject.Contains(SnapToObject.None) ||
                    SharedData.Graph.SnapSettings.SnapToObject == SnapToObject.None);
            else
                return !this.SnapToObject.Contains(SnapToObject.None);
        }

        private const TransformState ScaleXY = TransformState.ScaleX | TransformState.ScaleY;

        public void MovePivotDelta(Point delta)
        {
            double x = (ActualWidth * Pivot.X + delta.X) / ActualWidth;
            double y = (ActualHeight * Pivot.Y + delta.Y) / ActualHeight;
            //if (CanStartTransform(TransformState.TranslateX | TransformState.TranslateY))
            {
                //MovePivotTo(new Point(x, y));
                Pivot = new Point(x, y);
                //ResetTo(OffsetX, OffsetY, RotateAngle, new Point(x, y));
                //EndTransform(TransformState.TranslateX | TransformState.TranslateY);
            }
        }

        public override void SetActualSize(Size actualSize)
        {
            double currentZoom =SharedData.ScrollViewer.CurrentZoom;
            ActualWidth = actualSize.Width / currentZoom;
            ActualHeight = actualSize.Height / currentZoom;
            _mCurrentState.Width = actualSize.Width / currentZoom;
            _mCurrentState.Height = actualSize.Height / currentZoom;
        }

    
        public void ClearSelection()
        {
            InternalNodes.Clear();
            InternalConnectors.Clear();
            InternalGroups.Clear();
        }

        internal void Selected(SelectionArgs<IInternalGroupable> args)
        {
            if (args.ClearSelection)
            {
                ClearSelection();
            }
            if (args.Source is IGroup)
            {
                IWrapper wrapper = args.Source;
                SharedData.Graph.OnItemSelectedEvent(new DiagramEventArgs(wrapper.Source));
                InternalGroups.Add(args.Source as IInternalGroup, ItemSource.UnKnown);
            }
            else if (args.Source is IConnector)
            {
                IWrapper wrapper = args.Source;
                SharedData.Graph.OnItemSelectedEvent(new DiagramEventArgs(wrapper.Source));
                InternalConnectors.Add(args.Source as IInternalConnector, ItemSource.UnKnown);
            }
            else if (args.Source is INode)
            {
                IWrapper wrapper = args.Source;
                SharedData.Graph.OnItemSelectedEvent(new DiagramEventArgs(wrapper.Source));
                InternalNodes.Add(args.Source as IInternalNode, ItemSource.UnKnown);

            }
            UpdateSelector();
            selectionChanged = true;
        }
        bool selectionChanged = false;
        private void UpdateSelector()
        {
            int cnt = 0;
            foreach (var item in GetSelectedItems())
            {
                cnt++;
                if (item is INode || cnt > 1)
                {
                    HasItems = true;
                    Invalidate();
                    SelectedConnector = null;
                    return;
                }
            }
            HasItems = false;
            var con = GetSelectedItems().FirstOrDefault() as IInternalConnector;
            SelectedConnector = con;
        }

        internal void UnSelected(SelectionArgs<IInternalGroupable> args)
        {
            if (args.ClearSelection)
            {
                InternalNodes.Clear();
                InternalConnectors.Clear();
                InternalGroups.Clear();
                HasItems = false;
                Invalidate();
                if (!(args.Source is IConnector || args.Source is INode || args.Source is IGroup))
                    return;
            }
            if (args.Source is IGroup)
            {
                IWrapper wrapper = args.Source;
                SharedData.Graph.OnItemUnSelectedEvent(new DiagramEventArgs(wrapper.Source));
                InternalGroups.Remove(args.Source as IInternalGroup);
            }

            else if (args.Source is IConnector)
            {
                IWrapper wrapper = args.Source;
                SharedData.Graph.OnItemUnSelectedEvent(new DiagramEventArgs(wrapper.Source));
                InternalConnectors.Remove(args.Source as IInternalConnector);
            }
            else if (args.Source is INode)
            {
                IWrapper wrapper = args.Source;
                SharedData.Graph.OnItemUnSelectedEvent(new DiagramEventArgs(wrapper.Source));
                InternalNodes.Remove(args.Source as IInternalNode);
            }
            UpdateSelector();

        }

        public bool HasItems
        {
            get { return _hasItems; }
            private set
            {
                _hasItems = value;
                UIElement view = View as UIElement;
                if (view != null)
                {
                    view.Visibility = _hasItems ? Visibility.Visible : Visibility.Collapsed;
                    Invalidate();
                }
            }
        }

        public override void MovePivotTo(Point newPivot)
        {
            IInternalGroupable firstItem;
            if (IsOneItemSelected(out firstItem))
            {
                if (firstItem is IInternalNode)
                {
                    Point delta;
                    GetPivotDelta(ref newPivot, out delta);
                    (firstItem as IInternalNode).Pivot = newPivot;
                }
            }
            else
            {
                base.MovePivotTo(newPivot);
            }
        }

        protected override Size UpdateBounds()
        {
            IInternalGroupable firstItem;
            Size size = new Size(0, 0);
            if (!GetSelectedItems().Any())
            {
            }
            else if (IsOneItemSelected(out firstItem))
            {
                if (firstItem is INode)
                {
                    IInternalNode item =
                        firstItem as IInternalNode;
                    //if (CanStartTransform(TransformState.Transforming))
                    {


                        //MovePivotTo(item.Pivot, false);
                        //RotateTo(item.RotateAngle, null, false);
                        //DragXTo(item.OffsetX, false);
                        //DragYTo(item.OffsetY, false);
                        //ScaleTo(item.ActualWidth, item.ActualHeight, null, false, false);

                        ResetTo(item.OffsetX, item.OffsetY, item.RotateAngle, item.Pivot);

                        this.Constraints = item.Constraints;
                        this.SnapToObject = item.SnapToObject;
                        //EndTransform(TransformState.Transforming);
                    }
                    //SelectedConnector = null;
                    size = new Size(item.ActualWidth, item.ActualHeight);
                }
                else if (firstItem is IConnector)
                {
                    IInternalConnector item =
                        firstItem as IInternalConnector;

                    //MovePivotTo(new Point(0, 0), false);
                    //RotateTo(0, null, false);
                    //DragXTo(item.Bounds.Left, false);
                    //DragYTo(item.Bounds.Top, false);
                    //ScaleTo(item.Bounds.Width, item.Bounds.Height, null, false, false);

                    ResetTo(item.Bounds.Left, item.Bounds.Top, 0, new Point(0, 0));

                    //SelectedConnector = item;
                    //throw new NotImplementedException();
                    size = new Size(item.Bounds.Width, item.Bounds.Height);
                }
            }
            else
            {
                this.Constraints = NodeConstraints.Default;
                if (View != null)
                {
                    //SelectedConnector = null;
                }
                size = base.UpdateBounds();
            }
            if (selectionChanged)
            {
                if (size.Width > 0 && size.Height > 0)
                {
                    SetDesiredSize(size);
                    SetActualSize(size);
                    (this.View as Selector).OnSelectorSizeChanged(null, "");
                    selectionChanged = false;
                }
            }
            return size;
        }

        public IInternalGroupable FirstSelectedItem
        {
            get;
            set;
        }

        private IInternalConnector SelectedConnector
        {
            set { SharedData.Adorner.SelectConnector(value); }
        }

        public void ManualPressTargetThumb(MouseEventArgs e)
        {
            SharedData.Adorner.ManualPressTargetThumb(e);
        }
    }
}
