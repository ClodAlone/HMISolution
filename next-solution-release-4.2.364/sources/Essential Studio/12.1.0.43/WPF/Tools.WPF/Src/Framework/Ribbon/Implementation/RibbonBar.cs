#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Windows.Automation.Peers;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a RibbonBar control.
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
    /// <example><code>public class RibbonBar : ItemsControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:RibbonBar Name="bar" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// Represents a Ribbon Bar control that appears within a <see cref="RibbonTab"/> and hosts buttons and other controls.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a RibbonBar in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:RibbonBar Header="Pages">
    /// <ribbon:DropDownButton Label="Cover Page" SizeForm="Large"  LargeIcon="SampleImages/CoverPage32.png"/>
    /// <ribbon:RibbonButton Label="BlankPage" SizeForm="Large" LargeIcon="SampleImages/BlankPage32.png"/>
    ///  <ribbon:RibbonButton Label="PageBreak" SizeForm="Large" LargeIcon="SampleImages/PageBreak32.png"/>
    /// </ribbon:RibbonBar>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a RibbonBar in C#.
    /// <code>    
    /// RibbonTab tab;
    /// RibbonButton button1;
    /// RibbonButton button2;
    /// RibbonBar bar = new RibbonBar();
    /// bar.Items.Add(button1);
    /// bar.Items.Add(button2);
    /// tab.Items.Add( bar );
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonBar : ItemsControl, IRibbonControl
    {
        #region Private members
        /// <summary>
        /// Desired size of RibbonBar, this value is setting before
        /// collapsing.
        /// </summary>
        public Size m_DesiredSize;

        /// <summary>
        /// Header border element
        /// </summary>
        private FrameworkElement m_headerBorder;

        /// <summary>
        /// Launcher Buttons
        /// </summary>
        private RibbonButton m_launcherButton;

        /// <summary>
        /// Inner Border
        /// </summary>
        internal Border m_innerBorder;

        /// <summary>
        /// Measure mode. 
        /// </summary>
        internal MeasureMode m_measureMode = MeasureMode.Default;

        SystemGesture msystemGesture;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the type of layout.
        /// </summary>
        /// <remarks>
        /// Default value it true.
        /// </remarks>
        /// <value>
        /// Type: <see cref="Boolean"/> 
        /// True if the layout of <see cref="RibbonBar"/> controls should arrange large controls; false is controls should be arranged in two rows.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonBar bar;
        /// // ......
        /// bar.IsLargeButtonPanel = false;
        /// </code>
        /// </example>
        public bool IsLargeButtonPanel
        {
            get
            {
                return (bool)base.GetValue(IsLargeButtonPanelProperty);
            }

            set
            {
                base.SetValue(IsLargeButtonPanelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text that headers the <see cref="RibbonBar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// Text that headers the <see cref="RibbonBar"/>. The default is "RibbonBar" string.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonBar bar = new RibbonBar();
        /// bar.Header = "Buttons Bar";        
        /// </code>
        /// </example>
        /// <seealso cref="RibbonBar"/>
        /// <seealso cref="string"/>
        public string Header
        {
            get
            {
                return (string)base.GetValue(HeaderProperty);
            }

            set
            {
                base.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets the Launcher button of the <see cref="RibbonBar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="RibbonButton"/>
        /// <see cref="RibbonButton"/> that appears in header of the <see cref="RibbonBar"/>.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonBar bar = new RibbonBar();
        /// RibbonButton launcher = bar.LauncherButton;  
        /// launcher.Visibility = Visibility.Hidden;
        /// </code>
        /// </example>
        /// <seealso cref="RibbonButton"/>
        /// <seealso cref="RibbonBar"/>
        public RibbonButton LauncherButton
        {
            get
            {

                return this.GetTemplateChild("PART_DialogLauncherButton") as RibbonButton;
            }

        }



        /// <summary>
        /// Gets or sets the image that appears in a when <see cref="RibbonBar"/> is collapsed.
        /// </summary>
        /// <value>
        /// </value>
        /// <example>
        /// <code>
        /// // Create the image element.
        /// RibbonBar bar = new RibbonBar();
        /// // Create source.
        /// BitmapImage bimage = new BitmapImage();
        /// // BitmapImage.UriSource must be in a BeginInit/EndInit block.
        /// bimage.BeginInit();
        /// bimage.UriSource = new Uri(@"/sampleImages/sample.jpg",UriKind.RelativeOrAbsolute);
        /// bimage.EndInit();
        /// // Set the image source.
        /// bar.CollapseImage = bimage;
        /// </code>
        /// </example>
        /// <seealso cref="RibbonBar"/>
        /// <seealso cref="ImageSource"/>
        public ImageSource CollapseImage
        {
            get
            {
                return (ImageSource)GetValue(CollapseImageProperty);
            }

            set
            {
                SetValue(CollapseImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates state of the <see cref="RibbonBar"/>.
        /// </summary>
        /// <remarks>
        /// Default value it TwoRow.
        /// </remarks>
        /// <value>
        /// Type: <see cref="RibbonBarState"/> 
        /// TwoRow if controls should be arranged in two rows.
        /// Collapsed if large collapse image should be displayed.
        /// Extra Small if <see cref="RibbonBar"/> is collapsed in extra small size.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonBar bar;
        /// // ......
        /// RibbonBarState state = bar.PanelState;
        /// </code>
        /// </example>
        /// <seealso cref="RibbonBar"/>
        /// <seealso cref="RibbonBarState"/>
        public RibbonBarState PanelState
        {
            get
            {
                return (RibbonBarState)GetValue(PanelStateProperty);
            }

            protected internal set
            {
                SetValue(PanelStatePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the launcher tool tip. <see cref="LauncherButton"/> control.
        /// </summary>
        /// <value>
        /// Type: <see cref="Object"/>
        /// Object that represents <see cref="LauncherButton"/> tooltip.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonBar bar;
        /// // ......
        /// bar.LauncherToolTip = "Tooltip text";
        /// </code>
        /// </example>
        /// <seealso cref="LauncherButton"/>
        public object LauncherToolTip
        {
            get
            {
                return (object)GetValue(LauncherToolTipProperty);
            }

            set
            {
                SetValue(LauncherToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is launcher button visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is launcher button visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsLauncherButtonVisible
        {
            get
            {
                return (bool)GetValue(IsLauncherButtonVisibleProperty);
            }

            set
            {
                SetValue(IsLauncherButtonVisibleProperty, value);
            }
        }



        /// <summary>
        /// Gets or sets a value indicating whether this instance is ribbon gallery present.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is ribbon gallery present; otherwise, <c>false</c>.
        /// </value>
        public bool IsRibbonGalleryPresent
        {
            get
            {
                return (bool)GetValue(IsRibbonGalleryPresentProperty);
            }

            set
            {
                SetValue(IsRibbonGalleryPresentProperty, value);
            }
        }

        /// <summary>
        /// Gets the drop down button.
        /// </summary>
        /// <value>The drop down button.</value>
        internal DropDownButton DropDownButton
        {
            get
            {
                return GetTemplateChild("PART_RibbonDropDownButton") as DropDownButton;
            }
        }

        /// <summary>
        /// Gets or sets a value which identifies the measure mode of Ribbon bar. It is used in Resizing controls.
        /// </summary>
        internal MeasureMode MeasureMode
        {
            get
            {
                return m_measureMode;
            }
            set
            {
                m_measureMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show in more commands].
        /// </summary>
        /// <value><c>true</c> if [show in more commands]; otherwise, <c>false</c>.</value>
        public bool ShowInMoreCommands
        {
            get { return (bool)GetValue(ShowInMoreCommandsProperty); }
            set { SetValue(ShowInMoreCommandsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowInMoreCommands.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Gets or sets a value indicating whether [show in more commands].
        /// </summary>
        public static readonly DependencyProperty ShowInMoreCommandsProperty =
            DependencyProperty.Register("ShowInMoreCommands", typeof(bool), typeof(RibbonBar), new UIPropertyMetadata(true));

        
        public string KeyTipOnCollapsed
        {
            get { return (string)GetValue(KeyTipOnCollapsedProperty); }
            set { SetValue(KeyTipOnCollapsedProperty, value); }
        }

        #endregion

        #region Events
        /// <summary>
        /// Identifies the Click routed event.
        /// </summary>
        /// <value>
        /// Type: <see cref="RoutedEvent"/>
        /// The identifier for the Launcher button Click routed event.
        /// </value>
        /// <remarks>
        /// This event corresponds to a left mouse Launcher button click.
        /// </remarks>
        public static RoutedEvent LauncherClickEvent;

        /// <summary>
        /// Event that is raised when <see cref="LauncherButton"/> is clicked.
        /// </summary>
        public event RoutedEventHandler LauncherClick
        {
            add
            {
                base.AddHandler(LauncherClickEvent, value);
            }

            remove
            {
                base.RemoveHandler(LauncherClickEvent, value);
            }
        }

        /// <summary>
        /// Event that is raised when CollapseImage property is changed.
        /// </summary>
        public event PropertyChangedCallback CollapseImageChanged;

        /// <summary>
        /// Event that is raised when PanelState property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelStateChanged;

        /// <summary>
        /// Event that is raised when Header property is changed.
        /// </summary>
        public event PropertyChangedCallback HeaderChanged;

        #endregion

        #region Dependency Properties
        /// <summary>
        /// Gets or sets the value of the ImageSource which will be
        /// shown, when RibbonBar is collapsed. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapseImageProperty = DependencyProperty.Register("CollapseImage", typeof(ImageSource), typeof(RibbonBar), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCollapseImageChanged)));

        /// <summary>
        /// Gets or sets Header of the RibbonBar. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(RibbonBar), new FrameworkPropertyMetadata("RibbonBar", FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnHeaderChanged)));

        /// <summary>
        /// Gets or sets value that indicates type of layout. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLargeButtonPanelProperty = DependencyProperty.Register("IsLargeButtonPanel", typeof(bool), typeof(RibbonBar), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Defines state of the panel for read-only purposes. This is a dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey PanelStatePropertyKey = DependencyProperty.RegisterReadOnly("PanelState", typeof(RibbonBarState), typeof(RibbonBar), new FrameworkPropertyMetadata(RibbonBarState.TwoRow, new PropertyChangedCallback(OnPanelStateChanged)));

        /// <summary>
        /// Defines state of the panel. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelStateProperty = PanelStatePropertyKey.DependencyProperty;

        /// <summary>
        /// Get / sets the LauncherButton tooltip object. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LauncherToolTipProperty =
           DependencyProperty.Register("LauncherToolTip", typeof(object), typeof(RibbonBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Get/sets whether LauncherButton is visible. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLauncherButtonVisibleProperty =
            DependencyProperty.Register("IsLauncherButtonVisible", typeof(bool), typeof(RibbonBar), new UIPropertyMetadata(true));

        /// <summary>
        /// Get/sets whether RibbonGallery is present in RibbonBar. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRibbonGalleryPresentProperty =
            DependencyProperty.Register("IsRibbonGalleryPresent", typeof(bool), typeof(RibbonBar), new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for KeyTipTextOnCollapsed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyTipOnCollapsedProperty = DependencyProperty.Register("KeyTipOnCollapsed", typeof(string), typeof(RibbonBar), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnKeyTipOnCollapsedChanged)));



        #endregion

        #region Attached Properties


        /// <summary>
        /// Represents the Launcher command, This is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty LauncherCommandProperty =
            DependencyProperty.RegisterAttached("LauncherCommand", typeof(ICommand), typeof(RibbonBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Represents the Launcher command target, This is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty LauncherCommandTargetProperty =
           DependencyProperty.RegisterAttached("LauncherCommandTarget", typeof(IInputElement), typeof(RibbonBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Represents the launcher command parameter, This is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty LauncherCommandParameterProperty =
           DependencyProperty.RegisterAttached("LauncherCommandParameter", typeof(object), typeof(RibbonBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the launcher command.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static ICommand GetLauncherCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LauncherCommandProperty);
        }

        /// <summary>
        /// Sets the launcher command.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetLauncherCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LauncherCommandProperty, value);
        }


        /// <summary>
        /// Gets the launcher command parameter.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static object GetLauncherCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(LauncherCommandParameterProperty);
        }

        /// <summary>
        /// Sets the launcher command parameter.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetLauncherCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(LauncherCommandParameterProperty, value);
        }


        /// <summary>
        /// Gets the launcher command target.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static IInputElement GetLauncherCommandTarget(DependencyObject obj)
        {
            return (IInputElement)obj.GetValue(LauncherCommandTargetProperty);
        }

        /// <summary>
        /// Sets the launcher command target.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetLauncherCommandTarget(DependencyObject obj, IInputElement value)
        {
            obj.SetValue(LauncherCommandTargetProperty, value);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RibbonBar"/> class.
        /// </summary>
        static RibbonBar()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonBar));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonBar), new FrameworkPropertyMetadata(typeof(RibbonBar)));
            LauncherClickEvent = EventManager.RegisterRoutedEvent("LaunchDialog", RoutingStrategy.Direct, typeof(EventHandler), typeof(RibbonBar));

        }

        public RibbonBar()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && Application.Current.MainWindow != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }         
        }       

        #endregion

        #region Implementation

        /// <summary>
        /// Compree Small Buttons in the Ribbon Bar. Used while resizing.
        /// </summary>
        /// <returns>return true if compressed.</returns>
        internal bool CompressSmallItems()
        {
            bool issmall = false;
            int count = Items.Count;
            int i = 0;
            while (i < count)
            {
                ICollapsable col = ItemContainerGenerator.ContainerFromIndex(i) as ICollapsable;
                if (col != null)
                {
                    //if (!(col is DropDownButton || col is SplitButton))
                    //{
                        if (Ribbon.GetIsAutoSizeFormEnabled (col as DependencyObject) &&  col.SizeForm == SizeForm.Small)
                        {
                            if (((FrameworkElement)col).Tag == null)
                            {
                                issmall = true;
                                col.SizeForm = SizeForm.ExtraSmall;
                                ((FrameworkElement)col).Tag = "Resized";
                                MeasureMode = MeasureMode.Compressed;
                            }
                        }
                    //}
                }
                i++;
            }
            return issmall;
        }

        /// <summary>
        /// Compress Large Buttons in the ribbon bar. Used while resizing.
        /// </summary>
        /// <returns>return true if compressed.</returns>
        internal bool CompressLargeItems()
        {
            Ribbon ribbon = null;
            if (this.Parent is RibbonTab)
            {
                ribbon = (this.Parent as RibbonTab).Parent as Ribbon;
            }

            bool islarge = false;
            int count = Items.Count;
            int i = 0;
            while (i < count)
            {
                ICollapsable col = ItemContainerGenerator.ContainerFromIndex(i) as ICollapsable;
                if (col != null)
                {
                    
                    if (ribbon!=null &&  Ribbon.GetIsAutoSizeFormEnabled(ribbon as DependencyObject) && col.SizeForm == SizeForm.Large)
                    {
                        if (((FrameworkElement)col).Tag == null)
                        {
                            islarge = true;
                            col.SizeForm = SizeForm.Small;
                            ((FrameworkElement)col).Tag = "Resized";
                            MeasureMode = MeasureMode.Compressed;
                            if (col is RibbonButton)
                            {
                                RibbonButton button = col as RibbonButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.CollapseLabel;
                            }

                            if (col is DropDownButton)
                            {
                                DropDownButton button = col as DropDownButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.CollapseLabel;
                            }

                            if (col is SplitButton)
                            {
                                SplitButton button = col as SplitButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.CollapseLabel;
                            }
                        }
                    }
                    //}
                }
                i++;
            }
            return islarge;
        }

        /// <summary>
        /// Sets to default.
        /// </summary>
        internal void SetToDefault()
        {
            foreach (FrameworkElement element in this.Items)
            {
                if (element.Tag != null)
                {
                    ICollapsable col = (ICollapsable)element;
                    if (col != null)
                    {
                        if (col.SizeForm == SizeForm.ExtraSmall)
                        {
                            col.SizeForm = SizeForm.Small;

                            if (col is RibbonButton)
                            {
                                RibbonButton button = col as RibbonButton;
                                if (button.CollapseLabel != null)                                                              
                                    button.Label = button.CollapseLabel;                                
                            }

                            if (col is DropDownButton)
                            {
                                DropDownButton button = col as DropDownButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.CollapseLabel;
                            }

                            if (col is SplitButton)
                            {
                                SplitButton button = col as SplitButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.CollapseLabel;
                            }
                          
                        }
                        else if (col.SizeForm == SizeForm.Small)
                        {
                            col.SizeForm = SizeForm.Large;

                            if (col is RibbonButton)
                            {
                                RibbonButton button = col as RibbonButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.tempLabel;
                            }

                            if (col is DropDownButton)
                            {
                                DropDownButton button = col as DropDownButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.tempLabel;
                            }

                            if (col is SplitButton)
                            {
                                SplitButton button = col as SplitButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.tempLabel;
                            }
                        }
                        this.MeasureMode = MeasureMode.Default;
                        element.Tag = null;
                    }
                }
            }
        }

        /// <summary>
        /// Calls OnCollapseImageChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnCollapseImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar instance = (RibbonBar)d;
            instance.OnCollapseImageChanged(e);

        }

        /// <summary>
        /// Updates property value cache and raises CollapseImageChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnCollapseImageChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CollapseImageChanged != null)
            {
                CollapseImageChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHeaderChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar instance = (RibbonBar)d;
            instance.OnHeaderChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HeaderChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HeaderChanged != null)
            {
                HeaderChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelStateChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnPanelStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar instance = (RibbonBar)d;
            instance.OnPanelStateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises PanelStateChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnPanelStateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelStateChanged != null)
            {
                PanelStateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelStateChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnKeyTipOnCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar instance = (RibbonBar)d;
            instance.OnKeyTipOnCollapsedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises PanelStateChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnKeyTipOnCollapsedChanged(DependencyPropertyChangedEventArgs e)
        {
           
        }

        /// <summary>
        /// Show the popup of the RibbonBar if it is collapsed.
        /// </summary>
        public void ShowPopup()
        {
            if (this.PanelState == RibbonBarState.Collapsed && this.DropDownButton != null)
            {
                this.DropDownButton.IsDropDownOpen = true;
            }
        }

        /// <summary>
        /// Hide the popup of the RibbonBar if it is collapsed.
        /// </summary>
        public void HidePopup()
        {
            if (this.PanelState == RibbonBarState.Collapsed && this.DropDownButton != null)
            {
                this.DropDownButton.IsDropDownOpen = false;
            }
        }
 
 
        #endregion

        #region Overrides
        /// <summary>
        /// This method will be invoked whenever application code or
        /// internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {

            base.OnApplyTemplate();
            m_innerBorder = (Border)GetTemplateChild("PART_InnerBorder");
            if (m_innerBorder != null)
            {
                m_innerBorder.MouseEnter += new MouseEventHandler(m_innerBorder_MouseMove);
                m_innerBorder.MouseLeave += new MouseEventHandler(m_innerBorder_MouseLeave);

                 #if !SyncfusionFramework3_5
                m_innerBorder.TouchEnter += new EventHandler<TouchEventArgs>(m_innerBorder_TouchLeave);
                m_innerBorder.TouchLeave += new EventHandler<TouchEventArgs>(m_innerBorder_TouchLeave);
#endif

            }
            m_launcherButton = (RibbonButton)this.GetTemplateChild("PART_DialogLauncherButton");
            if (m_launcherButton != null)
            {
                m_launcherButton.Label = Header + " Launcher";
                m_launcherButton.Click += new RoutedEventHandler(LauncherButton_Click);
            }

            m_headerBorder = GetTemplateChild("PART_headerBorder") as FrameworkElement;

            if (this.DropDownButton != null)
            {
                this.DropDownButton.MouseRightButtonUp += new MouseButtonEventHandler(DropDownButton_MouseRightButtonUp);
                 #if !SyncfusionFramework3_5
                this.DropDownButton.TouchUp+=new EventHandler<TouchEventArgs>(DropDownButton_TouchUp);
#endif
            }

            Ribbon ribbon = VisualUtils.FindSomeParent(this, typeof(Ribbon)) as Ribbon;
            if (ribbon != null && CollapseImage==null)
            {
                BindingUtils.SetBinding(this, ribbon, RibbonBar.CollapseImageProperty, Ribbon.RibbonBarCollapseImageProperty, BindingMode.TwoWay);
            }
            
        }

         #if !SyncfusionFramework3_5
        void m_innerBorder_TouchLeave(object sender, TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch && null != m_innerBorder)
            {
                if (!m_innerBorder.IsMouseOver)
                {
                    RibbonTab tab = this.Parent as RibbonTab;
                    SetBorder(tab, true);
                }
            }
        }
#endif

        /// <summary>
        /// Handles the MouseLeave event of the m_innerBorder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_innerBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (null != m_innerBorder && e.StylusDevice==null)
            {
                if (!m_innerBorder.IsMouseOver)
                {
                    RibbonTab tab = this.Parent as RibbonTab;
                    SetBorder(tab, true);
                }
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the m_innerBorder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_innerBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (null != m_innerBorder)
            {
                if (m_innerBorder.IsMouseOver)
                {
                    RibbonTab tab = this.Parent as RibbonTab;
                    SetBorder(tab, false);
                }
            }
        }

        /// <summary>
        /// Sets the border.
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="setdefault">if set to <c>true</c> [setdefault].</param>
        private void SetBorder(RibbonTab tab, bool setdefault)
        {
            if (!setdefault)
            {
                GradientStopCollection coll = new GradientStopCollection();
                GradientStop gr = new GradientStop((Color)ColorConverter.ConvertFromString("#C7C7C7"), 1.0);
                if (tab != null)
                {
                    GradientStop gr1 = new GradientStop(tab.ContextColor, 0.0);
                    coll.Add(gr);
                    coll.Add(gr1);
                }
                LinearGradientBrush brush = new LinearGradientBrush(coll, new Point(0.5, 1), new Point(0.5, 0));
                if (m_innerBorder != null)
                {
                    string currentskin = SkinStorage.GetVisualStyle(this).ToString();
                    if(currentskin!="SyncOrange")
                    m_innerBorder.BorderBrush = brush;
                }
            }
            else
            {
                if (m_innerBorder != null)
                {
                    m_innerBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        /// <summary>
        /// Handles the MouseRightButtonUp event of the DropDownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DropDownButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if ((e.Source == DropDownButton && e.StylusDevice == null )|| (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                DropDownButton.IsDropDownOpen = false;
                RibbonContextMenu.CreateContextMenu(this);
                e.Handled = true;
            }
        }

         #if !SyncfusionFramework3_5
        private void DropDownButton_TouchUp(object sender, TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.Source == DropDownButton && ribbonTouch!=null && ribbonTouch.EnableTouch && msystemGesture == SystemGesture.RightTap)
            {
                DropDownButton.IsDropDownOpen = false;
                RibbonContextMenu.CreateContextMenu(this);
                e.Handled = true;
            }
        }
#endif


        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            msystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                if (this.ContextMenu != null)
                {
                    this.ContextMenu.IsOpen = false;
                    this.ContextMenu = null;
                }

                base.OnMouseRightButtonUp(e);

                bool isLauncherButtonPart = VisualUtils.IsDescendant(m_launcherButton, e.OriginalSource as DependencyObject);
                bool isHeaderBorderPart = VisualUtils.IsDescendant(m_headerBorder, e.OriginalSource as DependencyObject);
                if (isLauncherButtonPart == false && isHeaderBorderPart == true)
                {
                    RibbonContextMenu.CreateContextMenu(this);
                    e.Handled = true;
                    return;
                }

                if (e.Source is ItemsControl)
                {
                    FrameworkElement realSource = RibbonContextMenu.GetRealSource(e.Source as ItemsControl);
                    if (realSource != null)
                    {
                        if (!realSource.IsEnabled)
                        {
                            ContextMenuService.SetShowOnDisabled(realSource, true);
                            RibbonContextMenu.CreateContextMenu(realSource);
                            e.Handled = true;
                            return;
                        }
                    }
                }
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (msystemGesture == SystemGesture.RightTap && ribbonTouch!=null && ribbonTouch.EnableTouch)
            {
                if (this.ContextMenu != null)
                {
                    this.ContextMenu.IsOpen = false;
                    this.ContextMenu = null;
                }

                base.OnTouchUp(e);

                bool isLauncherButtonPart = VisualUtils.IsDescendant(m_launcherButton, e.OriginalSource as DependencyObject);
                bool isHeaderBorderPart = VisualUtils.IsDescendant(m_headerBorder, e.OriginalSource as DependencyObject);
                if (isLauncherButtonPart == false && isHeaderBorderPart == true)
                {
                    RibbonContextMenu.CreateContextMenu(this);
                    e.Handled = true;
                    return;
                }

                if (e.Source is ItemsControl)
                {
                    FrameworkElement realSource = RibbonContextMenu.GetRealSource(e.Source as ItemsControl);
                    if (realSource != null)
                    {
                        if (!realSource.IsEnabled)
                        {
                            ContextMenuService.SetShowOnDisabled(realSource, true);
                            RibbonContextMenu.CreateContextMenu(realSource);
                            e.Handled = true;
                            return;
                        }
                    }
                }

              
            }
        }

#endif
        /// <summary>
        /// Handles the Click event of the launcherButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void LauncherButton_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = LauncherClickEvent;
            base.RaiseEvent(args);
        }

        /// <summary>
        /// Invoked when the <see cref="E:System.Windows.UIElement.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
            {
                if (this.DropDownButton != null && this.DropDownButton.IsDropDownOpen)
                {
                    this.DropDownButton.IsDropDownOpen = false;
                    e.Handled = true;
                }


            }

        }

        /// <summary>
        /// Invoked when control is initialized.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.ApplyTemplate();
        }

        #endregion

        #region IRibbonControl Members

        /// <summary>
        /// Gets the value of the Label property.
        /// </summary>
        /// <value></value>
        string IRibbonControl.Label
        {
            get
            {
                return Header;
            }
        }

        /// <summary>
        /// Gets the value of the Image property.
        /// </summary>
        /// <value></value>
        ImageSource IRibbonControl.SmallIcon
        {
            get
            {
                return CollapseImage;
            }
        }
        #endregion

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RibbonBarAutomationPeer(this);
        }

    }   
   
}
