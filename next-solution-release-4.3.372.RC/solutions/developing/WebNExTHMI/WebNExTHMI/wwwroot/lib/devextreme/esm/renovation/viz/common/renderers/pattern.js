/**
 * DevExtreme (esm/renovation/viz/common/renderers/pattern.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["color", "hatching", "id"];
import {
    createVNode,
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    RectSvgElement
} from "./svg_rect";
import {
    PathSvgElement
} from "./svg_path";
import {
    normalizeEnum
} from "../../../../viz/core/utils";
export var viewFunction = _ref => {
    var {
        d: d,
        props: {
            color: color,
            hatching: hatching,
            id: id
        },
        step: step
    } = _ref;
    return createVNode(32, "pattern", null, [createComponentVNode(2, RectSvgElement, {
        x: 0,
        y: 0,
        width: step,
        height: step,
        fill: color,
        opacity: null === hatching || void 0 === hatching ? void 0 : hatching.opacity
    }), createComponentVNode(2, PathSvgElement, {
        d: d,
        strokeWidth: Number(null === hatching || void 0 === hatching ? void 0 : hatching.width) || 1,
        stroke: color
    })], 4, {
        id: id,
        width: step,
        height: step,
        patternUnits: "userSpaceOnUse"
    })
};
export var SvgPatternProps = {
    color: ""
};
export class SvgPattern extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get step() {
        var _this$props$hatching;
        return Number(null === (_this$props$hatching = this.props.hatching) || void 0 === _this$props$hatching ? void 0 : _this$props$hatching.step) || 6
    }
    get d() {
        var _this$props$hatching2;
        var stepTo2 = this.step / 2;
        var stepBy15 = 1.5 * this.step;
        return "right" === normalizeEnum(null === (_this$props$hatching2 = this.props.hatching) || void 0 === _this$props$hatching2 ? void 0 : _this$props$hatching2.direction) ? "M ".concat(stepTo2, " ").concat(-stepTo2, " L ").concat(-stepTo2, " ").concat(stepTo2, " M 0 ").concat(this.step, " L ").concat(this.step, " 0 M ").concat(stepBy15, " ").concat(stepTo2, " L ").concat(stepTo2, " ").concat(stepBy15) : "M 0 0 L ".concat(this.step, " ").concat(this.step, " M ").concat(-stepTo2, " ").concat(stepTo2, " L ").concat(stepTo2, " ").concat(stepBy15, " M ").concat(stepTo2, " ").concat(-stepTo2, " L ").concat(stepBy15, " ").concat(stepTo2)
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
            step: this.step,
            d: this.d,
            restAttributes: this.restAttributes
        })
    }
}
SvgPattern.defaultProps = SvgPatternProps;
