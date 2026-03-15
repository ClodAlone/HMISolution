// <copyright file="TabPanelAdv.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents tab panel for TabControlExt
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TabPanelAdv : ContentControl
    {
        #region Private members

        /// <summary>
        /// Stores the scroll information.
        /// </summary>
        private Syncfusion.Windows.Tools.Controls.TabLayoutPanel.ScrollInfo m_scrollInfo;

        /// <summary>
        /// Button for Previous tab.
        /// </summary>
        private Button m_prevTab;

        /// <summary>
        /// Button for Next tab.
        /// </summary>
        private Button m_nextTab;

        /// <summary>
        /// Button for Previous page.
        /// </summary>
        private Button m_prevPage;

        /// <summary>
        /// Button for Next page.
        /// </summary>
        private Button m_nextPage;

        /// <summary>
        /// Button for First tab.
        /// </summary>
        private Button m_firstTab;

        /// <summary>
        /// Button for Last tab.
        /// </summary>
        private Button m_lastTab;

        #endregion

        #region Internal Members

         /// <summary>
        /// Internal variable which represents tab item ext
        /// </summary>
        internal TabItemExt tab;

        internal double PageScrollWidth = 0.0;

        internal TabControlExt ParentTabControl
        {
            get
            {
                return TemplatedParent as TabControlExt;
            }
        }

        internal ScrollViewer ChildScrollViewer
        {
            get
            {
                return Content as ScrollViewer;
            }
        }

        internal TabLayoutPanel LayoutPanel
        {
            get
            {
                if (Content is ScrollViewer && ChildScrollViewer != null)
                    return ChildScrollViewer.Content as TabLayoutPanel;
                return Content as TabLayoutPanel;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="TabPanelAdv"/> class.
        /// </summary>
        static TabPanelAdv()
        {
            EnvironmentTest.ValidateLicense(typeof(TabPanelAdv));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabPanelAdv), new FrameworkPropertyMetadata(typeof(TabPanelAdv)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabPanelAdv"/> class.
        /// </summary>
        public TabPanelAdv()
        {
            this.Loaded += new RoutedEventHandler(TabPanelAdv_Loaded);
            this.Unloaded += TabPanelAdv_Unloaded;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Unloaded event of the TabPanelAdv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void TabPanelAdv_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_prevTab != null && m_prevPage != null && m_nextTab != null && m_nextPage != null && m_lastTab != null && m_firstTab != null)
            {
                m_firstTab.Click -= ProcessScrollingButtonClick;
                m_lastTab.Click -= ProcessScrollingButtonClick;
                m_nextPage.Click -= ProcessScrollingButtonClick;
                m_nextTab.Click -= ProcessScrollingButtonClick;
                m_prevPage.Click -= ProcessScrollingButtonClick;
                m_prevTab.Click -= ProcessScrollingButtonClick;
            }
            if (ChildScrollViewer != null)
                ChildScrollViewer.ScrollChanged -= ChildScrollViewer_ScrollChanged;
        }

        /// <summary>
        /// Handles the Loaded event of the TabPanelAdv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void TabPanelAdv_Loaded(object sender, RoutedEventArgs e)
        {
            if (ChildScrollViewer != null)
            {
#if !SyncfusionFramework3_5
                //ChildScrollViewer.PanningMode = PanningMode.Both;
#endif
                ChildScrollViewer.ScrollChanged += ChildScrollViewer_ScrollChanged;
            }
            if (m_prevTab == null || m_prevPage == null || m_nextTab == null || m_nextPage == null || m_lastTab == null || m_firstTab == null)
            {
                m_firstTab = GetTemplateChild("PART_FirstTab") as Button;
                m_lastTab = GetTemplateChild("PART_LastTab") as Button;
                m_nextPage = GetTemplateChild("PART_NextPage") as Button;
                m_nextTab = GetTemplateChild("PART_NextTab") as Button;
                m_prevPage = GetTemplateChild("PART_PrevPage") as Button;
                m_prevTab = GetTemplateChild("PART_PrevTab") as Button;
            }
            if (m_prevTab != null && m_prevPage != null && m_nextTab != null && m_nextPage != null && m_lastTab != null && m_firstTab != null)
            {
                m_firstTab.Click -= ProcessScrollingButtonClick;
                m_lastTab.Click -= ProcessScrollingButtonClick;
                m_nextPage.Click -= ProcessScrollingButtonClick;
                m_nextTab.Click -= ProcessScrollingButtonClick;
                m_prevPage.Click -= ProcessScrollingButtonClick;
                m_prevTab.Click -= ProcessScrollingButtonClick;

                m_firstTab.Click += ProcessScrollingButtonClick;
                m_lastTab.Click += ProcessScrollingButtonClick;
                m_nextPage.Click += ProcessScrollingButtonClick;
                m_nextTab.Click += ProcessScrollingButtonClick;
                m_prevPage.Click += ProcessScrollingButtonClick;
                m_prevTab.Click += ProcessScrollingButtonClick;
            }
        }

        void ChildScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ViewportWidth < e.ExtentWidth)
            {
                PageScrollWidth = e.ViewportWidth;
            }
            else
                PageScrollWidth = 0.0;

            double scrollviewerwidth = 0.0;
            double tablayoutpanelwidth = 0.0;
            
            if (ChildScrollViewer != null)
                scrollviewerwidth = ChildScrollViewer.ViewportWidth;
            if (LayoutPanel != null)
                tablayoutpanelwidth = LayoutPanel.DesiredSize.Width;


            m_scrollInfo.NeedScrollButtonsShow = scrollviewerwidth < tablayoutpanelwidth;

            IsAllItemsVisible = !m_scrollInfo.NeedScrollButtonsShow ||
                                                 TabItemLayout != TabItemLayoutType.SingleLine ||
                                                 TabItemSize != TabItemSizeMode.Normal;
            switch (TabScrollButtonVisibility)
            {
                case TabScrollButtonVisibility.Auto:
                    if (!IsAllItemsVisible)
                    {
                        ShowScrollingButtons();
                    }
                    else
                    {
                        HideScrollingButtons();
                    }

                    break;

                case TabScrollButtonVisibility.Hidden:
                    HideScrollingButtons();
                    break;

                case TabScrollButtonVisibility.Visible:
                    if (TabItemLayout == TabItemLayoutType.SingleLine && TabItemSize == TabItemSizeMode.Normal)
                    {
                        ShowScrollingButtons();
                    }
                    else
                    {
                        HideScrollingButtons();
                    }

                    break;
                default:
                    break;
            }
            if (LayoutPanel != null)
            {
                LayoutPanel.CheckScrollBehavior(sender as ScrollViewer, LayoutPanel.DesiredSize);
                LayoutPanel.m_IsScrollButtonClicked = false;
            }
            
            if (e.HorizontalOffset > 0)
                EnablePrevPart();
            else if (e.HorizontalOffset == 0)
                DisablePrevPart();

            if (e.ViewportWidth + e.HorizontalOffset < e.ExtentWidth)
            {
                EnableNextPart();
            }
            else if (e.ViewportWidth + e.HorizontalOffset >= e.ExtentWidth)
                DisableNextPart();
            //if (e.HorizontalOffset != 0 && LayoutPanel.m_ScrollToSelectedItem != null && LayoutPanel.Children.IndexOf(LayoutPanel.m_ScrollToSelectedItem) == LayoutPanel.Children.Count - 1)
            //    DisableNextPart();
        }

        /// <summary>
        /// Invokes scrolling.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessScrollingButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ScrollDirection scrollDirection = (ScrollDirection)Enum.Parse(typeof(ScrollDirection), button.Tag.ToString());
            LayoutPanel.ProcessScrollInternal(scrollDirection);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Called to arrange and size the content of a <see cref="T:System.Windows.Controls.Control"/> object.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return base.ArrangeOverride(arrangeBounds);
        }

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by <paramref name="constraint"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            TabControlExt tabcontrol = TemplatedParent as TabControlExt;
            if (tabcontrol != null)
            {
             
                
                tab = GetTemplateChild("PART_NewTab") as TabItemExt;
                if (tab != null)
                {
                    tab.newtabParent = TemplatedParent as TabControlExt;
                }
            }
            //LayoutPanel.MeasureElements(new Size(ChildScrollViewer.ViewportWidth, constraint.Height));
            return base.MeasureOverride(constraint);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (ChildScrollViewer != null)
            {
#if !SyncfusionFramework3_5
                //ChildScrollViewer.PanningMode = PanningMode.Both;
#endif
                ChildScrollViewer.ScrollChanged += ChildScrollViewer_ScrollChanged;
            }

            m_firstTab = GetTemplateChild("PART_FirstTab") as Button;
            m_lastTab = GetTemplateChild("PART_LastTab") as Button;
            m_nextPage = GetTemplateChild("PART_NextPage") as Button;
            m_nextTab = GetTemplateChild("PART_NextTab") as Button;
            m_prevPage = GetTemplateChild("PART_PrevPage") as Button;
            m_prevTab = GetTemplateChild("PART_PrevTab") as Button;

            if (m_prevTab != null && m_prevPage != null && m_nextTab != null && m_nextPage != null && m_lastTab != null && m_firstTab != null)
            {
                m_firstTab.Click += ProcessScrollingButtonClick;
                m_lastTab.Click += ProcessScrollingButtonClick;
                m_nextPage.Click += ProcessScrollingButtonClick;
                m_nextTab.Click += ProcessScrollingButtonClick;
                m_prevPage.Click += ProcessScrollingButtonClick;
                m_prevTab.Click += ProcessScrollingButtonClick;
            }
            
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (!(this.Content is TabSplitterItemPanel))
            {
                if (e.Property == SkinStorage.VisualStyleProperty)
                {
                    ResourceDictionary rd = new ResourceDictionary();
                    TabSplitter splitter = null;

                    splitter = this.TemplatedParent as TabSplitter;
                    if (splitter == null)
                    {
                        if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);

                            this.Style = rd["Office2007BlueTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "Blend")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute);

                            this.Style = rd["BlendTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "Office2003")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute);

                            this.Style = rd["Office2003TabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "Office2007Black")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);

                            this.Style = rd["Office2007BlackTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "Office2007Silver")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);

                            this.Style = rd["Office2007SilverTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "ShinyRed")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);

                            this.Style = rd["ShinyRedTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "ShinyBlue")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);

                            this.Style = rd["ShinyBlueTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "SyncOrange")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);

                            this.Style = rd["SyncOrangeTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "VS2010")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute);

                            this.Style = rd["VS2010TabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "Office2010Blue")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                            this.Style = rd["Office2010BlueTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                            this.Style = rd["Office2010BlackTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "Office2010Silver")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                            this.Style = rd["Office2010SilverTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "Metro")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                            this.Style = rd["MetroTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "Transparent")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                            this.Style = rd["TransparentThemeTabPanelAdvStyle"] as Style;
                        }
                        else if (SkinStorage.GetVisualStyle(this) == "Office2013")
                        {
                            rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2013Style.xaml", UriKind.RelativeOrAbsolute);
                            this.Style = rd["Office2013TabPanelAdvStyle"] as Style;
                        }
                    }
                }
            }
        }

        #endregion

        #region Public methods

        internal void EnsureTabScrollStyle()
        {
            m_scrollInfo.NeedScrollButtonsShow = (this.Content is ScrollViewer) ? (this.Content as ScrollViewer).ViewportWidth < ((this.Content as ScrollViewer).Content as TabLayoutPanel).DesiredSize.Width : false;

            IsAllItemsVisible = !m_scrollInfo.NeedScrollButtonsShow ||
                                                 TabItemLayout != TabItemLayoutType.SingleLine ||
                                                 TabItemSize != TabItemSizeMode.Normal;
            switch (TabScrollButtonVisibility)
            {
                case TabScrollButtonVisibility.Auto:
                    if (!IsAllItemsVisible)
                    {
                        ShowScrollingButtons();
                    }
                    else
                    {
                        HideScrollingButtons();
                    }

                    break;

                case TabScrollButtonVisibility.Hidden:
                    HideScrollingButtons();
                    break;

                case TabScrollButtonVisibility.Visible:
                    if (TabItemLayout == TabItemLayoutType.SingleLine && TabItemSize == TabItemSizeMode.Normal)
                    {
                        ShowScrollingButtons();
                    }
                    else
                    {
                        HideScrollingButtons();
                    }

                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Shows scroll buttons when it is needed.
        /// </summary>
        public void ShowScrollingButtons()
        {
            if (LayoutPanel.TabScrollStyle == TabScrollStyle.Extended)
            {
                if (LayoutPanel.m_ParentTabControl.TabVisualStyle == TabVisualStyle.None)
                {
                    m_prevPage.Visibility = Visibility.Visible;
                    m_nextPage.Visibility = Visibility.Visible;
                }
                m_firstTab.Visibility = Visibility.Visible;
                m_lastTab.Visibility = Visibility.Visible;
            }
            else
            {
                if (LayoutPanel.m_ParentTabControl.TabVisualStyle == TabVisualStyle.None)
                {
                    m_prevPage.Visibility = Visibility.Collapsed;
                    m_nextPage.Visibility = Visibility.Collapsed;
                }
                m_firstTab.Visibility = Visibility.Collapsed;
                m_lastTab.Visibility = Visibility.Collapsed;
            }

            m_prevTab.Visibility = Visibility.Visible;
            m_nextTab.Visibility = Visibility.Visible;
            //Showing = true;
        }


        /// <summary>
        /// Hides scroll buttons when it is needed.
        /// </summary>
        public void HideScrollingButtons()
        {
            if (LayoutPanel != null && LayoutPanel.TabScrollStyle == TabScrollStyle.Extended)
            {
                m_prevPage.Visibility = Visibility.Collapsed;
                m_nextPage.Visibility = Visibility.Collapsed;
                m_firstTab.Visibility = Visibility.Collapsed;
                m_lastTab.Visibility = Visibility.Collapsed;
            }

            m_prevTab.Visibility = Visibility.Collapsed;
            m_nextTab.Visibility = Visibility.Collapsed;
            //Showing = false;
        }

        /// <summary>
        /// Disables Next part of scrolling buttons.
        /// </summary>
        public void DisableNextPart()
        {
            if (LayoutPanel != null)
            {
                if (LayoutPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
                {
                    m_nextTab.IsEnabled = false;
                    m_lastTab.IsEnabled = false;
                    m_nextPage.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Enables Next part of scrolling buttons.
        /// </summary>
        public void EnableNextPart()
        {
            if (LayoutPanel != null)
            {
                if (LayoutPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
                {
                    m_nextTab.IsEnabled = true;
                    m_lastTab.IsEnabled = true;
                    m_nextPage.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Disables Prev part of scrolling buttons.
        /// </summary>
        public void DisablePrevPart()
        {
            if (LayoutPanel != null)
            {
                if (LayoutPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
                {
                    m_prevTab.IsEnabled = false;
                    m_firstTab.IsEnabled = false;
                    m_prevPage.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Enables Prev part of scrolling buttons.
        /// </summary>
        public void EnablePrevPart()
        {
            if (LayoutPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
            {
                m_prevTab.IsEnabled = true;
                m_firstTab.IsEnabled = true;
                m_prevPage.IsEnabled = true;
            }
        }
        #endregion

        #region Properies
        /// <summary>
        /// Gets a value indicating whether IsAllItemsVisible is true.
        /// </summary>
        public bool IsAllItemsVisible
        {
            get
            {
                return (bool)GetValue(IsAllItemsVisibleProperty);
            }

            internal set
            {
                SetValue(IsAllItemsVisiblePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the duplicate TabControlExt TabItemLayout property.
        /// </summary>
        /// <value>The tab item layout.</value>
        private TabItemLayoutType TabItemLayout
        {
            get
            {
                TabItemLayoutType result = TabItemLayoutType.SingleLine;
                if (ParentTabControl != null)
                {
                    result = ParentTabControl.TabItemLayout;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the duplicate TabControlExt TabItemSize property.
        /// </summary>
        /// <value>The size of the tab item.</value>
        private TabItemSizeMode TabItemSize
        {
            get
            {
                TabItemSizeMode result = TabItemSizeMode.Normal;
                if (ParentTabControl != null)
                {
                    result = ParentTabControl.TabItemSize;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the duplicate TabControlExt TabScrollButtonVisibility property.
        /// </summary>
        /// <value>The tab scroll button visibility.</value>
        private TabScrollButtonVisibility TabScrollButtonVisibility
        {
            get
            {
                TabScrollButtonVisibility visibility = TabScrollButtonVisibility.Auto;
                if (ParentTabControl != null)
                {
                    visibility = ParentTabControl.TabScrollButtonVisibility;
                }

                return visibility;
            }
        }

        /// <summary>
        /// Gets or sets the value of the IsAllItemsVisible dependency property.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        #endregion

        #region DP properties
        /// <summary>
        /// Represents the IsAllItemsVisiblePropertyKey
        /// </summary>
        protected static readonly DependencyPropertyKey IsAllItemsVisiblePropertyKey =
            DependencyProperty.RegisterReadOnly("IsAllItemsVisible", typeof(bool), typeof(TabPanelAdv), new FrameworkPropertyMetadata(true));

        /// <summary>
        ///  Represents the IsAllItemsVisible Dependency property
        /// </summary>
        public static readonly DependencyProperty IsAllItemsVisibleProperty = IsAllItemsVisiblePropertyKey.DependencyProperty;

        /// <summary>
        /// Represents the eCornerRadius Dependency property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(TabPanelAdv));
        #endregion
    }

    public class TabScrollViewer : ScrollViewer
    {
#if !SyncfusionFramework3_5
        protected override void OnManipulationBoundaryFeedback(System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
            base.OnManipulationBoundaryFeedback(e);
        }

        public TabScrollViewer()
        {
            PanningMode = PanningMode.HorizontalFirst;
            Focusable = false;
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);
            //if (e.OriginalSource is TabItemExt)
            //{
            //    TabItemExt item = e.OriginalSource as TabItemExt;
            //    if (item != null && item.m_IsTouchDownElement)
            //    {
            //        item.m_IsNoSelect = true;
            //    }
            //}
            //if (e.Source is TabLayoutPanel)
            //{
            //    (this.Content as TabLayoutPanel).m_AllowDrag = false;
            //}
            if (e.ManipulationOrigin.Y > (this.Parent as TabPanelAdv).ActualHeight + 20)
            {
                //PanningMode = PanningMode.None;
                e.Complete();
                //(this.Content as TabLayoutPanel).m_AllowDrag = true;
            }
        }
#endif
    }
}
