/**
 * DevExtreme (esm/ui/scheduler/tooltip_strategies/desktopTooltipStrategy.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    TooltipStrategyBase
} from "./tooltipStrategyBase";
import Tooltip from "../../tooltip";
import {
    touch
} from "../../../core/utils/support";
var APPOINTMENT_TOOLTIP_WRAPPER_CLASS = "dx-scheduler-appointment-tooltip-wrapper";
var MAX_TOOLTIP_HEIGHT = 200;
export class DesktopTooltipStrategy extends TooltipStrategyBase {
    _prepareBeforeVisibleChanged(dataList) {
        this._tooltip.option("position", {
            my: "bottom",
            at: "top",
            boundary: this._getBoundary(dataList),
            offset: this._extraOptions.offset,
            collision: "fit flipfit"
        })
    }
    _getBoundary(dataList) {
        return this._options.isAppointmentInAllDayPanel(dataList[0].appointment) ? this._options.container : this._options.getScrollableContainer()
    }
    _onShown() {
        super._onShown();
        if (this._extraOptions.isButtonClick) {
            this._list.focus();
            this._list.option("focusedElement", null)
        }
    }
    _createListOption(target, dataList) {
        var result = super._createListOption(target, dataList);
        result.showScrollbar = touch ? "always" : "onHover";
        return result
    }
    _createTooltip(target, dataList) {
        var tooltip = this._createTooltipElement(APPOINTMENT_TOOLTIP_WRAPPER_CLASS);
        return this._options.createComponent(tooltip, Tooltip, {
            target: target,
            maxHeight: MAX_TOOLTIP_HEIGHT,
            rtlEnabled: this._extraOptions.rtlEnabled,
            onShown: this._onShown.bind(this),
            contentTemplate: this._getContentTemplate(dataList),
            copyRootClassesToWrapper: true,
            _ignoreCopyRootClassesToWrapperDeprecation: true
        })
    }
    _onListRender(e) {
        return this._extraOptions.dragBehavior && this._extraOptions.dragBehavior(e)
    }
}
