using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using System;
using System.Windows.Media.Media3D;
using Utilities.WPF;
using Utilities;

namespace ScreenManager.Adorners
{
    /// <summary>
    /// Drag controll for DragResizeRotateManipulator
    /// </summary>
    class DragThumbSelected : Thumb
    {
    }

    class DragThumbSelectedCenter : Thumb
    {
    }

    class DragThumbMultiSelected : Thumb
    {
    }

    class DragThumb : Thumb
    {
        readonly UIElement AdornedElement;
        readonly IGridViewInfoService GridViewInfoService;
        bool bDragging;
        bool bIsPolygon;

        public DragThumb(UIElement parent, IGridViewInfoService gridViewInfoService, bool bPolygon = false)
        {
            AdornedElement = parent;
            GridViewInfoService = gridViewInfoService;
            bIsPolygon = bPolygon;
        }

        internal void Activate()
        {
            if (!bIsPolygon)
            {
                AdornedElement.Dispatcher.BeginInvokeAsynchronously(() =>
                {
                    base.DragDelta += DragThumb_DragDelta;
                    base.DragStarted += ResizeThumb_DragStarted;
                });
            }
        }

        internal void Deactivate()
        {
            if (!bIsPolygon)
            {
                base.DragDelta -= DragThumb_DragDelta;
                base.DragStarted -= ResizeThumb_DragStarted;
            }
        }

        private Canvas FindParentInkCanvas(DependencyObject dObj)
        {
            if (AdornedElement is Canvas)
                return AdornedElement as Canvas;

            var ret = dObj.FindParent<Canvas>();
            if (ret != null)
            {
                var first = ret.FindParent<Canvas>();
                while (first != null)
                {
                    ret = first;
                    first = first.FindParent<Canvas>();
                }
                return ret;
            }
            return dObj.FindParent<Canvas>();
        }

        private Point mLastPos;
        void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            FrameworkElement item = DataContext as FrameworkElement;
            if (item == null)
                return;
            if (!GridViewInfoService.CanDragStart(item, e))
            {
                bDragging = false;
                return;
            }

            bDragging = true;
            Point pos = Mouse.GetPosition(item);
            mLastPos = new Point(pos.X - item.ActualWidth / 2, item.ActualHeight / 2 - pos.Y);
        }

        internal void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement item = DataContext as FrameworkElement;
            if (item == null || !bDragging)
                return;

            if (((Keyboard.Modifiers & ModifierKeys.Shift) > 0) && item is Viewport3D)
            {
                Viewport3D v3D = item as Viewport3D;

                Point pos = Mouse.GetPosition(item);
                Point actualPos = new Point(
                        pos.X - item.ActualWidth / 2,
                        item.ActualHeight / 2 - pos.Y);
                double dx = actualPos.X - mLastPos.X;
                double dy = actualPos.Y - mLastPos.Y;
                double mouseAngle = 0;

                if (dx != 0 && dy != 0)
                {
                    mouseAngle = Math.Asin(Math.Abs(dy) /
                        Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    if (dx < 0 && dy > 0) mouseAngle += Math.PI / 2;
                    else if (dx < 0 && dy < 0) mouseAngle += Math.PI;
                    else if (dx > 0 && dy < 0) mouseAngle += Math.PI * 1.5;
                }
                else if (dx == 0 && dy != 0)
                {
                    mouseAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
                }
                else if (dx != 0 && dy == 0)
                {
                    mouseAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;
                }

                double axisAngle = mouseAngle + Math.PI / 2;

                Vector3D axis = new Vector3D(
                        Math.Cos(axisAngle) * 4,
                        Math.Sin(axisAngle) * 4, 0);

                double rotation = 0.02 *
                        Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                foreach (Visual3D visual3D in v3D.Children)
                {
                    QuaternionRotation3D r =
                         new QuaternionRotation3D(
                         new Quaternion(axis, rotation * 180 / Math.PI));

                    Transform3DGroup group = visual3D.Transform as Transform3DGroup;
                    if (group == null)
                    {
                        group = new Transform3DGroup();
                        visual3D.Transform = group;
                        group.Children.Add(new RotateTransform3D(r));
                    }
                    else
                    {
                        group.Children.Add(new RotateTransform3D(r));
                        Matrix3D groupResult = group.Value;
                        //clear previous
                        group.Children.Clear();
                        //now add the sum result
                        group.Children.Add(new MatrixTransform3D(groupResult));
                    }
                }

                mLastPos = actualPos;
            }
            else
            {
                Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

                //if (item.RenderTransform != null)
                //    dragDelta = item.RenderTransform.Transform(dragDelta);

                Canvas canvas = VisualTreeHelper.GetParent(item) as Canvas;
                Canvas inkcanvas = FindParentInkCanvas(item);

                //if (inkcanvas != null)
                //{
                    double left = Canvas.GetLeft(item);
                    double top = Canvas.GetTop(item);

                    left = double.IsNaN(left) ? 0 : left;
                    top = double.IsNaN(top) ? 0 : top;

                    double x = GridViewInfoService.SnapPointToGridSizeHorizontal(left + dragDelta.X);
                    Canvas.SetLeft(item, x);
                    double y = GridViewInfoService.SnapPointToGridSizeVertical(top + dragDelta.Y);
                    Canvas.SetTop(item, y);
                    //item.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //item.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);

                    GridViewInfoService.DragMultipleSelection(x - left, y - top, item);
                //}
                //else if (canvas != null)
                //{
                //    double left = Canvas.GetLeft(item);
                //    double top = Canvas.GetTop(item);

                //    left = double.IsNaN(left) ? 0 : left;
                //    top = double.IsNaN(top) ? 0 : top;

                //    double x = GridViewInfoService.SnapPointToGridSizeHorizontal(left + dragDelta.X);
                //    Canvas.SetLeft(item, x);
                //    double y = GridViewInfoService.SnapPointToGridSizeVertical(top + dragDelta.Y);
                //    Canvas.SetTop(item, y);
                //    item.SetValue(InkCanvas.TopProperty, DependencyProperty.UnsetValue);
                //    item.SetValue(InkCanvas.LeftProperty, DependencyProperty.UnsetValue);

                //    GridViewInfoService.DragMultipleSelection(x - left, y - top, item);
                //}
            }
            // e.Handled = false;
        }
    }
}
