/**
 * DevExtreme (renovation/ui/scheduler/timeZoneCalculator/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.TimeZoneCalculator = void 0;
var _type = require("../../../../core/utils/type");
var _date = _interopRequireDefault(require("../../../../core/utils/date"));
var _types = require("./types");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var TimeZoneCalculator = function() {
    function TimeZoneCalculator(options) {
        this.options = options
    }
    var _proto = TimeZoneCalculator.prototype;
    _proto.createDate = function(sourceDate, info) {
        var date = new Date(sourceDate);
        switch (info.path) {
            case _types.PathTimeZoneConversion.fromSourceToAppointment:
                return this.getConvertedDate(date, info.appointmentTimeZone, true, false);
            case _types.PathTimeZoneConversion.fromAppointmentToSource:
                return this.getConvertedDate(date, info.appointmentTimeZone, true, true);
            case _types.PathTimeZoneConversion.fromSourceToGrid:
                return this.getConvertedDate(date, info.appointmentTimeZone, false, false);
            case _types.PathTimeZoneConversion.fromGridToSource:
                return this.getConvertedDate(date, info.appointmentTimeZone, false, true);
            default:
                throw new Error("not specified pathTimeZoneConversion")
        }
    };
    _proto.getOffsets = function(date, appointmentTimezone) {
        var clientOffset = -this.getClientOffset(date) / _date.default.dateToMilliseconds("hour");
        var commonOffset = this.getCommonOffset(date);
        var appointmentOffset = this.getAppointmentOffset(date, appointmentTimezone);
        return {
            client: clientOffset,
            common: !(0, _type.isDefined)(commonOffset) ? clientOffset : commonOffset,
            appointment: "number" !== typeof appointmentOffset ? clientOffset : appointmentOffset
        }
    };
    _proto.getConvertedDateByOffsets = function(date, clientOffset, targetOffset, isBack) {
        var direction = isBack ? -1 : 1;
        var utcDate = date.getTime() - direction * clientOffset * _date.default.dateToMilliseconds("hour");
        return new Date(utcDate + direction * targetOffset * _date.default.dateToMilliseconds("hour"))
    };
    _proto.getClientOffset = function(date) {
        return this.options.getClientOffset(date)
    };
    _proto.getCommonOffset = function(date) {
        return this.options.getCommonOffset(date)
    };
    _proto.getAppointmentOffset = function(date, appointmentTimezone) {
        return this.options.getAppointmentOffset(date, appointmentTimezone)
    };
    _proto.getConvertedDate = function(date, appointmentTimezone, useAppointmentTimeZone, isBack) {
        var newDate = new Date(date.getTime());
        var offsets = this.getOffsets(newDate, appointmentTimezone);
        if (useAppointmentTimeZone && !!appointmentTimezone) {
            return this.getConvertedDateByOffsets(date, offsets.client, offsets.appointment, isBack)
        }
        return this.getConvertedDateByOffsets(date, offsets.client, offsets.common, isBack)
    };
    return TimeZoneCalculator
}();
exports.TimeZoneCalculator = TimeZoneCalculator;
