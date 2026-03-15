/**
 * DevExtreme (esm/integration/knockout/event_registrator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import ko from "knockout";
import {
    isPlainObject
} from "../../core/utils/type";
import eventRegistratorCallbacks from "../../events/core/event_registrator_callbacks";
import {
    addNamespace
} from "../../events/utils/index";
if (ko) {
    eventRegistratorCallbacks.add((function(name) {
        var koBindingEventName = addNamespace(name, name + "Binding");
        ko.bindingHandlers[name] = {
            update: function(element, valueAccessor, allBindingsAccessor, viewModel) {
                var $element = $(element);
                var unwrappedValue = ko.utils.unwrapObservable(valueAccessor());
                var eventSource = unwrappedValue.execute ? unwrappedValue.execute : unwrappedValue;
                eventsEngine.off($element, koBindingEventName);
                eventsEngine.on($element, koBindingEventName, isPlainObject(unwrappedValue) ? unwrappedValue : {}, (function(e) {
                    eventSource.call(viewModel, viewModel, e)
                }))
            }
        }
    }))
}
