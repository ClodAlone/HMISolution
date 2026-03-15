// <copyright file="TaskBarItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents <see cref="TaskBar"/> item control.
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public partial class TaskBarItem : HeaderedItemsControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<local:TaskBarItem Name="taskBarItem" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// UI framework element based on <see cref="HeaderedItemsControl"/> class. Control is used for grouping of given content objects.
    /// Used as wrapper in <see cref="TaskBar"/>.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a TaskBarItem in XAML.
    /// <code>
    /// <![CDATA[
    /// <Window x:Class="TaskBar.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
    /// Title="TaskBar" Height="300" Width="300">
    /// <StackPanel HorizontalAlignment="Center">
    /// <local:TaskBar Name="taskBar">
    /// <local:TaskBarItem Name="taskBarItem"/>
    /// </local:TaskBar>
    /// </StackPanel>
    /// </Window>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a TaskBarItem in C#.
    /// <code>
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// namespace Sample1
    /// {
    /// public partial class Window1 : Window
    /// {
    /// public Window1()
    /// {
    /// InitializeComponent();
    /// TaskBar taskBar = new TaskBar();
    /// stackPanel.Children.Add( taskBar );
    /// TaskBarItem item = new TaskBarItem();
    /// taskBar.Items.Add( item );
    /// }
    /// }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
 Type = typeof(TaskBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/TransparentStyle.xaml")]
    public class TaskBarItem : HeaderedItemsControl
    {
        /// <summary>
        /// internal variable which has expander ext
        /// </summary>
        private ExpanderExt m_expander;

        internal Window OldWindow;

        /// <summary>
        /// internal variable which has taskbar height
        /// </summary>
        private double taskBarItemHeight;

        /// <summary>
        /// internal variable which has limit
        /// </summary>
        private double limit;

        /// <summary>
        /// internal variable which has expander ext
        /// </summary>
        private Syncfusion.Windows.Tools.Controls.ExpanderExt SyncfusionExpander = null;

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TaskBarItem"/> class.
        /// </summary>
        static TaskBarItem()
        {
            EnvironmentTest.ValidateLicense(typeof(TaskBarItem));
            EventManager.RegisterClassHandler(typeof(TaskBarItem), ExpanderExt.CollapsedEvent, new RoutedEventHandler(OnExpanderCollapsed));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskBarItem), new FrameworkPropertyMetadata(typeof(TaskBarItem)));
            FocusableProperty.OverrideMetadata(typeof(TaskBarItem), new FrameworkPropertyMetadata(false));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBarItem"/> class.
        /// </summary>
        public TaskBarItem()
        {
            BorderThickness = new Thickness(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBarItem"/> class.
        /// </summary>
        /// <param name="item">The item task bar item.</param>
        public TaskBarItem(object item)
        {
            if (null != item)
            {
                FrameworkElement element = (FrameworkElement)item;
                this.Header = GetAttachHeader(element);
                this.Items.Add(element);
            }
        }

        #endregion Initialization

        #region Public methods

        /// <summary>
        /// Method gets the value of AttachHeader attached property from given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <returns> object type </returns>
        public static Object GetAttachHeader(DependencyObject obj)
        {
            return (Object)obj.GetValue(AttachHeaderProperty);
        }

        /// <summary>
        /// Sets the attach header.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value.</param>
        public static void SetAttachHeader(DependencyObject obj, Object value)
        {
            obj.SetValue(AttachHeaderProperty, value);
        }

        /// <summary>
        /// Wrapped the remove logical child.
        /// </summary>
        /// <param name="item">The item of wrapped logical child.</param>
        public void WrappedRemoveLogicalChild(Object item)
        {
            RemoveLogicalChild(item);
        }

        #endregion Public methods

        #region Events

        /// <summary>
        /// Event that is raised when AttachHeader property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback AttachHeaderChanged;

        #endregion Events

        #region Implementation

        /// <summary>
        /// Updates property value cache and raises <see cref="AttachHeaderChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnAttachHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AttachHeaderChanged != null)
            {
                AttachHeaderChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAttachHeaderChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnAttachHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBarItem instance = d as TaskBarItem;

            if (null != instance)
            {
                instance.OnAttachHeaderChanged(e);
            }
        }

        /// <summary>
        /// Handles the ExpanderExt.Collase event. Executes when expander of the TaskBar is getting collapsed.
        /// </summary>
        /// <param name="sender">An object, the event occurs on.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private static void OnExpanderCollapsed(object sender, RoutedEventArgs e)
        {
            e.Handled = true; // prevent ExpanderExt.Collapsed event from bubbling.
        }

        #endregion Implementation

        #region Properties

        /// <summary>
        /// Gets or sets the header background.
        /// </summary>
        /// <value>The header background.</value>
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
        /// Gets or sets a value indicating whether [allow collapse].
        /// </summary>
        /// <value><c>true</c> if [allow collapse]; otherwise, <c>false</c>.</value>
        public bool AllowCollapse
        {
            get
            {
                return (bool)GetValue(AllowCollapseProperty);
            }

            set
            {
                SetValue(AllowCollapseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow expand].
        /// </summary>
        /// <value><c>true</c> if [allow expand]; otherwise, <c>false</c>.</value>
        public bool AllowExpand
        {
            get
            {
                return (bool)GetValue(AllowExpandProperty);
            }

            set
            {
                SetValue(AllowExpandProperty, value);
            }
        }

        #endregion Properties

        #region Dependency Properties

        /// <summary>
        /// Identifies AttachHeader dependency attached property.
        /// </summary>
        public static readonly DependencyProperty AttachHeaderProperty = DependencyProperty.RegisterAttached("AttachHeader", typeof(Object), typeof(TaskBarItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnAttachHeaderChanged));

        /// <summary>
        /// Identifies the HeaderBackground of the Taskbar item
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(TaskBarItem), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the AllowCollapseProperty of the Taskbar item
        /// </summary>
        public static readonly DependencyProperty AllowCollapseProperty = DependencyProperty.Register("AllowCollapse", typeof(bool), typeof(TaskBarItem), new FrameworkPropertyMetadata(true, null));

        /// <summary>
        /// Identifies the AllowExpandProperty of the Taskbar item
        /// </summary>
        public static readonly DependencyProperty AllowExpandProperty = DependencyProperty.Register("AllowExpand", typeof(bool), typeof(TaskBarItem), new FrameworkPropertyMetadata(true, null));

        #endregion Dependency Properties

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (OldWindow != null)
            {
                OldWindow.SizeChanged -= new SizeChangedEventHandler(win_SizeChanged);
            }

            Window win = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
            OldWindow = win;
            if (win != null)
            {
                win.SizeChanged += new SizeChangedEventHandler(win_SizeChanged);
            }

            m_expander = this.GetTemplateChild("ExpandTemplate") as ExpanderExt;
            limit = TaskBar.GetButtonSize(this) + 8;

            taskBarItemHeight = this.Height;

            if (m_expander != null)
            {
                m_expander.Collapsed += new RoutedEventHandler(m_expander_Collapsed);
                m_expander.Expanded += new RoutedEventHandler(m_expander_Expanded);
            }
            this.Unloaded -= new RoutedEventHandler(TaskBarItem_Unloaded);
            this.Unloaded += new RoutedEventHandler(TaskBarItem_Unloaded);
        }

        private void TaskBarItem_Unloaded(object sender, RoutedEventArgs e)
        {
            Window win = OldWindow;
            if (win != null)
            {
                win.SizeChanged -= new SizeChangedEventHandler(win_SizeChanged);
            }
            if (m_expander != null)
            {
                m_expander.Collapsed -= new RoutedEventHandler(m_expander_Collapsed);
                m_expander.Expanded -= new RoutedEventHandler(m_expander_Expanded);
            }
            this.Unloaded -= new RoutedEventHandler(TaskBarItem_Unloaded);
        }

        /// <summary>
        /// Used to hide Header of TaskBarItem
        /// </summary>
        public void HideHeader()
        {
            ToggleButtonExt toggle = m_expander.Template.FindName("HeaderSite1", m_expander) as ToggleButtonExt;
            if (toggle != null)
            {
                toggle.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Used to Show Header of TaskBarItem
        /// </summary>
        public void ShowHeader()
        {
            ToggleButtonExt toggle = m_expander.Template.FindName("HeaderSite1", m_expander) as ToggleButtonExt;
            if (toggle != null)
            {
                toggle.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the Window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void win_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TaskBarStackPanel panel = VisualUtils.FindDescendant(this, typeof(TaskBarStackPanel)) as TaskBarStackPanel;
            if (panel != null)
            {
                panel.SizeChanged();
            }
        }

        /// <summary>
        /// Handles the Expanded event of the m_expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void m_expander_Expanded(object sender, RoutedEventArgs e)
        {
            SyncfusionExpander = e.OriginalSource as Syncfusion.Windows.Tools.Controls.ExpanderExt;

            if (!(double.IsNaN(taskBarItemHeight)))
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.From = limit;
                doubleAnimation.To = taskBarItemHeight;
                doubleAnimation.Duration = new TimeSpan(0, 0, 0, 0, 100 + (int)Math.Ceiling(TaskBar.GetSpeed(this)));

                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(TaskBarItem.HeightProperty));

                Storyboard storyBoard = new Storyboard();
                storyBoard.Children.Add(doubleAnimation);

                storyBoard.Begin(this);
            }

            //Allows or disallows the opening of task bar item
            if (!this.AllowExpand)
            {
                m_expander.IsExpanded = false;
            }
            else
            {
                m_expander.IsExpanded = true;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Handles the Collapsed event of the m_expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void m_expander_Collapsed(object sender, RoutedEventArgs e)
        {
            SyncfusionExpander = e.OriginalSource as Syncfusion.Windows.Tools.Controls.ExpanderExt;

            if (SyncfusionExpander != null)
            {
                if (!(double.IsNaN(taskBarItemHeight)))
                {
                    DoubleAnimation doubleAnimation = new DoubleAnimation();
                    doubleAnimation.From = taskBarItemHeight;
                    doubleAnimation.To = limit;
                    doubleAnimation.Duration = new TimeSpan(0, 0, 0, 0, 100 + (int)Math.Ceiling(TaskBar.GetSpeed(this)));

                    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(TaskBarItem.HeightProperty));

                    Storyboard storyBoard = new Storyboard();
                    storyBoard.Children.Add(doubleAnimation);

                    storyBoard.Begin(this);
                }

                //Allows or disallows the closing of task bar item
                if (!this.AllowCollapse)
                {
                    m_expander.IsExpanded = true;
                }
                else
                {
                    m_expander.IsExpanded = false;
                }
            }
        }

        #endregion Overrides
    }
}