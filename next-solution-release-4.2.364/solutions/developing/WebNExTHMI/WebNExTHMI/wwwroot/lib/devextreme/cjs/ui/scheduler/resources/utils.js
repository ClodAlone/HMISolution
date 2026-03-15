/**
 * DevExtreme (cjs/ui/scheduler/resources/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.setResourceToAppointment = exports.reduceResourcesTree = exports.loadResources = exports.isResourceMultiple = exports.groupAppointmentsByResourcesCore = exports.groupAppointmentsByResources = exports.getWrappedDataSource = exports.getValueExpr = exports.getResourcesDataByGroups = exports.getResourceTreeLeaves = exports.getResourceColor = exports.getResourceByField = exports.getPathToLeaf = exports.getPaintedResources = exports.getOrLoadResourceItem = exports.getGroupsObjectFromGroupsArray = exports.getGroupCount = exports.getFieldExpr = exports.getDisplayExpr = exports.getDataAccessors = exports.getCellGroups = exports.getAppointmentColor = exports.getAllGroups = exports.filterResources = exports.createResourcesTree = exports.createResourceEditorModel = exports.createReducedResourcesTree = exports.createExpressions = void 0;
var _utils = require("../../../data/data_source/utils");
var _data_source = require("../../../data/data_source/data_source");
var _deferred = require("../../../core/utils/deferred");
var _query = _interopRequireDefault(require("../../../data/query"));
var _data = require("../../../core/utils/data");
var _iterator = require("../../../core/utils/iterator");
var _extend = require("../../../core/utils/extend");
var _type = require("../../../core/utils/type");
var _array = require("../../../core/utils/array");
var _object = require("../../../core/utils/object");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}

function _defineProperty(obj, key, value) {
    if (key in obj) {
        Object.defineProperty(obj, key, {
            value: value,
            enumerable: true,
            configurable: true,
            writable: true
        })
    } else {
        obj[key] = value
    }
    return obj
}

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}
var getValueExpr = function(resource) {
    return resource.valueExpr || "id"
};
exports.getValueExpr = getValueExpr;
var getDisplayExpr = function(resource) {
    return resource.displayExpr || "text"
};
exports.getDisplayExpr = getDisplayExpr;
var getFieldExpr = function(resource) {
    return resource.fieldExpr || resource.field
};
exports.getFieldExpr = getFieldExpr;
var getWrappedDataSource = function(dataSource) {
    if (dataSource instanceof _data_source.DataSource) {
        return dataSource
    }
    var result = {
        store: (0, _utils.normalizeDataSourceOptions)(dataSource).store,
        pageSize: 0
    };
    if (!Array.isArray(dataSource)) {
        result.filter = dataSource.filter
    }
    return new _data_source.DataSource(result)
};
exports.getWrappedDataSource = getWrappedDataSource;
var createResourcesTree = function(groups) {
    var leafIndex = 0;
    return function make(group, groupIndex, result, parent) {
        result = result || [];
        for (var itemIndex = 0; itemIndex < group.items.length; itemIndex++) {
            var _group$data;
            var currentGroupItem = group.items[itemIndex];
            var resultItem = {
                name: group.name,
                value: currentGroupItem.id,
                title: currentGroupItem.text,
                data: null === (_group$data = group.data) || void 0 === _group$data ? void 0 : _group$data[itemIndex],
                children: [],
                parent: parent || null
            };
            var nextGroupIndex = groupIndex + 1;
            if (groups[nextGroupIndex]) {
                make(groups[nextGroupIndex], nextGroupIndex, resultItem.children, resultItem)
            }
            if (!resultItem.children.length) {
                resultItem.leafIndex = leafIndex;
                leafIndex++
            }
            result.push(resultItem)
        }
        return result
    }(groups[0], 0)
};
exports.createResourcesTree = createResourcesTree;
var getPathToLeaf = function(leafIndex, groups) {
    var tree = createResourcesTree(groups);
    var leaf = function findLeafByIndex(data, index) {
        for (var i = 0; i < data.length; i++) {
            if (data[i].leafIndex === index) {
                return data[i]
            } else {
                var _leaf = findLeafByIndex(data[i].children, index);
                if (_leaf) {
                    return _leaf
                }
            }
        }
    }(tree, leafIndex);
    return function makeBranch(leaf, result) {
        result = result || [];
        result.push(leaf.value);
        if (leaf.parent) {
            makeBranch(leaf.parent, result)
        }
        return result
    }(leaf).reverse()
};
exports.getPathToLeaf = getPathToLeaf;
var getCellGroups = function(groupIndex, groups) {
    var result = [];
    if (getGroupCount(groups)) {
        if (groupIndex < 0) {
            return
        }
        var path = getPathToLeaf(groupIndex, groups);
        for (var i = 0; i < groups.length; i++) {
            result.push({
                name: groups[i].name,
                id: path[i]
            })
        }
    }
    return result
};
exports.getCellGroups = getCellGroups;
var getGroupCount = function(groups) {
    var result = 0;
    for (var i = 0, len = groups.length; i < len; i++) {
        if (!i) {
            result = groups[i].items.length
        } else {
            result *= groups[i].items.length
        }
    }
    return result
};
exports.getGroupCount = getGroupCount;
var getGroupsObjectFromGroupsArray = function(groupsArray) {
    return groupsArray.reduce((function(currentGroups, _ref) {
        var name = _ref.name,
            id = _ref.id;
        return _extends({}, currentGroups, _defineProperty({}, name, id))
    }), {})
};
exports.getGroupsObjectFromGroupsArray = getGroupsObjectFromGroupsArray;
var getAllGroups = function(groups) {
    var groupCount = getGroupCount(groups);
    return _toConsumableArray(new Array(groupCount)).map((function(_, groupIndex) {
        var groupsArray = getCellGroups(groupIndex, groups);
        return getGroupsObjectFromGroupsArray(groupsArray)
    }))
};
exports.getAllGroups = getAllGroups;
var getResourceByField = function(fieldName, loadedResources) {
    for (var i = 0; i < loadedResources.length; i++) {
        var resource = loadedResources[i];
        if (resource.name === fieldName) {
            return resource.data
        }
    }
    return []
};
exports.getResourceByField = getResourceByField;
var createResourceEditorModel = function(resources, loadedResources) {
    return resources.map((function(resource) {
        var dataField = getFieldExpr(resource);
        var dataSource = getResourceByField(dataField, loadedResources);
        return {
            editorOptions: {
                dataSource: dataSource.length ? dataSource : getWrappedDataSource(resource.dataSource),
                displayExpr: getDisplayExpr(resource),
                valueExpr: getValueExpr(resource)
            },
            dataField: dataField,
            editorType: resource.allowMultiple ? "dxTagBox" : "dxSelectBox",
            label: {
                text: resource.label || dataField
            }
        }
    }))
};
exports.createResourceEditorModel = createResourceEditorModel;
var isResourceMultiple = function(resources, resourceField) {
    var resource = resources.find((function(resource) {
        var field = getFieldExpr(resource);
        return field === resourceField
    }));
    return !!(null !== resource && void 0 !== resource && resource.allowMultiple)
};
exports.isResourceMultiple = isResourceMultiple;
var filterResources = function(resources, fields) {
    return resources.filter((function(resource) {
        var field = getFieldExpr(resource);
        return fields.indexOf(field) > -1
    }))
};
exports.filterResources = filterResources;
var getPaintedResources = function(resources, groups) {
    var newGroups = groups || [];
    var result = resources.find((function(resource) {
        return resource.useColorAsDefault
    }));
    if (result) {
        return result
    }
    var newResources = newGroups.length ? filterResources(resources, newGroups) : resources;
    return newResources[newResources.length - 1]
};
exports.getPaintedResources = getPaintedResources;
var getOrLoadResourceItem = function(resources, resourceLoaderMap, field, value) {
    var result = new _deferred.Deferred;
    resources.forEach((function(resource) {
        var resourceField = getFieldExpr(resource);
        if (resourceField === field) {
            var dataSource = getWrappedDataSource(resource.dataSource);
            var valueExpr = getValueExpr(resource);
            if (!resourceLoaderMap.has(field)) {
                resourceLoaderMap.set(field, dataSource.load())
            }
            resourceLoaderMap.get(field).done((function(data) {
                var filteredData = (0, _query.default)(data).filter(valueExpr, value).toArray();
                result.resolve(filteredData[0])
            })).fail((function() {
                resourceLoaderMap.delete(field);
                result.reject()
            }))
        }
    }));
    return result.promise()
};
exports.getOrLoadResourceItem = getOrLoadResourceItem;
var getDataAccessors = function(dataAccessors, fieldName, type) {
    var actions = dataAccessors[type];
    return actions[fieldName]
};
exports.getDataAccessors = getDataAccessors;
var groupAppointmentsByResources = function(config, appointments) {
    var groups = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : [];
    var result = {
        0: appointments
    };
    if (groups.length && config.loadedResources.length) {
        result = groupAppointmentsByResourcesCore(config, appointments, config.loadedResources)
    }
    var totalResourceCount = 0;
    config.loadedResources.forEach((function(resource, index) {
        if (!index) {
            totalResourceCount = resource.items.length
        } else {
            totalResourceCount *= resource.items.length
        }
    }));
    for (var index = 0; index < totalResourceCount; index++) {
        var key = index.toString();
        if (result[key]) {
            continue
        }
        result[key] = []
    }
    return result
};
exports.groupAppointmentsByResources = groupAppointmentsByResources;
var groupAppointmentsByResourcesCore = function(config, appointments, resources) {
    var tree = createResourcesTree(resources);
    var result = {};
    appointments.forEach((function(appointment) {
        var treeLeaves = getResourceTreeLeaves((function(field, action) {
            return getDataAccessors(config.dataAccessors, field, action)
        }), tree, appointment);
        for (var i = 0; i < treeLeaves.length; i++) {
            if (!result[treeLeaves[i]]) {
                result[treeLeaves[i]] = []
            }
            result[treeLeaves[i]].push((0, _object.deepExtendArraySafe)({}, appointment, true))
        }
    }));
    return result
};
exports.groupAppointmentsByResourcesCore = groupAppointmentsByResourcesCore;
var getResourceTreeLeaves = function getResourceTreeLeaves(getDataAccessors, tree, rawAppointment, result) {
    result = result || [];
    for (var i = 0; i < tree.length; i++) {
        if (!hasGroupItem(getDataAccessors, rawAppointment, tree[i].name, tree[i].value)) {
            continue
        }
        if ((0, _type.isDefined)(tree[i].leafIndex)) {
            result.push(tree[i].leafIndex)
        }
        if (tree[i].children) {
            getResourceTreeLeaves(getDataAccessors, tree[i].children, rawAppointment, result)
        }
    }
    return result
};
exports.getResourceTreeLeaves = getResourceTreeLeaves;
var hasGroupItem = function(getDataAccessors, rawAppointment, groupName, itemValue) {
    var resourceValue = getDataAccessors(groupName, "getter")(rawAppointment);
    var groups = (0, _array.wrapToArray)(resourceValue);
    if (groups) {
        if ((0, _array.inArray)(itemValue, groups) > -1) {
            return true
        }
    }
    return false
};
var createReducedResourcesTree = function(loadedResources, getDataAccessors, appointments) {
    var tree = createResourcesTree(loadedResources);
    return reduceResourcesTree(getDataAccessors, tree, appointments)
};
exports.createReducedResourcesTree = createReducedResourcesTree;
var reduceResourcesTree = function reduceResourcesTree(getDataAccessors, tree, existingAppointments, _result) {
    _result = _result ? _result.children : [];
    tree.forEach((function(node, index) {
        var ok = false;
        var resourceName = node.name;
        var resourceValue = node.value;
        var resourceTitle = node.title;
        var resourceData = node.data;
        var resourceGetter = getDataAccessors(resourceName, "getter");
        existingAppointments.forEach((function(appointment) {
            if (!ok) {
                var resourceFromAppointment = resourceGetter(appointment);
                if (Array.isArray(resourceFromAppointment)) {
                    if (resourceFromAppointment.indexOf(resourceValue) > -1) {
                        _result.push({
                            name: resourceName,
                            value: resourceValue,
                            title: resourceTitle,
                            data: resourceData,
                            children: []
                        });
                        ok = true
                    }
                } else if (resourceFromAppointment === resourceValue) {
                    _result.push({
                        name: resourceName,
                        value: resourceValue,
                        title: resourceTitle,
                        data: resourceData,
                        children: []
                    });
                    ok = true
                }
            }
        }));
        if (ok && node.children && node.children.length) {
            reduceResourcesTree(getDataAccessors, node.children, existingAppointments, _result[index])
        }
    }));
    return _result
};
exports.reduceResourcesTree = reduceResourcesTree;
var getResourcesDataByGroups = function(loadedResources, resources, groups) {
    if (!groups || !groups.length) {
        return loadedResources
    }
    var fieldNames = {};
    var currentResourcesData = [];
    groups.forEach((function(group) {
        (0, _iterator.each)(group, (function(name, value) {
            return fieldNames[name] = value
        }))
    }));
    var resourceData = loadedResources.filter((function(_ref2) {
        var name = _ref2.name;
        return (0, _type.isDefined)(fieldNames[name])
    }));
    resourceData.forEach((function(data) {
        return currentResourcesData.push((0, _extend.extend)({}, data))
    }));
    currentResourcesData.forEach((function(currentResource) {
        var items = currentResource.items,
            data = currentResource.data,
            resourceName = currentResource.name;
        var resource = filterResources(resources, [resourceName])[0] || {};
        var valueExpr = getValueExpr(resource);
        var filteredItems = [];
        var filteredData = [];
        groups.filter((function(group) {
            return (0, _type.isDefined)(group[resourceName])
        })).forEach((function(group) {
            (0, _iterator.each)(group, (function(name, value) {
                if (!filteredItems.filter((function(item) {
                        return item.id === value && item[valueExpr] === name
                    })).length) {
                    var currentItems = items.filter((function(item) {
                        return item.id === value
                    }));
                    var currentData = data.filter((function(item) {
                        return item[valueExpr] === value
                    }));
                    filteredItems.push.apply(filteredItems, _toConsumableArray(currentItems));
                    filteredData.push.apply(filteredData, _toConsumableArray(currentData))
                }
            }))
        }));
        currentResource.items = filteredItems;
        currentResource.data = filteredData
    }));
    return currentResourcesData
};
exports.getResourcesDataByGroups = getResourcesDataByGroups;
var setResourceToAppointment = function(resources, dataAccessors, appointment, groups) {
    var resourcesSetter = dataAccessors.setter;
    for (var name in groups) {
        var resourceData = groups[name];
        var value = isResourceMultiple(resources, name) ? (0, _array.wrapToArray)(resourceData) : resourceData;
        resourcesSetter[name](appointment, value)
    }
};
exports.setResourceToAppointment = setResourceToAppointment;
var getResourceColor = function(resources, resourceLoaderMap, field, value) {
    var result = new _deferred.Deferred;
    var resource = filterResources(resources, [field])[0] || {};
    var colorExpr = resource.colorExpr || "color";
    var colorGetter = (0, _data.compileGetter)(colorExpr);
    getOrLoadResourceItem(resources, resourceLoaderMap, field, value).done((function(resource) {
        return result.resolve(colorGetter(resource))
    })).fail((function() {
        return result.reject()
    }));
    return result.promise()
};
exports.getResourceColor = getResourceColor;
var getAppointmentColor = function(resourceConfig, appointmentConfig) {
    var resources = resourceConfig.resources,
        dataAccessors = resourceConfig.dataAccessors,
        loadedResources = resourceConfig.loadedResources,
        resourceLoaderMap = resourceConfig.resourceLoaderMap;
    var groupIndex = appointmentConfig.groupIndex,
        groups = appointmentConfig.groups,
        itemData = appointmentConfig.itemData;
    var paintedResources = getPaintedResources(resources || [], groups);
    if (paintedResources) {
        var field = getFieldExpr(paintedResources);
        var cellGroups = getCellGroups(groupIndex, loadedResources);
        var resourceValues = (0, _array.wrapToArray)(getDataAccessors(dataAccessors, field, "getter")(itemData));
        var groupId = resourceValues[0];
        for (var i = 0; i < cellGroups.length; i++) {
            if (cellGroups[i].name === field) {
                groupId = cellGroups[i].id;
                break
            }
        }
        return getResourceColor(resources, resourceLoaderMap, field, groupId)
    }
    return (new _deferred.Deferred).resolve().promise()
};
exports.getAppointmentColor = getAppointmentColor;
var createExpressions = function() {
    var resources = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : [];
    var result = {
        getter: {},
        setter: {}
    };
    resources.forEach((function(resource) {
        var field = getFieldExpr(resource);
        result.getter[field] = (0, _data.compileGetter)(field);
        result.setter[field] = (0, _data.compileSetter)(field)
    }));
    return result
};
exports.createExpressions = createExpressions;
var getTransformedResourceData = function(resource, data) {
    var valueGetter = (0, _data.compileGetter)(getValueExpr(resource));
    var displayGetter = (0, _data.compileGetter)(getDisplayExpr(resource));
    return data.map((function(item) {
        var result = {
            id: valueGetter(item),
            text: displayGetter(item)
        };
        if (item.color) {
            result.color = item.color
        }
        return result
    }))
};
var loadResources = function(groups, resources, resourceLoaderMap) {
    var result = new _deferred.Deferred;
    var deferreds = [];
    var newGroups = groups || [];
    var newResources = resources || [];
    var loadedResources = [];
    filterResources(newResources, newGroups).forEach((function(resource) {
        var deferred = new _deferred.Deferred;
        var name = getFieldExpr(resource);
        deferreds.push(deferred);
        var dataSourcePromise = getWrappedDataSource(resource.dataSource).load();
        resourceLoaderMap.set(name, dataSourcePromise);
        dataSourcePromise.done((function(data) {
            var items = getTransformedResourceData(resource, data);
            deferred.resolve({
                name: name,
                items: items,
                data: data
            })
        })).fail((function() {
            return deferred.reject()
        }))
    }));
    if (!deferreds.length) {
        return result.resolve(loadedResources)
    }
    _deferred.when.apply(null, deferreds).done((function() {
        for (var _len = arguments.length, resources = new Array(_len), _key = 0; _key < _len; _key++) {
            resources[_key] = arguments[_key]
        }
        var hasEmpty = resources.some((function(r) {
            return 0 === r.items.length
        }));
        loadedResources = hasEmpty ? [] : resources;
        result.resolve(loadedResources)
    })).fail((function() {
        return result.reject()
    }));
    return result.promise()
};
exports.loadResources = loadResources;
