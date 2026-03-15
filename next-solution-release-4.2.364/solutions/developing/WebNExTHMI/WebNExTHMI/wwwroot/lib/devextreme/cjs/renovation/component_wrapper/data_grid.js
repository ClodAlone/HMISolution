/**
 * DevExtreme (cjs/renovation/component_wrapper/data_grid.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _type = require("../../core/utils/type");
var _data = require("../../core/utils/data");
var _component = _interopRequireDefault(require("./common/component"));
var _uiData_grid = _interopRequireDefault(require("../../ui/data_grid/ui.data_grid.core"));
var _update_props_immutable = require("./utils/update_props_immutable");
var _get_updated_options = require("../ui/common/utils/get_updated_options");
var _themes_callback = require("../../ui/themes_callback");
var _component_registrator_callbacks = _interopRequireDefault(require("../../core/component_registrator_callbacks"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
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
var dataGridClass = null;
_component_registrator_callbacks.default.add((function(name, componentClass) {
    if ("dxDataGrid" === name) {
        dataGridClass = componentClass
    }
}));
var DataGridWrapper = function(_Component) {
    _inheritsLoose(DataGridWrapper, _Component);

    function DataGridWrapper(element, options) {
        var _dataGridClass;
        var _this;
        _this = _Component.call(this, element, (null !== (_dataGridClass = dataGridClass) && void 0 !== _dataGridClass && _dataGridClass.defaultOptions({}), options)) || this;
        _this._skipInvalidate = false;
        return _this
    }
    var _proto = DataGridWrapper.prototype;
    _proto.state = function(_state) {
        var internalInstance = this._getInternalInstance();
        if (internalInstance) {
            if (void 0 === _state) {
                return internalInstance.state()
            }
            internalInstance.state(_state)
        }
        return
    };
    _proto.getController = function(name) {
        var _this$_getInternalIns;
        return null === (_this$_getInternalIns = this._getInternalInstance()) || void 0 === _this$_getInternalIns ? void 0 : _this$_getInternalIns.getController(name)
    };
    _proto.getView = function(name) {
        var _this$_getInternalIns2;
        return null === (_this$_getInternalIns2 = this._getInternalInstance()) || void 0 === _this$_getInternalIns2 ? void 0 : _this$_getInternalIns2.getView(name)
    };
    _proto.beginUpdate = function() {
        var _this$_getInternalIns3;
        _Component.prototype.beginUpdate.call(this);
        null === (_this$_getInternalIns3 = this._getInternalInstance()) || void 0 === _this$_getInternalIns3 ? void 0 : _this$_getInternalIns3.beginUpdate()
    };
    _proto.endUpdate = function() {
        var _this$_getInternalIns4;
        _Component.prototype.endUpdate.call(this);
        null === (_this$_getInternalIns4 = this._getInternalInstance()) || void 0 === _this$_getInternalIns4 ? void 0 : _this$_getInternalIns4.endUpdate()
    };
    _proto.isReady = function() {
        var _this$_getInternalIns5;
        return null === (_this$_getInternalIns5 = this._getInternalInstance()) || void 0 === _this$_getInternalIns5 ? void 0 : _this$_getInternalIns5.isReady()
    };
    _proto._getInternalInstance = function() {
        var _this$viewRef;
        return null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.getComponentInstance()
    };
    _proto._fireContentReady = function() {};
    _proto._wrapKeyDownHandler = function(handler) {
        return handler
    };
    _proto._optionChanging = function(fullName, prevValue, value) {
        var _this2 = this;
        _Component.prototype._optionChanging.call(this, fullName, prevValue, value);
        if (this.viewRef && prevValue !== value) {
            var name = (0, _data.getPathParts)(fullName)[0];
            var prevProps = _extends({}, this.viewRef.prevProps);
            if ("integrationOptions" === name) {
                return
            }
            if ("editing" === name && name !== fullName) {
                (0, _update_props_immutable.updatePropsImmutable)(prevProps, this.option(), name, name)
            }
            if ((0, _type.isPlainObject)(prevValue) && (0, _type.isPlainObject)(value)) {
                var updatedOptions = (0, _get_updated_options.getUpdatedOptions)(prevValue, value);
                updatedOptions.forEach((function(item) {
                    (0, _update_props_immutable.updatePropsImmutable)(prevProps, _this2.option(), name, "".concat(fullName, ".").concat(item.path))
                }))
            } else {
                (0, _update_props_immutable.updatePropsImmutable)(prevProps, this.option(), name, fullName)
            }
            this.viewRef.prevProps = prevProps
        }
    };
    _proto._optionChanged = function(e) {
        var internalInstance = this._getInternalInstance();
        ["dataSource", "editing.changes"].forEach((function(fullName) {
            if (internalInstance && e.fullName === fullName && e.value === internalInstance.option(fullName)) {
                internalInstance.option(fullName, e.value)
            }
        }));
        _Component.prototype._optionChanged.call(this, e)
    };
    _proto._createTemplateComponent = function(templateOption) {
        return templateOption
    };
    _proto._initializeComponent = function() {
        var options = this.option();
        this._onInitialized = options.onInitialized;
        options.onInitialized = null;
        _Component.prototype._initializeComponent.call(this)
    };
    _proto._patchOptionValues = function(options) {
        var _this3 = this;
        options.onInitialized = this._onInitialized;
        var exportOptions = options.export;
        var originalCustomizeExcelCell = null === exportOptions || void 0 === exportOptions ? void 0 : exportOptions.customizeExcelCell;
        if (originalCustomizeExcelCell) {
            exportOptions.customizeExcelCell = function(e) {
                e.component = _this3;
                return originalCustomizeExcelCell(e)
            }
        }
        var onInitialized = options.onInitialized;
        if (onInitialized) {
            options.onInitialized = function(e) {
                e.component = _this3;
                onInitialized(e)
            }
        }
        return _Component.prototype._patchOptionValues.call(this, options)
    };
    _proto._renderWrapper = function(props) {
        var isFirstRender = !this._isNodeReplaced;
        _Component.prototype._renderWrapper.call(this, props);
        if (isFirstRender) {
            var _this$_getInternalIns6;
            null === (_this$_getInternalIns6 = this._getInternalInstance()) || void 0 === _this$_getInternalIns6 ? void 0 : _this$_getInternalIns6.on("optionChanged", this._internalOptionChangedHandler.bind(this))
        }
    };
    _proto._internalOptionChangedHandler = function(e) {
        var isSecondLevelOption = e.name !== e.fullName;
        if (isSecondLevelOption && e.value !== e.previousValue) {
            if (e.fullName.startsWith("columns[")) {
                if (this.option(e.fullName) !== e.value) {
                    this._cancelOptionChange = e.fullName;
                    this._notifyOptionChanged(e.fullName, e.value, e.previousValue);
                    this._cancelOptionChange = void 0
                }
            } else {
                this._skipInvalidate = true;
                this._options.silent(e.fullName, e.previousValue);
                this.option(e.fullName, e.value);
                this._skipInvalidate = false
            }
        }
    };
    _proto._invalidate = function() {
        if (this._skipInvalidate) {
            return
        }
        _Component.prototype._invalidate.call(this)
    };
    _proto._setOptionsByReference = function() {
        _Component.prototype._setOptionsByReference.call(this);
        this._optionsByReference.focusedRowKey = true;
        this._optionsByReference["editing.editRowKey"] = true;
        this._optionsByReference["editing.changes"] = true
    };
    _proto._setDeprecatedOptions = function() {
        _Component.prototype._setDeprecatedOptions.call(this);
        this._deprecatedOptions.useKeyboard = {
            since: "19.2",
            alias: "keyboardNavigation.enabled"
        };
        this._deprecatedOptions.rowTemplate = {
            since: "21.2",
            message: 'Use the "dataRowTemplate" option instead'
        };
        this._deprecatedOptions.onToolbarPreparing = {
            since: "21.2",
            message: 'Use the "toolbar" option instead'
        }
    };
    _proto._getDefaultOptions = function() {
        var defaultOptions = _Component.prototype._getDefaultOptions.call(this);
        delete defaultOptions.rowTemplate;
        return defaultOptions
    };
    _proto._getAdditionalProps = function() {
        return _Component.prototype._getAdditionalProps.call(this).concat(["onInitialized", "onColumnsChanging", "integrationOptions", "adaptColumnWidthByRatio", "useLegacyKeyboardNavigation", "templatesRenderAsynchronously", "forceApplyBindings", "nestedComponentOptions"])
    };
    return DataGridWrapper
}(_component.default);
exports.default = DataGridWrapper;
DataGridWrapper.registerModule = _uiData_grid.default.registerModule.bind(_uiData_grid.default);
_themes_callback.themeReadyCallback.add();
module.exports = exports.default;
module.exports.default = exports.default;
