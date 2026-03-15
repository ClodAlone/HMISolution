/**
 * DevExtreme (esm/renovation/ui/editors/check_box/check_box.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../../core/component_registrator";
import BaseComponent from "../../../component_wrapper/editors/check_box";
import {
    CheckBox as CheckBoxComponent,
    defaultOptions
} from "./check_box";
export default class CheckBox extends BaseComponent {
    getProps() {
        var props = super.getProps();
        props.onKeyDown = this._wrapKeyDownHandler(props.onKeyDown);
        return props
    }
    focus() {
        var _this$viewRef;
        return null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.focus(...arguments)
    }
    blur() {
        var _this$viewRef2;
        return null === (_this$viewRef2 = this.viewRef) || void 0 === _this$viewRef2 ? void 0 : _this$viewRef2.blur(...arguments)
    }
    _getActionConfigs() {
        return {
            onFocusIn: {},
            onClick: {}
        }
    }
    get _propsInfo() {
        return {
            twoWay: [
                ["value", "defaultValue", "valueChange"]
            ],
            allowNull: ["defaultValue", "validationError", "validationErrors", "value"],
            elements: [],
            templates: [],
            props: ["text", "iconSize", "activeStateEnabled", "hoverStateEnabled", "saveValueChangeEvent", "defaultValue", "valueChange", "readOnly", "name", "validationError", "validationErrors", "validationMessageMode", "validationStatus", "isValid", "onFocusIn", "className", "accessKey", "disabled", "focusStateEnabled", "height", "hint", "onClick", "onKeyDown", "rtlEnabled", "tabIndex", "visible", "width", "aria", "value"]
        }
    }
    get _viewComponent() {
        return CheckBoxComponent
    }
}
registerComponent("dxCheckBox", CheckBox);
CheckBox.defaultOptions = defaultOptions;
