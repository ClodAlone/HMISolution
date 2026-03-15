/**
 * DevExtreme (esm/ui/scheduler/appointments/settingsGenerator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import dateUtils from "../../../core/utils/date";
import {
    isEmptyObject
} from "../../../core/utils/type";
import {
    extend
} from "../../../core/utils/extend";
import {
    getRecurrenceProcessor
} from "../recurrence";
import timeZoneUtils from "../utils.timeZone";
import {
    createResourcesTree,
    getDataAccessors,
    getGroupCount,
    getResourceTreeLeaves
} from "../resources/utils";
import {
    createAppointmentAdapter
} from "../appointmentAdapter";
import {
    CellPositionCalculator
} from "./cellPositionCalculator";
import {
    ExpressionUtils
} from "../expressionUtils";
import {
    isDateAndTimeView
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
import {
    createFormattedDateText
} from "./textUtils";
var toMs = dateUtils.dateToMilliseconds;
var APPOINTMENT_DATE_TEXT_FORMAT = "TIME";
export class DateGeneratorBaseStrategy {
    constructor(options) {
        this.options = options
    }
    get rawAppointment() {
        return this.options.rawAppointment
    }
    get timeZoneCalculator() {
        return this.options.timeZoneCalculator
    }
    get viewDataProvider() {
        return this.options.viewDataProvider
    }
    get appointmentTakesAllDay() {
        return this.options.appointmentTakesAllDay
    }
    get supportAllDayRow() {
        return this.options.supportAllDayRow
    }
    get isAllDayRowAppointment() {
        return this.options.isAllDayRowAppointment
    }
    get timeZone() {
        return this.options.timeZone
    }
    get dateRange() {
        return this.options.dateRange
    }
    get firstDayOfWeek() {
        return this.options.firstDayOfWeek
    }
    get viewStartDayHour() {
        return this.options.viewStartDayHour
    }
    get viewEndDayHour() {
        return this.options.viewEndDayHour
    }
    get endViewDate() {
        return this.options.endViewDate
    }
    get viewType() {
        return this.options.viewType
    }
    get isGroupedByDate() {
        return this.options.isGroupedByDate
    }
    get isVerticalOrientation() {
        return this.options.isVerticalOrientation
    }
    get dataAccessors() {
        return this.options.dataAccessors
    }
    get loadedResources() {
        return this.options.loadedResources
    }
    get isDateAppointment() {
        return !isDateAndTimeView(this.viewType) && this.appointmentTakesAllDay
    }
    getIntervalDuration() {
        return this.appointmentTakesAllDay ? this.options.allDayIntervalDuration : this.options.intervalDuration
    }
    generate(appointmentAdapter) {
        var itemGroupIndices = this._getGroupIndices(this.rawAppointment);
        var appointmentList = this._createAppointments(appointmentAdapter, itemGroupIndices);
        appointmentList = this._getProcessedByAppointmentTimeZone(appointmentList, appointmentAdapter);
        if (this._canProcessNotNativeTimezoneDates(appointmentAdapter)) {
            appointmentList = this._getProcessedNotNativeTimezoneDates(appointmentList, appointmentAdapter)
        }
        var dateSettings = this._createGridAppointmentList(appointmentList, appointmentAdapter);
        dateSettings = this._cropAppointmentsByStartDayHour(dateSettings, this.rawAppointment);
        dateSettings = this._fillNormalizedEndDate(dateSettings, this.rawAppointment);
        if (this._needSeparateLongParts()) {
            dateSettings = this._separateLongParts(dateSettings, appointmentAdapter)
        }
        var {
            isRecurrent: isRecurrent
        } = appointmentAdapter;
        return {
            dateSettings: dateSettings,
            itemGroupIndices: itemGroupIndices,
            isRecurrent: isRecurrent
        }
    }
    _getProcessedByAppointmentTimeZone(appointmentList, appointment) {
        var hasAppointmentTimeZone = !isEmptyObject(appointment.startDateTimeZone) || !isEmptyObject(appointment.endDateTimeZone);
        if (hasAppointmentTimeZone) {
            var appointmentOffsets = {
                startDate: this.timeZoneCalculator.getOffsets(appointment.startDate, appointment.startDateTimeZone),
                endDate: this.timeZoneCalculator.getOffsets(appointment.endDate, appointment.endDateTimeZone)
            };
            appointmentList.forEach(a => {
                var sourceOffsets_startDate = this.timeZoneCalculator.getOffsets(a.startDate, appointment.startDateTimeZone),
                    sourceOffsets_endDate = this.timeZoneCalculator.getOffsets(a.endDate, appointment.endDateTimeZone);
                var startDateOffsetDiff = appointmentOffsets.startDate.appointment - sourceOffsets_startDate.appointment;
                var endDateOffsetDiff = appointmentOffsets.endDate.appointment - sourceOffsets_endDate.appointment;
                if (sourceOffsets_startDate.appointment !== sourceOffsets_startDate.common) {
                    a.startDate = new Date(a.startDate.getTime() + startDateOffsetDiff * toMs("hour"))
                }
                if (sourceOffsets_endDate.appointment !== sourceOffsets_endDate.common) {
                    a.endDate = new Date(a.endDate.getTime() + endDateOffsetDiff * toMs("hour"))
                }
            })
        }
        return appointmentList
    }
    _createAppointments(appointment, groupIndices) {
        var appointments = this._createRecurrenceAppointments(appointment, groupIndices);
        if (!appointment.isRecurrent && 0 === appointments.length) {
            appointments.push({
                startDate: appointment.startDate,
                endDate: appointment.endDate
            })
        }
        appointments = appointments.map(item => {
            var _item$endDate;
            var resultEndTime = null === (_item$endDate = item.endDate) || void 0 === _item$endDate ? void 0 : _item$endDate.getTime();
            if (item.startDate.getTime() === resultEndTime) {
                item.endDate.setTime(resultEndTime + toMs("minute"))
            }
            return _extends({}, item, {
                exceptionDate: new Date(item.startDate)
            })
        });
        return appointments
    }
    _canProcessNotNativeTimezoneDates(appointment) {
        var isTimeZoneSet = !isEmptyObject(this.timeZone);
        if (!isTimeZoneSet) {
            return false
        }
        if (!appointment.isRecurrent) {
            return false
        }
        return !timeZoneUtils.isEqualLocalTimeZone(this.timeZone, appointment.startDate)
    }
    _getProcessedNotNativeDateIfCrossDST(date, offset) {
        if (offset < 0) {
            var newDate = new Date(date);
            var newDateMinusOneHour = new Date(newDate);
            newDateMinusOneHour.setHours(newDateMinusOneHour.getHours() - 1);
            var newDateOffset = this.timeZoneCalculator.getOffsets(newDate).common;
            var newDateMinusOneHourOffset = this.timeZoneCalculator.getOffsets(newDateMinusOneHour).common;
            if (newDateOffset !== newDateMinusOneHourOffset) {
                return 0
            }
        }
        return offset
    }
    _getCommonOffset(date) {
        return this.timeZoneCalculator.getOffsets(date).common
    }
    _getProcessedNotNativeTimezoneDates(appointmentList, appointment) {
        return appointmentList.map(item => {
            var diffStartDateOffset = this._getCommonOffset(appointment.startDate) - this._getCommonOffset(item.startDate);
            var diffEndDateOffset = this._getCommonOffset(appointment.endDate) - this._getCommonOffset(item.endDate);
            if (0 === diffStartDateOffset && 0 === diffEndDateOffset) {
                return item
            }
            diffStartDateOffset = this._getProcessedNotNativeDateIfCrossDST(item.startDate, diffStartDateOffset);
            diffEndDateOffset = this._getProcessedNotNativeDateIfCrossDST(item.endDate, diffEndDateOffset);
            var newStartDate = new Date(item.startDate.getTime() + diffStartDateOffset * toMs("hour"));
            var newEndDate = new Date(item.endDate.getTime() + diffEndDateOffset * toMs("hour"));
            var testNewStartDate = this.timeZoneCalculator.createDate(newStartDate, {
                path: "toGrid"
            });
            var testNewEndDate = this.timeZoneCalculator.createDate(newEndDate, {
                path: "toGrid"
            });
            if (appointment.duration > testNewEndDate.getTime() - testNewStartDate.getTime()) {
                newEndDate = new Date(newStartDate.getTime() + appointment.duration)
            }
            return _extends({}, item, {
                startDate: newStartDate,
                endDate: newEndDate,
                exceptionDate: new Date(newStartDate)
            })
        })
    }
    _needSeparateLongParts() {
        return this.isVerticalOrientation ? this.isGroupedByDate : this.isGroupedByDate && this.appointmentTakesAllDay
    }
    normalizeEndDateByViewEnd(rawAppointment, endDate) {
        var result = new Date(endDate.getTime());
        var isAllDay = isDateAndTimeView(this.viewType) && this.appointmentTakesAllDay;
        if (!isAllDay) {
            var roundedEndViewDate = dateUtils.roundToHour(this.endViewDate);
            if (result > roundedEndViewDate) {
                result = roundedEndViewDate
            }
        }
        var endDayHour = this.viewEndDayHour;
        var allDay = ExpressionUtils.getField(this.dataAccessors, "allDay", rawAppointment);
        var currentViewEndTime = new Date(new Date(endDate.getTime()).setHours(endDayHour, 0, 0, 0));
        if (result.getTime() > currentViewEndTime.getTime() || allDay && result.getHours() < endDayHour) {
            result = currentViewEndTime
        }
        return result
    }
    _fillNormalizedEndDate(dateSettings, rawAppointment) {
        return dateSettings.map(item => {
            var {
                endDate: endDate
            } = item;
            var normalizedEndDate = this.normalizeEndDateByViewEnd(rawAppointment, endDate);
            return _extends({}, item, {
                normalizedEndDate: normalizedEndDate
            })
        })
    }
    _separateLongParts(gridAppointmentList, appointmentAdapter) {
        var result = [];
        gridAppointmentList.forEach(gridAppointment => {
            var maxDate = new Date(this.dateRange[1]);
            var {
                normalizedEndDate: endDateOfPart
            } = gridAppointment;
            var longStartDateParts = dateUtils.getDatesOfInterval(gridAppointment.startDate, endDateOfPart, {
                milliseconds: this.getIntervalDuration(this.appointmentTakesAllDay)
            });
            var list = longStartDateParts.filter(startDatePart => new Date(startDatePart) < maxDate).map(date => {
                var endDate = new Date(new Date(date).setMilliseconds(appointmentAdapter.duration));
                var normalizedEndDate = this.normalizeEndDateByViewEnd(this.rawAppointment, endDate);
                return {
                    startDate: date,
                    endDate: endDate,
                    normalizedEndDate: normalizedEndDate,
                    source: gridAppointment.source
                }
            });
            result = result.concat(list)
        });
        return result
    }
    _createGridAppointmentList(appointmentList, appointment) {
        return appointmentList.map(source => {
            var offsetDifference = appointment.startDate.getTimezoneOffset() - source.startDate.getTimezoneOffset();
            if (0 !== offsetDifference && this._canProcessNotNativeTimezoneDates(appointment)) {
                source.startDate = new Date(source.startDate.getTime() + offsetDifference * toMs("minute"));
                source.endDate = new Date(source.endDate.getTime() + offsetDifference * toMs("minute"));
                source.exceptionDate = new Date(source.startDate)
            }
            var startDate = this.timeZoneCalculator.createDate(source.startDate, {
                path: "toGrid"
            });
            var endDate = this.timeZoneCalculator.createDate(source.endDate, {
                path: "toGrid"
            });
            return {
                startDate: startDate,
                endDate: endDate,
                source: source
            }
        })
    }
    _createExtremeRecurrenceDates() {
        var startViewDate = this.appointmentTakesAllDay ? dateUtils.trimTime(this.dateRange[0]) : this.dateRange[0];
        var endViewDateByEndDayHour = this.dateRange[1];
        if (this.timeZone) {
            startViewDate = this.timeZoneCalculator.createDate(startViewDate, {
                path: "fromGrid"
            });
            endViewDateByEndDayHour = this.timeZoneCalculator.createDate(endViewDateByEndDayHour, {
                path: "fromGrid"
            });
            var daylightOffset = timeZoneUtils.getDaylightOffsetInMs(startViewDate, endViewDateByEndDayHour);
            if (daylightOffset) {
                endViewDateByEndDayHour = new Date(endViewDateByEndDayHour.getTime() + daylightOffset)
            }
        }
        return [startViewDate, endViewDateByEndDayHour]
    }
    _createRecurrenceOptions(appointment, groupIndex) {
        var [minRecurrenceDate, maxRecurrenceDate] = this._createExtremeRecurrenceDates(groupIndex);
        return {
            rule: appointment.recurrenceRule,
            exception: appointment.recurrenceException,
            min: minRecurrenceDate,
            max: maxRecurrenceDate,
            firstDayOfWeek: this.firstDayOfWeek,
            start: appointment.startDate,
            end: appointment.endDate,
            getPostProcessedException: date => {
                if (isEmptyObject(this.timeZone) || timeZoneUtils.isEqualLocalTimeZone(this.timeZone, date)) {
                    return date
                }
                var appointmentOffset = this.timeZoneCalculator.getOffsets(appointment.startDate).common;
                var exceptionAppointmentOffset = this.timeZoneCalculator.getOffsets(date).common;
                var diff = appointmentOffset - exceptionAppointmentOffset;
                diff = this._getProcessedNotNativeDateIfCrossDST(date, diff);
                return new Date(date.getTime() - diff * dateUtils.dateToMilliseconds("hour"))
            }
        }
    }
    _createRecurrenceAppointments(appointment, groupIndices) {
        var {
            duration: duration
        } = appointment;
        var option = this._createRecurrenceOptions(appointment);
        var generatedStartDates = getRecurrenceProcessor().generateDates(option);
        return generatedStartDates.map(date => {
            var utcDate = timeZoneUtils.createUTCDateWithLocalOffset(date);
            utcDate.setTime(utcDate.getTime() + duration);
            var endDate = timeZoneUtils.createDateFromUTCWithLocalOffset(utcDate);
            return {
                startDate: new Date(date),
                endDate: endDate
            }
        })
    }
    _cropAppointmentsByStartDayHour(appointments, rawAppointment) {
        return appointments.filter(appointment => {
            var firstViewDate = this._getAppointmentFirstViewDate(appointment);
            if (!firstViewDate) {
                return false
            }
            var startDayHour = this._getViewStartDayHour(firstViewDate);
            var startDate = new Date(appointment.startDate);
            appointment.startDate = this._getAppointmentResultDate({
                appointment: appointment,
                rawAppointment: rawAppointment,
                startDate: startDate,
                startDayHour: startDayHour,
                firstViewDate: firstViewDate
            });
            return !this.isAllDayRowAppointment ? appointment.endDate > appointment.startDate : true
        })
    }
    _getViewStartDayHour() {
        return this.viewStartDayHour
    }
    _getAppointmentResultDate(options) {
        var {
            appointment: appointment,
            startDayHour: startDayHour,
            firstViewDate: firstViewDate
        } = options;
        var {
            startDate: startDate
        } = options;
        var resultDate = new Date(appointment.startDate);
        if (this.appointmentTakesAllDay) {
            resultDate = dateUtils.normalizeDate(startDate, firstViewDate)
        } else {
            if (startDate < firstViewDate) {
                startDate = firstViewDate
            }
            resultDate = dateUtils.normalizeDate(appointment.startDate, startDate)
        }
        return !this.isDateAppointment ? dateUtils.roundDateByStartDayHour(resultDate, startDayHour) : resultDate
    }
    _getAppointmentFirstViewDate(appointment) {
        var groupIndex = appointment.source.groupIndex || 0;
        var {
            startDate: startDate,
            endDate: endDate
        } = appointment;
        return this.viewDataProvider.findGroupCellStartDate(groupIndex, startDate, endDate, this.isAllDayRowAppointment, this.isDateAppointment)
    }
    _getGroupIndices(rawAppointment) {
        var result = [];
        if (rawAppointment && this.loadedResources.length) {
            var tree = createResourcesTree(this.loadedResources);
            result = getResourceTreeLeaves((field, action) => getDataAccessors(this.options.dataAccessors.resources, field, action), tree, rawAppointment)
        }
        return result
    }
}
export class DateGeneratorVirtualStrategy extends DateGeneratorBaseStrategy {
    get groupCount() {
        return getGroupCount(this.loadedResources)
    }
    _createRecurrenceAppointments(appointment, groupIndices) {
        var {
            duration: duration
        } = appointment;
        var result = [];
        var validGroupIndices = this.groupCount ? groupIndices : [0];
        validGroupIndices.forEach(groupIndex => {
            var option = this._createRecurrenceOptions(appointment, groupIndex);
            var generatedStartDates = getRecurrenceProcessor().generateDates(option);
            var recurrentInfo = generatedStartDates.map(date => {
                var startDate = new Date(date);
                var utcDate = timeZoneUtils.createUTCDateWithLocalOffset(date);
                utcDate.setTime(utcDate.getTime() + duration);
                var endDate = timeZoneUtils.createDateFromUTCWithLocalOffset(utcDate);
                return {
                    startDate: startDate,
                    endDate: endDate,
                    groupIndex: groupIndex
                }
            });
            result.push(...recurrentInfo)
        });
        return result
    }
    _getViewStartDayHour(firstViewDate) {
        return firstViewDate.getHours()
    }
    _updateGroupIndices(appointments, groupIndices) {
        var result = [];
        groupIndices.forEach(groupIndex => {
            var groupStartDate = this.viewDataProvider.getGroupStartDate(groupIndex);
            if (groupStartDate) {
                appointments.forEach(appointment => {
                    var appointmentCopy = extend({}, appointment);
                    appointmentCopy.groupIndex = groupIndex;
                    result.push(appointmentCopy)
                })
            }
        });
        return result
    }
    _getGroupIndices(resources) {
        var _groupIndices;
        var groupIndices = super._getGroupIndices(resources);
        var viewDataGroupIndices = this.viewDataProvider.getGroupIndices();
        if (!(null !== (_groupIndices = groupIndices) && void 0 !== _groupIndices && _groupIndices.length)) {
            groupIndices = [0]
        }
        return groupIndices.filter(groupIndex => -1 !== viewDataGroupIndices.indexOf(groupIndex))
    }
    _createAppointments(appointment, groupIndices) {
        var appointments = super._createAppointments(appointment, groupIndices);
        return !appointment.isRecurrent ? this._updateGroupIndices(appointments, groupIndices) : appointments
    }
}
export class AppointmentSettingsGenerator {
    constructor(options) {
        this.options = options;
        this.appointmentAdapter = createAppointmentAdapter(this.rawAppointment, this.dataAccessors, this.timeZoneCalculator)
    }
    get rawAppointment() {
        return this.options.rawAppointment
    }
    get dataAccessors() {
        return this.options.dataAccessors
    }
    get timeZoneCalculator() {
        return this.options.timeZoneCalculator
    }
    get isAllDayRowAppointment() {
        return this.options.appointmentTakesAllDay && this.options.supportAllDayRow
    }
    get modelGroups() {
        return this.options.modelGroups
    }
    get dateSettingsStrategy() {
        var options = _extends({}, this.options, {
            isAllDayRowAppointment: this.isAllDayRowAppointment
        });
        return this.options.isVirtualScrolling ? new DateGeneratorVirtualStrategy(options) : new DateGeneratorBaseStrategy(options)
    }
    create() {
        var {
            dateSettings: dateSettings,
            itemGroupIndices: itemGroupIndices,
            isRecurrent: isRecurrent
        } = this._generateDateSettings();
        var cellPositions = this._calculateCellPositions(dateSettings, itemGroupIndices);
        var result = this._prepareAppointmentInfos(dateSettings, cellPositions, isRecurrent);
        return result
    }
    _generateDateSettings() {
        return this.dateSettingsStrategy.generate(this.appointmentAdapter)
    }
    _calculateCellPositions(dateSettings, itemGroupIndices) {
        var cellPositionCalculator = new CellPositionCalculator(_extends({}, this.options, {
            dateSettings: dateSettings
        }));
        return cellPositionCalculator.calculateCellPositions(itemGroupIndices, this.isAllDayRowAppointment, this.appointmentAdapter.isRecurrent)
    }
    _prepareAppointmentInfos(dateSettings, cellPositions, isRecurrent) {
        var infos = [];
        cellPositions.forEach(_ref => {
            var {
                coordinates: coordinates,
                dateSettingIndex: dateSettingIndex
            } = _ref;
            var dateSetting = dateSettings[dateSettingIndex];
            var dateText = this._getAppointmentDateText(dateSetting);
            var info = {
                appointment: dateSetting,
                sourceAppointment: dateSetting.source,
                dateText: dateText,
                isRecurrent: isRecurrent
            };
            this._setResourceColor(info, coordinates.groupIndex);
            infos.push(_extends({}, coordinates, {
                info: info
            }))
        });
        return infos
    }
    _getAppointmentDateText(sourceAppointment) {
        var {
            startDate: startDate,
            endDate: endDate,
            allDay: allDay
        } = sourceAppointment;
        return createFormattedDateText({
            startDate: startDate,
            endDate: endDate,
            allDay: allDay,
            format: APPOINTMENT_DATE_TEXT_FORMAT
        })
    }
    _setResourceColor(info, groupIndex) {
        var appointmentConfig = {
            itemData: this.rawAppointment,
            groupIndex: groupIndex,
            groups: this.modelGroups
        };
        this.options.getAppointmentColor(appointmentConfig).done(color => info.resourceColor = color)
    }
}
