/**
 * DevExtreme (renovation/ui/scroll_view/utils/is_element_visible.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.isElementVisible = isElementVisible;

function isElementVisible(element) {
    if (element) {
        var _element$getClientRec;
        return !!(element.offsetWidth || element.offsetHeight || null !== (_element$getClientRec = element.getClientRects) && void 0 !== _element$getClientRec && _element$getClientRec.call(element).length)
    }
    return false
}
