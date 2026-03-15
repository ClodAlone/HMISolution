/**
 * DevExtreme (cjs/ui/gantt/ui.gantt.helper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.GanttHelper = void 0;
var _data = require("../../core/utils/data");
var _message = _interopRequireDefault(require("../../localization/message"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var GanttHelper = {
    prepareMapHandler: function(getters) {
        return function(data) {
            return Object.keys(getters).reduce((function(previous, key) {
                var resultKey = "key" === key ? "id" : key;
                previous[resultKey] = getters[key](data);
                return previous
            }), {})
        }
    },
    prepareSetterMapHandler: function(setters) {
        return function(data) {
            return Object.keys(setters).reduce((function(previous, key) {
                var resultKey = "key" === key ? "id" : key;
                setters[key](previous, data[resultKey]);
                return previous
            }), {})
        }
    },
    compileGettersByOption: function(optionValue) {
        var getters = {};
        for (var field in optionValue) {
            var exprMatches = field.match(/(\w*)Expr/);
            if (exprMatches) {
                getters[exprMatches[1]] = (0, _data.compileGetter)(optionValue[exprMatches[0]])
            }
        }
        return getters
    },
    compileSettersByOption: function(optionValue) {
        var setters = {};
        for (var field in optionValue) {
            var exprMatches = field.match(/(\w*)Expr/);
            if (exprMatches) {
                setters[exprMatches[1]] = (0, _data.compileSetter)(optionValue[exprMatches[0]])
            }
        }
        return setters
    },
    getStoreObject: function(option, modelObject) {
        var setters = GanttHelper.compileSettersByOption(option);
        return Object.keys(setters).reduce((function(previous, key) {
            if ("key" !== key) {
                setters[key](previous, modelObject[key])
            }
            return previous
        }), {})
    },
    getInvertedData: function(data, keyGetter) {
        var inverted = {};
        if (data) {
            for (var i = 0; i < data.length; i++) {
                var dataItem = data[i];
                var key = keyGetter(dataItem);
                inverted[key] = dataItem
            }
        }
        return inverted
    },
    getArrayFromOneElement: function(element) {
        return void 0 === element || null === element ? [] : [element]
    },
    getSelectionMode: function(allowSelection) {
        return allowSelection ? "single" : "none"
    },
    convertTreeToList: function(node, array) {
        if (null !== node && void 0 !== node && node.data && null !== node && void 0 !== node && node.visible) {
            array.push(node.data)
        }
        for (var i = 0; i < (null === (_node$children = node.children) || void 0 === _node$children ? void 0 : _node$children.length); i++) {
            var _node$children;
            var child = node.children[i];
            GanttHelper.convertTreeToList(child, array)
        }
    },
    getAllParentNodesKeys: function(node, array) {
        var _node$parent;
        if (null !== node && void 0 !== node && node.data) {
            array.push(node.key)
        }
        if (null !== node && void 0 !== node && null !== (_node$parent = node.parent) && void 0 !== _node$parent && _node$parent.data) {
            GanttHelper.getAllParentNodesKeys(node.parent, array)
        }
    },
    getDefaultOptions: function() {
        return {
            tasks: {
                dataSource: null,
                keyExpr: "id",
                parentIdExpr: "parentId",
                startExpr: "start",
                endExpr: "end",
                progressExpr: "progress",
                titleExpr: "title",
                colorExpr: "color"
            },
            dependencies: {
                dataSource: null,
                keyExpr: "id",
                predecessorIdExpr: "predecessorId",
                successorIdExpr: "successorId",
                typeExpr: "type"
            },
            resources: {
                dataSource: null,
                keyExpr: "id",
                textExpr: "text",
                colorExpr: "color"
            },
            resourceAssignments: {
                dataSource: null,
                keyExpr: "id",
                taskIdExpr: "taskId",
                resourceIdExpr: "resourceId"
            },
            columns: void 0,
            taskListWidth: 300,
            showResources: true,
            showDependencies: true,
            taskTitlePosition: "inside",
            firstDayOfWeek: void 0,
            selectedRowKey: void 0,
            onSelectionChanged: null,
            onTaskClick: null,
            onTaskDblClick: null,
            onTaskInserting: null,
            onTaskInserted: null,
            onTaskDeleting: null,
            onTaskDeleted: null,
            onTaskUpdating: null,
            onTaskUpdated: null,
            onTaskMoving: null,
            onTaskEditDialogShowing: null,
            onDependencyInserting: null,
            onDependencyInserted: null,
            onDependencyDeleting: null,
            onDependencyDeleted: null,
            onResourceInserting: null,
            onResourceInserted: null,
            onResourceDeleting: null,
            onResourceDeleted: null,
            onResourceAssigning: null,
            onResourceAssigned: null,
            onResourceUnassigning: null,
            onResourceUnassigned: null,
            onCustomCommand: null,
            onContextMenuPreparing: null,
            allowSelection: true,
            showRowLines: true,
            stripLines: void 0,
            scaleType: "auto",
            scaleTypeRange: {
                min: "minutes",
                max: "years"
            },
            editing: {
                enabled: false,
                allowTaskAdding: true,
                allowTaskDeleting: true,
                allowTaskUpdating: true,
                allowDependencyAdding: true,
                allowDependencyDeleting: true,
                allowResourceAdding: true,
                allowResourceDeleting: true,
                allowResourceUpdating: true,
                allowTaskResourceUpdating: true
            },
            validation: {
                validateDependencies: false,
                autoUpdateParentTasks: false,
                enablePredecessorGap: false
            },
            toolbar: null,
            contextMenu: {
                enabled: true,
                items: void 0
            },
            taskTooltipContentTemplate: null,
            taskProgressTooltipContentTemplate: null,
            taskTimeTooltipContentTemplate: null,
            taskContentTemplate: null,
            rootValue: 0,
            sorting: {
                ascendingText: _message.default.format("dxGantt-sortingAscendingText"),
                descendingText: _message.default.format("dxGantt-sortingDescendingText"),
                clearText: _message.default.format("dxGantt-sortingClearText"),
                mode: "single",
                showSortIndexes: false
            },
            filterRow: void 0,
            headerFilter: void 0
        }
    }
};
exports.GanttHelper = GanttHelper;
