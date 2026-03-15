/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.selection.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import gridCore from "../data_grid/ui.data_grid.core";
import gridCoreUtils from "./ui.grid_core.utils";
import {
    isDefined
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import {
    touch
} from "../../core/utils/support";
import {
    name as clickEventName
} from "../../events/click";
import messageLocalization from "../../localization/message";
import {
    addNamespace,
    isCommandKeyPressed
} from "../../events/utils/index";
import holdEvent from "../../events/hold";
import Selection from "../selection/selection";
import {
    Deferred
} from "../../core/utils/deferred";
import errors from "../widget/ui.errors";
var EDITOR_CELL_CLASS = "dx-editor-cell";
var ROW_CLASS = "dx-row";
var ROW_SELECTION_CLASS = "dx-selection";
var SELECT_CHECKBOX_CLASS = "dx-select-checkbox";
var CHECKBOXES_HIDDEN_CLASS = "dx-select-checkboxes-hidden";
var COMMAND_SELECT_CLASS = "dx-command-select";
var SELECTION_DISABLED_CLASS = "dx-selection-disabled";
var DATA_ROW_CLASS = "dx-data-row";
var SHOW_CHECKBOXES_MODE = "selection.showCheckBoxesMode";
var SELECTION_MODE = "selection.mode";
var processLongTap = function(that, dxEvent) {
    var selectionController = that.getController("selection");
    var rowsView = that.getView("rowsView");
    var $row = $(dxEvent.target).closest("." + DATA_ROW_CLASS);
    var rowIndex = rowsView.getRowIndex($row);
    if (rowIndex < 0) {
        return
    }
    if ("onLongTap" === that.option(SHOW_CHECKBOXES_MODE)) {
        if (selectionController.isSelectionWithCheckboxes()) {
            selectionController.stopSelectionWithCheckboxes()
        } else {
            selectionController.startSelectionWithCheckboxes()
        }
    } else {
        if ("onClick" === that.option(SHOW_CHECKBOXES_MODE)) {
            selectionController.startSelectionWithCheckboxes()
        }
        if ("always" !== that.option(SHOW_CHECKBOXES_MODE)) {
            selectionController.changeItemSelection(rowIndex, {
                control: true
            })
        }
    }
};
var SelectionController = gridCore.Controller.inherit(function() {
    var selectionCellTemplate = (container, options) => {
        var component = options.component;
        var rowsView = component.getView("rowsView");
        if (component.option("renderAsync") && !component.option("selection.deferred")) {
            options.value = component.isRowSelected(options.row.key)
        }
        rowsView.renderSelectCheckBoxContainer($(container), options)
    };
    var selectionHeaderTemplate = (container, options) => {
        var column = options.column;
        var $cellElement = $(container);
        var columnHeadersView = options.component.getView("columnHeadersView");
        $cellElement.addClass(EDITOR_CELL_CLASS);
        columnHeadersView._renderSelectAllCheckBox($cellElement, column);
        columnHeadersView._attachSelectAllCheckBoxClickEvent($cellElement)
    };
    return {
        init: function() {
            var {
                deferred: deferred,
                selectAllMode: selectAllMode,
                mode: mode
            } = this.option("selection") || {};
            if ("infinite" === this.option("scrolling.mode") && !deferred && "multiple" === mode && "allPages" === selectAllMode) {
                errors.log("W1018")
            }
            this._dataController = this.getController("data");
            this._selectionMode = mode;
            this._isSelectionWithCheckboxes = false;
            this._selection = this._createSelection();
            this._updateSelectColumn();
            this.createAction("onSelectionChanged", {
                excludeValidators: ["disabled", "readOnly"]
            });
            this._dataController && this._dataController.pushed.add(this._handleDataPushed.bind(this))
        },
        _handleDataPushed: function(changes) {
            var removedKeys = changes.filter(change => "remove" === change.type).map(change => change.key);
            removedKeys.length && this.deselectRows(removedKeys)
        },
        _getSelectionConfig: function() {
            var dataController = this._dataController;
            var columnsController = this.getController("columns");
            var selectionOptions = this.option("selection") || {};
            var deferred = selectionOptions.deferred;
            var scrollingMode = this.option("scrolling.mode");
            var virtualPaging = "virtual" === scrollingMode || "infinite" === scrollingMode;
            var allowSelectAll = this.option("selection.allowSelectAll");
            var legacyScrollingMode = this.option("scrolling.legacyMode");
            return {
                selectedKeys: this.option("selectedRowKeys"),
                mode: this._selectionMode,
                deferred: deferred,
                maxFilterLengthInRequest: selectionOptions.maxFilterLengthInRequest,
                selectionFilter: this.option("selectionFilter"),
                ignoreDisabledItems: true,
                allowLoadByRange: function() {
                    var hasGroupColumns = columnsController.getGroupColumns().length > 0;
                    return virtualPaging && !legacyScrollingMode && !hasGroupColumns && allowSelectAll && !deferred
                },
                key: function() {
                    return null === dataController || void 0 === dataController ? void 0 : dataController.key()
                },
                keyOf: function(item) {
                    return null === dataController || void 0 === dataController ? void 0 : dataController.keyOf(item)
                },
                dataFields: function() {
                    var _dataController$dataS;
                    return null === (_dataController$dataS = dataController.dataSource()) || void 0 === _dataController$dataS ? void 0 : _dataController$dataS.select()
                },
                load: function(options) {
                    var _dataController$dataS2;
                    return (null === (_dataController$dataS2 = dataController.dataSource()) || void 0 === _dataController$dataS2 ? void 0 : _dataController$dataS2.load(options)) || (new Deferred).resolve([])
                },
                plainItems: function() {
                    return dataController.items(true)
                },
                isItemSelected: function(item) {
                    return item.selected
                },
                isSelectableItem: function(item) {
                    return "data" === (null === item || void 0 === item ? void 0 : item.rowType) && !item.isNewRow
                },
                getItemData: function(item) {
                    return (null === item || void 0 === item ? void 0 : item.oldData) || (null === item || void 0 === item ? void 0 : item.data) || item
                },
                filter: function() {
                    return dataController.getCombinedFilter(deferred)
                },
                totalCount: () => dataController.totalCount(),
                getLoadOptions: function(loadItemIndex, focusedItemIndex, shiftItemIndex) {
                    var _dataController$dataS3, _dataController$dataS4;
                    var {
                        sort: sort,
                        filter: filter
                    } = null !== (_dataController$dataS3 = null === (_dataController$dataS4 = dataController.dataSource()) || void 0 === _dataController$dataS4 ? void 0 : _dataController$dataS4.lastLoadOptions()) && void 0 !== _dataController$dataS3 ? _dataController$dataS3 : {};
                    var minIndex = Math.min(loadItemIndex, focusedItemIndex);
                    var maxIndex = Math.max(loadItemIndex, focusedItemIndex);
                    if (isDefined(shiftItemIndex)) {
                        minIndex = Math.min(shiftItemIndex, minIndex);
                        maxIndex = Math.max(shiftItemIndex, maxIndex)
                    }
                    var take = maxIndex - minIndex + 1;
                    return {
                        skip: minIndex,
                        take: take,
                        filter: filter,
                        sort: sort
                    }
                },
                onSelectionChanged: this._updateSelectedItems.bind(this)
            }
        },
        _updateSelectColumn: function() {
            var columnsController = this.getController("columns");
            var isSelectColumnVisible = this.isSelectColumnVisible();
            columnsController.addCommandColumn({
                type: "selection",
                command: "select",
                visible: isSelectColumnVisible,
                visibleIndex: -1,
                dataType: "boolean",
                alignment: "center",
                cssClass: COMMAND_SELECT_CLASS,
                width: "auto",
                cellTemplate: selectionCellTemplate,
                headerCellTemplate: selectionHeaderTemplate
            });
            columnsController.columnOption("command:select", "visible", isSelectColumnVisible)
        },
        _createSelection: function() {
            var options = this._getSelectionConfig();
            return new Selection(options)
        },
        _fireSelectionChanged: function(options) {
            var argument = this.option("selection.deferred") ? {
                selectionFilter: this.option("selectionFilter")
            } : {
                selectedRowKeys: this.option("selectedRowKeys")
            };
            this.selectionChanged.fire(argument);
            if (options) {
                this.executeAction("onSelectionChanged", options)
            }
        },
        _updateCheckboxesState: function(options) {
            var isDeferredMode = options.isDeferredMode;
            var selectionFilter = options.selectionFilter;
            var selectedItemKeys = options.selectedItemKeys;
            var removedItemKeys = options.removedItemKeys;
            if ("onClick" === this.option(SHOW_CHECKBOXES_MODE)) {
                if (isDeferredMode ? selectionFilter && function(that, selectionFilter) {
                        var keyIndex = 0;
                        var store = that._dataController.store();
                        var key = store && store.key();
                        var isComplexKey = Array.isArray(key);
                        if (!selectionFilter.length) {
                            return false
                        }
                        if (isComplexKey && Array.isArray(selectionFilter[0]) && "and" === selectionFilter[1]) {
                            for (var i = 0; i < selectionFilter.length; i++) {
                                if (Array.isArray(selectionFilter[i])) {
                                    if (selectionFilter[i][0] !== key[keyIndex] || "=" !== selectionFilter[i][1]) {
                                        return true
                                    }
                                    keyIndex++
                                }
                            }
                            return false
                        }
                        return key !== selectionFilter[0]
                    }(this, selectionFilter) : selectedItemKeys.length > 1) {
                    this.startSelectionWithCheckboxes()
                } else if (isDeferredMode ? selectionFilter && !selectionFilter.length : 0 === selectedItemKeys.length && removedItemKeys.length) {
                    this.stopSelectionWithCheckboxes()
                }
            }
        },
        _updateSelectedItems: function(args) {
            var selectionChangedOptions;
            var isDeferredMode = this.option("selection.deferred");
            var selectionFilter = this._selection.selectionFilter();
            var dataController = this._dataController;
            var items = dataController.items();
            if (!items) {
                return
            }
            var isSelectionWithCheckboxes = this.isSelectionWithCheckboxes();
            var changedItemIndexes = this.getChangedItemIndexes(items);
            this._updateCheckboxesState({
                selectedItemKeys: args.selectedItemKeys,
                removedItemKeys: args.removedItemKeys,
                selectionFilter: selectionFilter,
                isDeferredMode: isDeferredMode
            });
            if (changedItemIndexes.length || isSelectionWithCheckboxes !== this.isSelectionWithCheckboxes()) {
                dataController.updateItems({
                    changeType: "updateSelection",
                    itemIndexes: changedItemIndexes
                })
            }
            if (isDeferredMode) {
                this.option("selectionFilter", selectionFilter);
                selectionChangedOptions = {}
            } else if (args.addedItemKeys.length || args.removedItemKeys.length) {
                this._selectedItemsInternalChange = true;
                this.option("selectedRowKeys", args.selectedItemKeys.slice(0));
                this._selectedItemsInternalChange = false;
                selectionChangedOptions = {
                    selectedRowsData: args.selectedItems.slice(0),
                    selectedRowKeys: args.selectedItemKeys.slice(0),
                    currentSelectedRowKeys: args.addedItemKeys.slice(0),
                    currentDeselectedRowKeys: args.removedItemKeys.slice(0)
                }
            }
            this._fireSelectionChanged(selectionChangedOptions)
        },
        getChangedItemIndexes: function(items) {
            var itemIndexes = [];
            var isDeferredSelection = this.option("selection.deferred");
            for (var i = 0, length = items.length; i < length; i++) {
                var row = items[i];
                var isItemSelected = this.isRowSelected(isDeferredSelection ? row.data : row.key);
                if (this._selection.isDataItem(row) && row.isSelected !== isItemSelected) {
                    itemIndexes.push(i)
                }
            }
            return itemIndexes
        },
        callbackNames: function() {
            return ["selectionChanged"]
        },
        optionChanged: function(args) {
            this.callBase(args);
            switch (args.name) {
                case "selection":
                    var oldSelectionMode = this._selectionMode;
                    this.init();
                    if ("selection.showCheckBoxesMode" !== args.fullName) {
                        var selectionMode = this._selectionMode;
                        var selectedRowKeys = this.option("selectedRowKeys");
                        if (oldSelectionMode !== selectionMode) {
                            if ("single" === selectionMode) {
                                if (selectedRowKeys.length > 1) {
                                    selectedRowKeys = [selectedRowKeys[0]]
                                }
                            } else if ("multiple" !== selectionMode) {
                                selectedRowKeys = []
                            }
                        }
                        this.selectRows(selectedRowKeys).always(() => {
                            this._fireSelectionChanged()
                        })
                    }
                    this.getController("columns").updateColumns();
                    args.handled = true;
                    break;
                case "selectionFilter":
                    this._selection.selectionFilter(args.value);
                    args.handled = true;
                    break;
                case "selectedRowKeys":
                    var value = args.value || [];
                    if (Array.isArray(value) && !this._selectedItemsInternalChange && (this.component.getDataSource() || !value.length)) {
                        this.selectRows(value)
                    }
                    args.handled = true
            }
        },
        publicMethods: function() {
            return ["selectRows", "deselectRows", "selectRowsByIndexes", "getSelectedRowKeys", "getSelectedRowsData", "clearSelection", "selectAll", "deselectAll", "startSelectionWithCheckboxes", "stopSelectionWithCheckboxes", "isRowSelected"]
        },
        isRowSelected: function(arg) {
            return this._selection.isItemSelected(arg)
        },
        isSelectColumnVisible: function() {
            return "multiple" === this.option(SELECTION_MODE) && ("always" === this.option(SHOW_CHECKBOXES_MODE) || "onClick" === this.option(SHOW_CHECKBOXES_MODE) || this._isSelectionWithCheckboxes)
        },
        _isOnePageSelectAll: function() {
            return "page" === this.option("selection.selectAllMode")
        },
        isSelectAll: function() {
            return this._selection.getSelectAllState(this._isOnePageSelectAll())
        },
        selectAll: function() {
            if ("onClick" === this.option(SHOW_CHECKBOXES_MODE)) {
                this.startSelectionWithCheckboxes()
            }
            return this._selection.selectAll(this._isOnePageSelectAll())
        },
        deselectAll: function() {
            return this._selection.deselectAll(this._isOnePageSelectAll())
        },
        clearSelection: function() {
            return this.selectedItemKeys([])
        },
        refresh: function() {
            var selectedRowKeys = this.option("selectedRowKeys") || [];
            if (!this.option("selection.deferred") && selectedRowKeys.length) {
                return this.selectedItemKeys(selectedRowKeys)
            }
            return (new Deferred).resolve().promise()
        },
        selectedItemKeys: function(value, preserve, isDeselect, isSelectAll) {
            return this._selection.selectedItemKeys(value, preserve, isDeselect, isSelectAll)
        },
        getSelectedRowKeys: function() {
            return this._selection.getSelectedItemKeys()
        },
        selectRows: function(keys, preserve) {
            return this.selectedItemKeys(keys, preserve)
        },
        deselectRows: function(keys) {
            return this.selectedItemKeys(keys, true, true)
        },
        selectRowsByIndexes: function(indexes) {
            var items = this._dataController.items();
            var keys = [];
            if (!Array.isArray(indexes)) {
                indexes = Array.prototype.slice.call(arguments, 0)
            }
            each(indexes, (function() {
                var item = items[this];
                if (item && "data" === item.rowType) {
                    keys.push(item.key)
                }
            }));
            return this.selectRows(keys)
        },
        getSelectedRowsData: function() {
            return this._selection.getSelectedItems()
        },
        changeItemSelection: function(visibleItemIndex, keys) {
            keys = keys || {};
            if (this.isSelectionWithCheckboxes()) {
                keys.control = true
            }
            var loadedItemIndex = visibleItemIndex + this._dataController.getRowIndexOffset() - this._dataController.getRowIndexOffset(true);
            return this._selection.changeItemSelection(loadedItemIndex, keys)
        },
        focusedItemIndex: function(itemIndex) {
            if (isDefined(itemIndex)) {
                this._selection._focusedItemIndex = itemIndex
            } else {
                return this._selection._focusedItemIndex
            }
        },
        isSelectionWithCheckboxes: function() {
            return "multiple" === this.option(SELECTION_MODE) && ("always" === this.option(SHOW_CHECKBOXES_MODE) || this._isSelectionWithCheckboxes)
        },
        startSelectionWithCheckboxes: function() {
            if ("multiple" === this.option(SELECTION_MODE) && !this.isSelectionWithCheckboxes()) {
                this._isSelectionWithCheckboxes = true;
                this._updateSelectColumn();
                return true
            }
            return false
        },
        stopSelectionWithCheckboxes: function() {
            if (this._isSelectionWithCheckboxes) {
                this._isSelectionWithCheckboxes = false;
                this._updateSelectColumn();
                return true
            }
            return false
        }
    }
}());
export var selectionModule = {
    defaultOptions: function() {
        return {
            selection: {
                mode: "none",
                showCheckBoxesMode: "onClick",
                allowSelectAll: true,
                selectAllMode: "allPages",
                maxFilterLengthInRequest: 1500,
                deferred: false
            },
            selectionFilter: [],
            selectedRowKeys: []
        }
    },
    controllers: {
        selection: SelectionController
    },
    extenders: {
        controllers: {
            data: {
                init: function() {
                    var selectionController = this.getController("selection");
                    var isDeferredMode = this.option("selection.deferred");
                    this.callBase.apply(this, arguments);
                    if (isDeferredMode) {
                        selectionController._updateCheckboxesState({
                            isDeferredMode: true,
                            selectionFilter: this.option("selectionFilter")
                        })
                    }
                },
                _loadDataSource: function() {
                    var that = this;
                    return that.callBase().done((function() {
                        that.getController("selection").refresh()
                    }))
                },
                _processDataItem: function(item, options) {
                    var that = this;
                    var selectionController = that.getController("selection");
                    var hasSelectColumn = selectionController.isSelectColumnVisible();
                    var isDeferredSelection = options.isDeferredSelection = void 0 === options.isDeferredSelection ? this.option("selection.deferred") : options.isDeferredSelection;
                    var dataItem = this.callBase.apply(this, arguments);
                    dataItem.isSelected = selectionController.isRowSelected(isDeferredSelection ? dataItem.data : dataItem.key);
                    if (hasSelectColumn && dataItem.values) {
                        for (var i = 0; i < options.visibleColumns.length; i++) {
                            if ("select" === options.visibleColumns[i].command) {
                                dataItem.values[i] = dataItem.isSelected;
                                break
                            }
                        }
                    }
                    return dataItem
                },
                refresh: function(options) {
                    var that = this;
                    var d = new Deferred;
                    this.callBase.apply(this, arguments).done((function() {
                        if (!options || options.selection) {
                            that.getController("selection").refresh().done(d.resolve).fail(d.reject)
                        } else {
                            d.resolve()
                        }
                    })).fail(d.reject);
                    return d.promise()
                },
                _handleDataChanged: function(e) {
                    this.callBase.apply(this, arguments);
                    if ((!e || "refresh" === e.changeType) && !this._repaintChangesOnly) {
                        this.getController("selection").focusedItemIndex(-1)
                    }
                },
                _applyChange: function(change) {
                    if (change && "updateSelection" === change.changeType) {
                        change.items.forEach((item, index) => {
                            var currentItem = this._items[index];
                            if (currentItem) {
                                currentItem.isSelected = item.isSelected;
                                currentItem.values = item.values
                            }
                        });
                        return
                    }
                    return this.callBase.apply(this, arguments)
                },
                _endUpdateCore: function() {
                    var changes = this._changes;
                    var isUpdateSelection = changes.length > 1 && changes.every(change => "updateSelection" === change.changeType);
                    if (isUpdateSelection) {
                        var itemIndexes = changes.map(change => change.itemIndexes || []).reduce((a, b) => a.concat(b));
                        this._changes = [{
                            changeType: "updateSelection",
                            itemIndexes: itemIndexes
                        }]
                    }
                    this.callBase.apply(this, arguments)
                }
            },
            contextMenu: {
                _contextMenuPrepared: function(options) {
                    var dxEvent = options.event;
                    if (dxEvent.originalEvent && "dxhold" !== dxEvent.originalEvent.type || options.items && options.items.length > 0) {
                        return
                    }
                    processLongTap(this, dxEvent)
                }
            }
        },
        views: {
            columnHeadersView: {
                init: function() {
                    this.callBase();
                    this.getController("selection").selectionChanged.add(this._updateSelectAllValue.bind(this))
                },
                _updateSelectAllValue: function() {
                    var $element = this.element();
                    var $editor = $element && $element.find("." + SELECT_CHECKBOX_CLASS);
                    if ($element && $editor.length && "multiple" === this.option("selection.mode")) {
                        var selectAllValue = this.getController("selection").isSelectAll();
                        var hasSelection = false !== selectAllValue;
                        var isVisible = this.option("selection.allowSelectAll") ? !this.getController("data").isEmpty() : hasSelection;
                        $editor.dxCheckBox("instance").option({
                            visible: isVisible,
                            value: selectAllValue
                        })
                    }
                },
                _handleDataChanged: function(e) {
                    this.callBase(e);
                    if (!e || "refresh" === e.changeType) {
                        this._updateSelectAllValue()
                    }
                },
                _renderSelectAllCheckBox: function($container, column) {
                    var that = this;
                    var selectionController = that.getController("selection");
                    var isEmptyData = that.getController("data").isEmpty();
                    var groupElement = $("<div>").appendTo($container).addClass(SELECT_CHECKBOX_CLASS);
                    that.setAria("label", messageLocalization.format("dxDataGrid-ariaSelectAll"), $container);
                    that.getController("editorFactory").createEditor(groupElement, extend({}, column, {
                        parentType: "headerRow",
                        dataType: "boolean",
                        value: selectionController.isSelectAll(),
                        editorOptions: {
                            visible: !isEmptyData && (that.option("selection.allowSelectAll") || false !== selectionController.isSelectAll())
                        },
                        tabIndex: that.option("useLegacyKeyboardNavigation") ? -1 : that.option("tabIndex") || 0,
                        setValue: function(value, e) {
                            var allowSelectAll = that.option("selection.allowSelectAll");
                            e.component.option("visible", allowSelectAll || false !== e.component.option("value"));
                            if (!e.event || selectionController.isSelectAll() === value) {
                                return
                            }
                            if (e.value && !allowSelectAll) {
                                e.component.option("value", false)
                            } else {
                                e.value ? selectionController.selectAll() : selectionController.deselectAll()
                            }
                            e.event.preventDefault()
                        }
                    }));
                    return groupElement
                },
                _attachSelectAllCheckBoxClickEvent: function($element) {
                    eventsEngine.on($element, clickEventName, this.createAction((function(e) {
                        var event = e.event;
                        if (!$(event.target).closest("." + SELECT_CHECKBOX_CLASS).length) {
                            eventsEngine.trigger($(event.currentTarget).children("." + SELECT_CHECKBOX_CLASS), clickEventName)
                        }
                        event.preventDefault()
                    })))
                }
            },
            rowsView: {
                renderSelectCheckBoxContainer: function($container, options) {
                    if ("data" === options.rowType && !options.row.isNewRow) {
                        $container.addClass(EDITOR_CELL_CLASS);
                        this._attachCheckBoxClickEvent($container);
                        this.setAria("label", messageLocalization.format("dxDataGrid-ariaSelectRow"), $container);
                        this._renderSelectCheckBox($container, options)
                    } else {
                        gridCoreUtils.setEmptyText($container)
                    }
                },
                _renderSelectCheckBox: function(container, options) {
                    var groupElement = $("<div>").addClass(SELECT_CHECKBOX_CLASS).appendTo(container);
                    this.getController("editorFactory").createEditor(groupElement, extend({}, options.column, {
                        parentType: "dataRow",
                        dataType: "boolean",
                        lookup: null,
                        value: options.value,
                        setValue: function(value, e) {
                            var _e$event;
                            if ("keydown" === (null === e || void 0 === e ? void 0 : null === (_e$event = e.event) || void 0 === _e$event ? void 0 : _e$event.type)) {
                                eventsEngine.trigger(e.element, clickEventName, e)
                            }
                        },
                        row: options.row
                    }));
                    return groupElement
                },
                _attachCheckBoxClickEvent: function($element) {
                    eventsEngine.on($element, clickEventName, this.createAction((function(e) {
                        var selectionController = this.getController("selection");
                        var event = e.event;
                        var rowIndex = this.getRowIndex($(event.currentTarget).closest("." + ROW_CLASS));
                        if (rowIndex >= 0) {
                            selectionController.startSelectionWithCheckboxes();
                            selectionController.changeItemSelection(rowIndex, {
                                shift: event.shiftKey
                            });
                            if ($(event.target).closest("." + SELECT_CHECKBOX_CLASS).length) {
                                this.getController("data").updateItems({
                                    changeType: "updateSelection",
                                    itemIndexes: [rowIndex]
                                })
                            }
                        }
                    })))
                },
                _update: function(change) {
                    var that = this;
                    var tableElements = that.getTableElements();
                    if ("updateSelection" === change.changeType) {
                        if (tableElements.length > 0) {
                            each(tableElements, (function(_, tableElement) {
                                each(change.itemIndexes || [], (function(_, index) {
                                    var $row;
                                    if (change.items[index]) {
                                        $row = that._getRowElements($(tableElement)).eq(index);
                                        if ($row.length) {
                                            var isSelected = change.items[index].isSelected;
                                            $row.toggleClass(ROW_SELECTION_CLASS, void 0 === isSelected ? false : isSelected).find("." + SELECT_CHECKBOX_CLASS).dxCheckBox("option", "value", isSelected);
                                            that.setAria("selected", isSelected, $row)
                                        }
                                    }
                                }))
                            }));
                            that._updateCheckboxesClass()
                        }
                    } else {
                        that.callBase(change)
                    }
                },
                _createTable: function() {
                    var that = this;
                    var selectionMode = that.option("selection.mode");
                    var $table = that.callBase.apply(that, arguments);
                    if ("none" !== selectionMode) {
                        if ("onLongTap" === that.option(SHOW_CHECKBOXES_MODE) || !touch) {
                            eventsEngine.on($table, addNamespace(holdEvent.name, "dxDataGridRowsView"), "." + DATA_ROW_CLASS, that.createAction((function(e) {
                                processLongTap(that.component, e.event);
                                e.event.stopPropagation()
                            })))
                        }
                        eventsEngine.on($table, "mousedown selectstart", that.createAction((function(e) {
                            var event = e.event;
                            if (event.shiftKey) {
                                event.preventDefault()
                            }
                        })))
                    }
                    return $table
                },
                _createRow: function(row) {
                    var $row = this.callBase.apply(this, arguments);
                    if (row) {
                        var isSelected = !!row.isSelected;
                        if (isSelected) {
                            $row.addClass(ROW_SELECTION_CLASS)
                        }
                        this.setAria("selected", isSelected, $row)
                    }
                    return $row
                },
                _rowClick: function(e) {
                    var dxEvent = e.event;
                    var isSelectionDisabled = $(dxEvent.target).closest("." + SELECTION_DISABLED_CLASS).length;
                    if (!this.isClickableElement($(dxEvent.target))) {
                        if (!isSelectionDisabled && ("multiple" !== this.option(SELECTION_MODE) || "always" !== this.option(SHOW_CHECKBOXES_MODE))) {
                            if (this.getController("selection").changeItemSelection(e.rowIndex, {
                                    control: isCommandKeyPressed(dxEvent),
                                    shift: dxEvent.shiftKey
                                })) {
                                dxEvent.preventDefault();
                                e.handled = true
                            }
                        }
                        this.callBase(e)
                    }
                },
                isClickableElement: function($target) {
                    var isCommandSelect = $target.closest("." + COMMAND_SELECT_CLASS).length;
                    return !!isCommandSelect
                },
                _renderCore: function(change) {
                    this.callBase(change);
                    this._updateCheckboxesClass()
                },
                _updateCheckboxesClass: function() {
                    var tableElements = this.getTableElements();
                    var selectionController = this.getController("selection");
                    var isCheckBoxesHidden = selectionController.isSelectColumnVisible() && !selectionController.isSelectionWithCheckboxes();
                    each(tableElements, (function(_, tableElement) {
                        $(tableElement).toggleClass(CHECKBOXES_HIDDEN_CLASS, isCheckBoxesHidden)
                    }))
                }
            }
        }
    }
};
