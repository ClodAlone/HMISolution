/**
 * DevExtreme (esm/exporter/jspdf/v3/row_utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined
} from "../../../core/utils/type";
import {
    calculateRowHeight
} from "./pdf_utils_v3";
import {
    normalizeBoundaryValue
} from "./normalizeOptions";

function calculateColumnsWidths(doc, dataProvider, topLeft, margin) {
    var _topLeft$x;
    var columnsWidths = dataProvider.getColumnsWidths();
    if (!columnsWidths.length) {
        return []
    }
    var summaryGridWidth = columnsWidths.reduce((accumulator, width) => accumulator + width);
    var normalizedMargin = normalizeBoundaryValue(margin);
    var availablePageWidth = doc.internal.pageSize.getWidth() - (null !== (_topLeft$x = null === topLeft || void 0 === topLeft ? void 0 : topLeft.x) && void 0 !== _topLeft$x ? _topLeft$x : 0) - normalizedMargin.left - normalizedMargin.right;
    var ratio = availablePageWidth >= summaryGridWidth ? 1 : availablePageWidth / summaryGridWidth;
    return columnsWidths.map(width => width * ratio)
}

function initializeCellsWidth(doc, dataProvider, rows, options) {
    var _options$columnWidths;
    var columnWidths = null !== (_options$columnWidths = null === options || void 0 === options ? void 0 : options.columnWidths) && void 0 !== _options$columnWidths ? _options$columnWidths : calculateColumnsWidths(doc, dataProvider, null === options || void 0 === options ? void 0 : options.topLeft, null === options || void 0 === options ? void 0 : options.margin);
    rows.forEach(row => {
        row.cells.forEach((_ref, index) => {
            var {
                gridCell: gridCell,
                pdfCell: pdfCell
            } = _ref;
            pdfCell._rect.w = columnWidths[index]
        })
    })
}

function calculateHeights(doc, rows, options) {
    rows.forEach(row => {
        var pdfCells = row.cells.map(c => c.pdfCell);
        var customerHeight;
        if (options.onRowExporting) {
            var args = {
                rowCells: pdfCells
            };
            options.onRowExporting(args);
            if (isDefined(args.rowHeight)) {
                customerHeight = args.rowHeight
            }
        }
        row.height = isDefined(customerHeight) ? customerHeight : calculateRowHeight(doc, row.cells, pdfCells.map(c => c._rect.w));
        pdfCells.forEach(cell => {
            cell._rect.h = row.height
        })
    })
}

function applyColSpans(rows) {
    for (var rowIndex = 0; rowIndex < rows.length; rowIndex++) {
        var row = rows[rowIndex];
        for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
            var cell = row.cells[cellIndex];
            if (isDefined(cell.colSpan) && !isDefined(cell.pdfCell.isMerged)) {
                for (var spanIndex = 1; spanIndex <= cell.colSpan; spanIndex++) {
                    var mergedCell = rows[rowIndex].cells[cellIndex + spanIndex];
                    cell.pdfCell._rect.w += mergedCell.pdfCell._rect.w;
                    mergedCell.pdfCell._rect.w = 0;
                    mergedCell.pdfCell.isMerged = true
                }
            }
        }
    }
}

function applyRowSpans(rows) {
    for (var rowIndex = 0; rowIndex < rows.length; rowIndex++) {
        var row = rows[rowIndex];
        for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
            var cell = row.cells[cellIndex];
            if (isDefined(cell.rowSpan) && !isDefined(cell.pdfCell.isMerged)) {
                for (var spanIndex = 1; spanIndex <= cell.rowSpan; spanIndex++) {
                    var mergedCell = rows[rowIndex + spanIndex].cells[cellIndex];
                    cell.pdfCell._rect.h += mergedCell.pdfCell._rect.h;
                    mergedCell.pdfCell._rect.h = 0;
                    mergedCell.pdfCell.isMerged = true
                }
            }
        }
    }
}

function resizeFirstColumnByIndentLevel(rows, options) {
    rows.forEach(row => {
        row.cells[0].pdfCell._rect.w -= row.indentLevel * options.indent
    })
}

function applyBordersConfig(rows) {
    for (var rowIndex = 0; rowIndex < rows.length; rowIndex++) {
        var cells = rows[rowIndex].cells;
        for (var columnIndex = 0; columnIndex < cells.length; columnIndex++) {
            var pdfCell = cells[columnIndex].pdfCell;
            var leftPdfCell = columnIndex >= 1 ? cells[columnIndex - 1].pdfCell : null;
            var topPdfCell = rowIndex >= 1 ? rows[rowIndex - 1].cells[columnIndex].pdfCell : null;
            if (false === pdfCell.drawLeftBorder && !isDefined(cells[columnIndex].colSpan)) {
                if (isDefined(leftPdfCell)) {
                    leftPdfCell.drawRightBorder = false
                }
            } else if (!isDefined(pdfCell.drawLeftBorder)) {
                if (isDefined(leftPdfCell) && false === leftPdfCell.drawRightBorder) {
                    pdfCell.drawLeftBorder = false
                }
            }
            if (false === pdfCell.drawTopBorder) {
                if (isDefined(topPdfCell)) {
                    topPdfCell.drawBottomBorder = false
                }
            } else if (!isDefined(pdfCell.drawTopBorder)) {
                if (isDefined(topPdfCell) && false === topPdfCell.drawBottomBorder) {
                    pdfCell.drawTopBorder = false
                }
            }
        }
    }
}

function calculateCoordinates(doc, rows, options) {
    var _topLeft$y;
    var topLeft = null === options || void 0 === options ? void 0 : options.topLeft;
    var margin = normalizeBoundaryValue(null === options || void 0 === options ? void 0 : options.margin);
    var y = (null !== (_topLeft$y = null === topLeft || void 0 === topLeft ? void 0 : topLeft.y) && void 0 !== _topLeft$y ? _topLeft$y : 0) + margin.top;
    rows.forEach(row => {
        var _topLeft$x2;
        var x = (null !== (_topLeft$x2 = null === topLeft || void 0 === topLeft ? void 0 : topLeft.x) && void 0 !== _topLeft$x2 ? _topLeft$x2 : 0) + margin.left;
        var intend = row.indentLevel * options.indent;
        row.cells.forEach(cell => {
            cell.pdfCell._rect.x = x + intend;
            cell.pdfCell._rect.y = y;
            x += cell.pdfCell._rect.w
        });
        y += row.height
    })
}

function calculateTableSize(doc, rows, options) {
    var _topLeft$x3, _topLeft$y2, _columnWidths$reduce, _rowHeights$reduce;
    var topLeft = null === options || void 0 === options ? void 0 : options.topLeft;
    var columnWidths = null === options || void 0 === options ? void 0 : options.columnWidths;
    var rowHeights = rows.map(row => row.height);
    return {
        x: null !== (_topLeft$x3 = null === topLeft || void 0 === topLeft ? void 0 : topLeft.x) && void 0 !== _topLeft$x3 ? _topLeft$x3 : 0,
        y: null !== (_topLeft$y2 = null === topLeft || void 0 === topLeft ? void 0 : topLeft.y) && void 0 !== _topLeft$y2 ? _topLeft$y2 : 0,
        w: null !== (_columnWidths$reduce = null === columnWidths || void 0 === columnWidths ? void 0 : columnWidths.reduce((a, b) => a + b, 0)) && void 0 !== _columnWidths$reduce ? _columnWidths$reduce : 0,
        h: null !== (_rowHeights$reduce = null === rowHeights || void 0 === rowHeights ? void 0 : rowHeights.reduce((a, b) => a + b, 0)) && void 0 !== _rowHeights$reduce ? _rowHeights$reduce : 0
    }
}
export {
    initializeCellsWidth,
    applyColSpans,
    applyRowSpans,
    resizeFirstColumnByIndentLevel,
    applyBordersConfig,
    calculateHeights,
    calculateCoordinates,
    calculateTableSize
};
