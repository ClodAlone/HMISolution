/**
 * DevExtreme (esm/renovation/ui/resizable/handle.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["direction", "disabled", "onResize", "onResizeEnd", "onResizeStart"];
import {
    createVNode
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import {
    start as dragEventStart,
    move as dragEventMove,
    end as dragEventEnd
} from "../../../events/drag";
import {
    addNamespace
} from "../../../events/utils/index";
import eventsEngine from "../../../events/core/events_engine";
var namespace = "dxResizable";
var dragStartEvent = addNamespace(dragEventStart, namespace);
var dragEvent = addNamespace(dragEventMove, namespace);
var dragEndEvent = addNamespace(dragEventEnd, namespace);
export var viewFunction = viewModel => {
    var {
        mainRef: mainRef,
        props: props
    } = viewModel;
    var {
        direction: direction
    } = props;
    return createVNode(1, "div", "dx-resizable-handle dx-resizable-handle-".concat(direction), null, 1, null, null, mainRef)
};
export var ResizableHandleProps = {
    direction: "top",
    disabled: false
};
import {
    convertRulesToOptions
} from "../../../core/options/utils";
import {
    createRef as infernoCreateRef
} from "inferno";
export class ResizableHandle extends InfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.mainRef = infernoCreateRef();
        this.dragEventsEffect = this.dragEventsEffect.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.dragEventsEffect, [this.props.disabled, this.props.onResize, this.props.onResizeEnd, this.props.onResizeStart])]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.disabled, this.props.onResize, this.props.onResizeEnd, this.props.onResizeStart])
    }
    dragEventsEffect() {
        var {
            disabled: disabled,
            onResize: onResize,
            onResizeEnd: onResizeEnd,
            onResizeStart: onResizeStart
        } = this.props;
        if (!disabled) {
            var handleEl = this.mainRef.current;
            var opts = {
                direction: "both",
                immediate: true
            };
            eventsEngine.on(handleEl, {
                [dragStartEvent]: event => {
                    eventsEngine.on(handleEl, {
                        [dragEvent]: onResize,
                        [dragEndEvent]: onResizeEnd
                    }, opts);
                    null === onResizeStart || void 0 === onResizeStart ? void 0 : onResizeStart(event)
                }
            }, opts);
            return () => eventsEngine.off(handleEl, void 0, void 0)
        }
        return
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
            mainRef: this.mainRef,
            restAttributes: this.restAttributes
        })
    }
}
ResizableHandle.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(ResizableHandleProps), Object.getOwnPropertyDescriptors(_extends({}, convertRulesToOptions([])))));
var __defaultOptionRules = [];
export function defaultOptions(rule) {
    __defaultOptionRules.push(rule);
    ResizableHandle.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(ResizableHandle.defaultProps), Object.getOwnPropertyDescriptors(convertRulesToOptions([])), Object.getOwnPropertyDescriptors(convertRulesToOptions(__defaultOptionRules))))
}
