/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.columns_resizing_reordering.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    setHeight as _setHeight,
    getHeight,
    setWidth,
    getWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import domAdapter from "../../core/dom_adapter";
import eventsEngine from "../../events/core/events_engine";
import Callbacks from "../../core/utils/callbacks";
import {
    isDefined,
    isString,
    isObject
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import {
    getBoundingRect
} from "../../core/utils/position";
import {
    addNamespace,
    eventData as getEventData,
    isTouchEvent
} from "../../events/utils/index";
import pointerEvents from "../../events/pointer";
import {
    start as dragEventStart,
    move as dragEventMove,
    end as dragEventEnd
} from "../../events/drag";
import modules from "./ui.grid_core.modules";
import gridCoreUtils from "./ui.grid_core.utils";
import fx from "../../animation/fx";
import swatchContainer from "../widget/swatch_container";
var COLUMNS_SEPARATOR_CLASS = "columns-separator";
var COLUMNS_SEPARATOR_TRANSPARENT = "columns-separator-transparent";
var DRAGGING_HEADER_CLASS = "drag-header";
var CELL_CONTENT_CLASS = "text-content";
var HEADERS_DRAG_ACTION_CLASS = "drag-action";
var TRACKER_CLASS = "tracker";
var HEADERS_DROP_HIGHLIGHT_CLASS = "drop-highlight";
var BLOCK_SEPARATOR_CLASS = "dx-block-separator";
var HEADER_ROW_CLASS = "dx-header-row";
var WIDGET_CLASS = "dx-widget";
var DRAGGING_COMMAND_CELL_CLASS = "dx-drag-command-cell";
var MODULE_NAMESPACE = "dxDataGridResizingReordering";
var COLUMNS_SEPARATOR_TOUCH_TRACKER_WIDTH = 10;
var DRAGGING_DELTA = 5;
var COLUMN_OPACITY = .5;
var allowResizing = function(that) {
    return that.option("allowColumnResizing") || that.getController("columns").isColumnOptionUsed("allowResizing")
};
var allowReordering = function(that) {
    return that.option("allowColumnReordering") || that.getController("columns").isColumnOptionUsed("allowReordering")
};
var TrackerView = modules.View.inherit({
    _renderCore: function() {
        this.callBase();
        this.element().addClass(this.addWidgetPrefix(TRACKER_CLASS));
        this.hide()
    },
    _unsubscribeFromCallback: function() {
        if (this._positionChanged) {
            this._tablePositionController.positionChanged.remove(this._positionChanged)
        }
    },
    _subscribeToCallback: function() {
        var that = this;
        that._positionChanged = function(position) {
            var $element = that.element();
            if ($element && $element.hasClass(that.addWidgetPrefix(TRACKER_CLASS))) {
                $element.css({
                    top: position.top
                });
                _setHeight($element, position.height)
            }
        };
        this._tablePositionController.positionChanged.add(that._positionChanged)
    },
    optionChanged: function(args) {
        if ("allowColumnResizing" === args.name) {
            this._unsubscribeFromCallback();
            if (args.value) {
                this._subscribeToCallback();
                this._invalidate()
            }
        }
        this.callBase(args)
    },
    init: function() {
        this.callBase();
        this._tablePositionController = this.getController("tablePosition");
        this._subscribeToCallback()
    },
    isVisible: function() {
        return allowResizing(this)
    },
    show: function() {
        this.element().show()
    },
    hide: function() {
        this.element() && this.element().hide()
    },
    setHeight: function(value) {
        _setHeight(this.element(), value)
    },
    dispose: function() {
        this._unsubscribeFromCallback();
        this.callBase()
    }
});
var SeparatorView = modules.View.inherit({
    _renderSeparator: function() {},
    _renderCore: function(options) {
        this.callBase(options);
        this._isShown = true;
        this._renderSeparator();
        this.hide()
    },
    show: function() {
        this._isShown = true
    },
    hide: function() {
        this._isShown = false
    },
    height: function(value) {
        var $element = this.element();
        if ($element) {
            if (isDefined(value)) {
                _setHeight($element, value)
            } else {
                return getHeight($element)
            }
        }
    },
    width: function(value) {
        var $element = this.element();
        if ($element) {
            if (isDefined(value)) {
                setWidth($element, value)
            } else {
                return getWidth($element)
            }
        }
    }
});
var ColumnsSeparatorView = SeparatorView.inherit({
    _renderSeparator: function() {
        this.callBase();
        var $element = this.element();
        $element.addClass(this.addWidgetPrefix(COLUMNS_SEPARATOR_CLASS))
    },
    _subscribeToCallback: function() {
        var that = this;
        var $element;
        that._positionChanged = function(position) {
            $element = that.element();
            if ($element) {
                $element.css({
                    top: position.top
                });
                _setHeight($element, position.height)
            }
        };
        that._tablePositionController.positionChanged.add(that._positionChanged)
    },
    _unsubscribeFromCallback: function() {
        this._positionChanged && this._tablePositionController.positionChanged.remove(this._positionChanged)
    },
    _init: function() {
        this._isTransparent = allowResizing(this);
        if (this.isVisible()) {
            this._subscribeToCallback()
        }
    },
    isVisible: function() {
        return this.option("showColumnHeaders") && (allowReordering(this) || allowResizing(this))
    },
    optionChanged: function(args) {
        if ("allowColumnResizing" === args.name) {
            if (args.value) {
                this._init();
                this._invalidate();
                this.hide(true)
            } else {
                this._unsubscribeFromCallback();
                this._isTransparent = allowResizing(this);
                this.hide(true)
            }
        }
        this.callBase(args)
    },
    init: function() {
        this.callBase();
        this._tablePositionController = this.getController("tablePosition");
        this._init()
    },
    show: function() {
        var $element = this.element();
        if ($element && !this._isShown) {
            if (this._isTransparent) {
                $element.removeClass(this.addWidgetPrefix(COLUMNS_SEPARATOR_TRANSPARENT))
            } else {
                $element.show()
            }
        }
        this.callBase()
    },
    hide: function(force) {
        var $element = this.element();
        var columnsSeparatorTransparent = this.addWidgetPrefix(COLUMNS_SEPARATOR_TRANSPARENT);
        if ($element && (this._isShown || force)) {
            if (this._isTransparent) {
                $element.addClass(columnsSeparatorTransparent);
                $element.css("left", "");
                $element.show()
            } else {
                if ($element.hasClass(columnsSeparatorTransparent)) {
                    $element.removeClass(columnsSeparatorTransparent)
                }
                $element.hide()
            }
        }
        this.callBase()
    },
    moveByX: function(outerX) {
        var $element = this.element();
        if ($element) {
            $element.css("left", null === outerX ? 0 : outerX - this._parentElement().offset().left)
        }
    },
    changeCursor: function(cursorName) {
        cursorName = isDefined(cursorName) ? cursorName : "";
        var $element = this.element();
        if ($element) {
            $element.css("cursor", cursorName)
        }
    },
    dispose: function() {
        this._unsubscribeFromCallback();
        this.callBase()
    }
});
var BlockSeparatorView = SeparatorView.inherit({
    init: function() {
        var that = this;
        this.callBase();
        this.getController("data").loadingChanged.add((function(isLoading) {
            if (!isLoading) {
                that.hide()
            }
        }))
    },
    _renderSeparator: function() {
        this.callBase();
        this.element().addClass(BLOCK_SEPARATOR_CLASS).html("&nbsp;")
    },
    hide: function() {
        var $parent = this._parentElement();
        var $element = this.element();
        if ($element && this._isShown) {
            $element.css("display", "none")
        }
        if ($parent && !$parent.children("." + BLOCK_SEPARATOR_CLASS).length) {
            $parent.prepend(this.element())
        }
        this.callBase()
    },
    isVisible: function() {
        var groupPanelOptions = this.option("groupPanel");
        var columnChooserOptions = this.option("columnChooser");
        return groupPanelOptions && groupPanelOptions.visible || columnChooserOptions && columnChooserOptions.enabled
    },
    show: function(targetLocation) {
        var $element = this.element();
        var startAnimate = function(toOptions) {
            fx.stop($element, true);
            fx.animate($element, {
                type: "slide",
                from: {
                    width: 0,
                    display: toOptions.display
                },
                to: toOptions,
                duration: 300,
                easing: "swing"
            })
        };
        if ($element && !this._isShown) {
            switch (targetLocation) {
                case "group":
                    startAnimate({
                        width: "50px",
                        display: "inline-block"
                    });
                    break;
                case "columnChooser":
                    startAnimate({
                        width: "100%",
                        display: "block"
                    });
                    break;
                default:
                    $element.css("display", "")
            }
        }
        this.callBase()
    }
});
var DraggingHeaderView = modules.View.inherit({
    _isDragging: false,
    isDragging: function() {
        return this._isDragging
    },
    _getDraggingPanelByPos: function(pos) {
        var result;
        each(this._dragOptions.draggingPanels, (function(index, draggingPanel) {
            if (draggingPanel) {
                var boundingRect = draggingPanel.getBoundingRect();
                if (boundingRect && (void 0 === boundingRect.bottom || pos.y < boundingRect.bottom) && (void 0 === boundingRect.top || pos.y > boundingRect.top) && (void 0 === boundingRect.left || pos.x > boundingRect.left) && (void 0 === boundingRect.right || pos.x < boundingRect.right)) {
                    result = draggingPanel;
                    return false
                }
            }
        }));
        return result
    },
    _renderCore: function() {
        this.element().addClass(this.addWidgetPrefix(DRAGGING_HEADER_CLASS) + " " + this.addWidgetPrefix(CELL_CONTENT_CLASS) + " " + WIDGET_CLASS).hide()
    },
    _resetTargetColumnOptions: function() {
        var params = this._dropOptions;
        params.targetColumnIndex = -1;
        delete params.targetColumnElement;
        delete params.isLast;
        delete params.posX;
        delete params.posY
    },
    _getVisibleIndexObject: function(rowIndex, visibleIndex) {
        if (isDefined(rowIndex)) {
            return {
                columnIndex: visibleIndex,
                rowIndex: rowIndex
            }
        }
        return visibleIndex
    },
    dispose: function() {
        var element = this.element();
        this._dragOptions = null;
        element && element.parent().find("." + this.addWidgetPrefix(DRAGGING_HEADER_CLASS)).remove()
    },
    isVisible: function() {
        var columnsController = this.getController("columns");
        var commonColumnSettings = columnsController.getCommonSettings();
        return this.option("showColumnHeaders") && (allowReordering(this) || commonColumnSettings.allowGrouping || commonColumnSettings.allowHiding)
    },
    init: function() {
        var that = this;
        this.callBase();
        this._controller = this.getController("draggingHeader");
        this._columnsResizerViewController = this.getController("columnsResizer");
        this.getController("data").loadingChanged.add((function(isLoading) {
            var element = that.element();
            if (!isLoading && element) {
                element.hide()
            }
        }))
    },
    dragHeader: function(options) {
        var columnElement = options.columnElement;
        var isCommandColumn = !!options.sourceColumn.type;
        this._isDragging = true;
        this._dragOptions = options;
        this._dropOptions = {
            sourceIndex: options.index,
            sourceColumnIndex: this._getVisibleIndexObject(options.rowIndex, options.columnIndex),
            sourceColumnElement: options.columnElement,
            sourceLocation: options.sourceLocation
        };
        var document = domAdapter.getDocument();
        this._onSelectStart = document.onselectstart;
        document.onselectstart = function() {
            return false
        };
        this._controller.drag(this._dropOptions);
        this.element().css({
            textAlign: columnElement && columnElement.css("textAlign"),
            height: columnElement && (isCommandColumn && columnElement.get(0).clientHeight || getHeight(columnElement)),
            width: columnElement && (isCommandColumn && columnElement.get(0).clientWidth || getWidth(columnElement)),
            whiteSpace: columnElement && columnElement.css("whiteSpace")
        }).addClass(this.addWidgetPrefix(HEADERS_DRAG_ACTION_CLASS)).toggleClass(DRAGGING_COMMAND_CELL_CLASS, isCommandColumn).text(isCommandColumn ? "" : options.sourceColumn.caption);
        this.element().appendTo(swatchContainer.getSwatchContainer(columnElement))
    },
    moveHeader: function(args) {
        var e = args.event;
        var that = e.data.that;
        var eventData = getEventData(e);
        var isResizing = that._columnsResizerViewController ? that._columnsResizerViewController.isResizing() : false;
        var dragOptions = that._dragOptions;
        if (that._isDragging && !isResizing) {
            var $element = that.element();
            var moveDeltaX = Math.abs(eventData.x - dragOptions.columnElement.offset().left - dragOptions.deltaX);
            var moveDeltaY = Math.abs(eventData.y - dragOptions.columnElement.offset().top - dragOptions.deltaY);
            if ($element.is(":visible") || moveDeltaX > DRAGGING_DELTA || moveDeltaY > DRAGGING_DELTA) {
                $element.show();
                var newLeft = eventData.x - dragOptions.deltaX;
                var newTop = eventData.y - dragOptions.deltaY;
                $element.css({
                    left: newLeft,
                    top: newTop
                });
                that.dockHeader(eventData)
            }
            e.preventDefault()
        }
    },
    dockHeader: function(eventData) {
        var targetDraggingPanel = this._getDraggingPanelByPos(eventData);
        var controller = this._controller;
        var params = this._dropOptions;
        var dragOptions = this._dragOptions;
        if (targetDraggingPanel) {
            var rtlEnabled = this.option("rtlEnabled");
            var isVerticalOrientation = "columnChooser" === targetDraggingPanel.getName();
            var axisName = isVerticalOrientation ? "y" : "x";
            var targetLocation = targetDraggingPanel.getName();
            var rowIndex = "headers" === targetLocation ? dragOptions.rowIndex : void 0;
            var sourceColumn = dragOptions.sourceColumn;
            var columnElements = targetDraggingPanel.getColumnElements(rowIndex, null === sourceColumn || void 0 === sourceColumn ? void 0 : sourceColumn.ownerBand) || [];
            var pointsByTarget = dragOptions.pointsByTarget = dragOptions.pointsByTarget || {};
            var pointsByColumns = "columnChooser" === targetLocation ? [] : pointsByTarget[targetLocation] || controller._generatePointsByColumns(extend({}, dragOptions, {
                targetDraggingPanel: targetDraggingPanel,
                columns: targetDraggingPanel.getColumns(rowIndex),
                columnElements: columnElements,
                isVerticalOrientation: isVerticalOrientation,
                startColumnIndex: "headers" === targetLocation && $(columnElements[0]).index()
            }));
            pointsByTarget[targetLocation] = pointsByColumns;
            params.targetLocation = targetLocation;
            if (pointsByColumns.length > 0) {
                for (var i = 0; i < pointsByColumns.length; i++) {
                    var centerPosition = pointsByColumns[i + 1] && (pointsByColumns[i][axisName] + pointsByColumns[i + 1][axisName]) / 2;
                    if (void 0 === centerPosition || (rtlEnabled && "x" === axisName ? eventData[axisName] > centerPosition : eventData[axisName] < centerPosition)) {
                        params.targetColumnIndex = this._getVisibleIndexObject(rowIndex, pointsByColumns[i].columnIndex);
                        if (columnElements[i]) {
                            params.targetColumnElement = columnElements.eq(i);
                            params.isLast = false
                        } else {
                            params.targetColumnElement = columnElements.last();
                            params.isLast = true
                        }
                        params.posX = pointsByColumns[i].x;
                        params.posY = pointsByColumns[i].y;
                        controller.dock(params);
                        break
                    }
                }
            } else {
                this._resetTargetColumnOptions();
                controller.dock(params)
            }
        }
    },
    dropHeader: function(args) {
        var e = args.event;
        var that = e.data.that;
        var controller = that._controller;
        that.element().hide();
        if (controller && that._isDragging) {
            controller.drop(that._dropOptions)
        }
        that.element().appendTo(that._parentElement());
        that._dragOptions = null;
        that._dropOptions = null;
        that._isDragging = false;
        domAdapter.getDocument().onselectstart = that._onSelectStart || null
    }
});
var isNextColumnResizingMode = function(that) {
    return "widget" !== that.option("columnResizingMode")
};
var ColumnsResizerViewController = modules.ViewController.inherit({
    _isHeadersRowArea: function(posY) {
        if (this._columnHeadersView) {
            var element = this._columnHeadersView.element();
            if (element) {
                var offsetTop = element.offset().top;
                var headersRowHeight = this._columnHeadersView.getHeadersRowHeight();
                return posY >= offsetTop && posY <= offsetTop + headersRowHeight
            }
        }
        return false
    },
    _isRtlParentStyle: function() {
        var _this$_$parentContain;
        return this.option("rtlEnabled") && "rtl" === (null === (_this$_$parentContain = this._$parentContainer) || void 0 === _this$_$parentContain ? void 0 : _this$_$parentContain.parent().css("direction"))
    },
    _pointCreated: function(point, cellsLength, columns) {
        var isNextColumnMode = isNextColumnResizingMode(this);
        var rtlEnabled = this.option("rtlEnabled");
        var isRtlParentStyle = this._isRtlParentStyle();
        var firstPointColumnIndex = !isNextColumnMode && rtlEnabled && !isRtlParentStyle ? 0 : 1;
        if (point.index >= firstPointColumnIndex && point.index < cellsLength + (!isNextColumnMode && (!rtlEnabled || isRtlParentStyle) ? 1 : 0)) {
            point.columnIndex -= firstPointColumnIndex;
            var currentColumn = columns[point.columnIndex] || {};
            var nextColumn = columns[point.columnIndex + 1] || {};
            return !(isNextColumnMode ? currentColumn.allowResizing && nextColumn.allowResizing : currentColumn.allowResizing)
        }
        return true
    },
    _getTargetPoint: function(pointsByColumns, currentX, deltaX) {
        if (pointsByColumns) {
            for (var i = 0; i < pointsByColumns.length; i++) {
                if (pointsByColumns[i].x === pointsByColumns[0].x && pointsByColumns[i + 1] && pointsByColumns[i].x === pointsByColumns[i + 1].x) {
                    continue
                }
                if (pointsByColumns[i].x - deltaX <= currentX && currentX <= pointsByColumns[i].x + deltaX) {
                    return pointsByColumns[i]
                }
            }
        }
        return null
    },
    _moveSeparator: function(args) {
        var _that$_draggingHeader;
        var e = args.event;
        var that = e.data;
        var columnsSeparatorWidth = that._columnsSeparatorView.width();
        var isNextColumnMode = isNextColumnResizingMode(that);
        var deltaX = columnsSeparatorWidth / 2;
        var parentOffset = that._$parentContainer.offset();
        var parentOffsetLeft = parentOffset.left;
        var eventData = getEventData(e);
        var rtlEnabled = that.option("rtlEnabled");
        var isRtlParentStyle = this._isRtlParentStyle();
        var isDragging = null === (_that$_draggingHeader = that._draggingHeaderView) || void 0 === _that$_draggingHeader ? void 0 : _that$_draggingHeader.isDragging();
        if (that._isResizing && that._resizingInfo) {
            if ((parentOffsetLeft <= eventData.x || !isNextColumnMode && isRtlParentStyle) && (!isNextColumnMode || eventData.x <= parentOffsetLeft + getWidth(that._$parentContainer))) {
                if (that._updateColumnsWidthIfNeeded(eventData.x)) {
                    var $cell = that._columnHeadersView.getColumnElements().eq(that._resizingInfo.currentColumnIndex);
                    var cell = $cell[0];
                    if (cell) {
                        var outerWidth = cell.getBoundingClientRect().width;
                        that._columnsSeparatorView.moveByX($cell.offset().left + ((isNextColumnMode || isRtlParentStyle) && rtlEnabled ? 0 : outerWidth));
                        that._tablePositionController.update(that._targetPoint.y);
                        e.preventDefault()
                    }
                }
            }
        } else if (!isDragging) {
            if (that._isHeadersRowArea(eventData.y)) {
                if (that._previousParentOffset) {
                    if (that._previousParentOffset.left !== parentOffset.left || that._previousParentOffset.top !== parentOffset.top) {
                        that.pointsByColumns(null)
                    }
                }
                that._targetPoint = that._getTargetPoint(that.pointsByColumns(), eventData.x, columnsSeparatorWidth);
                that._previousParentOffset = parentOffset;
                that._isReadyResizing = false;
                if (that._targetPoint) {
                    that._columnsSeparatorView.changeCursor("col-resize");
                    that._columnsSeparatorView.moveByX(that._targetPoint.x - deltaX);
                    that._tablePositionController.update(that._targetPoint.y);
                    that._isReadyResizing = true;
                    e.preventDefault()
                } else {
                    that._columnsSeparatorView.changeCursor();
                    that._columnsSeparatorView.moveByX(null)
                }
            } else {
                that.pointsByColumns(null);
                that._isReadyResizing = false;
                that._columnsSeparatorView.changeCursor();
                that._columnsSeparatorView.moveByX(null)
            }
        }
    },
    _endResizing: function(args) {
        var e = args.event;
        var that = e.data;
        if (that._isResizing) {
            that.pointsByColumns(null);
            that._resizingInfo = null;
            that._columnsSeparatorView.hide();
            that._columnsSeparatorView.changeCursor();
            that._trackerView.hide();
            that._isReadyResizing = false;
            that._isResizing = false
        }
    },
    _getNextColumnIndex: function(currentColumnIndex) {
        return currentColumnIndex + 1
    },
    _setupResizingInfo: function(posX) {
        var currentColumnIndex = this._targetPoint.columnIndex;
        var nextColumnIndex = this._getNextColumnIndex(currentColumnIndex);
        var currentHeader = this._columnHeadersView.getHeaderElement(currentColumnIndex);
        var nextHeader = this._columnHeadersView.getHeaderElement(nextColumnIndex);
        this._resizingInfo = {
            startPosX: posX,
            currentColumnIndex: currentColumnIndex,
            currentColumnWidth: currentHeader && currentHeader.length > 0 ? getBoundingRect(currentHeader[0]).width : 0,
            nextColumnIndex: nextColumnIndex,
            nextColumnWidth: nextHeader && nextHeader.length > 0 ? getBoundingRect(nextHeader[0]).width : 0
        }
    },
    _startResizing: function(args) {
        var e = args.event;
        var that = e.data;
        var eventData = getEventData(e);
        if (isTouchEvent(e)) {
            if (that._isHeadersRowArea(eventData.y)) {
                that._targetPoint = that._getTargetPoint(that.pointsByColumns(), eventData.x, COLUMNS_SEPARATOR_TOUCH_TRACKER_WIDTH);
                if (that._targetPoint) {
                    that._columnsSeparatorView.moveByX(that._targetPoint.x - that._columnsSeparatorView.width() / 2);
                    that._isReadyResizing = true
                }
            } else {
                that._isReadyResizing = false
            }
        }
        if (that._isReadyResizing) {
            that._setupResizingInfo(eventData.x);
            that._isResizing = true;
            that._tablePositionController.update(that._targetPoint.y);
            that._columnsSeparatorView.show();
            that._trackerView.show();
            var scrollable = that.component.getScrollable();
            if (scrollable && that._isRtlParentStyle()) {
                that._scrollRight = getWidth(scrollable.$content()) - getWidth(scrollable.container()) - scrollable.scrollLeft()
            }
            e.preventDefault();
            e.stopPropagation()
        }
        if (this.isResizing()) {
            this.getController("editorFactory").loseFocus()
        }
    },
    _generatePointsByColumns: function() {
        var that = this;
        var columns = that._columnsController ? that._columnsController.getVisibleColumns() : [];
        var cells = that._columnHeadersView.getColumnElements();
        var pointsByColumns = [];
        if (cells && cells.length > 0) {
            pointsByColumns = gridCoreUtils.getPointsByColumns(cells, (function(point) {
                return that._pointCreated(point, cells.length, columns)
            }))
        }
        that._pointsByColumns = pointsByColumns
    },
    _unsubscribeFromEvents: function() {
        this._moveSeparatorHandler && eventsEngine.off(domAdapter.getDocument(), addNamespace(pointerEvents.move, MODULE_NAMESPACE), this._moveSeparatorHandler);
        this._startResizingHandler && eventsEngine.off(this._$parentContainer, addNamespace(pointerEvents.down, MODULE_NAMESPACE), this._startResizingHandler);
        if (this._endResizingHandler) {
            eventsEngine.off(this._columnsSeparatorView.element(), addNamespace(pointerEvents.up, MODULE_NAMESPACE), this._endResizingHandler);
            eventsEngine.off(domAdapter.getDocument(), addNamespace(pointerEvents.up, MODULE_NAMESPACE), this._endResizingHandler)
        }
    },
    _subscribeToEvents: function() {
        this._moveSeparatorHandler = this.createAction(this._moveSeparator);
        this._startResizingHandler = this.createAction(this._startResizing);
        this._endResizingHandler = this.createAction(this._endResizing);
        eventsEngine.on(domAdapter.getDocument(), addNamespace(pointerEvents.move, MODULE_NAMESPACE), this, this._moveSeparatorHandler);
        eventsEngine.on(this._$parentContainer, addNamespace(pointerEvents.down, MODULE_NAMESPACE), this, this._startResizingHandler);
        eventsEngine.on(this._columnsSeparatorView.element(), addNamespace(pointerEvents.up, MODULE_NAMESPACE), this, this._endResizingHandler);
        eventsEngine.on(domAdapter.getDocument(), addNamespace(pointerEvents.up, MODULE_NAMESPACE), this, this._endResizingHandler)
    },
    _updateColumnsWidthIfNeeded: function(posX) {
        var deltaX;
        var needUpdate = false;
        var nextCellWidth;
        var resizingInfo = this._resizingInfo;
        var columnsController = this._columnsController;
        var visibleColumns = columnsController.getVisibleColumns();
        var columnsSeparatorWidth = this._columnsSeparatorView.width();
        var contentWidth = this._rowsView.contentWidth();
        var isNextColumnMode = isNextColumnResizingMode(this);
        var adaptColumnWidthByRatio = isNextColumnMode && this.option("adaptColumnWidthByRatio") && !this.option("columnAutoWidth");
        var minWidth;
        var nextColumn;
        var cellWidth;
        var rtlEnabled = this.option("rtlEnabled");
        var isRtlParentStyle = this._isRtlParentStyle();

        function isPercentWidth(width) {
            return isString(width) && "%" === width.slice(-1)
        }

        function setColumnWidth(column, columnWidth, contentWidth, adaptColumnWidthByRatio) {
            if (column) {
                var oldColumnWidth = column.width;
                if (oldColumnWidth) {
                    adaptColumnWidthByRatio = isPercentWidth(oldColumnWidth)
                }
                if (adaptColumnWidthByRatio) {
                    columnsController.columnOption(column.index, "visibleWidth", columnWidth);
                    columnsController.columnOption(column.index, "width", (columnWidth / contentWidth * 100).toFixed(3) + "%")
                } else {
                    columnsController.columnOption(column.index, "visibleWidth", null);
                    columnsController.columnOption(column.index, "width", columnWidth)
                }
            }
        }
        deltaX = posX - resizingInfo.startPosX;
        if ((isNextColumnMode || isRtlParentStyle) && rtlEnabled) {
            deltaX = -deltaX
        }
        cellWidth = resizingInfo.currentColumnWidth + deltaX;
        var column = visibleColumns[resizingInfo.currentColumnIndex];
        minWidth = column && column.minWidth || columnsSeparatorWidth;
        needUpdate = cellWidth >= minWidth;
        if (isNextColumnMode) {
            nextCellWidth = resizingInfo.nextColumnWidth - deltaX;
            nextColumn = visibleColumns[resizingInfo.nextColumnIndex];
            minWidth = nextColumn && nextColumn.minWidth || columnsSeparatorWidth;
            needUpdate = needUpdate && nextCellWidth >= minWidth
        }
        if (needUpdate) {
            columnsController.beginUpdate();
            cellWidth = Math.floor(cellWidth);
            contentWidth = function(contentWidth, visibleColumns) {
                var allColumnsHaveWidth = visibleColumns.every(column => column.width);
                if (allColumnsHaveWidth) {
                    var totalPercent = visibleColumns.reduce((sum, column) => {
                        if (isPercentWidth(column.width)) {
                            sum += parseFloat(column.width)
                        }
                        return sum
                    }, 0);
                    if (totalPercent > 100) {
                        contentWidth = contentWidth / totalPercent * 100
                    }
                }
                return contentWidth
            }(contentWidth, visibleColumns);
            setColumnWidth(column, cellWidth, contentWidth, adaptColumnWidthByRatio);
            if (isNextColumnMode) {
                nextCellWidth = Math.floor(nextCellWidth);
                setColumnWidth(nextColumn, nextCellWidth, contentWidth, adaptColumnWidthByRatio)
            } else {
                var columnWidths = this._columnHeadersView.getColumnWidths();
                columnWidths[resizingInfo.currentColumnIndex] = cellWidth;
                var hasScroll = columnWidths.reduce((totalWidth, width) => totalWidth + width, 0) > this._rowsView.contentWidth();
                if (!hasScroll) {
                    var lastColumnIndex = gridCoreUtils.getLastResizableColumnIndex(visibleColumns);
                    if (lastColumnIndex >= 0) {
                        columnsController.columnOption(visibleColumns[lastColumnIndex].index, "visibleWidth", "auto")
                    }
                }
                for (var i = 0; i < columnWidths.length; i++) {
                    if (visibleColumns[i] && visibleColumns[i] !== column && void 0 === visibleColumns[i].width) {
                        columnsController.columnOption(visibleColumns[i].index, "width", columnWidths[i])
                    }
                }
            }
            columnsController.endUpdate();
            if (!isNextColumnMode) {
                this.component.updateDimensions();
                var scrollable = this.component.getScrollable();
                if (scrollable && isRtlParentStyle) {
                    var left = getWidth(scrollable.$content()) - getWidth(scrollable.container()) - this._scrollRight;
                    scrollable.scrollTo({
                        left: left
                    })
                }
            }
        }
        return needUpdate
    },
    _subscribeToCallback: function(callback, handler) {
        callback.add(handler);
        this._subscribesToCallbacks.push({
            callback: callback,
            handler: handler
        })
    },
    _unsubscribeFromCallbacks: function() {
        for (var i = 0; i < this._subscribesToCallbacks.length; i++) {
            var subscribe = this._subscribesToCallbacks[i];
            subscribe.callback.remove(subscribe.handler)
        }
        this._subscribesToCallbacks = []
    },
    _unsubscribes: function() {
        this._unsubscribeFromEvents();
        this._unsubscribeFromCallbacks()
    },
    _init: function() {
        var that = this;
        var generatePointsByColumnsHandler = function() {
            if (!that._isResizing) {
                that.pointsByColumns(null)
            }
        };
        var generatePointsByColumnsScrollHandler = function(offset) {
            if (that._scrollLeft !== offset.left) {
                that._scrollLeft = offset.left;
                that.pointsByColumns(null)
            }
        };
        that._columnsSeparatorView = that.getView("columnsSeparatorView");
        that._columnHeadersView = that.getView("columnHeadersView");
        that._trackerView = that.getView("trackerView");
        that._rowsView = that.getView("rowsView");
        that._columnsController = that.getController("columns");
        that._tablePositionController = that.getController("tablePosition");
        that._$parentContainer = that.component.$element();
        that._draggingHeaderView = that.component.getView("draggingHeaderView");
        that._subscribeToCallback(that._columnHeadersView.renderCompleted, generatePointsByColumnsHandler);
        that._subscribeToCallback(that._columnHeadersView.resizeCompleted, generatePointsByColumnsHandler);
        that._subscribeToCallback(that._columnsSeparatorView.renderCompleted, (function() {
            that._unsubscribeFromEvents();
            that._subscribeToEvents()
        }));
        that._subscribeToCallback(that._rowsView.renderCompleted, (function() {
            that._rowsView.scrollChanged.remove(generatePointsByColumnsScrollHandler);
            that._rowsView.scrollChanged.add(generatePointsByColumnsScrollHandler)
        }));
        var previousScrollbarVisibility = 0 !== that._rowsView.getScrollbarWidth();
        var previousTableHeight = 0;
        that._subscribeToCallback(that.getController("tablePosition").positionChanged, (function(e) {
            if (that._isResizing && !that._rowsView.isResizing) {
                var scrollbarVisibility = 0 !== that._rowsView.getScrollbarWidth();
                if (previousScrollbarVisibility !== scrollbarVisibility || previousTableHeight && previousTableHeight !== e.height) {
                    previousScrollbarVisibility = scrollbarVisibility;
                    previousTableHeight = e.height;
                    that.component.updateDimensions()
                } else {
                    that._rowsView.updateFreeSpaceRowHeight()
                }
            }
            previousTableHeight = e.height
        }))
    },
    optionChanged: function(args) {
        this.callBase(args);
        if ("allowColumnResizing" === args.name) {
            if (args.value) {
                this._init();
                this._subscribeToEvents()
            } else {
                this._unsubscribes()
            }
        }
    },
    isResizing: function() {
        return this._isResizing
    },
    init: function() {
        this._subscribesToCallbacks = [];
        if (allowResizing(this)) {
            this._init()
        }
    },
    pointsByColumns: function(value) {
        if (void 0 !== value) {
            this._pointsByColumns = value
        } else {
            if (!this._pointsByColumns) {
                this._generatePointsByColumns()
            }
            return this._pointsByColumns
        }
    },
    dispose: function() {
        this._unsubscribes();
        this.callBase()
    }
});
var TablePositionViewController = modules.ViewController.inherit({
    update: function(top) {
        var params = {};
        var $element = this._columnHeadersView.element();
        var offset = $element && $element.offset();
        var offsetTop = offset && offset.top || 0;
        var diffOffsetTop = isDefined(top) ? Math.abs(top - offsetTop) : 0;
        var columnsHeadersHeight = this._columnHeadersView ? this._columnHeadersView.getHeight() : 0;
        var scrollBarWidth = this._rowsView.getScrollbarWidth(true);
        var rowsHeight = this._rowsView ? this._rowsView.height() - scrollBarWidth : 0;
        var columnsResizerController = this.component.getController("columnsResizer");
        var draggingHeaderView = this.component.getView("draggingHeaderView");
        params.height = columnsHeadersHeight;
        var isDraggingOrResizing = false !== columnsResizerController.isResizing() || draggingHeaderView.isDragging();
        if (isDraggingOrResizing) {
            params.height += rowsHeight - diffOffsetTop
        }
        if (null !== top && $element && $element.length) {
            params.top = $element[0].offsetTop + diffOffsetTop
        }
        this.positionChanged.fire(params)
    },
    init: function() {
        var that = this;
        that.callBase();
        that._columnHeadersView = this.getView("columnHeadersView");
        that._rowsView = this.getView("rowsView");
        that._pagerView = this.getView("pagerView");
        that._rowsView.resizeCompleted.add((function() {
            if (that.option("allowColumnResizing")) {
                var targetPoint = that.getController("columnsResizer")._targetPoint;
                that.update(targetPoint ? targetPoint.y : null)
            }
        }))
    },
    ctor: function(component) {
        this.callBase(component);
        this.positionChanged = Callbacks()
    }
});
var DraggingHeaderViewController = modules.ViewController.inherit({
    _generatePointsByColumns: function(options) {
        var that = this;
        return gridCoreUtils.getPointsByColumns(options.columnElements, (function(point) {
            return that._pointCreated(point, options.columns, options.targetDraggingPanel.getName(), options.sourceColumn)
        }), options.isVerticalOrientation, options.startColumnIndex)
    },
    _pointCreated: function(point, columns, location, sourceColumn) {
        var targetColumn = columns[point.columnIndex];
        var prevColumn = columns[point.columnIndex - 1];
        switch (location) {
            case "columnChooser":
                return true;
            case "headers":
                return sourceColumn && !sourceColumn.allowReordering || (!targetColumn || !targetColumn.allowReordering) && (!prevColumn || !prevColumn.allowReordering);
            default:
                return 0 === columns.length
        }
    },
    _subscribeToEvents: function(draggingHeader, draggingPanels) {
        var that = this;
        each(draggingPanels, (function(_, draggingPanel) {
            if (draggingPanel) {
                var columns;
                var rowCount = draggingPanel.getRowCount ? draggingPanel.getRowCount() : 1;
                var nameDraggingPanel = draggingPanel.getName();
                var subscribeToEvents = function(index, columnElement) {
                    if (!columnElement) {
                        return
                    }
                    var $columnElement = $(columnElement);
                    var column = columns[index];
                    if (draggingPanel.allowDragging(column, nameDraggingPanel, draggingPanels)) {
                        $columnElement.addClass(that.addWidgetPrefix(HEADERS_DRAG_ACTION_CLASS));
                        eventsEngine.on($columnElement, addNamespace(dragEventStart, MODULE_NAMESPACE), that.createAction((function(args) {
                            var e = args.event;
                            var eventData = getEventData(e);
                            draggingHeader.dragHeader({
                                deltaX: eventData.x - $(e.currentTarget).offset().left,
                                deltaY: eventData.y - $(e.currentTarget).offset().top,
                                sourceColumn: column,
                                index: column.index,
                                columnIndex: index,
                                columnElement: $columnElement,
                                sourceLocation: nameDraggingPanel,
                                draggingPanels: draggingPanels,
                                rowIndex: that._columnsController.getRowIndex(column.index, true)
                            })
                        })));
                        eventsEngine.on($columnElement, addNamespace(dragEventMove, MODULE_NAMESPACE), {
                            that: draggingHeader
                        }, that.createAction(draggingHeader.moveHeader));
                        eventsEngine.on($columnElement, addNamespace(dragEventEnd, MODULE_NAMESPACE), {
                            that: draggingHeader
                        }, that.createAction(draggingHeader.dropHeader))
                    }
                };
                for (var i = 0; i < rowCount; i++) {
                    var columnElements = draggingPanel.getColumnElements(i) || [];
                    if (columnElements.length) {
                        columns = draggingPanel.getColumns(i) || [];
                        each(columnElements, subscribeToEvents)
                    }
                }
            }
        }))
    },
    _unsubscribeFromEvents: function(draggingHeader, draggingPanels) {
        var that = this;
        each(draggingPanels, (function(_, draggingPanel) {
            if (draggingPanel) {
                var columnElements = draggingPanel.getColumnElements() || [];
                each(columnElements, (function(index, columnElement) {
                    var $columnElement = $(columnElement);
                    eventsEngine.off($columnElement, addNamespace(dragEventStart, MODULE_NAMESPACE));
                    eventsEngine.off($columnElement, addNamespace(dragEventMove, MODULE_NAMESPACE));
                    eventsEngine.off($columnElement, addNamespace(dragEventEnd, MODULE_NAMESPACE));
                    $columnElement.removeClass(that.addWidgetPrefix(HEADERS_DRAG_ACTION_CLASS))
                }))
            }
        }))
    },
    _getSeparator: function(targetLocation) {
        return "headers" === targetLocation ? this._columnsSeparatorView : this._blockSeparatorView
    },
    hideSeparators: function(type) {
        var blockSeparator = this._blockSeparatorView;
        var columnsSeparator = this._columnsSeparatorView;
        this._animationColumnIndex = null;
        blockSeparator && blockSeparator.hide();
        "block" !== type && columnsSeparator && columnsSeparator.hide()
    },
    init: function() {
        var that = this;
        that.callBase();
        that._columnsController = that.getController("columns");
        that._columnHeadersView = that.getView("columnHeadersView");
        that._columnsSeparatorView = that.getView("columnsSeparatorView");
        that._draggingHeaderView = that.getView("draggingHeaderView");
        that._rowsView = that.getView("rowsView");
        that._blockSeparatorView = that.getView("blockSeparatorView");
        that._headerPanelView = that.getView("headerPanel");
        that._columnChooserView = that.getView("columnChooserView");
        var subscribeToEvents = function() {
            if (that._draggingHeaderView) {
                var draggingPanels = [that._columnChooserView, that._columnHeadersView, that._headerPanelView];
                that._unsubscribeFromEvents(that._draggingHeaderView, draggingPanels);
                that._subscribeToEvents(that._draggingHeaderView, draggingPanels)
            }
        };
        that._columnHeadersView.renderCompleted.add(subscribeToEvents);
        that._headerPanelView && that._headerPanelView.renderCompleted.add(subscribeToEvents);
        that._columnChooserView && that._columnChooserView.renderCompleted.add(subscribeToEvents)
    },
    allowDrop: function(parameters) {
        return this._columnsController.allowMoveColumn(parameters.sourceColumnIndex, parameters.targetColumnIndex, parameters.sourceLocation, parameters.targetLocation)
    },
    drag: function(parameters) {
        var sourceIndex = parameters.sourceIndex;
        var sourceLocation = parameters.sourceLocation;
        var sourceColumnElement = parameters.sourceColumnElement;
        var headersView = this._columnHeadersView;
        var rowsView = this._rowsView;
        if (sourceColumnElement) {
            sourceColumnElement.css({
                opacity: COLUMN_OPACITY
            });
            if ("headers" === sourceLocation) {
                headersView && headersView.setRowsOpacity(sourceIndex, COLUMN_OPACITY);
                rowsView && rowsView.setRowsOpacity(sourceIndex, COLUMN_OPACITY)
            }
        }
    },
    dock: function(parameters) {
        var that = this;
        var targetColumnIndex = isObject(parameters.targetColumnIndex) ? parameters.targetColumnIndex.columnIndex : parameters.targetColumnIndex;
        var sourceLocation = parameters.sourceLocation;
        var targetLocation = parameters.targetLocation;
        var separator = that._getSeparator(targetLocation);
        var hasTargetVisibleIndex = targetColumnIndex >= 0;
        that._columnHeadersView.element().find("." + HEADER_ROW_CLASS).toggleClass(that.addWidgetPrefix(HEADERS_DROP_HIGHLIGHT_CLASS), "headers" !== sourceLocation && "headers" === targetLocation && !hasTargetVisibleIndex);
        if (separator) {
            if (that.allowDrop(parameters) && hasTargetVisibleIndex) {
                if ("group" === targetLocation || "columnChooser" === targetLocation) {
                    ! function() {
                        if (that._animationColumnIndex !== targetColumnIndex) {
                            that.hideSeparators();
                            separator.element()[parameters.isLast ? "insertAfter" : "insertBefore"](parameters.targetColumnElement);
                            that._animationColumnIndex = targetColumnIndex;
                            separator.show(targetLocation)
                        }
                    }()
                } else {
                    that.hideSeparators("block");
                    that.getController("tablePosition").update(parameters.posY);
                    separator.moveByX(parameters.posX - separator.width());
                    separator.show()
                }
            } else {
                that.hideSeparators()
            }
        }
    },
    drop: function(parameters) {
        var sourceColumnElement = parameters.sourceColumnElement;
        if (sourceColumnElement) {
            sourceColumnElement.css({
                opacity: ""
            });
            this._columnHeadersView.setRowsOpacity(parameters.sourceIndex, "");
            this._rowsView.setRowsOpacity(parameters.sourceIndex, "");
            this._columnHeadersView.element().find("." + HEADER_ROW_CLASS).removeClass(this.addWidgetPrefix(HEADERS_DROP_HIGHLIGHT_CLASS))
        }
        if (this.allowDrop(parameters)) {
            var separator = this._getSeparator(parameters.targetLocation);
            if (separator) {
                separator.hide()
            }
            this._columnsController.moveColumn(parameters.sourceColumnIndex, parameters.targetColumnIndex, parameters.sourceLocation, parameters.targetLocation)
        }
    },
    dispose: function() {
        if (this._draggingHeaderView) {
            this._unsubscribeFromEvents(this._draggingHeaderView, [this._columnChooserView, this._columnHeadersView, this._headerPanelView])
        }
    }
});
export var columnsResizingReorderingModule = {
    views: {
        columnsSeparatorView: ColumnsSeparatorView,
        blockSeparatorView: BlockSeparatorView,
        draggingHeaderView: DraggingHeaderView,
        trackerView: TrackerView
    },
    controllers: {
        draggingHeader: DraggingHeaderViewController,
        tablePosition: TablePositionViewController,
        columnsResizer: ColumnsResizerViewController
    },
    extenders: {
        views: {
            rowsView: {
                _needUpdateRowHeight: function(itemCount) {
                    var wordWrapEnabled = this.option("wordWrapEnabled");
                    var columnsResizerController = this.getController("columnsResizer");
                    var isResizing = columnsResizerController.isResizing();
                    return this.callBase.apply(this, arguments) || itemCount > 0 && wordWrapEnabled && isResizing
                }
            }
        },
        controllers: {
            editorFactory: {
                renderFocusOverlay: function() {
                    if (this.getController("columnsResizer").isResizing()) {
                        return
                    }
                    return this.callBase.apply(this, arguments)
                }
            }
        }
    }
};
