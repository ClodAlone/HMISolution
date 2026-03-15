/**
 * DevExtreme (esm/ui/scheduler/appointmentDragBehavior.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import Draggable from "../draggable";
import {
    extend
} from "../../core/utils/extend";
import {
    LIST_ITEM_DATA_KEY
} from "./constants";
var APPOINTMENT_ITEM_CLASS = "dx-scheduler-appointment";
export default class AppointmentDragBehavior {
    constructor(scheduler) {
        this.scheduler = scheduler;
        this.appointments = scheduler._appointments;
        this.initialPosition = {
            left: 0,
            top: 0
        };
        this.appointmentInfo = null
    }
    isAllDay(appointment) {
        return appointment.data("dxAppointmentSettings").allDay
    }
    onDragStart(e) {
        var {
            itemSettings: itemSettings,
            itemData: itemData,
            initialPosition: initialPosition
        } = e;
        this.initialPosition = initialPosition;
        this.appointmentInfo = {
            appointment: itemData,
            settings: itemSettings
        };
        this.appointments.notifyObserver("hideAppointmentTooltip")
    }
    onDragMove(e) {
        if (e.fromComponent !== e.toComponent) {
            this.appointments.notifyObserver("removeDroppableCellClass")
        }
    }
    getAppointmentElement(e) {
        var itemElement = e.event.data && e.event.data.itemElement || e.itemElement;
        return $(itemElement)
    }
    onDragEnd(e) {
        var element = this.getAppointmentElement(e);
        var rawAppointment = this.appointments._getItemData(element);
        var container = this.appointments._getAppointmentContainer(this.isAllDay(element));
        container.append(element);
        this.appointments.notifyObserver("updateAppointmentAfterDrag", {
            event: e,
            element: element,
            rawAppointment: rawAppointment,
            coordinates: this.initialPosition
        })
    }
    getItemData(appointmentElement) {
        var dataFromTooltip = $(appointmentElement).data(LIST_ITEM_DATA_KEY);
        var itemDataFromTooltip = null === dataFromTooltip || void 0 === dataFromTooltip ? void 0 : dataFromTooltip.appointment;
        var itemDataFromGrid = this.appointments._getItemData(appointmentElement);
        return itemDataFromTooltip || itemDataFromGrid
    }
    getItemSettings(appointment) {
        var itemData = $(appointment).data(LIST_ITEM_DATA_KEY);
        return itemData && itemData.settings || []
    }
    createDragStartHandler(options, appointmentDragging) {
        return e => {
            e.itemData = this.getItemData(e.itemElement);
            e.itemSettings = this.getItemSettings(e.itemElement);
            appointmentDragging.onDragStart && appointmentDragging.onDragStart(e);
            if (!e.cancel) {
                options.onDragStart(e)
            }
        }
    }
    createDragMoveHandler(options, appointmentDragging) {
        return e => {
            appointmentDragging.onDragMove && appointmentDragging.onDragMove(e);
            if (!e.cancel) {
                options.onDragMove(e)
            }
        }
    }
    createDragEndHandler(options, appointmentDragging) {
        return e => {
            this.appointmentInfo = null;
            appointmentDragging.onDragEnd && appointmentDragging.onDragEnd(e);
            if (!e.cancel) {
                options.onDragEnd(e);
                if (e.fromComponent !== e.toComponent) {
                    appointmentDragging.onRemove && appointmentDragging.onRemove(e)
                }
            }
        }
    }
    createDropHandler(appointmentDragging) {
        return e => {
            var updatedData = this.appointments.invoke("getUpdatedData", e.itemData);
            e.itemData = extend({}, e.itemData, updatedData);
            if (e.fromComponent !== e.toComponent) {
                appointmentDragging.onAdd && appointmentDragging.onAdd(e)
            }
        }
    }
    addTo(container, config) {
        var appointmentDragging = this.scheduler.option("appointmentDragging") || {};
        var options = extend({
            component: this.scheduler,
            contentTemplate: null,
            filter: ".".concat(APPOINTMENT_ITEM_CLASS),
            immediate: false,
            onDragStart: this.onDragStart.bind(this),
            onDragMove: this.onDragMove.bind(this),
            onDragEnd: this.onDragEnd.bind(this)
        }, config);
        this.appointments._createComponent(container, Draggable, extend({}, options, appointmentDragging, {
            onDragStart: this.createDragStartHandler(options, appointmentDragging),
            onDragMove: this.createDragMoveHandler(options, appointmentDragging),
            onDragEnd: this.createDragEndHandler(options, appointmentDragging),
            onDrop: this.createDropHandler(appointmentDragging)
        }))
    }
    updateDragSource(appointment, settings) {
        var {
            appointmentInfo: appointmentInfo
        } = this;
        if (appointmentInfo || appointment) {
            var currentAppointment = appointment || appointmentInfo.appointment;
            var currentSettings = settings || appointmentInfo.settings;
            this.appointments._setDragSourceAppointment(currentAppointment, currentSettings)
        }
    }
}
