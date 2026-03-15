/**
 * DevExtreme (cjs/renovation/ui/scheduler/resources/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.createGetAppointmentColor = void 0;
var _utils = require("../../../../ui/scheduler/resources/utils");
var createGetAppointmentColor = function(resourceConfig) {
    return function(appointmentConfig) {
        return (0, _utils.getAppointmentColor)(resourceConfig, appointmentConfig)
    }
};
exports.createGetAppointmentColor = createGetAppointmentColor;
