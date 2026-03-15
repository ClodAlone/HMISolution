/**
 * DevExtreme (esm/ui/pivot_grid/ui.pivot_grid.utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isNumeric,
    isDefined,
    type
} from "../../core/utils/type";
import coreAjaxUtils from "../../core/utils/ajax";
import {
    compileGetter
} from "../../core/utils/data";
import {
    each,
    map
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import localizationDate from "../../localization/date";
import formatHelper from "../../format_helper";
import {
    DataSource
} from "../../data/data_source/data_source";
import ArrayStore from "../../data/array_store";
import {
    when,
    Deferred
} from "../../core/utils/deferred";
export var setFieldProperty = function(field, property, value, isInitialization) {
    var initProperties = field._initProperties = field._initProperties || {};
    var initValue = isInitialization ? value : field[property];
    if (!Object.prototype.hasOwnProperty.call(initProperties, property) || isInitialization) {
        initProperties[property] = initValue
    }
    field[property] = value
};
export function sendRequest(options) {
    return coreAjaxUtils.sendRequest(options)
}
var foreachTreeAsyncDate = new Date;

function createForeachTreeFunc(isAsync) {
    return function foreachTreeFunc(items, callback, parentAtFirst, members, index, isChildrenProcessing) {
        members = members || [];
        items = items || [];
        var i;
        var deferred;
        index = index || 0;

        function createForeachTreeAsyncHandler(deferred, i, isChildrenProcessing) {
            when(foreachTreeFunc(items, callback, parentAtFirst, members, i, isChildrenProcessing)).done(deferred.resolve)
        }
        for (i = index; i < items.length; i++) {
            if (isAsync && i > index && i % 1e4 === 0 && new Date - foreachTreeAsyncDate >= 300) {
                foreachTreeAsyncDate = new Date;
                deferred = new Deferred;
                setTimeout(createForeachTreeAsyncHandler(deferred, i, false), 0);
                return deferred
            }
            var item = items[i];
            if (!isChildrenProcessing) {
                members.unshift(item);
                if (parentAtFirst && false === callback(members, i)) {
                    return
                }
                if (item.children) {
                    var childrenDeferred = foreachTreeFunc(item.children, callback, parentAtFirst, members);
                    if (isAsync && childrenDeferred) {
                        deferred = new Deferred;
                        childrenDeferred.done(createForeachTreeAsyncHandler(deferred, i, true));
                        return deferred
                    }
                }
            }
            isChildrenProcessing = false;
            if (!parentAtFirst && false === callback(members, i)) {
                return
            }
            members.shift();
            if (items[i] !== item) {
                i--
            }
        }
    }
}
export var foreachTree = createForeachTreeFunc(false);
export var foreachTreeAsync = createForeachTreeFunc(true);
export function findField(fields, id) {
    if (fields && isDefined(id)) {
        for (var i = 0; i < fields.length; i++) {
            var field = fields[i];
            if (field.name === id || field.caption === id || field.dataField === id || field.index === id) {
                return i
            }
        }
    }
    return -1
}
export function formatValue(value, options) {
    var valueText = value === value && formatHelper.format(value, options.format);
    var formatObject = {
        value: value,
        valueText: valueText || ""
    };
    return options.customizeText ? options.customizeText.call(options, formatObject) : formatObject.valueText
}
export function getCompareFunction(valueSelector) {
    return function(a, b) {
        var result = 0;
        var valueA = valueSelector(a);
        var valueB = valueSelector(b);
        var aIsDefined = isDefined(valueA);
        var bIsDefined = isDefined(valueB);
        if (aIsDefined && bIsDefined) {
            if (valueA > valueB) {
                result = 1
            } else if (valueA < valueB) {
                result = -1
            }
        }
        if (aIsDefined && !bIsDefined) {
            result = 1
        }
        if (!aIsDefined && bIsDefined) {
            result = -1
        }
        return result
    }
}
export function createPath(items) {
    var result = [];
    for (var i = items.length - 1; i >= 0; i--) {
        result.push(items[i].key || items[i].value)
    }
    return result
}
export function foreachDataLevel(data, callback, index, childrenField) {
    index = index || 0;
    childrenField = childrenField || "children";
    if (data.length) {
        callback(data, index)
    }
    for (var i = 0; i < data.length; i++) {
        var item = data[i];
        if (item[childrenField] && item[childrenField].length) {
            foreachDataLevel(item[childrenField], callback, index + 1, childrenField)
        }
    }
}
export function mergeArraysByMaxValue(values1, values2) {
    var result = [];
    for (var i = 0; i < values1.length; i++) {
        result.push(Math.max(values1[i] || 0, values2[i] || 0))
    }
    return result
}
export function getExpandedLevel(options, axisName) {
    var dimensions = options[axisName];
    var expandLevel = 0;
    var expandedPaths = ("columns" === axisName ? options.columnExpandedPaths : options.rowExpandedPaths) || [];
    if (options.headerName === axisName) {
        expandLevel = options.path.length
    } else if (options.headerName && options.headerName !== axisName && options.oppositePath) {
        expandLevel = options.oppositePath.length
    } else {
        each(expandedPaths, (function(_, path) {
            expandLevel = Math.max(expandLevel, path.length)
        }))
    }
    while (dimensions[expandLevel + 1] && dimensions[expandLevel].expanded) {
        expandLevel++
    }
    return expandLevel
}

function createGroupFields(item) {
    return map(["year", "quarter", "month"], (function(value, index) {
        return extend({}, item, {
            groupInterval: value,
            groupIndex: index
        })
    }))
}

function parseFields(dataSource, fieldsList, path, fieldsDataType) {
    var result = [];
    Object.keys(fieldsList || []).forEach((function(field) {
        if (field && 0 === field.indexOf("__")) {
            return
        }
        var dataIndex = 1;
        var currentPath = path.length ? path + "." + field : field;
        var dataType = fieldsDataType[currentPath];
        var getter = compileGetter(currentPath);
        var value = fieldsList[field];
        var items;
        while (!isDefined(value) && dataSource[dataIndex]) {
            value = getter(dataSource[dataIndex]);
            dataIndex++
        }
        if (!dataType && isDefined(value)) {
            dataType = type(value)
        }
        items = [{
            dataField: currentPath,
            dataType: dataType,
            groupName: "date" === dataType ? field : void 0,
            groupInterval: void 0,
            displayFolder: path
        }];
        if ("date" === dataType) {
            items = items.concat(createGroupFields(items[0]))
        } else if ("object" === dataType) {
            items = parseFields(dataSource, value, currentPath, fieldsDataType)
        }
        result.push.apply(result, items)
    }));
    return result
}
export function discoverObjectFields(items, fields) {
    var fieldsDataType = getFieldsDataType(fields);
    return parseFields(items, items[0], "", fieldsDataType)
}
export function getFieldsDataType(fields) {
    var result = {};
    each(fields, (function(_, field) {
        result[field.dataField] = result[field.dataField] || field.dataType
    }));
    return result
}
var DATE_INTERVAL_FORMATS = {
    month: function(value) {
        return localizationDate.getMonthNames()[value - 1]
    },
    quarter: function(value) {
        return localizationDate.format(new Date(2e3, 3 * value - 1), "quarter")
    },
    dayOfWeek: function(value) {
        return localizationDate.getDayNames()[value]
    }
};
export function setDefaultFieldValueFormatting(field) {
    if ("date" === field.dataType) {
        if (!field.format) {
            setFieldProperty(field, "format", DATE_INTERVAL_FORMATS[field.groupInterval])
        }
    } else if ("number" === field.dataType) {
        var groupInterval = isNumeric(field.groupInterval) && field.groupInterval > 0 && field.groupInterval;
        if (groupInterval && !field.customizeText) {
            setFieldProperty(field, "customizeText", (function(formatObject) {
                var secondValue = formatObject.value + groupInterval;
                var secondValueText = formatHelper.format(secondValue, field.format);
                return formatObject.valueText && secondValueText ? formatObject.valueText + " - " + secondValueText : ""
            }))
        }
    }
}
export function getFiltersByPath(fields, path) {
    var result = [];
    path = path || [];
    for (var i = 0; i < path.length; i++) {
        result.push(extend({}, fields[i], {
            groupIndex: null,
            groupName: null,
            filterType: "include",
            filterValues: [path[i]]
        }))
    }
    return result
}
export var storeDrillDownMixin = {
    createDrillDownDataSource: function(descriptions, params) {
        var items = this.getDrillDownItems(descriptions, params);

        function createCustomStoreMethod(methodName) {
            return function(options) {
                var d;
                if (void 0) {
                    d = (void 0)[methodName](options)
                } else {
                    d = new Deferred;
                    when(items).done((function(data) {
                        var arrayStore = new ArrayStore(data);
                        arrayStore[methodName](options).done(d.resolve).fail(d.reject)
                    })).fail(d.reject)
                }
                return d
            }
        }
        var dataSource = new DataSource({
            load: createCustomStoreMethod("load"),
            totalCount: createCustomStoreMethod("totalCount"),
            key: this.key()
        });
        return dataSource
    }
};
export function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1)
}
