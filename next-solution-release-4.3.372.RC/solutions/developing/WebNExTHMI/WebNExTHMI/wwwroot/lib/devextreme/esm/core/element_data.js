/**
 * DevExtreme (esm/core/element_data.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import WeakMap from "./polyfills/weak_map";
import domAdapter from "./dom_adapter";
import eventsEngine from "../events/core/events_engine";
import MemorizedCallbacks from "./memorized_callbacks";
var dataMap = new WeakMap;
var strategy;
export var strategyChanging = new MemorizedCallbacks;
var beforeCleanDataFunc = function() {};
var afterCleanDataFunc = function() {};
export var setDataStrategy = function(value) {
    strategyChanging.fire(value);
    strategy = value;
    var cleanData = strategy.cleanData;
    strategy.cleanData = function(nodes) {
        beforeCleanDataFunc(nodes);
        var result = cleanData.call(this, nodes);
        afterCleanDataFunc(nodes);
        return result
    }
};
setDataStrategy({
    data: function() {
        var element = arguments[0];
        var key = arguments[1];
        var value = arguments[2];
        if (!element) {
            return
        }
        var elementData = dataMap.get(element);
        if (!elementData) {
            elementData = {};
            dataMap.set(element, elementData)
        }
        if (void 0 === key) {
            return elementData
        }
        if (2 === arguments.length) {
            return elementData[key]
        }
        elementData[key] = value;
        return value
    },
    removeData: function(element, key) {
        if (!element) {
            return
        }
        if (void 0 === key) {
            dataMap.delete(element)
        } else {
            var elementData = dataMap.get(element);
            if (elementData) {
                delete elementData[key]
            }
        }
    },
    cleanData: function(elements) {
        for (var i = 0; i < elements.length; i++) {
            eventsEngine.off(elements[i]);
            dataMap.delete(elements[i])
        }
    }
});
export function getDataStrategy() {
    return strategy
}
export function data() {
    return strategy.data.apply(this, arguments)
}
export function beforeCleanData(callback) {
    beforeCleanDataFunc = callback
}
export function afterCleanData(callback) {
    afterCleanDataFunc = callback
}
export function cleanData(nodes) {
    return strategy.cleanData.call(this, nodes)
}
export function removeData(element, key) {
    return strategy.removeData.call(this, element, key)
}
export function cleanDataRecursive(element, cleanSelf) {
    if (!domAdapter.isElementNode(element)) {
        return
    }
    var childElements = element.getElementsByTagName("*");
    strategy.cleanData(childElements);
    if (cleanSelf) {
        strategy.cleanData([element])
    }
}
