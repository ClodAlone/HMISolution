/**
 * DevExtreme (cjs/ui/pivot_grid/utils/get_scrollbar_width.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getScrollbarWidth = getScrollbarWidth;

function getScrollbarWidth(containerElement) {
    return containerElement.offsetWidth - containerElement.clientWidth
}
