//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelEvents.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Represents a method that handles a <see cref="GridModel.QueryCellText"/>, <see cref="GridModel.SaveCellText"/>, 
    /// <see cref="GridModel.QueryCellFormattedText"/>, or <see cref="GridModel.SaveCellFormattedText"/> event.
    /// </summary>
    public delegate void GridCellTextEventHandler(object sender, GridCellTextEventArgs e);

    /// <summary>
    /// Provides event data for the <see cref="GridModel.QueryCellText"/>, <see cref="GridModel.SaveCellText"/>, 
    /// <see cref="GridModel.QueryCellFormattedText"/>, or <see cref="GridModel.SaveCellFormattedText"/> event.
    /// </summary>
    /// <remarks>
    /// If you want to customize the grid's behavior, you should set <see cref="SyncfusionHandledEventArgs.Handled"/> 
    /// to True. The grid will check this flag to see whether it should accept your modification 
    /// or use a conversion.
    /// <para/>
    /// If you need identity information about the cell such as row and column index, you can get that
    /// information by querying <see cref="GridStyleInfo.CellIdentity"/> of the <see cref="GridCellTextEventArgs.Style"/>
    /// object.
    /// <para/>
    /// The <see cref="GridModel.SaveCellFormattedText"/> and <see cref="GridModel.SaveCellText"/> events
    /// expect that you save the resulting value in <see cref="GridStyleInfo.CellValue"/> of the <see cref="GridCellTextEventArgs.Style"/>
    /// object.
    /// <para/>
    /// The <see cref="GridModel.QueryCellFormattedText"/> and <see cref="GridModel.QueryCellText"/> events
    /// expect that you save the resulting string in <see cref="GridCellTextEventArgs.Text"/>.
    /// <para/>
    /// The <see cref="TextInfo"/> is only used for  <see cref="GridModel.SaveCellFormattedText"/> and
    /// <see cref="GridModel.QueryCellFormattedText"/>.
    /// </remarks>
    /// <seealso cref="GridCellTextEventHandler"/>
    /// <seealso cref="GridCellTextEventArgs"/>
    /// <seealso cref="GridModel.SaveCellFormattedText"/>
    /// <seealso cref="GridModel.QueryCellFormattedText"/>
    /// <seealso cref="GridCellModelBase.ApplyFormattedText"/>
    /// <seealso cref="GridStyleInfo.FormattedText"/>
    public sealed class GridCellTextEventArgs : SyncfusionHandledEventArgs
    {
        string text;
        GridStyleInfo style;
        object value;
        int textInfo;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="text">The string that represents the underlying cell value.</param>
        /// <param name="style">The style object.</param>
        /// <param name="value">The cell value.</param>
        /// <param name="textInfo"> textInfo is a hint where the call originated, e.g. GridCellBaseTextInfo.DisplayText.</param>
        public GridCellTextEventArgs(string text, GridStyleInfo style, object value, int textInfo)
        {
            this.text = text;
            this.style = style;
            this.value = value;
            this.textInfo = textInfo;
        }

        /// <summary>
        /// Gets or sets the string that represents the underlying cell value.
        /// </summary>
        [TraceProperty(true)]
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        /// <summary>
        /// Gets the style object.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        /// Gets the cell value.
        /// </summary>
        [TraceProperty(true)]
        public object Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// Gets textInfo is a hint where the call originated, e.g. GridCellBaseTextInfo.DisplayText.
        /// </summary>
        [TraceProperty(true)]
        public int TextInfo
        {
            get
            {
                return textInfo;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.QueryCanMergeCells"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridQueryCanMergeCellsEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryCanMergeCellsEventHandler(object sender, GridQueryCanMergeCellsEventArgs e);
    
    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryCanMergeCells"/> event.
    /// </summary>
    /// <remarks>
    /// The GridQueryCanMergeCellsEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.QueryCanMergeCells"/> event
    /// when the model tries to find out whether two neighboring cells can be merged.
    /// <para/>
    /// You can customize the default comparison behavior of the grid and set <see cref="Result"/>
    /// to True when cells should be merged. You should also set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The
    /// grid will the use the result provided through the <see cref="Result"/> property.
    /// </remarks>
    /// <seealso cref="GridQueryCanMergeCellsEventHandler"/>
    /// <seealso cref="GridModel.QueryCanMergeCells"/>
    /// <seealso cref="GridModelOptions.MergeCellsMode"/>
    public sealed class GridQueryCanMergeCellsEventArgs : SyncfusionHandledEventArgs
    {
        GridStyleInfo style1;
        GridStyleInfo style2;
        bool result;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="style1">The first style object. Use the style object's <see cref="GridStyleInfo.CellIdentity"/> to find
        /// out about row and column index of the cell.</param>
        /// <param name="style2">The second style object. Use the style object's <see cref="GridStyleInfo.CellIdentity"/> to find
        /// out about row and column index of the cell.</param>
        /// <param name="result">The result that should be returned to the grid. You should also set <see cref="SyncfusionHandledEventArgs.Handled"/> to
        /// True if you want the grid to return this result instead of doing its own comparison.</param>
        public GridQueryCanMergeCellsEventArgs(GridStyleInfo style1, GridStyleInfo style2, bool result)
        {
            this.style1 = style1;
            this.style2 = style2;
            this.result = result;
        }

        /// <summary>
        /// Gets the first style object. Use the style object's <see cref="GridStyleInfo.CellIdentity"/> to find
        /// out about row and column index of the cell.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style1
        {
            get
            {
                return style1;
            }
        }

        /// <summary>
        /// Gets the second style object. Use the style object's <see cref="GridStyleInfo.CellIdentity"/> to find
        /// out about row and column index of the cell.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style2
        {
            get
            {
                return style2;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the result that should be returned to the grid. You should also set <see cref="SyncfusionHandledEventArgs.Handled"/> to
        /// True if you want the grid to return this result instead of doing its own comparison.
        /// </summary>
        [TraceProperty(true)]
        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridModel.SelectionChanging"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridSelectionChangingEventArgs"/> that contains the event data.</param>
    public delegate void GridSelectionChangingEventHandler(object sender, GridSelectionChangingEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridModel.SelectionChanging"/> event.
    /// </summary>
    /// <remarks>
    /// The GridSelectionChangingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.SelectionChanging"/> event
    /// when the model is in the process of selecting a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>
    /// method call.
    /// <para/>
    /// You can disallow the selection of specific cells at run-time when
    /// you assign true to <see cref="CancelEventArgs.Cancel"/>.<para/>
    /// You can also modify the <see cref="GridSelectionChangingEventArgs.Range"/> to include additional cells.
    /// <para/>
    /// The <see cref="GridModel"/> will raise a <see cref="GridModel.SelectionChanging"/> event before
    /// it updates its internal data structures and a <see cref="GridModel.SelectionChanged"/> event
    /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outlines
    /// the selected range of cells.
    /// </remarks>
    /// <seealso cref="GridSelectionChangingEventHandler"/>
    /// <seealso cref="GridModel.SelectionChanging"/>
    /// <seealso cref="GridSelectionChangedEventArgs"/>
    public sealed class GridSelectionChangingEventArgs : SyncfusionCancelEventArgs
    {
        private GridRangeInfo range;
        private GridSelectionReason reason;
        private GridRangeInfo clickRange = GridRangeInfo.Empty;

        /// <overload>
        /// Initalizes a new object.
        /// </overload>
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="range">The range of cells to be selected.</param>
        /// <param name="reason">The current state of the user action and reason for this event (mouse, keyboard or programmatic).</param>
        /// <param name="clickRange">The range of cells to be selected when the previous range is reset. <para/>
        /// Will be set only if reason is GridSelectionReason.SetCurrentCell, GridSelectionReason.MouseDown, GridSelectionReason.MouseMove.
        /// </param>
        public GridSelectionChangingEventArgs(GridRangeInfo range, GridSelectionReason reason, GridRangeInfo clickRange)
            : this(range, reason)
        {
            this.clickRange = clickRange;
        }

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="range">The range of cells to be selected.</param>
        /// <param name="reason">The current state of the user action and reason for this event (mouse, keyboard, or programmatic).</param>
        public GridSelectionChangingEventArgs(GridRangeInfo range, GridSelectionReason reason)
        {
            this.range = range;
            this.reason = reason;
        }

        /// <summary>
        /// Gets or sets the range of cells to be selected.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }

            set
            {
                range = value;
            }
        }

        /// <summary>
        /// Gets the range of cells to be selected when the previous range is reset. <para/>
        /// Will be set only if reason is GridSelectionReason.SetCurrentCell, GridSelectionReason.MouseDown, GridSelectionReason.MouseMove.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo ClickRange
        {
            get
            {
                return clickRange;
            }
        }

        /// <summary>
        /// Gets the current state of the user action and reason for this event (mouse, keyboard, or programmatic).
        /// </summary>
        [TraceProperty(true)]
        public GridSelectionReason Reason
        {
            get
            {
                return reason;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.SelectionChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridSelectionChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridSelectionChangedEventHandler(object sender, GridSelectionChangedEventArgs e);
    
    /// <summary>
    /// Provides data about the <see cref="GridModel.SelectionChanged"/> event.
    /// </summary>
    /// <remarks>
    /// The GridSelectionChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.SelectionChanged"/> event 
    /// when the model in the process of selecting a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>
    /// method call.
    /// <para/>
    /// The <see cref="GridModel"/> will raise a <see cref="GridModel.SelectionChanging"/> event before
    /// it updates its internal data structures and a <see cref="GridModel.SelectionChanged"/> event
    /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outlines
    /// the selected range of cells.
    /// </remarks>
    /// <seealso cref="GridSelectionChangedEventHandler"/>
    /// <seealso cref="GridModel.SelectionChanged"/>
    /// <seealso cref="GridSelectionChangingEventArgs"/>
    public sealed class GridSelectionChangedEventArgs : SyncfusionEventArgs
    {
        private GridRangeInfo range;
        private GridSelectionReason reason;
        private GridRangeInfoList oldRanges;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="range">The range of cells to be selected.</param>
        /// <param name="oldRanges">A <see cref="GridRangeInfoList"/> that holds all selected ranges before this user action.</param>
        /// <param name="reason">The origin source for this event (mouse, keyboard, or programmatic).</param>
        public GridSelectionChangedEventArgs(GridRangeInfo range, GridRangeInfoList oldRanges, GridSelectionReason reason)
        {
            this.range = range;
            this.reason = reason;
            this.oldRanges = oldRanges;
        }

        /// <summary>
        /// Gets the range of cells to be selected.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }
        }

        /// <summary>
        /// Gets the origin source for this event (mouse, keyboard, or programmatic).
        /// </summary>
        [TraceProperty(true)]
        public GridSelectionReason Reason
        {
            get
            {
                return reason;
            }
        }

        /// <summary>
        /// Gets a <see cref="GridRangeInfoList"/> that holds all selected ranges before this user action.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfoList OldRanges
        {
            get
            {
                return oldRanges;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.QueryCellModel"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridQueryCellModelEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryCellModelEventHandler(object sender, GridQueryCellModelEventArgs e);
    
    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryCellModel"/> event.
    /// </summary>
    /// <remarks>
    /// The GridQueryCellModelEventArgs is a custom event argument class used by the 
    /// <see cref="GridModel.QueryCellModel"/> event for querying the <see cref="GridCellModelBase"/>
    /// based on a string cellType.
    /// <para/>
    /// The GridModel has a table with all cell types used in the grid. Whenever the grid encounters
    /// a new cell type that it cannot find in the table, it will raise a <see cref="GridModel.QueryCellModel"/> event.
    /// The <see cref="GridStyleInfo.CellType"/> identifies the name of the cell type. The 
    /// <see cref="GridQueryCellModelEventArgs.CellModel"/> should receive the new instance of the
    /// associated cell object. This object will be stored in the table together with its name and
    /// reused among cells with the same <see cref="GridStyleInfo.CellType"/>.
    /// <para/>
    /// You should process this event if you want to add custom cell types and initialize these
    /// cell types on demand when associated cells are accessed the first time.
    /// </remarks>
    /// <seealso cref="GridQueryCellModelEventHandler"/>
    /// <seealso cref="GridModel.QueryCellModel"/> 
    public sealed class GridQueryCellModelEventArgs : GridModelEventArgs
    {
        string cellType;
        GridCellModelBase cellModel;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        /// <param name="cellType">The cell type identifier as used in the <see cref="GridStyleInfo.CellType"/> property.</param>
        public GridQueryCellModelEventArgs(GridModel gridModel, string cellType)
            : base(gridModel)
        {
            this.cellType = cellType;
            this.cellModel = null;
        }

        /// <summary>
        /// Gets the cell type identifier as used in the <see cref="GridStyleInfo.CellType"/> property.
        /// </summary>
        [TraceProperty(true)]
        public string CellType
        {
            get
            {
                return cellType;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="GridCellModelBase"/> for the cell type. You should create a new instance
        /// of the specific cell model and save it to this property.
        /// </summary>
        [TraceProperty(true)]
        public GridCellModelBase CellModel
        {
            get
            {
                return cellModel;
            }

            set
            {
                cellModel = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.QueryCoveredRange"/> event which can be
    /// marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridQueryCoveredRangeEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryCoveredRangeEventHandler(object sender, GridQueryCoveredRangeEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryCoveredRange"/> event which can be marked as handled.
    /// </summary>
    /// <remarks>
    /// GridQueryCoveredRangeEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.QueryCoveredRange"/> event to query information about 
    /// covered cells at a specified cell. 
    /// <para/>
    /// This event allows you to specify covered ranges at run-time, e.g when you have
    /// a large grid with repeating patterns of covered ranges. If the specified row and
    /// column index is part of a covered cell's range, you should assign the coordinates
    /// of the covered cell to <see cref="GridQueryCoveredRangeEventArgs.Range"/> and
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
    /// <para/>
    /// <see cref="SyncfusionHandledEventArgs.Handled"/> indicates that you supplied data
    /// from your event handler and no further querying for data about covered range information 
    /// for this cell is necessary.
    /// <para/>
    /// See the VirtualGrid sample for an example of how to use this event.
    /// </remarks>
    /// <seealso cref="GridQueryCoveredRangeEventHandler"/>
    /// <seealso cref="GridModel.QueryCoveredRange"/>  
    public class GridQueryCoveredRangeEventArgs : GridCellHandledEventArgs
    {
        GridRangeInfo range;

        /// <overload>
        /// Initalizes a new object.
        /// </overload>
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridQueryCoveredRangeEventArgs(int rowIndex, int colIndex)
            : base(rowIndex, colIndex)
        {
            this.range = null;
        }

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">A <see cref="GridRangeInfo"/> that will receive the resulting range for the covered cell.</param>
        public GridQueryCoveredRangeEventArgs(int rowIndex, int colIndex, GridRangeInfo range)
            : base(rowIndex, colIndex)
        {
            this.range = range;
        }

        /// <summary>
        /// Gets or sets a <see cref="GridRangeInfo"/> that will receive the resulting range for the covered cell.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }

            set
            {
                range = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.QueryBanneredRange"/> event which can be
    /// marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridQueryBanneredRangeEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryBanneredRangeEventHandler(object sender, GridQueryBanneredRangeEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryBanneredRange"/> event which can be marked as handled.
    /// </summary>
    /// <remarks>
    /// GridQueryBanneredRangeEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.QueryBanneredRange"/> event to query information about 
    /// a bannered range at a specified cell. 
    /// <para/>
    /// This event allows you to specify bannered ranges at run-time, e.g when you have
    /// a large grid with repeating patterns of bannered ranges. If the specified row and
    /// column index is part of a bannered cell's range, you should assign the coordinates
    /// of the bannered cell to <see cref="GridQueryBanneredRangeEventArgs.Range"/> and
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
    /// <para/>
    /// <see cref="SyncfusionHandledEventArgs.Handled"/> indicates that you supplied data
    /// from your event handler and no further querying for data about bannered range information 
    /// for this cell is necessary.
    /// <para/>
    /// See the BannerCells sample for an example how to use this event.
    /// </remarks>
    /// <seealso cref="GridQueryBanneredRangeEventHandler"/>
    /// <seealso cref="GridModel.QueryBanneredRange"/>  
    public sealed class GridQueryBanneredRangeEventArgs : GridCellHandledEventArgs
    {
        GridRangeInfo range;

        /// <overload>
        /// Initalizes a new object.
        /// </overload>
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridQueryBanneredRangeEventArgs(int rowIndex, int colIndex)
            : base(rowIndex, colIndex)
        {
            this.range = null;
        }

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">A <see cref="GridRangeInfo"/> that will receive the resulting range for the bannered cell.</param>
        public GridQueryBanneredRangeEventArgs(int rowIndex, int colIndex, GridRangeInfo range)
            : base(rowIndex, colIndex)
        {
            this.range = range;
        }

        /// <summary>
        /// Gets or sets a <see cref="GridRangeInfo"/> that will receive the resulting range for the bannered cell.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }

            set
            {
                range = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.QueryCellInfo"/> event which can be
    /// marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridQueryCellInfoEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryCellInfoEventHandler(object sender, GridQueryCellInfoEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryCellInfo"/> event which can be marked as handled.
    /// </summary>
    /// <remarks>
    /// GridQueryCellInfoEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.QueryCellInfo"/> event to query style information concerning
    /// a specified cell.
    /// <para/>
    /// This event allows you to customize cell contents at run-time on demand, just before
    /// the cell is drawn or programmatically accessed through <see cref="GridModel.this[int,int]"/>,
    /// <see cref="GridModel.ColStyles"/>, <see cref="GridModel.RowStyles"/>,
    /// or <see cref="GridModel.TableStyle"/>.
    /// <para/>
    /// If you made changes to <see cref="GridQueryCellInfoEventArgs.Style"/>, you should also
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
    /// flag to see whether the style has been initialized. If the event has been marked as
    /// handled, the grid will not access cell information from its own data store
    /// <see cref="GridModel.Data"/>. In the default case when the event is not marked as handled,
    /// the grid will locate cell information by calling <see cref="GridData.this"/>.
    /// <para/>
    /// In your handler for this event, normally you would set the
    /// CellValue for the GridStyleInfo object passed in with the event arguments. But you can also set
    /// other members of this GridStyleInfo object. For example,
    /// you could set BackColor to change the cell background. And, all this is done on a demand basis.
    /// There would be no BackColor value stored in any grid storage.
    /// <para/>
    /// The GridQueryCellInfoEventArgs members, e.ColIndex and e.RowIndex, specify column and row of the
    /// requested style. The e.Style member holds the
    /// GridStyleInfo object whose value this event should set provided it is a cell that you want to populate.
    /// It is possible that e.ColIndex and / or
    /// e.RowIndex may have the value of -1. A -1 indicates that a row style or column style is being requested.
    ///  So, e.ColIndex = -1 and and e.RowIndex = 4
    /// indicates the rowstyle for row 4 is being requested (GridControl.RowStyles[4]). Similarly, a positive
    /// column value with the row value = -1 would be a request for that
    /// particular columnstyle. If both values are -1, then the TableStyle property is being requested.
    /// <para/>
    /// Header rows and columns in an Essential Grid are treated the same as other rows and columns with
    /// respect to QueryCellInfo. If you have a single header row, then anytime e.ColIndex is 0, a row header
    /// is being requested. Similarly, if you have a
    /// single column header row, e.RowIndex = 0 is a request for the column header.
    /// <para/>
    /// Style information provided with QueryCellInfo is cached. This ensures this event is not hit
    /// everytime cell information is needed (e.g. when the user moves the mouse)
    /// and possibly forces a lookup in an external datasource which could be extensive depending on your
    /// implementation. If underlying data changes and you want to force a new call to QueryCellInfo,
    /// you should call <see cref="GridModel.ResetVolatileData"/> for the <see cref="GridModel"/> of a grid.
    /// <para/>
    /// You should not provide information in QueryCellInfo that depends on current view context, like
    /// changing the appearance of the cells that are on the current edited row. Use the
    /// <see cref="GridControlBase.PrepareViewStyleInfo"/> event to change
    /// the style of such cells about to be drawn. This event is fired from the cell renderer,
    /// and only reflects transient information which is not cached in the grid.<para/>
    /// <see cref="GridModel.QueryCellInfo"/> is fired from <see cref="GridModel"/>,
    /// and should be used mainly to provide non-transient information for a
    /// style such as the value from an external data source in a virtual grid.
    /// <see cref="GridControlBase.PrepareViewStyleInfo"/> is fired for every grid view and unique
    /// style settings for each view of the same model.
    /// <para/>
    /// See also the Virtual grid source code for example.
    /// </remarks>
    /// <example>
    /// <code lang="C#">
    /// private void GridQueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
    /// {
    ///         if(e.ColIndex > 0 &amp;&amp; e.RowIndex > 0)
    ///         {
    ///             e.Style.CellValue = this.intArray[e.RowIndex - 1, e.ColIndex - 1];
    ///             e.Handled = true;
    ///         }
    /// }
    /// </code>
    /// <code lang="VB">
    /// Private Sub GridQueryCellInfo(ByVal sender As Object, ByVal e As GridQueryCellInfoEventArgs)
    ///         If ((e.ColIndex > 0) AndAlso (e.RowIndex > 0)) Then
    ///             e.Style.CellValue = Me.intArray(e.RowIndex - 1, e.ColIndex - 1)
    ///             e.Handled = True
    ///         End If
    /// End Sub
    /// </code>
    /// </example>
    /// <seealso cref="GridQueryCellInfoEventHandler"/>
    /// <seealso cref="GridModel.QueryCellInfo"/>
    /// <seealso cref="IGridModelDataProvider"/>
    /// <seealso cref="GridSaveCellInfoEventArgs"/>
    public sealed class GridQueryCellInfoEventArgs : GridCellHandledEventArgs
    {
        GridStyleInfo style;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The style information for the cell.</param>
        public GridQueryCellInfoEventArgs(int rowIndex, int colIndex, GridStyleInfo style)
            : base(rowIndex, colIndex)
        {
            this.style = style;
        }

        /// <summary>
        /// Gets the style information for the cell.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.SaveCellInfo"/> event which can be
    /// marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An<see cref="GridSaveCellInfoEventArgs"/> that contains the event data.</param>
    public delegate void GridSaveCellInfoEventHandler(object sender, GridSaveCellInfoEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.SaveCellInfo"/> event which can be marked as handled.
    /// </summary>
    /// <remarks>
    /// GridSaveCellInfoEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.SaveCellInfo"/> event to save style information about
    /// at a specified cell.
    /// <para/>
    /// This event allows you to customize cell contents at run-time on demand, just before
    /// the cell is drawn or programmatically accessed through <see cref="GridModel.this[int,int]"/>,
    /// <see cref="GridModel.ColStyles"/>, <see cref="GridModel.RowStyles"/>,
    /// or <see cref="GridModel.TableStyle"/>.
    /// <para/>
    /// If you made changes to <see cref="GridSaveCellInfoEventArgs.Style"/> you should also
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
    /// flag to see whether the style has been changed from its original settings.
    /// <para/>
    /// The GridSaveCellInfoEventArgs members, e.ColIndex and e.RowIndex, specify column and row of the cell. The e.Style member holds the
    /// GridStyleInfo object whose properties this event should change provided it is a cell that you want to save changes for. It is possible that e.ColIndex and / or
    /// e.RowIndex may have the value of -1. A -1 indicates that a row style or column style is being saved. So, e.ColIndex = -1 and and e.RowIndex = 4
    /// indicates the rowstyle for row 4 is being saved (GridControl.RowStyles[4]). Similarly, a positive column value with the row value = -1 would be a request for that
    /// particular columnstyle. If both values are -1, the TableStyle property is being saved.
    /// <para/>
    /// Header rows and columns in an Essential Grid are treated the same as other rows and columns with
    /// respect to QueryCellInfo. If you have a single header row, anytime e.ColIndex is 0, a row header is being requested. Similarly, if you have a
    /// single column header row, e.RowIndex = 0 is a request for the column header.
    /// <para/>
    /// See DataBoundGrid source code for example.
    /// <note type="note">The intention of this event is to store and retrieve data. See
    /// <see cref="GridCellsChangedEventArgs"/> for the related UI event after changes were made to the
    /// data store.</note>
    /// </remarks>
    /// <seealso cref="GridSaveCellInfoEventHandler"/>
    /// <seealso cref="GridModel.SaveCellInfo"/>
    /// <seealso cref="IGridModelDataProvider"/>
    /// <seealso cref="GridQueryCellInfoEventArgs"/>
    public sealed class GridSaveCellInfoEventArgs : GridCellHandledEventArgs
    {
        GridStyleInfo style;
        StyleModifyType modifyType;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The style information for the cell.</param>
        /// <param name="modifyType">The style operation to be applied to the cells existing style.</param>
        public GridSaveCellInfoEventArgs(int rowIndex, int colIndex, GridStyleInfo style, StyleModifyType modifyType)
            : base(rowIndex, colIndex)
        {
            this.style = style;
            this.modifyType = modifyType;
        }

        /// <summary>
        /// Gets the style information for the cell.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        /// Gets the style operation to be applied to the cell's existing style.
        /// </summary>
        [TraceProperty(true)]
        public StyleModifyType ModifyType
        {
            get
            {
                return modifyType;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridModel.QueryColCount"/>, <see cref="GridModel.QueryRowCount"/>,
    /// <see cref="GridModel.SaveColCount"/>, and <see cref="GridModel.SaveRowCount"/> events that can be marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRowColCountEventArgs"/> that contains the event data.</param>
    public delegate void GridRowColCountEventHandler(object sender, GridRowColCountEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryColCount"/>, <see cref="GridModel.QueryRowCount"/>,
    /// <see cref="GridModel.SaveColCount"/>, and <see cref="GridModel.SaveRowCount"/> events which can be marked as handled.
    /// </summary>
    /// <remarks>
    /// GridRowColCountEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.QueryColCount"/>, <see cref="GridModel.QueryRowCount"/>,
    /// <see cref="GridModel.SaveColCount"/>, and <see cref="GridModel.SaveRowCount"/> events
    /// to query or save the row and column count of the grid.
    /// <para/>
    /// This event allows you to customize the grid dimensions at run-time on demand before
    /// the grid is drawn or programmatically accessed through <see cref="GridModel.RowCount"/>
    /// <see cref="GridModel.ColCount"/>.
    /// <para/>
    /// If you made changes to <see cref="GridRowColCountEventArgs.Count"/>, you should also
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. <see cref="GridModel.QueryColCount"/> and 
    /// <see cref="GridModel.QueryRowCount"/> will return <see cref="GridRowColCountEventArgs.Count"/> as the 
    /// actual number of rows or columns when the event is marked as handled.
    /// Otherwise, the grid will check its own data store 
    /// <see cref="GridModel.Data"/>. In the default case when the event is not marked as handled,
    /// the grid will locate row and column count by calling <see cref="GridData.ColCount"/> and
    /// <see cref="GridData.RowCount"/>.
    /// <para/>
    /// In <see cref="GridModel.SaveColCount"/> and <see cref="GridModel.SaveRowCount"/> the grid
    /// will not store row and column count if you marked the event as handled. Otherwise, in
    /// the default case that the event is not marked as handled the row and column count is stored
    /// by setting the <see cref="GridData.ColCount"/> and <see cref="GridData.RowCount"/> properties.
    /// <para/>
    /// See VirtualGrid sample code for example.
    /// <note type="note">The intention of this event is to store and retrieve data. See
    /// <see cref="GridRangeInsertedEventArgs"/> and <see cref="GridRangeRemovedEventArgs"/>
    /// for the related UI event after changes were made to the
    /// data store.</note>
    /// </remarks>
    /// <seealso cref="GridRowColCountEventHandler"/>
    /// <seealso cref="GridModel.QueryRowCount"/>
    /// <seealso cref="GridModel.QueryColCount"/>
    /// <seealso cref="GridModel.SaveRowCount"/>
    /// <seealso cref="GridModel.SaveColCount"/>
    /// <seealso cref="IGridModelDataProvider"/>  
    public sealed class GridRowColCountEventArgs : SyncfusionHandledEventArgs
    {
        int count;

        /// <overload>
        /// Initalizes a new object.
        /// </overload>
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        public GridRowColCountEventArgs()
        {
            this.count = 0;
        }

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="count">The number of rows or columns to be returned by this event.</param>
        public GridRowColCountEventArgs(int count)
        {
            this.count = count;
        }

        /// <summary>
        /// Gets or sets the number of rows or columns to be returned by this event.
        /// </summary>
        /// <remarks>
        /// Changing this value will only affect <see cref="GridModel.QueryColCount"/> and 
        /// <see cref="GridModel.QueryRowCount"/>. 
        /// <para/>
        /// <see cref="GridModel.SaveColCount"/> and <see cref="GridModel.SaveRowCount"/> will
        /// ignore changes to this value.
        /// </remarks>
        [TraceProperty(true)]
        public int Count
        {
            get
            {
                return count;
            }

            set
            {
                count = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.QueryRowHeightTotal"/> event that can be marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRowColSizeTotalEventArgs"/> that contains the event data.</param>
    public delegate void GridRowColSizeTotalEventHandler(object sender, GridRowColSizeTotalEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryRowHeightTotal"/> event which can be marked as handled and
    /// occurs when the <see cref="GridModelRowColSizeIndexer.GetTotal(int,int)"/> method of the <see cref="GridModel.RowHeights"/>
    /// class object is called.
    /// </summary>
    public sealed class GridRowColSizeTotalEventArgs : SyncfusionHandledEventArgs
    {
        int from;
        int last;
        int maximum;
        int size;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="from">The first row or column.</param>
        /// <param name="last">The last row or column.</param>
        /// <param name="maximum">Maximum value for total.</param>
        /// <param name="size">The total size.</param>
        internal GridRowColSizeTotalEventArgs(int from, int last, int maximum, int size)
        {
            this.from = from;
            this.last = last;
            this.maximum = maximum;
            this.size = size;
        }

        /// <summary>
        /// Gets the first row or column.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get
            {
                return from;
            }
        }

        /// <summary>
        /// Gets the last row or column.
        /// </summary>
        [TraceProperty(true)]
        public int Last
        {
            get
            {
                return last;
            }
        }

        /// <summary>
        /// Gets the Maximum value for total.
        /// </summary>
        [TraceProperty(true)]
        public int Maximum
        {
            get
            {
                return maximum;
            }
        }

        /// <summary>
        /// Gets or sets the result - Returns the total size.
        /// </summary>
        [TraceProperty(true)]
        public int Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }
    }
    
    /// <summary>
    /// Represents the method that handles <see cref="GridModel.QueryColWidth"/>, <see cref="GridModel.QueryRowHeight"/>,
    /// <see cref="GridModel.SaveColWidth"/>, and <see cref="GridModel.SaveRowHeight"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRowColSizeEventArgs"/> that contains the event data.</param>
    public delegate void GridRowColSizeEventHandler(object sender, GridRowColSizeEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryColWidth"/>, <see cref="GridModel.QueryRowHeight"/>,
    /// <see cref="GridModel.SaveColWidth"/>, and <see cref="GridModel.SaveRowHeight"/> events which can be marked as handled.
    /// </summary>
    /// <remarks>
    /// GridRowColSizeEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.QueryColWidth"/>, <see cref="GridModel.QueryRowHeight"/>,
    /// <see cref="GridModel.SaveColWidth"/>, and <see cref="GridModel.SaveRowHeight"/> events
    /// to query or save the row and column widths of the grid.
    /// <para/>
    /// This event allows you to customize the row and column sizes at run-time on demand before
    /// the grid is drawn or programmatically accessed through <see cref="GridModelRowColSizeIndexer.this[int]"/>.
    /// <para/>
    /// If you made changes to <see cref="GridRowColSizeEventArgs.Size"/> you should also
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. <see cref="GridModel.QueryColWidth"/> and 
    /// <see cref="GridModel.QueryRowHeight"/> will return <see cref="GridRowColSizeEventArgs.Size"/> as the 
    /// actual size of the row or column being queried when the event is marked as handled.
    /// Otherwise, the grid will check its own data store 
    /// <see cref="GridModelRowColSizeIndexer.Dictionary"/>. In the default case when the event is not marked as handled,
    /// the grid will locate row and column widths by getting the value from <see cref="IGridRowColSizeDictionary.this[int]"/>.
    /// <para/>
    /// In <see cref="GridModel.SaveColWidth"/> and <see cref="GridModel.SaveRowHeight"/>, the grid
    /// will not store row and column if you mark the event as handled. Otherwise, in
    /// the default case that the event is not marked as handled, the row and column width is stored
    /// by changing the <see cref="IGridRowColSizeDictionary.this[int]"/> property.
    /// <para/>
    /// <note type="note">The intention of this event is to store and retrieve data. See
    /// <see cref="GridRowColSizeChangedEventArgs"/> for the related UI event after changes are made to the
    /// data store.</note>
    /// </remarks>
    /// <seealso cref="GridRowColSizeEventHandler"/>
    /// <seealso cref="GridModel.QueryRowHeight"/>
    /// <seealso cref="GridModel.QueryColWidth"/>
    /// <seealso cref="GridModel.SaveRowHeight"/>
    /// <seealso cref="GridModel.SaveColWidth"/>
    /// <seealso cref="IGridRowColSizeDictionary"/>  
    /// <seealso cref="GridControlBase.GetRowHeight"/>
    /// <seealso cref="GridControlBase.GetColWidth"/>
    /// <seealso cref="GridModelRowColSizeIndexer"/>
    public sealed class GridRowColSizeEventArgs : SyncfusionHandledEventArgs
    {
        int index;
        int size;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="index">The row or column index.</param>
        /// <param name="size">The row height or column width. If size is less than 0, the grid will use the default
        /// size for row or column widths. See <see cref="GridModelRowColOperations.DefaultSize"/>.</param>
        public GridRowColSizeEventArgs(int index, int size)
        {
            this.index = index;
            this.size = size;
        }

        /// <summary>
        /// Gets the row or column index.
        /// </summary>
        [TraceProperty(true)]
        public int Index
        {
            get
            {
                return index;
            }
        }

        /// <summary>
        /// Gets or sets the row height or column width. If size is less than 0, the grid will use the default
        /// size for row or column widths. See <see cref="GridModelRowColOperations.DefaultSize"/>.
        /// </summary>
        [TraceProperty(true)]
        public int Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridModel.QueryHideCol"/>, <see cref="GridModel.QueryHideRow"/>,
    /// <see cref="GridModel.SaveHideCol"/>, and <see cref="GridModel.SaveHideRow"/> events which can be marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridRowColHideEventArgs"/> that contains the event data.</param>
    public delegate void GridRowColHideEventHandler(object sender, GridRowColHideEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryHideCol"/>, <see cref="GridModel.QueryHideRow"/>,
    /// <see cref="GridModel.SaveHideCol"/>, and <see cref="GridModel.SaveHideRow"/> events which can be marked as handled.
    /// </summary>
    /// <remarks>
    /// GridRowColHideEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.QueryHideCol"/>, <see cref="GridModel.QueryHideRow"/>,
    /// <see cref="GridModel.SaveHideCol"/>, and <see cref="GridModel.SaveHideRow"/> events
    /// to query or save if the row or column is hidden in the grid.
    /// <para/>
    /// This event allows you to customize the hidden state of row and column at run-time on demand before
    /// the grid is drawn or programmatically accessed through <see cref="GridModelHideRowColsIndexer.this[int]"/>.
    /// <para/>
    /// If you make changes to <see cref="GridRowColHideEventArgs.Hide"/>, you should also
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. <see cref="GridModel.QueryHideCol"/> and
    /// <see cref="GridModel.QueryHideRow"/> will return <see cref="GridRowColHideEventArgs.Hide"/> as the
    /// actual hide state of the row or column being queried when the event is marked as handled.
    /// Otherwise, the grid will check its own data store
    /// <see cref="GridModelHideRowColsIndexer.Dictionary"/>. In the default case when the event is not marked as handled,
    /// the grid will locate row and column widths by getting the value from <see cref="IGridRowColHideDictionary.this[int]"/>.
    /// <para/>
    /// In <see cref="GridModel.SaveHideCol"/> and <see cref="GridModel.SaveHideRow"/>, the grid
    /// will not store row and column hidden state if you mark the event as handled. Otherwise, in
    /// the default case that the event is not marked as handled the row and column hidden state is stored
    /// by changing the <see cref="IGridRowColHideDictionary.this[int]"/> property.
    /// <para/>
    /// <note type="note">The intention of this event is to store and retrieve data. See
    /// <see cref="GridRowColHiddenEventArgs"/> for the related UI event after changes were made to the
    /// data store.</note>
    /// </remarks>
    /// <seealso cref="GridRowColHideEventHandler"/>
    /// <seealso cref="GridModel.QueryHideRow"/>
    /// <seealso cref="GridModel.QueryHideCol"/>
    /// <seealso cref="GridModel.SaveHideRow"/>
    /// <seealso cref="GridModel.SaveHideCol"/>
    /// <seealso cref="GridModelHideColsIndexer"/>
    /// <seealso cref="GridModelHideRowsIndexer"/>
    /// <seealso cref="IGridRowColHideDictionary"/>
    /// <seealso cref="GridControlBase.GetRowHidden"/>
    /// <seealso cref="GridControlBase.GetColHidden"/>
    public sealed class GridRowColHideEventArgs : SyncfusionHandledEventArgs
    {
        int index;
        bool hide;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="index">The row or column index.</param>
        /// <param name="hide">The hidden flag for the row or column.</param>
        public GridRowColHideEventArgs(int index, bool hide)
        {
            this.index = index;
            this.hide = hide;
        }
        
        /// <summary>
        /// Gets the row or column index.
        /// </summary>
        [TraceProperty(true)]
        public int Index
        {
            get
            {
                return index;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the hidden flag for the row or column.
        /// </summary>
        [TraceProperty(true)]
        public bool Hide
        {
            get
            {
                return hide;
            }

            set
            {
                hide = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridModel.PrepareChangeSelection"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridPrepareChangeSelectionEventArgs"/> that contains the event data.</param>
    public delegate void GridPrepareChangeSelectionEventHandler(object sender, GridPrepareChangeSelectionEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.PrepareChangeSelection"/> event.
    /// </summary>
    /// <remarks>
    /// GridPrepareChangeSelectionEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.PrepareChangeSelection"/> event.
    /// <para/>
    ///  This event is raised by the model 
    /// to notify all associated views that there has been a change to the current selection
    /// in the grid and all associated views should redraw affected display contents. The change can
    /// be originated by a mouse or keyboard input or programmatically.
    /// </remarks>
    /// <seealso cref="GridPrepareChangeSelectionEventHandler"/>
    /// <seealso cref="GridModel.PrepareChangeSelection"/>
    public sealed class GridPrepareChangeSelectionEventArgs : SyncfusionEventArgs
    {
        GridRangeInfo oldRange;
        GridRangeInfo newRange;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="oldRange">The previous boundaries of the selected range.</param>
        /// <param name="newRange">The new boundaries of the selected range.</param>
        public GridPrepareChangeSelectionEventArgs(GridRangeInfo oldRange, GridRangeInfo newRange)
        {
            this.oldRange = oldRange;
            this.newRange = newRange;
        }

        /// <summary>
        /// Gets the previous boundaries of the selected range.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo OldRange
        {
            get { return oldRange; }
        }

        /// <summary>
        /// Gets the new boundaries of the selected range.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo NewRange
        {
            get { return newRange; }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.CellsChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellsChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridCellsChangedEventHandler(object sender, GridCellsChangedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.CellsChanged"/> event.
    /// </summary>
    /// <remarks>
    /// GridCellsChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.CellsChanged"/> event.
    /// <para/>
    /// This event is raised by the model
    /// to notify all associated views that there has been a change to the specified range of cells
    /// in the grid and all associated views should redraw affected display contents. The change can
    /// be originated by a mouse or keyboard input or programmatically by a <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/>
    /// method call.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were made to the model. If it is false, the operation is aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridCellsChangedEventHandler"/>
    /// <seealso cref="GridModel.CellsChanged"/>
    /// <seealso cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/>
    /// <seealso cref="GridCellsChangingEventArgs"/>
    public sealed class GridCellsChangedEventArgs : SyncfusionSuccessEventArgs
    {
        GridRangeInfo range;
        GridStyleInfo[] savedCellsInfo;

        /// <summary>
        /// Constructs a <see cref="GridCellsChangedEventArgs"/>.
        /// </summary>
        /// <param name="range">The range specifying the affected cells.</param>
        /// <param name="savedCellsInfo">Information about cell contents before the changes were applied to the grid.</param>
        /// <param name="success">Indicates whether an operation was successful.</param>
        public GridCellsChangedEventArgs(GridRangeInfo range, GridStyleInfo[] savedCellsInfo, bool success)
            : base(success)
        {
            this.range = range;
            this.savedCellsInfo = savedCellsInfo;
        }

        /// <summary>
        /// Gets the range specifying the affected cells.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }
        }

        /// <summary>
        /// Gets the Information about cell contents before the changes were applied to the grid.
        /// </summary>
        public GridStyleInfo[] SavedCellsInfo
        {
            get
            {
                return savedCellsInfo;
            }
        }
    }
    
    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.CellsChanging"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridCellsChangingEventArgs"/> that contains the event data.</param>
    public delegate void GridCellsChangingEventHandler(object sender, GridCellsChangingEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.CellsChanging"/> event.
    /// </summary>
    /// <remarks>
    /// GridCellsChangingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.CellsChanging"/> event. 
    /// <para/>
    /// This event is raised by the model
    /// to notify all associated views that it is contents for the specified range of cells
    /// in the grid and all associated views are polled if they are ok with that change and prepare
    /// for redrawing the affected area of cells. The change can
    /// be originated by a mouse or keyboard input or programmatically by a <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/>
    /// method call.
    /// <para/>
    /// The event handler can abort this operation by assigning True to the <see cref="CancelEventArgs.Cancel"/>
    /// property. No changes will then take place in the grid model.
    /// </remarks>
    /// <seealso cref="GridCellsChangingEventHandler"/>
    /// <seealso cref="GridModel.CellsChanging"/>
    /// <seealso cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/>
    /// <seealso cref="GridCellsChangedEventArgs"/>
    public sealed class GridCellsChangingEventArgs : SyncfusionCancelEventArgs
    {
        GridRangeInfo range;
        GridStyleInfo[] cellsInfo;
        StyleModifyType mt;

        /// <summary>
        /// Constructs a <see cref="GridCellsChangingEventArgs"/>.
        /// </summary>
        /// <param name="range">The range specifying the affected cells.</param>
        /// <param name="cellsInfo">The new contents to be stored in the cells. Can be NULL.</param>
        /// <param name="mt">The <see cref="Syncfusion.Styles.StyleModifyType"/> of the <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/> method call.</param>
        public GridCellsChangingEventArgs(GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType mt)
        {
            this.range = range;
            this.cellsInfo = cellsInfo;
            this.mt = mt;
        }

        /// <summary>
        /// Gets the range specifying the affected cells. 
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }
        }

        /// <summary>
        /// Gets the new contents to be stored in the cells. Can be NULL.
        /// </summary>
        public GridStyleInfo[] CellsInfo
        {
            get
            {
                return cellsInfo;
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Styles.StyleModifyType"/> of the <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[],Syncfusion.Styles.StyleModifyType)"/> method call.
        /// </summary>
        [TraceProperty(true)]
        public StyleModifyType ModiyType
        {
            get
            {
                return mt;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridModel.ColsHidden"/> and <see cref="GridModel.RowsHidden"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridRowColHiddenEventArgs"/> that contains the event data.</param>
    public delegate void GridRowColHiddenEventHandler(object sender, GridRowColHiddenEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsHidden"/> and <see cref="GridModel.ColsHidden"/>  event.
    /// </summary>
    /// <remarks>
    /// GridRowColHiddenEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsHidden"/> and <see cref="GridModel.ColsHidden"/> event.
    /// <para/>
    /// This event is raised by the model to notify all associated views that there has been
    /// a change to the specified range of rows and columns in the grid and all associated views
    /// should redraw affected display contents. The change can
    /// be originated by a mouse or keyboard input or programmatically by changing <see cref="GridModelHideRowColsIndexer.this[int]"/>.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were successfully made to the model. If it is false, this means the operation aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridRowColHiddenEventHandler"/>
    /// <seealso cref="GridModel.RowsHidden"/>
    /// <seealso cref="GridModel.ColsHidden"/>
    /// <seealso cref="GridRowColHidingEventHandler"/>
    public sealed class GridRowColHiddenEventArgs : SyncfusionSuccessEventArgs
    {
        // Fields
        private int from;
        private int to;
        private bool[] savedValues;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="from">The first row or column index of the affected range.</param>
        /// <param name="last">The last row or column index of the affected range.</param>
        /// <param name="savedValues">The previous state of the affected columns or rows.</param>
        /// <param name="success">Indicates if operation was successful or aborted.</param>
        public GridRowColHiddenEventArgs(int from, int last, bool[] savedValues, bool success)
            : base(success)
        {
            this.from = from;
            this.to = last;
            this.savedValues = savedValues;
        }

        /// <summary>
        /// Gets the first row or column index of the affected range.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get
            {
                return from;
            }
        }

        /// <summary>
        /// Gets the last row or column index of the affected range.
        /// </summary>
        [TraceProperty(true)]
        public int To
        {
            get
            {
                return to;
            }
        }

        /// <summary>
        /// Gets the previous state of the affected columns or rows.
        /// </summary>
        public bool[] SavedValues
        {
            get
            {
                return savedValues;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridModel.RowsHiding"/> and <see cref="GridModel.ColsHiding"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRowColHidingEventArgs"/> that contains the event data.</param>
    public delegate void GridRowColHidingEventHandler(object sender, GridRowColHidingEventArgs e);

    /// <summary>
    /// Used by <see cref="GridModel.RowsHiding"/> and <see cref="GridModel.ColsHiding"/> events.
    /// </summary>
    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsHidden"/> and <see cref="GridModel.ColsHidden"/> event.
    /// </summary>
    /// <remarks>
    /// GridRowColHidingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsHiding"/> and <see cref="GridModel.ColsHiding"/> event.
    /// <para/>
    /// This event is raised by the model to notify all associated views that there will be
    /// a change to the specified range of rows and columns in the grid and all associated views 
    /// should prepare to redraw the affected display contents. The change can
    /// be originated by a mouse or keyboard input or programmatically by changing <see cref="GridModelHideRowColsIndexer.this[int]"/>.
    /// <para/>
    /// The event handler can abort this operation by assigning true to the <see cref="CancelEventArgs.Cancel"/>
    /// property. No changes will then take place in the grid model.
    /// </remarks>
    /// <seealso cref="GridRowColHidingEventHandler"/>
    /// <seealso cref="GridModel.RowsHiding"/>
    /// <seealso cref="GridModel.ColsHiding"/>
    /// <seealso cref="GridRowColHiddenEventArgs"/>
    public sealed class GridRowColHidingEventArgs : SyncfusionCancelEventArgs
    {
        // Fields
        private int from;
        private int to;
        private bool[] values;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="from">The first row or column index of the affected range.</param>
        /// <param name="last">The last row or column index of the affected range.</param>
        /// <param name="values">The new state for the affected columns or rows.</param>
        public GridRowColHidingEventArgs(int from, int last, bool[] values)
        {
            this.from = from;
            this.to = last;
            this.values = values;
        }

        /// <summary>
        /// Gets the first row or column index of the affected range.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get
            {
                return from;
            }
        }

        /// <summary>
        /// Gets the last row or column index of the affected range.
        /// </summary>
        [TraceProperty(true)]
        public int To
        {
            get
            {
                return to;
            }
        }

        /// <summary>
        /// Gets the new state for the affected columns or rows.
        /// </summary>
        public bool[] Values
        {
            get
            {
                return values;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles  <see cref="GridModel.RowHeightsChanged"/> and 
    /// <see cref="GridModel.ColWidthsChanged"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRowColSizeChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridRowColSizeChangedEventHandler(object sender, GridRowColSizeChangedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowHeightsChanged"/> and <see cref="GridModel.ColWidthsChanged"/> event.
    /// </summary>
    /// <remarks>
    /// GridRowColSizeChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowHeightsChanged"/> and <see cref="GridModel.ColWidthsChanged"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that there has been
    /// a change to the specified range of rows and columns in the grid and all associated views
    /// should redraw affected display contents. The change can
    /// be originated by a mouse or keyboard input or programmatically by changing <see cref="GridModelRowColSizeIndexer.this[int]"/>.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were successfully made to the model. If it is false, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridRowColSizeChangedEventHandler"/>
    /// <seealso cref="GridModel.RowHeightsChanged"/>
    /// <seealso cref="GridModel.ColWidthsChanged"/>
    /// <seealso cref="GridRowColSizeChangingEventArgs"/>
    public class GridRowColSizeChangedEventArgs : SyncfusionSuccessEventArgs
    {
        // Fields
        private int from;
        private int to;
        private int[] savedValues;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="from">The first row or column index of the affected range.</param>
        /// <param name="last">The last row or column index of the affected range.</param>
        /// <param name="savedValues">The previous size of the affected columns or rows.</param>
        /// <param name="success">Indicates if operation was successful or aborted.</param>
        public GridRowColSizeChangedEventArgs(int from, int last, int[] savedValues, bool success)
            : base(success)
        {
            this.from = from;
            this.to = last;
            this.savedValues = savedValues;
        }

        /// <summary>
        /// Gets the first row or column index of the affected range.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get
            {
                return from;
            }
        }

        /// <summary>
        /// Gets the last row or column index of the affected range.
        /// </summary>
        [TraceProperty(true)]
        public int To
        {
            get
            {
                return to;
            }
        }

        /// <summary>
        /// Gets the previous size of the affected columns or rows.
        /// </summary>
        public int[] SavedValues
        {
            get
            {
                return savedValues;
            }
        }
    }
    
    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.RowHeightsChanging"/> and <see cref="GridModel.ColWidthsChanging"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRowColSizeChangingEventArgs"/> that contains the event data.</param>
    public delegate void GridRowColSizeChangingEventHandler(object sender, GridRowColSizeChangingEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowHeightsChanging"/> and <see cref="GridModel.ColWidthsChanging"/> events.
    /// </summary>
    /// <remarks>
    /// GridRowColSizeChangingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowHeightsChanging"/> and <see cref="GridModel.ColWidthsChanging"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it is about to 
    /// change the size for the specified range of rows and columns in the grid and all associated views
    /// should prepare to redraw affected display contents. The change can
    /// be originated by a mouse or keyboard input or programmatically by changing <see cref="GridModelRowColSizeIndexer.this[int]"/>.
    /// <para/>
    /// The event handler can abort this operation by assigning True to the <see cref="CancelEventArgs.Cancel"/>
    /// property. No changes will then take place in the grid model.
    /// </remarks>
    /// <seealso cref="GridRowColSizeChangingEventHandler"/>
    /// <seealso cref="GridModel.RowHeightsChanging"/>
    /// <seealso cref="GridModel.ColWidthsChanging"/>
    /// <seealso cref="GridRowColSizeChangedEventArgs"/>
    public sealed class GridRowColSizeChangingEventArgs : SyncfusionCancelEventArgs
    {
        // Fields
        private int from;
        private int to;
        private int[] values;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="from">The first row or column index of the affected range.</param>
        /// <param name="last">The last row or column index of the affected range.</param>
        /// <param name="values">The new size of the affected columns or rows.</param>
        public GridRowColSizeChangingEventArgs(int from, int last, int[] values)
        {
            this.from = from;
            this.to = last;
            this.values = values;
        }

        /// <summary>
        /// Gets the first row or column index of the affected range.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get
            {
                return from;
            }
        }

        /// <summary>
        /// Gets the last row or column index of the affected range.
        /// </summary>
        [TraceProperty(true)]
        public int To
        {
            get
            {
                return to;
            }
        }

        /// <summary>
        /// Gets the new size of the affected columns or rows.
        /// </summary>
        public int[] Values
        {
            get
            {
                return values;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.HeaderRowCountChanged"/>, <see cref="GridModel.FrozenRowCountChanged"/>,
    /// <see cref="GridModel.HeaderColCountChanged"/>, and <see cref="GridModel.FrozenColCountChanged"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridCountChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridCountChangedEventHandler(object sender, GridCountChangedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.HeaderRowCountChanged"/>, <see cref="GridModel.FrozenRowCountChanged"/>,
    /// <see cref="GridModel.HeaderColCountChanged"/>, and <see cref="GridModel.FrozenColCountChanged"/> events.
    /// </summary>
    /// <remarks>
    /// GridCountChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.HeaderRowCountChanged"/>, <see cref="GridModel.FrozenRowCountChanged"/>,
    /// <see cref="GridModel.HeaderColCountChanged"/>, and <see cref="GridModel.FrozenColCountChanged"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that there has been
    /// a change to the specified range of rows and columns in the grid and all associated views
    /// should redraw affected display contents. The change can
    /// be originated by a mouse or keyboard input or programmatically by changing <see cref="GridModelRowColOperations.HeaderCount"/>
    /// or <see cref="GridModelRowColOperations.FrozenCount"/>.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were successfully made to the model. If it is false, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridCountChangedEventHandler"/>
    /// <seealso cref="GridModelRowColOperations.HeaderCount"/> 
    /// <seealso cref="GridModelRowColOperations.FrozenCount"/> 
    /// <seealso cref="GridCountChangingEventArgs"/>
    public sealed class GridCountChangedEventArgs : SyncfusionSuccessEventArgs
    {
        private int savedCount;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="savedCount">The previous number of frozen or header rows or columns (depending on event).</param>
        /// <param name="success">Indicates if operation was successful or aborted.</param>
        public GridCountChangedEventArgs(int savedCount, bool success)
            : base(success)
        {
            this.savedCount = savedCount;
        }

        /// <summary>
        /// Gets the previous number of frozen or header rows or columns (depending on event).
        /// </summary>
        [TraceProperty(true)]
        public int SavedValue
        {
            get { return savedCount; }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.HeaderRowCountChanging"/>, <see cref="GridModel.FrozenRowCountChanging"/>,
    /// <see cref="GridModel.HeaderColCountChanging"/>, and <see cref="GridModel.FrozenColCountChanging"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridCountChangingEventArgs"/> that contains the event data.</param>
    public delegate void GridCountChangingEventHandler(object sender, GridCountChangingEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.HeaderRowCountChanging"/>, <see cref="GridModel.FrozenRowCountChanging"/>,
    /// <see cref="GridModel.HeaderColCountChanging"/>, and <see cref="GridModel.FrozenColCountChanging"/> events.
    /// </summary>
    /// <remarks>
    /// GridCountChangingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.HeaderRowCountChanging"/>, <see cref="GridModel.FrozenRowCountChanging"/>,
    /// <see cref="GridModel.HeaderColCountChanging"/>, and <see cref="GridModel.FrozenColCountChanging"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it is about to 
    /// change the specified range of rows and columns in the grid and all associated views
    /// should prepare to redraw affected display contents. The change can
    /// be originated by a mouse or keyboard input or programmatically by changing <see cref="GridModelRowColOperations.HeaderCount"/>
    /// or <see cref="GridModelRowColOperations.FrozenCount"/>.
    /// <para/>
    /// The event handler can abort this operation by assigning True to the <see cref="CancelEventArgs.Cancel"/>
    /// property. No changes will then take place in the grid model.
    /// </remarks>
    /// <seealso cref="GridCountChangingEventHandler"/>
    /// <seealso cref="GridModelRowColOperations.HeaderCount"/> 
    /// <seealso cref="GridModelRowColOperations.FrozenCount"/> 
    /// <seealso cref="GridCountChangedEventArgs"/>
    public sealed class GridCountChangingEventArgs : SyncfusionCancelEventArgs
    {
        private int count;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="count"> The new number of frozen or header rows or columns (depending on event).</param>
        public GridCountChangingEventArgs(int count)
        {
            this.count = count;
        }

        /// <summary>
        /// Gets the new number of frozen or header rows or columns (depending on event).
        /// </summary>
        [TraceProperty(true)]
        public int Value
        {
            get { return count; }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.DefaultRowHeightChanged"/> and <see cref="GridModel.DefaultColWidthChanged"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridDefaultSizeChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridDefaultSizeChangedEventHandler(object sender, GridDefaultSizeChangedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.DefaultRowHeightChanged"/> and <see cref="GridModel.DefaultColWidthChanged"/> events.
    /// </summary>
    /// <remarks>
    /// GridDefaultSizeChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.DefaultRowHeightChanged"/> and <see cref="GridModel.DefaultColWidthChanged"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it has changed the
    /// default row height or column width of the grid and all associated views
    /// should redraw affected display contents. The change can
    /// be originated by a mouse or keyboard input or programmatically by changing <see cref="GridModelRowColOperations.DefaultSize"/>.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were successfully made to the model. If it is false, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridDefaultSizeChangedEventHandler"/>
    /// <seealso cref="GridModelRowColOperations.DefaultSize"/>
    /// <seealso cref="GridDefaultSizeChangingEventArgs"/>
    public sealed class GridDefaultSizeChangedEventArgs : SyncfusionSuccessEventArgs
    {
        int savedValue;

        /// <summary>
        /// Constructs a <see cref="GridDefaultSizeChangedEventArgs"/>.
        /// </summary>
        /// <param name="savedValue">The previous default row height or column width</param>
        /// <param name="success">Indicates if the operation was succesful or aborted.</param>
        public GridDefaultSizeChangedEventArgs(int savedValue, bool success)
            : base(success)
        {
            this.savedValue = savedValue;
        }

        /// <summary>
        /// Gets the previous default row height or column width.
        /// </summary>
        [TraceProperty(true)]
        public int SavedValue
        {
            get { return savedValue; }
        }
    }
    
    /// <summary>
    /// Represents the method that handles the <see cref="GridControlBase.TopRowChanging"/> and <see cref="GridControlBase.LeftColChanging"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridDefaultSizeChangingEventArgs"/> that contains the event data.</param>
    public delegate void GridDefaultSizeChangingEventHandler(object sender, GridDefaultSizeChangingEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.DefaultRowHeightChanging"/> and <see cref="GridModel.DefaultColWidthChanging"/> events.
    /// </summary>
    /// <remarks>
    /// GridDefaultSizeChangingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.DefaultRowHeightChanging"/> and <see cref="GridModel.DefaultColWidthChanging"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it is about to change the
    /// default row height or column width of the grid and all associated views
    /// should prepare to redraw affected display contents. The change can
    /// be originated by a mouse or keyboard input or programmatically by changing <see cref="GridModelRowColOperations.DefaultSize"/>.
    /// <para/>
    /// The event handler can abort this operation by assigning True to the <see cref="CancelEventArgs.Cancel"/>
    /// property. No changes will then take place in the grid model.
    /// </remarks>
    /// <seealso cref="GridDefaultSizeChangingEventHandler"/>
    /// <seealso cref="GridModelRowColOperations.DefaultSize"/> 
    /// <seealso cref="GridDefaultSizeChangedEventArgs"/>
    public sealed class GridDefaultSizeChangingEventArgs : SyncfusionCancelEventArgs
    {
        int value;

        /// <summary>
        /// Constructs a <see cref="GridDefaultSizeChangingEventArgs"/>.
        /// </summary>
        /// <param name="value">The new default row height or column width.</param>
        public GridDefaultSizeChangingEventArgs(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the new default row height or column width.
        /// </summary>
        [TraceProperty(true)]
        public int Value
        {
            get { return value; }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.RowsInserted"/> and <see cref="GridModel.ColsInserted"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRangeInsertedEventArgs"/> that contains the event data.</param>
    public delegate void GridRangeInsertedEventHandler(object sender, GridRangeInsertedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsInserted"/> and <see cref="GridModel.ColsInserted"/> events.
    /// </summary>
    /// <remarks>
    /// GridRangeInsertedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsInserted"/> and <see cref="GridModel.ColsInserted"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it has inserted the
    /// specified number of rows or column into its data store and all associated views
    /// should redraw affected display contents. The change can
    /// be originated programmatically by a method call to <see cref="GridModelRowColOperations.InsertRange(int,int)"/>.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were successfully made to the model. If it is False, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridRangeInsertedEventHandler"/>
    /// <seealso cref="GridModelRowColOperations.InsertRange(int,int)"/>
    /// <seealso cref="GridRangeInsertingEventArgs"/>
    public sealed class GridRangeInsertedEventArgs : SyncfusionSuccessEventArgs
    {
        private int insertAt;
        private int count;
        private GridModelInsertRangeOptions iro;

        /// <summary>
        /// Initializes a new <see cref="GridRangeInsertedEventArgs"/>.
        /// </summary>
        /// <param name="insertAt">The row or column index where the cells should be inserted before.</param>
        /// <param name="count">The number of rows or columns.</param>
        /// <param name="iro">Information about the cells to be inserted such as cell contents, row, and column sizes and more.</param>
        /// <param name="success">Indicates whether an operation was successful.</param>
        public GridRangeInsertedEventArgs(int insertAt, int count, GridModelInsertRangeOptions iro, bool success)
            : base(success)
        {
            this.insertAt = insertAt;
            this.count = count;
            this.iro = iro;
        }

        /// <summary>
        /// Gets the row or column index where the cells should be inserted before.
        /// </summary>
        [TraceProperty(true)]
        public int InsertAt
        {
            get { return insertAt; }
        }

        /// <summary>
        /// Gets the number of rows or columns.
        /// </summary>
        [TraceProperty(true)]
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// Gets the Information about the cells to be inserted such as cell contents, row, and column sizes and more.
        /// </summary>
        /// <value>
        /// A <see cref="GridModelInsertRangeOptions"/> that holds information
        /// about the cells to be inserted such as cell contents, row, and column sizes and more.
        /// </value>
        public GridModelInsertRangeOptions InsertRangeOptions
        {
            get { return iro; }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.RowsInserting"/> and <see cref="GridModel.ColsInserting"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRangeInsertingEventArgs"/> that contains the event data.</param>
    public delegate void GridRangeInsertingEventHandler(object sender, GridRangeInsertingEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsInserting"/> and <see cref="GridModel.ColsInserting"/> events.
    /// </summary>
    /// <remarks>
    /// GridRangeInsertingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsInserting"/> and <see cref="GridModel.ColsInserting"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it is about to
    /// insert the specified number of rows or column into its data store and all associated views
    /// should prepare to redraw affected display contents. The change can
    /// be originated programmatically by a method call to <see cref="GridModelRowColOperations.InsertRange(int,int)"/>.
    /// <para/>
    /// The event handler can abort this operation by assigning True to the <see cref="CancelEventArgs.Cancel"/>
    /// property. No changes will then take place in the grid model.
    /// </remarks>
    /// <seealso cref="GridRangeInsertingEventHandler"/>
    /// <seealso cref="GridModelRowColOperations.InsertRange(int,int)"/>
    /// <seealso cref="GridRangeInsertedEventArgs"/>
    public sealed class GridRangeInsertingEventArgs : SyncfusionCancelEventArgs
    {
        private int insertAt;
        private int count;
        private GridModelInsertRangeOptions iro;

        /// <summary>
        /// Initializes a new <see cref="GridRangeInsertingEventArgs"/>.
        /// </summary>
        /// <param name="insertAt">The row or column index where the cells should be inserted before.</param>
        /// <param name="count">The number of rows or columns.</param>
        /// <param name="iro">Information about the cells to be inserted such as cell contents, row, and column sizes and more.</param>
        public GridRangeInsertingEventArgs(int insertAt, int count, GridModelInsertRangeOptions iro)
        {
            this.insertAt = insertAt;
            this.count = count;
            this.iro = iro;
        }

        /// <summary>
        /// Gets the row or column index where the cells should be inserted before.
        /// </summary>
        [TraceProperty(true)]
        public int InsertAt
        {
            get { return insertAt; }
        }

        /// <summary>
        /// Gets the number of rows or columns.
        /// </summary>
        [TraceProperty(true)]
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// Gets the Information about the cells to be inserted such as cell contents, row, and column sizes and more.
        /// </summary>
        /// <value>
        /// A <see cref="GridModelInsertRangeOptions"/> that holds information
        /// about the cells to be inserted such as cell contents, row, and column sizes and more.
        /// </value>
        public GridModelInsertRangeOptions InsertRangeOptions
        {
            get { return iro; }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridModel.RowsMoved"/> and <see cref="GridModel.ColsMoved"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRangeMovedEventArgs"/> that contains the event data.</param>
    public delegate void GridRangeMovedEventHandler(object sender, GridRangeMovedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsMoved"/> and <see cref="GridModel.ColsMoved"/> events.
    /// </summary>
    /// <remarks>
    /// GridRangeMovedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsMoved"/> and <see cref="GridModel.ColsMoved"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it has 
    /// rearranged the specified rows or columns in its data store and all associated views
    /// should redraw affected display contents. The change can
    /// be originated programmatically by a method call to <see cref="GridModelRowColOperations.MoveRange(int,int)"/>.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were successfully made to the model. If it is False, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridRangeMovedEventHandler"/>
    /// <seealso cref="GridModelRowColOperations.MoveRange(int,int)"/>
    /// <seealso cref="GridRangeMovingEventArgs"/>
    public sealed class GridRangeMovedEventArgs : SyncfusionSuccessEventArgs
    {
        private int from;
        private int count;
        private int target;

        /// <summary>
        /// Constructs a <see cref="GridRangeMovedEventArgs"/>.
        /// </summary>
        /// <param name="from">The index of the first row or column that was moved.</param>
        /// <param name="count">The number of rows or columns that was moved.</param>
        /// <param name="target">The row or column index where the cells were inserted before.</param>
        /// <param name="success">Indicates whether an operation was successful.</param>
        public GridRangeMovedEventArgs(int from, int count, int target, bool success)
            : base(success)
        {
            this.from = from;
            this.count = count;
            this.target = target;
        }

        /// <summary>
        /// Gets the index of the first row or column that was moved.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get { return from; }
        }

        /// <summary>
        /// Gets the index of the last row or column that was moved.
        /// </summary>
        [TraceProperty(true)]
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// Gets the row or column index where the cells were inserted before.
        /// </summary>
        [TraceProperty(true)]
        public int Target
        {
            get { return target; }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.RowsMoving"/> and <see cref="GridModel.ColsMoving"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRangeMovingEventArgs"/> that contains the event data.</param>
    public delegate void GridRangeMovingEventHandler(object sender, GridRangeMovingEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsMoving"/> and <see cref="GridModel.ColsMoving"/> events.
    /// </summary>
    /// <remarks>
    /// GridRangeMovingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsMoving"/> and <see cref="GridModel.ColsMoving"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it is about to
    /// rearrange the specified rows or columns in its data store and all associated views
    /// should prepare to redraw affected display contents. The change can
    /// be originated programmatically by a method call to <see cref="GridModelRowColOperations.MoveRange(int,int)"/>.
    /// <para/>
    /// The event handler can abort this operation by assigning True to the <see cref="CancelEventArgs.Cancel"/>
    /// property. No changes will then take place in the grid model.
    /// </remarks>
    /// <seealso cref="GridRangeMovingEventHandler"/>
    /// <seealso cref="GridModelRowColOperations.MoveRange(int,int)"/>
    /// <seealso cref="GridRangeMovedEventArgs"/>
    public sealed class GridRangeMovingEventArgs : SyncfusionCancelEventArgs
    {
        private int from;
        private int count;
        private int target;

        /// <summary>
        /// Constructs a <see cref="GridRangeMovingEventArgs"/>.
        /// </summary>
        /// <param name="from">The index of the first row or column to be moved.</param>
        /// <param name="count">The number of rows or columns to be moved.</param>
        /// <param name="target">The row or column index where the cells should be inserted before.</param>
        public GridRangeMovingEventArgs(int from, int count, int target)
        {
            this.from = from;
            this.count = count;
            this.target = target;
        }

        /// <summary>
        /// Gets the index of the first row or column to be moved.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get { return from; }
        }

        /// <summary>
        /// Gets the index of the last row or column to be moved.
        /// </summary>
        [TraceProperty(true)]
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// Gets the row or column index where the cells should be inserted before.
        /// </summary>
        [TraceProperty(true)]
        public int Target
        {
            get { return target; }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridModel.RowsRemoved"/> and <see cref="GridModel.ColsRemoved"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRangeRemovedEventArgs"/> that contains the event data.</param>
    public delegate void GridRangeRemovedEventHandler(object sender, GridRangeRemovedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsRemoved"/> and <see cref="GridModel.ColsRemoved"/> events.
    /// </summary>
    /// <remarks>
    /// GridRangeRemovedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsRemoved"/> and <see cref="GridModel.ColsRemoved"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it has 
    /// rearranged the specified rows or columns in its data store and all associated views
    /// should redraw affected display contents. The change can
    /// be originated programmatically by a method call too <see cref="GridModelRowColOperations.RemoveRange"/>.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were successfully made to the model. If it is false, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridRangeRemovedEventHandler"/>
    /// <seealso cref="GridModelRowColOperations.RemoveRange"/>
    /// <seealso cref="GridRangeRemovingEventArgs"/>
    public sealed class GridRangeRemovedEventArgs : SyncfusionSuccessEventArgs
    {
        private int from;
        private int to;
        private GridModelInsertRangeOptions iro;

        /// <summary>
        /// Constructs a <see cref="GridRangeRemovedEventArgs"/>.
        /// </summary>
        /// <param name="from">The index of the first row or column that was removed.</param>
        /// <param name="last">The index of the last row or column that was removed.</param>
        /// <param name="iro">Provides information about the cells that have been removed such as
        /// row heights, column widths, and hidden state of rows or columns. </param>
        /// <param name="success">Indicates whether an operation was successful.</param>
        public GridRangeRemovedEventArgs(int from, int last, GridModelInsertRangeOptions iro, bool success)
            : base(success)
        {
            this.from = from;
            this.to = last;
            this.iro = iro;
        }

        /// <summary>
        /// Gets the index of the first row or column that was removed.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get { return from; }
        }

        /// <summary>
        /// Gets the index of the last row or column that was removed.
        /// </summary>
        [TraceProperty(true)]
        public int To
        {
            get { return to; }
        }

        /// <summary>
        /// Gets the information about the cells that have been removed such as
        /// row heights, column widths, and hidden state of rows or columns. 
        /// </summary>
        /// <value>
        /// A <see cref="GridModelInsertRangeOptions"/> that holds information
        /// about the cells that have been deleted such as cell contents, row, and column sizes and more.
        /// </value>
        /// <remarks>
        /// If undo generation is enabled for the grid InsertRangeOptions will also
        /// contain information about the cells that have been deleted.
        /// </remarks>
        public GridModelInsertRangeOptions InsertRangeOptions
        {
            get { return iro; }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.RowsRemoving"/> and <see cref="GridModel.ColsRemoving"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRangeRemovingEventArgs"/> that contains the event data.</param>
    public delegate void GridRangeRemovingEventHandler(object sender, GridRangeRemovingEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsRemoving"/> and <see cref="GridModel.ColsRemoving"/> events.
    /// </summary>
    /// <remarks>
    /// GridRangeRemovingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsRemoving"/> and <see cref="GridModel.ColsRemoving"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it is about to
    /// remove the specified rows or columns from its data store and all associated views
    /// should prepare to redraw affected display contents. The change can
    /// be originated programmatically by a method call too <see cref="GridModelRowColOperations.RemoveRange"/>.
    /// <para/>
    /// The event handler can abort this operation by assigning True to the <see cref="CancelEventArgs.Cancel"/>
    /// property. No changes will then take place in the grid model.
    /// </remarks>
    /// <seealso cref="GridRangeRemovingEventHandler"/>
    /// <seealso cref="GridModelRowColOperations.RemoveRange"/>
    /// <seealso cref="GridRangeRemovedEventArgs"/>
    public sealed class GridRangeRemovingEventArgs : SyncfusionCancelEventArgs
    {
        private int from;
        private int to;

        /// <summary>
        /// Constructs a <see cref="GridRangeRemovingEventArgs"/>.
        /// </summary>
        /// <param name="from">The index of the first row or column to be removed.</param>
        /// <param name="last">The index of the last row or column to be removed.</param>
        public GridRangeRemovingEventArgs(int from, int last)
        {
            this.from = from;
            this.to = last;
        }

        /// <summary>
        /// Gets the index of the first row or column to be removed.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get { return from; }
        }

        /// <summary>
        /// Gets the index of the last row or column to be removed.
        /// </summary>
        [TraceProperty(true)]
        public int To
        {
            get { return to; }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.CoveredRangesChanging"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCoveredRangesChangingEventArgs"/> that contains the event data.</param>
    public delegate void GridCoveredRangesChangingEventHandler(object sender, GridCoveredRangesChangingEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridModel.CoveredRangesChanging"/> event.
    /// </summary>
    /// <remarks>
    /// GridCoveredRangesChangingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.CoveredRangesChanging"/> event.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it is about to
    /// make a range(s) of cells appear as covered cells or reset the covering for a list of ranges.
    /// <para/>
    /// This event is raised to make sure all associated views
    /// prepare to redraw affected display contents. The change can
    /// be originated programmatically by a method call to <see cref="GridModelCoveredRanges.Remove"/> or
    /// <see cref="GridModelCoveredRanges.Add"/>.
    /// <para/>
    /// The event handler can abort this operation by assigning True to the <see cref="CancelEventArgs.Cancel"/>
    /// property. No changes will then take place in the grid model.
    /// </remarks>
    /// <seealso cref="GridCoveredRangesChangingEventHandler"/>
    /// <seealso cref="GridModelCoveredRanges.Remove"/>
    /// <seealso cref="GridModelCoveredRanges.Add"/>
    /// <seealso cref="GridCoveredRangesChangedEventArgs"/>
    public sealed class GridCoveredRangesChangingEventArgs : SyncfusionCancelEventArgs
    {
        GridRangeInfoList ranges;
        bool setOrReset;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="ranges">A <see cref="GridRangeInfoList"/> with a list of affected ranges.</param>
        /// <param name="setOrReset">Specifies whether the specified ranges should be made covered ranges or
        /// if covering should be removed.</param>
        public GridCoveredRangesChangingEventArgs(GridRangeInfoList ranges, bool setOrReset)
        {
            this.ranges = ranges;
            this.setOrReset = setOrReset;
        }

        /// <summary>
        /// Gets a <see cref="GridRangeInfoList"/> with a list of affected ranges.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfoList Ranges
        {
            get
            {
                return ranges;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified ranges should be made covered ranges or
        /// if covering should be removed.
        /// </summary>
        /// <value>
        /// True if covered ranges; False is covering should be removed.
        /// </value>
        [TraceProperty(true)]
        public bool SetOrReset
        {
            get
            {
                return setOrReset;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.CoveredRangesChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridCoveredRangesChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridCoveredRangesChangedEventHandler(object sender, GridCoveredRangesChangedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.CoveredRangesChanged"/> event.
    /// </summary>
    /// <remarks>
    /// GridCoveredRangesChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.CoveredRangesChanged"/> event.
    /// <para/>
    /// This event is raised by the model to notify all associated views that the grid model 
    /// changed a range(s) of cells to appear as covered cells or reset the covering for a list of ranges.
    /// <para/>
    /// This event is raised to make sure all associated views
    /// redraw affected display contents. The change can
    /// be originated programmatically by a method call to <see cref="GridModelCoveredRanges.Remove"/> or
    /// <see cref="GridModelCoveredRanges.Add"/>.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates whether all changes
    /// were successfully made to the model. If it is False, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridCoveredRangesChangedEventHandler"/>
    /// <seealso cref="GridModelCoveredRanges.Remove"/>
    /// <seealso cref="GridModelCoveredRanges.Add"/>
    /// <seealso cref="GridCoveredRangesChangingEventArgs"/>
    public sealed class GridCoveredRangesChangedEventArgs : SyncfusionSuccessEventArgs
    {
        GridRangeInfoList ranges;
        bool setOrReset;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="ranges">A <see cref="GridRangeInfoList"/> with a list of affected ranges.</param>
        /// <param name="setOrReset">Specifies whether the specified ranges should be made covered ranges or
        /// if covering should be removed.</param>
        /// <param name="success">Indicates whether an operation was successful.</param>
        public GridCoveredRangesChangedEventArgs(GridRangeInfoList ranges, bool setOrReset, bool success)
            : base(success)
        {
            this.ranges = ranges;
            this.setOrReset = setOrReset;
        }

        /// <summary>
        /// Gets a <see cref="GridRangeInfoList"/> with a list of affected ranges.
        /// </summary>
        public GridRangeInfoList Ranges
        {
            get
            {
                return ranges;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified ranges should be made covered ranges or
        /// if covering should be removed.
        /// </summary>
        /// <value>
        /// True if covered ranges; False is covering should be removed.
        /// </value>
        [TraceProperty(true)]
        public bool SetOrReset
        {
            get
            {
                return setOrReset;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.BanneredRangesChanging"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridBanneredRangesChangingEventArgs"/> that contains the event data.</param>
    public delegate void GridBanneredRangesChangingEventHandler(object sender, GridBanneredRangesChangingEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridModel.BanneredRangesChanging"/> event.
    /// </summary>
    /// <remarks>
    /// GridBanneredRangesChangingEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.BanneredRangesChanging"/> event.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it is about to
    /// make a range(s) of cells appear as bannered cells or reset the bannering for a list of ranges.
    /// <para/>
    /// This event is raised to make sure all associated views
    /// prepare to redraw affected display contents. The change can
    /// be originated programmatically by a method call to <see cref="GridModelBanneredRanges.Remove"/> or
    /// <see cref="GridModelBanneredRanges.Add"/>.
    /// <para/>
    /// The event handler can abort this operation by assigning True to the <see cref="CancelEventArgs.Cancel"/>
    /// property. No changes will then take place in the grid model.
    /// </remarks>
    /// <seealso cref="GridBanneredRangesChangingEventHandler"/>
    /// <seealso cref="GridModelBanneredRanges.Remove"/>
    /// <seealso cref="GridModelBanneredRanges.Add"/>
    /// <seealso cref="GridBanneredRangesChangedEventArgs"/>
    public sealed class GridBanneredRangesChangingEventArgs : SyncfusionCancelEventArgs
    {
        GridRangeInfoList ranges;
        bool setOrReset;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="ranges">A <see cref="GridRangeInfoList"/> with a list of affected ranges.</param>
        /// <param name="setOrReset">Specifies whether the specified ranges should be made bannered ranges or
        /// if bannering should be removed.</param>
        public GridBanneredRangesChangingEventArgs(GridRangeInfoList ranges, bool setOrReset)
        {
            this.ranges = ranges;
            this.setOrReset = setOrReset;
        }

        /// <summary>
        /// Gets a <see cref="GridRangeInfoList"/> with a list of affected ranges.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfoList Ranges
        {
            get
            {
                return ranges;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified ranges should be made bannered ranges or
        /// if bannering should be removed.
        /// </summary>
        /// <value>
        /// True if bannered ranges; False if bannering should be removed.
        /// </value>
        [TraceProperty(true)]
        public bool SetOrReset
        {
            get
            {
                return setOrReset;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.BanneredRangesChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridBanneredRangesChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridBanneredRangesChangedEventHandler(object sender, GridBanneredRangesChangedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.BanneredRangesChanged"/> event.
    /// </summary>
    /// <remarks>
    /// GridBanneredRangesChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.BanneredRangesChanged"/> event.
    /// <para/>
    /// This event is raised by the model to notify all associated views that the grid model 
    /// changes a range(s) of cells to appear as bannered cells or resets the bannering for a list of ranges.
    /// <para/>
    /// This event is raised to make sure all associated views
    /// redraw affected display contents. The change can
    /// be originated programmatically by a method call to <see cref="GridModelBanneredRanges.Remove"/> or
    /// <see cref="GridModelBanneredRanges.Add"/>.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were successfully made to the model. If it is False, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridBanneredRangesChangedEventHandler"/>
    /// <seealso cref="GridModelBanneredRanges.Remove"/>
    /// <seealso cref="GridModelBanneredRanges.Add"/>
    /// <seealso cref="GridBanneredRangesChangingEventArgs"/>
    public sealed class GridBanneredRangesChangedEventArgs : SyncfusionSuccessEventArgs
    {
        GridRangeInfoList ranges;
        bool setOrReset;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="ranges">A <see cref="GridRangeInfoList"/> with a list of affected ranges.</param>
        /// <param name="setOrReset">Specifies whether the specified ranges should be made bannered ranges or
        /// if bannering should be removed.</param>
        /// <param name="success">Indicates whether an operation was successful.</param>
        public GridBanneredRangesChangedEventArgs(GridRangeInfoList ranges, bool setOrReset, bool success)
            : base(success)
        {
            this.ranges = ranges;
            this.setOrReset = setOrReset;
        }

        /// <summary>
        /// Gets a <see cref="GridRangeInfoList"/> with a list of affected ranges.
        /// </summary>
        public GridRangeInfoList Ranges
        {
            get
            {
                return ranges;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified ranges should be made bannered ranges or
        /// if bannering should be removed.
        /// </summary>
        /// <value>
        /// True if bannered ranges; False if bannering should be removed.
        /// </value>
        [TraceProperty(true)]
        public bool SetOrReset
        {
            get
            {
                return setOrReset;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.FloatingCellsChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridFloatingCellsChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridFloatingCellsChangedEventHandler(object sender, GridFloatingCellsChangedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.FloatingCellsChanged"/> event.
    /// </summary>
    /// <remarks>
    /// GridFloatingCellsChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.FloatingCellsChanged"/> event.
    /// <para/>
    /// This event is raised by the model to notify all associated views that the grid model
    /// changed a range(s) of cells to appear as a floating cell or reset the floating for a range.
    /// <para/>
    /// This event is raised to make sure all associated views
    /// redraw affected display contents.
    /// <para/>
    /// The change
    /// is normally originated by a user typing text into the cell that is larger than
    /// the current cell size. The grid checks the preferred cell width of the cell and if it
    /// is larger than the current size and the neighboring cell supports floating, the cell will
    /// changed to a floating cell.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were successfully made to the model. If it is False, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridFloatingCellsChangedEventHandler"/>
    /// <seealso cref="GridStyleInfo.FloatCell"/>
    /// <seealso cref="GridStyleInfo.FloodCell"/>
    /// <seealso cref="GridCellModelBase.OnQueryCanFloatCell"/>
    public sealed class GridFloatingCellsChangedEventArgs : SyncfusionSuccessEventArgs
    {
        GridRangeInfo range;

        internal GridFloatingCellsChangedEventArgs(GridRangeInfo range, bool success)
            : base(success)
        {
            this.range = range;
        }

        /// <summary>
        /// Gets the affected cells range.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.MergeCellsChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridMergeCellsChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridMergeCellsChangedEventHandler(object sender, GridMergeCellsChangedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.MergeCellsChanged"/> event.
    /// </summary>
    /// <remarks>
    /// GridMergeCellsChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.MergeCellsChanged"/> event.
    /// <para/>
    /// This event is raised by the model to notify all associated views that the grid model
    /// changed a range(s) of cells to appear as a merge cell or reset the merge for a range.
    /// <para/>
    /// This event is raised to make sure all associated views
    /// redraw affected display contents.
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if all changes
    /// were successfully made to the model. If it is False, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridMergeCellsChangedEventHandler"/>
    /// <seealso cref="GridStyleInfo.MergeCell"/>
    /// <seealso cref="GridCellModelBase.OnQueryCanMergeCell"/>
    public sealed class GridMergeCellsChangedEventArgs : SyncfusionSuccessEventArgs
    {
        GridRangeInfo range;

        internal GridMergeCellsChangedEventArgs(GridRangeInfo range, bool success)
            : base(success)
        {
            this.range = range;
        }

        /// <summary>
        /// Gets the affected cell's range.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridModel.EndUpdateRequest"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridEndUpdateRequestEventArgs"/> that contains the event data.</param>
    public delegate void GridEndUpdateRequestEventHandler(object sender, GridEndUpdateRequestEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.EndUpdateRequest"/> event.
    /// </summary>
    /// <remarks>
    /// GridFloatingCellsChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.EndUpdateRequest"/> event.
    /// <para/>
    /// This event is raised by the model when <see cref="GridModel.EndUpdate()"/> is programmatically 
    /// called and there are no pending <see cref="GridModel.BeginUpdate()"/> calls. When there are several calls,
    /// you have to call as many times EndUpdate as you called BeginUpdate before this event is raised.
    /// <para/>
    /// This event is raised to make sure all associated views
    /// update their display contents by completing any pending paint operations.
    /// </remarks>
    /// <seealso cref="GridEndUpdateRequestEventHandler"/>
    /// <seealso cref="GridModel.EndUpdate()"/>
    /// <seealso cref="GridModel.BeginUpdate()"/>
    public sealed class GridEndUpdateRequestEventArgs : SyncfusionEventArgs
    {
        bool value;

        /// <overload>
        /// Initalizes a new object.
        /// </overload>
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        public GridEndUpdateRequestEventArgs()
        {
            value = false;
        }

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="value">The value of the originating <see cref="GridModel.EndUpdate()"/> call. </param>
        public GridEndUpdateRequestEventArgs(bool value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value of the originating <see cref="GridModel.EndUpdate()"/> call. 
        /// </summary>
        /// <value>
        /// True if display should be updated immediately; False if pending paint operations should be discarded.
        /// </value>
        [TraceProperty(true)]
        public bool Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
    }

    /// <summary>
    /// Custom event argument base class used for events associated with a <see cref="GridModel"/>. 
    /// </summary>
    public class GridModelEventArgs : SyncfusionEventArgs
    {
        GridModel gridModel;

        /// <summary>
        /// Initializes a new <see cref="GridModelEventArgs"/> object.
        /// </summary>
        /// <param name="model">Reference to the <see cref="GridModel"/>.</param>
        public GridModelEventArgs(GridModel model)
        {
            this.gridModel = model;
        }

        /// <summary>
        /// Gets <see cref="GridModel"/>.
        /// </summary>
        [TraceProperty(true)]
        public GridModel GridModel
        {
            get
            {
                return gridModel;
            }
        }
    }

    /// <summary>
    /// Provides data about the <see cref="GridModel.CutPaste"/>: 
    /// <see cref="GridModelCutPaste.Paste"/>
    /// <see cref="GridModelCutPaste.CanPaste"/>
    /// <see cref="GridModelCutPaste.Cut"/>
    /// <see cref="GridModelCutPaste.CanCut"/>
    /// <see cref="GridModelCutPaste.Copy"/> and 
    /// <see cref="GridModelCutPaste.CanCopy"/>
    /// events.
    /// </summary>
    /// <remarks>
    /// GridCutPasteEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.CutPaste"/> operations in a <see cref="GridModel"/>.
    /// <para/>
    /// This event lets you supply your own clipboard formats or add support for pasting additional clipboard content.
    /// </remarks>
    public class GridCutPasteEventArgs : SyncfusionHandledEventArgs
    {
        bool ignoreCurrentCell;
        bool result;
        DataObject dataObject = null;
        int clipboardFlags = GridDragDropFlags.Text | GridDragDropFlags.Styles | GridDragDropFlags.Compose;
        GridRangeInfoList rangeList = null;

        /// <summary>
        /// Initializes a new <see cref="GridCutPasteEventArgs"/> object.
        /// </summary>
        /// <param name="ignoreCurrentCell">Ignore the current cell position.</param>
        /// <param name="result">The value that the called method should return.</param>
        /// <param name="clipboardFlags">Customize behavior of default clipboard operations.</param>
        /// <param name="rangeList">You can save here a list of ranges that have been copied.</param>
        public GridCutPasteEventArgs(bool ignoreCurrentCell, bool result, int clipboardFlags, GridRangeInfoList rangeList)
        {
            this.ignoreCurrentCell = ignoreCurrentCell;
            this.result = result;
            this.clipboardFlags = clipboardFlags;
            this.rangeList = rangeList;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the current cell. This property will only be checked by the calling
        /// method if you do not set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
        /// </summary>
        [TraceProperty(true)]
        public bool IgnoreCurrentCell
        {
            get
            {
                return ignoreCurrentCell;
            }

            set
            {
                ignoreCurrentCell = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the return value the called method should return when you 
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. 
        /// </summary>
        [TraceProperty(true)]
        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }

        /// <summary>
        /// Gets or sets the DataObject to be used for further clipboard operations. This property will only be checked by the calling
        /// method if you do not set <see cref="SyncfusionHandledEventArgs.Handled"/> to true. Can be NULL.
        /// </summary>
        public DataObject DataObject
        {
            get
            {
                return dataObject;
            }

            set
            {
                dataObject = value;
            }
        }

        /// <summary>
        /// Gets or sets customize behavior of default clipboard operations. This property will only be checked by the calling
        /// method if you do not set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. Allows you to specify
        /// if text or styles should be copied or if row or column headers should be included.
        /// </summary>
        [TraceProperty(true)]
        public int ClipboardFlags
        {
            get
            {
                return clipboardFlags;
            }

            set
            {
                clipboardFlags = value;
            }
        }

        /// <summary>
        /// Gets or sets range list. You can save here a list of ranges that have been copied. This property will only be checked for the
        /// Copy operation. Can be NULL.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfoList RangeList
        {
            get
            {
                return rangeList;
            }

            set
            {
                rangeList = value;
            }
        }
    }

    /// <summary>
    /// Provides data about the <see cref="GridModel.ClipboardCopyToBuffer"/>: 
    /// events.
    /// </summary>
    /// <remarks>
    /// ClipboardCopyToBufferEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.ClipboardCopyToBuffer"/> operations in a <see cref="GridModel"/>.
    /// <para/>
    /// This event lets you modify the text which is being copied to the buffer
    /// </remarks>
    public class ClipboardCopyToBufferEventArgs : SyncfusionHandledEventArgs
    {
        string text = string.Empty;
        GridRangeInfoList range = new GridRangeInfoList();
        /// <summary>
        /// Initializes a new <see cref="ClipboardCopyToBufferEventArgs"/> object.
        /// </summary>
        /// <param name="text">You can modify the text that have been copied.</param>
        /// <param name="range">You can set the range to copy the text into certain range.</param>
        public ClipboardCopyToBufferEventArgs(string text, GridRangeInfoList range)
        {
            this.CopyText = text;
            this.range = range;
        }

        /// <summary>
        /// Gets or sets a string value which will replace the text being copied
        /// </summary>
        [TraceProperty(true)]
        public string CopyText
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        /// <summary>
        /// Gets or sets a string value which will replace the text being copied
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfoList Ranges
        {
            get
            {
                return range;
            }
        }
    }

    /// <summary>
    /// Provides data about the <see cref="GridModel.ClearingCells"/> events.
    /// </summary>
    /// <remarks>
    /// GridCutPasteEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.CutPaste"/> operations in a <see cref="GridModel"/>.
    /// <para/>
    /// This event lets you supply your own clipboard formats or add support for pasting additional clipboard content.
    /// </remarks>
    public sealed class GridClearingCellsEventArgs : SyncfusionHandledEventArgs
    {
        GridRangeInfoList rangeList;
        bool clearStyle;
        bool result;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rangeList">Gets / sets a list of ranges to be cleared out.</param>
        /// <param name="clearStyle">True if all cell style information should be cleared; False if only text should be cleared.</param>
        /// <param name="result">Specifies the return value the called method should return when you 
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. </param>
        public GridClearingCellsEventArgs(GridRangeInfoList rangeList, bool clearStyle, bool result)
        {
            this.rangeList = rangeList;
            this.clearStyle = clearStyle;
            this.result = result;
        }

        /// <summary>
        /// Gets or sets a list of ranges to be cleared out.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfoList RangeList
        {
            get
            {
                return rangeList;
            }

            set
            {
                rangeList = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cell style information should be cleared. True if all cell style information should be cleared; False if only text should be cleared.
        /// </summary>
        [TraceProperty(true)]
        public bool ClearStyle
        {
            get
            {
                return clearStyle;
            }

            set
            {
                clearStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the return value the called method should return when you 
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. 
        /// </summary>
        [TraceProperty(true)]
        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }

    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryOleDataSourceData"/> event.
    /// </summary>
    /// <remarks>
    /// GridQueryOleDataSourceDataEventArgs is a custom event argument class used by the
    /// QueryOleDataSourceData even when a user starts dragging a range of selected cells
    /// using OLE drag-and-drop.
    /// <para/>
    /// This event lets you supply your own clipboard formats or add support for pasting additional clipboard content.
    /// </remarks>
    public class GridQueryOleDataSourceDataEventArgs : SyncfusionHandledEventArgs
    {
        bool ignoreCurrentCell = false;
        bool result = true;
        IDataObject dataObject;
        GridRangeInfoList rangeList;
        int dragDropFlags;
        int rowCount;
        int colCount;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="dataObject">The data object where data for the OLE drag operation is stored.</param>
        /// <param name="rangeList">You can save here a list of ranges that have been copied. This property will only be checked for the
        /// Copy operation. Can be NULL.</param>
        /// <param name="dragDropFlags">Lets you specify default behavior, e.g. if styles or text should be supplied, if row or column headers
        /// should be ignored.</param>
        /// <param name="rowCount">The number of rows that have been copied into the dataobject. Set this value if you modified the <see cref="DataObject"/>.</param>
        /// <param name="colCount">The number of columns that have been copied into the dataobject. Set this value if you modified the <see cref="DataObject"/>.</param>
        public GridQueryOleDataSourceDataEventArgs(IDataObject dataObject, GridRangeInfoList rangeList, int dragDropFlags, int rowCount, int colCount)
        {
            this.dataObject = dataObject;
            this.rangeList = rangeList;
            this.dragDropFlags = dragDropFlags;
            this.rowCount = rowCount;
            this.colCount = colCount;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the current cell. This property will only be checked by the calling
        /// method if you do not set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
        /// </summary>
        [TraceProperty(true)]
        public bool IgnoreCurrentCell
        {
            get
            {
                return ignoreCurrentCell;
            }

            set
            {
                ignoreCurrentCell = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the return value the called method should return when you 
        /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. 
        /// </summary>
        [TraceProperty(true)]
        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }

        /// <summary>
        /// Gets the data object where data for the OLE drag operation are stored.
        /// </summary>
        public IDataObject DataObject
        {
            get
            {
                return dataObject;
            }
        }

        /// <summary>
        /// Gets or sets the range list. You can save here a list of ranges that have been copied. This property will only be checked for the
        /// Copy operation. Can be NULL.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfoList RangeList
        {
            get
            {
                return rangeList;
            }

            set
            {
                rangeList = value;
            }
        }

        /// <summary>
        /// Gets or sets the behaviour. Lets you specify default behavior, e.g. if styles or text should be supplied, if row or column headers
        /// should be ignored.
        /// </summary>
        [TraceProperty(true)]
        public int DragDropFlags
        {
            get
            {
                return dragDropFlags;
            }

            set
            {
                dragDropFlags = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of rows that have been copied into the dataobject. Set this value if you modified the <see cref="DataObject"/>.
        /// </summary>
        [TraceProperty(true)]
        public int RowCount
        {
            get
            {
                return rowCount;
            }

            set
            {
                rowCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of columns that have been copied into the dataobject. Set this value if you modified the <see cref="DataObject"/>.
        /// </summary>
        [TraceProperty(true)]
        public int ColCount
        {
            get
            {
                return colCount;
            }

            set
            {
                colCount = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridModel.QueryOleDataSourceData"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridQueryOleDataSourceDataEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryOleDataSourceDataEventHandler(object sender, GridQueryOleDataSourceDataEventArgs e);

    /// <summary>
    /// Represents the method that handles the<see cref="GridModel.ClipboardCopyToBuffer"/>, <see cref="GridModel.ClipboardCopyToBuffer"/>, 
    /// events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="ClipboardCopyToBufferEventArgs"/> that contains the event data.</param>
    public delegate void ClipboardCopyToBufferEventHandler(object sender, ClipboardCopyToBufferEventArgs e);

    /// <summary>
    /// Represents the method that handles the<see cref="GridModel.ClipboardPaste"/>, <see cref="GridModel.ClipboardCanPaste"/>, 
    /// <see cref="GridModel.ClipboardCut"/>, <see cref="GridModel.ClipboardCanCut"/>, 
    /// <see cref="GridModel.ClipboardCopy"/>, and <see cref="GridModel.ClipboardCanCopy"/> 
    /// events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridCutPasteEventArgs"/> that contains the event data.</param>
    public delegate void GridCutPasteEventHandler(object sender, GridCutPasteEventArgs e);
    
    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.ClearingCells"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridClearingCellsEventArgs"/> that contains the event data.</param>
    public delegate void GridClearingCellsEventHandler(object sender, GridClearingCellsEventArgs e);
    
    // eva GridOleDropAtRowCol SyncfusionHandled IDataObject dataObject int rowIndex int colIndex bool result

    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.OleDropAtRowCol"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridOleDropAtRowColEventArgs"/> that contains the event data.</param>
    public delegate void GridOleDropAtRowColEventHandler(object sender, GridOleDropAtRowColEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.OleDropAtRowCol"/> event.
    /// </summary>
    /// <remarks>
    /// The event occurs when the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and
    /// before the data is applied to the grid.
    /// <para/>
    /// This event lets you provide your own customized paste data behavior.
    /// <para/>
    /// If you do not wish the grid to proceed with default behavior for this method,
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
    /// flag to see whether it should proceed. If you set it to True, the calling method
    /// will check <see cref="GridOleDropAtRowColEventArgs.Result"/> as indication if the
    /// operation was successful.
    /// <para/>
    /// If you want the grid to proceed with default behavior, you should not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
    /// </remarks>
    /// <seealso cref="GridOleDropAtRowColEventHandler"/>
    public sealed class GridOleDropAtRowColEventArgs : SyncfusionHandledEventArgs
    {
        IDataObject dataObject;
        int rowIndex;
        int colIndex;
        bool result;

        /// <summary>
        /// Initializes the new object.
        /// </summary>
        /// <param name="dataObject">The Data Object with clipboard data.</param>
        /// <param name="rowIndex">The target row index.</param>
        /// <param name="colIndex">The target column index.</param>
        /// <param name="result">Default value for <see cref="Result"/>.</param>
        public GridOleDropAtRowColEventArgs(IDataObject dataObject, int rowIndex, int colIndex, bool result)
        {
            this.dataObject = dataObject;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.result = result;
        }

        /// <summary>
        /// Gets or sets the Data Object with clipboard data.
        /// </summary>
        [TraceProperty(true)]
        public IDataObject DataObject
        {
            get
            {
                return dataObject;
            }

            set
            {
                dataObject = value;
            }
        }

        /// <summary>
        /// Gets or sets the target row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }

            set
            {
                rowIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the target column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }

            set
            {
                colIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the result of the operation (if Handled = true is specified).
        /// </summary>
        [TraceProperty(true)]
        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }

    /// <summary>
    /// Provides data about <see cref="GridModel.InvalidateRangeRequest"/> event of a <see cref="GridModel"/>.
    /// </summary>
    public sealed class GridInvalidateRangeRequestEventArgs : SyncfusionEventArgs
    {
        GridRangeInfo range;
        GridRangeOptions options;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="range">The range of cells to be repainted.</param>
        /// <param name="options">Options that indicate if method should enlarge the affected range of cells to include covered and floating cells.</param>
        public GridInvalidateRangeRequestEventArgs(GridRangeInfo range, GridRangeOptions options)
        {
            this.range = range;
            this.options = options;
        }

        /// <summary>
        /// Gets or sets the range of cells to be repainted.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }

            set
            {
                range = value;
            }
        }

        /// <summary>
        /// Gets or sets the options that indicate if method should enlarge the affected range of cells to include covered and floating cells.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeOptions Options
        {
            get
            {
                return options;
            }

            set
            {
                options = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridModel.InvalidateRangeRequest"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridInvalidateRangeRequestEventArgs"/> that contains the event data.</param>
    public delegate void GridInvalidateRangeRequestEventHandler(object sender, GridInvalidateRangeRequestEventArgs e);

    /// <summary>
    /// Holds row and column coordinates for events that can be marked as handled and are associated with a specific cell.
    /// </summary>
    /// <remarks>
    /// No events use this class directly but it is used 
    /// as a base class for several other events related to a specific cell.
    /// </remarks>
    public class GridCellHandledEventArgs : SyncfusionHandledEventArgs
    {
        int rowIndex;
        int colIndex;

        /// <summary>
        /// Initializes a new <see cref="GridCellHandledEventArgs"/> object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridCellHandledEventArgs(int rowIndex, int colIndex)
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }
        }
    }
    
    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.SaveCellInfo"/> event which can be
    /// marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridSaveCellInfoEventArgs"/> that contains the event data.</param>
    public delegate void GridPasteCellTextEventHandler(object sender, GridPasteCellTextEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.PasteCellText"/> event which can be marked as handled.
    /// </summary>
    /// <remarks>
    /// GridPasteCellTextEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.PasteCellText"/> event to save text information into 
    /// a specified cell.
    /// <para/>
    /// This event allows you to customize how to handle text pasted into a cell at run-time on demand.
    /// <para/>
    /// If you do not wish the grid to make any changes to the cell,
    /// set <see cref="CancelEventArgs.Cancel"/> to True. The grid will check this
    /// flag to see whether it should make changes to the cell.
    /// <para/>
    /// If you wish that the grid aborts the current paste operation (in case several cells are pasted),
    /// set the <see cref="GridPasteCellTextEventArgs.Abort"/> flag to True.
    /// <para/>
    /// The GridPasteCellTextEventArgs members, e.ColIndex and e.RowIndex, specify column and row of the cell. The e.Style member holds the
    /// GridStyleInfo object for the cell.
    /// </remarks>
    /// <seealso cref="GridPasteCellTextEventHandler"/>
    /// <seealso cref="GridModel.PasteCellText"/>
    public sealed class GridPasteCellTextEventArgs : SyncfusionCancelEventArgs
    {
        int rowIndex;
        int colIndex;
        GridStyleInfo style;
        string text;
        bool abort;

        /// <summary>
        /// Initializes a new <see cref="GridPasteCellTextEventArgs"/> object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The style information for the cell.</param>
        /// <param name="text">The text to be pasted into the cell.</param>
        /// <param name="abort">Specifies if operation should be aborted.</param>
        public GridPasteCellTextEventArgs(int rowIndex, int colIndex, GridStyleInfo style, string text, bool abort)
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.style = style;
            this.text = text;
            this.abort = abort;
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }
        }

        /// <summary>
        /// Gets the style information for the cell.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        /// Gets or sets the text to be pasted into the cell.
        /// </summary>
        [TraceProperty(true)]
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether operation should be aborted.
        /// </summary>
        [TraceProperty(true)]
        public bool Abort
        {
            get
            {
                return abort;
            }

            set
            {
                abort = value;
            }
        }
    }
}