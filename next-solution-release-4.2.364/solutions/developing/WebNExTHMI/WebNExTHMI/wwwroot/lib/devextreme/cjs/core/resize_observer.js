/**
 * DevExtreme (cjs/core/resize_observer.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _common = require("./utils/common");
var _window = require("./utils/window");
var window = (0, _window.getWindow)();
var ResizeObserverMock = {
    observe: _common.noop,
    unobserve: _common.noop,
    disconnect: _common.noop
};
var ResizeObserverSingleton = function() {
    function ResizeObserverSingleton() {
        var _this = this;
        if (!(0, _window.hasWindow)() || !window.ResizeObserver) {
            return ResizeObserverMock
        }
        this._callbacksMap = new Map;
        this._observer = new window.ResizeObserver((function(entries) {
            entries.forEach((function(entry) {
                var _this$_callbacksMap$g;
                null === (_this$_callbacksMap$g = _this._callbacksMap.get(entry.target)) || void 0 === _this$_callbacksMap$g ? void 0 : _this$_callbacksMap$g(entry)
            }))
        }))
    }
    var _proto = ResizeObserverSingleton.prototype;
    _proto.observe = function(element, callback) {
        this._callbacksMap.set(element, callback);
        this._observer.observe(element)
    };
    _proto.unobserve = function(element) {
        this._callbacksMap.delete(element);
        this._observer.unobserve(element)
    };
    _proto.disconnect = function() {
        this._callbacksMap.clear();
        this._observer.disconnect()
    };
    return ResizeObserverSingleton
}();
var resizeObserverSingleton = new ResizeObserverSingleton;
var _default = resizeObserverSingleton;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
