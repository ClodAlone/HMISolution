/**
 * DevExtreme (esm/renovation/ui/scheduler/timeZoneCalculator/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined
} from "../../../../core/utils/type";
import dateUtils from "../../../../core/utils/date";
import {
    PathTimeZoneConversion
} from "./types";
export class TimeZoneCalculator {
    constructor(options) {
        this.options = options
    }
    createDate(sourceDate, info) {
        var date = new Date(sourceDate);
        switch (info.path) {
            case PathTimeZoneConversion.fromSourceToAppointment:
                return this.getConvertedDate(date, info.appointmentTimeZone, true, false);
            case PathTimeZoneConversion.fromAppointmentToSource:
                return this.getConvertedDate(date, info.appointmentTimeZone, true, true);
            case PathTimeZoneConversion.fromSourceToGrid:
                return this.getConvertedDate(date, info.appointmentTimeZone, false, false);
            case PathTimeZoneConversion.fromGridToSource:
                return this.getConvertedDate(date, info.appointmentTimeZone, false, true);
            default:
                throw new Error("not specified pathTimeZoneConversion")
        }
    }
    getOffsets(date, appointmentTimezone) {
        var clientOffset = -this.getClientOffset(date) / dateUtils.dateToMilliseconds("hour");
        var commonOffset = this.getCommonOffset(date);
        var appointmentOffset = this.getAppointmentOffset(date, appointmentTimezone);
        return {
            client: clientOffset,
            common: !isDefined(commonOffset) ? clientOffset : commonOffset,
            appointment: "number" !== typeof appointmentOffset ? clientOffset : appointmentOffset
        }
    }
    getConvertedDateByOffsets(date, clientOffset, targetOffset, isBack) {
        var direction = isBack ? -1 : 1;
        var utcDate = date.getTime() - direction * clientOffset * dateUtils.dateToMilliseconds("hour");
        return new Date(utcDate + direction * targetOffset * dateUtils.dateToMilliseconds("hour"))
    }
    getClientOffset(date) {
        return this.options.getClientOffset(date)
    }
    getCommonOffset(date) {
        return this.options.getCommonOffset(date)
    }
    getAppointmentOffset(date, appointmentTimezone) {
        return this.options.getAppointmentOffset(date, appointmentTimezone)
    }
    getConvertedDate(date, appointmentTimezone, useAppointmentTimeZone, isBack) {
        var newDate = new Date(date.getTime());
        var offsets = this.getOffsets(newDate, appointmentTimezone);
        if (useAppointmentTimeZone && !!appointmentTimezone) {
            return this.getConvertedDateByOffsets(date, offsets.client, offsets.appointment, isBack)
        }
        return this.getConvertedDateByOffsets(date, offsets.client, offsets.common, isBack)
    }
}
