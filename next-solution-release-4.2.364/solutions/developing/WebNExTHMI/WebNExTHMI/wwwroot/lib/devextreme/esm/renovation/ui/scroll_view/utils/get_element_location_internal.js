/**
 * DevExtreme (esm/renovation/ui/scroll_view/utils/get_element_location_internal.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    titleize
} from "../../../../core/utils/inflector";
import {
    getRelativeOffset
} from "./get_relative_offset";
import {
    DIRECTION_VERTICAL,
    SCROLLABLE_CONTENT_CLASS
} from "../common/consts";
export function getElementLocationInternal(targetElement, direction, containerElement, scrollOffset, offset) {
    var additionalOffset = _extends({
        top: 0,
        left: 0,
        right: 0,
        bottom: 0
    }, offset);
    var isVertical = direction === DIRECTION_VERTICAL;
    var prop = isVertical ? "top" : "left";
    var inverseProp = isVertical ? "bottom" : "right";
    var dimension = isVertical ? "height" : "width";
    var containerOffsetSize = containerElement["offset".concat(titleize(dimension))];
    var containerClientSize = containerElement["client".concat(titleize(dimension))];
    var containerSize = containerElement.getBoundingClientRect()[dimension];
    var elementSize = targetElement.getBoundingClientRect()[dimension];
    var scale = 1;
    if (Math.abs(containerSize - containerOffsetSize) > 1) {
        scale = containerSize / containerOffsetSize
    }
    var relativeElementOffset = getRelativeOffset(SCROLLABLE_CONTENT_CLASS, targetElement)[prop] / scale;
    var containerScrollOffset = scrollOffset[prop];
    var relativeStartOffset = containerScrollOffset - relativeElementOffset + additionalOffset[prop];
    var relativeEndOffset = containerScrollOffset - relativeElementOffset - elementSize / scale + containerClientSize - additionalOffset[inverseProp];
    if (relativeStartOffset <= 0 && relativeEndOffset >= 0) {
        return containerScrollOffset
    }
    return containerScrollOffset - (Math.abs(relativeStartOffset) > Math.abs(relativeEndOffset) ? relativeEndOffset : relativeStartOffset)
}
