#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINDOWS_PHONE
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media.Animation;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class ColumnBase : SparklineBase
    {
        #region fields

        Shape mouseUnderRect, previousRect;
#if WINDOWS_PHONE
        BindingExpression bindingExpression;
#endif

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets value whether to highlight segment on mouse move.
        /// </summary>
        public bool HighlightSegment
        {
            get { return (bool)GetValue(HighlightSegmentProperty); }
            set { SetValue(HighlightSegmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightSegment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightSegmentProperty =
            DependencyProperty.Register("HighlightSegment", typeof(bool), typeof(ColumnBase), new PropertyMetadata(false));

        #endregion

        #region methods

#if !WINDOWS_PHONE
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
#else
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
#endif
        {
#if !WINDOWS_PHONE
            base.OnPointerMoved(e);
#else
            base.OnMouseMove(e);
#endif
            if (HighlightSegment)
            {
                if (e.OriginalSource is Shape)
                {
                    mouseUnderRect = (Shape)e.OriginalSource;
                    string segmentTag="";
                    if(((object[])(mouseUnderRect.Tag))!=null)
                    segmentTag = ((object[])(mouseUnderRect.Tag))[0] as string;

                    if (mouseUnderRect != previousRect && segmentTag == "Selectable")
                    {
                        if (previousRect != null)
                            ResetColor(previousRect);
                        ApplySelectionColor(mouseUnderRect);
                        previousRect = mouseUnderRect;
                    }
                }
            }
        }
#if WINDOWS_PHONE
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerExited(PointerRoutedEventArgs e)
#endif
        {
            ResetColor(previousRect);
            previousRect = mouseUnderRect = null;
        }

        protected override void AnimateSegments(UIElementCollection elements)
        {
            int i = 0;
            foreach (UIElement element in elements)
            {
                Storyboard sb = new Storyboard();
                Shape segment = element as Shape;
                double elementHeight = segment.Height;
                if (!double.IsNaN(elementHeight))
                {
                    segment.RenderTransform = new ScaleTransform();
                    if (yValues[i] > 0)
                        segment.RenderTransformOrigin = new Point(1, 1);
                    DoubleAnimationUsingKeyFrames keyFrames1 = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame keyFrame1 = new SplineDoubleKeyFrame();
                    keyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                    keyFrame1.Value = 0;
                    keyFrames1.KeyFrames.Add(keyFrame1);
                    keyFrame1 = new SplineDoubleKeyFrame();
                    keyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1));

                    KeySpline keySpline1 = new KeySpline();
                    keySpline1.ControlPoint1 = new Point(0.64, 0.84);
                    keySpline1.ControlPoint2 = new Point(0.67, 0.95);
                    keyFrame1.KeySpline = keySpline1;
                    keyFrames1.KeyFrames.Add(keyFrame1);
                    keyFrame1.Value = 1;
#if !WINDOWS_PHONE
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
#else
                    Storyboard.SetTargetProperty(keyFrames1, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
#endif
                    Storyboard.SetTarget(keyFrames1, element);
                    sb.Children.Add(keyFrames1);
                    sb.Begin();
                    i++;
                }
            }
        }

        internal override void SetBinding(Shape element)
        {
            base.SetBinding(element);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
        }

        protected override void RenderSegments()
        {
            ClearUnUsedSegments(xValues.Count - EmptyPointIndexes.Count);
        }

        private void ApplySelectionColor(Shape segment)
        {
#if WINDOWS_PHONE
            bindingExpression = segment.GetBindingExpression(Shape.FillProperty);
#endif
            Color color = (segment.Fill as SolidColorBrush).Color;
            segment.Fill = new SolidColorBrush(Color.FromArgb(color.A, (byte)((int)color.R * 0.6), (byte)((int)color.G * 0.6), (byte)((int)color.B * 0.6)));
        }

        private void ResetColor(Shape segment)
        {
            if (segment != null)
            {
#if !WINDOWS_PHONE
                Color color = (segment.Fill as SolidColorBrush).Color;
                segment.Fill = new SolidColorBrush(Color.FromArgb(color.A, (byte)((int)color.R / 0.6), (byte)((int)color.G / 0.6), (byte)((int)color.B / 0.6)));
#else
                segment.SetBinding(Shape.FillProperty, bindingExpression.ParentBinding);
#endif
            }
        }

        protected void BindFillProperty(Shape element, string propertyPath)
        {
            base.SetBinding(element);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath(propertyPath);
            element.SetBinding(Shape.FillProperty, binding);
        }

        #endregion

    }
}
