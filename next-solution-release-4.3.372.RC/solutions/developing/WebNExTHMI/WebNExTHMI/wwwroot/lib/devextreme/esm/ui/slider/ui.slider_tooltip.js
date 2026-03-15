/**
 * DevExtreme (esm/ui/slider/ui.slider_tooltip.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Tooltip from "../tooltip";
import {
    extend
} from "../../core/utils/extend";
import {
    SliderTooltipPositionController
} from "./slider_tooltip_position_controller";
import numberLocalization from "../../localization/number";
var SLIDER_TOOLTIP_VISIBILITY_CLASS = "dx-slider-tooltip-visible-on-hover";
var SliderTooltip = Tooltip.inherit({
    _getDefaultOptions() {
        return extend(this.callBase(), {
            visible: false,
            position: "top",
            closeOnOutsideClick: false,
            hideTopOverlayHandler: null,
            hideOnParentScroll: false,
            animation: null,
            arrowPosition: null,
            templatesRenderAsynchronously: false,
            _fixWrapperPosition: false,
            useResizeObserver: false,
            showMode: "onHover",
            format: value => value,
            value: 0
        })
    },
    _initMarkup() {
        this.callBase();
        this._attachToMarkup(this.option("visible"));
        this._toggleShowModeClass()
    },
    _renderContent() {
        this.callBase();
        this._renderContentText()
    },
    _renderContentText() {
        var {
            value: value,
            format: format
        } = this.option();
        var formattedText = numberLocalization.format(null !== value && void 0 !== value ? value : 0, format);
        this.$content().text(formattedText);
        this._renderPosition()
    },
    _toggleShowModeClass() {
        var isHoverMode = "onHover" === this.option("showMode");
        this._positionController.$container.toggleClass(SLIDER_TOOLTIP_VISIBILITY_CLASS, isHoverMode)
    },
    _initPositionController() {
        this._positionController = new SliderTooltipPositionController(this._getPositionControllerConfig())
    },
    _attachToMarkup(enabled) {
        enabled ? this.$element().appendTo(this._positionController.$container) : this.$element().detach()
    },
    _optionChanged(args) {
        var {
            name: name,
            value: value
        } = args;
        switch (name) {
            case "visible":
                this._attachToMarkup(value);
                this.callBase(args);
                break;
            case "showMode":
                this._toggleShowModeClass();
                break;
            case "format":
            case "value":
                this._renderContentText();
                break;
            default:
                this.callBase(args)
        }
    },
    updatePosition() {
        this._renderPosition()
    }
});
export default SliderTooltip;
