/**
 * DevExtreme (esm/renovation/ui/scheduler/model/views.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isObject,
    isString
} from "../../../../core/utils/type";
var VIEW_TYPES = ["day", "week", "workWeek", "month", "timelineDay", "timelineWeek", "timelineWorkWeek", "timelineMonth", "agenda"];
export var getCurrentView = (currentView, views) => {
    var currentViewProps = views.find(view => {
        var names = isObject(view) ? [view.name, view.type] : [view];
        if (names.includes(currentView)) {
            return true
        }
        return false
    });
    if (void 0 === currentViewProps) {
        if (VIEW_TYPES.includes(currentView)) {
            currentViewProps = currentView
        } else {
            [currentViewProps] = views
        }
    }
    return currentViewProps
};
export var getCurrentViewProps = (currentView, views) => {
    var currentViewProps = getCurrentView(currentView, views);
    return isString(currentViewProps) ? {
        type: currentViewProps
    } : currentViewProps
};

function getViewConfigProp(schedulerProp, viewProp) {
    return void 0 !== viewProp ? viewProp : schedulerProp
}
export var getCurrentViewConfig = (currentViewProps, schedulerProps) => {
    var {
        scrolling: schedulerScrolling
    } = schedulerProps;
    var {
        cellDuration: cellDuration,
        dataCellTemplate: dataCellTemplate,
        dateCellTemplate: dateCellTemplate,
        endDayHour: endDayHour,
        firstDayOfWeek: firstDayOfWeek,
        groupByDate: groupByDate,
        groupOrientation: groupOrientation,
        intervalCount: intervalCount,
        resourceCellTemplate: resourceCellTemplate,
        scrolling: scrolling,
        startDate: startDate,
        startDayHour: startDayHour,
        timeCellTemplate: timeCellTemplate,
        type: type
    } = currentViewProps;
    var isVirtualScrolling = "virtual" === schedulerScrolling.mode || "virtual" === (null === scrolling || void 0 === scrolling ? void 0 : scrolling.mode);
    var crossScrollingEnabled = schedulerProps.crossScrollingEnabled || isVirtualScrolling;
    var result = {
        firstDayOfWeek: getViewConfigProp(schedulerProps.firstDayOfWeek, firstDayOfWeek),
        startDayHour: getViewConfigProp(schedulerProps.startDayHour, startDayHour),
        endDayHour: getViewConfigProp(schedulerProps.endDayHour, endDayHour),
        cellDuration: getViewConfigProp(schedulerProps.cellDuration, cellDuration),
        groupByDate: getViewConfigProp(schedulerProps.groupByDate, groupByDate),
        scrolling: getViewConfigProp(schedulerScrolling, scrolling),
        dataCellTemplate: getViewConfigProp(schedulerProps.dataCellTemplate, dataCellTemplate),
        timeCellTemplate: getViewConfigProp(schedulerProps.timeCellTemplate, timeCellTemplate),
        resourceCellTemplate: getViewConfigProp(schedulerProps.resourceCellTemplate, resourceCellTemplate),
        dateCellTemplate: getViewConfigProp(schedulerProps.dateCellTemplate, dateCellTemplate),
        currentDate: schedulerProps.currentDate,
        intervalCount: intervalCount,
        groupOrientation: groupOrientation,
        startDate: startDate,
        type: type,
        showAllDayPanel: schedulerProps.showAllDayPanel,
        showCurrentTimeIndicator: schedulerProps.showCurrentTimeIndicator,
        indicatorUpdateInterval: schedulerProps.indicatorUpdateInterval,
        shadeUntilCurrentTime: schedulerProps.shadeUntilCurrentTime,
        crossScrollingEnabled: crossScrollingEnabled,
        schedulerHeight: schedulerProps.height,
        schedulerWidth: schedulerProps.width,
        tabIndex: schedulerProps.tabIndex,
        accessKey: schedulerProps.accessKey,
        focusStateEnabled: schedulerProps.focusStateEnabled,
        allowMultipleCellSelection: true,
        allDayPanelExpanded: true
    };
    return _extends({}, result, {
        hoursInterval: result.cellDuration / 60
    })
};
