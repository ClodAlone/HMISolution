/**
 * DevExtreme (esm/ui/scheduler/appointments/cellPositionCalculator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isDefined
} from "../../../core/utils/type";
import dateUtils from "../../../core/utils/date";
class BaseStrategy {
    constructor(options) {
        this.options = options
    }
    get DOMMetaData() {
        return this.options.DOMMetaData
    }
    get appointments() {
        return this.options.dateSettings
    }
    get viewDataProvider() {
        return this.options.viewDataProvider
    }
    get positionHelper() {
        return this.options.positionHelper
    }
    get startViewDate() {
        return this.options.startViewDate
    }
    get viewStartDayHour() {
        return this.options.viewStartDayHour
    }
    get viewEndDayHour() {
        return this.options.viewEndDayHour
    }
    get cellDuration() {
        return this.options.cellDuration
    }
    get getPositionShift() {
        return this.options.getPositionShiftCallback
    }
    get groupCount() {
        return this.options.groupCount
    }
    get rtlEnabled() {
        return this.options.rtlEnabled
    }
    get isVerticalGrouping() {
        return this.options.isVerticalOrientation
    }
    get showAllDayPanel() {
        return this.options.showAllDayPanel
    }
    get supportAllDayRow() {
        return this.options.supportAllDayRow
    }
    get isGroupedAllDayPanel() {
        return this.options.isGroupedAllDayPanel
    }
    get isVirtualScrolling() {
        return false
    }
    calculateCellPositions(groupIndices, isAllDayRowAppointment, isRecurrentAppointment) {
        var result = [];
        this.appointments.forEach((dateSetting, index) => {
            var coordinates = this.getCoordinateInfos({
                appointment: dateSetting,
                groupIndices: groupIndices,
                isAllDayRowAppointment: isAllDayRowAppointment,
                isRecurrentAppointment: isRecurrentAppointment
            });
            coordinates.forEach(item => {
                !!item && result.push(this._prepareObject(item, index))
            })
        });
        return result
    }
    getCoordinateInfos(options) {
        var {
            appointment: appointment,
            isAllDayRowAppointment: isAllDayRowAppointment,
            groupIndices: groupIndices,
            recurrent: recurrent
        } = options;
        var {
            startDate: startDate
        } = appointment;
        var groupIndex = !recurrent ? appointment.source.groupIndex : void 0;
        return this.getCoordinatesByDateInGroup(startDate, groupIndices, isAllDayRowAppointment, groupIndex)
    }
    _prepareObject(position, dateSettingIndex) {
        position.dateSettingIndex = dateSettingIndex;
        return {
            coordinates: position,
            dateSettingIndex: dateSettingIndex
        }
    }
    getCoordinatesByDate(date, groupIndex, inAllDayRow) {
        var validGroupIndex = groupIndex || 0;
        var cellInfo = {
            groupIndex: validGroupIndex,
            startDate: date,
            isAllDay: inAllDayRow
        };
        var positionByMap = this.viewDataProvider.findCellPositionInMap(cellInfo);
        if (!positionByMap) {
            return
        }
        var position = this.getCellPosition(positionByMap, inAllDayRow && !this.isVerticalGrouping);
        var timeShift = inAllDayRow ? 0 : this.getTimeShift(date);
        var shift = this.getPositionShift(timeShift, inAllDayRow);
        var horizontalHMax = this.positionHelper.getHorizontalMax(validGroupIndex, date);
        var verticalMax = this.positionHelper.getVerticalMax({
            groupIndex: validGroupIndex,
            isVirtualScrolling: this.isVirtualScrolling,
            showAllDayPanel: this.showAllDayPanel,
            supportAllDayRow: this.supportAllDayRow,
            isGroupedAllDayPanel: this.isGroupedAllDayPanel,
            isVerticalGrouping: this.isVerticalGrouping
        });
        return {
            positionByMap: positionByMap,
            cellPosition: position.left + shift.cellPosition,
            top: position.top + shift.top,
            left: position.left + shift.left,
            rowIndex: position.rowIndex,
            columnIndex: position.columnIndex,
            hMax: horizontalHMax,
            vMax: verticalMax,
            groupIndex: validGroupIndex
        }
    }
    getCoordinatesByDateInGroup(startDate, groupIndices, inAllDayRow, groupIndex) {
        var result = [];
        if (this.viewDataProvider.isSkippedDate(startDate)) {
            return result
        }
        var validGroupIndices = [groupIndex];
        if (!isDefined(groupIndex)) {
            validGroupIndices = this.groupCount ? groupIndices : [0]
        }
        validGroupIndices.forEach(groupIndex => {
            var coordinates = this.getCoordinatesByDate(startDate, groupIndex, inAllDayRow);
            if (coordinates) {
                result.push(coordinates)
            }
        });
        return result
    }
    getCellPosition(cellCoordinates, isAllDayPanel) {
        var {
            dateTableCellsMeta: dateTableCellsMeta,
            allDayPanelCellsMeta: allDayPanelCellsMeta
        } = this.DOMMetaData;
        var {
            columnIndex: columnIndex,
            rowIndex: rowIndex
        } = cellCoordinates;
        var position = isAllDayPanel ? allDayPanelCellsMeta[columnIndex] : dateTableCellsMeta[rowIndex][columnIndex];
        var validPosition = _extends({}, position);
        if (this.rtlEnabled) {
            validPosition.left += position.width
        }
        if (validPosition) {
            validPosition.rowIndex = cellCoordinates.rowIndex;
            validPosition.columnIndex = cellCoordinates.columnIndex
        }
        return validPosition
    }
    getTimeShift(date) {
        var currentDayStart = new Date(date);
        var currentDayEndHour = new Date(new Date(date).setHours(this.viewEndDayHour, 0, 0));
        if (date.getTime() <= currentDayEndHour.getTime()) {
            currentDayStart.setHours(this.viewStartDayHour, 0, 0, 0)
        }
        var timeZoneDifference = dateUtils.getTimezonesDifference(date, currentDayStart);
        var currentDateTime = date.getTime();
        var currentDayStartTime = currentDayStart.getTime();
        var minTime = this.startViewDate.getTime();
        return currentDateTime > minTime ? (currentDateTime - currentDayStartTime + timeZoneDifference) % this.cellDuration / this.cellDuration : 0
    }
}
class VirtualStrategy extends BaseStrategy {
    get isVirtualScrolling() {
        return true
    }
    calculateCellPositions(groupIndices, isAllDayRowAppointment, isRecurrentAppointment) {
        var appointments = isAllDayRowAppointment ? this.appointments : this.appointments.filter(_ref => {
            var {
                source: source,
                startDate: startDate,
                endDate: endDate
            } = _ref;
            return this.viewDataProvider.isGroupIntersectDateInterval(source.groupIndex, startDate, endDate)
        });
        if (isRecurrentAppointment) {
            return this.createRecurrentAppointmentInfos(appointments, isAllDayRowAppointment)
        }
        return super.calculateCellPositions(groupIndices, isAllDayRowAppointment, isRecurrentAppointment)
    }
    createRecurrentAppointmentInfos(dateSettings, isAllDayRowAppointment) {
        var result = [];
        dateSettings.forEach((_ref2, index) => {
            var {
                source: source,
                startDate: startDate
            } = _ref2;
            var coordinate = this.getCoordinatesByDate(startDate, source.groupIndex, isAllDayRowAppointment);
            if (coordinate) {
                result.push(this._prepareObject(coordinate, index))
            }
        });
        return result
    }
}
export class CellPositionCalculator {
    constructor(options) {
        this.options = options
    }
    calculateCellPositions(groupIndices, isAllDayRowAppointment, isRecurrentAppointment) {
        var strategy = this.options.isVirtualScrolling ? new VirtualStrategy(this.options) : new BaseStrategy(this.options);
        return strategy.calculateCellPositions(groupIndices, isAllDayRowAppointment, isRecurrentAppointment)
    }
}
