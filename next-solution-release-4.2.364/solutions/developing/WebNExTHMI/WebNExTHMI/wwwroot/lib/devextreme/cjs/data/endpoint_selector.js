/**
 * DevExtreme (cjs/data/endpoint_selector.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _errors = _interopRequireDefault(require("../core/errors"));
var _window = require("../core/utils/window");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var window = (0, _window.getWindow)();
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
            throw _errors.default.Error("E0006")
        }
        if (bag.production) {
            if (IS_WINJS_ORIGIN && !Debug.debuggerEnabled || !IS_WINJS_ORIGIN && !IS_LOCAL_ORIGIN) {
                return bag.production
            }
        }
        return bag.local
    }
};
var _default = EndpointSelector;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
