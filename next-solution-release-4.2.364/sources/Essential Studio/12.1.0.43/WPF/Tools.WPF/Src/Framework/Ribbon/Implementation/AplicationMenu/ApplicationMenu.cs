
// <copyright file="ApplicationMenu.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a ApplicationMenu control.
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
    /// <example><code>public class ApplicationMenu : HeaderedItemsControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:ApplicationMenu Name="menu" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// ApplicationMenu class represents a Ribbon menu popup control that can be displayed above the Ribbon.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a ApplicationMenu in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:ApplicationMenu ApplicationButtonImage="/SampleImages/superman.ico" >
    /// <ribbon:ApplicationMenu.MenuItems>
    /// <TextBlock MinWidth="300" FontWeight="Bold">Recent Documents</TextBlock>
    /// <Separator Padding="0"/>
    /// </ribbon:ApplicationMenu.MenuItems>
    /// <ribbon:ApplicationMenu.ApplicationItems>
    ///         <ribbon:ButtonPanel Margin="5,0,5,0">
    ///             <ribbon:RibbonButton   SizeForm = "Small" Label="Options" SmallIcon="SampleImages/Options.png"/>
    ///         </ribbon:ButtonPanel>
    ///         <ribbon:ButtonPanel>
    ///             <ribbon:RibbonButton SizeForm = "Small" Label="Exit" Command="ApplicationCommands.Close" SmallIcon="SampleImages/Exit.png"/>
    ///         </ribbon:ButtonPanel>
    ///     </ribbon:ApplicationMenu.ApplicationItems>
    ///     <ribbon:SimpleMenuButton Label="New" Icon="/SampleImages/Document32.png"/>
    /// </ribbon:ApplicationMenu>
    /// ]]>
    /// </code>
    /// <para>This example shows how to create a ApplicationMenu in C#.</para>
    /// <code>    
    /// Ribbon ribbon;
    /// RibbonButton button1;
    /// RibbonButton button2;
    /// ApplicationMenu menu = new ApplicationMenu();
    /// menu.MenuItems.Add(button1);
    /// menu.MenuItems.Add(button2);
    /// ribbon.ApplicationMenu = menu;
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ApplicationMenu : HeaderedItemsControl,IDisposable
    {
        #region Private members
        /// <summary>
        /// Time when the last click on title bar was performed.
        /// </summary>
        private DateTime m_lastAppButtonClick;

        /// <summary>
        /// Point where the last click on title bar was performed.
        /// </summary>
        private Point m_lastAppButtonPoint;

        /// <summary>
        /// Grid which contains Application Menu button.
        /// </summary>
        private Grid m_buttonGrid;

        /// <summary>
        /// Visual brush which duplicates button Application menu.
        /// </summary>
        private Grid m_fakeButton;

        /// <summary>
        /// Item's control which contains MenuItems.
        /// </summary>
        private ItemsControl m_menuItemsControl;

        /// <summary>
        /// Items control which stores application items.
        /// </summary>
        private ItemsControl m_appItemsControl;

        /// <summary>
        /// Represents the Item Presenter
        /// </summary>
        private ItemsPresenter m_itemsPresenter;

        /// <summary>
        /// Default fake button render transform.
        /// </summary>
        private Transform m_startTransform;

        /// <summary>
        /// Application Menu Popup.
        /// </summary>
        private Popup m_popup;

        /// <summary>
        /// IsOpen property binding data
        /// </summary>
        private BindingExpression m_isOpenBinding;

        /// <summary>
        /// Defines whether pressed element was captured. 
        /// </summary>
        private bool m_wasPressed;

        /// <summary>
        /// Last captured element.
        /// </summary>
        private object m_presseddElement;

        /// <summary>
        /// Represents the Remove Transform.
        /// </summary>
        private bool m_bRemoveTransform = false;

        /// <summary>
        /// Represents the observable collections MenuItems
        /// </summary>
        private ObservableCollection<object> m_menuItems = new ObservableCollection<object>();

        /// <summary>
        /// Represents the application items
        /// </summary>
        private ObservableCollection<object> m_applicationItems = new ObservableCollection<object>();

        /// <summary>
        /// Represents templateChangingDelayTime
        /// </summary>
        private static TimeSpan templateChangingDelayTime = new TimeSpan(0, 0, 0, 0, 150);

        /// <summary>
        /// Represents the Timer 
        /// </summary>
        private DispatcherTimer timer;

        /// <summary>
        /// represents the menubutton base
        /// </summary>
        private MenuButtonBase prevMenuButton;

        /// <summary>
        /// represents the closekeytip
        /// </summary>
        internal bool closekeytip = false;

        SystemGesture msystemGesture;

        #endregion

        #region Dependency properties

        /// <summary>
        /// Defines PlacementBorder for the ApplicationMenu.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementBorderProperty = DependencyProperty.Register("PlacementBorder", typeof(Border), typeof(ApplicationMenu), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines the selected application menu item.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(MenuButtonBase), typeof(ApplicationMenu), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        /// <summary>
        /// Defines whether Application button is below ApplicationMenu or not.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBelowAppButtonProperty = DependencyProperty.Register("IsBelowAppButton", typeof(bool), typeof(ApplicationMenu), new UIPropertyMetadata(true));

        /// <summary>
        /// Defines whether PopUp is open or not.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.Register("IsPopupOpen", typeof(bool), typeof(ApplicationMenu), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsPopupOpenChanged)));

        /// <summary>
        /// Defines whether key tips are shown or not.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsKeyTipShownProperty =
            DependencyProperty.Register("IsKeyTipShown", typeof(bool), typeof(ApplicationMenu), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets ApplicationButton image.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ApplicationButtonImageProperty =
            DependencyProperty.Register("ApplicationButtonImage", typeof(ImageSource), typeof(ApplicationMenu), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets ApplicationButton image width.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ApplicationButtonImageWidthProperty =
            DependencyProperty.Register("ApplicationButtonImageWidth", typeof(double), typeof(ApplicationMenu), new FrameworkPropertyMetadata(18d));

        /// <summary>
        /// Gets or sets ApplicationButton image height.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ApplicationButtonImageHeightProperty =
            DependencyProperty.Register("ApplicationButtonImageHeight", typeof(double), typeof(ApplicationMenu), new FrameworkPropertyMetadata(18d));

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets PlacementBorder for the ApplicationMenu.
        /// </summary>
        internal Border PlacementBorder
        {
            get
            {
                return (Border)GetValue(PlacementBorderProperty);
            }

            set
            {
                SetValue(PlacementBorderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected application menu item. 
        /// </summary>
        public MenuButtonBase SelectedItem
        {
            get
            {
                return (MenuButtonBase)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value of the MenuItems of the Application Menu
        /// </summary>
        public ObservableCollection<object> internalMenuItems
        {
            get { return (ObservableCollection<object>)GetValue(internalMenuItemsProperty); }
            set { SetValue(internalMenuItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for internalMenuItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty internalMenuItemsProperty =
            DependencyProperty.Register("internalMenuItems", typeof(ObservableCollection<object>), typeof(ApplicationMenu), new PropertyMetadata(new ObservableCollection<object>()));  

        /// <summary>
        /// Gets or sets a value indicating whether this instance is below app button.
        /// </summary>
        public bool IsBelowAppButton
        {
            get
            {
                return (bool)GetValue(IsBelowAppButtonProperty);
            }

            set
            {
                SetValue(IsBelowAppButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is popup open.
        /// </summary>
        public bool IsPopupOpen
        {
            get
            {
                return (bool)GetValue(IsPopupOpenProperty);
            }

            set
            {
                SetValue(IsPopupOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is key tip shown.
        /// </summary>
        [Browsable(false)]
        public bool IsKeyTipShown
        {
            get
            {
                return (bool)GetValue(IsKeyTipShownProperty);
            }

            set
            {
                SetValue(IsKeyTipShownProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ApplicationButton image.
        /// </summary>
        public ImageSource ApplicationButtonImage
        {
            get
            {
                return (ImageSource)GetValue(ApplicationButtonImageProperty);
            }

            set
            {
                SetValue(ApplicationButtonImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ApplicationButton image.
        /// </summary>
        public double ApplicationButtonImageHeight
        {
            get
            {
                return (double)GetValue(ApplicationButtonImageHeightProperty);
            }

            set
            {
                SetValue(ApplicationButtonImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ApplicationButton image.
        /// </summary>
        public double ApplicationButtonImageWidth
        {
            get
            {
                return (double)GetValue(ApplicationButtonImageWidthProperty);
            }

            set
            {
                SetValue(ApplicationButtonImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of MenuItems objects.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<object> MenuItems
        {
            get
            {
                return m_menuItems;
            }
        }

        /// <summary>
        /// Gets the collection of ApplicationItems objects.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<object> ApplicationItems
        {
            get
            {
                return m_applicationItems;
            }
        }

        /// <summary>
        /// Gets the last item.
        /// </summary>
        /// <value>The last item.</value>
        internal FrameworkElement LastItem
        {
            get
            {
                if (Items.Count > 0)
                {
                    return Items[Items.Count - 1] as FrameworkElement;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the first item.
        /// </summary>
        /// <value>The first item.</value>
        internal FrameworkElement FirstItem
        {
            get
            {
                if (Items.Count > 0)
                {
                    return Items[0] as FrameworkElement;
                }
                return null;
            }
        }



        public double PreviewPanelMinWidth
        {
            get { return (double)GetValue(PreviewPanelMinWidthProperty); }
            set { SetValue(PreviewPanelMinWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviewPanelMinWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviewPanelMinWidthProperty =
            DependencyProperty.Register("PreviewPanelMinWidth", typeof(double), typeof(ApplicationMenu), new FrameworkPropertyMetadata(0d));

        
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="ApplicationMenu"/> class.
        /// </summary>
        static ApplicationMenu()
        {
            EnvironmentTest.ValidateLicense(typeof(ApplicationMenu));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApplicationMenu), new FrameworkPropertyMetadata(typeof(ApplicationMenu)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationMenu"/> class.
        /// </summary>
        public ApplicationMenu()
        {
            ToolTipOpening += new ToolTipEventHandler(ApplicationMenu_ToolTipOpening);
        }

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when SelectedItem property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemChanged;

        /// <summary>
        /// Event that is raised when IsPopupOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsPopupOpenChanged;
        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnSelectedItemChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationMenu instance = (ApplicationMenu)d;
            instance.OnSelectedItemChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SelectedItemChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            //MenuButtonBase prevValue = (MenuButtonBase)e.OldValue;
            //MenuButtonBase nextValue = (MenuButtonBase)e.NewValue;
            //if (prevValue == null)
            //{
            //    nextValue.IsMenuOpen = true;
            //}
            //else
            //{
            //    if (nextValue == null)
            //    {
            //        prevValue.IsMenuOpen = false;
            //    }
            //    else
            //    {
            //        if (prevValue != nextValue.Parent)
            //        {
            //            prevValue.IsMenuOpen = false;
            //            nextValue.IsMenuOpen = true;
            //        }
            //        else
            //        {
            //            nextValue.IsMenuOpen = true;
            //        }
            //    }
            //}

            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsPopupOpenChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsPopupOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationMenu instance = (ApplicationMenu)d;
            instance.OnIsPopupOpenChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsPopupOpenChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsPopupOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (m_isOpenBinding == null)
            //{
            if(m_popup!=null)
            {
                m_isOpenBinding = m_popup.GetBindingExpression(Popup.IsOpenProperty);
            }

            //if (m_isOpenBinding.Status != BindingStatus.Detached)
            if(m_isOpenBinding!=null)
            m_isOpenBinding.UpdateTarget();

            if (IsPopupOpen == false|| m_popup==null)
            {
                m_bRemoveTransform = true;
            }
            else
            {
                HwndSource sourceReal = (HwndSource)PresentationSource.FromVisual(m_buttonGrid);
                HwndSource sourceFake = (HwndSource)PresentationSource.FromVisual(m_fakeButton);
                
                if (sourceReal != null && sourceFake != null)
                {
                    Point pointGridScreen = m_buttonGrid.PointToScreen(new Point(0, 0));
                    m_fakeButton.UpdateLayout();
                    Point pointGridInFake = m_fakeButton.PointFromScreen(pointGridScreen);

                    if (pointGridInFake.X != 0d)
                    {
                        TransformGroup group = new TransformGroup();
                        group.Children.Add(new TranslateTransform(pointGridInFake.X, 0));
                        if (FlowDirection == FlowDirection.RightToLeft)
                        {
                            m_startTransform = new TranslateTransform(0, m_startTransform.Value.OffsetY);
                        }

                        group.Children.Add(m_startTransform);
                        m_fakeButton.RenderTransform = group;
                    }
                    else
                    {
                        m_fakeButton.RenderTransform = m_startTransform;
                    }
                }
                Keyboard.Focus(this);
            }

            if (IsPopupOpenChanged != null)
            {
                IsPopupOpenChanged(this, e);
            }
        }

        /// <summary>
        /// Handles the ToolTipOpening event of the ApplicationMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.ToolTipEventArgs"/> instance containing the event data.</param>
        private void ApplicationMenu_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (IsPopupOpen)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Shows the popup.
        /// </summary>
        private void ShowPopup()
        {
            IsPopupOpen = true;
        }

        /// <summary>
        /// Handles the Tick event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            timer = null;
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Invoked whenever application code or internal processes call
        /// ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_menuItemsControl = (ItemsControl)this.GetTemplateChild("PART_MenuItems");
            m_appItemsControl = (ItemsControl)this.GetTemplateChild("PART_AppItems");
            m_itemsPresenter = (ItemsPresenter)this.GetTemplateChild("PART_Items");
            PlacementBorder = (Border)GetTemplateChild("PART_Border");
            m_fakeButton = (Grid)GetTemplateChild("PART_FakeAppButton");
            m_buttonGrid = (Grid)GetTemplateChild("PART_ButtonGrid");
            if(GetTemplateChild("PART_Popup") != null && ( GetTemplateChild("PART_Popup") is Popup || GetTemplateChild("PART_Popup") is CustomPopup))
                m_popup = (Popup)GetTemplateChild("PART_Popup");
            m_startTransform = m_fakeButton.RenderTransform;
            m_fakeButton.RenderTransform = new TranslateTransform(0, 0);
            internalMenuItems = this.MenuItems;
            m_appItemsControl.ItemsSource = this.ApplicationItems;
            if (m_popup != null)
            {
                m_popup.Closed -= new EventHandler(M_Popup_Closed);
                m_popup.PreviewKeyDown -= new KeyEventHandler(M_popup_PreviewKeyDown);
            }
            if (m_itemsPresenter != null)
                m_itemsPresenter.MouseMove -= new MouseEventHandler(M_ItemsPresenter_MouseMove);
            m_popup.Closed += new EventHandler(M_Popup_Closed);
            m_popup.PreviewKeyDown += new KeyEventHandler(M_popup_PreviewKeyDown);
            m_itemsPresenter.MouseMove += new MouseEventHandler(M_ItemsPresenter_MouseMove);
            this.Unloaded -= new RoutedEventHandler(ApplicationMenu_Unloaded);
            this.Unloaded += new RoutedEventHandler(ApplicationMenu_Unloaded);
        }

        void ApplicationMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
        /// <summary>
        /// Handles the key down evnt of Popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M_popup_PreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
                IsPopupOpen = false;
        }

        /// <summary>
        /// Gets the app menu pop up.
        /// </summary>
        /// <value>The app menu pop up.</value>
        internal Popup AppMenuPopUp
        {
            get
            {
                return m_popup;
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the m_itemsPresenter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void M_ItemsPresenter_MouseMove(object sender, MouseEventArgs e)
        {
            Keyboard.Focus(this);
        }

        /// <summary>
        /// Handles the Closed event of the m_popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void M_Popup_Closed(object sender, EventArgs e)
        {
            IsPopupOpen = false;
            if (this.IsFocused)
            {
                Keyboard.Focus(this);
            }

            if (m_bRemoveTransform)
            {
                m_fakeButton.RenderTransform = new TranslateTransform(0, 0);
                m_bRemoveTransform = false;
            }
        }

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            msystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event is
        /// raised on this element. Implement this method to add class
        /// handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the
        /// event data. The event data reports that the
        /// left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice==null)
            {
                base.OnMouseLeftButtonDown(e);

                Point position = e.GetPosition(this);
                if (((DateTime.Now.Subtract(m_lastAppButtonClick).TotalMilliseconds < System.Windows.Forms.SystemInformation.DoubleClickTime) && (Math.Abs((double)(m_lastAppButtonPoint.X - position.X)) <= 2)) && (Math.Abs((double)(m_lastAppButtonPoint.Y - position.Y)) <= 2))
                {
                    Visual v = e.OriginalSource as Visual;

                    if (VisualUtils.FindAncestor(v, VisualUtils.RootPopupType) == null)
                    {
                        RibbonWindow mainWindow = (RibbonWindow)VisualUtils.FindAncestor(this, typeof(RibbonWindow));
                        if (mainWindow != null)
                        {
                            mainWindow.Close();
                        }
                    }
                }
                else
                {
                    m_lastAppButtonPoint = e.GetPosition(this);
                }

                m_lastAppButtonClick = DateTime.Now;

                if (e.OriginalSource is Image || e.OriginalSource is TextBlock || e.OriginalSource is Rectangle || e.OriginalSource is Ellipse || e.OriginalSource.GetType() == VisualUtils.RootPopupType)
                {
                    Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
                    if (ribbon != null && !IsPopupOpen && ribbon.RibbonState == RibbonState.Adorner)
                    {
                        ribbon.HideAdorned();
                        (ribbon.SelectedItem as RibbonTab).m_tabButton.IsChecked = false;
                        ribbon.RibbonState = RibbonState.Hide;

                        timer = new DispatcherTimer(DispatcherPriority.Input);
                        timer.Tick += new EventHandler(Timer_Tick);
                        timer.Interval = templateChangingDelayTime;
                        timer.Start();
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(ShowPopup));
                    }
                    else
                    {
                        IsPopupOpen = !IsPopupOpen;
                        closekeytip = true;
                    }

                    e.Handled = true;
                }
            }
        }

#if !SyncfusionFramework3_5
        protected override void OnTouchDown(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch && msystemGesture == SystemGesture.Tap)
            {
                base.OnTouchDown(e);

                Point position = e.GetTouchPoint(this).Position;
                if (((DateTime.Now.Subtract(m_lastAppButtonClick).TotalMilliseconds < System.Windows.Forms.SystemInformation.DoubleClickTime) && (Math.Abs((double)(m_lastAppButtonPoint.X - position.X)) <= 2)) && (Math.Abs((double)(m_lastAppButtonPoint.Y - position.Y)) <= 2))
                {
                    Visual v = e.OriginalSource as Visual;

                    if (VisualUtils.FindAncestor(v, VisualUtils.RootPopupType) == null)
                    {
                        RibbonWindow mainWindow = (RibbonWindow)VisualUtils.FindAncestor(this, typeof(RibbonWindow));
                        if (mainWindow != null)
                        {
                            mainWindow.Close();
                        }
                    }
                }
                else
                {
                    m_lastAppButtonPoint = e.GetTouchPoint(this).Position;
                }

                m_lastAppButtonClick = DateTime.Now;

                if (e.OriginalSource is Image || e.OriginalSource is TextBlock || e.OriginalSource is Rectangle || e.OriginalSource is Ellipse || e.OriginalSource.GetType() == VisualUtils.RootPopupType)
                {
                    Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
                    if (ribbon != null && !IsPopupOpen && ribbon.RibbonState == RibbonState.Adorner)
                    {
                        ribbon.HideAdorned();
                        (ribbon.SelectedItem as RibbonTab).m_tabButton.IsChecked = false;
                        ribbon.RibbonState = RibbonState.Hide;

                        timer = new DispatcherTimer(DispatcherPriority.Input);
                        timer.Tick += new EventHandler(Timer_Tick);
                        timer.Interval = templateChangingDelayTime;
                        timer.Start();
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(ShowPopup));
                    }
                    else
                    {
                        IsPopupOpen = !IsPopupOpen;
                        closekeytip = true;
                    }

                    e.Handled = true;
                }
            }
        }

#endif

        /// <summary>
        /// Invoked when an unhandled PreviewMouseLeftButtonUp routed
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the
        /// event data. The event data reports that the
        /// left mouse button was released.</param>    
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                base.OnPreviewMouseLeftButtonUp(e);
                if (e.OriginalSource.GetType() == VisualUtils.RootPopupType)
                {
                    e.Handled = true;
                }
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnPreviewTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;

            if (ribbonTouch!=null && ribbonTouch.EnableTouch && msystemGesture == SystemGesture.Tap)
            {
                base.OnPreviewTouchUp(e);

                //if (e.OriginalSource.GetType() == VisualUtils.RootPopupType)
                //{
                    e.Handled = true;
                //}
            }
        }
#endif

        /// <summary>
        /// Returns class-specific AutomationPeer implementations for
        /// the Windows Presentation Foundation (WPF) infrastructure.
        /// </summary>
        /// <returns>
        /// The type-specific AutomationPeer implementation.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ApplicationMenuAutomationPeer(this);
        }

        /// <summary>
        /// Invoked when an unhandled routed event reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            FrameworkElement originalSource = e.OriginalSource as FrameworkElement;
            FrameworkElement source = null;
            
            if (originalSource != null)
            {
                source = originalSource.TemplatedParent as FrameworkElement;
            }

            if (source is SplitMenuButton)
            {
                MenuButtonBase parentmenu = (MenuButtonBase)VisualUtils.FindSomeParent((source as MenuButtonBase), typeof(MenuButtonBase));
                if (parentmenu != null && (source as MenuButtonBase).IsMenuOpen )
                {
                    parentmenu.IsMenuOpen = true;
                }
                if (prevMenuButton != null && prevMenuButton != source && parentmenu !=prevMenuButton)
                {
                    prevMenuButton.IsMenuOpen = false;

                    MenuButtonBase prevParent =
                        (MenuButtonBase) VisualUtils.FindSomeParent(prevMenuButton, typeof (MenuButtonBase));
                    if(prevParent !=null)
                    {
                        prevParent.IsMenuOpen = false;
                    }
                }

                (source as MenuButtonBase).IsMenuOpen = true;

                prevMenuButton = (source as MenuButtonBase);
            }
            else if (source != null && source.Parent is ApplicationMenu)
            {
                if (prevMenuButton != null)
                {
                    prevMenuButton.IsMenuOpen = false;
                }
            }

            base.OnMouseMove(e);
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
                FrameworkElement originalSource = e.OriginalSource as FrameworkElement;
                FrameworkElement source = null;
                if (originalSource != null)
                {
                    source = originalSource.TemplatedParent as FrameworkElement;
                }

                if (source != null && !(source is ApplicationMenu) && !(e.OriginalSource is ScrollViewer))
                {
                    if (!source.IsEnabled)
                    {
                        ContextMenuService.SetShowOnDisabled(source, true);
                    }

                    RibbonContextMenu menu = new RibbonContextMenu();
                    menu.PlacementTarget = source;
                    menu.Closed += delegate(object sender, RoutedEventArgs ea)
                    {
                        source.ContextMenu = null;
                    };

                    source.ContextMenu = menu;

                    if (source is MenuButtonBase)
                    {
                        (source as MenuButtonBase).IsMenuOpen = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch!=null&& ribbonTouch.EnableTouch && msystemGesture == SystemGesture.RightTap)
            {
                FrameworkElement originalSource = e.OriginalSource as FrameworkElement;
                FrameworkElement source = null;
                if (originalSource != null)
                {
                    source = originalSource.TemplatedParent as FrameworkElement;
                }

                if (source != null && !(source is ApplicationMenu) && !(e.OriginalSource is ScrollViewer))
                {
                    if (!source.IsEnabled)
                    {
                        ContextMenuService.SetShowOnDisabled(source, true);
                    }

                    RibbonContextMenu menu = new RibbonContextMenu();
                    menu.PlacementTarget = source;
                    menu.Closed += delegate(object sender, RoutedEventArgs ea)
                    {
                        source.ContextMenu = null;
                    };

                    source.ContextMenu = menu;

                    if (source is MenuButtonBase)
                    {
                        (source as MenuButtonBase).IsMenuOpen = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }

                base.OnTouchUp(e);
            }
        }

#endif

        /// <summary>
        /// Invoked when an unhandled Mouse.LostMouseCapture attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">TheMouseEventArgs that contains event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            m_presseddElement = e.OriginalSource;
            m_wasPressed = false;
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.GotMouseCapture attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event
        /// data.</param>
        protected override void OnGotMouseCapture(MouseEventArgs e)
        {
            base.OnGotMouseCapture(e);
            if (m_wasPressed)
            {
                m_presseddElement = null;
                IsPopupOpen = false;
            }
            else
            {
                if (m_presseddElement != null && (m_presseddElement.GetType() == typeof(ApplicationMenu) || m_presseddElement.ToString() == "System.Windows.Controls.Primitives.PopupRoot"))
                {
                    AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement((e.Source is ItemsPresenter && !(e.OriginalSource is RepeatButton)) ? e.Source as UIElement : e.OriginalSource as UIElement);

                    if (peer != null && peer.IsControlElement())
                    {
                        if (peer is IInvokeProvider)
                        {
                            if (e.OriginalSource is SplitMenuButton)
                            {
                                if ((e.OriginalSource as SplitMenuButton).Command.CanExecute((e.OriginalSource as SplitMenuButton).CommandParameter))
                                {
                                    m_wasPressed = true;
                                }
                            }
                            else
                            {
                                m_wasPressed = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the <see cref="E:System.Windows.UIElement.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape || e.Key == Key.System)
            {
                IsPopupOpen = false;
            }
            if (IsPopupOpen)
            {
                if (null != LastItem)
                {
                    LastItem.KeyDown += new KeyEventHandler(LastItem_KeyDown);
                }
                if (null != FirstItem)
                {
                    FirstItem.KeyDown += new KeyEventHandler(FirstItem_KeyDown);
                }
                switch (e.Key)
                {
                    case Key.Down:
                        {
                            e.Handled = true;

                            if (Items.Count > 0)
                            {
                                FrameworkElement firstItem = Items[0] as FrameworkElement;
                                if (null != firstItem)
                                {
                                    if (!firstItem.IsFocused)
                                    {
                                        firstItem.Focus();
                                    }
                                }
                            }
                            break;
                        }
                    case Key.Left:
                            {
                                e.Handled = true;
                                var menuItems = from FrameworkElement item in this.Items
                                                where item is SplitMenuButton
                                                select item;

                                foreach (SplitMenuButton menu in menuItems)
                                {
                                    if (menu.IsMenuOpen)
                                    {
                                        menu.IsMenuOpen = false;
                                        menu.Focus();
                                        break;
                                    }
                                }
                                break;
                            }
                    case Key.Right:
                            {
                                e.Handled = true;
                                var menuItems = from FrameworkElement item in this.Items
                                                where item is SplitMenuButton
                                                select item;

                                foreach (SplitMenuButton menu in menuItems)
                                {
                                    if (menu.IsFocused)
                                    {
                                        menu.IsMenuOpen = true;
                                        break;
                                    }
                                }
                                break;
                            }
                    case Key.Up:
                            {
                                e.Handled = true;
                                if (Items.Count > 0)
                                {
                                    FrameworkElement lastItem = Items[Items.Count - 1] as FrameworkElement;
                                    if (null != lastItem)
                                    {
                                        lastItem.Focus();
                                    }
                                }
                                break;
                            }
                }
            }

        }

        /// <summary>
        /// Invoked when first item in the ItemsCollection recieved a <see cref="E:System.Windows.UIElement.KeyDown"/> event.
        /// </summary>
        /// <param name="sender">The source</param>
        /// <param name="e">The KeyEventArgs</param>
        private void FirstItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                e.Handled = true;
                if (m_appItemsControl.Items.Count == 0)
                {
                    LastItem.Focus();
                }
                else
                {
                    FrameworkElement lastItem = m_appItemsControl.Items[m_appItemsControl.Items.Count - 1] as FrameworkElement;
                    if (null != lastItem)
                    {
                        lastItem.Focus();
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when first item in the ItemsCollection recieved a <see cref="E:System.Windows.UIElement.KeyDown"/> event.
        /// </summary>
        /// <param name="sender">The source</param>
        /// <param name="e">The KeyEventArgs</param>
        private void LastItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                e.Handled = true;
                if (m_appItemsControl.Items.Count == 0)
                {
                    FirstItem.Focus();
                }
                else
                {
                    FrameworkElement firstItem = m_appItemsControl.Items[0] as FrameworkElement;
                    if (null != firstItem)
                    {
                        firstItem.Focus();
                    }
                }
            }
        }
        #endregion

        public void Dispose()
        {           
           if (m_itemsPresenter != null)
               m_itemsPresenter.MouseMove -= new MouseEventHandler(M_ItemsPresenter_MouseMove);
            this.Unloaded -= new RoutedEventHandler(ApplicationMenu_Unloaded);
        }
    }   
}
