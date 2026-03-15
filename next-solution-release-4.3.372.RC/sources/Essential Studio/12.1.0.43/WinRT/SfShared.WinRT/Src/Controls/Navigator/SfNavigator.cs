#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
#if !(SILVERLIGHT||WPF||WINDOWS_PHONE_7)
using Windows.Foundation;
#endif

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235
#if WINDOWS_PHONE || WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace Syncfusion.WP.Controls
#else
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace Syncfusion.Tools.Controls
#else
#if WPF
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.Licensing;
namespace Syncfusion.Windows.Controls
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
namespace Syncfusion.UI.Xaml.Controls
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a control for navigation
    /// </summary>
#if !WINRT
    [ContentProperty("Host")]
#else
    [ContentProperty(Name = "Items")]
#endif

    public class SfNavigator : Control
    {
        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.SfNavigator"/> class.
        /// </summary>
        public SfNavigator()
        {
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(SfNavigator));
            }
#endif

            this.DefaultStyleKey = typeof(SfNavigator);
            this.Loaded += NavigatorLoaded;
            Items = new ChildCollection();
        }

        void SfNavigator_LayoutUpdated(object sender, object e)
        {
            UpdateTransform();
        }

        /// <summary>
        /// Updates the layout for the transformation
        /// </summary>
        public void UpdateTransform()
        {
            if (activeContent != null && supportingContent != null)
            {
#if !WPF
                if ((supportingContent.RenderTransform as CompositeTransform).TranslateX < 0 &&
                    -(supportingContent.RenderTransform as CompositeTransform).TranslateX < activeContent.ActualWidth)
                {
                    (supportingContent.RenderTransform as CompositeTransform).TranslateX = -activeContent.ActualWidth;
                }
                else if ((supportingContent.RenderTransform as CompositeTransform).TranslateX > 0 &&
                    (supportingContent.RenderTransform as CompositeTransform).TranslateX < activeContent.ActualWidth)
                {
                    (supportingContent.RenderTransform as CompositeTransform).TranslateX = activeContent.ActualWidth;
                }
#else
                if ((supportingContent.RenderTransform as TranslateTransform).X < 0 &&
                                   -(supportingContent.RenderTransform as TranslateTransform).X < activeContent.ActualWidth)
                {
                    (supportingContent.RenderTransform as TranslateTransform).X = -activeContent.ActualWidth;
                }
                else if ((supportingContent.RenderTransform as TranslateTransform).X > 0 &&
                    (supportingContent.RenderTransform as TranslateTransform).X < activeContent.ActualWidth)
                {
                    (supportingContent.RenderTransform as TranslateTransform).X = activeContent.ActualWidth;
                }
#endif
            }

        }

#if !WINRT
        /// <summary>
        /// Using a DependencyProperty as the backing store for Host.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HostProperty =
            DependencyProperty.Register("Host", typeof (object), typeof (SfNavigator), new PropertyMetadata(default(object),OnHostChanged));

        /// <summary>
        /// Gets or sets the host
        /// </summary>
        public object Host
        {
            get { return (object) GetValue(HostProperty); }
            set { SetValue(HostProperty, value); }
        }

        private static void OnHostChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfNavigator sfNavigator = sender as SfNavigator;
            sfNavigator.Items.Add(sfNavigator.Host);
        }
#endif
        void NavigatorLoaded(object sender, RoutedEventArgs e)
        {
            if (ActiveIndex >= 0)
            {
                ValidateActiveIndex(ActiveIndex);
            }
            this.LayoutUpdated += SfNavigator_LayoutUpdated;

            this.Loaded -= NavigatorLoaded;
        }

        /// <summary>
        /// Invoke event when item is navigated
        /// </summary>
        public event RoutedEventHandler Navigated;

        /// <summary>
        /// Gets or sets the items that are active
        /// </summary>
        public object ActiveItem
        {
            get { return (object)GetValue(ActiveItemProperty); }
            set { SetValue(ActiveItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ActiveItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ActiveItemProperty =
            DependencyProperty.Register("ActiveItem", typeof(object), typeof(SfNavigator), new PropertyMetadata(null, OnActiveItemChanged));

        /// <summary>
        /// Gets or sets the index of the items that are active
        /// </summary>
        public int ActiveIndex
        {
            get { return (int)GetValue(ActiveIndexProperty); }
            set { SetValue(ActiveIndexProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ActiveIndex.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ActiveIndexProperty =
            DependencyProperty.Register("ActiveIndex", typeof(int), typeof(SfNavigator), new PropertyMetadata(-1, OnActiveIndexChanged));


        private static void OnActiveItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as SfNavigator;
            if (control != null)
            {
                control.OnActiveItemChanged(args);
            }
        }

        /// <summary>
        /// Invoked when the active item has changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnActiveItemChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null && Items.Contains(args.NewValue))
            {
                int index = this.Items.IndexOf(args.NewValue);
                int oldindex = this.Items.IndexOf(args.OldValue);

                if (index >= 0)
                {
                    direction = index > oldindex ? Direction.Next : Direction.Previous;
                    ActiveIndex = index;
                    if (PART_Content != null && PART_SupportingContent != null)
                    {
                        Navigate(args.NewValue);
                    }
                }
            }
        }

        private static void OnActiveIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as SfNavigator;
            if (control != null)
            {
                control.OnActiveIndexChanged(args);
            }
        }

        /// <summary>
        /// Invoked when the index of the active item has changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnActiveIndexChanged(DependencyPropertyChangedEventArgs args)
        {
            int index = (int)args.NewValue;
            int oldindex = (int) args.OldValue;
            ValidateActiveIndex(index, oldindex);
        }

        private void ValidateActiveIndex(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                var item = Items[index];
                ActiveItem = item;
            }
        }

        private void ValidateActiveIndex(int index, int oldindex)
        {
            if (index >= 0 && index < Items.Count)
            {
                direction = index > oldindex ? Direction.Next : Direction.Previous;
                var item = Items[index];
                ActiveItem = item;
            }
        }

        /// <summary>
        /// Function to navigate through items
        /// </summary>
        /// <param name="child"></param>
        public void Navigate(object child)
        {
            if (child == null)
                return;

            //Clip the content
            if (ActualHeight > 0 && ActualWidth > 0)
            {
                var rectangle = new RectangleGeometry();
            #if WINDOWS_PHONE||WINDOWS_PHONE_7
                  rectangle.Rect = new System.Windows.Rect {Height = ActualHeight, Width = ActualWidth, X = 0, Y = 0};
            #else
                  rectangle.Rect = new Rect {Height = ActualHeight, Width = ActualWidth, X = 0, Y = 0};
            #endif           
                this.Clip = rectangle;
            }

            if (activeContent == null) //Means there is no active content.
            {
                PART_Content.Content = child;
                activeContent = PART_Content;
                supportingContent = PART_SupportingContent;
            }
            else
            {
                if (direction == Direction.Next)
                {
#if WPF 
                    var transform = new TranslateTransform();
                    transform.X = activeContent.ActualWidth;
                    supportingContent.RenderTransform = transform;
                    supportingContent.Content = child;

                    activeContent.RenderTransform = new TranslateTransform();
#else
                    var transform = new CompositeTransform();

                    transform.TranslateX = activeContent.ActualWidth;
                    supportingContent.RenderTransform = transform;
                    supportingContent.Content = child;

                    activeContent.RenderTransform = new CompositeTransform();
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    var outstory = CreateStoryBoard(activeContent,new PropertyPath(
                                                   "(UIElement.RenderTransform).(CompositeTransform.TranslateX)"), 0,
                                                   -activeContent.ActualWidth);
                    var instory = CreateStoryBoard(supportingContent,new PropertyPath(
                                                    "(UIElement.RenderTransform).(CompositeTransform.TranslateX)"), activeContent.ActualWidth,
                                                    0);
#else
#if WPF
                    var outstory = CreateStoryBoard(activeContent,
                                                                        "(UIElement.RenderTransform).(TranslateTransform.X)", 0,
                                                                        -activeContent.ActualWidth);
                    var instory = CreateStoryBoard(supportingContent,
                                                    "(UIElement.RenderTransform).(TranslateTransform.X)", activeContent.ActualWidth,
                                                    0);
#else
                    var outstory = CreateStoryBoard(activeContent,
                                                    "(UIElement.RenderTransform).(CompositeTransform.TranslateX)", 0,
                                                    -activeContent.ActualWidth);
                    var instory = CreateStoryBoard(supportingContent,
                                                    "(UIElement.RenderTransform).(CompositeTransform.TranslateX)", activeContent.ActualWidth,
                                                    0);
#endif
#endif
#if !WPF
                    outstory.Begin();
                    instory.Begin();
#endif
#if !WINRT
                    instory.Completed += delegate(object sender, EventArgs e)
#else
                    instory.Completed += delegate(object sender, object e)
#endif
                    {
                        if (Navigated != null)
                        {
                            Navigated(this, new RoutedEventArgs());
                        }
                    };
#if WPF
                    outstory.Begin();
                    instory.Begin();
#endif
                }
                else
                {
#if WPF
                    var transform = new TranslateTransform();
                    transform.X = -activeContent.ActualWidth;
                    supportingContent.RenderTransform = transform;
                    supportingContent.Content = child;

                    activeContent.RenderTransform = new TranslateTransform();
#else
                    var transform = new CompositeTransform();
                    transform.TranslateX = -activeContent.ActualWidth;
                    supportingContent.RenderTransform = transform;
                    supportingContent.Content = child;

                    activeContent.RenderTransform = new CompositeTransform();
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    var outstory = CreateStoryBoard(activeContent,new PropertyPath(
                                                    "(UIElement.RenderTransform).(CompositeTransform.TranslateX)"), 0,
                                                    activeContent.ActualWidth);
                    var instory = CreateStoryBoard(supportingContent,new PropertyPath(
                                                    "(UIElement.RenderTransform).(CompositeTransform.TranslateX)"), -activeContent.ActualWidth,
                                                    0);
#else
#if WPF
                    var outstory = CreateStoryBoard(activeContent,
                                                    "(UIElement.RenderTransform).(TranslateTransform.X)", 0,
                                                    activeContent.ActualWidth);
                    var instory = CreateStoryBoard(supportingContent,
                                                    "(UIElement.RenderTransform).(TranslateTransform.X)", -activeContent.ActualWidth,
                                                    0);
#else
                    var outstory = CreateStoryBoard(activeContent,
                                                    "(UIElement.RenderTransform).(CompositeTransform.TranslateX)", 0,
                                                    activeContent.ActualWidth);
                    var instory = CreateStoryBoard(supportingContent,
                                                    "(UIElement.RenderTransform).(CompositeTransform.TranslateX)", -activeContent.ActualWidth,
                                                    0);
#endif
#endif
#if !WPF
                    outstory.Begin();
                    instory.Begin();
#endif
#if !WINRT
                    instory.Completed += delegate(object sender, EventArgs e)
#else
                    instory.Completed += delegate(object sender, object e)
#endif
                    {
                        if (Navigated != null)
                        {

                            Navigated(this, new RoutedEventArgs());
                           
                        }
                    };
#if WPF
                    outstory.Begin();
                    instory.Begin();
#endif
                }

                SwapActiveContent();
            }
        }

        private void SwapActiveContent()
        {
            var temp = activeContent;
            activeContent = supportingContent;
            supportingContent = temp;
        }

        private Timeline CreateAnimation(double fromvalue, double toValue)
        {
            var animation = new DoubleAnimationUsingKeyFrames();
            var frame1 = new EasingDoubleKeyFrame() { Value = fromvalue, KeyTime = TimeSpan.FromSeconds(0) };
            var frame2 = new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(0.3), Value = toValue };
            frame2.EasingFunction = new PowerEase ();
            animation.KeyFrames.Add(frame1);
            animation.KeyFrames.Add(frame2);
            return animation;
        }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        private Storyboard CreateStoryBoard(DependencyObject target, PropertyPath property, double from, double to)
#else
        private Storyboard CreateStoryBoard(DependencyObject target, string property, double from, double to)
#endif        
        {
            var timeline = CreateAnimation(from, to);
            var story = new Storyboard();
            Storyboard.SetTarget(timeline, target);
#if WPFSILVERLIGHT
            Storyboard.SetTargetProperty(timeline, new PropertyPath(property));            
#else
            Storyboard.SetTargetProperty(timeline, property);
#endif
            story.Children.Add(timeline);
            return story;
        }

        /// <summary>
        /// Gets or sets the items of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.ChildCollection"/> control.
        /// </summary>
        public ChildCollection Items { get; private set; }

        private Direction direction;

        private ContentControl PART_Content;

        private ContentControl PART_SupportingContent;

        private ContentControl activeContent;

        private ContentControl supportingContent;

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.SfNavigator"/> control.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        
        {
            PART_Content = this.GetTemplateChild("PART_Content") as ContentControl;
            PART_SupportingContent = GetTemplateChild("PART_SupportingContent") as ContentControl;

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Represents an enum list for Direction of navigation
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Navigate to the Next item
            /// </summary>
            Next,

            /// <summary>
            /// Navigate to the Previous item
            /// </summary>
            Previous
        }

    }
}
