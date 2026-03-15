/**
 * DevExtreme (esm/ui/html_editor/utils/table_helper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import {
    each
} from "../../../core/utils/iterator";
import {
    camelize
} from "../../../core/utils/inflector";
var TABLE_FORMATS = ["table", "tableHeaderCell"];
var TABLE_OPERATIONS = ["insertTable", "insertHeaderRow", "insertRowAbove", "insertRowBelow", "insertColumnLeft", "insertColumnRight", "deleteColumn", "deleteRow", "deleteTable", "cellProperties", "tableProperties"];

function getTableFormats(quill) {
    var tableModule = quill.getModule("table");
    return null !== tableModule && void 0 !== tableModule && tableModule.tableFormats ? tableModule.tableFormats() : TABLE_FORMATS
}

function hasEmbedContent(module, selection) {
    return !!selection && module.quill.getText(selection).trim().length < selection.length
}

function unfixTableWidth($table, _ref) {
    var {
        tableBlot: tableBlot,
        quill: quill
    } = _ref;
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
    $lineElements.each((index, element) => {
        var $element = $(element);
        if ("" === $element.get(0).style[isHorizontal ? "width" : "height"]) {
            result.push($element)
        }
    });
    return result
}

function setLineElementsFormat(module, _ref2) {
    var {
        elements: elements,
        property: property,
        value: value
    } = _ref2;
    var tableBlotNames = module.quill.getModule("table").tableBlots;
    var fullPropertyName = "cell".concat(camelize(property, true));
    each(elements, (i, element) => {
        var _formatBlot;
        var formatBlot = module.quill.scroll.find(element);
        if (!tableBlotNames.includes(formatBlot.statics.blotName)) {
            var descendBlot = formatBlot.descendant(blot => tableBlotNames.includes(blot.statics.blotName));
            formatBlot = descendBlot ? descendBlot[0] : null
        }
        null === (_formatBlot = formatBlot) || void 0 === _formatBlot ? void 0 : _formatBlot.format(fullPropertyName, value + "px")
    })
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
    return () => {
        var table = quill.getModule("table");
        if (!table) {
            return
        }
        quill.focus();
        return table[operationName](...rest)
    }
}
export {
    TABLE_OPERATIONS,
    getTableFormats,
    getTableOperationHandler,
    unfixTableWidth,
    getColumnElements,
    getAutoSizedElements,
    setLineElementsFormat,
    getLineElements,
    getRowElements,
    hasEmbedContent
};
