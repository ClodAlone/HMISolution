/**
 * DevExtreme (cjs/ui/date_box/ui.date_view_roller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _size = require("../../core/utils/size");
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _events_engine = _interopRequireDefault(require("../../events/core/events_engine"));
var _component_registrator = _interopRequireDefault(require("../../core/component_registrator"));
var _extend = require("../../core/utils/extend");
var _iterator = require("../../core/utils/iterator");
var _index = require("../../events/utils/index");
var _click = require("../../events/click");
var _ui = _interopRequireDefault(require("../scroll_view/ui.scrollable"));
var _devices = _interopRequireDefault(require("../../core/devices"));
var _fx = _interopRequireDefault(require("../../animation/fx"));
var _translator = require("../../animation/translator");
var _convert_location = require("../../renovation/ui/scroll_view/utils/convert_location");

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
var DATEVIEW_ROLLER_CLASS = "dx-dateviewroller";
var DATEVIEW_ROLLER_ACTIVE_CLASS = "dx-state-active";
var DATEVIEW_ROLLER_CURRENT_CLASS = "dx-dateviewroller-current";
var DATEVIEW_ROLLER_ITEM_CLASS = "dx-dateview-item";
var DATEVIEW_ROLLER_ITEM_SELECTED_CLASS = "dx-dateview-item-selected";
var DATEVIEW_ROLLER_ITEM_SELECTED_FRAME_CLASS = "dx-dateview-item-selected-frame";
var DATEVIEW_ROLLER_ITEM_SELECTED_BORDER_CLASS = "dx-dateview-item-selected-border";
var DateViewRoller = function(_Scrollable) {
    _inheritsLoose(DateViewRoller, _Scrollable);

    function DateViewRoller() {
        return _Scrollable.apply(this, arguments) || this
    }
    var _proto = DateViewRoller.prototype;
    _proto._getDefaultOptions = function() {
        return (0, _extend.extend)(_Scrollable.prototype._getDefaultOptions.call(this), {
            showScrollbar: "never",
            useNative: false,
            selectedIndex: 0,
            bounceEnabled: false,
            items: [],
            showOnClick: false,
            onClick: null,
            onSelectedIndexChanged: null,
            scrollByContent: true
        })
    };
    _proto._init = function() {
        _Scrollable.prototype._init.call(this);
        this.option("onVisibilityChange", this._visibilityChangedHandler.bind(this));
        this.option("onEnd", this._endActionHandler.bind(this))
    };
    _proto._render = function() {
        _Scrollable.prototype._render.call(this);
        this._renderSelectedItemFrame();
        this.$element().addClass(DATEVIEW_ROLLER_CLASS);
        this._renderContainerClick();
        this._renderItems();
        this._renderSelectedValue();
        this._renderItemsClick();
        this._renderWheelEvent();
        this._renderSelectedIndexChanged()
    };
    _proto._renderSelectedIndexChanged = function() {
        this._selectedIndexChanged = this._createActionByOption("onSelectedIndexChanged")
    };
    _proto._renderWheelEvent = function() {
        var _this = this;
        _events_engine.default.on((0, _renderer.default)(this.container()), "dxmousewheel", (function(e) {
            _this._isWheelScrolled = true
        }))
    };
    _proto._renderContainerClick = function() {
        if (!this.option("showOnClick")) {
            return
        }
        var eventName = (0, _index.addNamespace)(_click.name, this.NAME);
        var clickAction = this._createActionByOption("onClick");
        _events_engine.default.off((0, _renderer.default)(this.container()), eventName);
        _events_engine.default.on((0, _renderer.default)(this.container()), eventName, (function(e) {
            clickAction({
                event: e
            })
        }))
    };
    _proto._renderItems = function() {
        var items = this.option("items") || [];
        var $items = (0, _renderer.default)();
        (0, _renderer.default)(this.content()).empty();
        items.forEach((function(item) {
            $items = $items.add((0, _renderer.default)("<div>").addClass(DATEVIEW_ROLLER_ITEM_CLASS).append(item))
        }));
        (0, _renderer.default)(this.content()).append($items);
        this._$items = $items;
        this.update()
    };
    _proto._renderSelectedItemFrame = function() {
        (0, _renderer.default)("<div>").addClass(DATEVIEW_ROLLER_ITEM_SELECTED_FRAME_CLASS).append((0, _renderer.default)("<div>").addClass(DATEVIEW_ROLLER_ITEM_SELECTED_BORDER_CLASS)).appendTo((0, _renderer.default)(this.container()))
    };
    _proto._renderSelectedValue = function(selectedIndex) {
        var index = this._fitIndex(null !== selectedIndex && void 0 !== selectedIndex ? selectedIndex : this.option("selectedIndex"));
        this._moveTo({
            top: this._getItemPosition(index)
        });
        this._renderActiveStateItem()
    };
    _proto._fitIndex = function(index) {
        var items = this.option("items") || [];
        var itemCount = items.length;
        if (index >= itemCount) {
            return itemCount - 1
        }
        if (index < 0) {
            return 0
        }
        return index
    };
    _proto._getItemPosition = function(index) {
        return Math.round(this._itemHeight() * index)
    };
    _proto._renderItemsClick = function() {
        var itemSelector = this._getItemSelector();
        var eventName = (0, _index.addNamespace)(_click.name, this.NAME);
        _events_engine.default.off(this.$element(), eventName, itemSelector);
        _events_engine.default.on(this.$element(), eventName, itemSelector, this._itemClickHandler.bind(this))
    };
    _proto._getItemSelector = function() {
        return "." + DATEVIEW_ROLLER_ITEM_CLASS
    };
    _proto._itemClickHandler = function(e) {
        this.option("selectedIndex", this._itemElementIndex(e.currentTarget))
    };
    _proto._itemElementIndex = function(itemElement) {
        return this._itemElements().index(itemElement)
    };
    _proto._itemElements = function() {
        return this.$element().find(this._getItemSelector())
    };
    _proto._renderActiveStateItem = function() {
        var selectedIndex = this.option("selectedIndex");
        (0, _iterator.each)(this._$items, (function(index) {
            (0, _renderer.default)(this).toggleClass(DATEVIEW_ROLLER_ITEM_SELECTED_CLASS, selectedIndex === index)
        }))
    };
    _proto._shouldScrollToNeighborItem = function() {
        return "desktop" === _devices.default.real().deviceType && this._isWheelScrolled
    };
    _proto._moveTo = function(targetLocation) {
        var _convertToLocation = (0, _convert_location.convertToLocation)(targetLocation),
            top = _convertToLocation.top,
            left = _convertToLocation.left;
        var location = this.scrollOffset();
        var delta = {
            x: location.left - left,
            y: location.top - top
        };
        if (this._isVisible() && (delta.x || delta.y)) {
            this._prepareDirections(true);
            if (this._animation && !this._shouldScrollToNeighborItem()) {
                var that = this;
                _fx.default.stop((0, _renderer.default)(this.content()));
                _fx.default.animate((0, _renderer.default)(this.content()), {
                    duration: 200,
                    type: "slide",
                    to: {
                        top: Math.floor(delta.y)
                    },
                    complete: function() {
                        (0, _translator.resetPosition)((0, _renderer.default)(that.content()));
                        that.handleMove({
                            delta: delta
                        })
                    }
                });
                delete this._animation
            } else {
                this.handleMove({
                    delta: delta
                })
            }
        }
    };
    _proto._validate = function(e) {
        return this._moveIsAllowed(e)
    };
    _proto._fitSelectedIndexInRange = function(index) {
        var itemsCount = this.option("items").length;
        return Math.max(Math.min(index, itemsCount - 1), 0)
    };
    _proto._isInNullNeighborhood = function(x) {
        return -.1 <= x && x <= .1
    };
    _proto._getSelectedIndexAfterScroll = function(currentSelectedIndex) {
        var locationTop = this.scrollOffset().top;
        var currentSelectedIndexPosition = currentSelectedIndex * this._itemHeight();
        var dy = locationTop - currentSelectedIndexPosition;
        if (this._isInNullNeighborhood(dy)) {
            return currentSelectedIndex
        }
        var direction = dy > 0 ? 1 : -1;
        var newSelectedIndex = this._fitSelectedIndexInRange(currentSelectedIndex + direction);
        return newSelectedIndex
    };
    _proto._getNewSelectedIndex = function(currentSelectedIndex) {
        if (this._shouldScrollToNeighborItem()) {
            return this._getSelectedIndexAfterScroll(currentSelectedIndex)
        }
        this._animation = true;
        var ratio = this.scrollOffset().top / this._itemHeight();
        return Math.round(ratio)
    };
    _proto._endActionHandler = function() {
        var currentSelectedIndex = this.option("selectedIndex");
        var newSelectedIndex = this._getNewSelectedIndex(currentSelectedIndex);
        if (newSelectedIndex === currentSelectedIndex) {
            this._renderSelectedValue(newSelectedIndex)
        } else {
            this.option("selectedIndex", newSelectedIndex)
        }
        this._isWheelScrolled = false
    };
    _proto._itemHeight = function() {
        var $item = this._$items.first();
        return (0, _size.getHeight)($item)
    };
    _proto._toggleActive = function(state) {
        this.$element().toggleClass(DATEVIEW_ROLLER_ACTIVE_CLASS, state)
    };
    _proto._isVisible = function() {
        return (0, _renderer.default)(this.container()).is(":visible")
    };
    _proto._fireSelectedIndexChanged = function(value, previousValue) {
        this._selectedIndexChanged({
            value: value,
            previousValue: previousValue,
            event: void 0
        })
    };
    _proto._visibilityChanged = function(visible) {
        _Scrollable.prototype._visibilityChanged.call(this, visible);
        this._visibilityChangedHandler(visible)
    };
    _proto._visibilityChangedHandler = function(visible) {
        var _this2 = this;
        if (visible) {
            this._visibilityTimer = setTimeout((function() {
                _this2._renderSelectedValue(_this2.option("selectedIndex"))
            }))
        }
        this.toggleActiveState(false)
    };
    _proto.toggleActiveState = function(state) {
        this.$element().toggleClass(DATEVIEW_ROLLER_CURRENT_CLASS, state)
    };
    _proto._refreshSelectedIndex = function() {
        var selectedIndex = this.option("selectedIndex");
        var fitIndex = this._fitIndex(selectedIndex);
        if (fitIndex === selectedIndex) {
            this._renderActiveStateItem()
        } else {
            this.option("selectedIndex", fitIndex)
        }
    };
    _proto._optionChanged = function(args) {
        switch (args.name) {
            case "selectedIndex":
                this._fireSelectedIndexChanged(args.value, args.previousValue);
                this._renderSelectedValue(args.value);
                break;
            case "items":
                this._renderItems();
                this._refreshSelectedIndex();
                break;
            case "onClick":
            case "showOnClick":
                this._renderContainerClick();
                break;
            case "onSelectedIndexChanged":
                this._renderSelectedIndexChanged();
                break;
            default:
                _Scrollable.prototype._optionChanged.call(this, args)
        }
    };
    _proto._dispose = function() {
        clearTimeout(this._visibilityTimer);
        _Scrollable.prototype._dispose.call(this)
    };
    return DateViewRoller
}(_ui.default);
(0, _component_registrator.default)("dxDateViewRoller", DateViewRoller);
var _default = DateViewRoller;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
