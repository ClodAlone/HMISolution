/**
* DevExtreme (pdf_exporter.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { DxPromise } from './core/utils/deferred';
import dxDataGrid, { Column } from './ui/data_grid';
import { ExportLoadPanel } from './exporter/export_load_panel';
import dxGantt from './ui/gantt';

/**
 * A DataGrid cell to be exported to PDF.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface PdfDataGridCell {
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

/**
 * Properties that can be passed as a parameter to the exportDataGrid(options) method from the pdfExporter module.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface PdfExportDataGridProps {
    /**
     * A jsPDF instance. This setting is required.
     */
    jsPDFDocument?: object;
    /**
     * Options of the generated PDF table. Refer to the jsPDF-autoTable plugin documentation to see the full list of available customizations.
     */
    autoTableOptions?: object;
    /**
     * A DataGrid instance. This setting is required.
     */
    component?: dxDataGrid;
    /**
     * Specifies whether or not to export only selected rows.
     */
    selectedRowsOnly?: boolean;
    /**
     * Specifies whether columns in the PDF file should have the same width as their source UI component&apos;s columns.
     */
    keepColumnWidths?: boolean;
    /**
     * Customizes a cell in PDF after creation.
     */
    customizeCell?: ((options: { gridCell?: PdfDataGridCell; pdfCell?: any }) => void);
    /**
     * Configures the load panel.
     */
    loadPanel?: ExportLoadPanel;
}

/**
 * Exports grid data to a PDF file.
 */
export function exportDataGrid(options: PdfExportDataGridProps): DxPromise<void>;

/**
  * Properties that you can pass as a parameter to the exportGantt(options) method from the pdfExporter module.
  * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
  */
 export interface PdfExportGanttProps {
  /**
   * A function that creates a PDF document.
   */
  createDocumentMethod?: ((options: any) => object);
  /**
   * A jsPDF instance. This setting is required.
   */
  jsPDFDocument?: object;
  /**
   * A Gantt instance. This setting is required.
   */
  component?: dxGantt;
  /**
   * Specifies the document size.
   */
  format?: string | object;
  /**
   * Specifies whether to use horizontal orientation for the document.
   */
  landscape?: boolean;
  /**
   * Specifies the file name.
   */
  fileName?: string;
  /**
   * Specifies the outer indents of the exported area.
   */
  margins?: object;
   /**
   * Specifies which part of the component to export (chart area, tree list area, or the entire component).
   */
  exportMode?: 'all' | 'treeList' | 'chart';
   /**
   * Specifies the date range for which to export tasks.
   */
  dateRange?: 'all' | 'visible' | object;
}

/**
  * Exports Gantt data to a PDF file.
  */
 export function exportGantt(options: PdfExportGanttProps): DxPromise<any>;
