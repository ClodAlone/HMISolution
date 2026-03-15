/**
 * DevExtreme (cjs/ui/scheduler/appointments/dataProvider/appointmentDataProvider.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.AppointmentDataProvider = void 0;
var _appointmentDataSource = require("./appointmentDataSource");
var _appointmentFilter = require("./appointmentFilter");

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
var FilterStrategies = {
    virtual: "virtual",
    standard: "standard"
};
var AppointmentDataProvider = function() {
    function AppointmentDataProvider(options) {
        this.options = options;
        this.dataSource = this.options.dataSource;
        this.dataAccessors = this.options.dataAccessors;
        this.timeZoneCalculator = this.options.timeZoneCalculator;
        this.appointmentDataSource = new _appointmentDataSource.AppointmentDataSource(this.dataSource);
        this.initFilterStrategy()
    }
    var _proto = AppointmentDataProvider.prototype;
    _proto.getFilterStrategy = function() {
        if (!this.filterStrategy || this.filterStrategy.strategyName !== this.filterStrategyName) {
            this.initFilterStrategy()
        }
        return this.filterStrategy
    };
    _proto.initFilterStrategy = function() {
        var filterOptions = {
            resources: this.options.resources,
            dataSource: this.dataSource,
            dataAccessors: this.dataAccessors,
            startDayHour: this.options.startDayHour,
            endDayHour: this.options.endDayHour,
            showAllDayPanel: this.options.showAllDayPanel,
            timeZoneCalculator: this.options.timeZoneCalculator,
            loadedResources: this.options.getLoadedResources,
            supportAllDayRow: this.options.getSupportAllDayRow,
            viewType: this.options.getViewType,
            viewDirection: this.options.getViewDirection,
            dateRange: this.options.getDateRange,
            groupCount: this.options.getGroupCount,
            viewDataProvider: this.options.getViewDataProvider
        };
        this.filterStrategy = this.filterStrategyName === FilterStrategies.virtual ? new _appointmentFilter.AppointmentFilterVirtualStrategy(filterOptions) : new _appointmentFilter.AppointmentFilterBaseStrategy(filterOptions)
    };
    _proto.setDataSource = function(dataSource) {
        this.dataSource = dataSource;
        this.initFilterStrategy();
        this.appointmentDataSource.setDataSource(this.dataSource)
    };
    _proto.updateDataAccessors = function(dataAccessors) {
        this.dataAccessors = dataAccessors;
        this.initFilterStrategy()
    };
    _proto.filter = function(preparedItems) {
        return this.getFilterStrategy().filter(preparedItems)
    };
    _proto.filterByDate = function(min, max, remoteFiltering, dateSerializationFormat) {
        this.getFilterStrategy().filterByDate(min, max, remoteFiltering, dateSerializationFormat)
    };
    _proto.hasAllDayAppointments = function(filteredItems, preparedItems) {
        return this.getFilterStrategy().hasAllDayAppointments(filteredItems, preparedItems)
    };
    _proto.filterLoadedAppointments = function(filterOption, preparedItems) {
        return this.getFilterStrategy().filterLoadedAppointments(filterOption, preparedItems)
    };
    _proto.calculateAppointmentEndDate = function(isAllDay, startDate) {
        return this.getFilterStrategy().calculateAppointmentEndDate(isAllDay, startDate)
    };
    _proto.cleanState = function() {
        this.appointmentDataSource.cleanState()
    };
    _proto.getUpdatedAppointment = function() {
        return this.appointmentDataSource._updatedAppointment
    };
    _proto.getUpdatedAppointmentKeys = function() {
        return this.appointmentDataSource._updatedAppointmentKeys
    };
    _proto.add = function(rawAppointment) {
        return this.appointmentDataSource.add(rawAppointment)
    };
    _proto.update = function(target, rawAppointment) {
        return this.appointmentDataSource.update(target, rawAppointment)
    };
    _proto.remove = function(rawAppointment) {
        return this.appointmentDataSource.remove(rawAppointment)
    };
    _createClass(AppointmentDataProvider, [{
        key: "filterMaker",
        get: function() {
            return this.getFilterStrategy().filterMaker
        }
    }, {
        key: "keyName",
        get: function() {
            return this.appointmentDataSource.keyName
        }
    }, {
        key: "filterStrategyName",
        get: function() {
            return this.options.getIsVirtualScrolling() ? FilterStrategies.virtual : FilterStrategies.standard
        }
    }]);
    return AppointmentDataProvider
}();
exports.AppointmentDataProvider = AppointmentDataProvider;
