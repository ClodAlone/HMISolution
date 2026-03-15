/**
 * DevExtreme (esm/ui/gantt/ui.gantt.treelist.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import dxTreeList from "../tree_list";
import {
    getBoundingRect
} from "../../core/utils/position";
import {
    isDefined
} from "../../core/utils/type";
import {
    GanttHelper
} from "./ui.gantt.helper";
var GANTT_TASKS = "tasks";
var GANTT_COLLAPSABLE_ROW = "dx-gantt-collapsable-row";
var GANTT_DEFAULT_ROW_HEIGHT = 34;
export class GanttTreeList {
    constructor(gantt) {
        this._gantt = gantt;
        this._$treeList = this._gantt._$treeList
    }
    getTreeList() {
        var {
            keyExpr: keyExpr,
            parentIdExpr: parentIdExpr
        } = this._gantt.option(GANTT_TASKS);
        this._treeList = this._gantt._createComponent(this._$treeList, dxTreeList, {
            dataSource: this._gantt._tasksRaw,
            keyExpr: keyExpr,
            parentIdExpr: parentIdExpr,
            columns: this.getColumns(),
            columnResizingMode: "nextColumn",
            height: this._getHeight(),
            width: this._gantt.option("taskListWidth"),
            selection: {
                mode: GanttHelper.getSelectionMode(this._gantt.option("allowSelection"))
            },
            selectedRowKeys: GanttHelper.getArrayFromOneElement(this._gantt.option("selectedRowKey")),
            sorting: this._gantt.option("sorting"),
            filterRow: this._gantt.option("filterRow"),
            headerFilter: this._gantt.option("headerFilter"),
            scrolling: {
                showScrollbar: "onHover",
                mode: "virtual"
            },
            allowColumnResizing: true,
            autoExpandAll: true,
            showRowLines: this._gantt.option("showRowLines"),
            rootValue: this._gantt.option("rootValue"),
            onContentReady: e => {
                this._onContentReady(e)
            },
            onSelectionChanged: e => {
                this._onSelectionChanged(e)
            },
            onRowCollapsed: e => {
                this._onRowCollapsed(e)
            },
            onRowExpanded: e => {
                this._onRowExpanded(e)
            },
            onRowPrepared: e => {
                this._onRowPrepared(e)
            },
            onContextMenuPreparing: e => {
                this._onContextMenuPreparing(e)
            },
            onRowClick: e => {
                this.onRowClick(e)
            },
            onRowDblClick: e => {
                this.onRowDblClick(e)
            }
        });
        return this._treeList
    }
    onAfterTreeListCreate() {
        if (this._postponedGanttInitRequired) {
            this._initGanttOnContentReady({
                component: this._treeList
            });
            delete this._postponedGanttInitRequired
        }
    }
    _onContentReady(e) {
        var hasTreeList = !!this._treeList;
        if (hasTreeList) {
            this._initGanttOnContentReady(e)
        } else {
            this._postponedGanttInitRequired = true
        }
    }
    _initGanttOnContentReady(e) {
        if (e.component.getDataSource()) {
            this._gantt._initGanttView();
            this._initScrollSync(e.component)
        }
        this._gantt._sortAndFilter();
        this._gantt._sizeHelper.updateGanttRowHeights()
    }
    _onSelectionChanged(e) {
        var selectedRowKey = e.currentSelectedRowKeys[0];
        this._gantt._setGanttViewOption("selectedRowKey", selectedRowKey);
        this._gantt._setOptionWithoutOptionChange("selectedRowKey", selectedRowKey);
        this._gantt._actionsManager.raiseSelectionChangedAction(selectedRowKey)
    }
    _onRowCollapsed(e) {
        this._gantt._onTreeListRowExpandChanged(e, false)
    }
    _onRowExpanded(e) {
        this._gantt._onTreeListRowExpandChanged(e, true)
    }
    _onRowPrepared(e) {
        if ("data" === e.rowType && e.node.children.length > 0) {
            $(e.rowElement).addClass(GANTT_COLLAPSABLE_ROW)
        }
    }
    _onContextMenuPreparing(e) {
        var _e$row, _e$row2;
        if ("header" === e.target) {
            return
        }
        if ("data" === (null === (_e$row = e.row) || void 0 === _e$row ? void 0 : _e$row.rowType)) {
            this.setOption("selectedRowKeys", [e.row.data[this._gantt.option("tasks.keyExpr")]])
        }
        e.items = [];
        var info = {
            cancel: false,
            event: e.event,
            type: "task",
            key: null === (_e$row2 = e.row) || void 0 === _e$row2 ? void 0 : _e$row2.key,
            position: {
                x: e.event.pageX,
                y: e.event.pageY
            }
        };
        this._gantt._showPopupMenu(info)
    }
    _getHeight() {
        if (getHeight(this._$treeList)) {
            return getHeight(this._$treeList)
        }
        this._gantt._hasHeight = isDefined(this._gantt.option("height")) && "" !== this._gantt.option("height");
        return this._gantt._hasHeight ? "100%" : ""
    }
    _initScrollSync(treeList) {
        var treeListScrollable = treeList.getScrollable();
        if (treeListScrollable) {
            treeListScrollable.off("scroll");
            treeListScrollable.on("scroll", e => {
                this._onScroll(e)
            })
        }
    }
    _onScroll(treeListScrollView) {
        var ganttViewTaskAreaContainer = this._gantt._ganttView.getTaskAreaContainer();
        if (ganttViewTaskAreaContainer.scrollTop !== treeListScrollView.component.scrollTop()) {
            ganttViewTaskAreaContainer.scrollTop = treeListScrollView.component.scrollTop()
        }
    }
    _correctRowsViewRowHeight(height) {
        var view = this._treeList._views && this._treeList._views.rowsView;
        if ((null === view || void 0 === view ? void 0 : view._rowHeight) !== height) {
            view._rowHeight = height
        }
    }
    _skipUpdateTreeListDataSource() {
        return this._gantt.option("validation.autoUpdateParentTasks")
    }
    selectRows(keys) {
        this.setOption("selectedRowKeys", keys)
    }
    scrollBy(scrollTop) {
        var treeListScrollable = this._treeList.getScrollable();
        if (treeListScrollable) {
            var diff = scrollTop - treeListScrollable.scrollTop();
            if (0 !== diff) {
                treeListScrollable.scrollBy({
                    left: 0,
                    top: diff
                })
            }
        }
    }
    updateDataSource(data) {
        var forceUpdate = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : false;
        var forceCustomData = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : false;
        var expandedRowKeys = this.getOption("expandedRowKeys");
        if (!this._skipUpdateTreeListDataSource() || forceUpdate) {
            this.setOption("dataSource", data)
        } else if (forceCustomData) {
            var _data = this._treeList.option("dataSource");
            this._gantt._onParentTasksRecalculated(_data)
        }
        this.setOption("expandedRowKeys", expandedRowKeys)
    }
    onRowClick(e) {
        this._gantt._actionsManager.raiseTaskClickAction(e.key, e.event)
    }
    onRowDblClick(e) {
        if (this._gantt._actionsManager.raiseTaskDblClickAction(e.key, e.event)) {
            this._gantt._ganttView._ganttViewCore.showTaskEditDialog()
        }
    }
    getOffsetHeight() {
        return this._gantt._treeList._$element.get(0).offsetHeight
    }
    getRowHeight() {
        var $row = this._treeList._$element.find(".dx-data-row");
        var height = $row.length ? getBoundingRect($row.last().get(0)).height : GANTT_DEFAULT_ROW_HEIGHT;
        if (!height) {
            height = GANTT_DEFAULT_ROW_HEIGHT
        }
        this._correctRowsViewRowHeight(height);
        return height
    }
    getHeaderHeight() {
        return getBoundingRect(this._treeList._$element.find(".dx-treelist-headers").get(0)).height
    }
    getColumns() {
        var columns = this._gantt.option("columns");
        if (columns) {
            for (var i = 0; i < columns.length; i++) {
                var column = columns[i];
                var isKeyColumn = column.dataField === this._gantt.option("".concat(GANTT_TASKS, ".keyExpr")) || column.dataField === this._gantt.option("".concat(GANTT_TASKS, ".parentIdExpr"));
                if (isKeyColumn && !column.dataType) {
                    column.dataType = "object"
                }
            }
        }
        return columns
    }
    getSievedItems() {
        var rootNode = this._treeList.getRootNode();
        if (!rootNode) {
            return
        }
        var resultArray = [];
        GanttHelper.convertTreeToList(rootNode, resultArray);
        var getters = GanttHelper.compileGettersByOption(this._gantt.option(GANTT_TASKS));
        var validatedData = this._gantt._validateSourceData(GANTT_TASKS, resultArray);
        var mappedData = validatedData.map(GanttHelper.prepareMapHandler(getters));
        return mappedData
    }
    setOption(optionName, value) {
        this._treeList && this._treeList.option(optionName, value)
    }
    getOption(optionName) {
        return this._treeList.option(optionName)
    }
    onTaskInserted(insertedId, parentId) {
        if (isDefined(parentId)) {
            var expandedRowKeys = this.getOption("expandedRowKeys");
            if (-1 === expandedRowKeys.indexOf(parentId)) {
                expandedRowKeys.push(parentId);
                this.setOption("expandedRowKeys", expandedRowKeys)
            }
        }
        this.selectRows(GanttHelper.getArrayFromOneElement(insertedId));
        this.setOption("focusedRowKey", insertedId)
    }
    getDataSource() {
        var _this$_treeList;
        return null === (_this$_treeList = this._treeList) || void 0 === _this$_treeList ? void 0 : _this$_treeList.getDataSource()
    }
}
