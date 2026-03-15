/**
 * DevExtreme (esm/localization/date.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import dependencyInjector from "../core/utils/dependency_injector";
import {
    isString
} from "../core/utils/type";
import {
    each
} from "../core/utils/iterator";
import {
    inArray
} from "../core/utils/array";
import errors from "../core/errors";
import {
    getFormatter as getLDMLDateFormatter
} from "./ldml/date.formatter";
import {
    getFormat as getLDMLDateFormat
} from "./ldml/date.format";
import {
    getParser as getLDMLDateParser
} from "./ldml/date.parser";
import defaultDateNames from "./default_date_names";
import firstDayOfWeekData from "./cldr-data/first_day_of_week_data";
import localizationCore from "./core";
import numberLocalization from "./number";
import intlDateLocalization from "./intl/date";
var DEFAULT_DAY_OF_WEEK_INDEX = 0;
var hasIntl = "undefined" !== typeof Intl;
var FORMATS_TO_PATTERN_MAP = {
    shortdate: "M/d/y",
    shorttime: "h:mm a",
    longdate: "EEEE, MMMM d, y",
    longtime: "h:mm:ss a",
    monthandday: "MMMM d",
    monthandyear: "MMMM y",
    quarterandyear: "QQQ y",
    day: "d",
    year: "y",
    shortdateshorttime: "M/d/y, h:mm a",
    longdatelongtime: "EEEE, MMMM d, y, h:mm:ss a",
    month: "LLLL",
    shortyear: "yy",
    dayofweek: "EEEE",
    quarter: "QQQ",
    hour: "HH",
    minute: "mm",
    second: "ss",
    millisecond: "SSS",
    "datetime-local": "yyyy-MM-ddTHH':'mm':'ss"
};
var possiblePartPatterns = {
    year: ["y", "yy", "yyyy"],
    day: ["d", "dd"],
    month: ["M", "MM", "MMM", "MMMM"],
    hours: ["H", "HH", "h", "hh", "ah"],
    minutes: ["m", "mm"],
    seconds: ["s", "ss"],
    milliseconds: ["S", "SS", "SSS"]
};
var dateLocalization = dependencyInjector({
    engine: function() {
        return "base"
    },
    _getPatternByFormat: function(format) {
        return FORMATS_TO_PATTERN_MAP[format.toLowerCase()]
    },
    _expandPattern: function(pattern) {
        return this._getPatternByFormat(pattern) || pattern
    },
    formatUsesMonthName: function(format) {
        return -1 !== this._expandPattern(format).indexOf("MMMM")
    },
    formatUsesDayName: function(format) {
        return -1 !== this._expandPattern(format).indexOf("EEEE")
    },
    getFormatParts: function(format) {
        var pattern = this._getPatternByFormat(format) || format;
        var result = [];
        each(pattern.split(/\W+/), (_, formatPart) => {
            each(possiblePartPatterns, (partName, possiblePatterns) => {
                if (inArray(formatPart, possiblePatterns) > -1) {
                    result.push(partName)
                }
            })
        });
        return result
    },
    getMonthNames: function(format) {
        return defaultDateNames.getMonthNames(format)
    },
    getDayNames: function(format) {
        return defaultDateNames.getDayNames(format)
    },
    getQuarterNames: function(format) {
        return defaultDateNames.getQuarterNames(format)
    },
    getPeriodNames: function(format) {
        return defaultDateNames.getPeriodNames(format)
    },
    getTimeSeparator: function() {
        return ":"
    },
    is24HourFormat: function(format) {
        var amTime = new Date(2017, 0, 20, 11, 0, 0, 0);
        var pmTime = new Date(2017, 0, 20, 23, 0, 0, 0);
        var amTimeFormatted = this.format(amTime, format);
        var pmTimeFormatted = this.format(pmTime, format);
        for (var i = 0; i < amTimeFormatted.length; i++) {
            if (amTimeFormatted[i] !== pmTimeFormatted[i]) {
                return !isNaN(parseInt(amTimeFormatted[i]))
            }
        }
    },
    format: function(date, _format) {
        if (!date) {
            return
        }
        if (!_format) {
            return date
        }
        var formatter;
        if ("function" === typeof _format) {
            formatter = _format
        } else if (_format.formatter) {
            formatter = _format.formatter
        } else {
            _format = _format.type || _format;
            if (isString(_format)) {
                _format = FORMATS_TO_PATTERN_MAP[_format.toLowerCase()] || _format;
                return numberLocalization.convertDigits(getLDMLDateFormatter(_format, this)(date))
            }
        }
        if (!formatter) {
            return
        }
        return formatter(date)
    },
    parse: function(text, format) {
        var that = this;
        var ldmlFormat;
        var formatter;
        if (!text) {
            return
        }
        if (!format) {
            return this.parse(text, "shortdate")
        }
        if (format.parser) {
            return format.parser(text)
        }
        if ("string" === typeof format && !FORMATS_TO_PATTERN_MAP[format.toLowerCase()]) {
            ldmlFormat = format
        } else {
            formatter = value => {
                var text = that.format(value, format);
                return numberLocalization.convertDigits(text, true)
            };
            try {
                ldmlFormat = getLDMLDateFormat(formatter)
            } catch (e) {}
        }
        if (ldmlFormat) {
            text = numberLocalization.convertDigits(text, true);
            return getLDMLDateParser(ldmlFormat, this)(text)
        }
        errors.log("W0012");
        var result = new Date(text);
        if (!result || isNaN(result.getTime())) {
            return
        }
        return result
    },
    firstDayOfWeekIndex: function() {
        var index = localizationCore.getValueByClosestLocale(locale => firstDayOfWeekData[locale]);
        return void 0 === index ? DEFAULT_DAY_OF_WEEK_INDEX : index
    }
});
if (hasIntl) {
    dateLocalization.inject(intlDateLocalization)
}
export default dateLocalization;
