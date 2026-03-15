/**
 * DevExtreme (cjs/renovation/component_wrapper/scheduler_header_panel.js)
 * Version: 21.2.3
 * Build date: Thu Oct 28 2021
 *
 * Copyright (c) 2012 - 2021 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _component = _interopRequireDefault(require("./common/component"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

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
var HeaderPanel = function(_Component) {
    _inheritsLoose(HeaderPanel, _Component);

    function HeaderPanel() {
        return _Component.apply(this, arguments) || this
    }
    var _proto = HeaderPanel.prototype;
    _proto._setOptionsByReference = function() {
        _Component.prototype._setOptionsByReference.call(this);
        this._optionsByReference = _extends({}, this._optionsByReference, {
            dateHeaderData: true
        })
    };
    return HeaderPanel
}(_component.default);
exports.default = HeaderPanel;
module.exports = exports.default;
module.exports.default = exports.default;
