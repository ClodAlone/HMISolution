// <copyright file="MDILayoutPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents layout panel for MDIWindow.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    
    public class MDILayoutPanel : FrameworkElement, ILayoutPanel
    {
        #region Constants
        /// <summary>
        /// Presents DEFAULT_OFFSET
        /// </summary>
        private const int DEFAULT_OFFSET = 20;

        /// <summary>
        /// Presents MIN_STEP
        /// </summary>
        private const double MIN_STEP = 10;

        /// <summary>
        /// Presents MIN_HEIGHT
        /// </summary>
        private const double MIN_HEIGHT = 18;

        /// <summary>
        /// Presents MIN_WIDTH
        /// </summary>
        private const double MIN_WIDTH = 24;

        /// <summary>
        /// Represents minimum width of MDIWindow.
        /// </summary>
        private const double MIN_WINDOW_WIDTH = 100;

        /// <summary>
        /// Represents minimum height of MDIWindow.
        /// </summary>
        private const double MIN_WINDOW_HEIGHT = 50;
        #endregion

        #region Private members
        /// <summary>
        /// Presents wrappers
        /// </summary>
        private readonly IList<MDIWindow> m_wrappers = new List<MDIWindow>();

        /// <summary>
        /// Presents WrappersTable
        /// </summary>
        private readonly Dictionary<UIElement, MDIWindow> m_WrappersTable = new Dictionary<UIElement, MDIWindow>();

        /// <summary>
        /// Presents windowMoveOrResize
        /// </summary>
        private MDIWindow m_windowMoveOrResize;

        /// <summary>
        /// Presents boundsInitial
        /// </summary>
        private Rect m_boundsInitial;

        /// <summary>
        /// Presents currentAdorner
        /// </summary>
        private Adorner m_currentAdorner;

        /// <summary>
        /// Presents StartPoint
        /// </summary>
        private Point m_startPoint = new Point(DEFAULT_OFFSET, DEFAULT_OFFSET);

        /// <summary>
        /// Presents AfterResize
        /// </summary>
        private bool m_afterResize;

        /// <summary>
        /// Presents DragWindowInfo
        /// </summary>
        private MDIWindowDragInfo m_dragWindowInfo = new MDIWindowDragInfo(PercentRect.Empty, new PercentPoint(0, 0), MDIBorder.Outside, null);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the Container dependency property.
        /// </summary>
        public DocumentContainer Container
        {
            get
            {
                return (DocumentContainer)GetValue(ContainerProperty);
            }

            protected internal set
            {
                SetValue(ContainerPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the drag window info.
        /// </summary>
        /// <value>The drag window info.</value>
        internal MDIWindowDragInfo DragWindowInfo
        {
            get
            {
                return m_dragWindowInfo;
            }

            set
            {
                m_dragWindowInfo = value;
            }
        }

        /// <summary>
        /// Gets document wrappers list.
        /// </summary>
        internal IList<MDIWindow> Wrappers
        {
            get
            {
                return m_wrappers;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dragging.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dragging; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDragging
        {
            get;
            set;
        }

        /// <summary>
        /// Gets count of the visual children.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return m_wrappers.Count;
            }
        }

        /// <summary>
        /// Gets enumerator for the logical children.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                return Wrappers.GetEnumerator();
            }
        }

        /// <summary>
        /// Gets or sets the value of the KeyboardOverrideMode dependency property.
        /// </summary>
        private KeyboardOverrideMode KeyboardOverrideMode
        {
            get
            {
                return (KeyboardOverrideMode)GetValue(KeyboardOverrideModeProperty);
            }

            set
            {
                SetValue(KeyboardOverrideModePropertyKey, value);
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="MDILayoutPanel"/> class.
        /// </summary>
        static MDILayoutPanel()
        {
            EnvironmentTest.ValidateLicense(typeof(MDILayoutPanel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MDILayoutPanel"/> class.
        /// </summary>
        public MDILayoutPanel()
        {
            AddHandler(DocumentContainer.MDIBoundsChangedEvent, new RoutedPropertyChangedEventHandler<Rect>(ProcessBoundsChange));
            AddHandler(DocumentContainer.MDIMinimizedBoundsChangedEvent, new RoutedPropertyChangedEventHandler<Rect>(ProcessBoundsChange));

            Loaded += new RoutedEventHandler(OnMDILayoutPanelLoaded);
            Unloaded += new RoutedEventHandler(OnMDILayoutPanelUnloaded);
            SubscribeToCommands();
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Container property is changed.
        /// </summary>
        public event PropertyChangedCallback ContainerChanged;
        #endregion

        #region Public methods


        private void SetMDILayoutBasedOnMDIMode()
        {
            if(DocumentContainer.IsMDILayoutset)
            {
               SetLayout(DocumentContainer.templayout);
            }
        }
        /// <summary>
        /// Refreshes the visibility of the items.
        /// </summary>
        public void RefreshChildrenVisibility()
        {
            if (Container == null)
            {
                throw new InvalidOperationException("Container is not available.");
            }

            ArrayList allItems = new ArrayList(Container.Items);
            ArrayList itemsToAdd = new ArrayList(Container.Items.Count);

            foreach (MDIWindow wrapper in m_wrappers)
            {
                UIElement elementInWrapper = wrapper.Content;

                if (ShouldDocumentBeVisible(elementInWrapper))
                {
                    allItems.Remove(elementInWrapper);
                }
            }

            foreach (UIElement element in allItems)
            {
                if (ShouldDocumentBeVisible(element))
                {
                    allItems.Remove(element);
                    itemsToAdd.Add(element);
                }
            }

            RemoveVisualChildren(allItems);
            AddVisualChildren(itemsToAdd);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Container != null)
                Container.Items.CollectionChanged -= Items_CollectionChanged;
            ClearValue(ContainerPropertyKey);
        }

        /// <summary>
        /// Finds the correct start point.
        /// </summary>
        public void FindCorrectStartPoint()
        {
            IList<Point> list = new List<Point>();

            foreach (MDIWindow wrapper in m_WrappersTable.Values)
            {
                Rect bounds = wrapper.GetMDIBounds();

                if ((bounds.X % DEFAULT_OFFSET) == 0 &&
                    (bounds.Y % DEFAULT_OFFSET) == 0)
                {
                    list.Add(bounds.Location);
                }
            }

            Point maxPoint = new Point(0, 0);

            if (list.Count > 0)
            {
                maxPoint = list[0];

                for (int i = 0; i < list.Count; i++)
                {
                    if (maxPoint.X < list[i].X && maxPoint.Y < list[i].Y)
                    {
                        maxPoint = list[i];
                    }
                }
            }

            m_startPoint = new Point(maxPoint.X + DEFAULT_OFFSET, maxPoint.Y + DEFAULT_OFFSET);
        }

        /// <summary>
        /// Sets the layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        public void SetLayout(MDILayout layout)
        {
            int cnt = Wrappers.Count;
            Size size = Container.RenderSize;

            if (MDILayout.Cascade == layout)
            {
                int offsetSize = DEFAULT_OFFSET * cnt;
                double height = Math.Max(MIN_HEIGHT, size.Height - offsetSize);
                double width = Math.Max(MIN_WIDTH, size.Width - offsetSize);
                Size itemSize = new Size(width, height);
                SetLayout(cnt, itemSize, DEFAULT_OFFSET, DEFAULT_OFFSET);
            }
            else if (MDILayout.Horizontal == layout)
            {
                double height = Math.Max(MIN_HEIGHT, size.Height / cnt - 2);
                double width = Math.Max(MIN_WIDTH, size.Width - 5);
                Size itemSize = new Size(width, height);
                SetLayout(cnt, itemSize, 0, height);
            }
            else if (MDILayout.Vertical == layout)
            {
                double height = Math.Max(MIN_HEIGHT, size.Height - 5);
                double width = Math.Max(MIN_WIDTH, size.Width / cnt - 2);
                Size itemSize = new Size(width, height);
                SetLayout(cnt, itemSize, width, 0);
            }
        }
        #endregion

        #region ILayoutPanel methods
        /// <summary>
        /// Creates the child document params.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <returns>ChildDocument Params</returns>
        public ChildDocumentParams CreateChildDocumentParams(FrameworkElement element, bool isActive)
        {
            return new ChildDocumentParams(element, PropertiesMode.Child, isActive);
        }

        /// <summary>
        /// Resets the visible list.
        /// </summary>
        public void ResetVisibleList()
        {
            RemoveVisualChildren(new ArrayList(m_WrappersTable.Keys));

            if (Container != null)
            {
                AddVisualChildren(FilterVisualChildrenByState(Container.Items));
            }
        }

        /// <summary>
        /// Gets the ordered items.
        /// </summary>
        /// <returns>Ilist Control Items</returns>
        public IList<Control> GetOrderedItems()
        {
            int cnt = m_wrappers.Count;
            IList<Control> result = new List<Control>(cnt);
            bool isVistaFlip = SwitchMode.VistaFlip == Container.SwitchMode;

            for (int i = 0; i < cnt; ++i)
            {
                MDIWindow window = m_wrappers[i];

                if (isVistaFlip)
                {
                    window.Arrange(DocumentContainer.GetMDIBounds(window.Content));
                }

                result.Add(window);
            }

            return result;
        }

        /// <summary>
        /// Sets the active item.
        /// </summary>
        /// <param name="activeItem">The active item.</param>
        public void SetActiveItem(FrameworkElement activeItem)
        {
            MDIWindow window = PrepareActiveDocument(activeItem);

            if (null != window && !window.IsKeyboardFocusWithin)
            {
                SetActive(window, m_wrappers);
                window.Focus();
            }
        }

        /// <summary>
        /// Determines whether this instance can switch.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can switch; otherwise, <c>false</c>.
        /// </returns>
        public bool CanSwitch()
        {
            return 1 < m_wrappers.Count;
        }

        /// <summary>
        /// Forwards the switch immediate.
        /// </summary>
        /// <param name="firstTabulation">if set to <c>true</c> [first tabulation].</param>
        /// <param name="isKeepCircle">if set to <c>true</c> [is keep circle].</param>
        public void ForwardSwitchImmediate(bool firstTabulation, bool isKeepCircle)
        {
            if (firstTabulation && !isKeepCircle)
            {
                SwitchWindow();
            }
            else
            {
                TryOrderMotion(true);
            }

            if (m_wrappers.Count > 0)
            {
                m_wrappers[0].Focus();
            }
        }

        /// <summary>
        /// Back forward the switch immediate.
        /// </summary>
        public void BackforwardSwitchImmediate()
        {
            TryOrderMotion(false);

            if (m_wrappers.Count > 0)
            {
                m_wrappers[0].Focus();
            }
        }

        /// <summary>
        /// Sets the focus.
        /// </summary>
        public void SetFocus()
        {
            Container.Focus();
            UIElement activeDocument = Container.ActiveDocument;

            if (null != activeDocument)
            {
                MDIWindow window = VisualUtils.FindAncestor(activeDocument, typeof(MDIWindow)) as MDIWindow;

                if (window != null)
                {
                    window.Focus();
                }
                else
                {
                    PrepareActiveDocument(activeDocument);
                }
            }
        }

        /// <summary>
        /// Sets the active document.
        /// </summary>
        /// <param name="element">The element.</param>
        public void SetActiveDocument(UIElement element)
        {
            PrepareActiveDocument(element);
        }

        /// <summary>
        /// Sets the active window.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        public void SetActiveWindow(Control wrapper)
        {
            MDIWindow window = (MDIWindow)wrapper;

            if (0 < m_wrappers.Count - 1)
            {
                if (m_wrappers.Contains(window))
                {
                    if (window != m_wrappers[0])
                    {
                        if (Container.IsKeepCircle)
                        {
                            SetCircleActiveWindow(window);
                        }
                        else
                        {
                            SetRandomActiveWindow(window);
                        }
                    }
                }
                else
                {
                   // throw new ArgumentException("Argument isn't contained in elements list");
                }
            }
        }

        /// <summary>
        /// Sets the focus after persist load.
        /// </summary>
        public void UpdateAfterPersistLoad()
        {
            Container = Container ?? (TemplatedParent as DocumentContainer);
            UIElement active = Container.ActiveDocument;

            foreach (MDIWindow wind in m_wrappers)
            {
                if (active == wind.Content)
                {
                    wind.Focus();
                    break;
                }
            }

            Container.LoadingPersistState = false;
        }

        /// <summary>
        /// Gets the Content.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        /// <returns>UIElement content</returns>
        public UIElement GetContent(Control wrapper)
        {
            MDIWindow window = (MDIWindow)wrapper;
            if (window.DocumentHeader.Header != null)
            {
                DocumentContainer.SetHeader(window.Content, window.DocumentHeader.Header);
            }
            //else
            //{
            //    //DocumentContainer.SetHeader(window.Content, );
            //}
            if (window.DocumentHeader.HeaderTemplate != null)
            {
                DocumentContainer.SetHeaderTemplate(window.Content, window.DocumentHeader.HeaderTemplate);
            }
            return window.Content;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the correct rectangle in which window will be rendered.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>Rect value of window</returns>
        internal Rect GetCorrectRectangle(MDIWindow window)
        {
            ScrollViewer viewer = (ScrollViewer)Parent;
            double scrollWidth = Container.ActualHeight - viewer.ViewportHeight;

            double height = (viewer.ComputedVerticalScrollBarVisibility == Visibility.Visible) ?
                viewer.ExtentHeight + scrollWidth : Container.ActualHeight;

            height -= (viewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible) ?
            GetHorizontalScrollBar(viewer).ActualHeight : 0;

            DocumentContainer owner = window.Container;
            double docheight = DocumentContainer.MINIMIZED_HEIGHT;
            Size minimizedSize;
            if (!Double.IsNaN(window.DocumentHeader.Height))
            {
                docheight = window.DocumentHeader.Height;
            }
            double RenderingHeight = docheight + 5;
            const double RenderingWidth = DocumentContainer.MINIMIZED_WIDTH + 5;
            minimizedSize = new Size(DocumentContainer.MINIMIZED_WIDTH, docheight);
            Point currentPoint = new Point(0, height - RenderingHeight);

            Rect bounds = new Rect(currentPoint, minimizedSize);
            DocumentContainer.SetMDIMinimizedBounds(window.Content, bounds);

            while (Intersect(window))
            {
                bounds = new Rect(currentPoint, minimizedSize);
                DocumentContainer.SetMDIMinimizedBounds(window.Content, bounds);

                currentPoint = GetNewPoint(RenderingHeight, RenderingWidth, currentPoint);
            }

            return bounds;
        }

        /// <summary>
        /// Removes the visual children.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <param name="detachHandlers">if set to <c>true</c> [detach handlers].</param>
        internal void RemoveVisualChildren(IEnumerable children, bool detachHandlers)
        {
            if (null == children)
            {
                throw new ArgumentNullException("children");
            }

            if (detachHandlers)
            {
                DetachHandlers(children);
            }

            foreach (UIElement item in children)
            {
                if (m_WrappersTable.ContainsKey(item) && m_wrappers.Count>0)
                {
                    MDIWindow wrapper = m_WrappersTable[item];
                    m_wrappers.RemoveAt(m_wrappers.IndexOf(wrapper));
                    wrapper.Content = null;
                    m_WrappersTable.Remove(item);

                    RemoveVisualChild(wrapper);
                    RemoveLogicalChild(wrapper);
                }
            }

            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
        }

        /// <summary>
        /// Switches the window.
        /// </summary>
        internal void SwitchWindow()
        {
            if (0 < m_wrappers.Count - 1)
            {
                SwitchWindow(1, 1);
            }
        }

        /// <summary>
        /// Tries the order motion.
        /// </summary>
        /// <param name="forward">if set to <c>true</c> [forward].</param>
        internal void TryOrderMotion(bool forward)
        {
            int iWrappsCount = m_wrappers.Count - 1;

            if (0 < iWrappsCount)
            {
                int outOfNext;
                int inOfNext;

                if (forward)
                {
                    outOfNext = 1;
                    inOfNext = iWrappsCount;
                }
                else
                {
                    outOfNext = iWrappsCount;
                    inOfNext = 1;
                }

                SwitchWindow(outOfNext, inOfNext);
            }
        }

        /// <summary>
        /// Sets the active.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="wrappers">The wrappers.</param>
        internal static void SetActive(MDIWindow window, IList<MDIWindow> wrappers)
        {
            foreach (MDIWindow wind in wrappers)
            {
                wind.IsActive = false;
            }

            window.IsActive = true;
            window.Container.ActiveDocument = window.Content;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Container = TemplatedParent as DocumentContainer;
            if(Container!=null)
            Container.Items.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);
            ScrollViewer viewer = (ScrollViewer)Parent;
            viewer.Loaded += ViewerLoaded;
            viewer.ScrollChanged += ViewerScrollChanged;
            SizeChanged += PanelSizeChanged;
            Container.IsInMDIMaximizedStateChanged += new PropertyChangedCallback(OnContainerIsInMDIMaximizedStateChanged);
        }

        void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            DocumentContainer container = Container;
            if (container != null)
            {

                foreach (FrameworkElement element in Container.Items)
                {
                    if ((DocumentContainer.GetMDIWindowState(element)) == MDIWindowState.Maximized && !container.IsInMDIMaximizedState)
                        container.IsInMDIMaximizedState = true;
                }
            }       
           
        }

       

        /// <summary>
        /// Refines the wrapper.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        private bool RefineWrapper(MDIWindow window)
        {
                try
                {
                    RemoveVisualChild(window);
                    AddVisualChild(window);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            
        }

        /// <summary>
        /// Refines the scroll.
        /// </summary>
        /// <param name="window">The window.</param>
        private void RefineScroll(MDIWindow window)
        {
            if (Scrollflag)
            {
                this.Height = window.ContentPresenter.DesiredSize.Height;// +window.DesiredSize.Height - window.Content.DesiredSize.Height;
                //Scrollflag = false;
            }
            
        }

        /// <summary>
        /// Detects the is maximized.
        /// </summary>
        private void DetectIsMaximized()
        {
            for (int i = 0; i < Wrappers.Count; i++)
            {
                if (Wrappers[i].IsMaximized)
                {
                    maxindex = i;
                    return;
                }
            }
            maxindex = -1;
        }

        /// <summary>
        /// Represents the max index
        /// </summary>
        int maxindex=-1;

        /// <summary>
        /// represents the scroll flag
        /// </summary>
        bool Scrollflag = true;

        /// <summary>
        /// represents the horizontal offset
        /// </summary>
        double horizontaloffset = 0.0;

        /// <summary>
        /// represents the vertical offset
        /// </summary>
        double verticaloffset = 0.0;

       

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(0, 0);
            ScrollViewer sv = null;
            double minleft = 0.0, mintop = 0.0, minwidth = 0.0, minheight = 0.0, minX = double.PositiveInfinity;
            Rect rectLastUnknown = new Rect(m_startPoint.X, m_startPoint.Y, Math.Max(10, availableSize.Width - DEFAULT_OFFSET * 2), Math.Max(10, availableSize.Height - DEFAULT_OFFSET * 2));
            DocumentContainer container = Container;
            bool bMaximized = container != null && container.IsInMDIMaximizedState;

            rectLastUnknown = ValidateRectLastUnknown(rectLastUnknown);
            DetectIsMaximized();
            for (int i = Wrappers.Count - 1; i > -1; --i)
            {
                if (maxindex != -1)
                {
                    i = maxindex;
                }
                //if (RefineWrapper(Wrappers[i]))
                //{
                    MDIWindow window = m_wrappers[i];

                    //if (window.IsMaximized && (window.ContentPresenter.DesiredSize.Height > this.DesiredSize.Height))
                    //{
                    //    RefineScroll(window);
                    //}

                    Rect bounds = window.GetMDIBounds();

                    minleft = Math.Min(minleft, bounds.Location.X);
                    mintop = Math.Min(mintop, bounds.Location.Y);

                    horizontaloffset = minleft;
                    verticaloffset = mintop;

                    Rect defautRect = (Rect)DocumentContainer.MDIBoundsProperty.DefaultMetadata.DefaultValue;

                    bool boundsIsDefault = true;

                    try
                    {
                        boundsIsDefault = bounds == defautRect && !IsDragging && Container.AdjustStartPosition;
                    }
                    catch (Exception) { }

                    if (bounds.IsEmpty)
                    {
                        bounds = defautRect;
                    }

                    Size sizeElement = bounds.Size;
                    rectLastUnknown.X = m_startPoint.X;
                    rectLastUnknown.Y = m_startPoint.Y;

                    if (boundsIsDefault && this.DesiredSize.Height >= bounds.Location.Y && this.DesiredSize.Width >= bounds.Location.X)
                    {
                        bounds.Location = rectLastUnknown.Location;
                        sizeElement = availableSize;
                    }
                    else
                    {
                        sizeElement.Width = Math.Min(bounds.Width, availableSize.Width);
                        sizeElement.Height = Math.Min(bounds.Height, availableSize.Height);
                    }

                    if (bMaximized)
                    {
                        window.Measure(availableSize);
                    }
                    else
                    {
                        window.Measure(sizeElement);
                    }

                    if (bMaximized || window.DesiredSize.Height <= MIN_WINDOW_HEIGHT && window.DesiredSize.Width <= MIN_WINDOW_WIDTH)
                    {
                        sizeElement = bounds.Size;
                    }
                    else
                    {
                        sizeElement = window.DesiredSize;
                    }

                    if (boundsIsDefault)
                    {
                        bounds.Size = sizeElement;
                        UpdateStartPoint(availableSize, sizeElement);

                        DependencyProperty dProperty = window.IsMinimized
                            ? DocumentContainer.MDIMinimizedBoundsProperty
                            : DocumentContainer.MDIBoundsProperty;

                        window.Content.SetValue(dProperty, bounds);
                    }

                    size.Width = Math.Max(size.Width, bounds.X + sizeElement.Width);
                    size.Height = Math.Max(size.Height, bounds.Y + sizeElement.Height);
                    minX = Math.Min(minX, bounds.X);
                //}
                if (maxindex != -1)
                {
                    if (container != null && container.GetActiveWindow() != null)
                    {
                        if (container.GetActiveWindow() == Wrappers[i])
                        {
                            MDILayoutPanel.SetActive(Wrappers[i], Wrappers);
                            i = -1;
                        }
                        else if (Wrappers[i] !=null && container.GetActiveWindow() != Wrappers[i] && DocumentContainer.GetMDIWindowState(Wrappers[i]) == MDIWindowState.Maximized)
                        {
                                DocumentContainer.SetMDIWindowState(Wrappers[i], MDIWindowState.Normal);
                                DocumentContainer.SetMDIWindowState(container.GetActiveWindow(), MDIWindowState.Maximized);
                                i = -1;
                        }
                    }
                }
            }

            if (!IsDragging && (verticaloffset < 0.0 || horizontaloffset < 0.0))
            {
                minX = 0;
                for (int i = Wrappers.Count - 1; i > -1; --i)
                {
                    //if (RefineWrapper(Wrappers[i]))
                    //{
                        MDIWindow window = m_wrappers[i];

                        Rect bounds;
                        bounds = window.GetMDIBounds();

                        if (horizontaloffset < 0.0)
                        {
                            if (bounds.Location.X >= 0.0)
                            {
                                bounds.Location = new Point(bounds.Location.X + Math.Abs(horizontaloffset), bounds.Location.Y);
                            }
                            else
                            {
                                bounds.Location = new Point(0, bounds.Location.Y);
                            }

                            DependencyProperty dProperty = window.IsMinimized
                           ? DocumentContainer.MDIMinimizedBoundsProperty
                           : DocumentContainer.MDIBoundsProperty;
                            window.Content.SetValue(dProperty, bounds);
                        }

                        if (verticaloffset < 0.0)
                        {
                            if (bounds.Location.Y >= 0.0)
                            {
                                bounds.Location = new Point(bounds.Location.X, bounds.Location.Y + Math.Abs(verticaloffset));
                            }
                            else
                            {
                                bounds.Location = new Point(bounds.Location.X, 0);
                            }

                            DependencyProperty dProperty = window.IsMinimized
                          ? DocumentContainer.MDIMinimizedBoundsProperty
                          : DocumentContainer.MDIBoundsProperty;
                            window.Content.SetValue(dProperty, bounds);
                        }

                        size.Width = Math.Max(size.Width, bounds.X + bounds.Size.Width);
                        size.Height = Math.Max(size.Height, bounds.Y + bounds.Size.Height);
                    //}
                }

                if (this.Parent != null && (this.Parent is ScrollViewer))
                {
                    sv = this.Parent as ScrollViewer;
                }

                if (sv != null)
                {
                    if (minleft < 0.0)
                    {
                        minwidth = sv.ViewportWidth + Math.Abs(minleft);
                    }
                    if (mintop < 0.0)
                    {
                        minheight = sv.ViewportHeight + Math.Abs(mintop);
                    }
                }

                size.Width = Math.Max(size.Width, minwidth);
                size.Height = Math.Max(size.Height, minheight);

                if (sv != null)
                {
                    sv.ScrollToHorizontalOffset(Math.Abs(minleft));
                    sv.ScrollToVerticalOffset(Math.Abs(mintop));
                }

                UpdateStartPoint(availableSize, size);

                return GetValidSize(size);
            }

            size.Width = Math.Min(size.Width, availableSize.Width);
            size.Height = Math.Min(size.Height, availableSize.Height);

            size = ValidateSizeOnMaximize(size);

            return GetValidSize(size);


        }

        /// <summary>
        /// Validates the size on maximize.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        private Size ValidateSizeOnMaximize(Size size)
        {
            foreach (MDIWindow window in m_wrappers)
            {
                if (window.IsMaximized)
                {
                    size.Height = ActualHeight;
                    return size;
                }
            }
            return size;
        }

        /// <summary>
        /// Arranges elements.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            DocumentContainer container = Container;           
            bool isntMaximized = container == null || !container.IsInMDIMaximizedState;
            Rect bounds = new Rect(0, 0, finalSize.Width, finalSize.Height);
            DetectIsMaximized();
            for (int i = 0; i < Wrappers.Count;i++)
            {
                MDIWindow window = Wrappers[i];
                //if (maxindex != -1)
                //{
                //    window = Wrappers[maxindex];
                //}
                //if (RefineWrapper(window))
                //{
                    if (isntMaximized)
                    {
                        if (DocumentContainer.IsMinimized(window.Content))
                        {
                            bounds = DocumentContainer.GetMDIMinimizedBounds(window.Content);
                            if ((m_afterResize &&
                                !window.WasMinimizedDragged)
                                || window.IsPanelLayout)
                            {
                                bounds = GetCorrectRectangle(window);
                                window.IsPanelLayout = false;
                            }

                            if (!bounds.IsEmpty)
                            {
                                window.Width = bounds.Width;
                                window.Height = bounds.Height + 5;
                            }
                        }
                        else
                        {
                            window.Width = Width;
                            window.Height = Height;
                            bounds = DocumentContainer.GetMDIBounds(window.Content);
                        }
                    }
                    else if (container.IsInMDIMaximizedState)
                    {
                        window.Width = ActualWidth;
                        window.Height = ActualHeight;
                    }

                    if (!bounds.IsEmpty)
                        window.Arrange(bounds);
                //}
                //if (maxindex != -1)
                //{
                //    break;
                //}
            }

            m_afterResize = false;
            return finalSize;
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return m_wrappers[m_wrappers.Count - index - 1];
        }

        /// <summary>
        /// Invoked when the parent of this element in the visual tree is changed. Overrides <see cref="M:System.Windows.UIElement.OnVisualParentChanged(System.Windows.DependencyObject)"/>.
        /// </summary>
        /// <param name="oldParent">The old parent element. May be null to indicate that the element did not have a visual parent previously.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            DocumentContainer container = VisualUtils.FindAncestor(this, typeof(DocumentContainer)) as DocumentContainer;
            if (container != null)
            {
                Container = container;
            }
            base.OnVisualParentChanged(oldParent);
        }

        /// <summary>
        /// Updates property value cache and raises KeyboardOverrideModeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnKeyboardOverrideModeChanged(DependencyPropertyChangedEventArgs e)
        {
            switch (KeyboardOverrideMode)
            {
                case KeyboardOverrideMode.None:
                    ReleseInputOverride();
                    break;

                case KeyboardOverrideMode.WindowResize:
                case KeyboardOverrideMode.WindowMove:
                    OverrideInput();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Updates property value cache and raises ContainerChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnContainerChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ContainerChanged != null)
            {
                ContainerChanged(this, e);
            }

            DocumentContainer containerOld = (DocumentContainer)e.OldValue;

            if (containerOld != null)
            {
                DetachFromContainer(containerOld);
            }

            DocumentContainer container = (DocumentContainer)e.NewValue;

            if (container != null)
            {
                AttachToContainer(container);
            }
        }

        /// <summary>
        /// Attaches the handlers.
        /// </summary>
        /// <param name="child">The child.</param>
        private void AttachHandlers(IInputElement child)
        {
            child.AddHandler(DockingManager.DockStateChangedEvent, new RoutedEventHandler(StateChangeHandler));
        }

        /// <summary>
        /// Detaches the handlers.
        /// </summary>
        /// <param name="child">The child.</param>
        private void DetachHandlers(IInputElement child)
        {
            child.RemoveHandler(DockingManager.DockStateChangedEvent, new RoutedEventHandler(StateChangeHandler));
        }

        /// <summary>
        /// Checks whether the window is not in minimized state.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>bool value type</returns>
        private bool ShouldDocumentBeVisible(DependencyObject document)
        {
            DockState state = DockingManager.GetState(document);
            return state !=
                DockState.Hidden && (!Container.IsDocumentStateRequired || state == DockState.Document);
        }

        /// <summary>
        /// Creates internal document wrapper and sets specified document as it's content.
        /// </summary>
        /// <param name="document">Document, the wrapper is created for.</param>
        /// <returns>MDI window document</returns>
        private MDIWindow CreateDocumentWrapper(UIElement document)
        {
            DocumentContainer container=null;
            if (TemplatedParent != null)
            {
                container = TemplatedParent as DocumentContainer;
            }
            else
            {
                FrameworkElement MDI = document as FrameworkElement;
                container = MDI.Parent as DocumentContainer;
            }
            MDIWindow wrapper = new MDIWindow(container)
            {
                Content = document,
            };

            return wrapper;
        }

        /// <summary>
        /// Panels the size changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void PanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_afterResize = true;
            InvalidateVisual();

            if (Container != null && Container.Mode == DocumentContainerMode.MDI &&
                Container.ActiveDocument != null)
            {
                MDIWindow window = FindWindowByEventSource(Container.ActiveDocument);

                if (window != null && Container.IsInMDIMaximizedState)
                {
                    window.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Viewers the loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ViewerLoaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer viewer = sender as ScrollViewer;
            ScrollBar horizontalScrollBar = GetHorizontalScrollBar(viewer);
            ScrollBar verticalscrollbar = GetVerticalScrollBar(viewer);
            if (horizontalScrollBar != null)
            {
                horizontalScrollBar.IsVisibleChanged += HorizontalScrollIsVisibleChanged;
            }
            if (verticalscrollbar != null)
            {
                verticalscrollbar.IsVisibleChanged += VerticallScrollIsVisibleChanged;
            }
        }

        /// <summary>
        /// Verticalls the scroll is visible changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void VerticallScrollIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_afterResize = true;
            //firsttime = true;
            InvalidateVisual();
           
        }

        /// <summary>
        /// Horizontals the scroll is visible changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void HorizontalScrollIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_afterResize = true;
            InvalidateVisual();
        }

        /// <summary>
        /// Gets the new point.
        /// </summary>
        /// <param name="renderingHeight">Height of the rendering.</param>
        /// <param name="renderingWidth">Width of the rendering.</param>
        /// <param name="currentPoint">The current point.</param>
        /// <returns>Point value  </returns>
        private Point GetNewPoint(double renderingHeight, double renderingWidth, Point currentPoint)
        {
            currentPoint.X += renderingWidth;

            if (currentPoint.X + renderingWidth > Container.ActualWidth)
            {
                currentPoint.X = 0;
                currentPoint.Y -= renderingHeight;
            }

            return currentPoint;
        }

        /// <summary>
        /// Indicates whether the specified window intersects with any windows.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>Boolean value type</returns>
        private bool Intersect(MDIWindow window)
        {
            bool intersect = false;

            foreach (MDIWindow wrapper in Wrappers)
            {
                if (MDIWindow.Intersects(window, wrapper)
                    && (window.Content != wrapper.Content))
                {
                    intersect = true;
                    break;
                }
            }

            return intersect;
        }

        

        /// <summary>
        /// Handles the ScrollChanged event of the viewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.ScrollChangedEventArgs"/> instance containing the event data.</param>
        private void ViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
             ScrollViewer viewer = (ScrollViewer)sender;

            if (viewer.ScrollableHeight != 0 &&
                viewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                foreach (MDIWindow window in Wrappers)
                {
                    if (!window.WasMinimizedDragged)
                    {
                        Rect bounds = DocumentContainer.GetMDIMinimizedBounds(window.Content);

                        if (!bounds.IsEmpty)
                        {
                            double height = (viewer.ComputedHorizontalScrollBarVisibility != Visibility.Visible) ?
                                e.ExtentHeightChange
                                : e.ExtentHeightChange + GetHorizontalScrollBar(viewer).ActualHeight;
                            bounds.Y += height;
                            //if (window.IsMaximized && height == 0 && firsttime && window.ContentPresenter.DesiredSize.Height > this.DesiredSize.Height)
                            //{
                            //    bounds.Y +=window.DesiredSize.Height;
                            //    firsttime = false;
                            //}
                            DocumentContainer.SetMDIMinimizedBounds(window.Content, bounds);
 
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the size of the valid.
        /// </summary>
        /// <param name="size">The size of DocumentContainer.</param>
        /// <returns>Size of DocumentContainer</returns>
        private Size GetValidSize(Size size)
        {
            Size holdSize = DocumentContainerHelper.GetHoldSize(this);
            return Size.Empty == holdSize ? size : holdSize;
        }

        /// <summary>
        /// Subscribes to commands.
        /// </summary>
        private void SubscribeToCommands()
        {
            CommandBinding bindingRestoreAll = new CommandBinding(
                DocumentContainer.RestoreAllDocumentsCommand,
                new ExecutedRoutedEventHandler(ExecuteRestoreAllDocumentsCommand),
                new CanExecuteRoutedEventHandler(CanExecuteRestoreAllDocumentsCommand));

            CommandBinding bindingMinimizeAll = new CommandBinding(
                DocumentContainer.MinimizeAllDocumentsCommand,
                new ExecutedRoutedEventHandler(ExecuteMinimizeAllDocumentsCommand),
                new CanExecuteRoutedEventHandler(CanExecuteMinimizeAllDocumentsCommand));

            CommandBinding bindingHideAll = new CommandBinding(
                DocumentContainer.HideAllDocumentsCommand,
                new ExecutedRoutedEventHandler(ExecuteHideAllDocumentsCommand));

            CommandBinding bindingBeginMoving = new CommandBinding(
                DocumentContainer.BeginDocumentMovingCommand,
                new ExecutedRoutedEventHandler(ExecuteBeginDocumentMovingCommand),
                new CanExecuteRoutedEventHandler(CanExecuteBeginDocumentMovingCommand));

            CommandBinding bindingBeginResizing = new CommandBinding(
                DocumentContainer.BeginDocumentResizingCommand,
                new ExecutedRoutedEventHandler(ExecuteBeginDocumentResizingCommand),
                new CanExecuteRoutedEventHandler(CanExecuteBeginDocumentResizingCommand));

            CommandBindings.Add(bindingBeginMoving);
            CommandBindings.Add(bindingBeginResizing);
            CommandBindings.Add(bindingMinimizeAll);
            CommandBindings.Add(bindingRestoreAll);
            CommandBindings.Add(bindingHideAll);
        }

        /// <summary>
        /// Overrides the input.
        /// </summary>
        private void OverrideInput()
        {
            if (m_windowMoveOrResize == null && Container.ActiveDocument != null)
            {
                m_windowMoveOrResize = FindWindowByEventSource(Container.ActiveDocument);
            }

            if (m_windowMoveOrResize != null && CaptureMouse())
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
                MoveMDIAdorner adorner = new MoveMDIAdorner(m_windowMoveOrResize);
                m_currentAdorner = adorner;
                if(adorner!=null)
                    try
                    {
                        layer.Add(adorner);
                    }
                    catch { }
                m_windowMoveOrResize.SetValue(DocumentContainerHelper.ForceIsActiveProperty, true);

                Focusable = true;
                LostMouseCapture += new MouseEventHandler(OnMDILayoutPanelLostMouseCapture);
#if !SyncfusionFramework3_5
                //LostTouchCapture += MDILayoutPanel_LostTouchCapture;
                //TouchDown += MDILayoutPanel_TouchDown;
#endif
                LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnMDILayoutPanelLostKeyboardFocus);
                KeyDown += new KeyEventHandler(OnMDILayoutPanelKeyDown);
                MouseDown += new MouseButtonEventHandler(OnMDILayoutPanelMouseDown);
                Focus();
            }
            else
            {
                KeyboardOverrideMode = KeyboardOverrideMode.None;
            }
        }

        /// <summary>
        /// Releases' the input override.
        /// </summary>
        private void ReleseInputOverride()
        {
            if (m_windowMoveOrResize != null)
            {
                if (m_currentAdorner != null)
                {
                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
                    layer.Remove(m_currentAdorner);
                    m_currentAdorner = null;
                }

                Focusable = false;
                m_windowMoveOrResize.ClearValue(DocumentContainerHelper.ForceIsActiveProperty);
                LostMouseCapture -= new MouseEventHandler(OnMDILayoutPanelLostMouseCapture);
                KeyDown -= new KeyEventHandler(OnMDILayoutPanelKeyDown);
                LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(OnMDILayoutPanelLostKeyboardFocus);
                MouseDown -= new MouseButtonEventHandler(OnMDILayoutPanelMouseDown);
#if !SyncfusionFramework3_5
                //TouchDown -= MDILayoutPanel_TouchDown;
#endif
                m_windowMoveOrResize.Focus();
                m_windowMoveOrResize = null;
            }
        }

        /// <summary>
        /// Called when [MDI layout panel mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnMDILayoutPanelMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                e.Handled = true;
                ReleaseMouseCapture();
            }
        }

#if !SyncfusionFramework3_5
        //void MDILayoutPanel_LostTouchCapture(object sender, TouchEventArgs e)
        //{
        //    if(Container!=null && Container.IsTouchEnabled && Container.m_documentContainerTouchDeviceId==e.TouchDevice.Id)
        //        KeyboardOverrideMode = KeyboardOverrideMode.None;
        //}

        //void MDILayoutPanel_TouchDown(object sender, TouchEventArgs e)
        //{
        //    if (Container != null && Container.IsTouchEnabled && Container.m_documentContainerTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        e.Handled = true;
        //        ReleaseTouchCapture(e.TouchDevice);
        //    }
        //}
#endif

        /// <summary>
        /// Called when [MDI layout panel lost keyboard focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void OnMDILayoutPanelLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.OldFocus == this)
            {
                ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Called when [MDI layout panel key down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void OnMDILayoutPanelKeyDown(object sender, KeyEventArgs e)
        {
            if (m_windowMoveOrResize == null)
            {
                ReleaseMouseCapture();
            }
            else if (Container.IsAllowMDIResize)
            {
                PrepareKyeDown(e.Key);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Prepares the kye down.
        /// </summary>
        /// <param name="key">The key of element.</param>
        private void PrepareKyeDown(Key key)
        {
            UIElement element = m_windowMoveOrResize.Content;

            if (!DocumentContainer.GetAllowMDIResize(element))
            {
                return;
            }

            MDIWindow window = (MDIWindow)VisualUtils.FindAncestor(element, typeof(MDIWindow));

            if (window != null)
            {
                Rect bounds = window.GetMDIBounds();
                bool bChanged = false;
                double sizeChange = MIN_STEP;
                Orientation orientation = Orientation.Horizontal;

                switch (key)
                {
                    case Key.Up:
                        sizeChange = -MIN_STEP;
                        orientation = Orientation.Vertical;
                        bChanged = true;
                        break;

                    case Key.Down:
                        orientation = Orientation.Vertical;
                        bChanged = true;
                        break;

                    case Key.Left:
                        sizeChange = -MIN_STEP;
                        bChanged = true;
                        break;

                    case Key.Right:
                        bChanged = true;
                        break;

                    case Key.Escape:
                        CancelMoveOrResize();
                        break;

                    case Key.Enter:
                        ReleaseMouseCapture();
                        break;
                }

                if (bChanged)
                {
                    bounds = PrepareBounds(bounds, sizeChange, orientation);

                    if (window.IsMinimized)
                    {
                        DocumentContainer.SetMDIMinimizedBounds(element, bounds);
                        window.WasMinimizedDragged = true;
                    }
                    else
                    {
                        DocumentContainer.SetMDIBounds(element, bounds);
                    }
                }
            }
        }

        /// <summary>
        /// Prepares the bounds.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="sizeChange">The size change.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns>Rect of orientation</returns>
        private Rect PrepareBounds(Rect bounds, double sizeChange, Orientation orientation)
        {
            if (KeyboardOverrideMode == KeyboardOverrideMode.WindowMove)
            {
                if (Orientation.Horizontal == orientation)
                {
                    bounds.X += sizeChange;
                }
                else
                {
                    bounds.Y += sizeChange;
                }
            }
            else
            {
                if (Orientation.Horizontal == orientation)
                {
                    double width = bounds.Width;
                    width += sizeChange;
                    bounds.Width = Math.Max(MIN_WIDTH, width);
                }
                else
                {
                    double height = bounds.Height;
                    height += sizeChange;
                    bounds.Height = Math.Max(MIN_HEIGHT, height);
                }
            }

            return bounds;
        }

        /// <summary>
        /// Cancels the move or resize.
        /// </summary>
        private void CancelMoveOrResize()
        {
            UIElement element = m_windowMoveOrResize.Content;
            ReleaseMouseCapture();
            DocumentContainer.SetMDIBounds(element, m_boundsInitial);
        }

        /// <summary>
        /// Updates the start point.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        /// <param name="sizeElement">The size element.</param>
        private void UpdateStartPoint(Size availableSize, Size sizeElement)
        {
            if (m_startPoint.X + sizeElement.Width + DEFAULT_OFFSET < availableSize.Width
                && m_startPoint.Y + sizeElement.Height + DEFAULT_OFFSET < availableSize.Height)
            {
                m_startPoint.X += DEFAULT_OFFSET;
                m_startPoint.Y += DEFAULT_OFFSET;
            }
            else
            {
                m_startPoint.X = DEFAULT_OFFSET;
                m_startPoint.Y = DEFAULT_OFFSET;
            }
        }

        /// <summary>
        /// Called when [MDI layout panel lost mouse capture].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMDILayoutPanelLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
                KeyboardOverrideMode = KeyboardOverrideMode.None;
        }

        /// <summary>
        /// Called when [container is in MDI maximized state changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnContainerIsInMDIMaximizedStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InvalidateMeasure();
            InvalidateArrange();
        }

        /// <summary>
        /// Switches the window.
        /// </summary>
        /// <param name="outOfNext">The out of next.</param>
        /// <param name="inOfNext">The in of next.</param>
        private void SwitchWindow(int outOfNext, int inOfNext)
        {
            MDIWindow next = m_wrappers[outOfNext];
            MDIWindow current = m_wrappers[0];

            m_wrappers.RemoveAt(outOfNext);
            m_wrappers.RemoveAt(0);
            m_wrappers.Insert(0, next);
            m_wrappers.Insert(inOfNext, current);
            UpdateWrappersOrderLayout();
        }

        /// <summary>
        /// Sets the circle active window.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        private void SetCircleActiveWindow(MDIWindow wrapper)
        {
            int index = m_wrappers.IndexOf(wrapper);
            List<MDIWindow> list = new List<MDIWindow>();

            for (int i = index - 1; i > -1; --i)
            {
                list.Add(m_wrappers[i]);
                m_wrappers.RemoveAt(i);
            }

            for (int i = list.Count - 1; i > -1; --i)
            {
                m_wrappers.Insert(m_wrappers.Count, list[i]);
            }

            UpdateWrappersOrderLayout();
        }

        /// <summary>
        /// Sets the random active window.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        private void SetRandomActiveWindow(MDIWindow wrapper)
        {
            m_wrappers.Remove(wrapper);
            m_wrappers.Insert(0, wrapper);
            UpdateWrappersOrderLayout();
        }

        /// <summary>
        /// Detaches from container.
        /// </summary>
        /// <param name="container">The container.</param>
        private void DetachFromContainer(IDocumentContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            INotifyCollectionChanged colNotifications = container.Items;

            if (colNotifications != null)
            {
                colNotifications.CollectionChanged -= OnNotificationsCollectionChanged;
            }

            RemoveVisualChildren(container.Items);
        }

        /// <summary>
        /// Attaches to container.
        /// </summary>
        /// <param name="container">The container.</param>
        private void  AttachToContainer(IDocumentContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            INotifyCollectionChanged colNotifications = container.Items;

            if (colNotifications != null)
            {
                colNotifications.CollectionChanged += OnNotificationsCollectionChanged;
            }

            List<ContentControl> controllist = new List<ContentControl>();
            bool _flag = false;
            foreach (object obj in container.Items)
            {
                if (obj is UIElement)
                {
                    AddVisualChildren(container.Items);
                    break;
                }
                else
                {
                    ContentControl control = new ContentControl();
                    control.DataContext = obj;
                    controllist.Add(control);
                    _flag = true;
                }


            }
            if (_flag)
            {
                AddVisualChildren(controllist);
            }
        }

        /// <summary>
        /// States the change handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void StateChangeHandler(object sender, RoutedEventArgs e)
        {
            UIElement source = (UIElement)e.OriginalSource;
            DockState dockState = DockingManager.GetState(source);

            if (!ShouldDocumentBeVisible(source))
            {
                if (dockState == DockState.Hidden)
                {
                    RemoveVisualChildren(new[] { source }, false);
                    SetNewActiveDocument();
                }
            }
            else if (!m_WrappersTable.ContainsKey(source))
            {
                AddVisualChildren(new[] { source }, false);
            }
        }

        /// <summary>
        /// Sets the new active document.
        /// </summary>
        private void SetNewActiveDocument()
        {
            MDIWindow activeWindow = null;
            UIElement activeDocument = Container.ActiveDocument;

            if (null != activeDocument && 0 < m_wrappers.Count)
            {
                foreach (MDIWindow window in m_wrappers)
                {
                    if (window.Content == activeDocument)
                    {
                        activeWindow = window;
                        break;
                    }
                }

                if (null == activeWindow)
                {
                    Container.ActiveDocument = m_wrappers[0].Content;
                }
            }
        }

        /// <summary>
        /// Filters the state of the visual children by.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <returns>IEnumerable in children</returns>
        private IEnumerable FilterVisualChildrenByState(IEnumerable children)
        {
            foreach (UIElement child in children)
            {
                if (ShouldDocumentBeVisible(child))
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Called when [notifications collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnNotificationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    GenerateVisualsFromObject(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveVisualsUsingObject(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    RemoveVisualsUsingObject(e.OldItems);
                    GenerateVisualsFromObject(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    ResetVisibleList();
                    break;
            }
        }

        private void GenerateVisualsFromObject(IList items)
        {
            bool _lflag;
            List<ContentControl> controllist = new List<ContentControl>();
            foreach (object obj in items)
            {
                if (obj is UIElement)
                {
                    AddVisualChildren(items);
                    break;
                }
                else
                {
                    ContentControl control = new ContentControl();
                    control.DataContext = obj;
                    controllist.Add(control);
                    _lflag = true;
                }
                if (_lflag)
                {
                    AddVisualChildren(controllist);
                }

            }
        }

        private void RemoveVisualsUsingObject(IList items)
        {
            bool _lflag=false;
            List<ContentControl> controllist = new List<ContentControl>();

            foreach (object obj in items)
            {
                if (obj is UIElement)
                {
                    RemoveVisualChildren(items);
                    break;
                }
                else
                {
                    foreach (MDIWindow window in m_wrappers)
                    {
                        if ((window.Content as ContentControl).DataContext == obj)
                        {
                            controllist.Add(window.Content as ContentControl);
                            _lflag = true;
                        }
                    }
                }
                if (_lflag)
                {
                    RemoveVisualChildren(controllist);
                }

            }
        }
        /// <summary>
        /// Attaches the handlers.
        /// </summary>
        /// <param name="children">The children.</param>
        private void AttachHandlers(IEnumerable children)
        {
            if (children == null)
            {
                throw new ArgumentNullException("children");
            }

            foreach (UIElement item in children)
            {
                AttachHandlers(item);
            }
        }

        /// <summary>
        /// Detaches the handlers.
        /// </summary>
        /// <param name="children">The children.</param>
        private void DetachHandlers(IEnumerable children)
        {
            if (children == null)
            {
                throw new ArgumentNullException("children");
            }

            foreach (UIElement item in children)
            {
                DetachHandlers(item);
            }
        }

        /// <summary>
        /// Adds the visual children.
        /// </summary>
        /// <param name="children">The children.</param>
        private void AddVisualChildren(IEnumerable children)
        {
            AddVisualChildren(children, true);
        }

        /// <summary>
        /// Adds the visual children.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <param name="attachHandlers">if set to <c>true</c> [attach handlers].</param>
        private void AddVisualChildren(IEnumerable children, bool attachHandlers)
        {
            if (children == null)
            {
                throw new ArgumentNullException("children");
            }

            if (attachHandlers)
            {
                AttachHandlers(children);
            }

            FrameworkElement last = null;

            foreach (FrameworkElement item in FilterVisualChildrenByState(children))
            {
                DocumentContainer.CheckNameOfElement(item);
                MDIWindow wrapper = CreateDocumentWrapper(item);
                m_wrappers.Add(wrapper);
                m_WrappersTable[item] = wrapper;

                AddVisualChild(wrapper);
                AddLogicalChild(wrapper);
                wrapper.DataContext = item.DataContext;
                //wrapper.InvalidateVisual();
                //wrapper.UpdateLayout();

                if (null == last)
                {
                    last = item;
                }
            }

            if (null != last && Container.ActiveDocument==null)
            {
                PrepareActiveDocument(last);
            }

            //InvalidateMeasure();
            //InvalidateArrange();
            //InvalidateVisual();
        }

        /// <summary>
        /// Removes the visual children.
        /// </summary>
        /// <param name="children">The children.</param>
        private void RemoveVisualChildren(IEnumerable children)
        {
            RemoveVisualChildren(children, true);
            if (Container != null && Container.Items.Count > 0 && m_wrappers.Count > 0)
            {
                //Container.ActiveDocument = null;
                Container.ActiveDocument = m_wrappers[0].Content;
            }
        }

        /// <summary>
        /// Updates the wrappers order layout.
        /// </summary>
        private void UpdateWrappersOrderLayout()
        {
            foreach (MDIWindow wrapper in m_wrappers)
            {
                try
                {
                    RemoveVisualChild(wrapper);
                    AddVisualChild(wrapper);
                }
                catch (Exception) { }

            }
        }

        /// <summary>
        /// Processes the bounds change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void ProcessBoundsChange(object sender, RoutedPropertyChangedEventArgs<Rect> e)
        {
            InvalidateMeasure();
            InvalidateArrange();
        }

        /// <summary>
        /// Called when [generic document layout panel loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMDILayoutPanelLoaded(object sender, RoutedEventArgs e)
        {
            if (null == Container)
            {
                Container = TemplatedParent as DocumentContainer;
            }
            SetMDILayoutBasedOnMDIMode();
        }

        /// <summary>
        /// Detaches from the container.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMDILayoutPanelUnloaded(object sender, RoutedEventArgs e)
        {
            Container = null;
        }

        /// <summary>
        /// Prepares the active document.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>MDI window wrapper</returns>
        private MDIWindow PrepareActiveDocument(UIElement element)
        {
            MDIWindow returnValue = null;

            foreach (MDIWindow wrapper in m_wrappers)
            {
                if (element == wrapper.Content)
                {
                    SetActiveWindow(wrapper);
                    SetActive(wrapper, m_wrappers);
                    returnValue = wrapper;
                    break;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Sets the layout.
        /// </summary>
        /// <param name="cnt">The CNT value.</param>
        /// <param name="itemSize">Size of the layout item.</param>
        /// <param name="offsetX">The offset X value.</param>
        /// <param name="offsetY">The offset Y value.</param>
        private void SetLayout(int cnt, Size itemSize, double offsetX, double offsetY)
        {
            Point start = new Point(0, 0);

            for (int i = cnt - 1; i > -1; --i)
            {
                DependencyObject element = Wrappers[i].Content;

                MDIWindow window = VisualUtils.FindAncestor((Visual)element, typeof(MDIWindow)) as MDIWindow;

                if (window != null)
                {
                    if (DocumentContainer.IsMinimized(element) || DocumentContainer.IsMinimized(window)
                        || window.IsMaximized || !DocumentContainer.GetAllowMDIResize(window))
                    {
                        DocumentContainer.SetAllowMDIResize(window, true);
                        window.IsMaximized = false;
                        DocumentContainer.SetMDIWindowState(window, MDIWindowState.Normal);
                        DocumentContainer.SetMDIWindowState(element, MDIWindowState.Normal);
                    }
                }
            }

            for (int i = cnt - 1; i > -1; --i)
            {
                DependencyObject element = Wrappers[i].Content;
                Rect rect = new Rect(start, itemSize);
                DocumentContainer.SetMDIBounds(element, rect);
                start.X += offsetX;
                start.Y += offsetY;
            }
        }

        /// <summary>
        /// Validates the rect last unknown.
        /// </summary>
        /// <param name="rect">The rect height and width.</param>
        /// <returns>Rect height and width</returns>
        private static Rect ValidateRectLastUnknown(Rect rect)
        {
            if (double.IsInfinity(rect.Height))
            {
                rect.Height = 200;
            }

            if (double.IsInfinity(rect.Width))
            {
                rect.Width = 200;
            }

            return rect;
        }

        /// <summary>
        /// Calls OnContainerChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDILayoutPanel instance = (MDILayoutPanel)d;
            instance.OnContainerChanged(e);
        }

        /// <summary>
        /// Gets the horizontal scroll bar.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <returns>ScrollBar of the element</returns>
        private static ScrollBar GetHorizontalScrollBar(DependencyObject viewer)
        {
            ScrollBar scrollBar = null;
            Grid grid = (Grid)VisualTreeHelper.GetChild(viewer, 0);

            foreach (UIElement element in grid.Children)
            {
                ScrollBar bar = element as ScrollBar;
                if (bar != null &&
                    bar.Orientation == Orientation.Horizontal)
                {
                    scrollBar = bar;
                    break;
                }
            }

            return scrollBar;
        }

        /// <summary>
        /// Gets the vertical scroll bar.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <returns></returns>
        private static ScrollBar GetVerticalScrollBar(DependencyObject viewer)
        {
            ScrollBar scrollBar = null;
            Grid grid = (Grid)VisualTreeHelper.GetChild(viewer, 0);

            foreach (UIElement element in grid.Children)
            {
                ScrollBar bar = element as ScrollBar;
                if (bar != null &&
                    bar.Orientation == Orientation.Vertical)
                {
                    scrollBar = bar;
                    break;
                }
            }

            return scrollBar;
        }

        /// <summary>
        /// Finds the window by event source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>MDIWindow of source</returns>
        private static MDIWindow FindWindowByEventSource(object source)
        {
            return source as MDIWindow
                ?? VisualUtils.FindAncestor((Visual)source, typeof(MDIWindow)) as MDIWindow;
        }

        /// <summary>
        /// Finds the child by event source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>UIElement of source</returns>
        private static UIElement FindChildByEventSource(object source)
        {
            MDIWindow window = FindWindowByEventSource(source);
            return (window != null) ? window.Content : null;
        }

        /// <summary>
        /// Get the state of the element that is related to the provided command source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>MDIWindowState of source</returns>
        private static MDIWindowState? FindChildStateByEventSource(object source)
        {
            UIElement element = FindChildByEventSource(source);

            if (element != null)
            {
                return DocumentContainer.GetMDIWindowState(element);
            }

            return null;
        }

        /// <summary>
        /// Calls OnKeyboardOverrideModeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnKeyboardOverrideModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDILayoutPanel instance = (MDILayoutPanel)d;
            instance.OnKeyboardOverrideModeChanged(e);
        }
        #endregion

        #region Commands processing
        /// <summary>
        /// Restores all the MDI windows.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteRestoreAllDocumentsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (Container.Mode == DocumentContainerMode.MDI)
            {
                IList<Control> wrappers = GetOrderedItems();

                foreach (MDIWindow wind in wrappers)
                {
                    if (!Container.IsInMDIMaximizedState &&
                        (MDIWindowState)wind.GetValue(DocumentContainer.MDIWindowStateProperty) == MDIWindowState.Normal)
                    {
                        continue;
                    }

                    SetActiveItem((FrameworkElement)wind.Content);

                    foreach (CommandBinding binding in wind.CommandBindings)
                    {
                        if (binding.Command == DocumentContainer.RestoreDocumentCommand)
                        {
                            binding.Command.Execute(null);
                            break;
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("MDI related command should not be execute in non-MDI mode.");
            }
        }

        /// <summary>
        /// Checks whether MDI document can be put into normal state.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecuteRestoreAllDocumentsCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Container.Mode == DocumentContainerMode.MDI;
        }

        /// <summary>
        /// Minimizes all the documents.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteMinimizeAllDocumentsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (Container.Mode == DocumentContainerMode.MDI)
            {
                Container.IsInMDIMaximizedState = false;

                if (m_wrappers.Count > 0)
                {
                    IList<Control> wrappers = GetOrderedItems();
                    foreach (MDIWindow wind in wrappers)
                    {
                        if (DocumentContainer.IsMinimized(wind.Content))
                        {
                            continue;
                        }

                        SetActiveItem((FrameworkElement)wind.Content);

                        foreach (CommandBinding binding in wind.CommandBindings)
                        {
                            if (binding.Command == DocumentContainer.MinimizeDocumentCommand)
                            {
                                binding.Command.Execute(null);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("MDI related command should not be execute in non-MDI mode.");
            }
        }

        /// <summary>
        /// Checks whether minimization command can be executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecuteMinimizeAllDocumentsCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Container.CanMDIMinimize;
        }

        /// <summary>
        /// Hides all the MDI window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteHideAllDocumentsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (Container.Mode == DocumentContainerMode.MDI)
            {
                for (int i = 0; i < m_wrappers.Count; i++)
                {
                    MDIWindow wind = m_wrappers[i];

                    foreach (CommandBinding binding in wind.CommandBindings)
                    {
                        if (binding.Command == DocumentContainer.HideDocumentCommand)
                        {
                            binding.Command.Execute(null);
                            i--;
                            break;
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("MDI related command should not be execute in non-MDI mode.");
            }
        }

        /// <summary>
        /// Executes the begin document moving command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteBeginDocumentMovingCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MDIWindow window = FindWindowByEventSource(e.OriginalSource);

            if (window == null)
            {
                throw new InvalidOperationException("Command has been executed on a wrong target.");
            }

            m_windowMoveOrResize = window;

            KeyboardOverrideMode = KeyboardOverrideMode.WindowMove;
            m_boundsInitial = window.GetMDIBounds();
        }

        /// <summary>
        /// Determines whether this instance [can execute begin document moving command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecuteBeginDocumentMovingCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanExecuteBeginDocumentResizing(e.OriginalSource);
        }
        bool resizeflag = true;
        /// <summary>
        /// Determines whether this instance [can execute begin document resizing command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecuteBeginDocumentResizingCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DependencyObject element = (DependencyObject)e.OriginalSource;
            e.CanExecute = CanExecuteBeginDocumentResizing(element)
                && DocumentContainer.GetAllowMDIResize(element)
                && Container.IsAllowMDIResize && resizeflag;
        }

        /// <summary>
        /// Determines whether this instance [can execute begin document resizing].
        /// </summary>
        /// <param name="originalSource">The original source.</param>
        /// <returns>
        /// <c>true</c> if this instance [can execute begin document resizing]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteBeginDocumentResizing(object originalSource)
        {
            bool canExecute = (Container.Mode == DocumentContainerMode.MDI) && !Container.IsInMDIMaximizedState;
            MDIWindowState? state = Container.Mode == DocumentContainerMode.MDI
                ? FindChildStateByEventSource(originalSource) : null;
            canExecute &= state.HasValue && !Container.IsInMDIMaximizedState;

            return canExecute;
        }

        /// <summary>
        /// Executes the begin document resizing command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteBeginDocumentResizingCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MDIWindow window = FindWindowByEventSource(e.OriginalSource);

            if (window == null)
            {
                throw new InvalidOperationException("Command has been executed on a wrong target.");
            }

            m_windowMoveOrResize = window;

            KeyboardOverrideMode = KeyboardOverrideMode.WindowResize;
            m_boundsInitial = DocumentContainer.GetMDIBounds(window.Content);
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Represents the KeyboardOverrideModePropertyKey 
        /// </summary>
        protected static readonly DependencyPropertyKey KeyboardOverrideModePropertyKey = DependencyProperty.RegisterReadOnly("KeyboardOverrideMode", typeof(KeyboardOverrideMode), typeof(MDILayoutPanel), new FrameworkPropertyMetadata(KeyboardOverrideMode.None, new PropertyChangedCallback(OnKeyboardOverrideModeChanged)));

        /// <summary>
        /// Represents the ContainerPropertyKey 
        /// </summary>
        protected static readonly DependencyPropertyKey ContainerPropertyKey =
            DependencyProperty.RegisterReadOnly(
            "Container",
            typeof(DocumentContainer),
            typeof(MDILayoutPanel),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnContainerChanged)));

        /// <summary>
        /// Represents the Keyboard Override Mode Dependency Property
        /// </summary>
        public static readonly DependencyProperty KeyboardOverrideModeProperty = KeyboardOverrideModePropertyKey.DependencyProperty;

        /// <summary>
        /// Represents the Container Dependency Property
        /// </summary>
        public static readonly DependencyProperty ContainerProperty = ContainerPropertyKey.DependencyProperty;
        #endregion
    }
}