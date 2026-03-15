/**
 * DevExtreme (esm/exporter/excel/excel.font_helper.js)
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
var fontHelper = {
    tryCreateTag: function(sourceObj) {
        var result = null;
        if (isDefined(sourceObj)) {
            result = {
                size: sourceObj.size,
                name: sourceObj.name,
                family: sourceObj.family,
                scheme: sourceObj.scheme,
                bold: sourceObj.bold,
                italic: sourceObj.italic,
                underline: sourceObj.underline,
                color: colorHelper.tryCreateTag(sourceObj.color)
            };
            if (fontHelper.isEmpty(result)) {
                result = null
            }
        }
        return result
    },
    copy: function(source) {
        var result = null;
        if (isDefined(source)) {
            result = {};
            if (void 0 !== source.size) {
                result.size = source.size
            }
            if (void 0 !== source.name) {
                result.name = source.name
            }
            if (void 0 !== source.family) {
                result.family = source.family
            }
            if (void 0 !== source.scheme) {
                result.scheme = source.scheme
            }
            if (void 0 !== source.bold) {
                result.bold = source.bold
            }
            if (void 0 !== source.italic) {
                result.italic = source.italic
            }
            if (void 0 !== source.underline) {
                result.underline = source.underline
            }
            if (void 0 !== source.color) {
                result.color = colorHelper.copy(source.color)
            }
        }
        return result
    },
    areEqual: function(leftTag, rightTag) {
        return fontHelper.isEmpty(leftTag) && fontHelper.isEmpty(rightTag) || isDefined(leftTag) && isDefined(rightTag) && leftTag.size === rightTag.size && leftTag.name === rightTag.name && leftTag.family === rightTag.family && leftTag.scheme === rightTag.scheme && (leftTag.bold === rightTag.bold || !leftTag.bold === !rightTag.bold) && (leftTag.italic === rightTag.italic || !leftTag.italic === !rightTag.italic) && leftTag.underline === rightTag.underline && colorHelper.areEqual(leftTag.color, rightTag.color)
    },
    isEmpty: function(tag) {
        return !isDefined(tag) || !isDefined(tag.size) && !isDefined(tag.name) && !isDefined(tag.family) && !isDefined(tag.scheme) && (!isDefined(tag.bold) || !tag.bold) && (!isDefined(tag.italic) || !tag.italic) && !isDefined(tag.underline) && colorHelper.isEmpty(tag.color)
    },
    toXml: function(tag) {
        var content = [isDefined(tag.bold) && tag.bold ? tagHelper.toXml("b", {}) : "", isDefined(tag.size) ? tagHelper.toXml("sz", {
            val: tag.size
        }) : "", isDefined(tag.color) ? colorHelper.toXml("color", tag.color) : "", isDefined(tag.name) ? tagHelper.toXml("name", {
            val: tag.name
        }) : "", isDefined(tag.family) ? tagHelper.toXml("family", {
            val: tag.family
        }) : "", isDefined(tag.scheme) ? tagHelper.toXml("scheme", {
            val: tag.scheme
        }) : "", isDefined(tag.italic) && tag.italic ? tagHelper.toXml("i", {}) : "", isDefined(tag.underline) ? tagHelper.toXml("u", {
            val: tag.underline
        }) : ""].join("");
        return tagHelper.toXml("font", {}, content)
    }
};
export default fontHelper;
