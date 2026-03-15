/**
 * DevExtreme (esm/renovation/ui/pager/utils/get_element_width.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import getElementComputedStyle from "../../../utils/get_computed_style";
import {
    toNumber
} from "../../../utils/type_conversion";
export function getElementStyle(name, element) {
    var _getElementComputedSt;
    var computedStyle = null !== (_getElementComputedSt = getElementComputedStyle(element)) && void 0 !== _getElementComputedSt ? _getElementComputedSt : {};
    return toNumber(computedStyle[name])
}
export function getElementWidth(element) {
    return getElementStyle("width", element)
}
export function getElementMinWidth(element) {
    return getElementStyle("minWidth", element)
}
