/**
 * DevExtreme (esm/renovation/ui/scheduler/props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import messageLocalization from "../../../localization/message";
import {
    BaseWidgetProps
} from "../common/base_props";
export var ResourceProps = {};
export var ViewProps = {};
export var AppointmentEditingProps = {};
export var AppointmentDraggingProps = {};
export var ScrollingProps = {};
export var SchedulerProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseWidgetProps), Object.getOwnPropertyDescriptors({
    adaptivityEnabled: false,
    crossScrollingEnabled: false,
    descriptionExpr: "description",
    get editing() {
        return {
            allowAdding: true,
            allowDeleting: true,
            allowDragging: true,
            allowResizing: true,
            allowUpdating: true,
            allowTimeZoneEditing: false
        }
    },
    focusStateEnabled: true,
    groupByDate: false,
    indicatorUpdateInterval: 3e5,
    get noDataText() {
        return messageLocalization.format("dxCollectionWidget-noDataText")
    },
    recurrenceEditMode: "dialog",
    remoteFiltering: false,
    get resources() {
        return []
    },
    get scrolling() {
        return {
            mode: "standard"
        }
    },
    get selectedCellData() {
        return []
    },
    shadeUntilCurrentTime: false,
    showAllDayPanel: true,
    showCurrentTimeIndicator: true,
    timeZone: "",
    useDropDownViewSwitcher: false,
    get views() {
        return ["day", "week"]
    },
    endDayHour: 24,
    startDayHour: 0,
    firstDayOfWeek: 0,
    cellDuration: 30,
    get groups() {
        return []
    },
    maxAppointmentsPerCell: "auto",
    recurrenceExceptionExpr: "recurrenceException",
    recurrenceRuleExpr: "recurrenceRule",
    startDateExpr: "startDate",
    startDateTimeZoneExpr: "startDateTimeZone",
    endDateExpr: "endDate",
    endDateTimeZoneExpr: "endDateTimeZone",
    allDayExpr: "allDay",
    textExpr: "text",
    get toolbar() {
        return [{
            defaultElement: "dateNavigator",
            location: "before"
        }, {
            defaultElement: "viewSwitcher",
            location: "after"
        }]
    },
    get defaultCurrentDate() {
        return new Date
    },
    currentDateChange: () => {},
    defaultCurrentView: "day",
    currentViewChange: () => {}
})));
