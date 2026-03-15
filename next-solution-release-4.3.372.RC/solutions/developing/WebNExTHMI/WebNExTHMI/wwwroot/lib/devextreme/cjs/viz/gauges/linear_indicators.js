/**
 * DevExtreme (cjs/viz/gauges/linear_indicators.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.trianglemarker = exports.textcloud = exports.rhombus = exports.rectangle = exports.rangebar = exports.circle = exports._default = void 0;
var _base_indicators = require("./base_indicators");
var _utils = require("../core/utils");
var _Number = Number;
var SimpleIndicator = _base_indicators.BaseIndicator.inherit({
    _move: function() {
        var delta = this._actualPosition - this._zeroPosition;
        this._rootElement.move(this.vertical ? 0 : delta, this.vertical ? delta : 0);
        this._trackerElement && this._trackerElement.move(this.vertical ? 0 : delta, this.vertical ? delta : 0)
    },
    _isEnabled: function() {
        this.vertical = this._options.vertical;
        return this._options.length > 0 && this._options.width > 0
    },
    _isVisible: function() {
        return true
    },
    _getTrackerSettings: function() {
        var options = this._options;
        var x1;
        var x2;
        var y1;
        var y2;
        var width = options.width / 2;
        var length = options.length / 2;
        var p = this._zeroPosition;
        width > 10 || (width = 10);
        length > 10 || (length = 10);
        if (this.vertical) {
            x1 = options.x - length;
            x2 = options.x + length;
            y1 = p + width;
            y2 = p - width
        } else {
            x1 = p - width;
            x2 = p + width;
            y1 = options.y + length;
            y2 = options.y - length
        }
        return {
            points: [x1, y1, x1, y2, x2, y2, x2, y1]
        }
    },
    _render: function() {
        this._zeroPosition = this._translator.getCodomainStart()
    },
    _clear: function() {
        delete this._element
    },
    measure: function(layout) {
        var p = this.vertical ? layout.x : layout.y;
        return {
            min: p - this._options.length / 2,
            max: p + this._options.length / 2
        }
    },
    getTooltipParameters: function() {
        var options = this._options;
        var p = this._actualPosition;
        var parameters = {
            x: p,
            y: p,
            value: this._currentValue,
            color: options.color,
            offset: options.width / 2
        };
        this.vertical ? parameters.x = options.x : parameters.y = options.y;
        return parameters
    }
});
var rectangle = SimpleIndicator.inherit({
    _render: function() {
        var options = this._options;
        var x1;
        var x2;
        var y1;
        var y2;
        this.callBase();
        var p = this._zeroPosition;
        if (this.vertical) {
            x1 = options.x - options.length / 2;
            x2 = options.x + options.length / 2;
            y1 = p + options.width / 2;
            y2 = p - options.width / 2
        } else {
            x1 = p - options.width / 2;
            x2 = p + options.width / 2;
            y1 = options.y + options.length / 2;
            y2 = options.y - options.length / 2
        }
        this._element = this._element || this._renderer.path([], "area").append(this._rootElement);
        this._element.attr({
            points: [x1, y1, x1, y2, x2, y2, x2, y1]
        })
    }
});
exports.rectangle = rectangle;
var rhombus = SimpleIndicator.inherit({
    _render: function() {
        var options = this._options;
        var x;
        var y;
        var dx;
        var dy;
        this.callBase();
        if (this.vertical) {
            x = options.x;
            y = this._zeroPosition;
            dx = options.length / 2 || 0;
            dy = options.width / 2 || 0
        } else {
            x = this._zeroPosition;
            y = options.y;
            dx = options.width / 2 || 0;
            dy = options.length / 2 || 0
        }
        this._element = this._element || this._renderer.path([], "area").append(this._rootElement);
        this._element.attr({
            points: [x - dx, y, x, y - dy, x + dx, y, x, y + dy]
        })
    }
});
exports.rhombus = rhombus;
var circle = SimpleIndicator.inherit({
    _render: function() {
        var options = this._options;
        var x;
        var y;
        this.callBase();
        if (this.vertical) {
            x = options.x;
            y = this._zeroPosition
        } else {
            x = this._zeroPosition;
            y = options.y
        }
        var r = options.length / 2 || 0;
        this._element = this._element || this._renderer.circle().append(this._rootElement);
        this._element.attr({
            cx: x,
            cy: y,
            r: r
        })
    }
});
exports.circle = circle;
var triangleMarker = SimpleIndicator.inherit({
    _isEnabled: function() {
        this.vertical = this._options.vertical;
        this._inverted = this.vertical ? "right" === (0, _utils.normalizeEnum)(this._options.horizontalOrientation) : "bottom" === (0, _utils.normalizeEnum)(this._options.verticalOrientation);
        return this._options.length > 0 && this._options.width > 0
    },
    _isVisible: function() {
        return true
    },
    _render: function() {
        var options = this._options;
        var x1;
        var x2;
        var y1;
        var y2;
        var settings = {
            stroke: "none",
            "stroke-width": 0,
            "stroke-linecap": "square"
        };
        this.callBase();
        if (this.vertical) {
            x1 = options.x;
            y1 = this._zeroPosition;
            x2 = x1 + _Number(this._inverted ? options.length : -options.length);
            settings.points = [x1, y1, x2, y1 - options.width / 2, x2, y1 + options.width / 2]
        } else {
            y1 = options.y;
            x1 = this._zeroPosition;
            y2 = y1 + _Number(this._inverted ? options.length : -options.length);
            settings.points = [x1, y1, x1 - options.width / 2, y2, x1 + options.width / 2, y2]
        }
        if (options.space > 0) {
            settings["stroke-width"] = Math.min(options.space, options.width / 4) || 0;
            settings.stroke = settings["stroke-width"] > 0 ? options.containerBackgroundColor || "none" : "none"
        }
        this._element = this._element || this._renderer.path([], "area").append(this._rootElement);
        this._element.attr(settings).sharp()
    },
    _getTrackerSettings: function() {
        var options = this._options;
        var width = options.width / 2;
        var length = _Number(options.length);
        var x1;
        var x2;
        var y1;
        var y2;
        var result;
        width > 10 || (width = 10);
        length > 20 || (length = 20);
        if (this.vertical) {
            x1 = options.x;
            x2 = x1 + (this._inverted ? length : -length);
            y1 = this._zeroPosition + width;
            y2 = this._zeroPosition - width;
            result = [x1, y1, x2, y1, x2, y2, x1, y2]
        } else {
            y1 = options.y;
            y2 = y1 + (this._inverted ? length : -length);
            x1 = this._zeroPosition - width;
            x2 = this._zeroPosition + width;
            result = [x1, y1, x1, y2, x2, y2, x2, y1]
        }
        return {
            points: result
        }
    },
    measure: function(layout) {
        var length = _Number(this._options.length);
        var minBound;
        var maxBound;
        if (this.vertical) {
            minBound = maxBound = layout.x;
            if (this._inverted) {
                maxBound = minBound + length
            } else {
                minBound = maxBound - length
            }
        } else {
            minBound = maxBound = layout.y;
            if (this._inverted) {
                maxBound = minBound + length
            } else {
                minBound = maxBound - length
            }
        }
        return {
            min: minBound,
            max: maxBound,
            indent: this._options.width / 2
        }
    },
    getTooltipParameters: function() {
        var options = this._options;
        var s = (this._inverted ? options.length : -options.length) / 2;
        var parameters = this.callBase();
        this.vertical ? parameters.x += s : parameters.y += s;
        parameters.offset = options.length / 2;
        return parameters
    }
});
exports.trianglemarker = triangleMarker;
var textCloud = _base_indicators.BaseTextCloudMarker.inherit({
    _isEnabled: function() {
        this.vertical = this._options.vertical;
        this._inverted = this.vertical ? "right" === (0, _utils.normalizeEnum)(this._options.horizontalOrientation) : "bottom" === (0, _utils.normalizeEnum)(this._options.verticalOrientation);
        return true
    },
    _isVisible: function() {
        return true
    },
    _getTextCloudOptions: function() {
        var x = this._actualPosition;
        var y = this._actualPosition;
        var type;
        if (this.vertical) {
            x = this._options.x;
            type = this._inverted ? "top-left" : "top-right"
        } else {
            y = this._options.y;
            type = this._inverted ? "right-top" : "right-bottom"
        }
        return {
            x: x,
            y: y,
            type: type
        }
    },
    measure: function(layout) {
        var minBound;
        var maxBound;
        var arrowLength = _Number(this._options.arrowLength) || 0;
        this._measureText();
        if (this.vertical) {
            if (this._inverted) {
                minBound = layout.x;
                maxBound = layout.x + arrowLength + this._textFullWidth
            } else {
                minBound = layout.x - arrowLength - this._textFullWidth;
                maxBound = layout.x
            }
        } else if (this._inverted) {
            minBound = layout.y;
            maxBound = layout.y + arrowLength + this._textFullHeight
        } else {
            minBound = layout.y - arrowLength - this._textFullHeight;
            maxBound = layout.y
        }
        return {
            min: minBound,
            max: maxBound,
            indent: 0
        }
    },
    _correctCloudType: function(type, _ref, _ref2) {
        var x = _ref.x,
            y = _ref.y;
        var width = _ref2.width,
            height = _ref2.height;
        if ("right-top" === type || "right-bottom" === type) {
            if (x - width < this._translator.getCodomainStart()) {
                type = "left-".concat(type.split("-")[1])
            }
        } else if ("top-left" === type || "top-right" === type) {
            if (y + height > this._translator.getCodomainStart()) {
                type = "bottom-".concat(type.split("-")[1])
            }
        }
        return type
    }
});
exports.textcloud = textCloud;
var rangeBar = _base_indicators.BaseRangeBar.inherit({
    _isEnabled: function() {
        this.vertical = this._options.vertical;
        this._inverted = this.vertical ? "right" === (0, _utils.normalizeEnum)(this._options.horizontalOrientation) : "bottom" === (0, _utils.normalizeEnum)(this._options.verticalOrientation);
        return this._options.size > 0
    },
    _isVisible: function() {
        return true
    },
    _createBarItem: function() {
        return this._renderer.path([], "area").append(this._rootElement)
    },
    _createTracker: function() {
        return this._renderer.path([], "area")
    },
    _setBarSides: function() {
        var options = this._options;
        var size = _Number(options.size);
        var minSide;
        var maxSide;
        if (this.vertical) {
            if (this._inverted) {
                minSide = options.x;
                maxSide = options.x + size
            } else {
                minSide = options.x - size;
                maxSide = options.x
            }
        } else if (this._inverted) {
            minSide = options.y;
            maxSide = options.y + size
        } else {
            minSide = options.y - size;
            maxSide = options.y
        }
        this._minSide = minSide;
        this._maxSide = maxSide;
        this._minBound = minSide;
        this._maxBound = maxSide
    },
    _getSpace: function() {
        var options = this._options;
        return options.space > 0 ? _Number(options.space) : 0
    },
    _isTextVisible: function() {
        var textOptions = this._options.text || {};
        return textOptions.indent > 0 || textOptions.indent < 0
    },
    _getTextAlign: function() {
        return this.vertical ? this._options.text.indent > 0 ? "left" : "right" : "center"
    },
    _setTextItemsSides: function() {
        var indent = _Number(this._options.text.indent);
        if (indent > 0) {
            this._lineStart = this._maxSide;
            this._lineEnd = this._maxSide + indent;
            this._textPosition = this._lineEnd + (this.vertical ? 2 : this._textHeight / 2);
            this._maxBound = this._textPosition + (this.vertical ? this._textWidth : this._textHeight / 2)
        } else if (indent < 0) {
            this._lineStart = this._minSide;
            this._lineEnd = this._minSide + indent;
            this._textPosition = this._lineEnd - (this.vertical ? 2 : this._textHeight / 2);
            this._minBound = this._textPosition - (this.vertical ? this._textWidth : this._textHeight / 2)
        }
    },
    _getPositions: function() {
        var startPosition = this._startPosition;
        var endPosition = this._endPosition;
        var space = this._space;
        var basePosition = this._basePosition;
        var actualPosition = this._actualPosition;
        var mainPosition1;
        var mainPosition2;
        var backPosition1;
        var backPosition2;
        if (startPosition < endPosition) {
            if (basePosition < actualPosition) {
                mainPosition1 = basePosition;
                mainPosition2 = actualPosition
            } else {
                mainPosition1 = actualPosition;
                mainPosition2 = basePosition
            }
            backPosition1 = mainPosition1 - space;
            backPosition2 = mainPosition2 + space
        } else {
            if (basePosition > actualPosition) {
                mainPosition1 = basePosition;
                mainPosition2 = actualPosition
            } else {
                mainPosition1 = actualPosition;
                mainPosition2 = basePosition
            }
            backPosition1 = mainPosition1 + space;
            backPosition2 = mainPosition2 - space
        }
        return {
            start: startPosition,
            end: endPosition,
            main1: mainPosition1,
            main2: mainPosition2,
            back1: backPosition1,
            back2: backPosition2
        }
    },
    _buildItemSettings: function(from, to) {
        var side1 = this._minSide;
        var side2 = this._maxSide;
        var points = this.vertical ? [side1, from, side1, to, side2, to, side2, from] : [from, side1, from, side2, to, side2, to, side1];
        return {
            points: points
        }
    },
    _updateTextPosition: function() {
        this._text.attr(this.vertical ? {
            x: this._textPosition,
            y: this._actualPosition + this._textVerticalOffset
        } : {
            x: this._actualPosition,
            y: this._textPosition + this._textVerticalOffset
        })
    },
    _updateLinePosition: function() {
        var actualPosition = this._actualPosition;
        var side1;
        var side2;
        var points;
        if (this.vertical) {
            if (this._basePosition >= actualPosition) {
                side1 = actualPosition;
                side2 = actualPosition + 2
            } else {
                side1 = actualPosition - 2;
                side2 = actualPosition
            }
            points = [this._lineStart, side1, this._lineStart, side2, this._lineEnd, side2, this._lineEnd, side1]
        } else {
            if (this._basePosition <= actualPosition) {
                side1 = actualPosition - 2;
                side2 = actualPosition
            } else {
                side1 = actualPosition;
                side2 = actualPosition + 2
            }
            points = [side1, this._lineStart, side1, this._lineEnd, side2, this._lineEnd, side2, this._lineStart]
        }
        this._line.attr({
            points: points
        }).sharp()
    },
    _getTooltipPosition: function() {
        var crossCenter = (this._minSide + this._maxSide) / 2;
        var alongCenter = (this._basePosition + this._actualPosition) / 2;
        return this.vertical ? {
            x: crossCenter,
            y: alongCenter
        } : {
            x: alongCenter,
            y: crossCenter
        }
    },
    measure: function(layout) {
        var size = _Number(this._options.size);
        var textIndent = _Number(this._options.text.indent);
        var minBound;
        var maxBound;
        var indent;
        this._measureText();
        if (this.vertical) {
            minBound = maxBound = layout.x;
            if (this._inverted) {
                maxBound += size
            } else {
                minBound -= size
            }
            if (this._hasText) {
                indent = this._textHeight / 2;
                if (textIndent > 0) {
                    maxBound += textIndent + this._textWidth
                }
                if (textIndent < 0) {
                    minBound += textIndent - this._textWidth
                }
            }
        } else {
            minBound = maxBound = layout.y;
            if (this._inverted) {
                maxBound += size
            } else {
                minBound -= size
            }
            if (this._hasText) {
                indent = this._textWidth / 2;
                if (textIndent > 0) {
                    maxBound += textIndent + this._textHeight
                }
                if (textIndent < 0) {
                    minBound += textIndent - this._textHeight
                }
            }
        }
        return {
            min: minBound,
            max: maxBound,
            indent: indent
        }
    }
});
exports.rangebar = exports._default = rangeBar;
