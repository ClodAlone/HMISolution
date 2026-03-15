/**
 * DevExtreme (renovation/ui/scheduler/view_model/to_test/views/utils/week.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getValidStartDate = exports.getTimePanelCellText = exports.getIntervalDuration = exports.calculateViewStartDate = exports.calculateStartViewDate = void 0;
var _date = _interopRequireDefault(require("../../../../../../../core/utils/date"));
var _date2 = _interopRequireDefault(require("../../../../../../../localization/date"));
var _base = require("./base");
var _utils = _interopRequireDefault(require("../../../../../../../ui/scheduler/utils.timeZone"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var getIntervalDuration = function(intervalCount) {
    return 7 * _date.default.dateToMilliseconds("day") * intervalCount
};
exports.getIntervalDuration = getIntervalDuration;
var getValidStartDate = function(startDate, firstDayOfWeek) {
    return startDate ? _date.default.getFirstWeekDate(startDate, firstDayOfWeek) : void 0
};
exports.getValidStartDate = getValidStartDate;
var calculateStartViewDate = function(currentDate, startDayHour, startDate, intervalDuration, firstDayOfWeekOption) {
    var firstDayOfWeek = (0, _base.getCalculatedFirstDayOfWeek)(firstDayOfWeekOption);
    var viewStart = (0, _base.getViewStartByOptions)(startDate, currentDate, intervalDuration, getValidStartDate(startDate, firstDayOfWeek));
    var firstViewDate = _date.default.getFirstWeekDate(viewStart, firstDayOfWeek);
    return (0, _base.setOptionHour)(firstViewDate, startDayHour)
};
exports.calculateStartViewDate = calculateStartViewDate;
var calculateViewStartDate = function(startDateOption, firstDayOfWeek) {
    var validFirstDayOfWeek = null !== firstDayOfWeek && void 0 !== firstDayOfWeek ? firstDayOfWeek : _date2.default.firstDayOfWeekIndex();
    return _date.default.getFirstWeekDate(startDateOption, validFirstDayOfWeek)
};
exports.calculateViewStartDate = calculateViewStartDate;
var getTimeCellDate = function(rowIndex, date, startViewDate, cellDuration, startDayHour) {
    if (!_utils.default.isTimezoneChangeInDate(date)) {
        return date
    }
    var startViewDateWithoutDST = _utils.default.getDateWithoutTimezoneChange(startViewDate);
    var result = new Date(startViewDateWithoutDST);
    var timeCellDuration = Math.round(cellDuration);
    var startViewDateOffset = (0, _base.getStartViewDateTimeOffset)(startViewDate, startDayHour);
    result.setMilliseconds(result.getMilliseconds() + timeCellDuration * rowIndex - startViewDateOffset);
    return result
};
var getTimePanelCellText = function(rowIndex, date, startViewDate, cellDuration, startDayHour) {
    if (rowIndex % 2 === 0) {
        var validDate = getTimeCellDate(rowIndex, date, startViewDate, cellDuration, startDayHour);
        return _date2.default.format(validDate, "shorttime")
    }
    return ""
};
exports.getTimePanelCellText = getTimePanelCellText;
