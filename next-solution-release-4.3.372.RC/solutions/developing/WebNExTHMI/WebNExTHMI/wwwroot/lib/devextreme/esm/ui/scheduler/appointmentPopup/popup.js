/**
 * DevExtreme (esm/ui/scheduler/appointmentPopup/popup.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    getWidth
} from "../../../core/utils/size";
import devices from "../../../core/devices";
import $ from "../../../core/renderer";
import dateUtils from "../../../core/utils/date";
import {
    Deferred,
    when
} from "../../../core/utils/deferred";
import {
    getWindow,
    hasWindow
} from "../../../core/utils/window";
import {
    triggerResizeEvent
} from "../../../events/visibility_change";
import messageLocalization from "../../../localization/message";
import Popup from "../../popup";
import {
    hide as hideLoading,
    show as showLoading
} from "../loading";
import {
    createAppointmentAdapter
} from "../appointmentAdapter";
import {
    each
} from "../../../core/utils/iterator";
import {
    isResourceMultiple
} from "../resources/utils";
import {
    wrapToArray
} from "../../../core/utils/array";
var toMs = dateUtils.dateToMilliseconds;
var APPOINTMENT_POPUP_CLASS = "dx-scheduler-appointment-popup";
var isMobile = () => "desktop" !== devices.current().deviceType;
var isIOSPlatform = () => "ios" === devices.current().platform;
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
        device: () => devices.current().android,
        options: {
            showTitle: false
        }
    }]
};
var createDoneButtonConfig = () => ({
    shortcut: "done",
    options: {
        text: messageLocalization.format("Done")
    },
    location: TOOLBAR_LOCATION.AFTER
});
var createCancelButtonConfig = () => ({
    shortcut: "cancel",
    location: isIOSPlatform() ? TOOLBAR_LOCATION.BEFORE : TOOLBAR_LOCATION.AFTER
});
var modifyResourceFields = (rawAppointment, dataAccessors, resources, returnedObject) => {
    each(dataAccessors.resources.getter, fieldName => {
        var value = dataAccessors.resources.getter[fieldName](rawAppointment);
        var isMultiple = isResourceMultiple(resources, fieldName);
        returnedObject[fieldName] = isMultiple ? wrapToArray(value) : value
    })
};
export var ACTION_TO_APPOINTMENT = {
    CREATE: 0,
    UPDATE: 1,
    EXCLUDE_FROM_SERIES: 2
};
export class AppointmentPopup {
    constructor(scheduler, form) {
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
    get visible() {
        return this.popup ? this.popup.option("visible") : false
    }
    show(appointment, config) {
        this.state.appointment.data = appointment;
        this.state.action = config.action;
        this.state.excludeInfo = config.excludeInfo;
        if (!this.popup) {
            var popupConfig = this._createPopupConfig();
            this.popup = this._createPopup(popupConfig)
        }
        this.popup.option("toolbarItems", this._createPopupToolbarItems(config.isToolbarVisible));
        this.popup.show()
    }
    hide() {
        this.popup.hide()
    }
    dispose() {
        var _this$popup;
        null === (_this$popup = this.popup) || void 0 === _this$popup ? void 0 : _this$popup.$element().remove()
    }
    _createPopup(options) {
        var popupElement = $("<div>").addClass(APPOINTMENT_POPUP_CLASS).appendTo(this.scheduler.getElement());
        return this.scheduler.createComponent(popupElement, Popup, options)
    }
    _createPopupConfig() {
        return _extends({}, POPUP_CONFIG, {
            onHiding: () => this.scheduler.focus(),
            contentTemplate: () => this._createPopupContent(),
            onShowing: e => this._onShowing(e),
            copyRootClassesToWrapper: true,
            _ignoreCopyRootClassesToWrapperDeprecation: true
        })
    }
    _onShowing(e) {
        this._updateForm();
        var arg = {
            form: this.form.dxForm,
            popup: this.popup,
            appointmentData: this.state.appointment.data,
            cancel: false
        };
        this.scheduler.getAppointmentFormOpening()(arg);
        this.scheduler.processActionResult(arg, canceled => {
            if (canceled) {
                e.cancel = true
            } else {
                this.updatePopupFullScreenMode()
            }
        })
    }
    _createPopupContent() {
        this._createForm();
        return this.form.dxForm.$element()
    }
    _createFormData(rawAppointment) {
        var appointment = this._createAppointmentAdapter(rawAppointment);
        var dataAccessors = this.scheduler.getDataAccessors();
        var resources = this.scheduler.getResources();
        var result = _extends({}, rawAppointment, {
            repeat: !!appointment.recurrenceRule
        });
        modifyResourceFields(rawAppointment, dataAccessors, resources, result);
        return result
    }
    _createForm() {
        var rawAppointment = this.state.appointment.data;
        var formData = this._createFormData(rawAppointment);
        this.form.create(this.triggerResize.bind(this), this.changeSize.bind(this), formData)
    }
    _isReadOnly(rawAppointment) {
        var appointment = this._createAppointmentAdapter(rawAppointment);
        if (rawAppointment && appointment.disabled) {
            return true
        }
        if (this.state.action === ACTION_TO_APPOINTMENT.CREATE) {
            return false
        }
        return !this.scheduler.getEditingConfig().allowUpdating
    }
    _createAppointmentAdapter(rawAppointment) {
        return createAppointmentAdapter(rawAppointment, this.scheduler.getDataAccessors(), this.scheduler.getTimeZoneCalculator())
    }
    _updateForm() {
        var {
            data: data
        } = this.state.appointment;
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
    }
    _isPopupFullScreenNeeded() {
        var width = this._tryGetWindowWidth();
        if (width) {
            return isMobile() ? width < POPUP_WIDTH.MOBILE.FULLSCREEN : width < POPUP_WIDTH.FULLSCREEN
        }
        return false
    }
    _tryGetWindowWidth() {
        if (hasWindow()) {
            var window = getWindow();
            return getWidth(window)
        }
    }
    triggerResize() {
        if (this.popup) {
            triggerResizeEvent(this.popup.$element())
        }
    }
    _getMaxWidth(isRecurrence) {
        if (isMobile()) {
            return POPUP_WIDTH.MOBILE.DEFAULT
        }
        return isRecurrence ? POPUP_WIDTH.RECURRENCE : POPUP_WIDTH.DEFAULT
    }
    changeSize(isRecurrence) {
        if (this.popup) {
            var fullScreen = this._isPopupFullScreenNeeded();
            this.popup.option({
                fullScreen: fullScreen,
                maxWidth: fullScreen ? "100%" : this._getMaxWidth(isRecurrence)
            })
        }
    }
    updatePopupFullScreenMode() {
        if (this.form.dxForm) {
            var formData = this.form.formData;
            var isRecurrence = formData[this.scheduler.getDataAccessors().expr.recurrenceRuleExpr];
            if (this.visible) {
                this.changeSize(isRecurrence)
            }
        }
    }
    _createPopupToolbarItems(isVisible) {
        var result = [];
        if (isVisible) {
            result.push(_extends({}, createDoneButtonConfig(), {
                onClick: e => this._doneButtonClickHandler(e)
            }))
        }
        result.push(createCancelButtonConfig());
        return result
    }
    saveChangesAsync(isShowLoadPanel) {
        var deferred = new Deferred;
        var validation = this.form.dxForm.validate();
        isShowLoadPanel && this._showLoadPanel();
        when(validation && validation.complete || validation).done(validation => {
            if (validation && !validation.isValid) {
                hideLoading();
                deferred.resolve(false);
                return
            }
            var adapter = this._createAppointmentAdapter(this.form.formData);
            var appointment = adapter.clone({
                pathTimeZone: "fromAppointment"
            }).source();
            delete appointment.repeat;
            switch (this.state.action) {
                case ACTION_TO_APPOINTMENT.CREATE:
                    this.scheduler.addAppointment(appointment).done(deferred.resolve);
                    break;
                case ACTION_TO_APPOINTMENT.UPDATE:
                    this.scheduler.updateAppointment(this.state.appointment.data, appointment).done(deferred.resolve);
                    break;
                case ACTION_TO_APPOINTMENT.EXCLUDE_FROM_SERIES:
                    this.scheduler.updateAppointment(this.state.excludeInfo.sourceAppointment, this.state.excludeInfo.updatedAppointment);
                    this.scheduler.addAppointment(appointment).done(deferred.resolve)
            }
            deferred.done(() => {
                hideLoading();
                this.state.lastEditData = appointment
            })
        });
        return deferred.promise()
    }
    _doneButtonClickHandler(e) {
        e.cancel = true;
        this.saveEditDataAsync()
    }
    saveEditDataAsync() {
        var deferred = new Deferred;
        if (this._tryLockSaveChanges()) {
            when(this.saveChangesAsync(true)).done(() => {
                if (this.state.lastEditData) {
                    var adapter = this._createAppointmentAdapter(this.state.lastEditData);
                    var {
                        startDate: startDate,
                        endDate: endDate,
                        allDay: allDay
                    } = adapter;
                    var startTime = startDate.getTime();
                    var endTime = endDate.getTime();
                    var inAllDayRow = allDay || endTime - startTime >= DAY_IN_MS;
                    var resources = {};
                    var dataAccessors = this.scheduler.getDataAccessors();
                    var resourceList = this.scheduler.getResources();
                    modifyResourceFields(this.state.lastEditData, dataAccessors, resourceList, resources);
                    this.scheduler.updateScrollPosition(startDate, resources, inAllDayRow);
                    this.state.lastEditData = null
                }
                this._unlockSaveChanges();
                deferred.resolve()
            })
        }
        return deferred.promise()
    }
    _showLoadPanel() {
        var container = this.popup.$overlayContent();
        showLoading({
            container: container,
            position: {
                of: container
            },
            copyRootClassesToWrapper: true,
            _ignoreCopyRootClassesToWrapperDeprecation: true
        })
    }
    _tryLockSaveChanges() {
        if (false === this.state.saveChangesLocker) {
            this.state.saveChangesLocker = true;
            return true
        }
        return false
    }
    _unlockSaveChanges() {
        this.state.saveChangesLocker = false
    }
}
