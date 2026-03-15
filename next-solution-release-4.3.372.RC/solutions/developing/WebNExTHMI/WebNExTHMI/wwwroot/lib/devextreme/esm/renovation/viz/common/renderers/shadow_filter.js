/**
 * DevExtreme (esm/renovation/viz/common/renderers/shadow_filter.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["blur", "color", "height", "id", "offsetX", "offsetY", "opacity", "width", "x", "y"];
import {
    createVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
export var viewFunction = _ref => {
    var {
        props: {
            blur: blur,
            color: color,
            height: height,
            id: id,
            offsetX: offsetX,
            offsetY: offsetY,
            opacity: opacity,
            width: width,
            x: x,
            y: y
        }
    } = _ref;
    return createVNode(32, "filter", null, [createVNode(32, "feGaussianBlur", null, null, 1, {
        in: "SourceGraphic",
        result: "gaussianBlurResult",
        stdDeviation: blur
    }), createVNode(32, "feOffset", null, null, 1, {
        in: "gaussianBlurResult",
        result: "offsetResult",
        dx: offsetX,
        dy: offsetY
    }), createVNode(32, "feFlood", null, null, 1, {
        result: "floodResult",
        "flood-color": color,
        "flood-opacity": opacity
    }), createVNode(32, "feComposite", null, null, 1, {
        in: "floodResult",
        in2: "offsetResult",
        operator: "in",
        result: "compositeResult"
    }), createVNode(32, "feComposite", null, null, 1, {
        in: "SourceGraphic",
        in2: "compositeResult",
        operator: "over"
    })], 4, {
        id: id,
        x: x,
        y: y,
        width: width,
        height: height
    })
};
export var ShadowFilterProps = {
    x: 0,
    y: 0,
    width: 0,
    height: 0,
    offsetX: 0,
    offsetY: 0,
    blur: 0,
    color: ""
};
export class ShadowFilter extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
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
            restAttributes: this.restAttributes
        })
    }
}
ShadowFilter.defaultProps = ShadowFilterProps;
