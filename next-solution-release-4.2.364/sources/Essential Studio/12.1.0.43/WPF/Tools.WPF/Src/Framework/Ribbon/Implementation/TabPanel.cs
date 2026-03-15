// <copyright file="TabPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Panel designed to layout RibbonTabs.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TabPanel : Panel
    {
        #region Class constants
        /// <summary>
        /// Min size for children. 
        /// </summary>
        private const double C_MIN_SIZE_CHILDREN = 16d;

        /// <summary>
        /// Min size for children. 
        /// </summary>
        private const double C_MIN_BUTTON_WIDTH = 23;

        /// <summary>
        /// Width of tab scrolling. 
        /// </summary>
        private const double C_SCROLL_WIDTH = 27;

        #endregion

        #region Private members
        /// <summary>
        /// Array for splitters.
        /// </summary>
        private List<Rectangle> m_splitters = new List<Rectangle>();

        /// <summary>
        /// Ribbon instance. 
        /// </summary>
        internal Ribbon m_ribbon;

        /// <summary>
        /// Ribbon window instance. 
        /// </summary>
        private RibbonWindow m_ribbonWindow;

        /// <summary>
        ///  Ribbon Visible children Count
        /// </summary>
        private int m_visibleChildrenCount;

        /// <summary>
        /// Ribbon collection visible children
        /// </summary>
        internal RibbonTabCollection m_visibleChildren;

        /// <summary>
        /// This member gets AdornerLayer control.
        /// </summary>
        private AdornerLayer m_adornerLayer;

        /// <summary>
        /// Represents the left offset
        /// </summary>
        private double m_leftOffset = 0;

        /// <summary>
        /// Represents the needed space
        /// </summary>
        private double m_neededSpace = 0;

        /// <summary>
        /// Represents the left button
        /// </summary>
        private Button m_leftButton = null;

        /// <summary>
        /// Represents the right button
        /// </summary>
        private Button m_rightButton = null;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TabPanel"/> class.
        /// </summary>
        static TabPanel()
        {
            RoutedEvent ev = RibbonTab.VisibilityChangedEvent.AddOwner(typeof(TabPanel));
            EventManager.RegisterClassHandler(typeof(TabPanel), ev, new RoutedEventHandler(VisibilityChangedEventHandler));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabPanel"/> class.
        /// </summary>
        public TabPanel()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the x.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>X property object</returns>
        private static double GetX(DependencyObject obj)
        {
            return (double)obj.GetValue(XProperty);
        }

        /// <summary>
        /// Sets the x.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="value">The value param.</param>
        private static void SetX(DependencyObject obj, double value)
        {
            obj.SetValue(XProperty, value);
        }

        /// <summary>
        /// Gets collection of visible RibbonTabs.
        /// </summary>
        protected RibbonTabCollection VisibleChildren
        {
            get
            {
                UIElementCollection collection = base.InternalChildren;
                int collCount = collection.Count;

                if (m_visibleChildrenCount == -1)
                {
                    m_visibleChildrenCount = collCount;
                }
                else if (m_visibleChildrenCount != collCount || m_visibleChildrenCount == 1)
                {
                    m_visibleChildren = null;
                    m_visibleChildrenCount = collCount;
                }

                if (m_visibleChildren != null)
                {
                    return m_visibleChildren;
                }
                else
                {
                    m_visibleChildren = new RibbonTabCollection();

                    foreach (UIElement item in collection)
                    {
                        if (item.Visibility != Visibility.Collapsed)
                        {
                            m_visibleChildren.Add((RibbonTab)item);
                        }
                    }

                    return m_visibleChildren;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the separator between tab buttons.
        /// </summary>
        public Color SeparatorColor
        {
            get
            {
                return (Color)GetValue(SeparatorColorProperty);
            }

            set
            {
                SetValue(SeparatorColorProperty, value);
            }
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Defines the x offset.
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.RegisterAttached("x", typeof(double), typeof(TabPanel), new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// Defines the color of the separator between tab buttons.
        /// </summary>
        public static readonly DependencyProperty SeparatorColorProperty =
            DependencyProperty.Register("SeparatorColor", typeof(Color), typeof(TabPanel), new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnSeparatorColorChanged)));
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when SeparatorColor property is changed.
        /// </summary>
        public event PropertyChangedCallback SeparatorColorChanged;
        #endregion

        #region Override methods
        /// <summary>
        /// Raises the Initialized event. This method is invoked
        /// whenever IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event
        /// data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            
            m_ribbonWindow = VisualUtils.FindRootVisual(this) as RibbonWindow;
            m_ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            m_ribbon.m_TabPanel = this;
            m_ribbon.IsQATBelowChanged += new PropertyChangedCallback(Ribbon_IsQATBelowChanged);
            m_adornerLayer = AdornerLayer.GetAdornerLayer(this);

            if (m_ribbon.QuickAccessToolBar != null)
            {
                m_ribbon.QuickAccessToolBar.VisibleItemsCountChanged += new PropertyChangedCallback(QuickAccessToolBar_VisibleItemsCountChanged);
            }
            this.Unloaded += new RoutedEventHandler(TabPanel_Unloaded);           
        }

        /// <summary>
        /// Handles the Unloaded event of the TabPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void TabPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_ribbon != null)
            {
                m_ribbon.IsQATBelowChanged -= new PropertyChangedCallback(Ribbon_IsQATBelowChanged);

                if (m_ribbon.QuickAccessToolBar != null)
                    m_ribbon.QuickAccessToolBar.VisibleItemsCountChanged -= new PropertyChangedCallback(QuickAccessToolBar_VisibleItemsCountChanged);

                this.m_ribbonWindow = null;

                this.m_ribbon = null;
            }
        }

        /// <summary>
        /// Gets the number of child objects in this instance of <see cref="T:System.Windows.Controls.Panel" />.
        /// </summary>
        /// <returns>
        /// The number of child objects. 
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                int countBase = base.VisualChildrenCount;
                int result = countBase;

                if (countBase > 1)
                {
                    result = countBase * 2 + 1;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a child of this <see cref="T:System.Windows.Controls.Panel" />
        /// at the specified index position.
        /// </summary>
        /// <param name="index">The index position of the child.</param>
        /// <returns>
        /// A child of the parent element.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            try
            {
                int countBase = Children.Count;
                int indexBase = (int)Math.Truncate(index / 2d);
                int iSplitter = indexBase;
                bool bSplitter = indexBase * 2 != index;

                if (VisualChildrenCount > 1)
                {
                    switch (VisualChildrenCount - index)
                    {
                        case 1:
                            return m_rightButton;
                        case 2:
                            return m_leftButton;
                    }
                }

                if (!bSplitter)
                {
                    return Children[indexBase];
                }
                else
                {
                    if (m_splitters.Count == iSplitter)
                    {
                        iSplitter--;
                    }

                    if (m_splitters.Count == 0 || iSplitter < 0 || iSplitter >= m_splitters.Count)
                        return Children[indexBase];
                    else
                        return m_splitters[iSplitter];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements
        /// and determines a size for a <see cref="T:System.Windows.FrameworkElement" />derived
        /// class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to
        /// arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = new Size();

            RibbonTabCollection visibleChildren = VisibleChildren;
            int visibleChildrenCount = visibleChildren.Count;

            double buttonsWidth = m_leftOffset;

            for (int i = 0; i < visibleChildrenCount; i++)
            {
                RibbonTab element = visibleChildren[i];
                
                element.Arrange(new Rect(new Point(buttonsWidth, 0), new Size(element.ArrangeWidth, element.ActualHeight)));
                element.Arrange(new Rect(new Point(buttonsWidth, 0), new Size(element.ArrangeWidth, element.ActualHeight)));
                
                TabPanel.SetX(element, buttonsWidth);
                buttonsWidth += element.ArrangeWidth + 1;
            }

            if (buttonsWidth - 1 > finalSize.Width)
            {
                m_neededSpace = buttonsWidth + size.Width - finalSize.Width;
                m_rightButton.Visibility = Visibility.Visible;
            }
            else
            {
                if (finalSize.Width - buttonsWidth > Math.Abs(m_leftOffset))
                {
                    m_leftOffset = 0;
                    m_leftButton.Visibility = Visibility.Collapsed;
                }

                m_rightButton.Visibility = Visibility.Collapsed;
            }

            Rect finalRect = new Rect(finalSize);
            finalRect.X = 0;
            finalRect.Width = m_leftButton.DesiredSize.Width;
            m_leftButton.Arrange(finalRect);

            finalRect.Width = m_rightButton.DesiredSize.Width;
            finalRect.X = finalSize.Width - m_rightButton.DesiredSize.Width;

            m_rightButton.Arrange(finalRect);

            if (m_splitters.Count > 0)
            {
                ArrangeSplitters(visibleChildren, visibleChildrenCount);
            }

            ContextTabGroup group = null;
            if (m_ribbon != null)
            {
                group = m_ribbon.FirstVisibleGroup;
            }
            QuickAccessToolBar qat=null;
            if (m_ribbon != null)
            {
                if (m_adornerLayer == null || m_ribbon.m_bItemsPanelAdornerLayerChanged)
                {
                    m_adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    m_ribbon.m_bItemsPanelAdornerLayerChanged = false;
                }

                 qat = m_ribbon.QuickAccessToolBar;
            }
            if (group != null && m_ribbon!= null)
            {
                ArrangeContextAdorners(m_ribbon.ContextTabGroups, finalSize);

                double x = (double)GetX(group.RibbonTabs[0]);
                double leftOffset = 0;
                double rightOffset = 0;

                if(m_ribbonWindow!=null)
                    rightOffset = m_ribbonWindow.TitleBar.SysButtonsColumnWidth;

                if (qat != null && !m_ribbon.IsQATBelow && x > 0)
                {
                    qat.MaxWidth = x;

                    leftOffset = qat.ActualWidth;

                    if (m_ribbonWindow != null && qat.MaxWidth <= 0)
                    {
                        qat.MaxWidth -= m_ribbonWindow.TitleBar.TitleActualWidth / 2;
                    }
                }

                Interval titleInterval = this.GetTitleInterval(this.VisibleChildren, leftOffset, rightOffset, finalSize);
                if (m_ribbonWindow != null)
                    ArrangeTitle(new Interval(titleInterval.A - leftOffset, titleInterval.B - leftOffset), m_ribbonWindow.TitleBar);
            }
            else
            {
                if (qat != null && m_ribbonWindow != null)
                {
                    TitleBar titleBar = m_ribbonWindow.TitleBar;
                    double appMenuColumnWidth = titleBar.AppMenuColumnWidth;
                    double sysBtnsColumnWidth = titleBar.SysButtonsColumnWidth;

                    double availableWidth = titleBar.ActualWidth - appMenuColumnWidth - sysBtnsColumnWidth - titleBar.TitleActualWidth;

                    if (availableWidth > 0 && !m_ribbon.IsQATBelow)
                    {
                        qat.MaxWidth = availableWidth;
                    }
                    else
                    {
                        qat.ClearValue(MaxWidthProperty);
                    }
                }
            }

            return finalSize;
        }

        /// <summary>
        /// Arranges the title.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <param name="titleBar">The title bar.</param>
        private void ArrangeTitle(Interval interval, TitleBar titleBar)
        {
            double titleWidth = titleBar.TitleActualWidth;
            double length = interval.Length;
            if (titleWidth > length)
            {
                titleBar.TitleMaxWidth = length;
                titleBar.HorizontalTitleOffset = interval.A + 2d;
                return;
            }
            else
            {
                titleBar.TitleMaxWidth = length;
            }
            titleBar.HorizontalTitleOffset = (interval.A + length / 2 - titleWidth / 2) + 2d;
        }

        /// <summary>
        /// Arranges the splitters.
        /// </summary>
        /// <param name="visibleChildren">The visible children.</param>
        /// <param name="visibleChildrenCount">The visible children count.</param>
        private void ArrangeSplitters(RibbonTabCollection visibleChildren, int visibleChildrenCount)
        {
            float cof = 0;
            if (VisibleChildren.Count != 0)
            {
                cof = (float)(1d / 7d * (12d - VisibleChildren[0].M_margin));
            }

            Color selectHoverBorderBrushStart = SeparatorColor;
            selectHoverBorderBrushStart.A = 50;

            Color selectHoverBorderBrushEnd = SeparatorColor;
            selectHoverBorderBrushEnd.ScA = cof;

            LinearGradientBrush m_selectHoverBorderBrushVertical = new LinearGradientBrush();
            m_selectHoverBorderBrushVertical.StartPoint = new Point(0.5, 1);
            m_selectHoverBorderBrushVertical.EndPoint = new Point(0.5, 0);
            m_selectHoverBorderBrushVertical.GradientStops.Add(new GradientStop(selectHoverBorderBrushEnd, 0));
            m_selectHoverBorderBrushVertical.GradientStops.Add(new GradientStop(selectHoverBorderBrushEnd, 0.65));
            m_selectHoverBorderBrushVertical.GradientStops.Add(new GradientStop(selectHoverBorderBrushStart, 1));

            double width = m_leftOffset;

            for (int i = 0, len = m_splitters.Count; i < len; i++)
            {
                if (i < visibleChildrenCount)
                {
                    m_splitters[i].Visibility = Visibility.Visible;

                    m_splitters[i].Fill = m_selectHoverBorderBrushVertical;

                    width += visibleChildren[i].ArrangeWidth + 1;

                    m_splitters[i].Arrange(new Rect(new Point(width - 1, 0), new Size(1, 24)));
                }
                else
                {
                    m_splitters[i].Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Arranges the context adorners.
        /// </summary>
        /// <param name="contextTabGroupCollection">The context tab group collection.</param>
        /// <param name="finalSize">The final size.</param>
        private void ArrangeContextAdorners(ContextTabGroupCollection contextTabGroupCollection, Size finalSize)
        {
            foreach (ContextTabGroup contextTabGroup in contextTabGroupCollection)
            {
                if (contextTabGroup.IsGroupVisible && contextTabGroup.RibbonTabs.Count != 0)
                {
                    RibbonTabCollection ribbonTabCollection = contextTabGroup.RibbonTabs;
                    RibbonTab firstTab = contextTabGroup.RibbonTabs[0];
                    double contextWidth = firstTab.ActualWidth;
                    ContextAdorner contextAdorner = CreateContextAdorner(firstTab);
                    //if (contextAdorner == null)
                    //{
                    //    contextAdorner = CreateContextAdorner(firstTab);
                    //}
                    double x = TabPanel.GetX(firstTab);
                    if (x < 0)
                    {
                        firstTab.ContextAdorner.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        firstTab.ContextAdorner.Visibility = Visibility.Visible;
                    }

                    int contextTabsCount = ribbonTabCollection.Count;

                    for (int i = 1; i < contextTabsCount; i++)
                    {
                        RibbonTab tab = ribbonTabCollection[i];
                        int commonIndex = VisibleChildren.IndexOf(tab);

                        if (commonIndex != -1 && commonIndex != 0)
                        {
                            if (VisibleChildren[commonIndex - 1].ContextTabGroup == tab.ContextTabGroup)
                            {
                                contextWidth += tab.ActualWidth + 1;
                                ContextAdorner temp = tab.ContextAdorner;

                                if (temp != null)
                                {
                                    m_adornerLayer.Remove(temp);
                                    tab.ContextAdorner = null;
                                }
                            }
                            else
                            {
                                contextAdorner.TemplatedInnerControl.Width = contextWidth;
                                contextWidth = tab.ActualWidth;
                                contextAdorner = tab.ContextAdorner;
                                if (contextAdorner == null)
                                {
                                    contextAdorner = CreateContextAdorner(tab);
                                }
                            }
                        }
                    }

                    contextAdorner.TemplatedInnerControl.Width = contextWidth;
                    if (m_ribbonWindow != null)
                    {
                        double correctedContextWidth;
                        double sysBtnsColumnWidth = m_ribbonWindow.TitleBar.SysButtonsColumnWidth;
                        if (x + contextWidth > finalSize.Width - sysBtnsColumnWidth)
                        {
                            correctedContextWidth = finalSize.Width - sysBtnsColumnWidth - x;
                            contextAdorner.TemplatedInnerControl.Width = (correctedContextWidth > 0) ? correctedContextWidth : 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the title interval.
        /// </summary>
        /// <param name="visibleTabs">The visible tabs.</param>
        /// <param name="leftOffset">The left offset.</param>
        /// <param name="rightOffset">The right offset.</param>
        /// <param name="finalSize">The final size.</param>
        /// <returns>return result</returns>
        private Interval GetTitleInterval(RibbonTabCollection visibleTabs, double leftOffset, double rightOffset, Size finalSize)
        {
            Interval result = new Interval();
            Interval temp;
            double d = leftOffset;
            foreach (RibbonTab tab in visibleTabs)
            {
                ContextAdorner contextAdorner = tab.ContextAdorner;
                if (contextAdorner != null)
                {
                    contextAdorner.TemplatedInnerControl.Measure(finalSize);
                    double actualAdornerWidth = actualAdornerWidth = contextAdorner.TemplatedInnerControl.DesiredSize.Width;
                    if (actualAdornerWidth > 3d)
                    {
                        temp = new Interval();
                        temp.A = d;
                        temp.B = (double)GetX(tab);
                        if (temp.Length > result.Length)
                        {
                            result = temp;
                        }

                        d += temp.Length + actualAdornerWidth;

                        if (d > finalSize.Width - rightOffset)
                        {
                            return result;
                        }
                    }
                }
            }

            temp = new Interval(d+30, finalSize.Width - rightOffset);
            if (temp.Length > result.Length)
            {
                result = temp;
            }

            return result;
        }

        /// <summary>
        /// Creates the context adorner.
        /// </summary>
        /// <param name="tab">The tab value.</param>
        /// <returns>context adorner</returns>
        private ContextAdorner CreateContextAdorner(RibbonTab tab)
        {
            try
            {
                ContextAdorner contextAdorner=null;
                if (tab.ContextAdorner != null)
                {
                    m_adornerLayer.Remove(tab.ContextAdorner);
                    contextAdorner = tab.ContextAdorner;
                }
                else
                    contextAdorner = new ContextAdorner(tab);
                if (m_ribbon != null && contextAdorner.TemplatedInnerControl != null)
                {
                    contextAdorner.TemplatedInnerControl.Style = m_ribbon.ContextAdornerStyle;
                }
                contextAdorner.OffsetY = -32d;
                tab.ContextAdorner = contextAdorner;
                if (contextAdorner.Parent == null && !IsAdornerExist(m_adornerLayer,tab,contextAdorner))
                    m_adornerLayer.Add(contextAdorner);
                return contextAdorner;
            }
            //SU I78477
            //catch (InvalidOperationException ioe)
            catch (InvalidOperationException)
                //EU I78477
            { return new ContextAdorner(tab); }
            
        }

        private bool IsAdornerExist(AdornerLayer adornerlayer, UIElement host, ContextAdorner contextAdorner)
        {
            if (adornerlayer != null && host != null && contextAdorner != null)
            {
                Adorner[] adorners = adornerlayer.GetAdorners(host);
                if (adorners != null)
                {
                    foreach (ContextAdorner adorner in adorners)
                    {
                        if (adorner==contextAdorner)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size tabSize = availableSize;
            RibbonTabCollection visibleChildren = VisibleChildren;

            InitializeSeparators();

            if (m_leftButton == null)
            {
                InitButtons();
            }

            m_leftButton.Measure(availableSize);
            m_rightButton.Measure(availableSize);

            if (tabSize.Width > 0)
            {
                tabSize.Width -= tabSize.Width > VisibleChildren.Count ? VisibleChildren.Count : 0;
            }

            double buttonsWidth = MeasureButtons(tabSize, true);

            if (buttonsWidth > tabSize.Width)
            {
                buttonsWidth = tabSize.Width;
            }

            return new Size(Math.Max(0, buttonsWidth), 22);
        }

        /// <summary>
        /// Measures the buttons.
        /// </summary>
        /// <param name="tabSize">Size of the tab.</param>
        /// <param name="needsMeasure">if set to <c>true</c> [needs measure].</param>
        /// <returns>measured button</returns>
        private double MeasureButtons(Size tabSize, bool needsMeasure)
        {
            List<RibbonTab> buttons = new List<RibbonTab>();
            RibbonTabCollection visibleChildren = VisibleChildren;
            double buttonsWidth = 0;
            if (visibleChildren.Count > 0)
            {
                RibbonTab maxWidthButton = visibleChildren[0];
                double def = 0;

                if (needsMeasure)
                {
                    foreach (RibbonTab element in visibleChildren)
                    {
                        element.ArrangeWidth = 0;
                        element.Measure(tabSize);
                        element.ArrangeWidth = element.DesiredSize.Width;
                    }
                }

                foreach (FrameworkElement element in visibleChildren)
                {
                    if (maxWidthButton.DesiredSize.Width < (element as RibbonTab).DesiredSize.Width)
                    {
                        buttons.Clear();
                        def = (element as RibbonTab).DesiredSize.Width - maxWidthButton.DesiredSize.Width;
                        maxWidthButton = element as RibbonTab;
                        buttons.Add(maxWidthButton);
                    }
                    else if (maxWidthButton.DesiredSize.Width == (element as RibbonTab).DesiredSize.Width)
                    {
                        buttons.Add((element as RibbonTab));
                    }
                    else if (maxWidthButton.DesiredSize.Width - (element as RibbonTab).DesiredSize.Width < def || def == 0)
                    {
                        def = maxWidthButton.DesiredSize.Width - (element as RibbonTab).DesiredSize.Width;
                    }

                    buttonsWidth += (element as RibbonTab).DesiredSize.Width;
                }

                if (buttonsWidth > tabSize.Width)
                {
                    if ((def * buttons.Count) > (buttonsWidth - tabSize.Width) || def == 0)
                    {
                        def = (buttonsWidth - tabSize.Width) / buttons.Count;
                        foreach (RibbonTab button in buttons)
                        {
                            double width = button.DesiredSize.Width - def;
                            buttonsWidth -= def;
                            button.ArrangeWidth = width;

                            button.InvalidateMeasure();
                            button.Measure(tabSize);
                        }
                    }
                    else
                    {
                        foreach (RibbonTab button in buttons)
                        {
                            double width = button.DesiredSize.Width - def;
                            button.ArrangeWidth = width;

                            button.InvalidateMeasure();
                            button.Measure(tabSize);
                        }

                        MeasureButtons(tabSize, false);
                    }
                }
                else
                {
                    double diff = (tabSize.Width - buttonsWidth) / visibleChildren.Count;
                    for (int i = 0; i < visibleChildren.Count; i++)
                    {
                        double width;
                        if (visibleChildren[i].m_tabButton.MaxWidth >= visibleChildren[i].DesiredSize.Width + diff)
                        {
                            width = visibleChildren[i].DesiredSize.Width + diff;
                        }
                        else
                        {
                            width = visibleChildren[i].m_tabButton.MaxWidth;
                        }

                        visibleChildren[i].ArrangeWidth = width;
                    }
                }
            }

            return buttonsWidth;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Inits the buttons.
        /// </summary>
        private void InitButtons()
        {
            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri("pack://application:,,,/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            Style style = (Style)dictionary["RibbonScrollingButton"];
            m_leftButton = new Button();
            m_rightButton = new Button();
            m_leftButton.Tag = "Left";
            m_rightButton.Tag = "Right";
            m_leftButton.Style = style;
            m_rightButton.Style = style;
            m_leftButton.Visibility = Visibility.Collapsed;
            m_rightButton.Visibility = Visibility.Collapsed;
            m_leftButton.Click += new RoutedEventHandler(LeftButton_Click);
            m_rightButton.Click += new RoutedEventHandler(RightButton_Click);
            m_rightButton.Height = 20;
            m_leftButton.Height = 20;
            AddLogicalChild(m_leftButton);
            AddVisualChild(m_leftButton);
            AddLogicalChild(m_rightButton);
            AddVisualChild(m_rightButton);
        }

        /// <summary>
        /// Handles the Click event of the RightButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_neededSpace > 27)
            {
                m_leftOffset -= 27;
                m_leftButton.Visibility = Visibility.Visible;
            }
            else
            {
                m_leftOffset -= m_neededSpace;
                m_rightButton.Visibility = Visibility.Collapsed;
                m_leftButton.Visibility = Visibility.Visible;
            }

            InvalidateArrange();
        }

        /// <summary>
        /// Handles the Click event of the LeftButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (Math.Abs(m_leftOffset) > 27)
            {
                m_leftOffset += 27;
                m_rightButton.Visibility = Visibility.Visible;
            }
            else
            {
                m_leftOffset = 0;
                m_leftButton.Visibility = Visibility.Collapsed;
                m_rightButton.Visibility = Visibility.Visible;
            }

            InvalidateArrange();
        }

        /// <summary>
        /// Initializes the separators.
        /// </summary>
        private void InitializeSeparators()
        {
            int childCount = InternalChildren.Count;

            if (childCount > 0)
            {
                int sepCount = childCount - 1;
                while (m_splitters.Count < sepCount)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = 3;
                    rect.Height = 22;
                    AddVisualChild(rect);
                    m_splitters.Add(rect);
                }

                while (m_splitters.Count > sepCount)
                {
                    RemoveVisualChild(m_splitters[m_splitters.Count - 1]);
                    m_splitters.RemoveAt(m_splitters.Count - 1);
                }
            }
        }

        /// <summary>
        /// Invoked when visibility of item is changed.
        /// </summary>
        /// <param name="sender">Item panel child.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void VisibilityChangedEventHandler(object sender, RoutedEventArgs e)
        {
            RibbonTab item = (RibbonTab)e.OriginalSource;
            TabPanel panel = (TabPanel)sender;

            if (item.Visibility == Visibility.Collapsed)
            {
                if (panel.m_visibleChildren != null)
                {
                    panel.m_visibleChildren.Remove(item);
                }
            }
            else
            {
                panel.m_visibleChildren = null;
            }
        }

        /// <summary>
        /// Buttons measure.
        /// </summary>
        /// <param name="width">The width.</param>
        private void ButtonsMeasure(double width)
        {
            List<RibbonTab> buttons = new List<RibbonTab>();
            RibbonTab maxWidthButton = (RibbonTab)VisibleChildren[0];
            double buttonsWidth = 0;
            double def = 0;

            foreach (FrameworkElement element in VisibleChildren)
            {
                if (maxWidthButton.ActualWidth < (element as RibbonTab).ActualWidth)
                {
                    buttons.Clear();
                    def = (element as RibbonTab).ActualWidth - maxWidthButton.ActualWidth;
                    maxWidthButton = element as RibbonTab;
                    buttons.Add(maxWidthButton);
                }
                else if (maxWidthButton.ActualWidth == (element as RibbonTab).ActualWidth)
                {
                    buttons.Add((element as RibbonTab));
                }
                else if (maxWidthButton.ActualWidth - (element as RibbonTab).ActualWidth < def || def == 0)
                {
                    def = maxWidthButton.ActualWidth - (element as RibbonTab).ActualWidth;
                }

                buttonsWidth += (element as RibbonTab).ActualWidth;
            }

            if (buttonsWidth > width)
            {
                if ((def * buttons.Count) > (buttonsWidth - width) || def == 0)
                {
                    def = (buttonsWidth - width) / buttons.Count;
                    foreach (RibbonTab button in buttons)
                    {
                        button.Arrange(new Rect(0, 0, button.ActualWidth - def, button.ActualHeight));
                    }
                }
                else
                {
                    foreach (RibbonTab button in buttons)
                    {
                        button.Arrange(new Rect(0, 0, button.ActualWidth - def, button.ActualHeight));
                    }

                    ButtonsMeasure(width);
                }
            }
        }

        /// <summary>
        /// Quick's the access tool bar_ visible items count changed.
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void QuickAccessToolBar_VisibleItemsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!m_ribbon.IsQATBelow)
            {
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Ribbon_s the is QAT below changed.
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Ribbon_IsQATBelowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElementCollection children = base.InternalChildren;

            foreach (UIElement child in children)
            {
                RibbonTab tab = child as RibbonTab;

                if (tab.ContextAdorner != null)
                {
                    m_adornerLayer.Remove(tab.ContextAdorner);
                    tab.ContextAdorner = null;
                }
            }

            m_adornerLayer = null;

            m_ribbon.QuickAccessToolBar.ClearValue(MaxWidthProperty);
        }

        /// <summary>
        /// Tabs the panel_ tab panel item changed.
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void TabPanel_TabPanelItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RemoveVisualChild((e.OldValue as Control));
            AddVisualChild((e.NewValue as Control));
        }

        /// <summary>
        /// Calls OnSeparatorColorChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSeparatorColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabPanel instance = (TabPanel)d;
            instance.OnSeparatorColorChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SeparatorColorChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnSeparatorColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SeparatorColorChanged != null)
            {
                SeparatorColorChanged(this, e);
            }
        }
        #endregion
    }

    #region Internal declarations

    /// <summary>
    /// Represents the interval class
    /// </summary>
    internal class Interval
    {
        #region Fields
        /// <summary>
        /// Represents a double value
        /// </summary>
        public double A = 0;

        /// <summary>
        /// Represents a double value
        /// </summary>
        public double B = 0;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public double Length
        {
            get
            {
                return B - A;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval"/> class.
        /// </summary>
        public Interval()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval"/> class.
        /// </summary>
        /// <param name="a">A param value.</param>
        /// <param name="b">The b param value.</param>
        public Interval(double a, double b)
        {
            this.A = a;
            this.B = b;
        }
        #endregion
    }
    #endregion
}
