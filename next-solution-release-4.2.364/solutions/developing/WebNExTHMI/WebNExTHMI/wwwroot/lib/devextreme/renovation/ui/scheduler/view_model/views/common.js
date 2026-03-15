/**
 * DevExtreme (renovation/ui/scheduler/view_model/views/common.js)
 * Version: 21.2.3
 * Build date: Thu Oct 28 2021
 *
 * Copyright (c) 2012 - 2021 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getStartViewDate = exports.getFirstDayOfWeek = exports.getCurrentDate = void 0;
var _date = _interopRequireDefault(require("../../../../../core/utils/date"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var getCurrentDate = function(currentDate, startDate) {
    var validCurrentDate = null !== startDate && void 0 !== startDate ? startDate : currentDate;
    return _date.default.trimTime(validCurrentDate)
};
exports.getCurrentDate = getCurrentDate;
var getFirstDayOfWeek = function(includedDays, firstDayOfWeek) {
    var isFirstDayOfWeekInIncludedDays = includedDays.includes(firstDayOfWeek);
    var sortedIncludedDays = includedDays.slice().sort((function(a, b) {
        return a - b
    }));
    return isFirstDayOfWeekInIncludedDays ? firstDayOfWeek : sortedIncludedDays[0]
};
exports.getFirstDayOfWeek = getFirstDayOfWeek;
var getStartViewDate = function(_startDayHour, _firstDayOfWeek, currentDate) {
    return currentDate
};
exports.getStartViewDate = getStartViewDate;
