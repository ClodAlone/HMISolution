/**
 * DevExtreme (cjs/ui/scheduler/header/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.validateViews = exports.nextWeek = exports.isOneView = exports.getViewType = exports.getViewText = exports.getViewName = exports.getStep = exports.getNextIntervalDate = exports.getCaption = exports.formatViews = void 0;
var _date = _interopRequireDefault(require("../../../core/utils/date"));
var _date2 = _interopRequireDefault(require("../../../localization/date"));
var _message = _interopRequireDefault(require("../../../localization/message"));
var _inflector = require("../../../core/utils/inflector");
var _type = require("../../../core/utils/type");
var _errors = _interopRequireDefault(require("../../../core/errors"));
var _constants = require("../constants");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var DAY_FORMAT = "d";
var DAYS_IN_WORK_WEEK = 5;
var getPeriodStart = _date.default.correctDateWithUnitBeginning,
    getWeekStart = _date.default.getFirstWeekDate,
    getLastMonthDay = _date.default.getLastMonthDay,
    addDateInterval = _date.default.addDateInterval;
var formatDate = _date2.default.format;
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
var subMS = function(date) {
    return addDateInterval(date, MS_DURATION, -1)
};
var addMS = function(date) {
    return addDateInterval(date, MS_DURATION, 1)
};
var nextDay = function(date) {
    return addDateInterval(date, DAY_DURATION, 1)
};
var nextWeek = function(date) {
    return addDateInterval(date, WEEK_DURATION, 1)
};
exports.nextWeek = nextWeek;
var nextMonth = function(date) {
    var days = getLastMonthDay(date);
    return addDateInterval(date, {
        days: days
    }, 1)
};
var isWeekend = function(date) {
    return date.getDay() === SATURDAY_INDEX || date.getDay() === SUNDAY_INDEX
};
var getWorkWeekStart = function(firstDayOfWeek) {
    var date = new Date(firstDayOfWeek);
    while (isWeekend(date)) {
        date = nextDay(date)
    }
    return date
};
var getDateAfterWorkWeek = function(workWeekStart) {
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
var nextAgendaStart = function(date, agendaDuration) {
    return addDateInterval(date, {
        days: agendaDuration
    }, 1)
};
var getInterval = function(options) {
    var startDate = getIntervalStartDate(options);
    var endDate = getIntervalEndDate(startDate, options);
    return {
        startDate: startDate,
        endDate: endDate
    }
};
var getIntervalStartDate = function(options) {
    var date = options.date,
        step = options.step,
        firstDayOfWeek = options.firstDayOfWeek;
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
var getIntervalEndDate = function(startDate, options) {
    var intervalCount = options.intervalCount,
        step = options.step,
        agendaDuration = options.agendaDuration;
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
var getPeriodEndDate = function(currentPeriodStartDate, step, agendaDuration) {
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
var getNextPeriodStartDate = function(currentPeriodEndDate, step) {
    var date = addMS(currentPeriodEndDate);
    if ("workWeek" === step) {
        while (isWeekend(date)) {
            date = nextDay(date)
        }
    }
    return date
};
var getNextIntervalDate = function(options, direction) {
    var date = options.date,
        step = options.step,
        intervalCount = options.intervalCount,
        agendaDuration = options.agendaDuration;
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
exports.getNextIntervalDate = getNextIntervalDate;
var getNextMonthDate = function(date, intervalCount, direction) {
    var currentDate = date.getDate();
    var currentMonthFirstDate = new Date(new Date(date.getTime()).setDate(1));
    var thatMonthFirstDate = new Date(currentMonthFirstDate.setMonth(currentMonthFirstDate.getMonth() + intervalCount * direction));
    var thatMonthDuration = getLastMonthDay(thatMonthFirstDate);
    var minDate = currentDate < thatMonthDuration ? currentDate : thatMonthDuration;
    var currentMonthMinDate = new Date(new Date(date.getTime()).setDate(minDate));
    var thatMonthMinDate = new Date(currentMonthMinDate.setMonth(currentMonthMinDate.getMonth() + intervalCount * direction));
    return thatMonthMinDate
};
var getDateMonthFormatter = function(isShort) {
    var monthType = isShort ? "abbreviated" : "wide";
    var months = _date2.default.getMonthNames(monthType);
    return function(date) {
        var day = formatDate(date, "day");
        var month = months[date.getMonth()];
        return "".concat(day, " ").concat(month)
    }
};
var formatMonthYear = function(date) {
    var months = _date2.default.getMonthNames("abbreviated");
    var month = months[date.getMonth()];
    var year = formatDate(date, "year");
    return "".concat(month, " ").concat(year)
};
var getDateMonthYearFormatter = function(isShort) {
    return function(date) {
        var dateMonthFormat = getDateMonthFormatter(isShort);
        var dateMonth = dateMonthFormat(date);
        var year = formatDate(date, "year");
        return "".concat(dateMonth, " ").concat(year)
    }
};
var getDifferentYearCaption = function(startDate, endDate) {
    var firstDateText = formatDate(startDate, getDateMonthYearFormatter(true));
    var lastDateDateText = formatDate(endDate, getDateMonthYearFormatter(true));
    return "".concat(firstDateText, "-").concat(lastDateDateText)
};
var getSameYearCaption = function(startDate, endDate, isShort) {
    var isDifferentMonthDates = startDate.getMonth() !== endDate.getMonth();
    var useShortFormat = isDifferentMonthDates || isShort;
    var firstDateFormat = isDifferentMonthDates ? getDateMonthFormatter(useShortFormat) : DAY_FORMAT;
    var firstDateText = formatDate(startDate, firstDateFormat);
    var lastDateText = formatDate(endDate, getDateMonthYearFormatter(useShortFormat));
    return "".concat(firstDateText, "-").concat(lastDateText)
};
var getSameDateCaption = function(date, step, isShort) {
    var useShortFormat = "agenda" === step ? isShort : false;
    var dateMonthFormat = getDateMonthFormatter(useShortFormat);
    var dateMonth = dateMonthFormat(date);
    var year = formatDate(date, "year");
    return "".concat(dateMonth, " ").concat(year)
};
var formatCaptionByMonths = function(startDate, endDate, isShort) {
    var isDifferentYears = startDate.getFullYear() !== endDate.getFullYear();
    if (isDifferentYears) {
        return getDifferentYearCaption(startDate, endDate)
    }
    return getSameYearCaption(startDate, endDate, isShort)
};
var formatMonthViewCaption = function(startDate, endDate) {
    if (_date.default.sameMonth(startDate, endDate)) {
        return formatDate(startDate, "monthandyear")
    }
    var isSameYear = _date.default.sameYear(startDate, endDate);
    var firstDateText = isSameYear ? _date2.default.getMonthNames("abbreviated")[startDate.getMonth()] : formatMonthYear(startDate);
    var lastDateText = formatMonthYear(endDate);
    return "".concat(firstDateText, "-").concat(lastDateText)
};
var getCaptionText = function(startDate, endDate, isShort, step) {
    if (_date.default.sameDate(startDate, endDate)) {
        return getSameDateCaption(startDate, step, isShort)
    }
    if ("month" === step) {
        return formatMonthViewCaption(startDate, endDate)
    }
    return formatCaptionByMonths(startDate, endDate, isShort)
};
var getCaption = function(options, isShort, customizationFunction) {
    var _getInterval = getInterval(options),
        startDate = _getInterval.startDate,
        endDate = _getInterval.endDate;
    var text = getCaptionText(startDate, endDate, isShort, options.step);
    if ((0, _type.isFunction)(customizationFunction)) {
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
exports.getCaption = getCaption;
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
var getStep = function(view) {
    return STEP_MAP[getViewType(view)]
};
exports.getStep = getStep;
var getViewType = function(view) {
    if ((0, _type.isObject)(view) && view.type) {
        return view.type
    }
    return view
};
exports.getViewType = getViewType;
var getViewName = function(view) {
    if ((0, _type.isObject)(view)) {
        return view.name ? view.name : view.type
    }
    return view
};
exports.getViewName = getViewName;
var getViewText = function(view) {
    if (view.name) {
        return view.name
    }
    var viewName = (0, _inflector.camelize)(view.type || view, true);
    return _message.default.format("dxScheduler-switcher" + viewName)
};
exports.getViewText = getViewText;
var isValidView = function(view) {
    return Object.values(_constants.VIEWS).includes(view)
};
var validateViews = function(views) {
    views.forEach((function(view) {
        var viewType = getViewType(view);
        if (!isValidView(viewType)) {
            _errors.default.log("W0008", viewType)
        }
    }))
};
exports.validateViews = validateViews;
var formatViews = function(views) {
    validateViews(views);
    return views.map((function(view) {
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
    }))
};
exports.formatViews = formatViews;
var isOneView = function(views, selectedView) {
    return 1 === views.length && views[0].name === selectedView
};
exports.isOneView = isOneView;
