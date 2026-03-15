/**
 * DevExtreme (esm/ui/scheduler/recurrence.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import errors from "../../core/errors";
import {
    each
} from "../../core/utils/iterator";
import {
    inArray
} from "../../core/utils/array";
import {
    RRule,
    RRuleSet
} from "rrule";
import dateUtils from "../../core/utils/date";
import timeZoneUtils from "./utils.timeZone";
var toMs = dateUtils.dateToMilliseconds;
var ruleNames = ["freq", "interval", "byday", "byweekno", "byyearday", "bymonth", "bymonthday", "count", "until", "byhour", "byminute", "bysecond", "bysetpos", "wkst"];
var freqNames = ["DAILY", "WEEKLY", "MONTHLY", "YEARLY", "SECONDLY", "MINUTELY", "HOURLY"];
var days = {
    SU: 0,
    MO: 1,
    TU: 2,
    WE: 3,
    TH: 4,
    FR: 5,
    SA: 6
};
var loggedWarnings = [];
var recurrence = null;
export function getRecurrenceProcessor() {
    if (!recurrence) {
        recurrence = new RecurrenceProcessor
    }
    return recurrence
}
class RecurrenceProcessor {
    constructor() {
        this.rRule = null;
        this.rRuleSet = null;
        this.validator = new RecurrenceValidator
    }
    generateDates(options) {
        var result = [];
        var recurrenceRule = this.evalRecurrenceRule(options.rule);
        var rule = recurrenceRule.rule;
        if (!recurrenceRule.isValid || !rule.freq) {
            return result
        }
        var startDateUtc = timeZoneUtils.createUTCDateWithLocalOffset(options.start);
        var endDateUtc = timeZoneUtils.createUTCDateWithLocalOffset(options.end);
        var minDateUtc = timeZoneUtils.createUTCDateWithLocalOffset(options.min);
        var maxDateUtc = timeZoneUtils.createUTCDateWithLocalOffset(options.max);
        var duration = endDateUtc ? endDateUtc.getTime() - startDateUtc.getTime() : 0;
        this._initializeRRule(options, startDateUtc, rule.until);
        var minTime = minDateUtc.getTime();
        var leftBorder = this._getLeftBorder(options, minDateUtc, duration);
        this.rRuleSet.between(leftBorder, maxDateUtc, true).forEach(date => {
            var endAppointmentTime = date.getTime() + duration;
            if (endAppointmentTime >= minTime) {
                var correctDate = timeZoneUtils.createDateFromUTCWithLocalOffset(date);
                result.push(correctDate)
            }
        });
        return result
    }
    hasRecurrence(options) {
        return !!this.generateDates(options).length
    }
    evalRecurrenceRule(rule) {
        var result = {
            rule: {},
            isValid: false
        };
        if (rule) {
            result.rule = this._parseRecurrenceRule(rule);
            result.isValid = this.validator.validateRRule(result.rule, rule)
        }
        return result
    }
    isValidRecurrenceRule(rule) {
        return this.evalRecurrenceRule(rule).isValid
    }
    daysFromByDayRule(rule) {
        var result = [];
        if (rule.byday) {
            if (Array.isArray(rule.byday)) {
                result = rule.byday
            } else {
                result = rule.byday.split(",")
            }
        }
        return result.map(item => {
            var match = item.match(/[A-Za-z]+/);
            return !!match && match[0]
        }).filter(item => !!item)
    }
    getAsciiStringByDate(date) {
        var currentOffset = date.getTimezoneOffset() * toMs("minute");
        var offsetDate = new Date(date.getTime() + currentOffset);
        return offsetDate.getFullYear() + ("0" + (offsetDate.getMonth() + 1)).slice(-2) + ("0" + offsetDate.getDate()).slice(-2) + "T" + ("0" + offsetDate.getHours()).slice(-2) + ("0" + offsetDate.getMinutes()).slice(-2) + ("0" + offsetDate.getSeconds()).slice(-2) + "Z"
    }
    getRecurrenceString(object) {
        if (!object || !object.freq) {
            return
        }
        var result = "";
        for (var field in object) {
            var value = object[field];
            if ("interval" === field && value < 2) {
                continue
            }
            if ("until" === field) {
                value = this.getAsciiStringByDate(value)
            }
            result += field + "=" + value + ";"
        }
        result = result.substring(0, result.length - 1);
        return result.toUpperCase()
    }
    _parseExceptionToRawArray(value) {
        return value.match(/(\d{4})(\d{2})(\d{2})(T(\d{2})(\d{2})(\d{2}))?(Z)?/)
    }
    getDateByAsciiString(exceptionText) {
        if ("string" !== typeof exceptionText) {
            return exceptionText
        }
        var result = this._parseExceptionToRawArray(exceptionText);
        if (!result) {
            return null
        }
        var [year, month, date, hours, minutes, seconds, isUtc] = this._createDateTuple(result);
        if (isUtc) {
            return new Date(Date.UTC(year, month, date, hours, minutes, seconds))
        }
        return new Date(year, month, date, hours, minutes, seconds)
    }
    _dispose() {
        if (this.rRuleSet) {
            delete this.rRuleSet;
            this.rRuleSet = null
        }
        if (this.rRule) {
            delete this.rRule;
            this.rRule = null
        }
    }
    _getTimeZoneOffset() {
        return (new Date).getTimezoneOffset()
    }
    _initializeRRule(options, startDateUtc, until) {
        var ruleOptions = RRule.parseString(options.rule);
        var firstDayOfWeek = options.firstDayOfWeek;
        ruleOptions.dtstart = startDateUtc;
        if (!ruleOptions.wkst && firstDayOfWeek) {
            ruleOptions.wkst = [6, 0, 1, 2, 3, 4, 5][firstDayOfWeek]
        }
        ruleOptions.until = timeZoneUtils.createUTCDateWithLocalOffset(until);
        this._createRRule(ruleOptions);
        if (options.exception) {
            var exceptionStrings = options.exception;
            var exceptionDates = exceptionStrings.split(",").map(rule => this.getDateByAsciiString(rule));
            exceptionDates.forEach(date => {
                if (options.getPostProcessedException) {
                    date = options.getPostProcessedException(date)
                }
                var utcDate = timeZoneUtils.createUTCDateWithLocalOffset(date);
                this.rRuleSet.exdate(utcDate)
            })
        }
    }
    _createRRule(ruleOptions) {
        this._dispose();
        var rRuleSet = new RRuleSet;
        this.rRuleSet = rRuleSet;
        this.rRule = new RRule(ruleOptions);
        this.rRuleSet.rrule(this.rRule)
    }
    _getLeftBorder(options, minDateUtc, appointmentDuration) {
        if (options.end && !timeZoneUtils.isSameAppointmentDates(options.start, options.end)) {
            return new Date(minDateUtc.getTime() - appointmentDuration)
        }
        return minDateUtc
    }
    _parseRecurrenceRule(recurrence) {
        var ruleObject = {};
        var ruleParts = recurrence.split(";");
        for (var i = 0, len = ruleParts.length; i < len; i++) {
            var rule = ruleParts[i].split("=");
            var ruleName = rule[0].toLowerCase();
            var ruleValue = rule[1];
            ruleObject[ruleName] = ruleValue
        }
        var count = parseInt(ruleObject.count);
        if (!isNaN(count)) {
            ruleObject.count = count
        }
        if (ruleObject.interval) {
            var interval = parseInt(ruleObject.interval);
            if (!isNaN(interval)) {
                ruleObject.interval = interval
            }
        } else {
            ruleObject.interval = 1
        }
        if (ruleObject.freq && ruleObject.until) {
            ruleObject.until = this.getDateByAsciiString(ruleObject.until)
        }
        return ruleObject
    }
    _createDateTuple(parseResult) {
        var isUtc = void 0 !== parseResult[8];
        parseResult.shift();
        if (void 0 === parseResult[3]) {
            parseResult.splice(3)
        } else {
            parseResult.splice(3, 1);
            parseResult.splice(6)
        }
        parseResult[1]--;
        parseResult.unshift(null);
        return [parseInt(parseResult[1]), parseInt(parseResult[2]), parseInt(parseResult[3]), parseInt(parseResult[4]) || 0, parseInt(parseResult[5]) || 0, parseInt(parseResult[6]) || 0, isUtc]
    }
}
class RecurrenceValidator {
    validateRRule(rule, recurrence) {
        if (this._brokenRuleNameExists(rule) || -1 === inArray(rule.freq, freqNames) || this._wrongCountRule(rule) || this._wrongIntervalRule(rule) || this._wrongDayOfWeek(rule) || this._wrongByMonthDayRule(rule) || this._wrongByMonth(rule) || this._wrongUntilRule(rule)) {
            this._logBrokenRule(recurrence);
            return false
        }
        return true
    }
    _wrongUntilRule(rule) {
        var wrongUntil = false;
        var until = rule.until;
        if (void 0 !== until && !(until instanceof Date)) {
            wrongUntil = true
        }
        return wrongUntil
    }
    _wrongCountRule(rule) {
        var wrongCount = false;
        var count = rule.count;
        if (count && "string" === typeof count) {
            wrongCount = true
        }
        return wrongCount
    }
    _wrongByMonthDayRule(rule) {
        var wrongByMonthDay = false;
        var byMonthDay = rule.bymonthday;
        if (byMonthDay && isNaN(parseInt(byMonthDay))) {
            wrongByMonthDay = true
        }
        return wrongByMonthDay
    }
    _wrongByMonth(rule) {
        var wrongByMonth = false;
        var byMonth = rule.bymonth;
        if (byMonth && isNaN(parseInt(byMonth))) {
            wrongByMonth = true
        }
        return wrongByMonth
    }
    _wrongIntervalRule(rule) {
        var wrongInterval = false;
        var interval = rule.interval;
        if (interval && "string" === typeof interval) {
            wrongInterval = true
        }
        return wrongInterval
    }
    _wrongDayOfWeek(rule) {
        var byDay = rule.byday;
        var daysByRule = getRecurrenceProcessor().daysFromByDayRule(rule);
        var brokenDaysExist = false;
        if ("" === byDay) {
            brokenDaysExist = true
        }
        each(daysByRule, (function(_, day) {
            if (!Object.prototype.hasOwnProperty.call(days, day)) {
                brokenDaysExist = true;
                return false
            }
        }));
        return brokenDaysExist
    }
    _brokenRuleNameExists(rule) {
        var brokenRuleExists = false;
        each(rule, (function(ruleName) {
            if (-1 === inArray(ruleName, ruleNames)) {
                brokenRuleExists = true;
                return false
            }
        }));
        return brokenRuleExists
    }
    _logBrokenRule(recurrence) {
        if (-1 === inArray(recurrence, loggedWarnings)) {
            errors.log("W0006", recurrence);
            loggedWarnings.push(recurrence)
        }
    }
}
