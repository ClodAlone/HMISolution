/**
 * DevExtreme (renovation/component_wrapper/editors/check_box.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _editor = _interopRequireDefault(require("./editor"));

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
var CheckBox = function(_Editor) {
    _inheritsLoose(CheckBox, _Editor);

    function CheckBox() {
        return _Editor.apply(this, arguments) || this
    }
    var _proto = CheckBox.prototype;
    _proto._useTemplates = function() {
        return false
    };
    _proto._isFocused = function() {
        var focusTarget = this.$element()[0];
        return focusTarget.classList.contains("dx-state-focused")
    };
    _proto.getSupportedKeyNames = function() {
        return ["space"]
    };
    _proto.getProps = function() {
        var props = _Editor.prototype.getProps.call(this);
        if (null !== props.value) {
            props.value = Boolean(props.value)
        }
        return props
    };
    return CheckBox
}(_editor.default);
exports.default = CheckBox;
module.exports = exports.default;
module.exports.default = exports.default;
