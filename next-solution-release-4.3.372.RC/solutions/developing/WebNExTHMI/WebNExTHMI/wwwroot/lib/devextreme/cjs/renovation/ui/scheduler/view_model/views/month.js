/**
 * DevExtreme (cjs/renovation/ui/scheduler/view_model/views/month.js)
 * Version: 21.2.3
 * Build date: Thu Oct 28 2021
 *
 * Copyright (c) 2012 - 2021 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getStartViewDate = void 0;
var _date = _interopRequireDefault(require("../../../../../core/utils/date"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var getStartViewDate = function(startDayHour, firstDayOfWeek, currentDate) {
    var firstMonthDate = _date.default.getFirstMonthDate(currentDate);
    var result = _date.default.getFirstWeekDate(firstMonthDate, firstDayOfWeek);
    result.setHours(startDayHour);
    return result
};
exports.getStartViewDate = getStartViewDate;
