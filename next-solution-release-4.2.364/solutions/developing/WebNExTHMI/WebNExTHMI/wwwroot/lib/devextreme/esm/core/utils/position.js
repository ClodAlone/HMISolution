/**
 * DevExtreme (esm/core/utils/position.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import config from "../config";
import {
    isWindow
} from "../utils/type";
var getDefaultAlignment = isRtlEnabled => {
    var rtlEnabled = null !== isRtlEnabled && void 0 !== isRtlEnabled ? isRtlEnabled : config().rtlEnabled;
    return rtlEnabled ? "right" : "left"
};
var getBoundingRect = element => {
    if (isWindow(element)) {
        return {
            width: element.outerWidth,
            height: element.outerHeight
        }
    }
    return element.getBoundingClientRect()
};
export {
    getBoundingRect,
    getDefaultAlignment
};
