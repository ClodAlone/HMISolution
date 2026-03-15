/**
 * DevExtreme (cjs/ui/date_box/ui.date_box.strategy.list.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _size = require("../../core/utils/size");
var _window = require("../../core/utils/window");
var _list_light = _interopRequireDefault(require("../list_light"));
require("../list/modules/selection");
var _uiDate_box = _interopRequireDefault(require("./ui.date_box.strategy"));
var _common = require("../../core/utils/common");
var _type = require("../../core/utils/type");
var _extend = require("../../core/utils/extend");
var _ui = _interopRequireDefault(require("./ui.date_utils"));
var _date = _interopRequireDefault(require("../../localization/date"));
var _date_serialization = _interopRequireDefault(require("../../core/utils/date_serialization"));
var _utils = require("../drop_down_editor/utils");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var window = (0, _window.getWindow)();
var DATE_FORMAT = "date";
var BOUNDARY_VALUES = {
    min: new Date(0, 0, 0, 0, 0),
    max: new Date(0, 0, 0, 23, 59)
};
var ListStrategy = _uiDate_box.default.inherit({
    NAME: "List",
    supportedKeys: function() {
        return {
            tab: function() {
                var _this$option = this.option(),
                    opened = _this$option.opened,
                    applyValueMode = _this$option.applyValueMode;
                if (opened && "instantly" === applyValueMode) {
                    this.close()
                }
            },
            space: _common.noop,
            home: _common.noop,
            end: _common.noop
        }
    },
    getDefaultOptions: function() {
        return (0, _extend.extend)(this.callBase(), {
            applyValueMode: "instantly"
        })
    },
    getDisplayFormat: function(displayFormat) {
        return displayFormat || "shorttime"
    },
    popupConfig: function(_popupConfig) {
        return _popupConfig
    },
    getValue: function() {
        var selectedIndex = this._widget.option("selectedIndex");
        if (-1 === selectedIndex) {
            return this.dateBox.option("value")
        }
        var itemData = this._widgetItems[selectedIndex];
        return this._getDateByItemData(itemData)
    },
    useCurrentDateByDefault: function() {
        return true
    },
    getDefaultDate: function() {
        return new Date(null)
    },
    popupShowingHandler: function() {
        this.dateBox._dimensionChanged()
    },
    _renderWidget: function() {
        this.callBase();
        this._refreshItems()
    },
    _getWidgetName: function() {
        return _list_light.default
    },
    _getWidgetOptions: function() {
        return {
            itemTemplate: this._timeListItemTemplate.bind(this),
            onItemClick: this._listItemClickHandler.bind(this),
            tabIndex: -1,
            onFocusedItemChanged: this._refreshActiveDescendant.bind(this),
            selectionMode: "single"
        }
    },
    _refreshActiveDescendant: function(e) {
        this.dateBox.setAria("activedescendant", "");
        this.dateBox.setAria("activedescendant", e.actionValue)
    },
    _refreshItems: function() {
        this._widgetItems = this._getTimeListItems();
        this._widget.option("items", this._widgetItems)
    },
    renderOpenedState: function() {
        if (!this._widget) {
            return
        }
        this._widget.option("focusedElement", null);
        this._setSelectedItemsByValue();
        if (this._widget.option("templatesRenderAsynchronously")) {
            this._asyncScrollTimeout = setTimeout(this._scrollToSelectedItem.bind(this))
        } else {
            this._scrollToSelectedItem()
        }
    },
    dispose: function() {
        this.callBase();
        clearTimeout(this._asyncScrollTimeout)
    },
    _updateValue: function() {
        if (!this._widget) {
            return
        }
        this._refreshItems();
        this._setSelectedItemsByValue();
        this._scrollToSelectedItem()
    },
    _setSelectedItemsByValue: function() {
        var value = this.dateBoxValue();
        var dateIndex = this._getDateIndex(value);
        if (-1 === dateIndex) {
            this._widget.option("selectedItems", [])
        } else {
            this._widget.option("selectedIndex", dateIndex)
        }
    },
    _scrollToSelectedItem: function() {
        this._widget.scrollToItem(this._widget.option("selectedIndex"))
    },
    _getDateIndex: function(date) {
        var result = -1;
        for (var i = 0, n = this._widgetItems.length; i < n; i++) {
            if (this._areDatesEqual(date, this._widgetItems[i])) {
                result = i;
                break
            }
        }
        return result
    },
    _areDatesEqual: function(first, second) {
        return (0, _type.isDate)(first) && (0, _type.isDate)(second) && first.getHours() === second.getHours() && first.getMinutes() === second.getMinutes()
    },
    _getTimeListItems: function() {
        var min = this.dateBox.dateOption("min") || this._getBoundaryDate("min");
        var max = this.dateBox.dateOption("max") || this._getBoundaryDate("max");
        var value = this.dateBox.dateOption("value") || null;
        var delta = max - min;
        var minutes = min.getMinutes() % this.dateBox.option("interval");
        if (delta < 0) {
            return []
        }
        if (delta > _ui.default.ONE_DAY) {
            delta = _ui.default.ONE_DAY
        }
        if (value - min < _ui.default.ONE_DAY) {
            return this._getRangeItems(min, new Date(min), delta)
        }
        min = this._getBoundaryDate("min");
        min.setMinutes(minutes);
        if (value && Math.abs(value - max) < _ui.default.ONE_DAY) {
            delta = (60 * max.getHours() + Math.abs(max.getMinutes() - minutes)) * _ui.default.ONE_MINUTE
        }
        return this._getRangeItems(min, new Date(min), delta)
    },
    _getRangeItems: function(startValue, currentValue, rangeDuration) {
        var rangeItems = [];
        var interval = this.dateBox.option("interval");
        while (currentValue - startValue <= rangeDuration) {
            rangeItems.push(new Date(currentValue));
            currentValue.setMinutes(currentValue.getMinutes() + interval)
        }
        return rangeItems
    },
    _getBoundaryDate: function(boundary) {
        var boundaryValue = BOUNDARY_VALUES[boundary];
        var currentValue = new Date((0, _common.ensureDefined)(this.dateBox.dateOption("value"), 0));
        return new Date(currentValue.getFullYear(), currentValue.getMonth(), currentValue.getDate(), boundaryValue.getHours(), boundaryValue.getMinutes())
    },
    _timeListItemTemplate: function(itemData) {
        var displayFormat = this.dateBox.option("displayFormat");
        return _date.default.format(itemData, this.getDisplayFormat(displayFormat))
    },
    _listItemClickHandler: function(e) {
        if ("useButtons" === this.dateBox.option("applyValueMode")) {
            return
        }
        var date = this._getDateByItemData(e.itemData);
        this.dateBox.option("opened", false);
        this.dateBoxValue(date, e.event)
    },
    _getDateByItemData: function(itemData) {
        var date = this.dateBox.option("value");
        var hours = itemData.getHours();
        var minutes = itemData.getMinutes();
        var seconds = itemData.getSeconds();
        var year = itemData.getFullYear();
        var month = itemData.getMonth();
        var day = itemData.getDate();
        if (date) {
            if (this.dateBox.option("dateSerializationFormat")) {
                date = _date_serialization.default.deserializeDate(date)
            } else {
                date = new Date(date)
            }
            date.setHours(hours);
            date.setMinutes(minutes);
            date.setSeconds(seconds);
            date.setFullYear(year);
            date.setMonth(month);
            date.setDate(day)
        } else {
            date = new Date(year, month, day, hours, minutes, 0, 0)
        }
        return date
    },
    getKeyboardListener: function() {
        return this._widget
    },
    _updatePopupHeight: function() {
        var dropDownOptionsHeight = (0, _utils.getSizeValue)(this.dateBox.option("dropDownOptions.height"));
        if (void 0 === dropDownOptionsHeight || "auto" === dropDownOptionsHeight) {
            this.dateBox._setPopupOption("height", "auto");
            var popupHeight = (0, _size.getOuterHeight)(this._widget.$element());
            var maxHeight = .45 * (0, _size.getHeight)(window);
            this.dateBox._setPopupOption("height", Math.min(popupHeight, maxHeight))
        }
        this.dateBox._timeList && this.dateBox._timeList.updateDimensions()
    },
    getParsedText: function(text, format) {
        var value = this.callBase(text, format);
        if (value) {
            value = _ui.default.mergeDates(value, new Date(null), DATE_FORMAT)
        }
        return value
    }
});
var _default = ListStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
