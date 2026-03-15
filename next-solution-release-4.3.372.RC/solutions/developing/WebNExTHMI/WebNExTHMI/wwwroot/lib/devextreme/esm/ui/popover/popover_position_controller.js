/**
 * DevExtreme (esm/ui/popover/popover_position_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["shading", "$arrow"];
import {
    isDefined,
    isString
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import positionUtils from "../../animation/position";
import {
    pairToObject
} from "../../core/utils/common";
import {
    borderWidthStyles
} from "../../renovation/ui/resizable/utils";
import {
    getWidth,
    getHeight
} from "../../core/utils/size";
import {
    OverlayPositionController
} from "../overlay/overlay_position_controller";
var WEIGHT_OF_SIDES = {
    left: -1,
    top: -1,
    center: 0,
    right: 1,
    bottom: 1
};
var POPOVER_POSITION_ALIASES = {
    top: {
        my: "bottom center",
        at: "top center",
        collision: "fit flip"
    },
    bottom: {
        my: "top center",
        at: "bottom center",
        collision: "fit flip"
    },
    right: {
        my: "left center",
        at: "right center",
        collision: "flip fit"
    },
    left: {
        my: "right center",
        at: "left center",
        collision: "flip fit"
    }
};
var POPOVER_DEFAULT_BOUNDARY_OFFSET = {
    h: 10,
    v: 10
};
class PopoverPositionController extends OverlayPositionController {
    constructor(_ref) {
        var {
            shading: shading,
            $arrow: $arrow
        } = _ref, args = _objectWithoutPropertiesLoose(_ref, _excluded);
        super(args);
        this._props = _extends({}, this._props, {
            shading: shading
        });
        this._$arrow = $arrow;
        this._positionSide = void 0
    }
    positionWrapper() {
        if (this._props.shading) {
            this._$wrapper.css({
                top: 0,
                left: 0
            })
        }
    }
    _renderBoundaryOffset() {}
    _getContainerPosition() {
        var offset = pairToObject(this._position.offset || "");
        var {
            h: hOffset,
            v: vOffset
        } = offset;
        var isVerticalSide = this._isVerticalSide();
        var isHorizontalSide = this._isHorizontalSide();
        if (isVerticalSide || isHorizontalSide) {
            var isPopoverInside = this._isPopoverInside();
            var sign = (isPopoverInside ? -1 : 1) * WEIGHT_OF_SIDES[this._positionSide];
            var arrowSize = isVerticalSide ? getHeight(this._$arrow) : getWidth(this._$arrow);
            var arrowSizeCorrection = this._getContentBorderWidth(this._positionSide);
            var arrowOffset = sign * (arrowSize - arrowSizeCorrection);
            isVerticalSide ? vOffset += arrowOffset : hOffset += arrowOffset
        }
        return extend({}, this._position, {
            offset: hOffset + " " + vOffset
        })
    }
    _getContentBorderWidth(side) {
        var borderWidth = this._$content.css(borderWidthStyles[side]);
        return parseInt(borderWidth) || 0
    }
    _isPopoverInside() {
        var my = positionUtils.setup.normalizeAlign(this._position.my);
        var at = positionUtils.setup.normalizeAlign(this._position.at);
        return my.h === at.h && my.v === at.v
    }
    _isVerticalSide() {
        var side = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : this._positionSide;
        return "top" === side || "bottom" === side
    }
    _isHorizontalSide() {
        var side = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : this._positionSide;
        return "left" === side || "right" === side
    }
    _getDisplaySide(position) {
        var my = positionUtils.setup.normalizeAlign(position.my);
        var at = positionUtils.setup.normalizeAlign(position.at);
        var weightSign = WEIGHT_OF_SIDES[my.h] === WEIGHT_OF_SIDES[at.h] && WEIGHT_OF_SIDES[my.v] === WEIGHT_OF_SIDES[at.v] ? -1 : 1;
        var horizontalWeight = Math.abs(WEIGHT_OF_SIDES[my.h] - weightSign * WEIGHT_OF_SIDES[at.h]);
        var verticalWeight = Math.abs(WEIGHT_OF_SIDES[my.v] - weightSign * WEIGHT_OF_SIDES[at.v]);
        return horizontalWeight > verticalWeight ? at.h : at.v
    }
    _normalizePosition(positionProp, targetProp) {
        var defaultPositionConfig = {
            of: targetProp,
            boundaryOffset: POPOVER_DEFAULT_BOUNDARY_OFFSET
        };
        var resultPosition;
        if (isDefined(positionProp)) {
            resultPosition = extend(true, {}, defaultPositionConfig, this._positionToObject(positionProp))
        } else {
            resultPosition = defaultPositionConfig
        }
        this._positionSide = this._getDisplaySide(resultPosition);
        return resultPosition
    }
    _positionToObject(positionProp) {
        if (isString(positionProp)) {
            return extend({}, POPOVER_POSITION_ALIASES[positionProp])
        }
        return positionProp
    }
}
export {
    PopoverPositionController,
    POPOVER_POSITION_ALIASES
};
