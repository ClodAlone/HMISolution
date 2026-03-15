/**
 * DevExtreme (esm/core/utils/data.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import errors from "../errors";
import Class from "../class";
import {
    deepExtendArraySafe
} from "./object";
import {
    isObject,
    isPlainObject,
    isFunction,
    isDefined
} from "./type";
import {
    each
} from "./iterator";
import variableWrapper from "./variable_wrapper";
var unwrapVariable = variableWrapper.unwrap;
var isWrapped = variableWrapper.isWrapped;
var assign = variableWrapper.assign;
var bracketsToDots = function(expr) {
    return expr.replace(/\[/g, ".").replace(/\]/g, "")
};
export var getPathParts = function(name) {
    return bracketsToDots(name).split(".")
};
var readPropValue = function(obj, propName, options) {
    options = options || {};
    if ("this" === propName) {
        return unwrap(obj, options)
    }
    return unwrap(obj[propName], options)
};
var assignPropValue = function(obj, propName, value, options) {
    if ("this" === propName) {
        throw new errors.Error("E4016")
    }
    var propValue = obj[propName];
    if (options.unwrapObservables && isWrapped(propValue)) {
        assign(propValue, value)
    } else {
        obj[propName] = value
    }
};
var prepareOptions = function(options) {
    options = options || {};
    options.unwrapObservables = void 0 !== options.unwrapObservables ? options.unwrapObservables : true;
    return options
};

function unwrap(value, options) {
    return options.unwrapObservables ? unwrapVariable(value) : value
}
export var compileGetter = function(expr) {
    if (arguments.length > 1) {
        expr = [].slice.call(arguments)
    }
    if (!expr || "this" === expr) {
        return function(obj) {
            return obj
        }
    }
    if ("string" === typeof expr) {
        var path = getPathParts(expr);
        return function(obj, options) {
            options = prepareOptions(options);
            var functionAsIs = options.functionsAsIs;
            var hasDefaultValue = "defaultValue" in options;
            var current = unwrap(obj, options);
            for (var i = 0; i < path.length; i++) {
                if (!current) {
                    if (null == current && hasDefaultValue) {
                        return options.defaultValue
                    }
                    break
                }
                var pathPart = path[i];
                if (hasDefaultValue && isObject(current) && !(pathPart in current)) {
                    return options.defaultValue
                }
                var next = unwrap(current[pathPart], options);
                if (!functionAsIs && isFunction(next)) {
                    next = next.call(current)
                }
                current = next
            }
            return current
        }
    }
    if (Array.isArray(expr)) {
        return combineGetters(expr)
    }
    if (isFunction(expr)) {
        return expr
    }
};

function combineGetters(getters) {
    var compiledGetters = {};
    for (var i = 0, l = getters.length; i < l; i++) {
        var getter = getters[i];
        compiledGetters[getter] = compileGetter(getter)
    }
    return function(obj, options) {
        var result;
        each(compiledGetters, (function(name) {
            var value = this(obj, options);
            if (void 0 === value) {
                return
            }
            var current = result || (result = {});
            var path = name.split(".");
            var last = path.length - 1;
            for (var _i = 0; _i < last; _i++) {
                var pathItem = path[_i];
                if (!(pathItem in current)) {
                    current[pathItem] = {}
                }
                current = current[pathItem]
            }
            current[path[last]] = value
        }));
        return result
    }
}
var ensurePropValueDefined = function(obj, propName, value, options) {
    if (isDefined(value)) {
        return value
    }
    var newValue = {};
    assignPropValue(obj, propName, newValue, options);
    return newValue
};
export var compileSetter = function(expr) {
    expr = getPathParts(expr || "this");
    var lastLevelIndex = expr.length - 1;
    return function(obj, value, options) {
        options = prepareOptions(options);
        var currentValue = unwrap(obj, options);
        expr.forEach((function(propertyName, levelIndex) {
            var propertyValue = readPropValue(currentValue, propertyName, options);
            var isPropertyFunc = !options.functionsAsIs && isFunction(propertyValue) && !isWrapped(propertyValue);
            if (levelIndex === lastLevelIndex) {
                if (options.merge && isPlainObject(value) && (!isDefined(propertyValue) || isPlainObject(propertyValue))) {
                    propertyValue = ensurePropValueDefined(currentValue, propertyName, propertyValue, options);
                    deepExtendArraySafe(propertyValue, value, false, true)
                } else if (isPropertyFunc) {
                    currentValue[propertyName](value)
                } else {
                    assignPropValue(currentValue, propertyName, value, options)
                }
            } else {
                propertyValue = ensurePropValueDefined(currentValue, propertyName, propertyValue, options);
                if (isPropertyFunc) {
                    propertyValue = propertyValue.call(currentValue)
                }
                currentValue = propertyValue
            }
        }))
    }
};
export var toComparable = function(value, caseSensitive) {
    if (value instanceof Date) {
        return value.getTime()
    }
    if (value && value instanceof Class && value.valueOf) {
        return value.valueOf()
    }
    if (!caseSensitive && "string" === typeof value) {
        return value.toLowerCase()
    }
    return value
};
