/**
 * DevExtreme (esm/renovation/viz/common/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import formatHelper from "../../../format_helper";
import {
    isDefined
} from "../../../core/utils/type";
import getElementComputedStyle from "../../utils/get_computed_style";
import {
    toNumber
} from "../../utils/type_conversion";
export function getElementWidth(element) {
    var style = getElementComputedStyle(element);
    return toNumber(null === style || void 0 === style ? void 0 : style.width) - toNumber(null === style || void 0 === style ? void 0 : style.paddingLeft) - toNumber(null === style || void 0 === style ? void 0 : style.paddingRight)
}
export function getElementHeight(element) {
    var style = getElementComputedStyle(element);
    return toNumber(null === style || void 0 === style ? void 0 : style.height) - toNumber(null === style || void 0 === style ? void 0 : style.paddingTop) - toNumber(null === style || void 0 === style ? void 0 : style.paddingBottom)
}
export var sizeIsValid = value => !!(value && value > 0);
export var pickPositiveValue = values => values.reduce((result, value) => value && value > 0 && !result ? value : result, 0);
export var pointInCanvas = (canvas, x, y) => x >= canvas.left && x <= canvas.right && y >= canvas.top && y <= canvas.bottom;
export function getFormatValue(value, specialFormat, _ref) {
    var {
        argumentFormat: argumentFormat,
        format: format
    } = _ref;
    var option = format;
    if (specialFormat) {
        option = "argument" === specialFormat ? argumentFormat : {
            type: "percent",
            precision: null === format || void 0 === format ? void 0 : format.percentPrecision
        }
    }
    return formatHelper.format(value, option)
}
export function isUpdatedFlatObject(newState, oldState) {
    return (isDefined(newState) || isDefined(oldState)) && (!isDefined(newState) || !isDefined(oldState) || Object.keys(newState).some(key => newState[key] !== oldState[key]))
}
