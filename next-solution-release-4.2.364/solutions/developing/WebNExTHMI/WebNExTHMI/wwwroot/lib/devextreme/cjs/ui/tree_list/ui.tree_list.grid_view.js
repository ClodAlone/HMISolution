/**
 * DevExtreme (cjs/ui/tree_list/ui.tree_list.grid_view.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
var _uiTree_list = _interopRequireDefault(require("./ui.tree_list.core"));
var _uiGrid_core = require("../grid_core/ui.grid_core.grid_view");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var GridView = _uiGrid_core.gridViewModule.views.gridView.inherit({
    _getWidgetAriaLabel: function() {
        return "dxTreeList-ariaTreeList"
    },
    _getTableRoleName: function() {
        return "treegrid"
    }
});
_uiTree_list.default.registerModule("gridView", {
    defaultOptions: _uiGrid_core.gridViewModule.defaultOptions,
    controllers: _uiGrid_core.gridViewModule.controllers,
    views: {
        gridView: GridView
    },
    extenders: {
        controllers: {
            resizing: {
                _toggleBestFitMode: function(isBestFit) {
                    this.callBase(isBestFit);
                    var $rowsTable = this._rowsView.getTableElement();
                    $rowsTable.find(".dx-treelist-cell-expandable").toggleClass(this.addWidgetPrefix("best-fit"), isBestFit)
                }
            }
        }
    }
});
