/**
 * DevExtreme (esm/exporter/jspdf/v3/export_data_grid_3.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isDefined
} from "../../../core/utils/type";
import {
    extend
} from "../../../core/utils/extend";
import {
    normalizeRowsInfo
} from "./normalizeOptions";
import {
    initializeCellsWidth,
    applyColSpans,
    applyRowSpans,
    applyBordersConfig,
    calculateHeights,
    calculateCoordinates,
    calculateTableSize,
    resizeFirstColumnByIndentLevel
} from "./row_utils";
import {
    updateRowsAndCellsHeights
} from "./height_updater";
import {
    generateRowsInfo
} from "./rows_generator";
import {
    drawCellsContent,
    drawCellsLines,
    drawGridLines,
    getDocumentStyles,
    setDocumentStyles
} from "./draw_utils";

function _getFullOptions(options) {
    var fullOptions = extend({}, options);
    if (!isDefined(fullOptions.topLeft)) {
        throw "options.topLeft is required"
    }
    if (!isDefined(fullOptions.indent)) {
        fullOptions.indent = 10
    }
    return fullOptions
}

function exportDataGrid(doc, dataGrid, options) {
    options = extend({}, _getFullOptions(options));
    var dataProvider = dataGrid.getDataProvider();
    return new Promise(resolve => {
        dataProvider.ready().done(() => {
            var _options$rowOptions, _options$rowOptions$h;
            var rowsInfo = generateRowsInfo(dataProvider, dataGrid, null === (_options$rowOptions = options.rowOptions) || void 0 === _options$rowOptions ? void 0 : null === (_options$rowOptions$h = _options$rowOptions.headerStyles) || void 0 === _options$rowOptions$h ? void 0 : _options$rowOptions$h.backgroundColor);
            if (options.customizeCell) {
                rowsInfo.forEach(rowInfo => rowInfo.cells.forEach(cellInfo => options.customizeCell(cellInfo)))
            }
            normalizeRowsInfo(rowsInfo);
            initializeCellsWidth(doc, dataProvider, rowsInfo, options);
            resizeFirstColumnByIndentLevel(rowsInfo, options);
            applyColSpans(rowsInfo);
            calculateHeights(doc, rowsInfo, options);
            applyRowSpans(rowsInfo);
            updateRowsAndCellsHeights(doc, rowsInfo);
            calculateCoordinates(doc, rowsInfo, options);
            applyBordersConfig(rowsInfo);
            var pdfCellsInfo = [].concat.apply([], rowsInfo.map(rowInfo => rowInfo.cells.filter(cell => !isDefined(cell.pdfCell.isMerged)).map(cellInfo => _extends({}, cellInfo.pdfCell, {
                gridCell: cellInfo.gridCell,
                pdfRowInfo: cellInfo.pdfRowInfo
            }))));
            var docStyles = getDocumentStyles(doc);
            drawCellsContent(doc, options.customDrawCell, pdfCellsInfo, docStyles);
            drawCellsLines(doc, pdfCellsInfo, docStyles);
            var isDrawTableBorderSpecified = true === options.drawTableBorder;
            var isEmptyPdfCellsInfoSpecified = isDefined(pdfCellsInfo) && 0 === pdfCellsInfo.length;
            if (isDrawTableBorderSpecified || isEmptyPdfCellsInfoSpecified) {
                var tableRect = calculateTableSize(doc, rowsInfo, options);
                drawGridLines(doc, tableRect, docStyles)
            }
            setDocumentStyles(doc, docStyles);
            resolve()
        })
    })
}
export {
    exportDataGrid
};
