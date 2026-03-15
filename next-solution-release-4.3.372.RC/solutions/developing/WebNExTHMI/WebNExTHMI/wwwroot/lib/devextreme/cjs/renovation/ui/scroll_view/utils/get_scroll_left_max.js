/**
 * DevExtreme (cjs/renovation/ui/scroll_view/utils/get_scroll_left_max.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getScrollLeftMax = getScrollLeftMax;

function getScrollLeftMax(element) {
    return element.scrollWidth - element.clientWidth
}
