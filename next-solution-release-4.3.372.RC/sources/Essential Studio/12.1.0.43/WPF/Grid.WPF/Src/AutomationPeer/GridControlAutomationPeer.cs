#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid.Automation.Peers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Automation;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Grid;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows;

#if SILVERLIGHT
    public class GridControlAutomationPeer : FrameworkElementAutomationPeer
#else
    public class GridControlAutomationPeer : FrameworkElementAutomationPeer, IGridProvider, ISelectionProvider, ITableProvider
#endif
    {
        #region ctor

        public GridControlAutomationPeer(GridControlBase owner)
            : base(owner)
        {
            if (owner == null)
            {
                throw new InvalidOperationException("Owner cannot be null");
            }
            this.UpdateEventsSource();
        }

        private void UpdateEventsSource()
        {
            //DataGridCell cell = (DataGridCell)Owner;
            //DataGrid dataGrid = cell.DataGridOwner;
            //if (dataGrid != null)
            //{
            //    DataGridAutomationPeer dataGridAutomationPeer = CreatePeerForElement(dataGrid) as DataGridAutomationPeer;
            //    if (dataGridAutomationPeer != null)
            //    {
            //        DataGridItemAutomationPeer itemAutomationPeer = dataGridAutomationPeer.GetOrCreateItemPeer(cell.DataContext);
            //        if (itemAutomationPeer != null)
            //        {
            //            DataGridCellItemAutomationPeer cellItemAutomationPeer = itemAutomationPeer.GetOrCreateCellItemPeer(cell.Column);
            //            this.EventsSource = cellItemAutomationPeer;
            //        }
            //    }
            //}
        }

        #endregion

        public virtual GridModel Model
        {
            get
            {
                var grid = this.Owner as GridControlBase;
                return grid.Model;
            }
        }

        public GridControlBase OwnerGrid
        {
            get
            {
                return this.Owner as GridControlBase;
            }
        }

        #region AutomationPeer overrides

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.DataGrid;
        }

        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }
#if !SILVERLIGHT
        protected override List<AutomationPeer> GetChildrenCore()
        {
            var peers = new List<AutomationPeer>();
            foreach (var kvp in this.cellPeers)
            {
                peers.Add(kvp.Value);
            }
            return peers;
        }
#endif

        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.Grid:
                case PatternInterface.Selection:
                case PatternInterface.Table:
                    return this;
                    //case PatternInterface.Scroll:
                    //    {
                    //        var scrollViewer = this.OwnerGrid.FindParentElementOfType<ScrollViewer>();
                    //        if (scrollViewer != null)
                    //        {
                    //            var scrollPeer = UIElementAutomationPeer.CreatePeerForElement(scrollViewer);
                    //            var scrollProvider = scrollPeer as IScrollProvider;
                    //            if (scrollPeer != null && scrollProvider != null)
                    //            {
                    //                scrollPeer.EventsSource = this;
                    //                return scrollProvider;
                    //            }
                    //        }
                    //        //else
                    //        //{
                    //        //    throw new InvalidOperationException("No ScrollViewer found in the parent");
                    //        //}
                    //    }
                    //break; Unreachable code
            }

            return base.GetPattern(patternInterface);
        }

        #endregion

#if !SILVERLIGHT

        #region IGridProvider Members

        int IGridProvider.ColumnCount
        {
            get
            {
                return this.Model.ColumnCount;
            }
        }

        IRawElementProviderSimple IGridProvider.GetItem(int row, int column)
        {
            if (row < this.Model.RowCount && column < this.Model.ColumnCount)
            {
                var cellPeer = this.GetOrCreateCellPeer(row, column);
                if (cellPeer != null)
                {
                    var item = this.ProviderFromPeer(cellPeer);
                    return item;
                }
            }

            return null;
        }

        Dictionary<RowColumnIndex, AutomationPeer> cellPeers = new Dictionary<RowColumnIndex, AutomationPeer>();
        internal AutomationPeer GetOrCreateCellPeer(int row, int column)
        {
            // use the IsDescendentOf method to do a quick check if the peer is connected to the reference peer
            // using reflector, I found these methods are inter-linked to generate the ElementProxy
            //System.Windows.Automation.Peers.AutomationPeer.ValidateConnected(AutomationPeer) : AutomationPeer
            //System.Windows.Automation.Peers.AutomationPeer.ProviderFromPeer(AutomationPeer) : IRawElementProviderSimple
            //MS.Internal.Automation.ElementProxy.StaticWrap(AutomationPeer, AutomationPeer) : ElementProxy
            //System.Windows.Automation.Peers.AutomationPeer.isDescendantOf(AutomationPeer) : Boolean
            var rowColIndex = new RowColumnIndex(row, column);
            AutomationPeer cellPeer = null;
            if (!this.cellPeers.TryGetValue(rowColIndex, out cellPeer))
            {
                if (!OwnerGrid.CurrentCell.HasCurrentCellAt(rowColIndex))
                {
                    // if we have a templated cell renderer
                    var cellElements = this.OwnerGrid.ArrangedCellUIElements.GetCellUIElements(rowColIndex);
                    if (cellElements != null && cellElements.UIElements.Count > 0)
                    {
                        var style = this.OwnerGrid.Model[rowColIndex.RowIndex, rowColIndex.ColumnIndex];
                        var cellElement = cellElements.UIElements[0] as FrameworkElement;
                        FrameworkElement element = default(FrameworkElement);
                        if (style.CellType == "DataTemplate" || style.CellType == "DataBoundTemplate")
                        {
                            var contentControl = cellElement as ContentControl;
                            if (contentControl != null)
                            {
                                var presenter = contentControl.FindElementOfType<ContentPresenter>();
                                element = FindAutomationElement(presenter, (el) =>
                                {
                                    var result = false;
                                    if (el != null)
                                    {
                                        result = GridControlBase.GetAutomationTemplateElement(el) == true;
                                    }

                                    return result;
                                });
                            }
                        }
                        else
                        {
                            element = cellElement;
                        }

                        if (element != null)
                        {
                            //element = (FrameworkElement)cellElements.UIElements[0];
                            cellPeer = FrameworkElementAutomationPeer.CreatePeerForElement(element);
                            if (cellPeer != null)
                            {
                                this.cellPeers.Add(rowColIndex, cellPeer);
                                this.ResetChildrenCache();
                                return cellPeer;
                            }
                        }
                    }

                    DrawingVisual dVisual;
                    this.OwnerGrid.RenderedCellVisuals.TryGetVisual(rowColIndex, out dVisual);
                    if (dVisual != null)
                    {
                        var cellElement = CreateGridCellElement(dVisual);
                        VirtualizingCellsControl.SetCellRowColumnIndex(dVisual, rowColIndex);
                        VirtualizingCellsControl.SetCellsControl(cellElement, this.OwnerGrid);
                        //if (!this.OwnerGrid.AutomationFrame.Children.Contains(dVisual))
                        //{
                        //this.OwnerGrid.AutomationFrame.AddAutomationElement(rowColIndex, cellElement);//.Children.Add(cellElement);
                        //}
                        cellPeer = CreatePeerForElement(cellElement) as GridCellElementAutomationPeer;
                        this.cellPeers.Add(rowColIndex, cellPeer);
                        this.ResetChildrenCache();
                        return cellPeer;
                    }
                }
                else
                {
                    // request for automation peer from the UIElement thru the IGridCellRenderer interface
                    var peer = this.OwnerGrid.CurrentCell.Renderer.OnCreateAutomationPeer();
                    if (peer != null)
                    {
                        this.cellPeers.Add(rowColIndex, peer);
                    }
                }
            }

            // Not sure if this is a workaround, for ensuring proper children elements we just reset the cache, this would call the GetChildrenCore() overload
            this.ResetChildrenCache();
            return cellPeer;
        }

        protected virtual GridCellElement CreateGridCellElement(DrawingVisual dVisual)
        {
            var cellElement = new GridCellElement(dVisual);
            return cellElement;
        }

        private static FrameworkElement FindAutomationElement(FrameworkElement element, Func<FrameworkElement, bool> predicate)
        {
            var result = predicate(element);
            if (result)
            {
                return element;
            }

            if (element != null)
            {
                int numChildren = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0;i < numChildren;i++)
                {
                    var child = FindAutomationElement(VisualTreeHelper.GetChild(element, i) as FrameworkElement, predicate);
                    if (child != null)
                    {
                        return child;
                    }
                }
            }

            return null;
        }

        int IGridProvider.RowCount
        {
            get
            {
                return this.Model.RowCount;
            }
        }

        #endregion

        #region ISelectionProvider Members

        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                var result = this.Model.Options.AllowSelection != GridSelectionFlags.None;
                return result;
            }
        }

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            // since the grid is virtualized, we can only get the visible cells in selection, if the grid is scrolled, then this method has to be called again            
            var selectedProviders = new List<IRawElementProviderSimple>();
            var model = this.OwnerGrid.Model;
            var scrollRows = this.OwnerGrid.ScrollRows;
            var scrollColumns = this.OwnerGrid.ScrollColumns;
            for (int n = 0;n < model.SelectedRanges.Count;n++)
            {
                GridRangeInfo selectedCells = model.SelectedRanges[n];
                if (!selectedCells.IsEmpty)
                {
                    selectedCells = this.OwnerGrid.ExpandSelectedCellsRange(selectedCells);
                    for (int top = selectedCells.Top;top <= (selectedCells.Bottom < scrollRows.LastBodyVisibleLineIndex ? selectedCells.Bottom : scrollRows.LastBodyVisibleLineIndex);top++)
                    {
                        for (int left = selectedCells.Left;left <= (selectedCells.Right < scrollColumns.LastBodyVisibleLineIndex ? selectedCells.Right : scrollColumns.LastBodyVisibleLineIndex);left++)
                        {
                            //Console.WriteLine("{0} / {1}", top, left);
                            var cellPeer = this.GetOrCreateCellPeer(top, left);
                            if (cellPeer != null)
                            {
                                var item = this.ProviderFromPeer(cellPeer);
                                if (item != null)
                                {
                                    selectedProviders.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            return selectedProviders.ToArray();
        }

        bool ISelectionProvider.IsSelectionRequired
        {
            get
            {
                return this.Model.Options.AllowSelection != GridSelectionFlags.None;
            }
        }

        #endregion

        #region ITableProvider Members

        IRawElementProviderSimple[] ITableProvider.GetColumnHeaders()
        {
            var columnHeaders = new List<IRawElementProviderSimple>();
            var model = this.OwnerGrid.Model;
            if (model.HeaderRows > 0)
            {
                for (int i = 0;i < model.HeaderRows;i++)
                {
                    for (int j = 0;j < model.ColumnCount;j++)
                    {
                        var cellPeer = this.GetOrCreateCellPeer(i, j);
                        if (cellPeer != null)
                        {
                            //if (cellPeer.IsDescendantOf(this))
                            //{
                            var provider = this.ProviderFromPeer(cellPeer);
                            columnHeaders.Add(provider);
                            //}
                        }
                    }
                }
            }
            return columnHeaders.ToArray();
        }

        IRawElementProviderSimple[] ITableProvider.GetRowHeaders()
        {
            var rowHeaders = new List<IRawElementProviderSimple>();
            var model = this.OwnerGrid.Model;
            if (model.HeaderRows > 0)
            {
                for (int i = 0;i < model.RowCount;i++)
                {
                    for (int j = 0;j < model.HeaderColumns;j++)
                    {
                        var item = this.GetOrCreateCellPeer(i, j);
                        var provider = this.ProviderFromPeer(item);
                        rowHeaders.Add(provider);
                    }
                }
            }
            return rowHeaders.ToArray();
        }

        RowOrColumnMajor ITableProvider.RowOrColumnMajor
        {
            get
            {
                var model = this.OwnerGrid.Model;
                RowOrColumnMajor rowColumnMajor = RowOrColumnMajor.Indeterminate;
                switch (model.Options.AllowSelection)
                {
                    case GridSelectionFlags.Column:
                        rowColumnMajor = RowOrColumnMajor.ColumnMajor;
                        break;
                    case GridSelectionFlags.Row:
                        rowColumnMajor = RowOrColumnMajor.RowMajor;
                        break;
                    default:
                        rowColumnMajor = RowOrColumnMajor.Indeterminate;
                        break;
                }

                return rowColumnMajor;
            }
        }

        #endregion
#endif
    }

    internal static class GridControlAutomationExtensions
    {
        public static bool IsDescendantOf(this AutomationPeer thisPeer, AutomationPeer parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            List<AutomationPeer> children = parent.GetChildren();
            if (children != null)
            {
                int count = children.Count;
                for (int i = 0;i < count;i++)
                {
                    AutomationPeer peer = children[i];
                    if ((peer == thisPeer) || thisPeer.IsDescendantOf(peer))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
