/**
 * DevExtreme (esm/exporter/excel/excel.pattern_fill_helper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined
} from "../../core/utils/type";
import tagHelper from "./excel.tag_helper";
import colorHelper from "./excel.color_helper";
var patternFillHelper = {
    tryCreateTag: function(sourceObj) {
        var result = null;
        if (isDefined(sourceObj)) {
            result = {
                patternType: sourceObj.patternType,
                backgroundColor: colorHelper.tryCreateTag(sourceObj.backgroundColor),
                foregroundColor: colorHelper.tryCreateTag(sourceObj.foregroundColor)
            };
            if (patternFillHelper.isEmpty(result)) {
                result = null
            }
        }
        return result
    },
    copy: function(source) {
        var result = null;
        if (isDefined(source)) {
            result = {};
            if (void 0 !== source.patternType) {
                result.patternType = source.patternType
            }
            if (void 0 !== source.backgroundColor) {
                result.backgroundColor = colorHelper.copy(source.backgroundColor)
            }
            if (void 0 !== source.foregroundColor) {
                result.foregroundColor = colorHelper.copy(source.foregroundColor)
            }
        }
        return result
    },
    areEqual: function(leftTag, rightTag) {
        return patternFillHelper.isEmpty(leftTag) && patternFillHelper.isEmpty(rightTag) || isDefined(leftTag) && isDefined(rightTag) && leftTag.patternType === rightTag.patternType && colorHelper.areEqual(leftTag.backgroundColor, rightTag.backgroundColor) && colorHelper.areEqual(leftTag.foregroundColor, rightTag.foregroundColor)
    },
    isEmpty: function(tag) {
        return !isDefined(tag) || !isDefined(tag.patternType)
    },
    toXml: function(tag) {
        var content = [isDefined(tag.foregroundColor) ? colorHelper.toXml("fgColor", tag.foregroundColor) : "", isDefined(tag.backgroundColor) ? colorHelper.toXml("bgColor", tag.backgroundColor) : ""].join("");
        return tagHelper.toXml("patternFill", {
            patternType: tag.patternType
        }, content)
    }
};
export default patternFillHelper;
