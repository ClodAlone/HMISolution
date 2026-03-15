/**
 * DevExtreme (cjs/exporter/jspdf/v3/pdf_utils_v3.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.calculateRowHeight = calculateRowHeight;
exports.calculateTargetRectWidth = calculateTargetRectWidth;
exports.calculateTextHeight = calculateTextHeight;
exports.getTextLines = getTextLines;
var _type = require("../../../core/utils/type");

function getTextLines(doc, text, font, _ref) {
    var wordWrapEnabled = _ref.wordWrapEnabled,
        targetRectWidth = _ref.targetRectWidth;
    if (wordWrapEnabled) {
        return doc.splitTextToSize(text, targetRectWidth, {
            fontSize: (null === font || void 0 === font ? void 0 : font.size) || doc.getFontSize()
        })
    }
    return text.split("\n")
}

function calculateTargetRectWidth(columnWidth, padding) {
    return columnWidth - (padding.left + padding.right)
}

function calculateTextHeight(doc, text, font, _ref2) {
    var wordWrapEnabled = _ref2.wordWrapEnabled,
        targetRectWidth = _ref2.targetRectWidth;
    var height = doc.getTextDimensions(text, {
        fontSize: (null === font || void 0 === font ? void 0 : font.size) || doc.getFontSize()
    }).h;
    var linesCount = getTextLines(doc, text, font, {
        wordWrapEnabled: wordWrapEnabled,
        targetRectWidth: targetRectWidth
    }).length;
    return height * linesCount * doc.getLineHeightFactor()
}

function calculateRowHeight(doc, cells, columnWidths) {
    if (cells.length !== columnWidths.length) {
        throw "the cells count must be equal to the count of the columns"
    }
    var rowHeight = 0;
    for (var cellIndex = 0; cellIndex < cells.length; cellIndex++) {
        if ((0, _type.isDefined)(cells[cellIndex].rowSpan)) {
            continue
        }
        var cellText = cells[cellIndex].pdfCell.text;
        var cellPadding = cells[cellIndex].pdfCell.padding;
        var font = cells[cellIndex].pdfCell.font;
        var wordWrapEnabled = cells[cellIndex].pdfCell.wordWrapEnabled;
        var columnWidth = columnWidths[cellIndex];
        var targetRectWidth = calculateTargetRectWidth(columnWidth, cellPadding);
        if ((0, _type.isDefined)(cellText)) {
            var cellHeight = calculateTextHeight(doc, cellText, font, {
                wordWrapEnabled: wordWrapEnabled,
                targetRectWidth: targetRectWidth
            }) + cellPadding.top + cellPadding.bottom;
            if (rowHeight < cellHeight) {
                rowHeight = cellHeight
            }
        }
    }
    return rowHeight
}
