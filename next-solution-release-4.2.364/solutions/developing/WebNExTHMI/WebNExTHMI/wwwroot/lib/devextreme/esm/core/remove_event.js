/**
 * DevExtreme (esm/core/remove_event.js)
 * Version: 21.2.3
 * Build date: Thu Oct 28 2021
 *
 * Copyright (c) 2012 - 2021 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "./renderer";
import {
    beforeCleanData
} from "./element_data";
import eventsEngine from "../events/core/events_engine";
import registerEvent from "../events/core/event_registrator";
export var removeEvent = "dxremove";
var eventPropName = "dxRemoveEvent";
beforeCleanData((function(elements) {
    elements = [].slice.call(elements);
    for (var i = 0; i < elements.length; i++) {
        var $element = $(elements[i]);
        if ($element.prop(eventPropName)) {
            $element[0][eventPropName] = null;
            eventsEngine.triggerHandler($element, removeEvent)
        }
    }
}));
registerEvent(removeEvent, {
    noBubble: true,
    setup: function(element) {
        $(element).prop(eventPropName, true)
    }
});
