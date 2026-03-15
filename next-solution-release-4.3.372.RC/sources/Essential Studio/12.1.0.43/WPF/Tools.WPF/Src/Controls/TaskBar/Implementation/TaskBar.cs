// <copyright file="TaskBar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a TaskBar control.
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
    /// <example><code>public partial class TaskBar : ItemsControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<local:TaskBar Name="taskBar" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// UI framework element based on <see cref="ItemsControl"/> class.
    /// Control is used for grouping content objects into special items called <see cref="TaskBarItem"/>.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a TaskBar in XAML.
    /// <code>
    /// <![CDATA[
    /// <Window x:Class="TaskBar.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
    /// Title="TaskBar" Height="300" Width="300">
    /// <StackPanel HorizontalAlignment="Center">
    ///     <local:TaskBar Name="taskBar" />
    /// </StackPanel>
    /// </Window>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a TaskBar in C#.
    /// <code>
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// namespace Sample1
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///        public Window1()
    ///        {
    ///             InitializeComponent();
    ///             TaskBar TaskBar = new TaskBar();
    ///             stackPanel.Children.Add( TaskBar );
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(true)]
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
    public class TaskBar : ItemsControl
    {
        #region Constants

        /// <summary>
        /// This is default style for all skins.
        /// </summary>
        private const string DefaultStyleName = "Default";

        #endregion Constants

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="VisualStyle"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback VisualStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupOrientation"/> property
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback GroupOrientationChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupMargin"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback GroupMarginChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupWidth"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback GroupWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.SpeedProperty"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SpeedChanged;

        /// <summary>
        /// Event that is raised when <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.GroupPaddingProperty"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback GroupPaddingChanged;

        /// <summary>
        /// Event that is raised when <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.ButtonSizeProperty"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback ButtonSizeChanged;

        /// <summary>
        /// Event that is raised when <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.HeaderStyleProperty"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback HeaderStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.IsOpenedProperty"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsOpenedChanged;

        /// <summary>
        /// Identifies <see cref="SelectedItemChangedEvent"/> routed event for TaskBar.
        /// </summary>
        public static readonly RoutedEvent SelectedItemChangedEvent = EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Bubble, typeof(EventHandler), typeof(TaskBar));

        /// <summary>
        /// Occurs when [selected item changed].
        /// </summary>
        public event RoutedEventHandler SelectedItemChanged
        {
            add
            {
                AddHandler(SelectedItemChangedEvent, value);
            }

            remove
            {
                RemoveHandler(SelectedItemChangedEvent, value);
            }
        }

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [last child fill].
        /// </summary>
        /// <value><c>true</c> if [last child fill]; otherwise, <c>false</c>.</value>
        public bool LastChildFill
        {
            get
            {
                return (bool)GetValue(LastChildFillProperty);
            }

            set
            {
                SetValue(LastChildFillProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents orientation of items arrangement.
        /// </summary>
        /// <value>
        /// Type: <see cref="Orientation"/>
        /// <para/>
        /// Value that represents orientation of items arrangement. Default value is <see cref="Orientation.Vertical"/>.
        /// </value>
        public Orientation GroupOrientation
        {
            get
            {
                return (Orientation)GetValue(GroupOrientationProperty);
            }

            set
            {
                SetValue(GroupOrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents margins for all items.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// <para/>
        /// A <see cref="Thickness"/> value that represents margins for all items.
        /// </value>
        public Thickness GroupMargin
        {
            get
            {
                return (Thickness)GetValue(GroupMarginProperty);
            }

            set
            {
                SetValue(GroupMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that limits width of <see cref="TaskBarItem"/> item, for all present <see cref="TaskBarItem"/>
        /// items in <see cref="TaskBar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// A <see cref="Double"/> value that limits width of <see cref="TaskBarItem"/> item,
        /// for all present <see cref="TaskBarItem"/>
        /// items in <see cref="TaskBar"/>. Default value is <see cref="Double.NaN"/>.
        /// </value>
        public double GroupWidth
        {
            get
            {
                return (double)GetValue(GroupWidthProperty);
            }

            set
            {
                SetValue(GroupWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents selected item of the <see cref="TaskBar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="TaskBarItem"/>
        /// <para/>
        /// A <see cref="TaskBarItem"/> object that represents selected item of the <see cref="TaskBar"/>. Default value is null.
        /// </value>
        public TaskBarItem SelectedItem
        {
            get
            {
                return (TaskBarItem)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TaskBar"/> class.
        /// </summary>
        static TaskBar()
        {
            //  EnvironmentTest.ValidateLicense(typeof(TaskBar));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskBar), new FrameworkPropertyMetadata(typeof(TaskBar)));
            FocusableProperty.OverrideMetadata(typeof(TaskBar), new FrameworkPropertyMetadata(false));
        }

        /// <summary>
        ///
        /// </summary>
        public TaskBar()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(TaskBar));
            }
            ResourceDictionary a = new ResourceDictionary();
            Style ToggleButtonStyle;
            a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/generic.xaml", UriKind.Relative);
            ToggleButtonStyle = (Style)a["DefaultToggleButtonStyle"];
            this.SetValue(HeaderStyleProperty, ToggleButtonStyle);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary a = new ResourceDictionary();
            Style ToggleButtonStyle;
            a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/generic.xaml", UriKind.Relative);
            ToggleButtonStyle = (Style)a["DefaultToggleButtonStyle"];
            NewMethod(ref e, a, ref ToggleButtonStyle);

            if (e.Property == SkinStorage.VisualStyleProperty)
                this.ClearValue(BackgroundProperty);

            base.OnPropertyChanged(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Unloaded -= new RoutedEventHandler(TaskBar_Unloaded);
            this.Unloaded += new RoutedEventHandler(TaskBar_Unloaded);
        }

        private void TaskBar_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= new RoutedEventHandler(TaskBar_Unloaded);
        }

        private void NewMethod(ref DependencyPropertyChangedEventArgs e, ResourceDictionary a, ref Style ToggleButtonStyle)
        {
            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                if (e.NewValue.ToString() == "Blend")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/BlendStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["BlendToggleButtonStyle"];
                }
                else if (e.NewValue.ToString() == "Office2007Blue")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2007BlueStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["Office2007BlueToggleButtonStyle"];
                }
                else if (e.NewValue.ToString() == "Office2007Black")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2007BlackStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["Office2007BlackToggleButtonStyle"];
                }
                else if (e.NewValue.ToString() == "Office2007Silver")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2007SilverStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["Office2007SilverToggleButtonStyle"];
                }

                else if (e.NewValue.ToString() == "SyncOrange")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/syncOrangeStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["SynOrangeToggleButtonStyle"];
                }

                else if (e.NewValue.ToString() == "ShinyRed")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/ShinyRedStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["ShinyRedToggleButtonStyle"];
                }

                else if (e.NewValue.ToString() == "ShinyBlue")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/ShinyBlueStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["ShinyBlueToggleButtonStyle"];
                }
                else if (e.NewValue.ToString() == "VS2010")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/VS2010Style.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["VS2010ToggleButtonStyle"];
                }
                else if (e.NewValue.ToString() == "Office2010Blue")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2010BlueStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["Office2010BlueToggleButtonStyle"];
                }
                else if (e.NewValue.ToString() == "Office2010Black")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2010BlackStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["Office2010BlackToggleButtonStyle"];
                }
                else if (e.NewValue.ToString() == "Office2010Silver")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/Office2010SilverStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["Office2010SilverToggleButtonStyle"];
                }
                else if (e.NewValue.ToString() == "Metro")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/MetroStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["MetroToggleButtonStyle"];
                }

                else if (e.NewValue.ToString() == "Transparent")
                {
                    a.Source = new Uri(@"/Syncfusion.Tools.Wpf;component/Controls/TaskBar/Themes/TransparentStyle.xaml", UriKind.Relative);
                    ToggleButtonStyle = (Style)a["TransparentToggleButtonStyle"];
                }
                this.SetValue(HeaderStyleProperty, ToggleButtonStyle);
            }
        }

        #endregion Initialization

        #region Public Methods

        /// <summary>
        /// Raises <see cref="SelectedItemChangedEvent"/> routed event.
        /// </summary>
        internal void FireSelectedItemChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectedItemChangedEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Method gets the value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.SpeedProperty"/> attached property from given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <returns>
        /// Value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.SpeedProperty"/> attached property from given object.
        /// </returns>
        public static double GetSpeed(DependencyObject obj)
        {
            return (double)obj.GetValue(SpeedProperty);
        }

        /// <summary>
        /// Method sets the value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.SpeedProperty"/> property to given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <param name="value">New value.</param>
        public static void SetSpeed(DependencyObject obj, double value)
        {
            obj.SetValue(SpeedProperty, value);
        }

        /// <summary>
        /// Method gets the value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.GroupPaddingProperty"/> attached property from given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <returns>
        /// Value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.GroupPaddingProperty"/> attached property from given object.
        /// </returns>
        public static Thickness GetGroupPadding(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(GroupPaddingProperty);
        }

        /// <summary>
        /// Method sets the value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.GroupPaddingProperty"/> property to given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <param name="value">New value.</param>
        public static void SetGroupPadding(DependencyObject obj, Thickness value)
        {
            obj.SetValue(GroupPaddingProperty, value);
        }

        /// <summary>
        /// Method gets the value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.ButtonSizeProperty"/> attached property from given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <returns>
        /// Value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.ButtonSizeProperty"/> attached property from given object.
        /// </returns>
        public static double GetButtonSize(DependencyObject obj)
        {
            return (double)obj.GetValue(ButtonSizeProperty);
        }

        /// <summary>
        /// Method sets the value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.ButtonSizeProperty"/> property to given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <param name="value">New value.</param>
        public static void SetButtonSize(DependencyObject obj, double value)
        {
            obj.SetValue(ButtonSizeProperty, value);
        }

        /// <summary>
        /// Method gets the value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.HeaderStyleProperty"/> attached property from given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <returns>
        /// Value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.HeaderStyleProperty"/> attached property from given object.
        /// </returns>
        public static Style GetHeaderStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(HeaderStyleProperty);
        }

        /// <summary>
        /// Method sets the value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.HeaderStyleProperty"/> property to given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <param name="value">New value.</param>
        public static void SetHeaderStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(HeaderStyleProperty, value);
        }

        /// <summary>
        /// Method gets the value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.IsOpenedProperty"/> attached property from given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <returns>
        /// <see cref="Boolean"/> value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.IsOpenedProperty"/> attached property from given object.
        /// </returns>
        public static bool GetIsOpened(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOpenedProperty);
        }

        /// <summary>
        /// Method sets the value of <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.IsOpenedProperty"/> property to given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <param name="value">New value.</param>
        public static void SetIsOpened(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOpenedProperty, value);
        }

        #endregion Public Methods

        #region Implementation

        /// <summary>
        /// Updates property value cache and raises <see cref="VisualStyleChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnVisualStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VisualStyleChanged != null)
            {
                VisualStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Fire SelectedItemChanged event.
        /// </summary>
        /// <param name="d">TaskBar selection changed</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBar owner = (TaskBar)d;
            owner.FireSelectedItemChanged();
        }

        /// <summary>
        /// Updates property value cache and raises
        /// <see cref="GroupOrientationChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value</param>
        protected virtual void OnGroupOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupOrientationChanged != null)
            {
                GroupOrientationChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="GroupMarginChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnGroupMarginChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupMarginChanged != null)
            {
                GroupMarginChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="GroupWidthChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnGroupWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupWidthChanged != null)
            {
                GroupWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SpeedChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnSpeedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SpeedChanged != null)
            {
                SpeedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="GroupPaddingChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        protected virtual void OnGroupPaddingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupPaddingChanged != null)
            {
                GroupPaddingChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ButtonSizeChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnButtonSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ButtonSizeChanged != null)
            {
                ButtonSizeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="HeaderStyleChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnHeaderStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HeaderStyleChanged != null)
            {
                HeaderStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsOpenedChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsOpenedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsOpenedChanged != null)
            {
                IsOpenedChanged(this, e);
            }
        }

        /// <summary>
        /// Undoes the effects of the <see cref="PrepareContainerForItemOverride( DependencyObject, Object)"/> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item clear container.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            FrameworkElement felement = (FrameworkElement)item;

            if (null != felement.Parent)
            {
                TaskBarItem parent = felement.Parent as TaskBarItem;

                if (null != parent)
                {
                    Debug.Assert(null != parent, "Unknown parent type.");
                    parent.WrappedRemoveLogicalChild(felement);
                }
            }
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is (or is eligible to be) its own container; otherwise, false</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TaskBarItem;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TaskBarItem();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (item is TaskBarItem)
            {
                TaskBarItem fxelement = (TaskBarItem)element;
                base.PrepareContainerForItemOverride(fxelement, item);
            }
            else
            {
                if (item is FrameworkElement)
                {
                    Object attachHeader = TaskBarItem.GetAttachHeader((FrameworkElement)item);
                    this.RemoveLogicalChild(item);
                    TaskBarItem result = (TaskBarItem)element;
                    Button b = new Button();
                    result.Header = attachHeader;
                    result.Items.Add(item);

                    if (null == attachHeader)
                    {
                        result.Header = " ";
                    }
                    base.PrepareContainerForItemOverride(result, result);
                }
                else
                {
                    TaskBarItem fxelement = (TaskBarItem)element;
                    if (fxelement != null)
                    {
                        base.PrepareContainerForItemOverride(fxelement, item);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method
        /// is invoked whenever <see cref="E:System.Windows.FrameworkElement.Initialized"/> is set
        /// to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        /// <summary>
        /// Method verifies given speed speed value.
        /// </summary>
        /// <param name="speed">Given object that represents speed value.</param>
        /// <returns>True if verified, otherwise false.</returns>
        private static bool CheckSpeed(object speed)
        {
            double d = (double)speed;
            return d >= 0;
        }

        /// <summary>
        /// Verify the height of the button
        /// </summary>
        /// <param name="height">Given object that represents height value.</param>
        /// <returns>True if verified, otherwise false.</returns>
        private static bool CheckButtonHeight(object height)
        {
            double d = (double)height;
            return d >= 14;
        }

        /// <summary>
        /// Calls OnGroupOrientationChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnGroupOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBar instance = (TaskBar)d;
            instance.OnGroupOrientationChanged(e);
        }

        /// <summary>
        /// Calls OnGroupMarginChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnGroupMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBar instance = (TaskBar)d;
            instance.OnGroupMarginChanged(e);
        }

        /// <summary>
        /// Calls OnGroupWidthChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBar instance = (TaskBar)d;
            instance.OnGroupWidthChanged(e);
        }

        /// <summary>
        /// Calls OnSpeedChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBar owner = d as TaskBar;

            if (null != owner)
            {
                owner.OnSpeedChanged(e);
            }
        }

        /// <summary>
        /// Calls OnGroupPaddingChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnGroupPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBar owner = d as TaskBar;

            if (null != owner)
            {
                owner.OnGroupPaddingChanged(e);
            }
        }

        /// <summary>
        /// Calls OnButtonSizeChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnButtonSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBar owner = d as TaskBar;

            if (null != owner)
            {
                owner.OnButtonSizeChanged(e);
            }
        }

        /// <summary>
        /// Calls OnHeaderStyleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBar owner = d as TaskBar;

            if (null != owner)
            {
                owner.OnHeaderStyleChanged(e);
            }
        }

        /// <summary>
        /// Calls IsOpenedChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsOpenedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskBar owner = d as TaskBar;

            if (null != owner)
            {
                owner.OnIsOpenedChanged(e);
            }

            TaskBarItem taskBarItem = d as TaskBarItem;

            if (taskBarItem != null)
            {
                TaskBar parent = taskBarItem.Parent as TaskBar;

                if (parent != null)
                {
                    parent.SelectedItem = taskBarItem;
                    parent.OnIsOpenedChanged(e);
                }
            }
        }

        #endregion Implementation

        #region Dependency Properties

        /// <summary>
        /// Identifies <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.SpeedProperty"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.RegisterAttached("Speed", typeof(double), typeof(TaskBar), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.Inherits, OnSpeedChanged), CheckSpeed);

        /// <summary>
        /// Identifies <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.GroupPaddingProperty"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupPaddingProperty = DependencyProperty.RegisterAttached("GroupPadding", typeof(Thickness), typeof(TaskBar), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.Inherits, OnGroupPaddingChanged));

        /// <summary>
        /// Identifies <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.GroupPaddingProperty"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.RegisterAttached("ButtonSize", typeof(double), typeof(TaskBar), new FrameworkPropertyMetadata(14d, FrameworkPropertyMetadataOptions.Inherits, OnButtonSizeChanged), CheckButtonHeight);

        /// <summary>
        /// Identifies <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.HeaderStyleProperty"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.RegisterAttached("HeaderStyle", typeof(Style), typeof(TaskBar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnHeaderStyleChanged));

        /// <summary>
        /// Identifies <see cref="Syncfusion.Windows.Tools.Controls.TaskBar.IsOpenedProperty"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenedProperty = DependencyProperty.RegisterAttached("IsOpened", typeof(bool), typeof(TaskBar), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits, OnIsOpenedChanged));

        /// <summary>
        /// Identifies <see cref="GroupOrientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupOrientationProperty = DependencyProperty.Register("GroupOrientation", typeof(Orientation), typeof(TaskBar), new UIPropertyMetadata(Orientation.Vertical, OnGroupOrientationChanged));

        /// <summary>
        /// Identifies <see cref="GroupMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupMarginProperty = DependencyProperty.Register("GroupMargin", typeof(Thickness), typeof(TaskBar), new UIPropertyMetadata(new Thickness(0), OnGroupMarginChanged));

        /// <summary>
        /// Identifies <see cref="GroupWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupWidthProperty = DependencyProperty.Register("GroupWidth", typeof(double), typeof(TaskBar), new UIPropertyMetadata(double.NaN, OnGroupWidthChanged));

        /// <summary>
        /// Identifies <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(TaskBarItem), typeof(TaskBar), new UIPropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// Identifies <see cref="LastChildFill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastChildFillProperty = DependencyProperty.Register("LastChildFill", typeof(bool), typeof(TaskBar), new UIPropertyMetadata(false));

        #endregion Dependency Properties
    }
}