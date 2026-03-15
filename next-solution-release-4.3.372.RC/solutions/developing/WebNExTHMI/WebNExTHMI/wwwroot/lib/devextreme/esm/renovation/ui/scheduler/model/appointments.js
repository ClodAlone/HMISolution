/**
 * DevExtreme (esm/renovation/ui/scheduler/model/appointments.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    getCellWidth,
    getCellHeight,
    getAllDayHeight,
    PositionHelper
} from "../../../../ui/scheduler/workspaces/helpers/positionHelper";
import {
    getGroupCount
} from "../../../../ui/scheduler/resources/utils";
import {
    isGroupingByDate
} from "../workspaces/utils";
import dateUtils from "../../../../core/utils/date";
import {
    calculateIsGroupedAllDayPanel,
    getCellDuration
} from "../view_model/to_test/views/utils/base";
import {
    createGetAppointmentColor
} from "../resources/utils";
var toMs = name => dateUtils.dateToMilliseconds(name);
export var getAppointmentRenderingStrategyName = viewType => {
    var {
        renderingStrategy: renderingStrategy
    } = {
        day: {
            renderingStrategy: "vertical"
        },
        week: {
            renderingStrategy: "week"
        },
        workWeek: {
            renderingStrategy: "week"
        },
        month: {
            renderingStrategy: "horizontalMonth"
        },
        timelineDay: {
            renderingStrategy: "horizontal"
        },
        timelineWeek: {
            renderingStrategy: "horizontal"
        },
        timelineWorkWeek: {
            renderingStrategy: "horizontal"
        },
        timelineMonth: {
            renderingStrategy: "horizontalMonthLine"
        },
        agenda: {
            renderingStrategy: "agenda"
        }
    } [viewType];
    return renderingStrategy
};
export var getAppointmentsConfig = (schedulerConfig, viewConfig, loadedResources, viewDataProvider, isAllDayPanelSupported) => {
    var groupCount = getGroupCount(loadedResources);
    var startViewDate = viewDataProvider.getStartViewDate();
    var dateRange = [startViewDate, viewDataProvider.getLastViewDateByEndDayHour(viewConfig.endDayHour)];
    return {
        adaptivityEnabled: schedulerConfig.adaptivityEnabled,
        rtlEnabled: schedulerConfig.rtlEnabled,
        resources: schedulerConfig.resources,
        maxAppointmentsPerCell: schedulerConfig.maxAppointmentsPerCell,
        timeZone: schedulerConfig.timeZone,
        modelGroups: schedulerConfig.groups,
        startDayHour: viewConfig.startDayHour,
        viewStartDayHour: viewConfig.startDayHour,
        endDayHour: viewConfig.endDayHour,
        viewEndDayHour: viewConfig.endDayHour,
        currentDate: viewConfig.currentDate,
        isVirtualScrolling: "virtual" === viewConfig.scrolling.mode,
        intervalCount: viewConfig.intervalCount,
        hoursInterval: viewConfig.hoursInterval,
        showAllDayPanel: viewConfig.showAllDayPanel,
        supportAllDayRow: isAllDayPanelSupported,
        groupOrientation: viewDataProvider.getViewOptions().groupOrientation,
        firstDayOfWeek: viewConfig.firstDayOfWeek,
        viewType: viewConfig.type,
        cellDurationInMinutes: viewConfig.cellDuration,
        isVerticalGroupOrientation: viewDataProvider.getViewOptions().isVerticalGrouping,
        groupByDate: viewDataProvider.getViewOptions().isGroupedByDate,
        startViewDate: startViewDate,
        loadedResources: loadedResources,
        appointmentCountPerCell: 2,
        appointmentOffset: 26,
        allowResizing: false,
        allowAllDayResizing: false,
        dateTableOffset: 0,
        groupCount: groupCount,
        dateRange: dateRange
    }
};
export var getAppointmentsModel = (appointmentsConfig, viewDataProvider, timeZoneCalculator, dataAccessors, cellsMetaData) => {
    var groupedByDate = isGroupingByDate(appointmentsConfig.modelGroups, appointmentsConfig.groupOrientation, appointmentsConfig.groupByDate);
    var {
        groupCount: groupCount,
        isVerticalGroupOrientation: isVerticalGroupOrientation
    } = appointmentsConfig;
    var positionHelper = new PositionHelper({
        viewDataProvider: viewDataProvider,
        groupedByDate: groupedByDate,
        rtlEnabled: appointmentsConfig.rtlEnabled,
        groupCount: groupCount,
        isVerticalGrouping: groupCount && isVerticalGroupOrientation,
        getDOMMetaDataCallback: () => cellsMetaData
    });
    var isGroupedAllDayPanel = calculateIsGroupedAllDayPanel(appointmentsConfig.loadedResources, appointmentsConfig.groupOrientation, appointmentsConfig.showAllDayPanel);
    var rowCount = viewDataProvider.getRowCount({
        intervalCount: appointmentsConfig.intervalCount,
        currentDate: appointmentsConfig.currentDate,
        viewType: appointmentsConfig.viewType,
        hoursInterval: appointmentsConfig.hoursInterval,
        startDayHour: appointmentsConfig.startDayHour,
        endDayHour: appointmentsConfig.endDayHour
    });
    var allDayHeight = getAllDayHeight(appointmentsConfig.showAllDayPanel, appointmentsConfig.isVerticalGroupOrientation, cellsMetaData);
    var endViewDate = viewDataProvider.getLastCellEndDate();
    var visibleDayDuration = viewDataProvider.getVisibleDayDuration(appointmentsConfig.startDayHour, appointmentsConfig.endDayHour, appointmentsConfig.hoursInterval);
    var {
        leftVirtualCellCount: leftVirtualCellCount,
        topVirtualRowCount: topVirtualRowCount
    } = viewDataProvider.viewData;
    var cellDuration = getCellDuration(appointmentsConfig.viewType, appointmentsConfig.startDayHour, appointmentsConfig.endDayHour, appointmentsConfig.hoursInterval);
    var getAppointmentColor = createGetAppointmentColor({
        resources: appointmentsConfig.resources,
        dataAccessors: dataAccessors.resources,
        loadedResources: appointmentsConfig.loadedResources,
        resourceLoaderMap: new Map
    });
    var appointmentRenderingStrategyName = getAppointmentRenderingStrategyName(appointmentsConfig.viewType);
    return _extends({}, appointmentsConfig, {
        appointmentRenderingStrategyName: appointmentRenderingStrategyName,
        loadedResources: appointmentsConfig.loadedResources,
        dataAccessors: dataAccessors,
        timeZoneCalculator: timeZoneCalculator,
        viewDataProvider: viewDataProvider,
        positionHelper: positionHelper,
        isGroupedAllDayPanel: isGroupedAllDayPanel,
        rowCount: rowCount,
        cellWidth: getCellWidth(cellsMetaData),
        cellHeight: getCellHeight(cellsMetaData),
        allDayHeight: allDayHeight,
        isGroupedByDate: groupedByDate,
        endViewDate: endViewDate,
        visibleDayDuration: visibleDayDuration,
        intervalDuration: cellDuration,
        allDayIntervalDuration: toMs("day"),
        leftVirtualCellCount: leftVirtualCellCount,
        topVirtualCellCount: topVirtualRowCount,
        cellDuration: cellDuration,
        getAppointmentColor: getAppointmentColor,
        resizableStep: positionHelper.getResizableStep(),
        DOMMetaData: cellsMetaData
    })
};
