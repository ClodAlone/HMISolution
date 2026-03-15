/**
 * DevExtreme (esm/animation/position.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getOuterWidth,
    getOuterHeight,
    getWidth,
    getHeight
} from "../core/utils/size";
import $ from "../core/renderer";
import {
    splitPair,
    pairToObject
} from "../core/utils/common";
import {
    each
} from "../core/utils/iterator";
import {
    getWindow
} from "../core/utils/window";
var window = getWindow();
import domAdapter from "../core/dom_adapter";
import {
    isWindow,
    isDefined
} from "../core/utils/type";
import {
    extend
} from "../core/utils/extend";
import {
    getBoundingRect
} from "../core/utils/position";
import browser from "../core/utils/browser";
import {
    resetPosition,
    move
} from "./translator";
import {
    touch
} from "../core/utils/support";
import devices from "../core/devices";
var horzRe = /left|right/;
var vertRe = /top|bottom/;
var collisionRe = /fit|flip|none/;
var scaleRe = /scale\(.+?\)/;
var IS_SAFARI = browser.safari;
var normalizeAlign = function(raw) {
    var result = {
        h: "center",
        v: "center"
    };
    var pair = splitPair(raw);
    if (pair) {
        each(pair, (function() {
            var w = String(this).toLowerCase();
            if (horzRe.test(w)) {
                result.h = w
            } else if (vertRe.test(w)) {
                result.v = w
            }
        }))
    }
    return result
};
var normalizeOffset = function(raw) {
    return pairToObject(raw)
};
var normalizeCollision = function(raw) {
    var pair = splitPair(raw);
    var h = String(pair && pair[0]).toLowerCase();
    var v = String(pair && pair[1]).toLowerCase();
    if (!collisionRe.test(h)) {
        h = "none"
    }
    if (!collisionRe.test(v)) {
        v = h
    }
    return {
        h: h,
        v: v
    }
};
var getAlignFactor = function(align) {
    switch (align) {
        case "center":
            return .5;
        case "right":
        case "bottom":
            return 1;
        default:
            return 0
    }
};
var inverseAlign = function(align) {
    switch (align) {
        case "left":
            return "right";
        case "right":
            return "left";
        case "top":
            return "bottom";
        case "bottom":
            return "top";
        default:
            return align
    }
};
var calculateOversize = function(data, bounds) {
    var oversize = 0;
    if (data.myLocation < bounds.min) {
        oversize += bounds.min - data.myLocation
    }
    if (data.myLocation > bounds.max) {
        oversize += data.myLocation - bounds.max
    }
    return oversize
};
var collisionSide = function(direction, data, bounds) {
    if (data.myLocation < bounds.min) {
        return "h" === direction ? "left" : "top"
    }
    if (data.myLocation > bounds.max) {
        return "h" === direction ? "right" : "bottom"
    }
    return "none"
};
var initMyLocation = function(data) {
    data.myLocation = data.atLocation + getAlignFactor(data.atAlign) * data.atSize - getAlignFactor(data.myAlign) * data.mySize + data.offset
};
var collisionResolvers = {
    fit: function(data, bounds) {
        var result = false;
        if (data.myLocation > bounds.max) {
            data.myLocation = bounds.max;
            result = true
        }
        if (data.myLocation < bounds.min) {
            data.myLocation = bounds.min;
            result = true
        }
        data.fit = result
    },
    flip: function(data, bounds) {
        data.flip = false;
        if ("center" === data.myAlign && "center" === data.atAlign) {
            return
        }
        if (data.myLocation < bounds.min || data.myLocation > bounds.max) {
            var inverseData = extend({}, data, {
                myAlign: inverseAlign(data.myAlign),
                atAlign: inverseAlign(data.atAlign),
                offset: -data.offset
            });
            initMyLocation(inverseData);
            inverseData.oversize = calculateOversize(inverseData, bounds);
            if (inverseData.myLocation >= bounds.min && inverseData.myLocation <= bounds.max || data.oversize > inverseData.oversize) {
                data.myLocation = inverseData.myLocation;
                data.oversize = inverseData.oversize;
                data.flip = true
            }
        }
    },
    flipfit: function(data, bounds) {
        this.flip(data, bounds);
        this.fit(data, bounds)
    },
    none: function(data) {
        data.oversize = 0
    }
};
var scrollbarWidth;
var calculateScrollbarWidth = function() {
    var $scrollDiv = $("<div>").css({
        width: 100,
        height: 100,
        overflow: "scroll",
        position: "absolute",
        top: -9999
    }).appendTo($("body"));
    var result = $scrollDiv.get(0).offsetWidth - $scrollDiv.get(0).clientWidth;
    $scrollDiv.remove();
    scrollbarWidth = result
};
var defaultPositionResult = {
    h: {
        location: 0,
        flip: false,
        fit: false,
        oversize: 0
    },
    v: {
        location: 0,
        flip: false,
        fit: false,
        oversize: 0
    }
};
var calculatePosition = function(what, options) {
    var $what = $(what);
    var currentOffset = $what.offset();
    var result = extend(true, {}, defaultPositionResult, {
        h: {
            location: currentOffset.left
        },
        v: {
            location: currentOffset.top
        }
    });
    if (!options) {
        return result
    }
    var my = normalizeAlign(options.my);
    var at = normalizeAlign(options.at);
    var of = $(options.of).length && options.of || window;
    var offset = normalizeOffset(options.offset);
    var collision = normalizeCollision(options.collision);
    var boundary = options.boundary;
    var boundaryOffset = normalizeOffset(options.boundaryOffset);
    var h = {
        mySize: getOuterWidth($what),
        myAlign: my.h,
        atAlign: at.h,
        offset: offset.h,
        collision: collision.h,
        boundaryOffset: boundaryOffset.h
    };
    var v = {
        mySize: getOuterHeight($what),
        myAlign: my.v,
        atAlign: at.v,
        offset: offset.v,
        collision: collision.v,
        boundaryOffset: boundaryOffset.v
    };
    if ( of .preventDefault) {
        h.atLocation = of .pageX;
        v.atLocation = of .pageY;
        h.atSize = 0;
        v.atSize = 0
    } else {
        of = $( of );
        if (isWindow( of [0])) {
            h.atLocation = of .scrollLeft();
            v.atLocation = of .scrollTop();
            if ("phone" === devices.real().deviceType && of [0].visualViewport) {
                h.atLocation = Math.max(h.atLocation, of [0].visualViewport.offsetLeft);
                v.atLocation = Math.max(v.atLocation, of [0].visualViewport.offsetTop);
                h.atSize = of [0].visualViewport.width;
                v.atSize = of [0].visualViewport.height
            } else {
                h.atSize = of [0].innerWidth > of [0].outerWidth ? of [0].innerWidth : getWidth( of );
                v.atSize = of [0].innerHeight > of [0].outerHeight || IS_SAFARI ? of [0].innerHeight : getHeight( of )
            }
        } else if (9 === of [0].nodeType) {
            h.atLocation = 0;
            v.atLocation = 0;
            h.atSize = getWidth( of );
            v.atSize = getHeight( of )
        } else {
            var ofRect = getBoundingRect( of .get(0));
            var o = getOffsetWithoutScale( of );
            h.atLocation = o.left;
            v.atLocation = o.top;
            h.atSize = Math.max(ofRect.width, getOuterWidth( of ));
            v.atSize = Math.max(ofRect.height, getOuterHeight( of ))
        }
    }
    initMyLocation(h);
    initMyLocation(v);
    var bounds = function() {
        var win = $(window);
        var windowWidth = getWidth(win);
        var windowHeight = getHeight(win);
        var left = win.scrollLeft();
        var top = win.scrollTop();
        var documentElement = domAdapter.getDocumentElement();
        var hZoomLevel = touch ? documentElement.clientWidth / windowWidth : 1;
        var vZoomLevel = touch ? documentElement.clientHeight / windowHeight : 1;
        if (void 0 === scrollbarWidth) {
            calculateScrollbarWidth()
        }
        var boundaryWidth = windowWidth;
        var boundaryHeight = windowHeight;
        if (boundary) {
            var $boundary = $(boundary);
            var boundaryPosition = $boundary.offset();
            left = boundaryPosition.left;
            top = boundaryPosition.top;
            boundaryWidth = getWidth($boundary);
            boundaryHeight = getHeight($boundary)
        }
        return {
            h: {
                min: left + h.boundaryOffset,
                max: left + boundaryWidth / hZoomLevel - h.mySize - h.boundaryOffset
            },
            v: {
                min: top + v.boundaryOffset,
                max: top + boundaryHeight / vZoomLevel - v.mySize - v.boundaryOffset
            }
        }
    }();
    h.oversize = calculateOversize(h, bounds.h);
    v.oversize = calculateOversize(v, bounds.v);
    h.collisionSide = collisionSide("h", h, bounds.h);
    v.collisionSide = collisionSide("v", v, bounds.v);
    if (collisionResolvers[h.collision]) {
        collisionResolvers[h.collision](h, bounds.h)
    }
    if (collisionResolvers[v.collision]) {
        collisionResolvers[v.collision](v, bounds.v)
    }
    var preciser = function(number) {
        return options.precise ? number : Math.round(number)
    };
    extend(true, result, {
        h: {
            location: preciser(h.myLocation),
            oversize: preciser(h.oversize),
            fit: h.fit,
            flip: h.flip,
            collisionSide: h.collisionSide
        },
        v: {
            location: preciser(v.myLocation),
            oversize: preciser(v.oversize),
            fit: v.fit,
            flip: v.flip,
            collisionSide: v.collisionSide
        },
        precise: options.precise
    });
    return result
};
var setScaleProperty = function(element, scale, transformProp, styleAttr, isEmpty) {
    var stylePropIsValid = isDefined(element.style) && !domAdapter.isNode(element.style);
    if (stylePropIsValid) {
        element.style.transform = isEmpty ? transformProp.replace(scale, "") : transformProp
    } else {
        element.setAttribute("style", isEmpty ? styleAttr.replace(scale, "") : styleAttr)
    }
};
var getOffsetWithoutScale = function getOffsetWithoutScale($startElement) {
    var _currentElement$getAt, _currentElement$style, _style$match;
    var $currentElement = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : $startElement;
    var currentElement = $currentElement.get(0);
    if (!currentElement) {
        return $startElement.offset()
    }
    var style = (null === (_currentElement$getAt = currentElement.getAttribute) || void 0 === _currentElement$getAt ? void 0 : _currentElement$getAt.call(currentElement, "style")) || "";
    var transform = null === (_currentElement$style = currentElement.style) || void 0 === _currentElement$style ? void 0 : _currentElement$style.transform;
    var scale = null === (_style$match = style.match(scaleRe)) || void 0 === _style$match ? void 0 : _style$match[0];
    var offset;
    if (scale) {
        setScaleProperty(currentElement, scale, transform, style, true);
        offset = getOffsetWithoutScale($startElement, $currentElement.parent());
        setScaleProperty(currentElement, scale, transform, style, false)
    } else {
        offset = getOffsetWithoutScale($startElement, $currentElement.parent())
    }
    return offset
};
var position = function(what, options) {
    var $what = $(what);
    if (!options) {
        return $what.offset()
    }
    resetPosition($what, true);
    var offset = getOffsetWithoutScale($what);
    var targetPosition = options.h && options.v ? options : calculatePosition($what, options);
    var preciser = function(number) {
        return options.precise ? number : Math.round(number)
    };
    move($what, {
        left: targetPosition.h.location - preciser(offset.left),
        top: targetPosition.v.location - preciser(offset.top)
    });
    return targetPosition
};
var offset = function(element) {
    element = $(element).get(0);
    if (isWindow(element)) {
        return null
    } else if (element && "pageY" in element && "pageX" in element) {
        return {
            top: element.pageY,
            left: element.pageX
        }
    }
    return $(element).offset()
};
if (!position.inverseAlign) {
    position.inverseAlign = inverseAlign
}
if (!position.normalizeAlign) {
    position.normalizeAlign = normalizeAlign
}
export default {
    calculateScrollbarWidth: calculateScrollbarWidth,
    calculate: calculatePosition,
    setup: position,
    offset: offset
};
