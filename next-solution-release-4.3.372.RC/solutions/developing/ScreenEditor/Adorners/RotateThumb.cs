using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.WPF;


namespace ScreenManager.Adorners
{
    /// <summary>
    /// Rotating controll for DragResizeRotateManipulator
    /// </summary>
    class RotateThumb : Thumb
    {
        IGridViewInfoService GridViewInfoService;

        double initialAngle;
        private Vector startVector;
        private Point centerPoint;

        private FrameworkElement designerItem;
        private FrameworkElement DesignerItem
        {
            get
            {
                designerItem = this.DataContext as FrameworkElement;;
                return designerItem;
            }
        }

        private RotateTransform ItemRotateTransform
        {
            get
            {
                if (DesignerItem.RenderTransform == null)
                {
                    TransformGroup tg = new TransformGroup();
                    tg.Children.Add(new RotateTransform());
                    DesignerItem.RenderTransform = tg;
                    DesignerItem.RenderTransformOrigin = new Point(0.5, 0.5);
                }
                else if (DesignerItem.RenderTransform is TransformGroup)
                {
                    TransformGroup tg = DesignerItem.RenderTransform as TransformGroup;
                    var vars = from transform in tg.Children
                               where transform is RotateTransform
                               select transform;
                    if (vars.Count() == 0)
                        tg.Children.Add(new RotateTransform());
                }
                else
                {
                    TransformGroup tg = new TransformGroup();
                    tg.Children.Add(DesignerItem.RenderTransform);
                    tg.Children.Add(new RotateTransform());
                    DesignerItem.RenderTransform = tg;
                }

                TransformGroup t = DesignerItem.RenderTransform as TransformGroup;
                var rotate = from transform in t.Children
                           where transform is RotateTransform
                           select transform;
                if (rotate.Count() == 0)
                    return null;

                return rotate.First() as RotateTransform;
            }
        }

        public RotateThumb(IGridViewInfoService gridViewInfoService)
        {
            GridViewInfoService = gridViewInfoService;

            base.DragDelta += RotateThumb_DragDelta;
            base.DragStarted += RotateThumb_DragStarted;
            base.DragCompleted += RotateThumb_DragCompleted;
            Width = 50;
            Height = 50;

        }
        
        void RotateThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            HideTooltip();
        }

        private static InkCanvas FindParentInkCanvas(DependencyObject dObj)
        {
            var ret = dObj.FindParent<InkCanvas>();
            if (ret != null)
                return ret;
            return dObj.FindParent<InkCanvas>();
        }

        void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            UIElement parent = FindParentInkCanvas(DesignerItem);
            if (parent == null)
                parent = VisualTreeHelper.GetParent(DesignerItem) as UIElement;

            if (DesignerItem != null && parent != null)
            {
                // the RenderTransformOrigin property of DesignerItem defines
                // transformation center relative to its bounds

                double dWidth = DesignerItem.DesiredSize.Width;
                double dHeight = DesignerItem.DesiredSize.Height;
                if (dWidth == 0.0)
                {
                    FrameworkElement element = DesignerItem as FrameworkElement;
                    if (element != null)
                        dWidth = element.ActualWidth;
                }
                if (dHeight == 0.0)
                {
                    FrameworkElement element = DesignerItem as FrameworkElement;
                    if (element != null)
                        dHeight = element.ActualHeight;
                }

                centerPoint = DesignerItem.TranslatePoint(
                    new Point(dWidth * DesignerItem.RenderTransformOrigin.X,
                              dHeight * DesignerItem.RenderTransformOrigin.Y),
                               parent);
            
            
                // calculate startVector, that is the vector from centerPoint to startPoint
                Point startPoint = Mouse.GetPosition(parent);
                startVector = Point.Subtract(startPoint, centerPoint);

                // check if the DesignerItem already has a RotateTransform set ...
                initialAngle = ItemRotateTransform.Angle;
                SetToolTip(ItemRotateTransform.Angle);              
            }
            // e.Handled = false;
        }

        void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            UIElement parent = VisualTreeHelper.GetParent(DesignerItem) as UIElement;

            if (DesignerItem != null && parent != null)
            {
                // calculate deltaVector, that is the vector from centerPoint to current mouse position                
                Point currentPoint = Mouse.GetPosition(parent);
                Vector deltaVector = Point.Subtract(currentPoint, centerPoint);

                //calculate the angle between startVector and dragVector
                double angle = Vector.AngleBetween(startVector, deltaVector);

                // and update the transformation
                ItemRotateTransform.Angle = initialAngle + Math.Round(angle, 0);
                SetToolTip(ItemRotateTransform.Angle);
           }
           // e.Handled = false;
        }

        ToolTip T;
        TextBlock textBlockTooltip;
        void SetToolTip(double angle)
        {
            if (textBlockTooltip == null)
            {
                textBlockTooltip = new TextBlock();
                T = new ToolTip();
                T.Content = textBlockTooltip;
                ToolTip = T;
                ToolTipService.SetShowDuration(this, 100000);
                T.IsOpen = true;
            }
            textBlockTooltip.Text = String.Format("{0}", angle);
        }

        void HideTooltip()
        {
            T.IsOpen = false;
            ToolTip = null;
            T.Content = null;
            T = null;
            textBlockTooltip = null;
        }
    }
}
