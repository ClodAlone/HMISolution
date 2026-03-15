/**
 * DevExtreme (esm/renovation/ui/droppable.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["children", "className", "disabled", "onDragEnter", "onDragLeave", "onDrop"];
import {
    createVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import {
    enter,
    leave,
    drop
} from "../../events/drag";
import eventsEngine from "../../events/core/events_engine";
import {
    combineClasses
} from "../utils/combine_classes";
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
export var DroppableProps = {
    disabled: false,
    className: ""
};
import {
    createRef as infernoCreateRef
} from "inferno";
export class Droppable extends InfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.widgetRef = infernoCreateRef();
        this.dropEventsEffect = this.dropEventsEffect.bind(this);
        this.dragEnterHandler = this.dragEnterHandler.bind(this);
        this.dragLeaveHandler = this.dragLeaveHandler.bind(this);
        this.dropHandler = this.dropHandler.bind(this);
        this.getEventArgs = this.getEventArgs.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.dropEventsEffect, [this.props.disabled, this.props.onDragEnter, this.props.onDragLeave, this.props.onDrop])]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.disabled, this.props.onDragEnter, this.props.onDragLeave, this.props.onDrop])
    }
    dropEventsEffect() {
        if (this.props.disabled) {
            return
        }
        eventsEngine.on(this.widgetRef.current, enter, this.dragEnterHandler);
        eventsEngine.on(this.widgetRef.current, leave, this.dragLeaveHandler);
        eventsEngine.on(this.widgetRef.current, drop, this.dropHandler);
        return () => {
            eventsEngine.off(this.widgetRef.current, enter, this.dragEnterHandler);
            eventsEngine.off(this.widgetRef.current, leave, this.dragLeaveHandler);
            eventsEngine.off(this.widgetRef.current, drop, this.dropHandler)
        }
    }
    get cssClasses() {
        var {
            className: className,
            disabled: disabled
        } = this.props;
        var classesMap = {
            [className]: !!className,
            "dx-droppable": true,
            "dx-state-disabled": !!disabled
        };
        return combineClasses(classesMap)
    }
    dragEnterHandler(event) {
        var dragEnterArgs = this.getEventArgs(event);
        var {
            onDragEnter: onDragEnter
        } = this.props;
        null === onDragEnter || void 0 === onDragEnter ? void 0 : onDragEnter(dragEnterArgs)
    }
    dragLeaveHandler(event) {
        var dragLeaveArgs = this.getEventArgs(event);
        var {
            onDragLeave: onDragLeave
        } = this.props;
        null === onDragLeave || void 0 === onDragLeave ? void 0 : onDragLeave(dragLeaveArgs)
    }
    dropHandler(event) {
        var dropArgs = this.getEventArgs(event);
        var {
            onDrop: onDrop
        } = this.props;
        null === onDrop || void 0 === onDrop ? void 0 : onDrop(dropArgs)
    }
    getEventArgs(e) {
        return {
            event: e,
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
            widgetRef: this.widgetRef,
            cssClasses: this.cssClasses,
            dragEnterHandler: this.dragEnterHandler,
            dragLeaveHandler: this.dragLeaveHandler,
            dropHandler: this.dropHandler,
            getEventArgs: this.getEventArgs,
            restAttributes: this.restAttributes
        })
    }
}
Droppable.defaultProps = DroppableProps;
