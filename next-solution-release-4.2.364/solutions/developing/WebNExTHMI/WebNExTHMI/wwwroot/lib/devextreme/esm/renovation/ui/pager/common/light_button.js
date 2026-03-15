/**
 * DevExtreme (esm/renovation/ui/pager/common/light_button.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["children", "className", "label", "onClick"];
import {
    createVNode
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import {
    subscribeToClickEvent
} from "../../../utils/subscribe_to_event";
import {
    KeyboardActionContext
} from "./keyboard_action_context";
export var viewFunction = _ref => {
    var {
        props: {
            children: children,
            className: className,
            label: label
        },
        widgetRef: widgetRef
    } = _ref;
    return createVNode(1, "div", className, children, 0, {
        tabIndex: 0,
        role: "button",
        "aria-label": label
    }, null, widgetRef)
};
export var LightButtonProps = {
    className: "",
    label: ""
};
import {
    createRef as infernoCreateRef
} from "inferno";
export class LightButton extends InfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.widgetRef = infernoCreateRef();
        this.keyboardEffect = this.keyboardEffect.bind(this);
        this.subscribeToClick = this.subscribeToClick.bind(this)
    }
    get keyboardContext() {
        if ("KeyboardActionContext" in this.context) {
            return this.context.KeyboardActionContext
        }
        return KeyboardActionContext
    }
    createEffects() {
        return [new InfernoEffect(this.keyboardEffect, [this.keyboardContext, this.props.onClick]), new InfernoEffect(this.subscribeToClick, [this.props.onClick])]
    }
    updateEffects() {
        var _this$_effects$, _this$_effects$2;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.keyboardContext, this.props.onClick]);
        null === (_this$_effects$2 = this._effects[1]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props.onClick])
    }
    keyboardEffect() {
        return this.keyboardContext.registerKeyboardAction(this.widgetRef.current, this.props.onClick)
    }
    subscribeToClick() {
        return subscribeToClickEvent(this.widgetRef.current, this.props.onClick)
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
            widgetRef: this.widgetRef,
            keyboardContext: this.keyboardContext,
            restAttributes: this.restAttributes
        })
    }
}
LightButton.defaultProps = LightButtonProps;
