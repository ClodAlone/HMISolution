#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
//// For Copy To Clipboard
using System.Linq;
using System.Xml;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Windows;
using System.Collections.Generic;
#if !WinRT
using Syncfusion.Windows.Collections;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.Styles;
using System.Security.Permissions;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.ComponentModel;
using Windows.ApplicationModel.DataTransfer;
using Syncfusion.WinRT.Controls.Cells;
using Windows.UI.Popups;
using System.Threading.Tasks;
namespace Syncfusion.WinRT.Controls.Grid
#endif
{

    /// <summary>
    /// This class manages cut-copy-paste operations for the grid
    /// </summary>
    /// <remarks>
    /// You access this class from a grid with the <see cref="GridModel.CutPaste"/>
    /// property of a <see cref="GridModel"/> instance.
    /// </remarks>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridModelCutPaste : NonFinalizeDisposable
    {
        /// <summary>
        /// hold the DataObject for ClipBoard
        /// </summary>

        //// Syncfusion.Windows.Forms.Grid.GridData gridData = new Syncfusion.Windows.Forms.Grid.GridData();
        private GridRangeInfoList copyRangeList = null;

#if (!SILVERLIGHT && !WinRT)
        private DataObject dataObject = null;
#endif
#if SILVERLIGHT
        private string dataObject = string.Empty;
#endif
#if WinRT
        private DataPackage dataObject = new DataPackage();
#endif

        GridRangeInfoList coveredRanges = null;

        /// <summary>
        /// Gets or sets the grid model.
        /// </summary>
        public GridModel Model
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies whether the current operation is clipboard-cut.
        /// </summary>
        /// <value>
        /// <c>True, if the current operation is clipboard-cut; false otherwise.</c>
        /// </value>
        public bool CutFlag
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies whether the current operation is clipboard-cut and whether to cut the cell text along with its style.
        /// </summary>
        /// <value>
        /// <c>True, if the current operation is clipboard-cut and the CopyPaste option is CutCell; false otherwise.</c>
        /// </value>
        public bool CutCell
        {
            get;
            set;
        }


        /// <summary>
        /// Initializes a new <see cref="GridModelCutPaste"/>.
        /// </summary>
        /// <param name="model">The grid model.</param>
        public GridModelCutPaste(GridModel model)
        {
            this.Model = model;
        }

        /// <summary>
        /// Contain the GridStyleInfo for Cells
        /// </summary>
        private GridCellData gridData;

        public GridCellData CellData
        {
            get { return gridData; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.gridData = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Checks if there are selected ranges that can be copied to clipboard or if the current cell's contents can be copied.
        /// </summary>
        /// <param name="gridControl">reference for GridModel</param>
        /// <param name="mod">reference for GridModel</param>
        /// <returns>True if there is information available to be copied to clipboard.</returns>
        public bool CanCopy()
        {
            bool flag = true;
            try
            {
                foreach (GridControlBase gcb in Model.Views.Where(gcb => gcb.CurrentCell.IsEditing))
                {
                    flag = false;
                }
                if ((this.Model.SelectedRanges.Count > 0 && flag) && (this.Model.Options.CopyPasteOption & CopyPaste.ExcludeCurrentCell) != CopyPaste.ExcludeCurrentCell)
                {
                    GridCutPasteEventArgs e = new GridCutPasteEventArgs(this.Model.SelectedRanges);
                    this.Model.RaiseClipboardCanCopy(e);
                    return !e.Handled;
                }
            }
            catch
            { }

            return false;
        }

        /// <summary>
        /// Copies the contents of cells in selected to clipboard and the current cell's contents.
        /// </summary>
        public void Copy()
        {
            CutCell = false;
            CutFlag = false;
            GridCutPasteEventArgs e = new GridCutPasteEventArgs(this.Model.SelectedRanges);
            this.Model.RaiseClipboardCopy(e);
#if WinRT
            this.dataObject.SetText(e.DataObject);
#else
            this.dataObject= e.DataObject;
#endif
            GridRangeInfoList rangeList = this.Model.SelectedRanges.Clone();

            // Expand the Selected ranges when Table, Row or Column is selected. 
            rangeList = this.GetExpandedRange(rangeList);
            this.copyRangeList = rangeList;

            if (!e.Handled)
            {
                if (this.Model.GridCopyPaste != null)
                {
                    // Copy the Cell models to Data Object. 
                    this.CopyCellsToDataObject(rangeList, false);
                    this.Model.GridCopyPaste.Copy(this.gridData, rangeList);
                }
                else
                {
                    this.CopyRange(rangeList, false, true);
                }
            }
            else
            {
                if (this.dataObject != null)
                {
                    try
                    {
#if (!SILVERLIGHT && !WinRT)
                        Clipboard.SetDataObject(string.Empty);
#endif
#if !WinRT
                        Clipboard.SetText(string.Empty);
#else
                        Clipboard.Clear();
#endif
                    }

                    finally
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Checks if there are selected ranges that can be cut and copied to clipboard or if the current cell's contents can be cut and copied.
        /// </summary>
        public bool CanCut()
        {
            bool flag = true;
            try
            {
                foreach (GridControlBase gcb in Model.Views.Where(gcb => gcb.CurrentCell.IsEditing))
                {
                    flag = false;
                }

                if ((this.Model.SelectedRanges.Count > 0 && flag) && (this.Model.Options.CopyPasteOption & CopyPaste.ExcludeCurrentCell) != CopyPaste.ExcludeCurrentCell)
                {
                    GridCutPasteEventArgs e = new GridCutPasteEventArgs(this.Model.SelectedRanges);
                    this.Model.RaiseClipboardCanCut(e);
                    return !e.Handled;
                }
            }
            catch
            { }

            return false;
        }

        /// <summary>
        /// Cuts and copies the contents of cells in selected to clipboard and the current cell's contents.
        /// </summary>
        public void Cut()
        {
            CutFlag = true;
            GridCutPasteEventArgs e = new GridCutPasteEventArgs(this.Model.SelectedRanges);
            this.Model.RaiseClipboardCut(e);
#if WinRT
            this.dataObject.SetText(e.DataObject);
#else
            this.dataObject = e.DataObject;
#endif
            GridRangeInfoList rangeList = this.Model.SelectedRanges.Clone();
            // Expand the Selected ranges when Table, Row or Column is selected. 
            rangeList = this.GetExpandedRange(rangeList);
            this.copyRangeList = rangeList;
            if (!e.Handled)
            {
                if (this.Model.GridCopyPaste != null)
                {
                    this.Model.GridCopyPaste.Cut(this.gridData, rangeList);
                }
                else
                {
                    this.CutRange(rangeList);
                }
                foreach (GridControlBase gcb in Model.Views)
                {
                    if (gcb.CurrentCell != null)
                    {
                        gcb.CurrentCell.Deactivate();
                        gcb.CurrentCell.Activate(gcb.CurrentCell.RowIndex, gcb.CurrentCell.ColumnIndex);
                    }
                }
            }
            else
            {
                if (this.dataObject != null)
                {
                    try
                    {

#if (!SILVERLIGHT && !WinRT)
                        Clipboard.SetDataObject(this.dataObject);
#endif

#if WinRT
                        Clipboard.SetContent(dataObject);
#endif



                    }
                    finally { }
                }
            }

        }

        /// <summary>
        /// Checks if there is information on the clipboard that can be pasted into the grid.
        /// </summary>
        /// <param name="gridControl">Reference for GridControl</param>
        /// <returns>Call this method for example to enable or gray out menu commands like "Paste Cells".</returns>
        public bool CanPaste()
        {

            try
            {
#if (!SILVERLIGHT && !WinRT)
                IDataObject idata = null;
#endif
#if SILVERLIGHT
            string ClipboardText = string.Empty;
#endif
#if WinRT
            DataPackageView idata;
#endif
                try
                {
#if (!SILVERLIGHT && !WinRT)
                    idata = Clipboard.GetDataObject();
#endif
#if SILVERLIGHT
                ClipboardText = Clipboard.GetText();
#endif
#if WinRT
                idata = Clipboard.GetContent();
#endif


                }
                finally { }

#if !SILVERLIGHT
                if (idata == null)
                {
                    return false;
                }
#else 
            if ((this.Model.Options.CopyPasteOption  == CopyPaste.PasteText  ) &&ClipboardText == string.Empty)
            {
                return false;
            }
#endif
                string buffer = string.Empty;
                bool flag = true;
                foreach (GridControlBase gcb in Model.Views.Where(gcb => gcb.CurrentCell.IsEditing))
                {
                    flag = false;
                }

#if !SILVERLIGHT && !WinRT
                if (idata.GetDataPresent(DataFormats.UnicodeText))
                {
                    buffer = idata.GetData(DataFormats.UnicodeText) as string;
                }
                else if (idata.GetDataPresent(DataFormats.Text))
                {
                    buffer = idata.GetData(DataFormats.Text) as string;
                }
#endif
#if SILVERLIGHT
            buffer = ClipboardText;
#endif
#if WinRT
            buffer = idata.ToString();
#endif
                if (((buffer != string.Empty) | (gridData != null)) && (this.Model.Options.CopyPasteOption & CopyPaste.ExcludeCurrentCell) != CopyPaste.ExcludeCurrentCell && flag)
                {
                    GridCutPasteEventArgs e = new GridCutPasteEventArgs(this.copyRangeList);
                    this.Model.RaiseClipboardCanPaste(e);
                    return !e.Handled;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Paste information from the clipboard into the grid at the current selected range or current cell.
        /// </summary>
        public void Paste()
        {
            GridCutPasteEventArgs e = new GridCutPasteEventArgs(this.Model.SelectedRanges);
            this.Model.RaiseClipboardPaste(e);

#if (!SILVERLIGHT && !WinRT)
            IDataObject idata = null;
#endif
#if SILVERLIGHT
            string idata = string.Empty;
#endif

#if WinRT
            var idata = new DataPackage();
#endif
            GridRangeInfoList rangeList = this.Model.SelectedRanges.Clone();
            // Expand the Selected ranges when Table, Row or Column is selected. 
            rangeList = GetExpandedRange(rangeList);

            try
            {
                if (!e.Handled)
                {
#if (!SILVERLIGHT && !WinRT)
                    idata = this.Model.GridCopyPaste != null
                                ? this.Model.GridCopyPaste.Paste(rangeList)
                                  : Clipboard.GetDataObject();
#endif
#if SILVERLIGHT
                    idata = this.Model.GridCopyPaste != null
                                ? this.Model.GridCopyPaste.Paste(rangeList)
                                : Clipboard.GetText();
#endif
#if WinRT
                    idata.SetText(this.Model.GridCopyPaste != null
                        ? this.Model.GridCopyPaste.Paste(rangeList)
                        : Clipboard.GetContent().ToString());
#endif
                }
                else
                {
#if !WinRT
                    idata = e.DataObject;
#else
                    idata.SetText(e.DataObject);
#endif
                }
            }

            finally { }

            if (idata == null || e.Handled)
            {
                return;
            }
#if !WinRT
            this.OnPasteFromClipboard(rangeList, idata);
#else
            this.OnPasteFromClipboard(rangeList, idata.ToString());
#endif
            this.Model.RaiseClipboardPasted(e);
        }



        /// <summary>
        /// Expands the Range list when it is Table, ROws, Cols to Cells. 
        /// </summary>
        /// <param name="rangeList">Range List to Expand.</param>
        /// <returns>Expanded Cells Range</returns>
        /// <remarks>When  we Select Entire Table, Column, Rows etc.. Expanded Ranges is must.</remarks>
        private GridRangeInfoList GetExpandedRange(GridRangeInfoList rangeList)
        {
            if (rangeList.Count > 0)
            {

                // If Selected Range is Column then expand it as cells.
                if (rangeList[0].IsCols)
                {
                    rangeList = rangeList.ExpandRanges(rangeList[0].Top + this.Model.HeaderRows, rangeList[0].Left, Model.RowCount, rangeList[0].Left);
                }
                // If Selected Range is Rows then expand it as cells.
                if (rangeList[0].IsRows)
                {
                    int firstColumn = rangeList[0].Left + this.Model.HeaderColumns;
#if !WinRT
                    if (this.Model is GridDataTableModel)
                        firstColumn = (this.Model as GridDataTableModel).Grid.NavigateWithArrowKeysCellsRange.Left;
#endif
                    rangeList = rangeList.ExpandRanges(rangeList[0].Top, firstColumn, rangeList[0].Top, Model.ColumnCount);
                }
                // If Selected Range is Table then expand it as cells.
                if (rangeList[0].IsTable)
                {
                    rangeList = rangeList.ExpandRanges(rangeList[0].Top, rangeList[0].Left, Model.RowCount, Model.ColumnCount);
                }
                return rangeList;
            }

            return null;
        }

        /// <summary>
        /// Copies the contents of a specified range of cells to clipboard.
        /// </summary>
        /// <param name="rangeList">contains the currently selected range list</param>
        public void CopyRange(GridRangeInfoList rangeList, bool clear, bool copy)
        {
#if (!SILVERLIGHT && !WinRT)

            // Copy the cell values with some basic styles to Excel using Xml format.
            if (((this.Model.Options.CopyPasteOption & CopyPaste.XmlCopyPaste) == CopyPaste.XmlCopyPaste))
            {
                this.CopyXmlToClipboard(rangeList, clear);
            }
#endif
            // Copy the Plain text of selected ranges to the Clipboard content. 
            if (((this.Model.Options.CopyPasteOption & CopyPaste.CopyText) == CopyPaste.CopyText) || (((this.Model.Options.CopyPasteOption & CopyPaste.CutText) == CopyPaste.CutText) && clear))
            {
                this.CopyTextToClipboard(rangeList, clear);
            }
            // Copy the complete cell models in the same grid.
            if ((((this.Model.Options.CopyPasteOption & CopyPaste.CopyCellData) == CopyPaste.CopyCellData && copy)) || (((this.Model.Options.CopyPasteOption & CopyPaste.CutCell) == CopyPaste.CutCell) && clear))
            {
                this.CopyCellsToDataObject(rangeList, clear);
            }
        }

        /// <summary>
        /// Copies the formatted text of a specified range of cells to clipboard.
        /// </summary>
        /// <param name="rangeList">The range list with cells to be copied.</param>
        private void CopyTextToClipboard(GridRangeInfoList rangeList, bool clear)
        {
            string buffer;
            int rowCount;
            int colCount;
            this.Model.TextDataExchange.CopyTextToBuffer(out buffer, rangeList, out rowCount, out colCount, clear);
#if (!SILVERLIGHT && !WinRT)
            DataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.UnicodeText, buffer);
#endif
            try
            {
#if (!SILVERLIGHT && !WinRT)
                Clipboard.SetDataObject(dataObject);
#endif
#if SILVERLIGHT
                Clipboard.SetText(buffer);
#endif
#if WinRT
                var data = new DataPackage();
                data.SetText(buffer);
                Clipboard.SetContent(data);
#endif
            }
            finally { }
        }

#if !SILVERLIGHT
        /// <summary>
        ///  Copy the Cells with basic styles in Xml format to support in MS Excel. 
        /// </summary>
        /// <param name="rangeList">Range list  to copy the data </param>
        /// <param name="clear">If set to <see langword="true"/> the selection clears otherwise the selection remains.</param>
        /// <remarks></remarks>
        private void CopyXmlToClipboard(GridRangeInfoList rangeList, bool clear)
        {
            string buffer;
            int rowCount;
            int colCount;

            //  Buffer value is set in Clipboard in the below method. 
            this.Model.TextDataExchange.CopyXmlToBuffer(out buffer, rangeList, out rowCount, out colCount, clear);
        }
#endif

        /// <summary>
        /// Creates a object and initializes it with style objects and covered cell information of a range of cells in the grid.
        /// </summary>
        /// <param name="rangeList">contains the currently selected range</param>
        /// <param name="bLoadBaseStyles">Contain the base style</param>
        public GridCellData CopyCellsToDataObject(GridRangeInfoList rangeList, bool clear)
        {
            coveredRanges = new GridRangeInfoList();
            this.gridData = new GridCellData();
            GridRangeInfoList rowRanges = rangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
            GridRangeInfoList colRanges = rangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);
            // int nrows = rowRanges.Cast<GridRangeInfo>().Sum(range => range.Height); Unused local variable

            // int ncols = colRanges.Cast<GridRangeInfo>().Sum(range => range.Width); Unused local variable

            GridStyleInfo gridstyle;
            int numRowsDone = 0, numColsDone = 0;
            for (int rowindex = 0; rowindex < rowRanges.Count; rowindex++)
            {
                for (int colindex = 0; colindex < colRanges.Count; colindex++)
                {
                    // REVIEW: does this also work with GridRangeInfoType.Rows and Cols?
                    GridRangeInfo intersectRange = GridRangeInfo.IntersectRange(rowRanges[rowindex], colRanges[colindex]);
                    GridRangeInfoList cl = Model.CoveredRanges.Ranges.GetRangesContained(intersectRange);

                    foreach (GridRangeInfo range in cl)
                    {
                        coveredRanges.Add(range.OffsetRange(-intersectRange.Top, -intersectRange.Left));
                    }
                }
                for (int nrow = rowRanges[rowindex].Top; nrow <= rowRanges[rowindex].Bottom; nrow++)
                {
                    numColsDone = 0;

                    for (int colindex = 0; colindex < colRanges.Count; colindex++)
                    {
                        for (int ncol = colRanges[colindex].Left; ncol <= colRanges[colindex].Right; ncol++)
                        {
                            if (!rangeList.AnyRangeContains(GridRangeInfo.Cell(nrow, ncol)))
                            {
                                numColsDone++;
                                continue;
                            }
                            gridstyle = this.Model[nrow, ncol];
                            GridStyleInfoStore gsis = gridstyle.Store;
                            this.gridData[numRowsDone, numColsDone] = (GridStyleInfoStore)gsis.Clone();
                            if (clear && ((this.Model.Options.CopyPasteOption & CopyPaste.CutCell) == CopyPaste.CutCell))
                            {
                                this.Model[nrow, ncol] = new GridStyleInfo();
                            }
                            numColsDone++;
                        }
                    }
                    numRowsDone++;
                }
            }
            if (clear && ((this.Model.Options.CopyPasteOption & CopyPaste.CutCell) == CopyPaste.CutCell))
            {
                foreach (GridRangeInfo rc in from GridRangeInfo range in rangeList
                                             from GridRangeInfo rc in Model.CoveredRanges.Ranges.GetRangesContained(range)
                                             select rc)
                {
                    this.Model.CoveredCells.Remove(new CoveredCellInfo(rc.Top, rc.Left, rc.Bottom, rc.Right));
                    this.Model.CoveredCells.Ranges.Remove(rc);
                    this.Model.CoveredCells.ResetCellSpan(new CoveredCellInfo(rc.Top, rc.Left, rc.Bottom, rc.Right));
                }
            }

            foreach (GridRangeInfo range in rangeList)
            {
                this.Model.InvalidateCell(range);
            }

            this.Model.InvalidateVisual(true);

            foreach (GridControlBase gcb in Model.Views.Where(gcb => gcb.CurrentCell != null))
            {
                gcb.CurrentCell.Deactivate();
                gcb.CurrentCell.Activate(gcb.CurrentCell.RowIndex, gcb.CurrentCell.ColumnIndex);
            }

            return gridData;
        }

        /// <summary>
        /// Cuts and copies the contents of a specified range of cells to clipboard.
        /// </summary>
        /// <param name="rangeList">contains the currently selected range list</param>
        public void CutRange(GridRangeInfoList rangeList)
        {
            foreach (GridRangeInfo range in rangeList)
            {
                for (int i = range.Top; i <= range.Bottom; i++)
                {
                    for (int j = range.Left; j <= range.Right; j++)
                    {
                        if ((this.Model[i, j] as GridStyleInfo).ReadOnly)
                        {
                            return;
                        }
                    }
                }
            }
            if (((this.Model.Options.CopyPasteOption & CopyPaste.CutText) == CopyPaste.CutText) || ((this.Model.Options.CopyPasteOption & CopyPaste.CutCell) == CopyPaste.CutCell))
            {
                CutCell = true;
                this.CopyRange(rangeList, true, false);
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Get The DataObject From the Clipboard 
        /// </summary>
        /// <param name="range"></param>
#if !WinRT
        public void OnPasteFromClipboard(GridRangeInfoList range, IDataObject data)
        {
#else
        public void OnPasteFromClipboard(GridRangeInfoList range, string data)
        {
#endif
            string buffer = string.Empty;

#if !WinRT
            IDataObject idata = data;

            // Gets the copied data from Clipboard
            if (idata.GetDataPresent(DataFormats.UnicodeText))
            {
                buffer = idata.GetData(DataFormats.UnicodeText) as string;
                int lastIndex = buffer.LastIndexOf("\r\n");
                if (lastIndex>0 && lastIndex == buffer.Length - 2)
                    buffer = buffer.Remove(buffer.Length - 2);
            }
            else if (idata.GetDataPresent(DataFormats.Text))
            {
                buffer = idata.GetData(DataFormats.Text) as string;
            }
            
            // Gets the Xml data from the Clipboard. This will satisfy only when we copy from MS Excel. 
            if ((this.Model.Options.CopyPasteOption & CopyPaste.XmlCopyPaste) == CopyPaste.XmlCopyPaste)
            {
                if (Clipboard.ContainsData("XML Spreadsheet"))
                {
                    var xmlStream = Clipboard.GetData("Xml Spreadsheet") as MemoryStream;
                    var xDocument = new XmlDocument();
                    if (xmlStream != null) xDocument.Load(xmlStream);
                    this.Model.TextDataExchange.PasteXmlFromBuffer(xDocument, range);
                }
                return;
            }
#endif
            // PAste the plain text copied in the clipboard. 
            if ((this.Model.Options.CopyPasteOption & CopyPaste.PasteText) == CopyPaste.PasteText)
            {
                this.Model.TextDataExchange.PasteTextFromBuffer(buffer, range);
                return;
            }
            // Paste the cell models copied in the GridDataObject. 
            if ((this.Model.Options.CopyPasteOption & CopyPaste.PasteCell) != CopyPaste.PasteCell) return;
            this.PasteCellsFromStyle(buffer, range);
            return;
        }
#endif
        /// <summary>
        /// Get The DataObject From the Clipboard 
        /// </summary>
        /// <param name="range"></param>
#if SILVERLIGHT
        public void OnPasteFromClipboard(GridRangeInfoList range, string data)
        {
            string buffer =string.Empty;
            try
            {
                buffer = Clipboard.GetText();
                int lastIndex = buffer.LastIndexOf("\r\n");
                if (lastIndex==buffer.Length-2)
                    buffer = buffer.Remove(buffer.Length - 2);
                
            }
            catch (SecurityException ex) 
            { 
            }
            if ((this.Model.Options.CopyPasteOption & CopyPaste.PasteText) == CopyPaste.PasteText)
            {
                this.Model.TextDataExchange.PasteTextFromBuffer(buffer, range);
            }
            if ((this.Model.Options.CopyPasteOption & CopyPaste.PasteCell) == CopyPaste.PasteCell)
            {
                this.PasteCellsFromStyle(buffer, range);
            }
        }

#endif

        /// <summary>
        ///  Checks the space avaliblity for Buffer text to paste. 
        /// </summary>
        /// <param name="rangeList"> Selected Ranges</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool PasteBoundCheck(GridRangeInfoList rangeList)
        {

            if (rangeList != null)
            {
                GridRangeInfoList rowRanges = GridRangeInfoList.Empty;
                GridRangeInfoList colRanges = GridRangeInfoList.Empty;
                // If the instance is in same grid the copy rangelist will contains the Rangelist to paste.
                if (this.copyRangeList != null)
                {
                    rowRanges = this.copyRangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
                    colRanges = this.copyRangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);
                }
                int numRows = 0;
                int numCols = 0;

                for (int i = 0; i < rowRanges.Count; i++)
                {
                    numRows += rowRanges[i].Height;
                }

                for (int j = 0; j < colRanges.Count; j++)
                {
                    numCols += colRanges[j].Width;
                }

                int top = rangeList[0].Top;
                int left = rangeList[0].Left;
                int bottom = rangeList[0].Top + numRows - 1;
                int right = rangeList[0].Left + numCols - 1;

                if (bottom > this.Model.RowCount || bottom < 0)
                {
                    bottom = this.Model.RowCount - 1;
                }
                if (right > this.Model.ColumnCount || right < 0)
                {
                    right = this.Model.ColumnCount - 1;
                }
                GridRangeInfo r = GridRangeInfo.Cells(top, left, bottom, right);
                // Validates the space and disply the messagebox if it fails.
                if (r.Bottom == this.Model.RowCount || r.Right == this.Model.ColumnCount)
                {
#if (!SILVERLIGHT && !WinRT)
                    if (MessageBox.Show(GridDataResourceWrapper.NotEnoughSpaceMessage, GridDataResourceWrapper.ClipboardCopyPaste, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return false;
                    }
#endif
#if SILVERLIGHT
                    if (MessageBox.Show(GridDataResourceWrapper.NotEnoughSpaceMessage, GridDataResourceWrapper.ClipboardCopyPaste, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
#endif
#if WinRT
                    //bool? result = null;
                    //result = ShowDialog();
                    //if (result == false)
                    //    return false;
#endif
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Calculates the Required space ondemad using buffer value when the Range is not selected.
        /// </summary>
        /// <param name="rangeList"></param>
        /// <param name="buffer"></param>
        /// <param name="delim"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool PasteBoundCheck(GridRangeInfoList rangeList, string buffer, string delim)
        {

            int numRows = 0;
            int numCols = 0;
            if (rangeList != null)
            {
                GridRangeInfoList rowRanges = GridRangeInfoList.Empty;
                GridRangeInfoList colRanges = GridRangeInfoList.Empty;
                // If the instance is in same grid the copy rangelist will contains the Rangelist to paste.
                if (this.copyRangeList != null)
                {
                    rowRanges = this.copyRangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
                    colRanges = this.copyRangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);

                    for (int i = 0; i < rowRanges.Count; i++)
                    {
                        numRows += rowRanges[i].Height;
                    }

                    for (int j = 0; j < colRanges.Count; j++)
                    {
                        numCols += colRanges[j].Width;
                    }
                }
                else
                {
                    //Checks the  range from buffer value to paste. 
                    numRows = buffer.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
                    string rowdata = new StringReader(buffer).ReadLine();
                    if (rowdata != null)
                        numCols = rowdata.Split(new string[] { delim }, StringSplitOptions.None).Length;
                }
                int top = rangeList[0].Top;
                int left = rangeList[0].Left;
                int bottom = rangeList[0].Top + numRows - 1;
                int right = rangeList[0].Left + numCols - 1;

                //if (bottom > this.Model.RowCount || bottom < 0)
                //{
                //    bottom = this.Model.RowCount - 1;
                //}
                //if (right > this.Model.ColumnCount || right < 0)
                //{
                //    right = this.Model.ColumnCount - 1;
                //}

                GridRangeInfo r = GridRangeInfo.Cells(top, left, bottom, right);

                if (r.Bottom >= this.Model.RowCount || r.Right >= this.Model.ColumnCount)
                {
                    string rowdata = new StringReader(buffer).ReadLine();
                    if (rowdata != null)
                        numCols = rowdata.Split(new string[] { delim.ToString() }, StringSplitOptions.None).Length;
                    top = rangeList[0].Top;
                    left = rangeList[0].Left;
                    right = rangeList[0].Left + numCols - 1;

                    //if (bottom > this.Model.RowCount || bottom < 0)
                    //{
                    //    bottom = this.Model.RowCount - 1;
                    //}
                    //if (right > this.Model.ColumnCount || right < 0)
                    //{
                    //    right = this.Model.ColumnCount - 1;
                    //}
                    r = GridRangeInfo.Cells(top, left, bottom, right);

                    if (r.Bottom >= this.Model.RowCount || r.Right >= this.Model.ColumnCount)
                    {
#if (!SILVERLIGHT && !WinRT)
                        if (MessageBox.Show(GridDataResourceWrapper.NotEnoughSpaceMessage,
                                            GridDataResourceWrapper.ClipboardCopyPaste,
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            return false;
                        }
#endif
#if SILVERLIGHT
                        if (MessageBox.Show(GridDataResourceWrapper.NotEnoughSpaceMessage, GridDataResourceWrapper.ClipboardCopyPaste, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            {
                                return false;
                            }
#endif
#if WinRT
                        //bool? result = null;
                        //result = ShowDialog();
                        //if (result == false)
                        //    return false;
#endif
                    }
                }
                return true;
            }
            return false;
        }
#if WinRT
        public async Task<bool?> ShowDialog()
        {
            bool? result = null;
            MessageDialog md = new MessageDialog("There is not enough space", "Warning");
            md.Commands.Add(new UICommand("OK", new UICommandInvokedHandler((cmd) => result = true)));
            md.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler((cmd) => result = false)));
            await md.ShowAsync();

            return result;

        }
#endif
        /// <summary>
        /// Copy the Cell Models from the Selected Ranges. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rangeList"></param>
        /// <param name="clear">If set to <see langword="true"/>, Clears the selected Ranges ; otherwise,  remains the selected ranges.</param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <remarks></remarks>
        internal void CopyCellsToDataObject(out GridCellData data, GridRangeInfoList rangeList, bool clear, out int row, out int column)
        {
            coveredRanges = new GridRangeInfoList();
            this.gridData = new GridCellData();
            GridRangeInfoList rowRanges = rangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
            GridRangeInfoList colRanges = rangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);
            int nrows = rowRanges.Cast<GridRangeInfo>().Sum(range => range.Height);
            int ncols = colRanges.Cast<GridRangeInfo>().Sum(range => range.Width);
            GridStyleInfo gridstyle;
            int numRowsDone = 0, numColsDone = 0;
            for (int rowindex = 0; rowindex < rowRanges.Count; rowindex++)
            {
                for (int colindex = 0; colindex < colRanges.Count; colindex++)
                {
                    // REVIEW: does this also work with GridRangeInfoType.Rows and Cols?
                    GridRangeInfo intersectRange = GridRangeInfo.IntersectRange(rowRanges[rowindex], colRanges[colindex]);
                    GridRangeInfoList cl = Model.CoveredRanges.Ranges.GetRangesContained(intersectRange);

                    foreach (GridRangeInfo range in cl)
                    {
                        coveredRanges.Add(range.OffsetRange(-intersectRange.Top, -intersectRange.Left));
                    }
                }
                for (int nrow = rowRanges[rowindex].Top; nrow <= rowRanges[rowindex].Bottom; nrow++)
                {
                    numColsDone = 0;

                    for (int colindex = 0; colindex < colRanges.Count; colindex++)
                    {
                        for (int ncol = colRanges[colindex].Left; ncol <= colRanges[colindex].Right; ncol++)
                        {
                            if (!rangeList.AnyRangeContains(GridRangeInfo.Cell(nrow, ncol)))
                            {
                                numColsDone++;
                                continue;
                            }
                            gridstyle = this.Model[nrow, ncol];
                            GridStyleInfoStore gsis = gridstyle.Store;
                            this.gridData[numRowsDone, numColsDone] = (GridStyleInfoStore)gsis.Clone();
                            if (clear && ((this.Model.Options.CopyPasteOption & CopyPaste.CutCell) == CopyPaste.CutCell))
                            {
                                this.Model[nrow, ncol] = new GridStyleInfo();
                            }
                            numColsDone++;
                        }
                    }

                    numRowsDone++;
                }
            }
            data = this.gridData;
            row = nrows;
            column = ncols;
            if (clear && ((this.Model.Options.CopyPasteOption & CopyPaste.CutCell) == CopyPaste.CutCell))
            {
                foreach (GridRangeInfo range in rangeList)
                {
                    foreach (GridRangeInfo rc in Model.CoveredRanges.Ranges.GetRangesContained(range))
                    {
                        this.Model.CoveredCells.Remove(new CoveredCellInfo(rc.Top, rc.Left, rc.Bottom, rc.Right));
                        this.Model.CoveredCells.Ranges.Remove(rc);
                        this.Model.CoveredCells.ResetCellSpan(new CoveredCellInfo(rc.Top, rc.Left, rc.Bottom, rc.Right));
                    }
                }
            }
            foreach (GridRangeInfo range in rangeList)
            {
                this.Model.InvalidateCell(range);
            }
            this.Model.InvalidateVisual(true);
            foreach (GridControlBase gcb in Model.Views)
            {
                if (gcb.CurrentCell != null)
                {
                    gcb.CurrentCell.Deactivate();
                    gcb.CurrentCell.Activate(gcb.CurrentCell.RowIndex, gcb.CurrentCell.ColumnIndex);
                }
            }
        }

        /// <summary>
        /// PAste the cell models.
        /// </summary>
        /// <param name="data">Copies Cell Models</param>
        /// <param name="rangeList">Cepied Range list</param>
        /// <returns></returns>
        internal bool PasteCellsFromStyle(GridCellData data, GridRangeInfoList rangeList)
        {
            if (data == null)
                return false;

            if (rangeList != null)
            {
                GridRangeInfoList rowRanges = rangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
                GridRangeInfoList colRanges = rangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);
                int numRows = 0;
                int numCols = 0;

                for (int i = 0; i < rowRanges.Count; i++)
                {
                    numRows += rowRanges[i].Height;
                }

                for (int j = 0; j < colRanges.Count; j++)
                {
                    numCols += colRanges[j].Width;
                }

                int top = rangeList[0].Top;
                int left = rangeList[0].Left;
                int bottom = rangeList[0].Top + numRows - 1;
                int right = rangeList[0].Left + numCols - 1;

                if (bottom > this.Model.RowCount)
                {
                    bottom = this.Model.RowCount;
                }

                if (right > this.Model.ColumnCount)
                {
                    right = this.Model.ColumnCount;
                }

                GridRangeInfo r = GridRangeInfo.Cells(top, left, bottom, right);

                if (!PasteBoundCheck(rangeList))
                {
                    return false;
                }

                if ((this.Model.Options.CopyPasteOption & CopyPaste.IncludeStyle) == CopyPaste.IncludeStyle)
                {
                    for (int rowIndex = r.Top; rowIndex <= r.Bottom; rowIndex++)
                    {
                        for (int colIndex = r.Left; colIndex <= r.Right; colIndex++)
                        {
                            CoveredCellInfo rc = this.Model.CoveredCells.GetCoveredCell(rowIndex, colIndex);
                            if (rc != null)
                            {
                                this.Model.CoveredCells.Remove(new CoveredCellInfo(rc.Top, rc.Left, rc.Bottom, rc.Right));
                                this.Model.CoveredCells.Ranges.Remove(GridRangeInfo.Cells(rc.Top, rc.Left, rc.Bottom, rc.Right));
                                this.Model.CoveredCells.ResetCellSpan(new CoveredCellInfo(rc.Top, rc.Left, rc.Bottom, rc.Right));
                            }
                        }
                    }
                }


                for (int rowIndex = r.Top; rowIndex <= r.Bottom; rowIndex++)
                {
                    for (int colIndex = r.Left; colIndex <= r.Right; colIndex++)
                    {
                        GridStyleInfo gsi = new GridStyleInfo(data[rowIndex - r.Top, colIndex - r.Left]);

                        if (this.Model[rowIndex, colIndex] != null)
                        {
                            if (data[rowIndex - r.Top, colIndex - r.Left] != null)
                            {
                                if ((this.Model.Options.CopyPasteOption & CopyPaste.IncludeStyle) == CopyPaste.IncludeStyle)
                                {
                                    this.Model[rowIndex, colIndex] = gsi;
                                }
                                else
                                {
                                    this.Model[rowIndex, colIndex].CellValue = gsi.CellValue;
                                }
                            }
                        }
                    }
                }

                if ((this.Model.Options.CopyPasteOption & CopyPaste.IncludeStyle) == CopyPaste.IncludeStyle)
                {
                    foreach (GridRangeInfo range in rangeList)
                    {
                        int coverTop = range.Top;
                        int coverLeft = range.Left;

                        if (coveredRanges != null)
                        {
                            foreach (GridRangeInfo rc in coveredRanges)
                            {
                                GridRangeInfo rcOffset = rc.OffsetRange(coverTop, coverLeft);
                                if (!Model.CoveredRanges.Ranges.AnyRangeIntersects(rcOffset))
                                {

                                    if (rcOffset.Bottom > this.Model.RowCount - 1 || rcOffset.Right > this.Model.ColumnCount - 1)
                                    {
                                        continue;
                                    }

                                    this.Model.CoveredCells.Add(new CoveredCellInfo(rcOffset.Top, rcOffset.Left, rcOffset.Bottom, rcOffset.Right));
                                    this.Model.CoveredCells.Ranges.Add(rcOffset);
                                }
                            }
                        }
                    }
                }

                Model.Selections.Clear();
                Model.Selections.Add(GridRangeInfo.Cells(top, left, bottom - 1, right - 1));
                this.Model.InvalidateCell(r.ToCellSpan());
                this.Model.InvalidateVisual(true);

                if (CutFlag || CutCell)
                {
                    gridData = null;
                    CutFlag = false;
                    try
                    {
#if (!SILVERLIGHT && !WinRT)
                        Clipboard.SetDataObject(string.Empty);
#endif
#if Silverlight
                        Clipboard.SetText(string.Empty);
#endif
#if WinRT
                        Clipboard.SetContent(null);
#endif
                    }
                    finally { }
                }
            }
            return true;
        }



        /// <summary>
        /// Set the style information for the new cell.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="rangeList"></param>
        private bool PasteCellsFromStyle(string buffer, GridRangeInfoList rangeList)
        {
            if (gridData == null)
                return false;

            if (rangeList != null)
            {
                GridRangeInfoList rowRanges = this.copyRangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
                GridRangeInfoList colRanges = this.copyRangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);
                int numRows = 0;
                int numCols = 0;

                for (int i = 0; i < rowRanges.Count; i++)
                {
                    numRows += rowRanges[i].Height;
                }

                for (int j = 0; j < colRanges.Count; j++)
                {
                    numCols += colRanges[j].Width;
                }

                int top = rangeList[0].Top;
                int left = rangeList[0].Left;
                int bottom = rangeList[0].Top + numRows - 1;
                int right = rangeList[0].Left + numCols - 1;

                if (bottom > this.Model.RowCount)
                {
                    bottom = this.Model.RowCount;
                }

                if (right > this.Model.ColumnCount)
                {
                    right = this.Model.ColumnCount;
                }

                GridRangeInfo r = GridRangeInfo.Cells(top, left, bottom, right);

                if (!PasteBoundCheck(rangeList))
                {
                    return false;
                }

                if ((this.Model.Options.CopyPasteOption & CopyPaste.IncludeStyle) == CopyPaste.IncludeStyle)
                {
                    for (int rowIndex = r.Top; rowIndex <= r.Bottom; rowIndex++)
                    {
                        for (int colIndex = r.Left; colIndex <= r.Right; colIndex++)
                        {
                            CoveredCellInfo rc = this.Model.CoveredCells.GetCoveredCell(rowIndex, colIndex);
                            if (rc != null)
                            {
                                this.Model.CoveredCells.Remove(new CoveredCellInfo(rc.Top, rc.Left, rc.Bottom, rc.Right));
                                if (this.Model.CoveredCells.Ranges.Count > 0)
                                    this.Model.CoveredCells.Ranges.Remove(GridRangeInfo.Cells(rc.Top, rc.Left, rc.Bottom, rc.Right));
                                if (this.Model.CoveredCells.Ranges.Count > 0)
                                    this.Model.CoveredCells.ResetCellSpan(new CoveredCellInfo(rc.Top, rc.Left, rc.Bottom, rc.Right));
                            }
                        }
                    }
                }

#if !WinRT

                for (int rowIndex = r.Top; rowIndex <= r.Bottom; rowIndex++)
                {
                    for (int colIndex = r.Left; colIndex <= r.Right; colIndex++)
                    {
                        GridStyleInfo gsi = new GridStyleInfo(this.gridData[rowIndex - r.Top, colIndex - r.Left]);
                        if (this.Model[rowIndex, colIndex] != null)
                        {
                            if (this.gridData[rowIndex - r.Top, colIndex - r.Left] != null)
                            {
                                if (this.Model[rowIndex, colIndex].CellType != "Static" &&
                                    this.Model[rowIndex, colIndex].CellType != "ReadOnly" &&
                                    this.Model[rowIndex, colIndex].CellType != "ExpandCollapseCell")
                                {
                                    GridStyleInfo style = this.Model[rowIndex, colIndex];
                                    GridDataControl datagrid = null;
                                    if ((this.Model.Options.CopyPasteOption & CopyPaste.IncludeStyle) ==
                                        CopyPaste.IncludeStyle)
                                    {
                                        this.Model[rowIndex, colIndex] = gsi;
                                    }
                                    // Since the cell valu of the Image Cells can't be stored as String Type and it should be as BitamapImage Type We avoid this for ImageCells.
                                    if (gsi.CellType != "ImageCell")
                                        foreach (GridControlBase grid in Model.Views)
                                        {
                                            bool state = false;
                                            string text = gsi.CellValue.ToString();
                                            if (grid != null)
                                            {
                                                datagrid = grid.FindParentElementOfType<GridDataControl>();
                                                if (datagrid != null)
                                                {
                                                    text = gsi.CellValue.ToString();
                                                    state = this.Model.TextDataExchange.PasteTextRowCol(rowIndex, colIndex, text);
                                                }
                                                else
                                                {
                                                    state = style.CellType == "DropDownList" || style.CellType == "ComboBox"
                                                                ? style.ApplyFormattedText(text)
                                                                : style.ApplyText(text);
                                                }
                                                if (state)
                                                    this.Model.InvalidateCell(new RowColumnIndex(rowIndex, colIndex));
                                            }
                                        }
                                }
                            }
                        }                 
                    }
                }
#endif

                if ((this.Model.Options.CopyPasteOption & CopyPaste.IncludeStyle) == CopyPaste.IncludeStyle)
                {
                    foreach (GridRangeInfo range in rangeList)
                    {
                        int coverTop = range.Top;
                        int coverLeft = range.Left;

                        if (coveredRanges != null)
                        {
                            foreach (GridRangeInfo rc in coveredRanges)
                            {
                                GridRangeInfo rcOffset = rc.OffsetRange(coverTop, coverLeft);
                                if (!Model.CoveredRanges.Ranges.AnyRangeIntersects(rcOffset))
                                {

                                    if (rcOffset.Bottom > this.Model.RowCount - 1 || rcOffset.Right > this.Model.ColumnCount - 1)
                                    {
                                        continue;
                                    }
                                    this.Model.CoveredCells.Add(new CoveredCellInfo(rcOffset.Top, rcOffset.Left, rcOffset.Bottom, rcOffset.Right));
                                    this.Model.CoveredCells.Ranges.Add(rcOffset);
                                }
                            }
                        }
                    }
                }

                Model.Selections.Clear();
                Model.Selections.Add(GridRangeInfo.Cells(top, left, bottom, right));
                this.Model.InvalidateCell(r.ToCellSpan());
                this.Model.InvalidateVisual(true);

                if (CutFlag || CutCell)
                {
                    gridData = null;
                    CutFlag = false;
                    try
                    {
#if (!SILVERLIGHT && !WinRT)
                        Clipboard.SetDataObject(string.Empty);
#endif
#if Silverlight
                        Clipboard.SetText(string.Empty);
#endif
#if WinRT
                        Clipboard.SetContent(null);
#endif
                        return false;
                    }
                    finally { }
                }
            }

            return false;
        }
    }
}
