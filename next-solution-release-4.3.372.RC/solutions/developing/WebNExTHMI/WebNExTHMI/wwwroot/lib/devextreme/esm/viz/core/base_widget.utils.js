/**
 * DevExtreme (esm/viz/core/base_widget.utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    version
} from "../../core/version";
import {
    format as _stringFormat
} from "../../core/utils/string";
import warnings from "./errors_warnings";
import {
    each
} from "../../core/utils/iterator";
var ERROR_MESSAGES = warnings.ERROR_MESSAGES;
export function createEventTrigger(eventsMap, callbackGetter) {
    var triggers = {};
    each(eventsMap, (function(name, info) {
        if (info.name) {
            createEvent(name)
        }
    }));
    var changes;
    triggerEvent.change = function(name) {
        var eventInfo = eventsMap[name];
        if (eventInfo) {
            (changes = changes || {})[name] = eventInfo
        }
        return !!eventInfo
    };
    triggerEvent.applyChanges = function() {
        if (changes) {
            each(changes, (function(name, eventInfo) {
                createEvent(eventInfo.newName || name)
            }));
            changes = null
        }
    };
    triggerEvent.dispose = function() {
        eventsMap = callbackGetter = triggers = null
    };
    return triggerEvent;

    function createEvent(name) {
        var eventInfo = eventsMap[name];
        triggers[eventInfo.name] = callbackGetter(name)
    }

    function triggerEvent(name, arg, complete) {
        triggers[name](arg);
        complete && complete()
    }
}
export var createIncidentOccurred = function(widgetName, eventTrigger) {
    return function(id, args) {
        eventTrigger("incidentOccurred", {
            target: {
                id: id,
                type: "E" === id[0] ? "error" : "warning",
                args: args,
                text: _stringFormat.apply(null, [ERROR_MESSAGES[id]].concat(args || [])),
                widget: widgetName,
                version: version
            }
        })
    }
};
export function createResizeHandler(callback) {
    var timeout;
    var handler = function() {
        clearTimeout(timeout);
        timeout = setTimeout(callback, 100)
    };
    handler.dispose = function() {
        clearTimeout(timeout);
        return this
    };
    return handler
}
