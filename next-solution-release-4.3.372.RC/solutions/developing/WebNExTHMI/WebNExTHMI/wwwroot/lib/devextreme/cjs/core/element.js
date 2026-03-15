/**
 * DevExtreme (cjs/core/element.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getPublicElement = getPublicElement;
exports.setPublicElementWrapper = setPublicElementWrapper;
var strategy = function(element) {
    return element && element.get(0)
};

function getPublicElement(element) {
    return strategy(element)
}

function setPublicElementWrapper(newStrategy) {
    strategy = newStrategy
}
