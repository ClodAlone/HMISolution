/**
 * DevExtreme (esm/renovation/viz/common/renderers/svg_path.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["className", "d", "dashStyle", "fill", "opacity", "pointerEvents", "points", "rotate", "rotateX", "rotateY", "scaleX", "scaleY", "sharp", "sharpDirection", "stroke", "strokeLineCap", "strokeOpacity", "strokeWidth", "translateX", "translateY", "type"];
import {
    createVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import SvgGraphicsProps from "./base_graphics_props";
import {
    combinePathParam,
    buildPathSegments,
    getGraphicExtraProps
} from "./utils";
export var viewFunction = _ref => {
    var {
        computedProps: computedProps,
        d: d,
        pathRef: pathRef
    } = _ref;
    var {
        className: className,
        fill: fill,
        opacity: opacity,
        pointerEvents: pointerEvents,
        stroke: stroke,
        strokeLineCap: strokeLineCap,
        strokeOpacity: strokeOpacity,
        strokeWidth: strokeWidth
    } = computedProps;
    return normalizeProps(createVNode(32, "path", className, null, 1, _extends({
        d: d,
        fill: fill,
        stroke: stroke,
        "stroke-width": strokeWidth,
        "stroke-opacity": strokeOpacity,
        "stroke-linecap": strokeLineCap,
        opacity: opacity,
        "pointer-events": pointerEvents
    }, getGraphicExtraProps(computedProps)), null, pathRef))
};
export var PathSvgElementProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(SvgGraphicsProps), Object.getOwnPropertyDescriptors({
    type: "line",
    d: ""
})));
import {
    createRef as infernoCreateRef
} from "inferno";
export class PathSvgElement extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.pathRef = infernoCreateRef()
    }
    get d() {
        var _this$props$points;
        var path = this.props.d;
        var segments = [];
        if (null !== (_this$props$points = this.props.points) && void 0 !== _this$props$points && _this$props$points.length) {
            segments = buildPathSegments(this.props.points, this.props.type);
            segments && (path = combinePathParam(segments))
        }
        return path
    }
    get computedProps() {
        return this.props
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            pathRef: this.pathRef,
            d: this.d,
            computedProps: this.computedProps,
            restAttributes: this.restAttributes
        })
    }
}
PathSvgElement.defaultProps = PathSvgElementProps;
