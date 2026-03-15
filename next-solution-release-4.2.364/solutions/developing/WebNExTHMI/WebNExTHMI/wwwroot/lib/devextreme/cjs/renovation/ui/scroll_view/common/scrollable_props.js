/**
 * DevExtreme (cjs/renovation/ui/scroll_view/common/scrollable_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ScrollableProps = void 0;
var _simulated_strategy_props = require("./simulated_strategy_props");
var _get_default_option_value = require("../utils/get_default_option_value");

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}
var ScrollableProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_simulated_strategy_props.ScrollableSimulatedProps), Object.getOwnPropertyDescriptors(Object.defineProperties({}, {
    useNative: {
        get: function() {
            return (0, _get_default_option_value.getDefaultUseNative)()
        },
        configurable: true,
        enumerable: true
    },
    useSimulatedScrollbar: {
        get: function() {
            return (0, _get_default_option_value.getDefaultUseSimulatedScrollbar)()
        },
        configurable: true,
        enumerable: true
    },
    refreshStrategy: {
        get: function() {
            return (0, _get_default_option_value.getDefaultNativeRefreshStrategy)()
        },
        configurable: true,
        enumerable: true
    }
}))));
exports.ScrollableProps = ScrollableProps;
