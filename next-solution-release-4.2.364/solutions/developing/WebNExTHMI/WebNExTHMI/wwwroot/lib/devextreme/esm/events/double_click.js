/**
 * DevExtreme (esm/events/double_click.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import eventsEngine from "../events/core/events_engine";
import {
    closestCommonParent
} from "../core/utils/dom";
import domAdapter from "../core/dom_adapter";
import Class from "../core/class";
import registerEvent from "./core/event_registrator";
import {
    name as clickEventName
} from "./click";
import {
    addNamespace,
    fireEvent
} from "./utils/index";
var DBLCLICK_EVENT_NAME = "dxdblclick";
var DBLCLICK_NAMESPACE = "dxDblClick";
var NAMESPACED_CLICK_EVENT = addNamespace(clickEventName, DBLCLICK_NAMESPACE);
var DBLCLICK_TIMEOUT = 300;
var DblClick = Class.inherit({
    ctor: function() {
        this._handlerCount = 0;
        this._forgetLastClick()
    },
    _forgetLastClick: function() {
        this._firstClickTarget = null;
        this._lastClickTimeStamp = -DBLCLICK_TIMEOUT
    },
    add: function() {
        if (this._handlerCount <= 0) {
            eventsEngine.on(domAdapter.getDocument(), NAMESPACED_CLICK_EVENT, this._clickHandler.bind(this))
        }
        this._handlerCount++
    },
    _clickHandler: function(e) {
        var timeStamp = e.timeStamp || Date.now();
        var timeBetweenClicks = timeStamp - this._lastClickTimeStamp;
        var isSimulated = timeBetweenClicks < 0;
        var isDouble = !isSimulated && timeBetweenClicks < DBLCLICK_TIMEOUT;
        if (isDouble) {
            fireEvent({
                type: DBLCLICK_EVENT_NAME,
                target: closestCommonParent(this._firstClickTarget, e.target),
                originalEvent: e
            });
            this._forgetLastClick()
        } else {
            this._firstClickTarget = e.target;
            this._lastClickTimeStamp = timeStamp
        }
    },
    remove: function() {
        this._handlerCount--;
        if (this._handlerCount <= 0) {
            this._forgetLastClick();
            eventsEngine.off(domAdapter.getDocument(), NAMESPACED_CLICK_EVENT)
        }
    }
});
registerEvent(DBLCLICK_EVENT_NAME, new DblClick);
export {
    DBLCLICK_EVENT_NAME as name
};
