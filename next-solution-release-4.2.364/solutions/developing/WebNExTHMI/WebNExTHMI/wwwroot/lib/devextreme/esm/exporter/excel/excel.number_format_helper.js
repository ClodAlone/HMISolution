/**
 * DevExtreme (esm/exporter/excel/excel.number_format_helper.js)
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
var numberFormatHelper = {
    ID_PROPERTY_NAME: "id",
    tryCreateTag: function(sourceObj) {
        var result = null;
        if ("string" === typeof sourceObj) {
            result = {
                formatCode: sourceObj
            };
            if (numberFormatHelper.isEmpty(result)) {
                result = null
            }
        }
        return result
    },
    areEqual: function(leftTag, rightTag) {
        return numberFormatHelper.isEmpty(leftTag) && numberFormatHelper.isEmpty(rightTag) || isDefined(leftTag) && isDefined(rightTag) && leftTag.formatCode === rightTag.formatCode
    },
    isEmpty: function(tag) {
        return !isDefined(tag) || !isDefined(tag.formatCode) || "" === tag.formatCode
    },
    toXml: function(tag) {
        return tagHelper.toXml("numFmt", {
            numFmtId: tag[numberFormatHelper.ID_PROPERTY_NAME],
            formatCode: tag.formatCode
        })
    }
};
export default numberFormatHelper;
