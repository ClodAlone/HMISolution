using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Utilities.WPF;

namespace ScreenManager.Adorners
{
    /// <summary>
    /// An Adorner which displays animated text message. 
    /// </summary>
    public class WarningAdorner : Adorner
    {
        #region Construtor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adornedElement"></param>
        public WarningAdorner(UIElement adornedElement)
            : base(adornedElement) 
        {
            visualChildren = new VisualCollection(this);
            control = new WarningAdornerControl();
            visualChildren.Add(control);

            control.HorizontalAlignment = HorizontalAlignment.Stretch;
            control.VerticalAlignment = VerticalAlignment.Stretch;

            Visibility = Visibility.Visible;
            InvalidateMeasure();
            InvalidateArrange();

            var ret = FindParentInkCanvas(AdornedElement);
            if (ret != null)
                ret.InvalidateArrange();
        }

        #endregion Construtor

        protected static Canvas FindParentInkCanvas(DependencyObject dObj)
        {
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
            return ret = dObj.FindParent<Canvas>();
        }

        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

        protected double ElementWidth
        {
            get
            {
                double dWidth = AdornedElement.DesiredSize.Width;
                if (dWidth == 0.0)
                {
                    FrameworkElement element = AdornedElement as FrameworkElement;
                    if (element != null)
                        dWidth = element.ActualWidth;
                }

                return dWidth;
            }
        }

        protected double ElementHeight
        {
            get
            {
                double dHeight = AdornedElement.DesiredSize.Height;
                if (dHeight == 0.0)
                {
                    FrameworkElement element = AdornedElement as FrameworkElement;
                    if (element != null)
                        dHeight = element.ActualHeight;
                }

                return dHeight;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var c in visualChildren)
            {
                if (c is FrameworkElement)
                    ArrangeControl(c);
            }
            return finalSize;
        }

        protected void ArrangeControl(Visual c)
        {
            if (c is FrameworkElement)
            {
                Rect ro = new Rect(0, 0, ElementWidth, ElementHeight);

                FrameworkElement control = c as FrameworkElement;
                Rect aligmentRect = new Rect
                {
                    Width = control.Width,
                    Height = control.Height,
                    Y = ro.Y,
                    X = ro.X
                };

                if (control is TransformOriginThumb)
                {
                    Point ptOrigin = AdornedElement.RenderTransformOrigin;

                    aligmentRect.Y = ElementHeight * ptOrigin.Y;
                    aligmentRect.X = ElementWidth * ptOrigin.X;
                }
                else
                {
                    switch (control.VerticalAlignment)
                    {
                        case VerticalAlignment.Top: aligmentRect.Y += 0;
                            break;
                        case VerticalAlignment.Bottom: aligmentRect.Y += ro.Height;
                            break;
                        case VerticalAlignment.Center: aligmentRect.Y += ro.Height / 2;
                            break;
                        case VerticalAlignment.Stretch: aligmentRect.Height = Math.Max(control.MinHeight, ro.Height);// *Math.Abs((AdornedElement.RenderTransform as TransformGroup).Children[0].Value.M22);
                            break;
                    }
                    switch (control.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left: aligmentRect.X += 0;
                            break;
                        case HorizontalAlignment.Right: aligmentRect.X += ro.Width;
                            break;
                        case HorizontalAlignment.Center: aligmentRect.X += ro.Width / 2;
                            break;
                        case HorizontalAlignment.Stretch: aligmentRect.Width = Math.Max(control.MinWidth, ro.Width);// *Math.Abs((AdornedElement.RenderTransform as TransformGroup).Children[0].Value.M11);
                            break;
                    }
                }

                Point p = AdornedElement.TransformToVisual(AdornedElement).Transform(new Point(aligmentRect.X, aligmentRect.Y));
                if (control.RenderTransform != null)
                {
                    p.X -= control.RenderTransform.Value.OffsetX;
                    p.Y -= control.RenderTransform.Value.OffsetY;
                }

                aligmentRect.X = p.X - (double.IsNaN(control.Width) ? 0 : control.Width) / 2;
                aligmentRect.Y = p.Y - (double.IsNaN(control.Height) ? 0 : control.Height) / 2;

                control.Arrange(aligmentRect);
            }
        }

        #region Private Fields

        VisualCollection visualChildren;
        WarningAdornerControl control;

        #endregion Private Fields
    }
}
