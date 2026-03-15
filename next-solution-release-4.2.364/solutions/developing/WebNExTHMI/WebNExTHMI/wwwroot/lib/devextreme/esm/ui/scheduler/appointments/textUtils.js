/**
 * DevExtreme (esm/ui/scheduler/appointments/textUtils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import dateUtils from "../../../core/utils/date";
import dateLocalization from "../../../localization/date";
export var createFormattedDateText = options => {
    var {
        startDate: startDate,
        endDate: endDate,
        allDay: allDay,
        format: format
    } = options;
    var formatType = format || getFormatType(startDate, endDate, allDay);
    return formatDates(startDate, endDate, formatType)
};
export var getFormatType = (startDate, endDate, isAllDay, isDateAndTimeView) => {
    if (isAllDay) {
        return "DATE"
    }
    if (isDateAndTimeView && dateUtils.sameDate(startDate, endDate)) {
        return "TIME"
    }
    return "DATETIME"
};
export var formatDates = (startDate, endDate, formatType) => {
    var isSameDate = startDate.getDate() === endDate.getDate();
    switch (formatType) {
        case "DATETIME":
            return [dateLocalization.format(startDate, "monthandday"), " ", dateLocalization.format(startDate, "shorttime"), " - ", isSameDate ? "" : dateLocalization.format(endDate, "monthandday") + " ", dateLocalization.format(endDate, "shorttime")].join("");
        case "TIME":
            return "".concat(dateLocalization.format(startDate, "shorttime"), " - ").concat(dateLocalization.format(endDate, "shorttime"));
        case "DATE":
            return "".concat(dateLocalization.format(startDate, "monthandday")).concat(isSameDate ? "" : " - " + dateLocalization.format(endDate, "monthandday"))
    }
};
