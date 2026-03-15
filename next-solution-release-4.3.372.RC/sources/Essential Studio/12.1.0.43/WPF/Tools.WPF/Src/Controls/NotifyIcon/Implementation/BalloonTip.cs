// <copyright file="BalloonTip.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the BalloonTip class.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class BalloonTip : HeaderedContentControl
    {
        #region Private members
        /// <summary>
        /// Stores the Showing Timer value.
        /// </summary>
        private DispatcherTimer m_showingTimer = null;

        /// <summary>
        /// Stores the hiding timer value.
        /// </summary>
        private DispatcherTimer m_hidingTimer = null;

        /// <summary>
        /// Stores the popup.
        /// </summary>
        private NonStickingPopup m_popup = null;

        /// <summary>
        /// Stores the Animation value.
        /// </summary>
        private AnimationTimeline m_showingAnimation = null;

        /// <summary>
        /// Stores the hiding animation value.
        /// </summary>
        private AnimationTimeline m_hidingAnimation = null;

        /// <summary>
        /// Stores the animation in process value.
        /// </summary>
        private bool m_bAnimationInProress = false;

        /// <summary>
        /// Stores the saved width value.
        /// </summary>
        private double m_savedWidth = 0.0;

        /// <summary>
        /// Stores the saved height value.
        /// </summary>
        private double m_savedHeight = 0.0;

        /// <summary>
        /// Stores the saved placement rectangle.
        /// </summary>
        private Rect m_savedPlacementRect;

        /////// <summary>
        /////// Default background.
        /////// </summary>
        ////private Brush m_defaultBackground = Brushes.White;

        /////// <summary>
        /////// Default foreground.
        /////// </summary>
        ////private Brush m_defaultForeground = SystemColors.ControlTextBrush;

        /////// <summary>
        /////// Default border brush.
        /////// </summary>
        ////private Brush m_defaultBorderBrush = Brushes.Black;

        /////// <summary>
        /////// Default background.
        /////// </summary>
        ////private Brush m_defaultBackgroundWithHeader = Brushes.White;

        /////// <summary>
        /////// Default foreground.
        /////// </summary>
        ////private Brush m_defaultForegroundWithHeader = SystemColors.ControlTextBrush;

        /////// <summary>
        /////// Default border brush.
        /////// </summary>
        ////private Brush m_defaultBorderBrushWithHeader = Brushes.Black;

        /////// <summary>
        /////// Default background.
        /////// </summary>
        ////private Brush m_defaultHeaderBackground = Brushes.White;

        /////// <summary>
        /////// Default foreground.
        /////// </summary>
        ////private Brush m_defaultHeaderForeground = SystemColors.ControlTextBrush;
        #endregion 

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="BalloonTip"/> class.
        /// </summary>
        static BalloonTip()
        {
            EnvironmentTest.ValidateLicense(typeof(BalloonTip));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BalloonTip), new FrameworkPropertyMetadata(typeof(BalloonTip)));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the task bar direction.
        /// </summary>
        /// <value>The task bar direction.</value>

       // private TaskBarDirection taskBarDirection=TaskBarDirection .Bottom ;
        public TaskBarDirection TaskBarDirection
        {
            get
            {
                return (TaskBarDirection)GetValue(TaskBarDirectionProperty);
            }

            set
            {
                SetValue(TaskBarDirectionProperty, value);
            }
        }

        /////// <summary>
        /////// Gets or sets the default background. This is a dependency property.
        /////// </summary>
        ////internal Brush DefaultBackground
        ////{
        ////    get
        ////    {
        ////        return (Brush)GetValue(DefaultBackgroundProperty);
        ////    }

        ////    set
        ////    {
        ////        SetValue(DefaultBackgroundProperty, value);
        ////    }
        ////}

        /////// <summary>
        /////// Gets or sets the default foreground. This is a dependency property.
        /////// </summary>
        ////internal Brush DefaultForeground
        ////{
        ////    get
        ////    {
        ////        return (Brush)GetValue(DefaultForegroundProperty);
        ////    }

        ////    set
        ////    {
        ////        SetValue(DefaultForegroundProperty, value);
        ////    }
        ////}

        /////// <summary>
        /////// Gets or sets the default BorderBrush. This is a dependency property.
        /////// </summary>
        ////internal Brush DefaultBorderBrush
        ////{
        ////    get
        ////    {
        ////        return (Brush)GetValue(DefaultBorderBrushProperty);
        ////    }

        ////    set
        ////    {
        ////        SetValue(DefaultBorderBrushProperty, value);
        ////    }
        ////}

        /// <summary>
        /// Gets or sets the header background of the balloon tip. This is a dependency property.
        /// </summary>
        public Brush HeaderBackground
        {
            get
            {
                return (Brush)GetValue(HeaderBackgroundProperty);
            }

            set
            {
                SetValue(HeaderBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header foreground of the balloon tip. This is a dependency property.
        /// </summary>
        public Brush HeaderForeground
        {
            get
            {
                return (Brush)GetValue(HeaderForegroundProperty);
            }

            set
            {
                SetValue(HeaderForegroundProperty, value);
            }
        }

        /////// <summary>
        /////// Gets or sets the default header background of the balloon tip. This is a dependency property.
        /////// </summary>
        ////internal Brush DefaultHeaderBackground
        ////{
        ////    get
        ////    {
        ////        return (Brush)GetValue(DefaultHeaderBackgroundProperty);
        ////    }

        ////    set
        ////    {
        ////        SetValue(DefaultHeaderBackgroundProperty, value);
        ////    }
        ////}

        /////// <summary>
        /////// Gets or sets the default header foreground of the balloon tip. This is a dependency property.
        /////// </summary>
        ////internal Brush DefaultHeaderForeground
        ////{
        ////    get
        ////    {
        ////        return (Brush)GetValue(DefaultHeaderForegroundProperty);
        ////    }

        ////    set
        ////    {
        ////        SetValue(DefaultHeaderForegroundProperty, value);
        ////    }
        ////}

        /// <summary>
        /// Gets or sets the balloon tip icon.
        /// </summary>
        /// <value>The balloon tip icon.</value>
        public BalloonTipIcon BalloonTipIcon
        {
            get
            {
                return (BalloonTipIcon)GetValue(BalloonTipIconProperty);
            }

            set
            {
                SetValue(BalloonTipIconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the balloon tip text.
        /// </summary>
        /// <value>The balloon tip text.</value>
        public string BalloonTipText
        {
            get
            {
                return (string)GetValue(BalloonTipTextProperty);
            }

            set
            {
                SetValue(BalloonTipTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the balloon tip title.
        /// </summary>
        /// <value>The balloon tip title.</value>
        public string BalloonTipTitle
        {
            get
            {
                return (string)GetValue(BalloonTipTitleProperty);
            }

            set
            {
                SetValue(BalloonTipTitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Internal balloon tip icon.
        /// </summary>
        /// <value>The Internal balloon tip icon.</value>
        internal BalloonTipIcon InternalBalloonTipIcon
        {
            get
            {
                return (BalloonTipIcon)GetValue(InternalBalloonTipIconProperty);
            }

            set
            {
                SetValue(InternalBalloonTipIconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Internal the balloon tip text.
        /// </summary>
        /// <value>The balloon tip text.</value>
        internal string InternalBalloonTipText
        {
            get
            {
                return (string)GetValue(InternalBalloonTipTextProperty);
            }

            set
            {
                SetValue(InternalBalloonTipTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Internal the balloon tip title.
        /// </summary>
        /// <value>The balloon tip title.</value>
        internal string InternalBalloonTipTitle
        {
            get
            {
                return (string)GetValue(InternalBalloonTipTitleProperty);
            }

            set
            {
                SetValue(InternalBalloonTipTitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the balloon tip location.
        /// </summary>
        /// <value>The balloon tip location.</value>
        public Point BalloonTipLocation
        {
            get
            {
                return (Point)GetValue(BalloonTipLocationProperty);
            }

            set
            {
                SetValue(BalloonTipLocationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the balloon tip size.
        /// </summary>
        /// <value>The balloon tip size.</value>
        public Size BalloonTipSize
        {
            get
            {
                return (Size)GetValue(BalloonTipSizeProperty);
            }

            set
            {
                SetValue(BalloonTipSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the balloon tip shape.
        /// </summary>
        /// <value>The balloon tip shape.</value>
        public BalloonTipShapes BalloonTipShape
        {
            get
            {
                return (BalloonTipShapes)GetValue(BalloonTipShapeProperty);
            }

            set
            {
                SetValue(BalloonTipShapeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the show balloon tip time in milliseconds.
        /// </summary>
        /// <value>The start balloon tip time.</value>
        public double ShowBalloonTipTime
        {
            get
            {
                return (double)GetValue(ShowBalloonTipTimeProperty);
            }

            set
            {
                SetValue(ShowBalloonTipTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the hide balloon tip time in milliseconds.
        /// </summary>
        /// <value>The hide balloon tip time.</value>
        public double HideBalloonTipTime
        {
            get
            {
                return (double)GetValue(HideBalloonTipTimeProperty);
            }

            set
            {
                SetValue(HideBalloonTipTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the balloon tip animation effect.
        /// </summary>
        /// <value>The balloon tip animation effect.</value>
        public BalloonTipAnimationEffects BalloonTipAnimationEffect
        {
            get
            {
                return (BalloonTipAnimationEffects)GetValue(BalloonTipAnimationEffectProperty);
            }

            set
            {
                SetValue(BalloonTipAnimationEffectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom balloon tip animation.
        /// </summary>
        /// <value>The custom balloon tip animation.</value>
        public AnimationTimeline CustomShowingAnimation
        {
            get
            {
                return (AnimationTimeline)GetValue(CustomShowingAnimationProperty);
            }

            set
            {
                SetValue(CustomShowingAnimationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom hiding animation.
        /// </summary>
        /// <value>The custom hiding animation.</value>
        public AnimationTimeline CustomHidingAnimation
        {
            get
            {
                return (AnimationTimeline)GetValue(CustomHidingAnimationProperty);
            }

            set
            {
                SetValue(CustomHidingAnimationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom animated property.
        /// </summary>
        /// <value>The custom animated property.</value>
        public DependencyProperty CustomAnimatedProperty
        {
            get
            {
                return (DependencyProperty)GetValue(CustomAnimatedPropertyProperty);
            }

            set
            {
                SetValue(CustomAnimatedPropertyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the balloon tip header image.
        /// </summary>
        /// <value>The balloon tip header image.</value>
        public ImageSource HeaderImage
        {
            get
            {
                return (ImageSource)GetValue(HeaderImageProperty);
            }

            set
            {
                SetValue(HeaderImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the icon template.
        /// </summary>
        /// <value>The icon template.</value>
        public DataTemplate IconTemplate
        {
            get
            {
                return (DataTemplate)GetValue(IconTemplateProperty);
            }

            set
            {
                SetValue(IconTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsOpenProperty is true.
        /// property.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }

            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value of the BalloonTipHeaderVisibilityProperty dependency
        /// property.
        /// </summary>
        public Visibility BalloonTipHeaderVisibility
        {
            get
            {
                return (Visibility)GetValue(BalloonTipHeaderVisibilityProperty);
            }

            set
            {
                SetValue(BalloonTipHeaderVisibilityProperty, value);
            }
        }

        #endregion

        #region dependency properties
        /// <summary>
        /// Identifies the <see cref="TaskBarDirection"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty TaskBarDirectionProperty = DependencyProperty.Register("TaskBarDirection", typeof(TaskBarDirection), typeof(BalloonTip), new FrameworkPropertyMetadata(TaskBarDirection.Bottom));

        /////// <summary>
        /////// Identifies the <see cref="DefaultBackground"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultBackgroundProperty =
        ////    DependencyProperty.Register("DefaultBackground", typeof(Brush), typeof(BalloonTip), new FrameworkPropertyMetadata(Brushes.Transparent));

        /////// <summary>
        /////// Identifies the <see cref="DefaultForeground"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultForegroundProperty =
        ////    DependencyProperty.Register("DefaultForeground", typeof(Brush), typeof(BalloonTip), new FrameworkPropertyMetadata(Brushes.Black));

        /////// <summary>
        /////// Identifies the <see cref="DefaultBorderBrush"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultBorderBrushProperty =
        ////    DependencyProperty.Register("DefaultBorderBrush", typeof(Brush), typeof(BalloonTip), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the <see cref="HeaderBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(BalloonTip), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the <see cref="HeaderForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(BalloonTip), new FrameworkPropertyMetadata(Brushes.Black));

        /////// <summary>
        /////// Identifies the <see cref="DefaultHeaderBackground"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultHeaderBackgroundProperty =
        ////    DependencyProperty.Register("DefaultHeaderBackground", typeof(Brush), typeof(BalloonTip), new FrameworkPropertyMetadata(Brushes.Transparent));

        /////// <summary>
        /////// Identifies the <see cref="DefaultHeaderForeground"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultHeaderForegroundProperty =
        ////    DependencyProperty.Register("DefaultHeaderForeground", typeof(Brush), typeof(BalloonTip), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the <see cref="BalloonTipIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipIconProperty =
            DependencyProperty.Register("BalloonTipIcon", typeof(BalloonTipIcon), typeof(BalloonTip), new FrameworkPropertyMetadata(BalloonTipIcon.None, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="BalloonTipText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipTextProperty =
            DependencyProperty.Register("BalloonTipText", typeof(string), typeof(BalloonTip), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnBalloonTipTextChanged)));

        /// <summary>
        /// Identifies the <see cref="BalloonTipTitle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipTitleProperty =
            DependencyProperty.Register("BalloonTipTitle", typeof(string), typeof(BalloonTip), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="InternalBalloonTipIcon"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty InternalBalloonTipIconProperty =
            DependencyProperty.Register("InternalBalloonTipIcon", typeof(BalloonTipIcon), typeof(BalloonTip), new FrameworkPropertyMetadata(BalloonTipIcon.None));

        /// <summary>
        /// Identifies the <see cref="InternalBalloonTipText"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty InternalBalloonTipTextProperty =
            DependencyProperty.Register("InternalBalloonTipText", typeof(string), typeof(BalloonTip), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="InternalBalloonTipTitle"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty InternalBalloonTipTitleProperty =
            DependencyProperty.Register("InternalBalloonTipTitle", typeof(string), typeof(BalloonTip), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BalloonTipLocation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipLocationProperty =
            DependencyProperty.Register("BalloonTipLocation", typeof(Point), typeof(BalloonTip), new FrameworkPropertyMetadata(new Point(0.0, 0.0)));

        /// <summary>
        /// Identifies the <see cref="BalloonTipSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipSizeProperty =
            DependencyProperty.Register("BalloonTipSize", typeof(Size), typeof(BalloonTip), new FrameworkPropertyMetadata(Size.Empty));

        /// <summary>
        /// Identifies the <see cref="BalloonTipShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipShapeProperty =
            DependencyProperty.Register("BalloonTipShape", typeof(BalloonTipShapes), typeof(BalloonTip), new FrameworkPropertyMetadata(BalloonTipShapes.Balloon, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnBalloonTipShapeChanged)));

        /// <summary>
        /// Identifies the <see cref="ShowBalloonTipTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowBalloonTipTimeProperty =
            DependencyProperty.Register("ShowBalloonTipTime", typeof(double), typeof(BalloonTip), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnBalloonTipTimeChanged)));

        /// <summary>
        /// Identifies the <see cref="HideBalloonTipTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HideBalloonTipTimeProperty =
            DependencyProperty.Register("HideBalloonTipTime", typeof(double), typeof(BalloonTip), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnBalloonTipTimeChanged)));

        /// <summary>
        /// Identifies the <see cref="BalloonTipAnimationEffect"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipAnimationEffectProperty =
            DependencyProperty.Register("BalloonTipAnimationEffect", typeof(BalloonTipAnimationEffects), typeof(BalloonTip), new FrameworkPropertyMetadata(BalloonTipAnimationEffects.Fade, new PropertyChangedCallback(OnBalloonTipAnimationEffectChanged)));

        /// <summary>
        /// Identifies the <see cref="CustomShowingAnimation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomShowingAnimationProperty =
            DependencyProperty.Register("CustomShowingAnimation", typeof(AnimationTimeline), typeof(BalloonTip), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCustomShowingAnimationChanged)));

        /// <summary>
        /// Identifies the <see cref="CustomHidingAnimation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomHidingAnimationProperty =
            DependencyProperty.Register("CustomHidingAnimation", typeof(AnimationTimeline), typeof(BalloonTip), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCustomHidingAnimationChanged)));

        /// <summary>
        /// Identifies the <see cref="CustomAnimatedProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomAnimatedPropertyProperty =
            DependencyProperty.Register("CustomAnimatedProperty", typeof(DependencyProperty), typeof(BalloonTip), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HeaderImage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderImageProperty =
            DependencyProperty.Register("HeaderImage", typeof(ImageSource), typeof(BalloonTip), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IconTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(BalloonTip), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// This property is responsible for the blink balloon tip
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(BalloonTip), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsOpenChanged), new CoerceValueCallback(CoerceBalloonTipOpen)));

        /// <summary>
        /// This property is responsible for the visibility of balloon tip's header.
        /// </summary>
        public static readonly DependencyProperty BalloonTipHeaderVisibilityProperty =
            DependencyProperty.Register("BalloonTipHeaderVisibility", typeof(Visibility), typeof(BalloonTip), new FrameworkPropertyMetadata(Visibility.Collapsed, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnBalloonTipHeaderVisibilityChanged)));

        #endregion //dependency properties

        #region Events
        /// <summary>
        /// Occurs when [balloon tip is opening].
        /// </summary>
        public event CancelEventHandler BalloonTipOpening;

        /// <summary>
        /// Occurs when [balloon tip is opened].
        /// </summary>
        public event PropertyChangedCallback BalloonTipOpened;

        /// <summary>
        /// Occurs when [balloon tip is hiding].
        /// </summary>
        public event CancelEventHandler BalloonTipHiding;

        /// <summary>
        /// Occurs when [balloon tip is hidden].
        /// </summary>
        public event PropertyChangedCallback BalloonTipHidden;

        /// <summary>
        /// Occurs when [close button click].
        /// </summary>
        public event EventHandler CloseButtonClick;

        /// <summary>
        /// Occurs when click is performed.
        /// </summary>
        public event EventHandler Click;
        #endregion //Events

        #region Implementation

        /// <summary>
        /// Invoked whenever the <see cref="BalloonTipText"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnBalloonTipTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonTip instance = (BalloonTip)d;
            instance.OnBalloonTipTextChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="BalloonTipText"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnBalloonTipTextChanged(DependencyPropertyChangedEventArgs e)
        {
            m_savedWidth = 0.0;
            m_savedHeight = 0.0;
        }

        /// <summary>
        /// Calls OnBalloonTipShapeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnBalloonTipShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonTip instance = (BalloonTip)d;
            instance.OnBalloonTipShapeChanged(e);
        }

        /// <summary>
        /// Called when balloon tip shape changes.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnBalloonTipShapeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BalloonTipShape == BalloonTipShapes.Balloon &&
                m_popup != null && m_popup.IsOpen)
            {
                m_savedHeight = 0.0;

                m_savedPlacementRect = FindPlacementRectangle();
                m_popup.PlacementRectangle = m_savedPlacementRect;
            }
        }

        /// <summary>
        /// Calls OnBalloonTipTimeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnBalloonTipTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonTip instance = (BalloonTip)d;
            instance.OnBalloonTipTimeChanged(e);
        }

        /// <summary>
        /// Called when balloon tip time changes.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnBalloonTipTimeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_showingAnimation == null || m_hidingAnimation == null)
            {
                InitializeAnimetion();
            }
            else
            {
                m_showingAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(ShowBalloonTipTime));
                m_hidingAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(HideBalloonTipTime));
            }
        }

        /// <summary>
        /// Calls OnBalloonTipAnimationEffectChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnBalloonTipAnimationEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonTip instance = (BalloonTip)d;
            instance.OnBalloonTipAnimationEffectChanged(e);
        }

        /// <summary>
        /// Called when balloon tip animation changes.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnBalloonTipAnimationEffectChanged(DependencyPropertyChangedEventArgs e)
        {
            InitializeAnimetion();
        }

        /// <summary>
        /// Calls OnCustomShowingAnimationChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnCustomShowingAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonTip instance = (BalloonTip)d;
            instance.OnCustomShowingAnimationChanged(e);
        }

        /// <summary>
        /// Called when custom showing animation changes.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnCustomShowingAnimationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CustomShowingAnimation != null)
            {
                CustomShowingAnimation.Completed += new EventHandler(ShowingAnimation_Completed);
            }
        }

        /// <summary>
        /// Calls OnCustomHidingAnimationChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnCustomHidingAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonTip instance = (BalloonTip)d;
            instance.OnCustomHidingAnimationChanged(e);
        }

        /// <summary>
        /// Called when custom hiding animation changes.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnCustomHidingAnimationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CustomHidingAnimation != null)
            {
                CustomHidingAnimation.Completed += new EventHandler(HidingAnimation_Completed);
            }
        }

        /// <summary>
        /// Coerces the balloon tip open.
        /// </summary>
        /// <param name="d">Sender Object.</param>
        /// <param name="obj">Value to be coerced.</param>
        /// <returns>Coerced Value</returns>
        private static object CoerceBalloonTipOpen(DependencyObject d, object obj)
        {
            BalloonTip instance = (BalloonTip)d;
            return instance.CoerceBalloonTipOpen(obj);
        }

        /// <summary>
        /// Coerces the balloon tip open.
        /// </summary>
        /// <param name="obj">The value to be Coerced.</param>
        /// <returns>Coerced value</returns>
        private object CoerceBalloonTipOpen(object obj)
        {
            bool isOpen = (bool)obj;
            CancelEventArgs args = new CancelEventArgs();

            if (isOpen)
            {
                if (IsOpen)
                {
                    return isOpen;
                }

                if (BalloonTipOpening != null)
                {
                    BalloonTipOpening(this, args);
                }

                if (args.Cancel)
                {
                    isOpen = false;
                }
            }

            return isOpen;
        }

        /// <summary>
        /// Calls OnIsOpenChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonTip instance = (BalloonTip)d;
            instance.OnIsOpenChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IsOpenChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsOpen)
            {
                if (m_popup == null)
                {
                    ApplyTemplate();
                }
            }
            else
            {
                if (BalloonTipHidden != null)
                {
                    BalloonTipHidden(this, e);
                }

                ClearInternalProperties();
            }
        }

        /// <summary>
        /// Calls OnBalloonTipHeaderVisibilityChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnBalloonTipHeaderVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonTip instance = (BalloonTip)d;
            instance.OnBalloonTipHeaderVisibilityChanged(e);
        }

        /// <summary>
        /// Called when balloon tip header visibility changes.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnBalloonTipHeaderVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            ////if (BalloonTipHeaderVisibility == Visibility.Visible)
            ////{
            ////    m_defaultBackgroundWithHeader = m_defaultBackground;
            ////    m_defaultBorderBrushWithHeader = m_defaultBorderBrush;
            ////    m_defaultForegroundWithHeader = m_defaultForeground;
            ////}
            ////else
            ////{
            ////    m_defaultBackground = m_defaultBackgroundWithHeader;
            ////    m_defaultBorderBrush = m_defaultBorderBrushWithHeader;
            ////    m_defaultForeground = m_defaultForegroundWithHeader;
            ////}

            if (m_popup != null && m_popup.IsOpen)
            {
                m_savedHeight = 0.0;

                m_savedPlacementRect = FindPlacementRectangle();
                m_popup.PlacementRectangle = m_savedPlacementRect;
            }
        }

        /// <summary>
        /// Starts the showing animation.
        /// </summary>
        private void StartShowingAnimation()
        {
            if (m_bAnimationInProress)
            {
                return;
            }

            m_bAnimationInProress = true;

            Size size = GetBalloonTipSize();

            m_savedPlacementRect = FindPlacementRectangle();

            if (BalloonTipAnimationEffect == BalloonTipAnimationEffects.Fade)
            {
                DoubleAnimation anim= null;

                if (m_showingAnimation != null)
                {
                    anim = (DoubleAnimation)m_showingAnimation;
                }
                else
                {
                    InitializeAnimetion();
                    anim = (DoubleAnimation)m_showingAnimation;
                }

                anim.From = 0.0;
                anim.To = 1.0;

                m_popup.PlacementRectangle = m_savedPlacementRect;

                BeginAnimation(OpacityProperty, anim);
            }
            else if (BalloonTipAnimationEffect == BalloonTipAnimationEffects.Slide)
            {
                if (m_popup != null)
                {
                    Opacity = 1.0;

                    RectAnimation anim = null;

                    if (m_showingAnimation != null)
                    {
                        anim = (RectAnimation)m_showingAnimation;
                    }
                    else
                    {
                        InitializeAnimetion();
                        anim = (RectAnimation)m_showingAnimation;
                    }

                    System.Drawing.Rectangle screenRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                    Point point = new Point();

                    if (TaskBarDirection == TaskBarDirection.Left)
                    {
                        point.X = screenRect.Left - size.Width;
                        point.Y = m_savedPlacementRect.Y;
                    }
                    else if (TaskBarDirection == TaskBarDirection.Top)
                    {
                        point.X = m_savedPlacementRect.X;
                        point.Y = screenRect.Top - size.Height;
                    }
                    else if (TaskBarDirection == TaskBarDirection.Right)
                    {
                        point.X = screenRect.Width + size.Width;
                        point.Y = m_savedPlacementRect.Y;
                    }
                    else
                    {
                        point.X = m_savedPlacementRect.X;
                        point.Y = screenRect.Bottom + size.Height;
                    }

                    anim.From = new Rect(point, size);
                    anim.To = m_savedPlacementRect;

                    m_popup.BeginAnimation(Popup.PlacementRectangleProperty, anim);
                }
            }
            else if (BalloonTipAnimationEffect == BalloonTipAnimationEffects.Scale)
            {


                DoubleAnimation anim = null;

                if (m_showingAnimation != null)
                {
                    anim = (DoubleAnimation)m_showingAnimation;
                }
                else
                {
                    InitializeAnimetion();
                    anim = (DoubleAnimation)m_showingAnimation;
                }

                DependencyProperty propToAnimate;

                Opacity = 1.0;
                m_popup.PlacementRectangle = m_savedPlacementRect;

                anim.From = 0.0;
                anim.Completed += new EventHandler(anim_Completed);
                if (TaskBarDirection == TaskBarDirection.Top || TaskBarDirection == TaskBarDirection.Bottom)
                {
                    propToAnimate = HeightProperty;
                    anim.To = size.Height;
                }
                else
                {
                    Height = size.Height;

                    Width = 0.0;
                    propToAnimate = WidthProperty;
                    anim.To = size.Width;
                }

                BeginAnimation(propToAnimate, anim);
            }
            else
            {
                if (CustomAnimatedProperty == null)
                {
                    throw new ArgumentException("CustomAnimatedProperty is null");
                }

                BeginAnimation(CustomAnimatedProperty, CustomShowingAnimation);
            }
        }

        void anim_Completed(object sender, EventArgs e)
        {
            BeginAnimation(HeightProperty, null);
        }

        /// <summary>
        /// Starts the hiding animation.
        /// </summary>
        private void StartHidingAnimation()
        {
            if (!IsOpen || m_bAnimationInProress || Opacity == 0.0 ||
                ActualWidth == 0.0 || ActualHeight == 0.0)
            {
                return;
            }

            if (BalloonTipHiding != null)
            {
                CancelEventArgs args = new CancelEventArgs();
                BalloonTipHiding(this, args);

                if (args.Cancel)
                {
                    return;
                }
            }

            Size size = GetBalloonTipSize();
            m_savedWidth = ActualWidth;
            m_savedHeight = ActualHeight;

            m_bAnimationInProress = true;

            if (BalloonTipAnimationEffect == BalloonTipAnimationEffects.Fade)
            {
                DoubleAnimation anim = (DoubleAnimation)m_hidingAnimation;

                anim.From = 1.0;
                anim.To = 0.0;

                BeginAnimation(OpacityProperty, anim);
            }
            else if (BalloonTipAnimationEffect == BalloonTipAnimationEffects.Slide)
            {
                RectAnimation anim = (RectAnimation)m_hidingAnimation;
                System.Drawing.Rectangle screenRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

                anim.From = new Rect(m_popup.PlacementRectangle.Location, size);

                if (TaskBarDirection == TaskBarDirection.Left)
                {
                    anim.To = new Rect(new Point(-m_savedWidth, m_popup.PlacementRectangle.Y), size);
                }
                else if (TaskBarDirection == TaskBarDirection.Top)
                {
                    anim.To = new Rect(new Point(m_popup.PlacementRectangle.X, -m_savedHeight), size);
                }
                else if (TaskBarDirection == TaskBarDirection.Right)
                {
                    anim.To = new Rect(new Point(screenRect.Right + m_savedWidth, m_popup.PlacementRectangle.Y), size);
                }
                else
                {
                    anim.To = new Rect(new Point(m_popup.PlacementRectangle.X, screenRect.Bottom + m_savedHeight), size);
                }

                m_popup.BeginAnimation(Popup.PlacementRectangleProperty, anim);
            }
            else if (BalloonTipAnimationEffect == BalloonTipAnimationEffects.Scale)
            {
                DoubleAnimation anim = (DoubleAnimation)m_hidingAnimation;
                DependencyProperty propToAnimate;

                if (TaskBarDirection == TaskBarDirection.Top || TaskBarDirection == TaskBarDirection.Bottom)
                {
                    propToAnimate = HeightProperty;
                    anim.From = m_savedHeight;
                }
                else
                {
                    propToAnimate = WidthProperty;
                    anim.From = m_savedWidth;
                }

                anim.To = 0.0;

                BeginAnimation(propToAnimate, anim);
            }
            else
            {
                if (CustomAnimatedProperty == null)
                {
                    throw new ArgumentException("CustomAnimatedProperty is null");
                }

                BeginAnimation(CustomAnimatedProperty, CustomHidingAnimation);
            }
        }

        /// <summary>
        /// Gets the size of the balloon tip.
        /// </summary>
        /// <returns>Returns the size</returns>
        private Size GetBalloonTipSize()
        {
            Size retValue = new Size(0.0, 0.0);

            ////if (m_savedWidth != 0.0 && !double.IsNaN(m_savedWidth) &&
            ////    m_savedHeight != 0.0 && !double.IsNaN(m_savedHeight))
            ////{
            ////    retValue = new Size(m_savedWidth, m_savedHeight);
            ////}
            ////else
            ////{
            if (ActualWidth != 0.0 && !double.IsNaN(ActualWidth))
            {
                retValue.Width = ActualWidth;
            }
            else
            {
                retValue.Width = m_savedWidth;
            }

            if (ActualHeight != 0.0 && !double.IsNaN(ActualHeight))
            {
                retValue.Height = ActualHeight;
            }
            else
            {
                retValue.Height = m_savedHeight;
            }

            Measure(new Size(double.MaxValue, double.MaxValue));

            if (retValue.Width == 0.0)
            {
                retValue.Width = DesiredSize.Width;
            }

            if (retValue.Height == 0.0)
            {
                retValue.Height = DesiredSize.Height;
            }
            ////}

            return retValue;
        }

        /// <summary>
        /// Initializes the animation
        /// </summary>
        private void InitializeAnimetion()
        {
            UnsubscribeAnimation();

            if (BalloonTipAnimationEffect == BalloonTipAnimationEffects.Slide)
            {
                m_showingAnimation = new RectAnimation();
                m_hidingAnimation = new RectAnimation();
            }
            else
            {
                m_showingAnimation = new DoubleAnimation();
                m_hidingAnimation = new DoubleAnimation();
            }

            m_showingAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(ShowBalloonTipTime));
            m_showingAnimation.Completed += new EventHandler(ShowingAnimation_Completed);

            m_hidingAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(HideBalloonTipTime));
            m_hidingAnimation.Completed += new EventHandler(HidingAnimation_Completed);
        }

        /// <summary>
        /// Unsubscribe the animation.
        /// </summary>
        private void UnsubscribeAnimation()
        {
            if (m_showingAnimation != null)
            {
                m_showingAnimation.Completed -= new EventHandler(ShowingAnimation_Completed);
                m_showingAnimation = null;
            }

            if (m_hidingAnimation != null)
            {
                m_hidingAnimation.Completed -= new EventHandler(HidingAnimation_Completed);
                m_hidingAnimation = null;
            }
        }

        /// <summary>
        /// Clears the internal properties.
        /// </summary>
        private void ClearInternalProperties()
        {
            ClearValue(InternalBalloonTipIconProperty);
            ClearValue(InternalBalloonTipTextProperty);
            ClearValue(InternalBalloonTipTitleProperty);
        }

        /// <summary>
        /// Shows the balloon tip.
        /// </summary>
        /// <param name="timeToShow">The time in milliseconds</param>
        public void ShowBalloonTip(int timeToShow)
        {
            ShowBalloonTip(timeToShow, BalloonTipTitle, BalloonTipText, BalloonTipIcon);
        }

        /// <summary>
        /// Shows the balloon tip.
        /// </summary>
        /// <param name="timeToShow">The time in milliseconds</param>
        /// <param name="title">Stores the title.</param>
        /// <param name="text">Stores the text.</param>
        /// <param name="tipIcon">Stores the tip icon.</param>
        public void ShowBalloonTip(int timeToShow, string title, string text, BalloonTipIcon tipIcon)
        {
            if (timeToShow < 0)
            {
                throw new ArgumentOutOfRangeException("TimeTo show is not valid!");
            }

            InternalBalloonTipIcon = BalloonTipIcon;
            InternalBalloonTipText = BalloonTipText;
            InternalBalloonTipTitle = BalloonTipTitle;

            BalloonTipIcon = tipIcon;
            BalloonTipText = text;
            BalloonTipTitle = title;

            if (!IsOpen)
            {
                IsOpen = true;

                m_showingTimer.Interval = new TimeSpan(0, 0, 0, 0, timeToShow);
                m_showingTimer.Start();
            }
        }

        /// <summary>
        /// Hides the balloon tip.
        /// </summary>
        public void HideBalloonTip()
        {
            StartHidingAnimation();
        }

        /// <summary>
        /// Hides the balloon tip.
        /// </summary>
        /// <param name="timeToHide">The time in milliseconds</param>
        public void HideBalloonTip(int timeToHide)
        {
            m_hidingTimer.Interval = new TimeSpan(0, 0, 0, 0, timeToHide);
            m_hidingTimer.Start();
        }

        /// <summary>
        /// Finds the placement rectangle.
        /// </summary>
        /// <returns>Returns a rectangle</returns>
        private Rect FindPlacementRectangle()
        {
            Rect retValue;
            Size balloonSize = GetBalloonTipSize();
            TaskBarDirection = NotifyIcon.GetTaskBarDirestion();

            if (BalloonTipLocation.X != 0.0 || BalloonTipLocation.Y != 0.0)
            {
                if (BalloonTipShape == BalloonTipShapes.Balloon)
                {
                    retValue = FindBallonPlacementRectangle(BalloonTipLocation, balloonSize);
                }
                else
                {
                    retValue = new Rect(BalloonTipLocation, balloonSize);
                }
            }
            else
            {
                NotifyIcon parent = TemplatedParent as NotifyIcon;
                if (BalloonTipShape == BalloonTipShapes.Balloon && parent != null)
                {
                    Rect iconRect = parent.FindIconRectInTray();
                    Point location = new Point(iconRect.X + (iconRect.Width / 2), iconRect.Y + (iconRect.Height / 2));
                    retValue = FindBallonPlacementRectangle(location, balloonSize);
                }
                else
                {
                    IntPtr taskbar = NativeMethods.FindWindow("Shell_TrayWnd", String.Empty);

                    RECT rect;
                    Point point = new Point(0.0, 0.0);
                    if (NativeMethods.GetWindowRect(taskbar, out rect))
                    {
                        if (TaskBarDirection == TaskBarDirection.Left)
                        {
                            point = new Point(rect.right, rect.bottom - balloonSize.Height);
                        }
                        else if (TaskBarDirection == TaskBarDirection.Right)
                        {
                            point = new Point(rect.left - balloonSize.Width, rect.bottom - balloonSize.Height);
                        }
                        else if (TaskBarDirection == TaskBarDirection.Bottom)
                        {
                            point = new Point(rect.right - balloonSize.Width, rect.top - balloonSize.Height);
                        }
                        else
                        {
                            point = new Point(rect.right - balloonSize.Width, rect.bottom);
                        }
                    }

                    retValue = new Rect(point, balloonSize);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Finds the balloon placement rectangle.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="size">The size of the object.</param>
        /// <returns>Returns a rectangle</returns>
        private Rect FindBallonPlacementRectangle(Point location, Size size)
        {
            Point p;
            CornerRadius radius = (CornerRadius)GetValue(Border.CornerRadiusProperty);
            double offcetX = BalloonTipBorder.DEF_CORNERHEIGHT + radius.BottomRight;

            if (TaskBarDirection == TaskBarDirection.Left)
            {
                p = new Point(location.X - offcetX, location.Y - size.Height);
            }
            else if (TaskBarDirection == TaskBarDirection.Top)
            {
                p = new Point(location.X - (size.Width - offcetX-BalloonTipBorder.DEF_CORNERHEIGHT), location.Y);
            }
            else
            {
               
                p = new Point(location.X - (size.Width - offcetX-BalloonTipBorder.DEF_CORNERHEIGHT), location.Y - size.Height);
            }

            Rect finalrect = new Rect(p, size);

            RECT rect;
            IntPtr taskbar = NativeMethods.FindWindow("Shell_TrayWnd", String.Empty);
            if (NativeMethods.GetWindowRect(taskbar, out rect))
            {
                int count = 0;
                foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    count++;
                }
                if (BalloonTipLocation.X != 0 && BalloonTipLocation.Y != 0)
                {
                    if (count <= 1)
                    {
                       
                            
                            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                            {
                                if (screen.Bounds.Contains(rect.left, rect.top))
                                {
                                    
                                    if (p.X < 0)
                                    {
                                        if (location.X < 0)
                                        {
                                            p.X = 0;
                                        }
                                        else
                                        {
                                            p.X = location.X;
                                        }
                                    }
                                    if (p.Y < 0)
                                    {
                                        if (location.Y < 0)
                                        {
                                            p.Y = 0;
                                        }
                                        else
                                        {
                                            p.Y = location.Y;
                                        }
                                    }
                                    break;
                                }
                            }
                            finalrect = new Rect(p, size);
                        }
                    
                    else
                    {
                        finalrect = new Rect(p, size);
                    }
                }
            }
            return finalrect;
        }

        /// <summary>
        /// Calls when balloon tip closes.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected internal void FireCloseButtonClick(EventArgs e)
        {
            IsOpen = false;

            if (CloseButtonClick != null)
            {
                CloseButtonClick(this, e);
            }
        }

        /// <summary>
        /// Handles the Tick event of the ShowingTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ShowingTimer_Tick(object sender, EventArgs e)
        {
            HideBalloonTip();
            m_showingTimer.Stop();
        }

        /// <summary>
        /// Handles the Tick event of the HidingTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HidingTimer_Tick(object sender, EventArgs e)
        {
            HideBalloonTip();
            m_hidingTimer.Stop();
        }

        /// <summary>
        /// Handles the Completed event of the ShowingAnimation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ShowingAnimation_Completed(object sender, EventArgs e)
        {
            if (BalloonTipOpened != null)
            {
                DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs(IsOpenProperty, false, true);
                BalloonTipOpened(this, args);
            }

            m_bAnimationInProress = false;
        }

        /// <summary>
        /// Handles the Completed event of the HidingAnimation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HidingAnimation_Completed(object sender, EventArgs e)
        {
            BeginAnimation(OpacityProperty, null);
            BeginAnimation(WidthProperty, null);
            BeginAnimation(HeightProperty, null);
            BeginAnimation(Popup.PlacementRectangleProperty, null);

            BalloonTipIcon = InternalBalloonTipIcon;
            BalloonTipText = InternalBalloonTipText;
            BalloonTipTitle = InternalBalloonTipTitle;

            IsOpen = false;
            m_bAnimationInProress = false;
        }

        /// <summary>
        /// Handles the Opened event of the Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Popup_Opened(object sender, EventArgs e)
        {
            StartShowingAnimation();
        }
        
        /// <summary>
        /// Handles the Closed event of the Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Popup_Closed(object sender, EventArgs e)
        {
            BeginAnimation(OpacityProperty, null);
            m_popup.BeginAnimation(Popup.PlacementRectangleProperty, null);
            BeginAnimation(HeightProperty, null);
            BeginAnimation(WidthProperty, null);
        }

        #endregion //Implementation

        #region Overrides

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            m_showingTimer = new DispatcherTimer();
            m_showingTimer.Tick += new EventHandler(ShowingTimer_Tick);

            m_hidingTimer = new DispatcherTimer();
            m_hidingTimer.Tick += new EventHandler(HidingTimer_Tick);

            ////Shared.DictionaryList list1 = SkinStorage.GetVisualStylesList(this);
            ////if (list1 != null)
            ////{
            ////    Shared.DictionaryList list2 = list1["Default"] as Shared.DictionaryList;
            ////    if (list2 != null)
            ////    {
            ////        if (list2.ContainsKey("Background"))
            ////        {
            ////            m_defaultBackground = list2["Background"] as Brush;
            ////        }

            ////        if (list2.ContainsKey("BorderBrush"))
            ////        {
            ////            m_defaultBorderBrush = list2["BorderBrush"] as Brush;
            ////        }

            ////        if (list2.ContainsKey("Foreground"))
            ////        {
            ////            m_defaultForeground = list2["Foreground"] as Brush;
            ////        }

            ////        if (list2.ContainsKey("HeaderBackground"))
            ////        {
            ////            m_defaultHeaderBackground = list2["HeaderBackground"] as Brush;
            ////        }

            ////        if (list2.ContainsKey("HeaderForeground"))
            ////        {
            ////            m_defaultHeaderForeground = list2["HeaderForeground"] as Brush;
            ////        }
            ////    }
            ////}

            ////if (SkinStorage.GetVisualStyle(this) == "Default")
            ////{
            ////    if (BalloonTipHeaderVisibility == Visibility.Visible)
            ////    {
            ////        Background = m_defaultBackgroundWithHeader;
            ////        BorderBrush = m_defaultBorderBrushWithHeader;
            ////        Foreground = m_defaultForegroundWithHeader;
            ////    }
            ////    else
            ////    {
            ////        Background = m_defaultBackground;
            ////        BorderBrush = m_defaultBorderBrush;
            ////        Foreground = m_defaultForeground;
            ////    }

            ////    HeaderBackground = m_defaultHeaderBackground;
            ////    HeaderForeground = m_defaultHeaderForeground;
            ////}
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_popup = Parent as NonStickingPopup;

            if (m_popup == null)
            {
                m_popup = new NonStickingPopup();
                m_popup.StaysOpen = true;
                m_popup.Placement = PlacementMode.Absolute;
                m_popup.Child = this;

                Binding b = new Binding();
                b.Source = this;
                b.Path = new PropertyPath(IsOpenProperty);
            }

            m_popup.Opened += new EventHandler(Popup_Opened);
            m_popup.Closed += new EventHandler(Popup_Closed);
        }

        /// <summary>
        /// Called when some dependencyProperty is changed.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            ////if (e.Property == SkinStorage.VisualStyleProperty &&
            ////    (string)e.NewValue == "Default")
            ////{
            ////    if (BalloonTipHeaderVisibility == Visibility.Visible)
            ////    {
            ////        Background = m_defaultBackgroundWithHeader;
            ////        Foreground = m_defaultForegroundWithHeader;
            ////        BorderBrush = m_defaultBorderBrushWithHeader;
            ////    }
            ////    else
            ////    {
            ////        Background = m_defaultBackground;
            ////        Foreground = m_defaultForeground;
            ////        BorderBrush = m_defaultBorderBrush;
            ////    }

            ////    HeaderBackground = m_defaultHeaderBackground;
            ////    HeaderForeground = m_defaultHeaderForeground;
            ////}
            ////else if (e.Property == BackgroundProperty)
            ////{
            ////    if (BalloonTipHeaderVisibility == Visibility.Visible)
            ////    {
            ////        m_defaultBackgroundWithHeader = Background;
            ////    }
            ////    else
            ////    {
            ////        m_defaultBackground = Background;
            ////    }
            ////}
            ////else if (e.Property == BorderBrushProperty)
            ////{
            ////    if (BalloonTipHeaderVisibility == Visibility.Visible)
            ////    {
            ////        m_defaultBorderBrushWithHeader = Background;
            ////    }
            ////    else
            ////    {
            ////        m_defaultBorderBrush = BorderBrush;
            ////    }
            ////}
            ////else if (e.Property == ForegroundProperty)
            ////{
            ////    if (BalloonTipHeaderVisibility == Visibility.Visible)
            ////    {
            ////        m_defaultForegroundWithHeader = Foreground;
            ////    }
            ////    else
            ////    {
            ////        m_defaultForeground = Foreground;
            ////    }
            ////}
            ////else if (e.Property == HeaderBackgroundProperty)
            ////{
            ////    m_defaultHeaderBackground = HeaderBackground;
            ////}
            ////else if (e.Property == HeaderForegroundProperty)
            ////{
            ////    m_defaultHeaderForeground = HeaderForeground;
            ////}
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (Click != null)
            {
                Click(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
