/**
 * DevExtreme (esm/renovation/ui/form/layout_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["screenByWidth"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    combineClasses
} from "../../utils/combine_classes";
import {
    Widget
} from "../common/widget";
import {
    ResponsiveBox
} from "../responsive_box/responsive_box";
import {
    LayoutManagerProps
} from "./layout_manager_props";
export var viewFunction = viewModel => {
    var {
        cssClasses: cssClasses,
        restAttributes: restAttributes
    } = viewModel;
    return normalizeProps(createComponentVNode(2, Widget, _extends({
        classes: cssClasses
    }, restAttributes, {
        children: createComponentVNode(2, ResponsiveBox, {
            screenByWidth: viewModel.props.screenByWidth
        })
    })))
};
export class LayoutManager extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get cssClasses() {
        return combineClasses({
            "dx-layout-manager": true
        })
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
            cssClasses: this.cssClasses,
            restAttributes: this.restAttributes
        })
    }
}
LayoutManager.defaultProps = LayoutManagerProps;
