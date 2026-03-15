/**
 * DevExtreme (esm/viz/sparklines/base_sparkline.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import eventsEngine from "../../events/core/events_engine";
import domAdapter from "../../core/dom_adapter";
import {
    isFunction
} from "../../core/utils/type";
import BaseWidget from "../core/base_widget";
import {
    extend
} from "../../core/utils/extend";
import {
    addNamespace
} from "../../events/utils/index";
import pointerEvents from "../../events/pointer";
import {
    pointInCanvas
} from "../core/utils";
var DEFAULT_LINE_SPACING = 2;
var EVENT_NS = "sparkline-tooltip";
var POINTER_ACTION = addNamespace([pointerEvents.down, pointerEvents.move], EVENT_NS);
import {
    Translator2D
} from "../translators/translator2d";
var _extend = extend;
var _floor = Math.floor;
import {
    noop as _noop
} from "../../core/utils/common";

function inCanvas(_ref, x, y) {
    var {
        width: width,
        height: height
    } = _ref;
    return pointInCanvas({
        left: 0,
        top: 0,
        right: width,
        bottom: height,
        width: width,
        height: height
    }, x, y)
}

function pointerHandler(_ref2) {
    var {
        data: data
    } = _ref2;
    var that = data.widget;
    that._enableOutHandler();
    that._showTooltip()
}

function generateDefaultCustomizeTooltipCallback(fontOptions, rtlEnabled) {
    var lineSpacing = fontOptions.lineSpacing;
    var lineHeight = (void 0 !== lineSpacing && null !== lineSpacing ? lineSpacing : DEFAULT_LINE_SPACING) + fontOptions.size;
    return function(customizeObject) {
        var html = "";
        var vt = customizeObject.valueText;
        for (var i = 0; i < vt.length; i += 2) {
            html += "<tr><td>" + vt[i] + "</td><td style='width: 15px'></td><td style='text-align: " + (rtlEnabled ? "left" : "right") + "'>" + vt[i + 1] + "</td></tr>"
        }
        return {
            html: "<table style='border-spacing:0px; line-height: " + lineHeight + "px'>" + html + "</table>"
        }
    }
}

function generateCustomizeTooltipCallback(customizeTooltip, fontOptions, rtlEnabled) {
    var defaultCustomizeTooltip = generateDefaultCustomizeTooltipCallback(fontOptions, rtlEnabled);
    if (isFunction(customizeTooltip)) {
        return function(customizeObject) {
            var res = customizeTooltip.call(customizeObject, customizeObject);
            if (!("html" in res) && !("text" in res)) {
                _extend(res, defaultCustomizeTooltip.call(customizeObject, customizeObject))
            }
            return res
        }
    } else {
        return defaultCustomizeTooltip
    }
}

function createAxis(isHorizontal) {
    var translator = new Translator2D({}, {}, {
        shiftZeroValue: !isHorizontal,
        isHorizontal: !!isHorizontal
    });
    return {
        getTranslator: function() {
            return translator
        },
        update: function(range, canvas, options) {
            translator.update(range, canvas, options)
        },
        getVisibleArea() {
            var visibleArea = translator.getCanvasVisibleArea();
            return [visibleArea.min, visibleArea.max]
        },
        visualRange: _noop,
        calculateInterval: _noop,
        getMarginOptions: () => ({})
    }
}
var _initTooltip;
var BaseSparkline = BaseWidget.inherit({
    _getLayoutItems: _noop,
    _useLinks: false,
    _themeDependentChanges: ["OPTIONS"],
    _initCore: function() {
        this._tooltipTracker = this._renderer.root;
        this._tooltipTracker.attr({
            "pointer-events": "visible"
        });
        this._createHtmlElements();
        this._initTooltipEvents();
        this._argumentAxis = createAxis(true);
        this._valueAxis = createAxis()
    },
    _getDefaultSize: function() {
        return this._defaultSize
    },
    _disposeCore: function() {
        this._disposeWidgetElements();
        this._disposeTooltipEvents();
        this._ranges = null
    },
    _optionChangesOrder: ["OPTIONS"],
    _change_OPTIONS: function() {
        this._prepareOptions();
        this._change(["UPDATE"])
    },
    _customChangesOrder: ["UPDATE"],
    _change_UPDATE: function() {
        this._update()
    },
    _update: function() {
        if (this._tooltipShown) {
            this._tooltipShown = false;
            this._tooltip.hide()
        }
        this._cleanWidgetElements();
        this._updateWidgetElements();
        this._drawWidgetElements()
    },
    _updateWidgetElements: function() {
        var canvas = this._getCorrectCanvas();
        this._updateRange();
        this._argumentAxis.update(this._ranges.arg, canvas, this._getStick());
        this._valueAxis.update(this._ranges.val, canvas)
    },
    _getStick: function() {},
    _applySize: function(rect) {
        this._allOptions.size = {
            width: rect[2] - rect[0],
            height: rect[3] - rect[1]
        };
        this._change(["UPDATE"])
    },
    _setupResizeHandler: _noop,
    _prepareOptions: function() {
        return _extend(true, {}, this._themeManager.theme(), this.option())
    },
    _getTooltipCoords: function() {
        var canvas = this._canvas;
        var rootOffset = this._renderer.getRootOffset();
        return {
            x: canvas.width / 2 + rootOffset.left,
            y: canvas.height / 2 + rootOffset.top
        }
    },
    _initTooltipEvents() {
        var data = {
            widget: this
        };
        this._renderer.root.off("." + EVENT_NS).on(POINTER_ACTION, data, pointerHandler)
    },
    _showTooltip() {
        var tooltip;
        if (!this._tooltipShown) {
            this._tooltipShown = true;
            tooltip = this._getTooltip();
            tooltip.isEnabled() && this._tooltip.show(this._getTooltipData(), this._getTooltipCoords(), {})
        }
    },
    _hideTooltip() {
        if (this._tooltipShown) {
            this._tooltipShown = false;
            this._tooltip.hide()
        }
    },
    _stopCurrentHandling() {
        this._hideTooltip()
    },
    _enableOutHandler() {
        var that = this;
        if (that._outHandler) {
            return
        }
        var handler = _ref3 => {
            var {
                pageX: pageX,
                pageY: pageY
            } = _ref3;
            var {
                left: left,
                top: top
            } = that._renderer.getRootOffset();
            var x = _floor(pageX - left);
            var y = _floor(pageY - top);
            if (!inCanvas(that._canvas, x, y)) {
                that._hideTooltip();
                that._disableOutHandler()
            }
        };
        eventsEngine.on(domAdapter.getDocument(), POINTER_ACTION, handler);
        this._outHandler = handler
    },
    _disableOutHandler() {
        this._outHandler && eventsEngine.off(domAdapter.getDocument(), POINTER_ACTION, this._outHandler);
        this._outHandler = null
    },
    _disposeTooltipEvents: function() {
        this._tooltipTracker.off();
        this._disableOutHandler();
        this._renderer.root.off("." + EVENT_NS)
    },
    _getTooltip: function() {
        var that = this;
        if (!that._tooltip) {
            _initTooltip.apply(this, arguments);
            that._setTooltipRendererOptions(that._tooltipRendererOptions);
            that._tooltipRendererOptions = null;
            that._setTooltipOptions()
        }
        return that._tooltip
    }
});
export default BaseSparkline;
import {
    plugin as tooltipPlugin
} from "../core/tooltip";
BaseSparkline.addPlugin(tooltipPlugin);
_initTooltip = BaseSparkline.prototype._initTooltip;
BaseSparkline.prototype._initTooltip = _noop;
var _disposeTooltip = BaseSparkline.prototype._disposeTooltip;
BaseSparkline.prototype._disposeTooltip = function() {
    if (this._tooltip) {
        _disposeTooltip.apply(this, arguments)
    }
};
BaseSparkline.prototype._setTooltipRendererOptions = function() {
    var options = this._getRendererOptions();
    if (this._tooltip) {
        this._tooltip.setRendererOptions(options)
    } else {
        this._tooltipRendererOptions = options
    }
};
BaseSparkline.prototype._setTooltipOptions = function() {
    var tooltip = this._tooltip;
    var options = tooltip && this._getOption("tooltip");
    tooltip && tooltip.update(_extend({}, options, {
        customizeTooltip: generateCustomizeTooltipCallback(options.customizeTooltip, options.font, this.option("rtlEnabled")),
        enabled: options.enabled && this._isTooltipEnabled()
    }))
};
import {
    plugin
} from "../core/export";
var exportPlugin = extend(true, {}, plugin, {
    init: _noop,
    dispose: _noop,
    customize: null,
    members: {
        _getExportMenuOptions: null
    }
});
BaseSparkline.addPlugin(exportPlugin);
