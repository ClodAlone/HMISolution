/**
 * DevExtreme (esm/renovation/ui/box/box.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["align", "crossAlign", "direction"];
import {
    createComponentVNode
} from "inferno";
import {
    InfernoWrapperComponent,
    normalizeStyles
} from "@devextreme/runtime/inferno";
import {
    Widget
} from "../common/widget";
import {
    BoxProps
} from "./box_props";
import {
    combineClasses
} from "../../utils/combine_classes";
export var viewFunction = viewModel => createComponentVNode(2, Widget, {
    classes: viewModel.cssClasses,
    style: normalizeStyles(viewModel.cssStyles)
});
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
export class Box extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    createEffects() {
        return [createReRenderEffect()]
    }
    get cssClasses() {
        return combineClasses({
            "dx-box dx-box-flex": true
        })
    }
    get cssStyles() {
        var tryGetFromMap = (prop, map) => prop in map ? map[prop] : prop;
        return {
            display: "flex",
            flexDirection: {
                row: "row",
                col: "column"
            } [this.props.direction],
            justifyContent: tryGetFromMap(this.props.align, {
                start: "flex-start",
                end: "flex-end",
                center: "center",
                "space-between": "space-between",
                "space-around": "space-around"
            }),
            alignItems: tryGetFromMap(this.props.crossAlign, {
                start: "flex-start",
                end: "flex-end",
                center: "center",
                stretch: "stretch"
            })
        }
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            cssClasses: this.cssClasses,
            cssStyles: this.cssStyles,
            restAttributes: this.restAttributes
        })
    }
}
Box.defaultProps = BoxProps;
