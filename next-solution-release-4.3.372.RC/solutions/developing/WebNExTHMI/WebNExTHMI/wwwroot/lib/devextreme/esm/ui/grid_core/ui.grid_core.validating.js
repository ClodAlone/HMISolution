/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.validating.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getOuterWidth,
    getWidth,
    getOuterHeight,
    setHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import modules from "./ui.grid_core.modules";
import gridCoreUtils from "./ui.grid_core.utils";
import {
    createObjectWithChanges
} from "../../data/array_utils";
import {
    deferUpdate,
    equalByValue,
    getKeyHash
} from "../../core/utils/common";
import {
    each
} from "../../core/utils/iterator";
import {
    isDefined,
    isEmptyObject,
    isObject
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import {
    focused
} from "../widget/selectors";
import messageLocalization from "../../localization/message";
import Button from "../button";
import pointerEvents from "../../events/pointer";
import ValidationEngine from "../validation_engine";
import Validator from "../validator";
import Overlay from "../overlay/ui.overlay";
import errors from "../widget/ui.errors";
import {
    Deferred,
    when,
    fromPromise
} from "../../core/utils/deferred";
import LoadIndicator from "../load_indicator";
import {
    encodeHtml
} from "../../core/utils/string";
import browser from "../../core/utils/browser";
var INVALIDATE_CLASS = "invalid";
var REVERT_TOOLTIP_CLASS = "revert-tooltip";
var ROWS_VIEW_CLASS = "rowsview";
var INVALID_MESSAGE_CLASS = "dx-invalid-message";
var WIDGET_INVALID_MESSAGE_CLASS = "invalid-message";
var INVALID_MESSAGE_ALWAYS_CLASS = "dx-invalid-message-always";
var REVERT_BUTTON_CLASS = "dx-revert-button";
var VALIDATOR_CLASS = "validator";
var PENDING_INDICATOR_CLASS = "dx-pending-indicator";
var VALIDATION_PENDING_CLASS = "dx-validation-pending";
var CONTENT_CLASS = "content";
var INSERT_INDEX = "__DX_INSERT_INDEX__";
var PADDING_BETWEEN_TOOLTIPS = 2;
var EDIT_MODE_ROW = "row";
var EDIT_MODE_FORM = "form";
var EDIT_MODE_BATCH = "batch";
var EDIT_MODE_CELL = "cell";
var EDIT_MODE_POPUP = "popup";
var GROUP_CELL_CLASS = "dx-group-cell";
var FORM_BASED_MODES = [EDIT_MODE_POPUP, EDIT_MODE_FORM];
var COMMAND_TRANSPARENT = "transparent";
var VALIDATION_STATUS = {
    valid: "valid",
    invalid: "invalid",
    pending: "pending"
};
var EDIT_DATA_INSERT_TYPE = "insert";
var EDIT_DATA_REMOVE_TYPE = "remove";
var VALIDATION_CANCELLED = "cancel";
var validationResultIsValid = function(result) {
    return isDefined(result) && result !== VALIDATION_CANCELLED
};
var cellValueShouldBeValidated = function(value, rowOptions) {
    return void 0 !== value || void 0 === value && rowOptions && !rowOptions.isNewRow
};
var ValidatingController = modules.Controller.inherit({
    init: function() {
        this._editingController = this.getController("editing");
        this.createAction("onRowValidating");
        if (!this._validationState) {
            this.initValidationState()
        }
    },
    initValidationState() {
        this._validationState = [];
        this._validationStateCache = {}
    },
    _rowIsValidated: function(change) {
        var validationData = this._getValidationData(null === change || void 0 === change ? void 0 : change.key);
        return !!validationData && !!validationData.validated
    },
    _getValidationData: function(key, create) {
        var keyHash = getKeyHash(key);
        var isObjectKeyHash = isObject(keyHash);
        var validationData;
        if (isObjectKeyHash) {
            validationData = this._validationState.filter(data => equalByValue(data.key, key))[0]
        } else {
            validationData = this._validationStateCache[keyHash]
        }
        if (!validationData && create) {
            validationData = {
                key: key,
                isValid: true
            };
            this._validationState.push(validationData);
            if (!isObjectKeyHash) {
                this._validationStateCache[keyHash] = validationData
            }
        }
        return validationData
    },
    _getBrokenRules: function(validationData, validationResults) {
        var brokenRules;
        if (validationResults) {
            brokenRules = validationResults.brokenRules || validationResults.brokenRule && [validationResults.brokenRule]
        } else {
            brokenRules = validationData.brokenRules || []
        }
        return brokenRules
    },
    _rowValidating: function(validationData, validationResults) {
        var deferred = new Deferred;
        var change = this._editingController.getChangeByKey(null === validationData || void 0 === validationData ? void 0 : validationData.key);
        var brokenRules = this._getBrokenRules(validationData, validationResults);
        var isValid = validationResults ? validationResults.isValid : validationData.isValid;
        var parameters = {
            brokenRules: brokenRules,
            isValid: isValid,
            key: change.key,
            newData: change.data,
            oldData: this._editingController._getOldData(change.key),
            promise: null,
            errorText: this.getHiddenValidatorsErrorText(brokenRules)
        };
        this.executeAction("onRowValidating", parameters);
        when(fromPromise(parameters.promise)).always((function() {
            validationData.isValid = parameters.isValid;
            validationData.errorText = parameters.errorText;
            deferred.resolve(parameters)
        }));
        return deferred.promise()
    },
    getHiddenValidatorsErrorText: function(brokenRules) {
        var brokenRulesMessages = [];
        each(brokenRules, (function(_, brokenRule) {
            var column = brokenRule.column;
            var isGroupExpandColumn = column && void 0 !== column.groupIndex && !column.showWhenGrouped;
            var isVisibleColumn = column && column.visible;
            if (!brokenRule.validator.$element().parent().length && (!isVisibleColumn || isGroupExpandColumn)) {
                brokenRulesMessages.push(brokenRule.message)
            }
        }));
        return brokenRulesMessages.join(", ")
    },
    validate: function(isFull) {
        var isValid = true;
        var editingController = this._editingController;
        var deferred = new Deferred;
        var completeList = [];
        var editMode = editingController.getEditMode();
        isFull = isFull || editMode === EDIT_MODE_ROW;
        if (this._isValidationInProgress) {
            return deferred.resolve(false).promise()
        }
        this._isValidationInProgress = true;
        if (isFull) {
            editingController.addDeferred(deferred);
            var changes = editingController.getChanges();
            each(changes, (index, _ref) => {
                var {
                    type: type,
                    key: key
                } = _ref;
                if ("remove" !== type) {
                    var validationData = this._getValidationData(key, true);
                    var validationResult = this.validateGroup(validationData);
                    completeList.push(validationResult);
                    validationResult.done(validationResult => {
                        validationData.validated = true;
                        isValid = isValid && validationResult.isValid
                    })
                }
            })
        } else if (this._currentCellValidator) {
            var validationResult = this.validateGroup(this._currentCellValidator._findGroup());
            completeList.push(validationResult);
            validationResult.done(validationResult => {
                isValid = validationResult.isValid
            })
        }
        when(...completeList).done(() => {
            this._isValidationInProgress = false;
            deferred.resolve(isValid)
        });
        return deferred.promise()
    },
    validateGroup: function(validationData) {
        var _validationResult;
        var result = new Deferred;
        var validateGroup = validationData && ValidationEngine.getGroupConfig(validationData);
        var validationResult;
        if (null !== validateGroup && void 0 !== validateGroup && validateGroup.validators.length) {
            this.resetRowValidationResults(validationData);
            validationResult = ValidationEngine.validateGroup(validationData)
        }
        when((null === (_validationResult = validationResult) || void 0 === _validationResult ? void 0 : _validationResult.complete) || validationResult).done(validationResult => {
            when(this._rowValidating(validationData, validationResult)).done(result.resolve)
        });
        return result.promise()
    },
    isRowDataModified: change => !isEmptyObject(change.data),
    updateValidationState: function(change) {
        var editMode = this._editingController.getEditMode();
        var key = change.key;
        var validationData = this._getValidationData(key, true);
        if (-1 === FORM_BASED_MODES.indexOf(editMode)) {
            if (change.type === EDIT_DATA_INSERT_TYPE && !this.isRowDataModified(change)) {
                validationData.isValid = true;
                return
            }
            this.setDisableApplyValidationResults(true);
            var groupConfig = ValidationEngine.getGroupConfig(validationData);
            if (groupConfig) {
                var validationResult = ValidationEngine.validateGroup(validationData);
                when(validationResult.complete || validationResult).done(validationResult => {
                    validationData.isValid = validationResult.isValid;
                    validationData.brokenRules = validationResult.brokenRules
                })
            } else if (!validationData.brokenRules || !validationData.brokenRules.length) {
                validationData.isValid = true
            }
            this.setDisableApplyValidationResults(false)
        } else {
            validationData.isValid = true
        }
    },
    setValidator: function(validator) {
        this._currentCellValidator = validator
    },
    renderCellPendingIndicator: function($container) {
        var $indicator = $container.find("." + PENDING_INDICATOR_CLASS);
        if (!$indicator.length) {
            var $indicatorContainer = $container;
            $indicator = $("<div>").appendTo($indicatorContainer).addClass(PENDING_INDICATOR_CLASS);
            this._createComponent($indicator, LoadIndicator);
            $container.addClass(VALIDATION_PENDING_CLASS)
        }
    },
    disposeCellPendingIndicator: function($container) {
        var $indicator = $container.find("." + PENDING_INDICATOR_CLASS);
        if ($indicator.length) {
            var indicator = LoadIndicator.getInstance($indicator);
            if (indicator) {
                indicator.dispose();
                indicator.$element().remove()
            }
            $container.removeClass(VALIDATION_PENDING_CLASS)
        }
    },
    validationStatusChanged: function(result) {
        var validator = result.validator;
        var validationGroup = validator.option("validationGroup");
        var column = validator.option("dataGetter")().column;
        this.updateCellValidationResult({
            rowKey: validationGroup.key,
            columnIndex: column.index,
            validationResult: result
        })
    },
    validatorInitialized: function(arg) {
        arg.component.on("validating", this.validationStatusChanged.bind(this));
        arg.component.on("validated", this.validationStatusChanged.bind(this))
    },
    validatorDisposing: function(arg) {
        var validator = arg.component;
        var validationGroup = validator.option("validationGroup");
        var column = validator.option("dataGetter")().column;
        var result = this.getCellValidationResult({
            rowKey: null === validationGroup || void 0 === validationGroup ? void 0 : validationGroup.key,
            columnIndex: column.index
        });
        if (validationResultIsValid(result) && result.status === VALIDATION_STATUS.pending) {
            this.cancelCellValidationResult({
                change: validationGroup,
                columnIndex: column.index
            })
        }
    },
    applyValidationResult: function($container, result) {
        var validator = result.validator;
        var validationGroup = validator.option("validationGroup");
        var column = validator.option("dataGetter")().column;
        result.brokenRules && result.brokenRules.forEach(rule => {
            rule.columnIndex = column.index;
            rule.column = column
        });
        if ($container) {
            var validationResult = this.getCellValidationResult({
                rowKey: validationGroup.key,
                columnIndex: column.index
            });
            var requestIsDisabled = validationResultIsValid(validationResult) && validationResult.disabledPendingId === result.id;
            if (this._disableApplyValidationResults || requestIsDisabled) {
                return
            }
            if (result.status === VALIDATION_STATUS.invalid) {
                var $focus = $container.find(":focus");
                if (!focused($focus)) {
                    eventsEngine.trigger($focus, "focus");
                    eventsEngine.trigger($focus, pointerEvents.down)
                }
            }
            var editor = !column.editCellTemplate && this.getController("editorFactory").getEditorInstance($container);
            if (result.status === VALIDATION_STATUS.pending) {
                if (editor) {
                    editor.option("validationStatus", VALIDATION_STATUS.pending)
                } else {
                    this.renderCellPendingIndicator($container)
                }
            } else if (editor) {
                editor.option("validationStatus", VALIDATION_STATUS.valid)
            } else {
                this.disposeCellPendingIndicator($container)
            }
            $container.toggleClass(this.addWidgetPrefix(INVALIDATE_CLASS), result.status === VALIDATION_STATUS.invalid)
        }
    },
    _syncInternalEditingData: function(parameters) {
        var _parameters$row;
        var editingController = this._editingController;
        var change = editingController.getChangeByKey(parameters.key);
        var oldDataFromState = editingController._getOldData(parameters.key);
        var oldData = null === (_parameters$row = parameters.row) || void 0 === _parameters$row ? void 0 : _parameters$row.oldData;
        if (change && oldData && !oldDataFromState) {
            editingController._addInternalData({
                key: parameters.key,
                oldData: oldData
            })
        }
    },
    createValidator: function(parameters, $container) {
        var editingController = this._editingController;
        var column = parameters.column;
        var showEditorAlways = column.showEditorAlways;
        if (isDefined(column.command) || !column.validationRules || !Array.isArray(column.validationRules) || !column.validationRules.length) {
            return
        }
        var editIndex = editingController.getIndexByKey(parameters.key, editingController.getChanges());
        var needCreateValidator = editIndex > -1;
        if (!needCreateValidator) {
            if (!showEditorAlways) {
                var columnsController = this.getController("columns");
                var visibleColumns = (null === columnsController || void 0 === columnsController ? void 0 : columnsController.getVisibleColumns()) || [];
                showEditorAlways = visibleColumns.some((function(column) {
                    return column.showEditorAlways
                }))
            }
            var isEditRow = equalByValue(this.option("editing.editRowKey"), parameters.key);
            var isCellOrBatchEditingAllowed = editingController.isCellOrBatchEditMode() && editingController.allowUpdating({
                row: parameters.row
            });
            needCreateValidator = isEditRow || isCellOrBatchEditingAllowed && showEditorAlways;
            if (isCellOrBatchEditingAllowed && showEditorAlways) {
                var _parameters$row$oldDa, _parameters$row2;
                editingController._addInternalData({
                    key: parameters.key,
                    oldData: null !== (_parameters$row$oldDa = null === (_parameters$row2 = parameters.row) || void 0 === _parameters$row2 ? void 0 : _parameters$row2.oldData) && void 0 !== _parameters$row$oldDa ? _parameters$row$oldDa : parameters.data
                })
            }
        }
        if (needCreateValidator) {
            if ($container && !$container.length) {
                errors.log("E1050");
                return
            }
            this._syncInternalEditingData(parameters);
            var validationData = this._getValidationData(parameters.key, true);
            var getValue = () => {
                var change = editingController.getChangeByKey(null === validationData || void 0 === validationData ? void 0 : validationData.key);
                var value = column.calculateCellValue((null === change || void 0 === change ? void 0 : change.data) || {});
                return void 0 !== value ? value : parameters.value
            };
            var useDefaultValidator = $container && $container.hasClass("dx-widget");
            $container && $container.addClass(this.addWidgetPrefix(VALIDATOR_CLASS));
            var validator = new Validator($container || $("<div>"), {
                name: column.caption,
                validationRules: extend(true, [], column.validationRules),
                validationGroup: validationData,
                adapter: useDefaultValidator ? null : {
                    getValue: getValue,
                    applyValidationResults: result => {
                        this.applyValidationResult($container, result)
                    }
                },
                dataGetter: function() {
                    var key = null === validationData || void 0 === validationData ? void 0 : validationData.key;
                    var change = editingController.getChangeByKey(key);
                    var oldData = editingController._getOldData(key);
                    return {
                        data: createObjectWithChanges(oldData, null === change || void 0 === change ? void 0 : change.data),
                        column: column
                    }
                },
                onInitialized: this.validatorInitialized.bind(this),
                onDisposing: this.validatorDisposing.bind(this)
            });
            if (useDefaultValidator) {
                var adapter = validator.option("adapter");
                if (adapter) {
                    adapter.getValue = getValue;
                    adapter.validationRequestsCallbacks = []
                }
            }
            return validator
        }
    },
    setDisableApplyValidationResults: function(flag) {
        this._disableApplyValidationResults = flag
    },
    getDisableApplyValidationResults: function() {
        return this._disableApplyValidationResults
    },
    isCurrentValidatorProcessing: function(_ref2) {
        var {
            rowKey: rowKey,
            columnIndex: columnIndex
        } = _ref2;
        return this._currentCellValidator && equalByValue(this._currentCellValidator.option("validationGroup").key, rowKey) && this._currentCellValidator.option("dataGetter")().column.index === columnIndex
    },
    validateCell: function(validator) {
        var cellParams = {
            rowKey: validator.option("validationGroup").key,
            columnIndex: validator.option("dataGetter")().column.index
        };
        var validationResult = this.getCellValidationResult(cellParams);
        var stateRestored = validationResultIsValid(validationResult);
        if (!stateRestored) {
            validationResult = validator.validate()
        }
        var deferred = new Deferred;
        var adapter = validator.option("adapter");
        if (stateRestored && validationResult.status === VALIDATION_STATUS.pending) {
            this.updateCellValidationResult(cellParams);
            adapter.applyValidationResults(validationResult)
        }
        when(validationResult.complete || validationResult).done(validationResult => {
            stateRestored && adapter.applyValidationResults(validationResult);
            deferred.resolve(validationResult)
        });
        return deferred.promise()
    },
    updateCellValidationResult: function(_ref3) {
        var {
            rowKey: rowKey,
            columnIndex: columnIndex,
            validationResult: validationResult
        } = _ref3;
        var validationData = this._getValidationData(rowKey);
        if (!validationData) {
            return
        }
        if (!validationData.validationResults) {
            validationData.validationResults = {}
        }
        var result;
        if (validationResult) {
            result = extend({}, validationResult);
            validationData.validationResults[columnIndex] = result;
            if (validationResult.status === VALIDATION_STATUS.pending) {
                if (this._editingController.getEditMode() === EDIT_MODE_CELL) {
                    result.deferred = new Deferred;
                    result.complete.always(() => {
                        result.deferred.resolve()
                    });
                    this._editingController.addDeferred(result.deferred)
                }
                if (this._disableApplyValidationResults) {
                    result.disabledPendingId = validationResult.id;
                    return
                }
            }
        } else {
            result = validationData.validationResults[columnIndex]
        }
        if (result && result.disabledPendingId) {
            delete result.disabledPendingId
        }
    },
    getCellValidationResult: function(_ref4) {
        var _validationData$valid;
        var {
            rowKey: rowKey,
            columnIndex: columnIndex
        } = _ref4;
        var validationData = this._getValidationData(rowKey, true);
        return null === validationData || void 0 === validationData ? void 0 : null === (_validationData$valid = validationData.validationResults) || void 0 === _validationData$valid ? void 0 : _validationData$valid[columnIndex]
    },
    removeCellValidationResult: function(_ref5) {
        var {
            change: change,
            columnIndex: columnIndex
        } = _ref5;
        var validationData = this._getValidationData(null === change || void 0 === change ? void 0 : change.key);
        if (validationData && validationData.validationResults) {
            this.cancelCellValidationResult({
                change: change,
                columnIndex: columnIndex
            });
            delete validationData.validationResults[columnIndex]
        }
    },
    cancelCellValidationResult: function(_ref6) {
        var {
            change: change,
            columnIndex: columnIndex
        } = _ref6;
        var validationData = this._getValidationData(change.key);
        if (change && validationData.validationResults) {
            var result = validationData.validationResults[columnIndex];
            if (result) {
                result.deferred && result.deferred.reject(VALIDATION_CANCELLED);
                validationData.validationResults[columnIndex] = VALIDATION_CANCELLED
            }
        }
    },
    resetRowValidationResults: function(validationData) {
        if (validationData) {
            validationData.validationResults && delete validationData.validationResults;
            delete validationData.validated
        }
    },
    isInvalidCell: function(_ref7) {
        var {
            rowKey: rowKey,
            columnIndex: columnIndex
        } = _ref7;
        var result = this.getCellValidationResult({
            rowKey: rowKey,
            columnIndex: columnIndex
        });
        return validationResultIsValid(result) && result.status === VALIDATION_STATUS.invalid
    },
    getCellValidator: function(_ref8) {
        var {
            rowKey: rowKey,
            columnIndex: columnIndex
        } = _ref8;
        var validationData = this._getValidationData(rowKey);
        var groupConfig = validationData && ValidationEngine.getGroupConfig(validationData);
        var validators = groupConfig && groupConfig.validators;
        return validators && validators.filter(v => {
            var column = v.option("dataGetter")().column;
            return column ? column.index === columnIndex : false
        })[0]
    },
    setCellValidationStatus: function(cellOptions) {
        var validationResult = this.getCellValidationResult({
            rowKey: cellOptions.key,
            columnIndex: cellOptions.column.index
        });
        if (isDefined(validationResult)) {
            cellOptions.validationStatus = validationResult !== VALIDATION_CANCELLED ? validationResult.status : VALIDATION_CANCELLED
        } else {
            delete cellOptions.validationStatus
        }
    }
});
export var validatingModule = {
    defaultOptions: function() {
        return {
            editing: {
                texts: {
                    validationCancelChanges: messageLocalization.format("dxDataGrid-validationCancelChanges")
                }
            }
        }
    },
    controllers: {
        validating: ValidatingController
    },
    extenders: {
        controllers: {
            editing: {
                _addChange: function(changeParams) {
                    var change = this.callBase.apply(this, arguments);
                    var validatingController = this.getController("validating");
                    if (change && changeParams.type !== EDIT_DATA_REMOVE_TYPE) {
                        validatingController.updateValidationState(change)
                    }
                    return change
                },
                _handleChangesChange: function(args) {
                    this.callBase.apply(this, arguments);
                    var validatingController = this.getController("validating");
                    args.value.forEach(change => {
                        if (void 0 === validatingController._getValidationData(change.key)) {
                            validatingController.updateValidationState(change)
                        }
                    })
                },
                _updateRowAndPageIndices: function() {
                    var that = this;
                    var startInsertIndex = that.getView("rowsView").getTopVisibleItemIndex();
                    var rowIndex = startInsertIndex;
                    each(that.getChanges(), (_, _ref9) => {
                        var {
                            key: key,
                            type: type
                        } = _ref9;
                        var validationData = this.getController("validating")._getValidationData(key);
                        if (validationData && !validationData.isValid && validationData.pageIndex !== that._pageIndex) {
                            validationData.pageIndex = that._pageIndex;
                            if (type === EDIT_DATA_INSERT_TYPE) {
                                validationData.rowIndex = startInsertIndex
                            } else {
                                validationData.rowIndex = rowIndex
                            }
                            rowIndex++
                        }
                    })
                },
                _getValidationGroupsInForm: function(detailOptions) {
                    var validatingController = this.getController("validating");
                    var validationData = validatingController._getValidationData(detailOptions.key, true);
                    return {
                        validationGroup: validationData
                    }
                },
                _validateEditFormAfterUpdate: function(row, isCustomSetCellValue) {
                    if (isCustomSetCellValue && this._editForm && !row.isNewRow) {
                        this._editForm.validate()
                    }
                    this.callBase.apply(this, arguments)
                },
                _prepareEditCell: function(params) {
                    var isNotCanceled = this.callBase.apply(this, arguments);
                    var validatingController = this.getController("validating");
                    if (isNotCanceled && params.column.showEditorAlways) {
                        validatingController.updateValidationState({
                            key: params.key
                        })
                    }
                    return isNotCanceled
                },
                processItems: function(items, changeType) {
                    var changes = this.getChanges();
                    var dataController = this.getController("data");
                    var validatingController = this.getController("validating");
                    items = this.callBase(items, changeType);
                    var itemsCount = items.length;
                    var addInValidItem = function(change, validationData) {
                        var data = {
                            key: change.key
                        };
                        var index = function(change, items) {
                            var index = -1;
                            var isInsert = change.type === EDIT_DATA_INSERT_TYPE;
                            var key = change.key;
                            each(items, (function(i, item) {
                                if (equalByValue(key, isInsert ? item.key : dataController.keyOf(item))) {
                                    index = i;
                                    return false
                                }
                            }));
                            return index
                        }(change, items);
                        if (index >= 0) {
                            return
                        }
                        validationData.rowIndex = validationData.rowIndex > itemsCount ? validationData.rowIndex % itemsCount : validationData.rowIndex;
                        var rowIndex = validationData.rowIndex;
                        data[INSERT_INDEX] = 1;
                        items.splice(rowIndex, 0, data)
                    };
                    if (this.getEditMode() === EDIT_MODE_BATCH && "prepend" !== changeType && "append" !== changeType) {
                        changes.forEach(change => {
                            var key = change.key;
                            var validationData = validatingController._getValidationData(key);
                            if (validationData && change.type && validationData.pageIndex === this._pageIndex && (null === change || void 0 === change ? void 0 : change.pageIndex) !== this._pageIndex) {
                                addInValidItem(change, validationData)
                            }
                        })
                    }
                    return items
                },
                processDataItem: function(item) {
                    var isInserted = item.data[INSERT_INDEX];
                    var key = isInserted ? item.data.key : item.key;
                    var editMode = this.getEditMode();
                    if (editMode === EDIT_MODE_BATCH && isInserted && key) {
                        var changes = this.getChanges();
                        var editIndex = gridCoreUtils.getIndexByKey(key, changes);
                        if (editIndex >= 0) {
                            var change = changes[editIndex];
                            if (change.type !== EDIT_DATA_INSERT_TYPE) {
                                var oldData = this._getOldData(change.key);
                                item.data = extend(true, {}, oldData, change.data);
                                item.key = key
                            }
                        }
                    }
                    this.callBase.apply(this, arguments)
                },
                _createInvisibleColumnValidators: function(changes) {
                    var that = this;
                    var validatingController = this.getController("validating");
                    var columnsController = this.getController("columns");
                    var columns = columnsController.getColumns();
                    var invisibleColumns = columnsController.getInvisibleColumns().filter(column => !column.isBand);
                    var groupColumns = columnsController.getGroupColumns().filter(column => !column.showWhenGrouped && -1 === invisibleColumns.indexOf(column));
                    var invisibleColumnValidators = [];
                    var isCellVisible = (column, rowKey) => this._dataController.getRowIndexByKey(rowKey) >= 0 && invisibleColumns.indexOf(column) < 0;
                    invisibleColumns.push(...groupColumns);
                    if (-1 === FORM_BASED_MODES.indexOf(this.getEditMode())) {
                        each(columns, (function(_, column) {
                            changes.forEach((function(change) {
                                var data;
                                if (isCellVisible(column, change.key)) {
                                    return
                                }
                                if (change.type === EDIT_DATA_INSERT_TYPE) {
                                    data = change.data
                                } else if ("update" === change.type) {
                                    var oldData = that._getOldData(change.key);
                                    data = createObjectWithChanges(oldData, change.data)
                                }
                                if (data) {
                                    var validator = validatingController.createValidator({
                                        column: column,
                                        key: change.key,
                                        value: column.calculateCellValue(data)
                                    });
                                    if (validator) {
                                        invisibleColumnValidators.push(validator)
                                    }
                                }
                            }))
                        }))
                    }
                    return function() {
                        invisibleColumnValidators.forEach((function(validator) {
                            validator.dispose()
                        }))
                    }
                },
                _beforeSaveEditData: function(change, editIndex) {
                    var result = this.callBase.apply(this, arguments);
                    var validatingController = this.getController("validating");
                    var validationData = validatingController._getValidationData(null === change || void 0 === change ? void 0 : change.key);
                    if (change) {
                        var isValid = "remove" === change.type || validationData.isValid;
                        result = result || !isValid
                    } else {
                        var disposeValidators = this._createInvisibleColumnValidators(this.getChanges());
                        result = new Deferred;
                        this.executeOperation(result, () => {
                            validatingController.validate(true).done(isFullValid => {
                                disposeValidators();
                                this._updateRowAndPageIndices();
                                switch (this.getEditMode()) {
                                    case EDIT_MODE_CELL:
                                        if (!isFullValid) {
                                            this._focusEditingCell()
                                        }
                                        break;
                                    case EDIT_MODE_BATCH:
                                        if (!isFullValid) {
                                            this._resetEditRowKey();
                                            this._resetEditColumnName();
                                            this.getController("data").updateItems()
                                        }
                                }
                                result.resolve(!isFullValid)
                            })
                        })
                    }
                    return result.promise ? result.promise() : result
                },
                _beforeEditCell: function(rowIndex, columnIndex, item) {
                    var result = this.callBase(rowIndex, columnIndex, item);
                    if (this.getEditMode() === EDIT_MODE_CELL) {
                        var $cell = this._rowsView._getCellElement(rowIndex, columnIndex);
                        var validator = $cell && $cell.data("dxValidator");
                        var rowOptions = $cell && $cell.closest(".dx-row").data("options");
                        var value = validator && validator.option("adapter").getValue();
                        if (validator && cellValueShouldBeValidated(value, rowOptions)) {
                            var validatingController = this.getController("validating");
                            var deferred = new Deferred;
                            when(validatingController.validateCell(validator), result).done((validationResult, result) => {
                                deferred.resolve(validationResult.status === VALIDATION_STATUS.valid && result)
                            });
                            return deferred.promise()
                        } else if (!validator) {
                            return result
                        }
                    }
                },
                _afterSaveEditData: function(cancel) {
                    var $firstErrorRow;
                    var isCellEditMode = this.getEditMode() === EDIT_MODE_CELL;
                    each(this.getChanges(), (_, change) => {
                        var $errorRow = this._showErrorRow(change);
                        $firstErrorRow = $firstErrorRow || $errorRow
                    });
                    if ($firstErrorRow) {
                        var scrollable = this._rowsView.getScrollable();
                        if (scrollable) {
                            scrollable.update();
                            scrollable.scrollToElement($firstErrorRow)
                        }
                    }
                    if (cancel && isCellEditMode && this._needUpdateRow()) {
                        var editRowIndex = this.getEditRowIndex();
                        this._dataController.updateItems({
                            changeType: "update",
                            rowIndices: [editRowIndex]
                        });
                        this._focusEditingCell()
                    } else if (!cancel) {
                        var shouldResetValidationState = true;
                        if (isCellEditMode) {
                            var columns = this.getController("columns").getColumns();
                            var columnsWithValidatingEditors = columns.filter(col => {
                                var _col$validationRules;
                                return col.showEditorAlways && (null === (_col$validationRules = col.validationRules) || void 0 === _col$validationRules ? void 0 : _col$validationRules.length) > 0
                            }).length > 0;
                            shouldResetValidationState = !columnsWithValidatingEditors
                        }
                        if (shouldResetValidationState) {
                            this.getController("validating").initValidationState()
                        }
                    }
                },
                _handleDataChanged: function(args) {
                    var validationState = this.getController("validating")._validationState;
                    if ("standard" === this.option("scrolling.mode")) {
                        this.resetRowAndPageIndices()
                    }
                    if ("prepend" === args.changeType) {
                        each(validationState, (function(_, validationData) {
                            validationData.rowIndex += args.items.length
                        }))
                    }
                    this.callBase(args)
                },
                resetRowAndPageIndices: function() {
                    var validationState = this.getController("validating")._validationState;
                    each(validationState, (_, validationData) => {
                        if (validationData.pageIndex !== this._pageIndex) {
                            delete validationData.pageIndex;
                            delete validationData.rowIndex
                        }
                    })
                },
                _beforeCancelEditData: function() {
                    this.getController("validating").initValidationState();
                    this.callBase()
                },
                _showErrorRow: function(change) {
                    var $popupContent;
                    var errorHandling = this.getController("errorHandling");
                    var items = this.getController("data").items();
                    var rowIndex = this.getIndexByKey(change.key, items);
                    var validationData = this.getController("validating")._getValidationData(change.key);
                    if (!(null !== validationData && void 0 !== validationData && validationData.isValid) && null !== validationData && void 0 !== validationData && validationData.errorText && rowIndex >= 0) {
                        $popupContent = this.getPopupContent();
                        return errorHandling && errorHandling.renderErrorRow(null === validationData || void 0 === validationData ? void 0 : validationData.errorText, rowIndex, $popupContent)
                    }
                },
                updateFieldValue: function(e) {
                    var validatingController = this.getController("validating");
                    var deferred = new Deferred;
                    validatingController.removeCellValidationResult({
                        change: this.getChangeByKey(e.key),
                        columnIndex: e.column.index
                    });
                    this.callBase.apply(this, arguments).done(() => {
                        var currentValidator = validatingController.getCellValidator({
                            rowKey: e.key,
                            columnIndex: e.column.index
                        });
                        when(currentValidator && validatingController.validateCell(currentValidator)).done(validationResult => {
                            this.getController("editorFactory").refocus();
                            deferred.resolve(validationResult)
                        })
                    });
                    return deferred.promise()
                },
                highlightDataCell: function($cell, parameters) {
                    this.callBase.apply(this, arguments);
                    var validatingController = this.getController("validating");
                    validatingController.setCellValidationStatus(parameters);
                    var isEditableCell = !!parameters.setValue;
                    var cellModified = this.isCellModified(parameters);
                    var isValidated = isDefined(parameters.validationStatus);
                    var needValidation = cellModified && parameters.column.setCellValue || isEditableCell && !cellModified && !(parameters.row.isNewRow || !isValidated);
                    if (needValidation) {
                        var validator = $cell.data("dxValidator");
                        if (validator) {
                            when(this.getController("validating").validateCell(validator)).done(() => {
                                validatingController.setCellValidationStatus(parameters)
                            })
                        }
                    }
                },
                getChangeByKey: function(key) {
                    var changes = this.getChanges();
                    return changes[gridCoreUtils.getIndexByKey(key, changes)]
                },
                isCellModified: function(parameters) {
                    var cellModified = this.callBase(parameters);
                    var change = this.getChangeByKey(parameters.key);
                    var isCellInvalid = !!parameters.row && this.getController("validating").isInvalidCell({
                        rowKey: parameters.key,
                        columnIndex: parameters.column.index
                    });
                    return cellModified || this.getController("validating")._rowIsValidated(change) && isCellInvalid
                }
            },
            editorFactory: {
                _showRevertButton: function($container) {
                    if (!$container || !$container.length) {
                        return
                    }
                    var $tooltipElement = this._rowsView.element().find("." + this.addWidgetPrefix(REVERT_TOOLTIP_CLASS));
                    var $overlayContainer = $container.closest(".".concat(this.addWidgetPrefix(CONTENT_CLASS)));
                    $tooltipElement && $tooltipElement.remove();
                    $tooltipElement = $("<div>").addClass(this.addWidgetPrefix(REVERT_TOOLTIP_CLASS)).appendTo($container);
                    var tooltipOptions = {
                        animation: null,
                        visible: true,
                        width: "auto",
                        height: "auto",
                        target: $container,
                        shading: false,
                        container: $overlayContainer,
                        propagateOutsideClick: true,
                        closeOnOutsideClick: false,
                        copyRootClassesToWrapper: true,
                        _ignoreCopyRootClassesToWrapperDeprecation: true,
                        contentTemplate: () => {
                            var $buttonElement = $("<div>").addClass(REVERT_BUTTON_CLASS);
                            var buttonOptions = {
                                icon: "revert",
                                hint: this.option("editing.texts.validationCancelChanges"),
                                onClick: () => {
                                    this._editingController.cancelEditData()
                                }
                            };
                            return new Button($buttonElement, buttonOptions).$element()
                        },
                        position: {
                            my: "left top",
                            at: "right top",
                            offset: "1 0",
                            collision: "flip",
                            boundaryOffset: "0 0",
                            boundary: this._rowsView.element()
                        },
                        onPositioned: this._positionedHandler.bind(this)
                    };
                    return new Overlay($tooltipElement, tooltipOptions)
                },
                _hideFixedGroupCell: function($cell, overlayOptions) {
                    var $nextFixedRowElement;
                    var $groupCellElement;
                    var isFixedColumns = this._rowsView.isFixedColumns();
                    var isFormOrPopupEditMode = this._editingController.isFormOrPopupEditMode();
                    if (isFixedColumns && !isFormOrPopupEditMode) {
                        var nextRowOptions = $cell.closest(".dx-row").next().data("options");
                        if (nextRowOptions && "group" === nextRowOptions.rowType) {
                            $nextFixedRowElement = $(this._rowsView.getRowElement(nextRowOptions.rowIndex)).last();
                            $groupCellElement = $nextFixedRowElement.find("." + GROUP_CELL_CLASS);
                            if ($groupCellElement.length && "hidden" !== $groupCellElement.get(0).style.visibility) {
                                $groupCellElement.css("visibility", "hidden");
                                overlayOptions.onDisposing = function() {
                                    $groupCellElement.css("visibility", "")
                                }
                            }
                        }
                    }
                },
                _positionedHandler: function(e, isOverlayVisible) {
                    if (!e.component.__skipPositionProcessing) {
                        var isRevertButton = $(e.element).hasClass(this.addWidgetPrefix(REVERT_TOOLTIP_CLASS));
                        var needRepaint = !isRevertButton && this._rowsView.updateFreeSpaceRowHeight();
                        var normalizedPosition = this._normalizeValidationMessagePositionAndMaxWidth(e, isRevertButton, isOverlayVisible);
                        e.component.__skipPositionProcessing = !!(needRepaint || normalizedPosition);
                        if (normalizedPosition) {
                            e.component.option(normalizedPosition)
                        } else if (needRepaint) {
                            e.component.repaint()
                        }
                    }
                },
                _showValidationMessage: function($cell, messages, alignment, revertTooltip) {
                    var editorPopup = $cell.find(".dx-dropdowneditor-overlay").data("dxPopup");
                    var isOverlayVisible = editorPopup && editorPopup.option("visible");
                    var myPosition = isOverlayVisible ? "top right" : "top " + alignment;
                    var atPosition = isOverlayVisible ? "top left" : "bottom " + alignment;
                    var $overlayContainer = $cell.closest(".".concat(this.addWidgetPrefix(CONTENT_CLASS)));
                    var errorMessageText = "";
                    messages && messages.forEach((function(message) {
                        errorMessageText += (errorMessageText.length ? "<br/>" : "") + encodeHtml(message)
                    }));
                    var invalidMessageClass = this.addWidgetPrefix(WIDGET_INVALID_MESSAGE_CLASS);
                    this._rowsView.element().find("." + invalidMessageClass).remove();
                    var $overlayElement = $("<div>").addClass(INVALID_MESSAGE_CLASS).addClass(INVALID_MESSAGE_ALWAYS_CLASS).addClass(invalidMessageClass).html(errorMessageText).appendTo($cell);
                    var overlayOptions = {
                        target: $cell,
                        container: $overlayContainer,
                        shading: false,
                        width: "auto",
                        height: "auto",
                        visible: true,
                        animation: false,
                        propagateOutsideClick: true,
                        closeOnOutsideClick: false,
                        copyRootClassesToWrapper: true,
                        _ignoreCopyRootClassesToWrapperDeprecation: true,
                        position: {
                            collision: "flip",
                            boundary: this._rowsView.element(),
                            boundaryOffset: "0 0",
                            offset: {
                                x: 0,
                                y: !isOverlayVisible && browser.mozilla ? -1 : 0
                            },
                            my: myPosition,
                            at: atPosition
                        },
                        onPositioned: e => {
                            this._positionedHandler(e, isOverlayVisible);
                            this._shiftValidationMessageIfNeed(e.component.$content(), revertTooltip && revertTooltip.$content(), $cell)
                        }
                    };
                    this._hideFixedGroupCell($cell, overlayOptions);
                    new Overlay($overlayElement, overlayOptions)
                },
                _normalizeValidationMessagePositionAndMaxWidth: function(options, isRevertButton, isOverlayVisible) {
                    var fixedColumns = this._columnsController.getFixedColumns();
                    if (!fixedColumns || !fixedColumns.length) {
                        return
                    }
                    var position;
                    var visibleTableWidth = !isRevertButton && (that = this, element = options.element, rowIndex = $(element).closest("tr").index(), $cellElements = $(that._rowsView.getRowElement(rowIndex)).first().children().filter(":not(.dx-hidden-cell)"), that._rowsView._getWidths($cellElements).reduce((w1, w2) => w1 + w2, 0));
                    var that, element, rowIndex, $cellElements;
                    var $overlayContentElement = options.component.$content();
                    var validationMessageWidth = getOuterWidth($overlayContentElement, true);
                    var needMaxWidth = !isRevertButton && validationMessageWidth > visibleTableWidth;
                    var columnIndex = this._rowsView.getCellIndex($(options.element).closest("td"));
                    var boundaryNonFixedColumnsInfo = function(fixedColumns) {
                        var firstNonFixedColumnIndex;
                        var lastNonFixedColumnIndex;
                        fixedColumns.some((column, index) => {
                            if (column.command === COMMAND_TRANSPARENT) {
                                firstNonFixedColumnIndex = 0 === index ? -1 : index;
                                lastNonFixedColumnIndex = index === fixedColumns.length - 1 ? -1 : index + column.colspan - 1;
                                return true
                            }
                        });
                        return {
                            startColumnIndex: firstNonFixedColumnIndex,
                            endColumnIndex: lastNonFixedColumnIndex
                        }
                    }(fixedColumns);
                    if (!isRevertButton && (columnIndex === boundaryNonFixedColumnsInfo.startColumnIndex || needMaxWidth)) {
                        position = {
                            collision: "none flip",
                            my: "top left",
                            at: isOverlayVisible ? "top right" : "bottom left"
                        }
                    } else if (columnIndex === boundaryNonFixedColumnsInfo.endColumnIndex) {
                        position = {
                            collision: "none flip",
                            my: "top right",
                            at: isRevertButton || isOverlayVisible ? "top left" : "bottom right"
                        };
                        if (isRevertButton) {
                            position.offset = "-1 0"
                        }
                    }
                    return position && {
                        position: position,
                        maxWidth: needMaxWidth ? visibleTableWidth - 2 : void 0
                    }
                },
                _shiftValidationMessageIfNeed: function($content, $revertContent, $cell) {
                    if (!$revertContent) {
                        return
                    }
                    var contentOffset = $content.offset();
                    var revertContentOffset = $revertContent.offset();
                    if (contentOffset.top === revertContentOffset.top && contentOffset.left + getWidth($content) > revertContentOffset.left) {
                        var left = getWidth($revertContent) + PADDING_BETWEEN_TOOLTIPS;
                        $content.css("left", revertContentOffset.left < $cell.offset().left ? -left : left)
                    }
                },
                _getTooltipsSelector: function() {
                    var invalidMessageClass = this.addWidgetPrefix(WIDGET_INVALID_MESSAGE_CLASS);
                    var revertTooltipClass = this.addWidgetPrefix(REVERT_TOOLTIP_CLASS);
                    return ".dx-editor-cell ." + revertTooltipClass + ", .dx-editor-cell ." + invalidMessageClass + ", .dx-cell-modified ." + invalidMessageClass
                },
                init: function() {
                    this.callBase();
                    this._editingController = this.getController("editing");
                    this._columnsController = this.getController("columns");
                    this._rowsView = this.getView("rowsView")
                },
                loseFocus: function(skipValidator) {
                    if (!skipValidator) {
                        this.getController("validating").setValidator(null)
                    }
                    this.callBase()
                },
                updateCellState: function($element, validationResult, hideBorder) {
                    var _change$data;
                    var $focus = null === $element || void 0 === $element ? void 0 : $element.closest(this._getFocusCellSelector());
                    var $cell = null !== $focus && void 0 !== $focus && $focus.is("td") ? $focus : null;
                    var rowOptions = null === $focus || void 0 === $focus ? void 0 : $focus.closest(".dx-row").data("options");
                    var change = rowOptions ? this.getController("editing").getChangeByKey(rowOptions.key) : null;
                    var column = $cell && this.getController("columns").getVisibleColumns()[$cell.index()];
                    var isCellModified = void 0 !== (null === change || void 0 === change ? void 0 : null === (_change$data = change.data) || void 0 === _change$data ? void 0 : _change$data[null === column || void 0 === column ? void 0 : column.name]) && !this._editingController.isSaving();
                    var revertTooltip;
                    if ((null === validationResult || void 0 === validationResult ? void 0 : validationResult.status) === VALIDATION_STATUS.invalid || isCellModified) {
                        if (this._editingController.getEditMode() === EDIT_MODE_CELL) {
                            revertTooltip = this._showRevertButton($focus)
                        }
                    }
                    var showValidationMessage = validationResult && validationResult.status === VALIDATION_STATUS.invalid;
                    if (showValidationMessage && $cell && column && validationResult && validationResult.brokenRules) {
                        var errorMessages = [];
                        validationResult.brokenRules.forEach((function(rule) {
                            errorMessages.push(rule.message)
                        }));
                        this._showValidationMessage($focus, errorMessages, column.alignment || "left", revertTooltip)
                    }!hideBorder && this._rowsView.element() && this._rowsView.updateFreeSpaceRowHeight()
                },
                focus: function($element, hideBorder) {
                    if (!arguments.length) {
                        return this.callBase()
                    }
                    var $tooltips = $element && $element.closest("." + this.addWidgetPrefix(ROWS_VIEW_CLASS)).find(this._getTooltipsSelector());
                    $tooltips && $tooltips.remove();
                    if (null !== $element && void 0 !== $element && $element.hasClass("dx-row")) {
                        return this.callBase($element, hideBorder)
                    }
                    var $focus = null === $element || void 0 === $element ? void 0 : $element.closest(this._getFocusCellSelector());
                    var callBase = this.callBase;
                    var validator = $focus && ($focus.data("dxValidator") || $element.find("." + this.addWidgetPrefix(VALIDATOR_CLASS)).eq(0).data("dxValidator"));
                    var rowOptions = $focus && $focus.closest(".dx-row").data("options");
                    var editingController = this.getController("editing");
                    var change = rowOptions ? editingController.getChangeByKey(rowOptions.key) : null;
                    var validationResult;
                    var validatingController = this.getController("validating");
                    if (validator) {
                        validatingController.setValidator(validator);
                        var value = validator.option("adapter").getValue();
                        if (cellValueShouldBeValidated(value, rowOptions) || validatingController._rowIsValidated(change)) {
                            editingController.waitForDeferredOperations().done(() => {
                                when(validatingController.validateCell(validator)).done(result => {
                                    validationResult = result;
                                    var column = validationResult.validator.option("dataGetter")().column;
                                    if (change && column && !validatingController.isCurrentValidatorProcessing({
                                            rowKey: change.key,
                                            columnIndex: column.index
                                        })) {
                                        return
                                    }
                                    if (validationResult.status === VALIDATION_STATUS.invalid) {
                                        hideBorder = true
                                    }
                                    this.updateCellState($element, validationResult, hideBorder);
                                    callBase.call(this, $element, hideBorder)
                                })
                            });
                            return this.callBase($element, hideBorder)
                        }
                    }
                    this.updateCellState($element, validationResult, hideBorder);
                    return this.callBase($element, hideBorder)
                },
                getEditorInstance: function($container) {
                    var $editor = $container.find(".dx-texteditor").eq(0);
                    return gridCoreUtils.getWidgetInstance($editor)
                }
            },
            data: {
                _isCellChanged: function(oldRow, newRow, visibleRowIndex, columnIndex, isLiveUpdate) {
                    var _oldRow$cells, _cell$column$validati;
                    var cell = null === (_oldRow$cells = oldRow.cells) || void 0 === _oldRow$cells ? void 0 : _oldRow$cells[columnIndex];
                    var oldValidationStatus = cell && cell.validationStatus;
                    var validatingController = this.getController("validating");
                    var validationResult = validatingController.getCellValidationResult({
                        rowKey: oldRow.key,
                        columnIndex: columnIndex
                    });
                    var validationData = validatingController._getValidationData(oldRow.key);
                    var newValidationStatus = validationResultIsValid(validationResult) ? validationResult.status : validationResult;
                    var rowIsModified = JSON.stringify(newRow.modifiedValues) !== JSON.stringify(oldRow.modifiedValues);
                    var cellIsMarkedAsInvalid = $(null === cell || void 0 === cell ? void 0 : cell.cellElement).hasClass(this.addWidgetPrefix(INVALIDATE_CLASS));
                    var editingChanged = oldRow.isEditing !== newRow.isEditing;
                    var hasValidationRules = null === cell || void 0 === cell ? void 0 : null === (_cell$column$validati = cell.column.validationRules) || void 0 === _cell$column$validati ? void 0 : _cell$column$validati.length;
                    if (editingChanged && hasValidationRules || oldValidationStatus !== newValidationStatus && rowIsModified || validationData.isValid && cellIsMarkedAsInvalid) {
                        return true
                    }
                    return this.callBase.apply(this, arguments)
                }
            }
        },
        views: {
            rowsView: {
                updateFreeSpaceRowHeight: function($table) {
                    var $rowElements;
                    var $freeSpaceRowElement;
                    var $freeSpaceRowElements;
                    var $element = this.element();
                    var $tooltipContent = $element && $element.find("." + this.addWidgetPrefix(WIDGET_INVALID_MESSAGE_CLASS) + " .dx-overlay-content");
                    this.callBase($table);
                    if ($tooltipContent && $tooltipContent.length) {
                        $rowElements = this._getRowElements();
                        $freeSpaceRowElements = this._getFreeSpaceRowElements($table);
                        $freeSpaceRowElement = $freeSpaceRowElements.first();
                        if ($freeSpaceRowElement && 1 === $rowElements.length && (!$freeSpaceRowElement.is(":visible") || getOuterHeight($tooltipContent) > getOuterHeight($freeSpaceRowElement))) {
                            $freeSpaceRowElements.show();
                            setHeight($freeSpaceRowElements, getOuterHeight($tooltipContent));
                            return true
                        }
                    }
                },
                _formItemPrepared: function(cellOptions, $container) {
                    this.callBase.apply(this, arguments);
                    deferUpdate(() => {
                        var $editor = $container.find(".dx-widget").first();
                        var isEditorDisposed = $editor.length && !$editor.children().length;
                        if (!isEditorDisposed) {
                            this.getController("validating").createValidator(cellOptions, $editor)
                        }
                    })
                },
                _cellPrepared: function($cell, parameters) {
                    if (!this.getController("editing").isFormOrPopupEditMode()) {
                        this.getController("validating").createValidator(parameters, $cell)
                    }
                    this.callBase.apply(this, arguments)
                },
                _restoreErrorRow: function(contentTable) {
                    var editingController = this.getController("editing");
                    editingController && editingController.hasChanges() && this._getRowElements(contentTable).each((_, item) => {
                        var rowOptions = $(item).data("options");
                        if (rowOptions) {
                            var change = editingController.getChangeByKey(rowOptions.key);
                            change && editingController._showErrorRow(change)
                        }
                    })
                }
            }
        }
    }
};
