/**
 * DevExtreme (esm/ui/scheduler/subscribes.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    isPlainObject
} from "../../core/utils/type";
import dateUtils from "../../core/utils/date";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import {
    AGENDA_LAST_IN_DATE_APPOINTMENT_CLASS
} from "./classes";
import {
    utils
} from "./utils";
import {
    createAppointmentAdapter
} from "./appointmentAdapter";
import {
    getFormatType,
    formatDates
} from "./appointments/textUtils";
var toMs = dateUtils.dateToMilliseconds;
var subscribes = {
    isCurrentViewAgenda: function() {
        return "agenda" === this.currentViewType
    },
    currentViewUpdated: function(currentView) {
        this.option("currentView", currentView)
    },
    currentDateUpdated: function(date) {
        this.option("currentDate", date)
    },
    getOption: function(name) {
        return this.option(name)
    },
    getWorkspaceOption: function(name) {
        return this.getWorkSpace().option(name)
    },
    isVirtualScrolling: function() {
        return this.isVirtualScrolling()
    },
    setCellDataCacheAlias: function(appointment, geometry) {
        this._workSpace.setCellDataCacheAlias(appointment, geometry)
    },
    isGroupedByDate: function() {
        return this.getWorkSpace().isGroupedByDate()
    },
    showAppointmentTooltip: function(options) {
        var targetedAppointment = this.getTargetedAppointment(options.data, options.target);
        this.showAppointmentTooltip(options.data, options.target, targetedAppointment)
    },
    hideAppointmentTooltip: function() {
        this.hideAppointmentTooltip()
    },
    showEditAppointmentPopup: function(options) {
        var targetedData = this.getTargetedAppointment(options.data, options.target);
        this.showAppointmentPopup(options.data, false, targetedData)
    },
    updateAppointmentAfterResize: function(options) {
        var info = utils.dataAccessors.getAppointmentInfo(options.$appointment);
        var exceptionDate = info.sourceAppointment.exceptionDate;
        this._checkRecurringAppointment(options.target, options.data, exceptionDate, function() {
            this._updateAppointment(options.target, options.data, (function() {
                this._appointments.moveAppointmentBack()
            }))
        }.bind(this))
    },
    getUpdatedData: function(rawAppointment) {
        return this._getUpdatedData(rawAppointment)
    },
    updateAppointmentAfterDrag: function(_ref) {
        var {
            event: event,
            element: element,
            rawAppointment: rawAppointment,
            coordinates: coordinates
        } = _ref;
        var info = utils.dataAccessors.getAppointmentInfo(element);
        var appointment = createAppointmentAdapter(rawAppointment, this._dataAccessors, this.timeZoneCalculator);
        var targetedAppointment = createAppointmentAdapter(extend({}, rawAppointment, this._getUpdatedData(rawAppointment)), this._dataAccessors, this.timeZoneCalculator);
        var targetedRawAppointment = targetedAppointment.source();
        var newCellIndex = this._workSpace.getDroppableCellIndex();
        var oldCellIndex = this._workSpace.getCellIndexByCoordinates(coordinates);
        var becomeAllDay = targetedAppointment.allDay;
        var wasAllDay = appointment.allDay;
        var movedBetweenAllDayAndSimple = this._workSpace.supportAllDayRow() && (wasAllDay && !becomeAllDay || !wasAllDay && becomeAllDay);
        if (newCellIndex !== oldCellIndex || movedBetweenAllDayAndSimple) {
            this._checkRecurringAppointment(rawAppointment, targetedRawAppointment, info.sourceAppointment.exceptionDate, function() {
                this._updateAppointment(rawAppointment, targetedRawAppointment, (function() {
                    this._appointments.moveAppointmentBack(event)
                }), event)
            }.bind(this), void 0, void 0, event)
        } else {
            this._appointments.moveAppointmentBack(event)
        }
    },
    onDeleteButtonPress: function(options) {
        var targetedData = this.getTargetedAppointment(options.data, $(options.target));
        this.checkAndDeleteAppointment(options.data, targetedData);
        this.hideAppointmentTooltip()
    },
    getTextAndFormatDate(appointmentRaw, targetedAppointmentRaw, format) {
        var appointmentAdapter = createAppointmentAdapter(appointmentRaw, this._dataAccessors, this.timeZoneCalculator);
        var targetedAdapter = createAppointmentAdapter(targetedAppointmentRaw || appointmentRaw, this._dataAccessors, this.timeZoneCalculator);
        var startDate = this.timeZoneCalculator.createDate(targetedAdapter.startDate, {
            path: "toGrid"
        });
        var endDate = this.timeZoneCalculator.createDate(targetedAdapter.endDate, {
            path: "toGrid"
        });
        var formatType = format || getFormatType(startDate, endDate, targetedAdapter.allDay, "month" !== this.currentViewType);
        return {
            text: targetedAdapter.text || appointmentAdapter.text,
            formatDate: formatDates(startDate, endDate, formatType)
        }
    },
    _createAppointmentTitle(data) {
        if (isPlainObject(data)) {
            return data.text
        }
        return String(data)
    },
    getResizableAppointmentArea: function(options) {
        var allDay = options.allDay;
        var groups = this._getCurrentViewOption("groups");
        if (groups && groups.length) {
            if (allDay || this.getLayoutManager().getRenderingStrategyInstance()._needHorizontalGroupBounds()) {
                var horizontalGroupBounds = this._workSpace.getGroupBounds(options.coordinates);
                return {
                    left: horizontalGroupBounds.left,
                    right: horizontalGroupBounds.right,
                    top: 0,
                    bottom: 0
                }
            }
            if (this.getLayoutManager().getRenderingStrategyInstance()._needVerticalGroupBounds(allDay) && this._workSpace._isVerticalGroupedWorkSpace()) {
                var verticalGroupBounds = this._workSpace.getGroupBounds(options.coordinates);
                return {
                    left: 0,
                    right: 0,
                    top: verticalGroupBounds.top,
                    bottom: verticalGroupBounds.bottom
                }
            }
        }
    },
    needRecalculateResizableArea: function() {
        return this.getWorkSpace().needRecalculateResizableArea()
    },
    getAppointmentGeometry: function(settings) {
        return this.getLayoutManager().getRenderingStrategyInstance().getAppointmentGeometry(settings)
    },
    isAllDay: function(appointmentData) {
        return this.getLayoutManager().getRenderingStrategyInstance().isAllDay(appointmentData)
    },
    getDeltaTime: function(e, initialSize, itemData) {
        return this.getLayoutManager().getRenderingStrategyInstance().getDeltaTime(e, initialSize, itemData)
    },
    getDropDownAppointmentWidth: function(isAllDay) {
        return this.getLayoutManager().getRenderingStrategyInstance().getDropDownAppointmentWidth(this._getViewCountConfig().intervalCount, isAllDay)
    },
    getDropDownAppointmentHeight: function() {
        return this.getLayoutManager().getRenderingStrategyInstance().getDropDownAppointmentHeight()
    },
    getCellWidth: function() {
        return this.getWorkSpace().getCellWidth()
    },
    getCellHeight: function() {
        return this.getWorkSpace().getCellHeight()
    },
    getMaxAppointmentCountPerCellByType: function(isAllDay) {
        return this.getRenderingStrategyInstance()._getMaxAppointmentCountPerCellByType(isAllDay)
    },
    needCorrectAppointmentDates: function() {
        return this.getRenderingStrategyInstance().needCorrectAppointmentDates()
    },
    getRenderingStrategyDirection: function() {
        return this.getRenderingStrategyInstance().getDirection()
    },
    updateAppointmentStartDate: function(options) {
        var appointment = options.appointment;
        var firstViewDate = this._workSpace.getStartViewDate();
        var startDate = new Date(options.startDate);
        var startDayHour = this._getCurrentViewOption("startDayHour");
        var updatedStartDate;
        if (this.appointmentTakesAllDay(appointment)) {
            updatedStartDate = dateUtils.normalizeDate(startDate, firstViewDate)
        } else {
            if (startDate < firstViewDate) {
                startDate = firstViewDate
            }
            updatedStartDate = dateUtils.normalizeDate(options.startDate, new Date(startDate))
        }
        return dateUtils.roundDateByStartDayHour(updatedStartDate, startDayHour)
    },
    updateAppointmentEndDate: function(options) {
        var endDate = options.endDate;
        var endDayHour = this._getCurrentViewOption("endDayHour");
        var startDayHour = this._getCurrentViewOption("startDayHour");
        var updatedEndDate = endDate;
        if (endDate.getHours() >= endDayHour) {
            updatedEndDate.setHours(endDayHour, 0, 0, 0)
        } else if (!options.isSameDate && startDayHour > 0 && 60 * endDate.getHours() + endDate.getMinutes() < 60 * startDayHour) {
            updatedEndDate = new Date(updatedEndDate.getTime() - toMs("day"));
            updatedEndDate.setHours(endDayHour, 0, 0, 0)
        }
        return updatedEndDate
    },
    renderCompactAppointments: function(options) {
        this._compactAppointmentsHelper.render(options)
    },
    clearCompactAppointments: function() {
        this._compactAppointmentsHelper.clear()
    },
    supportCompactDropDownAppointments: function() {
        return this.getLayoutManager().getRenderingStrategyInstance().supportCompactDropDownAppointments()
    },
    getGroupCount: function() {
        return this._workSpace._getGroupCount()
    },
    mapAppointmentFields: function(config) {
        var {
            itemData: itemData,
            itemElement: itemElement,
            targetedAppointment: targetedAppointment
        } = config;
        var targetedData = targetedAppointment || this.getTargetedAppointment(itemData, itemElement);
        return {
            appointmentData: config.itemData,
            appointmentElement: config.itemElement,
            targetedAppointmentData: targetedData
        }
    },
    dayHasAppointment: function(day, appointment, trimTime) {
        return this.dayHasAppointment(day, appointment, trimTime)
    },
    getLayoutManager: function() {
        return this._layoutManager
    },
    getAgendaVerticalStepHeight: function() {
        return this.getWorkSpace().getAgendaVerticalStepHeight()
    },
    getAgendaDuration: function() {
        return this._getCurrentViewOption("agendaDuration")
    },
    getStartViewDate: function() {
        return this.getStartViewDate()
    },
    getEndViewDate: function() {
        return this.getEndViewDate()
    },
    forceMaxAppointmentPerCell: function() {
        return this.forceMaxAppointmentPerCell()
    },
    onAgendaReady: function(rows) {
        var $appts = this.getAppointmentsInstance()._itemElements();
        var total = 0;
        var applyClass = function(_, count) {
            var index = count + total - 1;
            $appts.eq(index).addClass(AGENDA_LAST_IN_DATE_APPOINTMENT_CLASS);
            total += count
        };
        for (var i = 0; i < rows.length; i++) {
            each(rows[i], applyClass)
        }
    },
    getTimezone: function() {
        return this._getTimezoneOffsetByOption()
    },
    getTargetedAppointmentData: function(appointment, element) {
        return this.getTargetedAppointment(appointment, element)
    },
    getEndDayHour: function() {
        return this._workSpace.option("endDayHour") || this.option("endDayHour")
    },
    getStartDayHour: function() {
        return this._workSpace.option("startDayHour") || this.option("startDayHour")
    },
    isAdaptive: function() {
        return this.option("adaptivityEnabled")
    },
    removeDroppableCellClass: function() {
        this._workSpace.removeDroppableCellClass()
    }
};
export default subscribes;
