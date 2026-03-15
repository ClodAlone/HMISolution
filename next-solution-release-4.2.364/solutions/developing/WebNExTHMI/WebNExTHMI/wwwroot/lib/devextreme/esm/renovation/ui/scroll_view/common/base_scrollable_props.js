/**
 * DevExtreme (esm/renovation/ui/scroll_view/common/base_scrollable_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    touch
} from "../../../../core/utils/support";
import {
    getDefaultBounceEnabled,
    isDesktop
} from "../utils/get_default_option_value";
import {
    current,
    isMaterial
} from "../../../../ui/themes";
import messageLocalization from "../../../../localization/message";
export var BaseScrollableProps = {
    get aria() {
        return {}
    },
    addWidgetClass: false,
    disabled: false,
    visible: true,
    rtlEnabled: false,
    classes: "",
    direction: "vertical",
    get bounceEnabled() {
        return getDefaultBounceEnabled()
    },
    get scrollByContent() {
        return isDesktop() ? touch : true
    },
    pullDownEnabled: false,
    reachBottomEnabled: false,
    forceGeneratePockets: false,
    needScrollViewContentWrapper: false,
    needScrollViewLoadPanel: false,
    needRenderScrollbars: true,
    refreshStrategy: "simulated",
    get pullingDownText() {
        return isMaterial(current()) ? "" : messageLocalization.format("dxScrollView-pullingDownText")
    },
    get pulledDownText() {
        return isMaterial(current()) ? "" : messageLocalization.format("dxScrollView-pulledDownText")
    },
    get refreshingText() {
        return isMaterial(current()) ? "" : messageLocalization.format("dxScrollView-refreshingText")
    },
    get reachBottomText() {
        return isMaterial(current()) ? "" : messageLocalization.format("dxScrollView-reachBottomText")
    }
};
