// <copyright file="SplitHeaderPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class Represents the Splitter Header panel
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SplitHeaderPanel : Panel
    {
        #region Constants

        /// <summary>
        /// Contains SplitHeaderPanel name.
        /// </summary>
        private const string CsplitHeaderPanel = "PART_SplitHeaderPanel";
        
        /// <summary>
        /// Contains SplitButton name
        /// </summary>
        private const string CsplitButton = "PART_SplitButton";
        
        /// <summary>
        /// Contains SplitterMunuPanel name
        /// </summary>
        private const string CsplitterMunuPanel = "PART_SplitterMunuPanel";
        
        /// <summary>
        /// Name of the border that contains resize options between panels.
        /// </summary>
        private const string ChorizContainerBorder = "PART_HorizContainerBorder";
        
        /// <summary>
        /// Contains name of the SplitBorder.
        /// </summary>
        private const string CsplitBorder = "PART_SplitBorder";
        #endregion

        #region Properties
        /// <summary>
        /// Gets CustomGridSplitter.
        /// </summary>
        internal static CustomGridSplitter GridSplitter
        {
            get
            {
                return m_gridSplitter;
            }
        }
        
        /// <summary>
        /// Gets top items of the tab splitter.
        /// </summary>
        private ItemsControl TopItems
        {
            get
            {
                return InternalChildren[0] as ItemsControl;
            }
        }
        
        /// <summary>
        /// Gets bottom items of the tab splitter.
        /// </summary>
        private ItemsControl BottomItems
        {
            get
            {
                return InternalChildren[2] as ItemsControl;
            }
        }
        
        /// <summary>
        /// Gets split button of the tab splitter.
        /// </summary>
        private Button SplitButton
        {
            get
            {
                return InternalChildren[1] as Button;
            }
        }
        
        /// <summary>
        /// Gets horizontal button of the tab splitter.
        /// </summary>
        /// <value>The splitter menu panel.</value>
        private DockPanel SplitterMunuPanel
        {
            get
            {
                return InternalChildren[3] as DockPanel;
            }
        }
        #endregion

        #region Private members
        /// <summary>
        /// Contains height of the tab splitter row in horizontal position 
        /// (in vertical position is used as width).
        /// </summary>
        private double m_rowHeight;
        
        /// <summary>
        /// Contains CustomGridSplitter.
        /// </summary>
        private static CustomGridSplitter m_gridSplitter = null;
        
        /// <summary>
        /// Contains TabSplitter.
        /// </summary>
        private TabSplitter m_tabSplitter = null;
       
        /// <summary>
        /// Border that contains resize options between panels.
        /// </summary>
        private Border m_horizContainerBorder = null;
        
        /// <summary>
        /// Contains grid that changes resize direction.
        /// </summary>
        private Grid m_resizedGrid = null;
        
        /// <summary>
        /// Contains rows of the resized grid (is used when resize direction=rows)
        /// </summary>
        private RowDefinitionCollection m_gridRows = null;
        
        /// <summary>
        /// Contains columns of the resized grid (is used when resize direction=cols)
        /// </summary>
        private ColumnDefinitionCollection m_gridCols = null;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="SplitHeaderPanel"/> class.
        /// </summary>
        static SplitHeaderPanel()
        {
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(SplitHeaderPanel), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(SplitHeaderPanel), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calculates max RowHeight of the horizontal-oriented CustomGridSplitter
        /// </summary>
        /// <param name="gridSplitter">CustomGridSplitter to compare ResizeDirection.</param>
        /// <param name="availableSize">available size of the SplitHeaderPanel</param>
        private void CalculateMaxRowHeight(CustomGridSplitter gridSplitter, Size availableSize)
        {
            if (gridSplitter != null)
            {
                m_rowHeight = 0;

                foreach (UIElement element in InternalChildren)
                {
                    if (element.Visibility == Visibility.Collapsed)
                    {
                        continue;
                    }

                    element.Measure(availableSize);
                    double currentHeight = element.DesiredSize.Height;

                    if (m_rowHeight < currentHeight)
                    {
                        m_rowHeight = currentHeight;
                    }
                }
            }
        }
        
        /// <summary>
        /// Updates TabStripPlacement of the rows.
        /// </summary>
        private void UpdateRowsTabStripPlacement()
        {
            UpdatePlacement(Dock.Bottom, Dock.Bottom);
            BottomItems.ClearValue(SplitterPage.TabStripPlacementPropertyKey);
            SplitterPage.SetTabStripPlacement(BottomItems, Dock.Top);
        }
        
        /// <summary>
        /// Updates TabStripPlacement of the cols.
        /// </summary>
        private void UpdateColsTabStripPlacement()
        {
            UpdatePlacement(Dock.Bottom, Dock.Bottom);
            TopItems.ClearValue(SplitterPage.TabStripPlacementPropertyKey);
            BottomItems.ClearValue(SplitterPage.TabStripPlacementPropertyKey);

            SplitterPage.SetTabStripPlacement(TopItems, Dock.Left);
            SplitterPage.SetTabStripPlacement(BottomItems, Dock.Right);
        }
        
        /// <summary>
        /// Updates TabStripPlacement.
        /// </summary>
        /// <param name="topItemsPlacement">placement of the top items</param>
        /// <param name="botItemsPlacement">placement of the bottom items</param>
        private void UpdatePlacement(Dock topItemsPlacement, Dock botItemsPlacement)
        {
            TopItems.ClearValue(SplitterPage.TabStripPlacementPropertyKey);
            BottomItems.ClearValue(SplitterPage.TabStripPlacementPropertyKey);

            SplitterPage.SetTabStripPlacement(TopItems, topItemsPlacement);
            SplitterPage.SetTabStripPlacement(BottomItems, botItemsPlacement);
        }

        /// <summary>
        /// Updates width and height of the resized grid when resize direction = cols. It should be done for the correct resizing of the Grid
        /// where CustomGridSplitter is.
        /// </summary>
        private void UpdateResizedGridCols()
        {
            if (m_gridCols != null)
            {
                m_gridCols[0].Width = new GridLength(1, GridUnitType.Auto);
                m_gridCols[1].Width = new GridLength(1, GridUnitType.Auto);
                m_gridCols[2].Width = new GridLength(1, GridUnitType.Auto);

                m_gridRows[0].Height = new GridLength(1, GridUnitType.Auto);
                m_gridRows[1].Height = new GridLength(1, GridUnitType.Star);
                m_gridRows[2].Height = new GridLength(1, GridUnitType.Auto);

                m_resizedGrid.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Updates width and height of the resized grid when resize direction = rows. It should be done for the correct resizing of the Grid
        /// where CustomGridSplitter is.
        /// </summary>
        private void UpdateResizedGridRows()
        {
            if (m_gridCols != null)
            {
                m_gridCols[0].Width = new GridLength(1, GridUnitType.Auto);
                m_gridCols[1].Width = new GridLength(1, GridUnitType.Star);
                m_gridCols[2].Width = new GridLength(1, GridUnitType.Auto);

                m_gridRows[0].Height = new GridLength(1, GridUnitType.Auto);
                m_gridRows[1].Height = new GridLength(1, GridUnitType.Auto);
                m_gridRows[2].Height = new GridLength(1, GridUnitType.Star);

                m_resizedGrid.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Updates border that contains resize options between panels.
        /// </summary>
        /// <param name="availableWidth">available width if the split header panel.</param>
        private void UpdateResizedBorder(double availableWidth)
        {
            double borderWidth = availableWidth - TopItems.ActualWidth - BottomItems.ActualWidth - 10;
            if (m_horizContainerBorder != null)
            {
                if (borderWidth > 0)
                {
                    m_horizContainerBorder.Width = borderWidth;
                    m_horizContainerBorder.Margin = new Thickness(-SplitterMunuPanel.DesiredSize.Width + 10, 0, 0, 0);
                }
                else
                {
                    m_horizContainerBorder.Width = 0;
                }
            }
        }

        
        /// <summary>
        /// Initializes some template members if they weren`t initialized before.
        /// </summary>
        private void InitTemplateMembers()
        {
            //if (m_gridSplitter == null)
            //{
                m_gridSplitter = TemplatedParent as CustomGridSplitter;
                m_tabSplitter = m_gridSplitter.TemplatedParent as TabSplitter;
                m_horizContainerBorder = m_gridSplitter.Template.FindName(ChorizContainerBorder, m_gridSplitter) as Border;

                m_resizedGrid = m_tabSplitter.ResizedGrid;
                m_gridRows = m_resizedGrid.RowDefinitions;
                m_gridCols = m_resizedGrid.ColumnDefinitions;
            //}
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Invoked when template changed.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            InitTemplateMembers();
        }
        
        /// <summary>
        /// Measures child elements accordingly constraint size.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            InitTemplateMembers();
            if (m_gridSplitter != null)
            {
                if (m_gridSplitter.ResizeDirection == GridResizeDirection.Rows)
                {
                   if (double.IsInfinity(availableSize.Width))
                    {
                        if((m_tabSplitter!=null)&&(SplitButton!=null))
                        {
                            availableSize.Width = m_tabSplitter.ActualWidth - SplitButton.DesiredSize.Width;
                        }
                    }

                    CalculateMaxRowHeight(m_gridSplitter, availableSize);

                    if (m_gridSplitter.ActualHeight > m_gridSplitter.CustomGridSplitterHeight)
                    {
                        m_gridSplitter.Height = m_gridSplitter.CustomGridSplitterHeight;
                    }
                    if((SplitButton!=null)&&(SplitterMunuPanel!=null))
                    {
                        double availableWidth = availableSize.Width - SplitButton.DesiredSize.Width - SplitterMunuPanel.DesiredSize.Width;
                        double middleWidth = availableWidth / 2;

                        if (availableWidth < 0)
                        {
                            availableWidth = SplitButton.DesiredSize.Width + SplitterMunuPanel.DesiredSize.Width;
                            middleWidth = availableWidth / 2;
                        }
                        TopItems.Measure(new Size(middleWidth, availableSize.Height));
                        BottomItems.Measure(new Size(middleWidth, availableSize.Height));
                        UpdateResizedBorder(availableWidth);
                    }

                    UpdateResizedGridRows();
                    UpdateRowsTabStripPlacement();
                }
                else
                {
                    if (m_gridSplitter != null)
                    {
                        if (double.IsInfinity(availableSize.Width))
                        {
                            if ((m_tabSplitter != null) && (SplitButton != null))
                            {
                                availableSize.Width = m_tabSplitter.ActualWidth - SplitButton.DesiredSize.Width;
                            }
                        }
                        CalculateMaxRowHeight(m_gridSplitter, availableSize);
                        LayoutPanel panel = m_gridSplitter.Template.FindName(CsplitHeaderPanel, m_gridSplitter) as LayoutPanel;
                        panel.Height = m_rowHeight + 2;
                        if ((SplitButton != null) && (SplitterMunuPanel != null))
                        {
                            double splitButtonWidth = SplitButton.DesiredSize.Width;
                            double menuPanelWidth = SplitterMunuPanel.DesiredSize.Width;
                            double newSplitterHeight = ActualWidth - m_horizContainerBorder.ActualWidth;
                            double itemsWidth = TopItems.ActualWidth + BottomItems.ActualWidth;

                            ////double newSplitterHeight1 = TopItems.ActualWidth + BottomItems.ActualWidth + splitButtonWidth + menuPanelWidth;
                            //// if( m_tabSplitter.ActualHeight < newSplitterHeight )
                            if ((m_gridSplitter != null) && (m_tabSplitter != null))
                            {
                                m_gridSplitter.Height = panel.Width;
                                m_tabSplitter.Height = m_gridSplitter.Height + 35;
                                m_gridSplitter.VerticalAlignment = VerticalAlignment.Top;
                            }
                            double availableWidth = availableSize.Width - splitButtonWidth - menuPanelWidth;
                            UpdateResizedBorder(availableWidth);
                        }
                        UpdateResizedGridCols();
                        UpdateColsTabStripPlacement();
                    }
                }
            }
            return new Size(availableSize.Width, m_rowHeight);
        }
        
        /// <summary>
        /// Arranges child element and light-weight adorner, calculates areas of borders drawing.
        /// </summary>
        /// <param name="finalSize">Specifies the supposed size of the control.</param>
        /// <returns>Returns the actually used size. It can be larger than the initial size in case when the initial size is too small to draw all borders.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double currentWidth = 0;

            foreach (UIElement child in InternalChildren)
            {
                if (currentWidth + child.DesiredSize.Width <= finalSize.Width)
                {
                    if (child != SplitterMunuPanel)
                    {
                        child.Arrange(new Rect(currentWidth, 0, child.DesiredSize.Width, m_rowHeight));
                    }
                    else
                    {
                        child.Arrange(new Rect(finalSize.Width - child.DesiredSize.Width, 0, child.DesiredSize.Width, m_rowHeight));
                    }

                    currentWidth += child.DesiredSize.Width;
                }
                else
                {
                    child.Arrange(new Rect(currentWidth, 0, 0, 0));
                }
            }

            return new Size(finalSize.Width, m_rowHeight);
        }
        
        /// <summary>
        /// Raises the Initialized event.
        /// </summary>
        /// <param name="e"> The RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            TabSplitter parentSplitter = (TabSplitter)VisualUtils.FindAncestor(this, typeof(TabSplitter));
            if(parentSplitter!=null)
                parentSplitter.ItemsInitialization(TopItems, BottomItems);
            this.Loaded += new RoutedEventHandler(SplitHeaderPanel_Loaded);
            this.Unloaded += new RoutedEventHandler(SplitHeaderPanel_Unloaded);
        }

        void SplitHeaderPanel_Loaded(object sender, RoutedEventArgs e)
        {
            m_gridSplitter = TemplatedParent as CustomGridSplitter;
        }
        void SplitHeaderPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            m_gridSplitter = null;
        }
        #endregion
    }
}
