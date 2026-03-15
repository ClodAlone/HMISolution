#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

#if !WinRT
using Syncfusion.Windows.ComponentModel;
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Input;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.ComponentModel;
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
using System;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
namespace Syncfusion.WinRT.Controls.Grid
#endif
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


#if (!SILVERLIGHT && !WinRT)
        private DataObject dataObject = null;
#else
        private string dataObject = string.Empty;
#endif


        /// <summary>
        /// Its contain the range of cells cut or copied
        /// </summary>
        private GridRangeInfoList rangeList = null;

        /// <summary>
        /// Initializes a new <see cref="GridCutPasteEventArgs"/> object.
        /// </summary>
        /// <param name="rangeList">You can save here a list of ranges that have been copied.</param>
        public GridCutPasteEventArgs(GridRangeInfoList rangeList)
        {
            this.rangeList = rangeList;
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

#if (!SILVERLIGHT && !WinRT)
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

#else
        /// <summary>
        /// Gets or sets the DataObject to be used for further clipboard operations. This property will only be checked by the calling
        /// method if you do not set <see cref="SyncfusionHandledEventArgs.Handled"/> to true. Can be NULL.
        /// </summary>
        public string DataObject
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

#endif

        /// <summary>
        /// Gets or sets range list. You can save here a list of ranges that have been copied. This property will only be checked for the
        /// Copy operation. Can be NULL.
        /// </summary>

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
    /// 
#if !WinRT
    [Description("Occurs when the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and before the data are applied to the grid."),
    Category("Behavior")]
#endif
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
        GridDataObject dataObject;
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
        public GridOleDropAtRowColEventArgs(GridDataObject dataObject, int rowIndex, int colIndex, bool result)
        {
            this.dataObject = dataObject;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.result = result;
        }

        /// <summary>
        /// Gets or sets the Data Object with clipboard data.
        /// </summary>
        public GridDataObject DataObject
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
        GridDataObject dataObject;
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
        public GridQueryOleDataSourceDataEventArgs(GridDataObject dataObject, GridRangeInfoList rangeList, GridDragDropFlags dragDropFlags, int rowCount, int colCount)
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
        public GridDataObject DataObject
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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridWrapCellNextControlInFormEventArgs : SyncfusionRoutedEventArgs
    {
        bool forward;
        bool moveTopLeft;

        /// <summary>
        /// Initializes a new instance of the GridWrapCellNextControlInFormEventArgs class.
        /// </summary>
        /// <param name="forward">The <see cref="bool"/> object that holds cell information.</param>
        /// <param name="moveTopLeft"></param>
        public GridWrapCellNextControlInFormEventArgs(bool forward, bool moveTopLeft, object source)
            : base(source)
        {
            this.forward = forward;
            this.moveTopLeft = moveTopLeft;
        }

        /// <summary>
        /// Indicates if next or previous control in form should be selected.
        /// </summary>

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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
        public GridMoveCurrentCellDirectionEventArgs(GridDirectionType direction, int numCells, bool extendSelection, int rowIndex, int colIndex, object source)
            : base(direction, rowIndex, colIndex, source)
        {
            this.numCells = numCells;
            this.extendSelection = extendSelection;
        }

        /// <summary>
        /// >The number of cells to move.
        /// </summary>

        public int NumCells
        {
            get
            {
                return numCells;
            }
        }

        /// <summary>
        /// Extends the current selection.
        /// </summary>

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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
        public GridQueryNextCurrentCellPositionEventArgs(GridDirectionType direction, int rowIndex, int colIndex, object source)
            : base(source)
        {
            this.direction = direction;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        /// <summary>
        /// The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.
        /// </summary>

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



    public delegate void GridRangeInsertedEventHandler(object sender, GridRangeInsertedEventArgs e);

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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

        public int InsertAt
        {
            get { return insertAt; }
        }

        /// <summary>
        /// The number of rows or columns.
        /// </summary>

        public int Count
        {
            get { return count; }
        }
    }

    public delegate void GridRangeRemovedEventHandler(object sender, GridRangeRemovedEventArgs e);

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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

        public int RemoveAt
        {
            get { return removeAt; }
        }

        /// <summary>
        /// The index of the last row or column that was removed.
        /// </summary>

        public int Count
        {
            get { return count; }
        }

    }

    public delegate void GridRangeMovedEventHandler(object sender, GridRangeMovedEventArgs e);

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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

        public int RemoveAt
        {
            get { return removeAt; }
        }

        /// <summary>
        /// The index of the last row or column that was removed.
        /// </summary>

        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// The row or column index where the cells should be inserted before.
        /// </summary>

        public int InsertAt
        {
            get { return insertAt; }
        }
    }

    public delegate void GridCellClickEventHandler(object sender, GridCellClickEventArgs e);

    /// <summary>
    /// Holds the values when a cell is clicked.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellClickEventArgs : SyncfusionRoutedEventArgs
    {
        public GridCellClickEventArgs(int rowIndex, int columnIndex, object source)
            : base(source)
        {
            this._rowIndex = rowIndex;
            this._columnIndex = columnIndex;
        }

        private int _rowIndex;
        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>

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

        public int ColumnIndex
        {
            get
            {
                return this._columnIndex;
            }
        }
    }


    public delegate void GridCellButtonClickEventHandler(object sender, GridCellButtonClickEventArgs e);

    /// <summary>
    /// Holds the values when a cell is clicked.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellButtonClickEventArgs : SyncfusionRoutedEventArgs
    {
        public GridCellButtonClickEventArgs(int rowIndex, int columnIndex, object source)
            : base(source)
        {
            this._rowIndex = rowIndex;
            this._columnIndex = columnIndex;
        }

        private int _rowIndex;
        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>

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

        public int ColumnIndex
        {
            get
            {
                return this._columnIndex;
            }
        }
    }

#if !WinRT
    public delegate void GridCellCursorEventHandler(object sender, GridCellCursorEventArgs args);
    /// <summary>
    /// You can set the Cursor value for the mouse.
    /// </summary>
    public class GridCellCursorEventArgs : SyncfusionRoutedEventArgs
    {
        public GridCellCursorEventArgs(object source)
            : base(source) { }

        /// <summary>
        /// Gets or sets the cursor.
        /// </summary>
        /// <value>The cursor.</value>

        public Cursor Cursor
        {
            get;
            set;
        }
    }
#endif

    public delegate void GridCellMouseEventHandler(object sender, GridCellMouseEventArgs args);
    /// <summary>
    /// Holds the MouseEvent argument values.
    /// </summary>
    /// <remarks>
    /// Events related to the mouse events use this.
    /// </remarks>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellMouseEventArgs : SyncfusionRoutedEventArgs
    {
        public GridCellMouseEventArgs(object source)
            : base(source)
        {
        }


        public MouseEventArgs MouseEventArgs
        {
            get;
            internal set;
        }
    }

    public delegate void GridCellMouseControllerEventHandler(object sender, GridCellMouseControllerEventArgs args);
    /// <summary>
    /// Holds the MouseController argument values.
    /// </summary>
    /// <remarks>
    /// Used by the mouse related operations done by IMouseController implemented objects.
    /// </remarks>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellMouseControllerEventArgs : SyncfusionRoutedEventArgs
    {
        public GridCellMouseControllerEventArgs(object source)
            : base(source)
        {
        }

        /// <summary>
        /// Gets or sets the mouse controller event args.
        /// </summary>
        /// <value>The mouse controller event args.</value>

        public MouseControllerEventArgs MouseControllerEventArgs
        {
            get;
            internal set;
        }
    }

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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridResizingColumnsEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridResizingColumnsEventArgs"/> class.
        /// </summary>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public GridResizingColumnsEventArgs(object source)
            : base(source)
        {
            AllowResize = true;
        }

        /// <summary>
        /// Gets or sets the index of the column.
        /// </summary>
        /// <value>The index of the column.</value>

        public GridRangeInfo Columns
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates whether the resizing is happening for hidden columns.
        /// </summary>
        public bool InHiddenColResize
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>

        public double Width
        {
            get;
            set;
        }

        public GridResizeCellsReason Reason
        {
            get;
            internal set;
        }

        public Point Point
        {
            get;
            internal set;
        }

        public bool AllowResize
        {
            get;
            set;
        }
    }

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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridResizingRowsEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridResizingRowsEventArgs"/> class.
        /// </summary>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public GridResizingRowsEventArgs(object source)
            : base(source)
        {
            AllowResize = true;
        }

        /// <summary>
        /// Gets or sets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>

        public GridRangeInfo Rows
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates whether the resizing is happening for hidden rows.
        /// </summary>
        public bool InHiddenRowResize
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>

        public double Height
        {
            get;
            set;
        }

        public GridResizeCellsReason Reason
        {
            get;
            internal set;
        }

        public Point Point
        {
            get;
            internal set;
        }

        public bool AllowResize
        {
            get;
            set;
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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridCellKeyEventArgs : SyncfusionRoutedEventArgs
    {
        public GridCellKeyEventArgs(KeyEventArgs keyEventArgs)
        {
            this.KeyEventArgs = keyEventArgs;
        }
#if WinRT
        public GridCellKeyEventArgs(KeyRoutedEventArgs args)
        {
            this.KeyRoutedArgs = args;
        }

        public KeyRoutedEventArgs KeyRoutedArgs
        {
            get;
            private set;
        }
#endif

        /// <summary>
        /// Provides information about the key events.
        /// </summary>
        public KeyEventArgs KeyEventArgs
        {
            get;
            private set;
        }
        
#if !WinRT
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
#endif
    }

    public delegate void CellRequestNavigateEventHandler(object sender, CellRequestNavigateEventArgs e);
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class CellRequestNavigateEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CellRequestNavigateEventArgs"/> class.
        /// </summary>
        /// <param name="Name">The Target Name.</param>
        /// <param name="RowIndex">Target Row Index.</param>
        /// <param name="ColumnIndex">Target Column Index.</param>
        /// <param name="Uri">The URI.</param>
        /// <param name="CellValue">The cell value.</param>
        /// <param name="CellRowColumn">The cell row column index.</param>
        public CellRequestNavigateEventArgs(string Name, int RowIndex, int ColumnIndex, Uri Uri, RowColumnIndex CellRowColumn)
        {
            this.Name = Name;
            this.RowIndex = RowIndex;
            this.ColumnIndex = ColumnIndex;
            this.CellRowColumnIndex = CellRowColumn;
            this.Uri = Uri;
        }

        /// <summary>
        /// Gets the Target sheet name.
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the target row index.
        /// </summary>
        public int RowIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the target column index.
        /// </summary>
        /// <value>The index of the column.</value>
        public int ColumnIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public Uri Uri
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the Cell row column index.
        /// </summary>
        /// <value>The index of the cell row column.</value>
        public RowColumnIndex CellRowColumnIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the Record.
        /// </summary>
        /// <value>The record.</value>
        public object Record
        {
            get;
            internal set;
        }
    }

    public delegate void CurrentCellValidateEventHandler(object sender, CurrentCellValidateEventArgs e);
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class CurrentCellValidateEventArgs : EventArgs
    {
        public CurrentCellValidateEventArgs()
            : base()
        {
        }

        public CurrentCellValidateEventArgs(GridStyleInfo style)
        {
            Cancel = false;
            Style = style;
            Handled = false;
            SuspendMoveTo = false;
        }

        public CurrentCellValidateEventArgs(GridStyleInfo style, object oldValue, object newValue)
        {
            Cancel = false;
            Style = style;
            OldValue = oldValue;
            NewValue = newValue;
            Handled = false;
            SuspendMoveTo = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CurrentCellValidateEventArgs"/> is cancel.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// If it’s true then it will clear the selection after closing the popup window.
        /// </summary>
        public bool Handled { get; set; }

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

        /// <summary>
        /// Suspend the current cell moving after the validation.
        /// </summary>
        public bool SuspendMoveTo { get; set; }
    }

}
