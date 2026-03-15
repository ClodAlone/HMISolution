/**
 * DevExtreme (esm/ui/gantt/ui.gantt.data.option.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    Component
} from "../../core/component";
import DataHelperMixin from "../../data_helper";
class DataOption extends Component {
    constructor(optionName, getLoadPanel, dataSourceChangedCallback) {
        super();
        this._optionName = optionName;
        this._getLoadPanel = getLoadPanel;
        this._dataSourceChangedCallback = dataSourceChangedCallback
    }
    insert(data, callback, errorCallback) {
        this._showLoadPanel();
        this._getStore().insert(data).done(response => {
            if (callback) {
                callback(response)
            }
            this._hideLoadPanel()
        }).fail(error => {
            if (errorCallback) {
                errorCallback(error)
            }
            this._hideLoadPanel()
        })
    }
    update(key, data, callback, errorCallback) {
        this._showLoadPanel();
        this._getStore().update(key, data).done((data, key) => {
            if (callback) {
                callback(data, key)
            }
            this._hideLoadPanel()
        }).fail(error => {
            if (errorCallback) {
                errorCallback(error)
            }
            this._hideLoadPanel()
        })
    }
    remove(key, callback, errorCallback) {
        this._showLoadPanel();
        this._getStore().remove(key).done(key => {
            if (callback) {
                callback(key)
            }
            this._hideLoadPanel()
        }).fail(error => {
            if (errorCallback) {
                errorCallback(error)
            }
            this._hideLoadPanel()
        })
    }
    _dataSourceChangedHandler(newItems, e) {
        this._dataSourceChangedCallback(this._optionName, newItems)
    }
    _dataSourceOptions() {
        return {
            paginate: false
        }
    }
    _dataSourceLoadingChangedHandler(isLoading) {
        if (isLoading && !this._dataSource.isLoaded()) {
            this._showLoadPanel()
        } else {
            this._hideLoadPanel()
        }
    }
    _showLoadPanel() {
        var _this$_getLoadPanel;
        null === (_this$_getLoadPanel = this._getLoadPanel()) || void 0 === _this$_getLoadPanel ? void 0 : _this$_getLoadPanel.show()
    }
    _hideLoadPanel() {
        var _this$_getLoadPanel2;
        null === (_this$_getLoadPanel2 = this._getLoadPanel()) || void 0 === _this$_getLoadPanel2 ? void 0 : _this$_getLoadPanel2.hide()
    }
    _getStore() {
        return this._dataSource.store()
    }
    _getItems() {
        return this._getStore()._array || this._dataSource.items()
    }
    _reloadDataSource() {
        return this._dataSource.load()
    }
    dispose() {
        this._disposeDataSource()
    }
    _optionChanged(args) {
        args.name
    }
}
DataOption.include(DataHelperMixin);
export default DataOption;
