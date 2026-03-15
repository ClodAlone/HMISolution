/**
 * DevExtreme (cjs/ui/scheduler/appointments/rendering_strategies/appointmentsPositioning_strategy_base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _type = require("../../../../core/utils/type");
var COLLECTOR_DEFAULT_WIDTH = 24;
var COLLECTOR_DEFAULT_OFFSET = 3;
var COMPACT_THEME_APPOINTMENT_DEFAULT_OFFSET = 22;
var APPOINTMENT_MIN_COUNT = 1;
var APPOINTMENT_DEFAULT_WIDTH = 40;
var COLLECTOR_WIDTH_IN_PERCENTS = 75;
var APPOINTMENT_INCREASED_WIDTH = 50;
var AppointmentPositioningStrategy = function() {
    function AppointmentPositioningStrategy(renderingStrategy) {
        this._renderingStrategy = renderingStrategy
    }
    var _proto = AppointmentPositioningStrategy.prototype;
    _proto.getDropDownAppointmentWidth = function(intervalCount, isAllDay) {
        if (isAllDay || !(0, _type.isDefined)(isAllDay)) {
            return COLLECTOR_WIDTH_IN_PERCENTS * this._renderingStrategy.cellWidth / 100
        } else {
            return COLLECTOR_DEFAULT_WIDTH
        }
    };
    _proto.getCollectorTopOffset = function() {
        return COLLECTOR_DEFAULT_OFFSET
    };
    _proto.getCollectorLeftOffset = function() {
        return COLLECTOR_DEFAULT_OFFSET
    };
    _proto.getAppointmentDefaultOffset = function() {
        if (this._renderingStrategy._isCompactTheme()) {
            return COMPACT_THEME_APPOINTMENT_DEFAULT_OFFSET
        }
        return this._renderingStrategy.appointmentOffset
    };
    _proto.getDynamicAppointmentCountPerCell = function() {
        var renderingStrategy = this._renderingStrategy;
        var cellHeight = renderingStrategy.cellHeight;
        var allDayCount = Math.floor((cellHeight - renderingStrategy._getAppointmentDefaultOffset()) / renderingStrategy._getAppointmentDefaultHeight()) || this._getAppointmentMinCount();
        if (renderingStrategy.allDaySupported()) {
            return {
                allDay: "vertical" === renderingStrategy.groupOrientation ? allDayCount : this._renderingStrategy.appointmentCountPerCell,
                simple: this._calculateDynamicAppointmentCountPerCell() || this._getAppointmentMinCount()
            }
        } else {
            return allDayCount
        }
    };
    _proto.getDropDownAppointmentHeight = function() {
        return
    };
    _proto._getAppointmentMinCount = function() {
        return APPOINTMENT_MIN_COUNT
    };
    _proto._calculateDynamicAppointmentCountPerCell = function() {
        return Math.floor(this._renderingStrategy._getAppointmentMaxWidth() / APPOINTMENT_INCREASED_WIDTH)
    };
    _proto._getAppointmentDefaultWidth = function() {
        return APPOINTMENT_DEFAULT_WIDTH
    };
    return AppointmentPositioningStrategy
}();
var _default = AppointmentPositioningStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
