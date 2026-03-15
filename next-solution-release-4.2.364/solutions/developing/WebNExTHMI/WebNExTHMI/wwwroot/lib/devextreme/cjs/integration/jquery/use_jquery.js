/**
 * DevExtreme (cjs/integration/jquery/use_jquery.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = _default;
var _jquery = _interopRequireDefault(require("jquery"));
var _config = _interopRequireDefault(require("../../core/config"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var useJQuery = (0, _config.default)().useJQuery;
if (_jquery.default && false !== useJQuery) {
    (0, _config.default)({
        useJQuery: true
    })
}

function _default() {
    return _jquery.default && (0, _config.default)().useJQuery
}
module.exports = exports.default;
module.exports.default = exports.default;
