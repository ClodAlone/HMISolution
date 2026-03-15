#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Collections;
namespace Syncfusion.XlsIO.Drawing
{
    internal class ClientAnchor
    {
        #region members
        /// <summary>
        /// Represents the size information of the shape.
        /// </summary>
        private SizeProperties m_size;
        /// <summary>
        /// Represents the Current worksheet instance.
        /// </summary>
        private WorksheetImpl m_workSheet;
        #endregion

        #region Instantiate
        /// <summary>
        /// Intializes the members.
        /// </summary>
        /// <param name="worksheetImpl">Current Worksheet object.</param>
        internal ClientAnchor(WorksheetImpl worksheetImpl)
        {
            this.m_workSheet = worksheetImpl;
            m_size = new SizeProperties();
        }
        #endregion

        #region Properties

        internal WorksheetImpl Worksheet
        {
            get
            {
                return m_workSheet;
            }
            set
            {
                m_workSheet = value;
            }
        }

        internal PlacementType Placement
        {
            get
            {
                return this.m_size.GetPlacementType();
            }
            set
            {
                int width = 0;
                int height = 0;
                int topRow = 0;
                int top = 0;
                int leftColumn = 0;
                int left = 0;
                int bottomRow = 0;
                int bottom = 0;
                int rightColumn = 0;
                int right = 0;
                if (this.m_size.GetPlacementType() == PlacementType.MoveAndSize)
                {
                    topRow = this.m_size.GetTopRow();
                    top = this.m_size.Top;
                    leftColumn = this.m_size.GetLeftColumn();
                    left = this.m_size.Left;
                    bottomRow = this.m_size.GetBottomRow();
                    bottom = this.m_size.Bottom;
                    rightColumn = this.m_size.GetRightColumn();
                    right = this.m_size.Right;
                    if (value == PlacementType.FreeFloating)
                    {
                        width = this.CalculateWidth(0, 0, leftColumn, left);
                        height = this.CalculateHeight(0, 0, topRow, top);
                        this.m_size.Top = height;
                        this.m_size.Left = width;
                    }
                    width = this.CalculateWidth(leftColumn, left, rightColumn, right);
                    height = this.CalculateHeight(topRow, top, bottomRow, bottom);
                    this.m_size.Right = width;
                    this.m_size.Bottom = height;
                    this.m_size.SetPlacementType(value);
                }
                else if (this.m_size.GetPlacementType() == PlacementType.Move)
                {
                    topRow = this.m_size.GetTopRow();
                    top = this.m_size.Top;
                    leftColumn = this.m_size.GetLeftColumn();
                    left = this.m_size.Left;
                    if (value == PlacementType.MoveAndSize)
                    {
                        width = this.m_size.Right;
                        height = this.m_size.Bottom;
                        int[] numArray = this.GetLeftAndLeftColumnOffset(leftColumn, left, width);
                        this.m_size.SetRightColumn(numArray[0]);
                        this.m_size.Right = numArray[1];
                        numArray = this.GetTopAndTopRowOffset(topRow, top, height);
                        this.m_size.SetBottomRow(numArray[0]);
                        this.m_size.Bottom = numArray[1];
                    }
                    else if (value == PlacementType.FreeFloating)
                    {
                        width = this.CalculateWidth(0, 0, leftColumn, left);
                        this.m_size.Left = width;
                        height = this.CalculateHeight(0, 0, topRow, top);
                        this.m_size.Top = height;
                    }
                    this.m_size.SetPlacementType(value);
                }
                else if (this.m_size.GetPlacementType() == PlacementType.FreeFloating)
                {
                    width = this.m_size.Left;
                    height = this.m_size.Top;
                    int[] numArray2 = this.GetTopAndTopRowOffset(0, 0, height);
                    this.m_size.SetTopRow(numArray2[0]);
                    this.m_size.Top = numArray2[1];
                    topRow = numArray2[0];
                    top = numArray2[1];
                    numArray2 = this.GetLeftAndLeftColumnOffset(0, 0, width);
                    this.m_size.SetLeftColumn(numArray2[0]);
                    this.m_size.Left = numArray2[1];
                    leftColumn = numArray2[0];
                    left = numArray2[1];
                    if (value == PlacementType.MoveAndSize)
                    {
                        width = this.m_size.Right;
                        height = this.m_size.Bottom;
                        numArray2 = this.GetTopAndTopRowOffset(topRow, top, height);
                        this.m_size.SetBottomRow(numArray2[0]);
                        this.m_size.Bottom = numArray2[1];
                        numArray2 = this.GetLeftAndLeftColumnOffset(leftColumn, left, width);
                        this.m_size.SetRightColumn(numArray2[0]);
                        this.m_size.Right = numArray2[1];
                    }
                    this.m_size.SetPlacementType(value);
                }

            }

        }

        internal int Height
        {
            get
            {

                if ((this.Placement != PlacementType.Move) && (this.Placement != PlacementType.FreeFloating))
                {
                    int topRow = this.m_size.GetTopRow();
                    int bottomRow = this.m_size.GetBottomRow();
                    int top = this.m_size.Top;
                    int bottom = this.m_size.Bottom;
                    return this.CalculateHeight(topRow, top, bottomRow, bottom);
                }
                return this.m_size.Bottom;
            }
            set
            {
                if ((this.Placement != PlacementType.Move) && (this.Placement != PlacementType.FreeFloating))
                {
                    int topRow = this.m_size.GetTopRow();
                    int top = this.m_size.Top;
                    int[] numArray = this.GetTopAndTopRowOffset(topRow, top, value);
                    this.m_size.SetBottomRow(numArray[0]);
                    this.m_size.Bottom = numArray[1];
                }
                else
                {
                    this.m_size.Bottom = value;
                }
            }
        }

        internal int Width
        {
            get
            {
                if ((this.Placement != PlacementType.Move) && (this.Placement != PlacementType.FreeFloating))
                {
                    int leftColumn = this.m_size.GetLeftColumn();
                    int rightColumn = this.m_size.GetRightColumn();
                    int left = this.m_size.Left;
                    int right = this.m_size.Right;
                    return this.CalculateWidth(leftColumn, left, rightColumn, right);
                }
                return this.m_size.Right;
            }
            set
            {

                if ((this.Placement != PlacementType.Move) && (this.Placement != PlacementType.FreeFloating))
                {
                    int leftColumn = this.m_size.GetLeftColumn();
                    int left = this.m_size.Left;
                    int[] numArray = this.GetLeftAndLeftColumnOffset(leftColumn, left, value);
                    this.m_size.SetRightColumn(numArray[0]);
                    this.m_size.Right = numArray[1];
                }
                else
                {
                    this.m_size.Right = value;
                }
            }

        }

        internal int Top
        {
            get
            {
                return (int)((((float)(this.m_workSheet.GetInnerRowHeightInPixels(this.TopRow) * this.TopRowOffset)) / SizeProperties.FULL_ROW_OFFSET) + 0.5);
            }
            set
            {

                int rowHeightPixel = this.m_workSheet.GetInnerRowHeightInPixels(this.TopRow);
                if (rowHeightPixel < value)
                {
                    throw new ArgumentException("The top value must be less than the upper left row height.");
                }
                this.TopRowOffset = (int)(((value * SizeProperties.FULL_ROW_OFFSET) / ((float)rowHeightPixel)) + 0.5);

            }

        }

        internal int Left
        {
            get
            {

                return (int)((((float)(this.m_workSheet.GetViewColumnWidthPixel(this.LeftColumn) * this.LeftColumnOffset)) / SizeProperties.FULL_COLUMN_OFFSET) + 0.5);
            }
            set
            {


                int viewColumnWidthPixel = this.m_workSheet.GetViewColumnWidthPixel(this.LeftColumn);
                if (viewColumnWidthPixel < value)
                {
                    throw new ArgumentException("The left value must be less than the upper left column width.");
                }
                this.LeftColumnOffset = (int)(((value * SizeProperties.FULL_COLUMN_OFFSET) / ((float)viewColumnWidthPixel)) + 0.5);

            }
        }

        internal int LeftColumn
        {
            get
            {
                if ((this.Placement != PlacementType.Move) && (this.Placement != PlacementType.MoveAndSize))
                {
                    int left = this.m_size.Left;
                    return this.GetLeftAndLeftColumnOffset(0, 0, left)[0];
                }
                return this.m_size.GetLeftColumn();
            }
            set
            {
                WorksheetImpl.CheckColumnIndex(value);

                PlacementType placement = this.Placement;
                this.Placement = PlacementType.Move;
                this.m_size.SetLeftColumn(value);
                this.Placement = placement;

            }
        }

        internal int TopRow
        {
            get
            {
                if ((this.Placement != PlacementType.Move) && (this.Placement != PlacementType.MoveAndSize))
                {
                    int top = this.m_size.Top;
                    return this.GetTopAndTopRowOffset(0, 0, top)[0];
                }
                return this.m_size.GetTopRow();
            }
            set
            {
                WorksheetImpl.CheckRowIndex(value);

                PlacementType placement = this.Placement;
                this.Placement = PlacementType.Move;
                this.m_size.SetTopRow(value);
                this.Placement = placement;

            }
        }

        internal int TopRowOffset
        {
            get
            {

                if ((this.Placement != PlacementType.Move) && (this.Placement != PlacementType.MoveAndSize))
                {
                    int top = this.m_size.Top;
                    return this.GetTopAndTopRowOffset(0, 0, top)[1];
                }
                if (this.m_size.Top <= SizeProperties.FULL_ROW_OFFSET)
                {
                    return this.m_size.Top;
                }
                return (int)SizeProperties.FULL_ROW_OFFSET;
            }
            set
            {
                if ((value >= 0) && (value <= SizeProperties.FULL_ROW_OFFSET))
                {
                    PlacementType placement = this.Placement;
                    this.Placement = PlacementType.Move;
                    this.m_size.Top = value;
                    this.Placement = placement;
                }
            }
        }

        internal int LeftColumnOffset
        {
            get
            {
                if ((this.Placement != PlacementType.Move) && (this.Placement != PlacementType.MoveAndSize))
                {
                    int left = this.m_size.Left;
                    return this.GetLeftAndLeftColumnOffset(0, 0, left)[1];
                }
                if (this.m_size.Left <= SizeProperties.FULL_COLUMN_OFFSET)
                {
                    return this.m_size.Left;
                }
                return (int)SizeProperties.FULL_COLUMN_OFFSET;
            }
            set
            {
                if ((value >= 0) && (value <= SizeProperties.FULL_COLUMN_OFFSET))
                {
                    PlacementType placement = this.Placement;
                    this.Placement = PlacementType.Move;
                    this.m_size.Left = value;
                    this.Placement = placement;
                }
            }
        }

        internal int RightColumnOffset
        {
            get
            {

                if (this.Placement == PlacementType.MoveAndSize)
                {
                    return this.m_size.Right;
                }
                int leftColumn = 0;
                int left = 0;
                int right = this.m_size.Right;
                if (this.Placement == PlacementType.Move)
                {
                    leftColumn = this.m_size.GetLeftColumn();
                    left = this.m_size.Left;
                }
                else
                {
                    right += this.m_size.Left;
                }
                return this.GetLeftAndLeftColumnOffset(leftColumn, left, right)[1];
            }
            set
            {
                if ((value >= 0) || (value <= SizeProperties.FULL_COLUMN_OFFSET))
                {
                    int rightColumn = this.RightColumn;
                    PlacementType placement = this.Placement;
                    this.Placement = PlacementType.Move;
                    int right = this.m_size.Right;
                    int[] numArray = this.GetLeftAndLeftOffset(rightColumn, value, right);
                    this.m_size.SetLeftColumn(numArray[0]);
                    this.m_size.Left = numArray[1];
                    this.Placement = placement;
                }
            }
        }

        internal int BottomRowOffset
        {
            get
            {
                if (this.Placement == PlacementType.MoveAndSize)
                {
                    return this.m_size.Bottom;
                }
                int bottom = this.m_size.Bottom;
                int topRow = 0;
                int top = 0;
                if (this.Placement == PlacementType.Move)
                {
                    topRow = this.m_size.GetTopRow();
                    top = this.m_size.Top;
                }
                else
                {
                    bottom += this.m_size.Top;
                }
                return this.GetTopAndTopRowOffset(topRow, top, bottom)[1];
            }
            set
            {
                if ((value >= 0) && (value <= SizeProperties.FULL_ROW_OFFSET))
                {
                    int bottomRow = this.BottomRow;
                    PlacementType placement = this.Placement;
                    this.Placement = PlacementType.Move;
                    int bottom = this.m_size.Bottom;
                    int[] numArray = this.GetTopAndTopOffset(bottomRow, value, bottom);
                    this.m_size.SetTopRow(numArray[0]);
                    this.m_size.Top = numArray[1];
                    this.Placement = placement;
                }
            }
        }

        internal int RightColumn
        {
            get
            {

                if (this.Placement == PlacementType.MoveAndSize)
                {
                    return this.m_size.GetRightColumn();
                }
                int num2 = 0;
                int left = 0;
                int right = this.m_size.Right;
                if (this.Placement == PlacementType.Move)
                {
                    num2 = this.m_size.GetLeftColumn();
                    left = this.m_size.Left;
                }
                else
                {
                    right += this.m_size.Left;
                }
                return this.GetLeftAndLeftColumnOffset(num2, left, right)[0];
            }
            set
            {
                WorksheetImpl.CheckColumnIndex(value);

                int rightColumnOffset = this.RightColumnOffset;
                PlacementType placement = this.Placement;
                this.Placement = PlacementType.Move;
                int right = this.m_size.Right;
                int[] numArray = this.GetLeftAndLeftOffset(value, rightColumnOffset, right);
                this.m_size.SetLeftColumn(numArray[0]);
                this.m_size.Left = numArray[1];
                this.Placement = placement;

            }
        }

        internal int BottomRow
        {
            get
            {

                if (this.Placement == PlacementType.MoveAndSize)
                {
                    return this.m_size.GetBottomRow();
                }
                int bottom = this.m_size.Bottom;
                int num3 = 0;
                int top = 0;
                if (this.Placement == PlacementType.Move)
                {
                    num3 = this.m_size.GetTopRow();
                    top = this.m_size.Top;
                }
                else
                {
                    bottom += this.m_size.Top;
                }
                return this.GetTopAndTopRowOffset(num3, top, bottom)[0];
            }
            set
            {
                WorksheetImpl.CheckRowIndex(value);

                int bottomRowOffset = this.BottomRowOffset;
                PlacementType placement = this.Placement;
                this.Placement = PlacementType.Move;
                int bottom = this.m_size.Bottom;
                int[] numArray = this.GetTopAndTopOffset(value, bottomRowOffset, bottom);
                this.m_size.SetTopRow(numArray[0]);
                this.m_size.Top = numArray[1];
                this.Placement = placement;

            }
        }

        #endregion

        #region Methods.

        internal int[] GetTopAndTopRowOffset(int topRow, int top, int bottom)
        {
            int[] numArray = new int[2];
            if (bottom == 0)
            {
                numArray[0] = topRow;
                numArray[1] = top;
                return numArray;
            }
            int rowHeightPixel = 0;
            WorksheetImpl cells = this.m_workSheet;
            RecordTable recordTable = cells.CellRecords.Table;
            int rowCount = recordTable.Rows.GetCount();
            if (top != 0)
            {
                rowHeightPixel = cells.GetInnerRowHeightInPixels(topRow);
                int num2 = (int)((rowHeightPixel - (((float)(rowHeightPixel * top)) / SizeProperties.FULL_ROW_OFFSET)) + 0.5);
                if (bottom <= num2)
                {
                    numArray[0] = topRow;
                    numArray[1] = (int)((((bottom * SizeProperties.FULL_ROW_OFFSET) / ((float)rowHeightPixel)) + top) + 0.5);
                    return numArray;
                }
                topRow++;
                bottom -= num2;
            }
            int num3 = (int)(((cells.StandardHeight * cells.GetAppImpl().GetdpiY()) / 72.0) + 0.5);
            int arrIndex = 0;
            if (rowCount == 0)
            {
                int num7 = (int)Math.Ceiling((double)(((double)bottom) / ((double)num3)));
                topRow += num7 - 1;
                bottom -= num3 * num7;
                rowHeightPixel = num3;
            }
            else
            {
                recordTable.Rows.GetRowIndex(topRow, out arrIndex);
                if (arrIndex >= rowCount)
                {
                    int num5 = (int)Math.Ceiling((double)(((double)bottom) / ((double)num3)));
                    topRow += num5 - 1;
                    bottom -= num3 * num5;
                    rowHeightPixel = num3;
                }
                else
                {
                    RowStorage rowByIndex = recordTable.Rows[arrIndex];
                    while (topRow <= 0xfffff)
                    {
                        if (arrIndex == topRow)
                        {
                            rowHeightPixel = (int)(((cells.GetInnerRowHeight(arrIndex) * cells.GetAppImpl().GetdpiY()) / 72.0) + 0.5);
                            bottom -= rowHeightPixel;
                            if (bottom <= 0)
                            {
                                break;
                            }
                            arrIndex++;
                            if (arrIndex >= rowCount)
                            {
                                int num6 = (int)Math.Ceiling((double)(((double)bottom) / ((double)num3)));
                                topRow += num6;
                                bottom -= num3 * num6;
                                rowHeightPixel = num3;
                                break;
                            }
                            rowByIndex = recordTable.Rows[arrIndex];
                        }
                        else
                        {
                            rowHeightPixel = num3;
                            bottom -= rowHeightPixel;
                            if (bottom <= 0)
                            {
                                break;
                            }
                        }
                        topRow++;
                    }
                }
            }
            if ((bottom <= 0) && ((bottom != 0) || (topRow != 0xfffff)))
            {
                if (bottom == 0)
                {
                    numArray[0] = topRow + 1;
                    return numArray;
                }
                numArray[0] = topRow;
                numArray[1] = (int)((((bottom + rowHeightPixel) * SizeProperties.FULL_ROW_OFFSET) / ((float)rowHeightPixel)) + 0.5);
                return numArray;
            }
            numArray[0] = 0xfffff;
            numArray[1] = (int)SizeProperties.FULL_ROW_OFFSET;
            return numArray;
        }

        internal int CalculateHeight(int topRow, int top, int bottomRow, int bottom)
        {
            int num = 0;
            int rowHeightPixel = 0;
            WorksheetImpl cells = this.m_workSheet as WorksheetImpl;
            if (top >= SizeProperties.FULL_ROW_OFFSET)
            {
                top = (int)SizeProperties.FULL_ROW_OFFSET;
            }
            if (bottom >= SizeProperties.FULL_ROW_OFFSET)
            {
                bottom = (int)SizeProperties.FULL_ROW_OFFSET;
            }
            if (bottomRow == topRow)
            {
                rowHeightPixel = cells.GetInnerRowHeightInPixels(topRow);
                return (int)((((float)((bottom - top) * rowHeightPixel)) / SizeProperties.FULL_ROW_OFFSET) + 0.5);
            }
            if (bottomRow < topRow)
            {
                return 0;
            }
            rowHeightPixel = cells.GetInnerRowHeightInPixels(topRow);
            num += rowHeightPixel - ((int)((((float)(top * rowHeightPixel)) / SizeProperties.FULL_ROW_OFFSET) + 0.5));
            int num3 = topRow;
            topRow++;
            int arrIndex = 0;
            int num5 = 0;

            //cells.Rows.method_15(int_0, out arrIndex);
            RecordTable recordTable = cells.CellRecords.Table;

            int rowCount = recordTable.Rows.GetCount();
            while (arrIndex < rowCount)
            {
                RowStorage rowByIndex = recordTable.Rows[arrIndex];
                if (rowByIndex != null)
                {
                    if (arrIndex >= topRow)
                    {
                        if (arrIndex >= bottomRow)
                        {
                            break;
                        }
                        num5++;
                        num += (int)(((cells.GetInnerRowHeight(arrIndex) * cells.GetAppImpl().GetdpiY()) / 72.0) + 0.5);
                    }
                }
                arrIndex++;
            }
            int num6 = ((bottomRow - num3) - 1) - num5;
            if ((num6 > 0))// && !cells.method_25())
            {
                num += num6 * ((int)(((cells.StandardHeight * cells.GetAppImpl().GetdpiY()) / 72.0) + 0.5));
            }
            rowHeightPixel = cells.GetInnerRowHeightInPixels(bottomRow);
            return (num + ((int)((((float)(bottom * rowHeightPixel)) / SizeProperties.FULL_ROW_OFFSET) + 0.5)));
        }

        internal int CalculateRowOffset(int minRow, int minOffsetValue, int maxRow, int maxOffsetValue)
        {
            int num = 0;
            int num2 = 0;
            WorksheetImpl cells = this.m_workSheet as WorksheetImpl;
            if (maxRow == minRow)
            {
                num2 = cells.GetInnerRowHeightInPixels(minRow);
                return (int)((((float)((maxOffsetValue - minOffsetValue) * num2)) / SizeProperties.FULL_ROW_OFFSET) + 0.5);
            }
            num2 = cells.GetInnerRowHeightInPixels(minRow);
            num += num2 - ((int)((((float)(minOffsetValue * num2)) / SizeProperties.FULL_ROW_OFFSET) + 0.5));
            int num3 = minRow;
            minRow++;
            int arrIndex = 0;
            int num5 = 0;
            RecordTable recordTable = cells.CellRecords.Table;

            int rowCount = recordTable.Rows.GetCount();
            while (arrIndex < rowCount)
            {
                RowStorage rowByIndex = recordTable.Rows[arrIndex];
                if (rowByIndex != null)
                {
                    if (arrIndex >= minRow)
                    {
                        if (arrIndex >= maxRow)
                        {
                            break;
                        }
                        num5++;
                        num += (int)((cells.StandardHeight * cells.GetAppImpl().GetdpiY()) / 0x5a0) + ((int)0.5);
                    }
                }
                arrIndex++;
            }
            int num6 = ((maxRow - num3) - 1) - num5;
            if (num6 > 0)
            {
                num += num6 * ((int)(((cells.StandardHeight * cells.GetAppImpl().GetdpiY()) / 72.0) + 0.5));
            }
            num2 = cells.GetInnerRowHeightInPixels(maxRow);
            return (num + ((int)((((float)(maxOffsetValue * num2)) / SizeProperties.FULL_ROW_OFFSET) + 0.5)));
        }

        internal int CalculateColumnOffset(int minColumn, int minOffsetValue, int maxColumn, int maxOffsetValue)
        {
            int num4;
            int num = 0;
            int num2 = 0;
            WorksheetImpl cells = this.m_workSheet as WorksheetImpl;
            if (maxColumn == minColumn)
            {
                num2 = cells.GetColumnWidthInPixels(minColumn + 1);
                return (int)((((float)((maxOffsetValue - minOffsetValue) * num2)) / SizeProperties.FULL_COLUMN_OFFSET) + 0.5);
            }
            num2 = cells.GetColumnWidthInPixels(minColumn + 1);
            num += num2 - ((int)((((float)(minOffsetValue * num2)) / SizeProperties.FULL_COLUMN_OFFSET) + 0.5));
            int num3 = minColumn;
            minColumn++;
            int num5 = 0;
            cells.Columnss.GetColumnIndex(minColumn, out num4);
            while (num4 < cells.Columnss.Count)
            {
                Column columnByIndex = cells.Columnss.GetColumnByIndex(num4);
                if (columnByIndex.Index >= minColumn)
                {
                    if (columnByIndex.Index >= maxColumn)
                    {
                        break;
                    }
                    num5++;
                    if (!columnByIndex.IsHidden)
                    {
                        num += WorksheetImpl.CharacterWidth(columnByIndex.Width, cells.GetAppImpl());
                    }
                }
                num4++;
            }
            num += cells.Columnss.GetWidth((num3 + num5) + 1, maxColumn - 1, true, true);
            num2 = cells.GetColumnWidthInPixels(maxColumn + 1);
            return (num + ((int)((((float)(maxOffsetValue * num2)) / SizeProperties.FULL_COLUMN_OFFSET) + 0.5)));
        }

        internal int[] GetLeftAndLeftColumnOffset(int leftColumn, int left, int right)
        {
            int[] numArray = new int[2];
            int viewColumnWidthPixel = 0;
            WorksheetImpl cells = this.m_workSheet as WorksheetImpl;
            if (left != 0)
            {
                viewColumnWidthPixel = cells.GetViewColumnWidthPixel(leftColumn);
                int num2 = (int)((viewColumnWidthPixel - (((float)(viewColumnWidthPixel * left)) / SizeProperties.FULL_COLUMN_OFFSET)) + 0.5);
                if (right <= num2)
                {
                    numArray[0] = leftColumn;
                    numArray[1] = (int)((((right * SizeProperties.FULL_COLUMN_OFFSET) / ((float)viewColumnWidthPixel)) + left) + 0.5);
                    return numArray;
                }
                leftColumn++;
                right -= num2;
            }
            int arrIndex = 0;
            cells.Columnss.GetColumnIndex(leftColumn, out arrIndex);
            while (true)
            {
                double num4 = 0.0;
                if (arrIndex >= cells.Columnss.Count)
                {
                    num4 = cells.Columnss.GetWidth(leftColumn, false);
                }
                else
                {
                    Column columnByIndex = cells.Columnss.GetColumnByIndex(arrIndex);
                    if (columnByIndex.Index == leftColumn)
                    {
                        arrIndex++;
                        num4 = columnByIndex.IsHidden ? 0.0 : columnByIndex.defaultWidth;
                    }
                    else
                    {
                        num4 = cells.Columnss.GetWidth(leftColumn, false);
                    }
                }
                viewColumnWidthPixel = WorksheetImpl.CharacterWidth(num4, cells.GetAppImpl());
                right -= viewColumnWidthPixel;
                if (right <= 0)
                {
                    if ((right <= 0) && ((right != 0) || (leftColumn != 0x3fff)))
                    {
                        if (right == 0)
                        {
                            numArray[0] = leftColumn + 1;
                            return numArray;
                        }
                        numArray[0] = leftColumn;
                        numArray[1] = (int)((((right + viewColumnWidthPixel) * SizeProperties.FULL_COLUMN_OFFSET) / ((float)viewColumnWidthPixel)) + 0.5);
                        return numArray;
                    }
                    numArray[0] = 0x3fff;
                    numArray[1] = (int)SizeProperties.FULL_COLUMN_OFFSET;
                    return numArray;
                }
                leftColumn++;
                if (leftColumn > 0x3fff)
                {
                    numArray[0] = 0x3fff;
                    numArray[1] = (int)SizeProperties.FULL_COLUMN_OFFSET;
                    return numArray;
                }
            }
        }

        internal int CalculateWidth(int leftColumn, int left, int rightColumn, int right)
        {
            int num4;
            int num = 0;
            int viewColumnWidthPixel = 0;
            WorksheetImpl cells = this.m_workSheet as WorksheetImpl;
            if (rightColumn == leftColumn)
            {
                viewColumnWidthPixel = cells.GetViewColumnWidthPixel(leftColumn);
                return (int)((((float)((right - left) * viewColumnWidthPixel)) / SizeProperties.FULL_COLUMN_OFFSET) + 0.5);
            }
            if (rightColumn < leftColumn)
            {
                return 0;
            }
            viewColumnWidthPixel = cells.GetViewColumnWidthPixel(leftColumn);
            num += viewColumnWidthPixel - ((int)((((float)(left * viewColumnWidthPixel)) / SizeProperties.FULL_COLUMN_OFFSET) + 0.5));
            int num3 = leftColumn;
            leftColumn++;
            int num5 = 0;
            cells.Columnss.GetColumnIndex(leftColumn, out num4);
            while (num4 < cells.Columnss.Count)
            {
                Column columnByIndex = cells.Columnss.GetColumnByIndex(num4);
                if (columnByIndex.Index >= leftColumn)
                {
                    if (columnByIndex.Index >= rightColumn)
                    {
                        break;
                    }
                    num5++;
                    if (!columnByIndex.IsHidden)
                    {
                        num += WorksheetImpl.CharacterWidth(columnByIndex.Width, cells.GetAppImpl());
                    }
                }
                num4++;
            }
            num += cells.Columnss.GetWidth((num3 + num5) + 1, rightColumn - 1, false, true);
            viewColumnWidthPixel = cells.GetViewColumnWidthPixel(rightColumn);
            return (num + ((int)((((float)(right * viewColumnWidthPixel)) / SizeProperties.FULL_COLUMN_OFFSET) + 0.5)));
        }

        internal int[] GetTopAndTopOffset(int bottomRow, int bottomRowOffset, int bottom)
        {
            int[] numArray = new int[2];
            int rowHeightPixel = 0;
            WorksheetImpl cells = this.m_workSheet;
            if (bottomRowOffset != 0)
            {
                rowHeightPixel = cells.GetInnerRowHeightInPixels(bottomRow);
                int num2 = (int)((((float)(rowHeightPixel * bottomRowOffset)) / SizeProperties.FULL_ROW_OFFSET) + 0.5);
                if (bottom <= num2)
                {
                    numArray[0] = bottomRow;
                    numArray[1] = (int)(((((num2 - bottom) * SizeProperties.FULL_ROW_OFFSET) / ((float)rowHeightPixel)) + bottomRowOffset) + 0.5);
                    return numArray;
                }
                bottom -= num2;
            }
            bottomRow--;
            while (bottomRow >= 0)
            {
                rowHeightPixel = cells.GetInnerRowHeightInPixels(bottomRow);
                bottom -= rowHeightPixel;
                if (bottom <= 0)
                {
                    break;
                }
                bottomRow--;
            }
            if ((bottom <= 0) && ((bottom != 0) || (bottomRow > 0)))
            {
                if (bottom == 0)
                {
                    numArray[0] = bottomRow;
                    return numArray;
                }
                numArray[0] = bottomRow;
                numArray[1] = (int)(((-bottom * SizeProperties.FULL_ROW_OFFSET) / ((float)rowHeightPixel)) + 0.5);
            }
            return numArray;
        }

        internal int[] GetLeftAndLeftOffset(int rightColumn, int rightColumnOffset, int right)
        {
            int[] numArray = new int[2];
            if (right == 0)
            {
                numArray[0] = rightColumn;
                numArray[1] = rightColumnOffset;
                return numArray;
            }
            int viewColumnWidthPixel = 0;
            WorksheetImpl cells = this.m_workSheet;
            if (rightColumnOffset != 0)
            {
                viewColumnWidthPixel = cells.GetViewColumnWidthPixel(rightColumn);
                int num2 = (int)((((float)(viewColumnWidthPixel * rightColumnOffset)) / SizeProperties.FULL_COLUMN_OFFSET) + 0.5);
                if (right <= num2)
                {
                    numArray[0] = rightColumn;
                    numArray[1] = (int)(((((num2 - right) * SizeProperties.FULL_COLUMN_OFFSET) / ((float)viewColumnWidthPixel)) + rightColumnOffset) + 0.5);
                    return numArray;
                }
                right -= num2;
            }
            rightColumn--;
            while (rightColumn >= 0)
            {
                viewColumnWidthPixel = cells.GetViewColumnWidthPixel(rightColumn);
                right -= viewColumnWidthPixel;
                if (right <= 0)
                {
                    break;
                }
                rightColumn--;
            }
            if ((right <= 0) && ((right != 0) || (rightColumn > 0)))
            {
                numArray[0] = rightColumn;
                if (right != 0)
                {
                    numArray[0] = rightColumn;
                    numArray[1] = (int)(((-right * SizeProperties.FULL_COLUMN_OFFSET) / ((float)viewColumnWidthPixel)) + 0.5);
                }
            }
            return numArray;
        }

        internal void SetAnchor(int left, int top, int right, int bottom)
        {
            this.m_size.SetPlacementType(PlacementType.FreeFloating);
            this.m_size.Top = top;
            this.m_size.Left = left;
            this.m_size.Right = right;
            this.m_size.Bottom = bottom;
        }

        internal void SetAnchor(int topRow, int top, int leftColumn, int left, int height, int width)
        {
            PlacementType placement = this.Placement;
            int rowHeightPixel = this.m_workSheet.GetInnerRowHeightInPixels(topRow);
            top = (int)(((top * SizeProperties.FULL_ROW_OFFSET) / ((float)rowHeightPixel)) + 0.5);
            rowHeightPixel = this.m_workSheet.GetViewColumnWidthPixel(leftColumn);
            left = (int)(((left * SizeProperties.FULL_COLUMN_OFFSET) / ((float)rowHeightPixel)) + 0.5);
            this.m_size.Right = width;
            this.m_size.Bottom = height;
            this.m_size.SetLeftColumn(leftColumn);
            this.m_size.Left = left;
            this.m_size.SetTopRow(topRow);
            this.m_size.Top = top;
            if (placement != PlacementType.Move)
            {
                this.m_size.SetPlacementType(PlacementType.Move);
                this.Placement = placement;
            }
        }

        internal void SetAnchor(int topRow, int topRowOffset, int leftColumn, int leftColumnOffset, int bottomRow, int bottomRowOffset, int rightColumn, int rightColumnOffset)
        {
            PlacementType placement = this.Placement;
            int num = this.m_workSheet.GetInnerRowHeightInPixels(topRow);
            if (num <= topRowOffset)
            {
                topRowOffset = (int)SizeProperties.FULL_ROW_OFFSET;
            }
            else
            {
                topRowOffset = (int)(((topRowOffset * SizeProperties.FULL_ROW_OFFSET) / ((float)num)) + 0.5);
            }
            num = this.m_workSheet.GetViewColumnWidthPixel(leftColumn);
            if (num <= leftColumnOffset)
            {
                leftColumnOffset = (int)SizeProperties.FULL_COLUMN_OFFSET;
            }
            else
            {
                leftColumnOffset = (int)(((leftColumnOffset * SizeProperties.FULL_COLUMN_OFFSET) / ((float)num)) + 0.5);
            }
            this.m_size.SetLeftColumn(leftColumn);
            this.m_size.Left = leftColumnOffset;
            this.m_size.SetTopRow(topRow);
            this.m_size.Top = topRowOffset;
            num = this.m_workSheet.GetInnerRowHeightInPixels(bottomRow);
            if (num <= bottomRowOffset)
            {
                bottomRowOffset = (int)SizeProperties.FULL_ROW_OFFSET;
            }
            else
            {
                bottomRowOffset = (int)(((bottomRowOffset * SizeProperties.FULL_ROW_OFFSET) / ((float)num)) + 0.5);
            }
            if (rightColumn < 0x3fff)
            {
                num = this.m_workSheet.GetViewColumnWidthPixel(rightColumn);
                if (num <= rightColumnOffset)
                {
                    rightColumnOffset = (int)SizeProperties.FULL_COLUMN_OFFSET;
                }
                else
                {
                    rightColumnOffset = (int)(((rightColumnOffset * SizeProperties.FULL_COLUMN_OFFSET) / ((float)num)) + 0.5);
                }
            }
            else
            {
                rightColumn = 0x3fff;
                rightColumnOffset = (int)SizeProperties.FULL_COLUMN_OFFSET;
            }
            this.m_size.SetRightColumn(rightColumn);
            this.m_size.Right = rightColumnOffset;
            this.m_size.SetBottomRow(bottomRow);
            this.m_size.Bottom = bottomRowOffset;
            if (placement != PlacementType.MoveAndSize)
            {
                this.m_size.SetPlacementType(PlacementType.MoveAndSize);
                this.Placement = placement;
            }
        }
        #endregion

    }
}
