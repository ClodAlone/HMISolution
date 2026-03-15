/**
 * DevExtreme (esm/ui/pivot_grid/utils/calculate_scrollbar_width.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import domAdapter from "../../../core/dom_adapter";
import callOnce from "../../../core/utils/call_once";
import {
    getScrollbarWidth
} from "./get_scrollbar_width";
var calculateScrollbarWidth = callOnce((function() {
    var document = domAdapter.getDocument();
    document.body.insertAdjacentHTML("beforeend", "<div style='position: absolute; overflow: scroll; width: 100px; height: 100px; top: -9999;'></div>");
    var scrollbar = document.body.lastElementChild;
    var scrollbarWidth = getScrollbarWidth(scrollbar);
    document.body.removeChild(scrollbar);
    return scrollbarWidth
}));
export {
    calculateScrollbarWidth
};
