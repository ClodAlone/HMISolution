/**
 * DevExtreme (esm/renovation/ui/grids/data_grid/data_grid.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["onOptionChanged"],
    _excluded2 = ["accessKey", "activeStateEnabled", "adaptColumnWidthByRatio", "allowColumnReordering", "allowColumnResizing", "autoNavigateToFocusedRow", "cacheEnabled", "cellHintEnabled", "className", "columnAutoWidth", "columnChooser", "columnFixing", "columnHidingEnabled", "columnMinWidth", "columnResizingMode", "columnWidth", "columns", "commonColumnSettings", "customizeColumns", "customizeExportData", "dataRowTemplate", "dataSource", "dateSerializationFormat", "defaultFilterValue", "defaultFocusedColumnIndex", "defaultFocusedRowIndex", "defaultFocusedRowKey", "defaultSelectedRowKeys", "defaultSelectionFilter", "disabled", "editing", "errorRowEnabled", "export", "filterBuilder", "filterBuilderPopup", "filterPanel", "filterRow", "filterSyncEnabled", "filterValue", "filterValueChange", "focusStateEnabled", "focusedColumnIndex", "focusedColumnIndexChange", "focusedRowEnabled", "focusedRowIndex", "focusedRowIndexChange", "focusedRowKey", "focusedRowKeyChange", "groupPanel", "grouping", "headerFilter", "height", "highlightChanges", "hint", "hoverStateEnabled", "keyExpr", "keyboardNavigation", "loadPanel", "loadingTimeout", "masterDetail", "noDataText", "onAdaptiveDetailRowPreparing", "onCellClick", "onCellDblClick", "onCellHoverChanged", "onCellPrepared", "onClick", "onContextMenuPreparing", "onDataErrorOccurred", "onEditCanceled", "onEditCanceling", "onEditingStart", "onEditorPrepared", "onEditorPreparing", "onExported", "onExporting", "onFileSaving", "onFocusedCellChanged", "onFocusedCellChanging", "onFocusedRowChanged", "onFocusedRowChanging", "onInitNewRow", "onKeyDown", "onRowClick", "onRowCollapsed", "onRowCollapsing", "onRowDblClick", "onRowExpanded", "onRowExpanding", "onRowInserted", "onRowInserting", "onRowPrepared", "onRowRemoved", "onRowRemoving", "onRowUpdated", "onRowUpdating", "onRowValidating", "onSaved", "onSaving", "onSelectionChanged", "onToolbarPreparing", "pager", "paging", "regenerateColumnsByVisibleItems", "remoteOperations", "renderAsync", "repaintChangesOnly", "rowAlternationEnabled", "rowDragging", "rowTemplate", "rtlEnabled", "scrolling", "searchPanel", "selectedRowKeys", "selectedRowKeysChange", "selection", "selectionFilter", "selectionFilterChange", "showBorders", "showColumnHeaders", "showColumnLines", "showRowLines", "sortByGroupSummaryInfo", "sorting", "stateStoring", "summary", "tabIndex", "toolbar", "twoWayBindingEnabled", "useKeyboard", "useLegacyColumnButtonTemplate", "useLegacyKeyboardNavigation", "visible", "width", "wordWrapEnabled"];
import {
    createComponentVNode
} from "inferno";
import {
    InfernoEffect,
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    DataGridProps
} from "./common/data_grid_props";
import "../../../../ui/data_grid/ui.data_grid";
import {
    Widget
} from "../../common/widget";
import {
    DataGridComponent
} from "../../../component_wrapper/data_grid/datagrid_component";
import {
    DataGridViews
} from "./data_grid_views";
import {
    getUpdatedOptions
} from "../../common/utils/get_updated_options";
import {
    hasWindow
} from "../../../../core/utils/window";
var aria = {
    role: "presentation"
};
var rowSelector = ".dx-row";

function normalizeProps(props) {
    var result = {};
    Object.keys(props).forEach(key => {
        if (void 0 !== props[key]) {
            result[key] = props[key]
        }
    });
    return result
}
export var viewFunction = _ref => {
    var {
        initializedInstance: initializedInstance,
        onDimensionChanged: onDimensionChanged,
        onHoverEnd: onHoverEnd,
        onHoverStart: onHoverStart,
        onVisibilityChange: onVisibilityChange,
        props: {
            accessKey: accessKey,
            activeStateEnabled: activeStateEnabled,
            className: className,
            disabled: disabled,
            focusStateEnabled: focusStateEnabled,
            height: height,
            hint: hint,
            hoverStateEnabled: hoverStateEnabled,
            rtlEnabled: rtlEnabled,
            showBorders: showBorders,
            tabIndex: tabIndex,
            visible: visible,
            width: width
        },
        restAttributes: restAttributes,
        widgetElementRef: widgetElementRef
    } = _ref;
    return normalizeProps(createComponentVNode(2, Widget, _extends({
        rootElementRef: widgetElementRef,
        accessKey: accessKey,
        activeStateEnabled: activeStateEnabled,
        activeStateUnit: rowSelector,
        aria: aria,
        className: className,
        disabled: disabled,
        focusStateEnabled: focusStateEnabled,
        height: height,
        hint: hint,
        hoverStateEnabled: hoverStateEnabled,
        rtlEnabled: rtlEnabled,
        tabIndex: tabIndex,
        visible: visible,
        width: width,
        onHoverStart: onHoverStart,
        onHoverEnd: onHoverEnd,
        onDimensionChanged: onDimensionChanged,
        onVisibilityChange: onVisibilityChange
    }, restAttributes, {
        children: createComponentVNode(2, DataGridViews, {
            instance: initializedInstance,
            showBorders: showBorders
        })
    })))
};
import {
    convertRulesToOptions
} from "../../../../core/options/utils";
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
import {
    createRef as infernoCreateRef
} from "inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class DataGrid extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.widgetElementRef = infernoCreateRef();
        this.isTwoWayPropUpdating = false;
        this.state = {
            initialized: false,
            filterValue: void 0 !== this.props.filterValue ? this.props.filterValue : this.props.defaultFilterValue,
            focusedColumnIndex: void 0 !== this.props.focusedColumnIndex ? this.props.focusedColumnIndex : this.props.defaultFocusedColumnIndex,
            focusedRowIndex: void 0 !== this.props.focusedRowIndex ? this.props.focusedRowIndex : this.props.defaultFocusedRowIndex,
            focusedRowKey: void 0 !== this.props.focusedRowKey ? this.props.focusedRowKey : this.props.defaultFocusedRowKey,
            selectedRowKeys: void 0 !== this.props.selectedRowKeys ? this.props.selectedRowKeys : this.props.defaultSelectedRowKeys,
            selectionFilter: void 0 !== this.props.selectionFilter ? this.props.selectionFilter : this.props.defaultSelectionFilter
        };
        this.getComponentInstance = this.getComponentInstance.bind(this);
        this.beginCustomLoading = this.beginCustomLoading.bind(this);
        this.byKey = this.byKey.bind(this);
        this.cancelEditData = this.cancelEditData.bind(this);
        this.cellValue = this.cellValue.bind(this);
        this.clearFilter = this.clearFilter.bind(this);
        this.clearSelection = this.clearSelection.bind(this);
        this.clearSorting = this.clearSorting.bind(this);
        this.closeEditCell = this.closeEditCell.bind(this);
        this.collapseAdaptiveDetailRow = this.collapseAdaptiveDetailRow.bind(this);
        this.columnCount = this.columnCount.bind(this);
        this.columnOption = this.columnOption.bind(this);
        this.deleteColumn = this.deleteColumn.bind(this);
        this.deleteRow = this.deleteRow.bind(this);
        this.deselectAll = this.deselectAll.bind(this);
        this.deselectRows = this.deselectRows.bind(this);
        this.editCell = this.editCell.bind(this);
        this.editRow = this.editRow.bind(this);
        this.endCustomLoading = this.endCustomLoading.bind(this);
        this.expandAdaptiveDetailRow = this.expandAdaptiveDetailRow.bind(this);
        this.filter = this.filter.bind(this);
        this.focus = this.focus.bind(this);
        this.getCellElement = this.getCellElement.bind(this);
        this.getCombinedFilter = this.getCombinedFilter.bind(this);
        this.getDataSource = this.getDataSource.bind(this);
        this.getKeyByRowIndex = this.getKeyByRowIndex.bind(this);
        this.getRowElement = this.getRowElement.bind(this);
        this.getRowIndexByKey = this.getRowIndexByKey.bind(this);
        this.getScrollable = this.getScrollable.bind(this);
        this.getVisibleColumnIndex = this.getVisibleColumnIndex.bind(this);
        this.hasEditData = this.hasEditData.bind(this);
        this.hideColumnChooser = this.hideColumnChooser.bind(this);
        this.isAdaptiveDetailRowExpanded = this.isAdaptiveDetailRowExpanded.bind(this);
        this.isRowFocused = this.isRowFocused.bind(this);
        this.isRowSelected = this.isRowSelected.bind(this);
        this.keyOf = this.keyOf.bind(this);
        this.navigateToRow = this.navigateToRow.bind(this);
        this.pageCount = this.pageCount.bind(this);
        this.pageIndex = this.pageIndex.bind(this);
        this.pageSize = this.pageSize.bind(this);
        this.refresh = this.refresh.bind(this);
        this.repaintRows = this.repaintRows.bind(this);
        this.saveEditData = this.saveEditData.bind(this);
        this.searchByText = this.searchByText.bind(this);
        this.selectAll = this.selectAll.bind(this);
        this.selectRows = this.selectRows.bind(this);
        this.selectRowsByIndexes = this.selectRowsByIndexes.bind(this);
        this.showColumnChooser = this.showColumnChooser.bind(this);
        this.undeleteRow = this.undeleteRow.bind(this);
        this.updateDimensions = this.updateDimensions.bind(this);
        this.resize = this.resize.bind(this);
        this.addColumn = this.addColumn.bind(this);
        this.addRow = this.addRow.bind(this);
        this.clearGrouping = this.clearGrouping.bind(this);
        this.collapseAll = this.collapseAll.bind(this);
        this.collapseRow = this.collapseRow.bind(this);
        this.expandAll = this.expandAll.bind(this);
        this.expandRow = this.expandRow.bind(this);
        this.exportToExcel = this.exportToExcel.bind(this);
        this.getSelectedRowKeys = this.getSelectedRowKeys.bind(this);
        this.getSelectedRowsData = this.getSelectedRowsData.bind(this);
        this.getTotalSummaryValue = this.getTotalSummaryValue.bind(this);
        this.getVisibleColumns = this.getVisibleColumns.bind(this);
        this.getVisibleRows = this.getVisibleRows.bind(this);
        this.isRowExpanded = this.isRowExpanded.bind(this);
        this.totalCount = this.totalCount.bind(this);
        this.isScrollbarVisible = this.isScrollbarVisible.bind(this);
        this.getTopVisibleRowData = this.getTopVisibleRowData.bind(this);
        this.getScrollbarWidth = this.getScrollbarWidth.bind(this);
        this.getDataProvider = this.getDataProvider.bind(this);
        this.updateOptions = this.updateOptions.bind(this);
        this.dispose = this.dispose.bind(this);
        this.setupInstance = this.setupInstance.bind(this);
        this.instanceOptionChangedHandler = this.instanceOptionChangedHandler.bind(this);
        this.updateTwoWayValue = this.updateTwoWayValue.bind(this);
        this.onHoverStart = this.onHoverStart.bind(this);
        this.onHoverEnd = this.onHoverEnd.bind(this);
        this.onDimensionChanged = this.onDimensionChanged.bind(this);
        this.onVisibilityChange = this.onVisibilityChange.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.updateOptions, [this.props]), new InfernoEffect(this.dispose, []), new InfernoEffect(this.setupInstance, []), createReRenderEffect()]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props])
    }
    updateOptions() {
        if (this.instance && this.prevProps && !this.isTwoWayPropUpdating) {
            var updatedOptions = getUpdatedOptions(this.prevProps, _extends({}, this.props, {
                filterValue: void 0 !== this.props.filterValue ? this.props.filterValue : this.state.filterValue,
                focusedColumnIndex: void 0 !== this.props.focusedColumnIndex ? this.props.focusedColumnIndex : this.state.focusedColumnIndex,
                focusedRowIndex: void 0 !== this.props.focusedRowIndex ? this.props.focusedRowIndex : this.state.focusedRowIndex,
                focusedRowKey: void 0 !== this.props.focusedRowKey ? this.props.focusedRowKey : this.state.focusedRowKey,
                selectedRowKeys: void 0 !== this.props.selectedRowKeys ? this.props.selectedRowKeys : this.state.selectedRowKeys,
                selectionFilter: void 0 !== this.props.selectionFilter ? this.props.selectionFilter : this.state.selectionFilter
            }));
            this.instance.beginUpdate();
            updatedOptions.forEach(_ref2 => {
                var {
                    path: path,
                    previousValue: previousValue,
                    value: value
                } = _ref2;
                this.instance._options.silent(path, previousValue);
                this.instance.option(path, value)
            });
            this.prevProps = _extends({}, this.props, {
                filterValue: void 0 !== this.props.filterValue ? this.props.filterValue : this.state.filterValue,
                focusedColumnIndex: void 0 !== this.props.focusedColumnIndex ? this.props.focusedColumnIndex : this.state.focusedColumnIndex,
                focusedRowIndex: void 0 !== this.props.focusedRowIndex ? this.props.focusedRowIndex : this.state.focusedRowIndex,
                focusedRowKey: void 0 !== this.props.focusedRowKey ? this.props.focusedRowKey : this.state.focusedRowKey,
                selectedRowKeys: void 0 !== this.props.selectedRowKeys ? this.props.selectedRowKeys : this.state.selectedRowKeys,
                selectionFilter: void 0 !== this.props.selectionFilter ? this.props.selectionFilter : this.state.selectionFilter
            });
            this.instance.endUpdate()
        } else {
            this.prevProps = _extends({}, this.props, {
                filterValue: void 0 !== this.props.filterValue ? this.props.filterValue : this.state.filterValue,
                focusedColumnIndex: void 0 !== this.props.focusedColumnIndex ? this.props.focusedColumnIndex : this.state.focusedColumnIndex,
                focusedRowIndex: void 0 !== this.props.focusedRowIndex ? this.props.focusedRowIndex : this.state.focusedRowIndex,
                focusedRowKey: void 0 !== this.props.focusedRowKey ? this.props.focusedRowKey : this.state.focusedRowKey,
                selectedRowKeys: void 0 !== this.props.selectedRowKeys ? this.props.selectedRowKeys : this.state.selectedRowKeys,
                selectionFilter: void 0 !== this.props.selectionFilter ? this.props.selectionFilter : this.state.selectionFilter
            })
        }
    }
    dispose() {
        return () => {
            this.instance.dispose()
        }
    }
    setupInstance() {
        var _this$widgetElementRe;
        var element = null === (_this$widgetElementRe = this.widgetElementRef) || void 0 === _this$widgetElementRe ? void 0 : _this$widgetElementRe.current;
        var restAttributes = this.restAttributes;
        var {
            onContentReady: onContentReady,
            onInitialized: _onInitialized
        } = restAttributes;
        var _this$props$filterVal = _extends({}, _extends({}, this.props, {
                filterValue: void 0 !== this.props.filterValue ? this.props.filterValue : this.state.filterValue,
                focusedColumnIndex: void 0 !== this.props.focusedColumnIndex ? this.props.focusedColumnIndex : this.state.focusedColumnIndex,
                focusedRowIndex: void 0 !== this.props.focusedRowIndex ? this.props.focusedRowIndex : this.state.focusedRowIndex,
                focusedRowKey: void 0 !== this.props.focusedRowKey ? this.props.focusedRowKey : this.state.focusedRowKey,
                selectedRowKeys: void 0 !== this.props.selectedRowKeys ? this.props.selectedRowKeys : this.state.selectedRowKeys,
                selectionFilter: void 0 !== this.props.selectionFilter ? this.props.selectionFilter : this.state.selectionFilter
            }), {
                onInitialized: e => {
                    this.instance = e.component;
                    null === _onInitialized || void 0 === _onInitialized ? void 0 : _onInitialized(e)
                },
                onContentReady: onContentReady
            }),
            restProps = _objectWithoutPropertiesLoose(_this$props$filterVal, _excluded);
        new DataGridComponent(element, normalizeProps(restProps));
        if (hasWindow()) {
            this.instance.getController("resizing").updateSize(element)
        }
        this.instance.on("optionChanged", this.instanceOptionChangedHandler.bind(this));
        this.setState(__state_argument => ({
            initialized: true
        }))
    }
    get initializedInstance() {
        return this.state.initialized ? this.instance : void 0
    }
    instanceOptionChangedHandler(e) {
        try {
            this.isTwoWayPropUpdating = true;
            this.updateTwoWayValue(e)
        } finally {
            this.isTwoWayPropUpdating = false
        }
    }
    updateTwoWayValue(e) {
        var optionValue = e.component.option(e.fullName);
        var isValueCorrect = e.value === optionValue;
        if (e.value !== e.previousValue && isValueCorrect) {
            if ("editing" === e.name && this.props.editing) {
                if ("editing.changes" === e.fullName) {
                    this.props.editing.changes = e.value
                }
                if ("editing.editRowKey" === e.fullName) {
                    this.props.editing.editRowKey = e.value
                }
                if ("editing.editColumnName" === e.fullName) {
                    this.props.editing.editColumnName = e.value
                }
            }
            if ("searchPanel.text" === e.fullName && this.props.searchPanel) {
                this.props.searchPanel.text = e.value
            }
            if ("focusedRowKey" === e.fullName) {
                var __newValue;
                this.setState(__state_argument => {
                    __newValue = e.value;
                    return {
                        focusedRowKey: __newValue
                    }
                });
                this.props.focusedRowKeyChange(__newValue)
            }
            if ("focusedRowIndex" === e.fullName) {
                var _newValue;
                this.setState(__state_argument => {
                    _newValue = e.value;
                    return {
                        focusedRowIndex: _newValue
                    }
                });
                this.props.focusedRowIndexChange(_newValue)
            }
            if ("focusedColumnIndex" === e.fullName) {
                var _newValue2;
                this.setState(__state_argument => {
                    _newValue2 = e.value;
                    return {
                        focusedColumnIndex: _newValue2
                    }
                });
                this.props.focusedColumnIndexChange(_newValue2)
            }
            if ("filterValue" === e.fullName && (void 0 !== this.props.filterValue ? this.props.filterValue : this.state.filterValue) !== e.value) {
                var _newValue3;
                this.setState(__state_argument => {
                    _newValue3 = e.value;
                    return {
                        filterValue: _newValue3
                    }
                });
                this.props.filterValueChange(_newValue3)
            }
            if ("selectedRowKeys" === e.fullName) {
                var _newValue4;
                this.setState(__state_argument => {
                    _newValue4 = e.value;
                    return {
                        selectedRowKeys: _newValue4
                    }
                });
                this.props.selectedRowKeysChange(_newValue4)
            }
            if ("selectionFilter" === e.fullName) {
                var _newValue5;
                this.setState(__state_argument => {
                    _newValue5 = e.value;
                    return {
                        selectionFilter: _newValue5
                    }
                });
                this.props.selectionFilterChange(_newValue5)
            }
        }
    }
    onHoverStart(event) {
        event.currentTarget.classList.add("dx-state-hover")
    }
    onHoverEnd(event) {
        event.currentTarget.classList.remove("dx-state-hover")
    }
    onDimensionChanged() {
        var _this$instance;
        null === (_this$instance = this.instance) || void 0 === _this$instance ? void 0 : _this$instance.updateDimensions(true)
    }
    onVisibilityChange(visible) {
        if (visible) {
            var _this$instance2;
            null === (_this$instance2 = this.instance) || void 0 === _this$instance2 ? void 0 : _this$instance2.updateDimensions()
        }
    }
    get restAttributes() {
        var _this$props$filterVal2 = _extends({}, this.props, {
                filterValue: void 0 !== this.props.filterValue ? this.props.filterValue : this.state.filterValue,
                focusedColumnIndex: void 0 !== this.props.focusedColumnIndex ? this.props.focusedColumnIndex : this.state.focusedColumnIndex,
                focusedRowIndex: void 0 !== this.props.focusedRowIndex ? this.props.focusedRowIndex : this.state.focusedRowIndex,
                focusedRowKey: void 0 !== this.props.focusedRowKey ? this.props.focusedRowKey : this.state.focusedRowKey,
                selectedRowKeys: void 0 !== this.props.selectedRowKeys ? this.props.selectedRowKeys : this.state.selectedRowKeys,
                selectionFilter: void 0 !== this.props.selectionFilter ? this.props.selectionFilter : this.state.selectionFilter
            }),
            restProps = _objectWithoutPropertiesLoose(_this$props$filterVal2, _excluded2);
        return restProps
    }
    getComponentInstance() {
        return this.instance
    }
    beginCustomLoading(messageText) {
        var _this$instance3;
        return null === (_this$instance3 = this.instance) || void 0 === _this$instance3 ? void 0 : _this$instance3.beginCustomLoading(messageText)
    }
    byKey(key) {
        var _this$instance4;
        return null === (_this$instance4 = this.instance) || void 0 === _this$instance4 ? void 0 : _this$instance4.byKey(key)
    }
    cancelEditData() {
        var _this$instance5;
        return null === (_this$instance5 = this.instance) || void 0 === _this$instance5 ? void 0 : _this$instance5.cancelEditData()
    }
    cellValue(rowIndex, dataField, value) {
        var _this$instance6;
        return null === (_this$instance6 = this.instance) || void 0 === _this$instance6 ? void 0 : _this$instance6.cellValue(rowIndex, dataField, value)
    }
    clearFilter(filterName) {
        var _this$instance7;
        return null === (_this$instance7 = this.instance) || void 0 === _this$instance7 ? void 0 : _this$instance7.clearFilter(filterName)
    }
    clearSelection() {
        var _this$instance8;
        return null === (_this$instance8 = this.instance) || void 0 === _this$instance8 ? void 0 : _this$instance8.clearSelection()
    }
    clearSorting() {
        var _this$instance9;
        return null === (_this$instance9 = this.instance) || void 0 === _this$instance9 ? void 0 : _this$instance9.clearSorting()
    }
    closeEditCell() {
        var _this$instance10;
        return null === (_this$instance10 = this.instance) || void 0 === _this$instance10 ? void 0 : _this$instance10.closeEditCell()
    }
    collapseAdaptiveDetailRow() {
        var _this$instance11;
        return null === (_this$instance11 = this.instance) || void 0 === _this$instance11 ? void 0 : _this$instance11.collapseAdaptiveDetailRow()
    }
    columnCount() {
        var _this$instance12;
        return null === (_this$instance12 = this.instance) || void 0 === _this$instance12 ? void 0 : _this$instance12.columnCount()
    }
    columnOption(id, optionName, optionValue) {
        if (this.instance) {
            if (1 === arguments.length || void 0 === optionName) {
                return this.instance.columnOption(id)
            }
            if (2 === arguments.length) {
                return this.instance.columnOption(id, optionName)
            }
            return this.instance.columnOption(id, optionName, optionValue)
        }
        return null
    }
    deleteColumn(id) {
        var _this$instance13;
        return null === (_this$instance13 = this.instance) || void 0 === _this$instance13 ? void 0 : _this$instance13.deleteColumn(id)
    }
    deleteRow(rowIndex) {
        var _this$instance14;
        return null === (_this$instance14 = this.instance) || void 0 === _this$instance14 ? void 0 : _this$instance14.deleteRow(rowIndex)
    }
    deselectAll() {
        var _this$instance15;
        return null === (_this$instance15 = this.instance) || void 0 === _this$instance15 ? void 0 : _this$instance15.deselectAll()
    }
    deselectRows(keys) {
        var _this$instance16;
        return null === (_this$instance16 = this.instance) || void 0 === _this$instance16 ? void 0 : _this$instance16.deselectRows(keys)
    }
    editCell(rowIndex, dataFieldColumnIndex) {
        var _this$instance17;
        return null === (_this$instance17 = this.instance) || void 0 === _this$instance17 ? void 0 : _this$instance17.editCell(rowIndex, dataFieldColumnIndex)
    }
    editRow(rowIndex) {
        var _this$instance18;
        return null === (_this$instance18 = this.instance) || void 0 === _this$instance18 ? void 0 : _this$instance18.editRow(rowIndex)
    }
    endCustomLoading() {
        var _this$instance19;
        return null === (_this$instance19 = this.instance) || void 0 === _this$instance19 ? void 0 : _this$instance19.endCustomLoading()
    }
    expandAdaptiveDetailRow(key) {
        var _this$instance20;
        return null === (_this$instance20 = this.instance) || void 0 === _this$instance20 ? void 0 : _this$instance20.expandAdaptiveDetailRow(key)
    }
    filter(filterExpr) {
        var _this$instance21;
        return null === (_this$instance21 = this.instance) || void 0 === _this$instance21 ? void 0 : _this$instance21.filter(filterExpr)
    }
    focus(element) {
        var _this$instance22;
        return null === (_this$instance22 = this.instance) || void 0 === _this$instance22 ? void 0 : _this$instance22.focus(element)
    }
    getCellElement(rowIndex, dataField) {
        var _this$instance23;
        return null === (_this$instance23 = this.instance) || void 0 === _this$instance23 ? void 0 : _this$instance23.getCellElement(rowIndex, dataField)
    }
    getCombinedFilter(returnDataField) {
        var _this$instance24;
        return null === (_this$instance24 = this.instance) || void 0 === _this$instance24 ? void 0 : _this$instance24.getCombinedFilter(returnDataField)
    }
    getDataSource() {
        var _this$instance25;
        return null === (_this$instance25 = this.instance) || void 0 === _this$instance25 ? void 0 : _this$instance25.getDataSource()
    }
    getKeyByRowIndex(rowIndex) {
        var _this$instance26;
        return null === (_this$instance26 = this.instance) || void 0 === _this$instance26 ? void 0 : _this$instance26.getKeyByRowIndex(rowIndex)
    }
    getRowElement(rowIndex) {
        var _this$instance27;
        return null === (_this$instance27 = this.instance) || void 0 === _this$instance27 ? void 0 : _this$instance27.getRowElement(rowIndex)
    }
    getRowIndexByKey(key) {
        var _this$instance28;
        return null === (_this$instance28 = this.instance) || void 0 === _this$instance28 ? void 0 : _this$instance28.getRowIndexByKey(key)
    }
    getScrollable() {
        var _this$instance29;
        return null === (_this$instance29 = this.instance) || void 0 === _this$instance29 ? void 0 : _this$instance29.getScrollable()
    }
    getVisibleColumnIndex(id) {
        var _this$instance30;
        return null === (_this$instance30 = this.instance) || void 0 === _this$instance30 ? void 0 : _this$instance30.getVisibleColumnIndex(id)
    }
    hasEditData() {
        var _this$instance31;
        return null === (_this$instance31 = this.instance) || void 0 === _this$instance31 ? void 0 : _this$instance31.hasEditData()
    }
    hideColumnChooser() {
        var _this$instance32;
        return null === (_this$instance32 = this.instance) || void 0 === _this$instance32 ? void 0 : _this$instance32.hideColumnChooser()
    }
    isAdaptiveDetailRowExpanded(key) {
        var _this$instance33;
        return null === (_this$instance33 = this.instance) || void 0 === _this$instance33 ? void 0 : _this$instance33.isAdaptiveDetailRowExpanded(key)
    }
    isRowFocused(key) {
        var _this$instance34;
        return null === (_this$instance34 = this.instance) || void 0 === _this$instance34 ? void 0 : _this$instance34.isRowFocused(key)
    }
    isRowSelected(key) {
        var _this$instance35;
        return null === (_this$instance35 = this.instance) || void 0 === _this$instance35 ? void 0 : _this$instance35.isRowSelected(key)
    }
    keyOf(obj) {
        var _this$instance36;
        return null === (_this$instance36 = this.instance) || void 0 === _this$instance36 ? void 0 : _this$instance36.keyOf(obj)
    }
    navigateToRow(key) {
        var _this$instance37;
        return null === (_this$instance37 = this.instance) || void 0 === _this$instance37 ? void 0 : _this$instance37.navigateToRow(key)
    }
    pageCount() {
        var _this$instance38;
        return null === (_this$instance38 = this.instance) || void 0 === _this$instance38 ? void 0 : _this$instance38.pageCount()
    }
    pageIndex(newIndex) {
        var _this$instance39;
        return null === (_this$instance39 = this.instance) || void 0 === _this$instance39 ? void 0 : _this$instance39.pageIndex(newIndex)
    }
    pageSize(value) {
        var _this$instance40;
        return null === (_this$instance40 = this.instance) || void 0 === _this$instance40 ? void 0 : _this$instance40.pageSize(value)
    }
    refresh(changesOnly) {
        var _this$instance41;
        return null === (_this$instance41 = this.instance) || void 0 === _this$instance41 ? void 0 : _this$instance41.refresh(changesOnly)
    }
    repaintRows(rowIndexes) {
        var _this$instance42;
        return null === (_this$instance42 = this.instance) || void 0 === _this$instance42 ? void 0 : _this$instance42.repaintRows(rowIndexes)
    }
    saveEditData() {
        var _this$instance43;
        return null === (_this$instance43 = this.instance) || void 0 === _this$instance43 ? void 0 : _this$instance43.saveEditData()
    }
    searchByText(text) {
        var _this$instance44;
        return null === (_this$instance44 = this.instance) || void 0 === _this$instance44 ? void 0 : _this$instance44.searchByText(text)
    }
    selectAll() {
        var _this$instance45;
        return null === (_this$instance45 = this.instance) || void 0 === _this$instance45 ? void 0 : _this$instance45.selectAll()
    }
    selectRows(keys, preserve) {
        var _this$instance46;
        return null === (_this$instance46 = this.instance) || void 0 === _this$instance46 ? void 0 : _this$instance46.selectRows(keys, preserve)
    }
    selectRowsByIndexes(indexes) {
        var _this$instance47;
        return null === (_this$instance47 = this.instance) || void 0 === _this$instance47 ? void 0 : _this$instance47.selectRowsByIndexes(indexes)
    }
    showColumnChooser() {
        var _this$instance48;
        return null === (_this$instance48 = this.instance) || void 0 === _this$instance48 ? void 0 : _this$instance48.showColumnChooser()
    }
    undeleteRow(rowIndex) {
        var _this$instance49;
        return null === (_this$instance49 = this.instance) || void 0 === _this$instance49 ? void 0 : _this$instance49.undeleteRow(rowIndex)
    }
    updateDimensions() {
        var _this$instance50;
        return null === (_this$instance50 = this.instance) || void 0 === _this$instance50 ? void 0 : _this$instance50.updateDimensions()
    }
    resize() {
        var _this$instance51;
        return null === (_this$instance51 = this.instance) || void 0 === _this$instance51 ? void 0 : _this$instance51.resize()
    }
    addColumn(columnOptions) {
        var _this$instance52;
        return null === (_this$instance52 = this.instance) || void 0 === _this$instance52 ? void 0 : _this$instance52.addColumn(columnOptions)
    }
    addRow() {
        var _this$instance53;
        return null === (_this$instance53 = this.instance) || void 0 === _this$instance53 ? void 0 : _this$instance53.addRow()
    }
    clearGrouping() {
        var _this$instance54;
        return null === (_this$instance54 = this.instance) || void 0 === _this$instance54 ? void 0 : _this$instance54.clearGrouping()
    }
    collapseAll(groupIndex) {
        var _this$instance55;
        return null === (_this$instance55 = this.instance) || void 0 === _this$instance55 ? void 0 : _this$instance55.collapseAll(groupIndex)
    }
    collapseRow(key) {
        var _this$instance56;
        return null === (_this$instance56 = this.instance) || void 0 === _this$instance56 ? void 0 : _this$instance56.collapseRow(key)
    }
    expandAll(groupIndex) {
        var _this$instance57;
        return null === (_this$instance57 = this.instance) || void 0 === _this$instance57 ? void 0 : _this$instance57.expandAll(groupIndex)
    }
    expandRow(key) {
        var _this$instance58;
        return null === (_this$instance58 = this.instance) || void 0 === _this$instance58 ? void 0 : _this$instance58.expandRow(key)
    }
    exportToExcel(selectionOnly) {
        var _this$instance59;
        return null === (_this$instance59 = this.instance) || void 0 === _this$instance59 ? void 0 : _this$instance59.exportToExcel(selectionOnly)
    }
    getSelectedRowKeys() {
        var _this$instance60;
        return null === (_this$instance60 = this.instance) || void 0 === _this$instance60 ? void 0 : _this$instance60.getSelectedRowKeys()
    }
    getSelectedRowsData() {
        var _this$instance61;
        return null === (_this$instance61 = this.instance) || void 0 === _this$instance61 ? void 0 : _this$instance61.getSelectedRowsData()
    }
    getTotalSummaryValue(summaryItemName) {
        var _this$instance62;
        return null === (_this$instance62 = this.instance) || void 0 === _this$instance62 ? void 0 : _this$instance62.getTotalSummaryValue(summaryItemName)
    }
    getVisibleColumns(headerLevel) {
        var _this$instance63;
        return null === (_this$instance63 = this.instance) || void 0 === _this$instance63 ? void 0 : _this$instance63.getVisibleColumns(headerLevel)
    }
    getVisibleRows() {
        var _this$instance64;
        return null === (_this$instance64 = this.instance) || void 0 === _this$instance64 ? void 0 : _this$instance64.getVisibleRows()
    }
    isRowExpanded(key) {
        var _this$instance65;
        return null === (_this$instance65 = this.instance) || void 0 === _this$instance65 ? void 0 : _this$instance65.isRowExpanded(key)
    }
    totalCount() {
        var _this$instance66;
        return null === (_this$instance66 = this.instance) || void 0 === _this$instance66 ? void 0 : _this$instance66.totalCount()
    }
    isScrollbarVisible() {
        var _this$instance67;
        return null === (_this$instance67 = this.instance) || void 0 === _this$instance67 ? void 0 : _this$instance67.isScrollbarVisible()
    }
    getTopVisibleRowData() {
        var _this$instance68;
        return null === (_this$instance68 = this.instance) || void 0 === _this$instance68 ? void 0 : _this$instance68.getTopVisibleRowData()
    }
    getScrollbarWidth(isHorizontal) {
        var _this$instance69;
        return null === (_this$instance69 = this.instance) || void 0 === _this$instance69 ? void 0 : _this$instance69.getScrollbarWidth(isHorizontal)
    }
    getDataProvider(selectedRowsOnly) {
        var _this$instance70;
        return null === (_this$instance70 = this.instance) || void 0 === _this$instance70 ? void 0 : _this$instance70.getDataProvider(selectedRowsOnly)
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                filterValue: void 0 !== this.props.filterValue ? this.props.filterValue : this.state.filterValue,
                focusedColumnIndex: void 0 !== this.props.focusedColumnIndex ? this.props.focusedColumnIndex : this.state.focusedColumnIndex,
                focusedRowIndex: void 0 !== this.props.focusedRowIndex ? this.props.focusedRowIndex : this.state.focusedRowIndex,
                focusedRowKey: void 0 !== this.props.focusedRowKey ? this.props.focusedRowKey : this.state.focusedRowKey,
                selectedRowKeys: void 0 !== this.props.selectedRowKeys ? this.props.selectedRowKeys : this.state.selectedRowKeys,
                selectionFilter: void 0 !== this.props.selectionFilter ? this.props.selectionFilter : this.state.selectionFilter,
                rowTemplate: getTemplate(props.rowTemplate),
                dataRowTemplate: getTemplate(props.dataRowTemplate)
            }),
            initialized: this.state.initialized,
            widgetElementRef: this.widgetElementRef,
            initializedInstance: this.initializedInstance,
            instanceOptionChangedHandler: this.instanceOptionChangedHandler,
            updateTwoWayValue: this.updateTwoWayValue,
            onHoverStart: this.onHoverStart,
            onHoverEnd: this.onHoverEnd,
            onDimensionChanged: this.onDimensionChanged,
            onVisibilityChange: this.onVisibilityChange,
            restAttributes: this.restAttributes
        })
    }
}

function __processTwoWayProps(defaultProps) {
    var twoWayProps = ["filterValue", "focusedColumnIndex", "focusedRowIndex", "focusedRowKey", "selectedRowKeys", "selectionFilter"];
    return Object.keys(defaultProps).reduce((props, propName) => {
        var propValue = defaultProps[propName];
        var defaultPropName = twoWayProps.some(p => p === propName) ? "default" + propName.charAt(0).toUpperCase() + propName.slice(1) : propName;
        props[defaultPropName] = propValue;
        return props
    }, {})
}
DataGrid.defaultProps = DataGridProps;
var __defaultOptionRules = [];
export function defaultOptions(rule) {
    __defaultOptionRules.push(rule);
    DataGrid.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(DataGrid.defaultProps), Object.getOwnPropertyDescriptors(__processTwoWayProps(convertRulesToOptions(__defaultOptionRules)))))
}
