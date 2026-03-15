/**
 * DevExtreme (cjs/viz/gauges/base_indicators.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.BaseTextCloudMarker = exports.BaseRangeBar = exports.BaseIndicator = exports.BaseElement = void 0;
var _common = require("../../core/utils/common");
var _iterator = require("../../core/utils/iterator");
var _base_gauge = require("./base_gauge");
var _utils = require("../core/utils");
var _extend = require("../../core/utils/extend");
var _class = _interopRequireDefault(require("../../core/class"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _isFinite = isFinite;
var _Number = Number;
var _round = Math.round;
var _formatValue = _base_gauge.formatValue;
var _getSampleText = _base_gauge.getSampleText;
var BaseElement = _class.default.inherit({
    ctor: function(parameters) {
        var that = this;
        (0, _iterator.each)(parameters, (function(name, value) {
            that["_" + name] = value
        }));
        that._init()
    },
    dispose: function() {
        var that = this;
        that._dispose();
        (0, _iterator.each)(that, (function(name) {
            that[name] = null
        }));
        return that
    },
    getOffset: function() {
        return _Number(this._options.offset) || 0
    }
});
exports.BaseElement = BaseElement;
var BaseIndicator = BaseElement.inherit({
    _init: function() {
        this._rootElement = this._createRoot().linkOn(this._owner, {
            name: "value-indicator",
            after: "core"
        });
        this._trackerElement = this._createTracker()
    },
    _dispose: function() {
        this._rootElement.linkOff()
    },
    _setupAnimation: function() {
        var that = this;
        if (that._options.animation) {
            that._animation = {
                step: function(pos) {
                    that._actualValue = that._animation.start + that._animation.delta * pos;
                    that._actualPosition = that._translator.translate(that._actualValue);
                    that._move()
                },
                duration: that._options.animation.duration > 0 ? _Number(that._options.animation.duration) : 0,
                easing: that._options.animation.easing
            }
        }
    },
    _runAnimation: function(value) {
        var animation = this._animation;
        animation.start = this._actualValue;
        animation.delta = value - this._actualValue;
        this._rootElement.animate({
            _: 0
        }, {
            step: animation.step,
            duration: animation.duration,
            easing: animation.easing
        })
    },
    _createRoot: function() {
        return this._renderer.g().attr({
            class: this._className
        })
    },
    _createTracker: function() {
        return this._renderer.path([], "area")
    },
    _getTrackerSettings: _common.noop,
    clean: function() {
        this._animation && this._rootElement.stopAnimation();
        this._rootElement.linkRemove().clear();
        this._clear();
        this._tracker.detach(this._trackerElement);
        this._options = this.enabled = this._animation = null;
        return this
    },
    render: function(options) {
        this.type = options.type;
        this._options = options;
        this._actualValue = this._currentValue = this._translator.adjust(this._options.currentValue);
        this.enabled = this._isEnabled();
        if (this.enabled) {
            this._setupAnimation();
            this._rootElement.attr({
                fill: this._options.color
            }).linkAppend();
            this._tracker.attach(this._trackerElement, this, this._trackerInfo)
        }
        return this
    },
    resize: function(layout) {
        this._rootElement.clear();
        this._clear();
        this.visible = this._isVisible(layout);
        if (this.visible) {
            (0, _extend.extend)(this._options, layout);
            this._actualPosition = this._translator.translate(this._actualValue);
            this._render();
            this._trackerElement.attr(this._getTrackerSettings());
            this._move()
        }
        return this
    },
    value: function(arg, _noAnimation) {
        var val;
        var rootElement = this._rootElement;
        var visibility = null;
        if (void 0 === arg) {
            return this._currentValue
        }
        if (null === arg) {
            visibility = "hidden";
            this._currentValue = arg
        } else {
            val = this._translator.adjust(arg);
            if (this._currentValue !== val && _isFinite(val)) {
                this._currentValue = val;
                if (this.visible) {
                    if (this._animation && !_noAnimation) {
                        this._runAnimation(val)
                    } else {
                        this._actualValue = val;
                        this._actualPosition = this._translator.translate(val);
                        this._move()
                    }
                }
            }
        }
        rootElement.attr({
            visibility: visibility
        });
        return this
    },
    _isEnabled: null,
    _isVisible: null,
    _render: null,
    _clear: null,
    _move: null
});
exports.BaseIndicator = BaseIndicator;
var COEFFICIENTS_MAP = {};
COEFFICIENTS_MAP["right-bottom"] = COEFFICIENTS_MAP.rb = [0, -1, -1, 0, 0, 1, 1, 0];
COEFFICIENTS_MAP["bottom-right"] = COEFFICIENTS_MAP.br = [-1, 0, 0, -1, 1, 0, 0, 1];
COEFFICIENTS_MAP["left-bottom"] = COEFFICIENTS_MAP.lb = [0, -1, 1, 0, 0, 1, -1, 0];
COEFFICIENTS_MAP["bottom-left"] = COEFFICIENTS_MAP.bl = [1, 0, 0, -1, -1, 0, 0, 1];
COEFFICIENTS_MAP["left-top"] = COEFFICIENTS_MAP.lt = [0, 1, 1, 0, 0, -1, -1, 0];
COEFFICIENTS_MAP["top-left"] = COEFFICIENTS_MAP.tl = [1, 0, 0, 1, -1, 0, 0, -1];
COEFFICIENTS_MAP["right-top"] = COEFFICIENTS_MAP.rt = [0, 1, -1, 0, 0, -1, 1, 0];
COEFFICIENTS_MAP["top-right"] = COEFFICIENTS_MAP.tr = [-1, 0, 0, 1, 1, 0, 0, -1];

function getTextCloudInfo(options) {
    var x = options.x;
    var y = options.y;
    var type = COEFFICIENTS_MAP[options.type];
    var cloudWidth = options.cloudWidth;
    var cloudHeight = options.cloudHeight;
    var tailWidth;
    var tailHeight;
    var cx = x;
    var cy = y;
    tailWidth = tailHeight = options.tailLength;
    if (1 & type[0]) {
        tailHeight = Math.min(tailHeight, cloudHeight / 3)
    } else {
        tailWidth = Math.min(tailWidth, cloudWidth / 3)
    }
    return {
        cx: _round(cx + type[0] * tailWidth + (type[0] + type[2]) * cloudWidth / 2),
        cy: _round(cy + type[1] * tailHeight + (type[1] + type[3]) * cloudHeight / 2),
        points: [_round(x), _round(y), _round(x += type[0] * (cloudWidth + tailWidth)), _round(y += type[1] * (cloudHeight + tailHeight)), _round(x += type[2] * cloudWidth), _round(y += type[3] * cloudHeight), _round(x += type[4] * cloudWidth), _round(y += type[5] * cloudHeight), _round(x += type[6] * (cloudWidth - tailWidth)), _round(y += type[7] * (cloudHeight - tailHeight))]
    }
}
var BaseTextCloudMarker = BaseIndicator.inherit({
    _move: function() {
        var options = this._options;
        var textCloudOptions = this._getTextCloudOptions();
        var text = _formatValue(this._actualValue, options.text);
        this._text.attr({
            text: text
        });
        var bBox = this._text.getBBox();
        var x = textCloudOptions.x;
        var y = textCloudOptions.y;
        var cloudWidth = (bBox.width || text.length * this._textUnitWidth) + 2 * options.horizontalOffset;
        var cloudHeight = (bBox.height || this._textHeight) + 2 * options.verticalOffset;
        var info = getTextCloudInfo({
            x: x,
            y: y,
            cloudWidth: cloudWidth,
            cloudHeight: cloudHeight,
            tailLength: options.arrowLength,
            type: this._correctCloudType(textCloudOptions.type, {
                x: x,
                y: y
            }, {
                width: cloudWidth,
                height: cloudHeight
            })
        });
        this._text.attr({
            x: info.cx,
            y: info.cy + this._textVerticalOffset
        });
        this._cloud.attr({
            points: info.points
        });
        this._trackerElement && this._trackerElement.attr({
            points: info.points
        })
    },
    _measureText: function() {
        var root;
        var text;
        var bBox;
        var sampleText;
        if (!this._textVerticalOffset) {
            root = this._createRoot().append(this._owner);
            sampleText = _getSampleText(this._translator, this._options.text);
            text = this._renderer.text(sampleText, 0, 0).attr({
                align: "center"
            }).css((0, _utils.patchFontOptions)(this._options.text.font)).append(root);
            bBox = text.getBBox();
            root.remove();
            this._textVerticalOffset = -bBox.y - bBox.height / 2;
            this._textWidth = bBox.width;
            this._textHeight = bBox.height;
            this._textUnitWidth = this._textWidth / sampleText.length;
            this._textFullWidth = this._textWidth + 2 * this._options.horizontalOffset;
            this._textFullHeight = this._textHeight + 2 * this._options.verticalOffset
        }
    },
    _render: function() {
        this._measureText();
        this._cloud = this._cloud || this._renderer.path([], "area").append(this._rootElement);
        this._text = this._text || this._renderer.text().append(this._rootElement);
        this._text.attr({
            align: "center"
        }).css((0, _utils.patchFontOptions)(this._options.text.font))
    },
    _clear: function() {
        delete this._cloud;
        delete this._text
    },
    getTooltipParameters: function() {
        var position = this._getTextCloudOptions();
        return {
            x: position.x,
            y: position.y,
            value: this._currentValue,
            color: this._options.color
        }
    },
    _correctCloudType: function(type) {
        return type
    }
});
exports.BaseTextCloudMarker = BaseTextCloudMarker;
var BaseRangeBar = BaseIndicator.inherit({
    _measureText: function() {
        var root;
        var text;
        var bBox;
        this._hasText = this._isTextVisible();
        if (this._hasText && !this._textVerticalOffset) {
            root = this._createRoot().append(this._owner);
            text = this._renderer.text(_getSampleText(this._translator, this._options.text), 0, 0).attr({
                class: "dxg-text",
                align: "center"
            }).css((0, _utils.patchFontOptions)(this._options.text.font)).append(root);
            bBox = text.getBBox();
            root.remove();
            this._textVerticalOffset = -bBox.y - bBox.height / 2;
            this._textWidth = bBox.width;
            this._textHeight = bBox.height
        }
    },
    _move: function() {
        this._updateBarItemsPositions();
        if (this._hasText) {
            this._text.attr({
                text: _formatValue(this._actualValue, this._options.text)
            });
            this._updateTextPosition();
            this._updateLinePosition()
        }
    },
    _updateBarItems: function() {
        var options = this._options;
        var spaceColor;
        var translator = this._translator;
        this._setBarSides();
        this._startPosition = translator.translate(translator.getDomainStart());
        this._endPosition = translator.translate(translator.getDomainEnd());
        this._basePosition = translator.translate(options.baseValue);
        this._space = this._getSpace();
        var backgroundColor = options.backgroundColor || "none";
        if ("none" !== backgroundColor && this._space > 0) {
            spaceColor = options.containerBackgroundColor || "none"
        } else {
            this._space = 0;
            spaceColor = "none"
        }
        this._backItem1.attr({
            fill: backgroundColor
        });
        this._backItem2.attr({
            fill: backgroundColor
        });
        this._spaceItem1.attr({
            fill: spaceColor
        });
        this._spaceItem2.attr({
            fill: spaceColor
        })
    },
    _getSpace: function() {
        return 0
    },
    _updateTextItems: function() {
        if (this._hasText) {
            this._line = this._line || this._renderer.path([], "line").attr({
                class: "dxg-main-bar",
                "stroke-linecap": "square"
            }).append(this._rootElement);
            this._text = this._text || this._renderer.text("", 0, 0).attr({
                class: "dxg-text"
            }).append(this._rootElement);
            this._text.attr({
                align: this._getTextAlign()
            }).css(this._getFontOptions());
            this._setTextItemsSides()
        } else {
            if (this._line) {
                this._line.remove();
                delete this._line
            }
            if (this._text) {
                this._text.remove();
                delete this._text
            }
        }
    },
    _isTextVisible: function() {
        return false
    },
    _getTextAlign: function() {
        return "center"
    },
    _getFontOptions: function() {
        var options = this._options;
        var font = options.text.font;
        if (!font || !font.color) {
            font = (0, _extend.extend)({}, font, {
                color: options.color
            })
        }
        return (0, _utils.patchFontOptions)(font)
    },
    _updateBarItemsPositions: function() {
        var positions = this._getPositions();
        this._backItem1.attr(this._buildItemSettings(positions.start, positions.back1));
        this._backItem2.attr(this._buildItemSettings(positions.back2, positions.end));
        this._spaceItem1.attr(this._buildItemSettings(positions.back1, positions.main1));
        this._spaceItem2.attr(this._buildItemSettings(positions.main2, positions.back2));
        this._mainItem.attr(this._buildItemSettings(positions.main1, positions.main2));
        this._trackerElement && this._trackerElement.attr(this._buildItemSettings(positions.main1, positions.main2))
    },
    _render: function() {
        this._measureText();
        if (!this._backItem1) {
            this._backItem1 = this._createBarItem();
            this._backItem1.attr({
                class: "dxg-back-bar"
            })
        }
        if (!this._backItem2) {
            this._backItem2 = this._createBarItem();
            this._backItem2.attr({
                class: "dxg-back-bar"
            })
        }
        if (!this._spaceItem1) {
            this._spaceItem1 = this._createBarItem();
            this._spaceItem1.attr({
                class: "dxg-space-bar"
            })
        }
        if (!this._spaceItem2) {
            this._spaceItem2 = this._createBarItem();
            this._spaceItem2.attr({
                class: "dxg-space-bar"
            })
        }
        if (!this._mainItem) {
            this._mainItem = this._createBarItem();
            this._mainItem.attr({
                class: "dxg-main-bar"
            })
        }
        this._updateBarItems();
        this._updateTextItems()
    },
    _clear: function() {
        delete this._backItem1;
        delete this._backItem2;
        delete this._spaceItem1;
        delete this._spaceItem2;
        delete this._mainItem;
        delete this._hasText;
        delete this._line;
        delete this._text
    },
    getTooltipParameters: function() {
        var position = this._getTooltipPosition();
        return {
            x: position.x,
            y: position.y,
            value: this._currentValue,
            color: this._options.color,
            offset: 0
        }
    }
});
exports.BaseRangeBar = BaseRangeBar;
