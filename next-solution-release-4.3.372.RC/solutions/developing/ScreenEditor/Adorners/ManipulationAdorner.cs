using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Utilities.WPF;
using Utilities.Animations;
using System.Windows.Media.Animation;

namespace ScreenManager.Adorners
{
    class ManipulationAdorner : Adorner
    {
        #region Declarations

        protected VisualCollection visualChildren;

        protected ManipulationAdornerControl editControl;
        #endregion

        #region Constructors

        public ManipulationAdorner(UIElement element)
            : base(element)
        {
            editControl = new ManipulationAdornerControl(element);
            // editControl.DataContext = element;
            editControl.HorizontalAlignment = HorizontalAlignment.Left;
            editControl.VerticalAlignment = VerticalAlignment.Center;

            visualChildren = new VisualCollection(this);

            Activate();
        }

        #endregion

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            if (transform is MatrixTransform)
            {
                var matrix = transform as MatrixTransform;
                //SkewTransform.AngleX = MatrixTransform.Matrix.M12
                //SkewTransform.AngleY = MatrixTransform.Matrix.M21
                //ScaleTransform.ScaleX = MatrixTransform.Matrix.M11
                //ScaleTransform.ScaleY = MatrixTransform.Matrix.M22
                //TranslateTransform.X = MatrixTransform.Matrix.OffsetX
                //TranslateTransform.Y = MatrixTransform.Matrix.OffsetY
                var mx = new Matrix();
                mx.Translate(matrix.Matrix.OffsetX, matrix.Matrix.OffsetY);
                return new MatrixTransform(mx);
            }
            return null;
        }

        internal void AddButton(Button btn)
        {
            if (editControl == null)
                return;

            btn.Margin = new Thickness(4, 4, 4, 4);
            editControl.panelButtons.Children.Add(btn);
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
            foreach (var v in visualChildren)
            {
                ArrangeControl(v);
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

                switch (control.VerticalAlignment)
                {
                    case VerticalAlignment.Top: aligmentRect.Y += 0;
                        break;
                    case VerticalAlignment.Bottom: aligmentRect.Y += ro.Height;
                        break;
                    case VerticalAlignment.Center: aligmentRect.Y += ro.Height / 2;
                        break;
                    case VerticalAlignment.Stretch: aligmentRect.Height = ro.Height;// *Math.Abs((AdornedElement.RenderTransform as TransformGroup).Children[0].Value.M22);
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
                    case HorizontalAlignment.Stretch: aligmentRect.Width = ro.Width;// *Math.Abs((AdornedElement.RenderTransform as TransformGroup).Children[0].Value.M11);
                        break;
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

        #region Virtuals

        public virtual void Activate()
        {
            visualChildren.Add(editControl);

            Visibility = Visibility.Visible;
            InvalidateMeasure();
            InvalidateArrange();
            var ret = FindParentInkCanvas(AdornedElement);
            if (ret != null)
                ret.InvalidateArrange();
        }

        protected Canvas FindParentInkCanvas(DependencyObject dObj)
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
            return ret = dObj.FindParent<Canvas>();
        }

        public virtual void Deactivate()
        {
            // AdornedElement.Blink(0, 0.5, 1, new BackEase() { EasingMode = EasingMode.EaseOut });

            InvalidateMeasure();
            InvalidateArrange();
            var ret = FindParentInkCanvas(AdornedElement);
            if (ret != null)
                ret.InvalidateArrange();

            visualChildren.Clear();
            AdornedElement.UpdateLayout();

            Visibility = Visibility.Collapsed;
        }

        public virtual bool IsSelactable(UIElement el)
        {
            return true;
        }

        #endregion
    }
}