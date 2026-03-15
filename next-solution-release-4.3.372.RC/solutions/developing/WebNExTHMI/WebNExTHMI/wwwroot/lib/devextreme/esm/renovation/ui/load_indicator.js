/**
 * DevExtreme (esm/renovation/ui/load_indicator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["accessKey", "activeStateEnabled", "className", "disabled", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "indicatorSrc", "onClick", "onKeyDown", "rtlEnabled", "tabIndex", "visible", "width"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import LegacyLoadIndicator from "../../ui/load_indicator";
import {
    DomComponentWrapper
} from "./common/dom_component_wrapper";
import {
    BaseWidgetProps
} from "./common/base_props";
export var viewFunction = _ref => {
    var {
        props: props,
        restAttributes: restAttributes
    } = _ref;
    return normalizeProps(createComponentVNode(2, DomComponentWrapper, _extends({
        componentType: LegacyLoadIndicator,
        componentProps: props,
        templateNames: []
    }, restAttributes)))
};
export var LoadIndicatorProps = BaseWidgetProps;
export class LoadIndicator extends BaseInfernoComponent {
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
LoadIndicator.defaultProps = LoadIndicatorProps;
