/**
 * DevExtreme (cjs/ui/html_editor/converters/delta.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _converterController = _interopRequireDefault(require("../converterController"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var DeltaConverter = function() {
    function DeltaConverter() {}
    var _proto = DeltaConverter.prototype;
    _proto.setQuillInstance = function(quillInstance) {
        this.quillInstance = quillInstance
    };
    _proto.toHtml = function() {
        if (!this.quillInstance) {
            return
        }
        return this._isQuillEmpty() ? "" : this.quillInstance.getSemanticHTML(0, this.quillInstance.getLength() + 1)
    };
    _proto._isQuillEmpty = function() {
        var delta = this.quillInstance.getContents();
        return 1 === delta.length() && this._isDeltaEmpty(delta)
    };
    _proto._isDeltaEmpty = function(delta) {
        return delta.reduce((function(__, _ref) {
            var insert = _ref.insert;
            return -1 !== insert.indexOf("\n")
        }))
    };
    return DeltaConverter
}();
_converterController.default.addConverter("delta", DeltaConverter);
var _default = DeltaConverter;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
