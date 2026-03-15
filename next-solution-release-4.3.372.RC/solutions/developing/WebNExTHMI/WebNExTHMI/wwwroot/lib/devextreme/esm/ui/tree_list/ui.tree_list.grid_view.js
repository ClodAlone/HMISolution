/**
 * DevExtreme (esm/ui/tree_list/ui.tree_list.grid_view.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import treeListCore from "./ui.tree_list.core";
import {
    gridViewModule
} from "../grid_core/ui.grid_core.grid_view";
var GridView = gridViewModule.views.gridView.inherit({
    _getWidgetAriaLabel: function() {
        return "dxTreeList-ariaTreeList"
    },
    _getTableRoleName: function() {
        return "treegrid"
    }
});
treeListCore.registerModule("gridView", {
    defaultOptions: gridViewModule.defaultOptions,
    controllers: gridViewModule.controllers,
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
