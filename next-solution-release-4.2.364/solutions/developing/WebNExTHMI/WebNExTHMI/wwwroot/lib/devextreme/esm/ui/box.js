/**
 * DevExtreme (esm/ui/box.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import eventsEngine from "../events/core/events_engine";
import registerComponent from "../core/component_registrator";
import {
    extend
} from "../core/utils/extend";
import {
    noop
} from "../core/utils/common";
import {
    hasWindow
} from "../core/utils/window";
import {
    dasherize
} from "../core/utils/inflector";
import {
    isDefined
} from "../core/utils/type";
import {
    normalizeStyleProp,
    styleProp,
    stylePropPrefix
} from "../core/utils/style";
import {
    each
} from "../core/utils/iterator";
import {
    getWidth,
    setWidth,
    getHeight,
    setHeight
} from "../core/utils/size";
import CollectionWidgetItem from "./collection/item";
import CollectionWidget from "./collection/ui.collection_widget.edit";
var BOX_CLASS = "dx-box";
var BOX_SELECTOR = ".dx-box";
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
var FLEX_JUSTIFY_CONTENT_MAP = {
    start: "flex-start",
    end: "flex-end",
    center: "center",
    "space-between": "space-between",
    "space-around": "space-around"
};
var FLEX_ALIGN_ITEMS_MAP = {
    start: "flex-start",
    end: "flex-end",
    center: "center",
    stretch: "stretch"
};
var FLEX_DIRECTION_MAP = {
    row: "row",
    col: "column"
};
var setFlexProp = (element, prop, value) => {
    value = normalizeStyleProp(prop, value);
    element.style[styleProp(prop)] = value;
    if (!hasWindow()) {
        if ("" === value || !isDefined(value)) {
            return
        }
        var cssName = dasherize(prop);
        var styleExpr = cssName + ": " + value + ";";
        if (!element.attributes.style) {
            element.setAttribute("style", styleExpr)
        } else if (element.attributes.style.value.indexOf(styleExpr) < 0) {
            element.attributes.style.value += " " + styleExpr
        }
    }
};
var BOX_EVENTNAMESPACE = "dxBox";
var UPDATE_EVENT = "dxupdate." + BOX_EVENTNAMESPACE;
var FALLBACK_BOX_ITEM = "dx-box-fallback-item";
var FALLBACK_WRAP_MAP = {
    row: "nowrap",
    col: "normal"
};
var FALLBACK_MAIN_SIZE_MAP = {
    row: {
        name: "width",
        getter: getWidth,
        setter: setWidth
    },
    col: {
        name: "height",
        getter: getHeight,
        setter: setHeight
    }
};
var FALLBACK_CROSS_SIZE_MAP = {
    row: FALLBACK_MAIN_SIZE_MAP.col,
    col: FALLBACK_MAIN_SIZE_MAP.row
};
var FALLBACK_PRE_MARGIN_MAP = {
    row: "marginLeft",
    col: "marginTop"
};
var FALLBACK_POST_MARGIN_MAP = {
    row: "marginRight",
    col: "marginBottom"
};
var FALLBACK_CROSS_PRE_MARGIN_MAP = {
    row: "marginTop",
    col: "marginLeft"
};
var FALLBACK_CROSS_POST_MARGIN_MAP = {
    row: "marginBottom",
    col: "marginRight"
};
var MARGINS_RTL_FLIP_MAP = {
    marginLeft: "marginRight",
    marginRight: "marginLeft"
};
class BoxItem extends CollectionWidgetItem {
    _renderVisible(value, oldValue) {
        super._renderVisible(value);
        if (isDefined(oldValue)) {
            this._options.fireItemStateChangedAction({
                name: "visible",
                state: value,
                oldState: oldValue
            })
        }
    }
}
class FlexLayoutStrategy {
    constructor($element, option) {
        this._$element = $element;
        this._option = option;
        this.initSize = noop;
        this.update = noop
    }
    renderBox() {
        this._$element.css({
            display: stylePropPrefix("flexDirection") + "flex"
        });
        setFlexProp(this._$element.get(0), "flexDirection", FLEX_DIRECTION_MAP[this._option("direction")])
    }
    renderAlign() {
        this._$element.css({
            justifyContent: this._normalizedAlign()
        })
    }
    _normalizedAlign() {
        var align = this._option("align");
        return align in FLEX_JUSTIFY_CONTENT_MAP ? FLEX_JUSTIFY_CONTENT_MAP[align] : align
    }
    renderCrossAlign() {
        this._$element.css({
            alignItems: this._normalizedCrossAlign()
        })
    }
    _normalizedCrossAlign() {
        var crossAlign = this._option("crossAlign");
        return crossAlign in FLEX_ALIGN_ITEMS_MAP ? FLEX_ALIGN_ITEMS_MAP[crossAlign] : crossAlign
    }
    renderItems($items) {
        var flexPropPrefix = stylePropPrefix("flexDirection");
        var direction = this._option("direction");
        each($items, (function() {
            var $item = $(this);
            var item = $item.data(BOX_ITEM_DATA_KEY);
            $item.css({
                display: flexPropPrefix + "flex"
            }).css(MAXSIZE_MAP[direction], item.maxSize || "none").css(MINSIZE_MAP[direction], item.minSize || "0");
            setFlexProp($item.get(0), "flexBasis", item.baseSize || 0);
            setFlexProp($item.get(0), "flexGrow", item.ratio);
            setFlexProp($item.get(0), "flexShrink", isDefined(item.shrink) ? item.shrink : SHRINK);
            $item.children().each((_, itemContent) => {
                $(itemContent).css({
                    width: "auto",
                    height: "auto",
                    display: stylePropPrefix("flexDirection") + "flex",
                    flexBasis: 0
                });
                setFlexProp(itemContent, "flexGrow", 1);
                setFlexProp(itemContent, "flexDirection", $(itemContent)[0].style.flexDirection || "column")
            })
        }))
    }
}
class FallbackLayoutStrategy {
    constructor($element, option) {
        this._$element = $element;
        this._option = option
    }
    renderBox() {
        this._$element.css({
            fontSize: 0,
            whiteSpace: FALLBACK_WRAP_MAP[this._option("direction")],
            verticalAlign: "top"
        });
        eventsEngine.off(this._$element, UPDATE_EVENT);
        eventsEngine.on(this._$element, UPDATE_EVENT, this.update.bind(this))
    }
    renderAlign() {
        var $items = this._$items;
        if (!$items) {
            return
        }
        var align = this._option("align");
        var totalItemSize = this.totalItemSize;
        var direction = this._option("direction");
        var boxSize = FALLBACK_MAIN_SIZE_MAP[direction].getter(this._$element);
        var freeSpace = boxSize - totalItemSize;
        var shift = 0;
        this._setItemsMargins($items, direction, 0);
        switch (align) {
            case "start":
                break;
            case "end":
                shift = freeSpace;
                $items.first().css(this._chooseMarginSide(FALLBACK_PRE_MARGIN_MAP[direction]), shift);
                break;
            case "center":
                shift = .5 * freeSpace;
                $items.first().css(this._chooseMarginSide(FALLBACK_PRE_MARGIN_MAP[direction]), shift);
                $items.last().css(this._chooseMarginSide(FALLBACK_POST_MARGIN_MAP[direction]), shift);
                break;
            case "space-between":
                shift = .5 * freeSpace / ($items.length - 1);
                this._setItemsMargins($items, direction, shift);
                $items.first().css(this._chooseMarginSide(FALLBACK_PRE_MARGIN_MAP[direction]), 0);
                $items.last().css(this._chooseMarginSide(FALLBACK_POST_MARGIN_MAP[direction]), 0);
                break;
            case "space-around":
                shift = .5 * freeSpace / $items.length;
                this._setItemsMargins($items, direction, shift)
        }
    }
    _setItemsMargins($items, direction, shift) {
        $items.css(this._chooseMarginSide(FALLBACK_PRE_MARGIN_MAP[direction]), shift).css(this._chooseMarginSide(FALLBACK_POST_MARGIN_MAP[direction]), shift)
    }
    renderCrossAlign() {
        var $items = this._$items;
        if (!$items) {
            return
        }
        var crossAlign = this._option("crossAlign");
        var direction = this._option("direction");
        var size = FALLBACK_CROSS_SIZE_MAP[direction].getter(this._$element);
        var that = this;
        switch (crossAlign) {
            case "start":
                break;
            case "end":
                each($items, (function() {
                    var $item = $(this);
                    var itemSize = FALLBACK_CROSS_SIZE_MAP[direction].getter($item);
                    var shift = size - itemSize;
                    $item.css(that._chooseMarginSide(FALLBACK_CROSS_PRE_MARGIN_MAP[direction]), shift)
                }));
                break;
            case "center":
                each($items, (function() {
                    var $item = $(this);
                    var itemSize = FALLBACK_CROSS_SIZE_MAP[direction].getter($item);
                    var shift = .5 * (size - itemSize);
                    $item.css(that._chooseMarginSide(FALLBACK_CROSS_PRE_MARGIN_MAP[direction]), shift).css(that._chooseMarginSide(FALLBACK_CROSS_POST_MARGIN_MAP[direction]), shift)
                }));
                break;
            case "stretch":
                $items.css(that._chooseMarginSide(FALLBACK_CROSS_PRE_MARGIN_MAP[direction]), 0).css(that._chooseMarginSide(FALLBACK_CROSS_POST_MARGIN_MAP[direction]), 0).css(FALLBACK_CROSS_SIZE_MAP[direction].name, "100%")
        }
    }
    _chooseMarginSide(value) {
        if (!this._option("rtlEnabled")) {
            return value
        }
        return MARGINS_RTL_FLIP_MAP[value] || value
    }
    renderItems($items) {
        this._$items = $items;
        var direction = this._option("direction");
        var totalRatio = 0;
        var totalWeightedShrink = 0;
        var totalBaseSize = 0;
        each($items, (_, item) => {
            var $item = $(item);
            $item.css({
                display: "inline-block",
                verticalAlign: "top"
            });
            FALLBACK_MAIN_SIZE_MAP[direction].setter($item, "auto");
            $item.removeClass(FALLBACK_BOX_ITEM);
            var itemData = $item.data(BOX_ITEM_DATA_KEY);
            var ratio = itemData.ratio || 0;
            var size = this._baseSize($item);
            var shrink = isDefined(itemData.shrink) ? itemData.shrink : SHRINK;
            totalRatio += ratio;
            totalWeightedShrink += shrink * size;
            totalBaseSize += size
        });
        var freeSpaceSize = this._boxSize() - totalBaseSize;
        var itemSize = $item => {
            var itemData = $item.data(BOX_ITEM_DATA_KEY);
            var size = this._baseSize($item);
            var factor = freeSpaceSize >= 0 ? itemData.ratio || 0 : (isDefined(itemData.shrink) ? itemData.shrink : SHRINK) * size;
            var totalFactor = freeSpaceSize >= 0 ? totalRatio : totalWeightedShrink;
            var shift = totalFactor ? Math.round(freeSpaceSize * factor / totalFactor) : 0;
            return size + shift
        };
        var totalItemSize = 0;
        each($items, (_, item) => {
            var $item = $(item);
            var itemData = $(item).data(BOX_ITEM_DATA_KEY);
            var size = itemSize($item);
            totalItemSize += size;
            $item.css(MAXSIZE_MAP[direction], itemData.maxSize || "none").css(MINSIZE_MAP[direction], itemData.minSize || "0").css(FALLBACK_MAIN_SIZE_MAP[direction].name, size);
            $item.addClass(FALLBACK_BOX_ITEM)
        });
        this.totalItemSize = totalItemSize
    }
    _baseSize(item) {
        var itemData = $(item).data(BOX_ITEM_DATA_KEY);
        return null == itemData.baseSize ? 0 : "auto" === itemData.baseSize ? this._contentSize(item) : this._parseSize(itemData.baseSize)
    }
    _contentSize(item) {
        return FALLBACK_MAIN_SIZE_MAP[this._option("direction")].getter($(item))
    }
    _parseSize(size) {
        return String(size).match(/.+%$/) ? .01 * parseFloat(size) * this._boxSizeValue : size
    }
    _boxSize(value) {
        if (!arguments.length) {
            this._boxSizeValue = this._boxSizeValue || this._totalBaseSize();
            return this._boxSizeValue
        }
        this._boxSizeValue = value
    }
    _totalBaseSize() {
        var result = 0;
        each(this._$items, (_, item) => {
            result += this._baseSize(item)
        });
        return result
    }
    initSize() {
        this._boxSize(FALLBACK_MAIN_SIZE_MAP[this._option("direction")].getter(this._$element))
    }
    update() {
        if (!this._$items || this._$element.is(":hidden")) {
            return
        }
        this._$items.detach();
        this.initSize();
        this._$element.append(this._$items);
        this.renderItems(this._$items);
        this.renderAlign();
        this.renderCrossAlign();
        var element = this._$element.get(0);
        this._$items.find(BOX_SELECTOR).each((function() {
            if (element === $(this).parent().closest(BOX_SELECTOR).get(0)) {
                eventsEngine.triggerHandler(this, UPDATE_EVENT)
            }
        }))
    }
}
class Box extends CollectionWidget {
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            direction: "row",
            align: "start",
            crossAlign: "stretch",
            activeStateEnabled: false,
            focusStateEnabled: false,
            onItemStateChanged: void 0,
            _layoutStrategy: "flex",
            _queue: void 0
        })
    }
    _itemClass() {
        return BOX_ITEM_CLASS
    }
    _itemDataKey() {
        return BOX_ITEM_DATA_KEY
    }
    _itemElements() {
        return this._itemContainer().children(this._itemSelector())
    }
    _init() {
        super._init();
        this.$element().addClass("".concat(BOX_CLASS, "-").concat(this.option("_layoutStrategy")));
        this._initLayout();
        this._initBoxQueue()
    }
    _initLayout() {
        this._layout = "fallback" === this.option("_layoutStrategy") ? new FallbackLayoutStrategy(this.$element(), this.option.bind(this)) : new FlexLayoutStrategy(this.$element(), this.option.bind(this))
    }
    _initBoxQueue() {
        this._queue = this.option("_queue") || []
    }
    _queueIsNotEmpty() {
        return this.option("_queue") ? false : !!this._queue.length
    }
    _pushItemToQueue($item, config) {
        this._queue.push({
            $item: $item,
            config: config
        })
    }
    _shiftItemFromQueue() {
        return this._queue.shift()
    }
    _initMarkup() {
        this.$element().addClass(BOX_CLASS);
        this._layout.renderBox();
        super._initMarkup();
        this._renderAlign();
        this._renderActions()
    }
    _renderActions() {
        this._onItemStateChanged = this._createActionByOption("onItemStateChanged")
    }
    _renderAlign() {
        this._layout.renderAlign();
        this._layout.renderCrossAlign()
    }
    _renderItems(items) {
        this._layout.initSize();
        super._renderItems(items);
        while (this._queueIsNotEmpty()) {
            var item = this._shiftItemFromQueue();
            this._createComponent(item.$item, Box, extend({
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
        this._updateTimer = setTimeout(() => {
            if (!this._isUpdated) {
                this._layout.update()
            }
            this._isUpdated = false;
            this._updateTimer = null
        })
    }
    _renderItemContent(args) {
        var $itemNode = args.itemData && args.itemData.node;
        if ($itemNode) {
            return this._renderItemContentByNode(args, $itemNode)
        }
        return super._renderItemContent(args)
    }
    _postprocessRenderItem(args) {
        var boxConfig = args.itemData.box;
        if (!boxConfig) {
            return
        }
        this._pushItemToQueue(args.itemContent, boxConfig)
    }
    _createItemByTemplate(itemTemplate, args) {
        if (args.itemData.box) {
            return itemTemplate.source ? itemTemplate.source() : $()
        }
        return super._createItemByTemplate(itemTemplate, args)
    }
    _visibilityChanged(visible) {
        if (visible) {
            this._dimensionChanged()
        }
    }
    _dimensionChanged() {
        if (this._updateTimer) {
            return
        }
        this._isUpdated = true;
        this._layout.update()
    }
    _dispose() {
        clearTimeout(this._updateTimer);
        super._dispose.apply(this, arguments)
    }
    _itemOptionChanged(item, property, value, oldValue) {
        if ("visible" === property) {
            this._onItemStateChanged({
                name: property,
                state: value,
                oldState: false !== oldValue
            })
        }
        super._itemOptionChanged(item, property, value)
    }
    _optionChanged(args) {
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
                super._optionChanged(args)
        }
    }
    _itemOptions() {
        var options = super._itemOptions();
        options.fireItemStateChangedAction = e => {
            this._onItemStateChanged(e)
        };
        return options
    }
    repaint() {
        this._dimensionChanged()
    }
}
Box.ItemClass = BoxItem;
registerComponent("dxBox", Box);
export default Box;
