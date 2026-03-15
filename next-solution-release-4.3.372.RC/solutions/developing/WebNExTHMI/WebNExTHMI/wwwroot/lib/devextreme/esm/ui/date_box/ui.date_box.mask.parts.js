/**
 * DevExtreme (esm/ui/date_box/ui.date_box.mask.parts.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getPatternSetters
} from "../../localization/ldml/date.parser";
import {
    extend
} from "../../core/utils/extend";
import {
    fitIntoRange
} from "../../core/utils/math";
import {
    noop
} from "../../core/utils/common";
var monthGetter = date => date.getMonth() + 1;
var monthSetter = (date, value) => {
    var day = date.getDate();
    var monthLimits = getLimits("M", date);
    var newValue = fitIntoRange(parseInt(value), monthLimits.min, monthLimits.max);
    date.setMonth(newValue - 1, 1);
    var {
        min: min,
        max: max
    } = getLimits("dM", date);
    var newDay = fitIntoRange(day, min, max);
    date.setDate(newDay)
};
var PATTERN_GETTERS = {
    a: date => date.getHours() < 12 ? 0 : 1,
    E: "getDay",
    y: "getFullYear",
    M: monthGetter,
    L: monthGetter,
    d: "getDate",
    H: "getHours",
    h: "getHours",
    m: "getMinutes",
    s: "getSeconds",
    S: "getMilliseconds"
};
var PATTERN_SETTERS = extend({}, getPatternSetters(), {
    a: (date, value) => {
        var hours = date.getHours();
        var current = hours >= 12;
        if (current === !!parseInt(value)) {
            return
        }
        date.setHours((hours + 12) % 24)
    },
    d: (date, value) => {
        var lastDayInMonth = getLimits("dM", date).max;
        if (value > lastDayInMonth) {
            date.setMonth(date.getMonth() + 1)
        }
        date.setDate(value)
    },
    h: (date, value) => {
        var isPM = date.getHours() >= 12;
        date.setHours(+value % 12 + (isPM ? 12 : 0))
    },
    M: monthSetter,
    L: monthSetter,
    E: (date, value) => {
        if (value < 0) {
            return
        }
        date.setDate(date.getDate() - date.getDay() + parseInt(value))
    },
    y: (date, value) => {
        var currentYear = date.getFullYear();
        var valueLength = String(value).length;
        var maxLimitLength = String(getLimits("y", date).max).length;
        var newValue = parseInt(String(currentYear).substr(0, maxLimitLength - valueLength) + value);
        date.setFullYear(newValue)
    }
});
var getPatternGetter = patternChar => PATTERN_GETTERS[patternChar] || (() => patternChar);
export var renderDateParts = (text, regExpInfo) => {
    var result = regExpInfo.regexp.exec(text);
    var start = 0;
    var end = 0;
    var sections = [];
    var _loop = function(i) {
        start = end;
        end = start + result[i].length;
        var pattern = regExpInfo.patterns[i - 1].replace(/^'|'$/g, "");
        var getter = getPatternGetter(pattern[0]);
        sections.push({
            index: i - 1,
            isStub: pattern === result[i],
            caret: {
                start: start,
                end: end
            },
            pattern: pattern,
            text: result[i],
            limits: function() {
                for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
                    args[_key] = arguments[_key]
                }
                return getLimits(pattern[0], ...args)
            },
            setter: PATTERN_SETTERS[pattern[0]] || noop,
            getter: getter
        })
    };
    for (var i = 1; i < result.length; i++) {
        _loop(i)
    }
    return sections
};
var getLimits = (pattern, date, forcedPattern) => {
    var limits = {
        y: {
            min: 0,
            max: 9999
        },
        M: {
            min: 1,
            max: 12
        },
        L: {
            min: 1,
            max: 12
        },
        d: {
            min: 1,
            max: 31
        },
        dM: {
            min: 1,
            max: new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate()
        },
        E: {
            min: 0,
            max: 6
        },
        H: {
            min: 0,
            max: 23
        },
        h: {
            min: 0,
            max: 23
        },
        m: {
            min: 0,
            max: 59
        },
        s: {
            min: 0,
            max: 59
        },
        S: {
            min: 0,
            max: 999
        },
        a: {
            min: 0,
            max: 1
        }
    };
    return limits[forcedPattern || pattern] || limits.getAmPm
};
export var getDatePartIndexByPosition = (dateParts, position) => {
    for (var i = 0; i < dateParts.length; i++) {
        var caretInGroup = dateParts[i].caret.end >= position;
        if (!dateParts[i].isStub && caretInGroup) {
            return i
        }
    }
    return null
};
