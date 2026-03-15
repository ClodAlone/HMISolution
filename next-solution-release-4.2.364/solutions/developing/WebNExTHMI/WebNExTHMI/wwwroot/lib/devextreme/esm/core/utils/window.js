/**
 * DevExtreme (esm/core/utils/window.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import domAdapter from "../dom_adapter";
var hasWindowValue = "undefined" !== typeof window;
var hasWindow = () => hasWindowValue;
var windowObject = hasWindow() ? window : void 0;
if (!windowObject) {
    windowObject = {};
    windowObject.window = windowObject
}
var getWindow = () => windowObject;
var setWindow = (newWindowObject, hasWindow) => {
    if (void 0 === hasWindow) {
        hasWindowValue = "undefined" !== typeof window && window === newWindowObject
    } else {
        hasWindowValue = hasWindow
    }
    windowObject = newWindowObject
};
var hasProperty = prop => hasWindow() && prop in windowObject;
var defaultScreenFactorFunc = width => {
    if (width < 768) {
        return "xs"
    } else if (width < 992) {
        return "sm"
    } else if (width < 1200) {
        return "md"
    } else {
        return "lg"
    }
};
var getCurrentScreenFactor = screenFactorCallback => {
    var screenFactorFunc = screenFactorCallback || defaultScreenFactorFunc;
    var windowWidth = domAdapter.getDocumentElement().clientWidth;
    return screenFactorFunc(windowWidth)
};
var getNavigator = () => hasWindow() ? windowObject.navigator : {
    userAgent: ""
};
export {
    hasWindow,
    getWindow,
    setWindow,
    hasProperty,
    defaultScreenFactorFunc,
    getCurrentScreenFactor,
    getNavigator
};
