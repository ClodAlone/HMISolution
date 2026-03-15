/**
 * DevExtreme (esm/renovation/ui/overlays/load_panel.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["_checkParentVisibility", "accessKey", "activeStateEnabled", "animation", "className", "closeOnOutsideClick", "container", "contentTemplate", "delay", "disabled", "focusStateEnabled", "height", "hideOnParentScroll", "hint", "hoverStateEnabled", "integrationOptions", "maxWidth", "message", "onClick", "onKeyDown", "position", "propagateOutsideClick", "rtlEnabled", "shading", "tabIndex", "templatesRenderAsynchronously", "visible", "width"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import LegacyLoadPanel from "../../../ui/load_panel";
import {
    DomComponentWrapper
} from "../common/dom_component_wrapper";
import {
    OverlayProps
} from "./overlay";
export var viewFunction = _ref => {
    var {
        props: props,
        restAttributes: restAttributes
    } = _ref;
    return normalizeProps(createComponentVNode(2, DomComponentWrapper, _extends({
        componentType: LegacyLoadPanel,
        componentProps: props,
        templateNames: []
    }, restAttributes)))
};
export var LoadPanelProps = OverlayProps;
export class LoadPanel extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
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
LoadPanel.defaultProps = LoadPanelProps;
