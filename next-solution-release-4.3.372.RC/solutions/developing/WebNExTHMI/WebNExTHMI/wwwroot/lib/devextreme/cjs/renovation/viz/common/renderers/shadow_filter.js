/**
 * DevExtreme (cjs/renovation/viz/common/renderers/shadow_filter.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.ShadowFilterProps = exports.ShadowFilter = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _excluded = ["blur", "color", "height", "id", "offsetX", "offsetY", "opacity", "width", "x", "y"];

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
var viewFunction = function(_ref) {
    var _ref$props = _ref.props,
        blur = _ref$props.blur,
        color = _ref$props.color,
        height = _ref$props.height,
        id = _ref$props.id,
        offsetX = _ref$props.offsetX,
        offsetY = _ref$props.offsetY,
        opacity = _ref$props.opacity,
        width = _ref$props.width,
        x = _ref$props.x,
        y = _ref$props.y;
    return (0, _inferno.createVNode)(32, "filter", null, [(0, _inferno.createVNode)(32, "feGaussianBlur", null, null, 1, {
        in: "SourceGraphic",
        result: "gaussianBlurResult",
        stdDeviation: blur
    }), (0, _inferno.createVNode)(32, "feOffset", null, null, 1, {
        in: "gaussianBlurResult",
        result: "offsetResult",
        dx: offsetX,
        dy: offsetY
    }), (0, _inferno.createVNode)(32, "feFlood", null, null, 1, {
        result: "floodResult",
        "flood-color": color,
        "flood-opacity": opacity
    }), (0, _inferno.createVNode)(32, "feComposite", null, null, 1, {
        in: "floodResult",
        in2: "offsetResult",
        operator: "in",
        result: "compositeResult"
    }), (0, _inferno.createVNode)(32, "feComposite", null, null, 1, {
        in: "SourceGraphic",
        in2: "compositeResult",
        operator: "over"
    })], 4, {
        id: id,
        x: x,
        y: y,
        width: width,
        height: height
    })
};
exports.viewFunction = viewFunction;
var ShadowFilterProps = {
    x: 0,
    y: 0,
    width: 0,
    height: 0,
    offsetX: 0,
    offsetY: 0,
    blur: 0,
    color: ""
};
exports.ShadowFilterProps = ShadowFilterProps;
var ShadowFilter = function(_BaseInfernoComponent) {
    _inheritsLoose(ShadowFilter, _BaseInfernoComponent);

    function ShadowFilter(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = ShadowFilter.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            restAttributes: this.restAttributes
        })
    };
    _createClass(ShadowFilter, [{
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.blur, _this$props.color, _this$props.height, _this$props.id, _this$props.offsetX, _this$props.offsetY, _this$props.opacity, _this$props.width, _this$props.x, _this$props.y, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return ShadowFilter
}(_inferno2.BaseInfernoComponent);
exports.ShadowFilter = ShadowFilter;
ShadowFilter.defaultProps = ShadowFilterProps;
