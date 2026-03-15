/**
 * DevExtreme (renovation/ui/scheduler/header/header.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.SchedulerToolbarProps = exports.SchedulerToolbarBaseProps = exports.SchedulerToolbar = void 0;
exports.defaultOptions = defaultOptions;
exports.viewFunction = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _devices = _interopRequireDefault(require("../../../../core/devices"));
var _toolbar = require("../../toolbar/toolbar");
require("../../../../ui/button_group");
require("../../../../ui/drop_down_button");
var _date = _interopRequireDefault(require("../../../../core/utils/date"));
var _utils = require("../../../../ui/scheduler/header/utils");
var _utils2 = require("./utils");
var _props = require("../props");
var _calendar = require("./calendar");
var _utils3 = require("../../../../core/options/utils");
var _excluded = ["agendaDuration", "currentDate", "currentView", "currentViewChange", "customizationFunction", "defaultCurrentView", "firstDayOfWeek", "intervalCount", "items", "max", "min", "onCurrentDateUpdate", "onCurrentViewUpdate", "startViewDate", "useDropDownViewSwitcher", "useShortDateFormat", "viewType", "views"];

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
var trimTime = _date.default.trimTime;
var viewFunction = function(viewModel) {
    var _viewModel$props = viewModel.props,
        currentDate = _viewModel$props.currentDate,
        firstDayOfWeek = _viewModel$props.firstDayOfWeek,
        max = _viewModel$props.max,
        min = _viewModel$props.min;
    var calendarVisible = viewModel.calendarVisible,
        changeCalendarDate = viewModel.changeCalendarDate,
        changeCalendarVisible = viewModel.changeCalendarVisible,
        items = viewModel.items;
    return (0, _inferno.createVNode)(1, "div", "dx-scheduler-header", [(0, _inferno.createComponentVNode)(2, _calendar.SchedulerCalendar, {
        currentDate: currentDate,
        onCurrentDateUpdate: changeCalendarDate,
        min: min,
        max: max,
        firstDayOfWeek: firstDayOfWeek,
        visible: calendarVisible,
        onVisibleUpdate: changeCalendarVisible
    }), (0, _inferno.createComponentVNode)(2, _toolbar.Toolbar, {
        items: items
    })], 4)
};
exports.viewFunction = viewFunction;
var SchedulerToolbarBaseProps = Object.defineProperties({
    intervalCount: 1,
    firstDayOfWeek: 0,
    agendaDuration: 7,
    viewType: "day"
}, {
    useShortDateFormat: {
        get: function() {
            return !_devices.default.real().generic || _devices.default.isSimulator()
        },
        configurable: true,
        enumerable: true
    }
});
exports.SchedulerToolbarBaseProps = SchedulerToolbarBaseProps;
var SchedulerToolbarProps = Object.defineProperties({}, {
    intervalCount: {
        get: function() {
            return SchedulerToolbarBaseProps.intervalCount
        },
        configurable: true,
        enumerable: true
    },
    firstDayOfWeek: {
        get: function() {
            return SchedulerToolbarBaseProps.firstDayOfWeek
        },
        configurable: true,
        enumerable: true
    },
    agendaDuration: {
        get: function() {
            return SchedulerToolbarBaseProps.agendaDuration
        },
        configurable: true,
        enumerable: true
    },
    useShortDateFormat: {
        get: function() {
            return SchedulerToolbarBaseProps.useShortDateFormat
        },
        configurable: true,
        enumerable: true
    },
    viewType: {
        get: function() {
            return SchedulerToolbarBaseProps.viewType
        },
        configurable: true,
        enumerable: true
    },
    useDropDownViewSwitcher: {
        get: function() {
            return _props.SchedulerProps.useDropDownViewSwitcher
        },
        configurable: true,
        enumerable: true
    },
    defaultCurrentView: {
        get: function() {
            return _props.SchedulerProps.currentView
        },
        configurable: true,
        enumerable: true
    },
    currentViewChange: {
        get: function() {
            return function() {}
        },
        configurable: true,
        enumerable: true
    }
});
exports.SchedulerToolbarProps = SchedulerToolbarProps;
var SchedulerToolbar = function(_BaseInfernoComponent) {
    _inheritsLoose(SchedulerToolbar, _BaseInfernoComponent);

    function SchedulerToolbar(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.__getterCache = {};
        _this.state = {
            calendarVisible: false,
            currentView: void 0 !== _this.props.currentView ? _this.props.currentView : _this.props.defaultCurrentView
        };
        _this.setCurrentView = _this.setCurrentView.bind(_assertThisInitialized(_this));
        _this.setCurrentDate = _this.setCurrentDate.bind(_assertThisInitialized(_this));
        _this.getNextDate = _this.getNextDate.bind(_assertThisInitialized(_this));
        _this.updateDateByDirection = _this.updateDateByDirection.bind(_assertThisInitialized(_this));
        _this.isPreviousButtonDisabled = _this.isPreviousButtonDisabled.bind(_assertThisInitialized(_this));
        _this.isNextButtonDisabled = _this.isNextButtonDisabled.bind(_assertThisInitialized(_this));
        _this.changeCalendarDate = _this.changeCalendarDate.bind(_assertThisInitialized(_this));
        _this.changeCalendarVisible = _this.changeCalendarVisible.bind(_assertThisInitialized(_this));
        _this.showCalendar = _this.showCalendar.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = SchedulerToolbar.prototype;
    _proto.setCurrentView = function(view) {
        if (view.name !== (void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView)) {
            this.props.onCurrentViewUpdate(view.name)
        }
    };
    _proto.setCurrentDate = function(date) {
        if (date.getTime() !== this.props.currentDate.getTime()) {
            this.props.onCurrentDateUpdate(new Date(date))
        }
    };
    _proto.getNextDate = function(direction, initialDate) {
        var date = null !== initialDate && void 0 !== initialDate ? initialDate : this.props.currentDate;
        var options = _extends({}, this.intervalOptions, {
            date: date
        });
        return (0, _utils.getNextIntervalDate)(options, direction)
    };
    _proto.updateDateByDirection = function(direction) {
        var date = this.getNextDate(direction);
        this.setCurrentDate(date)
    };
    _proto.isPreviousButtonDisabled = function() {
        if (void 0 === this.props.min) {
            return false
        }
        var min = trimTime(new Date(this.props.min));
        var endDate = this.caption.endDate;
        var previousDate = this.getNextDate(-1, endDate);
        return previousDate < min
    };
    _proto.isNextButtonDisabled = function() {
        if (void 0 === this.props.max) {
            return false
        }
        var max = new Date(new Date(this.props.max).setHours(23, 59, 59));
        var startDate = this.caption.startDate;
        var nextDate = this.getNextDate(1, startDate);
        return nextDate > max
    };
    _proto.changeCalendarDate = function(date) {
        this.setState((function(__state_argument) {
            return {
                calendarVisible: false
            }
        }));
        this.setCurrentDate(date)
    };
    _proto.changeCalendarVisible = function(visible) {
        this.setState((function(__state_argument) {
            return {
                calendarVisible: visible
            }
        }))
    };
    _proto.showCalendar = function() {
        this.changeCalendarVisible(true)
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        if (this.props.startViewDate !== nextProps.startViewDate || this.props.viewType !== nextProps.viewType) {
            this.__getterCache.displayedDate = void 0
        }
        if (this.props.views !== nextProps.views) {
            this.__getterCache.views = void 0
        }
        if (this.props.viewType !== nextProps.viewType || this.props.intervalCount !== nextProps.intervalCount || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.agendaDuration !== nextProps.agendaDuration) {
            this.__getterCache.intervalOptions = void 0
        }
        if (this.props.useDropDownViewSwitcher !== nextProps.useDropDownViewSwitcher || this.state.currentView !== nextState.currentView || this.props.currentView !== nextProps.currentView || this.props.views !== nextProps.views || this.props.onCurrentViewUpdate !== nextProps.onCurrentViewUpdate || this.props.viewType !== nextProps.viewType || this.props.intervalCount !== nextProps.intervalCount || this.props.firstDayOfWeek !== nextProps.firstDayOfWeek || this.props.agendaDuration !== nextProps.agendaDuration || this.props.startViewDate !== nextProps.startViewDate || this.props.useShortDateFormat !== nextProps.useShortDateFormat || this.props.customizationFunction !== nextProps.customizationFunction || this.props.currentDate !== nextProps.currentDate || this.props.onCurrentDateUpdate !== nextProps.onCurrentDateUpdate || this.props.min !== nextProps.min || this.props.max !== nextProps.max || this.props.items !== nextProps.items) {
            this.__getterCache.items = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                currentView: void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView
            }),
            calendarVisible: this.state.calendarVisible,
            step: this.step,
            displayedDate: this.displayedDate,
            caption: this.caption,
            captionText: this.captionText,
            views: this.views,
            selectedView: this.selectedView,
            setCurrentView: this.setCurrentView,
            setCurrentDate: this.setCurrentDate,
            intervalOptions: this.intervalOptions,
            getNextDate: this.getNextDate,
            updateDateByDirection: this.updateDateByDirection,
            isPreviousButtonDisabled: this.isPreviousButtonDisabled,
            isNextButtonDisabled: this.isNextButtonDisabled,
            changeCalendarDate: this.changeCalendarDate,
            changeCalendarVisible: this.changeCalendarVisible,
            showCalendar: this.showCalendar,
            items: this.items,
            restAttributes: this.restAttributes
        })
    };
    _createClass(SchedulerToolbar, [{
        key: "step",
        get: function() {
            return (0, _utils.getStep)(this.props.viewType)
        }
    }, {
        key: "displayedDate",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.displayedDate) {
                return this.__getterCache.displayedDate
            }
            return this.__getterCache.displayedDate = function() {
                var startViewDate = new Date(_this2.props.startViewDate);
                if ((0, _utils2.isMonthView)(_this2.props.viewType)) {
                    return (0, _utils.nextWeek)(startViewDate)
                }
                return startViewDate
            }()
        }
    }, {
        key: "caption",
        get: function() {
            var options = {
                step: this.step,
                intervalCount: this.props.intervalCount,
                firstDayOfWeek: this.props.firstDayOfWeek,
                agendaDuration: this.props.agendaDuration,
                date: trimTime(this.displayedDate)
            };
            return (0, _utils.getCaption)(options, this.props.useShortDateFormat, this.props.customizationFunction)
        }
    }, {
        key: "captionText",
        get: function() {
            return this.caption.text
        }
    }, {
        key: "views",
        get: function() {
            var _this3 = this;
            if (void 0 !== this.__getterCache.views) {
                return this.__getterCache.views
            }
            return this.__getterCache.views = (0, _utils2.formatViews)(_this3.props.views)
        }
    }, {
        key: "selectedView",
        get: function() {
            return (0, _utils.getViewName)(void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView)
        }
    }, {
        key: "intervalOptions",
        get: function() {
            var _this4 = this;
            if (void 0 !== this.__getterCache.intervalOptions) {
                return this.__getterCache.intervalOptions
            }
            return this.__getterCache.intervalOptions = {
                step: _this4.step,
                intervalCount: _this4.props.intervalCount,
                firstDayOfWeek: _this4.props.firstDayOfWeek,
                agendaDuration: _this4.props.agendaDuration
            }
        }
    }, {
        key: "items",
        get: function() {
            var _this5 = this;
            if (void 0 !== this.__getterCache.items) {
                return this.__getterCache.items
            }
            return this.__getterCache.items = (options = {
                useDropDownViewSwitcher: _this5.props.useDropDownViewSwitcher,
                selectedView: _this5.selectedView,
                views: _this5.views,
                setCurrentView: function(view) {
                    return _this5.setCurrentView(view)
                },
                showCalendar: function() {
                    return _this5.showCalendar()
                },
                captionText: _this5.captionText,
                updateDateByDirection: function(direction) {
                    return _this5.updateDateByDirection(direction)
                },
                isPreviousButtonDisabled: _this5.isPreviousButtonDisabled(),
                isNextButtonDisabled: _this5.isNextButtonDisabled()
            }, _this5.props.items.map((function(item) {
                return (0, _utils2.formToolbarItem)(item, options)
            })));
            var options
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props$currentVi = _extends({}, this.props, {
                    currentView: void 0 !== this.props.currentView ? this.props.currentView : this.state.currentView
                }),
                restProps = (_this$props$currentVi.agendaDuration, _this$props$currentVi.currentDate, _this$props$currentVi.currentView, _this$props$currentVi.currentViewChange, _this$props$currentVi.customizationFunction, _this$props$currentVi.defaultCurrentView, _this$props$currentVi.firstDayOfWeek, _this$props$currentVi.intervalCount, _this$props$currentVi.items, _this$props$currentVi.max, _this$props$currentVi.min, _this$props$currentVi.onCurrentDateUpdate, _this$props$currentVi.onCurrentViewUpdate, _this$props$currentVi.startViewDate, _this$props$currentVi.useDropDownViewSwitcher, _this$props$currentVi.useShortDateFormat, _this$props$currentVi.viewType, _this$props$currentVi.views, _objectWithoutProperties(_this$props$currentVi, _excluded));
            return restProps
        }
    }]);
    return SchedulerToolbar
}(_inferno2.BaseInfernoComponent);
exports.SchedulerToolbar = SchedulerToolbar;

function __processTwoWayProps(defaultProps) {
    var twoWayProps = ["currentView"];
    return Object.keys(defaultProps).reduce((function(props, propName) {
        var propValue = defaultProps[propName];
        var defaultPropName = twoWayProps.some((function(p) {
            return p === propName
        })) ? "default" + propName.charAt(0).toUpperCase() + propName.slice(1) : propName;
        props[defaultPropName] = propValue;
        return props
    }), {})
}
SchedulerToolbar.defaultProps = SchedulerToolbarProps;
var __defaultOptionRules = [];

function defaultOptions(rule) {
    __defaultOptionRules.push(rule);
    SchedulerToolbar.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(SchedulerToolbar.defaultProps), Object.getOwnPropertyDescriptors(__processTwoWayProps((0, _utils3.convertRulesToOptions)(__defaultOptionRules)))))
}
