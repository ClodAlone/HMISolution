/**
 * DevExtreme (esm/core/utils/iterator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
var map = (values, callback) => {
    if (Array.isArray(values)) {
        return values.map(callback)
    }
    var result = [];
    for (var key in values) {
        result.push(callback(values[key], key))
    }
    return result
};
var each = (values, callback) => {
    if (!values) {
        return
    }
    if ("length" in values) {
        for (var i = 0; i < values.length; i++) {
            if (false === callback.call(values[i], i, values[i])) {
                break
            }
        }
    } else {
        for (var key in values) {
            if (false === callback.call(values[key], key, values[key])) {
                break
            }
        }
    }
    return values
};
var reverseEach = (array, callback) => {
    if (!array || !("length" in array) || 0 === array.length) {
        return
    }
    for (var i = array.length - 1; i >= 0; i--) {
        if (false === callback.call(array[i], i, array[i])) {
            break
        }
    }
};
export {
    map,
    each,
    reverseEach
};
