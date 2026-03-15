/**
 * DevExtreme (exporter/exceljs/exportDataGrid.js)
 * Version: 19.2.4
 * Build date: Tue Nov 19 2019
 *
 * Copyright (c) 2012 - 2019 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
Object.defineProperty(exports, "__esModule", {
    value: true
});
exports.MAX_EXCEL_COLUMN_WIDTH = exports.exportDataGrid = void 0;
var _type = require("../../core/utils/type");
var MAX_DIGIT_WIDTH_IN_PIXELS = 7;
var MAX_EXCEL_COLUMN_WIDTH = 255;

function exportDataGrid(options) {
    if (!(0, _type.isDefined)(options)) {
        return
    }
    var customizeCell = options.customizeCell,
        component = options.component,
        worksheet = options.worksheet,
        _options$topLeftCell = options.topLeftCell,
        topLeftCell = void 0 === _options$topLeftCell ? {
            row: 1,
            column: 1
        } : _options$topLeftCell,
        _options$excelFilterE = options.excelFilterEnabled,
        excelFilterEnabled = void 0 === _options$excelFilterE ? false : _options$excelFilterE,
        _options$keepColumnWi = options.keepColumnWidths,
        keepColumnWidths = void 0 === _options$keepColumnWi ? true : _options$keepColumnWi,
        _options$selectedRows = options.selectedRowsOnly,
        selectedRowsOnly = void 0 === _options$selectedRows ? false : _options$selectedRows;
    worksheet.properties.outlineProperties = {
        summaryBelow: false,
        summaryRight: false
    };
    var cellsRange = {
        from: {
            row: topLeftCell.row,
            column: topLeftCell.column
        },
        to: {
            row: topLeftCell.row,
            column: topLeftCell.column
        }
    };
    var dataProvider = component.getDataProvider(selectedRowsOnly);
    return new Promise(function(resolve) {
        dataProvider.ready().done(function() {
            var columns = dataProvider.getColumns();
            var headerRowCount = dataProvider.getHeaderRowCount();
            var dataRowsCount = dataProvider.getRowsCount();
            if (keepColumnWidths) {
                _setColumnsWidth(worksheet, columns, cellsRange.from.column)
            }
            var mergedCells = [];
            var mergeRanges = [];
            for (var rowIndex = 0; rowIndex < dataRowsCount; rowIndex++) {
                var row = worksheet.getRow(cellsRange.from.row + rowIndex);
                _exportRow(rowIndex, columns.length, row, cellsRange.from.column, dataProvider, customizeCell, headerRowCount, mergedCells, mergeRanges);
                if (rowIndex >= headerRowCount) {
                    row.outlineLevel = dataProvider.getGroupLevel(rowIndex)
                }
                if (rowIndex >= 1) {
                    cellsRange.to.row++
                }
            }
            _mergeCells(worksheet, topLeftCell, mergeRanges);
            cellsRange.to.column += columns.length > 0 ? columns.length - 1 : 0;
            if (true === excelFilterEnabled) {
                if (!(0, _type.isDefined)(worksheet.autoFilter) && dataRowsCount > 0) {
                    worksheet.autoFilter = cellsRange;
                    worksheet.views = [{
                        state: "frozen",
                        ySplit: cellsRange.from.row + dataProvider.getFrozenArea().y - 1
                    }]
                }
            }
            resolve(cellsRange)
        })
    })
}

function _exportRow(rowIndex, cellCount, row, startColumnIndex, dataProvider, customizeCell, headerRowCount, mergedCells, mergeRanges) {
    var styles = dataProvider.getStyles();
    for (var cellIndex = 0; cellIndex < cellCount; cellIndex++) {
        var cellData = dataProvider.getCellData(rowIndex, cellIndex, true);
        var gridCell = cellData.cellSourceData;
        var excelCell = row.getCell(startColumnIndex + cellIndex);
        excelCell.value = cellData.value;
        if ((0, _type.isDefined)(excelCell.value)) {
            var _styles$dataProvider$ = styles[dataProvider.getStyleId(rowIndex, cellIndex)],
                bold = _styles$dataProvider$.bold,
                alignment = _styles$dataProvider$.alignment,
                wrapText = _styles$dataProvider$.wrapText;
            _setFont(excelCell, bold);
            _setAlignment(excelCell, wrapText, alignment)
        }
        if ((0, _type.isDefined)(customizeCell)) {
            customizeCell({
                cell: excelCell,
                excelCell: excelCell,
                gridCell: gridCell
            })
        }
        if (rowIndex < headerRowCount) {
            var mergeRange = _tryGetMergeRange(rowIndex, cellIndex, mergedCells, dataProvider);
            if ((0, _type.isDefined)(mergeRange)) {
                mergeRanges.push(mergeRange)
            }
        }
    }
}

function _setFont(excelCell, bold) {
    if ((0, _type.isDefined)(bold)) {
        excelCell.font = excelCell.font || {};
        excelCell.font.bold = bold
    }
}

function _setAlignment(excelCell, wrapText, horizontalAlignment) {
    excelCell.alignment = excelCell.alignment || {};
    if ((0, _type.isDefined)(wrapText)) {
        excelCell.alignment.wrapText = wrapText
    }
    if ((0, _type.isDefined)(horizontalAlignment)) {
        excelCell.alignment.horizontal = horizontalAlignment
    }
}

function _setColumnsWidth(worksheet, columns, startColumnIndex) {
    if (!(0, _type.isDefined)(columns)) {
        return
    }
    for (var i = 0; i < columns.length; i++) {
        var columnWidth = columns[i].width;
        if ("number" === typeof columnWidth && isFinite(columnWidth)) {
            worksheet.getColumn(startColumnIndex + i).width = Math.min(MAX_EXCEL_COLUMN_WIDTH, Math.floor(columnWidth / MAX_DIGIT_WIDTH_IN_PIXELS * 100) / 100)
        }
    }
}

function _tryGetMergeRange(rowIndex, cellIndex, mergedCells, dataProvider) {
    if (!mergedCells[rowIndex] || !mergedCells[rowIndex][cellIndex]) {
        var cellMerge = dataProvider.getCellMerging(rowIndex, cellIndex);
        if (cellMerge.colspan || cellMerge.rowspan) {
            for (var i = rowIndex; i <= rowIndex + cellMerge.rowspan || 0; i++) {
                for (var j = cellIndex; j <= cellIndex + cellMerge.colspan || 0; j++) {
                    if (!mergedCells[i]) {
                        mergedCells[i] = []
                    }
                    mergedCells[i][j] = true
                }
            }
            return {
                start: {
                    row: rowIndex,
                    column: cellIndex
                },
                end: {
                    row: rowIndex + (cellMerge.rowspan || 0),
                    column: cellIndex + (cellMerge.colspan || 0)
                }
            }
        }
    }
}

function _mergeCells(worksheet, topLeftCell, mergeRanges) {
    mergeRanges.forEach(function(mergeRange) {
        worksheet.mergeCells(mergeRange.start.row + topLeftCell.row, mergeRange.start.column + topLeftCell.column, mergeRange.end.row + topLeftCell.row, mergeRange.end.column + topLeftCell.column)
    })
}
exports.exportDataGrid = exportDataGrid;
exports.MAX_EXCEL_COLUMN_WIDTH = MAX_EXCEL_COLUMN_WIDTH;
