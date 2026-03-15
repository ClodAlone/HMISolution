#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
#if !WinRT
using System.Windows.Controls;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.Controls
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class ScrollableContentPresenter : ContentPresenter, IScrollableInfo,IDisposable
    {
        private RectangleGeometry _clippingRectangle;
        private bool _isClipPropertySet = false;
        private ScrollData _scrollData = new ScrollData();
        private IScrollableInfo scrollableChild;
#if !WinRT
        #region Zoom

        /// <summary>
        /// Transformation matrix corresponding to _matrixTransform.
        /// </summary>
        private Matrix _transformation;

        /// <summary>
        /// Number of decimals to round the Matrix to.
        /// </summary>
        private const int DecimalsAfterRound = 4;

        /// <summary>
        /// RenderTransform/MatrixTransform applied to _transformRoot.
        /// </summary>
        private MatrixTransform _matrixTransform;

        /// <summary>
        /// Actual DesiredSize of Child element (the value it returned from its MeasureOverride method).
        /// </summary>
        private Size _childActualSize = Size.Empty;

        /// <summary>
        /// Acceptable difference between two doubles.
        /// </summary>
        private const double AcceptableDelta = 0.0001;

        /// <summary>
        /// Gets or sets the layout transform to apply on the LayoutTransformer 
        /// control content.
        /// </summary>
        /// <remarks>
        /// Corresponds to UIElement.LayoutTransform.
        /// </remarks>
        public Transform LayoutTransform
        {
            get { return (Transform)GetValue(LayoutTransformProperty); }
            set { SetValue(LayoutTransformProperty, value); }
        }

        /// <summary>
        /// Identifies the LayoutTransform DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty LayoutTransformProperty = DependencyProperty.Register(
            "LayoutTransform", typeof(Transform), typeof(ScrollableContentPresenter), new PropertyMetadata(LayoutTransformChanged));

        /// <summary>
        /// Handles changes to the Transform DependencyProperty.
        /// </summary>
        /// <param name="o">Source of the change.</param>
        /// <param name="e">Event args.</param>
        private static void LayoutTransformChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // Casts are safe because Silverlight is enforcing the types
            ((ScrollableContentPresenter)o).ProcessTransform((Transform)e.NewValue);
        }

        ScaleTransform scaleTransform = new ScaleTransform() { ScaleX = 1.0, ScaleY = 1.0 };

        /// <summary>
        /// Applies the layout transform on the LayoutTransformer control content.
        /// </summary>
        /// <remarks>
        /// Only used in advanced scenarios (like animating the LayoutTransform). 
        /// Should be used to notify the LayoutTransformer control that some aspect 
        /// of its Transform property has changed. 
        /// </remarks>
        public void ApplyLayoutTransform()
        {
            if (scaleTransform.ScaleX != ZoomScale || scaleTransform.ScaleY != ZoomScale)
            {
                scaleTransform.ScaleX = ZoomScale;
                scaleTransform.ScaleY = ZoomScale;
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleTransform);
                LayoutTransform = transformGroup;
                ProcessTransform(LayoutTransform);
            }
        }

        /// <summary>
        /// Processes the Transform to determine the corresponding Matrix.
        /// </summary>
        /// <param name="transform">Transform to process.</param>
        private void ProcessTransform(Transform transform)
        {
            // Get the transform matrix and apply it
            _transformation = RoundMatrix(GetTransformMatrix(transform), DecimalsAfterRound);
            if (null != _matrixTransform)
            {
                _matrixTransform.Matrix = _transformation;
            }
            // New transform means re-layout is necessary
            InvalidateMeasure();
        }

        /// <summary>
        /// Rounds the non-offset elements of a Matrix to avoid issues due to floating point imprecision.
        /// </summary>
        /// <param name="matrix">Matrix to round.</param>
        /// <param name="decimals">Number of decimal places to round to.</param>
        /// <returns>Rounded Matrix.</returns>
        private static Matrix RoundMatrix(Matrix matrix, int decimals)
        {
            return new Matrix(
                Math.Round(matrix.M11, decimals),
                Math.Round(matrix.M12, decimals),
                Math.Round(matrix.M21, decimals),
                Math.Round(matrix.M22, decimals),
                matrix.OffsetX,
                matrix.OffsetY);
        }

        /// <summary>
        /// Walks the Transform(Group) and returns the corresponding Matrix.
        /// </summary>
        /// <param name="transform">Transform(Group) to walk.</param>
        /// <returns>Computed Matrix.</returns>
        private Matrix GetTransformMatrix(Transform transform)
        {
            if (null != transform)
            {
                // WPF equivalent of this entire method:
                // return transform.Value;

                // Process the TransformGroup
                TransformGroup transformGroup = transform as TransformGroup;
                if (null != transformGroup)
                {
                    Matrix groupMatrix = Matrix.Identity;
                    foreach (Transform child in transformGroup.Children)
                    {
                        groupMatrix = MatrixMultiply(groupMatrix, GetTransformMatrix(child));
                    }
                    return groupMatrix;
                }

                // Process the RotateTransform
                RotateTransform rotateTransform = transform as RotateTransform;
                if (null != rotateTransform)
                {
                    double angle = rotateTransform.Angle;
                    double angleRadians = (2 * Math.PI * angle) / 360;
                    double sine = Math.Sin(angleRadians);
                    double cosine = Math.Cos(angleRadians);
                    return new Matrix(cosine, sine, -sine, cosine, 0, 0);
                }

                // Process the ScaleTransform
                ScaleTransform scaleTransform = transform as ScaleTransform;
                if (null != scaleTransform)
                {
                    double scaleX = scaleTransform.ScaleX;
                    double scaleY = scaleTransform.ScaleY;
                    return new Matrix(scaleX, 0, 0, scaleY, 0, 0);
                }

                // Process the SkewTransform
                SkewTransform skewTransform = transform as SkewTransform;
                if (null != skewTransform)
                {
                    double angleX = skewTransform.AngleX;
                    double angleY = skewTransform.AngleY;
                    double angleXRadians = (2 * Math.PI * angleX) / 360;
                    double angleYRadians = (2 * Math.PI * angleY) / 360;
                    return new Matrix(1, angleYRadians, angleXRadians, 1, 0, 0);
                }

                // Process the MatrixTransform
                MatrixTransform matrixTransform = transform as MatrixTransform;
                if (null != matrixTransform)
                {
                    return matrixTransform.Matrix;
                }

                // TranslateTransform has no effect in LayoutTransform
            }

            // Fall back to no-op transformation
            return Matrix.Identity;
        }

        /// <summary>
        /// Implements WPF's Matrix.Multiply on Silverlight.
        /// </summary>
        /// <param name="matrix1">First matrix.</param>
        /// <param name="matrix2">Second matrix.</param>
        /// <returns>Multiplication result.</returns>
        private static Matrix MatrixMultiply(Matrix matrix1, Matrix matrix2)
        {
            // WPF equivalent of following code:
            // return Matrix.Multiply(matrix1, matrix2);
            return new Matrix(
                (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21),
                (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22),
                (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21),
                (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22),
                ((matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21)) + matrix2.OffsetX,
                ((matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22)) + matrix2.OffsetY);
        }

        /// <summary>
        /// Compute the largest usable size (greatest area) after applying the transformation to the specified bounds.
        /// </summary>
        /// <param name="arrangeBounds">Arrange bounds.</param>
        /// <returns>Largest Size possible.</returns>
        //[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Closely corresponds to WPF's FrameworkElement.FindMaximalAreaLocalSpaceRect.")]
        private Size ComputeLargestTransformedSize(Size arrangeBounds)
        {

            // Computed largest transformed size
            Size computedSize = Size.Empty;

            // Detect infinite bounds and constrain the scenario
            bool infiniteWidth = double.IsInfinity(arrangeBounds.Width);
            if (infiniteWidth)
            {
                arrangeBounds.Width = arrangeBounds.Height;
            }
            bool infiniteHeight = double.IsInfinity(arrangeBounds.Height);
            if (infiniteHeight)
            {
                arrangeBounds.Height = arrangeBounds.Width;
            }

            // Capture the matrix parameters
            double a = _transformation.M11;
            double b = _transformation.M12;
            double c = _transformation.M21;
            double d = _transformation.M22;

            // Compute maximum possible transformed width/height based on starting width/height
            // These constraints define two lines in the positive x/y quadrant
            double maxWidthFromWidth = Math.Abs(arrangeBounds.Width / a);
            double maxHeightFromWidth = Math.Abs(arrangeBounds.Width / c);
            double maxWidthFromHeight = Math.Abs(arrangeBounds.Height / b);
            double maxHeightFromHeight = Math.Abs(arrangeBounds.Height / d);

            // The transformed width/height that maximize the area under each segment is its midpoint
            // At most one of the two midpoints will satisfy both constraints
            double idealWidthFromWidth = maxWidthFromWidth / 2;
            double idealHeightFromWidth = maxHeightFromWidth / 2;
            double idealWidthFromHeight = maxWidthFromHeight / 2;
            double idealHeightFromHeight = maxHeightFromHeight / 2;

            // Compute slope of both constraint lines
            double slopeFromWidth = -(maxHeightFromWidth / maxWidthFromWidth);
            double slopeFromHeight = -(maxHeightFromHeight / maxWidthFromHeight);

            if ((0 == arrangeBounds.Width) || (0 == arrangeBounds.Height))
            {
                // Check for empty bounds
                computedSize = new Size(arrangeBounds.Width, arrangeBounds.Height);
            }
            else if (infiniteWidth && infiniteHeight)
            {
                // Check for completely unbound scenario
                computedSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            }
            else if (!MatrixHasInverse(_transformation))
            {
                // Check for singular matrix
                computedSize = new Size(0, 0);
            }
            else if ((0 == b) || (0 == c))
            {
                // Check for 0/180 degree special cases
                double maxHeight = (infiniteHeight ? double.PositiveInfinity : maxHeightFromHeight);
                double maxWidth = (infiniteWidth ? double.PositiveInfinity : maxWidthFromWidth);
                if ((0 == b) && (0 == c))
                {
                    // No constraints
                    computedSize = new Size(maxWidth, maxHeight);
                }
                else if (0 == b)
                {
                    // Constrained by width
                    double computedHeight = Math.Min(idealHeightFromWidth, maxHeight);
                    computedSize = new Size(
                        maxWidth - Math.Abs((c * computedHeight) / a),
                        computedHeight);
                }
                else if (0 == c)
                {
                    // Constrained by height
                    double computedWidth = Math.Min(idealWidthFromHeight, maxWidth);
                    computedSize = new Size(
                        computedWidth,
                        maxHeight - Math.Abs((b * computedWidth) / d));
                }
            }
            else if ((0 == a) || (0 == d))
            {
                // Check for 90/270 degree special cases
                double maxWidth = (infiniteHeight ? double.PositiveInfinity : maxWidthFromHeight);
                double maxHeight = (infiniteWidth ? double.PositiveInfinity : maxHeightFromWidth);
                if ((0 == a) && (0 == d))
                {
                    // No constraints
                    computedSize = new Size(maxWidth, maxHeight);
                }
                else if (0 == a)
                {
                    // Constrained by width
                    double computedHeight = Math.Min(idealHeightFromHeight, maxHeight);
                    computedSize = new Size(
                        maxWidth - Math.Abs((d * computedHeight) / b),
                        computedHeight);
                }
                else if (0 == d)
                {
                    // Constrained by height
                    double computedWidth = Math.Min(idealWidthFromWidth, maxWidth);
                    computedSize = new Size(
                        computedWidth,
                        maxHeight - Math.Abs((a * computedWidth) / c));
                }
            }
            else if (idealHeightFromWidth <= ((slopeFromHeight * idealWidthFromWidth) + maxHeightFromHeight))
            {
                // Check the width midpoint for viability (by being below the height constraint line)
                computedSize = new Size(idealWidthFromWidth, idealHeightFromWidth);
            }
            else if (idealHeightFromHeight <= ((slopeFromWidth * idealWidthFromHeight) + maxHeightFromWidth))
            {
                // Check the height midpoint for viability (by being below the width constraint line)
                computedSize = new Size(idealWidthFromHeight, idealHeightFromHeight);
            }
            else
            {
                // Neither midpoint is viable; use the intersection of the two constraint lines instead
                // Compute width by setting heights equal (m1*x+c1=m2*x+c2)
                double computedWidth = (maxHeightFromHeight - maxHeightFromWidth) / (slopeFromWidth - slopeFromHeight);
                // Compute height from width constraint line (y=m*x+c; using height would give same result)
                computedSize = new Size(
                    computedWidth,
                    (slopeFromWidth * computedWidth) + maxHeightFromWidth);
            }

            // Return result
            return computedSize;
        }

        /// <summary>
        /// Implements WPF's Matrix.HasInverse on Silverlight.
        /// </summary>
        /// <param name="matrix">Matrix to check for inverse.</param>
        /// <returns>True if the Matrix has an inverse.</returns>
        private static bool MatrixHasInverse(Matrix matrix)
        {
            // WPF equivalent of following code:
            // return matrix.HasInverse;
            return (0 != ((matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21)));
        }

        /// <summary>
        /// Returns true if Size a is smaller than Size b in either dimension.
        /// </summary>
        /// <param name="a">Second Size.</param>
        /// <param name="b">First Size.</param>
        /// <returns>True if Size a is smaller than Size b in either dimension.</returns>
        private static bool IsSizeSmaller(Size a, Size b)
        {
            // WPF equivalent of following code:
            // return ((a.Width < b.Width) || (a.Height < b.Height));
            return ((a.Width + AcceptableDelta < b.Width) || (a.Height + AcceptableDelta < b.Height));
        }

        /// <summary>
        /// Implements WPF's Rect.Transform on Silverlight.
        /// </summary>
        /// <param name="rect">Rect to transform.</param>
        /// <param name="matrix">Matrix to transform with.</param>
        /// <returns>Bounding box of transformed Rect.</returns>
        private static Rect RectTransform(Rect rect, Matrix matrix)
        {
            // WPF equivalent of following code:
            // Rect rectTransformed = Rect.Transform(rect, matrix);
            Point leftTop = matrix.Transform(new Point(rect.Left, rect.Top));
            Point rightTop = matrix.Transform(new Point(rect.Right, rect.Top));
            Point leftBottom = matrix.Transform(new Point(rect.Left, rect.Bottom));
            Point rightBottom = matrix.Transform(new Point(rect.Right, rect.Bottom));
            double left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
            double top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
            double right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
            double bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));
            Rect rectTransformed = new Rect(left, top, right - left, bottom - top);
            return rectTransformed;
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _matrixTransform = new MatrixTransform();
            this.RenderTransform = _matrixTransform;
            // Apply the current transform
            ApplyLayoutTransform();
        }
#else

#if WinRT
        public ScrollableContentPresenter()
        {
            this.ManipulationMode = ManipulationModes.TranslateRailsX | ManipulationModes.TranslateRailsY;
        }
#endif
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
#endif

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Size finalSize = arrangeSize;
            if (this.ScrollOwner == null)
            {
                return base.ArrangeOverride(arrangeSize);
            }
#if !WinRT
            if (ZoomScale != 1.0)
            {
                FrameworkElement child = (VisualTreeHelper.GetChildrenCount(this) == 0) ? null : (VisualTreeHelper.GetChild(this, 0) as FrameworkElement); ;
                if ((null == ScrollOwner) || (null == child))
                {
                    // No child, use whatever was given
                    return arrangeSize;
                }

                // Determine the largest available size after the transformation
                Size finalSizeTransformed = ComputeLargestTransformedSize(arrangeSize);

                // Transform the working size to find its width/height
                Rect transformedRect = RectTransform(new Rect(0, 0, finalSizeTransformed.Width, finalSizeTransformed.Height), _transformation);
                // Create the Arrange rect to center the transformed content
                Rect finalRect = new Rect(
                    -transformedRect.Left + ((arrangeSize.Width - transformedRect.Width) / 2),
                    -transformedRect.Top + ((arrangeSize.Height - transformedRect.Height) / 2),
                    finalSizeTransformed.Width,
                    finalSizeTransformed.Height);
                arrangeSize = finalSizeTransformed;
                // This is the first opportunity under Silverlight to find out the Child's true DesiredSize
                if (IsSizeSmaller(finalSizeTransformed, child.RenderSize) && (Size.Empty == _childActualSize))
                {
                    // Unfortunately, all the work so far is invalid because the wrong DesiredSize was used
                    // Make a note of the actual DesiredSize
                    _childActualSize = new Size(child.ActualWidth, child.ActualHeight);
                    // Force a new measure/arrange pass
                    InvalidateMeasure();
                }
                else
                {
                    // Clear the "need to measure/arrange again" flag
                    _childActualSize = Size.Empty;
                }
            }
#endif
            this.UpdateClip(arrangeSize);

            UIElement element = (VisualTreeHelper.GetChildrenCount(this) == 0) ? null : (VisualTreeHelper.GetChild(this, 0) as UIElement);
            if (element != null)
            {
                if (scrollableChild != null)
                {
                    scrollableChild.SetVerticalOffset(VerticalOffset);
                    scrollableChild.SetHorizontalOffset(HorizontalOffset);
                    element.Arrange(new Rect(new Point(0, 0), arrangeSize));
                }
                else
                {
                    this.VerifyScrollData(arrangeSize, this._scrollData._extent);
                    Rect finalRect = new Rect(0.0, 0.0, element.DesiredSize.Width, element.DesiredSize.Height);
                    finalRect.X = -this._scrollData._computedOffset.X;
                    finalRect.Y = -this._scrollData._computedOffset.Y;
                    finalRect.Width = Math.Max(finalRect.Width, arrangeSize.Width);
                    finalRect.Height = Math.Max(finalRect.Height, arrangeSize.Height);
                    element.Arrange(finalRect);
                }
            }

            if (ZoomScale != 1.0)
                arrangeSize = finalSize;
            return arrangeSize;
        }

        private static double CoerceOffset(double offset, double extent, double viewport)
        {
            if (offset > (extent - viewport))
            {
                offset = extent - viewport;
            }
            if (offset < 0.0)
            {
                offset = 0.0;
            }
            return offset;
        }

        private bool CoerceOffsets()
        {
            Vector vector = new Vector(CoerceOffset(this._scrollData._offset.X, this._scrollData._extent.Width, this._scrollData._viewport.Width), CoerceOffset(this._scrollData._offset.Y, this._scrollData._extent.Height, this._scrollData._viewport.Height));
            bool result = this._scrollData._computedOffset.X == vector.X;
            result &= this._scrollData._computedOffset.Y == vector.Y;
            this._scrollData._computedOffset = vector;
            return result;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (this.ScrollOwner == null)
            {
                return base.MeasureOverride(constraint);
            }

            UIElement element = (VisualTreeHelper.GetChildrenCount(this) == 0) ? null : (VisualTreeHelper.GetChild(this, 0) as UIElement);
            if (element == null)
            {
                return new Size();
            }

            if (ScrollOwner.CanContentScroll)
                scrollableChild = element as IScrollableInfo;
            else
                scrollableChild = null;

            if (scrollableChild != null)
            {
                scrollableChild.ScrollOwner = ScrollOwner;
#if !WinRT
                if (ZoomScale != 1.0)
                {
                    ApplyLayoutTransform();
                    Size measureSize;
                    // Determine the largest size after the transformation
                    measureSize = ComputeLargestTransformedSize(constraint);
                    element.Measure(measureSize);
                    return constraint;
                }
#endif
                element.Measure(constraint);
                return element.DesiredSize;
            }

            Size size = new Size();
            Size availableSize = constraint;
            if (this._scrollData._canHorizontallyScroll)
            {
                availableSize.Width = double.PositiveInfinity;
            }
            if (this._scrollData._canVerticallyScroll)
            {
                availableSize.Height = double.PositiveInfinity;
            }
            element.Measure(availableSize);
            this.VerifyScrollData(constraint, element.DesiredSize);
            size.Width = Math.Min(constraint.Width, element.DesiredSize.Width);
            size.Height = Math.Min(constraint.Height, element.DesiredSize.Height);
            return size;
        }

        private void UpdateClip(Size arrangeSize)
        {
            if (!this._isClipPropertySet)
            {
                this._clippingRectangle = new RectangleGeometry();
                base.Clip = this._clippingRectangle;
                this._isClipPropertySet = true;
            }
            this._clippingRectangle.Rect = new Rect(0.0, 0.0, arrangeSize.Width, arrangeSize.Height);
        }

        private void VerifyScrollData(Size viewport, Size extent)
        {
            if (scrollableChild != null)
                return;

            bool equals = viewport == this._scrollData._viewport;
            equals &= extent == this._scrollData._extent;
            this._scrollData._viewport = viewport;
            this._scrollData._extent = extent;
            if (!(equals & this.CoerceOffsets()) && (this.ScrollOwner != null))
            {
                this.ScrollOwner.InvalidateScrollInfo();
            }
        }

        public IScrollableInfo ScrollableChild 
        { 
            get { return scrollableChild; } 
        }

        #region IScrollableInfo Members

        public void SetHorizontalOffset(double offset)
        {
            if (scrollableChild != null)
            {
                scrollableChild.SetHorizontalOffset(offset);
            }
            else if (this._scrollData._canHorizontallyScroll && (this._scrollData._offset.X != offset))
            {
                this._scrollData._offset.X = offset;
                base.InvalidateArrange();
            }
        }

        public void SetVerticalOffset(double offset)
        {
            if (scrollableChild != null)
            {
                scrollableChild.SetVerticalOffset(offset);
            }
            else if (this._scrollData._canVerticallyScroll && (this._scrollData._offset.Y != offset))
            {
                this._scrollData._offset.Y = offset;
                base.InvalidateArrange();
            }
        }

        public bool CanHorizontallyScroll
        {
            get
            {
                if (scrollableChild != null)
                    return scrollableChild.CanHorizontallyScroll;

                if (this._scrollData == null)
                {
                    return false;
                }
                return this._scrollData._canHorizontallyScroll;
            }
            set
            {
                if (scrollableChild != null)
                {
                    scrollableChild.CanHorizontallyScroll = value;
                    return;
                }

                if ((this._scrollData != null) && (this._scrollData._canHorizontallyScroll != value))
                {
                    this._scrollData._canHorizontallyScroll = value;
                    base.InvalidateMeasure();
                }
            }
        }

        public bool CanVerticallyScroll
        {
            get
            {
                if (scrollableChild != null)
                {
                    return scrollableChild.CanVerticallyScroll;
                }

                if (this._scrollData == null)
                {
                    return false;
                }
                return this._scrollData._canVerticallyScroll;
            }
            set
            {
                if (scrollableChild != null)
                {
                    scrollableChild.CanVerticallyScroll = value;
                    return;
                }
                
                if ((this._scrollData != null) && (this._scrollData._canVerticallyScroll != value))
                {
                    this._scrollData._canVerticallyScroll = value;
                    base.InvalidateMeasure();
                }
            }
        }

        public double ExtentHeight
        {
            get
            {
                if (scrollableChild != null)
                {
                    return scrollableChild.ExtentHeight;
                }
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return this._scrollData._extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                if (scrollableChild != null)
                {
                    return scrollableChild.ExtentWidth;
                }
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return this._scrollData._extent.Width;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                if (scrollableChild != null)
                {
                    return scrollableChild.HorizontalOffset;
                }
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return this._scrollData._computedOffset.X;
            }
        }

        public ScrollableContentViewer ScrollOwner
        {
            get
            {
                if (this._scrollData == null)
                {
                    return null;
                }
                return this._scrollData._scrollOwner;
            }
            set
            {
                if (this._scrollData != null)
                {
                    this._scrollData._scrollOwner = value;
                }
            }
        }

        public double VerticalOffset
        {
            get
            {
                if (scrollableChild != null)
                {
                    return scrollableChild.VerticalOffset;
                }
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return this._scrollData._computedOffset.Y;
            }
        }

        public double ViewportHeight
        {
            get
            {
                if (scrollableChild != null)
                {
                    return scrollableChild.ViewportHeight;
                }
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return this._scrollData._viewport.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                if (scrollableChild != null)
                {
                    return scrollableChild.ViewportWidth;
                }
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return this._scrollData._viewport.Width;
            }
        }

        public double ZoomScale
        {
            get
            {
                if (scrollableChild != null)
                {
                    return scrollableChild.ZoomScale;
                }
                return 1.0;
            }
            set
            {
                if (scrollableChild != null)
                {
                    scrollableChild.ZoomScale = value;
                }
            }
        }

        public void LineLeft()
        {
            if (scrollableChild != null)
                scrollableChild.LineLeft();
            else if (ScrollOwner != null)
                ScrollOwner.LineLeft();
        }

        public void LineRight()
        {
            if (scrollableChild != null)
                scrollableChild.LineRight();
            else if (ScrollOwner != null)
                ScrollOwner.LineRight();
        }

        public void LineUp()
        {
            if (scrollableChild != null)
                scrollableChild.LineUp();
            else if (ScrollOwner != null)
                ScrollOwner.LineUp();
        }

        public void LineDown()
        {
            if (scrollableChild != null)
                scrollableChild.LineDown();
            else if (ScrollOwner != null)
                ScrollOwner.LineDown();
        }

        public void MouseWheelUp()
        {
            this.LineUp();
        }

        public void MouseWheelDown()
        {
            this.LineDown();
        }

        public void MouseWheelLeft()
        {
            this.LineLeft();
        }

        public void MouseWheelRight()
        {
            this.LineRight();
        }

        public void PageUp()
        {
            if (scrollableChild != null)
                scrollableChild.PageUp();
            else if (ScrollOwner != null)
                ScrollOwner.PageUp();
        }

        public void PageDown()
        {
            if (scrollableChild != null)
                scrollableChild.PageDown();
            else if (ScrollOwner != null)
                ScrollOwner.PageDown();
        }

        public void PageRight()
        {
            if (scrollableChild != null)
                scrollableChild.PageRight();
            else if (ScrollOwner != null)
                ScrollOwner.PageRight();
        }

        public void PageLeft()
        {
            if (scrollableChild != null)
                scrollableChild.PageLeft();
            else if (ScrollOwner != null)
                ScrollOwner.PageLeft();
        }

        #endregion

        private class ScrollData
        {
            internal bool _canHorizontallyScroll;
            internal bool _canVerticallyScroll;
            internal ScrollableContentPresenter.Vector _computedOffset;
            internal Size _extent;
            internal ScrollableContentPresenter.Vector _offset;
            internal ScrollableContentViewer _scrollOwner;
            internal Size _viewport;
        }

        private struct Vector
        {
            private double _x;
            private double _y;
            internal Vector(double x, double y)
            {
                this._x = x;
                this._y = y;
            }

            internal double X
            {
                get
                {
                    return this._x;
                }
                set
                {
                    this._x = value;
                }
            }
            internal double Y
            {
                get
                {
                    return this._y;
                }
                set
                {
                    this._y = value;
                }
            }
        }

        public void Dispose()
        {
            this._clippingRectangle = null;
            this._scrollData._scrollOwner = null;
            this._scrollData = null;
        }
    }
}
