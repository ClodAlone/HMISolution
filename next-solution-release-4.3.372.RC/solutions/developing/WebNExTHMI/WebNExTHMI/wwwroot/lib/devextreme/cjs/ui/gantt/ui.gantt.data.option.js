/**
 * DevExtreme (cjs/ui/gantt/ui.gantt.data.option.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _component = require("../../core/component");
var _data_helper = _interopRequireDefault(require("../../data_helper"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var DataOption = function(_Component) {
    _inheritsLoose(DataOption, _Component);

    function DataOption(optionName, getLoadPanel, dataSourceChangedCallback) {
        var _this;
        _this = _Component.call(this) || this;
        _this._optionName = optionName;
        _this._getLoadPanel = getLoadPanel;
        _this._dataSourceChangedCallback = dataSourceChangedCallback;
        return _this
    }
    var _proto = DataOption.prototype;
    _proto.insert = function(data, callback, errorCallback) {
        var _this2 = this;
        this._showLoadPanel();
        this._getStore().insert(data).done((function(response) {
            if (callback) {
                callback(response)
            }
            _this2._hideLoadPanel()
        })).fail((function(error) {
            if (errorCallback) {
                errorCallback(error)
            }
            _this2._hideLoadPanel()
        }))
    };
    _proto.update = function(key, data, callback, errorCallback) {
        var _this3 = this;
        this._showLoadPanel();
        this._getStore().update(key, data).done((function(data, key) {
            if (callback) {
                callback(data, key)
            }
            _this3._hideLoadPanel()
        })).fail((function(error) {
            if (errorCallback) {
                errorCallback(error)
            }
            _this3._hideLoadPanel()
        }))
    };
    _proto.remove = function(key, callback, errorCallback) {
        var _this4 = this;
        this._showLoadPanel();
        this._getStore().remove(key).done((function(key) {
            if (callback) {
                callback(key)
            }
            _this4._hideLoadPanel()
        })).fail((function(error) {
            if (errorCallback) {
                errorCallback(error)
            }
            _this4._hideLoadPanel()
        }))
    };
    _proto._dataSourceChangedHandler = function(newItems, e) {
        this._dataSourceChangedCallback(this._optionName, newItems)
    };
    _proto._dataSourceOptions = function() {
        return {
            paginate: false
        }
    };
    _proto._dataSourceLoadingChangedHandler = function(isLoading) {
        if (isLoading && !this._dataSource.isLoaded()) {
            this._showLoadPanel()
        } else {
            this._hideLoadPanel()
        }
    };
    _proto._showLoadPanel = function() {
        var _this$_getLoadPanel;
        null === (_this$_getLoadPanel = this._getLoadPanel()) || void 0 === _this$_getLoadPanel ? void 0 : _this$_getLoadPanel.show()
    };
    _proto._hideLoadPanel = function() {
        var _this$_getLoadPanel2;
        null === (_this$_getLoadPanel2 = this._getLoadPanel()) || void 0 === _this$_getLoadPanel2 ? void 0 : _this$_getLoadPanel2.hide()
    };
    _proto._getStore = function() {
        return this._dataSource.store()
    };
    _proto._getItems = function() {
        return this._getStore()._array || this._dataSource.items()
    };
    _proto._reloadDataSource = function() {
        return this._dataSource.load()
    };
    _proto.dispose = function() {
        this._disposeDataSource()
    };
    _proto._optionChanged = function(args) {
        args.name
    };
    return DataOption
}(_component.Component);
DataOption.include(_data_helper.default);
var _default = DataOption;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
