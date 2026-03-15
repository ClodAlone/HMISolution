/**
 * DevExtreme (renovation/ui/scheduler/appointment/appointment.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.AppointmentProps = exports.Appointment = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _utils = require("./utils");
var _content = require("./content");
var _combine_classes = require("../../../utils/combine_classes");
var _excluded = ["appointmentTemplate", "index", "viewModel"];

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
        data = _ref.data,
        dateText = _ref.dateText,
        index = _ref.index,
        isReduced = _ref.isReduced,
        _ref$props = _ref.props,
        appointmentTemplate = _ref$props.appointmentTemplate,
        isRecurrent = _ref$props.viewModel.info.isRecurrent,
        styles = _ref.styles,
        text = _ref.text;
    var AppointmentTemplate = appointmentTemplate;
    return (0, _inferno.createVNode)(1, "div", classes, [!!appointmentTemplate && AppointmentTemplate({
        data: data,
        index: index
    }), !appointmentTemplate && (0, _inferno.createComponentVNode)(2, _content.AppointmentContent, {
        text: text,
        dateText: dateText,
        isRecurrent: isRecurrent,
        isReduced: isReduced
    })], 0, {
        style: (0, _inferno2.normalizeStyles)(styles),
        title: text,
        role: "button"
    })
};
exports.viewFunction = viewFunction;
var AppointmentProps = {
    index: 0
};
exports.AppointmentProps = AppointmentProps;
var getTemplate = function(TemplateProp) {
    return TemplateProp && (TemplateProp.defaultProps ? function(props) {
        return (0, _inferno.normalizeProps)((0, _inferno.createComponentVNode)(2, TemplateProp, _extends({}, props)))
    } : TemplateProp)
};
var Appointment = function(_BaseInfernoComponent) {
    _inheritsLoose(Appointment, _BaseInfernoComponent);

    function Appointment(props) {
        var _this;
        _this = _BaseInfernoComponent.call(this, props) || this;
        _this.state = {};
        return _this
    }
    var _proto = Appointment.prototype;
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                appointmentTemplate: getTemplate(props.appointmentTemplate)
            }),
            text: this.text,
            dateText: this.dateText,
            styles: this.styles,
            data: this.data,
            index: this.index,
            isReduced: this.isReduced,
            classes: this.classes,
            restAttributes: this.restAttributes
        })
    };
    _createClass(Appointment, [{
        key: "text",
        get: function() {
            return this.props.viewModel.appointment.text
        }
    }, {
        key: "dateText",
        get: function() {
            return this.props.viewModel.info.dateText
        }
    }, {
        key: "styles",
        get: function() {
            return (0, _utils.getAppointmentStyles)(this.props.viewModel)
        }
    }, {
        key: "data",
        get: function() {
            return {
                appointmentData: this.props.viewModel.info.appointment,
                targetedAppointmentData: this.props.viewModel.appointment
            }
        }
    }, {
        key: "index",
        get: function() {
            return this.props.index
        }
    }, {
        key: "isReduced",
        get: function() {
            var appointmentReduced = this.props.viewModel.info.appointmentReduced;
            return !!appointmentReduced
        }
    }, {
        key: "classes",
        get: function() {
            var _this$props$viewModel = this.props.viewModel.info,
                allDay = _this$props$viewModel.allDay,
                appointmentReduced = _this$props$viewModel.appointmentReduced,
                direction = _this$props$viewModel.direction,
                isRecurrent = _this$props$viewModel.isRecurrent;
            var isVerticalDirection = "vertical" === direction;
            return (0, _combine_classes.combineClasses)({
                "dx-scheduler-appointment": true,
                "dx-scheduler-appointment-horizontal": !isVerticalDirection,
                "dx-scheduler-appointment-vertical": isVerticalDirection,
                "dx-scheduler-appointment-recurrence": isRecurrent,
                "dx-scheduler-all-day-appointment": allDay,
                "dx-scheduler-appointment-reduced": this.isReduced,
                "dx-scheduler-appointment-head": "head" === appointmentReduced,
                "dx-scheduler-appointment-body": "body" === appointmentReduced,
                "dx-scheduler-appointment-tail": "tail" === appointmentReduced
            })
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props = this.props,
                restProps = (_this$props.appointmentTemplate, _this$props.index, _this$props.viewModel, _objectWithoutProperties(_this$props, _excluded));
            return restProps
        }
    }]);
    return Appointment
}(_inferno2.BaseInfernoComponent);
exports.Appointment = Appointment;
Appointment.defaultProps = AppointmentProps;
