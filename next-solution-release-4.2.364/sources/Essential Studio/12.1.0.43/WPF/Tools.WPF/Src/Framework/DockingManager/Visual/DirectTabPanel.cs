// <copyright file="DirectTabPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Arranges child elements into a single line that can be oriented horizontally
    /// or vertically. Used as TabItems container for TabControl and SidePanel.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DirectTabPanel : Panel
    {
        #region Constants
        /// <summary>
        /// Specifies min step.
        /// </summary>
        private const double MinStep = 1;

        /// <summary>
        /// Specifies prefix name.
        /// </summary>
        private const string PrefixName = "InternalName";
        #endregion

        #region Private member

        /// <summary>
        /// checks whether panel has been measured.
        /// </summary>
        private bool m_panelmeasured = false;

        /// <summary>
        /// stores the previous arranged size of panel
        /// </summary>
        private Size m_previousarrangesize = new Size(0, 0);

        /// <summary>
        /// Specifies header height.
        /// </summary>
        private double m_headerHeight;

        /// <summary>
        /// Specifies name creator.
        /// </summary>
        public static int m_nameCreator = 0;
        #endregion

        #region Private properties
        /// <summary>
        /// Gets the tab strip placement.
        /// </summary>
        /// <value>The tab strip placement.</value>
        private Dock TabStripPlacement
        {
            get
            {
                Dock dock = Dock.Top;
                TabControl control = TemplatedParent as TabControl;

                if (null != control)
                {
                    dock = control.TabStripPlacement;
                }

                return dock;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the System.Windows.FrameworkElement.Initialized event. This method
        /// is invoked whenever DockingManager.IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.AllowDrop = true;

            TabControl tabControl = TemplatedParent as TabControl;

            if (null != tabControl && string.IsNullOrEmpty(tabControl.Name))
            {
                tabControl.Name = PrefixName + (++m_nameCreator);
            }
        }

        /// <summary>
        /// Called when [tab item selected].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        internal void OnTabItemSelected(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if ((sender as TabItem).Content != null && ((sender as TabItem).Content as DependencyObject) != null)
                {
                    if (DockingManager.GetDockWindowState((sender as TabItem).Content as DependencyObject) == WindowState.Minimized)
                    {
                        DockingManager owner = DockingManager.ResolveManager((sender as TabItem).Content as UIElement);
                        if (owner != null)
                        {
                            FrameworkElement element = (sender as TabItem).Content as FrameworkElement;
                            bool bCanExecute = owner.DockFill && owner.FilterChildren(DockState.Document).Count > 0
                                && owner.DockFillDocumentMode == DockFillDocumentMode.Fill ? false : true;
                            if (bCanExecute)
                            {
                                owner.ExecuteUnAutoHide(element);
                                DockingManager.SetDockWindowState(element as DependencyObject, WindowState.Normal);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Measures the child elements of a <see cref="DirectTabPanel"/> in anticipation of arranging them during 
        /// the DirectTabPanel.ArrangeOverride( Size ) pass.
        /// </summary>
        /// <param name="constraint">An upper limit Size that should not be exceeded.</param>
        /// <returns>The Size that represents the desired size of the element.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            m_headerHeight = 0;
            m_panelmeasured = true;
            switch (TabStripPlacement)
            {
                case Dock.Top:
                case Dock.Bottom:
                    return MeasureHorizontal(constraint);

                case Dock.Left:
                case Dock.Right:
                    return MeasureVertical(constraint);

                default:
                    throw new NotImplementedException("This '" + TabStripPlacement + "' value incorrect for enum of Dock!");
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
            if (tabbedhost != null)
            {
                DockingManager docking = tabbedhost.DockingManager;
                if (docking != null && e.StylusDevice == null || e.StylusDevice != null)
                {
                    docking.OnDirectTabPanelMouseDown(this, e);
                }
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
            if (tabbedhost != null)
            {
                DockingManager docking = tabbedhost.DockingManager;
                if (docking != null && e.StylusDevice == null || e.StylusDevice != null)
                {
                    docking.OnDirectTabPanelMouseUp(this, e);
                }
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
            if (tabbedhost != null)
            {
                DockingManager docking = tabbedhost.DockingManager;
                if (docking != null)
                {
                    docking.OnDirectTabPanelDragOver(this, e);
                }
            }
            base.OnDragOver(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
                if (tabbedhost != null)
                {
                    DockingManager docking = tabbedhost.DockingManager;
                    if (docking != null)
                    {
                        docking.OnDirectTabPanelMouseMove(this, e);
                    }
                }
                base.OnMouseMove(e);
            }
        }

#if !SyncfusionFramework3_5
        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
        //    if (tabbedhost != null)
        //    {
        //        DockingManager DockingManager = tabbedhost.DockingManager;
        //        if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        {
        //            base.OnTouchDown(e);
        //            OnTouchLeftFingerDown(e);
        //        }
        //    }
        //}

        //protected override void OnTouchMove(TouchEventArgs e)
        //{
        //    DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
        //    if (tabbedhost != null)
        //    {
        //        DockingManager DockingManager = tabbedhost.DockingManager;
        //        if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        {
        //            if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //                OnTouchLeftFingerDown(e);
        //            else
        //            {
        //                DockingManager.OnDirectTabPanelTouchMove(this, e);
        //            }
        //        }
        //    }
        //    base.OnTouchMove(e);
        //}

        //protected override void OnTouchUp(TouchEventArgs e)
        //{
        //    DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
        //    if (tabbedhost != null)
        //    {
        //        DockingManager DockingManager = tabbedhost.DockingManager;
        //        if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        {
        //            #region TouchLeftFingerUp
        //            if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //                OnTouchLeftFingerUp(e);
        //            #endregion
        //        }
        //    }
        //    base.OnTouchUp(e);
        //}

        //private void OnTouchLeftFingerDown(TouchEventArgs e)
        //{
        //    DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
        //    if (tabbedhost != null)
        //    {
        //        DockingManager docking = tabbedhost.DockingManager;
        //        if (docking != null)
        //        {
        //            docking.OnDirectTabPanelTouchDown(this, e);
        //        }
        //    }
        //}

        //private void OnTouchLeftFingerUp(TouchEventArgs e)
        //{
        //    DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
        //    if (tabbedhost != null)
        //    {
        //        DockingManager docking = tabbedhost.DockingManager;
        //        if (docking != null)
        //        {
        //            docking.OnDirectTabPanelTouchUp(this, e);
        //        }
        //    }
        //}

        //void tabItem_TouchUp(object sender, TouchEventArgs e)
        //{
        //    DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
        //    if (tabbedhost != null)
        //    {
        //        DockingManager DockingManager = tabbedhost.DockingManager;
        //        if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        {
        //            #region tabItem_TouchLeftFingerUp
        //            if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //            {
        //                tabItem_TouchLeftFingerUp(sender, e);
        //            }
        //            #endregion
        //        }
        //    }
        //}

        //void tabItem_TouchLeftFingerUp(object sender, TouchEventArgs e)
        //{
        //    if ((sender as TabItem).Content != null && ((sender as TabItem).Content as DependencyObject) != null)
        //    {
        //        if (DockingManager.GetDockWindowState((sender as TabItem).Content as DependencyObject) == WindowState.Minimized)
        //        {
        //            DockingManager owner = DockingManager.ResolveManager((sender as TabItem).Content as UIElement);
        //            if (owner != null)
        //            {
        //                FrameworkElement element = (sender as TabItem).Content as FrameworkElement;
        //                bool bCanExecute = owner.DockFill && owner.FilterChildren(DockState.Document).Count > 0
        //                    && owner.DockFillDocumentMode == DockFillDocumentMode.Fill ? false : true;
        //                if (bCanExecute)
        //                {
        //                    owner.ExecuteUnAutoHide(element);
        //                    DockingManager.SetDockWindowState(element as DependencyObject, WindowState.Normal);
        //                }
        //            }
        //        }
        //    }
        //}

        //protected override void OnLostTouchCapture(TouchEventArgs e)
        //{
        //    DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
        //    if (tabbedhost != null)
        //    {
        //        DockingManager DockingManager = tabbedhost.DockingManager;
        //        if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        {
        //            DockingManager.OnDirectTabPanelLostTouchCapture(this,e);
        //            base.OnLostTouchCapture(e);
        //        }
        //    }
        //}

#endif
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DockedElementTabbedHost tabbedhost = VisualUtils.FindAncestor(this, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
                if (tabbedhost != null)
                {
                    DockingManager docking = tabbedhost.DockingManager;
                    if (docking != null)
                    {
                        docking.OnDirectTabPanelLostCapture(this, e);
                    }
                }
                base.OnLostMouseCapture(e);
            }
        }

        /// <summary>
        /// Arranges the content (child elements) of a <see cref="DirectTabPanel"/> element.
        /// </summary>
        /// <param name="arrangeSize">The Size this element uses to arrange its child elements.</param>
        /// <returns>The Size that represents the arranged size of this DirectTabPanel element.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            switch (TabStripPlacement)
            {
                case Dock.Top:
                case Dock.Bottom:
                    if (!m_panelmeasured && IsLoaded)
                    {
                        Measure(arrangeSize);
                    }
                    ArrangeHorizontal();
                    break;

                case Dock.Left:
                case Dock.Right:
                    if (!m_panelmeasured && IsLoaded)
                    {
                        Measure(arrangeSize);
                    }
                    ArrangeVertical(arrangeSize);
                    break;
            }
            m_panelmeasured = false;
            return arrangeSize;
        }

        /// <summary>
        /// Called when the visual children of a <see cref="DirectTabPanel"/> element change.
        /// </summary>
        /// <param name="visualAdded">Identifies the visual child that's added.</param>
        /// <param name="visualRemoved">Identifies the visual child that's removed.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            TabItem tabItem = visualAdded as TabItem;

            if (null != tabItem && string.IsNullOrEmpty(tabItem.Name))
            {
                tabItem.MouseLeftButtonUp += new MouseButtonEventHandler(OnTabItemSelected);
#if !SyncfusionFramework3_5
                //tabItem.TouchUp += tabItem_TouchUp;
#endif
                tabItem.Unloaded += new RoutedEventHandler(tabItem_Unloaded);
                tabItem.Name = string.Concat(PrefixName, ++m_nameCreator);
            }

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        void tabItem_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as TabItem).MouseLeftButtonUp -= new MouseButtonEventHandler(OnTabItemSelected);
#if !SyncfusionFramework3_5
            //(sender as TabItem).TouchUp -= tabItem_TouchUp;
#endif
            (sender as TabItem).Unloaded -= new RoutedEventHandler(tabItem_Unloaded);
        }

        /// <summary>
        /// Arranges the horizontal.
        /// </summary>
        private void ArrangeHorizontal()
        {
            Vector vector = new Vector();
            double[] headerWidth = GetHeadersWidth();
            UIElementCollection internalChildren = InternalChildren;
            for (int i = 0, cnt = internalChildren.Count; i < cnt; ++i)
            {
                Size size = new Size(headerWidth[i], m_headerHeight);
                internalChildren[i].Arrange(new Rect(vector.X, vector.Y, size.Width, size.Height));
                vector.X += size.Width;
            }
        }

        /// <summary>
        /// Arranges the vertical.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        private void ArrangeVertical(Size arrangeSize)
        {
            double y = 0;

            foreach (TabItem element in InternalChildren)
            {
                if (element.Visibility != Visibility.Collapsed)
                {
                    Size desiredSizeWithoutMargin = GetDesiredSizeWithoutMargin(element);
                    element.Arrange(new Rect(0, y, arrangeSize.Width, desiredSizeWithoutMargin.Height));
                    y += desiredSizeWithoutMargin.Height;
                }
            }
        }

        /// <summary>
        /// Measures the horizontal.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns>return size.</returns>
        private Size MeasureHorizontal(Size constraint)
        {
            return ChildMeasureHorizontal(constraint, constraint.Width);
        }

        /// <summary>
        /// Childs the measure horizontal.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="parentWidth">Width of the parent.</param>
        /// <returns>return size.</returns>
        private Size ChildMeasureHorizontal(Size constraint, double parentWidth)
        {
            Size returnSize = new Size();
            bool isNoFirstCycle = false;
            double sumWidth = 0;
            DockingManager owner = null;
            SidePanel m_sidepanel = null;
            ScrollViewer m_scrollinfo = null;
            ScrollButtonsBar m_scrollingbuttons = null;
            Border m_borderpanel = null;
            if (InternalChildren.Count > 0)
            {
                owner = VisualUtils.FindAncestor((Visual)InternalChildren[0], typeof(DockingManager)) as DockingManager;
            }
            if (owner != null && owner.EnableScrollableSidePanel)
            {
                m_sidepanel = VisualUtils.FindAncestor((Visual)this, typeof(SidePanel)) as SidePanel;
                if (m_sidepanel != null)
                {
                    m_scrollinfo = m_sidepanel.Template.FindName("PART_ScrollPanel", m_sidepanel) as ScrollViewer;
                    m_scrollingbuttons = m_sidepanel.Template.FindName("PART_ScrollButtons", m_sidepanel) as ScrollButtonsBar;
                    m_borderpanel = m_sidepanel.Template.FindName("PART_BorderName", m_sidepanel) as Border;
                    if (m_sidepanel.ActualWidth > 0)
                    {
                        parentWidth = m_sidepanel.ActualWidth;
                    }
                    else if (m_scrollinfo != null)
                    {
                        parentWidth = m_scrollinfo.ViewportWidth;
                    }
                }
                foreach (TabItem element in InternalChildren)
                {
                    if (Visibility.Collapsed != element.Visibility)
                    {
                        element.Measure(constraint);
                        Size desiredSizeWithoutMargin = GetDesiredSizeWithoutMargin(element);

                        if (m_headerHeight < desiredSizeWithoutMargin.Height)
                        {
                            m_headerHeight = desiredSizeWithoutMargin.Height;
                        }

                        isNoFirstCycle = true;
                        sumWidth += desiredSizeWithoutMargin.Width;
                    }
                }

                if (sumWidth > parentWidth && isNoFirstCycle)
                {
                    if (m_scrollingbuttons != null && m_sidepanel != null && m_borderpanel != null)
                    {
                        m_scrollingbuttons.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (m_scrollingbuttons != null && m_sidepanel != null && m_borderpanel != null
                        && owner.ScrollButtonMode == ScrollingButtonMode.Normal)
                    {
                        m_scrollingbuttons.Visibility = Visibility.Collapsed;
                        Canvas.SetLeft(m_borderpanel, 0);
                    }
                    else if (m_scrollingbuttons != null && m_sidepanel != null && m_borderpanel != null
                        && owner.ScrollButtonMode == ScrollingButtonMode.Extended)
                    {
                        m_scrollingbuttons.Visibility = Visibility.Visible;                        

                        if (Canvas.GetLeft(m_borderpanel) != m_scrollingbuttons.ActualWidth)
                            Canvas.SetLeft(m_borderpanel, m_scrollingbuttons.ActualWidth);
                    }
                }
            }
            else
            {
                foreach (TabItem element in InternalChildren)
                {
                    if (Visibility.Collapsed != element.Visibility)
                    {
                        element.Measure(constraint);
                        Size desiredSizeWithoutMargin = GetDesiredSizeWithoutMargin(element);

                        if (m_headerHeight < desiredSizeWithoutMargin.Height)
                        {
                            m_headerHeight = desiredSizeWithoutMargin.Height;
                        }

                        if ((sumWidth + desiredSizeWithoutMargin.Width) > parentWidth && isNoFirstCycle)
                        {
                            if (element.MinWidth < constraint.Width)
                            {
                                Size newSize = new Size(constraint.Width - MinStep, constraint.Height);
                                sumWidth = ChildMeasureHorizontal(newSize, parentWidth).Width;
                            }

                            break;
                        }
                        else
                        {
                            sumWidth += desiredSizeWithoutMargin.Width;
                        }

                        isNoFirstCycle = true;
                    }
                }
            }

            returnSize.Height = m_headerHeight;
            returnSize.Width = sumWidth;

            return returnSize;
        }

        /// <summary>
        /// Measures the vertical.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns>return size.</returns>
        private Size MeasureVertical(Size constraint)
        {
            return ChildMeasureVertical(constraint, constraint.Height);
        }

        /// <summary>
        /// Childs the measure vertical.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="parentHeight">Height of the parent.</param>
        /// <returns>return size.</returns>
        private Size ChildMeasureVertical(Size constraint, double parentHeight)
        {
            Size returnSize = new Size();
            bool isNoFirstCycle = false;
            double sumHeight = 0;
            DockingManager owner = null;
            SidePanel m_sidepanel = null;
            ScrollViewer m_scrollinfo=null;
            ScrollButtonsBar m_scrollingbuttons = null;
            Border m_borderpanel = null;
            if (InternalChildren.Count > 0)
            {
                owner = VisualUtils.FindAncestor((Visual)InternalChildren[0], typeof(DockingManager)) as DockingManager;
            }
            if (owner != null && owner.EnableScrollableSidePanel)
            {
                m_sidepanel = VisualUtils.FindAncestor((Visual)this, typeof(SidePanel)) as SidePanel;
                if (m_sidepanel != null)
                {
                    m_scrollinfo = m_sidepanel.Template.FindName("PART_ScrollPanel",m_sidepanel) as ScrollViewer;
                    m_scrollingbuttons = m_sidepanel.Template.FindName("PART_ScrollButtons", m_sidepanel) as ScrollButtonsBar;
                    m_borderpanel = m_sidepanel.Template.FindName("PART_BorderName", m_sidepanel) as Border;

                    if (m_sidepanel.ActualHeight > 0)
                    {
                        parentHeight = m_sidepanel.ActualHeight;
                    }
                    else if (m_scrollinfo != null)
                    {
                        parentHeight = m_scrollinfo.ViewportHeight;
                    }
                }
                foreach (TabItem element in InternalChildren)
                {
                    if (Visibility.Collapsed != element.Visibility)
                    {
                        element.Measure(constraint);
                        Size desiredSizeWithoutMargin = GetDesiredSizeWithoutMargin(element);

                        if (m_headerHeight < desiredSizeWithoutMargin.Width)
                        {
                            m_headerHeight = desiredSizeWithoutMargin.Width;
                        }
                        isNoFirstCycle = true;
                        sumHeight += desiredSizeWithoutMargin.Height;
                    }
                }

                if (sumHeight  > parentHeight && isNoFirstCycle)
                {
                    if (m_scrollingbuttons != null && m_sidepanel!=null && m_borderpanel!=null)
                    {
                        m_scrollingbuttons.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (m_scrollingbuttons != null && m_sidepanel != null && m_borderpanel != null
                        && owner.ScrollButtonMode == ScrollingButtonMode.Normal)
                    {
                        m_scrollingbuttons.Visibility = Visibility.Collapsed;                        
                        Canvas.SetTop(m_borderpanel, 0);
                    }
                    else if (m_scrollingbuttons != null && m_sidepanel != null && m_borderpanel != null 
                        && owner.ScrollButtonMode == ScrollingButtonMode.Extended)
                    {
                        m_scrollingbuttons.Visibility = Visibility.Visible;

                        if (Canvas.GetTop(m_borderpanel) != m_scrollingbuttons.ActualWidth)
                            Canvas.SetTop(m_borderpanel, m_scrollingbuttons.ActualWidth);
                        
                    }
                }

            }
            else
            {
                foreach (TabItem element in InternalChildren)
                {
                    if (Visibility.Collapsed != element.Visibility)
                    {
                        element.Measure(constraint);
                        Size desiredSizeWithoutMargin = GetDesiredSizeWithoutMargin(element);

                        if (m_headerHeight < desiredSizeWithoutMargin.Width)
                        {
                            m_headerHeight = desiredSizeWithoutMargin.Width;
                        }

                        if ((sumHeight + desiredSizeWithoutMargin.Height) > parentHeight && isNoFirstCycle)
                        {
                            if (element.MinHeight < constraint.Height)
                            {
                                Size newSize = new Size(constraint.Width, constraint.Height - MinStep);
                                sumHeight = ChildMeasureVertical(newSize, parentHeight).Height;
                            }

                            break;
                        }
                        else
                        {
                            sumHeight += desiredSizeWithoutMargin.Height;
                        }

                        isNoFirstCycle = true;
                    }
                }
            }

            returnSize.Height = sumHeight;
            returnSize.Width = m_headerHeight;

            return returnSize;
        }

        /// <summary>
        /// Gets the width of the headers.
        /// </summary>
        /// <returns>return double.</returns>
        private double[] GetHeadersWidth()
        {
            UIElementCollection internalChildren = InternalChildren;
            int childrenCount = internalChildren.Count;
            double[] headersSize = new double[childrenCount];

            for (int i = 0; i < childrenCount; ++i)
            {
                if (Visibility.Collapsed != internalChildren[i].Visibility)
                {
                    Size desiredSizeWithoutMargin = GetDesiredSizeWithoutMargin(internalChildren[i] as TabItem);
                    headersSize[i] = desiredSizeWithoutMargin.Width;
                }
                else
                {
                    headersSize[i] = 0;
                }
            }

            return headersSize;
        }

        /// <summary>
        /// Gets the desired size without margin.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return size.</returns>
        internal static Size GetDesiredSizeWithoutMargin(FrameworkElement element)
        {
            Thickness thickness = element.Margin;
            double height = Math.Max(0, (element.DesiredSize.Height - thickness.Top - thickness.Bottom));
            double width = Math.Max(0, (element.DesiredSize.Width - thickness.Left - thickness.Right));
            return new Size(width, height);
        }
        #endregion
    }
}
