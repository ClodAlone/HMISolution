/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import dateUtils from "../../../../core/utils/date";
import {
    BaseWidgetProps
} from "../../common/base_props";
var DEFAULT_GROUPS = [];
export var WorkSpaceProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseWidgetProps), Object.getOwnPropertyDescriptors({
    intervalCount: 1,
    groups: DEFAULT_GROUPS,
    groupByDate: false,
    crossScrollingEnabled: false,
    startDayHour: 0,
    endDayHour: 24,
    firstDayOfWeek: 0,
    hoursInterval: .5,
    showAllDayPanel: false,
    allDayPanelExpanded: false,
    allowMultipleCellSelection: true,
    get indicatorTime() {
        return new Date
    },
    get indicatorUpdateInterval() {
        return 5 * dateUtils.dateToMilliseconds("minute")
    },
    shadeUntilCurrentTime: true,
    get selectedCellData() {
        return []
    },
    get scrolling() {
        return {
            mode: "standard"
        }
    },
    cellDuration: 30,
    showCurrentTimeIndicator: true,
    type: "week"
})));
export var CurrentViewConfigType = {
    get intervalCount() {
        return WorkSpaceProps.intervalCount
    },
    get groups() {
        return WorkSpaceProps.groups
    },
    get groupByDate() {
        return WorkSpaceProps.groupByDate
    },
    get crossScrollingEnabled() {
        return WorkSpaceProps.crossScrollingEnabled
    },
    get startDayHour() {
        return WorkSpaceProps.startDayHour
    },
    get endDayHour() {
        return WorkSpaceProps.endDayHour
    },
    get firstDayOfWeek() {
        return WorkSpaceProps.firstDayOfWeek
    },
    get hoursInterval() {
        return WorkSpaceProps.hoursInterval
    },
    get showAllDayPanel() {
        return WorkSpaceProps.showAllDayPanel
    },
    get allDayPanelExpanded() {
        return WorkSpaceProps.allDayPanelExpanded
    },
    get allowMultipleCellSelection() {
        return WorkSpaceProps.allowMultipleCellSelection
    },
    get indicatorTime() {
        return WorkSpaceProps.indicatorTime
    },
    get indicatorUpdateInterval() {
        return WorkSpaceProps.indicatorUpdateInterval
    },
    get shadeUntilCurrentTime() {
        return WorkSpaceProps.shadeUntilCurrentTime
    },
    get selectedCellData() {
        return WorkSpaceProps.selectedCellData
    },
    get scrolling() {
        return WorkSpaceProps.scrolling
    },
    get cellDuration() {
        return WorkSpaceProps.cellDuration
    },
    get showCurrentTimeIndicator() {
        return WorkSpaceProps.showCurrentTimeIndicator
    },
    get type() {
        return WorkSpaceProps.type
    },
    get focusStateEnabled() {
        return WorkSpaceProps.focusStateEnabled
    },
    get tabIndex() {
        return WorkSpaceProps.tabIndex
    }
};
