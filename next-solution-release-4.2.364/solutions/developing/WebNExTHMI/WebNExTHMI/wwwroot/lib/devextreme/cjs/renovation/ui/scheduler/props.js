/**
 * DevExtreme (cjs/renovation/ui/scheduler/props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ViewProps = exports.ScrollingProps = exports.SchedulerProps = exports.ResourceProps = exports.AppointmentEditingProps = exports.AppointmentDraggingProps = void 0;
var _message = _interopRequireDefault(require("../../../localization/message"));
var _base_props = require("../common/base_props");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
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
var ResourceProps = {};
exports.ResourceProps = ResourceProps;
var ViewProps = {};
exports.ViewProps = ViewProps;
var AppointmentEditingProps = {};
exports.AppointmentEditingProps = AppointmentEditingProps;
var AppointmentDraggingProps = {};
exports.AppointmentDraggingProps = AppointmentDraggingProps;
var ScrollingProps = {};
exports.ScrollingProps = ScrollingProps;
var SchedulerProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_base_props.BaseWidgetProps), Object.getOwnPropertyDescriptors(Object.defineProperties({
    adaptivityEnabled: false,
    crossScrollingEnabled: false,
    descriptionExpr: "description",
    focusStateEnabled: true,
    groupByDate: false,
    indicatorUpdateInterval: 3e5,
    recurrenceEditMode: "dialog",
    remoteFiltering: false,
    shadeUntilCurrentTime: false,
    showAllDayPanel: true,
    showCurrentTimeIndicator: true,
    timeZone: "",
    useDropDownViewSwitcher: false,
    endDayHour: 24,
    startDayHour: 0,
    firstDayOfWeek: 0,
    cellDuration: 30,
    maxAppointmentsPerCell: "auto",
    recurrenceExceptionExpr: "recurrenceException",
    recurrenceRuleExpr: "recurrenceRule",
    startDateExpr: "startDate",
    startDateTimeZoneExpr: "startDateTimeZone",
    endDateExpr: "endDate",
    endDateTimeZoneExpr: "endDateTimeZone",
    allDayExpr: "allDay",
    textExpr: "text",
    currentDateChange: function() {},
    defaultCurrentView: "day",
    currentViewChange: function() {}
}, {
    editing: {
        get: function() {
            return {
                allowAdding: true,
                allowDeleting: true,
                allowDragging: true,
                allowResizing: true,
                allowUpdating: true,
                allowTimeZoneEditing: false
            }
        },
        configurable: true,
        enumerable: true
    },
    noDataText: {
        get: function() {
            return _message.default.format("dxCollectionWidget-noDataText")
        },
        configurable: true,
        enumerable: true
    },
    resources: {
        get: function() {
            return []
        },
        configurable: true,
        enumerable: true
    },
    scrolling: {
        get: function() {
            return {
                mode: "standard"
            }
        },
        configurable: true,
        enumerable: true
    },
    selectedCellData: {
        get: function() {
            return []
        },
        configurable: true,
        enumerable: true
    },
    views: {
        get: function() {
            return ["day", "week"]
        },
        configurable: true,
        enumerable: true
    },
    groups: {
        get: function() {
            return []
        },
        configurable: true,
        enumerable: true
    },
    toolbar: {
        get: function() {
            return [{
                defaultElement: "dateNavigator",
                location: "before"
            }, {
                defaultElement: "viewSwitcher",
                location: "after"
            }]
        },
        configurable: true,
        enumerable: true
    },
    defaultCurrentDate: {
        get: function() {
            return new Date
        },
        configurable: true,
        enumerable: true
    }
}))));
exports.SchedulerProps = SchedulerProps;
