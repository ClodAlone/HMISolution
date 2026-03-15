#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;

namespace Syncfusion.Windows.Grid.Olap
{
    /// <summary>
    /// Interaction logic for OlapPagingGrid.xaml
    /// </summary>
    public partial class OlapPagingGrid
    {
        #region Members

        /// <summary>
        /// Number of rows should be displayed in the first page.
        /// </summary>
        private int _FirstPageExpectedRows = 0;

        /// <summary>
        /// Number of rows should be displayed in all other pages.
        /// </summary>
        private int _ExpectedRowsPerPage = 0;

        /// <summary>
        /// Grid control used for displaying the paging.
        /// </summary>
        GridControl _GridControl = null;

        /// <summary>
        /// Row header count.
        /// </summary>
        private int _VerticalHeaderLength = 0;

        /// <summary>
        /// Column header count.
        /// </summary>
        private int _HorizontalHeaderLength = 0;

        /// <summary>
        /// Desired number of rows. This is a copy of Expected page size.
        /// </summary>
        private int _DesiredNumberOfRows = 0;

        /// <summary>
        /// Number of columns in the olap grid.
        /// </summary>
        private int _OriginalGridColumnCount = 0;

        /// <summary>
        /// Total number of rows in the olap grid.
        /// </summary>
        private int _TotalNumberOfRows = 0;

        /// <summary>
        /// Number of rows in the last page.
        /// </summary>
        private int _LastPageRows = 0;

        /// <summary>
        /// Grid range information.
        /// </summary>
        private GridRange _GridRangeInfoCells;

        /// <summary>
        /// Olap grid instance.
        /// </summary>
        private Syncfusion.Windows.Grid.Olap.OlapGrid _Olapgrid = new Syncfusion.Windows.Grid.Olap.OlapGrid();

        /// <summary>
        /// Indices of start and end postion for each page. 
        /// </summary>
        private Dictionary<int, Indices> _IndexDictionary = new Dictionary<int, Indices>();

        #endregion

        #region Constructor

        public OlapPagingGrid()
        {
            this._GridControl = new GridControl();
            this._GridControl.Model.HeaderColumns = 0;
            this._GridControl.Model.HeaderRows = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is last.
        /// </summary>
        /// <value><c>true</c> if this instance is last; otherwise, <c>false</c>.</value>
        public bool IsLast
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is first.
        /// </summary>
        /// <value><c>true</c> if this instance is first; otherwise, <c>false</c>.</value>
        public bool IsFirst
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the paging info.
        /// </summary>
        /// <value>The paging info.</value>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The olap grid.</value>
        public Syncfusion.Windows.Grid.Olap.OlapGrid Grid
        {
            get { return this._Olapgrid; }
            set { this._Olapgrid = value; }
        }

        /// <summary>
        /// Gets or sets the total page numbers.
        /// </summary>
        /// <value>The total page numbers.</value>
        public int TotalPageNumbers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        /// <value>The current page number.</value>
        public int CurrentPageNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the length of the horizontal header.
        /// </summary>
        /// <value>The length of the horizontal header.</value>
        public int HorizontalHeaderLength
        {
            get { return _HorizontalHeaderLength; }
            private set { _HorizontalHeaderLength = value; }
        }

        /// <summary>
        /// Gets or sets the length of the vertical header.
        /// </summary>
        /// <value>The length of the vertical header.</value>
        public int VerticalHeaderLength
        {
            get { return _VerticalHeaderLength; }
            private set { _VerticalHeaderLength = value; }
        }

        #endregion

        #region Paging Helper Methods

        /// <summary>
        /// Generates the pager grid. Calculates grid range.
        /// </summary>
        public void GeneratePagerGrid(int navigationValue, NavigationMode navigationMode)
        {
            this._FirstPageExpectedRows = SizeToRowConvertor.Convert(this.PagingInfo.FirstPagePreferredSize, this._Olapgrid);
            this._ExpectedRowsPerPage = SizeToRowConvertor.Convert(this.PagingInfo.ExpectedPageSize, this._Olapgrid);
            this._DesiredNumberOfRows = this._ExpectedRowsPerPage;

            //// Updates all the required memebers.
            this._GridRangeInfoCells = this.UpdateMembers(navigationValue, navigationMode);

            //// Calculates total number of pages.
            this.GetTotalNumberOfPages();            
        }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <returns>The Indices of the page containing starting row and ending row of the page.</returns>
        public Indices GetIndices(int pageNumber)
        {
            if (this._IndexDictionary.Keys.Contains(pageNumber))
            {
                return this._IndexDictionary[pageNumber];
            }
            else
            {
                throw new ArgumentException("Page not found for the specified index in the paging grid.");
            }
        }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        /// <returns>The total number of pages in the grid.</returns>
        private void GetTotalNumberOfPages()
        {
            int fp = SizeToRowConvertor.Convert(this.PagingInfo.FirstPagePreferredSize, _Olapgrid);
            int ep = SizeToRowConvertor.Convert(this.PagingInfo.ExpectedPageSize, this._Olapgrid);

            //// Total number of rows = (firstPreferredRows + (ExpectedRows * loopingConstantPageNumber) + rowsInLastPage)            
            for (int i = 1; ; i++)
            {
                this._LastPageRows = this._TotalNumberOfRows - (fp + (ep * i));

                if (this._LastPageRows < ep && this._LastPageRows > 0)
                {
                    //// i is number of loop. The number two represents the first page and last page.
                    this.TotalPageNumbers = i + 2;
                    break;
                }
                else if(this._LastPageRows < 0)
                {
                    this._LastPageRows = this._TotalNumberOfRows - fp;

                    if (this._LastPageRows > 0)
                    {
                        this.TotalPageNumbers = 2;
                        break;
                    }
                    else if (this._LastPageRows == 0)
                    {
                        this.TotalPageNumbers = 1;
                        break;
                    }
                    else if(this._LastPageRows < 0)
                    {
                        this.TotalPageNumbers = 1;
                        break;
                    }
                }
                else if (this._LastPageRows == 0)
                {
                    this._LastPageRows = ep;

                    //// i is number of loop. The number two represents the first page and last page. -1 represents the last page has no rows, so page count is reduced by 1.
                    this.TotalPageNumbers = i + 2 - 1;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the members.
        /// </summary>
        /// <param name="navigationValue">The navigation value.</param>
        /// <param name="navigationMode">The navigation mode.</param>
        /// <returns>The Grid range.</returns>
        private GridRange UpdateMembers(int navigationValue, NavigationMode navigationMode)
        {
            GridRange gridRangeInfo = new GridRange();

            switch (navigationMode)
            {
                case NavigationMode.Previous:
                    //// Setting the grid range.
                    gridRangeInfo = GridRange.Cells(this._GridRangeInfoCells.Left, this._GridRangeInfoCells.Top - navigationValue, this._GridRangeInfoCells.Right, this._GridRangeInfoCells.Bottom - navigationValue);
                    break;
                case NavigationMode.Next:
                    //// Setting the grid range.
                    gridRangeInfo = GridRange.Cells(this._GridRangeInfoCells.Left, this._GridRangeInfoCells.Bottom, this._GridRangeInfoCells.Right, this._GridRangeInfoCells.Bottom + navigationValue);
                    break;
                case NavigationMode.New:
                    {
                        //// Row header length.
                        this._VerticalHeaderLength = this._Olapgrid.InternalGrid.Model.HeaderRows;

                        //// Column header rows count.
                        this._HorizontalHeaderLength = this._Olapgrid.InternalGrid.Model.HeaderColumns;

                        //// Only data columns. Row Headers count.
                        this._OriginalGridColumnCount = this._Olapgrid.InternalGrid.Model.ColumnCount;

                        //// Total number of rows in entire grid. Including headers.
                        this._TotalNumberOfRows = this._Olapgrid.InternalGrid.Model.RowCount;

                        //// First page preferred number of rows.
                        // this._FirstPageExpectedRows = this.PagingInfo.FirstPagePreferredSize;
                        this._FirstPageExpectedRows = SizeToRowConvertor.Convert(this.PagingInfo.FirstPagePreferredSize, this._Olapgrid);

                        //// Desired number of rows.
                        this._DesiredNumberOfRows = SizeToRowConvertor.Convert(this.PagingInfo.ExpectedPageSize, this._Olapgrid);

                        //// Desired rows per page a copy.
                        this._ExpectedRowsPerPage = this._DesiredNumberOfRows;

                        //// New page is always first page.
                        this.IsFirst = true;
                        this.IsLast = false;

                        //// Corollary.
                        this.CurrentPageNumber = 1;

                        //// Setting the grid range.
                        gridRangeInfo = GridRange.Cells(0, 0, this._OriginalGridColumnCount, this._FirstPageExpectedRows);
                        break;
                    }
            }

            //// Total number of columns for the selection range is (Last column in selection range -  first coulumn in selection range)
            this._GridControl.Model.ColumnCount = gridRangeInfo.Right;

            //// Total number of rows for the selection range is (Last row in selection range -  first row in selection range)
            this._GridControl.Model.RowCount = gridRangeInfo.Bottom - gridRangeInfo.Top;

            //// Split and take the desired cells from the olap grid.
            this.SplitRows(gridRangeInfo);

            //// Updating the indeces of each page.
            if (!this._IndexDictionary.Keys.Contains(CurrentPageNumber))
            {
                this._IndexDictionary.Add(CurrentPageNumber, new Indices() { StartIndex = gridRangeInfo.Top, EndIndex = gridRangeInfo.Bottom - 1 });
            }

            return this._GridRangeInfoCells = gridRangeInfo;
        }

        /// <summary>
        /// Splits the rows.
        /// </summary>
        /// <param name="requiredGridRange">The required grid range.</param>
        private void SplitRows(GridRange requiredGridRange)
        {
            //// Top - Top of the selection range. Top selection starting point in selection range.
            int top = requiredGridRange.Top;

            //// Left - Left of the selection range. Left selection starting point in selection range.
            int left = requiredGridRange.Left;

            //// Bottom - Bottom of the selection range. Last row of selection range.
            int bottom = requiredGridRange.Bottom;

            //// Right - Right end of the selection range. Last column of selection range.
            int right = requiredGridRange.Right;

            ////// Setting column header style.
            //this.gc.Model. = this.grid.InternalGrid.Model.HeaderStyle;

            ////// Setting row header style.
            //this.gc.Model.HeaderRows = this.grid.InternalGrid.Model.HeaderRows;
            //// K = 5 is desired size.
            int i = 0, k = 0;
            for (i = top, k = 0; i < bottom && k < (bottom - top); i++, k++)
            {
                for (int j = left; j <= right; j++)
                {
                    this._GridControl.Model[k, j] = this._Olapgrid.InternalGrid.Model[i, j];
                }
            }
        }

        /// <summary>
        /// Moves the next.
        /// </summary>
        public void MoveNext()
        {
            //// Splitting the table as per the calculation [Top, Left, Bottom, Right] 5 is the next page size
            int nextPageRows = this._TotalNumberOfRows - this._GridRangeInfoCells.Bottom;

            if (nextPageRows > -1 && nextPageRows >= this._DesiredNumberOfRows)
            {
                this.IsLast = false;
                this.IsFirst = true;
                this.CurrentPageNumber++;

                //// If the number of rows in the next page is greater then or equal to desired number of rows. Navigating to next is possible.                
                this.UpdateMembers(_DesiredNumberOfRows, NavigationMode.Next);
            }
            else if (nextPageRows > 0)
            {
                this.IsLast = true;
                this.IsFirst = false;
                this._LastPageRows = nextPageRows;
                this.CurrentPageNumber++;

                //// If the number of rows is less then desired number of rows. Last page is alone possible.
                this.UpdateMembers(nextPageRows, NavigationMode.Next);
            }
        }

        /// <summary>
        /// Moves the previous.
        /// </summary>
        public void MovePrevious()
        {
            int previousPageRows = 0;
            if (this.IsLast)
            {
                previousPageRows = this._GridRangeInfoCells.Bottom - this._LastPageRows;
                this._DesiredNumberOfRows = this._LastPageRows;
                this._GridRangeInfoCells.Top -= (this._ExpectedRowsPerPage - this._LastPageRows);
            }
            else
            {
                previousPageRows = this._GridRangeInfoCells.Bottom - this._DesiredNumberOfRows;
                this._DesiredNumberOfRows = this._ExpectedRowsPerPage;
            }

            if (previousPageRows > this._ExpectedRowsPerPage)
            {
                //// Splitting the table as per the calculation [Top, Left, Bottom, Right] 5 is the next page size                
                this.UpdateMembers(_DesiredNumberOfRows, NavigationMode.Previous);
                this.IsFirst = false;
                this.IsLast = false;
                this.CurrentPageNumber--;
            }
            else if (previousPageRows > 0)
            {
                //// Splitting the table as per the calculation [Top, Left, Bottom, Right] 5 is the next page size                
                this.UpdateMembers(0, NavigationMode.New);
                this.IsFirst = true;
                this.IsLast = false;
            }
        }

        /// <summary>
        /// Moves to specific page.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <remarks> 
        ///     1. Given page number should be less then total number of pages and greater then 0.
        ///     2. If, Pagenumber is less then current page number, call move previous till the page.
        ///     3. If, Pagenumber is greater then current page number, call move next till the page.
        ///     4. If, Pagenumber is equal to current page number. Do nothing.
        /// </remarks>
        public void MoveTo(int pageNumber)
        {
            if (pageNumber <= this.TotalPageNumbers && pageNumber > 0)
            {                
                if (pageNumber < this.CurrentPageNumber)
                {
                    int count = this.CurrentPageNumber;
                    while(pageNumber != count)
                    {
                        this.MovePrevious();
                        count--;
                    }                    
                }
                else if(pageNumber > this.CurrentPageNumber)
                {
                    int count = this.CurrentPageNumber;
                    while(pageNumber != count)
                    {
                        this.MoveNext();
                        count++;
                    }
                }
            }
        }

        #endregion
    }

    #region Navigation Mode

    /// <summary>
    /// Specifies the type of navigation taking place.
    /// </summary>
    public enum NavigationMode
    {
        /// <summary>
        /// Navigates to the previous possible page of the paging grid. Using the desired size attribute..
        /// </summary>
        Previous,

        /// <summary>
        /// Navigates to the next possible page of the paging grid. Using the desired size attribute.
        /// </summary>
        Next,

        /// <summary>
        /// Navigates to the new page. Creates the new using the first page preferred size attribute.
        /// </summary>
        New
    }

    #endregion

    #region GridRange

    /// <summary>
    /// Specifies the grid ranges.
    /// </summary>
    public struct GridRange
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public int Top { get; set; }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        public int Right { get; set; }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public int Bottom { get; set; }

        #endregion

        #region Static Method

        /// <summary>
        /// Cellses the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <returns>The Grid range.</returns>
        public static GridRange Cells(int left, int top, int right, int bottom)
        {
            return new GridRange() { Left = left, Top = top, Right = right, Bottom = bottom };
        }

        #endregion
    }

    #endregion

    #region PagingInfo

    /// <summary>
    /// Specifies the paging info.
    /// </summary>
    public struct PagingInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the first size of the page preferred.
        /// </summary>
        /// <value>The first size of the page preferred.</value>
        public double FirstPagePreferredSize { get; set; }

        /// <summary>
        /// Gets or sets the expected size of the page.
        /// </summary>
        /// <value>The expected size of the page.</value>
        public double ExpectedPageSize { get; set; }

        #endregion
    }

    #endregion

    #region Size to row convertor

    /// <summary>
    /// Converts/Maps the size to rows. Converts, pixels to row and inches to rows.
    /// </summary>
    public struct SizeToRowConvertor
    {
        #region Helper Methods

        /// <summary>
        /// Converts the specified size in pixel.
        /// </summary>
        /// <param name="sizeInPixel">The size in pixel.</param>
        /// <returns>The Rows.</returns>
        public static int Convert(double sizeInPixel, Syncfusion.Windows.Grid.Olap.OlapGrid olapGrid)
        {
            int rows = 0;
            if (olapGrid != null)
            {
                if (olapGrid.InternalGrid != null)
                {
                    if (olapGrid.InternalGrid.Model != null)
                    {
                        //// Here, we are considering the initial height of each row as 1px;
                        //double[] rowHeights = new double[(int)sizeInPixel];
                        List<double> rowHeights = new List<double>();
                        int olapGridInternalGridModelRowCount = olapGrid.InternalGrid.Model.RowCount;
                        
                        if (new Comparer(olapGridInternalGridModelRowCount) > new Comparer(sizeInPixel))
                        {
                            for (int i = 0; new Comparer(i) < new Comparer(sizeInPixel); i++)
                            {
                                //// Storing the height of the rows. When, number of rows lesser then number of pixels
                                //rowHeights[i] = olapGrid.InternalGrid.Model.RowHeights[i];
                                rowHeights.Add(olapGrid.InternalGrid.Model.RowHeights[i]);
                            }
                        }
                        else if (olapGridInternalGridModelRowCount > 0)
                        {
                            for (int i = 0; i < olapGridInternalGridModelRowCount; i++)
                            {
                                //rowHeights[i] = olapGrid.InternalGrid.Model.RowHeights[i];
                                rowHeights.Add(olapGrid.InternalGrid.Model.RowHeights[i]);
                            }
                        }

                        double d = 0;
                        for(int i = 1; i <= rowHeights.Count ; i++)
                        {
                            d += rowHeights[i - 1];
                            if (new Comparer(d) <= new Comparer(sizeInPixel))
                            {
                                //// Checking whether the height reaches the row count.
                                //d += rowHeights[i - 1];
                                rows = i;
                            }
                            else
                            {
                                //// Here is the number of rows.
                                rows = i - 1;
                                break;
                            }
                        }
                    }
                }
            }

            return rows;
        }

        /// <summary>
        /// Converts from invariant string.
        /// </summary>
        /// <param name="sizeInPixel">The size in pixel. eg., 200</param>
        /// <returns>The Rows.</returns>
        public static int ConvertFromInvariantString(string sizeInPixel, Syncfusion.Windows.Grid.Olap.OlapGrid olapGrid)
        {            
            return Convert(double.Parse(sizeInPixel), olapGrid);
        }

        /// <summary>
        /// Converts from invariant inch string.
        /// </summary>
        /// <param name="sizeInInch">The size in inch. eg., 2</param>
        /// <returns></returns>
        public static int ConvertFromInvariantInchString(string sizeInInch, Syncfusion.Windows.Grid.Olap.OlapGrid olapGrid)
        {
            return Convert(double.Parse(sizeInInch) * 96, olapGrid);
        }
        
        #endregion
    }

    #endregion

    #region Comparer

    /// <summary>
    /// A comparer, compares int or double value for any of the comparison operators.
    /// </summary>
    public class Comparer
    {
        #region Members

        private double value = 0.0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer"/> class.
        /// </summary>
        public Comparer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer"/> class.
        /// </summary>
        /// <param name="intValue">The int value.</param>
        public Comparer(int intValue)
        {
            this.value = (double)intValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer"/> class.
        /// </summary>
        /// <param name="doubleValue">The double value.</param>
        public Comparer(double doubleValue)
        {
            this.value = doubleValue;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="intValue">The int value.</param>
        /// <param name="doubleValue">The double value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Comparer intValue, Comparer doubleValue)
        {
            return intValue.value > doubleValue.value;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="intValue">The int value.</param>
        /// <param name="doubleValue">The double value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Comparer intValue, Comparer doubleValue)
        {
            return intValue.value < doubleValue.value;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="intValue">The int value.</param>
        /// <param name="doubleValue">The double value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(Comparer intValue, Comparer doubleValue)
        {
            return intValue.value <= doubleValue.value;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="intValue">The int value.</param>
        /// <param name="doubleValue">The double value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(Comparer intValue, Comparer doubleValue)
        {
            return intValue.value >= doubleValue.value;
        }

        #endregion
    }

    #endregion

    #region Page Indices

    /// <summary>
    /// Contains the start and end index.
    /// </summary>
    public struct Indices
    {
        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        /// <value>The start index.</value>
        public int StartIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end index.
        /// </summary>
        /// <value>The end index.</value>
        public int EndIndex
        {
            get;
            set;
        }
    }

    #endregion
}
