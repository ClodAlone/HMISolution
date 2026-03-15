/**
 * DevExtreme (cjs/ui/scheduler/appointments/textUtils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getFormatType = exports.formatDates = exports.createFormattedDateText = void 0;
var _date = _interopRequireDefault(require("../../../core/utils/date"));
var _date2 = _interopRequireDefault(require("../../../localization/date"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var createFormattedDateText = function(options) {
    var startDate = options.startDate,
        endDate = options.endDate,
        allDay = options.allDay,
        format = options.format;
    var formatType = format || getFormatType(startDate, endDate, allDay);
    return formatDates(startDate, endDate, formatType)
};
exports.createFormattedDateText = createFormattedDateText;
var getFormatType = function(startDate, endDate, isAllDay, isDateAndTimeView) {
    if (isAllDay) {
        return "DATE"
    }
    if (isDateAndTimeView && _date.default.sameDate(startDate, endDate)) {
        return "TIME"
    }
    return "DATETIME"
};
exports.getFormatType = getFormatType;
var formatDates = function(startDate, endDate, formatType) {
    var isSameDate = startDate.getDate() === endDate.getDate();
    switch (formatType) {
        case "DATETIME":
            return [_date2.default.format(startDate, "monthandday"), " ", _date2.default.format(startDate, "shorttime"), " - ", isSameDate ? "" : _date2.default.format(endDate, "monthandday") + " ", _date2.default.format(endDate, "shorttime")].join("");
        case "TIME":
            return "".concat(_date2.default.format(startDate, "shorttime"), " - ").concat(_date2.default.format(endDate, "shorttime"));
        case "DATE":
            return "".concat(_date2.default.format(startDate, "monthandday")).concat(isSameDate ? "" : " - " + _date2.default.format(endDate, "monthandday"))
    }
};
exports.formatDates = formatDates;
