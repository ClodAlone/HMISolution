/**
 * DevExtreme (esm/core/utils/type.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
var types = {
    "[object Array]": "array",
    "[object Date]": "date",
    "[object Object]": "object",
    "[object String]": "string",
    "[object Null]": "null"
};
var type = function(object) {
    var typeOfObject = Object.prototype.toString.call(object);
    return "object" === typeof object ? types[typeOfObject] || "object" : typeof object
};
var isBoolean = function(object) {
    return "boolean" === typeof object
};
var isExponential = function(value) {
    return isNumeric(value) && -1 !== value.toString().indexOf("e")
};
var isDate = function(object) {
    return "date" === type(object)
};
var isDefined = function(object) {
    return null !== object && void 0 !== object
};
var isFunction = function(object) {
    return "function" === typeof object
};
var isString = function(object) {
    return "string" === typeof object
};
var isNumeric = function(object) {
    return "number" === typeof object && isFinite(object) || !isNaN(object - parseFloat(object))
};
var isObject = function(object) {
    return "object" === type(object)
};
var isEmptyObject = function(object) {
    var property;
    for (property in object) {
        return false
    }
    return true
};
var isPlainObject = function(object) {
    if (!object || "[object Object]" !== Object.prototype.toString.call(object)) {
        return false
    }
    var proto = Object.getPrototypeOf(object);
    var ctor = Object.hasOwnProperty.call(proto, "constructor") && proto.constructor;
    return "function" === typeof ctor && Object.toString.call(ctor) === Object.toString.call(Object)
};
var isPrimitive = function(value) {
    return -1 === ["object", "array", "function"].indexOf(type(value))
};
var isWindow = function(object) {
    return null != object && object === object.window
};
var isRenderer = function(object) {
    return !!object && !!(object.jquery || object.dxRenderer)
};
var isPromise = function(object) {
    return !!object && isFunction(object.then)
};
var isDeferred = function(object) {
    return !!object && isFunction(object.done) && isFunction(object.fail)
};
var isEvent = function(object) {
    return !!(object && object.preventDefault)
};
export {
    isBoolean,
    isExponential,
    isDate,
    isDefined,
    isFunction,
    isString,
    isNumeric,
    isObject,
    isEmptyObject,
    isPlainObject,
    isPrimitive,
    isWindow,
    isRenderer,
    isPromise,
    isDeferred,
    type,
    isEvent
};
