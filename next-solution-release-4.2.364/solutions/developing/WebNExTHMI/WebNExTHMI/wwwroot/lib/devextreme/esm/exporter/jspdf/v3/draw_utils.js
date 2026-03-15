/**
 * DevExtreme (esm/exporter/jspdf/v3/draw_utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["_rect", "pdfRowInfo", "gridCell"];
import {
    isDefined,
    isObject
} from "../../../core/utils/type";
import {
    extend
} from "../../../core/utils/extend";
import {
    calculateTextHeight,
    getTextLines
} from "./pdf_utils_v3";
var defaultBorderLineWidth = 1;

function round(value) {
    return Math.round(1e3 * value) / 1e3
}

function drawCellsContent(doc, customDrawCell, cellsArray, docStyles) {
    cellsArray.forEach(cell => {
        var {
            _rect: _rect,
            gridCell: gridCell
        } = cell, pdfCell = _objectWithoutPropertiesLoose(cell, _excluded);
        var eventArg = {
            doc: doc,
            rect: _rect,
            pdfCell: pdfCell,
            gridCell: gridCell,
            cancel: false
        };
        null === customDrawCell || void 0 === customDrawCell ? void 0 : customDrawCell(eventArg);
        if (!eventArg.cancel) {
            drawCellBackground(doc, cell);
            drawCellText(doc, cell, docStyles)
        }
    })
}

function drawLine(doc, startX, startY, endX, endY) {
    doc.line(round(startX), round(startY), round(endX), round(endY))
}

function drawRect(doc, x, y, width, height, style) {
    if (isDefined(style)) {
        doc.rect(round(x), round(y), round(width), round(height), style)
    } else {
        doc.rect(round(x), round(y), round(width), round(height))
    }
}

function getLineHeightShift(doc) {
    return (doc.getLineHeightFactor() - 1.15) * doc.getFontSize()
}

function drawTextInRect(doc, text, rect, verticalAlign, horizontalAlign, wordWrapEnabled) {
    var textArray = getTextLines(doc, text, doc.getFont(), {
        wordWrapEnabled: wordWrapEnabled,
        targetRectWidth: rect.w
    });
    var linesCount = textArray.length;
    var heightOfOneLine = calculateTextHeight(doc, textArray[0], doc.getFont(), {
        wordWrapEnabled: false
    });
    var vAlign = null !== verticalAlign && void 0 !== verticalAlign ? verticalAlign : "middle";
    var hAlign = null !== horizontalAlign && void 0 !== horizontalAlign ? horizontalAlign : "left";
    var verticalAlignCoefficientsMap = {
        top: 0,
        middle: .5,
        bottom: 1
    };
    var y = rect.y + rect.h * verticalAlignCoefficientsMap[vAlign] - heightOfOneLine * (linesCount - 1) * verticalAlignCoefficientsMap[vAlign] + getLineHeightShift(doc);
    var x = rect.x + rect.w * {
        left: 0,
        center: .5,
        right: 1
    } [hAlign];
    doc.text(textArray.join("\n"), round(x), round(y), {
        baseline: vAlign,
        align: hAlign
    })
}

function drawCellBackground(doc, cell) {
    if (isDefined(cell.backgroundColor)) {
        if (cell.backgroundColor !== doc.getFillColor()) {
            callMethodWithColorParameter(doc, "setFillColor", cell.backgroundColor)
        }
        drawRect(doc, cell._rect.x, cell._rect.y, cell._rect.w, cell._rect.h, "F")
    }
}

function drawCellText(doc, cell, docStyles) {
    if (isDefined(cell.text) && "" !== cell.text) {
        var {
            textColor: textColor,
            font: font,
            _rect: _rect,
            padding: padding
        } = cell;
        setTextStyles(doc, {
            textColor: textColor,
            font: font
        }, docStyles);
        var textRect = {
            x: _rect.x + padding.left,
            y: _rect.y + padding.top,
            w: _rect.w - (padding.left + padding.right),
            h: _rect.h - (padding.top + padding.bottom)
        };
        drawTextInRect(doc, cell.text, textRect, cell.verticalAlign, cell.horizontalAlign, cell.wordWrapEnabled)
    }
}

function drawCellsLines(doc, cellsArray, docStyles) {
    cellsArray.filter(cell => !isDefined(cell.borderColor)).forEach(cell => {
        drawBorders(doc, cell._rect, cell, docStyles)
    });
    cellsArray.filter(cell => isDefined(cell.borderColor)).forEach(cell => {
        drawBorders(doc, cell._rect, cell, docStyles)
    })
}

function drawGridLines(doc, rect, docStyles) {
    drawBorders(doc, rect, {}, docStyles)
}

function drawBorders(doc, rect, _ref, docStyles) {
    var {
        borderColor: borderColor,
        drawLeftBorder: drawLeftBorder = true,
        drawRightBorder: drawRightBorder = true,
        drawTopBorder: drawTopBorder = true,
        drawBottomBorder: drawBottomBorder = true
    } = _ref;
    if (!isDefined(rect)) {
        throw "rect is required"
    }
    if (!drawLeftBorder && !drawRightBorder && !drawTopBorder && !drawBottomBorder) {
        return
    } else if (drawLeftBorder && drawRightBorder && drawTopBorder && drawBottomBorder) {
        setLinesStyles(doc, {
            borderColor: borderColor
        }, docStyles);
        drawRect(doc, rect.x, rect.y, rect.w, rect.h)
    } else {
        setLinesStyles(doc, {
            borderColor: borderColor
        }, docStyles);
        if (drawTopBorder) {
            drawLine(doc, rect.x, rect.y, rect.x + rect.w, rect.y)
        }
        if (drawLeftBorder) {
            drawLine(doc, rect.x, rect.y, rect.x, rect.y + rect.h)
        }
        if (drawRightBorder) {
            drawLine(doc, rect.x + rect.w, rect.y, rect.x + rect.w, rect.y + rect.h)
        }
        if (drawBottomBorder) {
            drawLine(doc, rect.x, rect.y + rect.h, rect.x + rect.w, rect.y + rect.h)
        }
    }
}

function setTextStyles(doc, _ref2, docStyles) {
    var {
        textColor: textColor,
        font: font
    } = _ref2;
    var currentTextColor = isDefined(textColor) ? textColor : docStyles.textColor;
    if (currentTextColor !== doc.getTextColor()) {
        callMethodWithColorParameter(doc, "setTextColor", currentTextColor)
    }
    var currentFont = isDefined(font) ? extend({}, docStyles.font, font) : docStyles.font;
    var docFont = doc.getFont();
    if (currentFont.name !== docFont.fontName || currentFont.style !== docFont.fontStyle || isDefined(currentFont.weight)) {
        doc.setFont(currentFont.name, currentFont.style, currentFont.weight)
    }
    if (currentFont.size !== doc.getFontSize()) {
        doc.setFontSize(currentFont.size)
    }
}

function setLinesStyles(doc, _ref3, docStyles) {
    var {
        borderColor: borderColor
    } = _ref3;
    doc.setLineWidth(defaultBorderLineWidth);
    var currentBorderColor = isDefined(borderColor) ? borderColor : docStyles.borderColor;
    if (currentBorderColor !== doc.getDrawColor()) {
        callMethodWithColorParameter(doc, "setDrawColor", currentBorderColor)
    }
}

function callMethodWithColorParameter(doc, method, color) {
    if (!isObject(color)) {
        doc[method](color)
    } else {
        var argsCount = Object.keys(color).length;
        if (3 === argsCount) {
            doc[method](color.ch1, color.ch2, color.ch3)
        } else if (4 === argsCount) {
            doc[method](color.ch1, color.ch2, color.ch3, color.ch4)
        } else {
            throw Error("An incorrect color object was passed: 3 or 4 members are expected while the passed object has ".concat(argsCount, " members"))
        }
    }
}

function getDocumentStyles(doc) {
    var docFont = doc.getFont();
    return {
        borderColor: doc.getDrawColor(),
        font: {
            name: docFont.fontName,
            style: docFont.fontStyle,
            size: doc.getFontSize()
        },
        textColor: doc.getTextColor()
    }
}

function setDocumentStyles(doc, styles) {
    var {
        borderColor: borderColor,
        font: font,
        textColor: textColor
    } = styles;
    var docFont = doc.getFont();
    if (docFont.fontName !== font.name || docFont.fontStyle !== font.style) {
        doc.setFont(font.name, font.style, void 0)
    }
    var docFontSize = doc.getFontSize();
    if (docFontSize !== font.size) {
        doc.setFontSize(font.size)
    }
    if (doc.getDrawColor() !== borderColor) {
        doc.setDrawColor(borderColor)
    }
    if (doc.getTextColor() !== textColor) {
        doc.setTextColor(textColor)
    }
}
export {
    drawCellsContent,
    drawCellsLines,
    drawGridLines,
    getDocumentStyles,
    setDocumentStyles,
    drawTextInRect,
    drawRect,
    drawLine
};
