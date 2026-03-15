/**
 * DevExtreme (esm/ui/gantt/ui.gantt.custom_fields.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    compileGetter
} from "../../core/utils/data";
import {
    GanttDataCache
} from "./ui.gantt.cache";
import {
    GanttHelper
} from "./ui.gantt.helper";
var GANTT_TASKS = "tasks";
export class GanttCustomFieldsManager {
    constructor(gantt) {
        this._gantt = gantt;
        this._mappingHelper = gantt._mappingHelper;
        this.cache = new GanttDataCache
    }
    _getTaskCustomFields() {
        var columns = this._gantt.option("columns");
        var columnFields = columns && columns.map(c => c.dataField);
        var mappedFields = this._mappingHelper.getTaskMappedFieldNames();
        return columnFields ? columnFields.filter(f => mappedFields.indexOf(f) < 0) : []
    }
    _getCustomFieldsData(data) {
        return this._getTaskCustomFields().reduce((previous, field) => {
            if (data && void 0 !== data[field]) {
                previous[field] = data[field]
            }
            return previous
        }, {})
    }
    addCustomFieldsData(key, data) {
        if (data) {
            var modelData = this._gantt._tasksOption && this._gantt._tasksOption._getItems();
            var keyGetter = compileGetter(this._gantt.option("".concat(GANTT_TASKS, ".keyExpr")));
            var modelItem = modelData && modelData.filter(obj => keyGetter(obj) === key)[0];
            var customFields = this._getTaskCustomFields();
            if (modelItem) {
                for (var i = 0; i < customFields.length; i++) {
                    var field = customFields[i];
                    if (Object.prototype.hasOwnProperty.call(modelItem, field)) {
                        data[field] = modelItem[field]
                    }
                }
            }
        }
    }
    appendCustomFields(data) {
        var modelData = this._gantt._tasksOption && this._gantt._tasksOption._getItems();
        var keyGetter = this._gantt._getTaskKeyGetter();
        var invertedData = GanttHelper.getInvertedData(modelData, keyGetter);
        return data.reduce((previous, item) => {
            var key = keyGetter(item);
            var modelItem = invertedData[key];
            if (!modelItem) {
                previous.push(item)
            } else {
                var updatedItem = {};
                for (var field in modelItem) {
                    updatedItem[field] = Object.prototype.hasOwnProperty.call(item, field) ? item[field] : modelItem[field]
                }
                previous.push(updatedItem)
            }
            return previous
        }, [])
    }
    addCustomFieldsDataFromCache(key, data) {
        this.cache.pullDataFromCache(key, data)
    }
    saveCustomFieldsDataToCache(key, data) {
        var forceUpdateOnKeyExpire = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : false;
        var isCustomFieldsUpdateOnly = arguments.length > 3 && void 0 !== arguments[3] ? arguments[3] : false;
        var customFieldsData = this._getCustomFieldsData(data);
        if (Object.keys(customFieldsData).length > 0) {
            this.cache.saveData(key, customFieldsData, forceUpdateOnKeyExpire ? (key, data) => {
                var dataOption = this._gantt["_".concat(GANTT_TASKS, "Option")];
                if (dataOption && data) {
                    dataOption.update(key, data, (data, key) => {
                        var updatedCustomFields = {};
                        this.addCustomFieldsData(key, updatedCustomFields);
                        dataOption._reloadDataSource().done(data => {
                            this._gantt._ganttTreeList.updateDataSource(null !== data && void 0 !== data ? data : dataOption._dataSource, false, isCustomFieldsUpdateOnly)
                        });
                        var selectedRowKey = this._gantt.option("selectedRowKey");
                        this._gantt._ganttView._selectTask(selectedRowKey);
                        this._gantt._actionsManager.raiseUpdatedAction(GANTT_TASKS, updatedCustomFields, key)
                    })
                }
            } : null)
        }
    }
    resetCustomFieldsDataCache(key) {
        this.cache.resetCache(key)
    }
}
