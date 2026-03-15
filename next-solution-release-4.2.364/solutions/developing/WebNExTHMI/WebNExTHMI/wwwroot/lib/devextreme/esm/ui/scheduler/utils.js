/**
 * DevExtreme (esm/ui/scheduler/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    getOuterHeight,
    setHeight,
    setWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    each
} from "../../core/utils/iterator";
import {
    APPOINTMENT_SETTINGS_KEY
} from "./constants";
import {
    getPublicElement
} from "../../core/element";
import {
    compileGetter,
    compileSetter
} from "../../core/utils/data";
import dateSerialization from "../../core/utils/date_serialization";
export var utils = {
    dataAccessors: {
        getAppointmentSettings: element => $(element).data(APPOINTMENT_SETTINGS_KEY),
        getAppointmentInfo: element => {
            var settings = utils.dataAccessors.getAppointmentSettings(element);
            return null === settings || void 0 === settings ? void 0 : settings.info
        },
        create: (fields, currentDataAccessors, forceIsoDateParsing, dateSerializationFormat) => {
            var dataAccessors = currentDataAccessors ? _extends({}, currentDataAccessors) : {
                getter: {},
                setter: {},
                expr: {}
            };
            each(fields, (name, expr) => {
                if (expr) {
                    var getter = compileGetter(expr);
                    var setter = compileSetter(expr);
                    var dateGetter;
                    var dateSetter;
                    var serializationFormat;
                    if (field = name, "startDate" === field || "endDate" === field) {
                        dateGetter = object => {
                            var value = getter(object);
                            if (forceIsoDateParsing) {
                                value = dateSerialization.deserializeDate(value)
                            }
                            return value
                        };
                        dateSetter = (object, value) => {
                            if (dateSerializationFormat) {
                                serializationFormat = dateSerializationFormat
                            } else if (forceIsoDateParsing && !serializationFormat) {
                                var oldValue = getter(object);
                                serializationFormat = dateSerialization.getDateSerializationFormat(oldValue)
                            }
                            var newValue = dateSerialization.serializeDate(value, serializationFormat);
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
            });
            return dataAccessors
        }
    },
    DOM: {
        getHeaderHeight: header => header ? header._$element && parseInt(getOuterHeight(header._$element), 10) : 0
    },
    renovation: {
        renderComponent: (widget, parentElement, componentClass, componentName, viewModel) => {
            var component = widget[componentName];
            if (!component) {
                var container = getPublicElement(parentElement);
                component = widget._createComponent(container, componentClass, viewModel);
                widget[componentName] = component
            } else {
                var $element = component.$element();
                var elementStyle = $element.get(0).style;
                var height = elementStyle.height;
                var width = elementStyle.width;
                component.option(viewModel);
                if (height) {
                    setHeight($element, height)
                }
                if (width) {
                    setWidth($element, width)
                }
            }
        }
    }
};
