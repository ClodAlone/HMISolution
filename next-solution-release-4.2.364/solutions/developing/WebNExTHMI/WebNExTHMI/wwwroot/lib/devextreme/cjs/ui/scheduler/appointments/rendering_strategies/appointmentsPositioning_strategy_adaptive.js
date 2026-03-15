/**
 * DevExtreme (cjs/ui/scheduler/appointments/rendering_strategies/appointmentsPositioning_strategy_adaptive.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _appointmentsPositioning_strategy_base = _interopRequireDefault(require("./appointmentsPositioning_strategy_base"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
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
var COLLECTOR_ADAPTIVE_SIZE = 28;
var COLLECTOR_ADAPTIVE_BOTTOM_OFFSET = 40;
var ADAPTIVE_APPOINTMENT_DEFAULT_OFFSET = 35;
var ADAPTIVE_APPOINTMENT_DEFAULT_WIDTH = 30;
var AdaptivePositioningStrategy = function(_AppointmentPositioni) {
    _inheritsLoose(AdaptivePositioningStrategy, _AppointmentPositioni);

    function AdaptivePositioningStrategy() {
        return _AppointmentPositioni.apply(this, arguments) || this
    }
    var _proto = AdaptivePositioningStrategy.prototype;
    _proto.getDropDownAppointmentWidth = function(intervalCount, isAllDay) {
        return this.getDropDownButtonAdaptiveSize()
    };
    _proto.getDropDownButtonAdaptiveSize = function() {
        return COLLECTOR_ADAPTIVE_SIZE
    };
    _proto.getCollectorTopOffset = function(allDay) {
        var renderingStrategy = this._renderingStrategy;
        if (renderingStrategy.allDaySupported() && allDay) {
            return (renderingStrategy.allDayHeight - renderingStrategy.getDropDownButtonAdaptiveSize()) / 2
        } else {
            return this._renderingStrategy.cellHeight - COLLECTOR_ADAPTIVE_BOTTOM_OFFSET
        }
    };
    _proto.getCollectorLeftOffset = function() {
        var collectorWidth = this._renderingStrategy.getDropDownAppointmentWidth();
        return (this._renderingStrategy.cellWidth - collectorWidth) / 2
    };
    _proto.getAppointmentDefaultOffset = function() {
        return ADAPTIVE_APPOINTMENT_DEFAULT_OFFSET
    };
    _proto.getDynamicAppointmentCountPerCell = function() {
        var renderingStrategy = this._renderingStrategy;
        if (renderingStrategy.allDaySupported()) {
            return {
                allDay: 0,
                simple: this._calculateDynamicAppointmentCountPerCell() || this._getAppointmentMinCount()
            }
        } else {
            return 0
        }
    };
    _proto.getDropDownAppointmentHeight = function() {
        return COLLECTOR_ADAPTIVE_SIZE
    };
    _proto._getAppointmentMinCount = function() {
        return 0
    };
    _proto._getAppointmentDefaultWidth = function() {
        var renderingStrategy = this._renderingStrategy;
        if (renderingStrategy.allDaySupported()) {
            return ADAPTIVE_APPOINTMENT_DEFAULT_WIDTH
        }
        return _AppointmentPositioni.prototype._getAppointmentDefaultWidth.call(this)
    };
    _proto._calculateDynamicAppointmentCountPerCell = function() {
        return Math.floor(this._renderingStrategy._getAppointmentMaxWidth() / this._renderingStrategy._getAppointmentDefaultWidth())
    };
    return AdaptivePositioningStrategy
}(_appointmentsPositioning_strategy_base.default);
var _default = AdaptivePositioningStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
