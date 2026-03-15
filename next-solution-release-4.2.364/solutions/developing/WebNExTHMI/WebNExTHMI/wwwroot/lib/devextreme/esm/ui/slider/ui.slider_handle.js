/**
 * DevExtreme (esm/ui/slider/ui.slider_handle.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import Widget from "../widget/ui.widget";
import SliderTooltip from "./ui.slider_tooltip";
import {
    extend
} from "../../core/utils/extend";
var SLIDER_HANDLE_CLASS = "dx-slider-handle";
var SliderHandle = Widget.inherit({
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            hoverStateEnabled: false,
            value: 0,
            tooltip: {
                enabled: false,
                format: value => value,
                position: "top",
                showMode: "onHover"
            }
        })
    },
    _initMarkup: function() {
        this.callBase();
        this.$element().addClass(SLIDER_HANDLE_CLASS);
        this.setAria({
            role: "slider",
            valuenow: this.option("value")
        })
    },
    _render: function() {
        this.callBase();
        this._renderTooltip()
    },
    _renderTooltip: function() {
        var {
            tooltip: tooltip,
            value: value
        } = this.option();
        var {
            position: position,
            format: format,
            enabled: enabled,
            showMode: showMode
        } = tooltip;
        this._sliderTooltip = this._createComponent($("<div>"), SliderTooltip, {
            target: this.$element(),
            container: this.$element(),
            position: position,
            visible: enabled,
            showMode: showMode,
            format: format,
            value: value
        })
    },
    _clean: function() {
        this.callBase();
        this._sliderTooltip = null
    },
    _updateTooltipOptions(args) {
        var _this$_sliderTooltip;
        var tooltipOptions = Widget.getOptionsFromContainer(args);
        this._setWidgetOption("_sliderTooltip", [tooltipOptions]);
        null === (_this$_sliderTooltip = this._sliderTooltip) || void 0 === _this$_sliderTooltip ? void 0 : _this$_sliderTooltip.option("visible", tooltipOptions.enabled)
    },
    _optionChanged: function(args) {
        var {
            name: name,
            value: value
        } = args;
        switch (name) {
            case "value":
                var _this$_sliderTooltip2;
                null === (_this$_sliderTooltip2 = this._sliderTooltip) || void 0 === _this$_sliderTooltip2 ? void 0 : _this$_sliderTooltip2.option("value", value);
                this.setAria("valuenow", value);
                break;
            case "tooltip":
                this._updateTooltipOptions(args);
                break;
            default:
                this.callBase(arguments)
        }
    },
    updateTooltipPosition: function() {
        var _this$_sliderTooltip3;
        null === (_this$_sliderTooltip3 = this._sliderTooltip) || void 0 === _this$_sliderTooltip3 ? void 0 : _this$_sliderTooltip3.updatePosition()
    },
    repaint: function() {
        var _this$_sliderTooltip4;
        null === (_this$_sliderTooltip4 = this._sliderTooltip) || void 0 === _this$_sliderTooltip4 ? void 0 : _this$_sliderTooltip4.repaint()
    }
});
export default SliderHandle;
