/**
 * DevExtreme (cjs/ui/data_grid/ui.data_grid.core.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _uiGrid_core = _interopRequireDefault(require("../grid_core/ui.grid_core.utils"));
var _uiGrid_core2 = _interopRequireDefault(require("../grid_core/ui.grid_core.modules"));

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
var _default = _extends({}, _uiGrid_core2.default, _uiGrid_core.default, {
    modules: []
});
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
