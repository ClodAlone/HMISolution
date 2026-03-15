using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Globalization;

namespace ScreenManager.Adorners
{
    abstract class BasicPointAdorner : BasicAdorner
    {
        readonly int nNumberFixedAdorners = 4; // number of fixed adorner before the point adorners

        TransformOriginThumb TransformOrigin = new TransformOriginThumb();
        protected DragThumbSelectedCenter dragControlSelectedCenter;

        #region Constructors

        protected BasicPointAdorner(UIElement parent, Shape element, bool canMove, IGridViewInfoService gridViewInfoService)
            : base(parent, element, canMove, gridViewInfoService)
        {
            Activate();

            TransformOrigin.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Properties
        protected abstract PointCollection Points { get; set; }
        protected abstract bool CanAddPoints { get; }

        protected Shape Element
        {
            get
            {
                return adornedElement as Shape;
            }
        }

        #endregion

        #region Methods

        protected Rect GetBoundingRect(bool bInflate = true)
        {
            Rect b = new Rect();
            b.Y = b.X = Double.MaxValue;
            b.Width = b.Height = 0;
            foreach (Point p in Points)
            {
                if (p.X < b.X)
                    b.X = p.X;
                if (p.Y < b.Y)
                    b.Y = p.Y;
                if (p.X > b.Width)
                    b.Width = p.X;
                if (p.Y > b.Height)
                    b.Height = p.Y;
            }
            b.Width -= b.X;
            b.Height -= b.Y;

            if (bInflate && !b.IsEmpty)
            {
                if (b.Width < 4)
                    b.Inflate(3, 0);
                if (b.Height < 4)
                    b.Inflate(0, 3);
            }

            return b;
        }

        bool bCaptured;
        Point ptLast;
        private void AdornedElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (bCaptured)
            {
                bCaptured = false;
                adornedElement.ReleaseMouseCapture();

                Point pt = GridViewInfoService.SnapPointToGridSize(Mouse.GetPosition(adornedElement));
                double h = pt.X - ptLast.X;
                double v = pt.Y - ptLast.Y;
                ptLast = pt;
                control_DragCompleted(dragControl, new DragCompletedEventArgs(h, v, false));
            }
        }

        private void AdornedElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (!bCaptured)
                return;

            Point pt = GridViewInfoService.SnapPointToGridSize(Mouse.GetPosition(adornedElement));
            double h = pt.X - ptLast.X;
            double v = pt.Y - ptLast.Y;
            ptLast = pt;
            pointDragDelta(dragControl, new DragDeltaEventArgs(h, v));
        }

        void AdornerPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = Mouse.GetPosition(adornedElement);

            if (!CanAddPoints || !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                var hitTestResult = VisualTreeHelper.HitTest(adornedElement, pt);
                if (hitTestResult != null && hitTestResult.VisualHit != null)
                {
                    var parent = hitTestResult.VisualHit as DependencyObject;
                    if (parent != null)
                    {
                        do
                        {
                            if (parent == Element)
                                break;
                            parent = VisualTreeHelper.GetParent(parent) as DependencyObject;
                        } while (parent != null && parent != Element);
                    }

                    if (parent == Element)
                    {
                        Mouse.Capture(adornedElement);
                        bCaptured = true;
                        e.Handled = true;
                        ptLast = GridViewInfoService.SnapPointToGridSize(pt);
                        pointDragStarted(dragControl, new DragStartedEventArgs(0, 0));
                        return;
                    }
                }
                return;
            }

            for (int i = 0; i < Points.Count - 1; i++)
            {
                // Hit test
                LineGeometry lg = new LineGeometry(Points[i], Points[i + 1]);
                EllipseGeometry eg = new EllipseGeometry(pt, 5, 5);
                IntersectionDetail id = eg.FillContainsWithDetail(lg);
                if (id == IntersectionDetail.Intersects)
                {
                    Points.Insert(i + 1, pt);
                    PointDragThumb pd = new PointDragThumb();
                    pd.DragStarted += pointDragStarted;
                    pd.DragDelta += pointDragDelta;
                    pd.DragCompleted += control_DragCompleted;
                    pd.PreviewMouseLeftButtonDown += AdornerPreviewMouseLeftButtonUp;

                    visualChildren.Insert(nNumberFixedAdorners + i + 1, pd);
                    adornedElement.UpdateLayout();
                    UpdatePoints();
                    e.Handled = true;

                    OnFireChanged(this);
                    break;
                }
            }

            InvalidateVisual();
            InvalidateArrange();
            FindParentCanvas(adornedElement).InvalidateArrange();

            AdornerLayer.GetAdornerLayer(AdornedElement).Update();
        }

        void AdornerPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                return;
            if (Points.Count <= 2)
                return;

            Point p = Points[visualChildren.IndexOf(sender as PointDragThumb) - nNumberFixedAdorners];

            (sender as PointDragThumb).DragStarted -= pointDragStarted;
            (sender as PointDragThumb).DragDelta -= pointDragDelta;
            (sender as PointDragThumb).DragCompleted -= control_DragCompleted;
            (sender as PointDragThumb).PreviewMouseLeftButtonDown -= AdornerPreviewMouseLeftButtonUp;

            visualChildren.Remove((Visual)sender);
            Points.Remove(p);
            adornedElement.UpdateLayout();
            UpdatePoints();

            InvalidateVisual();
            InvalidateArrange();
            FindParentCanvas(adornedElement).InvalidateArrange();

            AdornerLayer.GetAdornerLayer(AdornedElement).Update();
            e.Handled = true;

            OnFireChanged(this);
        }

        Visibility lastDragSelectedVisibility;
        bool bChanged;
        void pointDragDelta(object sender, DragDeltaEventArgs e)
        {
            currentThumb = sender as Thumb;
            bChanged = true;

            OnFireChanging(this);
            bEnableFireChanging = false;

            if (sender is PointDragThumb)
            {
                Point p = Points[visualChildren.IndexOf(sender as PointDragThumb) - nNumberFixedAdorners];
                p.X = GridViewInfoService.SnapPointToGridSizeHorizontal(e.HorizontalChange + p.X);
                p.Y = GridViewInfoService.SnapPointToGridSizeVertical(e.VerticalChange + p.Y);

                Points[visualChildren.IndexOf(sender as PointDragThumb) - nNumberFixedAdorners] = p;
            }
            else
            {
                Rect b = GetBoundingRect(false);
                double offsetX = GridViewInfoService.SnapPointToGridSizeHorizontal(e.HorizontalChange + b.Left) - b.Left;
                double offsetY = GridViewInfoService.SnapPointToGridSizeVertical(e.VerticalChange + b.Top) - b.Top;

                for (int i = 0; i < Points.Count; i++)
                {
                    Point p = Points[i];
                    p.X += offsetX;
                    p.Y += offsetY;
                    Points[i] = p;
                }

                GridViewInfoService.DragMultipleSelection(offsetX, offsetY, adornedElement);
            }

            adornedElement.UpdateLayout();
            UpdatePoints();
            // e.Handled = true;

            InvalidateArrange();
            FindParentCanvas(adornedElement).InvalidateArrange();
            AdornerLayer.GetAdornerLayer(AdornedElement).Update();
        }

        void pointDragStarted(object sender, DragStartedEventArgs e)
        {
            lastDragSelectedVisibility = dragControlSelected.Visibility;
            dragControlSelected.Visibility = Visibility.Hidden;
            if (editControl != null)
                editControl.Visibility = Visibility.Collapsed;

            currentThumb = sender as Thumb;
            bChanged = false;
            InvalidateArrange();
            FindParentCanvas(adornedElement).InvalidateArrange();

            AdornerLayer.GetAdornerLayer(AdornedElement).Update();
        }

        void control_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender != dragControlSelectedCenter)
            {
                dragControlSelected.Visibility = lastDragSelectedVisibility;
                //editControl.Visibility = Visibility.Visible;
            }

            currentThumb = null;

            InvalidateArrange();
            if (bChanged)
                OnFireChanged(this);
            bEnableFireChanging = true;
            FindParentCanvas(adornedElement).InvalidateArrange();

            AdornerLayer.GetAdornerLayer(AdornedElement).Update();
        }

        #endregion

        #region Virtuals

        protected virtual void UpdatePoints()
        {
        }

        Transform previousTransform;
        public override void Activate()
        {
            previousTransform = adornedElement.RenderTransform;
            adornedElement.RenderTransform = null;

            foreach (Point p in Points)
            {
                PointDragThumb pd = new PointDragThumb();
                pd.DragStarted += pointDragStarted;
                pd.DragDelta += pointDragDelta;
                pd.DragCompleted += control_DragCompleted;
                if (CanAddPoints)
                    pd.PreviewMouseLeftButtonDown += AdornerPreviewMouseLeftButtonUp;
                visualChildren.Add(pd);

                if (!bCanMove)
                    pd.IsEnabled = false;
            }
            adornedElement.UpdateLayout();

            dragControl.DragDelta += pointDragDelta;
            dragControl.DragCompleted += control_DragCompleted;

            // if (CanAddPoints)
            {
                adornedElement.PreviewMouseLeftButtonDown += AdornerPreviewMouseLeftButtonDown;
                adornedElement.MouseMove += AdornedElement_MouseMove;
                adornedElement.MouseUp += AdornedElement_MouseUp;
            }

            for (int i = 0; i < Points.Count; i++)
            {
                Matrix m = Element.GeometryTransform.Value;

                Point p = m.Transform(Points[i]);
                p = adornedElement.TranslatePoint(p, (UIElement)Element.Parent);
                Points[i] = p;
            }
            // poly.Stretch = Stretch.None;
            adornedElement.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            adornedElement.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            adornedElement.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            adornedElement.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            adornedElement.SetValue(FrameworkElement.WidthProperty, DependencyProperty.UnsetValue);
            adornedElement.SetValue(FrameworkElement.HeightProperty, DependencyProperty.UnsetValue);

            adornedElement.UpdateLayout();
            UpdatePoints();

            if (dragControlSelectedCenter == null)
            {
                dragControlSelectedCenter = new DragThumbSelectedCenter();
                dragControlSelectedCenter.Visibility = Visibility.Visible;
                dragControlSelectedCenter.VerticalAlignment = VerticalAlignment.Top;
                dragControlSelectedCenter.HorizontalAlignment = HorizontalAlignment.Left;
                dragControlSelectedCenter.Cursor = Cursors.Cross;
            }

            visualChildren.Add(dragControlSelectedCenter);
            dragControlSelectedCenter.DragDelta += pointDragDelta;
            dragControlSelectedCenter.DragCompleted += control_DragCompleted;

            base.Activate();

            if (adornedElement.ReadLocalValue(RenderTransformOriginProperty) == DependencyProperty.UnsetValue)
                adornedElement.RenderTransformOrigin = new Point(0.5, 0.5);

            visualChildren.Insert(2, TransformOrigin);
            TransformOrigin.DragStarted += pointDragStartedTransformOrigin;
            TransformOrigin.DragDelta += pointDragDeltaTransformOrigin;
            TransformOrigin.DragCompleted += pointDragCompletedTransformOrigin;

            dragControlSelected.Visibility = Visibility.Hidden;
            dragControl.Visibility = Visibility.Collapsed;
        }

        internal override void ShowHideRotationThumbs()
        {
            base.ShowHideRotationThumbs();
            if (dragControlSelectedCenter != null)
            {
                dragControlSelectedCenter.Visibility = dragControlSelectedCenter.Visibility == System.Windows.Visibility.Visible ?
                        System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }
        }

        bool bDraggableVisible;
        public override void SetDraggable(bool bSet, bool bInside = false, bool bForce = false)
        {
            if (bInside || !bCanMove)
                return;

            if (!bForce)
            {
                if (bDraggableVisible)
                {
                    if (dragControlSelectedCenter != null)
                        dragControlSelectedCenter.Visibility = Visibility.Visible;

                    bDraggableVisible = false;
                    dragControlSelected.Visibility = Visibility.Hidden;
                    dragControl.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (dragControlSelectedCenter != null)
                        dragControlSelectedCenter.Visibility = Visibility.Collapsed;

                    dragControlSelected.Visibility = Visibility.Visible;
                    dragControl.Visibility = Visibility.Visible;
                    bDraggableVisible = true;
                }
            }
            else
            {
                if (bSet)
                {
                    if (dragControlSelectedCenter != null)
                        dragControlSelectedCenter.Visibility = Visibility.Collapsed;

                    dragControlSelected.Visibility = Visibility.Visible;
                    dragControl.Visibility = Visibility.Visible;
                    bDraggableVisible = true;
                }
                else
                {
                    if (dragControlSelectedCenter != null)
                        dragControlSelectedCenter.Visibility = Visibility.Visible;

                    bDraggableVisible = false;
                    dragControlSelected.Visibility = Visibility.Hidden;
                    dragControl.Visibility = Visibility.Collapsed;
                }
            }
        }

        public override void SetIsActive(bool bSet, bool isMultiselection = false)
        {
        }

        /*
        public override void RestorePosition(bool bSet)
        {
            if (bSet)
            {
                Rect b = GetBoundingRect();

                adornedElement.SetValue(Canvas.LeftProperty, b.X);
                adornedElement.SetValue(Canvas.TopProperty, b.Y);
                adornedElement.SetValue(Canvas.LeftProperty, b.X);
                adornedElement.SetValue(Canvas.TopProperty, b.Y);
            }
            else
            {
                adornedElement.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                adornedElement.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                adornedElement.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                adornedElement.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                adornedElement.SetValue(FrameworkElement.WidthProperty, DependencyProperty.UnsetValue);
                adornedElement.SetValue(FrameworkElement.HeightProperty, DependencyProperty.UnsetValue);
            }
        }
        */

        public override void Deactivate()
        {
            Rect b = GetBoundingRect();

            adornedElement.SetValue(Canvas.LeftProperty, b.X);
            adornedElement.SetValue(Canvas.TopProperty, b.Y);
            adornedElement.SetValue(Canvas.LeftProperty, b.X);
            adornedElement.SetValue(Canvas.TopProperty, b.Y);
            //poly.SetValue(FrameworkElement.WidthProperty, b.Width);
            //poly.SetValue(FrameworkElement.HeightProperty, b.Height);
            for (int i = 0; i < Points.Count; i++)
            {
                Point p = Points[i];
                p.X -= b.X;
                p.Y -= b.Y;
                Points[i] = p;
            }
            foreach (var v in visualChildren)
            {
                PointDragThumb pdt = v as PointDragThumb;
                if (pdt == null)
                    continue;

                pdt.DragStarted -= pointDragStarted;
                pdt.DragDelta -= pointDragDelta;
                pdt.DragCompleted -= control_DragCompleted;
                if (CanAddPoints)
                    pdt.PreviewMouseLeftButtonDown -= AdornerPreviewMouseLeftButtonUp;
            }
            UpdatePoints();

            dragControl.DragDelta -= pointDragDelta;
            dragControl.DragCompleted -= control_DragCompleted;

            dragControlSelectedCenter.DragDelta -= pointDragDelta;
            dragControlSelectedCenter.DragCompleted -= control_DragCompleted;

            // if (CanAddPoints)
            {
                adornedElement.PreviewMouseLeftButtonDown -= AdornerPreviewMouseLeftButtonDown;
                adornedElement.MouseMove -= AdornedElement_MouseMove;
                adornedElement.MouseUp -= AdornedElement_MouseUp;
            }

            adornedElement.RenderTransform = previousTransform;
            base.Deactivate();

            TransformOrigin.DragStarted -= pointDragStartedTransformOrigin;
            TransformOrigin.DragDelta -= pointDragDeltaTransformOrigin;
            TransformOrigin.DragCompleted -= pointDragCompletedTransformOrigin;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var v in visualChildren)
            {
                if (v == editControl)
                {
                    base.ArrangeControl(v);
                    continue;
                }
                else if (v == dragControlSelectedCenter)
                {
                    Rect aligmentRect = GetBoundingRect();
                    var t = v as Thumb;
                    var centerPoint = new Point(aligmentRect.Left + aligmentRect.Width / 2,
                                                aligmentRect.Top + aligmentRect.Height / 2);
                    var rect = new Rect(centerPoint.X - 10, centerPoint.Y - 10, 20, 20);
                    t.Arrange(rect);
                    continue;
                }

                PointDragThumb pdt = v as PointDragThumb;
                if (pdt == null)
                {
                    if (v is TransformOriginThumb)
                    {
                        Rect aligmentRect = GetBoundingRect();
                        TransformOriginThumb t = v as TransformOriginThumb;
                        Point ptOrigin = adornedElement.RenderTransformOrigin;

                        aligmentRect.Y = (aligmentRect.Height * (ptOrigin.Y - 0.5)) + aligmentRect.Top;
                        aligmentRect.X = (aligmentRect.Width * (ptOrigin.X - 0.5)) + aligmentRect.Left;
                        t.Arrange(aligmentRect);
                    }
                    else // if (v is Thumb)
                    {
                        // Rect aligmentRect = VisualTreeHelper.GetContentBounds(adornedElement);
                        Rect aligmentRect = GetBoundingRect();
                        var t = v as FrameworkElement;

                        if (FindParentCanvas(adornedElement).Children.Contains(adornedElement))
                        {
                            aligmentRect.X = aligmentRect.Left;
                            aligmentRect.Y = aligmentRect.Top;

                            Point pt = adornedElement.TransformToVisual(adornedElement).Transform(new Point(aligmentRect.X, aligmentRect.Y));
                            if (t.RenderTransform != null)
                            {
                                pt.X -= t.RenderTransform.Value.OffsetX;
                                pt.Y -= t.RenderTransform.Value.OffsetY;
                            }

                            aligmentRect.X = pt.X - (double.IsNaN(t.Width) ? 0 : t.Width) / 2;
                            aligmentRect.Y = pt.Y - (double.IsNaN(t.Height) ? 0 : t.Height) / 2;
                        }
                        else
                        {
                            Point po = adornedElement.TransformToVisual(adornedElement).Transform(aligmentRect.TopLeft);
                            aligmentRect.X = po.X;
                            aligmentRect.Y = po.Y;
                        }

                        t.Arrange(aligmentRect);
                    }
                    //else
                    //    ArrangeControl(v);
                    continue;
                }

                Point p = Points[visualChildren.IndexOf(pdt) - nNumberFixedAdorners];

                if (FindParentCanvas(adornedElement).Children.Contains(adornedElement))
                {
                    p.X -= pdt.DesiredSize.Width / 2;
                    p.Y -= pdt.DesiredSize.Height / 2;
                    pdt.Arrange(new Rect(p, pdt.DesiredSize));
                }
                else
                {
                    Point po = adornedElement.TransformToVisual(FindParentCanvas(adornedElement)).Transform(p);
                    po.X -= pdt.DesiredSize.Width / 2;
                    po.Y -= pdt.DesiredSize.Height / 2;
                    pdt.Arrange(new Rect(po, pdt.DesiredSize));
                }
            }

            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            Matrix m = new Matrix
            {
                OffsetX = ((MatrixTransform)transform).Matrix.OffsetX,
                OffsetY = ((MatrixTransform)transform).Matrix.OffsetY
            };

            return transform;//new MatrixTransform(m); //this code neded for right manipulators zooming
        }

        Pen pen = new Pen(Brushes.Blue, 0.2d);
        Pen penSnapped = new Pen(Brushes.Red, 1d);
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (currentThumb == null || currentThumb is RotateThumb)
                return;

            Rect b = GetBoundingRect();

            double top = b.Top;
            double left = b.Left;

            if (currentThumb is PointDragThumb)
            {
                if (visualChildren.Contains(currentThumb))
                {
                    Point p = Points[visualChildren.IndexOf(currentThumb as PointDragThumb) - nNumberFixedAdorners];

                    dc.DrawLine(GridViewInfoService.IsPointOnSnapLineVertical(p.Y) ? penSnapped : pen,
                        new Point(-100000, p.Y), new Point(100000, p.Y));
                    FormattedText ftextTop = new FormattedText(
                        (p.Y).ToString("F"),
                        CultureInfo.CurrentUICulture,
                        System.Windows.FlowDirection.LeftToRight,
                        new Typeface("TimesNewRoman"),
                        8, Brushes.Blue);
                    dc.DrawText(ftextTop, new Point(p.X + 4, p.Y - (ftextTop.Height + 4)));

                    dc.DrawLine(GridViewInfoService.IsPointOnSnapLineHorizontal(p.X) ? penSnapped : pen,
                        new Point(p.X, -100000), new Point(p.X, 100000));
                    FormattedText ftextLeft = new FormattedText(
                        (p.X).ToString("F"),
                        CultureInfo.CurrentUICulture,
                        System.Windows.FlowDirection.LeftToRight,
                        new Typeface("TimesNewRoman"),
                        8, Brushes.Blue);
                    dc.DrawText(ftextLeft, new Point(p.X - (ftextLeft.Width + 4), p.Y + 4));
                }
            }
            else
            {
                switch (currentThumb.VerticalAlignment)
                {
                    // case VerticalAlignment.Center:
                    case VerticalAlignment.Stretch:
                    case VerticalAlignment.Top:
                        dc.DrawLine(GridViewInfoService.IsPointOnSnapLineVertical(top) ? penSnapped : pen,
                            new Point(-100000, top), new Point(100000, top));
                        FormattedText ftextTop = new FormattedText(
                            top.ToString("F"),
                            CultureInfo.CurrentUICulture,
                            System.Windows.FlowDirection.LeftToRight,
                            new Typeface("TimesNewRoman"),
                            8, Brushes.Blue);
                        dc.DrawText(ftextTop, new Point(left + 4, top - (ftextTop.Height + 4)));
                        break;
                }
                switch (currentThumb.HorizontalAlignment)
                {
                    // case HorizontalAlignment.Center:
                    case HorizontalAlignment.Stretch:
                    case HorizontalAlignment.Left:
                        dc.DrawLine(GridViewInfoService.IsPointOnSnapLineHorizontal(left) ? penSnapped : pen,
                            new Point(left, -100000), new Point(left, 100000));
                        FormattedText ftextLeft = new FormattedText(
                            left.ToString("F"),
                            CultureInfo.CurrentUICulture,
                            System.Windows.FlowDirection.LeftToRight,
                            new Typeface("TimesNewRoman"),
                            8, Brushes.Blue);
                        dc.DrawText(ftextLeft, new Point(left - (ftextLeft.Width + 4), top + 4));
                        break;
                }
            }
        }

        private void pointDragStartedTransformOrigin(object sender, DragStartedEventArgs e)
        {
            // adornedElement.RenderTransform = new TransformGroup(); // reset the center
        }

        void pointDragDeltaTransformOrigin(object sender, DragDeltaEventArgs e)
        {
            OnFireChanging(this);
            bEnableFireChanging = false;

            Point currentPoint = Mouse.GetPosition(AdornedElement);
            Point ptOrigin = new Point();

            Rect aligmentRect = GetBoundingRect();

            double dWidth = aligmentRect.Width;
            double dHeight = aligmentRect.Height;
            currentPoint.X -= ElementLeft;
            currentPoint.Y -= ElementTop;

            if (dHeight == 0.0)
                ptOrigin.Y = 0;
            else
                ptOrigin.Y = currentPoint.Y / dHeight;

            if (dWidth == 0.0)
                ptOrigin.X = 0;
            else
                ptOrigin.X = currentPoint.X / dWidth;

            adornedElement.RenderTransformOrigin = ptOrigin;

            InvalidateArrange();
        }

        void pointDragCompletedTransformOrigin(object sender, DragCompletedEventArgs e)
        {
            OnFireChanged(this);
            bEnableFireChanging = true;
        }

        #endregion
    }
}
