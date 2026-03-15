/**
 * DevExtreme (renovation/ui/scroll_view/common/base_scrollable_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.BaseScrollableProps = void 0;
var _support = require("../../../../core/utils/support");
var _get_default_option_value = require("../utils/get_default_option_value");
var _themes = require("../../../../ui/themes");
var _message = _interopRequireDefault(require("../../../../localization/message"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var BaseScrollableProps = Object.defineProperties({
    addWidgetClass: false,
    disabled: false,
    visible: true,
    rtlEnabled: false,
    classes: "",
    direction: "vertical",
    pullDownEnabled: false,
    reachBottomEnabled: false,
    forceGeneratePockets: false,
    needScrollViewContentWrapper: false,
    needScrollViewLoadPanel: false,
    needRenderScrollbars: true,
    refreshStrategy: "simulated"
}, {
    aria: {
        get: function() {
            return {}
        },
        configurable: true,
        enumerable: true
    },
    bounceEnabled: {
        get: function() {
            return (0, _get_default_option_value.getDefaultBounceEnabled)()
        },
        configurable: true,
        enumerable: true
    },
    scrollByContent: {
        get: function() {
            return (0, _get_default_option_value.isDesktop)() ? _support.touch : true
        },
        configurable: true,
        enumerable: true
    },
    pullingDownText: {
        get: function() {
            return (0, _themes.isMaterial)((0, _themes.current)()) ? "" : _message.default.format("dxScrollView-pullingDownText")
        },
        configurable: true,
        enumerable: true
    },
    pulledDownText: {
        get: function() {
            return (0, _themes.isMaterial)((0, _themes.current)()) ? "" : _message.default.format("dxScrollView-pulledDownText")
        },
        configurable: true,
        enumerable: true
    },
    refreshingText: {
        get: function() {
            return (0, _themes.isMaterial)((0, _themes.current)()) ? "" : _message.default.format("dxScrollView-refreshingText")
        },
        configurable: true,
        enumerable: true
    },
    reachBottomText: {
        get: function() {
            return (0, _themes.isMaterial)((0, _themes.current)()) ? "" : _message.default.format("dxScrollView-reachBottomText")
        },
        configurable: true,
        enumerable: true
    }
});
exports.BaseScrollableProps = BaseScrollableProps;
