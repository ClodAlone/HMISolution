/**
 * DevExtreme (esm/renovation/utils/subscribe_to_event.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import eventsEngine from "../../events/core/events_engine";
import * as clickEvent from "../../events/click";
import {
    addNamespace
} from "../../events/utils/index";
import scrollEvents from "../../events/gesture/emitter.gesture.scroll";
import pointerEvents from "../../events/pointer";
export function subscribeToEvent(eventName) {
    return (element, handler, eventData, namespace) => {
        var event = namespace ? addNamespace(eventName, namespace) : eventName;
        if (handler) {
            eventsEngine.on(element, event, eventData, handler);
            return () => {
                eventsEngine.off(element, event, handler)
            }
        }
        return
    }
}
export var subscribeToClickEvent = subscribeToEvent(clickEvent.name);
export var subscribeToScrollEvent = subscribeToEvent(scrollEvents.scroll);
export var subscribeToScrollInitEvent = subscribeToEvent(scrollEvents.init);
export var subscribeToDXScrollStartEvent = subscribeToEvent(scrollEvents.start);
export var subscribeToDXScrollMoveEvent = subscribeToEvent(scrollEvents.move);
export var subscribeToDXScrollEndEvent = subscribeToEvent(scrollEvents.end);
export var subscribeToDXScrollStopEvent = subscribeToEvent(scrollEvents.stop);
export var subscribeToDXScrollCancelEvent = subscribeToEvent(scrollEvents.cancel);
export var subscribeToDXPointerDownEvent = subscribeToEvent(pointerEvents.down);
export var subscribeToDXPointerUpEvent = subscribeToEvent(pointerEvents.up);
export var subscribeToMouseEnterEvent = subscribeToEvent("mouseenter");
export var subscribeToMouseLeaveEvent = subscribeToEvent("mouseleave");
export var subscribeToKeyDownEvent = subscribeToEvent("keydown");
export var subscribeToDxActiveEvent = subscribeToEvent("dxactive");
export var subscribeToDxInactiveEvent = subscribeToEvent("dxinactive");
export var subscribeToDxHoverStartEvent = subscribeToEvent("dxhoverstart");
export var subscribeToDxHoverEndEvent = subscribeToEvent("dxhoverend");
export var subscribeToDxFocusInEvent = subscribeToEvent("focusin");
export var subscribeToDxFocusOutEvent = subscribeToEvent("focusout");
