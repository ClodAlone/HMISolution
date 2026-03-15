/**
 * DevExtreme (esm/viz/core/data_source.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    noop
} from "../../core/utils/common";
import DataHelperMixin from "../../data_helper";
var postCtor = DataHelperMixin.postCtor;
var name;
var members = {
    _dataSourceLoadErrorHandler: function() {
        this._dataSourceChangedHandler()
    },
    _dataSourceOptions: function() {
        return {
            paginate: false
        }
    },
    _updateDataSource: function() {
        this._refreshDataSource();
        if (!this.option("dataSource")) {
            this._dataSourceChangedHandler()
        }
    },
    _dataIsLoaded: function() {
        return !this._dataSource || this._dataSource.isLoaded()
    },
    _dataSourceItems: function() {
        return this._dataSource && this._dataSource.items()
    }
};
for (name in DataHelperMixin) {
    if ("postCtor" === name) {
        continue
    }
    members[name] = DataHelperMixin[name]
}
export var plugin = {
    name: "data_source",
    init: function() {
        postCtor.call(this)
    },
    dispose: noop,
    members: members
};
