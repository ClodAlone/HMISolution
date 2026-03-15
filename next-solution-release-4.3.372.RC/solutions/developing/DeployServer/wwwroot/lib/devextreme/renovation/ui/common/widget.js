/**
 * DevExtreme (renovation/ui/common/widget.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.WidgetProps = exports.Widget = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
require("../../../events/click");
require("../../../events/hover");
var _type = require("../../../core/utils/type");
var _short = require("../../../events/short");
var _subscribe_to_event = require("../../utils/subscribe_to_event");
var _combine_classes = require("../../utils/combine_classes");
var _extend = require("../../../core/utils/extend");
var _style = require("../../../core/utils/style");
var _base_props = require("./base_props");
var _config_context = require("../../common/config_context");
var _config_provider = require("../../common/config_provider");
var _resolve_rtl = require("../../utils/resolve_rtl");
var _resize_callbacks = _interopRequireDefault(require("../../../core/utils/resize_callbacks"));
var _errors = _interopRequireDefault(require("../../../core/errors"));
var _dom_adapter = _interopRequireDefault(require("../../../core/dom_adapter"));
var _excluded = ["_feedbackHideTimeout", "_feedbackShowTimeout", "accessKey", "activeStateEnabled", "activeStateUnit", "addWidgetClass", "aria", "children", "className", "classes", "cssText", "disabled", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "name", "onActive", "onClick", "onDimensionChanged", "onFocusIn", "onFocusOut", "onHoverEnd", "onHoverStart", "onInactive", "onKeyDown", "onRootElementRendered", "onVisibilityChange", "rootElementRef", "rtlEnabled", "tabIndex", "visible", "width"];

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

function _assertThisInitialized(self) {
    if (void 0 === self) {
        throw new ReferenceError("this hasn't been initialised - super() hasn't been called")
    }
    return self
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

function _defineProperty(obj, key, value) {
    if (key in obj) {
        Object.defineProperty(obj, key, {
            value: value,
            enumerable: true,
            configurable: true,
            writable: true
        })
    } else {
        obj[key] = value
    }
    return obj
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
var DEFAULT_FEEDBACK_HIDE_TIMEOUT = 400;
var DEFAULT_FEEDBACK_SHOW_TIMEOUT = 30;
var getAria = function(args) {
    return Object.keys(args).reduce((function(r, key) {
        if (args[key]) {
            return _extends({}, r, _defineProperty({}, "role" === key || "id" === key ? key : "aria-".concat(key), String(args[key])))
        }
        return r
    }), {})
};
var viewFunction = function(viewModel) {
    var widget = (0, _inferno.normalizeProps)((0, _inferno.createVNode)(1, "div", viewModel.cssClasses, viewModel.props.children, 0, _extends({}, viewModel.attributes, {
        tabIndex: viewModel.tabIndex,
        title: viewModel.props.hint,
        hidden: !viewModel.props.visible,
        style: (0, _inferno2.normalizeStyles)(viewModel.styles)
    }), null, viewModel.widgetElementRef));
    return viewModel.shouldRenderConfigProvider ? (0, _inferno.createComponentVNode)(2, _config_provider.ConfigProvider, {
        rtlEnabled: viewModel.rtlEnabled,
        children: widget
    }) : widget
};
exports.viewFunction = viewFunction;
var WidgetProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_base_props.BaseWidgetProps), Object.getOwnPropertyDescriptors(Object.defineProperties({
    _feedbackHideTimeout: DEFAULT_FEEDBACK_HIDE_TIMEOUT,
    _feedbackShowTimeout: DEFAULT_FEEDBACK_SHOW_TIMEOUT,
    cssText: "",
    classes: "",
    name: "",
    addWidgetClass: true
}, {
    aria: {
        get: function() {
            return {}
        },
        configurable: true,
        enumerable: true
    }
}))));
exports.WidgetProps = WidgetProps;
var Widget = function(_InfernoWrapperCompon) {
    _inheritsLoose(Widget, _InfernoWrapperCompon);

    function Widget(props) {
        var _this;
        _this = _InfernoWrapperCompon.call(this, props) || this;
        _this.widgetElementRef = (0, _inferno.createRef)();
        _this.state = {
            active: false,
            focused: false,
            hovered: false
        };
        _this.setRootElementRef = _this.setRootElementRef.bind(_assertThisInitialized(_this));
        _this.activeEffect = _this.activeEffect.bind(_assertThisInitialized(_this));
        _this.inactiveEffect = _this.inactiveEffect.bind(_assertThisInitialized(_this));
        _this.clickEffect = _this.clickEffect.bind(_assertThisInitialized(_this));
        _this.focus = _this.focus.bind(_assertThisInitialized(_this));
        _this.blur = _this.blur.bind(_assertThisInitialized(_this));
        _this.activate = _this.activate.bind(_assertThisInitialized(_this));
        _this.deactivate = _this.deactivate.bind(_assertThisInitialized(_this));
        _this.focusInEffect = _this.focusInEffect.bind(_assertThisInitialized(_this));
        _this.focusOutEffect = _this.focusOutEffect.bind(_assertThisInitialized(_this));
        _this.hoverStartEffect = _this.hoverStartEffect.bind(_assertThisInitialized(_this));
        _this.hoverEndEffect = _this.hoverEndEffect.bind(_assertThisInitialized(_this));
        _this.keyboardEffect = _this.keyboardEffect.bind(_assertThisInitialized(_this));
        _this.resizeEffect = _this.resizeEffect.bind(_assertThisInitialized(_this));
        _this.windowResizeEffect = _this.windowResizeEffect.bind(_assertThisInitialized(_this));
        _this.visibilityEffect = _this.visibilityEffect.bind(_assertThisInitialized(_this));
        _this.checkDeprecation = _this.checkDeprecation.bind(_assertThisInitialized(_this));
        _this.applyCssTextEffect = _this.applyCssTextEffect.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = Widget.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.setRootElementRef, []), new _inferno2.InfernoEffect(this.activeEffect, [this.props._feedbackShowTimeout, this.props.activeStateEnabled, this.props.activeStateUnit, this.props.disabled, this.props.onActive]), new _inferno2.InfernoEffect(this.inactiveEffect, [this.props._feedbackHideTimeout, this.props.activeStateEnabled, this.props.activeStateUnit, this.props.onInactive, this.state.active]), new _inferno2.InfernoEffect(this.clickEffect, [this.props.disabled, this.props.name, this.props.onClick]), new _inferno2.InfernoEffect(this.focusInEffect, [this.props.disabled, this.props.focusStateEnabled, this.props.name, this.props.onFocusIn]), new _inferno2.InfernoEffect(this.focusOutEffect, [this.props.focusStateEnabled, this.props.name, this.props.onFocusOut, this.state.focused]), new _inferno2.InfernoEffect(this.hoverStartEffect, [this.props.activeStateUnit, this.props.disabled, this.props.hoverStateEnabled, this.props.onHoverStart, this.state.active]), new _inferno2.InfernoEffect(this.hoverEndEffect, [this.props.activeStateUnit, this.props.hoverStateEnabled, this.props.onHoverEnd, this.state.hovered]), new _inferno2.InfernoEffect(this.keyboardEffect, [this.props.focusStateEnabled, this.props.onKeyDown]), new _inferno2.InfernoEffect(this.resizeEffect, [this.props.name, this.props.onDimensionChanged]), new _inferno2.InfernoEffect(this.windowResizeEffect, [this.props.onDimensionChanged]), new _inferno2.InfernoEffect(this.visibilityEffect, [this.props.name, this.props.onVisibilityChange]), new _inferno2.InfernoEffect(this.checkDeprecation, [this.props.height, this.props.width]), new _inferno2.InfernoEffect(this.applyCssTextEffect, [this.props.cssText]), (0, _inferno2.createReRenderEffect)()]
    };
    _proto.updateEffects = function() {
        var _this$_effects$, _this$_effects$2, _this$_effects$3, _this$_effects$4, _this$_effects$5, _this$_effects$6, _this$_effects$7, _this$_effects$8, _this$_effects$9, _this$_effects$10, _this$_effects$11, _this$_effects$12, _this$_effects$13;
        null === (_this$_effects$ = this._effects[1]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props._feedbackShowTimeout, this.props.activeStateEnabled, this.props.activeStateUnit, this.props.disabled, this.props.onActive]);
        null === (_this$_effects$2 = this._effects[2]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props._feedbackHideTimeout, this.props.activeStateEnabled, this.props.activeStateUnit, this.props.onInactive, this.state.active]);
        null === (_this$_effects$3 = this._effects[3]) || void 0 === _this$_effects$3 ? void 0 : _this$_effects$3.update([this.props.disabled, this.props.name, this.props.onClick]);
        null === (_this$_effects$4 = this._effects[4]) || void 0 === _this$_effects$4 ? void 0 : _this$_effects$4.update([this.props.disabled, this.props.focusStateEnabled, this.props.name, this.props.onFocusIn]);
        null === (_this$_effects$5 = this._effects[5]) || void 0 === _this$_effects$5 ? void 0 : _this$_effects$5.update([this.props.focusStateEnabled, this.props.name, this.props.onFocusOut, this.state.focused]);
        null === (_this$_effects$6 = this._effects[6]) || void 0 === _this$_effects$6 ? void 0 : _this$_effects$6.update([this.props.activeStateUnit, this.props.disabled, this.props.hoverStateEnabled, this.props.onHoverStart, this.state.active]);
        null === (_this$_effects$7 = this._effects[7]) || void 0 === _this$_effects$7 ? void 0 : _this$_effects$7.update([this.props.activeStateUnit, this.props.hoverStateEnabled, this.props.onHoverEnd, this.state.hovered]);
        null === (_this$_effects$8 = this._effects[8]) || void 0 === _this$_effects$8 ? void 0 : _this$_effects$8.update([this.props.focusStateEnabled, this.props.onKeyDown]);
        null === (_this$_effects$9 = this._effects[9]) || void 0 === _this$_effects$9 ? void 0 : _this$_effects$9.update([this.props.name, this.props.onDimensionChanged]);
        null === (_this$_effects$10 = this._effects[10]) || void 0 === _this$_effects$10 ? void 0 : _this$_effects$10.update([this.props.onDimensionChanged]);
        null === (_this$_effects$11 = this._effects[11]) || void 0 === _this$_effects$11 ? void 0 : _this$_effects$11.update([this.props.name, this.props.onVisibilityChange]);
        null === (_this$_effects$12 = this._effects[12]) || void 0 === _this$_effects$12 ? void 0 : _this$_effects$12.update([this.props.height, this.props.width]);
        null === (_this$_effects$13 = this._effects[13]) || void 0 === _this$_effects$13 ? void 0 : _this$_effects$13.update([this.props.cssText])
    };
    _proto.setRootElementRef = function() {
        var _this$props = this.props,
            onRootElementRendered = _this$props.onRootElementRendered,
            rootElementRef = _this$props.rootElementRef;
        if (rootElementRef) {
            rootElementRef.current = this.widgetElementRef.current
        }
        null === onRootElementRendered || void 0 === onRootElementRendered ? void 0 : onRootElementRendered(this.widgetElementRef.current)
    };
    _proto.activeEffect = function() {
        var _this2 = this;
        var _this$props2 = this.props,
            _feedbackShowTimeout = _this$props2._feedbackShowTimeout,
            activeStateEnabled = _this$props2.activeStateEnabled,
            activeStateUnit = _this$props2.activeStateUnit,
            disabled = _this$props2.disabled,
            onActive = _this$props2.onActive;
        var selector = activeStateUnit;
        if (activeStateEnabled) {
            if (!disabled) {
                return (0, _subscribe_to_event.subscribeToDxActiveEvent)(this.widgetElementRef.current, (function(event) {
                    _this2.setState((function(__state_argument) {
                        return {
                            active: true
                        }
                    }));
                    null === onActive || void 0 === onActive ? void 0 : onActive(event)
                }), {
                    timeout: _feedbackShowTimeout,
                    selector: selector
                }, "UIFeedback")
            }
        }
        return
    };
    _proto.inactiveEffect = function() {
        var _this3 = this;
        var _this$props3 = this.props,
            _feedbackHideTimeout = _this$props3._feedbackHideTimeout,
            activeStateEnabled = _this$props3.activeStateEnabled,
            activeStateUnit = _this$props3.activeStateUnit,
            onInactive = _this$props3.onInactive;
        var selector = activeStateUnit;
        if (activeStateEnabled) {
            return (0, _subscribe_to_event.subscribeToDxInactiveEvent)(this.widgetElementRef.current, (function(event) {
                if (_this3.state.active) {
                    _this3.setState((function(__state_argument) {
                        return {
                            active: false
                        }
                    }));
                    null === onInactive || void 0 === onInactive ? void 0 : onInactive(event)
                }
            }), {
                timeout: _feedbackHideTimeout,
                selector: selector
            }, "UIFeedback")
        }
        return
    };
    _proto.clickEffect = function() {
        var _this4 = this;
        var _this$props4 = this.props,
            disabled = _this$props4.disabled,
            name = _this$props4.name,
            onClick = _this$props4.onClick;
        var namespace = name;
        if (onClick && !disabled) {
            _short.dxClick.on(this.widgetElementRef.current, onClick, {
                namespace: namespace
            });
            return function() {
                return _short.dxClick.off(_this4.widgetElementRef.current, {
                    namespace: namespace
                })
            }
        }
        return
    };
    _proto.focusInEffect = function() {
        var _this5 = this;
        var _this$props5 = this.props,
            disabled = _this$props5.disabled,
            focusStateEnabled = _this$props5.focusStateEnabled,
            name = _this$props5.name,
            onFocusIn = _this$props5.onFocusIn;
        var namespace = "".concat(name, "Focus");
        if (focusStateEnabled) {
            if (!disabled) {
                return (0, _subscribe_to_event.subscribeToDxFocusInEvent)(this.widgetElementRef.current, (function(event) {
                    if (!event.isDefaultPrevented()) {
                        _this5.setState((function(__state_argument) {
                            return {
                                focused: true
                            }
                        }));
                        null === onFocusIn || void 0 === onFocusIn ? void 0 : onFocusIn(event)
                    }
                }), null, namespace)
            }
        }
        return
    };
    _proto.focusOutEffect = function() {
        var _this6 = this;
        var _this$props6 = this.props,
            focusStateEnabled = _this$props6.focusStateEnabled,
            name = _this$props6.name,
            onFocusOut = _this$props6.onFocusOut;
        var namespace = "".concat(name, "Focus");
        if (focusStateEnabled) {
            return (0, _subscribe_to_event.subscribeToDxFocusOutEvent)(this.widgetElementRef.current, (function(event) {
                if (!event.isDefaultPrevented() && _this6.state.focused) {
                    _this6.setState((function(__state_argument) {
                        return {
                            focused: false
                        }
                    }));
                    null === onFocusOut || void 0 === onFocusOut ? void 0 : onFocusOut(event)
                }
            }), null, namespace)
        }
        return
    };
    _proto.hoverStartEffect = function() {
        var _this7 = this;
        var _this$props7 = this.props,
            activeStateUnit = _this$props7.activeStateUnit,
            disabled = _this$props7.disabled,
            hoverStateEnabled = _this$props7.hoverStateEnabled,
            onHoverStart = _this$props7.onHoverStart;
        var selector = activeStateUnit;
        if (hoverStateEnabled) {
            if (!disabled) {
                return (0, _subscribe_to_event.subscribeToDxHoverStartEvent)(this.widgetElementRef.current, (function(event) {
                    !_this7.state.active && _this7.setState((function(__state_argument) {
                        return {
                            hovered: true
                        }
                    }));
                    null === onHoverStart || void 0 === onHoverStart ? void 0 : onHoverStart(event)
                }), {
                    selector: selector
                }, "UIFeedback")
            }
        }
        return
    };
    _proto.hoverEndEffect = function() {
        var _this8 = this;
        var _this$props8 = this.props,
            activeStateUnit = _this$props8.activeStateUnit,
            hoverStateEnabled = _this$props8.hoverStateEnabled,
            onHoverEnd = _this$props8.onHoverEnd;
        var selector = activeStateUnit;
        if (hoverStateEnabled) {
            return (0, _subscribe_to_event.subscribeToDxHoverEndEvent)(this.widgetElementRef.current, (function(event) {
                if (_this8.state.hovered) {
                    _this8.setState((function(__state_argument) {
                        return {
                            hovered: false
                        }
                    }));
                    null === onHoverEnd || void 0 === onHoverEnd ? void 0 : onHoverEnd(event)
                }
            }), {
                selector: selector
            }, "UIFeedback")
        }
        return
    };
    _proto.keyboardEffect = function() {
        var _this$props9 = this.props,
            focusStateEnabled = _this$props9.focusStateEnabled,
            onKeyDown = _this$props9.onKeyDown;
        if (focusStateEnabled && onKeyDown) {
            var id = _short.keyboard.on(this.widgetElementRef.current, this.widgetElementRef.current, (function(e) {
                return onKeyDown(e)
            }));
            return function() {
                return _short.keyboard.off(id)
            }
        }
        return
    };
    _proto.resizeEffect = function() {
        var _this9 = this;
        var namespace = "".concat(this.props.name, "VisibilityChange");
        var onDimensionChanged = this.props.onDimensionChanged;
        if (onDimensionChanged) {
            _short.resize.on(this.widgetElementRef.current, onDimensionChanged, {
                namespace: namespace
            });
            return function() {
                return _short.resize.off(_this9.widgetElementRef.current, {
                    namespace: namespace
                })
            }
        }
        return
    };
    _proto.windowResizeEffect = function() {
        var onDimensionChanged = this.props.onDimensionChanged;
        if (onDimensionChanged) {
            _resize_callbacks.default.add(onDimensionChanged);
            return function() {
                _resize_callbacks.default.remove(onDimensionChanged)
            }
        }
        return
    };
    _proto.visibilityEffect = function() {
        var _this10 = this;
        var _this$props10 = this.props,
            name = _this$props10.name,
            onVisibilityChange = _this$props10.onVisibilityChange;
        var namespace = "".concat(name, "VisibilityChange");
        if (onVisibilityChange) {
            _short.visibility.on(this.widgetElementRef.current, (function() {
                return onVisibilityChange(true)
            }), (function() {
                return onVisibilityChange(false)
            }), {
                namespace: namespace
            });
            return function() {
                return _short.visibility.off(_this10.widgetElementRef.current, {
                    namespace: namespace
                })
            }
        }
        return
    };
    _proto.checkDeprecation = function() {
        var _this$props11 = this.props,
            height = _this$props11.height,
            width = _this$props11.width;
        if ((0, _type.isFunction)(width)) {
            _errors.default.log("W0017", "width")
        }
        if ((0, _type.isFunction)(height)) {
            _errors.default.log("W0017", "height")
        }
    };
    _proto.applyCssTextEffect = function() {
        var cssText = this.props.cssText;
        if ("" !== cssText) {
            this.widgetElementRef.current.style.cssText = cssText
        }
    };
    _proto.focus = function() {
        _short.focus.trigger(this.widgetElementRef.current)
    };
    _proto.blur = function() {
        var activeElement = _dom_adapter.default.getActiveElement();
        if (this.widgetElementRef.current === activeElement) {
            activeElement.blur()
        }
    };
    _proto.activate = function() {
        this.setState((function(__state_argument) {
            return {
                active: true
            }
        }))
    };
    _proto.deactivate = function() {
        this.setState((function(__state_argument) {
            return {
                active: false
            }
        }))
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            active: this.state.active,
            focused: this.state.focused,
            hovered: this.state.hovered,
            widgetElementRef: this.widgetElementRef,
            config: this.config,
            shouldRenderConfigProvider: this.shouldRenderConfigProvider,
            rtlEnabled: this.rtlEnabled,
            attributes: this.attributes,
            styles: this.styles,
            cssClasses: this.cssClasses,
            tabIndex: this.tabIndex,
            restAttributes: this.restAttributes
        })
    };
    _createClass(Widget, [{
        key: "config",
        get: function() {
            if ("ConfigContext" in this.context) {
                return this.context.ConfigContext
            }
            return _config_context.ConfigContext
        }
    }, {
        key: "shouldRenderConfigProvider",
        get: function() {
            var rtlEnabled = this.props.rtlEnabled;
            return (0, _resolve_rtl.resolveRtlEnabledDefinition)(rtlEnabled, this.config)
        }
    }, {
        key: "rtlEnabled",
        get: function() {
            var rtlEnabled = this.props.rtlEnabled;
            return (0, _resolve_rtl.resolveRtlEnabled)(rtlEnabled, this.config)
        }
    }, {
        key: "attributes",
        get: function() {
            var _this$props12 = this.props,
                aria = _this$props12.aria,
                disabled = _this$props12.disabled,
                focusStateEnabled = _this$props12.focusStateEnabled,
                visible = _this$props12.visible;
            var accessKey = focusStateEnabled && !disabled && this.props.accessKey;
            return _extends({}, (0, _extend.extend)({}, this.restAttributes, accessKey && {
                accessKey: accessKey
            }), getAria(_extends({}, aria, {
                disabled: disabled,
                hidden: !visible
            })))
        }
    }, {
        key: "styles",
        get: function() {
            var _this$props13 = this.props,
                height = _this$props13.height,
                width = _this$props13.width;
            var style = this.restAttributes.style || {};
            var computedWidth = (0, _style.normalizeStyleProp)("width", (0, _type.isFunction)(width) ? width() : width);
            var computedHeight = (0, _style.normalizeStyleProp)("height", (0, _type.isFunction)(height) ? height() : height);
            return _extends({}, style, {
                height: null !== computedHeight && void 0 !== computedHeight ? computedHeight : style.height,
                width: null !== computedWidth && void 0 !== computedWidth ? computedWidth : style.width
            })
        }
    }, {
        key: "cssClasses",
        get: function() {
            var _classesMap;
            var _this$props14 = this.props,
                activeStateEnabled = _this$props14.activeStateEnabled,
                addWidgetClass = _this$props14.addWidgetClass,
                className = _this$props14.className,
                classes = _this$props14.classes,
                disabled = _this$props14.disabled,
                focusStateEnabled = _this$props14.focusStateEnabled,
                hoverStateEnabled = _this$props14.hoverStateEnabled,
                onVisibilityChange = _this$props14.onVisibilityChange,
                visible = _this$props14.visible;
            var isFocusable = !!focusStateEnabled && !disabled;
            var isHoverable = !!hoverStateEnabled && !disabled;
            var canBeActive = !!activeStateEnabled && !disabled;
            var classesMap = (_classesMap = {
                "dx-widget": !!addWidgetClass
            }, _defineProperty(_classesMap, String(classes), !!classes), _defineProperty(_classesMap, String(className), !!className), _defineProperty(_classesMap, "dx-state-disabled", !!disabled), _defineProperty(_classesMap, "dx-state-invisible", !visible), _defineProperty(_classesMap, "dx-state-focused", !!this.state.focused && isFocusable), _defineProperty(_classesMap, "dx-state-active", !!this.state.active && canBeActive), _defineProperty(_classesMap, "dx-state-hover", !!this.state.hovered && isHoverable && !this.state.active), _defineProperty(_classesMap, "dx-rtl", !!this.rtlEnabled), _defineProperty(_classesMap, "dx-visibility-change-handler", !!onVisibilityChange), _classesMap);
            return (0, _combine_classes.combineClasses)(classesMap)
        }
    }, {
        key: "tabIndex",
        get: function() {
            var _this$props15 = this.props,
                disabled = _this$props15.disabled,
                focusStateEnabled = _this$props15.focusStateEnabled,
                tabIndex = _this$props15.tabIndex;
            var isFocusable = focusStateEnabled && !disabled;
            return isFocusable ? tabIndex : void 0
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props16 = this.props,
                restProps = (_this$props16._feedbackHideTimeout, _this$props16._feedbackShowTimeout, _this$props16.accessKey, _this$props16.activeStateEnabled, _this$props16.activeStateUnit, _this$props16.addWidgetClass, _this$props16.aria, _this$props16.children, _this$props16.className, _this$props16.classes, _this$props16.cssText, _this$props16.disabled, _this$props16.focusStateEnabled, _this$props16.height, _this$props16.hint, _this$props16.hoverStateEnabled, _this$props16.name, _this$props16.onActive, _this$props16.onClick, _this$props16.onDimensionChanged, _this$props16.onFocusIn, _this$props16.onFocusOut, _this$props16.onHoverEnd, _this$props16.onHoverStart, _this$props16.onInactive, _this$props16.onKeyDown, _this$props16.onRootElementRendered, _this$props16.onVisibilityChange, _this$props16.rootElementRef, _this$props16.rtlEnabled, _this$props16.tabIndex, _this$props16.visible, _this$props16.width, _objectWithoutProperties(_this$props16, _excluded));
            return restProps
        }
    }]);
    return Widget
}(_inferno2.InfernoWrapperComponent);
exports.Widget = Widget;
Widget.defaultProps = WidgetProps;
