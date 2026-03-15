/**
 * DevExtreme (esm/localization/intl/date.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../../core/utils/extend";
import localizationCoreUtils from "../core";
var SYMBOLS_TO_REMOVE_REGEX = /[\u200E\u200F]/g;
var getIntlFormatter = format => date => {
    if (!format.timeZoneName) {
        var year = date.getFullYear();
        var recognizableAsTwentyCentury = String(year).length < 3;
        var temporaryYearValue = recognizableAsTwentyCentury ? year + 400 : year;
        var utcDate = new Date(Date.UTC(temporaryYearValue, date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds()));
        if (recognizableAsTwentyCentury) {
            utcDate.setFullYear(year)
        }
        var utcFormat = extend({
            timeZone: "UTC"
        }, format);
        return formatDateTime(utcDate, utcFormat)
    }
    return formatDateTime(date, format)
};
var formattersCache = {};
var getFormatter = format => {
    var key = localizationCoreUtils.locale() + "/" + JSON.stringify(format);
    if (!formattersCache[key]) {
        formattersCache[key] = new Intl.DateTimeFormat(localizationCoreUtils.locale(), format).format
    }
    return formattersCache[key]
};

function formatDateTime(date, format) {
    return getFormatter(format)(date).replace(SYMBOLS_TO_REMOVE_REGEX, "")
}
var formatNumber = number => new Intl.NumberFormat(localizationCoreUtils.locale()).format(number);
var getAlternativeNumeralsMap = (() => {
    var numeralsMapCache = {};
    return locale => {
        if (!(locale in numeralsMapCache)) {
            if ("0" === formatNumber(0)) {
                numeralsMapCache[locale] = false;
                return false
            }
            numeralsMapCache[locale] = {};
            for (var i = 0; i < 10; ++i) {
                numeralsMapCache[locale][formatNumber(i)] = i
            }
        }
        return numeralsMapCache[locale]
    }
})();
var normalizeNumerals = dateString => {
    var alternativeNumeralsMap = getAlternativeNumeralsMap(localizationCoreUtils.locale());
    if (!alternativeNumeralsMap) {
        return dateString
    }
    return dateString.split("").map(sign => sign in alternativeNumeralsMap ? String(alternativeNumeralsMap[sign]) : sign).join("")
};
var removeLeadingZeroes = str => str.replace(/(\D)0+(\d)/g, "$1$2");
var dateStringEquals = (actual, expected) => removeLeadingZeroes(actual) === removeLeadingZeroes(expected);
var normalizeMonth = text => text.replace("d\u2019", "de ");
var intlFormats = {
    day: {
        day: "numeric"
    },
    dayofweek: {
        weekday: "long"
    },
    longdate: {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric"
    },
    longdatelongtime: {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric",
        hour: "numeric",
        minute: "numeric",
        second: "numeric"
    },
    longtime: {
        hour: "numeric",
        minute: "numeric",
        second: "numeric"
    },
    month: {
        month: "long"
    },
    monthandday: {
        month: "long",
        day: "numeric"
    },
    monthandyear: {
        year: "numeric",
        month: "long"
    },
    shortdate: {},
    shorttime: {
        hour: "numeric",
        minute: "numeric"
    },
    shortyear: {
        year: "2-digit"
    },
    year: {
        year: "numeric"
    }
};
Object.defineProperty(intlFormats, "shortdateshorttime", {
    get: function() {
        var defaultOptions = Intl.DateTimeFormat(localizationCoreUtils.locale()).resolvedOptions();
        return {
            year: defaultOptions.year,
            month: defaultOptions.month,
            day: defaultOptions.day,
            hour: "numeric",
            minute: "numeric"
        }
    }
});
var getIntlFormat = format => "string" === typeof format && intlFormats[format.toLowerCase()];
var monthNameStrategies = {
    standalone: function(monthIndex, monthFormat) {
        var date = new Date(1999, monthIndex, 13, 1);
        var dateString = getIntlFormatter({
            month: monthFormat
        })(date);
        return dateString
    },
    format: function(monthIndex, monthFormat) {
        var date = new Date(0, monthIndex, 13, 1);
        var dateString = normalizeMonth(getIntlFormatter({
            day: "numeric",
            month: monthFormat
        })(date));
        var parts = dateString.split(" ").filter(part => part.indexOf("13") < 0);
        if (1 === parts.length) {
            return parts[0]
        } else if (2 === parts.length) {
            return parts[0].length > parts[1].length ? parts[0] : parts[1]
        }
        return monthNameStrategies.standalone(monthIndex, monthFormat)
    }
};
export default {
    engine: function() {
        return "intl"
    },
    getMonthNames: function(format, type) {
        var monthFormat = {
            wide: "long",
            abbreviated: "short",
            narrow: "narrow"
        } [format || "wide"];
        type = "format" === type ? type : "standalone";
        return Array.apply(null, new Array(12)).map((_, monthIndex) => monthNameStrategies[type](monthIndex, monthFormat))
    },
    getDayNames: function(format) {
        var result = (format => Array.apply(null, new Array(7)).map((_, dayIndex) => getIntlFormatter({
            weekday: format
        })(new Date(0, 0, dayIndex))))({
            wide: "long",
            abbreviated: "short",
            short: "narrow",
            narrow: "narrow"
        } [format || "wide"]);
        return result
    },
    getPeriodNames: function() {
        var hour12Formatter = getIntlFormatter({
            hour: "numeric",
            hour12: true
        });
        return [1, 13].map(hours => {
            var hourNumberText = formatNumber(1);
            var timeParts = hour12Formatter(new Date(0, 0, 1, hours)).split(hourNumberText);
            if (2 !== timeParts.length) {
                return ""
            }
            var biggerPart = timeParts[0].length > timeParts[1].length ? timeParts[0] : timeParts[1];
            return biggerPart.trim()
        })
    },
    format: function(date, _format) {
        if (!date) {
            return
        }
        if (!_format) {
            return date
        }
        if ("function" !== typeof _format && !_format.formatter) {
            _format = _format.type || _format
        }
        var intlFormat = getIntlFormat(_format);
        if (intlFormat) {
            return getIntlFormatter(intlFormat)(date)
        }
        var formatType = typeof _format;
        if (_format.formatter || "function" === formatType || "string" === formatType) {
            return this.callBase.apply(this, arguments)
        }
        return getIntlFormatter(_format)(date)
    },
    parse: function(dateString, format) {
        var formatter;
        if (format && !format.parser && "string" === typeof dateString) {
            dateString = normalizeMonth(dateString);
            formatter = date => normalizeMonth(this.format(date, format))
        }
        return this.callBase(dateString, formatter || format)
    },
    _parseDateBySimpleFormat: function(dateString, format) {
        dateString = normalizeNumerals(dateString);
        var formatParts = this.getFormatParts(format);
        var dateParts = dateString.split(/\D+/).filter(part => part.length > 0);
        if (formatParts.length !== dateParts.length) {
            return
        }
        var dateArgs = this._generateDateArgs(formatParts, dateParts);
        var constructValidDate = ampmShift => {
            var parsedDate = ((dateArgs, ampmShift) => {
                var hoursShift = ampmShift ? 12 : 0;
                return new Date(dateArgs.year, dateArgs.month, dateArgs.day, (dateArgs.hours + hoursShift) % 24, dateArgs.minutes, dateArgs.seconds)
            })(dateArgs, ampmShift);
            if (dateStringEquals(normalizeNumerals(this.format(parsedDate, format)), dateString)) {
                return parsedDate
            }
        };
        return constructValidDate(false) || constructValidDate(true)
    },
    _generateDateArgs: function(formatParts, dateParts) {
        var currentDate = new Date;
        var dateArgs = {
            year: currentDate.getFullYear(),
            month: currentDate.getMonth(),
            day: currentDate.getDate(),
            hours: 0,
            minutes: 0,
            seconds: 0
        };
        formatParts.forEach((formatPart, index) => {
            var datePart = dateParts[index];
            var parsed = parseInt(datePart, 10);
            if ("month" === formatPart) {
                parsed -= 1
            }
            dateArgs[formatPart] = parsed
        });
        return dateArgs
    },
    formatUsesMonthName: function(format) {
        if ("object" === typeof format && !(format.type || format.format)) {
            return "long" === format.month
        }
        return this.callBase.apply(this, arguments)
    },
    formatUsesDayName: function(format) {
        if ("object" === typeof format && !(format.type || format.format)) {
            return "long" === format.weekday
        }
        return this.callBase.apply(this, arguments)
    },
    getTimeSeparator: function() {
        return normalizeNumerals(formatDateTime(new Date(2001, 1, 1, 11, 11), {
            hour: "numeric",
            minute: "numeric",
            hour12: false
        })).replace(/\d/g, "")
    },
    getFormatParts: function(format) {
        if ("string" === typeof format) {
            return this.callBase(format)
        }
        var intlFormat = extend({}, intlFormats[format.toLowerCase()]);
        var date = new Date(2001, 2, 4, 5, 6, 7);
        var formattedDate = getIntlFormatter(intlFormat)(date);
        formattedDate = normalizeNumerals(formattedDate);
        return [{
            name: "year",
            value: 1
        }, {
            name: "month",
            value: 3
        }, {
            name: "day",
            value: 4
        }, {
            name: "hours",
            value: 5
        }, {
            name: "minutes",
            value: 6
        }, {
            name: "seconds",
            value: 7
        }].map(part => ({
            name: part.name,
            index: formattedDate.indexOf(part.value)
        })).filter(part => part.index > -1).sort((a, b) => a.index - b.index).map(part => part.name)
    }
};
