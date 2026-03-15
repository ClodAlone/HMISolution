/**
 * DevExtreme (esm/renovation/ui/responsive_box/responsive_box.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["screenByWidth"];
import {
    createComponentVNode
} from "inferno";
import {
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    Widget
} from "../common/widget";
import {
    ResponsiveBoxProps
} from "./responsive_box_props";
import {
    combineClasses
} from "../../utils/combine_classes";
import {
    Box
} from "../box/box";
import {
    hasWindow
} from "../../../core/utils/window";
import domAdapter from "../../../core/dom_adapter";
import {
    convertToScreenSizeQualifier
} from "./screen_utils";
var HD_SCREEN_WIDTH = 1920;
var RESPONSIVE_BOX_CLASS = "dx-responsivebox";
var SCREEN_SIZE_CLASS_PREFIX = "".concat(RESPONSIVE_BOX_CLASS, "-screen-");
export var viewFunction = viewModel => {
    var screenSizeQualifier = (screenWidth = hasWindow() ? domAdapter.getDocumentElement().clientWidth : HD_SCREEN_WIDTH, screenSizeFunc = null !== (_viewModel$props$scre = viewModel.props.screenByWidth) && void 0 !== _viewModel$props$scre ? _viewModel$props$scre : convertToScreenSizeQualifier, screenSizeFunc(screenWidth));
    var _viewModel$props$scre, screenWidth, screenSizeFunc;
    var cssClasses = combineClasses({
        [RESPONSIVE_BOX_CLASS]: true,
        [SCREEN_SIZE_CLASS_PREFIX + screenSizeQualifier]: true
    });
    return createComponentVNode(2, Widget, {
        classes: cssClasses,
        children: createComponentVNode(2, Box)
    })
};
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
export class ResponsiveBox extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    createEffects() {
        return [createReRenderEffect()]
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
            restAttributes: this.restAttributes
        })
    }
}
ResponsiveBox.defaultProps = ResponsiveBoxProps;
