/**
 * DevExtreme (renovation/ui/grids/data_grid/common/data_grid_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.DataGridToolbar = exports.DataGridSummaryTotalItem = exports.DataGridSummaryGroupItem = exports.DataGridSummary = exports.DataGridStateStoring = exports.DataGridSorting = exports.DataGridSortByGroupSummaryInfoItem = exports.DataGridSelection = exports.DataGridSearchPanel = exports.DataGridScrolling = exports.DataGridRowDragging = exports.DataGridProps = exports.DataGridPaging = exports.DataGridPager = exports.DataGridMasterDetail = exports.DataGridLoadPanel = exports.DataGridKeyboardNavigation = exports.DataGridHeaderFilter = exports.DataGridGrouping = exports.DataGridGroupPanel = exports.DataGridFilterRow = exports.DataGridFilterPanel = exports.DataGridExport = exports.DataGridEditingTexts = exports.DataGridEditing = exports.DataGridCommonColumnSettings = exports.DataGridColumnLookup = exports.DataGridColumnHeaderFilter = exports.DataGridColumnFixing = exports.DataGridColumnChooser = exports.DataGridColumnButton = exports.DataGridColumn = void 0;
var _base_props = require("../../../common/base_props");
var _message = _interopRequireDefault(require("../../../../../localization/message"));
var _devices = _interopRequireDefault(require("../../../../../core/devices"));
var _browser = _interopRequireDefault(require("../../../../../core/utils/browser"));
var _themes = require("../../../../../ui/themes");

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
var DataGridColumnButton = {};
exports.DataGridColumnButton = DataGridColumnButton;
var DataGridColumnHeaderFilter = {};
exports.DataGridColumnHeaderFilter = DataGridColumnHeaderFilter;
var DataGridColumnLookup = {};
exports.DataGridColumnLookup = DataGridColumnLookup;
var DataGridColumn = {};
exports.DataGridColumn = DataGridColumn;
var DataGridEditingTexts = {};
exports.DataGridEditingTexts = DataGridEditingTexts;
var DataGridEditing = Object.defineProperties({
    allowAdding: false,
    allowDeleting: false,
    allowUpdating: false,
    confirmDelete: true,
    mode: "row",
    newRowPosition: "viewportTop",
    refreshMode: "full",
    selectTextOnEditStart: false,
    startEditAction: "click",
    changesChange: function() {},
    defaultEditRowKey: null,
    editRowKeyChange: function() {},
    defaultEditColumnName: null,
    editColumnNameChange: function() {}
}, {
    form: {
        get: function() {
            return {
                colCount: 2
            }
        },
        configurable: true,
        enumerable: true
    },
    popup: {
        get: function() {
            return {}
        },
        configurable: true,
        enumerable: true
    },
    texts: {
        get: function() {
            return {
                editRow: _message.default.format("dxDataGrid-editingEditRow"),
                saveAllChanges: _message.default.format("dxDataGrid-editingSaveAllChanges"),
                saveRowChanges: _message.default.format("dxDataGrid-editingSaveRowChanges"),
                cancelAllChanges: _message.default.format("dxDataGrid-editingCancelAllChanges"),
                cancelRowChanges: _message.default.format("dxDataGrid-editingCancelRowChanges"),
                addRow: _message.default.format("dxDataGrid-editingAddRow"),
                deleteRow: _message.default.format("dxDataGrid-editingDeleteRow"),
                undeleteRow: _message.default.format("dxDataGrid-editingUndeleteRow"),
                confirmDeleteMessage: _message.default.format("dxDataGrid-editingConfirmDeleteMessage"),
                confirmDeleteTitle: "",
                validationCancelChanges: _message.default.format("dxDataGrid-validationCancelChanges")
            }
        },
        configurable: true,
        enumerable: true
    },
    useIcons: {
        get: function() {
            return (0, _themes.isMaterial)((0, _themes.current)())
        },
        configurable: true,
        enumerable: true
    },
    defaultChanges: {
        get: function() {
            return []
        },
        configurable: true,
        enumerable: true
    }
});
exports.DataGridEditing = DataGridEditing;
var DataGridScrolling = {};
exports.DataGridScrolling = DataGridScrolling;
var DataGridSelection = {};
exports.DataGridSelection = DataGridSelection;
var DataGridPaging = {};
exports.DataGridPaging = DataGridPaging;
var DataGridSortByGroupSummaryInfoItem = {};
exports.DataGridSortByGroupSummaryInfoItem = DataGridSortByGroupSummaryInfoItem;
var DataGridGroupPanel = {};
exports.DataGridGroupPanel = DataGridGroupPanel;
var DataGridGrouping = {};
exports.DataGridGrouping = DataGridGrouping;
var DataGridSummaryGroupItem = {};
exports.DataGridSummaryGroupItem = DataGridSummaryGroupItem;
var DataGridSummaryTotalItem = {};
exports.DataGridSummaryTotalItem = DataGridSummaryTotalItem;
var DataGridSummary = {};
exports.DataGridSummary = DataGridSummary;
var DataGridPager = {};
exports.DataGridPager = DataGridPager;
var DataGridMasterDetail = {};
exports.DataGridMasterDetail = DataGridMasterDetail;
var DataGridRowDragging = {};
exports.DataGridRowDragging = DataGridRowDragging;
var DataGridColumnChooser = {};
exports.DataGridColumnChooser = DataGridColumnChooser;
var DataGridColumnFixing = {};
exports.DataGridColumnFixing = DataGridColumnFixing;
var DataGridSearchPanel = {};
exports.DataGridSearchPanel = DataGridSearchPanel;
var DataGridSorting = {};
exports.DataGridSorting = DataGridSorting;
var DataGridStateStoring = {};
exports.DataGridStateStoring = DataGridStateStoring;
var DataGridFilterPanel = {};
exports.DataGridFilterPanel = DataGridFilterPanel;
var DataGridFilterRow = {};
exports.DataGridFilterRow = DataGridFilterRow;
var DataGridHeaderFilter = {};
exports.DataGridHeaderFilter = DataGridHeaderFilter;
var DataGridKeyboardNavigation = {};
exports.DataGridKeyboardNavigation = DataGridKeyboardNavigation;
var DataGridLoadPanel = {};
exports.DataGridLoadPanel = DataGridLoadPanel;
var DataGridExport = {};
exports.DataGridExport = DataGridExport;
var DataGridCommonColumnSettings = {};
exports.DataGridCommonColumnSettings = DataGridCommonColumnSettings;
var DataGridToolbar = {};
exports.DataGridToolbar = DataGridToolbar;
var DataGridProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(_base_props.BaseWidgetProps), Object.getOwnPropertyDescriptors(Object.defineProperties({
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
    filterSyncEnabled: "auto",
    focusedRowEnabled: false,
    highlightChanges: false,
    renderAsync: false,
    repaintChangesOnly: false,
    rowAlternationEnabled: false,
    showBorders: false,
    showColumnHeaders: true,
    twoWayBindingEnabled: true,
    wordWrapEnabled: false,
    adaptColumnWidthByRatio: true,
    regenerateColumnsByVisibleItems: false,
    useLegacyKeyboardNavigation: false,
    useLegacyColumnButtonTemplate: false,
    defaultFilterValue: null,
    filterValueChange: function() {},
    focusedColumnIndexChange: function() {},
    focusedRowIndexChange: function() {},
    defaultFocusedRowKey: null,
    focusedRowKeyChange: function() {},
    selectedRowKeysChange: function() {},
    selectionFilterChange: function() {}
}, {
    editing: {
        get: function() {
            return {
                mode: "row",
                newRowPosition: "viewportTop",
                refreshMode: "full",
                allowAdding: false,
                allowUpdating: false,
                allowDeleting: false,
                useIcons: (0, _themes.isMaterial)((0, _themes.current)()),
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
                    editRow: _message.default.format("dxDataGrid-editingEditRow"),
                    saveAllChanges: _message.default.format("dxDataGrid-editingSaveAllChanges"),
                    saveRowChanges: _message.default.format("dxDataGrid-editingSaveRowChanges"),
                    cancelAllChanges: _message.default.format("dxDataGrid-editingCancelAllChanges"),
                    cancelRowChanges: _message.default.format("dxDataGrid-editingCancelRowChanges"),
                    addRow: _message.default.format("dxDataGrid-editingAddRow"),
                    deleteRow: _message.default.format("dxDataGrid-editingDeleteRow"),
                    undeleteRow: _message.default.format("dxDataGrid-editingUndeleteRow"),
                    confirmDeleteMessage: _message.default.format("dxDataGrid-editingConfirmDeleteMessage"),
                    confirmDeleteTitle: "",
                    validationCancelChanges: _message.default.format("dxDataGrid-validationCancelChanges")
                }
            }
        },
        configurable: true,
        enumerable: true
    },
    export: {
        get: function() {
            return {
                enabled: false,
                fileName: "DataGrid",
                excelFilterEnabled: false,
                allowExportSelectedData: false,
                ignoreExcelErrors: true,
                customizeExcelCell: void 0,
                texts: {
                    exportTo: _message.default.format("dxDataGrid-exportTo"),
                    exportAll: _message.default.format("dxDataGrid-exportAll"),
                    exportSelectedRows: _message.default.format("dxDataGrid-exportSelectedRows")
                }
            }
        },
        configurable: true,
        enumerable: true
    },
    groupPanel: {
        get: function() {
            return {
                visible: false,
                emptyPanelText: _message.default.format("dxDataGrid-groupPanelEmptyText"),
                allowColumnDragging: true
            }
        },
        configurable: true,
        enumerable: true
    },
    grouping: {
        get: function() {
            return {
                autoExpandAll: true,
                allowCollapsing: true,
                contextMenuEnabled: false,
                expandMode: "desktop" !== _devices.default.real().deviceType ? "rowClick" : "buttonClick",
                texts: {
                    groupContinuesMessage: _message.default.format("dxDataGrid-groupContinuesMessage"),
                    groupContinuedMessage: _message.default.format("dxDataGrid-groupContinuedMessage"),
                    groupByThisColumn: _message.default.format("dxDataGrid-groupHeaderText"),
                    ungroup: _message.default.format("dxDataGrid-ungroupHeaderText"),
                    ungroupAll: _message.default.format("dxDataGrid-ungroupAllText")
                }
            }
        },
        configurable: true,
        enumerable: true
    },
    masterDetail: {
        get: function() {
            return {
                enabled: false,
                autoExpandAll: false
            }
        },
        configurable: true,
        enumerable: true
    },
    scrolling: {
        get: function() {
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
        configurable: true,
        enumerable: true
    },
    selection: {
        get: function() {
            return {
                mode: "none",
                showCheckBoxesMode: (0, _themes.isMaterial)((0, _themes.current)()) ? "always" : "onClick",
                allowSelectAll: true,
                selectAllMode: "allPages",
                maxFilterLengthInRequest: 1500,
                deferred: false
            }
        },
        configurable: true,
        enumerable: true
    },
    summary: {
        get: function() {
            return {
                groupItems: void 0,
                totalItems: void 0,
                calculateCustomSummary: void 0,
                skipEmptyValues: true,
                recalculateWhileEditing: false,
                texts: {
                    sum: _message.default.format("dxDataGrid-summarySum"),
                    sumOtherColumn: _message.default.format("dxDataGrid-summarySumOtherColumn"),
                    min: _message.default.format("dxDataGrid-summaryMin"),
                    minOtherColumn: _message.default.format("dxDataGrid-summaryMinOtherColumn"),
                    max: _message.default.format("dxDataGrid-summaryMax"),
                    maxOtherColumn: _message.default.format("dxDataGrid-summaryMaxOtherColumn"),
                    avg: _message.default.format("dxDataGrid-summaryAvg"),
                    avgOtherColumn: _message.default.format("dxDataGrid-summaryAvgOtherColumn"),
                    count: _message.default.format("dxDataGrid-summaryCount")
                }
            }
        },
        configurable: true,
        enumerable: true
    },
    columnChooser: {
        get: function() {
            return {
                enabled: false,
                allowSearch: false,
                searchTimeout: 500,
                mode: "dragAndDrop",
                width: 250,
                height: 260,
                title: _message.default.format("dxDataGrid-columnChooserTitle"),
                emptyPanelText: _message.default.format("dxDataGrid-columnChooserEmptyText")
            }
        },
        configurable: true,
        enumerable: true
    },
    columnFixing: {
        get: function() {
            return {
                enabled: false,
                texts: {
                    fix: _message.default.format("dxDataGrid-columnFixingFix"),
                    unfix: _message.default.format("dxDataGrid-columnFixingUnfix"),
                    leftPosition: _message.default.format("dxDataGrid-columnFixingLeftPosition"),
                    rightPosition: _message.default.format("dxDataGrid-columnFixingRightPosition")
                }
            }
        },
        configurable: true,
        enumerable: true
    },
    filterPanel: {
        get: function() {
            return {
                visible: false,
                filterEnabled: true,
                texts: {
                    createFilter: _message.default.format("dxDataGrid-filterPanelCreateFilter"),
                    clearFilter: _message.default.format("dxDataGrid-filterPanelClearFilter"),
                    filterEnabledHint: _message.default.format("dxDataGrid-filterPanelFilterEnabledHint")
                }
            }
        },
        configurable: true,
        enumerable: true
    },
    filterRow: {
        get: function() {
            return {
                visible: false,
                showOperationChooser: true,
                showAllText: _message.default.format("dxDataGrid-filterRowShowAllText"),
                resetOperationText: _message.default.format("dxDataGrid-filterRowResetOperationText"),
                applyFilter: "auto",
                applyFilterText: _message.default.format("dxDataGrid-applyFilterText"),
                operationDescriptions: {
                    equal: _message.default.format("dxDataGrid-filterRowOperationEquals"),
                    notEqual: _message.default.format("dxDataGrid-filterRowOperationNotEquals"),
                    lessThan: _message.default.format("dxDataGrid-filterRowOperationLess"),
                    lessThanOrEqual: _message.default.format("dxDataGrid-filterRowOperationLessOrEquals"),
                    greaterThan: _message.default.format("dxDataGrid-filterRowOperationGreater"),
                    greaterThanOrEqual: _message.default.format("dxDataGrid-filterRowOperationGreaterOrEquals"),
                    startsWith: _message.default.format("dxDataGrid-filterRowOperationStartsWith"),
                    contains: _message.default.format("dxDataGrid-filterRowOperationContains"),
                    notContains: _message.default.format("dxDataGrid-filterRowOperationNotContains"),
                    endsWith: _message.default.format("dxDataGrid-filterRowOperationEndsWith"),
                    between: _message.default.format("dxDataGrid-filterRowOperationBetween"),
                    isBlank: _message.default.format("dxFilterBuilder-filterOperationIsBlank"),
                    isNotBlank: _message.default.format("dxFilterBuilder-filterOperationIsNotBlank")
                },
                betweenStartText: _message.default.format("dxDataGrid-filterRowOperationBetweenStartText"),
                betweenEndText: _message.default.format("dxDataGrid-filterRowOperationBetweenEndText")
            }
        },
        configurable: true,
        enumerable: true
    },
    headerFilter: {
        get: function() {
            return {
                visible: false,
                width: 252,
                height: (0, _themes.isMaterial)((0, _themes.current)()) ? 315 : 325,
                allowSearch: false,
                searchTimeout: 500,
                texts: {
                    emptyValue: _message.default.format("dxDataGrid-headerFilterEmptyValue"),
                    ok: _message.default.format("dxDataGrid-headerFilterOK"),
                    cancel: _message.default.format("dxDataGrid-headerFilterCancel")
                }
            }
        },
        configurable: true,
        enumerable: true
    },
    keyboardNavigation: {
        get: function() {
            return {
                enabled: true,
                enterKeyAction: "startEdit",
                enterKeyDirection: "none",
                editOnKeyPress: false
            }
        },
        configurable: true,
        enumerable: true
    },
    loadPanel: {
        get: function() {
            return {
                enabled: "auto",
                text: _message.default.format("Loading"),
                width: 200,
                height: 90,
                showIndicator: true,
                indicatorSrc: "",
                showPane: true
            }
        },
        configurable: true,
        enumerable: true
    },
    pager: {
        get: function() {
            return {
                visible: "auto",
                showPageSizeSelector: false,
                allowedPageSizes: "auto"
            }
        },
        configurable: true,
        enumerable: true
    },
    paging: {
        get: function() {
            return {
                enabled: true
            }
        },
        configurable: true,
        enumerable: true
    },
    rowDragging: {
        get: function() {
            return {
                showDragIcons: true,
                dropFeedbackMode: "indicate",
                allowReordering: false,
                allowDropInsideItem: false
            }
        },
        configurable: true,
        enumerable: true
    },
    searchPanel: {
        get: function() {
            return {
                visible: false,
                width: 160,
                placeholder: _message.default.format("dxDataGrid-searchPanelPlaceholder"),
                highlightSearchText: true,
                highlightCaseSensitive: false,
                text: "",
                searchVisibleColumnsOnly: false
            }
        },
        configurable: true,
        enumerable: true
    },
    sorting: {
        get: function() {
            return {
                mode: "single",
                ascendingText: _message.default.format("dxDataGrid-sortingAscendingText"),
                descendingText: _message.default.format("dxDataGrid-sortingDescendingText"),
                clearText: _message.default.format("dxDataGrid-sortingClearText"),
                showSortIndexes: true
            }
        },
        configurable: true,
        enumerable: true
    },
    stateStoring: {
        get: function() {
            return {
                enabled: false,
                type: "localStorage",
                savingTimeout: 2e3
            }
        },
        configurable: true,
        enumerable: true
    },
    filterBuilder: {
        get: function() {
            return {
                groupOperationDescriptions: {
                    and: _message.default.format("dxFilterBuilder-and"),
                    or: _message.default.format("dxFilterBuilder-or"),
                    notAnd: _message.default.format("dxFilterBuilder-notAnd"),
                    notOr: _message.default.format("dxFilterBuilder-notOr")
                },
                filterOperationDescriptions: {
                    between: _message.default.format("dxFilterBuilder-filterOperationBetween"),
                    equal: _message.default.format("dxFilterBuilder-filterOperationEquals"),
                    notEqual: _message.default.format("dxFilterBuilder-filterOperationNotEquals"),
                    lessThan: _message.default.format("dxFilterBuilder-filterOperationLess"),
                    lessThanOrEqual: _message.default.format("dxFilterBuilder-filterOperationLessOrEquals"),
                    greaterThan: _message.default.format("dxFilterBuilder-filterOperationGreater"),
                    greaterThanOrEqual: _message.default.format("dxFilterBuilder-filterOperationGreaterOrEquals"),
                    startsWith: _message.default.format("dxFilterBuilder-filterOperationStartsWith"),
                    contains: _message.default.format("dxFilterBuilder-filterOperationContains"),
                    notContains: _message.default.format("dxFilterBuilder-filterOperationNotContains"),
                    endsWith: _message.default.format("dxFilterBuilder-filterOperationEndsWith"),
                    isBlank: _message.default.format("dxFilterBuilder-filterOperationIsBlank"),
                    isNotBlank: _message.default.format("dxFilterBuilder-filterOperationIsNotBlank")
                }
            }
        },
        configurable: true,
        enumerable: true
    },
    filterBuilderPopup: {
        get: function() {
            return {}
        },
        configurable: true,
        enumerable: true
    },
    noDataText: {
        get: function() {
            return _message.default.format("dxDataGrid-noDataText")
        },
        configurable: true,
        enumerable: true
    },
    showColumnLines: {
        get: function() {
            return !(0, _themes.isMaterial)((0, _themes.current)())
        },
        configurable: true,
        enumerable: true
    },
    showRowLines: {
        get: function() {
            return "ios" === _devices.default.real().platform || (0, _themes.isMaterial)((0, _themes.current)())
        },
        configurable: true,
        enumerable: true
    },
    loadingTimeout: {
        get: function() {
            return _browser.default.webkit ? 30 : 0
        },
        configurable: true,
        enumerable: true
    },
    commonColumnSettings: {
        get: function() {
            return {
                allowExporting: true,
                allowFiltering: true,
                allowHiding: true,
                allowSorting: true,
                allowEditing: true,
                encodeHtml: true,
                trueText: _message.default.format("dxDataGrid-trueText"),
                falseText: _message.default.format("dxDataGrid-falseText")
            }
        },
        configurable: true,
        enumerable: true
    },
    defaultFocusedColumnIndex: {
        get: function() {
            return -1
        },
        configurable: true,
        enumerable: true
    },
    defaultFocusedRowIndex: {
        get: function() {
            return -1
        },
        configurable: true,
        enumerable: true
    },
    defaultSelectedRowKeys: {
        get: function() {
            return []
        },
        configurable: true,
        enumerable: true
    },
    defaultSelectionFilter: {
        get: function() {
            return []
        },
        configurable: true,
        enumerable: true
    }
}))));
exports.DataGridProps = DataGridProps;
