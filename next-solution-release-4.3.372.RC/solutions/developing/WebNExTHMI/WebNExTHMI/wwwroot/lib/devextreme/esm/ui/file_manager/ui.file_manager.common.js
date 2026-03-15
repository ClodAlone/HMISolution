/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.common.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    when,
    Deferred
} from "../../core/utils/deferred";
import {
    extend
} from "../../core/utils/extend";
import {
    noop
} from "../../core/utils/common";
import {
    isFunction,
    isDefined
} from "../../core/utils/type";
export var whenSome = function(arg, onSuccess, onError) {
    onSuccess = onSuccess || noop;
    onError = onError || noop;
    if (!Array.isArray(arg)) {
        arg = [arg]
    }
    var deferreds = arg.map((item, index) => when(item).then(result => {
        isFunction(onSuccess) && onSuccess({
            item: item,
            index: index,
            result: result
        });
        return result
    }, error => {
        if (!error) {
            error = {}
        }
        error.index = index;
        isFunction(onError) && onError(error);
        return (new Deferred).resolve().promise()
    }));
    return when.apply(null, deferreds)
};
export var getDisplayFileSize = function(byteSize) {
    var sizesTitles = ["B", "KB", "MB", "GB", "TB"];
    var index = 0;
    var displaySize = byteSize;
    while (displaySize >= 1024 && index <= sizesTitles.length - 1) {
        displaySize /= 1024;
        index++
    }
    displaySize = Math.round(10 * displaySize) / 10;
    return "".concat(displaySize, " ").concat(sizesTitles[index])
};
export var extendAttributes = function(targetObject, sourceObject, objectKeysArray) {
    objectKeysArray.forEach(objectKey => {
        extend(true, targetObject, isDefined(sourceObject[objectKey]) ? {
            [objectKey]: sourceObject[objectKey]
        } : {})
    });
    return targetObject
};
export var findItemsByKeys = (itemInfos, keys) => {
    var itemMap = {};
    keys.forEach(key => {
        itemMap[key] = null
    });
    itemInfos.forEach(itemInfo => {
        var key = itemInfo.fileItem.key;
        if (Object.prototype.hasOwnProperty.call(itemMap, key)) {
            itemMap[key] = itemInfo
        }
    });
    var result = [];
    keys.forEach(key => {
        var itemInfo = itemMap[key];
        if (itemInfo) {
            result.push(itemInfo)
        }
    });
    return result
};
export var getMapFromObject = function(object) {
    var keys = Object.keys(object);
    var values = [];
    keys.forEach(key => values.push(object[key]));
    return {
        keys: keys,
        values: values
    }
};
