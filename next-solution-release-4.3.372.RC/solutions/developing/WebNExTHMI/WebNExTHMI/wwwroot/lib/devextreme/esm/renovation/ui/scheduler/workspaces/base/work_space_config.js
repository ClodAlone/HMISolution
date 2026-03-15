/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/work_space_config.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    formatWeekday,
    formatWeekdayAndDay
} from "../../view_model/to_test/views/utils/base";
import {
    getDateForHeaderText as timelineGetDateFrHeaderText
} from "../../view_model/to_test/views/utils/timeline_week";
import {
    MonthDateTableLayout
} from "../month/date_table/layout";
import {
    TimelineHeaderPanelLayout
} from "../timeline/header_panel/layout";
import {
    DateTableLayoutBase
} from "./date_table/layout";
import {
    HeaderPanelLayout
} from "./header_panel/layout";
import {
    TimePanelTableLayout
} from "./time_panel/layout";
import {
    getDateForHeaderText
} from "./utils";
import {
    isVerticalGroupingApplied
} from "../utils";
var TIMELINE_CLASS = "dx-scheduler-timeline";
var verticalViewConfig = {
    headerPanelTemplate: HeaderPanelLayout,
    dateTableTemplate: DateTableLayoutBase,
    timePanelTemplate: TimePanelTableLayout,
    isAllDayPanelSupported: true,
    isProvideVirtualCellsWidth: false,
    isRenderTimePanel: true,
    groupPanelClassName: "dx-scheduler-work-space-vertical-group-table",
    headerCellTextFormat: formatWeekdayAndDay,
    getDateForHeaderText: getDateForHeaderText,
    isRenderDateHeader: true,
    isGenerateWeekDaysHeaderData: false,
    scrollingDirection: "vertical",
    className: "dx-scheduler-work-space-day",
    isCreateCrossScrolling: false,
    defaultGroupOrientation: "horizontal"
};
var timelineViewConfig = {
    headerPanelTemplate: TimelineHeaderPanelLayout,
    dateTableTemplate: DateTableLayoutBase,
    isAllDayPanelSupported: false,
    isProvideVirtualCellsWidth: true,
    isRenderTimePanel: false,
    groupPanelClassName: "dx-scheduler-group-table",
    headerCellTextFormat: "shorttime",
    getDateForHeaderText: timelineGetDateFrHeaderText,
    isRenderDateHeader: true,
    isGenerateWeekDaysHeaderData: true,
    scrollingDirection: "horizontal",
    className: "dx-scheduler-timeline-day ".concat(TIMELINE_CLASS),
    isCreateCrossScrolling: true,
    defaultGroupOrientation: "vertical"
};
var getVerticalViewConfig = crossScrollingEnabled => _extends({}, verticalViewConfig, {
    isCreateCrossScrolling: crossScrollingEnabled
});
var getDayViewConfig = (crossScrollingEnabled, intervalCount) => _extends({}, getVerticalViewConfig(crossScrollingEnabled), {
    isRenderDateHeader: intervalCount > 1
});
var getWeekViewConfig = crossScrollingEnabled => _extends({}, getVerticalViewConfig(crossScrollingEnabled), {
    className: "dx-scheduler-work-space-week"
});
var getWorkWeekViewConfig = crossScrollingEnabled => _extends({}, getVerticalViewConfig(crossScrollingEnabled), {
    className: "dx-scheduler-work-space-work-week"
});
var getMonthViewConfig = (crossScrollingEnabled, _, groups, groupOrientation) => ({
    headerPanelTemplate: HeaderPanelLayout,
    dateTableTemplate: MonthDateTableLayout,
    isAllDayPanelSupported: false,
    isProvideVirtualCellsWidth: false,
    isRenderTimePanel: false,
    groupPanelClassName: "dx-scheduler-work-space-vertical-group-table",
    headerCellTextFormat: formatWeekday,
    getDateForHeaderText: getDateForHeaderText,
    isRenderDateHeader: true,
    isGenerateWeekDaysHeaderData: false,
    className: "dx-scheduler-work-space-month",
    scrollingDirection: "vertical",
    isCreateCrossScrolling: crossScrollingEnabled || isVerticalGroupingApplied(groups, groupOrientation),
    defaultGroupOrientation: "horizontal"
});
var getTimelineDayViewConfig = (_, intervalCount) => _extends({}, timelineViewConfig, {
    isGenerateWeekDaysHeaderData: intervalCount > 1
});
var getTimelineWeekViewConfig = () => _extends({}, timelineViewConfig, {
    className: "dx-scheduler-timeline-week ".concat(TIMELINE_CLASS)
});
var getTimelineWorkWeekViewConfig = () => _extends({}, timelineViewConfig, {
    className: "dx-scheduler-timeline-work-week ".concat(TIMELINE_CLASS)
});
var getTimelineMonthViewConfig = () => _extends({}, timelineViewConfig, {
    className: "dx-scheduler-timeline-month ".concat(TIMELINE_CLASS),
    headerCellTextFormat: formatWeekdayAndDay,
    isGenerateWeekDaysHeaderData: false,
    getDateForHeaderText: getDateForHeaderText
});
var VIEW_CONFIG_GETTERS = {
    day: getDayViewConfig,
    week: getWeekViewConfig,
    workWeek: getWorkWeekViewConfig,
    month: getMonthViewConfig,
    timelineDay: getTimelineDayViewConfig,
    timelineWeek: getTimelineWeekViewConfig,
    timelineWorkWeek: getTimelineWorkWeekViewConfig,
    timelineMonth: getTimelineMonthViewConfig,
    agenda: getWeekViewConfig
};
export var getViewRenderConfigByType = (viewType, crossScrollingEnabled, intervalCount, groups, groupOrientation) => VIEW_CONFIG_GETTERS[viewType](crossScrollingEnabled, intervalCount, groups, groupOrientation);
