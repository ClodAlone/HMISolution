/**
 * DevExtreme (esm/viz/gauges/base_gauge.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
var _Number = Number;
import {
    getAppropriateFormat as _getAppropriateFormat
} from "../core/utils";
import {
    extend
} from "../../core/utils/extend";
import {
    Translator1D
} from "../translators/translator1d";
var _extend = extend;
import BaseWidget from "../core/base_widget";
import themeManagerModule from "./theme_manager";
import Tracker from "./tracker";
import formatHelper from "../../format_helper";
import {
    plugin as exportPlugin
} from "../core/export";
import {
    plugin as titlePlugin
} from "../core/title";
import {
    plugin as tooltipPlugin
} from "../core/tooltip";
import {
    plugin as loadingIndicatorPlugin
} from "../core/loading_indicator";
var _format = formatHelper.format;
export var BaseGauge = BaseWidget.inherit({
    _rootClassPrefix: "dxg",
    _themeSection: "gauge",
    _createThemeManager: function() {
        return new themeManagerModule.ThemeManager(this._getThemeManagerOptions())
    },
    _initCore: function() {
        var root = this._renderer.root;
        this._valueChangingLocker = 0;
        this._translator = this._factory.createTranslator();
        this._tracker = this._factory.createTracker({
            renderer: this._renderer,
            container: root
        });
        this._setTrackerCallbacks()
    },
    _beginValueChanging: function() {
        this._resetIsReady();
        this._onBeginUpdate();
        ++this._valueChangingLocker
    },
    _endValueChanging: function() {
        if (0 === --this._valueChangingLocker) {
            this._drawn()
        }
    },
    _setTrackerCallbacks: function() {
        var renderer = this._renderer;
        var tooltip = this._tooltip;
        this._tracker.setCallbacks({
            "tooltip-show": function(target, info, callback) {
                var tooltipParameters = target.getTooltipParameters();
                var offset = renderer.getRootOffset();
                var formatObject = _extend({
                    value: tooltipParameters.value,
                    valueText: tooltip.formatValue(tooltipParameters.value),
                    color: tooltipParameters.color
                }, info);
                return tooltip.show(formatObject, {
                    x: tooltipParameters.x + offset.left,
                    y: tooltipParameters.y + offset.top,
                    offset: tooltipParameters.offset
                }, {
                    target: info
                }, void 0, callback)
            },
            "tooltip-hide": function() {
                return tooltip.hide()
            }
        })
    },
    _dispose: function() {
        this._cleanCore();
        this.callBase.apply(this, arguments)
    },
    _disposeCore: function() {
        this._themeManager.dispose();
        this._tracker.dispose();
        this._translator = this._tracker = null
    },
    _cleanCore: function() {
        this._tracker.deactivate();
        this._cleanContent()
    },
    _renderCore: function() {
        if (!this._isValidDomain) {
            return
        }
        this._renderContent();
        this._tracker.setTooltipState(this._tooltip.isEnabled());
        this._tracker.activate();
        this._noAnimation = false
    },
    _applyChanges: function() {
        this.callBase.apply(this, arguments);
        this._resizing = this._noAnimation = false
    },
    _setContentSize: function() {
        var that = this;
        that._resizing = that._noAnimation = 2 === that._changes.count();
        that.callBase.apply(that, arguments)
    },
    _applySize: function(rect) {
        this._innerRect = {
            left: rect[0],
            top: rect[1],
            right: rect[2],
            bottom: rect[3]
        };
        var layoutCache = this._layout._cache;
        this._cleanCore();
        this._renderCore();
        this._layout._cache = this._layout._cache || layoutCache;
        return [rect[0], this._innerRect.top, rect[2], this._innerRect.bottom]
    },
    _initialChanges: ["DOMAIN"],
    _themeDependentChanges: ["DOMAIN"],
    _optionChangesMap: {
        subtitle: "MOSTLY_TOTAL",
        indicator: "MOSTLY_TOTAL",
        geometry: "MOSTLY_TOTAL",
        animation: "MOSTLY_TOTAL",
        startValue: "DOMAIN",
        endValue: "DOMAIN"
    },
    _optionChangesOrder: ["DOMAIN", "MOSTLY_TOTAL"],
    _change_DOMAIN: function() {
        this._setupDomain()
    },
    _change_MOSTLY_TOTAL: function() {
        this._applyMostlyTotalChange()
    },
    _setupDomain: function() {
        this._setupDomainCore();
        this._isValidDomain = isFinite(1 / (this._translator.getDomain()[1] - this._translator.getDomain()[0]));
        if (!this._isValidDomain) {
            this._incidentOccurred("W2301")
        }
        this._change(["MOSTLY_TOTAL"])
    },
    _applyMostlyTotalChange: function() {
        this._setupCodomain();
        this._setupAnimationSettings();
        this._setupDefaultFormat();
        this._change(["LAYOUT"])
    },
    _setupAnimationSettings: function() {
        var option = this.option("animation");
        this._animationSettings = null;
        if (void 0 === option || option) {
            option = _extend({
                enabled: true,
                duration: 1e3,
                easing: "easeOutCubic"
            }, option);
            if (option.enabled && option.duration > 0) {
                this._animationSettings = {
                    duration: _Number(option.duration),
                    easing: option.easing
                }
            }
        }
        this._containerBackgroundColor = this.option("containerBackgroundColor") || this._themeManager.theme().containerBackgroundColor
    },
    _setupDefaultFormat: function() {
        var domain = this._translator.getDomain();
        this._defaultFormatOptions = _getAppropriateFormat(domain[0], domain[1], this._getApproximateScreenRange())
    },
    _setupDomainCore: null,
    _calculateSize: null,
    _cleanContent: null,
    _renderContent: null,
    _setupCodomain: null,
    _getApproximateScreenRange: null,
    _factory: {
        createTranslator: function() {
            return new Translator1D
        },
        createTracker: function(parameters) {
            return new Tracker(parameters)
        }
    }
});
export var formatValue = function(value, options, extra) {
    options = options || {};
    var text = _format(value, options.format);
    var formatObject;
    if ("function" === typeof options.customizeText) {
        formatObject = _extend({
            value: value,
            valueText: text
        }, extra);
        return String(options.customizeText.call(formatObject, formatObject))
    }
    return text
};
export var getSampleText = function(translator, options) {
    var text1 = formatValue(translator.getDomainStart(), options);
    var text2 = formatValue(translator.getDomainEnd(), options);
    return text1.length >= text2.length ? text1 : text2
};
export function compareArrays(array1, array2) {
    return array1 && array2 && array1.length === array2.length && compareArraysElements(array1, array2)
}

function compareArraysElements(array1, array2) {
    var i;
    var ii = array1.length;
    var array1ValueIsNaN;
    var array2ValueIsNaN;
    for (i = 0; i < ii; ++i) {
        array1ValueIsNaN = array1[i] !== array1[i];
        array2ValueIsNaN = array2[i] !== array2[i];
        if (array1ValueIsNaN && array2ValueIsNaN) {
            continue
        }
        if (array1[i] !== array2[i]) {
            return false
        }
    }
    return true
}
BaseGauge.addPlugin(exportPlugin);
BaseGauge.addPlugin(titlePlugin);
BaseGauge.addPlugin(tooltipPlugin);
BaseGauge.addPlugin(loadingIndicatorPlugin);
var _setTooltipOptions = BaseGauge.prototype._setTooltipOptions;
BaseGauge.prototype._setTooltipOptions = function() {
    _setTooltipOptions.apply(this, arguments);
    this._tracker && this._tracker.setTooltipState(this._tooltip.isEnabled())
};
