/**
 * DevExtreme (renovation/ui/scheduler/model/views.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getCurrentViewProps = exports.getCurrentViewConfig = exports.getCurrentView = void 0;
var _type = require("../../../../core/utils/type");

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
var VIEW_TYPES = ["day", "week", "workWeek", "month", "timelineDay", "timelineWeek", "timelineWorkWeek", "timelineMonth", "agenda"];
var getCurrentView = function(currentView, views) {
    var currentViewProps = views.find((function(view) {
        var names = (0, _type.isObject)(view) ? [view.name, view.type] : [view];
        if (names.includes(currentView)) {
            return true
        }
        return false
    }));
    if (void 0 === currentViewProps) {
        if (VIEW_TYPES.includes(currentView)) {
            currentViewProps = currentView
        } else {
            var _views = _slicedToArray(views, 1);
            currentViewProps = _views[0]
        }
    }
    return currentViewProps
};
exports.getCurrentView = getCurrentView;
var getCurrentViewProps = function(currentView, views) {
    var currentViewProps = getCurrentView(currentView, views);
    return (0, _type.isString)(currentViewProps) ? {
        type: currentViewProps
    } : currentViewProps
};
exports.getCurrentViewProps = getCurrentViewProps;

function getViewConfigProp(schedulerProp, viewProp) {
    return void 0 !== viewProp ? viewProp : schedulerProp
}
var getCurrentViewConfig = function(currentViewProps, schedulerProps) {
    var schedulerScrolling = schedulerProps.scrolling;
    var cellDuration = currentViewProps.cellDuration,
        dataCellTemplate = currentViewProps.dataCellTemplate,
        dateCellTemplate = currentViewProps.dateCellTemplate,
        endDayHour = currentViewProps.endDayHour,
        firstDayOfWeek = currentViewProps.firstDayOfWeek,
        groupByDate = currentViewProps.groupByDate,
        groupOrientation = currentViewProps.groupOrientation,
        intervalCount = currentViewProps.intervalCount,
        resourceCellTemplate = currentViewProps.resourceCellTemplate,
        scrolling = currentViewProps.scrolling,
        startDate = currentViewProps.startDate,
        startDayHour = currentViewProps.startDayHour,
        timeCellTemplate = currentViewProps.timeCellTemplate,
        type = currentViewProps.type;
    var isVirtualScrolling = "virtual" === schedulerScrolling.mode || "virtual" === (null === scrolling || void 0 === scrolling ? void 0 : scrolling.mode);
    var crossScrollingEnabled = schedulerProps.crossScrollingEnabled || isVirtualScrolling;
    var result = {
        firstDayOfWeek: getViewConfigProp(schedulerProps.firstDayOfWeek, firstDayOfWeek),
        startDayHour: getViewConfigProp(schedulerProps.startDayHour, startDayHour),
        endDayHour: getViewConfigProp(schedulerProps.endDayHour, endDayHour),
        cellDuration: getViewConfigProp(schedulerProps.cellDuration, cellDuration),
        groupByDate: getViewConfigProp(schedulerProps.groupByDate, groupByDate),
        scrolling: getViewConfigProp(schedulerScrolling, scrolling),
        dataCellTemplate: getViewConfigProp(schedulerProps.dataCellTemplate, dataCellTemplate),
        timeCellTemplate: getViewConfigProp(schedulerProps.timeCellTemplate, timeCellTemplate),
        resourceCellTemplate: getViewConfigProp(schedulerProps.resourceCellTemplate, resourceCellTemplate),
        dateCellTemplate: getViewConfigProp(schedulerProps.dateCellTemplate, dateCellTemplate),
        currentDate: schedulerProps.currentDate,
        intervalCount: intervalCount,
        groupOrientation: groupOrientation,
        startDate: startDate,
        type: type,
        showAllDayPanel: schedulerProps.showAllDayPanel,
        showCurrentTimeIndicator: schedulerProps.showCurrentTimeIndicator,
        indicatorUpdateInterval: schedulerProps.indicatorUpdateInterval,
        shadeUntilCurrentTime: schedulerProps.shadeUntilCurrentTime,
        crossScrollingEnabled: crossScrollingEnabled,
        schedulerHeight: schedulerProps.height,
        schedulerWidth: schedulerProps.width,
        tabIndex: schedulerProps.tabIndex,
        accessKey: schedulerProps.accessKey,
        focusStateEnabled: schedulerProps.focusStateEnabled,
        allowMultipleCellSelection: true,
        allDayPanelExpanded: true
    };
    return _extends({}, result, {
        hoursInterval: result.cellDuration / 60
    })
};
exports.getCurrentViewConfig = getCurrentViewConfig;
