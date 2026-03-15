/**
 * DevExtreme (renovation/ui/scroll_view/utils/subscribe_to_resize.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.subscribeToResize = subscribeToResize;
var _resize_observer = _interopRequireDefault(require("../../../../core/resize_observer"));
var _window = require("../../../../core/utils/window");
var _frame = require("../../../../animation/frame");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function subscribeToResize(element, handler) {
    if ((0, _window.hasWindow)() && element) {
        var resizeAnimationFrameID = -1;
        _resize_observer.default.observe(element, (function(_ref) {
            var target = _ref.target;
            resizeAnimationFrameID = (0, _frame.requestAnimationFrame)((function() {
                handler(target)
            }))
        }));
        return function() {
            (0, _frame.cancelAnimationFrame)(resizeAnimationFrameID);
            _resize_observer.default.unobserve(element)
        }
    }
    return
}
