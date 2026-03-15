/**
 * DevExtreme (cjs/ui/data_grid/ui.data_grid.data_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.DataController = void 0;
var _uiData_grid = _interopRequireDefault(require("./ui.data_grid.core"));
var _ui = _interopRequireDefault(require("../widget/ui.errors"));
var _uiData_grid2 = _interopRequireDefault(require("./ui.data_grid.data_source_adapter"));
var _uiGrid_core = require("../grid_core/ui.grid_core.data_controller");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var DataController = _uiGrid_core.dataControllerModule.controllers.data.inherit({
    _getDataSourceAdapter: function() {
        return _uiData_grid2.default
    },
    _getSpecificDataSourceOption: function() {
        var dataSource = this.option("dataSource");
        if (dataSource && !Array.isArray(dataSource) && this.option("keyExpr")) {
            _ui.default.log("W1011")
        }
        return this.callBase()
    }
});
exports.DataController = DataController;
_uiData_grid.default.registerModule("data", {
    defaultOptions: _uiGrid_core.dataControllerModule.defaultOptions,
    controllers: {
        data: DataController
    }
});
