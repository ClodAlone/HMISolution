/**
 * DevExtreme (esm/renovation/viz/common/base_widget.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["canvas", "canvasChange", "children", "className", "classes", "defaultCanvas", "disabled", "margin", "pointerEvents", "rootElementRef", "rtlEnabled", "size"];
import {
    createVNode,
    createFragment,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    Fragment
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import {
    isDefined
} from "../../../core/utils/type";
import {
    combineClasses
} from "../../utils/combine_classes";
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
    RootSvgElement
} from "./renderers/svg_root";
import {
    GrayScaleFilter
} from "./renderers/gray_scale_filter";
import {
    sizeIsValid,
    pickPositiveValue,
    getElementWidth,
    getElementHeight,
    isUpdatedFlatObject
} from "./utils";
import {
    resolveRtlEnabled,
    resolveRtlEnabledDefinition
} from "../../utils/resolve_rtl";
import {
    getNextDefsSvgId,
    getFuncIri
} from "./renderers/utils";
var DEFAULT_CANVAS = {
    left: 0,
    top: 0,
    right: 0,
    bottom: 0,
    width: 0,
    height: 0
};
var getCssClasses = model => {
    var containerClassesMap = {
        "dx-widget": true,
        "dx-visibility-change-handler": true,
        [String(model.className)]: !!model.className
    };
    return combineClasses(containerClassesMap)
};
var calculateCanvas = model => {
    var _model$size, _model$margin, _model$defaultCanvas;
    var {
        height: height,
        width: width
    } = null !== (_model$size = model.size) && void 0 !== _model$size ? _model$size : {};
    var margin = null !== (_model$margin = model.margin) && void 0 !== _model$margin ? _model$margin : {};
    var defaultCanvas = null !== (_model$defaultCanvas = model.defaultCanvas) && void 0 !== _model$defaultCanvas ? _model$defaultCanvas : DEFAULT_CANVAS;
    var elementWidth = !sizeIsValid(width) ? getElementWidth(model.element) : 0;
    var elementHeight = !sizeIsValid(height) ? getElementHeight(model.element) : 0;
    var canvas = {
        width: width && width <= 0 ? 0 : Math.floor(pickPositiveValue([width, elementWidth, defaultCanvas.width])),
        height: height && height <= 0 ? 0 : Math.floor(pickPositiveValue([height, elementHeight, defaultCanvas.height])),
        left: pickPositiveValue([margin.left, defaultCanvas.left]),
        top: pickPositiveValue([margin.top, defaultCanvas.top]),
        right: pickPositiveValue([margin.right, defaultCanvas.right]),
        bottom: pickPositiveValue([margin.bottom, defaultCanvas.bottom])
    };
    if (canvas.width - canvas.left - canvas.right <= 0 || canvas.height - canvas.top - canvas.bottom <= 0) {
        return _extends({}, defaultCanvas)
    }
    return canvas
};
export var viewFunction = viewModel => {
    var grayFilterId = viewModel.props.disabled ? getNextDefsSvgId() : void 0;
    var canvas = viewModel.props.canvas || DEFAULT_CANVAS;
    var widget = normalizeProps(createVNode(1, "div", viewModel.cssClasses, createComponentVNode(2, RootSvgElement, {
        rootElementRef: viewModel.svgElementRef,
        className: viewModel.props.classes,
        width: canvas.width,
        height: canvas.height,
        pointerEvents: viewModel.pointerEventsState,
        filter: grayFilterId ? getFuncIri(grayFilterId) : void 0,
        children: createFragment([createVNode(32, "defs", null, grayFilterId && createComponentVNode(2, GrayScaleFilter, {
            id: grayFilterId
        }), 0), viewModel.props.children], 0)
    }), 2, _extends({}, viewModel.restAttributes), null, viewModel.containerRef));
    return viewModel.shouldRenderConfigProvider ? createComponentVNode(2, ConfigProvider, {
        rtlEnabled: viewModel.rtlEnabled,
        children: widget
    }) : widget
};
export var Props = BaseWidgetProps;
import {
    createRef as infernoCreateRef
} from "inferno";
export class BaseWidget extends InfernoComponent {
    constructor(props) {
        super(props);
        this.containerRef = infernoCreateRef();
        this.svgElementRef = infernoCreateRef();
        this.state = {
            canvas: void 0 !== this.props.canvas ? this.props.canvas : this.props.defaultCanvas
        };
        this.setRootElementRef = this.setRootElementRef.bind(this);
        this.setCanvasEffect = this.setCanvasEffect.bind(this);
        this.svg = this.svg.bind(this);
        this.setCanvas = this.setCanvas.bind(this)
    }
    get config() {
        if ("ConfigContext" in this.context) {
            return this.context.ConfigContext
        }
        return ConfigContext
    }
    createEffects() {
        return [new InfernoEffect(this.setRootElementRef, []), new InfernoEffect(this.setCanvasEffect, [this.state.canvas, this.props.canvas, this.props.defaultCanvas, this.props.margin, this.props.size, this.props.canvasChange])]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[1]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.state.canvas, this.props.canvas, this.props.defaultCanvas, this.props.margin, this.props.size, this.props.canvasChange])
    }
    setRootElementRef() {
        this.props.rootElementRef.current = this.containerRef.current
    }
    setCanvasEffect() {
        this.setCanvas()
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
    get pointerEventsState() {
        var {
            disabled: disabled,
            pointerEvents: pointerEvents
        } = this.props;
        return disabled ? "none" : pointerEvents
    }
    get cssClasses() {
        var {
            className: className
        } = this.props;
        return getCssClasses({
            className: className
        })
    }
    setCanvas() {
        var {
            defaultCanvas: defaultCanvas,
            margin: margin,
            size: size
        } = this.props;
        var newCanvas = calculateCanvas({
            element: this.containerRef.current,
            defaultCanvas: defaultCanvas,
            size: size,
            margin: margin
        });
        if (isDefined(newCanvas.height) && isDefined(newCanvas.width) && isUpdatedFlatObject(void 0 !== this.props.canvas ? this.props.canvas : this.state.canvas, newCanvas)) {
            var __newValue;
            this.setState(__state_argument => {
                __newValue = newCanvas;
                return {
                    canvas: __newValue
                }
            });
            this.props.canvasChange(__newValue)
        }
    }
    get restAttributes() {
        var _this$props$canvas = _extends({}, this.props, {
                canvas: void 0 !== this.props.canvas ? this.props.canvas : this.state.canvas
            }),
            restProps = _objectWithoutPropertiesLoose(_this$props$canvas, _excluded);
        return restProps
    }
    svg() {
        return this.svgElementRef.current
    }
    render() {
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
    }
}
BaseWidget.defaultProps = Props;
