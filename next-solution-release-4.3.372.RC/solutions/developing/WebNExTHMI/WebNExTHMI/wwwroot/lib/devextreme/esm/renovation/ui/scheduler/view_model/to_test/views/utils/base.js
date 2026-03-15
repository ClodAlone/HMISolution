/**
 * DevExtreme (esm/renovation/ui/scheduler/view_model/to_test/views/utils/base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import errors from "../../../../../../../ui/widget/ui.errors";
import dateUtils from "../../../../../../../core/utils/date";
import {
    isDefined
} from "../../../../../../../core/utils/type";
import dateLocalization from "../../../../../../../localization/date";
import timeZoneUtils from "../../../../../../../ui/scheduler/utils.timeZone";
import {
    VERTICAL_GROUP_COUNT_CLASSES
} from "../../../../../../../ui/scheduler/classes";
import {
    VIEWS
} from "../../../../../../../ui/scheduler/constants";
import {
    getGroupCount
} from "../../../../../../../ui/scheduler/resources/utils";
import {
    isVerticalGroupingApplied
} from "../../../../workspaces/utils";
export var isDateInRange = (date, startDate, endDate, diff) => diff > 0 ? dateUtils.dateInRange(date, startDate, new Date(endDate.getTime() - 1)) : dateUtils.dateInRange(date, endDate, startDate, "date");
export var setOptionHour = (date, optionHour) => {
    var nextDate = new Date(date);
    if (!isDefined(optionHour)) {
        return nextDate
    }
    nextDate.setHours(optionHour, optionHour % 1 * 60, 0, 0);
    return nextDate
};
export var getViewStartByOptions = (startDate, currentDate, intervalDuration, startViewDate) => {
    if (!startDate) {
        return new Date(currentDate)
    }
    var currentStartDate = dateUtils.trimTime(startViewDate);
    var diff = currentStartDate.getTime() <= currentDate.getTime() ? 1 : -1;
    var endDate = new Date(currentStartDate.getTime() + intervalDuration * diff);
    while (!isDateInRange(currentDate, currentStartDate, endDate, diff)) {
        currentStartDate = endDate;
        endDate = new Date(currentStartDate.getTime() + intervalDuration * diff)
    }
    return diff > 0 ? currentStartDate : endDate
};
export var getCalculatedFirstDayOfWeek = firstDayOfWeekOption => isDefined(firstDayOfWeekOption) ? firstDayOfWeekOption : dateLocalization.firstDayOfWeekIndex();
export var calculateViewStartDate = startDateOption => startDateOption;
export var calculateCellIndex = (rowIndex, columnIndex, rowCount) => columnIndex * rowCount + rowIndex;
export var getStartViewDateWithoutDST = (startViewDate, startDayHour) => {
    var newStartViewDate = timeZoneUtils.getDateWithoutTimezoneChange(startViewDate);
    newStartViewDate.setHours(startDayHour);
    return newStartViewDate
};
export var getHeaderCellText = (headerIndex, date, headerCellTextFormat, getDateForHeaderText, additionalOptions) => {
    var validDate = getDateForHeaderText(headerIndex, date, additionalOptions);
    return dateLocalization.format(validDate, headerCellTextFormat)
};
export var validateDayHours = (startDayHour, endDayHour) => {
    if (startDayHour >= endDayHour) {
        throw errors.Error("E1058")
    }
};
export var getStartViewDateTimeOffset = (startViewDate, startDayHour) => {
    var validStartDayHour = Math.floor(startDayHour);
    var isDSTChange = timeZoneUtils.isTimezoneChangeInDate(startViewDate);
    if (isDSTChange && validStartDayHour !== startViewDate.getHours()) {
        return dateUtils.dateToMilliseconds("hour")
    }
    return 0
};
export var formatWeekday = date => dateLocalization.getDayNames("abbreviated")[date.getDay()];
export var formatWeekdayAndDay = date => "".concat(formatWeekday(date), " ").concat(dateLocalization.format(date, "day"));
export var getToday = (indicatorTime, timeZoneCalculator) => {
    var todayDate = null !== indicatorTime && void 0 !== indicatorTime ? indicatorTime : new Date;
    return (null === timeZoneCalculator || void 0 === timeZoneCalculator ? void 0 : timeZoneCalculator.createDate(todayDate, {
        path: "toGrid"
    })) || todayDate
};
export var getVerticalGroupCountClass = groups => {
    switch (null === groups || void 0 === groups ? void 0 : groups.length) {
        case 1:
            return VERTICAL_GROUP_COUNT_CLASSES[0];
        case 2:
            return VERTICAL_GROUP_COUNT_CLASSES[1];
        case 3:
            return VERTICAL_GROUP_COUNT_CLASSES[2];
        default:
            return
    }
};
export var isDateAndTimeView = viewType => viewType !== VIEWS.TIMELINE_MONTH && viewType !== VIEWS.MONTH;
export var isSupportMultiDayAppointments = viewType => [VIEWS.TIMELINE_DAY, VIEWS.TIMELINE_WEEK, VIEWS.TIMELINE_WORK_WEEK, VIEWS.TIMELINE_MONTH].includes(viewType);
export var getHorizontalGroupCount = (groups, groupOrientation) => {
    var groupCount = getGroupCount(groups) || 1;
    var isVerticalGrouping = isVerticalGroupingApplied(groups, groupOrientation);
    return isVerticalGrouping ? 1 : groupCount
};
export var calculateIsGroupedAllDayPanel = (groups, groupOrientation, isAllDayPanelVisible) => isVerticalGroupingApplied(groups, groupOrientation) && isAllDayPanelVisible;
export var calculateDayDuration = (startDayHour, endDayHour) => endDayHour - startDayHour;
export var isHorizontalView = viewType => {
    switch (viewType) {
        case VIEWS.TIMELINE_DAY:
        case VIEWS.TIMELINE_WEEK:
        case VIEWS.TIMELINE_WORK_WEEK:
        case VIEWS.TIMELINE_MONTH:
        case VIEWS.MONTH:
            return true;
        default:
            return false
    }
};
export var getTotalCellCountByCompleteData = completeData => completeData[completeData.length - 1].length;
export var getTotalRowCountByCompleteData = completeData => completeData.length;
export var getDisplayedCellCount = (displayedCellCount, completeData) => null !== displayedCellCount && void 0 !== displayedCellCount ? displayedCellCount : getTotalCellCountByCompleteData(completeData);
export var getDisplayedRowCount = (displayedRowCount, completeData) => null !== displayedRowCount && void 0 !== displayedRowCount ? displayedRowCount : getTotalRowCountByCompleteData(completeData);
export var getCellDuration = (viewType, startDayHour, endDayHour, hoursInterval) => {
    switch (viewType) {
        case "month":
            return 36e5 * calculateDayDuration(startDayHour, endDayHour);
        case "timelineMonth":
            return dateUtils.dateToMilliseconds("day");
        default:
            return 36e5 * hoursInterval
    }
};
