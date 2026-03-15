/**
 * DevExtreme (esm/ui/draggable.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWidth,
    getHeight,
    getOuterWidth,
    getOuterHeight
} from "../core/utils/size";
import $ from "../core/renderer";
import domAdapter from "../core/dom_adapter";
import {
    getWindow
} from "../core/utils/window";
import {
    getBoundingRect
} from "../core/utils/position";
import eventsEngine from "../events/core/events_engine";
import {
    quadToObject
} from "../core/utils/string";
import registerComponent from "../core/component_registrator";
import {
    locate,
    move
} from "../animation/translator";
import Animator from "./scroll_view/animator";
import {
    dasherize
} from "../core/utils/inflector";
import {
    extend
} from "../core/utils/extend";
import DOMComponent from "../core/dom_component";
import {
    getPublicElement
} from "../core/element";
import {
    addNamespace,
    needSkipEvent
} from "../events/utils/index";
import pointerEvents from "../events/pointer";
import {
    start as dragEventStart,
    move as dragEventMove,
    end as dragEventEnd,
    enter as dragEventEnter,
    leave as dragEventLeave
} from "../events/drag";
import positionUtils from "../animation/position";
import {
    isFunction,
    isObject
} from "../core/utils/type";
import {
    noop,
    splitPair
} from "../core/utils/common";
import {
    value as viewPort
} from "../core/utils/view_port";
import {
    EmptyTemplate
} from "../core/templates/empty_template";
import {
    when,
    fromPromise,
    Deferred
} from "../core/utils/deferred";
var window = getWindow();
var DRAGGABLE = "dxDraggable";
var DRAGSTART_EVENT_NAME = addNamespace(dragEventStart, DRAGGABLE);
var DRAG_EVENT_NAME = addNamespace(dragEventMove, DRAGGABLE);
var DRAGEND_EVENT_NAME = addNamespace(dragEventEnd, DRAGGABLE);
var DRAG_ENTER_EVENT_NAME = addNamespace(dragEventEnter, DRAGGABLE);
var DRAGEND_LEAVE_EVENT_NAME = addNamespace(dragEventLeave, DRAGGABLE);
var POINTERDOWN_EVENT_NAME = addNamespace(pointerEvents.down, DRAGGABLE);
var CLONE_CLASS = "clone";
var targetDraggable;
var sourceDraggable;
var ANONYMOUS_TEMPLATE_NAME = "content";
var getMousePosition = event => ({
    x: event.pageX - $(window).scrollLeft(),
    y: event.pageY - $(window).scrollTop()
});
var GESTURE_COVER_CLASS = "dx-gesture-cover";
var OVERLAY_WRAPPER_CLASS = "dx-overlay-wrapper";
var OVERLAY_CONTENT_CLASS = "dx-overlay-content";
class ScrollHelper {
    constructor(orientation, component) {
        this._preventScroll = true;
        this._component = component;
        if ("vertical" === orientation) {
            this._scrollValue = "scrollTop";
            this._overFlowAttr = "overflowY";
            this._sizeAttr = "height";
            this._scrollSizeProp = "scrollHeight";
            this._clientSizeProp = "clientHeight";
            this._limitProps = {
                start: "top",
                end: "bottom"
            }
        } else {
            this._scrollValue = "scrollLeft";
            this._overFlowAttr = "overflowX";
            this._sizeAttr = "width";
            this._scrollSizeProp = "scrollWidth";
            this._clientSizeProp = "clientWidth";
            this._limitProps = {
                start: "left",
                end: "right"
            }
        }
    }
    updateScrollable(elements, mousePosition) {
        var isScrollableFound = false;
        elements.some(element => {
            var $element = $(element);
            var isTargetOverOverlayWrapper = $element.hasClass(OVERLAY_WRAPPER_CLASS);
            var isTargetOverOverlayContent = $element.hasClass(OVERLAY_CONTENT_CLASS);
            if (isTargetOverOverlayWrapper || isTargetOverOverlayContent) {
                return true
            }
            isScrollableFound = this._trySetScrollable(element, mousePosition);
            return isScrollableFound
        });
        if (!isScrollableFound) {
            this._$scrollableAtPointer = null;
            this._scrollSpeed = 0
        }
    }
    isScrolling() {
        return !!this._scrollSpeed
    }
    isScrollable($element) {
        return ("auto" === $element.css(this._overFlowAttr) || $element.hasClass("dx-scrollable-container")) && $element.prop(this._scrollSizeProp) > ("width" === this._sizeAttr ? getWidth($element) : getHeight($element))
    }
    _trySetScrollable(element, mousePosition) {
        var $element = $(element);
        var distanceToBorders;
        var sensitivity = this._component.option("scrollSensitivity");
        var isScrollable = this.isScrollable($element);
        if (isScrollable) {
            distanceToBorders = this._calculateDistanceToBorders($element, mousePosition);
            if (sensitivity > distanceToBorders[this._limitProps.start]) {
                if (!this._preventScroll) {
                    this._scrollSpeed = -this._calculateScrollSpeed(distanceToBorders[this._limitProps.start]);
                    this._$scrollableAtPointer = $element
                }
            } else if (sensitivity > distanceToBorders[this._limitProps.end]) {
                if (!this._preventScroll) {
                    this._scrollSpeed = this._calculateScrollSpeed(distanceToBorders[this._limitProps.end]);
                    this._$scrollableAtPointer = $element
                }
            } else {
                isScrollable = false;
                this._preventScroll = false
            }
        }
        return isScrollable
    }
    _calculateDistanceToBorders($area, mousePosition) {
        var area = $area.get(0);
        var areaBoundingRect;
        if (area) {
            areaBoundingRect = getBoundingRect(area);
            return {
                left: mousePosition.x - areaBoundingRect.left,
                top: mousePosition.y - areaBoundingRect.top,
                right: areaBoundingRect.right - mousePosition.x,
                bottom: areaBoundingRect.bottom - mousePosition.y
            }
        } else {
            return {}
        }
    }
    _calculateScrollSpeed(distance) {
        var component = this._component;
        var sensitivity = component.option("scrollSensitivity");
        var maxSpeed = component.option("scrollSpeed");
        return Math.ceil(Math.pow((sensitivity - distance) / sensitivity, 2) * maxSpeed)
    }
    scrollByStep() {
        if (this._$scrollableAtPointer && this._scrollSpeed) {
            if (this._$scrollableAtPointer.hasClass("dx-scrollable-container")) {
                var $scrollable = this._$scrollableAtPointer.closest(".dx-scrollable");
                var scrollableInstance = $scrollable.data("dxScrollable") || $scrollable.data("dxScrollView");
                if (scrollableInstance) {
                    var nextScrollPosition = scrollableInstance.scrollOffset()[this._limitProps.start] + this._scrollSpeed;
                    scrollableInstance.scrollTo({
                        [this._limitProps.start]: nextScrollPosition
                    })
                }
            } else {
                var _nextScrollPosition = this._$scrollableAtPointer[this._scrollValue]() + this._scrollSpeed;
                this._$scrollableAtPointer[this._scrollValue](_nextScrollPosition)
            }
            var dragMoveArgs = this._component._dragMoveArgs;
            if (dragMoveArgs) {
                this._component._dragMoveHandler(dragMoveArgs)
            }
        }
    }
    reset() {
        this._$scrollableAtPointer = null;
        this._scrollSpeed = 0;
        this._preventScroll = true
    }
    isOutsideScrollable($scrollable, event) {
        if (!$scrollable) {
            return false
        }
        var scrollableSize = getBoundingRect($scrollable.get(0));
        var start = scrollableSize[this._limitProps.start];
        var size = scrollableSize[this._sizeAttr];
        var mousePosition = getMousePosition(event);
        var location = "width" === this._sizeAttr ? mousePosition.x : mousePosition.y;
        return location < start || location > start + size
    }
}
var ScrollAnimator = Animator.inherit({
    ctor: function(strategy) {
        this.callBase();
        this._strategy = strategy
    },
    _step: function() {
        var horizontalScrollHelper = this._strategy._horizontalScrollHelper;
        var verticalScrollHelper = this._strategy._verticalScrollHelper;
        horizontalScrollHelper && horizontalScrollHelper.scrollByStep();
        verticalScrollHelper && verticalScrollHelper.scrollByStep()
    }
});
var Draggable = DOMComponent.inherit({
    reset: noop,
    dragMove: noop,
    dragEnter: noop,
    dragLeave: noop,
    dragEnd: function(sourceEvent) {
        var sourceDraggable = this._getSourceDraggable();
        sourceDraggable._fireRemoveEvent(sourceEvent);
        return (new Deferred).resolve()
    },
    _fireRemoveEvent: noop,
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            onDragStart: null,
            onDragMove: null,
            onDragEnd: null,
            onDragEnter: null,
            onDragLeave: null,
            onDrop: null,
            immediate: true,
            dragDirection: "both",
            boundary: void 0,
            boundOffset: 0,
            allowMoveByClick: false,
            itemData: null,
            container: void 0,
            dragTemplate: void 0,
            contentTemplate: "content",
            handle: "",
            filter: "",
            clone: false,
            autoScroll: true,
            scrollSpeed: 30,
            scrollSensitivity: 60,
            group: void 0,
            data: void 0
        })
    },
    _setOptionsByReference: function() {
        this.callBase.apply(this, arguments);
        extend(this._optionsByReference, {
            component: true,
            group: true,
            itemData: true,
            data: true
        })
    },
    _init: function() {
        this.callBase();
        this._attachEventHandlers();
        this._scrollAnimator = new ScrollAnimator(this);
        this._horizontalScrollHelper = new ScrollHelper("horizontal", this);
        this._verticalScrollHelper = new ScrollHelper("vertical", this)
    },
    _normalizeCursorOffset: function(offset) {
        if (isObject(offset)) {
            offset = {
                h: offset.x,
                v: offset.y
            }
        }
        offset = splitPair(offset).map(value => parseFloat(value));
        return {
            left: offset[0],
            top: 1 === offset.length ? offset[0] : offset[1]
        }
    },
    _getNormalizedCursorOffset: function(offset, options) {
        if (isFunction(offset)) {
            offset = offset.call(this, options)
        }
        return this._normalizeCursorOffset(offset)
    },
    _calculateElementOffset: function(options) {
        var elementOffset;
        var dragElementOffset;
        var event = options.event;
        var $element = $(options.itemElement);
        var $dragElement = $(options.dragElement);
        var isCloned = this._dragElementIsCloned();
        var cursorOffset = this.option("cursorOffset");
        var normalizedCursorOffset = {
            left: 0,
            top: 0
        };
        var currentLocate = this._initialLocate = locate($dragElement);
        if (isCloned || options.initialOffset || cursorOffset) {
            elementOffset = options.initialOffset || $element.offset();
            if (cursorOffset) {
                normalizedCursorOffset = this._getNormalizedCursorOffset(cursorOffset, options);
                if (isFinite(normalizedCursorOffset.left)) {
                    elementOffset.left = event.pageX
                }
                if (isFinite(normalizedCursorOffset.top)) {
                    elementOffset.top = event.pageY
                }
            }
            dragElementOffset = $dragElement.offset();
            elementOffset.top -= dragElementOffset.top + (normalizedCursorOffset.top || 0) - currentLocate.top;
            elementOffset.left -= dragElementOffset.left + (normalizedCursorOffset.left || 0) - currentLocate.left
        }
        return elementOffset
    },
    _initPosition: function(options) {
        var $dragElement = $(options.dragElement);
        var elementOffset = this._calculateElementOffset(options);
        if (elementOffset) {
            this._move(elementOffset, $dragElement)
        }
        this._startPosition = locate($dragElement)
    },
    _startAnimator: function() {
        if (!this._scrollAnimator.inProgress()) {
            this._scrollAnimator.start()
        }
    },
    _stopAnimator: function() {
        this._scrollAnimator.stop()
    },
    _addWidgetPrefix: function(className) {
        var componentName = this.NAME;
        return dasherize(componentName) + (className ? "-" + className : "")
    },
    _getItemsSelector: function() {
        return this.option("filter") || ""
    },
    _$content: function() {
        var $element = this.$element();
        var $wrapper = $element.children(".dx-template-wrapper");
        return $wrapper.length ? $wrapper : $element
    },
    _attachEventHandlers: function() {
        if (this.option("disabled")) {
            return
        }
        var $element = this._$content();
        var itemsSelector = this._getItemsSelector();
        var allowMoveByClick = this.option("allowMoveByClick");
        var data = {
            direction: this.option("dragDirection"),
            immediate: this.option("immediate"),
            checkDropTarget: ($target, event) => {
                var targetGroup = this.option("group");
                var sourceGroup = this._getSourceDraggable().option("group");
                var $scrollable = this._getScrollable($target);
                if (this._verticalScrollHelper.isOutsideScrollable($scrollable, event) || this._horizontalScrollHelper.isOutsideScrollable($scrollable, event)) {
                    return false
                }
                return sourceGroup && sourceGroup === targetGroup
            }
        };
        if (allowMoveByClick) {
            $element = this._getArea();
            eventsEngine.on($element, POINTERDOWN_EVENT_NAME, data, this._pointerDownHandler.bind(this))
        }
        if (">" === itemsSelector[0]) {
            itemsSelector = itemsSelector.slice(1)
        }
        eventsEngine.on($element, DRAGSTART_EVENT_NAME, itemsSelector, data, this._dragStartHandler.bind(this));
        eventsEngine.on($element, DRAG_EVENT_NAME, data, this._dragMoveHandler.bind(this));
        eventsEngine.on($element, DRAGEND_EVENT_NAME, data, this._dragEndHandler.bind(this));
        eventsEngine.on($element, DRAG_ENTER_EVENT_NAME, data, this._dragEnterHandler.bind(this));
        eventsEngine.on($element, DRAGEND_LEAVE_EVENT_NAME, data, this._dragLeaveHandler.bind(this))
    },
    _dragElementIsCloned: function() {
        return this._$dragElement && this._$dragElement.hasClass(this._addWidgetPrefix(CLONE_CLASS))
    },
    _getDragTemplateArgs: function($element, $container) {
        return {
            container: getPublicElement($container),
            model: {
                itemData: this.option("itemData"),
                itemElement: getPublicElement($element)
            }
        }
    },
    _createDragElement: function($element) {
        var result = $element;
        var clone = this.option("clone");
        var $container = this._getContainer();
        var template = this.option("dragTemplate");
        if (template) {
            template = this._getTemplate(template);
            result = $("<div>").appendTo($container);
            template.render(this._getDragTemplateArgs($element, result))
        } else if (clone) {
            result = $("<div>").appendTo($container);
            $element.clone().css({
                width: $element.css("width"),
                height: $element.css("height")
            }).appendTo(result)
        }
        return result.toggleClass(this._addWidgetPrefix(CLONE_CLASS), result.get(0) !== $element.get(0)).toggleClass("dx-rtl", this.option("rtlEnabled"))
    },
    _resetDragElement: function() {
        if (this._dragElementIsCloned()) {
            this._$dragElement.remove()
        } else {
            this._toggleDraggingClass(false)
        }
        this._$dragElement = null
    },
    _resetSourceElement: function() {
        this._toggleDragSourceClass(false);
        this._$sourceElement = null
    },
    _detachEventHandlers: function() {
        eventsEngine.off(this._$content(), "." + DRAGGABLE);
        eventsEngine.off(this._getArea(), "." + DRAGGABLE)
    },
    _move: function(position, $element) {
        move($element || this._$dragElement, position)
    },
    _getDraggableElement: function(e) {
        var $sourceElement = this._getSourceElement();
        if ($sourceElement) {
            return $sourceElement
        }
        var allowMoveByClick = this.option("allowMoveByClick");
        if (allowMoveByClick) {
            return this.$element()
        }
        var $target = $(e && e.target);
        var itemsSelector = this._getItemsSelector();
        if (">" === itemsSelector[0]) {
            var $items = this._$content().find(itemsSelector);
            if (!$items.is($target)) {
                $target = $target.closest($items)
            }
        }
        return $target
    },
    _getSourceElement: function() {
        var draggable = this._getSourceDraggable();
        return draggable._$sourceElement
    },
    _pointerDownHandler: function(e) {
        if (needSkipEvent(e)) {
            return
        }
        var position = {};
        var $element = this.$element();
        var dragDirection = this.option("dragDirection");
        if ("horizontal" === dragDirection || "both" === dragDirection) {
            position.left = e.pageX - $element.offset().left + locate($element).left - getWidth($element) / 2
        }
        if ("vertical" === dragDirection || "both" === dragDirection) {
            position.top = e.pageY - $element.offset().top + locate($element).top - getHeight($element) / 2
        }
        this._move(position, $element);
        this._getAction("onDragMove")(this._getEventArgs(e))
    },
    _isValidElement: function(event, $element) {
        var handle = this.option("handle");
        var $target = $(event.originalEvent && event.originalEvent.target);
        if (handle && !$target.closest(handle).length) {
            return false
        }
        if (!$element.length) {
            return false
        }
        return !$element.is(".dx-state-disabled, .dx-state-disabled *")
    },
    _dragStartHandler: function(e) {
        var $element = this._getDraggableElement(e);
        if (!this._isValidElement(e, $element)) {
            e.cancel = true;
            return
        }
        if (this._$sourceElement) {
            return
        }
        var dragStartArgs = this._getDragStartArgs(e, $element);
        this._getAction("onDragStart")(dragStartArgs);
        if (dragStartArgs.cancel) {
            e.cancel = true;
            return
        }
        this.option("itemData", dragStartArgs.itemData);
        this._setSourceDraggable();
        this._$sourceElement = $element;
        var initialOffset = $element.offset();
        var $dragElement = this._$dragElement = this._createDragElement($element);
        this._toggleDraggingClass(true);
        this._toggleDragSourceClass(true);
        this._setGestureCoverCursor($dragElement.children());
        var isFixedPosition = "fixed" === $dragElement.css("position");
        this._initPosition(extend({}, dragStartArgs, {
            dragElement: $dragElement.get(0),
            initialOffset: isFixedPosition && initialOffset
        }));
        var $area = this._getArea();
        var areaOffset = this._getAreaOffset($area);
        var boundOffset = this._getBoundOffset();
        var areaWidth = getOuterWidth($area);
        var areaHeight = getOuterHeight($area);
        var elementWidth = getWidth($dragElement);
        var elementHeight = getHeight($dragElement);
        var startOffset_left = $dragElement.offset().left - areaOffset.left,
            startOffset_top = $dragElement.offset().top - areaOffset.top;
        if ($area.length) {
            e.maxLeftOffset = startOffset_left - boundOffset.left;
            e.maxRightOffset = areaWidth - startOffset_left - elementWidth - boundOffset.right;
            e.maxTopOffset = startOffset_top - boundOffset.top;
            e.maxBottomOffset = areaHeight - startOffset_top - elementHeight - boundOffset.bottom
        }
        if (this.option("autoScroll")) {
            this._startAnimator()
        }
    },
    _getAreaOffset: function($area) {
        var offset = $area && positionUtils.offset($area);
        return offset ? offset : {
            left: 0,
            top: 0
        }
    },
    _toggleDraggingClass: function(value) {
        this._$dragElement && this._$dragElement.toggleClass(this._addWidgetPrefix("dragging"), value)
    },
    _toggleDragSourceClass: function(value, $element) {
        var $sourceElement = $element || this._$sourceElement;
        $sourceElement && $sourceElement.toggleClass(this._addWidgetPrefix("source"), value)
    },
    _setGestureCoverCursor: function($element) {
        $(".".concat(GESTURE_COVER_CLASS)).css("cursor", $element.css("cursor"))
    },
    _getBoundOffset: function() {
        var boundOffset = this.option("boundOffset");
        if (isFunction(boundOffset)) {
            boundOffset = boundOffset.call(this)
        }
        return quadToObject(boundOffset)
    },
    _getArea: function() {
        var area = this.option("boundary");
        if (isFunction(area)) {
            area = area.call(this)
        }
        return $(area)
    },
    _getContainer: function() {
        var container = this.option("container");
        if (void 0 === container) {
            container = viewPort()
        }
        return $(container)
    },
    _dragMoveHandler: function(e, scrollBy) {
        this._dragMoveArgs = e;
        if (!this._$dragElement) {
            e.cancel = true;
            return
        }
        var offset = e.offset;
        var startPosition = this._startPosition;
        this._move({
            left: startPosition.left + offset.x,
            top: startPosition.top + offset.y
        });
        if (!scrollBy) {
            this._updateScrollable(e)
        }
        var eventArgs = this._getEventArgs(e);
        this._getAction("onDragMove")(eventArgs);
        if (true === eventArgs.cancel) {
            return
        }
        var targetDraggable = this._getTargetDraggable();
        targetDraggable.dragMove(e, scrollBy)
    },
    _updateScrollable: function(e) {
        if (this.option("autoScroll")) {
            var mousePosition = getMousePosition(e);
            var allObjects = domAdapter.elementsFromPoint(mousePosition.x, mousePosition.y);
            this._verticalScrollHelper.updateScrollable(allObjects, mousePosition);
            this._horizontalScrollHelper.updateScrollable(allObjects, mousePosition)
        }
    },
    _getScrollable: function($element) {
        var $scrollable;
        $element.parents().toArray().some(parent => {
            var $parent = $(parent);
            if (this._horizontalScrollHelper.isScrollable($parent) || this._verticalScrollHelper.isScrollable($parent)) {
                $scrollable = $parent;
                return true
            }
        });
        return $scrollable
    },
    _defaultActionArgs: function() {
        var args = this.callBase.apply(this, arguments);
        var component = this.option("component");
        if (component) {
            args.component = component;
            args.element = component.element()
        }
        return args
    },
    _getEventArgs: function(e) {
        var sourceDraggable = this._getSourceDraggable();
        var targetDraggable = this._getTargetDraggable();
        return {
            event: e,
            itemData: sourceDraggable.option("itemData"),
            itemElement: getPublicElement(sourceDraggable._$sourceElement),
            fromComponent: sourceDraggable.option("component") || sourceDraggable,
            toComponent: targetDraggable.option("component") || targetDraggable,
            fromData: sourceDraggable.option("data"),
            toData: targetDraggable.option("data")
        }
    },
    _getDragStartArgs: function(e, $itemElement) {
        var args = this._getEventArgs(e);
        return {
            event: args.event,
            itemData: args.itemData,
            itemElement: $itemElement,
            fromData: args.fromData
        }
    },
    _revertItemToInitialPosition: function() {
        !this._dragElementIsCloned() && this._move(this._initialLocate, this._$sourceElement)
    },
    _dragEndHandler: function(e) {
        var d = new Deferred;
        var dragEndEventArgs = this._getEventArgs(e);
        var dropEventArgs = this._getEventArgs(e);
        var targetDraggable = this._getTargetDraggable();
        var needRevertPosition = true;
        try {
            this._getAction("onDragEnd")(dragEndEventArgs)
        } finally {
            when(fromPromise(dragEndEventArgs.cancel)).done(cancel => {
                if (!cancel) {
                    if (targetDraggable !== this) {
                        targetDraggable._getAction("onDrop")(dropEventArgs)
                    }
                    if (!dropEventArgs.cancel) {
                        needRevertPosition = false;
                        when(fromPromise(targetDraggable.dragEnd(dragEndEventArgs))).always(d.resolve);
                        return
                    }
                }
                d.resolve()
            }).fail(d.resolve);
            d.done(() => {
                if (needRevertPosition) {
                    this._revertItemToInitialPosition()
                }
                this.reset();
                targetDraggable.reset();
                this._stopAnimator();
                this._horizontalScrollHelper.reset();
                this._verticalScrollHelper.reset();
                this._resetDragElement();
                this._resetSourceElement();
                this._resetTargetDraggable();
                this._resetSourceDraggable()
            })
        }
    },
    _isTargetOverAnotherDraggable: function(e) {
        var sourceDraggable = this._getSourceDraggable();
        if (this === sourceDraggable) {
            return false
        }
        var $dragElement = sourceDraggable._$dragElement;
        var $sourceDraggableElement = sourceDraggable.$element();
        var $targetDraggableElement = this.$element();
        var mousePosition = getMousePosition(e);
        var elements = domAdapter.elementsFromPoint(mousePosition.x, mousePosition.y);
        var firstWidgetElement = elements.filter(element => {
            var $element = $(element);
            if ($element.hasClass(this._addWidgetPrefix())) {
                return !$element.closest($dragElement).length
            }
        })[0];
        var $sourceElement = this._getSourceElement();
        var isTargetOverItself = firstWidgetElement === $sourceDraggableElement.get(0);
        var isTargetOverNestedDraggable = $(firstWidgetElement).closest($sourceElement).length;
        return !firstWidgetElement || firstWidgetElement === $targetDraggableElement.get(0) && !isTargetOverItself && !isTargetOverNestedDraggable
    },
    _dragEnterHandler: function(e) {
        this._fireDragEnterEvent(e);
        if (this._isTargetOverAnotherDraggable(e)) {
            this._setTargetDraggable()
        }
        var sourceDraggable = this._getSourceDraggable();
        sourceDraggable.dragEnter(e)
    },
    _dragLeaveHandler: function(e) {
        this._fireDragLeaveEvent(e);
        this._resetTargetDraggable();
        if (this !== this._getSourceDraggable()) {
            this.reset()
        }
        var sourceDraggable = this._getSourceDraggable();
        sourceDraggable.dragLeave(e)
    },
    _getAction: function(name) {
        return this["_" + name + "Action"] || this._createActionByOption(name)
    },
    _getAnonymousTemplateName: function() {
        return ANONYMOUS_TEMPLATE_NAME
    },
    _initTemplates: function() {
        if (!this.option("contentTemplate")) {
            return
        }
        this._templateManager.addDefaultTemplates({
            content: new EmptyTemplate
        });
        this.callBase.apply(this, arguments)
    },
    _render: function() {
        this.callBase();
        this.$element().addClass(this._addWidgetPrefix());
        var transclude = this._templateManager.anonymousTemplateName === this.option("contentTemplate");
        var template = this._getTemplateByOption("contentTemplate");
        if (template) {
            $(template.render({
                container: this.element(),
                transclude: transclude
            }))
        }
    },
    _optionChanged: function(args) {
        var name = args.name;
        switch (name) {
            case "onDragStart":
            case "onDragMove":
            case "onDragEnd":
            case "onDrop":
            case "onDragEnter":
            case "onDragLeave":
                this["_" + name + "Action"] = this._createActionByOption(name);
                break;
            case "dragTemplate":
            case "contentTemplate":
            case "container":
            case "clone":
                break;
            case "allowMoveByClick":
            case "dragDirection":
            case "disabled":
            case "boundary":
            case "filter":
            case "immediate":
                this._resetDragElement();
                this._detachEventHandlers();
                this._attachEventHandlers();
                break;
            case "autoScroll":
                this._verticalScrollHelper.reset();
                this._horizontalScrollHelper.reset();
                break;
            case "scrollSensitivity":
            case "scrollSpeed":
            case "boundOffset":
            case "handle":
            case "group":
            case "data":
            case "itemData":
                break;
            default:
                this.callBase(args)
        }
    },
    _getTargetDraggable: function() {
        return targetDraggable || this
    },
    _getSourceDraggable: function() {
        return sourceDraggable || this
    },
    _setTargetDraggable: function() {
        var currentGroup = this.option("group");
        var sourceDraggable = this._getSourceDraggable();
        if (currentGroup && currentGroup === sourceDraggable.option("group")) {
            targetDraggable = this
        }
    },
    _setSourceDraggable: function() {
        sourceDraggable = this
    },
    _resetSourceDraggable: function() {
        sourceDraggable = null
    },
    _resetTargetDraggable: function() {
        targetDraggable = null
    },
    _dispose: function() {
        this.callBase();
        this._detachEventHandlers();
        this._resetDragElement();
        this._resetTargetDraggable();
        this._resetSourceDraggable();
        this._$sourceElement = null;
        this._stopAnimator()
    },
    _fireDragEnterEvent: function(sourceEvent) {
        var args = this._getEventArgs(sourceEvent);
        this._getAction("onDragEnter")(args)
    },
    _fireDragLeaveEvent: function(sourceEvent) {
        var args = this._getEventArgs(sourceEvent);
        this._getAction("onDragLeave")(args)
    }
});
registerComponent(DRAGGABLE, Draggable);
export default Draggable;
