/**
 * DevExtreme (esm/renovation/ui/grids/data_grid/common/data_grid_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    BaseWidgetProps
} from "../../../common/base_props";
import messageLocalization from "../../../../../localization/message";
import devices from "../../../../../core/devices";
import browser from "../../../../../core/utils/browser";
import {
    isMaterial,
    current
} from "../../../../../ui/themes";
export var DataGridColumnButton = {};
export var DataGridColumnHeaderFilter = {};
export var DataGridColumnLookup = {};
export var DataGridColumn = {};
export var DataGridEditingTexts = {};
export var DataGridEditing = {
    allowAdding: false,
    allowDeleting: false,
    allowUpdating: false,
    confirmDelete: true,
    get form() {
        return {
            colCount: 2
        }
    },
    mode: "row",
    newRowPosition: "viewportTop",
    get popup() {
        return {}
    },
    refreshMode: "full",
    selectTextOnEditStart: false,
    startEditAction: "click",
    get texts() {
        return {
            editRow: messageLocalization.format("dxDataGrid-editingEditRow"),
            saveAllChanges: messageLocalization.format("dxDataGrid-editingSaveAllChanges"),
            saveRowChanges: messageLocalization.format("dxDataGrid-editingSaveRowChanges"),
            cancelAllChanges: messageLocalization.format("dxDataGrid-editingCancelAllChanges"),
            cancelRowChanges: messageLocalization.format("dxDataGrid-editingCancelRowChanges"),
            addRow: messageLocalization.format("dxDataGrid-editingAddRow"),
            deleteRow: messageLocalization.format("dxDataGrid-editingDeleteRow"),
            undeleteRow: messageLocalization.format("dxDataGrid-editingUndeleteRow"),
            confirmDeleteMessage: messageLocalization.format("dxDataGrid-editingConfirmDeleteMessage"),
            confirmDeleteTitle: "",
            validationCancelChanges: messageLocalization.format("dxDataGrid-validationCancelChanges")
        }
    },
    get useIcons() {
        return isMaterial(current())
    },
    get defaultChanges() {
        return []
    },
    changesChange: () => {},
    defaultEditRowKey: null,
    editRowKeyChange: () => {},
    defaultEditColumnName: null,
    editColumnNameChange: () => {}
};
export var DataGridScrolling = {};
export var DataGridSelection = {};
export var DataGridPaging = {};
export var DataGridSortByGroupSummaryInfoItem = {};
export var DataGridGroupPanel = {};
export var DataGridGrouping = {};
export var DataGridSummaryGroupItem = {};
export var DataGridSummaryTotalItem = {};
export var DataGridSummary = {};
export var DataGridPager = {};
export var DataGridMasterDetail = {};
export var DataGridRowDragging = {};
export var DataGridColumnChooser = {};
export var DataGridColumnFixing = {};
export var DataGridSearchPanel = {};
export var DataGridSorting = {};
export var DataGridStateStoring = {};
export var DataGridFilterPanel = {};
export var DataGridFilterRow = {};
export var DataGridHeaderFilter = {};
export var DataGridKeyboardNavigation = {};
export var DataGridLoadPanel = {};
export var DataGridExport = {};
export var DataGridCommonColumnSettings = {};
export var DataGridToolbar = {};
export var DataGridProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseWidgetProps), Object.getOwnPropertyDescriptors({
    get editing() {
        return {
            mode: "row",
            newRowPosition: "viewportTop",
            refreshMode: "full",
            allowAdding: false,
            allowUpdating: false,
            allowDeleting: false,
            useIcons: isMaterial(current()),
            selectTextOnEditStart: false,
            confirmDelete: true,
            form: {
                colCount: 2
            },
            popup: {},
            startEditAction: "click",
            editRowKey: null,
            editColumnName: null,
            changes: [],
            texts: {
                editRow: messageLocalization.format("dxDataGrid-editingEditRow"),
                saveAllChanges: messageLocalization.format("dxDataGrid-editingSaveAllChanges"),
                saveRowChanges: messageLocalization.format("dxDataGrid-editingSaveRowChanges"),
                cancelAllChanges: messageLocalization.format("dxDataGrid-editingCancelAllChanges"),
                cancelRowChanges: messageLocalization.format("dxDataGrid-editingCancelRowChanges"),
                addRow: messageLocalization.format("dxDataGrid-editingAddRow"),
                deleteRow: messageLocalization.format("dxDataGrid-editingDeleteRow"),
                undeleteRow: messageLocalization.format("dxDataGrid-editingUndeleteRow"),
                confirmDeleteMessage: messageLocalization.format("dxDataGrid-editingConfirmDeleteMessage"),
                confirmDeleteTitle: "",
                validationCancelChanges: messageLocalization.format("dxDataGrid-validationCancelChanges")
            }
        }
    },
    get export() {
        return {
            enabled: false,
            fileName: "DataGrid",
            excelFilterEnabled: false,
            allowExportSelectedData: false,
            ignoreExcelErrors: true,
            customizeExcelCell: void 0,
            texts: {
                exportTo: messageLocalization.format("dxDataGrid-exportTo"),
                exportAll: messageLocalization.format("dxDataGrid-exportAll"),
                exportSelectedRows: messageLocalization.format("dxDataGrid-exportSelectedRows")
            }
        }
    },
    get groupPanel() {
        return {
            visible: false,
            emptyPanelText: messageLocalization.format("dxDataGrid-groupPanelEmptyText"),
            allowColumnDragging: true
        }
    },
    get grouping() {
        return {
            autoExpandAll: true,
            allowCollapsing: true,
            contextMenuEnabled: false,
            expandMode: "desktop" !== devices.real().deviceType ? "rowClick" : "buttonClick",
            texts: {
                groupContinuesMessage: messageLocalization.format("dxDataGrid-groupContinuesMessage"),
                groupContinuedMessage: messageLocalization.format("dxDataGrid-groupContinuedMessage"),
                groupByThisColumn: messageLocalization.format("dxDataGrid-groupHeaderText"),
                ungroup: messageLocalization.format("dxDataGrid-ungroupHeaderText"),
                ungroupAll: messageLocalization.format("dxDataGrid-ungroupAllText")
            }
        }
    },
    get masterDetail() {
        return {
            enabled: false,
            autoExpandAll: false
        }
    },
    get scrolling() {
        return {
            timeout: 300,
            updateTimeout: 300,
            minTimeout: 0,
            renderingThreshold: 100,
            removeInvisiblePages: true,
            rowPageSize: 5,
            mode: "standard",
            preloadEnabled: false,
            rowRenderingMode: "standard",
            loadTwoPagesOnStart: false,
            columnRenderingMode: "standard",
            columnPageSize: 5,
            columnRenderingThreshold: 300,
            useNative: "auto",
            prerenderedRowChunkSize: 1,
            legacyMode: false,
            prerenderedRowCount: 1
        }
    },
    get selection() {
        return {
            mode: "none",
            showCheckBoxesMode: isMaterial(current()) ? "always" : "onClick",
            allowSelectAll: true,
            selectAllMode: "allPages",
            maxFilterLengthInRequest: 1500,
            deferred: false
        }
    },
    get summary() {
        return {
            groupItems: void 0,
            totalItems: void 0,
            calculateCustomSummary: void 0,
            skipEmptyValues: true,
            recalculateWhileEditing: false,
            texts: {
                sum: messageLocalization.format("dxDataGrid-summarySum"),
                sumOtherColumn: messageLocalization.format("dxDataGrid-summarySumOtherColumn"),
                min: messageLocalization.format("dxDataGrid-summaryMin"),
                minOtherColumn: messageLocalization.format("dxDataGrid-summaryMinOtherColumn"),
                max: messageLocalization.format("dxDataGrid-summaryMax"),
                maxOtherColumn: messageLocalization.format("dxDataGrid-summaryMaxOtherColumn"),
                avg: messageLocalization.format("dxDataGrid-summaryAvg"),
                avgOtherColumn: messageLocalization.format("dxDataGrid-summaryAvgOtherColumn"),
                count: messageLocalization.format("dxDataGrid-summaryCount")
            }
        }
    },
    get columnChooser() {
        return {
            enabled: false,
            allowSearch: false,
            searchTimeout: 500,
            mode: "dragAndDrop",
            width: 250,
            height: 260,
            title: messageLocalization.format("dxDataGrid-columnChooserTitle"),
            emptyPanelText: messageLocalization.format("dxDataGrid-columnChooserEmptyText")
        }
    },
    get columnFixing() {
        return {
            enabled: false,
            texts: {
                fix: messageLocalization.format("dxDataGrid-columnFixingFix"),
                unfix: messageLocalization.format("dxDataGrid-columnFixingUnfix"),
                leftPosition: messageLocalization.format("dxDataGrid-columnFixingLeftPosition"),
                rightPosition: messageLocalization.format("dxDataGrid-columnFixingRightPosition")
            }
        }
    },
    get filterPanel() {
        return {
            visible: false,
            filterEnabled: true,
            texts: {
                createFilter: messageLocalization.format("dxDataGrid-filterPanelCreateFilter"),
                clearFilter: messageLocalization.format("dxDataGrid-filterPanelClearFilter"),
                filterEnabledHint: messageLocalization.format("dxDataGrid-filterPanelFilterEnabledHint")
            }
        }
    },
    get filterRow() {
        return {
            visible: false,
            showOperationChooser: true,
            showAllText: messageLocalization.format("dxDataGrid-filterRowShowAllText"),
            resetOperationText: messageLocalization.format("dxDataGrid-filterRowResetOperationText"),
            applyFilter: "auto",
            applyFilterText: messageLocalization.format("dxDataGrid-applyFilterText"),
            operationDescriptions: {
                equal: messageLocalization.format("dxDataGrid-filterRowOperationEquals"),
                notEqual: messageLocalization.format("dxDataGrid-filterRowOperationNotEquals"),
                lessThan: messageLocalization.format("dxDataGrid-filterRowOperationLess"),
                lessThanOrEqual: messageLocalization.format("dxDataGrid-filterRowOperationLessOrEquals"),
                greaterThan: messageLocalization.format("dxDataGrid-filterRowOperationGreater"),
                greaterThanOrEqual: messageLocalization.format("dxDataGrid-filterRowOperationGreaterOrEquals"),
                startsWith: messageLocalization.format("dxDataGrid-filterRowOperationStartsWith"),
                contains: messageLocalization.format("dxDataGrid-filterRowOperationContains"),
                notContains: messageLocalization.format("dxDataGrid-filterRowOperationNotContains"),
                endsWith: messageLocalization.format("dxDataGrid-filterRowOperationEndsWith"),
                between: messageLocalization.format("dxDataGrid-filterRowOperationBetween"),
                isBlank: messageLocalization.format("dxFilterBuilder-filterOperationIsBlank"),
                isNotBlank: messageLocalization.format("dxFilterBuilder-filterOperationIsNotBlank")
            },
            betweenStartText: messageLocalization.format("dxDataGrid-filterRowOperationBetweenStartText"),
            betweenEndText: messageLocalization.format("dxDataGrid-filterRowOperationBetweenEndText")
        }
    },
    get headerFilter() {
        return {
            visible: false,
            width: 252,
            height: isMaterial(current()) ? 315 : 325,
            allowSearch: false,
            searchTimeout: 500,
            texts: {
                emptyValue: messageLocalization.format("dxDataGrid-headerFilterEmptyValue"),
                ok: messageLocalization.format("dxDataGrid-headerFilterOK"),
                cancel: messageLocalization.format("dxDataGrid-headerFilterCancel")
            }
        }
    },
    get keyboardNavigation() {
        return {
            enabled: true,
            enterKeyAction: "startEdit",
            enterKeyDirection: "none",
            editOnKeyPress: false
        }
    },
    get loadPanel() {
        return {
            enabled: "auto",
            text: messageLocalization.format("Loading"),
            width: 200,
            height: 90,
            showIndicator: true,
            indicatorSrc: "",
            showPane: true
        }
    },
    get pager() {
        return {
            visible: "auto",
            showPageSizeSelector: false,
            allowedPageSizes: "auto"
        }
    },
    get paging() {
        return {
            enabled: true
        }
    },
    get rowDragging() {
        return {
            showDragIcons: true,
            dropFeedbackMode: "indicate",
            allowReordering: false,
            allowDropInsideItem: false
        }
    },
    get searchPanel() {
        return {
            visible: false,
            width: 160,
            placeholder: messageLocalization.format("dxDataGrid-searchPanelPlaceholder"),
            highlightSearchText: true,
            highlightCaseSensitive: false,
            text: "",
            searchVisibleColumnsOnly: false
        }
    },
    get sorting() {
        return {
            mode: "single",
            ascendingText: messageLocalization.format("dxDataGrid-sortingAscendingText"),
            descendingText: messageLocalization.format("dxDataGrid-sortingDescendingText"),
            clearText: messageLocalization.format("dxDataGrid-sortingClearText"),
            showSortIndexes: true
        }
    },
    get stateStoring() {
        return {
            enabled: false,
            type: "localStorage",
            savingTimeout: 2e3
        }
    },
    remoteOperations: "auto",
    allowColumnReordering: false,
    allowColumnResizing: false,
    autoNavigateToFocusedRow: true,
    cacheEnabled: true,
    cellHintEnabled: true,
    columnAutoWidth: false,
    columnHidingEnabled: false,
    columnResizingMode: "nextColumn",
    errorRowEnabled: true,
    get filterBuilder() {
        return {
            groupOperationDescriptions: {
                and: messageLocalization.format("dxFilterBuilder-and"),
                or: messageLocalization.format("dxFilterBuilder-or"),
                notAnd: messageLocalization.format("dxFilterBuilder-notAnd"),
                notOr: messageLocalization.format("dxFilterBuilder-notOr")
            },
            filterOperationDescriptions: {
                between: messageLocalization.format("dxFilterBuilder-filterOperationBetween"),
                equal: messageLocalization.format("dxFilterBuilder-filterOperationEquals"),
                notEqual: messageLocalization.format("dxFilterBuilder-filterOperationNotEquals"),
                lessThan: messageLocalization.format("dxFilterBuilder-filterOperationLess"),
                lessThanOrEqual: messageLocalization.format("dxFilterBuilder-filterOperationLessOrEquals"),
                greaterThan: messageLocalization.format("dxFilterBuilder-filterOperationGreater"),
                greaterThanOrEqual: messageLocalization.format("dxFilterBuilder-filterOperationGreaterOrEquals"),
                startsWith: messageLocalization.format("dxFilterBuilder-filterOperationStartsWith"),
                contains: messageLocalization.format("dxFilterBuilder-filterOperationContains"),
                notContains: messageLocalization.format("dxFilterBuilder-filterOperationNotContains"),
                endsWith: messageLocalization.format("dxFilterBuilder-filterOperationEndsWith"),
                isBlank: messageLocalization.format("dxFilterBuilder-filterOperationIsBlank"),
                isNotBlank: messageLocalization.format("dxFilterBuilder-filterOperationIsNotBlank")
            }
        }
    },
    get filterBuilderPopup() {
        return {}
    },
    filterSyncEnabled: "auto",
    focusedRowEnabled: false,
    highlightChanges: false,
    get noDataText() {
        return messageLocalization.format("dxDataGrid-noDataText")
    },
    renderAsync: false,
    repaintChangesOnly: false,
    rowAlternationEnabled: false,
    showBorders: false,
    showColumnHeaders: true,
    get showColumnLines() {
        return !isMaterial(current())
    },
    get showRowLines() {
        return "ios" === devices.real().platform || isMaterial(current())
    },
    twoWayBindingEnabled: true,
    wordWrapEnabled: false,
    get loadingTimeout() {
        return browser.webkit ? 30 : 0
    },
    get commonColumnSettings() {
        return {
            allowExporting: true,
            allowFiltering: true,
            allowHiding: true,
            allowSorting: true,
            allowEditing: true,
            encodeHtml: true,
            trueText: messageLocalization.format("dxDataGrid-trueText"),
            falseText: messageLocalization.format("dxDataGrid-falseText")
        }
    },
    adaptColumnWidthByRatio: true,
    regenerateColumnsByVisibleItems: false,
    useLegacyKeyboardNavigation: false,
    useLegacyColumnButtonTemplate: false,
    defaultFilterValue: null,
    filterValueChange: () => {},
    get defaultFocusedColumnIndex() {
        return -1
    },
    focusedColumnIndexChange: () => {},
    get defaultFocusedRowIndex() {
        return -1
    },
    focusedRowIndexChange: () => {},
    defaultFocusedRowKey: null,
    focusedRowKeyChange: () => {},
    get defaultSelectedRowKeys() {
        return []
    },
    selectedRowKeysChange: () => {},
    get defaultSelectionFilter() {
        return []
    },
    selectionFilterChange: () => {}
})));
