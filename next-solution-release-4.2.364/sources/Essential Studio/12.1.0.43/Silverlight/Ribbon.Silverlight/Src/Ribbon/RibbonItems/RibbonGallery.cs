#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Ribbon gallery visual mode.
    /// </summary>
    public enum RibbonGalleryMode
    {
        /// <summary>
        /// General, no drop down.
        /// </summary>
        General,

        /// <summary>
        /// One row mode with drop down.
        /// </summary>
        Fixed
    }

    /// <summary>
    /// Represents ribbon's gallery control.
    /// </summary>
    [TemplateVisualState(GroupName = "RibbonGalleryMode", Name = "General")]
    [TemplateVisualState(GroupName = "RibbonGalleryMode", Name = "Fixed")]
    public class RibbonGallery :
        RibbonItemsControl, IRibbonControl
    {
        #region Fields

        internal RibbonGalleryPanel itemsPanel = null;
        internal RepeatButton rbScrollUp = null;
        internal RepeatButton rbScrollDown = null;
        private Storyboard sbScrollUp = null;
        private Storyboard sbScrollDown = null;
        internal RibbonDropDownItem dropDownButton = null;
        internal ContentControl generalHost = null;
        internal ContentControl fixedHost = null;
        internal ContentControl popupHost = null;
        internal UIElement resizeThumb = null;
        private bool resizeCaptured = false;
        private IRibbonSelector selector = null;
        private Size dropDownSize;
        internal bool IsResized = false;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonGallery"/> class.
        /// </summary>
        public RibbonGallery()
        {
            this.DefaultStyleKey = typeof(RibbonGallery);

            this.CreateStoryboards();
        }

        #endregion

        #region Dependency properties

        #region ItemWidth

        /// <summary>
        /// Gets or sets the width of the item.
        /// </summary>
        /// <value>The width of the item.</value>
        public double ItemWidth
        {
            get
            {
                return (double)GetValue(ItemWidthProperty);
            }

            set
            {
                SetValue(ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="RibbonGallery.ItemWidth"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
            "ItemWidth", typeof(double), typeof(RibbonGallery), new PropertyMetadata(0.0, new PropertyChangedCallback(OnItemWidthChanged)));

        /// <summary>
        /// Called when [item width changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnItemWidthChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:ItemWidthChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnItemWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            double newVal = (double)e.NewValue;
            if (double.IsNaN(newVal))
            {
                this.ItemWidth = (double)e.OldValue;
                throw new InvalidOperationException("ItemWidth cannot be NaN");
            }

            if (newVal < 1.0)
            {
                this.ItemWidth = (double)e.OldValue;
            }

            this.Measure(this.DesiredSize);
        }

        #endregion

        #region ItemHeight

        /// <summary>
        /// Gets or sets ItemHeight.
        /// </summary>
        public double ItemHeight
        {
            get
            {
                return (double)GetValue(ItemHeightProperty);
            }

            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="RibbonGallery.ItemHeight"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            "ItemHeight", typeof(double), typeof(RibbonGallery), new PropertyMetadata(0.0, new PropertyChangedCallback(OnItemHeightChanged)));

        /// <summary>
        /// Called when [item height changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnItemHeightChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:ItemHeightChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnItemHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            double newVal = (double)e.NewValue;

            if (double.IsNaN(newVal))
            {
                this.ItemHeight = (double)e.OldValue;
                throw new InvalidOperationException("ItemHeight cannot be NaN");
            }

            if (newVal < 1.0)
            {
                this.ItemHeight = (double)e.OldValue;
            }

            this.UpdateAnimations();
            this.InvalidateMeasure();
        }

        #endregion

        #region Footer

        /// <summary>
        /// Gets or sets Footer.
        /// </summary>
        /// <value>The footer.</value>
        public object Footer
        {
            get
            {
                return (object)GetValue(FooterProperty);
            }

            set
            {
                SetValue(FooterProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="RibbonGallery.Footer"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
            "Footer", typeof(object), typeof(RibbonGallery), new PropertyMetadata(null));

        #endregion

        #region FooterTemplate

        /// <summary>
        /// Gets or sets FooterTemplate.
        /// </summary>
        /// <value>The footer template.</value>
        public DataTemplate FooterTemplate
        {
            get
            {
                return (DataTemplate)GetValue(FooterTemplateProperty);
            }

            set
            {
                SetValue(FooterTemplateProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="RibbonGallery.FooterTemplate"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.Register(
            "FooterTemplate", typeof(DataTemplate), typeof(RibbonGallery), new PropertyMetadata(null));

        #endregion

        #region SelectedItem

        /// <summary>
        /// Gets or sets SelectedItem.
        /// </summary>RibbonGalleryItem
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get
            {
                return (object)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="RibbonGallery.SelectedItem"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemChanged;

        /// <summary>
        /// The identifier for the <see cref="RibbonGallery.SelectedItem"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(object), typeof(RibbonGallery), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        /// <summary>
        /// Called when [selected item changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnSelectedItemChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedItemChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            RibbonGalleryItem oldValue = e.OldValue as RibbonGalleryItem;
            RibbonGalleryItem newValue = e.NewValue as RibbonGalleryItem;

            if (oldValue != null)
            {
                oldValue.Checked = false;
            }

            if (newValue != null)
            {
                int index = this.Items.IndexOf(newValue);

                if (index == -1)
                {
                    object content = newValue.Content;

                    if (content != null)
                    {
                        index = this.Items.IndexOf(content);
                    }

                    if (index == -1)
                    {
                        throw new ArgumentException();
                    }
                }

                newValue.Checked = true;
            }

            if (this.SelectedItemChanged != null)
            {
                this.SelectedItemChanged(this, e);
            }
        }

        #endregion

        #region Mode

        /// <summary>
        /// Gets or sets the ribbon's mode.
        /// </summary>
        /// <value><see cref="RibbonGallery"/>'s visual mode.</value>
        public RibbonGalleryMode Mode
        {
            get
            {
                return (RibbonGalleryMode)GetValue(ModeProperty);
            }

            set
            {
                SetValue(ModeProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="RibbonGallery.Mode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof(RibbonGalleryMode), typeof(RibbonGallery), new PropertyMetadata(RibbonGalleryMode.General, new PropertyChangedCallback(OnModeChanged)));

        /// <summary>
        /// Called when [mode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnModeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:ModeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnModeChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateMode();
        }

        /// <summary>
        /// Updates the mode.
        /// </summary>
        private void UpdateMode()
        {
            if (this.fixedHost != null && this.generalHost != null)
            {
                object content = null;
                ContentControl from = null;
                ContentControl to = null;

                switch (this.Mode)
                {
                    case RibbonGalleryMode.General:
                        {
                            from = this.fixedHost;
                            to = this.generalHost;
                            break;
                        }

                    case RibbonGalleryMode.Fixed:
                        {
                            from = this.generalHost;
                            to = this.fixedHost;
                            break;
                        }
                }

                content = from.Content;

                if (content != null)
                {
                    from.Content = null;
                    to.Content = content;
                }

                this.UpdateVisualState();
            }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the state of the visual.
        /// </summary>
        /// <value>The state of the visual.</value>
        internal string VisualState
        {
            get
            {
                return Enum.GetName(typeof(RibbonGalleryMode), this.Mode);
            }
        }

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>The selector.</value>
        internal IRibbonSelector Selector
        {
            get { return this.selector; }
            set { this.selector = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is animation active.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is animation active; otherwise, <c>false</c>.
        /// </value>
        private bool IsAnimationActive
        {
            get
            {
                return this.sbScrollUp.GetCurrentState() == ClockState.Active || this.sbScrollDown.GetCurrentState() == ClockState.Active;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ItemsPresenter presenter = GetTemplateChild("ItemsPresenter") as ItemsPresenter;

            if (presenter != null)
            {
                presenter.Loaded += new RoutedEventHandler(this.OnItemsPresenterLoaded);
            }

            this.generalHost = GetTemplateChild("GeneralHost") as ContentControl;
            this.fixedHost = GetTemplateChild("FixedHost") as ContentControl;
            this.popupHost = GetTemplateChild("PopupHost") as ContentControl;

            this.UpdateMode();
            this.UpdateAnimations();
        }

        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            this.UpdateScrollButtonsState();

            return base.MeasureOverride(availableSize);
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
            return item is RibbonGalleryItem;
        }

        //protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    FrameworkElement realSource = RibbonContextMenu.GetRealSource(this, e);
        //    if (realSource == null) return;
        //    var parentRibbon = VisualUtils.FindAncestor(this, typeof(Ribbon));
        //    var parentRibbonBar = VisualUtils.FindAncestor(this, typeof(RibbonBar));
        //    if (parentRibbonBar != null)
        //    {
        //        ((RibbonBar)parentRibbonBar).IsRightClickNeeded = true;
        //    }
        //    if (parentRibbon != null && parentRibbon is Ribbon && realSource != null)
        //    {
        //        ((Ribbon)parentRibbon).AddQATinContextMenu(realSource);
        //    }
        //    base.OnMouseRightButtonDown(e);
        //}

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonGalleryItem();
        }

        /// <summary>
        /// Called when [item clicked].
        /// </summary>
        /// <param name="item">The item.</param>
        internal override void OnItemClicked(UIElement item)
        {
            base.OnItemClicked(item);

            if (this.Selector != null)
            {
                this.Selector.OnItemClicked(item);
            }

            RibbonGalleryItem rgi = item as RibbonGalleryItem;

            if (rgi != null)
            {
                this.SelectedItem = rgi;

                if (this.dropDownButton != null)
                {
                    this.dropDownButton.IsDropDownOpen = false;

                    this.UpdateSelectedItemScrollPosition();
                }
            }
        }

        /// <summary>
        /// Called when [item selected].
        /// </summary>
        /// <param name="item">The item.</param>
        internal override void OnItemSelected(UIElement item)
        {
            base.OnItemSelected(item);

            if (this.Selector != null)
            {
                this.Selector.OnItemSelected(item);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Updates the state of the scroll buttons.
        /// </summary>
        private void UpdateScrollButtonsState()
        {
            if (this.itemsPanel != null)
            {
                if (this.rbScrollUp != null)
                {
                    this.rbScrollUp.IsEnabled = this.itemsPanel.ScrollPosition > 0.0;
                }

                if (this.rbScrollDown != null)
                {
                    this.rbScrollDown.IsEnabled = this.itemsPanel.ScrollPosition + this.itemsPanel.ActualHeight < this.itemsPanel.Extent &&
                        this.itemsPanel.ScrollPosition + this.ItemHeight < this.itemsPanel.Extent;
                }
            }
        }

        /// <summary>
        /// Called when [extent changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnExtentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateScrollButtonsState();
        }

        /// <summary>
        /// Called when [scroll position changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnScrollPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateScrollButtonsState();
        }

        /// <summary>
        /// Called when [scroll up click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnScrollUpClick(object sender, RoutedEventArgs e)
        {
            if (!this.IsAnimationActive)
            {
                this.sbScrollUp.Begin();
            }
        }

        /// <summary>
        /// Called when [scroll down click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnScrollDownClick(object sender, RoutedEventArgs e)
        {
            if (!this.IsAnimationActive)
            {
                this.sbScrollDown.Begin();
            }
        }

        /// <summary>
        /// Updates the animations.
        /// </summary>
        private void UpdateAnimations()
        {
            this.UpdateAnimation(this.sbScrollUp, -this.ItemHeight);
            this.UpdateAnimation(this.sbScrollDown, this.ItemHeight);
        }

        /// <summary>
        /// Updates the animation.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="scrollStep">The scroll step.</param>
        private void UpdateAnimation(Storyboard sb, double scrollStep)
        {
            if (this.itemsPanel != null && sb != null && sb.Children.Count > 0)
            {
                sb.Stop();

                DoubleAnimation da = sb.Children[0] as DoubleAnimation;

                if (da != null)
                {
                    da.By = scrollStep;

                    Storyboard.SetTarget(da, this.itemsPanel);
                }
            }
        }

        /// <summary>
        /// Called when [items presenter loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnItemsPresenterLoaded(object sender, RoutedEventArgs e)
        {
            if (this.itemsPanel != null)
            {
                this.itemsPanel.Owner = null;
                this.itemsPanel.ExtentChanged -= new PropertyChangedCallback(this.OnExtentChanged);
                this.itemsPanel.ScrollPositionChanged -= new PropertyChangedCallback(this.OnScrollPositionChanged);
            }

            ItemsPresenter presenter = sender as ItemsPresenter;

            if (presenter != null)
            {
                presenter.Loaded -= new RoutedEventHandler(this.OnItemsPresenterLoaded);

                if (VisualTreeHelper.GetChildrenCount(presenter) > 0)
                {
                    this.itemsPanel = VisualTreeHelper.GetChild(presenter, 0) as RibbonGalleryPanel;

                    if (this.itemsPanel != null)
                    {
                        this.itemsPanel.Owner = this;
                        this.itemsPanel.ExtentChanged += new PropertyChangedCallback(this.OnExtentChanged);
                        this.itemsPanel.ScrollPositionChanged += new PropertyChangedCallback(this.OnScrollPositionChanged);

                        this.itemsPanel.InvalidateMeasure();

                        this.SetupDropDown();
                    }
                }
            }

            this.SetupScrollButtons();
            this.UpdateScrollButtonsState();
        }

        /// <summary>
        /// Creates the storyboards.
        /// </summary>
        private void CreateStoryboards()
        {
            this.sbScrollUp = RibbonGallery.CreateStoryboard();
            this.sbScrollDown = RibbonGallery.CreateStoryboard();
        }

        /// <summary>
        /// Creates the storyboard.
        /// </summary>
        /// <returns></returns>
        private static Storyboard CreateStoryboard()
        {
            Storyboard sb = new Storyboard() { SpeedRatio = 2 };
            DoubleAnimation da = new DoubleAnimation();

            Storyboard.SetTargetProperty(da, new PropertyPath("ScrollPosition", new object[] { }));

            sb.Children.Add(da);

            return sb;
        }

        /// <summary>
        /// Setups the scroll buttons.
        /// </summary>
        private void SetupScrollButtons()
        {
            if (this.rbScrollUp != null)
            {
                this.rbScrollUp.Click -= new RoutedEventHandler(this.OnScrollUpClick);
            }

            if (this.rbScrollDown != null)
            {
                this.rbScrollDown.Click -= new RoutedEventHandler(this.OnScrollDownClick);
            }

            this.rbScrollUp = GetTemplateChild("ScrollUpButton") as RepeatButton;
            this.rbScrollDown = GetTemplateChild("ScrollDownButton") as RepeatButton;

            if (this.rbScrollUp != null && this.sbScrollUp != null)
            {
                this.UpdateAnimation(this.sbScrollUp, -this.ItemHeight);

                this.rbScrollUp.Click += new RoutedEventHandler(this.OnScrollUpClick);
            }

            if (this.rbScrollDown != null && this.sbScrollDown != null)
            {
                this.UpdateAnimation(this.sbScrollDown, this.ItemHeight);

                this.rbScrollDown.Click += new RoutedEventHandler(this.OnScrollDownClick);
            }
        }

        /// <summary>
        /// Setups the drop down.
        /// </summary>
        private void SetupDropDown()
        {
            if (this.dropDownButton != null)
            {
                this.SizeChanged -= new SizeChangedEventHandler(this.OnSizeChanged);

                this.dropDownButton.IsDropDownOpenChanged -= new EventHandler(this.OnIsDropDownOpenChanged);
                this.dropDownButton.QueryDropDownBounds -= new EventHandler<BoundsEventArgs>(this.OnQueryButtonDropDownBounds);
            }

            this.dropDownButton = GetTemplateChild("DropDownButton") as RibbonDropDownItem;

            if (this.dropDownButton != null)
            {
                this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);

                this.dropDownButton.IsDropDownOpenChanged += new EventHandler(this.OnIsDropDownOpenChanged);
                this.dropDownButton.QueryDropDownBounds += new EventHandler<BoundsEventArgs>(this.OnQueryButtonDropDownBounds);

                this.SetupDropDownResizing();
            }
        }

        /// <summary>
        /// Setups the drop down resizing.
        /// </summary>
        private void SetupDropDownResizing()
        {
            if (this.resizeThumb != null)
            {
                this.resizeThumb.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OnResizeThumbMouseLeftButtonDown);
                this.resizeThumb.MouseLeftButtonUp -= new MouseButtonEventHandler(this.OnResizeThumbMouseLeftButtonUp);
                this.resizeThumb.MouseMove -= new MouseEventHandler(this.OnResizeThumbMouseMove);
            }

            this.resizeThumb = GetTemplateChild("ResizeThumb") as UIElement;

            if (this.resizeThumb != null)
            {
                this.resizeThumb.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnResizeThumbMouseLeftButtonDown);
                this.resizeThumb.MouseLeftButtonUp += new MouseButtonEventHandler(this.OnResizeThumbMouseLeftButtonUp);
                this.resizeThumb.MouseMove += new MouseEventHandler(this.OnResizeThumbMouseMove);
            }
        }

        /// <summary>
        /// Called when [resize thumb mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnResizeThumbMouseMove(object sender, MouseEventArgs e)
        {
            if (this.resizeCaptured)
            {
                RibbonDropDown dropDown = this.dropDownButton.DropDown;
                Point pt = e.GetPosition(dropDown);

                if (pt.X < this.ActualWidth + 4)
                {
                    pt.X = this.ActualWidth + 4;
                }

                if (pt.Y < this.ActualHeight + 4)
                {
                    pt.Y = this.ActualHeight + 4;
                }
                if (this.popupHost != null && !Double.IsNaN(this.resizeThumb.DesiredSize.Height))
                    this.popupHost.Height = pt.Y - this.resizeThumb.DesiredSize.Height - 4;
                dropDown.SetSize(pt.X, pt.Y);
            }
        }

        /// <summary>
        /// Called when [resize thumb mouse left button up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnResizeThumbMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.resizeCaptured)
            {
                this.resizeCaptured = false;
                this.resizeThumb.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Called when [resize thumb mouse left button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnResizeThumbMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.resizeCaptured = this.resizeThumb.CaptureMouse();

            if (this.resizeCaptured)
            {
                this.resizeThumb.LostMouseCapture += new MouseEventHandler(this.OnResizeThumbLostMouseCapture);
            }
        }

        /// <summary>
        /// Called when [resize thumb lost mouse capture].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnResizeThumbLostMouseCapture(object sender, MouseEventArgs e)
        {
            this.resizeCaptured = false;
            this.resizeThumb.LostMouseCapture -= new MouseEventHandler(this.OnResizeThumbLostMouseCapture);
        }

        /// <summary>
        /// Called when [size changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.dropDownButton.IsDropDownOpen = false;
        }

        /// <summary>
        /// Called when [query button drop down bounds].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.BoundsEventArgs"/> instance containing the event data.</param>
        private void OnQueryButtonDropDownBounds(object sender, BoundsEventArgs e)
        {
            GeneralTransform gt = this.TransformToVisual(sender as UIElement);
            Point pt = gt.Transform(new Point(-2, -2));

            gt = this.dropDownButton.DropDown.TransformToVisual(this.popupHost);
            pt = gt.Transform(pt);

            e.Rect = new Rect(pt.X, pt.Y, this.ActualWidth + 4, 0);
        }

        /// <summary>
        /// Called when [is drop down open changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnIsDropDownOpenChanged(object sender, EventArgs e)
        {
            if (this.fixedHost != null && this.popupHost != null)
            {
                if (this.dropDownButton.IsDropDownOpen)
                {
                    GeneralTransform objTransform = (this).TransformToVisual(Application.Current.RootVisual as UIElement);
                    Point point = objTransform.Transform(new Point(0, 0));
                    double expectedHeight = (Application.Current.Host.Content.ActualHeight) - (point.Y + this.ActualHeight);
                    if (this.itemsPanel != null && this.popupHost.ActualHeight <= 0 && this.Items.Count > 0)
                    {
                        RibbonDropDown dropDown = this.dropDownButton.DropDown as RibbonDropDown;
                        if (this.itemsPanel.Extent > expectedHeight && expectedHeight > 0)
                        {
                            this.popupHost.Height = expectedHeight;
                            if (this.resizeThumb != null)
                                this.dropDownSize.Height = expectedHeight + this.resizeThumb.DesiredSize.Height + 4;
                        }
                    }

                    object content = this.fixedHost.Content;

                    this.fixedHost.Content = null;
                    this.popupHost.Content = content;
                }
                else
                {
                    object content = this.popupHost.Content;

                    this.popupHost.Content = null;
                    this.fixedHost.Content = content;

                    this.UpdateScrollButtonsState();
                }

                if (this.dropDownButton.IsDropDownOpen)
                {
                    if (this.dropDownSize.Width > 0 && this.dropDownSize.Height > 0)
                    {
                        this.dropDownButton.DropDown.SetSize(this.dropDownSize.Width, this.dropDownSize.Height);
                    }
                }
                else
                {
                    this.dropDownSize = this.dropDownButton.DropDown.GetSize();
                }
            }
        }

        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
        private void UpdateVisualState()
        {
            VisualStateManager.GoToState(this, this.VisualState, true);
        }

        /// <summary>
        /// Gets the selected item row pos.
        /// </summary>
        /// <returns></returns>
        private int GetSelectedItemRowPos()
        {
            int rpos = -1;

            if (this.SelectedItem != null)
            {
                int cCol = this.itemsPanel.ColumnCount;
                int iPos = this.Items.IndexOf(this.SelectedItem);

                if (iPos == -1)
                {
                    object content = ((RibbonGalleryItem)this.SelectedItem).Content;

                    if (content != null)
                    {
                        iPos = this.Items.IndexOf(content);
                    }
                }

                if (cCol != 0)
                {
                    rpos = iPos / cCol;
                }
            }

            return rpos;
        }

        /// <summary>
        /// Updates the selected item scroll position.
        /// </summary>
        private void UpdateSelectedItemScrollPosition()
        {
            if (this.SelectedItem != null)
            {
                if (this.Mode == RibbonGalleryMode.Fixed)
                {
                    int itemRow = this.GetSelectedItemRowPos();

                    if (itemRow >= 0)
                    {
                        this.itemsPanel.ScrollPosition = this.ItemHeight * itemRow;
                    }
                }
                else
                {
                    ScrollViewer sv = this.generalHost as ScrollViewer;

                    if (sv != null)
                    {
                        double itemPos = this.ItemHeight * this.GetSelectedItemRowPos();

                        if (itemPos < sv.VerticalOffset)
                        {
                            sv.ScrollToVerticalOffset(itemPos);
                        }
                        else if (itemPos + this.ItemHeight > sv.VerticalOffset + sv.ViewportHeight)
                        {
                            sv.ScrollToVerticalOffset(itemPos - (this.ActualHeight - this.ItemHeight));
                        }
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Panel for layouting collection of <see cref="RibbonGalleryItem"/> wihtin <see cref="RibbonGallery"/>.
    /// </summary>
    public class RibbonGalleryPanel :
        ScrollPanel
    {
        #region Fields

        private RibbonGallery owner;
        private int cols;
        private int rows;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonGalleryPanel"/> class.
        /// </summary>
        public RibbonGalleryPanel()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        internal RibbonGallery Owner
        {
            get
            {
                return this.owner;
            }

            set
            {
                this.owner = value;
            }
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <value>The column count.</value>
        internal int ColumnCount
        {
            get
            {
                return this.cols;
            }
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <value>The row count.</value>
        internal int RowCount
        {
            get
            {
                return this.rows;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            RibbonGallery rg = this.Owner;
            UIElementCollection children = this.Children;
            int count = children.Count;

            if (count > 0 && rg != null)
            {
                Rect rc = new Rect(0.0, -this.ScrollPosition, rg.ItemWidth, rg.ItemHeight);

                for (int r = 0, i = 0; r < this.RowCount; ++r, rc.Y += rc.Height)
                {
                    rc.X = 0.0;

                    for (int c = 0; c < this.ColumnCount && i < count; ++c, ++i, rc.X += rc.Width)
                    {
                        UIElement child = children[i];

                        child.Arrange(rc);
                    }
                }
            }
            else
            {
                finalSize = base.ArrangeOverride(finalSize);
            }

            this.Clip = new RectangleGeometry() { Rect = new Rect(new Point(), finalSize) };

            return finalSize;
        }

        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            UIElementCollection children = this.Children;
            RibbonGallery rg = this.Owner;

            if (children.Count > 0 && rg != null)
            {
                double itemHeight = rg.ItemHeight;
                Size itemSize = new Size(rg.ItemWidth, itemHeight);

                foreach (UIElement item in children)
                {
                    item.Measure(itemSize);
                    Size size = item.DesiredSize;
                }

                if (double.IsInfinity(availableSize.Width))
                {
                    availableSize.Height = itemHeight;
                    availableSize.Width = children.Count * itemSize.Width;
                }

                double cols = Math.Floor(availableSize.Width / itemSize.Width);

                if (cols < 1.0)
                {
                    cols = 1.0;
                }

                double rows = (int)Math.Ceiling(children.Count / cols);

                this.cols = (int)Math.Round(cols);
                this.rows = (int)Math.Round(rows);

                this.Extent = rows * itemHeight;

                double availableHeight = availableSize.Height;

                if (double.IsInfinity(availableHeight))
                {
                    availableHeight = this.Extent;
                }

                if (availableHeight >= this.Extent)
                {
                    this.ScrollPosition = 0.0;
                }

                availableSize.Height = availableHeight;
            }
            else
            {
                availableSize = base.MeasureOverride(availableSize);

                this.Extent = availableSize.Width;
            }

            if (this.ScrollPosition + availableSize.Height > this.Extent)
            {
                this.ScrollPosition = this.Extent - availableSize.Height;
            }

            return availableSize;
        }

        /// <summary>
        /// Raises ScrollPositionChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnScrollPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ScrollPosition + this.ActualHeight <= this.Extent)
            {
                InvalidateMeasure();
                RaiseScrollPositionChangedEvent(e);
            }
            else
            {
                double pos = this.Extent - this.ActualHeight;

                if (pos < 0.0)
                {
                    pos = 0.0;
                }

                this.ScrollPosition = pos;
            }
        }

        #endregion
    }
}
