/**
 * DevExtreme (esm/renovation/ui/draggable/container.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["children", "className", "data", "disabled", "onDragEnd", "onDragMove", "onDragStart"];
import {
    createVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import {
    start,
    move,
    end
} from "../../../events/drag";
import eventsEngine from "../../../events/core/events_engine";
import {
    combineClasses
} from "../../utils/combine_classes";
export var viewFunction = _ref => {
    var {
        cssClasses: cssClasses,
        props: {
            children: children
        },
        restAttributes: restAttributes,
        widgetRef: widgetRef
    } = _ref;
    return normalizeProps(createVNode(1, "div", cssClasses, children, 0, _extends({}, restAttributes), null, widgetRef))
};
export var DraggableContainerProps = {
    className: ""
};
import {
    createRef as infernoCreateRef
} from "inferno";
export class DraggableContainer extends InfernoComponent {
    constructor(props) {
        super(props);
        this.widgetRef = infernoCreateRef();
        this.state = {
            isDragging: false
        };
        this.dragEffect = this.dragEffect.bind(this);
        this.dragStartHandler = this.dragStartHandler.bind(this);
        this.dragMoveHandler = this.dragMoveHandler.bind(this);
        this.dragEndHandler = this.dragEndHandler.bind(this);
        this.getEventArgs = this.getEventArgs.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.dragEffect, [this.props.disabled, this.props.data, this.props.onDragStart, this.props.onDragMove, this.props.onDragEnd])]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.disabled, this.props.data, this.props.onDragStart, this.props.onDragMove, this.props.onDragEnd])
    }
    dragEffect() {
        if (this.props.disabled) {
            return
        }
        eventsEngine.on(this.widgetRef.current, start, this.dragStartHandler);
        eventsEngine.on(this.widgetRef.current, move, this.dragMoveHandler);
        eventsEngine.on(this.widgetRef.current, end, this.dragEndHandler);
        return () => {
            eventsEngine.off(this.widgetRef.current, start, this.dragStartHandler);
            eventsEngine.off(this.widgetRef.current, move, this.dragMoveHandler);
            eventsEngine.off(this.widgetRef.current, end, this.dragEndHandler)
        }
    }
    get cssClasses() {
        var {
            className: className,
            disabled: disabled
        } = this.props;
        var classesMap = {
            [className]: !!className,
            "dx-draggable": true,
            "dx-draggable-dragging": this.state.isDragging,
            "dx-state-disabled": !!disabled
        };
        return combineClasses(classesMap)
    }
    dragStartHandler(event) {
        this.setState(__state_argument => ({
            isDragging: true
        }));
        var dragStartArgs = this.getEventArgs(event);
        var {
            onDragStart: onDragStart
        } = this.props;
        null === onDragStart || void 0 === onDragStart ? void 0 : onDragStart(dragStartArgs)
    }
    dragMoveHandler(event) {
        var dragMoveArgs = this.getEventArgs(event);
        var {
            onDragMove: onDragMove
        } = this.props;
        null === onDragMove || void 0 === onDragMove ? void 0 : onDragMove(dragMoveArgs)
    }
    dragEndHandler(event) {
        this.setState(__state_argument => ({
            isDragging: false
        }));
        var dragEndArgs = this.getEventArgs(event);
        var {
            onDragEnd: onDragEnd
        } = this.props;
        null === onDragEnd || void 0 === onDragEnd ? void 0 : onDragEnd(dragEndArgs)
    }
    getEventArgs(e) {
        return {
            event: e,
            data: this.props.data,
            itemElement: this.widgetRef.current
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
            isDragging: this.state.isDragging,
            widgetRef: this.widgetRef,
            cssClasses: this.cssClasses,
            dragStartHandler: this.dragStartHandler,
            dragMoveHandler: this.dragMoveHandler,
            dragEndHandler: this.dragEndHandler,
            getEventArgs: this.getEventArgs,
            restAttributes: this.restAttributes
        })
    }
}
DraggableContainer.defaultProps = DraggableContainerProps;
