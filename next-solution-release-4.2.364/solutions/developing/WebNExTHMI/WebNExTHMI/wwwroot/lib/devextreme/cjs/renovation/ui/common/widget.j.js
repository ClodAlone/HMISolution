/**
 * DevExtreme (cjs/renovation/ui/common/widget.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _component_registrator = _interopRequireDefault(require("../../../core/component_registrator"));
var _component = _interopRequireDefault(require("../../component_wrapper/common/component"));
var _widget = require("./widget");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
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
var Widget = function(_BaseComponent) {
    _inheritsLoose(Widget, _BaseComponent);

    function Widget() {
        return _BaseComponent.apply(this, arguments) || this
    }
    var _proto = Widget.prototype;
    _proto.getProps = function() {
        var props = _BaseComponent.prototype.getProps.call(this);
        props.onKeyDown = this._wrapKeyDownHandler(props.onKeyDown);
        return props
    };
    _proto.focus = function() {
        var _this$viewRef;
        return null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.focus.apply(_this$viewRef, arguments)
    };
    _proto.blur = function() {
        var _this$viewRef2;
        return null === (_this$viewRef2 = this.viewRef) || void 0 === _this$viewRef2 ? void 0 : _this$viewRef2.blur.apply(_this$viewRef2, arguments)
    };
    _proto.activate = function() {
        var _this$viewRef3;
        return null === (_this$viewRef3 = this.viewRef) || void 0 === _this$viewRef3 ? void 0 : _this$viewRef3.activate.apply(_this$viewRef3, arguments)
    };
    _proto.deactivate = function() {
        var _this$viewRef4;
        return null === (_this$viewRef4 = this.viewRef) || void 0 === _this$viewRef4 ? void 0 : _this$viewRef4.deactivate.apply(_this$viewRef4, arguments)
    };
    _proto._getActionConfigs = function() {
        return {
            onActive: {},
            onDimensionChanged: {},
            onInactive: {},
            onVisibilityChange: {},
            onFocusIn: {},
            onFocusOut: {},
            onHoverStart: {},
            onHoverEnd: {},
            onRootElementRendered: {},
            onClick: {}
        }
    };
    _createClass(Widget, [{
        key: "_propsInfo",
        get: function() {
            return {
                twoWay: [],
                allowNull: [],
                elements: [],
                templates: [],
                props: ["_feedbackHideTimeout", "_feedbackShowTimeout", "activeStateUnit", "cssText", "aria", "classes", "name", "addWidgetClass", "onActive", "onDimensionChanged", "onInactive", "onVisibilityChange", "onFocusIn", "onFocusOut", "onHoverStart", "onHoverEnd", "onRootElementRendered", "className", "accessKey", "activeStateEnabled", "disabled", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "onClick", "onKeyDown", "rtlEnabled", "tabIndex", "visible", "width"]
            }
        }
    }, {
        key: "_viewComponent",
        get: function() {
            return _widget.Widget
        }
    }]);
    return Widget
}(_component.default);
exports.default = Widget;
(0, _component_registrator.default)("dxWidget", Widget);
module.exports = exports.default;
module.exports.default = exports.default;
