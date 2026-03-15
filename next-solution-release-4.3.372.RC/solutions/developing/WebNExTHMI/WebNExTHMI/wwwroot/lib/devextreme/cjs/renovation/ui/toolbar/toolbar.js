/**
 * DevExtreme (cjs/renovation/ui/toolbar/toolbar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.Toolbar = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _toolbar = _interopRequireDefault(require("../../../ui/toolbar"));
var _dom_component_wrapper = require("../common/dom_component_wrapper");
var _toolbar_props = require("./toolbar_props");
var _type = require("../../../core/utils/type");
var _config_context = require("../../common/config_context");
var _resolve_rtl = require("../../utils/resolve_rtl");
var _excluded = ["accessKey", "activeStateEnabled", "className", "disabled", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "items", "onClick", "onKeyDown", "rtlEnabled", "tabIndex", "visible", "width"];

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
    var componentProps = _ref.componentProps,
        restAttributes = _ref.restAttributes;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _dom_component_wrapper.DomComponentWrapper, _extends({
        componentType: _toolbar.default,
        componentProps: componentProps,
        templateNames: []
    }, restAttributes)))
};
exports.viewFunction = viewFunction;
var Toolbar = function(_BaseInfernoComponent) {
    _inheritsLoose(Toolbar, _BaseInfernoComponent);

    function Toolbar(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.__getterCache = {};
        return _this
    }
    var _proto = Toolbar.prototype;
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        if (this.props.items !== nextProps.items || this.props.rtlEnabled !== nextProps.rtlEnabled || this.context.ConfigContext !== context.ConfigContext || this.props !== nextProps) {
            this.__getterCache.componentProps = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            config: this.config,
            componentProps: this.componentProps,
            resolvedRtlEnabled: this.resolvedRtlEnabled,
            restAttributes: this.restAttributes
        })
    };
    _createClass(Toolbar, [{
        key: "config",
        get: function() {
            if ("ConfigContext" in this.context) {
                return this.context.ConfigContext
            }
            return _config_context.ConfigContext
        }
    }, {
        key: "componentProps",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.componentProps) {
                return this.__getterCache.componentProps
            }
            return this.__getterCache.componentProps = (items = _this2.props.items, toolbarItems = null === items || void 0 === items ? void 0 : items.map((function(item) {
                var _item$options, _options$rtlEnabled;
                if (!(0, _type.isObject)(item)) {
                    return item
                }
                var options = null !== (_item$options = item.options) && void 0 !== _item$options ? _item$options : {};
                options.rtlEnabled = null !== (_options$rtlEnabled = options.rtlEnabled) && void 0 !== _options$rtlEnabled ? _options$rtlEnabled : _this2.resolvedRtlEnabled;
                return _extends({}, item, {
                    options: options
                })
            })), _extends({}, _this2.props, {
                items: toolbarItems
            }));
            var items, toolbarItems
        }
    }, {
        key: "resolvedRtlEnabled",
        get: function() {
            var rtlEnabled = this.props.rtlEnabled;
            return !!(0, _resolve_rtl.resolveRtlEnabled)(rtlEnabled, this.config)
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.accessKey, _this$props.activeStateEnabled, _this$props.className, _this$props.disabled, _this$props.focusStateEnabled, _this$props.height, _this$props.hint, _this$props.hoverStateEnabled, _this$props.items, _this$props.onClick, _this$props.onKeyDown, _this$props.rtlEnabled, _this$props.tabIndex, _this$props.visible, _this$props.width, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return Toolbar
}(_inferno2.BaseInfernoComponent);
exports.Toolbar = Toolbar;
Toolbar.defaultProps = _toolbar_props.ToolbarProps;
