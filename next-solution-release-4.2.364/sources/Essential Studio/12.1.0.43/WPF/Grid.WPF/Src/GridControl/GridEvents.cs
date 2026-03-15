#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.Windows.ComponentModel;
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Media;
using System.ComponentModel;
using System;

namespace Syncfusion.Windows.Controls.Grid
{
    #region CopyPaste
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
        /// <summary>
        /// For hold the the Clipboard information
        /// </summary>
        private DataObject dataObject = null;

        /// <summary>
        /// Its contain the range of cells cut or copied
        /// </summary>
        private GridRangeInfoList rangeList = null;

        /// <summary>
        /// Its contain the Clipboard information
        /// </summary>
        private string clipboardText = string.Empty;

        /// <summary>
        /// Initializes a new <see cref="GridCutPasteEventArgs"/> object.
        /// </summary>
        /// <param name="rangeList">You can save here a list of ranges that have been copied.</param>
        public GridCutPasteEventArgs(GridRangeInfoList rangeList)
        {
            this.rangeList = rangeList;
        }

        public GridCutPasteEventArgs(GridRangeInfoList rangeList,string buffer)
        {
            this.rangeList = rangeList;
            this.clipboardText = buffer;
        }

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
        /// Gets or sets the DataObject to be used for further clipboard operations. This property will only be checked by the calling
        /// method if you do not set <see cref="SyncfusionHandledEventArgs.Handled"/> to true. Can be NULL.
        /// </summary>
        public DataObject DataObject
        {
            get
            {
                return this.dataObject;
            }

            set
            {
                this.dataObject = value;
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
                return this.rangeList;
            }

            set
            {
                this.rangeList = value;
            }
        }

        public string ClipboardText
        {
            get
            {
                return this.clipboardText;
            }
            set
            {
                this.clipboardText = value;
            }
        }
    }
    #endregion


    #region DragDrop
    /// <summary>
    /// Occurs when the when the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and
    /// before the data are applied to the grid.
    /// </summary>
    /// <remarks>
    /// This event lets you provide your own customized paste data behavior.
    /// <para/>
    /// If you do not wish the grid to proceed with default behavior for this method,
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True. The grid will check this
    /// flag to see whether it should proceed. If you set it to True, the calling method
    /// will check <see cref="GridOleDropAtRowColEventArgs.Result"/> as indication if the
    /// operation was successful.
    /// <para/>
    /// If you want the grid to proceed with default behavior, do not change <see cref="SyncfusionHandledEventArgs.Handled"/>.
    /// </remarks>
    /// <seealso cref="GridOleDropAtRowColEventHandler"/>
    [Description("Occurs when the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and before the data are applied to the grid."),
    Category("Behavior")]        
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
    /// Represents the method that handles a cancelable <see cref="GridControlBase.QueryCanOleDragRange"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridQueryCanOleDragRangeEventArgs"/> that contains the event data.</param>  
    public delegate void GridExcelLikeDragRangeEventHandler(object sender, GridQueryCanDragRangeEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.QueryCanOleDragRange"/> event.
    /// </summary>
    /// <remarks>
    /// GridQueryCanOleDragRangeEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.QueryCanOleDragRange"/> event to determine whether
    /// a specified range can serve as an OLE drag source. The event is fired when the user
    /// hovers the mouse over the edge of a selected range.
    /// <para/>
    /// You can disallow the specified range to be used as OLE Data Source when
    /// you assign true to <see cref="CancelEventArgs.Cancel"/>.
    /// </remarks>
    /// <seealso cref="GridExcelLikeDragRangeEventHandler"/>
    /// <seealso cref="GridControlBase.QueryCanOleDragRange"/>   
    public class GridQueryCanDragRangeEventArgs : SyncfusionCancelEventArgs
    {
        private GridRangeInfo range;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="range">The range to be used as OLE data source.</param>     
        public GridQueryCanDragRangeEventArgs(GridRangeInfo range)
        {
            this.range = range;
        }

        internal GridQueryCanDragRangeEventArgs(GridRangeInfo range, bool cancel)
            : base(cancel)
        {
            this.range = range;
        }

        /// <summary>
        /// Gets the range to be used as OLE data source.
        /// </summary>
        [TraceProperty(true)]        
        public GridRangeInfo Range
        {
            get
            {
                return this.range;
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
        GridDragDropFlags dragDropFlags;
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
        public GridQueryOleDataSourceDataEventArgs(IDataObject dataObject, GridRangeInfoList rangeList, GridDragDropFlags dragDropFlags, int rowCount, int colCount)
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
        public GridDragDropFlags DragDropFlags
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

    #endregion

    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.WrapCellNextControlInForm"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name=" e">An <see cref="GridWrapCellNextControlInFormEventArgs"/> that contains the event data.</param>
    public delegate void GridWrapCellNextControlInFormEventHandler(object sender, GridWrapCellNextControlInFormEventArgs e);


    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.WrapCellNextControlInForm"/> event.
    /// </summary>
    /// <remarks>
    /// GridWrapCellNextControlInFormEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.WrapCellNextControlInForm"/> event to notify you 
    /// when the grid is about to be left because the user is at the top-left or bottom-right
    /// cell and about to tab out of the grid.
    /// <para/>
    /// This event is only raised if the <see cref="GridWrapCellBehavior.NextControlInForm"/>
    /// has been specified for <see cref="GridModelOptions.WrapCell"/>.
    /// </remarks>
    /// <seealso cref="GridWrapCellNextControlInFormEventHandler"/>
    public sealed class GridWrapCellNextControlInFormEventArgs : SyncfusionRoutedEventArgs
    {
        bool forward;
        bool moveTopLeft;

        /// <summary>
        /// Initializes a new instance of the GridWrapCellNextControlInFormEventArgs class.
        /// </summary>
        /// <param name="forward">Indicates if next or previous control in form should be selected.</param>
        /// <param name="moveTopLeft">When moving to the next control indicates if grid should move current cell
        /// to the top-left position.</param>
        /// <param name="routedEvent">Routed event.</param>
        /// <param name="source">Event source.</param>
        public GridWrapCellNextControlInFormEventArgs(bool forward, bool moveTopLeft, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.forward = forward;
            this.moveTopLeft = moveTopLeft;
        }

        /// <summary>
        /// Indicates if next or previous control in form should be selected.
        /// </summary>
        [TraceProperty(true)]
        public bool Forward
        {
            get
            {
                return forward;
            }

        }

        /// <summary>
        /// When moving to the next control indicates if grid should move current cell
        /// to the top-left position.
        /// </summary>
        [TraceProperty(true)]
        public bool MoveTopLeft
        {
            get
            {
                return moveTopLeft;
            }
            set
            {
                moveTopLeft = value;
            }
        }
    }

    // eva GridMoveCurrentCellDirection GridQueryNextCurrentCellPosition int numCells

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.MoveCurrentCellDirection"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridMoveCurrentCellDirectionEventArgs"/> that contains the event data.</param>
    public delegate void GridMoveCurrentCellDirectionEventHandler(object sender, GridMoveCurrentCellDirectionEventArgs e);

    /// <summary>
    /// Holds data for the <see cref="GridControlBase.MoveCurrentCellDirection"/> which lets you customize
    /// how the current cell is moved when the user navigates through the grid with arrow keys.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="GridControlBase.QueryNextCurrentCellPosition"/>.
    /// <para/>
    /// <see cref="GridControlBase.QueryNextCurrentCellPosition"/> occurs before the the current cell is moved into a specific direction. Normally, cells that are not
    /// marked as enabled with <see cref="GridStyleInfo.Enabled"/> will be skipped but you can hook into this
    /// mechanism by implementing a event handler for this event.
    /// <para/>
    /// If you want to customize the behavior, manually call CurrentCell.MoveTo from within the
    /// event handler and set e.Handled = True;
    /// </remarks>
    /// 
    /// <example>The following example implements wrapping the current cell to the next row at the end of a row:
    /// <code lang="C#">
    /// 
    /// 		private void gridControl1_MoveCurrentCellDirection(object sender, GridMoveCurrentCellDirectionEventArgs e)
    /// 		{
    /// 			GridControlBase grid = sender as GridControlBase;
    /// 			GridModel gridModel = grid.Model;
    /// 			int row = e.RowIndex;
    /// 			int col = e.ColIndex;
    /// 			switch (e.Direction)
    /// 			{
    /// 				case GridDirectionType.Right:
    /// 				{
    /// 					col++;
    /// 					if (col > gridModel.ColCount)
    /// 					{
    /// 						row++;
    /// 						col = grid.LeftColIndex;
    /// 					}
    /// 
    /// 					while (row &lt; gridModel.RowCount)
    /// 					{
    /// 						using (GridStyleInfo style = grid.GetViewStyleInfo(row, col))
    /// 						{
    /// 							if (style.Enabled)
    /// 							{
    /// 								e.Result = grid.CurrentCell.MoveTo(row, col);
    /// 								e.Handled = true;
    /// 								return;
    /// 							}
    /// 
    /// 							col++;
    /// 							if (col > gridModel.ColCount)
    /// 							{
    /// 								row++;
    /// 								col = grid.LeftColIndex;
    /// 							}
    /// 						}
    /// 					}
    /// 					e.Handled = true;
    /// 					e.Result = false;
    /// 					break;
    /// 				}
    /// 				case GridDirectionType.Left:
    /// 				{
    /// 					col--;
    /// 					if (col == gridModel.Cols.HeaderCount)
    /// 					{
    /// 						row--;
    /// 						col = gridModel.ColCount;
    /// 					}
    /// 
    /// 					while (row > gridModel.Rows.HeaderCount)
    /// 					{
    /// 						using (GridStyleInfo style = grid.GetViewStyleInfo(row, col))
    /// 						{
    /// 							if (style.Enabled)
    /// 							{
    /// 								e.Result = grid.CurrentCell.MoveTo(row, col);
    /// 								e.Handled = true;
    /// 								return;
    /// 							}
    /// 
    /// 							col--;
    /// 							if (col == gridModel.Cols.HeaderCount)
    /// 							{
    /// 								row--;
    /// 								col = gridModel.ColCount;
    /// 							}
    /// 						}
    /// 					}
    /// 					e.Handled = true;
    /// 					e.Result = false;
    /// 					break;
    /// 				}
    /// 			}
    /// 
    /// 		}
    /// 	}
    /// 
    /// </code>
    /// </example>
    public class GridMoveCurrentCellDirectionEventArgs : GridQueryNextCurrentCellPositionEventArgs
    {
        int numCells;
        bool extendSelection;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="numCells">The number of cells to move.</param>
        /// <param name="extendSelection">Extends the current selection.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridMoveCurrentCellDirectionEventArgs(GridDirectionType direction, int numCells, bool extendSelection, int rowIndex, int colIndex, RoutedEvent routedEvent, object source)
            : base(direction, rowIndex, colIndex, routedEvent, source)
        {
            this.numCells = numCells;
            this.extendSelection = extendSelection;
        }

        /// <summary>
        /// >The number of cells to move.
        /// </summary>
        [TraceProperty(true)]
        public int NumCells
        {
            get
            {
                return numCells;
            }
        }

        /// <summary>
        /// Specifies whether to extend the current selection.
        /// </summary>
        [TraceProperty(true)]
        public bool ExtendSelection
        {
            get
            {
                return this.extendSelection;
            }
        }

    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridQueryNextCurrentCellPositionEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryNextCurrentCellPositionEventHandler(object sender, GridQueryNextCurrentCellPositionEventArgs e);

    /// <summary>
    /// Holds and lets you customize row and column coordinates for 
    /// the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="GridControlBase.QueryNextCurrentCellPosition"/>.
    /// <para/>
    /// <see cref="GridControlBase.QueryNextCurrentCellPosition"/> occurs before the the current cell is moved into a specific direction. Normally, cells that are not
    /// marked as enabled with <see cref="GridStyleInfo.Enabled"/> will be skipped but you can hook into this
    /// mechanism by implementing a event handler for this event.
    /// <para/>
    /// See the SampleGrid class in the gridpad sample for an example.
    /// </remarks>
    public class GridQueryNextCurrentCellPositionEventArgs : SyncfusionRoutedEventArgs
    {
        GridDirectionType direction;
        int rowIndex;
        int colIndex;
        bool result = false;


        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridQueryNextCurrentCellPositionEventArgs(GridDirectionType direction, int rowIndex, int colIndex, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.direction = direction;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        /// <summary>
        /// The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.
        /// </summary>
        [TraceProperty(true)]
        public GridDirectionType Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        /// <summary>
        /// The row index.
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
        /// The column index.
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
        /// The resulting value. Set this to True if current cell can be moved in a specified direction;
        /// False if not. Don't forget to also set Handled to True.
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
    // eva ScrollPositionChanging SyncfusionCancel int scrollPosition


    /// <summary>
    /// Represents the method that handles the <see cref="GridModel.RowsInserted"/> and <see cref="GridModel.ColumnsInserted"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRangeInsertedEventArgs"/> that contains the event data.</param>
    public delegate void GridRangeInsertedEventHandler(object sender, GridRangeInsertedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsInserted"/> and <see cref="GridModel.ColumnsInserted"/> events.
    /// </summary>
    /// <remarks>
    /// GridRangeInsertedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsInserted"/> and <see cref="GridModel.ColumnsInserted"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it has inserted the specified number of rows
    /// or column into its data store and all associated views should redraw affected display contents.
    /// <para/>
    /// The <see cref="SyncfusionRoutedEventArgs.Handled"/> property indicates if all changes were successfully made to the model. If it is
    /// False, the operation was aborted. However, the view contents need to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridRangeInsertedEventHandler"/>
    public sealed class GridRangeInsertedEventArgs : SyncfusionRoutedEventArgs
    {
        private int insertAt;
        private int count;

        /// <summary>
        /// Initializes a new <see cref="GridRangeInsertedEventArgs"/>.
        /// </summary>
        /// <param name="insertAt">The row or column index where the cells should be inserted before.</param>
        /// <param name="count">The number of rows or columns.</param>
        /// <param name="iro">Information about the cells to be inserted such as cell contents, row, and column sizes and more.</param>
        /// <param name="success">Indicates whether an operation was successful.</param>
        public GridRangeInsertedEventArgs(int insertAt, int count)
        {
            this.insertAt = insertAt;
            this.count = count;
        }

        /// <summary>
        /// The row or column index where the cells should be inserted before.
        /// </summary>
        [TraceProperty(true)]
        public int InsertAt
        {
            get { return insertAt; }
        }

        /// <summary>
        /// The number of rows or columns.
        /// </summary>
        [TraceProperty(true)]
        public int Count
        {
            get { return count; }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridModel.RowsRemoved"/> and <see cref="GridModel.ColumnsRemoved"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRangeRemovedEventArgs"/> that contains the event data.</param>
    public delegate void GridRangeRemovedEventHandler(object sender, GridRangeRemovedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsRemoved"/> and <see cref="GridModel.ColumnsRemoved"/> events.
    /// </summary>
    /// <remarks>
    /// GridRangeRemovedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsRemoved"/> and <see cref="GridModel.ColumnsRemoved"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it has 
    /// rearranged the specified rows or columns in its data store and all associated views
    /// should redraw affected display contents.
    /// <para/>
    /// The <see cref="SyncfusionRoutedEventArgs.Handled"/> property indicates if all changes
    /// were successfully made to the model. If it is false, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridRangeRemovedEventHandler"/>
    public sealed class GridRangeRemovedEventArgs : SyncfusionRoutedEventArgs
    {
        private int removeAt;
        private int count;

        /// <summary>
        /// Constructs a <see cref="GridRangeRemovedEventArgs"/>.
        /// </summary>
        /// <param name="from">The index of the first row or column that was removed.</param>
        /// <param name="last">The index of the last row or column that was removed.</param>
        /// <param name="iro">Provides information about the cells that have been removed such as
        /// row heights, column widths, and hidden state of rows or columns. </param>
        /// <param name="success">Indicates wheter an operation was successful.</param>
        public GridRangeRemovedEventArgs(int removeAt, int count)
        {
            this.removeAt = removeAt;
            this.count = count;
        }

        /// <summary>
        /// The index of the first row or column that was removed.
        /// </summary>
        [TraceProperty(true)]
        public int RemoveAt
        {
            get { return removeAt; }
        }

        /// <summary>
        /// The index of the last row or column that was removed.
        /// </summary>
        [TraceProperty(true)]
        public int Count
        {
            get { return count; }
        }

    }

    /// <summary>
    /// Represents the method that handles <see cref="GridModel.RowsMoved"/> and <see cref="GridModel.ColumnsMoved"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridRangeMovedEventArgs"/> that contains the event data.</param>
    public delegate void GridRangeMovedEventHandler(object sender, GridRangeMovedEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.RowsMoved"/> and <see cref="GridModel.ColsMoved"/> events.
    /// </summary>
    /// <remarks>
    /// GridRangeMovedEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.RowsMoved"/> and <see cref="GridModel.ColumnsMoved"/> events.
    /// <para/>
    /// This event is raised by the model to notify all associated views that it has 
    /// rearranged the specified rows or columns in its data store and all associated views
    /// should redraw affected display contents.
    /// <para/>
    /// The <see cref="SyncfusionRoutedEventArgs.Handled"/> property indicates if all changes
    /// were successfully made to the model. If it is False, the operation was aborted. However, the view contents need
    /// to be redrawn no matter if the operation was successful or not.
    /// </remarks>
    /// <seealso cref="GridRangeMovedEventHandler"/>
    public sealed class GridRangeMovedEventArgs : SyncfusionRoutedEventArgs
    {
        private int removeAt;
        private int count;
        private int insertAt;

        /// <summary>
        /// Constructs a <see cref="GridRangeMovedEventArgs"/>.
        /// </summary>
        /// <param name="from">The index of the first row or column that was removed.</param>
        /// <param name="last">The index of the last row or column that was removed.</param>
        /// <param name="iro">Provides information about the cells that have been removed such as
        /// row heights, column widths, and hidden state of rows or columns. </param>
        /// <param name="success">Indicates wheter an operation was successful.</param>
        public GridRangeMovedEventArgs(int removeAt, int count, int insertAt)
        {
            this.removeAt = removeAt;
            this.count = count;
            this.insertAt = insertAt;
        }

        /// <summary>
        /// The index of the first row or column that was removed.
        /// </summary>
        [TraceProperty(true)]
        public int RemoveAt
        {
            get { return removeAt; }
        }

        /// <summary>
        /// The index of the last row or column that was removed.
        /// </summary>
        [TraceProperty(true)]
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// The row or column index where the cells should be inserted before.
        /// </summary>
        [TraceProperty(true)]
        public int InsertAt
        {
            get { return insertAt; }
        }
    }

    /// <summary>
    /// Represents a method that handles <see cref="GridControlBase.CellClick"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellClickEventArgs"/> that contains the event data.</param>
    public delegate void GridCellClickEventHandler(object sender, GridCellClickEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridControlBase.CellClick"/> event.
    /// </summary>
    /// <remarks>GridCellClickEventArgs is a custom event argument class used by the <see cref="GridControlBase.CellClick"/>
    /// event when the user clicks inside a cell.</remarks>
    /// <seealso cref="GridControlBase.RaiseGridCellClick"/>
    public class GridCellClickEventArgs : SyncfusionRoutedEventArgs
    {
        int clicks;
        /// <summary>
        /// Initializes a new <see cref="GridCellClickEventArgs"/>.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        /// <param name="clicks">Number of clicks.</param>
        /// <param name="routedEvent">Routed event.</param>
        /// <param name="source">Event source.</param>
        public GridCellClickEventArgs(int rowIndex, int columnIndex, int clicks, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this._rowIndex = rowIndex;
            this._columnIndex = columnIndex;
            this.clicks = clicks;
        }

        private int _rowIndex;
        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return this._rowIndex;
            }
        }

        private int _columnIndex;
        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        /// <value>The index of the column.</value>
        [TraceProperty(true)]
        public int ColumnIndex
        {
            get
            {
                return this._columnIndex;
            }
        }

        /// <summary>
        /// Number of clicks in the cell.
        /// </summary>
        public int ClickCount
        {
            get
            {
                return this.clicks;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.CellButtonClick"/>
    /// event that is raised when the user clicks on a cell button element inside a cell.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellButtonClickEventArgs"/> that contains the event data.</param>
    public delegate void GridCellButtonClickEventHandler(object sender, GridCellButtonClickEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridControlBase.CellButtonClick"/>
    /// event that is raised when the user clicks on a cell button element inside a cell.
    /// </summary>
    /// <seealso cref="GridCellButtonClickEventHandler"/>
    public class GridCellButtonClickEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellButtonClickEventArgs"/>.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        /// <param name="routedEvent">Routed event.</param>
        /// <param name="source">Event source.</param>
        public GridCellButtonClickEventArgs(int rowIndex, int columnIndex, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this._rowIndex = rowIndex;
            this._columnIndex = columnIndex;
        }

        private int _rowIndex;
        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return this._rowIndex;
            }
        }

        private int _columnIndex;
        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        /// <value>The index of the column.</value>
        [TraceProperty(true)]
        public int ColumnIndex
        {
            get
            {
                return this._columnIndex;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.CellCursor"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="GridCellCursorEventArgs"/> that contains the event data.</param>
    public delegate void GridCellCursorEventHandler(object sender, GridCellCursorEventArgs args);

    /// <summary>
    /// Provides data about the <see cref="GridControlBase.CellCursor"/> event when the grid queries
    /// which cursor to be displayed for a cell.
    /// </summary>
    /// <seealso cref="GridCellCursorEventHandler"/>
    public class GridCellCursorEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellCursorEventArgs"/>.
        /// </summary>
        /// <param name="routedEvent">Routed event.</param>
        /// <param name="source">Event source.</param>
        public GridCellCursorEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source) { }

        /// <summary>
        /// Gets or sets the cursor.
        /// </summary>
        /// <value>The cursor.</value>
        [TraceProperty(true)]
        public Cursor Cursor
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a method that handles cell-related mouse events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">A <see cref="GridCellMouseEventArgs"/> that contains the event data.</param>
    public delegate void GridCellMouseEventHandler(object sender, GridCellMouseEventArgs args);

    /// <summary>
    /// Holds the MouseEvent argument values.
    /// </summary>
    /// <remarks>
    /// Events related to the mouse events use this.
    /// </remarks>
    /// <seealso cref="GridCellMouseEventHandler"/>
    public class GridCellMouseEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellMouseEventArgs"/>.
        /// </summary>
        /// <param name="routedEvent">Routed event.</param>
        /// <param name="source">Event source.</param>
        public GridCellMouseEventArgs(RoutedEvent routedEvent,
            object source)
            : base(routedEvent, source)
        {
        }

        /// <summary>
        /// The mouse event arguments.
        /// </summary>
        [TraceProperty(true)]
        public MouseEventArgs MouseEventArgs
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Represents a method that handles mouse contrller argument values.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">A <see cref="GridCellMouseControllerEventArgs"/> that contains the event data.</param>
    public delegate void GridCellMouseControllerEventHandler(object sender, GridCellMouseControllerEventArgs args);

    /// <summary>
    /// Holds the MouseController argument values.
    /// </summary>
    /// <remarks>
    /// Used by the mouse related operations done by IMouseController implemented objects.
    /// </remarks>
    public class GridCellMouseControllerEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellMouseControllerEventArgs"/>.
        /// </summary>
        /// <param name="routedEvent">Routed event.</param>
        /// <param name="source">Event source.</param>
        public GridCellMouseControllerEventArgs(RoutedEvent routedEvent,
            object source)
            : base(routedEvent, source)
        {
        }

        /// <summary>
        /// Gets or sets the mouse controller event args.
        /// </summary>
        /// <value>The mouse controller event args.</value>
        [TraceProperty(true)]
        public MouseControllerEventArgs MouseControllerEventArgs
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Represents a method that handles <see cref="GridControlBase.ResizingColumns"/> event.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="args">A <see cref="GridResizingColumnsEventArgs"/> that contains the event data.</param>
    public delegate void GridResizingColumnsEventHandler(object sender, GridResizingColumnsEventArgs args);
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.ResizingColumns"/> event.
    /// </summary>
    /// <remarks>
    /// GridResizingColumnsEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.ResizingColumns"/> event when the user is about to resize
    /// a column or is in the process of resizing a column. 
    /// <para/>
    /// You can disallow the resizing of specific columns at run-time when
    /// you assign True to <see cref="RoutedEventArgs.Handled"/>.<para/>
    /// You can also limit resizing columns to a given maximum value by changing the <see cref="GridResizingColumnsEventArgs.Width"/>
    /// value.
    /// </remarks>
    /// <seealso cref="GridResizingColumnsEventHandler"/>
    /// <seealso cref="GridControlBase.ResizingColumns"/>
    public sealed class GridResizingColumnsEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridResizingColumnsEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public GridResizingColumnsEventArgs(RoutedEvent routedEvent,
            object source)
            : base(routedEvent, source)
        {
            AllowResize = true;
        }

        /// <summary>
        /// Gets or sets the index of the column.
        /// </summary>
        /// <value>The index of the column.</value>
        [TraceProperty(true)]
        public GridRangeInfo Columns
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates whether the resizing is happening for hidden columns.
        /// </summary>
        [TraceProperty(true)]
        public bool InHiddenColResize
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [TraceProperty(true)]
        public double Width
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the reason for this event and hints about the current state of the user action.
        /// </summary>
        public GridResizeCellsReason Reason
        {
            get;
            internal set;
        }

        /// <summary>
        /// Specifies the mouse hit point.
        /// </summary>
        public Point Point
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current column is resizable.
        /// </summary>
        public bool AllowResize
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.ResizingRows"/> event.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="args">A <see cref="GridResizingRowsEventArgs"/> that contains the event data.</param>
    public delegate void GridResizingRowsEventHandler(object sender, GridResizingRowsEventArgs args);
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.ResizingRows"/> event.
    /// </summary>
    /// <remarks>
    /// GridResizingRowsEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.ResizingRows"/> event when the user is about to resize
    /// a row or is in the process of resizing a row. 
    /// <para/>
    /// You can disallow the resizing of specific rows at run-time when
    /// you assign True to <see cref="RoutedEventArgs.Handled"/>.<para/>
    /// You can also limit resizing rows to a given maximum value by changing the <see cref="GridResizingRowsEventArgs.Height"/>
    /// value.
    /// </remarks>
    /// <seealso cref="GridResizingRowsEventHandler"/>
    /// <seealso cref="GridControlBase.ResizingRows"/>
    public sealed class GridResizingRowsEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridResizingRowsEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public GridResizingRowsEventArgs(RoutedEvent routedEvent,
            object source)
            : base(routedEvent, source)
        {
            AllowResize = true;
        }

        /// <summary>
        /// Gets or sets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        [TraceProperty(true)]
        public GridRangeInfo Rows
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates whether the resizing is happening for hidden rows.
        /// </summary>
        [TraceProperty(true)]
        public bool InHiddenRowResize
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [TraceProperty(true)]
        public double Height
        {
            get;
            internal set;
        }

        /// <summary>
        /// Specifies the reason for this event and hints about the current state of the user action.
        /// </summary>
        public GridResizeCellsReason Reason
        {
            get;
            internal set;
        }

        /// <summary>
        /// Specifies the mouse hit point.
        /// </summary>
        public Point Point
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current column is resizable.
        /// </summary>
        public bool AllowResize
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.CellRender"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridDrawCellEventArgs"/> that contains the event data.</param>
    public delegate void GridCellRenderEventHandler(object sender, GridCellRenderEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.CellRender"/> event.
    /// </summary>
    /// <remarks>
    /// GridDrawCellEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.DrawCell"/> event to allow custom drawing of
    /// a cell. Set the Cancel property true if you have drawn the cell contents and
    /// do not want the grid to proceed with default drawing of the cell.
    /// </remarks>
    /// <seealso cref="GridDrawCellEventHandler"/>
    public sealed class GridCellRenderEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellRenderEventArgs"/>.
        /// </summary>
        /// <param name="style">Cell style.</param>
        /// <param name="rca">Render cell information.</param>
        /// <param name="dc">Drawing context.</param>
        /// <param name="routedEvent">Routed event.</param>
        /// <param name="source">Event source.</param>
        public GridCellRenderEventArgs(GridStyleInfo style, RenderCellArgs rca, DrawingContext dc, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.DrawingContext = dc;
            this.Style = style;
            this.RenderCellArgs = rca;
        }

        /// <summary>
        /// Gets the drawing context associated with the current cell that is getting rendered. Do not dispose the DrawingContext in the event.
        /// </summary>
        /// <value>The drawing context.</value>
        [TraceProperty(true)]
        public DrawingContext DrawingContext
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the style object associated with that cell.
        /// </summary>
        /// <remarks>
        /// Changes to the style object are allowed. However, these changes
        /// will not be saved back in the grid or cached. But, you can make some
        /// adjustments to the style object just before the cell is drawn.
        /// You cannot change Base Style, Interior, or Cell Type at this time.
        /// See <see cref="GridControlBase.PrepareViewStyleInfo"/> for changing
        /// interior or other style formattings.
        /// </remarks>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get;
            private set;
        }

        /// <summary>
        /// Layout information about the cell.
        /// </summary>
        [TraceProperty(true)]
        public RenderCellArgs RenderCellArgs
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Represents a method that handles cell key events for grid.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="args">A <see cref="GridCellKeyEventArgs"/> that contains the event data.</param>
    public delegate void GridCellKeyEventHandler(object sender, GridCellKeyEventArgs args);

    /// <summary>
    /// Holds the key event argument values.
    /// </summary>
    public sealed class GridCellKeyEventArgs : SyncfusionCancelRoutedEventArgs
    {
        public GridCellKeyEventArgs(RoutedEvent routedEvent, object source, KeyEventArgs keyEventArgs)
            : base(routedEvent, source)
        {
            this.KeyEventArgs = keyEventArgs;
        }

        /// <summary>
        /// Provides information about the key events.
        /// </summary>
        public KeyEventArgs KeyEventArgs
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifies possible key values on a key board.
        /// </summary>
        public Key Key
        {
            get
            {
                return this.KeyEventArgs.Key;
            }
        }
    }

    public delegate void CellRequestNavigateEventHandler(object sender, CellRequestNavigateEventArgs e);

    public sealed class CellRequestNavigateEventArgs : SyncfusionRoutedEventArgs
    {
        public CellRequestNavigateEventArgs(string name, int rowIndex, int columnIndex, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.Name = name;
            this.RowIndex = rowIndex;
            this.ColumnIndex = columnIndex;
        }

        public CellRequestNavigateEventArgs(Uri uri, int rowIndex, int columnIndex, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.RowIndex = rowIndex;
            this.ColumnIndex = columnIndex;
            this.Uri = uri;
        }

        public string Name
        {
            get;
            set;
        }

        public int RowIndex
        {
            get;
            set;
        }

        public int ColumnIndex
        {
            get;
            set;
        }

        public Uri Uri
        {
            get;
            set;
        }
    }


    public delegate void CurrentCellValidatingEventHandler(object sender, CurrentCellValidatingEventArgs e);

    public class CurrentCellValidatingEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentCellValidatingEventArgs"/> class.
        /// </summary>
        public CurrentCellValidatingEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentCellValidatingEventArgs"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        public CurrentCellValidatingEventArgs(GridStyleInfo style)
        {
            Cancel = false;
            Style = style;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentCellValidatingEventArgs"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The source.</param>
        public CurrentCellValidatingEventArgs(GridStyleInfo style, object oldValue, object newValue, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            Cancel = false;
            Style = style;
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CurrentCellValidatingEventArgs"/> is cancel.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public object OldValue { get; internal set; }

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public object NewValue { get; set; }

        /// <summary>
        /// Gets the Cell Style.
        /// </summary>
        /// <value>The style.</value>
        public GridStyleInfo Style { get; internal set; }
    }
}
