/**
 * DevExtreme (cjs/ui/scheduler/loading.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.hide = hide;
exports.show = show;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _view_port = require("../../core/utils/view_port");
var _load_panel = _interopRequireDefault(require("../load_panel"));
var _deferred = require("../../core/utils/deferred");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var loading = null;
var createLoadPanel = function(options) {
    return new _load_panel.default((0, _renderer.default)("<div>").appendTo(options && options.container || (0, _view_port.value)()), options)
};
var removeLoadPanel = function() {
    if (!loading) {
        return
    }
    loading.$element().remove();
    loading = null
};

function show(options) {
    removeLoadPanel();
    loading = createLoadPanel(options);
    return loading.show()
}

function hide() {
    if (!loading) {
        return (new _deferred.Deferred).resolve()
    }
    return loading.hide().done(removeLoadPanel).promise()
}
