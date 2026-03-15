/**
 * DevExtreme (esm/core/resize_observer.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    noop
} from "./utils/common";
import {
    getWindow,
    hasWindow
} from "./utils/window";
var window = getWindow();
var ResizeObserverMock = {
    observe: noop,
    unobserve: noop,
    disconnect: noop
};
class ResizeObserverSingleton {
    constructor() {
        if (!hasWindow() || !window.ResizeObserver) {
            return ResizeObserverMock
        }
        this._callbacksMap = new Map;
        this._observer = new window.ResizeObserver(entries => {
            entries.forEach(entry => {
                var _this$_callbacksMap$g;
                null === (_this$_callbacksMap$g = this._callbacksMap.get(entry.target)) || void 0 === _this$_callbacksMap$g ? void 0 : _this$_callbacksMap$g(entry)
            })
        })
    }
    observe(element, callback) {
        this._callbacksMap.set(element, callback);
        this._observer.observe(element)
    }
    unobserve(element) {
        this._callbacksMap.delete(element);
        this._observer.unobserve(element)
    }
    disconnect() {
        this._callbacksMap.clear();
        this._observer.disconnect()
    }
}
var resizeObserverSingleton = new ResizeObserverSingleton;
export default resizeObserverSingleton;
