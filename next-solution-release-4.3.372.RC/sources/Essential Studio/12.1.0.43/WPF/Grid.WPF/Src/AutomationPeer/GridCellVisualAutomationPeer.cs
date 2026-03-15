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
    using Syncfusion.Windows.Controls.Cells;
    using System.Windows.Automation.Provider;
    using System.Windows.Automation;
    using System.Windows;
    using System.Windows.Media;

    public class GridCell : UIElement
    {
        public GridCell(RowColumnIndex rowColIndex, DrawingVisual visual)
        {
            this.RowColumnIndex = rowColIndex;
            this.RenderedVisual = visual;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawDrawing(this.RenderedVisual.Drawing);
        }

        public DrawingVisual RenderedVisual
        {
            get;
            private set;
        }

        public RowColumnIndex RowColumnIndex
        {
            get;
            private set;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new GridCellVisualAutomationPeer(this);
        }
    }

    public class GridCellVisualAutomationPeer : UIElementAutomationPeer, IValueProvider, IGridItemProvider
    {
        public GridCellVisualAutomationPeer(GridCell cell)
            : base(cell)
        {
        }

        public GridCell OwnerCell
        {
            get
            {
                return this.Owner as GridCell;
            }
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        #region IValueProvider Members

        bool IValueProvider.IsReadOnly
        {
            get { return false; }
        }

        void IValueProvider.SetValue(string value)
        {
            //var style = this.Model[this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex];
            //style.CellValue = value;
            //this.OwnerCell.InvalidateCell(this.RowColumnIndex);
        }

        string IValueProvider.Value
        {
            get
            {
                //var style = this.Model[this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex];
                //return style.CellValue != null ? style.CellValue.ToString() : string.Empty;
                return string.Empty;
            }
        }

        #endregion

        #region IGridItemProvider Members

        int IGridItemProvider.Column
        {
            get
            {
                return this.OwnerCell.RowColumnIndex.ColumnIndex;
            }
        }

        int IGridItemProvider.ColumnSpan
        {
            get
            {
                return this.OwnerCell.RowColumnIndex.ColumnIndex;
            }
        }

        private IRawElementProviderSimple ContainingGrid
        {
            get
            {
                AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(this.OwnerCell);
                if (peer != null)
                {
                    return ProviderFromPeer(peer);
                }

                return null;
            }
        }

        IRawElementProviderSimple IGridItemProvider.ContainingGrid
        {
            get
            {
                return this.ContainingGrid;
            }
        }

        int IGridItemProvider.Row
        {
            get
            {
                return this.OwnerCell.RowColumnIndex.RowIndex;
            }
        }

        int IGridItemProvider.RowSpan
        {
            get
            {
                return this.OwnerCell.RowColumnIndex.RowIndex;
            }
        }

        #endregion
    }
}
