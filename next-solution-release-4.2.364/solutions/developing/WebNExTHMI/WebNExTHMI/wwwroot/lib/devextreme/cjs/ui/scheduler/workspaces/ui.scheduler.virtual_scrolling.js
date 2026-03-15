/**
 * DevExtreme (cjs/ui/scheduler/workspaces/ui.scheduler.virtual_scrolling.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.VirtualScrollingRenderer = exports.VirtualScrollingDispatcher = void 0;
var _dom_adapter = _interopRequireDefault(require("../../../core/dom_adapter"));
var _events_engine = _interopRequireDefault(require("../../../events/core/events_engine"));
var _window = require("../../../core/utils/window");
var _index = require("../../../events/utils/index");
var _type = require("../../../core/utils/type");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
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

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}
var DEFAULT_CELL_HEIGHT = 50;
var MIN_CELL_WIDTH = 1;
var MIN_SCROLL_OFFSET = 10;
var VIRTUAL_APPOINTMENTS_RENDER_TIMEOUT = 15;
var DOCUMENT_SCROLL_EVENT_NAMESPACE = (0, _index.addNamespace)("scroll", "dxSchedulerVirtualScrolling");
var scrollingOrientations = {
    vertical: "vertical",
    horizontal: "horizontal",
    both: "both",
    none: "none"
};
var DefaultScrollingOrientation = scrollingOrientations.both;
var VirtualScrollingDispatcher = function() {
    function VirtualScrollingDispatcher(options) {
        this.options = options;
        if (options) {
            this._rowHeight = this.getCellHeight();
            this._cellWidth = this.getCellWidth();
            this._createVirtualScrollingBase()
        }
    }
    var _proto = VirtualScrollingDispatcher.prototype;
    _proto.setViewOptions = function(options) {
        this.options = options
    };
    _proto.getRenderState = function() {
        var _this$verticalVirtual, _this$horizontalVirtu;
        var verticalRenderState = (null === (_this$verticalVirtual = this.verticalVirtualScrolling) || void 0 === _this$verticalVirtual ? void 0 : _this$verticalVirtual.getRenderState()) || {};
        var horizontalRenderState = (null === (_this$horizontalVirtu = this.horizontalVirtualScrolling) || void 0 === _this$horizontalVirtu ? void 0 : _this$horizontalVirtu.getRenderState()) || {};
        return _extends({}, verticalRenderState, horizontalRenderState)
    };
    _proto.getCellHeight = function() {
        var cellHeight = this.options.getCellHeight();
        var result = cellHeight > 0 ? cellHeight : DEFAULT_CELL_HEIGHT;
        return Math.floor(result)
    };
    _proto.getCellWidth = function() {
        var cellWidth = this.options.getCellWidth();
        var minCellWidth = this.options.getCellMinWidth();
        if (!cellWidth || cellWidth < minCellWidth) {
            cellWidth = minCellWidth
        }
        var result = cellWidth > 0 ? cellWidth : MIN_CELL_WIDTH;
        return Math.floor(result)
    };
    _proto.calculateCoordinatesByDataAndPosition = function(cellData, position, date, isCalculateTime, isVerticalDirectionView) {
        var rowIndex = position.rowIndex,
            columnIndex = position.columnIndex;
        var startDate = cellData.startDate,
            endDate = cellData.endDate,
            allDay = cellData.allDay;
        var timeToScroll = date.getTime();
        var cellStartTime = startDate.getTime();
        var cellEndTime = endDate.getTime();
        var scrollInCell = allDay || !isCalculateTime ? 0 : (timeToScroll - cellStartTime) / (cellEndTime - cellStartTime);
        var cellWidth = this.getCellWidth();
        var rowHeight = this.getCellHeight();
        var top = isVerticalDirectionView ? (rowIndex + scrollInCell) * rowHeight : rowIndex * rowHeight;
        var left = isVerticalDirectionView ? columnIndex * cellWidth : (columnIndex + scrollInCell) * cellWidth;
        if (this.isRTL) {
            left = this.options.getScrollableOuterWidth() - left
        }
        return {
            top: top,
            left: left
        }
    };
    _proto.dispose = function() {
        if (this._onScrollHandler) {
            _events_engine.default.off(this.document, DOCUMENT_SCROLL_EVENT_NAMESPACE, this._onScrollHandler)
        }
    };
    _proto.createVirtualScrolling = function() {
        var isVerticalVirtualScrollingCreated = !!this.verticalVirtualScrolling;
        var isHorizontalVirtualScrollingCreated = !!this.horizontalVirtualScrolling;
        if (this.verticalScrollingAllowed !== isVerticalVirtualScrollingCreated || this.horizontalScrollingAllowed !== isHorizontalVirtualScrollingCreated) {
            this._rowHeight = this.getCellHeight();
            this._cellWidth = this.getCellWidth();
            this._createVirtualScrollingBase()
        }
    };
    _proto._createVirtualScrollingBase = function() {
        if (this.verticalScrollingAllowed) {
            this.verticalVirtualScrolling = new VerticalVirtualScrolling(_extends({}, this.options, {
                viewportHeight: this.viewportHeight,
                rowHeight: this.rowHeight,
                outlineCount: this.outlineCount
            }))
        }
        if (this.horizontalScrollingAllowed) {
            this.horizontalVirtualScrolling = new HorizontalVirtualScrolling(_extends({}, this.options, {
                viewportWidth: this.viewportWidth,
                cellWidth: this.cellWidth,
                outlineCount: this.outlineCount
            }))
        }
    };
    _proto.attachScrollableEvents = function() {
        if ((this.horizontalScrollingAllowed || this.verticalScrollingAllowed) && !this.height) {
            this._attachWindowScroll()
        }
    };
    _proto._attachWindowScroll = function() {
        var _this = this;
        var window = (0, _window.getWindow)();
        this._onScrollHandler = this.options.createAction((function() {
            var scrollX = window.scrollX,
                scrollY = window.scrollY;
            if (scrollX >= MIN_SCROLL_OFFSET || scrollY >= MIN_SCROLL_OFFSET) {
                _this.handleOnScrollEvent({
                    left: scrollX,
                    top: scrollY
                })
            }
        }));
        _events_engine.default.on(this.document, DOCUMENT_SCROLL_EVENT_NAMESPACE, this._onScrollHandler)
    };
    _proto.handleOnScrollEvent = function(scrollPosition) {
        if (scrollPosition) {
            var _this$verticalVirtual2, _this$horizontalVirtu2;
            var left = scrollPosition.left,
                top = scrollPosition.top;
            var verticalStateChanged = (0, _type.isDefined)(top) && (null === (_this$verticalVirtual2 = this.verticalVirtualScrolling) || void 0 === _this$verticalVirtual2 ? void 0 : _this$verticalVirtual2.updateState(top));
            var horizontalStateChanged = (0, _type.isDefined)(left) && (null === (_this$horizontalVirtu2 = this.horizontalVirtualScrolling) || void 0 === _this$horizontalVirtu2 ? void 0 : _this$horizontalVirtu2.updateState(left));
            if (verticalStateChanged || horizontalStateChanged) {
                var _this$options$updateR, _this$options;
                null === (_this$options$updateR = (_this$options = this.options).updateRender) || void 0 === _this$options$updateR ? void 0 : _this$options$updateR.call(_this$options)
            }
        }
    };
    _proto.updateDimensions = function(isForce) {
        var cellHeight = this.getCellHeight();
        var needUpdateVertical = this.verticalScrollingAllowed && cellHeight !== this.rowHeight;
        if ((needUpdateVertical || isForce) && this.verticalVirtualScrolling) {
            this.rowHeight = cellHeight;
            this.verticalVirtualScrolling.viewportSize = this.viewportHeight;
            this.verticalVirtualScrolling.reinitState(cellHeight, isForce)
        }
        var cellWidth = this.getCellWidth();
        var needUpdateHorizontal = this.horizontalScrollingAllowed && cellWidth !== this.cellWidth;
        if ((needUpdateHorizontal || isForce) && this.horizontalVirtualScrolling) {
            this.cellWidth = cellWidth;
            this.horizontalVirtualScrolling.viewportSize = this.viewportWidth;
            this.horizontalVirtualScrolling.reinitState(cellWidth, isForce)
        }
        if (needUpdateVertical || needUpdateHorizontal) {
            var _this$options$updateG, _this$options2;
            null === (_this$options$updateG = (_this$options2 = this.options).updateGrid) || void 0 === _this$options$updateG ? void 0 : _this$options$updateG.call(_this$options2)
        }
    };
    _createClass(VirtualScrollingDispatcher, [{
        key: "isRTL",
        get: function() {
            return this.options.isRTL()
        }
    }, {
        key: "verticalVirtualScrolling",
        get: function() {
            return this._verticalVirtualScrolling
        },
        set: function(value) {
            this._verticalVirtualScrolling = value
        }
    }, {
        key: "horizontalVirtualScrolling",
        get: function() {
            return this._horizontalVirtualScrolling
        },
        set: function(value) {
            this._horizontalVirtualScrolling = value
        }
    }, {
        key: "document",
        get: function() {
            return _dom_adapter.default.getDocument()
        }
    }, {
        key: "height",
        get: function() {
            return this.options.getSchedulerHeight()
        }
    }, {
        key: "width",
        get: function() {
            return this.options.getSchedulerWidth()
        }
    }, {
        key: "rowHeight",
        get: function() {
            return this._rowHeight
        },
        set: function(value) {
            this._rowHeight = value
        }
    }, {
        key: "outlineCount",
        get: function() {
            return this.options.getScrolling().outlineCount
        }
    }, {
        key: "viewportHeight",
        get: function() {
            return this.height ? this.options.getViewHeight() : (0, _window.getWindow)().innerHeight
        }
    }, {
        key: "cellWidth",
        get: function() {
            return this._cellWidth
        },
        set: function(value) {
            this._cellWidth = value
        }
    }, {
        key: "viewportWidth",
        get: function() {
            return this.width ? this.options.getViewWidth() : (0, _window.getWindow)().innerWidth
        }
    }, {
        key: "cellCountInsideTopVirtualRow",
        get: function() {
            var _this$verticalScrolli;
            return (null === (_this$verticalScrolli = this.verticalScrollingState) || void 0 === _this$verticalScrolli ? void 0 : _this$verticalScrolli.virtualItemCountBefore) || 0
        }
    }, {
        key: "cellCountInsideLeftVirtualCell",
        get: function() {
            var _this$horizontalScrol;
            return (null === (_this$horizontalScrol = this.horizontalScrollingState) || void 0 === _this$horizontalScrol ? void 0 : _this$horizontalScrol.virtualItemCountBefore) || 0
        }
    }, {
        key: "cellCountInsideRightVirtualCell",
        get: function() {
            var _this$horizontalScrol2;
            return (null === (_this$horizontalScrol2 = this.horizontalScrollingState) || void 0 === _this$horizontalScrol2 ? void 0 : _this$horizontalScrol2.virtualItemCountAfter) || 0
        }
    }, {
        key: "topVirtualRowsCount",
        get: function() {
            return this.cellCountInsideTopVirtualRow > 0 ? 1 : 0
        }
    }, {
        key: "leftVirtualCellsCount",
        get: function() {
            var virtualItemsCount = !this.isRTL ? this.cellCountInsideLeftVirtualCell : this.cellCountInsideRightVirtualCell;
            return virtualItemsCount > 0 ? 1 : 0
        }
    }, {
        key: "virtualRowOffset",
        get: function() {
            var _this$verticalScrolli2;
            return (null === (_this$verticalScrolli2 = this.verticalScrollingState) || void 0 === _this$verticalScrolli2 ? void 0 : _this$verticalScrolli2.virtualItemSizeBefore) || 0
        }
    }, {
        key: "virtualCellOffset",
        get: function() {
            var _this$horizontalScrol3;
            return (null === (_this$horizontalScrol3 = this.horizontalScrollingState) || void 0 === _this$horizontalScrol3 ? void 0 : _this$horizontalScrol3.virtualItemSizeBefore) || 0
        }
    }, {
        key: "scrollingState",
        get: function() {
            var _this$verticalVirtual3, _this$horizontalVirtu3;
            return {
                vertical: null === (_this$verticalVirtual3 = this.verticalVirtualScrolling) || void 0 === _this$verticalVirtual3 ? void 0 : _this$verticalVirtual3.state,
                horizontal: null === (_this$horizontalVirtu3 = this.horizontalVirtualScrolling) || void 0 === _this$horizontalVirtu3 ? void 0 : _this$horizontalVirtu3.state
            }
        }
    }, {
        key: "verticalScrollingState",
        get: function() {
            return this.scrollingState.vertical
        }
    }, {
        key: "horizontalScrollingState",
        get: function() {
            return this.scrollingState.horizontal
        }
    }, {
        key: "scrollingOrientation",
        get: function() {
            var scrolling = this.options.getScrolling();
            if ("standard" === scrolling.mode) {
                return scrollingOrientations.none
            }
            return scrolling.orientation || DefaultScrollingOrientation
        }
    }, {
        key: "verticalScrollingAllowed",
        get: function() {
            return this.scrollingOrientation === scrollingOrientations.vertical || this.scrollingOrientation === scrollingOrientations.both
        }
    }, {
        key: "horizontalScrollingAllowed",
        get: function() {
            return this.scrollingOrientation === scrollingOrientations.horizontal || this.scrollingOrientation === scrollingOrientations.both
        }
    }]);
    return VirtualScrollingDispatcher
}();
exports.VirtualScrollingDispatcher = VirtualScrollingDispatcher;
var VirtualScrollingBase = function() {
    function VirtualScrollingBase(options) {
        this.options = options;
        this._state = this.defaultState;
        this.viewportSize = options.viewportSize;
        this._itemSize = options.itemSize;
        this._position = -1;
        this._itemSizeChanged = false;
        this.updateState(0)
    }
    var _proto2 = VirtualScrollingBase.prototype;
    _proto2.needUpdateState = function(position) {
        var _this$state = this.state,
            prevPosition = _this$state.prevPosition,
            startIndex = _this$state.startIndex;
        var isFirstInitialization = startIndex < 0;
        if (isFirstInitialization) {
            return true
        }
        var isStartIndexChanged = false;
        if (this._validateAndSavePosition(position)) {
            if (0 === position || position === this.maxScrollPosition) {
                return true
            }
            var currentPosition = prevPosition;
            var currentItemsCount = Math.floor(currentPosition / this.itemSize);
            var itemsCount = Math.floor(position / this.itemSize);
            isStartIndexChanged = Math.abs(currentItemsCount - itemsCount) >= this.outlineCount
        }
        return isStartIndexChanged
    };
    _proto2._validateAndSavePosition = function(position) {
        if (!(0, _type.isDefined)(position)) {
            return false
        }
        var result = this.position !== position;
        this.position = position;
        return result
    };
    _proto2._correctPosition = function(position) {
        return position >= 0 ? Math.min(position, this.maxScrollPosition) : -1
    };
    _proto2.updateState = function(position, isForce) {
        position = this._correctPosition(position);
        if (!this.needUpdateState(position) && !isForce) {
            return false
        }
        var itemsInfoBefore = this._calcItemInfoBefore(position);
        var itemsDeltaBefore = this._calcItemDeltaBefore(itemsInfoBefore);
        var _this$_calcItemInfoAf = this._calcItemInfoAfter(itemsDeltaBefore),
            outlineCountAfter = _this$_calcItemInfoAf.outlineCountAfter,
            virtualItemCountAfter = _this$_calcItemInfoAf.virtualItemCountAfter,
            itemCountWithAfter = _this$_calcItemInfoAf.itemCountWithAfter;
        var virtualItemCountBefore = itemsInfoBefore.virtualItemCountBefore,
            outlineCountBefore = itemsInfoBefore.outlineCountBefore;
        var itemCount = outlineCountBefore + itemCountWithAfter + outlineCountAfter;
        var itemCountBefore = Math.floor(position / this.itemSize);
        this.state.prevPosition = itemCountBefore * this.itemSize;
        this.state.startIndex = itemCountBefore - outlineCountBefore;
        this.state.virtualItemCountBefore = virtualItemCountBefore;
        this.state.outlineCountBefore = outlineCountBefore;
        this.state.itemCount = itemCount;
        this.state.outlineCountAfter = outlineCountAfter;
        this.state.virtualItemCountAfter = virtualItemCountAfter;
        this._updateStateCore();
        return true
    };
    _proto2.reinitState = function(itemSize, isForceUpdate) {
        var position = this.position;
        this.itemSize = itemSize;
        this.updateState(0, isForceUpdate);
        if (position > 0) {
            this.updateState(position, isForceUpdate)
        }
    };
    _proto2._calcItemInfoBefore = function(position) {
        var virtualItemCountBefore = Math.floor(position / this.itemSize);
        var outlineCountBefore = Math.min(virtualItemCountBefore, this.outlineCount);
        virtualItemCountBefore -= outlineCountBefore;
        return {
            virtualItemCountBefore: virtualItemCountBefore,
            outlineCountBefore: outlineCountBefore
        }
    };
    _proto2._calcItemDeltaBefore = function(itemInfoBefore) {
        var virtualItemCountBefore = itemInfoBefore.virtualItemCountBefore,
            outlineCountBefore = itemInfoBefore.outlineCountBefore;
        var totalItemCount = this.getTotalItemCount();
        return totalItemCount - virtualItemCountBefore - outlineCountBefore
    };
    _proto2.getTotalItemCount = function() {
        throw "getTotalItemCount method should be implemented"
    };
    _proto2.getRenderState = function() {
        throw "getRenderState method should be implemented"
    };
    _proto2._calcItemInfoAfter = function(itemsDeltaBefore) {
        var itemCountWithAfter = itemsDeltaBefore >= this.pageSize ? this.pageSize : itemsDeltaBefore;
        var virtualItemCountAfter = itemsDeltaBefore - itemCountWithAfter;
        var outlineCountAfter = virtualItemCountAfter > 0 ? Math.min(virtualItemCountAfter, this.outlineCount) : 0;
        if (virtualItemCountAfter > 0) {
            virtualItemCountAfter -= outlineCountAfter
        }
        return {
            virtualItemCountAfter: virtualItemCountAfter,
            outlineCountAfter: outlineCountAfter,
            itemCountWithAfter: itemCountWithAfter
        }
    };
    _proto2._updateStateCore = function() {
        var state = this.state;
        var virtualItemCountBefore = state.virtualItemCountBefore;
        var virtualItemCountAfter = state.virtualItemCountAfter;
        var outlineCountBefore = state.outlineCountBefore;
        var outlineCountAfter = state.outlineCountAfter;
        var prevVirtualItemSizeBefore = state.virtualItemSizeBefore;
        var prevVirtualItemSizeAfter = state.virtualItemSizeAfter;
        var prevOutlineSizeBefore = state.outlineSizeBefore;
        var prevOutlineSizeAfter = state.outlineSizeAfter;
        var virtualItemSizeBefore = this.itemSize * virtualItemCountBefore;
        var virtualItemSizeAfter = this.itemSize * virtualItemCountAfter;
        var outlineSizeBefore = this.itemSize * outlineCountBefore;
        var outlineSizeAfter = this.itemSize * outlineCountAfter;
        var prevVirtualSizeBefore = prevVirtualItemSizeBefore + prevOutlineSizeBefore;
        var virtualSizeBefore = virtualItemSizeBefore + outlineSizeBefore;
        var prevVirtualSizeAfter = prevVirtualItemSizeAfter + prevOutlineSizeAfter;
        var virtualSizeAfter = virtualItemSizeAfter + outlineSizeAfter;
        var isAppend = prevVirtualSizeBefore < virtualSizeBefore;
        var isPrepend = prevVirtualSizeAfter < virtualSizeAfter;
        var needAddItems = this._itemSizeChanged || isAppend || isPrepend;
        if (needAddItems) {
            this._updateStateVirtualItems(virtualItemSizeBefore, virtualItemSizeAfter)
        }
    };
    _proto2._updateStateVirtualItems = function(virtualItemSizeBefore, virtualItemSizeAfter) {
        var state = this.state;
        state.virtualItemSizeBefore = virtualItemSizeBefore;
        state.virtualItemSizeAfter = virtualItemSizeAfter
    };
    _createClass(VirtualScrollingBase, [{
        key: "itemSize",
        get: function() {
            return this._itemSize
        },
        set: function(value) {
            this._itemSizeChanged = this._itemSize !== value;
            this._itemSize = value
        }
    }, {
        key: "state",
        get: function() {
            return this._state
        },
        set: function(value) {
            this._state = value
        }
    }, {
        key: "startIndex",
        get: function() {
            return this.state.startIndex
        }
    }, {
        key: "pageSize",
        get: function() {
            return Math.ceil(this.viewportSize / this.itemSize)
        }
    }, {
        key: "outlineCount",
        get: function() {
            return (0, _type.isDefined)(this.options.outlineCount) ? this.options.outlineCount : Math.floor(this.pageSize / 2)
        }
    }, {
        key: "groupCount",
        get: function() {
            return this.options.getGroupCount()
        }
    }, {
        key: "isVerticalGrouping",
        get: function() {
            return this.options.isVerticalGrouping()
        }
    }, {
        key: "defaultState",
        get: function() {
            return {
                prevPosition: 0,
                startIndex: -1,
                itemCount: 0,
                virtualItemCountBefore: 0,
                virtualItemCountAfter: 0,
                outlineCountBefore: 0,
                outlineCountAfter: 0,
                virtualItemSizeBefore: 0,
                virtualItemSizeAfter: 0,
                outlineSizeBefore: 0,
                outlineSizeAfter: 0
            }
        }
    }, {
        key: "maxScrollPosition",
        get: function() {
            return this.getTotalItemCount() * this.itemSize - this.viewportSize
        }
    }, {
        key: "position",
        get: function() {
            return this._position
        },
        set: function(value) {
            this._position = value
        }
    }]);
    return VirtualScrollingBase
}();
var VerticalVirtualScrolling = function(_VirtualScrollingBase) {
    _inheritsLoose(VerticalVirtualScrolling, _VirtualScrollingBase);

    function VerticalVirtualScrolling(options) {
        return _VirtualScrollingBase.call(this, _extends({}, options, {
            itemSize: options.rowHeight,
            viewportSize: options.viewportHeight
        })) || this
    }
    var _proto3 = VerticalVirtualScrolling.prototype;
    _proto3.getTotalItemCount = function() {
        return this.options.getTotalRowCount(this.groupCount, this.isVerticalGrouping)
    };
    _proto3.getRenderState = function() {
        return {
            topVirtualRowHeight: this.state.virtualItemSizeBefore,
            bottomVirtualRowHeight: this.state.virtualItemSizeAfter,
            startRowIndex: this.state.startIndex,
            rowCount: this.state.itemCount,
            startIndex: this.state.startIndex
        }
    };
    _createClass(VerticalVirtualScrolling, [{
        key: "prevTopPosition",
        get: function() {
            return this.state.prevPosition
        }
    }, {
        key: "rowCount",
        get: function() {
            return this.state.itemCount
        }
    }, {
        key: "topVirtualRowCount",
        get: function() {
            return this.state.virtualItemCountBefore
        }
    }, {
        key: "bottomVirtualRowCount",
        get: function() {
            return this.state.virtualItemCountAfter
        }
    }]);
    return VerticalVirtualScrolling
}(VirtualScrollingBase);
var HorizontalVirtualScrolling = function(_VirtualScrollingBase2) {
    _inheritsLoose(HorizontalVirtualScrolling, _VirtualScrollingBase2);

    function HorizontalVirtualScrolling(options) {
        return _VirtualScrollingBase2.call(this, _extends({}, options, {
            itemSize: options.cellWidth,
            viewportSize: options.viewportWidth
        })) || this
    }
    var _proto4 = HorizontalVirtualScrolling.prototype;
    _proto4.getTotalItemCount = function() {
        return this.options.getTotalCellCount(this.groupCount, this.isVerticalGrouping)
    };
    _proto4.getRenderState = function() {
        return {
            leftVirtualCellWidth: this.state.virtualItemSizeBefore,
            rightVirtualCellWidth: this.state.virtualItemSizeAfter,
            startCellIndex: this.state.startIndex,
            cellCount: this.state.itemCount,
            cellWidth: this.itemSize
        }
    };
    _proto4._updateStateVirtualItems = function(virtualItemSizeBefore, virtualItemSizeAfter) {
        if (!this.isRTL) {
            _VirtualScrollingBase2.prototype._updateStateVirtualItems.call(this, virtualItemSizeBefore, virtualItemSizeAfter)
        } else {
            var state = this.state;
            state.virtualItemSizeAfter = virtualItemSizeBefore;
            state.virtualItemSizeBefore = virtualItemSizeAfter;
            state.startIndex = this.getTotalItemCount() - this.startIndex - this.state.itemCount
        }
    };
    _createClass(HorizontalVirtualScrolling, [{
        key: "isRTL",
        get: function() {
            return this.options.isRTL()
        }
    }]);
    return HorizontalVirtualScrolling
}(VirtualScrollingBase);
var VirtualScrollingRenderer = function() {
    function VirtualScrollingRenderer(workspace) {
        this._workspace = workspace;
        this._renderAppointmentTimeoutID = null
    }
    var _proto5 = VirtualScrollingRenderer.prototype;
    _proto5.getRenderTimeout = function() {
        return this._workspace.option("isRenovatedAppointments") ? -1 : VIRTUAL_APPOINTMENTS_RENDER_TIMEOUT
    };
    _proto5.updateRender = function() {
        this._renderGrid();
        this._renderAppointments()
    };
    _proto5._renderGrid = function() {
        this.workspace.renderWorkSpace(false)
    };
    _proto5._renderAppointments = function() {
        var _this2 = this;
        var renderTimeout = this.getRenderTimeout();
        if (renderTimeout >= 0) {
            clearTimeout(this._renderAppointmentTimeoutID);
            this._renderAppointmentTimeoutID = setTimeout((function() {
                return _this2.workspace.updateAppointments()
            }), renderTimeout)
        } else {
            this.workspace.updateAppointments()
        }
    };
    _createClass(VirtualScrollingRenderer, [{
        key: "workspace",
        get: function() {
            return this._workspace
        }
    }]);
    return VirtualScrollingRenderer
}();
exports.VirtualScrollingRenderer = VirtualScrollingRenderer;
