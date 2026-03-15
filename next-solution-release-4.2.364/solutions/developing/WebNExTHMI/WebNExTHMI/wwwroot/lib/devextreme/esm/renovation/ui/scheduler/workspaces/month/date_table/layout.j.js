/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/month/date_table/layout.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../../../../core/component_registrator";
import {
    DateTable
} from "../../../../../component_wrapper/scheduler/date_table";
import {
    MonthDateTableLayout as MonthDateTableLayoutComponent
} from "./layout";
export default class MonthDateTableLayout extends DateTable {
    get _propsInfo() {
        return {
            twoWay: [],
            allowNull: [],
            elements: [],
            templates: ["cellTemplate", "dataCellTemplate"],
            props: ["cellTemplate", "viewData", "groupOrientation", "leftVirtualCellWidth", "rightVirtualCellWidth", "topVirtualRowHeight", "bottomVirtualRowHeight", "addDateTableClass", "addVerticalSizesClassToRows", "width", "dataCellTemplate"]
        }
    }
    get _viewComponent() {
        return MonthDateTableLayoutComponent
    }
}
registerComponent("dxMonthDateTableLayout", MonthDateTableLayout);
