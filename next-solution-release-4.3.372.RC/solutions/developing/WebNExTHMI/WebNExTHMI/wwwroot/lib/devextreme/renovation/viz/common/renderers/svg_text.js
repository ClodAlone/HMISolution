/**
 * DevExtreme (renovation/viz/common/renderers/svg_text.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.viewFunction = exports.TextSvgElementProps = exports.TextSvgElement = void 0;
var _inferno = require("inferno");
var _inferno2 = require("@devextreme/runtime/inferno");
var _base_graphics_props = _interopRequireDefault(require("./base_graphics_props"));
var _utils = require("./utils");
var _type = require("../../../../core/utils/type");
var _config_context = require("../../../common/config_context");
var _excluded = ["align", "className", "dashStyle", "encodeHtml", "fill", "opacity", "rotate", "rotateX", "rotateY", "scaleX", "scaleY", "sharp", "sharpDirection", "stroke", "strokeOpacity", "strokeWidth", "styles", "text", "textsAlignment", "translateX", "translateY", "x", "y"];

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _objectWithoutProperties(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = _objectWithoutPropertiesLoose(source, excluded);
    var key, i;
    if (Object.getOwnPropertySymbols) {
        var sourceSymbolKeys = Object.getOwnPropertySymbols(source);
        for (i = 0; i < sourceSymbolKeys.length; i++) {
            key = sourceSymbolKeys[i];
            if (excluded.indexOf(key) >= 0) {
                continue
            }
            if (!Object.prototype.propertyIsEnumerable.call(source, key)) {
                continue
            }
            target[key] = source[key]
        }
    }
    return target
}

function _objectWithoutPropertiesLoose(source, excluded) {
    if (null == source) {
        return {}
    }
    var target = {};
    var sourceKeys = Object.keys(source);
    var key, i;
    for (i = 0; i < sourceKeys.length; i++) {
        key = sourceKeys[i];
        if (excluded.indexOf(key) >= 0) {
            continue
        }
        target[key] = source[key]
    }
    return target
}

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}

function _assertThisInitialized(self) {
    if (void 0 === self) {
        throw new ReferenceError("this hasn't been initialised - super() hasn't been called")
    }
    return self
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}
var KEY_STROKE = "stroke";
var viewFunction = function(_ref) {
    var computedProps = _ref.computedProps,
        isStroked = _ref.isStroked,
        styles = _ref.styles,
        textAnchor = _ref.textAnchor,
        textItems = _ref.textItems,
        textRef = _ref.textRef;
    var texts = null !== textItems && void 0 !== textItems ? textItems : [];
    var fill = computedProps.fill,
        opacity = computedProps.opacity,
        stroke = computedProps.stroke,
        strokeOpacity = computedProps.strokeOpacity,
        strokeWidth = computedProps.strokeWidth,
        text = computedProps.text,
        x = computedProps.x,
        y = computedProps.y;
    return (0, _inferno.normalizeProps)((0, _inferno.createVNode)(32, "text", null, [texts.length ? isStroked && texts.map((function(_ref2, index) {
        var className = _ref2.className,
            style = _ref2.style,
            value = _ref2.value;
        return (0, _inferno.createVNode)(32, "tspan", className, value, 0, {
            style: (0, _inferno2.normalizeStyles)(style)
        }, index)
    })) : null, texts.length ? texts.map((function(_ref3, index) {
        var className = _ref3.className,
            style = _ref3.style,
            value = _ref3.value;
        return (0, _inferno.createVNode)(32, "tspan", className, value, 0, {
            style: (0, _inferno2.normalizeStyles)(style)
        }, index)
    })) : null, !texts.length && text], 0, _extends({
        x: x,
        y: y,
        style: (0, _inferno2.normalizeStyles)(styles),
        "text-anchor": textAnchor,
        fill: fill,
        stroke: stroke,
        "stroke-width": strokeWidth,
        "stroke-opacity": strokeOpacity,
        opacity: opacity
    }, (0, _utils.getGraphicExtraProps)(computedProps, x, y)), null, textRef))
};
exports.viewFunction = viewFunction;
var TextSvgElementProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_base_graphics_props.default), Object.getOwnPropertyDescriptors({
    text: "",
    x: 0,
    y: 0,
    align: "center",
    encodeHtml: true
})));
exports.TextSvgElementProps = TextSvgElementProps;
var TextSvgElement = function(_InfernoComponent) {
    _inheritsLoose(TextSvgElement, _InfernoComponent);

    function TextSvgElement(props) {
        var _this;
        _this = _InfernoComponent.call(this, props) || this;
        _this.state = {};
        _this.textRef = (0, _inferno.createRef)();
        _this.__getterCache = {};
        _this.effectUpdateText = _this.effectUpdateText.bind(_assertThisInitialized(_this));
        _this.parseTspanElements = _this.parseTspanElements.bind(_assertThisInitialized(_this));
        _this.alignTextNodes = _this.alignTextNodes.bind(_assertThisInitialized(_this));
        _this.locateTextNodes = _this.locateTextNodes.bind(_assertThisInitialized(_this));
        _this.strokeTextNodes = _this.strokeTextNodes.bind(_assertThisInitialized(_this));
        return _this
    }
    var _proto = TextSvgElement.prototype;
    _proto.createEffects = function() {
        return [new _inferno2.InfernoEffect(this.effectUpdateText, [this.props.text, this.props.encodeHtml, this.props.stroke, this.props.strokeWidth, this.props.textsAlignment, this.props.x, this.props.y, this.props.styles, this.props.strokeOpacity])]
    };
    _proto.updateEffects = function() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.text, this.props.encodeHtml, this.props.stroke, this.props.strokeWidth, this.props.textsAlignment, this.props.x, this.props.y, this.props.styles, this.props.strokeOpacity])
    };
    _proto.effectUpdateText = function() {
        var texts = this.textItems;
        if (texts) {
            var items = this.parseTspanElements(texts);
            this.alignTextNodes(items);
            if (void 0 !== this.props.x || void 0 !== this.props.y) {
                this.locateTextNodes(items)
            }
            this.strokeTextNodes(items)
        }
    };
    _proto.parseTspanElements = function(texts) {
        var items = _toConsumableArray(texts);
        var textElements = this.textRef.current.children;
        var strokeLength = !this.isStroked ? 0 : items.length;
        for (var i = 0; i < textElements.length; i++) {
            if (i < strokeLength) {
                items[i].stroke = textElements[i]
            } else {
                items[i % items.length].tspan = textElements[i]
            }
        }
        return items
    };
    _proto.alignTextNodes = function(items) {
        var alignment = this.props.textsAlignment;
        if (!items || !alignment || "center" === alignment) {
            return
        }
        var direction = "left" === alignment ? -1 : 1;
        var maxTextWidth = Math.max.apply(Math, _toConsumableArray(items.map((function(t) {
            return (0, _utils.getTextWidth)(t)
        }))));
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var textWidth = (0, _utils.getTextWidth)(item);
            if (0 !== maxTextWidth && maxTextWidth !== textWidth) {
                var value = direction * (Math.round((maxTextWidth - textWidth) / 2 * 10) / 10);
                (0, _utils.setTextNodeAttribute)(item, "dx", String(value))
            }
        }
    };
    _proto.locateTextNodes = function(items) {
        var _this$props = this.props,
            styles = _this$props.styles,
            x = _this$props.x,
            y = _this$props.y;
        var lineHeight = (0, _utils.getLineHeight)(null !== styles && void 0 !== styles ? styles : {});
        var item = items[0];
        (0, _utils.setTextNodeAttribute)(item, "x", String(x));
        (0, _utils.setTextNodeAttribute)(item, "y", String(y));
        for (var i = 1, ii = items.length; i < ii; ++i) {
            item = items[i];
            if ((0, _type.isDefined)(item.height) && item.height >= 0) {
                (0, _utils.setTextNodeAttribute)(item, "x", String(x));
                var height = (0, _utils.getItemLineHeight)(item, lineHeight);
                (0, _utils.setTextNodeAttribute)(item, "dy", String(height))
            }
        }
    };
    _proto.strokeTextNodes = function(items) {
        if (!this.isStroked) {
            return
        }
        var _this$props2 = this.props,
            stroke = _this$props2.stroke,
            strokeWidth = _this$props2.strokeWidth;
        var strokeOpacity = Number(this.props.strokeOpacity) || 1;
        for (var i = 0, ii = items.length; i < ii; ++i) {
            var tspan = items[i].stroke;
            if (tspan) {
                tspan.setAttribute(KEY_STROKE, String(stroke));
                tspan.setAttribute("stroke-width", String(strokeWidth));
                tspan.setAttribute("stroke-opacity", String(strokeOpacity));
                tspan.setAttribute("stroke-linejoin", "round")
            }
        }
    };
    _proto.componentWillUpdate = function(nextProps, nextState, context) {
        _InfernoComponent.prototype.componentWillUpdate.call(this);
        if (this.props.text !== nextProps.text || this.props.encodeHtml !== nextProps.encodeHtml || this.props.stroke !== nextProps.stroke || this.props.strokeWidth !== nextProps.strokeWidth) {
            this.__getterCache.textItems = void 0
        }
    };
    _proto.render = function() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            textRef: this.textRef,
            config: this.config,
            styles: this.styles,
            textItems: this.textItems,
            isStroked: this.isStroked,
            textAnchor: this.textAnchor,
            computedProps: this.computedProps,
            parseTspanElements: this.parseTspanElements,
            alignTextNodes: this.alignTextNodes,
            locateTextNodes: this.locateTextNodes,
            strokeTextNodes: this.strokeTextNodes,
            restAttributes: this.restAttributes
        })
    };
    _createClass(TextSvgElement, [{
        key: "config",
        get: function() {
            if ("ConfigContext" in this.context) {
                return this.context.ConfigContext
            }
            return _config_context.ConfigContext
        }
    }, {
        key: "styles",
        get: function() {
            var _this$props$styles;
            var style = null !== (_this$props$styles = this.props.styles) && void 0 !== _this$props$styles ? _this$props$styles : {};
            return _extends({
                whiteSpace: "pre"
            }, style)
        }
    }, {
        key: "textItems",
        get: function() {
            var _this2 = this;
            if (void 0 !== this.__getterCache.textItems) {
                return this.__getterCache.textItems
            }
            return this.__getterCache.textItems = function() {
                var items = void 0;
                var parsedHtml = "";
                var text = _this2.props.text;
                if (!text) {
                    return
                }
                if (!_this2.props.encodeHtml && (/<[a-z][\s\S]*>/i.test(text) || text.includes("&"))) {
                    parsedHtml = (0, _utils.removeExtraAttrs)(text);
                    items = (0, _utils.parseHTML)(parsedHtml)
                } else if (/\n/g.test(text)) {
                    items = (0, _utils.parseMultiline)(text)
                } else if (_this2.isStroked) {
                    items = [{
                        value: text.trim(),
                        height: 0
                    }]
                }
                return items
            }()
        }
    }, {
        key: "isStroked",
        get: function() {
            return (0, _type.isDefined)(this.props.stroke) && (0, _type.isDefined)(this.props.strokeWidth)
        }
    }, {
        key: "textAnchor",
        get: function() {
            var _this$config;
            return (0, _utils.convertAlignmentToAnchor)(this.props.align, null === (_this$config = this.config) || void 0 === _this$config ? void 0 : _this$config.rtlEnabled)
        }
    }, {
        key: "computedProps",
        get: function() {
            return this.props
        }
    }, {
        key: "restAttributes",
        get: function() {
            var _this$props3 = this.props,
                restProps = (_this$props3.align, _this$props3.className, _this$props3.dashStyle, _this$props3.encodeHtml, _this$props3.fill, _this$props3.opacity, _this$props3.rotate, _this$props3.rotateX, _this$props3.rotateY, _this$props3.scaleX, _this$props3.scaleY, _this$props3.sharp, _this$props3.sharpDirection, _this$props3.stroke, _this$props3.strokeOpacity, _this$props3.strokeWidth, _this$props3.styles, _this$props3.text, _this$props3.textsAlignment, _this$props3.translateX, _this$props3.translateY, _this$props3.x, _this$props3.y, _objectWithoutProperties(_this$props3, _excluded));
            return restProps
        }
    }]);
    return TextSvgElement
}(_inferno2.InfernoComponent);
exports.TextSvgElement = TextSvgElement;
TextSvgElement.defaultProps = TextSvgElementProps;
