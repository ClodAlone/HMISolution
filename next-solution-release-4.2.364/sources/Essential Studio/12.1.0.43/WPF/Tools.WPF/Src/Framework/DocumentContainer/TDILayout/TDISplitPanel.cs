// <copyright file="TDISplitPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#define DEBUG
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents split panel for locate two elements.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TDISplitPanel : PanelBase
    {
        #region Constants
        /// <summary>
        /// Presents visual children count.
        /// </summary>
        private const int VISUAL_CHILDREN_COUNT = 3;
        
        /// <summary>
        /// Presents splitter size.
        /// </summary>
        private const double SPLITTER_SIZE = 5.0;
        
        /// <summary>
        /// Presents indent for splitter.
        /// </summary>
        private const double INDENT = 100;
        #endregion

        #region Private members
        /// <summary>
        /// Presents empty point.
        /// </summary>
        private readonly static Point EMPTY_POINT = new Point(0, 0);
        
        /// <summary>
        /// Presents orientation for split.
        /// </summary>
        private readonly Orientation m_Orientation = Orientation.Horizontal;
        
        /// <summary>
        /// Presents splitter.
        /// </summary>
        private readonly Splitter m_Splitter = new Splitter();

        /// <summary>
        /// Presents first element in panel.
        /// </summary>
        private UIElement m_firstElement = null;
        
        /// <summary>
        /// Presents second element in panel.
        /// </summary>
        private UIElement m_secondElement = null;

     
        /// <summary>
        /// Presents global offset of splitter.
        /// </summary>
        internal double Offset
        {
            get;
            set;
        }
        
        /// <summary>
        /// Presents LogicalChildrenInternal
        /// </summary>
        private UIElement[] logicalChildrenInternal;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="TDISplitPanel"/> class.
        /// </summary>
        /// <param name="firstElement">The first element.</param>
        /// <param name="secondElement">The second element.</param>
        /// <param name="orientation">The orientation.</param>
        public TDISplitPanel(UIElement firstElement, UIElement secondElement, Orientation orientation)
        {
            m_firstElement = firstElement;
            m_secondElement = secondElement;
            AddChild(m_firstElement);
            AddChild(m_Splitter);
            AddChild(m_secondElement);
            BindingUtils.SetBinding(m_firstElement, this, SkinStorage.VisualStyleProperty, SkinStorage.VisualStyleProperty);
            BindingUtils.SetBinding(m_secondElement, this, SkinStorage.VisualStyleProperty, SkinStorage.VisualStyleProperty);
            m_Splitter.OffsetChanged += new PropertyChangedCallback(OnSplitterOffsetChanged);
            m_Orientation = orientation;
            m_Splitter.Orientation = orientation;
            m_Splitter.Background = Brushes.Transparent;
            SetOffset();
            logicalChildrenInternal = new UIElement[2] { m_firstElement, m_secondElement };
            this.Unloaded += new RoutedEventHandler(TDISplitPanel_Unloaded);
        }

        private void SetOffset()
        {
            DocumentTabControl tabcontrol = GetDocumentTabControl();
            double tempoffset = 0.0;

            if (tabcontrol !=null && tabcontrol.Items.Count > 0)
            {
                foreach (object element in tabcontrol.Items)
                {
                    TabItemExt tabitem = element as TabItemExt;
                    if (tabitem.Content is ContentPresenter)
                    {
                        double value = TDILayoutPanel.GetSplitPanelOffset((tabitem.Content as ContentPresenter).Content as UIElement);
                        if (value != 0.0)
                        {
                            tempoffset = value;
                        }
                    }
                }

                Offset = tempoffset;
            }
        }

        private DocumentTabControl GetDocumentTabControl()
        {
            if (m_firstElement is TDISplitPanel)
                return m_secondElement as DocumentTabControl;
            else
                return m_firstElement as DocumentTabControl;
        }

        void TDISplitPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_Splitter != null) 
                m_Splitter.OffsetChanged -= new PropertyChangedCallback(OnSplitterOffsetChanged);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return VISUAL_CHILDREN_COUNT;
            }
        }
        
        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        /// <value></value>
        /// <returns>An enumerator for logical child elements of this element.</returns>
        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                return logicalChildrenInternal.GetEnumerator();
            }
        }
        #endregion

        #region PanelBase methods
        /// <summary>
        /// Removes the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public override void RemoveElement(UIElement element)
        {
            RemoveVisualChild(element);
            RemoveLogicalChild(element);
            BindingOperations.ClearBinding(element, SkinStorage.VisualStyleProperty);

            if (element == m_firstElement)
            {
                m_firstElement = null;
            }
            else if (element == m_secondElement)
            {
                m_secondElement = null;
            }
        }
        
        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public override void AddElement(UIElement element)
        {
            if (null == m_firstElement)
            {
                m_firstElement = element;
            }
            else if (null == m_secondElement)
            {
                m_secondElement = element;
            }

            AddChild(element);
            BindingUtils.SetBinding(element, this, SkinStorage.VisualStyleProperty, SkinStorage.VisualStyleProperty);
            InvalidateMeasure();
            InvalidateArrange();
        }
        
        /// <summary>
        /// Determines whether [has other element] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="next">if set to <c>true</c> validate next element otherwise previous.</param>
        /// <returns>
        /// <c>true</c> if [has other element] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public override bool HasElement(UIElement element, bool next)
        {
            return next ? element == m_firstElement : element == m_secondElement;
        }
        
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <param name="first">if set to <c>true</c> [first].</param>
        /// <returns>UIElement Element</returns>
        public override UIElement GetElement(bool first)
        {
            return first ? m_firstElement : m_secondElement;
        }
        
        /// <summary>
        /// Gets the opposite element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>UIElement element</returns>
        public override UIElement GetOppositeElement(UIElement element)
        {
#if DEBUG
            if (element != m_firstElement && element != m_secondElement)
            {
                throw new ArgumentException();
            }
#endif

            return element == m_firstElement ? m_secondElement : m_firstElement;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            switch (index)
            {
                case 0:
                    return m_firstElement;
                case 1:
                    return m_Splitter;
                case 2:
                    return m_secondElement;
                default:
                    throw new ArgumentException();
            }
        }
        
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Orientation.Horizontal == m_Orientation)
            {
                double width = finalSize.Width;
                double height = (finalSize.Height - SPLITTER_SIZE) / 2;
                double highHeight = Math.Max(height + Offset, 0);
                double lowHeight = Math.Max(height - Offset, 0);

                m_firstElement.Arrange(new Rect(EMPTY_POINT, new Size(width, highHeight)));
                m_Splitter.Arrange(new Rect(new Point(0, highHeight), new Size(width, SPLITTER_SIZE)));
                m_secondElement.Arrange(new Rect(new Point(0, highHeight + SPLITTER_SIZE), new Size(width, lowHeight)));
            }
            else
            {
                double width = (finalSize.Width - SPLITTER_SIZE) / 2;
                double height = finalSize.Height;
                double highWidth = Math.Max(width + Offset, 0);
                double lowWidth = Math.Max(width - Offset, 0);
                m_firstElement.Arrange(new Rect(EMPTY_POINT, new Size(highWidth, height)));
                m_Splitter.Arrange(new Rect(new Point(highWidth, 0), new Size(SPLITTER_SIZE, height)));
                m_secondElement.Arrange(new Rect(new Point(highWidth + SPLITTER_SIZE, 0), new Size(lowWidth, height)));
            }

            return finalSize;
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
            m_Splitter.Measure(availableSize);

            if (Orientation.Horizontal == m_Orientation)
            {
                double width = availableSize.Width;
                double height = (availableSize.Height - SPLITTER_SIZE) / 2;
                m_firstElement.Measure(new Size(width, Math.Max(height + Offset, 0)));
                m_secondElement.Measure(new Size(width, Math.Max(height - Offset, 0)));
            }
            else
            {
                double width = (availableSize.Width - SPLITTER_SIZE) / 2;
                double height = availableSize.Height;
                m_firstElement.Measure(new Size(Math.Max(width + Offset, 0), height));
                m_secondElement.Measure(new Size(Math.Max(width - Offset, 0), height));
            }

            return availableSize;
        }
        
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (Orientation.Horizontal == m_Orientation)
            {
                double previousWidth = sizeInfo.PreviousSize.Width;

                if (0 != previousWidth)
                {
                    double factor = sizeInfo.NewSize.Width / previousWidth;
                    //m_offset *= factor;
                }
            }
            else
            {
                double previousHeight = sizeInfo.PreviousSize.Height;

                if (0 != previousHeight)
                {
                    double factor = sizeInfo.NewSize.Height / previousHeight;
                    //m_offset *= factor;
                }
            }

            CalculateOffsetMaximums();
            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// Called when [splitter offset changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSplitterOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Offset += (double)e.NewValue;
            CalculateOffsetMaximums();
            InvalidateMeasure();
            InvalidateArrange();
        }
        
        /// <summary>
        /// Calculates the offset maximums.
        /// </summary>
        private void CalculateOffsetMaximums()
        {
            if (Orientation.Horizontal == m_Orientation)
            {
                double maxWidth = (RenderSize.Height - INDENT)/2;
                m_Splitter.MaxOffsetLeft = Math.Max(0, maxWidth + Offset);
                m_Splitter.MaxOffsetRight = Math.Max(0, maxWidth - Offset);
            }
            else
            {
                double maxHeight = (RenderSize.Height - INDENT+250)/2;
                m_Splitter.MaxOffsetLeft = Math.Max(0, maxHeight + Offset);
                m_Splitter.MaxOffsetRight = Math.Max(0, maxHeight - Offset);
            }
        }
        
        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="element">The element.</param>
        private void AddChild(Visual element)
        {
            AddVisualChild(element);
            AddLogicalChild(element);
        }
        #endregion
    }
}