/**
 * DevExtreme (esm/renovation/ui/editors/drop_down_editors/select_box.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["accessKey", "activeStateEnabled", "className", "dataSource", "defaultValue", "disabled", "displayExpr", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "onClick", "onKeyDown", "rtlEnabled", "tabIndex", "value", "valueChange", "valueExpr", "visible", "width"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import LegacySelectBox from "../../../../ui/select_box";
import {
    DomComponentWrapper
} from "../../common/dom_component_wrapper";
import {
    BaseWidgetProps
} from "../../common/base_props";
export var viewFunction = _ref => {
    var {
        props: props,
        restAttributes: restAttributes
    } = _ref;
    return normalizeProps(createComponentVNode(2, DomComponentWrapper, _extends({
        componentType: LegacySelectBox,
        componentProps: props,
        templateNames: ["dropDownButtonTemplate", "groupTemplate", "itemTemplate"]
    }, restAttributes)))
};
export var SelectBoxProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseWidgetProps), Object.getOwnPropertyDescriptors({
    focusStateEnabled: true,
    hoverStateEnabled: true,
    defaultValue: null,
    isReactComponentWrapper: true
})));
export class SelectBox extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {
            value: void 0 !== this.props.value ? this.props.value : this.props.defaultValue
        }
    }
    get restAttributes() {
        var _this$props$value = _extends({}, this.props, {
                value: void 0 !== this.props.value ? this.props.value : this.state.value
            }),
            restProps = _objectWithoutPropertiesLoose(_this$props$value, _excluded);
        return restProps
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                value: void 0 !== this.props.value ? this.props.value : this.state.value
            }),
            restAttributes: this.restAttributes
        })
    }
}
SelectBox.defaultProps = SelectBoxProps;
