#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Syncfusion.Windows.Chart
{
   
    /// <summary>
    /// The instance for ChartAnimation class is created internally by WPF Chart Sereis to animate segment and Adornments
    /// </summary>
    /// <seealso cref="ChartAnimation"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartAnimation : DependencyObject
    {
        /// <summary>
        /// Idenfities Animated Chart Series
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeries), typeof(ChartAnimation), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the storyboard contains all the double animation values.
        /// </summary>
        internal Storyboard Storyboard
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Adorment storyboard animation
        /// </summary>
        internal Storyboard AdornmentStoryboard
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ChartSeries.
        /// </summary>
        /// <value>The ChartSeries.</value>
        public ChartSeries Series
        {
            get { return (ChartSeries)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// Initialize new instance for Chart Animation class
        /// </summary>
        /// <param name="series">The series to animate</param>
        public ChartAnimation(ChartSeries series)
        {
            this.Storyboard = new Storyboard();
            this.AdornmentStoryboard = new Storyboard();
            this.Series = series;
        }

        /// <summary>
        /// Default initialization for Chart Animation class
        /// </summary>
        public ChartAnimation()
        {
            this.Storyboard = new Storyboard();
            this.AdornmentStoryboard = new Storyboard();
            this.Series = new ChartSeries();
        }

        /// <summary>
        /// Generate the Opacity animation to hide all the segments first and animation based on the requirements.
        /// </summary>
        internal void AnimateSeries()
        {
            Storyboard sb = new Storyboard();
            if (this.Series != null && this.Series.Presenter != null && this.Series.Presenter.m_visibleElements != null)
            {
                foreach (UIElement element in this.Series.Presenter.m_visibleElements)
                {
                    element.Opacity = 0d;
                    ContentPresenter presenter = element as ContentPresenter;
                    if (presenter != null)
                    {
                        ChartSegment segment = presenter.Content as ChartSegment;
                        if (segment != null)
                        {
                            DoubleAnimation da1 = new DoubleAnimation();
                            da1 = getDoubleAnimation(segment, element, new PropertyPath(UIElement.OpacityProperty), 1d, 0d);
                            sb.Children.Add(da1);
                            da1.BeginTime = new TimeSpan(0, 0, 0, 0, 0);
                            da1.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0));
                            da1.FillBehavior = FillBehavior.HoldEnd;

                            DoubleAnimation da = new DoubleAnimation();
                            da = getDoubleAnimation(segment, element, new PropertyPath(UIElement.OpacityProperty), 0d, 1d);
                            sb.Children.Add(da);
                            da.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0));
                        }
                    }
                }

                sb.Begin();
                this.Storyboard.Stop();
                this.Storyboard.Children.Clear();
                foreach (UIElement element in this.Series.Presenter.m_visibleElements)
                {
                    GenerateSeriesAnimation(element as ContentPresenter, Series.Presenter.TotalSize, this.Storyboard);
                }

                if (this.Series.AdornmentPresenter != null && this.AdornmentStoryboard.Children.Count != 0)
                {
                    AnimateAdornment(this.Series.AdornmentPresenter);
                }

                this.Storyboard.Begin();
            }
        }

        /// <summary>
        /// Animate the Adornments based on the Animation options selected
        /// </summary>
        /// <param name="Elements"></param>
        internal void AnimateAdornment(List<UIElement> Elements)
        {
            if (Elements == null)
            {
                return;
            }

            Storyboard adornment = this.AdornmentStoryboard;
            adornment.Stop();
            adornment.Children.Clear();
            Storyboard storyboard = new Storyboard();
            foreach (UIElement element in Elements)
            {
                element.Opacity = 0d;
                FrameworkElement frameworkelemnt = (FrameworkElement)element;
                ChartAdornmentsPresenter.ChartAdornmentContainer container = (ChartAdornmentsPresenter.ChartAdornmentContainer)frameworkelemnt;

                DoubleAnimation opacityanimation1 = new DoubleAnimation();
                opacityanimation1 = getDoubleAnimation(container.Adornment, element, new PropertyPath(UIElement.OpacityProperty), 1d, 0d);
                storyboard.Children.Add(opacityanimation1);
                opacityanimation1.BeginTime = new TimeSpan(0, 0, 0, 0, 0);
                opacityanimation1.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0));
                opacityanimation1.FillBehavior = FillBehavior.HoldEnd;

                DoubleAnimation opacityanimation = new DoubleAnimation();
                opacityanimation = getDoubleAnimation(container.Adornment, element, new PropertyPath(UIElement.OpacityProperty), 0d, 1d);
                storyboard.Children.Add(opacityanimation);
                opacityanimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0));
            }

            storyboard.Begin();
            foreach (UIElement element in Elements)
            {
                DoubleAnimation animation = new DoubleAnimation();
                FrameworkElement frameworkelemnt = (FrameworkElement)element;
                ChartAdornmentsPresenter.ChartAdornmentContainer container = (ChartAdornmentsPresenter.ChartAdornmentContainer)frameworkelemnt;
                Size avilableSize = container.Adornment.Series.Presenter.TotalSize;
                TranslateTransform tt = new TranslateTransform();
                DoubleAnimation danimation = null;
                switch (Series.AnimateOption)
                {
                    case AnimationOptions.Bottom:
                        frameworkelemnt.RenderTransform = tt;
                        danimation = getDoubleAnimation(container.Adornment, frameworkelemnt, "(Shape.RenderTransform).(TranslateTransform.Y)", avilableSize.Height, 0);
                        adornment.Children.Add(danimation);
                        break;
                    case AnimationOptions.Top:
                        frameworkelemnt.RenderTransform = tt;
                        danimation = getDoubleAnimation(container.Adornment, frameworkelemnt, "(Shape.RenderTransform).(TranslateTransform.Y)", 0-avilableSize.Height, 0);
                        adornment.Children.Add(danimation);
                        break;
                    case AnimationOptions.Right:
                        frameworkelemnt.RenderTransform = tt;
                        danimation = getDoubleAnimation(container.Adornment, frameworkelemnt, "(Shape.RenderTransform).(TranslateTransform.X)", avilableSize.Width, 0);
                        adornment.Children.Add(danimation);
                        break;
                    case AnimationOptions.Left:
                        frameworkelemnt.RenderTransform = tt;
                        danimation = getDoubleAnimation(container.Adornment, frameworkelemnt, "(Shape.RenderTransform).(TranslateTransform.X)", 0-avilableSize.Width, 0);
                        adornment.Children.Add(danimation);
                        break;
                    case AnimationOptions.Rotate:
                        RotateTransform rt = new RotateTransform();
                        rt.CenterX = container.DesiredSize.Width / 2d;
                        rt.CenterY = container.DesiredSize.Height / 2d;
                        frameworkelemnt.RenderTransform = rt;
                        danimation = getDoubleAnimation(container.Adornment, frameworkelemnt, "(Shape.RenderTransform).(RotateTransform.Angle)", 0, 360);
                        adornment.Children.Add(danimation);
                        break;
                    case AnimationOptions.Fade:
                        danimation = getDoubleAnimation(container.Adornment, frameworkelemnt, new PropertyPath(UIElement.OpacityProperty), 0, 1);
                        adornment.Children.Add(danimation);
                        break;
                    case AnimationOptions.Scaling:
                        ScaleTransform st = new ScaleTransform();
                        st.CenterX = container.ActualWidth / 2d;
                        st.CenterY = container.ActualHeight / 2d;
                        frameworkelemnt.RenderTransform = st;
                        danimation = getDoubleAnimation(container.Adornment, frameworkelemnt, "(Shape.RenderTransform).(ScaleTransform.ScaleX)", 0, 1);
                        adornment.Children.Add(danimation);
                        adornment.Children.Add(getDoubleAnimation(container.Adornment, frameworkelemnt, "(Shape.RenderTransform).(ScaleTransform.ScaleY)", 0, 1));
                        break;
                }
            }

            adornment.Begin();
        }

        /// <summary>
        /// Generate the Animation for each segments and add to the Story board
        /// </summary>
        /// <param name="presenter">series presenter</param>
        /// <param name="avilableSize">total size</param>
        /// <param name="sb">story board</param>
        private void GenerateSeriesAnimation(ContentPresenter presenter, Size avilableSize, Storyboard sb)
        {
            int count = VisualTreeHelper.GetChildrenCount(presenter);
            if (count > 0)
            {
                FrameworkElement element = VisualTreeHelper.GetChild(presenter, 0) as FrameworkElement;
                List<UIElement> obj = null;
                if (element is Canvas)
                {
                    obj = (from UIElement ele in ((Canvas)element).Children select ele).ToList<UIElement>();
                }
                else if (element is Grid)
                {
                    obj = (from UIElement ele in ((Grid)element).Children select ele).ToList<UIElement>();
                }
                else
                {
                    obj = new List<UIElement>();
                    obj.Add(element);
                }

                ChartSegment segment = presenter.Content as ChartSegment;
                sb.FillBehavior = FillBehavior.Stop;
                foreach (FrameworkElement sh in obj)
                {
                    TranslateTransform tt = new TranslateTransform();
                    DoubleAnimation danimation = null;
                    switch (Series.AnimateOption)
                    {
                        case AnimationOptions.Bottom:
                            sh.RenderTransform = tt;
                            danimation = getDoubleAnimation(segment, sh, "(Shape.RenderTransform).(TranslateTransform.Y)", avilableSize.Height, 0);
                            break;
                        case AnimationOptions.Top:
                            sh.RenderTransform = tt;
                            danimation = getDoubleAnimation(segment, sh, "(Shape.RenderTransform).(TranslateTransform.Y)", 0 - avilableSize.Height, 0d);
                            break;
                        case AnimationOptions.Right:
                            sh.RenderTransform = tt;
                            danimation = getDoubleAnimation(segment, sh, "(Shape.RenderTransform).(TranslateTransform.X)", avilableSize.Width, 0d);
                            break;
                        case AnimationOptions.Left:
                            sh.RenderTransform = tt;
                            danimation = getDoubleAnimation(segment, sh, "(Shape.RenderTransform).(TranslateTransform.X)", 0 - avilableSize.Width, 0d);
                            break;
                        case AnimationOptions.Rotate:
                            RotateTransform t1 = new RotateTransform();
                            t1.CenterX = double.IsNaN(sh.Width) ? (sh is Path || sh is Polyline ? avilableSize.Width / 2d : sh.DesiredSize.Width / 2d) : sh.Width / 2d;
                            t1.CenterY = double.IsNaN(sh.Height) ? (sh is Path || sh is Polyline ? avilableSize.Height / 2d : sh.DesiredSize.Height / 2d) : sh.Height / 2d;
                            sh.RenderTransform = t1;
                            danimation = getDoubleAnimation(segment, sh, "(Shape.RenderTransform).(RotateTransform.Angle)", 0d, 360d);
                            break;
                        case AnimationOptions.Fade:
                            danimation = getDoubleAnimation(segment, sh, new PropertyPath(Shape.OpacityProperty), 0d, 1d);
                            break;
                        case AnimationOptions.Scaling:
                            ScaleTransform t = new ScaleTransform();
                            t.CenterX = double.IsNaN(sh.Width) ? (sh is Path || sh is Polyline ? avilableSize.Width / 2d : sh.DesiredSize.Width / 2d) : sh.Width / 2d; 
                            t.CenterY = double.IsNaN(sh.Height) ? (sh is Path || sh is Polyline ? avilableSize.Height / 2d : sh.DesiredSize.Height / 2d) : sh.Height / 2d; 
                            sh.RenderTransform = t;
                            danimation = getDoubleAnimation(segment, sh, "(Shape.RenderTransform).(ScaleTransform.ScaleX)", 0d, 1d);
                            DoubleAnimation d2 = getDoubleAnimation(segment, sh, "(Shape.RenderTransform).(ScaleTransform.ScaleY)", 0d, 1d);
                            sb.Children.Add(d2);
                            break;
                    }

                    sb.Children.Add(danimation);
                }
            }
        }

        /// <summary>
        /// Create Double Animation object based the Targment element, From and To
        /// </summary>
        /// <param name="segment">Segment to animate</param>
        /// <param name="targetElement">target element</param>
        /// <param name="propertypath">property path</param>
        /// <param name="from">from - double value</param>
        /// <param name="to">to double value</param>
        /// <returns>Double Animation</returns>
        private DoubleAnimation getDoubleAnimation(ChartSegment segment, DependencyObject targetElement, PropertyPath propertypath, double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation();
            Storyboard.SetTarget(animation, targetElement);
            Storyboard.SetTargetProperty(animation, propertypath);
            animation.From = from;
            animation.To = double.IsNaN(to) ? 0 : to;
            TimeSpan timespan = Series.AnimationDuration;
            double totalmiliseconds = timespan.TotalMilliseconds;
            double averageseconds = 0d;
            averageseconds = totalmiliseconds / (double)(segment is ChartAdornment ? segment.Series.Adornments.Count : segment.Series.Segments.Count);
            if (Series.AnimateOneByOne == true)
            {
                int index = segment is ChartAdornment ? segment.Series.Adornments.IndexOf(segment as ChartAdornment) : segment.Series.Segments.IndexOf(segment);
                index = (index < 0) ? index = 0 : index;
                averageseconds = double.IsInfinity(averageseconds) ? totalmiliseconds : averageseconds;
                animation.BeginTime = new TimeSpan(0, 0, 0, 0, (int)averageseconds * index);
                animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)averageseconds));
            }
            else
            {
                animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)totalmiliseconds));
            }

            return animation;
        }

        /// <summary>
        /// Create Double Animation object based the Targment element, From and To
        /// </summary>
        /// <param name="segment">>Segment to animate</param>
        /// <param name="targetElement">target element</param>
        /// <param name="propertypath">property path as string</param>
        /// <param name="from">from - double value</param>
        /// <param name="to">to double value</param>
        /// <returns>Double Animation</returns>
        private DoubleAnimation getDoubleAnimation(ChartSegment segment, DependencyObject targetElement, string propertypath, double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation();
            Storyboard.SetTarget(animation, targetElement);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertypath));
            animation.From = double.IsNaN(from) ? ((Shape)targetElement).ActualWidth : from;
            animation.To = double.IsNaN(to) ? 0 : to;
            TimeSpan timespan = Series.AnimationDuration;
            double totalmiliseconds = timespan.TotalMilliseconds;
            double averageseconds = 0d;
            averageseconds = totalmiliseconds / (double)(segment is ChartAdornment ? segment.Series.Adornments.Count : segment.Series.Segments.Count);
            if (Series.AnimateOneByOne == true)
            {
                int index = segment is ChartAdornment ? segment.Series.Adornments.IndexOf(segment as ChartAdornment) : segment.Series.Segments.IndexOf(segment);
                index = (index < 0) ? index = 0 : index;
                averageseconds = double.IsInfinity(averageseconds) ? totalmiliseconds : averageseconds;
                animation.BeginTime = new TimeSpan(0, 0, 0, 0, (int)averageseconds * index);
                animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)averageseconds));
            }
            else
            {
                animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)totalmiliseconds));
            }

            return animation;
        }
    }
}
