/**
 * DevExtreme (esm/renovation/ui/responsive_box/responsive_box.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../core/component_registrator";
import BaseComponent from "../../component_wrapper/common/component";
import {
    ResponsiveBox as ResponsiveBoxComponent
} from "./responsive_box";
export default class ResponsiveBox extends BaseComponent {
    get _propsInfo() {
        return {
            twoWay: [],
            allowNull: [],
            elements: [],
            templates: [],
            props: ["screenByWidth"]
        }
    }
    get _viewComponent() {
        return ResponsiveBoxComponent
    }
}
registerComponent("dxResponsiveBox", ResponsiveBox);
