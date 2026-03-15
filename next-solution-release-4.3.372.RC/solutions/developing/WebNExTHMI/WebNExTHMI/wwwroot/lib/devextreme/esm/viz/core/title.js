/**
 * DevExtreme (esm/viz/core/title.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isString as _isString
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import {
    patchFontOptions as _patchFontOptions,
    enumParser
} from "./utils";
import {
    LayoutElement
} from "./layout_element";
var _Number = Number;
var parseHorizontalAlignment = enumParser(["left", "center", "right"]);
var parseVerticalAlignment = enumParser(["top", "bottom"]);
var DEFAULT_MARGIN = 10;

function hasText(text) {
    return !!(text && String(text).length > 0)
}

function processTitleLength(elem, text, width, options, placeholderSize) {
    if (elem.attr({
            text: text
        }).setMaxSize(width, placeholderSize, options).textChanged) {
        elem.setTitle(text)
    }
}

function pickMarginValue(value) {
    return value >= 0 ? _Number(value) : DEFAULT_MARGIN
}

function validateMargin(margin) {
    var result;
    if (margin >= 0) {
        result = {
            left: _Number(margin),
            top: _Number(margin),
            right: _Number(margin),
            bottom: _Number(margin)
        }
    } else {
        margin = margin || {};
        result = {
            left: pickMarginValue(margin.left),
            top: pickMarginValue(margin.top),
            right: pickMarginValue(margin.right),
            bottom: pickMarginValue(margin.bottom)
        }
    }
    return result
}

function checkRect(rect, boundingRect) {
    return rect[2] - rect[0] < boundingRect.width || rect[3] - rect[1] < boundingRect.height
}
export var Title = function(params) {
    this._params = params;
    this._group = params.renderer.g().attr({
        class: params.cssClass
    }).linkOn(params.root || params.renderer.root, "title");
    this._hasText = false
};
extend(Title.prototype, LayoutElement.prototype, {
    dispose: function() {
        this._group.linkRemove();
        this._group.linkOff();
        if (this._titleElement) {
            this._clipRect.dispose();
            this._titleElement = this._subtitleElement = this._clipRect = null
        }
        this._params = this._group = this._options = null
    },
    _updateOptions: function(options) {
        this._options = options;
        this._options.horizontalAlignment = parseHorizontalAlignment(options.horizontalAlignment, "center");
        this._options.verticalAlignment = parseVerticalAlignment(options.verticalAlignment, "top");
        this._options.margin = validateMargin(options.margin)
    },
    _updateStructure: function() {
        var renderer = this._params.renderer;
        var group = this._group;
        var options = this._options;
        var align = options.horizontalAlignment;
        if (!this._titleElement) {
            this._titleElement = renderer.text().append(group);
            this._subtitleElement = renderer.text();
            this._clipRect = renderer.clipRect();
            group.attr({
                "clip-path": this._clipRect.id
            })
        }
        this._titleElement.attr({
            align: align,
            class: options.cssClass
        });
        this._subtitleElement.attr({
            align: align,
            class: options.subtitle.cssClass
        });
        group.linkAppend();
        hasText(options.subtitle.text) ? this._subtitleElement.append(group) : this._subtitleElement.remove()
    },
    _updateTexts: function() {
        var options = this._options;
        var subtitleOptions = options.subtitle;
        var titleElement = this._titleElement;
        var subtitleElement = this._subtitleElement;
        var titleBox;
        titleElement.attr({
            text: "A",
            y: 0
        }).css(_patchFontOptions(options.font));
        titleBox = titleElement.getBBox();
        this._baseLineCorrection = titleBox.height + titleBox.y;
        titleElement.attr({
            text: options.text
        });
        titleBox = titleElement.getBBox();
        var y = -titleBox.y;
        titleElement.attr({
            y: y
        });
        if (hasText(subtitleOptions.text)) {
            subtitleElement.attr({
                text: subtitleOptions.text,
                y: 0
            }).css(_patchFontOptions(subtitleOptions.font))
        }
    },
    _shiftSubtitle() {
        var titleBox = this._titleElement.getBBox();
        var element = this._subtitleElement;
        var offset = this._options.subtitle.offset;
        element.move(0, titleBox.y + titleBox.height - element.getBBox().y - offset)
    },
    _updateBoundingRectAlignment: function() {
        var boundingRect = this._boundingRect;
        var options = this._options;
        boundingRect.verticalAlignment = options.verticalAlignment;
        boundingRect.horizontalAlignment = options.horizontalAlignment;
        boundingRect.cutLayoutSide = options.verticalAlignment;
        boundingRect.cutSide = "vertical";
        boundingRect.position = {
            horizontal: options.horizontalAlignment,
            vertical: options.verticalAlignment
        }
    },
    hasText: function() {
        return this._hasText
    },
    update: function(themeOptions, userOptions) {
        var options = extend(true, {}, themeOptions, processTitleOptions(userOptions));
        var _hasText = hasText(options.text);
        var isLayoutChanged = _hasText || _hasText !== this._hasText;
        this._baseLineCorrection = 0;
        this._updateOptions(options);
        this._boundingRect = {};
        if (_hasText) {
            this._updateStructure();
            this._updateTexts()
        } else {
            this._group.linkRemove()
        }
        this._updateBoundingRect();
        this._updateBoundingRectAlignment();
        this._hasText = _hasText;
        return isLayoutChanged
    },
    draw: function(width, height) {
        if (this._hasText) {
            this._group.linkAppend();
            this._correctTitleLength(width);
            if (this._group.getBBox().height > height) {
                this.freeSpace()
            }
        }
        return this
    },
    _correctTitleLength: function(width) {
        var options = this._options;
        var margin = options.margin;
        var maxWidth = width - margin.left - margin.right;
        var placeholderSize = options.placeholderSize;
        processTitleLength(this._titleElement, options.text, maxWidth, options, placeholderSize);
        if (this._subtitleElement) {
            if (_Number(placeholderSize) > 0) {
                placeholderSize -= this._titleElement.getBBox().height
            }
            processTitleLength(this._subtitleElement, options.subtitle.text, maxWidth, options.subtitle, placeholderSize);
            this._shiftSubtitle()
        }
        this._updateBoundingRect();
        var {
            x: x,
            y: y,
            height: height
        } = this.getCorrectedLayoutOptions();
        this._clipRect.attr({
            x: x,
            y: y,
            width: width,
            height: height
        })
    },
    getLayoutOptions: function() {
        return this._boundingRect || null
    },
    shift: function(x, y) {
        var box = this.getLayoutOptions();
        this._group.move(x - box.x, y - box.y);
        return this
    },
    _updateBoundingRect: function() {
        var options = this._options;
        var margin = options.margin;
        var boundingRect = this._boundingRect;
        var box = this._hasText ? this._group.getBBox() : {
            width: 0,
            height: 0,
            x: 0,
            y: 0,
            isEmpty: true
        };
        if (!box.isEmpty) {
            box.height += margin.top + margin.bottom - this._baseLineCorrection;
            box.width += margin.left + margin.right;
            box.x -= margin.left;
            box.y += this._baseLineCorrection - margin.top
        }
        if (options.placeholderSize > 0) {
            box.height = options.placeholderSize
        }
        boundingRect.height = box.height;
        boundingRect.width = box.width;
        boundingRect.x = box.x;
        boundingRect.y = box.y
    },
    getCorrectedLayoutOptions() {
        var srcBox = this.getLayoutOptions();
        var correction = this._baseLineCorrection;
        return extend({}, srcBox, {
            y: srcBox.y - correction,
            height: srcBox.height + correction
        })
    },
    layoutOptions: function() {
        if (!this._hasText) {
            return null
        }
        return {
            horizontalAlignment: this._boundingRect.horizontalAlignment,
            verticalAlignment: this._boundingRect.verticalAlignment,
            priority: 0
        }
    },
    measure: function(size) {
        this.draw(size[0], size[1]);
        return [this._boundingRect.width, this._boundingRect.height]
    },
    move: function(rect, fitRect) {
        var boundingRect = this._boundingRect;
        if (checkRect(rect, boundingRect)) {
            this.shift(fitRect[0], fitRect[1])
        } else {
            this.shift(Math.round(rect[0]), Math.round(rect[1]))
        }
    },
    freeSpace: function() {
        this._params.incidentOccurred("W2103");
        this._group.linkRemove();
        this._boundingRect.width = this._boundingRect.height = 0
    },
    getOptions: function() {
        return this._options
    },
    changeLink: function(root) {
        this._group.linkRemove();
        this._group.linkOn(root, "title")
    }
});

function processTitleOptions(options) {
    var newOptions = _isString(options) ? {
        text: options
    } : options || {};
    newOptions.subtitle = _isString(newOptions.subtitle) ? {
        text: newOptions.subtitle
    } : newOptions.subtitle || {};
    return newOptions
}
export var plugin = {
    name: "title",
    init: function() {
        this._title = new Title({
            renderer: this._renderer,
            cssClass: this._rootClassPrefix + "-title",
            incidentOccurred: this._incidentOccurred
        });
        this._layout.add(this._title)
    },
    dispose: function() {
        this._title.dispose();
        this._title = null
    },
    customize: function(constructor) {
        constructor.addChange({
            code: "TITLE",
            handler: function() {
                if (this._title.update(this._themeManager.theme("title"), this.option("title"))) {
                    this._change(["LAYOUT"])
                }
            },
            isThemeDependent: true,
            option: "title",
            isOptionChange: true
        })
    },
    fontFields: ["title.font", "title.subtitle.font"]
};
