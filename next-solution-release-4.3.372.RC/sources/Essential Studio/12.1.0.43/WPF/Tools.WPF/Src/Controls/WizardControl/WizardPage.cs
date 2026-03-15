// <copyright file="WizardPage.cs" company="Syncfusion">
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
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Specifies a wizard page that can be added to a <see cref="WizardControl" />.
    /// There are plenty of properties exposed to customize the look and feel of the
    /// page easily.
    /// </summary>
    /// <example>
    ///  <code lang="XAML">
    /// <Window x:Class="WizardControl.Window1"
    ///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///     Title="Window1" Height="300" Width="300"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:WizardControl Name="wizardControl">
    ///             <syncfusion:WizardPage Name="wizardPage"/>
    ///         </syncfusion:WizardControl>
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
    ///             WizardPage wizardPage = new WizardPage();
    ///             wizardControl.Items.Add(wizardPage); 
    ///             this.Content = wizardControl;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class WizardPage : ContentControl
    {
        #region properties

        internal WizardControl ParentWizardControl
        {
            get
            {
                WizardControl parentwizard=this.Parent as WizardControl;

                return parentwizard;
            }
        }

        #endregion

        #region DEPENDENCY PROPERTIES
        /// <summary>
        /// Identifies the <see cref="Title"/> property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(WizardPage), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="Description"/> property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(WizardPage), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="BannerImage"/> property.
        /// </summary>
        public static readonly DependencyProperty BannerImageProperty = DependencyProperty.Register("BannerImage", typeof(ImageSource), typeof(WizardPage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="BannerBackground"/> property.
        /// </summary>
        public static readonly DependencyProperty BannerBackgroundProperty = DependencyProperty.Register("BannerBackground", typeof(Brush), typeof(WizardPage), new FrameworkPropertyMetadata(SystemColors.ControlLightLightBrush, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="PageType"/> property.
        /// </summary>
        public static readonly DependencyProperty PageTypeProperty = DependencyProperty.Register("PageType", typeof(WizardPageType), typeof(WizardPage), new FrameworkPropertyMetadata(WizardPageType.Interior, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="FinishVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty FinishVisibleProperty = DependencyProperty.Register("FinishVisible", typeof(bool?), typeof(WizardPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="NextVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty NextVisibleProperty = DependencyProperty.Register("NextVisible", typeof(bool?), typeof(WizardPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BackVisibleProperty"/> property.
        /// </summary>
        public static readonly DependencyProperty BackVisibleProperty = DependencyProperty.Register("BackVisible", typeof(bool?), typeof(WizardPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CancelVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty CancelVisibleProperty = DependencyProperty.Register("CancelVisible", typeof(bool?), typeof(WizardPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HelpVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty HelpVisibleProperty = DependencyProperty.Register("HelpVisible", typeof(bool?), typeof(WizardPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CancelVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty CancelVisibilityProperty = DependencyProperty.Register("CancelVisibility", typeof(Visibility), typeof(WizardPage), new UIPropertyMetadata(Visibility.Visible));
        
        /// <summary>
        /// Identifies the <see cref="HelpVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty HelpVisibilityProperty = DependencyProperty.Register("HelpVisibility", typeof(Visibility), typeof(WizardPage), new UIPropertyMetadata(Visibility.Visible));
        
        /// <summary>
        /// Identifies the <see cref="BackVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty BackVisibilityProperty = DependencyProperty.Register("BackVisibility", typeof(Visibility), typeof(WizardPage), new UIPropertyMetadata(Visibility.Visible));
        
        /// <summary>
        /// Identifies the <see cref="NextVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty NextVisibilityProperty = DependencyProperty.Register("NextVisibility", typeof(Visibility), typeof(WizardPage), new UIPropertyMetadata(Visibility.Visible));
        
        /// <summary>
        /// Identifies the <see cref="FinishVisibility"/> property.
        /// </summary>
        private static readonly DependencyProperty FinishVisibilityProperty = DependencyProperty.Register("FinishVisibility", typeof(Visibility), typeof(WizardPage), new UIPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="NextEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty NextEnabledProperty = DependencyProperty.Register("NextEnabled", typeof(bool?), typeof(WizardPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BackEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty BackEnabledProperty = DependencyProperty.Register("BackEnabled", typeof(bool?), typeof(WizardPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CancelEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty CancelEnabledProperty = DependencyProperty.Register("CancelEnabled", typeof(bool?), typeof(WizardPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FinishEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty FinishEnabledProperty = DependencyProperty.Register("FinishEnabled", typeof(bool?), typeof(WizardPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="NextPage"/> property.
        /// </summary>
        public static readonly DependencyProperty NextPageProperty = DependencyProperty.Register("NextPage", typeof(WizardPage), typeof(WizardPage), new FrameworkPropertyMetadata(null, NextPage_Changed));

        /// <summary>
        /// Identifies the <see cref="PreviousPage"/> property.
        /// </summary>
        public static readonly DependencyProperty PreviousPageProperty = DependencyProperty.Register("PreviousPage", typeof(WizardPage), typeof(WizardPage), new FrameworkPropertyMetadata(null, PreviousPage_Changed));

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> property.
        /// </summary>
        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(WizardPage), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="Selecting"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectingEvent = EventManager.RegisterRoutedEvent("Selecting", RoutingStrategy.Bubble, typeof(EventHandler<CancelRoutedEventArgs>), typeof(WizardPage));

        /// <summary>
        /// Identifies <see cref="Unselecting"/> routed event.
        /// </summary>
        public static readonly RoutedEvent UnselectingEvent = EventManager.RegisterRoutedEvent("Unselecting", RoutingStrategy.Bubble, typeof(EventHandler<CancelRoutedEventArgs>), typeof(WizardPage));

        /// <summary>
        /// Identifies <see cref="Selected"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WizardPage));

        /// <summary>
        /// Identifies <see cref="Unselected"/> routed event.
        /// </summary>
        public static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble, typeof(EventHandler), typeof(WizardPage));

        public static readonly DependencyProperty BannerImageHeightProperty = DependencyProperty.Register("BannerImageHeight", typeof(double), typeof(WizardPage), new UIPropertyMetadata(double.NaN));

        public static readonly DependencyProperty BannerImageWidthProperty = DependencyProperty.Register("BannerImageWidth", typeof(double), typeof(WizardPage), new UIPropertyMetadata(double.NaN));
        
        #endregion
        #region PROPERTY_CHANGED_EVENTHANDLERS
        /// <summary>
        /// Next's the page_ changed.
        /// </summary>
        /// <param name="o">The o DependencyObject.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void NextPage_Changed(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            WizardPage selectedwizardpage = o as WizardPage;
            if (args.NewValue != null)
            {
                WizardPage nextPage = args.NewValue as WizardPage;

                int nextindex = nextPage.ParentWizardControl.ItemContainerGenerator.IndexFromContainer(nextPage);
                int currentindex = (o as WizardPage).ParentWizardControl.ItemContainerGenerator.IndexFromContainer(o);

                if (nextindex - 1 == currentindex)
                {
                    if (nextPage.PreviousPage != o as WizardPage)
                    {
                        nextPage.PreviousPage = o as WizardPage;
                    }
                }

                WizardControl control = selectedwizardpage.ParentWizardControl;
                if (control != null)
                {
                    if (!control.Items.Contains(nextPage))
                    {
                        control.Items.Add(nextPage);
                    }
                }
            }
        }
        
        /// <summary>
        /// Previous the page_ changed.
        /// </summary>
        /// <param name="o">The o DependencyObject.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void PreviousPage_Changed(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                WizardPage prevPage = args.NewValue as WizardPage;

                int previousindex = prevPage.ParentWizardControl.ItemContainerGenerator.IndexFromContainer(prevPage);
                int currentindex = (o as WizardPage).ParentWizardControl.ItemContainerGenerator.IndexFromContainer(o);

                if (previousindex + 1 == currentindex)
                {
                    if (prevPage.NextPage != o as WizardPage)
                    {
                        prevPage.NextPage = o as WizardPage;
                    }
                }
            }
        }
        #endregion
        #region PROPERTIES
        /// <summary>
        /// Gets the cancel visibility.
        /// </summary>
        /// <value>The cancel visibility.</value>
        public Visibility CancelVisibility
        {
            get 
            { 
                return (Visibility)GetValue(CancelVisibilityProperty); 
            }

            internal set 
            { 
                SetValue(CancelVisibilityProperty, value); 
            }
        }

        /// <summary>
        /// Gets the help visibility.
        /// </summary>
        /// <value>The help visibility.</value>
        public Visibility HelpVisibility
        {
            get 
            { 
                return (Visibility)GetValue(HelpVisibilityProperty); 
            }

            internal set 
            { 
                SetValue(HelpVisibilityProperty, value); 
            }
        }

        /// <summary>
        /// Gets the back visibility.
        /// </summary>
        /// <value>The back visibility.</value>
        public Visibility BackVisibility
        {
            get 
            { 
                return (Visibility)GetValue(BackVisibilityProperty); 
            }

            internal set 
            { 
                SetValue(BackVisibilityProperty, value); 
            }
        }

        /// <summary>
        /// Gets the next visibility.
        /// </summary>
        /// <value>The next visibility.</value>
        public Visibility NextVisibility
        {
            get 
            { 
                return (Visibility)GetValue(NextVisibilityProperty); 
            }

            internal set 
            { 
                SetValue(NextVisibilityProperty, value); 
            }
        }

        /// <summary>
        /// Gets the finish visibility.
        /// </summary>
        /// <value>The finish visibility.</value>
        public Visibility FinishVisibility
        {
            get 
            { 
                return (Visibility)GetValue(FinishVisibilityProperty); 
            }

            internal set 
            { 
                SetValue(FinishVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get 
            { 
                return (string)GetValue(TitleProperty); 
            }

            set 
            { 
                SetValue(TitleProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get 
            { 
                return (string)GetValue(DescriptionProperty);
            }

            set 
            { 
                SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the banner image.
        /// </summary>
        /// <value>The banner image.</value>
        public ImageSource BannerImage
        {
            get 
            { 
                return (ImageSource)GetValue(BannerImageProperty); 
            }

            set 
            { 
                SetValue(BannerImageProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the banner background.
        /// </summary>
        /// <value>The banner background.</value>
        public Brush BannerBackground
        {
            get 
            { 
                return (Brush)GetValue(BannerBackgroundProperty);
            }

            set 
            { 
                SetValue(BannerBackgroundProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the type of the page.
        /// </summary>
        /// <value>The type of the page.</value>
        public WizardPageType PageType
        {
            get 
            { 
                return (WizardPageType)GetValue(PageTypeProperty); 
            }

            set 
            { 
                SetValue(PageTypeProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the help visible.
        /// </summary>
        /// <value>The help visible.</value>
        public bool? HelpVisible
        {
            get 
            { 
                return (bool?)GetValue(HelpVisibleProperty); 
            }

            set 
            { 
                SetValue(HelpVisibleProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the finish visible.
        /// </summary>
        /// <value>The finish visible.</value>
        public bool? FinishVisible
        {
            get 
            { 
                return (bool?)GetValue(FinishVisibleProperty); 
            }

            set 
            { 
                SetValue(FinishVisibleProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the next visible.
        /// </summary>
        /// <value>The next visible.</value>
        public bool? NextVisible
        {
            get 
            { 
                return (bool?)GetValue(NextVisibleProperty); 
            }

            set 
            { 
                SetValue(NextVisibleProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the back visible.
        /// </summary>
        /// <value>The back visible.</value>
        public bool? BackVisible
        {
            get 
            { 
                return (bool?)GetValue(BackVisibleProperty); 
            }

            set 
            { 
                SetValue(BackVisibleProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the cancel visible.
        /// </summary>
        /// <value>The cancel visible.</value>
        public bool? CancelVisible
        {
            get 
            { 
                return (bool?)GetValue(CancelVisibleProperty); 
            }

            set 
            { 
                SetValue(CancelVisibleProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the finish enabled.
        /// </summary>
        /// <value>The finish enabled.</value>
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
        /// Gets or sets the cancel enabled.
        /// </summary>
        /// <value>The cancel enabled.</value>
        public bool? CancelEnabled
        {
            get 
            { 
                return (bool?)GetValue(CancelEnabledProperty);
            }

            set
            { 
                SetValue(CancelEnabledProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the back enabled.
        /// </summary>
        /// <value>The back enabled.</value>
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
        /// Gets or sets the next page.
        /// </summary>
        /// <value>The next page.</value>
        public WizardPage NextPage
        {
            get 
            { 
                return (WizardPage)GetValue(NextPageProperty); 
            }

            set 
            { 
                SetValue(NextPageProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the previous page.
        /// </summary>
        /// <value>The previous page.</value>
        public WizardPage PreviousPage
        {
            get 
            { 
                return (WizardPage)GetValue(PreviousPageProperty); 
            }

            set 
            { 
                SetValue(PreviousPageProperty, value); 
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// true if this instance is selected; otherwise, false.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
        
        public double BannerImageHeight 
        { 
            get
            {
                return (double)GetValue(BannerImageHeightProperty);
            }
            set
            {
                SetValue(BannerImageHeightProperty, value);
            }
        }

        public double BannerImageWidth
        {
            get
            {
                return (double)GetValue(BannerImageWidthProperty);
            }
            set
            {
                SetValue(BannerImageWidthProperty, value);
            }
        }
        #endregion
        #region EVENTS
        /// <summary>
        /// Cancellable bubbling routed event, fired when this page is about to be replaced by another page as the selected page.
        /// </summary>
        public event EventHandler<CancelRoutedEventArgs> Selecting
        {
            add
            {
                AddHandler(SelectingEvent, value);
            }

            remove
            {
                RemoveHandler(SelectingEvent, value);
            }
        }
        
        /// <summary>
        /// Cancellable bubbling routed event, fired before unselecting this page.
        /// </summary>
        public event EventHandler<CancelRoutedEventArgs> Unselecting
        {
            add
            {
                AddHandler(UnselectingEvent, value);
            }

            remove
            {
                RemoveHandler(UnselectingEvent, value);
            }
        }
        
        /// <summary>
        /// Bubbling routed event, fired when this page just became the selected page.
        /// </summary>
        public event RoutedEventHandler Selected
        {
            add
            {
                AddHandler(SelectedEvent, value);
            }

            remove
            {
                RemoveHandler(SelectedEvent, value);
            }
        }
        
        /// <summary>
        /// Bubbling routed event, fired when this page just became unselected.
        /// </summary>
        public event RoutedEventHandler Unselected
        {
            add
            {
                AddHandler(UnselectedEvent, value);
            }

            remove
            {
                RemoveHandler(UnselectedEvent, value);
            }
        }
        #endregion
        
        /// <summary>
        /// Initializes static members of the <see cref="WizardPage"/> class.
        /// </summary>
        static WizardPage()
        {
            EnvironmentTest.ValidateLicense(typeof(WizardPage));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardPage), new FrameworkPropertyMetadata(typeof(WizardPage)));
        }
    }
    
    /// <summary>
    /// Enumerates the different types of <see cref="WizardPage"/>s supported.
    /// </summary>
    public enum WizardPageType
    {
        /// <summary>
        /// No banner will be displayed.
        /// </summary>
        Blank,
        
        /// <summary>
        /// A top-banner with title, description and a image will be displayed.
        /// </summary>
        Interior,
        
        /// <summary>
        /// A left-banner with an image will be displayed. The title and description will be displayed to the right.
        /// </summary>
        Exterior
    }
}
