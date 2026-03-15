/**
 * DevExtreme (cjs/ui/scheduler/dataStructures.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.AppointmentTooltipInfo = void 0;
var AppointmentTooltipInfo = function(appointment) {
    var targetedAppointment = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : void 0;
    var color = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : [];
    var settings = arguments.length > 3 && void 0 !== arguments[3] ? arguments[3] : [];
    this.appointment = appointment;
    this.targetedAppointment = targetedAppointment;
    this.color = color;
    this.settings = settings
};
exports.AppointmentTooltipInfo = AppointmentTooltipInfo;
