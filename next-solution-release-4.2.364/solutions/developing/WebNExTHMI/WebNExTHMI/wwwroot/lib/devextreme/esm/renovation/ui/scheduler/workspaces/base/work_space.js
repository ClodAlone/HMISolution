/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/work_space.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["accessKey", "activeStateEnabled", "allDayAppointments", "allDayPanelExpanded", "allowMultipleCellSelection", "appointments", "cellDuration", "className", "crossScrollingEnabled", "currentDate", "dataCellTemplate", "dateCellTemplate", "disabled", "endDayHour", "firstDayOfWeek", "focusStateEnabled", "groupByDate", "groupOrientation", "groups", "height", "hint", "hoursInterval", "hoverStateEnabled", "indicatorTime", "indicatorUpdateInterval", "intervalCount", "onClick", "onKeyDown", "onViewRendered", "resourceCellTemplate", "rtlEnabled", "schedulerHeight", "schedulerWidth", "scrolling", "selectedCellData", "shadeUntilCurrentTime", "showAllDayPanel", "showCurrentTimeIndicator", "startDate", "startDayHour", "tabIndex", "timeCellTemplate", "type", "visible", "width"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import {
    combineClasses
} from "../../../../utils/combine_classes";
import {
    OrdinaryLayout
} from "./ordinary_layout";
import {
    VirtualScrollingDispatcher
} from "../../../../../ui/scheduler/workspaces/ui.scheduler.virtual_scrolling";
import ViewDataProvider from "../../../../../ui/scheduler/workspaces/view_model/view_data_provider";
import {
    createCellElementMetaData,
    createVirtualScrollingOptions,
    DATE_TABLE_MIN_CELL_WIDTH,
    getDateTableWidth
} from "./utils";
import {
    WorkSpaceProps
} from "../props";
import {
    getViewRenderConfigByType
} from "./work_space_config";
import {
    isGroupingByDate,
    isHorizontalGroupingApplied,
    isVerticalGroupingApplied
} from "../utils";
import {
    CrossScrollingLayout
} from "./cross_scrolling_layout";
import {
    getViewDataGeneratorByViewType
} from "../../../../../ui/scheduler/workspaces/view_model/utils";
import {
    calculateIsGroupedAllDayPanel
} from "../../view_model/to_test/views/utils/base";
import {
    DateHeaderDataGenerator
} from "../../../../../ui/scheduler/workspaces/view_model/date_header_data_generator";
import {
    TimePanelDataGenerator
} from "../../../../../ui/scheduler/workspaces/view_model/time_panel_data_generator";
import {
    getGroupPanelData
} from "../../view_model/group_panel/utils";
var defaultVirtualScrollingMetaData = {
    cellHeight: 50,
    cellWidth: DATE_TABLE_MIN_CELL_WIDTH,
    viewWidth: 300,
    viewHeight: 300,
    scrollableWidth: 300
};
var calculateDefaultVirtualScrollingState = options => {
    var completeColumnCount = options.completeViewDataMap[0].length;
    var completeRowCount = options.completeViewDataMap.length;
    options.virtualScrollingDispatcher.setViewOptions(createVirtualScrollingOptions({
        cellHeight: defaultVirtualScrollingMetaData.cellHeight,
        cellWidth: defaultVirtualScrollingMetaData.cellWidth,
        schedulerHeight: options.schedulerHeight,
        schedulerWidth: options.schedulerWidth,
        viewHeight: defaultVirtualScrollingMetaData.viewHeight,
        viewWidth: defaultVirtualScrollingMetaData.viewWidth,
        scrolling: options.scrolling,
        scrollableWidth: defaultVirtualScrollingMetaData.scrollableWidth,
        groups: options.groups,
        isVerticalGrouping: options.isVerticalGrouping,
        completeRowCount: completeRowCount,
        completeColumnCount: completeColumnCount
    }));
    options.virtualScrollingDispatcher.createVirtualScrolling();
    options.virtualScrollingDispatcher.updateDimensions(true);
    return options.virtualScrollingDispatcher.getRenderState()
};
export var prepareGenerationOptions = (workSpaceProps, renderConfig, isAllDayPanelVisible, virtualStartIndices) => {
    var {
        cellDuration: cellDuration,
        currentDate: currentDate,
        endDayHour: endDayHour,
        firstDayOfWeek: firstDayOfWeek,
        groupByDate: groupByDate,
        groupOrientation: groupOrientation,
        groups: groups,
        hoursInterval: hoursInterval,
        intervalCount: intervalCount,
        startDate: startDate,
        startDayHour: startDayHour,
        type: type
    } = workSpaceProps;
    var {
        getDateForHeaderText: getDateForHeaderText,
        headerCellTextFormat: headerCellTextFormat,
        isGenerateWeekDaysHeaderData: isGenerateWeekDaysHeaderData,
        isProvideVirtualCellsWidth: isProvideVirtualCellsWidth,
        isRenderTimePanel: isRenderTimePanel
    } = renderConfig;
    return {
        startRowIndex: virtualStartIndices.startRowIndex,
        startCellIndex: virtualStartIndices.startCellIndex,
        groupOrientation: groupOrientation,
        groupByDate: groupByDate,
        groups: groups,
        isProvideVirtualCellsWidth: isProvideVirtualCellsWidth,
        isAllDayPanelVisible: isAllDayPanelVisible,
        selectedCells: void 0,
        focusedCell: void 0,
        headerCellTextFormat: headerCellTextFormat,
        getDateForHeaderText: getDateForHeaderText,
        startDayHour: startDayHour,
        endDayHour: endDayHour,
        cellDuration: cellDuration,
        viewType: type,
        intervalCount: intervalCount,
        hoursInterval: hoursInterval,
        currentDate: currentDate,
        startDate: startDate,
        firstDayOfWeek: firstDayOfWeek,
        isGenerateTimePanelData: isRenderTimePanel,
        isGenerateWeekDaysHeaderData: isGenerateWeekDaysHeaderData
    }
};
export var viewFunction = _ref => {
    var {
        allDayPanelRef: allDayPanelRef,
        classes: classes,
        dateHeaderData: dateHeaderData,
        dateTableRef: dateTableRef,
        dateTableTemplate: dateTableTemplate,
        groupOrientation: groupOrientation,
        groupPanelData: groupPanelData,
        groupPanelHeight: groupPanelHeight,
        groupPanelRef: groupPanelRef,
        headerEmptyCellWidth: headerEmptyCellWidth,
        headerPanelTemplate: headerPanelTemplate,
        isAllDayPanelVisible: isAllDayPanelVisible,
        isGroupedByDate: isGroupedByDate,
        isRenderHeaderEmptyCell: isRenderHeaderEmptyCell,
        isStandaloneAllDayPanel: isStandaloneAllDayPanel,
        isVerticalGrouping: isVerticalGrouping,
        layout: Layout,
        layoutRef: layoutRef,
        onScroll: onScroll,
        props: {
            allDayAppointments: allDayAppointments,
            allDayPanelExpanded: allDayPanelExpanded,
            appointments: appointments,
            dataCellTemplate: dataCellTemplate,
            dateCellTemplate: dateCellTemplate,
            groups: groups,
            intervalCount: intervalCount,
            resourceCellTemplate: resourceCellTemplate,
            timeCellTemplate: timeCellTemplate
        },
        renderConfig: {
            groupPanelClassName: groupPanelClassName,
            isRenderDateHeader: isRenderDateHeader,
            scrollingDirection: scrollingDirection
        },
        tablesWidth: tablesWidth,
        timePanelData: timePanelData,
        timePanelRef: timePanelRef,
        timePanelTemplate: timePanelTemplate,
        viewData: viewData,
        widgetElementRef: widgetElementRef
    } = _ref;
    return createComponentVNode(2, Layout, {
        viewData: viewData,
        dateHeaderData: dateHeaderData,
        timePanelData: timePanelData,
        groupPanelData: groupPanelData,
        dataCellTemplate: dataCellTemplate,
        dateCellTemplate: dateCellTemplate,
        timeCellTemplate: timeCellTemplate,
        resourceCellTemplate: resourceCellTemplate,
        groups: groups,
        groupByDate: isGroupedByDate,
        groupOrientation: groupOrientation,
        groupPanelClassName: groupPanelClassName,
        intervalCount: intervalCount,
        headerPanelTemplate: headerPanelTemplate,
        dateTableTemplate: dateTableTemplate,
        timePanelTemplate: timePanelTemplate,
        isAllDayPanelCollapsed: !allDayPanelExpanded,
        isAllDayPanelVisible: isAllDayPanelVisible,
        isRenderDateHeader: isRenderDateHeader,
        isRenderHeaderEmptyCell: isRenderHeaderEmptyCell,
        isRenderGroupPanel: isVerticalGrouping,
        isStandaloneAllDayPanel: isStandaloneAllDayPanel,
        scrollingDirection: scrollingDirection,
        groupPanelHeight: groupPanelHeight,
        headerEmptyCellWidth: headerEmptyCellWidth,
        tablesWidth: tablesWidth,
        onScroll: onScroll,
        className: classes,
        dateTableRef: dateTableRef,
        allDayPanelRef: allDayPanelRef,
        timePanelRef: timePanelRef,
        groupPanelRef: groupPanelRef,
        widgetElementRef: widgetElementRef,
        appointments: appointments,
        allDayAppointments: allDayAppointments
    }, null, layoutRef)
};
import {
    createRef as infernoCreateRef
} from "inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class WorkSpace extends InfernoComponent {
    constructor(props) {
        super(props);
        this.dateTableRef = infernoCreateRef();
        this.allDayPanelRef = infernoCreateRef();
        this.timePanelRef = infernoCreateRef();
        this.groupPanelRef = infernoCreateRef();
        this.layoutRef = infernoCreateRef();
        this.widgetElementRef = infernoCreateRef();
        this.__getterCache = {};
        this.state = {
            groupPanelHeight: void 0,
            headerEmptyCellWidth: void 0,
            tablesWidth: void 0,
            virtualScrolling: new VirtualScrollingDispatcher,
            virtualScrollingData: void 0
        };
        this.groupPanelHeightEffect = this.groupPanelHeightEffect.bind(this);
        this.headerEmptyCellWidthEffect = this.headerEmptyCellWidthEffect.bind(this);
        this.tablesWidthEffect = this.tablesWidthEffect.bind(this);
        this.virtualScrollingMetaDataEffect = this.virtualScrollingMetaDataEffect.bind(this);
        this.onViewRendered = this.onViewRendered.bind(this);
        this.createDateTableElementsMeta = this.createDateTableElementsMeta.bind(this);
        this.createAllDayPanelElementsMeta = this.createAllDayPanelElementsMeta.bind(this);
        this.onScroll = this.onScroll.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.groupPanelHeightEffect, [this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]), new InfernoEffect(this.headerEmptyCellWidthEffect, [this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]), new InfernoEffect(this.tablesWidthEffect, [this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]), new InfernoEffect(this.virtualScrollingMetaDataEffect, [this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]), new InfernoEffect(this.onViewRendered, [this.props.allDayPanelExpanded, this.props.cellDuration, this.props.crossScrollingEnabled, this.props.currentDate, this.props.endDayHour, this.props.firstDayOfWeek, this.props.groupByDate, this.props.groupOrientation, this.props.type, this.props.intervalCount, this.props.groups, this.props.hoursInterval, this.props.onViewRendered, this.props.scrolling, this.props.showAllDayPanel, this.props.startDate, this.props.startDayHour, this.state.virtualScrollingData, this.props.schedulerHeight, this.props.schedulerWidth, this.state.virtualScrolling, this.state.tablesWidth])]
    }
    updateEffects() {
        var _this$_effects$, _this$_effects$2, _this$_effects$3, _this$_effects$4, _this$_effects$5;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]);
        null === (_this$_effects$2 = this._effects[1]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]);
        null === (_this$_effects$3 = this._effects[2]) || void 0 === _this$_effects$3 ? void 0 : _this$_effects$3.update([this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]);
        null === (_this$_effects$4 = this._effects[3]) || void 0 === _this$_effects$4 ? void 0 : _this$_effects$4.update([this.props, this.state.groupPanelHeight, this.state.headerEmptyCellWidth, this.state.tablesWidth, this.state.virtualScrolling, this.state.virtualScrollingData, this.props.dataCellTemplate, this.props.dateCellTemplate, this.props.timeCellTemplate, this.props.resourceCellTemplate, this.props.intervalCount, this.props.groups, this.props.groupByDate, this.props.groupOrientation, this.props.crossScrollingEnabled, this.props.startDayHour, this.props.endDayHour, this.props.firstDayOfWeek, this.props.currentDate, this.props.startDate, this.props.hoursInterval, this.props.showAllDayPanel, this.props.allDayPanelExpanded, this.props.allowMultipleCellSelection, this.props.indicatorTime, this.props.indicatorUpdateInterval, this.props.shadeUntilCurrentTime, this.props.selectedCellData, this.props.scrolling, this.props.cellDuration, this.props.showCurrentTimeIndicator, this.props.schedulerHeight, this.props.schedulerWidth, this.props.type, this.props.onViewRendered, this.props.appointments, this.props.allDayAppointments, this.props.className, this.props.accessKey, this.props.activeStateEnabled, this.props.disabled, this.props.focusStateEnabled, this.props.height, this.props.hint, this.props.hoverStateEnabled, this.props.onClick, this.props.onKeyDown, this.props.rtlEnabled, this.props.tabIndex, this.props.visible, this.props.width]);
        null === (_this$_effects$5 = this._effects[4]) || void 0 === _this$_effects$5 ? void 0 : _this$_effects$5.update([this.props.allDayPanelExpanded, this.props.cellDuration, this.props.crossScrollingEnabled, this.props.currentDate, this.props.endDayHour, this.props.firstDayOfWeek, this.props.groupByDate, this.props.groupOrientation, this.props.type, this.props.intervalCount, this.props.groups, this.props.hoursInterval, this.props.onViewRendered, this.props.scrolling, this.props.showAllDayPanel, this.props.startDate, this.props.startDayHour, this.state.virtualScrollingData, this.props.schedulerHeight, this.props.schedulerWidth, this.state.virtualScrolling, this.state.tablesWidth])
    }
    groupPanelHeightEffect() {
        this.setState(__state_argument => {
            var _this$dateTableRef$cu;
            return {
                groupPanelHeight: null === (_this$dateTableRef$cu = this.dateTableRef.current) || void 0 === _this$dateTableRef$cu ? void 0 : _this$dateTableRef$cu.getBoundingClientRect().height
            }
        })
    }
    headerEmptyCellWidthEffect() {
        var _this$timePanelRef$cu, _this$timePanelRef$cu2, _this$groupPanelRef$c, _this$groupPanelRef$c2;
        var timePanelWidth = null !== (_this$timePanelRef$cu = null === (_this$timePanelRef$cu2 = this.timePanelRef.current) || void 0 === _this$timePanelRef$cu2 ? void 0 : _this$timePanelRef$cu2.getBoundingClientRect().width) && void 0 !== _this$timePanelRef$cu ? _this$timePanelRef$cu : 0;
        var groupPanelWidth = null !== (_this$groupPanelRef$c = null === (_this$groupPanelRef$c2 = this.groupPanelRef.current) || void 0 === _this$groupPanelRef$c2 ? void 0 : _this$groupPanelRef$c2.getBoundingClientRect().width) && void 0 !== _this$groupPanelRef$c ? _this$groupPanelRef$c : 0;
        this.setState(__state_argument => ({
            headerEmptyCellWidth: timePanelWidth + groupPanelWidth
        }))
    }
    tablesWidthEffect() {
        if (this.isCalculateTablesWidth) {
            var {
                currentDate: currentDate,
                endDayHour: endDayHour,
                groups: groups,
                hoursInterval: hoursInterval,
                intervalCount: intervalCount,
                startDayHour: startDayHour,
                type: viewType
            } = this.props;
            this.setState(__state_argument => ({
                tablesWidth: getDateTableWidth(this.layoutRef.current.getScrollableWidth(), this.dateTableRef.current, this.viewDataProvider, {
                    intervalCount: intervalCount,
                    currentDate: currentDate,
                    viewType: viewType,
                    hoursInterval: hoursInterval,
                    startDayHour: startDayHour,
                    endDayHour: endDayHour,
                    groups: groups,
                    groupOrientation: this.groupOrientation
                })
            }))
        }
    }
    virtualScrollingMetaDataEffect() {
        var dateTableCell = this.dateTableRef.current.querySelector("td:not(.dx-scheduler-virtual-cell)");
        var cellRect = dateTableCell.getBoundingClientRect();
        var cellHeight = Math.floor(cellRect.height);
        var cellWidth = Math.floor(cellRect.width);
        var scrollableWidth = this.layoutRef.current.getScrollableWidth();
        var widgetRect = this.widgetElementRef.current.getBoundingClientRect();
        var viewHeight = widgetRect.height;
        var viewWidth = widgetRect.width;
        var nextSizes = {
            cellHeight: cellHeight,
            cellWidth: cellWidth,
            scrollableWidth: scrollableWidth,
            viewWidth: viewWidth,
            viewHeight: viewHeight
        };
        var isNextMetaDataNotEqualToCurrent = !this.state.virtualScrollingData || Object.entries(nextSizes).some(_ref2 => {
            var [key, value] = _ref2;
            return value !== this.state.virtualScrollingData.sizes[key]
        });
        if (isNextMetaDataNotEqualToCurrent) {
            var {
                groups: groups,
                schedulerHeight: schedulerHeight,
                schedulerWidth: schedulerWidth,
                scrolling: scrolling
            } = this.props;
            var completeColumnCount = this.completeViewDataMap[0].length;
            var completeRowCount = this.completeViewDataMap.length;
            this.state.virtualScrolling.setViewOptions(createVirtualScrollingOptions({
                cellHeight: nextSizes.cellHeight,
                cellWidth: nextSizes.cellWidth,
                schedulerHeight: schedulerHeight,
                schedulerWidth: schedulerWidth,
                viewHeight: nextSizes.viewHeight,
                viewWidth: nextSizes.viewWidth,
                scrolling: scrolling,
                scrollableWidth: nextSizes.scrollableWidth,
                groups: groups,
                isVerticalGrouping: this.isVerticalGrouping,
                completeRowCount: completeRowCount,
                completeColumnCount: completeColumnCount
            }));
            this.state.virtualScrolling.createVirtualScrolling();
            this.state.virtualScrolling.updateDimensions(true);
            this.setState(__state_argument => ({
                virtualScrollingData: {
                    state: this.state.virtualScrolling.getRenderState(),
                    sizes: nextSizes
                }
            }))
        }
    }
    onViewRendered() {
        var {
            allDayPanelExpanded: allDayPanelExpanded,
            cellDuration: cellDuration,
            crossScrollingEnabled: crossScrollingEnabled,
            currentDate: currentDate,
            endDayHour: endDayHour,
            firstDayOfWeek: firstDayOfWeek,
            groupByDate: groupByDate,
            groupOrientation: groupOrientation,
            groups: groups,
            hoursInterval: hoursInterval,
            intervalCount: intervalCount,
            onViewRendered: onViewRendered,
            scrolling: scrolling,
            showAllDayPanel: showAllDayPanel,
            startDate: startDate,
            startDayHour: startDayHour,
            type: viewType
        } = this.props;
        var tableWidths = getDateTableWidth(this.layoutRef.current.getScrollableWidth(), this.dateTableRef.current, this.viewDataProvider, {
            intervalCount: intervalCount,
            currentDate: currentDate,
            viewType: viewType,
            hoursInterval: hoursInterval,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            groups: groups,
            groupOrientation: this.groupOrientation
        });
        if (!this.isCalculateTablesWidth || tableWidths === this.state.tablesWidth) {
            var columnCount = this.viewDataMap.dateTableMap[0].length;
            var dateTableCellsMeta = this.createDateTableElementsMeta(columnCount);
            var allDayPanelCellsMeta = this.createAllDayPanelElementsMeta();
            onViewRendered({
                viewDataProvider: this.viewDataProvider,
                cellsMetaData: {
                    dateTableCellsMeta: dateTableCellsMeta,
                    allDayPanelCellsMeta: allDayPanelCellsMeta
                },
                viewDataProviderValidationOptions: {
                    intervalCount: intervalCount,
                    currentDate: currentDate,
                    type: viewType,
                    hoursInterval: hoursInterval,
                    startDayHour: startDayHour,
                    endDayHour: endDayHour,
                    groups: groups,
                    groupOrientation: groupOrientation,
                    groupByDate: groupByDate,
                    crossScrollingEnabled: crossScrollingEnabled,
                    firstDayOfWeek: firstDayOfWeek,
                    startDate: startDate,
                    showAllDayPanel: showAllDayPanel,
                    allDayPanelExpanded: allDayPanelExpanded,
                    scrolling: scrolling,
                    cellDuration: cellDuration
                }
            })
        }
    }
    get renderConfig() {
        if (void 0 !== this.__getterCache.renderConfig) {
            return this.__getterCache.renderConfig
        }
        return this.__getterCache.renderConfig = (() => getViewRenderConfigByType(this.props.type, this.props.crossScrollingEnabled, this.props.intervalCount, this.props.groups, this.props.groupOrientation))()
    }
    get groupOrientation() {
        var {
            groupOrientation: groupOrientation
        } = this.props;
        var {
            defaultGroupOrientation: defaultGroupOrientation
        } = this.renderConfig;
        return null !== groupOrientation && void 0 !== groupOrientation ? groupOrientation : defaultGroupOrientation
    }
    get isVerticalGrouping() {
        return isVerticalGroupingApplied(this.props.groups, this.groupOrientation)
    }
    get isHorizontalGrouping() {
        return isHorizontalGroupingApplied(this.props.groups, this.groupOrientation)
    }
    get isGroupedByDate() {
        return isGroupingByDate(this.props.groups, this.groupOrientation, this.props.groupByDate)
    }
    get layout() {
        return this.renderConfig.isCreateCrossScrolling ? CrossScrollingLayout : OrdinaryLayout
    }
    get isAllDayPanelVisible() {
        var {
            showAllDayPanel: showAllDayPanel
        } = this.props;
        var {
            isAllDayPanelSupported: isAllDayPanelSupported
        } = this.renderConfig;
        return isAllDayPanelSupported && showAllDayPanel
    }
    get viewDataGenerator() {
        if (void 0 !== this.__getterCache.viewDataGenerator) {
            return this.__getterCache.viewDataGenerator
        }
        return this.__getterCache.viewDataGenerator = (() => getViewDataGeneratorByViewType(this.props.type))()
    }
    get dateHeaderDataGenerator() {
        if (void 0 !== this.__getterCache.dateHeaderDataGenerator) {
            return this.__getterCache.dateHeaderDataGenerator
        }
        return this.__getterCache.dateHeaderDataGenerator = (() => new DateHeaderDataGenerator(this.viewDataGenerator))()
    }
    get timePanelDataGenerator() {
        if (void 0 !== this.__getterCache.timePanelDataGenerator) {
            return this.__getterCache.timePanelDataGenerator
        }
        return this.__getterCache.timePanelDataGenerator = (() => new TimePanelDataGenerator(this.viewDataGenerator))()
    }
    get startViewDate() {
        if (void 0 !== this.__getterCache.startViewDate) {
            return this.__getterCache.startViewDate
        }
        return this.__getterCache.startViewDate = (() => {
            var {
                currentDate: currentDate,
                firstDayOfWeek: firstDayOfWeek,
                intervalCount: intervalCount,
                startDate: startDate,
                startDayHour: startDayHour,
                type: type
            } = this.props;
            var options = {
                currentDate: currentDate,
                startDayHour: startDayHour,
                startDate: startDate,
                intervalCount: intervalCount,
                firstDayOfWeek: firstDayOfWeek
            };
            var viewDataGenerator = getViewDataGeneratorByViewType(type);
            var startViewDate = viewDataGenerator.getStartViewDate(options);
            return startViewDate
        })()
    }
    get completeViewDataMap() {
        if (void 0 !== this.__getterCache.completeViewDataMap) {
            return this.__getterCache.completeViewDataMap
        }
        return this.__getterCache.completeViewDataMap = (() => {
            var {
                cellDuration: cellDuration,
                currentDate: currentDate,
                endDayHour: endDayHour,
                firstDayOfWeek: firstDayOfWeek,
                groupByDate: groupByDate,
                groups: groups,
                hoursInterval: hoursInterval,
                intervalCount: intervalCount,
                startDate: startDate,
                startDayHour: startDayHour,
                type: type
            } = this.props;
            return this.viewDataGenerator.getCompleteViewDataMap({
                currentDate: currentDate,
                startDate: startDate,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                groupByDate: groupByDate,
                groups: groups,
                intervalCount: intervalCount,
                firstDayOfWeek: firstDayOfWeek,
                hoursInterval: hoursInterval,
                cellDuration: cellDuration,
                startViewDate: this.startViewDate,
                groupOrientation: this.groupOrientation,
                isVerticalGrouping: this.isVerticalGrouping,
                isHorizontalGrouping: this.isHorizontalGrouping,
                isGroupedByDate: this.isGroupedByDate,
                isAllDayPanelVisible: this.isAllDayPanelVisible,
                viewType: type,
                interval: this.viewDataGenerator.getInterval(hoursInterval)
            })
        })()
    }
    get correctedVirtualScrollingState() {
        if (void 0 !== this.__getterCache.correctedVirtualScrollingState) {
            return this.__getterCache.correctedVirtualScrollingState
        }
        return this.__getterCache.correctedVirtualScrollingState = (() => {
            var _this$state$virtualSc;
            var result = null === (_this$state$virtualSc = this.state.virtualScrollingData) || void 0 === _this$state$virtualSc ? void 0 : _this$state$virtualSc.state;
            if (!result) {
                var {
                    groups: groups,
                    schedulerHeight: schedulerHeight,
                    schedulerWidth: schedulerWidth,
                    scrolling: scrolling
                } = this.props;
                result = calculateDefaultVirtualScrollingState({
                    virtualScrollingDispatcher: this.state.virtualScrolling,
                    scrolling: scrolling,
                    groups: groups,
                    completeViewDataMap: this.completeViewDataMap,
                    isVerticalGrouping: this.isVerticalGrouping,
                    schedulerHeight: schedulerHeight,
                    schedulerWidth: schedulerWidth
                })
            }
            return _extends({
                startCellIndex: 0,
                startRowIndex: 0
            }, result)
        })()
    }
    get viewDataMap() {
        if (void 0 !== this.__getterCache.viewDataMap) {
            return this.__getterCache.viewDataMap
        }
        return this.__getterCache.viewDataMap = (() => this.viewDataGenerator.generateViewDataMap(this.completeViewDataMap, _extends({}, this.correctedVirtualScrollingState, {
            isVerticalGrouping: this.isVerticalGrouping,
            isAllDayPanelVisible: this.isAllDayPanelVisible
        })))()
    }
    get viewData() {
        if (void 0 !== this.__getterCache.viewData) {
            return this.__getterCache.viewData
        }
        return this.__getterCache.viewData = (() => {
            var {
                groups: groups
            } = this.props;
            var result = this.viewDataGenerator.getViewDataFromMap(this.completeViewDataMap, this.viewDataMap, _extends({}, this.correctedVirtualScrollingState, {
                isProvideVirtualCellsWidth: this.renderConfig.isProvideVirtualCellsWidth,
                isVerticalGrouping: this.isVerticalGrouping,
                isAllDayPanelVisible: this.isAllDayPanelVisible,
                isGroupedAllDayPanel: calculateIsGroupedAllDayPanel(groups, this.groupOrientation, this.isAllDayPanelVisible)
            }));
            return result
        })()
    }
    get completeDateHeaderData() {
        if (void 0 !== this.__getterCache.completeDateHeaderData) {
            return this.__getterCache.completeDateHeaderData
        }
        return this.__getterCache.completeDateHeaderData = (() => {
            var {
                currentDate: currentDate,
                endDayHour: endDayHour,
                groups: groups,
                hoursInterval: hoursInterval,
                intervalCount: intervalCount,
                startDayHour: startDayHour,
                type: viewType
            } = this.props;
            return this.dateHeaderDataGenerator.getCompleteDateHeaderMap({
                isGenerateWeekDaysHeaderData: this.renderConfig.isGenerateWeekDaysHeaderData,
                isGroupedByDate: this.isGroupedByDate,
                groups: groups,
                groupOrientation: this.groupOrientation,
                isHorizontalGrouping: this.isHorizontalGrouping,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                hoursInterval: hoursInterval,
                intervalCount: intervalCount,
                headerCellTextFormat: this.renderConfig.headerCellTextFormat,
                getDateForHeaderText: this.renderConfig.getDateForHeaderText,
                interval: this.viewDataGenerator.getInterval(hoursInterval),
                startViewDate: this.startViewDate,
                currentDate: currentDate,
                viewType: viewType,
                today: new Date
            }, this.completeViewDataMap)
        })()
    }
    get dateHeaderData() {
        if (void 0 !== this.__getterCache.dateHeaderData) {
            return this.__getterCache.dateHeaderData
        }
        return this.__getterCache.dateHeaderData = (() => {
            var {
                endDayHour: endDayHour,
                groups: groups,
                hoursInterval: hoursInterval,
                startDayHour: startDayHour
            } = this.props;
            return this.dateHeaderDataGenerator.generateDateHeaderData(this.completeDateHeaderData, this.completeViewDataMap, _extends({
                isGenerateWeekDaysHeaderData: this.renderConfig.isGenerateWeekDaysHeaderData,
                isProvideVirtualCellsWidth: this.renderConfig.isProvideVirtualCellsWidth,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                hoursInterval: hoursInterval,
                groups: groups,
                groupOrientation: this.groupOrientation,
                isGroupedByDate: this.isGroupedByDate
            }, this.correctedVirtualScrollingState))
        })()
    }
    get completeTimePanelData() {
        if (void 0 !== this.__getterCache.completeTimePanelData) {
            return this.__getterCache.completeTimePanelData
        }
        return this.__getterCache.completeTimePanelData = (() => {
            if (!this.renderConfig.isRenderTimePanel) {
                return
            }
            var {
                cellDuration: cellDuration,
                currentDate: currentDate,
                endDayHour: endDayHour,
                hoursInterval: hoursInterval,
                intervalCount: intervalCount,
                startDayHour: startDayHour,
                type: type
            } = this.props;
            return this.timePanelDataGenerator.getCompleteTimePanelMap({
                startViewDate: this.startViewDate,
                cellDuration: cellDuration,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                isVerticalGrouping: this.isVerticalGrouping,
                intervalCount: intervalCount,
                currentDate: currentDate,
                viewType: type,
                hoursInterval: hoursInterval
            }, this.completeViewDataMap)
        })()
    }
    get timePanelData() {
        if (void 0 !== this.__getterCache.timePanelData) {
            return this.__getterCache.timePanelData
        }
        return this.__getterCache.timePanelData = (() => {
            if (!this.completeTimePanelData) {
                return
            }
            return this.timePanelDataGenerator.generateTimePanelData(this.completeTimePanelData, _extends({
                isGroupedAllDayPanel: calculateIsGroupedAllDayPanel(this.props.groups, this.groupOrientation, this.isAllDayPanelVisible),
                isVerticalGrouping: this.isVerticalGrouping,
                isAllDayPanelVisible: this.isAllDayPanelVisible
            }, this.correctedVirtualScrollingState))
        })()
    }
    get viewDataProvider() {
        if (void 0 !== this.__getterCache.viewDataProvider) {
            return this.__getterCache.viewDataProvider
        }
        return this.__getterCache.viewDataProvider = (() => {
            var {
                cellDuration: cellDuration,
                currentDate: currentDate,
                endDayHour: endDayHour,
                firstDayOfWeek: firstDayOfWeek,
                groups: groups,
                hoursInterval: hoursInterval,
                intervalCount: intervalCount,
                startDate: startDate,
                startDayHour: startDayHour,
                type: type
            } = this.props;
            var viewDataProvider = new ViewDataProvider(type);
            viewDataProvider.completeViewDataMap = this.completeViewDataMap;
            viewDataProvider.viewDataMap = this.viewDataMap;
            viewDataProvider.viewData = this.viewData;
            var generationOptions = prepareGenerationOptions({
                intervalCount: intervalCount,
                groups: groups,
                groupByDate: this.isGroupedByDate,
                groupOrientation: this.groupOrientation,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                currentDate: currentDate,
                startDate: startDate,
                firstDayOfWeek: firstDayOfWeek,
                hoursInterval: hoursInterval,
                type: type,
                cellDuration: cellDuration
            }, this.renderConfig, this.isAllDayPanelVisible, this.correctedVirtualScrollingState);
            viewDataProvider.setViewOptions(generationOptions);
            viewDataProvider.createGroupedDataMapProvider();
            return viewDataProvider
        })()
    }
    get groupPanelData() {
        if (void 0 !== this.__getterCache.groupPanelData) {
            return this.__getterCache.groupPanelData
        }
        return this.__getterCache.groupPanelData = (() => {
            var {
                currentDate: currentDate,
                endDayHour: endDayHour,
                groups: groups,
                hoursInterval: hoursInterval,
                intervalCount: intervalCount,
                startDayHour: startDayHour,
                type: type
            } = this.props;
            var columnCountPerGroup = this.viewDataGenerator.getCellCount({
                intervalCount: intervalCount,
                hoursInterval: hoursInterval,
                currentDate: currentDate,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                viewType: type
            });
            var groupPanelData = getGroupPanelData(groups, columnCountPerGroup, this.isGroupedByDate, this.isGroupedByDate ? 1 : columnCountPerGroup);
            return groupPanelData
        })()
    }
    get headerPanelTemplate() {
        var {
            headerPanelTemplate: headerPanelTemplate
        } = this.renderConfig;
        return headerPanelTemplate
    }
    get dateTableTemplate() {
        var {
            dateTableTemplate: dateTableTemplate
        } = this.renderConfig;
        return dateTableTemplate
    }
    get timePanelTemplate() {
        var {
            timePanelTemplate: timePanelTemplate
        } = this.renderConfig;
        return timePanelTemplate
    }
    get isRenderHeaderEmptyCell() {
        return this.isVerticalGrouping || !!this.timePanelTemplate
    }
    get isWorkSpaceWithOddCells() {
        return false
    }
    get classes() {
        var {
            allDayPanelExpanded: allDayPanelExpanded,
            groups: groups,
            intervalCount: intervalCount
        } = this.props;
        return combineClasses({
            [this.renderConfig.className]: true,
            "dx-scheduler-work-space-count": intervalCount > 1,
            "dx-scheduler-work-space-odd-cells": !!this.isWorkSpaceWithOddCells,
            "dx-scheduler-work-space-all-day-collapsed": !allDayPanelExpanded && this.isAllDayPanelVisible,
            "dx-scheduler-work-space-all-day": this.isAllDayPanelVisible,
            "dx-scheduler-work-space-group-by-date": this.isGroupedByDate,
            "dx-scheduler-work-space-grouped": groups.length > 0,
            "dx-scheduler-work-space-vertical-grouped": this.isVerticalGrouping && "vertical" !== this.renderConfig.defaultGroupOrientation,
            "dx-scheduler-work-space-horizontal-grouped": isHorizontalGroupingApplied(groups, this.groupOrientation) && "vertical" === this.renderConfig.defaultGroupOrientation,
            "dx-scheduler-group-column-count-one": this.isVerticalGrouping && 1 === groups.length,
            "dx-scheduler-group-column-count-two": this.isVerticalGrouping && 2 === groups.length,
            "dx-scheduler-group-column-count-three": this.isVerticalGrouping && 3 === groups.length,
            "dx-scheduler-work-space-both-scrollbar": this.props.crossScrollingEnabled,
            "dx-scheduler-work-space": true
        })
    }
    get isStandaloneAllDayPanel() {
        var {
            groups: groups
        } = this.props;
        return !isVerticalGroupingApplied(groups, this.groupOrientation) && this.isAllDayPanelVisible
    }
    get isCalculateTablesWidth() {
        return this.props.crossScrollingEnabled && "vertical" !== this.renderConfig.defaultGroupOrientation
    }
    createDateTableElementsMeta(totalCellCount) {
        var dateTableCells = this.dateTableRef.current.querySelectorAll("td:not(.dx-scheduler-virtual-cell)");
        var dateTableRect = this.dateTableRef.current.getBoundingClientRect();
        var dateTableCellsMeta = [];
        dateTableCells.forEach((cellElement, index) => {
            if (index % totalCellCount === 0) {
                dateTableCellsMeta.push([])
            }
            var cellRect = cellElement.getBoundingClientRect();
            var validCellRect = createCellElementMetaData(dateTableRect, cellRect);
            dateTableCellsMeta[dateTableCellsMeta.length - 1].push(validCellRect)
        });
        return dateTableCellsMeta
    }
    createAllDayPanelElementsMeta() {
        if (!this.allDayPanelRef.current) {
            return []
        }
        var allDayPanelCells = this.allDayPanelRef.current.querySelectorAll("td");
        var allDayPanelRect = this.allDayPanelRef.current.getBoundingClientRect();
        var allDayPanelCellsMeta = [];
        allDayPanelCells.forEach(cellElement => {
            var cellRect = cellElement.getBoundingClientRect();
            allDayPanelCellsMeta.push(createCellElementMetaData(allDayPanelRect, cellRect))
        });
        return allDayPanelCellsMeta
    }
    onScroll(event) {
        if ("virtual" === this.props.scrolling.mode) {
            this.state.virtualScrolling.handleOnScrollEvent(event.scrollOffset);
            var nextState = this.state.virtualScrolling.getRenderState();
            var isUpdateState = Object.entries(nextState).some(_ref3 => {
                var [key, value] = _ref3;
                return value !== this.state.virtualScrollingData.state[key]
            });
            if (isUpdateState) {
                this.setState(__state_argument => ({
                    virtualScrollingData: {
                        state: nextState,
                        sizes: __state_argument.virtualScrollingData.sizes
                    }
                }))
            }
        }
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        super.componentWillUpdate();
        if (this.props.type !== nextProps.type || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.intervalCount !== nextProps.intervalCount || this.props.groups !== nextProps.groups || this.props.groupOrientation !== nextProps.groupOrientation) {
            this.__getterCache.renderConfig = void 0
        }
        if (this.props.type !== nextProps.type) {
            this.__getterCache.viewDataGenerator = void 0
        }
        if (this.props.type !== nextProps.type) {
            this.__getterCache.dateHeaderDataGenerator = void 0
        }
        if (this.props.type !== nextProps.type) {
            this.__getterCache.timePanelDataGenerator = void 0
        }
        if (this.props.currentDate !== nextProps.currentDate || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type) {
            this.__getterCache.startViewDate = void 0
        }
        if (this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.groupByDate !== nextProps.groupByDate || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.showAllDayPanel !== nextProps.showAllDayPanel) {
            this.__getterCache.completeViewDataMap = void 0
        }
        if (this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.groups !== nextProps.groups || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling || this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.groupByDate !== nextProps.groupByDate || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.showAllDayPanel !== nextProps.showAllDayPanel) {
            this.__getterCache.correctedVirtualScrollingState = void 0
        }
        if (this.props.type !== nextProps.type || this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.groupByDate !== nextProps.groupByDate || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.showAllDayPanel !== nextProps.showAllDayPanel || this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling) {
            this.__getterCache.viewDataMap = void 0
        }
        if (this.props.groups !== nextProps.groups || this.props.type !== nextProps.type || this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.groupByDate !== nextProps.groupByDate || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.showAllDayPanel !== nextProps.showAllDayPanel || this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling) {
            this.__getterCache.viewData = void 0
        }
        if (this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.groupOrientation !== nextProps.groupOrientation || this.props.groupByDate !== nextProps.groupByDate || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.startDate !== nextProps.startDate || this.props.cellDuration !== nextProps.cellDuration || this.props.showAllDayPanel !== nextProps.showAllDayPanel) {
            this.__getterCache.completeDateHeaderData = void 0
        }
        if (this.props.endDayHour !== nextProps.endDayHour || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.currentDate !== nextProps.currentDate || this.props.intervalCount !== nextProps.intervalCount || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.groupOrientation !== nextProps.groupOrientation || this.props.groupByDate !== nextProps.groupByDate || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.startDate !== nextProps.startDate || this.props.cellDuration !== nextProps.cellDuration || this.props.showAllDayPanel !== nextProps.showAllDayPanel || this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling) {
            this.__getterCache.dateHeaderData = void 0
        }
        if (this.props.type !== nextProps.type || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.intervalCount !== nextProps.intervalCount || this.props.groups !== nextProps.groups || this.props.groupOrientation !== nextProps.groupOrientation || this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.hoursInterval !== nextProps.hoursInterval || this.props.startDayHour !== nextProps.startDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.startDate !== nextProps.startDate || this.props.groupByDate !== nextProps.groupByDate || this.props.showAllDayPanel !== nextProps.showAllDayPanel) {
            this.__getterCache.completeTimePanelData = void 0
        }
        if (this.props.type !== nextProps.type || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.intervalCount !== nextProps.intervalCount || this.props.groups !== nextProps.groups || this.props.groupOrientation !== nextProps.groupOrientation || this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.hoursInterval !== nextProps.hoursInterval || this.props.startDayHour !== nextProps.startDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.startDate !== nextProps.startDate || this.props.groupByDate !== nextProps.groupByDate || this.props.showAllDayPanel !== nextProps.showAllDayPanel || this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling) {
            this.__getterCache.timePanelData = void 0
        }
        if (this.props.cellDuration !== nextProps.cellDuration || this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDate !== nextProps.startDate || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.groupByDate !== nextProps.groupByDate || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.showAllDayPanel !== nextProps.showAllDayPanel || this.state.virtualScrollingData !== nextState.virtualScrollingData || this.props.schedulerHeight !== nextProps.schedulerHeight || this.props.schedulerWidth !== nextProps.schedulerWidth || this.props.scrolling !== nextProps.scrolling || this.state.virtualScrolling !== nextState.virtualScrolling) {
            this.__getterCache.viewDataProvider = void 0
        }
        if (this.props.currentDate !== nextProps.currentDate || this.props.endDayHour !== nextProps.endDayHour || this.props.groups !== nextProps.groups || this.props.hoursInterval !== nextProps.hoursInterval || this.props.intervalCount !== nextProps.intervalCount || this.props.startDayHour !== nextProps.startDayHour || this.props.type !== nextProps.type || this.props.groupOrientation !== nextProps.groupOrientation || this.props.crossScrollingEnabled !== nextProps.crossScrollingEnabled || this.props.groupByDate !== nextProps.groupByDate) {
            this.__getterCache.groupPanelData = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                dataCellTemplate: getTemplate(props.dataCellTemplate),
                dateCellTemplate: getTemplate(props.dateCellTemplate),
                timeCellTemplate: getTemplate(props.timeCellTemplate),
                resourceCellTemplate: getTemplate(props.resourceCellTemplate)
            }),
            groupPanelHeight: this.state.groupPanelHeight,
            headerEmptyCellWidth: this.state.headerEmptyCellWidth,
            tablesWidth: this.state.tablesWidth,
            virtualScrolling: this.state.virtualScrolling,
            virtualScrollingData: this.state.virtualScrollingData,
            dateTableRef: this.dateTableRef,
            allDayPanelRef: this.allDayPanelRef,
            timePanelRef: this.timePanelRef,
            groupPanelRef: this.groupPanelRef,
            widgetElementRef: this.widgetElementRef,
            layoutRef: this.layoutRef,
            renderConfig: this.renderConfig,
            groupOrientation: this.groupOrientation,
            isVerticalGrouping: this.isVerticalGrouping,
            isHorizontalGrouping: this.isHorizontalGrouping,
            isGroupedByDate: this.isGroupedByDate,
            layout: this.layout,
            isAllDayPanelVisible: this.isAllDayPanelVisible,
            viewDataGenerator: this.viewDataGenerator,
            dateHeaderDataGenerator: this.dateHeaderDataGenerator,
            timePanelDataGenerator: this.timePanelDataGenerator,
            startViewDate: this.startViewDate,
            completeViewDataMap: this.completeViewDataMap,
            correctedVirtualScrollingState: this.correctedVirtualScrollingState,
            viewDataMap: this.viewDataMap,
            viewData: this.viewData,
            completeDateHeaderData: this.completeDateHeaderData,
            dateHeaderData: this.dateHeaderData,
            completeTimePanelData: this.completeTimePanelData,
            timePanelData: this.timePanelData,
            viewDataProvider: this.viewDataProvider,
            groupPanelData: this.groupPanelData,
            headerPanelTemplate: this.headerPanelTemplate,
            dateTableTemplate: this.dateTableTemplate,
            timePanelTemplate: this.timePanelTemplate,
            isRenderHeaderEmptyCell: this.isRenderHeaderEmptyCell,
            isWorkSpaceWithOddCells: this.isWorkSpaceWithOddCells,
            classes: this.classes,
            isStandaloneAllDayPanel: this.isStandaloneAllDayPanel,
            isCalculateTablesWidth: this.isCalculateTablesWidth,
            createDateTableElementsMeta: this.createDateTableElementsMeta,
            createAllDayPanelElementsMeta: this.createAllDayPanelElementsMeta,
            onScroll: this.onScroll,
            restAttributes: this.restAttributes
        })
    }
}
WorkSpace.defaultProps = WorkSpaceProps;
