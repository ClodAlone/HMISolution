/**
 * DevExtreme (cjs/ui/html_editor/utils/table_helper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.TABLE_OPERATIONS = void 0;
exports.getAutoSizedElements = getAutoSizedElements;
exports.getColumnElements = getColumnElements;
exports.getLineElements = getLineElements;
exports.getRowElements = getRowElements;
exports.getTableFormats = getTableFormats;
exports.getTableOperationHandler = getTableOperationHandler;
exports.hasEmbedContent = hasEmbedContent;
exports.setLineElementsFormat = setLineElementsFormat;
exports.unfixTableWidth = unfixTableWidth;
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _iterator = require("../../../core/utils/iterator");
var _inflector = require("../../../core/utils/inflector");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var TABLE_FORMATS = ["table", "tableHeaderCell"];
var TABLE_OPERATIONS = ["insertTable", "insertHeaderRow", "insertRowAbove", "insertRowBelow", "insertColumnLeft", "insertColumnRight", "deleteColumn", "deleteRow", "deleteTable", "cellProperties", "tableProperties"];
exports.TABLE_OPERATIONS = TABLE_OPERATIONS;

function getTableFormats(quill) {
    var tableModule = quill.getModule("table");
    return null !== tableModule && void 0 !== tableModule && tableModule.tableFormats ? tableModule.tableFormats() : TABLE_FORMATS
}

function hasEmbedContent(module, selection) {
    return !!selection && module.quill.getText(selection).trim().length < selection.length
}

function unfixTableWidth($table, _ref) {
    var tableBlot = _ref.tableBlot,
        quill = _ref.quill;
    var formatBlot = null !== tableBlot && void 0 !== tableBlot ? tableBlot : quill.scroll.find($table.get(0));
    formatBlot.format("tableWidth", "initial")
}

function getColumnElements($table) {
    var index = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : 0;
    return $table.find("tr").eq(index).find("th, td")
}

function getAutoSizedElements($table) {
    var direction = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : "horizontal";
    var result = [];
    var isHorizontal = "horizontal" === direction;
    var $lineElements = isHorizontal ? getColumnElements($table) : getRowElements($table);
    $lineElements.each((function(index, element) {
        var $element = (0, _renderer.default)(element);
        if ("" === $element.get(0).style[isHorizontal ? "width" : "height"]) {
            result.push($element)
        }
    }));
    return result
}

function setLineElementsFormat(module, _ref2) {
    var elements = _ref2.elements,
        property = _ref2.property,
        value = _ref2.value;
    var tableBlotNames = module.quill.getModule("table").tableBlots;
    var fullPropertyName = "cell".concat((0, _inflector.camelize)(property, true));
    (0, _iterator.each)(elements, (function(i, element) {
        var _formatBlot;
        var formatBlot = module.quill.scroll.find(element);
        if (!tableBlotNames.includes(formatBlot.statics.blotName)) {
            var descendBlot = formatBlot.descendant((function(blot) {
                return tableBlotNames.includes(blot.statics.blotName)
            }));
            formatBlot = descendBlot ? descendBlot[0] : null
        }
        null === (_formatBlot = formatBlot) || void 0 === _formatBlot ? void 0 : _formatBlot.format(fullPropertyName, value + "px")
    }))
}

function getLineElements($table, index) {
    var direction = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : "horizontal";
    return "horizontal" === direction ? getRowElements($table, index) : getColumnElements($table, index)
}

function getRowElements($table) {
    var index = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : 0;
    return $table.find("th:nth-child(".concat(1 + index, "), td:nth-child(").concat(1 + index, ")"))
}

function getTableOperationHandler(quill, operationName) {
    for (var _len = arguments.length, rest = new Array(_len > 2 ? _len - 2 : 0), _key = 2; _key < _len; _key++) {
        rest[_key - 2] = arguments[_key]
    }
    return function() {
        var table = quill.getModule("table");
        if (!table) {
            return
        }
        quill.focus();
        return table[operationName].apply(table, rest)
    }
}
