/**
 * DevExtreme (esm/renovation/ui/scheduler/appointment_tooltip/item_content.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["className", "formattedDate", "text"];
import {
    createVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
export var viewFunction = viewModel => normalizeProps(createVNode(1, "div", "dx-tooltip-appointment-item-content ".concat(viewModel.props.className), [createVNode(1, "div", "dx-tooltip-appointment-item-content-subject", viewModel.props.text, 0), createVNode(1, "div", "dx-tooltip-appointment-item-content-date", viewModel.props.formattedDate, 0)], 4, _extends({}, viewModel.restAttributes)));
export var TooltipItemContentProps = {
    className: "",
    text: "",
    formattedDate: ""
};
export class TooltipItemContent extends BaseInfernoComponent {
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
TooltipItemContent.defaultProps = TooltipItemContentProps;
