using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Utilities.WPF;
using System.Windows.Input;

namespace ScreenManager.Adorners
{
    /// <summary>
    /// resize controll dor DragResizeRotateManipulator
    /// </summary>
    /// 
    class ResizeThumb : Thumb
    {
        readonly UIElement AdornedElement;
        readonly IGridViewInfoService GridViewInfoService;

        private double angle;
        private Point transformOrigin;
        private FrameworkElement parentItem;
        private FrameworkElement ParentItem
        {
            get
            {
                if (parentItem == null)
                {
                    parentItem = this.DataContext as FrameworkElement;
                }
                return parentItem;
            }
        }

        public ResizeThumb(UIElement parent, IGridViewInfoService gridViewInfoService)
        {
            AdornedElement = parent;
            GridViewInfoService = gridViewInfoService;

            Width = 10;
            Height = 10;

            base.DragStarted += ResizeThumb_DragStarted;
            base.DragDelta += ResizeThumb_DragDelta;
        }

        Double dKeepAspectRatio;
        void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (ParentItem != null)
            {
                transformOrigin = ParentItem.RenderTransformOrigin;

                RotateTransform rotateTransform = null;
                TransformGroup t = ParentItem.RenderTransform as TransformGroup;
                if (t != null)
                {
                    var rotate = from transform in t.Children
                                 where transform is RotateTransform
                                 select transform;
                    if (rotate.Count() > 0)
                        rotateTransform = rotate.First() as RotateTransform;
                }

                if (rotateTransform != null)
                    angle = rotateTransform.Angle * Math.PI / 180.0;   //convert degrees to radians
                else
                    angle = 0.0d;

                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    try
                    {
                        dKeepAspectRatio = ParentItem.ActualWidth / ParentItem.ActualHeight;
                    }
                    catch
                    {
                        dKeepAspectRatio = Double.NaN;
                    }
                }
                else
                    dKeepAspectRatio = Double.NaN;
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

        void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ParentItem != null)
            {
                Canvas canvas = VisualTreeHelper.GetParent(ParentItem) as Canvas;
                Canvas inkcanvas = FindParentInkCanvas(ParentItem);

                double deltaVertical=0, deltaHorizontal=0;


                // Point dragDelta = ParentItem.RenderTransform.Inverse.Transform(new Point(e.HorizontalChange, e.VerticalChange));
                Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);
                double w, h;
                double top = 0, left = 0;
                w = double.IsNaN(ParentItem.Width) ? ParentItem.ActualWidth : ParentItem.Width;
                h = double.IsNaN(ParentItem.Height) ? ParentItem.ActualHeight : ParentItem.Height;

                //if (inkcanvas != null)
                //{
                    top = double.IsNaN(Canvas.GetTop(ParentItem)) ? 0 : Canvas.GetTop(ParentItem);
                    left = double.IsNaN(Canvas.GetLeft(ParentItem)) ? 0 : Canvas.GetLeft(ParentItem);
                //}
                //else
                //{
                //    top = double.IsNaN(Canvas.GetTop(ParentItem)) ? 0 : Canvas.GetTop(ParentItem);
                //    left = double.IsNaN(Canvas.GetLeft(ParentItem)) ? 0 : Canvas.GetLeft(ParentItem);
                //}

                bool bAdjustPoint = false;
                switch (base.VerticalAlignment)
                {
                    case System.Windows.VerticalAlignment.Bottom:
                        bAdjustPoint = true;
                        deltaVertical = Math.Min(-dragDelta.Y, ParentItem.ActualHeight - Height);
                        ParentItem.Height = Math.Max(GridViewInfoService.SnapPointToGridSizeVertical(h - deltaVertical + top) - top, 0);
                        if (!Double.IsNaN(dKeepAspectRatio))
                            ParentItem.Width = ParentItem.Height * dKeepAspectRatio;
                        break;
                    case System.Windows.VerticalAlignment.Top:
                        deltaVertical = Math.Min(dragDelta.Y, ParentItem.ActualHeight - Height);
                        Point p = new Point(0, deltaVertical);
                        //if (ParentItem.RenderTransform != null)
                        //    p = ParentItem.RenderTransform.Transform(p);

                        //Point sizeDelta = new Point(deltaHorizontal, deltaVertical);
                        //Point sizeDeltaTrans = sizeDelta;
                        //if (ParentItem.RenderTransform != null)
                        //    sizeDeltaTrans = ParentItem.RenderTransform.Transform(sizeDelta);
                        //Vector v = sizeDelta - sizeDeltaTrans;

                        double snapped = GridViewInfoService.SnapPointToGridSizeVertical(top + p.Y);
                        //if (inkcanvas != null)
                        //{
                            Canvas.SetTop(ParentItem, snapped);
                            // InkCanvas.SetLeft(ParentItem, GridViewInfoService.SnapPointToGridSizeHorizontal(left + p.X));
                            // ParentItem.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                            // ParentItem.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                            //if (ParentItem.RenderTransform != null)
                            //{
                            //    left = double.IsNaN(InkCanvas.GetLeft(ParentItem)) ? 0 : InkCanvas.GetLeft(ParentItem);
                            //    InkCanvas.SetLeft(ParentItem, left - v.X * parentItem.RenderTransformOrigin.X);
                            //    ParentItem.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                            //}
                        //}
                        //else
                        //{
                        //    Canvas.SetTop(ParentItem, snapped);
                        //    // Canvas.SetLeft(ParentItem, GridViewInfoService.SnapPointToGridSizeHorizontal(left + p.X));
                        //    ParentItem.SetValue(InkCanvas.TopProperty, DependencyProperty.UnsetValue);
                        //    // ParentItem.SetValue(InkCanvas.LeftProperty, DependencyProperty.UnsetValue);
                        //    //if (ParentItem.RenderTransform != null)
                        //    //{
                        //    //    left = double.IsNaN(Canvas.GetLeft(ParentItem)) ? 0 : Canvas.GetLeft(ParentItem);
                        //    //    Canvas.SetLeft(ParentItem, left - v.X * parentItem.RenderTransformOrigin.X);
                        //    //    ParentItem.SetValue(InkCanvas.LeftProperty, DependencyProperty.UnsetValue);
                        //    //}
                        //}

                        ParentItem.Height = Math.Max(h + top - snapped, 0);
                        if (!Double.IsNaN(dKeepAspectRatio))
                            ParentItem.Width = ParentItem.Height * dKeepAspectRatio;
                        break;
                    default:
                        break;
                }

                switch (base.HorizontalAlignment)
                {
                    case System.Windows.HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(dragDelta.X, ParentItem.ActualWidth - Width);
                        Point p = new Point(deltaHorizontal, 0);
                        //if (ParentItem.RenderTransform != null)
                        //    p = ParentItem.RenderTransform.Transform(p);

                        //Point sizeDelta = new Point(deltaHorizontal, deltaVertical);
                        //Point sizeDeltaTrans = sizeDelta;
                        //if (ParentItem.RenderTransform != null)
                        //    sizeDeltaTrans = ParentItem.RenderTransform.Transform(sizeDelta);
                        //Vector v = sizeDelta - sizeDeltaTrans;
                        double snapped = GridViewInfoService.SnapPointToGridSizeHorizontal(left + p.X);
                        //if (inkcanvas != null)
                        //{
                            // InkCanvas.SetTop(ParentItem, GridViewInfoService.SnapPointToGridSizeVertical(top + p.Y));
                            Canvas.SetLeft(ParentItem, snapped);
                            // ParentItem.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                            // ParentItem.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                            //if (ParentItem.RenderTransform != null)
                            //{
                            //    top = double.IsNaN(InkCanvas.GetTop(ParentItem)) ? 0 : InkCanvas.GetTop(ParentItem);
                            //    InkCanvas.SetTop(ParentItem, top - v.Y * parentItem.RenderTransformOrigin.Y);
                            //    ParentItem.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                            //}
                        //}
                        //else
                        //{
                        //    // Canvas.SetTop(ParentItem, GridViewInfoService.SnapPointToGridSizeVertical(top + p.Y));
                        //    Canvas.SetLeft(ParentItem, snapped);
                        //    // ParentItem.SetValue(InkCanvas.TopProperty, DependencyProperty.UnsetValue);
                        //    ParentItem.SetValue(InkCanvas.LeftProperty, DependencyProperty.UnsetValue);
                        //    //if (ParentItem.RenderTransform != null)
                        //    //{
                        //    //    top = double.IsNaN(Canvas.GetTop(ParentItem)) ? 0 : Canvas.GetTop(ParentItem);
                        //    //    Canvas.SetTop(ParentItem, top - v.Y * parentItem.RenderTransformOrigin.Y);
                        //    //    ParentItem.SetValue(InkCanvas.TopProperty, DependencyProperty.UnsetValue);
                        //    //}
                        //}

                        ParentItem.Width = Math.Max(w + left - snapped, 0);
                        if (!Double.IsNaN(dKeepAspectRatio))
                            ParentItem.Height = ParentItem.Width / dKeepAspectRatio;
                        break;
                    case System.Windows.HorizontalAlignment.Right:
                        bAdjustPoint = true;
                        deltaHorizontal = Math.Min(-dragDelta.X, ParentItem.ActualWidth - Width);
                        ParentItem.Width = Math.Max(GridViewInfoService.SnapPointToGridSizeHorizontal(w - deltaHorizontal + left) - left, 0);
                        if (!Double.IsNaN(dKeepAspectRatio))
                            ParentItem.Height = ParentItem.Width / dKeepAspectRatio;
                        break;
                    default:
                        break;
                }

                GridViewInfoService.GridServiceObjectResized();

                //if (bAdjustPoint)
                //{
                //    Point sizeDelta = new Point(deltaHorizontal, deltaVertical);
                //    Point sizeDeltaTrans = sizeDelta;
                //    if (ParentItem.RenderTransform != null)
                //        sizeDeltaTrans = ParentItem.RenderTransform.Transform(sizeDelta);
                //    Vector v = sizeDelta - sizeDeltaTrans;

                //    if (inkcanvas != null)
                //    {
                //        top = double.IsNaN(InkCanvas.GetTop(ParentItem)) ? 0 : InkCanvas.GetTop(ParentItem);
                //        left = double.IsNaN(InkCanvas.GetLeft(ParentItem)) ? 0 : InkCanvas.GetLeft(ParentItem);
                //        InkCanvas.SetTop(ParentItem, top + v.Y * parentItem.RenderTransformOrigin.Y);
                //        InkCanvas.SetLeft(ParentItem, left + v.X * parentItem.RenderTransformOrigin.X);
                //        ParentItem.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                //        ParentItem.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                //    }
                //    else
                //    {
                //        top = double.IsNaN(Canvas.GetTop(ParentItem)) ? 0 : Canvas.GetTop(ParentItem);
                //        left = double.IsNaN(Canvas.GetLeft(ParentItem)) ? 0 : Canvas.GetLeft(ParentItem);
                //        Canvas.SetTop(ParentItem, top + v.Y * parentItem.RenderTransformOrigin.Y);
                //        Canvas.SetLeft(ParentItem, left + v.X * parentItem.RenderTransformOrigin.X);
                //        ParentItem.SetValue(InkCanvas.TopProperty, DependencyProperty.UnsetValue);
                //        ParentItem.SetValue(InkCanvas.LeftProperty, DependencyProperty.UnsetValue);
                //    }
                //}
            }
            // e.Handled = true;
        }
    }
}
