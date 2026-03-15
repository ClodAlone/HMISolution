/**
 * DevExtreme (esm/renovation/ui/scheduler/view_model/to_test/views/utils/month.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import dateUtils from "../../../../../../../core/utils/date";
import dateLocalization from "../../../../../../../localization/date";
import {
    getCalculatedFirstDayOfWeek,
    isDateInRange,
    setOptionHour
} from "./base";
export var getViewStartByOptions = (startDate, currentDate, intervalCount, startViewDate) => {
    if (!startDate) {
        return new Date(currentDate)
    }
    var currentStartDate = new Date(startViewDate);
    var validStartViewDate = new Date(startViewDate);
    var diff = currentStartDate.getTime() <= currentDate.getTime() ? 1 : -1;
    var endDate = new Date(new Date(validStartViewDate.setMonth(validStartViewDate.getMonth() + diff * intervalCount)));
    while (!isDateInRange(currentDate, currentStartDate, endDate, diff)) {
        currentStartDate = new Date(endDate);
        if (diff > 0) {
            currentStartDate.setDate(1)
        }
        endDate = new Date(new Date(endDate.setMonth(endDate.getMonth() + diff * intervalCount)))
    }
    return diff > 0 ? currentStartDate : endDate
};
export var calculateStartViewDate = (currentDate, startDayHour, startDate, intervalCount, firstDayOfWeekOption) => {
    var viewStart = getViewStartByOptions(startDate, currentDate, intervalCount, dateUtils.getFirstMonthDate(startDate));
    var firstMonthDate = dateUtils.getFirstMonthDate(viewStart);
    var firstDayOfWeek = getCalculatedFirstDayOfWeek(firstDayOfWeekOption);
    var firstViewDate = dateUtils.getFirstWeekDate(firstMonthDate, firstDayOfWeek);
    return setOptionHour(firstViewDate, startDayHour)
};
export var calculateCellIndex = (rowIndex, columnIndex, _, columnCount) => rowIndex * columnCount + columnIndex;
export var isFirstCellInMonthWithIntervalCount = (cellDate, intervalCount) => 1 === cellDate.getDate() && intervalCount > 1;
export var getCellText = (date, intervalCount) => {
    if (isFirstCellInMonthWithIntervalCount(date, intervalCount)) {
        var monthName = dateLocalization.getMonthNames("abbreviated")[date.getMonth()];
        return [monthName, dateLocalization.format(date, "day")].join(" ")
    }
    return dateLocalization.format(date, "dd")
};
