/**
 * DevExtreme (esm/ui/scheduler/header/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import dateUtils from "../../../core/utils/date";
import dateLocalization from "../../../localization/date";
import messageLocalization from "../../../localization/message";
import {
    camelize
} from "../../../core/utils/inflector";
import {
    isFunction,
    isObject
} from "../../../core/utils/type";
import errors from "../../../core/errors";
import {
    VIEWS
} from "../constants";
var DAY_FORMAT = "d";
var DAYS_IN_WORK_WEEK = 5;
var {
    correctDateWithUnitBeginning: getPeriodStart,
    getFirstWeekDate: getWeekStart,
    getLastMonthDay: getLastMonthDay,
    addDateInterval: addDateInterval
} = dateUtils;
var {
    format: formatDate
} = dateLocalization;
var MS_DURATION = {
    milliseconds: 1
};
var DAY_DURATION = {
    days: 1
};
var WEEK_DURATION = {
    days: 7
};
var SATURDAY_INDEX = 6;
var SUNDAY_INDEX = 0;
var subMS = date => addDateInterval(date, MS_DURATION, -1);
var addMS = date => addDateInterval(date, MS_DURATION, 1);
var nextDay = date => addDateInterval(date, DAY_DURATION, 1);
export var nextWeek = date => addDateInterval(date, WEEK_DURATION, 1);
var nextMonth = date => {
    var days = getLastMonthDay(date);
    return addDateInterval(date, {
        days: days
    }, 1)
};
var isWeekend = date => date.getDay() === SATURDAY_INDEX || date.getDay() === SUNDAY_INDEX;
var getWorkWeekStart = firstDayOfWeek => {
    var date = new Date(firstDayOfWeek);
    while (isWeekend(date)) {
        date = nextDay(date)
    }
    return date
};
var getDateAfterWorkWeek = workWeekStart => {
    var date = new Date(workWeekStart);
    var workDaysCount = 0;
    while (workDaysCount < DAYS_IN_WORK_WEEK) {
        if (!isWeekend(date)) {
            workDaysCount++
        }
        date = nextDay(date)
    }
    return date
};
var nextAgendaStart = (date, agendaDuration) => addDateInterval(date, {
    days: agendaDuration
}, 1);
var getInterval = options => {
    var startDate = getIntervalStartDate(options);
    var endDate = getIntervalEndDate(startDate, options);
    return {
        startDate: startDate,
        endDate: endDate
    }
};
var getIntervalStartDate = options => {
    var {
        date: date,
        step: step,
        firstDayOfWeek: firstDayOfWeek
    } = options;
    switch (step) {
        case "day":
        case "week":
        case "month":
            return getPeriodStart(date, step, false, firstDayOfWeek);
        case "workWeek":
            var firstWeekDay = getWeekStart(date, firstDayOfWeek);
            return getWorkWeekStart(firstWeekDay);
        case "agenda":
            return new Date(date)
    }
};
var getIntervalEndDate = (startDate, options) => {
    var {
        intervalCount: intervalCount,
        step: step,
        agendaDuration: agendaDuration
    } = options;
    var periodStartDate;
    var periodEndDate;
    var nextPeriodStartDate = new Date(startDate);
    for (var i = 0; i < intervalCount; i++) {
        periodStartDate = nextPeriodStartDate;
        periodEndDate = getPeriodEndDate(periodStartDate, step, agendaDuration);
        nextPeriodStartDate = getNextPeriodStartDate(periodEndDate, step)
    }
    return periodEndDate
};
var getPeriodEndDate = (currentPeriodStartDate, step, agendaDuration) => {
    var date;
    switch (step) {
        case "day":
            date = nextDay(currentPeriodStartDate);
            break;
        case "week":
            date = nextWeek(currentPeriodStartDate);
            break;
        case "month":
            date = nextMonth(currentPeriodStartDate);
            break;
        case "workWeek":
            date = getDateAfterWorkWeek(currentPeriodStartDate);
            break;
        case "agenda":
            date = nextAgendaStart(currentPeriodStartDate, agendaDuration)
    }
    return subMS(date)
};
var getNextPeriodStartDate = (currentPeriodEndDate, step) => {
    var date = addMS(currentPeriodEndDate);
    if ("workWeek" === step) {
        while (isWeekend(date)) {
            date = nextDay(date)
        }
    }
    return date
};
export var getNextIntervalDate = (options, direction) => {
    var {
        date: date,
        step: step,
        intervalCount: intervalCount,
        agendaDuration: agendaDuration
    } = options;
    var dayDuration;
    switch (step) {
        case "day":
            dayDuration = 1 * intervalCount;
            break;
        case "week":
        case "workWeek":
            dayDuration = 7 * intervalCount;
            break;
        case "agenda":
            dayDuration = agendaDuration;
            break;
        case "month":
            return getNextMonthDate(date, intervalCount, direction)
    }
    return addDateInterval(date, {
        days: dayDuration
    }, direction)
};
var getNextMonthDate = (date, intervalCount, direction) => {
    var currentDate = date.getDate();
    var currentMonthFirstDate = new Date(new Date(date.getTime()).setDate(1));
    var thatMonthFirstDate = new Date(currentMonthFirstDate.setMonth(currentMonthFirstDate.getMonth() + intervalCount * direction));
    var thatMonthDuration = getLastMonthDay(thatMonthFirstDate);
    var minDate = currentDate < thatMonthDuration ? currentDate : thatMonthDuration;
    var currentMonthMinDate = new Date(new Date(date.getTime()).setDate(minDate));
    var thatMonthMinDate = new Date(currentMonthMinDate.setMonth(currentMonthMinDate.getMonth() + intervalCount * direction));
    return thatMonthMinDate
};
var getDateMonthFormatter = isShort => {
    var monthType = isShort ? "abbreviated" : "wide";
    var months = dateLocalization.getMonthNames(monthType);
    return date => {
        var day = formatDate(date, "day");
        var month = months[date.getMonth()];
        return "".concat(day, " ").concat(month)
    }
};
var formatMonthYear = date => {
    var months = dateLocalization.getMonthNames("abbreviated");
    var month = months[date.getMonth()];
    var year = formatDate(date, "year");
    return "".concat(month, " ").concat(year)
};
var getDateMonthYearFormatter = isShort => date => {
    var dateMonthFormat = getDateMonthFormatter(isShort);
    var dateMonth = dateMonthFormat(date);
    var year = formatDate(date, "year");
    return "".concat(dateMonth, " ").concat(year)
};
var getDifferentYearCaption = (startDate, endDate) => {
    var firstDateText = formatDate(startDate, getDateMonthYearFormatter(true));
    var lastDateDateText = formatDate(endDate, getDateMonthYearFormatter(true));
    return "".concat(firstDateText, "-").concat(lastDateDateText)
};
var getSameYearCaption = (startDate, endDate, isShort) => {
    var isDifferentMonthDates = startDate.getMonth() !== endDate.getMonth();
    var useShortFormat = isDifferentMonthDates || isShort;
    var firstDateFormat = isDifferentMonthDates ? getDateMonthFormatter(useShortFormat) : DAY_FORMAT;
    var firstDateText = formatDate(startDate, firstDateFormat);
    var lastDateText = formatDate(endDate, getDateMonthYearFormatter(useShortFormat));
    return "".concat(firstDateText, "-").concat(lastDateText)
};
var getSameDateCaption = (date, step, isShort) => {
    var useShortFormat = "agenda" === step ? isShort : false;
    var dateMonthFormat = getDateMonthFormatter(useShortFormat);
    var dateMonth = dateMonthFormat(date);
    var year = formatDate(date, "year");
    return "".concat(dateMonth, " ").concat(year)
};
var formatCaptionByMonths = (startDate, endDate, isShort) => {
    var isDifferentYears = startDate.getFullYear() !== endDate.getFullYear();
    if (isDifferentYears) {
        return getDifferentYearCaption(startDate, endDate)
    }
    return getSameYearCaption(startDate, endDate, isShort)
};
var formatMonthViewCaption = (startDate, endDate) => {
    if (dateUtils.sameMonth(startDate, endDate)) {
        return formatDate(startDate, "monthandyear")
    }
    var isSameYear = dateUtils.sameYear(startDate, endDate);
    var firstDateText = isSameYear ? dateLocalization.getMonthNames("abbreviated")[startDate.getMonth()] : formatMonthYear(startDate);
    var lastDateText = formatMonthYear(endDate);
    return "".concat(firstDateText, "-").concat(lastDateText)
};
var getCaptionText = (startDate, endDate, isShort, step) => {
    if (dateUtils.sameDate(startDate, endDate)) {
        return getSameDateCaption(startDate, step, isShort)
    }
    if ("month" === step) {
        return formatMonthViewCaption(startDate, endDate)
    }
    return formatCaptionByMonths(startDate, endDate, isShort)
};
export var getCaption = (options, isShort, customizationFunction) => {
    var {
        startDate: startDate,
        endDate: endDate
    } = getInterval(options);
    var text = getCaptionText(startDate, endDate, isShort, options.step);
    if (isFunction(customizationFunction)) {
        text = customizationFunction({
            startDate: startDate,
            endDate: endDate,
            text: text
        })
    }
    return {
        startDate: startDate,
        endDate: endDate,
        text: text
    }
};
var STEP_MAP = {
    day: "day",
    week: "week",
    workWeek: "workWeek",
    month: "month",
    timelineDay: "day",
    timelineWeek: "week",
    timelineWorkWeek: "workWeek",
    timelineMonth: "month",
    agenda: "agenda"
};
export var getStep = view => STEP_MAP[getViewType(view)];
export var getViewType = view => {
    if (isObject(view) && view.type) {
        return view.type
    }
    return view
};
export var getViewName = view => {
    if (isObject(view)) {
        return view.name ? view.name : view.type
    }
    return view
};
export var getViewText = view => {
    if (view.name) {
        return view.name
    }
    var viewName = camelize(view.type || view, true);
    return messageLocalization.format("dxScheduler-switcher" + viewName)
};
var isValidView = view => Object.values(VIEWS).includes(view);
export var validateViews = views => {
    views.forEach(view => {
        var viewType = getViewType(view);
        if (!isValidView(viewType)) {
            errors.log("W0008", viewType)
        }
    })
};
export var formatViews = views => {
    validateViews(views);
    return views.map(view => {
        var text = getViewText(view);
        var type = getViewType(view);
        var name = getViewName(view);
        return {
            text: text,
            name: name,
            view: {
                text: text,
                type: type,
                name: name
            }
        }
    })
};
export var isOneView = (views, selectedView) => 1 === views.length && views[0].name === selectedView;
