/**
 * DevExtreme (cjs/renovation/ui/box/_toDelete/box.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../core/renderer"));
var _component_registrator = _interopRequireDefault(require("../core/component_registrator"));
var _extend = require("../core/utils/extend");
var _common = require("../core/utils/common");
var _window = require("../core/utils/window");
var _inflector = require("../core/utils/inflector");
var _type = require("../core/utils/type");
var _style = require("../core/utils/style");
var _iterator = require("../core/utils/iterator");
var _item = _interopRequireDefault(require("./collection/item"));
var _uiCollection_widget = _interopRequireDefault(require("./collection/ui.collection_widget.edit"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
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
var BOX_ITEM_CLASS = "dx-box-item";
var BOX_ITEM_DATA_KEY = "dxBoxItemData";
var MINSIZE_MAP = {
    row: "minWidth",
    col: "minHeight"
};
var MAXSIZE_MAP = {
    row: "maxWidth",
    col: "maxHeight"
};
var SHRINK = 1;
var setFlexProp = function(element, prop, value) {
    value = (0, _style.normalizeStyleProp)(prop, value);
    element.style[(0, _style.styleProp)(prop)] = value;
    if (!(0, _window.hasWindow)()) {
        if ("" === value || !(0, _type.isDefined)(value)) {
            return
        }
        var cssName = (0, _inflector.dasherize)(prop);
        var styleExpr = cssName + ": " + value + ";";
        if (!element.attributes.style) {
            element.setAttribute("style", styleExpr)
        } else if (element.attributes.style.value.indexOf(styleExpr) < 0) {
            element.attributes.style.value += " " + styleExpr
        }
    }
};
var BoxItem = function(_CollectionWidgetItem) {
    _inheritsLoose(BoxItem, _CollectionWidgetItem);

    function BoxItem() {
        return _CollectionWidgetItem.apply(this, arguments) || this
    }
    var _proto = BoxItem.prototype;
    _proto._renderVisible = function(value, oldValue) {
        _CollectionWidgetItem.prototype._renderVisible.call(this, value);
        if ((0, _type.isDefined)(oldValue)) {
            this._options.fireItemStateChangedAction({
                name: "visible",
                state: value,
                oldState: oldValue
            })
        }
    };
    return BoxItem
}(_item.default);
var FlexLayoutStrategy = function() {
    function FlexLayoutStrategy($element, option) {
        this._$element = $element;
        this._option = option;
        this.initSize = _common.noop;
        this.update = _common.noop
    }
    var _proto2 = FlexLayoutStrategy.prototype;
    _proto2.renderBox = function() {};
    _proto2.renderItems = function($items) {
        var flexPropPrefix = (0, _style.stylePropPrefix)("flexDirection");
        var direction = this._option("direction");
        (0, _iterator.each)($items, (function() {
            var $item = (0, _renderer.default)(this);
            var item = $item.data(BOX_ITEM_DATA_KEY);
            $item.css({
                display: flexPropPrefix + "flex"
            }).css(MAXSIZE_MAP[direction], item.maxSize || "none").css(MINSIZE_MAP[direction], item.minSize || "0");
            setFlexProp($item.get(0), "flexBasis", item.baseSize || 0);
            setFlexProp($item.get(0), "flexGrow", item.ratio);
            setFlexProp($item.get(0), "flexShrink", (0, _type.isDefined)(item.shrink) ? item.shrink : SHRINK);
            $item.children().each((function(_, itemContent) {
                (0, _renderer.default)(itemContent).css({
                    width: "auto",
                    height: "auto",
                    display: (0, _style.stylePropPrefix)("flexDirection") + "flex",
                    flexBasis: 0
                });
                setFlexProp(itemContent, "flexGrow", 1);
                setFlexProp(itemContent, "flexDirection", (0, _renderer.default)(itemContent)[0].style.flexDirection || "column")
            }))
        }))
    };
    return FlexLayoutStrategy
}();
var Box = function(_CollectionWidget) {
    _inheritsLoose(Box, _CollectionWidget);

    function Box() {
        return _CollectionWidget.apply(this, arguments) || this
    }
    var _proto3 = Box.prototype;
    _proto3._getDefaultOptions = function() {
        return (0, _extend.extend)(_CollectionWidget.prototype._getDefaultOptions.call(this), {
            activeStateEnabled: false,
            focusStateEnabled: false,
            onItemStateChanged: void 0,
            _layoutStrategy: "flex",
            _queue: void 0
        })
    };
    _proto3._itemClass = function() {
        return BOX_ITEM_CLASS
    };
    _proto3._itemDataKey = function() {
        return BOX_ITEM_DATA_KEY
    };
    _proto3._itemElements = function() {
        return this._itemContainer().children(this._itemSelector())
    };
    _proto3._init = function() {
        _CollectionWidget.prototype._init.call(this);
        this._initLayout();
        this._initBoxQueue()
    };
    _proto3._initLayout = function() {
        this._layout = new FlexLayoutStrategy(this.$element(), this.option.bind(this))
    };
    _proto3._initBoxQueue = function() {
        this._queue = this.option("_queue") || []
    };
    _proto3._queueIsNotEmpty = function() {
        return this.option("_queue") ? false : !!this._queue.length
    };
    _proto3._pushItemToQueue = function($item, config) {
        this._queue.push({
            $item: $item,
            config: config
        })
    };
    _proto3._shiftItemFromQueue = function() {
        return this._queue.shift()
    };
    _proto3._initMarkup = function() {
        this._layout.renderBox();
        _CollectionWidget.prototype._initMarkup.call(this);
        this._renderActions()
    };
    _proto3._renderActions = function() {
        this._onItemStateChanged = this._createActionByOption("onItemStateChanged")
    };
    _proto3._renderItems = function(items) {
        var _this = this;
        this._layout.initSize();
        _CollectionWidget.prototype._renderItems.call(this, items);
        while (this._queueIsNotEmpty()) {
            var item = this._shiftItemFromQueue();
            this._createComponent(item.$item, Box, (0, _extend.extend)({
                _layoutStrategy: this.option("_layoutStrategy"),
                itemTemplate: this.option("itemTemplate"),
                itemHoldTimeout: this.option("itemHoldTimeout"),
                onItemHold: this.option("onItemHold"),
                onItemClick: this.option("onItemClick"),
                onItemContextMenu: this.option("onItemContextMenu"),
                onItemRendered: this.option("onItemRendered"),
                _queue: this._queue
            }, item.config))
        }
        this._layout.renderItems(this._itemElements());
        clearTimeout(this._updateTimer);
        this._updateTimer = setTimeout((function() {
            if (!_this._isUpdated) {
                _this._layout.update()
            }
            _this._isUpdated = false;
            _this._updateTimer = null
        }))
    };
    _proto3._renderItemContent = function(args) {
        var $itemNode = args.itemData && args.itemData.node;
        if ($itemNode) {
            return this._renderItemContentByNode(args, $itemNode)
        }
        return _CollectionWidget.prototype._renderItemContent.call(this, args)
    };
    _proto3._postprocessRenderItem = function(args) {
        var boxConfig = args.itemData.box;
        if (!boxConfig) {
            return
        }
        this._pushItemToQueue(args.itemContent, boxConfig)
    };
    _proto3._createItemByTemplate = function(itemTemplate, args) {
        if (args.itemData.box) {
            return itemTemplate.source ? itemTemplate.source() : (0, _renderer.default)()
        }
        return _CollectionWidget.prototype._createItemByTemplate.call(this, itemTemplate, args)
    };
    _proto3._visibilityChanged = function(visible) {
        if (visible) {
            this._dimensionChanged()
        }
    };
    _proto3._dimensionChanged = function() {
        if (this._updateTimer) {
            return
        }
        this._isUpdated = true;
        this._layout.update()
    };
    _proto3._dispose = function() {
        clearTimeout(this._updateTimer);
        _CollectionWidget.prototype._dispose.apply(this, arguments)
    };
    _proto3._itemOptionChanged = function(item, property, value, oldValue) {
        if ("visible" === property) {
            this._onItemStateChanged({
                name: property,
                state: value,
                oldState: false !== oldValue
            })
        }
        _CollectionWidget.prototype._itemOptionChanged.call(this, item, property, value)
    };
    _proto3._optionChanged = function(args) {
        switch (args.name) {
            case "_layoutStrategy":
            case "_queue":
            case "direction":
                this._invalidate();
                break;
            case "align":
                this._layout.renderAlign();
                break;
            case "crossAlign":
                this._layout.renderCrossAlign();
                break;
            default:
                _CollectionWidget.prototype._optionChanged.call(this, args)
        }
    };
    _proto3._itemOptions = function() {
        var _this2 = this;
        var options = _CollectionWidget.prototype._itemOptions.call(this);
        options.fireItemStateChangedAction = function(e) {
            _this2._onItemStateChanged(e)
        };
        return options
    };
    _proto3.repaint = function() {
        this._dimensionChanged()
    };
    return Box
}(_uiCollection_widget.default);
Box.ItemClass = BoxItem;
(0, _component_registrator.default)("dxBox", Box);
var _default = Box;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
