/**
 * DevExtreme (cjs/ui/html_editor/themes/base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _devextremeQuill = _interopRequireDefault(require("devextreme-quill"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var BaseTheme;
if (_devextremeQuill.default) {
    var Theme = _devextremeQuill.default.import("core/theme");
    BaseTheme = function(_Theme) {
        _inheritsLoose(BaseTheme, _Theme);

        function BaseTheme(quill, options) {
            var _this;
            _this = _Theme.call(this, quill, options) || this;
            _this.quill.root.classList.add("dx-htmleditor-content");
            _this.quill.root.setAttribute("role", "textbox");
            return _this
        }
        return BaseTheme
    }(Theme)
} else {
    BaseTheme = {}
}
var _default = BaseTheme;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
