/**
 * DevExtreme (esm/renovation/viz/common/renderers/gray_scale_filter.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["id"];
import {
    createVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
export var viewFunction = _ref => {
    var {
        props: {
            id: id
        }
    } = _ref;
    return createVNode(32, "filter", null, createVNode(32, "feColorMatrix", null, null, 1, {
        type: "matrix",
        values: "0.3333 0.3333 0.3333 0 0 0.3333 0.3333 0.3333 0 0 0.3333 0.3333 0.3333 0 0 0 0 0 0.6 0"
    }), 2, {
        id: id
    })
};
export var GrayScaleFilterProps = {};
export class GrayScaleFilter extends BaseInfernoComponent {
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
GrayScaleFilter.defaultProps = GrayScaleFilterProps;
