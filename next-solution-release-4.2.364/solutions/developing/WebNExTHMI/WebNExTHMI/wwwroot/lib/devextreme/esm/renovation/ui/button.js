/**
 * DevExtreme (esm/renovation/ui/button.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["accessKey", "activeStateEnabled", "children", "className", "disabled", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "icon", "iconPosition", "iconTemplate", "onClick", "onKeyDown", "onSubmit", "pressed", "rtlEnabled", "stylingMode", "tabIndex", "template", "templateData", "text", "type", "useInkRipple", "useSubmitBehavior", "visible", "width"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    createDefaultOptionRules,
    convertRulesToOptions
} from "../../core/options/utils";
import devices from "../../core/devices";
import {
    isMaterial,
    current
} from "../../ui/themes";
import {
    click
} from "../../events/short";
import {
    combineClasses
} from "../utils/combine_classes";
import {
    getImageSourceType
} from "../../core/utils/icon";
import {
    Icon
} from "./common/icon";
import {
    InkRipple
} from "./common/ink_ripple";
import {
    Widget
} from "./common/widget";
import {
    BaseWidgetProps
} from "./common/base_props";
var stylingModes = ["outlined", "text", "contained"];
var getCssClasses = model => {
    var {
        icon: icon,
        iconPosition: iconPosition,
        stylingMode: stylingMode,
        text: text,
        type: type
    } = model;
    var isValidStylingMode = stylingMode && stylingModes.includes(stylingMode);
    var classesMap = {
        "dx-button": true,
        ["dx-button-mode-".concat(isValidStylingMode ? stylingMode : "contained")]: true,
        ["dx-button-".concat(null !== type && void 0 !== type ? type : "normal")]: true,
        "dx-button-has-text": !!text,
        "dx-button-has-icon": !!icon,
        "dx-button-icon-right": "left" !== iconPosition
    };
    return combineClasses(classesMap)
};
export var viewFunction = viewModel => {
    var {
        children: children,
        iconPosition: iconPosition,
        iconTemplate: IconTemplate,
        template: ButtonTemplate,
        text: text
    } = viewModel.props;
    var renderText = !viewModel.props.template && !children && "" !== text;
    var isIconLeft = "left" === iconPosition;
    var iconComponent = !viewModel.props.template && !children && (viewModel.iconSource || viewModel.props.iconTemplate) && createComponentVNode(2, Icon, {
        source: viewModel.iconSource,
        position: iconPosition,
        iconTemplate: IconTemplate
    });
    return normalizeProps(createComponentVNode(2, Widget, _extends({
        accessKey: viewModel.props.accessKey,
        activeStateEnabled: viewModel.props.activeStateEnabled,
        aria: viewModel.aria,
        className: viewModel.props.className,
        classes: viewModel.cssClasses,
        disabled: viewModel.props.disabled,
        focusStateEnabled: viewModel.props.focusStateEnabled,
        height: viewModel.props.height,
        hint: viewModel.props.hint,
        hoverStateEnabled: viewModel.props.hoverStateEnabled,
        onActive: viewModel.onActive,
        onClick: viewModel.onWidgetClick,
        onInactive: viewModel.onInactive,
        onKeyDown: viewModel.keyDown,
        rtlEnabled: viewModel.props.rtlEnabled,
        tabIndex: viewModel.props.tabIndex,
        visible: viewModel.props.visible,
        width: viewModel.props.width
    }, viewModel.restAttributes, {
        children: createVNode(1, "div", "dx-button-content", [viewModel.props.template && ButtonTemplate({
            data: viewModel.buttonTemplateData
        }), !viewModel.props.template && children, isIconLeft && iconComponent, renderText && createVNode(1, "span", "dx-button-text", text, 0), !isIconLeft && iconComponent, viewModel.props.useSubmitBehavior && createVNode(64, "input", "dx-button-submit-input", null, 1, {
            type: "submit",
            tabIndex: -1
        }, null, viewModel.submitInputRef), viewModel.props.useInkRipple && createComponentVNode(2, InkRipple, {
            config: viewModel.inkRippleConfig
        }, null, viewModel.inkRippleRef)], 0, null, null, viewModel.contentRef)
    }), null, viewModel.widgetRef))
};
export var ButtonProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseWidgetProps), Object.getOwnPropertyDescriptors({
    activeStateEnabled: true,
    hoverStateEnabled: true,
    icon: "",
    iconPosition: "left",
    stylingMode: "contained",
    text: "",
    type: "normal",
    useInkRipple: false,
    useSubmitBehavior: false,
    get templateData() {
        return {}
    }
})));
export var defaultOptionRules = createDefaultOptionRules([{
    device: () => "desktop" === devices.real().deviceType && !devices.isSimulator(),
    options: {
        focusStateEnabled: true
    }
}, {
    device: () => isMaterial(current()),
    options: {
        useInkRipple: true
    }
}]);
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
import {
    createRef as infernoCreateRef
} from "inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class Button extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.contentRef = infernoCreateRef();
        this.inkRippleRef = infernoCreateRef();
        this.submitInputRef = infernoCreateRef();
        this.widgetRef = infernoCreateRef();
        this.__getterCache = {};
        this.focus = this.focus.bind(this);
        this.activate = this.activate.bind(this);
        this.deactivate = this.deactivate.bind(this);
        this.submitEffect = this.submitEffect.bind(this);
        this.onActive = this.onActive.bind(this);
        this.onInactive = this.onInactive.bind(this);
        this.onWidgetClick = this.onWidgetClick.bind(this);
        this.keyDown = this.keyDown.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.submitEffect, [this.props.onSubmit, this.props.useSubmitBehavior]), createReRenderEffect()]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.onSubmit, this.props.useSubmitBehavior])
    }
    submitEffect() {
        var {
            onSubmit: onSubmit,
            useSubmitBehavior: useSubmitBehavior
        } = this.props;
        if (useSubmitBehavior && onSubmit) {
            click.on(this.submitInputRef.current, event => onSubmit({
                event: event,
                submitInput: this.submitInputRef.current
            }), {
                namespace: "UIFeedback"
            });
            return () => click.off(this.submitInputRef.current, {
                namespace: "UIFeedback"
            })
        }
        return
    }
    onActive(event) {
        var {
            useInkRipple: useInkRipple
        } = this.props;
        useInkRipple && this.inkRippleRef.current.showWave({
            element: this.contentRef.current,
            event: event
        })
    }
    onInactive(event) {
        var {
            useInkRipple: useInkRipple
        } = this.props;
        useInkRipple && this.inkRippleRef.current.hideWave({
            element: this.contentRef.current,
            event: event
        })
    }
    onWidgetClick(event) {
        var {
            onClick: onClick,
            useSubmitBehavior: useSubmitBehavior
        } = this.props;
        null === onClick || void 0 === onClick ? void 0 : onClick({
            event: event
        });
        useSubmitBehavior && this.submitInputRef.current.click()
    }
    keyDown(e) {
        var {
            onKeyDown: onKeyDown
        } = this.props;
        var {
            keyName: keyName,
            originalEvent: originalEvent,
            which: which
        } = e;
        var result = null === onKeyDown || void 0 === onKeyDown ? void 0 : onKeyDown(e);
        if (null !== result && void 0 !== result && result.cancel) {
            return result
        }
        if ("space" === keyName || "space" === which || "enter" === keyName || "enter" === which) {
            originalEvent.preventDefault();
            this.onWidgetClick(originalEvent)
        }
        return
    }
    get aria() {
        var {
            icon: icon,
            text: text
        } = this.props;
        var label = (null !== text && void 0 !== text ? text : "") || icon;
        if (!text && icon && "image" === getImageSourceType(icon)) {
            label = !icon.includes("base64") ? icon.replace(/.+\/([^.]+)\..+$/, "$1") : "Base64"
        }
        return _extends({
            role: "button"
        }, label ? {
            label: label
        } : {})
    }
    get cssClasses() {
        return getCssClasses(this.props)
    }
    get iconSource() {
        var {
            icon: icon,
            type: type
        } = this.props;
        if (icon || "back" === type) {
            return (null !== icon && void 0 !== icon ? icon : "") || "back"
        }
        return ""
    }
    get inkRippleConfig() {
        if (void 0 !== this.__getterCache.inkRippleConfig) {
            return this.__getterCache.inkRippleConfig
        }
        return this.__getterCache.inkRippleConfig = (() => {
            var {
                icon: icon,
                text: text,
                type: type
            } = this.props;
            return !text && icon || "back" === type ? {
                isCentered: true,
                useHoldAnimation: false,
                waveSizeCoefficient: 1
            } : {}
        })()
    }
    get buttonTemplateData() {
        var {
            icon: icon,
            templateData: templateData,
            text: text
        } = this.props;
        return _extends({
            icon: icon,
            text: text
        }, templateData)
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    focus() {
        this.widgetRef.current.focus()
    }
    activate() {
        this.widgetRef.current.activate()
    }
    deactivate() {
        this.widgetRef.current.deactivate()
    }
    componentWillUpdate(nextProps, nextState, context) {
        super.componentWillUpdate();
        if (this.props.icon !== nextProps.icon || this.props.text !== nextProps.text || this.props.type !== nextProps.type) {
            this.__getterCache.inkRippleConfig = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                template: getTemplate(props.template),
                iconTemplate: getTemplate(props.iconTemplate)
            }),
            contentRef: this.contentRef,
            submitInputRef: this.submitInputRef,
            inkRippleRef: this.inkRippleRef,
            widgetRef: this.widgetRef,
            onActive: this.onActive,
            onInactive: this.onInactive,
            onWidgetClick: this.onWidgetClick,
            keyDown: this.keyDown,
            aria: this.aria,
            cssClasses: this.cssClasses,
            iconSource: this.iconSource,
            inkRippleConfig: this.inkRippleConfig,
            buttonTemplateData: this.buttonTemplateData,
            restAttributes: this.restAttributes
        })
    }
}
Button.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(ButtonProps), Object.getOwnPropertyDescriptors(_extends({}, convertRulesToOptions(defaultOptionRules)))));
var __defaultOptionRules = [];
export function defaultOptions(rule) {
    __defaultOptionRules.push(rule);
    Button.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(Button.defaultProps), Object.getOwnPropertyDescriptors(convertRulesToOptions(defaultOptionRules)), Object.getOwnPropertyDescriptors(convertRulesToOptions(__defaultOptionRules))))
}
