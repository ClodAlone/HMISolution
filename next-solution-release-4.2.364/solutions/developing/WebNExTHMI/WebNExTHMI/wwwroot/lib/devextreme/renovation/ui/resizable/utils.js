/**
 * DevExtreme (renovation/ui/resizable/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getAreaFromObject = exports.getAreaFromElement = exports.filterOffsets = exports.borderWidthStyles = void 0;
exports.getDragOffsets = getDragOffsets;
exports.getMovingSides = void 0;
var _type = require("../../../core/utils/type");
var _extend = require("../../../core/utils/extend");
var _size = require("../../../core/utils/size");
var borderWidthStyles = {
    left: "borderLeftWidth",
    top: "borderTopWidth",
    right: "borderRightWidth",
    bottom: "borderBottomWidth"
};
exports.borderWidthStyles = borderWidthStyles;

function getBorderWidth(el, direction) {
    if (!(0, _type.isWindow)(el)) {
        var borderWidth = el.style[borderWidthStyles[direction]];
        return parseInt(borderWidth, 10) || 0
    }
    return 0
}
var correctGeometry = function(area, mainEl) {
    var el = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : void 0;
    var height = area.height,
        offset = area.offset,
        width = area.width;
    var left = offset.left,
        top = offset.top;
    var areaBorderLeft = el ? getBorderWidth(el, "left") : 0;
    var areaBorderTop = el ? getBorderWidth(el, "top") : 0;
    return {
        width: width - (0, _size.getOuterWidth)(mainEl) - (0, _size.getInnerWidth)(mainEl),
        height: height - (0, _size.getOuterHeight)(mainEl) - (0, _size.getInnerHeight)(mainEl),
        offset: {
            left: left + areaBorderLeft + getBorderWidth(mainEl, "left"),
            top: top + areaBorderTop + getBorderWidth(mainEl, "top")
        }
    }
};
var getAreaFromElement = function(el, mainEl) {
    return correctGeometry({
        width: (0, _size.getInnerWidth)(el),
        height: (0, _size.getInnerHeight)(el),
        offset: (0, _extend.extend)({
            top: 0,
            left: 0
        }, (0, _type.isWindow)(el) ? {} : (0, _size.getOffset)(el))
    }, mainEl, el)
};
exports.getAreaFromElement = getAreaFromElement;
var getAreaFromObject = function(_ref, mainEl) {
    var bottom = _ref.bottom,
        left = _ref.left,
        right = _ref.right,
        top = _ref.top;
    return correctGeometry({
        width: right - left,
        height: bottom - top,
        offset: {
            left: left,
            top: top
        }
    }, mainEl)
};
exports.getAreaFromObject = getAreaFromObject;
var getMovingSides = function(el) {
    var className = el.className;
    var hasCornerTopLeftClass = className.includes("dx-resizable-handle-corner-top-left");
    var hasCornerTopRightClass = className.includes("dx-resizable-handle-corner-top-right");
    var hasCornerBottomLeftClass = className.includes("dx-resizable-handle-corner-bottom-left");
    var hasCornerBottomRightClass = className.includes("dx-resizable-handle-corner-bottom-right");
    return {
        top: className.includes("dx-resizable-handle-top") || hasCornerTopLeftClass || hasCornerTopRightClass,
        left: className.includes("dx-resizable-handle-left") || hasCornerTopLeftClass || hasCornerBottomLeftClass,
        bottom: className.includes("dx-resizable-handle-bottom") || hasCornerBottomLeftClass || hasCornerBottomRightClass,
        right: className.includes("dx-resizable-handle-right") || hasCornerTopRightClass || hasCornerBottomRightClass
    }
};
exports.getMovingSides = getMovingSides;

function getDragOffsets(area, handleEl, areaProp) {
    var hWidth = (0, _size.getOuterWidth)(handleEl);
    var hHeight = (0, _size.getOuterHeight)(handleEl);
    var hOffset = (0, _size.getOffset)(handleEl);
    var areaOffset = area.offset;
    var isAreaWindow = (0, _type.isWindow)(areaProp);
    var scrollOffset_scrollX = isAreaWindow ? areaProp.pageXOffset : 0,
        scrollOffset_scrollY = isAreaWindow ? areaProp.pageYOffset : 0;
    return {
        maxLeftOffset: hOffset.left - areaOffset.left - scrollOffset_scrollX,
        maxRightOffset: areaOffset.left + area.width - hOffset.left - hWidth + scrollOffset_scrollX,
        maxTopOffset: hOffset.top - areaOffset.top - scrollOffset_scrollY,
        maxBottomOffset: areaOffset.top + area.height - hOffset.top - hHeight + scrollOffset_scrollY
    }
}
var filterOffsets = function(offset, handleEl) {
    var sides = getMovingSides(handleEl);
    var offsetX = !sides.left && !sides.right ? 0 : offset.x;
    var offsetY = !sides.top && !sides.bottom ? 0 : offset.y;
    return {
        x: offsetX,
        y: offsetY
    }
};
exports.filterOffsets = filterOffsets;
