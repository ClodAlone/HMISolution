/**
 * DevExtreme (esm/renovation/ui/box/box.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../core/component_registrator";
import BaseComponent from "../../component_wrapper/common/component";
import {
    Box as BoxComponent
} from "./box";
export default class Box extends BaseComponent {
    get _propsInfo() {
        return {
            twoWay: [],
            allowNull: [],
            elements: [],
            templates: [],
            props: ["direction", "align", "crossAlign"]
        }
    }
    get _viewComponent() {
        return BoxComponent
    }
}
registerComponent("dxBox", Box);
