/**
 * DevExtreme (esm/ui/scheduler/utils.timeZone.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import dateUtils from "../../core/utils/date";
import timeZoneDataUtils from "./timezones/utils.timezones_data";
import DateAdapter from "./dateAdapter";
var toMs = dateUtils.dateToMilliseconds;
var MINUTES_IN_HOUR = 60;
var createUTCDateWithLocalOffset = date => {
    if (!date) {
        return null
    }
    return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds()))
};
var createDateFromUTCWithLocalOffset = date => {
    var result = DateAdapter(date);
    var timezoneOffsetBeforeInMin = result.getTimezoneOffset();
    result.addTime(result.getTimezoneOffset("minute"));
    result.subtractMinutes(timezoneOffsetBeforeInMin - result.getTimezoneOffset());
    return result.source
};
var getTimeZones = function() {
    var date = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : new Date;
    var dateInUTC = createUTCDate(date);
    return timeZoneDataUtils.getDisplayedTimeZones(dateInUTC.getTime())
};
var createUTCDate = date => new Date(Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes()));
var getTimezoneOffsetChangeInMinutes = (startDate, endDate, updatedStartDate, updatedEndDate) => getDaylightOffset(updatedStartDate, updatedEndDate) - getDaylightOffset(startDate, endDate);
var getTimezoneOffsetChangeInMs = (startDate, endDate, updatedStartDate, updatedEndDate) => getTimezoneOffsetChangeInMinutes(startDate, endDate, updatedStartDate, updatedEndDate) * toMs("minute");
var getDaylightOffset = (startDate, endDate) => new Date(startDate).getTimezoneOffset() - new Date(endDate).getTimezoneOffset();
var getDaylightOffsetInMs = (startDate, endDate) => getDaylightOffset(startDate, endDate) * toMs("minute");
var calculateTimezoneByValue = function(timezone) {
    var date = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : new Date;
    if ("string" === typeof timezone) {
        var dateUtc = createUTCDate(date);
        return timeZoneDataUtils.getTimeZoneOffsetById(timezone, dateUtc.getTime())
    }
    return timezone
};
var _getDaylightOffsetByTimezone = (startDate, endDate, timeZone) => calculateTimezoneByValue(timeZone, startDate) - calculateTimezoneByValue(timeZone, endDate);
var getCorrectedDateByDaylightOffsets = (convertedOriginalStartDate, convertedDate, date, timeZone, startDateTimezone) => {
    var daylightOffsetByCommonTimezone = _getDaylightOffsetByTimezone(convertedOriginalStartDate, convertedDate, timeZone);
    var daylightOffsetByAppointmentTimezone = _getDaylightOffsetByTimezone(convertedOriginalStartDate, convertedDate, startDateTimezone);
    var diff = daylightOffsetByCommonTimezone - daylightOffsetByAppointmentTimezone;
    return new Date(date.getTime() - diff * toMs("hour"))
};
var correctRecurrenceExceptionByTimezone = function(exception, exceptionByStartDate, timeZone, startDateTimeZone) {
    var isBackConversion = arguments.length > 4 && void 0 !== arguments[4] ? arguments[4] : false;
    var timezoneOffset = (exception.getTimezoneOffset() - exceptionByStartDate.getTimezoneOffset()) / MINUTES_IN_HOUR;
    if (startDateTimeZone) {
        timezoneOffset = _getDaylightOffsetByTimezone(exceptionByStartDate, exception, startDateTimeZone)
    } else if (timeZone) {
        timezoneOffset = _getDaylightOffsetByTimezone(exceptionByStartDate, exception, timeZone)
    }
    return new Date(exception.getTime() + (isBackConversion ? -1 : 1) * timezoneOffset * toMs("hour"))
};
var isTimezoneChangeInDate = date => {
    var startDayDate = new Date(new Date(date).setHours(0, 0, 0, 0));
    var endDayDate = new Date(new Date(date).setHours(23, 59, 59, 0));
    return startDayDate.getTimezoneOffset() - endDayDate.getTimezoneOffset() !== 0
};
var getDateWithoutTimezoneChange = date => {
    var clonedDate = new Date(date);
    if (isTimezoneChangeInDate(clonedDate)) {
        var result = new Date(clonedDate);
        return new Date(result.setDate(result.getDate() + 1))
    }
    return clonedDate
};
var isSameAppointmentDates = (startDate, endDate) => {
    endDate = new Date(endDate.getTime() - 1);
    return dateUtils.sameDate(startDate, endDate)
};
var getClientTimezoneOffset = function() {
    var date = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : new Date;
    return 6e4 * date.getTimezoneOffset()
};
var isEqualLocalTimeZone = function(timeZoneName) {
    var date = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : new Date;
    if (Intl) {
        var localTimeZoneName = Intl.DateTimeFormat().resolvedOptions().timeZone;
        if (localTimeZoneName === timeZoneName) {
            return true
        }
    }
    return isEqualLocalTimeZoneByDeclaration(timeZoneName, date)
};
var hasDSTInLocalTimeZone = () => {
    var [startDate, endDate] = getExtremeDates();
    return startDate.getTimezoneOffset() !== endDate.getTimezoneOffset()
};
var isEqualLocalTimeZoneByDeclaration = (timeZoneName, date) => {
    var year = date.getFullYear();
    var getOffset = date => -date.getTimezoneOffset() / 60;
    var getDateAndMoveHourBack = dateStamp => new Date(dateStamp - 36e5);
    var configTuple = timeZoneDataUtils.getTimeZoneDeclarationTuple(timeZoneName, year);
    var [summerTime, winterTime] = configTuple;
    var noDSTInTargetTimeZone = configTuple.length < 2;
    if (noDSTInTargetTimeZone) {
        var targetTimeZoneOffset = timeZoneDataUtils.getTimeZoneOffsetById(timeZoneName, date);
        var localTimeZoneOffset = getOffset(date);
        if (targetTimeZoneOffset !== localTimeZoneOffset) {
            return false
        }
        return hasDSTInLocalTimeZone() ? false : true
    }
    var localSummerOffset = getOffset(new Date(summerTime.date));
    var localWinterOffset = getOffset(new Date(winterTime.date));
    if (localSummerOffset !== summerTime.offset) {
        return false
    }
    if (localSummerOffset === getOffset(getDateAndMoveHourBack(summerTime.date))) {
        return false
    }
    if (localWinterOffset !== winterTime.offset) {
        return false
    }
    if (localWinterOffset === getOffset(getDateAndMoveHourBack(winterTime.date))) {
        return false
    }
    return true
};
var getExtremeDates = () => {
    var nowDate = new Date(Date.now());
    var startDate = new Date;
    var endDate = new Date;
    startDate.setFullYear(nowDate.getFullYear(), 0, 1);
    endDate.setFullYear(nowDate.getFullYear(), 6, 1);
    return [startDate, endDate]
};
var utils = {
    getDaylightOffset: getDaylightOffset,
    getDaylightOffsetInMs: getDaylightOffsetInMs,
    getTimezoneOffsetChangeInMinutes: getTimezoneOffsetChangeInMinutes,
    getTimezoneOffsetChangeInMs: getTimezoneOffsetChangeInMs,
    calculateTimezoneByValue: calculateTimezoneByValue,
    getCorrectedDateByDaylightOffsets: getCorrectedDateByDaylightOffsets,
    isSameAppointmentDates: isSameAppointmentDates,
    correctRecurrenceExceptionByTimezone: correctRecurrenceExceptionByTimezone,
    getClientTimezoneOffset: getClientTimezoneOffset,
    createUTCDateWithLocalOffset: createUTCDateWithLocalOffset,
    createDateFromUTCWithLocalOffset: createDateFromUTCWithLocalOffset,
    createUTCDate: createUTCDate,
    isTimezoneChangeInDate: isTimezoneChangeInDate,
    getDateWithoutTimezoneChange: getDateWithoutTimezoneChange,
    hasDSTInLocalTimeZone: hasDSTInLocalTimeZone,
    isEqualLocalTimeZone: isEqualLocalTimeZone,
    isEqualLocalTimeZoneByDeclaration: isEqualLocalTimeZoneByDeclaration,
    getTimeZones: getTimeZones
};
export default utils;
