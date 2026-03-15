/**
 * DevExtreme (renovation/viz/common/base_widget.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.Props = exports.BaseWidget = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _type = require("../../../core/utils/type");
var _combine_classes = require("../../utils/combine_classes");
var _base_props = require("./base_props");
var _config_context = require("../../common/config_context");
var _config_provider = require("../../common/config_provider");
var _svg_root = require("./renderers/svg_root");
var _gray_scale_filter = require("./renderers/gray_scale_filter");
var _utils = require("./utils");
var _resolve_rtl = require("../../utils/resolve_rtl");
var _utils2 = require("./renderers/utils");
var _excluded = ["canvas", "canvasChange", "children", "className", "classes", "defaultCanvas", "disabled", "margin", "pointerEvents", "rootElementRef", "rtlEnabled", "size"];

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
var DEFAULT_CANVAS = {
    left: 0,
    top: 0,
    right: 0,
    bottom: 0,
    width: 0,
    height: 0
};
var getCssClasses = function(model) {
    var containerClassesMap = _defineProperty({
        "dx-widget": true,
        "dx-visibility-change-handler": true
    }, String(model.className), !!model.className);
    return (0, _combine_classes.combineClasses)(containerClassesMap)
};
var calculateCanvas = function(model) {
    var _model$size, _model$margin, _model$defaultCanvas;
    var _ref = null !== (_model$size = model.size) && void 0 !== _model$size ? _model$size : {},
        height = _ref.height,
        width = _ref.width;
    var margin = null !== (_model$margin = model.margin) && void 0 !== _model$margin ? _model$margin : {};
    var defaultCanvas = null !== (_model$defaultCanvas = model.defaultCanvas) && void 0 !== _model$defaultCanvas ? _model$defaultCanvas : DEFAULT_CANVAS;
    var elementWidth = !(0, _utils.sizeIsValid)(width) ? (0, _utils.getElementWidth)(model.element) : 0;
    var elementHeight = !(0, _utils.sizeIsValid)(height) ? (0, _utils.getElementHeight)(model.element) : 0;
    var canvas = {
        width: width && width <= 0 ? 0 : Math.floor((0, _utils.pickPositiveValue)([width, elementWidth, defaultCanvas.width])),
        height: height && height <= 0 ? 0 : Math.floor((0, _utils.pickPositiveValue)([height, elementHeight, defaultCanvas.height])),
        left: (0, _utils.pickPositiveValue)([margin.left, defaultCanvas.left]),
        top: (0, _utils.pickPositiveValue)([margin.top, defaultCanvas.top]),
        right: (0, _utils.pickPositiveValue)([margin.right, defaultCanvas.right]),
        bottom: (0, _utils.pickPositiveValue)([margin.bottom, defaultCanvas.bottom])
    };
    if (canvas.width - canvas.left - canvas.right <= 0 || canvas.height - canvas.top - canvas.bottom <= 0) {
        return _extends({}, defaultCanvas)
    }
    return canvas
};
var viewFunction = function(viewModel) {
    var grayFilterId = viewModel.props.disabled ? (0, _utils2.getNextDefsSvgId)() : void 0;
    var canvas = viewModel.props.canvas || DEFAULT_CANVAS;
    var widget = (0, _inferno.normalizeProps)((0, _inferno.createVNode)(1, "div", viewModel.cssClasses, (0, _inferno.createComponentVNode)(2, _svg_root.RootSvgElement, {
        rootElementRef: viewModel.svgElementRef,
        className: viewModel.props.classes,
        width: canvas.width,
        height: canvas.height,
        pointerEvents: viewModel.pointerEventsState,
        filter: grayFilterId ? (0, _utils2.getFuncIri)(grayFilterId) : void 0,
        children: (0, _inferno.createFragment)([(0, _inferno.createVNode)(32, "defs", null, grayFilterId && (0, _inferno.createComponentVNode)(2, _gray_scale_filter.GrayScaleFilter, {
            id: grayFilterId
        }), 0), viewModel.props.children], 0)
    }), 2, _extends({}, viewModel.restAttributes), null, viewModel.containerRef));
    return viewModel.shouldRenderConfigProvider ? (0, _inferno.createComponentVNode)(2, _config_provider.ConfigProvider, {
        rtlEnabled: viewModel.rtlEnabled,
        children: widget
    }) : widget
};
exports.viewFunction = viewFunction;
var Props = _base_props.BaseWidgetProps;
exports.Props = Props;
var BaseWidget = function(_InfernoComponent) {
    _inheritsLoose(BaseWidget, _InfernoComponent);

    function BaseWidget(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.containerRef = (0, _inferno.createRef)();
        _this.svgElementRef = (0, _inferno.createRef)();
        _this.state = {
            canvas: void 0 !== _this.props.canvas ? _this.props.canvas : _this.props.defaultCanvas
        };
        _this.setRootElementRef = _this.setRootElementRef.bind(_assertThisInitialized(_this));
        _this.setCanvasEffect = _this.setCanvasEffect.bind(_assertThisInitialized(_this));
        _this.svg = _this.svg.bind(_assertThisInitialized(_this));
        _this.setCanvas = _this.setCanvas.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = BaseWidget.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.setRootElementRef, []), new _inferno2.InfernoEffect(this.setCanvasEffect, [this.state.canvas, this.props.canvas, this.props.defaultCanvas, this.props.margin, this.props.size, this.props.canvasChange])]
    };
    _proto.updateEffects = function() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[1]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.state.canvas, this.props.canvas, this.props.defaultCanvas, this.props.margin, this.props.size, this.props.canvasChange])
    };
    _proto.setRootElementRef = function() {
        this.props.rootElementRef.current = this.containerRef.current
    };
    _proto.setCanvasEffect = function() {
        this.setCanvas()
    };
    _proto.setCanvas = function() {
        var _this$props = this.props,
            defaultCanvas = _this$props.defaultCanvas,
            margin = _this$props.margin,
            size = _this$props.size;
        var newCanvas = calculateCanvas({
            element: this.containerRef.current,
            defaultCanvas: defaultCanvas,
            size: size,
            margin: margin
        });
        if ((0, _type.isDefined)(newCanvas.height) && (0, _type.isDefined)(newCanvas.width) && (0, _utils.isUpdatedFlatObject)(void 0 !== this.props.canvas ? this.props.canvas : this.state.canvas, newCanvas)) {
            var __newValue;
            this.setState((function(__state_argument) {
                __newValue = newCanvas;
                return {
                    canvas: __newValue
                }
            }));
            this.props.canvasChange(__newValue)
        }
    };
    _proto.svg = function() {
        return this.svgElementRef.current
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                canvas: void 0 !== this.props.canvas ? this.props.canvas : this.state.canvas
            }),
            containerRef: this.containerRef,
            svgElementRef: this.svgElementRef,
            config: this.config,
            shouldRenderConfigProvider: this.shouldRenderConfigProvider,
            rtlEnabled: this.rtlEnabled,
            pointerEventsState: this.pointerEventsState,
            cssClasses: this.cssClasses,
            setCanvas: this.setCanvas,
            restAttributes: this.restAttributes
        })
    };
    _createClass(BaseWidget, [{
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
        key: "pointerEventsState",
        get: function() {
            var _this$props2 = this.props,
                disabled = _this$props2.disabled,
                pointerEvents = _this$props2.pointerEvents;
            return disabled ? "none" : pointerEvents
        }
    }, {
        key: "cssClasses",
        get: function() {
            var className = this.props.className;
            return getCssClasses({
                className: className
            })
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props$canvas = _extends({}, this.props, {
                    canvas: void 0 !== this.props.canvas ? this.props.canvas : this.state.canvas
                }),
                restProps = (_this$props$canvas.canvas, _this$props$canvas.canvasChange, _this$props$canvas.children, _this$props$canvas.className, _this$props$canvas.classes, _this$props$canvas.defaultCanvas, _this$props$canvas.disabled, _this$props$canvas.margin, _this$props$canvas.pointerEvents, _this$props$canvas.rootElementRef, _this$props$canvas.rtlEnabled, _this$props$canvas.size, _objectWithoutProperties(_this$props$canvas, _excluded));
            return restProps
        }
    }]);
    return BaseWidget
}(_inferno2.InfernoComponent);
exports.BaseWidget = BaseWidget;
BaseWidget.defaultProps = Props;
