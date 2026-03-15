/**
 * DevExtreme (esm/data/array_utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isPlainObject,
    isEmptyObject,
    isDefined,
    isObject
} from "../core/utils/type";
import config from "../core/config";
import Guid from "../core/guid";
import {
    extend,
    extendFromObject
} from "../core/utils/extend";
import {
    errors
} from "./errors";
import {
    deepExtendArraySafe
} from "../core/utils/object";
import {
    compileGetter
} from "../core/utils/data";
import {
    keysEqual,
    rejectedPromise,
    trivialPromise
} from "./utils";

function hasKey(target, keyOrKeys) {
    var key;
    var keys = "string" === typeof keyOrKeys ? keyOrKeys.split() : keyOrKeys.slice();
    while (keys.length) {
        key = keys.shift();
        if (key in target) {
            return true
        }
    }
    return false
}

function findItems(keyInfo, items, key, groupCount) {
    var childItems;
    var result;
    if (groupCount) {
        for (var i = 0; i < items.length; i++) {
            childItems = items[i].items || items[i].collapsedItems || [];
            result = findItems(keyInfo, childItems || [], key, groupCount - 1);
            if (result) {
                return result
            }
        }
    } else if (indexByKey(keyInfo, items, key) >= 0) {
        return items
    }
}

function getItems(keyInfo, items, key, groupCount) {
    if (groupCount) {
        return findItems(keyInfo, items, key, groupCount) || []
    }
    return items
}

function generateDataByKeyMap(keyInfo, array) {
    if (keyInfo.key() && (!array._dataByKeyMap || array._dataByKeyMapLength !== array.length)) {
        var dataByKeyMap = {};
        var arrayLength = array.length;
        for (var i = 0; i < arrayLength; i++) {
            dataByKeyMap[JSON.stringify(keyInfo.keyOf(array[i]))] = array[i]
        }
        array._dataByKeyMap = dataByKeyMap;
        array._dataByKeyMapLength = arrayLength
    }
}

function getCacheValue(array, key) {
    if (array._dataByKeyMap) {
        return array._dataByKeyMap[JSON.stringify(key)]
    }
}

function getHasKeyCacheValue(array, key) {
    if (array._dataByKeyMap) {
        return array._dataByKeyMap[JSON.stringify(key)]
    }
    return true
}

function setDataByKeyMapValue(array, key, data) {
    if (array._dataByKeyMap) {
        array._dataByKeyMap[JSON.stringify(key)] = data;
        array._dataByKeyMapLength += data ? 1 : -1
    }
}

function cloneInstance(instance, clonedInstances) {
    clonedInstances = clonedInstances || new WeakMap;
    var result = instance ? Object.create(Object.getPrototypeOf(instance)) : {};
    if (instance) {
        clonedInstances.set(instance, result)
    }
    var instanceWithoutPrototype = extendFromObject({}, instance);
    for (var name in instanceWithoutPrototype) {
        var prop = instanceWithoutPrototype[name];
        if (isObject(prop) && !isPlainObject(prop) && !clonedInstances.has(prop)) {
            instanceWithoutPrototype[name] = cloneInstance(prop, clonedInstances)
        }
    }
    deepExtendArraySafe(result, instanceWithoutPrototype, true, true);
    for (var _name in result) {
        var _prop = result[_name];
        if (isObject(_prop) && clonedInstances.has(_prop)) {
            result[_name] = clonedInstances.get(_prop)
        }
    }
    return result
}

function createObjectWithChanges(target, changes) {
    var result = cloneInstance(target);
    return deepExtendArraySafe(result, changes, true, true)
}

function applyBatch(_ref) {
    var {
        keyInfo: keyInfo,
        data: data,
        changes: changes,
        groupCount: groupCount,
        useInsertIndex: useInsertIndex,
        immutable: immutable,
        disableCache: disableCache,
        logError: logError
    } = _ref;
    var resultItems = true === immutable ? [...data] : data;
    changes.forEach(item => {
        var items = "insert" === item.type ? resultItems : getItems(keyInfo, resultItems, item.key, groupCount);
        !disableCache && generateDataByKeyMap(keyInfo, items);
        switch (item.type) {
            case "update":
                update(keyInfo, items, item.key, item.data, true, immutable, logError);
                break;
            case "insert":
                insert(keyInfo, items, item.data, useInsertIndex && isDefined(item.index) ? item.index : -1, true, logError);
                break;
            case "remove":
                remove(keyInfo, items, item.key, true, logError)
        }
    });
    return resultItems
}

function getErrorResult(isBatch, logError, errorCode) {
    return !isBatch ? rejectedPromise(errors.Error(errorCode)) : logError && errors.log(errorCode)
}

function applyChanges(data, changes) {
    var options = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : {};
    var {
        keyExpr: keyExpr = "id",
        immutable: immutable = true
    } = options;
    var keyGetter = compileGetter(keyExpr);
    var keyInfo = {
        key: () => keyExpr,
        keyOf: obj => keyGetter(obj)
    };
    return applyBatch({
        keyInfo: keyInfo,
        data: data,
        changes: changes,
        immutable: immutable,
        disableCache: true,
        logError: true
    })
}

function update(keyInfo, array, key, data, isBatch, immutable, logError) {
    var target;
    var keyExpr = keyInfo.key();
    if (keyExpr) {
        if (hasKey(data, keyExpr) && !keysEqual(keyExpr, key, keyInfo.keyOf(data))) {
            return getErrorResult(isBatch, logError, "E4017")
        }
        target = getCacheValue(array, key);
        if (!target) {
            var index = indexByKey(keyInfo, array, key);
            if (index < 0) {
                return getErrorResult(isBatch, logError, "E4009")
            }
            target = array[index];
            if (true === immutable && isDefined(target)) {
                var newTarget = createObjectWithChanges(target, data);
                array[index] = newTarget;
                return !isBatch && trivialPromise(newTarget, key)
            }
        }
    } else {
        target = key
    }
    deepExtendArraySafe(target, data, true);
    if (!isBatch) {
        if (config().useLegacyStoreResult) {
            return trivialPromise(key, data)
        } else {
            return trivialPromise(target, key)
        }
    }
}

function insert(keyInfo, array, data, index, isBatch, logError) {
    var keyValue;
    var keyExpr = keyInfo.key();
    var obj = isPlainObject(data) ? extend({}, data) : data;
    if (keyExpr) {
        keyValue = keyInfo.keyOf(obj);
        if (void 0 === keyValue || "object" === typeof keyValue && isEmptyObject(keyValue)) {
            if (Array.isArray(keyExpr)) {
                throw errors.Error("E4007")
            }
            keyValue = obj[keyExpr] = String(new Guid)
        } else if (void 0 !== array[indexByKey(keyInfo, array, keyValue)]) {
            return getErrorResult(isBatch, logError, "E4008")
        }
    } else {
        keyValue = obj
    }
    if (index >= 0) {
        array.splice(index, 0, obj)
    } else {
        array.push(obj)
    }
    setDataByKeyMapValue(array, keyValue, obj);
    if (!isBatch) {
        return trivialPromise(config().useLegacyStoreResult ? data : obj, keyValue)
    }
}

function remove(keyInfo, array, key, isBatch, logError) {
    var index = indexByKey(keyInfo, array, key);
    if (index > -1) {
        array.splice(index, 1);
        setDataByKeyMapValue(array, key, null)
    }
    if (!isBatch) {
        return trivialPromise(key)
    } else if (index < 0) {
        return getErrorResult(isBatch, logError, "E4009")
    }
}

function indexByKey(keyInfo, array, key) {
    var keyExpr = keyInfo.key();
    if (!getHasKeyCacheValue(array, key)) {
        return -1
    }
    for (var i = 0, arrayLength = array.length; i < arrayLength; i++) {
        if (keysEqual(keyExpr, keyInfo.keyOf(array[i]), key)) {
            return i
        }
    }
    return -1
}
export {
    applyBatch,
    createObjectWithChanges,
    update,
    insert,
    remove,
    indexByKey,
    applyChanges
};
