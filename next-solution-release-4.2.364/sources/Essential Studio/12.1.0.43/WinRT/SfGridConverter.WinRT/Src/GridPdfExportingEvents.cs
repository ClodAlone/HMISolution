#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.UI.Xaml.Grid.Converter
{
    public delegate void GridPdfExportingEventhandler(object sender, GridPdfExportingEventArgs e);

    public delegate void GridCellPdfExportingEventhandler(object sender, GridCellPdfExportingEventArgs e);

    public delegate void ChildGridPdfExportingEventhandler(object sender, ChildGridPdfExportingEventArgs e);

    public delegate void PdfHeaderFooterEventHandler(object sender, PdfHeaderFooterEventArgs e);

    public sealed class GridPdfExportingEventArgs : GridEventArgs
    {
        public GridPdfExportingEventArgs(PdfGrid pdfGrid, object dataGrid, int level)
            : base(dataGrid)
        {
            this.PdfGrid = pdfGrid;
            this.Level = level;
        }

        public PdfGrid PdfGrid { get; internal set; }

        public ExportCellType CellType { get; internal set; }

        public PdfGridCellStyle CellStyle { get; set; }

        public int Level { get; private set; }

        internal PdfGridCellStyle HeaderCellStyle;
        internal PdfGridCellStyle RecordCellStyle;
        internal PdfGridCellStyle IndentCellStyle;
        internal PdfGridCellStyle CaptionCellStyle;
        internal PdfGridCellStyle GroupsummaryCellStyle;
        internal PdfGridCellStyle TablesummaryCellStyle;
        internal PdfGridCellStyle TopTablesummaryCellStyle;
        internal PdfGridCellStyle StackedHeaderCellStyle;
    }

    public sealed class GridCellPdfExportingEventArgs : GridHandledEventArgs
    {
        public GridCellPdfExportingEventArgs(SfDataGrid dataGrid, PdfGridCell pdfGridCell, ExportCellType exportCellType, object cellValue,
            object exportNodeEntry, ViewDefinition gridViewDefintion, int level, string columnName, IPropertyAccessProvider propertyAccessProvider)
            : base(dataGrid)
        {
            this.PdfGridCell = pdfGridCell;
            this.CellType = exportCellType;
            this.CellValue = cellValue;
            this.NodeEntry = exportNodeEntry;
            this.ColumnName = columnName;
            this.PropertyAccessProvider = propertyAccessProvider;
        }

        public PdfGridCell PdfGridCell { get; private set; }

        public ExportCellType CellType { get; private set; }

        public object CellValue { get; set; }

        public object NodeEntry { get; private set; }

        public string ColumnName { get; private set; }

        public ViewDefinition GridViewDefinition { get; private set; }

        public int Level { get; private set; }

        public IPropertyAccessProvider PropertyAccessProvider { get; private set; }
    }

    public sealed class ChildGridPdfExportingEventArgs : GridCancelEventArgs
    {
        public ChildGridPdfExportingEventArgs(object nodeEntry, string relationalColumn, int childLevel, SfDataGrid dataGrid, PdfExportingOptions pdfExportingOptions)
            : base(dataGrid)
        {
            this.NodeEntry = nodeEntry;
            this.RelationalColumn = relationalColumn;
            this.Level = childLevel;
            this.PdfExportingOptions = pdfExportingOptions;
        }

        public object NodeEntry { get; private set; }

        public string RelationalColumn { get; private set; }

        public int Level { get; private set; }

        public PdfExportingOptions PdfExportingOptions { get; set; }
    }

    public sealed class PdfHeaderFooterEventArgs : GridEventArgs
    {
        public PdfHeaderFooterEventArgs(SfDataGrid dataGrid, PdfPage pdfPage, PdfDocumentTemplate pdfDocumentTemplate)
            : base(dataGrid)
        {
            this.PdfPage = pdfPage;
            this.PdfDocumentTemplate = pdfDocumentTemplate;
        }

        public readonly PdfPage PdfPage;

        //public PdfPageTemplateElement HeaderTemplateElement { get; set; }

        //public PdfPageTemplateElement FooterTemplateElement { get; set; }

        public PdfDocumentTemplate PdfDocumentTemplate { get; private set; }
    }
}
