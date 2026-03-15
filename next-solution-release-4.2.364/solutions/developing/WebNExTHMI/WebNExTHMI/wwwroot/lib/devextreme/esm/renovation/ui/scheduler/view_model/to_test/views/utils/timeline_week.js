/**
 * DevExtreme (esm/renovation/ui/scheduler/view_model/to_test/views/utils/timeline_week.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import timeZoneUtils from "../../../../../../../ui/scheduler/utils.timeZone";
import {
    getStartViewDateWithoutDST
} from "./base";
export var getDateForHeaderText = (index, date, options) => {
    if (!timeZoneUtils.isTimezoneChangeInDate(date)) {
        return date
    }
    var {
        cellCountInDay: cellCountInDay,
        interval: interval,
        startDayHour: startDayHour,
        startViewDate: startViewDate
    } = options;
    var result = getStartViewDateWithoutDST(startViewDate, startDayHour);
    var validIndex = index % cellCountInDay;
    result.setTime(result.getTime() + validIndex * interval);
    return result
};
