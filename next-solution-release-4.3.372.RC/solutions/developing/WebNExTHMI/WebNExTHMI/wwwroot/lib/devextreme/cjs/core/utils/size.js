/**
 * DevExtreme (cjs/core/utils/size.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.setWidth = exports.setOuterWidth = exports.setOuterHeight = exports.setInnerWidth = exports.setInnerHeight = exports.setHeight = exports.parseHeight = exports.implementationsMap = exports.getWindowByElement = exports.getWidth = exports.getVisibleHeight = exports.getVerticalOffsets = exports.getSize = exports.getOuterWidth = exports.getOuterHeight = exports.getOffset = exports.getInnerWidth = exports.getInnerHeight = exports.getHeight = exports.getElementBoxParams = exports.addOffsetToMinHeight = exports.addOffsetToMaxHeight = void 0;
var _window = require("../../core/utils/window");
var _dom_adapter = _interopRequireDefault(require("../../core/dom_adapter"));
var _type = require("../utils/type");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var window = (0, _window.getWindow)();
var SPECIAL_HEIGHT_VALUES = ["auto", "none", "inherit", "initial"];
var getSizeByStyles = function(elementStyles, styles) {
    var result = 0;
    styles.forEach((function(style) {
        result += parseFloat(elementStyles[style]) || 0
    }));
    return result
};
var getElementBoxParams = function(name, elementStyles) {
    var beforeName = "width" === name ? "Left" : "Top";
    var afterName = "width" === name ? "Right" : "Bottom";
    return {
        padding: getSizeByStyles(elementStyles, ["padding" + beforeName, "padding" + afterName]),
        border: getSizeByStyles(elementStyles, ["border" + beforeName + "Width", "border" + afterName + "Width"]),
        margin: getSizeByStyles(elementStyles, ["margin" + beforeName, "margin" + afterName])
    }
};
exports.getElementBoxParams = getElementBoxParams;
var getElementComputedStyle = function(element) {
    var _element$ownerDocumen;
    var view = (null === element || void 0 === element ? void 0 : null === (_element$ownerDocumen = element.ownerDocument) || void 0 === _element$ownerDocumen ? void 0 : _element$ownerDocumen.defaultView) || window;
    return view.getComputedStyle && view.getComputedStyle(element)
};
var getCSSProperty = function(element, styles, name, defaultValue) {
    var _element$style;
    return (null === styles || void 0 === styles ? void 0 : styles[name]) || (null === (_element$style = element.style) || void 0 === _element$style ? void 0 : _element$style[name]) || defaultValue
};
var boxIndices = {
    content: 0,
    padding: 1,
    border: 2,
    margin: 3,
    "content-box": 0,
    "border-box": 2
};
var dimensionComponents = {
    width: ["left", "right"],
    height: ["top", "bottom"]
};

function getComponentThickness(elem, dimension, component, styles) {
    var get = function(elem, styles, field) {
        return parseFloat(getCSSProperty(elem, styles, field, "0")) || 0
    };
    var suffix = "border" === component ? "-width" : "";
    return get(elem, styles, "".concat(component, "-").concat(dimensionComponents[dimension][0]).concat(suffix)) + get(elem, styles, "".concat(component, "-").concat(dimensionComponents[dimension][1]).concat(suffix))
}
var getSize = function(element, dimension, box) {
    var offsetFieldName = "width" === dimension ? "offsetWidth" : "offsetHeight";
    var styles = getElementComputedStyle(element);
    var result = getCSSProperty(element, styles, dimension);
    if ("" === result || "auto" === result) {
        result = element[offsetFieldName]
    }
    result = parseFloat(result) || 0;
    var currentBox = getCSSProperty(element, styles, "boxSizing", "content-box");
    var targetBox = box || currentBox;
    var targetBoxIndex = boxIndices[targetBox];
    var currentBoxIndex = boxIndices[currentBox];
    if (void 0 === targetBoxIndex || void 0 === currentBoxIndex) {
        throw new Error
    }
    if (currentBoxIndex === targetBoxIndex) {
        return result
    }
    var coeff = Math.sign(targetBoxIndex - currentBoxIndex);
    var padding = false;
    var border = false;
    var margin = false;
    var scrollThickness = false;
    if (1 === coeff) {
        targetBoxIndex += 1;
        currentBoxIndex += 1
    }
    for (var boxPart = currentBoxIndex; boxPart !== targetBoxIndex; boxPart += coeff) {
        switch (boxPart) {
            case boxIndices.content:
                break;
            case boxIndices.padding:
                padding = coeff * getComponentThickness(element, dimension, "padding", styles);
                break;
            case boxIndices.border:
                border = coeff * getComponentThickness(element, dimension, "border", styles);
                break;
            case boxIndices.margin:
                margin = coeff * getComponentThickness(element, dimension, "margin", styles)
        }
    }
    if (padding || border) {
        var paddingAndBorder = (false === padding ? coeff * getComponentThickness(element, dimension, "padding", styles) : padding) + (false === border ? coeff * getComponentThickness(element, dimension, "border", styles) : border);
        scrollThickness = coeff * Math.max(0, Math.floor(element[offsetFieldName] - result - coeff * paddingAndBorder)) || 0
    }
    return result + margin + padding + border + scrollThickness
};
exports.getSize = getSize;
var getContainerHeight = function(container) {
    return (0, _type.isWindow)(container) ? container.innerHeight : container.offsetHeight
};
var parseHeight = function(value, container) {
    if (value.indexOf("px") > 0) {
        value = parseInt(value.replace("px", ""))
    } else if (value.indexOf("%") > 0) {
        value = parseInt(value.replace("%", "")) * getContainerHeight(container) / 100
    } else if (!isNaN(value)) {
        value = parseInt(value)
    } else if (value.indexOf("vh") > 0) {
        value = window.innerHeight / 100 * parseInt(value.replace("vh", ""))
    }
    return value
};
exports.parseHeight = parseHeight;
var getHeightWithOffset = function(value, offset, container) {
    if (!value) {
        return null
    }
    if (SPECIAL_HEIGHT_VALUES.indexOf(value) > -1) {
        return offset ? null : value
    }
    if ((0, _type.isString)(value)) {
        value = parseHeight(value, container)
    }
    if ((0, _type.isNumeric)(value)) {
        return Math.max(0, value + offset)
    }
    var operationString = offset < 0 ? " - " : " ";
    return "calc(" + value + operationString + Math.abs(offset) + "px)"
};
var addOffsetToMaxHeight = function(value, offset, container) {
    var maxHeight = getHeightWithOffset(value, offset, container);
    return null !== maxHeight ? maxHeight : "none"
};
exports.addOffsetToMaxHeight = addOffsetToMaxHeight;
var addOffsetToMinHeight = function(value, offset, container) {
    var minHeight = getHeightWithOffset(value, offset, container);
    return null !== minHeight ? minHeight : 0
};
exports.addOffsetToMinHeight = addOffsetToMinHeight;
var getVerticalOffsets = function(element, withMargins) {
    if (!element) {
        return 0
    }
    var boxParams = getElementBoxParams("height", window.getComputedStyle(element));
    return boxParams.padding + boxParams.border + (withMargins ? boxParams.margin : 0)
};
exports.getVerticalOffsets = getVerticalOffsets;
var getVisibleHeight = function(element) {
    if (element) {
        var boundingClientRect = element.getBoundingClientRect();
        if (boundingClientRect.height) {
            return boundingClientRect.height
        }
    }
    return 0
};
exports.getVisibleHeight = getVisibleHeight;
var implementationsMap = {
    getWidth: function() {
        for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
            args[_key] = arguments[_key]
        }
        return elementSizeHelper.apply(void 0, ["width"].concat(args))
    },
    setWidth: function() {
        for (var _len2 = arguments.length, args = new Array(_len2), _key2 = 0; _key2 < _len2; _key2++) {
            args[_key2] = arguments[_key2]
        }
        return elementSizeHelper.apply(void 0, ["width"].concat(args))
    },
    getHeight: function() {
        for (var _len3 = arguments.length, args = new Array(_len3), _key3 = 0; _key3 < _len3; _key3++) {
            args[_key3] = arguments[_key3]
        }
        return elementSizeHelper.apply(void 0, ["height"].concat(args))
    },
    setHeight: function() {
        for (var _len4 = arguments.length, args = new Array(_len4), _key4 = 0; _key4 < _len4; _key4++) {
            args[_key4] = arguments[_key4]
        }
        return elementSizeHelper.apply(void 0, ["height"].concat(args))
    },
    getOuterWidth: function() {
        for (var _len5 = arguments.length, args = new Array(_len5), _key5 = 0; _key5 < _len5; _key5++) {
            args[_key5] = arguments[_key5]
        }
        return elementSizeHelper.apply(void 0, ["outerWidth"].concat(args))
    },
    setOuterWidth: function() {
        for (var _len6 = arguments.length, args = new Array(_len6), _key6 = 0; _key6 < _len6; _key6++) {
            args[_key6] = arguments[_key6]
        }
        return elementSizeHelper.apply(void 0, ["outerWidth"].concat(args))
    },
    getOuterHeight: function() {
        for (var _len7 = arguments.length, args = new Array(_len7), _key7 = 0; _key7 < _len7; _key7++) {
            args[_key7] = arguments[_key7]
        }
        return elementSizeHelper.apply(void 0, ["outerHeight"].concat(args))
    },
    setOuterHeight: function() {
        for (var _len8 = arguments.length, args = new Array(_len8), _key8 = 0; _key8 < _len8; _key8++) {
            args[_key8] = arguments[_key8]
        }
        return elementSizeHelper.apply(void 0, ["outerHeight"].concat(args))
    },
    getInnerWidth: function() {
        for (var _len9 = arguments.length, args = new Array(_len9), _key9 = 0; _key9 < _len9; _key9++) {
            args[_key9] = arguments[_key9]
        }
        return elementSizeHelper.apply(void 0, ["innerWidth"].concat(args))
    },
    setInnerWidth: function() {
        for (var _len10 = arguments.length, args = new Array(_len10), _key10 = 0; _key10 < _len10; _key10++) {
            args[_key10] = arguments[_key10]
        }
        return elementSizeHelper.apply(void 0, ["innerWidth"].concat(args))
    },
    getInnerHeight: function() {
        for (var _len11 = arguments.length, args = new Array(_len11), _key11 = 0; _key11 < _len11; _key11++) {
            args[_key11] = arguments[_key11]
        }
        return elementSizeHelper.apply(void 0, ["innerHeight"].concat(args))
    },
    setInnerHeight: function() {
        for (var _len12 = arguments.length, args = new Array(_len12), _key12 = 0; _key12 < _len12; _key12++) {
            args[_key12] = arguments[_key12]
        }
        return elementSizeHelper.apply(void 0, ["innerHeight"].concat(args))
    }
};
exports.implementationsMap = implementationsMap;

function elementSizeHelper(sizeProperty, el, value) {
    return 2 === arguments.length ? elementSize(el, sizeProperty) : elementSize(el, sizeProperty, value)
}
var getWidth = function(el) {
    return implementationsMap.getWidth(el)
};
exports.getWidth = getWidth;
var setWidth = function(el, value) {
    return implementationsMap.setWidth(el, value)
};
exports.setWidth = setWidth;
var getHeight = function(el) {
    return implementationsMap.getHeight(el)
};
exports.getHeight = getHeight;
var setHeight = function(el, value) {
    return implementationsMap.setHeight(el, value)
};
exports.setHeight = setHeight;
var getOuterWidth = function(el, includeMargin) {
    return implementationsMap.getOuterWidth(el, includeMargin || false)
};
exports.getOuterWidth = getOuterWidth;
var setOuterWidth = function(el, value) {
    return implementationsMap.setOuterWidth(el, value)
};
exports.setOuterWidth = setOuterWidth;
var getOuterHeight = function(el, includeMargin) {
    return implementationsMap.getOuterHeight(el, includeMargin || false)
};
exports.getOuterHeight = getOuterHeight;
var setOuterHeight = function(el, value) {
    return implementationsMap.setOuterHeight(el, value)
};
exports.setOuterHeight = setOuterHeight;
var getInnerWidth = function(el) {
    return implementationsMap.getInnerWidth(el)
};
exports.getInnerWidth = getInnerWidth;
var setInnerWidth = function(el, value) {
    return implementationsMap.setInnerWidth(el, value)
};
exports.setInnerWidth = setInnerWidth;
var getInnerHeight = function(el) {
    return implementationsMap.getInnerHeight(el)
};
exports.getInnerHeight = getInnerHeight;
var setInnerHeight = function(el, value) {
    return implementationsMap.setInnerHeight(el, value)
};
exports.setInnerHeight = setInnerHeight;
var elementSize = function elementSize(el, sizeProperty, value) {
    var partialName = sizeProperty.toLowerCase().indexOf("width") >= 0 ? "Width" : "Height";
    var propName = partialName.toLowerCase();
    var isOuter = 0 === sizeProperty.indexOf("outer");
    var isInner = 0 === sizeProperty.indexOf("inner");
    var isGetter = 2 === arguments.length || "boolean" === typeof value;
    if ((0, _type.isRenderer)(el)) {
        if (el.length > 1 && !isGetter) {
            for (var i = 0; i < el.length; i++) {
                elementSize(el[i], sizeProperty, value)
            }
            return
        }
        el = el[0]
    }
    if (!el) {
        return
    }
    if ((0, _type.isWindow)(el)) {
        return isOuter ? el["inner" + partialName] : _dom_adapter.default.getDocumentElement()["client" + partialName]
    }
    if (_dom_adapter.default.isDocument(el)) {
        var documentElement = _dom_adapter.default.getDocumentElement();
        var body = _dom_adapter.default.getBody();
        return Math.max(body["scroll" + partialName], body["offset" + partialName], documentElement["scroll" + partialName], documentElement["offset" + partialName], documentElement["client" + partialName])
    }
    if (isGetter) {
        var box = "content";
        if (isOuter) {
            box = value ? "margin" : "border"
        }
        if (isInner) {
            box = "padding"
        }
        return getSize(el, propName, box)
    }
    if ((0, _type.isNumeric)(value)) {
        var elementStyles = getElementComputedStyle(el);
        var sizeAdjustment = getElementBoxParams(propName, elementStyles);
        var isBorderBox = "border-box" === elementStyles.boxSizing;
        value = Number(value);
        if (isOuter) {
            value -= isBorderBox ? 0 : sizeAdjustment.border + sizeAdjustment.padding
        } else if (isInner) {
            value += isBorderBox ? sizeAdjustment.border : -sizeAdjustment.padding
        } else if (isBorderBox) {
            value += sizeAdjustment.border + sizeAdjustment.padding
        }
    }
    value += (0, _type.isNumeric)(value) ? "px" : "";
    _dom_adapter.default.setStyle(el, propName, value);
    return null
};
var getWindowByElement = function(el) {
    return (0, _type.isWindow)(el) ? el : el.defaultView
};
exports.getWindowByElement = getWindowByElement;
var getOffset = function(el) {
    if (!el.getClientRects().length) {
        return {
            top: 0,
            left: 0
        }
    }
    var rect = el.getBoundingClientRect();
    var win = getWindowByElement(el.ownerDocument);
    var docElem = el.ownerDocument.documentElement;
    return {
        top: rect.top + win.pageYOffset - docElem.clientTop,
        left: rect.left + win.pageXOffset - docElem.clientLeft
    }
};
exports.getOffset = getOffset;
