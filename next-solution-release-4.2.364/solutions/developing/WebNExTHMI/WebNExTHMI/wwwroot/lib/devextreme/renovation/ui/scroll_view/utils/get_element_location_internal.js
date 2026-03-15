/**
 * DevExtreme (renovation/ui/scroll_view/utils/get_element_location_internal.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getElementLocationInternal = getElementLocationInternal;
var _inflector = require("../../../../core/utils/inflector");
var _get_relative_offset = require("./get_relative_offset");
var _consts = require("../common/consts");

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}

function getElementLocationInternal(targetElement, direction, containerElement, scrollOffset, offset) {
    var additionalOffset = _extends({
        top: 0,
        left: 0,
        right: 0,
        bottom: 0
    }, offset);
    var isVertical = direction === _consts.DIRECTION_VERTICAL;
    var prop = isVertical ? "top" : "left";
    var inverseProp = isVertical ? "bottom" : "right";
    var dimension = isVertical ? "height" : "width";
    var containerOffsetSize = containerElement["offset".concat((0, _inflector.titleize)(dimension))];
    var containerClientSize = containerElement["client".concat((0, _inflector.titleize)(dimension))];
    var containerSize = containerElement.getBoundingClientRect()[dimension];
    var elementSize = targetElement.getBoundingClientRect()[dimension];
    var scale = 1;
    if (Math.abs(containerSize - containerOffsetSize) > 1) {
        scale = containerSize / containerOffsetSize
    }
    var relativeElementOffset = (0, _get_relative_offset.getRelativeOffset)(_consts.SCROLLABLE_CONTENT_CLASS, targetElement)[prop] / scale;
    var containerScrollOffset = scrollOffset[prop];
    var relativeStartOffset = containerScrollOffset - relativeElementOffset + additionalOffset[prop];
    var relativeEndOffset = containerScrollOffset - relativeElementOffset - elementSize / scale + containerClientSize - additionalOffset[inverseProp];
    if (relativeStartOffset <= 0 && relativeEndOffset >= 0) {
        return containerScrollOffset
    }
    return containerScrollOffset - (Math.abs(relativeStartOffset) > Math.abs(relativeEndOffset) ? relativeEndOffset : relativeStartOffset)
}
