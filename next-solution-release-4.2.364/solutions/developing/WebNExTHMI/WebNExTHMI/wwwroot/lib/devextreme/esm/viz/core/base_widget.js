/**
 * DevExtreme (esm/viz/core/base_widget.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    noop
} from "../../core/utils/common";
import {
    getWindow,
    hasWindow
} from "../../core/utils/window";
import domAdapter from "../../core/dom_adapter";
import {
    isNumeric,
    isFunction,
    isDefined,
    isObject as _isObject,
    type
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import _windowResizeCallbacks from "../../core/utils/resize_callbacks";
import {
    extend
} from "../../core/utils/extend";
import {
    BaseThemeManager
} from "../core/base_theme_manager";
import DOMComponent from "../../core/dom_component";
import {
    changes,
    replaceInherit
} from "./helpers";
import {
    parseScalar as _parseScalar
} from "./utils";
import warnings from "./errors_warnings";
import {
    Renderer
} from "./renderers/renderer";
import {
    getWidth,
    getHeight
} from "../../core/utils/size";
import _Layout from "./layout";
import devices from "../../core/devices";
import eventsEngine from "../../events/core/events_engine";
import {
    when
} from "../../core/utils/deferred";
import {
    createEventTrigger,
    createResizeHandler,
    createIncidentOccurred
} from "./base_widget.utils";
var _floor = Math.floor;
var _log = warnings.log;
var OPTION_RTL_ENABLED = "rtlEnabled";
var SIZED_ELEMENT_CLASS = "dx-sized-element";
var _option = DOMComponent.prototype.option;

function getTrue() {
    return true
}

function getFalse() {
    return false
}

function areCanvasesDifferent(canvas1, canvas2) {
    return !(canvas1.width === canvas2.width && canvas1.height === canvas2.height && canvas1.left === canvas2.left && canvas1.top === canvas2.top && canvas1.right === canvas2.right && canvas1.bottom === canvas2.bottom)
}

function defaultOnIncidentOccurred(e) {
    if (!e.component._eventsStrategy.hasEvent("incidentOccurred")) {
        _log.apply(null, [e.target.id].concat(e.target.args || []))
    }
}

function pickPositiveValue(values) {
    return values.reduce((function(result, value) {
        return value > 0 && !result ? value : result
    }), 0)
}
var getEmptyComponent = function() {
    var emptyComponentConfig = {
        _initTemplates() {},
        ctor(element, options) {
            this.callBase(element, options);
            var sizedElement = domAdapter.createElement("div");
            var width = options && isNumeric(options.width) ? options.width + "px" : "100%";
            var height = options && isNumeric(options.height) ? options.height + "px" : this._getDefaultSize().height + "px";
            domAdapter.setStyle(sizedElement, "width", width);
            domAdapter.setStyle(sizedElement, "height", height);
            domAdapter.setClass(sizedElement, SIZED_ELEMENT_CLASS);
            domAdapter.insertElement(element, sizedElement)
        }
    };
    var EmptyComponent = DOMComponent.inherit(emptyComponentConfig);
    var originalInherit = EmptyComponent.inherit;
    EmptyComponent.inherit = function(config) {
        for (var field in config) {
            if (isFunction(config[field]) && "_" !== field.substr(0, 1) && "option" !== field || "_dispose" === field || "_optionChanged" === field) {
                config[field] = noop
            }
        }
        return originalInherit.call(this, config)
    };
    return EmptyComponent
};

function callForEach(functions) {
    functions.forEach(c => c())
}
var isServerSide = !hasWindow();

function sizeIsValid(value) {
    return isDefined(value) && value > 0
}
var baseWidget = isServerSide ? getEmptyComponent() : DOMComponent.inherit({
    _eventsMap: {
        onIncidentOccurred: {
            name: "incidentOccurred"
        },
        onDrawn: {
            name: "drawn"
        }
    },
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            onIncidentOccurred: defaultOnIncidentOccurred
        })
    },
    _useLinks: true,
    _init: function() {
        var that = this;
        that._$element.children("." + SIZED_ELEMENT_CLASS).remove();
        that.callBase.apply(that, arguments);
        that._changesLocker = 0;
        that._optionChangedLocker = 0;
        that._asyncFirstDrawing = true;
        that._changes = changes();
        that._suspendChanges();
        that._themeManager = that._createThemeManager();
        that._themeManager.setCallback((function() {
            that._requestChange(that._themeDependentChanges)
        }));
        that._renderElementAttributes();
        that._initRenderer();
        var linkTarget = that._useLinks && that._renderer.root;
        linkTarget && linkTarget.enableLinks().virtualLink("core").virtualLink("peripheral");
        that._renderVisibilityChange();
        that._attachVisibilityChangeHandlers();
        that._toggleParentsScrollSubscription(this._isVisible());
        that._initEventTrigger();
        that._incidentOccurred = createIncidentOccurred(that.NAME, that._eventTrigger);
        that._layout = new _Layout;
        linkTarget && linkTarget.linkAfter("core");
        that._initPlugins();
        that._initCore();
        linkTarget && linkTarget.linkAfter();
        that._change(that._initialChanges)
    },
    _createThemeManager() {
        return new BaseThemeManager(this._getThemeManagerOptions())
    },
    _getThemeManagerOptions() {
        return {
            themeSection: this._themeSection,
            fontFields: this._fontFields
        }
    },
    _initialChanges: ["LAYOUT", "RESIZE_HANDLER", "THEME", "DISABLED"],
    _initPlugins: function() {
        var that = this;
        each(that._plugins, (function(_, plugin) {
            plugin.init.call(that)
        }))
    },
    _disposePlugins: function() {
        var that = this;
        each(that._plugins.slice().reverse(), (function(_, plugin) {
            plugin.dispose.call(that)
        }))
    },
    _change: function(codes) {
        this._changes.add(codes)
    },
    _suspendChanges: function() {
        ++this._changesLocker
    },
    _resumeChanges: function() {
        if (0 === --this._changesLocker && this._changes.count() > 0 && !this._applyingChanges) {
            this._renderer.lock();
            this._applyingChanges = true;
            this._applyChanges();
            this._changes.reset();
            this._applyingChanges = false;
            this._changesApplied();
            this._renderer.unlock();
            if (this._optionsQueue) {
                this._applyQueuedOptions()
            }
            this.resolveItemsDeferred(this._legend ? [this._legend] : []);
            this._optionChangedLocker++;
            this._notify();
            this._optionChangedLocker--
        }
    },
    resolveItemsDeferred(items) {
        this._resolveDeferred(this._getTemplatesItems(items))
    },
    _collectTemplatesFromItems: items => items.reduce((prev, i) => ({
        items: prev.items.concat(i.getTemplatesDef()),
        groups: prev.groups.concat(i.getTemplatesGroups())
    }), {
        items: [],
        groups: []
    }),
    _getTemplatesItems(items) {
        var elements = this._collectTemplatesFromItems(items);
        var extraItems = this._getExtraTemplatesItems();
        return {
            items: extraItems.items.concat(elements.items),
            groups: extraItems.groups.concat(elements.groups),
            launchRequest: [extraItems.launchRequest],
            doneRequest: [extraItems.doneRequest]
        }
    },
    _getExtraTemplatesItems: () => ({
        items: [],
        groups: [],
        launchRequest: () => {},
        doneRequest: () => {}
    }),
    _resolveDeferred(_ref) {
        var {
            items: items,
            launchRequest: launchRequest,
            doneRequest: doneRequest,
            groups: groups
        } = _ref;
        var that = this;
        that._setGroupsVisibility(groups, "hidden");
        if (that._changesApplying) {
            that._changesApplying = false;
            callForEach(doneRequest);
            return
        }
        var syncRendering = true;
        when.apply(that, items).done(() => {
            if (syncRendering) {
                that._setGroupsVisibility(groups, "visible");
                return
            }
            callForEach(launchRequest);
            that._changesApplying = true;
            var changes = ["LAYOUT", "FULL_RENDER"];
            if (that._asyncFirstDrawing) {
                changes.push("FORCE_FIRST_DRAWING");
                that._asyncFirstDrawing = false
            } else {
                changes.push("FORCE_DRAWING")
            }
            that._requestChange(changes);
            that._setGroupsVisibility(groups, "visible")
        });
        syncRendering = false
    },
    _setGroupsVisibility(groups, visibility) {
        groups.forEach(g => g.attr({
            visibility: visibility
        }))
    },
    _applyQueuedOptions: function() {
        var queue = this._optionsQueue;
        this._optionsQueue = null;
        this.beginUpdate();
        each(queue, (function(_, action) {
            action()
        }));
        this.endUpdate()
    },
    _requestChange: function(codes) {
        this._suspendChanges();
        this._change(codes);
        this._resumeChanges()
    },
    _applyChanges: function() {
        var changes = this._changes;
        var order = this._totalChangesOrder;
        var i;
        var ii = order.length;
        for (i = 0; i < ii; ++i) {
            if (changes.has(order[i])) {
                this["_change_" + order[i]]()
            }
        }
    },
    _optionChangesOrder: ["EVENTS", "THEME", "RENDERER", "RESIZE_HANDLER"],
    _layoutChangesOrder: ["ELEMENT_ATTR", "CONTAINER_SIZE", "LAYOUT"],
    _customChangesOrder: ["DISABLED"],
    _change_EVENTS: function() {
        this._eventTrigger.applyChanges()
    },
    _change_THEME: function() {
        this._setThemeAndRtl()
    },
    _change_RENDERER: function() {
        this._setRendererOptions()
    },
    _change_RESIZE_HANDLER: function() {
        this._setupResizeHandler()
    },
    _change_ELEMENT_ATTR: function() {
        this._renderElementAttributes();
        this._change(["CONTAINER_SIZE"])
    },
    _change_CONTAINER_SIZE: function() {
        this._updateSize()
    },
    _change_LAYOUT: function() {
        this._setContentSize()
    },
    _change_DISABLED: function() {
        var renderer = this._renderer;
        var root = renderer.root;
        if (this.option("disabled")) {
            this._initDisabledState = root.attr("pointer-events");
            root.attr({
                "pointer-events": "none",
                filter: renderer.getGrayScaleFilter().id
            })
        } else if ("none" === root.attr("pointer-events")) {
            root.attr({
                "pointer-events": isDefined(this._initDisabledState) ? this._initDisabledState : null,
                filter: null
            })
        }
    },
    _themeDependentChanges: ["RENDERER"],
    _initRenderer: function() {
        this._canvas = this._calculateCanvas();
        this._renderer = new Renderer({
            cssClass: this._rootClassPrefix + " " + this._rootClass,
            pathModified: this.option("pathModified"),
            container: this._$element[0]
        });
        this._renderer.resize(this._canvas.width, this._canvas.height)
    },
    _disposeRenderer: function() {
        this._renderer.dispose()
    },
    _getAnimationOptions: noop,
    render: function() {
        this._requestChange(["CONTAINER_SIZE"]);
        var visible = this._isVisible();
        this._toggleParentsScrollSubscription(visible);
        !visible && this._stopCurrentHandling()
    },
    _toggleParentsScrollSubscription: function(subscribe) {
        var $parents = $(this._renderer.root.element).parents();
        if ("generic" === devices.real().platform) {
            $parents = $parents.add(getWindow())
        }
        this._proxiedTargetParentsScrollHandler = this._proxiedTargetParentsScrollHandler || function() {
            this._stopCurrentHandling()
        }.bind(this);
        eventsEngine.off($().add(this._$prevRootParents), "scroll.viz_widgets", this._proxiedTargetParentsScrollHandler);
        if (subscribe) {
            eventsEngine.on($parents, "scroll.viz_widgets", this._proxiedTargetParentsScrollHandler);
            this._$prevRootParents = $parents
        }
    },
    _stopCurrentHandling: noop,
    _dispose: function() {
        var that = this;
        that.callBase.apply(that, arguments);
        that._toggleParentsScrollSubscription(false);
        that._removeResizeHandler();
        that._layout.dispose();
        that._eventTrigger.dispose();
        that._disposeCore();
        that._disposePlugins();
        that._disposeRenderer();
        that._themeManager.dispose();
        that._themeManager = that._renderer = that._eventTrigger = null
    },
    _initEventTrigger: function() {
        var that = this;
        that._eventTrigger = createEventTrigger(that._eventsMap, (function(name) {
            return that._createActionByOption(name)
        }))
    },
    _calculateCanvas: function() {
        var that = this;
        var size = that.option("size") || {};
        var margin = that.option("margin") || {};
        var defaultCanvas = that._getDefaultSize() || {};
        var getSizeOfSide = (size, side, getter) => {
            if (sizeIsValid(size[side]) || !hasWindow()) {
                return 0
            }
            var elementSize = getter(that._$element);
            return elementSize <= 1 ? 0 : elementSize
        };
        var elementWidth = getSizeOfSide(size, "width", x => getWidth(x));
        var elementHeight = getSizeOfSide(size, "height", x => getHeight(x));
        var canvas = {
            width: size.width <= 0 ? 0 : _floor(pickPositiveValue([size.width, elementWidth, defaultCanvas.width])),
            height: size.height <= 0 ? 0 : _floor(pickPositiveValue([size.height, elementHeight, defaultCanvas.height])),
            left: pickPositiveValue([margin.left, defaultCanvas.left]),
            top: pickPositiveValue([margin.top, defaultCanvas.top]),
            right: pickPositiveValue([margin.right, defaultCanvas.right]),
            bottom: pickPositiveValue([margin.bottom, defaultCanvas.bottom])
        };
        if (canvas.width - canvas.left - canvas.right <= 0 || canvas.height - canvas.top - canvas.bottom <= 0) {
            canvas = {
                width: 0,
                height: 0
            }
        }
        return canvas
    },
    _updateSize: function() {
        var canvas = this._calculateCanvas();
        this._renderer.fixPlacement();
        if (areCanvasesDifferent(this._canvas, canvas) || this.__forceRender) {
            this._canvas = canvas;
            this._recreateSizeDependentObjects(true);
            this._renderer.resize(canvas.width, canvas.height);
            this._change(["LAYOUT"])
        }
    },
    _recreateSizeDependentObjects: noop,
    _getMinSize: function() {
        return [0, 0]
    },
    _getAlignmentRect: noop,
    _setContentSize: function() {
        var canvas = this._canvas;
        var layout = this._layout;
        var rect = canvas.width > 0 && canvas.height > 0 ? [canvas.left, canvas.top, canvas.width - canvas.right, canvas.height - canvas.bottom] : [0, 0, 0, 0];
        rect = layout.forward(rect, this._getMinSize());
        var nextRect = this._applySize(rect) || rect;
        layout.backward(nextRect, this._getAlignmentRect() || nextRect)
    },
    _getOption: function(name, isScalar) {
        var theme = this._themeManager.theme(name);
        var option = this.option(name);
        return isScalar ? void 0 !== option ? option : theme : extend(true, {}, theme, option)
    },
    _setupResizeHandler: function() {
        var that = this;
        var redrawOnResize = _parseScalar(this._getOption("redrawOnResize", true), true);
        if (that._resizeHandler) {
            that._removeResizeHandler()
        }
        that._resizeHandler = createResizeHandler((function() {
            if (redrawOnResize) {
                that._requestChange(["CONTAINER_SIZE"])
            } else {
                that._renderer.fixPlacement()
            }
        }));
        _windowResizeCallbacks.add(that._resizeHandler)
    },
    _removeResizeHandler: function() {
        if (this._resizeHandler) {
            _windowResizeCallbacks.remove(this._resizeHandler);
            this._resizeHandler.dispose();
            this._resizeHandler = null
        }
    },
    _onBeginUpdate: noop,
    beginUpdate: function() {
        var that = this;
        if (that._initialized && that._isUpdateAllowed()) {
            that._onBeginUpdate();
            that._suspendChanges()
        }
        that.callBase.apply(that, arguments);
        return that
    },
    endUpdate: function() {
        this.callBase();
        this._isUpdateAllowed() && this._resumeChanges();
        return this
    },
    option: function(name) {
        var that = this;
        if (that._initialized && that._applyingChanges && (arguments.length > 1 || _isObject(name))) {
            that._optionsQueue = that._optionsQueue || [];
            that._optionsQueue.push(that._getActionForUpdating(arguments))
        } else {
            return _option.apply(that, arguments)
        }
    },
    _getActionForUpdating: function(args) {
        var that = this;
        return function() {
            _option.apply(that, args)
        }
    },
    _clean: noop,
    _render: noop,
    _optionChanged: function(arg) {
        var that = this;
        if (that._optionChangedLocker) {
            return
        }
        var partialChanges = that.getPartialChangeOptionsName(arg);
        var changes = [];
        if (partialChanges.length > 0) {
            partialChanges.forEach(pc => changes.push(that._partialOptionChangesMap[pc]))
        } else {
            changes.push(that._optionChangesMap[arg.name])
        }
        changes = changes.filter(c => !!c);
        if (that._eventTrigger.change(arg.name)) {
            that._change(["EVENTS"])
        } else if (changes.length > 0) {
            that._change(changes)
        } else {
            that.callBase.apply(that, arguments)
        }
    },
    _notify: noop,
    _changesApplied: noop,
    _optionChangesMap: {
        size: "CONTAINER_SIZE",
        margin: "CONTAINER_SIZE",
        redrawOnResize: "RESIZE_HANDLER",
        theme: "THEME",
        rtlEnabled: "THEME",
        encodeHtml: "THEME",
        elementAttr: "ELEMENT_ATTR",
        disabled: "DISABLED"
    },
    _partialOptionChangesMap: {},
    _partialOptionChangesPath: {},
    getPartialChangeOptionsName: function(changedOption) {
        var that = this;
        var fullName = changedOption.fullName;
        var sections = fullName.split(/[.]/);
        var name = changedOption.name;
        var value = changedOption.value;
        var options = this._partialOptionChangesPath[name];
        var partialChangeOptionsName = [];
        if (options) {
            if (true === options) {
                partialChangeOptionsName.push(name)
            } else {
                options.forEach(op => {
                    fullName.indexOf(op) >= 0 && partialChangeOptionsName.push(op)
                });
                if (1 === sections.length) {
                    if ("object" === type(value)) {
                        that._addOptionsNameForPartialUpdate(value, options, partialChangeOptionsName)
                    } else if ("array" === type(value)) {
                        if (value.length > 0 && value.every(item => that._checkOptionsForPartialUpdate(item, options))) {
                            value.forEach(item => that._addOptionsNameForPartialUpdate(item, options, partialChangeOptionsName))
                        }
                    }
                }
            }
        }
        return partialChangeOptionsName.filter((value, index, self) => self.indexOf(value) === index)
    },
    _checkOptionsForPartialUpdate: function(optionObject, options) {
        return !Object.keys(optionObject).some(key => -1 === options.indexOf(key))
    },
    _addOptionsNameForPartialUpdate: function(optionObject, options, partialChangeOptionsName) {
        var optionKeys = Object.keys(optionObject);
        if (this._checkOptionsForPartialUpdate(optionObject, options)) {
            optionKeys.forEach(key => options.indexOf(key) > -1 && partialChangeOptionsName.push(key))
        }
    },
    _visibilityChanged: function() {
        this.render()
    },
    _setThemeAndRtl: function() {
        this._themeManager.setTheme(this.option("theme"), this.option(OPTION_RTL_ENABLED))
    },
    _getRendererOptions: function() {
        return {
            rtl: this.option(OPTION_RTL_ENABLED),
            encodeHtml: this.option("encodeHtml"),
            animation: this._getAnimationOptions()
        }
    },
    _setRendererOptions: function() {
        this._renderer.setOptions(this._getRendererOptions())
    },
    svg: function() {
        return this._renderer.svg()
    },
    getSize: function() {
        var canvas = this._canvas || {};
        return {
            width: canvas.width,
            height: canvas.height
        }
    },
    isReady: getFalse,
    _dataIsReady: getTrue,
    _resetIsReady: function() {
        this.isReady = getFalse
    },
    _drawn: function() {
        var that = this;
        that.isReady = getFalse;
        if (that._dataIsReady()) {
            that._renderer.onEndAnimation((function() {
                that.isReady = getTrue
            }))
        }
        that._eventTrigger("drawn", {})
    }
});
export default baseWidget;
replaceInherit(baseWidget);
