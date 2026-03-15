/**
 * DevExtreme (esm/renovation/ui/overlays/overlay.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["_checkParentVisibility", "accessKey", "activeStateEnabled", "animation", "className", "closeOnOutsideClick", "container", "contentTemplate", "disabled", "focusStateEnabled", "height", "hideOnParentScroll", "hint", "hoverStateEnabled", "integrationOptions", "maxWidth", "onClick", "onKeyDown", "position", "propagateOutsideClick", "rtlEnabled", "shading", "tabIndex", "templatesRenderAsynchronously", "visible", "width"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import LegacyOverlay from "../../../ui/overlay/ui.overlay";
import {
    DomComponentWrapper
} from "../common/dom_component_wrapper";
import {
    BaseWidgetProps
} from "../common/base_props";
export var viewFunction = _ref => {
    var {
        props: props,
        restAttributes: restAttributes
    } = _ref;
    return normalizeProps(createComponentVNode(2, DomComponentWrapper, _extends({
        componentType: LegacyOverlay,
        componentProps: props,
        templateNames: []
    }, restAttributes)))
};
export var OverlayProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseWidgetProps), Object.getOwnPropertyDescriptors({
    get integrationOptions() {
        return {}
    },
    templatesRenderAsynchronously: false,
    shading: true,
    closeOnOutsideClick: false,
    hideOnParentScroll: false,
    get animation() {
        return {
            type: "pop",
            duration: 300,
            to: {
                opacity: 0,
                scale: .55
            },
            from: {
                opacity: 1,
                scale: 1
            }
        }
    },
    visible: false,
    propagateOutsideClick: true,
    _checkParentVisibility: false,
    rtlEnabled: false,
    contentTemplate: "content",
    maxWidth: null,
    isReactComponentWrapper: true
})));
export class Overlay extends BaseInfernoComponent {
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
Overlay.defaultProps = OverlayProps;
