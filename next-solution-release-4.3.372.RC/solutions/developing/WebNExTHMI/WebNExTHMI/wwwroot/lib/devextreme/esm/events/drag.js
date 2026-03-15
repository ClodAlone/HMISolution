/**
 * DevExtreme (esm/events/drag.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../core/renderer";
import {
    data as elementData,
    removeData
} from "../core/element_data";
import {
    wrapToArray,
    inArray
} from "../core/utils/array";
import * as iteratorUtils from "../core/utils/iterator";
import {
    contains
} from "../core/utils/dom";
import registerEvent from "./core/event_registrator";
import {
    eventData as eData,
    fireEvent
} from "./utils/index";
import GestureEmitter from "./gesture/emitter.gesture";
import registerEmitter from "./core/emitter_registrator";
var DRAG_START_EVENT = "dxdragstart";
var DRAG_EVENT = "dxdrag";
var DRAG_END_EVENT = "dxdragend";
var DRAG_ENTER_EVENT = "dxdragenter";
var DRAG_LEAVE_EVENT = "dxdragleave";
var DROP_EVENT = "dxdrop";
var DX_DRAG_EVENTS_COUNT_KEY = "dxDragEventsCount";
var knownDropTargets = [];
var knownDropTargetSelectors = [];
var knownDropTargetConfigs = [];
var dropTargetRegistration = {
    setup: function(element, data) {
        var knownDropTarget = -1 !== inArray(element, knownDropTargets);
        if (!knownDropTarget) {
            knownDropTargets.push(element);
            knownDropTargetSelectors.push([]);
            knownDropTargetConfigs.push(data || {})
        }
    },
    add: function(element, handleObj) {
        var index = inArray(element, knownDropTargets);
        this.updateEventsCounter(element, handleObj.type, 1);
        var selector = handleObj.selector;
        if (-1 === inArray(selector, knownDropTargetSelectors[index])) {
            knownDropTargetSelectors[index].push(selector)
        }
    },
    updateEventsCounter: function(element, event, value) {
        if ([DRAG_ENTER_EVENT, DRAG_LEAVE_EVENT, DROP_EVENT].indexOf(event) > -1) {
            var eventsCount = elementData(element, DX_DRAG_EVENTS_COUNT_KEY) || 0;
            elementData(element, DX_DRAG_EVENTS_COUNT_KEY, Math.max(0, eventsCount + value))
        }
    },
    remove: function(element, handleObj) {
        this.updateEventsCounter(element, handleObj.type, -1)
    },
    teardown: function(element) {
        var handlersCount = elementData(element, DX_DRAG_EVENTS_COUNT_KEY);
        if (!handlersCount) {
            var index = inArray(element, knownDropTargets);
            knownDropTargets.splice(index, 1);
            knownDropTargetSelectors.splice(index, 1);
            knownDropTargetConfigs.splice(index, 1);
            removeData(element, DX_DRAG_EVENTS_COUNT_KEY)
        }
    }
};
registerEvent(DRAG_ENTER_EVENT, dropTargetRegistration);
registerEvent(DRAG_LEAVE_EVENT, dropTargetRegistration);
registerEvent(DROP_EVENT, dropTargetRegistration);
var getItemDelegatedTargets = function($element) {
    var dropTargetIndex = inArray($element.get(0), knownDropTargets);
    var dropTargetSelectors = knownDropTargetSelectors[dropTargetIndex].filter(selector => selector);
    var $delegatedTargets = $element.find(dropTargetSelectors.join(", "));
    if (-1 !== inArray(void 0, knownDropTargetSelectors[dropTargetIndex])) {
        $delegatedTargets = $delegatedTargets.add($element)
    }
    return $delegatedTargets
};
var getItemConfig = function($element) {
    var dropTargetIndex = inArray($element.get(0), knownDropTargets);
    return knownDropTargetConfigs[dropTargetIndex]
};
var getItemPosition = function(dropTargetConfig, $element) {
    if (dropTargetConfig.itemPositionFunc) {
        return dropTargetConfig.itemPositionFunc($element)
    } else {
        return $element.offset()
    }
};
var getItemSize = function(dropTargetConfig, $element) {
    if (dropTargetConfig.itemSizeFunc) {
        return dropTargetConfig.itemSizeFunc($element)
    }
    return {
        width: $element.get(0).getBoundingClientRect().width,
        height: $element.get(0).getBoundingClientRect().height
    }
};
var DragEmitter = GestureEmitter.inherit({
    ctor: function(element) {
        this.callBase(element);
        this.direction = "both"
    },
    _init: function(e) {
        this._initEvent = e
    },
    _start: function(e) {
        e = this._fireEvent(DRAG_START_EVENT, this._initEvent);
        this._maxLeftOffset = e.maxLeftOffset;
        this._maxRightOffset = e.maxRightOffset;
        this._maxTopOffset = e.maxTopOffset;
        this._maxBottomOffset = e.maxBottomOffset;
        var dropTargets = wrapToArray(e.targetElements || (null === e.targetElements ? [] : knownDropTargets));
        this._dropTargets = iteratorUtils.map(dropTargets, (function(element) {
            return $(element).get(0)
        }))
    },
    _move: function(e) {
        var eventData = eData(e);
        var dragOffset = this._calculateOffset(eventData);
        e = this._fireEvent(DRAG_EVENT, e, {
            offset: dragOffset
        });
        this._processDropTargets(e);
        if (!e._cancelPreventDefault) {
            e.preventDefault()
        }
    },
    _calculateOffset: function(eventData) {
        return {
            x: this._calculateXOffset(eventData),
            y: this._calculateYOffset(eventData)
        }
    },
    _calculateXOffset: function(eventData) {
        if ("vertical" !== this.direction) {
            var offset = eventData.x - this._startEventData.x;
            return this._fitOffset(offset, this._maxLeftOffset, this._maxRightOffset)
        }
        return 0
    },
    _calculateYOffset: function(eventData) {
        if ("horizontal" !== this.direction) {
            var offset = eventData.y - this._startEventData.y;
            return this._fitOffset(offset, this._maxTopOffset, this._maxBottomOffset)
        }
        return 0
    },
    _fitOffset: function(offset, minOffset, maxOffset) {
        if (null != minOffset) {
            offset = Math.max(offset, -minOffset)
        }
        if (null != maxOffset) {
            offset = Math.min(offset, maxOffset)
        }
        return offset
    },
    _processDropTargets: function(e) {
        var target = this._findDropTarget(e);
        var sameTarget = target === this._currentDropTarget;
        if (!sameTarget) {
            this._fireDropTargetEvent(e, DRAG_LEAVE_EVENT);
            this._currentDropTarget = target;
            this._fireDropTargetEvent(e, DRAG_ENTER_EVENT)
        }
    },
    _fireDropTargetEvent: function(event, eventName) {
        if (!this._currentDropTarget) {
            return
        }
        var eventData = {
            type: eventName,
            originalEvent: event,
            draggingElement: this._$element.get(0),
            target: this._currentDropTarget
        };
        fireEvent(eventData)
    },
    _findDropTarget: function(e) {
        var that = this;
        var result;
        iteratorUtils.each(knownDropTargets, (function(_, target) {
            if (!that._checkDropTargetActive(target)) {
                return
            }
            var $target = $(target);
            iteratorUtils.each(getItemDelegatedTargets($target), (function(_, delegatedTarget) {
                var $delegatedTarget = $(delegatedTarget);
                if (that._checkDropTarget(getItemConfig($target), $delegatedTarget, $(result), e)) {
                    result = delegatedTarget
                }
            }))
        }));
        return result
    },
    _checkDropTargetActive: function(target) {
        var active = false;
        iteratorUtils.each(this._dropTargets, (function(_, activeTarget) {
            active = active || activeTarget === target || contains(activeTarget, target);
            return !active
        }));
        return active
    },
    _checkDropTarget: function(config, $target, $prevTarget, e) {
        var isDraggingElement = $target.get(0) === $(e.target).get(0);
        if (isDraggingElement) {
            return false
        }
        var targetPosition = getItemPosition(config, $target);
        if (e.pageX < targetPosition.left) {
            return false
        }
        if (e.pageY < targetPosition.top) {
            return false
        }
        var targetSize = getItemSize(config, $target);
        if (e.pageX > targetPosition.left + targetSize.width) {
            return false
        }
        if (e.pageY > targetPosition.top + targetSize.height) {
            return false
        }
        if ($prevTarget.length && $prevTarget.closest($target).length) {
            return false
        }
        if (config.checkDropTarget && !config.checkDropTarget($target, e)) {
            return false
        }
        return $target
    },
    _end: function(e) {
        var eventData = eData(e);
        this._fireEvent(DRAG_END_EVENT, e, {
            offset: this._calculateOffset(eventData)
        });
        this._fireDropTargetEvent(e, DROP_EVENT);
        delete this._currentDropTarget
    }
});
registerEmitter({
    emitter: DragEmitter,
    events: [DRAG_START_EVENT, DRAG_EVENT, DRAG_END_EVENT]
});
export {
    DRAG_EVENT as move, DRAG_START_EVENT as start, DRAG_END_EVENT as end, DRAG_ENTER_EVENT as enter, DRAG_LEAVE_EVENT as leave, DROP_EVENT as drop
};
