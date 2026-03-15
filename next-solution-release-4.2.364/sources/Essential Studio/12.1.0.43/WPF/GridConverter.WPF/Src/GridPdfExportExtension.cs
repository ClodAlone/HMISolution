#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Drawing;

namespace Syncfusion.Windows.Controls.Grid.Converter
{
    using System;
    using Syncfusion.Pdf;
    using Syncfusion.Pdf.Grid;
    using Syncfusion.Pdf.Graphics;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    /// <summary>
    /// Event Data Class of <see cref="GridCellExportToPdfHandler"/>
    /// </summary>
    public class ExportingToPdfEventArgs : EventArgs
    {
        public ExportingToPdfEventArgs(GridStyleInfo gridStyle, PdfGridCell pdfGridCell)
        {
            GridStyle = gridStyle;
            PdfGridCell = pdfGridCell;
        }

        /// <summary>
        /// Bool variable used to determine whether the user handled the exporting or not.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// GridStyleInfo holds the style info of the exporting Grid Cell.
        /// </summary>
        public GridStyleInfo GridStyle
        {
            get;
            internal set;
        }

        /// <summary>
        /// Currently Exporting PdfGridCell
        /// </summary>
        public PdfGridCell PdfGridCell
        {
            get;
            set;
        }
    }

    public delegate void GridCellExportToPdfHandler(object sender, ExportingToPdfEventArgs e);

    public class PdfHeaderFooterEventArgs : EventArgs
    {
        PdfPageTemplateElement _headerFooterTemplate;

        public PdfPageTemplateElement HeaderFooterTemplate
        {
            get
            {
                return _headerFooterTemplate;
            }

            set
            {
                _headerFooterTemplate = value;
            }
        }

        public PdfHeaderFooterEventArgs(PdfPageTemplateElement headerFooterTemplate)
        {
            _headerFooterTemplate = headerFooterTemplate;
        }
    }

    public delegate void DrawPdfHeaderFooterEventHandler(object sender, PdfHeaderFooterEventArgs e);

    /// <summary>
    /// Class which exports the GridModel to PDF
    /// </summary>
    public static partial class GridPdfExportExtension 
    {               
        #region Public Methods

        public static event DrawPdfHeaderFooterEventHandler DrawPdfHeader;
        public static event DrawPdfHeaderFooterEventHandler DrawPdfFooter;

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>
        /// <returns>PdfDocument</returns>
        public static PdfDocument ExportToPdfGridDocument(this GridModel gridModel, GridRangeInfo range)
        {
            return ExportToPdfGridDocument(gridModel, range, null, DrawPdfHeader, DrawPdfFooter);
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>
        /// <param name="pdfExportHandler">Delegate Event handler which fires for every cell before exporting.</param>
        /// <returns>PdfDocument</returns>
        public static PdfDocument ExportToPdfGridDocument(this GridModel gridModel, GridRangeInfo range, GridCellExportToPdfHandler pdfExportHandler)
        {
            return ExportToPdfGridDocument(gridModel, range, pdfExportHandler, DrawPdfHeader, DrawPdfFooter);
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>
        /// <param name="drawPdfHeader">Event Handler which fires before drawing the header. </param>
        /// <param name="drawPdfFooter">Event Handler which fires before drawing the footer.</param>
        /// <returns>PdfDocument</returns>
        public static PdfDocument ExportToPdfGridDocument(this GridModel gridModel, GridRangeInfo range, DrawPdfHeaderFooterEventHandler drawPdfHeader, DrawPdfHeaderFooterEventHandler drawPdfFooter)
        {
            return ExportToPdfGridDocument(gridModel, range, null, drawPdfHeader, drawPdfFooter);
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>
        /// <param name="pdfExportHandler">Delegate Event handler which fires for every cell before exporting.</param>
        /// <param name="drawPdfHeader">Event Handler which fires before drawing the header. </param>
        /// <param name="drawPdfFooter">Event Handler which fires before drawing the footer.</param>
        /// <returns>PdfDocument</returns>
        public static PdfDocument ExportToPdfGridDocument(this GridModel gridModel, GridRangeInfo range,
                                                          GridCellExportToPdfHandler pdfExportHandler,
                                                          DrawPdfHeaderFooterEventHandler drawPdfHeader,
                                                          DrawPdfHeaderFooterEventHandler drawPdfFooter)
        {
            var pdfDocument = new PdfDocument();
            var pages = pdfDocument.Pages.Add();
            var exportOptions = new ExportToPdfOptions {PdfExportHandler = pdfExportHandler};
            var pdfGrid = ExportToPdf(gridModel, range, exportOptions);
            var format = new PdfGridLayoutFormat
                {
                    Layout = PdfLayoutType.Paginate,
                    Break = PdfLayoutBreakType.FitElement
                };
            OnDrawHeader(gridModel, ref pdfDocument, pages, drawPdfHeader);
            OnDrawFooter(gridModel, ref pdfDocument, pages, drawPdfFooter);
            pdfGrid.Draw(pages, new PointF(), format);
            return pdfDocument;
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>
        /// <param name="fileName"> Name of the Pdf file to be saved after exporting.</param>
        public static void ExportToPdfGridDocument(this GridModel gridModel, GridRangeInfo range, string fileName)
        {
            var document = ExportToPdfGridDocument(gridModel, range);
            var stream = new FileStream(fileName, FileMode.OpenOrCreate);
            document.Save(stream);
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>
        /// <param name="pdfExportHandler">Delegate Event handler which fires for every cell before exporting.</param>
        /// <param name="fileName"> Name of the Pdf file to be saved after exporting.</param>
        public static void ExportToPdfGridDocument(this GridModel gridModel, GridRangeInfo range, GridCellExportToPdfHandler pdfExportHandler, string fileName)
        {
            var document = ExportToPdfGridDocument(gridModel, range, pdfExportHandler);
            var stream = new FileStream(fileName, FileMode.OpenOrCreate);
            document.Save(stream);
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>
        /// <param name="drawPdfHeader">Event Handler which fires before drawing the header. </param>
        /// <param name="drawPdfFooter">Event Handler which fires before drawing the footer.</param>
        /// <param name="fileName"> Name of the Pdf file to be saved after exporting.</param>
        public static void ExportToPdfGridDocument(this GridModel gridModel, GridRangeInfo range, DrawPdfHeaderFooterEventHandler drawPdfHeader, DrawPdfHeaderFooterEventHandler drawPdfFooter, string fileName)
        {
            var document = ExportToPdfGridDocument(gridModel, range, drawPdfHeader, drawPdfFooter);
            var stream = new FileStream(fileName, FileMode.OpenOrCreate);
            document.Save(stream);
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>
        /// <param name="pdfExportHandler">Delegate Event handler which fires for every cell before exporting.</param>
        /// <param name="drawPdfHeader">Event Handler which fires before drawing the header. </param>
        /// <param name="drawPdfFooter">Event Handler which fires before drawing the footer.</param>
        /// <param name="fileName"> Name of the Pdf file to be saved after exporting.</param>
        public static void ExportToPdfGridDocument(this GridModel gridModel, GridRangeInfo range, GridCellExportToPdfHandler pdfExportHandler, DrawPdfHeaderFooterEventHandler drawPdfHeader, DrawPdfHeaderFooterEventHandler drawPdfFooter, string fileName)
        {
            var document = ExportToPdfGridDocument(gridModel, range, pdfExportHandler, drawPdfHeader, drawPdfFooter);
            var stream = new FileStream(fileName, FileMode.OpenOrCreate);
            document.Save(stream);
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>
        /// <returns>PdfGrid</returns>
        public static PdfGrid ExportToPdfGrid(this GridModel gridModel, GridRangeInfo range)
        {
            return ExportToPdfGrid(gridModel, range, null);
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>
        /// <param name="pdfExportHandler">Delegate Event handler which fires for every cell before exporting.</param>
        /// <returns>PdfGrid</returns>
        public static PdfGrid ExportToPdfGrid(this GridModel gridModel, GridRangeInfo range, GridCellExportToPdfHandler pdfExportHandler)
        {
            var exportOptions = new ExportToPdfOptions {PdfExportHandler = pdfExportHandler};
            return ExportToPdf(gridModel, range, exportOptions);
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>        
        /// <param name="exportingOptions"><see cref="ExportToPdfOptions"/> holds the Pdf Exporting Options</param>
        /// <returns>PdfGrid</returns>
        public static PdfGrid ExportToPdf(this GridModel gridModel, GridRangeInfo range, ExportToPdfOptions exportingOptions)
        {
            if (range.IsEmpty || range.IsTable)
            {
                exportingOptions.StartRowIndex = gridModel.HeaderRows;
                exportingOptions.StartColumnIndex = 0;
                exportingOptions.EndRowIndex = gridModel.RowCount - 1;
                
                if (gridModel is GridDataTableModel && (gridModel as GridDataTableModel).TableProperties.Relations.Count > 0) 
                {
                    var gridDataModel = gridModel as GridDataTableModel;
                    var expanderColumnIndex = 0;
                    if (!exportingOptions.ExportNestedGrid)
                    {
                        expanderColumnIndex = gridDataModel.TableProperties.ShowRowHeader ? expanderColumnIndex + 1 : expanderColumnIndex;
                        expanderColumnIndex = gridDataModel.View.GroupDescriptions.Count > 0 ? expanderColumnIndex + gridDataModel.View.GroupDescriptions.Count : expanderColumnIndex;
                        exportingOptions.ExcludeColumnsIndexList.Add(expanderColumnIndex);
                    }
                    exportingOptions.EndColumnIndex = gridModel.ColumnCount - 2;  
                }                                                 
                else
                    exportingOptions.EndColumnIndex = gridModel.ColumnCount - 1;
            }
            else
            {
                exportingOptions.StartRowIndex = range.Top < gridModel.HeaderRows ? gridModel.HeaderRows : range.Top;
                exportingOptions.StartColumnIndex = range.Left;
                exportingOptions.EndRowIndex = range.Bottom;
                exportingOptions.EndColumnIndex = range.Right;
                exportingOptions.ExportNestedGrid = false;
            }       
            return CreatePdfGrid(gridModel, exportingOptions);
        }

        /// <summary>
        /// Exports the GridModel to Pdf.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="range"><see cref="GridRangeInfo"/> The Range to be exported to Pdf</param>        
        /// <param name="exportingOptions"><see cref="ExportToPdfOptions"/> holds the Pdf Exporting Options</param>
        /// <returns>PdfGridDocument</returns>
        public static PdfDocument ExportToPdfDocument(this GridModel gridModel, GridRangeInfo range, ExportToPdfOptions exportingOptions)
        {
            var pdfDocument = new PdfDocument();
            var pages = pdfDocument.Pages.Add();                        
            var pdfGrid = ExportToPdf(gridModel, range, exportingOptions);
            var format = new PdfGridLayoutFormat
                {
                    Layout = PdfLayoutType.Paginate,
                    Break = PdfLayoutBreakType.FitElement
                };
            pdfGrid.Draw(pages, new PointF(), format);
            return pdfDocument;
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Creates the PdfGrid and Export the data from Grid To PdfGrid
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="exportingOptions"><see cref="ExportToPdfOptions"/> holds the Pdf Exporting Options</param>
        /// <returns>PdfGrid</returns>
        private static PdfGrid CreatePdfGrid(GridModel gridModel, ExportToPdfOptions exportingOptions)
        {
            var pdfGrid = new PdfGrid();
#if !SILVERLIGHT
            var univconv = new PdfUnitConvertor();
#endif            
            pdfGrid.Style.AllowHorizontalOverflow = true;
            pdfGrid.Style.HorizontalOverflowType = PdfHorizontalOverflowType.NextPage;

            if (!exportingOptions.ExportStyles)
                pdfGrid.Style.CellPadding = new PdfPaddings(1, 1, 1, 1);

            if (gridModel is GridDataTableModel)
            {
               var gridDataTableModel = gridModel as GridDataTableModel;
               for (var i = exportingOptions.StartColumnIndex; i <= exportingOptions.EndColumnIndex; i++)
               {
                   var visibleColumnIndex = gridDataTableModel.ResolvePositionToVisibleColumnIndex(i);
                   if (visibleColumnIndex >= 0 && exportingOptions.ExcludeColumns.Contains(gridDataTableModel.TableProperties.VisibleColumns[visibleColumnIndex].MappingName))
                       exportingOptions.ExcludeColumnsIndexList.Add(i);
                   else
                       pdfGrid.Columns.Add();
               }
            }
            else if (gridModel is GridTreeModel)
            {
                var treeModel = gridModel as GridTreeModel;
                GridTreeControlImpl treeControl = null;
                foreach (GridControlBase grid in treeModel.Views)
                {
                    treeControl = grid as GridTreeControlImpl;
                    if (treeControl != null)
                        break;
                }
                for (var i = exportingOptions.StartColumnIndex; i <= exportingOptions.EndColumnIndex; i++)
                {
                    var visibleColumnIndex = treeControl.ResolveIndexToColumnIndex(i);
                    
                    if (visibleColumnIndex >= 0 && exportingOptions.ExcludeColumns.Contains(treeControl.Columns[visibleColumnIndex].MappingName))
                        exportingOptions.ExcludeColumnsIndexList.Add(i);
                    else
                        pdfGrid.Columns.Add();
                }
            }
            else
                pdfGrid.Columns.Add((exportingOptions.EndColumnIndex - exportingOptions.StartColumnIndex) + 1);

            if(exportingOptions.RepeatHeaders)
                pdfGrid.RepeatHeader = true;
                        
            var pdfColIndex = 0;

            if (!exportingOptions.AutoColumnWidth)
            {
                for (var coll = exportingOptions.StartColumnIndex; coll <= exportingOptions.EndColumnIndex; coll++)
                {
                    float columnWidth = 0;
#if !SILVERLIGHT
                    columnWidth = univconv.ConvertFromPixels((float)gridModel.ColumnWidths[coll], PdfGraphicsUnit.Point);
#else
                    columnWidth = (float)gridModel.ColumnWidths[pdfColIndex] / 2;
#endif
                    if (gridModel is GridDataTableModel)
                    {
                        if (exportingOptions.ExcludeColumnsIndexList.Contains(coll))
                            continue;

                        if (columnWidth > 0)
                            pdfGrid.Columns[pdfColIndex].Width = columnWidth;
                    }
                    else if( gridModel is GridTreeModel)
                    {
                        if (exportingOptions.ExcludeColumnsIndexList.Contains(coll))
                            continue;
                        if (columnWidth > 0)

                            pdfGrid.Columns[pdfColIndex].Width = columnWidth;
                    }
                    else
                        pdfGrid.Columns[pdfColIndex].Width = columnWidth;

                    pdfColIndex++;
                }
            }

            if (gridModel.HeaderRows > 0 && exportingOptions.IncludeHeaders)
            {
                CreateHeaderRows(pdfGrid, gridModel, exportingOptions);
            }

            ExportEmptyRows(gridModel, pdfGrid, exportingOptions);
            
            // Add Rows to the grid.
            for (var rowIndex = exportingOptions.StartRowIndex; rowIndex <= exportingOptions.EndRowIndex; rowIndex++)
            {
                PdfGridRow pdfGridRow;
                if (gridModel.RowHeights[rowIndex] >0)
                {
                    pdfGridRow = pdfGrid.Rows.Add();                    
#if!SILVERLIGHT
                    pdfGridRow.Height = univconv.ConvertFromPixels((float)gridModel.RowHeights[rowIndex], PdfGraphicsUnit.Point);
#else
                    pdfGridRow.Height = (float)gridModel.RowHeights[rowIndex] / 2;
#endif
                }
                else
                    continue;

                pdfColIndex = 0;
                for (var columnIndex = exportingOptions.StartColumnIndex; columnIndex <= exportingOptions.EndColumnIndex; columnIndex++)
                {
                    if (exportingOptions.ExcludeColumnsIndexList.Contains(columnIndex))
                        continue;

                    if (gridModel[rowIndex, columnIndex].CellType == "NestedGrid")
                    {
                        if (!exportingOptions.ExportNestedGrid)
                        {
                            pdfGrid.Rows.RemoveAt(pdfGrid.Rows.Count - 1);                            
                            break;
                        }

                        PdfGrid childPdfGrid = null;
                        var childModel = gridModel[rowIndex, columnIndex].CellValue as GridDataChildTableModel;
                        var childExportingOptions= new ExportToPdfOptions
                            {
                                PdfExportHandler = exportingOptions.PdfExportHandler,
                                ExportStyles = exportingOptions.ExportStyles,
                                StartColumnIndex = 0,
                                EndColumnIndex = childModel.ColumnCount - 1
                            };
                        childPdfGrid = CreateChildPdfGrid(childModel, childExportingOptions);                        
#if !SILVERLIGHT
                        pdfGridRow.Cells[pdfColIndex].Style.Borders.All = new PdfPen(new PdfColor(System.Drawing.Color.Transparent));
#else
                        pdfGridRow.Cells[pdfColIndex].Style.Borders.All = new PdfPen(new PdfColor(Colors.Transparent));
#endif
                        pdfGridRow.Cells[pdfColIndex].ColumnSpan = (pdfGrid.Columns.Count - columnIndex);
                        pdfGridRow.Cells[pdfColIndex].Value = childPdfGrid;
                        break;
                    }
                    else
                    {
                        var pdfGridCell = pdfGridRow.Cells[pdfColIndex];
                        GridPdfConverterControl.GridCellToPdf(gridModel, pdfGridCell, rowIndex, columnIndex, exportingOptions);
                    }
                    pdfColIndex++;
                }                
            }
            return pdfGrid;
        }

        /// <summary>
        /// Exports the AddNewRow and FilterBarRow to PdfGrid.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="pdfGrid">PdfGrid</param>
        /// <param name="exportingOptions"><see cref="ExportToPdfOptions"/> holds the Pdf Exporting Options</param>
        private static void ExportEmptyRows(GridModel gridModel,PdfGrid pdfGrid,ExportToPdfOptions exportingOptions)
        {
            if (!(gridModel is GridDataTableModel)) 
                return;

#if !SILVERLIGHT
            var univconv = new PdfUnitConvertor();
#endif
            var gridDataModel = gridModel as GridDataTableModel;

            if (gridDataModel.TableProperties.ShowFilterBar)
            {
                if(exportingOptions.ExportFilterBar)
                {
                    var pdfGridRow=pdfGrid.Rows.Add();
#if!SILVERLIGHT
                    pdfGridRow.Height = univconv.ConvertFromPixels((float)gridModel.RowHeights[exportingOptions.StartRowIndex], PdfGraphicsUnit.Point);
#else
                    pdfGridRow.Height = (float)gridModel.RowHeights[exportingOptions.StartRowIndex] / 2;
#endif
                    var pdfColIndex = 0;
                    for (int columnIndex = exportingOptions.StartColumnIndex; columnIndex <= exportingOptions.EndColumnIndex; columnIndex++)
                    {
                        if (exportingOptions.ExcludeColumnsIndexList.Contains(columnIndex))
                            continue;

                        var pdfGridCell = pdfGridRow.Cells[pdfColIndex];
                        GridPdfConverterControl.GridCellToPdf(gridModel, pdfGridCell, exportingOptions.StartRowIndex, columnIndex, exportingOptions);
                        pdfColIndex++;
                    }
                }
                exportingOptions.StartRowIndex++;
            }

            if (gridDataModel.TableProperties.ShowAddNewRow)
            {
                if (exportingOptions.ExportAddNewRow)
                {
                    var pdfGridRow = pdfGrid.Rows.Add();
#if!SILVERLIGHT
                    pdfGridRow.Height = univconv.ConvertFromPixels((float)gridModel.RowHeights[exportingOptions.StartRowIndex], PdfGraphicsUnit.Point);
#else
                    pdfGridRow.Height = (float)gridModel.RowHeights[exportingOptions.StartRowIndex] / 2;
#endif

                    var pdfColIndex = 0;
                    for (var columnIndex = exportingOptions.StartColumnIndex; columnIndex <= exportingOptions.EndColumnIndex; columnIndex++)
                    {
                        if (exportingOptions.ExcludeColumnsIndexList.Contains(columnIndex))
                            continue;

                        var pdfGridCell = pdfGridRow.Cells[pdfColIndex];
                        GridPdfConverterControl.GridCellToPdf(gridModel, pdfGridCell, exportingOptions.StartRowIndex, columnIndex, exportingOptions);
                        pdfColIndex++;
                    }
                }
                exportingOptions.StartRowIndex++;
            }
        }

        /// <summary>
        /// Exports the Nested Grid to PdfGrid.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="exportingOptions"><see cref="ExportToPdfOptions"/> holds the Pdf Exporting Options</param>
        /// <returns>PdfGrid</returns>
        private static PdfGrid CreateChildPdfGrid(GridModel gridModel, ExportToPdfOptions exportingOptions)
        {
            var pdfGrid = new PdfGrid();
#if !SILVERLIGHT
            var univconv = new PdfUnitConvertor();
#endif            
            pdfGrid.Style.AllowHorizontalOverflow = true;
            pdfGrid.Style.HorizontalOverflowType = PdfHorizontalOverflowType.NextPage;
            pdfGrid.RepeatHeader = false;
            pdfGrid.Columns.Add(gridModel.ColumnCount);                    

            for (var rowIndex = 0; rowIndex <gridModel.RowCount; rowIndex++)
            {
                PdfGridRow pdfGridRow = null;
                if (gridModel.RowHeights[rowIndex] > 0)
                {
                    pdfGridRow = pdfGrid.Rows.Add();
#if!SILVERLIGHT
                    pdfGridRow.Height = univconv.ConvertFromPixels((float)gridModel.RowHeights[rowIndex], PdfGraphicsUnit.Point);
#else
                    pdfGridRow.Height = (float)gridModel.RowHeights[rowIndex] / 2;
#endif
                }
                else
                    continue;

                for (int columnIndex = 0; columnIndex <gridModel.ColumnCount; columnIndex++)
                {
                    if (gridModel[rowIndex, columnIndex].CellType == "NestedGrid")
                    {
                        PdfGrid childPdfGrid = null;
                        var childModel = gridModel[rowIndex, columnIndex].CellValue as GridDataChildTableModel;
                        var childExportingOptions = new ExportToPdfOptions
                            {
                                PdfExportHandler = exportingOptions.PdfExportHandler,
                                ExportStyles = exportingOptions.ExportStyles
                            };
                        childPdfGrid = CreateChildPdfGrid(childModel, childExportingOptions);
#if !SILVERLIGHT
                        pdfGridRow.Cells[columnIndex].Style.Borders.All = new PdfPen(new PdfColor(System.Drawing.Color.Transparent));
#else
                        pdfGridRow.Cells[columnIndex].Style.Borders.All = new PdfPen(new PdfColor(Colors.Transparent));
#endif
                        pdfGridRow.Cells[columnIndex].ColumnSpan = (pdfGrid.Columns.Count - columnIndex);
                        pdfGridRow.Cells[columnIndex].Value = childPdfGrid;
                        continue;
                    }
                    else
                    {
                        var pdfGridCell = pdfGridRow.Cells[columnIndex];
                        GridPdfConverterControl.GridCellToPdf(gridModel, pdfGridCell, rowIndex, columnIndex, exportingOptions);
                    }
                }
            }
            return pdfGrid;
        }

        /// <summary>
        /// Exports the Header rows from Grid to PdfGrid.
        /// </summary>
        /// <param name="pdfGrid"> PdfGrid</param>
        /// <param name="gridModel"><see cref="GridModel"/> Grid Model</param>
        /// <param name="exportingOptions"><see cref="ExportToPdfOptions"/> holds the Pdf Exporting Options</param>
        private static void CreateHeaderRows(PdfGrid pdfGrid, GridModel gridModel, ExportToPdfOptions exportingOptions)
        {
#if !SILVERLIGHT
            var univconv = new PdfUnitConvertor();
#endif
            var headerCount = gridModel.HeaderRows;
            var headerRow = pdfGrid.Headers.Add(headerCount);
            for (var row = 0; row < headerCount; row++)
            {
#if!SILVERLIGHT
                pdfGrid.Headers[row].Height = univconv.ConvertFromPixels((float) gridModel.RowHeights[row],
                                                                         PdfGraphicsUnit.Point);
#else
                pdfGrid.Headers[row].Height = (float)gridModel.RowHeights[row]/2;
#endif
                var pdfColumn = 0;
                for (var column = exportingOptions.StartColumnIndex; column <= exportingOptions.EndColumnIndex; column++)
                {
                    if (exportingOptions.ExcludeColumnsIndexList.Contains(column))
                        continue;
                    var pdfCell = headerRow[row].Cells[pdfColumn];
                    GridPdfConverterControl.GridCellToPdf(gridModel, pdfCell, row, column, exportingOptions);
                    pdfColumn++;
                }
            }
        }

        /// <summary>
        /// Method used to draw the header for PdfGrid
        /// </summary>        
        private static void OnDrawHeader(GridModel gridModel,ref PdfDocument pdfDocument,PdfPage pages, DrawPdfHeaderFooterEventHandler drawPdfHeader)
        {
            pdfDocument.Template.Top = null;
            if (drawPdfHeader == null) 
                return;
            var width = pages.GetClientSize().Width;
            var header = new PdfPageTemplateElement(width, 50);
            var args = new PdfHeaderFooterEventArgs(header);
            drawPdfHeader(gridModel, args);
            pdfDocument.Template.Top = header;
        }

        /// <summary>
        /// Method used to draw the header for PdfGrid
        /// </summary>        
        private static void OnDrawFooter(GridModel gridModel,ref PdfDocument pdfDocument, PdfPage pages, DrawPdfHeaderFooterEventHandler drawPdfFooter)
        {
            pdfDocument.Template.Bottom = null;
            if (drawPdfFooter == null) 
                return;
            var width = pages.GetClientSize().Width;
            var footer = new PdfPageTemplateElement(width, 50);
            var args = new PdfHeaderFooterEventArgs(footer);
            drawPdfFooter(gridModel, args);
            pdfDocument.Template.Bottom = footer;
        }

        #endregion

    }

    /// <summary>
    /// Event Data Class of <see cref="GridCellExportToPdfHandler"/>
    /// </summary>
    public class ExportToPdfOptions
    {
        internal bool _repeatHeaders=true;
        internal GridCellExportToPdfHandler _pdfExportHandler = null;        
        internal bool _autoColumnWidth = false;
        internal bool _exportStyles = true;
        internal bool _includeHeaders = true;
        internal bool _exportNestedGrid = false;
        internal int _endColumnIndex;
        internal int _startColumnIndex;
        internal int _endRowIndex;
        internal int _startRowIndex;
        public List<string> ExcludeColumns = new List<string>();
        internal List<int> ExcludeColumnsIndexList = new List<int>();
        internal bool _exportAddNewRow = false;
        internal bool _exportFilterBar = false;

        public ExportToPdfOptions()
        { 
        }

        /// <summary>
        /// Determines whether the headers shoule be exported to all pdf pages or not.
        /// </summary>
        public bool RepeatHeaders
        {
            set { _repeatHeaders = value; }
            get { return _repeatHeaders; }
        }

        /// <summary>
        /// Determines whether the headers should be exported or not.
        /// </summary>
        public bool IncludeHeaders
        {
            set { _includeHeaders = value; }
            get { return _includeHeaders; }
        }

        /// <summary>
        /// Gets or Sets the delegate handler of type GridCellExportToPdfHandler.
        /// </summary>
        public GridCellExportToPdfHandler PdfExportHandler
        {
            set { _pdfExportHandler = value; }
            get { return _pdfExportHandler; }
        }

        /// <summary>
        /// Determines whether the column widths should copied from Grid or Automatically set by the PdfGrid based on the cell Value.
        /// </summary>
        public bool AutoColumnWidth
        {
            set { _autoColumnWidth = value; }
            get { return _autoColumnWidth; }
        }

        /// <summary>
        /// Determines whether the styles should be exported or not.
        /// </summary>
        public bool ExportStyles
        {
            set { _exportStyles = value; }
            get { return _exportStyles; }
        }

        /// <summary>
        /// Determines whether the NestedGrid should be exported or Not.
        /// </summary>
        public bool ExportNestedGrid
        {
            set { _exportNestedGrid = value; }
            get { return _exportNestedGrid; }
        }

        /// <summary>
        /// Determines whether the method should export AddNewRow or Not, Applicable for GridDataControl Only.
        /// </summary>
        public bool ExportAddNewRow
        {
            set { _exportAddNewRow = value; }
            get { return _exportAddNewRow; }
        }

        /// <summary>
        /// Determines whether the method should export FiletrBar or Not, Applicable for GridDataControl Only.
        /// </summary>
        public bool ExportFilterBar
        {
            set { _exportFilterBar = value; }
            get { return _exportFilterBar; }
        }

        internal int StartColumnIndex
        {
            set { _startColumnIndex = value; }
            get { return _startColumnIndex; }
        }

        internal int EndColumnIndex
        {
            set { _endColumnIndex = value; }
            get { return _endColumnIndex; }
        }

        internal int StartRowIndex
        {
            set { _startRowIndex = value; }
            get { return _startRowIndex; }
        }

        internal int EndRowIndex
        {
            set { _endRowIndex = value; }
            get { return _endRowIndex; }
        }
    }
}
