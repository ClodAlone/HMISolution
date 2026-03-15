/**
 * DevExtreme (renovation/ui/scheduler/workspaces/base/work_space.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.prepareGenerationOptions = exports.WorkSpace = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _combine_classes = require("../../../../utils/combine_classes");
var _ordinary_layout = require("./ordinary_layout");
var _uiScheduler = require("../../../../../ui/scheduler/workspaces/ui.scheduler.virtual_scrolling");
var _view_data_provider = _interopRequireDefault(require("../../../../../ui/scheduler/workspaces/view_model/view_data_provider"));
var _utils = require("./utils");
var _props = require("../props");
var _work_space_config = require("./work_space_config");
var _utils2 = require("../utils");
var _cross_scrolling_layout = require("./cross_scrolling_layout");
var _utils3 = require("../../../../../ui/scheduler/workspaces/view_model/utils");
var _base = require("../../view_model/to_test/views/utils/base");
var _date_header_data_generator = require("../../../../../ui/scheduler/workspaces/view_model/date_header_data_generator");
var _time_panel_data_generator = require("../../../../../ui/scheduler/workspaces/view_model/time_panel_data_generator");
var _utils4 = require("../../view_model/group_panel/utils");
var _excluded = ["accessKey", "activeStateEnabled", "allDayAppointments", "allDayPanelExpanded", "allowMultipleCellSelection", "appointments", "cellDuration", "className", "crossScrollingEnabled", "currentDate", "dataCellTemplate", "dateCellTemplate", "disabled", "endDayHour", "firstDayOfWeek", "focusStateEnabled", "groupByDate", "groupOrientation", "groups", "height", "hint", "hoursInterval", "hoverStateEnabled", "indicatorTime", "indicatorUpdateInterval", "intervalCount", "onClick", "onKeyDown", "onViewRendered", "resourceCellTemplate", "rtlEnabled", "schedulerHeight", "schedulerWidth", "scrolling", "selectedCellData", "shadeUntilCurrentTime", "showAllDayPanel", "showCurrentTimeIndicator", "startDate", "startDayHour", "tabIndex", "timeCellTemplate", "type", "visible", "width"];

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _objectWithoutProperties(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = _objectWithoutPropertiesLoose(source, excluded);
    var key, i;
    if (Object.getOwnPropertySymbols) {
        var sourceSymbolKeys = Object.getOwnPropertySymbols(source);
        for (i = 0; i < sourceSymbolKeys.length; i++) {
            key = sourceSymbolKeys[i];
            if (excluded.indexOf(key) >= 0) {
                continue
            }
            if (!Object.prototype.propertyIsEnumerable.call(source, key)) {
                continue
            }
            target[key] = source[key]
        }
    }
    return target
}

function _objectWithoutPropertiesLoose(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = {};
    var sourceKeys = Object.keys(source);
    var key, i;
    for (i = 0; i < sourceKeys.length; i++) {
        key = sourceKeys[i];
        if (excluded.indexOf(key) >= 0) {
            continue
        }
        target[key] = source[key]
    }
    return target
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

function _slicedToArray(arr, i) {
    return _arrayWithHoles(arr) || _iterableToArrayLimit(arr, i) || _unsupportedIterableToArray(arr, i) || _nonIterableRest()
}

function _nonIterableRest() {
    throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
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

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}

function _iterableToArrayLimit(arr, i) {
    var _i = null == arr ? null : "undefined" !== typeof Symbol && arr[Symbol.iterator] || arr["@@iterator"];
    if (null == _i) {
        return
    }
    var _arr = [];
    var _n = true;
    var _d = false;
    var _s, _e;
    try {
        for (_i = _i.call(arr); !(_n = (_s = _i.next()).done); _n = true) {
            _arr.push(_s.value);
            if (i && _arr.length === i) {
                break
            }
        }
    } catch (err) {
        _d = true;
        _e = err
    } finally {
        try {
            if (!_n && null != _i.return) {
                _i.return()
            }
        } finally {
            if (_d) {
                throw _e
            }
        }
    }
    return _arr
}

function _arrayWithHoles(arr) {
    if (Array.isArray(arr)) {
        return arr
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

function _assertThisInitialized(self) {
    if (void 0 === self) {
        throw new ReferenceError("this hasn't been initialised - super() hasn't been called")
    }
    return self
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
var defaultVirtualScrollingMetaData = {
    cellHeight: 50,
    cellWidth: _utils.DATE_TABLE_MIN_CELL_WIDTH,
    viewWidth: 300,
    viewHeight: 300,
    scrollableWidth: 300
};
var calculateDefaultVirtualScrollingState = function(options) {
    var completeColumnCount = options.completeViewDataMap[0].length;
    var completeRowCount = options.completeViewDataMap.length;
    options.virtualScrollingDispatcher.setViewOptions((0, _utils.createVirtualScrollingOptions)({
        cellHeight: defaultVirtualScrollingMetaData.cellHeight,
        cellWidth: defaultVirtualScrollingMetaData.cellWidth,
        schedulerHeight: options.schedulerHeight,
        schedulerWidth: options.schedulerWidth,
        viewHeight: defaultVirtualScrollingMetaData.viewHeight,
        viewWidth: defaultVirtualScrollingMetaData.viewWidth,
        scrolling: options.scrolling,
        scrollableWidth: defaultVirtualScrollingMetaData.scrollableWidth,
        groups: options.groups,
        isVerticalGrouping: options.isVerticalGrouping,
        completeRowCount: completeRowCount,
        completeColumnCount: completeColumnCount
    }));
    options.virtualScrollingDispatcher.createVirtualScrolling();
    options.virtualScrollingDispatcher.updateDimensions(true);
    return options.virtualScrollingDispatcher.getRenderState()
};
var prepareGenerationOptions = function(workSpaceProps, renderConfig, isAllDayPanelVisible, virtualStartIndices) {
    var cellDuration = workSpaceProps.cellDuration,
        currentDate = workSpaceProps.currentDate,
        endDayHour = workSpaceProps.endDayHour,
        firstDayOfWeek = workSpaceProps.firstDayOfWeek,
        groupByDate = workSpaceProps.groupByDate,
        groupOrientation = workSpaceProps.groupOrientation,
        groups = workSpaceProps.groups,
        hoursInterval = workSpaceProps.hoursInterval,
        intervalCount = workSpaceProps.intervalCount,
        startDate = workSpaceProps.startDate,
        startDayHour = workSpaceProps.startDayHour,
        type = workSpaceProps.type;
    var getDateForHeaderText = renderConfig.getDateForHeaderText,
        headerCellTextFormat = renderConfig.headerCellTextFormat,
        isGenerateWeekDaysHeaderData = renderConfig.isGenerateWeekDaysHeaderData,
        isProvideVirtualCellsWidth = renderConfig.isProvideVirtualCellsWidth,
        isRenderTimePanel = renderConfig.isRenderTimePanel;
    return {
        startRowIndex: virtualStartIndices.startRowIndex,
        startCellIndex: virtualStartIndices.startCellIndex,
        groupOrientation: groupOrientation,
        groupByDate: groupByDate,
        groups: groups,
        isProvideVirtualCellsWidth: isProvideVirtualCellsWidth,
        isAllDayPanelVisible: isAllDayPanelVisible,
        selectedCells: void 0,
        focusedCell: void 0,
        headerCellTextFormat: headerCellTextFormat,
        getDateForHeaderText: getDateForHeaderText,
        startDayHour: startDayHour,
        endDayHour: endDayHour,
        cellDuration: cellDuration,
        viewType: type,
        intervalCount: intervalCount,
        hoursInterval: hoursInterval,
        currentDate: currentDate,
        startDate: startDate,
        firstDayOfWeek: firstDayOfWeek,
        isGenerateTimePanelData: isRenderTimePanel,
        isGenerateWeekDaysHeaderData: isGenerateWeekDaysHeaderData
    }
};
exports.prepareGenerationOptions = prepareGenerationOptions;
var viewFunction = function(_ref) {
    var allDayPanelRef = _ref.allDayPanelRef,
        classes = _ref.classes,
        dateHeaderData = _ref.dateHeaderData,
        dateTableRef = _ref.dateTableRef,
        dateTableTemplate = _ref.dateTableTemplate,
        groupOrientation = _ref.groupOrientation,
        groupPanelData = _ref.groupPanelData,
        groupPanelHeight = _ref.groupPanelHeight,
        groupPanelRef = _ref.groupPanelRef,
        headerEmptyCellWidth = _ref.headerEmptyCellWidth,
        headerPanelTemplate = _ref.headerPanelTemplate,
        isAllDayPanelVisible = _ref.isAllDayPanelVisible,
        isGroupedByDate = _ref.isGroupedByDate,
        isRenderHeaderEmptyCell = _ref.isRenderHeaderEmptyCell,
        isStandaloneAllDayPanel = _ref.isStandaloneAllDayPanel,
        isVerticalGrouping = _ref.isVerticalGrouping,
        Layout = _ref.layout,
        layoutRef = _ref.layoutRef,
        onScroll = _ref.onScroll,
        _ref$props = _ref.props,
        allDayAppointments = _ref$props.allDayAppointments,
        allDayPanelExpanded = _ref$props.allDayPanelExpanded,
        appointments = _ref$props.appointments,
        dataCellTemplate = _ref$props.dataCellTemplate,
        dateCellTemplate = _ref$props.dateCellTemplate,
        groups = _ref$props.groups,
        intervalCount = _ref$props.intervalCount,
        resourceCellTemplate = _ref$props.resourceCellTemplate,
        timeCellTemplate = _ref$props.timeCellTemplate,
        _ref$renderConfig = _ref.renderConfig,
        groupPanelClassName = _ref$renderConfig.groupPanelClassName,
        isRenderDateHeader = _ref$renderConfig.isRenderDateHeader,
        scrollingDirection = _ref$renderConfig.scrollingDirection,
        tablesWidth = _ref.tablesWidth,
        timePanelData = _ref.timePanelData,
        timePanelRef = _ref.timePanelRef,
        timePanelTemplate = _ref.timePanelTemplate,
        viewData = _ref.viewData,
        widgetElementRef = _ref.widgetElementRef;
    return (0, _inferno.createComponentVNode)(2, Layout, {
        viewData: viewData,
        dateHeaderData: dateHeaderData,
        timePanelData: timePanelData,
        groupPanelData: groupPanelData,
        dataCellTemplate: dataCellTemplate,
        dateCellTemplate: dateCellTemplate,
        timeCellTemplate: timeCellTemplate,
        resourceCellTemplate: resourceCellTemplate,
        groups: groups,
        groupByDate: isGroupedByDate,
        groupOrientation: groupOrientation,
        groupPanelClassName: groupPanelClassName,
        intervalCount: intervalCount,
        headerPanelTemplate: headerPanelTemplate,
        dateTableTemplate: dateTableTemplate,
        timePanelTemplate: timePanelTemplate,
        isAllDayPanelCollapsed: !allDayPanelExpanded,
        isAllDayPanelVisible: isAllDayPanelVisible,
        isRenderDateHeader: isRenderDateHeader,
        isRenderHeaderEmptyCell: isRenderHeaderEmptyCell,
        isRenderGroupPanel: isVerticalGrouping,
        isStandaloneAllDayPanel: isStandaloneAllDayPanel,
        scrollingDirection: scrollingDirection,
        groupPanelHeight: groupPanelHeight,
        headerEmptyCellWidth: headerEmptyCellWidth,
        tablesWidth: tablesWidth,
        onScroll: onScroll,
        className: classes,
        dateTableRef: dateTableRef,
        allDayPanelRef: allDayPanelRef,
        timePanelRef: timePanelRef,
        groupPanelRef: groupPanelRef,
        widgetElementRef: widgetElementRef,
        appointments: appointments,
        allDayAppointments: allDayAppointments
    }, null, layoutRef)
};
exports.viewFunction = viewFunction;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var WorkSpace = function(_InfernoComponent) {
    _inheritsLoose(WorkSpace, _InfernoComponent);

    function WorkSpace(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.dateTableRef = (0, _inferno.createRef)();
        _this.allDayPanelRef = (0, _inferno.createRef)();
        _this.timePanelRef = (0, _inferno.createRef)();
        _this.groupPanelRef = (0, _inferno.createRef)();
        _this.layoutRef = (0, _inferno.createRef)();
        _this.widgetElementRef = (0, _inferno.createRef)();
        _this.__getterCache = {};
        _this.state = {
            groupPanelHeight: void 0,
            headerEmptyCellWidth: void 0,
            tablesWidth: void 0,
            virtualScrolling: new _uiScheduler.VirtualScrollingDispatcher,
            virtualScrollingData: void 0
        };
        _this.groupPanelHeightEffect = _this.groupPanelHeightEffect.bind(_assertThisInitialized(_this));
        _this.headerEmptyCellWidthEffect = _this.headerEmptyCellWidthEffect.bind(_assertThisInitialized(_this));
        _this.tablesWidthEffect = _this.tablesWidthEffect.bind(_assertThisInitialized(_this));
        _this.virtualScrollingMetaDataEffect = _this.virtualScrollingMetaDataEffect.bind(_assertThisInitialized(_this));
        _this.onViewRendered = _this.onViewRendered.bind(_assertThisInitialized(_this));
        _this.createDateTableElementsMeta = _this.createDateTableElementsMeta.bind(_assertThisInitialized(_this));
        _this.createAllDayPanelElementsMeta = _this.createAllDayPanelElementsMeta.bind(_assertThisInitialized(_this));
        _this.onScroll = _this.onScroll.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = WorkSpace.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.groupPanelHeightEffect, [this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]), new _inferno2.InfernoEffect(this.headerEmptyCellWidthEffect, [this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]), new _inferno2.InfernoEffect(this.tablesWidthEffect, [this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]), new _inferno2.InfernoEffect(this.virtualScrollingMetaDataEffect, [this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]), new _inferno2.InfernoEffect(this.onViewRendered, [this.props.allDayPanelExpanded, this.props.cellDuration, this.props.crossScrollingEnabled, this.props.currentDate, this.props.endDayHour, this.props.firstDayOfWeek, this.props.groupByDate, this.props.groupOrientation, this.props.type, this.props.intervalCount, this.props.groups, this.props.hoursInterval, this.props.onViewRendered, this.props.scrolling, this.props.showAllDayPanel, this.props.startDate, this.props.startDayHour, this.state.virtualScrollingData, this.props.schedulerHeight, this.props.schedulerWidth, this.state.virtualScrolling, this.state.tablesWidth])]
    };
    _proto.updateEffects = function() {
        var _this$_effects$, _this$_effects$2, _this$_effects$3, _this$_effects$4, _this$_effects$5;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]);
        null === (_this$_effects$2 = this._effects[1]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]);
        null === (_this$_effects$3 = this._effects[2]) || void 0 === _this$_effects$3 ? void 0 : _this$_effects$3.update([this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]);
        null === (_this$_effects$4 = this._effects[3]) || void 0 === _this$_effects$4 ? void 0 : _this$_effects$4.update([this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]);
        null === (_this$_effects$5 = this._effects[4]) || void 0 === _this$_effects$5 ? void 0 : _this$_effects$5.update([this.props.allDayPanelExpanded, this.props.cellDuration, this.props.crossScrollingEnabled, this.props.currentDate, this.props.endDayHour, this.props.firstDayOfWeek, this.props.groupByDate, this.props.groupOrientation, this.props.type, this.props.intervalCount, this.props.groups, this.props.hoursInterval, this.props.onViewRendered, this.props.scrolling, this.props.showAllDayPanel, this.props.startDate, this.props.startDayHour, this.state.virtualScrollingData, this.props.schedulerHeight, this.props.schedulerWidth, this.state.virtualScrolling, this.state.tablesWidth])
    };
    _proto.groupPanelHeightEffect = function() {
        var _this2 = this;
        this.setState((function(__state_argument) {
            var _this2$dateTableRef$c;
            return {
                groupPanelHeight: null === (_this2$dateTableRef$c = _this2.dateTableRef.current) || void 0 === _this2$dateTableRef$c ? void 0 : _this2$dateTableRef$c.getBoundingClientRect().height
            }
        }))
    };
    _proto.headerEmptyCellWidthEffect = function() {
        var _this$timePanelRef$cu, _this$timePanelRef$cu2, _this$groupPanelRef$c, _this$groupPanelRef$c2;
        var timePanelWidth = null !== (_this$timePanelRef$cu = null === (_this$timePanelRef$cu2 = this.timePanelRef.current) || void 0 === _this$timePanelRef$cu2 ? void 0 : _this$timePanelRef$cu2.getBoundingClientRect().width) && void 0 !== _this$timePanelRef$cu ? _this$timePanelRef$cu : 0;
        var groupPanelWidth = null !== (_this$groupPanelRef$c = null === (_this$groupPanelRef$c2 = this.groupPanelRef.current) || void 0 === _this$groupPanelRef$c2 ? void 0 : _this$groupPanelRef$c2.getBoundingClientRect().width) && void 0 !== _this$groupPanelRef$c ? _this$groupPanelRef$c : 0;
        this.setState((function(__state_argument) {
            return {
                headerEmptyCellWidth: timePanelWidth + groupPanelWidth
            }
        }))
    };
    _proto.tablesWidthEffect = function() {
        var _this3 = this;
        if (this.isCalculateTablesWidth) {
            var _this$props = this.props,
                currentDate = _this$props.currentDate,
                endDayHour = _this$props.endDayHour,
                groups = _this$props.groups,
                hoursInterval = _this$props.hoursInterval,
                intervalCount = _this$props.intervalCount,
                startDayHour = _this$props.startDayHour,
                viewType = _this$props.type;
            this.setState((function(__state_argument) {
                return {
                    tablesWidth: (0, _utils.getDateTableWidth)(_this3.layoutRef.current.getScrollableWidth(), _this3.dateTableRef.current, _this3.viewDataProvider, {
                        intervalCount: intervalCount,
                        currentDate: currentDate,
                        viewType: viewType,
                        hoursInterval: hoursInterval,
                        startDayHour: startDayHour,
                        endDayHour: endDayHour,
                        groups: groups,
                        groupOrientation: _this3.groupOrientation
                    })
                }
            }))
        }
    };
    _proto.virtualScrollingMetaDataEffect = function() {
        var _this4 = this;
        var dateTableCell = this.dateTableRef.current.querySelector("td:not(.dx-scheduler-virtual-cell)");
        var cellRect = dateTableCell.getBoundingClientRect();
        var cellHeight = Math.floor(cellRect.height);
        var cellWidth = Math.floor(cellRect.width);
        var scrollableWidth = this.layoutRef.current.getScrollableWidth();
        var widgetRect = this.widgetElementRef.current.getBoundingClientRect();
        var viewHeight = widgetRect.height;
        var viewWidth = widgetRect.width;
        var nextSizes = {
            cellHeight: cellHeight,
            cellWidth: cellWidth,
            scrollableWidth: scrollableWidth,
            viewWidth: viewWidth,
            viewHeight: viewHeight
        };
        var isNextMetaDataNotEqualToCurrent = !this.state.virtualScrollingData || Object.entries(nextSizes).some((function(_ref2) {
            var _ref3 = _slicedToArray(_ref2, 2),
                key = _ref3[0],
                value = _ref3[1];
            return value !== _this4.state.virtualScrollingData.sizes[key]
        }));
        if (isNextMetaDataNotEqualToCurrent) {
            var _this$props2 = this.props,
                groups = _this$props2.groups,
                schedulerHeight = _this$props2.schedulerHeight,
                schedulerWidth = _this$props2.schedulerWidth,
                scrolling = _this$props2.scrolling;
            var completeColumnCount = this.completeViewDataMap[0].length;
            var completeRowCount = this.completeViewDataMap.length;
            this.state.virtualScrolling.setViewOptions((0, _utils.createVirtualScrollingOptions)({
                cellHeight: nextSizes.cellHeight,
                cellWidth: nextSizes.cellWidth,
                schedulerHeight: schedulerHeight,
                schedulerWidth: schedulerWidth,
                viewHeight: nextSizes.viewHeight,
                viewWidth: nextSizes.viewWidth,
                scrolling: scrolling,
                scrollableWidth: nextSizes.scrollableWidth,
                groups: groups,
                isVerticalGrouping: this.isVerticalGrouping,
                completeRowCount: completeRowCount,
                completeColumnCount: completeColumnCount
            }));
            this.state.virtualScrolling.createVirtualScrolling();
            this.state.virtualScrolling.updateDimensions(true);
            this.setState((function(__state_argument) {
                return {
                    virtualScrollingData: {
                        state: _this4.state.virtualScrolling.getRenderState(),
                        sizes: nextSizes
                    }
                }
            }))
        }
    };
    _proto.onViewRendered = function() {
        var _this$props3 = this.props,
            allDayPanelExpanded = _this$props3.allDayPanelExpanded,
            cellDuration = _this$props3.cellDuration,
            crossScrollingEnabled = _this$props3.crossScrollingEnabled,
            currentDate = _this$props3.currentDate,
            endDayHour = _this$props3.endDayHour,
            firstDayOfWeek = _this$props3.firstDayOfWeek,
            groupByDate = _this$props3.groupByDate,
            groupOrientation = _this$props3.groupOrientation,
            groups = _this$props3.groups,
            hoursInterval = _this$props3.hoursInterval,
            intervalCount = _this$props3.intervalCount,
            onViewRendered = _this$props3.onViewRendered,
            scrolling = _this$props3.scrolling,
            showAllDayPanel = _this$props3.showAllDayPanel,
            startDate = _this$props3.startDate,
            startDayHour = _this$props3.startDayHour,
            viewType = _this$props3.type;
        var tableWidths = (0, _utils.getDateTableWidth)(this.layoutRef.current.getScrollableWidth(), this.dateTableRef.current, this.viewDataProvider, {
            intervalCount: intervalCount,
            currentDate: currentDate,
            viewType: viewType,
            hoursInterval: hoursInterval,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            groups: groups,
            groupOrientation: this.groupOrientation
        });
        if (!this.isCalculateTablesWidth || tableWidths === this.state.tablesWidth) {
            var columnCount = this.viewDataMap.dateTableMap[0].length;
            var dateTableCellsMeta = this.createDateTableElementsMeta(columnCount);
            var allDayPanelCellsMeta = this.createAllDayPanelElementsMeta();
            onViewRendered({
                viewDataProvider: this.viewDataProvider,
                cellsMetaData: {
                    dateTableCellsMeta: dateTableCellsMeta,
                    allDayPanelCellsMeta: allDayPanelCellsMeta
                },
                viewDataProviderValidationOptions: {
                    intervalCount: intervalCount,
                    currentDate: currentDate,
                    type: viewType,
                    hoursInterval: hoursInterval,
                    startDayHour: startDayHour,
                    endDayHour: endDayHour,
                    groups: groups,
                    groupOrientation: groupOrientation,
                    groupByDate: groupByDate,
                    crossScrollingEnabled: crossScrollingEnabled,
                    firstDayOfWeek: firstDayOfWeek,
                    startDate: startDate,
                    showAllDayPanel: showAllDayPanel,
                    allDayPanelExpanded: allDayPanelExpanded,
                    scrolling: scrolling,
                    cellDuration: cellDuration
                }
            })
        }
    };
    _proto.createDateTableElementsMeta = function(totalCellCount) {
        var dateTableCells = this.dateTableRef.current.querySelectorAll("td:not(.dx-scheduler-virtual-cell)");
        var dateTableRect = this.dateTableRef.current.getBoundingClientRect();
        var dateTableCellsMeta = [];
        dateTableCells.forEach((function(cellElement, index) {
            if (index % totalCellCount === 0) {
                dateTableCellsMeta.push([])
            }
            var cellRect = cellElement.getBoundingClientRect();
            var validCellRect = (0, _utils.createCellElementMetaData)(dateTableRect, cellRect);
            dateTableCellsMeta[dateTableCellsMeta.length - 1].push(validCellRect)
        }));
        return dateTableCellsMeta
    };
    _proto.createAllDayPanelElementsMeta = function() {
        if (!this.allDayPanelRef.current) {
            return []
        }
        var allDayPanelCells = this.allDayPanelRef.current.querySelectorAll("td");
        var allDayPanelRect = this.allDayPanelRef.current.getBoundingClientRect();
        var allDayPanelCellsMeta = [];
        allDayPanelCells.forEach((function(cellElement) {
            var cellRect = cellElement.getBoundingClientRect();
            allDayPanelCellsMeta.push((0, _utils.createCellElementMetaData)(allDayPanelRect, cellRect))
        }));
        return allDayPanelCellsMeta
    };
    _proto.onScroll = function(event) {
        var _this5 = this;
        if ("virtual" === this.props.scrolling.mode) {
            this.state.virtualScrolling.handleOnScrollEvent(event.scrollOffset);
            var nextState = this.state.virtualScrolling.getRenderState();
            var isUpdateState = Object.entries(nextState).some((function(_ref4) {
                var _ref5 = _slicedToArray(_ref4, 2),
                    key = _ref5[0],
                    value = _ref5[1];
                return value !== _this5.state.virtualScrollingData.state[key]
            }));
            if (isUpdateState) {
                this.setState((function(__state_argument) {
                    return {
                        virtualScrollingData: {
                            state: nextState,
                            sizes: __state_argument.virtualScrollingData.sizes
                        }
                    }
                }))
            }
        }
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        _InfernoComponent.prototype.componentWillUpdate.call(this);
        if (this.props.type !== nextProps.type || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.intervalCount !== nextProps.intervalCount || this.props.groups !== nextProps.groups || this.props.groupOrientation !== nextProps.groupOrientation) {
            this.__getterCache.renderConfig = void 0
        }
        if (this.props.type !== nextProps.type) {
            this.__getterCache.viewDataGenerator = void 0
        }
        if (this.props.type !== nextProps.type) {
            this.__getterCache.dateHeaderDataGenerator = void 0
        }
        if (this.props.type !== nextProps.type) {
            this.__getterCache.timePanelDataGenerator = void 0
        }
        if (this.props.currentDate !== nextProps.currentDate || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type) {
            this.__getterCache.startViewDate = void 0
        }
        if (this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.groupByDate !== nextProps.groupByDate || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.showAllDayPanel !== nextProps.showAllDayPanel) {
            this.__getterCache.completeViewDataMap = void 0
        }
        if (this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.groups !== nextProps.groups || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling || this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.groupByDate !== nextProps.groupByDate || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.showAllDayPanel !== nextProps.showAllDayPanel) {
            this.__getterCache.correctedVirtualScrollingState = void 0
        }
        if (this.props.type !== nextProps.type || this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.groupByDate !== nextProps.groupByDate || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.showAllDayPanel !== nextProps.showAllDayPanel || this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling) {
            this.__getterCache.viewDataMap = void 0
        }
        if (this.props.groups !== nextProps.groups || this.props.type !== nextProps.type || this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.groupByDate !== nextProps.groupByDate || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.showAllDayPanel !== nextProps.showAllDayPanel || this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling) {
            this.__getterCache.viewData = void 0
        }
        if (this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.groupOrientation !== nextProps.groupOrientation || this.props.groupByDate !== nextProps.groupByDate || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.startDate !== nextProps.startDate || this.props.cellDuration !== nextProps.cellDuration || this.props.showAllDayPanel !== nextProps.showAllDayPanel) {
            this.__getterCache.completeDateHeaderData = void 0
        }
        if (this.props.endDayHour !== nextProps.endDayHour || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.currentDate !== nextProps.currentDate || this.props.intervalCount !== nextProps.intervalCount || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.groupOrientation !== nextProps.groupOrientation || this.props.groupByDate !== nextProps.groupByDate || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.startDate !== nextProps.startDate || this.props.cellDuration !== nextProps.cellDuration || this.props.showAllDayPanel !== nextProps.showAllDayPanel || this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling) {
            this.__getterCache.dateHeaderData = void 0
        }
        if (this.props.type !== nextProps.type || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.intervalCount !== nextProps.intervalCount || this.props.groups !== nextProps.groups || this.props.groupOrientation !== nextProps.groupOrientation || this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.hoursInterval !== nextProps.hoursInterval || this.props.startDayHour !== nextProps.startDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.startDate !== nextProps.startDate || this.props.groupByDate !== nextProps.groupByDate || this.props.showAllDayPanel !== nextProps.showAllDayPanel) {
            this.__getterCache.completeTimePanelData = void 0
        }
        if (this.props.type !== nextProps.type || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.intervalCount !== nextProps.intervalCount || this.props.groups !== nextProps.groups || this.props.groupOrientation !== nextProps.groupOrientation || this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.hoursInterval !== nextProps.hoursInterval || this.props.startDayHour !== nextProps.startDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.startDate !== nextProps.startDate || this.props.groupByDate !== nextProps.groupByDate || this.props.showAllDayPanel !== nextProps.showAllDayPanel || this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling) {
            this.__getterCache.timePanelData = void 0
        }
        if (this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.groupByDate !== nextProps.groupByDate || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.showAllDayPanel !== nextProps.showAllDayPanel || this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling) {
            this.__getterCache.viewDataProvider = void 0
        }
        if (this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.groupByDate !== nextProps.groupByDate) {
            this.__getterCache.groupPanelData = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                dataCellTemplate: getTemplate(props.dataCellTemplate),
                dateCellTemplate: getTemplate(props.dateCellTemplate),
                timeCellTemplate: getTemplate(props.timeCellTemplate),
                resourceCellTemplate: getTemplate(props.resourceCellTemplate)
            }),
            groupPanelHeight: this.state.groupPanelHeight,
            headerEmptyCellWidth: this.state.headerEmptyCellWidth,
            tablesWidth: this.state.tablesWidth,
            virtualScrolling: this.state.virtualScrolling,
            virtualScrollingData: this.state.virtualScrollingData,
            dateTableRef: this.dateTableRef,
            allDayPanelRef: this.allDayPanelRef,
            timePanelRef: this.timePanelRef,
            groupPanelRef: this.groupPanelRef,
            widgetElementRef: this.widgetElementRef,
            layoutRef: this.layoutRef,
            renderConfig: this.renderConfig,
            groupOrientation: this.groupOrientation,
            isVerticalGrouping: this.isVerticalGrouping,
            isHorizontalGrouping: this.isHorizontalGrouping,
            isGroupedByDate: this.isGroupedByDate,
            layout: this.layout,
            isAllDayPanelVisible: this.isAllDayPanelVisible,
            viewDataGenerator: this.viewDataGenerator,
            dateHeaderDataGenerator: this.dateHeaderDataGenerator,
            timePanelDataGenerator: this.timePanelDataGenerator,
            startViewDate: this.startViewDate,
            completeViewDataMap: this.completeViewDataMap,
            correctedVirtualScrollingState: this.correctedVirtualScrollingState,
            viewDataMap: this.viewDataMap,
            viewData: this.viewData,
            completeDateHeaderData: this.completeDateHeaderData,
            dateHeaderData: this.dateHeaderData,
            completeTimePanelData: this.completeTimePanelData,
            timePanelData: this.timePanelData,
            viewDataProvider: this.viewDataProvider,
            groupPanelData: this.groupPanelData,
            headerPanelTemplate: this.headerPanelTemplate,
            dateTableTemplate: this.dateTableTemplate,
            timePanelTemplate: this.timePanelTemplate,
            isRenderHeaderEmptyCell: this.isRenderHeaderEmptyCell,
            isWorkSpaceWithOddCells: this.isWorkSpaceWithOddCells,
            classes: this.classes,
            isStandaloneAllDayPanel: this.isStandaloneAllDayPanel,
            isCalculateTablesWidth: this.isCalculateTablesWidth,
            createDateTableElementsMeta: this.createDateTableElementsMeta,
            createAllDayPanelElementsMeta: this.createAllDayPanelElementsMeta,
            onScroll: this.onScroll,
            restAttributes: this.restAttributes
        })
    };
    _createClass(WorkSpace, [{
        key: "renderConfig",
        get: function() {
            var _this6 = this;
            if (void 0 !== this.__getterCache.renderConfig) {
                return this.__getterCache.renderConfig
            }
            return this.__getterCache.renderConfig = (0, _work_space_config.getViewRenderConfigByType)(_this6.props.type, _this6.props.crossScrollingEnabled, _this6.props.intervalCount, _this6.props.groups, _this6.props.groupOrientation)
        }
    }, {
        key: "groupOrientation",
        get: function() {
            var groupOrientation = this.props.groupOrientation;
            var defaultGroupOrientation = this.renderConfig.defaultGroupOrientation;
            return null !== groupOrientation && void 0 !== groupOrientation ? groupOrientation : defaultGroupOrientation
        }
    }, {
        key: "isVerticalGrouping",
        get: function() {
            return (0, _utils2.isVerticalGroupingApplied)(this.props.groups, this.groupOrientation)
        }
    }, {
        key: "isHorizontalGrouping",
        get: function() {
            return (0, _utils2.isHorizontalGroupingApplied)(this.props.groups, this.groupOrientation)
        }
    }, {
        key: "isGroupedByDate",
        get: function() {
            return (0, _utils2.isGroupingByDate)(this.props.groups, this.groupOrientation, this.props.groupByDate)
        }
    }, {
        key: "layout",
        get: function() {
            return this.renderConfig.isCreateCrossScrolling ? _cross_scrolling_layout.CrossScrollingLayout : _ordinary_layout.OrdinaryLayout
        }
    }, {
        key: "isAllDayPanelVisible",
        get: function() {
            var showAllDayPanel = this.props.showAllDayPanel;
            var isAllDayPanelSupported = this.renderConfig.isAllDayPanelSupported;
            return isAllDayPanelSupported && showAllDayPanel
        }
    }, {
        key: "viewDataGenerator",
        get: function() {
            var _this7 = this;
            if (void 0 !== this.__getterCache.viewDataGenerator) {
                return this.__getterCache.viewDataGenerator
            }
            return this.__getterCache.viewDataGenerator = (0, _utils3.getViewDataGeneratorByViewType)(_this7.props.type)
        }
    }, {
        key: "dateHeaderDataGenerator",
        get: function() {
            var _this8 = this;
            if (void 0 !== this.__getterCache.dateHeaderDataGenerator) {
                return this.__getterCache.dateHeaderDataGenerator
            }
            return this.__getterCache.dateHeaderDataGenerator = new _date_header_data_generator.DateHeaderDataGenerator(_this8.viewDataGenerator)
        }
    }, {
        key: "timePanelDataGenerator",
        get: function() {
            var _this9 = this;
            if (void 0 !== this.__getterCache.timePanelDataGenerator) {
                return this.__getterCache.timePanelDataGenerator
            }
            return this.__getterCache.timePanelDataGenerator = new _time_panel_data_generator.TimePanelDataGenerator(_this9.viewDataGenerator)
        }
    }, {
        key: "startViewDate",
        get: function() {
            var _this10 = this;
            if (void 0 !== this.__getterCache.startViewDate) {
                return this.__getterCache.startViewDate
            }
            return this.__getterCache.startViewDate = (_this10$props = _this10.props, currentDate = _this10$props.currentDate, firstDayOfWeek = _this10$props.firstDayOfWeek, intervalCount = _this10$props.intervalCount, startDate = _this10$props.startDate, startDayHour = _this10$props.startDayHour, type = _this10$props.type, options = {
                currentDate: currentDate,
                startDayHour: startDayHour,
                startDate: startDate,
                intervalCount: intervalCount,
                firstDayOfWeek: firstDayOfWeek
            }, viewDataGenerator = (0, _utils3.getViewDataGeneratorByViewType)(type), startViewDate = viewDataGenerator.getStartViewDate(options), startViewDate);
            var _this10$props, currentDate, firstDayOfWeek, intervalCount, startDate, startDayHour, type, options, viewDataGenerator, startViewDate
        }
    }, {
        key: "completeViewDataMap",
        get: function() {
            var _this11 = this;
            if (void 0 !== this.__getterCache.completeViewDataMap) {
                return this.__getterCache.completeViewDataMap
            }
            return this.__getterCache.completeViewDataMap = (_this11$props = _this11.props, cellDuration = _this11$props.cellDuration, currentDate = _this11$props.currentDate, endDayHour = _this11$props.endDayHour, firstDayOfWeek = _this11$props.firstDayOfWeek, groupByDate = _this11$props.groupByDate, groups = _this11$props.groups, hoursInterval = _this11$props.hoursInterval, intervalCount = _this11$props.intervalCount, startDate = _this11$props.startDate, startDayHour = _this11$props.startDayHour, type = _this11$props.type, _this11.viewDataGenerator.getCompleteViewDataMap({
                currentDate: currentDate,
                startDate: startDate,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                groupByDate: groupByDate,
                groups: groups,
                intervalCount: intervalCount,
                firstDayOfWeek: firstDayOfWeek,
                hoursInterval: hoursInterval,
                cellDuration: cellDuration,
                startViewDate: _this11.startViewDate,
                groupOrientation: _this11.groupOrientation,
                isVerticalGrouping: _this11.isVerticalGrouping,
                isHorizontalGrouping: _this11.isHorizontalGrouping,
                isGroupedByDate: _this11.isGroupedByDate,
                isAllDayPanelVisible: _this11.isAllDayPanelVisible,
                viewType: type,
                interval: _this11.viewDataGenerator.getInterval(hoursInterval)
            }));
            var _this11$props, cellDuration, currentDate, endDayHour, firstDayOfWeek, groupByDate, groups, hoursInterval, intervalCount, startDate, startDayHour, type
        }
    }, {
        key: "correctedVirtualScrollingState",
        get: function() {
            var _this12 = this;
            if (void 0 !== this.__getterCache.correctedVirtualScrollingState) {
                return this.__getterCache.correctedVirtualScrollingState
            }
            return this.__getterCache.correctedVirtualScrollingState = function() {
                var _this12$state$virtual;
                var result = null === (_this12$state$virtual = _this12.state.virtualScrollingData) || void 0 === _this12$state$virtual ? void 0 : _this12$state$virtual.state;
                if (!result) {
                    var _this12$props = _this12.props,
                        groups = _this12$props.groups,
                        schedulerHeight = _this12$props.schedulerHeight,
                        schedulerWidth = _this12$props.schedulerWidth,
                        scrolling = _this12$props.scrolling;
                    result = calculateDefaultVirtualScrollingState({
                        virtualScrollingDispatcher: _this12.state.virtualScrolling,
                        scrolling: scrolling,
                        groups: groups,
                        completeViewDataMap: _this12.completeViewDataMap,
                        isVerticalGrouping: _this12.isVerticalGrouping,
                        schedulerHeight: schedulerHeight,
                        schedulerWidth: schedulerWidth
                    })
                }
                return _extends({
                    startCellIndex: 0,
                    startRowIndex: 0
                }, result)
            }()
        }
    }, {
        key: "viewDataMap",
        get: function() {
            var _this13 = this;
            if (void 0 !== this.__getterCache.viewDataMap) {
                return this.__getterCache.viewDataMap
            }
            return this.__getterCache.viewDataMap = _this13.viewDataGenerator.generateViewDataMap(_this13.completeViewDataMap, _extends({}, _this13.correctedVirtualScrollingState, {
                isVerticalGrouping: _this13.isVerticalGrouping,
                isAllDayPanelVisible: _this13.isAllDayPanelVisible
            }))
        }
    }, {
        key: "viewData",
        get: function() {
            var _this14 = this;
            if (void 0 !== this.__getterCache.viewData) {
                return this.__getterCache.viewData
            }
            return this.__getterCache.viewData = (groups = _this14.props.groups, result = _this14.viewDataGenerator.getViewDataFromMap(_this14.completeViewDataMap, _this14.viewDataMap, _extends({}, _this14.correctedVirtualScrollingState, {
                isProvideVirtualCellsWidth: _this14.renderConfig.isProvideVirtualCellsWidth,
                isVerticalGrouping: _this14.isVerticalGrouping,
                isAllDayPanelVisible: _this14.isAllDayPanelVisible,
                isGroupedAllDayPanel: (0, _base.calculateIsGroupedAllDayPanel)(groups, _this14.groupOrientation, _this14.isAllDayPanelVisible)
            })), result);
            var groups, result
        }
    }, {
        key: "completeDateHeaderData",
        get: function() {
            var _this15 = this;
            if (void 0 !== this.__getterCache.completeDateHeaderData) {
                return this.__getterCache.completeDateHeaderData
            }
            return this.__getterCache.completeDateHeaderData = (_this15$props = _this15.props, currentDate = _this15$props.currentDate, endDayHour = _this15$props.endDayHour, groups = _this15$props.groups, hoursInterval = _this15$props.hoursInterval, intervalCount = _this15$props.intervalCount, startDayHour = _this15$props.startDayHour, viewType = _this15$props.type, _this15.dateHeaderDataGenerator.getCompleteDateHeaderMap({
                isGenerateWeekDaysHeaderData: _this15.renderConfig.isGenerateWeekDaysHeaderData,
                isGroupedByDate: _this15.isGroupedByDate,
                groups: groups,
                groupOrientation: _this15.groupOrientation,
                isHorizontalGrouping: _this15.isHorizontalGrouping,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                hoursInterval: hoursInterval,
                intervalCount: intervalCount,
                headerCellTextFormat: _this15.renderConfig.headerCellTextFormat,
                getDateForHeaderText: _this15.renderConfig.getDateForHeaderText,
                interval: _this15.viewDataGenerator.getInterval(hoursInterval),
                startViewDate: _this15.startViewDate,
                currentDate: currentDate,
                viewType: viewType,
                today: new Date
            }, _this15.completeViewDataMap));
            var _this15$props, currentDate, endDayHour, groups, hoursInterval, intervalCount, startDayHour, viewType
        }
    }, {
        key: "dateHeaderData",
        get: function() {
            var _this16 = this;
            if (void 0 !== this.__getterCache.dateHeaderData) {
                return this.__getterCache.dateHeaderData
            }
            return this.__getterCache.dateHeaderData = (_this16$props = _this16.props, endDayHour = _this16$props.endDayHour, groups = _this16$props.groups, hoursInterval = _this16$props.hoursInterval, startDayHour = _this16$props.startDayHour, _this16.dateHeaderDataGenerator.generateDateHeaderData(_this16.completeDateHeaderData, _this16.completeViewDataMap, _extends({
                isGenerateWeekDaysHeaderData: _this16.renderConfig.isGenerateWeekDaysHeaderData,
                isProvideVirtualCellsWidth: _this16.renderConfig.isProvideVirtualCellsWidth,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                hoursInterval: hoursInterval,
                groups: groups,
                groupOrientation: _this16.groupOrientation,
                isGroupedByDate: _this16.isGroupedByDate
            }, _this16.correctedVirtualScrollingState)));
            var _this16$props, endDayHour, groups, hoursInterval, startDayHour
        }
    }, {
        key: "completeTimePanelData",
        get: function() {
            var _this17 = this;
            if (void 0 !== this.__getterCache.completeTimePanelData) {
                return this.__getterCache.completeTimePanelData
            }
            return this.__getterCache.completeTimePanelData = function() {
                if (!_this17.renderConfig.isRenderTimePanel) {
                    return
                }
                var _this17$props = _this17.props,
                    cellDuration = _this17$props.cellDuration,
                    currentDate = _this17$props.currentDate,
                    endDayHour = _this17$props.endDayHour,
                    hoursInterval = _this17$props.hoursInterval,
                    intervalCount = _this17$props.intervalCount,
                    startDayHour = _this17$props.startDayHour,
                    type = _this17$props.type;
                return _this17.timePanelDataGenerator.getCompleteTimePanelMap({
                    startViewDate: _this17.startViewDate,
                    cellDuration: cellDuration,
                    startDayHour: startDayHour,
                    endDayHour: endDayHour,
                    isVerticalGrouping: _this17.isVerticalGrouping,
                    intervalCount: intervalCount,
                    currentDate: currentDate,
                    viewType: type,
                    hoursInterval: hoursInterval
                }, _this17.completeViewDataMap)
            }()
        }
    }, {
        key: "timePanelData",
        get: function() {
            var _this18 = this;
            if (void 0 !== this.__getterCache.timePanelData) {
                return this.__getterCache.timePanelData
            }
            return this.__getterCache.timePanelData = function() {
                if (!_this18.completeTimePanelData) {
                    return
                }
                return _this18.timePanelDataGenerator.generateTimePanelData(_this18.completeTimePanelData, _extends({
                    isGroupedAllDayPanel: (0, _base.calculateIsGroupedAllDayPanel)(_this18.props.groups, _this18.groupOrientation, _this18.isAllDayPanelVisible),
                    isVerticalGrouping: _this18.isVerticalGrouping,
                    isAllDayPanelVisible: _this18.isAllDayPanelVisible
                }, _this18.correctedVirtualScrollingState))
            }()
        }
    }, {
        key: "viewDataProvider",
        get: function() {
            var _this19 = this;
            if (void 0 !== this.__getterCache.viewDataProvider) {
                return this.__getterCache.viewDataProvider
            }
            return this.__getterCache.viewDataProvider = function() {
                var _this19$props = _this19.props,
                    cellDuration = _this19$props.cellDuration,
                    currentDate = _this19$props.currentDate,
                    endDayHour = _this19$props.endDayHour,
                    firstDayOfWeek = _this19$props.firstDayOfWeek,
                    groups = _this19$props.groups,
                    hoursInterval = _this19$props.hoursInterval,
                    intervalCount = _this19$props.intervalCount,
                    startDate = _this19$props.startDate,
                    startDayHour = _this19$props.startDayHour,
                    type = _this19$props.type;
                var viewDataProvider = new _view_data_provider.default(type);
                viewDataProvider.completeViewDataMap = _this19.completeViewDataMap;
                viewDataProvider.viewDataMap = _this19.viewDataMap;
                viewDataProvider.viewData = _this19.viewData;
                var generationOptions = prepareGenerationOptions({
                    intervalCount: intervalCount,
                    groups: groups,
                    groupByDate: _this19.isGroupedByDate,
                    groupOrientation: _this19.groupOrientation,
                    startDayHour: startDayHour,
                    endDayHour: endDayHour,
                    currentDate: currentDate,
                    startDate: startDate,
                    firstDayOfWeek: firstDayOfWeek,
                    hoursInterval: hoursInterval,
                    type: type,
                    cellDuration: cellDuration
                }, _this19.renderConfig, _this19.isAllDayPanelVisible, _this19.correctedVirtualScrollingState);
                viewDataProvider.setViewOptions(generationOptions);
                viewDataProvider.createGroupedDataMapProvider();
                return viewDataProvider
            }()
        }
    }, {
        key: "groupPanelData",
        get: function() {
            var _this20 = this;
            if (void 0 !== this.__getterCache.groupPanelData) {
                return this.__getterCache.groupPanelData
            }
            return this.__getterCache.groupPanelData = (_this20$props = _this20.props, currentDate = _this20$props.currentDate, endDayHour = _this20$props.endDayHour, groups = _this20$props.groups, hoursInterval = _this20$props.hoursInterval, intervalCount = _this20$props.intervalCount, startDayHour = _this20$props.startDayHour, type = _this20$props.type, columnCountPerGroup = _this20.viewDataGenerator.getCellCount({
                intervalCount: intervalCount,
                hoursInterval: hoursInterval,
                currentDate: currentDate,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                viewType: type
            }), groupPanelData = (0, _utils4.getGroupPanelData)(groups, columnCountPerGroup, _this20.isGroupedByDate, _this20.isGroupedByDate ? 1 : columnCountPerGroup), groupPanelData);
            var _this20$props, currentDate, endDayHour, groups, hoursInterval, intervalCount, startDayHour, type, columnCountPerGroup, groupPanelData
        }
    }, {
        key: "headerPanelTemplate",
        get: function() {
            var headerPanelTemplate = this.renderConfig.headerPanelTemplate;
            return headerPanelTemplate
        }
    }, {
        key: "dateTableTemplate",
        get: function() {
            var dateTableTemplate = this.renderConfig.dateTableTemplate;
            return dateTableTemplate
        }
    }, {
        key: "timePanelTemplate",
        get: function() {
            var timePanelTemplate = this.renderConfig.timePanelTemplate;
            return timePanelTemplate
        }
    }, {
        key: "isRenderHeaderEmptyCell",
        get: function() {
            return this.isVerticalGrouping || !!this.timePanelTemplate
        }
    }, {
        key: "isWorkSpaceWithOddCells",
        get: function() {
            return false
        }
    }, {
        key: "classes",
        get: function() {
            var _combineClasses;
            var _this$props4 = this.props,
                allDayPanelExpanded = _this$props4.allDayPanelExpanded,
                groups = _this$props4.groups,
                intervalCount = _this$props4.intervalCount;
            return (0, _combine_classes.combineClasses)((_combineClasses = {}, _defineProperty(_combineClasses, this.renderConfig.className, true), _defineProperty(_combineClasses, "dx-scheduler-work-space-count", intervalCount > 1), _defineProperty(_combineClasses, "dx-scheduler-work-space-odd-cells", !!this.isWorkSpaceWithOddCells), _defineProperty(_combineClasses, "dx-scheduler-work-space-all-day-collapsed", !allDayPanelExpanded && this.isAllDayPanelVisible), _defineProperty(_combineClasses, "dx-scheduler-work-space-all-day", this.isAllDayPanelVisible), _defineProperty(_combineClasses, "dx-scheduler-work-space-group-by-date", this.isGroupedByDate), _defineProperty(_combineClasses, "dx-scheduler-work-space-grouped", groups.length > 0), _defineProperty(_combineClasses, "dx-scheduler-work-space-vertical-grouped", this.isVerticalGrouping && "vertical" !== this.renderConfig.defaultGroupOrientation), _defineProperty(_combineClasses, "dx-scheduler-work-space-horizontal-grouped", (0, _utils2.isHorizontalGroupingApplied)(groups, this.groupOrientation) && "vertical" === this.renderConfig.defaultGroupOrientation), _defineProperty(_combineClasses, "dx-scheduler-group-column-count-one", this.isVerticalGrouping && 1 === groups.length), _defineProperty(_combineClasses, "dx-scheduler-group-column-count-two", this.isVerticalGrouping && 2 === groups.length), _defineProperty(_combineClasses, "dx-scheduler-group-column-count-three", this.isVerticalGrouping && 3 === groups.length), _defineProperty(_combineClasses, "dx-scheduler-work-space-both-scrollbar", this.props.crossScrollingEnabled), _defineProperty(_combineClasses, "dx-scheduler-work-space", true), _combineClasses))
        }
    }, {
        key: "isStandaloneAllDayPanel",
        get: function() {
            var groups = this.props.groups;
            return !(0, _utils2.isVerticalGroupingApplied)(groups, this.groupOrientation) && this.isAllDayPanelVisible
        }
    }, {
        key: "isCalculateTablesWidth",
        get: function() {
            return this.props.crossScrollingEnabled && "vertical" !== this.renderConfig.defaultGroupOrientation
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props5 = this.props,
                restProps = (_this$props5.accessKey, _this$props5.activeStateEnabled, _this$props5.allDayAppointments, _this$props5.allDayPanelExpanded, _this$props5.allowMultipleCellSelection, _this$props5.appointments, _this$props5.cellDuration, _this$props5.className, _this$props5.crossScrollingEnabled, _this$props5.currentDate, _this$props5.dataCellTemplate, _this$props5.dateCellTemplate, _this$props5.disabled, _this$props5.endDayHour, _this$props5.firstDayOfWeek, _this$props5.focusStateEnabled, _this$props5.groupByDate, _this$props5.groupOrientation, _this$props5.groups, _this$props5.height, _this$props5.hint, _this$props5.hoursInterval, _this$props5.hoverStateEnabled, _this$props5.indicatorTime, _this$props5.indicatorUpdateInterval, _this$props5.intervalCount, _this$props5.onClick, _this$props5.onKeyDown, _this$props5.onViewRendered, _this$props5.resourceCellTemplate, _this$props5.rtlEnabled, _this$props5.schedulerHeight, _this$props5.schedulerWidth, _this$props5.scrolling, _this$props5.selectedCellData, _this$props5.shadeUntilCurrentTime, _this$props5.showAllDayPanel, _this$props5.showCurrentTimeIndicator, _this$props5.startDate, _this$props5.startDayHour, _this$props5.tabIndex, _this$props5.timeCellTemplate, _this$props5.type, _this$props5.visible, _this$props5.width, _objectWithoutProperties(_this$props5, _excluded));
            return restProps
        }
    }]);
    return WorkSpace
}(_inferno2.InfernoComponent);
exports.WorkSpace = WorkSpace;
WorkSpace.defaultProps = _props.WorkSpaceProps;
