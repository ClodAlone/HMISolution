// <copyright file="VistaSpecificProgressBar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class gives vista-like progress-bar.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class VistaSpecificProgressBar : FrameworkElement
    {
        #region Constants

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant indicates the maximum possible percent for
        /// visibility value.
        /// </summary>
        private const int MaxPercentVisibility = 100;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant indicates the minimum possible percent for
        /// visibility value.
        /// </summary>
        private const int MinPercentVisibility = 0;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant presents child count.
        /// </summary>
        private const int ChildCount = 2;

        #endregion Constants

        #region Private members

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This value indicates the main rectangle for progress
        /// animation.
        /// </summary>
        private readonly Rectangle m_MainRect = new Rectangle();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This value indicates the top rectangle for progress
        /// animation.
        /// </summary>
        private readonly Rectangle m_TopRect = new Rectangle();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This value indicates storyboard for progress animation.
        /// </summary>
        private readonly Storyboard m_Storyboard = new Storyboard();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This value indicates animation for change opacity.
        /// </summary>
        private readonly DoubleAnimation m_DoubleAnimation = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This value indicates whether animation is in progress.
        /// </summary>
        private bool m_isAnimationInProgres = false;

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="VistaSpecificProgressBar"/> class.
        /// </summary>
        static VistaSpecificProgressBar()
        {
            EnvironmentTest.ValidateLicense(typeof(VistaSpecificProgressBar));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VistaSpecificProgressBar"/> class.
        /// </summary>
        public VistaSpecificProgressBar()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DispatcherHandler(LoadDispatherLoaded));
            m_DoubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(700)), FillBehavior.Stop);
            m_DoubleAnimation.Completed += new EventHandler(OnDoubleAnimationCompleted);
        }

        #endregion Initialization

        #region Events

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when PercentVisibility property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback PercentVisibilityChanged;

        #endregion Events

        #region Properties

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets the value of the PercentVisibility dependency
        /// property.
        /// </summary>
        private double PercentVisibility
        {
            get
            {
                return (Double)GetValue(PercentVisibilityProperty);
            }

            set
            {
                SetValue(PercentVisibilityProperty, value);
            }
        }

        #endregion Properties

        #region Implementation

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method starts progress-bar-like animation.
        /// </summary>
        internal void StartProgressBarAnimation()
        {
            if (!m_isAnimationInProgres)
            {
                PropertyPath propertyPath = new PropertyPath(VistaSpecificProgressBar.PercentVisibilityProperty);

                DoubleAnimation doubleAnimationFirst = new DoubleAnimation(0, 25, new Duration(TimeSpan.FromSeconds(5)));
                Storyboard.SetTargetProperty(doubleAnimationFirst, propertyPath);
                Storyboard.SetTargetName(doubleAnimationFirst, Name);

                DoubleAnimation doubleAnimationTwo = new DoubleAnimation(25, 50, new Duration(TimeSpan.FromSeconds(10))) { BeginTime = TimeSpan.FromSeconds(5) };
                Storyboard.SetTargetProperty(doubleAnimationTwo, propertyPath);
                Storyboard.SetTargetName(doubleAnimationTwo, Name);

                DoubleAnimation doubleAnimationThree = new DoubleAnimation(50, 75, new Duration(TimeSpan.FromSeconds(15))) { BeginTime = TimeSpan.FromSeconds(15) };
                Storyboard.SetTargetProperty(doubleAnimationThree, propertyPath);
                Storyboard.SetTargetName(doubleAnimationThree, Name);

                DoubleAnimation doubleAnimationFour = new DoubleAnimation(75, 100, new Duration(TimeSpan.FromSeconds(30))) { BeginTime = TimeSpan.FromSeconds(30) };
                Storyboard.SetTargetProperty(doubleAnimationFour, propertyPath);
                Storyboard.SetTargetName(doubleAnimationFour, Name);

                m_Storyboard.Children.Add(doubleAnimationFirst);
                m_Storyboard.Children.Add(doubleAnimationTwo);
                m_Storyboard.Children.Add(doubleAnimationThree);
                m_Storyboard.Children.Add(doubleAnimationFour);

                m_Storyboard.Begin(this, true);
                m_isAnimationInProgres = true;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method stops progress-bar-like animation.
        /// </summary>
        internal void StopProgressBarAnimation()
        {
            if (m_isAnimationInProgres)
            {
                m_Storyboard.Stop(this);
                PercentVisibility = 100;
                m_isAnimationInProgres = false;
                ClearValue(VistaSpecificProgressBar.OpacityProperty);
                BeginAnimation(VistaSpecificProgressBar.OpacityProperty, m_DoubleAnimation);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises
        /// PercentVisibilityChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnPercentVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PercentVisibilityChanged != null)
            {
                PercentVisibilityChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return ChildCount;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Returns the specified Visual objects for which it is the
        /// parent.
        /// </summary>
        /// <param name="index">The index of the visual object.</param>
        /// <returns>
        /// The child which response to specified index.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            if (0 == index)
            {
                return m_TopRect;
            }
            else if (1 == index)
            {
                return m_MainRect;
            }
            else
            {
                Debug.Fail("What is it?");
                return null;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Positions child elements and determines size.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to
        /// arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double newWidth = finalSize.Width * PercentVisibility / MaxPercentVisibility;
            double minPartHeight = finalSize.Height / 5;
            double topHeight = 2 * minPartHeight;

            Rect rect = new Rect(0, 0, newWidth, topHeight);
            m_TopRect.Arrange(rect);

            rect = new Rect(0, topHeight, newWidth, 3 * minPartHeight);
            m_MainRect.Arrange(rect);

            return base.ArrangeOverride(finalSize);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size.
        /// </summary>
        /// <param name="availableSize">The available size that this
        /// element can give to child
        /// elements. Infinity can be
        /// specified as a value to indicate
        /// that the element will size to
        /// whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout,
        /// based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            m_TopRect.Measure(availableSize);
            m_MainRect.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls after control's graphic was loaded.
        /// </summary>
        private void LoadDispatherLoaded()
        {
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            linearGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xE4, 0xFB, 0xE4), .5));
            linearGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xC8, 0xF3, 0xD3), .5));
            m_TopRect.Fill = linearGradientBrush;
            AddVisualChild(m_TopRect);

            SolidColorBrush solidColorBrush = new SolidColorBrush
            {
                Color = Color.FromArgb(0xFF, 0x7A, 0xE5, 0x91)
            };
            m_MainRect.Fill = solidColorBrush;
            AddVisualChild(m_MainRect);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method calls when opacity animation is completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        /// <remarks>For class object DoubleAnimation it is impossible to define
        /// which sender raised the event.</remarks>
        private void OnDoubleAnimationCompleted(Object sender, EventArgs e)
        {
            PercentVisibility = 0;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Validates accuracy of new value.
        /// </summary>
        /// <remarks>If value is incorrect, it will not be set.</remarks>
        /// <param name="percentVisibilityTarget">New value for checking
        /// the validity.</param>
        /// <returns>
        /// True if new value is correct; otherwise, false
        /// </returns>
        private static bool ValidatePercentVisibility(Object percentVisibilityTarget)
        {
            double percentVisibility = (Double)percentVisibilityTarget;
            return MinPercentVisibility <= percentVisibility && MaxPercentVisibility >= percentVisibility;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnPercentVisibilityChanged method of the instance,
        /// notifies about dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnPercentVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VistaSpecificProgressBar instance = (VistaSpecificProgressBar)d;
            instance.OnPercentVisibilityChanged(e);
        }

        #endregion Implementation

        #region Dependency property

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates percent's visibility state.
        /// </summary>
        private static readonly DependencyProperty PercentVisibilityProperty =
            DependencyProperty.Register("PercentVisibility", typeof(double), typeof(VistaSpecificProgressBar), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnPercentVisibilityChanged)), new ValidateValueCallback(ValidatePercentVisibility));

        #endregion Dependency property
    }
}