/**
 * DevExtreme (cjs/ui/gantt/ui.gantt.actions.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.GanttActionsManager = void 0;
var Actions = {
    onContextMenuPreparing: "onContextMenuPreparing",
    onCustomCommand: "onCustomCommand",
    onDependencyDeleted: "onDependencyDeleted",
    onDependencyDeleting: "onDependencyDeleting",
    onDependencyInserted: "onDependencyInserted",
    onDependencyInserting: "onDependencyInserting",
    onResourceAssigned: "onResourceAssigned",
    onResourceAssigning: "onResourceAssigning",
    onResourceDeleted: "onResourceDeleted",
    onResourceDeleting: "onResourceDeleting",
    onResourceInserted: "onResourceInserted",
    onResourceInserting: "onResourceInserting",
    onResourceManagerDialogShowing: "onResourceManagerDialogShowing",
    onResourceUnassigned: "onResourceUnassigned",
    onResourceUnassigning: "onResourceUnassigning",
    onSelectionChanged: "onSelectionChanged",
    onTaskClick: "onTaskClick",
    onTaskDblClick: "onTaskDblClick",
    onTaskDeleted: "onTaskDeleted",
    onTaskDeleting: "onTaskDeleting",
    onTaskEditDialogShowing: "onTaskEditDialogShowing",
    onTaskInserted: "onTaskInserted",
    onTaskInserting: "onTaskInserting",
    onTaskMoving: "onTaskMoving",
    onTaskUpdated: "onTaskUpdated",
    onTaskUpdating: "onTaskUpdating"
};
var GANTT_TASKS = "tasks";
var GANTT_DEPENDENCIES = "dependencies";
var GANTT_RESOURCES = "resources";
var GANTT_RESOURCE_ASSIGNMENTS = "resourceAssignments";
var GANTT_NEW_TASK_CACHE_KEY = "gantt_new_task_key";
var GanttActionsManager = function() {
    function GanttActionsManager(gantt) {
        this._gantt = gantt;
        this._mappingHelper = gantt._mappingHelper;
        this._customFieldsManager = gantt._customFieldsManager
    }
    var _proto = GanttActionsManager.prototype;
    _proto._createActionByOption = function(optionName) {
        return this._gantt._createActionByOption(optionName)
    };
    _proto._getTaskData = function(key) {
        return this._gantt.getTaskData(key)
    };
    _proto._convertCoreToMappedData = function(optionName, coreData) {
        return this._mappingHelper.convertCoreToMappedData(optionName, coreData)
    };
    _proto._convertMappedToCoreData = function(optionName, mappedData) {
        return this._mappingHelper.convertMappedToCoreData(optionName, mappedData)
    };
    _proto._convertMappedToCoreFields = function(optionName, fields) {
        return this._mappingHelper.convertMappedToCoreFields(optionName, fields)
    };
    _proto._convertCoreToMappedFields = function(optionName, fields) {
        return this._mappingHelper.convertCoreToMappedFields(optionName, fields)
    };
    _proto._saveCustomFieldsDataToCache = function(key, data) {
        var forceUpdateOnKeyExpire = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : false;
        var isCustomFieldsUpdateOnly = arguments.length > 3 && void 0 !== arguments[3] ? arguments[3] : false;
        this._customFieldsManager.saveCustomFieldsDataToCache(key, data, forceUpdateOnKeyExpire, isCustomFieldsUpdateOnly)
    };
    _proto.createTaskDblClickAction = function() {
        this._taskDblClickAction = this._createActionByOption(Actions.onTaskDblClick)
    };
    _proto.taskDblClickAction = function(args) {
        if (!this._taskDblClickAction) {
            this.createTaskDblClickAction()
        }
        this._taskDblClickAction(args)
    };
    _proto.raiseTaskDblClickAction = function(key, event) {
        var args = {
            cancel: false,
            data: this._getTaskData(key),
            event: event,
            key: key
        };
        this.taskDblClickAction(args);
        return !args.cancel
    };
    _proto.createTaskClickAction = function() {
        this._taskClickAction = this._createActionByOption(Actions.onTaskClick)
    };
    _proto.taskClickAction = function(args) {
        if (!this._taskClickAction) {
            this.createTaskClickAction()
        }
        this._taskClickAction(args)
    };
    _proto.raiseTaskClickAction = function(key, event) {
        var args = {
            key: key,
            event: event,
            data: this._getTaskData(key)
        };
        this.taskClickAction(args)
    };
    _proto.createSelectionChangedAction = function() {
        this._selectionChangedAction = this._createActionByOption(Actions.onSelectionChanged)
    };
    _proto.selectionChangedAction = function(args) {
        if (!this._selectionChangedAction) {
            this.createSelectionChangedAction()
        }
        this._selectionChangedAction(args)
    };
    _proto.raiseSelectionChangedAction = function(selectedRowKey) {
        this.selectionChangedAction({
            selectedRowKey: selectedRowKey
        })
    };
    _proto.createCustomCommandAction = function() {
        this._customCommandAction = this._createActionByOption(Actions.onCustomCommand)
    };
    _proto.customCommandAction = function(args) {
        if (!this._customCommandAction) {
            this.createCustomCommandAction()
        }
        this._customCommandAction(args)
    };
    _proto.raiseCustomCommand = function(commandName) {
        this.customCommandAction({
            name: commandName
        })
    };
    _proto.createContextMenuPreparingAction = function() {
        this._contextMenuPreparingAction = this._createActionByOption(Actions.onContextMenuPreparing)
    };
    _proto.contextMenuPreparingAction = function(args) {
        if (!this._contextMenuPreparingAction) {
            this.createContextMenuPreparingAction()
        }
        this._contextMenuPreparingAction(args)
    };
    _proto.raiseContextMenuPreparing = function(options) {
        this.contextMenuPreparingAction(options)
    };
    _proto._getInsertingAction = function(optionName) {
        switch (optionName) {
            case GANTT_TASKS:
                return this._getTaskInsertingAction();
            case GANTT_DEPENDENCIES:
                return this._getDependencyInsertingAction();
            case GANTT_RESOURCES:
                return this._getResourceInsertingAction();
            case GANTT_RESOURCE_ASSIGNMENTS:
                return this._getResourceAssigningAction()
        }
        return function() {}
    };
    _proto.raiseInsertingAction = function(optionName, coreArgs) {
        var action = this._getInsertingAction(optionName);
        if (action) {
            var args = {
                cancel: false,
                values: this._convertCoreToMappedData(optionName, coreArgs.values)
            };
            action(args);
            coreArgs.cancel = args.cancel;
            coreArgs.values = this._convertMappedToCoreData(optionName, args.values);
            if (optionName === GANTT_TASKS) {
                this._saveCustomFieldsDataToCache(GANTT_NEW_TASK_CACHE_KEY, args.values)
            }
        }
    };
    _proto.createTaskInsertingAction = function() {
        this._taskInsertingAction = this._createActionByOption(Actions.onTaskInserting)
    };
    _proto.taskInsertingAction = function(args) {
        var action = this._getTaskInsertingAction();
        action(args)
    };
    _proto._getTaskInsertingAction = function() {
        if (!this._taskInsertingAction) {
            this.createTaskInsertingAction()
        }
        return this._taskInsertingAction
    };
    _proto.createDependencyInsertingAction = function() {
        this._dependencyInsertingAction = this._createActionByOption(Actions.onDependencyInserting)
    };
    _proto.dependencyInsertingAction = function(args) {
        var action = this._getDependencyInsertingAction();
        action(args)
    };
    _proto._getDependencyInsertingAction = function() {
        if (!this._dependencyInsertingAction) {
            this.createDependencyInsertingAction()
        }
        return this._dependencyInsertingAction
    };
    _proto.createResourceInsertingAction = function() {
        this._resourceInsertingAction = this._createActionByOption(Actions.onResourceInserting)
    };
    _proto.resourceInsertingAction = function(args) {
        var action = this._getResourceInsertingAction();
        action(args)
    };
    _proto._getResourceInsertingAction = function() {
        if (!this._resourceInsertingAction) {
            this.createResourceInsertingAction()
        }
        return this._resourceInsertingAction
    };
    _proto.createResourceAssigningAction = function() {
        this._resourceAssigningAction = this._createActionByOption(Actions.onResourceAssigning)
    };
    _proto.resourceAssigningAction = function(args) {
        var action = this._getResourceAssigningAction();
        action(args)
    };
    _proto._getResourceAssigningAction = function() {
        if (!this._resourceAssigningAction) {
            this.createResourceAssigningAction()
        }
        return this._resourceAssigningAction
    };
    _proto._getInsertedAction = function(optionName) {
        switch (optionName) {
            case GANTT_TASKS:
                return this._getTaskInsertedAction();
            case GANTT_DEPENDENCIES:
                return this._getDependencyInsertedAction();
            case GANTT_RESOURCES:
                return this._getResourceInsertedAction();
            case GANTT_RESOURCE_ASSIGNMENTS:
                return this._getResourceAssignedAction()
        }
        return function() {}
    };
    _proto.raiseInsertedAction = function(optionName, data, key) {
        var action = this._getInsertedAction(optionName);
        if (action) {
            var args = {
                values: data,
                key: key
            };
            action(args)
        }
    };
    _proto.createTaskInsertedAction = function() {
        this._taskInsertedAction = this._createActionByOption(Actions.onTaskInserted)
    };
    _proto.taskInsertedAction = function(args) {
        var action = this._getTaskInsertedAction();
        action(args)
    };
    _proto._getTaskInsertedAction = function() {
        if (!this._taskInsertedAction) {
            this.createTaskInsertedAction()
        }
        return this._taskInsertedAction
    };
    _proto.createDependencyInsertedAction = function() {
        this._dependencyInsertedAction = this._createActionByOption(Actions.onDependencyInserted)
    };
    _proto.dependencyInsertedAction = function(args) {
        var action = this._getDependencyInsertedAction();
        action(args)
    };
    _proto._getDependencyInsertedAction = function() {
        if (!this._dependencyInsertedAction) {
            this.createDependencyInsertedAction()
        }
        return this._dependencyInsertedAction
    };
    _proto.createResourceInsertedAction = function() {
        this._resourceInsertedAction = this._createActionByOption(Actions.onResourceInserted)
    };
    _proto.resourceInsertedAction = function(args) {
        var action = this._getResourceInsertedAction();
        action(args)
    };
    _proto._getResourceInsertedAction = function() {
        if (!this._resourceInsertedAction) {
            this.createResourceInsertedAction()
        }
        return this._resourceInsertedAction
    };
    _proto.createResourceAssignedAction = function() {
        this._resourceAssignedAction = this._createActionByOption(Actions.onResourceAssigned)
    };
    _proto.resourceAssignedAction = function(args) {
        var action = this._getResourceAssignedAction();
        action(args)
    };
    _proto._getResourceAssignedAction = function() {
        if (!this._resourceAssignedAction) {
            this.createResourceAssignedAction()
        }
        return this._resourceAssignedAction
    };
    _proto._getDeletingAction = function(optionName) {
        switch (optionName) {
            case GANTT_TASKS:
                return this._getTaskDeletingAction();
            case GANTT_DEPENDENCIES:
                return this._getDependencyDeletingAction();
            case GANTT_RESOURCES:
                return this._getResourceDeletingAction();
            case GANTT_RESOURCE_ASSIGNMENTS:
                return this._getResourceUnassigningAction()
        }
        return function() {}
    };
    _proto.raiseDeletingAction = function(optionName, coreArgs) {
        var action = this._getDeletingAction(optionName);
        if (action) {
            var args = {
                cancel: false,
                key: coreArgs.key,
                values: this._convertCoreToMappedData(optionName, coreArgs.values)
            };
            action(args);
            coreArgs.cancel = args.cancel
        }
    };
    _proto.createTaskDeletingAction = function() {
        this._taskDeletingAction = this._createActionByOption(Actions.onTaskDeleting)
    };
    _proto.taskDeletingAction = function(args) {
        var action = this._getTaskDeletingAction();
        action(args)
    };
    _proto._getTaskDeletingAction = function() {
        if (!this._taskDeletingAction) {
            this.createTaskDeletingAction()
        }
        return this._taskDeletingAction
    };
    _proto.createDependencyDeletingAction = function() {
        this._dependencyDeletingAction = this._createActionByOption(Actions.onDependencyDeleting)
    };
    _proto.dependencyDeletingAction = function(args) {
        var action = this._getDependencyDeletingAction();
        action(args)
    };
    _proto._getDependencyDeletingAction = function() {
        if (!this._dependencyDeletingAction) {
            this.createDependencyDeletingAction()
        }
        return this._dependencyDeletingAction
    };
    _proto.createResourceDeletingAction = function() {
        this._resourceDeletingAction = this._createActionByOption(Actions.onResourceDeleting)
    };
    _proto.resourceDeletingAction = function(args) {
        var action = this._getResourceDeletingAction();
        action(args)
    };
    _proto._getResourceDeletingAction = function() {
        if (!this._resourceDeletingAction) {
            this.createResourceDeletingAction()
        }
        return this._resourceDeletingAction
    };
    _proto.createResourceUnassigningAction = function() {
        this._resourceUnassigningAction = this._createActionByOption(Actions.onResourceUnassigning)
    };
    _proto.resourceUnassigningAction = function(args) {
        var action = this._getResourceUnassigningAction();
        action(args)
    };
    _proto._getResourceUnassigningAction = function() {
        if (!this._resourceUnassigningAction) {
            this.createResourceUnassigningAction()
        }
        return this._resourceUnassigningAction
    };
    _proto._getDeletedAction = function(optionName) {
        switch (optionName) {
            case GANTT_TASKS:
                return this._getTaskDeletedAction();
            case GANTT_DEPENDENCIES:
                return this._getDependencyDeletedAction();
            case GANTT_RESOURCES:
                return this._getResourceDeletedAction();
            case GANTT_RESOURCE_ASSIGNMENTS:
                return this._getResourceUnassignedAction()
        }
        return function() {}
    };
    _proto.raiseDeletedAction = function(optionName, key, data) {
        var action = this._getDeletedAction(optionName);
        if (action) {
            var args = {
                key: key,
                values: data
            };
            action(args)
        }
    };
    _proto.createTaskDeletedAction = function() {
        this._taskDeletedAction = this._createActionByOption(Actions.onTaskDeleted)
    };
    _proto.taskDeletedAction = function(args) {
        var action = this._getTaskDeletedAction();
        action(args)
    };
    _proto._getTaskDeletedAction = function() {
        if (!this._taskDeletedAction) {
            this.createTaskDeletedAction()
        }
        return this._taskDeletedAction
    };
    _proto.createDependencyDeletedAction = function() {
        this._dependencyDeletedAction = this._createActionByOption(Actions.onDependencyDeleted)
    };
    _proto.dependencyDeletedAction = function(args) {
        var action = this._getDependencyDeletedAction();
        action(args)
    };
    _proto._getDependencyDeletedAction = function() {
        if (!this._dependencyDeletedAction) {
            this.createDependencyDeletedAction()
        }
        return this._dependencyDeletedAction
    };
    _proto.createResourceDeletedAction = function() {
        this._resourceDeletedAction = this._createActionByOption(Actions.onResourceDeleted)
    };
    _proto.resourceDeletedAction = function(args) {
        var action = this._getResourceDeletedAction();
        action(args)
    };
    _proto._getResourceDeletedAction = function() {
        if (!this._resourceDeletedAction) {
            this.createResourceDeletedAction()
        }
        return this._resourceDeletedAction
    };
    _proto.createResourceUnassignedAction = function() {
        this._resourceUnassignedAction = this._createActionByOption(Actions.onResourceUnassigned)
    };
    _proto.resourceUnassignedAction = function(args) {
        var action = this._getResourceUnassignedAction();
        action(args)
    };
    _proto._getResourceUnassignedAction = function() {
        if (!this._resourceUnassignedAction) {
            this.createResourceUnassignedAction()
        }
        return this._resourceUnassignedAction
    };
    _proto._getUpdatingAction = function(optionName) {
        switch (optionName) {
            case GANTT_TASKS:
                return this._getTaskUpdatingAction()
        }
        return function() {}
    };
    _proto.raiseUpdatingAction = function(optionName, coreArgs, action) {
        action = action || this._getUpdatingAction(optionName);
        if (action) {
            var isTaskUpdating = optionName === GANTT_TASKS;
            var args = {
                cancel: false,
                key: coreArgs.key,
                newValues: this._convertCoreToMappedData(optionName, coreArgs.newValues),
                values: isTaskUpdating ? this._getTaskData(coreArgs.key) : this._convertCoreToMappedData(optionName, coreArgs.values)
            };
            if (isTaskUpdating && this._customFieldsManager.cache.hasData(args.key)) {
                this._customFieldsManager.addCustomFieldsDataFromCache(args.key, args.newValues)
            }
            action(args);
            coreArgs.cancel = args.cancel;
            coreArgs.newValues = this._convertMappedToCoreData(optionName, args.newValues);
            if (isTaskUpdating) {
                if (args.cancel) {
                    this._customFieldsManager.resetCustomFieldsDataCache(args.key)
                } else {
                    var forceUpdateOnKeyExpire = !Object.keys(coreArgs.newValues).length;
                    this._saveCustomFieldsDataToCache(args.key, args.newValues, forceUpdateOnKeyExpire)
                }
            }
        }
    };
    _proto.createTaskUpdatingAction = function() {
        this._taskUpdatingAction = this._createActionByOption(Actions.onTaskUpdating)
    };
    _proto.taskUpdatingAction = function(args) {
        var action = this._getTaskUpdatingAction();
        action(args)
    };
    _proto._getTaskUpdatingAction = function() {
        if (!this._taskUpdatingAction) {
            this.createTaskUpdatingAction()
        }
        return this._taskUpdatingAction
    };
    _proto._getUpdatedAction = function(optionName) {
        switch (optionName) {
            case GANTT_TASKS:
                return this._getTaskUpdatedAction()
        }
        return function() {}
    };
    _proto.raiseUpdatedAction = function(optionName, data, key) {
        var action = this._getUpdatedAction(optionName);
        if (action) {
            var args = {
                values: data,
                key: key
            };
            action(args)
        }
    };
    _proto.createTaskUpdatedAction = function() {
        this._taskUpdatedAction = this._createActionByOption(Actions.onTaskUpdated)
    };
    _proto.taskUpdatedAction = function(args) {
        var action = this._getTaskUpdatedAction();
        action(args)
    };
    _proto._getTaskUpdatedAction = function() {
        if (!this._taskUpdatedAction) {
            this.createTaskUpdatedAction()
        }
        return this._taskUpdatedAction
    };
    _proto.createTaskEditDialogShowingAction = function() {
        this._taskEditDialogShowingAction = this._createActionByOption(Actions.onTaskEditDialogShowing)
    };
    _proto.taskEditDialogShowingAction = function(args) {
        var action = this._getTaskEditDialogShowingAction();
        action(args)
    };
    _proto._getTaskEditDialogShowingAction = function() {
        if (!this._taskEditDialogShowingAction) {
            this.createTaskEditDialogShowingAction()
        }
        return this._taskEditDialogShowingAction
    };
    _proto.raiseTaskEditDialogShowingAction = function(coreArgs) {
        var action = this._getTaskEditDialogShowingAction();
        if (action) {
            var args = {
                cancel: false,
                key: coreArgs.key,
                values: this._convertCoreToMappedData(GANTT_TASKS, coreArgs.values),
                readOnlyFields: this._convertCoreToMappedFields(GANTT_TASKS, coreArgs.readOnlyFields),
                hiddenFields: this._convertCoreToMappedFields(GANTT_TASKS, coreArgs.hiddenFields)
            };
            action(args);
            coreArgs.cancel = args.cancel;
            coreArgs.values = this._convertMappedToCoreData(GANTT_TASKS, args.values);
            coreArgs.readOnlyFields = this._convertMappedToCoreFields(GANTT_TASKS, args.readOnlyFields);
            coreArgs.hiddenFields = this._convertMappedToCoreFields(GANTT_TASKS, args.hiddenFields)
        }
    };
    _proto.createResourceManagerDialogShowingAction = function() {
        this._resourceManagerDialogShowingAction = this._createActionByOption(Actions.onResourceManagerDialogShowing)
    };
    _proto.resourceManagerDialogShowingAction = function(args) {
        var action = this._getResourceManagerDialogShowingAction();
        action(args)
    };
    _proto._getResourceManagerDialogShowingAction = function() {
        if (!this._resourceManagerDialogShowingAction) {
            this.createResourceManagerDialogShowingAction()
        }
        return this._resourceManagerDialogShowingAction
    };
    _proto.raiseResourceManagerDialogShowingAction = function(coreArgs) {
        var _this = this;
        var action = this._getResourceManagerDialogShowingAction();
        if (action) {
            var mappedResources = coreArgs.values.resources.items.map((function(r) {
                return _this._convertMappedToCoreData(GANTT_RESOURCES, r)
            }));
            var args = {
                cancel: false,
                values: mappedResources
            };
            action(args);
            coreArgs.cancel = args.cancel
        }
    };
    _proto.createTaskMovingAction = function() {
        this._taskMovingAction = this._createActionByOption(Actions.onTaskMoving)
    };
    _proto.taskMovingAction = function(args) {
        var action = this.getTaskMovingAction();
        action(args)
    };
    _proto.getTaskMovingAction = function() {
        if (!this._taskMovingAction) {
            this.createTaskMovingAction()
        }
        return this._taskMovingAction
    };
    return GanttActionsManager
}();
exports.GanttActionsManager = GanttActionsManager;
