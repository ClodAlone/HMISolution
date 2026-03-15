/**
 * DevExtreme (esm/viz/range_selector/slider_marker.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    patchFontOptions
} from "../core/utils";
import {
    consts
} from "./common";
var POINTER_SIZE = consts.pointerSize;
var SLIDER_MARKER_UPDATE_DELAY = 75;

function SliderMarker(renderer, root, isLeftPointer) {
    this._isLeftPointer = isLeftPointer;
    this._isOverlapped = false;
    this._group = renderer.g().attr({
        class: "slider-marker"
    }).append(root);
    this._area = renderer.path(null, "area").append(this._group);
    this._label = renderer.text().attr({
        align: "left"
    }).append(this._group);
    this._tracker = renderer.rect().attr({
        class: "slider-marker-tracker",
        fill: "#000000",
        opacity: 1e-4
    }).css({
        cursor: "pointer"
    }).append(this._group);
    this._border = renderer.rect(0, 0, 1, 0)
}
SliderMarker.prototype = {
    constructor: SliderMarker,
    _getRectSize: function(textSize) {
        return {
            width: Math.round(2 * this._paddingLeftRight + textSize.width),
            height: Math.round(2 * this._paddingTopBottom + textSize.height)
        }
    },
    _getTextSize: function() {
        var textSize = this._label.getBBox();
        if (!this._textHeight && isFinite(textSize.height)) {
            this._textHeight = textSize.height
        }
        return {
            width: textSize.width,
            height: this._textHeight,
            y: textSize.y
        }
    },
    _getAreaPointsInfo: function(textSize) {
        var rectSize = this._getRectSize(textSize);
        var rectWidth = rectSize.width;
        var rectHeight = rectSize.height;
        var rectLeftBorder = -rectWidth;
        var rectRightBorder = 0;
        var pointerRightPoint = POINTER_SIZE;
        var pointerCenterPoint = 0;
        var pointerLeftPoint = -POINTER_SIZE;
        var position = this._position;
        var isLeft = this._isLeftPointer;
        var correctCloudBorders = function() {
            rectLeftBorder++;
            rectRightBorder++;
            pointerRightPoint++;
            pointerCenterPoint++;
            pointerLeftPoint++
        };
        var checkPointerBorders = function() {
            if (pointerRightPoint > rectRightBorder) {
                pointerRightPoint = rectRightBorder
            } else if (pointerLeftPoint < rectLeftBorder) {
                pointerLeftPoint = rectLeftBorder
            }
            isLeft && correctCloudBorders()
        };
        var borderPosition = position;
        if (isLeft) {
            if (position > this._range[1] - rectWidth) {
                rectRightBorder = -position + this._range[1];
                rectLeftBorder = rectRightBorder - rectWidth;
                checkPointerBorders();
                borderPosition += rectLeftBorder
            } else {
                rectLeftBorder = pointerLeftPoint = 0;
                rectRightBorder = rectWidth
            }
        } else if (position - this._range[0] < rectWidth) {
            rectLeftBorder = -(position - this._range[0]);
            rectRightBorder = rectLeftBorder + rectWidth;
            checkPointerBorders();
            borderPosition += rectRightBorder
        } else {
            pointerRightPoint = 0;
            correctCloudBorders()
        }
        this._borderPosition = borderPosition;
        return {
            offset: rectLeftBorder,
            isCut: (!isLeft || pointerCenterPoint !== pointerLeftPoint) && (isLeft || pointerCenterPoint !== pointerRightPoint),
            points: [rectLeftBorder, 0, rectRightBorder, 0, rectRightBorder, rectHeight, pointerRightPoint, rectHeight, pointerCenterPoint, rectHeight + POINTER_SIZE, pointerLeftPoint, rectHeight, rectLeftBorder, rectHeight]
        }
    },
    _update: function() {
        var that = this;
        var textSize;
        clearTimeout(that._timeout);
        that._label.attr({
            text: that._text || ""
        });
        var currentTextSize = that._getTextSize();
        var rectSize = that._getRectSize(currentTextSize);
        textSize = that._textSize || currentTextSize;
        textSize = that._textSize = currentTextSize.width > textSize.width || currentTextSize.height > textSize.height ? currentTextSize : textSize;
        that._timeout = setTimeout((function() {
            updateSliderMarker(currentTextSize, rectSize);
            that._textSize = currentTextSize
        }), SLIDER_MARKER_UPDATE_DELAY);

        function updateSliderMarker(size, rectSize) {
            rectSize = rectSize || that._getRectSize(size);
            that._group.attr({
                translateY: -(rectSize.height + POINTER_SIZE)
            });
            var pointsData = that._getAreaPointsInfo(size);
            var points = pointsData.points;
            var offset = pointsData.offset;
            that._area.attr({
                points: points
            });
            that._border.attr({
                x: that._isLeftPointer ? points[0] - 1 : points[2],
                height: pointsData.isCut ? rectSize.height : rectSize.height + POINTER_SIZE
            });
            that._tracker.attr({
                translateX: offset,
                width: rectSize.width,
                height: rectSize.height + POINTER_SIZE
            });
            that._label.attr({
                translateX: that._paddingLeftRight + offset,
                translateY: rectSize.height / 2 - (size.y + size.height / 2)
            })
        }
        updateSliderMarker(textSize)
    },
    setText: function(value) {
        this._text = value
    },
    setPosition: function(position) {
        this._position = position;
        this._update()
    },
    applyOptions: function(options, screenRange) {
        this._range = screenRange;
        this._paddingLeftRight = options.paddingLeftRight;
        this._paddingTopBottom = options.paddingTopBottom;
        this._textHeight = null;
        this._colors = [options.invalidRangeColor, options.color];
        this._area.attr({
            fill: options.color
        });
        this._border.attr({
            fill: options.borderColor
        });
        this._label.css(patchFontOptions(options.font));
        this._update()
    },
    getTracker: function() {
        return this._tracker
    },
    setValid: function(isValid) {
        this._area.attr({
            fill: this._colors[Number(isValid)]
        })
    },
    setColor: function(color) {
        this._area.attr({
            fill: color
        })
    },
    dispose: function() {
        clearTimeout(this._timeout)
    },
    setOverlapped: function(isOverlapped) {
        if (this._isOverlapped !== isOverlapped) {
            if (isOverlapped) {
                this._border.append(this._group)
            } else {
                this._isOverlapped && this._border.remove()
            }
            this._isOverlapped = isOverlapped
        }
    },
    getBorderPosition: function() {
        return this._borderPosition
    }
};
export default SliderMarker;
