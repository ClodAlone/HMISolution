/**
 * DevExtreme (esm/core/utils/object.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isNumeric,
    isPlainObject,
    isObject
} from "./type";
import variableWrapper from "./variable_wrapper";
var clone = function() {
    function Clone() {}
    return function(obj) {
        Clone.prototype = obj;
        return new Clone
    }
}();
var orderEach = function(map, func) {
    var keys = [];
    var key;
    var i;
    for (key in map) {
        if (Object.prototype.hasOwnProperty.call(map, key)) {
            keys.push(key)
        }
    }
    keys.sort((function(x, y) {
        var isNumberX = isNumeric(x);
        var isNumberY = isNumeric(y);
        if (isNumberX && isNumberY) {
            return x - y
        }
        if (isNumberX && !isNumberY) {
            return -1
        }
        if (!isNumberX && isNumberY) {
            return 1
        }
        if (x < y) {
            return -1
        }
        if (x > y) {
            return 1
        }
        return 0
    }));
    for (i = 0; i < keys.length; i++) {
        key = keys[i];
        func(key, map[key])
    }
};
var assignValueToProperty = function(target, property, value, assignByReference) {
    if (!assignByReference && variableWrapper.isWrapped(target[property])) {
        variableWrapper.assign(target[property], value)
    } else {
        target[property] = value
    }
};
var deepExtendArraySafe = function deepExtendArraySafe(target, changes, extendComplexObject, assignByReference) {
    var prevValue;
    var newValue;
    for (var name in changes) {
        prevValue = target[name];
        newValue = changes[name];
        if ("__proto__" === name || "constructor" === name || target === newValue) {
            continue
        }
        if (isPlainObject(newValue)) {
            var goDeeper = extendComplexObject ? isObject(prevValue) : isPlainObject(prevValue);
            newValue = deepExtendArraySafe(goDeeper ? prevValue : {}, newValue, extendComplexObject, assignByReference)
        }
        if (void 0 !== newValue && prevValue !== newValue) {
            assignValueToProperty(target, name, newValue, assignByReference)
        }
    }
    return target
};
export {
    clone,
    orderEach,
    deepExtendArraySafe
};
