/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.item_list.details.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import {
    extendAttributes,
    getDisplayFileSize
} from "./ui.file_manager.common";
import {
    isString,
    isFunction,
    isDefined
} from "../../core/utils/type";
import messageLocalization from "../../localization/message";
import DataGrid from "../data_grid/ui.data_grid";
import FileManagerItemListBase from "./ui.file_manager.item_list";
import FileManagerFileActionsButton from "./ui.file_manager.file_actions_button";
import {
    Deferred
} from "../../core/utils/deferred";
var FILE_MANAGER_DETAILS_ITEM_LIST_CLASS = "dx-filemanager-details";
var FILE_MANAGER_DETAILS_ITEM_THUMBNAIL_CLASS = "dx-filemanager-details-item-thumbnail";
var FILE_MANAGER_DETAILS_ITEM_NAME_CLASS = "dx-filemanager-details-item-name";
var FILE_MANAGER_DETAILS_ITEM_NAME_WRAPPER_CLASS = "dx-filemanager-details-item-name-wrapper";
var FILE_MANAGER_DETAILS_ITEM_IS_DIRECTORY_CLASS = "dx-filemanager-details-item-is-directory";
var FILE_MANAGER_PARENT_DIRECTORY_ITEM = "dx-filemanager-parent-directory-item";
var DATA_GRID_DATA_ROW_CLASS = "dx-data-row";
var DEFAULT_COLUMN_CONFIGS = {
    thumbnail: {
        caption: "",
        calculateSortValue: "isDirectory",
        width: 36,
        alignment: "center",
        cssClass: FILE_MANAGER_DETAILS_ITEM_IS_DIRECTORY_CLASS
    },
    name: {
        caption: messageLocalization.format("dxFileManager-listDetailsColumnCaptionName")
    },
    dateModified: {
        caption: messageLocalization.format("dxFileManager-listDetailsColumnCaptionDateModified"),
        width: 110,
        hidingPriority: 1
    },
    size: {
        caption: messageLocalization.format("dxFileManager-listDetailsColumnCaptionFileSize"),
        width: 90,
        alignment: "right",
        hidingPriority: 0
    },
    isParentFolder: {
        caption: "isParentFolder",
        visible: false,
        sortIndex: 0,
        sortOrder: "asc"
    }
};
class FileManagerDetailsItemList extends FileManagerItemListBase {
    _initMarkup() {
        this._itemCount = 0;
        this._focusedItem = null;
        this._hasParentDirectoryItem = false;
        this._parentDirectoryItemKey = null;
        this._selectAllCheckBox = null;
        this._selectAllCheckBoxUpdating = false;
        this.$element().addClass(FILE_MANAGER_DETAILS_ITEM_LIST_CLASS);
        this._createFilesView();
        this._contextMenu.option("onContextMenuHidden", () => this._onContextMenuHidden());
        super._initMarkup()
    }
    _createFilesView() {
        var $filesView = $("<div>").appendTo(this.$element());
        var selectionMode = this._isMultipleSelectionMode() ? "multiple" : "none";
        this._filesView = this._createComponent($filesView, DataGrid, {
            dataSource: this._createDataSource(),
            hoverStateEnabled: true,
            selection: {
                mode: selectionMode,
                showCheckBoxesMode: this._isDesktop() ? "onClick" : "none"
            },
            selectedRowKeys: this.option("selectedItemKeys"),
            focusedRowKey: this.option("focusedItemKey"),
            focusedRowEnabled: true,
            allowColumnResizing: true,
            scrolling: {
                mode: "virtual"
            },
            sorting: {
                mode: "single",
                showSortIndexes: false
            },
            showColumnLines: false,
            showRowLines: false,
            columnHidingEnabled: false,
            columns: this._createColumns(),
            onEditorPreparing: this._onEditorPreparing.bind(this),
            onRowPrepared: this._onRowPrepared.bind(this),
            onContextMenuPreparing: this._onContextMenuPreparing.bind(this),
            onSelectionChanged: this._onFilesViewSelectionChanged.bind(this),
            onFocusedRowChanged: this._onFilesViewFocusedRowChanged.bind(this),
            onOptionChanged: this._onFilesViewOptionChanged.bind(this),
            onContentReady: () => {
                var _this$_refreshDeferre;
                return null === (_this$_refreshDeferre = this._refreshDeferred) || void 0 === _this$_refreshDeferre ? void 0 : _this$_refreshDeferre.resolve()
            }
        })
    }
    _createColumns() {
        var columns = this.option("detailColumns");
        columns = columns.slice(0);
        columns = columns.map(column => {
            var extendedItem = column;
            if (isString(column)) {
                extendedItem = {
                    dataField: column
                }
            }
            return this._getPreparedColumn(extendedItem)
        });
        var customizeDetailColumns = this.option("customizeDetailColumns");
        if (isFunction(customizeDetailColumns)) {
            columns = customizeDetailColumns(columns)
        }
        columns.push(this._getPreparedColumn({
            dataField: "isParentFolder"
        }));
        columns.forEach(column => this._updateColumnDataField(column));
        return columns
    }
    _getPreparedColumn(columnOptions) {
        var result = {};
        var resultCssClass = "";
        if (this._isDefaultColumn(columnOptions.dataField)) {
            var defaultConfig = extend(true, {}, DEFAULT_COLUMN_CONFIGS[columnOptions.dataField]);
            resultCssClass = defaultConfig.cssClass || "";
            switch (columnOptions.dataField) {
                case "thumbnail":
                    defaultConfig.cellTemplate = this._createThumbnailColumnCell.bind(this);
                    defaultConfig.calculateSortValue = "fileItem.".concat(defaultConfig.calculateSortValue);
                    break;
                case "name":
                    defaultConfig.cellTemplate = this._createNameColumnCell.bind(this);
                    defaultConfig.caption = messageLocalization.format("dxFileManager-listDetailsColumnCaptionName");
                    break;
                case "size":
                    defaultConfig.calculateCellValue = this._calculateSizeColumnCellValue.bind(this);
                    defaultConfig.caption = messageLocalization.format("dxFileManager-listDetailsColumnCaptionFileSize");
                    defaultConfig.calculateSortValue = rowData => rowData.fileItem.isDirectory ? -1 : rowData.fileItem.size;
                    break;
                case "dateModified":
                    defaultConfig.caption = messageLocalization.format("dxFileManager-listDetailsColumnCaptionDateModified")
            }
            extend(true, result, defaultConfig)
        }
        extendAttributes(result, columnOptions, ["alignment", "caption", "dataField", "dataType", "hidingPriority", "sortIndex", "sortOrder", "visible", "visibleIndex", "width"]);
        if (columnOptions.cssClass) {
            resultCssClass = "".concat(resultCssClass, " ").concat(columnOptions.cssClass)
        }
        if (resultCssClass) {
            result.cssClass = resultCssClass
        }
        return result
    }
    _updateColumnDataField(column) {
        var dataItemSuffix = this._isDefaultColumn(column.dataField) ? "" : "dataItem.";
        column.dataField = "fileItem." + dataItemSuffix + column.dataField;
        return column
    }
    _isDefaultColumn(columnDataField) {
        return !!DEFAULT_COLUMN_CONFIGS[columnDataField]
    }
    _onFileItemActionButtonClick(_ref) {
        var {
            component: component,
            element: element,
            event: event
        } = _ref;
        event.stopPropagation();
        var $row = component.$element().closest(this._getItemSelector());
        var fileItemInfo = $row.data("item");
        this._selectItem(fileItemInfo);
        var target = {
            itemData: fileItemInfo,
            itemElement: $row,
            isActionButton: true
        };
        var items = this._getFileItemsForContextMenu(fileItemInfo);
        this._showContextMenu(items, element, event, target);
        this._activeFileActionsButton = component;
        this._activeFileActionsButton.setActive(true)
    }
    _onContextMenuHidden() {
        if (this._activeFileActionsButton) {
            this._activeFileActionsButton.setActive(false)
        }
    }
    _getItemThumbnailCssClass() {
        return FILE_MANAGER_DETAILS_ITEM_THUMBNAIL_CLASS
    }
    _getItemSelector() {
        return ".".concat(DATA_GRID_DATA_ROW_CLASS)
    }
    _onItemDblClick(e) {
        var $row = $(e.currentTarget);
        var fileItemInfo = $row.data("item");
        this._raiseSelectedItemOpened(fileItemInfo)
    }
    _isAllItemsSelected() {
        var selectableItemsCount = this._hasParentDirectoryItem ? this._itemCount - 1 : this._itemCount;
        var selectedRowKeys = this._filesView.option("selectedRowKeys");
        if (!selectedRowKeys.length) {
            return false
        }
        return selectedRowKeys.length >= selectableItemsCount ? true : void 0
    }
    _onEditorPreparing(_ref2) {
        var {
            component: component,
            command: command,
            row: row,
            parentType: parentType,
            editorOptions: editorOptions
        } = _ref2;
        if (!this._filesView) {
            this._filesView = component
        }
        if ("select" === command && row) {
            if (this._isParentDirectoryItem(row.data)) {
                editorOptions.disabled = true
            }
        } else if ("headerRow" === parentType) {
            editorOptions.onInitialized = _ref3 => {
                var {
                    component: component
                } = _ref3;
                this._selectAllCheckBox = component
            };
            editorOptions.value = this._isAllItemsSelected();
            editorOptions.onValueChanged = args => this._onSelectAllCheckBoxValueChanged(args)
        }
    }
    _onSelectAllCheckBoxValueChanged(_ref4) {
        var {
            event: event,
            previousValue: previousValue,
            value: value
        } = _ref4;
        if (!event) {
            if (previousValue && !this._selectAllCheckBoxUpdating && this._selectAllCheckBox) {
                this._selectAllCheckBox.option("value", previousValue)
            }
            return
        }
        if (this._isAllItemsSelected() === value) {
            return
        }
        if (value) {
            this._filesView.selectAll()
        } else {
            this._filesView.deselectAll()
        }
        event.preventDefault()
    }
    _onRowPrepared(_ref5) {
        var {
            rowType: rowType,
            rowElement: rowElement,
            data: data
        } = _ref5;
        if ("data" === rowType) {
            var $row = $(rowElement);
            $row.data("item", data);
            if (this._isParentDirectoryItem(data)) {
                $row.addClass(FILE_MANAGER_PARENT_DIRECTORY_ITEM)
            }
        }
    }
    _onContextMenuPreparing(e) {
        if (!this._isDesktop()) {
            return
        }
        var fileItems = null;
        var item = {};
        if (e.row && "data" === e.row.rowType) {
            item = e.row.data;
            this._selectItem(item);
            fileItems = this._getFileItemsForContextMenu(item)
        }
        var eventArgs = extend({}, {
            targetElement: "content" === e.target && isDefined(e.row) ? this._filesView.getRowElement(e.rowIndex) : void 0,
            itemData: item,
            options: this._contextMenu.option(),
            event: e.event,
            isActionButton: false,
            cancel: false
        });
        this._raiseContextMenuShowing(eventArgs);
        e.items = eventArgs.cancel ? [] : this._contextMenu.createContextMenuItems(fileItems, null, item)
    }
    _onFilesViewSelectionChanged(_ref6) {
        var {
            component: component,
            selectedRowsData: selectedRowsData,
            selectedRowKeys: selectedRowKeys,
            currentSelectedRowKeys: currentSelectedRowKeys,
            currentDeselectedRowKeys: currentDeselectedRowKeys
        } = _ref6;
        this._filesView = this._filesView || component;
        if (this._selectAllCheckBox) {
            this._selectAllCheckBoxUpdating = true;
            this._selectAllCheckBox.option("value", this._isAllItemsSelected());
            this._selectAllCheckBoxUpdating = false
        }
        var selectedItems = selectedRowsData.map(itemInfo => itemInfo.fileItem);
        this._tryRaiseSelectionChanged({
            selectedItemInfos: selectedRowsData,
            selectedItems: selectedItems,
            selectedItemKeys: selectedRowKeys,
            currentSelectedItemKeys: currentSelectedRowKeys,
            currentDeselectedItemKeys: currentDeselectedRowKeys
        })
    }
    _onFilesViewFocusedRowChanged(e) {
        var _e$row2;
        if (!this._isMultipleSelectionMode()) {
            var _e$row;
            this._selectItemSingleSelection(null === (_e$row = e.row) || void 0 === _e$row ? void 0 : _e$row.data)
        }
        var fileSystemItem = (null === (_e$row2 = e.row) || void 0 === _e$row2 ? void 0 : _e$row2.data.fileItem) || null;
        this._onFocusedItemChanged({
            item: fileSystemItem,
            itemKey: null === fileSystemItem || void 0 === fileSystemItem ? void 0 : fileSystemItem.key,
            itemElement: e.rowElement
        })
    }
    _onFilesViewOptionChanged(_ref7) {
        var {
            fullName: fullName
        } = _ref7;
        if (fullName.indexOf("sortOrder") > -1) {
            this._filesView.columnOption("isParentFolder", {
                sortOrder: "asc",
                sortIndex: 0
            })
        }
    }
    _resetFocus() {
        this._setFocusedItemKey(void 0)
    }
    _createThumbnailColumnCell(container, cellInfo) {
        this._getItemThumbnailContainer(cellInfo.data).appendTo(container)
    }
    _createNameColumnCell(container, cellInfo) {
        var $button = $("<div>");
        var $name = $("<span>").text(cellInfo.data.fileItem.name).addClass(FILE_MANAGER_DETAILS_ITEM_NAME_CLASS);
        var $wrapper = $("<div>").append($name, $button).addClass(FILE_MANAGER_DETAILS_ITEM_NAME_WRAPPER_CLASS);
        $(container).append($wrapper);
        this._createComponent($button, FileManagerFileActionsButton, {
            onClick: e => this._onFileItemActionButtonClick(e)
        })
    }
    _calculateSizeColumnCellValue(rowData) {
        return rowData.fileItem.isDirectory ? "" : getDisplayFileSize(rowData.fileItem.size)
    }
    _selectItem(fileItemInfo) {
        var selectItemFunc = this._isMultipleSelectionMode() ? this._selectItemMultipleSelection : this._selectItemSingleSelection;
        selectItemFunc.call(this, fileItemInfo)
    }
    _deselectItem(item) {
        this._filesView.deselectRows([item.fileItem.key])
    }
    _selectItemSingleSelection(fileItemInfo) {
        if (!this._focusedItem || !fileItemInfo || this._focusedItem.fileItem.key !== fileItemInfo.fileItem.key) {
            var oldFocusedItem = this._focusedItem;
            this._focusedItem = fileItemInfo;
            var deselectedKeys = [];
            if (oldFocusedItem) {
                deselectedKeys.push(oldFocusedItem.fileItem.key)
            }
            var selectedItems = [];
            var selectedKeys = [];
            if (fileItemInfo && !this._isParentDirectoryItem(fileItemInfo)) {
                selectedItems.push(fileItemInfo.fileItem);
                selectedKeys.push(fileItemInfo.fileItem.key)
            }
            this._raiseSelectionChanged({
                selectedItems: selectedItems,
                selectedItemKeys: selectedKeys,
                currentSelectedItemKeys: [...selectedKeys],
                currentDeselectedItemKeys: deselectedKeys
            })
        }
    }
    _selectItemMultipleSelection(_ref8) {
        var {
            fileItem: fileItem
        } = _ref8;
        if (!this._filesView.isRowSelected(fileItem.key)) {
            var selectionController = this._filesView.getController("selection");
            var preserve = selectionController.isSelectionWithCheckboxes();
            this._filesView.selectRows([fileItem.key], preserve)
        }
    }
    _setSelectedItemKeys(itemKeys) {
        this._filesView.option("selectedRowKeys", itemKeys)
    }
    _setFocusedItemKey(itemKey) {
        var _this$_filesView;
        null === (_this$_filesView = this._filesView) || void 0 === _this$_filesView ? void 0 : _this$_filesView.option("focusedRowKey", itemKey)
    }
    clearSelection() {
        if (this._isMultipleSelectionMode()) {
            this._filesView.clearSelection()
        } else {
            this._filesView.option("focusedRowIndex", -1)
        }
    }
    refresh(options) {
        var actualOptions = {
            dataSource: this._createDataSource()
        };
        if (options && Object.prototype.hasOwnProperty.call(options, "focusedItemKey")) {
            if (isDefined(options.focusedItemKey)) {
                actualOptions.focusedRowKey = options.focusedItemKey
            } else {
                actualOptions.focusedRowIndex = -1
            }
        }
        this._filesView.option(actualOptions);
        this._refreshDeferred = new Deferred;
        return this._refreshDeferred.promise()
    }
    getSelectedItems() {
        if (this._isMultipleSelectionMode()) {
            return this._filesView.getSelectedRowsData()
        }
        return this._focusedItem && !this._isParentDirectoryItem(this._focusedItem) ? [this._focusedItem] : []
    }
}
export default FileManagerDetailsItemList;
