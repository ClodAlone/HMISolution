#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.UI.Xaml.Diagram.Panels;
using Syncfusion.UI.Xaml.Diagram.Utility;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls;
using System.Windows.Media; 
#endif
using System.ComponentModel;
using ScrollViewer = Syncfusion.UI.Xaml.Diagram.Controls.ScrollViewer;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Diagram
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public partial class Selector :
        Group,
        ISelectorView,
        ISelector
    {
        private DiagramThumb _mPivotThumb;
        private DiagramThumb _mRotatorThumb;
        private DiagramThumb[] _mResizerThumbs;

#if WPF
        static Selector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Selector), new FrameworkPropertyMetadata(typeof(Selector)));
        } 
#endif
        private void OnQuickCommandsChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        public IDiagramCommands DiagramCommands
        {
            get { return (IDiagramCommands)GetValue(DiagramCommandsProperty); }
        }

        // Using a DependencyProperty as the backing store for DiagramCommands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DiagramCommandsProperty =
            DependencyProperty.Register("DiagramCommands", typeof(IDiagramCommands), typeof(Selector), new PropertyMetadata(null));

        public Selector()
        {
#if !WPF
            this.DefaultStyleKey = typeof(Selector);
#endif

            this.PropertyChanged += Selector_PropertyChanged;
            this.Loaded += Selector_Loaded;
        }

        void Selector_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Selector_Loaded;

            if (InternalSelector.HasItems)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        void Selector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Pivot")
            {
                if (_mRotatorThumb != null && _mPivotThumb != null)
                {
                    Canvas.SetLeft(_mRotatorThumb, Pivot.X);
                    Canvas.SetLeft(_mPivotThumb, Pivot.X);
                    Canvas.SetTop(_mPivotThumb, Pivot.Y);
                    if (_mPivotThumb.Parent is Panel)
                    {
                        (_mPivotThumb.Parent as Panel).InvalidateArrange();
                    }
                }
            }
        }

        private IInternalSelector InternalSelector
        {
            get { return Wrapper as IInternalSelector; }
        }

        internal override void SetWrapper(IProtectedNode value)
        {
            base.SetWrapper(value);
            if (InternalSelector != null)
            {
                SetValue(DiagramCommandsProperty, InternalSelector.SharedData.Commands);
            }
        }

        private void DoApplyTemplate()
        {
            DiagramThumb topLeft = GetTemplateChild("PART_TopLeft") as DiagramThumb;
            if (topLeft != null) topLeft.Corner = Corner.TopLeft;
            DiagramThumb top = GetTemplateChild("PART_Top") as DiagramThumb;
            if (top != null) top.Corner = Corner.Top;
            DiagramThumb topRight = GetTemplateChild("PART_TopRight") as DiagramThumb;
            if (topRight != null) topRight.Corner = Corner.TopRight;
            DiagramThumb left = GetTemplateChild("PART_Left") as DiagramThumb;
            if (left != null) left.Corner = Corner.Left;
            DiagramThumb right = GetTemplateChild("PART_Right") as DiagramThumb;
            if (right != null) right.Corner = Corner.Right;
            DiagramThumb bottomLeft = GetTemplateChild("PART_BottomLeft") as DiagramThumb;
            if (bottomLeft != null) bottomLeft.Corner = Corner.BottomLeft;
            DiagramThumb bottom = GetTemplateChild("PART_Bottom") as DiagramThumb;
            if (bottom != null) bottom.Corner = Corner.Bottom;
            DiagramThumb bottomRight = GetTemplateChild("PART_BottomRight") as DiagramThumb;
            if (bottomRight != null) bottomRight.Corner = Corner.BottomRight;

            _mResizerThumbs = new DiagramThumb[]
                {
                    topLeft,
                    top,
                    topRight,
                    left,
                    right,
                    bottomLeft,
                    bottom,
                    bottomRight
                };

            foreach (var diagramThumb in _mResizerThumbs)
            {
                if (diagramThumb != null)
                {
                    diagramThumb.DragStarting += thumb_DragStarting;
                    diagramThumb.DragDelta += resizer_DragDelta;
                    diagramThumb.DragComplete += resizer_DragDelta;
                    diagramThumb.DragComplete += thumb_DragCompleted;
                }
            }

            _mPivotThumb = GetTemplateChild("PART_Pivot") as DiagramThumb;
            if (_mPivotThumb != null)
            {
                Canvas.SetLeft(_mPivotThumb, Pivot.X);
                Canvas.SetTop(_mPivotThumb, Pivot.Y);
                _mPivotThumb.DragStarting += thumb_DragStarting;
                _mPivotThumb.DragDelta += pivotThumb_DragComplete;
                _mPivotThumb.DragComplete += pivotThumb_DragComplete;
                _mPivotThumb.DragComplete += thumb_DragCompleted;
            }

            _mRotatorThumb = GetTemplateChild("PART_Rotator") as DiagramThumb;
            if (_mRotatorThumb != null)
            {
                Canvas.SetLeft(_mRotatorThumb, Pivot.X);
                _mRotatorThumb.DragStarting += thumb_DragStarting;
                _mRotatorThumb.DragDelta += rotatorThumb_DragDelta;
                _mRotatorThumb.DragComplete += rotatorThumb_DragDelta;
                _mRotatorThumb.DragComplete += thumb_DragCompleted;
            }
        }

        internal bool nodeThumbDragging = false;

        private void thumb_DragCompleted(object sender, DiagramThumbDragEventArgs args)
        {
            Wrapper.UserInteracting = true;
            nodeThumbDragging = false;
            if (_mResizerThumbs.Contains(sender) || sender == _mRotatorThumb)
            {
                if (_mResizerThumbs.Contains(sender))
                {
                    OnSelectorSizeChanged(sender as DiagramThumb, "Stop");
                }
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
            if (_mSharedData.UndoRedoController != null) _mSharedData.UndoRedoController.EndComposite();
            Wrapper.UserInteracting = false;
        }

        private void thumb_DragStarting(object sender, DiagramThumbDragStartingEventArgs args)
        {
            Wrapper.UserInteracting = true;
            nodeThumbDragging = true;
            if (_mSharedData.UndoRedoController != null) _mSharedData.UndoRedoController.BeginComposite();
            NodeConstraints constraints = Wrapper.Constraints;

            if (sender == _mRotatorThumb && constraints.Contains(NodeConstraints.Rotatable))
            {
                _mPivotThumb.Visibility = Visibility.Visible;
            }
            else if (_mResizerThumbs.Contains(sender) && constraints.Contains(NodeConstraints.Resizable))
            {
                OnSelectorSizeChanged(sender as DiagramThumb, "Start");
                _mPivotThumb.Visibility = Visibility.Collapsed;
            }
            else if (sender == _mPivotThumb)
            {
            }
            else
            {
                args.Cancel = true;
                nodeThumbDragging = false;
                return;
            }

            args.InitialValue = new Point(InternalSelector.ActualWidth, InternalSelector.ActualHeight);
            Wrapper.UserInteracting = false;
        }

        internal virtual void OnSelectorSizeChanged(DiagramThumb thumb, string action)
        {
            if (thumb == null)
            {
                UpdateSelector(null);
            }
            double width = InternalSelector.DesiredSize.Width * _mSharedData.ScrollViewer.CurrentZoom;
            double height = InternalSelector.DesiredSize.Height * _mSharedData.ScrollViewer.CurrentZoom;
            if (action.Equals("Stop"))
            {
                if (width < 50 || height < 50)
                {
                    switch (thumb.Corner)
                    {
                        case Corner.Top:
                            _mResizerThumbs[1].Visibility = Visibility.Collapsed;
                            break;
                        case Corner.Bottom:
                            _mResizerThumbs[6].Visibility = Visibility.Collapsed;
                            break;
                        case Corner.Left:
                            _mResizerThumbs[3].Visibility = Visibility.Collapsed;
                            break;
                        case Corner.Right:
                            _mResizerThumbs[4].Visibility = Visibility.Collapsed;
                            break;
                    }
                }
            }
            else if (action.Equals("delta"))
            {
                UpdateSelector(thumb);
            }
        }

        bool _resized = false;

        void UpdateSelector(DiagramThumb thumb)
        {
            if (thumb == null)
            {
                if (_mResizerThumbs == null)
                {
                    return;
                }
            }
            double width = InternalSelector.DesiredSize.Width * _mSharedData.ScrollViewer.CurrentZoom;
            double height = InternalSelector.DesiredSize.Height * _mSharedData.ScrollViewer.CurrentZoom;
            if (width < 30 || height < 30)
            {
                foreach (DiagramThumb thum in _mResizerThumbs)
                {
                    if (thum != thumb)
                    {
                        thum.Visibility = Visibility.Collapsed;
                    }
                }

                if (thumb != null && (width >= 30 || height >= 30))
                {
                    switch (thumb.Corner)
                    {
                        case Corner.TopLeft:
                            _mResizerThumbs[7].Visibility = Visibility.Visible;
                            break;
                        case Corner.TopRight:
                            _mResizerThumbs[5].Visibility = Visibility.Visible;
                            break;
                        case Corner.BottomRight:
                            _mResizerThumbs[0].Visibility = Visibility.Visible;
                            break;
                        case Corner.BottomLeft:
                            _mResizerThumbs[2].Visibility = Visibility.Visible;
                            break;
                        case Corner.Bottom:
                        case Corner.Top:
                            _mResizerThumbs[7].Visibility = Visibility.Visible;
                            _mResizerThumbs[0].Visibility = Visibility.Visible;
                            break;
                        case Corner.Left:
                        case Corner.Right:
                            _mResizerThumbs[5].Visibility = Visibility.Visible;
                            _mResizerThumbs[2].Visibility = Visibility.Visible;
                            break;
                    }

                }
                else if (thumb == null)
                {
                    if (width >= 30 || height >= 30)
                        _mResizerThumbs[0].Visibility = Visibility.Visible;
                    _mResizerThumbs[7].Visibility = Visibility.Visible;

                }
                _resized = true;
            }
            else if (width < 50 && width >= 30
                || height < 50 && height >= 30)
            {
                _mResizerThumbs[1].Visibility = Visibility.Collapsed;
                _mResizerThumbs[6].Visibility = Visibility.Collapsed;
                _mResizerThumbs[3].Visibility = Visibility.Collapsed;
                _mResizerThumbs[4].Visibility = Visibility.Collapsed;
                if (thumb != null)
                {
                    switch (thumb.Corner)
                    {
                        case Corner.Top:
                            _mResizerThumbs[1].Visibility = Visibility.Visible;
                            break;
                        case Corner.Bottom:
                            _mResizerThumbs[6].Visibility = Visibility.Visible;
                            break;
                        case Corner.Left:
                            _mResizerThumbs[3].Visibility = Visibility.Visible;
                            break;
                        case Corner.Right:
                            _mResizerThumbs[4].Visibility = Visibility.Visible;
                            break;
                    }
                }
                if (thumb == null || _resized)
                {
                    _mResizerThumbs[0].Visibility = Visibility.Visible;
                    _mResizerThumbs[2].Visibility = Visibility.Visible;
                    _mResizerThumbs[5].Visibility = Visibility.Visible;
                    _mResizerThumbs[7].Visibility = Visibility.Visible;
                }

                _resized = false;
            }
            else if (width >= 50 || height >= 50)
            {
                foreach (DiagramThumb thum in _mResizerThumbs)
                {
                    thum.Visibility = Visibility.Visible;
                }
                _resized = false;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double currentZoom = _mSharedData.ScrollViewer.CurrentZoom;
            Size desired = base.MeasureOverride(availableSize);
            Wrapper.UpdateCompositeTrans();
            ArrangeTransform.TranslateX = (Wrapper.OffsetX - desired.Width * Pivot.X) * currentZoom;
            ArrangeTransform.TranslateY = (Wrapper.OffsetY - desired.Height * Pivot.Y) * currentZoom;
            ArrangeTransform.Rotation = RotateAngle;
            double width = desired.Width * currentZoom;
            double height = desired.Height * currentZoom;
            return new Size(width, height);
        }

        internal void HidePivotThumb()
        {
            if (_mPivotThumb != null)
                _mPivotThumb.Visibility = Visibility.Collapsed;
        }

        void rotatorThumb_DragDelta(object sender, DiagramThumbDragEventArgs args)
        {
            Wrapper.UserInteracting = true;
            List<SnapParameter> snaps = new List<SnapParameter>();
            Point s = new Point(Wrapper.OffsetX, Wrapper.OffsetY);
            Point e = args.PointerArgs.GetCurrentPoint(args.Container).Position;
            double angle = s.FindAngle(e) + 90;

            if (_mSharedData.SnapSettingsController.CanSnapAngle(Constraints))
            {
                double? current = angle;
                double? roundedAngle = angle.Round(_mSharedData.Graph.SnapSettings.SnapAngle);
                if (angle != roundedAngle)
                {
                    snaps.Add(new SnapParameter(
                        SnapReason.Angle,
                        SnapChanges.Angle,
                        new SnapState(null, null, null, null, current),
                        new SnapState(null, null, null, null, roundedAngle),
                        null));
                }
                _mSharedData.SnapSettingsController.SnapRotationAngle(snaps, this.Wrapper);

            }
            else
                InternalSelector.RotateAngle = angle;

            //Wrapper.UserInteracting = false;
        }


        void pivotThumb_DragComplete(object sender, DiagramThumbDragEventArgs args)
        {
            Wrapper.UserInteracting = true;
            MatrixExt inverse = MatrixExt.Identity;
            inverse.Rotate(-InternalSelector.RotateAngle);
            Point delta = inverse.Transform(new Point(args.Delta.HorizontalChange, args.Delta.VerticalChange));
            InternalSelector.MovePivotDelta(delta);
            this.InvalidateMeasure();
            if (_mSharedData.UndoRedoController != null) _mSharedData.UndoRedoController.EndComposite();
            Wrapper.UserInteracting = false;
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

        private void resizer_DragDelta(object sender, DiagramThumbDragEventArgs args)
        {
            _mSharedData.Adorner.ClearGuidelines();
            Wrapper.UserInteracting = true;
            DiagramThumb thumb = sender as DiagramThumb;
            Point cumDelta = new Point(args.Cumulative.HorizontalChange, args.Cumulative.VerticalChange);
            MatrixExt inverse = MatrixExt.Identity;
            inverse.Rotate(-InternalSelector.RotateAngle);
            cumDelta = inverse.Transform(cumDelta);
            if (InternalSelector.CanStartTransform(TransformState.ScaleX | TransformState.ScaleY))
            {
                double? newWidth = args.StartingArgs.InitialValue.Value.X;
                double? newHeight = args.StartingArgs.InitialValue.Value.Y;
                Point pivot = new Point(0, 0);
                //RectCorners internalSelectorCorners = InternalSelector.ActualCorners.Value;
                MatrixExt trans = MatrixExt.Identity;
                Point pt = InternalSelector.Pivot;
                double x = InternalSelector.OffsetX;
                double y = InternalSelector.OffsetY;
                trans.RotateAt(InternalSelector.RotateAngle, x, y);
                double w = InternalSelector.DesiredSize.Width;
                double h = InternalSelector.DesiredSize.Height;
                x = x - w * pt.X;
                y = y - h * pt.Y;
                List<SnapParameter> Snaps = new List<SnapParameter>();
                SnapParameter _horizontalSnap = null;
                SnapParameter _verticalSnap = null;
                switch (thumb.Corner)
                {
                    case Corner.TopLeft:
                        newWidth = Math.Max(1, args.StartingArgs.InitialValue.Value.X - cumDelta.X);
                        newHeight = Math.Max(1, args.StartingArgs.InitialValue.Value.Y - cumDelta.Y);
                        SnapTop(ref newHeight, ref _verticalSnap);
                        SnapLeft(ref newWidth, ref _horizontalSnap);
                        //pivot = internalSelectorCorners.BottomRight;
                        pivot = trans.Transform(new Point(x + w, y + h));
                        break;
                    case Corner.Top:
                        newHeight = Math.Max(1, args.StartingArgs.InitialValue.Value.Y - cumDelta.Y);
                        SnapTop(ref newHeight, ref _verticalSnap);
                        //pivot = internalSelectorCorners.Bottom;
                        pivot = trans.Transform(new Point(x + w / 2, y + h));
                        break;
                    case Corner.TopRight:
                        newWidth = Math.Max(1, args.StartingArgs.InitialValue.Value.X + cumDelta.X);
                        newHeight = Math.Max(1, args.StartingArgs.InitialValue.Value.Y - cumDelta.Y);
                        SnapTop(ref newHeight, ref _verticalSnap);
                        SnapRight(ref newWidth, ref _horizontalSnap);
                        //pivot = internalSelectorCorners.BottomLeft;
                        pivot = trans.Transform(new Point(x, y + h));
                        break;
                    case Corner.Left:
                        newWidth = Math.Max(1, args.StartingArgs.InitialValue.Value.X - cumDelta.X);
                        SnapLeft(ref newWidth, ref _horizontalSnap);
                        //pivot = internalSelectorCorners.Right;
                        pivot = trans.Transform(new Point(x + w, y + h / 2));
                        break;
                    case Corner.Right:
                        newWidth = Math.Max(1, args.StartingArgs.InitialValue.Value.X + cumDelta.X);
                        SnapRight(ref newWidth, ref _horizontalSnap);
                        //pivot = internalSelectorCorners.Left;
                        pivot = trans.Transform(new Point(x, y + h / 2));
                        break;
                    case Corner.BottomLeft:
                        newWidth = Math.Max(1, args.StartingArgs.InitialValue.Value.X - cumDelta.X);
                        newHeight = Math.Max(1, args.StartingArgs.InitialValue.Value.Y + cumDelta.Y);
                        SnapLeft(ref newWidth, ref _horizontalSnap);
                        SnapBottom(ref newHeight, ref _verticalSnap);
                        //pivot = internalSelectorCorners.TopRight;
                        pivot = trans.Transform(new Point(x + w, y));
                        break;
                    case Corner.Bottom:
                        newHeight = Math.Max(1, args.StartingArgs.InitialValue.Value.Y + cumDelta.Y);
                        SnapBottom(ref newHeight, ref _verticalSnap);
                        //pivot = internalSelectorCorners.Top;
                        pivot = trans.Transform(new Point(x + w / 2, y));
                        break;
                    case Corner.BottomRight:
                        newWidth = Math.Max(1, args.StartingArgs.InitialValue.Value.X + cumDelta.X);
                        newHeight = Math.Max(1, args.StartingArgs.InitialValue.Value.Y + cumDelta.Y);
                        SnapRight(ref newWidth, ref _horizontalSnap);
                        SnapBottom(ref newHeight, ref _verticalSnap);
                        //pivot = internalSelectorCorners.TopLeft;
                        pivot = trans.Transform(new Point(x, y));
                        break;
                }
                if (_horizontalSnap != null)
                    Snaps.Add(_horizontalSnap);
                if (_verticalSnap != null)
                    Snaps.Add(_verticalSnap);
                if (Snaps.Count > 0)
                {
                    if (InternalSelector.InternalNodes.Count == 1 && InternalSelector.InternalGroups.Count == 0
                        && InternalSelector.InternalConnectors.Count == 0)
                    {
                        _mSharedData.SnapSettingsController.SnapSize(Snaps,
                            InternalSelector.InternalNodes.ElementAt(0), ref newWidth, ref newHeight);
                    }
                    else
                    {
                        _mSharedData.SnapSettingsController.SnapSize(Snaps, this.Wrapper, ref newWidth, ref newHeight);
                    }
                }
                InternalSelector.ScaleTo(newWidth.Value, newHeight.Value, pivot);
                OnSelectorSizeChanged(thumb, "delta");
                InternalSelector.EndTransform(TransformState.ScaleX | TransformState.ScaleY);
            }
            // Wrapper.UserInteracting = false;
        }

        private void SnapLeft(ref double? newWidth, ref SnapParameter horizontalSnap)
        {
            if (InternalSelector.InternalNodes.Count == 1)
            {
                horizontalSnap = _mSharedData.SnapSettingsController.GetPossibleSnapsOnResizing(
                    InternalSelector.InternalNodes.ElementAt(0), newWidth, Side.Left);
            }
            if (_mSharedData.SnapSettingsController.CanSnapToVerticalGridlines(Constraints) && horizontalSnap == null)
            {
                double left = InternalSelector.Bounds.Left + (InternalSelector.Bounds.Width - newWidth.Value);
                double roundedleft = left.Round(_mSharedData.SnapSettingsController.GetVerticalSnapInterval());
                double? proposed = newWidth.Value - (roundedleft - left);
                horizontalSnap = new SnapParameter(
                    SnapReason.GridLine | SnapReason.Size,
                    SnapChanges.Width,
                    new SnapState(null, null, newWidth, null, null),
                    new SnapState(null, null, proposed, null, null),
                    new GridlineSnapInfo(roundedleft, Side.Left));
            }

        }

        private void SnapRight(ref double? newWidth, ref SnapParameter horizontalSnap)
        {
            if (InternalSelector.InternalNodes.Count == 1)
            {
                horizontalSnap = _mSharedData.SnapSettingsController.GetPossibleSnapsOnResizing(
                    InternalSelector.InternalNodes.ElementAt(0), newWidth, Side.Right);
            }

            if (_mSharedData.SnapSettingsController.CanSnapToVerticalGridlines(Constraints) && horizontalSnap == null)
            {
                double right = InternalSelector.Bounds.Right + (newWidth.Value - InternalSelector.Bounds.Width);
                double roundedRight = right.Round(_mSharedData.SnapSettingsController.GetVerticalSnapInterval());
                double? proposed = newWidth.Value + (roundedRight - right);
                horizontalSnap = new SnapParameter(
                    SnapReason.GridLine | SnapReason.Size,
                    SnapChanges.Width,
                    new SnapState(null, null, newWidth, null, null),
                    new SnapState(null, null, proposed, null, null),
                    new GridlineSnapInfo(roundedRight, Side.Right));
            }

        }

        private void SnapTop(ref double? newHeight, ref SnapParameter verticalSnap)
        {
            if (InternalSelector.InternalNodes.Count == 1)
            {
                verticalSnap = _mSharedData.SnapSettingsController.GetPossibleSnapsOnResizing(
                    InternalSelector.InternalNodes.ElementAt(0), newHeight, Side.Top);
            }
            if (_mSharedData.SnapSettingsController.CanSnapToHorizontalGridlines(Constraints) && verticalSnap == null)
            {
                double top = InternalSelector.Bounds.Top + (InternalSelector.Bounds.Height - newHeight.Value);
                double roundedTop = top.Round(_mSharedData.SnapSettingsController.GetHorizontalSnapInterval());
                double? proposed = newHeight.Value - (roundedTop - top);
                verticalSnap = new SnapParameter(
                    SnapReason.GridLine | SnapReason.Size,
                    SnapChanges.Height,
                    new SnapState(null, null, null, newHeight, null),
                    new SnapState(null, null, null, proposed, null),
                    new GridlineSnapInfo(roundedTop, Side.Top));
            }

        }

        private void SnapBottom(ref double? newHeight, ref SnapParameter verticalSnap)
        {
            if (InternalSelector.InternalNodes.Count == 1)
            {
                verticalSnap = _mSharedData.SnapSettingsController.GetPossibleSnapsOnResizing(
                    InternalSelector.InternalNodes.ElementAt(0), newHeight, Side.Bottom);
            }
            if (_mSharedData.SnapSettingsController.CanSnapToHorizontalGridlines(Constraints) && verticalSnap == null)
            {
                double bottom = InternalSelector.Bounds.Bottom + (newHeight.Value - InternalSelector.Bounds.Height);
                double roundedBottom = bottom.Round(_mSharedData.SnapSettingsController.GetHorizontalSnapInterval());
                double? proposed = newHeight.Value + (roundedBottom - bottom);
                verticalSnap = new SnapParameter(
                    SnapReason.GridLine | SnapReason.Size,
                    SnapChanges.Height,
                    new SnapState(null, null, null, newHeight, null),
                    new SnapState(null, null, null, proposed, null),
                    new GridlineSnapInfo(roundedBottom, Side.Bottom));
            }

        }

    }
}
