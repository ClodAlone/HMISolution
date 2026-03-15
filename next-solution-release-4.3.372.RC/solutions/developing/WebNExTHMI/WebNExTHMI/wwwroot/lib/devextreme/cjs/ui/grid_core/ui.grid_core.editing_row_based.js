/**
 * DevExtreme (cjs/ui/grid_core/ui.grid_core.editing_row_based.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.editingRowBasedModule = void 0;
var _uiGrid_core = require("./ui.grid_core.editing_constants");
var EDIT_ROW = "dx-edit-row";
var editingRowBasedModule = {
    extenders: {
        controllers: {
            editing: {
                isRowEditMode: function() {
                    return this.getEditMode() === _uiGrid_core.EDIT_MODE_ROW
                },
                _afterCancelEditData: function(rowIndex) {
                    var dataController = this._dataController;
                    if (this.isRowBasedEditMode() && rowIndex >= 0) {
                        dataController.updateItems({
                            changeType: "update",
                            rowIndices: [rowIndex, rowIndex + 1]
                        })
                    } else {
                        this.callBase.apply(this, arguments)
                    }
                },
                _isDefaultButtonVisible: function(button, options) {
                    var isRowMode = this.isRowBasedEditMode();
                    var isEditRow = options.row && options.row.rowIndex === this._getVisibleEditRowIndex();
                    if (isRowMode) {
                        switch (button.name) {
                            case "edit":
                                return !isEditRow && this.allowUpdating(options);
                            case "delete":
                                return this.callBase.apply(this, arguments) && !isEditRow;
                            case "save":
                            case "cancel":
                                return isEditRow;
                            default:
                                return this.callBase.apply(this, arguments)
                        }
                    }
                    return this.callBase.apply(this, arguments)
                },
                isEditRow: function(rowIndex) {
                    return this.isRowBasedEditMode() && this._isEditRowByIndex(rowIndex)
                },
                _cancelSaving: function() {
                    if (this.isRowBasedEditMode()) {
                        if (!this.hasChanges()) {
                            this._cancelEditDataCore()
                        }
                    }
                    this.callBase.apply(this, arguments)
                },
                _refreshCore: function() {
                    if (this.isRowBasedEditMode()) {
                        this.init()
                    }
                    this.callBase.apply(this, arguments)
                },
                _isEditColumnVisible: function() {
                    var result = this.callBase.apply(this, arguments);
                    var editingOptions = this.option("editing");
                    var isRowEditMode = this.isRowEditMode();
                    var isVisibleInRowEditMode = editingOptions.allowUpdating || editingOptions.allowAdding;
                    return result || isRowEditMode && isVisibleInRowEditMode
                },
                _focusEditorIfNeed: function() {
                    var _this = this;
                    var editMode = this.getEditMode();
                    if (this._needFocusEditor) {
                        if (-1 !== _uiGrid_core.MODES_WITH_DELAYED_FOCUS.indexOf(editMode)) {
                            var $editingCell = this.getFocusedCellInRow(this._getVisibleEditRowIndex());
                            this._delayedInputFocus($editingCell, (function() {
                                $editingCell && _this.component.focus($editingCell)
                            }))
                        }
                        this._needFocusEditor = false
                    }
                }
            },
            data: {
                _getChangedColumnIndices: function(oldItem, newItem, rowIndex, isLiveUpdate) {
                    var editingController = this.getController("editing");
                    if (editingController.isRowBasedEditMode() && oldItem.isEditing !== newItem.isEditing) {
                        return
                    }
                    return this.callBase.apply(this, arguments)
                }
            }
        },
        views: {
            rowsView: {
                _createRow: function(row) {
                    var $row = this.callBase.apply(this, arguments);
                    if (row) {
                        var editingController = this._editingController;
                        var isEditRow = editingController.isEditRow(row.rowIndex);
                        if (isEditRow) {
                            $row.addClass(EDIT_ROW);
                            $row.removeClass(_uiGrid_core.ROW_SELECTED_CLASS);
                            if ("detail" === row.rowType) {
                                $row.addClass(this.addWidgetPrefix(_uiGrid_core.EDIT_FORM_CLASS))
                            }
                        }
                    }
                    return $row
                },
                _update: function(change) {
                    this.callBase(change);
                    if ("updateSelection" === change.changeType) {
                        this.getTableElements().children("tbody").children("." + EDIT_ROW).removeClass(_uiGrid_core.ROW_SELECTED_CLASS)
                    }
                }
            }
        }
    }
};
exports.editingRowBasedModule = editingRowBasedModule;
