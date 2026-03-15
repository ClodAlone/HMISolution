/**
 * DevExtreme (esm/events/visibility_change.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import eventsEngine from "./core/events_engine";
var triggerVisibilityChangeEvent = function(eventName) {
    return function(element) {
        var $element = $(element || "body");
        var changeHandlers = $element.filter(".dx-visibility-change-handler").add($element.find(".dx-visibility-change-handler"));
        for (var i = 0; i < changeHandlers.length; i++) {
            eventsEngine.triggerHandler(changeHandlers[i], eventName)
        }
    }
};
export var triggerShownEvent = triggerVisibilityChangeEvent("dxshown");
export var triggerHidingEvent = triggerVisibilityChangeEvent("dxhiding");
export var triggerResizeEvent = triggerVisibilityChangeEvent("dxresize");
