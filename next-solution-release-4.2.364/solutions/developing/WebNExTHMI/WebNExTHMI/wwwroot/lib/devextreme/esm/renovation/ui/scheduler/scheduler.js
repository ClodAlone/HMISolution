/**
 * DevExtreme (esm/renovation/ui/scheduler/scheduler.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["accessKey", "activeStateEnabled", "adaptivityEnabled", "allDayExpr", "appointmentCollectorTemplate", "appointmentDragging", "appointmentTemplate", "appointmentTooltipTemplate", "cellDuration", "className", "crossScrollingEnabled", "currentDate", "currentDateChange", "currentView", "currentViewChange", "customizeDateNavigatorText", "dataCellTemplate", "dataSource", "dateCellTemplate", "dateSerializationFormat", "defaultCurrentDate", "defaultCurrentView", "descriptionExpr", "disabled", "editing", "endDateExpr", "endDateTimeZoneExpr", "endDayHour", "firstDayOfWeek", "focusStateEnabled", "groupByDate", "groups", "height", "hint", "hoverStateEnabled", "indicatorUpdateInterval", "max", "maxAppointmentsPerCell", "min", "noDataText", "onAppointmentAdded", "onAppointmentAdding", "onAppointmentClick", "onAppointmentContextMenu", "onAppointmentDblClick", "onAppointmentDeleted", "onAppointmentDeleting", "onAppointmentFormOpening", "onAppointmentRendered", "onAppointmentUpdated", "onAppointmentUpdating", "onCellClick", "onCellContextMenu", "onClick", "onKeyDown", "recurrenceEditMode", "recurrenceExceptionExpr", "recurrenceRuleExpr", "remoteFiltering", "resourceCellTemplate", "resources", "rtlEnabled", "scrolling", "selectedCellData", "shadeUntilCurrentTime", "showAllDayPanel", "showCurrentTimeIndicator", "startDateExpr", "startDateTimeZoneExpr", "startDayHour", "tabIndex", "textExpr", "timeCellTemplate", "timeZone", "toolbar", "useDropDownViewSwitcher", "views", "visible", "width"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    SchedulerProps
} from "./props";
import {
    Widget
} from "../common/widget";
import DataSource from "../../../data/data_source";
import {
    getCurrentViewConfig,
    getCurrentViewProps
} from "./model/views";
import {
    WorkSpace
} from "./workspaces/base/work_space";
import {
    SchedulerToolbar
} from "./header/header";
import {
    getViewDataGeneratorByViewType
} from "../../../ui/scheduler/workspaces/view_model/utils";
import {
    createDataAccessors,
    isViewDataProviderConfigValid
} from "./common";
import {
    createTimeZoneCalculator
} from "./timeZoneCalculator/createTimeZoneCalculator";
import {
    getGroupCount,
    loadResources
} from "../../../ui/scheduler/resources/utils";
import {
    getAppointmentsViewModel
} from "./view_model/appointments/appointments";
import {
    getAppointmentsConfig,
    getAppointmentsModel
} from "./model/appointments";
import {
    AppointmentLayout
} from "./appointment/layout";
import {
    getViewRenderConfigByType
} from "./workspaces/base/work_space_config";
import {
    getPreparedDataItems,
    resolveDataItems
} from "./utils/data";
import {
    getFilterStrategy
} from "./utils/filter";
export var viewFunction = _ref => {
    var {
        appointmentsViewModel: appointmentsViewModel,
        currentViewConfig: currentViewConfig,
        dataCellTemplate: dataCellTemplate,
        dateCellTemplate: dateCellTemplate,
        loadedResources: loadedResources,
        onViewRendered: onViewRendered,
        props: {
            accessKey: accessKey,
            activeStateEnabled: activeStateEnabled,
            className: className,
            currentView: currentView,
            customizeDateNavigatorText: customizeDateNavigatorText,
            disabled: disabled,
            focusStateEnabled: focusStateEnabled,
            height: height,
            hint: hint,
            hoverStateEnabled: hoverStateEnabled,
            max: max,
            min: min,
            rtlEnabled: rtlEnabled,
            tabIndex: tabIndex,
            toolbar: toolbarItems,
            useDropDownViewSwitcher: useDropDownViewSwitcher,
            views: views,
            visible: visible,
            width: width
        },
        resourceCellTemplate: resourceCellTemplate,
        restAttributes: restAttributes,
        setCurrentDate: setCurrentDate,
        setCurrentView: setCurrentView,
        startViewDate: startViewDate,
        timeCellTemplate: timeCellTemplate,
        workSpaceKey: workSpaceKey
    } = _ref;
    var {
        allDayPanelExpanded: allDayPanelExpanded,
        allowMultipleCellSelection: allowMultipleCellSelection,
        cellDuration: cellDuration,
        crossScrollingEnabled: crossScrollingEnabled,
        currentDate: currentDate,
        endDayHour: endDayHour,
        firstDayOfWeek: firstDayOfWeek,
        groupByDate: groupByDate,
        groupOrientation: groupOrientation,
        hoursInterval: hoursInterval,
        indicatorTime: indicatorTime,
        indicatorUpdateInterval: indicatorUpdateInterval,
        intervalCount: intervalCount,
        scrolling: scrolling,
        shadeUntilCurrentTime: shadeUntilCurrentTime,
        showAllDayPanel: showAllDayPanel,
        showCurrentTimeIndicator: showCurrentTimeIndicator,
        startDate: startDate,
        startDayHour: startDayHour,
        type: type
    } = currentViewConfig;
    return normalizeProps(createComponentVNode(2, Widget, _extends({
        classes: "dx-scheduler dx-scheduler-native",
        accessKey: accessKey,
        activeStateEnabled: activeStateEnabled,
        disabled: disabled,
        focusStateEnabled: focusStateEnabled,
        height: height,
        hint: hint,
        hoverStateEnabled: hoverStateEnabled,
        rtlEnabled: rtlEnabled,
        tabIndex: tabIndex,
        visible: visible,
        width: width,
        className: className
    }, restAttributes, {
        children: createVNode(1, "div", "dx-scheduler-container", [0 !== toolbarItems.length && createComponentVNode(2, SchedulerToolbar, {
            items: toolbarItems,
            views: views,
            currentView: currentView,
            onCurrentViewUpdate: setCurrentView,
            currentDate: currentDate,
            onCurrentDateUpdate: setCurrentDate,
            startViewDate: startViewDate,
            min: min,
            max: max,
            intervalCount: intervalCount,
            firstDayOfWeek: firstDayOfWeek,
            useDropDownViewSwitcher: useDropDownViewSwitcher,
            customizationFunction: customizeDateNavigatorText,
            viewType: type
        }), createComponentVNode(2, WorkSpace, {
            firstDayOfWeek: firstDayOfWeek,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            cellDuration: cellDuration,
            groupByDate: groupByDate,
            scrolling: scrolling,
            currentDate: currentDate,
            intervalCount: intervalCount,
            groupOrientation: groupOrientation,
            startDate: startDate,
            showAllDayPanel: showAllDayPanel,
            showCurrentTimeIndicator: showCurrentTimeIndicator,
            indicatorUpdateInterval: indicatorUpdateInterval,
            shadeUntilCurrentTime: shadeUntilCurrentTime,
            crossScrollingEnabled: crossScrollingEnabled,
            hoursInterval: hoursInterval,
            groups: loadedResources,
            type: type,
            schedulerHeight: height,
            schedulerWidth: width,
            indicatorTime: indicatorTime,
            allowMultipleCellSelection: allowMultipleCellSelection,
            allDayPanelExpanded: allDayPanelExpanded,
            onViewRendered: onViewRendered,
            dataCellTemplate: dataCellTemplate,
            timeCellTemplate: timeCellTemplate,
            dateCellTemplate: dateCellTemplate,
            resourceCellTemplate: resourceCellTemplate,
            allDayAppointments: createComponentVNode(2, AppointmentLayout, {
                isAllDay: true,
                appointments: appointmentsViewModel.allDay,
                overflowIndicators: appointmentsViewModel.allDayCompact
            }),
            appointments: createComponentVNode(2, AppointmentLayout, {
                appointments: appointmentsViewModel.regular,
                overflowIndicators: appointmentsViewModel.regularCompact
            })
        }, workSpaceKey)], 0)
    })))
};
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class Scheduler extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.__getterCache = {};
        this.state = {
            instance: void 0,
            workSpaceViewModel: void 0,
            resourcePromisesMap: new Map,
            loadedResources: void 0,
            dataItems: [],
            currentDate: void 0 !== this.props.currentDate ? this.props.currentDate : this.props.defaultCurrentDate,
            currentView: void 0 !== this.props.currentView ? this.props.currentView : this.props.defaultCurrentView
        };
        this.getComponentInstance = this.getComponentInstance.bind(this);
        this.addAppointment = this.addAppointment.bind(this);
        this.deleteAppointment = this.deleteAppointment.bind(this);
        this.updateAppointment = this.updateAppointment.bind(this);
        this.getDataSource = this.getDataSource.bind(this);
        this.getEndViewDate = this.getEndViewDate.bind(this);
        this.getStartViewDate = this.getStartViewDate.bind(this);
        this.hideAppointmentPopup = this.hideAppointmentPopup.bind(this);
        this.hideAppointmentTooltip = this.hideAppointmentTooltip.bind(this);
        this.scrollTo = this.scrollTo.bind(this);
        this.scrollToTime = this.scrollToTime.bind(this);
        this.showAppointmentPopup = this.showAppointmentPopup.bind(this);
        this.showAppointmentTooltip = this.showAppointmentTooltip.bind(this);
        this.dispose = this.dispose.bind(this);
        this.loadGroupResources = this.loadGroupResources.bind(this);
        this.loadDataSource = this.loadDataSource.bind(this);
        this.onViewRendered = this.onViewRendered.bind(this);
        this.setCurrentView = this.setCurrentView.bind(this);
        this.setCurrentDate = this.setCurrentDate.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.dispose, []), new InfernoEffect(this.loadGroupResources, [this.props.groups, this.props.resources, this.state.currentView, this.props.currentView, this.props.views, this.state.resourcePromisesMap]), new InfernoEffect(this.loadDataSource, [this.props.dataSource]), createReRenderEffect()]
    }
    updateEffects() {
        var _this$_effects$, _this$_effects$2;
        null === (_this$_effects$ = this._effects[1]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.groups, this.props.resources, this.state.currentView, this.props.currentView, this.props.views, this.state.resourcePromisesMap]);
        null === (_this$_effects$2 = this._effects[2]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props.dataSource])
    }
    dispose() {
        return () => {
            this.state.instance.dispose()
        }
    }
    loadGroupResources() {
        var {
            groups: schedulerGroups,
            resources: resources
        } = this.props;
        var {
            groups: currentViewProps
        } = this.currentViewProps;
        var validGroups = null !== currentViewProps && void 0 !== currentViewProps ? currentViewProps : schedulerGroups;
        loadResources(validGroups, resources, this.state.resourcePromisesMap).then(loadedResources => {
            this.setState(__state_argument => ({
                loadedResources: loadedResources
            }))
        })
    }
    loadDataSource() {
        if (!this.internalDataSource.isLoaded() && !this.internalDataSource.isLoading()) {
            this.internalDataSource.load().done(loadOptions => {
                this.setState(__state_argument => ({
                    dataItems: resolveDataItems(loadOptions)
                }))
            })
        }
    }
    get currentViewProps() {
        var {
            views: views
        } = this.props;
        return getCurrentViewProps(void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView, views)
    }
    get currentViewConfig() {
        if (void 0 !== this.__getterCache.currentViewConfig) {
            return this.__getterCache.currentViewConfig
        }
        return this.__getterCache.currentViewConfig = (() => getCurrentViewConfig(this.currentViewProps, _extends({}, this.props, {
            currentDate: void 0 !== this.props.currentDate ? this.props.currentDate : this.state.currentDate,
            currentView: void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView
        })))()
    }
    get isValidViewDataProvider() {
        var _this$state$workSpace;
        var {
            allDayPanelExpanded: allDayPanelExpanded,
            cellDuration: cellDuration,
            crossScrollingEnabled: crossScrollingEnabled,
            currentDate: currentDate,
            endDayHour: endDayHour,
            firstDayOfWeek: firstDayOfWeek,
            groupByDate: groupByDate,
            groupOrientation: groupOrientation,
            hoursInterval: hoursInterval,
            intervalCount: intervalCount,
            scrolling: scrolling,
            showAllDayPanel: showAllDayPanel,
            startDate: startDate,
            startDayHour: startDayHour,
            type: type
        } = this.currentViewConfig;
        return isViewDataProviderConfigValid(null === (_this$state$workSpace = this.state.workSpaceViewModel) || void 0 === _this$state$workSpace ? void 0 : _this$state$workSpace.viewDataProviderValidationOptions, {
            intervalCount: null !== intervalCount && void 0 !== intervalCount ? intervalCount : 1,
            currentDate: currentDate,
            type: type,
            hoursInterval: hoursInterval,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            groupOrientation: groupOrientation,
            groupByDate: groupByDate,
            crossScrollingEnabled: crossScrollingEnabled,
            firstDayOfWeek: firstDayOfWeek,
            startDate: startDate,
            showAllDayPanel: showAllDayPanel,
            allDayPanelExpanded: allDayPanelExpanded,
            scrolling: scrolling,
            cellDuration: cellDuration,
            groups: this.state.loadedResources
        })
    }
    get dataAccessors() {
        if (void 0 !== this.__getterCache.dataAccessors) {
            return this.__getterCache.dataAccessors
        }
        return this.__getterCache.dataAccessors = (() => createDataAccessors(_extends({}, this.props, {
            currentDate: void 0 !== this.props.currentDate ? this.props.currentDate : this.state.currentDate,
            currentView: void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView
        })))()
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
            } = this.currentViewConfig;
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
    get isVirtualScrolling() {
        var _this$currentViewProp;
        return "virtual" === this.props.scrolling.mode || "virtual" === (null === (_this$currentViewProp = this.currentViewProps.scrolling) || void 0 === _this$currentViewProp ? void 0 : _this$currentViewProp.mode)
    }
    get timeZoneCalculator() {
        if (void 0 !== this.__getterCache.timeZoneCalculator) {
            return this.__getterCache.timeZoneCalculator
        }
        return this.__getterCache.timeZoneCalculator = (() => createTimeZoneCalculator(this.props.timeZone))()
    }
    get internalDataSource() {
        if (this.props.dataSource instanceof DataSource) {
            return this.props.dataSource
        }
        if (this.props.dataSource instanceof Array) {
            return new DataSource({
                store: {
                    type: "array",
                    data: this.props.dataSource
                },
                paginate: false
            })
        }
        return new DataSource(this.props.dataSource)
    }
    get appointmentsConfig() {
        if (void 0 !== this.__getterCache.appointmentsConfig) {
            return this.__getterCache.appointmentsConfig
        }
        return this.__getterCache.appointmentsConfig = (() => {
            if (!this.isValidViewDataProvider || !this.state.loadedResources) {
                return
            }
            var renderConfig = getViewRenderConfigByType(this.currentViewConfig.type, this.currentViewConfig.crossScrollingEnabled, this.currentViewConfig.intervalCount, this.state.loadedResources, this.currentViewConfig.groupOrientation);
            return getAppointmentsConfig(_extends({}, this.props, {
                currentDate: void 0 !== this.props.currentDate ? this.props.currentDate : this.state.currentDate,
                currentView: void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView
            }), this.currentViewConfig, this.state.loadedResources, this.state.workSpaceViewModel.viewDataProvider, renderConfig.isAllDayPanelSupported)
        })()
    }
    get preparedDataItems() {
        if (void 0 !== this.__getterCache.preparedDataItems) {
            return this.__getterCache.preparedDataItems
        }
        return this.__getterCache.preparedDataItems = (() => getPreparedDataItems(this.state.dataItems, this.dataAccessors, this.currentViewConfig.cellDuration, this.timeZoneCalculator))()
    }
    get filteredItems() {
        if (void 0 !== this.__getterCache.filteredItems) {
            return this.__getterCache.filteredItems
        }
        return this.__getterCache.filteredItems = (() => {
            if (!this.appointmentsConfig) {
                return []
            }
            var filterStrategy = getFilterStrategy(this.appointmentsConfig.resources, this.appointmentsConfig.startDayHour, this.appointmentsConfig.endDayHour, this.appointmentsConfig.cellDurationInMinutes, this.appointmentsConfig.showAllDayPanel, this.appointmentsConfig.supportAllDayRow, this.appointmentsConfig.firstDayOfWeek, this.appointmentsConfig.viewType, this.appointmentsConfig.dateRange, this.appointmentsConfig.groupCount, this.appointmentsConfig.loadedResources, this.appointmentsConfig.isVirtualScrolling, this.timeZoneCalculator, this.dataAccessors, this.state.workSpaceViewModel.viewDataProvider);
            return filterStrategy.filter(this.preparedDataItems)
        })()
    }
    get appointmentsViewModel() {
        if (void 0 !== this.__getterCache.appointmentsViewModel) {
            return this.__getterCache.appointmentsViewModel
        }
        return this.__getterCache.appointmentsViewModel = (() => {
            if (!this.appointmentsConfig || 0 === this.filteredItems.length) {
                return {
                    allDay: [],
                    allDayCompact: [],
                    regular: [],
                    regularCompact: []
                }
            }
            var model = getAppointmentsModel(this.appointmentsConfig, this.state.workSpaceViewModel.viewDataProvider, this.timeZoneCalculator, this.dataAccessors, this.state.workSpaceViewModel.cellsMetaData);
            return getAppointmentsViewModel(model, this.filteredItems)
        })()
    }
    get workSpaceKey() {
        var _this$state$loadedRes;
        var {
            crossScrollingEnabled: crossScrollingEnabled
        } = this.props;
        var {
            groupOrientation: groupOrientation,
            intervalCount: intervalCount
        } = this.currentViewConfig;
        if (!crossScrollingEnabled) {
            return ""
        }
        var groupCount = getGroupCount(null !== (_this$state$loadedRes = this.state.loadedResources) && void 0 !== _this$state$loadedRes ? _this$state$loadedRes : []);
        return "".concat(void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView, "_").concat(groupOrientation, "_").concat(intervalCount, "_").concat(groupCount)
    }
    get dataCellTemplate() {
        return this.currentViewConfig.dataCellTemplate
    }
    get dateCellTemplate() {
        return this.currentViewConfig.dateCellTemplate
    }
    get timeCellTemplate() {
        return this.currentViewConfig.timeCellTemplate
    }
    get resourceCellTemplate() {
        return this.currentViewConfig.resourceCellTemplate
    }
    onViewRendered(viewMetaData) {
        this.setState(__state_argument => ({
            workSpaceViewModel: viewMetaData
        }))
    }
    setCurrentView(view) {
        var __newValue;
        this.setState(__state_argument => {
            __newValue = view;
            return {
                currentView: __newValue
            }
        });
        this.props.currentViewChange(__newValue)
    }
    setCurrentDate(date) {
        var __newValue;
        this.setState(__state_argument => {
            __newValue = date;
            return {
                currentDate: __newValue
            }
        });
        this.props.currentDateChange(__newValue)
    }
    get restAttributes() {
        var _this$props$currentDa = _extends({}, this.props, {
                currentDate: void 0 !== this.props.currentDate ? this.props.currentDate : this.state.currentDate,
                currentView: void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView
            }),
            restProps = _objectWithoutPropertiesLoose(_this$props$currentDa, _excluded);
        return restProps
    }
    getComponentInstance() {
        return this.state.instance
    }
    addAppointment(appointment) {
        this.state.instance.addAppointment(appointment)
    }
    deleteAppointment(appointment) {
        this.state.instance.deleteAppointment(appointment)
    }
    updateAppointment(target, appointment) {
        this.state.instance.updateAppointment(target, appointment)
    }
    getDataSource() {
        return this.state.instance.getDataSource()
    }
    getEndViewDate() {
        return this.state.instance.getEndViewDate()
    }
    getStartViewDate() {
        return this.state.instance.getStartViewDate()
    }
    hideAppointmentPopup(saveChanges) {
        this.state.instance.hideAppointmentPopup(saveChanges)
    }
    hideAppointmentTooltip() {
        this.state.instance.hideAppointmentTooltip()
    }
    scrollTo(date, group, allDay) {
        this.state.instance.scrollTo(date, group, allDay)
    }
    scrollToTime(hours, minutes, date) {
        this.state.instance.scrollToTime(hours, minutes, date)
    }
    showAppointmentPopup(appointmentData, createNewAppointment, currentAppointmentData) {
        this.state.instance.showAppointmentPopup(appointmentData, createNewAppointment, currentAppointmentData)
    }
    showAppointmentTooltip(appointmentData, target, currentAppointmentData) {
        this.state.instance.showAppointmentTooltip(appointmentData, target, currentAppointmentData)
    }
    componentWillUpdate(nextProps, nextState, context) {
        super.componentWillUpdate();
        if (this.props.views !== nextProps.views || this.state.currentView !== nextState.currentView || this.props.currentView !== nextProps.currentView || this.props !== nextProps) {
            this.__getterCache.currentViewConfig = void 0
        }
        if (this.props !== nextProps) {
            this.__getterCache.dataAccessors = void 0
        }
        if (this.props.views !== nextProps.views || this.state.currentView !== nextState.currentView || this.props.currentView !== nextProps.currentView || this.props !== nextProps) {
            this.__getterCache.startViewDate = void 0
        }
        if (this.props.timeZone !== nextProps.timeZone) {
            this.__getterCache.timeZoneCalculator = void 0
        }
        if (this.props.views !== nextProps.views || this.state.currentView !== nextState.currentView || this.props.currentView !== nextProps.currentView || this.props !== nextProps || this.state.workSpaceViewModel !== nextState.workSpaceViewModel || this.state.loadedResources !== nextState.loadedResources) {
            this.__getterCache.appointmentsConfig = void 0
        }
        if (this.state.dataItems !== nextState.dataItems || this.props !== nextProps || this.props.views !== nextProps.views || this.state.currentView !== nextState.currentView || this.props.currentView !== nextProps.currentView || this.props.timeZone !== nextProps.timeZone) {
            this.__getterCache.preparedDataItems = void 0
        }
        if (this.props.views !== nextProps.views || this.state.currentView !== nextState.currentView || this.props.currentView !== nextProps.currentView || this.props !== nextProps || this.state.workSpaceViewModel !== nextState.workSpaceViewModel || this.state.loadedResources !== nextState.loadedResources || this.props.timeZone !== nextProps.timeZone || this.state.dataItems !== nextState.dataItems) {
            this.__getterCache.filteredItems = void 0
        }
        if (this.props.views !== nextProps.views || this.state.currentView !== nextState.currentView || this.props.currentView !== nextProps.currentView || this.props !== nextProps || this.state.workSpaceViewModel !== nextState.workSpaceViewModel || this.state.loadedResources !== nextState.loadedResources || this.props.timeZone !== nextProps.timeZone || this.state.dataItems !== nextState.dataItems) {
            this.__getterCache.appointmentsViewModel = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                currentDate: void 0 !== this.props.currentDate ? this.props.currentDate : this.state.currentDate,
                currentView: void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView,
                dataCellTemplate: getTemplate(props.dataCellTemplate),
                dateCellTemplate: getTemplate(props.dateCellTemplate),
                timeCellTemplate: getTemplate(props.timeCellTemplate),
                resourceCellTemplate: getTemplate(props.resourceCellTemplate),
                appointmentCollectorTemplate: getTemplate(props.appointmentCollectorTemplate),
                appointmentTemplate: getTemplate(props.appointmentTemplate),
                appointmentTooltipTemplate: getTemplate(props.appointmentTooltipTemplate)
            }),
            instance: this.state.instance,
            workSpaceViewModel: this.state.workSpaceViewModel,
            resourcePromisesMap: this.state.resourcePromisesMap,
            loadedResources: this.state.loadedResources,
            dataItems: this.state.dataItems,
            currentViewProps: this.currentViewProps,
            currentViewConfig: this.currentViewConfig,
            isValidViewDataProvider: this.isValidViewDataProvider,
            dataAccessors: this.dataAccessors,
            startViewDate: this.startViewDate,
            isVirtualScrolling: this.isVirtualScrolling,
            timeZoneCalculator: this.timeZoneCalculator,
            internalDataSource: this.internalDataSource,
            appointmentsConfig: this.appointmentsConfig,
            preparedDataItems: this.preparedDataItems,
            filteredItems: this.filteredItems,
            appointmentsViewModel: this.appointmentsViewModel,
            workSpaceKey: this.workSpaceKey,
            dataCellTemplate: this.dataCellTemplate,
            dateCellTemplate: this.dateCellTemplate,
            timeCellTemplate: this.timeCellTemplate,
            resourceCellTemplate: this.resourceCellTemplate,
            onViewRendered: this.onViewRendered,
            setCurrentView: this.setCurrentView,
            setCurrentDate: this.setCurrentDate,
            restAttributes: this.restAttributes
        })
    }
}
Scheduler.defaultProps = SchedulerProps;
