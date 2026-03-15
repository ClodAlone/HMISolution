/**
 * DevExtreme (cjs/ui/gantt/ui.gantt.view.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.GanttView = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _ui = _interopRequireDefault(require("../widget/ui.widget"));
var _gantt_importer = require("./gantt_importer");
var _uiGanttTaskArea = require("./ui.gantt.task.area.container");
var _date = _interopRequireDefault(require("../../localization/date"));
var _type = require("../../core/utils/type");
var _message = _interopRequireDefault(require("../../localization/message"));
var _string = require("../../core/utils/string");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var GanttView = function(_Widget) {
    _inheritsLoose(GanttView, _Widget);

    function GanttView() {
        return _Widget.apply(this, arguments) || this
    }
    var _proto = GanttView.prototype;
    _proto._init = function() {
        _Widget.prototype._init.call(this);
        this._onSelectionChanged = this._createActionByOption("onSelectionChanged");
        this._onScroll = this._createActionByOption("onScroll");
        this._onDialogShowing = this._createActionByOption("onDialogShowing");
        this._onPopupMenuShowing = this._createActionByOption("onPopupMenuShowing");
        this._onPopupMenuHiding = this._createActionByOption("onPopupMenuHiding");
        this._expandAll = this._createActionByOption("onExpandAll");
        this._collapseAll = this._createActionByOption("onCollapseAll");
        this._taskClick = this._createActionByOption("onTaskClick");
        this._taskDblClick = this._createActionByOption("onTaskDblClick");
        this._onAdjustControl = this._createActionByOption("onAdjustControl")
    };
    _proto._initMarkup = function() {
        var _GanttView = (0, _gantt_importer.getGanttViewCore)();
        this._ganttViewCore = new _GanttView(this.$element().get(0), this, {
            showResources: this.option("showResources"),
            showDependencies: this.option("showDependencies"),
            taskTitlePosition: this._getTaskTitlePosition(this.option("taskTitlePosition")),
            firstDayOfWeek: this._getFirstDayOfWeek(this.option("firstDayOfWeek")),
            allowSelectTask: this.option("allowSelection"),
            startDateRange: this.option("startDateRange"),
            endDateRange: this.option("endDateRange"),
            editing: this._parseEditingSettings(this.option("editing")),
            validation: this.option("validation"),
            stripLines: {
                stripLines: this.option("stripLines")
            },
            areHorizontalBordersEnabled: this.option("showRowLines"),
            areAlternateRowsEnabled: false,
            viewType: this._getViewTypeByScaleType(this.option("scaleType")),
            viewTypeRange: this._parseViewTypeRangeSettings(this.option("scaleTypeRange")),
            cultureInfo: this._getCultureInfo(),
            taskTooltipContentTemplate: this.option("taskTooltipContentTemplate"),
            taskProgressTooltipContentTemplate: this.option("taskProgressTooltipContentTemplate"),
            taskTimeTooltipContentTemplate: this.option("taskTimeTooltipContentTemplate"),
            taskContentTemplate: this.option("taskContentTemplate"),
            sieve: this.option("sieve")
        });
        this._selectTask(this.option("selectedRowKey"));
        this.updateBarItemsState()
    };
    _proto._getFirstDayOfWeek = function(value) {
        return (0, _type.isDefined)(value) ? value : _date.default.firstDayOfWeekIndex()
    };
    _proto.getTaskAreaContainer = function() {
        return this._ganttViewCore.getTaskAreaContainer()
    };
    _proto.getBarManager = function() {
        return this._ganttViewCore.barManager
    };
    _proto.executeCoreCommand = function(id) {
        var command = this._ganttViewCore.getCommandByKey(id);
        if (command) {
            command.execute()
        }
    };
    _proto.changeTaskExpanded = function(id, value) {
        this._ganttViewCore.changeTaskExpanded(id, value)
    };
    _proto.updateView = function() {
        var _this$_ganttViewCore;
        null === (_this$_ganttViewCore = this._ganttViewCore) || void 0 === _this$_ganttViewCore ? void 0 : _this$_ganttViewCore.updateView()
    };
    _proto.updateBarItemsState = function() {
        this._ganttViewCore.barManager.updateItemsState([])
    };
    _proto.setWidth = function(value) {
        this._ganttViewCore.setWidth(value)
    };
    _proto._onDimensionChanged = function() {
        this._ganttViewCore.onBrowserWindowResize()
    };
    _proto._selectTask = function(id) {
        this._ganttViewCore.selectTaskById(id)
    };
    _proto._update = function(keepExpandState) {
        var _this$_ganttViewCore2;
        null === (_this$_ganttViewCore2 = this._ganttViewCore) || void 0 === _this$_ganttViewCore2 ? void 0 : _this$_ganttViewCore2.updateWithDataReload(keepExpandState)
    };
    _proto._getCultureInfo = function() {
        return {
            monthNames: _date.default.getMonthNames("wide"),
            dayNames: _date.default.getDayNames("wide"),
            abbrMonthNames: _date.default.getMonthNames("abbreviated"),
            abbrDayNames: _date.default.getDayNames("abbreviated"),
            quarterNames: this._getQuarterNames(),
            amText: _date.default.getPeriodNames()[0],
            pmText: _date.default.getPeriodNames()[1]
        }
    };
    _proto._getQuarterNames = function() {
        var quarterFormat = _message.default.format("dxGantt-quarter");
        if (!quarterFormat) {
            return _date.default.getQuarterNames()
        }
        return [(0, _string.format)(quarterFormat, 1), (0, _string.format)(quarterFormat, 2), (0, _string.format)(quarterFormat, 3), (0, _string.format)(quarterFormat, 4)]
    };
    _proto._getTaskTitlePosition = function(value) {
        switch (value) {
            case "outside":
                return 1;
            case "none":
                return 2;
            default:
                return 0
        }
    };
    _proto._getViewTypeByScaleType = function(scaleType) {
        switch (scaleType) {
            case "minutes":
                return 0;
            case "hours":
                return 1;
            case "days":
                return 3;
            case "weeks":
                return 4;
            case "months":
                return 5;
            case "quarters":
                return 6;
            case "years":
                return 7;
            default:
                return
        }
    };
    _proto._parseEditingSettings = function(value) {
        return {
            enabled: value.enabled,
            allowDependencyDelete: value.allowDependencyDeleting,
            allowDependencyInsert: value.allowDependencyAdding,
            allowTaskDelete: value.allowTaskDeleting,
            allowTaskInsert: value.allowTaskAdding,
            allowTaskUpdate: value.allowTaskUpdating,
            allowResourceDelete: value.allowResourceDeleting,
            allowResourceInsert: value.allowResourceAdding,
            allowResourceUpdate: value.allowResourceUpdating,
            allowTaskResourceUpdate: value.allowTaskResourceUpdating
        }
    };
    _proto._parseViewTypeRangeSettings = function(value) {
        return {
            min: this._getViewTypeByScaleType(value.min),
            max: this._getViewTypeByScaleType(value.max)
        }
    };
    _proto._optionChanged = function(args) {
        switch (args.name) {
            case "width":
                _Widget.prototype._optionChanged.call(this, args);
                this._ganttViewCore.setWidth(args.value);
                break;
            case "height":
                this._ganttViewCore.setHeight(args.value);
                break;
            case "tasks":
            case "dependencies":
            case "resources":
            case "resourceAssignments":
                this._sieveOptions = void 0;
                this._update(true);
                break;
            case "showResources":
                this._ganttViewCore.setShowResources(args.value);
                break;
            case "showDependencies":
                this._ganttViewCore.setShowDependencies(args.value);
                break;
            case "taskTitlePosition":
                this._ganttViewCore.setTaskTitlePosition(this._getTaskTitlePosition(args.value));
                break;
            case "firstDayOfWeek":
                this._ganttViewCore.setFirstDayOfWeek(this._getFirstDayOfWeek(args.value));
                break;
            case "startDateRange":
                this._ganttViewCore.setStartDateRange(args.value);
                break;
            case "endDateRange":
                this._ganttViewCore.setEndDateRange(args.value);
                break;
            case "allowSelection":
                this._ganttViewCore.setAllowSelection(args.value);
                break;
            case "selectedRowKey":
                this._selectTask(args.value);
                break;
            case "editing":
                this._ganttViewCore.setEditingSettings(this._parseEditingSettings(args.value));
                break;
            case "validation":
                this._ganttViewCore.setValidationSettings(args.value);
                this._update(true);
                break;
            case "showRowLines":
                this._ganttViewCore.setRowLinesVisible(args.value);
                break;
            case "scaleType":
                this._ganttViewCore.setViewType(this._getViewTypeByScaleType(args.value));
                break;
            case "scaleTypeRange":
                this._ganttViewCore.setViewTypeRange(this._getViewTypeByScaleType(args.value.min), this._getViewTypeByScaleType(args.value.max));
                break;
            case "stripLines":
                this._ganttViewCore.setStripLines({
                    stripLines: args.value
                });
                break;
            case "taskTooltipContentTemplate":
                this._ganttViewCore.setTaskTooltipContentTemplate(args.value);
                break;
            case "taskProgressTooltipContentTemplate":
                this._ganttViewCore.setTaskProgressTooltipContentTemplate(args.value);
                break;
            case "taskTimeTooltipContentTemplate":
                this._ganttViewCore.setTaskTimeTooltipContentTemplate(args.value);
                break;
            case "taskContentTemplate":
                this._ganttViewCore.setTaskContentTemplate(args.value);
                break;
            case "sieve":
                this._sortAndFilter(args.value);
                break;
            default:
                _Widget.prototype._optionChanged.call(this, args)
        }
    };
    _proto.getRowHeight = function() {
        return this.option("rowHeight")
    };
    _proto.getHeaderHeight = function() {
        return this.option("headerHeight")
    };
    _proto.getGanttTasksData = function() {
        var tasks = this.option("tasks");
        var sieveOptions = this.getSieveOptions();
        if (null !== sieveOptions && void 0 !== sieveOptions && sieveOptions.sievedItems && null !== sieveOptions && void 0 !== sieveOptions && sieveOptions.sieveColumn) {
            return sieveOptions.sievedItems
        }
        return tasks
    };
    _proto._sortAndFilter = function(args) {
        this._sieveOptions = args;
        this._update(true);
        var selectedRowKey = this.option("selectedRowKey");
        this._selectTask(selectedRowKey)
    };
    _proto.getSieveOptions = function() {
        return this._sieveOptions
    };
    _proto.getGanttDependenciesData = function() {
        return this.option("dependencies")
    };
    _proto.getGanttResourcesData = function() {
        return this.option("resources")
    };
    _proto.getGanttResourceAssignmentsData = function() {
        return this.option("resourceAssignments")
    };
    _proto.getGanttWorkTimeRules = function() {
        return null
    };
    _proto.getExternalTaskAreaContainer = function(element) {
        if (!this._taskAreaContainer) {
            this._taskAreaContainer = new _uiGanttTaskArea.TaskAreaContainer(element, this)
        }
        return this._taskAreaContainer
    };
    _proto.prepareExternalTaskAreaContainer = function(element, info) {
        if (null !== info && void 0 !== info && info.height) {
            this._taskAreaContainer._scrollView.option("height", info.height)
        }
    };
    _proto.changeGanttTaskSelection = function(id, selected) {
        this._onSelectionChanged({
            id: id,
            selected: selected
        })
    };
    _proto.onGanttScroll = function(scrollTop) {
        this._onScroll({
            scrollTop: scrollTop
        })
    };
    _proto.showDialog = function(name, parameters, callback, afterClosing) {
        this._onDialogShowing({
            name: name,
            parameters: parameters,
            callback: callback,
            afterClosing: afterClosing
        })
    };
    _proto.getModelChangesListener = function() {
        return this.option("modelChangesListener")
    };
    _proto.getExportInfo = function() {
        return this.option("exportInfo")
    };
    _proto.showPopupMenu = function(info) {
        this._onPopupMenuShowing(info)
    };
    _proto.hidePopupMenu = function(info) {
        this._onPopupMenuHiding(info)
    };
    _proto.getMainElement = function() {
        return this.option("mainElement").get(0)
    };
    _proto.adjustControl = function() {
        this._onAdjustControl()
    };
    _proto.getRequireFirstLoadParentAutoCalc = function() {
        return this.option("validation.autoUpdateParentTasks")
    };
    _proto.collapseAll = function() {
        this._collapseAll()
    };
    _proto.expandAll = function() {
        this._expandAll()
    };
    _proto.onTaskClick = function(key, event) {
        this._taskClick({
            key: key,
            event: event
        });
        return true
    };
    _proto.onTaskDblClick = function(key, event) {
        return this._taskDblClick({
            key: key,
            event: event
        })
    };
    _proto.onGanttViewContextMenu = function(event, key, type) {
        return true
    };
    _proto.getFormattedDateText = function(date) {
        var result = "";
        if (date) {
            var datePart = _date.default.format(date, "shortDate");
            var timePart = _date.default.format(date, "hh:mm");
            result = datePart + " " + timePart
        }
        return result
    };
    _proto.destroyTemplate = function(container) {
        (0, _renderer.default)(container).empty()
    };
    _proto.onTaskAreaSizeChanged = function(info) {
        var scrollView = this._taskAreaContainer._scrollView;
        if ((0, _type.isDefined)(null === info || void 0 === info ? void 0 : info.height)) {
            var direction = (null === info || void 0 === info ? void 0 : info.height) > this._taskAreaContainer.getHeight() ? "both" : "horizontal";
            scrollView.option("direction", direction)
        }
    };
    _proto.getTreeListTableStyle = function() {
        return this.callExportHelperMethod("getTreeListTableStyle")
    };
    _proto.getTreeListColCount = function() {
        return this.callExportHelperMethod("getTreeListColCount")
    };
    _proto.getTreeListHeaderInfo = function(colIndex) {
        return this.callExportHelperMethod("getTreeListHeaderInfo", colIndex)
    };
    _proto.getTreeListCellInfo = function(rowIndex, colIndex, key) {
        return this.callExportHelperMethod("getTreeListCellInfo", key, colIndex)
    };
    _proto.callExportHelperMethod = function(methodName) {
        var helper = this.option("exportHelper");
        for (var _len = arguments.length, args = new Array(_len > 1 ? _len - 1 : 0), _key = 1; _key < _len; _key++) {
            args[_key - 1] = arguments[_key]
        }
        return helper[methodName].apply(helper, args)
    };
    _createClass(GanttView, [{
        key: "bars",
        get: function() {
            return this.option("bars")
        }
    }]);
    return GanttView
}(_ui.default);
exports.GanttView = GanttView;
