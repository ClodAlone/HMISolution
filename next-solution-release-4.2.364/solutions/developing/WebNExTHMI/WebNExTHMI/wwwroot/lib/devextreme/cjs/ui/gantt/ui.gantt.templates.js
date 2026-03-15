/**
 * DevExtreme (cjs/ui/gantt/ui.gantt.templates.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.GanttTemplatesManager = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _element = require("../../core/element");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var GanttTemplatesManager = function() {
    function GanttTemplatesManager(gantt) {
        this._gantt = gantt
    }
    var _proto = GanttTemplatesManager.prototype;
    _proto.getTaskTooltipContentTemplateFunc = function(taskTooltipContentTemplateOption) {
        var _this = this;
        var template = taskTooltipContentTemplateOption && this._gantt._getTemplate(taskTooltipContentTemplateOption);
        var createTemplateFunction = template && function(container, item, callback) {
            template.render({
                model: _this._gantt.getTaskDataByCoreData(item),
                container: (0, _element.getPublicElement)((0, _renderer.default)(container)),
                onRendered: function() {
                    callback()
                }
            });
            return true
        };
        return createTemplateFunction
    };
    _proto.getTaskProgressTooltipContentTemplateFunc = function(taskTooltipContentTemplateOption) {
        var template = taskTooltipContentTemplateOption && this._gantt._getTemplate(taskTooltipContentTemplateOption);
        var createTemplateFunction = template && function(container, item, callback) {
            template.render({
                model: item,
                container: (0, _element.getPublicElement)((0, _renderer.default)(container)),
                onRendered: function() {
                    callback()
                }
            });
            return true
        };
        return createTemplateFunction
    };
    _proto.getTaskTimeTooltipContentTemplateFunc = function(taskTooltipContentTemplateOption) {
        var template = taskTooltipContentTemplateOption && this._gantt._getTemplate(taskTooltipContentTemplateOption);
        var createTemplateFunction = template && function(container, item, callback) {
            template.render({
                model: item,
                container: (0, _element.getPublicElement)((0, _renderer.default)(container)),
                onRendered: function() {
                    callback()
                }
            });
            return true
        };
        return createTemplateFunction
    };
    _proto.getTaskContentTemplateFunc = function(taskContentTemplateOption) {
        var _this2 = this;
        var template = taskContentTemplateOption && this._gantt._getTemplate(taskContentTemplateOption);
        var createTemplateFunction = template && function(container, item, callback, index) {
            item.taskData = _this2._gantt.getTaskDataByCoreData(item.taskData);
            template.render({
                model: item,
                container: (0, _element.getPublicElement)((0, _renderer.default)(container)),
                onRendered: function() {
                    callback(container, index)
                }
            });
            return true
        };
        return createTemplateFunction
    };
    return GanttTemplatesManager
}();
exports.GanttTemplatesManager = GanttTemplatesManager;
