/**
 * DevExtreme (esm/ui/overlay/overlay_drag.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    locate,
    move
} from "../../animation/translator";
import domAdapter from "../../core/dom_adapter";
import {
    getOffset,
    getOuterWidth,
    getOuterHeight
} from "../../core/utils/size";
import {
    fitIntoRange
} from "../../core/utils/math";
import {
    isWindow
} from "../../core/utils/type";
import eventsEngine from "../../events/core/events_engine";
import {
    start as dragStartEvent,
    move as dragMoveEvent,
    end as dragEndEvent
} from "../../events/drag";
import {
    addNamespace
} from "../../events/utils/index";
var KEYBOARD_DRAG_STEP = 5;
class OverlayDrag {
    constructor(config) {
        this.init(config)
    }
    init(_ref) {
        var {
            dragEnabled: dragEnabled,
            handle: handle,
            draggableElement: draggableElement,
            positionController: positionController
        } = _ref;
        this._positionController = positionController;
        this._draggableElement = draggableElement;
        this._handle = handle;
        this._dragEnabled = dragEnabled;
        this.unsubscribe();
        if (!dragEnabled) {
            return
        }
        this.subscribe()
    }
    moveDown(e) {
        this._moveTo(KEYBOARD_DRAG_STEP, 0, e)
    }
    moveUp(e) {
        this._moveTo(-KEYBOARD_DRAG_STEP, 0, e)
    }
    moveLeft(e) {
        this._moveTo(0, -KEYBOARD_DRAG_STEP, e)
    }
    moveRight(e) {
        this._moveTo(0, KEYBOARD_DRAG_STEP, e)
    }
    subscribe() {
        var eventNames = this._getEventNames();
        eventsEngine.on(this._handle, eventNames.startEventName, e => {
            this._dragStartHandler(e)
        });
        eventsEngine.on(this._handle, eventNames.updateEventName, e => {
            this._dragUpdateHandler(e)
        });
        eventsEngine.on(this._handle, eventNames.endEventName, e => {
            this._dragEndHandler(e)
        })
    }
    unsubscribe() {
        var eventNames = this._getEventNames();
        eventsEngine.off(this._handle, eventNames.startEventName);
        eventsEngine.off(this._handle, eventNames.updateEventName);
        eventsEngine.off(this._handle, eventNames.endEventName)
    }
    _getEventNames() {
        var startEventName = addNamespace(dragStartEvent, "overlayDrag");
        var updateEventName = addNamespace(dragMoveEvent, "overlayDrag");
        var endEventName = addNamespace(dragEndEvent, "overlayDrag");
        return {
            startEventName: startEventName,
            updateEventName: updateEventName,
            endEventName: endEventName
        }
    }
    _dragStartHandler(e) {
        var allowedOffsets = this._getAllowedOffsets();
        this._prevOffset = {
            x: 0,
            y: 0
        };
        e.targetElements = [];
        e.maxTopOffset = allowedOffsets.top;
        e.maxBottomOffset = allowedOffsets.bottom;
        e.maxLeftOffset = allowedOffsets.left;
        e.maxRightOffset = allowedOffsets.right
    }
    _dragUpdateHandler(e) {
        var targetOffset = {
            top: e.offset.y - this._prevOffset.y,
            left: e.offset.x - this._prevOffset.x
        };
        this._moveByOffset(targetOffset);
        this._prevOffset = e.offset
    }
    _dragEndHandler(event) {
        this._positionController.dragHandled();
        this._positionController.detectVisualPositionChange(event)
    }
    _moveTo(top, left, e) {
        if (!this._dragEnabled) {
            return
        }
        e.preventDefault();
        e.stopPropagation();
        var offset = this._fitOffsetIntoAllowedRange(top, left);
        this._moveByOffset(offset);
        this._dragEndHandler(e)
    }
    _fitOffsetIntoAllowedRange(top, left) {
        var allowedOffsets = this._getAllowedOffsets();
        return {
            top: fitIntoRange(top, -allowedOffsets.top, allowedOffsets.bottom),
            left: fitIntoRange(left, -allowedOffsets.left, allowedOffsets.right)
        }
    }
    _getContainerDimensions() {
        var document = domAdapter.getDocument();
        var container = this._positionController.$dragResizeContainer.get(0);
        var containerWidth = getOuterWidth(container);
        var containerHeight = getOuterHeight(container);
        if (isWindow(container)) {
            containerHeight = Math.max(document.body.clientHeight, containerHeight);
            containerWidth = Math.max(document.body.clientWidth, containerWidth)
        }
        return {
            width: containerWidth,
            height: containerHeight
        }
    }
    _getContainerPosition() {
        var container = this._positionController.$dragResizeContainer.get(0);
        return isWindow(container) ? {
            top: 0,
            left: 0
        } : getOffset(container)
    }
    _getElementPosition() {
        return getOffset(this._draggableElement)
    }
    _getInnerDelta() {
        var containerDimensions = this._getContainerDimensions();
        var elementDimensions = this._getElementDimensions();
        return {
            x: containerDimensions.width - elementDimensions.width,
            y: containerDimensions.height - elementDimensions.height
        }
    }
    _getOuterDelta() {
        var {
            width: width,
            height: height
        } = this._getElementDimensions();
        var outsideDragFactor = this._positionController.outsideDragFactor;
        return {
            x: width * outsideDragFactor,
            y: height * outsideDragFactor
        }
    }
    _getFullDelta() {
        var fullDelta = this._getInnerDelta();
        var outerDelta = this._getOuterDelta();
        return {
            x: fullDelta.x + outerDelta.x,
            y: fullDelta.y + outerDelta.y
        }
    }
    _getElementDimensions() {
        return {
            width: this._draggableElement.offsetWidth,
            height: this._draggableElement.offsetHeight
        }
    }
    _getAllowedOffsets() {
        var fullDelta = this._getFullDelta();
        var isDragAllowed = fullDelta.y >= 0 && fullDelta.x >= 0;
        if (!isDragAllowed) {
            return {
                top: 0,
                bottom: 0,
                left: 0,
                right: 0
            }
        }
        var elementPosition = this._getElementPosition();
        var containerPosition = this._getContainerPosition();
        var outerDelta = this._getOuterDelta();
        return {
            top: elementPosition.top - containerPosition.top + outerDelta.y,
            bottom: -elementPosition.top + containerPosition.top + fullDelta.y,
            left: elementPosition.left - containerPosition.left + outerDelta.x,
            right: -elementPosition.left + containerPosition.left + fullDelta.x
        }
    }
    _moveByOffset(offset) {
        var currentPosition = locate(this._draggableElement);
        var newPosition = {
            left: currentPosition.left + offset.left,
            top: currentPosition.top + offset.top
        };
        move(this._draggableElement, newPosition)
    }
}
export default OverlayDrag;
