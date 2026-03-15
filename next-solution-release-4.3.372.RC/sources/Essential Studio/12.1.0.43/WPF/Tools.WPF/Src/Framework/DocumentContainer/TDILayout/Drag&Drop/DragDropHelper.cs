// <copyright file="DragDropHelper.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.Windows.Controls.Primitives;


namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// TabGroup Adorner
    /// </summary>
    public class TabPreviewAdorner : Adorner
    {
        /// <summary>
        /// Indicates side in which preview occurs
        /// </summary>
        public DockSide side;

        /// <summary>
        /// Indicates target tab control
        /// </summary>
        public DocumentTabControl tabControl;

        /// <summary>
        /// Indicates Dragged tabItem
        /// </summary>
        public TabItemExt item;

        /// <summary>
        /// Indicates source tabcontrol in which item is dragged
        /// </summary>
        public DocumentTabControl m_Source;

        /// <summary>
        /// Constructor for Adorner
        /// </summary>
        /// <param name="adornedElement"></param>
        public TabPreviewAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            AllowDrop = true;
            PreviewDragEnter += new DragEventHandler(TabPreviewAdorner_PreviewDragEnter);
            PreviewDragLeave += new DragEventHandler(TabPreviewAdorner_PreviewDragLeave);
            PreviewDrop += new DragEventHandler(TabPreviewAdorner_PreviewDrop);
            PreviewDragOver += new DragEventHandler(TabPreviewAdorner_PreviewDragOver);
        }

        /// <summary>
        /// Handles PreviewDragOver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TabPreviewAdorner_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (tabControl != null && item != null)
            {
                if (side == DockSide.Right)
                {
                    RemoveAdornerRight(e);
                }
                else if (side == DockSide.Bottom)
                {
                    RemoveAdornerBottom(e);
                }
                e.Handled = true;
            }
        }
        /// <summary>
        /// Removes the Right Adorner from the AdornerLayer
        /// </summary>
        /// <param name="e"></param>
        private void RemoveAdornerRight(DragEventArgs e)
        {
            if (e.GetPosition(this).X < this.RenderSize.Width - 10 || e.GetPosition(this).X > this.RenderSize.Width)
            {
                AdornerLayer layer = Parent as AdornerLayer;
                Adorner[] adorners = layer.GetAdorners(this.AdornedElement);
                foreach (Adorner ador in adorners)
                {
                    layer.Remove(ador);
                }
            }
        }

        /// <summary>
        /// Removes the adorner.
        /// </summary>
        private void RemoveAdorner()
        {
            if (tabControl != null)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(tabControl);
                Adorner[] adorners = layer.GetAdorners(this.AdornedElement);
                foreach (Adorner ador in adorners)
                {
                    layer.Remove(ador);
                }
            }
        }
        /// <summary>
        /// Removes the Bottom Adorner from the AdornerLayer
        /// </summary>
        /// <param name="e"></param>
        private void RemoveAdornerBottom(DragEventArgs e)
        {
            if (e.GetPosition(this).Y < this.RenderSize.Height - 10 || e.GetPosition(this).Y > this.RenderSize.Height)
            {
                if (Parent != null)
                {
                    AdornerLayer layer = Parent as AdornerLayer;
                    Adorner[] adorners = layer.GetAdorners(this.AdornedElement);
                    foreach (Adorner ador in adorners)
                    {
                        layer.Remove(ador);
                    }
                }
            }
        }
        /// <summary>
        /// Handles PreviewDragLeave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TabPreviewAdorner_PreviewDragLeave(object sender, DragEventArgs e)
        {
            if (Parent != null)
            {
                AdornerLayer layer = Parent as AdornerLayer;
                Adorner[] adorners = layer.GetAdorners(this.AdornedElement);
                foreach (Adorner ador in adorners)
                {
                    layer.Remove(ador);
                }
                e.Handled = true;
            }
        }

       
        /// <summary>
        /// Create the TabGroup on Drop
        /// </summary>
        /// <param name="orientation"></param>
        void CreateTabGroup(Orientation orientation)
        {
            TDILayoutPanel panel = VisualUtils.FindAncestor(tabControl, typeof(TDILayoutPanel)) as TDILayoutPanel;
            UIElement element = null;
            if (panel != null)
            {
                ContentPresenter presenter = item.Content as ContentPresenter;
                element = presenter.Content as UIElement;
                if (element != null)
                {
                    panel.DragDropNewTab(element, tabControl, orientation);
                    if (m_Source != tabControl)
                    {
                        if (m_Source != null)
                        {
                            m_Source.Items.Remove(item);
                        }
                        else
                        {
                            TabControl newtabcontrol = VisualUtils.FindAncestor(item, typeof(TabControlExt)) as TabControlExt;
                            if (newtabcontrol != null)
                            {
                                newtabcontrol.Items.Remove(item);
                            }
                            else
                            {
                                tabControl.Items.Remove(item);
                            }
                        }
                    }
                }

            }
        }
        /// <summary>
        /// Handles the Drop event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TabPreviewAdorner_PreviewDrop(object sender, DragEventArgs e)
        {
            if (tabControl != null && item != null && tabControl.Container.TabGroupEnabled)
            {
                if (side == DockSide.Right)
                {
                    RemoveAdornerRight(e);
                    RemoveAdorner();
                    CreateTabGroup(Orientation.Vertical);
                }
                else if (side == DockSide.Bottom)
                {
                    RemoveAdornerBottom(e);
                    RemoveAdorner();
                    CreateTabGroup(Orientation.Horizontal);
                }
            }
        }
        /// <summary>
        /// Preview Drag Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TabPreviewAdorner_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (tabControl != null && m_Source != null)
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// Invokes on Render
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Blue);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Blue), 1);
            //SU I78477
            //double renderRadius = 5.0;
            //EU I78477
            if (side == DockSide.Right)
            {
                adornedElementRect.X = adornedElementRect.Width / 2;
                adornedElementRect.Width = adornedElementRect.Width / 2;
            }
            else if (side == DockSide.Bottom)
            {
                adornedElementRect.Y = adornedElementRect.Height / 2;
                adornedElementRect.Height = adornedElementRect.Height / 2;
            }

            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
        }
    }
    /// <summary>
    /// Class Represents the DragDrop helper for the TDI layout panel
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DragDropHelper : DependencyObject
    {
        #region Private members
        /// <summary>
        /// Presents initialMousePosition
        /// </summary>
        private Point m_initialMousePosition;
        
        /// <summary>
        /// Presents draggedAdorner
        /// </summary>
        private DraggedAdorner m_draggedAdorner;
        
        /// <summary>
        /// Presents InsertAdorner
        /// </summary>
        private InsertionAdorner m_inserAdorner;
        
        /// <summary>
        /// Presents topWindow
        /// </summary>
        private Window m_topWindow;
        
        /// <summary>
        /// Represents the source of Document Tab Control
        /// </summary>
        public static DocumentTabControl m_Source;
        
        /// <summary>
        /// Presents DraggedItem
        /// </summary>
        private TabItemExt m_draggedItem;
        
        /// <summary>
        /// Represents the Target of Document Tab Control
        /// </summary>
        public static DocumentTabControl m_Target;
        
        /// <summary>
        /// Presents OverItem
        /// </summary>
        private TabItemExt m_overItem;
        
        /// <summary>
        /// Presents hasVerticalOrientation
        /// </summary>
        private bool m_hasVerticalOrientation;
        
        /// <summary>
        /// Presents insertionIndex
        /// </summary>
        private int m_insertionIndex;
        
        /// <summary>
        /// Presents isInFirstHalf
        /// </summary>
        private bool m_isInFirstHalf;

        /// <summary>
        /// Indicates tab items grag status.
        /// </summary>
        internal static bool dragTabItemFlag = false;
        
        /// <summary>
        /// Presents Instance
        /// </summary>
        private static DragDropHelper m_instance;

        #endregion

        #region Initialize
        /// <summary>
        /// Prevents a default instance of the <see cref="DragDropHelper"/> class from being created.
        /// </summary>
        private DragDropHelper()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is dragging.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dragging; otherwise, <c>false</c>.
        /// </value>
        public bool IsDragging
        {
            get
            {
                return null != m_draggedItem;
            }
        }
        
        /// <summary>
        /// Gets the dragged item.
        /// </summary>
        /// <value>The dragged item.</value>
        public TabItemExt DraggedItem
        {
            get
            {
                return m_draggedItem;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>DragDrop Helper</returns>
        public static DragDropHelper GetInstance()
        {
            if (null == m_instance)
            {
                m_instance = new DragDropHelper();
            }

            return m_instance;
        }
        
        /// <summary>
        /// Gets the is drag source.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>bool IsDragSourceProperty</returns>
        public static bool GetIsDragSource(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragSourceProperty);
        }
        
        /// <summary>
        /// Sets the is drag source.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsDragSource(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragSourceProperty, value);
        }
        
        /// <summary>
        /// Gets the is drop target.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>bool IsDropTargetProperty</returns>
        public static bool GetIsDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDropTargetProperty);
        }
        
        /// <summary>
        /// Sets the is drop target.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDropTargetProperty, value);
        }
        
        /// <summary>
        /// Gets the drag drop template.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>DataTemplate DragDropTemplateProperty</returns>
        public static DataTemplate GetDragDropTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(DragDropTemplateProperty);
        }
        
        /// <summary>
        /// Sets the drag drop template.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetDragDropTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DragDropTemplateProperty, value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the DragSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                m_Source = (DocumentTabControl)sender;

                Visual visual = e.OriginalSource as Visual;

                m_topWindow = (Window)VisualUtils.FindAncestor(m_Source, typeof(Window));
                m_initialMousePosition = e.GetPosition(m_topWindow);

                m_draggedItem = Utilities.GetItemContainer(m_Source, visual);
                if (m_draggedItem != null && m_draggedItem.Parent != m_Source)
                {
                    m_draggedItem = null;
                    TabControlExt.DragSourceObject = null;
                }
            }
        }
        
        /// <summary>
        /// Handles the PreviewMouseMove event of the DragSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.StylusDevice == null || e.StylusDevice != null)
                {
                    if (m_draggedItem != null)
                    {
                        if (m_Source == null)
                            m_Source = (DocumentTabControl)sender;

                        if (Utilities.IsMovementBigEnough(m_initialMousePosition, e.GetPosition(m_topWindow)))
                        {
                            bool previousAllowDrop = false;
                            if (m_topWindow != null)
                            {
                                previousAllowDrop = m_topWindow.AllowDrop;
                                m_topWindow.AllowDrop = true;
                                m_topWindow.DragEnter += TopWindow_DragEnter;
                                m_topWindow.DragOver += TopWindow_DragOver;
                                m_topWindow.DragLeave += TopWindow_DragLeave;
                            }

                            TabPanelAdv tabpanel = VisualUtils.FindAncestor(e.OriginalSource as FrameworkElement, typeof(TabPanelAdv)) as TabPanelAdv;
                            if (tabpanel != null && tabpanel.Content is ScrollViewer && ((tabpanel.Content as ScrollViewer).Content as TabLayoutPanel) != null)
                            {
                                //scrollingpanel = (tabpanel.Content as TabLayoutPanel).m_scrollingPanel;
                                Border tabpanelborder = tabpanel.Template.FindName("Bord", tabpanel) as Border;
                                if (tabpanelborder != null)
                                    tabpanelborder.AllowDrop = false;
                            }

                            if (!BrowserInteropHelper.IsBrowserHosted)
                            {
                                ContentPresenter presenter = m_draggedItem.Content as ContentPresenter;
                                FrameworkElement element = presenter.Content as FrameworkElement;
                                if (presenter != null && element != null && DockingManager.GetCanDragTab(element))
                                {
                                    if (m_Source != null && m_Source.Container != null && (m_Source.Container.IsTDIDragDropEnabled || (m_Source as TabControlExt).AllowDragDrop))
                                    {
                                        try
                                        {
                                            if ((m_draggedItem as TabItemExt) != null && (m_draggedItem as TabItemExt).Content != null
                                                && ((m_draggedItem as TabItemExt).Content as ContentPresenter) != null
                                                && ((m_draggedItem as TabItemExt).Content as ContentPresenter).Content != null)
                                            {
                                                TabControlExt.DragSourceObject = m_Source;
                                                TabControlExtDragEventArgs args = m_Source.FireDragStart(m_draggedItem);
                                                if (element != null && !args.Cancel && args.DragSource != null)
                                                {
                                                    if (DockingManager.ResolveManager(element as UIElement) != null && DockingManager.ResolveManager(element as UIElement).IsVS2010DraggingEnabled)
                                                    {
                                                        if (DockingManager.GetState(element as DependencyObject) == DockState.Document)
                                                        {
                                                            DockingManager.SetDesiredWidthInFloatingMode(element as DependencyObject, ((m_draggedItem as TabItemExt).Parent as DocumentTabControl).ActualWidth);
                                                            DockingManager.SetDesiredHeightInFloatingMode(element as DependencyObject, ((m_draggedItem as TabItemExt).Parent as DocumentTabControl).ActualHeight);
                                                            DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, DockState.Float);
                                                            if (host != null)
                                                            {
                                                                DockingManager.ResolveManager(element).m_draggedElement = element;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            DragDropEffects effects = DragDrop.DoDragDrop((DependencyObject)sender, m_draggedItem, DragDropEffects.Move);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        DragDropEffects effects = DragDrop.DoDragDrop((DependencyObject)sender, m_draggedItem, DragDropEffects.Move);
                                                    }
                                                }
                                            }
                                        }
                                        //SU I78477
                                        //catch(Exception x) { }
                                        catch (Exception) { }
                                        //EU I78477
                                        RemoveDraggedAdorner();
                                    }
                                }
                                else if (DockingManager.GetCanDragTab(element))
                                {
                                    DragDropEffects effects = DragDrop.DoDragDrop((DependencyObject)sender, m_draggedItem, DragDropEffects.Move);
                                    RemoveDraggedAdorner();
                                }

                            }

                            if (m_topWindow != null)
                            {
                                m_topWindow.AllowDrop = previousAllowDrop;
                                m_topWindow.DragEnter -= TopWindow_DragEnter;
                                m_topWindow.DragOver -= TopWindow_DragOver;
                                m_topWindow.DragLeave -= TopWindow_DragLeave;
                            }

                            m_draggedItem = null;
                            TabControlExt.DragSourceObject = null;
                        }
                    }
                }
            }
            catch
            {

            }
        }

#if !SyncfusionFramework3_5
        internal void dragSource_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (m_Source != null && m_Source.Container != null && m_Source.Container.IsTouchEnabled)
            {
                if (m_draggedItem != null)
                {
                    if (m_Source == null)
                        m_Source = (DocumentTabControl)sender;

                    if (Utilities.IsMovementBigEnough(m_initialMousePosition, e.GetTouchPoint(m_topWindow).Position))
                    {
                        bool previousAllowDrop = false;
                        if (m_topWindow != null)
                        {
                            previousAllowDrop = m_topWindow.AllowDrop;
                            m_topWindow.AllowDrop = true;
                            m_topWindow.DragEnter += TopWindow_DragEnter;
                            m_topWindow.DragOver += TopWindow_DragOver;
                            m_topWindow.DragLeave += TopWindow_DragLeave;
                        }

                        TabPanelAdv tabpanel = VisualUtils.FindAncestor(e.OriginalSource as FrameworkElement, typeof(TabPanelAdv)) as TabPanelAdv;
                        if (tabpanel != null && ((tabpanel.Content as ScrollViewer).Content as TabLayoutPanel) != null)
                        {
                            //scrollingpanel = (tabpanel.Content as TabLayoutPanel).m_scrollingPanel;
                            Border tabpanelborder = tabpanel.Template.FindName("Bord", tabpanel) as Border;
                            if (tabpanelborder != null)
                                tabpanelborder.AllowDrop = false;
                        }

                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            ContentPresenter presenter = m_draggedItem.Content as ContentPresenter;
                            FrameworkElement element = presenter.Content as FrameworkElement;
                            if (presenter != null && element != null && DockingManager.GetCanDragTab(element))
                            {
                                if (m_Source != null && m_Source.Container != null && (m_Source.Container.IsTDIDragDropEnabled || (m_Source as TabControlExt).AllowDragDrop))
                                {
                                    try
                                    {
                                        if ((m_draggedItem as TabItemExt) != null && (m_draggedItem as TabItemExt).Content != null
                                            && ((m_draggedItem as TabItemExt).Content as ContentPresenter) != null
                                            && ((m_draggedItem as TabItemExt).Content as ContentPresenter).Content != null)
                                        {
                                            TabControlExt.DragSourceObject = m_Source;
                                            TabControlExtDragEventArgs args = m_Source.FireDragStart(m_draggedItem);
                                            if (element != null && !args.Cancel && args.DragSource != null)
                                            {
                                                if (DockingManager.ResolveManager(element as UIElement) != null && DockingManager.ResolveManager(element as UIElement).IsVS2010DraggingEnabled)
                                                {
                                                    if (DockingManager.GetState(element as DependencyObject) == DockState.Document)
                                                    {
                                                        DockingManager.SetDesiredWidthInFloatingMode(element as DependencyObject, ((m_draggedItem as TabItemExt).Parent as DocumentTabControl).ActualWidth);
                                                        DockingManager.SetDesiredHeightInFloatingMode(element as DependencyObject, ((m_draggedItem as TabItemExt).Parent as DocumentTabControl).ActualHeight);
                                                        DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, DockState.Float);
                                                        if (host != null)
                                                        {
                                                            DockingManager.ResolveManager(element).m_draggedElement = element;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        DragDropEffects effects = DragDrop.DoDragDrop((DependencyObject)sender, m_draggedItem, DragDropEffects.Move);
                                                    }
                                                }
                                                else
                                                {
                                                    DragDropEffects effects = DragDrop.DoDragDrop((DependencyObject)sender, m_draggedItem, DragDropEffects.Move);
                                                }
                                            }
                                        }
                                    }
                                    //SU I78477
                                    //catch(Exception x) { }
                                    catch (Exception) { }
                                    //EU I78477
                                    RemoveDraggedAdorner();
                                }
                            }
                            else if (DockingManager.GetCanDragTab(element))
                            {
                                DragDropEffects effects = DragDrop.DoDragDrop((DependencyObject)sender, m_draggedItem, DragDropEffects.Move);
                                RemoveDraggedAdorner();
                            }

                        }

                        if (m_topWindow != null)
                        {
                            m_topWindow.AllowDrop = previousAllowDrop;
                            m_topWindow.DragEnter -= TopWindow_DragEnter;
                            m_topWindow.DragOver -= TopWindow_DragOver;
                            m_topWindow.DragLeave -= TopWindow_DragLeave;
                        }

                        m_draggedItem = null;
                        TabControlExt.DragSourceObject = null;
                    }
                }
            }
        }

        internal void dragSource_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            m_Source = (DocumentTabControl)sender;
            if (m_Source != null && m_Source.Container != null && m_Source.Container.IsTouchEnabled)
            {
                #region PreviewTouchLeftFingerDown

                Visual visual = e.OriginalSource as Visual;

                m_topWindow = (Window)VisualUtils.FindAncestor(m_Source, typeof(Window));
                m_initialMousePosition = e.GetTouchPoint(m_topWindow).Position;

                m_draggedItem = Utilities.GetItemContainer(m_Source, visual);
                if (m_draggedItem != null && m_draggedItem.Parent != m_Source)
                {
                    m_draggedItem = null;
                    TabControlExt.DragSourceObject = null;
                }
                #endregion
            }
        }

        internal void dragSource_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            if (m_Source != null && m_Source.Container != null && m_Source.Container.IsTouchEnabled)
            {
                #region PreviewTouchLeftFingerUp
                if (m_Source.Container.m_documentContainerSystemGesture == SystemGesture.Tap)
                {
                    m_draggedItem = null;
                    TabControlExt.DragSourceObject = null;
                }
                #endregion
            }
        }

        
#endif

        internal void SetNull()
        {
            m_topWindow = null;
            m_Source = null;
            m_Target = null;
            m_overItem = null;
            m_instance = null;
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonUp event of the DragSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                m_draggedItem = null;
                TabControlExt.DragSourceObject = null;
            }
        }

        /// <summary>
        /// Handles the PreviewDragEnter event of the DropTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            m_Target = (DocumentTabControl)sender;
            if (m_Source != null && m_Target != null && m_Target.Container != m_Source.Container)
            {
                return;
            }
            DecideDropTarget(e);
            DocumentTabControl overTabControl = m_overItem != null? m_overItem.TabControlParent as DocumentTabControl: null;

            if (overTabControl != null && overTabControl.Items.Contains(m_overItem))
                overTabControl = null;

            bool canShowPopup = overTabControl != null ? overTabControl.Container == m_Source.Container  : true;

            double actualwidth = 0.0;
            double tabitemswidth = 0.0;
            if (e.OriginalSource is FrameworkElement && m_overItem != null)
            {
                TabPanelAdv tabpanel = VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabPanelAdv)) as TabPanelAdv;
                if (tabpanel != null && (tabpanel.Content is ScrollViewer))
                {
                    TabLayoutPanel tablayoutpanel = (tabpanel.Content as ScrollViewer).Content as TabLayoutPanel;
                    if (tablayoutpanel != null)
                    {
                        actualwidth = tablayoutpanel.ActualWidth;
                        double[] headersize = tablayoutpanel.GetHeadersSize();
                        tabitemswidth = tablayoutpanel.m_scrollInfo.Offset;
                        foreach (double width in headersize)
                        {
                            tabitemswidth += width;
                            if (tabitemswidth > actualwidth)
                            {
                                tabitemswidth -= width;
                                break;
                            }
                        }
                        tabitemswidth += m_overItem.ActualWidth / 2;
                        if ((e.GetPosition(tabpanel).X > tabitemswidth && !m_isInFirstHalf) || e.GetPosition(tabpanel).X > actualwidth)
                        {
                            canShowPopup = false;
                        }
                    }
                }
            }
            
            if (canShowPopup)
            {
                if (m_Target.Container.IsTabPreviewEnabled && m_draggedItem != null && m_Target.Container.TabGroupEnabled)
                {
                    bool right = e.GetPosition(m_Target).X > m_Target.RenderSize.Width - 10;
                    bool bottom = e.GetPosition(m_Target).Y > m_Target.RenderSize.Height - 10;
                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(m_Target);
                    TabPreviewAdorner adorner = new TabPreviewAdorner(m_Target);
                    adorner.AllowDrop = true;
                    if (right ^ bottom && m_Source != null && m_Source.Items.Count > 1)
                    {
                        if (right)
                        {
                            RemoveAllAdornerLayers();
                            adorner.side = DockSide.Right;
                            adorner.tabControl = m_Target;
                            adorner.m_Source = m_Source;
                            adorner.item = m_draggedItem;
                            layer.Add(adorner);
                        }
                        else if (bottom)
                        {
                            RemoveAllAdornerLayers();
                            adorner.side = DockSide.Bottom;
                            adorner.tabControl = m_Target;
                            adorner.item = m_draggedItem;
                            layer.Add(adorner);
                        }
                    }
                    else
                    {
                        Adorner[] adorners = layer.GetAdorners(m_Target);
                        if (adorners != null)
                        {
                            foreach (Adorner ador in adorners)
                            {
                                layer.Remove(ador);
                            }
                        }
                    }
                }
                if (null != m_draggedItem && m_topWindow != null)
                {
                    ShowDraggedAdorner(e.GetPosition(m_topWindow));
                    double tabpaneltotal = 0.0;
                    double tabitemtotalwidth = 0.0;
                    //double scrollpanel;
                    TabPanelAdv tabpanel = null;
                    if (e.OriginalSource is Border && (e.OriginalSource as Border).Child is DockPanel)
                    {
                        UIElementCollection temp = ((e.OriginalSource as Border).Child as DockPanel).Children as UIElementCollection;

                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (temp[i] is ToggleButton)
                            {
                                tabpaneltotal = tabpaneltotal + (temp[i] as ToggleButton).ActualWidth;
                            }
                        }
                        if (e.OriginalSource is FrameworkElement)
                            tabpanel = VisualUtils.FindAncestor(e.OriginalSource as FrameworkElement, typeof(TabPanelAdv)) as TabPanelAdv;
                        //if (tabpanel != null && (tabpanel.Content as TabLayoutPanel).m_scrollingPanel != null)
                        //{
                        //    //scrollpanel = (tabpanel.Content as TabLayoutPanel).m_scrollingPanel.ActualWidth;
                        //    //tabpaneltotal = tabpaneltotal + scrollpanel;
                        //}
                        if (tabpanel != null)
                        {
                            if (tabpanel.Content is TabLayoutPanel)
                                tabitemtotalwidth = tabpanel != null ? (tabpanel.Content as TabLayoutPanel).ActualWidth - tabpaneltotal : 0.0;
                            else if (tabpanel.Content is TabScrollViewer)
                                tabitemtotalwidth = tabpanel != null ? ((tabpanel.Content as TabScrollViewer).Content as TabLayoutPanel).ActualWidth - tabpaneltotal : 0.0;
                        }
                    }
                    if (e.OriginalSource is FrameworkElement)
                        tabpanel = VisualUtils.FindAncestor(e.OriginalSource as FrameworkElement, typeof(TabPanelAdv)) as TabPanelAdv;
                    if (e.OriginalSource is FrameworkElement && tabpanel != null && (e.GetPosition(tabpanel).X < tabitemtotalwidth || tabitemtotalwidth == 0.0))
                        CreateInsertionAdorner();
                    e.Handled = true;
                }
            }
        }
        
        /// <summary>
        /// Handles the PreviewDragOver event of the DropTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            DecideDropTarget(e);

            if (null != m_draggedItem)
            {
                DocumentTabControl overTabControl = m_overItem != null? m_overItem.TabControlParent as DocumentTabControl: null;

                if (overTabControl != null && overTabControl.Items.Contains(m_overItem) && overTabControl.Items.Contains(m_draggedItem))
                    overTabControl = null;

                bool canShowPopup = overTabControl != null ? overTabControl.Container == m_Source.Container : true;

                

                double actualwidth = 0.0;
                double tabitemswidth = 0.0;
                if (e.OriginalSource is FrameworkElement)
                {
                    TabPanelAdv tabpanel = VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabPanelAdv)) as TabPanelAdv;
                    if (tabpanel != null && (tabpanel.Content is ScrollViewer))
                    {
                        TabLayoutPanel tablayoutpanel = (tabpanel.Content as ScrollViewer).Content as TabLayoutPanel;
                        if (tablayoutpanel != null)
                        {
                            actualwidth = tablayoutpanel.ActualWidth;
                            double[] headersize = tablayoutpanel.GetHeadersSize();
                            tabitemswidth = tablayoutpanel.m_scrollInfo.Offset;
                            foreach (double width in headersize)
                            {
                                tabitemswidth += width;
                                if (tabitemswidth > actualwidth)
                                {
                                    tabitemswidth -= width;
                                    break;
                                }
                            }
                            tabitemswidth += m_overItem.ActualWidth / 2;
                            if ((e.GetPosition(tabpanel).X > tabitemswidth && !m_isInFirstHalf) || e.GetPosition(tabpanel).X > actualwidth)
                            {
                                canShowPopup = false;
                            }
                        }
                    }
                }
                

                if (canShowPopup)
                {
                    ShowDraggedAdorner(e.GetPosition(m_topWindow));
                    UpdateInsertionAdornerPosition();
                    e.Handled = true;
                }
            }
        }
        
        /// <summary>
        /// Handles the PreviewDrop event of the DropTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            RemoveDraggedAdorner();
            RemoveInsertionAdorner();
            dragTabItemFlag = true;
            FrameworkElement elelemt = e.OriginalSource as FrameworkElement;
            DocumentTabControl overTabControl = m_overItem != null ? m_overItem.TabControlParent as DocumentTabControl : null;
            bool canDrop = overTabControl != null ? overTabControl.Container == m_Source.Container : true;
            bool isdynamictabgrpenabled = overTabControl != null ? overTabControl.Container.TabGroupEnabled : false; 
            if (overTabControl != null)
            {
                TabControlExtDragEventArgs args = overTabControl.FireDragEnd(m_draggedItem);
                canDrop = !args.Cancel;
            }
            if (canDrop && m_insertionIndex != -1)
            {
                if (null != elelemt && (null != VisualUtils.FindAncestor(elelemt, typeof(TabPanelAdv)) || elelemt is HeaderPanel) && m_Source != null && m_Target != null && m_Source.Container == m_Target.Container)
                {
                    int indexRemoved = -1;

                    if (m_draggedItem != null)
                    {
                        bool flag = DetectSameIndexTarget(m_Source, m_Target, m_draggedItem);
                        if ((e.Effects & DragDropEffects.Move) != 0 && flag)
                        {
                            indexRemoved = Utilities.RemoveItemFromItemsControl(m_Source, m_draggedItem);
                            if (indexRemoved != -1)
                            {
                                UIElement element = m_draggedItem.Content is ContentPresenter ? (m_draggedItem.Content as ContentPresenter).Content as UIElement
                                    : m_draggedItem.Content as UIElement;
                                if (m_Source.TabPositionCache.Contains(element))
                                {
                                    m_Source.TabPositionCache.Remove(element);
                                    m_Source.UpdateTabPositionCache(m_Source.TabPositionCache);
                                }
                            }
                        }

                        if (indexRemoved != -1 && m_Source == m_Target && indexRemoved < m_insertionIndex)
                        {
                            m_insertionIndex--;
                        }
                        if (flag)
                        {
                            Utilities.InsertItemInItemsControl(m_Target, m_draggedItem, m_insertionIndex);
                            UIElement element = m_draggedItem.Content is ContentPresenter ? (m_draggedItem.Content as ContentPresenter).Content as UIElement
                                    : m_draggedItem.Content as UIElement;
                            m_Target.TabPositionCache.Insert(m_insertionIndex, element);
                            m_Target.UpdateTabPositionCache(m_Target.TabPositionCache);
                        }
                        e.Handled = true;
                    }
                }
                else if (m_draggedItem != null && m_Target != null && isdynamictabgrpenabled)
                {
                    if (!m_Target.Container.IsInDockingManager || (m_Target.Container.IsInDockingManager && m_Target.Container.DockingManager.IsContextMenuVisible))
                    {
                        TDILayoutPanel panel = (TDILayoutPanel)m_Target.Container.ILayoutPanel;
                        panel.UpdateTabControls(m_draggedItem, m_Source, m_Target);
                        e.Handled = true;
                    }
                }

                if (m_draggedItem != null && m_Source != m_Target)
                {
                    m_Target.UpdateMenuItems(m_draggedItem);
                }
            }
        }
        /// <summary>
        /// Detects same index and targets
        /// </summary>
        /// <param name="source">represents source</param>
        /// <param name="target">represents target</param>
        /// <param name="dragged">represents dragged element</param>
        /// <returns></returns>
        private bool DetectSameIndexTarget(ItemsControl source, ItemsControl target, TabItemExt dragged)
        {
            int index = source.Items.IndexOf(dragged);
            int insertion = m_insertionIndex;
            if (index != -1 && source == target && index < insertion)
            {
                insertion--;
            }
            if (source == target && index == insertion)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Handles the PreviewDragLeave event of the DropTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            if (null != m_draggedItem)
            {
                RemoveInsertionAdorner();
                e.Handled = true;
            }
        }
        
        /// <summary>
        /// Decides the drop target.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void DecideDropTarget(DragEventArgs e)
        {
            int targetDocumentTabControlCount = m_Target.Items.Count;

            if (IsDropDataTypeAllowed(m_draggedItem))
            {
                if (targetDocumentTabControlCount > 0)
                {
                    m_hasVerticalOrientation = m_Target.RotateTextWhenVertical;
                    m_overItem = Utilities.GetItemContainer(m_Target, e.OriginalSource as Visual);

                    if (m_overItem != null && m_Target.Items.Contains(m_overItem))
                    {
                        Point positionRelativeToItemContainer = e.GetPosition(m_overItem);
                        m_isInFirstHalf = Utilities.IsInFirstHalf(m_overItem, m_Target.TabStripPlacement, positionRelativeToItemContainer, m_hasVerticalOrientation);
                        m_insertionIndex = m_Target.ItemContainerGenerator.IndexFromContainer(m_overItem);
                        if (m_insertionIndex != -1)
                        {
                            if (m_Target.FlowDirection == FlowDirection.LeftToRight)
                            {
                                if (!m_isInFirstHalf)
                                {
                                    m_insertionIndex++;
                                }
                            }
                            if (m_Target.FlowDirection == FlowDirection.RightToLeft)
                            {
                                if (m_Target.TabStripPlacement == Dock.Left)
                                {
                                    if (!m_isInFirstHalf)
                                    {
                                        m_insertionIndex++;
                                    }
                                }
                                else if (m_isInFirstHalf)
                                {
                                    m_insertionIndex++;
                                }
                            }
                        }
                    }
                    else
                    {
                        m_overItem = m_Target.ItemContainerGenerator.ContainerFromIndex(targetDocumentTabControlCount - 1) as TabItemExt;
                        m_isInFirstHalf = false;
                        m_insertionIndex = targetDocumentTabControlCount;
                    }
                }
                else
                {
                    m_overItem = null;
                    m_insertionIndex = 0;
                }
            }
            else
            {
                m_overItem = null;
                m_insertionIndex = -1;
                ////e.Effects = DragDropEffects.None;
            }
        }
        
        /// <summary>
        /// Determines whether [is drop data type allowed] [the specified dragged item].
        /// </summary>
        /// <param name="draggedItem">The dragged item.</param>
        /// <returns>
        /// <c>true</c> if [is drop data type allowed] [the specified dragged item]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsDropDataTypeAllowed(object draggedItem)
        {
            bool isDropDataTypeAllowed;
            IEnumerable collectionSource = m_Target.ItemsSource;

            if (draggedItem != null)
            {
                if (collectionSource != null)
                {
                    Type draggedType = draggedItem.GetType();
                    Type collectionType = collectionSource.GetType();
                    Type genericIListType = collectionType.GetInterface("IList`1");

                    if (genericIListType != null)
                    {
                        Type[] genericArguments = genericIListType.GetGenericArguments();
                        isDropDataTypeAllowed = genericArguments[0].IsAssignableFrom(draggedType);
                    }
                    else
                    {
                        isDropDataTypeAllowed = typeof(IList).IsAssignableFrom(collectionType);
                    }
                }
                else
                {
                    isDropDataTypeAllowed = true;
                }
            }
            else
            {
                isDropDataTypeAllowed = false;
            }

            return isDropDataTypeAllowed;
        }

        /// <summary>
        /// Handles the DragEnter event of the TopWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void TopWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (m_topWindow != null)
            {
                ShowDraggedAdorner(e.GetPosition(m_topWindow));
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the DragOver event of the TopWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void TopWindow_DragOver(object sender, DragEventArgs e)
        {
            if (m_topWindow != null)
            {
                ShowDraggedAdorner(e.GetPosition(m_topWindow));
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the DragLeave event of the TopWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void TopWindow_DragLeave(object sender, DragEventArgs e)
        {
            RemoveDraggedAdorner();
            e.Handled = true;
        }

        /// <summary>
        /// Shows the dragged adorner.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        private void ShowDraggedAdorner(Point currentPosition)
        {
            if (null != m_Target && GetShowDragAdorner(m_Target))
            {
                if (m_draggedAdorner == null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(m_Source);
                    if(m_draggedItem!=null)
                    m_draggedAdorner = new DraggedAdorner(m_draggedItem.Content, GetDragDropTemplate(m_Source), m_draggedItem, adornerLayer);
                }

                if (m_draggedAdorner != null)
                {
                    //Point ptscrollingpanel = scrollingpanel != null && PresentationSource.FromVisual(scrollingpanel) != null ? scrollingpanel.PointToScreen(new Point())
                      //  : new Point(currentPosition.X + 1, currentPosition.Y + 1);
                    //if (currentPosition.X + m_draggedAdorner.m_contentPresenter.ActualWidth >= ptscrollingpanel.X)
                    //{
                    //    //m_draggedAdorner.SetPosition(-m_draggedAdorner.m_contentPresenter.ActualWidth, currentPosition.Y - m_initialMousePosition.Y);
                    //    return;
                    //}
                    //else
                        m_draggedAdorner.SetPosition(currentPosition.X - m_initialMousePosition.X, currentPosition.Y - m_initialMousePosition.Y);
                }
            }
        }

        /// <summary>
        /// Removes the dragged adorner.
        /// </summary>
        private void RemoveDraggedAdorner()
        {
            if (m_draggedAdorner != null)
            {
                m_draggedAdorner.Detach();
                m_draggedAdorner = null;
            }
        }

        internal void RemoveAllAdornerLayers()
        {
            if (m_Source != null)
            {
                TDILayoutPanel tdiLayoutPanel = VisualUtils.FindAncestor(m_Source, typeof(TDILayoutPanel)) as TDILayoutPanel;

                if (tdiLayoutPanel != null)
                {
                    foreach (DocumentTabControl docTabControl in tdiLayoutPanel.m_TabList)
                    {
                        if (docTabControl != null)
                        {
                            AdornerLayer layerSource = AdornerLayer.GetAdornerLayer(docTabControl);
                            Adorner[] adorners = layerSource.GetAdorners(docTabControl);
                            if (adorners != null)
                            {
                                foreach (Adorner ador in adorners)
                                {
                                    layerSource.Remove(ador);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Creates the insertion adorner.
        /// </summary>
        private void CreateInsertionAdorner()
        {
            if (m_overItem != null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(m_overItem);
                m_inserAdorner = new InsertionAdorner(m_hasVerticalOrientation, m_isInFirstHalf, m_Target.TabStripPlacement, m_overItem, adornerLayer);
            }
        }

        /// <summary>
        /// Updates the insertion adorner position.
        /// </summary>
        private void UpdateInsertionAdornerPosition()
        {
            if (m_inserAdorner != null)
            {
                m_inserAdorner.IsInFirstHalf = m_isInFirstHalf;
                m_inserAdorner.InvalidateVisual();
            }
        }

        /// <summary>
        /// Removes the insertion adorner.
        /// </summary>
        private void RemoveInsertionAdorner()
        {
            if (m_inserAdorner != null)
            {
                m_inserAdorner.Detach();
                m_inserAdorner = null;
            }
        }

        /// <summary>
        /// Determines whether [is drag source changed] [the specified obj].
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsDragSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var dragSource = obj as DocumentTabControl;

            if (dragSource != null)
            {
                DragDropHelper instance = GetInstance();

                if (Object.Equals(e.NewValue, true))
                {
                    dragSource.PreviewMouseLeftButtonDown += instance.DragSource_PreviewMouseLeftButtonDown;
                    dragSource.PreviewMouseLeftButtonUp += instance.DragSource_PreviewMouseLeftButtonUp;
                    dragSource.PreviewMouseMove += instance.DragSource_PreviewMouseMove;
#if !SyncfusionFramework3_5
                    dragSource.PreviewTouchMove += instance.dragSource_PreviewTouchMove;
                    dragSource.PreviewTouchDown += instance.dragSource_PreviewTouchDown;
                    dragSource.PreviewTouchUp += instance.dragSource_PreviewTouchUp;
#endif
                }
                else
                {
                    dragSource.PreviewMouseLeftButtonDown -= instance.DragSource_PreviewMouseLeftButtonDown;
                    dragSource.PreviewMouseLeftButtonUp -= instance.DragSource_PreviewMouseLeftButtonUp;
                    dragSource.PreviewMouseMove -= instance.DragSource_PreviewMouseMove;
#if !SyncfusionFramework3_5
                    dragSource.PreviewTouchMove -= instance.dragSource_PreviewTouchMove;
                    dragSource.PreviewTouchDown -= instance.dragSource_PreviewTouchDown;
                    dragSource.PreviewTouchUp -= instance.dragSource_PreviewTouchUp;
#endif
                }
            }
        }

        /// <summary>
        /// Determines whether [is drop target changed] [the specified obj].
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsDropTargetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var dropTarget = obj as DocumentTabControl;

            if (dropTarget != null)
            {
                DragDropHelper instance = GetInstance();

                if (Object.Equals(e.NewValue, true))
                {
                    dropTarget.AllowDrop = true;
                    dropTarget.PreviewDrop += instance.DropTarget_PreviewDrop;
                    dropTarget.PreviewDragEnter += instance.DropTarget_PreviewDragEnter;
                    dropTarget.PreviewDragOver += instance.DropTarget_PreviewDragOver;
                    dropTarget.PreviewDragLeave += instance.DropTarget_PreviewDragLeave;
                    dropTarget.PreviewMouseMove += new MouseEventHandler(dropTarget_PreviewMouseMove);
#if !SyncfusionFramework3_5
                    dropTarget.PreviewTouchMove += dropTarget_PreviewTouchMove;
#endif
                }
                else
                {
                    dropTarget.AllowDrop = false;
                    dropTarget.PreviewDrop -= instance.DropTarget_PreviewDrop;
                    dropTarget.PreviewDragEnter -= instance.DropTarget_PreviewDragEnter;
                    dropTarget.PreviewDragOver -= instance.DropTarget_PreviewDragOver;
                    dropTarget.PreviewDragLeave -= instance.DropTarget_PreviewDragLeave;
                    dropTarget.PreviewMouseMove -= new MouseEventHandler(dropTarget_PreviewMouseMove);
#if !SyncfusionFramework3_5
                    dropTarget.PreviewTouchMove -= dropTarget_PreviewTouchMove;
#endif
                }
            }
        }

#if !SyncfusionFramework3_5
        static void dropTarget_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (m_Source != null && m_Source.Container != null && m_Source.Container.IsTouchEnabled)
            {
                TDILayoutPanel tdiLayoutPanel = VisualUtils.FindAncestor(m_Source, typeof(TDILayoutPanel)) as TDILayoutPanel;
                if (tdiLayoutPanel != null)
                {
                    foreach (DocumentTabControl docTabControl in tdiLayoutPanel.m_TabList)
                    {
                        if (docTabControl != null && m_Target != docTabControl)
                        {
                            AdornerLayer layerSource = AdornerLayer.GetAdornerLayer(docTabControl);
                            if (layerSource != null)
                            {
                                Adorner[] adorners = layerSource.GetAdorners(docTabControl);
                                if (adorners != null)
                                {
                                    foreach (Adorner ador in adorners)
                                    {
                                        layerSource.Remove(ador);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (m_Target != null)
            {
                Point targetPoint = e.GetTouchPoint(m_Target).Position;
                if (targetPoint.Y <= 0 || targetPoint.X > m_Target.ActualWidth)
                {
                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(m_Target);
                    if (m_Target != null && layer != null)
                    {
                        Adorner[] adorners = layer.GetAdorners(m_Target);
                        if (adorners != null)
                        {
                            foreach (Adorner ador in adorners)
                            {
                                layer.Remove(ador);
                            }
                        }
                    }
                }
            }
        }
#endif

        static void dropTarget_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (m_Source != null)
                {
                    TDILayoutPanel tdiLayoutPanel = VisualUtils.FindAncestor(m_Source, typeof(TDILayoutPanel)) as TDILayoutPanel;
                    if (tdiLayoutPanel != null)
                    {
                        foreach (DocumentTabControl docTabControl in tdiLayoutPanel.m_TabList)
                        {
                            if (docTabControl != null && m_Target != docTabControl)
                            {
                                AdornerLayer layerSource = AdornerLayer.GetAdornerLayer(docTabControl);
                                if (layerSource != null)
                                {
                                    Adorner[] adorners = layerSource.GetAdorners(docTabControl);
                                    if (adorners != null)
                                    {
                                        foreach (Adorner ador in adorners)
                                        {
                                            layerSource.Remove(ador);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (m_Target != null)
                {
                    Point targetPoint = e.GetPosition(m_Target);
                    if (targetPoint.Y <= 0 || targetPoint.X > m_Target.ActualWidth)
                    {
                        AdornerLayer layer = AdornerLayer.GetAdornerLayer(m_Target);
                        if (m_Target != null && layer != null)
                        {
                            Adorner[] adorners = layer.GetAdorners(m_Target);
                            if (adorners != null)
                            {
                                foreach (Adorner ador in adorners)
                                {
                                    layer.Remove(ador);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Represents the property for the bool IsDragSourceProperty DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDropHelper), new UIPropertyMetadata(false, IsDragSourceChanged));
        
        /// <summary>
        /// Represents the property for the bool IsDropTargetProperty DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsDropTargetProperty = DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDropHelper), new UIPropertyMetadata(false, IsDropTargetChanged));
        
        /// <summary>
        /// Represents the property for the bool DragDropTemplateProperty DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DragDropTemplateProperty = DependencyProperty.RegisterAttached("DragDropTemplate", typeof(DataTemplate), typeof(DragDropHelper), new UIPropertyMetadata(null));
        #endregion
        /// <summary>
        /// Gets the value of the ShowDragAdorner dependency property.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>bool ShowDragAdornerProperty</returns>
        public static bool GetShowDragAdorner(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowDragAdornerProperty);
        }

        /// <summary>
        /// Sets the value of the ShowDragAdorner dependency property.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowDragAdorner(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowDragAdornerProperty, value);
        }

        /// <summary>
        /// Represents the ShowDragAdorner Dependency property
        /// </summary>
        public static readonly DependencyProperty ShowDragAdornerProperty = DependencyProperty.RegisterAttached("ShowDragAdorner", typeof(bool), typeof(DragDropHelper), new FrameworkPropertyMetadata(true));

        
    }

}