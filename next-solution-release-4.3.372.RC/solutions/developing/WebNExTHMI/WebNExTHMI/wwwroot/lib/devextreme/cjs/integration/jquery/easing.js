/**
 * DevExtreme (cjs/integration/jquery/easing.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
var _jquery = _interopRequireDefault(require("jquery"));
var _easing = require("../../animation/easing");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
if (_jquery.default) {
    (0, _easing.setEasing)(_jquery.default.easing)
}
