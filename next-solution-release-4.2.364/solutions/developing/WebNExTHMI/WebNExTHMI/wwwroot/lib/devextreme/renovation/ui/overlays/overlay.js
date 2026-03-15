/**
 * DevExtreme (renovation/ui/overlays/overlay.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.OverlayProps = exports.Overlay = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _ui = _interopRequireDefault(require("../../../ui/overlay/ui.overlay"));
var _dom_component_wrapper = require("../common/dom_component_wrapper");
var _base_props = require("../common/base_props");
var _excluded = ["_checkParentVisibility", "accessKey", "activeStateEnabled", "animation", "className", "closeOnOutsideClick", "container", "contentTemplate", "disabled", "focusStateEnabled", "height", "hideOnParentScroll", "hint", "hoverStateEnabled", "integrationOptions", "maxWidth", "onClick", "onKeyDown", "position", "propagateOutsideClick", "rtlEnabled", "shading", "tabIndex", "templatesRenderAsynchronously", "visible", "width"];

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
    var props = _ref.props,
        restAttributes = _ref.restAttributes;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _dom_component_wrapper.DomComponentWrapper, _extends({
        componentType: _ui.default,
        componentProps: props,
        templateNames: []
    }, restAttributes)))
};
exports.viewFunction = viewFunction;
var OverlayProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_base_props.BaseWidgetProps), Object.getOwnPropertyDescriptors(Object.defineProperties({
    templatesRenderAsynchronously: false,
    shading: true,
    closeOnOutsideClick: false,
    hideOnParentScroll: false,
    visible: false,
    propagateOutsideClick: true,
    _checkParentVisibility: false,
    rtlEnabled: false,
    contentTemplate: "content",
    maxWidth: null,
    isReactComponentWrapper: true
}, {
    integrationOptions: {
        get: function() {
            return {}
        },
        configurable: true,
        enumerable: true
    },
    animation: {
        get: function() {
            return {
                type: "pop",
                duration: 300,
                to: {
                    opacity: 0,
                    scale: .55
                },
                from: {
                    opacity: 1,
                    scale: 1
                }
            }
        },
        configurable: true,
        enumerable: true
    }
}))));
exports.OverlayProps = OverlayProps;
var Overlay = function(_BaseInfernoComponent) {
    _inheritsLoose(Overlay, _BaseInfernoComponent);

    function Overlay(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = Overlay.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            restAttributes: this.restAttributes
        })
    };
    _createClass(Overlay, [{
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props._checkParentVisibility, _this$props.accessKey, _this$props.activeStateEnabled, _this$props.animation, _this$props.className, _this$props.closeOnOutsideClick, _this$props.container, _this$props.contentTemplate, _this$props.disabled, _this$props.focusStateEnabled, _this$props.height, _this$props.hideOnParentScroll, _this$props.hint, _this$props.hoverStateEnabled, _this$props.integrationOptions, _this$props.maxWidth, _this$props.onClick, _this$props.onKeyDown, _this$props.position, _this$props.propagateOutsideClick, _this$props.rtlEnabled, _this$props.shading, _this$props.tabIndex, _this$props.templatesRenderAsynchronously, _this$props.visible, _this$props.width, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return Overlay
}(_inferno2.BaseInfernoComponent);
exports.Overlay = Overlay;
Overlay.defaultProps = OverlayProps;
