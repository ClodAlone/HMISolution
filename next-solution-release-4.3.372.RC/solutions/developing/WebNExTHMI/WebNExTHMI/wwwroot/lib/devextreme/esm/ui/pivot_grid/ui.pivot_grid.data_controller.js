/**
 * DevExtreme (esm/ui/pivot_grid/ui.pivot_grid.data_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Callbacks from "../../core/utils/callbacks";
import {
    when,
    Deferred
} from "../../core/utils/deferred";
import {
    extend
} from "../../core/utils/extend";
import {
    inArray
} from "../../core/utils/array";
import {
    map,
    each
} from "../../core/utils/iterator";
import Class from "../../core/class";
import {
    format
} from "../../core/utils/string";
import {
    deferUpdate
} from "../../core/utils/common";
import {
    isDefined,
    isString
} from "../../core/utils/type";
import {
    VirtualScrollController
} from "../grid_core/ui.grid_core.virtual_scrolling_core";
import {
    foreachColumnInfo,
    createColumnsInfo
} from "../grid_core/ui.grid_core.virtual_columns_core";
import {
    StateStoringController
} from "../grid_core/ui.grid_core.state_storing_core";
import PivotGridDataSource from "./data_source";
import {
    findField,
    foreachTree,
    foreachTreeAsync,
    createPath,
    formatValue
} from "./ui.pivot_grid.utils";
var math = Math;
var GRAND_TOTAL_TYPE = "GT";
var TOTAL_TYPE = "T";
var DATA_TYPE = "D";
var NOT_AVAILABLE = "#N/A";
var CHANGING_DURATION_IF_PAGINATE = 300;
var proxyMethod = function(instance, methodName, defaultResult) {
    if (!instance[methodName]) {
        instance[methodName] = function() {
            var dataSource = this._dataSource;
            return dataSource ? dataSource[methodName].apply(dataSource, arguments) : defaultResult
        }
    }
};
export var DataController = Class.inherit(function() {
    function formatCellValue(value, dataField, errorText) {
        return value === NOT_AVAILABLE ? errorText : formatValue(value, dataField)
    }
    var createHeaderInfo = function() {
        var addInfoItem = function(info, options) {
            var breadth = options.lastIndex - options.index || 1;
            var itemInfo = function(headerItem, breadth, isHorizontal, isTree) {
                var infoItem = {
                    type: headerItem.type,
                    text: headerItem.text
                };
                if (headerItem.path) {
                    infoItem.path = headerItem.path
                }
                if (headerItem.width) {
                    infoItem.width = headerItem.width
                }
                if (isDefined(headerItem.wordWrapEnabled)) {
                    infoItem.wordWrapEnabled = headerItem.wordWrapEnabled
                }
                if (headerItem.isLast) {
                    infoItem.isLast = true
                }
                if (headerItem.sorted) {
                    infoItem.sorted = true
                }
                if (headerItem.isMetric) {
                    infoItem.dataIndex = headerItem.dataIndex
                }
                if (isDefined(headerItem.expanded)) {
                    infoItem.expanded = headerItem.expanded
                }
                if (breadth > 1) {
                    infoItem[isHorizontal ? "colspan" : "rowspan"] = breadth
                }
                if (headerItem.depthSize && headerItem.depthSize > 1) {
                    infoItem[isHorizontal ? "rowspan" : "colspan"] = headerItem.depthSize
                }
                if (headerItem.index >= 0) {
                    infoItem.dataSourceIndex = headerItem.index
                }
                if (isTree && headerItem.children && headerItem.children.length && !headerItem.children[0].isMetric) {
                    infoItem.width = null;
                    infoItem.isWhiteSpace = true
                }
                return infoItem
            }(options.headerItem, breadth, options.isHorizontal, options.isTree);
            ! function(info, infoItem, itemIndex, depthIndex, isHorizontal) {
                var index = isHorizontal ? depthIndex : itemIndex;
                while (!info[index]) {
                    info.push([])
                }
                if (isHorizontal) {
                    info[index].push(infoItem)
                } else {
                    info[index].unshift(infoItem)
                }
            }(info, itemInfo, options.index, options.depth, options.isHorizontal);
            if (!options.headerItem.children || 0 === options.headerItem.children.length) {
                return options.lastIndex + 1
            }
            return options.lastIndex
        };
        var getViewHeaderItems = function(headerItems, headerDescriptions, cellDescriptions, depthSize, options) {
            var cellDescriptionsCount = cellDescriptions.length;
            var viewHeaderItems = function(headerItems, headerDescriptions) {
                var headerDescriptionsCount = headerDescriptions && headerDescriptions.length || 0;
                var childrenStack = [];
                var d = new Deferred;
                var headerItem;
                when(foreachTreeAsync(headerItems, (function(items, index) {
                    var item = items[0];
                    var path = createPath(items);
                    headerItem = createHeaderItem(childrenStack, path.length, index);
                    headerItem.type = DATA_TYPE;
                    headerItem.value = item.value;
                    headerItem.path = path;
                    headerItem.text = item.text;
                    headerItem.index = item.index;
                    headerItem.displayText = item.displayText;
                    headerItem.key = item.key;
                    headerItem.isEmpty = item.isEmpty;
                    if (path.length < headerDescriptionsCount && (!item.children || 0 !== item.children.length)) {
                        headerItem.expanded = !!item.children
                    }
                }))).done((function() {
                    d.resolve(createHeaderItem(childrenStack, 0, 0).children || [])
                }));
                return d
            }(headerItems, headerDescriptions);
            var dataFields = options.dataFields;
            var d = new Deferred;
            when(viewHeaderItems).done((function(viewHeaderItems) {
                options.notifyProgress(.5);
                if (options.showGrandTotals) {
                    viewHeaderItems[!options.showTotalsPrior ? "push" : "unshift"]({
                        type: GRAND_TOTAL_TYPE,
                        isEmpty: options.isEmptyGrandTotal
                    })
                }
                var hideTotals = false === options.showTotals || dataFields.length > 0 && dataFields.length === options.hiddenTotals.length;
                var hideData = dataFields.length > 0 && options.hiddenValues.length === dataFields.length;
                if (hideData && hideTotals) {
                    depthSize = 1
                }
                if (!hideTotals || "tree" === options.layout) {
                    ! function(headerItems, headerDescriptions, showTotalsPrior, isTree) {
                        showTotalsPrior = showTotalsPrior || isTree;
                        foreachTree(headerItems, (function(items, index) {
                            var item = items[0];
                            var parentChildren = (items[1] ? items[1].children : headerItems) || [];
                            var dataField = headerDescriptions[items.length - 1];
                            if (item.type === DATA_TYPE && item.expanded && (false !== dataField.showTotals || isTree)) {
                                -1 !== index && parentChildren.splice(showTotalsPrior ? index : index + 1, 0, extend({}, item, {
                                    children: null,
                                    type: TOTAL_TYPE,
                                    expanded: showTotalsPrior ? true : null,
                                    isAdditionalTotal: true
                                }));
                                if (showTotalsPrior) {
                                    item.expanded = null
                                }
                            }
                        }))
                    }(viewHeaderItems, headerDescriptions, options.showTotalsPrior, "tree" === options.layout)
                }
                when(foreachTreeAsync(viewHeaderItems, (function(items) {
                    var item = items[0];
                    if (!item.children || 0 === item.children.length) {
                        item.depthSize = depthSize - items.length + 1
                    }
                }))).done((function() {
                    if (cellDescriptionsCount > 1) {
                        ! function(headerItems, cellDescriptions, options) {
                            foreachTree(headerItems, (function(items) {
                                var item = items[0];
                                var i;
                                if (!item.children || 0 === item.children.length) {
                                    item.children = [];
                                    for (i = 0; i < cellDescriptions.length; i++) {
                                        var isGrandTotal = item.type === GRAND_TOTAL_TYPE;
                                        var isTotal = item.type === TOTAL_TYPE;
                                        var isValue = item.type === DATA_TYPE;
                                        var columnIsHidden = false === cellDescriptions[i].visible || isGrandTotal && -1 !== inArray(i, options.hiddenGrandTotals) || isTotal && -1 !== inArray(i, options.hiddenTotals) || isValue && -1 !== inArray(i, options.hiddenValues);
                                        if (columnIsHidden) {
                                            continue
                                        }
                                        item.children.push({
                                            caption: cellDescriptions[i].caption,
                                            path: item.path,
                                            type: item.type,
                                            value: i,
                                            index: item.index,
                                            dataIndex: i,
                                            isMetric: true,
                                            isEmpty: item.isEmpty && item.isEmpty[i]
                                        })
                                    }
                                }
                            }))
                        }(viewHeaderItems, cellDescriptions, options)
                    }!options.showEmpty && function(headerItems) {
                        foreachTree([{
                            children: headerItems
                        }], (function(items, index) {
                            var item = items[0];
                            var parentChildren = (items[1] ? items[1].children : headerItems) || [];
                            var isEmpty = item.isEmpty;
                            if (isEmpty && isEmpty.length) {
                                isEmpty = item.isEmpty.filter((function(isEmpty) {
                                    return isEmpty
                                })).length === isEmpty.length
                            }
                            if (item && !item.children && isEmpty) {
                                parentChildren.splice(index, 1);
                                ! function removeEmptyParent(items, index) {
                                    var parent = items[index + 1];
                                    if (!items[index].children.length && parent && parent.children) {
                                        parent.children.splice(inArray(items[index], parent.children), 1);
                                        removeEmptyParent(items, index + 1)
                                    }
                                }(items, 1)
                            }
                        }))
                    }(viewHeaderItems);
                    options.notifyProgress(.75);
                    when(foreachTreeAsync(viewHeaderItems, (function(items) {
                        var item = items[0];
                        var isMetric = item.isMetric;
                        var field = headerDescriptions[items.length - 1] || {};
                        if (item.type === DATA_TYPE && !isMetric) {
                            item.width = field.width
                        }
                        if (true === hideData && item.type === DATA_TYPE) {
                            var parentChildren = (items[1] ? items[1].children : viewHeaderItems) || [];
                            parentChildren.splice(inArray(item, parentChildren), 1);
                            return
                        }
                        if (isMetric) {
                            item.wordWrapEnabled = cellDescriptions[item.dataIndex].wordWrapEnabled
                        } else {
                            item.wordWrapEnabled = field.wordWrapEnabled
                        }
                        item.isLast = !item.children || !item.children.length;
                        if (item.isLast) {
                            each(options.sortBySummaryPaths, (function(index, sortBySummaryPath) {
                                if (!isDefined(item.dataIndex)) {
                                    sortBySummaryPath = sortBySummaryPath.slice(0);
                                    sortBySummaryPath.pop()
                                }
                                if (function(items, sortBySummaryPath) {
                                        var path;
                                        var item = items[0];
                                        var stringValuesUsed = isString(sortBySummaryPath[0]);
                                        var headerItem = item.dataIndex >= 0 ? items[1] : item;
                                        if (stringValuesUsed && -1 !== sortBySummaryPath[0].indexOf("&[") && headerItem.key || !headerItem.key) {
                                            path = createPath(items)
                                        } else {
                                            path = map(items, (function(item) {
                                                return item.dataIndex >= 0 ? item.value : item.text
                                            })).reverse()
                                        }
                                        if (item.type === GRAND_TOTAL_TYPE) {
                                            path = path.slice(1)
                                        }
                                        return path.join("/") === sortBySummaryPath.join("/")
                                    }(items, sortBySummaryPath)) {
                                    item.sorted = true;
                                    return false
                                }
                            }))
                        }
                        item.text = function(item, description, options) {
                            var text = item.text;
                            if (isDefined(item.displayText)) {
                                text = item.displayText
                            } else if (isDefined(item.caption)) {
                                text = item.caption
                            } else if (item.type === GRAND_TOTAL_TYPE) {
                                text = options.texts.grandTotal
                            }
                            if (item.isAdditionalTotal) {
                                text = format(options.texts.total || "", text)
                            }
                            return text
                        }(item, 0, options)
                    }))).done((function() {
                        if (!viewHeaderItems.length) {
                            viewHeaderItems.push({})
                        }
                        options.notifyProgress(1);
                        d.resolve(viewHeaderItems)
                    }))
                }))
            }));
            return d
        };

        function createHeaderItem(childrenStack, depth, index) {
            var parent = childrenStack[depth] = childrenStack[depth] || [];
            var node = parent[index] = {};
            if (childrenStack[depth + 1]) {
                node.children = childrenStack[depth + 1];
                for (var i = depth + 1; i < childrenStack.length; i++) {
                    childrenStack[i] = void 0
                }
                childrenStack.length = depth + 1
            }
            return node
        }
        return function(headerItems, headerDescriptions, cellDescriptions, isHorizontal, options) {
            var info = [];
            var depthSize = function(headerItems) {
                var depth = 0;
                foreachTree(headerItems, (function(items) {
                    depth = math.max(depth, items.length)
                }));
                return depth
            }(headerItems) || 1;
            var d = new Deferred;
            getViewHeaderItems(headerItems, headerDescriptions, cellDescriptions, depthSize, options).done((function(viewHeaderItems) {
                ! function(info, viewHeaderItems, depthSize, isHorizontal, isTree) {
                    var lastIndex = 0;
                    var index;
                    var depth;
                    var indexesByDepth = [0];
                    foreachTree(viewHeaderItems, (function(items) {
                        var headerItem = items[0];
                        depth = headerItem.isMetric ? depthSize : items.length - 1;
                        while (indexesByDepth.length - 1 < depth) {
                            indexesByDepth.push(indexesByDepth[indexesByDepth.length - 1])
                        }
                        index = indexesByDepth[depth] || 0;
                        lastIndex = addInfoItem(info, {
                            headerItem: headerItem,
                            index: index,
                            lastIndex: lastIndex,
                            depth: depth,
                            isHorizontal: isHorizontal,
                            isTree: isTree
                        });
                        indexesByDepth.length = depth;
                        indexesByDepth.push(lastIndex)
                    }))
                }(info, viewHeaderItems, depthSize, isHorizontal, "tree" === options.layout);
                options.notifyProgress(1);
                d.resolve(info)
            }));
            return d
        }
    }();

    function createSortPaths(headerFields, dataFields) {
        var sortBySummaryPaths = [];
        each(headerFields, (function(index, headerField) {
            var fieldIndex = findField(dataFields, headerField.sortBySummaryField);
            if (fieldIndex >= 0) {
                sortBySummaryPaths.push((headerField.sortBySummaryPath || []).concat([fieldIndex]))
            }
        }));
        return sortBySummaryPaths
    }

    function foreachRowInfo(rowsInfo, callback) {
        var columnOffset = 0;
        var columnOffsetResetIndexes = [];
        for (var i = 0; i < rowsInfo.length; i++) {
            for (var j = 0; j < rowsInfo[i].length; j++) {
                var rowSpanOffset = (rowsInfo[i][j].rowspan || 1) - 1;
                var visibleIndex = i + rowSpanOffset;
                if (columnOffsetResetIndexes[i]) {
                    columnOffset -= columnOffsetResetIndexes[i];
                    columnOffsetResetIndexes[i] = 0
                }
                if (false === callback(rowsInfo[i][j], visibleIndex, i, j, columnOffset)) {
                    break
                }
                columnOffsetResetIndexes[i + (rowsInfo[i][j].rowspan || 1)] = (columnOffsetResetIndexes[i + (rowsInfo[i][j].rowspan || 1)] || 0) + 1;
                columnOffset++
            }
        }
    }

    function getHeaderIndexedItems(headerItems, options) {
        var visibleIndex = 0;
        var indexedItems = [];
        foreachTree(headerItems, (function(items) {
            var headerItem = items[0];
            var path = createPath(items);
            if (headerItem.children && false === options.showTotals) {
                return
            }
            var indexedItem = extend(true, {}, headerItem, {
                visibleIndex: visibleIndex++,
                path: path
            });
            if (isDefined(indexedItem.index)) {
                indexedItems[indexedItem.index] = indexedItem
            } else {
                indexedItems.push(indexedItem)
            }
        }));
        return indexedItems
    }

    function createScrollController(dataController, component, dataAdapter) {
        return new VirtualScrollController(component, extend({
            hasKnownLastPage: function() {
                return true
            },
            pageCount: function() {
                return math.ceil(this.totalItemsCount() / this.pageSize())
            },
            updateLoading: function() {},
            itemsCount: function() {
                if (this.pageIndex() < this.pageCount() - 1) {
                    return this.pageSize()
                } else {
                    return this.totalItemsCount() % this.pageSize()
                }
            },
            items: function() {
                return []
            },
            viewportItems: function() {
                return []
            },
            onChanged: function() {},
            isLoading: function() {
                return dataController.isLoading()
            },
            changingDuration: function() {
                var dataSource = dataController._dataSource;
                if (dataSource.paginate()) {
                    return CHANGING_DURATION_IF_PAGINATE
                }
                return dataController._changingDuration || 0
            }
        }, dataAdapter))
    }
    var members = {
        ctor: function(options) {
            var that = this;
            var virtualScrollControllerChanged = that._fireChanged.bind(that);
            options = that._options = options || {};
            that.dataSourceChanged = Callbacks();
            that._dataSource = that._createDataSource(options);
            if (options.component && "virtual" === options.component.option("scrolling.mode")) {
                that._rowsScrollController = createScrollController(that, options.component, {
                    totalItemsCount: function() {
                        return that.totalRowCount()
                    },
                    pageIndex: function(index) {
                        return that.rowPageIndex(index)
                    },
                    pageSize: function() {
                        return that.rowPageSize()
                    },
                    load: function() {
                        if (that._rowsScrollController.pageIndex() >= this.pageCount()) {
                            that._rowsScrollController.pageIndex(this.pageCount() - 1)
                        }
                        return that._rowsScrollController.handleDataChanged((function() {
                            if (that._dataSource.paginate()) {
                                that._dataSource.load()
                            } else {
                                virtualScrollControllerChanged.apply(this, arguments)
                            }
                        }))
                    }
                });
                that._columnsScrollController = createScrollController(that, options.component, {
                    totalItemsCount: function() {
                        return that.totalColumnCount()
                    },
                    pageIndex: function(index) {
                        return that.columnPageIndex(index)
                    },
                    pageSize: function() {
                        return that.columnPageSize()
                    },
                    load: function() {
                        if (that._columnsScrollController.pageIndex() >= this.pageCount()) {
                            that._columnsScrollController.pageIndex(this.pageCount() - 1)
                        }
                        return that._columnsScrollController.handleDataChanged((function() {
                            if (that._dataSource.paginate()) {
                                that._dataSource.load()
                            } else {
                                virtualScrollControllerChanged.apply(this, arguments)
                            }
                        }))
                    }
                })
            }
            that._stateStoringController = new StateStoringController(options.component).init();
            that._columnsInfo = [];
            that._rowsInfo = [];
            that._cellsInfo = [];
            that.expandValueChanging = Callbacks();
            that.loadingChanged = Callbacks();
            that.progressChanged = Callbacks();
            that.scrollChanged = Callbacks();
            that.load();
            that._update();
            that.changed = Callbacks()
        },
        _fireChanged: function() {
            var startChanging = new Date;
            this.changed && !this._lockChanged && this.changed.fire();
            this._changingDuration = new Date - startChanging
        },
        _correctSkipsTakes: function(rowIndex, rowSkip, rowSpan, levels, skips, takes) {
            var endIndex = rowSpan ? rowIndex + rowSpan - 1 : rowIndex;
            skips[levels.length] = skips[levels.length] || 0;
            takes[levels.length] = takes[levels.length] || 0;
            if (endIndex < rowSkip) {
                skips[levels.length]++
            } else {
                takes[levels.length]++
            }
        },
        _calculatePagingForRowExpandedPaths: function(options, skips, takes, rowExpandedSkips, rowExpandedTakes) {
            var rows = this._rowsInfo;
            var rowCount = Math.min(options.rowSkip + options.rowTake, rows.length);
            var rowExpandedPaths = options.rowExpandedPaths;
            var levels = [];
            var expandedPathIndexes = {};
            var i;
            var j;
            var path;
            rowExpandedPaths.forEach((path, index) => {
                expandedPathIndexes[path] = index
            });
            for (i = 0; i < rowCount; i++) {
                takes.length = skips.length = levels.length + 1;
                for (j = 0; j < rows[i].length; j++) {
                    var cell = rows[i][j];
                    if ("D" === cell.type) {
                        this._correctSkipsTakes(i, options.rowSkip, cell.rowspan, levels, skips, takes);
                        path = cell.path || path;
                        var expandIndex = path && path.length > 1 ? expandedPathIndexes[path.slice(0, -1)] : -1;
                        if (expandIndex >= 0) {
                            rowExpandedSkips[expandIndex] = skips[levels.length] || 0;
                            rowExpandedTakes[expandIndex] = takes[levels.length] || 0
                        }
                        if (cell.rowspan) {
                            levels.push(cell.rowspan)
                        }
                    }
                }
                levels = levels.map(level => level - 1).filter(level => level > 0)
            }
        },
        _calculatePagingForColumnExpandedPaths: function(options, skips, takes, expandedSkips, expandedTakes) {
            var skipByPath = {};
            var takeByPath = {};
            foreachColumnInfo(this._columnsInfo, (function(columnInfo, columnIndex) {
                if ("D" === columnInfo.type && columnInfo.path && void 0 === columnInfo.dataIndex) {
                    var colspan = columnInfo.colspan || 1;
                    var path = columnInfo.path.slice(0, -1).toString();
                    skipByPath[path] = skipByPath[path] || 0;
                    takeByPath[path] = takeByPath[path] || 0;
                    if (columnIndex + colspan <= options.columnSkip) {
                        skipByPath[path]++
                    } else if (columnIndex < options.columnSkip + options.columnTake) {
                        takeByPath[path]++
                    }
                }
            }));
            skips[0] = skipByPath[[]];
            takes[0] = takeByPath[[]];
            options.columnExpandedPaths.forEach((function(path, index) {
                var skip = skipByPath[path];
                var take = takeByPath[path];
                if (void 0 !== skip) {
                    expandedSkips[index] = skip
                }
                if (void 0 !== take) {
                    expandedTakes[index] = take
                }
            }))
        },
        _processPagingForExpandedPaths: function(options, area, storeLoadOptions, reload) {
            var expandedPaths = options[area + "ExpandedPaths"];
            var expandedSkips = expandedPaths.map(() => 0);
            var expandedTakes = expandedPaths.map(() => reload ? options.pageSize : 0);
            var skips = [];
            var takes = [];
            if (!reload) {
                if ("row" === area) {
                    this._calculatePagingForRowExpandedPaths(options, skips, takes, expandedSkips, expandedTakes)
                } else {
                    this._calculatePagingForColumnExpandedPaths(options, skips, takes, expandedSkips, expandedTakes)
                }
            }
            this._savePagingForExpandedPaths(options, area, storeLoadOptions, skips[0], takes[0], expandedSkips, expandedTakes)
        },
        _savePagingForExpandedPaths: function(options, area, storeLoadOptions, skip, take, expandedSkips, expandedTakes) {
            var expandedPaths = options[area + "ExpandedPaths"];
            options[area + "ExpandedPaths"] = [];
            options[area + "Skip"] = void 0 !== skip ? skip : options[area + "Skip"];
            options[area + "Take"] = void 0 !== take ? take : options[area + "Take"];
            for (var i = 0; i < expandedPaths.length; i++) {
                if (expandedTakes[i]) {
                    var isOppositeArea = options.area && options.area !== area;
                    storeLoadOptions.push(extend({
                        area: area,
                        headerName: area + "s"
                    }, options, {
                        [area + "Skip"]: expandedSkips[i],
                        [area + "Take"]: expandedTakes[i],
                        [isOppositeArea ? "oppositePath" : "path"]: expandedPaths[i]
                    }))
                }
            }
        },
        _handleCustomizeStoreLoadOptions: function(storeLoadOptions, reload) {
            var options = storeLoadOptions[0];
            var rowsScrollController = this._rowsScrollController;
            if (this._dataSource.paginate() && rowsScrollController) {
                var rowPageSize = rowsScrollController.pageSize();
                if ("rows" === options.headerName) {
                    options.rowSkip = 0;
                    options.rowTake = rowPageSize;
                    options.rowExpandedPaths = []
                } else {
                    options.rowSkip = rowsScrollController.beginPageIndex() * rowPageSize;
                    options.rowTake = (rowsScrollController.endPageIndex() - rowsScrollController.beginPageIndex() + 1) * rowPageSize;
                    this._processPagingForExpandedPaths(options, "row", storeLoadOptions, reload)
                }
            }
            var columnsScrollController = this._columnsScrollController;
            if (this._dataSource.paginate() && columnsScrollController) {
                var columnPageSize = columnsScrollController.pageSize();
                storeLoadOptions.forEach((options, index) => {
                    if ("columns" === options.headerName) {
                        options.columnSkip = 0;
                        options.columnTake = columnPageSize;
                        options.columnExpandedPaths = []
                    } else {
                        options.columnSkip = columnsScrollController.beginPageIndex() * columnPageSize;
                        options.columnTake = (columnsScrollController.endPageIndex() - columnsScrollController.beginPageIndex() + 1) * columnPageSize;
                        this._processPagingForExpandedPaths(options, "column", storeLoadOptions, reload)
                    }
                })
            }
        },
        load: function() {
            var that = this;
            var stateStoringController = this._stateStoringController;
            if (stateStoringController.isEnabled() && !stateStoringController.isLoaded()) {
                stateStoringController.load().always((function(state) {
                    if (state) {
                        that._dataSource.state(state)
                    } else {
                        that._dataSource.load()
                    }
                }))
            } else {
                that._dataSource.load()
            }
        },
        calculateVirtualContentParams: function(contentParams) {
            var rowsScrollController = this._rowsScrollController;
            var columnsScrollController = this._columnsScrollController;
            if (rowsScrollController && columnsScrollController) {
                rowsScrollController.viewportItemSize(contentParams.virtualRowHeight);
                rowsScrollController.viewportSize(contentParams.viewportHeight / rowsScrollController.viewportItemSize());
                rowsScrollController.setContentItemSizes(contentParams.itemHeights);
                columnsScrollController.viewportItemSize(contentParams.virtualColumnWidth);
                columnsScrollController.viewportSize(contentParams.viewportWidth / columnsScrollController.viewportItemSize());
                columnsScrollController.setContentItemSizes(contentParams.itemWidths);
                deferUpdate((function() {
                    columnsScrollController.loadIfNeed();
                    rowsScrollController.loadIfNeed()
                }));
                this.scrollChanged.fire({
                    left: columnsScrollController.getViewportPosition(),
                    top: rowsScrollController.getViewportPosition()
                });
                return {
                    contentTop: rowsScrollController.getContentOffset(),
                    contentLeft: columnsScrollController.getContentOffset(),
                    width: columnsScrollController.getVirtualContentSize(),
                    height: rowsScrollController.getVirtualContentSize()
                }
            }
        },
        setViewportPosition: function(left, top) {
            this._rowsScrollController.setViewportPosition(top || 0);
            this._columnsScrollController.setViewportPosition(left || 0)
        },
        subscribeToWindowScrollEvents: function($element) {
            this._rowsScrollController && this._rowsScrollController.subscribeToWindowScrollEvents($element)
        },
        updateWindowScrollPosition: function(position) {
            this._rowsScrollController && this._rowsScrollController.scrollTo(position)
        },
        updateViewOptions: function(options) {
            extend(this._options, options);
            this._update()
        },
        _handleExpandValueChanging: function(e) {
            this.expandValueChanging.fire(e)
        },
        _handleLoadingChanged: function(isLoading) {
            this.loadingChanged.fire(isLoading)
        },
        _handleProgressChanged: function(progress) {
            this.progressChanged.fire(progress)
        },
        _handleFieldsPrepared: function(e) {
            this._options.onFieldsPrepared && this._options.onFieldsPrepared(e)
        },
        _createDataSource: function(options) {
            var that = this;
            var dataSourceOptions = options.dataSource;
            var dataSource;
            that._isSharedDataSource = dataSourceOptions instanceof PivotGridDataSource;
            if (that._isSharedDataSource) {
                dataSource = dataSourceOptions
            } else {
                dataSource = new PivotGridDataSource(dataSourceOptions)
            }
            that._expandValueChangingHandler = that._handleExpandValueChanging.bind(that);
            that._loadingChangedHandler = that._handleLoadingChanged.bind(that);
            that._fieldsPreparedHandler = that._handleFieldsPrepared.bind(that);
            that._customizeStoreLoadOptionsHandler = that._handleCustomizeStoreLoadOptions.bind(that);
            that._changedHandler = function() {
                that._update();
                that.dataSourceChanged.fire()
            };
            that._progressChangedHandler = function(progress) {
                that._handleProgressChanged(.8 * progress)
            };
            dataSource.on("changed", that._changedHandler);
            dataSource.on("expandValueChanging", that._expandValueChangingHandler);
            dataSource.on("loadingChanged", that._loadingChangedHandler);
            dataSource.on("progressChanged", that._progressChangedHandler);
            dataSource.on("fieldsPrepared", that._fieldsPreparedHandler);
            dataSource.on("customizeStoreLoadOptions", that._customizeStoreLoadOptionsHandler);
            return dataSource
        },
        getDataSource: function() {
            return this._dataSource
        },
        isLoading: function() {
            return this._dataSource.isLoading()
        },
        beginLoading: function() {
            this._dataSource.beginLoading()
        },
        endLoading: function() {
            this._dataSource.endLoading()
        },
        _update: function() {
            var that = this;
            var dataSource = that._dataSource;
            var options = that._options;
            var columnFields = dataSource.getAreaFields("column");
            var rowFields = dataSource.getAreaFields("row");
            var dataFields = dataSource.getAreaFields("data");
            var dataFieldsForRows = "row" === options.dataFieldArea ? dataFields : [];
            var dataFieldsForColumns = "row" !== options.dataFieldArea ? dataFields : [];
            var data = dataSource.getData();
            var hiddenTotals = function(dataFields) {
                var result = [];
                each(dataFields, (function(index, field) {
                    if (false === field.showTotals) {
                        result.push(index)
                    }
                }));
                return result
            }(dataFields);
            var hiddenValues = function(dataFields) {
                var result = [];
                dataFields.forEach((function(field, index) {
                    if (void 0 === field.showValues && false === field.showTotals || false === field.showValues) {
                        result.push(index)
                    }
                }));
                return result
            }(dataFields);
            var hiddenGrandTotals = function(dataFields, columnFields) {
                var result = [];
                each(dataFields, (function(index, field) {
                    if (false === field.showGrandTotals) {
                        result.push(index)
                    }
                }));
                if (0 === columnFields.length && result.length === dataFields.length) {
                    result = []
                }
                return result
            }(dataFields, columnFields);
            var grandTotalsAreHiddenForNotAllDataFields = dataFields.length > 0 ? hiddenGrandTotals.length !== dataFields.length : true;
            var rowOptions = {
                isEmptyGrandTotal: data.isEmptyGrandTotalRow,
                texts: options.texts || {},
                hiddenTotals: hiddenTotals,
                hiddenValues: hiddenValues,
                hiddenGrandTotals: [],
                showTotals: options.showRowTotals,
                showGrandTotals: false !== options.showRowGrandTotals && grandTotalsAreHiddenForNotAllDataFields,
                sortBySummaryPaths: createSortPaths(columnFields, dataFields),
                showTotalsPrior: "rows" === options.showTotalsPrior || "both" === options.showTotalsPrior,
                showEmpty: !options.hideEmptySummaryCells,
                layout: options.rowHeaderLayout,
                fields: rowFields,
                dataFields: dataFields,
                progress: 0
            };
            var columnOptions = {
                isEmptyGrandTotal: data.isEmptyGrandTotalColumn,
                texts: options.texts || {},
                hiddenTotals: hiddenTotals,
                hiddenValues: hiddenValues,
                hiddenGrandTotals: hiddenGrandTotals,
                showTotals: options.showColumnTotals,
                showTotalsPrior: "columns" === options.showTotalsPrior || "both" === options.showTotalsPrior,
                showGrandTotals: false !== options.showColumnGrandTotals && grandTotalsAreHiddenForNotAllDataFields,
                sortBySummaryPaths: createSortPaths(rowFields, dataFields),
                showEmpty: !options.hideEmptySummaryCells,
                fields: columnFields,
                dataFields: dataFields,
                progress: 0
            };
            var notifyProgress = function(progress) {
                this.progress = progress;
                that._handleProgressChanged(.8 + .1 * rowOptions.progress + .1 * columnOptions.progress)
            };
            rowOptions.notifyProgress = notifyProgress;
            columnOptions.notifyProgress = notifyProgress;
            if (!isDefined(data.grandTotalRowIndex)) {
                data.grandTotalRowIndex = getHeaderIndexedItems(data.rows, rowOptions).length
            }
            if (!isDefined(data.grandTotalColumnIndex)) {
                data.grandTotalColumnIndex = getHeaderIndexedItems(data.columns, columnOptions).length
            }
            dataSource._changeLoadingCount(1);
            when(createHeaderInfo(data.columns, columnFields, dataFieldsForColumns, true, columnOptions), createHeaderInfo(data.rows, rowFields, dataFieldsForRows, false, rowOptions)).always((function() {
                dataSource._changeLoadingCount(-1)
            })).done((function(columnsInfo, rowsInfo) {
                that._columnsInfo = columnsInfo;
                that._rowsInfo = rowsInfo;
                if (that._rowsScrollController && that._columnsScrollController && that.changed && !that._dataSource.paginate()) {
                    that._rowsScrollController.reset(true);
                    that._columnsScrollController.reset(true);
                    that._lockChanged = true;
                    that._rowsScrollController.load();
                    that._columnsScrollController.load();
                    that._lockChanged = false
                }
            })).done((function() {
                that._fireChanged();
                if (that._stateStoringController.isEnabled() && !that._dataSource.isLoading()) {
                    that._stateStoringController.state(that._dataSource.state());
                    that._stateStoringController.save()
                }
            }))
        },
        getRowsInfo: function(getAllData) {
            var rowsInfo = this._rowsInfo;
            var scrollController = this._rowsScrollController;
            var rowspan;
            if (scrollController && !getAllData) {
                var startIndex = scrollController.beginPageIndex() * this.rowPageSize();
                var endIndex = scrollController.endPageIndex() * this.rowPageSize() + this.rowPageSize();
                var newRowsInfo = [];
                var maxDepth = 1;
                foreachRowInfo(rowsInfo, (function(rowInfo, visibleIndex, rowIndex, _, columnIndex) {
                    var isVisible = visibleIndex >= startIndex && rowIndex < endIndex;
                    var index = rowIndex < startIndex ? 0 : rowIndex - startIndex;
                    var cell = rowInfo;
                    if (isVisible) {
                        newRowsInfo[index] = newRowsInfo[index] || [];
                        rowspan = rowIndex < startIndex ? rowInfo.rowspan - (startIndex - rowIndex) || 1 : rowInfo.rowspan;
                        if (startIndex + index + rowspan > endIndex) {
                            rowspan = endIndex - (index + startIndex) || 1
                        }
                        if (rowspan !== rowInfo.rowspan) {
                            cell = extend({}, cell, {
                                rowspan: rowspan
                            })
                        }
                        newRowsInfo[index].push(cell);
                        maxDepth = math.max(maxDepth, columnIndex + 1)
                    } else if (void 0 > endIndex) {
                        return false
                    }
                }));
                foreachRowInfo(newRowsInfo, (function(rowInfo, visibleIndex, rowIndex, columnIndex, realColumnIndex) {
                    var colspan = rowInfo.colspan || 1;
                    if (realColumnIndex + colspan > maxDepth) {
                        newRowsInfo[rowIndex][columnIndex] = extend({}, rowInfo, {
                            colspan: maxDepth - realColumnIndex || 1
                        })
                    }
                }));
                return newRowsInfo
            }
            return rowsInfo
        },
        getColumnsInfo: function(getAllData) {
            var info = this._columnsInfo;
            var scrollController = this._columnsScrollController;
            if (scrollController && !getAllData) {
                var startIndex = scrollController.beginPageIndex() * this.columnPageSize();
                var endIndex = scrollController.endPageIndex() * this.columnPageSize() + this.columnPageSize();
                info = createColumnsInfo(info, startIndex, endIndex)
            }
            return info
        },
        totalRowCount: function() {
            return this._rowsInfo.length
        },
        rowPageIndex: function(index) {
            if (void 0 !== index) {
                this._rowPageIndex = index
            }
            return this._rowPageIndex || 0
        },
        totalColumnCount: function() {
            var count = 0;
            if (this._columnsInfo && this._columnsInfo.length) {
                for (var i = 0; i < this._columnsInfo[0].length; i++) {
                    count += this._columnsInfo[0][i].colspan || 1
                }
            }
            return count
        },
        rowPageSize: function(size) {
            if (void 0 !== size) {
                this._rowPageSize = size
            }
            return this._rowPageSize || 20
        },
        columnPageSize: function(size) {
            if (void 0 !== size) {
                this._columnPageSize = size
            }
            return this._columnPageSize || 20
        },
        columnPageIndex: function(index) {
            if (void 0 !== index) {
                this._columnPageIndex = index
            }
            return this._columnPageIndex || 0
        },
        getCellsInfo: function(getAllData) {
            var rowsInfo = this.getRowsInfo(getAllData);
            var columnsInfo = this.getColumnsInfo(getAllData);
            var data = this._dataSource.getData();
            var texts = this._options.texts || {};
            return function(rowsInfo, columnsInfo, data, dataFields, dataFieldArea, errorText) {
                var info = [];
                var dataFieldAreaInRows = "row" === dataFieldArea;
                var dataSourceCells = data.values;
                dataSourceCells.length && foreachRowInfo(rowsInfo, (function(rowInfo, rowIndex) {
                    var row = info[rowIndex] = [];
                    var dataRow = dataSourceCells[rowInfo.dataSourceIndex >= 0 ? rowInfo.dataSourceIndex : data.grandTotalRowIndex] || [];
                    rowInfo.isLast && foreachColumnInfo(columnsInfo, (function(columnInfo, columnIndex) {
                        var dataIndex = (dataFieldAreaInRows ? rowInfo.dataIndex : columnInfo.dataIndex) || 0;
                        var dataField = dataFields[dataIndex];
                        if (columnInfo.isLast && dataField && false !== dataField.visible) {
                            var cell = dataRow[columnInfo.dataSourceIndex >= 0 ? columnInfo.dataSourceIndex : data.grandTotalColumnIndex];
                            if (!Array.isArray(cell)) {
                                cell = [cell]
                            }
                            var cellValue = cell[dataIndex];
                            row[columnIndex] = {
                                text: formatCellValue(cellValue, dataField, errorText),
                                value: cellValue,
                                format: dataField.format,
                                dataType: dataField.dataType,
                                columnType: columnInfo.type,
                                rowType: rowInfo.type,
                                rowPath: rowInfo.path || [],
                                columnPath: columnInfo.path || [],
                                dataIndex: dataIndex
                            };
                            if (dataField.width) {
                                row[columnIndex].width = dataField.width
                            }
                        }
                    }))
                }));
                return info
            }(rowsInfo, columnsInfo, data, this._dataSource.getAreaFields("data"), this._options.dataFieldArea, texts.dataNotAvailable)
        },
        dispose: function() {
            if (this._isSharedDataSource) {
                this._dataSource.off("changed", this._changedHandler);
                this._dataSource.off("expandValueChanging", this._expandValueChangingHandler);
                this._dataSource.off("loadingChanged", this._loadingChangedHandler);
                this._dataSource.off("progressChanged", this._progressChangedHandler);
                this._dataSource.off("fieldsPrepared", this._fieldsPreparedHandler);
                this._dataSource.off("customizeStoreLoadOptions", this._customizeStoreLoadOptionsHandler)
            } else {
                this._dataSource.dispose()
            }
            this._columnsScrollController && this._columnsScrollController.dispose();
            this._rowsScrollController && this._rowsScrollController.dispose();
            this._stateStoringController.dispose();
            this.expandValueChanging.empty();
            this.changed.empty();
            this.loadingChanged.empty();
            this.progressChanged.empty();
            this.scrollChanged.empty();
            this.dataSourceChanged.empty()
        }
    };
    proxyMethod(members, "applyPartialDataSource");
    proxyMethod(members, "collapseHeaderItem");
    proxyMethod(members, "expandHeaderItem");
    proxyMethod(members, "getData");
    proxyMethod(members, "isEmpty");
    return members
}());
