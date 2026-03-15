/**
 * DevExtreme (esm/renovation/ui/form/form.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["screenByWidth", "scrollingEnabled", "useNativeScrolling"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    FormProps
} from "./form_props";
import {
    combineClasses
} from "../../utils/combine_classes";
import {
    Widget
} from "../common/widget";
import {
    LayoutManager
} from "./layout_manager";
import {
    Scrollable
} from "../scroll_view/scrollable";
export var viewFunction = viewModel => {
    var aria = {
        role: "form"
    };
    var cssClasses = combineClasses({
        "dx-form": true
    });
    var {
        props: {
            scrollingEnabled: scrollingEnabled,
            useNativeScrolling: useNativeScrolling
        },
        restAttributes: restAttributes
    } = viewModel;
    var rootLayoutManager = createComponentVNode(2, LayoutManager, {
        screenByWidth: viewModel.props.screenByWidth
    });
    return scrollingEnabled ? createComponentVNode(2, Scrollable, {
        aria: aria,
        classes: cssClasses,
        useNative: !!useNativeScrolling,
        useSimulatedScrollbar: !useNativeScrolling,
        useKeyboard: false,
        direction: "both",
        bounceEnabled: false,
        children: rootLayoutManager
    }) : normalizeProps(createComponentVNode(2, Widget, _extends({
        aria: aria,
        classes: cssClasses
    }, restAttributes, {
        children: rootLayoutManager
    })))
};
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
export class Form extends InfernoWrapperComponent {
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
Form.defaultProps = FormProps;
