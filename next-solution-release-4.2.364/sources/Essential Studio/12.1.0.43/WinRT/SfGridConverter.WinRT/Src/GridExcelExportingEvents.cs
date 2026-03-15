#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Collections.ComponentModel;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


#if WinRT
using Windows.UI;
using Windows.UI.Xaml.Media;
using Syncfusion.Data;
#else
using System.Windows.Media;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid.Converter
{
    public delegate void GridExcelExportingEventhandler(object sender,GridExcelExportingEventArgs e);

    public delegate void GridCellExcelExportingEventHandler(object sender,GridCellExcelExportingEventArgs e);

    public delegate void GridChildExportingEventHandler(object sender,GridChildExportingEventArgs e);

    public sealed class GridCellExcelExportingEventArgs : GridHandledEventArgs
    {        
        public GridCellExcelExportingEventArgs(IRange exportRange, ExportCellType exportCellType, object cellValue, object exportNodeEntry, string columnName,
            ViewDefinition viewDefinition, int childLevel, object dataGrid, ExportMode exportMode, IPropertyAccessProvider propertyAccessProvider)
            : base(dataGrid)
        {
            this.CellType = exportCellType;
            this.Range = exportRange;
            this.CellValue = cellValue;
            this.NodeEntry = exportNodeEntry;
            this.ColumnName = columnName;
            this.GridViewDefinition = viewDefinition;
            this.Level = childLevel;
            this.PropertyAccessProvider = propertyAccessProvider;
            this.ExportMode = exportMode;
        }
        
        public ExportCellType CellType { get; private set; }

        public IRange Range { get; set; }

        public object CellValue { get; private set; }

        public object NodeEntry { get; private set; }

        public string ColumnName { get; private set; }

        public int Level { get; private set; }

        public ViewDefinition GridViewDefinition { get; private set; }

        public IPropertyAccessProvider PropertyAccessProvider { get; private set; }

        public ExportMode ExportMode { get; set; }
    }

    public sealed class GridExcelExportingEventArgs : GridHandledEventArgs
    {
        public GridExcelExportingEventArgs(IWorksheet worksheet, ExportCellType exportCellType, ExportCellStyle exportCellStyle,object dataGrid):base(dataGrid)
        {
            this.WorkSheet = worksheet;
            this.CellType = exportCellType;
            this.CellStyle = exportCellStyle;
        }

        public GridExcelExportingEventArgs(IWorksheet worksheet, ExportCellType exportCellType, ExportCellStyle exportCellStyle,int childLevel, object dataGrid)
            : base(dataGrid)
        {
            this.WorkSheet = worksheet;
            this.CellType = exportCellType;
            this.CellStyle = exportCellStyle;
            this.Level = childLevel;
        }

        public IWorksheet WorkSheet { get; set; }

        public ExportCellType CellType { get; set; }

        public ExportCellStyle CellStyle { get; set; }

        public int Level { get; private set; }

    }

    public sealed class GridChildExportingEventArgs : GridCancelEventArgs
    {
        public GridChildExportingEventArgs(object nodeEntry, string relationalColumn,int childLevel,List<string> excludeColumns, SfDataGrid dataGrid): base(dataGrid)
        {
            this.NodeEntry = nodeEntry;
            this.RelationalColumn = relationalColumn;            
            this.Level = childLevel;
            this.ExcludeColumns = excludeColumns;
        }

        public List<string> ExcludeColumns { get; private set; }
        public object NodeEntry { get; private set; }

        public string RelationalColumn { get; private set; }

        public int Level { get; private set; }
    }
    
}
