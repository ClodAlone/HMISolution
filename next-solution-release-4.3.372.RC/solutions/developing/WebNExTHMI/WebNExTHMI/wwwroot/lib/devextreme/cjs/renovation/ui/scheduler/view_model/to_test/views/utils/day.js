/**
 * DevExtreme (cjs/renovation/ui/scheduler/view_model/to_test/views/utils/day.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.calculateStartViewDate = void 0;
var _base = require("./base");
var calculateStartViewDate = function(currentDate, startDayHour, startDate, intervalDuration) {
    var firstViewDate = (0, _base.getViewStartByOptions)(startDate, currentDate, intervalDuration, startDate);
    return (0, _base.setOptionHour)(firstViewDate, startDayHour)
};
exports.calculateStartViewDate = calculateStartViewDate;
