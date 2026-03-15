/**
 * DevExtreme (cjs/renovation/ui/overlays/popup.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.PopupProps = exports.Popup = void 0;
exports.defaultOptions = defaultOptions;
exports.viewFunction = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _window = require("../../../core/utils/window");
var _devices = _interopRequireDefault(require("../../../core/devices"));
var _popup = _interopRequireDefault(require("../../../ui/popup"));
var _dom_component_wrapper = require("../common/dom_component_wrapper");
var _base_props = require("../common/base_props");
var _utils = require("../../../core/options/utils");
var _excluded = ["accessKey", "activeStateEnabled", "animation", "children", "className", "closeOnOutsideClick", "container", "contentTemplate", "defaultVisible", "deferRendering", "disabled", "dragEnabled", "elementAttr", "focusStateEnabled", "fullScreen", "height", "hint", "hoverStateEnabled", "maxHeight", "maxWidth", "minHeight", "minWidth", "onClick", "onHidden", "onHiding", "onInitialized", "onKeyDown", "onOptionChanged", "onResize", "onResizeEnd", "onResizeStart", "onShowing", "onShown", "onTitleRendered", "position", "resizeEnabled", "rtlEnabled", "shading", "shadingColor", "showCloseButton", "showTitle", "tabIndex", "title", "titleTemplate", "toolbarItems", "visible", "visibleChange", "width"];

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
var isDesktop = !(!_devices.default.real().generic || _devices.default.isSimulator());
var window = (0, _window.getWindow)();
var viewFunction = function(_ref) {
    var domComponentWrapperRef = _ref.domComponentWrapperRef,
        props = _ref.props,
        restAttributes = _ref.restAttributes;
    var children = props.children;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _dom_component_wrapper.DomComponentWrapper, _extends({
        componentType: _popup.default,
        componentProps: props,
        templateNames: ["titleTemplate", "contentTemplate"]
    }, restAttributes, {
        children: children
    }), null, domComponentWrapperRef))
};
exports.viewFunction = viewFunction;
var PopupProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_base_props.BaseWidgetProps), Object.getOwnPropertyDescriptors(Object.defineProperties({
    closeOnOutsideClick: false,
    contentTemplate: "content",
    deferRendering: true,
    disabled: false,
    dragEnabled: isDesktop,
    focusStateEnabled: isDesktop,
    fullScreen: false,
    hoverStateEnabled: false,
    maxHeight: null,
    maxWidth: null,
    minHeight: null,
    minWidth: null,
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
    visibleChange: function() {},
    isReactComponentWrapper: true
}, {
    animation: {
        get: function() {
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
        configurable: true,
        enumerable: true
    },
    elementAttr: {
        get: function() {
            return {}
        },
        configurable: true,
        enumerable: true
    },
    position: {
        get: function() {
            return {
                my: "center",
                at: "center",
                of: "window"
            }
        },
        configurable: true,
        enumerable: true
    }
}))));
exports.PopupProps = PopupProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var Popup = function(_InfernoComponent) {
    _inheritsLoose(Popup, _InfernoComponent);

    function Popup(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.domComponentWrapperRef = (0, _inferno.createRef)();
        _this.state = {
            visible: void 0 !== _this.props.visible ? _this.props.visible : _this.props.defaultVisible
        };
        _this.saveInstance = _this.saveInstance.bind(_assertThisInitialized(_this));
        _this.setHideEventListener = _this.setHideEventListener.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = Popup.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.saveInstance, []), new _inferno2.InfernoEffect(this.setHideEventListener, [this.props.visibleChange])]
    };
    _proto.updateEffects = function() {
        var _this$_effects$, _this$_effects$2;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([]);
        null === (_this$_effects$2 = this._effects[1]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props.visibleChange])
    };
    _proto.saveInstance = function() {
        var _this$domComponentWra;
        this.instance = null === (_this$domComponentWra = this.domComponentWrapperRef.current) || void 0 === _this$domComponentWra ? void 0 : _this$domComponentWra.getInstance()
    };
    _proto.setHideEventListener = function() {
        var _this2 = this;
        this.instance.option("onHiding", (function() {
            var __newValue;
            _this2.setState((function(__state_argument) {
                __newValue = false;
                return {
                    visible: __newValue
                }
            }));
            _this2.props.visibleChange(__newValue)
        }))
    };
    _proto.render = function() {
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
    };
    _createClass(Popup, [{
        key: "restAttributes",
        get: function() {
            var _this$props$visible = _extends({}, this.props, {
                    visible: void 0 !== this.props.visible ? this.props.visible : this.state.visible
                }),
                restProps = (_this$props$visible.accessKey, _this$props$visible.activeStateEnabled, _this$props$visible.animation, _this$props$visible.children, _this$props$visible.className, _this$props$visible.closeOnOutsideClick, _this$props$visible.container, _this$props$visible.contentTemplate, _this$props$visible.defaultVisible, _this$props$visible.deferRendering, _this$props$visible.disabled, _this$props$visible.dragEnabled, _this$props$visible.elementAttr, _this$props$visible.focusStateEnabled, _this$props$visible.fullScreen, _this$props$visible.height, _this$props$visible.hint, _this$props$visible.hoverStateEnabled, _this$props$visible.maxHeight, _this$props$visible.maxWidth, _this$props$visible.minHeight, _this$props$visible.minWidth, _this$props$visible.onClick, _this$props$visible.onHidden, _this$props$visible.onHiding, _this$props$visible.onInitialized, _this$props$visible.onKeyDown, _this$props$visible.onOptionChanged, _this$props$visible.onResize, _this$props$visible.onResizeEnd, _this$props$visible.onResizeStart, _this$props$visible.onShowing, _this$props$visible.onShown, _this$props$visible.onTitleRendered, _this$props$visible.position, _this$props$visible.resizeEnabled, _this$props$visible.rtlEnabled, _this$props$visible.shading, _this$props$visible.shadingColor, _this$props$visible.showCloseButton, _this$props$visible.showTitle, _this$props$visible.tabIndex, _this$props$visible.title, _this$props$visible.titleTemplate, _this$props$visible.toolbarItems, _this$props$visible.visible, _this$props$visible.visibleChange, _this$props$visible.width, _objectWithoutProperties(_this$props$visible, _excluded));
            return restProps
        }
    }]);
    return Popup
}(_inferno2.InfernoComponent);
exports.Popup = Popup;

function __processTwoWayProps(defaultProps) {
    var twoWayProps = ["visible"];
    return Object.keys(defaultProps).reduce((function(props, propName) {
        var propValue = defaultProps[propName];
        var defaultPropName = twoWayProps.some((function(p) {
            return p === propName
        })) ? "default" + propName.charAt(0).toUpperCase() + propName.slice(1) : propName;
        props[defaultPropName] = propValue;
        return props
    }), {})
}
Popup.defaultProps = PopupProps;
var __defaultOptionRules = [];

function defaultOptions(rule) {
    __defaultOptionRules.push(rule);
    Popup.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(Popup.defaultProps), Object.getOwnPropertyDescriptors(__processTwoWayProps((0, _utils.convertRulesToOptions)(__defaultOptionRules)))))
}
