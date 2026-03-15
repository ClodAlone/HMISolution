/**
 * DevExtreme (esm/ui/scheduler/appointments/rendering_strategies/strategy_agenda.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import dateUtils from "../../../../core/utils/date";
import {
    each
} from "../../../../core/utils/iterator";
import {
    merge
} from "../../../../core/utils/array";
import BaseRenderingStrategy from "./strategy.base";
import {
    ExpressionUtils
} from "../../expressionUtils";
import {
    groupAppointmentsByResources
} from "../../resources/utils";
import {
    createAppointmentAdapter
} from "../../appointmentAdapter";
import {
    replaceWrongEndDate,
    getAppointmentTakesSeveralDays
} from "../dataProvider/utils";
class AgendaRenderingStrategy extends BaseRenderingStrategy {
    get instance() {
        return this.options.instance
    }
    get agendaDuration() {
        return this.options.agendaDuration
    }
    getAppointmentMinSize() {}
    getDeltaTime() {}
    keepAppointmentSettings() {
        return true
    }
    getAppointmentGeometry(geometry) {
        return geometry
    }
    groupAppointmentByResources(appointments) {
        var groups = this.instance._getCurrentViewOption("groups");
        var config = {
            loadedResources: this.options.loadedResources,
            resources: this.options.resources,
            dataAccessors: this.dataAccessors.resources
        };
        return groupAppointmentsByResources(config, appointments, groups)
    }
    createTaskPositionMap(appointments) {
        var height;
        var appointmentsByResources;
        this.calculateRows(appointments, this.agendaDuration, this.currentDate);
        if (appointments.length) {
            height = this.instance.fire("getAgendaVerticalStepHeight");
            appointmentsByResources = this.groupAppointmentByResources(appointments);
            var groupedAppts = [];
            each(appointmentsByResources, function(i, appts) {
                var additionalAppointments = [];
                var recurrentIndexes = [];
                each(appts, function(index, appointment) {
                    var recurrenceBatch = this.instance.getAppointmentsInstance()._processRecurrenceAppointment(appointment, index);
                    var appointmentBatch = null;
                    if (!recurrenceBatch.indexes.length) {
                        appointmentBatch = {
                            parts: []
                        };
                        appointmentBatch = this.instance.getAppointmentsInstance()._processLongAppointment(appointment);
                        additionalAppointments = additionalAppointments.concat(appointmentBatch.parts)
                    }
                    additionalAppointments = additionalAppointments.concat(recurrenceBatch.parts);
                    recurrentIndexes = recurrentIndexes.concat(recurrenceBatch.indexes)
                }.bind(this));
                this.instance.getAppointmentsInstance()._reduceRecurrenceAppointments(recurrentIndexes, appts);
                this.instance.getAppointmentsInstance()._combineAppointments(appts, additionalAppointments);
                groupedAppts = groupedAppts.concat(appts)
            }.bind(this));
            Array.prototype.splice.apply(appointments, [0, appointments.length].concat(groupedAppts))
        }
        var result = [];
        var sortedIndex = 0;
        appointments.forEach(function(appt, index) {
            result.push([{
                height: height,
                width: "100%",
                sortedIndex: sortedIndex++,
                groupIndex: this._calculateGroupIndex(index, appointmentsByResources),
                agendaSettings: appt.settings
            }]);
            delete appt.settings
        }.bind(this));
        return result
    }
    _calculateGroupIndex(apptIndex, appointmentsByResources) {
        var resultInd;
        var counter = 0;
        for (var i in appointmentsByResources) {
            var countApptInGroup = appointmentsByResources[i].length;
            if (apptIndex >= counter && apptIndex < counter + countApptInGroup) {
                resultInd = Number(i);
                break
            }
            counter += countApptInGroup
        }
        return resultInd
    }
    _getDeltaWidth() {}
    _getAppointmentMaxWidth() {
        return this.cellWidth
    }
    _needVerifyItemSize() {
        return false
    }
    _getAppointmentParts() {}
    _reduceMultiWeekAppointment() {}
    calculateAppointmentHeight() {
        return 0
    }
    calculateAppointmentWidth() {
        return 0
    }
    isAppointmentGreaterThan() {}
    isAllDay() {
        return false
    }
    _sortCondition() {}
    _rowCondition() {}
    _columnCondition() {}
    _findIndexByKey() {}
    _markAppointmentAsVirtual() {}
    getDropDownAppointmentWidth() {}
    getCollectorLeftOffset() {}
    getCollectorTopOffset() {}
    replaceWrongAppointmentEndDate(rawAppointment, startDate, endDate) {
        var adapter = createAppointmentAdapter(rawAppointment, this.dataAccessors, this.timeZoneCalculator);
        replaceWrongEndDate(adapter, startDate, endDate, this.cellDuration, this.dataAccessors)
    }
    calculateRows(appointments, agendaDuration, currentDate, needClearSettings) {
        this._rows = [];
        currentDate = dateUtils.trimTime(new Date(currentDate));
        var groupedAppointments = this.groupAppointmentByResources(appointments);
        each(groupedAppointments, function(_, currentAppointments) {
            var groupResult = [];
            var appts = {
                indexes: [],
                parts: []
            };
            if (!currentAppointments.length) {
                this._rows.push([]);
                return true
            }
            each(currentAppointments, function(index, appointment) {
                var startDate = ExpressionUtils.getField(this.dataAccessors, "startDate", appointment);
                var endDate = ExpressionUtils.getField(this.dataAccessors, "endDate", appointment);
                this.replaceWrongAppointmentEndDate(appointment, startDate, endDate);
                needClearSettings && delete appointment.settings;
                var result = this.instance.getAppointmentsInstance()._processRecurrenceAppointment(appointment, index, false);
                appts.parts = appts.parts.concat(result.parts);
                appts.indexes = appts.indexes.concat(result.indexes)
            }.bind(this));
            this.instance.getAppointmentsInstance()._reduceRecurrenceAppointments(appts.indexes, currentAppointments);
            merge(currentAppointments, appts.parts);
            var appointmentCount = currentAppointments.length;
            for (var i = 0; i < agendaDuration; i++) {
                var day = new Date(currentDate);
                day.setMilliseconds(day.getMilliseconds() + 864e5 * i);
                if (void 0 === groupResult[i]) {
                    groupResult[i] = 0
                }
                for (var j = 0; j < appointmentCount; j++) {
                    var appointmentData = currentAppointments[j].settings || currentAppointments[j];
                    var adapter = createAppointmentAdapter(currentAppointments[j], this.dataAccessors, this.timeZoneCalculator);
                    var appointmentIsLong = getAppointmentTakesSeveralDays(adapter);
                    var appointmentIsRecurrence = ExpressionUtils.getField(this.dataAccessors, "recurrenceRule", currentAppointments[j]);
                    if (this.instance.fire("dayHasAppointment", day, appointmentData, true) || !appointmentIsRecurrence && appointmentIsLong && this.instance.fire("dayHasAppointment", day, currentAppointments[j], true)) {
                        groupResult[i] += 1
                    }
                }
            }
            this._rows.push(groupResult)
        }.bind(this));
        return this._rows
    }
    _iterateRow(row, obj, index) {
        for (var i = 0; i < row.length; i++) {
            obj.counter = obj.counter + row[i];
            if (obj.counter >= index) {
                obj.indexInRow = i;
                break
            }
        }
    }
    getDateByIndex(index, rows, startViewDate) {
        var obj = {
            counter: 0,
            indexInRow: 0
        };
        index++;
        for (var i = 0; i < rows.length; i++) {
            this._iterateRow(rows[i], obj, index);
            if (obj.indexInRow) {
                break
            }
        }
        return new Date(new Date(startViewDate).setDate(startViewDate.getDate() + obj.indexInRow))
    }
    getAppointmentDataCalculator() {
        return function($appointment, originalStartDate) {
            var apptIndex = $appointment.index();
            var startViewDate = this.instance.getStartViewDate();
            var calculatedStartDate = this.getDateByIndex(apptIndex, this._rows, startViewDate);
            var wrappedOriginalStartDate = new Date(originalStartDate);
            return {
                startDate: new Date(calculatedStartDate.setHours(wrappedOriginalStartDate.getHours(), wrappedOriginalStartDate.getMinutes(), wrappedOriginalStartDate.getSeconds(), wrappedOriginalStartDate.getMilliseconds()))
            }
        }.bind(this)
    }
}
export default AgendaRenderingStrategy;
