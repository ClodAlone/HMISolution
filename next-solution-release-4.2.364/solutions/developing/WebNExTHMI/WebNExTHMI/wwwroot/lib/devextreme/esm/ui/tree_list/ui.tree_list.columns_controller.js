/**
 * DevExtreme (esm/ui/tree_list/ui.tree_list.columns_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined
} from "../../core/utils/type";
import treeListCore from "./ui.tree_list.core";
import {
    columnsControllerModule
} from "../grid_core/ui.grid_core.columns_controller";
export var ColumnsController = columnsControllerModule.controllers.columns.inherit({
    _getFirstItems: function(dataSourceAdapter) {
        return this.callBase(dataSourceAdapter).map((function(node) {
            return node.data
        }))
    },
    getFirstDataColumnIndex: function() {
        var visibleColumns = this.getVisibleColumns();
        var visibleColumnsLength = visibleColumns.length;
        var firstDataColumnIndex = 0;
        for (var i = 0; i <= visibleColumnsLength - 1; i++) {
            if (!isDefined(visibleColumns[i].command)) {
                firstDataColumnIndex = visibleColumns[i].index;
                break
            }
        }
        return firstDataColumnIndex
    }
});
treeListCore.registerModule("columns", {
    defaultOptions: columnsControllerModule.defaultOptions,
    controllers: {
        columns: ColumnsController
    }
});
