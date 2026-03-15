#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Provides a method to create a copy of an existing splitter pane or window.
	/// </summary>
    public interface ICreateNewWindow
    {
		/// <summary>
		/// Creates a copy of an existing splitter pane or window.
		/// </summary>
		/// <param name="row">The row in a <see cref="SplitterControl"/>.</param>
		/// <param name="column">The column in a <see cref="SplitterControl"/>.</param>
		/// <param name="parent">A reference to the parent control, e.g a splitter frame.</param>
		/// <returns>A new instance of a control.</returns>
		Control CreateNewControl(Control parent, int row, int column);
    }

	
	/// <summary>
	/// Defines split behavior for a <see cref="SplitterControl"/>.
	/// </summary>
	[Flags]
	public enum DynamicSplitBars
	{
		/// <summary>
		/// No dynamic splitter.
		/// </summary>
		None = 0,

		/// <summary>
		/// Split rows vertically.
		/// </summary>
		SplitRows = 1,

		/// <summary>
		/// Split columns horizontally.
		/// </summary>
		SplitColumns = 2,

		/// <summary>
		/// Allow both splitting the view vertically and horizontally.
		/// </summary>
		Both = 3
	}

	/// <summary>
	/// Defines an interface for a control that supports splitting the view into
	/// several row panes and column panes.
	/// </summary>
	/// <remarks>
	/// Both <see cref="SplitterControl"/> and <see cref="TabBarPage"/>
	/// implement this interface. This gives client controls a one stop interface
	/// to get all splitter functionality no matter if they are embedded inside
	/// a <see cref="SplitterControl"/> or <see cref="TabBarPage"/> inside a <see cref="TabBarSplitterControl"/>.
	/// </remarks>
	public interface IDynamicSplitterFrame
	{
		/// <summary>
		/// Gets / sets a value indicating what split behavior is supported. Rows, Columns or Both.
		/// </summary>
		DynamicSplitBars SplitBars { get; }

		/// <summary>
		/// Returns the number of visible row panes.
		/// </summary>
		int RowCount { get; }

		/// <summary>
		/// Returns the number of visible column panes.
		/// </summary>
		int ColumnCount { get; }

		/// <summary>
		/// Indicates whether the rows were split at the given y coordinate.
		/// </summary>
		/// <param name="cy">The vertical position in percentages of the splitter control's height.</param>
		/// <returns>True if rows were split successfully; False if they were already split or the operation aborted.</returns>
		bool SplitRow(int cy);

		/// <summary>
		/// Indicates whether the columns were split horizontally at the specified x coordinate.
		/// </summary>
		/// <param name="cx">The horizontal position in percentages of the splitter control's width.</param>
		/// <returns>True if columns were split successfully; False if they were already split or the operation aborted.</returns>
		bool SplitColumn(int cx);

		/// <summary>
		/// Deletes the splitter panes at the specified row.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		void DeleteRow(int row);

		/// <summary>
		/// Deletes the splitter panes at the specified column.
		/// </summary>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		void DeleteColumn(int column);

		/// <summary>
		/// Returns the splitter pane at the specified row and column.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		/// <returns>The control at the pane.</returns>
		Control GetPane(int row, int column);

		/// <summary>
		/// Returns the row and column index for a child pane.
		/// </summary>
		/// <param name="control">The control to search for.</param>
		/// <param name="row">A placeholder where the row is returned.</param>
		/// <param name="column">A placeholder where the column is returned.</param>
		/// <returns>True if the control is a pane; False if the control was not a child pane.</returns>
		bool FindPane(Control control, out int row, out int column);

		/// <summary>
		/// Gets / sets the active pane in the splitter control.
		/// </summary>
		Control ActivePane { get; set; }

		/// <summary>
		/// Sets the active pane in the splitter control specified by row and column.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		void SetActivePane(int row, int column);

		/// <summary>
		/// Indicates whether there is a next or previous pane that can be activated.
		/// </summary>
		/// <param name="prev">True if previous pane should be activated; False if next pane should be activated.</param>
		/// <returns>True if activating next or previous pane is good; False if already at last or first pane.</returns>
		bool CanActivateNext(bool prev);

		/// <summary>
		/// Activates the next or previous pane.
		/// </summary>
		/// <param name="prev">True if previous pane should be activated; False if next pane should be activated.</param>
		void ActivateNext(bool prev);

		/// <summary>
		/// Occurs when the <see cref="SplitBars"/> property has changed.
		/// </summary>
		event EventHandler SplitBarsChanged;

		/// <summary>
		/// Occurs when the vertical splitter position has changed.
		/// </summary>
		event EventHandler VSplitPosChanged;

		/// <summary>
		/// Occurs when the horizontal splitter position has changed.
		/// </summary>
		event EventHandler HSplitPosChanged;

		/// <summary>
		/// Occurs when the splitter layout has changed.
		/// </summary>
		event EventHandler SplitterLayoutChanged;
	}


	/// <summary>
	/// Provides a <see cref="FillSplitterPane"/> property support for using the control
	/// inside a dynamic splitter window and sharing scrollbars
	/// with the parent window.
	/// </summary>
	public interface ISplitterPaneSupport
	{
		/// <summary>
		/// Toggles support for using the control inside a dynamic splitter window and sharing scrollbars
		/// with the parent window.
		/// </summary>
		bool FillSplitterPane { get; }

		/// <summary>
		/// Indicates that the splitter control is closing the pane with this control.
		/// </summary>
		void PaneClosing();

		/// <summary>
		/// Indicates that the splitter control has closed the pane with this control.
		/// </summary>
		void PaneClosed();

		/// <summary>
		/// Indicates whether the splitter control is closing the pane with this control.
		/// </summary>
		bool IsSplitterPaneClosing { get; }

		/// <summary>
		/// Indicates whether the splitter control has closed the pane with this control.
		/// </summary>
		bool IsSplitterPaneClosed { get; }
	}


}
