/**
 * DevExtreme (cjs/ui/scheduler/appointments/rendering_strategies/strategy_horizontal.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _strategy = _interopRequireDefault(require("./strategy.base"));
var _date = _interopRequireDefault(require("../../../../core/utils/date"));
var _expressionUtils = require("../../expressionUtils");
var _getSkippedHoursInRange = _interopRequireDefault(require("../../../../renovation/ui/scheduler/view_model/appointments/utils/getSkippedHoursInRange"));

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
var DEFAULT_APPOINTMENT_HEIGHT = 60;
var MIN_APPOINTMENT_HEIGHT = 35;
var DROP_DOWN_BUTTON_OFFSET = 2;
var toMs = _date.default.dateToMilliseconds;
var HorizontalRenderingStrategy = function(_BaseAppointmentsStra) {
    _inheritsLoose(HorizontalRenderingStrategy, _BaseAppointmentsStra);

    function HorizontalRenderingStrategy() {
        return _BaseAppointmentsStra.apply(this, arguments) || this
    }
    var _proto = HorizontalRenderingStrategy.prototype;
    _proto._needVerifyItemSize = function() {
        return true
    };
    _proto.calculateAppointmentWidth = function(appointment, position) {
        var cellWidth = this.cellWidth || this.getAppointmentMinSize();
        var allDay = _expressionUtils.ExpressionUtils.getField(this.dataAccessors, "allDay", appointment);
        var startDate = position.info.appointment.startDate;
        var endDate = position.info.appointment.endDate;
        var normalizedEndDate = position.info.appointment.normalizedEndDate;
        var duration = this.getAppointmentDurationInMs(startDate, normalizedEndDate, allDay);
        duration = this._adjustDurationByDaylightDiff(duration, startDate, normalizedEndDate);
        var cellDuration = this.cellDurationInMinutes * toMs("minute");
        var skippedHours = (0, _getSkippedHoursInRange.default)(startDate, endDate, this.viewDataProvider);
        var durationInCells = (duration - skippedHours * toMs("hour")) / cellDuration;
        var width = this.cropAppointmentWidth(durationInCells * cellWidth, cellWidth);
        return width
    };
    _proto._needAdjustDuration = function(diff) {
        return diff < 0
    };
    _proto.getAppointmentGeometry = function(coordinates) {
        var result = this._customizeAppointmentGeometry(coordinates);
        return _BaseAppointmentsStra.prototype.getAppointmentGeometry.call(this, result)
    };
    _proto._customizeAppointmentGeometry = function(coordinates) {
        var config = this._calculateGeometryConfig(coordinates);
        return this._customizeCoordinates(coordinates, config.height, config.appointmentCountPerCell, config.offset)
    };
    _proto._getOffsets = function() {
        return {
            unlimited: 0,
            auto: 0
        }
    };
    _proto._getCompactLeftCoordinate = function(itemLeft, index) {
        var cellWidth = this.cellWidth || this.getAppointmentMinSize();
        return itemLeft + cellWidth * index
    };
    _proto._getMaxHeight = function() {
        return this.cellHeight || this.getAppointmentMinSize()
    };
    _proto._getAppointmentCount = function(overlappingMode, coordinates) {
        return this._getMaxAppointmentCountPerCellByType(false)
    };
    _proto._getAppointmentDefaultHeight = function() {
        return DEFAULT_APPOINTMENT_HEIGHT
    };
    _proto._getAppointmentMinHeight = function() {
        return MIN_APPOINTMENT_HEIGHT
    };
    _proto._sortCondition = function(a, b) {
        return this._columnCondition(a, b)
    };
    _proto._getOrientation = function() {
        return ["left", "right", "top"]
    };
    _proto.getDropDownAppointmentWidth = function() {
        return this.cellWidth - 2 * DROP_DOWN_BUTTON_OFFSET
    };
    _proto.getDeltaTime = function(args, initialSize) {
        var deltaTime;
        var deltaWidth = args.width - initialSize.width;
        deltaTime = toMs("minute") * Math.round(deltaWidth / this.cellWidth * this.cellDurationInMinutes);
        return deltaTime
    };
    _proto.isAllDay = function(appointmentData) {
        return _expressionUtils.ExpressionUtils.getField(this.dataAccessors, "allDay", appointmentData)
    };
    _proto._isItemsCross = function(firstItem, secondItem) {
        var orientation = this._getOrientation();
        return this._checkItemsCrossing(firstItem, secondItem, orientation)
    };
    _proto.getPositionShift = function(timeShift) {
        var positionShift = _BaseAppointmentsStra.prototype.getPositionShift.call(this, timeShift);
        var left = this.cellWidth * timeShift;
        if (this.rtlEnabled) {
            left *= -1
        }
        left += positionShift.left;
        return {
            top: 0,
            left: left,
            cellPosition: left
        }
    };
    _proto.supportCompactDropDownAppointments = function() {
        return false
    };
    return HorizontalRenderingStrategy
}(_strategy.default);
var _default = HorizontalRenderingStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
