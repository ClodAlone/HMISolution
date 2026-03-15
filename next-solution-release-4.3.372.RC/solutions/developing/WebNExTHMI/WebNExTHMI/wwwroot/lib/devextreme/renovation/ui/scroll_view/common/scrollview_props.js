/**
 * DevExtreme (renovation/ui/scroll_view/common/scrollview_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ScrollViewProps = void 0;
var _scrollable_props = require("./scrollable_props");

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
var ScrollViewProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_scrollable_props.ScrollableProps), Object.getOwnPropertyDescriptors({
    pullDownEnabled: false,
    reachBottomEnabled: false
})));
exports.ScrollViewProps = ScrollViewProps;
