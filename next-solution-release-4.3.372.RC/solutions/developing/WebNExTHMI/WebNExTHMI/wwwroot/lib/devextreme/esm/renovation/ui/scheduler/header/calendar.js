/**
 * DevExtreme (esm/renovation/ui/scheduler/header/calendar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["currentDate", "firstDayOfWeek", "isMobileLayout", "max", "min", "onCurrentDateUpdate", "onVisibleUpdate", "visible"];
import {
    createVNode,
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    isMobileLayout
} from "./utils";
import {
    Popup
} from "../../overlays/popup";
import {
    Popover
} from "../../overlays/popover";
import {
    Calendar
} from "../../editors/calendar";
export var viewFunction = viewModel => {
    var {
        calendarRef: calendarRef,
        focusCalendar: focusCalendar,
        isMobile: isMobile,
        props: props,
        updateDate: updateDate,
        updateVisible: updateVisible
    } = viewModel;
    var {
        currentDate: currentDate,
        firstDayOfWeek: firstDayOfWeek,
        max: max,
        min: min,
        visible: visible
    } = props;
    var calendar = createVNode(1, "div", "dx-scheduler-navigator-calendar", createComponentVNode(2, Calendar, {
        value: currentDate,
        valueChange: updateDate,
        min: min,
        max: max,
        firstDayOfWeek: firstDayOfWeek,
        width: "100%",
        focusStateEnabled: true,
        skipFocusCheck: true
    }, null, calendarRef), 2);
    return isMobile ? createComponentVNode(2, Popup, {
        className: "dx-scheduler-navigator-calendar-popup",
        showTitle: false,
        closeOnOutsideClick: true,
        visible: visible,
        visibleChange: updateVisible,
        showCloseButton: true,
        fullScreen: true,
        toolbarItems: [{
            shortcut: "cancel"
        }],
        onShown: focusCalendar,
        children: calendar
    }) : createComponentVNode(2, Popover, {
        target: ".dx-scheduler-navigator-caption",
        className: "dx-scheduler-navigator-calendar-popover",
        showTitle: false,
        closeOnOutsideClick: true,
        visible: visible,
        visibleChange: updateVisible,
        onShown: focusCalendar,
        children: calendar
    })
};
export var SchedulerCalendarProps = {
    get isMobileLayout() {
        return isMobileLayout()
    }
};
import {
    convertRulesToOptions
} from "../../../../core/options/utils";
import {
    createRef as infernoCreateRef
} from "inferno";
export class SchedulerCalendar extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.calendarRef = infernoCreateRef();
        this.focusCalendar = this.focusCalendar.bind(this);
        this.updateVisible = this.updateVisible.bind(this);
        this.updateDate = this.updateDate.bind(this)
    }
    get isMobile() {
        return this.props.isMobileLayout
    }
    focusCalendar() {
        var _this$calendarRef$cur;
        null === (_this$calendarRef$cur = this.calendarRef.current) || void 0 === _this$calendarRef$cur ? void 0 : _this$calendarRef$cur.focus()
    }
    updateVisible(visible) {
        this.props.onVisibleUpdate(visible)
    }
    updateDate(date) {
        this.props.onCurrentDateUpdate(date)
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
            calendarRef: this.calendarRef,
            isMobile: this.isMobile,
            focusCalendar: this.focusCalendar,
            updateVisible: this.updateVisible,
            updateDate: this.updateDate,
            restAttributes: this.restAttributes
        })
    }
}
SchedulerCalendar.defaultProps = SchedulerCalendarProps;
var __defaultOptionRules = [];
export function defaultOptions(rule) {
    __defaultOptionRules.push(rule);
    SchedulerCalendar.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(SchedulerCalendar.defaultProps), Object.getOwnPropertyDescriptors(convertRulesToOptions(__defaultOptionRules))))
}
