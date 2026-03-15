/**
 * DevExtreme (esm/core/utils/view_port.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../renderer";
import readyCallbacks from "./ready_callbacks";
var ready = readyCallbacks.add;
import callbacks from "./callbacks";
var changeCallback = callbacks();
var $originalViewPort = $();
var value = function() {
    var $current;
    return function(element) {
        if (!arguments.length) {
            return $current
        }
        var $element = $(element);
        $originalViewPort = $element;
        var isNewViewportFound = !!$element.length;
        var prevViewPort = value();
        $current = isNewViewportFound ? $element : $("body");
        changeCallback.fire(isNewViewportFound ? value() : $(), prevViewPort)
    }
}();
ready((function() {
    value(".dx-viewport")
}));
export {
    value,
    changeCallback
};
export function originalViewPort() {
    return $originalViewPort
}
