/**
 * DevExtreme (esm/events/contextmenu.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import eventsEngine from "../events/core/events_engine";
import {
    touch
} from "../core/utils/support";
import devices from "../core/devices";
import Class from "../core/class";
import registerEvent from "./core/event_registrator";
import {
    addNamespace,
    fireEvent,
    isMouseEvent
} from "./utils/index";
import holdEvent from "./hold";
var CONTEXTMENU_NAMESPACE = "dxContexMenu";
var CONTEXTMENU_NAMESPACED_EVENT_NAME = addNamespace("contextmenu", CONTEXTMENU_NAMESPACE);
var HOLD_NAMESPACED_EVENT_NAME = addNamespace(holdEvent.name, CONTEXTMENU_NAMESPACE);
var CONTEXTMENU_EVENT_NAME = "dxcontextmenu";
var ContextMenu = Class.inherit({
    setup: function(element) {
        var $element = $(element);
        eventsEngine.on($element, CONTEXTMENU_NAMESPACED_EVENT_NAME, this._contextMenuHandler.bind(this));
        if (touch || devices.isSimulator()) {
            eventsEngine.on($element, HOLD_NAMESPACED_EVENT_NAME, this._holdHandler.bind(this))
        }
    },
    _holdHandler: function(e) {
        if (isMouseEvent(e) && !devices.isSimulator()) {
            return
        }
        this._fireContextMenu(e)
    },
    _contextMenuHandler: function(e) {
        this._fireContextMenu(e)
    },
    _fireContextMenu: function(e) {
        return fireEvent({
            type: CONTEXTMENU_EVENT_NAME,
            originalEvent: e
        })
    },
    teardown: function(element) {
        eventsEngine.off(element, "." + CONTEXTMENU_NAMESPACE)
    }
});
registerEvent(CONTEXTMENU_EVENT_NAME, new ContextMenu);
export var name = CONTEXTMENU_EVENT_NAME;
