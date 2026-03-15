/**
 * DevExtreme (esm/ui/text_box/utils.scroll.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    isDxMouseWheelEvent
} from "../../events/utils/index";
var allowScroll = function(container, delta, shiftKey) {
    var $container = $(container);
    var scrollTopPos = shiftKey ? $container.scrollLeft() : $container.scrollTop();
    var prop = shiftKey ? "Width" : "Height";
    var scrollBottomPos = $container.prop("scroll".concat(prop)) - $container.prop("client".concat(prop)) - scrollTopPos;
    if (0 === scrollTopPos && 0 === scrollBottomPos) {
        return false
    }
    var isScrollFromTop = 0 === scrollTopPos && delta >= 0;
    var isScrollFromBottom = 0 === scrollBottomPos && delta <= 0;
    var isScrollFromMiddle = scrollTopPos > 0 && scrollBottomPos > 0;
    if (isScrollFromTop || isScrollFromBottom || isScrollFromMiddle) {
        return true
    }
};
var prepareScrollData = function(container, validateTarget) {
    var $container = $(container);
    return {
        validate: function(e) {
            if (isDxMouseWheelEvent(e) && (eventTarget = e.target, validateTarget ? $(eventTarget).is(container) : true)) {
                if (allowScroll($container, -e.delta, e.shiftKey)) {
                    e._needSkipEvent = true;
                    return true
                }
                return false
            }
            var eventTarget
        }
    }
};
export {
    allowScroll,
    prepareScrollData
};
