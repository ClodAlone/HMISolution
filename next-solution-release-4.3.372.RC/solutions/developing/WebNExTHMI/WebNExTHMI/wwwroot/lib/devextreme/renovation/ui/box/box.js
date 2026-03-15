/**
 * DevExtreme (renovation/ui/box/box.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.Box = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _widget = require("../common/widget");
var _box_props = require("./box_props");
var _combine_classes = require("../../utils/combine_classes");
var _excluded = ["align", "crossAlign", "direction"];

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
var viewFunction = function(viewModel) {
    return (0, _inferno.createComponentVNode)(2, _widget.Widget, {
        classes: viewModel.cssClasses,
        style: (0, _inferno2.normalizeStyles)(viewModel.cssStyles)
    })
};
exports.viewFunction = viewFunction;
var Box = function(_InfernoWrapperCompon) {
    _inheritsLoose(Box, _InfernoWrapperCompon);

    function Box(props) {
        var _this;
        _this = _InfernoWrapperCompon.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = Box.prototype;
    _proto.createEffects = function() {
        return [(0, _inferno2.createReRenderEffect)()]
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            cssClasses: this.cssClasses,
            cssStyles: this.cssStyles,
            restAttributes: this.restAttributes
        })
    };
    _createClass(Box, [{
        key: "cssClasses",
        get: function() {
            return (0, _combine_classes.combineClasses)({
                "dx-box dx-box-flex": true
            })
        }
    }, {
        key: "cssStyles",
        get: function() {
            var tryGetFromMap = function(prop, map) {
                return prop in map ? map[prop] : prop
            };
            return {
                display: "flex",
                flexDirection: {
                    row: "row",
                    col: "column"
                } [this.props.direction],
                justifyContent: tryGetFromMap(this.props.align, {
                    start: "flex-start",
                    end: "flex-end",
                    center: "center",
                    "space-between": "space-between",
                    "space-around": "space-around"
                }),
                alignItems: tryGetFromMap(this.props.crossAlign, {
                    start: "flex-start",
                    end: "flex-end",
                    center: "center",
                    stretch: "stretch"
                })
            }
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.align, _this$props.crossAlign, _this$props.direction, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return Box
}(_inferno2.InfernoWrapperComponent);
exports.Box = Box;
Box.defaultProps = _box_props.BoxProps;
