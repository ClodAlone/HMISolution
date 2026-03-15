/**
 * DevExtreme (esm/exporter/excel/excel.cell_alignment_helper.js)
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
var cellAlignmentHelper = {
    tryCreateTag: function(sourceObj) {
        var result = null;
        if (isDefined(sourceObj)) {
            result = {
                vertical: sourceObj.vertical,
                wrapText: sourceObj.wrapText,
                horizontal: sourceObj.horizontal
            };
            if (cellAlignmentHelper.isEmpty(result)) {
                result = null
            }
        }
        return result
    },
    copy: function(source) {
        var result = null;
        if (isDefined(source)) {
            result = {};
            if (void 0 !== source.horizontal) {
                result.horizontal = source.horizontal
            }
            if (void 0 !== source.vertical) {
                result.vertical = source.vertical
            }
            if (void 0 !== source.wrapText) {
                result.wrapText = source.wrapText
            }
        }
        return result
    },
    areEqual: function(leftTag, rightTag) {
        return cellAlignmentHelper.isEmpty(leftTag) && cellAlignmentHelper.isEmpty(rightTag) || isDefined(leftTag) && isDefined(rightTag) && leftTag.vertical === rightTag.vertical && leftTag.wrapText === rightTag.wrapText && leftTag.horizontal === rightTag.horizontal
    },
    isEmpty: function(tag) {
        return !isDefined(tag) || !isDefined(tag.vertical) && !isDefined(tag.wrapText) && !isDefined(tag.horizontal)
    },
    toXml: function(tag) {
        return tagHelper.toXml("alignment", {
            vertical: tag.vertical,
            wrapText: isDefined(tag.wrapText) ? Number(tag.wrapText) : void 0,
            horizontal: tag.horizontal
        })
    }
};
export default cellAlignmentHelper;
