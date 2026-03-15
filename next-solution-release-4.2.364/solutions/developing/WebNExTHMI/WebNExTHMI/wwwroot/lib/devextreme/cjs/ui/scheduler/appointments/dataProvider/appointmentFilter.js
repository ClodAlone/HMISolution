/**
 * DevExtreme (cjs/ui/scheduler/appointments/dataProvider/appointmentFilter.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.AppointmentFilterVirtualStrategy = exports.AppointmentFilterBaseStrategy = void 0;
var _config = _interopRequireDefault(require("../../../../core/config"));
var _date = _interopRequireDefault(require("../../../../core/utils/date"));
var _common = require("../../../../core/utils/common");
var _date_serialization = _interopRequireDefault(require("../../../../core/utils/date_serialization"));
var _recurrence = require("../../recurrence");
var _array = require("../../../../core/utils/array");
var _extend = require("../../../../core/utils/extend");
var _iterator = require("../../../../core/utils/iterator");
var _type = require("../../../../core/utils/type");
var _query = _interopRequireDefault(require("../../../../data/query"));
var _appointmentAdapter = require("../../appointmentAdapter");
var _base = require("../../../../renovation/ui/scheduler/view_model/to_test/views/utils/base");
var _utils = require("../../resources/utils");
var _utils2 = require("./utils");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
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

function _slicedToArray(arr, i) {
    return _arrayWithHoles(arr) || _iterableToArrayLimit(arr, i) || _unsupportedIterableToArray(arr, i) || _nonIterableRest()
}

function _nonIterableRest() {
    throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}

function _iterableToArrayLimit(arr, i) {
    var _i = null == arr ? null : "undefined" !== typeof Symbol && arr[Symbol.iterator] || arr["@@iterator"];
    if (null == _i) {
        return
    }
    var _arr = [];
    var _n = true;
    var _d = false;
    var _s, _e;
    try {
        for (_i = _i.call(arr); !(_n = (_s = _i.next()).done); _n = true) {
            _arr.push(_s.value);
            if (i && _arr.length === i) {
                break
            }
        }
    } catch (err) {
        _d = true;
        _e = err
    } finally {
        try {
            if (!_n && null != _i.return) {
                _i.return()
            }
        } finally {
            if (_d) {
                throw _e
            }
        }
    }
    return _arr
}

function _arrayWithHoles(arr) {
    if (Array.isArray(arr)) {
        return arr
    }
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
var toMs = _date.default.dateToMilliseconds;
var DATE_FILTER_POSITION = 0;
var USER_FILTER_POSITION = 1;
var FilterStrategies = {
    virtual: "virtual",
    standard: "standard"
};
var FilterMaker = function() {
    function FilterMaker(dataAccessors) {
        this._filterRegistry = null;
        this.dataAccessors = dataAccessors
    }
    var _proto = FilterMaker.prototype;
    _proto.isRegistered = function() {
        return !!this._filterRegistry
    };
    _proto.clearRegistry = function() {
        delete this._filterRegistry
    };
    _proto.make = function(type, args) {
        if (!this._filterRegistry) {
            this._filterRegistry = {}
        }
        this._make(type).apply(this, args)
    };
    _proto._make = function(type) {
        var _this = this;
        switch (type) {
            case "date":
                return function(min, max, useAccessors) {
                    var startDate = useAccessors ? _this.dataAccessors.getter.startDate : _this.dataAccessors.expr.startDateExpr;
                    var endDate = useAccessors ? _this.dataAccessors.getter.endDate : _this.dataAccessors.expr.endDateExpr;
                    var recurrenceRule = _this.dataAccessors.expr.recurrenceRuleExpr;
                    _this._filterRegistry.date = [
                        [
                            [endDate, ">=", min],
                            [startDate, "<", max]
                        ], "or", [recurrenceRule, "startswith", "freq"], "or", [
                            [endDate, min],
                            [startDate, min]
                        ]
                    ];
                    if (!recurrenceRule) {
                        _this._filterRegistry.date.splice(1, 2)
                    }
                };
            case "user":
                return function(userFilter) {
                    _this._filterRegistry.user = userFilter
                }
        }
    };
    _proto.combine = function() {
        var filter = [];
        this._filterRegistry.date && filter.push(this._filterRegistry.date);
        this._filterRegistry.user && filter.push(this._filterRegistry.user);
        return filter
    };
    _proto.dateFilter = function() {
        var _this$_filterRegistry;
        return null === (_this$_filterRegistry = this._filterRegistry) || void 0 === _this$_filterRegistry ? void 0 : _this$_filterRegistry.date
    };
    return FilterMaker
}();
var AppointmentFilterBaseStrategy = function() {
    function AppointmentFilterBaseStrategy(options) {
        this.options = options;
        this.dataSource = this.options.dataSource;
        this.dataAccessors = this.options.dataAccessors;
        this._init()
    }
    var _proto2 = AppointmentFilterBaseStrategy.prototype;
    _proto2._resolveOption = function(name) {
        var result = this.options[name];
        return "function" === typeof result ? result() : result
    };
    _proto2._init = function() {
        this.setDataAccessors(this.dataAccessors);
        this.setDataSource(this.dataSource)
    };
    _proto2.filter = function(preparedItems) {
        var dateRange = this.dateRange;
        var allDay;
        if (!this.showAllDayPanel && this.supportAllDayRow) {
            allDay = false
        }
        return this.filterLoadedAppointments({
            startDayHour: this.viewStartDayHour,
            endDayHour: this.viewEndDayHour,
            viewStartDayHour: this.viewStartDayHour,
            viewEndDayHour: this.viewEndDayHour,
            min: dateRange[0],
            max: dateRange[1],
            resources: this.loadedResources,
            allDay: allDay,
            supportMultiDayAppointments: (0, _base.isSupportMultiDayAppointments)(this.viewType),
            firstDayOfWeek: this.firstDayOfWeek
        }, preparedItems)
    };
    _proto2.filterByDate = function(min, max, remoteFiltering, dateSerializationFormat) {
        if (!this.dataSource) {
            return
        }
        var _getTrimDates = (0, _utils2.getTrimDates)(min, max),
            _getTrimDates2 = _slicedToArray(_getTrimDates, 2),
            trimMin = _getTrimDates2[0],
            trimMax = _getTrimDates2[1];
        if (!this.filterMaker.isRegistered()) {
            this._createFilter(trimMin, trimMax, remoteFiltering, dateSerializationFormat)
        } else {
            var _this$dataSource$filt;
            if ((null === (_this$dataSource$filt = this.dataSource.filter()) || void 0 === _this$dataSource$filt ? void 0 : _this$dataSource$filt.length) > 1) {
                var userFilter = this._serializeRemoteFilter([this.dataSource.filter()[1]], dateSerializationFormat);
                this.filterMaker.make("user", userFilter)
            }
            if (remoteFiltering) {
                this.filterMaker.make("date", [trimMin, trimMax]);
                this.dataSource.filter(this._combineRemoteFilter(dateSerializationFormat))
            }
        }
    };
    _proto2.hasAllDayAppointments = function(filteredItems, preparedItems) {
        var _this2 = this;
        var adapters = filteredItems.map((function(item) {
            return (0, _appointmentAdapter.createAppointmentAdapter)(item, _this2.dataAccessors, _this2.timeZoneCalculator)
        }));
        var result = false;
        (0, _iterator.each)(adapters, (function(_, item) {
            if ((0, _utils2.getAppointmentTakesAllDay)(item, _this2.viewStartDayHour, _this2.viewEndDayHour)) {
                result = true;
                return false
            }
        }));
        return result
    };
    _proto2.setDataAccessors = function(dataAccessors) {
        this.dataAccessors = dataAccessors;
        this.filterMaker = new FilterMaker(this.dataAccessors)
    };
    _proto2.setDataSource = function(dataSource) {
        var _this$filterMaker;
        this.dataSource = dataSource;
        null === (_this$filterMaker = this.filterMaker) || void 0 === _this$filterMaker ? void 0 : _this$filterMaker.clearRegistry()
    };
    _proto2._createAllDayAppointmentFilter = function(filterOptions) {
        var viewStartDayHour = filterOptions.viewStartDayHour,
            viewEndDayHour = filterOptions.viewEndDayHour;
        return [
            [function(appointment) {
                return (0, _utils2.getAppointmentTakesAllDay)(appointment, viewStartDayHour, viewEndDayHour)
            }]
        ]
    };
    _proto2._createCombinedFilter = function(filterOptions) {
        var _this3 = this;
        var min = new Date(filterOptions.min);
        var max = new Date(filterOptions.max);
        var startDayHour = filterOptions.startDayHour,
            endDayHour = filterOptions.endDayHour,
            viewStartDayHour = filterOptions.viewStartDayHour,
            viewEndDayHour = filterOptions.viewEndDayHour,
            resources = filterOptions.resources,
            firstDayOfWeek = filterOptions.firstDayOfWeek,
            checkIntersectViewport = filterOptions.checkIntersectViewport,
            supportMultiDayAppointments = filterOptions.supportMultiDayAppointments;
        var _getTrimDates3 = (0, _utils2.getTrimDates)(min, max),
            _getTrimDates4 = _slicedToArray(_getTrimDates3, 2),
            trimMin = _getTrimDates4[0],
            trimMax = _getTrimDates4[1];
        var useRecurrence = (0, _type.isDefined)(this.dataAccessors.getter.recurrenceRule);
        return [
            [function(appointment) {
                var _appointment$visible;
                var appointmentVisible = null !== (_appointment$visible = appointment.visible) && void 0 !== _appointment$visible ? _appointment$visible : true;
                if (!appointmentVisible) {
                    return false
                }
                var startDate = appointment.startDate,
                    endDate = appointment.endDate,
                    hasRecurrenceRule = appointment.hasRecurrenceRule;
                if (!hasRecurrenceRule) {
                    if (!(endDate >= trimMin && startDate < trimMax || _date.default.sameDate(endDate, trimMin) && _date.default.sameDate(startDate, trimMin))) {
                        return false
                    }
                }
                var recurrenceRule;
                if (useRecurrence) {
                    recurrenceRule = appointment.recurrenceRule
                }
                var appointmentTakesAllDay = (0, _utils2.getAppointmentTakesAllDay)(appointment, viewStartDayHour, viewEndDayHour);
                var appointmentTakesSeveralDays = (0, _utils2.getAppointmentTakesSeveralDays)(appointment);
                var isAllDay = appointment.allDay;
                var isLongAppointment = appointmentTakesSeveralDays || appointmentTakesAllDay;
                if (null !== resources && void 0 !== resources && resources.length && !_this3._filterAppointmentByResources(appointment.rawAppointment, resources)) {
                    return false
                }
                if (appointmentTakesAllDay && false === filterOptions.allDay) {
                    return false
                }
                if (hasRecurrenceRule) {
                    var recurrenceException = (0, _utils2.getRecurrenceException)(appointment, _this3.timeZoneCalculator, _this3.timezone);
                    if (!_this3._filterAppointmentByRRule({
                            startDate: startDate,
                            endDate: endDate,
                            recurrenceRule: recurrenceRule,
                            recurrenceException: recurrenceException,
                            allDay: appointmentTakesAllDay
                        }, min, max, startDayHour, endDayHour, firstDayOfWeek)) {
                        return false
                    }
                }
                if (!isAllDay && supportMultiDayAppointments && isLongAppointment) {
                    if (endDate < min && (!useRecurrence || useRecurrence && !hasRecurrenceRule)) {
                        return false
                    }
                }
                if ((0, _type.isDefined)(startDayHour) && (!useRecurrence || !filterOptions.isVirtualScrolling)) {
                    if (!(0, _utils2.compareDateWithStartDayHour)(startDate, endDate, startDayHour, appointmentTakesAllDay, appointmentTakesSeveralDays)) {
                        return false
                    }
                }
                if ((0, _type.isDefined)(endDayHour)) {
                    if (!(0, _utils2.compareDateWithEndDayHour)({
                            startDate: startDate,
                            endDate: endDate,
                            startDayHour: startDayHour,
                            endDayHour: endDayHour,
                            viewStartDayHour: viewStartDayHour,
                            viewEndDayHour: viewEndDayHour,
                            allDay: appointmentTakesAllDay,
                            severalDays: appointmentTakesSeveralDays,
                            min: min,
                            max: max,
                            checkIntersectViewport: checkIntersectViewport
                        })) {
                        return false
                    }
                }
                if (!isAllDay && (!isLongAppointment || supportMultiDayAppointments)) {
                    if (endDate < min && useRecurrence && !hasRecurrenceRule) {
                        return false
                    }
                }
                return true
            }]
        ]
    };
    _proto2._createAppointmentFilter = function(filterOptions) {
        if (this.filterMaker.isRegistered()) {
            this.filterMaker.make("user", void 0)
        }
        return this._createCombinedFilter(filterOptions)
    };
    _proto2._excessFiltering = function() {
        var dateFilter = this.filterMaker.dateFilter();
        var dataSourceFilter = this.dataSource.filter();
        return dateFilter && dataSourceFilter && ((0, _common.equalByValue)(dataSourceFilter, dateFilter) || dataSourceFilter.length && (0, _common.equalByValue)(dataSourceFilter[DATE_FILTER_POSITION], dateFilter))
    };
    _proto2._combineRemoteFilter = function(dateSerializationFormat) {
        var combinedFilter = this.filterMaker.combine();
        return this._serializeRemoteFilter(combinedFilter, dateSerializationFormat)
    };
    _proto2._serializeRemoteFilter = function(filter, dateSerializationFormat) {
        if (!Array.isArray(filter)) {
            return filter
        }
        filter = (0, _extend.extend)([], filter);
        var startDate = this.dataAccessors.expr.startDateExpr;
        var endDate = this.dataAccessors.expr.endDateExpr;
        if ((0, _type.isString)(filter[0])) {
            if ((0, _config.default)().forceIsoDateParsing && filter.length > 1) {
                if (filter[0] === startDate || filter[0] === endDate) {
                    filter[filter.length - 1] = _date_serialization.default.serializeDate(new Date(filter[filter.length - 1]), dateSerializationFormat)
                }
            }
        }
        for (var i = 0; i < filter.length; i++) {
            filter[i] = this._serializeRemoteFilter(filter[i], dateSerializationFormat)
        }
        return filter
    };
    _proto2._createFilter = function(min, max, remoteFiltering, dateSerializationFormat) {
        if (remoteFiltering) {
            this.filterMaker.make("date", [min, max]);
            var userFilterPosition = this._excessFiltering() ? this.dataSource.filter()[USER_FILTER_POSITION] : this.dataSource.filter();
            this.filterMaker.make("user", [userFilterPosition]);
            this.dataSource.filter(this._combineRemoteFilter(dateSerializationFormat))
        }
    };
    _proto2._filterAppointmentByResources = function(appointment, resources) {
        var _this4 = this;
        var checkAppointmentResourceValues = function(resourceName, resourceIndex) {
            var resourceGetter = _this4.dataAccessors.resources.getter[resourceName];
            var resource;
            if ((0, _type.isFunction)(resourceGetter)) {
                resource = resourceGetter(appointment)
            }
            var appointmentResourceValues = (0, _array.wrapToArray)(resource);
            var resourceData = (0, _iterator.map)(resources[resourceIndex].items, (function(item) {
                return item.id
            }));
            for (var j = 0; j < appointmentResourceValues.length; j++) {
                if ((0, _array.inArray)(appointmentResourceValues[j], resourceData) > -1) {
                    return true
                }
            }
            return false
        };
        var result = false;
        for (var i = 0; i < resources.length; i++) {
            var resourceName = resources[i].name;
            result = checkAppointmentResourceValues(resourceName, i);
            if (!result) {
                return false
            }
        }
        return result
    };
    _proto2._filterAppointmentByRRule = function(appointment, min, max, startDayHour, endDayHour, firstDayOfWeek) {
        var recurrenceRule = appointment.recurrenceRule;
        var recurrenceException = appointment.recurrenceException;
        var allDay = appointment.allDay;
        var result = true;
        var appointmentStartDate = appointment.startDate;
        var appointmentEndDate = appointment.endDate;
        var recurrenceProcessor = (0, _recurrence.getRecurrenceProcessor)();
        if (allDay || (0, _utils2._appointmentPartInInterval)(appointmentStartDate, appointmentEndDate, startDayHour, endDayHour)) {
            var _getTrimDates5 = (0, _utils2.getTrimDates)(min, max),
                _getTrimDates6 = _slicedToArray(_getTrimDates5, 2),
                trimMin = _getTrimDates6[0],
                trimMax = _getTrimDates6[1];
            min = trimMin;
            max = new Date(trimMax.getTime() - toMs("minute"))
        }
        if (recurrenceRule && !recurrenceProcessor.isValidRecurrenceRule(recurrenceRule)) {
            result = appointmentEndDate > min && appointmentStartDate <= max
        }
        if (result && recurrenceProcessor.isValidRecurrenceRule(recurrenceRule)) {
            result = recurrenceProcessor.hasRecurrence({
                rule: recurrenceRule,
                exception: recurrenceException,
                start: appointmentStartDate,
                end: appointmentEndDate,
                min: min,
                max: max,
                firstDayOfWeek: firstDayOfWeek
            })
        }
        return result
    };
    _proto2.filterLoadedAppointments = function(filterOptions, preparedItems) {
        var filteredItems = this.filterPreparedItems(filterOptions, preparedItems);
        return filteredItems.map((function(_ref) {
            var rawAppointment = _ref.rawAppointment;
            return rawAppointment
        }))
    };
    _proto2.filterPreparedItems = function(filterOptions, preparedItems) {
        var combinedFilter = this._createAppointmentFilter(filterOptions);
        return (0, _query.default)(preparedItems).filter(combinedFilter).toArray()
    };
    _proto2.filterAllDayAppointments = function(filterOptions, preparedItems) {
        var combinedFilter = this._createAllDayAppointmentFilter(filterOptions);
        return (0, _query.default)(preparedItems).filter(combinedFilter).toArray().map((function(_ref2) {
            var rawAppointment = _ref2.rawAppointment;
            return rawAppointment
        }))
    };
    _createClass(AppointmentFilterBaseStrategy, [{
        key: "strategyName",
        get: function() {
            return FilterStrategies.standard
        }
    }, {
        key: "timeZoneCalculator",
        get: function() {
            return this.options.timeZoneCalculator
        }
    }, {
        key: "viewStartDayHour",
        get: function() {
            return this.options.startDayHour
        }
    }, {
        key: "viewEndDayHour",
        get: function() {
            return this.options.endDayHour
        }
    }, {
        key: "timezone",
        get: function() {
            return this.options.timezone
        }
    }, {
        key: "firstDayOfWeek",
        get: function() {
            return this.options.firstDayOfWeek
        }
    }, {
        key: "showAllDayPanel",
        get: function() {
            return this.options.showAllDayPanel
        }
    }, {
        key: "loadedResources",
        get: function() {
            return this._resolveOption("loadedResources")
        }
    }, {
        key: "supportAllDayRow",
        get: function() {
            return this._resolveOption("supportAllDayRow")
        }
    }, {
        key: "viewType",
        get: function() {
            return this._resolveOption("viewType")
        }
    }, {
        key: "viewDirection",
        get: function() {
            return this._resolveOption("viewDirection")
        }
    }, {
        key: "dateRange",
        get: function() {
            return this._resolveOption("dateRange")
        }
    }, {
        key: "groupCount",
        get: function() {
            return this._resolveOption("groupCount")
        }
    }, {
        key: "viewDataProvider",
        get: function() {
            return this._resolveOption("viewDataProvider")
        }
    }]);
    return AppointmentFilterBaseStrategy
}();
exports.AppointmentFilterBaseStrategy = AppointmentFilterBaseStrategy;
var AppointmentFilterVirtualStrategy = function(_AppointmentFilterBas) {
    _inheritsLoose(AppointmentFilterVirtualStrategy, _AppointmentFilterBas);

    function AppointmentFilterVirtualStrategy() {
        return _AppointmentFilterBas.apply(this, arguments) || this
    }
    var _proto3 = AppointmentFilterVirtualStrategy.prototype;
    _proto3.filter = function(preparedItems) {
        var _this5 = this;
        var hourMs = toMs("hour");
        var isCalculateStartAndEndDayHour = (0, _base.isDateAndTimeView)(this.viewType);
        var checkIntersectViewport = isCalculateStartAndEndDayHour && "horizontal" === this.viewDirection;
        var isAllDayWorkspace = !this.supportAllDayRow;
        var showAllDayAppointments = this.showAllDayPanel || isAllDayWorkspace;
        var endViewDate = this.viewDataProvider.getLastViewDateByEndDayHour(this.viewEndDayHour);
        var filterOptions = [];
        var groupsInfo = this.viewDataProvider.getCompletedGroupsInfo();
        groupsInfo.forEach((function(item) {
            var groupIndex = item.groupIndex;
            var groupStartDate = item.startDate;
            var groupEndDate = new Date(Math.min(item.endDate, endViewDate));
            var startDayHour = isCalculateStartAndEndDayHour ? groupStartDate.getHours() : _this5.viewStartDayHour;
            var endDayHour = isCalculateStartAndEndDayHour ? startDayHour + groupStartDate.getMinutes() / 60 + (groupEndDate - groupStartDate) / hourMs : _this5.viewEndDayHour;
            var resources = _this5._getPrerenderFilterResources(groupIndex);
            var allDayPanel = _this5.viewDataProvider.getAllDayPanel(groupIndex);
            var supportAllDayAppointment = isAllDayWorkspace || !!showAllDayAppointments && (null === allDayPanel || void 0 === allDayPanel ? void 0 : allDayPanel.length) > 0;
            filterOptions.push({
                isVirtualScrolling: true,
                startDayHour: startDayHour,
                endDayHour: endDayHour,
                viewStartDayHour: _this5.viewStartDayHour,
                viewEndDayHour: _this5.viewEndDayHour,
                min: groupStartDate,
                max: groupEndDate,
                supportMultiDayAppointments: (0, _base.isSupportMultiDayAppointments)(_this5.viewType),
                allDay: supportAllDayAppointment,
                resources: resources,
                firstDayOfWeek: _this5.firstDayOfWeek,
                checkIntersectViewport: checkIntersectViewport
            })
        }));
        return this.filterLoadedAppointments({
            filterOptions: filterOptions,
            groupCount: this.groupCount
        }, preparedItems)
    };
    _proto3.filterPreparedItems = function(_ref3, preparedItems) {
        var _this6 = this;
        var filterOptions = _ref3.filterOptions,
            groupCount = _ref3.groupCount;
        var combinedFilters = [];
        var itemsToFilter = preparedItems;
        var needPreFilter = groupCount > 0;
        if (needPreFilter) {
            itemsToFilter = itemsToFilter.filter((function(_ref4) {
                var rawAppointment = _ref4.rawAppointment;
                for (var i = 0; i < filterOptions.length; ++i) {
                    var resources = filterOptions[i].resources;
                    if (_this6._filterAppointmentByResources(rawAppointment, resources)) {
                        return true
                    }
                }
            }))
        }
        filterOptions.forEach((function(option) {
            combinedFilters.length && combinedFilters.push("or");
            var filter = _this6._createAppointmentFilter(option);
            combinedFilters.push(filter)
        }));
        return (0, _query.default)(itemsToFilter).filter(combinedFilters).toArray()
    };
    _proto3.hasAllDayAppointments = function(adapters, preparedItems) {
        return this.filterAllDayAppointments({
            viewStartDayHour: this.viewStartDayHour,
            viewEndDayHour: this.viewEndDayHour
        }, preparedItems).length > 0
    };
    _proto3._getPrerenderFilterResources = function(groupIndex) {
        var cellGroup = this.viewDataProvider.getCellsGroup(groupIndex);
        return (0, _utils.getResourcesDataByGroups)(this.loadedResources, this.resources, [cellGroup])
    };
    _createClass(AppointmentFilterVirtualStrategy, [{
        key: "strategyName",
        get: function() {
            return FilterStrategies.virtual
        }
    }, {
        key: "resources",
        get: function() {
            return this.options.resources
        }
    }]);
    return AppointmentFilterVirtualStrategy
}(AppointmentFilterBaseStrategy);
exports.AppointmentFilterVirtualStrategy = AppointmentFilterVirtualStrategy;
