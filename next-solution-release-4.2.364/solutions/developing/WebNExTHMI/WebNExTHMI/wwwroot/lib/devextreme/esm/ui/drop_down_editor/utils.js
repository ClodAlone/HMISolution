/**
 * DevExtreme (esm/ui/drop_down_editor/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getOuterWidth
} from "../../core/utils/size";
import {
    hasWindow
} from "../../core/utils/window";
var getElementWidth = function($element) {
    if (hasWindow()) {
        return getOuterWidth($element)
    }
};
var getSizeValue = function(size) {
    if (null === size) {
        size = void 0
    }
    if ("function" === typeof size) {
        size = size()
    }
    return size
};
export {
    getElementWidth,
    getSizeValue
};
