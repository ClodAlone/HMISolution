/**
 * DevExtreme (cjs/core/polyfills/weak_map.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _array = require("../utils/array");
var _window = require("../utils/window");
var weakMap = (0, _window.hasWindow)() ? (0, _window.getWindow)().WeakMap : WeakMap;
if (!weakMap) {
    weakMap = function() {
        var keys = [];
        var values = [];
        this.set = function(key, value) {
            var index = (0, _array.inArray)(key, keys);
            if (-1 === index) {
                keys.push(key);
                values.push(value)
            } else {
                values[index] = value
            }
        };
        this.get = function(key) {
            var index = (0, _array.inArray)(key, keys);
            if (-1 === index) {
                return
            }
            return values[index]
        };
        this.has = function(key) {
            var index = (0, _array.inArray)(key, keys);
            if (-1 === index) {
                return false
            }
            return true
        };
        this.delete = function(key) {
            var index = (0, _array.inArray)(key, keys);
            if (-1 === index) {
                return
            }
            keys.splice(index, 1);
            values.splice(index, 1)
        }
    }
}
var _default = weakMap;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
