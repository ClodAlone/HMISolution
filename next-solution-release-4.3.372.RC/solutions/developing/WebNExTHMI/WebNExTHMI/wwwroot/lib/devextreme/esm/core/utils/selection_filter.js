/**
 * DevExtreme (esm/core/utils/selection_filter.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getKeyHash,
    equalByValue
} from "./common";
import {
    isString,
    isObject
} from "./type";
export var SelectionFilterCreator = function(selectedItemKeys, isSelectAll) {
    this.getLocalFilter = function(keyGetter, equalKeys, equalByReference, keyExpr) {
        equalKeys = void 0 === equalKeys ? equalByValue : equalKeys;
        return functionFilter.bind(this, equalKeys, keyGetter, equalByReference, keyExpr)
    };
    this.getExpr = function(keyExpr) {
        if (!keyExpr) {
            return
        }
        var filterExpr;
        selectedItemKeys.forEach((function(key, index) {
            filterExpr = filterExpr || [];
            var filterExprPart;
            if (index > 0) {
                filterExpr.push(isSelectAll ? "and" : "or")
            }
            if (isString(keyExpr)) {
                filterExprPart = getFilterForPlainKey(keyExpr, key)
            } else {
                filterExprPart = function(keyExpr, itemKeyValue) {
                    var filterExpr = [];
                    for (var i = 0, length = keyExpr.length; i < length; i++) {
                        var currentKeyExpr = keyExpr[i];
                        var currentKeyValue = itemKeyValue && itemKeyValue[currentKeyExpr];
                        var filterExprPart = getFilterForPlainKey(currentKeyExpr, currentKeyValue);
                        if (!filterExprPart) {
                            break
                        }
                        if (i > 0) {
                            filterExpr.push(isSelectAll ? "or" : "and")
                        }
                        filterExpr.push(filterExprPart)
                    }
                    return filterExpr
                }(keyExpr, key)
            }
            filterExpr.push(filterExprPart)
        }));
        if (filterExpr && 1 === filterExpr.length) {
            filterExpr = filterExpr[0]
        }
        return filterExpr
    };
    this.getCombinedFilter = function(keyExpr, dataSourceFilter) {
        var filterExpr = this.getExpr(keyExpr);
        var combinedFilter = filterExpr;
        if (isSelectAll && dataSourceFilter) {
            if (filterExpr) {
                combinedFilter = [];
                combinedFilter.push(filterExpr);
                combinedFilter.push(dataSourceFilter)
            } else {
                combinedFilter = dataSourceFilter
            }
        }
        return combinedFilter
    };
    var selectedItemKeyHashesMap;
    var normalizeKeys = function(keys, keyOf, keyExpr) {
        return Array.isArray(keyExpr) ? keys.map(key => keyOf(key)) : keys
    };

    function functionFilter(equalKeys, keyOf, equalByReference, keyExpr, item) {
        var key = keyOf(item);
        var keyHash;
        var i;
        if (!equalByReference) {
            keyHash = getKeyHash(key);
            if (!isObject(keyHash)) {
                var selectedKeyHashesMap = function(keyOf, keyExpr) {
                    if (!selectedItemKeyHashesMap) {
                        selectedItemKeyHashesMap = {};
                        var normalizedKeys = normalizeKeys(selectedItemKeys, keyOf, keyExpr);
                        for (var i = 0; i < normalizedKeys.length; i++) {
                            selectedItemKeyHashesMap[getKeyHash(normalizedKeys[i])] = true
                        }
                    }
                    return selectedItemKeyHashesMap
                }(keyOf, keyExpr);
                if (selectedKeyHashesMap[keyHash]) {
                    return !isSelectAll
                }
                return !!isSelectAll
            }
        }
        for (i = 0; i < selectedItemKeys.length; i++) {
            if (equalKeys(selectedItemKeys[i], key)) {
                return !isSelectAll
            }
        }
        return !!isSelectAll
    }

    function getFilterForPlainKey(keyExpr, keyValue) {
        if (void 0 === keyValue) {
            return
        }
        return [keyExpr, isSelectAll ? "<>" : "=", keyValue]
    }
};
