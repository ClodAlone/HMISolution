/**
 * DevExtreme (esm/animation/frame.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    hasWindow,
    getWindow
} from "../core/utils/window";
var window = hasWindow() ? getWindow() : {};
import callOnce from "../core/utils/call_once";
var FRAME_ANIMATION_STEP_TIME = 1e3 / 60;
var request = function(callback) {
    return setTimeout(callback, FRAME_ANIMATION_STEP_TIME)
};
var cancel = function(requestID) {
    clearTimeout(requestID)
};
var setAnimationFrameMethods = callOnce((function() {
    var nativeRequest = window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.oRequestAnimationFrame || window.msRequestAnimationFrame;
    var nativeCancel = window.cancelAnimationFrame || window.webkitCancelAnimationFrame || window.mozCancelAnimationFrame || window.oCancelAnimationFrame || window.msCancelAnimationFrame;
    if (nativeRequest && nativeCancel) {
        request = nativeRequest;
        cancel = nativeCancel
    }
    if (nativeRequest && !nativeCancel) {
        var canceledRequests = {};
        request = function(callback) {
            var requestId = nativeRequest.call(window, (function() {
                try {
                    if (requestId in canceledRequests) {
                        return
                    }
                    callback.apply(this, arguments)
                } finally {
                    delete canceledRequests[requestId]
                }
            }));
            return requestId
        };
        cancel = function(requestId) {
            canceledRequests[requestId] = true
        }
    }
}));
export function requestAnimationFrame() {
    setAnimationFrameMethods();
    return request.apply(window, arguments)
}
export function cancelAnimationFrame() {
    setAnimationFrameMethods();
    cancel.apply(window, arguments)
}
