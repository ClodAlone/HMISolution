/**
 * DevExtreme (esm/ui/pivot_grid/ui.pivot_grid.data_area.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    AreaItem
} from "./ui.pivot_grid.area_item";
import {
    nativeScrolling
} from "../../core/utils/support";
import {
    calculateScrollbarWidth
} from "./utils/calculate_scrollbar_width";
var PIVOTGRID_AREA_CLASS = "dx-pivotgrid-area";
var PIVOTGRID_AREA_DATA_CLASS = "dx-pivotgrid-area-data";
var PIVOTGRID_TOTAL_CLASS = "dx-total";
var PIVOTGRID_GRAND_TOTAL_CLASS = "dx-grandtotal";
var PIVOTGRID_ROW_TOTAL_CLASS = "dx-row-total";
export var DataArea = AreaItem.inherit({
    _getAreaName: function() {
        return "data"
    },
    _createGroupElement: function() {
        return $("<div>").addClass(PIVOTGRID_AREA_CLASS).addClass(PIVOTGRID_AREA_DATA_CLASS).css("borderTopWidth", 0)
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
        var {
            useNative: useNative
        } = this.component.option("scrolling");
        return "auto" === useNative ? !!nativeScrolling : !!useNative
    },
    getScrollbarWidth: function() {
        return this.getUseNativeValue() ? calculateScrollbarWidth() : 0
    },
    updateScrollableOptions: function(_ref) {
        var {
            direction: direction,
            rtlEnabled: rtlEnabled
        } = _ref;
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
