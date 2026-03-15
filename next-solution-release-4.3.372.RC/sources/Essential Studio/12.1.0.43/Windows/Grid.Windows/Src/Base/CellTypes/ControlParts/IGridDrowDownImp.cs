//-------------------------------------------------------------------------------------------------
// <copyright file="IGridDrowDownImp.cs" company="syncfusion">
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
    /// Provides a <see cref="EnsureDropDownContainer"/> method that ensures that a drop-down container is correctly initialized.
    /// </summary>
    public interface IGridDropDownCell
    {
        /// <summary>
        /// Ensures that a drop-down container is correctly initialized.
        /// </summary>
        void EnsureDropDownContainer();
    }

    /// <summary>
    /// A interface with base method for <see cref="GridDropDownContainer"/> to use without
    /// having strong reference to that class.
    /// </summary>
    public interface IGridDropDownContainer : /*IPopupControlContainer,*/ IPopupChild, IPopupParent, IQueryFocusInside
    {
        /// <summary>
        /// Gets or sets the <see cref="IPopupParent"/> parent.
        /// </summary>
        /// <value>An instance of the <see cref="IPopupParent"/> interface.</value>
        /// <remarks>
        /// The Popup framework can handle a hierarchy of popups (like
        /// in a menu) for which it requires each popup child to provide
        /// a reference to its Popup parent.
        /// </remarks>
        new IPopupParent PopupParent { get; set; }

        /// <summary>
        /// Shows the popup at the specified location.
        /// </summary>
        /// <param name="pt">A point in screen coordinates.
        /// Can be Point.Empty.</param>
        void ShowPopup(Point pt);

        /// <summary>
        /// Gets or sets the PopupControlContainer's Control Parent.
        /// </summary>
        /// <value>A control instance.</value>
        /// <remarks>
        /// <para>The Parent-Child relationship in this case is NOT similar
        /// to the one in the control hierarchy.</para>
        /// <para>
        /// When you specify a Parent control via
        /// ParentControl and pass a Point.Empty location to
        /// ShowPopup, the popup location will be dynamically determined
        /// based on the ParentControl bounds and the screen area.</para>
        /// </remarks>
        Control ParentControl { get; set; }
    }

    /// <summary>
    /// Provides support for <see cref="PopupControlContainer"/> property.
    /// </summary>
    public interface ISupportsPopupControlContainer
    {
        /// <summary>
        /// Gets or sets The container this child control is associated with.
        /// </summary>
        IGridDropDownContainer PopupControlContainer { get; set; }
    }

    /// <summary>
    /// A helper interface for drop-down cell functionality. The GridDropDownCellImp class
    /// implements this interface.
    /// </summary>
    public interface IGridDropDownCellImp : IDisposable, IQueryFocusInside, IPopupParent
    {
        /// <summary>
        /// Gets or sets a value indicating whether the focus should remain with the grid or active text box when dropped-down. True if focus
        /// should remain with grid; False if drop-down should get focus.
        /// </summary>
        bool InitFocusEditPart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Environment.TickCount value until a second click should be considered a double click.
        /// </summary>
        int IgnoreDoubleClickTicks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether grid focus notification should be temporarily ignored.
        /// </summary>
        bool IgnoreFocus { get; set; }

        /// <summary>
        /// Gets a reference to the parent grid.
        /// </summary>
        GridControlBase Grid { get; }

        /// <summary>
        /// Gets a reference to GridCurrentCell implementation
        /// of the GridControlBase this cell renderer is associated with.
        /// </summary>
        GridCurrentCell CurrentCell { get; }

        /// <summary>
        /// Gets or sets the drop-down button.
        /// </summary>
        GridCellButton DropDownButton { get; set; }

        /// <summary>
        /// Creates a <see cref="IGridDropDownContainer"/> and associates it with
        /// the cells parent grid.
        /// </summary>
        /// <returns>The container where you can insert child controls to be displayed as a drop-down part for your cell.</returns>
        IGridDropDownContainer CreateDropDownContainer();

        /// <summary>
        /// Ensures the container is valid and initialized.
        /// </summary>
        void EnsureDropDownContainer();

        /// <summary>
        /// Called to initialize contents of the drop-down container for the first time.
        /// </summary>
        void InitializeDropDownContainer();

        /// <summary>
        /// This method gets called from the cell renderer's initialize method. Override this method if you need to do any initialization
        /// for the current cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        void OnInitialize(int rowIndex, int colIndex);

        /// <summary>
        /// Override this method if your cell renderer supports in-place editing and you want
        /// to do any custom initialization at this point before cell gets redrawn.
        /// </summary>
        void OnHasFocusControlChanged();

        /// <summary>
        /// Will be called to indicate that the popup child was closed.
        /// </summary>
        /// <param name="sender">The child that was closed. </param>
        /// <param name="e">The event data with a <see cref="PopupClosedEventArgs.PopupCloseType"/> value. </param>
        void DropDownContainerCloseDropDown(object sender, PopupClosedEventArgs e);

        /// <summary>
        /// Occurs after the popup has been dropped-down and made visible.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        void DropDownContainerShowedDropDown(object sender, EventArgs e);

        /// <summary>
        /// Occurs when the drop-down container is about to be shown.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        void DropDownContainerShowingDropDown(object sender, CancelEventArgs e);

        /// <summary>
        /// Raises the <see cref="GridControlBase.OnCurrentCellShowingDropDown"/> for the parent grid.
        /// </summary>
        /// <param name="size">The suggested size of the drop-down</param>
        /// <returns>True if drop-down should be shown; False if operation should be canceled.</returns>
        bool NotifyShowingDropDown(ref Size size);

        /// <summary>
        /// Raises the <see cref="GridControlBase.OnCurrentCellShowedDropDown"/> for the parent grid.
        /// </summary>
        void NotifyShowedDropDown();

        /// <summary>
        /// Gets a value indicating whether the drop-down is currently dropped-down and visible.
        /// </summary>
        bool IsDroppedDown { get; }

        /// <summary>
        /// This is called from GridCurrentCell.ShowDropDown after BeginEdit has been called.
        /// </summary>
        /// <remarks>
        /// If your renderer supports dropped-down state, the drop-down window should be made
        /// visible at this time.
        /// </remarks>
        void OnShowDropDown();

        /// <summary>
        /// This is called from GridCurrentCell.CloseDropDown.
        /// </summary>
        /// <remarks>
        /// If your renderer supports dropped-down state, the drop-down window should be
        /// closed at this time.
        /// </remarks>
        void OnCloseDropDown(PopupCloseType reason);

        /// <summary>
        /// This method is called when the user clicks a cell button inside a cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="button">The button control</param>
        /// <remarks>In your overriden version
        /// of this method, you can activate the current cell for the given row and column index and then
        /// drop-down a list.</remarks>
        void OnButtonClicked(int rowIndex, int colIndex, int button);

        /// <summary>
        /// User pressed key down. (similar to Control.OnKeyDown)
        /// </summary>
        /// <param name="e">The event args of Key down</param>
        void OnKeyDown(KeyEventArgs e);

        /// <summary>
        /// Ensures that the drop-down part is the top-most window.
        /// </summary>
        void FixTopMostWindow();

        /// <summary>
        /// Gets the text box that is displayed in the user input field for
        /// a combo box.
        /// </summary>
        Control EditPart { get; }

        /// <summary>
        /// Gets the container where you can insert child controls to be displayed as drop-down part for your cell.
        /// </summary>
        IGridDropDownContainer PopupControlContainer { get; }

        /// <summary>
        /// Gets the container same as <see cref="PopupControlContainer"/>, for convenience only.
        /// </summary>
        IGridDropDownContainer DropDownContainer { get; }
    }
}
