/**
 * DevExtreme (esm/renovation/ui/editors/calendar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["_todayDate", "accessKey", "activeStateEnabled", "className", "defaultValue", "disabled", "firstDayOfWeek", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "max", "min", "onClick", "onKeyDown", "rtlEnabled", "skipFocusCheck", "tabIndex", "value", "valueChange", "visible", "width"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import LegacyCalendar from "../../../ui/calendar";
import {
    DomComponentWrapper
} from "../common/dom_component_wrapper";
import {
    BaseWidgetProps
} from "../common/base_props";

function today() {
    return new Date
}
export var viewFunction = _ref => {
    var {
        domComponentWrapperRef: domComponentWrapperRef,
        props: props,
        restAttributes: restAttributes
    } = _ref;
    return normalizeProps(createComponentVNode(2, DomComponentWrapper, _extends({
        componentType: LegacyCalendar,
        componentProps: props,
        templateNames: ["cellTemplate"]
    }, restAttributes), null, domComponentWrapperRef))
};
export var CalendarProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseWidgetProps), Object.getOwnPropertyDescriptors({
    _todayDate: today,
    skipFocusCheck: false,
    defaultValue: null,
    isReactComponentWrapper: true
})));
import {
    createRef as infernoCreateRef
} from "inferno";
export class Calendar extends InfernoComponent {
    constructor(props) {
        super(props);
        this.domComponentWrapperRef = infernoCreateRef();
        this.state = {
            value: void 0 !== this.props.value ? this.props.value : this.props.defaultValue
        };
        this.saveInstance = this.saveInstance.bind(this);
        this.focus = this.focus.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.saveInstance, [])]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([])
    }
    saveInstance() {
        var _this$domComponentWra;
        this.instance = null === (_this$domComponentWra = this.domComponentWrapperRef.current) || void 0 === _this$domComponentWra ? void 0 : _this$domComponentWra.getInstance()
    }
    get restAttributes() {
        var _this$props$value = _extends({}, this.props, {
                value: void 0 !== this.props.value ? this.props.value : this.state.value
            }),
            restProps = _objectWithoutPropertiesLoose(_this$props$value, _excluded);
        return restProps
    }
    focus() {
        var _this$instance;
        null === (_this$instance = this.instance) || void 0 === _this$instance ? void 0 : _this$instance.focus()
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                value: void 0 !== this.props.value ? this.props.value : this.state.value
            }),
            domComponentWrapperRef: this.domComponentWrapperRef,
            restAttributes: this.restAttributes
        })
    }
}
Calendar.defaultProps = CalendarProps;
