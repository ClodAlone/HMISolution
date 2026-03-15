/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.row_dragging.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    setWidth,
    getWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import Sortable from "../sortable";
import gridCoreUtils from "./ui.grid_core.utils";
import {
    deferUpdate
} from "../../core/utils/common";
var COMMAND_HANDLE_CLASS = "dx-command-drag";
var CELL_FOCUS_DISABLED_CLASS = "dx-cell-focus-disabled";
var HANDLE_ICON_CLASS = "drag-icon";
var ROWS_VIEW = "rowsview";
var SORTABLE_WITHOUT_HANDLE_CLASS = "dx-sortable-without-handle";
var RowDraggingExtender = {
    init: function() {
        this.callBase.apply(this, arguments);
        this._updateHandleColumn()
    },
    _allowReordering: function() {
        var rowDragging = this.option("rowDragging");
        return !!(rowDragging && (rowDragging.allowReordering || rowDragging.allowDropInsideItem || rowDragging.group))
    },
    _updateHandleColumn: function() {
        var rowDragging = this.option("rowDragging");
        var allowReordering = this._allowReordering();
        var columnsController = this._columnsController;
        var isHandleColumnVisible = allowReordering && rowDragging.showDragIcons;
        columnsController && columnsController.addCommandColumn({
            type: "drag",
            command: "drag",
            visibleIndex: -2,
            alignment: "center",
            cssClass: COMMAND_HANDLE_CLASS,
            width: "auto",
            cellTemplate: this._getHandleTemplate(),
            visible: isHandleColumnVisible
        });
        columnsController.columnOption("type:drag", "visible", isHandleColumnVisible)
    },
    _renderContent: function() {
        var rowDragging = this.option("rowDragging");
        var allowReordering = this._allowReordering();
        var $content = this.callBase.apply(this, arguments);
        var isFixedTableRendering = this._isFixedTableRendering;
        var sortableName = "_sortable";
        var sortableFixedName = "_sortableFixed";
        var currentSortableName = isFixedTableRendering ? sortableFixedName : sortableName;
        var anotherSortableName = isFixedTableRendering ? sortableName : sortableFixedName;
        var togglePointerEventsStyle = toggle => {
            var _this$sortableFixedNa;
            null === (_this$sortableFixedNa = this[sortableFixedName]) || void 0 === _this$sortableFixedNa ? void 0 : _this$sortableFixedNa.$element().css("pointerEvents", toggle ? "auto" : "")
        };
        var filter = this.option("dataRowTemplate") ? "> table > tbody.dx-row:not(.dx-freespace-row):not(.dx-virtual-row)" : "> table > tbody > .dx-row:not(.dx-freespace-row):not(.dx-virtual-row)";
        if ((allowReordering || this[currentSortableName]) && $content.length) {
            this[currentSortableName] = this._createComponent($content, Sortable, extend({
                component: this.component,
                contentTemplate: null,
                filter: filter,
                dragTemplate: this._getDraggableRowTemplate(),
                handle: rowDragging.showDragIcons && ".".concat(COMMAND_HANDLE_CLASS),
                dropFeedbackMode: "indicate"
            }, rowDragging, {
                onDragStart: e => {
                    var _this$getController, _rowDragging$onDragSt;
                    null === (_this$getController = this.getController("keyboardNavigation")) || void 0 === _this$getController ? void 0 : _this$getController._resetFocusedCell();
                    var row = e.component.getVisibleRows()[e.fromIndex];
                    e.itemData = row && row.data;
                    var isDataRow = row && "data" === row.rowType;
                    e.cancel = !allowReordering || !isDataRow;
                    null === (_rowDragging$onDragSt = rowDragging.onDragStart) || void 0 === _rowDragging$onDragSt ? void 0 : _rowDragging$onDragSt.call(rowDragging, e)
                },
                onDragEnter: () => {
                    togglePointerEventsStyle(true)
                },
                onDragLeave: () => {
                    togglePointerEventsStyle(false)
                },
                onDragEnd: e => {
                    var _rowDragging$onDragEn;
                    togglePointerEventsStyle(false);
                    null === (_rowDragging$onDragEn = rowDragging.onDragEnd) || void 0 === _rowDragging$onDragEn ? void 0 : _rowDragging$onDragEn.call(rowDragging, e)
                },
                onAdd: e => {
                    var _rowDragging$onAdd;
                    togglePointerEventsStyle(false);
                    null === (_rowDragging$onAdd = rowDragging.onAdd) || void 0 === _rowDragging$onAdd ? void 0 : _rowDragging$onAdd.call(rowDragging, e)
                },
                dropFeedbackMode: rowDragging.dropFeedbackMode,
                onOptionChanged: e => {
                    var hasFixedSortable = this[sortableFixedName];
                    if (hasFixedSortable) {
                        if ("fromIndex" === e.name || "toIndex" === e.name) {
                            this[anotherSortableName].option(e.name, e.value)
                        }
                    }
                }
            }));
            $content.toggleClass("dx-scrollable-container", isFixedTableRendering);
            $content.toggleClass(SORTABLE_WITHOUT_HANDLE_CLASS, allowReordering && !rowDragging.showDragIcons)
        }
        return $content
    },
    _renderCore(e) {
        this.callBase.apply(this, arguments);
        if (e && "update" === e.changeType && e.repaintChangesOnly && gridCoreUtils.isVirtualRowRendering(this)) {
            deferUpdate(() => {
                this._updateSortable()
            })
        }
    },
    _updateSortable() {
        var offset = this._dataController.getRowIndexOffset();
        [this._sortable, this._sortableFixed].forEach(sortable => {
            null === sortable || void 0 === sortable ? void 0 : sortable.option("offset", offset);
            null === sortable || void 0 === sortable ? void 0 : sortable.update()
        })
    },
    _resizeCore: function() {
        this.callBase.apply(this, arguments);
        this._updateSortable()
    },
    _getDraggableGridOptions: function(options) {
        var gridOptions = this.option();
        var columns = this.getColumns();
        var $rowElement = $(this.getRowElement(options.rowIndex));
        return {
            dataSource: [{
                id: 1,
                parentId: 0
            }],
            showBorders: true,
            showColumnHeaders: false,
            scrolling: {
                useNative: false,
                showScrollbar: "never"
            },
            pager: {
                visible: false
            },
            loadingTimeout: null,
            columnFixing: gridOptions.columnFixing,
            columnAutoWidth: gridOptions.columnAutoWidth,
            showColumnLines: gridOptions.showColumnLines,
            columns: columns.map(column => ({
                width: column.width || column.visibleWidth,
                fixed: column.fixed,
                fixedPosition: column.fixedPosition
            })),
            onRowPrepared: e => {
                var rowsView = e.component.getView("rowsView");
                $(e.rowElement).replaceWith($rowElement.eq(rowsView._isFixedTableRendering ? 1 : 0).clone())
            }
        }
    },
    _getDraggableRowTemplate: function() {
        return options => {
            var $rootElement = this.component.$element();
            var $dataGridContainer = $("<div>");
            setWidth($dataGridContainer, getWidth($rootElement));
            var items = this._dataController.items();
            var row = items && items[options.fromIndex];
            var gridOptions = this._getDraggableGridOptions(row);
            this._createComponent($dataGridContainer, this.component.NAME, gridOptions);
            $dataGridContainer.find(".dx-gridbase-container").children(":not(.".concat(this.addWidgetPrefix(ROWS_VIEW), ")")).hide();
            return $dataGridContainer
        }
    },
    _getHandleTemplate: function() {
        return (container, options) => {
            if ("data" === options.rowType) {
                $(container).addClass(CELL_FOCUS_DISABLED_CLASS);
                return $("<span>").addClass(this.addWidgetPrefix(HANDLE_ICON_CLASS))
            } else {
                gridCoreUtils.setEmptyText($(container))
            }
        }
    },
    optionChanged: function(args) {
        if ("rowDragging" === args.name) {
            this._updateHandleColumn();
            this._invalidate(true, true);
            args.handled = true
        }
        this.callBase.apply(this, arguments)
    }
};
export var rowDraggingModule = {
    defaultOptions: function() {
        return {
            rowDragging: {
                showDragIcons: true,
                dropFeedbackMode: "indicate",
                allowReordering: false,
                allowDropInsideItem: false
            }
        }
    },
    extenders: {
        views: {
            rowsView: RowDraggingExtender
        }
    }
};
