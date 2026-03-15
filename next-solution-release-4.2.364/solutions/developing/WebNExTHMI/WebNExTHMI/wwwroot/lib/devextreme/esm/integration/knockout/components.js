/**
 * DevExtreme (esm/integration/knockout/components.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import ko from "knockout";
import {
    getImageContainer
} from "../../core/utils/icon";
if (ko) {
    ko.bindingHandlers.dxControlsDescendantBindings = {
        init: function(_, valueAccessor) {
            return {
                controlsDescendantBindings: ko.unwrap(valueAccessor())
            }
        }
    };
    ko.bindingHandlers.dxIcon = {
        init: function(element, valueAccessor) {
            var options = ko.utils.unwrapObservable(valueAccessor()) || {};
            var iconElement = getImageContainer(options);
            ko.virtualElements.emptyNode(element);
            if (iconElement) {
                ko.virtualElements.prepend(element, iconElement.get(0))
            }
        },
        update: function(element, valueAccessor) {
            var options = ko.utils.unwrapObservable(valueAccessor()) || {};
            var iconElement = getImageContainer(options);
            ko.virtualElements.emptyNode(element);
            if (iconElement) {
                ko.virtualElements.prepend(element, iconElement.get(0))
            }
        }
    };
    ko.virtualElements.allowedBindings.dxIcon = true
}
