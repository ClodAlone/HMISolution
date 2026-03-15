/**
* DevExtreme (excel_exporter.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { DxPromise } from './core/utils/deferred';
import dxDataGrid, { Column } from './ui/data_grid';
import dxPivotGrid, { Cell } from './ui/pivot_grid';
import { ExportLoadPanel } from './exporter/export_load_panel';

export type DataGridCell = ExcelDataGridCell;

/**
 * @deprecated Use DataGridCell instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ExcelDataGridCell {
    /**
     * The configuration of the cell&apos;s column.
     */
    column?: Column;
    /**
     * The data object of the cell&apos;s row.
     */
    data?: any;
    /**
     * The group index of the cell&apos;s row. Available when the rowType is &apos;group&apos;.
     */
    groupIndex?: number;
    /**
     * Information about group summary items the cell represents.
     */
    groupSummaryItems?: Array<{
      /**
       * The group summary item&apos;s identifier.
       */
      name?: string;
      /**
       * The group summary item&apos;s raw value.
       */
      value?: any;
    }>;
    /**
     * The type of the cell&apos;s row.
     */
    rowType?: string;
    /**
     * The identifier of the total summary item that the cell represents.
     */
    totalSummaryItemName?: string;
    /**
     * The cell&apos;s raw value.
     */
    value?: any;
}

export type PivotGridCell = ExcelPivotGridCell;

/**
 * @deprecated Use PivotGridCell instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ExcelPivotGridCell extends Cell {
    /**
     * The area to which the cell belongs.
     */
    area?: string;
    /**
     * A zero-based index that indicates the position of the cell&apos;s row.
     */
    rowIndex?: number;
    /**
     * A zero-based index that indicates the position of the cell&apos;s column.
     */
    columnIndex?: number;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface CellAddress {
    /**
     * The index of a row that contains the cell.
     */
    row?: number;
    /**
     * The index of a column that contains the cell.
     */
    column?: number;
}

/**
 * The coordinates of the exported DataGrid in the Excel file.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface CellRange {
    /**
     * Coordinates of the top left cell.
     */
    from?: CellAddress;
    /**
     * Coordinates of the bottom right cell.
     */
    to?: CellAddress;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ExcelExportBaseProps {
    /**
     * An Excel worksheet to which the grid should be exported.
     */
    worksheet?: object;
    /**
     * A cell used as a start position for export.
     */
    topLeftCell?: CellAddress | string;
    /**
     * Specifies whether Excel columns should have the same width as their source UI component&apos;s columns.
     */
    keepColumnWidths?: boolean;
    /**
     * Configures the load panel.
     */
    loadPanel?: ExportLoadPanel;
}

/**
 * Properties that can be passed to the exportDataGrid(options) method from the excelExporter module.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ExcelExportDataGridProps extends ExcelExportBaseProps {
    /**
     * A DataGrid instance. This setting is required.
     */
    component?: dxDataGrid;
    /**
     * Specifies whether to export only selected rows.
     */
    selectedRowsOnly?: boolean;
    /**
     * Specifies whether to enable Excel filtering in the document.
     */
    autoFilterEnabled?: boolean;
    /**
     * Customizes an Excel cell after creation.
     */
    customizeCell?: ((options: { gridCell?: DataGridCell; excelCell?: any }) => void);
}

/**
 * Properties that can be passed to the exportPivotGrid(options) method from the excelExporter module.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ExcelExportPivotGridProps extends ExcelExportBaseProps {
    /**
     * A PivotGrid instance. This setting is required.
     */
    component?: dxPivotGrid;
    /**
     * Specifies whether to merge neighbouring cells in the row field if they have the same values.
     */
    mergeRowFieldValues?: boolean;
    /**
     * Specifies whether to merge neighbouring cells in the column field if they have the same values.
     */
    mergeColumnFieldValues?: boolean;
    /**
     * Customizes an Excel cell after creation.
     */
    customizeCell?: ((options: { pivotCell?: PivotGridCell; excelCell?: any }) => void);
}

/**
 * Exports grid data to Excel.
 */
export function exportDataGrid(options: ExcelExportDataGridProps): DxPromise<CellRange>;

/**
 * Exports pivot grid data to Excel.
 */
export function exportPivotGrid(options: ExcelExportPivotGridProps): DxPromise<CellRange>;
