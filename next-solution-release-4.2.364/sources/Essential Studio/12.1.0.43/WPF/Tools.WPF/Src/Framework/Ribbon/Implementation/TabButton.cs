// <copyright file="TabButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

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
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is responsible for Tab button of RibbonTab.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TabButton : ToggleButton
    {
        #region Private Members
        /// <summary>
        /// Canvas for drawing tab button.
        /// </summary>
        private Canvas m_canvas;

        /// <summary>
        /// Clip border for capture.
        /// </summary>
        private Border m_clipBorder;

        /// <summary>
        /// Capture for tab button.
        /// </summary>
        internal TextBlock m_caption;

        /// <summary>
        /// Content of tab button.
        /// </summary>
        internal ContentPresenter m_content;

        /// <summary>
        /// Margin for tab button.
        /// </summary>
        public double M_margin;

        /// <summary>
        /// Tab button parent.
        /// </summary>
        internal Ribbon m_ribbonParent;

        /// <summary>
        /// Arrange the width
        /// </summary>
        public double M_arrangeWidth = 0;

        /// <summary>
        /// Represents the checked tab count
        /// </summary>
        private static int checkedTabCount = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the internal visual parent.
        /// </summary>
        /// <value>
        /// The internal visual parent.
        /// </value>
        internal DependencyObject InternalVisualParent
        {
            get
            {
                return this.VisualParent;
            }
        }

        /// <summary>
        /// Gets or sets button label text.
        /// </summary>
        public string Caption
        {
            get
            {
                return (string)GetValue(CaptionProperty);
            }

            set
            {
                SetValue(CaptionProperty, value);
            }
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies button label text. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionProperty =
                DependencyProperty.Register("Caption", typeof(string), typeof(TabButton), new FrameworkPropertyMetadata(" ", new PropertyChangedCallback(OnCaptionChanged)));
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TabButton"/> class.
        /// </summary>
        static TabButton()
        {
            EnvironmentTest.ValidateLicense(typeof(TabButton));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabButton), new FrameworkPropertyMetadata(typeof(TabButton)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabButton"/> class.
        /// </summary>
        public TabButton()
        {
            Initialize();
            WindowChrome.SetIsHitTestVisibleInChrome(this, true);
            OnClickTimer = new System.Windows.Threading.DispatcherTimer();
            OnClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            OnClickTimer.Tick += new EventHandler(OnClickTimer_Tick);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.SnapsToDevicePixels = true;
            m_canvas = new Canvas();
            m_canvas.SnapsToDevicePixels = true;
            m_canvas.Height = 23;
            m_canvas.MinHeight = 23;
            m_canvas.MaxHeight = 23;

            m_caption = new TextBlock();
            m_caption.HorizontalAlignment = HorizontalAlignment.Left;
            m_caption.VerticalAlignment = VerticalAlignment.Top;

            m_clipBorder = new Border();
            m_clipBorder.Child = m_caption;
            m_clipBorder.Opacity = 0;

            m_canvas.Children.Add(m_clipBorder);
            Canvas.SetLeft(m_clipBorder, 8);
            Canvas.SetTop(m_clipBorder, 5);

            this.AddVisualChild(m_canvas);
        }

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Caption property is changed.
        /// </summary>
        public event PropertyChangedCallback CaptionChanged;
        #endregion

        #region Static Methods
        /// <summary>
        /// Calls OnCaptionChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabButton instance = (TabButton)d;
            instance.OnCaptionChanged(e);
        }
        #endregion

        #region Override methods
        /// <summary>
        /// When overridden in a derived class, is invoked whenever
        /// application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_caption = GetTemplateChild("PART_Caption") as TextBlock;
            m_content = GetTemplateChild("PART_CaptionContent") as ContentPresenter;
            if (m_content != null)
            {
                if (m_ribbonParent != null && m_ribbonParent.ItemsSource != null && m_ribbonParent.DisplayMemberPath != string.Empty)
                {
                    m_content.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Updates property value cache and raises CaptionChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnCaptionChanged(DependencyPropertyChangedEventArgs e)
        {
            #if SyncfusionFramework3_5
            m_caption.Text = (string)e.NewValue;

#else
            m_caption.SetCurrentValue(TabButton.CaptionProperty, (string)e.NewValue);
#endif

            if (CaptionChanged != null)
            {
                CaptionChanged(this, e);
            }
        }


        /// <summary>
        /// Called when a <see cref="T:System.Windows.Controls.Primitives.ToggleButton" />
        /// raises a <see cref="E:System.Windows.Controls.Primitives.ToggleButton.Checked" />
        /// event.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.Primitives.ToggleButton.Checked" />
        /// event.</param>
        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);

            if (this.VisualParent != null)
            {
                if (IsChecked.Value)
                {
                    for (int i = 0, len = m_ribbonParent.Items.Count; i < len; i++)
                    {
                        RibbonTab ribbonTab = m_ribbonParent.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab;                       

                        if (ribbonTab != null)
                        {
                            if (m_ribbonParent.ItemsSource == null)
                            {
                                if (this != ribbonTab.m_tabButton)
                                {
                                    ribbonTab.IsChecked = false;
                                }
                                else
                                {
                                    ribbonTab.IsChecked = true;
                                    m_ribbonParent.SelectedIndex = i;
                                    if (ribbonTab.HasContextTabGroup)
                                    {
                                        m_ribbonParent.SelectedTabItem.HasContextTabGroup = true;
                                        m_ribbonParent.IsContextTabChecked = true;
                                        m_ribbonParent.SelectedContextTabGroupBackColor = m_ribbonParent.SelectedTabItem.ContextTabGroup.BackColor;

                                    }
                                    else
                                    {
                                        m_ribbonParent.IsContextTabChecked = false;

                                    }
                                }
                            }

                            if (m_ribbonParent.ItemsSource != null)
                            {
                                if (selectedtabitem == null)
                                {
                                    if (this != ribbonTab.m_tabButton)
                                    {
                                        ribbonTab.IsChecked = false;
                                    }
                                    else
                                    {
                                        ribbonTab.IsChecked = true;
                                        m_ribbonParent.SelectedIndex = i;
                                    }
                                }

                                if (selectedtabitem != null)
                                {
                                    if (selectedtabitem.Caption != ribbonTab.m_tabButton.Caption)
                                    {
                                        ribbonTab.IsChecked = false;
                                    }
                                    else
                                    {
                                        ribbonTab.IsChecked = true;
                                        m_ribbonParent.SelectedIndex = i;
                                    }
                                }
                            }
                        }
                        else
                        {
                            RibbonTab ribbonTab1 = m_ribbonParent.Items[i] as RibbonTab;
                            if (ribbonTab1 != null)
                            {
                                if (this != ribbonTab1.m_tabButton)
                                {
                                    ribbonTab1.IsChecked = false;
                                }
                                else
                                {
                                    ribbonTab1.IsChecked = true;
                                    m_ribbonParent.SelectedIndex = i;
                                }
                            }
                        }
                    }
                }

                if ((this.VisualParent as RibbonTab) != null && (this.VisualParent as RibbonTab).IsChecked != IsChecked)
                {
                    (this.VisualParent as RibbonTab).IsChecked = IsChecked.Value;
                }
            }
        }


        static int ContextTabCount = 0;

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">Measurement constraints, a control
        /// cannot return a size larger than
        /// the constraint.</param>
        /// <returns>
        /// The size of the control.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            try
            {
                double textWidth;
                double textBlockWidth = 0;
                bool isInDesignMode = (bool)DesignerProperties.GetIsInDesignMode(this);
                base.MeasureOverride(constraint);
                m_canvas.Measure(constraint);
                if (m_ribbonParent != null && m_ribbonParent.ItemTemplate != null && DataContext != null)
                {
                    if (m_content.ContentTemplate == null)
                    {
                        ContentPresenter temp_presenter = new ContentPresenter();
                        temp_presenter.Content = this.DataContext;
                        temp_presenter.ContentTemplate = m_ribbonParent.ItemTemplate;
                        temp_presenter.Measure(constraint);
                        textWidth = temp_presenter.DesiredSize.Width;
                    }
                    else
                    {
                        m_content.Measure(constraint);
                        textWidth = m_content.DesiredSize.Width;
                    }
                }
                else
                {
                    if (m_ribbonParent != null)
                    {
                        TemplatedAdornerInternalControl adorner = VisualUtils.FindDescendant(m_ribbonParent, typeof(TemplatedAdornerInternalControl)) as TemplatedAdornerInternalControl;
                        string caption = "";
                        bool isTabPresent = false;
                        bool isSingleTab = false;
                        RibbonTab currentTab = VisualUtils.FindAncestor(this, typeof(RibbonTab)) as RibbonTab;

                        if (adorner != null)
                        {
                            foreach (ContextTabGroup item in m_ribbonParent.ContextTabGroups)
                            {
                                if (item.RibbonTabs.Contains(currentTab))
                                {
                                    isTabPresent = true;

                                }

                            }

                            if (m_ribbonParent != null && m_ribbonParent.Items.Count > 0 && m_ribbonParent.Items[0] is RibbonTab)
                            {
                                if ((m_ribbonParent.Items[0] as RibbonTab).Caption == currentTab.Caption)
                                    ContextTabCount = 0;
                            }
                            int tempCount = 0;
                            if (m_ribbonParent.ContextTabGroups.Count > ContextTabCount)
                            {
                                foreach (var tab in m_ribbonParent.ContextTabGroups[ContextTabCount].RibbonTabs)
                                {
                                    if (tab is RibbonTab)
                                    {
                                        if (tab.Visibility == Visibility.Visible)
                                        {
                                            tempCount++;
                                            if (tempCount > 1)
                                                isSingleTab = false;
                                            else
                                            {
                                                isSingleTab = true;
                                                caption = tab.Caption;
                                            }
                                        }
                                    }
                                }

                                if (isTabPresent)
                                    ContextTabCount++;

                            }

                            if (isTabPresent && isSingleTab && caption == this.Caption)
                            {
                                var txtblock = adorner.Template.FindName("PART_Label", adorner) as TextBlock;
                                if (txtblock != null)
                                    textBlockWidth = txtblock.ActualWidth;
                            }
                        }
                    }
                    m_caption.Measure(constraint);

                    if (textBlockWidth < m_caption.DesiredSize.Width)
                        textBlockWidth = 0;

                    textWidth = m_caption.DesiredSize.Width + textBlockWidth;
                }


                m_canvas.MaxWidth = textWidth + 45;

                if (M_arrangeWidth == 0)
                {
                    M_arrangeWidth = textWidth + 15;
                }

                if (isInDesignMode && (double.IsPositiveInfinity(constraint.Height) || double.IsNegativeInfinity(constraint.Height) || double.IsInfinity(constraint.Height)))
                {
                    constraint.Height = 23;
                }
                this.MaxWidth = M_arrangeWidth + 30;

                return new Size(M_arrangeWidth, constraint.Height);
            }
            catch
            {
                return new Size(1, 1);
            }
        }

        /// <summary>
        /// Called to arrange and size the content of a <see cref="T:System.Windows.Controls.Control" />
        /// object.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used
        /// to arrange the content.</param>
        /// <returns>
        /// The size of the control.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            try
            {
                double textWidth;
                if (m_ribbonParent != null && m_ribbonParent.ItemTemplate != null)
                {
                    if (m_content.DesiredSize.Width == 0)
                    {
                        m_content.Measure(arrangeBounds);
                    }

                    textWidth = m_content.DesiredSize.Width;
                }
                else
                {
                    if (m_caption.DesiredSize.Width == 0)
                    {
                        m_caption.Measure(arrangeBounds);
                    }

                    textWidth = m_caption.DesiredSize.Width;
                }
                m_canvas.Arrange(new Rect(new Point(0, 0), arrangeBounds));
                m_clipBorder.Width = Math.Abs(m_canvas.ActualWidth - 16);

                M_margin = (arrangeBounds.Width - (textWidth + 6)) / 2;
                if (M_margin > 5)
                {
                    m_caption.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else
                {
                    m_caption.HorizontalAlignment = HorizontalAlignment.Left;
                }


                return base.ArrangeOverride(new Size(m_canvas.ActualWidth, m_canvas.ActualHeight));
            }
            catch
            {
                return base.ArrangeOverride(new Size(1, 1));
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown" />attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" />that
        /// contains the event data. This event data
        /// reports details about the mouse button that
        /// was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch != null && !ribbonTouch.EnableTouch))
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    isDoubleClick = false;
                    clickTime = new TimeSpan();
                    OnClickTimer.Start();
                }

                base.OnMouseDown(e);
                m_ribbonParent.Focus();
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnTouchDown(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch)
            {
                isDoubleClick = false;
                clickTime = new TimeSpan();
                OnClickTimer.Start();

                base.OnTouchDown(e);
                m_ribbonParent.Focus();
            }
        }
        #endif

        void OnClickTimer_Tick(object sender, EventArgs e)
        {
            if (isDoubleClick)
            {
                OnClickTimer.Stop();
                isDoubleClick = false;
                return;
            }
            if (clickTime == new TimeSpan(0, 0, 0, 0, 10))
            {
                OnMouseSingleClick();
                OnClickTimer.Stop();
            }
            clickTime += new TimeSpan(0, 0, 0, 0, 10);
        }

        private TimeSpan clickTime = new TimeSpan();
        private bool isDoubleClick = false;
        private System.Windows.Threading.DispatcherTimer OnClickTimer;
        private int currentSelectedIndex = 0;
        internal RibbonTab selectedtabitem;
        private void OnMouseSingleClick()
        {
            //if (currentSelectedIndex == m_ribbonParent.SelectedIndex)
            // {
            currentSelectedIndex = m_ribbonParent.SelectedIndex;
            if (m_ribbonParent.RibbonState == RibbonState.Hide)
            {
                if (m_ribbonParent.ItemsSource != null)
                {
                    selectedtabitem = ((RibbonTab)VisualUtils.FindAncestor(this, typeof(RibbonTab)));
                    if (selectedtabitem != null)
                    {
                        if (this.VisualParent is RibbonTab && ((RibbonTab)this.VisualParent).IsCancelRibbonState == false && !m_ribbonParent.m_cancelAdornerState)
                        {
                            IsChecked = true;
                            m_ribbonParent.RibbonState = RibbonState.Adorner;
                        }
                    }
                }
                if (m_ribbonParent.ItemsSource == null)
                {
                    if (this.VisualParent is RibbonTab && ((RibbonTab)this.VisualParent).IsCancelRibbonState == false && !m_ribbonParent.m_cancelAdornerState)
                        m_ribbonParent.RibbonState = RibbonState.Adorner;
                }
            }
            else
            {
                RibbonTab tab = m_ribbonParent.ItemContainerGenerator.ContainerFromItem(m_ribbonParent.SelectedItem) as RibbonTab;

                if (tab != null)
                {
                    if (m_ribbonParent.RibbonState == RibbonState.Adorner && tab != null && tab.Equals(this.VisualParent))
                    {
                        m_ribbonParent.RibbonState = RibbonState.Hide;
                        if (this.VisualParent is RibbonTab && ((RibbonTab)this.VisualParent).IsCancelRibbonState == false)
                            IsChecked = false;
                    }
                }
                else
                {
                    if (m_ribbonParent.SelectedIndex > 0 && m_ribbonParent.SelectedIndex < m_ribbonParent.Items.Count)
                    {
                        RibbonTab tab1 = m_ribbonParent.Items[m_ribbonParent.SelectedIndex] as RibbonTab;
                        if (tab1 != null)
                        {
                            if (m_ribbonParent.RibbonState == RibbonState.Adorner && tab1 != null && tab1.Equals(this.VisualParent))
                            {
                                m_ribbonParent.HideAdorned();
                                if (this.VisualParent is RibbonTab && ((RibbonTab)this.VisualParent).IsCancelRibbonState == false)
                                    m_ribbonParent.RibbonState = RibbonState.Hide;
                                IsChecked = false;
                            }
                        }
                    }
                }
            }

            if (m_ribbonParent.RibbonState != RibbonState.Hide)
            {
                IsChecked = true;
            }
            if (m_ribbonParent.m_cancelAdornerState && m_ribbonParent.RibbonState == RibbonState.Hide)
            {
                m_ribbonParent.m_cancelAdornerState = false;
            }
            //}
        }

        /// <summary>
        /// Updates property value cache and raises Click event.
        /// </summary>
        protected override void OnClick()
        {
            bool ischecked = IsChecked.Value;
            base.OnClick();
            IsChecked = ischecked;
            if (m_ribbonParent.BackStageButton != null && m_ribbonParent.BackStageButton.IsOpen)
                m_ribbonParent.BackStageButton.IsOpen = false;
        }

        /// <summary>
        /// Invoked when the parent of this element in the visual tree is
        /// changed. Overrides <see cref="M:System.Windows.UIElement.OnVisualParentChanged(System.Windows.DependencyObject)" />.
        /// </summary>
        /// <param name="oldParent">The old parent element. May be null
        /// to indicate that the element did not
        /// have a visual parent previously.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {

            base.OnVisualParentChanged(oldParent);

            if (m_ribbonParent == null)
            {
                m_ribbonParent = (this.VisualParent as RibbonTab).Parent as Ribbon;
                if (m_ribbonParent == null)
                    m_ribbonParent = VisualUtils.FindAncestor(this as Visual, typeof(Ribbon)) as Ribbon;
            }
            if (m_ribbonParent != null)
            {
                for (int i = 0, len = m_ribbonParent.Items.Count; i < len; i++)
                {
                    RibbonTab ribbonTab = m_ribbonParent.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab;
                    if (ribbonTab != null && ribbonTab.IsChecked)
                    {
                        checkedTabCount++;
                        m_ribbonParent.SelectedIndex = i;
                    }
                    //else if ((m_ribbonParent.Items[i] as RibbonTab).IsChecked)
                    //{
                    //    //(m_ribbonParent.Items[i] as RibbonTab).IsChecked = true;  
                    //    checkedTabCount++;
                    //    m_ribbonParent.SelectedIndex = i;

                    //}

                }

                if (checkedTabCount == 0 && m_ribbonParent.Items.Count > 1)
                {
                    RibbonTab checkedribbonTab = m_ribbonParent.ItemContainerGenerator.ContainerFromIndex(0) as RibbonTab;
                    if (checkedribbonTab != null)
                    {
                        if (checkedribbonTab.IsChecked)
                            checkedribbonTab.IsChecked = true;
                        else
                            checkedribbonTab.IsChecked = false;
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick" />event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            isDoubleClick = true;
            if (OnClickTimer != null) OnClickTimer.Stop();
            if (this.VisualParent is RibbonTab && ((RibbonTab)this.VisualParent).IsCancelRibbonState == true) return;
            if (m_ribbonParent.RibbonState == RibbonState.Normal)
            {
                m_ribbonParent.RibbonState = RibbonState.Hide;
                IsChecked = false;
            }
            else if (m_ribbonParent.RibbonState == RibbonState.Adorner)
            {
                m_ribbonParent.HideAdorned();
                m_ribbonParent.RibbonState = RibbonState.Normal;
                IsChecked = true;
            }
            else
            {
                m_ribbonParent.RibbonState = RibbonState.Normal;
                IsChecked = true;
            }

            e.Handled = true;
            base.OnMouseDoubleClick(e);
        }
        #endregion

        /// <summary>
        /// Adds the canvas.
        /// </summary>
        public void AddCanvas()
        {
            this.AddLogicalChild(m_canvas);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TabButtonAutomationPeer(this);
        }
    }

}
