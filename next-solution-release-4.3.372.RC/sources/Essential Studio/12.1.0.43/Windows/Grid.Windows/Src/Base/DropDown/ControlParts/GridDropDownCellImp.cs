//-------------------------------------------------------------------------------------------------
// <copyright file="GridDropDownCellImp.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// A version of the <see cref="PopupControlContainer"/> class that implements the 
    /// <see cref="IPopupParent"/> interface specific for a <see cref="GridControlBase"/>.
    /// </summary>
    [ToolboxItem(false)]
    public class GridDropDownContainer : PopupControlContainer, IGridDropDownContainer
    {
        internal IPopupParent inner = null;
        internal GridControlBase grid;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridDropDownContainer()
            : base()
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inner = null;
                grid = null;
            }

            base.Dispose(disposing);
        }

        bool IQueryFocusInside.QueryFocusInside()
        {
            if (ContainsFocus)
            {
                return true;
            }
            else
            {
                foreach (Control c in this.Controls)
                {
                    IQueryFocusInside qfi = c as IQueryFocusInside;
                    if (qfi != null)
                    {
                        if (qfi.QueryFocusInside())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Will be called to indicate that the popup child was closed in the specified mode.
        /// </summary>
        /// <param name="childUI">The child that was closed. </param>
        /// <param name="popupCloseType">A <see cref="PopupCloseType"/> value. </param>
        void IPopupParent.ChildClosing(IPopupChild childUI, PopupCloseType popupCloseType)
        {
            if (inner != null)
            {
                inner.ChildClosing(childUI, popupCloseType);
            }
        }
        
        Point IPopupParent.GetLocationForPopupAlignment(PopupRelativeAlignment relativeAlignment, out PopupRelativeAlignment newAlignment)
        {
            if (inner == null)
            {
                newAlignment = PopupRelativeAlignment.Default;
                return Point.Empty;
            }

            return inner.GetLocationForPopupAlignment(relativeAlignment, out newAlignment);
        }

        Point[] IPopupParent.GetBorderOverlapCue(PopupRelativeAlignment relativeAlignment)
        {
            if (inner == null)
            {
                return null;
            }

            return inner.GetBorderOverlapCue(relativeAlignment);
        }
    }

    /// <summary>
    /// A helper class for drop-down cell functionality.
    /// </summary>
    public class GridDropDownCellImp : IGridDropDownCellImp, IQueryFocusInside, IPopupParent
    {
        GridControlBase grid;
        GridCellRendererBase renderer;
        GridCellButton dropdownButton;
        internal GridDropDownContainer dropdownContainer;
        IPopupParent popupParent;        

        bool initFocusEditPart = false;
        int ignoreDoubleClickTicks = int.MinValue;
        bool ignoreFocus;

        /// <summary>
        /// Gets or sets a value indicating whether the focus should remain with the grid or active text box when dropped-down. True if focus
        /// should remain with grid; False if drop-down should get focus.
        /// </summary>
        public bool InitFocusEditPart
        {
            get
            {
                return initFocusEditPart;
            }

            set
            {
                initFocusEditPart = value;
            }
        }

        /// <summary>
        /// Gets or sets the Environment.TickCount value until a second click should be considered a double click.
        /// </summary>
        public int IgnoreDoubleClickTicks
        {
            get
            {
                return ignoreDoubleClickTicks;
            }

            set
            {
                ignoreDoubleClickTicks = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether grid focus notification should be temporarily ignored.
        /// </summary>
        public bool IgnoreFocus
        {
            get
            {
                return ignoreFocus;
            }

            set
            {
                ignoreFocus = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridDropDownCellImp"/> object.
        /// </summary>
        /// <param name="renderer">The cell renderer that you want to enable for drop-down functionality.</param>
        public GridDropDownCellImp(GridCellRendererBase renderer)
        {
            this.renderer = renderer;
            this.grid = renderer.Grid;
            this.popupParent = (IPopupParent) renderer;
        }

        /// <summary>
        /// Gets a reference to the parent grid.
        /// </summary>
        public GridControlBase Grid
        {
            get
            {
                return grid;
            }
        }

        /// <summary>
        /// Gets a reference to GridCurrentCell implementation
        /// of the GridControlBase this cell renderer is associated with.
        /// </summary>
        public GridCurrentCell CurrentCell
        {
            get
            {
                return grid.CurrentCell;
            }
        }

        /// <summary>
        /// Determines if this control contains focus. Override this method if you
        /// want to show drop-down windows and indicate the control has not lost focus when 
        /// the drop-down is shown.
        /// </summary>
        /// <returns>True if the control or any child control has focus; False otherwise.</returns>
        public bool QueryFocusInside()
        {
            if (EditPart != null && EditPart.Focused)
            {
                return true;
            }
            else if (CurrentCell.IsDroppedDown)
            {
                IQueryFocusInside qfi = dropdownContainer as IQueryFocusInside;
                if (qfi != null)
                {
                    return qfi.QueryFocusInside();
                }
            }

            return false;
        }
        
        /// <summary>
        /// Unwires any events subscribed from <see cref="GridDropDownContainer"/>.
        /// </summary>
        public void Dispose()
        {
            if (this.dropdownContainer != null)
            {
                this.dropdownContainer.BeforePopup -= new CancelEventHandler(_DropDownContainerShowingDropDown);
                this.dropdownContainer.Popup -= new EventHandler(_DropDownContainerShowedDropDown);
                this.dropdownContainer.Dispose();
                this.dropdownContainer = null;

                this.grid = null;
                this.renderer = null;
                this.dropdownButton = null;
                this.dropdownContainer = null;
                this.popupParent = null;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets or sets the drop-down button.
        /// </summary>
        public GridCellButton DropDownButton
        {
            get
            {
                return dropdownButton;
            }

            set
            {
                if (value != dropdownButton)
                {
                    if (dropdownButton != null)
                    {
                        renderer.IntRemoveButton(dropdownButton);
                    }

                    dropdownButton = value;
                    if (dropdownButton != null)
                    {
                        renderer.IntAddButton(dropdownButton);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="GridDropDownContainer"/> and associates it with 
        /// the cell's parent grid.
        /// </summary>
        /// <returns>The container where you can insert child controls to be displayed as drop-down part for your cell.</returns>
        public virtual IGridDropDownContainer CreateDropDownContainer()
        {
            GridDropDownContainer container = this.InternalCreateDropDownContainer();
            container.Location = new Point(10000, 10000);
            container.PopupParent = popupParent; // if popupParent is parent renderer, notifications will be forwarded to me (backward compatibility)
            container.FakeFocus = false;
            container.grid = this.grid;
            return container;
        }

        /// <summary>
        /// Just creates and returns the GridDropDownContainer. Good method to override and return a custom container.
        /// </summary>
        /// <returns>Returns a IGridDropDownContainer.</returns>
        protected virtual GridDropDownContainer InternalCreateDropDownContainer()
        {
            return new GridDropDownContainer();
        }

        /// <summary>
        /// Ensures the container is valid and initialized. 
        /// </summary>
        public void EnsureDropDownContainer()
        {
            if (this.dropdownContainer == null)
            {
                bool focused = !this.ignoreFocus && Grid.Focused;
                this.dropdownContainer = (GridDropDownContainer) renderer.IntCreateDropDownContainer();
                ISupportsPopupControlContainer sppc = EditPart as ISupportsPopupControlContainer;
                if (sppc != null)
                {
                    sppc.PopupControlContainer = this.dropdownContainer;
                }
                    
                this.dropdownContainer.BeforePopup += new CancelEventHandler(_DropDownContainerShowingDropDown);
                this.dropdownContainer.Popup += new EventHandler(_DropDownContainerShowedDropDown);
                    
                //// Compatibility layer - give renderer first a chance to handle event and
                //// forward it back to this object.
                renderer.IntInitializeDropDownContainer();

                if (focused)
                {
                    Grid.Focus();
                }
            }
        }

        /// <summary>
        /// Called to initialize contents of the drop-down container for the first time.
        /// </summary>
        public virtual void InitializeDropDownContainer()
        {
        }

        /// <summary>
        /// This method gets called from the cell renderer's Initialize method. Override this method if you need to any initialization
        /// for the current cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public virtual void OnInitialize(int rowIndex, int colIndex)
        {
            if (dropdownButton != null)
            {
                Grid.InternalInvalidate(dropdownButton.Bounds);
            }
        }

        /// <summary>
        /// Override this method if your cell renderer supports in-place editing and you want
        /// to do any custom initialization at this point before cell gets redrawn.
        /// </summary>
        public virtual void OnHasFocusControlChanged()
        {
            if (renderer.HasFocusControl)
            {
                EnsureDropDownContainer();
            }
        }

        /// <summary>
        /// Will be called to indicate that the popup child was closed in the specified mode.
        /// </summary>
        /// <param name="childUI">The child that was closed. </param>
        /// <param name="popupCloseType">A <see cref="PopupCloseType"/> value. </param>
        /// <seealso cref="IPopupItem"/>
        public virtual void ChildClosing(IPopupChild childUI, PopupCloseType popupCloseType)
        {
            if (popupCloseType == PopupCloseType.Deactivated)
            {
                GridDropDownContainer c = (GridDropDownContainer) grid.DropDownContainerParent;
                if (c != null && c.grid != null)
                {
                    c.grid.CurrentCell.CloseDropDown(popupCloseType);
                }
            }

            //// Compatibility layer - give renderer first a chance to handle event and
            //// forward it back to this object.
            renderer.DropDownContainerCloseDropDown(childUI, new PopupClosedEventArgs(popupCloseType));
        }

        /// <summary>
        /// Will be called to indicate that the popup child was closed.
        /// </summary>
        /// <param name="sender">The child that was closed. </param>
        /// <param name="e">The event data with a <see cref="PopupClosedEventArgs.PopupCloseType"/> value. </param>
        public virtual void DropDownContainerCloseDropDown(object sender, PopupClosedEventArgs e) 
        {
            Grid.RaiseCurrentCellCloseDropDown(e);
        }

        Point IPopupParent.GetLocationForPopupAlignment(PopupRelativeAlignment prevAlign, out PopupRelativeAlignment newAlign)
        {
            int currentColIndex = renderer.ColIndex;
            int currentRowIndex = renderer.RowIndex;
            GridCellLayout layout = renderer.GetCellLayout(currentRowIndex, currentColIndex, Grid.Model[currentRowIndex, currentColIndex]);
            Rectangle cellBounds = Grid.RectangleToScreen(layout.InnerRectangle);
            return PopupUtils.ComputeDefaultTopBottomAlignment(prevAlign, out newAlign, cellBounds, Grid.IsRightToLeft());
        }

        Point[] IPopupParent.GetBorderOverlapCue(PopupRelativeAlignment relativeAlignment)
        {
            return null;
        }

        bool IPopupParent.IsRightToLeft
        {
            get
            {
                return grid.IsRightToLeft();
            }
        }

        /// <summary>
        /// Occurs after the popup has been dropped-down and made visible.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        public virtual void DropDownContainerShowedDropDown(object sender, EventArgs e)
        {
            // Ideal hook to set focus to button control or refresh parent focus.
            if (initFocusEditPart || dropdownContainer.Controls.Count == 0)
            {
                dropdownContainer.FocusParent();
            }
            else
            {
                dropdownContainer.Focus();
            }

            NotifyShowedDropDown();
        }

        void _DropDownContainerShowedDropDown(object sender, EventArgs e)
        {
            //// Compatibility layer - give renderer first a chance to handle event and
            //// forward it back to this object.
            renderer.DropDownContainerShowedDropDown(sender, e);
        }

        void _DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            // Compatibility layer - give renderer first a chance to handle event and
            // forward it back to this object.
            renderer.DropDownContainerShowingDropDown(sender, e);
        }

        /// <summary>
        /// Occurs when the drop-down container is about to be shown.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        public virtual void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            Size size = dropdownContainer.Size;
            e.Cancel = !NotifyShowingDropDown(ref size);
            dropdownContainer.Size = size;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.OnCurrentCellShowingDropDown"/> for the parent grid.
        /// </summary>
        /// <param name="size">The suggested size of the drop-down</param>
        /// <returns>True if drop-down should be shown; False if operation should be canceled.</returns>
        public bool NotifyShowingDropDown(ref Size size)
        {
            GridCurrentCellShowingDropDownEventArgs ce = new GridCurrentCellShowingDropDownEventArgs(size);
            Grid.RaiseCurrentCellShowingDropDown(ce);
            size = ce.Size;
            return !ce.Cancel;
        }
        
        /// <summary>
        /// Raises the <see cref="GridControlBase.OnCurrentCellShowedDropDown"/> for the parent grid.
        /// </summary>
        public void NotifyShowedDropDown()
        {
            Grid.RaiseCurrentCellShowedDropDown(EventArgs.Empty);
        }

        Control IPopupItem.GetPopupParentControl()
        {
            return this.Grid;
        }
        
        /// <summary>
        /// Called to find out whether a specified control is part of the popup hierarchy.
        /// </summary>
        /// <param name="control">A Control instance. </param>
        /// <param name="askPopupParent">True indicates this query should be passed to the IPopupParent, if any; False indicates you should not query the popup parent.</param>
        /// <returns>True if the control is part of the popup hierarchy; False if not.</returns>
        /// <seealso cref="IPopupItem"/>
        public virtual bool IsRelatedControl(Control control, bool askPopupParent)
        {
            if (control == this.Grid || control == this.EditPart)
            {
                return true;
            }
            else if (control is GridControlBase && ((GridControlBase)control).GetWindow() == Grid.GetWindow())
            {
                return true;
            }
            else
            { 
                return false; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether the drop-down is currently dropped-down and visible.
        /// </summary>
        public bool IsDroppedDown
        {
            get
            {
                return this.dropdownContainer != null && this.dropdownContainer.IsShowing();
            }
        }

        /// <summary>
        /// This is called from GridCurrentCell.ShowDropDown after BeginEdit has been called.
        /// </summary>
        /// <remarks>
        /// If your renderer supports dropped-down state, the drop-down window should be made
        /// visible at this time.
        /// </remarks>
        public virtual void OnShowDropDown()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (IsDroppedDown)
            {
                return;
            }

            ignoreFocus = true;
            Grid.CurrentCell.BeginEdit();
            Grid.Update();
            if (this.DropDownContainer != null) 
            {
                if (Grid.popupParent != null)
                {
                    ((GridDropDownContainer)Grid.popupParent).inner = popupParent;
                    this.DropDownContainer.PopupParent = Grid.popupParent;
                }
                else
                {
                    this.DropDownContainer.PopupParent = popupParent;
                }
               
                this.DropDownContainer.ShowPopup(Point.Empty);
            }

            ignoreFocus = false;
        }

        /// <summary>
        /// This is called from GridCurrentCell.CloseDropDown.
        /// </summary>
        /// <remarks>
        /// If your renderer supports dropped-down state, the drop-down window should be
        /// closed at this time.
        /// </remarks>
        public virtual void OnCloseDropDown(PopupCloseType reason)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (!IsDroppedDown)
            {
                return;
            }

            ignoreFocus = true;
            this.DropDownContainer.HidePopup(reason);
            ignoreFocus = false;
        }

        /// <summary>
        /// This method is called when the user clicks a cell button inside cell. 
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="button">The button</param>
        /// <remarks>In your overriden version
        /// of this method, you can activate the current cell for the given row and column index and then
        /// drop-down a list.</remarks>
        public virtual void OnButtonClicked(int rowIndex, int colIndex, int button)
        {
            if (!CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
            {
                CurrentCell.MoveTo(rowIndex, colIndex, GridSetCurrentCellOptions.None);
            }

            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
            {
                this.ignoreFocus = true;

                CurrentCell.ToggleDropDown();

                if (this.grid == null)
                {
                    return;
                }

                this.ignoreFocus = false;
                ////  Does nothing anyway - base.OnButtonClicked(rowIndex, colIndex, button).

                if (!CurrentCell.IsDroppedDown)
                {
                    this.ignoreFocus = false;
                }

                this.ignoreDoubleClickTicks = Environment.TickCount + 120;
            }
        }

        /// <summary>
        /// User pressed key down. (similar to Control.OnKeyDown)
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnKeyDown(KeyEventArgs e)  
        {
            bool bAlt = (Control.ModifierKeys & Keys.Alt) != Keys.None;

            if (e.Handled)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (bAlt)
                    {
                        e.Handled = true;
                        if (CurrentCell.Renderer.StyleInfo.Clickable)
                        {
                            CurrentCell.ToggleDropDown();
                        }

                        return;
                    }

                    break;
                case Keys.F4:
                    if (Control.ModifierKeys == Keys.None)
                    {
                        e.Handled = true;
                        if (CurrentCell.Renderer.StyleInfo.Clickable)
                        {
                            CurrentCell.ToggleDropDown();
                        }
                    }

                    return;
            }
        }

        /// <summary>
        /// Ensures that the drop-down part is the top-most window.
        /// </summary>
        public void FixTopMostWindow()
        {
            if (this.IsDroppedDown)
            {
                this.dropdownContainer.PopupHost.ShowWindowTopMost();
            }
        }

        // Properties

        /// <summary>
        /// Gets the text box that is displayed in the user input field for 
        /// a combo box. 
        /// </summary>
        public Control EditPart
        {
            get
            {
                return renderer.Control;
            }
        }

        /// <summary>
        /// Gets the container where you can insert child controls to be displayed as drop-down part for your cell.
        /// </summary>
        public IGridDropDownContainer PopupControlContainer
        {
            get
            {
                EnsureDropDownContainer();
                return this.dropdownContainer;
            }
        }
    
        /// <summary>
        /// Gets DropDownContainer. For convenience only. Same as <see cref="PopupControlContainer"/>.
        /// </summary>
        public IGridDropDownContainer DropDownContainer
        {
            get
            {
                EnsureDropDownContainer();
                return dropdownContainer;
            }
        }
    }
}
