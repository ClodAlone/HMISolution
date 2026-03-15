/**
 * DevExtreme (cjs/ui/scheduler/appointments/viewModelGenerator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.AppointmentViewModelGenerator = void 0;
var _strategy_vertical = _interopRequireDefault(require("./rendering_strategies/strategy_vertical"));
var _strategy_week = _interopRequireDefault(require("./rendering_strategies/strategy_week"));
var _strategy_horizontal = _interopRequireDefault(require("./rendering_strategies/strategy_horizontal"));
var _strategy_horizontal_month_line = _interopRequireDefault(require("./rendering_strategies/strategy_horizontal_month_line"));
var _strategy_horizontal_month = _interopRequireDefault(require("./rendering_strategies/strategy_horizontal_month"));
var _strategy_agenda = _interopRequireDefault(require("./rendering_strategies/strategy_agenda"));
var _utils = require("../../../renovation/ui/scheduler/appointment/utils");
var _utils2 = require("../../../renovation/ui/scheduler/appointment/overflow_indicator/utils");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
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

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
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
var RENDERING_STRATEGIES = {
    horizontal: _strategy_horizontal.default,
    horizontalMonth: _strategy_horizontal_month.default,
    horizontalMonthLine: _strategy_horizontal_month_line.default,
    vertical: _strategy_vertical.default,
    week: _strategy_week.default,
    agenda: _strategy_agenda.default
};
var AppointmentViewModelGenerator = function() {
    function AppointmentViewModelGenerator() {}
    var _proto = AppointmentViewModelGenerator.prototype;
    _proto.initRenderingStrategy = function(options) {
        var RenderingStrategy = RENDERING_STRATEGIES[options.appointmentRenderingStrategyName];
        this.renderingStrategy = new RenderingStrategy(options)
    };
    _proto.generate = function(filteredItems, options) {
        var isRenovatedAppointments = options.isRenovatedAppointments;
        var appointments = filteredItems ? filteredItems.slice() : [];
        this.initRenderingStrategy(options);
        var renderingStrategy = this.getRenderingStrategy();
        var positionMap = renderingStrategy.createTaskPositionMap(appointments);
        var viewModel = this.postProcess(appointments, positionMap, isRenovatedAppointments);
        if (isRenovatedAppointments) {
            return this.makeRenovatedViewModels(viewModel)
        }
        return {
            positionMap: positionMap,
            viewModel: viewModel
        }
    };
    _proto.postProcess = function(filteredItems, positionMap, isRenovatedAppointments) {
        var renderingStrategy = this.getRenderingStrategy();
        return filteredItems.map((function(data, index) {
            if (!renderingStrategy.keepAppointmentSettings()) {
                delete data.settings
            }
            var appointmentSettings = positionMap[index];
            appointmentSettings.forEach((function(item) {
                item.direction = "vertical" === renderingStrategy.getDirection() && !item.allDay ? "vertical" : "horizontal"
            }));
            var item = {
                itemData: data,
                settings: appointmentSettings
            };
            if (!isRenovatedAppointments) {
                item.needRepaint = true;
                item.needRemove = false
            }
            return item
        }))
    };
    _proto.makeRenovatedViewModels = function(viewModel) {
        var _this = this;
        var strategy = this.getRenderingStrategy();
        var regularViewModels = [];
        var allDayViewModels = [];
        var compactOptions = [];
        viewModel.forEach((function(_ref) {
            var itemData = _ref.itemData,
                settings = _ref.settings;
            settings.forEach((function(options) {
                var item = _this.prepareViewModel(options, strategy, itemData);
                if (options.isCompact) {
                    compactOptions.push({
                        compactViewModel: options.virtual,
                        appointmentViewModel: item
                    })
                } else if (options.allDay) {
                    allDayViewModels.push(item)
                } else {
                    regularViewModels.push(item)
                }
            }))
        }));
        var compactViewModels = this.prepareCompactViewModels(compactOptions);
        var result = _extends({
            allDay: allDayViewModels,
            regular: regularViewModels
        }, compactViewModels);
        return result
    };
    _proto.prepareViewModel = function(options, strategy, itemData) {
        var geometry = strategy.getAppointmentGeometry(options);
        var viewModel = {
            key: (0, _utils.getAppointmentKey)(geometry),
            appointment: itemData,
            geometry: _extends({}, geometry, {
                leftVirtualWidth: options.leftVirtualWidth,
                topVirtualHeight: options.topVirtualHeight
            }),
            info: _extends({}, options.info, {
                allDay: options.allDay,
                direction: options.direction,
                appointmentReduced: options.appointmentReduced
            })
        };
        return viewModel
    };
    _proto.getCompactViewModelFrame = function(compactViewModel) {
        return {
            isAllDay: !!compactViewModel.isAllDay,
            isCompact: compactViewModel.isCompact,
            geometry: {
                left: compactViewModel.left,
                top: compactViewModel.top,
                width: compactViewModel.width,
                height: compactViewModel.height
            },
            items: {
                colors: [],
                data: [],
                settings: []
            }
        }
    };
    _proto.prepareCompactViewModels = function(compactOptions) {
        var _this2 = this;
        var regularCompact = {};
        var allDayCompact = {};
        compactOptions.forEach((function(_ref2) {
            var compactViewModel = _ref2.compactViewModel,
                appointmentViewModel = _ref2.appointmentViewModel;
            var index = compactViewModel.index,
                isAllDay = compactViewModel.isAllDay;
            var viewModel = isAllDay ? allDayCompact : regularCompact;
            if (!viewModel[index]) {
                viewModel[index] = _this2.getCompactViewModelFrame(compactViewModel)
            }
            var _viewModel$index$item = viewModel[index].items,
                settings = _viewModel$index$item.settings,
                data = _viewModel$index$item.data,
                colors = _viewModel$index$item.colors;
            settings.push(appointmentViewModel);
            data.push(appointmentViewModel.appointment);
            colors.push(appointmentViewModel.info.resourceColor)
        }));
        var toArray = function(items) {
            return Object.keys(items).map((function(key) {
                return _extends({
                    key: key
                }, items[key])
            }))
        };
        var allDayViewModels = toArray(allDayCompact);
        var regularViewModels = toArray(regularCompact);
        [].concat(_toConsumableArray(allDayViewModels), _toConsumableArray(regularViewModels)).forEach((function(viewModel) {
            var colors = viewModel.items.colors;
            viewModel.color = (0, _utils2.getOverflowIndicatorColor)(colors[0], colors)
        }));
        return {
            allDayCompact: allDayViewModels,
            regularCompact: regularViewModels
        }
    };
    _proto.getRenderingStrategy = function() {
        return this.renderingStrategy
    };
    return AppointmentViewModelGenerator
}();
exports.AppointmentViewModelGenerator = AppointmentViewModelGenerator;
