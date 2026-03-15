// <copyright file="NotifyIcon.cs" company="Syncfusion">
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
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.Windows.Interop;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Reflection;
using System.IO;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;


namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Specifies a component that creates an icon in the system notification area.
    /// Icons in the notification area are shortcuts to processes that are running in
    /// the background of a computer, such as a virus protection program or a volume
    /// control. These processes do not come with their own user interfaces. The
    /// NotifyIcon class provides a way to program in this functionality.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="NotifyIcon.Window1"
    ///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///     Title="Window1" Height="300" Width="300"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///       <syncfusion:NotifyIcon Name="notifyIcon" BalloonTipText="Custom Notify
    /// Icon is Available"         BalloonTipTitle="Default NotifyIcon"
    /// ShowBalloonTipTime="1000" HideBalloonTipTime="1000">
    ///         </syncfusion:NotifyIcon>
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Imaging;
    /// using System.Windows.Navigation;
    /// using System.Windows.Shapes;
    /// using Syncfusion.Windows.Tools.Controls;
    /// namespace NotifyIcon 
    /// {
    ///     /// <summary>
    ///     /// Interaction logic for Window1.xaml
    ///     /// </summary>
    ///     public partial class Window1 : Window
    ///     {
    ///         public Window1()
    ///         {
    ///             InitializeComponent();
    ///             NotifyIcon notifyIcon = new NotifyIcon();
    ///             notifyIcon.BalloonTipText = "Custom Notify Icon is Available";
    ///             notifyIcon.BalloonTipTitle = "Default NotifyIcon";
    ///             notifyIcon.ShowBalloonTipTime = 1000;
    ///             notifyIcon.HideBalloonTipTime = 1000;
    ///             this.Content = notifyIcon;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif

#if WPF
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
   Type = typeof(NotifyIcon), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/NotifyIcon/Themes/TransparentStyle.xaml")]
#endif

    public class NotifyIcon : HeaderedContentControl
    {
        #region Constants
        /// <summary>
        /// Stores the icon's rectangle padding
        /// </summary>
        private int dEF_ICONRECTPADDING = 1;
        #endregion

        #region Private members
        /// <summary>
        /// Stores the internal icon.
        /// </summary>
        private System.Drawing.Icon m_internalIcon = null;

        /// <summary>
        /// Stores the converter.
        /// </summary>
        private ImageSourceToIconConverter m_imSorceConverter = new ImageSourceToIconConverter();

        /// <summary>
        /// Stores the internal control.
        /// </summary>
        private System.Windows.Forms.NotifyIcon m_ctrlInternal = null;

        /// <summary>
        /// Stores the tooltip.
        /// </summary>
        private ToolTip m_toolTip = null;

        /// <summary>
        /// Stores the balloon.
        /// </summary>
        private BalloonTip m_ballon = null;

        /// <summary>
        /// Stores the show in Taskbar.
        /// </summary>
        private bool m_bShowInTaskBar = true;

        private bool m_bToolTipRectresult = false;
        /*
        ///// <summary>
        ///// Default background.
        ///// </summary>
        //private Brush m_defaultBackground = Brushes.White;
        ///// <summary>
        ///// Default foreground.
        ///// </summary>
        //private Brush m_defaultForeground = SystemColors.ControlTextBrush;
        ///// <summary>
        ///// Default border brush.
        ///// </summary>
        //private Brush m_defaultBorderBrush = Brushes.Black;
        ///// <summary>
        ///// Default background.
        ///// </summary>
        //private Brush m_defaultBackgroundWithHeader = Brushes.White;
        ///// <summary>
        ///// Default foreground.
        ///// </summary>
        //private Brush m_defaultForegroundWithHeader = SystemColors.ControlTextBrush;
        ///// <summary>
        ///// Default border brush.
        ///// </summary>
        //private Brush m_defaultBorderBrushWithHeader = Brushes.Black;
        ///// <summary>
        ///// Default background.
        ///// </summary>
        //private Brush m_defaultHeaderBackground = Brushes.White;
        ///// <summary>
        ///// Default foreground.
        ///// </summary>
        //private Brush m_defaultHeaderForeground = SystemColors.ControlTextBrush;
        */

        /// <summary>
        /// Note: Influenced real rect.
        /// </summary>
        private Rect m_savedIconRect = Rect.Empty;
        #endregion //Private members

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="NotifyIcon"/> class.
        /// </summary>
        static NotifyIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotifyIcon), new FrameworkPropertyMetadata(typeof(NotifyIcon)));
            EnvironmentTest.ValidateLicense(typeof(NotifyIcon));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyIcon"/> class.
        /// </summary>
        public NotifyIcon()
        {

            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(NotifyIcon));
            }
            Dispatcher.ShutdownFinished += new EventHandler(Dispatcher_ShutdownFinished);
        }

        #endregion //Initialization

        #region Properties

        /// <summary>
        /// Gets or sets the task bar direction.
        /// </summary>
        /// <value>The task bar direction.</value>
        internal TaskBarDirection TaskBarDirection
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
        
        /*
        ///// <summary>
        ///// Gets default background. This is a dependency property.
        ///// </summary>
        //internal Brush DefaultBackground
        //{
        //    get
        //    {
        //        return (Brush)GetValue(DefaultBackgroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(DefaultBackgroundProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Gets default foreground. This is a dependency property.
        ///// </summary>
        //internal Brush DefaultForeground
        //{
        //    get
        //    {
        //        return (Brush)GetValue(DefaultForegroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(DefaultForegroundProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Gets default BorderBrush. This is a dependency property.
        ///// </summary>
        //internal Brush DefaultBorderBrush
        //{
        //    get
        //    {
        //        return (Brush)GetValue(DefaultBorderBrushProperty);
        //    }
        //    set
        //    {
        //        SetValue(DefaultBorderBrushProperty, value);
        //    }
        //}
        */

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

        /*
        ///// <summary>
        ///// Gets or sets the default header background of the balloon tip. This is a dependency property.
        ///// </summary>
        //internal Brush DefaultHeaderBackground
        //{
        //    get
        //    {
        //        return (Brush)GetValue(DefaultHeaderBackgroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(DefaultHeaderBackgroundProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the default header foreground of the balloon tip. This is a dependency property.
        ///// </summary>
        //internal Brush DefaultHeaderForeground
        //{
        //    get
        //    {
        //        return (Brush)GetValue(DefaultHeaderForegroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(DefaultHeaderForegroundProperty, value);
        //    }
        //}
        */

        /// <summary>
        /// Gets the internal icon.
        /// </summary>
        /// <value>The internal icon.</value>
        internal System.Drawing.Icon InternalIcon
        {
            get
            {
                return m_internalIcon;
            }
        }

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
        /// Gets or sets the text.
        /// </summary>      
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the icon to be displayed.
        /// </summary>        
        public ImageSource Icon
        {
            get
            {
                return (ImageSource)GetValue(IconProperty);
            }

            set
            {
                SetValue(IconProperty, value);
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
        /// Gets or sets the custom showing animation.
        /// </summary>
        /// <value>The custom showing animation.</value>
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
        /// Gets or sets the tooltip template.
        /// </summary>
        /// <value>The icon template.</value>
        public DataTemplate ToolTipTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ToolTipTemplateProperty);
            }

            set
            {
                SetValue(ToolTipTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ToolTipOpenProperty is true or false.
        /// property.
        /// </summary>
        internal bool IsToolTipOpen
        {
            get
            {
                return (bool)GetValue(IsToolTipOpenProperty);
            }

            set
            {
                SetValue(IsToolTipOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show icon in taskbar.
        /// </summary>
        /// <value><c>true</c> if show in task bar; otherwise, <c>false</c>.</value>
        public bool ShowInTaskBar
        {
            get
            {
                return m_bShowInTaskBar;
            }

            set
            {
                if (m_bShowInTaskBar != value)
                {
                    m_bShowInTaskBar = value;
                    UpdateIconInTaskBar();
                }
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

        #endregion //Properties

        #region dependency properties

        /// <summary>
        /// Identifies the <see cref="TaskBarDirection"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty TaskBarDirectionProperty =
            DependencyProperty.Register("TaskBarDirection", typeof(TaskBarDirection), typeof(NotifyIcon), new FrameworkPropertyMetadata(TaskBarDirection.Bottom));

        /// <summary>
        /// Identifies the DefaultBackground dependency property.
        /// </summary>
        internal static readonly DependencyProperty DefaultBackgroundProperty =
            DependencyProperty.Register("DefaultBackground", typeof(Brush), typeof(NotifyIcon), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the DefaultForeground dependency property.
        /// </summary>
        internal static readonly DependencyProperty DefaultForegroundProperty =
            DependencyProperty.Register("DefaultForeground", typeof(Brush), typeof(NotifyIcon), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the DefaultBorderBrush dependency property.
        /// </summary>
        internal static readonly DependencyProperty DefaultBorderBrushProperty =
            DependencyProperty.Register("DefaultBorderBrush", typeof(Brush), typeof(NotifyIcon), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the <see cref="HeaderBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(NotifyIcon), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the <see cref="HeaderForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(NotifyIcon), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the DefaultHeaderBackground dependency property.
        /// </summary>
        internal static readonly DependencyProperty DefaultHeaderBackgroundProperty =
            DependencyProperty.Register("DefaultHeaderBackground", typeof(Brush), typeof(NotifyIcon), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the DefaultHeaderForeground dependency property.
        /// </summary>
        internal static readonly DependencyProperty DefaultHeaderForegroundProperty =
            DependencyProperty.Register("DefaultHeaderForeground", typeof(Brush), typeof(NotifyIcon), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the <see cref="BalloonTipIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipIconProperty =
            DependencyProperty.Register("BalloonTipIcon", typeof(BalloonTipIcon), typeof(NotifyIcon), new FrameworkPropertyMetadata(BalloonTipIcon.None, new PropertyChangedCallback(OnBalloonTipIconChanged)));

        /// <summary>
        /// Identifies the <see cref="BalloonTipText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipTextProperty =
            DependencyProperty.Register("BalloonTipText", typeof(string), typeof(NotifyIcon), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBalloonTipTextChanged)));

        /// <summary>
        /// Identifies the <see cref="BalloonTipTitle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipTitleProperty =
            DependencyProperty.Register("BalloonTipTitle", typeof(string), typeof(NotifyIcon), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBalloonTipTitleChanged)));

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NotifyIcon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(NotifyIcon), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIconChanged)));

        /// <summary>
        /// Identifies the <see cref="BalloonTipLocation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipLocationProperty =
            DependencyProperty.Register("BalloonTipLocation", typeof(Point), typeof(NotifyIcon), new FrameworkPropertyMetadata(new Point(0.0, 0.0)));

        /// <summary>
        /// Identifies the <see cref="BalloonTipSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipSizeProperty =
            DependencyProperty.Register("BalloonTipSize", typeof(Size), typeof(NotifyIcon), new FrameworkPropertyMetadata(Size.Empty));

        /// <summary>
        /// Identifies the <see cref="BalloonTipShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipShapeProperty =
            DependencyProperty.Register("BalloonTipShape", typeof(BalloonTipShapes), typeof(NotifyIcon), new FrameworkPropertyMetadata(BalloonTipShapes.Balloon));

        /// <summary>
        /// Identifies the <see cref="ShowBalloonTipTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowBalloonTipTimeProperty =
            DependencyProperty.Register("ShowBalloonTipTime", typeof(double), typeof(NotifyIcon), new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="HideBalloonTipTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HideBalloonTipTimeProperty =
            DependencyProperty.Register("HideBalloonTipTime", typeof(double), typeof(NotifyIcon), new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="BalloonTipAnimationEffect"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonTipAnimationEffectProperty =
            DependencyProperty.Register("BalloonTipAnimationEffect", typeof(BalloonTipAnimationEffects), typeof(NotifyIcon), new FrameworkPropertyMetadata(BalloonTipAnimationEffects.Fade));

        /// <summary>
        /// Identifies the <see cref="CustomShowingAnimation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomShowingAnimationProperty =
            DependencyProperty.Register("CustomShowingAnimation", typeof(AnimationTimeline), typeof(NotifyIcon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CustomHidingAnimation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomHidingAnimationProperty =
            DependencyProperty.Register("CustomHidingAnimation", typeof(AnimationTimeline), typeof(NotifyIcon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CustomAnimatedProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomAnimatedPropertyProperty =
            DependencyProperty.Register("CustomAnimatedProperty", typeof(DependencyProperty), typeof(NotifyIcon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HeaderImage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderImageProperty =
            DependencyProperty.Register("HeaderImage", typeof(ImageSource), typeof(NotifyIcon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IconTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(NotifyIcon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ToolTipTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolTipTemplateProperty =
            DependencyProperty.Register("ToolTipTemplate", typeof(DataTemplate), typeof(NotifyIcon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// This property is responsible for the blink popup.
        /// </summary>
        internal static readonly DependencyProperty IsToolTipOpenProperty =
            DependencyProperty.Register("IsToolTipOpen", typeof(bool), typeof(NotifyIcon), new FrameworkPropertyMetadata(false, null, new CoerceValueCallback(CoerceIsToolTipOpen)));

        /// <summary>
        /// This property is responsible for the visibility of balloon tip's header.
        /// </summary>
        public static readonly DependencyProperty BalloonTipHeaderVisibilityProperty =
            DependencyProperty.Register("BalloonTipHeaderVisibility", typeof(Visibility), typeof(NotifyIcon), new FrameworkPropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnBalloonTipHeaderVisibilityChanged)));

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

        /// <summary>
        /// Occurs when icon is clicked.
        /// </summary>
        public event MouseButtonEventHandler IconClick;

        /// <summary>
        /// Occurs when icon is double clicked.
        /// </summary>
        public event MouseButtonEventHandler IconDoubleClick;

        #endregion //Events

        #region Implementation

        /// <summary>
        /// Handles the ShutdownFinished event of the Dispatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Dispatcher_ShutdownFinished(object sender, EventArgs e)
        {
            if (m_ctrlInternal != null)
            {
                m_ctrlInternal.Dispose();
            }
            Dispatcher.ShutdownFinished -= new EventHandler(Dispatcher_ShutdownFinished);
        }

        /// <summary>
        /// Invoked whenever the <see cref="BalloonTipIcon"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnBalloonTipIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NotifyIcon instance = (NotifyIcon)d;
            instance.OnBalloonTipIconChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="BalloonTipIcon"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnBalloonTipIconChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_ballon != null)
            {
                m_ballon.BalloonTipIcon = BalloonTipIcon;
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="BalloonTipText"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnBalloonTipTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NotifyIcon instance = (NotifyIcon)d;
            instance.OnBalloonTipTextChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="BalloonTipText"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnBalloonTipTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_ballon != null)
            {
                m_ballon.BalloonTipText = BalloonTipText;
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="BalloonTipTitle"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnBalloonTipTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NotifyIcon instance = (NotifyIcon)d;
            instance.OnBalloonTipTitleChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="BalloonTipTitle"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnBalloonTipTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_ballon != null)
            {
                m_ballon.BalloonTipTitle = BalloonTipTitle;
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="BalloonTipIcon"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NotifyIcon instance = (NotifyIcon)d;
            instance.OnIconChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="BalloonTipIcon"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnIconChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Icon != null && !(DesignerProperties.GetIsInDesignMode(this)))            
            {
                m_internalIcon = (System.Drawing.Icon)m_imSorceConverter.Convert(Icon, typeof(System.Drawing.Icon), null, CultureInfo.CurrentCulture);
            }
            else
            {
                m_internalIcon = null;
            }

            UpdateIcon();
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
            NotifyIcon instance = (NotifyIcon)d;
            instance.OnBalloonTipHeaderVisibilityChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:BalloonTipHeaderVisibilityChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnBalloonTipHeaderVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            ////    if (BalloonTipHeaderVisibility == Visibility.Visible)
            ////    {
            ////        m_defaultBackgroundWithHeader = m_defaultBackground;
            ////        m_defaultBorderBrushWithHeader = m_defaultBorderBrush;
            ////        m_defaultForegroundWithHeader = m_defaultForeground;
            ////    }
            ////    else
            ////    {
            ////        m_defaultBackground = m_defaultBackgroundWithHeader;
            ////        m_defaultBorderBrush = m_defaultBorderBrushWithHeader;
            ////        m_defaultForeground = m_defaultForegroundWithHeader;
            ////    }
        }

        /// <summary>
        /// Coerces the balloon tip open.
        /// </summary>
        /// <param name="d">Sender object.</param>
        /// <param name="obj">The object.</param>
        /// <returns>Coerced Value</returns>
        private static object CoerceIsToolTipOpen(DependencyObject d, object obj)
        {
            NotifyIcon instance = (NotifyIcon)d;
            return instance.CoerceIsToolTipOpen(obj);
        }

        /// <summary>
        /// Coerces the is tool tip open.
        /// </summary>
        /// <param name="obj">sender object.</param>
        /// <returns>Coerced value</returns>
        private object CoerceIsToolTipOpen(object obj)
        {
            bool isOpen = (bool)obj;

            if (isOpen &&
                (IsMouseOver || (m_ballon != null && m_ballon.IsMouseOver)))
            {
                isOpen = false;
            }

            return isOpen;
        }

        /// <summary>
        /// Updates the icon.
        /// </summary>
        private void UpdateIcon()
        {
            if (m_ctrlInternal != null)
            {
                if (m_internalIcon != null) 
                    m_ctrlInternal.Icon = m_internalIcon;
                UpdateIconInTaskBar();
            }
        }

        /// <summary>
        /// Updates the icon in task bar.
        /// </summary>
        private void UpdateIconInTaskBar()
        {
            if (m_ctrlInternal != null)
            {
                m_ctrlInternal.Visible = m_bShowInTaskBar;
            }
        }

        /// <summary>
        /// Shows the balloon tip.
        /// </summary>
        /// <param name="timeToShow">The time in milliseconds</param>
        public void ShowBalloonTip(int timeToShow)
        {
            if(m_ctrlInternal == null)
                InitializeInternalControl();

            if (m_ballon != null)
            {
                m_ballon.ShowBalloonTip(timeToShow);
            }
            else
            {
                this.ApplyTemplate();
                if (m_ballon != null)
                    m_ballon.ShowBalloonTip(timeToShow);
            }
        }

        /// <summary>
        /// Shows the balloon tip.
        /// </summary>
        /// <param name="timeToShow">The time in milliseconds</param>
        /// <param name="title">The title value.</param>
        /// <param name="text">The text value.</param>
        /// <param name="tipIcon">The tip icon.</param>
        public void ShowBalloonTip(int timeToShow, string title, string text, BalloonTipIcon tipIcon)
        {
            if(m_ctrlInternal == null)
                InitializeInternalControl();

            if (m_ballon != null)
            {
                m_ballon.ShowBalloonTip(timeToShow, title, text, tipIcon);
            }
            else
            {
                this.ApplyTemplate();
                if (m_ballon != null)
                    m_ballon.ShowBalloonTip(timeToShow, title, text, tipIcon);
            }
        }

        /// <summary>
        /// Hides the balloon tip.
        /// </summary>
        public void HideBalloonTip()
        {
            m_ballon.HideBalloonTip();
            Dispose();
        }

        /// <summary>
        /// Hides the balloon tip.
        /// </summary>
        /// <param name="timeToHide">The time in milliseconds</param>
        public void HideBalloonTip(int timeToHide)
        {
            m_ballon.HideBalloonTip(timeToHide);
            Dispose();
        }

        private void Dispose()
        {
            if (m_ctrlInternal != null)
            {
                m_ctrlInternal.Dispose();
                m_ctrlInternal.Click -= OnClick;
                m_ctrlInternal.MouseClick -= OnMouseClick;
                m_ctrlInternal.MouseDoubleClick -= OnMouseDoubleClick;
                m_ctrlInternal.MouseDown -= OnMouseDown;
                m_ctrlInternal.MouseUp -= OnMouseUp;
                m_ctrlInternal.MouseMove -= OnMouseMove;               
            }
            Dispatcher.ShutdownFinished -= new EventHandler(Dispatcher_ShutdownFinished);
        }

        /// <summary>
        /// Initializes the internal control.
        /// </summary>
        private void InitializeInternalControl()
        {
            m_ctrlInternal = new System.Windows.Forms.NotifyIcon();

            m_ctrlInternal.Click += OnClick;
            m_ctrlInternal.MouseClick += OnMouseClick;
            m_ctrlInternal.MouseDoubleClick += OnMouseDoubleClick;
            m_ctrlInternal.MouseDown += OnMouseDown;
            m_ctrlInternal.MouseUp += OnMouseUp;
            m_ctrlInternal.MouseMove += OnMouseMove;

            UpdateIcon();
        }

        /// <summary>
        /// Initializes the tool tip.
        /// </summary>
        private void InitializeToolTip()
        {
            Border border = (Border)GetTemplateChild("PART_Border");
            if (border != null)
            {
                m_toolTip = (ToolTip)border.ToolTip;
                m_toolTip.PlacementTarget = this;
                m_toolTip.Opened += new RoutedEventHandler(ToolTip_Opened);
            }
        }

        /// <summary>
        /// Initializes the balloon tip.
        /// </summary>
        private void InitializeBalloonTip()
        {
            m_ballon = (BalloonTip)GetTemplateChild("PART_BalloonTip");

            if (m_ballon != null)
            {
                m_ballon.Click += new EventHandler(Ballon_Click);
                m_ballon.BalloonTipOpening += new CancelEventHandler(Ballon_BalloonTipOpening);
                m_ballon.BalloonTipOpened += new PropertyChangedCallback(Ballon_BalloonTipOpened);
                m_ballon.BalloonTipHiding += new CancelEventHandler(Ballon_BalloonTipHiding);
                m_ballon.BalloonTipHidden += new PropertyChangedCallback(Ballon_BalloonTipHidden);
                m_ballon.CloseButtonClick += new EventHandler(Ballon_CloseButtonClick);
            }
        }
        
        /// <summary>
        /// Handles the Opened event of the ToolTip control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            if (m_savedIconRect == Rect.Empty || !IsToolTipOpen)
            {
                m_toolTip.IsOpen = false;
                return;
            }

            Size tooltipSize = GetToolTipSize();

            Rect placementRect = m_savedIconRect;
            placementRect.Inflate(1, 1);

            placementRect.Size = tooltipSize;

            System.Drawing.Point p = System.Windows.Forms.Cursor.Position;
            Point mousePoint = new Point(p.X, p.Y);

            placementRect.X = mousePoint.X;
            placementRect.Y -= tooltipSize.Height;

            m_toolTip.PlacementRectangle = placementRect;
            m_toolTip.PlacementTarget = this;

          
        }

        /// <summary>
        /// Handles the Click event of the Balloon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Ballon_Click(object sender, EventArgs e)
        {
            OnClick(sender, e);
        }
        
        /// <summary>
        /// Handles the BalloonTipOpening event of the Balloon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Ballon_BalloonTipOpening(object sender, CancelEventArgs e)
        {
            if (BalloonTipOpening != null)
            {
                BalloonTipOpening(this, e);
            }
        }

        /// <summary>
        /// Ballon_s the balloon tip opened.
        /// </summary>
        /// <param name="d">Sender object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Ballon_BalloonTipOpened(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (m_ballon != null && m_ballon.IsOpen)
            {
                if (ContextMenu != null)
                {
                    ContextMenu.IsOpen = false;
                }

                IsToolTipOpen = false;
            }

            if (BalloonTipOpened != null)
            {
                BalloonTipOpened(this, e);
            }
        }
        
        /// <summary>
        /// Handles the BalloonTipHiding event of the Balloon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Ballon_BalloonTipHiding(object sender, CancelEventArgs e)
        {
            if (BalloonTipHiding != null)
            {
                BalloonTipHiding(this, e);
            }
        }
        
        /// <summary>
        /// Ballon_s the balloon tip hidden.
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Ballon_BalloonTipHidden(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (BalloonTipHidden != null)
            {
                BalloonTipHidden(this, e);
            }
        }
        
        /// <summary>
        /// Handles the CloseButtonClick event of the Balloon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Ballon_CloseButtonClick(object sender, EventArgs e)
        {
            if (CloseButtonClick != null)
            {
                CloseButtonClick(this, e);
            }
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnClick(object sender, EventArgs e)
        {
            if (Click != null)
            {
                Click(this, e);
            }
        }
        
        /// <summary>
        /// Called when [raise event].
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnRaiseEvent(RoutedEvent handler, MouseEventArgs e)
        {
            e.RoutedEvent = handler;
            RaiseEvent(e);
        }
        
        /// <summary>
        /// Called when [raise event].
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnRaiseEvent(RoutedEvent handler, MouseButtonEventArgs e)
        {
            e.RoutedEvent = handler;
            RaiseEvent(e);
        }

        /// <summary>
        /// Called when [mouse click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (IconClick != null)
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button));

                IconClick(sender, args);
            }
        }
        
        /// <summary>
        /// Called when [mouse double click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseButtonEventArgs args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button));

            if (IconDoubleClick != null)
            {
                IconDoubleClick(sender, args);
            }

            OnRaiseEvent(MouseDoubleClickEvent, args);
        }
        
        /// <summary>
        /// Called when [mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(Text) &&
                (ContextMenu == null || (ContextMenu != null && !ContextMenu.IsOpen)))
            {
                System.Drawing.Point p = System.Windows.Forms.Cursor.Position;
                Point mousePoint = new Point(p.X, p.Y);

                if (!IsToolTipOpen)
                {
                    if (m_savedIconRect == Rect.Empty)
                    {
                        m_bToolTipRectresult = true;
                        m_savedIconRect = FindIconRectInTray();
                        m_bToolTipRectresult = false;
                        m_savedIconRect.Inflate(-1, -1);
                    }

                    if (m_savedIconRect.Contains(mousePoint))
                    {
                        IsToolTipOpen = true;
                    }
                }
                else
                {
                    if (m_savedIconRect != Rect.Empty &&
                        !m_savedIconRect.Contains(mousePoint))
                    {
                        IsToolTipOpen = false;
                        m_savedIconRect = Rect.Empty;
                    }
                }
            }

            OnRaiseEvent(MouseMoveEvent, new MouseEventArgs(InputManager.Current.PrimaryMouseDevice, 0));
        }

        /// <summary>
        /// Called when [mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnRaiseEvent(MouseDownEvent, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
        }
        
        /// <summary>
        /// Called when [mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ShowContextMenu();
            }

            OnRaiseEvent(MouseUpEvent, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
        }

        /// <summary>
        /// Shows the context menu.
        /// </summary>
        private void ShowContextMenu()
        {
            IsToolTipOpen = false;

            if (ContextMenu != null)
            {
                ContextMenu.StaysOpen = false;
                ContextMenu.PlacementTarget = this;
                ContextMenu.IsOpen = true;

              
            }
        }

        /// <summary>
        /// Toes the mouse button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>MouseButton object</returns>
        private MouseButton ToMouseButton(System.Windows.Forms.MouseButtons button)
        {
            switch (button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    return MouseButton.Left;
                case System.Windows.Forms.MouseButtons.Right:
                    return MouseButton.Right;
                case System.Windows.Forms.MouseButtons.Middle:
                    return MouseButton.Middle;
                case System.Windows.Forms.MouseButtons.XButton1:
                    return MouseButton.XButton1;
                case System.Windows.Forms.MouseButtons.XButton2:
                    return MouseButton.XButton2;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the task bar direction
        /// </summary>
        /// <returns>Returns taskbarDirection</returns>
        internal static TaskBarDirection GetTaskBarDirestion()
        {
            System.Drawing.Rectangle screenRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            TaskBarDirection direction = TaskBarDirection.Bottom;

            RECT rect;
            Point point = new Point(0.0, 0.0);
            IntPtr taskbar = NativeMethods.FindWindow("Shell_TrayWnd", String.Empty);
            if (NativeMethods.GetWindowRect(taskbar, out rect))
            {
                foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    if (screen.Bounds.Contains(rect.left, rect.top))
                    {
                        screenRect = screen.Bounds;
                        break;
                    }
                }
                if (rect.right < screenRect.Right)
                {
                    direction = TaskBarDirection.Left;
                }
                else if (rect.left > screenRect.Left)
                {
                    direction = TaskBarDirection.Right;
                }
                else if (rect.top > screenRect.Top)
                {
                    direction = TaskBarDirection.Bottom;
                }
                else if(rect.bottom<screenRect.Bottom)
                {
                    direction = TaskBarDirection.Top;
                }
            }

            return direction;
        }

        /// <summary>
        /// Finds the icon rect in tray.
        /// </summary>
        /// <returns>Returns rectangle</returns>
        internal Rect FindIconRectInTray()
        {
            Rect rezult = Rect.Empty;

            IntPtr hTray = NativeMethods.FindWindow("Shell_TrayWnd", String.Empty);
            IntPtr hNotify = NativeMethods.FindWindowEx(hTray, IntPtr.Zero, "TrayNotifyWnd", IntPtr.Zero);
            IntPtr hPager = NativeMethods.FindWindowEx(hNotify, IntPtr.Zero, "SysPager", IntPtr.Zero);
            IntPtr hToolbar = NativeMethods.FindWindowEx(hPager, IntPtr.Zero, "ToolbarWindow32", IntPtr.Zero);

            RECT rect;
            Point point = new Point(0.0, 0.0);
            Size iconSize = new Size(16.0, 16.0);

            bool bFounded = false;
            int collapseToolBarButtonWidth = 6;  // Half width of the button.
            TaskBarDirection = NotifyIcon.GetTaskBarDirestion();
            if (NativeMethods.GetWindowRect(hToolbar, out rect))
            {
                if (TaskBarDirection == TaskBarDirection.Top || TaskBarDirection == TaskBarDirection.Bottom)
                {
                    if (!m_bToolTipRectresult)
                    {
                        rezult = new Rect(new Point(rect.left - (collapseToolBarButtonWidth * 2), rect.top + (rect.Size.Height / 2) - collapseToolBarButtonWidth), iconSize);
                    }
                    else
                    {
                        rezult = new Rect(new Point(rect.left + (collapseToolBarButtonWidth), rect.top + ((rect.Size.Height / 6) * 2) - collapseToolBarButtonWidth), iconSize);
                    }
                }
                else
                {
                    rezult = new Rect(new Point(rect.left + (rect.Size.Width / 2) - collapseToolBarButtonWidth, rect.top - (collapseToolBarButtonWidth * 2)), iconSize);
                }

                if (m_bShowInTaskBar &&
                    m_ctrlInternal != null && m_ctrlInternal.Visible && (m_ctrlInternal.Icon != null))
                {
                    System.Drawing.Icon blackIcon = null;

                    Assembly ass = GetType().Module.Assembly;
                    //Stream stream = ass.GetManifestResourceStream("Syncfusion.Tools.WPF.Controls.NotifyIcon.Resources.BlackIcon.ico");
                    Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Tools.Controls.Controls.NotifyIcon.Resources.BlackIcon.ico");

                    if (stream != null)
                    {
                        blackIcon = new System.Drawing.Icon(stream);
                    }

                    if (blackIcon == null)
                    {
                        throw new ArgumentException("Black Icon is not valid!");
                    }

                    //m_ctrlInternal.Icon = blackIcon;

                    System.Drawing.Bitmap bm = DrawingUtils.GetScreenShot(hToolbar, rect.Size);
                    int pointX = dEF_ICONRECTPADDING;
                    int pointY = 0;
                    int numLines = 0;
                    int size = 0;
                    for (int y = 0; y < bm.Size.Height; y += 2)
                    {
                        int numOfPixelsInLine = 0;

                        for (int x = pointX; x < bm.Size.Width - dEF_ICONRECTPADDING; x++)
                        {
                            int crPixel = bm.GetPixel(x, y).ToArgb();
                            int crPixel2 = bm.GetPixel(x - dEF_ICONRECTPADDING, y).ToArgb();
                            int crPixel3 = bm.GetPixel(x + dEF_ICONRECTPADDING, y).ToArgb();

                            if (CheckIfColorIsBlack(crPixel) &&
                                CheckIfColorIsBlack(crPixel2) &&
                                CheckIfColorIsBlack(crPixel3))
                            {
                                if (pointX == dEF_ICONRECTPADDING && pointY == 0)
                                {
                                    pointX = x;
                                    pointY = y;
                                }

                                numOfPixelsInLine++;

                                if (x == bm.Size.Width - dEF_ICONRECTPADDING - 1)
                                {
                                    size = numOfPixelsInLine + dEF_ICONRECTPADDING;
                                    numLines++;
                                    break;
                                }
                            }
                            else
                            {
                                if (numOfPixelsInLine > 8)
                                {
                                    size = numOfPixelsInLine + (dEF_ICONRECTPADDING * 2);
                                    numLines++;
                                    break;
                                }
                                else
                                {
                                    size = 0;
                                    pointX = dEF_ICONRECTPADDING;
                                    pointY = 0;
                                    numLines = 0;
                                }

                                numOfPixelsInLine = 0;
                            }
                        }

                        if (numLines > 4)
                        {
                            bFounded = true;
                            break;
                        }
                    }

                    if (bFounded)
                    {
                        iconSize = new Size(size, size);
                        rezult = new Rect(new Point(rect.left + pointX - dEF_ICONRECTPADDING, rect.top + point.Y), iconSize);
                    }

                    bm.Dispose();

                    UpdateIcon();
                }
            }

            return rezult;
        }

        /// <summary>
        /// Checks if color is black.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>Returns Boolean</returns>
        private bool CheckIfColorIsBlack(int color)
        {
            return color == 00000000; // #ff000000.
        }

        /// <summary>
        /// Gets the size of the tool tip.
        /// </summary>
        /// <returns>Returns Size</returns>
        private Size GetToolTipSize()
        {
            Size size = Size.Empty;

            if (m_toolTip != null)
            {
                m_toolTip.Measure(new Size(double.MaxValue, double.MaxValue));
                size = m_toolTip.DesiredSize;
            }

            return size;
        }

        #endregion //Implementation

        #region Overrides

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (m_ctrlInternal == null)
                InitializeInternalControl();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            InitializeToolTip();
            InitializeBalloonTip();
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.FrameworkElement.ContextMenuOpening"/> routed event reaches this class in its route. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);

            e.Handled = true;
        }

        /*
        ///// <summary>
        ///// Called when some dependencyProperty is changed.
        ///// </summary>
        ///// <param name="e">Event args.</param>
        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    base.OnPropertyChanged(e);

        //    if (e.Property == SkinStorage.VisualStyleProperty &&
        //        (string)e.NewValue == "Default")
        //    {
        //        if (BalloonTipHeaderVisibility == Visibility.Visible)
        //        {
        //            Background = m_defaultBackgroundWithHeader;
        //            Foreground = m_defaultForegroundWithHeader;
        //            BorderBrush = m_defaultBorderBrushWithHeader;
        //        }
        //        else
        //        {
        //            Background = m_defaultBackground;
        //            Foreground = m_defaultForeground;
        //            BorderBrush = m_defaultBorderBrush;
        //        }

        //        HeaderBackground = m_defaultHeaderBackground;
        //        HeaderForeground = m_defaultHeaderForeground;
        //    }
        //    else if (e.Property == BackgroundProperty)
        //    {
        //        if (BalloonTipHeaderVisibility == Visibility.Visible)
        //            m_defaultBackgroundWithHeader = Background;
        //        else
        //            m_defaultBackground = Background;

        //        if (m_ballon != null)
        //        {
        //            m_ballon.Background = Background;
        //        }
        //    }
        //    else if (e.Property == BorderBrushProperty)
        //    {
        //        if (BalloonTipHeaderVisibility == Visibility.Visible)
        //            m_defaultBorderBrushWithHeader = Background;
        //        else
        //            m_defaultBorderBrush = BorderBrush;

        //        if (m_ballon != null)
        //        {
        //            m_ballon.BorderBrush = BorderBrush;
        //        }
        //    }
        //    else if (e.Property == ForegroundProperty)
        //    {
        //        if (BalloonTipHeaderVisibility == Visibility.Visible)
        //            m_defaultForegroundWithHeader = Foreground;
        //        else
        //            m_defaultForeground = Foreground;

        //        if (m_ballon != null)
        //        {
        //            m_ballon.Foreground = Foreground;
        //        }
        //    }
        //    else if (e.Property == HeaderBackgroundProperty)
        //    {
        //        m_defaultHeaderBackground = HeaderBackground;

        //        if (m_ballon != null)
        //        {
        //            m_ballon.HeaderBackground = HeaderBackground;
        //        }
        //    }
        //    else if (e.Property == HeaderForegroundProperty)
        //    {
        //        m_defaultHeaderForeground = HeaderForeground;

        //        if (m_ballon != null)
        //        {
        //            m_ballon.HeaderForeground = HeaderForeground;
        //        }
        //    }
        //}
        */
        #endregion //Overrides
    }
}
