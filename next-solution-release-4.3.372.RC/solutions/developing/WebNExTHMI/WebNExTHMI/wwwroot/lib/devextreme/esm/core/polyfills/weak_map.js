/**
 * DevExtreme (esm/core/polyfills/weak_map.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    inArray
} from "../utils/array";
import {
    hasWindow,
    getWindow
} from "../utils/window";
var weakMap = hasWindow() ? getWindow().WeakMap : WeakMap;
if (!weakMap) {
    weakMap = function() {
        var keys = [];
        var values = [];
        this.set = function(key, value) {
            var index = inArray(key, keys);
            if (-1 === index) {
                keys.push(key);
                values.push(value)
            } else {
                values[index] = value
            }
        };
        this.get = function(key) {
            var index = inArray(key, keys);
            if (-1 === index) {
                return
            }
            return values[index]
        };
        this.has = function(key) {
            var index = inArray(key, keys);
            if (-1 === index) {
                return false
            }
            return true
        };
        this.delete = function(key) {
            var index = inArray(key, keys);
            if (-1 === index) {
                return
            }
            keys.splice(index, 1);
            values.splice(index, 1)
        }
    }
}
export default weakMap;
