/**
 * DevExtreme (cjs/ui/html_editor/modules/tableResizing.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _size = require("../../../core/utils/size");
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _events_engine = _interopRequireDefault(require("../../../events/core/events_engine"));
var _type = require("../../../core/utils/type");
var _index = require("../../../events/utils/index");
var _resize_callbacks = _interopRequireDefault(require("../../../core/utils/resize_callbacks"));
var _translator = require("../../../animation/translator");
var _position = require("../../../core/utils/position");
var _base = _interopRequireDefault(require("./base"));
var _draggable = _interopRequireDefault(require("../../draggable"));
var _iterator = require("../../../core/utils/iterator");
var _window = require("../../../core/utils/window");
var _extend = require("../../../core/utils/extend");
var _table_helper = require("../utils/table_helper");

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
var DX_COLUMN_RESIZE_FRAME_CLASS = "dx-table-resize-frame";
var DX_COLUMN_RESIZER_CLASS = "dx-htmleditor-column-resizer";
var DX_ROW_RESIZER_CLASS = "dx-htmleditor-row-resizer";
var DEFAULTS = {
    minColumnWidth: 40,
    minRowHeight: 24
};
var DRAGGABLE_ELEMENT_OFFSET = 2;
var ROUGH_OFFSET = 3;
var MODULE_NAMESPACE = "dxHtmlTableResizingModule";
var POINTERDOWN_EVENT = (0, _index.addNamespace)("dxpointerdown", MODULE_NAMESPACE);
var SCROLL_EVENT = (0, _index.addNamespace)("scroll", MODULE_NAMESPACE);
var TableResizingModule = function(_BaseModule) {
    _inheritsLoose(TableResizingModule, _BaseModule);

    function TableResizingModule(quill, options) {
        var _this;
        _this = _BaseModule.call(this, quill, options) || this;
        _this.enabled = !!options.enabled;
        _this._tableResizeFrames = [];
        _this._minColumnWidth = _this._minSizeLimit("minColumnWidth", options.minColumnWidth);
        _this._minRowHeight = _this._minSizeLimit("minRowHeight", options.minRowHeight);
        _this._quillContainer = _this.editorInstance._getQuillContainer();
        _this._tableData = [];
        if (_this.enabled) {
            _this._applyResizing()
        }
        return _this
    }
    var _proto = TableResizingModule.prototype;
    _proto._applyResizing = function(forcedStart) {
        if (forcedStart) {
            this._applyResizingImpl()
        } else {
            this.editorInstance.addContentInitializedCallback(this._applyResizingImpl.bind(this))
        }
        this.addCleanCallback(this.clean.bind(this));
        this._resizeHandlerWithContext = _resize_callbacks.default.add(this._resizeHandler.bind(this))
    };
    _proto._minSizeLimit = function(propertyName, newValue) {
        return (0, _type.isDefined)(newValue) ? Math.max(newValue, 0) : DEFAULTS[propertyName]
    };
    _proto._applyResizingImpl = function() {
        var $tables = this._findTables();
        if ($tables.length) {
            this._fixTablesWidths($tables);
            this._createResizeFrames($tables);
            this._updateFramesPositions();
            this._updateFramesSeparators()
        }
        this._attachEvents()
    };
    _proto._attachEvents = function() {
        _events_engine.default.on(this.editorInstance._getContent(), SCROLL_EVENT, this._updateFramesPositions.bind(this));
        this.quill.on("text-change", this._getQuillTextChangeHandler())
    };
    _proto._detachEvents = function() {
        _events_engine.default.off(this.editorInstance._getContent(), MODULE_NAMESPACE);
        this.quill.off("text-change", this._quillTextChangeHandler)
    };
    _proto._getQuillTextChangeHandler = function(delta, oldContent, source) {
        var _this2 = this;
        return function(delta, oldContent, source) {
            if (_this2._isTableChanging()) {
                var $tables = _this2._findTables();
                _this2._removeResizeFrames();
                if ("api" === source) {
                    _this2._fixTablesWidths($tables)
                }
                _this2._updateTablesColumnsWidth($tables);
                _this2._createResizeFrames($tables);
                _this2._updateFramesPositions();
                _this2._updateFramesSeparators()
            } else {
                _this2._updateFramesPositions();
                if (!_this2._isDragging) {
                    _this2._updateFramesSeparators()
                }
            }
        }
    };
    _proto._getFrameForTable = function($table) {
        var _this$_framesForTable;
        return null === (_this$_framesForTable = this._framesForTables) || void 0 === _this$_framesForTable ? void 0 : _this$_framesForTable.get($table.get(0))
    };
    _proto._resizeHandler = function() {
        var _this3 = this;
        this._windowResizeTimeout = setTimeout((function() {
            var $tables = _this3._findTables();
            (0, _iterator.each)($tables, (function(index, table) {
                var $table = (0, _renderer.default)(table);
                var frame = _this3._tableResizeFrames[index];
                var actualTableWidth = (0, _size.getOuterWidth)($table);
                var lastTableWidth = _this3._tableLastWidth(frame);
                if (Math.abs(actualTableWidth - lastTableWidth) > 1) {
                    _this3._tableLastWidth(frame, actualTableWidth);
                    _this3._updateColumnsWidth($table, index)
                }
            }));
            _this3._updateFramesPositions();
            _this3._updateFramesSeparators()
        }))
    };
    _proto._findTables = function() {
        return (0, _renderer.default)(this._quillContainer).find("table")
    };
    _proto._getWidthStyleValue = function($element) {
        var styleValue = $element[0].style.width;
        return "" !== styleValue ? parseInt(styleValue) : void 0
    };
    _proto._tableLastWidth = function(frame, newValue) {
        if ((0, _type.isDefined)(newValue)) {
            frame.lastWidth = newValue
        } else {
            return null === frame || void 0 === frame ? void 0 : frame.lastWidth
        }
    };
    _proto._fixTablesWidths = function($tables) {
        var _this4 = this;
        (0, _iterator.each)($tables, (function(index, table) {
            var $table = (0, _renderer.default)(table);
            var $columnElements = _this4._getTableDeterminantElements($table, "horizontal");
            if (!_this4._tableResizeFrames[index]) {
                _this4._tableResizeFrames[index] = {
                    lastWidth: void 0
                }
            }
            var frame = _this4._getFrameForTable($table);
            if (!frame) {
                _this4._tableResizeFrames.push({
                    $table: $table
                })
            }
            if (0 === (0, _table_helper.getAutoSizedElements)($table).length) {
                var _this4$_tableLastWidt;
                var _this4$_getColumnElem = _this4._getColumnElementsSum($columnElements),
                    columnsSum = _this4$_getColumnElem.columnsSum;
                (0, _table_helper.unfixTableWidth)($table, {
                    quill: _this4.quill
                });
                var tableWidth = null !== (_this4$_tableLastWidt = _this4._tableLastWidth(frame)) && void 0 !== _this4$_tableLastWidt ? _this4$_tableLastWidt : (0, _size.getOuterWidth)($table);
                if (frame) {
                    _this4._tableLastWidth(frame, Math.max(columnsSum, tableWidth))
                }
            }
        }))
    };
    _proto._createResizeFrames = function($tables) {
        var _this5 = this;
        this._framesForTables = new Map;
        $tables.each((function(index, table) {
            var _this5$_tableResizeFr;
            var $table = (0, _renderer.default)(table);
            var $lastTable = null === (_this5$_tableResizeFr = _this5._tableResizeFrames[index]) || void 0 === _this5$_tableResizeFr ? void 0 : _this5$_tableResizeFr.$table;
            var $tableLastWidth = _this5._tableResizeFrames[index].lastWidth;
            _this5._tableResizeFrames[index] = {
                $frame: _this5._createTableResizeFrame(table),
                $table: $table,
                index: index,
                lastWidth: $lastTable && table === $lastTable.get(0) ? $tableLastWidth : void 0,
                columnsCount: _this5._getTableDeterminantElements($table, "horizontal").length,
                rowsCount: _this5._getTableDeterminantElements($table, "vertical").length
            };
            _this5._framesForTables.set(table, _this5._tableResizeFrames[index])
        }));
        this._tableResizeFrames.length = $tables.length
    };
    _proto._isTableChanging = function() {
        var _this6 = this;
        var $tables = this._findTables();
        var result = false;
        if ($tables.length !== this._tableResizeFrames.length) {
            result = true
        } else {
            (0, _iterator.each)($tables, (function(index, table) {
                var $table = (0, _renderer.default)(table);
                var frame = _this6._tableResizeFrames[index];
                var isColumnsCountChanged = (null === frame || void 0 === frame ? void 0 : frame.columnsCount) !== _this6._getTableDeterminantElements($table, "horizontal").length;
                var isRowCountChanged = (null === frame || void 0 === frame ? void 0 : frame.rowsCount) !== _this6._getTableDeterminantElements($table, "vertical").length;
                if (isColumnsCountChanged || isRowCountChanged) {
                    result = true;
                    return false
                }
            }))
        }
        return result
    };
    _proto._removeResizeFrames = function(clearArray) {
        var _this$_framesForTable2, _this7 = this;
        (0, _iterator.each)(this._tableResizeFrames, (function(index, resizeFrame) {
            if (resizeFrame.$frame) {
                var _resizeFrame$$frame;
                var resizerElementsSelector = ".".concat(DX_COLUMN_RESIZER_CLASS, ", .").concat(DX_ROW_RESIZER_CLASS);
                _this7._detachSeparatorEvents(null === (_resizeFrame$$frame = resizeFrame.$frame) || void 0 === _resizeFrame$$frame ? void 0 : _resizeFrame$$frame.find(resizerElementsSelector));
                resizeFrame.$frame.remove()
            }
        }));
        null === (_this$_framesForTable2 = this._framesForTables) || void 0 === _this$_framesForTable2 ? void 0 : _this$_framesForTable2.clear();
        if (clearArray) {
            this._tableResizeFrames = []
        }
    };
    _proto._detachSeparatorEvents = function($lineSeparators) {
        $lineSeparators.each((function(i, $lineSeparator) {
            _events_engine.default.off($lineSeparator, POINTERDOWN_EVENT)
        }))
    };
    _proto._createTableResizeFrame = function() {
        return (0, _renderer.default)("<div>").addClass(DX_COLUMN_RESIZE_FRAME_CLASS).appendTo(this._quillContainer)
    };
    _proto._updateFramesPositions = function() {
        var _this8 = this;
        (0, _iterator.each)(this._tableResizeFrames, (function(index, tableResizeFrame) {
            _this8._updateFramePosition(tableResizeFrame.$table, tableResizeFrame.$frame)
        }))
    };
    _proto._updateFramePosition = function($table, $frame) {
        var _getBoundingRect = (0, _position.getBoundingRect)($table.get(0)),
            height = _getBoundingRect.height,
            width = _getBoundingRect.width,
            targetTop = _getBoundingRect.top,
            targetLeft = _getBoundingRect.left;
        var _getBoundingRect2 = (0, _position.getBoundingRect)(this.quill.root),
            containerTop = _getBoundingRect2.top,
            containerLeft = _getBoundingRect2.left;
        $frame.css({
            height: height,
            width: width,
            top: targetTop - containerTop,
            left: targetLeft - containerLeft
        });
        (0, _translator.move)($frame, {
            left: 0,
            top: 0
        })
    };
    _proto._updateFramesSeparators = function(direction) {
        var _this9 = this;
        (0, _iterator.each)(this._tableResizeFrames, (function(index, frame) {
            if (direction) {
                _this9._updateFrameSeparators(frame, direction)
            } else {
                _this9._updateFrameSeparators(frame, "vertical");
                _this9._updateFrameSeparators(frame, "horizontal")
            }
        }))
    };
    _proto._isDraggable = function($element) {
        return $element.hasClass("dx-draggable") && $element.is(":visible")
    };
    _proto._removeDraggable = function($currentLineSeparator, lineResizerClass) {
        if (this._isDraggable($currentLineSeparator)) {
            var draggable = (0, _renderer.default)($currentLineSeparator).dxDraggable("instance");
            draggable.dispose();
            (0, _renderer.default)($currentLineSeparator).addClass(lineResizerClass)
        }
    };
    _proto._getDirectionInfo = function(direction) {
        if ("vertical" === direction) {
            return {
                lineResizerClass: DX_ROW_RESIZER_CLASS,
                sizeFunction: function(x) {
                    return (0, _size.getOuterHeight)(x)
                },
                positionCoordinate: "top",
                positionStyleProperty: "height",
                positionCoordinateName: "y"
            }
        } else {
            return {
                lineResizerClass: DX_COLUMN_RESIZER_CLASS,
                sizeFunction: function(x) {
                    return (0, _size.getOuterWidth)(x)
                },
                positionCoordinate: this.editorInstance.option("rtlEnabled") ? "right" : "left",
                positionStyleProperty: "width",
                positionCoordinateName: "x"
            }
        }
    };
    _proto._getSize = function($element, directionInfo) {
        return directionInfo.sizeFunction($element)
    };
    _proto._updateFrameSeparators = function(frame, direction) {
        var $determinantElements = this._getTableDeterminantElements(frame.$table, direction);
        var determinantElementsCount = $determinantElements.length;
        var determinantElementsSeparatorsCount = determinantElementsCount - 1;
        var directionInfo = this._getDirectionInfo(direction);
        var lineSeparators = frame.$frame.find(".".concat(directionInfo.lineResizerClass));
        var styleOptions = {
            transform: "none"
        };
        var currentPosition = 0;
        for (var i = 0; i <= determinantElementsSeparatorsCount; i++) {
            currentPosition += this._getSize($determinantElements.eq(i), directionInfo);
            if (!(0, _type.isDefined)(lineSeparators[i])) {
                lineSeparators[i] = (0, _renderer.default)("<div>").addClass(directionInfo.lineResizerClass).appendTo(frame.$frame).get(0)
            }
            var $currentLineSeparator = (0, _renderer.default)(lineSeparators[i]);
            this._removeDraggable($currentLineSeparator, directionInfo.lineResizerClass);
            styleOptions[directionInfo.positionCoordinate] = currentPosition - DRAGGABLE_ELEMENT_OFFSET;
            (0, _renderer.default)($currentLineSeparator).css(styleOptions);
            var attachSeparatorData = {
                lineSeparator: lineSeparators[i],
                index: i,
                $determinantElements: $determinantElements,
                frame: frame,
                direction: direction
            };
            this._attachColumnSeparatorEvents(attachSeparatorData)
        }
    };
    _proto._getTableDeterminantElements = function($table, direction) {
        if ("vertical" === direction) {
            return $table.find("th:first-child, td:first-child")
        } else {
            return (0, _table_helper.getColumnElements)($table)
        }
    };
    _proto._attachColumnSeparatorEvents = function(options) {
        var _this10 = this;
        _events_engine.default.on(options.lineSeparator, POINTERDOWN_EVENT, (function() {
            _this10._createDraggableElement(options)
        }))
    };
    _proto._dragStartHandler = function(_ref) {
        var $determinantElements = _ref.$determinantElements,
            index = _ref.index,
            frame = _ref.frame,
            direction = _ref.direction,
            lineSeparator = _ref.lineSeparator;
        var directionInfo = this._getDirectionInfo(direction);
        this._isDragging = true;
        this._fixColumnsWidth(frame.$table);
        this._startLineSize = parseInt(this._getSize((0, _renderer.default)($determinantElements[index]), directionInfo));
        this._startTableWidth = (0, _size.getOuterWidth)(frame.$table);
        this._startLineSeparatorPosition = parseInt((0, _renderer.default)(lineSeparator).css(directionInfo.positionCoordinate));
        this._nextLineSize = 0;
        if ($determinantElements[index + 1]) {
            this._nextLineSize = parseInt(this._getSize((0, _renderer.default)($determinantElements[index + 1]), directionInfo))
        } else if ("horizontal" === direction) {
            (0, _table_helper.unfixTableWidth)(frame.$table, {
                quill: this.quill
            })
        }
    };
    _proto._shouldRevertOffset = function(direction) {
        return "horizontal" === direction && this.editorInstance.option("rtlEnabled")
    };
    _proto._isNextColumnWidthEnough = function(nextColumnNewSize, $nextColumnElement, eventOffset) {
        if (!this._nextLineSize) {
            return true
        } else if (nextColumnNewSize >= this._minColumnWidth) {
            var isWidthIncreased = this._nextColumnOffsetLimit ? eventOffset < this._nextColumnOffsetLimit : eventOffset < 0;
            var isWidthLimited = Math.abs(this._getWidthStyleValue($nextColumnElement) - (0, _size.getOuterWidth)($nextColumnElement)) > ROUGH_OFFSET;
            return isWidthIncreased || !isWidthLimited
        }
        return false
    };
    _proto._shouldSetNextColumnWidth = function(nextColumnNewSize) {
        return this._nextLineSize && nextColumnNewSize > 0
    };
    _proto._horizontalDragHandler = function(_ref2) {
        var currentLineNewSize = _ref2.currentLineNewSize,
            directionInfo = _ref2.directionInfo,
            eventOffset = _ref2.eventOffset,
            $determinantElements = _ref2.$determinantElements,
            index = _ref2.index,
            frame = _ref2.frame;
        var nextColumnNewSize = this._nextLineSize && this._nextLineSize - eventOffset;
        var isCurrentColumnWidthEnough = currentLineNewSize >= this._minColumnWidth;
        var $lineElements = (0, _table_helper.getLineElements)(frame.$table, index);
        var $nextLineElements = (0, _table_helper.getLineElements)(frame.$table, index + 1);
        var realWidthDiff = (0, _size.getOuterWidth)($lineElements.eq(0)) - currentLineNewSize;
        if (isCurrentColumnWidthEnough) {
            if (this._isNextColumnWidthEnough(nextColumnNewSize, $determinantElements.eq(index + 1), eventOffset)) {
                (0, _table_helper.setLineElementsFormat)(this, {
                    elements: $lineElements,
                    property: directionInfo.positionStyleProperty,
                    value: currentLineNewSize
                });
                if (this._shouldSetNextColumnWidth(nextColumnNewSize)) {
                    (0, _table_helper.setLineElementsFormat)(this, {
                        elements: $nextLineElements,
                        property: directionInfo.positionStyleProperty,
                        value: nextColumnNewSize
                    })
                }
                var isTableWidthChanged = Math.abs(this._startTableWidth - (0, _size.getOuterWidth)(frame.$table)) < ROUGH_OFFSET;
                var shouldRevertNewValue = Math.abs(realWidthDiff) > ROUGH_OFFSET || !this._nextLineSize && isTableWidthChanged;
                if (shouldRevertNewValue) {
                    (0, _table_helper.setLineElementsFormat)(this, {
                        elements: $lineElements,
                        property: directionInfo.positionStyleProperty,
                        value: (0, _size.getOuterWidth)($lineElements.eq(0))
                    });
                    nextColumnNewSize += currentLineNewSize - (0, _size.getOuterWidth)($lineElements.eq(0));
                    if (this._shouldSetNextColumnWidth(nextColumnNewSize)) {
                        (0, _table_helper.setLineElementsFormat)(this, {
                            elements: $nextLineElements,
                            property: directionInfo.positionStyleProperty,
                            value: nextColumnNewSize
                        })
                    }
                }
            } else {
                this._nextColumnOffsetLimit = this._nextColumnOffsetLimit || eventOffset
            }
        }
        this._$highlightedElement.css(directionInfo.positionCoordinate, this._startLineSeparatorPosition + eventOffset + realWidthDiff + "px")
    };
    _proto._verticalDragHandler = function(_ref3) {
        var currentLineNewSize = _ref3.currentLineNewSize,
            directionInfo = _ref3.directionInfo,
            eventOffset = _ref3.eventOffset,
            $determinantElements = _ref3.$determinantElements,
            index = _ref3.index,
            frame = _ref3.frame;
        var newHeight = Math.max(currentLineNewSize, this._minRowHeight);
        var $lineElements = (0, _table_helper.getLineElements)(frame.$table, index, "vertical");
        (0, _table_helper.setLineElementsFormat)(this, {
            elements: $lineElements,
            property: directionInfo.positionStyleProperty,
            value: newHeight
        });
        var rowHeightDiff = (0, _size.getOuterHeight)($determinantElements.eq(index)) - currentLineNewSize;
        this._$highlightedElement.css(directionInfo.positionCoordinate, this._startLineSeparatorPosition + eventOffset + rowHeightDiff + "px")
    };
    _proto._dragMoveHandler = function(event, _ref4) {
        var $determinantElements = _ref4.$determinantElements,
            index = _ref4.index,
            frame = _ref4.frame,
            direction = _ref4.direction;
        var directionInfo = this._getDirectionInfo(direction);
        var eventOffset = event.offset[directionInfo.positionCoordinateName];
        this.editorInstance._saveValueChangeEvent(event);
        if (this._shouldRevertOffset(direction)) {
            eventOffset = -eventOffset
        }
        var currentLineNewSize = this._startLineSize + eventOffset;
        if ("horizontal" === direction) {
            this._horizontalDragHandler({
                currentLineNewSize: currentLineNewSize,
                directionInfo: directionInfo,
                eventOffset: eventOffset,
                $determinantElements: $determinantElements,
                index: index,
                frame: frame
            })
        } else {
            this._verticalDragHandler({
                currentLineNewSize: currentLineNewSize,
                directionInfo: directionInfo,
                eventOffset: eventOffset,
                $determinantElements: $determinantElements,
                index: index,
                frame: frame
            })
        }
        this._updateFramePosition(frame.$table, frame.$frame)
    };
    _proto._dragEndHandler = function(options) {
        var _this$_$highlightedEl;
        null === (_this$_$highlightedEl = this._$highlightedElement) || void 0 === _this$_$highlightedEl ? void 0 : _this$_$highlightedEl.remove();
        this._isDragging = void 0;
        this._nextColumnOffsetLimit = void 0;
        this._tableLastWidth(options.frame, (0, _size.getOuterWidth)(options.frame.$table));
        this._updateFramesPositions();
        this._updateFramesSeparators()
    };
    _proto._isLastColumnResizing = function(_ref5) {
        var $determinantElements = _ref5.$determinantElements,
            index = _ref5.index;
        return !(0, _type.isDefined)($determinantElements[index + 1])
    };
    _proto._getBoundaryConfig = function(options) {
        var result = {};
        if ("vertical" === options.direction) {
            result.boundary = options.frame.$table;
            result.boundOffset = {
                bottom: (0, _window.hasWindow)() ? -(0, _size.getHeight)((0, _window.getWindow)()) : -(0, _size.getOuterHeight)(this._quillContainer),
                top: 0,
                left: 0,
                right: 0
            }
        } else if (!this._isLastColumnResizing(options)) {
            result.boundary = options.frame.$table
        } else {
            var $content = this.editorInstance._getContent();
            result.boundary = $content;
            result.boundOffset = {
                bottom: 0,
                top: 0,
                left: $content.css("paddingLeft"),
                right: $content.css("paddingRight")
            }
        }
        return result
    };
    _proto._createDraggableElement = function(options) {
        var _this$_$highlightedEl2, _this11 = this;
        var boundaryConfig = this._getBoundaryConfig(options);
        var directionClass = "vertical" === options.direction ? "dx-htmleditor-highlighted-row" : "dx-htmleditor-highlighted-column";
        null === (_this$_$highlightedEl2 = this._$highlightedElement) || void 0 === _this$_$highlightedEl2 ? void 0 : _this$_$highlightedEl2.remove();
        this._$highlightedElement = (0, _renderer.default)("<div>").addClass("".concat(directionClass)).insertAfter((0, _renderer.default)(options.lineSeparator));
        var config = {
            contentTemplate: null,
            allowMoveByClick: false,
            dragDirection: options.direction,
            onDragMove: function(_ref6) {
                _ref6.component;
                var event = _ref6.event;
                _this11._dragMoveHandler(event, options)
            },
            onDragStart: function() {
                _this11._dragStartHandler(options)
            },
            onDragEnd: function() {
                _this11._dragEndHandler(options)
            }
        };
        (0, _extend.extend)(config, boundaryConfig);
        this._currentDraggableElement = this.editorInstance._createComponent(options.lineSeparator, _draggable.default, config)
    };
    _proto._fixColumnsWidth = function($table) {
        var _this12 = this;
        var determinantElements = this._getTableDeterminantElements($table);
        (0, _iterator.each)(determinantElements, (function(index, element) {
            var columnWidth = (0, _size.getOuterWidth)(element);
            var $lineElements = (0, _table_helper.getLineElements)($table, index);
            (0, _table_helper.setLineElementsFormat)(_this12, {
                elements: $lineElements,
                property: "width",
                value: Math.max(columnWidth, _this12._minColumnWidth)
            })
        }))
    };
    _proto._getColumnElementsSum = function(columnElements) {
        var _this13 = this;
        var columnsWidths = [];
        var columnsSum = 0;
        (0, _iterator.each)(columnElements, (function(index, element) {
            var $element = (0, _renderer.default)(element);
            var columnWidth = _this13._getWidthStyleValue($element) || (0, _size.getOuterWidth)($element);
            columnsWidths[index] = Math.max(columnWidth, _this13._minColumnWidth);
            columnsSum += columnsWidths[index]
        }));
        return {
            columnsWidths: columnsWidths,
            columnsSum: columnsSum
        }
    };
    _proto._setColumnsRatioWidth = function(columnElements, ratio, columnsWidths, $table) {
        var _this14 = this;
        (0, _iterator.each)(columnElements, (function(index) {
            var $lineElements = (0, _table_helper.getLineElements)($table, index);
            var resultWidth;
            if (ratio > 0) {
                resultWidth = _this14._minColumnWidth + Math.round((columnsWidths[index] - _this14._minColumnWidth) * ratio)
            } else {
                resultWidth = _this14._minColumnWidth
            }(0, _table_helper.setLineElementsFormat)(_this14, {
                elements: $lineElements,
                property: "width",
                value: resultWidth
            })
        }))
    };
    _proto._updateColumnsWidth = function($table, frameIndex) {
        var determinantElements = this._getTableDeterminantElements($table);
        var frame = this._tableResizeFrames[frameIndex];
        if (!frame) {
            this._tableResizeFrames[frameIndex] = {}
        }
        frame = this._tableResizeFrames[frameIndex];
        var tableWidth = this._tableLastWidth(frame) || (0, _size.getOuterWidth)($table);
        var ratio;
        var _this$_getColumnEleme = this._getColumnElementsSum(determinantElements),
            columnsWidths = _this$_getColumnEleme.columnsWidths,
            columnsSum = _this$_getColumnEleme.columnsSum;
        var minWidthForColumns = determinantElements.length * this._minColumnWidth;
        if (columnsSum > minWidthForColumns) {
            ratio = (tableWidth - minWidthForColumns) / (columnsSum - minWidthForColumns)
        } else {
            ratio = -1
        }
        this._tableLastWidth(frame, ratio > 0 ? tableWidth : minWidthForColumns);
        this._setColumnsRatioWidth(determinantElements, ratio, columnsWidths, $table)
    };
    _proto._updateTablesColumnsWidth = function($tables) {
        var _this15 = this;
        (0, _iterator.each)($tables, (function(index, table) {
            _this15._updateColumnsWidth((0, _renderer.default)(table), index)
        }))
    };
    _proto.option = function(_option, value) {
        if ("tableResizing" === _option) {
            this.handleOptionChangeValue(value);
            return
        }
        if ("enabled" === _option) {
            this.enabled = value;
            value ? this._applyResizing(true) : this.clean()
        } else if (["minColumnWidth", "minRowHeight"].includes(_option)) {
            this["_".concat(_option)] = this._minSizeLimit(_option, value)
        }
    };
    _proto.clean = function() {
        this._removeResizeFrames(true);
        this._detachEvents();
        _resize_callbacks.default.remove(this._resizeHandlerWithContext);
        clearTimeout(this._windowResizeTimeout);
        this._resizeHandlerWithContext = void 0;
        this._isDragging = void 0;
        this._startTableWidth = void 0;
        clearTimeout(this._attachResizerTimeout)
    };
    return TableResizingModule
}(_base.default);
exports.default = TableResizingModule;
module.exports = exports.default;
module.exports.default = exports.default;
