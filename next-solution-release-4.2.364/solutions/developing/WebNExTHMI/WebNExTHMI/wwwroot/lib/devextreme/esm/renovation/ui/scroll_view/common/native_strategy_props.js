/**
 * DevExtreme (esm/renovation/ui/scroll_view/common/native_strategy_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    BaseScrollableProps
} from "./base_scrollable_props";
import {
    getDefaultNativeRefreshStrategy,
    getDefaultUseSimulatedScrollbar
} from "../utils/get_default_option_value";
export var ScrollableNativeProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseScrollableProps), Object.getOwnPropertyDescriptors({
    get useSimulatedScrollbar() {
        return getDefaultUseSimulatedScrollbar()
    },
    showScrollbar: "onScroll",
    get refreshStrategy() {
        return getDefaultNativeRefreshStrategy()
    }
})));
