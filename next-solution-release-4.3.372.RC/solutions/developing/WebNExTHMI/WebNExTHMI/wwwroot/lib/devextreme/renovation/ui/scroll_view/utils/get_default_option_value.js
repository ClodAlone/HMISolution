/**
 * DevExtreme (renovation/ui/scroll_view/utils/get_default_option_value.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getDefaultBounceEnabled = getDefaultBounceEnabled;
exports.getDefaultNativeRefreshStrategy = getDefaultNativeRefreshStrategy;
exports.getDefaultUseNative = getDefaultUseNative;
exports.getDefaultUseSimulatedScrollbar = getDefaultUseSimulatedScrollbar;
exports.isDesktop = isDesktop;
var _devices = _interopRequireDefault(require("../../../../core/devices"));
var _support = require("../../../../core/utils/support");
var _browser = _interopRequireDefault(require("../../../../core/utils/browser"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function isDesktop() {
    return !_devices.default.isSimulator() && "desktop" === _devices.default.real().deviceType && "generic" === _devices.default.current().platform
}

function getDefaultUseSimulatedScrollbar() {
    return !!_support.nativeScrolling && "android" === _devices.default.real().platform && !_browser.default.mozilla
}

function getDefaultBounceEnabled() {
    return !isDesktop()
}

function getDefaultUseNative() {
    return !!_support.nativeScrolling
}

function getDefaultNativeRefreshStrategy() {
    return "android" === _devices.default.real().platform ? "swipeDown" : "pullDown"
}
