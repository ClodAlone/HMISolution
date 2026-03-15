/**
 * DevExtreme (esm/renovation/utils/get_element_offset.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWindow,
    hasWindow
} from "../../core/utils/window";
var window = getWindow();
var DEFAULT_OFFSET = {
    top: 0,
    left: 0
};
export function getElementOffset(el) {
    if (el && hasWindow()) {
        var rect = el.getBoundingClientRect();
        return {
            top: rect.top + window.scrollY,
            left: rect.left + window.scrollX
        }
    }
    return DEFAULT_OFFSET
}
