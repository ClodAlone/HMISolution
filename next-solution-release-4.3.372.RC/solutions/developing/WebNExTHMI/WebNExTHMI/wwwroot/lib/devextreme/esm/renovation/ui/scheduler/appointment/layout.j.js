/**
 * DevExtreme (esm/renovation/ui/scheduler/appointment/layout.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../../core/component_registrator";
import BaseComponent from "../../../component_wrapper/common/component";
import {
    AppointmentLayout as AppointmentLayoutComponent
} from "./layout";
export default class AppointmentLayout extends BaseComponent {
    get _propsInfo() {
        return {
            twoWay: [],
            allowNull: [],
            elements: [],
            templates: ["appointmentTemplate", "overflowIndicatorTemplate"],
            props: ["isAllDay", "appointments", "overflowIndicators", "appointmentTemplate", "overflowIndicatorTemplate"]
        }
    }
    get _viewComponent() {
        return AppointmentLayoutComponent
    }
}
registerComponent("dxAppointmentLayout", AppointmentLayout);
