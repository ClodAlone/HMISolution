/**
 * DevExtreme (esm/ui/gantt/ui.gantt.templates.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    getPublicElement
} from "../../core/element";
export class GanttTemplatesManager {
    constructor(gantt) {
        this._gantt = gantt
    }
    getTaskTooltipContentTemplateFunc(taskTooltipContentTemplateOption) {
        var template = taskTooltipContentTemplateOption && this._gantt._getTemplate(taskTooltipContentTemplateOption);
        var createTemplateFunction = template && ((container, item, callback) => {
            template.render({
                model: this._gantt.getTaskDataByCoreData(item),
                container: getPublicElement($(container)),
                onRendered: () => {
                    callback()
                }
            });
            return true
        });
        return createTemplateFunction
    }
    getTaskProgressTooltipContentTemplateFunc(taskTooltipContentTemplateOption) {
        var template = taskTooltipContentTemplateOption && this._gantt._getTemplate(taskTooltipContentTemplateOption);
        var createTemplateFunction = template && ((container, item, callback) => {
            template.render({
                model: item,
                container: getPublicElement($(container)),
                onRendered: () => {
                    callback()
                }
            });
            return true
        });
        return createTemplateFunction
    }
    getTaskTimeTooltipContentTemplateFunc(taskTooltipContentTemplateOption) {
        var template = taskTooltipContentTemplateOption && this._gantt._getTemplate(taskTooltipContentTemplateOption);
        var createTemplateFunction = template && ((container, item, callback) => {
            template.render({
                model: item,
                container: getPublicElement($(container)),
                onRendered: () => {
                    callback()
                }
            });
            return true
        });
        return createTemplateFunction
    }
    getTaskContentTemplateFunc(taskContentTemplateOption) {
        var template = taskContentTemplateOption && this._gantt._getTemplate(taskContentTemplateOption);
        var createTemplateFunction = template && ((container, item, callback, index) => {
            item.taskData = this._gantt.getTaskDataByCoreData(item.taskData);
            template.render({
                model: item,
                container: getPublicElement($(container)),
                onRendered: () => {
                    callback(container, index)
                }
            });
            return true
        });
        return createTemplateFunction
    }
}
