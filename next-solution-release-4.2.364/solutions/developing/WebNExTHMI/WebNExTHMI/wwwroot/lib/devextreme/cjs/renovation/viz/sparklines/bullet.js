/**
 * DevExtreme (cjs/renovation/viz/sparklines/bullet.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.BulletProps = exports.Bullet = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _combine_classes = require("../../utils/combine_classes");
var _resolve_rtl = require("../../utils/resolve_rtl");
var _get_element_offset = require("../../utils/get_element_offset");
var _base_props = require("../common/base_props");
var _base_widget = require("../common/base_widget");
var _utils = require("./utils");
var _config_context = require("../../common/config_context");
var _svg_path = require("../common/renderers/svg_path");
var _tooltip = require("../common/tooltip");
var _utils2 = require("../common/utils");
var _events_engine = _interopRequireDefault(require("../../../events/core/events_engine"));
var _index = require("../../../events/utils/index");
var _pointer = _interopRequireDefault(require("../../../events/pointer"));
var _dom_adapter = _interopRequireDefault(require("../../../core/dom_adapter"));
var _number = _interopRequireDefault(require("../../../core/polyfills/number"));
var _excluded = ["canvas", "canvasChange", "children", "className", "classes", "color", "defaultCanvas", "disabled", "endScaleValue", "margin", "onTooltipHidden", "onTooltipShown", "pointerEvents", "rtlEnabled", "showTarget", "showZeroLevel", "size", "startScaleValue", "target", "targetColor", "targetWidth", "tooltip", "value"];

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
var TARGET_MIN_Y = .02;
var TARGET_MAX_Y = .98;
var BAR_VALUE_MIN_Y = .1;
var BAR_VALUE_MAX_Y = .9;
var DEFAULT_CANVAS_WIDTH = 300;
var DEFAULT_CANVAS_HEIGHT = 30;
var DEFAULT_HORIZONTAL_MARGIN = 1;
var DEFAULT_VERTICAL_MARGIN = 2;
var DEFAULT_OFFSET = {
    top: 0,
    left: 0
};
var EVENT_NS = "sparkline-tooltip";
var POINTER_ACTION = (0, _index.addNamespace)([_pointer.default.down, _pointer.default.move], EVENT_NS);
var inCanvas = function(canvas, x, y) {
    var height = canvas.height,
        width = canvas.width;
    return (0, _utils2.pointInCanvas)({
        left: 0,
        top: 0,
        right: width,
        bottom: height,
        width: width,
        height: height
    }, x, y)
};
var getCssClasses = function(_ref) {
    var classes = _ref.classes;
    var rootClassesMap = _defineProperty({
        dxb: true,
        "dxb-bullet": true
    }, String(classes), !!classes);
    return (0, _combine_classes.combineClasses)(rootClassesMap)
};
var getContainerCssClasses = function(_ref2) {
    var className = _ref2.className;
    var rootClassesMap = _defineProperty({
        "dx-bullet": true
    }, String(className), !!className);
    return (0, _combine_classes.combineClasses)(rootClassesMap)
};
var viewFunction = function(viewModel) {
    var _viewModel$props = viewModel.props,
        color = _viewModel$props.color,
        disabled = _viewModel$props.disabled,
        margin = _viewModel$props.margin,
        size = _viewModel$props.size,
        targetColor = _viewModel$props.targetColor,
        targetWidth = _viewModel$props.targetWidth;
    var barValueShape = viewModel.barValueShape,
        customizedTooltipProps = viewModel.customizedTooltipProps,
        isValidBulletScale = viewModel.isValidBulletScale,
        isValidTarget = viewModel.isValidTarget,
        isValidZeroLevel = viewModel.isValidZeroLevel,
        targetShape = viewModel.targetShape,
        zeroLevelShape = viewModel.zeroLevelShape;
    return (0, _inferno.createFragment)([(0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _base_widget.BaseWidget, _extends({
        rootElementRef: viewModel.widgetRootRef,
        classes: viewModel.cssClasses,
        className: viewModel.cssClassName,
        size: size,
        margin: margin,
        defaultCanvas: viewModel.defaultCanvas,
        disabled: disabled,
        rtlEnabled: viewModel.rtlEnabled,
        canvasChange: viewModel.onCanvasChange,
        pointerEvents: "visible"
    }, viewModel.restAttributes, {
        children: isValidBulletScale ? (0, _inferno.createFragment)([(0, _inferno.createComponentVNode)(2, _svg_path.PathSvgElement, {
            type: "line",
            points: barValueShape,
            className: "dxb-bar-value",
            strokeLineCap: "square",
            fill: color
        }), isValidTarget && (0, _inferno.createComponentVNode)(2, _svg_path.PathSvgElement, {
            type: "line",
            points: targetShape,
            className: "dxb-target",
            sharp: true,
            strokeLineCap: "square",
            stroke: targetColor,
            strokeWidth: targetWidth
        }), isValidZeroLevel && (0, _inferno.createComponentVNode)(2, _svg_path.PathSvgElement, {
            type: "line",
            points: zeroLevelShape,
            className: "dxb-zero-level",
            sharp: true,
            strokeLineCap: "square",
            stroke: targetColor,
            strokeWidth: 1
        })], 0) : void 0
    }), null, viewModel.widgetRef)), customizedTooltipProps.enabled && (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _tooltip.Tooltip, _extends({
        rootWidget: viewModel.widgetRootRef
    }, customizedTooltipProps, {
        visible: viewModel.tooltipVisible
    }), null, viewModel.tooltipRef))], 0)
};
exports.viewFunction = viewFunction;
var BulletProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_base_props.BaseWidgetProps), Object.getOwnPropertyDescriptors(Object.defineProperties({
    value: 0,
    color: "#e8c267",
    target: 0,
    targetColor: "#666666",
    targetWidth: 4,
    showTarget: true,
    showZeroLevel: true,
    startScaleValue: 0
}, {
    tooltip: {
        get: function() {
            return _tooltip.TooltipProps
        },
        configurable: true,
        enumerable: true
    }
}))));
exports.BulletProps = BulletProps;
var Bullet = function(_InfernoWrapperCompon) {
    _inheritsLoose(Bullet, _InfernoWrapperCompon);

    function Bullet(props) {
        var _this;
        _this = _InfernoWrapperCompon.call(this, props) || this;
        _this.widgetRef = (0, _inferno.createRef)();
        _this.tooltipRef = (0, _inferno.createRef)();
        _this.widgetRootRef = (0, _inferno.createRef)();
        _this.__getterCache = {};
        _this.state = {
            argumentAxis: (0, _utils.createAxis)(true),
            valueAxis: (0, _utils.createAxis)(false),
            canvasState: {
                width: 0,
                height: 0,
                top: 0,
                left: 0,
                bottom: 0,
                right: 0
            },
            offsetState: DEFAULT_OFFSET,
            tooltipVisible: false,
            canvas: void 0 !== _this.props.canvas ? _this.props.canvas : _this.props.defaultCanvas
        };
        _this.tooltipEffect = _this.tooltipEffect.bind(_assertThisInitialized(_this));
        _this.tooltipOutEffect = _this.tooltipOutEffect.bind(_assertThisInitialized(_this));
        _this.onCanvasChange = _this.onCanvasChange.bind(_assertThisInitialized(_this));
        _this.prepareScaleProps = _this.prepareScaleProps.bind(_assertThisInitialized(_this));
        _this.getRange = _this.getRange.bind(_assertThisInitialized(_this));
        _this.getSimpleShape = _this.getSimpleShape.bind(_assertThisInitialized(_this));
        _this.pointerHandler = _this.pointerHandler.bind(_assertThisInitialized(_this));
        _this.pointerOutHandler = _this.pointerOutHandler.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = Bullet.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.tooltipEffect, [this.props.disabled, this.props.onTooltipHidden, this.props.onTooltipShown, this.props.tooltip, this.props.value, this.props.target, this.props.rtlEnabled, this.config, this.state.canvasState, this.state.offsetState]), new _inferno2.InfernoEffect(this.tooltipOutEffect, [this.state.tooltipVisible, this.state.offsetState, this.state.canvasState]), (0, _inferno2.createReRenderEffect)()]
    };
    _proto.updateEffects = function() {
        var _this$_effects$, _this$_effects$2;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.disabled, this.props.onTooltipHidden, this.props.onTooltipShown, this.props.tooltip, this.props.value, this.props.target, this.props.rtlEnabled, this.config, this.state.canvasState, this.state.offsetState]);
        null === (_this$_effects$2 = this._effects[1]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.state.tooltipVisible, this.state.offsetState, this.state.canvasState])
    };
    _proto.tooltipEffect = function() {
        var _this2 = this;
        var disabled = this.props.disabled;
        if (!disabled && this.customizedTooltipProps.enabled) {
            var _this$widgetRef$curre;
            var svg = null === (_this$widgetRef$curre = this.widgetRef.current) || void 0 === _this$widgetRef$curre ? void 0 : _this$widgetRef$curre.svg();
            _events_engine.default.on(svg, POINTER_ACTION, this.pointerHandler);
            return function() {
                _events_engine.default.off(svg, POINTER_ACTION, _this2.pointerHandler)
            }
        }
        return
    };
    _proto.tooltipOutEffect = function() {
        var _this3 = this;
        if (this.state.tooltipVisible) {
            var document = _dom_adapter.default.getDocument();
            _events_engine.default.on(document, POINTER_ACTION, this.pointerOutHandler);
            return function() {
                _events_engine.default.off(document, POINTER_ACTION, _this3.pointerOutHandler)
            }
        }
        return
    };
    _proto.onCanvasChange = function(canvas) {
        var _this$widgetRef$curre2, _this$widgetRef$curre3;
        this.setState((function(__state_argument) {
            return {
                canvasState: canvas
            }
        }));
        var svgElement = null !== (_this$widgetRef$curre2 = null === (_this$widgetRef$curre3 = this.widgetRef.current) || void 0 === _this$widgetRef$curre3 ? void 0 : _this$widgetRef$curre3.svg()) && void 0 !== _this$widgetRef$curre2 ? _this$widgetRef$curre2 : void 0;
        this.setState((function(__state_argument) {
            return {
                offsetState: (0, _get_element_offset.getElementOffset)(svgElement)
            }
        }))
    };
    _proto.prepareScaleProps = function() {
        var _this$props = this.props,
            endScaleValue = _this$props.endScaleValue,
            startScaleValue = _this$props.startScaleValue,
            target = _this$props.target,
            value = _this$props.value;
        var tmpProps = {
            inverted: false,
            value: value,
            target: target,
            startScaleValue: void 0 === startScaleValue ? Math.min(target, value, 0) : startScaleValue,
            endScaleValue: void 0 === endScaleValue ? Math.max(target, value) : endScaleValue
        };
        if (tmpProps.endScaleValue < tmpProps.startScaleValue) {
            var level = tmpProps.endScaleValue;
            tmpProps.endScaleValue = tmpProps.startScaleValue;
            tmpProps.startScaleValue = level;
            tmpProps.inverted = true
        }
        return tmpProps
    };
    _proto.getRange = function(scaleProps) {
        var endScaleValue = scaleProps.endScaleValue,
            inverted = scaleProps.inverted,
            startScaleValue = scaleProps.startScaleValue;
        return {
            arg: {
                invert: this.rtlEnabled ? !inverted : inverted,
                min: startScaleValue,
                max: endScaleValue,
                axisType: "continuous",
                dataType: "numeric"
            },
            val: {
                min: 0,
                max: 1,
                axisType: "continuous",
                dataType: "numeric"
            }
        }
    };
    _proto.getSimpleShape = function(value) {
        var translatorY = this.state.valueAxis.getTranslator();
        var x = this.state.argumentAxis.getTranslator().translate(value);
        return [x, translatorY.translate(TARGET_MIN_Y), x, translatorY.translate(TARGET_MAX_Y)]
    };
    _proto.pointerHandler = function() {
        this.setState((function(__state_argument) {
            return {
                tooltipVisible: true
            }
        }))
    };
    _proto.pointerOutHandler = function(_ref3) {
        var pageX = _ref3.pageX,
            pageY = _ref3.pageY;
        var _this$state$offsetSta = this.state.offsetState,
            left = _this$state$offsetSta.left,
            top = _this$state$offsetSta.top;
        var x = Math.floor(pageX - left);
        var y = Math.floor(pageY - top);
        if (!inCanvas(this.state.canvasState, x, y)) {
            this.setState((function(__state_argument) {
                return {
                    tooltipVisible: false
                }
            }))
        }
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        _InfernoWrapperCompon.prototype.componentWillUpdate.call(this);
        if (this.props.target !== nextProps.target || this.props.tooltip !== nextProps.tooltip || this.props.value !== nextProps.value) {
            this.__getterCache.tooltipData = void 0
        }
        if (this.state.canvasState !== nextState.canvasState || this.state.offsetState !== nextState.offsetState) {
            this.__getterCache.tooltipCoords = void 0
        }
        if (this.props.endScaleValue !== nextProps.endScaleValue || this.props.startScaleValue !== nextProps.startScaleValue || this.props.target !== nextProps.target || this.props.value !== nextProps.value || this.state.canvasState !== nextState.canvasState || this.props.rtlEnabled !== nextProps.rtlEnabled || this.context.ConfigContext !== context.ConfigContext || this.state.argumentAxis !== nextState.argumentAxis || this.state.valueAxis !== nextState.valueAxis) {
            this.__getterCache.scaleProps = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                canvas: void 0 !== this.props.canvas ? this.props.canvas : this.state.canvas
            }),
            argumentAxis: this.state.argumentAxis,
            valueAxis: this.state.valueAxis,
            canvasState: this.state.canvasState,
            offsetState: this.state.offsetState,
            tooltipVisible: this.state.tooltipVisible,
            widgetRootRef: this.widgetRootRef,
            widgetRef: this.widgetRef,
            tooltipRef: this.tooltipRef,
            config: this.config,
            cssClasses: this.cssClasses,
            cssClassName: this.cssClassName,
            rtlEnabled: this.rtlEnabled,
            tooltipEnabled: this.tooltipEnabled,
            tooltipData: this.tooltipData,
            tooltipCoords: this.tooltipCoords,
            customizedTooltipProps: this.customizedTooltipProps,
            defaultCanvas: this.defaultCanvas,
            scaleProps: this.scaleProps,
            isValidBulletScale: this.isValidBulletScale,
            targetShape: this.targetShape,
            zeroLevelShape: this.zeroLevelShape,
            isValidTarget: this.isValidTarget,
            isValidZeroLevel: this.isValidZeroLevel,
            barValueShape: this.barValueShape,
            onCanvasChange: this.onCanvasChange,
            prepareScaleProps: this.prepareScaleProps,
            getRange: this.getRange,
            getSimpleShape: this.getSimpleShape,
            pointerHandler: this.pointerHandler,
            pointerOutHandler: this.pointerOutHandler,
            restAttributes: this.restAttributes
        })
    };
    _createClass(Bullet, [{
        key: "config",
        get: function() {
            if ("ConfigContext" in this.context) {
                return this.context.ConfigContext
            }
            return _config_context.ConfigContext
        }
    }, {
        key: "cssClasses",
        get: function() {
            var classes = this.props.classes;
            return getCssClasses({
                classes: classes
            })
        }
    }, {
        key: "cssClassName",
        get: function() {
            var className = this.props.className;
            return getContainerCssClasses({
                className: className
            })
        }
    }, {
        key: "rtlEnabled",
        get: function() {
            var rtlEnabled = this.props.rtlEnabled;
            return (0, _resolve_rtl.resolveRtlEnabled)(rtlEnabled, this.config)
        }
    }, {
        key: "tooltipEnabled",
        get: function() {
            return !(void 0 === this.props.value && void 0 === this.props.target)
        }
    }, {
        key: "tooltipData",
        get: function() {
            var _this4 = this;
            if (void 0 !== this.__getterCache.tooltipData) {
                return this.__getterCache.tooltipData
            }
            return this.__getterCache.tooltipData = (_this4$props = _this4.props, target = _this4$props.target, tooltip = _this4$props.tooltip, value = _this4$props.value, valueText = (0, _utils2.getFormatValue)(value, void 0, {
                format: null === tooltip || void 0 === tooltip ? void 0 : tooltip.format
            }), targetText = (0, _utils2.getFormatValue)(target, void 0, {
                format: null === tooltip || void 0 === tooltip ? void 0 : tooltip.format
            }), {
                originalValue: value,
                originalTarget: target,
                value: valueText,
                target: targetText,
                valueTexts: ["Actual Value:", valueText, "Target Value:", targetText]
            });
            var _this4$props, target, tooltip, value, valueText, targetText
        }
    }, {
        key: "tooltipCoords",
        get: function() {
            var _this5 = this;
            if (void 0 !== this.__getterCache.tooltipCoords) {
                return this.__getterCache.tooltipCoords
            }
            return this.__getterCache.tooltipCoords = (canvas = _this5.state.canvasState, rootOffset = _this5.state.offsetState, {
                x: canvas.width / 2 + rootOffset.left,
                y: canvas.height / 2 + rootOffset.top
            });
            var canvas, rootOffset
        }
    }, {
        key: "customizedTooltipProps",
        get: function() {
            var _this$props2 = this.props,
                onTooltipHidden = _this$props2.onTooltipHidden,
                onTooltipShown = _this$props2.onTooltipShown,
                tooltip = _this$props2.tooltip;
            var customProps = _extends({
                enabled: this.tooltipEnabled,
                eventData: {
                    component: this.widgetRef
                },
                onTooltipHidden: onTooltipHidden,
                onTooltipShown: onTooltipShown,
                customizeTooltip: (0, _utils.generateCustomizeTooltipCallback)(tooltip.customizeTooltip, tooltip.font, this.rtlEnabled),
                data: this.tooltipData
            }, this.tooltipCoords);
            return _extends({}, tooltip, customProps, {
                enabled: tooltip.enabled && this.tooltipEnabled
            })
        }
    }, {
        key: "defaultCanvas",
        get: function() {
            return {
                width: DEFAULT_CANVAS_WIDTH,
                height: DEFAULT_CANVAS_HEIGHT,
                left: DEFAULT_HORIZONTAL_MARGIN,
                right: DEFAULT_HORIZONTAL_MARGIN,
                top: DEFAULT_VERTICAL_MARGIN,
                bottom: DEFAULT_VERTICAL_MARGIN
            }
        }
    }, {
        key: "scaleProps",
        get: function() {
            var _this6 = this;
            if (void 0 !== this.__getterCache.scaleProps) {
                return this.__getterCache.scaleProps
            }
            return this.__getterCache.scaleProps = function() {
                var props = _this6.prepareScaleProps();
                var canvas = _this6.state.canvasState;
                var ranges = _this6.getRange(props);
                _this6.state.argumentAxis.update(ranges.arg, canvas, void 0);
                _this6.state.valueAxis.update(ranges.val, canvas, void 0);
                return props
            }()
        }
    }, {
        key: "isValidBulletScale",
        get: function() {
            var _this$props3 = this.props,
                endScaleValue = _this$props3.endScaleValue,
                startScaleValue = _this$props3.startScaleValue,
                target = _this$props3.target,
                value = _this$props3.value;
            var isValidBounds = startScaleValue !== endScaleValue;
            var isValidMin = _number.default.isFinite(startScaleValue);
            var isValidMax = _number.default.isFinite(endScaleValue);
            var isValidValue = _number.default.isFinite(value);
            var isValidTarget = _number.default.isFinite(target);
            return isValidBounds && isValidMax && isValidMin && isValidTarget && isValidValue
        }
    }, {
        key: "targetShape",
        get: function() {
            return this.getSimpleShape(this.scaleProps.target)
        }
    }, {
        key: "zeroLevelShape",
        get: function() {
            return this.getSimpleShape(0)
        }
    }, {
        key: "isValidTarget",
        get: function() {
            var showTarget = this.props.showTarget;
            return !(this.scaleProps.target > this.scaleProps.endScaleValue || this.scaleProps.target < this.scaleProps.startScaleValue || !showTarget)
        }
    }, {
        key: "isValidZeroLevel",
        get: function() {
            var showZeroLevel = this.props.showZeroLevel;
            return !(this.scaleProps.endScaleValue < 0 || this.scaleProps.startScaleValue > 0 || !showZeroLevel)
        }
    }, {
        key: "barValueShape",
        get: function() {
            var translatorX = this.state.argumentAxis.getTranslator();
            var translatorY = this.state.valueAxis.getTranslator();
            var y2 = translatorY.translate(BAR_VALUE_MIN_Y);
            var y1 = translatorY.translate(BAR_VALUE_MAX_Y);
            var x1 = _number.default.NaN;
            var x2 = _number.default.NaN;
            if (this.scaleProps.value > 0) {
                x1 = Math.max(0, this.scaleProps.startScaleValue);
                x2 = this.scaleProps.value >= this.scaleProps.endScaleValue ? this.scaleProps.endScaleValue : Math.max(this.scaleProps.value, x1)
            } else {
                x1 = Math.min(0, this.scaleProps.endScaleValue);
                x2 = this.scaleProps.value < this.scaleProps.startScaleValue ? this.scaleProps.startScaleValue : Math.min(this.scaleProps.value, x1)
            }
            x1 = translatorX.translate(x1);
            x2 = translatorX.translate(x2);
            return [x1, y1, x2, y1, x2, y2, x1, y2]
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props$canvas = _extends({}, this.props, {
                    canvas: void 0 !== this.props.canvas ? this.props.canvas : this.state.canvas
                }),
                restProps = (_this$props$canvas.canvas, _this$props$canvas.canvasChange, _this$props$canvas.children, _this$props$canvas.className, _this$props$canvas.classes, _this$props$canvas.color, _this$props$canvas.defaultCanvas, _this$props$canvas.disabled, _this$props$canvas.endScaleValue, _this$props$canvas.margin, _this$props$canvas.onTooltipHidden, _this$props$canvas.onTooltipShown, _this$props$canvas.pointerEvents, _this$props$canvas.rtlEnabled, _this$props$canvas.showTarget, _this$props$canvas.showZeroLevel, _this$props$canvas.size, _this$props$canvas.startScaleValue, _this$props$canvas.target, _this$props$canvas.targetColor, _this$props$canvas.targetWidth, _this$props$canvas.tooltip, _this$props$canvas.value, _objectWithoutProperties(_this$props$canvas, _excluded));
            return restProps
        }
    }]);
    return Bullet
}(_inferno2.InfernoWrapperComponent);
exports.Bullet = Bullet;
Bullet.defaultProps = BulletProps;
