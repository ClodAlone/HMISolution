/**
 * DevExtreme (esm/ui/scheduler/workspaces/ui.scheduler.work_space.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    setWidth,
    getOuterHeight,
    getOuterWidth,
    setOuterHeight,
    getHeight,
    getWidth
} from "../../../core/utils/size";
import $ from "../../../core/renderer";
import domAdapter from "../../../core/dom_adapter";
import eventsEngine from "../../../events/core/events_engine";
import dateUtils from "../../../core/utils/date";
import {
    getWindow,
    hasWindow
} from "../../../core/utils/window";
import {
    getPublicElement
} from "../../../core/element";
import {
    extend
} from "../../../core/utils/extend";
import {
    getBoundingRect
} from "../../../core/utils/position";
import messageLocalization from "../../../localization/message";
import {
    noop
} from "../../../core/utils/common";
import {
    isDefined
} from "../../../core/utils/type";
import {
    addNamespace,
    isMouseEvent
} from "../../../events/utils/index";
import pointerEvents from "../../../events/pointer";
import errors from "../../widget/ui.errors";
import {
    name as clickEventName
} from "../../../events/click";
import {
    name as contextMenuEventName
} from "../../../events/contextmenu";
import {
    enter as dragEventEnter,
    leave as dragEventLeave,
    drop as dragEventDrop
} from "../../../events/drag";
import Scrollable from "../../scroll_view/ui.scrollable";
import HorizontalGroupedStrategy from "./ui.scheduler.work_space.grouped.strategy.horizontal";
import VerticalGroupedStrategy from "./ui.scheduler.work_space.grouped.strategy.vertical";
import tableCreatorModule from "../table_creator";
var {
    tableCreator: tableCreator
} = tableCreatorModule;
import VerticalShader from "../shaders/ui.scheduler.current_time_shader.vertical";
import AppointmentDragBehavior from "../appointmentDragBehavior";
import {
    APPOINTMENT_SETTINGS_KEY
} from "../constants";
import {
    FIXED_CONTAINER_CLASS,
    VIRTUAL_CELL_CLASS,
    TIME_PANEL_CLASS,
    DATE_TABLE_CLASS,
    DATE_TABLE_ROW_CLASS,
    GROUP_ROW_CLASS,
    GROUP_HEADER_CONTENT_CLASS,
    VERTICAL_GROUP_COUNT_CLASSES
} from "../classes";
import WidgetObserver from "../base/widgetObserver";
import {
    resetPosition,
    locate
} from "../../../animation/translator";
import {
    VirtualScrollingDispatcher,
    VirtualScrollingRenderer
} from "./ui.scheduler.virtual_scrolling";
import ViewDataProvider from "./view_model/view_data_provider";
import dxrDateTableLayout from "../../../renovation/ui/scheduler/workspaces/base/date_table/layout.j";
import dxrAllDayPanelTable from "../../../renovation/ui/scheduler/workspaces/base/date_table/all_day_panel/table.j";
import dxrAllDayPanelTitle from "../../../renovation/ui/scheduler/workspaces/base/date_table/all_day_panel/title.j";
import dxrTimePanelTableLayout from "../../../renovation/ui/scheduler/workspaces/base/time_panel/layout.j";
import dxrGroupPanel from "../../../renovation/ui/scheduler/workspaces/base/group_panel/group_panel.j";
import dxrDateHeader from "../../../renovation/ui/scheduler/workspaces/base/header_panel/layout.j";
import CellsSelectionState from "./cells_selection_state";
import {
    Cache
} from "./cache";
import {
    CellsSelectionController
} from "./cells_selection_controller";
import {
    calculateViewStartDate,
    getViewStartByOptions,
    validateDayHours,
    getStartViewDateTimeOffset,
    isDateAndTimeView,
    calculateIsGroupedAllDayPanel,
    getCellDuration
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
import {
    createResourcesTree,
    getCellGroups,
    getGroupsObjectFromGroupsArray,
    getGroupCount
} from "../resources/utils";
import {
    ScrollSemaphore
} from "../../../renovation/ui/scheduler/utils/semaphore/scrollSemaphore";
import {
    getCellWidth,
    getCellHeight,
    getAllDayHeight,
    getMaxAllowedPosition,
    PositionHelper
} from "./helpers/positionHelper";
import {
    utils
} from "../utils";
import {
    compileGetter
} from "../../../core/utils/data";
var abstract = WidgetObserver.abstract;
var toMs = dateUtils.dateToMilliseconds;
var COMPONENT_CLASS = "dx-scheduler-work-space";
var GROUPED_WORKSPACE_CLASS = "dx-scheduler-work-space-grouped";
var VERTICAL_GROUPED_WORKSPACE_CLASS = "dx-scheduler-work-space-vertical-grouped";
var WORKSPACE_VERTICAL_GROUP_TABLE_CLASS = "dx-scheduler-work-space-vertical-group-table";
var WORKSPACE_WITH_BOTH_SCROLLS_CLASS = "dx-scheduler-work-space-both-scrollbar";
var WORKSPACE_WITH_COUNT_CLASS = "dx-scheduler-work-space-count";
var WORKSPACE_WITH_GROUP_BY_DATE_CLASS = "dx-scheduler-work-space-group-by-date";
var WORKSPACE_WITH_ODD_CELLS_CLASS = "dx-scheduler-work-space-odd-cells";
var TIME_PANEL_CELL_CLASS = "dx-scheduler-time-panel-cell";
var TIME_PANEL_ROW_CLASS = "dx-scheduler-time-panel-row";
var ALL_DAY_PANEL_CLASS = "dx-scheduler-all-day-panel";
var ALL_DAY_TABLE_CLASS = "dx-scheduler-all-day-table";
var ALL_DAY_CONTAINER_CLASS = "dx-scheduler-all-day-appointments";
var ALL_DAY_TITLE_CLASS = "dx-scheduler-all-day-title";
var ALL_DAY_TABLE_CELL_CLASS = "dx-scheduler-all-day-table-cell";
var ALL_DAY_TABLE_ROW_CLASS = "dx-scheduler-all-day-table-row";
var WORKSPACE_WITH_ALL_DAY_CLASS = "dx-scheduler-work-space-all-day";
var WORKSPACE_WITH_COLLAPSED_ALL_DAY_CLASS = "dx-scheduler-work-space-all-day-collapsed";
var WORKSPACE_WITH_MOUSE_SELECTION_CLASS = "dx-scheduler-work-space-mouse-selection";
var HORIZONTAL_SIZES_CLASS = "dx-scheduler-cell-sizes-horizontal";
var VERTICAL_SIZES_CLASS = "dx-scheduler-cell-sizes-vertical";
var HEADER_PANEL_CLASS = "dx-scheduler-header-panel";
var HEADER_PANEL_CELL_CLASS = "dx-scheduler-header-panel-cell";
var HEADER_ROW_CLASS = "dx-scheduler-header-row";
var GROUP_HEADER_CLASS = "dx-scheduler-group-header";
var DATE_TABLE_CELL_CLASS = "dx-scheduler-date-table-cell";
var DATE_TABLE_FOCUSED_CELL_CLASS = "dx-scheduler-focused-cell";
var VIRTUAL_ROW_CLASS = "dx-scheduler-virtual-row";
var DATE_TABLE_DROPPABLE_CELL_CLASS = "dx-scheduler-date-table-droppable-cell";
var SCHEDULER_HEADER_SCROLLABLE_CLASS = "dx-scheduler-header-scrollable";
var SCHEDULER_SIDEBAR_SCROLLABLE_CLASS = "dx-scheduler-sidebar-scrollable";
var SCHEDULER_DATE_TABLE_SCROLLABLE_CLASS = "dx-scheduler-date-table-scrollable";
var SCHEDULER_WORKSPACE_DXPOINTERDOWN_EVENT_NAME = addNamespace(pointerEvents.down, "dxSchedulerWorkSpace");
var DragEventNames = {
    ENTER: addNamespace(dragEventEnter, "dxSchedulerDateTable"),
    DROP: addNamespace(dragEventDrop, "dxSchedulerDateTable"),
    LEAVE: addNamespace(dragEventLeave, "dxSchedulerDateTable")
};
var SCHEDULER_CELL_DXCLICK_EVENT_NAME = addNamespace(clickEventName, "dxSchedulerDateTable");
var SCHEDULER_CELL_DXPOINTERDOWN_EVENT_NAME = addNamespace(pointerEvents.down, "dxSchedulerDateTable");
var SCHEDULER_CELL_DXPOINTERUP_EVENT_NAME = addNamespace(pointerEvents.up, "dxSchedulerDateTable");
var SCHEDULER_CELL_DXPOINTERMOVE_EVENT_NAME = addNamespace(pointerEvents.move, "dxSchedulerDateTable");
var CELL_DATA = "dxCellData";
var DATE_TABLE_MIN_CELL_WIDTH = 75;
var DAY_MS = toMs("day");
var HOUR_MS = toMs("hour");
var DRAG_AND_DROP_SELECTOR = ".".concat(DATE_TABLE_CLASS, " td, .").concat(ALL_DAY_TABLE_CLASS, " td");
var CELL_SELECTOR = ".".concat(DATE_TABLE_CELL_CLASS, ", .").concat(ALL_DAY_TABLE_CELL_CLASS);
class SchedulerWorkSpace extends WidgetObserver {
    get viewDataProvider() {
        if (!this._viewDataProvider) {
            this._viewDataProvider = new ViewDataProvider(this.type)
        }
        return this._viewDataProvider
    }
    get cache() {
        if (!this._cache) {
            this._cache = new Cache
        }
        return this._cache
    }
    get cellsSelectionState() {
        if (!this._cellsSelectionState) {
            this._cellsSelectionState = new CellsSelectionState(this.viewDataProvider);
            var selectedCellsOption = this.option("selectedCellData");
            if ((null === selectedCellsOption || void 0 === selectedCellsOption ? void 0 : selectedCellsOption.length) > 0) {
                var validSelectedCells = selectedCellsOption.map(selectedCell => {
                    var groups = selectedCell.groups;
                    if (!groups || 0 === this._getGroupCount()) {
                        return _extends({}, selectedCell, {
                            groupIndex: 0
                        })
                    }
                    var groupIndex = this._getGroupIndexByResourceId(groups);
                    return _extends({}, selectedCell, {
                        groupIndex: groupIndex
                    })
                });
                this._cellsSelectionState.setSelectedCellsByData(validSelectedCells)
            }
        }
        return this._cellsSelectionState
    }
    get cellsSelectionController() {
        if (!this._cellsSelectionController) {
            this._cellsSelectionController = new CellsSelectionController
        }
        return this._cellsSelectionController
    }
    get isAllDayPanelVisible() {
        return this._isShowAllDayPanel() && this.supportAllDayRow()
    }
    get verticalGroupTableClass() {
        return WORKSPACE_VERTICAL_GROUP_TABLE_CLASS
    }
    get viewDirection() {
        return "vertical"
    }
    get renovatedHeaderPanelComponent() {
        return dxrDateHeader
    }
    get timeZoneCalculator() {
        return this.option("timeZoneCalculator")
    }
    _supportedKeys() {
        var clickHandler = function(e) {
            e.preventDefault();
            e.stopPropagation();
            var selectedCells = this.cellsSelectionState.getSelectedCells();
            if (null !== selectedCells && void 0 !== selectedCells && selectedCells.length) {
                var selectedCellsElement = selectedCells.map(cellData => this._getCellByData(cellData)).filter(cell => !!cell);
                e.target = selectedCellsElement;
                this._showPopup = true;
                this._cellClickAction({
                    event: e,
                    cellElement: $(selectedCellsElement),
                    cellData: selectedCells[0]
                })
            }
        };
        var onArrowPressed = (e, key) => {
            var _this$cellsSelectionS;
            e.preventDefault();
            e.stopPropagation();
            var focusedCellData = null === (_this$cellsSelectionS = this.cellsSelectionState.focusedCell) || void 0 === _this$cellsSelectionS ? void 0 : _this$cellsSelectionS.cellData;
            if (focusedCellData) {
                var isAllDayPanelCell = focusedCellData.allDay && !this._isVerticalGroupedWorkSpace();
                var isMultiSelection = e.shiftKey;
                var isMultiSelectionAllowed = this.option("allowMultipleCellSelection");
                var isRTL = this._isRTL();
                var groupCount = this._getGroupCount();
                var isGroupedByDate = this.isGroupedByDate();
                var isHorizontalGrouping = this._isHorizontalGroupedWorkSpace();
                var focusedCellPosition = this.viewDataProvider.findCellPositionInMap(_extends({}, focusedCellData, {
                    isAllDay: focusedCellData.allDay
                }));
                var edgeIndices = isHorizontalGrouping && isMultiSelection && !isGroupedByDate ? this.viewDataProvider.getGroupEdgeIndices(focusedCellData.groupIndex, isAllDayPanelCell) : this.viewDataProvider.getViewEdgeIndices(isAllDayPanelCell);
                var nextCellData = this.cellsSelectionController.handleArrowClick({
                    focusedCellPosition: focusedCellPosition,
                    edgeIndices: edgeIndices,
                    isRTL: isRTL,
                    isGroupedByDate: isGroupedByDate,
                    groupCount: groupCount,
                    isMultiSelection: isMultiSelection,
                    isMultiSelectionAllowed: isMultiSelectionAllowed,
                    viewType: this.type,
                    key: key,
                    getCellDataByPosition: this.viewDataProvider.getCellData.bind(this.viewDataProvider),
                    isAllDayPanelCell: isAllDayPanelCell,
                    focusedCellData: focusedCellData
                });
                this._processNextSelectedCell(nextCellData, focusedCellData, isMultiSelection && isMultiSelectionAllowed)
            }
        };
        return extend(super._supportedKeys(), {
            enter: clickHandler,
            space: clickHandler,
            downArrow: e => {
                onArrowPressed(e, "down")
            },
            upArrow: e => {
                onArrowPressed(e, "up")
            },
            rightArrow: e => {
                onArrowPressed(e, "right")
            },
            leftArrow: e => {
                onArrowPressed(e, "left")
            }
        })
    }
    _isRTL() {
        return this.option("rtlEnabled")
    }
    _moveToCell($cell, isMultiSelection) {
        if (!isDefined($cell) || !$cell.length) {
            return
        }
        var isMultiSelectionAllowed = this.option("allowMultipleCellSelection");
        var currentCellData = this._getFullCellData($cell);
        var focusedCellData = this.cellsSelectionState.focusedCell.cellData;
        var nextFocusedCellData = this.cellsSelectionController.moveToCell({
            isMultiSelection: isMultiSelection,
            isMultiSelectionAllowed: isMultiSelectionAllowed,
            currentCellData: currentCellData,
            focusedCellData: focusedCellData,
            isVirtualCell: $cell.hasClass(VIRTUAL_CELL_CLASS)
        });
        this._processNextSelectedCell(nextFocusedCellData, focusedCellData, isMultiSelectionAllowed && isMultiSelection)
    }
    _processNextSelectedCell(nextCellData, focusedCellData, isMultiSelection) {
        var nextCellPosition = this.viewDataProvider.findCellPositionInMap({
            startDate: nextCellData.startDate,
            groupIndex: nextCellData.groupIndex,
            isAllDay: nextCellData.allDay,
            index: nextCellData.index
        });
        if (!this.viewDataProvider.isSameCell(focusedCellData, nextCellData)) {
            var $cell = nextCellData.allDay && !this._isVerticalGroupedWorkSpace() ? this._dom_getAllDayPanelCell(nextCellPosition.columnIndex) : this._dom_getDateCell(nextCellPosition);
            var isNextCellAllDay = nextCellData.allDay;
            this._setSelectedCellsStateAndUpdateSelection(isNextCellAllDay, nextCellPosition, isMultiSelection, $cell);
            this._dateTableScrollable.scrollToElement($cell)
        }
    }
    _setSelectedCellsStateAndUpdateSelection(isAllDay, cellPosition, isMultiSelection, $nextFocusedCell) {
        var nextCellCoordinates = {
            rowIndex: cellPosition.rowIndex,
            columnIndex: cellPosition.columnIndex,
            allDay: isAllDay
        };
        this.cellsSelectionState.setFocusedCell(nextCellCoordinates.rowIndex, nextCellCoordinates.columnIndex, isAllDay);
        if (isMultiSelection) {
            this.cellsSelectionState.setSelectedCells(nextCellCoordinates)
        } else {
            this.cellsSelectionState.setSelectedCells(nextCellCoordinates, nextCellCoordinates)
        }
        this.updateCellsSelection();
        this._updateSelectedCellDataOption(this.cellsSelectionState.getSelectedCells(), $nextFocusedCell)
    }
    _hasAllDayClass($cell) {
        return $cell.hasClass(ALL_DAY_TABLE_CELL_CLASS)
    }
    _focusInHandler(e) {
        if ($(e.target).is(this._focusTarget()) && false !== this._isCellClick) {
            delete this._isCellClick;
            delete this._contextMenuHandled;
            super._focusInHandler.apply(this, arguments);
            this.cellsSelectionState.restoreSelectedAndFocusedCells();
            if (!this.cellsSelectionState.focusedCell) {
                var cellCoordinates = {
                    columnIndex: 0,
                    rowIndex: 0,
                    allDay: this._isVerticalGroupedWorkSpace() && this.isAllDayPanelVisible
                };
                this.cellsSelectionState.setFocusedCell(cellCoordinates.rowIndex, cellCoordinates.columnIndex, cellCoordinates.allDay);
                this.cellsSelectionState.setSelectedCells(cellCoordinates, cellCoordinates)
            }
            this.updateCellsSelection();
            this._updateSelectedCellDataOption(this.cellsSelectionState.getSelectedCells())
        }
    }
    _focusOutHandler() {
        super._focusOutHandler.apply(this, arguments);
        if (!this._contextMenuHandled && !this._disposed) {
            this.cellsSelectionState.releaseSelectedAndFocusedCells();
            this.viewDataProvider.updateViewData(this.generateRenderOptions());
            this.updateCellsSelection()
        }
    }
    _focusTarget() {
        return this.$element()
    }
    _isVerticalGroupedWorkSpace() {
        return !!this.option("groups").length && "vertical" === this.option("groupOrientation")
    }
    _isHorizontalGroupedWorkSpace() {
        return !!this.option("groups").length && "horizontal" === this.option("groupOrientation")
    }
    _isWorkSpaceWithCount() {
        return this.option("intervalCount") > 1
    }
    _isWorkspaceWithOddCells() {
        return .5 === this.option("hoursInterval") && !this.isVirtualScrolling()
    }
    _getRealGroupOrientation() {
        return this._isVerticalGroupedWorkSpace() ? "vertical" : "horizontal"
    }
    createRAllDayPanelElements() {
        this._$allDayPanel = $("<div>").addClass(ALL_DAY_PANEL_CLASS);
        this._$allDayTitle = $("<div>").appendTo(this._$headerPanelEmptyCell)
    }
    _dateTableScrollableConfig() {
        var config = {
            useKeyboard: false,
            bounceEnabled: false,
            updateManually: true,
            onScroll: () => {
                var _this$_groupedStrateg;
                null === (_this$_groupedStrateg = this._groupedStrategy.cache) || void 0 === _this$_groupedStrateg ? void 0 : _this$_groupedStrateg.clear()
            }
        };
        if (this._needCreateCrossScrolling()) {
            config = extend(config, this._createCrossScrollingConfig(config))
        }
        if (this.isVirtualScrolling() && (this.virtualScrollingDispatcher.horizontalScrollingAllowed || this.virtualScrollingDispatcher.height)) {
            var currentOnScroll = config.onScroll;
            config = _extends({}, config, {
                onScroll: e => {
                    null === currentOnScroll || void 0 === currentOnScroll ? void 0 : currentOnScroll(e);
                    this.virtualScrollingDispatcher.handleOnScrollEvent(null === e || void 0 === e ? void 0 : e.scrollOffset)
                }
            })
        }
        return config
    }
    _createCrossScrollingConfig(currentConfig) {
        var config = {};
        config.direction = "both";
        var currentOnScroll = currentConfig.onScroll;
        config.onScroll = e => {
            currentOnScroll();
            this._dataTableSemaphore.take(e.scrollOffset);
            if (this._sideBarSemaphore.isFree(e.scrollOffset)) {
                var _this$_sidebarScrolla;
                null === (_this$_sidebarScrolla = this._sidebarScrollable) || void 0 === _this$_sidebarScrolla ? void 0 : _this$_sidebarScrolla.scrollTo({
                    top: e.scrollOffset.top
                })
            }
            if (this._headerSemaphore.isFree(e.scrollOffset)) {
                var _this$_headerScrollab;
                null === (_this$_headerScrollab = this._headerScrollable) || void 0 === _this$_headerScrollab ? void 0 : _this$_headerScrollab.scrollTo({
                    left: e.scrollOffset.left
                })
            }
            this._dataTableSemaphore.release()
        };
        config.onEnd = () => {
            this.option("onScrollEnd")()
        };
        return config
    }
    _headerScrollableConfig() {
        var config = {
            useKeyboard: false,
            showScrollbar: "never",
            direction: "horizontal",
            useNative: false,
            updateManually: true,
            bounceEnabled: false,
            onScroll: e => {
                this._headerSemaphore.take(e.scrollOffset);
                this._dataTableSemaphore.isFree(e.scrollOffset) && this._dateTableScrollable.scrollTo({
                    left: e.scrollOffset.left
                });
                this._headerSemaphore.release()
            }
        };
        return config
    }
    _visibilityChanged(visible) {
        this.cache.clear();
        if (visible) {
            this._updateGroupTableHeight()
        }
        if (visible && this._needCreateCrossScrolling()) {
            this._setTableSizes()
        }
    }
    _setTableSizes() {
        this.cache.clear();
        this._attachTableClasses();
        var cellWidth = this.getCellWidth();
        if (cellWidth < this.getCellMinWidth()) {
            cellWidth = this.getCellMinWidth()
        }
        var minWidth = this.getWorkSpaceMinWidth();
        var groupCount = this._getGroupCount();
        var totalCellCount = this._getTotalCellCount(groupCount);
        var width = cellWidth * totalCellCount;
        if (width < minWidth) {
            width = minWidth
        }
        setWidth(this._$headerPanel, width);
        setWidth(this._$dateTable, width);
        if (this._$allDayTable) {
            setWidth(this._$allDayTable, width)
        }
        this._attachHeaderTableClasses();
        this._updateGroupTableHeight();
        this._updateScrollable()
    }
    getWorkSpaceMinWidth() {
        return this._groupedStrategy.getWorkSpaceMinWidth()
    }
    _dimensionChanged() {
        if (!this._isVisible()) {
            return
        }
        if (this.option("crossScrollingEnabled")) {
            this._setTableSizes()
        }
        this.updateHeaderEmptyCellWidth();
        this._updateScrollable();
        this.cache.clear()
    }
    _needCreateCrossScrolling() {
        return this.option("crossScrollingEnabled")
    }
    _getElementClass() {
        return noop()
    }
    _getRowCount() {
        return this.viewDataProvider.getRowCount({
            intervalCount: this.option("intervalCount"),
            currentDate: this.option("currentDate"),
            viewType: this.type,
            hoursInterval: this.option("hoursInterval"),
            startDayHour: this.option("startDayHour"),
            endDayHour: this.option("endDayHour")
        })
    }
    _getCellCount() {
        return this.viewDataProvider.getCellCount({
            intervalCount: this.option("intervalCount"),
            currentDate: this.option("currentDate"),
            viewType: this.type,
            hoursInterval: this.option("hoursInterval"),
            startDayHour: this.option("startDayHour"),
            endDayHour: this.option("endDayHour")
        })
    }
    isRenovatedRender() {
        return this.renovatedRenderSupported() && this.option("renovateRender")
    }
    _isVirtualModeOn() {
        return "virtual" === this.option("scrolling.mode")
    }
    isVirtualScrolling() {
        return this.isRenovatedRender() && this._isVirtualModeOn()
    }
    _initVirtualScrolling() {
        if (this.virtualScrollingDispatcher) {
            this.virtualScrollingDispatcher.dispose();
            this.virtualScrollingDispatcher = null
        }
        this.virtualScrollingDispatcher = new VirtualScrollingDispatcher(this._getVirtualScrollingDispatcherOptions());
        this.virtualScrollingDispatcher.attachScrollableEvents();
        this.renderer = new VirtualScrollingRenderer(this)
    }
    onDataSourceChanged() {}
    isGroupedAllDayPanel() {
        return calculateIsGroupedAllDayPanel(this.option("groups"), this.option("groupOrientation"), this.isAllDayPanelVisible)
    }
    generateRenderOptions(isProvideVirtualCellsWidth) {
        var _this$_getToday;
        var groupCount = this._getGroupCount();
        var groupOrientation = groupCount > 0 ? this.option("groupOrientation") : this._getDefaultGroupStrategy();
        var options = _extends({
            groupByDate: this.option("groupByDate"),
            startRowIndex: 0,
            startCellIndex: 0,
            groupOrientation: groupOrientation,
            today: null === (_this$_getToday = this._getToday) || void 0 === _this$_getToday ? void 0 : _this$_getToday.call(this),
            groups: this.option("groups"),
            isProvideVirtualCellsWidth: isProvideVirtualCellsWidth,
            isAllDayPanelVisible: this.isAllDayPanelVisible,
            selectedCells: this.cellsSelectionState.getSelectedCells(),
            focusedCell: this.cellsSelectionState.focusedCell,
            headerCellTextFormat: this._getFormat(),
            getDateForHeaderText: (_, date) => date,
            startDayHour: this.option("startDayHour"),
            endDayHour: this.option("endDayHour"),
            cellDuration: this.getCellDuration(),
            viewType: this.type,
            intervalCount: this.option("intervalCount"),
            hoursInterval: this.option("hoursInterval"),
            currentDate: this.option("currentDate"),
            startDate: this.option("startDate"),
            firstDayOfWeek: this.option("firstDayOfWeek")
        }, this.virtualScrollingDispatcher.getRenderState());
        return options
    }
    renovatedRenderSupported() {
        return true
    }
    _updateGroupTableHeight() {
        if (this._isVerticalGroupedWorkSpace() && hasWindow()) {
            this._setHorizontalGroupHeaderCellsHeight()
        }
    }
    updateHeaderEmptyCellWidth() {
        if (hasWindow() && this._isRenderHeaderPanelEmptyCell()) {
            var timePanelWidth = this.getTimePanelWidth();
            var groupPanelWidth = this.getGroupTableWidth();
            this._$headerPanelEmptyCell.css("width", timePanelWidth + groupPanelWidth)
        }
    }
    _isGroupsSpecified(resources) {
        return this.option("groups").length && resources
    }
    _getGroupIndexByResourceId(id) {
        var groups = this.option("groups");
        var resourceTree = createResourcesTree(groups);
        if (!resourceTree.length) {
            return 0
        }
        return this._getGroupIndexRecursively(resourceTree, id)
    }
    _getGroupIndexRecursively(resourceTree, id) {
        var currentKey = resourceTree[0].name;
        var currentValue = id[currentKey];
        return resourceTree.reduce((prevIndex, _ref) => {
            var {
                leafIndex: leafIndex,
                value: value,
                children: children
            } = _ref;
            var areValuesEqual = currentValue === value;
            if (areValuesEqual && void 0 !== leafIndex) {
                return leafIndex
            }
            if (areValuesEqual) {
                return this._getGroupIndexRecursively(children, id)
            }
            return prevIndex
        }, 0)
    }
    _getViewStartByOptions() {
        return getViewStartByOptions(this.option("startDate"), this.option("currentDate"), this._getIntervalDuration(), this.option("startDate") ? this._calculateViewStartDate() : void 0)
    }
    _getIntervalDuration() {
        return this.viewDataProvider.getIntervalDuration(this.option("intervalCount"))
    }
    _getHeaderDate() {
        return this.getStartViewDate()
    }
    _calculateViewStartDate() {
        return calculateViewStartDate(this.option("startDate"))
    }
    _firstDayOfWeek() {
        return this.viewDataProvider.getFirstDayOfWeek(this.option("firstDayOfWeek"))
    }
    _attachEvents() {
        this._createSelectionChangedAction();
        this._attachClickEvent();
        this._attachContextMenuEvent()
    }
    _attachClickEvent() {
        var that = this;
        var pointerDownAction = this._createAction((function(e) {
            that._pointerDownHandler(e.event)
        }));
        this._createCellClickAction();
        var cellSelector = "." + DATE_TABLE_CELL_CLASS + ",." + ALL_DAY_TABLE_CELL_CLASS;
        var $element = this.$element();
        eventsEngine.off($element, SCHEDULER_WORKSPACE_DXPOINTERDOWN_EVENT_NAME);
        eventsEngine.off($element, SCHEDULER_CELL_DXCLICK_EVENT_NAME);
        eventsEngine.on($element, SCHEDULER_WORKSPACE_DXPOINTERDOWN_EVENT_NAME, (function(e) {
            if (isMouseEvent(e) && e.which > 1) {
                e.preventDefault();
                return
            }
            pointerDownAction({
                event: e
            })
        }));
        eventsEngine.on($element, SCHEDULER_CELL_DXCLICK_EVENT_NAME, cellSelector, (function(e) {
            var $cell = $(e.target);
            that._cellClickAction({
                event: e,
                cellElement: getPublicElement($cell),
                cellData: that.getCellData($cell)
            })
        }))
    }
    _createCellClickAction() {
        this._cellClickAction = this._createActionByOption("onCellClick", {
            afterExecute: e => this._cellClickHandler(e.args[0].event)
        })
    }
    _createSelectionChangedAction() {
        this._selectionChangedAction = this._createActionByOption("onSelectionChanged")
    }
    _cellClickHandler() {
        if (this._showPopup) {
            delete this._showPopup;
            this._handleSelectedCellsClick()
        }
    }
    _pointerDownHandler(e) {
        var $target = $(e.target);
        if (!$target.hasClass(DATE_TABLE_CELL_CLASS) && !$target.hasClass(ALL_DAY_TABLE_CELL_CLASS)) {
            this._isCellClick = false;
            return
        }
        this._isCellClick = true;
        if ($target.hasClass(DATE_TABLE_FOCUSED_CELL_CLASS)) {
            this._showPopup = true
        } else {
            var cellCoordinates = this._getCoordinatesByCell($target);
            var isAllDayCell = this._hasAllDayClass($target);
            this._setSelectedCellsStateAndUpdateSelection(isAllDayCell, cellCoordinates, false, $target)
        }
    }
    _handleSelectedCellsClick() {
        var selectedCells = this.cellsSelectionState.getSelectedCells();
        var firstCellData = selectedCells[0];
        var lastCellData = selectedCells[selectedCells.length - 1];
        var result = {
            startDate: firstCellData.startDate,
            endDate: lastCellData.endDate
        };
        if (void 0 !== lastCellData.allDay) {
            result.allDay = lastCellData.allDay
        }
        this.option("onSelectedCellsClick")(result, lastCellData.groups)
    }
    _attachContextMenuEvent() {
        this._createContextMenuAction();
        var cellSelector = "." + DATE_TABLE_CELL_CLASS + ",." + ALL_DAY_TABLE_CELL_CLASS;
        var $element = this.$element();
        var eventName = addNamespace(contextMenuEventName, this.NAME);
        eventsEngine.off($element, eventName, cellSelector);
        eventsEngine.on($element, eventName, cellSelector, this._contextMenuHandler.bind(this))
    }
    _contextMenuHandler(e) {
        var $cell = $(e.target);
        this._contextMenuAction({
            event: e,
            cellElement: getPublicElement($cell),
            cellData: this.getCellData($cell)
        });
        this._contextMenuHandled = true
    }
    _createContextMenuAction() {
        this._contextMenuAction = this._createActionByOption("onCellContextMenu")
    }
    _getGroupHeaderContainer() {
        if (this._isVerticalGroupedWorkSpace()) {
            return this._$groupTable
        }
        return this._$thead
    }
    _getDateHeaderContainer() {
        return this._$thead
    }
    _getCalculateHeaderCellRepeatCount() {
        return this._groupedStrategy.calculateHeaderCellRepeatCount()
    }
    _updateScrollable() {
        var _this$_headerScrollab2, _this$_sidebarScrolla2;
        this._dateTableScrollable.update();
        null === (_this$_headerScrollab2 = this._headerScrollable) || void 0 === _this$_headerScrollab2 ? void 0 : _this$_headerScrollab2.update();
        null === (_this$_sidebarScrolla2 = this._sidebarScrollable) || void 0 === _this$_sidebarScrolla2 ? void 0 : _this$_sidebarScrolla2.update()
    }
    _getTimePanelRowCount() {
        return this._getCellCountInDay()
    }
    _getCellCountInDay() {
        var hoursInterval = this.option("hoursInterval");
        var startDayHour = this.option("startDayHour");
        var endDayHour = this.option("endDayHour");
        return this.viewDataProvider.getCellCountInDay(startDayHour, endDayHour, hoursInterval)
    }
    _getTotalCellCount(groupCount) {
        return this._groupedStrategy.getTotalCellCount(groupCount)
    }
    _getTotalRowCount(groupCount, includeAllDayPanelRows) {
        var result = this._groupedStrategy.getTotalRowCount(groupCount);
        if (includeAllDayPanelRows && this.isAllDayPanelVisible) {
            result += groupCount
        }
        return result
    }
    _getGroupIndex(rowIndex, columnIndex) {
        return this._groupedStrategy.getGroupIndex(rowIndex, columnIndex)
    }
    calculateEndDate(startDate) {
        var viewDataGenerator = this.viewDataProvider.viewDataGenerator;
        return viewDataGenerator.calculateEndDate(startDate, viewDataGenerator.getInterval(this.option("hoursInterval")), this.option("endDayHour"))
    }
    _getGroupCount() {
        return getGroupCount(this.option("groups"))
    }
    _attachTablesEvents() {
        var element = this.$element();
        this._attachDragEvents(element);
        this._attachPointerEvents(element)
    }
    _detachDragEvents(element) {
        eventsEngine.off(element, DragEventNames.ENTER);
        eventsEngine.off(element, DragEventNames.LEAVE);
        eventsEngine.off(element, DragEventNames.DROP)
    }
    _attachDragEvents(element) {
        this._detachDragEvents(element);
        eventsEngine.on(element, DragEventNames.ENTER, DRAG_AND_DROP_SELECTOR, {
            checkDropTarget: (target, event) => !this._isOutsideScrollable(target, event)
        }, e => {
            this.removeDroppableCellClass();
            $(e.target).addClass(DATE_TABLE_DROPPABLE_CELL_CLASS)
        });
        eventsEngine.on(element, DragEventNames.LEAVE, () => this.removeDroppableCellClass());
        eventsEngine.on(element, DragEventNames.DROP, DRAG_AND_DROP_SELECTOR, () => this.removeDroppableCellClass())
    }
    _attachPointerEvents(element) {
        var isPointerDown = false;
        eventsEngine.off(element, SCHEDULER_CELL_DXPOINTERMOVE_EVENT_NAME);
        eventsEngine.off(element, SCHEDULER_CELL_DXPOINTERDOWN_EVENT_NAME);
        eventsEngine.on(element, SCHEDULER_CELL_DXPOINTERDOWN_EVENT_NAME, DRAG_AND_DROP_SELECTOR, e => {
            if (isMouseEvent(e) && 1 === e.which) {
                isPointerDown = true;
                this.$element().addClass(WORKSPACE_WITH_MOUSE_SELECTION_CLASS);
                eventsEngine.off(domAdapter.getDocument(), SCHEDULER_CELL_DXPOINTERUP_EVENT_NAME);
                eventsEngine.on(domAdapter.getDocument(), SCHEDULER_CELL_DXPOINTERUP_EVENT_NAME, () => {
                    isPointerDown = false;
                    this.$element().removeClass(WORKSPACE_WITH_MOUSE_SELECTION_CLASS)
                })
            }
        });
        eventsEngine.on(element, SCHEDULER_CELL_DXPOINTERMOVE_EVENT_NAME, DRAG_AND_DROP_SELECTOR, e => {
            if (isPointerDown && this._dateTableScrollable && !this._dateTableScrollable.option("scrollByContent")) {
                e.preventDefault();
                e.stopPropagation();
                this._moveToCell($(e.target), true)
            }
        })
    }
    _getFormat() {
        return abstract()
    }
    getWorkArea() {
        return this._$dateTableContainer
    }
    getScrollable() {
        return this._dateTableScrollable
    }
    getScrollableScrollTop() {
        return this._dateTableScrollable.scrollTop()
    }
    getGroupedScrollableScrollTop(allDay) {
        return this._groupedStrategy.getScrollableScrollTop(allDay)
    }
    getScrollableScrollLeft() {
        return this._dateTableScrollable.scrollLeft()
    }
    getScrollableOuterWidth() {
        return this._dateTableScrollable.scrollWidth()
    }
    getScrollableContainer() {
        return $(this._dateTableScrollable.container())
    }
    getHeaderPanelHeight() {
        return this._$headerPanel && getOuterHeight(this._$headerPanel, true)
    }
    getTimePanelWidth() {
        return this._$timePanel && getBoundingRect(this._$timePanel.get(0)).width
    }
    getGroupTableWidth() {
        return this._$groupTable ? getOuterWidth(this._$groupTable) : 0
    }
    getWorkSpaceLeftOffset() {
        return this._groupedStrategy.getLeftOffset()
    }
    _getCellCoordinatesByIndex(index) {
        var columnIndex = Math.floor(index / this._getRowCount());
        var rowIndex = index - this._getRowCount() * columnIndex;
        return {
            columnIndex: columnIndex,
            rowIndex: rowIndex
        }
    }
    _getDateGenerationOptions() {
        var _this$viewDataProvide;
        return {
            startDayHour: this.option("startDayHour"),
            endDayHour: this.option("endDayHour"),
            isWorkView: this.viewDataProvider.viewDataGenerator.isWorkView,
            interval: null === (_this$viewDataProvide = this.viewDataProvider.viewDataGenerator) || void 0 === _this$viewDataProvide ? void 0 : _this$viewDataProvide.getInterval(this.option("hoursInterval")),
            startViewDate: this.getStartViewDate(),
            firstDayOfWeek: this._firstDayOfWeek()
        }
    }
    _getIntervalBetween(currentDate, allDay) {
        var firstViewDate = this.getStartViewDate();
        var startDayTime = this.option("startDayHour") * HOUR_MS;
        var timeZoneOffset = dateUtils.getTimezonesDifference(firstViewDate, currentDate);
        var fullInterval = currentDate.getTime() - firstViewDate.getTime() - timeZoneOffset;
        var days = this._getDaysOfInterval(fullInterval, startDayTime);
        var weekendsCount = this._getWeekendsCount(days);
        var result = (days - weekendsCount) * DAY_MS;
        if (!allDay) {
            var hiddenInterval = this.viewDataProvider.hiddenInterval;
            var visibleDayDuration = this.getVisibleDayDuration();
            result = fullInterval - days * hiddenInterval - weekendsCount * visibleDayDuration
        }
        return result
    }
    _getWeekendsCount() {
        return 0
    }
    _getDaysOfInterval(fullInterval, startDayTime) {
        return Math.floor((fullInterval + startDayTime) / DAY_MS)
    }
    _updateIndex(index) {
        return index * this._getRowCount()
    }
    _getDroppableCell() {
        return this._getDateTables().find("." + DATE_TABLE_DROPPABLE_CELL_CLASS)
    }
    _getWorkSpaceWidth() {
        return this.cache.get("workspaceWidth", () => {
            if (this._needCreateCrossScrolling()) {
                return getBoundingRect(this._$dateTable.get(0)).width
            }
            var totalWidth = getBoundingRect(this.$element().get(0)).width;
            var timePanelWidth = this.getTimePanelWidth();
            var groupTableWidth = this.getGroupTableWidth();
            return totalWidth - timePanelWidth - groupTableWidth
        })
    }
    _getCellByCoordinates(cellCoordinates, groupIndex, inAllDayRow) {
        var indexes = this._groupedStrategy.prepareCellIndexes(cellCoordinates, groupIndex, inAllDayRow);
        return this._dom_getDateCell(indexes)
    }
    _dom_getDateCell(position) {
        return this._$dateTable.find("tr:not(.".concat(VIRTUAL_ROW_CLASS, ")")).eq(position.rowIndex).find("td:not(.".concat(VIRTUAL_CELL_CLASS, ")")).eq(position.columnIndex)
    }
    _dom_getAllDayPanelCell(columnIndex) {
        return this._$allDayPanel.find("tr").eq(0).find("td").eq(columnIndex)
    }
    _getCells(allDay, direction) {
        var cellClass = allDay ? ALL_DAY_TABLE_CELL_CLASS : DATE_TABLE_CELL_CLASS;
        if ("vertical" === direction) {
            var result = [];
            for (var i = 1;; i++) {
                var cells = this.$element().find("tr .".concat(cellClass, ":nth-child(").concat(i, ")"));
                if (!cells.length) {
                    break
                }
                result = result.concat(cells.toArray())
            }
            return $(result)
        } else {
            return this.$element().find("." + cellClass)
        }
    }
    _getAllCells(allDay) {
        if (this._isVerticalGroupedWorkSpace()) {
            return this._$dateTable.find("td:not(.".concat(VIRTUAL_CELL_CLASS, ")"))
        }
        var cellClass = allDay && this.supportAllDayRow() ? ALL_DAY_TABLE_CELL_CLASS : DATE_TABLE_CELL_CLASS;
        return this.$element().find(".".concat(cellClass))
    }
    _setHorizontalGroupHeaderCellsHeight() {
        var height = getBoundingRect(this._$dateTable.get(0)).height;
        setOuterHeight(this._$groupTable, height)
    }
    _getGroupHeaderCells() {
        return this.$element().find("." + GROUP_HEADER_CLASS)
    }
    _getScrollCoordinates(hours, minutes, date, groupIndex, allDay) {
        var currentDate = date || new Date(this.option("currentDate"));
        var startDayHour = this.option("startDayHour");
        var endDayHour = this.option("endDayHour");
        if (hours < startDayHour) {
            hours = startDayHour
        }
        if (hours >= endDayHour) {
            hours = endDayHour - 1
        }
        currentDate.setHours(hours, minutes, 0, 0);
        var cell = this.viewDataProvider.findGlobalCellPosition(currentDate, groupIndex, allDay);
        var {
            position: position,
            cellData: cellData
        } = cell;
        return this.virtualScrollingDispatcher.calculateCoordinatesByDataAndPosition(cellData, position, currentDate, isDateAndTimeView(this.type), "vertical" === this.viewDirection)
    }
    _isOutsideScrollable(target, event) {
        var $dateTableScrollableElement = this._dateTableScrollable.$element();
        var scrollableSize = getBoundingRect($dateTableScrollableElement.get(0));
        var window = getWindow();
        var isTargetInAllDayPanel = !$(target).closest($dateTableScrollableElement).length;
        var isOutsideHorizontalScrollable = event.pageX < scrollableSize.left || event.pageX > scrollableSize.left + scrollableSize.width + (window.scrollX || 0);
        var isOutsideVerticalScrollable = event.pageY < scrollableSize.top || event.pageY > scrollableSize.top + scrollableSize.height + (window.scrollY || 0);
        if (isTargetInAllDayPanel && !isOutsideHorizontalScrollable) {
            return false
        }
        return isOutsideVerticalScrollable || isOutsideHorizontalScrollable
    }
    setCellDataCache(cellCoordinates, groupIndex, $cell) {
        var key = JSON.stringify({
            rowIndex: cellCoordinates.rowIndex,
            columnIndex: cellCoordinates.columnIndex,
            groupIndex: groupIndex
        });
        this.cache.set(key, this.getCellData($cell))
    }
    setCellDataCacheAlias(appointment, geometry) {
        var key = JSON.stringify({
            rowIndex: appointment.rowIndex,
            columnIndex: appointment.columnIndex,
            groupIndex: appointment.groupIndex
        });
        var aliasKey = JSON.stringify({
            top: geometry.top,
            left: geometry.left
        });
        this.cache.set(aliasKey, this.cache.get(key))
    }
    supportAllDayRow() {
        return true
    }
    keepOriginalHours() {
        return false
    }
    _filterCellDataFields(cellData) {
        return extend(true, {}, {
            startDate: cellData.startDate,
            endDate: cellData.endDate,
            groups: cellData.groups,
            groupIndex: cellData.groupIndex,
            allDay: cellData.allDay
        })
    }
    getCellData($cell) {
        var cellData = this._getFullCellData($cell) || {};
        return this._filterCellDataFields(cellData)
    }
    _getFullCellData($cell) {
        var currentCell = $cell[0];
        if (currentCell) {
            return this._getDataByCell($cell)
        }
        return
    }
    _getVirtualRowOffset() {
        return this.virtualScrollingDispatcher.virtualRowOffset
    }
    _getVirtualCellOffset() {
        return this.virtualScrollingDispatcher.virtualCellOffset
    }
    _getDataByCell($cell) {
        var rowIndex = $cell.parent().index() - this.virtualScrollingDispatcher.topVirtualRowsCount;
        var columnIndex = $cell.index() - this.virtualScrollingDispatcher.leftVirtualCellsCount;
        var {
            viewDataProvider: viewDataProvider
        } = this;
        var isAllDayCell = this._hasAllDayClass($cell);
        var cellData = viewDataProvider.getCellData(rowIndex, columnIndex, isAllDayCell);
        return cellData ? cellData : void 0
    }
    isGroupedByDate() {
        return this.option("groupByDate") && this._isHorizontalGroupedWorkSpace() && this._getGroupCount() > 0
    }
    getCellIndexByDate(date, inAllDayRow) {
        var viewDataGenerator = this.viewDataProvider.viewDataGenerator;
        var timeInterval = inAllDayRow ? 864e5 : viewDataGenerator.getInterval(this.option("hoursInterval"));
        var startViewDateOffset = getStartViewDateTimeOffset(this.getStartViewDate(), this.option("startDayHour"));
        var dateTimeStamp = this._getIntervalBetween(date, inAllDayRow) + startViewDateOffset;
        var index = Math.floor(dateTimeStamp / timeInterval);
        if (inAllDayRow) {
            index = this._updateIndex(index)
        }
        if (index < 0) {
            index = 0
        }
        return index
    }
    getDroppableCellIndex() {
        var $droppableCell = this._getDroppableCell();
        var $row = $droppableCell.parent();
        var rowIndex = $row.index();
        return rowIndex * $row.find("td").length + $droppableCell.index()
    }
    getDataByDroppableCell() {
        var cellData = this.getCellData($(this._getDroppableCell()));
        var allDay = cellData.allDay;
        var startDate = cellData.startDate;
        var endDate = cellData.endDate;
        return {
            startDate: startDate,
            endDate: endDate,
            allDay: allDay,
            groups: cellData.groups
        }
    }
    getDateRange() {
        return [this.getStartViewDate(), this.getEndViewDateByEndDayHour()]
    }
    getCellMinWidth() {
        return DATE_TABLE_MIN_CELL_WIDTH
    }
    getRoundedCellWidth(groupIndex, startIndex, cellCount) {
        if (groupIndex < 0 || !hasWindow()) {
            return 0
        }
        var $row = this.$element().find(".".concat(DATE_TABLE_ROW_CLASS)).eq(0);
        var width = 0;
        var $cells = $row.find("." + DATE_TABLE_CELL_CLASS);
        var totalCellCount = this._getCellCount() * groupIndex;
        cellCount = cellCount || this._getCellCount();
        if (!isDefined(startIndex)) {
            startIndex = totalCellCount
        }
        for (var i = startIndex; i < totalCellCount + cellCount; i++) {
            var element = $($cells).eq(i).get(0);
            var elementWidth = element ? getBoundingRect(element).width : 0;
            width += elementWidth
        }
        return width / (totalCellCount + cellCount - startIndex)
    }
    getCellWidth() {
        return getCellWidth(this.getDOMElementsMetaData())
    }
    getCellHeight() {
        return getCellHeight(this.getDOMElementsMetaData())
    }
    getAllDayHeight() {
        return getAllDayHeight(this.option("showAllDayPanel"), this._isVerticalGroupedWorkSpace(), this.getDOMElementsMetaData())
    }
    getMaxAllowedPosition(groupIndex) {
        return getMaxAllowedPosition(groupIndex, this.viewDataProvider, this.option("rtlEnabled"), this.getDOMElementsMetaData())
    }
    getAllDayOffset() {
        return this._groupedStrategy.getAllDayOffset()
    }
    getCellIndexByCoordinates(coordinates, allDay) {
        var cellCount = this._getTotalCellCount(this._getGroupCount());
        var cellWidth = Math.floor(this._getWorkSpaceWidth() / cellCount);
        var cellHeight = allDay ? this.getAllDayHeight() : this.getCellHeight();
        var topIndex = Math.floor(Math.floor(coordinates.top) / Math.floor(cellHeight));
        var leftIndex = Math.floor((coordinates.left + 5) / cellWidth);
        if (this._isRTL()) {
            leftIndex = cellCount - leftIndex - 1
        }
        return cellCount * topIndex + leftIndex
    }
    getStartViewDate() {
        return this.viewDataProvider.getStartViewDate()
    }
    getEndViewDate() {
        return this.viewDataProvider.getLastCellEndDate()
    }
    getEndViewDateByEndDayHour() {
        return this.viewDataProvider.getLastViewDateByEndDayHour(this.option("endDayHour"))
    }
    getCellDuration() {
        return getCellDuration(this.type, this.option("startDayHour"), this.option("endDayHour"), this.option("hoursInterval"))
    }
    getIntervalDuration(allDay) {
        return allDay ? toMs("day") : this.getCellDuration()
    }
    getVisibleDayDuration() {
        var startDayHour = this.option("startDayHour");
        var endDayHour = this.option("endDayHour");
        var hoursInterval = this.option("hoursInterval");
        return this.viewDataProvider.getVisibleDayDuration(startDayHour, endDayHour, hoursInterval)
    }
    getGroupBounds(coordinates) {
        var cellCount = this._getCellCount();
        var $cells = this._getCells();
        var cellWidth = this.getCellWidth();
        var groupedDataMap = this.viewDataProvider.groupedDataMap;
        var result = this._groupedStrategy.getGroupBoundsOffset(cellCount, $cells, cellWidth, coordinates, groupedDataMap);
        if (this._isRTL()) {
            var startOffset = result.left;
            result.left = result.right - 2 * cellWidth;
            result.right = startOffset + 2 * cellWidth
        }
        return result
    }
    needRecalculateResizableArea() {
        return this._isVerticalGroupedWorkSpace() && 0 !== this.getScrollable().scrollTop()
    }
    getCellDataByCoordinates(coordinates, allDay) {
        var key = JSON.stringify({
            top: coordinates.top,
            left: coordinates.left
        });
        return this.cache.get(key, () => {
            var $cells = this._getCells(allDay);
            var cellIndex = this.getCellIndexByCoordinates(coordinates, allDay);
            var $cell = $cells.eq(cellIndex);
            return this.getCellData($cell)
        })
    }
    getVisibleBounds() {
        var result = {};
        var $scrollable = this.getScrollable().$element();
        var cellHeight = this.getCellHeight();
        var scrolledCellCount = this.getScrollableScrollTop() / cellHeight;
        var totalCellCount = scrolledCellCount + getHeight($scrollable) / cellHeight;
        result.top = {
            hours: Math.floor(scrolledCellCount * this.option("hoursInterval")) + this.option("startDayHour"),
            minutes: scrolledCellCount % 2 ? 30 : 0
        };
        result.bottom = {
            hours: Math.floor(totalCellCount * this.option("hoursInterval")) + this.option("startDayHour"),
            minutes: Math.floor(totalCellCount) % 2 ? 30 : 0
        };
        return result
    }
    updateScrollPosition(date, groups) {
        var allDay = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : false;
        var newDate = this.timeZoneCalculator.createDate(date, {
            path: "toGrid"
        });
        var inAllDayRow = allDay && this.isAllDayPanelVisible;
        if (this.needUpdateScrollPosition(newDate, groups, inAllDayRow)) {
            this.scrollTo(newDate, groups, inAllDayRow, false)
        }
    }
    needUpdateScrollPosition(date, groups, inAllDayRow) {
        var cells = this._getCellsInViewport(inAllDayRow);
        var groupIndex = this._isGroupsSpecified(groups) ? this._getGroupIndexByResourceId(groups) : 0;
        var time = date.getTime();
        var trimmedTime = dateUtils.trimTime(date).getTime();
        return cells.reduce((currentResult, cell) => {
            var {
                startDate: cellStartDate,
                endDate: cellEndDate,
                groupIndex: cellGroupIndex
            } = this.getCellData(cell);
            var cellStartTime = cellStartDate.getTime();
            var cellEndTime = cellEndDate.getTime();
            if ((!inAllDayRow && cellStartTime <= time && time < cellEndTime || inAllDayRow && trimmedTime === cellStartTime) && groupIndex === cellGroupIndex) {
                return false
            }
            return currentResult
        }, true)
    }
    _getCellsInViewport(inAllDayRow) {
        var $scrollable = this.getScrollable().$element();
        var cellHeight = this.getCellHeight();
        var cellWidth = this.getCellWidth();
        var totalColumnCount = this._getTotalCellCount(this._getGroupCount());
        var scrollableScrollTop = this.getScrollableScrollTop();
        var scrollableScrollLeft = this.getScrollableScrollLeft();
        var fullScrolledRowCount = scrollableScrollTop / cellHeight - this.virtualScrollingDispatcher.topVirtualRowsCount;
        var scrolledRowCount = Math.floor(fullScrolledRowCount);
        if (scrollableScrollTop % cellHeight !== 0) {
            scrolledRowCount += 1
        }
        var fullScrolledColumnCount = scrollableScrollLeft / cellWidth;
        var scrolledColumnCount = Math.floor(fullScrolledColumnCount);
        if (scrollableScrollLeft % cellWidth !== 0) {
            scrolledColumnCount += 1
        }
        var rowCount = Math.floor(fullScrolledRowCount + getHeight($scrollable) / cellHeight);
        var columnCount = Math.floor(fullScrolledColumnCount + getWidth($scrollable) / cellWidth);
        var $cells = this._getAllCells(inAllDayRow);
        var result = [];
        $cells.each((function(index) {
            var $cell = $(this);
            var columnIndex = index % totalColumnCount;
            var rowIndex = index / totalColumnCount;
            if (scrolledColumnCount <= columnIndex && columnIndex < columnCount && scrolledRowCount <= rowIndex && rowIndex < rowCount) {
                result.push($cell)
            }
        }));
        return result
    }
    scrollToTime(hours, minutes, date) {
        if (!this._isValidScrollDate(date)) {
            return
        }
        var coordinates = this._getScrollCoordinates(hours, minutes, date);
        var scrollable = this.getScrollable();
        scrollable.scrollBy({
            top: coordinates.top - scrollable.scrollTop(),
            left: 0
        })
    }
    scrollTo(date, groups) {
        var allDay = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : false;
        var throwWarning = arguments.length > 3 && void 0 !== arguments[3] ? arguments[3] : true;
        if (!this._isValidScrollDate(date, throwWarning)) {
            return
        }
        var groupIndex = this._getGroupCount() && groups ? this._getGroupIndexByResourceId(groups) : 0;
        var isScrollToAllDay = allDay && this.isAllDayPanelVisible;
        var coordinates = this._getScrollCoordinates(date.getHours(), date.getMinutes(), date, groupIndex, isScrollToAllDay);
        var scrollable = this.getScrollable();
        var $scrollable = scrollable.$element();
        var cellWidth = this.getCellWidth();
        var offset = this.option("rtlEnabled") ? cellWidth : 0;
        var scrollableHeight = getHeight($scrollable);
        var scrollableWidth = getWidth($scrollable);
        var cellHeight = this.getCellHeight();
        var xShift = (scrollableWidth - cellWidth) / 2;
        var yShift = (scrollableHeight - cellHeight) / 2;
        var left = coordinates.left - scrollable.scrollLeft() - xShift - offset;
        var top = coordinates.top - scrollable.scrollTop() - yShift;
        if (isScrollToAllDay && !this._isVerticalGroupedWorkSpace()) {
            top = 0
        }
        if (this.option("templatesRenderAsynchronously")) {
            setTimeout(() => {
                scrollable.scrollBy({
                    left: left,
                    top: top
                })
            })
        } else {
            scrollable.scrollBy({
                left: left,
                top: top
            })
        }
    }
    _isValidScrollDate(date) {
        var throwWarning = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : true;
        var min = this.getStartViewDate();
        var max = this.getEndViewDate();
        if (date < min || date > max) {
            throwWarning && errors.log("W1008", date);
            return false
        }
        return true
    }
    needApplyCollectorOffset() {
        return false
    }
    removeDroppableCellClass($cellElement) {
        ($cellElement || this._getDroppableCell()).removeClass(DATE_TABLE_DROPPABLE_CELL_CLASS)
    }
    _getCoordinatesByCell($cell) {
        var columnIndex = $cell.index() - this.virtualScrollingDispatcher.leftVirtualCellsCount;
        var rowIndex = $cell.parent().index();
        var isAllDayCell = this._hasAllDayClass($cell);
        var isVerticalGrouping = this._isVerticalGroupedWorkSpace();
        if (!(isAllDayCell && !isVerticalGrouping)) {
            rowIndex -= this.virtualScrollingDispatcher.topVirtualRowsCount
        }
        return {
            rowIndex: rowIndex,
            columnIndex: columnIndex
        }
    }
    _isShowAllDayPanel() {
        return this.option("showAllDayPanel")
    }
    _getTimePanelCells() {
        return this.$element().find(".".concat(TIME_PANEL_CELL_CLASS))
    }
    _getRDateTableProps() {
        return {
            viewData: this.viewDataProvider.viewData,
            dataCellTemplate: this.option("dataCellTemplate"),
            addDateTableClass: !this.option("crossScrollingEnabled") || this.isVirtualScrolling(),
            groupOrientation: this.option("groupOrientation"),
            addVerticalSizesClassToRows: false
        }
    }
    _updateSelectedCellDataOption(selectedCellData) {
        var correctedSelectedCellData = selectedCellData.map(_ref2 => {
            var {
                startDate: startDate,
                endDate: endDate,
                allDay: allDay,
                groupIndex: groupIndex,
                groups: groups
            } = _ref2;
            return {
                startDate: startDate,
                endDate: endDate,
                allDay: allDay,
                groupIndex: groupIndex,
                groups: groups
            }
        });
        this.option("selectedCellData", correctedSelectedCellData);
        this._selectionChangedAction({
            selectedCellData: correctedSelectedCellData
        })
    }
    _getCellByData(cellData) {
        var {
            startDate: startDate,
            groupIndex: groupIndex,
            allDay: allDay,
            index: index
        } = cellData;
        var position = this.viewDataProvider.findCellPositionInMap({
            startDate: startDate,
            groupIndex: groupIndex,
            isAllDay: allDay,
            index: index
        });
        if (!position) {
            return
        }
        return allDay && !this._isVerticalGroupedWorkSpace() ? this._dom_getAllDayPanelCell(position.columnIndex) : this._dom_getDateCell(position)
    }
    getDOMElementsMetaData() {
        return this.cache.get("cellElementsMeta", () => ({
            dateTableCellsMeta: this._getDateTableDOMElementsInfo(),
            allDayPanelCellsMeta: this._getAllDayPanelDOMElementsInfo()
        }))
    }
    _getDateTableDOMElementsInfo() {
        var dateTableCells = this._getAllCells(false);
        if (!dateTableCells.length || !hasWindow()) {
            return [
                [{}]
            ]
        }
        var dateTable = this._getDateTable();
        var dateTableRect = getBoundingRect(dateTable.get(0));
        var columnsCount = this.viewDataProvider.getColumnsCount();
        var result = [];
        dateTableCells.each((index, cell) => {
            var rowIndex = Math.floor(index / columnsCount);
            if (result.length === rowIndex) {
                result.push([])
            }
            this._addCellMetaData(result[rowIndex], cell, dateTableRect)
        });
        return result
    }
    _getAllDayPanelDOMElementsInfo() {
        var result = [];
        if (this.isAllDayPanelVisible && !this._isVerticalGroupedWorkSpace() && hasWindow()) {
            var allDayCells = this._getAllCells(true);
            if (!allDayCells.length) {
                return [{}]
            }
            var allDayAppointmentContainer = this._$allDayPanel;
            var allDayPanelRect = getBoundingRect(allDayAppointmentContainer.get(0));
            allDayCells.each((_, cell) => {
                this._addCellMetaData(result, cell, allDayPanelRect)
            })
        }
        return result
    }
    _addCellMetaData(cellMetaDataArray, cell, parentRect) {
        var cellRect = getBoundingRect(cell);
        cellMetaDataArray.push({
            left: cellRect.left - parentRect.left,
            top: cellRect.top - parentRect.top,
            width: cellRect.width,
            height: cellRect.height
        })
    }
    _oldRender_getAllDayCellData(groupIndex) {
        return (cell, rowIndex, columnIndex) => {
            var validColumnIndex = columnIndex % this._getCellCount();
            var options = this._getDateGenerationOptions(true);
            var startDate = this.viewDataProvider.viewDataGenerator.getDateByCellIndices(options, rowIndex, validColumnIndex, this._getCellCountInDay());
            startDate = dateUtils.trimTime(startDate);
            var validGroupIndex = groupIndex || 0;
            if (this.isGroupedByDate()) {
                validGroupIndex = Math.floor(columnIndex % this._getGroupCount())
            } else if (this._isHorizontalGroupedWorkSpace()) {
                validGroupIndex = Math.floor(columnIndex / this._getCellCount())
            }
            var data = {
                startDate: startDate,
                endDate: startDate,
                allDay: true,
                groupIndex: validGroupIndex
            };
            var groupsArray = getCellGroups(validGroupIndex, this.option("groups"));
            if (groupsArray.length) {
                data.groups = getGroupsObjectFromGroupsArray(groupsArray)
            }
            return {
                key: CELL_DATA,
                value: data
            }
        }
    }
    renderRWorkSpace(componentsToRender) {
        var components = null !== componentsToRender && void 0 !== componentsToRender ? componentsToRender : {
            header: true,
            timePanel: true,
            dateTable: true,
            allDayPanel: true
        };
        components.header && this.renderRHeaderPanel();
        components.timePanel && this.renderRTimeTable();
        components.dateTable && this.renderRDateTable();
        components.allDayPanel && this.renderRAllDayPanel()
    }
    renderRDateTable() {
        utils.renovation.renderComponent(this, this._$dateTable, dxrDateTableLayout, "renovatedDateTable", this._getRDateTableProps())
    }
    renderRGroupPanel() {
        var options = {
            groups: this.option("groups"),
            groupOrientation: this.option("groupOrientation"),
            groupByDate: this.isGroupedByDate(),
            resourceCellTemplate: this.option("resourceCellTemplate"),
            className: this.verticalGroupTableClass,
            groupPanelData: this.viewDataProvider.getGroupPanelData(this.generateRenderOptions())
        };
        if (this.option("groups").length) {
            this._attachGroupCountClass();
            utils.renovation.renderComponent(this, this._getGroupHeaderContainer(), dxrGroupPanel, "renovatedGroupPanel", options)
        } else {
            this._detachGroupCountClass()
        }
    }
    renderRAllDayPanel() {
        var visible = this.isAllDayPanelVisible && !this.isGroupedAllDayPanel();
        if (visible) {
            var _this$virtualScrollin;
            this._toggleAllDayVisibility(false);
            var options = _extends({
                viewData: this.viewDataProvider.viewData,
                dataCellTemplate: this.option("dataCellTemplate"),
                startCellIndex: 0
            }, (null === (_this$virtualScrollin = this.virtualScrollingDispatcher.horizontalVirtualScrolling) || void 0 === _this$virtualScrollin ? void 0 : _this$virtualScrollin.getRenderState()) || {});
            utils.renovation.renderComponent(this, this._$allDayTable, dxrAllDayPanelTable, "renovatedAllDayPanel", options);
            utils.renovation.renderComponent(this, this._$allDayTitle, dxrAllDayPanelTitle, "renovatedAllDayPanelTitle", {})
        }
        this._toggleAllDayVisibility(true)
    }
    renderRTimeTable() {
        utils.renovation.renderComponent(this, this._$timePanel, dxrTimePanelTableLayout, "renovatedTimePanel", {
            timePanelData: this.viewDataProvider.timePanelData,
            timeCellTemplate: this.option("timeCellTemplate"),
            groupOrientation: this.option("groupOrientation")
        })
    }
    renderRHeaderPanel() {
        var isRenderDateHeader = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : true;
        if (this.option("groups").length) {
            this._attachGroupCountClass()
        } else {
            this._detachGroupCountClass()
        }
        utils.renovation.renderComponent(this, this._$thead, this.renovatedHeaderPanelComponent, "renovatedHeaderPanel", {
            dateHeaderData: this.viewDataProvider.dateHeaderData,
            groupPanelData: this.viewDataProvider.getGroupPanelData(this.generateRenderOptions()),
            dateCellTemplate: this.option("dateCellTemplate"),
            timeCellTemplate: this.option("timeCellTemplate"),
            groups: this.option("groups"),
            groupByDate: this.isGroupedByDate(),
            groupOrientation: this.option("groupOrientation"),
            resourceCellTemplate: this.option("resourceCellTemplate"),
            isRenderDateHeader: isRenderDateHeader
        })
    }
    initDragBehavior(scheduler) {
        if (!this.dragBehavior && scheduler) {
            this.dragBehavior = new AppointmentDragBehavior(scheduler);
            this._createDragBehavior(this.getWorkArea());
            this._createDragBehavior(this.getAllDayContainer());
            this._createDragBehavior(this._$allDayPanel)
        }
    }
    _createDragBehavior($element) {
        var options = {
            getItemData: (itemElement, appointments) => appointments._getItemData(itemElement),
            getItemSettings: $itemElement => $itemElement.data(APPOINTMENT_SETTINGS_KEY)
        };
        this._createDragBehaviorBase($element, options)
    }
    _createDragBehaviorBase($element, options) {
        var container = this.$element().find(".".concat(FIXED_CONTAINER_CLASS));
        var element = this.$element();
        var isDefaultDraggingMode = "default" === this.option("draggingMode");
        this.dragBehavior.addTo($element, createDragBehaviorConfig(container, isDefaultDraggingMode, this.dragBehavior, () => this._attachDragEvents(element), () => this._detachDragEvents(element), () => this._getDroppableCell(), () => this._getDateTables(), () => this.removeDroppableCellClass(), () => this.getCellWidth(), options))
    }
    _isRenderHeaderPanelEmptyCell() {
        return this._isVerticalGroupedWorkSpace()
    }
    _dispose() {
        super._dispose();
        this.virtualScrollingDispatcher.dispose()
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            currentDate: new Date,
            intervalCount: 1,
            startDate: null,
            firstDayOfWeek: void 0,
            startDayHour: 0,
            endDayHour: 24,
            hoursInterval: .5,
            activeStateEnabled: true,
            hoverStateEnabled: true,
            groups: [],
            showAllDayPanel: true,
            allDayExpanded: false,
            onCellClick: null,
            crossScrollingEnabled: false,
            dataCellTemplate: null,
            timeCellTemplate: null,
            resourceCellTemplate: null,
            dateCellTemplate: null,
            allowMultipleCellSelection: true,
            indicatorTime: new Date,
            indicatorUpdateInterval: 5 * toMs("minute"),
            shadeUntilCurrentTime: true,
            groupOrientation: "horizontal",
            selectedCellData: [],
            groupByDate: false,
            scrolling: {
                mode: "standard"
            },
            renovateRender: true,
            height: void 0,
            draggingMode: "outlook",
            onScrollEnd: () => {},
            getHeaderHeight: void 0,
            onVirtualScrollingUpdated: void 0,
            onSelectedCellsClick: () => {},
            timeZoneCalculator: void 0,
            schedulerHeight: void 0,
            schedulerWidth: void 0
        })
    }
    _optionChanged(args) {
        switch (args.name) {
            case "startDayHour":
                validateDayHours(args.value, this.option("endDayHour"));
                this._cleanWorkSpace();
                break;
            case "endDayHour":
                validateDayHours(this.option("startDayHour"), args.value);
                this._cleanWorkSpace();
                break;
            case "dateCellTemplate":
            case "resourceCellTemplate":
            case "dataCellTemplate":
            case "timeCellTemplate":
            case "hoursInterval":
            case "firstDayOfWeek":
            case "currentDate":
            case "startDate":
                this._cleanWorkSpace();
                break;
            case "groups":
                this._cleanView();
                this._removeAllDayElements();
                this._initGrouping();
                this.repaint();
                break;
            case "groupOrientation":
                this._initGroupedStrategy();
                this._createAllDayPanelElements();
                this._removeAllDayElements();
                this._cleanWorkSpace();
                this._toggleGroupByDateClass();
                break;
            case "showAllDayPanel":
                if (this._isVerticalGroupedWorkSpace()) {
                    this._cleanView();
                    this._removeAllDayElements();
                    this._initGrouping();
                    this.repaint()
                } else if (!this.isRenovatedRender()) {
                    this._toggleAllDayVisibility(true)
                } else {
                    this.renderWorkSpace()
                }
                break;
            case "allDayExpanded":
                this._changeAllDayVisibility();
                this._attachTablesEvents();
                this._updateScrollable();
                break;
            case "onSelectionChanged":
                this._createSelectionChangedAction();
                break;
            case "onCellClick":
                this._createCellClickAction();
                break;
            case "onCellContextMenu":
                this._attachContextMenuEvent();
                break;
            case "intervalCount":
                this._cleanWorkSpace();
                this._toggleWorkSpaceCountClass();
                break;
            case "groupByDate":
                this._cleanWorkSpace();
                this._toggleGroupByDateClass();
                break;
            case "crossScrollingEnabled":
                this._toggleHorizontalScrollClass();
                this._dateTableScrollable.option(this._dateTableScrollableConfig());
                break;
            case "width":
                super._optionChanged(args);
                this._dimensionChanged();
                break;
            case "timeZoneCalculator":
            case "allowMultipleCellSelection":
            case "selectedCellData":
                break;
            case "renovateRender":
            case "scrolling":
                this.repaint();
                break;
            case "schedulerHeight":
            case "schedulerWidth":
                this.virtualScrollingDispatcher.updateDimensions(true);
                break;
            default:
                super._optionChanged(args)
        }
    }
    _getVirtualScrollingDispatcherOptions() {
        return {
            getCellHeight: this.getCellHeight.bind(this),
            getCellWidth: this.getCellWidth.bind(this),
            getCellMinWidth: this.getCellMinWidth.bind(this),
            isRTL: this._isRTL.bind(this),
            getSchedulerHeight: () => this.option("schedulerHeight"),
            getSchedulerWidth: () => this.option("schedulerWidth"),
            getViewHeight: () => this.$element().height ? this.$element().height() : getHeight(this.$element()),
            getViewWidth: () => this.$element().width ? this.$element().width() : getWidth(this.$element()),
            getScrolling: () => this.option("scrolling"),
            getScrollableOuterWidth: this.getScrollableOuterWidth.bind(this),
            getScrollable: this.getScrollable.bind(this),
            createAction: this._createAction.bind(this),
            updateRender: this.updateRender.bind(this),
            updateGrid: this.updateGrid.bind(this),
            getGroupCount: this._getGroupCount.bind(this),
            isVerticalGrouping: this._isVerticalGroupedWorkSpace.bind(this),
            getTotalRowCount: this._getTotalRowCount.bind(this),
            getTotalCellCount: this._getTotalCellCount.bind(this)
        }
    }
    _cleanWorkSpace() {
        this._cleanView();
        this._toggleGroupedClass();
        this._toggleWorkSpaceWithOddCells();
        this.virtualScrollingDispatcher.updateDimensions(true);
        this._renderView();
        this.option("crossScrollingEnabled") && this._setTableSizes();
        this.cache.clear()
    }
    _init() {
        this._headerSemaphore = new ScrollSemaphore;
        this._sideBarSemaphore = new ScrollSemaphore;
        this._dataTableSemaphore = new ScrollSemaphore;
        this._viewDataProvider = null;
        this._cellsSelectionState = null;
        this._activeStateUnit = CELL_SELECTOR;
        super._init();
        this._initGrouping();
        this._toggleHorizontalScrollClass();
        this._toggleWorkSpaceCountClass();
        this._toggleGroupByDateClass();
        this._toggleWorkSpaceWithOddCells();
        this.$element().addClass(COMPONENT_CLASS).addClass(this._getElementClass())
    }
    _initPositionHelper() {
        this.positionHelper = new PositionHelper({
            key: this.option("key"),
            viewDataProvider: this.viewDataProvider,
            viewStartDayHour: this.option("startDayHour"),
            viewEndDayHour: this.option("endDayHour"),
            cellDuration: this.getCellDuration(),
            groupedStrategy: this._groupedStrategy,
            isGroupedByDate: this.isGroupedByDate(),
            rtlEnabled: this.option("rtlEnabled"),
            startViewDate: this.getStartViewDate(),
            isVerticalGrouping: this._isVerticalGroupedWorkSpace(),
            groupCount: this._getGroupCount(),
            isVirtualScrolling: this.isVirtualScrolling(),
            getDOMMetaDataCallback: this.getDOMElementsMetaData.bind(this)
        })
    }
    _initGrouping() {
        this._initGroupedStrategy();
        this._toggleGroupingDirectionClass();
        this._toggleGroupByDateClass()
    }
    isVerticalOrientation() {
        var orientation = this.option("groups").length ? this.option("groupOrientation") : this._getDefaultGroupStrategy();
        return "vertical" === orientation
    }
    _initGroupedStrategy() {
        var Strategy = this.isVerticalOrientation() ? VerticalGroupedStrategy : HorizontalGroupedStrategy;
        this._groupedStrategy = new Strategy(this)
    }
    _getDefaultGroupStrategy() {
        return "horizontal"
    }
    _toggleHorizontalScrollClass() {
        this.$element().toggleClass(WORKSPACE_WITH_BOTH_SCROLLS_CLASS, this.option("crossScrollingEnabled"))
    }
    _toggleGroupByDateClass() {
        this.$element().toggleClass(WORKSPACE_WITH_GROUP_BY_DATE_CLASS, this.isGroupedByDate())
    }
    _toggleWorkSpaceCountClass() {
        this.$element().toggleClass(WORKSPACE_WITH_COUNT_CLASS, this._isWorkSpaceWithCount())
    }
    _toggleWorkSpaceWithOddCells() {
        this.$element().toggleClass(WORKSPACE_WITH_ODD_CELLS_CLASS, this._isWorkspaceWithOddCells())
    }
    _toggleGroupingDirectionClass() {
        this.$element().toggleClass(VERTICAL_GROUPED_WORKSPACE_CLASS, this._isVerticalGroupedWorkSpace())
    }
    _getDateTableCellClass(rowIndex, columnIndex) {
        var cellClass = DATE_TABLE_CELL_CLASS + " " + HORIZONTAL_SIZES_CLASS + " " + VERTICAL_SIZES_CLASS;
        return this._groupedStrategy.addAdditionalGroupCellClasses(cellClass, columnIndex + 1, rowIndex, columnIndex)
    }
    _getGroupHeaderClass(i) {
        var cellClass = GROUP_HEADER_CLASS;
        return this._groupedStrategy.addAdditionalGroupCellClasses(cellClass, i + 1)
    }
    _initWorkSpaceUnits() {
        this._$headerPanelContainer = $("<div>").addClass("dx-scheduler-header-panel-container");
        this._$headerTablesContainer = $("<div>").addClass("dx-scheduler-header-tables-container");
        this._$headerPanel = $("<table>");
        this._$thead = $("<thead>").appendTo(this._$headerPanel);
        this._$headerPanelEmptyCell = $("<div>").addClass("dx-scheduler-header-panel-empty-cell");
        this._$allDayTable = $("<table>");
        this._$fixedContainer = $("<div>").addClass(FIXED_CONTAINER_CLASS);
        this._$allDayContainer = $("<div>").addClass(ALL_DAY_CONTAINER_CLASS);
        this._$dateTableScrollableContent = $("<div>").addClass("dx-scheduler-date-table-scrollable-content");
        this._$sidebarScrollableContent = $("<div>").addClass("dx-scheduler-side-bar-scrollable-content");
        this._initAllDayPanelElements();
        if (this.isRenovatedRender()) {
            this.createRAllDayPanelElements()
        } else {
            this._createAllDayPanelElements()
        }
        this._$timePanel = $("<table>").addClass(TIME_PANEL_CLASS);
        this._$dateTable = $("<table>");
        this._$dateTableContainer = $("<div>").addClass("dx-scheduler-date-table-container");
        this._$groupTable = $("<div>").addClass(WORKSPACE_VERTICAL_GROUP_TABLE_CLASS)
    }
    _initAllDayPanelElements() {
        this._allDayTitles = [];
        this._allDayTables = [];
        this._allDayPanels = []
    }
    _initDateTableScrollable() {
        var $dateTableScrollable = $("<div>").addClass(SCHEDULER_DATE_TABLE_SCROLLABLE_CLASS);
        this._dateTableScrollable = this._createComponent($dateTableScrollable, Scrollable, this._dateTableScrollableConfig())
    }
    _createWorkSpaceElements() {
        if (this.option("crossScrollingEnabled")) {
            this._createWorkSpaceScrollableElements()
        } else {
            this._createWorkSpaceStaticElements()
        }
    }
    _createWorkSpaceStaticElements() {
        this._$dateTableContainer.append(this._$dateTable);
        if (this._isVerticalGroupedWorkSpace()) {
            this._$dateTableContainer.append(this._$allDayContainer);
            this._$dateTableScrollableContent.append(this._$groupTable, this._$timePanel, this._$dateTableContainer);
            this._dateTableScrollable.$content().append(this._$dateTableScrollableContent);
            this._$headerTablesContainer.append(this._$headerPanel)
        } else {
            var _this$_$allDayPanel;
            this._$dateTableScrollableContent.append(this._$timePanel, this._$dateTableContainer);
            this._dateTableScrollable.$content().append(this._$dateTableScrollableContent);
            this._$headerTablesContainer.append(this._$headerPanel, this._$allDayPanel);
            null === (_this$_$allDayPanel = this._$allDayPanel) || void 0 === _this$_$allDayPanel ? void 0 : _this$_$allDayPanel.append(this._$allDayContainer, this._$allDayTable)
        }
        this._appendHeaderPanelEmptyCellIfNecessary();
        this._$headerPanelContainer.append(this._$headerTablesContainer);
        this.$element().append(this._$fixedContainer, this._$headerPanelContainer, this._dateTableScrollable.$element())
    }
    _createWorkSpaceScrollableElements() {
        this.$element().append(this._$fixedContainer);
        this._$flexContainer = $("<div>").addClass("dx-scheduler-work-space-flex-container");
        this._createHeaderScrollable();
        this._headerScrollable.$content().append(this._$headerPanel);
        this._appendHeaderPanelEmptyCellIfNecessary();
        this._$headerPanelContainer.append(this._$headerTablesContainer);
        this.$element().append(this._$headerPanelContainer);
        this.$element().append(this._$flexContainer);
        this._createSidebarScrollable();
        this._$flexContainer.append(this._dateTableScrollable.$element());
        this._$dateTableContainer.append(this._$dateTable);
        this._$dateTableScrollableContent.append(this._$dateTableContainer);
        this._dateTableScrollable.$content().append(this._$dateTableScrollableContent);
        if (this._isVerticalGroupedWorkSpace()) {
            this._$dateTableContainer.append(this._$allDayContainer);
            this._$sidebarScrollableContent.append(this._$groupTable, this._$timePanel)
        } else {
            var _this$_$allDayPanel2;
            this._headerScrollable.$content().append(this._$allDayPanel);
            null === (_this$_$allDayPanel2 = this._$allDayPanel) || void 0 === _this$_$allDayPanel2 ? void 0 : _this$_$allDayPanel2.append(this._$allDayContainer, this._$allDayTable);
            this._$sidebarScrollableContent.append(this._$timePanel)
        }
        this._sidebarScrollable.$content().append(this._$sidebarScrollableContent)
    }
    _appendHeaderPanelEmptyCellIfNecessary() {
        this._isRenderHeaderPanelEmptyCell() && this._$headerPanelContainer.append(this._$headerPanelEmptyCell)
    }
    _createHeaderScrollable() {
        var $headerScrollable = $("<div>").addClass(SCHEDULER_HEADER_SCROLLABLE_CLASS).appendTo(this._$headerTablesContainer);
        this._headerScrollable = this._createComponent($headerScrollable, Scrollable, this._headerScrollableConfig())
    }
    _createSidebarScrollable() {
        var $timePanelScrollable = $("<div>").addClass(SCHEDULER_SIDEBAR_SCROLLABLE_CLASS).appendTo(this._$flexContainer);
        this._sidebarScrollable = this._createComponent($timePanelScrollable, Scrollable, {
            useKeyboard: false,
            showScrollbar: "never",
            direction: "vertical",
            useNative: false,
            updateManually: true,
            bounceEnabled: false,
            onScroll: e => {
                this._sideBarSemaphore.take(e.scrollOffset);
                if (this._dataTableSemaphore.isFree(e.scrollOffset)) {
                    this._dateTableScrollable.scrollTo({
                        top: e.scrollOffset.top
                    })
                }
                this._sideBarSemaphore.release()
            }
        })
    }
    _attachTableClasses() {
        this._addTableClass(this._$dateTable, DATE_TABLE_CLASS);
        if (this._isVerticalGroupedWorkSpace()) {
            var groupCount = this._getGroupCount();
            for (var i = 0; i < groupCount; i++) {
                this._addTableClass(this._allDayTables[i], ALL_DAY_TABLE_CLASS)
            }
        } else if (!this.isRenovatedRender()) {
            this._addTableClass(this._$allDayTable, ALL_DAY_TABLE_CLASS)
        }
    }
    _attachHeaderTableClasses() {
        this._addTableClass(this._$headerPanel, HEADER_PANEL_CLASS)
    }
    _addTableClass($el, className) {
        $el && !$el.hasClass(className) && $el.addClass(className)
    }
    _initMarkup() {
        this.cache.clear();
        this._initWorkSpaceUnits();
        this._initVirtualScrolling();
        this._initDateTableScrollable();
        this._createWorkSpaceElements();
        super._initMarkup();
        if (!this.option("crossScrollingEnabled")) {
            this._attachTableClasses();
            this._attachHeaderTableClasses()
        }
        this._toggleGroupedClass();
        this._renderView();
        this._attachEvents()
    }
    _render() {
        super._render();
        this._renderDateTimeIndication();
        this._setIndicationUpdateInterval()
    }
    _toggleGroupedClass() {
        this.$element().toggleClass(GROUPED_WORKSPACE_CLASS, this._getGroupCount() > 0)
    }
    _renderView() {
        if (this.isRenovatedRender()) {
            if (this._isVerticalGroupedWorkSpace()) {
                this.renderRGroupPanel()
            }
        } else {
            this._applyCellTemplates(this._renderGroupHeader())
        }
        this.renderWorkSpace();
        this._updateGroupTableHeight();
        this.updateHeaderEmptyCellWidth();
        this._shader = new VerticalShader(this)
    }
    updateCellsSelection() {
        var renderOptions = this.generateRenderOptions();
        this.viewDataProvider.updateViewData(renderOptions);
        this.renderRWorkSpace({
            timePanel: true,
            dateTable: true,
            allDayPanel: true
        })
    }
    _renderDateTimeIndication() {
        return noop()
    }
    _setIndicationUpdateInterval() {
        return noop()
    }
    _refreshDateTimeIndication() {
        return noop()
    }
    _detachGroupCountClass() {
        [...VERTICAL_GROUP_COUNT_CLASSES].forEach(className => {
            this.$element().removeClass(className)
        })
    }
    _attachGroupCountClass() {
        var className = this._groupedStrategy.getGroupCountClass(this.option("groups"));
        this.$element().addClass(className)
    }
    _getDateHeaderTemplate() {
        return this.option("dateCellTemplate")
    }
    _toggleAllDayVisibility(isUpdateScrollable) {
        var showAllDayPanel = this._isShowAllDayPanel();
        this.$element().toggleClass(WORKSPACE_WITH_ALL_DAY_CLASS, showAllDayPanel);
        this._changeAllDayVisibility();
        isUpdateScrollable && this._updateScrollable()
    }
    _changeAllDayVisibility() {
        this.cache.clear();
        this.$element().toggleClass(WORKSPACE_WITH_COLLAPSED_ALL_DAY_CLASS, !this.option("allDayExpanded") && this._isShowAllDayPanel())
    }
    _getDateTables() {
        return this._$dateTable.add(this._$allDayTable)
    }
    _getDateTable() {
        return this._$dateTable
    }
    _removeAllDayElements() {
        this._$allDayTable && this._$allDayTable.remove();
        this._$allDayTitle && this._$allDayTitle.remove()
    }
    _cleanView() {
        var _this$_shader;
        this.cache.clear();
        this._cleanTableWidths();
        this.cellsSelectionState.clearSelectedAndFocusedCells();
        if (!this.isRenovatedRender()) {
            var _this$_$allDayTable, _this$_$sidebarTable;
            this._$thead.empty();
            this._$dateTable.empty();
            this._$timePanel.empty();
            this._$groupTable.empty();
            null === (_this$_$allDayTable = this._$allDayTable) || void 0 === _this$_$allDayTable ? void 0 : _this$_$allDayTable.empty();
            null === (_this$_$sidebarTable = this._$sidebarTable) || void 0 === _this$_$sidebarTable ? void 0 : _this$_$sidebarTable.empty()
        }
        null === (_this$_shader = this._shader) || void 0 === _this$_shader ? void 0 : _this$_shader.clean();
        delete this._interval
    }
    _clean() {
        eventsEngine.off(domAdapter.getDocument(), SCHEDULER_CELL_DXPOINTERUP_EVENT_NAME);
        this._disposeRenovatedComponents();
        super._clean()
    }
    _cleanTableWidths() {
        this._$headerPanel.css("width", "");
        this._$dateTable.css("width", "");
        this._$allDayTable && this._$allDayTable.css("width", "")
    }
    _disposeRenovatedComponents() {
        var _this$renovatedAllDay, _this$renovatedDateTa, _this$renovatedTimePa, _this$renovatedGroupP, _this$renovatedHeader;
        null === (_this$renovatedAllDay = this.renovatedAllDayPanel) || void 0 === _this$renovatedAllDay ? void 0 : _this$renovatedAllDay.dispose();
        this.renovatedAllDayPanel = void 0;
        null === (_this$renovatedDateTa = this.renovatedDateTable) || void 0 === _this$renovatedDateTa ? void 0 : _this$renovatedDateTa.dispose();
        this.renovatedDateTable = void 0;
        null === (_this$renovatedTimePa = this.renovatedTimePanel) || void 0 === _this$renovatedTimePa ? void 0 : _this$renovatedTimePa.dispose();
        this.renovatedTimePanel = void 0;
        null === (_this$renovatedGroupP = this.renovatedGroupPanel) || void 0 === _this$renovatedGroupP ? void 0 : _this$renovatedGroupP.dispose();
        this.renovatedGroupPanel = void 0;
        null === (_this$renovatedHeader = this.renovatedHeaderPanel) || void 0 === _this$renovatedHeader ? void 0 : _this$renovatedHeader.dispose();
        this.renovatedHeaderPanel = void 0
    }
    getGroupedStrategy() {
        return this._groupedStrategy
    }
    getFixedContainer() {
        return this._$fixedContainer
    }
    getAllDayContainer() {
        return this._$allDayContainer
    }
    updateRender() {
        this.renderer.updateRender()
    }
    updateGrid() {
        this.renderer._renderGrid()
    }
    updateAppointments() {
        var _this$dragBehavior;
        this.option("onVirtualScrollingUpdated")();
        null === (_this$dragBehavior = this.dragBehavior) || void 0 === _this$dragBehavior ? void 0 : _this$dragBehavior.updateDragSource()
    }
    _createAllDayPanelElements() {
        var groupCount = this._getGroupCount();
        if (this._isVerticalGroupedWorkSpace() && 0 !== groupCount) {
            for (var i = 0; i < groupCount; i++) {
                var $allDayTitle = $("<div>").addClass(ALL_DAY_TITLE_CLASS).text(messageLocalization.format("dxScheduler-allDay"));
                this._allDayTitles.push($allDayTitle);
                this._$allDayTable = $("<table>");
                this._allDayTables.push(this._$allDayTable);
                this._$allDayPanel = $("<div>").addClass(ALL_DAY_PANEL_CLASS).append(this._$allDayTable);
                this._allDayPanels.push(this._$allDayPanel)
            }
        } else {
            this._$allDayTitle = $("<div>").addClass(ALL_DAY_TITLE_CLASS).text(messageLocalization.format("dxScheduler-allDay")).appendTo(this.$element());
            this._$allDayTable = $("<table>");
            this._$allDayPanel = $("<div>").addClass(ALL_DAY_PANEL_CLASS).append(this._$allDayTable)
        }
    }
    renderWorkSpace() {
        var isGenerateNewViewData = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : true;
        this.cache.clear();
        this.viewDataProvider.update(this.generateRenderOptions(), isGenerateNewViewData);
        if (this.isRenovatedRender()) {
            this.renderRWorkSpace();
            this.virtualScrollingDispatcher.updateDimensions()
        } else {
            this._renderDateHeader();
            this._renderTimePanel();
            this._renderGroupAllDayPanel();
            this._renderDateTable();
            this._renderAllDayPanel()
        }
        this._initPositionHelper()
    }
    _renderGroupHeader() {
        var $container = this._getGroupHeaderContainer();
        var groupCount = this._getGroupCount();
        var cellTemplates = [];
        if (groupCount) {
            var groupRows = this._makeGroupRows(this.option("groups"), this.option("groupByDate"));
            this._attachGroupCountClass();
            $container.append(groupRows.elements);
            cellTemplates = groupRows.cellTemplates
        } else {
            this._detachGroupCountClass()
        }
        return cellTemplates
    }
    _applyCellTemplates(templates) {
        null === templates || void 0 === templates ? void 0 : templates.forEach((function(template) {
            template()
        }))
    }
    _makeGroupRows(groups, groupByDate) {
        var tableCreatorStrategy = this._isVerticalGroupedWorkSpace() ? tableCreator.VERTICAL : tableCreator.HORIZONTAL;
        return tableCreator.makeGroupedTable(tableCreatorStrategy, groups, {
            groupHeaderRowClass: GROUP_ROW_CLASS,
            groupRowClass: GROUP_ROW_CLASS,
            groupHeaderClass: this._getGroupHeaderClass.bind(this),
            groupHeaderContentClass: GROUP_HEADER_CONTENT_CLASS
        }, this._getCellCount() || 1, this.option("resourceCellTemplate"), this._getGroupCount(), groupByDate)
    }
    _renderDateHeader() {
        var container = this._getDateHeaderContainer();
        var $headerRow = $("<tr>").addClass(HEADER_ROW_CLASS);
        var count = this._getCellCount();
        var cellTemplate = this._getDateHeaderTemplate();
        var repeatCount = this._getCalculateHeaderCellRepeatCount();
        var templateCallbacks = [];
        var groupByDate = this.isGroupedByDate();
        if (!groupByDate) {
            for (var rowIndex = 0; rowIndex < repeatCount; rowIndex++) {
                for (var columnIndex = 0; columnIndex < count; columnIndex++) {
                    var templateIndex = rowIndex * count + columnIndex;
                    this._renderDateHeaderTemplate($headerRow, columnIndex, templateIndex, cellTemplate, templateCallbacks)
                }
            }
            container.append($headerRow)
        } else {
            var colSpan = groupByDate ? this._getGroupCount() : 1;
            for (var _columnIndex = 0; _columnIndex < count; _columnIndex++) {
                var _templateIndex = _columnIndex * repeatCount;
                var cellElement = this._renderDateHeaderTemplate($headerRow, _columnIndex, _templateIndex, cellTemplate, templateCallbacks);
                cellElement.attr("colSpan", colSpan)
            }
            container.prepend($headerRow)
        }
        this._applyCellTemplates(templateCallbacks);
        return $headerRow
    }
    _renderDateHeaderTemplate(container, panelCellIndex, templateIndex, cellTemplate, templateCallbacks) {
        var validTemplateIndex = this.isGroupedByDate() ? Math.floor(templateIndex / this._getGroupCount()) : templateIndex;
        var completeDateHeaderMap = this.viewDataProvider.completeDateHeaderMap;
        var {
            text: text,
            startDate: date
        } = completeDateHeaderMap[completeDateHeaderMap.length - 1][validTemplateIndex];
        var $cell = $("<th>").addClass(this._getHeaderPanelCellClass(panelCellIndex)).attr("title", text);
        if (null !== cellTemplate && void 0 !== cellTemplate && cellTemplate.render) {
            templateCallbacks.push(cellTemplate.render.bind(cellTemplate, {
                model: _extends({
                    text: text,
                    date: date
                }, this._getGroupsForDateHeaderTemplate(templateIndex)),
                index: templateIndex,
                container: getPublicElement($cell)
            }))
        } else {
            $cell.text(text)
        }
        container.append($cell);
        return $cell
    }
    _getGroupsForDateHeaderTemplate(templateIndex) {
        var indexMultiplier = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : 1;
        var groupIndex;
        var groups;
        if (this._isHorizontalGroupedWorkSpace() && !this.isGroupedByDate()) {
            groupIndex = this._getGroupIndex(0, templateIndex * indexMultiplier);
            var groupsArray = getCellGroups(groupIndex, this.option("groups"));
            groups = getGroupsObjectFromGroupsArray(groupsArray)
        }
        return {
            groups: groups,
            groupIndex: groupIndex
        }
    }
    _getHeaderPanelCellClass(i) {
        var cellClass = HEADER_PANEL_CELL_CLASS + " " + HORIZONTAL_SIZES_CLASS;
        return this._groupedStrategy.addAdditionalGroupCellClasses(cellClass, i + 1, void 0, void 0, this.isGroupedByDate())
    }
    _renderAllDayPanel(index) {
        var cellCount = this._getCellCount();
        if (!this._isVerticalGroupedWorkSpace()) {
            cellCount *= this._getGroupCount() || 1
        }
        var cellTemplates = this._renderTableBody({
            container: this._allDayPanels.length ? getPublicElement(this._allDayTables[index]) : getPublicElement(this._$allDayTable),
            rowCount: 1,
            cellCount: cellCount,
            cellClass: this._getAllDayPanelCellClass.bind(this),
            rowClass: ALL_DAY_TABLE_ROW_CLASS,
            cellTemplate: this.option("dataCellTemplate"),
            getCellData: this._oldRender_getAllDayCellData(index),
            groupIndex: index
        }, true);
        this._toggleAllDayVisibility(true);
        this._applyCellTemplates(cellTemplates)
    }
    _renderGroupAllDayPanel() {
        if (this._isVerticalGroupedWorkSpace()) {
            var groupCount = this._getGroupCount();
            for (var i = 0; i < groupCount; i++) {
                this._renderAllDayPanel(i)
            }
        }
    }
    _getAllDayPanelCellClass(i, j) {
        var cellClass = ALL_DAY_TABLE_CELL_CLASS + " " + HORIZONTAL_SIZES_CLASS;
        return this._groupedStrategy.addAdditionalGroupCellClasses(cellClass, j + 1)
    }
    _renderTimePanel() {
        var repeatCount = this._groupedStrategy.calculateTimeCellRepeatCount();
        var getData = (rowIndex, field) => {
            var allDayPanelsCount = 0;
            if (this.isAllDayPanelVisible) {
                allDayPanelsCount = 1
            }
            if (this.isGroupedAllDayPanel()) {
                allDayPanelsCount = Math.ceil((rowIndex + 1) / this._getRowCount())
            }
            var validRowIndex = rowIndex + allDayPanelsCount;
            return this.viewDataProvider.completeTimePanelMap[validRowIndex][field]
        };
        this._renderTableBody({
            container: getPublicElement(this._$timePanel),
            rowCount: this._getTimePanelRowCount() * repeatCount,
            cellCount: 1,
            cellClass: this._getTimeCellClass.bind(this),
            rowClass: TIME_PANEL_ROW_CLASS,
            cellTemplate: this.option("timeCellTemplate"),
            getCellText: rowIndex => getData(rowIndex, "text"),
            getCellDate: rowIndex => getData(rowIndex, "startDate"),
            groupCount: this._getGroupCount(),
            allDayElements: this._insertAllDayRowsIntoDateTable() ? this._allDayTitles : void 0,
            getTemplateData: (rowIndex => {
                if (!this._isVerticalGroupedWorkSpace()) {
                    return {}
                }
                var groupIndex = this._getGroupIndex(rowIndex, 0);
                var groupsArray = getCellGroups(groupIndex, this.option("groups"));
                var groups = getGroupsObjectFromGroupsArray(groupsArray);
                return {
                    groupIndex: groupIndex,
                    groups: groups
                }
            }).bind(this)
        })
    }
    _getTimeCellClass(i) {
        var cellClass = TIME_PANEL_CELL_CLASS + " " + VERTICAL_SIZES_CLASS;
        return this._isVerticalGroupedWorkSpace() ? this._groupedStrategy.addAdditionalGroupCellClasses(cellClass, i, i) : cellClass
    }
    _renderDateTable() {
        var groupCount = this._getGroupCount();
        this._renderTableBody({
            container: getPublicElement(this._$dateTable),
            rowCount: this._getTotalRowCount(groupCount),
            cellCount: this._getTotalCellCount(groupCount),
            cellClass: this._getDateTableCellClass.bind(this),
            rowClass: DATE_TABLE_ROW_CLASS,
            cellTemplate: this.option("dataCellTemplate"),
            getCellData: (_, rowIndex, columnIndex) => {
                var isGroupedAllDayPanel = this.isGroupedAllDayPanel();
                var validRowIndex = rowIndex;
                if (isGroupedAllDayPanel) {
                    var rowCount = this._getRowCount();
                    var allDayPanelsCount = Math.ceil(rowIndex / rowCount);
                    validRowIndex += allDayPanelsCount
                }
                var cellData = this.viewDataProvider.viewDataMap.dateTableMap[validRowIndex][columnIndex].cellData;
                return {
                    value: this._filterCellDataFields(cellData),
                    fullValue: cellData,
                    key: CELL_DATA
                }
            },
            allDayElements: this._insertAllDayRowsIntoDateTable() ? this._allDayPanels : void 0,
            groupCount: groupCount,
            groupByDate: this.option("groupByDate")
        })
    }
    _insertAllDayRowsIntoDateTable() {
        return this._groupedStrategy.insertAllDayRowsIntoDateTable()
    }
    _renderTableBody(options, delayCellTemplateRendering) {
        var result = [];
        if (!delayCellTemplateRendering) {
            this._applyCellTemplates(tableCreator.makeTable(options))
        } else {
            result = tableCreator.makeTable(options)
        }
        return result
    }
}
var createDragBehaviorConfig = (container, isDefaultDraggingMode, dragBehavior, attachGeneralEvents, detachGeneralEvents, getDroppableCell, getDateTables, removeDroppableCellClass, getCellWidth, options) => {
    var state = {
        dragElement: void 0,
        itemData: void 0
    };
    var isItemDisabled = () => {
        var {
            itemData: itemData
        } = state;
        if (itemData) {
            var getter = compileGetter("disabled");
            return getter(itemData)
        }
        return true
    };
    var cursorOffset = options.isSetCursorOffset ? () => {
        var $dragElement = $(state.dragElement);
        return {
            x: getWidth($dragElement) / 2,
            y: getHeight($dragElement) / 2
        }
    } : void 0;
    return {
        container: container,
        dragTemplate: () => state.dragElement,
        onDragStart: e => {
            if (!isDefaultDraggingMode) {
                detachGeneralEvents()
            }
            var canceled = e.cancel;
            var event = e.event;
            var $itemElement = $(e.itemElement);
            var appointments = e.component._appointments;
            state.itemData = options.getItemData(e.itemElement, appointments);
            var settings = options.getItemSettings($itemElement, e);
            var initialPosition = options.initialPosition;
            if (!isItemDisabled()) {
                event.data = event.data || {};
                if (!canceled) {
                    if (!settings.isCompact) {
                        dragBehavior.updateDragSource(state.itemData, settings)
                    }
                    state.dragElement = ((itemData, settings, appointments) => {
                        var appointmentIndex = appointments.option("items").length;
                        settings.isCompact = false;
                        settings.virtual = false;
                        var items = appointments._renderItem(appointmentIndex, {
                            itemData: itemData,
                            settings: [settings]
                        });
                        return items[0]
                    })(state.itemData, settings, appointments);
                    event.data.itemElement = state.dragElement;
                    event.data.initialPosition = null !== initialPosition && void 0 !== initialPosition ? initialPosition : locate($(state.dragElement));
                    event.data.itemData = state.itemData;
                    event.data.itemSettings = settings;
                    dragBehavior.onDragStart(event.data);
                    resetPosition($(state.dragElement))
                }
            }
        },
        onDragMove: () => {
            if (isDefaultDraggingMode) {
                return
            }
            var appointmentWidth = getWidth(state.dragElement);
            var cellWidth = getCellWidth();
            var isWideAppointment = appointmentWidth > cellWidth;
            var dragElementContainer = $(state.dragElement).parent();
            var boundingRect = getBoundingRect(dragElementContainer.get(0));
            var newX = boundingRect.left + 10;
            var newY = boundingRect.top + 10;
            var elements = isWideAppointment ? domAdapter.elementsFromPoint(newX, newY) : domAdapter.elementsFromPoint(newX + appointmentWidth / 2, newY);
            var dateTables = getDateTables();
            var droppableCell = elements.find(el => {
                var classList = el.classList;
                var isCurrentSchedulerElement = 1 === dateTables.find(el).length;
                return isCurrentSchedulerElement && (classList.contains(DATE_TABLE_CELL_CLASS) || classList.contains(ALL_DAY_TABLE_CELL_CLASS))
            });
            if (droppableCell) {
                var oldDroppableCell = getDroppableCell();
                if (!oldDroppableCell.is(droppableCell)) {
                    removeDroppableCellClass()
                }
                $(droppableCell).addClass(DATE_TABLE_DROPPABLE_CELL_CLASS)
            }
        },
        onDragEnd: e => {
            var _state$dragElement;
            if (!isDefaultDraggingMode) {
                attachGeneralEvents()
            }
            if (!isItemDisabled()) {
                dragBehavior.onDragEnd(e)
            }
            null === (_state$dragElement = state.dragElement) || void 0 === _state$dragElement ? void 0 : _state$dragElement.remove();
            removeDroppableCellClass()
        },
        cursorOffset: cursorOffset,
        filter: options.filter
    }
};
export default SchedulerWorkSpace;
