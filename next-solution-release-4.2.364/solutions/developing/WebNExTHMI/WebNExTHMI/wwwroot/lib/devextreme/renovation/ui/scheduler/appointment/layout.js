/**
 * DevExtreme (renovation/ui/scheduler/appointment/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.AppointmentLayoutProps = exports.AppointmentLayout = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _appointment = require("./appointment");
var _layout = require("./overflow_indicator/layout");
var _combine_classes = require("../../../utils/combine_classes");
var _excluded = ["appointmentTemplate", "appointments", "isAllDay", "overflowIndicatorTemplate", "overflowIndicators"];

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
    var classes = _ref.classes,
        _ref$props = _ref.props,
        appointmentTemplate = _ref$props.appointmentTemplate,
        appointments = _ref$props.appointments,
        overflowIndicatorTemplate = _ref$props.overflowIndicatorTemplate,
        overflowIndicators = _ref$props.overflowIndicators;
    return (0, _inferno.createVNode)(1, "div", classes, [appointments.map((function(item, index) {
        return (0, _inferno.createComponentVNode)(2, _appointment.Appointment, {
            viewModel: item,
            appointmentTemplate: appointmentTemplate,
            index: index
        }, item.key)
    })), overflowIndicators.map((function(item) {
        return (0, _inferno.createComponentVNode)(2, _layout.OverflowIndicator, {
            viewModel: item,
            overflowIndicatorTemplate: overflowIndicatorTemplate
        }, item.key)
    }))], 0)
};
exports.viewFunction = viewFunction;
var AppointmentLayoutProps = Object.defineProperties({
    isAllDay: false
}, {
    appointments: {
        get: function() {
            return []
        },
        configurable: true,
        enumerable: true
    },
    overflowIndicators: {
        get: function() {
            return []
        },
        configurable: true,
        enumerable: true
    }
});
exports.AppointmentLayoutProps = AppointmentLayoutProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var AppointmentLayout = function(_InfernoWrapperCompon) {
    _inheritsLoose(AppointmentLayout, _InfernoWrapperCompon);

    function AppointmentLayout(props) {
        var _this;
        _this = _InfernoWrapperCompon.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = AppointmentLayout.prototype;
    _proto.createEffects = function() {
        return [(0, _inferno2.createReRenderEffect)()]
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                appointmentTemplate: getTemplate(props.appointmentTemplate),
                overflowIndicatorTemplate: getTemplate(props.overflowIndicatorTemplate)
            }),
            classes: this.classes,
            restAttributes: this.restAttributes
        })
    };
    _createClass(AppointmentLayout, [{
        key: "classes",
        get: function() {
            var isAllDay = this.props.isAllDay;
            return (0, _combine_classes.combineClasses)({
                "dx-scheduler-scrollable-appointments": !isAllDay,
                "dx-scheduler-all-day-appointments": isAllDay
            })
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.appointmentTemplate, _this$props.appointments, _this$props.isAllDay, _this$props.overflowIndicatorTemplate, _this$props.overflowIndicators, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return AppointmentLayout
}(_inferno2.InfernoWrapperComponent);
exports.AppointmentLayout = AppointmentLayout;
AppointmentLayout.defaultProps = AppointmentLayoutProps;
