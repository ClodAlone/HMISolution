/**
 * DevExtreme (esm/exporter/excel/excel.fill_helper.js)
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
import patternFillHelper from "./excel.pattern_fill_helper";
var fillHelper = {
    tryCreateTag: function(sourceObj) {
        var result = null;
        if (isDefined(sourceObj)) {
            result = {
                patternFill: patternFillHelper.tryCreateTag(sourceObj.patternFill)
            };
            if (fillHelper.isEmpty(result)) {
                result = null
            }
        }
        return result
    },
    tryCreateFillFromSimpleFormat: function() {
        var {
            backgroundColor: backgroundColor,
            fillPatternType: fillPatternType,
            fillPatternColor: fillPatternColor
        } = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : {};
        if (isDefined(backgroundColor) && !(isDefined(fillPatternType) && isDefined(fillPatternColor))) {
            return {
                patternFill: {
                    patternType: "solid",
                    foregroundColor: {
                        rgb: backgroundColor
                    }
                }
            }
        } else if (isDefined(fillPatternType) && isDefined(fillPatternColor)) {
            return {
                patternFill: {
                    patternType: fillPatternType,
                    foregroundColor: {
                        rgb: fillPatternColor
                    },
                    backgroundColor: {
                        rgb: backgroundColor
                    }
                }
            }
        }
    },
    copySimpleFormat: function(source, target) {
        if (void 0 !== source.backgroundColor) {
            target.backgroundColor = source.backgroundColor
        }
        if (void 0 !== source.fillPatternType) {
            target.fillPatternType = source.fillPatternType
        }
        if (void 0 !== source.fillPatternColor) {
            target.fillPatternColor = source.fillPatternColor
        }
    },
    copy: function(source) {
        var result = null;
        if (isDefined(source)) {
            result = {};
            if (void 0 !== source.patternFill) {
                result.patternFill = patternFillHelper.copy(source.patternFill)
            }
        }
        return result
    },
    areEqual: function(leftTag, rightTag) {
        return fillHelper.isEmpty(leftTag) && fillHelper.isEmpty(rightTag) || isDefined(leftTag) && isDefined(rightTag) && patternFillHelper.areEqual(leftTag.patternFill, rightTag.patternFill)
    },
    isEmpty: function(tag) {
        return !isDefined(tag) || patternFillHelper.isEmpty(tag.patternFill)
    },
    toXml: function(tag) {
        return tagHelper.toXml("fill", {}, patternFillHelper.toXml(tag.patternFill))
    }
};
export default fillHelper;
