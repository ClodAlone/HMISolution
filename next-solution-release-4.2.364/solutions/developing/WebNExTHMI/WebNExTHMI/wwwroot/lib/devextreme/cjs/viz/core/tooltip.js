/**
 * DevExtreme (cjs/viz/core/tooltip.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.plugin = exports.Tooltip = void 0;
var _size = require("../../core/utils/size");
var _dom_adapter = _interopRequireDefault(require("../../core/dom_adapter"));
var _window = require("../../core/utils/window");
var _dom = require("../../core/utils/dom");
var _inflector = require("../../core/utils/inflector");
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _renderer2 = require("./renderers/renderer");
var _type = require("../../core/utils/type");
var _extend = require("../../core/utils/extend");
var _utils = require("./utils");
var _format_helper = _interopRequireDefault(require("../../format_helper"));
var _plaque = require("./plaque");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var format = _format_helper.default.format;
var mathCeil = Math.ceil;
var mathMax = Math.max;
var mathMin = Math.min;
var window = (0, _window.getWindow)();
var DEFAULT_HTML_GROUP_WIDTH = 3e3;

function hideElement($element) {
    $element.css({
        left: "-9999px"
    }).detach()
}

function getSpecialFormatOptions(options, specialFormat) {
    var result = options;
    switch (specialFormat) {
        case "argument":
            result = {
                format: options.argumentFormat
            };
            break;
        case "percent":
            result = {
                format: {
                    type: "percent",
                    precision: options.format && options.format.percentPrecision
                }
            }
    }
    return result
}

function createTextHtml() {
    return (0, _renderer.default)("<div>").css({
        position: "relative",
        display: "inline-block",
        padding: 0,
        margin: 0,
        border: "0px solid transparent"
    })
}

function removeElements(elements) {
    elements.forEach((function(el) {
        return el.remove()
    }))
}
var Tooltip = function(params) {
    this._eventTrigger = params.eventTrigger;
    this._widgetRoot = params.widgetRoot;
    this._widget = params.widget;
    this._textHtmlContainers = [];
    this._wrapper = (0, _renderer.default)("<div>").css({
        position: "absolute",
        overflow: "hidden",
        pointerEvents: "none"
    }).addClass(params.cssClass);
    var renderer = this._renderer = new _renderer2.Renderer({
        pathModified: params.pathModified,
        container: this._wrapper[0]
    });
    var root = renderer.root;
    root.attr({
        "pointer-events": "none"
    });
    this._text = renderer.text(void 0, 0, 0);
    this._textGroupHtml = (0, _renderer.default)("<div>").css({
        position: "absolute",
        padding: 0,
        margin: 0,
        border: "0px solid transparent"
    }).appendTo(this._wrapper);
    this._textHtml = createTextHtml().appendTo(this._textGroupHtml)
};
exports.Tooltip = Tooltip;
Tooltip.prototype = {
    constructor: Tooltip,
    dispose: function() {
        this._wrapper.remove();
        this._renderer.dispose();
        this._options = this._widgetRoot = null
    },
    _getContainer: function() {
        var options = this._options;
        var container = (0, _renderer.default)(this._widgetRoot).closest(options.container);
        if (0 === container.length) {
            container = (0, _renderer.default)(options.container)
        }
        return (container.length ? container : (0, _renderer.default)("body")).get(0)
    },
    setTemplate: function(contentTemplate) {
        this._template = contentTemplate ? this._widget._getTemplate(contentTemplate) : null
    },
    setOptions: function(options) {
        var _this = this;
        options = options || {};
        var that = this;
        that._options = options;
        that._textFontStyles = (0, _utils.patchFontOptions)(options.font);
        that._textFontStyles.color = that._textFontStyles.fill;
        that._wrapper.css({
            zIndex: options.zIndex
        });
        that._customizeTooltip = options.customizeTooltip;
        var textGroupHtml = that._textGroupHtml;
        if (this.plaque) {
            this.plaque.clear()
        }
        this.setTemplate(options.contentTemplate);
        var pointerEvents = options.interactive ? "auto" : "none";
        if (options.interactive) {
            this._renderer.root.css({
                "-ms-user-select": "auto",
                "-moz-user-select": "auto",
                "-webkit-user-select": "auto"
            })
        }
        this.plaque = new _plaque.Plaque({
            opacity: that._options.opacity,
            color: that._options.color,
            border: that._options.border,
            paddingLeftRight: that._options.paddingLeftRight,
            paddingTopBottom: that._options.paddingTopBottom,
            arrowLength: that._options.arrowLength,
            arrowWidth: 20,
            shadow: that._options.shadow,
            cornerRadius: that._options.cornerRadius
        }, that, that._renderer.root, (function(_ref) {
            var group = _ref.group,
                onRender = _ref.onRender,
                eventData = _ref.eventData,
                isMoving = _ref.isMoving,
                _ref$templateCallback = _ref.templateCallback,
                templateCallback = void 0 === _ref$templateCallback ? function() {} : _ref$templateCallback;
            var state = that._state;
            if (!isMoving) {
                var template = that._template;
                var useTemplate = template && !state.formatObject.skipTemplate;
                if (state.html || useTemplate) {
                    textGroupHtml.css({
                        color: state.textColor,
                        width: DEFAULT_HTML_GROUP_WIDTH,
                        pointerEvents: pointerEvents
                    });
                    if (useTemplate) {
                        var htmlContainers = that._textHtmlContainers;
                        var containerToTemplateRender = createTextHtml();
                        htmlContainers.push(containerToTemplateRender);
                        template.render({
                            model: state.formatObject,
                            container: containerToTemplateRender,
                            onRendered: function() {
                                removeElements(htmlContainers.splice(0, htmlContainers.length - 1));
                                var containerWithContent = htmlContainers[0];
                                (0, _dom.replaceWith)(that._textHtml, containerWithContent);
                                that._textHtml = containerWithContent;
                                state.html = containerWithContent.html();
                                if (0 === (0, _size.getWidth)(containerWithContent) && 0 === (0, _size.getHeight)(containerWithContent)) {
                                    _this.plaque.clear();
                                    templateCallback(false);
                                    return
                                }
                                onRender();
                                that._riseEvents(eventData);
                                that._moveWrapper();
                                that.plaque.customizeCloud({
                                    fill: state.color,
                                    stroke: state.borderColor,
                                    "pointer-events": pointerEvents
                                });
                                templateCallback(true);
                                that._textHtmlContainers = []
                            }
                        });
                        return
                    } else {
                        that._text.attr({
                            text: ""
                        });
                        that._textHtml.html(state.html)
                    }
                } else {
                    that._text.css({
                        fill: state.textColor
                    }).attr({
                        text: state.text,
                        class: options.cssClass,
                        "pointer-events": pointerEvents
                    }).append(group.attr({
                        align: options.textAlignment
                    }))
                }
                that._riseEvents(eventData);
                that.plaque.customizeCloud({
                    fill: state.color,
                    stroke: state.borderColor,
                    "pointer-events": pointerEvents
                })
            }
            onRender();
            that._moveWrapper();
            return true
        }), true, (function(tooltip, g) {
            var state = tooltip._state;
            if (state.html) {
                var bBox;
                var getComputedStyle = window.getComputedStyle;
                if (getComputedStyle) {
                    bBox = getComputedStyle(that._textHtml.get(0));
                    bBox = {
                        x: 0,
                        y: 0,
                        width: mathCeil(parseFloat(bBox.width)),
                        height: mathCeil(parseFloat(bBox.height))
                    }
                } else {
                    bBox = that._textHtml.get(0).getBoundingClientRect();
                    bBox = {
                        x: 0,
                        y: 0,
                        width: mathCeil(bBox.width ? bBox.width : bBox.right - bBox.left),
                        height: mathCeil(bBox.height ? bBox.height : bBox.bottom - bBox.top)
                    }
                }
                return bBox
            }
            return g.getBBox()
        }), (function(tooltip, g, x, y) {
            var state = tooltip._state;
            if (state.html) {
                that._textGroupHtml.css({
                    left: x,
                    top: y
                })
            } else {
                g.move(x, y)
            }
        }));
        return that
    },
    _riseEvents: function(eventData) {
        this._eventData && this._eventTrigger("tooltipHidden", this._eventData);
        this._eventData = eventData;
        this._eventTrigger("tooltipShown", this._eventData)
    },
    setRendererOptions: function(options) {
        this._renderer.setOptions(options);
        this._textGroupHtml.css({
            direction: options.rtl ? "rtl" : "ltr"
        });
        return this
    },
    update: function(options) {
        this.setOptions(options);
        hideElement(this._wrapper);
        var normalizedCSS = {};
        for (var name in this._textFontStyles) {
            normalizedCSS[(0, _inflector.camelize)(name)] = this._textFontStyles[name]
        }
        this._textGroupHtml.css(normalizedCSS);
        this._text.css(this._textFontStyles);
        this._eventData = null;
        return this
    },
    _prepare: function(formatObject, state) {
        var customizeTooltip = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : this._customizeTooltip;
        var options = this._options;
        var customize = {};
        if ((0, _type.isFunction)(customizeTooltip)) {
            customize = customizeTooltip.call(formatObject, formatObject);
            customize = (0, _type.isPlainObject)(customize) ? customize : {};
            if ("text" in customize) {
                state.text = (0, _type.isDefined)(customize.text) ? String(customize.text) : ""
            }
            if ("html" in customize) {
                state.html = (0, _type.isDefined)(customize.html) ? String(customize.html) : ""
            }
        }
        if (!("text" in state) && !("html" in state)) {
            state.text = formatObject.valueText || formatObject.description || ""
        }
        state.color = customize.color || options.color;
        state.borderColor = customize.borderColor || (options.border || {}).color;
        state.textColor = customize.fontColor || (this._textFontStyles || {}).color;
        return !!state.text || !!state.html || !!this._template
    },
    show: function(formatObject, params, eventData, customizeTooltip, templateCallback) {
        if (this._options.forceEvents) {
            eventData.x = params.x;
            eventData.y = params.y - params.offset;
            this._riseEvents(eventData);
            return true
        }
        var state = {
            formatObject: formatObject,
            eventData: eventData,
            templateCallback: templateCallback
        };
        if (!this._prepare(formatObject, state, customizeTooltip)) {
            return false
        }
        this._state = state;
        this._wrapper.appendTo(this._getContainer());
        this._clear();
        var parameters = (0, _extend.extend)({}, this._options, {
            canvas: this._getCanvas()
        }, state, {
            x: params.x,
            y: params.y,
            offset: params.offset
        });
        return this.plaque.clear().draw(parameters)
    },
    isCursorOnTooltip: function(x, y) {
        if (this._options.interactive) {
            var box = this.plaque.getBBox();
            return x > box.x && x < box.x + box.width && y > box.y && y < box.y + box.height
        }
        return false
    },
    hide: function(isPointerOut) {
        hideElement(this._wrapper);
        if (this._eventData) {
            this._eventTrigger("tooltipHidden", this._options.forceEvents ? (0, _extend.extend)({
                isPointerOut: isPointerOut
            }, this._eventData) : this._eventData);
            this._clear();
            this._eventData = null
        }
    },
    _clear: function() {
        this._textHtml.empty()
    },
    move: function(x, y, offset) {
        this.plaque.draw({
            x: x,
            y: y,
            offset: offset,
            canvas: this._getCanvas(),
            isMoving: true
        })
    },
    _moveWrapper: function() {
        var plaqueBBox = this.plaque.getBBox();
        this._renderer.resize(plaqueBBox.width, plaqueBBox.height);
        var offset = this._wrapper.css({
            left: 0,
            top: 0
        }).offset();
        var left = plaqueBBox.x;
        var top = plaqueBBox.y;
        this._wrapper.css({
            left: left - offset.left,
            top: top - offset.top
        });
        this.plaque.moveRoot(-left, -top);
        if (this._state.html) {
            this._textHtml.css({
                left: -left,
                top: -top
            });
            this._textGroupHtml.css({
                width: mathCeil((0, _size.getWidth)(this._textHtml))
            })
        }
    },
    formatValue: function(value, _specialFormat) {
        var options = _specialFormat ? getSpecialFormatOptions(this._options, _specialFormat) : this._options;
        return format(value, options.format)
    },
    getOptions: function() {
        return this._options
    },
    getLocation: function() {
        return (0, _utils.normalizeEnum)(this._options.location)
    },
    isEnabled: function() {
        return !!this._options.enabled || !!this._options.forceEvents
    },
    isShared: function() {
        return !!this._options.shared
    },
    _getCanvas: function() {
        var container = this._getContainer();
        var containerBox = container.getBoundingClientRect();
        var html = _dom_adapter.default.getDocumentElement();
        var document = _dom_adapter.default.getDocument();
        var left = window.pageXOffset || html.scrollLeft || 0;
        var top = window.pageYOffset || html.scrollTop || 0;
        var box = {
            left: left,
            top: top,
            width: mathMax(html.clientWidth, document.body.clientWidth) + left,
            height: mathMax(document.body.scrollHeight, html.scrollHeight, document.body.offsetHeight, html.offsetHeight, document.body.clientHeight, html.clientHeight),
            right: 0,
            bottom: 0
        };
        if (container !== _dom_adapter.default.getBody()) {
            left = mathMax(box.left, box.left + containerBox.left);
            top = mathMax(box.top, box.top + containerBox.top);
            box.width = mathMin(containerBox.width, box.width) + left + box.left;
            box.height = mathMin(containerBox.height, box.height) + top + box.top;
            box.left = left;
            box.top = top
        }
        return box
    }
};
var plugin = {
    name: "tooltip",
    init: function() {
        this._initTooltip()
    },
    dispose: function() {
        this._disposeTooltip()
    },
    members: {
        _initTooltip: function() {
            this._tooltip = new Tooltip({
                cssClass: this._rootClassPrefix + "-tooltip",
                eventTrigger: this._eventTrigger,
                pathModified: this.option("pathModified"),
                widgetRoot: this.element(),
                widget: this
            })
        },
        _disposeTooltip: function() {
            this._tooltip.dispose();
            this._tooltip = null
        },
        _setTooltipRendererOptions: function() {
            this._tooltip.setRendererOptions(this._getRendererOptions())
        },
        _setTooltipOptions: function() {
            this._tooltip.update(this._getOption("tooltip"))
        }
    },
    extenders: {
        _stopCurrentHandling: function() {
            this._tooltip && this._tooltip.hide()
        }
    },
    customize: function(constructor) {
        var proto = constructor.prototype;
        proto._eventsMap.onTooltipShown = {
            name: "tooltipShown"
        };
        proto._eventsMap.onTooltipHidden = {
            name: "tooltipHidden"
        };
        constructor.addChange({
            code: "TOOLTIP_RENDERER",
            handler: function() {
                this._setTooltipRendererOptions()
            },
            isThemeDependent: true,
            isOptionChange: true
        });
        constructor.addChange({
            code: "TOOLTIP",
            handler: function() {
                this._setTooltipOptions()
            },
            isThemeDependent: true,
            isOptionChange: true,
            option: "tooltip"
        })
    },
    fontFields: ["tooltip.font"]
};
exports.plugin = plugin;
