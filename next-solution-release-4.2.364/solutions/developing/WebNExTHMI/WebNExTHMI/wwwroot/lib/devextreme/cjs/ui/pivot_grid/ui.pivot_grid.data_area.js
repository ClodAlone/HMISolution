/**
 * DevExtreme (cjs/ui/pivot_grid/ui.pivot_grid.data_area.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.DataArea = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _uiPivot_grid = require("./ui.pivot_grid.area_item");
var _support = require("../../core/utils/support");
var _calculate_scrollbar_width = require("./utils/calculate_scrollbar_width");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var PIVOTGRID_AREA_CLASS = "dx-pivotgrid-area";
var PIVOTGRID_AREA_DATA_CLASS = "dx-pivotgrid-area-data";
var PIVOTGRID_TOTAL_CLASS = "dx-total";
var PIVOTGRID_GRAND_TOTAL_CLASS = "dx-grandtotal";
var PIVOTGRID_ROW_TOTAL_CLASS = "dx-row-total";
var DataArea = _uiPivot_grid.AreaItem.inherit({
    _getAreaName: function() {
        return "data"
    },
    _createGroupElement: function() {
        return (0, _renderer.default)("<div>").addClass(PIVOTGRID_AREA_CLASS).addClass(PIVOTGRID_AREA_DATA_CLASS).css("borderTopWidth", 0)
    },
    _applyCustomStyles: function(options) {
        var cell = options.cell;
        var classArray = options.classArray;
        if ("T" === cell.rowType || "T" === cell.columnType) {
            classArray.push(PIVOTGRID_TOTAL_CLASS)
        }
        if ("GT" === cell.rowType || "GT" === cell.columnType) {
            classArray.push(PIVOTGRID_GRAND_TOTAL_CLASS)
        }
        if ("T" === cell.rowType || "GT" === cell.rowType) {
            classArray.push(PIVOTGRID_ROW_TOTAL_CLASS)
        }
        if (options.rowIndex === options.rowsCount - 1) {
            options.cssArray.push("border-bottom: 0px")
        }
        this.callBase(options)
    },
    _moveFakeTable: function(scrollPos) {
        this._moveFakeTableHorizontally(scrollPos.x);
        this._moveFakeTableTop(scrollPos.y);
        this.callBase()
    },
    renderScrollable: function() {
        this._groupElement.dxScrollable({
            useNative: this.getUseNativeValue(),
            useSimulatedScrollbar: false,
            rtlEnabled: this.component.option("rtlEnabled"),
            bounceEnabled: false,
            updateManually: true
        })
    },
    getUseNativeValue: function() {
        var _this$component$optio = this.component.option("scrolling"),
            useNative = _this$component$optio.useNative;
        return "auto" === useNative ? !!_support.nativeScrolling : !!useNative
    },
    getScrollbarWidth: function() {
        return this.getUseNativeValue() ? (0, _calculate_scrollbar_width.calculateScrollbarWidth)() : 0
    },
    updateScrollableOptions: function(_ref) {
        var direction = _ref.direction,
            rtlEnabled = _ref.rtlEnabled;
        var scrollable = this._getScrollable();
        scrollable.option("useNative", this.getUseNativeValue());
        scrollable.option({
            direction: direction,
            rtlEnabled: rtlEnabled
        })
    },
    getScrollableDirection: function(horizontal, vertical) {
        if (horizontal && !vertical) {
            return "horizontal"
        } else if (!horizontal && vertical) {
            return "vertical"
        }
        return "both"
    },
    reset: function() {
        this.callBase();
        if (this._virtualContent) {
            this._virtualContent.parent().css("height", "auto")
        }
    },
    setVirtualContentParams: function(params) {
        this.callBase(params);
        this._virtualContent.parent().css("height", params.height);
        this._setTableCss({
            top: params.top,
            left: params.left
        })
    }
});
exports.DataArea = DataArea;
