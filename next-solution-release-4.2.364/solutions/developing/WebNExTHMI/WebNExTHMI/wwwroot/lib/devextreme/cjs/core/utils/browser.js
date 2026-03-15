/**
 * DevExtreme (cjs/core/utils/browser.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _extend = require("./extend");
var _window = require("./window");
var navigator = (0, _window.getNavigator)();
var webkitRegExp = /(webkit)[ /]([\w.]+)/;
var mozillaRegExp = /(mozilla)(?:.*? rv:([\w.]+))/;
var browserFromUA = function(ua) {
    ua = ua.toLowerCase();
    var result = {};
    var matches = webkitRegExp.exec(ua) || ua.indexOf("compatible") < 0 && mozillaRegExp.exec(ua) || [];
    var browserName = matches[1];
    var browserVersion = matches[2];
    if ("webkit" === browserName) {
        result.webkit = true;
        if (ua.indexOf("chrome") >= 0 || ua.indexOf("crios") >= 0) {
            browserName = "chrome";
            browserVersion = /(?:chrome|crios)\/(\d+\.\d+)/.exec(ua);
            browserVersion = browserVersion && browserVersion[1]
        } else if (ua.indexOf("fxios") >= 0) {
            browserName = "mozilla";
            browserVersion = /fxios\/(\d+\.\d+)/.exec(ua);
            browserVersion = browserVersion && browserVersion[1]
        } else if (ua.indexOf("safari") >= 0 && /version|phantomjs/.test(ua)) {
            browserName = "safari";
            browserVersion = /(?:version|phantomjs)\/([0-9.]+)/.exec(ua);
            browserVersion = browserVersion && browserVersion[1]
        } else {
            browserName = "unknown";
            browserVersion = /applewebkit\/([0-9.]+)/.exec(ua);
            browserVersion = browserVersion && browserVersion[1]
        }
    }
    if (browserName) {
        result[browserName] = true;
        result.version = browserVersion
    }
    return result
};
var _default = (0, _extend.extend)({
    _fromUA: browserFromUA
}, browserFromUA(navigator.userAgent));
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
