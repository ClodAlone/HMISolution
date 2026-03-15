/**
 * DevExtreme (esm/core/utils/array.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isDefined
} from "./type";
import {
    each
} from "./iterator";
import {
    orderEach
} from "./object";
import config from "../config";
export var isEmpty = function(entity) {
    return Array.isArray(entity) && !entity.length
};
export var wrapToArray = function(entity) {
    return Array.isArray(entity) ? entity : [entity]
};
export var inArray = function(value, object) {
    if (!object) {
        return -1
    }
    var array = Array.isArray(object) ? object : object.toArray();
    return array.indexOf(value)
};
export var intersection = function(a, b) {
    if (!Array.isArray(a) || 0 === a.length || !Array.isArray(b) || 0 === b.length) {
        return []
    }
    var result = [];
    each(a, (function(_, value) {
        var index = inArray(value, b);
        if (-1 !== index) {
            result.push(value)
        }
    }));
    return result
};
export var uniqueValues = function(data) {
    return [...new Set(data)]
};
export var removeDuplicates = function(from, what) {
    if (!Array.isArray(from) || 0 === from.length) {
        return []
    }
    var result = from.slice();
    if (!Array.isArray(what) || 0 === what.length) {
        return result
    }
    each(what, (function(_, value) {
        var index = inArray(value, result);
        result.splice(index, 1)
    }));
    return result
};
export var normalizeIndexes = function(items, indexParameterName, currentItem, needIndexCallback) {
    var indexedItems = {};
    var parameterIndex = 0;
    var useLegacyVisibleIndex = config().useLegacyVisibleIndex;
    each(items, (function(index, item) {
        index = item[indexParameterName];
        if (index >= 0) {
            indexedItems[index] = indexedItems[index] || [];
            if (item === currentItem) {
                indexedItems[index].unshift(item)
            } else {
                indexedItems[index].push(item)
            }
        } else {
            item[indexParameterName] = void 0
        }
    }));
    if (!useLegacyVisibleIndex) {
        each(items, (function() {
            if (!isDefined(this[indexParameterName]) && (!needIndexCallback || needIndexCallback(this))) {
                while (indexedItems[parameterIndex]) {
                    parameterIndex++
                }
                indexedItems[parameterIndex] = [this];
                parameterIndex++
            }
        }))
    }
    parameterIndex = 0;
    orderEach(indexedItems, (function(index, items) {
        each(items, (function() {
            if (index >= 0) {
                this[indexParameterName] = parameterIndex++
            }
        }))
    }));
    if (useLegacyVisibleIndex) {
        each(items, (function() {
            if (!isDefined(this[indexParameterName]) && (!needIndexCallback || needIndexCallback(this))) {
                this[indexParameterName] = parameterIndex++
            }
        }))
    }
    return parameterIndex
};
export var merge = function(array1, array2) {
    for (var i = 0; i < array2.length; i++) {
        array1[array1.length] = array2[i]
    }
    return array1
};
export var find = function(array, condition) {
    for (var i = 0; i < array.length; i++) {
        if (condition(array[i])) {
            return array[i]
        }
    }
};
export var groupBy = (array, cb) => array.reduce((result, item) => _extends({}, result, {
    [cb(item)]: [...result[cb(item)] || [], item]
}), {});
