/**
 * DevExtreme (esm/ui/date_box/ui.date_view_roller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import registerComponent from "../../core/component_registrator";
import {
    extend
} from "../../core/utils/extend";
import {
    each
} from "../../core/utils/iterator";
import {
    addNamespace
} from "../../events/utils/index";
import {
    name as clickEventName
} from "../../events/click";
import Scrollable from "../scroll_view/ui.scrollable";
import devices from "../../core/devices";
import fx from "../../animation/fx";
import {
    resetPosition
} from "../../animation/translator";
import {
    convertToLocation
} from "../../renovation/ui/scroll_view/utils/convert_location";
var DATEVIEW_ROLLER_CLASS = "dx-dateviewroller";
var DATEVIEW_ROLLER_ACTIVE_CLASS = "dx-state-active";
var DATEVIEW_ROLLER_CURRENT_CLASS = "dx-dateviewroller-current";
var DATEVIEW_ROLLER_ITEM_CLASS = "dx-dateview-item";
var DATEVIEW_ROLLER_ITEM_SELECTED_CLASS = "dx-dateview-item-selected";
var DATEVIEW_ROLLER_ITEM_SELECTED_FRAME_CLASS = "dx-dateview-item-selected-frame";
var DATEVIEW_ROLLER_ITEM_SELECTED_BORDER_CLASS = "dx-dateview-item-selected-border";
class DateViewRoller extends Scrollable {
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
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
    }
    _init() {
        super._init();
        this.option("onVisibilityChange", this._visibilityChangedHandler.bind(this));
        this.option("onEnd", this._endActionHandler.bind(this))
    }
    _render() {
        super._render();
        this._renderSelectedItemFrame();
        this.$element().addClass(DATEVIEW_ROLLER_CLASS);
        this._renderContainerClick();
        this._renderItems();
        this._renderSelectedValue();
        this._renderItemsClick();
        this._renderWheelEvent();
        this._renderSelectedIndexChanged()
    }
    _renderSelectedIndexChanged() {
        this._selectedIndexChanged = this._createActionByOption("onSelectedIndexChanged")
    }
    _renderWheelEvent() {
        eventsEngine.on($(this.container()), "dxmousewheel", e => {
            this._isWheelScrolled = true
        })
    }
    _renderContainerClick() {
        if (!this.option("showOnClick")) {
            return
        }
        var eventName = addNamespace(clickEventName, this.NAME);
        var clickAction = this._createActionByOption("onClick");
        eventsEngine.off($(this.container()), eventName);
        eventsEngine.on($(this.container()), eventName, (function(e) {
            clickAction({
                event: e
            })
        }))
    }
    _renderItems() {
        var items = this.option("items") || [];
        var $items = $();
        $(this.content()).empty();
        items.forEach((function(item) {
            $items = $items.add($("<div>").addClass(DATEVIEW_ROLLER_ITEM_CLASS).append(item))
        }));
        $(this.content()).append($items);
        this._$items = $items;
        this.update()
    }
    _renderSelectedItemFrame() {
        $("<div>").addClass(DATEVIEW_ROLLER_ITEM_SELECTED_FRAME_CLASS).append($("<div>").addClass(DATEVIEW_ROLLER_ITEM_SELECTED_BORDER_CLASS)).appendTo($(this.container()))
    }
    _renderSelectedValue(selectedIndex) {
        var index = this._fitIndex(null !== selectedIndex && void 0 !== selectedIndex ? selectedIndex : this.option("selectedIndex"));
        this._moveTo({
            top: this._getItemPosition(index)
        });
        this._renderActiveStateItem()
    }
    _fitIndex(index) {
        var items = this.option("items") || [];
        var itemCount = items.length;
        if (index >= itemCount) {
            return itemCount - 1
        }
        if (index < 0) {
            return 0
        }
        return index
    }
    _getItemPosition(index) {
        return Math.round(this._itemHeight() * index)
    }
    _renderItemsClick() {
        var itemSelector = this._getItemSelector();
        var eventName = addNamespace(clickEventName, this.NAME);
        eventsEngine.off(this.$element(), eventName, itemSelector);
        eventsEngine.on(this.$element(), eventName, itemSelector, this._itemClickHandler.bind(this))
    }
    _getItemSelector() {
        return "." + DATEVIEW_ROLLER_ITEM_CLASS
    }
    _itemClickHandler(e) {
        this.option("selectedIndex", this._itemElementIndex(e.currentTarget))
    }
    _itemElementIndex(itemElement) {
        return this._itemElements().index(itemElement)
    }
    _itemElements() {
        return this.$element().find(this._getItemSelector())
    }
    _renderActiveStateItem() {
        var selectedIndex = this.option("selectedIndex");
        each(this._$items, (function(index) {
            $(this).toggleClass(DATEVIEW_ROLLER_ITEM_SELECTED_CLASS, selectedIndex === index)
        }))
    }
    _shouldScrollToNeighborItem() {
        return "desktop" === devices.real().deviceType && this._isWheelScrolled
    }
    _moveTo(targetLocation) {
        var {
            top: top,
            left: left
        } = convertToLocation(targetLocation);
        var location = this.scrollOffset();
        var delta = {
            x: location.left - left,
            y: location.top - top
        };
        if (this._isVisible() && (delta.x || delta.y)) {
            this._prepareDirections(true);
            if (this._animation && !this._shouldScrollToNeighborItem()) {
                var that = this;
                fx.stop($(this.content()));
                fx.animate($(this.content()), {
                    duration: 200,
                    type: "slide",
                    to: {
                        top: Math.floor(delta.y)
                    },
                    complete() {
                        resetPosition($(that.content()));
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
    }
    _validate(e) {
        return this._moveIsAllowed(e)
    }
    _fitSelectedIndexInRange(index) {
        var itemsCount = this.option("items").length;
        return Math.max(Math.min(index, itemsCount - 1), 0)
    }
    _isInNullNeighborhood(x) {
        return -.1 <= x && x <= .1
    }
    _getSelectedIndexAfterScroll(currentSelectedIndex) {
        var locationTop = this.scrollOffset().top;
        var currentSelectedIndexPosition = currentSelectedIndex * this._itemHeight();
        var dy = locationTop - currentSelectedIndexPosition;
        if (this._isInNullNeighborhood(dy)) {
            return currentSelectedIndex
        }
        var direction = dy > 0 ? 1 : -1;
        var newSelectedIndex = this._fitSelectedIndexInRange(currentSelectedIndex + direction);
        return newSelectedIndex
    }
    _getNewSelectedIndex(currentSelectedIndex) {
        if (this._shouldScrollToNeighborItem()) {
            return this._getSelectedIndexAfterScroll(currentSelectedIndex)
        }
        this._animation = true;
        var ratio = this.scrollOffset().top / this._itemHeight();
        return Math.round(ratio)
    }
    _endActionHandler() {
        var currentSelectedIndex = this.option("selectedIndex");
        var newSelectedIndex = this._getNewSelectedIndex(currentSelectedIndex);
        if (newSelectedIndex === currentSelectedIndex) {
            this._renderSelectedValue(newSelectedIndex)
        } else {
            this.option("selectedIndex", newSelectedIndex)
        }
        this._isWheelScrolled = false
    }
    _itemHeight() {
        var $item = this._$items.first();
        return getHeight($item)
    }
    _toggleActive(state) {
        this.$element().toggleClass(DATEVIEW_ROLLER_ACTIVE_CLASS, state)
    }
    _isVisible() {
        return $(this.container()).is(":visible")
    }
    _fireSelectedIndexChanged(value, previousValue) {
        this._selectedIndexChanged({
            value: value,
            previousValue: previousValue,
            event: void 0
        })
    }
    _visibilityChanged(visible) {
        super._visibilityChanged(visible);
        this._visibilityChangedHandler(visible)
    }
    _visibilityChangedHandler(visible) {
        if (visible) {
            this._visibilityTimer = setTimeout(() => {
                this._renderSelectedValue(this.option("selectedIndex"))
            })
        }
        this.toggleActiveState(false)
    }
    toggleActiveState(state) {
        this.$element().toggleClass(DATEVIEW_ROLLER_CURRENT_CLASS, state)
    }
    _refreshSelectedIndex() {
        var selectedIndex = this.option("selectedIndex");
        var fitIndex = this._fitIndex(selectedIndex);
        if (fitIndex === selectedIndex) {
            this._renderActiveStateItem()
        } else {
            this.option("selectedIndex", fitIndex)
        }
    }
    _optionChanged(args) {
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
                super._optionChanged(args)
        }
    }
    _dispose() {
        clearTimeout(this._visibilityTimer);
        super._dispose()
    }
}
registerComponent("dxDateViewRoller", DateViewRoller);
export default DateViewRoller;
