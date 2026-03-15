/**
 * DevExtreme (esm/ui/slider/slider_tooltip_position_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    PopoverPositionController
} from "../popover/popover_position_controller";
import {
    move
} from "../../animation/translator";
import positionUtils from "../../animation/position";
import {
    extend
} from "../../core/utils/extend";
import {
    isString
} from "../../core/utils/type";
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
class SliderTooltipPositionController extends PopoverPositionController {
    _normalizePosition(positionProp, $sliderHandle) {
        var sliderClass = ".".concat(SLIDER_CLASS);
        var $slider = null === $sliderHandle || void 0 === $sliderHandle ? void 0 : $sliderHandle.closest(sliderClass);
        var defaultPositionConfig = {
            of: $sliderHandle,
            boundaryOffset: SLIDER_TOOLTIP_DEFAULT_BOUNDARY_OFFSET,
            boundary: null === $slider || void 0 === $slider ? void 0 : $slider.get(0)
        };
        var resultPosition = extend(true, {}, defaultPositionConfig, this._positionToObject(positionProp));
        this._positionSide = this._getDisplaySide(resultPosition);
        return resultPosition
    }
    _renderContentInitialPosition() {
        super._renderContentInitialPosition();
        this._fitIntoSlider()
    }
    _fitIntoSlider() {
        var {
            collisionSide: collisionSide,
            oversize: oversize
        } = positionUtils.calculate(this._$content, this._position).h;
        var left = this._visualPosition.left;
        var isLeftSide = "left" === collisionSide;
        var offset = (isLeftSide ? 1 : -1) * oversize;
        move(this._$content, {
            left: left + offset
        });
        this._updateVisualPositionValue()
    }
    _positionToObject(positionProp) {
        if (isString(positionProp)) {
            return extend({}, SLIDER_TOOLTIP_POSITION_ALIASES[positionProp])
        }
        return positionProp
    }
}
export {
    SliderTooltipPositionController
};
