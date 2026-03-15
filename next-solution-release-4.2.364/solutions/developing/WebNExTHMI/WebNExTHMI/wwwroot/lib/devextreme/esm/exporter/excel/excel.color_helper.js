/**
 * DevExtreme (esm/exporter/excel/excel.color_helper.js)
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
var colorHelper = {
    _tryConvertColor: function(source) {
        if ("string" !== typeof source) {
            return source
        }
        var result;
        if (source.length > 0 && "#" === source[0]) {
            var colorCode = source.substr(1, source.length);
            if (6 === colorCode.length) {
                result = "FF" + colorCode
            } else if (8 === colorCode.length) {
                result = colorCode[6] + colorCode[7] + colorCode.substr(0, 6)
            } else {
                result = colorCode
            }
        } else {
            result = source
        }
        return result
    },
    tryCreateTag: function(sourceObj) {
        var result = null;
        if (isDefined(sourceObj)) {
            if ("string" === typeof sourceObj) {
                result = {
                    rgb: this._tryConvertColor(sourceObj)
                }
            } else {
                result = {
                    rgb: this._tryConvertColor(sourceObj.rgb),
                    theme: sourceObj.theme
                }
            }
            if (colorHelper.isEmpty(result)) {
                result = null
            }
        }
        return result
    },
    copy: function(source) {
        var result = null;
        if (isDefined(source)) {
            if ("string" === typeof source) {
                result = source
            } else {
                result = {};
                if (void 0 !== source.rgb) {
                    result.rgb = source.rgb
                }
                if (void 0 !== source.theme) {
                    result.theme = source.theme
                }
            }
        }
        return result
    },
    isEmpty: function(tag) {
        return !isDefined(tag) || !isDefined(tag.rgb) && !isDefined(tag.theme)
    },
    areEqual: function(leftTag, rightTag) {
        return colorHelper.isEmpty(leftTag) && colorHelper.isEmpty(rightTag) || isDefined(leftTag) && isDefined(rightTag) && leftTag.rgb === rightTag.rgb && leftTag.theme === rightTag.theme
    },
    toXml: function(tagName, tag) {
        return tagHelper.toXml(tagName, {
            rgb: tag.rgb,
            theme: tag.theme
        })
    }
};
export default colorHelper;
