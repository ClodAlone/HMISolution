/**
 * DevExtreme (esm/viz/range_selector/slider.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    utils,
    formatValue
} from "./common";
var animationSettings = utils.animationSettings;
import SliderMarker from "./slider_marker";
import {
    touchEvents,
    pointerEvents
} from "../../core/utils/support";
var SPLITTER_WIDTH = 8;
var TOUCH_SPLITTER_WIDTH = 20;

function getSliderTrackerWidth(sliderHandleWidth) {
    return touchEvents || pointerEvents ? TOUCH_SPLITTER_WIDTH : SPLITTER_WIDTH < sliderHandleWidth ? sliderHandleWidth : SPLITTER_WIDTH
}

function Slider(params, index) {
    this._translator = params.translator;
    this._sliderGroup = params.renderer.g().attr({
        class: "slider"
    }).append(params.root);
    this._line = params.renderer.path(null, "line").append(this._sliderGroup);
    this._marker = new SliderMarker(params.renderer, this._sliderGroup, 1 === index);
    this._tracker = params.renderer.rect().attr({
        class: "slider-tracker",
        fill: "#000000",
        opacity: 1e-4
    }).css({
        cursor: "w-resize"
    }).append(params.trackersGroup)
}
Slider.prototype = {
    constructor: Slider,
    cancelAnimation: function() {
        this._sliderGroup.stopAnimation();
        this._tracker.stopAnimation()
    },
    applyPosition: function(isAnimated) {
        var slider = this._sliderGroup;
        var tracker = this._tracker;
        var attrs = {
            translateX: this._position
        };
        this._marker.setPosition(this._position);
        if (isAnimated) {
            slider.animate(attrs, animationSettings);
            tracker.animate(attrs, animationSettings)
        } else {
            slider.attr(attrs);
            tracker.attr(attrs)
        }
    },
    _setValid: function(isValid) {
        this._marker.setValid(isValid);
        this._line.attr({
            stroke: this._colors[Number(isValid)]
        })
    },
    _setText: function(text) {
        this._marker.setText(text)
    },
    update: function(verticalRange, sliderHandleOptions, sliderMarkerOptions) {
        this._formatOptions = {
            format: sliderMarkerOptions.format,
            customizeText: sliderMarkerOptions.customizeText
        };
        this._marker.applyOptions(sliderMarkerOptions, this._translator.getScreenRange());
        this._colors = [sliderMarkerOptions.invalidRangeColor, sliderHandleOptions.color];
        this._sliderGroup.attr({
            translateY: verticalRange[0]
        });
        this._line.attr({
            "stroke-width": sliderHandleOptions.width,
            stroke: sliderHandleOptions.color,
            "stroke-opacity": sliderHandleOptions.opacity,
            sharp: "h",
            points: [0, 0, 0, verticalRange[1] - verticalRange[0]]
        });
        var trackerWidth = getSliderTrackerWidth(sliderHandleOptions.width);
        this._tracker.attr({
            x: -trackerWidth / 2,
            y: 0,
            width: trackerWidth,
            height: verticalRange[1] - verticalRange[0],
            translateY: verticalRange[0]
        })
    },
    toForeground: function() {
        this._sliderGroup.toForeground()
    },
    getSliderTracker: function() {
        return this._tracker
    },
    getPosition: function() {
        return this._position
    },
    setDisplayValue: function(value) {
        this._value = value;
        this._setText(formatValue(value, this._formatOptions))
    },
    setOverlapped: function(isOverlapped) {
        this._marker.setOverlapped(isOverlapped)
    },
    getValue: function() {
        return this._value
    },
    on: function(event, handler) {
        this._tracker.on(event, handler);
        this._marker.getTracker().on(event, handler)
    },
    getCloudBorder: function() {
        return this._marker.getBorderPosition()
    },
    dispose: function() {
        this._marker.dispose()
    }
};
export default Slider;
