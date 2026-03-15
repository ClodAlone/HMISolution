/**
 * DevExtreme (cjs/ui/scheduler/appointmentPopup/form.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.AppointmentForm = exports.APPOINTMENT_FORM_GROUP_NAMES = void 0;
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _form = _interopRequireDefault(require("../../form"));
var _date_serialization = _interopRequireDefault(require("../../../core/utils/date_serialization"));
var _message = _interopRequireDefault(require("../../../localization/message"));
var _devices = _interopRequireDefault(require("../../../core/devices"));
var _data_source = _interopRequireDefault(require("../../../data/data_source"));
var _utils = _interopRequireDefault(require("../timezones/utils.timezones_data"));
var _extend = require("../../../core/utils/extend");
var _date = _interopRequireDefault(require("../../../core/utils/date"));
var _semaphore = require("../../../renovation/ui/scheduler/utils/semaphore/semaphore");
require("../recurrence_editor");
require("../../text_area");
require("../../tag_box");
require("../../switch");
require("../../select_box");

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
var SCREEN_SIZE_OF_SINGLE_COLUMN = 600;
var APPOINTMENT_FORM_GROUP_NAMES = {
    Main: "mainGroup",
    Recurrence: "recurrenceGroup"
};
exports.APPOINTMENT_FORM_GROUP_NAMES = APPOINTMENT_FORM_GROUP_NAMES;
var getStartDateWithStartHour = function(startDate, startDayHour) {
    return new Date(new Date(startDate).setHours(startDayHour))
};
var validateAppointmentFormDate = function(editor, value, previousValue) {
    var isCurrentDateCorrect = null === value || !!value;
    var isPreviousDateCorrect = null === previousValue || !!previousValue;
    if (!isCurrentDateCorrect && isPreviousDateCorrect) {
        editor.option("value", previousValue)
    }
};
var updateRecurrenceItemVisibility = function(recurrenceRuleExpr, value, form) {
    var _form$getEditor;
    form.itemOption(APPOINTMENT_FORM_GROUP_NAMES.Recurrence, "visible", value);
    !value && form.updateData(recurrenceRuleExpr, "");
    null === (_form$getEditor = form.getEditor(recurrenceRuleExpr)) || void 0 === _form$getEditor ? void 0 : _form$getEditor.changeValueByVisibility(value)
};
var createDateBoxEditor = function(dataField, colSpan, firstDayOfWeek, label, onValueChanged) {
    return {
        editorType: "dxDateBox",
        dataField: dataField,
        colSpan: colSpan,
        label: {
            text: _message.default.format(label)
        },
        validationRules: [{
            type: "required"
        }],
        editorOptions: {
            width: "100%",
            calendarOptions: {
                firstDayOfWeek: firstDayOfWeek
            },
            onValueChanged: onValueChanged,
            useMaskBehavior: true
        }
    }
};
var AppointmentForm = function() {
    function AppointmentForm(scheduler) {
        this.scheduler = scheduler;
        this.form = null;
        this.semaphore = new _semaphore.Semaphore
    }
    var _proto = AppointmentForm.prototype;
    _proto.create = function(triggerResize, changeSize, formData) {
        var allowTimeZoneEditing = this.scheduler.getEditingConfig().allowTimeZoneEditing;
        var _this$scheduler$getDa = this.scheduler.getDataAccessors(),
            expr = _this$scheduler$getDa.expr;
        var recurrenceEditorVisibility = !!formData[expr.recurrenceRuleExpr];
        var colSpan = recurrenceEditorVisibility ? 1 : 2;
        var mainItems = [].concat(_toConsumableArray(this._createMainItems(expr, triggerResize, changeSize, allowTimeZoneEditing)), _toConsumableArray(this.scheduler.createResourceEditorModel()));
        changeSize(recurrenceEditorVisibility);
        var items = [{
            itemType: "group",
            name: APPOINTMENT_FORM_GROUP_NAMES.Main,
            colCountByScreen: {
                lg: 2,
                xs: 1
            },
            colSpan: colSpan,
            items: mainItems
        }, {
            itemType: "group",
            name: APPOINTMENT_FORM_GROUP_NAMES.Recurrence,
            visible: recurrenceEditorVisibility,
            colSpan: colSpan,
            items: this._createRecurrenceEditor(expr)
        }];
        var element = (0, _renderer.default)("<div>");
        this.form = this.scheduler.createComponent(element, _form.default, {
            items: items,
            showValidationSummary: true,
            scrollingEnabled: true,
            colCount: "auto",
            colCountByScreen: {
                lg: 2,
                xs: 1
            },
            formData: formData,
            showColonAfterLabel: false,
            labelLocation: "top",
            screenByWidth: function(width) {
                return width < SCREEN_SIZE_OF_SINGLE_COLUMN || "desktop" !== _devices.default.current().deviceType ? "xs" : "lg"
            }
        })
    };
    _proto._dateBoxValueChanged = function(args, dateExpr, isNeedCorrect) {
        validateAppointmentFormDate(args.component, args.value, args.previousValue);
        var value = _date_serialization.default.deserializeDate(args.value);
        var previousValue = _date_serialization.default.deserializeDate(args.previousValue);
        var dateEditor = this.form.getEditor(dateExpr);
        var dateValue = _date_serialization.default.deserializeDate(dateEditor.option("value"));
        if (this.semaphore.isFree() && dateValue && value && isNeedCorrect(dateValue, value)) {
            var duration = previousValue ? dateValue.getTime() - previousValue.getTime() : 0;
            dateEditor.option("value", new Date(value.getTime() + duration))
        }
    };
    _proto._createTimezoneEditor = function(timeZoneExpr, secondTimeZoneExpr, visibleIndex, colSpan, isMainTimeZone) {
        var _this = this;
        var visible = arguments.length > 5 && void 0 !== arguments[5] ? arguments[5] : false;
        var noTzTitle = _message.default.format("dxScheduler-noTimezoneTitle");
        return {
            dataField: timeZoneExpr,
            editorType: "dxSelectBox",
            visibleIndex: visibleIndex,
            colSpan: colSpan,
            label: {
                text: " "
            },
            editorOptions: {
                displayExpr: "title",
                valueExpr: "id",
                placeholder: noTzTitle,
                searchEnabled: true,
                onValueChanged: function(args) {
                    var form = _this.form;
                    var secondTimezoneEditor = form.getEditor(secondTimeZoneExpr);
                    if (isMainTimeZone) {
                        secondTimezoneEditor.option("value", args.value)
                    }
                }
            },
            visible: visible
        }
    };
    _proto._createDateBoxItems = function(dataExprs, allowTimeZoneEditing) {
        var _this2 = this;
        var colSpan = allowTimeZoneEditing ? 2 : 1;
        var firstDayOfWeek = this.scheduler.getFirstDayOfWeek();
        return [createDateBoxEditor(dataExprs.startDateExpr, colSpan, firstDayOfWeek, "dxScheduler-editorLabelStartDate", (function(args) {
            _this2._dateBoxValueChanged(args, dataExprs.endDateExpr, (function(endValue, startValue) {
                return endValue < startValue
            }))
        })), this._createTimezoneEditor(dataExprs.startDateTimeZoneExpr, dataExprs.endDateTimeZoneExpr, 1, colSpan, true, allowTimeZoneEditing), createDateBoxEditor(dataExprs.endDateExpr, colSpan, firstDayOfWeek, "dxScheduler-editorLabelEndDate", (function(args) {
            _this2._dateBoxValueChanged(args, dataExprs.startDateExpr, (function(startValue, endValue) {
                return endValue < startValue
            }))
        })), this._createTimezoneEditor(dataExprs.endDateTimeZoneExpr, dataExprs.startDateTimeZoneExpr, 3, colSpan, false, allowTimeZoneEditing)]
    };
    _proto._changeFormItemDateType = function(itemPath, isAllDay) {
        var itemEditorOptions = this.form.itemOption(itemPath).editorOptions;
        var type = isAllDay ? "date" : "datetime";
        var newEditorOption = _extends({}, itemEditorOptions, {
            type: type
        });
        this.form.itemOption(itemPath, "editorOptions", newEditorOption)
    };
    _proto._createMainItems = function(dataExprs, triggerResize, changeSize, allowTimeZoneEditing) {
        var _this3 = this;
        return [{
            dataField: dataExprs.textExpr,
            editorType: "dxTextBox",
            colSpan: 2,
            label: {
                text: _message.default.format("dxScheduler-editorLabelTitle")
            }
        }, {
            itemType: "group",
            colSpan: 2,
            colCountByScreen: {
                lg: 2,
                xs: 1
            },
            items: this._createDateBoxItems(dataExprs, allowTimeZoneEditing)
        }, {
            itemType: "group",
            colCountByScreen: {
                lg: 3,
                xs: 3
            },
            colSpan: 2,
            items: [{
                dataField: dataExprs.allDayExpr,
                cssClass: "dx-appointment-form-switch",
                editorType: "dxSwitch",
                label: {
                    text: _message.default.format("dxScheduler-allDay"),
                    location: "right"
                },
                editorOptions: {
                    onValueChanged: function(args) {
                        var value = args.value;
                        var startDateEditor = _this3.form.getEditor(dataExprs.startDateExpr);
                        var endDateEditor = _this3.form.getEditor(dataExprs.endDateExpr);
                        var startDate = _date_serialization.default.deserializeDate(startDateEditor.option("value"));
                        if (_this3.semaphore.isFree() && startDate) {
                            if (value) {
                                var allDayStartDate = _date.default.trimTime(startDate);
                                startDateEditor.option("value", new Date(allDayStartDate));
                                endDateEditor.option("value", new Date(allDayStartDate))
                            } else {
                                var startDateWithStartHour = getStartDateWithStartHour(startDate, _this3.scheduler.getStartDayHour());
                                var endDate = _this3.scheduler.getCalculatedEndDate(startDateWithStartHour);
                                startDateEditor.option("value", startDateWithStartHour);
                                endDateEditor.option("value", endDate)
                            }
                        }
                        var startDateItemPath = "".concat(APPOINTMENT_FORM_GROUP_NAMES.Main, ".").concat(dataExprs.startDateExpr);
                        var endDateItemPath = "".concat(APPOINTMENT_FORM_GROUP_NAMES.Main, ".").concat(dataExprs.endDateExpr);
                        _this3._changeFormItemDateType(startDateItemPath, value);
                        _this3._changeFormItemDateType(endDateItemPath, value)
                    }
                }
            }, {
                editorType: "dxSwitch",
                dataField: "repeat",
                cssClass: "dx-appointment-form-switch",
                name: "visibilityChanged",
                label: {
                    text: _message.default.format("dxScheduler-editorLabelRecurrence"),
                    location: "right"
                },
                editorOptions: {
                    onValueChanged: function(args) {
                        var form = _this3.form;
                        var colSpan = args.value ? 1 : 2;
                        form.itemOption(APPOINTMENT_FORM_GROUP_NAMES.Main, "colSpan", colSpan);
                        form.itemOption(APPOINTMENT_FORM_GROUP_NAMES.Recurrence, "colSpan", colSpan);
                        updateRecurrenceItemVisibility(dataExprs.recurrenceRuleExpr, args.value, form);
                        changeSize(args.value);
                        triggerResize()
                    }
                }
            }]
        }, {
            itemType: "empty",
            colSpan: 2
        }, {
            dataField: dataExprs.descriptionExpr,
            editorType: "dxTextArea",
            colSpan: 2,
            label: {
                text: _message.default.format("dxScheduler-editorLabelDescription")
            }
        }, {
            itemType: "empty",
            colSpan: 2
        }]
    };
    _proto._createRecurrenceEditor = function(dataExprs) {
        return [{
            dataField: dataExprs.recurrenceRuleExpr,
            editorType: "dxRecurrenceEditor",
            editorOptions: {
                firstDayOfWeek: this.scheduler.getFirstDayOfWeek()
            },
            label: {
                text: " ",
                visible: false
            }
        }]
    };
    _proto.setEditorsType = function(allDay) {
        var _this$scheduler$getDa2 = this.scheduler.getDataAccessors().expr,
            startDateExpr = _this$scheduler$getDa2.startDateExpr,
            endDateExpr = _this$scheduler$getDa2.endDateExpr;
        var startDateItemPath = "".concat(APPOINTMENT_FORM_GROUP_NAMES.Main, ".").concat(startDateExpr);
        var endDateItemPath = "".concat(APPOINTMENT_FORM_GROUP_NAMES.Recurrence, ".").concat(endDateExpr);
        var startDateFormItem = this.form.itemOption(startDateItemPath);
        var endDateFormItem = this.form.itemOption(endDateItemPath);
        if (startDateFormItem && endDateFormItem) {
            var startDateEditorOptions = startDateFormItem.editorOptions;
            var endDateEditorOptions = endDateFormItem.editorOptions;
            startDateEditorOptions.type = endDateEditorOptions.type = allDay ? "date" : "datetime";
            this.form.itemOption(startDateItemPath, "editorOptions", startDateEditorOptions);
            this.form.itemOption(endDateItemPath, "editorOptions", endDateEditorOptions)
        }
    };
    _proto.updateTimeZoneEditorDataSource = function(date, expression) {
        var timeZoneDataSource = new _data_source.default({
            store: _utils.default.getDisplayedTimeZones(date),
            paginate: true,
            pageSize: 10
        });
        var options = {
            dataSource: timeZoneDataSource
        };
        this.setEditorOptions(expression, "Main", options)
    };
    _proto.updateRecurrenceEditorStartDate = function(date, expression) {
        var options = {
            startDate: date
        };
        this.setEditorOptions(expression, "Recurrence", options)
    };
    _proto.setEditorOptions = function(name, groupName, options) {
        var editorPath = "".concat(APPOINTMENT_FORM_GROUP_NAMES.groupName, ".").concat(name);
        var editor = this.form.itemOption(editorPath);
        editor && this.form.itemOption(editorPath, "editorOptions", (0, _extend.extend)({}, editor.editorOptions, options))
    };
    _proto.updateFormData = function(formData) {
        this.semaphore.take();
        var dataExprs = this.scheduler.getDataAccessors().expr;
        var allDay = formData[dataExprs.allDayExpr];
        var startDate = new Date(formData[dataExprs.startDateExpr]);
        var endDate = new Date(formData[dataExprs.endDateExpr]);
        this.updateTimeZoneEditorDataSource(startDate, dataExprs.startDateTimeZoneExpr);
        this.updateTimeZoneEditorDataSource(endDate, dataExprs.endDateTimeZoneExpr);
        this.updateRecurrenceEditorStartDate(startDate, dataExprs.recurrenceRuleExpr);
        this.form.option("formData", formData);
        this.setEditorsType(allDay);
        this.semaphore.release()
    };
    _createClass(AppointmentForm, [{
        key: "dxForm",
        get: function() {
            return this.form
        }
    }, {
        key: "readOnly",
        set: function(value) {
            this.form.option("readOnly", value);
            var recurrenceRuleExpr = this.scheduler.getDataAccessors().expr.recurrenceRuleExpr;
            var recurrenceEditor = this.form.getEditor(recurrenceRuleExpr);
            null === recurrenceEditor || void 0 === recurrenceEditor ? void 0 : recurrenceEditor.option("readOnly", value)
        }
    }, {
        key: "formData",
        get: function() {
            return this.form.option("formData")
        },
        set: function(value) {
            this.form.option("formData", value)
        }
    }]);
    return AppointmentForm
}();
exports.AppointmentForm = AppointmentForm;
