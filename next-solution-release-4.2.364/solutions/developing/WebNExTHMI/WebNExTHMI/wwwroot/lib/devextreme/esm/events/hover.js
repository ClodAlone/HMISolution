/**
 * DevExtreme (esm/events/hover.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import eventsEngine from "../events/core/events_engine";
import {
    removeData,
    data as elementData
} from "../core/element_data";
import Class from "../core/class";
import devices from "../core/devices";
import registerEvent from "./core/event_registrator";
import {
    addNamespace,
    isTouchEvent,
    fireEvent
} from "./utils/index";
import pointerEvents from "./pointer";
var HOVERSTART_NAMESPACE = "dxHoverStart";
var HOVERSTART = "dxhoverstart";
var POINTERENTER_NAMESPACED_EVENT_NAME = addNamespace(pointerEvents.enter, HOVERSTART_NAMESPACE);
var HOVEREND_NAMESPACE = "dxHoverEnd";
var HOVEREND = "dxhoverend";
var POINTERLEAVE_NAMESPACED_EVENT_NAME = addNamespace(pointerEvents.leave, HOVEREND_NAMESPACE);
var Hover = Class.inherit({
    noBubble: true,
    ctor: function() {
        this._handlerArrayKeyPath = this._eventNamespace + "_HandlerStore"
    },
    setup: function(element) {
        elementData(element, this._handlerArrayKeyPath, {})
    },
    add: function(element, handleObj) {
        var that = this;
        var handler = function(e) {
            that._handler(e)
        };
        eventsEngine.on(element, this._originalEventName, handleObj.selector, handler);
        elementData(element, this._handlerArrayKeyPath)[handleObj.guid] = handler
    },
    _handler: function(e) {
        if (isTouchEvent(e) || devices.isSimulator()) {
            return
        }
        fireEvent({
            type: this._eventName,
            originalEvent: e,
            delegateTarget: e.delegateTarget
        })
    },
    remove: function(element, handleObj) {
        var handler = elementData(element, this._handlerArrayKeyPath)[handleObj.guid];
        eventsEngine.off(element, this._originalEventName, handleObj.selector, handler)
    },
    teardown: function(element) {
        removeData(element, this._handlerArrayKeyPath)
    }
});
var HoverStart = Hover.inherit({
    ctor: function() {
        this._eventNamespace = HOVERSTART_NAMESPACE;
        this._eventName = HOVERSTART;
        this._originalEventName = POINTERENTER_NAMESPACED_EVENT_NAME;
        this.callBase()
    },
    _handler: function(e) {
        var pointers = e.pointers || [];
        if (!pointers.length) {
            this.callBase(e)
        }
    }
});
var HoverEnd = Hover.inherit({
    ctor: function() {
        this._eventNamespace = HOVEREND_NAMESPACE;
        this._eventName = HOVEREND;
        this._originalEventName = POINTERLEAVE_NAMESPACED_EVENT_NAME;
        this.callBase()
    }
});
registerEvent(HOVERSTART, new HoverStart);
registerEvent(HOVEREND, new HoverEnd);
export {
    HOVERSTART as start, HOVEREND as end
};
