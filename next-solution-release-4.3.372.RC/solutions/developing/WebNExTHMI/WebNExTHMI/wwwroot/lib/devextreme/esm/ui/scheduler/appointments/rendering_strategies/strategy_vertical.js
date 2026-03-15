/**
 * DevExtreme (esm/ui/scheduler/appointments/rendering_strategies/strategy_vertical.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import BaseAppointmentsStrategy from "./strategy.base";
import {
    extend
} from "../../../../core/utils/extend";
import {
    isNumeric
} from "../../../../core/utils/type";
import dateUtils from "../../../../core/utils/date";
import timeZoneUtils from "../../utils.timeZone";
import {
    ExpressionUtils
} from "../../expressionUtils";
import {
    createAppointmentAdapter
} from "../../appointmentAdapter";
import {
    getAppointmentTakesAllDay
} from "../dataProvider/utils";
import getSkippedHoursInRange from "../../../../renovation/ui/scheduler/view_model/appointments/utils/getSkippedHoursInRange";
var ALLDAY_APPOINTMENT_MIN_VERTICAL_OFFSET = 5;
var ALLDAY_APPOINTMENT_MAX_VERTICAL_OFFSET = 20;
var toMs = dateUtils.dateToMilliseconds;
class VerticalRenderingStrategy extends BaseAppointmentsStrategy {
    getDeltaTime(args, initialSize, appointment) {
        var deltaTime = 0;
        if (this.isAllDay(appointment)) {
            deltaTime = this._getDeltaWidth(args, initialSize) * toMs("day")
        } else {
            var deltaHeight = args.height - initialSize.height;
            deltaTime = toMs("minute") * Math.round(deltaHeight / this.cellHeight * this.cellDurationInMinutes)
        }
        return deltaTime
    }
    _correctCollectorCoordinatesInAdaptive(coordinates, isAllDay) {
        if (isAllDay) {
            super._correctCollectorCoordinatesInAdaptive(coordinates, isAllDay)
        } else if (0 === this._getMaxAppointmentCountPerCellByType()) {
            var cellHeight = this.cellHeight;
            var cellWidth = this.cellWidth;
            coordinates.top += (cellHeight - this.getDropDownButtonAdaptiveSize()) / 2;
            coordinates.left += (cellWidth - this.getDropDownButtonAdaptiveSize()) / 2
        }
    }
    getAppointmentGeometry(coordinates) {
        var geometry = null;
        if (coordinates.allDay) {
            geometry = this._getAllDayAppointmentGeometry(coordinates)
        } else {
            geometry = this.isAdaptive && coordinates.isCompact ? this._getAdaptiveGeometry(coordinates) : this._getVerticalAppointmentGeometry(coordinates)
        }
        return super.getAppointmentGeometry(geometry)
    }
    _getAdaptiveGeometry(coordinates) {
        var config = this._calculateGeometryConfig(coordinates);
        return this._customizeCoordinates(coordinates, config.height, config.appointmentCountPerCell, config.offset)
    }
    _getItemPosition(appointment) {
        var adapter = createAppointmentAdapter(appointment, this.dataAccessors, this.timeZoneCalculator);
        var allDay = this.isAllDay(appointment);
        var isRecurring = !!adapter.recurrenceRule;
        var appointmentStartDate = adapter.calculateStartDate("toGrid");
        var appointmentEndDate = adapter.calculateEndDate("toGrid");
        var isAppointmentTakesSeveralDays = !timeZoneUtils.isSameAppointmentDates(appointmentStartDate, appointmentEndDate);
        if (allDay) {
            return super._getItemPosition(appointment)
        }
        var settings = this.generateAppointmentSettings(appointment);
        var result = [];
        for (var j = 0; j < settings.length; j++) {
            var currentSetting = settings[j];
            var height = this.calculateAppointmentHeight(appointment, currentSetting);
            var width = this.calculateAppointmentWidth(appointment, currentSetting);
            var resultHeight = height;
            var appointmentReduced = null;
            var multiDaysAppointmentParts = [];
            var currentMaxAllowedPosition = currentSetting.vMax;
            if (this._isMultiViewAppointment(currentSetting, height) || isAppointmentTakesSeveralDays && !isRecurring) {
                var reduceHead = dateUtils.sameDate(appointmentStartDate, currentSetting.info.appointment.startDate) || isRecurring;
                if (reduceHead) {
                    resultHeight = this._reduceMultiDayAppointment(height, {
                        top: currentSetting.top,
                        bottom: currentMaxAllowedPosition
                    });
                    multiDaysAppointmentParts = this._getAppointmentParts({
                        sourceAppointmentHeight: height,
                        reducedHeight: resultHeight,
                        width: width
                    }, currentSetting)
                }
                var isMultiDay = this._isMultiDayAppointment(currentSetting, height);
                if (isMultiDay) {
                    appointmentReduced = reduceHead ? "head" : "tail"
                }
            }
            extend(currentSetting, {
                height: resultHeight,
                width: width,
                allDay: allDay,
                appointmentReduced: appointmentReduced
            });
            result = this._getAppointmentPartsPosition(multiDaysAppointmentParts, currentSetting, result)
        }
        return result
    }
    _isMultiDayAppointment(position, height) {
        if (this.isVirtualScrolling) {
            var maxTop = this._getGroupHeight() - this._getGroupTopOffset(position);
            return height > maxTop
        }
        return false
    }
    _isMultiViewAppointment(position, height) {
        return height > position.vMax - position.top
    }
    _reduceMultiDayAppointment(sourceAppointmentHeight, bound) {
        sourceAppointmentHeight = bound.bottom - Math.floor(bound.top);
        return sourceAppointmentHeight
    }
    _getGroupHeight() {
        return this.cellHeight * this.rowCount
    }
    _getGroupTopOffset(appointmentSettings) {
        var {
            groupIndex: groupIndex
        } = appointmentSettings;
        var groupTop = Math.max(0, this.positionHelper.getGroupTop({
            groupIndex: groupIndex,
            showAllDayPanel: this.showAllDayPanel,
            isGroupedAllDayPanel: this.isGroupedAllDayPanel
        }));
        var allDayPanelOffset = this.positionHelper.getOffsetByAllDayPanel({
            groupIndex: groupIndex,
            supportAllDayRow: this.allDaySupported(),
            showAllDayPanel: this.showAllDayPanel
        });
        var appointmentGroupTopOffset = appointmentSettings.top - groupTop - allDayPanelOffset;
        return appointmentGroupTopOffset
    }
    _getTailHeight(appointmentGeometry, appointmentSettings) {
        if (!this.isVirtualScrolling) {
            return appointmentGeometry.sourceAppointmentHeight - appointmentGeometry.reducedHeight
        }
        var appointmentGroupTopOffset = this._getGroupTopOffset(appointmentSettings);
        var {
            sourceAppointmentHeight: sourceAppointmentHeight
        } = appointmentGeometry;
        var groupHeight = this._getGroupHeight();
        var tailHeight = appointmentGroupTopOffset + sourceAppointmentHeight - groupHeight;
        return tailHeight
    }
    _getAppointmentParts(appointmentGeometry, appointmentSettings) {
        var tailHeight = this._getTailHeight(appointmentGeometry, appointmentSettings);
        var width = appointmentGeometry.width;
        var result = [];
        var currentPartTop = Math.max(0, this.positionHelper.getGroupTop({
            groupIndex: appointmentSettings.groupIndex,
            showAllDayPanel: this.showAllDayPanel,
            isGroupedAllDayPanel: this.isGroupedAllDayPanel
        }));
        var cellsDiff = this.isGroupedByDate ? this.groupCount : 1;
        var offset = this.cellWidth * cellsDiff;
        var left = appointmentSettings.left + offset;
        if (tailHeight > 0) {
            var minHeight = this.getAppointmentMinSize();
            if (tailHeight < minHeight) {
                tailHeight = minHeight
            }
            var allDayPanelOffset = this.positionHelper.getOffsetByAllDayPanel({
                groupIndex: appointmentSettings.groupIndex,
                supportAllDayRow: this.allDaySupported(),
                showAllDayPanel: this.showAllDayPanel
            });
            currentPartTop += allDayPanelOffset;
            result.push(extend(true, {}, appointmentSettings, {
                top: currentPartTop,
                left: left,
                height: tailHeight,
                width: width,
                appointmentReduced: "tail",
                rowIndex: 0,
                columnIndex: appointmentSettings.columnIndex + cellsDiff
            }))
        }
        return result
    }
    _getMinuteHeight() {
        return this.cellHeight / this.cellDurationInMinutes
    }
    _getCompactLeftCoordinate(itemLeft, index) {
        var cellWidth = this.cellWidth || this.getAppointmentMinSize();
        return itemLeft + (1 + cellWidth) * index
    }
    _getVerticalAppointmentGeometry(coordinates) {
        var config = this._calculateVerticalGeometryConfig(coordinates);
        return this._customizeVerticalCoordinates(coordinates, config.width, config.appointmentCountPerCell, config.offset)
    }
    _customizeVerticalCoordinates(coordinates, width, appointmentCountPerCell, topOffset, isAllDay) {
        var appointmentWidth = Math.max(width / appointmentCountPerCell, width / coordinates.count);
        var height = coordinates.height;
        var appointmentLeft = coordinates.left + coordinates.index * appointmentWidth;
        var top = coordinates.top;
        if (coordinates.isCompact) {
            this._markAppointmentAsVirtual(coordinates, isAllDay)
        }
        return {
            height: height,
            width: appointmentWidth,
            top: top,
            left: appointmentLeft,
            empty: this._isAppointmentEmpty(height, width)
        }
    }
    _calculateVerticalGeometryConfig(coordinates) {
        var overlappingMode = this.maxAppointmentsPerCell;
        var offsets = this._getOffsets();
        var appointmentDefaultOffset = this._getAppointmentDefaultOffset();
        var appointmentCountPerCell = this._getAppointmentCount(overlappingMode, coordinates);
        var ratio = this._getDefaultRatio(coordinates, appointmentCountPerCell);
        var maxWidth = this._getMaxWidth();
        if (!appointmentCountPerCell) {
            appointmentCountPerCell = coordinates.count;
            ratio = (maxWidth - offsets.unlimited) / maxWidth
        }
        var topOffset = (1 - ratio) * maxWidth;
        if ("auto" === overlappingMode || isNumeric(overlappingMode)) {
            ratio = 1;
            maxWidth -= appointmentDefaultOffset;
            topOffset = 0
        }
        return {
            width: ratio * maxWidth,
            appointmentCountPerCell: appointmentCountPerCell,
            offset: topOffset
        }
    }
    _getMaxWidth() {
        return this.cellWidth || this.cellWidth
    }
    isAllDay(appointmentData) {
        var allDay = ExpressionUtils.getField(this.dataAccessors, "allDay", appointmentData);
        if (allDay) {
            return true
        }
        var adapter = createAppointmentAdapter(appointmentData, this.dataAccessors, this.timeZoneCalculator);
        return getAppointmentTakesAllDay(adapter, this.startDayHour, this.endDayHour)
    }
    _getAppointmentMaxWidth() {
        return this.cellWidth - this._getAppointmentDefaultOffset()
    }
    calculateAppointmentWidth(appointment, position) {
        if (!this.isAllDay(appointment)) {
            return 0
        }
        var startDate = dateUtils.trimTime(position.info.appointment.startDate);
        var {
            normalizedEndDate: normalizedEndDate
        } = position.info.appointment;
        var cellWidth = this.cellWidth || this.getAppointmentMinSize();
        var durationInHours = (normalizedEndDate.getTime() - startDate.getTime()) / toMs("hour");
        var skippedHours = getSkippedHoursInRange(position.info.appointment.startDate, position.info.appointment.endDate, this.viewDataProvider);
        var width = Math.ceil((durationInHours - skippedHours) / 24) * cellWidth;
        width = this.cropAppointmentWidth(width, cellWidth);
        return width
    }
    calculateAppointmentHeight(appointment, position) {
        if (this.isAllDay(appointment)) {
            return 0
        }
        var startDate = position.info.appointment.startDate;
        var {
            normalizedEndDate: normalizedEndDate
        } = position.info.appointment;
        var allDay = ExpressionUtils.getField(this.dataAccessors, "allDay", appointment);
        var duration = this.getAppointmentDurationInMs(startDate, normalizedEndDate, allDay);
        var durationInMinutes = this._adjustDurationByDaylightDiff(duration, startDate, normalizedEndDate) / toMs("minute");
        var height = durationInMinutes * this._getMinuteHeight();
        return height
    }
    getDirection() {
        return "vertical"
    }
    _sortCondition(a, b) {
        var allDayCondition = a.allDay - b.allDay;
        var isAllDay = a.allDay && b.allDay;
        var condition = "vertical" === this.groupOrientation && isAllDay ? this._columnCondition(a, b) : this._rowCondition(a, b);
        return allDayCondition ? allDayCondition : condition
    }
    allDaySupported() {
        return true
    }
    _getAllDayAppointmentGeometry(coordinates) {
        var config = this._calculateGeometryConfig(coordinates);
        return this._customizeCoordinates(coordinates, config.height, config.appointmentCountPerCell, config.offset, true)
    }
    _calculateGeometryConfig(coordinates) {
        if (!this.allowResizing || !this.allowAllDayResizing) {
            coordinates.skipResizing = true
        }
        var config = super._calculateGeometryConfig(coordinates);
        if (coordinates.count <= this._getDynamicAppointmentCountPerCell().allDay) {
            config.offset = 0
        }
        return config
    }
    _getAppointmentCount(overlappingMode, coordinates) {
        return "auto" !== overlappingMode && 1 === coordinates.count && !isNumeric(overlappingMode) ? coordinates.count : this._getMaxAppointmentCountPerCellByType(coordinates.allDay)
    }
    _getDefaultRatio(coordinates, appointmentCountPerCell) {
        return coordinates.count > this.appointmentCountPerCell ? .65 : 1
    }
    _getOffsets() {
        return {
            unlimited: ALLDAY_APPOINTMENT_MIN_VERTICAL_OFFSET,
            auto: ALLDAY_APPOINTMENT_MAX_VERTICAL_OFFSET
        }
    }
    _getMaxHeight() {
        return this.allDayHeight || this.getAppointmentMinSize()
    }
    _needVerticalGroupBounds(allDay) {
        return !allDay
    }
    _needHorizontalGroupBounds() {
        return false
    }
    getPositionShift(timeShift, isAllDay) {
        if (!isAllDay && this.isAdaptive && 0 === this._getMaxAppointmentCountPerCellByType(isAllDay)) {
            return {
                top: 0,
                left: 0,
                cellPosition: 0
            }
        }
        return super.getPositionShift(timeShift, isAllDay)
    }
}
export default VerticalRenderingStrategy;
