/**
 * DevExtreme (esm/exporter/excel/excel.cell_format_helper.js)
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
import cellAlignmentHelper from "./excel.cell_alignment_helper";
import fillHelper from "./excel.fill_helper";
import fontHelper from "./excel.font_helper";
var cellFormatHelper = {
    tryCreateTag: function(sourceObj, sharedItemsContainer) {
        var result = null;
        if (isDefined(sourceObj)) {
            var numberFormatId;
            if ("number" === typeof sourceObj.numberFormat) {
                numberFormatId = sourceObj.numberFormat
            } else {
                numberFormatId = sharedItemsContainer.registerNumberFormat(sourceObj.numberFormat)
            }
            var fill = sourceObj.fill;
            if (!isDefined(fill)) {
                fill = fillHelper.tryCreateFillFromSimpleFormat(sourceObj)
            }
            result = {
                numberFormatId: numberFormatId,
                alignment: cellAlignmentHelper.tryCreateTag(sourceObj.alignment),
                fontId: sharedItemsContainer.registerFont(sourceObj.font),
                fillId: sharedItemsContainer.registerFill(fill)
            };
            if (cellFormatHelper.isEmpty(result)) {
                result = null
            }
        }
        return result
    },
    copy: function(source) {
        var result;
        if (null === source) {
            result = null
        } else if (isDefined(source)) {
            result = {};
            if (void 0 !== source.numberFormat) {
                result.numberFormat = source.numberFormat
            }
            if (void 0 !== source.fill) {
                result.fill = fillHelper.copy(source.fill)
            } else {
                fillHelper.copySimpleFormat(source, result)
            }
            if (void 0 !== source.alignment) {
                result.alignment = cellAlignmentHelper.copy(source.alignment)
            }
            if (void 0 !== source.font) {
                result.font = fontHelper.copy(source.font)
            }
        }
        return result
    },
    areEqual: function(leftTag, rightTag) {
        return cellFormatHelper.isEmpty(leftTag) && cellFormatHelper.isEmpty(rightTag) || isDefined(leftTag) && isDefined(rightTag) && leftTag.fontId === rightTag.fontId && leftTag.numberFormatId === rightTag.numberFormatId && leftTag.fillId === rightTag.fillId && cellAlignmentHelper.areEqual(leftTag.alignment, rightTag.alignment)
    },
    isEmpty: function(tag) {
        return !isDefined(tag) || !isDefined(tag.fontId) && !isDefined(tag.numberFormatId) && !isDefined(tag.fillId) && cellAlignmentHelper.isEmpty(tag.alignment)
    },
    toXml: function(tag) {
        var isAlignmentEmpty = cellAlignmentHelper.isEmpty(tag.alignment);
        var applyNumberFormat;
        if (isDefined(tag.numberFormatId)) {
            applyNumberFormat = tag.numberFormatId > 0 ? 1 : 0
        }
        return tagHelper.toXml("xf", {
            xfId: 0,
            applyAlignment: isAlignmentEmpty ? null : 1,
            fontId: tag.fontId,
            applyNumberFormat: applyNumberFormat,
            fillId: tag.fillId,
            numFmtId: tag.numberFormatId
        }, isAlignmentEmpty ? null : cellAlignmentHelper.toXml(tag.alignment))
    }
};
export default cellFormatHelper;
