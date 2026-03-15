/**
 * DevExtreme (cjs/exporter/jspdf/v3/export_data_grid_3.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.exportDataGrid = exportDataGrid;
var _type = require("../../../core/utils/type");
var _extend = require("../../../core/utils/extend");
var _normalizeOptions = require("./normalizeOptions");
var _row_utils = require("./row_utils");
var _height_updater = require("./height_updater");
var _rows_generator = require("./rows_generator");
var _draw_utils = require("./draw_utils");

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}

function _getFullOptions(options) {
    var fullOptions = (0, _extend.extend)({}, options);
    if (!(0, _type.isDefined)(fullOptions.topLeft)) {
        throw "options.topLeft is required"
    }
    if (!(0, _type.isDefined)(fullOptions.indent)) {
        fullOptions.indent = 10
    }
    return fullOptions
}

function exportDataGrid(doc, dataGrid, options) {
    options = (0, _extend.extend)({}, _getFullOptions(options));
    var dataProvider = dataGrid.getDataProvider();
    return new Promise((function(resolve) {
        dataProvider.ready().done((function() {
            var _options$rowOptions, _options$rowOptions$h;
            var rowsInfo = (0, _rows_generator.generateRowsInfo)(dataProvider, dataGrid, null === (_options$rowOptions = options.rowOptions) || void 0 === _options$rowOptions ? void 0 : null === (_options$rowOptions$h = _options$rowOptions.headerStyles) || void 0 === _options$rowOptions$h ? void 0 : _options$rowOptions$h.backgroundColor);
            if (options.customizeCell) {
                rowsInfo.forEach((function(rowInfo) {
                    return rowInfo.cells.forEach((function(cellInfo) {
                        return options.customizeCell(cellInfo)
                    }))
                }))
            }(0, _normalizeOptions.normalizeRowsInfo)(rowsInfo);
            (0, _row_utils.initializeCellsWidth)(doc, dataProvider, rowsInfo, options);
            (0, _row_utils.resizeFirstColumnByIndentLevel)(rowsInfo, options);
            (0, _row_utils.applyColSpans)(rowsInfo);
            (0, _row_utils.calculateHeights)(doc, rowsInfo, options);
            (0, _row_utils.applyRowSpans)(rowsInfo);
            (0, _height_updater.updateRowsAndCellsHeights)(doc, rowsInfo);
            (0, _row_utils.calculateCoordinates)(doc, rowsInfo, options);
            (0, _row_utils.applyBordersConfig)(rowsInfo);
            var pdfCellsInfo = [].concat.apply([], rowsInfo.map((function(rowInfo) {
                return rowInfo.cells.filter((function(cell) {
                    return !(0, _type.isDefined)(cell.pdfCell.isMerged)
                })).map((function(cellInfo) {
                    return _extends({}, cellInfo.pdfCell, {
                        gridCell: cellInfo.gridCell,
                        pdfRowInfo: cellInfo.pdfRowInfo
                    })
                }))
            })));
            var docStyles = (0, _draw_utils.getDocumentStyles)(doc);
            (0, _draw_utils.drawCellsContent)(doc, options.customDrawCell, pdfCellsInfo, docStyles);
            (0, _draw_utils.drawCellsLines)(doc, pdfCellsInfo, docStyles);
            var isDrawTableBorderSpecified = true === options.drawTableBorder;
            var isEmptyPdfCellsInfoSpecified = (0, _type.isDefined)(pdfCellsInfo) && 0 === pdfCellsInfo.length;
            if (isDrawTableBorderSpecified || isEmptyPdfCellsInfoSpecified) {
                var tableRect = (0, _row_utils.calculateTableSize)(doc, rowsInfo, options);
                (0, _draw_utils.drawGridLines)(doc, tableRect, docStyles)
            }(0, _draw_utils.setDocumentStyles)(doc, docStyles);
            resolve()
        }))
    }))
}
