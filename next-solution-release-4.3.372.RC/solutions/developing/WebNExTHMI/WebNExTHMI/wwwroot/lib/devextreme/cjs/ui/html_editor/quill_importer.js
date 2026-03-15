/**
 * DevExtreme (cjs/ui/html_editor/quill_importer.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getQuill = getQuill;
var _ui = _interopRequireDefault(require("../widget/ui.errors"));
var _devextremeQuill = _interopRequireDefault(require("devextreme-quill"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function getQuill() {
    if (!_devextremeQuill.default) {
        throw _ui.default.Error("E1041", "Quill")
    }
    return _devextremeQuill.default
}
