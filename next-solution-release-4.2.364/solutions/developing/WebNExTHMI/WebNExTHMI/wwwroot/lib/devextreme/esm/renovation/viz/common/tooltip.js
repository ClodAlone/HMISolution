/**
 * DevExtreme (esm/renovation/viz/common/tooltip.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["argumentFormat", "arrowLength", "arrowWidth", "border", "className", "color", "container", "contentTemplate", "cornerRadius", "customizeTooltip", "data", "enabled", "eventData", "font", "format", "interactive", "location", "offset", "onTooltipHidden", "onTooltipShown", "opacity", "paddingLeftRight", "paddingTopBottom", "rootWidget", "rtl", "shadow", "shared", "visible", "x", "y", "zIndex"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    Portal,
    InfernoEffect,
    InfernoComponent,
    normalizeStyles
} from "@devextreme/runtime/inferno";
import {
    combineClasses
} from "../../utils/combine_classes";
import {
    PathSvgElement
} from "./renderers/svg_path";
import {
    TextSvgElement
} from "./renderers/svg_text";
import {
    ShadowFilter
} from "./renderers/shadow_filter";
import {
    getNextDefsSvgId,
    getFuncIri
} from "./renderers/utils";
import {
    RootSvgElement
} from "./renderers/svg_root";
import {
    isDefined
} from "../../../core/utils/type";
import {
    getCloudPoints,
    recalculateCoordinates,
    getCloudAngle,
    prepareData,
    getCanvas,
    isTextEmpty
} from "./tooltip_utils";
import {
    normalizeEnum
} from "../../../viz/core/utils";
import domAdapter from "../../../core/dom_adapter";
import {
    isUpdatedFlatObject
} from "./utils";
var DEFAULT_CANVAS = {
    left: 0,
    top: 0,
    right: 0,
    bottom: 0,
    width: 0,
    height: 0
};
var DEFAULT_FONT = {
    color: "#232323",
    family: "Segoe UI",
    opacity: 1,
    size: 12,
    weight: 400
};
var DEFAULT_SHADOW = {
    blur: 2,
    color: "#000",
    offsetX: 0,
    offsetY: 4,
    opacity: .4
};
var DEFAULT_BORDER = {
    color: "#d3d3d3",
    width: 1,
    dashStyle: "solid",
    visible: true
};
var DEFAULT_SIZE = {
    x: 0,
    y: 0,
    width: 0,
    height: 0
};
export var viewFunction = _ref => {
    var {
        border: border,
        cloudRef: cloudRef,
        cloudSize: cloudSize,
        container: container,
        correctedCoordinates: correctedCoordinates,
        cssClassName: cssClassName,
        customizedOptions: customizedOptions,
        filterId: filterId,
        htmlRef: htmlRef,
        isEmptyContainer: isEmptyContainer,
        pointerEvents: pointerEvents,
        props: {
            arrowWidth: arrowWidth,
            contentTemplate: TooltipTemplate,
            cornerRadius: cornerRadius,
            data: data,
            font: font,
            interactive: interactive,
            opacity: opacity,
            rtl: rtl,
            shadow: shadow,
            visible: visible,
            zIndex: zIndex
        },
        textRef: textRef,
        textSize: textSize,
        textSizeWithPaddings: textSizeWithPaddings,
        textSvgElementStyles: textSvgElementStyles
    } = _ref;
    if (!visible || !correctedCoordinates || isTextEmpty(customizedOptions) || isEmptyContainer) {
        return createVNode(1, "div")
    }
    var angle = getCloudAngle(textSizeWithPaddings, correctedCoordinates);
    var d = getCloudPoints(textSizeWithPaddings, correctedCoordinates, angle, {
        cornerRadius: cornerRadius,
        arrowWidth: arrowWidth
    }, true);
    var styles = interactive ? {
        msUserSelect: "text",
        MozUserSelect: "auto",
        WebkitUserSelect: "auto"
    } : {};
    styles = _extends({}, styles, {
        position: "absolute"
    });
    return createComponentVNode(2, Portal, {
        container: container,
        children: createVNode(1, "div", cssClassName, [createComponentVNode(2, RootSvgElement, {
            width: cloudSize.width,
            height: cloudSize.height,
            styles: styles,
            children: [createVNode(32, "defs", null, createComponentVNode(2, ShadowFilter, {
                id: filterId,
                x: "-50%",
                y: "-50%",
                width: "200%",
                height: "200%",
                blur: shadow.blur,
                color: shadow.color,
                offsetX: shadow.offsetX,
                offsetY: shadow.offsetY,
                opacity: shadow.opacity
            }), 2), createVNode(32, "g", null, [createComponentVNode(2, PathSvgElement, {
                pointerEvents: pointerEvents,
                d: d,
                fill: customizedOptions.color,
                stroke: customizedOptions.borderColor,
                strokeWidth: border.strokeWidth,
                strokeOpacity: border.strokeOpacity,
                dashStyle: border.dashStyle,
                opacity: opacity,
                rotate: angle,
                rotateX: correctedCoordinates.x,
                rotateY: correctedCoordinates.y
            }), customizedOptions.html || TooltipTemplate ? null : createVNode(32, "g", null, createComponentVNode(2, TextSvgElement, {
                text: customizedOptions.text,
                styles: textSvgElementStyles
            }), 2, {
                "text-anchor": "middle",
                transform: "translate(".concat(correctedCoordinates.x, ", ").concat(correctedCoordinates.y - textSize.height / 2 - textSize.y, ")")
            }, null, textRef)], 0, {
                filter: getFuncIri(filterId),
                transform: "translate(".concat(-cloudSize.x, ", ").concat(-cloudSize.y, ")")
            }, null, cloudRef)]
        }), !(customizedOptions.html || TooltipTemplate) ? null : createVNode(1, "div", null, TooltipTemplate && TooltipTemplate(_extends({}, data)), 0, {
            style: normalizeStyles({
                position: "relative",
                display: "inline-block",
                left: correctedCoordinates.x - cloudSize.x - textSize.width / 2,
                top: correctedCoordinates.y - cloudSize.y - textSize.height / 2,
                fill: customizedOptions.fontColor,
                fontFamily: font.family,
                fontSize: font.size,
                fontWeight: font.weight,
                opacity: font.opacity,
                pointerEvents: pointerEvents,
                direction: rtl ? "rtl" : "ltr"
            })
        }, null, htmlRef)], 0, {
            style: normalizeStyles({
                position: "absolute",
                pointerEvents: "none",
                left: cloudSize.x,
                top: cloudSize.y,
                zIndex: zIndex
            })
        })
    })
};
export var TooltipProps = {
    color: "#fff",
    border: DEFAULT_BORDER,
    get data() {
        return {}
    },
    paddingLeftRight: 18,
    paddingTopBottom: 15,
    x: 0,
    y: 0,
    cornerRadius: 0,
    arrowWidth: 20,
    arrowLength: 10,
    offset: 0,
    font: DEFAULT_FONT,
    shadow: DEFAULT_SHADOW,
    interactive: false,
    enabled: true,
    shared: false,
    location: "center",
    visible: false,
    rtl: false
};
import {
    createRef as infernoCreateRef
} from "inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class Tooltip extends InfernoComponent {
    constructor(props) {
        super(props);
        this.cloudRef = infernoCreateRef();
        this.textRef = infernoCreateRef();
        this.htmlRef = infernoCreateRef();
        this.__getterCache = {};
        this.state = {
            filterId: getNextDefsSvgId(),
            textSize: DEFAULT_SIZE,
            cloudSize: DEFAULT_SIZE,
            currentEventData: void 0,
            isEmptyContainer: false,
            canvas: DEFAULT_CANVAS
        };
        this.setHtmlText = this.setHtmlText.bind(this);
        this.calculateSize = this.calculateSize.bind(this);
        this.eventsEffect = this.eventsEffect.bind(this);
        this.checkContainer = this.checkContainer.bind(this);
        this.setCanvas = this.setCanvas.bind(this);
        this.getLocation = this.getLocation.bind(this);
        this.calculateContentSize = this.calculateContentSize.bind(this);
        this.calculateCloudSize = this.calculateCloudSize.bind(this)
    }
    createEffects() {
        var _this$props$rootWidge;
        return [new InfernoEffect(this.setHtmlText, [this.props.border, this.props.color, this.props.customizeTooltip, this.props.data, this.props.font, this.props.visible]), new InfernoEffect(this.calculateSize, [this.props.visible, this.props.x, this.props.y, this.props.shadow, this.state.textSize, this.state.cloudSize]), new InfernoEffect(this.eventsEffect, [this.props.eventData, this.props.onTooltipHidden, this.props.onTooltipShown, this.props.visible, this.props.arrowLength, this.props.offset, this.props.x, this.props.y, this.state.canvas, this.props.paddingLeftRight, this.props.paddingTopBottom, this.state.textSize, this.state.currentEventData]), new InfernoEffect(this.checkContainer, [this.props.visible]), new InfernoEffect(this.setCanvas, [this.props.container, null === (_this$props$rootWidge = this.props.rootWidget) || void 0 === _this$props$rootWidge ? void 0 : _this$props$rootWidge.current])]
    }
    updateEffects() {
        var _this$_effects$, _this$_effects$2, _this$_effects$3, _this$_effects$4, _this$_effects$5, _this$props$rootWidge2;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.border, this.props.color, this.props.customizeTooltip, this.props.data, this.props.font, this.props.visible]);
        null === (_this$_effects$2 = this._effects[1]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props.visible, this.props.x, this.props.y, this.props.shadow, this.state.textSize, this.state.cloudSize]);
        null === (_this$_effects$3 = this._effects[2]) || void 0 === _this$_effects$3 ? void 0 : _this$_effects$3.update([this.props.eventData, this.props.onTooltipHidden, this.props.onTooltipShown, this.props.visible, this.props.arrowLength, this.props.offset, this.props.x, this.props.y, this.state.canvas, this.props.paddingLeftRight, this.props.paddingTopBottom, this.state.textSize, this.state.currentEventData]);
        null === (_this$_effects$4 = this._effects[3]) || void 0 === _this$_effects$4 ? void 0 : _this$_effects$4.update([this.props.visible]);
        null === (_this$_effects$5 = this._effects[4]) || void 0 === _this$_effects$5 ? void 0 : _this$_effects$5.update([this.props.container, null === (_this$props$rootWidge2 = this.props.rootWidget) || void 0 === _this$props$rootWidge2 ? void 0 : _this$props$rootWidge2.current])
    }
    setHtmlText() {
        var htmlText = this.customizedOptions.html;
        if (htmlText && this.htmlRef.current && this.props.visible) {
            this.htmlRef.current.innerHTML = htmlText
        }
    }
    calculateSize() {
        var contentSize = this.calculateContentSize();
        var cloudSize = this.calculateCloudSize();
        if (isUpdatedFlatObject(this.state.textSize, contentSize)) {
            this.setState(__state_argument => ({
                textSize: contentSize
            }))
        }
        if (isUpdatedFlatObject(this.state.cloudSize, cloudSize)) {
            this.setState(__state_argument => ({
                cloudSize: cloudSize
            }))
        }
    }
    eventsEffect() {
        var {
            eventData: eventData,
            onTooltipHidden: onTooltipHidden,
            onTooltipShown: onTooltipShown,
            visible: visible
        } = this.props;
        if (visible && this.correctedCoordinates && !((object1, object2) => {
                if (!object1) {
                    return false
                }
                return JSON.stringify(object1.target) === JSON.stringify(object2.target)
            })(this.state.currentEventData, eventData)) {
            this.state.currentEventData && (null === onTooltipHidden || void 0 === onTooltipHidden ? void 0 : onTooltipHidden(this.state.currentEventData));
            null === onTooltipShown || void 0 === onTooltipShown ? void 0 : onTooltipShown(eventData);
            this.setState(__state_argument => ({
                currentEventData: eventData
            }))
        }
        if (!visible && this.state.currentEventData) {
            null === onTooltipHidden || void 0 === onTooltipHidden ? void 0 : onTooltipHidden(this.state.currentEventData);
            this.setState(__state_argument => ({
                currentEventData: void 0
            }))
        }
    }
    checkContainer() {
        if (this.htmlRef.current && this.props.visible) {
            var htmlTextSize = this.htmlRef.current.getBoundingClientRect();
            if (!htmlTextSize.width && !htmlTextSize.height) {
                this.setState(__state_argument => ({
                    isEmptyContainer: true
                }))
            }
        }
    }
    setCanvas() {
        this.setState(__state_argument => ({
            canvas: getCanvas(this.container)
        }))
    }
    get textSvgElementStyles() {
        return _extends({}, this.fontStyles, {
            pointerEvents: this.pointerEvents
        })
    }
    get textSizeWithPaddings() {
        var {
            paddingLeftRight: paddingLeftRight,
            paddingTopBottom: paddingTopBottom
        } = this.props;
        return {
            width: this.state.textSize.width + 2 * paddingLeftRight,
            height: this.state.textSize.height + 2 * paddingTopBottom
        }
    }
    get border() {
        if (void 0 !== this.__getterCache.border) {
            return this.__getterCache.border
        }
        return this.__getterCache.border = (() => {
            var {
                border: border
            } = this.props;
            if (border.visible) {
                return {
                    stroke: border.color,
                    strokeWidth: border.width,
                    strokeOpacity: border.opacity,
                    dashStyle: border.dashStyle
                }
            }
            return {}
        })()
    }
    get container() {
        var propsContainer = this.props.container;
        if (propsContainer) {
            if ("string" === typeof propsContainer) {
                var _this$props$rootWidge3;
                var tmp = null === (_this$props$rootWidge3 = this.props.rootWidget) || void 0 === _this$props$rootWidge3 ? void 0 : _this$props$rootWidge3.current;
                var node = null === tmp || void 0 === tmp ? void 0 : tmp.closest(propsContainer);
                if (!node) {
                    node = domAdapter.getDocument().querySelector(propsContainer)
                }
                if (node) {
                    return node
                }
            } else {
                return propsContainer
            }
        }
        return domAdapter.getBody()
    }
    get customizedOptions() {
        if (void 0 !== this.__getterCache.customizedOptions) {
            return this.__getterCache.customizedOptions
        }
        return this.__getterCache.customizedOptions = (() => {
            var {
                border: border,
                color: color,
                customizeTooltip: customizeTooltip,
                data: data,
                font: font
            } = this.props;
            return prepareData(data, color, border, font, customizeTooltip)
        })()
    }
    get margins() {
        if (void 0 !== this.__getterCache.margins) {
            return this.__getterCache.margins
        }
        return this.__getterCache.margins = (() => {
            var {
                max: max
            } = Math;
            var {
                shadow: shadow
            } = this.props;
            var xOff = shadow.offsetX;
            var yOff = shadow.offsetY;
            var blur = 2 * shadow.blur + 1;
            return {
                lm: max(blur - xOff, 0),
                rm: max(blur + xOff, 0),
                tm: max(blur - yOff, 0),
                bm: max(blur + yOff, 0)
            }
        })()
    }
    get pointerEvents() {
        var {
            interactive: interactive
        } = this.props;
        return interactive ? "auto" : "none"
    }
    get cssClassName() {
        var {
            className: className
        } = this.props;
        var classesMap = {
            [String(className)]: !!className
        };
        return combineClasses(classesMap)
    }
    get fontStyles() {
        var {
            font: font
        } = this.props;
        var result = {};
        void 0 !== font.family && (result.fontFamily = font.family);
        void 0 !== font.size && (result.fontSize = String(font.size));
        void 0 !== font.weight && (result.fontWeight = String(font.weight));
        void 0 !== font.opacity && (result.opacity = String(font.opacity));
        void 0 !== this.customizedOptions.fontColor && (result.fill = this.customizedOptions.fontColor);
        return result
    }
    get correctedCoordinates() {
        if (void 0 !== this.__getterCache.correctedCoordinates) {
            return this.__getterCache.correctedCoordinates
        }
        return this.__getterCache.correctedCoordinates = (() => {
            var {
                arrowLength: arrowLength,
                offset: offset,
                x: x,
                y: y
            } = this.props;
            return recalculateCoordinates({
                canvas: this.state.canvas,
                anchorX: x,
                anchorY: y,
                size: this.textSizeWithPaddings,
                offset: offset,
                arrowLength: arrowLength
            })
        })()
    }
    calculateContentSize() {
        var size = DEFAULT_SIZE;
        if (this.props.visible) {
            if (this.textRef.current) {
                size = this.textRef.current.getBBox()
            } else if (this.htmlRef.current) {
                size = this.htmlRef.current.getBoundingClientRect()
            }
        }
        return size
    }
    calculateCloudSize() {
        var cloudSize = DEFAULT_SIZE;
        if (isDefined(this.props.x) && isDefined(this.props.y) && this.props.visible && this.cloudRef.current) {
            var size = this.cloudRef.current.getBBox();
            var {
                bm: bm,
                lm: lm,
                rm: rm,
                tm: tm
            } = this.margins;
            cloudSize = {
                x: Math.floor(size.x - lm),
                y: Math.floor(size.y - tm),
                width: size.width + lm + rm,
                height: size.height + tm + bm
            }
        }
        return cloudSize
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    getLocation() {
        return normalizeEnum(this.props.location)
    }
    componentWillUpdate(nextProps, nextState, context) {
        super.componentWillUpdate();
        if (this.props.border !== nextProps.border) {
            this.__getterCache.border = void 0
        }
        if (this.props.border !== nextProps.border || this.props.color !== nextProps.color || this.props.customizeTooltip !== nextProps.customizeTooltip || this.props.data !== nextProps.data || this.props.font !== nextProps.font) {
            this.__getterCache.customizedOptions = void 0
        }
        if (this.props.shadow !== nextProps.shadow) {
            this.__getterCache.margins = void 0
        }
        if (this.props.arrowLength !== nextProps.arrowLength || this.props.offset !== nextProps.offset || this.props.x !== nextProps.x || this.props.y !== nextProps.y || this.state.canvas !== nextState.canvas || this.props.paddingLeftRight !== nextProps.paddingLeftRight || this.props.paddingTopBottom !== nextProps.paddingTopBottom || this.state.textSize !== nextState.textSize) {
            this.__getterCache.correctedCoordinates = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                contentTemplate: getTemplate(props.contentTemplate)
            }),
            filterId: this.state.filterId,
            textSize: this.state.textSize,
            cloudSize: this.state.cloudSize,
            currentEventData: this.state.currentEventData,
            isEmptyContainer: this.state.isEmptyContainer,
            canvas: this.state.canvas,
            cloudRef: this.cloudRef,
            textRef: this.textRef,
            htmlRef: this.htmlRef,
            textSvgElementStyles: this.textSvgElementStyles,
            textSizeWithPaddings: this.textSizeWithPaddings,
            border: this.border,
            container: this.container,
            customizedOptions: this.customizedOptions,
            margins: this.margins,
            pointerEvents: this.pointerEvents,
            cssClassName: this.cssClassName,
            fontStyles: this.fontStyles,
            correctedCoordinates: this.correctedCoordinates,
            calculateContentSize: this.calculateContentSize,
            calculateCloudSize: this.calculateCloudSize,
            restAttributes: this.restAttributes
        })
    }
}
Tooltip.defaultProps = TooltipProps;
