/**
 * DevExtreme (esm/ui/scheduler/tooltip_strategies/mobileTooltipStrategy.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getHeight,
    getWidth,
    getOuterHeight
} from "../../../core/utils/size";
import Overlay from "../../overlay/ui.overlay";
import {
    TooltipStrategyBase
} from "./tooltipStrategyBase";
import {
    getWindow
} from "../../../core/utils/window";
var SLIDE_PANEL_CLASS_NAME = "dx-scheduler-overlay-panel";
var MAX_TABLET_OVERLAY_HEIGHT_FACTOR = .9;
var MAX_HEIGHT = {
    PHONE: 250,
    TABLET: "90%",
    DEFAULT: "auto"
};
var MAX_WIDTH = {
    PHONE: "100%",
    TABLET: "80%"
};
var animationConfig = {
    show: {
        type: "slide",
        duration: 300,
        from: {
            position: {
                my: "top",
                at: "bottom",
                of: getWindow()
            }
        },
        to: {
            position: {
                my: "center",
                at: "center",
                of: getWindow()
            }
        }
    },
    hide: {
        type: "slide",
        duration: 300,
        to: {
            position: {
                my: "top",
                at: "bottom",
                of: getWindow()
            }
        },
        from: {
            position: {
                my: "center",
                at: "center",
                of: getWindow()
            }
        }
    }
};
var createPhoneDeviceConfig = listHeight => ({
    shading: false,
    width: MAX_WIDTH.PHONE,
    height: listHeight > MAX_HEIGHT.PHONE ? MAX_HEIGHT.PHONE : MAX_HEIGHT.DEFAULT,
    position: {
        my: "bottom",
        at: "bottom",
        of: getWindow()
    }
});
var createTabletDeviceConfig = listHeight => {
    var currentMaxHeight = getHeight(getWindow()) * MAX_TABLET_OVERLAY_HEIGHT_FACTOR;
    return {
        shading: true,
        width: MAX_WIDTH.TABLET,
        height: listHeight > currentMaxHeight ? MAX_HEIGHT.TABLET : MAX_HEIGHT.DEFAULT,
        position: {
            my: "center",
            at: "center",
            of: getWindow()
        }
    }
};
export class MobileTooltipStrategy extends TooltipStrategyBase {
    _shouldUseTarget() {
        return false
    }
    _onShowing() {
        var isTabletWidth = getWidth(getWindow()) > 700;
        this._tooltip.option("height", MAX_HEIGHT.DEFAULT);
        var listHeight = getOuterHeight(this._list.$element());
        this._tooltip.option(isTabletWidth ? createTabletDeviceConfig(listHeight) : createPhoneDeviceConfig(listHeight))
    }
    _createTooltip(target, dataList) {
        var element = this._createTooltipElement(SLIDE_PANEL_CLASS_NAME);
        return this._options.createComponent(element, Overlay, {
            target: getWindow(),
            closeOnOutsideClick: true,
            animation: animationConfig,
            onShowing: () => this._onShowing(),
            onShown: this._onShown.bind(this),
            contentTemplate: this._getContentTemplate(dataList),
            copyRootClassesToWrapper: true,
            _ignoreCopyRootClassesToWrapperDeprecation: true
        })
    }
}
