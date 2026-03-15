/**
 * DevExtreme (esm/ui/editor/ui.data_expression.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import variableWrapper from "../../core/utils/variable_wrapper";
import {
    compileGetter,
    toComparable
} from "../../core/utils/data";
import {
    ensureDefined,
    noop
} from "../../core/utils/common";
import {
    isDefined,
    isObject as isObjectType,
    isString,
    isFunction
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import DataHelperMixin from "../../data_helper";
import {
    DataSource
} from "../../data/data_source/data_source";
import ArrayStore from "../../data/array_store";
import {
    Deferred
} from "../../core/utils/deferred";
var DataExpressionMixin = extend({}, DataHelperMixin, {
    _dataExpressionDefaultOptions: function() {
        return {
            items: [],
            dataSource: null,
            itemTemplate: "item",
            value: null,
            valueExpr: "this",
            displayExpr: void 0
        }
    },
    _initDataExpressions: function() {
        this._compileValueGetter();
        this._compileDisplayGetter();
        this._initDynamicTemplates();
        this._initDataSource();
        this._itemsToDataSource()
    },
    _itemsToDataSource: function() {
        if (!this.option("dataSource")) {
            this._dataSource = new DataSource({
                store: new ArrayStore(this.option("items")),
                pageSize: 0
            })
        }
    },
    _compileDisplayGetter: function() {
        this._displayGetter = compileGetter(this._displayGetterExpr())
    },
    _displayGetterExpr: function() {
        return this.option("displayExpr")
    },
    _compileValueGetter: function() {
        this._valueGetter = compileGetter(this._valueGetterExpr())
    },
    _valueGetterExpr: function() {
        return this.option("valueExpr") || "this"
    },
    _loadValue: function(value) {
        var deferred = new Deferred;
        value = this._unwrappedValue(value);
        if (!isDefined(value)) {
            return deferred.reject().promise()
        }
        this._loadSingle(this._valueGetterExpr(), value).done(function(item) {
            this._isValueEquals(this._valueGetter(item), value) ? deferred.resolve(item) : deferred.reject()
        }.bind(this)).fail((function() {
            deferred.reject()
        }));
        this._loadValueDeferred = deferred;
        return deferred.promise()
    },
    _rejectValueLoading: function() {
        var _this$_loadValueDefer;
        null === (_this$_loadValueDefer = this._loadValueDeferred) || void 0 === _this$_loadValueDefer ? void 0 : _this$_loadValueDefer.reject({
            shouldSkipCallback: true
        })
    },
    _getCurrentValue: function() {
        return this.option("value")
    },
    _unwrappedValue: function(value) {
        var _value;
        value = null !== (_value = value) && void 0 !== _value ? _value : this._getCurrentValue();
        if (value && this._dataSource && "this" === this._valueGetterExpr()) {
            value = this._getItemKey(value)
        }
        return variableWrapper.unwrap(value)
    },
    _getItemKey: function(value) {
        var key = this._dataSource.key();
        if (Array.isArray(key)) {
            var result = {};
            for (var i = 0, n = key.length; i < n; i++) {
                result[key[i]] = value[key[i]]
            }
            return result
        }
        if (key && "object" === typeof value) {
            value = value[key]
        }
        return value
    },
    _isValueEquals: function(value1, value2) {
        var dataSourceKey = this._dataSource && this._dataSource.key();
        var result = this._compareValues(value1, value2);
        if (!result && dataSourceKey && isDefined(value1) && isDefined(value2)) {
            if (Array.isArray(dataSourceKey)) {
                result = this._compareByCompositeKey(value1, value2, dataSourceKey)
            } else {
                result = this._compareByKey(value1, value2, dataSourceKey)
            }
        }
        return result
    },
    _compareByCompositeKey: function(value1, value2, key) {
        var isObject = isObjectType;
        if (!isObject(value1) || !isObject(value2)) {
            return false
        }
        for (var i = 0, n = key.length; i < n; i++) {
            if (value1[key[i]] !== value2[key[i]]) {
                return false
            }
        }
        return true
    },
    _compareByKey: function(value1, value2, key) {
        var unwrapObservable = variableWrapper.unwrap;
        var valueKey1 = ensureDefined(unwrapObservable(value1[key]), value1);
        var valueKey2 = ensureDefined(unwrapObservable(value2[key]), value2);
        return this._compareValues(valueKey1, valueKey2)
    },
    _compareValues: function(value1, value2) {
        return toComparable(value1, true) === toComparable(value2, true)
    },
    _initDynamicTemplates: noop,
    _setCollectionWidgetItemTemplate: function() {
        this._initDynamicTemplates();
        this._setCollectionWidgetOption("itemTemplate", this.option("itemTemplate"))
    },
    _getCollectionKeyExpr: function() {
        var valueExpr = this.option("valueExpr");
        var isValueExprField = isString(valueExpr) && "this" !== valueExpr || isFunction(valueExpr);
        return isValueExprField ? valueExpr : null
    },
    _dataExpressionOptionChanged: function(args) {
        switch (args.name) {
            case "items":
                this._itemsToDataSource();
                this._setCollectionWidgetOption("items");
                break;
            case "dataSource":
                this._initDataSource();
                break;
            case "itemTemplate":
                this._setCollectionWidgetItemTemplate();
                break;
            case "valueExpr":
                this._compileValueGetter();
                break;
            case "displayExpr":
                this._compileDisplayGetter();
                this._initDynamicTemplates();
                this._setCollectionWidgetOption("displayExpr")
        }
    }
});
export default DataExpressionMixin;
