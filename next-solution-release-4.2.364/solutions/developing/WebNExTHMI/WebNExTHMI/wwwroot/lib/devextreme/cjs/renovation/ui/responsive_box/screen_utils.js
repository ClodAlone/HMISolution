/**
 * DevExtreme (cjs/renovation/ui/responsive_box/screen_utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.convertToScreenSizeQualifier = void 0;
var convertToScreenSizeQualifier = function(width) {
    if (width < 768) {
        return "xs"
    }
    if (width < 992) {
        return "sm"
    }
    if (width < 1200) {
        return "md"
    }
    return "lg"
};
exports.convertToScreenSizeQualifier = convertToScreenSizeQualifier;
