/**
 * DevExtreme (cjs/ui/scheduler/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.utils = void 0;
var _size = require("../../core/utils/size");
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _iterator = require("../../core/utils/iterator");
var _constants = require("./constants");
var _element = require("../../core/element");
var _data = require("../../core/utils/data");
var _date_serialization = _interopRequireDefault(require("../../core/utils/date_serialization"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
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
var utils = {
    dataAccessors: {
        getAppointmentSettings: function(element) {
            return (0, _renderer.default)(element).data(_constants.APPOINTMENT_SETTINGS_KEY)
        },
        getAppointmentInfo: function(element) {
            var settings = utils.dataAccessors.getAppointmentSettings(element);
            return null === settings || void 0 === settings ? void 0 : settings.info
        },
        create: function(fields, currentDataAccessors, forceIsoDateParsing, dateSerializationFormat) {
            var dataAccessors = currentDataAccessors ? _extends({}, currentDataAccessors) : {
                getter: {},
                setter: {},
                expr: {}
            };
            (0, _iterator.each)(fields, (function(name, expr) {
                if (expr) {
                    var getter = (0, _data.compileGetter)(expr);
                    var setter = (0, _data.compileSetter)(expr);
                    var dateGetter;
                    var dateSetter;
                    var serializationFormat;
                    if (field = name, "startDate" === field || "endDate" === field) {
                        dateGetter = function(object) {
                            var value = getter(object);
                            if (forceIsoDateParsing) {
                                value = _date_serialization.default.deserializeDate(value)
                            }
                            return value
                        };
                        dateSetter = function(object, value) {
                            if (dateSerializationFormat) {
                                serializationFormat = dateSerializationFormat
                            } else if (forceIsoDateParsing && !serializationFormat) {
                                var oldValue = getter(object);
                                serializationFormat = _date_serialization.default.getDateSerializationFormat(oldValue)
                            }
                            var newValue = _date_serialization.default.serializeDate(value, serializationFormat);
                            setter(object, newValue)
                        }
                    }
                    dataAccessors.getter[name] = dateGetter || getter;
                    dataAccessors.setter[name] = dateSetter || setter;
                    dataAccessors.expr["".concat(name, "Expr")] = expr
                } else {
                    delete dataAccessors.getter[name];
                    delete dataAccessors.setter[name];
                    delete dataAccessors.expr["".concat(name, "Expr")]
                }
                var field
            }));
            return dataAccessors
        }
    },
    DOM: {
        getHeaderHeight: function(header) {
            return header ? header._$element && parseInt((0, _size.getOuterHeight)(header._$element), 10) : 0
        }
    },
    renovation: {
        renderComponent: function(widget, parentElement, componentClass, componentName, viewModel) {
            var component = widget[componentName];
            if (!component) {
                var container = (0, _element.getPublicElement)(parentElement);
                component = widget._createComponent(container, componentClass, viewModel);
                widget[componentName] = component
            } else {
                var $element = component.$element();
                var elementStyle = $element.get(0).style;
                var height = elementStyle.height;
                var width = elementStyle.width;
                component.option(viewModel);
                if (height) {
                    (0, _size.setHeight)($element, height)
                }
                if (width) {
                    (0, _size.setWidth)($element, width)
                }
            }
        }
    }
};
exports.utils = utils;
