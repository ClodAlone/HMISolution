// <copyright file="DockedElementsContainer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent a container that can position and arrange child docked elements.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
public class DockedElementsContainer : Panel, IRemoveChild, IChildrenResize,IDisposable
    {
        #region Constants
        /// <summary>
        /// Specify the min element length.
        /// </summary>
        private const double MinElementLength = 15;

        /// <summary>
        /// Specify the min element width.
        /// </summary>
        private const double MinElementWidth = 30;

        /// <summary>
        /// Specify the min element height.
        /// </summary>
        private const double MinElementHeight = 20;
        #endregion

        #region Private members

        internal DockedElementsContainer rootParentContainer = null;

        /// <summary>
        /// Specify the splitter.
        /// </summary>
        internal List<Splitter> m_Splitters;

        /// <summary>
        /// Specify the actual child host that needs to be resized during splitter resize
        /// </summary>
        internal DockedElementTabbedHost m_actualhostresize = null;

        /// <summary>
        /// Stores the resizable elements tracked for EdgeChildren Splitter resize mode.
        /// </summary>
        internal ObservableFrameworkElements m_resizableelements = new ObservableFrameworkElements();

        /// <summary>
        /// Specifies whether the container can resize specific to EdgeChildren Splitter resize mode.
        /// </summary>
        internal bool m_cancontainerresize = false;

        /// <summary>
        /// Specify the desired size.
        /// </summary>
        internal Size m_desiredSize;

        internal bool m_checkminmaxsize = true;

        /// <summary>
        /// Specify the previous desired size.
        /// </summary>
        internal Size m_previousSize;

        /// <summary>
        /// Specify the previous multiplier.
        /// </summary>
        internal double m_multiplier;

        /// <summary>
        /// Specify the non assigned fixed length which should be deducted from actual fixed length
        /// </summary>
        internal double m_nonassignedfixedlength;

        internal bool m_dockedwithpreview = false;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="DockedElementsContainer"/> class.
        /// </summary>
        static DockedElementsContainer()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(DockedElementsContainer), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockedElementsContainer"/> class.
        /// </summary>
        /// <param name="dockingManager">The docking manager.</param>
        public DockedElementsContainer(DockingManager dockingManager)
        {
            DockingManager = dockingManager;
            m_desiredSize = Size.Empty;
            m_Splitters = new List<Splitter>();
            this.Unloaded += new RoutedEventHandler(DockedElementsContainer_Unloaded);
        }

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Unloaded -= new RoutedEventHandler(DockedElementsContainer_Unloaded);
            if (m_Splitters.Count > 0)
                m_Splitters.Clear();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        #endregion

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DockedElementsContainer"/> is reclaimed by garbage collection.
        /// </summary>
        ~DockedElementsContainer()
        {
        }

        /// <summary>
        /// Handles the Unloaded event of the DockedElementsContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void DockedElementsContainer_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.Children.Count == 0)
            {
                this.Dispose();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Orientation for the DockedElementsContainer that is used in measure and arrange processes. 
        /// This is a dependency property.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size that this element computed during the measure pass of the layout process.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The computed size, which becomes the desired size for the arrange pass.
        /// </returns>
        Size IDesiredSize.DesiredSize
        {
            get
            {
                return m_desiredSize;
            }

            set
            {
                m_desiredSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the docking manager.
        /// </summary>
        /// <value>The docking manager.</value>
        public DockingManager DockingManager
        {
            get
            {
                return (DockingManager)GetValue(DockingManagerProperty);
            }

            set
            {
                SetValue(DockingManagerProperty, value);
            }
        }

        #endregion

        #region Public method
        /// <summary>
        /// Gets the element located before splitter. This is a dependency property.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockedElementsContainer.ElementBeforeSplitter�dependency property.</returns>
        public static FrameworkElement GetElementBeforeSplitter(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(ElementBeforeSplitterProperty);
        }

        /// <summary>
        /// Sets the element located before splitter. This is a dependency property.
        /// </summary>
        /// <param name="obj">The element on which to set the DockedElementsContainer.ElementBeforeSplitter�dependency property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetElementBeforeSplitter(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(ElementBeforeSplitterProperty, value);
        }

        /// <summary>
        /// Gets the element located after splitter. This is a dependency property.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockedElementsContainer.ElementAfterSplitter�dependency property.</returns>
        public static FrameworkElement GetElementAfterSplitter(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(ElementAfterSplitterProperty);
        }

        /// <summary>
        /// Sets the element located after splitter. This is a dependency property.
        /// </summary>
        /// <param name="obj">The element on which to set the DockedElementsContainer.ElementAfterSplitter�dependency property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetElementAfterSplitter(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(ElementAfterSplitterProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockedElementsContainer.GetSplitterTargetSide�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockedElementsContainer.GetSplitterTargetSide�attached property.</returns>
        public static SplitterTargetSide GetSplitterTargetSide(DependencyObject obj)
        {
            return (SplitterTargetSide)obj.GetValue(SplitterTargetSideProperty);
        }

        /// <summary>
        /// Sets the value of the DockedElementsContainer.GetSplitterTargetSide�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockedElementsContainer.GetSplitterTargetSide�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetSplitterTargetSide(DependencyObject obj, SplitterTargetSide value)
        {
            obj.SetValue(SplitterTargetSideProperty, value);
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Sets new width value of the element.
        /// </summary>
        /// <param name="width">new width value</param>
        void IChildrenResize.SetWidth(double width)
        {
            if (width >= 0)
            {
                if (Orientation == Orientation.Vertical)
                {
                    foreach (IChildrenResize resireChild in Children)
                    {
                        if (resireChild is DockedElementTabbedHost && ((DockedElementTabbedHost)resireChild).InternalDataContext != null)
                        {
                            if (DockingManager.CheckFixedsize(((DockedElementTabbedHost)resireChild).InternalDataContext, Orientation)
                                && !DockingManager.CheckResize(((DockedElementTabbedHost)resireChild).InternalDataContext, DockState.Dock, Orientation)
                                && (DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)resireChild).InternalDataContext) == DockSide.Left ||
                                DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)resireChild).InternalDataContext) == DockSide.Right))
                            {
                                return;
                            }
                        }
                        resireChild.SetWidth(width);
                    }
                }
                else
                {
                    if (m_desiredSize.Width != 0)
                    {
                        double coef = width / m_desiredSize.Width;

                        foreach (IChildrenResize resireChild in Children)
                        {
                            if (resireChild is DockedElementTabbedHost && ((DockedElementTabbedHost)resireChild).InternalDataContext != null)
                            {
                                if (DockingManager.CheckFixedsize(((DockedElementTabbedHost)resireChild).InternalDataContext, Orientation)
                                    && !DockingManager.CheckResize(((DockedElementTabbedHost)resireChild).InternalDataContext, DockState.Dock, Orientation)
                                    && (DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)resireChild).InternalDataContext) == DockSide.Left ||
                                    DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)resireChild).InternalDataContext) == DockSide.Right))
                                {
                                    return;
                                }
                            }
                            resireChild.SetWidth(resireChild.DesiredSize.Width * coef);
                        }
                    }
                }
                
                if (m_desiredSize.IsEmpty)
                {
                    InitDesiredSize(m_desiredSize);
                }
                m_desiredSize = new Size(width < 0 ? MinElementWidth : width, m_desiredSize.Height);
                VisualUtils.InvalidateParentMeasure(this);
            }
        }

        /// <summary>
        /// Sets new height value of the element.
        /// </summary>
        /// <param name="height">new height value</param>
        void IChildrenResize.SetHeight(double height)
        {
            if (height >= 0)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    foreach (IChildrenResize resireChild in Children)
                    {
                        if (resireChild is DockedElementTabbedHost && ((DockedElementTabbedHost)resireChild).InternalDataContext != null)
                        {
                            if (DockingManager.CheckFixedsize(((DockedElementTabbedHost)resireChild).InternalDataContext, Orientation)
                                && !DockingManager.CheckResize(((DockedElementTabbedHost)resireChild).InternalDataContext, DockState.Dock, Orientation)
                                && (DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)resireChild).InternalDataContext) == DockSide.Top ||
                                DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)resireChild).InternalDataContext) == DockSide.Bottom))
                            {
                                return;
                            }
                        }
                        resireChild.SetHeight(height);
                    }
                }
                else
                {
                    

                    foreach (IChildrenResize resireChild in Children)
                    {
                        if (resireChild is DockedElementTabbedHost && ((DockedElementTabbedHost)resireChild).InternalDataContext != null)
                        {
                            if (DockingManager.CheckFixedsize(((DockedElementTabbedHost)resireChild).InternalDataContext, Orientation)
                                && !DockingManager.CheckResize(((DockedElementTabbedHost)resireChild).InternalDataContext, DockState.Dock, Orientation)
                                && (DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)resireChild).InternalDataContext) == DockSide.Top ||
                                DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)resireChild).InternalDataContext) == DockSide.Bottom))
                            {
                                return;
                            }
                        }
                        resireChild.SetHeight(height);
                    }
                }
                
                if (m_desiredSize.IsEmpty)
                {
                    InitDesiredSize(m_desiredSize);
                }
                m_desiredSize = new Size(m_desiredSize.Width, height < 0 ? MinElementHeight : height);
                VisualUtils.InvalidateParentMeasure(this);
            }
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="visual">The visual.</param>
        void IRemoveChild.RemoveChild(Visual visual)
        {
            Children.Remove((UIElement)visual);
            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);

            CheckVisibility();
            CheckIfEmpty();
            InvalidateSplitters();
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>The requested child element.</returns>
        protected override Visual GetVisualChild(int index)
        {
            int indexBase = (int)Math.Truncate(index / 2d);
            int iSplitter = indexBase;
            bool bSplitter = indexBase * 2 != index;
            Visual returnElement = null;

            if (!bSplitter)
            {
               if(Children.Count>indexBase && indexBase>=0)
                returnElement = Children[indexBase];
            }
            else
            {
                InvalidateSplitters();
                if (m_Splitters.Count > iSplitter) 
                returnElement = m_Splitters[iSplitter];
            }

            return returnElement;
        }

        internal void NestedContainerVisibility(Visibility visibility)
        {
            foreach (Splitter splitter in m_Splitters)
            {
                if(splitter !=null)
                {
                    splitter.Visibility = visibility;
                }
            }
            foreach (UIElement element in Children)
            {
                if (element is DockedElementsContainer)
                {
                    (element as DockedElementsContainer).NestedContainerVisibility(visibility);
                }
            }
        }

        internal void SetVisibility(Visibility visible)
        {
            Visibility = visible;
            DockedElementsContainer parentcontainer = Parent as DockedElementsContainer;
            if (parentcontainer != null)
            {
                if (CheckAllVisibility(parentcontainer, visible))
                {
                    DockingManager.m_restorehostelements.Add(parentcontainer);
                    parentcontainer.SetVisibility(visible);
                }
            }
        }

        private bool CheckAllVisibility(DockedElementsContainer container, Visibility visibility)
        {
            List<FrameworkElement> list = new List<FrameworkElement>();

            foreach (FrameworkElement element in container.Children)
            {
                list.Add(element);
            }

            if (list.TrueForAll(e => e.Visibility == visibility))
                return true;

            return false;
        }

        internal void ResetVisibility()
        {
            if (CheckAllVisibility(this, Visibility.Collapsed))
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                foreach (FrameworkElement element in Children)
                {
                    if (element is DockedElementsContainer)
                    {
                        DockedElementsContainer container = element as DockedElementsContainer;
                        container.ResetVisibility();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the max offset.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="splitterorientation">The splitterorientation.</param>
        /// <returns></returns>
        internal double GetMaxOffset(FrameworkElement element, Orientation splitterorientation)
        {
            double minlength = 0;
            if (element is DockedElementsContainer)
            {
                double size = splitterorientation == System.Windows.Controls.Orientation.Vertical ? GetMinWidth(element as DockedElementsContainer, splitterorientation) :
                   GetMinHeight(element as DockedElementsContainer, splitterorientation);
                minlength += size;
            }
            else if (element.Visibility == System.Windows.Visibility.Visible &&
                element is DockedElementTabbedHost && (element as DockedElementTabbedHost).InternalDataContext != null)
            {
                FrameworkElement actualelement = (element as DockedElementTabbedHost).InternalDataContext;
                DockSide side = DockingManager.GetSideInDockedMode(actualelement);
                bool canaddsize = splitterorientation == System.Windows.Controls.Orientation.Vertical ? side == DockSide.Left || side == DockSide.Right : side == DockSide.Top || side == DockSide.Bottom;
                if (canaddsize)
                {
                    double size = splitterorientation == System.Windows.Controls.Orientation.Vertical ? DockingManager.GetDesiredMinWidthInDockedMode(actualelement) :
                        DockingManager.GetDesiredMinHeightInDockedMode(actualelement);
                    size = (size == 0 ? splitterorientation == System.Windows.Controls.Orientation.Vertical ? MinElementWidth : MinElementHeight : size);
                    minlength += size;
                }
                if (element.ActualWidth == MinElementWidth)
                {
                    if (actualelement is UserControl)
                    {
                        if (((actualelement as UserControl).Content) != null && (((actualelement as UserControl).Content) as Panel) != null)
                        {
                            for (int i = 0; i < (((actualelement as UserControl).Content) as Panel).Children.Count; i++)
                            {
                                if ((((actualelement as UserControl).Content) as Panel).Children[i] is WebBrowser)
                                    (element as DockedElementTabbedHost).MarkAsFrozen = true;
                            }
                        }
                    }
                    else if (actualelement is Panel)
                    {
                        for (int i = 0; i < (actualelement as Panel).Children.Count; i++)
                        {
                            if ((actualelement as Panel).Children[i] is WebBrowser)
                                (element as DockedElementTabbedHost).MarkAsFrozen = true;
                        }
                    }
                }
                else
                    (element as DockedElementTabbedHost).MarkAsFrozen = false;

            }

            return minlength == 0 ? 50 : minlength;
        }

        /// <summary>
        /// Gets the width of the min.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="splitterorientation">The splitterorientation.</param>
        /// <returns></returns>
        private double GetMinWidth(DockedElementsContainer container, Orientation splitterorientation)
        {
            double minWidth = 0;
            if (container.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                foreach (FrameworkElement element in container.Children)
                {
                    if (element is DockedElementsContainer)
                        minWidth += GetMinWidth(element as DockedElementsContainer, splitterorientation);
                    else if (element.Visibility == System.Windows.Visibility.Visible &&
                        element is DockedElementTabbedHost && (element as DockedElementTabbedHost).InternalDataContext != null)
                    {
                        double width = DockingManager.GetDesiredMinWidthInDockedMode((element as DockedElementTabbedHost).InternalDataContext);
                        width = (width == 0 ? MinElementWidth : width);
                        minWidth += width;
                    }
                }
            }
            else
            {
                foreach (FrameworkElement element in container.Children)
                {
                    if (element is DockedElementsContainer)
                        minWidth += GetMinWidth(element as DockedElementsContainer, splitterorientation);
                }
            }

            minWidth = (minWidth > MinElementWidth ? minWidth : MinElementWidth);

            return minWidth;
        }

        /// <summary>
        /// Gets the height of the min.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="splitterorientation">The splitterorientation.</param>
        /// <returns></returns>
        private double GetMinHeight(DockedElementsContainer container, Orientation splitterorientation)
        {
            double minHeight = 0;
            if (container.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                foreach (FrameworkElement element in container.Children)
                {
                    if (element is DockedElementsContainer)
                        minHeight += GetMinHeight(element as DockedElementsContainer, splitterorientation);
                    else if (element.Visibility == System.Windows.Visibility.Visible &&
                        element is DockedElementTabbedHost && (element as DockedElementTabbedHost).InternalDataContext != null)
                    {
                        double height = DockingManager.GetDesiredMinHeightInDockedMode((element as DockedElementTabbedHost).InternalDataContext);
                        height = (height == 0 ? MinElementHeight : height);
                        minHeight += height;
                    }
                }
            }
            else
            {
                foreach (FrameworkElement element in container.Children)
                {
                    if (element is DockedElementsContainer)
                        minHeight += GetMinHeight(element as DockedElementsContainer, splitterorientation);
                }
            }

            minHeight = (minHeight > MinElementHeight ? minHeight : MinElementHeight);

            return minHeight;
        }


        /// <summary>
        /// Measures the size in layout required for child elements and 
        /// determines a size for the DockedElementsContainer and derived class.
        /// </summary>
        /// <param name="availableSize"> The available size that this element can give to child elements. 
        /// Infinity can be specified as a value to indicate that the element will size to whatever
        /// content is available.</param>
        /// <returns> The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FrameworkElement element in Children)
            {
                if (element.Visibility != Visibility.Collapsed &&
                    element is DockedElementsContainer)
                {
                    element.Measure(availableSize);
                }
            }

            InvalidateSplitters();
            MeasureSplitters(availableSize);
            InitDesiredSize(availableSize);
            HandleMinimumLength();

            return new Size(0, 0);
        }

        /// <summary>
        /// Checks the size of the container.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void CheckContainerSize(FrameworkElement element, bool bIsHorizontalOrientation)
        {
            List<FrameworkElement> ElementList = new List<FrameworkElement>();
            if (element is DockedElementsContainer)
            {
                foreach (FrameworkElement Item in (element as DockedElementsContainer).Children)
                {
                    if ((Item is DockedElementTabbedHost) && (Item as DockedElementTabbedHost).InternalDataContext != null && DockingManager.IsVisibleState(DockingManager.GetState((Item as DockedElementTabbedHost).InternalDataContext)))
                    {
                        ElementList.Add((Item as DockedElementTabbedHost).InternalDataContext);
                    }
                }


                foreach (FrameworkElement child in ElementList)
                {
                    if (!ElementList.Contains(DockingManager.GetTargetElement(child, DockingManager.GetState(child))) || child == DockingManager.GetTargetElement(child, DockingManager.GetState(child)))
                    {
                        if (!DockingManager.GetElementFlag(child))
                        {
                            Size containersize = DockingManager.GetDockedElementsContainerDesiredSize(child);

                            if (bIsHorizontalOrientation)
                            {
                                if (containersize.Width > 0)
                                {
                                    (element as DockedElementsContainer).m_desiredSize.Width = containersize.Width;
                                    foreach (FrameworkElement tabchildren in DockingManager.GetHost(child, DockingManager.GetState(child)).TabChildren)
                                    {
                                        DockingManager.SetDockedElementsContainerDesiredSize(tabchildren, new Size(0, 0));
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                if (containersize.Height > 0)
                                {
                                    (element as DockedElementsContainer).m_desiredSize.Height = containersize.Height;
                                    foreach (FrameworkElement tabchildren in DockingManager.GetHost(child, DockingManager.GetState(child)).TabChildren)
                                    {
                                        DockingManager.SetDockedElementsContainerDesiredSize(tabchildren, new Size(0, 0));
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the size of the container.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void ResetContainerSize(FrameworkElement element)
        {
            if (element is DockedElementsContainer)
            {
                foreach (FrameworkElement child in (element as DockedElementsContainer).Children)
                {
                    if (child is DockedElementTabbedHost && (child as DockedElementTabbedHost).InternalDataContext != null)
                    {
                        DockingManager.SetDockedElementsContainerDesiredSize((child as DockedElementTabbedHost).InternalDataContext, new Size(0, 0));
                    }
                    else if (child is DockedElementsContainer)
                    {
                        ResetContainerSize(child as DockedElementsContainer);
                    }
                }
            }
            else if (element is DockedElementTabbedHost && (element as DockedElementTabbedHost).InternalDataContext != null)
            {
                DockingManager.SetDockedElementsContainerDesiredSize((element as DockedElementTabbedHost).InternalDataContext, new Size(0, 0));
            }
        }

        /// <summary>
        /// Calculates the length of the fixed.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="desired">The desired.</param>
        /// <param name="desiredLength">Length of the desired.</param>
        /// <param name="bIsHorizontalOrientation">The b is horizontal orientation.</param>
        /// <returns></returns>
        private double CalculateFixedLength(DockedElementTabbedHost element, Size desired, double desiredLength, bool bIsHorizontalOrientation)
        {
            DockSide elementside = DockingManager.GetSideInDockedMode(element.InternalDataContext);
            if (element.ActualHeight > 0
                && (elementside == DockSide.Top || elementside == DockSide.Bottom))
            {
                if (DockingManager.GetFixedHeight(element.InternalDataContext) == 0)
                {
                    DockingManager.SetFixedHeight(element.InternalDataContext, desired.Height);
                }
                desired.Height = element.ActualHeight;
            }
            if (element.ActualWidth > 0
                && (elementside == DockSide.Left || elementside == DockSide.Right))
            {
                if (DockingManager.GetFixedWidth(element.InternalDataContext) == 0)
                {
                    DockingManager.SetFixedWidth(element.InternalDataContext, desired.Width);
                }
                desired.Width = element.ActualWidth;
            }

            if (!bIsHorizontalOrientation)
            {
                if (DockingManager.GetFixedHeight(element.InternalDataContext) > 0)
                {
                    desiredLength = DockingManager.GetFixedHeight(element.InternalDataContext);
                }
                else
                {
                    desiredLength = desired.Height;
                }
            }
            else
            {
                if (DockingManager.GetFixedWidth(element.InternalDataContext) > 0)
                {
                    desiredLength = DockingManager.GetFixedWidth(element.InternalDataContext);
                }
                else
                {
                    desiredLength = desired.Width;
                }
            }
            return desiredLength;
        }

        /// <summary>
        /// Checks the absolute mode possible.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private bool CheckAbsoluteModePossible(Orientation orientation, FrameworkElement element)
        {
            DockState actualelementstate = DockingManager.GetState(element);
            double desiredwidth = DockingManager.GetDesiredWidthInDockedMode(element);
            double desiredheight = DockingManager.GetDesiredHeightInDockedMode(element);

            if (actualelementstate == DockState.Dock)
            {
                if ((orientation.Equals(Orientation.Horizontal) && desiredwidth != 90) 
                    || (orientation.Equals(Orientation.Vertical) && desiredheight != 90))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Calculates the edge children resize.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="multiplier">The multiplier.</param>
        /// <param name="m_sharedelementscount">The m_sharedelementscount.</param>
        /// <param name="bIsHorizontalOrientation">if set to <c>true</c> [b is horizontal orientation].</param>
        internal void CalculateEdgeChildrenResize(double multiplier, int m_sharedelementscount, bool bIsHorizontalOrientation)
        {
            double m_sharedlength = 0;
            ObservableFrameworkElements m_resizablecontainerelements = new ObservableFrameworkElements();
            foreach (FrameworkElement element in Children)
            {
                if (element.Visibility != Visibility.Collapsed && !m_resizableelements.Contains(element) && m_cancontainerresize)
                {
                    double actuallength = 0;
                    if (bIsHorizontalOrientation)
                    {
                        if (element is DockedElementTabbedHost)
                        {
                            actuallength = (element as DockedElementTabbedHost).m_desiredSize.Width;
                            (element as DockedElementTabbedHost).m_desiredSize.Width = element.ActualWidth / multiplier;
                        }
                        else if (element is DockedElementsContainer)
                        {
                            actuallength = (element as DockedElementsContainer).m_desiredSize.Width;
                            (element as DockedElementsContainer).m_desiredSize.Width = element.ActualWidth / multiplier;
                        }
                        m_sharedlength += actuallength - (element.ActualWidth / multiplier);
                    }
                    else
                    {
                        if (element is DockedElementTabbedHost)
                        {
                            actuallength = (element as DockedElementTabbedHost).m_desiredSize.Height;
                            (element as DockedElementTabbedHost).m_desiredSize.Height = element.ActualHeight / multiplier;
                        }
                        else if (element is DockedElementsContainer)
                        {
                            actuallength = (element as DockedElementsContainer).m_desiredSize.Height;
                            (element as DockedElementsContainer).m_desiredSize.Height = element.ActualHeight / multiplier;
                        }
                        m_sharedlength += actuallength - (element.ActualHeight / multiplier);
                    }
                }
                if (element is DockedElementsContainer && (element as DockedElementsContainer).m_cancontainerresize)
                {
                    m_resizablecontainerelements.Add(element);
                }
            }

            foreach (DockedElementTabbedHost host in m_resizableelements)
            {
                Size desired = ((IDesiredSize)host).DesiredSize;

                if (bIsHorizontalOrientation)
                {
                    if ((desired.Width + m_sharedlength) > 0)
                        host.m_desiredSize.Width = desired.Width + m_sharedlength;
                }
                else
                {
                    if ((desired.Height + m_sharedlength) > 0)
                        host.m_desiredSize.Height = desired.Height + m_sharedlength;
                }
            }

            foreach (DockedElementsContainer container in m_resizablecontainerelements)
            {
                Size desired = ((IDesiredSize)container).DesiredSize;

                if (bIsHorizontalOrientation)
                {
                    if ((desired.Width + m_sharedlength) > 0)
                        container.m_desiredSize.Width = desired.Width + m_sharedlength;
                }
                else
                {
                    if ((desired.Height + m_sharedlength) > 0)
                        container.m_desiredSize.Height = desired.Height + m_sharedlength;
                }
            }

            m_resizableelements.Clear();
            m_cancontainerresize = false;
        }

        /// <summary>
        /// Calculates the new size.
        /// </summary>
        /// <param name="m_absolutemodehost">The m_absolutemodehost.</param>
        /// <param name="multiplier">The multiplier.</param>
        /// <param name="m_sharedelementscount">The m_sharedelementscount.</param>
        /// <param name="bIsHorizontalOrientation">if set to <c>true</c> [b is horizontal orientation].</param>
        internal void CalculateNewSize(DockedElementTabbedHost host, double multiplier, int m_sharedelementscount, bool bIsHorizontalOrientation)
        {
            double m_sharedlength = 0;
            foreach (FrameworkElement element in Children)
            {
                if (element.Visibility != Visibility.Collapsed && !element.Equals(host))
                {
                    double actuallength = 0;
                    if (bIsHorizontalOrientation)
                    {
                        if (element is DockedElementTabbedHost)
                        {
                            actuallength = (element as DockedElementTabbedHost).m_desiredSize.Width;
                            (element as DockedElementTabbedHost).m_desiredSize.Width = element.ActualWidth / multiplier;
                        }
                        else if (element is DockedElementsContainer)
                        {
                            actuallength = (element as DockedElementsContainer).m_desiredSize.Width;
                            (element as DockedElementsContainer).m_desiredSize.Width = element.ActualWidth / multiplier;
                        }
                        m_sharedlength += actuallength - (element.ActualWidth / multiplier);
                    }
                    else
                    {
                        if (element is DockedElementTabbedHost)
                        {
                            actuallength = (element as DockedElementTabbedHost).m_desiredSize.Height;
                            (element as DockedElementTabbedHost).m_desiredSize.Height = element.ActualHeight / multiplier;
                        }
                        else if (element is DockedElementsContainer)
                        {
                            actuallength = (element as DockedElementsContainer).m_desiredSize.Height;
                            (element as DockedElementsContainer).m_desiredSize.Height = element.ActualHeight / multiplier;
                        }
                        m_sharedlength += actuallength - (element.ActualHeight / multiplier);
                    }
                }
            }

            Size desired = ((IDesiredSize)host).DesiredSize;
            
            if (bIsHorizontalOrientation)
            {
                if ((desired.Width + m_sharedlength) > 0) 
                    host.m_desiredSize.Width = desired.Width + m_sharedlength;
            }
            else
            {
                if ((desired.Height + m_sharedlength) > 0) 
                    host.m_desiredSize.Height = desired.Height + m_sharedlength;
            }
        }

        /// <summary>
        /// Checks the size of the extent.
        /// </summary>
        /// <param name="bIsHorizontalOrientation">The b is horizontal orientation.</param>
        /// <returns></returns>
        internal bool CheckExtentSize(bool bIsHorizontalOrientation)
        {
            double m_absoluteelementssize = 0;
            double m_ownersize = bIsHorizontalOrientation ? DockingManager.ActualWidth : DockingManager.ActualHeight;
            foreach (FrameworkElement element in Children)
            {
                if (element is DockedElementTabbedHost
                    && (element as DockedElementTabbedHost).InternalDataContext != null
                    && DockingManager.GetDockFillMode((element as DockedElementTabbedHost).InternalDataContext) == DockFillModes.Absolute)
                {
                    if (bIsHorizontalOrientation)
                        m_absoluteelementssize += (element as DockedElementTabbedHost).m_desiredSize.Width;
                    else
                        m_absoluteelementssize += (element as DockedElementTabbedHost).m_desiredSize.Height;
                }
            }
            if (m_absoluteelementssize > m_ownersize)
                return false;
            return true;
        }

        /// <summary>
        /// Calculates the size of the absolute.
        /// </summary>
        /// <param name="m_absolutemodehost">The m_absolutemodehost.</param>
        /// <param name="multiplier">The multiplier.</param>
        /// <param name="m_sharedelementscount">The m_sharedelementscount.</param>
        /// <param name="bIsHorizontalOrientation">if set to <c>true</c> [b is horizontal orientation].</param>
        internal void CalculateAbsoluteSize(DockedElementTabbedHost m_absolutemodehost, double multiplier, int m_sharedelementscount, bool bIsHorizontalOrientation)
        {
            if (m_absolutemodehost != null && CheckExtentSize(bIsHorizontalOrientation))
            {
                Size m_absolutedesiredsize = ((IChildrenResize)m_absolutemodehost).DesiredSize;
                double m_sharedlength = 0;
                if (bIsHorizontalOrientation)
                {
                    m_absolutemodehost.m_desiredSize.Width = m_absolutedesiredsize.Width / multiplier;
                    m_sharedlength = (m_absolutedesiredsize.Width * multiplier - m_absolutedesiredsize.Width) / multiplier;
                }
                else
                {
                    m_absolutemodehost.m_desiredSize.Height = m_absolutedesiredsize.Height / multiplier;
                    m_sharedlength = (m_absolutedesiredsize.Height * multiplier - m_absolutedesiredsize.Height) / multiplier;
                }
                if (Children.Count > 2)
                {
                    m_sharedlength = m_sharedlength / (m_sharedelementscount - 1);
                }
                foreach (FrameworkElement element in Children)
                {
                    if (element.Visibility != Visibility.Collapsed && !element.Equals(m_absolutemodehost))
                    {
                        Size desired = ((IDesiredSize)element).DesiredSize;
                        if (bIsHorizontalOrientation)
                        {
                            if (element is DockedElementTabbedHost)
                            {
                                if ((desired.Width + m_sharedlength) > 0)
                                    (element as DockedElementTabbedHost).m_desiredSize.Width = desired.Width + m_sharedlength;
                            }
                            else if (element is DockedElementsContainer)
                            {
                                if ((desired.Width + m_sharedlength) > 0)
                                    (element as DockedElementsContainer).m_desiredSize.Width = desired.Width + m_sharedlength;
                            }
                        }
                        else
                        {
                            if (element is DockedElementTabbedHost)
                            {
                                if ((desired.Height + m_sharedlength) > 0)
                                    (element as DockedElementTabbedHost).m_desiredSize.Height = desired.Height + m_sharedlength;
                            }
                            else if (element is DockedElementsContainer)
                            {
                                if ((desired.Height + m_sharedlength) > 0)
                                    (element as DockedElementsContainer).m_desiredSize.Height = desired.Height + m_sharedlength;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the length of the fixed.
        /// </summary>
        /// <param name="fullLength">The full length.</param>
        /// <param name="fixedLength">Length of the fixed.</param>
        /// <param name="iFixed">The i fixed.</param>
        /// <returns></returns>
        private double CheckFixedLength(double fullLength, double fixedLength,double minElementLenght, int fixedelementscount, int visiblechildrencount)
        {
            //SD12403 has been refixed and committed
            int nonfixedelementscount = visiblechildrencount > fixedelementscount ? visiblechildrencount - fixedelementscount : fixedelementscount - visiblechildrencount;
            if (fixedelementscount > 0 && (nonfixedelementscount > 0 || m_dockedwithpreview) && fullLength - (fixedLength + nonfixedelementscount * minElementLenght) < 0)
            {
                double availablelength = fullLength - fixedLength;
                double neededlength = nonfixedelementscount * minElementLenght;
                if ((neededlength <= availablelength) )
                {
                    fixedLength = fixedLength - (neededlength - availablelength);
                    m_nonassignedfixedlength = (neededlength - availablelength) / fixedelementscount;
                }
                m_dockedwithpreview = false;
            }
            return fixedLength;
        }

        /// <summary>
        /// Calculates the multiplier.
        /// </summary>
        /// <param name="fullLength">The full length.</param>
        /// <param name="previousLength">Length of the previous.</param>
        /// <param name="minElementLenght">The min element lenght.</param>
        /// <param name="usedLength">Length of the used.</param>
        /// <param name="fixedlength">The fixedlength.</param>
        /// <param name="lengthUsedBySplitters">The length used by splitters.</param>
        /// <param name="iNonFixed">The i non fixed.</param>
        /// <returns></returns>
        private double CalculateMultiplier(double fullLength, double previousLength, double minElementLenght, double usedLength, double fixedlength, double lengthUsedBySplitters, int iNonFixed)
        {
            double multiplier = 1;
            
            if (iNonFixed > 0)
            {
                if (usedLength + minElementLenght * iNonFixed + lengthUsedBySplitters > fullLength && usedLength < fullLength)
                {
                    ////bUseScaling = true;
                    if (fixedlength == 0)
                    {
                        multiplier = (fullLength - minElementLenght * iNonFixed - lengthUsedBySplitters) / usedLength;
                    }
                    else
                    {
                        multiplier = (fullLength - fixedlength - minElementLenght * iNonFixed - lengthUsedBySplitters) / ((usedLength > fixedlength) ? (usedLength - fixedlength) : (fixedlength - usedLength));
                    }
                }
                else if (usedLength > 0 && m_multiplier > 0 && previousLength != 0 && previousLength != fullLength && (ValidateManagerSizeChanged() && usedLength > fullLength))
                {
                    double addedsize = (fullLength > previousLength) ? fullLength - previousLength : previousLength - fullLength;
                    double previoususedLength = usedLength * m_multiplier;
                    if (fixedlength == 0)
                    {
                        if (fullLength > previousLength)
                            multiplier = (previoususedLength + (addedsize - (addedsize / (previousLength / (previousLength - previoususedLength))))) / usedLength;
                        else
                            multiplier = (previoususedLength - (addedsize - (addedsize / (previousLength / (previousLength - previoususedLength))))) / usedLength;
                    }
                    else if (usedLength != fixedlength)
                    {
                        if (fullLength > previousLength)
                            multiplier = (previoususedLength - fixedlength + (addedsize - (addedsize / (previousLength / (previousLength - previoususedLength))))) / ((usedLength > fixedlength) ? (usedLength - fixedlength) : (fixedlength - usedLength));
                        else
                            multiplier = (previoususedLength - fixedlength - (addedsize - (addedsize / (previousLength / (previousLength - previoususedLength))))) / ((usedLength > fixedlength) ? (usedLength - fixedlength) : (fixedlength - usedLength));
                    }
                }
            }
            else
            {
                //// bUseScaling = true;
                if (fixedlength == 0)
                {
                    multiplier = (fullLength - lengthUsedBySplitters) / usedLength;
                }
                else
                {
                    if (usedLength != fixedlength)
                    {
                        multiplier = (fullLength - fixedlength - lengthUsedBySplitters) / ((usedLength > fixedlength) ? (usedLength - fixedlength) : (fixedlength - usedLength));
                    }
                }
                multiplier = usedLength == 0 ? 1 : multiplier;
            }
            return multiplier;
        }

        /// <summary>
        /// Validates the manager size changed.
        /// </summary>
        /// <returns></returns>
        private bool ValidateManagerSizeChanged()
        {
            DockingManager parentDockingManager = VisualUtils.FindAncestor((Visual)DockingManager, typeof(DockingManager)) as DockingManager;
            if (parentDockingManager == null && DockingManager.IsLoaded && !BrowserInteropHelper.IsBrowserHosted 
                && !DockingManager.m_loadingState && Window.GetWindow(DockingManager)!=null )
                return DockingManager.IsSizeChanging;
            return false;
        }

        /// <summary>
        /// Positions child elements and determines a size for a DockedElementsContainer and derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange
        /// itself and its children.</param>
        /// <returns> The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int iChildren = Children.Count;
            int iNonFixed = 0;
            int fixedelementscount = 0;
            double usedLength = 0;
            double fixedlength = 0;
            double multiplier = 1;
            ////bool bUseScaling = false;
            Splitter splitterLast = null;
            bool bIsHorizontalOrientation = Orientation == Orientation.Horizontal;
            WindowState m_state = WindowState.Normal;
            bool m_restrictstatechange = false;
            DockedElementTabbedHost m_absolutemodehost = null;
            int m_sharedelementscount = 0;
            bool hasPriorityControl = false;
            Size prioritySize = Size.Empty;

            try
            {
                foreach (FrameworkElement element in Children)
                {
                    if (element.Visibility != Visibility.Collapsed)
                    {
                        m_sharedelementscount++;
                        CheckContainerSize(element, bIsHorizontalOrientation);
                        Size desired = ((IDesiredSize)element).DesiredSize;
                        double desiredLength = bIsHorizontalOrientation ? desired.Width : desired.Height;
                        usedLength += desiredLength;

                        if ((element as DockedElementTabbedHost) != null && (element as DockedElementTabbedHost).InternalDataContext != null && !m_restrictstatechange)
                        {
                            m_state = DockingManager.GetDockWindowState((element as DockedElementTabbedHost).InternalDataContext as DependencyObject);
                            if (m_state == WindowState.Maximized)
                            {
                                m_restrictstatechange = true;
                            }
                        }

                        #region FixedSizeComponent Calculations

                        if (this.DockingManager != null && element is DockedElementTabbedHost)
                        {
                            DockedElementTabbedHost elementhost = (element as DockedElementTabbedHost);
                            if (elementhost.InternalDataContext != null
                                && DockingManager.CheckFixedsize(elementhost.InternalDataContext, Orientation))
                            {
                                DockState elementstate = DockingManager.GetState(elementhost.InternalDataContext);
                                if (elementstate == DockState.Dock)
                                {
                                    if (!DockingManager.CheckResize(elementhost.InternalDataContext, DockState.Dock, Orientation))
                                    {
                                        double elementfixedLength = CalculateFixedLength(elementhost, desired, desiredLength, bIsHorizontalOrientation);
                                        fixedlength += elementfixedLength;
                                        usedLength -= desiredLength;
                                        usedLength += elementfixedLength;
                                        fixedelementscount++;
                                    }
                                }
                            }
                        }

                        #endregion

                        #region Absolute DockFillMode Calculations

                        if (element is DockedElementTabbedHost
                                && !(element as DockedElementTabbedHost).IsLoaded
                                && !(element as DockedElementTabbedHost).m_isAbsoluteSizeUpdated
                                && (element as DockedElementTabbedHost).InternalDataContext != null
                                && this.DockingManager != null && this.DockingManager.Children.Count > 1
                                && DockingManager.GetDockFillMode((element as DockedElementTabbedHost).InternalDataContext) == DockFillModes.Absolute
                                && CheckAbsoluteModePossible(Orientation, (element as DockedElementTabbedHost).InternalDataContext))
                        {
                            m_absolutemodehost = (DockedElementTabbedHost)element;
                            (element as DockedElementTabbedHost).m_isAbsoluteSizeUpdated = true;
                        }

                        #endregion

                        if (desiredLength == 0)
                        {
                            ++iNonFixed;
                        }

                        if (element is DockedElementTabbedHost && (element as DockedElementTabbedHost).isPriorityControl)
                        {
                            hasPriorityControl = true;
                            prioritySize = ((IDesiredSize)element).DesiredSize;
                        }
                    }
                    else
                    {
                        --iChildren;
                    }
                }

                //MT2247, MT2268 has been fixed. Skipped the State variable (m_state) in Full Screen mode. The m_state is required only in Default mode. -Jawahar
                if (this.DockingManager.MaximizeMode == MaximizeMode.Default && m_state == WindowState.Maximized)
                {
                    iNonFixed = 0;
                    m_restrictstatechange = false;
                }

                double fullLength = bIsHorizontalOrientation ? finalSize.Width : finalSize.Height;
                double previousLength = bIsHorizontalOrientation ? m_previousSize.Width : m_previousSize.Height;
                double lengthUsedBySplitters = GetSplittersDesiredSizeWidth(iChildren);
                double minElementLenght = bIsHorizontalOrientation ? MinElementWidth : MinElementHeight;

                #region multiplier Calculations

                fixedlength = CheckFixedLength(fullLength, fixedlength, minElementLenght, fixedelementscount, m_sharedelementscount);

                if (hasPriorityControl)
                {
                    //SD12513- This code has been added to fix the preview size which happened when we dock globally. 
                    //Issue: The compoenent will be docked to the size larger than the preview size

                    double sizeToSubtract = (bIsHorizontalOrientation ? prioritySize.Width : prioritySize.Height);
                    fullLength = fullLength - sizeToSubtract;
                    usedLength = usedLength - sizeToSubtract;
                }

                multiplier = CalculateMultiplier(fullLength, previousLength, minElementLenght, usedLength, fixedlength, lengthUsedBySplitters, iNonFixed);
                #endregion

                if (DockingManager.ContainerSplitterResize != SplitterResizeMode.EdgeChildren)
                {
                    if (m_actualhostresize != null && multiplier > 0)
                        CalculateNewSize(m_actualhostresize, multiplier, m_sharedelementscount, bIsHorizontalOrientation);
                }
                else if (multiplier > 0)
                {
                    CalculateEdgeChildrenResize(multiplier, m_sharedelementscount, bIsHorizontalOrientation);
                }
                #region Absolute Mode Calculations
                if (multiplier > 0)
                {
                    CalculateAbsoluteSize(m_absolutemodehost, multiplier, m_sharedelementscount, bIsHorizontalOrientation);
                }
                #endregion

                m_multiplier = multiplier;
                m_actualhostresize = null;
                double valueOnPrimarySide = 0;
                int iSplitter = 0;
                double lengthFinal = 0;
                double usedLengthInverted = !bIsHorizontalOrientation
                    ? finalSize.Width : finalSize.Height;
                FrameworkElement m_focusableelement = null;

                foreach (FrameworkElement element in Children)
                {
                    if (element.Visibility != Visibility.Collapsed)
                    {
                        Size desired = ((IDesiredSize)element).DesiredSize;

                        #region FixedSizeComponent Calculations

                        if (this.DockingManager != null && element is DockedElementTabbedHost)
                        {
                            FrameworkElement actualelement1 = (element as DockedElementTabbedHost).InternalDataContext as FrameworkElement;
                            DockedElementTabbedHost host = element as DockedElementTabbedHost;
                            if (actualelement1 != null && DockingManager.CheckFixedsize(actualelement1, Orientation))
                            {
                                DockState actualelementstate1 = DockingManager.GetState(host.InternalDataContext);
                                DockSide actualelementside = DockingManager.GetSideInDockedMode(actualelement1);
                                if (actualelementstate1 == DockState.Dock)
                                {
                                    if (!DockingManager.CheckResize(host.InternalDataContext, DockState.Dock, Orientation))
                                    {
                                        if ((element as DockedElementTabbedHost).ActualHeight > 0
                                            && (actualelementside == DockSide.Top || actualelementside == DockSide.Bottom))
                                        {
                                            desired.Height = host.ActualHeight;
                                        }
                                        if ((element as DockedElementTabbedHost).ActualWidth > 0
                                            && (actualelementside == DockSide.Left || actualelementside == DockSide.Right))
                                        {
                                            desired.Width = host.ActualWidth;
                                        }
                                        if (!bIsHorizontalOrientation)
                                        {
                                            if (DockingManager.GetFixedHeight(actualelement1 as DependencyObject) > 0)
                                            {
                                                desired.Height = DockingManager.GetFixedHeight(actualelement1 as DependencyObject);
                                            }
                                        }
                                        else
                                        {
                                            if (DockingManager.GetFixedWidth(actualelement1 as DependencyObject) > 0)
                                            {
                                                desired.Width = DockingManager.GetFixedWidth(actualelement1 as DependencyObject);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        double desiredLength = bIsHorizontalOrientation ? desired.Width : desired.Height;
                        bool bFixed = desiredLength != 0;
                        bool executesizechange = false;

                        if (bFixed)
                            lengthFinal = desiredLength;
                        else
                        {
                            if (previousLength != 0 && previousLength != fullLength && (ValidateManagerSizeChanged() || usedLength > fullLength))
                            {
                                if (fixedlength == 0 || usedLength == fixedlength)
                                {
                                    if (((fullLength - usedLength * multiplier - lengthUsedBySplitters) / iNonFixed) / multiplier >= 0)
                                        lengthFinal = ((fullLength - usedLength * multiplier - lengthUsedBySplitters) / iNonFixed) / multiplier;
                                }
                                else
                                {
                                    if (((fullLength - (usedLength - fixedlength) * multiplier - fixedlength - lengthUsedBySplitters) / iNonFixed) / multiplier >= 0)
                                        lengthFinal = ((fullLength - (usedLength - fixedlength) * multiplier - fixedlength - lengthUsedBySplitters) / iNonFixed) / multiplier;
                                }
                                executesizechange = true;
                            }
                            else
                            {
                                if ((fullLength - usedLength - lengthUsedBySplitters) / iNonFixed >= 0)
                                    lengthFinal = (fullLength - usedLength - lengthUsedBySplitters) / iNonFixed;
                            }
                        }

                        //SD12513- This following condition has been added to fix the preview size issue which happened when we dock globally. 
                        //Issue: The compoenent will be docked to the size larger than the preview size

                        if (!(element is DockedElementTabbedHost && (element as DockedElementTabbedHost).isPriorityControl))
                            lengthFinal *= multiplier;

                        lengthFinal = (lengthFinal < minElementLenght) ? minElementLenght : lengthFinal;

                        //MT2247, MT2268 has been fixed. Skipped the State variable (m_state) in Full Screen mode. The m_state is required only in Default mode. -Jawahar
                        if (this.DockingManager.MaximizeMode == MaximizeMode.Default && m_state == WindowState.Maximized && !bFixed)
                        {
                            lengthFinal = desiredLength;
                        }

                        #region FixedSizeComponent Calculations

                        if (this.DockingManager != null && element is DockedElementTabbedHost)
                        {
                            FrameworkElement actualelement = (element as DockedElementTabbedHost).InternalDataContext as FrameworkElement;

                            if (actualelement != null && bFixed && DockingManager.CheckFixedsize(actualelement, Orientation))
                            {
                                DockState actualelementstate = DockingManager.GetState((element as DockedElementTabbedHost).InternalDataContext);
                                if (actualelementstate == DockState.Dock)
                                {
                                    if (!DockingManager.CheckResize((element as DockedElementTabbedHost).InternalDataContext, DockState.Dock, Orientation))
                                    {
                                        if (m_nonassignedfixedlength > 0)
                                        {
                                            if (desiredLength - m_nonassignedfixedlength >= 0)
                                                lengthFinal = desiredLength - m_nonassignedfixedlength;

                                            //SD12403 has been refixed and committed

                                            if (bIsHorizontalOrientation)
                                                DockingManager.SetFixedWidth((element as DockedElementTabbedHost).InternalDataContext, lengthFinal);
                                            else
                                                DockingManager.SetFixedHeight((element as DockedElementTabbedHost).InternalDataContext, lengthFinal);
                                        }
                                        else
                                            lengthFinal = desiredLength;
                                    }
                                }
                            }
                        }

                        #endregion

                        if (!double.IsInfinity(lengthFinal) && !double.IsInfinity(usedLengthInverted))
                        {
                            if (lengthFinal < 0)
                                lengthFinal = 1;

                            if (previousLength != 0 && executesizechange)
                            {
                                if (element is DockedElementTabbedHost)
                                    (element as DockedElementTabbedHost).m_desiredSize = bIsHorizontalOrientation && (element as DockedElementTabbedHost).m_desiredSize.Width == 90 ? new Size(lengthFinal, (element as DockedElementTabbedHost).m_desiredSize.Height)
                                        : (element as DockedElementTabbedHost).m_desiredSize.Height == 90 ? new Size((element as DockedElementTabbedHost).m_desiredSize.Width, lengthFinal) : (element as DockedElementTabbedHost).m_desiredSize;
                                if (element is DockedElementsContainer)
                                {
                                    (element as DockedElementsContainer).m_desiredSize = bIsHorizontalOrientation && (element as DockedElementsContainer).m_desiredSize.Width == 0 ? new Size(lengthFinal, (element as DockedElementsContainer).m_desiredSize.Height)
                                        : (element as DockedElementsContainer).m_desiredSize.Height == 0 ? new Size((element as DockedElementsContainer).m_desiredSize.Width, lengthFinal) : (element as DockedElementsContainer).m_desiredSize;
                                    foreach (FrameworkElement child in (element as DockedElementsContainer).Children)
                                    {
                                        if (child is DockedElementTabbedHost && (child as DockedElementTabbedHost).InternalDataContext != null)
                                            DockingManager.SetDockedElementsContainerDesiredSize((child as DockedElementTabbedHost).InternalDataContext, (element as DockedElementsContainer).m_desiredSize);
                                    }
                                }
                            }

                            ArrangeElement(element, valueOnPrimarySide, usedLengthInverted, lengthFinal, Orientation);

                            //SD12513- This following condition have been added to fix the preview size issue which happened when we dock globally. 
                            //Issue: The compoenent will be docked to the size larger than the preview size

                            if (hasPriorityControl)
                            {
                                if (Orientation == Orientation.Horizontal)
                                {
                                    ((IChildrenResize)element).SetWidth(lengthFinal);
                                    // ((IChildrenResize)element).SetHeight(usedLengthInverted);
                                }
                                else
                                {
                                    // ((IChildrenResize)element).SetWidth(usedLengthInverted);
                                    ((IChildrenResize)element).SetHeight(lengthFinal);
                                }
                                if (element is DockedElementTabbedHost && (element as DockedElementTabbedHost).isPriorityControl)
                                {
                                    (element as DockedElementTabbedHost).isPriorityControl = false;
                                }
                            }
                        }
                        valueOnPrimarySide += lengthFinal;

                        if (m_state == WindowState.Maximized)
                        {
                            if (lengthFinal != 0)
                            {
                                m_focusableelement = element;
                                Keyboard.Focus(m_focusableelement);
                            }
                            else
                            {
                                if (m_focusableelement != null)
                                {
                                    Keyboard.Focus(m_focusableelement);
                                }
                            }
                        }

                        if (splitterLast != null)
                        {
                            if (m_state == WindowState.Maximized)
                            {
                                if (lengthFinal == 0)
                                {
                                    splitterLast.Visibility = Visibility.Collapsed;
                                }
                                else
                                {
                                    splitterLast.Visibility = Visibility.Visible;
                                }
                            }
                            DockedElementsContainer.SetElementAfterSplitter(splitterLast, element);
                            splitterLast.MaxOffsetRight = Math.Max(0, lengthFinal - GetMaxOffset(element, splitterLast.Orientation));
                        }

                        if (iSplitter < m_Splitters.Count)
                        {
                            Splitter splitter = m_Splitters[iSplitter++];
                            splitter.MaxOffsetLeft = Math.Max(0, lengthFinal - GetMaxOffset(element, splitter.Orientation));

                            ////SplitterTargetSide sideSplitter = SplitterTargetSide.Both;

                            DockedElementsContainer.SetElementBeforeSplitter(splitter, element);
                            valueOnPrimarySide = ArrangeSplitter(splitter, valueOnPrimarySide, usedLengthInverted, Orientation);

                            if (m_state != WindowState.Maximized)
                            {
                                splitter.Visibility = Visibility.Visible;
                            }
                            splitterLast = splitter;
                        }
                    }

                    // MT 2256 - Arrange the splitters which was missed while arrangement..
                    int index = Children.IndexOf(element);
                    if (index == Children.Count - 1 && lengthFinal > 0)
                    {
                        for (int j = iSplitter; j < m_Splitters.Count; j++)
                        {
                            Splitter splitter = m_Splitters[j];
                            splitter.MaxOffsetLeft = Math.Max(0, lengthFinal - minElementLenght);
                            ArrangeSplitter(splitter, valueOnPrimarySide, usedLengthInverted, Orientation);
                        }
                    }
                }
                m_previousSize = finalSize;
                m_nonassignedfixedlength = 0;
            }
            catch { }

            return finalSize;
        }

        /// <summary>
        /// Gets the number of child System.Windows.Media.Visual objects in this instance of DockedElementsContainer.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                int countBase = Children.Count;
                return (countBase > 1) ? (countBase * 2 - 1) : countBase;
            }
        }

        /// <summary>
        /// Gets the width of the splitters desired size.
        /// </summary>
        /// <param name="iChildren">The i children.</param>
        /// <returns>return double value</returns>
        private double GetSplittersDesiredSizeWidth(int iChildren)
        {
            double result = 0;
            bool bIsHorizontalOrientation = Orientation == Orientation.Horizontal;

            for (int iSplitter = 0, cnt = iChildren - 1; iSplitter < cnt; ++iSplitter)
            {
                Splitter splitter = m_Splitters[iSplitter];

                result += bIsHorizontalOrientation
                    ? splitter.DesiredSize.Width : splitter.DesiredSize.Height;
            }

            return result;
        }

        /// <summary>
        /// Checks the max splitter offset.
        /// </summary>
        /// <param name="elementBefore">The element before.</param>
        /// <param name="elementAfter">The element after.</param>
        /// <param name="splitter">The splitter.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        internal double CheckMaxSplitterOffset(FrameworkElement elementBefore, FrameworkElement elementAfter, Splitter splitter, double offset)
        {
            double maxoffsetright = 0;
            double maxoffsetleft = 0;

            if (elementBefore != null)
                maxoffsetleft = GetMaxOffset(elementBefore, splitter.Orientation);
            if (elementAfter != null)
                maxoffsetright = GetMaxOffset(elementAfter, splitter.Orientation);

            if (offset > 0 && maxoffsetright > 0)
            {
                splitter.MaxOffsetRight = splitter.Orientation == System.Windows.Controls.Orientation.Vertical ? elementAfter.ActualWidth - maxoffsetright : elementAfter.ActualHeight - maxoffsetright;
                if (offset > splitter.MaxOffsetRight)
                    offset = splitter.MaxOffsetRight;
            }
            else if (maxoffsetleft > 0)
            {
                splitter.MaxOffsetLeft = splitter.Orientation == System.Windows.Controls.Orientation.Vertical ? elementBefore.ActualWidth - maxoffsetleft : elementBefore.ActualHeight - maxoffsetleft;
                if (-offset > splitter.MaxOffsetLeft)
                    offset = -splitter.MaxOffsetLeft;
            }

            return offset;
        }

        /// <summary>
        /// Checks the container eligible.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns></returns>
        private bool CheckContainerEligible(FrameworkElement element, Orientation orientation)
        {
            bool m_canexecute = true;

            DockedElementsContainer lastVisibleChildren, firstVisibleChildren;
            lastVisibleChildren = GetLastVisibleChildren(element as DockedElementsContainer) as DockedElementsContainer;
            firstVisibleChildren = GetFirstVisibleChildren(element as DockedElementsContainer) as DockedElementsContainer;

            if (lastVisibleChildren != null && firstVisibleChildren != null)
            {
                if (lastVisibleChildren.Equals(firstVisibleChildren))
                    return false;
            }
            foreach (FrameworkElement child in (element as DockedElementsContainer).Children)
            {
                if (child is DockedElementTabbedHost && child.Visibility == Visibility.Visible)
                {
                    FrameworkElement internalelement = (child as DockedElementTabbedHost).InternalDataContext;
                    if (internalelement != null)
                    {
                        DockSide side = DockingManager.GetSideInDockedMode(internalelement);
                        DockState state = DockingManager.GetState(internalelement);
                        if (state == DockState.Dock)
                        {
                            switch (orientation)
                            {
                                case Orientation.Horizontal:
                                    if (side == DockSide.Top || side == DockSide.Bottom)
                                        return false;
                                    break;
                                case Orientation.Vertical:
                                    if (side == DockSide.Left || side == DockSide.Right)
                                        return false;
                                    break;
                            }
                        }
                    }
                }
                else if (child is DockedElementsContainer)
                {
                    m_canexecute = CheckContainerEligible(child, orientation);
                    if (!m_canexecute)
                        return false;
                }
            }
            return m_canexecute;
        }

        /// <summary>
        /// Gets the last visible children.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        private FrameworkElement GetLastVisibleChildren(DockedElementsContainer container)
        {
            for (int i = container.Children.Count - 1; i >= 0; i--)
            {
                if (container.Children[i].Visibility == Visibility.Visible)
                    return container.Children[i] as FrameworkElement;
            }
            return null;
        }

        /// <summary>
        /// Gets the first visible children.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        private FrameworkElement GetFirstVisibleChildren(DockedElementsContainer container)
        {
            for (int i = 0; i < container.Children.Count; i++)
            {
                if (container.Children[i].Visibility == Visibility.Visible)
                    return container.Children[i] as FrameworkElement;
            }
            return null;
        }

        /// <summary>
        /// Checks the elements resize disabling.
        /// </summary>
        /// <param name="element">The element.</param>
        private double CheckElementsCompleteResizeDisabling(FrameworkElement element,bool beforesplitter,Orientation orientation,double offset)
        {
            if (element is DockedElementsContainer)
            {
                DockedElementsContainer container = element as DockedElementsContainer;
                bool m_canexecute = CheckContainerEligible(element, orientation);
                if (m_canexecute)
                {
                    if (beforesplitter)
                    {
                        FrameworkElement lastelement = GetLastVisibleChildren(container);
                        if (lastelement is DockedElementsContainer)
                            return CheckElementsCompleteResizeDisabling(lastelement as FrameworkElement, beforesplitter, orientation,offset);
                        else if (lastelement is DockedElementTabbedHost)
                        {
                            (element as DockedElementsContainer).m_actualhostresize = lastelement as DockedElementTabbedHost;
                            return GetPossibleOffset((element as DockedElementsContainer).m_actualhostresize, beforesplitter, orientation, offset);
                        }
                    }
                    else
                    {
                        FrameworkElement firstelement = GetFirstVisibleChildren(container);
                        if (firstelement is DockedElementsContainer)
                            return CheckElementsCompleteResizeDisabling(firstelement as FrameworkElement, beforesplitter, orientation,offset);
                        else if (firstelement is DockedElementTabbedHost)
                        {
                            (element as DockedElementsContainer).m_actualhostresize = firstelement as DockedElementTabbedHost;
                            return GetPossibleOffset((element as DockedElementsContainer).m_actualhostresize, beforesplitter, orientation, offset);
                        }
                    }
                }
            }
            return offset;
        }

        /// <summary>
        /// Gets the possible offset.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="beforesplitter">if set to <c>true</c> [beforesplitter].</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private double GetPossibleOffset(DockedElementTabbedHost host, bool beforesplitter,Orientation orientation,double offset)
        {
            double desiredlength = orientation == Orientation.Horizontal ? host.ActualWidth : host.ActualHeight;
            double minElementLength = (orientation == Orientation.Horizontal) ? MinElementWidth : MinElementHeight;
            double neededLength = 0;

            if (beforesplitter)
                neededLength = desiredlength + offset;
            else
                neededLength = desiredlength - offset;

            if (neededLength < minElementLength)
                return offset > 0 ? desiredlength - minElementLength : minElementLength - desiredlength;

            return offset;
        }

        /// <summary>
        /// Checks the edge children.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="beforesplitter">if set to <c>true</c> [beforesplitter].</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private double CheckEdgeChildren(FrameworkElement element, bool beforesplitter, Orientation orientation, double offset,Splitter splitter)
        {
            double m_finaloffset = offset;
            if (element is DockedElementsContainer)
            {
                Rect m_splitterboundingrect = DockingManager.GetElementBoundaryRect(splitter, this);
                DockedElementsContainer container = element as DockedElementsContainer;
                foreach (FrameworkElement child in container.Children)
                {
                    if (child is DockedElementTabbedHost && child.Visibility == Visibility.Visible)
                    {
                        Rect m_elementboundingrect = DockingManager.GetElementBoundaryRect(child, this);
                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                if (beforesplitter)
                                {
                                    if (Math.Round(m_elementboundingrect.Right).Equals(Math.Round(m_splitterboundingrect.X)))
                                    {
                                        container.m_cancontainerresize = true;
                                        container.m_resizableelements.Add(child);
                                        m_finaloffset = GetPossibleOffset(child as DockedElementTabbedHost, beforesplitter, orientation, m_finaloffset);
                                    }
                                }
                                else
                                {
                                    if (Math.Round(m_elementboundingrect.Left).Equals(Math.Round(m_splitterboundingrect.X + DockingManager.SplitterSize))) 
                                    {
                                        container.m_cancontainerresize = true;
                                        container.m_resizableelements.Add(child);
                                        m_finaloffset = GetPossibleOffset(child as DockedElementTabbedHost, beforesplitter, orientation, m_finaloffset);
                                    }
                                }
                                break;
                            case Orientation.Vertical:
                                if (beforesplitter)
                                {
                                    if (Math.Round(m_elementboundingrect.Bottom).Equals(Math.Round(m_splitterboundingrect.Y)))
                                    {
                                        container.m_cancontainerresize = true;
                                        container.m_resizableelements.Add(child);
                                        m_finaloffset = GetPossibleOffset(child as DockedElementTabbedHost, beforesplitter, orientation, m_finaloffset);
                                    }
                                }
                                else
                                {
                                    if (Math.Round(m_elementboundingrect.Top).Equals(Math.Round(m_splitterboundingrect.Y + DockingManager.SplitterSize))) 
                                    {
                                        container.m_cancontainerresize = true;
                                        container.m_resizableelements.Add(child);
                                        m_finaloffset = GetPossibleOffset(child as DockedElementTabbedHost, beforesplitter, orientation, m_finaloffset);
                                    }
                                }
                                break;
                        }
                    }
                    else if (child is DockedElementsContainer)
                    {
                        m_finaloffset = CheckEdgeChildren(child, beforesplitter, orientation, m_finaloffset, splitter);
                        if (!container.m_cancontainerresize)
                            container.m_cancontainerresize = (child as DockedElementsContainer).m_cancontainerresize;
                    }
                }
            }
            return m_finaloffset;
        }

        /// <summary>
        /// Called when [SPLT offset changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSpltOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double offset = (double)e.NewValue;

            if (offset != 0)
            {
                bool canresize=true;
                Splitter splitter = (Splitter)d;
                FrameworkElement elementBefore = GetElementBeforeSplitter(splitter);
                IChildrenResize resizerBefore = (IChildrenResize)elementBefore;

                FrameworkElement elementAfter = GetElementAfterSplitter(splitter);
                IChildrenResize resizerAfter = (IChildrenResize)elementAfter;

                Size renderSizeBefore = Size.Empty;
                Size renderSizeAfter = Size.Empty;

                offset = CheckMaxSplitterOffset(elementBefore, elementAfter, d as Splitter, offset);

                if(elementBefore != null)
                    renderSizeBefore = elementBefore.RenderSize;
                if(elementAfter != null)
                    renderSizeAfter = elementAfter.RenderSize;

                DockedElementTabbedHost actualhostbefore = null;
                DockedElementTabbedHost actualhostafter = null;

                Rect m_splitterboundingrect = DockingManager.GetElementBoundaryRect(splitter, this);

                #region CanResize Feature Calculations

                if ((elementBefore as DockedElementsContainer) != null)
                {
                    actualhostbefore = IterateContainerItems((elementBefore as DockedElementsContainer), renderSizeBefore, m_splitterboundingrect, true, Orientation);
                    if (actualhostbefore != null && actualhostbefore.HostedElement != null)
                    {
                        if (!DockingManager.CheckResize(actualhostbefore.HostedElement, DockState.Dock, Orientation)) 
                        {
                            canresize = false;
                        }
                    }
                }
                else if ((elementBefore as DockedElementTabbedHost) != null)
                {
                    actualhostbefore = (elementBefore as DockedElementTabbedHost);
                    if (actualhostbefore != null && actualhostbefore.HostedElement!=null)
                    {
                        if (!DockingManager.CheckResize(actualhostbefore.HostedElement, DockState.Dock, Orientation)) 
                        {
                            canresize = false;
                        }
                    }

                }

                if ((elementAfter as DockedElementsContainer) != null)
                {
                    actualhostafter = IterateContainerItems((elementAfter as DockedElementsContainer), renderSizeAfter, m_splitterboundingrect, false, Orientation);
                    if (actualhostafter != null && actualhostafter.HostedElement != null)
                    {
                        if (!DockingManager.CheckResize(actualhostafter.HostedElement, DockState.Dock, Orientation)) 
                        {
                            canresize = false;
                        }
                    }
                }
                else if ((elementAfter as DockedElementTabbedHost) != null)
                {
                    actualhostafter = (elementAfter as DockedElementTabbedHost);
                    if (actualhostafter != null && actualhostafter.HostedElement!=null)
                    {
                        if (!DockingManager.CheckResize(actualhostafter.HostedElement, DockState.Dock, Orientation)) 
                        {
                            canresize = false;
                        }
                    }
                }
                //var dockedElementTabbedHost = elementAfter as DockedElementTabbedHost;
                //var elementTabbedHost = elementBefore as DockedElementTabbedHost;
                //if (dockedElementTabbedHost != null && dockedElementTabbedHost.HostedElement != null
                //&& elementTabbedHost != null && elementTabbedHost.HostedElement != null
                //&& DockingManager.GetDockWindowState(dockedElementTabbedHost.HostedElement) == WindowState.Minimized
                //&& DockingManager.GetDockWindowState(elementTabbedHost.HostedElement) == WindowState.Minimized)
                //    canresize = true;
                //else
                //    canresize = false;
                #region To check if Maximized
                var elementAfterState = WindowState.Normal;
                var elementBeforeState = WindowState.Normal;
                var dockedElementTabbedHost = elementAfter as DockedElementTabbedHost;
                if (dockedElementTabbedHost != null)
                {
                    if (dockedElementTabbedHost.HostedElement != null)
                    {
                        elementAfterState = DockingManager.GetDockWindowState(dockedElementTabbedHost.HostedElement);
                    }
                }

                var elementTabbedHost = elementBefore as DockedElementTabbedHost;
                if (elementTabbedHost != null)
                    if (elementTabbedHost.HostedElement != null)
                        elementBeforeState = DockingManager.GetDockWindowState(elementTabbedHost.HostedElement);

                if (elementAfterState == WindowState.Maximized || elementBeforeState == WindowState.Maximized)
                {
                    canresize = false;
                }
                
                #endregion
                #endregion

                if (canresize)
                {
                    ResetContainerSize(resizerBefore as FrameworkElement);
                    ResetContainerSize(resizerAfter as FrameworkElement);
                    if (DockingManager.ContainerSplitterResize.Equals(SplitterResizeMode.EdgeChildren))
                    {
                        offset = CheckEdgeChildren(elementBefore, true, Orientation, offset, splitter);
                        offset = CheckEdgeChildren(elementAfter, false, Orientation, offset, splitter);
                    }
                    else
                    {
                        offset = CheckElementsCompleteResizeDisabling(elementBefore, true, Orientation,offset);
                        offset = CheckElementsCompleteResizeDisabling(elementAfter, false, Orientation,offset);
                    }
                    ResizeElements(resizerBefore, resizerAfter, offset, m_splitterboundingrect);

                    WindowResizingEventArgs beforeargs = new WindowResizingEventArgs();
                    if (actualhostbefore!=null && actualhostbefore.m_desiredSize != new Size(0, 0))
                    {
                        beforeargs.DesiredHeight = actualhostbefore.m_desiredSize.Height;
                        beforeargs.DesiredWidth = actualhostbefore.m_desiredSize.Width;
                        beforeargs.State = DockState.Dock;
                        DockingManager.FireWindowResizingEvent(actualhostbefore, beforeargs);
                    }
                    WindowResizingEventArgs afterargs = new WindowResizingEventArgs();
                    if (actualhostafter!=null && actualhostafter.m_desiredSize != new Size(0, 0))
                    {
                        afterargs.DesiredHeight = actualhostafter.m_desiredSize.Height;
                        afterargs.DesiredWidth = actualhostafter.m_desiredSize.Width;
                        afterargs.State = DockState.Dock;
                        DockingManager.FireWindowResizingEvent(actualhostafter, afterargs);
                    }
                }
            }
        }

        /// <summary>
        /// Iterates the container items.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="rendersize">The rendersize.</param>
        /// <returns></returns>
        private DockedElementTabbedHost IterateContainerItems(DockedElementsContainer container, Size rendersize, Rect m_splitterboundingrect, bool beforesplitter, Orientation orientation)
        {
            for (int i = 0; i < container.Children.Count; i++)
            {
                DockedElementsContainer container1 = container.Children[i] as DockedElementsContainer;
                if (container1 != null)
                {
                    return IterateContainerItems(container1, rendersize, m_splitterboundingrect, beforesplitter, orientation);
                }
                else if(container.Children[i] is DockedElementTabbedHost)
                {
                    Rect m_elementboundingrect = DockingManager.GetElementBoundaryRect(container.Children[i] as FrameworkElement, this);
                    switch (orientation)
                    {
                        case Orientation.Horizontal:
                            if (beforesplitter)
                            {
                                if (Math.Round(m_elementboundingrect.Right).Equals(Math.Round(m_splitterboundingrect.X)))
                                    return container.Children[i] as DockedElementTabbedHost;
                            }
                            else
                            {
                                if (Math.Round(m_elementboundingrect.Left).Equals(Math.Round(m_splitterboundingrect.X + DockingManager.SplitterSize)))
                                    return container.Children[i] as DockedElementTabbedHost;
                            }
                            break;
                        case Orientation.Vertical:
                            if (beforesplitter)
                            {
                                if (Math.Round(m_elementboundingrect.Bottom).Equals(Math.Round(m_splitterboundingrect.Y)))
                                    return container.Children[i] as DockedElementTabbedHost;
                            }
                            else
                            {
                                if (Math.Round(m_elementboundingrect.Top).Equals(Math.Round(m_splitterboundingrect.Y + DockingManager.SplitterSize)))
                                    return container.Children[i] as DockedElementTabbedHost;
                            }
                            break;
                    }
                }
            }
            return null;
        }


        private ObservableCollection<DockedElementTabbedHost> IterateDocumentContainerItems(DockedElementsContainer container, Size rendersize, Rect m_splitterboundingrect, bool beforesplitter, Orientation orientation)
        {
            ObservableCollection<DockedElementTabbedHost> _container = new ObservableCollection<DockedElementTabbedHost>();
            for (int i = 0; i < container.Children.Count; i++)
            {
                DockedElementsContainer container1 = container.Children[i] as DockedElementsContainer;
                if (container1 != null)
                {
                    return IterateDocumentContainerItems(container1, rendersize, m_splitterboundingrect, beforesplitter, orientation);
                }
                else if (container.Children[i] is DockedElementTabbedHost)
                {
                    Rect m_elementboundingrect = DockingManager.GetElementBoundaryRect(container.Children[i] as FrameworkElement, this);
                    switch (orientation)
                    {
                        case Orientation.Horizontal:
                            if (beforesplitter)
                            {
                                if (Math.Round(m_elementboundingrect.Right).Equals(Math.Round(m_splitterboundingrect.X)))
                                    _container.Add(container.Children[i] as DockedElementTabbedHost);
                                
                            }
                            else
                            {
                                if (Math.Round(m_elementboundingrect.Left).Equals(Math.Round(m_splitterboundingrect.X + DockingManager.SplitterSize)))
                                    _container.Add(container.Children[i] as DockedElementTabbedHost);
                  
                            }
                            break;
                        case Orientation.Vertical:
                            if (beforesplitter)
                            {
                                if (Math.Round(m_elementboundingrect.Bottom).Equals(Math.Round(m_splitterboundingrect.Y)))
                                    _container.Add(container.Children[i] as DockedElementTabbedHost);
                         
                            }
                            else
                            {
                                if (Math.Round(m_elementboundingrect.Top).Equals(Math.Round(m_splitterboundingrect.Y + DockingManager.SplitterSize)))
                                    _container.Add(container.Children[i] as DockedElementTabbedHost);
                           
                            }
                            break;
                    }
                }
                
            }
            return _container;
        }

        /// <summary>
        /// Gets the height of the previous Container.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static double GetPreviousContainerHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(PreviousContainerHeightProperty);
        }

        /// <summary>
        /// Gets the width of the previous Container.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static double GetPreviousContainerWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(PreviousContainerWidthProperty);
        }

        /// <summary>
        /// Sets the width of the previous Container.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetPreviousContainerWidth(DependencyObject obj, double value)
        {
            obj.SetValue(PreviousContainerWidthProperty, value);
        }

        /// <summary>
        /// Sets the height of the previous Container.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetPreviousContainerHeight(DependencyObject obj, double value)
        {
            obj.SetValue(PreviousContainerHeightProperty, value);
        }

        /// <summary>
        /// Gets the desired size splitters.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        /// <param name="iChildren">The i children.</param>
        /// <returns>return double value.</returns>
        private double GetDesirdeSizeSplitters(Size availableSize, int iChildren)
        {
            int iSplitter = 0;
            double result = 0;

            foreach (Splitter splitter in m_Splitters)
            {
                splitter.Measure(availableSize);

                if (iSplitter < iChildren)
                {
                    switch (Orientation)
                    {
                        case Orientation.Horizontal:
                            result += splitter.DesiredSize.Width;
                            break;

                        case Orientation.Vertical:
                            result += splitter.DesiredSize.Height;
                            break;
                    }

                    ++iSplitter;
                }
            }

            return result;
        }

        /// <summary>
        /// Calls OnOrientationChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockedElementsContainer instance = (DockedElementsContainer)d;
            instance.OnOrientationChanged(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Arranges the splitter.
        /// </summary>
        /// <param name="splitter">The splitter.</param>
        /// <param name="coordinate">The coordinate.</param>
        /// <param name="usedLengthInverted">The used length inverted.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns>return double value.</returns>
        private double ArrangeSplitter(UIElement splitter, double coordinate, double usedLengthInverted, Orientation orientation)
        {
            Rect rectArrange = new Rect(0, 0, 0, 0);

            switch (orientation)
            {
                case Orientation.Horizontal:
                    rectArrange.X = coordinate;
                    rectArrange.Height = usedLengthInverted;
                    rectArrange.Width = (this.DockingManager.IsTouchEnabled) ? splitter.DesiredSize.Width + 6 : splitter.DesiredSize.Width;
                    coordinate += rectArrange.Width;
                    break;

                case Orientation.Vertical:
                    rectArrange.Y = coordinate;
                    rectArrange.Width = usedLengthInverted;
                    rectArrange.Height = (this.DockingManager.IsTouchEnabled) ? splitter.DesiredSize.Height + 6 : splitter.DesiredSize.Height;
                    coordinate += rectArrange.Height;
                    break;
            }

            if (splitter.DesiredSize.Width <= 0.0 && splitter.DesiredSize.Height<=0.0)
            {
                splitter.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            splitter.Arrange(rectArrange);
            return coordinate;
        }

        /// <summary>
        /// Arranges the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="coordinate">The coordinate.</param>
        /// <param name="usedLengthInverted">The used length inverted.</param>
        /// <param name="lengthFinal">The length final.</param>
        /// <param name="orientation">The orientation.</param>
        private static void ArrangeElement(UIElement element, double coordinate, double usedLengthInverted, double lengthFinal, Orientation orientation)
        {
            Rect rectArrange = new Rect(0, 0, 0, 0);

            switch (orientation)
            {
                case Orientation.Horizontal:
                    rectArrange.X = coordinate;
                    rectArrange.Height = usedLengthInverted;
                    rectArrange.Width = lengthFinal;
                    break;

                case Orientation.Vertical:
                    rectArrange.Y = coordinate;
                    rectArrange.Width = usedLengthInverted;
                    rectArrange.Height = lengthFinal;
                    break;
            }

            if (!double.IsNaN(rectArrange.Size.Height) && !double.IsNaN(rectArrange.Size.Width))
            {
                element.Measure(rectArrange.Size);
                element.Arrange(rectArrange);
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies Orientation dependency property of the <see cref="DockedElementsContainer"/>.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DockedElementsContainer), new FrameworkPropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Identifies <see cref="DockedElementsContainer"/> PreviousContainerWidth attached property.
        /// </summary>
        internal static readonly DependencyProperty PreviousContainerWidthProperty =
    DependencyProperty.RegisterAttached("PreviousContainerWidth", typeof(double), typeof(DockedElementsContainer), new FrameworkPropertyMetadata(90d));

        /// <summary>
        /// Identifies <see cref="DockedElementsContainer"/> PreviousContainerHeight attached property.
        /// </summary>
        internal static readonly DependencyProperty PreviousContainerHeightProperty =
    DependencyProperty.RegisterAttached("PreviousContainerHeight", typeof(double), typeof(DockedElementsContainer), new FrameworkPropertyMetadata(90d));


        /// <summary>
        /// Identifies ElementBeforeSplitter dependency property of the <see cref="DockedElementsContainer"/>.
        /// </summary>
        public static readonly DependencyProperty ElementBeforeSplitterProperty =
            DependencyProperty.Register("ElementBeforeSplitter", typeof(FrameworkElement), typeof(DockedElementsContainer), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies SplitterTargetSide attached property of the <see cref="DockedElementsContainer"/>.
        /// </summary>
        public static readonly DependencyProperty SplitterTargetSideProperty =
            DependencyProperty.RegisterAttached("SplitterTargetSide", typeof(SplitterTargetSide), typeof(DockedElementsContainer), new FrameworkPropertyMetadata(SplitterTargetSide.None));

        /// <summary>
        /// Identifies ElementAfterSplitter dependency property of the <see cref="DockedElementsContainer"/>.
        /// </summary>
        public static readonly DependencyProperty ElementAfterSplitterProperty =
            DependencyProperty.Register("ElementAfterSplitter", typeof(FrameworkElement), typeof(DockedElementsContainer), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the DockingManager dependency property
        /// </summary>
        public static readonly DependencyProperty DockingManagerProperty =
            DependencyProperty.Register("DockingManager", typeof(DockingManager), typeof(DockedElementsContainer));
        #endregion

        #region Alternative methods

        /// <summary>
        /// Collapses the maximize button visibility.
        /// </summary>
        /// <param name="container">The container.</param>
        internal static void CollapseMaxMinButtonVisibility(DockedElementsContainer container)
        {
            if(container.Children.Count<=2)
            {
                for (int i = 0; i < container.Children.Count; i++)
                {
                    DockedElementTabbedHost host = container.Children[i] as DockedElementTabbedHost;
                    if (host != null)
                    {
                        FrameworkElement element = host.InternalDataContext as FrameworkElement;
                        DockingManager owner = DockingManager.ResolveManager(element);
                        if (element != null)
                        {
                            if (DockingManager.GetSideInDockedMode(element) != DockSide.Tabbed && DockingManager.GetState(element)==DockState.Dock)
                            {
                                if (owner != null && owner.CheckMaximizeButtonMode(element)) 
                                    DockingManager.SetMaximizeButtonVisibility(element, Visibility.Collapsed);
                                DockingManager.SetMinimizeButtonVisibility(element, Visibility.Collapsed);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the minimized max min button visibility.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="elementtobeminimized">The elementtobeminimized.</param>
        internal static void CheckMinimizedMaxMinButtonVisibility(DockedElementsContainer container,FrameworkElement elementtobeminimized)
        {
            int count = 0;
            DockedElementTabbedHost hostminimized = null;
            ObservableFrameworkElements siblingelements = new ObservableFrameworkElements();

            if (container.Children.Count > 1)
            {
                for (int i = 0; i < container.Children.Count; i++)
                {
                    DockedElementTabbedHost host = container.Children[i] as DockedElementTabbedHost;
                    if (host != null)
                    {
                        FrameworkElement element = host.InternalDataContext as FrameworkElement;
                        if (element != null)
                        {
                            if (element.Equals(elementtobeminimized))
                            {
                                hostminimized = host;
                                count++;
                            }
                            else
                            {
                                siblingelements.Add(element);
                            }
                        }
                    }
                }

                if (siblingelements.Count == 0)
                {
                    DockedElementsContainer childcontainer = container.Parent as DockedElementsContainer;
                    if (childcontainer != null)
                    {
                        for (int j = 0; j < childcontainer.Children.Count; j++)
                        {
                            DockedElementTabbedHost host1 = childcontainer.Children[j] as DockedElementTabbedHost;
                            if (host1 != null)
                            {
                                FrameworkElement element2 = host1.InternalDataContext as FrameworkElement;
                                if (element2 != null
                                    && !(DockingManager.GetState(element2 as DependencyObject) == DockState.AutoHidden))
                                {
                                    siblingelements.Add(element2);
                                }
                            }
                        }
                    }

                    if (hostminimized != null && siblingelements.Count == 1)
                    {
                        hostminimized.m_siblingElement = siblingelements[0];
                    }
                }
                else
                {
                    if (hostminimized != null && siblingelements.Count == 1)
                    {
                        hostminimized.m_siblingElement = siblingelements[0];
                    }
                }

                DockingManager owner = DockingManager.ResolveManager(elementtobeminimized);

                if (DockingManager.GetDockWindowState(elementtobeminimized) == WindowState.Maximized)
                    DockingManager.SetRestoreButtonVisibility(elementtobeminimized, Visibility.Collapsed);
                else if (owner != null && owner.MaximizeButtonEnabled && (DockingManager.GetMaximizeButtonVisibility(elementtobeminimized)==Visibility.Visible))
                    DockingManager.SetMaximizeButtonVisibility(elementtobeminimized, Visibility.Collapsed);
                else if (owner != null && owner.MinimizeButtonEnabled && DockingManager.GetCanMinimize(elementtobeminimized))
                    DockingManager.SetMinimizeButtonVisibility(elementtobeminimized, Visibility.Collapsed);

                if (hostminimized != null && hostminimized.m_siblingElement != null) 
                {
                    if (DockingManager.GetDockWindowState(hostminimized.m_siblingElement) == WindowState.Maximized)
                        DockingManager.SetRestoreButtonVisibility(hostminimized.m_siblingElement, Visibility.Collapsed);
                    else if (owner != null && owner.MaximizeButtonEnabled && (DockingManager.GetMaximizeButtonVisibility(hostminimized.m_siblingElement) == Visibility.Visible))
                        DockingManager.SetMaximizeButtonVisibility(hostminimized.m_siblingElement, Visibility.Collapsed);
                    else if (owner != null && owner.MinimizeButtonEnabled && DockingManager.GetCanMinimize(hostminimized.m_siblingElement))
                        DockingManager.SetMinimizeButtonVisibility(hostminimized.m_siblingElement, Visibility.Collapsed);
                }
            }
        }

        /// <summary>
        /// Checks the maximize button visibility.
        /// </summary>
        /// <param name="container">The container.</param>
        internal static void EnableMaxMinButtonVisibility(DockedElementsContainer container)
        {
            if (container.DockingManager != null && (container.DockingManager.Children.Count > 1 
                && container.DockingManager.RootContainer != container && container.Children.Count > 1) || container.DockingManager.MaximizeMode == MaximizeMode.FullScreen) 
            {
                for (int i = 0; i < container.Children.Count; i++)
                {
                    DockedElementTabbedHost host = container.Children[i] as DockedElementTabbedHost;
                    if (host != null)
                    {
                        FrameworkElement element = host.InternalDataContext as FrameworkElement;
                        if (element != null)
                        {
                            if (DockingManager.GetSideInDockedMode(element) != DockSide.Tabbed && DockingManager.GetState(element) == DockState.Dock)
                            {
                                DockingManager owner = DockingManager.ResolveManager(element);
                                if (owner != null && owner.MaximizeButtonEnabled && DockingManager.GetCanMaximize(element))
                                {
                                    if (DockingManager.GetDockWindowState(element) == WindowState.Normal)
                                        DockingManager.SetMaximizeButtonVisibility(element, Visibility.Visible);
                                    else if (DockingManager.GetDockWindowState(element) == WindowState.Maximized)
                                        DockingManager.SetRestoreButtonVisibility(element, Visibility.Visible);
                                }
                                if (!owner.CheckMaximizeButtonMode(element) && (DockingManager.GetDockWindowState(element) == WindowState.Normal)) 
                                {
                                        DockingManager.SetMaximizeButtonVisibility(element, Visibility.Visible);
                                }
                                if (owner != null && owner.MinimizeButtonEnabled && DockingManager.GetCanMinimize(element))
                                {
                                    if (DockingManager.GetDockWindowState(element) == WindowState.Normal)
                                        DockingManager.SetMinimizeButtonVisibility(element, Visibility.Visible);
                                }
                            }
                        }
                    }
                    else
                    {
                        DockedElementsContainer childcontainer = container.Children[i] as DockedElementsContainer;
                        if (childcontainer != null)
                        {
                            for (int j = 0; j < childcontainer.Children.Count; j++)
                            {
                                DockedElementTabbedHost host1 = childcontainer.Children[j] as DockedElementTabbedHost;
                                if (host1 != null)
                                {
                                    FrameworkElement element = host1.InternalDataContext as FrameworkElement;
                                    if (element != null && DockingManager.GetTargetNameInDockedMode(element).Equals(string.Empty))
                                    {
                                        if (DockingManager.GetSideInDockedMode(element) != DockSide.Tabbed && DockingManager.GetState(element) == DockState.Dock)
                                        {
                                            DockingManager owner = DockingManager.ResolveManager(element);
                                            if (owner != null && owner.MaximizeButtonEnabled && DockingManager.GetCanMaximize(element))
                                            {
                                                if (DockingManager.GetDockWindowState(element) == WindowState.Normal)
                                                    DockingManager.SetMaximizeButtonVisibility(element, Visibility.Visible);
                                                else if (DockingManager.GetDockWindowState(element) == WindowState.Maximized)
                                                    DockingManager.SetRestoreButtonVisibility(element, Visibility.Visible);
                                            }
                                            if (!owner.CheckMaximizeButtonMode(element) && (DockingManager.GetDockWindowState(element) == WindowState.Normal)) 
                                            {
                                                    DockingManager.SetMaximizeButtonVisibility(element, Visibility.Visible);
                                            }
                                            if (owner != null && owner.MinimizeButtonEnabled && DockingManager.GetCanMinimize(element))
                                            {
                                                if (DockingManager.GetDockWindowState(element) == WindowState.Normal)
                                                    DockingManager.SetMinimizeButtonVisibility(element, Visibility.Visible);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Changes the orientation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>return docked element container.</returns>
        internal static DockedElementsContainer ChangeOrientation(DockedElementsContainer container)
        {
            DockedElementsContainer newContainer = container;

            if (newContainer.Children.Count > 1)
            {
                newContainer = new DockedElementsContainer(container.DockingManager);
                ((IDesiredSize)newContainer).DesiredSize = ((IDesiredSize)container).DesiredSize;
                newContainer.Orientation = (Orientation.Horizontal == container.Orientation)
                    ? Orientation.Vertical : Orientation.Horizontal;

                if (container.Parent is DockedElementsContainer)
                {
                    DockedElementsContainer parent = container.Parent as DockedElementsContainer;
                    int iIndex = parent.Children.IndexOf(container);
                    parent.Children.Insert(iIndex, newContainer);
                    parent.Children.Remove(container);
                    newContainer.Children.Add(newContainer);
                    DockedElementsContainer.EnableMaxMinButtonVisibility(newContainer);
                }
                else
                {
                    ContentControl parent = container.Parent as ContentControl;
                    parent.Content = newContainer;
                    newContainer.Children.Add(container);
                    DockedElementsContainer.EnableMaxMinButtonVisibility(newContainer);
                }
            }
            else
            {
                newContainer.Orientation = (Orientation.Horizontal == container.Orientation)
                    ? Orientation.Vertical : Orientation.Horizontal;
            }

            return newContainer;
        }

        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="host">The docked element tabbed host.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns>return docked elements container.</returns>
        internal static DockedElementsContainer CreateContainer(DockedElementsContainer container, DockedElementTabbedHost host, Orientation orientation)
        {
            DockedElementsContainer newContainer = new DockedElementsContainer(container.DockingManager);
            ((IDesiredSize)newContainer).DesiredSize = ((IDesiredSize)host).DesiredSize;
            int iIndex = container.Children.IndexOf(host);
            container.Children.Insert(iIndex, newContainer);
            container.Children.Remove(host);
            newContainer.Children.Add(host);
            DockedElementsContainer.EnableMaxMinButtonVisibility(newContainer);
            newContainer.Orientation = orientation;

            return newContainer;
        }

        /// <summary>
        /// Inserts the child.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="index">The index.</param>
        internal void InsertChild(FrameworkElement child, int index)
        {
            if (index < 0)
            {
                index = 0;
            }

            if (index >= Children.Count)
            {
                Children.Add(child);
            }
            else
            {
                Children.Insert(index, child);
            }
            
            DockedElementsContainer.EnableMaxMinButtonVisibility(this);
            InvalidateSplitters();
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="target">The target.</param>
        /// <param name="offset">The offset.</param>
        internal void AddChild(FrameworkElement child, FrameworkElement target, int offset)
        {
            int iIndex = Children.IndexOf(target);
            InsertChild(child, iIndex + offset);
        }

        /// <summary>
        /// Builds the container clone.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="state">The state.</param>
        /// <returns>return docked elements container.</returns>
        internal static DockedElementsContainer BuildContainerClone(DockedElementsContainer container, DockState state)
        {
            DockedElementsContainer clone = new DockedElementsContainer(container.DockingManager);
            clone.Orientation = container.Orientation;
            ((IDesiredSize)clone).DesiredSize = ((IDesiredSize)container).DesiredSize;

            foreach (FrameworkElement element in container.Children)
            {
                if (element.Visibility != Visibility.Collapsed)
                {
                    if (element is DockedElementTabbedHost)
                    {
                        DockedElementTabbedHost sourceHost = element as DockedElementTabbedHost;
                        DockedElementTabbedHost newHost = BuildHostClone(sourceHost, state, true, container.DockingManager);

                        clone.Children.Add(newHost);
                    }
                    else if (element is DockedElementsContainer)
                    {
                        DockedElementsContainer childClone = BuildContainerClone(element as DockedElementsContainer, state);
                        clone.Children.Add(childClone);
                    }
                }
            }

            return clone;
        }

        /// <summary>
        /// Builds the host clone.
        /// </summary>
        /// <param name="sourceHost">The source host.</param>
        /// <param name="state">The state.</param>
        /// <param name="bMoveTabs">if set to <c>true</c> [b move tabs].</param>
        /// <param name="docking">The docking.</param>
        /// <returns>return docked elements tabbed host.</returns>
        internal static DockedElementTabbedHost BuildHostClone(DockedElementTabbedHost sourceHost, DockState state, bool bMoveTabs, DockingManager docking)
        {
            DockedElementTabbedHost newHost = new DockedElementTabbedHost(docking);
            docking.m_Completehost.Add(newHost);
            ((IDesiredSize)newHost).DesiredSize = ((IDesiredSize)sourceHost).DesiredSize;
            newHost.State = state;

            if (bMoveTabs)
            {
                bool bSameState = sourceHost.State == state;
                List<FrameworkElement> tabs = new List<FrameworkElement>(sourceHost.TabChildren);
                sourceHost.TabChildren.Clear();
                tabs.Sort(new TabChildrenComparer());

                foreach (FrameworkElement tab in tabs)
                {
                    if (!bSameState)
                    {
                        DockedElementTabbedHost.RemoveTab(tab, state);
                    }
                }
                docking.m_setinternal = true;
                foreach (FrameworkElement tab in tabs)
                {
                    if (!bSameState)
                    {
                        DockingManager.CopyDockingParameters(tab, state);
                        DockingManager.SetState(tab, state);
                    }

                    newHost.TabChildren.Add(tab);
                    DockingManager.SetTabbedHost(tab, newHost, state);
                }
                docking.m_setinternal = false;
            }

            return newHost;
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="child">The child.</param>
        internal void RemoveChild(FrameworkElement child)
        {
            Children.Remove(child);
            InvalidateSplitters();
        }

        /// <summary>
        /// Invalidates the splitters.
        /// </summary>
        internal void InvalidateSplitters()
        {
            int iCount = Children.Count - 1;

            Orientation orientation = (Orientation == Orientation.Horizontal)
                ? Orientation.Vertical : Orientation.Horizontal;
            if (m_Splitters != null)
            {
                if (m_Splitters.Count < iCount)
                {
                    for (int i = m_Splitters.Count; i < iCount; i++)
                    {
                        Splitter splt = new Splitter(orientation);
                        splt.DockingManager = DockingManager;
                        splt.UseNativeFloatWindow = DockingManager.UseNativeFloatWindow;
                        splt.OffsetChanged += new PropertyChangedCallback(OnSpltOffsetChanged);
                        m_Splitters.Add(splt);
                        AddVisualChild(splt);
                    }
                }
                else if (m_Splitters.Count > iCount && iCount >= 0)
                {
                    for (int i = iCount; i < m_Splitters.Count; i++)
                    {
                        RemoveVisualChild(m_Splitters[i]);
                    }

                    m_Splitters.RemoveRange(iCount, m_Splitters.Count - iCount);
                }

                foreach (Splitter splt in m_Splitters)
                {
                    splt.Orientation = orientation;
                }
            }
        }

        /// <summary>
        /// Checks the visibility.
        /// </summary>
        internal void CheckVisibility()
        {
            bool bHasVisiobleChild = false;

            foreach (FrameworkElement child in Children)
            {
                if (child.Visibility == Visibility.Visible)
                {
                    bHasVisiobleChild = true;
                }
            }

            Visibility = bHasVisiobleChild ? Visibility.Visible : Visibility.Collapsed;

            DockedElementsContainer parent = Parent as DockedElementsContainer;

            if (parent != null)
            {
                parent.CheckVisibility();
            }
        }

        /// <summary>
        /// Checks if empty.
        /// </summary>
        internal void CheckIfEmpty()
        {
            bool bIsEmpty = Children.Count == 0;

            if (bIsEmpty)
            {
                DockedElementsContainer parent = Parent as DockedElementsContainer;

                if (parent != null)
                {
                    ((IRemoveChild)parent).RemoveChild(this);
                }
            }
        }

        /// <summary>
        /// Inits the size of the desired.
        /// </summary>
        /// <param name="size">The size value.</param>
        internal void InitDesiredSize(Size size)
        {
            if (m_desiredSize == Size.Empty || m_desiredSize == new Size(0,0))
            {
                int iCount = 0;
                bool bHasDockToFillElement = false;
                Size containerSize = new Size(0, 0);

                foreach (FrameworkElement child in Children)
                {
                    if (child.Visibility != Visibility.Collapsed)
                    {
                        Size sizeElement = ((IDesiredSize)child).DesiredSize;

                        if (sizeElement.Width == 0 && Orientation == Orientation.Horizontal ||
                            sizeElement.Height == 0 && Orientation == Orientation.Vertical)
                        {
                            bHasDockToFillElement = true;
                            break;
                        }

                        switch (Orientation)
                        {
                            case Orientation.Horizontal:
                                containerSize.Width += sizeElement.Width;
                                containerSize.Height = Math.Max(containerSize.Height, sizeElement.Height);
                                break;

                            case Orientation.Vertical:
                                containerSize.Width = Math.Max(containerSize.Width, sizeElement.Width);
                                containerSize.Height += sizeElement.Height;
                                break;
                        }

                        iCount++;
                    }
                }

                if (Orientation == Orientation.Horizontal)
                {
                    containerSize.Width += GetDesirdeSizeSplitters(size, iCount);
                }
                else
                {
                    containerSize.Height += GetDesirdeSizeSplitters(size, iCount);
                }

                m_desiredSize = (!bHasDockToFillElement) ? containerSize : new Size(0, 0);
            }
        }

        /// <summary>
        /// Gets the minimum length.
        /// </summary>
        /// <returns>return double value.</returns>
        private double GetMinimumLength()
        {
            double minCintainerLength = 0;

            double minElementLength = (Orientation == Orientation.Horizontal)
                ? MinElementWidth : MinElementHeight;

            foreach (FrameworkElement child in Children)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                    minCintainerLength += (child is DockedElementsContainer)
                        ? (child as DockedElementsContainer).GetMinimumLength()
                        : minElementLength;
                }
            }

            return minCintainerLength;
        }

        /// <summary>
        /// Handles the minimum length.
        /// </summary>
        private void HandleMinimumLength()
        {
            if (!(Parent is DockedElementsContainer))
            {
                int visibleChildren = 0;

                foreach (FrameworkElement element in Children)
                {
                    if (element.Visibility != Visibility.Collapsed)
                    {
                        ++visibleChildren;
                    }
                }

                double minLength = GetMinimumLength() + GetSplittersDesiredSizeWidth(visibleChildren);
                bool bIsHorizontalOrientation = Orientation == Orientation.Horizontal;

                if (bIsHorizontalOrientation)
                {
                    DockingManager.MinWidth = DockingManager.ActualWidth < minLength ? DockingManager.ActualWidth : minLength;
                }
                else
                {
                    DockingManager.MinHeight = DockingManager.ActualHeight < minLength ? DockingManager.ActualHeight : minLength;
                }

                if (DockingManager.RestrictWindowMinimumSize)
                {
                    Window wnd = VisualUtils.FindRootVisual(this) as Window;

                    if (wnd != null && wnd.IsLoaded && !BrowserInteropHelper.IsBrowserHosted)
                    {
                        if (bIsHorizontalOrientation)
                        {
                            wnd.MinWidth = minLength;
                        }
                        else
                        {
                            wnd.MinHeight = minLength;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="renderSize">Size of the render.</param>
        /// <returns></returns>
        private DockedElementTabbedHost GetHost(FrameworkElement element, Size renderSize, Rect m_splitterboundingrect, bool beforesplitter, Orientation orientation)
        {
            DockedElementTabbedHost host = null;
            if ((element as DockedElementsContainer) != null)
            {
                host = IterateContainerItems((element as DockedElementsContainer), renderSize, m_splitterboundingrect, beforesplitter, orientation);
            }
            else if ((element as DockedElementTabbedHost) != null)
            {
                host = (element as DockedElementTabbedHost);
            }
            return host;
        }

        /// <summary>
        /// Gets the possible offset.
        /// </summary>
        /// <param name="splitteroffset">The splitteroffset.</param>
        /// <param name="host">The host.</param>
        /// <param name="rendersize">The rendersize.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns></returns>
        private double GetPossibleOffset(double splitterOffset, DockedElementTabbedHost host, Size rendersize,Orientation orientation,bool childafterflag)
        {
            if (host != null && host.HostedElement != null)
            {
                if (orientation == Orientation.Horizontal)
                {
                    double minwidth = DockingManager.GetDesiredMinWidthInDockedMode(host.HostedElement);
                    double maxwidth = DockingManager.GetDesiredMaxWidthInDockedMode(host.HostedElement);
                    if (!childafterflag)
                    {
                        if ((rendersize.Width + splitterOffset) < minwidth)
                        {
                            if (minwidth != 0)
                            {
                                splitterOffset = splitterOffset + (minwidth - (rendersize.Width + splitterOffset));
                            }
                        }
                        if ((rendersize.Width + splitterOffset) > maxwidth)
                        {
                            if (maxwidth != 0)
                            {
                                splitterOffset = splitterOffset - ((rendersize.Width + splitterOffset) - maxwidth);
                            }
                        }
                    }
                    else
                    {
                        if ((rendersize.Width - splitterOffset) < minwidth)
                        {
                            if (minwidth != 0)
                            {
                                if (rendersize.Width < minwidth)
                                    splitterOffset = 0;
                                else
                                    splitterOffset = splitterOffset - (minwidth - (rendersize.Width - splitterOffset));
                            }
                        }
                        if ((rendersize.Width - splitterOffset) > maxwidth)
                        {
                            if (maxwidth != 0)
                            {
                                splitterOffset = splitterOffset + ((rendersize.Width - splitterOffset) - maxwidth);
                            }
                        }
                    }
                }
                else if (orientation == Orientation.Vertical)
                {
                    double minHeight = DockingManager.GetDesiredMinHeightInDockedMode(host.HostedElement);
                    double maxHeight = DockingManager.GetDesiredMaxHeightInDockedMode(host.HostedElement);
                    if (!childafterflag)
                    {
                        if ((rendersize.Height + splitterOffset) < minHeight)
                        {
                            if (minHeight != 0)
                            {
                                splitterOffset = splitterOffset + (minHeight - (rendersize.Height + splitterOffset));
                            }
                        }
                        if ((rendersize.Height + splitterOffset) > maxHeight)
                        {
                            if (maxHeight != 0)
                            {
                                splitterOffset = splitterOffset - ((rendersize.Height + splitterOffset) - maxHeight);
                            }
                        }
                    }
                    else
                    {
                        if ((rendersize.Height - splitterOffset) < minHeight)
                        {
                            if (minHeight != 0)
                            {
                                if (rendersize.Height < minHeight)
                                    splitterOffset = 0;
                                else 
                                    splitterOffset = splitterOffset - (minHeight - (rendersize.Height - splitterOffset));
                            }
                        }
                        if ((rendersize.Height - splitterOffset) > maxHeight)
                        {
                            if (maxHeight != 0)
                            {
                                splitterOffset = splitterOffset + ((rendersize.Height - splitterOffset) - maxHeight);
                            }
                        }
                    }
                }
            }
            return splitterOffset;
        }

        /// <summary>
        /// Gets the size of the correct.
        /// </summary>
        /// <param name="actualhost">The actualhost.</param>
        /// <param name="element">The element.</param>
        /// <param name="rendersize">The rendersize.</param>
        /// <returns></returns>
        private Size GetExactHostDesiredSize(DockedElementTabbedHost actualhost, FrameworkElement element, Size rendersize)
        {
            if (element is DockedElementsContainer && actualhost != null 
                && (actualhost as IChildrenResize).DesiredSize.Width > 0
                && (actualhost as IChildrenResize).DesiredSize.Height > 0)
            {
                return (actualhost as IChildrenResize).DesiredSize;
            }
            return rendersize;
        }

        /// <summary>
        /// Resizes the elements.
        /// </summary>
        /// <param name="childBefore">The child before.</param>
        /// <param name="childAfter">The child after.</param>
        /// <param name="splitterOffset">The splitter offset.</param>
        private void ResizeElements(IChildrenResize childBefore, IChildrenResize childAfter, double splitterOffset, Rect m_splitterboundingrect)
        {
            if (childBefore != null && childAfter != null)
            {
                FrameworkElement elementBefore = (FrameworkElement)childBefore;
                FrameworkElement elementAfter = (FrameworkElement)childAfter;

                this.m_checkminmaxsize = false;
                if (elementBefore is DockedElementsContainer)
                    (elementBefore as DockedElementsContainer).m_checkminmaxsize = false;
                if (elementAfter is DockedElementsContainer)
                    (elementAfter as DockedElementsContainer).m_checkminmaxsize = false;

                Size renderSizeBefore = elementBefore.RenderSize;
                Size renderSizeAfter = elementAfter.RenderSize;

                Size desiredSizeBefore = childBefore.DesiredSize;
                Size desiredSizeAfter = childAfter.DesiredSize;
                ObservableCollection<DockedElementTabbedHost> _container = new ObservableCollection<DockedElementTabbedHost>();

                DockedElementTabbedHost actualhost = null;
                Size m_actualdesiredsize = new Size(0, 0);

                double multiplier = 1;

                switch (Orientation)
                {
                    case Orientation.Horizontal:

                        if (childBefore is DockedElementTabbedHost && ((DockedElementTabbedHost)childBefore).InternalDataContext != null)
                        {
                            if (DockingManager.CheckFixedsize(((DockedElementTabbedHost)childBefore).InternalDataContext, Orientation) &&
                                (DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)childBefore).InternalDataContext) == DockSide.Left ||
                                DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)childBefore).InternalDataContext) == DockSide.Right)
                                && !DockingManager.CheckResize(((DockedElementTabbedHost)childBefore).InternalDataContext, DockState.Dock, Orientation))
                            {
                                return;
                            }
                        }

                        if ((childBefore as DockedElementsContainer) != null && !CheckResizeCondition(childBefore, renderSizeBefore, m_splitterboundingrect))
                        {
                            return;
                        }

                        multiplier = (desiredSizeBefore.Width != 0)
                            ? renderSizeBefore.Width / desiredSizeBefore.Width
                            : renderSizeAfter.Width / desiredSizeAfter.Width;

                        if (double.IsInfinity(multiplier))
                        {
                            multiplier = 1;
                        }

                        actualhost = GetHost(elementBefore, renderSizeBefore, m_splitterboundingrect, true, Orientation.Horizontal);
                        m_actualdesiredsize = GetExactHostDesiredSize(actualhost, elementBefore, renderSizeBefore);
                        splitterOffset = GetPossibleOffset(splitterOffset, actualhost, m_actualdesiredsize, Orientation.Horizontal, false);

                        if (childAfter is DockedElementTabbedHost && ((DockedElementTabbedHost)childAfter).InternalDataContext != null)
                        {
                            if (DockingManager.CheckFixedsize(((DockedElementTabbedHost)childAfter).InternalDataContext, Orientation) &&
                                (DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)childAfter).InternalDataContext) == DockSide.Left ||
                                DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)childAfter).InternalDataContext) == DockSide.Right)
                                && !DockingManager.CheckResize(((DockedElementTabbedHost)childAfter).InternalDataContext, DockState.Dock, Orientation))
                            {
                                return;
                            }
                        }

                        if ((childAfter as DockedElementsContainer) != null && !CheckResizeCondition(childAfter, renderSizeAfter, m_splitterboundingrect))
                        {
                            return;
                        }

                        actualhost = GetHost(elementAfter, renderSizeAfter, m_splitterboundingrect, false, Orientation.Horizontal);
                        m_actualdesiredsize = GetExactHostDesiredSize(actualhost, elementAfter, renderSizeAfter);
                        splitterOffset = GetPossibleOffset(splitterOffset, actualhost, m_actualdesiredsize, Orientation.Horizontal, true);
                        
                        if (splitterOffset != 0)
                        {
                            childBefore.SetWidth((renderSizeBefore.Width + splitterOffset) / multiplier);

                            childAfter.SetWidth((renderSizeAfter.Width - splitterOffset) / multiplier);
                        }

                        break;

                    case Orientation.Vertical:

                        if (childBefore is DockedElementTabbedHost && ((DockedElementTabbedHost)childBefore).InternalDataContext != null)
                        {
                            if (DockingManager.CheckFixedsize(((DockedElementTabbedHost)childBefore).InternalDataContext, Orientation) &&
                                (DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)childBefore).InternalDataContext) == DockSide.Top ||
                                DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)childBefore).InternalDataContext) == DockSide.Bottom)
                                && !DockingManager.CheckResize(((DockedElementTabbedHost)childBefore).InternalDataContext, DockState.Dock, Orientation))
                            {
                                return;
                            }
                        }

                        if ((childBefore as DockedElementsContainer) != null && !CheckResizeCondition(childBefore, renderSizeBefore, m_splitterboundingrect))
                        {
                            return;
                        }
                        multiplier = (desiredSizeBefore.Height != 0)
                            ? renderSizeBefore.Height / desiredSizeBefore.Height
                            : renderSizeAfter.Height / desiredSizeAfter.Height;

                        if (double.IsInfinity(multiplier))
                        {
                            multiplier = 1;
                        }

                        actualhost = GetHost(elementBefore, renderSizeBefore, m_splitterboundingrect, true, Orientation.Vertical);
                        m_actualdesiredsize = GetExactHostDesiredSize(actualhost, elementBefore, renderSizeBefore);
                        splitterOffset = GetPossibleOffset(splitterOffset, actualhost, m_actualdesiredsize, Orientation.Vertical,false);

                        if (childAfter is DockedElementTabbedHost && ((DockedElementTabbedHost)childAfter).InternalDataContext != null)
                        {
                            if (DockingManager.CheckFixedsize(((DockedElementTabbedHost)childAfter).InternalDataContext, Orientation) &&
                                (DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)childAfter).InternalDataContext) == DockSide.Top ||
                                DockingManager.GetSideInDockedMode(((DockedElementTabbedHost)childAfter).InternalDataContext) == DockSide.Bottom)
                                && !DockingManager.CheckResize(((DockedElementTabbedHost)childAfter).InternalDataContext, DockState.Dock, Orientation))
                            {
                                return;
                            }
                        }

                        if ((childAfter as DockedElementsContainer) != null && !CheckResizeCondition(childAfter, renderSizeAfter, m_splitterboundingrect))
                        {
                            return;
                        }

                       
                        actualhost = GetHost(elementAfter, renderSizeAfter, m_splitterboundingrect, false, Orientation.Vertical);
                        m_actualdesiredsize = GetExactHostDesiredSize(actualhost, elementAfter, renderSizeAfter);
                        splitterOffset = GetPossibleOffset(splitterOffset, actualhost, m_actualdesiredsize, Orientation.Vertical, true);

                        if (splitterOffset != 0)
                        {
                            childBefore.SetHeight((renderSizeBefore.Height + splitterOffset) / multiplier);

                            childAfter.SetHeight((renderSizeAfter.Height - splitterOffset) / multiplier);
                        }
                        break;
                }
                this.m_checkminmaxsize = true;

                if (elementBefore is DockedElementsContainer)
                    (elementBefore as DockedElementsContainer).m_checkminmaxsize = true;
                if (elementAfter is DockedElementsContainer)
                    (elementAfter as DockedElementsContainer).m_checkminmaxsize = true;
            }
        }

        internal bool CheckResizeCondition(IChildrenResize child, Size renderSize, Rect m_splitterboundingrect)
        {
            ObservableCollection<DockedElementTabbedHost> _container = IterateDocumentContainerItems((child as DockedElementsContainer), renderSize, m_splitterboundingrect, false, Orientation);
            if (_container != null)
            {
                for (int i = 0; i < _container.Count; i++)
                {
                    DockedElementTabbedHost actualhost = _container[i];
                    if (actualhost != null && actualhost.HostedElement != null)
                    {
                        if (!DockingManager.CheckResize(actualhost.HostedElement, DockState.Dock, Orientation))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Measures the splitters.
        /// </summary>
        /// <param name="size">The size of the splitter.</param>
        private void MeasureSplitters(Size size)
        {
            foreach (Splitter splitter in m_Splitters)
            {
                splitter.Measure(size);
            }
        }
        #endregion
    }
}