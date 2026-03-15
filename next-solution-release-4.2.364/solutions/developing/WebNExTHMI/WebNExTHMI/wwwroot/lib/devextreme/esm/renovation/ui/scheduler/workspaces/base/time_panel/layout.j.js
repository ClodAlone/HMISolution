/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/time_panel/layout.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../../../../core/component_registrator";
import {
    TimePanel
} from "../../../../../component_wrapper/scheduler/time_panel";
import {
    TimePanelTableLayout as TimePanelTableLayoutComponent
} from "./layout";
export default class TimePanelTableLayout extends TimePanel {
    get _propsInfo() {
        return {
            twoWay: [],
            allowNull: [],
            elements: [],
            templates: ["timeCellTemplate"],
            props: ["groupOrientation", "timePanelData", "timeCellTemplate"]
        }
    }
    get _viewComponent() {
        return TimePanelTableLayoutComponent
    }
}
registerComponent("dxTimePanelTableLayout", TimePanelTableLayout);
