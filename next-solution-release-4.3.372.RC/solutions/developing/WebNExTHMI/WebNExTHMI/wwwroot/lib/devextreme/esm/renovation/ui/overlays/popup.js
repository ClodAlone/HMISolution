/**
 * DevExtreme (esm/renovation/ui/overlays/popup.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["accessKey", "activeStateEnabled", "animation", "children", "className", "closeOnOutsideClick", "container", "contentTemplate", "defaultVisible", "deferRendering", "disabled", "dragEnabled", "elementAttr", "focusStateEnabled", "fullScreen", "height", "hint", "hoverStateEnabled", "maxHeight", "maxWidth", "minHeight", "minWidth", "onClick", "onHidden", "onHiding", "onInitialized", "onKeyDown", "onOptionChanged", "onResize", "onResizeEnd", "onResizeStart", "onShowing", "onShown", "onTitleRendered", "position", "resizeEnabled", "rtlEnabled", "shading", "shadingColor", "showCloseButton", "showTitle", "tabIndex", "title", "titleTemplate", "toolbarItems", "visible", "visibleChange", "width"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import {
    getWindow
} from "../../../core/utils/window";
import devices from "../../../core/devices";
import LegacyPopup from "../../../ui/popup";
import {
    DomComponentWrapper
} from "../common/dom_component_wrapper";
import {
    BaseWidgetProps
} from "../common/base_props";
var isDesktop = !(!devices.real().generic || devices.isSimulator());
var window = getWindow();
export var viewFunction = _ref => {
    var {
        domComponentWrapperRef: domComponentWrapperRef,
        props: props,
        restAttributes: restAttributes
    } = _ref;
    var {
        children: children
    } = props;
    return normalizeProps(createComponentVNode(2, DomComponentWrapper, _extends({
        componentType: LegacyPopup,
        componentProps: props,
        templateNames: ["titleTemplate", "contentTemplate"]
    }, restAttributes, {
        children: children
    }), null, domComponentWrapperRef))
};
export var PopupProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseWidgetProps), Object.getOwnPropertyDescriptors({
    get animation() {
        return {
            show: {
                type: "slide",
                duration: 400,
                from: {
                    position: {
                        my: "top",
                        at: "bottom",
                        of: window
                    }
                },
                to: {
                    position: {
                        my: "center",
                        at: "center",
                        of: window
                    }
                }
            },
            hide: {
                type: "slide",
                duration: 400,
                from: {
                    position: {
                        my: "center",
                        at: "center",
                        of: window
                    }
                },
                to: {
                    position: {
                        my: "top",
                        at: "bottom",
                        of: window
                    }
                }
            }
        }
    },
    closeOnOutsideClick: false,
    contentTemplate: "content",
    deferRendering: true,
    disabled: false,
    dragEnabled: isDesktop,
    get elementAttr() {
        return {}
    },
    focusStateEnabled: isDesktop,
    fullScreen: false,
    hoverStateEnabled: false,
    maxHeight: null,
    maxWidth: null,
    minHeight: null,
    minWidth: null,
    get position() {
        return {
            my: "center",
            at: "center",
            of: "window"
        }
    },
    resizeEnabled: false,
    rtlEnabled: false,
    shading: true,
    shadingColor: "",
    showCloseButton: isDesktop,
    showTitle: true,
    tabIndex: 0,
    title: "",
    titleTemplate: "title",
    defaultVisible: true,
    visibleChange: () => {},
    isReactComponentWrapper: true
})));
import {
    convertRulesToOptions
} from "../../../core/options/utils";
import {
    createRef as infernoCreateRef
} from "inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class Popup extends InfernoComponent {
    constructor(props) {
        super(props);
        this.domComponentWrapperRef = infernoCreateRef();
        this.state = {
            visible: void 0 !== this.props.visible ? this.props.visible : this.props.defaultVisible
        };
        this.saveInstance = this.saveInstance.bind(this);
        this.setHideEventListener = this.setHideEventListener.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.saveInstance, []), new InfernoEffect(this.setHideEventListener, [this.props.visibleChange])]
    }
    updateEffects() {
        var _this$_effects$, _this$_effects$2;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([]);
        null === (_this$_effects$2 = this._effects[1]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props.visibleChange])
    }
    saveInstance() {
        var _this$domComponentWra;
        this.instance = null === (_this$domComponentWra = this.domComponentWrapperRef.current) || void 0 === _this$domComponentWra ? void 0 : _this$domComponentWra.getInstance()
    }
    setHideEventListener() {
        this.instance.option("onHiding", () => {
            var __newValue;
            this.setState(__state_argument => {
                __newValue = false;
                return {
                    visible: __newValue
                }
            });
            this.props.visibleChange(__newValue)
        })
    }
    get restAttributes() {
        var _this$props$visible = _extends({}, this.props, {
                visible: void 0 !== this.props.visible ? this.props.visible : this.state.visible
            }),
            restProps = _objectWithoutPropertiesLoose(_this$props$visible, _excluded);
        return restProps
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                visible: void 0 !== this.props.visible ? this.props.visible : this.state.visible,
                contentTemplate: getTemplate(props.contentTemplate),
                titleTemplate: getTemplate(props.titleTemplate)
            }),
            domComponentWrapperRef: this.domComponentWrapperRef,
            restAttributes: this.restAttributes
        })
    }
}

function __processTwoWayProps(defaultProps) {
    var twoWayProps = ["visible"];
    return Object.keys(defaultProps).reduce((props, propName) => {
        var propValue = defaultProps[propName];
        var defaultPropName = twoWayProps.some(p => p === propName) ? "default" + propName.charAt(0).toUpperCase() + propName.slice(1) : propName;
        props[defaultPropName] = propValue;
        return props
    }, {})
}
Popup.defaultProps = PopupProps;
var __defaultOptionRules = [];
export function defaultOptions(rule) {
    __defaultOptionRules.push(rule);
    Popup.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(Popup.defaultProps), Object.getOwnPropertyDescriptors(__processTwoWayProps(convertRulesToOptions(__defaultOptionRules)))))
}
