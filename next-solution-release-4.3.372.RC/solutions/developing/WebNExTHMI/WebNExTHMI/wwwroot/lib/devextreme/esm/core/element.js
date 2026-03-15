/**
 * DevExtreme (esm/core/element.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
var strategy = function(element) {
    return element && element.get(0)
};
export function getPublicElement(element) {
    return strategy(element)
}
export function setPublicElementWrapper(newStrategy) {
    strategy = newStrategy
}
