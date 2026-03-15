/**
 * DevExtreme (cjs/ui/slider/ui.slider_tooltip.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _tooltip = _interopRequireDefault(require("../tooltip"));
var _extend = require("../../core/utils/extend");
var _slider_tooltip_position_controller = require("./slider_tooltip_position_controller");
var _number = _interopRequireDefault(require("../../localization/number"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var SLIDER_TOOLTIP_VISIBILITY_CLASS = "dx-slider-tooltip-visible-on-hover";
var SliderTooltip = _tooltip.default.inherit({
    _getDefaultOptions: function() {
        return (0, _extend.extend)(this.callBase(), {
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
            format: function(value) {
                return value
            },
            value: 0
        })
    },
    _initMarkup: function() {
        this.callBase();
        this._attachToMarkup(this.option("visible"));
        this._toggleShowModeClass()
    },
    _renderContent: function() {
        this.callBase();
        this._renderContentText()
    },
    _renderContentText: function() {
        var _this$option = this.option(),
            value = _this$option.value,
            format = _this$option.format;
        var formattedText = _number.default.format(null !== value && void 0 !== value ? value : 0, format);
        this.$content().text(formattedText);
        this._renderPosition()
    },
    _toggleShowModeClass: function() {
        var isHoverMode = "onHover" === this.option("showMode");
        this._positionController.$container.toggleClass(SLIDER_TOOLTIP_VISIBILITY_CLASS, isHoverMode)
    },
    _initPositionController: function() {
        this._positionController = new _slider_tooltip_position_controller.SliderTooltipPositionController(this._getPositionControllerConfig())
    },
    _attachToMarkup: function(enabled) {
        enabled ? this.$element().appendTo(this._positionController.$container) : this.$element().detach()
    },
    _optionChanged: function(args) {
        var name = args.name,
            value = args.value;
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
    updatePosition: function() {
        this._renderPosition()
    }
});
var _default = SliderTooltip;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
