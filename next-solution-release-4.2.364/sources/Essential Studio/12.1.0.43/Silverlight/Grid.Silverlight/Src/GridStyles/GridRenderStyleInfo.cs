#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System;

#if !WinRT
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Styles;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    // TODO: Review GridRenderStyleInfo.Dispose and GridControlBase.GetRenderStyleInfo


    /// <summary>
    /// GridRenderStyleInfo can be attached to a GridRenderer.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridRenderStyleInfo : GridStyleInfo, IStyleChanged, IAllowInvalidateCell
    {
        GridControlBase gridControl;
        GridStyleInfo modelStyle;
        WeakReference weakReference;
        bool allowInvalidateCell;

        #region Ctor
        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoIdentity"/> that holds the indentity for this <see cref="GridStyleInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="GridStyleInfoStore"/> that holds data for this <see cref="GridStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridStyleInfoStore"/> object.
        /// </param>
        public GridRenderStyleInfo(GridControlBase gridControl, GridStyleInfo modelStyle)
            : base(new GridStyleInfoIdentity(modelStyle.CellIdentity.Data, modelStyle.CellIdentity.CellRowColumnIndex, true), (GridStyleInfoStore)modelStyle.Store.Clone())
        {
            // GridStyleInfoIdentity is offline. Dispose will not call ResetItem.
            this.CacheValues = true;
            this.modelStyle = modelStyle;
            this.gridControl = gridControl;
            weakReference = new WeakReference(this);
            this.modelStyle.WeakReferenceChangedListeners.Add(weakReference);
        }

        void IStyleChanged.StyleChanged(StyleChangedEventArgs e)
        {
            SetStore((GridStyleInfoStore)modelStyle.Store.Clone());
            OnStyleChanged(e.Sip);

            // IStyleChanged.StyleChanged is called from ModelStyle. So,
            // it should clear cached style, cell visuals and cell uielements
            // for this cell. PrepareRenderCell will be called again.
            InvalidateCell(e.Sip);
        }

        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            // OnStyleChanged could be called from PrepareRenderCell.
            // Therefore check AllowInvalidateCell which is set
            // in GridControlRenderStyles.GetRenderStyleInfo
            // before PrepareRenderCell event is raised.
            if (AllowInvalidateCell && !Updating)
                InvalidateCell(sip);

            base.OnStyleChanged(sip);
        }

        private void InvalidateCell(StyleInfoProperty sip)
        {
            if (sip != null && sip.DoNotInvalidateCellWhenChanged)
                return;

            if (gridControl.CurrentCell.IsInConfirmChanges
                && gridControl.CurrentCell.CellRowColumnIndex == CellRowColumnIndex)
            {
                // do nothing - let CellRenderers OnDeactivated and OnEditingComplete decide
                // whether to invalidate cell or not.
            }
            else
            {
                gridControl.ArrangedCellUIElements.Invalidate(CellIdentity.CellRowColumnIndex);
                gridControl.RenderStyles.Clear(CellIdentity.CellRowColumnIndex);
            }
            gridControl.InvalidateVisual(false);
        }

        #endregion

        public GridRenderStyleInfo Copy()
        {
            return new GridRenderStyleInfo(GridControl, this);
        }

        #region Properties

        public GridControlBase GridControl
        {
            get { return gridControl; }
        }

        /// <summary>
        /// give access to underlying model style. use it to apply changes to style 
        /// (.e.g. in gridControl textbox: modelstyle.cellvalue = ...)
        /// </summary>
        public GridStyleInfo ModelStyle
        {
            get { return modelStyle; }
        }

        public IGridCellRenderer CellRenderer
        {
            get
            {
                return gridControl.CellRenderers[CellType];
            }
        }

        public bool AllowInvalidateCell
        {
            get { return allowInvalidateCell; }
            set { allowInvalidateCell = value; }
        }

        #endregion

        public override void Dispose()
        {
            this.modelStyle.WeakReferenceChangedListeners.Remove(weakReference);

            // GridStyleInfoIdentity is offline. Dispose will not call ResetItem.
            base.Dispose();
        }


    }

}
