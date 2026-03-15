/**
 * DevExtreme (cjs/ui/tree_list/ui.tree_list.validating.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
var _extend = require("../../core/utils/extend");
var _uiTree_list = _interopRequireDefault(require("./ui.tree_list.core"));
var _uiGrid_core = require("../grid_core/ui.grid_core.validating");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var EditingControllerExtender = (0, _extend.extend)({}, _uiGrid_core.validatingModule.extenders.controllers.editing);
delete EditingControllerExtender.processItems;
delete EditingControllerExtender.processDataItem;
_uiTree_list.default.registerModule("validating", {
    defaultOptions: _uiGrid_core.validatingModule.defaultOptions,
    controllers: _uiGrid_core.validatingModule.controllers,
    extenders: {
        controllers: {
            editing: EditingControllerExtender,
            editorFactory: _uiGrid_core.validatingModule.extenders.controllers.editorFactory
        },
        views: _uiGrid_core.validatingModule.extenders.views
    }
});
