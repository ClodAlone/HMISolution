/**
 * DevExtreme (esm/viz/core/layout_element.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    noop
} from "../../core/utils/common";
var _round = Math.round;
import {
    clone
} from "../../core/utils/object";
var defaultOffset = {
    horizontal: 0,
    vertical: 0
};
var alignFactors = {
    center: .5,
    right: 1,
    bottom: 1,
    left: 0,
    top: 0
};

function LayoutElement(options) {
    this._options = options
}
LayoutElement.prototype = {
    constructor: LayoutElement,
    position: function(options) {
        var ofBBox = options.of.getLayoutOptions();
        var myBBox = this.getLayoutOptions();
        var at = options.at;
        var my = options.my;
        var offset = options.offset || defaultOffset;
        var shiftX = -alignFactors[my.horizontal] * myBBox.width + ofBBox.x + alignFactors[at.horizontal] * ofBBox.width + parseInt(offset.horizontal);
        var shiftY = -alignFactors[my.vertical] * myBBox.height + ofBBox.y + alignFactors[at.vertical] * ofBBox.height + parseInt(offset.vertical);
        this.shift(_round(shiftX), _round(shiftY))
    },
    getLayoutOptions: noop
};

function WrapperLayoutElement(renderElement, bBox) {
    this._renderElement = renderElement;
    this._cacheBBox = bBox
}
var wrapperLayoutElementPrototype = WrapperLayoutElement.prototype = clone(LayoutElement.prototype);
wrapperLayoutElementPrototype.constructor = WrapperLayoutElement;
wrapperLayoutElementPrototype.getLayoutOptions = function() {
    return this._cacheBBox || this._renderElement.getBBox()
};
wrapperLayoutElementPrototype.shift = function(shiftX, shiftY) {
    var bBox = this.getLayoutOptions();
    this._renderElement.move(_round(shiftX - bBox.x), _round(shiftY - bBox.y))
};
export {
    LayoutElement,
    WrapperLayoutElement
};
