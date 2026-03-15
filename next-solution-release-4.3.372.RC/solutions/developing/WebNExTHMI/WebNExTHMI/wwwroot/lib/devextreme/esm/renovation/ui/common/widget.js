/**
 * DevExtreme (esm/renovation/ui/common/widget.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["_feedbackHideTimeout", "_feedbackShowTimeout", "accessKey", "activeStateEnabled", "activeStateUnit", "addWidgetClass", "aria", "children", "className", "classes", "cssText", "disabled", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "name", "onActive", "onClick", "onDimensionChanged", "onFocusIn", "onFocusOut", "onHoverEnd", "onHoverStart", "onInactive", "onKeyDown", "onRootElementRendered", "onVisibilityChange", "rootElementRef", "rtlEnabled", "tabIndex", "visible", "width"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoWrapperComponent,
    normalizeStyles
} from "@devextreme/runtime/inferno";
import "../../../events/click";
import "../../../events/hover";
import {
    isFunction
} from "../../../core/utils/type";
import {
    dxClick,
    focus,
    keyboard,
    resize,
    visibility
} from "../../../events/short";
import {
    subscribeToDxActiveEvent,
    subscribeToDxInactiveEvent,
    subscribeToDxHoverStartEvent,
    subscribeToDxHoverEndEvent,
    subscribeToDxFocusInEvent,
    subscribeToDxFocusOutEvent
} from "../../utils/subscribe_to_event";
import {
    combineClasses
} from "../../utils/combine_classes";
import {
    extend
} from "../../../core/utils/extend";
import {
    normalizeStyleProp
} from "../../../core/utils/style";
import {
    BaseWidgetProps
} from "./base_props";
import {
    ConfigContext
} from "../../common/config_context";
import {
    ConfigProvider
} from "../../common/config_provider";
import {
    resolveRtlEnabled,
    resolveRtlEnabledDefinition
} from "../../utils/resolve_rtl";
import resizeCallbacks from "../../../core/utils/resize_callbacks";
import errors from "../../../core/errors";
import domAdapter from "../../../core/dom_adapter";
var DEFAULT_FEEDBACK_HIDE_TIMEOUT = 400;
var DEFAULT_FEEDBACK_SHOW_TIMEOUT = 30;
var getAria = args => Object.keys(args).reduce((r, key) => {
    if (args[key]) {
        return _extends({}, r, {
            ["role" === key || "id" === key ? key : "aria-".concat(key)]: String(args[key])
        })
    }
    return r
}, {});
export var viewFunction = viewModel => {
    var widget = normalizeProps(createVNode(1, "div", viewModel.cssClasses, viewModel.props.children, 0, _extends({}, viewModel.attributes, {
        tabIndex: viewModel.tabIndex,
        title: viewModel.props.hint,
        hidden: !viewModel.props.visible,
        style: normalizeStyles(viewModel.styles)
    }), null, viewModel.widgetElementRef));
    return viewModel.shouldRenderConfigProvider ? createComponentVNode(2, ConfigProvider, {
        rtlEnabled: viewModel.rtlEnabled,
        children: widget
    }) : widget
};
export var WidgetProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseWidgetProps), Object.getOwnPropertyDescriptors({
    _feedbackHideTimeout: DEFAULT_FEEDBACK_HIDE_TIMEOUT,
    _feedbackShowTimeout: DEFAULT_FEEDBACK_SHOW_TIMEOUT,
    cssText: "",
    get aria() {
        return {}
    },
    classes: "",
    name: "",
    addWidgetClass: true
})));
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
import {
    createRef as infernoCreateRef
} from "inferno";
export class Widget extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.widgetElementRef = infernoCreateRef();
        this.state = {
            active: false,
            focused: false,
            hovered: false
        };
        this.setRootElementRef = this.setRootElementRef.bind(this);
        this.activeEffect = this.activeEffect.bind(this);
        this.inactiveEffect = this.inactiveEffect.bind(this);
        this.clickEffect = this.clickEffect.bind(this);
        this.focus = this.focus.bind(this);
        this.blur = this.blur.bind(this);
        this.activate = this.activate.bind(this);
        this.deactivate = this.deactivate.bind(this);
        this.focusInEffect = this.focusInEffect.bind(this);
        this.focusOutEffect = this.focusOutEffect.bind(this);
        this.hoverStartEffect = this.hoverStartEffect.bind(this);
        this.hoverEndEffect = this.hoverEndEffect.bind(this);
        this.keyboardEffect = this.keyboardEffect.bind(this);
        this.resizeEffect = this.resizeEffect.bind(this);
        this.windowResizeEffect = this.windowResizeEffect.bind(this);
        this.visibilityEffect = this.visibilityEffect.bind(this);
        this.checkDeprecation = this.checkDeprecation.bind(this);
        this.applyCssTextEffect = this.applyCssTextEffect.bind(this)
    }
    get config() {
        if ("ConfigContext" in this.context) {
            return this.context.ConfigContext
        }
        return ConfigContext
    }
    createEffects() {
        return [new InfernoEffect(this.setRootElementRef, []), new InfernoEffect(this.activeEffect, [this.props._feedbackShowTimeout, this.props.activeStateEnabled, this.props.activeStateUnit, this.props.disabled, this.props.onActive]), new InfernoEffect(this.inactiveEffect, [this.props._feedbackHideTimeout, this.props.activeStateEnabled, this.props.activeStateUnit, this.props.onInactive, this.state.active]), new InfernoEffect(this.clickEffect, [this.props.disabled, this.props.name, this.props.onClick]), new InfernoEffect(this.focusInEffect, [this.props.disabled, this.props.focusStateEnabled, this.props.name, this.props.onFocusIn]), new InfernoEffect(this.focusOutEffect, [this.props.focusStateEnabled, this.props.name, this.props.onFocusOut, this.state.focused]), new InfernoEffect(this.hoverStartEffect, [this.props.activeStateUnit, this.props.disabled, this.props.hoverStateEnabled, this.props.onHoverStart, this.state.active]), new InfernoEffect(this.hoverEndEffect, [this.props.activeStateUnit, this.props.hoverStateEnabled, this.props.onHoverEnd, this.state.hovered]), new InfernoEffect(this.keyboardEffect, [this.props.focusStateEnabled, this.props.onKeyDown]), new InfernoEffect(this.resizeEffect, [this.props.name, this.props.onDimensionChanged]), new InfernoEffect(this.windowResizeEffect, [this.props.onDimensionChanged]), new InfernoEffect(this.visibilityEffect, [this.props.name, this.props.onVisibilityChange]), new InfernoEffect(this.checkDeprecation, [this.props.height, this.props.width]), new InfernoEffect(this.applyCssTextEffect, [this.props.cssText]), createReRenderEffect()]
    }
    updateEffects() {
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
    }
    setRootElementRef() {
        var {
            onRootElementRendered: onRootElementRendered,
            rootElementRef: rootElementRef
        } = this.props;
        if (rootElementRef) {
            rootElementRef.current = this.widgetElementRef.current
        }
        null === onRootElementRendered || void 0 === onRootElementRendered ? void 0 : onRootElementRendered(this.widgetElementRef.current)
    }
    activeEffect() {
        var {
            _feedbackShowTimeout: _feedbackShowTimeout,
            activeStateEnabled: activeStateEnabled,
            activeStateUnit: activeStateUnit,
            disabled: disabled,
            onActive: onActive
        } = this.props;
        var selector = activeStateUnit;
        if (activeStateEnabled) {
            if (!disabled) {
                return subscribeToDxActiveEvent(this.widgetElementRef.current, event => {
                    this.setState(__state_argument => ({
                        active: true
                    }));
                    null === onActive || void 0 === onActive ? void 0 : onActive(event)
                }, {
                    timeout: _feedbackShowTimeout,
                    selector: selector
                }, "UIFeedback")
            }
        }
        return
    }
    inactiveEffect() {
        var {
            _feedbackHideTimeout: _feedbackHideTimeout,
            activeStateEnabled: activeStateEnabled,
            activeStateUnit: activeStateUnit,
            onInactive: onInactive
        } = this.props;
        var selector = activeStateUnit;
        if (activeStateEnabled) {
            return subscribeToDxInactiveEvent(this.widgetElementRef.current, event => {
                if (this.state.active) {
                    this.setState(__state_argument => ({
                        active: false
                    }));
                    null === onInactive || void 0 === onInactive ? void 0 : onInactive(event)
                }
            }, {
                timeout: _feedbackHideTimeout,
                selector: selector
            }, "UIFeedback")
        }
        return
    }
    clickEffect() {
        var {
            disabled: disabled,
            name: name,
            onClick: onClick
        } = this.props;
        var namespace = name;
        if (onClick && !disabled) {
            dxClick.on(this.widgetElementRef.current, onClick, {
                namespace: namespace
            });
            return () => dxClick.off(this.widgetElementRef.current, {
                namespace: namespace
            })
        }
        return
    }
    focusInEffect() {
        var {
            disabled: disabled,
            focusStateEnabled: focusStateEnabled,
            name: name,
            onFocusIn: onFocusIn
        } = this.props;
        var namespace = "".concat(name, "Focus");
        if (focusStateEnabled) {
            if (!disabled) {
                return subscribeToDxFocusInEvent(this.widgetElementRef.current, event => {
                    if (!event.isDefaultPrevented()) {
                        this.setState(__state_argument => ({
                            focused: true
                        }));
                        null === onFocusIn || void 0 === onFocusIn ? void 0 : onFocusIn(event)
                    }
                }, null, namespace)
            }
        }
        return
    }
    focusOutEffect() {
        var {
            focusStateEnabled: focusStateEnabled,
            name: name,
            onFocusOut: onFocusOut
        } = this.props;
        var namespace = "".concat(name, "Focus");
        if (focusStateEnabled) {
            return subscribeToDxFocusOutEvent(this.widgetElementRef.current, event => {
                if (!event.isDefaultPrevented() && this.state.focused) {
                    this.setState(__state_argument => ({
                        focused: false
                    }));
                    null === onFocusOut || void 0 === onFocusOut ? void 0 : onFocusOut(event)
                }
            }, null, namespace)
        }
        return
    }
    hoverStartEffect() {
        var {
            activeStateUnit: activeStateUnit,
            disabled: disabled,
            hoverStateEnabled: hoverStateEnabled,
            onHoverStart: onHoverStart
        } = this.props;
        var selector = activeStateUnit;
        if (hoverStateEnabled) {
            if (!disabled) {
                return subscribeToDxHoverStartEvent(this.widgetElementRef.current, event => {
                    !this.state.active && this.setState(__state_argument => ({
                        hovered: true
                    }));
                    null === onHoverStart || void 0 === onHoverStart ? void 0 : onHoverStart(event)
                }, {
                    selector: selector
                }, "UIFeedback")
            }
        }
        return
    }
    hoverEndEffect() {
        var {
            activeStateUnit: activeStateUnit,
            hoverStateEnabled: hoverStateEnabled,
            onHoverEnd: onHoverEnd
        } = this.props;
        var selector = activeStateUnit;
        if (hoverStateEnabled) {
            return subscribeToDxHoverEndEvent(this.widgetElementRef.current, event => {
                if (this.state.hovered) {
                    this.setState(__state_argument => ({
                        hovered: false
                    }));
                    null === onHoverEnd || void 0 === onHoverEnd ? void 0 : onHoverEnd(event)
                }
            }, {
                selector: selector
            }, "UIFeedback")
        }
        return
    }
    keyboardEffect() {
        var {
            focusStateEnabled: focusStateEnabled,
            onKeyDown: onKeyDown
        } = this.props;
        if (focusStateEnabled && onKeyDown) {
            var id = keyboard.on(this.widgetElementRef.current, this.widgetElementRef.current, e => onKeyDown(e));
            return () => keyboard.off(id)
        }
        return
    }
    resizeEffect() {
        var namespace = "".concat(this.props.name, "VisibilityChange");
        var {
            onDimensionChanged: onDimensionChanged
        } = this.props;
        if (onDimensionChanged) {
            resize.on(this.widgetElementRef.current, onDimensionChanged, {
                namespace: namespace
            });
            return () => resize.off(this.widgetElementRef.current, {
                namespace: namespace
            })
        }
        return
    }
    windowResizeEffect() {
        var {
            onDimensionChanged: onDimensionChanged
        } = this.props;
        if (onDimensionChanged) {
            resizeCallbacks.add(onDimensionChanged);
            return () => {
                resizeCallbacks.remove(onDimensionChanged)
            }
        }
        return
    }
    visibilityEffect() {
        var {
            name: name,
            onVisibilityChange: onVisibilityChange
        } = this.props;
        var namespace = "".concat(name, "VisibilityChange");
        if (onVisibilityChange) {
            visibility.on(this.widgetElementRef.current, () => onVisibilityChange(true), () => onVisibilityChange(false), {
                namespace: namespace
            });
            return () => visibility.off(this.widgetElementRef.current, {
                namespace: namespace
            })
        }
        return
    }
    checkDeprecation() {
        var {
            height: height,
            width: width
        } = this.props;
        if (isFunction(width)) {
            errors.log("W0017", "width")
        }
        if (isFunction(height)) {
            errors.log("W0017", "height")
        }
    }
    applyCssTextEffect() {
        var {
            cssText: cssText
        } = this.props;
        if ("" !== cssText) {
            this.widgetElementRef.current.style.cssText = cssText
        }
    }
    get shouldRenderConfigProvider() {
        var {
            rtlEnabled: rtlEnabled
        } = this.props;
        return resolveRtlEnabledDefinition(rtlEnabled, this.config)
    }
    get rtlEnabled() {
        var {
            rtlEnabled: rtlEnabled
        } = this.props;
        return resolveRtlEnabled(rtlEnabled, this.config)
    }
    get attributes() {
        var {
            aria: aria,
            disabled: disabled,
            focusStateEnabled: focusStateEnabled,
            visible: visible
        } = this.props;
        var accessKey = focusStateEnabled && !disabled && this.props.accessKey;
        return _extends({}, extend({}, this.restAttributes, accessKey && {
            accessKey: accessKey
        }), getAria(_extends({}, aria, {
            disabled: disabled,
            hidden: !visible
        })))
    }
    get styles() {
        var {
            height: height,
            width: width
        } = this.props;
        var style = this.restAttributes.style || {};
        var computedWidth = normalizeStyleProp("width", isFunction(width) ? width() : width);
        var computedHeight = normalizeStyleProp("height", isFunction(height) ? height() : height);
        return _extends({}, style, {
            height: null !== computedHeight && void 0 !== computedHeight ? computedHeight : style.height,
            width: null !== computedWidth && void 0 !== computedWidth ? computedWidth : style.width
        })
    }
    get cssClasses() {
        var {
            activeStateEnabled: activeStateEnabled,
            addWidgetClass: addWidgetClass,
            className: className,
            classes: classes,
            disabled: disabled,
            focusStateEnabled: focusStateEnabled,
            hoverStateEnabled: hoverStateEnabled,
            onVisibilityChange: onVisibilityChange,
            visible: visible
        } = this.props;
        var isFocusable = !!focusStateEnabled && !disabled;
        var isHoverable = !!hoverStateEnabled && !disabled;
        var canBeActive = !!activeStateEnabled && !disabled;
        var classesMap = {
            "dx-widget": !!addWidgetClass,
            [String(classes)]: !!classes,
            [String(className)]: !!className,
            "dx-state-disabled": !!disabled,
            "dx-state-invisible": !visible,
            "dx-state-focused": !!this.state.focused && isFocusable,
            "dx-state-active": !!this.state.active && canBeActive,
            "dx-state-hover": !!this.state.hovered && isHoverable && !this.state.active,
            "dx-rtl": !!this.rtlEnabled,
            "dx-visibility-change-handler": !!onVisibilityChange
        };
        return combineClasses(classesMap)
    }
    get tabIndex() {
        var {
            disabled: disabled,
            focusStateEnabled: focusStateEnabled,
            tabIndex: tabIndex
        } = this.props;
        var isFocusable = focusStateEnabled && !disabled;
        return isFocusable ? tabIndex : void 0
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    focus() {
        focus.trigger(this.widgetElementRef.current)
    }
    blur() {
        var activeElement = domAdapter.getActiveElement();
        if (this.widgetElementRef.current === activeElement) {
            activeElement.blur()
        }
    }
    activate() {
        this.setState(__state_argument => ({
            active: true
        }))
    }
    deactivate() {
        this.setState(__state_argument => ({
            active: false
        }))
    }
    render() {
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
    }
}
Widget.defaultProps = WidgetProps;
