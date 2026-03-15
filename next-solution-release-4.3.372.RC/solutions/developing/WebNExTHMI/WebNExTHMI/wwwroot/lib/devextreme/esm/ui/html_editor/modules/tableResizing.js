/**
 * DevExtreme (esm/ui/html_editor/modules/tableResizing.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getOuterWidth,
    getOuterHeight,
    getHeight
} from "../../../core/utils/size";
import $ from "../../../core/renderer";
import eventsEngine from "../../../events/core/events_engine";
import {
    isDefined
} from "../../../core/utils/type";
import {
    addNamespace
} from "../../../events/utils/index";
import _windowResizeCallbacks from "../../../core/utils/resize_callbacks";
import {
    move
} from "../../../animation/translator";
import {
    getBoundingRect
} from "../../../core/utils/position";
import BaseModule from "./base";
import Draggable from "../../draggable";
import {
    each
} from "../../../core/utils/iterator";
import {
    getWindow,
    hasWindow
} from "../../../core/utils/window";
import {
    extend
} from "../../../core/utils/extend";
import {
    setLineElementsFormat,
    getLineElements,
    getAutoSizedElements,
    getColumnElements,
    unfixTableWidth
} from "../utils/table_helper";
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
var POINTERDOWN_EVENT = addNamespace("dxpointerdown", MODULE_NAMESPACE);
var SCROLL_EVENT = addNamespace("scroll", MODULE_NAMESPACE);
export default class TableResizingModule extends BaseModule {
    constructor(quill, options) {
        super(quill, options);
        this.enabled = !!options.enabled;
        this._tableResizeFrames = [];
        this._minColumnWidth = this._minSizeLimit("minColumnWidth", options.minColumnWidth);
        this._minRowHeight = this._minSizeLimit("minRowHeight", options.minRowHeight);
        this._quillContainer = this.editorInstance._getQuillContainer();
        this._tableData = [];
        if (this.enabled) {
            this._applyResizing()
        }
    }
    _applyResizing(forcedStart) {
        if (forcedStart) {
            this._applyResizingImpl()
        } else {
            this.editorInstance.addContentInitializedCallback(this._applyResizingImpl.bind(this))
        }
        this.addCleanCallback(this.clean.bind(this));
        this._resizeHandlerWithContext = _windowResizeCallbacks.add(this._resizeHandler.bind(this))
    }
    _minSizeLimit(propertyName, newValue) {
        return isDefined(newValue) ? Math.max(newValue, 0) : DEFAULTS[propertyName]
    }
    _applyResizingImpl() {
        var $tables = this._findTables();
        if ($tables.length) {
            this._fixTablesWidths($tables);
            this._createResizeFrames($tables);
            this._updateFramesPositions();
            this._updateFramesSeparators()
        }
        this._attachEvents()
    }
    _attachEvents() {
        eventsEngine.on(this.editorInstance._getContent(), SCROLL_EVENT, this._updateFramesPositions.bind(this));
        this.quill.on("text-change", this._getQuillTextChangeHandler())
    }
    _detachEvents() {
        eventsEngine.off(this.editorInstance._getContent(), MODULE_NAMESPACE);
        this.quill.off("text-change", this._quillTextChangeHandler)
    }
    _getQuillTextChangeHandler(delta, oldContent, source) {
        return (delta, oldContent, source) => {
            if (this._isTableChanging()) {
                var $tables = this._findTables();
                this._removeResizeFrames();
                if ("api" === source) {
                    this._fixTablesWidths($tables)
                }
                this._updateTablesColumnsWidth($tables);
                this._createResizeFrames($tables);
                this._updateFramesPositions();
                this._updateFramesSeparators()
            } else {
                this._updateFramesPositions();
                if (!this._isDragging) {
                    this._updateFramesSeparators()
                }
            }
        }
    }
    _getFrameForTable($table) {
        var _this$_framesForTable;
        return null === (_this$_framesForTable = this._framesForTables) || void 0 === _this$_framesForTable ? void 0 : _this$_framesForTable.get($table.get(0))
    }
    _resizeHandler() {
        this._windowResizeTimeout = setTimeout(() => {
            var $tables = this._findTables();
            each($tables, (index, table) => {
                var $table = $(table);
                var frame = this._tableResizeFrames[index];
                var actualTableWidth = getOuterWidth($table);
                var lastTableWidth = this._tableLastWidth(frame);
                if (Math.abs(actualTableWidth - lastTableWidth) > 1) {
                    this._tableLastWidth(frame, actualTableWidth);
                    this._updateColumnsWidth($table, index)
                }
            });
            this._updateFramesPositions();
            this._updateFramesSeparators()
        })
    }
    _findTables() {
        return $(this._quillContainer).find("table")
    }
    _getWidthStyleValue($element) {
        var styleValue = $element[0].style.width;
        return "" !== styleValue ? parseInt(styleValue) : void 0
    }
    _tableLastWidth(frame, newValue) {
        if (isDefined(newValue)) {
            frame.lastWidth = newValue
        } else {
            return null === frame || void 0 === frame ? void 0 : frame.lastWidth
        }
    }
    _fixTablesWidths($tables) {
        each($tables, (index, table) => {
            var $table = $(table);
            var $columnElements = this._getTableDeterminantElements($table, "horizontal");
            if (!this._tableResizeFrames[index]) {
                this._tableResizeFrames[index] = {
                    lastWidth: void 0
                }
            }
            var frame = this._getFrameForTable($table);
            if (!frame) {
                this._tableResizeFrames.push({
                    $table: $table
                })
            }
            if (0 === getAutoSizedElements($table).length) {
                var _this$_tableLastWidth;
                var {
                    columnsSum: columnsSum
                } = this._getColumnElementsSum($columnElements);
                unfixTableWidth($table, {
                    quill: this.quill
                });
                var tableWidth = null !== (_this$_tableLastWidth = this._tableLastWidth(frame)) && void 0 !== _this$_tableLastWidth ? _this$_tableLastWidth : getOuterWidth($table);
                if (frame) {
                    this._tableLastWidth(frame, Math.max(columnsSum, tableWidth))
                }
            }
        })
    }
    _createResizeFrames($tables) {
        this._framesForTables = new Map;
        $tables.each((index, table) => {
            var _this$_tableResizeFra;
            var $table = $(table);
            var $lastTable = null === (_this$_tableResizeFra = this._tableResizeFrames[index]) || void 0 === _this$_tableResizeFra ? void 0 : _this$_tableResizeFra.$table;
            var $tableLastWidth = this._tableResizeFrames[index].lastWidth;
            this._tableResizeFrames[index] = {
                $frame: this._createTableResizeFrame(table),
                $table: $table,
                index: index,
                lastWidth: $lastTable && table === $lastTable.get(0) ? $tableLastWidth : void 0,
                columnsCount: this._getTableDeterminantElements($table, "horizontal").length,
                rowsCount: this._getTableDeterminantElements($table, "vertical").length
            };
            this._framesForTables.set(table, this._tableResizeFrames[index])
        });
        this._tableResizeFrames.length = $tables.length
    }
    _isTableChanging() {
        var $tables = this._findTables();
        var result = false;
        if ($tables.length !== this._tableResizeFrames.length) {
            result = true
        } else {
            each($tables, (index, table) => {
                var $table = $(table);
                var frame = this._tableResizeFrames[index];
                var isColumnsCountChanged = (null === frame || void 0 === frame ? void 0 : frame.columnsCount) !== this._getTableDeterminantElements($table, "horizontal").length;
                var isRowCountChanged = (null === frame || void 0 === frame ? void 0 : frame.rowsCount) !== this._getTableDeterminantElements($table, "vertical").length;
                if (isColumnsCountChanged || isRowCountChanged) {
                    result = true;
                    return false
                }
            })
        }
        return result
    }
    _removeResizeFrames(clearArray) {
        var _this$_framesForTable2;
        each(this._tableResizeFrames, (index, resizeFrame) => {
            if (resizeFrame.$frame) {
                var _resizeFrame$$frame;
                var resizerElementsSelector = ".".concat(DX_COLUMN_RESIZER_CLASS, ", .").concat(DX_ROW_RESIZER_CLASS);
                this._detachSeparatorEvents(null === (_resizeFrame$$frame = resizeFrame.$frame) || void 0 === _resizeFrame$$frame ? void 0 : _resizeFrame$$frame.find(resizerElementsSelector));
                resizeFrame.$frame.remove()
            }
        });
        null === (_this$_framesForTable2 = this._framesForTables) || void 0 === _this$_framesForTable2 ? void 0 : _this$_framesForTable2.clear();
        if (clearArray) {
            this._tableResizeFrames = []
        }
    }
    _detachSeparatorEvents($lineSeparators) {
        $lineSeparators.each((i, $lineSeparator) => {
            eventsEngine.off($lineSeparator, POINTERDOWN_EVENT)
        })
    }
    _createTableResizeFrame() {
        return $("<div>").addClass(DX_COLUMN_RESIZE_FRAME_CLASS).appendTo(this._quillContainer)
    }
    _updateFramesPositions() {
        each(this._tableResizeFrames, (index, tableResizeFrame) => {
            this._updateFramePosition(tableResizeFrame.$table, tableResizeFrame.$frame)
        })
    }
    _updateFramePosition($table, $frame) {
        var {
            height: height,
            width: width,
            top: targetTop,
            left: targetLeft
        } = getBoundingRect($table.get(0));
        var {
            top: containerTop,
            left: containerLeft
        } = getBoundingRect(this.quill.root);
        $frame.css({
            height: height,
            width: width,
            top: targetTop - containerTop,
            left: targetLeft - containerLeft
        });
        move($frame, {
            left: 0,
            top: 0
        })
    }
    _updateFramesSeparators(direction) {
        each(this._tableResizeFrames, (index, frame) => {
            if (direction) {
                this._updateFrameSeparators(frame, direction)
            } else {
                this._updateFrameSeparators(frame, "vertical");
                this._updateFrameSeparators(frame, "horizontal")
            }
        })
    }
    _isDraggable($element) {
        return $element.hasClass("dx-draggable") && $element.is(":visible")
    }
    _removeDraggable($currentLineSeparator, lineResizerClass) {
        if (this._isDraggable($currentLineSeparator)) {
            var draggable = $($currentLineSeparator).dxDraggable("instance");
            draggable.dispose();
            $($currentLineSeparator).addClass(lineResizerClass)
        }
    }
    _getDirectionInfo(direction) {
        if ("vertical" === direction) {
            return {
                lineResizerClass: DX_ROW_RESIZER_CLASS,
                sizeFunction: x => getOuterHeight(x),
                positionCoordinate: "top",
                positionStyleProperty: "height",
                positionCoordinateName: "y"
            }
        } else {
            return {
                lineResizerClass: DX_COLUMN_RESIZER_CLASS,
                sizeFunction: x => getOuterWidth(x),
                positionCoordinate: this.editorInstance.option("rtlEnabled") ? "right" : "left",
                positionStyleProperty: "width",
                positionCoordinateName: "x"
            }
        }
    }
    _getSize($element, directionInfo) {
        return directionInfo.sizeFunction($element)
    }
    _updateFrameSeparators(frame, direction) {
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
            if (!isDefined(lineSeparators[i])) {
                lineSeparators[i] = $("<div>").addClass(directionInfo.lineResizerClass).appendTo(frame.$frame).get(0)
            }
            var $currentLineSeparator = $(lineSeparators[i]);
            this._removeDraggable($currentLineSeparator, directionInfo.lineResizerClass);
            styleOptions[directionInfo.positionCoordinate] = currentPosition - DRAGGABLE_ELEMENT_OFFSET;
            $($currentLineSeparator).css(styleOptions);
            var attachSeparatorData = {
                lineSeparator: lineSeparators[i],
                index: i,
                $determinantElements: $determinantElements,
                frame: frame,
                direction: direction
            };
            this._attachColumnSeparatorEvents(attachSeparatorData)
        }
    }
    _getTableDeterminantElements($table, direction) {
        if ("vertical" === direction) {
            return $table.find("th:first-child, td:first-child")
        } else {
            return getColumnElements($table)
        }
    }
    _attachColumnSeparatorEvents(options) {
        eventsEngine.on(options.lineSeparator, POINTERDOWN_EVENT, () => {
            this._createDraggableElement(options)
        })
    }
    _dragStartHandler(_ref) {
        var {
            $determinantElements: $determinantElements,
            index: index,
            frame: frame,
            direction: direction,
            lineSeparator: lineSeparator
        } = _ref;
        var directionInfo = this._getDirectionInfo(direction);
        this._isDragging = true;
        this._fixColumnsWidth(frame.$table);
        this._startLineSize = parseInt(this._getSize($($determinantElements[index]), directionInfo));
        this._startTableWidth = getOuterWidth(frame.$table);
        this._startLineSeparatorPosition = parseInt($(lineSeparator).css(directionInfo.positionCoordinate));
        this._nextLineSize = 0;
        if ($determinantElements[index + 1]) {
            this._nextLineSize = parseInt(this._getSize($($determinantElements[index + 1]), directionInfo))
        } else if ("horizontal" === direction) {
            unfixTableWidth(frame.$table, {
                quill: this.quill
            })
        }
    }
    _shouldRevertOffset(direction) {
        return "horizontal" === direction && this.editorInstance.option("rtlEnabled")
    }
    _isNextColumnWidthEnough(nextColumnNewSize, $nextColumnElement, eventOffset) {
        if (!this._nextLineSize) {
            return true
        } else if (nextColumnNewSize >= this._minColumnWidth) {
            var isWidthIncreased = this._nextColumnOffsetLimit ? eventOffset < this._nextColumnOffsetLimit : eventOffset < 0;
            var isWidthLimited = Math.abs(this._getWidthStyleValue($nextColumnElement) - getOuterWidth($nextColumnElement)) > ROUGH_OFFSET;
            return isWidthIncreased || !isWidthLimited
        }
        return false
    }
    _shouldSetNextColumnWidth(nextColumnNewSize) {
        return this._nextLineSize && nextColumnNewSize > 0
    }
    _horizontalDragHandler(_ref2) {
        var {
            currentLineNewSize: currentLineNewSize,
            directionInfo: directionInfo,
            eventOffset: eventOffset,
            $determinantElements: $determinantElements,
            index: index,
            frame: frame
        } = _ref2;
        var nextColumnNewSize = this._nextLineSize && this._nextLineSize - eventOffset;
        var isCurrentColumnWidthEnough = currentLineNewSize >= this._minColumnWidth;
        var $lineElements = getLineElements(frame.$table, index);
        var $nextLineElements = getLineElements(frame.$table, index + 1);
        var realWidthDiff = getOuterWidth($lineElements.eq(0)) - currentLineNewSize;
        if (isCurrentColumnWidthEnough) {
            if (this._isNextColumnWidthEnough(nextColumnNewSize, $determinantElements.eq(index + 1), eventOffset)) {
                setLineElementsFormat(this, {
                    elements: $lineElements,
                    property: directionInfo.positionStyleProperty,
                    value: currentLineNewSize
                });
                if (this._shouldSetNextColumnWidth(nextColumnNewSize)) {
                    setLineElementsFormat(this, {
                        elements: $nextLineElements,
                        property: directionInfo.positionStyleProperty,
                        value: nextColumnNewSize
                    })
                }
                var isTableWidthChanged = Math.abs(this._startTableWidth - getOuterWidth(frame.$table)) < ROUGH_OFFSET;
                var shouldRevertNewValue = Math.abs(realWidthDiff) > ROUGH_OFFSET || !this._nextLineSize && isTableWidthChanged;
                if (shouldRevertNewValue) {
                    setLineElementsFormat(this, {
                        elements: $lineElements,
                        property: directionInfo.positionStyleProperty,
                        value: getOuterWidth($lineElements.eq(0))
                    });
                    nextColumnNewSize += currentLineNewSize - getOuterWidth($lineElements.eq(0));
                    if (this._shouldSetNextColumnWidth(nextColumnNewSize)) {
                        setLineElementsFormat(this, {
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
    }
    _verticalDragHandler(_ref3) {
        var {
            currentLineNewSize: currentLineNewSize,
            directionInfo: directionInfo,
            eventOffset: eventOffset,
            $determinantElements: $determinantElements,
            index: index,
            frame: frame
        } = _ref3;
        var newHeight = Math.max(currentLineNewSize, this._minRowHeight);
        var $lineElements = getLineElements(frame.$table, index, "vertical");
        setLineElementsFormat(this, {
            elements: $lineElements,
            property: directionInfo.positionStyleProperty,
            value: newHeight
        });
        var rowHeightDiff = getOuterHeight($determinantElements.eq(index)) - currentLineNewSize;
        this._$highlightedElement.css(directionInfo.positionCoordinate, this._startLineSeparatorPosition + eventOffset + rowHeightDiff + "px")
    }
    _dragMoveHandler(event, _ref4) {
        var {
            $determinantElements: $determinantElements,
            index: index,
            frame: frame,
            direction: direction
        } = _ref4;
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
    }
    _dragEndHandler(options) {
        var _this$_$highlightedEl;
        null === (_this$_$highlightedEl = this._$highlightedElement) || void 0 === _this$_$highlightedEl ? void 0 : _this$_$highlightedEl.remove();
        this._isDragging = void 0;
        this._nextColumnOffsetLimit = void 0;
        this._tableLastWidth(options.frame, getOuterWidth(options.frame.$table));
        this._updateFramesPositions();
        this._updateFramesSeparators()
    }
    _isLastColumnResizing(_ref5) {
        var {
            $determinantElements: $determinantElements,
            index: index
        } = _ref5;
        return !isDefined($determinantElements[index + 1])
    }
    _getBoundaryConfig(options) {
        var result = {};
        if ("vertical" === options.direction) {
            result.boundary = options.frame.$table;
            result.boundOffset = {
                bottom: hasWindow() ? -getHeight(getWindow()) : -getOuterHeight(this._quillContainer),
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
    }
    _createDraggableElement(options) {
        var _this$_$highlightedEl2;
        var boundaryConfig = this._getBoundaryConfig(options);
        var directionClass = "vertical" === options.direction ? "dx-htmleditor-highlighted-row" : "dx-htmleditor-highlighted-column";
        null === (_this$_$highlightedEl2 = this._$highlightedElement) || void 0 === _this$_$highlightedEl2 ? void 0 : _this$_$highlightedEl2.remove();
        this._$highlightedElement = $("<div>").addClass("".concat(directionClass)).insertAfter($(options.lineSeparator));
        var config = {
            contentTemplate: null,
            allowMoveByClick: false,
            dragDirection: options.direction,
            onDragMove: _ref6 => {
                var {
                    component: component,
                    event: event
                } = _ref6;
                this._dragMoveHandler(event, options)
            },
            onDragStart: () => {
                this._dragStartHandler(options)
            },
            onDragEnd: () => {
                this._dragEndHandler(options)
            }
        };
        extend(config, boundaryConfig);
        this._currentDraggableElement = this.editorInstance._createComponent(options.lineSeparator, Draggable, config)
    }
    _fixColumnsWidth($table) {
        var determinantElements = this._getTableDeterminantElements($table);
        each(determinantElements, (index, element) => {
            var columnWidth = getOuterWidth(element);
            var $lineElements = getLineElements($table, index);
            setLineElementsFormat(this, {
                elements: $lineElements,
                property: "width",
                value: Math.max(columnWidth, this._minColumnWidth)
            })
        })
    }
    _getColumnElementsSum(columnElements) {
        var columnsWidths = [];
        var columnsSum = 0;
        each(columnElements, (index, element) => {
            var $element = $(element);
            var columnWidth = this._getWidthStyleValue($element) || getOuterWidth($element);
            columnsWidths[index] = Math.max(columnWidth, this._minColumnWidth);
            columnsSum += columnsWidths[index]
        });
        return {
            columnsWidths: columnsWidths,
            columnsSum: columnsSum
        }
    }
    _setColumnsRatioWidth(columnElements, ratio, columnsWidths, $table) {
        each(columnElements, index => {
            var $lineElements = getLineElements($table, index);
            var resultWidth;
            if (ratio > 0) {
                resultWidth = this._minColumnWidth + Math.round((columnsWidths[index] - this._minColumnWidth) * ratio)
            } else {
                resultWidth = this._minColumnWidth
            }
            setLineElementsFormat(this, {
                elements: $lineElements,
                property: "width",
                value: resultWidth
            })
        })
    }
    _updateColumnsWidth($table, frameIndex) {
        var determinantElements = this._getTableDeterminantElements($table);
        var frame = this._tableResizeFrames[frameIndex];
        if (!frame) {
            this._tableResizeFrames[frameIndex] = {}
        }
        frame = this._tableResizeFrames[frameIndex];
        var tableWidth = this._tableLastWidth(frame) || getOuterWidth($table);
        var ratio;
        var {
            columnsWidths: columnsWidths,
            columnsSum: columnsSum
        } = this._getColumnElementsSum(determinantElements);
        var minWidthForColumns = determinantElements.length * this._minColumnWidth;
        if (columnsSum > minWidthForColumns) {
            ratio = (tableWidth - minWidthForColumns) / (columnsSum - minWidthForColumns)
        } else {
            ratio = -1
        }
        this._tableLastWidth(frame, ratio > 0 ? tableWidth : minWidthForColumns);
        this._setColumnsRatioWidth(determinantElements, ratio, columnsWidths, $table)
    }
    _updateTablesColumnsWidth($tables) {
        each($tables, (index, table) => {
            this._updateColumnsWidth($(table), index)
        })
    }
    option(option, value) {
        if ("tableResizing" === option) {
            this.handleOptionChangeValue(value);
            return
        }
        if ("enabled" === option) {
            this.enabled = value;
            value ? this._applyResizing(true) : this.clean()
        } else if (["minColumnWidth", "minRowHeight"].includes(option)) {
            this["_".concat(option)] = this._minSizeLimit(option, value)
        }
    }
    clean() {
        this._removeResizeFrames(true);
        this._detachEvents();
        _windowResizeCallbacks.remove(this._resizeHandlerWithContext);
        clearTimeout(this._windowResizeTimeout);
        this._resizeHandlerWithContext = void 0;
        this._isDragging = void 0;
        this._startTableWidth = void 0;
        clearTimeout(this._attachResizerTimeout)
    }
}
