/**
 * DevExtreme (esm/ui/scheduler/appointments/render.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import {
    utils
} from "../utils";
import dxrAppointmentLayout from "../../../renovation/ui/scheduler/appointment/layout.j";
export var renderAppointments = options => {
    var {
        instance: instance,
        $dateTable: $dateTable,
        viewModel: viewModel
    } = options;
    var container = getAppointmentsContainer($dateTable);
    utils.renovation.renderComponent(instance, container, dxrAppointmentLayout, "renovatedAppointments", viewModel)
};
var getAppointmentsContainer = $dateTable => {
    var container = $(".dx-appointments-container");
    if (0 === container.length) {
        container = $("<div>").addClass("dx-appointments-container").appendTo($dateTable)
    }
    return container
};
