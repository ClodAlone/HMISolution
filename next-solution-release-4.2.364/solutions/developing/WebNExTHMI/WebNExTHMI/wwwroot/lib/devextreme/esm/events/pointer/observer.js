/**
 * DevExtreme (esm/events/pointer/observer.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    each
} from "../../core/utils/iterator";
import readyCallbacks from "../../core/utils/ready_callbacks";
import domAdapter from "../../core/dom_adapter";
var addEventsListener = function(events, handler) {
    readyCallbacks.add((function() {
        events.split(" ").forEach((function(event) {
            domAdapter.listen(domAdapter.getDocument(), event, handler, true)
        }))
    }))
};
var Observer = function(eventMap, pointerEquals, onPointerAdding) {
    onPointerAdding = onPointerAdding || function() {};
    var pointers = [];
    var getPointerIndex = function(e) {
        var index = -1;
        each(pointers, (function(i, pointer) {
            if (!pointerEquals(e, pointer)) {
                return true
            }
            index = i;
            return false
        }));
        return index
    };
    var removePointer = function(e) {
        var index = getPointerIndex(e);
        if (index > -1) {
            pointers.splice(index, 1)
        }
    };
    addEventsListener(eventMap.dxpointerdown, (function(e) {
        if (-1 === getPointerIndex(e)) {
            onPointerAdding(e);
            pointers.push(e)
        }
    }));
    addEventsListener(eventMap.dxpointermove, (function(e) {
        pointers[getPointerIndex(e)] = e
    }));
    addEventsListener(eventMap.dxpointerup, removePointer);
    addEventsListener(eventMap.dxpointercancel, removePointer);
    this.pointers = function() {
        return pointers
    };
    this.reset = function() {
        pointers = []
    }
};
export default Observer;
