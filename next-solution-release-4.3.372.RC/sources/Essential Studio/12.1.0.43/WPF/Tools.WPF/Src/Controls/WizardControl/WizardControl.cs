// <copyright file="WizardControl.cs" company="Syncfusion">
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
//using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Specialized;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Tools.Controls.Resources;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// A WizardControl that implements a classic Wizard UI. Add <see cref="WizardPage"
    /// />s to the control and switch between the pages using the Next, Back, etc.
    /// buttons. Navigation and look and feel is fully customizable
    /// </summary>
    /// <example>
    ///  <code lang="XAML">
    /// <Window x:Class="WizardControl.Window1"
    ///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///     Title="Window1" Height="300" Width="300"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:WizardControl Name="wizardControl"/>
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
    /// namespace wizardControl
    /// {
    ///     /// <summary>
    ///     /// Interaction logic for Window1.xaml
    ///     /// </summary>
    ///     public partial class Window1 : Window
    ///     {
    ///         public Window1()
    ///         {
    ///             InitializeComponent();
    ///             WizardControl wizardControl = new WizardControl();
    ///             this.Content = wizardControl;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif
  
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Blend,
    Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/BlendStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Blue,
        Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/Office2007BlueStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Black,
        Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/Office2007BlackStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Silver,
        Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/Office2007SilverStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Blue,
       Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/Office2010BlueStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Black,
        Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/Office2010BlackStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Silver,
        Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/Office2010SilverStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Default,
        Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/Generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2003,
        Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/Office2003Style.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.ShinyBlue,
      Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/ShinyBlue.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.ShinyRed,
       Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/ShinyRed.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.SyncOrange,
       Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/SyncOrangeStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.VS2010,
       Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/VS2010Style.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Metro,
      Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/MetroStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Transparent,
      Type = typeof(WizardControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/WizardControl/Themes/TransparentStyle.xaml")]  


    public class WizardControl : ItemsControl
    {
        #region FIELDS

        /// <summary>
        /// Represent object reference of Resource wrapper class.
        /// </summary>
        ResourceWrapper wrapper = new ResourceWrapper();

        /// <summary>
        /// Presents Wizard page
        /// </summary>
        internal bool flagChangingSelectedWizardPage = false;

        /// <summary>
        /// Presents current item
        /// </summary>
        internal bool flagChangingCurrentItem = false;

        /// <summary>
        /// Indicates Itemsourcechanged
        /// </summary>

        internal bool IsItemSourceChanged = false;

        /// <summary>
        /// This varaible is a reference to language dictionary.
        /// </summary>
       // private ResourceDictionary langDictionary;

        /// <summary>
        /// This varaible is used to store help text
        /// </summary>
        private static string helpText;

        /// <summary>
        /// This varaible is used to store finish text
        /// </summary>
        private static string finishText;

        /// <summary>
        /// This varaible is used to store cancel text
        /// </summary>
        private static string cancelText;

        /// <summary>
        /// This varaible is used to store back text
        /// </summary>
        private static string backText;

        /// <summary>
        /// This varaible is used to store next text
        /// </summary>
        private static string nextText;
        #endregion

        #region PROPERTY_DEFINITIONS

        /// <summary>
        /// Identifies the <see cref=" "/> property.
        /// </summary>
        public static readonly DependencyProperty CancelSPCEventsOnItemsSourceChangedProperty =
           DependencyProperty.RegisterAttached("CancelSPCEventsOnItemsSourceChanged", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(false));
    
        /// <summary>
        /// Identifies the <see cref="ExteriorPageBannerImageMinWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty ExteriorPageBannerImageMinWidthProperty
            = DependencyProperty.Register("ExteriorPageBannerImageMinWidth", typeof(double), typeof(WizardControl), new FrameworkPropertyMetadata(40d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="InteriorPageHeaderMinHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty InteriorPageHeaderMinHeightProperty
            = DependencyProperty.Register("InteriorPageHeaderMinHeight", typeof(double), typeof(WizardControl), new FrameworkPropertyMetadata(40d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="NextText"/> property.
        /// </summary>
        public static readonly DependencyProperty NextTextProperty = DependencyProperty.Register("NextText", typeof(string), typeof(WizardControl), new UIPropertyMetadata("Next"));

        /// <summary>
        /// Identifies the <see cref="NextFocused"/> property.
        /// </summary>
        public static readonly DependencyProperty NextFocusedProperty = DependencyProperty.Register("NextFocused", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="FinishFocused"/> property.
        /// </summary>
        public static readonly DependencyProperty FinishFocusedProperty = DependencyProperty.Register("FinishFocused", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="BackText"/> property.
        /// </summary>
        public static readonly DependencyProperty BackTextProperty = DependencyProperty.Register("BackText", typeof(string), typeof(WizardControl), new UIPropertyMetadata("Back"));

        /// <summary>
        /// Identifies the <see cref="FinishText"/> property.
        /// </summary>
        public static readonly DependencyProperty FinishTextProperty = DependencyProperty.Register("FinishText", typeof(string), typeof(WizardControl), new UIPropertyMetadata("Finish"));

        /// <summary>
        /// Identifies the <see cref="CancelText"/> property.
        /// </summary>
        public static readonly DependencyProperty CancelTextProperty = DependencyProperty.Register("CancelText", typeof(string), typeof(WizardControl), new UIPropertyMetadata("Cancel"));

        /// <summary>
        /// Identifies the <see cref="HelpText"/> property.
        /// </summary>
        public static readonly DependencyProperty HelpTextProperty = DependencyProperty.Register("HelpText", typeof(string), typeof(WizardControl), new UIPropertyMetadata("Help"));

        /// <summary>
        /// Identifies the <see cref="FinishEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty FinishEnabledProperty = DependencyProperty.Register("FinishEnabled", typeof(bool?), typeof(WizardControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="NextEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty NextEnabledProperty = DependencyProperty.Register("NextEnabled", typeof(bool?), typeof(WizardControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BackEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty BackEnabledProperty = DependencyProperty.Register("BackEnabled", typeof(bool?), typeof(WizardControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CancelEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty CancelEnabledProperty = DependencyProperty.Register("CancelEnabled", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="HelpVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty HelpVisibleProperty = DependencyProperty.Register("HelpVisible", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CancelVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty CancelVisibleProperty = DependencyProperty.Register("CancelVisible", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="BackVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty BackVisibleProperty = DependencyProperty.Register("BackVisible", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="NextVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty NextVisibleProperty = DependencyProperty.Register("NextVisible", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="FinishVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty FinishVisibleProperty = DependencyProperty.Register("FinishVisible", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="SelectedWizardPage"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedWizardPageProperty = DependencyProperty.Register("SelectedWizardPage", typeof(WizardPage), typeof(WizardControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(SelectedWizardPage_Changed), CoerceSelectedWizardPage));

        /// <summary>
        /// Identifies <see cref="Cancel"/> routed event.
        /// </summary>
        public static readonly RoutedEvent CancelEvent = EventManager.RegisterRoutedEvent("Cancel", RoutingStrategy.Bubble, typeof(EventHandler), typeof(WizardControl));

        /// <summary>
        /// Identifies <see cref="Finish"/> routed event.
        /// </summary>
        public static readonly RoutedEvent FinishEvent = EventManager.RegisterRoutedEvent("Finish", RoutingStrategy.Bubble, typeof(EventHandler), typeof(WizardControl));

        /// <summary>
        /// Identifies <see cref="Help"/> routed event.
        /// </summary>
        public static readonly RoutedEvent HelpEvent = EventManager.RegisterRoutedEvent("Help", RoutingStrategy.Bubble, typeof(EventHandler), typeof(WizardControl));

        /// <summary>
        /// Identifies <see cref="Help"/> routed event.
        /// </summary>
        public static readonly RoutedEvent NextEvent = EventManager.RegisterRoutedEvent("Next", RoutingStrategy.Bubble, typeof(EventHandler), typeof(WizardControl));

        /// <summary>
        /// Identifies <see cref="SelectedPageChanging"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectedPageChangingEvent = EventManager.RegisterRoutedEvent("SelectedPageChanging", RoutingStrategy.Bubble, typeof(EventHandler<WizardPageSelectionChangeEventArgs>), typeof(WizardControl));

        /// <summary>
        /// Identifies <see cref="SelectedPageChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectedPageChangedEvent = EventManager.RegisterRoutedEvent("SelectedPageChanged", RoutingStrategy.Bubble, typeof(EventHandler), typeof(WizardControl));

        /// <summary>
        /// Identifies the <see cref="EnsureDefaultSelection"/> property.
        /// </summary>
        public static readonly DependencyProperty EnsureDefaultSelectionProperty = DependencyProperty.Register("EnsureDefaultSelection", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CancelButtonCancelsWindow"/> property.
        /// </summary>
        public static readonly DependencyProperty CancelButtonCancelsWindowProperty = DependencyProperty.Register("CancelButtonCancelsWindow", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="NextAndFinishAreDefaultButtons"/> property.
        /// </summary>
        public static readonly DependencyProperty NextAndFinishAreDefaultButtonsProperty = DependencyProperty.Register("NextAndFinishAreDefaultButtons", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="FinishButtonClosesWindow"/> property.
        /// </summary>
        public static readonly DependencyProperty FinishButtonClosesWindowProperty = DependencyProperty.Register("FinishButtonClosesWindow", typeof(bool), typeof(WizardControl), new UIPropertyMetadata(false));

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets value indicates to cancel the selected page changing event.
        /// </summary>


        public bool CancelSPCEventsOnItemsSourceChanged
        {
            get
            {
                return (bool)GetValue(CancelSPCEventsOnItemsSourceChangedProperty);
            }
            set
            {
                SetValue(CancelSPCEventsOnItemsSourceChangedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cancel button cancels window].
        /// </summary>
        /// <value>
        /// true if [cancel button cancels window]; otherwise, false.
        /// </value>
        [Category("Button Behavior")]
        public bool CancelButtonCancelsWindow
        {
            get
            {
                return (bool)GetValue(CancelButtonCancelsWindowProperty);
            }

            set
            {
                SetValue(CancelButtonCancelsWindowProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [next and finish are default buttons].
        /// </summary>
        /// <value>
        /// true if [next and finish are default buttons]; otherwise, false.
        /// </value>
        [Category("Button Behavior")]
        public bool NextAndFinishAreDefaultButtons
        {
            get
            {
                return (bool)GetValue(NextAndFinishAreDefaultButtonsProperty);
            }

            set
            {
                SetValue(NextAndFinishAreDefaultButtonsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [finish button closes window].
        /// </summary>
        /// <value>
        /// true if [finish button closes window]; otherwise, false.
        /// </value>
        [Category("Button Behavior")]
        public bool FinishButtonClosesWindow
        {
            get
            {
                return (bool)GetValue(FinishButtonClosesWindowProperty);
            }

            set
            {
                SetValue(FinishButtonClosesWindowProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [help visible].
        /// </summary>
        /// <value>true if [help visible]; otherwise, false.</value>
        [Category("Button Behavior")]
        public bool HelpVisible
        {
            get
            {
                return (bool)GetValue(HelpVisibleProperty);
            }

            set
            {
                SetValue(HelpVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cancel visible].
        /// </summary>
        /// <value>true if [cancel visible]; otherwise, false.</value>
        [Category("Button Behavior")]
        public bool CancelVisible
        {
            get
            {
                return (bool)GetValue(CancelVisibleProperty);
            }

            set
            {
                SetValue(CancelVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [back visible].
        /// </summary>
        /// <value>true if [back visible]; otherwise, false.</value>
        [Category("Button Behavior")]
        public bool BackVisible
        {
            get
            {
                return (bool)GetValue(BackVisibleProperty);
            }

            set
            {
                SetValue(BackVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [next visible].
        /// </summary>
        /// <value>true if [next visible]; otherwise, false.</value>
        [Category("Button Behavior")]
        public bool NextVisible
        {
            get
            {
                return (bool)GetValue(NextVisibleProperty);
            }

            set
            {
                SetValue(NextVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [finish visible].
        /// </summary>
        /// <value>true if [finish visible]; otherwise, false.</value>
        [Category("Button Behavior")]
        public bool FinishVisible
        {
            get
            {
                return (bool)GetValue(FinishVisibleProperty);
            }

            set
            {
                SetValue(FinishVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the back enabled.
        /// </summary>
        /// <value>The back enabled.</value>
        [Category("Button Behavior")]
        public bool? BackEnabled
        {
            get
            {
                return (bool?)GetValue(BackEnabledProperty);
            }

            set
            {
                SetValue(BackEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the next enabled.
        /// </summary>
        /// <value>The next enabled.</value>
        [Category("Button Behavior")]
        public bool? NextEnabled
        {
            get
            {
                return (bool?)GetValue(NextEnabledProperty);
            }

            set
            {
                SetValue(NextEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cancel enabled].
        /// </summary>
        /// <value><c>true</c> if [cancel enabled]; otherwise, <c>false</c>.</value>
        [Category("Button Behavior")]
        public bool CancelEnabled
        {
            get
            {
                return (bool)GetValue(CancelEnabledProperty);
            }

            set
            {
                SetValue(CancelEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the finish enabled.
        /// </summary>
        /// <value>The finish enabled.</value>
        [Category("Button Behavior")]
        public bool? FinishEnabled
        {
            get
            {
                return (bool?)GetValue(FinishEnabledProperty);
            }

            set
            {
                SetValue(FinishEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ensure default selection].
        /// </summary>
        /// <value>
        /// true if [ensure default selection]; otherwise, false.
        /// </value>
        [Category("Common Properties")]
        public bool EnsureDefaultSelection
        {
            get
            {
                return (bool)GetValue(EnsureDefaultSelectionProperty);
            }

            set
            {
                SetValue(EnsureDefaultSelectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected wizard page.
        /// </summary>
        /// <value>The selected wizard page.</value>
        public WizardPage SelectedWizardPage
        {
            get
            {
                return (WizardPage)GetValue(SelectedWizardPageProperty);
            }

            set
            {
                if (this.SelectedWizardPage != value)
                {
                    SetValue(SelectedWizardPageProperty, value);
                }
            }
        }

        internal List<object> Pages
        {
            get
            {
                if (ItemsSource != null)
                {
                    return (from object w in ItemsSource select w).ToList<object>();
                }
                return null;
            }
        }

        /// <summary>
        /// Coerces the selected wizard page.
        /// </summary>
        /// <param name="o">The o DependencyObject.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>object type</returns>
        private static object CoerceSelectedWizardPage(DependencyObject o, object baseValue)
        {
            WizardPage newPage = baseValue as WizardPage;

            WizardControl wizard = o as WizardControl;

            if (wizard.flagChangingSelectedWizardPage == false)
            {
                if (wizard.SelectedWizardPage != newPage && wizard.ValidateNewPage(WizardPageSelectionChangeCause.Programmatic, newPage) == false)
                {
                    newPage = wizard.SelectedWizardPage;
                }
            }

            //// if (wizard.NextVisible == true)
            //// {
            ////    wizard.Focus();
            //// }

            //// if (wizard.CancelEnabled == true)
            //// {
            ////    wizard.Focus();
            //// }

            if (wizard.NextAndFinishAreDefaultButtons == true)
            {
                if (wizard.SelectedWizardPage != null)
                {
                    if (wizard.SelectedWizardPage.NextEnabled != false)
                    {
                        wizard.NextFocused = true;
                    }
                }
            }
            else
            {
                wizard.NextFocused = false;
            }

            if (wizard.NextAndFinishAreDefaultButtons == true)
            {
                if (wizard.SelectedWizardPage != null)
                {
                    if (wizard.SelectedWizardPage.NextEnabled == null || wizard.SelectedWizardPage.NextVisibility == Visibility.Collapsed)
                    {
                        wizard.FinishFocused = true;
                    }
                }
            }
            else
            {
                wizard.FinishFocused = false;
            }

            return newPage;
        }

        /// <summary>
        /// Selected the wizard page_ changed.
        /// </summary>
        /// <param name="o">The o DependencyObject.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void SelectedWizardPage_Changed(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            WizardControl parentwizard = o as WizardControl;
            if (!parentwizard.Items.Contains(args.NewValue))
            {
                if (parentwizard.ItemsSource != null)
                {
                    parentwizard.Pages.Add(args.NewValue);
                }
                else
                {
                    parentwizard.Items.Add(args.NewValue);
                }
            }
            WizardPage newPage = null, oldPage = null;
            if (args.NewValue != null)
            {
                newPage = args.NewValue as WizardPage;
                newPage.IsSelected = true;
            }

            if (args.OldValue != null)
            {
                oldPage = args.OldValue as WizardPage;
                oldPage.IsSelected = false;
            }

            WizardControl wizard = o as WizardControl;

            wizard.flagChangingCurrentItem = true;
            wizard.Items.MoveCurrentTo(newPage);
            wizard.flagChangingCurrentItem = false;

            //// Fire WizardControl.SelectedPageChanged
            RoutedEventArgs rea = new RoutedEventArgs(WizardControl.SelectedPageChangedEvent, wizard);
            if (wizard.IsItemSourceChanged != true)
            {
                wizard.RaiseEvent(rea);
            }
        
            //// Fire WizardPage.Selected
            if (newPage != null)
            {
                RoutedEventArgs es = new RoutedEventArgs(WizardPage.SelectedEvent, newPage);
                newPage.RaiseEvent(es);
            }

            //// Fire WizardPage.Unselected
            if (oldPage != null)
            {
                RoutedEventArgs eos = new RoutedEventArgs(WizardPage.UnselectedEvent, oldPage);
                oldPage.RaiseEvent(eos);
            }

            //// If there is no selected page, set a default one.
            wizard.EnsureSelectedPage();
        }

        /// <summary>
        /// Gets or sets the height of the interior page header min.
        /// </summary>
        /// <value>The height of the interior page header min.</value>
        [Category("Layout")]
        public double InteriorPageHeaderMinHeight
        {
            get
            {
                return (double)GetValue(InteriorPageHeaderMinHeightProperty);
            }

            set
            {
                SetValue(InteriorPageHeaderMinHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the exterior page banner image min.
        /// </summary>
        /// <value>The width of the exterior page banner image min.</value>
        [Category("Layout")]
        public double ExteriorPageBannerImageMinWidth
        {
            get
            {
                return (double)GetValue(ExteriorPageBannerImageMinWidthProperty);
            }

            set
            {
                SetValue(ExteriorPageBannerImageMinWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the next text.
        /// </summary>
        /// <value>The next text.</value>
        [Category("Button Behavior")]
        public string NextText
        {
            get
            {
                return (string)GetValue(NextTextProperty);
            }

            set
            {
                SetValue(NextTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [next focused].
        /// </summary>
        /// <value><c>true</c> if [next focused]; otherwise, <c>false</c>.</value>
        [Category("Button Behavior")]
        public bool NextFocused
        {
            get
            {
                return (bool)GetValue(NextFocusedProperty);
            }

            set
            {
                SetValue(NextFocusedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [finish focused].
        /// </summary>
        /// <value><c>true</c> if [finish focused]; otherwise, <c>false</c>.</value>
        [Category("Button Behavior")]
        public bool FinishFocused
        {
            get
            {
                return (bool)GetValue(FinishFocusedProperty);
            }

            set
            {
                SetValue(FinishFocusedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the back text.
        /// </summary>
        /// <value>The back text.</value>
        [Category("Button Behavior")]
        public string BackText
        {
            get
            {
                return (string)GetValue(BackTextProperty);
            }

            set
            {
                SetValue(BackTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the finish text.
        /// </summary>
        /// <value>The finish text.</value>
        [Category("Button Behavior")]
        public string FinishText
        {
            get
            {
                return (string)GetValue(FinishTextProperty);
            }

            set
            {
                SetValue(FinishTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cancel text.
        /// </summary>
        /// <value>The cancel text.</value>
        [Category("Button Behavior")]
        public string CancelText
        {
            get
            {
                return (string)GetValue(CancelTextProperty);
            }

            set
            {
                SetValue(CancelTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>The help text.</value>
        [Category("Button Behavior")]
        public string HelpText
        {
            get
            {
                return (string)GetValue(HelpTextProperty);
            }

            set
            {
                SetValue(HelpTextProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardControl"/> class.
        /// </summary>
        public WizardControl()
        {

            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(WizardControl));
            }

            this.CommandBindings.Add(new CommandBinding(WizardCommands.Next, ProcessNextCommand, CanProcessNextCommand));
            this.CommandBindings.Add(new CommandBinding(WizardCommands.Previous, ProcessPreviousCommand, CanProcessPreviousCommand));
            this.CommandBindings.Add(new CommandBinding(WizardCommands.Finish, ProcessFinishCommand, CanProcessFinishCommand));
            this.CommandBindings.Add(new CommandBinding(WizardCommands.Cancel, ProcessCancelCommand, CanProcessCancelCommand));
            this.CommandBindings.Add(new CommandBinding(WizardCommands.SelectPage, ProcessSelectPageCommand));
            this.CommandBindings.Add(new CommandBinding(WizardCommands.Help, ProcessHelpCommand));

            //langDictionary = new ResourceDictionary();
            //langDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Themes/LangDictionary.xaml", UriKind.Relative);

            helpText = wrapper.HelpText; //langDictionary["HelpText"] as string;
            SetValue(HelpTextProperty, helpText);
            nextText = wrapper.NextText; //langDictionary["NextText"] as string;
            SetValue(NextTextProperty, nextText);
            backText = wrapper.BackText; //langDictionary["BackText"] as string;
            SetValue(BackTextProperty, backText);
            cancelText = wrapper.CancelText; //langDictionary["CancelText"] as string;
            SetValue(CancelTextProperty, cancelText);
            finishText = wrapper.FinishText; //langDictionary["FinishText"] as string;
            SetValue(FinishTextProperty, finishText);

            this.Items.CurrentChanged += new EventHandler(Items_CurrentChanged);
            this.Loaded += new RoutedEventHandler(WizardControl_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the WizardControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void WizardControl_Loaded(object sender, RoutedEventArgs e)
        {
            setNewPage();
            if (this.NextAndFinishAreDefaultButtons == true)
            {
                if (this.SelectedWizardPage != null && this.SelectedWizardPage.NextEnabled != false)
                {
                    this.NextFocused = true;
                }
            }
            else
            {
                this.NextFocused = false;
            }
        }

        /// <summary>
        /// Initializes static members of the <see cref="WizardControl"/> class.
        /// </summary>
        static WizardControl()
        {
            //EnvironmentTest.ValidateLicense(typeof(WizardControl));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardControl), new FrameworkPropertyMetadata(typeof(WizardControl)));
        }

        #region PUBLIC EVENTS
        /// <summary>
        /// Bubbling routed event, fired when the Finish button was clicked.
        /// </summary>
        public event RoutedEventHandler Finish
        {
            add
            {
                AddHandler(FinishEvent, value);
            }

            remove
            {
                RemoveHandler(FinishEvent, value);
            }
        }

        /// <summary>
        /// Bubbling routed event, fired when the Cancel button was clicked.
        /// </summary>
        public event RoutedEventHandler Cancel
        {
            add
            {
                AddHandler(CancelEvent, value);
            }

            remove
            {
                RemoveHandler(CancelEvent, value);
            }
        }

        /// <summary>
        /// Bubbling routed event, fired when the Help button was clicked.
        /// </summary>
        public event RoutedEventHandler Help
        {
            add
            {
                AddHandler(HelpEvent, value);
            }

            remove
            {
                RemoveHandler(HelpEvent, value);
            }
        }

        /// <summary>
        /// Bubbling routed event, fired when the Next button was clicked.
        /// </summary>
        public event RoutedEventHandler Next
        {
            add
            {
                AddHandler(NextEvent, value);
            }

            remove
            {
                RemoveHandler(NextEvent, value);
            }
        }

        /// <summary>
        /// Bubbling routed event, fired when the selected page is changing
        /// </summary>
        public event EventHandler<WizardPageSelectionChangeEventArgs> SelectedPageChanging
        {
            add
            {
                AddHandler(SelectedPageChangingEvent, value);
            }

            remove
            {
                RemoveHandler(SelectedPageChangingEvent, value);
            }
        }

        /// <summary>
        /// Bubbling routed event, fired when the selected page changed.
        /// </summary>
        public event RoutedEventHandler SelectedPageChanged
        {
            add
            {
                AddHandler(SelectedPageChangedEvent, value);
            }

            remove
            {
                RemoveHandler(SelectedPageChangedEvent, value);
            }
        }
        #endregion

        #region COMMAND EVENT HANDLERS
        /// <summary>
        /// Moves the next.
        /// </summary>
        public void MoveNext()
        {
            WizardCommands.Next.Execute(null, this);
        }

        /// <summary>
        /// Moves the previous.
        /// </summary>
        public void MovePrevious()
        {
            WizardCommands.Previous.Execute(null, this);
        }

        /// <summary>
        /// Moves the first.
        /// </summary>
        public void MoveFirst()
        {
            if (this.Items.Count > 0)
            {
                this.SelectedWizardPage = ItemContainerGenerator.ContainerFromItem(Items[0]) as WizardPage;
            }
        }

        /// <summary>
        /// Moves the last.
        /// </summary>
        public void MoveLast()
        {
            if (this.Items.Count > 0)
            {
                this.SelectedWizardPage = ItemContainerGenerator.ContainerFromItem(Items[this.Items.Count - 1]) as WizardPage;
            }
        }

        /// <summary>
        /// Processes the select page command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessSelectPageCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WizardPageSelectionChangeCause cause = WizardPageSelectionChangeCause.SelectPageCommand;
            WizardPage page = null;
            if (e.Parameter is WizardSelectPageCommandParameter)
            {
                WizardSelectPageCommandParameter param = e.Parameter as WizardSelectPageCommandParameter;
                page = param.Page;
                cause = param.Cause;
            }
            else if (!(e.Parameter is WizardPage))
            {
                throw new ArgumentException("The parameter passed for the SelectPage command should be a WizardPage");
            }

            if (page == null)
            {
                page = e.Parameter as WizardPage;
            }

            if (ItemContainerGenerator.IndexFromContainer(page) == -1)
            {
                throw new ArgumentException("The WizardPage specified in the SelectPage command is not part of the WizardControl.");
            }

            if (this.ValidateNewPage(cause, page))
            {
                this.flagChangingSelectedWizardPage = true;
                this.SetValue(SelectedWizardPageProperty, page);
                this.flagChangingSelectedWizardPage = false;
            }
        }

        /// <summary>
        /// Processes the next command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessNextCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(WizardControl.NextEvent, this);
            base.RaiseEvent(newEventArgs);
            if (this.SelectedWizardPage != null && this.SelectedWizardPage.NextPage != null && (this.SelectedWizardPage.NextPage.Visibility == Visibility.Collapsed || !this.SelectedWizardPage.NextPage.IsEnabled))
            {
                this.SelectedWizardPage.NextPage=null;
            }

            IsItemSourceChanged = false;
            WizardPage newSelPage = null;
            if (this.Items.Count == 0)
            {
                return;
            }
            else if (this.SelectedWizardPage == null)
            {
                //// Select the first page
                setNewPage();
            }
            else if (this.SelectedWizardPage.NextPage != null)
            {
                if (ItemContainerGenerator.IndexFromContainer(this.SelectedWizardPage.NextPage) == -1)
                {
                    throw new Exception("The specified NextPage is not in the WizardControl.Items collection.");
                }

                newSelPage = this.SelectedWizardPage.NextPage;
            }
            else
            {
                //// Select the next enabled page
                int curIndex = ItemContainerGenerator.IndexFromContainer(this.SelectedWizardPage);
                for (int i = curIndex + 1; i <= this.Items.Count - 1; i++)
                {
                    WizardPage ip = ItemContainerGenerator.ContainerFromItem(Items[i]) as WizardPage;
                    if (ip != null && ip.IsEnabled && ip.Visibility == Visibility.Visible)
                    {
                        newSelPage = ip;
                        break;
                    }
                }
            }

            if (newSelPage != null)
            {
                WizardSelectPageCommandParameter param = new WizardSelectPageCommandParameter()
                {
                    Cause = WizardPageSelectionChangeCause.NextPageCommand,
                    Page = newSelPage
                };
                WizardCommands.SelectPage.Execute(param, this);
            }
        }

        /// <summary>
        /// Determines whether this instance [can process next command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanProcessNextCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.SelectedWizardPage == null)
            {
                e.CanExecute = false;
            }
            else
            {
                if (this.SelectedWizardPage.NextEnabled != null)
                {
                    e.CanExecute = this.SelectedWizardPage.NextEnabled == true;
                }
                else if (this.NextEnabled != null)
                {
                    e.CanExecute = this.NextEnabled == true;
                }
                else
                {
                    //// Default logic:
                    e.CanExecute = this.SelectedWizardPage.NextPage != null;
                    if (e.CanExecute == false)
                    {
                        //// is there an enabled page next to this one in the collection?
                        bool enabledPageAvailable = false;
                        int curIndex = ItemContainerGenerator.IndexFromContainer(this.SelectedWizardPage);
                        for (int i = curIndex + 1; i <= this.Items.Count - 1; i++)
                        {
                            WizardPage ip = ItemContainerGenerator.ContainerFromItem(Items[i]) as WizardPage;
                            if (ip != null)
                            {
                                if (ip.IsEnabled)
                                {
                                    enabledPageAvailable = true;
                                    break;
                                }
                            }
                        }

                        e.CanExecute = enabledPageAvailable;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the previous command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessPreviousCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WizardPage newSelPage = null;

             if(this.SelectedWizardPage != null && this.SelectedWizardPage.PreviousPage != null && (this.SelectedWizardPage.PreviousPage.Visibility == Visibility.Collapsed || !this.SelectedWizardPage.PreviousPage.IsEnabled))
            {
                this.SelectedWizardPage.PreviousPage=null;
            }
            if (this.Items.Count == 0)
            {
                return;
            }
            else if (this.SelectedWizardPage == null)
            {
                //// Select the first page
                setNewPage();
            }
            else if (this.SelectedWizardPage.PreviousPage != null)
            {
                if (ItemContainerGenerator.IndexFromContainer(this.SelectedWizardPage.PreviousPage) == -1)
                {
                    throw new Exception("The specified NextPage is not in the WizardControl.Items collection.");
                }

                newSelPage = this.SelectedWizardPage.PreviousPage;
            }
            else
            {
                //// Select the previous enabled page
                int curIndex = ItemContainerGenerator.IndexFromContainer(this.SelectedWizardPage) - 1;
                for (int i = curIndex; i >= 0; i--)
                {
                    WizardPage wp = ItemContainerGenerator.ContainerFromItem(Items[i]) as WizardPage;
                    if (wp!= null && wp.IsEnabled && wp.Visibility == Visibility.Visible)
                    {
                        newSelPage = wp;
                        break;
                    }
                }
            }

            if (newSelPage != null)
            {
                WizardSelectPageCommandParameter param = new WizardSelectPageCommandParameter()
                {
                    Cause = WizardPageSelectionChangeCause.PreviousPageCommand,
                    Page = newSelPage
                };

                WizardCommands.SelectPage.Execute(param, this);
            }
        }

        /// <summary>
        /// Determines whether this instance [can process previous command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanProcessPreviousCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.SelectedWizardPage == null)
            {
                e.CanExecute = false;
            }
            else
            {
                if (this.SelectedWizardPage.BackEnabled != null)
                {
                    e.CanExecute = this.SelectedWizardPage.BackEnabled == true;
                }
                else if (this.BackEnabled != null)
                {
                    e.CanExecute = this.BackEnabled == true;
                }
                else
                {
                    // Default logic.
                    e.CanExecute = this.SelectedWizardPage.PreviousPage != null;
                    if (e.CanExecute == false)
                    {
                        // is there an enabled page previous to this one in the collection?
                        bool enabledPageAvailable = false;
                        int curIndex = ItemContainerGenerator.IndexFromContainer(this.SelectedWizardPage);
                        for (int i = curIndex - 1; i >= 0; i--)
                        {
                            WizardPage ip = ItemContainerGenerator.ContainerFromItem(Items[i]) as WizardPage;
                            if (ip.IsEnabled)
                            {
                                enabledPageAvailable = true;
                                break;
                            }
                        }

                        e.CanExecute = enabledPageAvailable;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the cancel command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessCancelCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(WizardControl.CancelEvent, this);
            base.RaiseEvent(newEventArgs);

            if (this.CancelButtonCancelsWindow)
            {
                Window parentWindow = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
                if (parentWindow != null)
                {
                    try
                    {
                        parentWindow.DialogResult = false;
                    }
                    catch
                    {
                    }

                    parentWindow.Close();
                }
            }
        }

        /// <summary>
        /// Determines whether this instance [can process cancel command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanProcessCancelCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.SelectedWizardPage == null)
            {
                e.CanExecute = false;
            }
            else
            {
                if (this.SelectedWizardPage.CancelEnabled != null)
                {
                    e.CanExecute = this.SelectedWizardPage.CancelEnabled == true;
                }
                else
                {
                    e.CanExecute = this.CancelEnabled;
                }
            }
        }

        /// <summary>
        /// Processes the finish command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessFinishCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.SelectedWizardPage != null)
            {
                RoutedEventArgs newEventArgs = new RoutedEventArgs(WizardControl.FinishEvent, this);
                base.RaiseEvent(newEventArgs);
            }

            if (this.FinishButtonClosesWindow)
            {
                Window parentWindow = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
                if (parentWindow != null)
                {
                    try
                    {
                        parentWindow.DialogResult = true;
                    }
                    catch
                    {
                    }

                    parentWindow.Close();
                }
            }
        }

        /// <summary>
        /// Determines whether this instance [can process finish command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanProcessFinishCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.SelectedWizardPage == null)
            {
                e.CanExecute = false;
            }
            else
            {
                //CommandManager.InvalidateRequerySuggested();
                //// the page should overwrite the above setting
                if (this.SelectedWizardPage.FinishEnabled != null)
                {
                    e.CanExecute = this.SelectedWizardPage.FinishEnabled == true;
                }
                else if (this.FinishEnabled != null)
                {
                    e.CanExecute = this.FinishEnabled == true;
                }
                else
                {
                    //// can, if the selected is the last page
                    e.CanExecute = ItemContainerGenerator.IndexFromContainer(this.SelectedWizardPage) == this.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// Processes the help command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessHelpCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(WizardControl.HelpEvent, this);
            base.RaiseEvent(newEventArgs);
        }

        #endregion

        private void setNewPage()
        {
            WizardPage visiblePage = null;
            if (this.SelectedWizardPage != null && (this.SelectedWizardPage.Visibility == Visibility.Collapsed || !this.SelectedWizardPage.IsEnabled))
                this.SelectedWizardPage = null;
            if (this.SelectedWizardPage == null)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    visiblePage = this.Items[i] as WizardPage;
                    if (visiblePage == null)
                        visiblePage = this.ItemContainerGenerator.ContainerFromIndex(i) as WizardPage;
                    if (visiblePage != null && visiblePage.IsEnabled && visiblePage.Visibility == Visibility.Visible)
                    {
                        this.SelectedWizardPage = visiblePage;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Validates the change in selection by firing the <see cref="WizardPage.Unselecting"/>, <see cref="WizardControl.SelectedPageChanging"/>
        /// and the <see cref="WizardPage.Selecting"/> events.
        /// </summary>
        /// <param name="cause">Specifies what caused the change in selection.</param>
        /// <param name="newPage">The new page that is to be selected.</param>
        /// <returns>True if validation succeeds, false otherwise.</returns>
        protected bool ValidateNewPage(WizardPageSelectionChangeCause cause, WizardPage newPage)
        {
            //// Don't validate in design mode.
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return true;
            }

            bool validated = true;
            WizardPage oldPage = this.SelectedWizardPage;
            //// Fire WizardPage.Unselecting
            if (oldPage != null)
            {
                CancelRoutedEventArgs args = new CancelRoutedEventArgs(WizardPage.UnselectingEvent, oldPage);
                oldPage.RaiseEvent(args);
                if (args.Cancel)
                {
                    validated = false;
                }
            }

            if (validated)
            {
                //// Fire WizardControl.SelectedPageChanging
                WizardPageSelectionChangeEventArgs wpargs = new WizardPageSelectionChangeEventArgs(newPage, oldPage, cause, WizardControl.SelectedPageChangingEvent, this);

                if (IsItemSourceChanged != true)
                {
                    this.RaiseEvent(wpargs);
                }

                if (wpargs.Cancel)
                {
                    validated = false;
                }
            }


            if (validated && newPage != null)
            {
                //// Fire WizardPage.Selecting
                CancelRoutedEventArgs e = new CancelRoutedEventArgs(WizardPage.SelectingEvent, newPage);
                newPage.RaiseEvent(e);
                if (e.Cancel)
                {
                    validated = false;
                }
            }

            return validated;
        }
        #region ITEMS_CONTROL_RELATED

        /// <summary>
        /// Handles the CurrentChanged event of the Items control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Items_CurrentChanged(object sender, EventArgs e)
        {
            if (!this.flagChangingCurrentItem)
            {
                this.SelectedWizardPage = ItemContainerGenerator.ContainerFromItem(Items.CurrentItem) as WizardPage;
                IsItemSourceChanged = false;
            }
        }

        /// <summary>
        /// Invoked when the <see cref="E:System.Windows.UIElement.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Return)
            {
                if (this.SelectedWizardPage == ItemContainerGenerator.ContainerFromItem(Items[this.Items.Count - 1]) as WizardPage)
                {
                    this.SelectedWizardPage.NextEnabled = false;
                }
                
                if (this.NextAndFinishAreDefaultButtons == true)
                {
                    if (this.SelectedWizardPage == ItemContainerGenerator.ContainerFromItem(Items[this.Items.Count - 1]) as WizardPage)
                    {
                        if (this.SelectedWizardPage.NextEnabled == false)
                        {
                            if (e.Key == Key.Return)
                            {
                                WizardCommands.Finish.Execute(null, this);
                            }
                        }
                    }
                }
            }

            if (e.Key == Key.Escape)
            {
                if (this.CancelEnabled == true)
                {
                    WizardCommands.Cancel.Execute(null, this);
                }
            }
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WizardPage();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WizardPage;
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.EnsureSelectedPage();
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    //// Ensure the selected page is still a child.
                    this.EnsureValidSelectedPage();
                    break;
                default: break;
            }
        }

        /// <summary>
        /// invoked when changes in the ItemsSource collection.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (CancelSPCEventsOnItemsSourceChanged == true)
            {
                IsItemSourceChanged = true;
            }
        }

        /// <summary>
        /// Ensures the selected page.
        /// </summary>
        private void EnsureSelectedPage()
        {
            if (!this.EnsureDefaultSelection)
            {
                return;
            }

            if (this.SelectedWizardPage == null && this.Items.Count > 0)
            {
                setNewPage();
            }
        }

        /// <summary>
        /// Ensures the valid selected page.
        /// </summary>
        private void EnsureValidSelectedPage()
        {
            if (this.ItemsSource != null)
            {
                if (SelectedWizardPage == null)
                    EnsureSelectedPage();
            }
            if (this.SelectedWizardPage != null)
            {
                //// If the selected is not a child anymore
                if (ItemContainerGenerator.IndexFromContainer(this.SelectedWizardPage) == -1)
                {
                    //// reset the selection.
                    this.SelectedWizardPage = null;
                    //// try to set a default selection.
                    this.EnsureSelectedPage();
                }
            }
            else
            {
                this.EnsureSelectedPage();
            }
        }
        #endregion
    }
}