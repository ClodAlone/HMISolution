/**
 * DevExtreme (esm/ui/gantt/ui.gantt.actions.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
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
export class GanttActionsManager {
    constructor(gantt) {
        this._gantt = gantt;
        this._mappingHelper = gantt._mappingHelper;
        this._customFieldsManager = gantt._customFieldsManager
    }
    _createActionByOption(optionName) {
        return this._gantt._createActionByOption(optionName)
    }
    _getTaskData(key) {
        return this._gantt.getTaskData(key)
    }
    _convertCoreToMappedData(optionName, coreData) {
        return this._mappingHelper.convertCoreToMappedData(optionName, coreData)
    }
    _convertMappedToCoreData(optionName, mappedData) {
        return this._mappingHelper.convertMappedToCoreData(optionName, mappedData)
    }
    _convertMappedToCoreFields(optionName, fields) {
        return this._mappingHelper.convertMappedToCoreFields(optionName, fields)
    }
    _convertCoreToMappedFields(optionName, fields) {
        return this._mappingHelper.convertCoreToMappedFields(optionName, fields)
    }
    _saveCustomFieldsDataToCache(key, data) {
        var forceUpdateOnKeyExpire = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : false;
        var isCustomFieldsUpdateOnly = arguments.length > 3 && void 0 !== arguments[3] ? arguments[3] : false;
        this._customFieldsManager.saveCustomFieldsDataToCache(key, data, forceUpdateOnKeyExpire, isCustomFieldsUpdateOnly)
    }
    createTaskDblClickAction() {
        this._taskDblClickAction = this._createActionByOption(Actions.onTaskDblClick)
    }
    taskDblClickAction(args) {
        if (!this._taskDblClickAction) {
            this.createTaskDblClickAction()
        }
        this._taskDblClickAction(args)
    }
    raiseTaskDblClickAction(key, event) {
        var args = {
            cancel: false,
            data: this._getTaskData(key),
            event: event,
            key: key
        };
        this.taskDblClickAction(args);
        return !args.cancel
    }
    createTaskClickAction() {
        this._taskClickAction = this._createActionByOption(Actions.onTaskClick)
    }
    taskClickAction(args) {
        if (!this._taskClickAction) {
            this.createTaskClickAction()
        }
        this._taskClickAction(args)
    }
    raiseTaskClickAction(key, event) {
        var args = {
            key: key,
            event: event,
            data: this._getTaskData(key)
        };
        this.taskClickAction(args)
    }
    createSelectionChangedAction() {
        this._selectionChangedAction = this._createActionByOption(Actions.onSelectionChanged)
    }
    selectionChangedAction(args) {
        if (!this._selectionChangedAction) {
            this.createSelectionChangedAction()
        }
        this._selectionChangedAction(args)
    }
    raiseSelectionChangedAction(selectedRowKey) {
        this.selectionChangedAction({
            selectedRowKey: selectedRowKey
        })
    }
    createCustomCommandAction() {
        this._customCommandAction = this._createActionByOption(Actions.onCustomCommand)
    }
    customCommandAction(args) {
        if (!this._customCommandAction) {
            this.createCustomCommandAction()
        }
        this._customCommandAction(args)
    }
    raiseCustomCommand(commandName) {
        this.customCommandAction({
            name: commandName
        })
    }
    createContextMenuPreparingAction() {
        this._contextMenuPreparingAction = this._createActionByOption(Actions.onContextMenuPreparing)
    }
    contextMenuPreparingAction(args) {
        if (!this._contextMenuPreparingAction) {
            this.createContextMenuPreparingAction()
        }
        this._contextMenuPreparingAction(args)
    }
    raiseContextMenuPreparing(options) {
        this.contextMenuPreparingAction(options)
    }
    _getInsertingAction(optionName) {
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
        return () => {}
    }
    raiseInsertingAction(optionName, coreArgs) {
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
    }
    createTaskInsertingAction() {
        this._taskInsertingAction = this._createActionByOption(Actions.onTaskInserting)
    }
    taskInsertingAction(args) {
        var action = this._getTaskInsertingAction();
        action(args)
    }
    _getTaskInsertingAction() {
        if (!this._taskInsertingAction) {
            this.createTaskInsertingAction()
        }
        return this._taskInsertingAction
    }
    createDependencyInsertingAction() {
        this._dependencyInsertingAction = this._createActionByOption(Actions.onDependencyInserting)
    }
    dependencyInsertingAction(args) {
        var action = this._getDependencyInsertingAction();
        action(args)
    }
    _getDependencyInsertingAction() {
        if (!this._dependencyInsertingAction) {
            this.createDependencyInsertingAction()
        }
        return this._dependencyInsertingAction
    }
    createResourceInsertingAction() {
        this._resourceInsertingAction = this._createActionByOption(Actions.onResourceInserting)
    }
    resourceInsertingAction(args) {
        var action = this._getResourceInsertingAction();
        action(args)
    }
    _getResourceInsertingAction() {
        if (!this._resourceInsertingAction) {
            this.createResourceInsertingAction()
        }
        return this._resourceInsertingAction
    }
    createResourceAssigningAction() {
        this._resourceAssigningAction = this._createActionByOption(Actions.onResourceAssigning)
    }
    resourceAssigningAction(args) {
        var action = this._getResourceAssigningAction();
        action(args)
    }
    _getResourceAssigningAction() {
        if (!this._resourceAssigningAction) {
            this.createResourceAssigningAction()
        }
        return this._resourceAssigningAction
    }
    _getInsertedAction(optionName) {
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
        return () => {}
    }
    raiseInsertedAction(optionName, data, key) {
        var action = this._getInsertedAction(optionName);
        if (action) {
            var args = {
                values: data,
                key: key
            };
            action(args)
        }
    }
    createTaskInsertedAction() {
        this._taskInsertedAction = this._createActionByOption(Actions.onTaskInserted)
    }
    taskInsertedAction(args) {
        var action = this._getTaskInsertedAction();
        action(args)
    }
    _getTaskInsertedAction() {
        if (!this._taskInsertedAction) {
            this.createTaskInsertedAction()
        }
        return this._taskInsertedAction
    }
    createDependencyInsertedAction() {
        this._dependencyInsertedAction = this._createActionByOption(Actions.onDependencyInserted)
    }
    dependencyInsertedAction(args) {
        var action = this._getDependencyInsertedAction();
        action(args)
    }
    _getDependencyInsertedAction() {
        if (!this._dependencyInsertedAction) {
            this.createDependencyInsertedAction()
        }
        return this._dependencyInsertedAction
    }
    createResourceInsertedAction() {
        this._resourceInsertedAction = this._createActionByOption(Actions.onResourceInserted)
    }
    resourceInsertedAction(args) {
        var action = this._getResourceInsertedAction();
        action(args)
    }
    _getResourceInsertedAction() {
        if (!this._resourceInsertedAction) {
            this.createResourceInsertedAction()
        }
        return this._resourceInsertedAction
    }
    createResourceAssignedAction() {
        this._resourceAssignedAction = this._createActionByOption(Actions.onResourceAssigned)
    }
    resourceAssignedAction(args) {
        var action = this._getResourceAssignedAction();
        action(args)
    }
    _getResourceAssignedAction() {
        if (!this._resourceAssignedAction) {
            this.createResourceAssignedAction()
        }
        return this._resourceAssignedAction
    }
    _getDeletingAction(optionName) {
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
        return () => {}
    }
    raiseDeletingAction(optionName, coreArgs) {
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
    }
    createTaskDeletingAction() {
        this._taskDeletingAction = this._createActionByOption(Actions.onTaskDeleting)
    }
    taskDeletingAction(args) {
        var action = this._getTaskDeletingAction();
        action(args)
    }
    _getTaskDeletingAction() {
        if (!this._taskDeletingAction) {
            this.createTaskDeletingAction()
        }
        return this._taskDeletingAction
    }
    createDependencyDeletingAction() {
        this._dependencyDeletingAction = this._createActionByOption(Actions.onDependencyDeleting)
    }
    dependencyDeletingAction(args) {
        var action = this._getDependencyDeletingAction();
        action(args)
    }
    _getDependencyDeletingAction() {
        if (!this._dependencyDeletingAction) {
            this.createDependencyDeletingAction()
        }
        return this._dependencyDeletingAction
    }
    createResourceDeletingAction() {
        this._resourceDeletingAction = this._createActionByOption(Actions.onResourceDeleting)
    }
    resourceDeletingAction(args) {
        var action = this._getResourceDeletingAction();
        action(args)
    }
    _getResourceDeletingAction() {
        if (!this._resourceDeletingAction) {
            this.createResourceDeletingAction()
        }
        return this._resourceDeletingAction
    }
    createResourceUnassigningAction() {
        this._resourceUnassigningAction = this._createActionByOption(Actions.onResourceUnassigning)
    }
    resourceUnassigningAction(args) {
        var action = this._getResourceUnassigningAction();
        action(args)
    }
    _getResourceUnassigningAction() {
        if (!this._resourceUnassigningAction) {
            this.createResourceUnassigningAction()
        }
        return this._resourceUnassigningAction
    }
    _getDeletedAction(optionName) {
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
        return () => {}
    }
    raiseDeletedAction(optionName, key, data) {
        var action = this._getDeletedAction(optionName);
        if (action) {
            var args = {
                key: key,
                values: data
            };
            action(args)
        }
    }
    createTaskDeletedAction() {
        this._taskDeletedAction = this._createActionByOption(Actions.onTaskDeleted)
    }
    taskDeletedAction(args) {
        var action = this._getTaskDeletedAction();
        action(args)
    }
    _getTaskDeletedAction() {
        if (!this._taskDeletedAction) {
            this.createTaskDeletedAction()
        }
        return this._taskDeletedAction
    }
    createDependencyDeletedAction() {
        this._dependencyDeletedAction = this._createActionByOption(Actions.onDependencyDeleted)
    }
    dependencyDeletedAction(args) {
        var action = this._getDependencyDeletedAction();
        action(args)
    }
    _getDependencyDeletedAction() {
        if (!this._dependencyDeletedAction) {
            this.createDependencyDeletedAction()
        }
        return this._dependencyDeletedAction
    }
    createResourceDeletedAction() {
        this._resourceDeletedAction = this._createActionByOption(Actions.onResourceDeleted)
    }
    resourceDeletedAction(args) {
        var action = this._getResourceDeletedAction();
        action(args)
    }
    _getResourceDeletedAction() {
        if (!this._resourceDeletedAction) {
            this.createResourceDeletedAction()
        }
        return this._resourceDeletedAction
    }
    createResourceUnassignedAction() {
        this._resourceUnassignedAction = this._createActionByOption(Actions.onResourceUnassigned)
    }
    resourceUnassignedAction(args) {
        var action = this._getResourceUnassignedAction();
        action(args)
    }
    _getResourceUnassignedAction() {
        if (!this._resourceUnassignedAction) {
            this.createResourceUnassignedAction()
        }
        return this._resourceUnassignedAction
    }
    _getUpdatingAction(optionName) {
        switch (optionName) {
            case GANTT_TASKS:
                return this._getTaskUpdatingAction()
        }
        return () => {}
    }
    raiseUpdatingAction(optionName, coreArgs, action) {
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
    }
    createTaskUpdatingAction() {
        this._taskUpdatingAction = this._createActionByOption(Actions.onTaskUpdating)
    }
    taskUpdatingAction(args) {
        var action = this._getTaskUpdatingAction();
        action(args)
    }
    _getTaskUpdatingAction() {
        if (!this._taskUpdatingAction) {
            this.createTaskUpdatingAction()
        }
        return this._taskUpdatingAction
    }
    _getUpdatedAction(optionName) {
        switch (optionName) {
            case GANTT_TASKS:
                return this._getTaskUpdatedAction()
        }
        return () => {}
    }
    raiseUpdatedAction(optionName, data, key) {
        var action = this._getUpdatedAction(optionName);
        if (action) {
            var args = {
                values: data,
                key: key
            };
            action(args)
        }
    }
    createTaskUpdatedAction() {
        this._taskUpdatedAction = this._createActionByOption(Actions.onTaskUpdated)
    }
    taskUpdatedAction(args) {
        var action = this._getTaskUpdatedAction();
        action(args)
    }
    _getTaskUpdatedAction() {
        if (!this._taskUpdatedAction) {
            this.createTaskUpdatedAction()
        }
        return this._taskUpdatedAction
    }
    createTaskEditDialogShowingAction() {
        this._taskEditDialogShowingAction = this._createActionByOption(Actions.onTaskEditDialogShowing)
    }
    taskEditDialogShowingAction(args) {
        var action = this._getTaskEditDialogShowingAction();
        action(args)
    }
    _getTaskEditDialogShowingAction() {
        if (!this._taskEditDialogShowingAction) {
            this.createTaskEditDialogShowingAction()
        }
        return this._taskEditDialogShowingAction
    }
    raiseTaskEditDialogShowingAction(coreArgs) {
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
    }
    createResourceManagerDialogShowingAction() {
        this._resourceManagerDialogShowingAction = this._createActionByOption(Actions.onResourceManagerDialogShowing)
    }
    resourceManagerDialogShowingAction(args) {
        var action = this._getResourceManagerDialogShowingAction();
        action(args)
    }
    _getResourceManagerDialogShowingAction() {
        if (!this._resourceManagerDialogShowingAction) {
            this.createResourceManagerDialogShowingAction()
        }
        return this._resourceManagerDialogShowingAction
    }
    raiseResourceManagerDialogShowingAction(coreArgs) {
        var action = this._getResourceManagerDialogShowingAction();
        if (action) {
            var mappedResources = coreArgs.values.resources.items.map(r => this._convertMappedToCoreData(GANTT_RESOURCES, r));
            var args = {
                cancel: false,
                values: mappedResources
            };
            action(args);
            coreArgs.cancel = args.cancel
        }
    }
    createTaskMovingAction() {
        this._taskMovingAction = this._createActionByOption(Actions.onTaskMoving)
    }
    taskMovingAction(args) {
        var action = this.getTaskMovingAction();
        action(args)
    }
    getTaskMovingAction() {
        if (!this._taskMovingAction) {
            this.createTaskMovingAction()
        }
        return this._taskMovingAction
    }
}
