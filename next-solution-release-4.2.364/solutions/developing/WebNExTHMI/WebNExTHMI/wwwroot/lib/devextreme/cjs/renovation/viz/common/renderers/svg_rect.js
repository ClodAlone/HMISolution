/**
 * DevExtreme (cjs/renovation/viz/common/renderers/svg_rect.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.RectSvgElementProps = exports.RectSvgElement = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _base_graphics_props = _interopRequireDefault(require("./base_graphics_props"));
var _utils = require("./utils");
var _excluded = ["className", "dashStyle", "fill", "height", "opacity", "rotate", "rotateX", "rotateY", "rx", "ry", "scaleX", "scaleY", "sharp", "sharpDirection", "stroke", "strokeOpacity", "strokeWidth", "translateX", "translateY", "width", "x", "y"];

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _objectWithoutProperties(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = _objectWithoutPropertiesLoose(source, excluded);
    var key, i;
    if (Object.getOwnPropertySymbols) {
        var sourceSymbolKeys = Object.getOwnPropertySymbols(source);
        for (i = 0; i < sourceSymbolKeys.length; i++) {
            key = sourceSymbolKeys[i];
            if (excluded.indexOf(key) >= 0) {
                continue
            }
            if (!Object.prototype.propertyIsEnumerable.call(source, key)) {
                continue
            }
            target[key] = source[key]
        }
    }
    return target
}

function _objectWithoutPropertiesLoose(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = {};
    var sourceKeys = Object.keys(source);
    var key, i;
    for (i = 0; i < sourceKeys.length; i++) {
        key = sourceKeys[i];
        if (excluded.indexOf(key) >= 0) {
            continue
        }
        target[key] = source[key]
    }
    return target
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}

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
var viewFunction = function(_ref) {
    var parsedProps = _ref.parsedProps,
        rectRef = _ref.rectRef;
    var fill = parsedProps.fill,
        height = parsedProps.height,
        opacity = parsedProps.opacity,
        rx = parsedProps.rx,
        ry = parsedProps.ry,
        stroke = parsedProps.stroke,
        strokeOpacity = parsedProps.strokeOpacity,
        strokeWidth = parsedProps.strokeWidth,
        width = parsedProps.width,
        x = parsedProps.x,
        y = parsedProps.y;
    return (0, _inferno.normalizeProps)((0, _inferno.createVNode)(32, "rect", null, null, 1, _extends({
        x: x,
        y: y,
        width: width,
        height: height,
        rx: rx,
        ry: ry,
        fill: fill,
        stroke: stroke,
        "stroke-width": strokeWidth,
        "stroke-opacity": strokeOpacity,
        opacity: opacity
    }, (0, _utils.getGraphicExtraProps)(parsedProps, x, y)), null, rectRef))
};
exports.viewFunction = viewFunction;
var RectSvgElementProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_base_graphics_props.default), Object.getOwnPropertyDescriptors({
    x: 0,
    y: 0,
    width: 0,
    height: 0
})));
exports.RectSvgElementProps = RectSvgElementProps;
var RectSvgElement = function(_BaseInfernoComponent) {
    _inheritsLoose(RectSvgElement, _BaseInfernoComponent);

    function RectSvgElement(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.rectRef = (0, _inferno.createRef)();
        return _this
    }
    var _proto = RectSvgElement.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            rectRef: this.rectRef,
            parsedProps: this.parsedProps,
            restAttributes: this.restAttributes
        })
    };
    _createClass(RectSvgElement, [{
        key: "parsedProps",
        get: function() {
            var tmpX = Number.NaN;
            var tmpY = Number.NaN;
            var tmpWidth = Number.NaN;
            var tmpHeight = Number.NaN;
            var tmpProps = _extends({}, this.props);
            var height = tmpProps.height,
                strokeWidth = tmpProps.strokeWidth,
                width = tmpProps.width,
                x = tmpProps.x,
                y = tmpProps.y;
            if (void 0 !== x || void 0 !== y || void 0 !== width || void 0 !== height || void 0 !== strokeWidth) {
                tmpX = void 0 !== x ? x : 0;
                tmpY = void 0 !== y ? y : 0;
                tmpWidth = void 0 !== width ? width : 0;
                tmpHeight = void 0 !== height ? height : 0;
                var sw = void 0 !== strokeWidth ? strokeWidth : 0;
                var maxSW = ~~((tmpWidth < tmpHeight ? tmpWidth : tmpHeight) / 2);
                var newSW = Math.min(sw, maxSW);
                tmpProps.x = tmpX + newSW / 2;
                tmpProps.y = tmpY + newSW / 2;
                tmpProps.width = tmpWidth - newSW;
                tmpProps.height = tmpHeight - newSW;
                (sw !== newSW || !(0 === newSW && void 0 === strokeWidth)) && (tmpProps.strokeWidth = newSW)
            }
            tmpProps.sharp && (tmpProps.sharp = false);
            return tmpProps
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.className, _this$props.dashStyle, _this$props.fill, _this$props.height, _this$props.opacity, _this$props.rotate, _this$props.rotateX, _this$props.rotateY, _this$props.rx, _this$props.ry, _this$props.scaleX, _this$props.scaleY, _this$props.sharp, _this$props.sharpDirection, _this$props.stroke, _this$props.strokeOpacity, _this$props.strokeWidth, _this$props.translateX, _this$props.translateY, _this$props.width, _this$props.x, _this$props.y, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return RectSvgElement
}(_inferno2.BaseInfernoComponent);
exports.RectSvgElement = RectSvgElement;
RectSvgElement.defaultProps = RectSvgElementProps;
