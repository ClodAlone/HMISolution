/**
 * DevExtreme (cjs/renovation/ui/scheduler/scheduler.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.Scheduler = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _props = require("./props");
var _widget = require("../common/widget");
var _data_source = _interopRequireDefault(require("../../../data/data_source"));
var _views = require("./model/views");
var _work_space = require("./workspaces/base/work_space");
var _header = require("./header/header");
var _utils = require("../../../ui/scheduler/workspaces/view_model/utils");
var _common = require("./common");
var _createTimeZoneCalculator = require("./timeZoneCalculator/createTimeZoneCalculator");
var _utils2 = require("../../../ui/scheduler/resources/utils");
var _appointments = require("./view_model/appointments/appointments");
var _appointments2 = require("./model/appointments");
var _layout = require("./appointment/layout");
var _work_space_config = require("./workspaces/base/work_space_config");
var _data = require("./utils/data");
var _filter = require("./utils/filter");
var _excluded = ["accessKey", "activeStateEnabled", "adaptivityEnabled", "allDayExpr", "appointmentCollectorTemplate", "appointmentDragging", "appointmentTemplate", "appointmentTooltipTemplate", "cellDuration", "className", "crossScrollingEnabled", "currentDate", "currentDateChange", "currentView", "currentViewChange", "customizeDateNavigatorText", "dataCellTemplate", "dataSource", "dateCellTemplate", "dateSerializationFormat", "defaultCurrentDate", "defaultCurrentView", "descriptionExpr", "disabled", "editing", "endDateExpr", "endDateTimeZoneExpr", "endDayHour", "firstDayOfWeek", "focusStateEnabled", "groupByDate", "groups", "height", "hint", "hoverStateEnabled", "indicatorUpdateInterval", "max", "maxAppointmentsPerCell", "min", "noDataText", "onAppointmentAdded", "onAppointmentAdding", "onAppointmentClick", "onAppointmentContextMenu", "onAppointmentDblClick", "onAppointmentDeleted", "onAppointmentDeleting", "onAppointmentFormOpening", "onAppointmentRendered", "onAppointmentUpdated", "onAppointmentUpdating", "onCellClick", "onCellContextMenu", "onClick", "onKeyDown", "recurrenceEditMode", "recurrenceExceptionExpr", "recurrenceRuleExpr", "remoteFiltering", "resourceCellTemplate", "resources", "rtlEnabled", "scrolling", "selectedCellData", "shadeUntilCurrentTime", "showAllDayPanel", "showCurrentTimeIndicator", "startDateExpr", "startDateTimeZoneExpr", "startDayHour", "tabIndex", "textExpr", "timeCellTemplate", "timeZone", "toolbar", "useDropDownViewSwitcher", "views", "visible", "width"];

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _objectWithoutProperties(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = _objectWithoutPropertiesLoose(source, excluded);
    var key, i;
    if (Object.getOwnPropertySymbols) {
        var sourceSymbolKeys = Object.getOwnPropertySymbols(source);
        for (i = 0; i < sourceSymbolKeys.length; i++) {
            key = sourceSymbolKeys[i];
            if (excluded.indexOf(key) >= 0) {
                continue
            }
            if (!Object.prototype.propertyIsEnumerable.call(source, key)) {
                continue
            }
            target[key] = source[key]
        }
    }
    return target
}

function _objectWithoutPropertiesLoose(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = {};
    var sourceKeys = Object.keys(source);
    var key, i;
    for (i = 0; i < sourceKeys.length; i++) {
        key = sourceKeys[i];
        if (excluded.indexOf(key) >= 0) {
            continue
        }
        target[key] = source[key]
    }
    return target
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}

function _assertThisInitialized(self) {
    if (void 0 === self) {
        throw new ReferenceError("this hasn't been initialised - super() hasn't been called")
    }
    return self
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}
var viewFunction = function(_ref) {
    var appointmentsViewModel = _ref.appointmentsViewModel,
        currentViewConfig = _ref.currentViewConfig,
        dataCellTemplate = _ref.dataCellTemplate,
        dateCellTemplate = _ref.dateCellTemplate,
        loadedResources = _ref.loadedResources,
        onViewRendered = _ref.onViewRendered,
        _ref$props = _ref.props,
        accessKey = _ref$props.accessKey,
        activeStateEnabled = _ref$props.activeStateEnabled,
        className = _ref$props.className,
        currentView = _ref$props.currentView,
        customizeDateNavigatorText = _ref$props.customizeDateNavigatorText,
        disabled = _ref$props.disabled,
        focusStateEnabled = _ref$props.focusStateEnabled,
        height = _ref$props.height,
        hint = _ref$props.hint,
        hoverStateEnabled = _ref$props.hoverStateEnabled,
        max = _ref$props.max,
        min = _ref$props.min,
        rtlEnabled = _ref$props.rtlEnabled,
        tabIndex = _ref$props.tabIndex,
        toolbarItems = _ref$props.toolbar,
        useDropDownViewSwitcher = _ref$props.useDropDownViewSwitcher,
        views = _ref$props.views,
        visible = _ref$props.visible,
        width = _ref$props.width,
        resourceCellTemplate = _ref.resourceCellTemplate,
        restAttributes = _ref.restAttributes,
        setCurrentDate = _ref.setCurrentDate,
        setCurrentView = _ref.setCurrentView,
        startViewDate = _ref.startViewDate,
        timeCellTemplate = _ref.timeCellTemplate,
        workSpaceKey = _ref.workSpaceKey;
    var allDayPanelExpanded = currentViewConfig.allDayPanelExpanded,
        allowMultipleCellSelection = currentViewConfig.allowMultipleCellSelection,
        cellDuration = currentViewConfig.cellDuration,
        crossScrollingEnabled = currentViewConfig.crossScrollingEnabled,
        currentDate = currentViewConfig.currentDate,
        endDayHour = currentViewConfig.endDayHour,
        firstDayOfWeek = currentViewConfig.firstDayOfWeek,
        groupByDate = currentViewConfig.groupByDate,
        groupOrientation = currentViewConfig.groupOrientation,
        hoursInterval = currentViewConfig.hoursInterval,
        indicatorTime = currentViewConfig.indicatorTime,
        indicatorUpdateInterval = currentViewConfig.indicatorUpdateInterval,
        intervalCount = currentViewConfig.intervalCount,
        scrolling = currentViewConfig.scrolling,
        shadeUntilCurrentTime = currentViewConfig.shadeUntilCurrentTime,
        showAllDayPanel = currentViewConfig.showAllDayPanel,
        showCurrentTimeIndicator = currentViewConfig.showCurrentTimeIndicator,
        startDate = currentViewConfig.startDate,
        startDayHour = currentViewConfig.startDayHour,
        type = currentViewConfig.type;
    return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, _widget.Widget, _extends({
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
        children: (0, _inferno.createVNode)(1, "div", "dx-scheduler-container", [0 !== toolbarItems.length && (0, _inferno.createComponentVNode)(2, _header.SchedulerToolbar, {
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
        }), (0, _inferno.createComponentVNode)(2, _work_space.WorkSpace, {
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
            allDayAppointments: (0, _inferno.createComponentVNode)(2, _layout.AppointmentLayout, {
                isAllDay: true,
                appointments: appointmentsViewModel.allDay,
                overflowIndicators: appointmentsViewModel.allDayCompact
            }),
            appointments: (0, _inferno.createComponentVNode)(2, _layout.AppointmentLayout, {
                appointments: appointmentsViewModel.regular,
                overflowIndicators: appointmentsViewModel.regularCompact
            })
        }, workSpaceKey)], 0)
    })))
};
exports.viewFunction = viewFunction;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var Scheduler = function(_InfernoWrapperCompon) {
    _inheritsLoose(Scheduler, _InfernoWrapperCompon);

    function Scheduler(props) {
        var _this;
        _this = _InfernoWrapperCompon.call(this, props) || this;
        _this.__getterCache = {};
        _this.state = {
            instance: void 0,
            workSpaceViewModel: void 0,
            resourcePromisesMap: new Map,
            loadedResources: void 0,
            dataItems: [],
            currentDate: void 0 !== _this.props.currentDate ? _this.props.currentDate : _this.props.defaultCurrentDate,
            currentView: void 0 !== _this.props.currentView ? _this.props.currentView : _this.props.defaultCurrentView
        };
        _this.getComponentInstance = _this.getComponentInstance.bind(_assertThisInitialized(_this));
        _this.addAppointment = _this.addAppointment.bind(_assertThisInitialized(_this));
        _this.deleteAppointment = _this.deleteAppointment.bind(_assertThisInitialized(_this));
        _this.updateAppointment = _this.updateAppointment.bind(_assertThisInitialized(_this));
        _this.getDataSource = _this.getDataSource.bind(_assertThisInitialized(_this));
        _this.getEndViewDate = _this.getEndViewDate.bind(_assertThisInitialized(_this));
        _this.getStartViewDate = _this.getStartViewDate.bind(_assertThisInitialized(_this));
        _this.hideAppointmentPopup = _this.hideAppointmentPopup.bind(_assertThisInitialized(_this));
        _this.hideAppointmentTooltip = _this.hideAppointmentTooltip.bind(_assertThisInitialized(_this));
        _this.scrollTo = _this.scrollTo.bind(_assertThisInitialized(_this));
        _this.scrollToTime = _this.scrollToTime.bind(_assertThisInitialized(_this));
        _this.showAppointmentPopup = _this.showAppointmentPopup.bind(_assertThisInitialized(_this));
        _this.showAppointmentTooltip = _this.showAppointmentTooltip.bind(_assertThisInitialized(_this));
        _this.dispose = _this.dispose.bind(_assertThisInitialized(_this));
        _this.loadGroupResources = _this.loadGroupResources.bind(_assertThisInitialized(_this));
        _this.loadDataSource = _this.loadDataSource.bind(_assertThisInitialized(_this));
        _this.onViewRendered = _this.onViewRendered.bind(_assertThisInitialized(_this));
        _this.setCurrentView = _this.setCurrentView.bind(_assertThisInitialized(_this));
        _this.setCurrentDate = _this.setCurrentDate.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = Scheduler.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.dispose, []), new _inferno2.InfernoEffect(this.loadGroupResources, [this.props.groups, this.props.resources, this.state.currentView, this.props.currentView, this.props.views, this.state.resourcePromisesMap]), new _inferno2.InfernoEffect(this.loadDataSource, [this.props.dataSource]), (0, _inferno2.createReRenderEffect)()]
    };
    _proto.updateEffects = function() {
        var _this$_effects$, _this$_effects$2;
        null === (_this$_effects$ = this._effects[1]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.groups, this.props.resources, this.state.currentView, this.props.currentView, this.props.views, this.state.resourcePromisesMap]);
        null === (_this$_effects$2 = this._effects[2]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props.dataSource])
    };
    _proto.dispose = function() {
        var _this2 = this;
        return function() {
            _this2.state.instance.dispose()
        }
    };
    _proto.loadGroupResources = function() {
        var _this3 = this;
        var _this$props = this.props,
            schedulerGroups = _this$props.groups,
            resources = _this$props.resources;
        var currentViewProps = this.currentViewProps.groups;
        var validGroups = null !== currentViewProps && void 0 !== currentViewProps ? currentViewProps : schedulerGroups;
        (0, _utils2.loadResources)(validGroups, resources, this.state.resourcePromisesMap).then((function(loadedResources) {
            _this3.setState((function(__state_argument) {
                return {
                    loadedResources: loadedResources
                }
            }))
        }))
    };
    _proto.loadDataSource = function() {
        var _this4 = this;
        if (!this.internalDataSource.isLoaded() && !this.internalDataSource.isLoading()) {
            this.internalDataSource.load().done((function(loadOptions) {
                _this4.setState((function(__state_argument) {
                    return {
                        dataItems: (0, _data.resolveDataItems)(loadOptions)
                    }
                }))
            }))
        }
    };
    _proto.onViewRendered = function(viewMetaData) {
        this.setState((function(__state_argument) {
            return {
                workSpaceViewModel: viewMetaData
            }
        }))
    };
    _proto.setCurrentView = function(view) {
        var __newValue;
        this.setState((function(__state_argument) {
            __newValue = view;
            return {
                currentView: __newValue
            }
        }));
        this.props.currentViewChange(__newValue)
    };
    _proto.setCurrentDate = function(date) {
        var __newValue;
        this.setState((function(__state_argument) {
            __newValue = date;
            return {
                currentDate: __newValue
            }
        }));
        this.props.currentDateChange(__newValue)
    };
    _proto.getComponentInstance = function() {
        return this.state.instance
    };
    _proto.addAppointment = function(appointment) {
        this.state.instance.addAppointment(appointment)
    };
    _proto.deleteAppointment = function(appointment) {
        this.state.instance.deleteAppointment(appointment)
    };
    _proto.updateAppointment = function(target, appointment) {
        this.state.instance.updateAppointment(target, appointment)
    };
    _proto.getDataSource = function() {
        return this.state.instance.getDataSource()
    };
    _proto.getEndViewDate = function() {
        return this.state.instance.getEndViewDate()
    };
    _proto.getStartViewDate = function() {
        return this.state.instance.getStartViewDate()
    };
    _proto.hideAppointmentPopup = function(saveChanges) {
        this.state.instance.hideAppointmentPopup(saveChanges)
    };
    _proto.hideAppointmentTooltip = function() {
        this.state.instance.hideAppointmentTooltip()
    };
    _proto.scrollTo = function(date, group, allDay) {
        this.state.instance.scrollTo(date, group, allDay)
    };
    _proto.scrollToTime = function(hours, minutes, date) {
        this.state.instance.scrollToTime(hours, minutes, date)
    };
    _proto.showAppointmentPopup = function(appointmentData, createNewAppointment, currentAppointmentData) {
        this.state.instance.showAppointmentPopup(appointmentData, createNewAppointment, currentAppointmentData)
    };
    _proto.showAppointmentTooltip = function(appointmentData, target, currentAppointmentData) {
        this.state.instance.showAppointmentTooltip(appointmentData, target, currentAppointmentData)
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        _InfernoWrapperCompon.prototype.componentWillUpdate.call(this);
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
    };
    _proto.render = function() {
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
    };
    _createClass(Scheduler, [{
        key: "currentViewProps",
        get: function() {
            var views = this.props.views;
            return (0, _views.getCurrentViewProps)(void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView, views)
        }
    }, {
        key: "currentViewConfig",
        get: function() {
            var _this5 = this;
            if (void 0 !== this.__getterCache.currentViewConfig) {
                return this.__getterCache.currentViewConfig
            }
            return this.__getterCache.currentViewConfig = (0, _views.getCurrentViewConfig)(_this5.currentViewProps, _extends({}, _this5.props, {
                currentDate: void 0 !== _this5.props.currentDate ? _this5.props.currentDate : _this5.state.currentDate,
                currentView: void 0 !== _this5.props.currentView ? _this5.props.currentView : _this5.state.currentView
            }))
        }
    }, {
        key: "isValidViewDataProvider",
        get: function() {
            var _this$state$workSpace;
            var _this$currentViewConf = this.currentViewConfig,
                allDayPanelExpanded = _this$currentViewConf.allDayPanelExpanded,
                cellDuration = _this$currentViewConf.cellDuration,
                crossScrollingEnabled = _this$currentViewConf.crossScrollingEnabled,
                currentDate = _this$currentViewConf.currentDate,
                endDayHour = _this$currentViewConf.endDayHour,
                firstDayOfWeek = _this$currentViewConf.firstDayOfWeek,
                groupByDate = _this$currentViewConf.groupByDate,
                groupOrientation = _this$currentViewConf.groupOrientation,
                hoursInterval = _this$currentViewConf.hoursInterval,
                intervalCount = _this$currentViewConf.intervalCount,
                scrolling = _this$currentViewConf.scrolling,
                showAllDayPanel = _this$currentViewConf.showAllDayPanel,
                startDate = _this$currentViewConf.startDate,
                startDayHour = _this$currentViewConf.startDayHour,
                type = _this$currentViewConf.type;
            return (0, _common.isViewDataProviderConfigValid)(null === (_this$state$workSpace = this.state.workSpaceViewModel) || void 0 === _this$state$workSpace ? void 0 : _this$state$workSpace.viewDataProviderValidationOptions, {
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
    }, {
        key: "dataAccessors",
        get: function() {
            var _this6 = this;
            if (void 0 !== this.__getterCache.dataAccessors) {
                return this.__getterCache.dataAccessors
            }
            return this.__getterCache.dataAccessors = (0, _common.createDataAccessors)(_extends({}, _this6.props, {
                currentDate: void 0 !== _this6.props.currentDate ? _this6.props.currentDate : _this6.state.currentDate,
                currentView: void 0 !== _this6.props.currentView ? _this6.props.currentView : _this6.state.currentView
            }))
        }
    }, {
        key: "startViewDate",
        get: function() {
            var _this7 = this;
            if (void 0 !== this.__getterCache.startViewDate) {
                return this.__getterCache.startViewDate
            }
            return this.__getterCache.startViewDate = (_this7$currentViewCon = _this7.currentViewConfig, currentDate = _this7$currentViewCon.currentDate, firstDayOfWeek = _this7$currentViewCon.firstDayOfWeek, intervalCount = _this7$currentViewCon.intervalCount, startDate = _this7$currentViewCon.startDate, startDayHour = _this7$currentViewCon.startDayHour, type = _this7$currentViewCon.type, options = {
                currentDate: currentDate,
                startDayHour: startDayHour,
                startDate: startDate,
                intervalCount: intervalCount,
                firstDayOfWeek: firstDayOfWeek
            }, viewDataGenerator = (0, _utils.getViewDataGeneratorByViewType)(type), startViewDate = viewDataGenerator.getStartViewDate(options), startViewDate);
            var _this7$currentViewCon, currentDate, firstDayOfWeek, intervalCount, startDate, startDayHour, type, options, viewDataGenerator, startViewDate
        }
    }, {
        key: "isVirtualScrolling",
        get: function() {
            var _this$currentViewProp;
            return "virtual" === this.props.scrolling.mode || "virtual" === (null === (_this$currentViewProp = this.currentViewProps.scrolling) || void 0 === _this$currentViewProp ? void 0 : _this$currentViewProp.mode)
        }
    }, {
        key: "timeZoneCalculator",
        get: function() {
            var _this8 = this;
            if (void 0 !== this.__getterCache.timeZoneCalculator) {
                return this.__getterCache.timeZoneCalculator
            }
            return this.__getterCache.timeZoneCalculator = (0, _createTimeZoneCalculator.createTimeZoneCalculator)(_this8.props.timeZone)
        }
    }, {
        key: "internalDataSource",
        get: function() {
            if (this.props.dataSource instanceof _data_source.default) {
                return this.props.dataSource
            }
            if (this.props.dataSource instanceof Array) {
                return new _data_source.default({
                    store: {
                        type: "array",
                        data: this.props.dataSource
                    },
                    paginate: false
                })
            }
            return new _data_source.default(this.props.dataSource)
        }
    }, {
        key: "appointmentsConfig",
        get: function() {
            var _this9 = this;
            if (void 0 !== this.__getterCache.appointmentsConfig) {
                return this.__getterCache.appointmentsConfig
            }
            return this.__getterCache.appointmentsConfig = function() {
                if (!_this9.isValidViewDataProvider || !_this9.state.loadedResources) {
                    return
                }
                var renderConfig = (0, _work_space_config.getViewRenderConfigByType)(_this9.currentViewConfig.type, _this9.currentViewConfig.crossScrollingEnabled, _this9.currentViewConfig.intervalCount, _this9.state.loadedResources, _this9.currentViewConfig.groupOrientation);
                return (0, _appointments2.getAppointmentsConfig)(_extends({}, _this9.props, {
                    currentDate: void 0 !== _this9.props.currentDate ? _this9.props.currentDate : _this9.state.currentDate,
                    currentView: void 0 !== _this9.props.currentView ? _this9.props.currentView : _this9.state.currentView
                }), _this9.currentViewConfig, _this9.state.loadedResources, _this9.state.workSpaceViewModel.viewDataProvider, renderConfig.isAllDayPanelSupported)
            }()
        }
    }, {
        key: "preparedDataItems",
        get: function() {
            var _this10 = this;
            if (void 0 !== this.__getterCache.preparedDataItems) {
                return this.__getterCache.preparedDataItems
            }
            return this.__getterCache.preparedDataItems = (0, _data.getPreparedDataItems)(_this10.state.dataItems, _this10.dataAccessors, _this10.currentViewConfig.cellDuration, _this10.timeZoneCalculator)
        }
    }, {
        key: "filteredItems",
        get: function() {
            var _this11 = this;
            if (void 0 !== this.__getterCache.filteredItems) {
                return this.__getterCache.filteredItems
            }
            return this.__getterCache.filteredItems = function() {
                if (!_this11.appointmentsConfig) {
                    return []
                }
                var filterStrategy = (0, _filter.getFilterStrategy)(_this11.appointmentsConfig.resources, _this11.appointmentsConfig.startDayHour, _this11.appointmentsConfig.endDayHour, _this11.appointmentsConfig.cellDurationInMinutes, _this11.appointmentsConfig.showAllDayPanel, _this11.appointmentsConfig.supportAllDayRow, _this11.appointmentsConfig.firstDayOfWeek, _this11.appointmentsConfig.viewType, _this11.appointmentsConfig.dateRange, _this11.appointmentsConfig.groupCount, _this11.appointmentsConfig.loadedResources, _this11.appointmentsConfig.isVirtualScrolling, _this11.timeZoneCalculator, _this11.dataAccessors, _this11.state.workSpaceViewModel.viewDataProvider);
                return filterStrategy.filter(_this11.preparedDataItems)
            }()
        }
    }, {
        key: "appointmentsViewModel",
        get: function() {
            var _this12 = this;
            if (void 0 !== this.__getterCache.appointmentsViewModel) {
                return this.__getterCache.appointmentsViewModel
            }
            return this.__getterCache.appointmentsViewModel = function() {
                if (!_this12.appointmentsConfig || 0 === _this12.filteredItems.length) {
                    return {
                        allDay: [],
                        allDayCompact: [],
                        regular: [],
                        regularCompact: []
                    }
                }
                var model = (0, _appointments2.getAppointmentsModel)(_this12.appointmentsConfig, _this12.state.workSpaceViewModel.viewDataProvider, _this12.timeZoneCalculator, _this12.dataAccessors, _this12.state.workSpaceViewModel.cellsMetaData);
                return (0, _appointments.getAppointmentsViewModel)(model, _this12.filteredItems)
            }()
        }
    }, {
        key: "workSpaceKey",
        get: function() {
            var _this$state$loadedRes;
            var crossScrollingEnabled = this.props.crossScrollingEnabled;
            var _this$currentViewConf2 = this.currentViewConfig,
                groupOrientation = _this$currentViewConf2.groupOrientation,
                intervalCount = _this$currentViewConf2.intervalCount;
            if (!crossScrollingEnabled) {
                return ""
            }
            var groupCount = (0, _utils2.getGroupCount)(null !== (_this$state$loadedRes = this.state.loadedResources) && void 0 !== _this$state$loadedRes ? _this$state$loadedRes : []);
            return "".concat(void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView, "_").concat(groupOrientation, "_").concat(intervalCount, "_").concat(groupCount)
        }
    }, {
        key: "dataCellTemplate",
        get: function() {
            return this.currentViewConfig.dataCellTemplate
        }
    }, {
        key: "dateCellTemplate",
        get: function() {
            return this.currentViewConfig.dateCellTemplate
        }
    }, {
        key: "timeCellTemplate",
        get: function() {
            return this.currentViewConfig.timeCellTemplate
        }
    }, {
        key: "resourceCellTemplate",
        get: function() {
            return this.currentViewConfig.resourceCellTemplate
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props$currentDa = _extends({}, this.props, {
                    currentDate: void 0 !== this.props.currentDate ? this.props.currentDate : this.state.currentDate,
                    currentView: void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView
                }),
                restProps = (_this$props$currentDa.accessKey, _this$props$currentDa.activeStateEnabled, _this$props$currentDa.adaptivityEnabled, _this$props$currentDa.allDayExpr, _this$props$currentDa.appointmentCollectorTemplate, _this$props$currentDa.appointmentDragging, _this$props$currentDa.appointmentTemplate, _this$props$currentDa.appointmentTooltipTemplate, _this$props$currentDa.cellDuration, _this$props$currentDa.className, _this$props$currentDa.crossScrollingEnabled, _this$props$currentDa.currentDate, _this$props$currentDa.currentDateChange, _this$props$currentDa.currentView, _this$props$currentDa.currentViewChange, _this$props$currentDa.customizeDateNavigatorText, _this$props$currentDa.dataCellTemplate, _this$props$currentDa.dataSource, _this$props$currentDa.dateCellTemplate, _this$props$currentDa.dateSerializationFormat, _this$props$currentDa.defaultCurrentDate, _this$props$currentDa.defaultCurrentView, _this$props$currentDa.descriptionExpr, _this$props$currentDa.disabled, _this$props$currentDa.editing, _this$props$currentDa.endDateExpr, _this$props$currentDa.endDateTimeZoneExpr, _this$props$currentDa.endDayHour, _this$props$currentDa.firstDayOfWeek, _this$props$currentDa.focusStateEnabled, _this$props$currentDa.groupByDate, _this$props$currentDa.groups, _this$props$currentDa.height, _this$props$currentDa.hint, _this$props$currentDa.hoverStateEnabled, _this$props$currentDa.indicatorUpdateInterval, _this$props$currentDa.max, _this$props$currentDa.maxAppointmentsPerCell, _this$props$currentDa.min, _this$props$currentDa.noDataText, _this$props$currentDa.onAppointmentAdded, _this$props$currentDa.onAppointmentAdding, _this$props$currentDa.onAppointmentClick, _this$props$currentDa.onAppointmentContextMenu, _this$props$currentDa.onAppointmentDblClick, _this$props$currentDa.onAppointmentDeleted, _this$props$currentDa.onAppointmentDeleting, _this$props$currentDa.onAppointmentFormOpening, _this$props$currentDa.onAppointmentRendered, _this$props$currentDa.onAppointmentUpdated, _this$props$currentDa.onAppointmentUpdating, _this$props$currentDa.onCellClick, _this$props$currentDa.onCellContextMenu, _this$props$currentDa.onClick, _this$props$currentDa.onKeyDown, _this$props$currentDa.recurrenceEditMode, _this$props$currentDa.recurrenceExceptionExpr, _this$props$currentDa.recurrenceRuleExpr, _this$props$currentDa.remoteFiltering, _this$props$currentDa.resourceCellTemplate, _this$props$currentDa.resources, _this$props$currentDa.rtlEnabled, _this$props$currentDa.scrolling, _this$props$currentDa.selectedCellData, _this$props$currentDa.shadeUntilCurrentTime, _this$props$currentDa.showAllDayPanel, _this$props$currentDa.showCurrentTimeIndicator, _this$props$currentDa.startDateExpr, _this$props$currentDa.startDateTimeZoneExpr, _this$props$currentDa.startDayHour, _this$props$currentDa.tabIndex, _this$props$currentDa.textExpr, _this$props$currentDa.timeCellTemplate, _this$props$currentDa.timeZone, _this$props$currentDa.toolbar, _this$props$currentDa.useDropDownViewSwitcher, _this$props$currentDa.views, _this$props$currentDa.visible, _this$props$currentDa.width, _objectWithoutProperties(_this$props$currentDa, _excluded));
            return restProps
        }
    }]);
    return Scheduler
}(_inferno2.InfernoWrapperComponent);
exports.Scheduler = Scheduler;
Scheduler.defaultProps = _props.SchedulerProps;
