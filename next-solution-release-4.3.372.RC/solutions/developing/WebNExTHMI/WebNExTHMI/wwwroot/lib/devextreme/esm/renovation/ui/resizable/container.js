/**
 * DevExtreme (esm/renovation/ui/resizable/container.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["children", "disabled", "handles", "height", "mainRef", "onResize", "onResizeEnd", "onResizeStart", "rtlEnabled", "width"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent,
    normalizeStyles
} from "@devextreme/runtime/inferno";
import {
    ResizableHandle
} from "./handle";
import {
    combineClasses
} from "../../utils/combine_classes";
import {
    triggerResizeEvent
} from "../../../events/visibility_change";
var getCssClasses = (disabled, rtlEnabled, isResizing) => combineClasses({
    "dx-resizable": true,
    "dx-state-disabled": disabled,
    "dx-rtl": rtlEnabled,
    "dx-resizable-resizing": isResizing
});
export var viewFunction = viewModel => {
    var {
        cssClasses: cssClasses,
        handles: handles,
        mainContainerRef: mainContainerRef,
        onHandleResize: onHandleResize,
        onHandleResizeEnd: onHandleResizeEnd,
        onHandleResizeStart: onHandleResizeStart,
        props: props,
        restAttributes: restAttributes,
        styles: styles
    } = viewModel;
    var {
        children: children,
        disabled: disabled
    } = props;
    return normalizeProps(createVNode(1, "div", cssClasses, [children, handles.map(handleType => createComponentVNode(2, ResizableHandle, {
        onResizeStart: event => onHandleResizeStart(event, handleType),
        onResize: event => onHandleResize(event, handleType),
        onResizeEnd: event => onHandleResizeEnd(event, handleType),
        disabled: disabled,
        direction: handleType
    }, handleType))], 0, _extends({
        style: normalizeStyles(styles)
    }, restAttributes), null, mainContainerRef))
};
export var ResizableContainerProps = {
    get handles() {
        return []
    },
    get children() {
        return []
    },
    rtlEnabled: false,
    disabled: false
};
import {
    convertRulesToOptions
} from "../../../core/options/utils";
import {
    createRef as infernoCreateRef
} from "inferno";
export class ResizableContainer extends InfernoComponent {
    constructor(props) {
        super(props);
        this.startX = Number.NaN;
        this.startY = Number.NaN;
        this.mainContainerRef = infernoCreateRef();
        this.__getterCache = {};
        this.state = {
            isResizing: false
        };
        this.forwardRefInitEffect = this.forwardRefInitEffect.bind(this);
        this.onHandleResizeStart = this.onHandleResizeStart.bind(this);
        this.onHandleResize = this.onHandleResize.bind(this);
        this.onHandleResizeEnd = this.onHandleResizeEnd.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.forwardRefInitEffect, [])]
    }
    forwardRefInitEffect() {
        if (this.props.mainRef) {
            this.props.mainRef.current = this.mainContainerRef.current
        }
        return
    }
    onHandleResizeStart(event, handle) {
        var _this$props$onResizeS, _this$props;
        this.setState(__state_argument => ({
            isResizing: true
        }));
        this.startX = event.clientX;
        this.startY = event.clientY;
        null === (_this$props$onResizeS = (_this$props = this.props).onResizeStart) || void 0 === _this$props$onResizeS ? void 0 : _this$props$onResizeS.call(_this$props, {
            event: event,
            handle: handle
        });
        event.targetElements = [];
        return
    }
    onHandleResize(event, handle) {
        var {
            onResize: onResize
        } = this.props;
        null === onResize || void 0 === onResize ? void 0 : onResize({
            event: event,
            handle: handle,
            delta: {
                x: event.clientX - this.startX,
                y: event.clientY - this.startY
            }
        });
        triggerResizeEvent(this.mainContainerRef.current);
        return
    }
    onHandleResizeEnd(event, handle) {
        var _this$props$onResizeE, _this$props2;
        this.setState(__state_argument => ({
            isResizing: false
        }));
        this.startX = Number.NaN;
        this.startY = Number.NaN;
        null === (_this$props$onResizeE = (_this$props2 = this.props).onResizeEnd) || void 0 === _this$props$onResizeE ? void 0 : _this$props$onResizeE.call(_this$props2, {
            event: event,
            handle: handle
        });
        return
    }
    get cssClasses() {
        var {
            disabled: disabled,
            rtlEnabled: rtlEnabled
        } = this.props;
        return getCssClasses(!!disabled, !!rtlEnabled, this.state.isResizing)
    }
    get styles() {
        var {
            height: height,
            width: width
        } = this.props;
        var style = this.restAttributes.style || {};
        return _extends({}, style, {
            height: height,
            width: width
        })
    }
    get handles() {
        if (void 0 !== this.__getterCache.handles) {
            return this.__getterCache.handles
        }
        return this.__getterCache.handles = (() => {
            var {
                handles: handles
            } = this.props;
            if ("string" === typeof handles) {
                handles = [handles]
            }
            var result = handles.map(handle => handle);
            if (result.includes("bottom")) {
                result.includes("right") && result.push("corner-bottom-right");
                result.includes("left") && result.push("corner-bottom-left")
            }
            if (result.includes("top")) {
                result.includes("right") && result.push("corner-top-right");
                result.includes("left") && result.push("corner-top-left")
            }
            return result
        })()
    }
    get restAttributes() {
        var _this$props3 = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props3, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        super.componentWillUpdate();
        if (this.props.handles !== nextProps.handles) {
            this.__getterCache.handles = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            isResizing: this.state.isResizing,
            mainContainerRef: this.mainContainerRef,
            onHandleResizeStart: this.onHandleResizeStart,
            onHandleResize: this.onHandleResize,
            onHandleResizeEnd: this.onHandleResizeEnd,
            cssClasses: this.cssClasses,
            styles: this.styles,
            handles: this.handles,
            restAttributes: this.restAttributes
        })
    }
}
ResizableContainer.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(ResizableContainerProps), Object.getOwnPropertyDescriptors(_extends({}, convertRulesToOptions([])))));
var __defaultOptionRules = [];
export function defaultOptions(rule) {
    __defaultOptionRules.push(rule);
    ResizableContainer.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(ResizableContainer.defaultProps), Object.getOwnPropertyDescriptors(convertRulesToOptions([])), Object.getOwnPropertyDescriptors(convertRulesToOptions(__defaultOptionRules))))
}
