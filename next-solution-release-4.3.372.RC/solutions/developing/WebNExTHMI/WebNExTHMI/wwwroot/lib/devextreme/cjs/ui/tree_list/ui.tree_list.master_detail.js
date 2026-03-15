/**
 * DevExtreme (cjs/ui/tree_list/ui.tree_list.master_detail.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
var _uiTree_list = _interopRequireDefault(require("./ui.tree_list.core"));
var _uiGrid_core = require("../grid_core/ui.grid_core.master_detail");
var _extend = require("../../core/utils/extend");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
_uiTree_list.default.registerModule("masterDetail", (0, _extend.extend)(true, {}, _uiGrid_core.masterDetailModule, {
    extenders: {
        controllers: {
            data: {
                isRowExpanded: function() {
                    return this.callBase.apply(this, arguments)
                },
                _processItems: function() {
                    return this.callBase.apply(this, arguments)
                },
                _processDataItem: function() {
                    return this.callBase.apply(this, arguments)
                }
            }
        }
    }
}));
