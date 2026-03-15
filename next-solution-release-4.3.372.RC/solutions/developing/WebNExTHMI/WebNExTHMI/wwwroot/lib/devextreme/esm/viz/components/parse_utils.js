/**
 * DevExtreme (esm/viz/components/parse_utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    noop
} from "../../core/utils/common";
import dateSerialization from "../../core/utils/date_serialization";
import {
    isDefined
} from "../../core/utils/type";
var parsers = {
    string: function(val) {
        return isDefined(val) ? "" + val : val
    },
    numeric: function(val) {
        if (!isDefined(val)) {
            return val
        }
        var parsedVal = Number(val);
        if (isNaN(parsedVal)) {
            parsedVal = void 0
        }
        return parsedVal
    },
    datetime: function(val) {
        if (!isDefined(val)) {
            return val
        }
        var parsedVal;
        var numVal = Number(val);
        if (!isNaN(numVal)) {
            parsedVal = new Date(numVal)
        } else {
            parsedVal = dateSerialization.deserializeDate(val)
        }
        if (isNaN(Number(parsedVal))) {
            parsedVal = void 0
        }
        return parsedVal
    }
};
export function correctValueType(type) {
    return "numeric" === type || "datetime" === type || "string" === type ? type : ""
}
export var getParser = function(valueType) {
    return parsers[correctValueType(valueType)] || noop
};
