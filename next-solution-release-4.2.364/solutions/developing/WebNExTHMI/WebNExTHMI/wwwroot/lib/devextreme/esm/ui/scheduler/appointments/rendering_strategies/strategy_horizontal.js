/**
 * DevExtreme (esm/ui/scheduler/appointments/rendering_strategies/strategy_horizontal.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import BaseAppointmentsStrategy from "./strategy.base";
import dateUtils from "../../../../core/utils/date";
import {
    ExpressionUtils
} from "../../expressionUtils";
import getSkippedHoursInRange from "../../../../renovation/ui/scheduler/view_model/appointments/utils/getSkippedHoursInRange";
var DEFAULT_APPOINTMENT_HEIGHT = 60;
var MIN_APPOINTMENT_HEIGHT = 35;
var DROP_DOWN_BUTTON_OFFSET = 2;
var toMs = dateUtils.dateToMilliseconds;
class HorizontalRenderingStrategy extends BaseAppointmentsStrategy {
    _needVerifyItemSize() {
        return true
    }
    calculateAppointmentWidth(appointment, position) {
        var cellWidth = this.cellWidth || this.getAppointmentMinSize();
        var allDay = ExpressionUtils.getField(this.dataAccessors, "allDay", appointment);
        var startDate = position.info.appointment.startDate;
        var endDate = position.info.appointment.endDate;
        var {
            normalizedEndDate: normalizedEndDate
        } = position.info.appointment;
        var duration = this.getAppointmentDurationInMs(startDate, normalizedEndDate, allDay);
        duration = this._adjustDurationByDaylightDiff(duration, startDate, normalizedEndDate);
        var cellDuration = this.cellDurationInMinutes * toMs("minute");
        var skippedHours = getSkippedHoursInRange(startDate, endDate, this.viewDataProvider);
        var durationInCells = (duration - skippedHours * toMs("hour")) / cellDuration;
        var width = this.cropAppointmentWidth(durationInCells * cellWidth, cellWidth);
        return width
    }
    _needAdjustDuration(diff) {
        return diff < 0
    }
    getAppointmentGeometry(coordinates) {
        var result = this._customizeAppointmentGeometry(coordinates);
        return super.getAppointmentGeometry(result)
    }
    _customizeAppointmentGeometry(coordinates) {
        var config = this._calculateGeometryConfig(coordinates);
        return this._customizeCoordinates(coordinates, config.height, config.appointmentCountPerCell, config.offset)
    }
    _getOffsets() {
        return {
            unlimited: 0,
            auto: 0
        }
    }
    _getCompactLeftCoordinate(itemLeft, index) {
        var cellWidth = this.cellWidth || this.getAppointmentMinSize();
        return itemLeft + cellWidth * index
    }
    _getMaxHeight() {
        return this.cellHeight || this.getAppointmentMinSize()
    }
    _getAppointmentCount(overlappingMode, coordinates) {
        return this._getMaxAppointmentCountPerCellByType(false)
    }
    _getAppointmentDefaultHeight() {
        return DEFAULT_APPOINTMENT_HEIGHT
    }
    _getAppointmentMinHeight() {
        return MIN_APPOINTMENT_HEIGHT
    }
    _sortCondition(a, b) {
        return this._columnCondition(a, b)
    }
    _getOrientation() {
        return ["left", "right", "top"]
    }
    getDropDownAppointmentWidth() {
        return this.cellWidth - 2 * DROP_DOWN_BUTTON_OFFSET
    }
    getDeltaTime(args, initialSize) {
        var deltaTime;
        var deltaWidth = args.width - initialSize.width;
        deltaTime = toMs("minute") * Math.round(deltaWidth / this.cellWidth * this.cellDurationInMinutes);
        return deltaTime
    }
    isAllDay(appointmentData) {
        return ExpressionUtils.getField(this.dataAccessors, "allDay", appointmentData)
    }
    _isItemsCross(firstItem, secondItem) {
        var orientation = this._getOrientation();
        return this._checkItemsCrossing(firstItem, secondItem, orientation)
    }
    getPositionShift(timeShift) {
        var positionShift = super.getPositionShift(timeShift);
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
    }
    supportCompactDropDownAppointments() {
        return false
    }
}
export default HorizontalRenderingStrategy;
