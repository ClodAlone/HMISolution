/**
 * DevExtreme (cjs/exporter/jspdf/v3/height_updater.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.updateRowsAndCellsHeights = updateRowsAndCellsHeights;
var _type = require("../../../core/utils/type");
var _pdf_utils_v = require("./pdf_utils_v3");

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}

function updateRowsAndCellsHeights(doc, rows) {
    var rowsAdditionalHeights = calculateAdditionalRowsHeights(doc, rows);
    rows.forEach((function(row) {
        row.height += rowsAdditionalHeights[row.rowIndex]
    }));
    rows.forEach((function(row) {
        row.cells.forEach((function(cell) {
            var _cell$rowSpan;
            var rowsCount = (null !== (_cell$rowSpan = cell.rowSpan) && void 0 !== _cell$rowSpan ? _cell$rowSpan : 0) + 1;
            cell.pdfCell._rect.h = rows.slice(row.rowIndex, row.rowIndex + rowsCount).reduce((function(accumulator, rowInfo) {
                return accumulator + rowInfo.height
            }), 0)
        }))
    }))
}

function calculateAdditionalRowsHeights(doc, rows) {
    var rowsAdditionalHeights = Array.from({
        length: rows.length
    }, (function() {
        return 0
    }));
    var sortedRows = sortRowsByMaxRowSpanAsc(rows);
    sortedRows.forEach((function(row) {
        var cellsWithRowSpan = row.cells.filter((function(cell) {
            return (0, _type.isDefined)(cell.rowSpan)
        }));
        cellsWithRowSpan.forEach((function(cell) {
            var targetRectWidth = (0, _pdf_utils_v.calculateTargetRectWidth)(cell.pdfCell._rect.w, cell.pdfCell.padding);
            var textHeight = (0, _pdf_utils_v.calculateTextHeight)(doc, cell.pdfCell.text, cell.pdfCell.font, {
                wordWrapEnabled: cell.pdfCell.wordWrapEnabled,
                targetRectWidth: targetRectWidth
            });
            var cellHeight = textHeight + cell.pdfCell.padding.top + cell.pdfCell.padding.bottom;
            var rowsCount = cell.rowSpan + 1;
            var currentRowSpanRowsHeight = rows.slice(row.rowIndex, row.rowIndex + rowsCount).reduce((function(accumulator, rowInfo) {
                return accumulator + rowInfo.height + rowsAdditionalHeights[rowInfo.rowIndex]
            }), 0);
            if (cellHeight > currentRowSpanRowsHeight) {
                var delta = (cellHeight - currentRowSpanRowsHeight) / rowsCount;
                for (var spanIndex = row.rowIndex; spanIndex < row.rowIndex + rowsCount; spanIndex++) {
                    rowsAdditionalHeights[spanIndex] += delta
                }
            }
        }))
    }));
    return rowsAdditionalHeights
}

function sortRowsByMaxRowSpanAsc(rows) {
    var getMaxRowSpan = function(row) {
        var spansArray = row.cells.map((function(cell) {
            var _cell$rowSpan2;
            return null !== (_cell$rowSpan2 = cell.rowSpan) && void 0 !== _cell$rowSpan2 ? _cell$rowSpan2 : 0
        }));
        return Math.max.apply(Math, _toConsumableArray(spansArray))
    };
    return _toConsumableArray(rows).sort((function(row1, row2) {
        var row1RowSpan = getMaxRowSpan(row1);
        var row2RowSpan = getMaxRowSpan(row2);
        if (row1RowSpan > row2RowSpan) {
            return 1
        }
        if (row2RowSpan > row1RowSpan) {
            return -1
        }
        return 0
    }))
}
