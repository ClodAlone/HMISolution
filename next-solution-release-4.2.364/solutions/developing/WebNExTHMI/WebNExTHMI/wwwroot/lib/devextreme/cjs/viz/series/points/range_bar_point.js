/**
 * DevExtreme (cjs/viz/series/points/range_bar_point.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _common = require("../../../core/utils/common");
var _extend2 = require("../../../core/utils/extend");
var _bar_point = _interopRequireDefault(require("./bar_point"));
var _range_symbol_point = _interopRequireDefault(require("./range_symbol_point"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _extend = _extend2.extend;
var _default = _extend({}, _bar_point.default, {
    deleteLabel: _range_symbol_point.default.deleteLabel,
    _getFormatObject: _range_symbol_point.default._getFormatObject,
    clearVisibility: function() {
        var graphic = this.graphic;
        if (graphic && graphic.attr("visibility")) {
            graphic.attr({
                visibility: null
            })
        }
    },
    setInvisibility: function() {
        var graphic = this.graphic;
        if (graphic && "hidden" !== graphic.attr("visibility")) {
            graphic.attr({
                visibility: "hidden"
            })
        }
        this._topLabel.draw(false);
        this._bottomLabel.draw(false)
    },
    getTooltipParams: function(location) {
        var edgeLocation = "edge" === location;
        var x;
        var y;
        if (this._options.rotated) {
            x = edgeLocation ? this.x + this.width : this.x + this.width / 2;
            y = this.y + this.height / 2
        } else {
            x = this.x + this.width / 2;
            y = edgeLocation ? this.y : this.y + this.height / 2
        }
        return {
            x: x,
            y: y,
            offset: 0
        }
    },
    _translate: function() {
        var barMethods = _bar_point.default;
        barMethods._translate.call(this);
        if (this._options.rotated) {
            this.width = this.width || 1
        } else {
            this.height = this.height || 1
        }
    },
    hasCoords: _range_symbol_point.default.hasCoords,
    _updateData: _range_symbol_point.default._updateData,
    _getLabelPosition: _range_symbol_point.default._getLabelPosition,
    _getLabelMinFormatObject: _range_symbol_point.default._getLabelMinFormatObject,
    _updateLabelData: _range_symbol_point.default._updateLabelData,
    _updateLabelOptions: _range_symbol_point.default._updateLabelOptions,
    getCrosshairData: _range_symbol_point.default.getCrosshairData,
    _createLabel: _range_symbol_point.default._createLabel,
    _checkOverlay: _range_symbol_point.default._checkOverlay,
    _checkLabelsOverlay: _range_symbol_point.default._checkLabelsOverlay,
    _getOverlayCorrections: _range_symbol_point.default._getOverlayCorrections,
    _drawLabel: _range_symbol_point.default._drawLabel,
    _getLabelCoords: _range_symbol_point.default._getLabelCoords,
    getLabel: _range_symbol_point.default.getLabel,
    getLabels: _range_symbol_point.default.getLabels,
    getBoundingRect: _common.noop,
    getMinValue: _range_symbol_point.default.getMinValue,
    getMaxValue: _range_symbol_point.default.getMaxValue
});
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
