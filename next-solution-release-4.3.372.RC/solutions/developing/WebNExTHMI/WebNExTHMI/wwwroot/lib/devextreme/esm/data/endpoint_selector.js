/**
 * DevExtreme (esm/data/endpoint_selector.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import errors from "../core/errors";
import {
    getWindow
} from "../core/utils/window";
var window = getWindow();
var IS_WINJS_ORIGIN;
var IS_LOCAL_ORIGIN;

function isLocalHostName(url) {
    return /^(localhost$|127\.)/i.test(url)
}
var EndpointSelector = function(config) {
    this.config = config;
    IS_WINJS_ORIGIN = "ms-appx:" === window.location.protocol;
    IS_LOCAL_ORIGIN = isLocalHostName(window.location.hostname)
};
EndpointSelector.prototype = {
    urlFor: function(key) {
        var bag = this.config[key];
        if (!bag) {
            throw errors.Error("E0006")
        }
        if (bag.production) {
            if (IS_WINJS_ORIGIN && !Debug.debuggerEnabled || !IS_WINJS_ORIGIN && !IS_LOCAL_ORIGIN) {
                return bag.production
            }
        }
        return bag.local
    }
};
export default EndpointSelector;
