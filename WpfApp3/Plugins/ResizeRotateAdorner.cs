using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp3.Plugins
{
    public class ResizeRotateAdorner : Adorner
    {
        private VisualCollection _visuals;
        private Thumb _topLeft, _topRight, _bottomLeft, _bottomRight;
        private Thumb _rotateHandle;
        private Rectangle _outline;
        private Line _rotateLine;

        public ResizeRotateAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visuals = new VisualCollection(this);

            _outline = new Rectangle { Stroke = Brushes.Blue, StrokeThickness = 1, StrokeDashArray = new DoubleCollection { 4, 2 }, IsHitTestVisible = false };
            _rotateLine = new Line { Stroke = Brushes.Blue, StrokeThickness = 1, X1 = 0, Y1 = 0, X2 = 0, Y2 = -20, IsHitTestVisible = false };

            _topLeft = BuildThumb(Cursors.SizeNWSE);
            _topRight = BuildThumb(Cursors.SizeNESW);
            _bottomLeft = BuildThumb(Cursors.SizeNESW);
            _bottomRight = BuildThumb(Cursors.SizeNWSE);
            _rotateHandle = BuildThumb(Cursors.Hand);
            _rotateHandle.Background = Brushes.LightBlue;

            _topLeft.DragDelta += (s, e) => Resize(s, e, -1, -1);
            _topRight.DragDelta += (s, e) => Resize(s, e, 1, -1);
            _bottomLeft.DragDelta += (s, e) => Resize(s, e, -1, 1);
            _bottomRight.DragDelta += (s, e) => Resize(s, e, 1, 1);
            _rotateHandle.DragDelta += Rotate_DragDelta;

            _visuals.Add(_outline);
            _visuals.Add(_rotateLine);
            _visuals.Add(_topLeft);
            _visuals.Add(_topRight);
            _visuals.Add(_bottomLeft);
            _visuals.Add(_bottomRight);
            _visuals.Add(_rotateHandle);
        }

        private Thumb BuildThumb(Cursor cursor)
        {
            return new Thumb
            {
                Cursor = cursor,
                Width = 10,
                Height = 10,
                Background = Brushes.White,
                BorderBrush = Brushes.Blue,
                BorderThickness = new Thickness(1),
                Opacity = 0.8
            };
        }

        protected override Visual GetVisualChild(int index) => _visuals[index];
        protected override int VisualChildrenCount => _visuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (AdornedElement is FrameworkElement element)
            {
                double w = element.ActualWidth;
                double h = element.ActualHeight;
                if (double.IsNaN(w)) w = element.DesiredSize.Width;
                if (double.IsNaN(h)) h = element.DesiredSize.Height;

                _outline.Arrange(new Rect(0, 0, w, h));
                
                _topLeft.Arrange(new Rect(-5, -5, 10, 10));
                _topRight.Arrange(new Rect(w - 5, -5, 10, 10));
                _bottomLeft.Arrange(new Rect(-5, h - 5, 10, 10));
                _bottomRight.Arrange(new Rect(w - 5, h - 5, 10, 10));

                _rotateLine.Arrange(new Rect(w / 2, -20, 1, 20)); // Relative to 0,0? No, Adorner coordinates.
                // Re-think logic: Adorner overlay covers element.
                // 0,0 is TopLeft of element (usually).
                
                _rotateLine.X1 = w / 2;
                _rotateLine.Y1 = 0;
                _rotateLine.X2 = w / 2;
                _rotateLine.Y2 = -20;
                _rotateLine.Arrange(new Rect(0, -20, w, 20 + h)); // Needs enough bounds

                _rotateHandle.Arrange(new Rect((w / 2) - 5, -30, 10, 10));
            }
            return finalSize;
        }

        private void Resize(object sender, DragDeltaEventArgs e, int directionX, int directionY)
        {
            if (AdornedElement is FrameworkElement element)
            {
                double minSize = 10;

                if (directionX == 1) // Right
                {
                    double newWidth = Math.Max(minSize, element.Width + e.HorizontalChange);
                    element.Width = newWidth;
                }
                else // Left
                {
                    double newWidth = Math.Max(minSize, element.Width - e.HorizontalChange);
                    if (newWidth > minSize)
                    {
                        double oldLeft = Canvas.GetLeft(element);
                        if (double.IsNaN(oldLeft)) oldLeft = 0;
                        Canvas.SetLeft(element, oldLeft + e.HorizontalChange);
                        element.Width = newWidth;
                    }
                }

                if (directionY == 1) // Bottom
                {
                    double newHeight = Math.Max(minSize, element.Height + e.VerticalChange);
                    element.Height = newHeight;
                }
                else // Top
                {
                    double newHeight = Math.Max(minSize, element.Height - e.VerticalChange);
                    if (newHeight > minSize)
                    {
                        double oldTop = Canvas.GetTop(element);
                        if (double.IsNaN(oldTop)) oldTop = 0;
                        Canvas.SetTop(element, oldTop + e.VerticalChange);
                        element.Height = newHeight;
                    }
                }
            }
        }

        private void Rotate_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (AdornedElement is FrameworkElement element)
            {
                var group = element.RenderTransform as TransformGroup;
                RotateTransform rotateTransform = null;

                if (group != null)
                {
                     foreach (var t in group.Children) if (t is RotateTransform rt) { rotateTransform = rt; break; }
                }
                else
                {
                    rotateTransform = element.RenderTransform as RotateTransform;
                }

                if (rotateTransform == null)
                {
                    rotateTransform = new RotateTransform();
                    element.RenderTransformOrigin = new Point(0.5, 0.5);
                    element.RenderTransform = rotateTransform;
                }
                
                // Simple horizontal drag to rotate
                rotateTransform.Angle += e.HorizontalChange;
            }
        }
    }
}
