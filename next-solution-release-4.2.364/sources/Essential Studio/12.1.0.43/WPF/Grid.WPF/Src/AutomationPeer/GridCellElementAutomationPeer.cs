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
    using Syncfusion.Windows.Controls.Cells;
    using System.Windows.Media;

    public class GridCellElementAutomationPeer : FrameworkElementAutomationPeer, IGridItemProvider, IValueProvider, ISelectionItemProvider, IScrollItemProvider
    {
        public GridCellElementAutomationPeer(GridCellElement cell)
            : base(cell)
        {
        }

        public GridCellElement Cell
        {
            get
            {
                return this.Owner as GridCellElement;
            }
        }

        private GridControlBase ownerGrid;
        public GridControlBase OwnerGrid
        {
            get
            {
                if (this.ownerGrid == null)
                {
                    this.ownerGrid = VirtualizingCellsControl.GetCellsControl(this.Cell) as GridControlBase;
                }

                return this.ownerGrid;
            }
        }

        #region AutomationPeer overrides

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.DataItem;
        }

        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.GridItem:
                case PatternInterface.Value:
                case PatternInterface.SelectionItem:
                case PatternInterface.ScrollItem:
                    return this;
            }

            return base.GetPattern(patternInterface);
        }

        #endregion

        #region IGridItemProvider Members

        private RowColumnIndex rowColIndex = RowColumnIndex.Empty;
        protected RowColumnIndex RowColumnIndex
        {
            get
            {
                if (rowColIndex == RowColumnIndex.Empty)
                {
                    this.rowColIndex = VirtualizingCellsControl.GetCellRowColumnIndex(this.Cell.ContentVisual);
                }

                return this.rowColIndex;
            }
        }

        int IGridItemProvider.Column
        {
            get
            {
                return this.RowColumnIndex.ColumnIndex;
            }
        }

        int IGridItemProvider.ColumnSpan
        {
            get
            {
                if (this.OwnerGrid != null)
                {
                    var grid = this.OwnerGrid;
                    if (grid.Model != null)
                    {
                        var cc = grid.Model.CoveredCells.GetCoveredCell(this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex);
                        if (cc != null)
                        {
                            return cc.Right;
                        }
                    }
                }
                return 1;
            }
        }

        IRawElementProviderSimple IGridItemProvider.ContainingGrid
        {
            get
            {
                var peer = FrameworkElementAutomationPeer.CreatePeerForElement(this.OwnerGrid);
                return this.ProviderFromPeer(peer);
            }
        }

        int IGridItemProvider.Row
        {
            get
            {
                return this.RowColumnIndex.RowIndex;
            }
        }

        int IGridItemProvider.RowSpan
        {
            get
            {
                if (this.OwnerGrid != null)
                {
                    var grid = this.OwnerGrid;
                    var model = grid.Model;
                    var cc = model.CoveredCells.GetCoveredCell(this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex);
                    if (cc != null)
                    {
                        return cc.Height;
                    }
                }
                return 1;
            }
        }

        #endregion

        #region IValueProvider Members

        bool IValueProvider.IsReadOnly
        {
            get
            {
                var grid = this.OwnerGrid;
                if (grid != null)
                {
                    var model = grid.Model;
                    var style = model[this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex];
                    return !style.Enabled;
                }

                return false;
            }
        }

        void IValueProvider.SetValue(string value)
        {
            var grid = this.OwnerGrid;
            if (grid != null)
            {
                var style = grid.Model[this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex];
                style.Text = value;
            }
        }

        string IValueProvider.Value
        {
            get
            {
                var grid = this.OwnerGrid;
                if (grid != null)
                {
                    var model = grid.Model;
                    var style = model[this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex];
                    return style.Text;
                }

                return string.Empty;
            }
        }

        #endregion

        #region ISelectionItemProvider Members

        void ISelectionItemProvider.AddToSelection()
        {
            var model = this.OwnerGrid.Model;
            var cellRange = GridRangeInfo.Cell(this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex);
            if (!model.SelectedRanges.Contains(cellRange))
            {
                model.SelectedRanges.Add(cellRange);
            }
        }

        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                var model = this.OwnerGrid.Model;
                var cellRange = GridRangeInfo.Cell(this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex);
                return model.SelectedRanges.Contains(cellRange);
            }
        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
            //var model = this.OwnerGrid.Model; Unused local variable
            //var cellRange = GridRangeInfo.Cell(this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex);
            // TODO - Add remove from selection support by intersecting the range and removing one particular cell
        }

        void ISelectionItemProvider.Select()
        {
            var model = this.OwnerGrid.Model;
            model.SelectedRanges.Clear();
            var selectionItemProvider = this as ISelectionItemProvider;
            selectionItemProvider.AddToSelection();
        }

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                var peer = FrameworkElementAutomationPeer.CreatePeerForElement(this.OwnerGrid);
                return this.ProviderFromPeer(peer);
            }
        }

        #endregion

        #region IScrollItemProvider Members

        void IScrollItemProvider.ScrollIntoView()
        {
            if (!this.RowColumnIndex.IsEmpty)
            {
                this.OwnerGrid.ScrollInView(this.RowColumnIndex);
            }
        }

        #endregion
    }
}
