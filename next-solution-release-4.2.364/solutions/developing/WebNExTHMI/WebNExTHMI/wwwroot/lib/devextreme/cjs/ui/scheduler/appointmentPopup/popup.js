/**
 * DevExtreme (cjs/ui/scheduler/appointmentPopup/popup.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.AppointmentPopup = exports.ACTION_TO_APPOINTMENT = void 0;
var _size = require("../../../core/utils/size");
var _devices = _interopRequireDefault(require("../../../core/devices"));
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _date = _interopRequireDefault(require("../../../core/utils/date"));
var _deferred = require("../../../core/utils/deferred");
var _window = require("../../../core/utils/window");
var _visibility_change = require("../../../events/visibility_change");
var _message = _interopRequireDefault(require("../../../localization/message"));
var _popup = _interopRequireDefault(require("../../popup"));
var _loading = require("../loading");
var _appointmentAdapter = require("../appointmentAdapter");
var _iterator = require("../../../core/utils/iterator");
var _utils = require("../resources/utils");
var _array = require("../../../core/utils/array");

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
var APPOINTMENT_POPUP_CLASS = "dx-scheduler-appointment-popup";
var isMobile = function() {
    return "desktop" !== _devices.default.current().deviceType
};
var isIOSPlatform = function() {
    return "ios" === _devices.default.current().platform
};
var POPUP_WIDTH = {
    DEFAULT: 485,
    RECURRENCE: 970,
    FULLSCREEN: 1e3,
    MOBILE: {
        DEFAULT: 350,
        FULLSCREEN: 500
    }
};
var TOOLBAR_LOCATION = {
    AFTER: "after",
    BEFORE: "before"
};
var DAY_IN_MS = toMs("day");
var POPUP_CONFIG = {
    height: "auto",
    maxHeight: "100%",
    showCloseButton: false,
    showTitle: false,
    defaultOptionsRules: [{
        device: function() {
            return _devices.default.current().android
        },
        options: {
            showTitle: false
        }
    }]
};
var createDoneButtonConfig = function() {
    return {
        shortcut: "done",
        options: {
            text: _message.default.format("Done")
        },
        location: TOOLBAR_LOCATION.AFTER
    }
};
var createCancelButtonConfig = function() {
    return {
        shortcut: "cancel",
        location: isIOSPlatform() ? TOOLBAR_LOCATION.BEFORE : TOOLBAR_LOCATION.AFTER
    }
};
var modifyResourceFields = function(rawAppointment, dataAccessors, resources, returnedObject) {
    (0, _iterator.each)(dataAccessors.resources.getter, (function(fieldName) {
        var value = dataAccessors.resources.getter[fieldName](rawAppointment);
        var isMultiple = (0, _utils.isResourceMultiple)(resources, fieldName);
        returnedObject[fieldName] = isMultiple ? (0, _array.wrapToArray)(value) : value
    }))
};
var ACTION_TO_APPOINTMENT = {
    CREATE: 0,
    UPDATE: 1,
    EXCLUDE_FROM_SERIES: 2
};
exports.ACTION_TO_APPOINTMENT = ACTION_TO_APPOINTMENT;
var AppointmentPopup = function() {
    function AppointmentPopup(scheduler, form) {
        this.scheduler = scheduler;
        this.form = form;
        this.popup = null;
        this.state = {
            action: null,
            lastEditData: null,
            saveChangesLocker: false,
            appointment: {
                data: null
            }
        }
    }
    var _proto = AppointmentPopup.prototype;
    _proto.show = function(appointment, config) {
        this.state.appointment.data = appointment;
        this.state.action = config.action;
        this.state.excludeInfo = config.excludeInfo;
        if (!this.popup) {
            var popupConfig = this._createPopupConfig();
            this.popup = this._createPopup(popupConfig)
        }
        this.popup.option("toolbarItems", this._createPopupToolbarItems(config.isToolbarVisible));
        this.popup.show()
    };
    _proto.hide = function() {
        this.popup.hide()
    };
    _proto.dispose = function() {
        var _this$popup;
        null === (_this$popup = this.popup) || void 0 === _this$popup ? void 0 : _this$popup.$element().remove()
    };
    _proto._createPopup = function(options) {
        var popupElement = (0, _renderer.default)("<div>").addClass(APPOINTMENT_POPUP_CLASS).appendTo(this.scheduler.getElement());
        return this.scheduler.createComponent(popupElement, _popup.default, options)
    };
    _proto._createPopupConfig = function() {
        var _this = this;
        return _extends({}, POPUP_CONFIG, {
            onHiding: function() {
                return _this.scheduler.focus()
            },
            contentTemplate: function() {
                return _this._createPopupContent()
            },
            onShowing: function(e) {
                return _this._onShowing(e)
            },
            copyRootClassesToWrapper: true,
            _ignoreCopyRootClassesToWrapperDeprecation: true
        })
    };
    _proto._onShowing = function(e) {
        var _this2 = this;
        this._updateForm();
        var arg = {
            form: this.form.dxForm,
            popup: this.popup,
            appointmentData: this.state.appointment.data,
            cancel: false
        };
        this.scheduler.getAppointmentFormOpening()(arg);
        this.scheduler.processActionResult(arg, (function(canceled) {
            if (canceled) {
                e.cancel = true
            } else {
                _this2.updatePopupFullScreenMode()
            }
        }))
    };
    _proto._createPopupContent = function() {
        this._createForm();
        return this.form.dxForm.$element()
    };
    _proto._createFormData = function(rawAppointment) {
        var appointment = this._createAppointmentAdapter(rawAppointment);
        var dataAccessors = this.scheduler.getDataAccessors();
        var resources = this.scheduler.getResources();
        var result = _extends({}, rawAppointment, {
            repeat: !!appointment.recurrenceRule
        });
        modifyResourceFields(rawAppointment, dataAccessors, resources, result);
        return result
    };
    _proto._createForm = function() {
        var rawAppointment = this.state.appointment.data;
        var formData = this._createFormData(rawAppointment);
        this.form.create(this.triggerResize.bind(this), this.changeSize.bind(this), formData)
    };
    _proto._isReadOnly = function(rawAppointment) {
        var appointment = this._createAppointmentAdapter(rawAppointment);
        if (rawAppointment && appointment.disabled) {
            return true
        }
        if (this.state.action === ACTION_TO_APPOINTMENT.CREATE) {
            return false
        }
        return !this.scheduler.getEditingConfig().allowUpdating
    };
    _proto._createAppointmentAdapter = function(rawAppointment) {
        return (0, _appointmentAdapter.createAppointmentAdapter)(rawAppointment, this.scheduler.getDataAccessors(), this.scheduler.getTimeZoneCalculator())
    };
    _proto._updateForm = function() {
        var data = this.state.appointment.data;
        var appointment = this._createAppointmentAdapter(this._createFormData(data));
        if (appointment.startDate) {
            appointment.startDate = appointment.calculateStartDate("toAppointment")
        }
        if (appointment.endDate) {
            appointment.endDate = appointment.calculateEndDate("toAppointment")
        }
        var formData = appointment.source();
        this.form.readOnly = this._isReadOnly(formData);
        this.form.updateFormData(formData)
    };
    _proto._isPopupFullScreenNeeded = function() {
        var width = this._tryGetWindowWidth();
        if (width) {
            return isMobile() ? width < POPUP_WIDTH.MOBILE.FULLSCREEN : width < POPUP_WIDTH.FULLSCREEN
        }
        return false
    };
    _proto._tryGetWindowWidth = function() {
        if ((0, _window.hasWindow)()) {
            var window = (0, _window.getWindow)();
            return (0, _size.getWidth)(window)
        }
    };
    _proto.triggerResize = function() {
        if (this.popup) {
            (0, _visibility_change.triggerResizeEvent)(this.popup.$element())
        }
    };
    _proto._getMaxWidth = function(isRecurrence) {
        if (isMobile()) {
            return POPUP_WIDTH.MOBILE.DEFAULT
        }
        return isRecurrence ? POPUP_WIDTH.RECURRENCE : POPUP_WIDTH.DEFAULT
    };
    _proto.changeSize = function(isRecurrence) {
        if (this.popup) {
            var fullScreen = this._isPopupFullScreenNeeded();
            this.popup.option({
                fullScreen: fullScreen,
                maxWidth: fullScreen ? "100%" : this._getMaxWidth(isRecurrence)
            })
        }
    };
    _proto.updatePopupFullScreenMode = function() {
        if (this.form.dxForm) {
            var formData = this.form.formData;
            var isRecurrence = formData[this.scheduler.getDataAccessors().expr.recurrenceRuleExpr];
            if (this.visible) {
                this.changeSize(isRecurrence)
            }
        }
    };
    _proto._createPopupToolbarItems = function(isVisible) {
        var _this3 = this;
        var result = [];
        if (isVisible) {
            result.push(_extends({}, createDoneButtonConfig(), {
                onClick: function(e) {
                    return _this3._doneButtonClickHandler(e)
                }
            }))
        }
        result.push(createCancelButtonConfig());
        return result
    };
    _proto.saveChangesAsync = function(isShowLoadPanel) {
        var _this4 = this;
        var deferred = new _deferred.Deferred;
        var validation = this.form.dxForm.validate();
        isShowLoadPanel && this._showLoadPanel();
        (0, _deferred.when)(validation && validation.complete || validation).done((function(validation) {
            if (validation && !validation.isValid) {
                (0, _loading.hide)();
                deferred.resolve(false);
                return
            }
            var adapter = _this4._createAppointmentAdapter(_this4.form.formData);
            var appointment = adapter.clone({
                pathTimeZone: "fromAppointment"
            }).source();
            delete appointment.repeat;
            switch (_this4.state.action) {
                case ACTION_TO_APPOINTMENT.CREATE:
                    _this4.scheduler.addAppointment(appointment).done(deferred.resolve);
                    break;
                case ACTION_TO_APPOINTMENT.UPDATE:
                    _this4.scheduler.updateAppointment(_this4.state.appointment.data, appointment).done(deferred.resolve);
                    break;
                case ACTION_TO_APPOINTMENT.EXCLUDE_FROM_SERIES:
                    _this4.scheduler.updateAppointment(_this4.state.excludeInfo.sourceAppointment, _this4.state.excludeInfo.updatedAppointment);
                    _this4.scheduler.addAppointment(appointment).done(deferred.resolve)
            }
            deferred.done((function() {
                (0, _loading.hide)();
                _this4.state.lastEditData = appointment
            }))
        }));
        return deferred.promise()
    };
    _proto._doneButtonClickHandler = function(e) {
        e.cancel = true;
        this.saveEditDataAsync()
    };
    _proto.saveEditDataAsync = function() {
        var _this5 = this;
        var deferred = new _deferred.Deferred;
        if (this._tryLockSaveChanges()) {
            (0, _deferred.when)(this.saveChangesAsync(true)).done((function() {
                if (_this5.state.lastEditData) {
                    var adapter = _this5._createAppointmentAdapter(_this5.state.lastEditData);
                    var startDate = adapter.startDate,
                        endDate = adapter.endDate,
                        allDay = adapter.allDay;
                    var startTime = startDate.getTime();
                    var endTime = endDate.getTime();
                    var inAllDayRow = allDay || endTime - startTime >= DAY_IN_MS;
                    var resources = {};
                    var dataAccessors = _this5.scheduler.getDataAccessors();
                    var resourceList = _this5.scheduler.getResources();
                    modifyResourceFields(_this5.state.lastEditData, dataAccessors, resourceList, resources);
                    _this5.scheduler.updateScrollPosition(startDate, resources, inAllDayRow);
                    _this5.state.lastEditData = null
                }
                _this5._unlockSaveChanges();
                deferred.resolve()
            }))
        }
        return deferred.promise()
    };
    _proto._showLoadPanel = function() {
        var container = this.popup.$overlayContent();
        (0, _loading.show)({
            container: container,
            position: {
                of: container
            },
            copyRootClassesToWrapper: true,
            _ignoreCopyRootClassesToWrapperDeprecation: true
        })
    };
    _proto._tryLockSaveChanges = function() {
        if (false === this.state.saveChangesLocker) {
            this.state.saveChangesLocker = true;
            return true
        }
        return false
    };
    _proto._unlockSaveChanges = function() {
        this.state.saveChangesLocker = false
    };
    _createClass(AppointmentPopup, [{
        key: "visible",
        get: function() {
            return this.popup ? this.popup.option("visible") : false
        }
    }]);
    return AppointmentPopup
}();
exports.AppointmentPopup = AppointmentPopup;
