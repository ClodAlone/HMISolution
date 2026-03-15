/**
 * DevExtreme (cjs/ui/slider/slider_tooltip_position_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.SliderTooltipPositionController = void 0;
var _popover_position_controller = require("../popover/popover_position_controller");
var _translator = require("../../animation/translator");
var _position = _interopRequireDefault(require("../../animation/position"));
var _extend = require("../../core/utils/extend");
var _type = require("../../core/utils/type");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var SLIDER_TOOLTIP_POSITION_ALIASES = {
    top: {
        my: "bottom center",
        at: "top center",
        collision: "fit none"
    },
    bottom: {
        my: "top center",
        at: "bottom center",
        collision: "fit none"
    }
};
var SLIDER_TOOLTIP_DEFAULT_BOUNDARY_OFFSET = {
    h: 2,
    v: 1
};
var SLIDER_CLASS = "dx-slider";
var SliderTooltipPositionController = function(_PopoverPositionContr) {
    _inheritsLoose(SliderTooltipPositionController, _PopoverPositionContr);

    function SliderTooltipPositionController() {
        return _PopoverPositionContr.apply(this, arguments) || this
    }
    var _proto = SliderTooltipPositionController.prototype;
    _proto._normalizePosition = function(positionProp, $sliderHandle) {
        var sliderClass = ".".concat(SLIDER_CLASS);
        var $slider = null === $sliderHandle || void 0 === $sliderHandle ? void 0 : $sliderHandle.closest(sliderClass);
        var defaultPositionConfig = {
            of: $sliderHandle,
            boundaryOffset: SLIDER_TOOLTIP_DEFAULT_BOUNDARY_OFFSET,
            boundary: null === $slider || void 0 === $slider ? void 0 : $slider.get(0)
        };
        var resultPosition = (0, _extend.extend)(true, {}, defaultPositionConfig, this._positionToObject(positionProp));
        this._positionSide = this._getDisplaySide(resultPosition);
        return resultPosition
    };
    _proto._renderContentInitialPosition = function() {
        _PopoverPositionContr.prototype._renderContentInitialPosition.call(this);
        this._fitIntoSlider()
    };
    _proto._fitIntoSlider = function() {
        var _positionUtils$calcul = _position.default.calculate(this._$content, this._position).h,
            collisionSide = _positionUtils$calcul.collisionSide,
            oversize = _positionUtils$calcul.oversize;
        var left = this._visualPosition.left;
        var isLeftSide = "left" === collisionSide;
        var offset = (isLeftSide ? 1 : -1) * oversize;
        (0, _translator.move)(this._$content, {
            left: left + offset
        });
        this._updateVisualPositionValue()
    };
    _proto._positionToObject = function(positionProp) {
        if ((0, _type.isString)(positionProp)) {
            return (0, _extend.extend)({}, SLIDER_TOOLTIP_POSITION_ALIASES[positionProp])
        }
        return positionProp
    };
    return SliderTooltipPositionController
}(_popover_position_controller.PopoverPositionController);
exports.SliderTooltipPositionController = SliderTooltipPositionController;
